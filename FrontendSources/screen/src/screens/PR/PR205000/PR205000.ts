import { autoinject } from 'aurelia-framework';
import {
	PXFieldOptions,
	PXFieldState,
	PXScreen,
	PXView,
	columnConfig,
	createCollection,
	graphInfo,
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.PR.PRPayGroupMaint', primaryView: 'PayGroup' })
@autoinject
export class PR205000 extends PXScreen {
	PayGroup = createCollection(PayGroup, {
		adjustPageSize: true, initNewRow: true, syncPosition: true, mergeToolbarWith: "ScreenToolbar"
	});
}

export class PayGroup extends PXView {
	PayGroupID: PXFieldState<PXFieldOptions.CommitChanges>;
	Description: PXFieldState;
	RoleName: PXFieldState;
	EarningsAcctID: PXFieldState;
	EarningsSubID: PXFieldState;
	DedLiabilityAcctID: PXFieldState;
	DedLiabilitySubID: PXFieldState;
	BenefitExpenseAcctID: PXFieldState;
	BenefitExpenseSubID: PXFieldState;
	BenefitLiabilityAcctID: PXFieldState;
	BenefitLiabilitySubID: PXFieldState;
	TaxExpenseAcctID: PXFieldState;
	TaxExpenseSubID: PXFieldState;
	TaxLiabilityAcctID: PXFieldState;
	TaxLiabilitySubID: PXFieldState;
	PTOExpenseAcctID: PXFieldState;
	PTOExpenseSubID: PXFieldState;
	PTOLiabilityAcctID: PXFieldState;
	PTOLiabilitySubID: PXFieldState;
	PTOAssetAcctID: PXFieldState;
	PTOAssetSubID: PXFieldState;
	IsDefault: PXFieldState<PXFieldOptions.CommitChanges>;
}
