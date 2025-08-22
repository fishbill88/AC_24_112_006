import { CR306000 } from '../CR306000';
import {
	PXView,
	PXFieldState,
	createSingle,
	PXFieldOptions
} from "client-controls";

export interface CR306000_panel_CreateServiceOrder extends CR306000 { }
export class CR306000_panel_CreateServiceOrder {
	CreateServiceOrderFilter = createSingle(CreateServiceOrderFilter);
}

export class CreateServiceOrderFilter extends PXView {
	SrvOrdType: PXFieldState<PXFieldOptions.CommitChanges>;
	BranchLocationID: PXFieldState<PXFieldOptions.CommitChanges>;
	AssignedEmpID: PXFieldState<PXFieldOptions.CommitChanges>;
	ProblemID: PXFieldState<PXFieldOptions.CommitChanges>;
}