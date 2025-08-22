import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
} from "client-controls";

import { NotificationSource, NotificationRecipient } from "../../interfaces/CR/MailingPrintingSettings";

@graphInfo({
	graphType: "PX.Objects.CR.CRSetupMaint",
	primaryView: "CRSetupRecord",
})
export class CR101000 extends PXScreen {
	CRSetupRecord = createSingle(CRSetupRecord);
	Notifications = createCollection(NotificationSource);
	Recipients = createCollection(NotificationRecipient);
}

class CRSetupRecord extends PXView {
	OpportunityNumberingID: PXFieldState;
	CaseNumberingID: PXFieldState;
	MassMailNumberingID: PXFieldState;
	CampaignNumberingID: PXFieldState;
	QuoteNumberingID: PXFieldState;
	DefaultLeadClassID: PXFieldState;
	DefaultContactClassID: PXFieldState;
	DefaultCustomerClassID: PXFieldState;
	DefaultOpportunityClassID: PXFieldState;
	DefaultCaseClassID: PXFieldState;
	AMEstimateEntry: PXFieldState;
	AMConfigurationEntry: PXFieldState;
	DuplicateScoresNormalization: PXFieldState;
	CopyNotes: PXFieldState;
	CopyFiles: PXFieldState;
	DefaultRateTypeID: PXFieldState;
	AllowOverrideRate: PXFieldState;
	LeadDefaultAssignmentMapID: PXFieldState;
	ContactDefaultAssignmentMapID: PXFieldState;
	DefaultBAccountAssignmentMapID: PXFieldState;
	DefaultOpportunityAssignmentMapID: PXFieldState;
	DefaultCaseAssignmentMapID: PXFieldState;
	QuoteApprovalMapID: PXFieldState;
	QuoteApprovalNotificationID: PXFieldState;
}
