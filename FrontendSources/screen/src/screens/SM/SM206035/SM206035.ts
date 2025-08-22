import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs, PXViewCollection } from 'client-controls';
import { SYHistory, SYData, SYReplace, SYSubstitutionInfo, SYImportOperation, SYMapping } from './views';

@graphInfo({ graphType: 'PX.Api.SYImportProcess', primaryView: 'Operation', })
export class SM206035 extends PXScreen {
	viewReplacement: PXActionState;
	addSubstitutions: PXActionState;
	viewHistory: PXActionState;
	viewPreparedData: PXActionState;
	savePreparedData: PXActionState;
	replaceOneValue: PXActionState;
	replaceAllValues: PXActionState;
	saveSubstitutions: PXActionState;
	closeSubstitutions: PXActionState;

	@viewInfo({ containerName: 'Parameters' })
	Operation = createSingle(SYImportOperation);

	@viewInfo({ containerName: 'Scenarios' })
	Mappings = createCollection(SYMapping);

	@viewInfo({ containerName: 'History' })
	History = createCollection(SYHistory);

	@viewInfo({ containerName: 'Prepared Data' })
	PreparedData = createCollection(SYData);

	@viewInfo({ containerName: 'Replace' })
	ReplacementProperties = createSingle(SYReplace);
	@viewInfo({ containerName: 'Add Substitution' })
	SubstitutionInfo = createCollection(SYSubstitutionInfo);

	@handleEvent(CustomEventType.RowSelected, { view: "PreparedData" })
	onSyDataRowSelected(args: RowSelectedHandlerArgs<PXViewCollection<SYData>>) {
		const model = (<any>args.viewModel as SYData);

		// temporary solution with processing nulllabel value until the bug with dynamic columns is not resolved
		model.addSubstitutions.enabled = args.viewModel.activeRow && (args.viewModel.activeRow?.CanAddSubstitutions?.value ?? true);
		model.viewReplacement.enabled = args.viewModel.activeRow != null;
	}
}