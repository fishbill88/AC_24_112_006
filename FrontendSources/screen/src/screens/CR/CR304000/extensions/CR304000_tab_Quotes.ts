import { CR304000 } from "../CR304000";
import {
	PXView,
	PXFieldState,
	PXFieldOptions,
	createCollection,
	gridConfig,
	viewInfo,
	PXActionState,
	columnConfig,
	linkCommand,
	TextAlign,
	GridColumnType,
	GridPreset,
} from "client-controls";

export interface CR304000_Quotes extends CR304000 {}
export class CR304000_Quotes {
	@viewInfo({ containerName: "Quotes" })
	Quotes = createCollection(CRQuote);
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	allowUpdate: false,
	adjustPageSize: true,
	topBarItems: {
		CreateQuote: {
			index: 2,
			config: {
				commandName: "CreateQuote",
				images: { normal: "main@RecordAdd" },
			},
		},
		CopyQuote: {
			index: 3,
			config: {
				commandName: "CopyQuote",
				text: "Copy Quote",
			},
		},
		PrimaryQuote: {
			index: 4,
			config: {
				commandName: "PrimaryQuote",
				text: "Set as Primary",
			},
		},
	},
})
export class CRQuote extends PXView {
	CreateQuote: PXActionState;
	CopyQuote: PXActionState;
	PrimaryQuote: PXActionState;

	@linkCommand("PrimaryQuote")
	@columnConfig({ allowNull: false, textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	IsPrimary: PXFieldState;
	@linkCommand("ViewQuote")
	QuoteNbr: PXFieldState;
	QuoteType: PXFieldState;
	Subject: PXFieldState;
	Status: PXFieldState;
	DocumentDate: PXFieldState;
	ExpirationDate: PXFieldState;
	@columnConfig({ hideViewLink: true })
	CuryID: PXFieldState;
	@columnConfig({ textAlign: TextAlign.Center, type: GridColumnType.CheckBox })
	ManualTotalEntry: PXFieldState;
	@columnConfig({ textAlign: TextAlign.Right })
	CuryAmount: PXFieldState;
	@columnConfig({ textAlign: TextAlign.Right })
	CuryDiscTot: PXFieldState;
	@columnConfig({ textAlign: TextAlign.Right })
	CuryTaxTotal: PXFieldState;
	@columnConfig({ textAlign: TextAlign.Right })
	CuryProductsAmount: PXFieldState;
	BAccountID: PXFieldState;
	LocationID: PXFieldState;
	ContactID: PXFieldState;
	@linkCommand("ViewProject")
	QuoteProjectID: PXFieldState;
	CuryCostTotal: PXFieldState;
	CuryGrossMarginAmount: PXFieldState;
	GrossMarginPct: PXFieldState;
}
