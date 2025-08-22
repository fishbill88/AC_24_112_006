import {
	PXScreen,
	createCollection,
	createSingle,
	graphInfo,
} from 'client-controls';

import {
	EarningTypes,
	EarningSettings,
	PRRegularTypesForOvertime,
	EarningTypeTaxesUS,
	EarningTypeTaxesCAN,
} from './views';

@graphInfo({
	graphType: 'PX.Objects.PR.PREarningTypeMaint',
	primaryView: 'EarningTypes'
})
export class PR102000 extends PXScreen {
	EarningTypes = createSingle(EarningTypes);
	EarningSettings = createSingle(EarningSettings);
	PRRegularTypesForOvertime = createCollection(PRRegularTypesForOvertime);
	EarningTypeTaxesUS = createCollection(EarningTypeTaxesUS);
	EarningTypeTaxesCAN = createCollection(EarningTypeTaxesCAN);

	// Default to assuming US Payroll is enabled
	ShowUSTab = true;
	ShowCANTab = false;

	async attached() {
		const payrollFeatureSet = this.features;

		// If Canadian payroll is enabled, then we need to update our flags accordingly
		if (payrollFeatureSet["PX.Objects.CS.FeaturesSet"]?.PayrollCAN) {
			this.ShowUSTab = false;
			this.ShowCANTab = true;
		}

		await super.attached();
	}
}

