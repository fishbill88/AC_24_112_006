import { createCollection, PXScreen, graphInfo, PXActionState } from "client-controls";
import { EPEarningType } from "./views";

@graphInfo({graphType: "PX.Objects.EP.EPEarningTypesSetup", primaryView: "EarningTypes" })
export class EP102000 extends PXScreen {
	RedirectToScreen: PXActionState;

	EarningTypes = createCollection(EPEarningType);

	async attached() {
		const payrollFeatureSet = this.features;
		// Check if the payroll module is installed.
		// If payroll is installed, redirect user to the payroll version of this page.
		if (payrollFeatureSet["PX.Objects.CS.FeaturesSet"]?.PayrollModule) {
			// Execute the server Action "RedirectToScreen", which will
			// redirect users to the payroll version of this page
			await this.screenService.executeCommand("RedirectToPayrollScreen");
			return;
		}

		await super.attached();
	}
}
