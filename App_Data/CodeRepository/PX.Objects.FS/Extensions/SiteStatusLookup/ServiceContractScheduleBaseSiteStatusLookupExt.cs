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

namespace PX.Objects.FS.SiteStatusLookup
{
	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	public abstract class ServiceContractScheduleBaseSiteStatusLookupExt<TScheduleGraph, TPrimary, TScheduleID, TEntityID, TCustomerID>
		: FSSiteStatusLookupExt<TScheduleGraph, FSSchedule, FSScheduleDet>
		where TScheduleGraph : ServiceContractScheduleEntryBase<TScheduleGraph, TPrimary, TScheduleID, TEntityID, TCustomerID>
		where TPrimary : class, PX.Data.IBqlTable, new()
		where TScheduleID : PX.Data.IBqlField
		where TEntityID : PX.Data.IBqlField
		where TCustomerID : PX.Data.IBqlField
	{
		protected override FSScheduleDet CreateNewLine(FSSiteStatusSelected line)
		{
			InventoryItem inventoryItem =
				PXSelect<InventoryItem,
				Where<
					InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>
				.Select(Base, line.InventoryID);

			FSScheduleDet newline = PXCache<FSScheduleDet>.CreateCopy(Base.ScheduleDetails.Insert(new FSScheduleDet()));
			if (inventoryItem.StkItem == true)
			{
				newline.LineType = ID.LineType_ALL.INVENTORY_ITEM;
			}
			else
			{
				newline.LineType = inventoryItem.ItemType == INItemTypes.ServiceItem ? ID.LineType_ALL.SERVICE : ID.LineType_ALL.NONSTOCKITEM;
			}

			newline.InventoryID = line.InventoryID;
			newline = PXCache<FSScheduleDet>.CreateCopy(Base.ScheduleDetails.Update(newline));

			if (line.BillingRule == ID.BillingRule.TIME)
			{
				newline.EstimatedDuration = line.DurationSelected;
			}
			else
			{
				newline.Qty = line.QtySelected;
			}

			return Base.ScheduleDetails.Update(newline);
		}
	}
}
