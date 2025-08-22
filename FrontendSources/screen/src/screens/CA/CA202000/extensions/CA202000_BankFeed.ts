import { createSingle, viewInfo, PXView, PXFieldState, featureInstalled } from 'client-controls';

import { CA202000 } from '../CA202000';

export interface CA202000_BankFeed extends CA202000 {}
@featureInstalled('PX.Objects.CS.FeaturesSet+BankFeedIntegration')
export class CA202000_BankFeed {
	@viewInfo({containerName: 'Cash Account Summary'})
	BankFeed = createSingle(CABankFeed);
}

@featureInstalled('PX.Objects.CS.FeaturesSet+BankFeedIntegration')
export class CABankFeed extends PXView {
	StatementImportSource: PXFieldState;
}
