import { graphInfo, PXScreen, PXView, PXFieldState, createCollection, gridConfig, columnConfig, PXFieldOptions } from "client-controls";

@graphInfo({ graphType: 'PX.AutocompleteGenerator.UI.AutocompleteSuggesterTrainingProcess', primaryView: 'Users', bpEventsIndicator: false })
export class SM508000 extends PXScreen {
	Users = createCollection(Users);

}

@gridConfig({allowInsert: false, allowDelete: false, allowUpdate: false, mergeToolbarWith: 'ScreenToolbar'})
export class Users extends PXView {
	ProcessingStatus: PXFieldState<PXFieldOptions.Hidden>;
	ProcessingMessage: PXFieldState<PXFieldOptions.Hidden>;
	@columnConfig({allowSort: false, allowCheckAll: true})
	Selected: PXFieldState;
	@columnConfig({hideViewLink: true})
	Username: PXFieldState;
	CreatedDateTime: PXFieldState;
	SuccessString: PXFieldState;
}
