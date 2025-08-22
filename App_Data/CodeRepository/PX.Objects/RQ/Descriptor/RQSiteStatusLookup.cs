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
using PX.Objects.IN;
using System;

namespace PX.Objects.RQ
{
	// The RQSiteStatusLookup class has been moved to this file from "WebSites/Pure/PX.Objects/RQ/Descriptor/Attribute.cs".
	// All changes made to this class must be duplicated in the RQSiteStatusLookupBaseExt class.
	[Obsolete("This class is obsolete. Use RQSiteStatusLookupBaseExt instead.")]
	public class RQSiteStatusLookup<Status, StatusFilter> : INSiteStatusLookup<Status, StatusFilter>
		where Status : class, IBqlTable, new()
		where StatusFilter : class, IBqlTable, new()
	{
		#region Ctor
		public RQSiteStatusLookup(PXGraph graph)
			: base(graph)
		{
		}

		public RQSiteStatusLookup(PXGraph graph, Delegate handler)
			: base(graph, handler)
		{
		}

		protected override PXView CreateIntView(PXGraph graph)
		{
			RQRequestClass curClass = graph.Caches[typeof(RQRequestClass)].Current as RQRequestClass;
			if (curClass != null && curClass.RestrictItemList == true)
			{
				return CreateRestictedIntView(graph);
			}
			return base.CreateIntView(graph);
		}

		private static PXView CreateRestictedIntView(PXGraph graph)
		{
			Type inventoryID = GetTypeField<Status>(typeof(INSiteStatusByCostCenter.inventoryID).Name);

			Type join =
				BqlCommand.Compose(
				typeof(InnerJoin<,>),
				typeof(RQRequestClassItem),
				typeof(On<,,>),
						typeof(RQRequestClassItem.inventoryID), typeof(Equal<>), inventoryID,
				typeof(And<RQRequestClassItem.reqClassID, Equal<Current<RQRequestClass.reqClassID>>>));

			Type where = CreateWhere(graph);

			Type selectType =
			BqlCommand.Compose(
				typeof(Select2<,,>),
				typeof(Status),
				join,
				where);

			return new LookupView(graph, BqlCommand.CreateInstance(selectType));
		}

		#endregion
	}
}
