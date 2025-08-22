import {
	CS102000
} from '../CS102000';

import {
	PXView, PXFieldState,
	createSingle, viewInfo, featureInstalled
} from 'client-controls';

export interface CS102000_PayrollCAN extends CS102000 { }

@featureInstalled('PX.Objects.CS.FeaturesSet+PayrollCAN')
export class CS102000_PayrollCAN {
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
