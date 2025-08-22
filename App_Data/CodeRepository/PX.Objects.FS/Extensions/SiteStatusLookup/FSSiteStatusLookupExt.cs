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
using PX.Objects.CM;
using PX.Objects.Extensions.AddItemLookup;
using PX.Objects.SO;
using System.Collections;

namespace PX.Objects.FS.SiteStatusLookup
{
	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	public abstract class FSSiteStatusLookupExt<TGraph, TDocument, TLine> : SiteStatusLookupExt<TGraph, TDocument, TLine, FSSiteStatusSelected, FSSiteStatusFilter>
		where TGraph : PXGraph, ISiteStatusLookupCompatible
		where TDocument : class, IBqlTable, new()
		where TLine : class, IBqlTable, new()
	{
		protected override bool LineShallBeAdded(FSSiteStatusSelected line)
		{
			return line.Selected == true
				&& (line.QtySelected > 0 || line.DurationSelected > 0);
		}

		protected override IEnumerable AddSelectedItemsHandler(PXAdapter adapter)
		{
			var lines = Base.Caches<TLine>();
			lines.ForceExceptionHandling = true;

			foreach (FSSiteStatusSelected line in ItemInfo.Cache.Cached)
			{
				if (LineShallBeAdded(line))
				{
					CreateNewLine(line);
				}
			}

			ItemInfo.Cache.Clear();
			return adapter.Get();
		}

		protected virtual void _(Events.RowSelecting<FSSiteStatusSelected> e)
		{
			if (e.Cache.Fields.Contains(typeof(FSSiteStatusSelected.curyID).Name) &&
					e.Cache.GetValue<FSSiteStatusSelected.curyID>(e.Row) == null)
			{
				PXCache srvOrdCache = e.Cache.Graph.Caches[typeof(FSServiceOrder)];
				e.Cache.SetValue<FSSiteStatusSelected.curyID>(e.Row,
					srvOrdCache.GetValue<FSServiceOrder.curyID>(srvOrdCache.Current));
				e.Cache.SetValue<FSSiteStatusSelected.curyInfoID>(e.Row,
					srvOrdCache.GetValue<FSServiceOrder.curyInfoID>(srvOrdCache.Current));
			}

			if (e.Cache.Graph is RouteServiceContractScheduleEntry
				|| e.Cache.Graph is ServiceContractScheduleEntry)
			{
				Currency currency = CurrencyCollection.GetBaseCurrency();

				if (currency != null)
				{
					e.Cache.SetValue<FSSiteStatusSelected.curyID>(e.Row, currency.CuryID);
					e.Cache.SetValue<FSSiteStatusSelected.curyInfoID>(e.Row, currency.CuryInfoID);
				}
			}
		}

		protected override void _(Events.RowSelected<FSSiteStatusSelected> e)
		{
			base._(e);

			PXUIFieldAttribute.SetEnabled<FSSiteStatusSelected.durationSelected>(e.Cache, e.Row, true);
		}

		protected virtual void _(Events.RowInserted<FSSiteStatusFilter> e)
		{
			if (e.Row == null)
				return;

			bool includeIN = Base.InventoryItemsAreIncluded();

			e.Row.IncludeIN = includeIN;

			if (!includeIN)
				e.Row.OnlyAvailable = false;

			if (e.Row.HistoryDate == null)
				e.Row.HistoryDate = e.Cache.Graph.Accessinfo.BusinessDate.GetValueOrDefault().AddMonths(-3);
		}

		protected override void _(Events.RowSelected<FSSiteStatusFilter> e)
		{
			base._(e);

			if (e.Row == null)
				return;

			PXUIFieldAttribute.SetVisible<FSSiteStatusFilter.historyDate>(e.Cache, null, e.Row.Mode == SOAddItemMode.ByCustomer);

			PXCache status = e.Cache.Graph.Caches[typeof(FSSiteStatusSelected)];
			PXUIFieldAttribute.SetVisible<FSSiteStatusSelected.qtyLastSale>(status, null, e.Row.Mode == SOAddItemMode.ByCustomer);
			PXUIFieldAttribute.SetVisible<FSSiteStatusSelected.curyID>(status, null, e.Row.Mode == SOAddItemMode.ByCustomer);
			PXUIFieldAttribute.SetVisible<FSSiteStatusSelected.curyUnitPrice>(status, null, e.Row.Mode == SOAddItemMode.ByCustomer);
			PXUIFieldAttribute.SetVisible<FSSiteStatusSelected.lastSalesDate>(status, null, e.Row.Mode == SOAddItemMode.ByCustomer);

			bool includeIN = Base.InventoryItemsAreIncluded();
			bool nonStock = e.Row.LineType == ID.LineType_ALL.NONSTOCKITEM || e.Row.LineType == ID.LineType_ALL.SERVICE;

			PXUIFieldAttribute.SetVisible<FSSiteStatusFilter.inventory_Wildcard>(e.Cache, e.Row, includeIN);
			PXUIFieldAttribute.SetVisible<FSSiteStatusFilter.mode>(e.Cache, e.Row, includeIN);
			PXUIFieldAttribute.SetVisible<FSSiteStatusFilter.barCode>(e.Cache, e.Row, includeIN);
			PXUIFieldAttribute.SetVisible<FSSiteStatusFilter.barCodeWildcard>(e.Cache, e.Row, includeIN);
			PXUIFieldAttribute.SetVisible<FSSiteStatusFilter.siteID>(e.Cache, e.Row, includeIN);
			PXUIFieldAttribute.SetVisible<FSSiteStatusFilter.onlyAvailable>(e.Cache, e.Row, includeIN && !nonStock);

			FSLineType.SetLineTypeList<FSSiteStatusFilter.lineType>(e.Cache,
																  e.Row,
																  includeIN,
																  false,
																  false,
																  false,
																  true);
		}
	}
}
