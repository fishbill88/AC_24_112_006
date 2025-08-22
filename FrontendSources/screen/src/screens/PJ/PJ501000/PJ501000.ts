import {
	createCollection,
	graphInfo,
	PXActionState,
	PXScreen
} from "client-controls";

import {
	RequestsForInformation
} from "./views";

@graphInfo({
	graphType: "PX.Objects.PJ.RequestsForInformation.PJ.Graphs.AssignRequestForInformationMassProcess",
	primaryView: "RequestsForInformation"
})
export class PJ501000 extends PXScreen {
	RequestsForInformation_ViewDetails: PXActionState;
	RequestsForInformation_EntityDetails: PXActionState;
	RequestsForInformation_ContactDetails: PXActionState;

	RequestsForInformation = createCollection(RequestsForInformation);
}
