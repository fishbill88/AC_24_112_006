import { AR303000 } from '../AR303000';
import { PXView, createCollection, PXFieldState, PXFieldOptions, linkCommand, gridConfig, PXActionState, createSingle } from 'client-controls';

export interface AR303000_CR_TeamsContactPanel extends AR303000 { }

// #include file="~\Pages\CR\Includes\TeamsContactPanel.inc"
export class AR303000_CR_TeamsContactPanel {

	TeamsContactCard = createSingle(TeamsContactCard);
	TeamsOwnerCard = createSingle(TeamsContactCard);

}

export class TeamsContactCard extends PXView {

	PhotoFileName: PXFieldState;
	DisplayName: PXFieldState;
	TeamsStatus: PXFieldState;
	JobTitle: PXFieldState;
	CompanyName: PXFieldState;
	Email: PXFieldState;
	MobilePhone: PXFieldState;

}
