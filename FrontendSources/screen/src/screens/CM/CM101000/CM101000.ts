import { createSingle, PXScreen, graphInfo, PXView, PXFieldState } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.CM.CMSetupMaint', primaryView: 'cmsetup' })
export class CM101000 extends PXScreen {

	cmsetup = createSingle(CMSetup);

}

export class CMSetup extends PXView {

	BatchNumberingID: PXFieldState;
	TranslNumberingID: PXFieldState;
	ExtRefNbrNumberingID: PXFieldState;
	GainLossSubMask: PXFieldState;
	AutoPostOption: PXFieldState;
	RateVarianceWarn: PXFieldState;
	RateVariance: PXFieldState;
	TranslDefId: PXFieldState;
	GLRateTypeDflt: PXFieldState;
	GLRateTypeReval: PXFieldState;
	CARateTypeDflt: PXFieldState;
	ARRateTypeDflt: PXFieldState;
	ARRateTypeReval: PXFieldState;
	APRateTypeDflt: PXFieldState;
	APRateTypeReval: PXFieldState;
	PMRateTypeDflt: PXFieldState;

}
