// TODO: disable FastFilter in the grid (https://jira.acumatica.com/browse/AC-290459)
// TODO: disable Refresh button in the grid (https://jira.acumatica.com/browse/AC-291874)

import {
	PXView,
	PXFieldState,
	gridConfig,
	columnConfig,
	graphInfo,
	PXScreen,
	viewInfo,
	createSingle,
	createCollection,
} from "client-controls";

@graphInfo({
	graphType: "PX.SM.CertificateChangeProcess",
	primaryView: "Filter",
})
export class SM200535 extends PXScreen {
	@viewInfo({ containerName: "Encryption Certificate Selection" })
	Filter = createSingle(ProcessFilter);
	@viewInfo({ containerName: "Encrypted Entities" })
	Process = createCollection(EncryptedSource);
}

export class ProcessFilter extends PXView {
	PendingCertificate: PXFieldState;
	CurrentCertificate: PXFieldState;
}

@gridConfig({
	allowDelete: false,
	allowInsert: false,
	allowUpdate: false,
	adjustPageSize: true,
	batchUpdate: true,
})
export class EncryptedSource extends PXView {
	EntityType: PXFieldState;
	EntityName: PXFieldState;
}
