import { createSingle, PXScreen, graphInfo, PXView, PXFieldState, PXFieldOptions } from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.TX.TXSetupMaint', primaryView: 'TXSetupRecord', })
export class TX103000 extends PXScreen {

	TXSetupRecord = createSingle(TXSetup);
}

export class TXSetup extends PXView {

	TaxAdjustmentNumberingID: PXFieldState;
	TaxRoundingGainAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxRoundingGainSubID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxRoundingLossAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxRoundingLossSubID: PXFieldState<PXFieldOptions.CommitChanges>;

}
