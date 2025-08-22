import { SM204002 } from '../SM204002';
import {
	PXView,
	PXFieldState,
	createSingle,
} from "client-controls";

export interface SM204002_panel_SendTestEmail extends SM204002 { }
export class SM204002_panel_SendTestEmail {
    SendTestEmailFilter = createSingle(SendTestEmailFilter);
}

export class SendTestEmailFilter extends PXView {
	EmailAddress: PXFieldState;
}