import {
	PXScreen, graphInfo, PXView, PXFieldState, PXActionState, createCollection, linkCommand, gridConfig
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CA.CAPendingReviewEnq', primaryView: 'Documents' })
export class CA403000 extends PXScreen {
	Documents = createCollection(ARPaymentInfo);

	redirectToDoc: PXActionState;
	redirectToPaymentMethod: PXActionState;
	redirectToProcCenter: PXActionState;
	redirectToCustomer: PXActionState;
}

@gridConfig({ syncPosition: true, initNewRow: true, mergeToolbarWith: 'ScreenToolbar', quickFilterFields: ["RefNbr", "CustomerID"] })
export class ARPaymentInfo extends PXView {
	BranchID: PXFieldState;
	DocType: PXFieldState;

	@linkCommand("redirectToDoc")
	RefNbr: PXFieldState;

	@linkCommand("redirectToPaymentMethod")
	PaymentMethodID: PXFieldState;

	PMInstanceDescr: PXFieldState;

	@linkCommand("redirectToProcCenter")
	ProcessingCenterID: PXFieldState;

	@linkCommand("redirectToCustomer")
	CustomerID: PXFieldState;

	CuryOrigDocAmt: PXFieldState;
	DocDate: PXFieldState;
	CCPaymentStateDescr: PXFieldState;
	ValidationStatus: PXFieldState;
}
