import { SM202025 } from '../SM202025';
import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	createSingle,
} from "client-controls";


export interface SM202025_panel_UpdatePages extends SM202025 { }
export class SM202025_panel_UpdatePages {
	fltArticlesProps = createSingle(WikiPageUpdatableProps);
}

export class WikiPageUpdatableProps extends PXView {
	ParentUID: PXFieldState<PXFieldOptions.CommitChanges>;
	TagID: PXFieldState<PXFieldOptions.CommitChanges>;
	Keywords: PXFieldState<PXFieldOptions.CommitChanges | PXFieldOptions.Multiline>;
	Hold: PXFieldState<PXFieldOptions.CommitChanges>;
	Versioned: PXFieldState<PXFieldOptions.CommitChanges>;
}