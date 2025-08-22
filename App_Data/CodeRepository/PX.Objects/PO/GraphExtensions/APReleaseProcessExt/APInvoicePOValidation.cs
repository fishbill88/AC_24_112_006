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
using PX.Objects.AP;
using System;
using POLineDTO = PX.Objects.PO.GraphExtensions.APInvoiceEntryExt.APInvoicePOValidation.POLineDTO;
using PX.Common;

namespace PX.Objects.PO.GraphExtensions.APReleaseProcessExt
{
	public class APInvoicePOValidation : PXGraphExtension<UpdatePOOnRelease, APReleaseProcess.MultiCurrency, APReleaseProcess>
	{
		public delegate POLine updatePOLineDelegate(APTran tran, APInvoice apdoc, POOrder srcDoc, POLine updLine, bool isPrebooking);

		public PXSelect<POLineBillingRevision> POLineRevision;

		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<CS.FeaturesSet.distributionModule>();
		}

		private APInvoicePOValidationService _validationService;
		public virtual APInvoicePOValidationService GetValidationService(Lazy<POSetup> poSetup)
		{
			if (_validationService == null)
				_validationService = new APInvoicePOValidationService(poSetup);
			return _validationService;
		}
				
		public void DeletePOLineRevision( APInvoice apdoc, POLine srcLine, POOrder srcDoc)
		{
			if (!Base.IsIntegrityCheck)
			{
				POLineDTO poLine = new POLineDTO(srcLine, srcDoc.CuryID);
				DeletePOLineRevision(apdoc, poLine);
			}
		}

		[PXOverride]
		public virtual POLine UpdatePOLine(APTran tran, APInvoice apdoc, POOrder srcDoc, POLine updLine,
			bool isPrebooking, updatePOLineDelegate baseMethod)
		{
			if (!Base.IsIntegrityCheck && !isPrebooking)
			{
				APInvoicePOValidationService validationService = GetValidationService(Lazy.By(() => Base.posetup));
				if (validationService.IsLineValidationRequired(tran))
				{
					var srcLine = POLine.PK.Find(Base, updLine);
					POLineDTO poLine = new POLineDTO(srcLine, srcDoc.CuryID);

					if (validationService.ShouldCreateRevision(Base.Caches[typeof(APTran)], tran, apdoc.CuryID, poLine))
						SavePOLineRevision(apdoc, poLine);
				}
			}

			return baseMethod(tran, apdoc, srcDoc, updLine, isPrebooking);
		}

		/// Overrides <see cref="APReleaseProcess.PerformPersist(PXGraph.IPersistPerformer)"/>
		[PXOverride]
		public void PerformPersist(PXGraph.IPersistPerformer persister)
		{
			persister.Insert(POLineRevision.Cache);
			persister.Delete(POLineRevision.Cache);
		}

		public virtual void SavePOLineRevision(APInvoice apdoc, POLineDTO poLine)
		{
			var revision = new POLineBillingRevision();
			revision.APDocType = apdoc.DocType;
			revision.APRefNbr = apdoc.RefNbr;
			revision.LineType = poLine.LineType;
			revision.OrderType = poLine.OrderType;
			revision.OrderNbr = poLine.OrderNbr;
			revision.OrderLineNbr = poLine.OrderLineNbr;

			// Bill may contain two receipts of the same PO Order.
			if (FindPOLineRevision(revision) == null)
			{
				revision.CuryID = poLine.CuryID;
				revision.InventoryID = poLine.InventoryID;
				revision.UOM = poLine.UOM;
				revision.OrderQty = poLine.OrderQty;
				revision.BaseOrderQty = poLine.BaseOrderQty;
				revision.ReceivedQty = poLine.ReceivedQty;
				revision.BaseReceivedQty = poLine.BaseReceivedQty;
				revision.RcptQtyMax = poLine.RcptQtyMax;
				revision.UnbilledQty = poLine.UnbilledQty;
				revision.BaseUnbilledQty = poLine.BaseUnbilledQty;
				revision.CuryUnbilledAmt = poLine.CuryUnbilledAmt;
				revision.UnbilledAmt = poLine.UnbilledAmt;
				revision.CuryUnitCost = poLine.CuryUnitCost;
				revision.UnitCost = poLine.UnitCost;

				POLineRevision.Insert(revision);
			}
		}

		public virtual void DeletePOLineRevision(APInvoice apdoc, POLineDTO poLine)
		{
			var revision = new POLineBillingRevision();
			revision.APDocType = apdoc.DocType;
			revision.APRefNbr = apdoc.RefNbr;
			revision.OrderType = poLine.OrderType;
			revision.OrderNbr = poLine.OrderNbr;
			revision.OrderLineNbr = poLine.OrderLineNbr;

			var revisionDel = FindPOLineRevision(revision);
			if (revisionDel != null)
				POLineRevision.Delete(revisionDel);
		}

		private POLineBillingRevision FindPOLineRevision(POLineBillingRevision revision)
		{
			return POLineRevision.Search
				<POLineBillingRevision.apDocType, POLineBillingRevision.apRefNbr, POLineBillingRevision.orderType, POLineBillingRevision.orderNbr, POLineBillingRevision.orderLineNbr>
				(revision.APDocType, revision.APRefNbr, revision.OrderType, revision.OrderNbr, revision.OrderLineNbr);
		}

		protected virtual void _(Events.RowPersisting<POLineBillingRevision> e)
		{
			POLineBillingRevision row = e.Row;

			if (row == null) return;

			bool isForInventory = POLineType.IsStock(row.LineType);
			bool isNonStock = POLineType.IsNonStock(row.LineType);
			PXDefaultAttribute.SetPersistingCheck<POLineBillingRevision.uOM>(POLineRevision.Cache, row, isForInventory || isNonStock ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
		}
	}
}
