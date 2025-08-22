import { Rows } from './../../../CS/CS206010/CS206010';
import { Format } from 'client-controls/services';
/* eslint-disable brace-style */
import { SchedulerPro, SchedulerEventModel, SchedulerResourceModel, SchedulerAssignmentModel,
	DragHelper, StringHelper, Tooltip } from '@bryntum/schedulerpro/schedulerpro.module';
import { QpGridCustomElement, eventTypeMetadataName } from 'client-controls';
import { dateFormatInfo, numberFormatInfo, siteRoot } from 'client-controls/utils/platform-dependable-utils';
import { formatDate } from 'client-controls/utils';
import { DragHelperEventInfo, FS300300 } from '../FS300300';
import { AppointmentsDataHandler } from '../data-handlers/appointments-data-handler';
import { DraggableEntity } from '../view-models';


type ScheduleEventFunc = (eventId: number, resourceId: number, dateBegin: Date, dateEnd: Date) => Promise<boolean>;
type ScheduleEntityFunc = (entity: DraggableEntity, resourceId: number, dateBegin: Date, dateEnd: Date) => Promise<number>;

export class ExternalEventDragHelperConfig {
	owner: FS300300;
	getSnapToResourcePosition: (resourceRecord : SchedulerResourceModel, x: number, y: number) => ([snapX: number, snapY: number]);
	getScheduler: () => SchedulerPro;
	getProxyInner: (entity: DraggableEntity) => string;
	getEventInfo: (entity: DraggableEntity) => DragHelperEventInfo;
	getNewEventId?: (entity: DraggableEntity) => string;
	getAppointmentStart: (resourceId: string, entity: DraggableEntity) => Date;
	getDurationMS: (entity: DraggableEntity) => number;
	getEntity: (element: HTMLElement) => DraggableEntity;
	allowDragging?: () => boolean;
	toggleTimeProjection?: (start: Date, end: Date) => void;
	scheduleEvent?: ScheduleEventFunc;
	scheduleEntity?: ScheduleEntityFunc;
}

export class ExternalEventDragHelper extends DragHelper {

	private draggedEntity: DraggableEntity;
	private draggedWidth: number;
	private draggedHeight: number;
	private targetBeginDate: Date;
	private tooltip: Tooltip;
	private projectedAssignment: SchedulerAssignmentModel;
	private projectedAssignmentResource: SchedulerResourceModel;
	private projectedEvent: SchedulerEventModel;
	private isDraggingReentryProtectionOn = false;
	private prevTarget: any;

	private get scheduler() { return this.env.getScheduler(); }

	protected static get defaultConfig() {
		return {
			cloneTarget: true,
			autoSizeClonedTarget: false,
			unifiedProxy: true,
			mode: 'translateXY',
			dropTargetSelector: '.b-grid-row, .b-timeline-subgrid, .b-resourceheader',
		};
	}

	constructor(protected env: ExternalEventDragHelperConfig, config) {
		super(config);

		const me = this; // eslint-disable-line @typescript-eslint/no-this-alias

		this.on({
			beforeDragStart: me.onBeforeDragStartFunc,
			dragStart: me.onDragStartFunc,
			drag: me.onDragFunc,
			drop: me.onDropFunc,
			abort: me.onAbortFunc,
		});
	}

	public createProxy(element) {
		const draggedEntity = this.env.getEntity(element);
		if (draggedEntity == null) return null;

		const durationInPixels = this.scheduler.timeAxisViewModel.getDistanceForDuration(this.env.getDurationMS(draggedEntity));
		const margin = 2 * this.scheduler.resourceMargin;
		this.draggedWidth = this.isHorizontalMode() ? durationInPixels : this.scheduler.resourceColumnWidth - margin;
		this.draggedHeight = this.isHorizontalMode() ? this.scheduler.rowHeight - margin : durationInPixels;

		const proxy = document.createElement('div');
		proxy.style.cssText = 'display: block';
		proxy.style.width = `${this.draggedWidth}px`;
		proxy.style.height = `${this.draggedHeight}px`;

		// Fake an event bar
		//proxy.classList.add();
		proxy.innerHTML = `
            <div class="qp-sch-dragged-wrapper ${this.isHorizontalMode() ? "" : "qp-sch-vertical"}">
				<div class="qp-sch-dragged">
					${this.env.getProxyInner(draggedEntity)}
				</div>
            </div>
        `;

		return proxy;
	}

	protected onBeforeDragStartFunc({ event, context }) {
		return this.env.allowDragging ? this.env.allowDragging() : true;
	}

	protected onDragStartFunc({ event, context }) {
		this.draggedEntity = this.env.getEntity(event.target);
		if (this.tooltip != null) {
			this.tooltip.destroy();
		}
		this.tooltip = new Tooltip({
			forSelector: '*',
			getHtml: () => this.getTooltipHtml(),
			bodyCls: "qp-sch-new-event-tooltip b-sch-event-tooltip b-widget b-popup b-panel b-floating b-vbox b-text-popup b-outer b-windows b-anchored b-aligned-above",
			autoShow: true,
			// anchorToTarget: false,
			anchor: true,
			// allowOver: true,
			mouseOffsetX: - 180 / 2, // eslint-disable-line @typescript-eslint/no-magic-numbers
			mouseOffsetY: - this.draggedHeight / 2 - 100 - 15, // eslint-disable-line @typescript-eslint/no-magic-numbers
			hoverDelay: 0,
			trackMouse: true
		});
		this.projectedEvent = this.scheduler.eventStore.getById("dnd_event") as SchedulerEventModel;

		const element = document.elementFromPoint(event.pageX, event.pageY) as HTMLElement;
		const resource = this.scheduler.resolveResourceRecord(element, [context.newX, context.new]);
		const dateBegin = this.env.getAppointmentStart(resource?.id.toString(), this.draggedEntity);
		const durationInMS = this.env.getDurationMS(this.draggedEntity);
		const dateEnd = dateBegin ? new Date(dateBegin) : null;
		dateEnd?.setMilliseconds(dateBegin.getMilliseconds() + durationInMS);
		this.env.toggleTimeProjection?.bind(this.env.owner)(dateBegin, dateEnd);
	}

	protected getTooltipHtml() {
		const anchorHtml = `
			<div class="b-anchor b-anchor-bottom">
				<svg class="b-pointer-el" version="1.1" height="8" width="16">
					<path d="M0,8L8,0.5L16,8"></path>
				</svg>
			</div>
			`;

		const eventInfo = this.env.getEventInfo(this.draggedEntity);
		const [dateBegin, dateEnd] = this.getSelectedTimes();

		// TODO: It's somewhat of a kludge - redo this
		if (dateBegin == null || dateEnd == null) {
			const orderDate = eventInfo.date == null ? "" : `
				<div class="b-sch-clockwrap b-sch-clock-hour b-sch-tooltip-startdate">
					<svg><use href="${this.env.owner.iconDatePicker.SvgUrl}"></use></svg>
					<span class="b-sch-clock-text">${formatPopupDate(eventInfo.date, false)}</span>
				</div>
				`;
			return `
				<div class="b-sch-event-title" style="white-space: nowrap;">${eventInfo.name}</div>
				${orderDate}
				${anchorHtml}
				`;
		}

		return `
			<div class="b-sch-event-title" style="white-space: nowrap;">${eventInfo.name}</div>
			<div class="b-sch-clockwrap b-sch-clock-hour b-sch-tooltip-startdate">
				<div class="b-sch-clock">
					${getHoursDiv(dateBegin.getHours())}
					${getMinutesDiv(dateBegin.getMinutes())}
					<div class="b-sch-clock-dot"></div>
				</div>
				<span class="b-sch-clock-text">${formatPopupDate(dateBegin)}</span>
			</div>
			<div class="b-sch-clockwrap b-sch-clock-hour b-sch-tooltip-enddate">
				<div class="b-sch-clock">
					${getHoursDiv(dateEnd.getHours())}
					${getMinutesDiv(dateEnd.getMinutes())}
					<div class="b-sch-clock-dot"></div>
				</div>
				<span class="b-sch-clock-text">${formatPopupDate(dateEnd)}</span>
			</div>
			${anchorHtml}
		`;

		function getHoursDiv(hours: number) {
			// eslint-disable-next-line @typescript-eslint/no-magic-numbers
			return `<div class="b-sch-hour-indicator" style="transform: rotate(${hours % 12 * 30}deg);"></div>`;
		}

		function getMinutesDiv(minutes: number) {
			// eslint-disable-next-line @typescript-eslint/no-magic-numbers
			return `<div class="b-sch-minute-indicator" style="transform: rotate(${minutes * 6}deg);"></div>`;
		}

		function formatPopupDate(date: Date, withTime = true) {
			if (withTime) {
				return formatDate(date, "MMM d, yyyy HH:mm", dateFormatInfo());
			}
			return formatDate(date, "MMM d, yyyy", dateFormatInfo());
		}
	}

	protected getSelectedTimes(resource : SchedulerResourceModel = null) {
		let dateBegin = this.env.getAppointmentStart((resource ?? this.projectedAssignmentResource)?.id.toString(), this.draggedEntity);
		const durationInMS = this.env.getDurationMS(this.draggedEntity);
		if (this.targetBeginDate != null) {
			dateBegin = this.targetBeginDate;
		}
		if (dateBegin == null || isNaN(dateBegin.getTime())) {
			return [null, null];
			// dateBegin = new Date();
			// dateBegin.setHours(12, 0, 0, 0); // eslint-disable-line @typescript-eslint/no-magic-numbers
		}
		const dateEnd = new Date(dateBegin);
		dateEnd.setMilliseconds(dateBegin.getMilliseconds() + durationInMS);
		return [dateBegin, dateEnd];
	}

	protected isHorizontalMode() {
		return this.scheduler.mode === 'horizontal';
	}

	protected async onDragFunc({ context, event }) {
		const env = this.env;
		const isHorizontalMode = this.isHorizontalMode();
		const scheduler = this.scheduler;

		if (this.isDraggingReentryProtectionOn) return;
		try {
			this.isDraggingReentryProtectionOn = true;

			let [targetType, resourceAtCursor] = this.updateTargetTypeAndDate(context, event);
			context.valid = targetType === "time" || targetType === "resource";

			if (targetType === "resource") {
				// if (context.target !== this.prevTarget) { // optimization: same header item
				await this.showProjectionInGrid(resourceAtCursor);
				// }
				const [dateBegin, dateEnd] = this.getSelectedTimes(resourceAtCursor);
				if (this.projectedAssignment == null && dateBegin == null) {
					context.valid = false;
					targetType = 'n/a';
				}

			} else {
				await this.hideProjectedAssignment();
			}
			this.prevTarget = context.target;

			context.element.classList.toggle('qp-sch-dragged-valid', targetType === "time");
			context.element.classList.toggle('qp-sch-dragged-overheader', targetType === "resource");
			this.tooltip.html = this.getTooltipHtml();

			if (targetType === "time") {
				showProxyEventInGrid.bind(this)(resourceAtCursor);
			}

		}
		finally {
			this.isDraggingReentryProtectionOn = false;
		}

		function showProxyEventInGrid(resourceAtCursor: SchedulerResourceModel) {
			const coordTime = scheduler.getCoordinateFromDate(this.targetBeginDate, false); // eslint-disable-line @typescript-eslint/no-invalid-this
			if (coordTime < 0) return;

			const xPos = isHorizontalMode ? coordTime : context.newX;
			const yPos = isHorizontalMode ? context.newY : coordTime;
			const [xSnapped, ySnapped] = env.getSnapToResourcePosition.bind(env.owner)(resourceAtCursor, xPos, yPos);
			context.element.style.transform = `matrix(1, 0, 0, 1, ${xSnapped}, ${ySnapped})`;
		}
	}

	protected async showProjectionInGrid(resourceAtCursor: SchedulerResourceModel) {
		const scheduler = this.scheduler;

		if (this.projectedEvent == null || this.projectedEvent.resources.length > 1) {
			debugger;
		}
		const prevResource = this.projectedEvent.getResource();

		this.scheduler.suspendRefresh();

		try {
			const [dateBegin, dateEnd] = this.getSelectedTimes(resourceAtCursor);
			if (dateBegin == null || dateEnd == null
				|| dateBegin.getTime() >= this.scheduler.endDate.getTime()
				|| dateEnd.getTime() <= this.scheduler.startDate.getTime())
			{
				await this.hideProjectedAssignment();
				return;
			}
			const eventInfo = this.env.getEventInfo(this.draggedEntity);
			this.projectedEvent.startDate = dateBegin;
			this.projectedEvent.endDate = dateEnd;
			this.projectedEvent.name = `${eventInfo.name}`;
			this.projectedEvent.set('eventInfo', eventInfo);

			if (prevResource?.id === resourceAtCursor?.id) {
				updateProjectedAssignment.bind(this)(); // it could've been hidden - make sure that we have projectedAssignment set
				return; // same resource -- don't change assignments
			}

			await this.scheduler.scheduleEvent({startDate: dateBegin, eventRecord: this.projectedEvent, resourceRecord: resourceAtCursor});
			updateProjectedAssignment.bind(this)();
			if (this.projectedAssignment != null && prevResource != null) {
				this.scheduler.eventStore.unassignEventFromResource(this.projectedEvent, prevResource);
			}
		}
		catch (ex) {
			debugger;
		}
		finally {
			await this.scheduler.resumeRefresh(true);
		}

		if (!showProjectedAssignmentElement(this.projectedAssignment)) {
			//console.log(`restoring state after we failed to attach the resource for some reason`);
			await this.hideProjectedAssignment();
		}

		function showProjectedAssignmentElement(projectedAssignment) {
			const element = scheduler.getElementFromAssignmentRecord(projectedAssignment);
			// if (element == null) debugger; // totally possible to be null if it's outside the viewport
			if (element == null) return false;
			element.classList.add("qp-sch-projected-assignment");
			return true;
		}

		function updateProjectedAssignment() {
			if (resourceAtCursor == null) return;
			this.projectedAssignment = this.scheduler.eventStore // eslint-disable-line @typescript-eslint/no-invalid-this
				.getAssignmentsForEvent(this.projectedEvent)?.find(x => x.resourceId === resourceAtCursor.id) ?? null; // eslint-disable-line @typescript-eslint/no-invalid-this
			if (this.projectedAssignment != null) { // eslint-disable-line @typescript-eslint/no-invalid-this
				this.projectedAssignmentResource = resourceAtCursor; // eslint-disable-line @typescript-eslint/no-invalid-this
			}
		}
	}

	protected updateTargetTypeAndDate(context, event) : ["time" | "resource" | "n/a", SchedulerResourceModel] {
		const element = document.elementFromPoint(event.pageX, event.pageY) as HTMLElement;
		let resource = this.scheduler.resolveResourceRecord(element, [context.pageX, context.pageY]);
		if (context.pageY < this.scheduler.y || context.pageX < this.scheduler.x) {
			return ["n/a", null];
		}
		let beginDate = this.scheduler.getDateFromCoordinate(this.isHorizontalMode() ? context.newX : context.newY, 'round', false);
		if (beginDate < this.scheduler.visibleDateRange.startDate) {
			beginDate = null; // it must be a header w/ scrolled scheduler
		}
		const isHorizontalResourceHeader = resource != null && beginDate == null;

		let isVerticalResourceHeader = false;
		if (resource == null && !this.isHorizontalMode()) {
			resource = this.scheduler.resources.find(x => x.id === context.target.dataset.resourceId) as SchedulerResourceModel;
			isVerticalResourceHeader = resource != null;
		}

		if (isVerticalResourceHeader || isHorizontalResourceHeader) {
			this.targetBeginDate = null;
			return ["resource", resource];
		}
		if (resource == null) {
			return ["n/a", null];
		}

		this.targetBeginDate = beginDate;
		if (this.targetBeginDate != null) {
			return ["time", resource];
		}
		return ["n/a", null];
	}

	protected onAbortFunc() {
		this.hideProjectedAssignment();
		this.finalize();
	}

	protected async hideProjectedAssignment() {
		// TODO: rewrite this cludge -- there must be a _normal_ way of doing this
		const invisibleDate = new Date(1970, 1, 1); // eslint-disable-line @typescript-eslint/no-magic-numbers
		const projectedEvent = this.scheduler.eventStore.getById(AppointmentsDataHandler.dndEventId) as SchedulerEventModel;
		if (projectedEvent?.resource == null
			|| (projectedEvent.endDate as Date)?.toISOString() === invisibleDate.toISOString()) return;

		this.scheduler.suspendRefresh();
		projectedEvent.startDate = invisibleDate;
		projectedEvent.endDate = invisibleDate;
		this.projectedAssignment = null;
		this.projectedAssignmentResource = null;
		await this.scheduler.resumeRefresh(true);

	}

	protected finalize() {
		this.env.toggleTimeProjection?.bind(this.env.owner)(null, null);
		this.destroyTooltip();
	}

	protected destroyTooltip() {
		this.tooltip?.destroy();
		this.tooltip = null;
	}

	protected async onDropFunc({ context, event }) {
		try {
			if (!context.valid || this.scheduler == null) return;

			const [targetType, resource] = this.updateTargetTypeAndDate(context, event);
			if (targetType === 'n/a') return;

			const [dateBegin, dateEnd] = this.getSelectedTimes();
			const eventId = this.env.getNewEventId ? this.env.getNewEventId(this.draggedEntity) : -1; // -1 is ok since we're gonna reload anyway

			let scheduledOnServer = false;
			if (this.env.scheduleEvent != null) {
				scheduledOnServer = await this.env.scheduleEvent.bind(this.env.owner)(
					eventId, Number(resource.id), dateBegin, dateEnd);
			}
			else {
				scheduledOnServer = await this.env.scheduleEntity.bind(this.env.owner)(
					this.draggedEntity, Number(resource.id), dateBegin, dateEnd);
			}
		}
		catch (ex) {
			// TODO: handle error-reporting
			throw ex;
		}
		finally {
			this.finalize();
		}
	}
};

