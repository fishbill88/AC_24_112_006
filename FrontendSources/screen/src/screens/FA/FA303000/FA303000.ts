import {
	PXScreen, createSingle, createCollection, graphInfo, PXActionState
} from 'client-controls';
import {
	FixedAsset,
	FADetails,
	FALocationHistory,
	FABookBalance,
	FAComponent,
	FAHistory,
	DeprBookFilter,
	FASheetHistory,
	FASetup,
	TranBookFilter,
	FATran,
	GLTranFilter,
	DsplFAATran,
	DisposeParams,
	SuspendParameters,
	ReverseDisposalInfo,
} from './views';

@graphInfo({ graphType: 'PX.Objects.FA.AssetMaint', primaryView: 'Asset', udfTypeField: "AssetTypeID", showUDFIndicator: true })
export class FA303000 extends PXScreen {
	Asset = createSingle(FixedAsset);
	CurrentAsset = createSingle(FixedAsset);
	AssetDetails = createSingle(FADetails);
	AssetLocation = createSingle(FALocationHistory);
	AssetBalance = createCollection(FABookBalance);

	AssetElements = createCollection(FAComponent);
	deprbookfilter = createSingle(DeprBookFilter);
	fasetup = createSingle(FASetup);

	AssetHistory = createCollection(FAHistory);
	BookSheetHistory = createCollection(FASheetHistory);

	ViewDocument: PXActionState;
	ViewBatch: PXActionState;
	bookfilter = createSingle(TranBookFilter);
	FATransactions = createCollection(FATran);
	LocationHistory = createCollection(FALocationHistory);

	ReduceUnreconCost: PXActionState;
	GLTrnFilter = createSingle(GLTranFilter);
	DsplAdditions = createCollection(DsplFAATran);

	DisposalOK: PXActionState;
	DispParams = createSingle(DisposeParams);

	SuspendParams = createSingle(SuspendParameters);

	RevDispInfo = createSingle(ReverseDisposalInfo);
}
