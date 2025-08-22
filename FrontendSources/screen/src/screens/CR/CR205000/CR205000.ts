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


@graphInfo({ graphType: 'PX.Objects.CR.CRContactClassMaint', primaryView: 'Class' })
export class CR205000 extends PXScreen {
	Class = createSingle(CRContactClass);
	Mapping = createCollection(CSAttributeGroup);
}

class CRContactClass extends PXView {
	ClassID: PXFieldState;
	IsInternal: PXFieldState;
	Description: PXFieldState;
	DefaultOwner: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultAssignmentMapID: PXFieldState;
	TargetLeadClassID: PXFieldState;
	TargetBAccountClassID: PXFieldState;
	TargetOpportunityClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	TargetOpportunityStage: PXFieldState;
	DefaultEMailAccountID: PXFieldState;
}
