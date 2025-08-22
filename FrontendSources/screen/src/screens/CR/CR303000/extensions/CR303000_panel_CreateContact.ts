import { CR303000 } from "../CR303000";
import {
	ContactFilter,
	PopupAttributes,
	PopupUDFAttributes,
} from "./CR303000_panel_view";
import { createSingle, createCollection, viewInfo } from "client-controls";

export interface CR303000_panel_CreateContact extends CR303000 {}
export class CR303000_panel_CreateContact {
	@viewInfo({ containerName: "Create Contact" })
	ContactInfo = createSingle(ContactFilter);

	@viewInfo({ containerName: "Create Contact" })
	ContactInfoAttributes = createCollection(PopupAttributes);

	@viewInfo({ containerName: "Create Contact" })
	ContactInfoUDF = createCollection(PopupUDFAttributes);
}
