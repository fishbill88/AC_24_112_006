import {
	AR301000
} from '../AR301000';

import {
	PXView, createCollection, createSingle, PXFieldState, PXFieldOptions, featureInstalled, PXActionState,
	linkCommand, localizable, gridConfig, columnConfig, viewInfo
} from 'client-controls';


export interface AR301000_Retainage extends AR301000 { }

@featureInstalled('PX.Objects.CS.FeaturesSet+Retainage')
export class AR301000_Retainage {

	@viewInfo({ containerName: "Release Retainage" })
	ReleaseRetainageOptions = createSingle(RetainageOptions);
}

export class RetainageOptions extends PXView {

	@columnConfig({ allowNull: false })
	RetainagePct: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowNull: false })
	CuryRetainageAmt: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowNull: false })
	CuryRetainageUnreleasedAmt: PXFieldState<PXFieldOptions.CommitChanges>;
}
