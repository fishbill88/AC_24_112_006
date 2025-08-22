import {
	graphInfo,
	PXScreen,
	PXView,
	PXFieldState,
	createSingle,
	headerDescription
} from "client-controls";

@graphInfo({ graphType: 'PX.Objects.FS.RoomMaint', primaryView: 'RoomRecords' })
export class FS203500 extends PXScreen {
	RoomRecords = createSingle(RoomMaint);
}

export class RoomMaint extends PXView {
	BranchLocationID: PXFieldState;
	RoomID: PXFieldState;
	Descr: PXFieldState;
	FloorNbr: PXFieldState;
	@headerDescription FormCaptionDescription: PXFieldState;
}
