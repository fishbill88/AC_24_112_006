import { createSingle, createCollection, viewInfo } from "client-controls";
import { AccountsFilter, PopupAttributes, PopupUDFAttributes } from "../views";

export abstract class PanelCreateBothContactAndAccountBase {
	@viewInfo({ containerName: "Creation Dialog" })
	AccountInfo = createSingle(AccountsFilter);
	@viewInfo({ containerName: "Create Account" })
	AccountInfoAttributes = createCollection(PopupAttributes);

	@viewInfo({ containerName: "Create Account" })
	AccountInfoUDF = createCollection(PopupUDFAttributes);
	/*
	@viewInfo({ containerName: "Create Contact" })
	ContactInfo = createSingle(ContactFilter);

	@viewInfo({ containerName: "Create Contact" })
	ContactInfoAttributes = createCollection(PopupAttributes);

	@viewInfo({ containerName: "Create Contact" })
	ContactInfoUDF = createCollection(PopupUDFAttributes);
	*/
}
/*
should be used with panel-create-contact.ts
*/
