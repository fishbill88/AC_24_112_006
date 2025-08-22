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
using PX.Objects.AP;
using PX.Objects.CA;
using PX.Objects.CM.Extensions;
using PX.Objects.Common.Exceptions;
using PX.Objects.Extensions.MultiCurrency;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PO.LandedCosts;
using PX.Objects.TX;
using System;
using System.Collections.Generic;
using System.Linq;

using Amount = PX.Objects.AR.ARReleaseProcess.Amount;

namespace PX.Objects.PO.GraphExtensions.APReleaseProcessExt
{
	public class UpdatePOOnRelease : BaseUpdatePOAccrual<APReleaseProcess.MultiCurrency, APReleaseProcess, APRegister>
	{
		public static bool IsActive() => true;//PXAccess.FeatureInstalled<FeaturesSet.distributionModule>() || PXAccess.FeatureInstalled<FeaturesSet.construction>();
											  //Disable the extension by the feature settings after move POVendorInventory updating to another graph extension in AC-222872. 

		protected List<APTran> PPVLines { get; set; }
		protected List<AllocationServiceBase.POReceiptLineAdjustment> PPVTransactions { get; set; }

		/// <summary>
		/// Overrides <see cref="Extensions.MultiCurrency.MultiCurrencyGraph{APReleaseProcess, APRegister}.GetTrackedExceptChildren"/>
		/// </summary>
		[PXOverride]
		public virtual PXSelectBase[] GetTrackedExceptChildren(Func<PXSelectBase[]> baseImpl)
		{
			return baseImpl().
				Union(new PXSelectBase[]
					{
						poOrderPrepUpd,
						poOrderLineUPD
					})
				.ToArray();
		}

		#region Views

		public PXSelect<POReceipt,
						Where<
							POReceipt.receiptType, Equal<Required<POReceipt.receiptType>>,
							And<POReceipt.receiptNbr, Equal<Required<POReceipt.receiptNbr>>>>> poReceiptUPD;

		public PXSelectJoin<POReceiptLine,
						LeftJoin<POReceipt,
							On<POReceiptLine.FK.Receipt>>,
						Where<
							POReceiptLine.receiptType, Equal<Required<POReceiptLine.receiptType>>,
							And<POReceiptLine.receiptNbr, Equal<Required<POReceiptLine.receiptNbr>>,
							And<POReceiptLine.lineNbr, Equal<Required<POReceiptLine.lineNbr>>>>>> poReceiptLineUPD;

		public PXSelect<POOrder,
						Where<POOrder.orderType, Equal<Required<POOrder.orderType>>,
							And<POOrder.orderNbr, Equal<Required<POOrder.orderNbr>>>>> poOrderUPD;

		public PXSelectJoin<POLine,
						LeftJoin<POOrder, On<POOrder.orderType, Equal<POLine.orderType>,
							And<POOrder.orderNbr, Equal<POLine.orderNbr>>>>,
						Where<POLine.orderType, Equal<Required<POLine.orderType>>,
							And<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
							And<POLine.lineNbr, Equal<Required<POLine.lineNbr>>>>>> poOrderLineUPD;

		public PXSelect<POOrderPrepayment,
			Where<POOrderPrepayment.aPDocType, Equal<Required<APRegister.docType>>, And<POOrderPrepayment.aPRefNbr, Equal<Required<APRegister.refNbr>>>>>
			poOrderPrepUpd;

		public PXSelect<POItemCostManager.POVendorInventoryPriceUpdate> poVendorInventoryPriceUpdate;

		public PXSelect<POTax> poTaxUpdate;
		public PXSelect<POTaxTran> poTaxTranUpdate;

		public PXSelect<POAccrualStatus> poAccrualUpdate;
		public PXSelect<POAccrualDetail,
				Where<POAccrualDetail.documentNoteID, Equal<Required<POAccrualDetail.documentNoteID>>,
					And<POAccrualDetail.lineNbr, Equal<Required<POAccrualDetail.lineNbr>>>>> poAccrualDetailUpdate;
		public PXSelect<POAccrualSplit> poAccrualSplitUpdate;

		public PXSelect<POLandedCostDetail> landedCostDetails;

		public PXSelect<POAdjust,
			Where<POAdjust.adjgDocType, Equal<Required<APPayment.docType>>,
				And<POAdjust.adjgRefNbr, Equal<Required<APPayment.refNbr>>>>> poAdjustments;

		#endregion

		#region Cache Attached

		[VendorActiveOrHoldPayments(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Vendor.acctName), CacheGlobal = true, Filterable = true, DisplayName = "Vendor ID")]
		[PXDefault(typeof(Vendor.bAccountID))]
		public virtual void POVendorInventoryPriceUpdate_VendorID_CacheAttached(PXCache sender) { }

		#endregion

		/// Overrides <see cref="APReleaseProcess.PerformPersist"/> 
		[PXOverride]
		public void PerformPersist(PXGraph.IPersistPerformer persister)
		{
			persister.Update(poReceiptLineUPD.Cache);
			persister.Update(poReceiptUPD.Cache);
			persister.Update(poOrderLineUPD.Cache);
			persister.Insert(poOrderPrepUpd.Cache);
			persister.Update(poOrderPrepUpd.Cache);
			persister.Insert(poAdjustments.Cache);
			persister.Update(poAdjustments.Cache);
			persister.Update(poOrderUPD.Cache);
			persister.Insert(poVendorInventoryPriceUpdate.Cache);
			persister.Update(poVendorInventoryPriceUpdate.Cache);

			persister.Insert(poAccrualUpdate.Cache);
			persister.Update(poAccrualUpdate.Cache);
			persister.Insert(poAccrualDetailUpdate.Cache);
			persister.Update(poAccrualDetailUpdate.Cache);
			persister.Insert(poAccrualSplitUpdate.Cache);
			persister.Update(poAccrualSplitUpdate.Cache);

			persister.Update(poTaxUpdate.Cache);
			persister.Update(poTaxTranUpdate.Cache);

			persister.Update(landedCostDetails.Cache);
		}

		public delegate List<APRegister> ReleaseInvoiceHandler(
			JournalEntry je,
			ref APRegister doc,
			PXResult<APInvoice, CurrencyInfo, Terms, Vendor> res,
			bool isPrebooking,
			out List<INRegister> inDocs);

		/// <summary>
		/// Overrides <see cref="APReleaseProcess.ReleaseInvoice(JournalEntry, ref APRegister, PXResult{APInvoice, CurrencyInfo, Terms, Vendor}, bool, out List{INRegister})"/>
		/// </summary>
		[PXOverride]
		public virtual List<APRegister> ReleaseInvoice(
			JournalEntry je,
			ref APRegister doc,
			PXResult<APInvoice, CurrencyInfo, Terms, Vendor> res,
			bool isPrebooking,
			out List<INRegister> inDocs, ReleaseInvoiceHandler baseImpl)
		{
			PPVLines = new List<APTran>();
			PPVTransactions = new List<AllocationServiceBase.POReceiptLineAdjustment>();

			return baseImpl(je, ref doc, res, isPrebooking, out inDocs);
		}

		/// <summary>
		/// Extends <see cref="APReleaseProcess.InvoiceTransactionsReleased(InvoiceTransactionsReleasedArgs)"/>
		/// </summary>
		[PXOverride]
		public virtual void InvoiceTransactionsReleased(InvoiceTransactionsReleasedArgs args)
		{
			if(args.Invoice.DocType == APDocType.Prepayment)
			{
				UpdatePOOrderPrepaymentOnRelease(args.Invoice, args.IsPrebooking);
				return;
			}

			if (!PPVTransactions.Any())
				return;

			INRegister adjustment = CreatePPVAdjustment(args.Invoice);

			foreach (APTran n in PPVLines)
			{
				n.PPVDocType = adjustment.DocType;
				n.PPVRefNbr = adjustment.RefNbr;
				Base.APTran_TranType_RefNbr.Update(n);
			}

			var details = PPVLines
			.Select(n => PrepareAPTranAccrualDetail(n, args.Invoice))
			.Where(x => x != null)
			.ToList();

			foreach (var detail in details)
			{
				detail.PPVAdjRefNbr = adjustment.RefNbr;
				detail.PPVAdjPosted = false;
				poAccrualDetailUpdate.Update(detail);

				var status = new POAccrualStatus
				{
					RefNoteID = detail.POAccrualRefNoteID,
					LineNbr = detail.POAccrualLineNbr,
					Type = detail.POAccrualType
				};
				status = poAccrualUpdate.Locate(status);
				if (status != null)
				{
					status.UnreleasedPPVAdjCntr++;
					poAccrualUpdate.Update(status);
				}
			}

			args.INDocuments.Add(adjustment);
		}

		public virtual INRegister CreatePPVAdjustment(APInvoice apdoc)
		{
			INAdjustmentEntry inGraph = PXGraph.CreateInstance<INAdjustmentEntry>();
			inGraph.Clear();

			inGraph.FieldVerifying.AddHandler<INTran.inventoryID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });
			inGraph.FieldVerifying.AddHandler<INTran.origRefNbr>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });

			inGraph.insetup.Current.RequireControlTotal = false;
			inGraph.insetup.Current.HoldEntry = false;

			INRegister newdoc = new INRegister();
			newdoc.DocType = INDocType.Adjustment;
			newdoc.OrigModule = BatchModule.AP;
			newdoc.OrigRefNbr = apdoc.RefNbr;
			newdoc.SiteID = null;
			newdoc.TranDate = apdoc.DocDate;
			newdoc.FinPeriodID = apdoc.FinPeriodID;
			newdoc.BranchID = apdoc.BranchID;
			newdoc.IsPPVTran = true;
			inGraph.adjustment.Insert(newdoc);

			var purchasePriceVarianceINAdjustmentFactory = GetPurchasePriceVarianceINAdjustmentFactory(inGraph);
			purchasePriceVarianceINAdjustmentFactory.CreateAdjustmentTran(PPVTransactions, Base.posetup.PPVReasonCodeID);
			inGraph.Save.Press();

			return inGraph.adjustment.Current;
		}

		public virtual PurchasePriceVarianceINAdjustmentFactory GetPurchasePriceVarianceINAdjustmentFactory(INAdjustmentEntry inGraph)
		{
			return new PurchasePriceVarianceINAdjustmentFactory(inGraph);
		}

		/// <summary>
		/// Extends <see cref="APReleaseProcess.InvoiceTransactionReleasing(InvoiceTransactionReleasingArgs)"/>
		/// </summary>
		[PXOverride]
		public virtual void InvoiceTransactionReleasing(InvoiceTransactionReleasingArgs args)
		{
			PXResult<POLine, POOrder> poRes;
			if (args.Invoice.DocType == APDocType.Prepayment)
			{
				poRes = (PXResult<POLine, POOrder>)poOrderLineUPD.Select(args.Transaction.POOrderType, args.Transaction.PONbr, args.Transaction.POLineNbr);

				if (poRes != null)
				{
					var updLine = (POLine)poRes;

					UpdatePOLine(args.Transaction, args.Invoice, (POOrder)poRes, updLine, args.IsPrebooking);
				}

				return;
			}

			APInvoice apdoc = args.Invoice;
			bool updatePOReceipt = !args.IsPrebooking;
			bool updateVendorPrice = !args.IsPrebooking;

			UnlinkRelatedLandedCosts(args.Register, args.Transaction);
			ApplyLandedCostVariance(args.IsPrebooking, args.IsPrebookVoiding, args.JournalEntry, args.GLTransaction, args.CurrencyInfo, args.Transaction,args.LandedCostCode, args.PostedAmount);

			POAccrualStatus accrual = ApplyPurchasePriceVariance(args.IsPrebooking, args.JournalEntry, args.GLTransaction, args.CurrencyInfo, apdoc, args.Transaction);
			POReceiptLine rctLine = null;
			poRes = (PXResult<POLine, POOrder>)poOrderLineUPD.Select(args.Transaction.POOrderType, args.Transaction.PONbr, args.Transaction.POLineNbr);

			if (poRes != null)
			{
				args.GLTransaction.ReclassificationProhibited |= ((POLine)poRes).CommitmentID.HasValue;
			}

			// TODO: Refactor the calls to POItemCostManager.Update
			if (!Base.IsIntegrityCheck && !Base.IsInvoiceReclassification && updatePOReceipt
				 && (!string.IsNullOrEmpty(args.Transaction.ReceiptNbr) && args.Transaction.ReceiptLineNbr != null
				 || args.Transaction.POAccrualType == POAccrualType.Order && !string.IsNullOrEmpty(args.Transaction.POOrderType) && !string.IsNullOrEmpty(args.Transaction.PONbr) && args.Transaction.POLineNbr != null)
				 && (Base.apsetup?.VendorPriceUpdate == APVendorPriceUpdateType.ReleaseAPBill)
				 && args.Transaction.InventoryID != null && args.Transaction.CuryUnitCost != null)
			{
				int? subItemID = null;
				if (!string.IsNullOrEmpty(args.Transaction.ReceiptNbr) && args.Transaction.ReceiptLineNbr != null)
				{
					rctLine = poReceiptLineUPD.Select(args.Transaction.ReceiptType, args.Transaction.ReceiptNbr, args.Transaction.ReceiptLineNbr);
					subItemID = rctLine?.SubItemID;
				}
				else if (poRes != null)
				{
					subItemID = ((POLine)poRes).SubItemID;
				}
				if (subItemID != null)
				{
					POItemCostManager.Update(Base,
								args.Register.VendorID,
								args.Register.VendorLocationID,
								args.Register.CuryID,
								args.Transaction.InventoryID,
								subItemID,
								args.Transaction.UOM,
								args.Transaction.CuryUnitCost.Value);
				}
			}

			if (!Base.IsIntegrityCheck && !Base.IsInvoiceReclassification && (!PXAccess.FeatureInstalled<FeaturesSet.distributionModule>()
				|| (Base.apsetup?.VendorPriceUpdate == APVendorPriceUpdateType.ReleaseAPBill) || (args.Transaction.PONbr == null && args.Transaction.ReceiptNbr == null)) &&
				args.Transaction.InventoryID != null && args.Transaction.CuryUnitCost != null && updateVendorPrice)
			{
				POItemCostManager.Update(Base,
							args.Register.VendorID,
							args.Register.VendorLocationID,
							args.Register.CuryID,
							args.Transaction.InventoryID,
							null,
							args.Transaction.UOM,
							args.Transaction.CuryUnitCost.Value);
			}

			UpdatePOAccrualStatus(accrual, args.Transaction, apdoc, poRes, poRes, rctLine);
			UpdatePOAccrualDetail(args.Transaction, apdoc);

			if (poRes != null)
			{
				var updLine = (POLine)poRes;
				updLine = UpdatePOLine(args.Transaction, apdoc, (POOrder)poRes, updLine, args.IsPrebooking);
			}
		}

		#region Void POOrder Prepayment Request

		/// <summary>
		/// Overrides <see cref="APReleaseProcess.ProcessPrepaymentRequestApplication(APRegister, APAdjust)"/>
		/// </summary>
		/// <param name="prepaymentRequest"></param>
		/// <param name="prepaymentAdj"></param>
		[PXOverride]
		public virtual void ProcessPrepaymentRequestApplication(APRegister prepaymentRequest, APAdjust prepaymentAdj, Action<APRegister, APAdjust> baseImpl)
		{
			baseImpl(prepaymentRequest, prepaymentAdj);

			if (prepaymentAdj.AdjgDocType == APDocType.VoidCheck ||
				prepaymentAdj.VoidAdjNbr != null && prepaymentAdj.AdjgDocType != APDocType.VoidRefund)
			{
				VoidPOOrderPrepaymentRequest(prepaymentRequest);
			}
		}

		protected virtual void VoidPOOrderPrepaymentRequest(APRegister payRegister)
		{
			if (Base.IsIntegrityCheck || payRegister.DocType != APDocType.Prepayment || payRegister.OrigModule != BatchModule.PO)
				return;

			foreach (PXResult<POLine, APTran> poLineRes in PXSelectJoin<POLine,
				InnerJoin<APTran, On<APTran.pOOrderType, Equal<POLine.orderType>,
					And<APTran.pONbr, Equal<POLine.orderNbr>,
					And<APTran.pOLineNbr, Equal<POLine.lineNbr>>>>>,
				Where<APTran.tranType, Equal<Required<APRegister.docType>>,
					And<APTran.refNbr, Equal<Required<APRegister.refNbr>>>>>
				.Select(Base, payRegister.DocType, payRegister.RefNbr))
			{
				POLine lineCopy = PXCache<POLine>.CreateCopy(poLineRes);
				APTran tran = poLineRes;
				lineCopy.ReqPrepaidQty -= tran.Qty;
				lineCopy.CuryReqPrepaidAmt -= tran.CuryTranAmt + tran.CuryRetainageAmt;
				lineCopy = poOrderLineUPD.Update(lineCopy);
			}

			POOrderPrepayment prepay = poOrderPrepUpd.Select(payRegister.DocType, payRegister.RefNbr);
			if (prepay != null)
			{
				POOrderPrepayment prepayCopy = PXCache<POOrderPrepayment>.CreateCopy(prepay);
				prepayCopy.CuryAppliedAmt = 0m;
				prepayCopy = poOrderPrepUpd.Update(prepayCopy);

				POOrder order = poOrderUPD.Select(prepay.OrderType, prepay.OrderNbr);
				var orderCopy = PXCache<POOrder>.CreateCopy(order);
				orderCopy.CuryPrepaidTotal -= payRegister.CuryOrigDocAmt;
				orderCopy = poOrderUPD.Update(orderCopy);
			}
		}

		#endregion

		#region POAccrualStatus

		public virtual POAccrualStatus UpdatePOAccrualStatus(POAccrualStatus origRow, APTran tran, APInvoice apdoc, POLine poLine, POOrder order, POReceiptLine rctLine)
		{
			if (Base.IsIntegrityCheck || Base.IsInvoiceReclassification || tran.POAccrualType == null)
			{
				return null;
			}

			PXCache cache = poAccrualUpdate.Cache;
			POAccrualStatus row;
			if (origRow == null)
			{
				row = new POAccrualStatus()
				{
					Type = tran.POAccrualType,
					RefNoteID = tran.POAccrualRefNoteID,
					LineNbr = tran.POAccrualLineNbr,
				};

				row = (POAccrualStatus)cache.Insert(row);
			}
			else
			{
				row = (POAccrualStatus)cache.CreateCopy(origRow);
			}

			SetIfNotNull<POAccrualStatus.lineType>(cache, row, tran.LineType);
			SetIfNotNull<POAccrualStatus.orderType>(cache, row, tran.POOrderType);
			SetIfNotNull<POAccrualStatus.orderNbr>(cache, row, tran.PONbr);
			SetIfNotNull<POAccrualStatus.orderLineNbr>(cache, row, tran.POLineNbr);
			SetIfNotNull<POAccrualStatus.receiptType>(cache, row, tran.ReceiptType);
			SetIfNotNull<POAccrualStatus.receiptNbr>(cache, row, tran.ReceiptNbr);

			if (row.MaxFinPeriodID == null || apdoc.FinPeriodID.CompareTo(row.MaxFinPeriodID) > 0)
				SetIfNotNull<POAccrualStatus.maxFinPeriodID>(cache, row, apdoc.FinPeriodID);

			SetIfNotNull<POAccrualStatus.origUOM>(cache, row, tran.UOM);
			SetIfNotNull<POAccrualStatus.origCuryID>(cache, row, order?.CuryID);

			SetIfNotNull<POAccrualStatus.vendorID>(cache, row, order?.VendorID ?? tran.VendorID);
			SetIfNotNull<POAccrualStatus.payToVendorID>(cache, row, order?.PayToVendorID);
			SetIfNotNull<POAccrualStatus.inventoryID>(cache, row, tran.InventoryID);
			SetIfNotNull<POAccrualStatus.subItemID>(cache, row, rctLine?.SubItemID ?? poLine?.SubItemID);
			SetIfNotNull<POAccrualStatus.siteID>(cache, row, rctLine?.SiteID ?? poLine?.SiteID);
			if (POLineType.UsePOAccrual(tran.LineType))
			{
				SetIfNotNull<POAccrualStatus.acctID>(cache, row, tran.AccountID);
				SetIfNotNull<POAccrualStatus.subID>(cache, row, tran.SubID);
			}

			Amount origAccrualAmt = null;
			if (poLine?.OrderNbr != null)
			{
				origAccrualAmt = APReleaseProcess.GetExpensePostingAmount(Base, poLine);
			}
			SetIfNotEmpty<POAccrualStatus.origQty>(cache, row, poLine?.OrderQty);
			SetIfNotEmpty<POAccrualStatus.baseOrigQty>(cache, row, poLine?.BaseOrderQty);
			SetIfNotEmpty<POAccrualStatus.curyOrigAmt>(cache, row, poLine?.CuryExtCost + poLine?.CuryRetainageAmt);
			SetIfNotEmpty<POAccrualStatus.origAmt>(cache, row, poLine?.ExtCost + poLine?.RetainageAmt);
			SetIfNotEmpty<POAccrualStatus.curyOrigCost>(cache, row, origAccrualAmt?.Cury);
			SetIfNotEmpty<POAccrualStatus.origCost>(cache, row, origAccrualAmt?.Base);
			SetIfNotEmpty<POAccrualStatus.curyOrigDiscAmt>(cache, row, poLine?.CuryDiscAmt);
			SetIfNotEmpty<POAccrualStatus.origDiscAmt>(cache, row, poLine?.DiscAmt);

			bool nulloutBilledQty = (origRow != null && (origRow.BilledQty == null || !origRow.BilledUOM.IsIn(null, tran.UOM)));
			row.BilledUOM = nulloutBilledQty ? null : tran.UOM;
			row.BilledQty += nulloutBilledQty ? null : tran.SignedQty;
			row.BaseBilledQty += tran.Sign * tran.BaseQty;

			var billAccrualAmt = APReleaseProcess.GetExpensePostingAmount(Base, tran);
			bool nulloutCuryAmts = (origRow != null && (origRow.CuryBilledCost == null || !origRow.BillCuryID.IsIn(null, apdoc.CuryID)));
			row.BillCuryID = nulloutCuryAmts ? null : apdoc.CuryID;
			row.CuryBilledAmt += nulloutCuryAmts ? null : tran.Sign * (tran.CuryTranAmt + tran.CuryRetainageAmt);
			row.BilledAmt += tran.Sign * (tran.TranAmt + tran.RetainageAmt);
			row.CuryBilledCost += nulloutCuryAmts ? null : tran.Sign * billAccrualAmt.Cury;
			row.BilledCost += tran.Sign * billAccrualAmt.Base;
			row.CuryBilledDiscAmt += nulloutCuryAmts ? null : tran.Sign * tran.CuryDiscAmt;
			row.BilledDiscAmt += tran.Sign * tran.DiscAmt;

			return poAccrualUpdate.Update(row);
		}

		#endregion

		#region POAccrualDetail

		public virtual bool StorePOAccrualDetail(APTran tran)
			=> tran.POAccrualType != null
			&& tran.LineType.IsNotIn(POLineType.Service, POLineType.Freight)
			&& (tran.POOrderType != POOrderType.ProjectDropShip || tran.DropshipExpenseRecording != DropshipExpenseRecordingOption.OnBillRelease);

		public virtual POAccrualDetail FindPOReceiptLineAccrualDetail(POAccrualSplit split)
			=> FindPOReceiptLineAccrualDetail(split.POReceiptType, split.POReceiptNbr, split.POReceiptLineNbr);

		protected virtual POAccrualDetail FindPOReceiptLineAccrualDetail(string receiptType, string receiptNbr, int? lineNbr)
		{
			POAccrualDetail detail = SelectFrom<POAccrualDetail>
				.Where<
					POAccrualDetail.pOReceiptType.IsEqual<@P.AsString.ASCII>
					.And<POAccrualDetail.pOReceiptNbr.IsEqual<@P.AsString>>
					.And<POAccrualDetail.lineNbr.IsEqual<@P.AsInt>>>
				.View
				.SelectWindowed(Base, 0, 1, receiptType, receiptNbr, lineNbr);

			return detail;
		}

		public virtual POAccrualDetail PrepareAPTranAccrualDetail(APTran tran, APRegister doc)
		{
			if (Base.IsIntegrityCheck || !StorePOAccrualDetail(tran))
				return null;

			var detail = new POAccrualDetail
			{
				DocumentNoteID = doc.NoteID,
				LineNbr = tran.LineNbr
			};
			detail = poAccrualDetailUpdate.Locate(detail) ?? poAccrualDetailUpdate.Select(doc.NoteID, tran.LineNbr);
			if (detail == null)
			{
				detail = new POAccrualDetail
				{
					DocumentNoteID = doc.NoteID,
					APDocType = tran.TranType,
					APRefNbr = tran.RefNbr,
					LineNbr = tran.LineNbr,

					POAccrualRefNoteID = tran.POAccrualRefNoteID,
					POAccrualLineNbr = tran.POAccrualLineNbr,
					POAccrualType = tran.POAccrualType,

					VendorID = tran.VendorID,
					IsDropShip = POLineType.IsDropShip(tran.LineType),
					BranchID = tran.BranchID,
					DocDate = tran.TranDate,
					FinPeriodID = tran.FinPeriodID,
					TranDesc = tran.TranDesc,
					UOM = tran.UOM,
					Posted = true
				};
				detail = poAccrualDetailUpdate.Insert(detail);
			}
			return detail;
		}

		protected virtual POAccrualDetail UpdatePOAccrualDetail(APTran tran, APInvoice apdoc)
		{
			POAccrualDetail detail = PrepareAPTranAccrualDetail(tran, apdoc);
			if (detail == null)
				return null;

			detail.AccruedQty += tran.SignedQty;
			detail.BaseAccruedQty += tran.Sign * tran.BaseQty;

			var billAccrualAmt = APReleaseProcess.GetExpensePostingAmount(Base, tran);
			detail.AccruedCost += tran.Sign * billAccrualAmt.Base;
			detail.PPVAmt += tran.POPPVAmt;

			detail.PPVAdjPosted = true;//will be cleared when create PPV adjustment
			detail.TaxAdjPosted = true;//will be cleared when create Tax adjustment

			var reversedDetail = poAccrualDetailUpdate.Cache
				.Updated
				.OfType<POAccrualDetail>()
				.FirstOrDefault(x =>
					x.APDocType == apdoc.OrigDocType
					&& x.APRefNbr == apdoc.OrigRefNbr
					&& x.LineNbr == tran.OrigLineNbr
					&& x.IsReversed == true);
			detail.IsReversing = reversedDetail != null;
			detail.ReversingFinPeriodID = reversedDetail?.FinPeriodID;

			return poAccrualDetailUpdate.Update(detail);
		}

		#endregion

		#region POAccrualSplit

		protected virtual POAccrualSplit InsertPOAccrualSplit(
			POAccrualStatus poAccrual, APTran tran, POReceiptLine rctLine, POAccrualSplit splitToReverse,
			string accruedUom, decimal? accruedQty, decimal? baseAccruedQty, decimal? accruedCost, decimal? ppvAmt)
		{
			if (splitToReverse != null)
				ReversePOAccrual(splitToReverse);

			var poReceipt = new POReceipt
			{
				ReceiptType = rctLine.ReceiptType,
				ReceiptNbr = rctLine.ReceiptNbr
			};
			poReceipt = poReceiptUPD.Locate(poReceipt)
				?? poReceiptUPD.Select(rctLine.ReceiptType, rctLine.ReceiptNbr);

			var poAccrualSplit = new POAccrualSplit()
			{
				RefNoteID = poAccrual.RefNoteID,
				LineNbr = poAccrual.LineNbr,
				Type = poAccrual.Type,
				APDocType = tran.TranType,
				APRefNbr = tran.RefNbr,
				APLineNbr = tran.LineNbr,
				POReceiptType = rctLine.ReceiptType,
				POReceiptNbr = rctLine.ReceiptNbr,
				POReceiptLineNbr = rctLine.LineNbr,
			};
			poAccrualSplit = this.poAccrualSplitUpdate.Insert(poAccrualSplit);

			poAccrualSplit.UOM = accruedUom;
			poAccrualSplit.AccruedQty = accruedQty * GetAPSign(rctLine);
			poAccrualSplit.BaseAccruedQty = baseAccruedQty * GetAPSign(rctLine);
			poAccrualSplit.AccruedCost = accruedCost * GetAPSign(rctLine);
			poAccrualSplit.PPVAmt = ppvAmt;
			poAccrualSplit.FinPeriodID = new string[] { poReceipt.FinPeriodID, tran.FinPeriodID }.Max();

			if (poReceipt.POType == POOrderType.DropShip && rctLine.INReleased == true)
			{
				INTran issueTran = SelectFrom<INTran>
					.Where<INTran.docType.IsEqual<INDocType.issue>
					.And<INTran.pOReceiptType.IsEqual<@P.AsString.ASCII>>
					.And<INTran.pOReceiptNbr.IsEqual<@P.AsString>>
					.And<INTran.pOReceiptLineNbr.IsEqual<@P.AsInt>>>
					.View
					.SelectWindowed(Base, 0, 1, rctLine.ReceiptType, rctLine.ReceiptNbr, rctLine.LineNbr);

				if (issueTran != null && issueTran.FinPeriodID.CompareTo(poAccrualSplit.FinPeriodID) > 0)
					poAccrualSplit.FinPeriodID = issueTran.FinPeriodID;
			}

			return this.poAccrualSplitUpdate.Update(poAccrualSplit);
		}

		#endregion

		#region Apply Purchase Price Variance

		protected virtual POAccrualStatus ApplyPurchasePriceVariance(
			bool isPrebooking, JournalEntry je, GLTran patternTran, CurrencyInfo curyInfo,
			APInvoice apdoc, APTran tran)
		{
			if (Base.IsIntegrityCheck || Base.IsInvoiceReclassification || isPrebooking || tran.POAccrualType == null)
				return null;

			PXResult<POLine, POOrder> poLineRes = null;
			POLine poLine = null;
			PXResult<POReceiptLine, POReceipt> rctLineRes = null;
			switch (tran.POAccrualType)
			{
				case POAccrualType.Order:
					poLineRes = (PXResult<POLine, POOrder>)poOrderLineUPD.Select(tran.POOrderType, tran.PONbr, tran.POLineNbr);
					poLine = (POLine)poLineRes;
					break;
				case POAccrualType.Receipt:
					rctLineRes = (PXResult<POReceiptLine, POReceipt>)poReceiptLineUPD.Select(tran.ReceiptType, tran.ReceiptNbr, tran.ReceiptLineNbr);
					break;
			}

			POAccrualStatus poAccrual = POAccrualStatus.PK.FindDirty(Base, tran.POAccrualRefNoteID, tran.POAccrualLineNbr, tran.POAccrualType);

			if (poAccrual == null)
			{
				// PPV occurs only if items are received before billing, Debit Adjustment can be applied both to Receipt or Return
				tran.UnreceivedQty = tran.Qty;
				return poAccrual;
			}

			var rctLinesToBill = CollectReceiptLinesForBilling(apdoc, tran, poAccrual, poLine, rctLineRes);

			if (tran.POAccrualType == POAccrualType.Receipt)
			{
				var rctLine = rctLinesToBill.First().Item1;
				if (rctLine.TranCostFinal == null)
				{
					if (POLineType.UsePOAccrual(rctLine.LineType))
					{
						// that means that the corresponding IN Issue was not released before the upgrade
						throw new PXException(Messages.INIssueMustReleasedPriorBilling);
					}
				}
				else if (poAccrual.ReceivedCost != GetAPSign(rctLine) * rctLine.TranCostFinal)
				{
					// pass actual cost which can be updated during releasing IN Issue
					poAccrual.ReceivedCost = GetAPSign(rctLine) * rctLine.TranCostFinal;
				}
			}

			decimal sign = tran.Sign; // for the correct application of the APDebitAdjustment to POReceipt
			if (rctLinesToBill.Any(r => GetAPSign(r.Item1) < 0m))
			{
				if (tran.POAccrualType == POAccrualType.Order)
				{
					throw new InvalidOperationException("PO Return cannot be Order-based, it must create its own separate PO Accrual.");
				}
				else
				{
					sign *= decimal.MinusOne; // for the correct application of the APDebitAdjustment to POReturn 
				}
			}

			bool closingPOAccrual;
			bool uomCoincide = rctLinesToBill.All(rl => string.Equals(rl.Item1.UOM, tran.UOM, StringComparison.OrdinalIgnoreCase));
			decimal? accrualBilledQty;
			if (uomCoincide && poAccrual.ReceivedQty != null && poAccrual.ReceivedUOM.IsIn(null, tran.UOM) && poAccrual.BilledQty != null && poAccrual.BilledUOM.IsIn(null, tran.UOM))
			{
				// means that all APTran and POReceiptLine records of the accrual have the same UOM
				// and therefore Qty can be calculated in the documents UOM
				accrualBilledQty = poAccrual.BilledQty + tran.Sign * tran.Qty;
				closingPOAccrual = poAccrual.ReceivedQty == accrualBilledQty;
			}
			else
			{
				accrualBilledQty = poAccrual.BaseBilledQty + tran.Sign * tran.BaseQty;
				closingPOAccrual = poAccrual.BaseReceivedQty == accrualBilledQty;
			}

			decimal? billQty = (uomCoincide ? tran.Qty : tran.BaseQty) * sign;
			decimal? accrualAmt = APReleaseProcess.GetExpensePostingAmount(Base, tran).Base;
			decimal? billAmt = accrualAmt * sign;
			decimal? ppvAmtSum = 0m;

			foreach (var rctLineToBillTuple in rctLinesToBill)
			{
				if (billQty == 0m) break;

				POReceiptLine rctLineToBill = rctLineToBillTuple.Item1;
				POAccrualSplit splitToReverse = rctLineToBillTuple.Item2;

				decimal? rctLineQty = (uomCoincide ? rctLineToBill.ReceiptQty : rctLineToBill.BaseReceiptQty);
				decimal? rctLineBillQty;
				if (splitToReverse != null)
				{
					rctLineBillQty = -(uomCoincide ? splitToReverse.AccruedQty : splitToReverse.BaseAccruedQty);
				}
				else
				{
					decimal? rctLineUnbilledQty = (uomCoincide ? rctLineToBill.UnbilledQty : rctLineToBill.BaseUnbilledQty);
					decimal? rctLineBilledQty = rctLineQty - rctLineUnbilledQty;
					rctLineBillQty = (billQty < 0m)
						? (billQty <= -rctLineBilledQty) ? -rctLineBilledQty : billQty
						: (billQty >= rctLineUnbilledQty) ? rctLineUnbilledQty : billQty;
				}
				if (rctLineBillQty == 0m) continue;

				decimal? rctLineBillAmt = (rctLineBillQty == billQty) ? billAmt
					: CM.PXCurrencyAttribute.BaseRound(Base, billAmt * rctLineBillQty / billQty);

				bool lineClosingPOAccrual = closingPOAccrual && (rctLineBillQty == billQty) && (splitToReverse == null);
				decimal? ppvAmt;
				if (!POLineType.UsePOAccrual(rctLineToBill.LineType) || (POLineType.IsProjectDropShip(tran.LineType) && tran.DropshipExpenseRecording != DropshipExpenseRecordingOption.OnReceiptRelease))
				{
					ppvAmt = 0m;
				}
				else if (splitToReverse != null)
				{
					ppvAmt = -splitToReverse.PPVAmt;
				}
				else if (lineClosingPOAccrual)
				{
					// eliminate rounding difference if po accrual status is closing
					// POAccrualStatus: ReceivedCost = BilledCost + PPVAmt
					ppvAmt = poAccrual.ReceivedCost
						- (poAccrual.BilledCost + tran.Sign * accrualAmt)
						- poAccrual.PPVAmt - ppvAmtSum;
				}
				else
				{
					decimal? origRctTranCost = tran.POAccrualType == POAccrualType.Receipt
						? rctLineToBill.TranCostFinal
						: rctLineToBill.TranCost;
					decimal? origRctUnitCost = origRctTranCost / rctLineQty;
					decimal? origRctAmt = rctLineBillQty * origRctUnitCost;
					origRctAmt = CM.PXCurrencyAttribute.BaseRound(Base, origRctAmt);

					ppvAmt = (origRctAmt - rctLineBillAmt) * GetAPSign(rctLineToBill);
				}

				ApplyPPVAmount(ppvAmt, je, patternTran, tran, rctLineToBill, curyInfo);
				ppvAmtSum += ppvAmt;

				billQty -= rctLineBillQty;
				billAmt -= rctLineBillAmt;

				rctLineToBill.BillPPVAmt += ppvAmt;
				decimal? baseUnbilledQtyDiff;
				if (uomCoincide)
				{
					baseUnbilledQtyDiff = rctLineToBill.BaseUnbilledQty;
					rctLineToBill.UnbilledQty -= rctLineBillQty;
					PXDBQuantityAttribute.CalcBaseQty<POReceiptLine.unbilledQty>(poReceiptLineUPD.Cache, rctLineToBill);
					baseUnbilledQtyDiff -= rctLineToBill.BaseUnbilledQty;
				}
				else
				{
					rctLineToBill.BaseUnbilledQty -= rctLineBillQty;
					PXDBQuantityAttribute.CalcTranQty<POReceiptLine.unbilledQty>(poReceiptLineUPD.Cache, rctLineToBill);
					baseUnbilledQtyDiff = rctLineBillQty;
				}
				UpdatePOReceiptLine(rctLineToBill, tran);

				InsertPOAccrualSplit(
					poAccrual, tran, rctLineToBill, splitToReverse,
					uomCoincide ? tran.UOM : null,
					uomCoincide ? rctLineBillQty : null,
					baseUnbilledQtyDiff,
					rctLineBillAmt,
					ppvAmt);
			}
			if (IsReverseAPTran(apdoc, tran))
			{
				ReverseOrigAPBillPOAccrual(apdoc, tran);
			}
			poAccrual.PPVAmt += ppvAmtSum;
			tran.POPPVAmt = ppvAmtSum;

			if (tran.TranType == APDocType.DebitAdj)
			{
				if (tran.POAccrualType == POAccrualType.Order && accrualBilledQty < 0m)
				{
					if (poLine?.OrderType != POOrderType.RegularSubcontract)
					{
						throw new PXException(AP.Messages.DoublePOOrderBillReverse, tran.LineNbr, tran.PONbr, tran.POLineNbr);
					}
					else
					{
						throw new PXException(AP.Messages.DoubleSubcontractBillReverse, tran.LineNbr, tran.PONbr);
					}
				}

				if (tran.POAccrualType == POAccrualType.Receipt && billQty != 0m)
				{
					string message = billQty > 0m ? AP.Messages.DoublePOReturnBilling : AP.Messages.DoublePOReceiptBillReverse;
					throw new PXException(message, tran.LineNbr, tran.RefNbr, tran.ReceiptNbr);
				}
			}
			else if (tran.POAccrualType == POAccrualType.Receipt && tran.Sign > 0m && billQty > 0)
			{
				throw new PXException(AP.Messages.POReceiptOverBilling, tran.LineNbr, tran.RefNbr, tran.ReceiptNbr);
			}

			if (uomCoincide)
			{
				tran.UnreceivedQty = billQty * sign;
			}
			else
			{
				tran.BaseUnreceivedQty = billQty * sign;
				PXDBQuantityAttribute.CalcTranQty<APTran.unreceivedQty>(Base.Caches[typeof(APTran)], tran);
			}
			return poAccrual;
		}

		protected virtual POReceiptLine UpdatePOReceiptLine(POReceiptLine rctLine, APTran n)
		{
			return poReceiptLineUPD.Update(rctLine);
		}

		protected virtual List<Tuple<POReceiptLine, POAccrualSplit>> CollectReceiptLinesForBilling(APInvoice apdoc, APTran tran,
			POAccrualStatus poAccrual, POLine poLine, POReceiptLine rctLine)
		{
			var rctLinesToBill = new List<Tuple<POReceiptLine, POAccrualSplit>>();
			if (tran.POAccrualType == null)
				return rctLinesToBill;
			if (tran.POAccrualType == POAccrualType.Receipt)
			{
				rctLinesToBill.Add(new Tuple<POReceiptLine, POAccrualSplit>(rctLine, null));
				return rctLinesToBill;
			}

			var rctLines = PXSelect<POReceiptLine,
				Where<POReceiptLine.pOType, Equal<Current<POLine.orderType>>,
					And<POReceiptLine.pONbr, Equal<Current<POLine.orderNbr>>,
					And<POReceiptLine.pOLineNbr, Equal<Current<POLine.lineNbr>>,
					And<POReceiptLine.pOAccrualType, Equal<POAccrualType.order>,
					And<POReceiptLine.released, Equal<True>>>>>>>
				.SelectMultiBound(Base, new object[] { poLine })
				.RowCast<POReceiptLine>();

			if (IsReverseAPTran(apdoc, tran))
			{
				var origAccrualSplits = PXSelectReadonly<POAccrualSplit,
					Where<POAccrualSplit.refNoteID, Equal<Required<POAccrualSplit.refNoteID>>,
						And<POAccrualSplit.lineNbr, Equal<Required<POAccrualSplit.lineNbr>>,
						And<POAccrualSplit.type, Equal<Required<POAccrualSplit.type>>,
						And<POAccrualSplit.aPDocType, Equal<Required<POAccrualSplit.aPDocType>>,
						And<POAccrualSplit.aPRefNbr, Equal<Required<POAccrualSplit.aPRefNbr>>,
						And<POAccrualSplit.aPLineNbr, Equal<Required<POAccrualSplit.aPLineNbr>>,
						And<POAccrualSplit.isReversed, Equal<False>>>>>>>>>
					.Select(Base, poAccrual.RefNoteID, poAccrual.LineNbr, poAccrual.Type, apdoc.OrigDocType, apdoc.OrigRefNbr, tran.OrigLineNbr)
					.RowCast<POAccrualSplit>();
				rctLinesToBill.AddRange(
					rctLines.Select(r => new Tuple<POReceiptLine, POAccrualSplit>(r,
						origAccrualSplits.FirstOrDefault(s => s.POReceiptType == r.ReceiptType && s.POReceiptNbr == r.ReceiptNbr && s.POReceiptLineNbr == r.LineNbr)))
					.Where(r => r.Item2 != null));
			}
			else
			{
				rctLinesToBill.AddRange(
					rctLines.Select(r => new Tuple<POReceiptLine, POAccrualSplit>(r, null)));
			}

			return rctLinesToBill;
		}

		protected virtual bool IsReverseAPTran(APInvoice apdoc, APTran tran) =>
			tran.Sign < 0m
			&& !string.IsNullOrEmpty(apdoc.OrigDocType) && !string.IsNullOrEmpty(apdoc.OrigRefNbr) && tran.OrigLineNbr != null;

		protected virtual void ApplyPPVAmount(decimal? ppvAmt, JournalEntry je, GLTran patternTran, APTran tran, POReceiptLine rctLine, CurrencyInfo curyInfo)
		{
			if ((ppvAmt ?? 0m) == 0m)
				return;
			if (IsInventoryPPV(rctLine))
			{
				POReceiptLine poreceiptline = PXSelect<POReceiptLine,
					Where<POReceiptLine.receiptType, Equal<Required<POReceiptLine.receiptType>>,
						And<POReceiptLine.receiptNbr, Equal<Required<POReceiptLine.receiptNbr>>,
						And<POReceiptLine.lineNbr, Equal<Required<POReceiptLine.lineNbr>>>>>>
					.Select(Base, rctLine.ReceiptType, rctLine.ReceiptNbr, rctLine.LineNbr);
				if (poreceiptline == null) throw new PXException(AP.Messages.CannotFindPOReceipt, rctLine.ReceiptNbr);

				decimal totalToDistribute = -ppvAmt.Value;

				totalToDistribute -= PurchasePriceVarianceAllocationService.Instance.AllocateOverRCTLine(Base, PPVTransactions, poreceiptline, totalToDistribute, tran.BranchID);
				PurchasePriceVarianceAllocationService.Instance.AllocateRestOverRCTLines(PPVTransactions, totalToDistribute);
				if (!PPVLines.Contains(tran))
				{
					PPVLines.Add(tran);
				}
			}
			else
			{
				GLTran poAccrualTran = (GLTran)je.GLTranModuleBatNbr.Cache.CreateCopy(patternTran);
				GLTran ppVTran = (GLTran)je.GLTranModuleBatNbr.Cache.CreateCopy(patternTran);
				int? accountID = null;
				int? subID = null;
				if (Base.posetup.PPVAllocationMode == PPVMode.Inventory && POLineType.IsNonStock(rctLine.LineType) && rctLine.POType != POOrderType.ProjectDropShip)
				{
					accountID = rctLine.ExpenseAcctID;
					subID = rctLine.ExpenseSubID;
				}
				else
				{
					Base.RetrievePPVAccount(je, rctLine, ref accountID, ref subID);
				}
				decimal curyAmt;
				CM.PXCurrencyAttribute.CuryConvCury(je.currencyinfo.Cache, curyInfo, ppvAmt.Value, out curyAmt);

				ppVTran.AccountID = accountID;
				ppVTran.SubID = subID;
				//Type of transaction is already counted in the sign
				poAccrualTran.CuryDebitAmt = curyAmt;
				poAccrualTran.CuryCreditAmt = decimal.Zero;
				poAccrualTran.DebitAmt = ppvAmt;
				poAccrualTran.CreditAmt = decimal.Zero;

				ppVTran.CuryDebitAmt = decimal.Zero;
				ppVTran.CuryCreditAmt = curyAmt;
				ppVTran.DebitAmt = decimal.Zero;
				ppVTran.CreditAmt = ppvAmt;

				SetProjectForPPVTransaction(ppVTran, tran, rctLine);

				poAccrualTran = Base.InsertInvoiceDetailsPOReceiptLineTransaction(je, poAccrualTran,
					new APReleaseProcess.GLTranInsertionContext { APTranRecord = tran, POReceiptLineRecord = rctLine });
				ppVTran = Base.InsertInvoiceDetailsPOReceiptLineTransaction(je, ppVTran,
					new APReleaseProcess.GLTranInsertionContext { APTranRecord = tran, POReceiptLineRecord = rctLine });
			}
		}

		protected virtual void SetProjectForPPVTransaction(GLTran ppvTran, APTran tran, POReceiptLine rctLine)
		{
		}

		protected virtual bool IsInventoryPPV(POReceiptLine rctLine)
		{
			bool res = false;
			if (rctLine.ReceiptType != POReceiptType.POReturn && Base.posetup.PPVAllocationMode == PPVMode.Inventory && rctLine.POType != POOrderType.ProjectDropShip)
			{
				InventoryItem item = PXSelect<InventoryItem,
					Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>
					.Select(Base, rctLine.InventoryID);
				res = (item == null ? false : item.ValMethod != INValMethod.Standard) && !POLineType.IsNonStock(rctLine.LineType);
			}
			return res;
		}

		#endregion

		#region Apply Landed Cost Variance

		protected virtual void UnlinkRelatedLandedCosts(APRegister doc, APTran tran)
		{
			// On Bill reverse should unlink LC lines that are present in original Bill only but not manually added ones.
			if (GetLCOriginalBillLine(doc, tran) == null)
				return;

			var landedCostDetail = GetLandedCostDetail(tran.LCDocType, tran.LCRefNbr, tran.LCLineNbr.Value);

			landedCostDetail.APDocType = null;
			landedCostDetail.APRefNbr = null;

			landedCostDetails.Cache.MarkUpdated(landedCostDetail, assertError: true);
		}

		protected virtual APTran GetLCOriginalBillLine(APRegister doc, APTran tran)
		{
			if (doc.DocType != APDocType.DebitAdj || String.IsNullOrEmpty(doc.OrigRefNbr)
				|| String.IsNullOrEmpty(tran.LCDocType) || String.IsNullOrEmpty(tran.LCRefNbr) || !tran.LCLineNbr.HasValue)
				return null;

			return PXSelectReadonly<APTran,
					Where<APTran.tranType, Equal<Required<APTran.tranType>>,
					And<APTran.refNbr, Equal<Required<APTran.refNbr>>,
					And<APTran.lCRefNbr, IsNotNull>>>>
				.Select(Base, doc.OrigDocType, doc.OrigRefNbr)
				.RowCast<APTran>()
				.FirstOrDefault(ot => ot.LCDocType == tran.LCDocType && ot.LCRefNbr == tran.LCRefNbr && ot.LCLineNbr == tran.LCLineNbr);
		}

		protected virtual POLandedCostDetail GetLandedCostDetail(string docType, string refNbr, int lineNbr)
		{
			var result =
				(POLandedCostDetail)PXSelect<POLandedCostDetail,
					Where<POLandedCostDetail.docType, Equal<Required<POLandedCostDetail.docType>>,
						And<POLandedCostDetail.refNbr, Equal<Required<POLandedCostDetail.refNbr>>,
							And<POLandedCostDetail.lineNbr, Equal<Required<POLandedCostDetail.lineNbr>>
							>>>>.Select(Base, docType, refNbr, lineNbr);

			return result;
		}

		public virtual decimal GetLandedCostAmount(POLandedCostDetail landedCostDetail)
		{
			var taxesQuery = new PXSelectJoin<POLandedCostTax, InnerJoin<Tax, On<POLandedCostTax.taxID, Equal<Tax.taxID>>>,
				Where<POLandedCostTax.docType, Equal<Required<POLandedCostTax.docType>>, And<POLandedCostTax.refNbr, Equal<Required<POLandedCostTax.refNbr>>>>>(Base);

			var taxes = taxesQuery.Select(landedCostDetail.DocType, landedCostDetail.RefNbr).Select(t => (PXResult<POLandedCostTax, Tax>)t).ToList();

			var landedCostTax = taxes
				.Where(t => ((Tax)t).TaxCalcLevel == CSTaxCalcLevel.Inclusive && ((Tax)t).TaxType != CSTaxType.Withholding && ((Tax)t).ReverseTax != true)
				.RowCast<POLandedCostTax>()
				.FirstOrDefault(t => t.LineNbr == landedCostDetail.LineNbr);

			if (landedCostTax != null)
			{
				return landedCostTax.TaxableAmt ?? 0;
			}

			return landedCostDetail.LineAmt ?? 0;
		}

		protected virtual void ApplyLandedCostVariance(
			bool isPrebooking, bool isPrebookingVoiding,
			JournalEntry je, GLTran patternTran, CurrencyInfo curyInfo,
			APTran n, LandedCostCode lcCode, Amount postedAmount)
		{
			if (!String.IsNullOrEmpty(n.LCDocType) && !String.IsNullOrEmpty(n.LCRefNbr) && n.LCLineNbr.HasValue
				&& !isPrebooking && !isPrebookingVoiding)
			{
				APRegister doc = PXSelect<APRegister,
					Where<APRegister.docType, Equal<Required<APRegister.docType>>,
						And<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>>
					.Select(Base, n.TranType, n.RefNbr);
				bool IsLCReverseTran = GetLCOriginalBillLine(doc, n) != null;

				// On Bill reverse we should preserve original sign for correct LCdelta reverse.
				decimal lcMult = n.DrCr == DrCr.Debit || IsLCReverseTran ? 1m : -1m;

				var landedCostDetail = GetLandedCostDetail(n.LCDocType, n.LCRefNbr, n.LCLineNbr.Value);
				var landedCostLineAmt = lcMult * GetLandedCostAmount(landedCostDetail);

				if (postedAmount.Base != landedCostLineAmt)
				{
					decimal LCdelta = (landedCostLineAmt) - (postedAmount.Base ?? 0); //For Debit Adjustment LCAmount and AmountSign are negative
					decimal curyLCdelta;
					CM.PXCurrencyAttribute.CuryConvCury(je.currencyinfo.Cache, curyInfo, LCdelta, out curyLCdelta);
					if (LCdelta != Decimal.Zero || curyLCdelta != decimal.Zero)
					{
						var corrTran1 = (GLTran)je.GLTranModuleBatNbr.Cache.CreateCopy(patternTran);
						var corrTran2 = (GLTran)je.GLTranModuleBatNbr.Cache.CreateCopy(patternTran);
						corrTran1.TranDesc = AP.Messages.LandedCostAccrualCorrection;

						corrTran1.CuryDebitAmt = (n.DrCr == DrCr.Debit) ? curyLCdelta : 0m;
						corrTran1.DebitAmt = (n.DrCr == DrCr.Debit) ? LCdelta : 0m;
						corrTran1.CuryCreditAmt = (n.DrCr == DrCr.Debit) ? 0m : curyLCdelta;
						corrTran1.CreditAmt = (n.DrCr == DrCr.Debit) ? 0m : LCdelta;

						corrTran2.TranDesc = AP.Messages.LandedCostVariance;
						corrTran2.CuryDebitAmt = (n.DrCr == DrCr.Debit) ? 0m : curyLCdelta;
						corrTran2.DebitAmt = (n.DrCr == DrCr.Debit) ? 0m : LCdelta;
						corrTran2.CuryCreditAmt = (n.DrCr == DrCr.Debit) ? curyLCdelta : 0m;
						corrTran2.CreditAmt = (n.DrCr == DrCr.Debit) ? LCdelta : 0m;
						corrTran2.AccountID = lcCode.LCVarianceAcct;
						corrTran2.SubID = lcCode.LCVarianceSub;

						Base.InsertInvoiceDetailsTransaction(je, corrTran1,
							new APReleaseProcess.GLTranInsertionContext { APTranRecord = n });
						Base.InsertInvoiceDetailsTransaction(je, corrTran2,
							new APReleaseProcess.GLTranInsertionContext { APTranRecord = n });
					}
				}
			}
		}

		#endregion

		public virtual POLine UpdatePOLine(APTran n, APInvoice apdoc, POOrder srcDoc, POLine updLine, bool isPrebooking)
		{
			if (Base.IsIntegrityCheck || Base.IsInvoiceReclassification || isPrebooking || n.TranType != APDocType.Prepayment && updLine.POAccrualType == null)
				return updLine;

			updLine = (POLine)poOrderLineUPD.Cache.CreateCopy(updLine);

			decimal qtyDelta = n.Qty ?? 0m;
			if (n.InventoryID != null && !string.IsNullOrEmpty(n.UOM) && !string.IsNullOrEmpty(updLine.UOM) && !string.Equals(n.UOM, updLine.UOM, StringComparison.OrdinalIgnoreCase))
			{
				qtyDelta = INUnitAttribute.ConvertFromBase(poOrderLineUPD.Cache, n.InventoryID, updLine.UOM, n.BaseQty ?? 0m, INPrecision.QUANTITY);
			}

			decimal amtDelta = (n.CuryTranAmt + n.CuryRetainageAmt) ?? 0m;
			if (apdoc.CuryID != srcDoc.CuryID)
			{
				CM.PXCurrencyAttribute.CuryConvCury(poOrderLineUPD.Cache, updLine, (n.TranAmt + n.RetainageAmt) ?? 0m, out amtDelta);
			}

			if (n.TranType == APDocType.Prepayment)
			{
				updLine.ReqPrepaidQty += qtyDelta;
				updLine.CuryReqPrepaidAmt += amtDelta;

				updLine = poOrderLineUPD.Update(updLine);

				bool isNegativeNonStock = POLineType.IsNonStock(updLine.LineType) && updLine.CuryExtCost + updLine.CuryRetainageAmt < 0;
				if (!isNegativeNonStock && updLine.CuryReqPrepaidAmt > updLine.CuryExtCost + updLine.CuryRetainageAmt
									|| isNegativeNonStock && updLine.CuryReqPrepaidAmt < updLine.CuryExtCost + updLine.CuryRetainageAmt)
				{
					throw new PXException(
						!isNegativeNonStock ? Messages.PrepayPOLineMoreThanAllowed : Messages.PrepayPOLineLessThanAllowed,
						updLine.OrderNbr,
						updLine.LineNbr,
						Base.APTran_TranType_RefNbr.Cache.GetValueExt<APTran.inventoryID>(n),
						updLine.CuryExtCost + updLine.CuryRetainageAmt);
				}
			}
			else
			{
				updLine.BilledQty += n.Sign * qtyDelta;
				updLine.CuryBilledAmt += n.Sign * amtDelta;

				var poLineAccrualStatus = GetAccrualStatusSummary(updLine);
				bool closePOLineByQty = (updLine.CompletePOLine == CompletePOLineTypes.Quantity);
				bool needsPOReceipt = !POLineType.IsService(updLine.LineType) && POOrderEntry.NeedsPOReceipt(updLine, false, Base.posetup);
				bool qtyCoincide = (poLineAccrualStatus.BilledUOM != null && poLineAccrualStatus.BilledUOM == poLineAccrualStatus.ReceivedUOM)
					? (poLineAccrualStatus.BilledQty == poLineAccrualStatus.ReceivedQty)
					: (poLineAccrualStatus.BaseBilledQty == poLineAccrualStatus.BaseReceivedQty);
				bool closePOLine = false;
				if (!needsPOReceipt || qtyCoincide && (updLine.Completed == true || !closePOLineByQty))
				{
					bool billedEnough;
					if (closePOLineByQty)
					{
						if (updLine.Completed == true && (n.Sign > 0m || (!POLineType.IsService(updLine.LineType) && !POLineType.IsProjectDropShip(updLine.LineType))))
						{
							billedEnough = true;
						}
						else
						{
							billedEnough = (poLineAccrualStatus.BilledUOM != null && poLineAccrualStatus.BilledUOM == updLine.UOM)
								? (updLine.OrderQty * updLine.RcptQtyThreshold / 100m <= poLineAccrualStatus.BilledQty)
								: (updLine.BaseOrderQty * updLine.RcptQtyThreshold / 100m <= poLineAccrualStatus.BaseBilledQty);
						}
					}
					else
					{
						if (poLineAccrualStatus.BillCuryID != null && poLineAccrualStatus.BillCuryID == srcDoc.CuryID)
						{
							decimal? amountThreshold = (updLine.CuryExtCost + updLine.CuryRetainageAmt) * updLine.RcptQtyThreshold / 100m;
							billedEnough = amountThreshold != null && poLineAccrualStatus.CuryBilledAmt != null
								&& ((Math.Sign(amountThreshold.Value) == 0 && Math.Sign(poLineAccrualStatus.CuryBilledAmt.Value) > 0)
									|| Math.Sign(amountThreshold.Value) == Math.Sign(poLineAccrualStatus.CuryBilledAmt.Value))
								&& Math.Abs(amountThreshold.Value) <= Math.Abs(poLineAccrualStatus.CuryBilledAmt.Value);
						}
						else
						{
							decimal? amountThreshold = (updLine.ExtCost + updLine.RetainageAmt) * updLine.RcptQtyThreshold / 100m;
							billedEnough = amountThreshold != null && poLineAccrualStatus.BilledAmt != null
								&& ((Math.Sign(amountThreshold.Value) == 0 && Math.Sign(poLineAccrualStatus.BilledAmt.Value) > 0)
									|| Math.Sign(amountThreshold.Value) == Math.Sign(poLineAccrualStatus.BilledAmt.Value))
								&& Math.Abs(amountThreshold.Value) <= Math.Abs(poLineAccrualStatus.BilledAmt.Value);
						}
					}
					closePOLine = billedEnough;
				}

				if (closePOLine || (!closePOLine && updLine.Closed == true && n.Sign < 0m))
				{
					updLine.Closed = closePOLine;
					if (closePOLine)
					{
						updLine.Completed = true;
					}
					else if (updLine.OrderType == POOrderType.RegularSubcontract)
					{
						updLine.Completed = false;
					}
				}

				updLine = poOrderLineUPD.Update(updLine);

				if (updLine.POAccrualType == POAccrualType.Order
					&& updLine.RcptQtyAction == POReceiptQtyAction.Reject)
				{
					decimal? maxRcptQty = PXDBQuantityAttribute.Round(updLine.OrderQty * updLine.RcptQtyMax / 100m);
					if (updLine.BilledQty > maxRcptQty)
					{
						throw new PXException(Messages.POLineOverbillingNotAllowed,
							updLine.OrderNbr,
							maxRcptQty,
							updLine.LineNbr,
							n.LineNbr);
					}
				}
			}

			return updLine;
		}

		protected virtual void ReversePOAccrual(POAccrualSplit split)
		{
			split.IsReversed = true;
			split = poAccrualSplitUpdate.Update(split);
		}

		protected virtual void ReverseOrigAPBillPOAccrual(APInvoice apdoc, APTran tran)
		{
			switch (tran.POAccrualType)
			{
				case POAccrualType.Order:
					break;
				case POAccrualType.Receipt:
					var origTran = APTran.PK.Find(Base, apdoc.OrigDocType, apdoc.OrigRefNbr, tran.OrigLineNbr);
					if (tran.Qty != origTran?.Qty || !string.Equals(tran.UOM, origTran?.UOM, StringComparison.OrdinalIgnoreCase)
						|| tran.CuryTranAmt != origTran?.CuryTranAmt)
					{
						return;
					}
					break;
				default:
					return;
			}

			POAccrualDetail detail = SelectFrom<POAccrualDetail>
				.Where<POAccrualDetail.aPDocType.IsEqual<@P.AsString.ASCII>
				.And<POAccrualDetail.aPRefNbr.IsEqual<@P.AsString>>
				.And<POAccrualDetail.lineNbr.IsEqual<@P.AsInt>>>
				.View.SelectWindowed(Base, 0, 1, apdoc.OrigDocType, apdoc.OrigRefNbr, tran.OrigLineNbr);

			if (detail != null)
			{
				detail.IsReversed = true;
				detail.ReversedFinPeriodID = apdoc.FinPeriodID;
				poAccrualDetailUpdate.Update(detail);
			}
		}

		private decimal GetAPSign(POReceiptLine rctLine) => rctLine.InvtMult < 0 ? Decimal.MinusOne : Decimal.One;

		#region Payment & Prepayment

		/// <summary>
		/// Overrides <see cref="APReleaseProcess.ProcessPayment(JournalEntry, APRegister, PXResult{APPayment, CurrencyInfo, Currency, Vendor, CashAccount})"/>
		/// </summary>
		[PXOverride]
		public virtual void ProcessPayment(
			JournalEntry je,
			APRegister doc,
			PXResult<APPayment, CurrencyInfo, Currency, Vendor, CashAccount> res, Action<JournalEntry, APRegister, PXResult<APPayment, CurrencyInfo, Currency, Vendor, CashAccount>> baseImpl)
		{
			baseImpl(je, doc, res);

			ReleasePOAdjustments(res);
		}

		protected virtual void ReleasePOAdjustments(APPayment payment)
		{
			if (payment.DocType == APDocType.Prepayment)
			{
				var poadjustments = poAdjustments.SelectMain(payment.DocType, payment.RefNbr);

				foreach (POAdjust poadjustment in poadjustments)
					ReleasePOAdjustment(payment, poadjustment);
			}
			else if (payment.DocType == APDocType.VoidCheck)
			{
				var poadjustments = poAdjustments.SelectMain(payment.OrigDocType, payment.OrigRefNbr);

				foreach (POAdjust poadjustment in poadjustments)
					VoidPOAdjustment(payment, poadjustment);

				poadjustments = poAdjustments.SelectMain(payment.DocType, payment.RefNbr);

				foreach (POAdjust poadjustment in poadjustments)
				{
					if (poadjustment.Voided != true)
					{
						poadjustment.Released = true;
						poadjustment.Voided = true;
						poAdjustments.Cache.MarkUpdated(poadjustment, assertError: true);
					}
				}
			}
		}

		protected virtual void ReleasePOAdjustment(APPayment payment, POAdjust poadjustment)
		{
			if (poadjustment.IsRequest != true && poadjustment.Released != true)
			{
				var poOrderPrepayment = new PXSelect<POOrderPrepayment,
					Where<POOrderPrepayment.orderType, Equal<Required<POOrderPrepayment.orderType>>,
						And<POOrderPrepayment.orderNbr, Equal<Required<POOrderPrepayment.orderNbr>>,
						And<POOrderPrepayment.aPDocType, Equal<Required<POOrderPrepayment.aPDocType>>,
						And<POOrderPrepayment.aPRefNbr, Equal<Required<POOrderPrepayment.aPRefNbr>>>>>>>(Base)
					.SelectSingle(poadjustment.AdjdOrderType, poadjustment.AdjdOrderNbr, payment.DocType, payment.RefNbr);

				if (poOrderPrepayment == null)
				{
					poOrderPrepayment = poOrderPrepUpd.Insert(new POOrderPrepayment()
					{
						OrderType = poadjustment.AdjdOrderType,
						OrderNbr = poadjustment.AdjdOrderNbr,
						APDocType = payment.DocType,
						APRefNbr = payment.RefNbr,
						IsRequest = false,
						CuryAppliedAmt = 0m,
					});
				}

				poOrderPrepayment.CuryAppliedAmt += poadjustment.CuryAdjdAmt;
				poOrderPrepayment.PayDocType = payment.DocType;
				poOrderPrepayment.PayRefNbr = payment.RefNbr;

				poOrderPrepUpd.Update(poOrderPrepayment);

				POOrder poorder = poOrderUPD.Select(poadjustment.AdjdOrderType, poadjustment.AdjdOrderNbr);

				if (poorder == null)
					throw new RowNotFoundException(poOrderUPD.Cache, poadjustment.AdjdOrderType, poadjustment.AdjdOrderNbr);

				var poorderCopy = PXCache<POOrder>.CreateCopy(poorder);
				poorderCopy.CuryPrepaidTotal += poadjustment.CuryAdjdAmt;
				poorderCopy = poOrderUPD.Update(poorderCopy);

				var curyUnbilledOrderTotal = (poorderCopy.CuryUnbilledLineTotal == poorderCopy.CuryLineTotal) ? poorderCopy.CuryOrderTotal : poorderCopy.CuryUnbilledOrderTotal;
				if (poorderCopy.CuryPrepaidTotal > curyUnbilledOrderTotal)
					throw new PXException(Messages.PrepaidTotalGreaterUnbilledOrderTotalOrOrderTotal, poorderCopy.OrderNbr);
			}

			poadjustment.Released = true;
			poAdjustments.Update(poadjustment);
		}

		protected virtual void VoidPOAdjustment(APPayment payment, POAdjust poadjustment)
		{
			if (poadjustment.IsRequest != true && poadjustment.Released == true && poadjustment.Voided != true)
			{
				var poOrderPrepayment = new PXSelect<POOrderPrepayment,
					Where<POOrderPrepayment.orderType, Equal<Required<POOrderPrepayment.orderType>>,
						And<POOrderPrepayment.orderNbr, Equal<Required<POOrderPrepayment.orderNbr>>,
						And<POOrderPrepayment.aPDocType, Equal<Required<POOrderPrepayment.aPDocType>>,
						And<POOrderPrepayment.aPRefNbr, Equal<Required<POOrderPrepayment.aPRefNbr>>>>>>>(Base)
					.SelectSingle(poadjustment.AdjdOrderType, poadjustment.AdjdOrderNbr, payment.OrigDocType, payment.OrigRefNbr);

				if (poOrderPrepayment == null)
					throw new RowNotFoundException(poOrderPrepUpd.Cache,
						poadjustment.AdjdOrderType, poadjustment.AdjdOrderNbr, payment.OrigDocType, payment.OrigRefNbr);

				poOrderPrepayment.CuryAppliedAmt -= poadjustment.CuryAdjdAmt;
				poOrderPrepUpd.Update(poOrderPrepayment);

				POOrder poorder = poOrderUPD.Select(poadjustment.AdjdOrderType, poadjustment.AdjdOrderNbr);

				if (poorder == null)
					throw new RowNotFoundException(poOrderUPD.Cache, poadjustment.AdjdOrderType, poadjustment.AdjdOrderNbr);

				var poorderCopy = PXCache<POOrder>.CreateCopy(poorder);

				decimal curyAdjdAmtWithoutBilledAmt;

				if (poorder.CuryID == payment.CuryID)
					curyAdjdAmtWithoutBilledAmt = poadjustment.CuryAdjgAmt ?? 0;
				else
					CM.PXCurrencyAttribute.CuryConvCury(poOrderUPD.Cache, poorderCopy, poadjustment.AdjgAmt ?? 0, out curyAdjdAmtWithoutBilledAmt);

				poorderCopy.CuryPrepaidTotal -= curyAdjdAmtWithoutBilledAmt;
				poorderCopy = poOrderUPD.Update(poorderCopy);
			}

			if (poadjustment.Voided != true)
			{
				poadjustment.Voided = true;
				poAdjustments.Cache.MarkUpdated(poadjustment, assertError: true);
			}
		}

		protected virtual void UpdatePOOrderPrepaymentOnRelease(APInvoice apdoc, bool isPrebooking)
		{
			if (Base.IsIntegrityCheck || Base.IsInvoiceReclassification || isPrebooking || apdoc.DocType != APDocType.Prepayment || apdoc.OrigModule != BatchModule.PO)
				return;
			POOrderPrepayment prepay = poOrderPrepUpd.Select(apdoc.DocType, apdoc.RefNbr);
			if (prepay == null) return;

			var prepayCopy = PXCache<POOrderPrepayment>.CreateCopy(prepay);
			prepayCopy.CuryAppliedAmt = apdoc.CuryOrigDocAmt;
			prepayCopy = poOrderPrepUpd.Update(prepayCopy);

			POOrder order = poOrderUPD.Select(prepayCopy.OrderType, prepayCopy.OrderNbr);
			var orderCopy = PXCache<POOrder>.CreateCopy(order);
			orderCopy.CuryPrepaidTotal += apdoc.CuryDocBal;
			orderCopy = poOrderUPD.Update(orderCopy);

			var curyUnbilledOrderTotal = (orderCopy.CuryUnbilledLineTotal == orderCopy.CuryLineTotal) ? orderCopy.CuryOrderTotal : orderCopy.CuryUnbilledOrderTotal;
			if (orderCopy.CuryPrepaidTotal > curyUnbilledOrderTotal)
			{
				throw new PXException(Messages.PrepaidTotalGreaterUnbilledOrderTotalOrOrderTotal, orderCopy.OrderNbr);
			}
		}

		/// <summary>
		/// Overrides <see cref="APReleaseProcess.ProcessPrepaymentRequestAppliedToCheck(APRegister, APAdjust)"/>
		/// </summary>
		[PXOverride]
		public virtual void ProcessPrepaymentRequestAppliedToCheck(APRegister prepaymentRequest, APAdjust prepaymentAdj, Action<APRegister, APAdjust> baseImpl)
		{
			baseImpl(prepaymentRequest, prepaymentAdj);

			if (prepaymentRequest.DocType == APDocType.Prepayment && prepaymentRequest.OrigModule == BatchModule.PO)
			{
				POOrderPrepayment prepay = poOrderPrepUpd.Select(prepaymentRequest.DocType, prepaymentRequest.RefNbr);
				if (prepay != null)
				{
					prepay.PayDocType = prepaymentAdj.AdjgDocType;
					prepay.PayRefNbr = prepaymentAdj.AdjgRefNbr;
					prepay = poOrderPrepUpd.Update(prepay);

					if (prepay.IsRequest == true)
						InsertPOAdjustment(prepaymentRequest, prepaymentAdj, prepay);
				}
			}
		}

		/// <summary>
		/// Overrides <see cref="APReleaseProcess.AdjustmentProcessingOnApplication(APRegister, APAdjust)"/>
		/// </summary>
		[PXOverride]
		public virtual void AdjustmentProcessingOnApplication(APRegister paymentRegister, APAdjust adj, Action<APRegister, APAdjust> baseImpl)
		{
			if (!Base.IsIntegrityCheck && paymentRegister.DocType == APDocType.Prepayment)
			{
				UpdatePOOrderPrepaymentOnApplication(paymentRegister, adj);
			}

			baseImpl(paymentRegister, adj);
		}

		public virtual void UpdatePOOrderPrepaymentOnApplication(APRegister payRegister, APAdjust adj)
		{
			POOrderPrepayment[] prepays = poOrderPrepUpd.SelectMain(payRegister.DocType, payRegister.RefNbr);

			decimal? undistributedAmt = adj.AdjgBalSign * adj.CuryAdjgAmt;

			foreach (POOrderPrepayment prepay in prepays.Where(prepay => prepay.IsRequest == true))
			{
				POOrder order = poOrderUPD.Select(prepay.OrderType, prepay.OrderNbr);
				var orderCopy = PXCache<POOrder>.CreateCopy(order);
				orderCopy.CuryPrepaidTotal -= undistributedAmt;
				orderCopy = poOrderUPD.Update(orderCopy);
			}

			UpdatePrepaymentPOAdjust(adj, undistributedAmt);

			if (adj.AdjdDocType == APDocType.Prepayment || undistributedAmt <= 0m || !prepays.Any(prepay => prepay.IsRequest != true))
				return;

			APTran[] aptranGroupedByOrder = PXSelectGroupBy<APTran,
				Where<APTran.tranType, Equal<Required<APTran.tranType>>, And<APTran.refNbr, Equal<Required<APTran.refNbr>>,
					And<APTran.pOOrderType, IsNotNull, And<APTran.pONbr, IsNotNull>>>>,
				Aggregate<GroupBy<APTran.pOOrderType, GroupBy<APTran.pONbr, Sum<APTran.curyTranAmt>>>>,
				OrderBy<Desc<APTran.curyTranAmt>>>
				.Select(Base, adj.AdjdDocType, adj.AdjdRefNbr)
				.RowCast<APTran>().ToArray();
			if (aptranGroupedByOrder.Length == 0) return;

			POAdjust[] poAdjusts = poAdjustments.Select(payRegister.DocType, payRegister.RefNbr).RowCast<POAdjust>()
				.Where(poAdjust => poAdjust.IsRequest != true).ToArray();

			foreach (APTran aptranGroup in aptranGroupedByOrder)
			{
				foreach (POAdjust poAdjust in poAdjusts.Where(poAdjust
					=> poAdjust.AdjdOrderType == aptranGroup.POOrderType
					&& poAdjust.AdjdOrderNbr == aptranGroup.PONbr))
				{
					decimal? distributedAmt = (poAdjust.CuryAdjgAmt < undistributedAmt) ? poAdjust.CuryAdjgAmt : undistributedAmt;
					if (distributedAmt == 0m) continue;
					POAdjust poAdjustCopy = PXCache<POAdjust>.CreateCopy(poAdjust);
					poAdjustCopy.CuryAdjgAmt -= distributedAmt;
					poAdjustCopy.CuryAdjgBilledAmt += distributedAmt;
					poAdjustments.Update(poAdjustCopy);
					undistributedAmt -= distributedAmt;

					POOrder order = poOrderUPD.Select(poAdjust.AdjdOrderType, poAdjust.AdjdOrderNbr);
					var orderCopy = PXCache<POOrder>.CreateCopy(order);
					if (order.CuryID == payRegister.CuryID)
					{
						orderCopy.CuryPrepaidTotal -= distributedAmt;
					}
					else
					{
						CM.PXCurrencyAttribute.CuryConvBase<POAdjust.adjgCuryInfoID>(poAdjustments.Cache, poAdjust, distributedAmt ?? 0m, out decimal baseCuryAmt);
						CM.PXCurrencyAttribute.CuryConvCury(poOrderUPD.Cache, order, baseCuryAmt, out decimal orderCuryAmt);
						orderCopy.CuryPrepaidTotal -= orderCuryAmt;
					}
					orderCopy = poOrderUPD.Update(orderCopy);
				}
				if (undistributedAmt == 0m) break;
			}
		}

		protected virtual void InsertPOAdjustment(APRegister prepaymentRequest, APAdjust prepaymentAdj, POOrderPrepayment orderPrepayment)
		{
			var poadjustment = GetPrepaymentPOAjust(prepaymentRequest.RefNbr);
			if (poadjustment != null)
				return;

			poadjustment = poAdjustments.Insert(new POAdjust()
			{
				AdjgDocType = prepaymentRequest.DocType,
				AdjgRefNbr = prepaymentRequest.RefNbr,
				AdjdOrderType = orderPrepayment.OrderType,
				AdjdOrderNbr = orderPrepayment.OrderNbr,
				AdjdDocType = prepaymentRequest.DocType,
				AdjdRefNbr = prepaymentRequest.RefNbr,
				AdjNbr = 0,
				IsRequest = true,
			});

			poadjustment.CuryAdjgAmt = prepaymentAdj.CuryAdjdAmt;
			poAdjustments.Update(poadjustment);
		}

		protected virtual POAdjust GetPrepaymentPOAjust(string prepaymentNbr)
		{
			return new PXSelect<POAdjust,
				Where<POAdjust.adjgDocType, Equal<Required<APPayment.docType>>,
					And<POAdjust.adjgRefNbr, Equal<Required<APPayment.refNbr>>,
					And<POAdjust.adjNbr, Equal<int0>,
					And<POAdjust.adjdDocType, Equal<Required<APInvoice.docType>>,
					And<POAdjust.adjdRefNbr, Equal<Required<APInvoice.refNbr>>>>>>>>(Base)
					.Select(APDocType.Prepayment, prepaymentNbr,
						APDocType.Prepayment, prepaymentNbr);
		}

		protected virtual void UpdatePrepaymentPOAdjust(APAdjust adj, decimal? billedAmt)
		{
			if (adj.AdjgDocType == APDocType.Prepayment)
			{
				var prepayment = GetPrepaymentPOAjust(adj.AdjgRefNbr);
				if (prepayment != null)
				{
					POAdjust poAdjustCopy = PXCache<POAdjust>.CreateCopy(prepayment);
					poAdjustCopy.CuryAdjgAmt -= billedAmt;
					poAdjustCopy.CuryAdjgBilledAmt += billedAmt;
					poAdjustments.Update(poAdjustCopy);
				}
			}
		}

		#endregion
	}
}
