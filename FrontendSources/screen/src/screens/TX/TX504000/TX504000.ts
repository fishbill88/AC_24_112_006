import { createCollection,createSingle,PXScreen,graphInfo,viewInfo,PXView,PXFieldState,PXFieldOptions } from 'client-controls';

@graphInfo({graphType: 'PX.Objects.TX.TaxByZipEnq', primaryView: 'Filter', })
export class TX504000 extends PXScreen {

	@viewInfo({containerName: 'Selection'})
	Filter = createSingle(TaxFilter);

	@viewInfo({containerName: 'Details'})
	Records = createCollection(TaxZoneDet);

}

export class TaxFilter extends PXView  {

	TaxDate: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxZoneID: PXFieldState<PXFieldOptions.CommitChanges>;
	TaxID: PXFieldState<PXFieldOptions.CommitChanges>;

}

export class TaxZoneDet extends PXView  {

	TaxZoneID: PXFieldState;
	ZipCode: PXFieldState;
	TaxID: PXFieldState;
	Descr: PXFieldState;
	TaxRate: PXFieldState;

}
