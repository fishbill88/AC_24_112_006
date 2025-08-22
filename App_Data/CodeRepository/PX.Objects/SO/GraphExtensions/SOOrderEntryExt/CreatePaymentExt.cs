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
using PX.Objects.AR;
using PX.Objects.CA;
using PX.Objects.CM;
using PX.Objects.CS;
using System;
using System.Collections;
using System.Collections.Generic;
using ARRegisterAlias = PX.Objects.AR.Standalone.ARRegisterAlias;

namespace PX.Objects.SO.GraphExtensions.SOOrderEntryExt
{
	public class CreatePaymentExt : CreatePaymentExtBase<SOOrderEntry, SOOrder, SOAdjust>
	{
		bool isReqPrepaymentCalculationInProgress = false;

		#region Initialize

		public override void Initialize()
		{
			base.Initialize();

			if (!GetAdjustView().Cache.Fields.Contains(nameof(CanDeletePayment)))
			{
				GetAdjustView().Cache.Fields.Add(nameof(CanDeletePayment));

				Base.FieldSelecting.AddHandler(typeof(SOAdjust), nameof(CanDeletePayment),
					CreateVoidCaptureFieldSelecting(nameof(CanDeletePayment), CanDeletePayment));
			}

			if (!GetAdjustView().Cache.Fields.Contains(nameof(CanDeleteRefund)))
			{
				GetAdjustView().Cache.Fields.Add(nameof(CanDeleteRefund));

				Base.FieldSelecting.AddHandler(typeof(SOAdjust), nameof(CanDeleteRefund),
					CreateVoidCaptureFieldSelecting(nameof(CanDeleteRefund), CanDeleteRefund));
			}
		}

		#endregion

		#region Buttons

		[PXUIField(DisplayName = Messages.CreatePayment, MapViewRights = PXCacheRights.Select, MapEnableRights = PXCacheRights.Update)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.AddNew, Tooltip = Messages.CreatePayment, DisplayOnMainToolbar = false, PopupCommand = nameof(syncPaymentTransaction))]        
		protected override IEnumerable CreateDocumentPayment(PXAdapter adapter)
		{
			CanMakeAPayment();
			CheckTermsInstallmentType();

			return base.CreateDocumentPayment(adapter);
		}

		public PXAction<SOOrder> createOrderPrepayment;
		[PXUIField(DisplayName = Messages.CreatePrepayment, MapViewRights = PXCacheRights.Select, MapEnableRights = PXCacheRights.Update)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.AddNew, Tooltip = Messages.CreatePrepayment, DisplayOnMainToolbar = false, PopupCommand = nameof(syncPaymentTransaction))]
		protected virtual IEnumerable CreateOrderPrepayment(PXAdapter adapter)
		{
			CanMakeAPayment();
			CheckTermsInstallmentType();

			if (AskCreatePaymentDialog(Messages.CreatePrepayment) == WebDialogResult.OK)
				CreatePayment(QuickPayment.Current, ARPaymentType.Prepayment, false);

			return adapter.Get();
		}

		public PXAction<SOOrder> deletePayment;
		[PXUIField(DisplayName = "Delete Payment", MapViewRights = PXCacheRights.Select, MapEnableRights = PXCacheRights.Update)]
		[PXButton(Tooltip = "Delete Payment", DisplayOnMainToolbar = false)]
		protected virtual IEnumerable DeletePayment(PXAdapter adapter)
		{
			Base.Save.Press();
			ProcessDeletePayment(Base.Adjustments.Current);

			return adapter.Get();
		}

		public PXAction<SOOrder> deleteRefund;
		[PXUIField(DisplayName = "Delete Refund", MapViewRights = PXCacheRights.Select, MapEnableRights = PXCacheRights.Update)]
		[PXButton(Tooltip = "Delete Refund", DisplayOnMainToolbar = false)]
		protected virtual IEnumerable DeleteRefund(PXAdapter adapter)
		{
			Base.Save.Press();
			ProcessDeletePayment(Base.Adjustments.Current);

			return adapter.Get();
		}

		public virtual void CheckTermsInstallmentType()
		{
			Terms terms = Terms.PK.Find(Base, Base.Document.Current.TermsID);

			if (terms != null && terms.InstallmentType != TermsInstallmentType.Single)
			{
				throw new PXSetPropertyException(AR.Messages.PrepaymentAppliedToMultiplyInstallments);
			}
		}

		public virtual void CanMakeAPayment()
		{
			ARTran tran = SelectFrom<ARTran>
						.Where<ARTran.sOOrderNbr.IsEqual<@P.AsString>
						.And<ARTran.sOOrderType.IsEqual<@P.AsString.ASCII>>
						.And<ARTran.released.IsEqual<False>>
						.And<ARTran.invtMult.IsNotEqual<short0>>>
						.View.Select(Base, Base.Document.Current.OrderNbr, Base.Document.Current.OrderType);

			if (tran != null)
			{
				throw new PXSetPropertyException(Messages.InvoiceHasAlreadyBeenCreatedForThisSalesOrder, tran.RefNbr);
			}
		}

		#endregion // Buttons

		#region SOOrder events

		public virtual SOOrderStateForPayments GetDocumentState(PXCache cache, SOOrder order)
		{
			if (order == null) throw new PXArgumentException(nameof(order));

			SOOrderStateForPayments res = new SOOrderStateForPayments
			{
				IsMixedOrder = order.Behavior == SOBehavior.MO,
				Inserted = Base.Caches<SOOrder>().GetStatus(order) == PXEntryStatus.Inserted,
				PaymentsAllowed = Base.soordertype.Current?.CanHavePayments ?? false,
				RefundsAllowed = Base.soordertype.Current?.CanHaveRefunds ?? false,
				PaymentType = PaymentMethod.PK.Find(Base, order.PaymentMethodID)?.PaymentType
			};

			res.PaymentsAllowedByAmount = (!res.IsMixedOrder || order.CuryOrderTotal >= 0m);
			res.DocStatusAllowsPayment = order.Completed != true
				&& order.Cancelled != true
				&& ((order.Approved ?? order.DontApprove == true) || order.Hold == true)
				&& Base.IsAddingPaymentsAllowed(order, Base.soordertype.Current)
				&& (!res.IsMixedOrder || order.Status.IsIn(SOOrderStatus.Open, SOOrderStatus.CreditHold));

			res.CreatePaymentEnabled = res.PaymentsAllowed && !res.Inserted && res.DocStatusAllowsPayment && res.PaymentsAllowedByAmount;

			res.RefundsAllowedByAmount = (!res.IsMixedOrder || order.CuryOrderTotal < 0m);

			res.CreateRefundEnabled = res.RefundsAllowed && !res.Inserted && order.Status == SOOrderStatus.Open
				&& Base.soordertype.Current?.AllowRefundBeforeReturn == true && res.RefundsAllowedByAmount;

			res.ImportPaymentEnabled = res.CreatePaymentEnabled && order.PaymentMethodID != null &&
				(res.PaymentType == PaymentMethodType.CreditCard || res.PaymentType == PaymentMethodType.EFT);

			res.IsReqPrepaymentEnabled = GetRequiredPrepaymentEnabled(order);
			res.ChildExists = order.ChildLineCntr > 0;

			return res;
		}

		protected override void _(Events.RowSelected<SOOrder> eventArgs)
		{
			base._(eventArgs);
			if (eventArgs.Row == null)
				return;

			SOOrderStateForPayments docState = GetDocumentState(eventArgs.Cache , eventArgs.Row);

			createDocumentPayment.SetVisible(docState.PaymentsAllowed);
			createDocumentPayment.SetEnabled(docState.CreatePaymentEnabled);
			createDocumentRefund.SetVisible(docState.RefundsAllowed);
			createDocumentRefund.SetEnabled(docState.CreateRefundEnabled);
			createOrderPrepayment.SetVisible(docState.PaymentsAllowed && !docState.IsMixedOrder);
			createOrderPrepayment.SetEnabled(docState.CreatePaymentEnabled && !docState.IsMixedOrder);
			importDocumentPayment.SetVisible(docState.PaymentsAllowed && !docState.IsMixedOrder);
			importDocumentPayment.SetEnabled(docState.ImportPaymentEnabled && !docState.IsMixedOrder);
			captureDocumentPayment.SetVisible(docState.PaymentsAllowed && !docState.IsMixedOrder);
			voidDocumentPayment.SetVisible(docState.PaymentsAllowed);
			deletePayment.SetVisible(docState.IsMixedOrder);
			deleteRefund.SetVisible(docState.IsMixedOrder);

			eventArgs.Cache.Adjust<PXUIFieldAttribute>(eventArgs.Row)
				.For<SOOrder.curyPrepaymentReqAmt>(a => a.Enabled = eventArgs.Row.OverridePrepayment == true)
				.SameFor<SOOrder.prepaymentReqPct>();

			PXUIFieldAttribute.SetEnabled<SOOrder.prepaymentReqSatisfied>(eventArgs.Cache, eventArgs.Row, false);

			eventArgs.Cache.Adjust<PXUIFieldAttribute>(eventArgs.Row)
				.For<SOOrder.prepaymentReqPct>(a => a.Visible = docState.IsReqPrepaymentEnabled)
				.SameFor<SOOrder.curyPrepaymentReqAmt>()
				.SameFor<SOOrder.overridePrepayment>()
				.SameFor<SOOrder.prepaymentReqSatisfied>();

			ARPaymentType.SOListAttribute.SetPaymentList<SOAdjust.adjgDocType>(Base.Adjustments.Cache, docState.RefundsAllowed && docState.RefundsAllowedByAmount);

			PXSetPropertyException setPropertyException = null;

			eventArgs.Cache.RaiseExceptionHandling<SOOrder.curyPaymentTotal>(
				eventArgs.Row, eventArgs.Row.CuryPaymentTotal, setPropertyException);

			createDocumentPayment.SetTooltip(docState.ChildExists ? Messages.CannotCreatePaymentBecauseOfChildOrders : Messages.CreatePayment);
			createOrderPrepayment.SetTooltip(docState.ChildExists ? Messages.CannotCreatePrepaymentBecauseOfChildOrders : Messages.CreatePrepayment);
			importDocumentPayment.SetTooltip(docState.ChildExists ? Messages.CannotImportPaymentBecauseOfChildOrders : string.Empty);
		}

		protected virtual void _(Events.FieldVerifying<SOOrder, SOOrder.cancelled> eventArgs)
		{
			if ((bool?)eventArgs.NewValue == true)
			{
				SOAdjust adj = GetPaymentLinkToOrder(eventArgs.Row);
				if (adj != null)
					throw new PXException(adj.AdjgDocType == ARPaymentType.Refund
						? Messages.OrderWithAppliedRefundsCantBeCancelled : Messages.OrderWithAppliedPaymentsCantBeCancelled,
						eventArgs.Row.OrderNbr, adj.AdjgRefNbr);
			}
		}

		protected virtual void _(Events.RowDeleting<SOOrder> eventArgs)
		{
			SOAdjust adj = GetPaymentLinkToOrder(eventArgs.Row);
			if (adj != null)
			{
				throw new PXException(Messages.OrderWithAppliedPaymentsCantBeDeleted, eventArgs.Row.OrderNbr);
			}
		}

		protected virtual void _(Events.FieldUpdated<SOOrder, SOOrder.termsID> eventArgs)
		{
			if (eventArgs.Row == null)
				return;

			eventArgs.Cache.SetDefaultExt<SOOrder.overridePrepayment>(eventArgs.Row);
		}

		protected virtual void _(Events.FieldUpdated<SOOrder, SOOrder.overridePrepayment> eventArgs)
		{
			if (eventArgs.Row.OverridePrepayment != (bool?)eventArgs.OldValue)
			{
				if (eventArgs.Row.DontApprove != true && eventArgs.Row.Approved == true)
					eventArgs.Cache.SetValueExt<SOOrder.approved>(eventArgs.Row, false);

				if (eventArgs.Row.OverridePrepayment != true)
					eventArgs.Cache.SetDefaultExt<SOOrder.prepaymentReqPct>(eventArgs.Row);
			}
		}

		protected virtual void _(Events.FieldVerifying<SOOrder, SOOrder.prepaymentReqPct> eventArgs)
		{
			if (eventArgs.Row == null)
				return;

			decimal? newValue = (decimal?)eventArgs.NewValue;

			// TODO: SOCreatePayment: Investigate why if we used "throw PXSetPropertyException" instead of RaiseExceptionHandling,
			// the system doesn't clear error message when an user change the value of the override flag. (Or changed the value of the prepayment percent)
			PXSetPropertyException setPropertyException = null;

			if (newValue < 0m)
				setPropertyException = new PXSetPropertyException<SOOrder.prepaymentReqPct>(Messages.PrepaymentPercentShouldBeMoreZero);

			if (newValue > 100m)
				setPropertyException = new PXSetPropertyException<SOOrder.prepaymentReqPct>(Messages.PrepaymentPercentShouldBeLess100);

			eventArgs.Cache.RaiseExceptionHandling<SOOrder.prepaymentReqPct>(eventArgs.Row, newValue, setPropertyException);
		}

		protected virtual void _(Events.FieldVerifying<SOOrder, SOOrder.curyPrepaymentReqAmt> eventArgs)
		{
			if (eventArgs.Row == null || !GetRequiredPrepaymentEnabled(eventArgs.Row))
				return;

			decimal? newValue = (decimal?)eventArgs.NewValue;

			PXSetPropertyException setPropertyException = null;

			if (newValue < 0m)
				setPropertyException = new PXSetPropertyException<SOOrder.curyPrepaymentReqAmt>(Messages.PrepaymentShouldBeMoreZero);
			else if (newValue > eventArgs.Row.CuryOrderTotal)
				setPropertyException = new PXSetPropertyException<SOOrder.curyPrepaymentReqAmt>(
					Messages.PrepaymentShouldBeLessOrderTotalAmount, eventArgs.Row.CuryOrderTotal);

			eventArgs.Cache.RaiseExceptionHandling<SOOrder.curyPrepaymentReqAmt>(eventArgs.Row, newValue, setPropertyException);
		}

		protected virtual void _(Events.FieldUpdated<SOOrder, SOOrder.prepaymentReqPct> eventArgs)
		{
			if (eventArgs.Row == null)
				return;

			if (eventArgs.Row.OverridePrepayment == true)
				SetAmountByPercent(eventArgs.Cache, eventArgs.Row);
		}

		protected virtual void _(Events.FieldUpdated<SOOrder, SOOrder.curyOrderTotal> eventArgs)
		{
			if (eventArgs.Row == null)
				return;

			if (eventArgs.Row.OverridePrepayment == true)
				SetAmountByPercent(eventArgs.Cache, eventArgs.Row);
		}

		protected virtual void _(Events.FieldUpdated<SOOrder, SOOrder.curyPrepaymentReqAmt> eventArgs)
		{
			if (eventArgs.Row == null)
				return;

			if (eventArgs.Row.OverridePrepayment == true && !Base.IsCopyPasteContext && !Base.IsCopyOrder)
				SetPercentByAmount(eventArgs.Cache, eventArgs.Row);
		}

		protected virtual void _(Events.RowUpdated<SOOrder> eventArgs)
		{
			if (!eventArgs.Cache.ObjectsEqual<SOOrder.curyPrepaymentReqAmt, SOOrder.curyPaymentOverall, SOOrder.completed>(eventArgs.Row, eventArgs.OldRow))
			{
				if (eventArgs.Row.CuryPaymentOverall >= eventArgs.Row.CuryPrepaymentReqAmt)
					eventArgs.Row.SatisfyPrepaymentRequirements(Base);
				else
					eventArgs.Row.ViolatePrepaymentRequirements(Base);
			}

			if (!eventArgs.Cache.ObjectsEqual<SOOrder.paymentsNeedValidationCntr>(eventArgs.Row, eventArgs.OldRow))
			{
				if (eventArgs.Row.PaymentsNeedValidationCntr == 0 && eventArgs.OldRow.PaymentsNeedValidationCntr != null)
					SOOrder.Events.Select(e => e.LostLastPaymentInPendingProcessing).FireOn(Base, eventArgs.Row);
				else if (eventArgs.OldRow.PaymentsNeedValidationCntr < eventArgs.Row.PaymentsNeedValidationCntr)
					SOOrder.Events.Select(e => e.ObtainedPaymentInPendingProcessing).FireOn(Base, eventArgs.Row);
			}

			if (eventArgs.Row.CreditHold == true && eventArgs.Row.IsFullyPaid == true &&
					eventArgs.OldRow.IsFullyPaid != eventArgs.Row.IsFullyPaid)
			{
				eventArgs.Row.SatisfyCreditLimitByPayment(Base);
			}
		}

		#endregion // SOOrder events

		#region SOLine events

		protected virtual void _(Events.RowDeleted<SOLine> eventArgs)
		{
			MarkRefundAdjUpdatedForValidation(eventArgs.Row);
		}

		protected virtual void _(Events.RowUpdated<SOLine> eventArgs)
		{
			if (!eventArgs.Cache.ObjectsEqual<SOLine.orderQty>(eventArgs.OldRow, eventArgs.Row)
				&& eventArgs.Row.OrderQty == 0m)
			{
				MarkRefundAdjUpdatedForValidation(eventArgs.Row);
			}
		}

		#endregion // SOLine events

		#region SOAdjust events

		protected override void _(Events.RowSelected<SOAdjust> eventArgs)
		{
			if (ARPaymentType.DrCr(eventArgs.Row?.AdjgDocType) == GL.DrCr.Credit)
			{
				PXSetPropertyException exception = null;

				if (PXAccess.FeatureInstalled<FeaturesSet.integratedCardProcessing>() &&
					eventArgs.Row.IsCCPayment == true &&
					eventArgs.Row.IsCCAuthorized != true &&
					eventArgs.Row.IsCCCaptured != true &&
					eventArgs.Row.Voided != true &&
					eventArgs.Row.Released != true &&
					eventArgs.Row.PaymentReleased != true)
				{
					exception = new PXSetPropertyException(Messages.PaymentHasNoActiveRefundedTransaction, PXErrorLevel.RowWarning, eventArgs.Row.AdjgRefNbr);
				}

				eventArgs.Cache.RaiseExceptionHandling(GetPaymentErrorFieldName(), eventArgs.Row, null, exception);
			}
			else
			{
				base._(eventArgs);
			}
		}

		protected virtual void _(Events.RowDeleting<SOAdjust> eventArgs)
		{
			if (eventArgs.Row?.CuryAdjdBilledAmt > 0)
			{
				throw new PXException(Messages.PaymentsCantBeRemovedTransferedToInvoice, eventArgs.Row.AdjgRefNbr);
			}
		}

		protected virtual void _(Events.RowPersisting<SOAdjust> eventArgs)
		{
			if (eventArgs.Operation.Command().IsNotIn(PXDBOperation.Insert, PXDBOperation.Update)
				|| eventArgs.Row.AdjgDocType != ARPaymentType.Refund
				|| eventArgs.Row.ValidateCCRefundOrigTransaction == false)
				return;

			ARPayment refund = ARPayment.PK.Find(Base, eventArgs.Row.AdjgDocType, eventArgs.Row.AdjgRefNbr);
			if (!string.IsNullOrEmpty(refund?.RefTranExtNbr)
				&& !HasReturnLineForOrigTran(refund.ProcessingCenterID, refund.RefTranExtNbr))
			{
				eventArgs.Cache.RaiseExceptionHandling<SOAdjust.adjgDocType>(
					eventArgs.Row, eventArgs.Row.AdjgDocType,
					new PXSetPropertyException(Messages.OrigTranNumberNotRelatedToReturnedInvoices,
					PXErrorLevel.RowError, refund.RefTranExtNbr));
			}
		}

		#endregion // SOAdjust events

		#region SOQuickPayment events

		#region SOQuickPayment CacheAttached

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXRestrictor(typeof(Where2<Where<Exists<Select<SOOrderType, 
			Where<SOOrderType.orderType, Equal<Current2<SOOrder.orderType>>, And<SOOrderType.validateCCRefundsOrigTransactions, Equal<False>>>>>>,
			Or<Where<Exists<Select2<ARAdjust,
			InnerJoin<SOLine, On<SOLine.invoiceType, Equal<ARAdjust.adjdDocType>, And<SOLine.invoiceNbr, Equal<ARAdjust.adjdRefNbr>>>>,
			Where<ARAdjust.adjgDocType, Equal<ARPayment.docType>, And<ARAdjust.adjgRefNbr, Equal<ARPayment.refNbr>,
				And<SOLine.orderType, Equal<Current2<SOOrder.orderType>>, And<SOLine.orderNbr, Equal<Current2<SOOrder.orderNbr>>,
				And<ARAdjust.voided, NotEqual<True>, And<ARAdjust.curyAdjdAmt, NotEqual<decimal0>,
				And<SOLine.curyLineAmt, NotEqual<decimal0>>>>>>>>>>>>>),
			Messages.OrigTranNumberNotRelatedToReturnedInvoices, typeof(ExternalTransaction.tranNumber))]
		protected virtual void _(Events.CacheAttached<SOQuickPayment.refTranExtNbr> eventArgs)
		{
		}

		#endregion // SOQuickPayment CacheAttached

		#endregion // SOQuickPayment events

		#region Override methods

		protected override PXSelectBase<SOAdjust> GetAdjustView()
			=> Base.Adjustments;

		protected override PXSelectBase<SOAdjust> GetAdjustView(ARPaymentEntry paymentEntry)
			=> paymentEntry.GetOrdersToApplyTabExtension(true).SOAdjustments;

		protected override ARSetup GetARSetup()
			=> Base.arsetup.Current;

		protected override CustomerClass GetCustomerClass()
			=> Base.customerclass.SelectSingle();

		protected override void SetCurrentDocument(SOOrder document)
		{
			Base.Document.Current = Base.Document.Search<SOOrder.orderNbr>(document.OrderNbr, document.OrderType);
		}

		protected override void AddAdjust(ARPaymentEntry paymentEntry, SOOrder document)
		{
			var newAdjust = new SOAdjust()
			{
				AdjdOrderType = document.OrderType,
				AdjdOrderNbr = document.OrderNbr
			};

			paymentEntry.GetOrdersToApplyTabExtension(true).SOAdjustments.Insert(newAdjust);
		}

		protected override void VerifyAdjustments(ARPaymentEntry paymentEntry, string actionName)
		{
			SOOrder document = Base.Document.Current;
			ARPayment payment = paymentEntry.Document.Current;

			if (IsMultipleApplications(paymentEntry, out ARPaymentTotals paymentTotals, out SOInvoice invoice))
			{
				if (actionName == nameof(CaptureDocumentPayment))
				{
					if (payment.DocType == ARDocType.Payment)
						throw new PXException(Messages.CapturePaymentWithMultipleApplicationsError, payment.RefNbr);
					else
						throw new PXException(Messages.CapturePrepaymentWithMultipleApplicationsError, payment.RefNbr);
				}
				else
				{
					if (payment.DocType == ARDocType.Payment)
						throw new PXException(Messages.VoidPaymentWithMultipleApplicationsError, payment.RefNbr);
					else
						throw new PXException(Messages.VoidPrepaymentWithMultipleApplicationsError, payment.RefNbr);
				}
			}

			if (IsPaymentLinkedToInvoiceWithTheSameOrder(paymentTotals, invoice))
			{
				if (actionName == nameof(CaptureDocumentPayment))
				{
					if (payment.DocType == ARDocType.Payment)
						throw new PXException(Messages.CaptureTransferedToInvoicePaymentError, invoice.RefNbr, document.OrderNbr, payment.RefNbr);
					else
						throw new PXException(Messages.CaptureTransferedToInvoicePrepaymentError, invoice.RefNbr, document.OrderNbr, payment.RefNbr);
				}
				else
				{
					if (payment.DocType == ARDocType.Payment)
						throw new PXException(Messages.VoidTransferedToInvoicePaymentError, invoice.RefNbr, document.OrderNbr, payment.RefNbr);
					else
						throw new PXException(Messages.VoidTransferedToInvoicePrepaymentError, invoice.RefNbr, document.OrderNbr, payment.RefNbr);
				}
			}
		}

		protected override void PrepareForCreateCCPayment(SOOrder doc)
		{
			base.PrepareForCreateCCPayment(doc);
			CheckTermsInstallmentType();
		}

		protected override bool CanVoid(SOAdjust adjust, ARPayment payment)
		{
			bool isMixedOrder = Base.Document.Current?.Behavior == SOBehavior.MO;
			return base.CanVoid(adjust, payment) &&
				Base.Document.Current?.Completed == false &&
				Base.Document.Current?.Cancelled == false &&
				(!isMixedOrder || Base.Document.Current.Status.IsIn(SOOrderStatus.Hold, SOOrderStatus.Open, SOOrderStatus.CreditHold));
		}

		protected override bool CanCapture(SOAdjust adjust, ARPayment payment)
		{
			return base.CanCapture(adjust, payment) &&
				Base.Document.Current?.Completed == false &&
				Base.Document.Current?.Cancelled == false;
		}

		protected override string GetDocumentDescr(SOOrder document) => document.OrderDesc;

		protected override Type GetPaymentMethodField()
			=> typeof(SOOrder.paymentMethodID);

		protected override bool IsCashSale()
			=> Base.Document.Current?.IsCashSaleOrder == true;

		protected override string GetCCPaymentIsNotSupportedMessage()
			=> Messages.CCPaymentMethodIsNotSupportedInSOCashSale;

		protected override Type GetDocumentPMInstanceIDField()
			=> typeof(SOOrder.pMInstanceID);

		protected override decimal? GetDefaultPaymentAmount(SOQuickPayment qp)
		{
			SOOrder order = GetCurrent<SOOrder>();
			bool mixedOrderRefund = qp?.IsRefund == true && order?.Behavior == SOBehavior.MO;
			if (!mixedOrderRefund)
			{
				return base.GetDefaultPaymentAmount(qp);
			}
			decimal? amt = null;
			if (qp == null || order == null)
				return amt;
			if (qp.CuryID == order.CuryID)
			{
				amt = order.CuryUnrefundedBalance;
			}
			else if (qp.CuryID != null)
			{
				PXCurrencyAttribute.CuryConvCury(QuickPayment.Cache, qp, order.UnrefundedBalance ?? 0m, out decimal curyUnrefundedBalance);
				amt = curyUnrefundedBalance;
			}
			return amt;
		}

		protected override bool AllowsAuthorization(SOQuickPayment qp)
		{
			return base.AllowsAuthorization(qp) && (Base.soordertype.Current?.CanHaveRefunds != true);
		}

		protected override string GetExceedingAmountErrorMessage(SOQuickPayment qp)
		{
			if (qp.IsRefund == true && GetCurrent<SOOrder>()?.Behavior == SOBehavior.MO)
			{
				return Messages.RefundAmountMustBeLessUnrefunded;
			}
			return base.GetExceedingAmountErrorMessage(qp);
		}

		#endregion // Override methods

		#region Methods

		protected virtual SOAdjust GetPaymentLinkToOrder(SOOrder order)
		{
			if (order?.CuryPaymentTotal > 0m)
			{
				foreach (PXResult<SOAdjust, ARRegisterAlias, ARPayment> res
					in Base.Adjustments_Raw.View.SelectMultiBound(new object[] { order }))
				{
					SOAdjust adj = res;
					if (adj?.Voided == false && adj.CuryAdjdAmt > 0m)
					{
						return adj;
					}
				}
			}

			return null;
		}

		protected virtual bool IsPaymentLinkedToInvoiceWithTheSameOrder(ARPaymentTotals paymentTotals, SOInvoice invoice)
		{
			return (paymentTotals != null && invoice != null &&
				paymentTotals.AdjdOrderType == invoice.SOOrderType &&
				paymentTotals.AdjdOrderNbr == invoice.SOOrderNbr);
		}

		protected virtual bool GetRequiredPrepaymentEnabled(SOOrder order)
		{
			return order?.Behavior == SOBehavior.SO &&
				order.IsTransferOrder != true &&
				order.IsNoAROrder != true &&
				order.TermsID != null &&
				(order.OverridePrepayment == true || order.PrepaymentReqPct > 0m);
		}

		protected virtual void SetAmountByPercent(PXCache cache, SOOrder order)
		{
			if (!isReqPrepaymentCalculationInProgress)
			{
				try
				{
					isReqPrepaymentCalculationInProgress = true;

					decimal? prepaymentAmount = 0m;

					if (order.PrepaymentReqPct == null ||
						order.PrepaymentReqPct < 0m ||
						order.PrepaymentReqPct > 100m)
					{
						prepaymentAmount = null;
					}
					else if (order.PrepaymentReqPct != 0m)
					{
						prepaymentAmount = order.CuryOrderTotal * order.PrepaymentReqPct / 100.0m;
						prepaymentAmount = PXCurrencyAttribute.RoundCury(cache, order, (decimal)prepaymentAmount);
					}
					cache.SetValueExt<SOOrder.curyPrepaymentReqAmt>(order, prepaymentAmount);
				}
				finally
				{
					isReqPrepaymentCalculationInProgress = false;
				}
			}
		}

		protected virtual void SetPercentByAmount(PXCache cache, SOOrder order)
		{
			const int PercentPrecision = 2;

			if (!isReqPrepaymentCalculationInProgress)
			{
				try
				{
					isReqPrepaymentCalculationInProgress = true;

					decimal? prepaymentPercent = 0m;

					if (order.CuryPrepaymentReqAmt == null ||
						order.CuryPrepaymentReqAmt > order.CuryOrderTotal ||
						order.CuryPrepaymentReqAmt < 0m)
					{
						prepaymentPercent = null;
					}
					else if (order.CuryPrepaymentReqAmt != 0m && (order.CuryOrderTotal ?? 0m) != 0m)
					{
						prepaymentPercent = order.CuryPrepaymentReqAmt * 100.0m / order.CuryOrderTotal;
						if (prepaymentPercent > 100.0m)
							prepaymentPercent = 100.0m;

						prepaymentPercent = PXCurrencyAttribute.Round(cache, order, (decimal)prepaymentPercent, CMPrecision.TRANCURY, PercentPrecision);
					}
					cache.SetValueExt<SOOrder.prepaymentReqPct>(order, prepaymentPercent);
				}
				finally
				{
					isReqPrepaymentCalculationInProgress = false;
				}
			}
		}

		protected override bool HasReturnLineForOrigTran(string procCenterID, string tranNumber)
		{
			if (string.IsNullOrEmpty(procCenterID))
			{
				throw new PXArgumentException(nameof(procCenterID));
			}
			if (string.IsNullOrEmpty(tranNumber))
			{
				throw new PXArgumentException(nameof(tranNumber));
			}
			SOLine returnLine = SelectFrom<SOLine>
				.InnerJoin<ARAdjust>.On<ARAdjust.adjdDocType.IsEqual<SOLine.invoiceType>
					.And<ARAdjust.adjdRefNbr.IsEqual<SOLine.invoiceNbr>>>
				.InnerJoin<ExternalTransaction>.On<ExternalTransaction.docType.IsEqual<ARAdjust.adjgDocType>
					.And<ExternalTransaction.refNbr.IsEqual<ARAdjust.adjgRefNbr>>>
				.Where<SOLine.orderType.IsEqual<SOOrder.orderType.FromCurrent>
					.And<SOLine.orderNbr.IsEqual<SOOrder.orderNbr.FromCurrent>>
					.And<ExternalTransaction.processingCenterID.IsEqual<@P.AsString>>
					.And<ExternalTransaction.tranNumber.IsEqual<@P.AsString>>
					.And<ARAdjust.voided.IsNotEqual<True>>.And<ARAdjust.curyAdjdAmt.IsNotEqual<decimal0>>
					.And<SOLine.curyLineAmt.IsNotEqual<decimal0>>>
				.View.SelectWindowed(Base, 0, 1, procCenterID, tranNumber);
			return returnLine != null;
		}

		protected override bool ValidateCCRefundOrigTransaction()
		{
			SOOrderType orderType = PXSetup<SOOrderType>.Select(Base);

			return orderType?.ValidateCCRefundsOrigTransactions != false;
		}

		protected virtual void MarkRefundAdjUpdatedForValidation(SOLine line)
		{
			if (Base.soordertype.Current?.CanHaveRefunds ?? false
				&& line.Operation == SOOperation.Receipt
				&& !string.IsNullOrEmpty(line.InvoiceNbr))
			{
				foreach (SOAdjust adj in Base.Adjustments.Select())
				{
					Base.Adjustments.Cache.MarkUpdated(adj, assertError: true);
				}
			}
		}

		protected virtual bool CanDeletePayment(SOAdjust adjust, ARPayment payment)
		{
			bool isMixedOrder = Base.Document.Current?.Behavior == SOBehavior.MO;
			return adjust.AdjgDocType.IsIn(ARPaymentType.Payment, ARPaymentType.Prepayment)
				&& payment != null && payment.Status != ARDocStatus.Open
				&& isMixedOrder && Base.Document.Current.Status.IsIn(SOOrderStatus.Hold, SOOrderStatus.Open, SOOrderStatus.CreditHold);
		}

		protected virtual bool CanDeleteRefund(SOAdjust adjust, ARPayment payment)
		{
			bool isMixedOrder = Base.Document.Current?.Behavior == SOBehavior.MO;
			return adjust.AdjgDocType == ARPaymentType.Refund
				&& payment != null && payment.Status != ARDocStatus.Open
				&& isMixedOrder && Base.Document.Current.Status.IsIn(SOOrderStatus.Hold, SOOrderStatus.Open, SOOrderStatus.CreditHold);
		}

		protected virtual void ProcessDeletePayment(SOAdjust adjust)
		{
			ARPaymentEntry paymentEntry = PXGraph.CreateInstance<ARPaymentEntry>();
			paymentEntry.Clear();
			paymentEntry.Document.Current = paymentEntry.Document.Search<ARPayment.refNbr>(adjust.AdjgRefNbr, adjust.AdjgDocType);

			PressButtonIfEnabled(paymentEntry, nameof(paymentEntry.Delete));
			Base.Cancel.Press();
		}

		[PXOverride]
		public virtual void CopyPasteGetScript(bool isImportSimple, List<PX.Api.Models.Command> script, List<PX.Api.Models.Container> containers,
			Action<bool, List<PX.Api.Models.Command>, List<PX.Api.Models.Container>> baseMethod)
		{
			var moveFields = new string[]
			{
				nameof(SOOrder.OverridePrepayment),
				nameof(SOOrder.PrepaymentReqPct),
				nameof(SOOrder.CuryPrepaymentReqAmt),
			};

			foreach (var field in moveFields)
			{
				int commandIndex = script.FindIndex(command => field.Equals(command.FieldName, StringComparison.OrdinalIgnoreCase));

				if (commandIndex != -1)
				{
					Api.Models.Command cmdCustField = script[commandIndex];
					Api.Models.Container cntCustField = containers[commandIndex];

					script.Remove(cmdCustField);
					containers.Remove(cntCustField);

					script.Add(cmdCustField);
					containers.Add(cntCustField);
				}
			}

			baseMethod?.Invoke(isImportSimple, script, containers);
		}

		#endregion // Methods
	}
}
