import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	viewInfo,
} from "client-controls";
import {
	LienWaiverSetup,
	NotificationSetup,
	NotificationSetupRecipient,
	ComplianceAttributeFilter,
	ComplianceAttribute,
	CSAttributeGroup
} from "./views";

@graphInfo({
	graphType: "PX.Objects.CN.Compliance.CL.Graphs.ComplianceDocumentSetupMaint",
	primaryView: "LienWaiverSetup",
})
export class CL301000 extends PXScreen {

	LienWaiverSetup = createSingle(LienWaiverSetup);
	@viewInfo({ containerName: "Default Sources" })
	ComplianceNotifications = createCollection(NotificationSetup);
	@viewInfo({ containerName: "Default Recipients" })
	Recipients = createCollection(NotificationSetupRecipient);
	@viewInfo({ containerName: "Attribute Group" })
	Filter = createSingle(ComplianceAttributeFilter);
	@viewInfo({ containerName: "Attributes" })
	Mapping = createCollection(ComplianceAttribute);
	@viewInfo({ containerName: "Attributes" })
	MappingCommon = createCollection(CSAttributeGroup);
}
