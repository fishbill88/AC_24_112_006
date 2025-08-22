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
using PX.Data.BQL;
using PX.Objects.AM.Attributes;
using PX.Objects.CS;
using PX.Objects.IN;

namespace PX.Objects.AM.CacheExtensions
{
	/// <summary>
	///Manufacturing Cache Extension for <see cref="PX.Objects.IN.InventoryItemCurySettings"/>
	/// </summary>
	public sealed class InventoryItemCurySettingsExt : PXCacheExtension<InventoryItemCurySettings>
	{
		public static bool IsActive()
		{
			return Features.ManufacturingOrDRPOrReplenishmentEnabled();
		}
		#region AMSourceSiteID
		public abstract class aMSourceSiteID : BqlInt.Field<aMSourceSiteID> { }

		/// <summary>
		/// Source Warehouse
		/// </summary>
		[AMSite(DisplayName = "Source Warehouse", FieldClass = nameof(FeaturesSet.Warehouse))]
		[PXDefault(typeof(Search<INItemClassExt.aMSourceSiteID, Where<INItemClass.itemClassID, Equal<Current<InventoryItem.itemClassID>>>>), CacheGlobal = true, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIEnabled(typeof(Where<InventoryItem.replenishmentSource.FromCurrent, Equal<INReplenishmentSource.transfer>,
			Or2<Where<InventoryItem.planningMethod.FromCurrent, Equal<INPlanningMethod.mRP>, Or<InventoryItem.planningMethod.FromCurrent, Equal<INPlanningMethod.dRP>>>,
				And<Where<InventoryItem.replenishmentSource.FromCurrent, Equal<INReplenishmentSource.purchased>, Or<InventoryItem.replenishmentSource.FromCurrent, Equal<INReplenishmentSource.purchaseToOrder>>>>>>))]
		public int? AMSourceSiteID { get; set; }
		#endregion
		#region AMScrapSiteID
		public abstract class aMScrapSiteID : PX.Data.BQL.BqlInt.Field<aMScrapSiteID> { }

		/// <summary>
		/// Scrap Warehouse
		/// </summary>
		[PXRestrictor(typeof(Where<INSite.active, Equal<True>>), PX.Objects.IN.Messages.InactiveWarehouse, typeof(INSite.siteCD), CacheGlobal = true)]
		[Site(DisplayName = "Scrap Warehouse", FieldClass = Features.MANUFACTURINGFIELDCLASS)]
		public Int32? AMScrapSiteID { get; set; }
		#endregion
		#region AMScrapLocationID
		public abstract class aMScrapLocationID : PX.Data.BQL.BqlInt.Field<aMScrapLocationID> { }

		/// <summary>
		/// Scrap Location
		/// </summary>
		[Location(typeof(InventoryItemCurySettingsExt.aMScrapSiteID), DisplayName = "Scrap Location", FieldClass = Features.MANUFACTURINGFIELDCLASS)]
		[PXRestrictor(typeof(Where<INLocation.active, Equal<True>>),
			PX.Objects.IN.Messages.InactiveLocation, typeof(INLocation.locationCD), CacheGlobal = true)]
		public Int32? AMScrapLocationID { get; set; }
		#endregion
	}
}
