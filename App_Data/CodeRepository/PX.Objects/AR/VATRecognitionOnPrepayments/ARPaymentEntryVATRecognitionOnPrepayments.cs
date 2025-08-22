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
using PX.Common;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.SO;
using PX.Objects.SO.GraphExtensions.ARPaymentEntryExt;
using System;
using System.Linq;

namespace PX.Objects.AR
{
	public class ARPaymentEntryVATRecognitionOnPrepayments : PXGraphExtension<OrdersToApplyTab, ARPaymentEntry>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.vATRecognitionOnPrepayments>();
		}

		public override void Initialize()
		{
			base.Initialize();
			Base.Document.WhereAnd<Where<ARPayment.docType, NotEqual<ARDocType.prepaymentInvoice>,
										Or<ARPayment.released, Equal<True>>>>();
			Base.ARPost.WhereAnd<Where<ARTranPostBal.accountID, Equal<Current<ARPayment.aRAccountID>>,
										Or<Where2<Not<ARTranPostBal.docType, Equal<ARDocType.prepaymentInvoice>, And<ARTranPostBal.sourceDocType, Equal<ARDocType.creditMemo>>>,
												And<Not<ARTranPostBal.docType, Equal<ARDocType.creditMemo>, And<ARTranPostBal.sourceDocType, Equal<ARDocType.prepaymentInvoice>>>>>>>>();
		}

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXRemoveBaseAttribute(typeof(ARPaymentType.RefNbrAttribute))]
		[ARPaymentType.RefNbr(typeof(Search2<Standalone.ARRegisterAlias.refNbr,
			InnerJoinSingleTable<ARPayment, On<ARPayment.docType, Equal<Standalone.ARRegisterAlias.docType>,
				And<ARPayment.refNbr, Equal<Standalone.ARRegisterAlias.refNbr>>>,
			InnerJoinSingleTable<Customer, On<Standalone.ARRegisterAlias.customerID, Equal<Customer.bAccountID>>>>,
			Where<Standalone.ARRegisterAlias.docType, Equal<Current<ARPayment.docType>>,
				And2<Where<Standalone.ARRegisterAlias.docType, NotEqual<ARDocType.prepaymentInvoice>,
					Or<Standalone.ARRegisterAlias.released, Equal<True>>>,
				And<Match<Customer, Current<AccessInfo.userName>>>>>,
			OrderBy<Desc<Standalone.ARRegisterAlias.refNbr>>>), Filterable = true, IsPrimaryViewCompatible = true)]
		protected virtual void ARPayment_RefNbr_CacheAttached(PXCache sender) { }

		protected virtual void _(Events.FieldVerifying<ARPayment.adjDate> e)
		{
			ARPayment doc = (ARPayment)e.Row;
			if (doc == null) return;

			if (doc.DocType == ARDocType.PrepaymentInvoice)
			{
				ARAdjust latestAdj = PXSelect<ARAdjust,
					Where<ARAdjust.adjdDocType, Equal<Required<ARAdjust.adjdDocType>>,
						And<ARAdjust.adjdRefNbr, Equal<Required<ARAdjust.adjdRefNbr>>,
						And<ARAdjust.adjgDocType, NotEqual<ARDocType.creditMemo>,
						And<ARAdjust.released, Equal<True>,
						And<ARAdjust.voided, NotEqual<True>>>>>>,
					OrderBy<Desc<ARAdjust.adjgDocDate>>>.SelectSingleBound(Base, null, doc.DocType, doc.RefNbr);
				if (latestAdj != null && (DateTime)e.NewValue < latestAdj.AdjgDocDate)
				{
					throw new PXSetPropertyException(Messages.ApplicationDateCannotBeEarlierThanPayment, latestAdj.AdjgDocDate.Value.ToString("d"));
				}
			}
		}

		protected virtual void _(Events.FieldVerifying<ARPayment.adjFinPeriodID> e)
		{
			ARPayment doc = (ARPayment)e.Row;
			if (doc == null) return;

			if (doc.DocType == ARDocType.PrepaymentInvoice)
			{
				ARAdjust latestAdj = PXSelect<ARAdjust,
					Where<ARAdjust.adjdDocType, Equal<Required<ARAdjust.adjdDocType>>,
						And<ARAdjust.adjdRefNbr, Equal<Required<ARAdjust.adjdRefNbr>>,
						And<ARAdjust.adjgDocType, NotEqual<ARDocType.creditMemo>,
						And<ARAdjust.released, Equal<True>,
						And<ARAdjust.voided, NotEqual<True>>>>>>,
					OrderBy<Desc<ARAdjust.adjgFinPeriodID>>>.SelectSingleBound(Base, null, doc.DocType, doc.RefNbr);
				if (latestAdj != null && latestAdj.AdjgFinPeriodID.CompareTo((string)e.NewValue) > 0)
				{
					e.NewValue = FinPeriodIDAttribute.FormatForDisplay((string)e.NewValue);
					throw new PXSetPropertyException(Messages.ApplicationPeriodCannotBeEarlierThanPayment, FinPeriodIDAttribute.FormatForError(latestAdj.AdjgFinPeriodID));
				}
			}
		}

		protected virtual void _(Events.RowInserting<ARPayment> e)
		{
			ARPayment doc = e.Row as ARPayment;
			if (doc?.DocType == ARDocType.PrepaymentInvoice
				&& doc.RefNbr == AutoNumberAttribute.GetNewNumberSymbol<ARPayment.refNbr>(e.Cache, e.Row))
			{
				e.Cache.SetValue<ARPayment.released>(e.Row, true);
			}
		}

		protected virtual void _(Events.RowPersisting<ARPayment> e)
		{
			ARPayment pmt = e.Row;

			if (pmt?.DocType == ARDocType.PrepaymentInvoice &&
				e.Cache.GetStatus(e.Row).IsIn(PXEntryStatus.Inserted, PXEntryStatus.InsertedDeleted))
				return;

			if (pmt?.DocType != ARDocType.PrepaymentInvoice ||
				e.Cache.GetStatus(e.Row).IsIn(PXEntryStatus.Deleted, PXEntryStatus.InsertedDeleted))
				return;

			SOAdjust adjToBlanketOrder = (SOAdjust)PXSelectJoin<SOAdjust,
				InnerJoin<SOOrderType, On<SOOrderType.orderType, Equal<SOAdjust.adjdOrderType>>>,
				Where<SOAdjust.adjgDocType, Equal<Required<ARPayment.docType>>,
					And<SOAdjust.adjgRefNbr, Equal<Required<ARPayment.refNbr>>,
					And<SOOrderType.behavior, Equal<SOBehavior.bL>>>>>
				.Select(Base, pmt.DocType, pmt.RefNbr);

			if (adjToBlanketOrder != null)
			{
				Base1.SOAdjustments.Cache.RaiseExceptionHandling<SOAdjust.adjdOrderType>
					(adjToBlanketOrder, adjToBlanketOrder.AdjdOrderType,
					new PXSetPropertyException(Messages.PrepaymentInvoicesAreNotSupportedForBlanketSalesOrders));
				throw new PXRowPersistingException(typeof(SOAdjust.adjdOrderType).Name,
					adjToBlanketOrder.AdjdOrderType, Messages.PrepaymentInvoicesAreNotSupportedForBlanketSalesOrders);
			}
		}

		private void CheckForBlanketOrders(SOAdjust adj)
		{
			if (adj?.AdjgDocType != ARDocType.PrepaymentInvoice || adj?.AdjdOrderType == null)
				return;

			bool orderTypeHasBlanketBehaviour = PXSelect<SOOrderType,
				Where<SOOrderType.orderType, Equal<Required<SOOrderType.orderType>>,
					And<SOOrderType.behavior, Equal<SOBehavior.bL>>>>
				.Select(Base, adj.AdjdOrderType).Any();

			if (orderTypeHasBlanketBehaviour)
			{
				Base1.SOAdjustments.Cache.RaiseExceptionHandling<SOAdjust.adjdOrderType>
					(adj, adj.AdjdOrderType, new PXSetPropertyException(Messages.PrepaymentInvoicesAreNotSupportedForBlanketSalesOrders));
			}
		}

		protected virtual void _(Events.FieldUpdating<SOAdjust.adjdOrderType> e) => CheckForBlanketOrders(e.Row as SOAdjust);

		protected virtual void _(Events.RowInserting<SOAdjust> e) => CheckForBlanketOrders(e.Row);

		#region Cache Attached Events

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[Account(Visibility = PXUIVisibility.Invisible)]
		protected virtual void ARInvoice_PrepaymentAccountID_CacheAttached(PXCache sender) { }

		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXFormula(typeof(Sub<IIf<Where<ARPayment.docType, Equal<ARDocType.prepaymentInvoice>, And<ARPayment.pendingPayment, Equal<True>>>,
									ARPayment.curyOrigDocAmt,
									ARPayment.curyDocBal>,
							Add<ARPayment.curyApplAmt, ARPayment.curySOApplAmt>>))]
		protected virtual void ARPayment_CuryUnappliedBal_CacheAttached(PXCache sender) { }

		#endregion

		[PXOverride]
		public virtual void CheckDocumentBeforeVoiding(PXGraph graph, ARPayment payment)
		{
			if (payment == null || !(payment.DocType == ARDocType.Payment || payment.DocType == ARDocType.Prepayment))
				return;

			PXResult<ARAdjust, ARAdjust2> application = (PXResult<ARAdjust, ARAdjust2>)PXSelectJoin<ARAdjust,
				InnerJoin<ARAdjust2,
					On<ARAdjust2.adjgRefNbr, Equal<ARAdjust.adjdRefNbr>>>,
				Where<ARAdjust.adjgDocType, Equal<Required<ARAdjust.adjgDocType>>,
					And<ARAdjust.adjgRefNbr, Equal<Required<ARAdjust.adjgRefNbr>>,
					And<ARAdjust.adjdDocType, Equal<ARDocType.prepaymentInvoice>,
					And<ARAdjust2.adjgDocType, Equal<ARDocType.prepaymentInvoice>,
					And<ARAdjust.released, Equal<True>, And<ARAdjust.voided, NotEqual<True>,
					And<ARAdjust2.voided, NotEqual<True>>>>>>>>,
				OrderBy<Asc<ARAdjust.adjdRefNbr>>>
				.Select(graph, payment.DocType, payment.RefNbr);

			if (application != null)
			{
				ARAdjust applicationOfThisDocToPrepmtInv = application;
				ARAdjust2 applicationOfPrepmtInvToOtherDoc = application;

				throw new PXException(Messages.DocumentCannotBeVoidedDueToApplicationToPrepaymentInvoice,
					ARDocType.GetDisplayName(payment.DocType),
					applicationOfThisDocToPrepmtInv.AdjdRefNbr,
					ARDocType.GetDisplayName(applicationOfPrepmtInvToOtherDoc.AdjdDocType),
					applicationOfPrepmtInvToOtherDoc.AdjdRefNbr,
					ARDocType.GetDisplayName(applicationOfPrepmtInvToOtherDoc.AdjdDocType));
			}
		}

		[PXOverride]
		public virtual void CheckDocumentBeforeReversing(PXGraph graph, ARAdjust application)
		{
			if (application != null && application.AdjdDocType == ARDocType.PrepaymentInvoice
				&& (application.AdjgDocType == ARDocType.Payment || application.AdjgDocType == ARDocType.Prepayment))
			{
				ARAdjust prepaymentInvoiceApplication = PXSelect <ARAdjust,
					Where<ARAdjust.adjgDocType, Equal<ARDocType.prepaymentInvoice>,
						And<ARAdjust.adjgRefNbr, Equal<Required<ARAdjust.adjgRefNbr>>,
						And<ARAdjust.voided, NotEqual<True>>>>>
					.Select(graph, application.AdjdRefNbr);

				if (prepaymentInvoiceApplication != null)
				{
					throw new PXException(Messages.ApplicationToPrepaymentInvoiceCannotBeReversed,
						ARDocType.GetDisplayName(prepaymentInvoiceApplication.AdjdDocType),
						prepaymentInvoiceApplication.AdjdRefNbr);
				}
			}
		}

	}
}
