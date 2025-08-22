import { Formats } from './FS300300';
import { SchedulerAssignmentModel, SchedulerEventModel, SchedulerPro, SchedulerResourceModel } from "@bryntum/schedulerpro";
import { AppointmentsDataHandler } from "./data-handlers/appointments-data-handler";
import { BingMapCustomElement, MapPinClickFunc } from "./bing-map";
import { AppointmentEmployeeModel } from "./view-models";
import { dateFormatInfo } from 'client-controls/utils/platform-dependable-utils';
import { formatDate } from 'client-controls/utils';

export class MapController {
	private assignmentsForPins: AppointmentEmployeeModel[] = [];
	private assignmentsForRouteSegments: {from: AppointmentEmployeeModel; to: AppointmentEmployeeModel}[] = [];
	private assignmentsLocations = new Map<AppointmentEmployeeModel, Microsoft.Maps.Location>();
	private routeSegmentsTo = new Map<AppointmentEmployeeModel, any>();
	private selectedResources: SchedulerResourceModel[] = [];

	public constructor(private map: BingMapCustomElement, private scheduler: SchedulerPro,
		private data: AppointmentsDataHandler, private popupFunc: MapPinClickFunc) {
	}

	public async updateMap() {
		if (!this.scheduler) return;

		this.selectedResources = this.scheduler.selectedRows as SchedulerResourceModel[];
		this.map.clearAllPins();

		if (!this.selectedResources?.length) return;

		this.selectAssignmentsForPinsAndRoutes();
		await this.fetchAssignmentsLocations();
		await this.fetchRouteSegments();
		this.placePinsAndRoutesOnMap();
		this.map.updateView();
	}

	private selectAssignmentsForPinsAndRoutes() {
		this.selectedResources.forEach(resource => {
			const schedulerAssignments = this.getSortedAssignmentsForResource(resource) ?? [];
			let prevAssignment: AppointmentEmployeeModel = null;
			schedulerAssignments.forEach(schedulerAssignment => {
				const assignment = this.data.getAssignment(schedulerAssignment.id.toString());
				if (!assignment) return; // not yet persisted apppointment
				this.assignmentsForPins.push(assignment);
				if (prevAssignment != null) {
					this.assignmentsForRouteSegments.push({from: prevAssignment, to: assignment});
				}
				prevAssignment = assignment;
			});
		});
	}

	private async fetchAssignmentsLocations() {
		await Promise.all(this.assignmentsForPins.map(async assignment => {
			const location = await this.getAssignmentLocation(assignment);
 			this.assignmentsLocations.set(assignment, location);
		}));
	};

	private async getAssignmentLocation(assignment: AppointmentEmployeeModel) {
		const latitude = assignment.SchedulerAppointment__MapLatitude?.value;
		const longitude = assignment.SchedulerAppointment__MapLongitude?.value;
		if (latitude != null && longitude != null) {
			return new Microsoft.Maps.Location(latitude, longitude);
		}
		return await this.map.getLocationByAddress(assignment.fullAddress);
	}

	private async fetchRouteSegments() {
		await Promise.all(this.assignmentsForRouteSegments.map(async locations => {
			const fromLocation = this.assignmentsLocations.get(locations.from);
			const toLocation = this.assignmentsLocations.get(locations.to);
			if (fromLocation == null || toLocation == null) return;
			const segment = await this.map.getRouteBetweenLocations(fromLocation, toLocation); // eslint-disable-line @typescript-eslint/no-invalid-this
			this.routeSegmentsTo.set(locations.to, segment);
		}));
	};

	private placePinsAndRoutesOnMap() {
		this.selectedResources.forEach(resource => {
			const schedulerAssignments = this.getSortedAssignmentsForResource(resource) ?? [];
			const assignments = schedulerAssignments.map(x => this.data.getAssignment(x.id.toString()));

			let assignmentNumber = 1;
			assignments.forEach(assignment => {
				if (assignment == null) return; // we can have a projected assignment that doesn't have a real assignment yet
				const location = this.assignmentsLocations.get(assignment);
				if (!location) return;
				const segment = this.routeSegmentsTo.get(assignment);
				const pinText = `${formatDate(assignment.dateTimeBegin, Formats.TimeAxisHour, dateFormatInfo())} - ${formatDate(assignment.dateTimeEnd, Formats.TimeAxisHour, dateFormatInfo())}`;
				this.map.addPin(location, assignment.appointmentID, pinText, assignmentNumber.toString(), this.popupFunc);
				if (segment != null) {
					this.map.addRouteSegment(segment, assignmentNumber === 2); // first segment of the route
				}
				assignmentNumber ++;
			});
		});
	}

	private getSortedAssignmentsForResource(resource: SchedulerResourceModel) {
		const scheduler = this.scheduler;
		const data = this.data;
		const [viewPortDay, nextDay] = getViewPortDates();
		if (viewPortDay == null) return null;

		return this.scheduler.assignmentStore.getAssignmentsForResource(resource)
			?.filter(x => viewPortDay <= getDate(x)  && getDate(x) <= nextDay)
			?.sort((a, b) => getDate(a).getTime() - getDate(b).getTime());

		function getViewPortDates() {
			if (!scheduler.visibleDateRange) return [null, null];

			const { startDate, endDate } = scheduler.visibleDateRange;
			let useStartDate = startDate.getDate() === endDate.getDate() && startDate.getMonth() === endDate.getMonth();
			if (!useStartDate) {
				const aMinuteBeforeStartDate = new Date(startDate);
				aMinuteBeforeStartDate.setMinutes(aMinuteBeforeStartDate.getMinutes() - 1);
				useStartDate = !data.isWorkingTime(aMinuteBeforeStartDate, startDate);
			}
			const viewPortDay = new Date(startDate);
			viewPortDay.setDate(startDate.getDate() + (useStartDate ? 0 : 1));
			viewPortDay.setHours(0, 0, 0, 0);
			const nextDay = new Date(viewPortDay);
			nextDay.setDate(nextDay.getDate() + 1);
			return [viewPortDay, nextDay];
		}

		function getDate(assignment: SchedulerAssignmentModel) {
			return ((assignment.event as SchedulerEventModel).startDate as Date);
		}
	}
}

