import {
	IN204000
} from '../IN204000';

import {
	INSiteAccounts,
	INLocation
} from '../views';

import {
	PXView,
	createCollection,
	PXFieldState,
	PXFieldOptions,
	viewInfo,
	gridConfig,
	placeAfterProperty,
	columnConfig
} from 'client-controls';

export interface IN204000_Manufacturing extends IN204000 {}
export class IN204000_Manufacturing {
	
	@viewInfo({containerName: 'Transfer Lead Time'})
	AMLeadTimes = createCollection(AMSiteTransfer);
}

@gridConfig({
	syncPosition: true,
	adjustPageSize: true
}) 
export class AMSiteTransfer extends PXView {
	@columnConfig({ hideViewLink: true })
	TransferSiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	TransferLeadTime: PXFieldState<PXFieldOptions.CommitChanges>;
}

export interface INSiteAccounts_Manufacturing extends INSiteAccounts { }
export class INSiteAccounts_Manufacturing {
	@placeAfterProperty("SignEndBalance")
	LCVarianceSubID: PXFieldState;
	@placeAfterProperty("SignEndBalance")
	AMWIPAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	@placeAfterProperty("SignEndBalance")
	AMWIPSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	@placeAfterProperty("SignEndBalance")
	AMWIPVarianceAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	@placeAfterProperty("SignEndBalance")
	AMWIPVarianceSubID: PXFieldState;
	@placeAfterProperty("CarrierFacility")
	AMScrapSiteID: PXFieldState<PXFieldOptions.CommitChanges>;
	@placeAfterProperty("CarrierFacility")
	AMScrapLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	@placeAfterProperty("CarrierFacility")
	AMMRPFcst: PXFieldState;
	@placeAfterProperty("CarrierFacility")
	AMMRPPO: PXFieldState;
	@placeAfterProperty("CarrierFacility")
	AMMRPFlag: PXFieldState;
	@placeAfterProperty("CarrierFacility")
	AMMRPProd: PXFieldState;
	@placeAfterProperty("CarrierFacility")
	AMMRPMPS: PXFieldState;
	@placeAfterProperty("CarrierFacility")
	AMMRPSO: PXFieldState;
	@placeAfterProperty("CarrierFacility")
	AMMRPShip: PXFieldState;
}

export interface INLocation_Manufacturing extends INLocation { }
export class INLocation_Manufacturing {
	@placeAfterProperty("PathPriority")
	AMMRPFlag: PXFieldState;
}