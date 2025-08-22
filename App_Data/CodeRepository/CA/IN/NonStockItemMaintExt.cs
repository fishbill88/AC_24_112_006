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
using PX.Objects.IN;
using PX.Objects.Localizations.CA.IN.DAC;

namespace PX.Objects.Localizations.CA.IN
{
	public class NonStockItemMaintExt : PXGraphExtension<NonStockItemMaint>
	{
		#region IsActive

		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.canadianLocalization>();
		}

		#endregion
		protected virtual void _(Events.FieldUpdated<InventoryItem, InventoryItem.itemType> e)
		{
			if (e.Row == null || e.NewValue == null) return;

			INItemClass itemClass = PXSelect<INItemClass, Where<INItemClass.itemClassID,
				Equal<Required<INItemClass.itemClassID>>>>.Select(Base, e.Row.ItemClassID);
			if (itemClass == null) return;

			var itemClassExtension = PXCache<INItemClass>.GetExtension<INItemClassExt>(itemClass);
			if (itemClassExtension == null) return;

			e.Cache.SetValueExt<InventoryItemExt.t5018Service>(e.Row, itemClassExtension.T5018Service);
		}
	}
}
