import { CR303010 } from "../CR303010";
import {
	PXView,
	PXFieldState,
	createSingle,
	PXFieldOptions,
	viewInfo,
} from "client-controls";

export interface CR303010_Shipping extends CR303010 {}
export class CR303010_Shipping {
	LocationCurrent = createSingle(Location2);
}

export class Location2 extends PXView {
	Descr: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideAddress: PXFieldState<PXFieldOptions.CommitChanges>;
	OverrideContact: PXFieldState<PXFieldOptions.CommitChanges>;
	CBranchID: PXFieldState;
	CPriceClassID: PXFieldState;
	CDefProjectID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxRegistrationID: PXFieldState;
	CTaxZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
	CTaxCalcMode: PXFieldState;
	CAvalaraExemptionNumber: PXFieldState;
	CAvalaraCustomerUsageType: PXFieldState;
	CSiteID: PXFieldState;
	CCarrierID: PXFieldState<PXFieldOptions.CommitChanges>;
	CShipTermsID: PXFieldState;
	CShipZoneID: PXFieldState;
	CFOBPointID: PXFieldState;
	CResedential: PXFieldState;
	CSaturdayDelivery: PXFieldState;
	CInsurance: PXFieldState;
	CGroundCollect: PXFieldState;
	CShipComplete: PXFieldState;
	COrderPriority: PXFieldState;
	CLeadTime: PXFieldState;
	CCalendarID: PXFieldState;
}
