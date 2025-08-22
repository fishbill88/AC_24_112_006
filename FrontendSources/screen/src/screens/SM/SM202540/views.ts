import
{
	PXView,
	PXFieldState,
	gridConfig,
	headerDescription,
	ICurrencyInfo,
	disabled,
	selectorSettings,
	PXFieldOptions,
	linkCommand,
	columnConfig,
	GridColumnShowHideMode,
	GridColumnType,
	PXActionState,
	TextAlign,
	localizable
} from "client-controls";

// Views

export class BlobStorageConfig extends PXView  {
	Provider : PXFieldState<PXFieldOptions.CommitChanges>;
	AllowWrite : PXFieldState;
}

@gridConfig({
	allowInsert: false,
	allowDelete: false,
	allowImport: false,
	adjustPageSize: false
})
export class BlobProviderSettings extends PXView  {
	@columnConfig({ width: 200 })
	Name: PXFieldState;
	@columnConfig({ width: 400 })
	Value: PXFieldState;
}

export class BlobStorageMessage extends PXView  {
	Messages : PXFieldState<PXFieldOptions.NoLabel>;
}
