import {
	AR301000
} from '../AR301000';

import {
	PXView,
	createCollection,
	createSingle,
	PXFieldState,
	PXFieldOptions,
	featureInstalled,
	PXActionState,
	linkCommand,
	columnConfig,
	localizable,
	viewInfo
} from 'client-controls';


export interface AR301000_Intercompany extends AR301000 { }

@featureInstalled('PX.Objects.CS.FeaturesSet+InterBranch')
export class AR301000_Intercompany {

	ViewRelatedAPDocument: PXActionState;

	@viewInfo({ containerName: "RelatedIntercompanyAPDocument" })
	RelatedIntercompanyAPDocument = createSingle(APInvoice);
}

export class APInvoice extends PXView {

	@linkCommand("ViewRelatedAPDocument")
	DocumentKey: PXFieldState;
}

