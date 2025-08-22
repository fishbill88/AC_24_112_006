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
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR.Repositories;
using PX.Objects.CS;
using PX.Objects.Extensions.AddItemLookup;
using PX.Objects.PM;
using PX.TM;
using System;
using System.Collections;

namespace PX.Objects.AR.GraphExtensions.ARPriceWorksheetMaintExt
{
	[PXProtectedAccess(typeof(ARPriceWorksheetMaint))]
	public abstract class ARPriceWorksheetAddItemLookupExt : AddItemLookupExt<ARPriceWorksheetMaint, ARPriceWorksheet, ARAddItemSelected, AddItemFilter, AddItemParameters>
	{
		protected override IEnumerable AddSelectedItemsHandler(PXAdapter adapter)
		{
			if ((addItemParameters.Current.PriceType != PriceTypes.BasePrice && addItemParameters.Current.PriceCode != null) ||
							(addItemParameters.Current.PriceType == PriceTypes.BasePrice && addItemParameters.Current.PriceCode == null))
			{
				foreach (ARAddItemSelected line in ItemInfo.Cache.Cached)
				{
					if (line.Selected != true)
						continue;

					ARPriceWorksheetDetail newDetail = CreateWorksheetDetailOnAddSelItems(line);
					Base.Details.Update(newDetail);
				}

				ItemFilter.Cache.Clear();
				ItemInfo.Cache.Clear();
				addItemParameters.Cache.Clear();
			}
			else if (string.IsNullOrEmpty(addItemParameters.Current.PriceCode))
			{
				var exception = new PXSetPropertyException(ErrorMessages.FieldIsEmpty, PXErrorLevel.Error, $"[{nameof(AddItemParameters.priceCode)}]");
				addItemParameters.Cache.RaiseExceptionHandling<AddItemParameters.priceCode>(addItemParameters.Current,
																							addItemParameters.Current.PriceCode,
																						exception);
			}

			return adapter.Get();
		}

		protected override IEnumerable AddAllItemsHandler(PXAdapter adapter)
		{
			if ((addItemParameters.Current.PriceType != PriceTypes.BasePrice && addItemParameters.Current.PriceCode != null) ||
				(addItemParameters.Current.PriceType == PriceTypes.BasePrice && addItemParameters.Current.PriceCode == null))
			{
				foreach (ARAddItemSelected line in ItemInfo.Select())
				{
					if (line.InventoryID != (PMInventorySelectorAttribute.EmptyInventoryID))
					{
						ARPriceWorksheetDetail newDetail = CreateWorksheetDetailOnAddSelItems(line);
						Base.Details.Update(newDetail);
					}
				}

				ItemFilter.Cache.Clear();
				ItemInfo.Cache.Clear();
				addItemParameters.Cache.Clear();
			}
			else if (string.IsNullOrEmpty(addItemParameters.Current.PriceCode))
			{
				var exception = new PXSetPropertyException(ErrorMessages.FieldIsEmpty, PXErrorLevel.Error, $"[{nameof(AddItemParameters.priceCode)}]");
				addItemParameters.Cache.RaiseExceptionHandling<AddItemParameters.priceCode>(addItemParameters.Current,
																							addItemParameters.Current.PriceCode,
																						exception);
			}

			return adapter.Get();
		}

		protected virtual ARPriceWorksheetDetail CreateWorksheetDetailOnAddSelItems(ARAddItemSelected line)
		{
			string priceCode = addItemParameters.Current.PriceCode;

			if (addItemParameters.Current.PriceType == PriceTypeList.Customer)
			{
				Customer customer = CustomerRepository.FindByCD(addItemParameters.Current.PriceCode);

				if (customer != null)
				{
					priceCode = customer.BAccountID.ToString();
				}
			}

			ARPriceWorksheetDetail newWorksheetDetail = new ARPriceWorksheetDetail
			{
				InventoryID = line.InventoryID,
				SiteID = addItemParameters.Current.SiteID,
				CuryID = addItemParameters.Current.CuryID,
				UOM = line.BaseUnit,
				PriceType = addItemParameters.Current.PriceType,
				SkipLineDiscounts = addItemParameters.Current.SkipLineDiscounts == true && Base.Document.Current.IsFairValue != true
			};

			newWorksheetDetail.CurrentPrice = GetItemPrice(Base, addItemParameters.Current.PriceType, priceCode, newWorksheetDetail.InventoryID,
														   newWorksheetDetail.CuryID, Base.Document.Current.EffectiveDate);

			if (addItemParameters.Current.PriceType == PriceTypes.Customer)
				newWorksheetDetail.CustomerID = Convert.ToInt32(priceCode);
			else
				newWorksheetDetail.CustPriceClassID = priceCode;

			newWorksheetDetail.PriceCode = addItemParameters.Current.PriceCode;
			return newWorksheetDetail;
		}

		protected virtual void _(Events.RowSelected<ARPriceWorksheet> e)
		{
			showItems.SetEnabled(e.Row.Hold == true && e.Row.Status == SPWorksheetStatus.Hold);
		}

		protected override void _(Events.RowSelected<AddItemFilter> e)
		{
			base._(e);
			PXCache status = e.Cache.Graph.Caches<ARAddItemSelected>();
			PXUIFieldAttribute.SetVisible<ARAddItemSelected.curyID>(status, null, true);
			PXUIFieldAttribute.SetEnabled<AddItemFilter.workGroupID>(e.Cache, e.Row, e.Row?.MyWorkGroup != true);
			PXUIFieldAttribute.SetEnabled<AddItemFilter.ownerID>(e.Cache, e.Row, e.Row?.MyOwner != true);
		}

		protected override PXView CreateItemInfoView()
		{
			var view = base.CreateItemInfoView();

			view.WhereAnd<Where<
				Brackets<AddItemFilter.ownerID.FromCurrent.IsNull.
					Or<AddItemFilter.ownerID.FromCurrent.IsEqual<ARAddItemSelected.priceManagerID>>>
				.And<AddItemFilter.myWorkGroup.FromCurrent.IsEqual<False>.
					Or<ARAddItemSelected.priceWorkgroupID.Is<IsWorkgroupOfContact<AddItemFilter.currentOwnerID.FromCurrent.Value>>>>
				.And<AddItemFilter.workGroupID.FromCurrent.IsNull.
					Or<AddItemFilter.workGroupID.FromCurrent.IsEqual<ARAddItemSelected.priceWorkgroupID>>>>>();
			
			return view;
		}

		#region AddItemParameters event handlers
		protected virtual void _(Events.RowSelected<AddItemParameters> e)
		{
			if (e.Row == null) return;
			PXUIFieldAttribute.SetVisible<AddItemParameters.curyID>(e.Cache, e.Row, PXAccess.FeatureInstalled<FeaturesSet.multicurrency>());
			PXUIFieldAttribute.SetEnabled<AddItemParameters.priceCode>(e.Cache, e.Row, e.Row.PriceType != PriceTypeList.BasePrice);
		}

		protected virtual void _(Events.RowUpdated<AddItemParameters> e)
		{
			if (e.Row == null) return;

			if (!e.Cache.ObjectsEqual<AddItemParameters.priceType>(e.Row, e.OldRow))
				e.Row.PriceCode = null;

			if (!e.Cache.ObjectsEqual<AddItemParameters.priceCode>(e.Row, e.OldRow))
			{
				if (e.Row.PriceType == PriceTypes.Customer)
				{
					PXResult<Customer> customer = SelectFrom<AR.Customer>.
													Where<AR.Customer.acctCD.IsEqual<@P.AsString>>.
													View.Select(Base, e.Row.PriceCode);
					if (customer != null)
					{
						if (((Customer)customer).CuryID != null)
							e.Row.CuryID = ((Customer)customer).CuryID;
						else
							e.Cache.SetDefaultExt<AddItemParameters.curyID>(e.Row);
					}
				}
			}
		}
		#endregion

		#region Protected Access
		[PXProtectedAccess]
		protected abstract CustomerRepository CustomerRepository { get; }

		[PXProtectedAccess]
		protected abstract decimal GetItemPrice(PXGraph graph, string priceType, string priceCode, int? inventoryID, string toCuryID, DateTime? curyEffectiveDate);
		#endregion
	}
}
