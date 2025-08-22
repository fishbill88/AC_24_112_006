/* ---------------------------------------------------------------------*
*                             Acumatica Inc.                            *

*              Copyright (c) 2005-2024 All rights reserved.             *

*                                                                       *

*                                                                       *

* This file and its contents are protected by United States and         *

* International copyright laws.  Unauthorized reproduction and/or       *

* distribution of all or any portion of the code contained herein       *

* is strictly prohibited and will result in severe civil and criminal   *

* penalties.  Any violations of this copyright will be prosecuted       *

* to the fullest extent possible under law.                             *

*                                                                       *

* UNDER NO CIRCUMSTANCES MAY THE SOURCE CODE BE USED IN WHOLE OR IN     *

* PART, AS THE BASIS FOR CREATING A PRODUCT THAT PROVIDES THE SAME, OR  *

* SUBSTANTIALLY THE SAME, FUNCTIONALITY AS ANY ACUMATICA PRODUCT.       *

*                                                                       *

* THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE.              *

* --------------------------------------------------------------------- */

using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.Common;
using PX.Objects.CS;
using PX.Objects.Extensions.MultiCurrency;
using PX.Objects.GL;
using PX.Objects.SO;
using PX.Objects.TX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CurrencyInfo = PX.Objects.CM.Extensions.CurrencyInfo;

namespace PX.Objects.AR
{
	public class SOOrderEntryVATRecognitionOnPrepayments : PXGraphExtension<SO.GraphExtensions.SOOrderEntryExt.CreatePaymentExt, SOOrderEntry>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.vATRecognitionOnPrepayments>();
		}

		[PXCopyPasteHiddenView]
		public PXFilter<SOQuickPrepaymentInvoice> QuickPrepaymentInvoice;

		[PXHidden]
		public SelectFrom<SOAdjust>
				.LeftJoin<ARRegister>
					.On<SOAdjust.adjgDocType.IsEqual<ARRegister.docType>
					.And<SOAdjust.adjgRefNbr.IsEqual<ARRegister.refNbr>>>
				.Where<SOAdjust.adjdOrderType.IsEqual<@P.AsString.ASCII>
					.And<SOAdjust.adjdOrderNbr.IsEqual<@P.AsString>>
					.And<ARRegister.docType.IsEqual<ARDocType.prepaymentInvoice>>
					.And<ARRegister.openDoc.IsEqual<True>>>
				.View OpenAdjustingPrepaymentInvoices;

		public PXAction<SOOrder> createPrepaymentInvoice;
		[PXUIField(DisplayName = SO.Messages.CreatePrepaymentInvoice, MapViewRights = PXCacheRights.Select, MapEnableRights = PXCacheRights.Update)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.AddNew, Tooltip = SO.Messages.CreatePrepaymentInvoice, DisplayOnMainToolbar = false, PopupCommand = nameof(refreshGraph))]
		public virtual IEnumerable CreatePrepaymentInvoice(PXAdapter adapter)
		{
			Base.Save.Press();
			SOOrder order = Base.Document.Current;

			if (order.Behavior == SOBehavior.BL)
			{
				throw new PXException(Messages.PrepaymentInvoicesAreNotSupportedForBlanketSalesOrders);
			}
			if (Base.Transactions.Any() == false)
			{
				throw new PXException(Messages.PrepaymentInvoiceWihtoutTransCannotBeCreated);
			}

			Base1.CheckTermsInstallmentType();

			string localizedHeader = PXMessages.LocalizeNoPrefix(SO.Messages.CreatePrepaymentInvoice);
			var dialogResult = QuickPrepaymentInvoice.View.AskExtWithHeader(localizedHeader, InitializeQuickPrepaymentInvoicePanel);
			if (dialogResult == WebDialogResult.OK)
			{
				if (QuickPrepaymentInvoice.Current.CuryPrepaymentAmt > order.CuryUnpaidBalance)
				{
					throw new PXException(Messages.PrepaymentAmountCannotBeGreaterThanUnpaidBalance, order.CuryUnpaidBalance);
				}
				if (QuickPrepaymentInvoice.Current.CuryPrepaymentAmt <= 0)
				{
					throw new PXException(Messages.PrepaymentAmountCannotBeZeroOrLess);
				}
				ARInvoiceEntry invoiceEntry = PXGraph.CreateInstance<ARInvoiceEntry>();
				bool requireControlTotal = invoiceEntry.ARSetup.Current.RequireControlTotal == true;
				invoiceEntry.ARSetup.Current.RequireControlTotal = false;
				ARInvoice invoice = invoiceEntry.Document.Insert(new ARInvoice() { DocType = ARDocType.PrepaymentInvoice });
				invoice = FillPrepaymentInvoice(invoiceEntry, invoice, order, QuickPrepaymentInvoice.Current);
				invoiceEntry.Document.Current = invoice;
				SOAdjust soadjust = CreateSOApplication(invoiceEntry, order);
				invoiceEntry.GetExtension<ARInvoiceEntryVATRecognitionOnPrepayments>().SOAdjustments.Current = soadjust;
				invoiceEntry.ARSetup.Current.RequireControlTotal = requireControlTotal;
				PXRedirectHelper.TryRedirect(invoiceEntry, PXRedirectHelper.WindowMode.NewWindow);
			}
			return adapter.Get();
		}

		public PXAction<SOOrder> refreshGraph;
		[PXUIField(MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXButton]
		public virtual IEnumerable RefreshGraph(PXAdapter adapter)
		{
			return Base.Cancel.Press(adapter);
		}

		public PXAction<SOOrder> createPrepaymentInvoiceOK;
		[PXUIField(DisplayName = "Create", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton(DisplayOnMainToolbar = false)]
		public virtual IEnumerable CreatePrepaymentInvoiceOK(PXAdapter adapter)
		{
			return adapter.Get();
		}

		#region SOOrder events
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[ARPaymentType.AdjgRefNbr(typeof(Search<ARPayment.refNbr,
			Where<ARPayment.customerID, In3<Current<SOOrder.customerID>, Current<Customer.consolidatingBAccountID>>,
				And<ARPayment.docType, Equal<Optional<SOAdjust.adjgDocType>>,
				And<ARPayment.openDoc, Equal<True>,
				And<Where<ARPayment.docType, NotEqual<ARDocType.prepaymentInvoice>,
						Or<ARPayment.released, Equal<True>,
						Or<Exists<Select<SOAdjust,
								Where<SOAdjust.adjgRefNbr, Equal<ARPayment.refNbr>,
									And<SOAdjust.adjgDocType, Equal<ARPayment.docType>>>>>>>>>>>>>), Filterable = true)]
		protected virtual void _(Events.CacheAttached<SOAdjust.adjgRefNbr> eventArgs) { }

		protected virtual void _(Events.RowSelected<SOOrder> eventArgs)
		{
			if (eventArgs.Row == null)
				return;
			SOOrderStateForPayments docState = Base1.GetDocumentState(eventArgs.Cache, eventArgs.Row);
			createPrepaymentInvoice.SetVisible(docState.PaymentsAllowed);
			createPrepaymentInvoice.SetEnabled(docState.CreatePaymentEnabled);
		}

		protected virtual void _(Events.RowDeleting<SOOrder> eventArgs)
		{
			SOOrder order = eventArgs.Row;
			if (order == null)
				return;

			if (OpenAdjustingPrepaymentInvoices.Select(order.OrderType, order.RefNbr).Any())
			{
				throw new PXException(Messages.CannotDeleteSalesOrderDueToLinkedPrepaymentInvoice, order.RefNbr);
			}
		}

		protected virtual void _(Events.FieldVerifying<SOOrder, SOOrder.cancelled> eventArgs)
		{
			SOOrder order = eventArgs.Row;
			if (order == null || (bool?)eventArgs.NewValue != true)
				return;

			if (OpenAdjustingPrepaymentInvoices.Select(order.OrderType, order.RefNbr).Any())
			{
				throw new PXException(Messages.CannotCancelSalesOrderDueToLinkedPrepaymentInvoice, order.RefNbr);
			}
		}
		#endregion

		#region SOOrder overrides

		[Obsolete(Common.Messages.MethodIsObsoleteAndWillBeRemoved2025R1)]
		public void CalculateApplicationBalance(CM.CurrencyInfo inv_info, ARPayment payment, SOAdjust adj)
		{
		}

		[PXOverride]
		public decimal GetPaymentBalance(CM.CurrencyInfo inv_info, ARPayment payment, SOAdjust adj,
				Func<CM.CurrencyInfo, ARPayment, SOAdjust, decimal> baseMethod)
		{
			if (payment.IsPrepaymentInvoiceDocument() && payment.PendingPayment == true)
			{
				// get PPI balance corrected by CRM
				ARTranPostGL paymentBalance = SelectFrom<ARTranPostGL>
					.Where<ARTranPostGL.docType.IsEqual<@P.AsString.ASCII>
						.And<ARTranPostGL.refNbr.IsEqual<@P.AsString>>
						.And<ARTranPostGL.accountID.IsEqual<@P.AsInt>>
						.And<ARTranPostGL.sourceDocType.IsIn<ARDocType.prepaymentInvoice, ARDocType.creditMemo>>>
					.Aggregate<To<
						GroupBy<ARTranPostGL.docType>,
						GroupBy<ARTranPostGL.refNbr>,
						Sum<ARTranPostGL.curyBalanceAmt>,
						Sum<ARTranPostGL.balanceAmt>>>
					.View.Select(Base, payment.DocType, payment.RefNbr, payment.ARAccountID);

				payment.CuryDocBal = paymentBalance?.CuryBalanceAmt ?? payment.CuryOrigDocAmt;
				payment.DocBal = paymentBalance?.BalanceAmt ?? payment.OrigDocAmt;
			}
			return baseMethod(inv_info, payment, adj);
		}

		public delegate decimal NewBalanceCalculationDelegate(SOAdjust adj, decimal newValue);
		[PXOverride]
		public decimal NewBalanceCalculation(SOAdjust adj, decimal newValue, NewBalanceCalculationDelegate baseMethod)
		{
			decimal newBalance = 0m;

			ARRegister register = ARRegister.PK.Find(Base, adj.AdjgDocType, adj.AdjgRefNbr);
			if (register != null && register.IsPrepaymentInvoiceDocument() && register.PendingPayment == true)
			{
				newBalance = (decimal)register.CuryOrigDocAmt - newValue;
			}
			else
			{
				newBalance = baseMethod.Invoke(adj, newValue);
			}

			return newBalance;
		}

		[Obsolete(Common.Messages.MethodIsObsoleteAndWillBeRemoved2025R1)]
		public void UpdateBalanceOnCuryAdjdAmtUpdated(PXCache sender, PXFieldUpdatedEventArgs e,
														Action<PXCache, PXFieldUpdatedEventArgs> baseMethod)
		{
			baseMethod(sender, e);
		}

		#endregion

		#region SOAdjust events
		protected virtual void _(Events.RowSelected<SOAdjust> e)
		{
			SOOrder order = Base.Document.Current;
			SOAdjust adj = e.Row;

			if (adj?.AdjgDocType == ARDocType.PrepaymentInvoice && order?.Behavior == SOBehavior.BL)
			{
				Base.Adjustments.Cache.RaiseExceptionHandling<SOAdjust.adjgDocType>
					(e.Row, adj.AdjgDocType, new PXSetPropertyException(Messages.PrepaymentInvoicesAreNotSupportedForBlanketSalesOrders));
			}
		}

		protected virtual void _(Events.RowPersisting<SOAdjust> e)
		{
			SOOrder order = Base.Document.Current;
			SOAdjust adj = e.Row;

			if (adj?.AdjgDocType == ARDocType.PrepaymentInvoice && order?.Behavior == SOBehavior.BL &&
				e.Cache.GetStatus(e.Row).IsNotIn(PXEntryStatus.Deleted, PXEntryStatus.InsertedDeleted))
			{
				throw new PXRowPersistingException(typeof(SOAdjust.adjgDocType).Name,
					adj.AdjgDocType, Messages.PrepaymentInvoicesAreNotSupportedForBlanketSalesOrders);
			}
		}
		#endregion

		#region SOQuickPrepaymentInvoice Panel

		protected virtual void _(Events.RowSelected<SOQuickPrepaymentInvoice> eventArgs)
		{
			SOQuickPrepaymentInvoice row = eventArgs.Row;
			SOOrder order = Base.Document.Current;

			if (order == null || row?.PrepaymentPct == null || row?.CuryPrepaymentAmt == null)
			{
				createPrepaymentInvoiceOK.SetEnabled(false);
				return;
			}

			bool inputValid = row.PrepaymentPct >= 0 && row.PrepaymentPct <= 100m
				&& row.CuryPrepaymentAmt > 0 && row.CuryPrepaymentAmt <= order.CuryUnpaidBalance;

			createPrepaymentInvoiceOK.SetEnabled(inputValid);

			if (row.CuryPrepaymentAmt <= 0)
			{
				eventArgs.Cache.RaiseExceptionHandling<SOQuickPrepaymentInvoice.curyPrepaymentAmt>(row, row.CuryPrepaymentAmt,
					new PXSetPropertyException(Messages.PrepaymentAmountCannotBeZeroOrLess, PXErrorLevel.Error));
			}
			else if (row.CuryPrepaymentAmt > order.CuryUnpaidBalance)
			{
				eventArgs.Cache.RaiseExceptionHandling<SOQuickPrepaymentInvoice.curyPrepaymentAmt>(row, row.CuryPrepaymentAmt,
					new PXSetPropertyException(Messages.PrepaymentAmountCannotBeGreaterThanUnpaidBalance, PXErrorLevel.Error, order.CuryUnpaidBalance));
			}
			else
			{
				eventArgs.Cache.RaiseExceptionHandling<SOQuickPrepaymentInvoice.curyPrepaymentAmt>(row, row.CuryPrepaymentAmt, null);
			}
		}

		protected virtual void _(Events.RowUpdating<SOQuickPrepaymentInvoice> eventArgs)
		{
			PXCache cache = eventArgs.Cache;
			SOQuickPrepaymentInvoice row = eventArgs.Row;
			SOQuickPrepaymentInvoice newRow = eventArgs.NewRow;

			if (row?.CuryPrepaymentAmt == null || newRow?.CuryPrepaymentAmt == null ||
				row?.PrepaymentPct == null || newRow?.PrepaymentPct == null)
				return;

			if (newRow.CuryOrigDocAmt != 0)
			{
				if (row.CuryPrepaymentAmt != newRow.CuryPrepaymentAmt)
				{
					cache.SetValueExt<SOQuickPrepaymentInvoice.prepaymentPct>(newRow,
						100m * (newRow.CuryPrepaymentAmt / newRow.CuryOrigDocAmt));
				}
				else if (row.PrepaymentPct != newRow.PrepaymentPct)
				{
					cache.SetValueExt<SOQuickPrepaymentInvoice.curyPrepaymentAmt>(newRow,
						newRow.CuryOrigDocAmt * (newRow.PrepaymentPct / 100m));
				}
			}

		}

		protected virtual void InitializeQuickPrepaymentInvoicePanel(PXGraph graph, string viewName)
		{
			SetDefaultValues(QuickPrepaymentInvoice.Current, Base.Document.Current);
			QuickPrepaymentInvoice.Cache.RaiseRowSelected(QuickPrepaymentInvoice.Current);
		}

		protected virtual void SetDefaultValues(SOQuickPrepaymentInvoice prepaymentInvoice, SOOrder order)
		{
			decimal? prepaymentPrc = order.CuryUnbilledOrderTotal == 0m || order.CuryUnpaidBalance == 0m || order.CuryOrderTotal == 0m
				? 0
				: order.CuryUnpaidBalance == order.CuryOrderTotal
					? order.PrepaymentReqPct != 0
						? order.PrepaymentReqPct
						: 100m
					: order.CuryUnpaidBalance * 100m / order.CuryUnbilledOrderTotal;

			decimal? prepaymentAmt = order.CuryUnbilledOrderTotal * prepaymentPrc / 100m;

			PXCache parametersCache = QuickPrepaymentInvoice.Cache;

			parametersCache.SetValueExt<SOQuickPrepaymentInvoice.curyID>(prepaymentInvoice, order.CuryID);
			parametersCache.SetValueExt<SOQuickPrepaymentInvoice.curyInfoID>(prepaymentInvoice, order.CuryInfoID);
			parametersCache.SetValueExt<SOQuickPrepaymentInvoice.curyOrigDocAmt>(prepaymentInvoice, order.CuryUnbilledOrderTotal);
			parametersCache.SetValueExt<SOQuickPrepaymentInvoice.curyPrepaymentAmt>(prepaymentInvoice, prepaymentAmt);
			parametersCache.SetValueExt<SOQuickPrepaymentInvoice.prepaymentPct>(prepaymentInvoice, prepaymentPrc);
		}

		#endregion

		#region Create Invoice

		protected virtual ARInvoice FillPrepaymentInvoice(ARInvoiceEntry invoiceEntry, ARInvoice newdoc, SOOrder soOrder, SOQuickPrepaymentInvoice prepaymentParams)
		{
			TaxCalc oldTaxCalc = TaxBaseAttribute.GetTaxCalc<ARTran.taxCategoryID>(invoiceEntry.Transactions.Cache, null);

			newdoc.BranchID = soOrder.BranchID;
			if (string.IsNullOrEmpty(soOrder.FinPeriodID) == false)
			{
				newdoc.FinPeriodID = soOrder.FinPeriodID;
			}
			newdoc.CustomerID = soOrder.CustomerID;
			newdoc.CuryID = soOrder.CuryID;
			newdoc.ProjectID = soOrder.ProjectID;
			newdoc.CustomerLocationID = soOrder.CustomerLocationID;
			newdoc.TaxZoneID = soOrder.TaxZoneID;
			newdoc.TaxCalcMode = soOrder.TaxCalcMode;
			newdoc.ExternalTaxExemptionNumber = soOrder.ExternalTaxExemptionNumber;
			newdoc.AvalaraCustomerUsageType = soOrder.AvalaraCustomerUsageType;
			newdoc.DocDesc = soOrder.OrderDesc;
			newdoc.InvoiceNbr = soOrder.CustomerOrderNbr;
			newdoc.DisableAutomaticTaxCalculation = soOrder.DisableAutomaticTaxCalculation;
			newdoc.DisableAutomaticDiscountCalculation = true;
			ARInvoiceVATRecognitionOnPrepayments newdocExt = Base.Caches[typeof(ARInvoice)].GetExtension<ARInvoiceVATRecognitionOnPrepayments>(newdoc);
			newdocExt.CuryPrepaymentAmt = prepaymentParams.CuryPrepaymentAmt;

			if (Base.customer.Current?.TermsID != null)
			{
				Terms terms = Terms.PK.Find(Base, Base.customer.Current.TermsID);
				if (terms != null && (terms.DiscPercent != 0 || terms.InstallmentType == TermsInstallmentType.Multiple))
					newdoc.TermsID = null;
			}

			CurrencyInfo info = invoiceEntry.currencyinfo.Select();
			CurrencyInfo orderCurrencyInfo = PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<SOOrder.curyInfoID>>>>.Select(Base);
			SOOrderType soOrderType = SOOrderType.PK.Find(invoiceEntry, soOrder.OrderType);
			bool curyRateNotDefined = (info.CuryRate ?? 0m) == 0m;
			if (curyRateNotDefined || soOrderType.UseCuryRateFromSO == true)
			{
				PXCache<CurrencyInfo>.RestoreCopy(info, orderCurrencyInfo);
				info.CuryInfoID = newdoc.CuryInfoID;
			}
			else
			{
				info.CuryRateTypeID = orderCurrencyInfo.CuryRateTypeID;
				invoiceEntry.currencyinfo.Update(info);
			}
			newdoc = invoiceEntry.Document.Update(newdoc);

			// Set TaxCalc to Manual to prevent tax recalculation
			TaxAttribute.SetTaxCalc<ARTran.taxCategoryID>(invoiceEntry.Transactions.Cache, null, TaxCalc.ManualCalc);

			ARTran tranMax = CreateDetails(invoiceEntry, soOrder, prepaymentParams);
			if (invoiceEntry.Transactions.Any() == false)
			{
				throw new PXException(Messages.PrepaymentInvoiceWihtoutTransCannotBeCreated);
			}
			CreateFreightDetail(invoiceEntry, newdoc, soOrder, prepaymentParams);
			CreateTaxes(invoiceEntry, newdoc);
			ProcessRoundingDiff(invoiceEntry, newdoc, prepaymentParams, tranMax, oldTaxCalc);

			TaxBaseAttribute.SetTaxCalc<ARTran.taxCategoryID>(invoiceEntry.Transactions.Cache, null, oldTaxCalc);

			newdoc.CuryOrigDocAmt = newdoc.CuryDocBal;
			newdoc.OrigDocAmt = newdoc.DocBal;

			return newdoc;
		}

		protected virtual ARTran CreateDetails(ARInvoiceEntry invoiceEntry, SOOrder soOrder, SOQuickPrepaymentInvoice prepaymentParams)
		{
			ARTran maxTran = null;

			foreach (SOLine orderline in Base.Transactions.Select())
			{
				if (orderline.UnbilledQty == 0 && orderline.CuryUnbilledAmt == 0)
					continue;

				ARTran newtran = new ARTran();
				newtran.LineType = orderline.LineType;
				newtran.BranchID = orderline.BranchID;
				newtran.AccountID = orderline.SalesAcctID;
				newtran.SubID = orderline.SalesSubID;
				newtran.InventoryID = orderline.InventoryID;
				newtran.SiteID = orderline.SiteID;
				newtran.ProjectID = orderline.ProjectID;
				newtran.TaskID = orderline.TaskID;
				newtran.UOM = orderline.UOM;
				newtran.Qty = orderline.UnbilledQty;
				newtran.BaseQty = orderline.BaseUnbilledQty;
				newtran.CuryUnitPrice = orderline.CuryUnitPrice;
				newtran.CuryTranAmt = orderline.CuryUnbilledAmt;

				decimal? curyExtPrice = 0m;
				if (orderline.UnbilledQty != 0)
				{
					curyExtPrice = orderline.UnbilledQty * orderline.CuryUnitPrice;
				}
				else if (orderline.CuryUnbilledAmt != 0)
				{
					curyExtPrice = orderline.CuryExtPrice;
				}
				newtran.CuryExtPrice = curyExtPrice;
				newtran.TranDesc = orderline.TranDesc;
				newtran.TaxCategoryID = orderline.TaxCategoryID;
				newtran.AvalaraCustomerUsageType = orderline.AvalaraCustomerUsageType;
				newtran.DiscPct = orderline.DiscPct;
				newtran.CuryDiscAmt = newtran.CuryExtPrice * newtran.DiscPct / 100m;
				newtran.IsFree = orderline.IsFree;
				newtran.ManualPrice = true;
				newtran.ManualDisc = true;
				newtran.FreezeManualDisc = true;

				newtran.DiscountID = orderline.DiscountID;
				newtran.DiscountSequenceID = orderline.DiscountSequenceID;

				newtran.DRTermStartDate = orderline.DRTermStartDate;
				newtran.DRTermEndDate = orderline.DRTermEndDate;
				newtran.CuryUnitPriceDR = orderline.CuryUnitPriceDR;
				newtran.DiscPctDR = orderline.DiscPctDR;
				newtran.DefScheduleID = orderline.DefScheduleID;
				newtran.SortOrder = orderline.SortOrder;
				newtran.OrigInvoiceType = orderline.InvoiceType;
				newtran.OrigInvoiceNbr = orderline.InvoiceNbr;
				newtran.OrigInvoiceLineNbr = orderline.InvoiceLineNbr;
				newtran.OrigInvoiceDate = orderline.InvoiceDate;
				newtran.CostCodeID = orderline.CostCodeID;

				newtran.BlanketType = orderline.BlanketType;
				newtran.BlanketNbr = orderline.BlanketNbr;
				newtran.BlanketLineNbr = orderline.BlanketLineNbr;
				newtran.BlanketSplitLineNbr = orderline.BlanketSplitLineNbr;

				newtran.SOOrderType = orderline.OrderType;
				newtran.SOOrderNbr = orderline.OrderNbr;
				newtran = invoiceEntry.Transactions.Insert(newtran);

				ARTranVATRecognitionOnPrepayments tranExt = Base.Caches[typeof(ARTran)].GetExtension<ARTranVATRecognitionOnPrepayments>(newtran);
				tranExt.PrepaymentPct = prepaymentParams.PrepaymentPct;
				tranExt.CuryPrepaymentAmt = invoiceEntry.FindImplementation<IPXCurrencyHelper>().RoundCury((decimal)orderline.CuryUnbilledAmt * (decimal)prepaymentParams.PrepaymentPct / 100);
				newtran.CuryTranAmt = invoiceEntry.FindImplementation<IPXCurrencyHelper>().RoundCury(
					(orderline.CuryUnbilledAmt.Value + GetPerUnitTaxAmt(orderline)) * (decimal)prepaymentParams.PrepaymentPct / 100);

				if (tranExt.CuryPrepaymentAmt != 0m)
				{
					decimal diff = Math.Abs(tranExt.CuryPrepaymentAmt.Value) - Math.Abs(newtran.CuryExtPrice.Value - newtran.CuryDiscAmt.Value);
					decimal diffAmt = diff * (tranExt.CuryPrepaymentAmt.Value / Math.Abs(tranExt.CuryPrepaymentAmt.Value));
					if (diff > 0)
					{
						DistributeDiffBetweenExtPriceAndDiscount(newtran, diffAmt);
					}
				}

				using (new DisableFormulaCalculationScope(invoiceEntry.Transactions.Cache,
					typeof(ARTranVATRecognitionOnPrepayments.prepaymentPct),
					typeof(ARTranVATRecognitionOnPrepayments.curyPrepaymentAmt),
					typeof(ARTran.curyTranAmt)))
				{
					newtran = invoiceEntry.Transactions.Update(newtran);
				}

				if (maxTran == null || Math.Abs(newtran.CuryTranAmt.Value) > Math.Abs(maxTran.CuryTranAmt.Value))
					maxTran = newtran;
			}
			return maxTran;
		}

		protected virtual decimal GetPerUnitTaxAmt(SOLine orderline)
		{
			if (orderline.OrderQty.Value == 0m)
				return 0m;
			else
				return GetPerUnitSOTaxDetails(orderline).Sum(_ => _.CuryTaxAmt.Value) * orderline.UnbilledQty.Value / orderline.OrderQty.Value;
		}

		protected virtual IEnumerable<SOTax> GetPerUnitSOTaxDetails(SOLine orderLine)
		{
			foreach (string taxID in Base.Taxes.Select()
				.Where(_ => _.GetItem<Tax>().TaxType == CSTaxType.PerUnit && _.GetItem<Tax>().TaxCalcLevel != CSTaxCalcLevel.Inclusive)
				.Select(_ => _.GetItem<SOTaxTran>().TaxID))
			{
				SOTax foundTax = Base.Tax_Rows.Select()
					.Select(_ => _.GetItem<SOTax>())
					.SingleOrDefault(_ => _.TaxID == taxID && _.LineNbr == orderLine.LineNbr);

				if (foundTax?.CuryTaxAmt != null)
					yield return foundTax;
			}
		}

		protected virtual void CreateFreightDetail(ARInvoiceEntry invoiceEntry, ARInvoice newdoc, SOOrder soOrder, SOQuickPrepaymentInvoice prepaymentParams)
		{
			if (soOrder.CuryUnbilledFreightTot > 0)
			{
				ARTran freightTran = new ARTran();
				freightTran.LineType = SOLineType.Freight;
				freightTran.SOOrderType = soOrder.OrderType;
				freightTran.SOOrderNbr = soOrder.OrderNbr;
				freightTran.BranchID = soOrder.BranchID;
				freightTran.CuryInfoID = soOrder.CuryInfoID;

				// There will be no postings on this freight in Prepayment Invoice,
				// so we just copy account from any existing transaction
				// to avoid unnecessary exceptions
				ARTran anyTran = invoiceEntry.Transactions.Select().First();
				freightTran.AccountID = anyTran?.AccountID;
				freightTran.SubID = anyTran?.SubID;

				freightTran.Qty = 1;
				freightTran.CuryUnitPrice = soOrder.CuryUnbilledFreightTot;
				freightTran.CuryExtPrice = soOrder.CuryUnbilledFreightTot;
				freightTran.ProjectID = soOrder.ProjectID;
				freightTran.Commissionable = false;
				if (PM.CostCodeAttribute.UseCostCode())
					freightTran.CostCodeID = PM.CostCodeAttribute.DefaultCostCode;
				using (new PXLocaleScope(Base.customer.Current.LocaleName))
				{
					if (String.IsNullOrEmpty(soOrder.ShipVia))
					{
						freightTran.TranDesc = PXMessages.LocalizeNoPrefix(SO.Messages.FreightDesc);
					}
					else
					{
						freightTran.TranDesc = PXMessages.LocalizeFormatNoPrefix(SO.Messages.FreightDescr, soOrder.ShipVia);
					}
				}

				freightTran = invoiceEntry.Transactions.Insert(freightTran);
				freightTran.TaxCategoryID = soOrder.FreightTaxCategoryID;

				ARTranVATRecognitionOnPrepayments tranExt = Base.Caches[typeof(ARTran)].GetExtension<ARTranVATRecognitionOnPrepayments>(freightTran);
				tranExt.PrepaymentPct = prepaymentParams.PrepaymentPct;
				freightTran = invoiceEntry.Transactions.Update(freightTran);

				newdoc.CuryFreightAmt = soOrder.CuryUnbilledFreightTot;
				newdoc.CuryFreightTot = soOrder.CuryUnbilledFreightTot;
				newdoc = invoiceEntry.Document.Update(newdoc);
			}
		}

		protected virtual void CreateTaxes(ARInvoiceEntry invoiceEntry, ARInvoice newdoc)
		{
			foreach (Tax appliedTax in Base.Taxes.Select().Select(_ => _.GetItem<Tax>()))
			{
				ARTaxTran newTaxTran = new ARTaxTran { Module = BatchModule.AR };
				invoiceEntry.Taxes.Cache.SetDefaultExt<ARTaxTran.origTranType>(newTaxTran);
				invoiceEntry.Taxes.Cache.SetDefaultExt<ARTaxTran.origRefNbr>(newTaxTran);
				invoiceEntry.Taxes.Cache.SetDefaultExt<ARTaxTran.lineRefNbr>(newTaxTran);
				newTaxTran.TranType = newdoc.DocType;
				newTaxTran.RefNbr = newdoc.RefNbr;
				newTaxTran.TaxID = appliedTax.TaxID;
				newTaxTran.TaxRate = 0m;
				newTaxTran.CuryID = newdoc.CuryID;

				if (appliedTax.TaxType == CSTaxType.PerUnit)
				{
					newTaxTran.TaxRate = -1; //hack. Disables undesireable recalculation of totals: TaxAttribute.VerifyTaxId sets _NoSumTotals = true, if TaxRate is not zero
					invoiceEntry.Taxes.Cache.RaiseRowInserted(newTaxTran); //We don't need ARTaxTran, but stil need ARTax record
				}
				else
				{
					newTaxTran = invoiceEntry.Taxes.Insert(newTaxTran);
				}
			}
		}

		protected void ProcessRoundingDiff(ARInvoiceEntry invoiceEntry, ARInvoice newdoc, SOQuickPrepaymentInvoice prepaymentParams, ARTran tranMax, TaxCalc oldTaxCalc)
		{
			//To avoid perfomance degradation for large documents (more 1000 rows) we put all rounding difference to the largest transaction.
			//Then recalculate taxes. If there will be still rounding difference, we put it into the tax amount.

			//1. calculate diff and put it into TaxableAmt
			decimal diff = (prepaymentParams.CuryPrepaymentAmt ?? 0m) - (newdoc.CuryDocBal ?? 0m);
			decimal diffTaxable = 0m;

			PXResult<ARTax, Tax> maxTaxAmt = null;

			if (diff != 0m)
			{
				//discrepancy limit = 0.01 x number of line(including freight) +0.01 x 2(maximum discount lines) + 0.01 x number of records in ARTax table related to exclusive taxes.
				CurrencyInfo currencyInfo = invoiceEntry.FindImplementation<IPXCurrencyHelper>().GetDefaultCurrencyInfo();
				decimal precisionLimit = 1;
				for (int i = 0; i < currencyInfo.CuryPrecision; i++)
				{
					precisionLimit = precisionLimit / 10;
				}

				int nonInclusiveTaxCount = SelectFrom<ARTax>
						.InnerJoin<Tax>
							.On<ARTax.taxID.IsEqual<Tax.taxID>>
						.Where<ARTax.tranType.IsEqual<ARInvoice.docType.FromCurrent>
							.And<ARTax.refNbr.IsEqual<ARInvoice.refNbr.FromCurrent>
							.And<Tax.taxCalcLevel.IsNotEqual<CSTaxCalcLevel.inclusive>>>>
						.View.Select(invoiceEntry)
						.AsEnumerable()
						.Cast<PXResult<ARTax, Tax>>()
						.Count();

				decimal? discrepancyLimit = precisionLimit * (invoiceEntry.Transactions.Cache.Inserted.Count() + 2 + nonInclusiveTaxCount);

				if (PXAccess.FeatureInstalled<FeaturesSet.netGrossEntryMode>()
					&& newdoc.TaxCalcMode == TaxCalculationMode.Gross)
				{
					if (Math.Abs(diff) > discrepancyLimit)
					{
						return;
					}
					diffTaxable = diff;
				}
				else
				{
					IEnumerable<PXResult<ARTax, Tax>> taxLines = SelectFrom<ARTax>
						.InnerJoin<Tax>
							.On<ARTax.taxID.IsEqual<Tax.taxID>>
						.Where<ARTax.tranType.IsEqual<ARInvoice.docType.FromCurrent>
							.And<ARTax.refNbr.IsEqual<ARInvoice.refNbr.FromCurrent>
							.And<ARTax.lineNbr.IsEqual<@P.AsInt>>>>
						.View.Select(invoiceEntry, tranMax.LineNbr)
						.AsEnumerable()
						.Cast<PXResult<ARTax, Tax>>();

					//We try to spread diff between TaxableAmt and TaxAmt for largest TaxAmt (except inclusive taxes)
					IEnumerable<PXResult<ARTax, Tax>> nonInclusiveTaxes = taxLines.Where(row => ((Tax)row).TaxCalcLevel != CSTaxCalcLevel.Inclusive);

					if (Math.Abs(diff) > discrepancyLimit)
					{
						return;
					}

					if (nonInclusiveTaxes.Any())
					{
						maxTaxAmt = nonInclusiveTaxes.OrderByDescending(row => Math.Abs(((ARTax)row).CuryTaxAmt ?? 0m)).First();
						diffTaxable = ProcessRoundingDiffToTaxable(invoiceEntry, taxLines, diff, maxTaxAmt);
					}
					else
					{
						diffTaxable = diff;
					}
				}
			}

			//2. put diff to the largest transaction
			// restore TaxCalc to recalculate taxes after adjusting tran amount
			TaxBaseAttribute.SetTaxCalc<ARTran.taxCategoryID>(invoiceEntry.Transactions.Cache, null, oldTaxCalc);

			tranMax.CuryTranAmt += diffTaxable;
			diffTaxable = DistributeDiffBetweenExtPriceAndDiscount(tranMax, diffTaxable);
			ARTranVATRecognitionOnPrepayments tranExt = invoiceEntry.Caches[typeof(ARTran)].GetExtension<ARTranVATRecognitionOnPrepayments>(tranMax);
			tranExt.CuryPrepaymentAmt += diffTaxable;
			using (new DisableFormulaCalculationScope(invoiceEntry.Transactions.Cache,
														typeof(ARTranVATRecognitionOnPrepayments.curyPrepaymentAmt),
														typeof(ARTranVATRecognitionOnPrepayments.prepaymentPct),
														typeof(ARTran.curyTranAmt)))
			{
				invoiceEntry.Transactions.Update(tranMax);
			}

			//3. calculate diff which appears after tax recalculation and put it into TaxAmt (without recalculation)
			diff = Math.Abs(prepaymentParams.CuryPrepaymentAmt ?? 0m) - (newdoc.CuryDocBal ?? 0m);
			if (diff != 0m)
			{
				TaxAttribute.SetTaxCalc<ARTran.taxCategoryID>(invoiceEntry.Transactions.Cache, null, TaxCalc.ManualCalc);
				if (maxTaxAmt.GetItem<Tax>().TaxType == CSTaxType.PerUnit)
				{
					//ARTaxTran does not exist
					tranMax.CuryTranAmt += diff;
					invoiceEntry.Transactions.Cache.Update(tranMax);
				}
				else //ARTaxTran does exist - distribute on it
					ProcessRoundingDiffToTax(invoiceEntry, diff, maxTaxAmt);
			}
		}

		protected virtual decimal DistributeDiffBetweenExtPriceAndDiscount(ARTran tranMax, decimal diffTaxable)
		{
			if (tranMax.CuryDiscAmt != 0m && diffTaxable < tranMax.CuryDiscAmt.Value)
			{
				tranMax.CuryDiscAmt -= diffTaxable;
			}
			else if (tranMax.CuryDiscAmt != 0m && diffTaxable >= tranMax.CuryDiscAmt.Value)
			{
				diffTaxable -= tranMax.CuryDiscAmt.Value;
				tranMax.CuryDiscAmt = 0m;
			}

			if (tranMax.CuryDiscAmt == 0m)
			{
				tranMax.CuryExtPrice += diffTaxable;
			}

			return diffTaxable;
		}

		protected virtual decimal ProcessRoundingDiffToTaxable(ARInvoiceEntry invoiceEntry, IEnumerable<PXResult<ARTax, Tax>> taxLines,
																decimal diff, ARTax maxTaxAmt)
		{
			decimal diffTaxable = diff;
			decimal diffTax = 0m;

			if (maxTaxAmt != null)
			{
				diffTaxable = invoiceEntry.FindImplementation<IPXCurrencyHelper>().RoundCury(
					(decimal)diff / (1 + taxLines.Where(row => ((Tax)row).TaxCalcLevel != CSTaxCalcLevel.Inclusive && row.GetItem<Tax>().TaxType != CSTaxType.PerUnit).Sum(row => ((ARTax)row).TaxRate.Value / 100)));
				diffTax = diff - diffTaxable;
			}
			else
			{
				diffTaxable = diff;
			}

			foreach (var groupByTax in taxLines.GroupBy(row => new { ((ARTax)row).TranType, ((ARTax)row).RefNbr, ((ARTax)row).TaxID }))
			{
				foreach (PXResult<ARTax, Tax> res in groupByTax)
				{
					Tax tax = (Tax)res;
					ARTax taxDetail = (ARTax)res;

					ARTax newTaxDetail = PXCache<ARTax>.CreateCopy(taxDetail);

					diffTax = invoiceEntry.FindImplementation<IPXCurrencyHelper>().RoundCury((decimal)diff * (decimal)taxDetail.TaxRate / 100);
					if (maxTaxAmt != null && newTaxDetail.TaxID == maxTaxAmt.TaxID)
					{
						newTaxDetail.CuryTaxableAmt += diffTaxable;
						newTaxDetail.CuryTaxAmt += diffTax;
					}
					else
					{
						newTaxDetail.CuryTaxableAmt += diffTaxable;
					}

					newTaxDetail = invoiceEntry.Tax_Rows.Update(newTaxDetail);
				}

				ARTaxTran taxSum = SelectFrom<ARTaxTran>
					.Where<ARTaxTran.tranType.IsEqual<@P.AsString.ASCII>
						.And<ARTaxTran.refNbr.IsEqual<@P.AsString>
						.And<ARTaxTran.taxID.IsEqual<@P.AsString>>>>
					.View.Select(invoiceEntry, groupByTax.Key.TranType, groupByTax.Key.RefNbr, groupByTax.Key.TaxID);
				if (taxSum != null)
				{
					ARTaxTran newTaxSum = PXCache<ARTaxTran>.CreateCopy(taxSum);
					if (maxTaxAmt != null && newTaxSum.TaxID == maxTaxAmt.TaxID)
					{
						newTaxSum.CuryTaxableAmt += diffTaxable;
						newTaxSum.CuryTaxAmt += diffTax;
					}
					else
					{
						newTaxSum.CuryTaxableAmt += diffTaxable;
					}
					newTaxSum = invoiceEntry.Taxes.Update(newTaxSum);
				}
			}
			return diffTaxable;
		}

		protected virtual void ProcessRoundingDiffToTax(ARInvoiceEntry invoiceEntry, decimal diff, ARTax maxTaxAmt)
		{
			if (maxTaxAmt == null) return;

			ARTax newTaxDetail = PXCache<ARTax>.CreateCopy(maxTaxAmt);
			newTaxDetail.CuryTaxAmt += diff;
			invoiceEntry.Tax_Rows.Update(newTaxDetail);

			ARTaxTran taxSum = SelectFrom<ARTaxTran>
				.Where<ARTaxTran.tranType.IsEqual<@P.AsString.ASCII>
					.And<ARTaxTran.refNbr.IsEqual<@P.AsString>
					.And<ARTaxTran.taxID.IsEqual<@P.AsString>>>>
				.View.Select(invoiceEntry, maxTaxAmt.TranType, maxTaxAmt.RefNbr, maxTaxAmt.TaxID);
			if (taxSum != null && taxSum.TaxID == maxTaxAmt.TaxID)
			{
				ARTaxTran newTaxSum = PXCache<ARTaxTran>.CreateCopy(taxSum);
				newTaxSum.CuryTaxAmt += diff;
				invoiceEntry.Taxes.Update(newTaxSum);
			}
		}

		protected virtual SOAdjust CreateSOApplication(ARInvoiceEntry invoiceEntry, SOOrder order)
		{
			ARInvoice invoice = invoiceEntry.Document.Current;
			SOAdjust adj = new SOAdjust();
			adj.AdjdOrderType = order.OrderType;
			adj.AdjdOrderNbr = order.OrderNbr;
			adj.AdjgRefNbr = invoice.RefNbr;
			adj.AdjgDocType = invoice.DocType;
			adj.CustomerID = Base.Document.Current.CustomerID;
			adj.AdjgCuryInfoID = Base.Document.Current.CuryInfoID;
			adj.AdjdCuryInfoID = order.CuryInfoID;

			SOOrder orderCopy = PXCache<SOOrder>.CreateCopy(order);
			adj.AdjdOrigCuryInfoID = orderCopy.CuryInfoID;
			adj.AdjgDocDate = invoice.DocDate;
			adj.AdjgCuryInfoID = Base.Document.Current.CuryInfoID;
			adj.AdjdCuryInfoID = order.CuryInfoID;
			adj.AdjdOrigCuryInfoID = orderCopy.CuryInfoID;
			adj.AdjdOrderDate = orderCopy.OrderDate;
			adj.AdjdOrderDate = orderCopy.OrderDate > invoice.DocDate
				? invoice.DocDate
				: orderCopy.OrderDate;
			adj.Released = false;

			adj.IsCCPayment = false;
			adj.PaymentReleased = false;
			adj.PendingPayment = false;
			adj.IsCCAuthorized = false;
			adj.IsCCCaptured = false;
			adj.Voided = false;
			adj.Hold = true;
			adj.PaymentMethodID = orderCopy.PaymentMethodID;
			adj.CashAccountID = orderCopy.CashAccountID;
			adj.PMInstanceID = orderCopy.PMInstanceID;

			adj = invoiceEntry.GetExtension<ARInvoiceEntryVATRecognitionOnPrepayments>().SOAdjustments.Insert(adj);

			return adj;
		}

		#endregion
	}
}
