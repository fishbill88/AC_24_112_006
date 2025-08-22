import {
	CS101500
} from '../CS101500';

import {
	PXView, PXFieldState,
	createSingle, viewInfo, featureInstalled
} from 'client-controls';

export interface CS101500_CanadianPayroll extends CS101500 { }

@featureInstalled('PX.Objects.CS.FeaturesSet+PayrollCAN')
export class CS101500_CanadianPayroll {
	@viewInfo({ containerName: 'Canadian Tax Reporting' })
	TaxReportingAccount = createSingle(PRTaxReportingAccount);
}

export class PRTaxReportingAccount extends PXView {
	CRAPayrollAccountNumber: PXFieldState;
	RL1IdentificationNumber: PXFieldState;
	RL1FileNumber: PXFieldState;
	RL1QuebecEnterpriseNumber: PXFieldState;
	RL1QuebecTransmitterNumber: PXFieldState;
}
