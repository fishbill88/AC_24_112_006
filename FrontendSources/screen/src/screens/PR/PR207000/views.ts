import { PXView, PXFieldState, gridConfig, PXFieldOptions, PXActionState } from 'client-controls';

export class PRAcaCompanyYearlyInformation extends PXView  {
	OrgBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	Year: PXFieldState<PXFieldOptions.CommitChanges>;
	Ein: PXFieldState;
	IsPartOfAggregateGroup: PXFieldState<PXFieldOptions.CommitChanges>;
	IsAuthoritativeTransmittal: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class MonthFilter extends PXView  {
	Month: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({syncPosition: true})
export class PRAcaEmployeeMonthlyInformation extends PXView  {
	UpdateSelectedEmployees: PXActionState;
	UpdateAllEmployees: PXActionState;

	MinimumIndividualContribution: PXFieldState;
	Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	EPEmployee__AcctCD: PXFieldState;
	EPEmployee__AcctName: PXFieldState;
	Month: PXFieldState;
	FTStatus: PXFieldState<PXFieldOptions.CommitChanges>;
	OfferOfCoverage: PXFieldState<PXFieldOptions.CommitChanges>;
	Section4980H: PXFieldState;
	HoursWorked: PXFieldState;
}

@gridConfig({syncPosition: true})
export class PRAcaCompanyMonthlyInformation extends PXView  {
	UpdateSelectedCompanyMonths: PXActionState;
	UpdateAllCompanyMonths: PXActionState;

	Selected: PXFieldState<PXFieldOptions.CommitChanges>;
	Month: PXFieldState;
	NumberOfFte: PXFieldState;
	NumberOfEmployees: PXFieldState;
	PctEmployeesCoveredByMec: PXFieldState;
	CertificationOfEligibility: PXFieldState;
	SelfInsured: PXFieldState;
	Numberof1095C: PXFieldState;
}

export class PRAcaAggregateGroupMember extends PXView  {
	HighestMonthlyFteNumber: PXFieldState;
	MemberCompanyName: PXFieldState;
	MemberEin: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class PRAcaUpdateEmployeeFilter extends PXView  {
	UpdateAcaFTStatus: PXFieldState<PXFieldOptions.CommitChanges>;
	UpdateOfferOfCoverage: PXFieldState<PXFieldOptions.CommitChanges>;
	UpdateSection4980H: PXFieldState<PXFieldOptions.CommitChanges>;
	UpdateMinimumIndividualContribution: PXFieldState<PXFieldOptions.CommitChanges>;
	AcaFTStatus: PXFieldState;
	OfferOfCoverage: PXFieldState;
	Section4980H: PXFieldState;
	MinimumIndividualContribution: PXFieldState;
}

export class PRAcaUpdateCompanyMonthFilter extends PXView  {
	UpdateCertificationOfEligibility: PXFieldState<PXFieldOptions.CommitChanges>;
	UpdateSelfInsured: PXFieldState<PXFieldOptions.CommitChanges>;
	CertificationOfEligibility: PXFieldState;
	SelfInsured: PXFieldState;
}
