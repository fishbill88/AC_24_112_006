import {
	PXScreen,
	PXActionState,
	createSingle,
	createCollection,
	graphInfo
} from 'client-controls';

import {
	ClaimDetails,
	CurrentClaimDetails,
	Taxes,
	Approval,
	CurrencyInfo,
	ReasonApproveRejectParams,
	ReassignApprovalFilter
} from './views';

@graphInfo({
	graphType: 'PX.Objects.EP.ExpenseClaimDetailEntry',
	primaryView: 'ClaimDetails',
	showUDFIndicator: true
})
export class EP301020 extends PXScreen {
	SaveTaxZone: PXActionState;

	ClaimDetails = createSingle(ClaimDetails);
	CurrentClaimDetails = createSingle(CurrentClaimDetails);
	Taxes = createCollection(Taxes);
	Approval = createCollection(Approval);
	CurrencyInfo = createSingle(CurrencyInfo);
	ReasonApproveRejectParams = createSingle(ReasonApproveRejectParams);
	ReassignApprovalFilter = createSingle(ReassignApprovalFilter);
}

