import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
	gridConfig,
	GridPreset,
} from "client-controls";

import { CSAttributeGroup } from "../../interfaces/CR/AttributeGroup";

@graphInfo({ graphType: 'PX.Objects.CR.CROpportunityClassMaint', primaryView: 'OpportunityClass' })
export class CR209000 extends PXScreen {
	OpportunityClass = createSingle(CROpportunityClass);
	Mapping = createCollection(CSAttributeGroup);
	OpportunityProbabilities = createCollection(CROpportunityProbability);
}

class CROpportunityClass extends PXView {
	CROpportunityClassID: PXFieldState;
	IsInternal: PXFieldState;
	Description: PXFieldState;
	DefaultOwner: PXFieldState<PXFieldOptions.CommitChanges>;
	DefaultAssignmentMapID: PXFieldState;
	TargetContactClassID: PXFieldState;
	TargetBAccountClassID: PXFieldState;
	DefaultEMailAccountID: PXFieldState;
	ShowContactActivities: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	fastFilterByAllFields: false,
	suppressNoteFiles: true,
	autoAdjustColumns: true,
})
class CROpportunityProbability extends PXView {
	IsActive: PXFieldState;
	StageCode: PXFieldState;
	Name: PXFieldState;
	Probability: PXFieldState<PXFieldOptions.CommitChanges>;
	SortOrder: PXFieldState;
}
