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
using PX.Data.BQL.Fluent;
using PX.Objects.AP;
using PX.Objects.CN.Common.Services;
using PX.Objects.CN.Subcontracts.AP.CacheExtensions;
using PX.Objects.CS;
using PX.Objects.CT;
using PX.Objects.PO;
using PX.Objects.PO.GraphExtensions.APInvoiceSmartPanel;
using ApMessages = PX.Objects.CN.Subcontracts.AP.Descriptor.Messages;
using PoMessages = PX.Objects.PO.Messages;

namespace PX.Objects.CN.Subcontracts.AP.GraphExtensions
{
    public class ApInvoiceEntryAddSubcontractsExtension : PXGraphExtension<AddPOOrderLineExtension, AddPOOrderExtension,
        APInvoiceEntry>
    {
        public class POOrderTypeListAttribute : PXStringListAttribute
        {
            public POOrderTypeListAttribute() : base(
                new[]
                {
                    Pair(POOrderType.RegularOrder, PoMessages.RegularOrder),
                    Pair(POOrderType.DropShip, PoMessages.DropShip),
                    Pair(POOrderType.Blanket, PoMessages.Blanket),
                    Pair(POOrderType.StandardBlanket, PoMessages.StandardBlanket),
                    Pair(POOrderType.RegularSubcontract, PM.Descriptor.Messages.Subcontract),
                })
            { }
        }

        [PXCopyPasteHiddenView]
        public SelectFrom<POOrderRS>
			.InnerJoin<POLine>.On<POOrderRS.orderNbr.IsEqual<POLine.orderNbr>
				.And<POOrderRS.orderType.IsEqual<POLine.orderType>>>
			.View Subcontracts;

        [PXCopyPasteHiddenView]
        public PXSelect<POLineRS> SubcontractLines;
        public PXSelect<POLine> POLines;

        public PXAction<APInvoice> AddSubcontracts;
        public PXAction<APInvoice> AddSubcontract;
        public PXAction<APInvoice> AddSubcontractLines;
        public PXAction<APInvoice> AddSubcontractLine;

        public static bool IsActive()
        {
            return PXAccess.FeatureInstalled<FeaturesSet.construction>() &&
                !SiteMapExtension.IsTaxBillsAndAdjustmentsScreenId();
        }

        public IEnumerable poOrdersList()
        {
            return Base1.pOOrderslist().Cast<POOrderRS>()
               .Where(po => po.OrderType != POOrderType.RegularSubcontract);
        }

        public IEnumerable poOrderLinesList()
        {
            return Base2.pOOrderLinesList().Cast<POLineRS>()
                 .Where(line => line.OrderType != POOrderType.RegularSubcontract);
        }

        public IEnumerable subcontracts()
        {
            var subcontracts = Base1
				.GetPOOrderList()
				.Where(x => x.GetItem<POOrderRS>().OrderType == POOrderType.RegularSubcontract &&
					x.GetItem<POOrderRS>().Status != POOrderStatus.Completed);
            foreach (var subcontract in subcontracts)
            {
                if (Base.APSetup.Current.RequireSingleProjectPerDocument == false && GetProjectCount(subcontract) > 1)
                {
					PXUIFieldAttribute.SetWarning<POLine.projectID>(POLines.Cache, subcontract.GetItem<POLine>(), PoMessages.SubcontractIncludesDifferentProjects);
                }
                yield return subcontract;
            }
        }

        public IEnumerable subcontractLines()
        {
            var extension = PXCache<POOrderFilter>.GetExtension<PoOrderFilterExt>(Base2.orderfilter.Current);
            var result = Base2.pOOrderLinesList().Cast<POLineRS>()
                .Where(line => line.OrderType == POOrderType.RegularSubcontract &&
					extension.SubcontractNumber.IsIn(null, line.OrderNbr) &&
					line.Status != POOrderStatus.Completed);
            foreach (var line in result)
            {
                line.BilledQty = line.BilledQty ?? 0;
            }
			if (extension.ShowUnbilledLines != true)
			{
				result = result.Where(line => line.BilledQty != 0 || line.BilledAmt != 0);
			}
			return result;
        }

        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [POOrderTypeListAttribute]
        public virtual void _(Events.CacheAttached<APTran.pOOrderType> e) { }

        public virtual void _(Events.RowSelected<APInvoice> args)
        {
            var invoiceState = Base.GetDocumentState(args.Cache, args.Row);

            AddSubcontracts.SetEnabled(Base1.addPOOrder.GetEnabled());
            AddSubcontracts.SetVisible(invoiceState.IsDocumentInvoice || invoiceState.IsDocumentDebitAdjustment);
            AddSubcontractLines.SetEnabled(Base2.addPOOrderLine.GetEnabled());
            AddSubcontractLines.SetVisible(invoiceState.IsDocumentInvoice || invoiceState.IsDocumentDebitAdjustment);

            PXUIFieldAttribute.SetVisible<POLineRS.unbilledQty>(SubcontractLines.Cache, null, invoiceState.IsDocumentInvoice || invoiceState.IsDocumentDebitAdjustment);
            PXUIFieldAttribute.SetVisible<POLineRS.curyUnbilledAmt>(SubcontractLines.Cache, null, invoiceState.IsDocumentInvoice || invoiceState.IsDocumentDebitAdjustment);
            PXUIFieldAttribute.SetVisible<POLineRS.billedQty>(SubcontractLines.Cache, null, invoiceState.IsDocumentDebitAdjustment);
            PXUIFieldAttribute.SetVisible<POLineRS.curyBilledAmt>(SubcontractLines.Cache, null, invoiceState.IsDocumentDebitAdjustment);
       
			PXUIFieldAttribute.SetEnabled<APTran.curyDiscAmt>(Base.Transactions.Cache, null, !invoiceState.IsDocumentDebitAdjustment && !invoiceState.IsDocumentReleasedOrPrebooked);
			PXUIFieldAttribute.SetEnabled<APTran.discPct>(Base.Transactions.Cache, null, !invoiceState.IsDocumentDebitAdjustment && !invoiceState.IsDocumentReleasedOrPrebooked);

            PXUIFieldAttribute.SetVisible<PoOrderFilterExt.showUnbilledLines>(Base2.orderfilter.Cache, Base2.orderfilter.Current, invoiceState.IsDocumentDebitAdjustment);
        }

		public virtual void _(Events.RowSelected<APTran> args)
		{
			if (args.Row == null) return;

			bool isDocumentReleasedOrPrebooked = Base.Document.Current?.Released == true || Base.Document.Current?.Prebooked == true;

			PXUIFieldAttribute.SetEnabled<APTran.curyDiscAmt>(args.Cache, args.Row, Base.Document.Current?.DocType != APDocType.DebitAdj && !isDocumentReleasedOrPrebooked);
			PXUIFieldAttribute.SetEnabled<APTran.discPct>(args.Cache, args.Row, Base.Document.Current?.DocType != APDocType.DebitAdj && !isDocumentReleasedOrPrebooked);
		}

        public virtual void _(Events.FieldDefaulting<POOrderFilter.showBilledLines> e)
        {
            APInvoice doc = Base.Document.Current;
            if (e.Row != null)
            {
                e.NewValue = doc.DocType == APDocType.DebitAdj;
            }
        }

        public virtual void _(Events.FieldDefaulting<PoOrderFilterExt.showUnbilledLines> e)
        {
            APInvoice doc = Base.Document.Current;
            if (e.Row != null)
            {
                e.NewValue = doc.DocType != APDocType.DebitAdj;
            }
        }

        [PXButton]
        [PXUIField(DisplayName = ApMessages.AddSubcontract, FieldClass = ApMessages.FieldClass.Distribution,
            MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [APMigrationModeDependentActionRestriction(
            restrictInMigrationMode: true,
            restrictForRegularDocumentInMigrationMode: true,
            restrictForUnreleasedMigratedDocumentInNormalMode: true)]
        public virtual IEnumerable addSubcontracts(PXAdapter adapter)
        {
            Base.checkTaxCalcMode();
            if (ShouldAddSubcontracts())
            {
                Base.updateTaxCalcMode();
                return addSubcontract(adapter);
            }
            return adapter.Get();
        }

        [PXButton]
        [PXUIField(DisplayName = ApMessages.AddSubcontract,
            MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
        public virtual IEnumerable addSubcontract(PXAdapter adapter)
        {
            return AddLines(Base1.AddPOOrder2, adapter);
        }

        [PXButton]
        [PXUIField(DisplayName = ApMessages.AddSubcontractLine, MapEnableRights = PXCacheRights.Select,
            MapViewRights = PXCacheRights.Select)]
        [APMigrationModeDependentActionRestriction(
            restrictInMigrationMode: true,
            restrictForRegularDocumentInMigrationMode: true,
            restrictForUnreleasedMigratedDocumentInNormalMode: true)]
        public virtual IEnumerable addSubcontractLines(PXAdapter adapter)
        {
            Base.checkTaxCalcMode();
            return ShouldAddSubcontractLines()
                ? addSubcontractLine(adapter)
                : adapter.Get();
        }

        [PXButton]
        [PXUIField(DisplayName = ApMessages.AddSubcontractLine, MapEnableRights = PXCacheRights.Select,
            MapViewRights = PXCacheRights.Select, Visible = false)]
        public virtual IEnumerable addSubcontractLine(PXAdapter adapter)
        {
            return AddLines(Base2.AddPOOrderLine2, adapter);
        }

        private static IEnumerable AddLines(Func<PXAdapter, IEnumerable> addLine, PXAdapter adapter)
        {
            try
            {
                return addLine(adapter);
            }
            catch (PXException exception) when (exception.MessageNoPrefix == PoMessages.FailedToAddLine)
            {
                throw new Exception(ApMessages.FailedToAddSubcontractLinesError);
            }
        }

        private bool ShouldAddSubcontracts()
        {
            return IsAdditionSubcontractsAvailable() &&
                Subcontracts.AskExt(AddSubcontractsPanelInitializeHandler, true).IsPositive();
        }

        private bool ShouldAddSubcontractLines()
        {
            return IsAdditionSubcontractsAvailable() &&
                SubcontractLines.AskExt(AddSubcontractLinesPanelInitializeHandler, true).IsPositive();
        }

        [PXOverride]
        public virtual bool ShouldAddPOOrder()
        {
            return IsAdditionSubcontractsAvailable();
        }

        [PXOverride]
        public virtual bool ShouldAddPOOrderLine()
        {
            return IsAdditionSubcontractsAvailable();
        }

        private bool IsAdditionSubcontractsAvailable()
        {
            return Base.Document.Current != null &&
                (Base.Document.Current.DocType == APDocType.Invoice || Base.Document.Current.DocType == APDocType.DebitAdj) &&
                Base.Document.Current.Released == false &&
                Base.Document.Current.Prebooked == false;
        }

        private void AddSubcontractLinesPanelInitializeHandler(PXGraph graph, string view)
        {
            ClearViewCache(Base2.orderfilter);
            ClearViewCache(SubcontractLines);
        }

        private void AddSubcontractsPanelInitializeHandler(PXGraph graph, string view)
        {
            var linkLineExtension = Base.GetExtension<LinkLineExtension>();
            ClearViewCache(linkLineExtension.filter);
            ClearViewCache(Subcontracts);
        }

        private static void ClearViewCache(PXSelectBase selectBase)
        {
            selectBase.Cache.ClearQueryCache();
            selectBase.View.Clear();
            selectBase.Cache.Clear();
        }

        private int GetProjectCount(POOrder subcontract)
        {
            var query = new PXSelectJoin<Contract,
                LeftJoin<POLine, On<POLine.projectID, Equal<Contract.contractID>>>,
                Where<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
                    And<POLine.orderType, Equal<POOrderType.regularSubcontract>>>,
                OrderBy<Asc<POLine.lineNbr>>>(Base);

            int projectCount = query.Select<Contract>(subcontract.OrderNbr).Distinct(x => x.ContractID).Count();
            return projectCount;
        }
    }
}
