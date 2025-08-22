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

using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using PX.Objects.Extensions.AddItemLookup;
using PX.TM;
using System.Collections;
using System.Linq;

namespace PX.Objects.AP.GraphExtensions.APPriceWorksheetMaintExt
{
	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	public class APPriceWorksheetAddItemLookupExt : AddItemLookupExt<APPriceWorksheetMaint, APPriceWorksheet, APAddItemSelected, AddItemFilter, AddItemParameters>
	{
		protected override IEnumerable AddSelectedItemsHandler(PXAdapter adapter)
		{
			if (addItemParameters.Current.VendorID == null)
			{
				PXSetPropertyException exception = new PXSetPropertyException(ErrorMessages.FieldIsEmpty,
																			  PXErrorLevel.Error, $"[{nameof(AddItemParameters.vendorID)}]");
				addItemParameters.Cache.RaiseExceptionHandling<AddItemParameters.vendorID>(addItemParameters.Current,
																						   addItemParameters.Current.VendorID, exception);
				return adapter.Get();
			}

			foreach (APAddItemSelected line in ItemInfo.Cache.Cached)
			{
				if (line.Selected != true)
					continue;

				InsertOrUpdateARAddItem(line);
			}

			ItemFilter.Cache.Clear();
			ItemInfo.Cache.Clear();
			addItemParameters.Cache.Clear();

			return adapter.Get();
		}

		protected override IEnumerable AddAllItemsHandler(PXAdapter adapter)
		{
			if (addItemParameters.Current.VendorID == null)
			{
				PXSetPropertyException exception = new PXSetPropertyException(ErrorMessages.FieldIsEmpty,
																			  PXErrorLevel.Error, $"[{nameof(AddItemParameters.vendorID)}]");
				addItemParameters.Cache.RaiseExceptionHandling<AddItemParameters.vendorID>(addItemParameters.Current,
																						   addItemParameters.Current.VendorID, exception);
				return adapter.Get();
			}

			foreach (APAddItemSelected line in ItemInfo.Select())
			{
				InsertOrUpdateARAddItem(line);
			}

			ItemFilter.Cache.Clear();
			ItemInfo.Cache.Clear();
			addItemParameters.Cache.Clear();

			return adapter.Get();
		}

		protected virtual void InsertOrUpdateARAddItem(APAddItemSelected line)
		{
			int? priceCode = addItemParameters.Current.VendorID;

			var salesPrices = SelectFrom<APVendorPrice>.
				 Where<APVendorPrice.vendorID.IsEqual<@P.AsInt>.
				 And<APVendorPrice.inventoryID.IsEqual<@P.AsInt>>.
				 And<APVendorPrice.curyID.IsEqual<@P.AsString>>.
				 And<APVendorPrice.effectiveDate.IsLessEqual<@P.AsDateTime>.
					And<APVendorPrice.expirationDate.IsNull>.
					Or<APVendorPrice.effectiveDate.IsLessEqual<@P.AsDateTime>.
					And<APVendorPrice.expirationDate.IsGreater<@P.AsDateTime>>>>>.
				 AggregateTo<
								GroupBy<APVendorPrice.vendorID,
								GroupBy<APVendorPrice.inventoryID,
								GroupBy<APVendorPrice.uOM,
								GroupBy<APVendorPrice.breakQty,
								GroupBy<APVendorPrice.curyID>>>>>>.
				View.SelectMultiBound(Base, null, priceCode, line.InventoryID, addItemParameters.Current.CuryID,
					Base.Document.Current.EffectiveDate, Base.Document.Current.EffectiveDate, Base.Document.Current.EffectiveDate).ToList();

			if (salesPrices.Count > 0)
			{
				salesPrices.Select(price => CreateWorksheetDetailFromVendorPriceOnAddSelItems(price))
						   .ForEach(newWorksheetDetail => Base.Details.Update(newWorksheetDetail));
			}
			else
			{
				APPriceWorksheetDetail newWorksheetDetail = CreateWorksheetDetailWhenPriceNotFoundOnAddSelItems(line);
				Base.Details.Update(newWorksheetDetail);
			}
		}

		protected virtual APPriceWorksheetDetail CreateWorksheetDetailFromVendorPriceOnAddSelItems(APVendorPrice salesPrice)
		{
			return new APPriceWorksheetDetail
			{
				VendorID = addItemParameters.Current.VendorID,
				InventoryID = salesPrice.InventoryID,
				SiteID = addItemParameters.Current.SiteID ?? salesPrice.SiteID,
				UOM = salesPrice.UOM,
				BreakQty = salesPrice.BreakQty,
				CurrentPrice = salesPrice.SalesPrice,
				CuryID = addItemParameters.Current.CuryID
			};
		}

		protected virtual APPriceWorksheetDetail CreateWorksheetDetailWhenPriceNotFoundOnAddSelItems(APAddItemSelected line)
		{
			return new APPriceWorksheetDetail
			{
				InventoryID = line.InventoryID,
				SiteID = addItemParameters.Current.SiteID,
				CuryID = addItemParameters.Current.CuryID,
				UOM = line.BaseUnit,
				VendorID = addItemParameters.Current.VendorID,
				CurrentPrice = 0m
			};
		}

		protected override void _(Events.RowSelected<AddItemFilter> e)
		{
			base._(e);
			PXCache status = e.Cache.Graph.Caches<APAddItemSelected>();
			PXUIFieldAttribute.SetVisible<APAddItemSelected.curyID>(status, null, true);
			PXUIFieldAttribute.SetEnabled<AddItemFilter.workGroupID>(e.Cache, e.Row, e.Row?.MyWorkGroup != true);
			PXUIFieldAttribute.SetEnabled<AddItemFilter.ownerID>(e.Cache, e.Row, e.Row?.MyOwner != true);
		}

		protected override PXView CreateItemInfoView()
		{
			var view = base.CreateItemInfoView();
			
			view.WhereAnd<Where<
				Brackets<AddItemFilter.ownerID.FromCurrent.IsNull.
					Or<AddItemFilter.ownerID.FromCurrent.IsEqual<APAddItemSelected.productManagerID>>>.
				And<AddItemFilter.myWorkGroup.FromCurrent.IsEqual<False>.
					Or<APAddItemSelected.productWorkgroupID.Is<IsWorkgroupOfContact<AddItemFilter.currentOwnerID.FromCurrent.Value>>>>.
				And<AddItemFilter.workGroupID.FromCurrent.IsNull.
					Or<AddItemFilter.workGroupID.FromCurrent.IsEqual<APAddItemSelected.productWorkgroupID>>>>>();
			
			return view;
		}

		protected virtual void _(Events.RowSelected<APPriceWorksheet> e)
		{
			if (e.Row == null) return;

			bool allowEdit = e.Row.Status == AR.SPWorksheetStatus.Hold || e.Row.Status == AR.SPWorksheetStatus.Open;
			showItems.SetEnabled(e.Row.Hold == true && allowEdit);
		}

		#region AddItemParameters event handlers
		protected virtual void _(Events.RowSelected<AddItemParameters> e)
		{
			if (e.Row == null) return;
			PXUIFieldAttribute.SetVisible<AddItemParameters.curyID>(e.Cache, e.Row, PXAccess.FeatureInstalled<FeaturesSet.multicurrency>());
		}

		protected virtual void _(Events.RowUpdated<AddItemParameters> e)
		{
			if (e.Row == null) return;

			if (e.Row.VendorID != e.OldRow.VendorID)
			{
				Vendor vendor = SelectFrom<Vendor>.
								Where<Vendor.bAccountID.IsEqual<@P.AsInt>>.
								View.Select(Base, e.Row.VendorID);
				if (vendor != null)
				{
					if (vendor.CuryID != null)
						e.Row.CuryID = vendor.CuryID;
					else
						e.Cache.SetDefaultExt<AddItemParameters.curyID>(e.Row);
				}
			}
		}
		#endregion
	}
}
