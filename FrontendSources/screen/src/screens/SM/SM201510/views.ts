import {
	PXView,
	PXFieldState,
	gridConfig,
	headerDescription,
	ICurrencyInfo,
	disabled,
	selectorSettings,
	PXFieldOptions,
	linkCommand,
	columnConfig,
	GridColumnShowHideMode,
	GridColumnType,
	PXActionState,
	localizable,
} from "client-controls";

// Views

@localizable
export class LicenseDisclamer {
	static LicenseText =
		"Disclaimer: The license Key is a 20-character alphanumeric string (example: 1234-5678-90AB-CDEF-1234) that your partner receives from Acumatica upon completion of software contract signing. \nIf your system has been upgraded or reinstalled, please contact your partner to get a new license immediately.";
	static LicenseWarning =
		"This license is being used by at least one other Acumatica instance and cannot be used for one more instance simultaneously. If you activate the license, it will be deactivated from the oldest active site.\nAre you sure you want to proceed and deactivate the license from another active site?"
}

@localizable
export class Eula {
	static EulaText = "To continue activation of the new license you must agree to the terms of the ";
	static EulaLinkText = "software license agreement.";
	static EulaLink = "";
}

export class UploadLisenceFilter extends PXView {}

export class LicensingKeyFilter extends PXView {
	LicensingKey: PXFieldState;
}

export class LicensingKeyWarning extends PXView {}

export class LicenseKeyAgreement extends PXView {}

export class LicenseInfo extends PXView {
	Status: PXFieldState<PXFieldOptions.Disabled>;
	Preview: PXFieldState<PXFieldOptions.Disabled>;
	ValidFrom: PXFieldState<PXFieldOptions.Disabled>;
	Processors: PXFieldState<PXFieldOptions.Disabled>;
	Users: PXFieldState<PXFieldOptions.Disabled>;
	Valid: PXFieldState<PXFieldOptions.Disabled>;
	ValidTo: PXFieldState<PXFieldOptions.Disabled>;
	Version: PXFieldState<PXFieldOptions.Disabled>;
	Companies: PXFieldState<PXFieldOptions.Disabled>;
}

@gridConfig({ adjustPageSize: true })
export class LicenseFeature extends PXView {
	@columnConfig({ allowUpdate: false })
	Enabled: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Name: PXFieldState;
}
