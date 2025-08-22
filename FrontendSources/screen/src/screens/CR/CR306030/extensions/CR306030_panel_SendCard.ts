import { CR306030 } from '../CR306030';
import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	createSingle,
} from "client-controls";

export interface CR306030_panel_SendCard extends CR306030 { }
export class CR306030_panel_SendCard {
	SendCardSettings = createSingle(SendCardFilter);
}

export class SendCardFilter extends PXView {
	Email: PXFieldState<PXFieldOptions.CommitChanges>;
}