import { CR302000 } from "../CR302000";
import {
	AccountsFilter,
	ContactFilter,
	PopupAttributes,
	PopupUDFAttributes,
} from "./CR302000_panel_view";
import { createSingle, createCollection, viewInfo } from "client-controls";

export interface CR302000_panel_CreateBothContactAndAccount extends CR302000 {}
export class CR302000_panel_CreateBothContactAndAccount {
	@viewInfo({ containerName: "Creation Dialog" })
	AccountInfo = createSingle(AccountsFilter);

	@viewInfo({ containerName: "Create Account" })
	AccountInfoAttributes = createCollection(PopupAttributes);

	@viewInfo({ containerName: "Create Account" })
	AccountInfoUDF = createCollection(PopupUDFAttributes);
}
