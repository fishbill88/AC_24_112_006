import {
	SO301000
} from '../SO301000';

import {
	PXView,
	createSingle,
	PXFieldState,
	PXFieldOptions,
	viewInfo
} from 'client-controls';

export interface SO301000_FieldServices extends SO301000 {}
export class SO301000_FieldServices {

	@viewInfo({containerName: "Create Service Order"})
	CreateServiceOrderFilter = createSingle(CreateServiceOrderFilter);
}

export class CreateServiceOrderFilter extends PXView {
	SrvOrdType: PXFieldState<PXFieldOptions.CommitChanges>;
	AssignedEmpID: PXFieldState<PXFieldOptions.CommitChanges>;
	SLAETA_Date: PXFieldState;
	SLAETA_Time: PXFieldState<PXFieldOptions.NoLabel>;
}

