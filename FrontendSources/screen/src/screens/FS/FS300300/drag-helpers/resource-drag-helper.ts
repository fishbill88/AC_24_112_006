import { Format } from 'client-controls/services';
/* eslint-disable brace-style */
import { Scheduler, SchedulerEventModel, SchedulerResourceModel, TimeSpan, SchedulerAssignmentModel,
	DragHelper, Toast, StringHelper, Tooltip } from '@bryntum/schedulerpro/schedulerpro.module';
// import { IGridRow, QpGridCustomElement } from 'client-controls';
import { dateFormatInfo, numberFormatInfo } from 'client-controls/utils/platform-dependable-utils';
import { formatDate } from 'client-controls/utils';
import { ResourceModule } from 'aurelia-framework';
import { DragHelperEventInfo } from '../FS300300';

export class ResourceDragHelperConfig {
	owner: any;
	getScheduler: () => Scheduler;
	// grid: QpGridCustomElement;
	// getRows: () => IGridRow[];
	// getProxyInner: (row: IGridRow) => string;
	// getEventName: (row: IGridRow) => string;
	// getDuration?: (row: IGridRow) => number;
	getEventInfo: (assignmentId: string) => DragHelperEventInfo;
	addResource?: (resourceId: number, eventId: number) => Promise<boolean>;
}

export class ResourceDragHelper extends DragHelper {

	private draggedResource: SchedulerResourceModel;
	private hoverAssignmentElement: HTMLElement;
	private projectedAssignment: SchedulerAssignmentModel;
	private initialScrollPosition: any;
	private verticalScrollTime: Date;
	private get hoverAssignment() {
		return (this.hoverAssignmentElement != null) ? this.scheduler.resolveAssignmentRecord(this.hoverAssignmentElement) : null;
	}

	private get scheduler() { return this.env.getScheduler(); }

	protected static get defaultConfig() {
		return {
			cloneTarget: true,
			autoSizeClonedTarget: false,
			unifiedProxy: true,
			dragThreshold: 0,
			mode: 'translateXY',
			dropTargetSelector: '.b-sch-event-wrap',
		};
	}

	constructor(protected env: ResourceDragHelperConfig, config) {
		super(config);

		const me = this; // eslint-disable-line @typescript-eslint/no-this-alias

		this.on({
			dragStart: me.onDragStartFunc,
			drag: me.onDragFunc,
			drop: me.onDropFunc,
			abort: me.onAbortFunc,
		});
	}

	public createProxy(element: HTMLElement) {
		const draggedResource = this.getDraggedResource(element);

		const draggedWidth = this.scheduler.resourceColumnWidth - (2 * this.scheduler.resourceMargin);
		const draggedHeight = this.scheduler.rowHeight - (2 * this.scheduler.resourceMargin);

		const proxy = document.createElement('div');
		proxy.style.cssText = 'display: block';
		proxy.style.width = `${draggedWidth}px`;
		proxy.style.height = `${draggedHeight}px`;

		proxy.classList.add();
		proxy.innerHTML = `
            <div class="qp-sch-dragged qp-sch-dragged-resource">
				<div>${draggedResource.name}</div>
            </div>
        `;

		return proxy;
	}

	protected getDraggedResource(element: HTMLElement): SchedulerResourceModel {
		if (element.localName === "span") {
			element = element.parentElement;
		}
		return (this.isHorizontalMode()
			? this.scheduler.resolveResourceRecord(element)
			: this.scheduler.resources.find(x => x.id === element.dataset.resourceId)) as SchedulerResourceModel;
	}

	protected onDragStartFunc({ event, context }) {
		this.draggedResource = this.getDraggedResource(event.target);
		console.log(`dragging ${this.draggedResource.id}`);
		this.scheduler.snap = false;
	}

	protected isHorizontalMode() {
		return this.scheduler.mode === 'horizontal';
	}

	protected async onDragFunc({ context, event }) {
		if ((new Date().getTime() - this.verticalScrollTime?.getTime() ?? 0) < 200) return; // eslint-disable-line @typescript-eslint/no-magic-numbers

		const scheduler = this.scheduler;
		const hoverElement = context.target as HTMLElement;
		const hoverAssignment = hoverElement != null && scheduler.resolveAssignmentRecord(hoverElement);
		if (this.hoverAssignment === hoverAssignment) return; // nothing changed -- nothing to process

		this.hoverAssignmentElement = hoverElement;

		context.valid = this.isValidContext(hoverAssignment);
		if (!context.valid) {
			await this.unassignProjectedAssignment();
			return;
		}

		scheduler.suspendRefresh();
		await this.unassignProjectedAssignment(false);

		const eventInfo = this.env.getEventInfo(hoverAssignment.id.toString());
		const eventModel = hoverAssignment.event as SchedulerEventModel;
		eventModel.set('eventInfo', eventInfo);
		this.projectedAssignment = scheduler.eventStore.assignEventToResource(eventModel, this.draggedResource)?.[0] ?? null;

		const posBefore = hoverElement.getBoundingClientRect();
		await scheduler.resumeRefresh(true);

		// Make sure hovered element stays in place -- otherwise we'd jump on and off it several times per second
		const posAfter = hoverElement.getBoundingClientRect();
		if (posAfter.y !== posBefore.y) {
			this.initialScrollPosition = scheduler.storeScroll();
			await scheduler.scrollVerticallyTo(this.initialScrollPosition.scrollTop + posAfter.y - posBefore.y, false);
			this.verticalScrollTime = new Date();
		}

		if (!showProjectedAssignmentElement(this.projectedAssignment)) {
			// console.log(`restoring state after we failed to attach the resource for some reason`);
			await this.unassignProjectedAssignment();
		}

		function showProjectedAssignmentElement(projectedAssignment) {
			const element = scheduler.getElementFromAssignmentRecord(projectedAssignment);
			if (element == null) return false;
			element.classList.add("qp-sch-projected-assignment");
			return true;
		}
	}

	protected isValidContext(hoverAssignment: SchedulerAssignmentModel) {
		if (hoverAssignment == null) return false;
		const scheduledResources = this.scheduler.eventStore.getResourcesForEvent(hoverAssignment.event);
		if (scheduledResources.some(x => x.id === this.draggedResource.id)) return false;

		return true;
	}

	protected setClassIf(element: HTMLElement, condition: boolean, className: string) {
		if (condition) {
			element.classList.add(className);
		}
		else {
			element.classList.remove(className);
		}
	}

	protected onAbortFunc() {
		this.unassignProjectedAssignment();
		this.finalize();
	}

	protected finalize() {
		this.scheduler.snap = true;
	}

	protected async unassignProjectedAssignment(refresh = true) {
		if (this.projectedAssignment == null) return;

		this.scheduler.eventStore.unassignEventFromResource(this.projectedAssignment.event, this.draggedResource);
		this.projectedAssignment = null;
		if (refresh) {
			await this.scheduler.resumeRefresh(true);
		}

		if (this.initialScrollPosition != null) {
			this.scheduler.restoreScroll(this.initialScrollPosition);
			this.initialScrollPosition = null;
		}
	}

	protected async onDropFunc({ context, event }) {
		let scheduledOnServer = false;
		try {
			if (!context.valid || this.scheduler == null) return;

			const hoverAssignment = context.target && this.scheduler.resolveAssignmentRecord(context.target);
			if (hoverAssignment == null || hoverAssignment.resourceId === this.draggedResource.id) return;

			console.log(`dropping ${this.draggedResource.id} on ${hoverAssignment?.id}`);

			scheduledOnServer = await this.env.addResource.bind(this.env.owner)(this.draggedResource.id, hoverAssignment.eventId);
		}
		catch (ex) {
			// TODO: handle error-reporting
			throw ex;
		}
		finally {
			if (!scheduledOnServer) { // no need to unassign if the conntrol object is brand new
				this.unassignProjectedAssignment();
			}
			this.finalize();
		}
	}
};

