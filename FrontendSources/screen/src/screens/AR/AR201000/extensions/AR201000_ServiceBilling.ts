import {
	AR201000
} from '../AR201000';

import {
	PXView,
	createCollection,
	PXFieldState,
	PXFieldOptions,
	featureInstalled,
	columnConfig,
	localizable
} from 'client-controls';


export interface AR201000_ServiceBilling extends AR201000 { }

@featureInstalled('PX.Objects.CS.FeaturesSet+ServiceManagementModule')
export class AR201000_ServiceBilling {
	BillingCycles = createCollection(FSCustomerClassBillingSetup);
}

export class FSCustomerClassBillingSetup extends PXView {

	@columnConfig({ hideViewLink: true })
	SrvOrdType: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ hideViewLink: true })
	BillingCycleID: PXFieldState<PXFieldOptions.CommitChanges>;

	SendInvoicesTo: PXFieldState<PXFieldOptions.CommitChanges>;
	BillShipmentSource: PXFieldState;
	FrequencyType: PXFieldState<PXFieldOptions.CommitChanges>;
}
