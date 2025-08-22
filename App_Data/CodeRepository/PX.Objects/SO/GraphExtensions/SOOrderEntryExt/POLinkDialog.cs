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
using PX.Objects.Common.Extensions;
using System.Collections;
using System.Collections.Generic;

namespace PX.Objects.SO.GraphExtensions.SOOrderEntryExt
{
	public class POLinkDialog : PXGraphExtension<PurchaseSupplyBaseExt, SOOrderEntry>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.sOToPOLink>()
				|| PXAccess.FeatureInstalled<FeaturesSet.dropShipments>()
				|| PXAccess.FeatureInstalled<FeaturesSet.purchaseRequisitions>();
		}

		public PXSelect<SOLine,
			Where<SOLine.orderType, Equal<Optional<SOLine.orderType>>,
				And<SOLine.orderNbr, Equal<Optional<SOLine.orderNbr>>,
					And<SOLine.lineNbr, Equal<Optional<SOLine.lineNbr>>>>>> SOLineDemand;

		[PXCopyPasteHiddenView()]
		public PXSelect<SupplyPOLine> SupplyPOLines;

		public virtual IEnumerable supplyPOLines()
		{
			SOLine currentSOLine = (SOLine)SOLineDemand.Select() ?? Base.Transactions.Current;
			if (currentSOLine == null || currentSOLine.IsLegacyDropShip == true)
				return new List<SupplyPOLine>(0);

			List<SupplyPOLine> supplyPOLines = new List<SupplyPOLine>();
			CollectSupplyPOLines(currentSOLine, supplyPOLines);
			return supplyPOLines;
		}

		public virtual void CollectSupplyPOLines(SOLine currentSOLine, ICollection<SupplyPOLine> supplyPOLines)
		{
		}

		public PXAction<SOOrder> pOSupplyOK;

		[PXUIField(DisplayName = "PO Link", MapViewRights = PXCacheRights.Select, MapEnableRights = PXCacheRights.Select)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntryF, VisibleOnDataSource = false, CommitChanges = true)]
		protected virtual IEnumerable POSupplyOK(PXAdapter adapter)
		{
			SOLine currentSOLine = SOLineDemand.Select();
			if (currentSOLine != null && currentSOLine.POCreate == true &&
				SOLineDemand.AskExt(POSupplyDialogInitializer) == WebDialogResult.OK)
			{
				LinkPOSupply(currentSOLine);
			}

			return adapter.Get();
		}

		public virtual void POSupplyDialogInitializer(PXGraph graph, string viewName)
		{
			foreach (SupplyPOLine supplyLine in SupplyPOLines.Cache.Updated)
			{
				// We should not preserve user input if dialog was closed without saving.
				supplyLine.SelectedSOLines = supplyLine.LinkedSOLines.SparseArrayCopy();
			}
		}

		public virtual void LinkPOSupply(SOLine currentSOLine)
		{
		}
	}
}
