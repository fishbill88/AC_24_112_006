import { CR306000 } from '../CR306000';
import {
	PXView,
	PXFieldState,
	createSingle,
	PXFieldOptions
} from "client-controls";

export interface CR306000_panel_CreateReturnOrder extends CR306000 { }
export class CR306000_panel_CreateReturnOrder {
	CreateOrderParams = createSingle(CreateReturnOrderParams);
}

export class CreateReturnOrderParams extends PXView {
	OrderType: PXFieldState<PXFieldOptions.CommitChanges>;
}