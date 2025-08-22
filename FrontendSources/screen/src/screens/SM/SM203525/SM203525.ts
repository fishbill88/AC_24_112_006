import { SpaceUsageCalculationHistory,TableSize,PopupCompaniesByTableDefinition,UPCompany,UPSnapshot,Tables, PopupCompanyTablesHeader, PopupSnapshotTablesHeader, PopupCompaniesByTableHeader } from './views';
import { graphInfo, PXScreen, viewInfo, createSingle, createCollection, PXActionState, PXPageLoadBehavior } from "client-controls";

@graphInfo({graphType: 'PX.SM.SpaceUsageMaint', primaryView: 'CalculationHistory', pageLoadBehavior: PXPageLoadBehavior.GoLastRecord})
export class SM203525 extends PXScreen {
	ViewCompany: PXActionState;
	ViewSnapshot: PXActionState;

	@viewInfo({containerName: 'Company Summary'})
	CalculationHistory = createSingle(SpaceUsageCalculationHistory);

	@viewInfo({containerName: 'Tenants'})
	Companies = createCollection(UPCompany);
	@viewInfo({containerName: 'Snapshots'})
	Snapshots = createCollection(UPSnapshot);
	@viewInfo({containerName: 'Tables'})
	Tables = createCollection(Tables);

	@viewInfo({containerName: 'Table Sizes by Tenant'})
	PopupCompanyTablesDefinition = createCollection(TableSize);
	PopupCompanyTablesHeader = createSingle(PopupCompanyTablesHeader);
	@viewInfo({containerName: 'Table Sizes by Snapshot'})
	PopupSnapshotTablesDefinition = createCollection(TableSize);
	PopupSnapshotTablesHeader = createSingle(PopupSnapshotTablesHeader);
	@viewInfo({containerName: 'Used Space by Tenants and Snapshots'})
	PopupCompaniesByTableDefinition = createCollection(PopupCompaniesByTableDefinition);
	PopupCompaniesByTableHeader = createSingle(PopupCompaniesByTableHeader);

}