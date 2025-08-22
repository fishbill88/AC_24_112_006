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
using PX.Objects.PO;
using PX.Objects.Extensions.AddItemLookup;
using PX.Data.BQL.Fluent;

namespace PX.Objects.RQ.GraphExtensions
{
	public abstract class RQSiteStatusLookupBaseExt<TGraph, TDocument, TLine> : SiteStatusLookupExt<TGraph, TDocument, TLine, RQSiteStatusSelected, POSiteStatusFilter>
		where TGraph : PXGraph
		where TDocument : class, IBqlTable, new()
		where TLine : class, IBqlTable, new()
	{
		protected override PXView CreateItemInfoView()
		{
			RQRequestClass curClass = (RQRequestClass) Base.Caches<RQRequestClass>().Current;
			if (curClass != null && curClass.RestrictItemList == true)
			{
				return CreateRestrictedIntView();
			}
			return base.CreateItemInfoView();
		}

		private PXView CreateRestrictedIntView()
		{
			BqlCommand command = new
				SelectFrom<RQSiteStatusSelected>.
				InnerJoin<RQRequestClassItem>.
					On<RQRequestClassItem.inventoryID.IsEqual<RQSiteStatusSelected.inventoryID>.
					And<RQRequestClassItem.reqClassID.IsEqual<RQRequestClass.reqClassID.FromCurrent>>>().
				WhereAnd(CreateWhere());

			return new LookupView(Base, command);
		}
	}
}
