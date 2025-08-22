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
using PX.Data.BQL;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.IN.RelatedItems;
using PX.Objects.SO.BQL;
using System;

namespace PX.Objects.SO.GraphExtensions.SOInvoiceEntryExt
{
    public class AddRelatedItemsToInvoice : AddRelatedItemExt<SOInvoiceEntry, SOInvoice, ARTran>
    {
        public static bool IsActive() => PXAccess.FeatureInstalled<FeaturesSet.relatedItems>() && PXAccess.FeatureInstalled<FeaturesSet.advancedSOInvoices>();

        protected override bool SplitSerialTrackingItems => true;

        protected override DateTime? GetDocumentDate(SOInvoice invoice) => Base.Document.Current?.DocDate;

        protected override ARTran FindFocusFor(ARTran line) => Base.Transactions.Search<ARTran.sortOrder>(line.SortOrder + 1);

        protected override RelatedItemHistory FindHistoryLine(int? lineNbr) => RelatedItemsHistory.Search<RelatedItemHistory.relatedInvoiceLineNbr>(lineNbr);

        public override void Initialize()
        {
            base.Initialize();

            RelatedItemsHistory.WhereAnd<Where<
                RelatedItemHistory.invoiceDocType.IsEqual<SOInvoice.docType.FromCurrent>
                .And<RelatedItemHistory.invoiceRefNbr.IsEqual<SOInvoice.refNbr.FromCurrent>>>>();
        }

        #region Fields CacheAttached

        [PXMergeAttributes]
		[PXFormula(typeof(SuggestRelatedItemsIsTrue<SOInvoice.docType, ARInvoice.released.FromCurrent, SOInvoice.customerID>))]
        protected virtual void _(Events.CacheAttached<SOInvoice.suggestRelatedItems> e) { }

        [PXMergeAttributes]
		[PXFormula(typeof(ARTran.lineType.IsNotIn<SOLineType.discount, SOLineType.freight>
			.And<ARTran.sOOrderLineNbr.IsNull>.And<SuggestRelatedItemsIsTrue<ARTran.tranType, ARTran.released, ARTran.customerID>>))]
        protected virtual void ARTran_SuggestRelatedItems_CacheAttached(PXCache cache) { }

        [RelatedItems(typeof(SubstitutableARTran.suggestRelatedItems), typeof(SubstitutableARTran.relatedItemsRelation), typeof(SubstitutableARTran.relatedItemsRequired),
            DocumentDateField = typeof(ARInvoice.docDate))]
        protected virtual void ARTran_RelatedItems_CacheAttached(PXCache cache) { }

        [PXMergeAttributes]
        [PXDefault(typeof(ARInvoice.docType))]
        protected virtual void _(Events.CacheAttached<RelatedItemHistory.invoiceDocType> e) { }

        [PXMergeAttributes]
        [PXRemoveBaseAttribute(typeof(PXSelectorAttribute))]
        [PXDBDefault(typeof(ARInvoice.refNbr))]
        protected virtual void _(Events.CacheAttached<RelatedItemHistory.invoiceRefNbr> e) { }

        #endregion

        protected virtual void _(Events.RowSelected<ARInvoice> e)
        {
            if (e.Row == null)
                return;
            SetRelatedItemsVisible(AllowRelatedItems(Base.SODocument.Select()));
        }

        protected virtual void _(Events.RowUpdated<ARInvoice> e)
        {
            if (!e.Cache.ObjectsEqual<ARInvoice.customerID, ARInvoice.released>(e.Row, e.OldRow))
            {
                SOInvoice invoice = Base.SODocument.Select();
                var suggestRelatedItems = PXFormulaAttribute.Evaluate<SOInvoice.suggestRelatedItems>(Base.SODocument.Cache, invoice);
                Base.SODocument.SetValueExt<SOInvoice.suggestRelatedItems>(invoice, suggestRelatedItems);
            }
            
            if (!e.Cache.ObjectsEqual<ARInvoice.docDate>(e.OldRow, e.Row))
            {
                foreach (var line in Base.Transactions.Select())
                    ResetSubstitutionRequired(line);
            }
        }

        protected override decimal? GetAvailableQty(ARTran line)
        {
            var availability = Base.ItemAvailabilityExt.FetchSite(line, excludeCurrent: true, CostCenter.FreeStock);
            var availableQty = INUnitAttribute.ConvertFromBase(Base.Transactions.Cache, line.InventoryID, line.UOM, availability?.QtyAvail ?? 0, INPrecision.QUANTITY);
            return availableQty;
        }

        protected override void FillRelatedItemHistory(RelatedItemHistory historyLine, RelatedItemsFilter filter, ARTran originalLine, ARTran relatedLine, RelatedItem relatedItem)
        {
            base.FillRelatedItemHistory(historyLine, filter, originalLine, relatedLine, relatedItem);
            historyLine.OriginalInvoiceLineNbr = originalLine.LineNbr;
            historyLine.RelatedInvoiceLineNbr = relatedLine.LineNbr;
        }
	}

    public sealed class SubstitutableARTran : PXCacheExtension<ARTran>, ISubstitutableLineExt
    {
        public static bool IsActive() => AddRelatedItemsToInvoice.IsActive();

        #region SuggestRelatedItems
        [PXBool]
        public bool? SuggestRelatedItems { get; set; }
        public abstract class suggestRelatedItems : BqlBool.Field<suggestRelatedItems> { }
        #endregion

        #region RelatedItems
        [PXString]
        public string RelatedItems { get; set; }
        public abstract class relatedItems : BqlString.Field<relatedItems> { }
        #endregion

        #region RelatedItemsRelation
        [PXInt]
        public int? RelatedItemsRelation { get; set; }
        public abstract class relatedItemsRelation : BqlInt.Field<relatedItemsRelation> { }
        #endregion

        #region RelatedItemsRequired
        [PXInt]
        public int? RelatedItemsRequired { get; set; }
        public abstract class relatedItemsRequired : BqlInt.Field<relatedItemsRequired> { }
        #endregion

        #region HistoryLineID
        [PXInt]
        [PXDBDefault(typeof(RelatedItemHistory.lineID), PersistingCheck = PXPersistingCheck.Nothing)]
        public int? HistoryLineID { get; set; }
        public abstract class historyLineID : BqlInt.Field<historyLineID> { }
        #endregion
    }
}
