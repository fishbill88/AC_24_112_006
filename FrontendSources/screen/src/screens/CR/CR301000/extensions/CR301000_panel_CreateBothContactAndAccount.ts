import { CR301000 } from "../CR301000";
import {
	AccountsFilter,
	ContactFilter,
	PopupAttributes,
	PopupUDFAttributes,
} from "./CR301000_panel_view";
import { createSingle, createCollection, viewInfo } from "client-controls";

export interface CR301000_panel_CreateBothContactAndAccount extends CR301000 {}
export class CR301000_panel_CreateBothContactAndAccount {
	@viewInfo({ containerName: "Creation Dialog" })
	AccountInfo = createSingle(AccountsFilter);

	ContactInfo = createSingle(ContactFilter);

	@viewInfo({ containerName: "Create Account" })
	AccountInfoAttributes = createCollection(PopupAttributes);

	@viewInfo({ containerName: "Create Contact" })
	ContactInfoAttributes = createCollection(PopupAttributes);

	@viewInfo({ containerName: "Create Account" })
	AccountInfoUDF = createCollection(PopupUDFAttributes);

	@viewInfo({ containerName: "Create Contact" })
	ContactInfoUDF = createCollection(PopupUDFAttributes);
}
