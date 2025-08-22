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
using PX.Data;

using PX.Objects.Common;
using PX.Objects.Common.Bql;

using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.IN;

namespace PX.Objects.SO
{
	/// <exclude />
	[PXCacheName(Messages.InventoryLookupFilter)]
	public partial class SOSiteStatusFilter : INSiteStatusFilter
	{
		#region SiteID
		[PXUIField(DisplayName = "Warehouse")]
		[Site]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public override Int32? SiteID { get; set; }
		public new abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID> { }
		#endregion

		#region Inventory
		public new abstract class inventory : PX.Data.BQL.BqlString.Field<inventory> { }
		#endregion

		#region Mode
		[PXDBInt]
		[PXDefault(0)]
		[PXUIField(DisplayName = "Selection Mode")]
		[SOAddItemMode.List]
		public virtual int? Mode { get; set; }
		public abstract class mode : PX.Data.BQL.BqlInt.Field<mode> { }
		#endregion

		#region HistoryDate
		[PXDBDate]
		[PXUIField(DisplayName = "Sold Since")]
		public virtual DateTime? HistoryDate { get; set; }
		public abstract class historyDate : PX.Data.BQL.BqlDateTime.Field<historyDate> { }
		#endregion

		#region DropShipSales
		[PXDBBool]
		[PXDefault(false)]
		[PXFormula(typeof(Default<mode>))]
		[PXUIField(DisplayName = "Show Drop-Ship Sales")]
		public virtual bool? DropShipSales { get; set; }
		public abstract class dropShipSales : PX.Data.BQL.BqlBool.Field<dropShipSales> { }
		#endregion

		#region Behavior
		[PXDBString(2, IsFixed = true, InputMask = ">aa")]
		public virtual string Behavior { get; set; }
		public abstract class behavior : Data.BQL.BqlString.Field<behavior> { }
		#endregion

		#region CustomerLocationID
		[LocationActive(typeof(Where<Location.bAccountID, Equal<Current<SOOrder.customerID>>,
			And<MatchWithBranch<Location.cBranchID>>>), DescriptionField = typeof(Location.descr))]
		[PXUIField(DisplayName = "Ship-To Location")]
		public virtual int? CustomerLocationID { get; set; }
		public abstract class customerLocationID : Data.BQL.BqlInt.Field<customerLocationID> { }
		#endregion
	}

	public class SOAddItemMode
	{
		public const int BySite = 0;
		public const int ByCustomer = 1;

		public class ListAttribute : PXIntListAttribute
		{
			public ListAttribute() : base(
				new[]
			{
					Pair(BySite, Messages.BySite),
					Pair(ByCustomer, Messages.ByCustomer),
				})
			{ }
		}

		public class bySite : PX.Data.BQL.BqlInt.Constant<bySite> { public bySite() : base(BySite) { } }
		public class byCustomer : PX.Data.BQL.BqlInt.Constant<byCustomer> { public byCustomer() : base(ByCustomer) { } }
	}
}
