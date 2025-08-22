import { CR301000 } from "../CR301000";
import {
	OpportunityFilter,
	AccountsFilter,
	ContactFilter,
	PopupAttributes,
	PopupUDFAttributes,
} from "./CR301000_panel_view";
import { createSingle, createCollection, viewInfo } from "client-controls";

export interface CR301000_panel_CreateOpportunityAll extends CR301000 {}
export class CR301000_panel_CreateOpportunityAll {
	@viewInfo({ containerName: "Create Opportunity" })
	OpportunityInfo = createSingle(OpportunityFilter);

	@viewInfo({ containerName: "Creation Dialog" })
	AccountInfo = createSingle(AccountsFilter);

	@viewInfo({ containerName: "Create Contact" })
	ContactInfo = createSingle(ContactFilter);

	@viewInfo({ containerName: "Create Opportunity" })
	OpportunityInfoAttributes = createCollection(PopupAttributes);

	@viewInfo({ containerName: "Create Account" })
	AccountInfoAttributes = createCollection(PopupAttributes);

	@viewInfo({ containerName: "Create Contact" })
	ContactInfoAttributes = createCollection(PopupAttributes);

	@viewInfo({ containerName: "Create Opportunity" })
	OpportunityInfoUDF = createCollection(PopupUDFAttributes);

	@viewInfo({ containerName: "Create Account" })
	AccountInfoUDF = createCollection(PopupUDFAttributes);

	@viewInfo({ containerName: "Create Contact" })
	ContactInfoUDF = createCollection(PopupUDFAttributes);
}
