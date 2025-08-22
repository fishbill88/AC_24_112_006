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

@graphInfo({ graphType: 'PX.Objects.CR.CRLeadClassMaint', primaryView: 'Class' })
export class CR207000 extends PXScreen {
	Class = createSingle(CRLeadClass);
	Mapping = createCollection(CSAttributeGroup);
}

class CRLeadClass extends PXView {
	ClassID: PXFieldState;
	IsInternal: PXFieldState;
	Description: PXFieldState;
	DefaultSource: PXFieldState;
	DefaultOwner: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultAssignmentMapID: PXFieldState;
	TargetContactClassID: PXFieldState;
	TargetBAccountClassID: PXFieldState;
	RequireBAccountCreation: PXFieldState;
	TargetOpportunityClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	TargetOpportunityStage: PXFieldState;
	DefaultEMailAccountID: PXFieldState;
}
