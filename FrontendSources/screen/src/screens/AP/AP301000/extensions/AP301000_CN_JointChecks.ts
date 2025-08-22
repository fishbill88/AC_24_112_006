import { AP301000 } from '../AP301000';
import { PXView, createCollection, PXFieldState, PXActionState, linkCommand, PXFieldOptions, GridColumnShowHideMode, columnConfig, gridConfig, GridPreset } from 'client-controls';

export interface AP3010000_JointChecks extends AP301000 { }
export class AP3010000_JointChecks {

	JointPayees_Vendor_ViewDetails: PXActionState;

	// PX.Objects.CN.Compliance.AP.GraphExtensions+ApInvoiceEntryExt

	JointPayees = createCollection(JointPayee);
	JointAmountApplications = createCollection(JointPayeePayment);

}

@gridConfig({ preset: GridPreset.Details })
export class JointPayee extends PXView {

	@linkCommand("JointPayees_Vendor_ViewDetails")
	JointPayeeInternalId: PXFieldState<PXFieldOptions.CommitChanges>;

	JointPayeeExternalName: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	APLineNbr: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	BillLineAmount: PXFieldState;

	CuryJointAmountOwed: PXFieldState<PXFieldOptions.CommitChanges>;

	CuryJointAmountPaid: PXFieldState<PXFieldOptions.CommitChanges>;
	CuryJointBalance: PXFieldState<PXFieldOptions.CommitChanges>;

	@columnConfig({ allowShowHide: GridColumnShowHideMode.False })
	CanDelete: PXFieldState;

}

@gridConfig({ preset: GridPreset.Details })
export class JointPayeePayment extends PXView {

	JointPayees_Vendor_ViewDetails: PXActionState;
	ViewApPayment: PXActionState;

	@linkCommand("JointPayees_Vendor_ViewDetails")
	JointPayee__JointPayeeInternalId: PXFieldState<PXFieldOptions.CommitChanges>;

	JointPayee__JointPayeeExternalName: PXFieldState;
	BillLineNumber: PXFieldState;
	CuryJointAmountToPay: PXFieldState;
	APPayment__CuryID: PXFieldState;
	APPayment__DocType: PXFieldState;

	@linkCommand("ViewApPayment")
	APPayment__RefNbr: PXFieldState<PXFieldOptions.CommitChanges>;

	APPayment__ExtRefNbr: PXFieldState;
	APPayment__Status: PXFieldState;

}
