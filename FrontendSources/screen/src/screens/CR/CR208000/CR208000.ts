import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
} from "client-controls";

import { CSAttributeGroup } from "../../interfaces/CR/AttributeGroup";
import { NotificationSource, NotificationRecipient } from "../../interfaces/CR/MailingPrintingSettings";

@graphInfo({ graphType: 'PX.Objects.CR.CRCustomerClassMaint', primaryView: 'CustomerClass' })
export class CR208000 extends PXScreen {
	CustomerClass = createSingle(CRCustomerClass);
	Mapping = createCollection(CSAttributeGroup);
	NotificationSources = createCollection(NotificationSource);
	NotificationRecipients = createCollection(NotificationRecipient);
}

class CRCustomerClass extends PXView {
	CRCustomerClassID: PXFieldState;
	IsInternal: PXFieldState;
	Description: PXFieldState;
	DefaultOwner: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultAssignmentMapID: PXFieldState;
	DefaultEMailAccountID: PXFieldState;
	CuryID: PXFieldState;
	AllowOverrideCury: PXFieldState;
}
