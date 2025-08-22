import
{
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	viewInfo
} from 'client-controls';

import {
	PRAcaCompanyYearlyInformation,
	MonthFilter,
	PRAcaEmployeeMonthlyInformation,
	PRAcaCompanyMonthlyInformation,
	PRAcaAggregateGroupMember,
	PRAcaUpdateEmployeeFilter,
	PRAcaUpdateCompanyMonthFilter
} from './views';

@graphInfo({graphType: 'PX.Objects.PR.PRAcaReportingMaint', primaryView: 'CompanyYearlyInformation', })
export class PR207000 extends PXScreen {
	@viewInfo({containerName: ''})
	CompanyYearlyInformation = createSingle(PRAcaCompanyYearlyInformation);

	@viewInfo({ containerName: 'Employee' })
	EmployeeMonthFilter = createSingle(MonthFilter);

	@viewInfo({ containerName: 'Employee' })
	FilteredEmployeeInformation = createCollection(PRAcaEmployeeMonthlyInformation);

	@viewInfo({containerName: 'Company'})
	CompanyMonthlyInformation = createCollection(PRAcaCompanyMonthlyInformation);

	@viewInfo({containerName: 'Aggregate Group'})
	AggregateGroupInformation = createCollection(PRAcaAggregateGroupMember);

	@viewInfo({containerName: 'Mass Update'})
	EmployeeUpdate = createSingle(PRAcaUpdateEmployeeFilter);

	@viewInfo({ containerName: 'Mass Update' })
	CompanyUpdate = createSingle(PRAcaUpdateCompanyMonthFilter);
}
