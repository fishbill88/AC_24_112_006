import { AP302000 } from '../AP302000';
import { PXView, PXFieldState, createCollection, columnConfig, gridConfig, createSingle, featureInstalled, GridPreset } from 'client-controls';

export interface AP302000_CN_JointCheck extends AP302000 { }

@featureInstalled('PX.Objects.CS.FeaturesSet+Construction')
export class AP302000_CN_JointCheck {

	// PX.Objects.CN.JointChecks.APPaymentEntryJointCheck AP302000_CN_JointCheck

	BillWithJointPayee = createCollection(BillWithJointPayee);
	SelectedJointPayee = createSingle(JointPayeeDisplay);

}

export class JointPayeeDisplay extends PXView {

	Name: PXFieldState;

}

@gridConfig({
	preset: GridPreset.Details,
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false
})
export class BillWithJointPayee extends PXView {

	@columnConfig({ allowCheckAll: true })
	Selected: PXFieldState;
	RefNbr: PXFieldState;
	LineNbr: PXFieldState;
	Name: PXFieldState;
	CuryJointAmountOwed: PXFieldState;
	CuryJointAmountPaid: PXFieldState;
	CuryBalance: PXFieldState;

}
