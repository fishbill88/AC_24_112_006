import { Messages as SysMessages } from "client-controls/services/messages";
import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	PXActionState,
	viewInfo,
	handleEvent,
	CustomEventType,
	RowSelectedHandlerArgs,
	PXViewCollection,
} from "client-controls";
import {
	LicensingKeyFilter,
	LicensingKeyWarning,
	LicenseKeyAgreement,
	LicenseInfo,
	LicenseFeature,
	LicenseDisclamer,
	UploadLisenceFilter,
	Eula
} from "./views";

@graphInfo({
	graphType: "PX.SM.LicensingSetup",
	primaryView: "License",
	hideFilesIndicator: true,
	hideNotesIndicator: true,
})
export class SM201510 extends PXScreen {
	@viewInfo({ containerName: "" })
	License = createSingle(LicenseInfo);
	@viewInfo({ containerName: "Activate New License" })
	LicenseKeyPanel = createSingle(LicensingKeyFilter);
	@viewInfo({ containerName: "Warning" })
	LicenseWarningPanel = createSingle(LicensingKeyWarning);
	@viewInfo({ containerName: "Agree to proceed" })
	LicenseAgreementPanel = createSingle(LicenseKeyAgreement);
	@viewInfo({ containerName: "Upload New License File" })
	UploadDialogPanel = createSingle(UploadLisenceFilter);
	@viewInfo({ containerName: "Operations" })
	Features = createCollection(LicenseFeature);
	LicenseDisclamer = LicenseDisclamer;
	Eula = Eula;
}
