import { CR301000 } from "../CR301000";
import {
	ContactFilter,
	PopupAttributes,
	PopupUDFAttributes,
} from "./CR301000_panel_view";
import { createSingle, createCollection, viewInfo } from "client-controls";

export interface CR301000_panel_CreateContact extends CR301000 {}
export class CR301000_panel_CreateContact {
	@viewInfo({ containerName: "Create Contact" })
	ContactInfo = createSingle(ContactFilter);

	@viewInfo({ containerName: "Create Contact" })
	ContactInfoAttributes = createCollection(PopupAttributes);

	@viewInfo({ containerName: "Create Contact" })
	ContactInfoUDF = createCollection(PopupUDFAttributes);
}
