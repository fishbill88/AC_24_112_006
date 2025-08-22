import {
	createCollection,
	PXScreen,
	PXView,
	graphInfo,
	PXFieldState,
	columnConfig,
	gridConfig,
	GridPreset
} from 'client-controls';

@graphInfo({ graphType: 'PX.Objects.EP.DepartmentMaint', primaryView: 'EPDepartment' })
export class EP201500 extends PXScreen {
	EPDepartment = createCollection(EPDepartment);
}

@gridConfig({
	preset: GridPreset.Primary,
	fastFilterByAllFields: false,
})
export class EPDepartment extends PXView {
	DepartmentID: PXFieldState;
	Description: PXFieldState;
	@columnConfig({hideViewLink: true})
	ExpenseAccountID: PXFieldState;
	@columnConfig({hideViewLink: true})
	ExpenseSubID: PXFieldState;
}
