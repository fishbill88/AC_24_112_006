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

using PX.Commerce.Amazon.API.Rest;
using PX.Commerce.Core;
using PX.Commerce.Core.API;
using PX.Commerce.Objects;
using PX.Data;
using PX.Objects.CA;
using PX.Objects.IN;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PX.Commerce.Amazon
{
	public class AmazonHelper : CommerceHelper
	{
		private string[] _itemStatuses = new string[] { InventoryItemStatus.Active, InventoryItemStatus.NoPurchases, InventoryItemStatus.NoRequest };

		public virtual void SetFilterDate(FilterOrders filter, DateTime? minDateTime, DateTime? maxDateTime, DateTime? syncOrdersFrom)
		{
			if (minDateTime != null || maxDateTime != null)
			{
				DateTime? localMinDateTime = minDateTime?.ToDate();
				filter.LastUpdatedAfter = localMinDateTime >= syncOrdersFrom ? localMinDateTime : syncOrdersFrom;
				if (maxDateTime != null)
				{
					filter.LastUpdatedBefore = maxDateTime.ToDate();
				}
			}
			else if (syncOrdersFrom != null)
			{
				filter.CreatedAfter = syncOrdersFrom.Value;
			}
		}
		public virtual string GetInventoryCDByExternID(string sku, string asin, string message, out string uom)
		{
			string SKUConverted = sku.Trim().ToUpper();
			string asinConverted = asin.Trim().ToUpper();

			Dictionary<string, string> crossrefUOMs = new Dictionary<string, string>();

			PX.Objects.IN.InventoryItem item = TryGetInventoryByInventoryCD(SKUConverted, asinConverted);

			if (item == null && (sku != null || asin != null)) //Search by cross references
			{
				PX.Objects.IN.InventoryItem itemCandidate = null;
				PX.Objects.IN.INItemXRef crossrefCandidate = null;

				string[] currentAlternateIDs = new string[] { asinConverted, SKUConverted };
				List<string> matchItems = new List<string>();
				foreach (PXResult<PX.Objects.IN.INItemXRef, PX.Objects.IN.InventoryItem> result in PXSelectJoin<
					PX.Objects.IN.INItemXRef,
					InnerJoin<PX.Objects.IN.InventoryItem,
						On<PX.Objects.IN.INItemXRef.inventoryID, Equal<PX.Objects.IN.InventoryItem.inventoryID>>>,
					Where<PX.Objects.IN.INItemXRef.alternateType, Equal<INAlternateType.global>,
						And<PX.Objects.IN.INItemXRef.alternateID, In<Required<PX.Objects.IN.INItemXRef.alternateID>>,
							And<PX.Objects.IN.InventoryItem.itemStatus, In<Required<InventoryItem.itemStatus>>>>>>
					.Select(this, currentAlternateIDs, _itemStatuses))
				{
					if (itemCandidate != null && itemCandidate.InventoryID != result.GetItem<PX.Objects.IN.InventoryItem>().InventoryID)
					{
						matchItems.Add(result.GetItem<PX.Objects.IN.InventoryItem>().InventoryCD.Trim());
						continue;
					}

					itemCandidate = result.GetItem<PX.Objects.IN.InventoryItem>();
					crossrefCandidate = result.GetItem<PX.Objects.IN.INItemXRef>();
					matchItems.Add(result.GetItem<PX.Objects.IN.InventoryItem>().InventoryCD.Trim());
					crossrefUOMs.Add(crossrefCandidate?.AlternateID.ToUpper().Trim(), crossrefCandidate?.UOM);
				}

				if (matchItems.Count > 1)
					throw new PXException(AmazonMessages.MultipleItemsSameCrossRefAmazon, string.IsNullOrEmpty(sku) ? asin : sku, string.Join(", ", matchItems));

				item = itemCandidate;
			}

			if (item == null)
				throw new PXException(message, sku, asin);

			uom = GetUOMFromCrossRefs(crossrefUOMs, SKUConverted, asinConverted) ?? item?.SalesUnit?.Trim();
			return item?.InventoryCD?.Trim();
		}

		public virtual InventoryItem TryGetInventoryByInventoryCD(string sku, string asin)
		{
			string[] searchValues = new string[] { sku, asin };

			InventoryItem item;
			var inventoryItems = PXSelect<
							PX.Objects.IN.InventoryItem,
							Where<PX.Objects.IN.InventoryItem.inventoryCD,In<Required<PX.Objects.IN.InventoryItem.inventoryCD>>,
								And<PX.Objects.IN.InventoryItem.itemStatus, In<Required<InventoryItem.itemStatus>>>>>
							.Select(this, searchValues, _itemStatuses)
							.RowCast<PX.Objects.IN.InventoryItem>()
							.ToList();

			if (inventoryItems.Any(x => x.InventoryCD.Trim() == sku))
			{
				item = inventoryItems.Where(x => x.InventoryCD.Trim() == sku).FirstOrDefault();
			}
			else
			{
				item = inventoryItems.Where(x => x.InventoryCD.Trim() == asin).FirstOrDefault();
			}

			return item;
		}

		public virtual string GetUOMFromCrossRefs(Dictionary<string, string> crossrefUOMs, string sku, string asin)
		{
			string skuUOM;
			crossrefUOMs.TryGetValue(sku, out skuUOM);

			string asinUOM;
			crossrefUOMs.TryGetValue(asin, out asinUOM);

			return skuUOM ?? asinUOM;
		}

		public virtual BCPaymentMethods GetPaymentMethodMapping(string gateway, string currency, out string cashAccount, bool throwError = true)
		{
			cashAccount = null;
			BCPaymentMethods result = null;
			if (!PaymentMethods().Any(x => string.Equals(currency, x.StoreCurrency, StringComparison.OrdinalIgnoreCase) && string.Equals(x.StorePaymentMethod, gateway, StringComparison.OrdinalIgnoreCase)))
			{
				PXCache cache = base.Caches[typeof(BCPaymentMethods)];
				BCPaymentMethods newMapping = new BCPaymentMethods()
				{
					BindingID = _processor.Operation.Binding,
					StoreCurrency = currency,
					StorePaymentMethod = gateway.ToUpper(),
					Active = true,
				};
				newMapping = (BCPaymentMethods)cache.Insert(newMapping);
				cache.Persist(newMapping, PXDBOperation.Insert);
				if (throwError)
					throw new PXException(BCMessages.OrderPaymentMethodIsMissing, gateway?.ToUpper(), null, currency);
			}
			else if (!PaymentMethods().Any(x => string.Equals(currency, x.StoreCurrency, StringComparison.OrdinalIgnoreCase) && string.Equals(x.StorePaymentMethod, gateway, StringComparison.OrdinalIgnoreCase) && x.Active == true))
			{
				if (throwError)// note this parameter is passed as false for refunds
					throw new PXException(BCMessages.OrderPaymentMethodIsMissing, gateway?.ToUpper(), null, currency);
				return null;
			}
			else
			{
				result = PaymentMethods().FirstOrDefault(x => string.Equals(currency, x.StoreCurrency, StringComparison.OrdinalIgnoreCase) && string.Equals(x.StorePaymentMethod, gateway, StringComparison.OrdinalIgnoreCase) && x.Active == true);

				CashAccount ca = PXSelect<CashAccount, Where<CashAccount.cashAccountID, Equal<Required<CashAccount.cashAccountID>>>>.Select(this, result?.CashAccountID);

				cashAccount = ca?.CashAccountCD;

				if (cashAccount == null || result?.PaymentMethodID == null)
				{
					throw new PXException(BCMessages.OrderPaymentMethodIsMissing, gateway?.ToUpper(), null, currency);
				}
			}

			return result;
		}

		public virtual (string InventoryCD, string UOM) GetInventoryCDForGiftWrap(int? giftWrapInventoryID)
		{
			InventoryItem inventory = PX.Objects.IN.InventoryItem.PK.Find(this, giftWrapInventoryID);
			return (inventory?.InventoryCD?.Trim() ?? string.Empty, inventory?.BaseUnit?.Trim() ?? string.Empty);
		}

		public virtual (string InventoryCD, string UOM) GetInventoryCDForShippingItem(int? shippingInventoryID)
		{
			if (shippingInventoryID == null)
			{
				throw new PXException(AmazonMessages.TheShippingItemIsNotSpecified);
			}

			InventoryItem inventory = PX.Objects.IN.InventoryItem.PK.Find(this, shippingInventoryID);

			if (inventory?.InventoryCD == null)
				throw new PXException(AmazonMessages.ShippingPriceItemDoesNotExist);

			return (inventory.InventoryCD.Trim(), inventory.BaseUnit.Trim());
		}
	}
}
