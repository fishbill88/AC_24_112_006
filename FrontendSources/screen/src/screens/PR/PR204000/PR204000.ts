import { createCollection, createSingle, PXScreen, graphInfo, viewInfo } from "client-controls";
import { PRPTOBank, PRPTOBank2, PREmployeeClassPTOBank, PRBandingRulePTOBank } from "./views";

@graphInfo({graphType: "PX.Objects.PR.PTOBankMaint", primaryView: "Bank" })
export class PR204000 extends PXScreen {
	Bank = createSingle(PRPTOBank);
	CurrentBank = createSingle(PRPTOBank2);
   	@viewInfo({containerName: "Employee Classes"})
	EmployeeClassPTOBanks = createCollection(PREmployeeClassPTOBank);
   	@viewInfo({containerName: "Banding rules"})
	BandingRulePTOBanks = createCollection(PRBandingRulePTOBank);
	
	CANPayrollEnabled = false;

	async attached() {
		const payrollFeatureSet = this.features;

		if (payrollFeatureSet["PX.Objects.CS.FeaturesSet"]?.PayrollCAN) {
			this.CANPayrollEnabled = true;
		}

		await super.attached();
	}
}
