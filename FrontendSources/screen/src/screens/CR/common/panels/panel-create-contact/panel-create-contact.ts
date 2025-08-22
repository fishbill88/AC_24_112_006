import { createSingle, createCollection, viewInfo } from "client-controls";
import { PopupAttributes, PopupUDFAttributes } from "../views";
import { ContactFilter } from "../../forms/form-create-contact/form-create-contact";

export abstract class PanelCreateContactBase {
	@viewInfo({ containerName: "Create Contact" })
	ContactInfo = createSingle(ContactFilter);

	@viewInfo({ containerName: "Create Contact" })
	ContactInfoAttributes = createCollection(PopupAttributes);

	@viewInfo({ containerName: "Create Contact" })
	ContactInfoUDF = createCollection(PopupUDFAttributes);
}
