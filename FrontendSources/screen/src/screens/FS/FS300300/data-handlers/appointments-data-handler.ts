/* eslint-disable brace-style */
import { AssignmentModel, EventModel, ProjectModel, ResourceModel, ResourceTimeRangeModel, StringHelper, SchedulerProjectModel, ResourceStore, TimeSpan } from "@bryntum/schedulerpro";
import { PXView } from "client-controls";
import { AppointmentEmployeeModel, DatesFilterModel, SearchAppointmentModel } from "../view-models";
import { PeriodKind } from "../scheduler-types";
import { Captions } from "../FS300300";

class TimePeriod {
	start: Date;
	end: Date;
}


export class AppointmentsDataHandler {
	static minimumIntervalMin = 30; // eslint-disable-line @typescript-eslint/no-magic-numbers
	static minimumExistingIntervalMin = 15; // eslint-disable-line @typescript-eslint/no-magic-numbers
	static dndEventId = "dnd_event";

	protected source: AppointmentEmployeeModel[] = [];
	protected sourceBacklog: AppointmentEmployeeModel[] = [];

	protected assignments: AppointmentEmployeeModel[];
	protected assignmentsById: Map<string, AppointmentEmployeeModel>;
	protected timeSlots: { start: Date; end: Date; resourceId: string }[];

	protected unassignedAppointmentById: Map<string, AppointmentEmployeeModel>;

	protected _resources = new Map<string, ResourceModel>();
	public get resources() { return this._resources; }

	protected offWorkTimeRanges: ResourceTimeRangeModel[] = [];
	protected workingHours = new Set<string>();
	protected resourcesToEmptyPeriods = new Map<string, Array<TimePeriod>>();

	protected datesFilter: DatesFilterModel;

	protected generation = 0;
	protected lastTimeSpanId = 1;

	public initializeWith(appointments: AppointmentEmployeeModel[]) {
		// if (this.source.length === 0 && assignments.length === 0) return false;
		this.source = appointments.filter(obj => obj.resourceId !== AppointmentEmployeeModel.unassignedId);
		this.sourceBacklog = appointments.filter(obj => obj.resourceId === AppointmentEmployeeModel.unassignedId);

		return true;
	}

	public mergeDataFrom(appointment: AppointmentEmployeeModel[]) {
		if (appointment.some(obj => obj.resourceId !== AppointmentEmployeeModel.unassignedId)) {
			const replacingAssignments = appointment.filter(obj => this.assignmentsById.has(obj.assignmentID));
			const replacingAssignmentsById = new Map(replacingAssignments.map(obj => [obj.assignmentID, obj]));
			this.source = this.source.filter(obj => !replacingAssignmentsById.has(obj.assignmentID));
			this.source = this.source.concat(appointment);
		}
		else {
			this.sourceBacklog = this.sourceBacklog.filter(obj => obj.appointmentID !== appointment[0].appointmentID);
			this.sourceBacklog = this.sourceBacklog.concat(appointment[0]);
		}
	}

	public removeAppointment(appointmentID: string) {
		this.source = this.source.filter(obj => obj.appointmentID !== appointmentID);
		this.removeFromUnassigned(appointmentID);
	}

	public removeAssignment(assignmentID: string) {
		this.source = this.source.filter(obj => obj.assignmentID !== assignmentID);
	}

	public removeFromUnassigned(appointmentID: string) {
		this.sourceBacklog = this.sourceBacklog.filter(obj => obj.appointmentID !== appointmentID);
		this.unassignedAppointmentById.delete(appointmentID);
	}

	public createProjects(datesFilter: DatesFilterModel, currentSearchAppointment: SearchAppointmentModel, sameGeneration = false) {
		if (!sameGeneration) {
			this.generation ++;
		}

		this.assignments = [...this.source]; // make a copy to be handled by Scheduler control
		const nonEmptyAssignments = this.assignments.filter(obj => obj.appointmentID.length > 0);
		this.assignmentsById = new Map(nonEmptyAssignments.map(obj => [obj.assignmentID, obj]));
		this.unassignedAppointmentById = new Map(this.sourceBacklog.map(obj => [obj.appointmentID, obj]));
		this.datesFilter = datesFilter;

		this.timeSlots = this.assignments
			.filter(obj => obj.FSTimeSlot__TimeStart?.value != null && obj.FSTimeSlot__TimeEnd?.value != null)
			.map(obj => ({start: new Date(obj.FSTimeSlot__TimeStart.value), end: new Date(obj.FSTimeSlot__TimeEnd.value), resourceId: obj.resourceId}))
			.filter(obj => obj.start < obj.end);

		this._resources = this.getResources(currentSearchAppointment);
		this.offWorkTimeRanges = this.getOffWorkTimeRanges(datesFilter);
		this.resourcesToEmptyPeriods = this.getResourcesToEmptyPeriods();
		this.workingHours = this.getWorkingHours();

		const mainProject = new ProjectModel({
			resources: [...this._resources.values()],
			events: this.getEvents(),
			assignments: this.getAssignments(),
			resourceTimeRanges: this.getOffWorkTimeRanges(datesFilter),
			timeRanges: this.getTimeProjectionRange(),
		});

		const [periodStartDate, periodEndDate] = AppointmentsDataHandler.getPeriodStartEndDates(datesFilter);
		const backlogProject = new ProjectModel({
			resources: [new ResourceModel({
				id: AppointmentEmployeeModel.unassignedId,
				name: Captions.Unassigned
			})],
			events: [...this.sourceBacklog.values()].map(obj => new EventModel({
				id: obj.appointmentID,
				startDate: obj.dateTimeBegin,
				endDate: this.getAdjustedEventEndTime(obj),
				name: StringHelper.xss`${obj.caption}`,
				cls: getUnassignedAppointmentClass(obj),
			})),
			assignments: [...this.sourceBacklog.values()].map(obj => new AssignmentModel({
				id: obj.assignmentID,
				eventId: obj.appointmentID,
				resourceId: AppointmentEmployeeModel.unassignedId,
			})),
			resourceTimeRanges: [new ResourceTimeRangeModel({
				id: 0,
				resourceId: AppointmentEmployeeModel.unassignedId,
				timeRangeColor: 'blue',
				startDate: periodStartDate,
				endDate: periodEndDate
			})],
			timeRanges: this.getTimeProjectionRange(),
		});

		function getUnassignedAppointmentClass(assignment: AppointmentEmployeeModel) {
			if (assignment.isLocked) return "qp-sch-locked";

			const confirmed = assignment.isValidatedByDispatcher || assignment.isConfirmed;
			const cls = `qp-sch-not-assigned ${confirmed ? "" : "qp-sch-not-confirmed"}`;
			return cls;
		}

		return [backlogProject, mainProject];
	}


	public isWorkingTime(start: Date, end: Date) {
		const startHour = new Date(start);
		startHour.setMinutes(0, 0, 0);
		for (let hour = startHour; hour < end; hour.setHours(hour.getHours() + 1)) {
			if (this.workingHours.has(hour.toISOString())) {
				return true;
			}
		}
		return false;
	}

	public getWorkingHoursSignature() {
		const [start, end] = AppointmentsDataHandler.getPeriodStartEndDates(this.datesFilter);
		const startHour = new Date(start);
		let signature = "";
		startHour.setMinutes(0, 0, 0);
		for (let hour = startHour; hour < end; hour.setHours(hour.getHours() + 1)) {
			if (this.workingHours.has(hour.toISOString())) {
				signature ??= hour.toISOString();
				signature += "1";
			} else if (signature) {
				signature += "0";
			}
		}
		return signature;
	}

	public getAssignment(id: string) {
		const normalizedId = this.getPartialId(id);
		return this.assignmentsById.get(normalizedId);
	}

	public getAssignmentsByAppointmentId(id: string) {
		const normalizedId = this.getPartialId(id);
		// it's not most effecient (O(N) vs O(1)), but that's efficient enough for use in the UI, and it's easier to maintain
		return this.assignments.filter(obj => obj.appointmentID === normalizedId);
	}

	public getAssignmentByAppointmentAndResource(appointmentId: string, resourseId: string) {
		const normalizedId = AppointmentEmployeeModel.getAssignmentId(this.getPartialId(appointmentId), resourseId);
		return this.assignmentsById.get(normalizedId);
	}

	public getUnassignedAppointment(id: string) {
		return this.unassignedAppointmentById?.get(id);
	}

	public getAppointment(id: string) {
		const unassigned = this.getUnassignedAppointment(id);
		if (unassigned) return unassigned;
		return this.getAssignmentsByAppointmentId(id)[0];
	}

	public getFullId(id: string) {
		return `${id}=${this.generation}`;
	}
	public getPartialId(id: string) {
		return id.indexOf("=") > 0 ? id.substring(0, id.indexOf("=")) : id;
	}

	public findEmptyTimePeriod(resourceId: string, lengthMS: number, rangeFrom: Date, rangeTo: Date) {
		if (resourceId == null) return null;

		const emptyPeriods = this.resourcesToEmptyPeriods.get(resourceId)
			.map(x => ({
				start: normalizeTime(x.start > rangeFrom ? x.start : rangeFrom),
				end: x.end <= rangeTo ? x.end : rangeTo
			}));

		const timePeriod = emptyPeriods?.find(period => period.end.getTime() - period.start.getTime() >= lengthMS);
		return timePeriod?.start;

		function normalizeTime(time: Date) {
			const timeNormalized = new Date(time);
			timeNormalized.setMinutes(Math.ceil(time.getMinutes() / AppointmentsDataHandler.minimumIntervalMin) * AppointmentsDataHandler.minimumIntervalMin, 0, 0);
			return timeNormalized;
		}
	}

	public static getPeriodStartEndDates(datesFilter: DatesFilterModel) {
		const date = new Date(datesFilter.DateSelected.value);
		let start: Date;
		let end: Date;

		switch (datesFilter.PeriodKind.value) {
			case PeriodKind.Day:
				start = date;
				end = new Date(start);
				end.setDate(end.getDate() + 1);
				break;

			case PeriodKind.Week:
				start = getStartOfWeek(date, 1);
				end = new Date(start);
				end.setDate(end.getDate() + 7); // eslint-disable-line @typescript-eslint/no-magic-numbers
				break;

			case PeriodKind.Month:
				start = new Date(date.getFullYear(), date.getMonth(), 1);
				end = new Date(date.getFullYear(), date.getMonth() + 1, 1);
				break;
		}

		start = start?.clearHoursUTC();
		end = end?.clearHoursUTC();

		return [start, end];

		function getStartOfWeek(date: Date, firstDayOfWeek: number) {
			const day = date.clearHoursUTC().getDay();
			const diff = ((day < firstDayOfWeek) ? 7 : 0) + day - firstDayOfWeek; // eslint-disable-line @typescript-eslint/no-magic-numbers
			const startOfWeek = new Date(date);
			startOfWeek.setDate(date.getDate() - diff);
			return startOfWeek;
		}
	}

	public hasWorkingCalendarSet() {
		return this.timeSlots.length > 0;
	}

	public hasData() {
		return this.source.length > 0 || this.sourceBacklog.length > 0;
	}

	protected getResources(currentSearchAppointment: SearchAppointmentModel) {
		const resources = new Map(this.assignments.map(obj => [obj.resourceId,
			new ResourceModel({ id: obj.resourceId, name: StringHelper.xss`${obj.AcctName.cellText}` })]));
		if (!currentSearchAppointment) {
			return resources;
		}
		const assignmentsFound = this.getAssignmentsByAppointmentId(currentSearchAppointment.appointmentID);
		const resourcesUsed = new Set<string>(assignmentsFound.map(obj => obj.resourceId));
		const sortedResources = new Map([...resources.entries()].sort((a, b) => {
			const aUsed = resourcesUsed.has(a[0]);
			const bUsed = resourcesUsed.has(b[0]);
			if (aUsed && !bUsed) return -1;
			if (!aUsed && bUsed) return 1;
			return 0;
		}));
		return sortedResources;
	}

	protected getAdjustedEventEndTime(appointment: AppointmentEmployeeModel) {
		const minEndDate = new Date(appointment.dateTimeBegin.getTime() + AppointmentsDataHandler.minimumExistingIntervalMin * 60 * 1000); // eslint-disable-line @typescript-eslint/no-magic-numbers
		const endTime = (appointment.dateTimeEnd >= minEndDate) ? appointment.dateTimeEnd : minEndDate;
		return endTime;
	}

	protected getEvents() {
		const singleAssignmentsById = new Map([...this.assignmentsById.values()].map(obj => [obj.appointmentID, obj]));
		const events = [...singleAssignmentsById.values()].map(obj => new EventModel({
			id: `${this.getFullId(obj.appointmentID)}`,
			startDate: obj.dateTimeBegin,
			endDate: this.getAdjustedEventEndTime(obj),
			name: StringHelper.xss`${obj.caption}`,
			cls: `${getAppointmentClass(obj)}`,
			resizable: !obj?.SchedulerAppointment__Locked.value,
			draggable: !obj?.SchedulerAppointment__Locked.value,
		}));
		const dndEventModel = new EventModel({
			id: AppointmentsDataHandler.dndEventId,
			startDate: new Date(),
			endDate: new Date(),
			name: `DND`,
		});
		events.push(dndEventModel);
		return events;

		function getAppointmentClass(assignment: AppointmentEmployeeModel) {
			if (assignment.SchedulerAppointmentEmployee__IsFilteredOut?.value) {
				return "qp-sch-filtered-out";
			}
			if (assignment.isLocked) {
				return "qp-sch-locked";
			}
			if (!assignment.isValidatedByDispatcher && !assignment.isConfirmed) {
				return "qp-sch-not-confirmed";
			}
			return "b-sch-event";
		}
	}

	protected getAssignments() {
		const assignments = [...this.assignmentsById.values()].map(obj => new AssignmentModel({
			id: `${this.getFullId(obj.assignmentID)}`,
			eventId: `${this.getFullId(obj.appointmentID)}`,
			resourceId: obj.resourceId,
		}));
		return assignments;
	}

	protected getOffWorkTimeRanges(datesFilter: DatesFilterModel) {
		const [periodStartDate, periodEndDate] = AppointmentsDataHandler.getPeriodStartEndDates(datesFilter);
		const resourceIdToTimeRange = new Map<string, Array<({start: Date; end: Date})>>();
		this.resources.forEach(resource => {
			if (!resourceIdToTimeRange.has(resource.id.toString())) {
				resourceIdToTimeRange.set(resource.id.toString(), []);
			}
		});

		const offWorkTimeRanges: ResourceTimeRangeModel[] = [];
		const timeSlots = this.timeSlots;
		prepareWorkTimeRanges(this.assignments);
		computeInversedTimeRanges(this);

		return offWorkTimeRanges;

		function prepareWorkTimeRanges(assignments: AppointmentEmployeeModel[]) {
			timeSlots.forEach(obj => {
				//TODO: we don't really need to compare with the period range here --
				// -- it's just that we need to filter out incorrect results provided by the server
				if (obj.end.getTime() < periodStartDate.getTime()) return;
				if (obj.start.getTime() > periodEndDate.getTime()) return;

				const timeRanges = resourceIdToTimeRange.get(obj.resourceId);
				timeRanges.push({start: obj.start, end: obj.end});
			});
		}

		function computeInversedTimeRanges(self: AppointmentsDataHandler) {
			resourceIdToTimeRange.forEach((timeRanges, resourceId) => {
				let curTime =  periodStartDate;
				timeRanges.sort((a, b) => a.start.getTime() - b.start.getTime());
				timeRanges.forEach(range => {
					addTimeSpan(resourceId, curTime, range.start);
					curTime = range.end;
				});
				addTimeSpan(resourceId, curTime, periodEndDate);
			});

			function addTimeSpan(resourceId: string, startDate: Date, endDate: Date) {
				if (startDate >= endDate) return;

				offWorkTimeRanges.push(new ResourceTimeRangeModel({
					id: self.lastTimeSpanId,
					resourceId: resourceId,
					timeRangeColor: 'blue',
					startDate: startDate,
					endDate: endDate,
				}));
				self.lastTimeSpanId ++;
			}
		}
	}

	protected getTimeProjectionRange() {
		return [new TimeSpan({ id: 1, cls: 'qp-sch-time-projection' })];
	}

	protected getResourcesToEmptyPeriods() {
		const resourcesToEmptyPeriods = new Map<string, Array<TimePeriod>>();

		const mapAssignmentRanges = getResourceToTimeRanges([...this.assignmentsById.values()],
			(obj) => ({resourceId: obj.resourceId, start: obj.dateTimeBegin, end: obj.dateTimeEnd}));
		const mapOffWorkRanges = getResourceToTimeRanges(this.offWorkTimeRanges,
			(obj) => ({resourceId: obj.resourceId, start: obj.startDate, end: obj.endDate}));
		const resources = [...mapOffWorkRanges.keys()];

		resources.forEach(resourceId => {
			const assignmentRanges = mapAssignmentRanges.get(resourceId) ?? [];
			const offWorkRanges = mapOffWorkRanges.get(resourceId) ?? [];
			const freeSlots = findFreeSlots(offWorkRanges, assignmentRanges);
			resourcesToEmptyPeriods.set(resourceId, freeSlots);
		});

		return resourcesToEmptyPeriods;

		function getResourceToTimeRanges(source: Array<any>, getRange: (obj) => ({resourceId: string; start: Date; end: Date})) {
			const mapRanges = new Map<string, Array<TimePeriod>>();

			source.forEach(obj => {
				const range = getRange(obj);
				if (!mapRanges.has(range.resourceId)) {
					mapRanges.set(range.resourceId, new Array<TimePeriod>());
				}
				mapRanges.get(range.resourceId).push({start: range.start, end: range.end});
			});
			const resources = [...mapRanges.keys()];
			resources.forEach(resourceId => {
				const sorted = mapRanges.get(resourceId).sort((a, b) => a.start.getTime() - b.start.getTime());
				const merged: TimePeriod[] = [];
				sorted.forEach(period => {
					const last = merged.length > 0 ? merged[merged.length - 1] : null;
					if (merged.length === 0 || last.end < period.start) {
						merged.push(period);
					} else if (last.end < period.end) {
						last.end = period.end;
					}
				});

				mapRanges.set(resourceId, merged);
			});
			return mapRanges;
		}

		function findFreeSlots(offWorkPeriods: TimePeriod[], appointments: TimePeriod[]): TimePeriod[] {
			enum TimePointType {
				OffWorkStart = 0,
				OffWorkEnd = 1,
				AppointmentStart = 2,
				AppointmentEnd = 3,
			}
			interface TimePoint {
				time: Date;
				type: TimePointType;
			}

			const timePoints: TimePoint[] = [];
			offWorkPeriods.forEach(period => {
				timePoints.push({ time: period.start, type: TimePointType.OffWorkStart });
				timePoints.push({ time: period.end, type: TimePointType.OffWorkEnd });
			});
			appointments.forEach(period => {
				timePoints.push({ time: period.start, type: TimePointType.AppointmentStart });
				timePoints.push({ time: period.end, type: TimePointType.AppointmentEnd });
			});
			timePoints.sort((a, b) => a.time.getTime() - b.time.getTime() || a.type - b.type);

			let inWorkTime = false;
			let inAppointment = false;
			let possibleStart: Date = null;
			const freeSlots: TimePeriod[] = [];
			timePoints.forEach(point => {
				switch (point.type) {
					case TimePointType.OffWorkEnd:
						if (!inAppointment) {
							possibleStart = point.time;
						}
						inWorkTime = true;
						break;

					case TimePointType.OffWorkStart:
						if (inWorkTime && !inAppointment && possibleStart != null && possibleStart.getTime() !== point.time.getTime()) {
							freeSlots.push({start: new Date(possibleStart), end: new Date(point.time)});
						}
						inWorkTime = false;
						break;

					case TimePointType.AppointmentStart:
						if (inWorkTime && possibleStart != null && possibleStart.getTime() !== point.time.getTime()) {
							freeSlots.push({start: new Date(possibleStart), end: new Date(point.time)});
						}
						possibleStart = null;
						inAppointment = true;
						break;

					case TimePointType.AppointmentEnd:
						if (inWorkTime) {
							possibleStart = point.time;
						}
						inAppointment = false;
						break;
				}
			});

			return freeSlots;
		}
	}

	protected getWorkingHours() {
		const [periodStartDate, periodEndDate] = AppointmentsDataHandler.getPeriodStartEndDates(this.datesFilter);
		const workingHoursSet = new Set<string>();
		addTimeSlotsToWorkingHours(workingHoursSet, this.timeSlots);
		addTimeSlotsToWorkingHours(workingHoursSet, [...this.assignmentsById.values()]
			.map(obj => ({start: obj.dateTimeBegin, end: obj.dateTimeEnd}))
		);
		addTimeSlotsToWorkingHours(workingHoursSet, [...this.sourceBacklog.values()]
			.map(obj => ({start: obj.dateTimeBegin, end: obj.dateTimeEnd}))
		);
		return workingHoursSet;

		function addTimeSlotsToWorkingHours(workingHoursSet: Set<string>, timeSlots: {start: Date; end: Date}[]) {
			timeSlots.sort((a, b) => a.start.getTime() - b.start.getTime());
			let currentHour: Date = null;

			for (const slot of timeSlots) {
				const maxStart = slot.start > periodStartDate ? slot.start : periodStartDate;
				const minEnd = slot.end < periodEndDate ? slot.end : periodEndDate;
				if (currentHour == null || maxStart > currentHour) {
					currentHour = new Date(maxStart);
					currentHour.setHours(currentHour.getHours(), 0, 0, 0);
				}

				while (currentHour < minEnd) {
					const hourText = currentHour.toISOString();
					if (!workingHoursSet.has(hourText)) {
						workingHoursSet.add(hourText);
					}
					currentHour.setHours(currentHour.getHours() + 1);
				}
			}
		}
	}
}
