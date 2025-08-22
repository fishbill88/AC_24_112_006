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

using PX.Data;
using PX.Objects.CS;
using PX.Objects.GL;
using System.Collections.Generic;
using PX.Objects.TX;
using PX.Objects.SO;
using PX.Objects.CM.Extensions;
using System;
using PX.Objects.Common;
using PX.Common;
using PX.Data.BQL.Fluent;
using PX.Objects.Common.Discount.Attributes;
using PX.Objects.Extensions.MultiCurrency;
using PX.Objects.Common.Attributes;
using PX.Data.BQL;

namespace PX.Objects.AR
{
	public class ARInvoiceEntryVATRecognitionOnPrepayments : PXGraphExtension<ARInvoiceEntry.MultiCurrency, ARInvoiceEntry>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.vATRecognitionOnPrepayments>();
		}
		public override void Initialize()
		{
			base.Initialize();
			Base.TaxesList.WhereAnd<Where<ARTaxTran.tranType, NotEqual<ARDocType.prepaymentInvoice>, Or<ARTaxTran.taxInvoiceNbr, IsNull>>>();
			SOAdjust soAdjust = SOAdjustments.Current;
		}

		[PXHidden]
		public SelectFrom<Standalone2.ARPayment>
				.Where<Standalone2.ARPayment.docType.IsEqual<ARRegister.docType.FromCurrent>
					.And<Standalone2.ARPayment.refNbr.IsEqual<ARRegister.refNbr.FromCurrent>>>
				.View ARPayment_DocType_RefNbr;

		[PXCopyPasteHiddenView]
		public PXSelectJoin<SOAdjust,
			InnerJoin<SOOrder,
				On<SOOrder.orderType, Equal<SOAdjust.adjdOrderType>,
				And<SOOrder.orderNbr, Equal<SOAdjust.adjdOrderNbr>>>>,
			Where<SOAdjust.adjgDocType, Equal<Current<ARInvoice.docType>>,
				And<SOAdjust.adjgRefNbr, Equal<Current<ARInvoice.refNbr>>>>> SOAdjustments;

		public PXSelect<SOOrder,
			Where<SOOrder.customerID, Equal<Required<SOOrder.customerID>>,
				And<SOOrder.orderType, Equal<Required<SOOrder.orderType>>,
				And<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>> SOOrder_CustomerID_OrderType_RefNbr;

		[PXHidden]
		public PXSelect<ARTran,
			Where<ARTran.tranType, Equal<Current<ARInvoice.docType>>,
				And<ARTran.refNbr, Equal<Current<ARInvoice.refNbr>>,
				And<Where<ARTran.sOOrderNbr, IsNotNull>>>>> LinesWithSOLinks;

		public SelectFrom<ARTax>
				.InnerJoin<Tax>
					.On<ARTax.taxID.IsEqual<Tax.taxID>>
				.Where<ARTax.tranType.IsEqual<ARInvoice.docType.FromCurrent>
					.And<ARTax.refNbr.IsEqual<ARInvoice.refNbr.FromCurrent>
					.And<Tax.taxCalcLevel.IsNotEqual<CSTaxCalcLevel.inclusive>>>>
				.View NonInclusiveTaxes;

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXRestrictor(typeof(Where<
			Current<ARRegister.docType>, NotEqual<ARDocType.prepaymentInvoice>,
			Or<Current<ARRegister.docType>, Equal<ARDocType.prepaymentInvoice>,
				And<Terms.installmentType, Equal<TermsInstallmentType.single>>>>), Messages.CannotSelectMultipleInstallmentCreditTermsForPrepaymentInvoice)]
		[PXRestrictor(typeof(Where<
			Current<ARRegister.docType>, NotEqual<ARDocType.prepaymentInvoice>,
			Or<Current<ARRegister.docType>, Equal<ARDocType.prepaymentInvoice>,
				And<Terms.discPercent, Equal<decimal0>>>>), Messages.CannotSelectCreditTermsWithDiscountForPrepaymentInvoice)]
		[PXRemoveBaseAttribute(typeof(PXFormulaAttribute))]
		[PXFormula(typeof(TermsByCustomer<ARInvoice.customerID, ARInvoice.termsID, ARInvoice.docType>))]
		protected virtual void ARInvoice_TermsID_CacheAttached(PXCache sender) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXRemoveBaseAttribute(typeof(AccountAttribute))]
		[Account(typeof(ARInvoice.branchID), typeof(Search<Account.accountID,
			Where2<Match<Current<AccessInfo.userName>>,
				And<Account.active, Equal<True>,
				And<Where<Current<GLSetup.ytdNetIncAccountID>, IsNull,
					Or<Account.accountID, NotEqual<Current<GLSetup.ytdNetIncAccountID>>>>>>>>),
			DisplayName = "Prepayment Account", ControlAccountForModule = ControlAccountModule.AR)]
		[PXUIRequired(typeof(Where<ARInvoice.docType, Equal<ARDocType.prepaymentInvoice>>))]
		protected virtual void ARInvoice_PrepaymentAccountID_CacheAttached(PXCache sender) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXUIRequired(typeof(Where<ARInvoice.docType, Equal<ARDocType.prepaymentInvoice>>))]
		protected virtual void ARInvoice_PrepaymentSubID_CacheAttached(PXCache sender) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXRemoveBaseAttribute(typeof(PXFormulaAttribute))]
		[PXFormula(typeof(
			Switch<
				Case<Where<ARTran.tranType, NotEqual<ARDocType.prepaymentInvoice>>,
					Sub<ARTran.curyExtPrice, Add<ARTran.curyDiscAmt, ARTran.curyRetainageAmt>>>,
				ARTranVATRecognitionOnPrepayments.curyPrepaymentAmt>))]
		[PXFormula(null, typeof(CountCalc<ARSalesPerTran.refCntr>))]
		protected virtual void _(Events.CacheAttached<ARTran.curyTranAmt> e) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXRemoveBaseAttribute(typeof(ManualDiscountMode))]
		[ManualDiscountModeVATRecognitionOnPrepayments(typeof(ARTran.curyDiscAmt), typeof(ARTran.curyTranAmt),
				typeof(ARTran.discPct), typeof(ARTran.freezeManualDisc), DiscountFeatureType.CustomerDiscount)]
		protected virtual void _(Events.CacheAttached<ARTran.manualDisc> e) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXRemoveBaseAttribute(typeof(DenormalizedFromAttribute))]
		[DenormalizedFrom(
			new[] { typeof(ARRegister.voided), typeof(ARRegister.hold), typeof(ARRegister.docDesc), typeof(ARRegister.pendingPayment)},
			new[] { typeof(SOAdjust.voided), typeof(SOAdjust.hold), typeof(SOAdjust.docDesc), typeof(SOAdjust.pendingPayment)})]
		[DenormalizedFrom(
			new[] { typeof(ARInvoice.paymentMethodID), typeof(ARInvoice.cashAccountID) },
			new[] { typeof(SOAdjust.paymentMethodID), typeof(SOAdjust.cashAccountID) })]
		protected virtual void _(Events.CacheAttached<SOAdjust.isCCPayment> e) { }

		/// <summary>
		/// Overrides <see cref="ARInvoiceEntry.MultiCurrency.AllowOverrideCury()"/>
		/// </summary>
		[PXOverride]
		public virtual bool AllowOverrideCury(Func<bool> baseAllowOverrideCury)
		{
			ARInvoice doc = Base.Document.Current as ARInvoice;

			if (doc != null && (doc.IsPrepaymentInvoiceDocumentReverse()
					|| doc.IsPrepaymentInvoiceDocument() && LinesWithSOLinks.SelectSingle() != null))
				return false;

			return baseAllowOverrideCury();
		}

		protected virtual void _(Events.FieldDefaulting<ARInvoice, ARInvoice.prepaymentAccountID> e)
		{
			if (e.Row != null && Base.customer.Current != null)
			{
				e.NewValue = Base.GetAcctSub<Customer.prepaymentAcctID>(Base.customer.Cache, Base.customer.Current);
			}
		}

		protected virtual void _(Events.FieldDefaulting<ARInvoice, ARInvoice.prepaymentSubID> e)
		{
			if (e.Row != null && Base.customer.Current != null)
			{
				e.NewValue = Base.GetAcctSub<Customer.prepaymentSubID>(Base.customer.Cache, Base.customer.Current);
			}
		}
		public virtual void ARInvoice_CustomerLocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<ARInvoice.prepaymentAccountID>(e.Row);
			sender.SetDefaultExt<ARInvoice.prepaymentSubID>(e.Row);
		}

		protected virtual void _(Events.RowInserted<ARTran> e)
		{
			if (!(e.Row is ARTran arTran))
				return;

			if (arTran.TranType == ARDocType.PrepaymentInvoice)
				arTran.Commissionable = false;
		}

		protected virtual void ARSalesPerTran_RowInserting(PXCache sender, PXRowInsertingEventArgs e, PXRowInserting baseDelegate)
		{
			ARSalesPerTran row = (ARSalesPerTran)e.Row;
			if (row.DocType == ARDocType.PrepaymentInvoice)
				e.Cancel = true;
			else
			{
				baseDelegate(sender, e);
			}
		}

		public virtual void ARTaxTran_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if (Base.Document.Current != null && (e.Operation == PXDBOperation.Insert || e.Operation == PXDBOperation.Update))
			{
				if (Base.Document.Current.IsPrepaymentInvoiceDocument())
				{
					ARTaxTran taxTran = (ARTaxTran)e.Row;
					taxTran.TaxType = TaxType.PendingSales;
					taxTran.CuryAdjustedTaxableAmt = taxTran.CuryTaxableAmt;
					taxTran.AdjustedTaxableAmt = taxTran.TaxableAmt;
					taxTran.CuryAdjustedTaxAmt = taxTran.CuryTaxAmt;
					taxTran.AdjustedTaxAmt = taxTran.TaxAmt;
				}
			}
		}

		public virtual void ARInvoice_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			ARInvoice doc = e.Row as ARInvoice;
			if (doc == null)
				return;

			ARInvoiceState state = Base.GetDocumentState(cache, doc);

			bool substituteBalanceWithUnpaidBalance = state.IsDocumentPrepaymentInvoice
				&& doc.Status != ARDocStatus.Unapplied
				&& doc.Status != ARDocStatus.Closed
				&& doc.Status != ARDocStatus.Voided
				&& doc.Status != ARDocStatus.Reserved;

			PXUIFieldAttribute.SetEnabled<ARInvoice.curyDocUnpaidBal>(cache, doc, false);
			PXUIFieldAttribute.SetVisible<ARInvoice.curyDocBal>(cache, doc, !state.IsUnreleasedMigratedDocument && !substituteBalanceWithUnpaidBalance);
			PXUIFieldAttribute.SetVisible<ARInvoice.curyDocUnpaidBal>(cache, doc, !state.IsUnreleasedMigratedDocument && substituteBalanceWithUnpaidBalance);
			PXUIFieldAttribute.SetVisible<ARTaxTran.curyAdjustedTaxableAmt>(Base.Taxes.Cache, null, state.IsDocumentPrepaymentInvoice);
			PXUIFieldAttribute.SetVisible<ARTaxTran.curyAdjustedTaxAmt>(Base.Taxes.Cache, null, state.IsDocumentPrepaymentInvoice);
			PXUIFieldAttribute.SetVisible<ARInvoice.prepaymentAccountID>(cache, null, state.IsDocumentPrepaymentInvoice);
			PXUIFieldAttribute.SetVisible<ARInvoice.prepaymentSubID>(cache, null, state.IsDocumentPrepaymentInvoice);
			PXUIFieldAttribute.SetEnabled<ARInvoice.prepaymentAccountID>(cache, null, state.IsDocumentPrepaymentInvoice && !state.ShouldDisableHeader);
			PXUIFieldAttribute.SetEnabled<ARInvoice.prepaymentSubID>(cache, null, state.IsDocumentPrepaymentInvoice && !state.ShouldDisableHeader);

			PXUIFieldAttribute.SetDisplayName<ARTaxTran.curyTaxableAmt>(Base.Taxes.Cache, state.IsDocumentPrepaymentInvoice ? Messages.OrigTaxableAmount : "Taxable Amount");
			PXUIFieldAttribute.SetDisplayName<ARTaxTran.curyTaxAmt>(Base.Taxes.Cache, state.IsDocumentPrepaymentInvoice ? Messages.OrigTaxAmount : "Tax Amount");

			PXUIFieldAttribute.SetVisible<ARTran.sOOrderNbr>(Base.Transactions.Cache, null, state.IsDocumentPrepaymentInvoice);
			PXUIFieldAttribute.SetVisible<ARTran.sOOrderType>(Base.Transactions.Cache, null, state.IsDocumentPrepaymentInvoice);
			PXUIFieldAttribute.SetEnabled<ARTran.sOOrderNbr>(Base.Transactions.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<ARTran.sOOrderType>(Base.Transactions.Cache, null, false);

			PXUIFieldAttribute.SetVisible<ARTranVATRecognitionOnPrepayments.prepaymentPct>(Base.Transactions.Cache, null, state.IsDocumentPrepaymentInvoice);
			PXUIFieldAttribute.SetVisible<ARTranVATRecognitionOnPrepayments.curyPrepaymentAmt>(Base.Transactions.Cache, null, state.IsDocumentPrepaymentInvoice);
			PXUIFieldAttribute.SetEnabled<ARTranVATRecognitionOnPrepayments.prepaymentPct>(Base.Transactions.Cache, null, state.IsDocumentPrepaymentInvoice);
			PXUIFieldAttribute.SetEnabled<ARTranVATRecognitionOnPrepayments.curyPrepaymentAmt>(Base.Transactions.Cache, null, state.IsDocumentPrepaymentInvoice);

			if (LinesWithSOLinks.Any() || SOAdjustments.Any())
			{
				PXUIFieldAttribute.SetEnabled<ARInvoice.customerID>(cache, doc, false);
			}

			ARInvoiceVATRecognitionOnPrepayments docExt = Base.Caches[typeof(ARInvoice)].GetExtension<ARInvoiceVATRecognitionOnPrepayments>(doc);
			if (docExt.CuryPrepaymentAmt != null && doc.CuryDocBal != 0m)
			{
				decimal diff = (docExt.CuryPrepaymentAmt ?? 0m) - (doc.CuryDocBal ?? 0m);
				if (diff != 0)
				{
					//discrepancy limit = 0.01 x number of line(including freight) +0.01 x 2(maximum discount lines) + 0.01 x number of records in ARTax table related to exclusive taxes.
					CurrencyInfo currencyInfo = Base.FindImplementation<IPXCurrencyHelper>().GetDefaultCurrencyInfo();
					decimal precisionLimit = 1;
					for (int i = 0; i < currencyInfo.CuryPrecision; i++)
					{
						precisionLimit = precisionLimit / 10;
					}
					decimal? discrepancyLimit = precisionLimit * (Base.Transactions.Select().Count() + 2);

					if (PXAccess.FeatureInstalled<FeaturesSet.netGrossEntryMode>()
						&& doc.TaxCalcMode == TaxCalculationMode.Gross)
					{
						if (Math.Abs(diff) > discrepancyLimit)
						{
							Base.Document.Cache.RaiseExceptionHandling<ARRegister.curyDocUnpaidBal>(Base.Document.Current, Base.Document.Current.CuryDocUnpaidBal,
								new PXSetPropertyException(Base.Document.Current, Messages.PrepaymentAmountCannotBeAdjusted, PXErrorLevel.Warning, doc.CuryDocBal, docExt.CuryPrepaymentAmt));
						}
					}
					else
					{
						discrepancyLimit += precisionLimit * NonInclusiveTaxes.Select().Count();
						if (Math.Abs(diff) > discrepancyLimit)
						{
							Base.Document.Cache.RaiseExceptionHandling<ARInvoice.curyDocUnpaidBal>(Base.Document.Current, Base.Document.Current.CuryDocUnpaidBal,
								new PXSetPropertyException(Base.Document.Current, Messages.PrepaymentAmountCannotBeAdjusted, PXErrorLevel.Warning, doc.CuryDocBal, docExt.CuryPrepaymentAmt));
						}
					}
				}
			}
		}

		private string CheckPrepaymentAmount(ARTran tran, ARTranVATRecognitionOnPrepayments tranExt)
		{
			decimal? priceLessDiscount = tran.CuryExtPrice - tran.CuryDiscAmt;

			if ((tranExt.CuryPrepaymentAmt > 0 && priceLessDiscount < 0)
				|| (tranExt.CuryPrepaymentAmt < 0 && priceLessDiscount > 0))
			{
				return Messages.PrepaymentAmountCannotHaveDifferentSignThanLineUnpaidBalance;
			}
			else if (Math.Abs(tranExt.CuryPrepaymentAmt.Value) > Math.Abs(priceLessDiscount.Value))
			{
				return Messages.PrepaymentAmountCannotBeGreaterThanLineUnpaidBalanceInAbsoluteValues;
			}

			return null;
		}

		protected virtual void _(Events.RowSelected<ARTran> e)
		{
			ARTran tran = e.Row;
			if (tran?.CuryExtPrice == null || tran?.CuryDiscAmt == null
				|| tran?.TranType != ARDocType.PrepaymentInvoice)
				return;

			ARTranVATRecognitionOnPrepayments tranExt = e.Cache.GetExtension<ARTranVATRecognitionOnPrepayments>(tran);
			if (tranExt?.CuryPrepaymentAmt == null)
				return;

			string errorMessage = CheckPrepaymentAmount(tran, tranExt);

			PXSetPropertyException ex = errorMessage != null
				? new PXSetPropertyException(errorMessage)
				: null;

			Base.Transactions.Cache.RaiseExceptionHandling<ARTranVATRecognitionOnPrepayments.curyPrepaymentAmt>
				(e.Row, tranExt.CuryPrepaymentAmt, ex);
		}


		protected virtual void _(Events.RowUpdated<ARTran> e)
		{
			if (e.Row?.TranType != ARDocType.PrepaymentInvoice) return;
			else TaxAttribute.Calculate<ARTran.taxCategoryID>(e.Cache, new PXRowUpdatedEventArgs(e.Row, e.OldRow, e.ExternalCall));
		}

		protected virtual void _(Events.RowPersisting<ARTran> e)
		{
			ARTran tran = e.Row;
			if (tran?.CuryExtPrice == null || tran?.CuryDiscAmt == null
				|| tran?.TranType != ARDocType.PrepaymentInvoice)
				return;

			ARTranVATRecognitionOnPrepayments tranExt = e.Cache.GetExtension<ARTranVATRecognitionOnPrepayments>(tran);
			if (tranExt?.CuryPrepaymentAmt == null)
				return;

			string errorMessage = CheckPrepaymentAmount(tran, tranExt);

			if (errorMessage != null)
			{
				throw new PXRowPersistingException(typeof(ARTranVATRecognitionOnPrepayments.curyPrepaymentAmt).Name,
					tranExt.CuryPrepaymentAmt, errorMessage);
			}
		}

		protected virtual void _(Events.FieldUpdated<ARInvoice, ARInvoice.taxZoneID> e)
		{
			if (e.Row?.DocType != ARDocType.PrepaymentInvoice) return;
			foreach (ARTran aRTran in Base.Transactions.Cache.Updated)
			{
				TaxAttribute.Calculate<ARTran.taxCategoryID>(Base.Transactions.Cache, new PXRowUpdatedEventArgs(aRTran, aRTran, e.ExternalCall));
			}
		}

		protected virtual void _(Events.RowPersisting<ARInvoice> e)
		{
			ARInvoice doc = e.Row as ARInvoice;
			if (doc == null) return;

			bool isMigrationMode = Base.ARSetup.Current?.MigrationMode == true;
			if (doc.DocType == ARDocType.PrepaymentInvoice && isMigrationMode)
			{
				throw new PXException(Messages.PrepaymentInvoiceIsNotSupportedForMigrationMode);
			}

			if (doc.DocType == ARDocType.PrepaymentInvoice && doc.RetainageApply == true)
			{
				throw new PXException(Messages.PrepaymentInvoiceCannotHaveRetainage);
			}

			if (e.Operation.Command() == PXDBOperation.Delete)
			{
				// Acuminator disable once PX1043 SavingChangesInEventHandlers[Justification]
				PXDatabase.Delete<Standalone2.ARPayment>(
					new PXDataFieldRestrict<Standalone2.ARPayment.docType>(PXDbType.Char, 3, doc.DocType, PXComp.EQ),
					new PXDataFieldRestrict<Standalone2.ARPayment.refNbr>(PXDbType.VarChar, 15, doc.RefNbr, PXComp.EQ));
			}
		}

		public virtual void _(Events.RowDeleting<ARTax> e)
		{
			if (Base.Document.Current.IsPrepaymentInvoiceDocument() && e.Row.TaxUOM != null)  //Per-unit tax
			{
				ARTran linkedARTran = Base.Transactions.Locate(new ARTran
				{
					TranType = e.Row.TranType,
					RefNbr = e.Row.RefNbr,
					LineNbr = e.Row.LineNbr
				});

				Base.Transactions.Cache.SetValueExt<ARTran.curyTranAmt>(linkedARTran, linkedARTran.CuryTranAmt - e.Row.CuryTaxAmt);
			}
		}

		[PXOverride]
		public void Persist(Action basePersist)
		{
			foreach (ARInvoice ardoc in Base.Document.Cache.Cached)
			{
				PXEntryStatus status = Base.Document.Cache.GetStatus(ardoc);
				if (ardoc.IsPrepaymentInvoiceDocument() && status == PXEntryStatus.Deleted)
				{
					// clear cache because the ARPayment part is deleted on RowPersisting<ARInvoice>
					Base.Caches<ARPayment>().Clear();
				}
			}
			basePersist();
		}

		protected virtual void _(Events.RowPersisted<ARInvoice> e, PXRowPersisted baseMethod)
		{
			ARInvoice doc = (ARInvoice)e.Row;
			ARRegister ardoc = (ARRegister)e.Row;

			if (doc.IsPrepaymentInvoiceDocument() && e.Operation.Command() != PXDBOperation.Delete && e.TranStatus != PXTranStatus.Aborted)
			{
				Standalone2.ARPayment prepayment = new Standalone2.ARPayment();
				prepayment.DocType = doc.DocType;
				prepayment.RefNbr = doc.RefNbr;

				prepayment.CashAccountID = doc.CashAccountID;
				prepayment.PaymentMethodID = doc.PaymentMethodID;

				prepayment.AdjDate = ardoc.DocDate;
				prepayment.AdjFinPeriodID = ardoc.FinPeriodID;
				prepayment.AdjTranPeriodID = ardoc.TranPeriodID;

				prepayment.CuryOrigTaxDiscAmt = 0m;
				prepayment.OrigTaxDiscAmt = 0m;

				ARPayment_DocType_RefNbr.Cache.SetDefaultExt<ARPayment.projectID>(prepayment);
				ARPayment_DocType_RefNbr.Cache.SetDefaultExt<ARPayment.taskID>(prepayment);
				ARPayment_DocType_RefNbr.Cache.SetDefaultExt<ARPayment.chargeCntr>(prepayment);
				ARPayment_DocType_RefNbr.Cache.SetDefaultExt<ARPayment.isCCPayment>(prepayment);
				ARPayment_DocType_RefNbr.Cache.SetDefaultExt<ARPayment.isCCAuthorized>(prepayment);
				ARPayment_DocType_RefNbr.Cache.SetDefaultExt<ARPayment.isCCCaptured>(prepayment);
				ARPayment_DocType_RefNbr.Cache.SetDefaultExt<ARPayment.isCCCaptureFailed>(prepayment);
				ARPayment_DocType_RefNbr.Cache.SetDefaultExt<ARPayment.isCCRefunded>(prepayment);
				ARPayment_DocType_RefNbr.Cache.SetDefaultExt<ARPayment.isCCUserAttention>(prepayment);
				ARPayment_DocType_RefNbr.Cache.SetDefaultExt<ARPayment.cCActualExternalTransactionID>(prepayment);
				ARPayment_DocType_RefNbr.Cache.SetDefaultExt<ARPayment.cCPaymentStateDescr>(prepayment);
				ARPayment_DocType_RefNbr.Cache.SetDefaultExt<ARPayment.cCReauthTriesLeft>(prepayment);
				ARPayment_DocType_RefNbr.Cache.SetDefaultExt<ARPayment.cCTransactionRefund>(prepayment);
				ARPayment_DocType_RefNbr.Cache.SetDefaultExt<ARPayment.consolidateChargeTotal>(prepayment);
				ARPayment_DocType_RefNbr.Cache.SetDefaultExt<ARPayment.curyConsolidateChargeTotal>(prepayment);
				ARPayment_DocType_RefNbr.Cache.SetDefaultExt<ARPayment.cleared>(prepayment);
				ARPayment_DocType_RefNbr.Cache.SetDefaultExt<ARPayment.saveCard>(prepayment);
				ARPayment_DocType_RefNbr.Cache.SetDefaultExt<ARPayment.depositAsBatch>(prepayment);
				ARPayment_DocType_RefNbr.Cache.SetDefaultExt<ARPayment.deposited>(prepayment);
				ARPayment_DocType_RefNbr.Cache.SetDefaultExt<ARPayment.settled>(prepayment);

				ARPayment_DocType_RefNbr.Cache.Update(prepayment);

				SOAdjustments.Cache.SetValueExt<SOAdjust.curyOrigDocAmt>(SOAdjustments.Current, doc.CuryOrigDocAmt);
			}
			baseMethod.Invoke(e.Cache, e.Args);
		}

		protected virtual void _(Events.FieldUpdated<ARInvoice, ARInvoice.curyDocBal> e)
		{
			if (e.Row == null || e.NewValue == e.OldValue) return;

			foreach (PXResult<SOAdjust, SOOrder> item in SOAdjustments.Select())
			{
				SOAdjust adjust = item;
				SOOrder order = item;

				decimal originalCuryAdjdAmt = (decimal?)SOAdjustments.Cache.GetValueOriginal<SOAdjust.curyAdjdAmt>(adjust) ?? 0m;

				if (adjust.AdjdCuryInfoID != null && e.Cache.GetStatus(e.Row) != PXEntryStatus.Deleted)
				{
					if ((decimal?)e.NewValue < (originalCuryAdjdAmt + order.CuryUnpaidBalance))
					{
						adjust.CuryAdjdAmt = (decimal?)e.NewValue;
					}
					else
					{
						adjust.CuryAdjdAmt = originalCuryAdjdAmt + order.CuryUnpaidBalance;
					}
					adjust.CuryOrigDocAmt = (decimal?)e.NewValue;
					CalcBalances(adjust, false, false);
					SOAdjustments.Cache.Update(adjust);
				}
			}
		}

		public void _(Events.FieldDefaulting<ARTran.taxCategoryID> e)
		{
			if (e.Row != null && string.IsNullOrEmpty(((ARTran)e.Row).SOOrderNbr) == false)
			{
				//tax category is taken from invoice lines
				e.NewValue = null;
				e.Cancel = true;
			}
		}

		public void _(Events.FieldDefaulting<ARTaxTran.accountID> e)
		{
			ARTaxTran taxTran = (ARTaxTran)e.Row;
			if (taxTran == null)
				return;

				if (taxTran.TranType == ARDocType.PrepaymentInvoice)
				{
					SalesTax salesTax = SelectFrom<SalesTax>
						.Where<SalesTax.taxID.IsEqual<@P.AsString>>
						.View.Select(Base, taxTran.TaxID);
				e.NewValue = salesTax?.OnARPrepaymentTaxAcctID;
			}
		}

		public void _(Events.FieldDefaulting<ARTaxTran.subID> e)
		{
			ARTaxTran taxTran = (ARTaxTran)e.Row;
			if (taxTran == null)
				return;

			if (taxTran.TranType == ARDocType.PrepaymentInvoice)
			{
				SalesTax salesTax = SelectFrom<SalesTax>
					.Where<SalesTax.taxID.IsEqual<@P.AsString>>
					.View.Select(Base, taxTran.TaxID);
				e.NewValue = salesTax?.OnARPrepaymentTaxSubID;
			}
		}

		public void CalcBalances(SOAdjust adj, SOOrder invoice, bool isCalcRGOL, bool DiscOnDiscDate)
		{
			ARInvoiceEntry.MultiCurrency Base1 = Base.GetExtension<ARInvoiceEntry.MultiCurrency>();
			new PaymentBalanceCalculator(Base1).CalcBalances(adj.AdjgCuryInfoID, adj.AdjdCuryInfoID, invoice, adj);

			if (DiscOnDiscDate)
			{
				CM.PaymentEntry.CalcDiscount(adj.AdjgDocDate, invoice, adj);
			}
			//CM.PaymentEntry.WarnDiscount(Base, adj.AdjgDocDate, invoice, adj);

			new PaymentBalanceAjuster(Base1).AdjustBalance(adj);

			if (isCalcRGOL && (adj.Voided != true))
			{
				new PaymentRGOLCalculator(Base1, adj, adj.ReverseGainLoss).Calculate(invoice);
			}
		}

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXRemoveBaseAttribute(typeof(CM.PXCurrencyAttribute))]
		[PXCurrency(typeof(SOOrder.curyInfoID), typeof(SOOrder.docBal))]
		protected virtual void SOOrder_CuryDocBal_CacheAttached(PXCache sender) { }

		public void CalcBalances(SOAdjust adj, bool isCalcRGOL, bool DiscOnDiscDate)
		{
			if (PXTransactionScope.IsConnectionOutOfScope) return;

			foreach (PXResult<SOOrder> res in SOOrder_CustomerID_OrderType_RefNbr.Select(adj.CustomerID, adj.AdjdOrderType, adj.AdjdOrderNbr))
			{
				SOOrder invoice = PXCache<SOOrder>.CreateCopy(res);

				//Base.internalCall = true;

				SOAdjust other = PXSelectGroupBy<SOAdjust,
				Where<SOAdjust.voided, Equal<False>,
				And<SOAdjust.adjdOrderType, Equal<Required<SOAdjust.adjdOrderType>>,
				And<SOAdjust.adjdOrderNbr, Equal<Required<SOAdjust.adjdOrderNbr>>,
				And<Where<SOAdjust.adjgDocType, NotEqual<Required<SOAdjust.adjgDocType>>, Or<SOAdjust.adjgRefNbr, NotEqual<Required<SOAdjust.adjgRefNbr>>>>>>>>,
				Aggregate<GroupBy<SOAdjust.adjdOrderType,
				GroupBy<SOAdjust.adjdOrderNbr, Sum<SOAdjust.curyAdjdAmt, Sum<SOAdjust.adjAmt>>>>>>.Select(Base, adj.AdjdOrderType, adj.AdjdOrderNbr, adj.AdjgDocType, adj.AdjgRefNbr);

				if (other != null && other.AdjdOrderNbr != null)
				{
					invoice.CuryDocBal -= other.CuryAdjdAmt;
					invoice.DocBal -= other.AdjAmt;
				}
				//Base.internalCall = false;

				CalcBalances(adj, invoice, isCalcRGOL, DiscOnDiscDate);

				//if (IsApplicationToBlanketOrderWithChild(adj))
				//{
				//	adj.CuryDocBal = 0m;
				//	adj.DocBal = 0m;
				//}
			}
		}

		protected virtual void _(Events.FieldUpdated<SOAdjust, SOAdjust.adjdOrderNbr> e)
		{
			try
			{
				if (e.Row.AdjdCuryInfoID == null)
				{
					foreach (PXResult<SOOrder, CurrencyInfo> res in
						PXSelectJoin<SOOrder,
							InnerJoin<CurrencyInfo, On<CurrencyInfo.curyInfoID, Equal<SOOrder.curyInfoID>>>,
							Where<SOOrder.orderType, Equal<Required<SOOrder.orderType>>,
								And<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>>>
							.Select(Base, e.Row.AdjdOrderType, e.Row.AdjdOrderNbr))
					{
						UpdateAppliedToOrderAmount(res, res, e.Row);
						return;
					}
				}
			}
			catch (PXSetPropertyException ex)
			{
				throw new PXException(ex.Message);
			}
		}

		protected virtual void UpdateAppliedToOrderAmount(SOOrder order, CurrencyInfo curyInfo, SOAdjust adj)
		{
			SOOrder orderCopy = PXCache<SOOrder>.CreateCopy(order);

			adj.CustomerID = Base.Document.Current.CustomerID;
			adj.AdjgDocDate = Base.Document.Current.DocDate;
			adj.AdjgCuryInfoID = Base.Document.Current.CuryInfoID;
			adj.AdjdCuryInfoID = curyInfo.CuryInfoID;
			adj.AdjdOrigCuryInfoID = orderCopy.CuryInfoID;
			adj.AdjdOrderDate = orderCopy.OrderDate > Base.Document.Current.DocDate
				? Base.Document.Current.DocDate
				: orderCopy.OrderDate;
			adj.Released = false;

			SOAdjust other = PXSelectGroupBy<SOAdjust,
				Where<SOAdjust.voided, Equal<False>,
				And<SOAdjust.adjdOrderType, Equal<Required<SOAdjust.adjdOrderType>>,
				And<SOAdjust.adjdOrderNbr, Equal<Required<SOAdjust.adjdOrderNbr>>,
				And<Where<SOAdjust.adjgDocType, NotEqual<Required<SOAdjust.adjgDocType>>, Or<SOAdjust.adjgRefNbr, NotEqual<Required<SOAdjust.adjgRefNbr>>>>>>>>,
				Aggregate<GroupBy<SOAdjust.adjdOrderType,
				GroupBy<SOAdjust.adjdOrderNbr, Sum<SOAdjust.curyAdjdAmt, Sum<SOAdjust.adjAmt>>>>>>
				.Select(Base, adj.AdjdOrderType, adj.AdjdOrderNbr, adj.AdjgDocType, adj.AdjgRefNbr);
			if (other != null && other.AdjdOrderNbr != null)
			{
				orderCopy.CuryDocBal -= other.CuryAdjdAmt;
				orderCopy.DocBal -= other.AdjAmt;
			}

			CalcBalances(adj, orderCopy, false, true);

			decimal? CuryApplAmt = adj.CuryDocBal - adj.CuryDiscBal;
			decimal? CuryApplDiscAmt = adj.CuryDiscBal;
			decimal? CuryUnappliedBal = Base.Document.Current.CuryDocBal;

			if (adj.CuryDiscBal >= 0m && adj.CuryDocBal - adj.CuryDiscBal <= 0m)
			{
				//no amount suggestion is possible
				return;
			}

			if (Base.Document.Current != null && string.IsNullOrEmpty(Base.Document.Current.DocDesc))
			{
				Base.Document.Current.DocDesc = orderCopy.OrderDesc;
			}
			if (Base.Document.Current != null && CuryUnappliedBal > 0m)
			{
				CuryApplAmt = Math.Min((decimal)CuryApplAmt, (decimal)CuryUnappliedBal);

				if (CuryApplAmt + CuryApplDiscAmt < adj.CuryDocBal)
				{
					CuryApplDiscAmt = 0m;
				}
			}
			else if (Base.Document.Current != null && CuryUnappliedBal <= 0m && ((ARInvoice)Base.Document.Current).CuryOrigDocAmt > 0)
			{
				CuryApplAmt = 0m;
				CuryApplDiscAmt = 0m;
			}

			var actual = (SOAdjust)SOAdjustments.Cache.Locate(adj);
			if (actual == null)
			{
				actual = SOAdjust.PK.Find(Base, adj);
			}
			else if (SOAdjustments.Cache.GetStatus(actual).IsIn(PXEntryStatus.Deleted, PXEntryStatus.InsertedDeleted))
			{
				actual = null;
			}
			SOAdjustments.Cache.SetValue<SOAdjust.curyAdjgAmt>(adj, actual?.CuryAdjgAmt ?? 0m);
			SOAdjustments.Cache.SetValue<SOAdjust.curyAdjgDiscAmt>(adj, actual?.CuryAdjgDiscAmt ?? 0m);
			SOAdjustments.Cache.SetValueExt<SOAdjust.curyAdjgAmt>(adj, CuryApplAmt);
			SOAdjustments.Cache.SetValueExt<SOAdjust.curyAdjgDiscAmt>(adj, CuryApplDiscAmt);

			CalcBalances(adj, orderCopy, true, true);
		}

		#region TermsByCustomer
		public class TermsByCustomer<CustomerID, TermsID, DocType> : BqlFormulaEvaluator<CustomerID, TermsID, DocType>
			where CustomerID : IBqlOperand
			where TermsID : IBqlOperand
			where DocType : IBqlOperand
		{
			public override object Evaluate(PXCache cache, object item, Dictionary<Type, object> pars)
			{
				int? customerID = (int?)pars[typeof(CustomerID)];
				string invoiceTermsID = (string)pars[typeof(TermsID)];
				string docType = (string)pars[typeof(DocType)];

				if (customerID == null)
					return null;

				Terms det = null;
				object valuePending = cache.GetValuePending<ARInvoice.termsID>(item);
				
				if (IsExternalCall == true || invoiceTermsID == null
					// <PendingValue<ARInvoice.termsID>, IsNotPending>
					|| (valuePending == null || valuePending == PXCache.NotSetValue))
				{
					Customer cust = PXSelect<Customer, Where<Customer.bAccountID, Equal<Required<Customer.bAccountID>>>>.Select(cache.Graph, customerID);
					ARSetup setup = PXSelect<ARSetup>.Select(cache.Graph);
					if (docType == ARDocType.PrepaymentInvoice)
					{
						det = PXSelect<Terms,
								Where<Terms.termsID, Equal<Required<Terms.termsID>>,
									And<Terms.discPercent, Equal<decimal0>,
									And<Terms.installmentType, NotEqual<TermsInstallmentType.multiple>>>>>.Select(cache.Graph, cust.TermsID);
					}
					else if (docType != ARDocType.CreditMemo || setup?.TermsInCreditMemos == true)
					{
						det = PXSelect<Terms, Where<Terms.termsID, Equal<Required<Terms.termsID>>>>.Select(cache.Graph, cust.TermsID);
					}
				}
				return det == null ? (object)null : det.TermsID;
			}
		}
		#endregion

		[PXOverride]
		public void CopyPasteGetScript(bool isImportSimple, List<Api.Models.Command> script, List<Api.Models.Container> containers)
		{
			if (Base.CurrentDocument.Current?.DocType == ARDocType.PrepaymentInvoice)
			{
				(string objectNamePrefix, string fieldName)[] commandsToDelete =
				{
					("Document", "RetainageApply"),
					("CurrentDocument", "RetainageAcctID"),
					("CurrentDocument", "RetainageSubID"),
					("CurrentDocument", "DefRetainagePct"),
					("Transactions", "RetainagePct"),
					("Transactions", "CuryRetainageAmt"),
					("Transactions", "CuryRetainageBal")
				};

				foreach (var (objectNamePrefix, fieldName) in commandsToDelete)
				{
					int commandIndex = script.FindIndex(cmd => cmd.ObjectName.StartsWith(objectNamePrefix)
						&& cmd.FieldName == fieldName);
					if (commandIndex != -1)
					{
						script.RemoveAt(commandIndex);
						containers.RemoveAt(commandIndex);
					}
				}
			}
		}
	}

	public class CalculatePrepaymentPercent<PrepaymentPct, CuryPrepaymentAmt, CuryExtPrice, CuryDiscAmt>
		: BqlFormulaEvaluator<PrepaymentPct, CuryPrepaymentAmt, CuryExtPrice, CuryDiscAmt>
		where PrepaymentPct : IBqlOperand
		where CuryPrepaymentAmt : IBqlOperand
		where CuryExtPrice : IBqlOperand
		where CuryDiscAmt : IBqlOperand
	{
		public override object Evaluate(PXCache cache, object item, Dictionary<Type, object> pars)
		{
			// This constant should be coherent with the decimal places constant
			// used in PXDBDecimal attribute of the field for which this formula is used.
			const int decimalPlaces = 6;
			decimal? prepaymentPct = (decimal?)pars[typeof(PrepaymentPct)];
			decimal? curyPrepaymentAmt = (decimal?)pars[typeof(CuryPrepaymentAmt)];
			decimal? curyExtPrice = (decimal?)pars[typeof(CuryExtPrice)];
			decimal? curyDiscAmt = (decimal?)pars[typeof(CuryDiscAmt)];

			if (curyPrepaymentAmt == null || curyExtPrice == null || curyDiscAmt == null || curyExtPrice - curyDiscAmt == 0)
				return prepaymentPct;

			//calculate prepaymentPct only when curyPrepaymentAmt updated
			object curyPrepaymentAmtValuePending = cache.GetValuePending<ARTranVATRecognitionOnPrepayments.curyPrepaymentAmt>(cache.Current);
			if (curyPrepaymentAmtValuePending == null || curyPrepaymentAmtValuePending == PXCache.NotSetValue)
			{
				return prepaymentPct;
			}

			decimal pct = decimal.Round((100m * curyPrepaymentAmt / (curyExtPrice - curyDiscAmt)).Value, decimalPlaces, MidpointRounding.AwayFromZero);

			return 0 <= pct && pct <= 100 ? pct : prepaymentPct;
		}
	}
}
