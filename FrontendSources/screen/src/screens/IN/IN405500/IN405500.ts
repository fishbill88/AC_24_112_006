import { Messages as SysMessages } from 'client-controls/services/messages';
import { INDeadStockEnqFilter,INDeadStockEnqResult } from './views';
import { PXScreen, createCollection, createSingle, graphInfo, viewInfo } from 'client-controls';

@graphInfo({graphType: 'PX.Objects.IN.INDeadStockEnq', primaryView: 'Filter', bpEventsIndicator: false, udfTypeField: ''})
export class IN405500 extends PXScreen {
	@viewInfo({containerName: 'Filter'})
	Filter = createSingle(INDeadStockEnqFilter);
	@viewInfo({containerName: 'Result'})
	Result = createCollection(INDeadStockEnqResult);
}
