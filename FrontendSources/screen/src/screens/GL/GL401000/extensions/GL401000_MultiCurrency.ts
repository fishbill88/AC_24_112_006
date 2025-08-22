import { GL401000, GLHistoryEnqFilter, GLHistoryEnquiryResult } from '../GL401000';
import {
	PXFieldState,
	PXFieldOptions,
	GridColumnShowHideMode,
	columnConfig,
	featureInstalled,
	placeAfterProperty,
} from 'client-controls';

export interface GL401000_MultiCurrency extends GL401000 {}
@featureInstalled('PX.Objects.CS.FeaturesSet+Multicurrency')
export class GL401000_MultiCurrency  {
}

export interface GLHistoryEnqFilter_MultiCurrency extends GLHistoryEnqFilter { }
@featureInstalled('PX.Objects.CS.FeaturesSet+Multicurrency')
export class GLHistoryEnqFilter_MultiCurrency {
	ShowCuryDetail: PXFieldState<PXFieldOptions.CommitChanges>;
}

export interface GLHistoryEnquiryResult_MultiCurrency extends GLHistoryEnquiryResult { }
@featureInstalled('PX.Objects.CS.FeaturesSet+Multicurrency')
export class GLHistoryEnquiryResult_MultiCurrency {
	@placeAfterProperty("SignEndBalance") @columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	CuryID: PXFieldState;
	@placeAfterProperty("SignEndBalance") @columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	SignCuryBegBalance: PXFieldState;
	@placeAfterProperty("SignEndBalance") @columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	CuryPtdDebitTotal: PXFieldState;
	@placeAfterProperty("SignEndBalance") @columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	CuryPtdCreditTotal: PXFieldState;
	@placeAfterProperty("SignEndBalance") @columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	SignCuryEndBalance: PXFieldState;
	@placeAfterProperty("SignEndBalance") @columnConfig({ allowShowHide: GridColumnShowHideMode.Server })
	CuryPtdSaldo: PXFieldState;
}
