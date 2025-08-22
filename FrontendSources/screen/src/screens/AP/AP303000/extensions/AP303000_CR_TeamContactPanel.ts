import { AP303000 } from '../AP303000';
import { PXView, PXFieldState, createSingle } from 'client-controls';

export interface AP303000_CR_TeamsContactPanel extends AP303000 { }

// #include file="~\Pages\CR\Includes\TeamsContactPanel.inc"
export class AP303000_CR_TeamsContactPanel {

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
