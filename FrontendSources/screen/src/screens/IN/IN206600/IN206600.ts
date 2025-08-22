import { Messages as SysMessages } from 'client-controls/services/messages';
import { createCollection, createSingle, PXScreen, graphInfo, PXActionState, viewInfo, handleEvent, CustomEventType, RowSelectedHandlerArgs, PXViewCollection, PXPageLoadBehavior } from 'client-controls';
import { INReplenishmentPolicy, INReplenishmentSeason } from './views';

@graphInfo({graphType: 'PX.Objects.IN.INReplenishmentPolicyMaint', primaryView: 'Policies'})
export class IN206600 extends PXScreen {
	@viewInfo({containerName: 'Seasonality Summary'})
	Policies = createSingle(INReplenishmentPolicy);
	@viewInfo({containerName: 'Low Seasons'})
	seasons = createCollection(INReplenishmentSeason);
}
