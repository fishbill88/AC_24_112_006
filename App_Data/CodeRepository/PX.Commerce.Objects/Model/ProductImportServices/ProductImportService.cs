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

using PX.Commerce.Core;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Data.BQL;
using PX.Objects.CS;
using PX.Objects.IN;
using System;
using System.Linq;
using PX.Commerce.Core.API;
using System.Collections.Generic;

namespace PX.Commerce.Objects
{
	/// <summary>
	/// CommerceHelper GrahExtension implements methods and validation rules for the product import.
	/// </summary>	
	public class ProductImportService : PXGraphExtension<CommerceHelper>
	{
		public static bool IsActive() =>  CommerceFeaturesHelper.CommerceEdition;
				
		private ProductImportSettings _settings = null;

		protected string _subEntityType = null;	

		[InjectDependency]
		protected IProductImportSettingsBuilder SettingsFactory { get; set; }

		///<inheritdoc/>
		public ProductImportSettings Settings
		{
			get
			{
				if (_settings == null)
				{
					_settings = SettingsFactory.GetSettingsInstance(Base.Processor);
				}
				return _settings;
			}
		}

		/// <summary>
		/// Returns the inventory id based on the current settings of the store.
		/// If a numbering has been defined, it will return the next number in the sequence.
		/// If there is no numbering, it returns the external sku.
		/// If the external sku is null or empty, it raises an exception.
		/// </summary>
		/// <param name="sku">The SKU from the external system</param>
		/// <param name="productName">The name of the product in the external system (used only in case we raise an exception)</param>
		/// <returns></returns>
		public virtual string GetNewProductInventoryID(string sku, string productName)
		{
			if (!string.IsNullOrWhiteSpace(Settings.InventoryNumberingID))
			{
				var inventoryMaintGraph = PXGraph.CreateInstance<InventoryItemMaint>();
				BCAutoNumberAttribute.CheckAutoNumbering(Base, Settings.InventoryNumberingID);
				return AutoNumberAttribute.GetNextNumber(inventoryMaintGraph.Caches[typeof(InventoryItem)], null, Settings.InventoryNumberingID, inventoryMaintGraph.Accessinfo.BusinessDate);				
			}

			if (String.IsNullOrEmpty(sku))
				throw new PXException(BCMessages.PISkuIsEmptyAndNoNumberingDefined, productName);

			var dimensionName = BaseInventoryAttribute.DimensionName;
			var dimension = Dimension.PK.Find(Base, dimensionName);
			if (dimension == null || !dimension.Length.HasValue)
				return sku.Trim().ToUpper();

			if (sku.Trim().Length > dimension.Length.Value)
				throw new PXException(BCMessages.PISkuLengthExceedsSegmentedKey, productName);

			return sku.Trim().ToUpper();
		}

		/// <summary>
		/// Verifies if the <paramref name="inventoryCode"/> already exists in the Database.
		/// </summary>
		/// <param name="inventoryCode">The inventory code.</param>
		/// <returns>True if there is such inventory code in the Database; otherwise - false.</returns>
		public bool IsDuplicateInventoryCode(string inventoryCode)
		{
			if (string.IsNullOrWhiteSpace(inventoryCode) || this.Settings.InventoryNumberingID is not null)
				return false;

			var duplicatedInventoryItem = InventoryItem.UK.Find(this.Base, inventoryCode);

			return duplicatedInventoryItem is not null;
		}

		/// <summary>
		/// Makes a search for a stock/non stock item based on the external sku
		/// Look for a stock/non stock item that has the sku as the inventoryCD
		/// If not found: Check for an InventoryItem that has the external sku as an alternate id		
		/// </summary>
		/// <param name="sku">The SKU of the product from the external system.</param>
		/// <param name="parentId">Used to indicate that the sku is a variant and parentId is the template item id.</param>
		/// <returns></returns>
		public virtual IEnumerable<ProductItem> FindSimilarProductForSku<T>(string sku, string parentId = null) where T:ProductItem, new()
		{
			if (string.IsNullOrWhiteSpace(sku))
				return Enumerable.Empty<ProductItem>();

			var result = new List<InventoryItem>();

			var inventoryItem = PX.Objects.IN.InventoryItem.UK.Find(Base, sku.Trim().ToUpper());

			if (inventoryItem != null && inventoryItem.InventoryID.HasValue)
				result.Add(inventoryItem);

			var xRefInventoryItems = SelectFrom<PX.Objects.IN.INItemXRef>
							  .InnerJoin<PX.Objects.IN.InventoryItem>.On<PX.Objects.IN.InventoryItem.inventoryID.IsEqual<PX.Objects.IN.INItemXRef.inventoryID>>
							  .Where<Brackets<PX.Objects.IN.INItemXRef.alternateType.IsEqual<INAlternateType.global>
										  .Or<PX.Objects.IN.INItemXRef.alternateType.IsEqual<INAlternateType.externalSku>>>
							  .And<PX.Objects.IN.InventoryItem.stkItem.IsEqual<P.AsBool>>
							  .And<PX.Objects.IN.INItemXRef.alternateID.IsEqual<P.AsString>>>										
							  .View.Select(Base, Settings.IsStockItem, sku.Trim().ToUpper()).ToList();

			foreach(var xRefItem in xRefInventoryItems)
			{
				var item = xRefItem.GetItem<PX.Objects.IN.InventoryItem>();
				if (item!=null && !result.Any(x=> x.InventoryID == item.InventoryID))
					result.Add(item);
			}

			return result.Select(x => new T()
			{
				InventoryID = x.InventoryCD.ValueField(),
				NoteID = x.NoteID.ValueField(),				
				Id = x.NoteID,
				TemplateItemID = GetParentTemplateFromId(parentId) == x.TemplateItemID ? parentId.ValueField() : null,
				LastModifiedDateTime = x.LastModifiedDateTime.ValueField()
			}).ToList();
		}

		private int? GetParentTemplateFromId(string templateId)
		{
			if (string.IsNullOrEmpty(templateId))
				return null;

			var template = InventoryItem.UK.Find(Base, templateId);
			
			if (template == null)
				return null;
			
			return template.InventoryID;
		}


		#region "Similar Template Items"

		/// <summary>
		/// Search for similar Template Items.
		/// 1. Search by SKUs comparing the SKUs to InventoryCD and AlternateID (global & externalSku)
		/// 2. If the step 1 result in an empty list, search by description comparing the description to InventoryItem.descr
		/// 3. Group the results by TemplateItemID. If more than one, make a search by Template Items description.
		/// 4. Return the template items that are matching.
		/// </summary>
		/// <param name="variantsSKUs">List of Variants SKUs.</param>
		/// <param name="variantsNames">List of Variants Names.</param>
		/// <param name="templateItemsName">Template Items Name in the external system</param>
		/// <returns></returns>
		public virtual IEnumerable<TemplateItems> FindSimilarTemplateItems(List<string> variantsSKUs, List<string> variantsNames, string templateItemsName)
		{
			var intventoryItems = new List<InventoryItem>();

			var trimmedToUpperSkus = variantsSKUs.Select(x => x.Trim().ToUpper()).ToList();
			var trimmedNames = variantsNames !=null ? variantsNames.Select(x => x.Trim()).ToList() : new List<string>();

			var similarInventoryItems = SelectFrom<PX.Objects.IN.InventoryItem>
							  .LeftJoin<PX.Objects.IN.INItemXRef>
							  .On<PX.Objects.IN.InventoryItem.inventoryID.IsEqual<PX.Objects.IN.INItemXRef.inventoryID>>
							  .Where<Brackets<PX.Objects.IN.INItemXRef.alternateType.IsEqual<INAlternateType.global>
										  .Or<PX.Objects.IN.INItemXRef.alternateType.IsEqual<INAlternateType.externalSku>>>
							  .And<PX.Objects.IN.InventoryItem.templateItemID.IsNotNull>
							  .And<Brackets<PX.Objects.IN.InventoryItem.inventoryCD.IsIn<P.AsString>>
								.Or<PX.Objects.IN.INItemXRef.alternateID.IsIn<P.AsString>>
								.Or<PX.Objects.IN.InventoryItem.descr.IsIn<P.AsString>>>>
							  .View.Select(Base, trimmedToUpperSkus, trimmedToUpperSkus, trimmedNames).ToList();

			//if there is one inventory that matches with the SKU, then we don't consider other inventory items that match with the description
			var skipDescriptionMatches = similarInventoryItems.Any(x => trimmedToUpperSkus.Contains(x.GetItem<PX.Objects.IN.InventoryItem>().InventoryCD.Trim()));

			foreach (var inventoryItem in similarInventoryItems)
			{
				var item = inventoryItem.GetItem<PX.Objects.IN.InventoryItem>();

				if (skipDescriptionMatches && !trimmedToUpperSkus.Contains(item.InventoryCD.Trim()))
					continue;

				if (!intventoryItems.Any(x => x.InventoryID == item.InventoryID))
					intventoryItems.Add(item);
			}

			//Group by templateItemID
			var groupedByTemplateItems = intventoryItems.GroupBy(x => x.TemplateItemID).ToList();

			//Retrieve template items
			var templateItems = SelectFrom<PX.Objects.IN.InventoryItem>
							   .Where<PX.Objects.IN.InventoryItem.inventoryID.IsIn<P.AsInt>>
							   .View.Select(Base, groupedByTemplateItems.Select(x => x.Key).ToList()).Select(x => x.GetItem<PX.Objects.IN.InventoryItem>()).ToList();

			//If there is only one template item, then we return it
			if (templateItems.Count == 1)
			{
				yield return FromInventoryItem(templateItems[0]);
				yield break;
			}

			//If there is more than one template item, then we need to search by description
			if (templateItems.Count > 1)
			{
				var templateWithMatchingName = templateItems.Where(x => x.Descr.Equals(templateItemsName, StringComparison.OrdinalIgnoreCase)).ToList();

				//Return only those that have a matching name 
				if (templateWithMatchingName?.Count >= 1)
				{
					foreach (var template in templateWithMatchingName)
						yield return FromInventoryItem(template);

					yield break;
				}
			}

			if (templateItems.Count == 0)
			{
				var templateItemsByDesription =
					SelectFrom<InventoryItem>
					.Where<InventoryItem.isTemplate.IsEqual<True>
						.And<InventoryItem.descr.IsEqual<P.AsString>>>
					.View.Select(Base, templateItemsName).ToList();

				foreach (var template in templateItemsByDesription)
					yield return FromInventoryItem(template);

				yield break;
			}

			foreach (var template in templateItems)
				yield return FromInventoryItem(template);

			yield break;
		}

		/// <summary>
		/// Returns a template items from an InventoryItem record.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected virtual TemplateItems FromInventoryItem(InventoryItem item)
		{
			return new TemplateItems()
			{
				InventoryID = item.InventoryCD.ValueField(),
				NoteID = item.NoteID.ValueField(),
				Id = item.NoteID,
				TemplateItemID = null,
				LastModifiedDateTime = item.LastModifiedDateTime.ValueField()
			};
		}
		#endregion

		/// <summary>
		/// Given an existing inventoryItem that matches with the current inventoryItem that
		/// we want to import. This method will validate whether or not we can merge the imported product with the existing one.
		/// If the merge is not allowed, it will throw an exception.
		/// If the current inventoryItem does not have an external sku, then we can merge it with the existing one.
		/// If it has an sku:
		/// - If it has a record in BCSyncHistory for the current connector, then we can merge it (same inventoryItem that is updated)
		/// - If it has not a record in BCSyncHistory for the current connector, then we cannot merge it (it is a duplicate product with same sku but not the same product)		 
		/// </summary>
		/// <param name="productItem">The matching productItem (considered as the same as the one we want to import)</param>
		/// <param name="exernalRefNumber">The ID (not sku) from the external system - same id stored in the BCSyncHistory table</param>
		/// <param name="entityType"></param>
		public virtual void ValidateMergingWithExistingItem(ProductItem productItem, string exernalRefNumber, string entityType)
		{
			if (productItem == null || productItem.NoteID == null)
				return;

			var syncStatus = BCSyncStatus.LocalIDIndex.Find(Base, Settings.ConnectorType, Settings.BindingId, entityType, productItem.NoteID.Value);

			var duplicateSku = syncStatus != null && syncStatus.ExternID != null && !syncStatus.ExternID.Equals(exernalRefNumber);
			var hasBeenSyncedButNotWithCurrentConnector = syncStatus == null && !string.IsNullOrEmpty(productItem.ExternalSku?.Value);

			if (duplicateSku || hasBeenSyncedButNotWithCurrentConnector)
				throw new PXException(BCMessages.PICannotImportBecauseOfDuplicateSku, exernalRefNumber, productItem.ExternalSku, productItem.InventoryID.Value);
		}

		#region "Product Class ID"
		/// <summary>
		/// Gets the item class for the product.
		/// 1 - if a substitution list is provided, it will try to find a class that matches the product type.
		/// 1.1  - if the class id in the substitution list is not a valid class id, raise an error.
		/// 2 - if no substitution list is provided, it will try to find a class that matches the product type (case insensitive
		/// 3 - if no class is found, it will use the default class.
		/// </summary>
		/// <param name="itemClassId">A string representing the externalSystemItemClassId</param>
		/// <returns></returns>
		public virtual string GetItemClassFromID(string itemClassId)
		{
			var defaultClassId = Settings.GetDefaultItemClassID();
			
			var substitutionListId = Settings.ItemClassSubstitutionListID;
			string classId = null;

			if (!String.IsNullOrEmpty(substitutionListId) && !String.IsNullOrEmpty(itemClassId))
			{
				var substitutedClassId = Base.GetSubstituteLocalByExtern(substitutionListId, itemClassId.ToUpper(), defaultClassId.ToString());
				if (!String.IsNullOrEmpty(substitutedClassId) && substitutedClassId != defaultClassId.ToString())
				{
					classId = SearchForItemClassByID(substitutedClassId)?.ItemClassCD.Trim();
					//if (string.IsNullOrEmpty(classId))
					//	throw new PXException(BCMessages.PIItemClassDoesNotExist, substitutedClassId);
					return classId;
				}
			}

			//Not found in the substitution list
			//Try to find it by the external identifier.
			classId = !string.IsNullOrEmpty(itemClassId) ? SearchForItemClassByID(itemClassId)?.ItemClassCD.Trim() : null;
			return !string.IsNullOrEmpty(classId) ? classId : defaultClassId.ToString();
		}

		/// <summary>
		/// Returns the item class based on the given id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public virtual INItemClass SearchForItemClassByID(string id)
		{
			if (string.IsNullOrEmpty(id))
				return null;

			var itemClass = SelectFrom<INItemClass>
				.Where<PX.Data.BQL.RTrim<INItemClass.itemClassCD>.IsEqual<P.AsString>>
				.View.Select(Base, id.ToUpper()).FirstOrDefault()?.GetItem<INItemClass>();

			return itemClass;
		}

		/// <summary>
		/// Validates the item class and raise exceptions in case it does not exist or it is not the right type
		/// </summary>
		/// <param name="itemClass">The item class to validate</param>
		/// <param name="itemClassId">The id of the item class (used to raise an exception if it does not exist)</param>
		/// <param name="itemName">The current Item for which we validate. It is used for error message display only</param>
		/// <param name="numberOfAttirbutesForTemplateItems">Number of attributes (options) for a template items.</param>
		public virtual void ValidateItemClass(INItemClass itemClass, string itemClassId, string itemName, int numberOfAttirbutesForTemplateItems = 0)
		{
			var entityName = Settings.IsStockItem ? BCAPICaptions.StockItem : BCAPICaptions.NonStockItem;
			if (Settings.IsTemplateItem)
				entityName = BCAPICaptions.TemplateItem;

			if (itemClass == null || itemClass.ItemClassID.HasValue == false)
				throw new PXException(BCMessages.PIItemClassDoesNotExist, itemName, entityName, Settings.ItemClassSubstitutionListID);

			if (Settings.IsTemplateItem && Settings.IsNonStockItem && itemClass.StkItem.HasValue && itemClass.StkItem.Value == true)
				throw new PXException(BCMessages.PIInvalidItemClassForTemplateWithNonStockItems, itemClass.ItemClassCD.Trim(), itemName);

			if (Settings.IsNonStockItem && itemClass.StkItem.HasValue && itemClass.StkItem.Value == true)
				throw new PXException(BCMessages.PIInvalidItemClassForNonStockItem, itemClass.ItemClassCD.Trim());

			if (!Settings.IsNonStockItem && (!itemClass.StkItem.HasValue || itemClass.StkItem.Value == false))
				throw new PXException(BCMessages.PIInvalidItemClassForNonStockItem, itemClass.ItemClassCD.Trim());

			if (string.IsNullOrEmpty(itemClass.PostClassID))
				throw new PXException(BCMessages.PIItemClassDoesNotHavePostingClassID, itemClass.ItemClassCD.Trim());

			if (!Settings.IsNonStockItem && string.IsNullOrEmpty(itemClass.LotSerClassID))
				throw new PXException(BCMessages.PIItemClassDoesNotHaveALotSerialClass, itemClass.ItemClassCD.Trim());

			if (Settings.IsTemplateItem && (string.IsNullOrEmpty(itemClass.DefaultColumnMatrixAttributeID)))
				throw new PXException(BCMessages.PITemplateItemClassDoesNotHaveDefaultColumn, itemName, itemClass.ItemClassCD.Trim());

			if (Settings.IsTemplateItem && (numberOfAttirbutesForTemplateItems > 1 && string.IsNullOrEmpty(itemClass.DefaultRowMatrixAttributeID)))
				throw new PXException(BCMessages.PITemplateItemClassDoesNotHaveDefaultColumn, itemName, itemClass.ItemClassCD.Trim());
		}

		#endregion

		#region "Tax Category"

		/// <summary>
		/// Get a tax category from the substitution list (if configured)
		/// </summary>
		/// <param name="taxCategory"></param>
		/// <returns></returns>
		public virtual string GetTaxCategoryFromSubstitutionList(string taxCategory)
		{
			var substitutionListId = Settings.TaxCategorySubstitutionListID;
			return GetValueFromSubstitutionList(taxCategory, substitutionListId);
		}

		/// <summary>
		/// Returns a corresponding value from a substitution list.
		/// </summary>
		/// <param name="originalValue"></param>
		/// <param name="substitutionListId"></param>
		/// <returns></returns>
		protected virtual string GetValueFromSubstitutionList(string originalValue, string substitutionListId)
		{
			if (!String.IsNullOrEmpty(substitutionListId) && !String.IsNullOrEmpty(originalValue))
			{
				var substitutedTaxCategory = Base.GetSubstituteLocalByExtern(substitutionListId, originalValue.ToUpper(), null);
				return substitutedTaxCategory;
			}

			return null;
		}

		/// <summary>
		/// Returns the tax category based on the given item class and substitution list (if configured)
		/// </summary>
		/// <param name="itemClassCD">the Item Class ID of the imported product</param>
		/// <returns></returns>
		public virtual string GetTaxCategoryFromItemClass(string itemClassCD)
		{
			if (string.IsNullOrEmpty(itemClassCD))
				return null;

			INItemClass itemClass = SelectFrom<INItemClass>
				.Where<PX.Data.BQL.RTrim<INItemClass.itemClassCD>.IsEqual<P.AsString>>
				.View
				.Select(Base, itemClassCD.ToUpper()).FirstOrDefault()?.GetItem<INItemClass>();

			return itemClass?.TaxCategoryID; 
		}
		#endregion
	}
}
