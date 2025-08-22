import {
	PXScreen,
	createSingle,
	createCollection,
	graphInfo
} from 'client-controls';

import {
	EmployeeClass,
	CurEmployeeClassRecord,
	WorkLocations,
} from './views';

@graphInfo({
	graphType: 'PX.Objects.PR.PREmployeeClassMaint',
	primaryView: 'EmployeeClass'
})
export class PR202000 extends PXScreen {
	EmployeeClass = createSingle(EmployeeClass);
	CurEmployeeClassRecord = createSingle(CurEmployeeClassRecord);
	WorkLocations = createCollection(WorkLocations);
}

