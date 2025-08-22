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

using System;
using System.Collections;
using System.Linq;
using PX.Common;
using PX.Data;
using PX.Objects.AR.CCPaymentProcessing.Common;
using PX.Objects.AR.CCPaymentProcessing.Helpers;
using PX.Objects.AR.CCPaymentProcessing.Interfaces;
using PX.Objects.AR.Standalone;
using PX.Objects.CA;
using PX.Objects.CC;
using PX.Objects.CM;
using PX.Objects.Common;
using PX.Objects.Extensions.PaymentTransaction;

namespace PX.Objects.AR.GraphExtensions
{
	public class ARCashSaleEntryPaymentTransaction : PaymentTransactionGraph<ARCashSaleEntry, ARCashSale>
	{
		public static bool IsActive() => PXAccess.FeatureInstalled<CS.FeaturesSet.integratedCardProcessing>();

		public PXSelect<ExternalTransaction> externalTran;

		public PXSelect<DefaultTerminal, Where<DefaultTerminal.userID, Equal<Current<AccessInfo.userID>>,
			And<DefaultTerminal.branchID, Equal<Current<AccessInfo.branchID>>,
				And<DefaultTerminal.processingCenterID, Equal<Current<ARCashSale.processingCenterID>>>>>> DefaultTerminal;

		[PXOverride]
		public virtual void Persist(Action persist)
		{
			ARCashSale payment = Base.Document.Current;
			PaymentMethod pm = Base.paymentmethod.Select();

			if (pm?.PaymentType == PaymentMethodType.POSTerminal && !string.IsNullOrEmpty(payment.ProcessingCenterID) && !string.IsNullOrEmpty(payment?.TerminalID))
			{
				DefaultTerminal defaultTerminal = DefaultTerminal.SelectSingle();
				if (defaultTerminal == null)
				{
					defaultTerminal = new DefaultTerminal
					{
						BranchID = PXContext.GetBranchID(),
						UserID = Base.CurrentUserInformationProvider.GetUserId(),
						ProcessingCenterID = payment.ProcessingCenterID,
						TerminalID = payment.TerminalID,
					};

					DefaultTerminal.Insert(defaultTerminal);
				}
				else if (payment.TerminalID != defaultTerminal.TerminalID)
				{
					defaultTerminal.TerminalID = payment.TerminalID;
					DefaultTerminal.Update(defaultTerminal);
				}
			}

			persist();
		}

		protected override PaymentTransactionDetailMapping GetPaymentTransactionMapping()
		{
			return new PaymentTransactionDetailMapping(typeof(CCProcTran));
		}

		protected override ExternalTransactionDetailMapping GetExternalTransactionMapping()
		{
			return new ExternalTransactionDetailMapping(typeof(ExternalTransaction));
		}

		protected override PaymentMapping GetPaymentMapping()
		{
			return new PaymentMapping(typeof(ARCashSale));
		}

		protected override void MapViews(ARCashSaleEntry graph)
		{
			this.PaymentTransaction = new PXSelectExtension<PaymentTransactionDetail>(Base.ccProcTran);
			this.ExternalTransaction = new PXSelectExtension<ExternalTransactionDetail>(Base.ExternalTran);
		}

		protected override void BeforeVoidPayment(ARCashSale doc)
		{
			base.BeforeVoidPayment(doc);
			ReleaseDoc = doc.VoidAppl == true && doc.Released == false && this.ARSetup.Current.IntegratedCCProcessing == true;
		}

		protected override void BeforeCapturePayment(ARCashSale doc)
		{
			base.BeforeCapturePayment(doc);
			ReleaseDoc = doc.Released == false && ARSetup.Current.IntegratedCCProcessing == true;
		}

		protected override void BeforeCreditPayment(ARCashSale doc)
		{
			base.BeforeCreditPayment(doc);

			if (Base.paymentmethod.Current?.PaymentType == PaymentMethodType.POSTerminal && string.IsNullOrEmpty(doc.RefTranExtNbr) && string.IsNullOrEmpty(doc.TerminalID))
			{
				var ex = new PXSetPropertyException(Messages.EitherOrigTranOrTerminalMustBeSpecified, PXErrorLevel.Error);
				Base.Document.Cache.RaiseExceptionHandling<ARCashSale.refTranExtNbr>(doc, doc.RefTranExtNbr, ex);
				throw ex;
			}

			ReleaseDoc = doc.Released == false && ARSetup.Current.IntegratedCCProcessing == true;
		}

		protected override AfterProcessingManager GetAfterProcessingManager(ARCashSaleEntry graph)
		{
			var manager = GetARCashSaleAfterProcessingManager();
			manager.Graph = graph;
			return manager;
		}

		protected override AfterProcessingManager GetAfterProcessingManager()
		{
			return GetARCashSaleAfterProcessingManager();
		}

		private ARCashSaleAfterProcessingManager GetARCashSaleAfterProcessingManager()
		{
			return new ARCashSaleAfterProcessingManager() { ReleaseDoc = true };
		}

		protected override void RowSelected(Events.RowSelected<ARCashSale> e)
		{
			base.RowSelected(e);
			ARCashSale doc = e.Row;
			if (doc == null)
				return;
			TranHeldwarnMsg = AR.Messages.CCProcessingARPaymentTranHeldWarning;
			PXCache cache = e.Cache;
			bool isDocTypePayment = IsDocTypePayment(doc);
			bool isReleased = doc.Released == true;
			bool isPMInstanceRequired = false;
			if (!string.IsNullOrEmpty(doc.PaymentMethodID))
			{
				isPMInstanceRequired = Base.paymentmethod.Current?.IsAccountNumberRequired ?? false;
			}

			ProcessingCCSettings(doc, cache, isPMInstanceRequired);

			CCProcessingCenter procCenter = CCProcessingCenter.PK.Find(Base, doc.ProcessingCenterID);

			SelectedProcessingCenter = procCenter?.ProcessingCenterID;
			SelectedProcessingCenterType = procCenter?.ProcessingTypeName;
			ExternalTransactionState tranState = GetActiveTransactionState();
			bool canAuthorize = CanAuthorize(doc, tranState, isDocTypePayment);
			bool canAuthorizeIfExtAuthOnly = procCenter?.IsExternalAuthorizationOnly == false;
			this.authorizeCCPayment.SetEnabled(canAuthorize && canAuthorizeIfExtAuthOnly);

			bool canCapture = CanCapture(doc, tranState, isDocTypePayment);
			bool canCaptureIfExtAuthOnly = canAuthorizeIfExtAuthOnly || procCenter?.IsExternalAuthorizationOnly == true && tranState.IsActive;
			this.captureCCPayment.SetEnabled(canCapture && canCaptureIfExtAuthOnly);

			bool docIsNotHoldAndCashReturn = doc.Hold == false && doc.DocType == ARDocType.CashReturn;
			bool canVoid = docIsNotHoldAndCashReturn && (tranState.IsCaptured || tranState.IsPreAuthorized)
				|| (tranState.IsPreAuthorized && isDocTypePayment);
			bool enableCCProcess = EnableCCProcess(doc);
			this.voidCCPayment.SetEnabled(enableCCProcess && canVoid);

			bool canCredit = docIsNotHoldAndCashReturn && !tranState.IsRefunded
				&& (tranState.IsCaptured || tranState.IsPreAuthorized || string.IsNullOrEmpty(doc.OrigRefNbr));
			this.creditCCPayment.SetEnabled(enableCCProcess && canCredit);

			doc.CCPaymentStateDescr = GetPaymentStateDescr(tranState);

			bool canValidate = CanValidate(doc);
			this.validateCCPayment.SetEnabled(canValidate);

			this.recordCCPayment.SetEnabled(false);
			this.recordCCPayment.SetVisible(false);
			this.captureOnlyCCPayment.SetEnabled(false);
			this.captureOnlyCCPayment.SetVisible(false);

			PXUIFieldAttribute.SetRequired<ARCashSale.extRefNbr>(cache, enableCCProcess || ARSetup.Current.RequireExtRef == true);
			PXUIFieldAttribute.SetVisible<ARCashSale.cCPaymentStateDescr>(cache, doc, enableCCProcess && !string.IsNullOrEmpty(doc.CCPaymentStateDescr));
			PXUIFieldAttribute.SetVisible<ARCashSale.refTranExtNbr>(cache, doc, doc.DocType == ARDocType.CashReturn && enableCCProcess);
			PXUIFieldAttribute.SetRequired<ARPayment.pMInstanceID>(cache, isPMInstanceRequired);
			PXDefaultAttribute.SetPersistingCheck<ARPayment.pMInstanceID>(cache, doc, isPMInstanceRequired ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);

			if (enableCCProcess && !isReleased && (tranState.IsPreAuthorized || tranState.IsCaptured
				|| (doc.DocType == ARDocType.CashReturn && (tranState.IsRefunded || CheckLastProcessedTranIsVoided(doc)))))
			{
				SetHeaderFields(cache, doc, false);
				if (doc.Status != ARDocStatus.PendingApproval)
				{
					PXUIFieldAttribute.SetEnabled<ARCashSale.adjDate>(cache, doc, true);
					PXUIFieldAttribute.SetEnabled<ARCashSale.adjFinPeriodID>(cache, doc, true);
				}
				PXUIFieldAttribute.SetEnabled<ARCashSale.hold>(cache, doc, true);
				//calculate only on data entry, differences from the applications will be moved to RGOL upon closure
				PXDBCurrencyAttribute.SetBaseCalc<ARCashSale.curyDocBal>(cache, null, true);
				PXDBCurrencyAttribute.SetBaseCalc<ARCashSale.curyDiscBal>(cache, null, true);

				cache.AllowDelete = doc.DocType == ARDocType.CashReturn && !tranState.IsRefunded && !CheckLastProcessedTranIsVoided(doc);
				cache.AllowUpdate = true;
				Base.Transactions.Cache.AllowDelete = true;
				Base.Transactions.Cache.AllowUpdate = true;
				Base.Transactions.Cache.AllowInsert = doc.CustomerID != null && doc.CustomerLocationID != null;
				Base.release.SetEnabled(doc.Hold == false);
				Base.voidCheck.SetEnabled(false);
			}
			else
			{
				PXUIFieldAttribute.SetEnabled<ARCashSale.refTranExtNbr>(cache, doc, enableCCProcess && !isReleased && ((doc.DocType == ARDocType.CashReturn) && !tranState.IsRefunded));
				if (doc.Released == true || doc.Voided == true)
					cache.AllowDelete = false;
				else
					cache.AllowDelete = !ExternalTranHelper.HasTransactions(Base.ExternalTran);
			}

			#region CCProcessing integrated with doc
			if (enableCCProcess && CCProcessingHelper.IntegratedProcessingActivated(ARSetup.Current))
			{
				if (doc.Released == false)
				{
					bool releaseActionEnabled = doc.Hold == false &&
												doc.OpenDoc == true &&
											   (doc.DocType == ARDocType.CashReturn ? tranState.IsRefunded : tranState.IsCaptured);

					Base.release.SetEnabled(releaseActionEnabled);
				}
			}
			#endregion

			PXUIFieldAttribute.SetEnabled<ARCashSale.docType>(cache, doc, true);
			PXUIFieldAttribute.SetEnabled<ARCashSale.refNbr>(cache, doc, true);
			ShowWarningIfActualFinPeriodClosed(e, doc);
			ShowWarningIfExternalAuthorizationOnly(e, doc);
			ShowWarningOnProcessingCenterID(e, doc);

			SetActionCaptions();

			EnableDisableFieldsAndActions(doc, tranState, cache);
		}

		protected virtual void EnableDisableFieldsAndActions(ARCashSale doc, ExternalTransactionState tranState, PXCache cache)
		{
			bool isPOS = Base.paymentmethod.Current?.PaymentType == PaymentMethodType.POSTerminal;
			PXUIFieldAttribute.SetVisible<ARCashSale.processingCenterID>(cache, doc, isPOS);
			PXUIFieldAttribute.SetVisible<ARCashSale.terminalID>(cache, doc, isPOS);
			PXUIFieldAttribute.SetVisible<ARCashSale.pMInstanceID>(cache, doc, !isPOS);

			if (doc.Released == true || doc.IsCCPayment == false)
			{
				return;
			}

			bool isBalanced = doc.PendingProcessing == false && doc.Voided == false && doc.Hold == false;
			bool isHoldOrPendingProcessing = doc.Hold == true || doc.PendingProcessing == true;
			bool isHoldAndCaptured = doc.Hold == true && tranState.IsCaptured;
			switch (doc.DocType)
			{
				case ARDocType.CashSale:
					SetAdditionalHeaderFields(cache, doc, isHoldOrPendingProcessing && (!tranState.IsCaptured || tranState.IsPreAuthorized));
					SetDetailsTabFields(Base.Transactions.Cache, !(isHoldAndCaptured || isBalanced));
					SetFinancialTabFields(cache, doc, (isHoldOrPendingProcessing && (!tranState.IsCaptured || tranState.IsPreAuthorized)) || isHoldAndCaptured);

					var projDescrEditable = (isHoldOrPendingProcessing && (!tranState.IsCaptured || tranState.IsPreAuthorized)) || isHoldAndCaptured;
					PXUIFieldAttribute.SetEnabled<ARCashSale.projectID>(cache, doc, projDescrEditable);
					PXUIFieldAttribute.SetEnabled<ARCashSale.docDesc>(cache, doc, projDescrEditable);
					PXUIFieldAttribute.SetEnabled<ARTran.qty>(Base.Transactions.Cache, null, !(isHoldAndCaptured || isBalanced));

					bool csTabPermissions = !(isHoldAndCaptured || isBalanced);
					SetTabPermissions(Base.Transactions, csTabPermissions, null, null, csTabPermissions);
					SetTabPermissions(Base.Taxes, csTabPermissions, null, csTabPermissions, csTabPermissions);
					SetTabPermissions(Base.salesPerTrans, !isBalanced, null, !isBalanced, !isBalanced);

					break;
				case ARDocType.CashReturn:
					bool isVoidedOrRefunded = tranState.IsRefunded || tranState.IsVoided || CheckLastProcessedTranIsVoided(doc);
					bool isVoidedOrRefundedOrCaptured = isVoidedOrRefunded || tranState.IsCaptured;
					bool isExistActiveApprovalMap = PXSelectReadonly<EP.EPAssignmentMap, Where<EP.EPAssignmentMap.entityType, Equal<EP.AssignmentMapType.AssignmentMapTypeARCashSale>>>.Select(Base).Count != 0 &&
						PXSelectReadonly<ARSetupApproval, Where<ARSetupApproval.docType, Equal<ARDocType.cashReturn>, And<ARSetupApproval.isActive, Equal<True>>>>.Select(Base).Count != 0;
					bool isPendingApproval = doc.Hold == false && doc.Approved == false && doc.DontApprove == false;
					bool isApprovedOrPending = isExistActiveApprovalMap && (isPendingApproval || doc.Approved == true);
					bool isPendingProcessingOrBalanced = doc.PendingProcessing == true || (doc.Voided == false && doc.Hold == false);
					bool isCreatedFromCS = !string.IsNullOrEmpty(doc.OrigRefNbr);

					// Header
					var projDescrEditableCashRet = !((isPendingProcessingOrBalanced && isApprovedOrPending) && doc.Hold == false || isVoidedOrRefunded && doc.Hold == false);
					PXUIFieldAttribute.SetEnabled<ARCashSale.projectID>(cache, doc, projDescrEditableCashRet);
					PXUIFieldAttribute.SetEnabled<ARCashSale.docDesc>(cache, doc, projDescrEditableCashRet);
					SetAdditionalHeaderFields(cache, doc, !((isPendingProcessingOrBalanced && isApprovedOrPending) || (isCreatedFromCS && doc.Hold == true) || (isCreatedFromCS && doc.Hold == false) || isVoidedOrRefunded));
					PXUIFieldAttribute.SetEnabled<ARCashSale.pMInstanceID>(cache, doc, isPendingApproval && !isVoidedOrRefundedOrCaptured);

					// Details
					bool canEditDetails = !((isHoldOrPendingProcessing || doc.Voided == false) && (isVoidedOrRefunded || isExistActiveApprovalMap && (doc.Approved == true || tranState.IsCaptured)));
					SetDetailsTabFields(Base.Transactions.Cache, canEditDetails);
					SetTabPermissions(Base.Transactions, canEditDetails, null, canEditDetails, canEditDetails);
					PXUIFieldAttribute.SetEnabled<ARTran.qty>(Base.Transactions.Cache, null, !((doc.Hold == true || doc.PendingProcessing == true || doc.Voided == false) && isVoidedOrRefunded));

					// Financial
					SetFinancialTabFields(cache, doc, !((isHoldOrPendingProcessing || doc.Voided == false) && isVoidedOrRefunded && isHoldAndCaptured || (isBalanced && tranState.IsRefunded) || isApprovedOrPending));
					SetTaxTabFields(Base.Transactions.Cache, !((isHoldOrPendingProcessing || doc.Voided == false) && isVoidedOrRefunded));

					// Taxes
					bool crTabPermission = !((isHoldOrPendingProcessing || doc.Voided == false) && (isVoidedOrRefunded || isExistActiveApprovalMap && (doc.Approved == true || tranState.IsCaptured)));
					SetTabPermissions(Base.Taxes, crTabPermission, null, crTabPermission, crTabPermission);

					break;
			}			
		}

		protected virtual void SetHeaderFields(PXCache cache, ARCashSale doc, bool value)
		{
			PXUIFieldAttribute.SetEnabled<ARCashSale.curyOrigDocAmt>(cache, doc, value);
			PXUIFieldAttribute.SetEnabled<ARCashSale.curyOrigDiscAmt>(cache, doc, value);
			PXUIFieldAttribute.SetEnabled<ARCashSale.refTranExtNbr>(cache, doc, value);
			PXUIFieldAttribute.SetEnabled<ARCashSale.customerLocationID>(cache, doc, value);
			PXUIFieldAttribute.SetEnabled<ARCashSale.paymentMethodID>(cache, doc, value);
			PXUIFieldAttribute.SetEnabled<ARCashSale.pMInstanceID>(cache, doc, value);
			PXUIFieldAttribute.SetEnabled<ARCashSale.cashAccountID>(cache, doc, value);
			PXUIFieldAttribute.SetEnabled<ARCashSale.taxCalcMode>(cache, doc, value);
			PXUIFieldAttribute.SetEnabled<ARCashSale.processingCenterID>(cache, doc, value);
			PXUIFieldAttribute.SetEnabled<ARCashSale.terminalID>(cache, doc, value);
		}

		protected virtual void SetAdditionalHeaderFields(PXCache cache, ARCashSale doc, bool value)
		{
			PXUIFieldAttribute.SetEnabled<ARCashSale.termsID>(cache, doc, value);
			PXUIFieldAttribute.SetEnabled<ARCashSale.extRefNbr>(cache, doc, value);
		}

		protected virtual void SetDetailsTabFields(PXCache cache, bool value)
		{
			PXUIFieldAttribute.SetEnabled<ARTran.curyUnitPrice>(cache, null, value);
			PXUIFieldAttribute.SetEnabled<ARTran.discPct>(cache, null, value);
			PXUIFieldAttribute.SetEnabled<ARTran.curyDiscAmt>(cache, null, value);
			PXUIFieldAttribute.SetEnabled<ARTran.curyExtPrice>(cache, null, value);
			PXUIFieldAttribute.SetEnabled<ARTran.manualDisc>(cache, null, value);
			PXUIFieldAttribute.SetEnabled<ARTran.taxCategoryID>(cache, null, value);
			PXUIFieldAttribute.SetEnabled<ARTran.inventoryID>(cache, null, value);
			PXUIFieldAttribute.SetEnabled<ARTran.uOM>(cache, null, value);
		}

		protected virtual void SetFinancialTabFields(PXCache cache, ARCashSale doc, bool value)
		{
			PXUIFieldAttribute.SetEnabled<ARCashSale.branchID>(cache, doc, value);
			PXUIFieldAttribute.SetEnabled<ARCashSale.aRAccountID>(cache, doc, value);
			PXUIFieldAttribute.SetEnabled<ARCashSale.aRSubID>(cache, doc, value);
			PXUIFieldAttribute.SetEnabled<ARCashSale.taxZoneID>(cache, doc, value);
			PXUIFieldAttribute.SetEnabled<ARCashSale.externalTaxExemptionNumber>(cache, doc, value);
			PXUIFieldAttribute.SetEnabled<ARCashSale.avalaraCustomerUsageType>(cache, doc, value);
			PXUIFieldAttribute.SetEnabled<ARCashSale.workgroupID>(cache, doc, value);
			PXUIFieldAttribute.SetEnabled<ARCashSale.ownerID>(cache, doc, value);
			PXUIFieldAttribute.SetEnabled<ARCashSale.dontPrint>(cache, doc, value);
			PXUIFieldAttribute.SetEnabled<ARCashSale.dontEmail>(cache, doc, value);
		}

		protected virtual void SetTaxTabFields(PXCache cache, bool value)
		{
			PXUIFieldAttribute.SetEnabled<ARTaxTran.taxID>(cache, null, value);
			PXUIFieldAttribute.SetEnabled<ARTaxTran.taxRate>(cache, null, value);
		}

		protected virtual void SetTabPermissions(PXSelectBase view, bool? allowInsert, bool? allowSelect, bool? allowUpdate, bool? allowDelete)
		{
			if (allowInsert.HasValue)
			{
				view.AllowInsert = allowInsert.Value;
			}
			if (allowSelect.HasValue)
			{
				view.AllowSelect = allowSelect.Value;
			}
			if (allowUpdate.HasValue)
			{
				view.AllowUpdate = allowUpdate.Value;
			}
			if (allowDelete.HasValue)
			{
				view.AllowDelete = allowDelete.Value;
			}
		}

		protected virtual void ProcessingCCSettings(ARCashSale doc, PXCache cache, bool isPMInstanceRequired)
		{
			PXUIFieldAttribute.SetRequired<ARCashSale.pMInstanceID>(cache, isPMInstanceRequired);
			bool isDocumentReleasedOrVoided = doc.Released == true || doc.Voided == true;
			PXUIFieldAttribute.SetEnabled<ARCashSale.pMInstanceID>(cache, doc, isPMInstanceRequired && !isDocumentReleasedOrVoided);
			UIState.RaiseOrHideErrorByErrorLevelPriority<ARPayment.status>(cache, doc, false, Messages.CardProcessingActionsNotAvailable, PXErrorLevel.Warning);
			Base.EnableVoidIfPossible(doc, cache, doc.Status == ARDocStatus.CCHold);
		}

		private void SetActionCaptions()
		{
			bool isEft = IsEft();
			this.voidCCPayment.SetCaption(isEft ? Messages.VoidEftPayment : Messages.VoidCardPayment);
			this.creditCCPayment.SetCaption(isEft ? Messages.RefundEftPayment : Messages.RefundCardPayment);
			this.validateCCPayment.SetCaption(isEft ? Messages.ValidateEftPayment : Messages.ValidateCardPayment);
		}

		private bool IsCCPaymentMethod(ARCashSale doc)
		{
			if (string.IsNullOrEmpty(doc.PaymentMethodID))
				return false;

			PaymentMethod paymentMethod = PaymentMethod.PK.Find(Base, doc.PaymentMethodID);
			return paymentMethod?.PaymentType.IsIn(PaymentMethodType.CreditCard, PaymentMethodType.EFT) == true;
		}

		protected virtual void ARCashSaleRowDeleting(Events.RowDeleting<ARCashSale> e, ARCashSale doc)
		{
			PaymentMethod pm = Base.paymentmethod.Current;
			ExternalTransactionState state = ExternalTranHelper.GetActiveTransactionState(Base, Base.ExternalTran);
			bool cashReturnWithoutRefund = doc.DocType == ARDocType.CashReturn && !state.IsRefunded && !state.IsVoided;
			if (pm?.PaymentType == CA.PaymentMethodType.CreditCard
				&& pm?.ARIsProcessingRequired == true
				&& state?.IsActive == true
				&& !cashReturnWithoutRefund)
			{
				throw new PXException(AR.Messages.CannotDeletedBecauseOfTransactions);
			}
		}

		protected virtual void ShowWarningIfActualFinPeriodClosed(Events.RowSelected<ARCashSale> e, ARCashSale doc)
		{
			if (IsCCPaymentMethod(doc) && string.IsNullOrEmpty(PXUIFieldAttribute.GetError<ARCashSale.paymentMethodID>(e.Cache, doc)))
			{
				RaiseWarning(e.Cache, doc, PXContext.GetBranchID());
				RaiseWarning(e.Cache, doc, doc.BranchID);
			}
		}

		private void RaiseWarning(PXCache cache, ARCashSale doc, int? branchId)
		{
			if (IsActualFinPeriodClosedForBranch(branchId))
			{
				cache.RaiseExceptionHandling<ARCashSale.paymentMethodID>(doc, doc.PaymentMethodID,
				new PXSetPropertyException(Messages.CreditCardProcessingIsDisabled, PXErrorLevel.Warning,
					PXAccess.GetBranch(branchId).Organization.OrganizationCD));
			}
		}

		protected virtual void ShowWarningIfExternalAuthorizationOnly(Events.RowSelected<ARCashSale> e, ARCashSale doc)
		{
			ExternalTransactionState state = GetActiveTransactionState();
			CCProcessingCenter procCenter = CCProcessingCenter.PK.Find(Base, doc.ProcessingCenterID);
			CustomerPaymentMethod cpm = CustomerPaymentMethod.PK.Find(Base, doc.PMInstanceID);

			bool IsExternalAuthorizationOnly = procCenter?.IsExternalAuthorizationOnly == true && !state.IsActive
												&& doc.Status == ARDocStatus.CCHold && doc.DocType == Standalone.ARCashSaleType.CashSale;

			UIState.RaiseOrHideErrorByErrorLevelPriority<ARCashSale.pMInstanceID>(e.Cache, e.Row, IsExternalAuthorizationOnly,
				Messages.CardAssociatedWithExternalAuthorizationOnlyProcessingCenter, PXErrorLevel.Warning, cpm?.Descr, procCenter?.ProcessingCenterID);
		}

		protected virtual void ShowWarningOnProcessingCenterID(Events.RowSelected<ARCashSale> e, ARCashSale doc)
		{
			CCProcessingCenter procCenter = CCProcessingCenter.PK.Find(Base, doc.ProcessingCenterID);

			string errorMessage = string.Empty;
			bool isIncorrect = false;

			if (Base.paymentmethod.Current?.PaymentType == PaymentMethodType.POSTerminal && procCenter?.AcceptPOSPayments != true)
			{
				errorMessage = Messages.ProcessingCenterDoesNotAcceptPOS;
				isIncorrect = true;
			}

			UIState.RaiseOrHideErrorByErrorLevelPriority<ARCashSale.processingCenterID>(e.Cache, e.Row, isIncorrect,
				errorMessage, PXErrorLevel.Warning, procCenter?.ProcessingCenterID);
		}

		protected virtual void FieldUpdated(Events.FieldUpdated<ARCashSale.paymentMethodID> e)
		{
			PXCache cache = e.Cache;
			ARCashSale cashSale = e.Row as ARCashSale;
			if (cashSale == null) return;
			SetPendingProcessingIfNeeded(cache, cashSale);
			cache.SetDefaultExt<ARCashSale.terminalID>(cashSale);
		}

		protected virtual void FieldUpdated(Events.FieldUpdated<ARCashSale.processingCenterID> e)
		{
			var cashSale = e.Row as ARCashSale;
			if (cashSale == null) return;

			e.Cache.SetDefaultExt<ARCashSale.terminalID>(cashSale);
		}

		protected virtual void FieldUpdated(Events.FieldUpdated<ARCashSale.terminalID> e)
		{
			PXCache cache = e.Cache;
			var cashSale = e.Row as ARCashSale;
			if (cashSale == null) return;

			cache.SetValue<ARCashSale.cardPresent>(cashSale, !string.IsNullOrEmpty(cashSale.TerminalID));

			if (!string.IsNullOrEmpty(cashSale.TerminalID) && !string.IsNullOrEmpty(cashSale.RefTranExtNbr))
			{
				cache.SetValueExt<ARCashSale.refTranExtNbr>(cashSale, null);
			}
		}

		protected virtual void FieldUpdated(Events.FieldUpdated<ARCashSale.refTranExtNbr> e)
		{
			var cashSale = e.Row as ARCashSale;
			if (cashSale == null) return;

			if (!string.IsNullOrEmpty(cashSale.RefTranExtNbr) && !string.IsNullOrEmpty(cashSale.TerminalID))
			{
				e.Cache.SetValueExt<ARCashSale.terminalID>(cashSale, null);
			}
		}

		public static bool IsDocTypeSuitableForCC(ARCashSale doc) => doc.DocType.IsIn(ARDocType.CashSale, ARDocType.CashReturn);

		private bool IsEft() => Base.paymentmethod.Current != null && Base.paymentmethod.Current.PaymentType == CA.PaymentMethodType.EFT;

		public static bool IsDocTypePayment(ARCashSale doc)
		{
			bool docTypePayment = doc.DocType == ARDocType.CashSale;
			return docTypePayment;
		}

		public bool EnableCCProcess(ARCashSale doc)
		{
			bool enableCCProcess = false;

			if (doc.IsMigratedRecord != true &&
				Base.paymentmethod.Current != null &&
				Base.paymentmethod.Current.PaymentType.IsIn(CA.PaymentMethodType.CreditCard, CA.PaymentMethodType.EFT, CA.PaymentMethodType.POSTerminal))
			{
				enableCCProcess = IsDocTypeSuitableForCC(doc);
			}
			enableCCProcess &= !doc.Voided.Value;

			enableCCProcess &= !IsProcCenterDisabled(SelectedProcessingCenterType);

			return enableCCProcess &&
				IsFinPeriodValid(PXContext.GetBranchID(), Base.glsetup.Current.RestrictAccessToClosedPeriods) &&
				IsFinPeriodValid(doc.BranchID, Base.glsetup.Current.RestrictAccessToClosedPeriods);
		}

		private bool CanAuthorize(ARCashSale doc, ExternalTransactionState tranState, bool isDocTypePayment)
		{
			if (!EnableCCProcess(doc))
				return false;

			if (IsEft())
				return false;

			if (Base.paymentmethod.Current.PaymentType == PaymentMethodType.POSTerminal && string.IsNullOrEmpty(doc.TerminalID))
				return false;

			return doc.Hold == false && isDocTypePayment && !(tranState.IsPreAuthorized || tranState.IsCaptured) && doc.CuryDocBal > 0;
		}

		private bool CanCapture(ARCashSale doc, ExternalTransactionState tranState, bool isDocTypePayment)
		{
			if (!EnableCCProcess(doc))
				return false;

			if (Base.paymentmethod.Current.PaymentType == PaymentMethodType.POSTerminal && string.IsNullOrEmpty(doc.TerminalID))
				return false;

			return doc.Hold == false && isDocTypePayment && !tranState.IsCaptured && doc.CuryDocBal > 0;
		}

		public bool CanValidate(ARCashSale doc)
		{
			if (!EnableCCProcess(doc))
				return false;

			PXCache cache = Base.Document.Cache;
			bool isDocTypePayment = IsDocTypePayment(doc);
			ExternalTransactionState tranState = GetActiveTransactionState();
			bool canValidate = doc.Hold == false && isDocTypePayment && tranState.IsActive &&
				cache.GetStatus(doc) != PXEntryStatus.Inserted;

			if (!canValidate)
				return false;

			canValidate = CanCapture(doc, tranState, isDocTypePayment) || CanAuthorize(doc, tranState, isDocTypePayment) || tranState.IsOpenForReview
				|| ExternalTranHelper.HasImportedNeedSyncTran(Base, GetExtTrans())
				|| tranState.NeedSync || tranState.IsImportedUnknown || doc.PMInstanceID == PaymentTranExtConstants.NewPaymentProfile;

			if (canValidate && doc.DocType == ARDocType.Refund)
			{
				var sharedTranStatus = ExternalTranHelper.GetSharedTranStatus(Base, GetExtTrans().FirstOrDefault());
				if (sharedTranStatus.IsIn(ExternalTranHelper.SharedTranStatus.ClearState, ExternalTranHelper.SharedTranStatus.Synchronized))
				{
					canValidate = false;
				}
			}

			if (!canValidate)
			{
				var manager = GetAfterProcessingManager(Base);
				canValidate = manager != null && !manager.CheckDocStateConsistency(doc);
			}

			canValidate &= GettingDetailsByTranSupported();

			return canValidate;
		}

		private string GetPaymentStateDescr(ExternalTransactionState state) => GetLastTransactionDescription();

		public override string GetTransactionStateDescription(IExternalTransaction targetTran)
		{
			foreach (var extTran in GetExtTrans())
			{
				if (extTran.TransactionID == targetTran.ParentTranID && extTran.ProcStatus == ExtTransactionProcStatusCode.VoidSuccess)
				{
					return ExternalTranHelper.GetTransactionState(Base, extTran).Description;
				}
			}

			return base.GetTransactionStateDescription(targetTran);
		}

		private bool GettingDetailsByTranSupported()
		{
			CCProcessingCenter procCenter = Base.ProcessingCenter.SelectSingle();
			return CCProcessingFeatureHelper.IsFeatureSupported(procCenter, CCProcessingFeature.TransactionGetter, false);
		}

		protected void SetPendingProcessingIfNeeded(PXCache sender, ARCashSale document)
		{
			PaymentMethod pm = new PXSelect<PaymentMethod, Where<PaymentMethod.paymentMethodID, Equal<Required<PaymentMethod.paymentMethodID>>>>(Base)
				.SelectSingle(document.PaymentMethodID);
			bool pendingProc = CCProcessingHelper.PaymentMethodSupportsIntegratedProcessing(pm) && document.Released == false;
			sender.SetValue<ARRegister.pendingProcessing>(document, pendingProc);
		}

		protected override ARCashSale SetCurrentDocument(ARCashSaleEntry graph, ARCashSale doc)
		{
			var document = graph.Document;
			document.Current = document.Search<ARCashSale.refNbr>(doc.RefNbr, doc.DocType);
			return document.Current;
		}

		protected override PaymentTransactionGraph<ARCashSaleEntry, ARCashSale> GetPaymentTransactionExt(ARCashSaleEntry graph) => graph.GetExtension<ARCashSaleEntryPaymentTransaction>();

		private bool CheckLastProcessedTranIsVoided(ARCashSale cashSale)
		{
			var extTrans = GetExtTrans();
			var externalTran = ExternalTranHelper.GetLastProcessedExtTran(extTrans, GetProcTrans());
			
			var transaction = extTrans.Where(i => i.TransactionID == externalTran.TransactionID).FirstOrDefault();
			return transaction != null ? ExternalTranHelper.GetTransactionState(Base, transaction).IsVoided : false;
		}

		public PXAction<ARCashSale> authorizeCCPayment;

		[PXUIField(DisplayName = "Authorize", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public override IEnumerable AuthorizeCCPayment(PXAdapter adapter)
		{
			if (base.PaymentDoc.Current != null)
			{
				CalcTax(base.PaymentDoc.Current);
				return base.AuthorizeCCPayment(adapter);
			}
			return adapter.Get();
		}

		public PXAction<ARCashSale> captureCCPayment;

		[PXUIField(DisplayName = "Capture", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public override IEnumerable CaptureCCPayment(PXAdapter adapter)
		{
			if (base.PaymentDoc.Current != null)
			{
				CalcTax(base.PaymentDoc.Current);
				return base.CaptureCCPayment(adapter);
			}
			return adapter.Get();
		}

		public virtual void CalcTax(Payment payment)
		{
			ARCashSale currentDocument = Base.CurrentDocument.Current;
			payment.Tax = currentDocument.CuryTaxTotal;
			payment.SubtotalAmount = currentDocument.CuryOrigDocAmt - payment.Tax;
		}

	}
}
