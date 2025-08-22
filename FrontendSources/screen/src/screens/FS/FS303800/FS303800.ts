import {
	graphInfo,
	gridConfig,
	createSingle,
	createCollection,
	linkCommand,
	PXScreen,
	PXView,
	PXFieldState,
	PXActionState,
	PXFieldOptions
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.RouteSequenceMaint', primaryView: 'Filter' })
export class FS303800 extends PXScreen {
	OpenContract: PXActionState;
	OpenSchedule: PXActionState;
	Filter = createSingle(ServiceContractsByRouteFilter);
	ServiceContracts = createCollection(FSScheduleRoute);
}

export class ServiceContractsByRouteFilter extends PXView {
	RouteID: PXFieldState<PXFieldOptions.CommitChanges>;
	ServiceContractFlag: PXFieldState<PXFieldOptions.CommitChanges>;
	ScheduleFlag: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false,
	allowImport: true
})
export class FSScheduleRoute extends PXView {
	GlobalSequence: PXFieldState;
	FSServiceContract__CustomerID: PXFieldState;
	FSServiceContract__CustomerLocationID: PXFieldState;
	Location__Descr: PXFieldState;
	Address__AddressLine1: PXFieldState;
	Address__City: PXFieldState;
	Address__State: PXFieldState;
	@linkCommand("OpenContract") FSServiceContract__RefNbr: PXFieldState;
	FSServiceContract__CustomerContractNbr: PXFieldState;
	FSServiceContract__DocDesc: PXFieldState;
	FSServiceContract__Status: PXFieldState;
	@linkCommand("OpenSchedule") FSSchedule__RefNbr: PXFieldState;
}
