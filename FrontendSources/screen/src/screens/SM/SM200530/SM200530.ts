// TODO: disable FastFilter in the grid (https://jira.acumatica.com/browse/AC-290459)
// TODO: hide Import from Excel button (https://jira.acumatica.com/browse/AC-290457)

import {
	PXScreen,
	createCollection,
	graphInfo,
	PXView,
	PXFieldState,
	gridConfig,
} from "client-controls";

@graphInfo({
	graphType: "PX.SM.CertificateMaintenance",
	primaryView: "Certificates",
})
export class SM200530 extends PXScreen {
	Certificates = createCollection(Certificate);
}

@gridConfig({
	mergeToolbarWith: "ScreenToolbar",
})
export class Certificate extends PXView {
	Name: PXFieldState;
	Password: PXFieldState;
}
