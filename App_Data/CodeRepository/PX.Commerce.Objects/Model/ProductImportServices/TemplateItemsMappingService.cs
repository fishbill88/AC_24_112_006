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

using System.Collections.Generic;
using System.Linq;
using PX.Commerce.Core;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Data.BQL;
using PX.Objects.IN;
using PX.Objects.CS;
using PX.Commerce.Core.API;
using PX.Common;
using System;

namespace PX.Commerce.Objects
{
	#region DTO Classes
	/// <summary>
	/// View that represent a mapping from external system option/value to an attribute/value in the ERP.
	/// </summary>
	public class TemplateMappedOptions
	{
		/// <summary>
		/// The optionID from the external system
		/// </summary>
		public string OptionID { get; set; }

		/// <summary>
		/// The option sort order.
		/// </summary>
		public int? OptionSortOrder { get; set; }

		/// <summary>
		/// The option name in the external system
		/// </summary>
		public string OptionName { get; set; }
		/// <summary>
		/// The value from the external system
		/// </summary>
		public string OptionValue { get; set; }
		/// <summary>
		/// Mapped Item Class ID
		/// </summary>
		public int? ItemClassID { get; set; }
		/// <summary>
		/// Attribute mapped to the Option Id
		/// </summary>
		public string AttributeID { get; set; }
		/// <summary>
		/// Value mapped to the option value
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// Description of the ERP attribute.
		/// </summary>
		public string AttributeDescription { get; set; }

		/// <summary>
		/// Defines if the instance is similar to the specified <paramref name="attribute"/>.
		/// </summary>
		/// <param name="attribute">The attribute value to compare.</param>
		/// <returns>True if current instance is similar to the specified attribute; otherwise false.</returns>
		public bool IsSimilarTo(AttributeValue attribute)
		{
			if (attribute is null)
				throw new ArgumentNullException(nameof(attribute));

			if (attribute.AttributeID is null)
				throw new ArgumentNullException(nameof(attribute.AttributeID));

			if (attribute.Value is null)
				throw new ArgumentNullException(nameof(attribute.Value));

			return attribute.AttributeID.Value.Equals(this.AttributeID, StringComparison.OrdinalIgnoreCase)
				&& attribute.Value.Value.Equals(this.Value, StringComparison.OrdinalIgnoreCase);
		}
	}

	/// <summary>
	/// Represent an External Option for Templates
	/// </summary>
	public class ExternalTemplateOptions
	{
		/// <summary>
		/// The Sync in the BCSyncStatus table
		/// </summary>
		public int SyncID { get; set; }

		/// <summary>
		/// Option's sort order in the external system
		/// </summary>

		public int? OptionSortOrder { get; set; }
		/// <summary>
		/// Option ID in the external system
		/// </summary>

		public string OptionID { get; set; }
		/// <summary>
		/// The option display name in the external system
		/// </summary>
		public string OptionDisplayName { get; set; }

		/// <summary>
		/// The option name in the external system
		/// </summary>
		public string OptionName { get; set; }

		/// <summary>
		/// The option value in the external system
		/// </summary>
		public string OptionValue { get; set; }
		/// <summary>
		/// The external ID in the external system
		/// </summary>
		public string ExternalID { get; set; }

		/// <summary>
		/// The external id of the option value
		/// </summary>
		public string OptionValueExternalId { get; set; }

		/// <summary>
		/// The Status of the template in the BCSyncStatus table
		/// </summary>
		public EntityStatus SyncStatus { get; set; }
	}

	public class AttributeDetails
	{
		public string OptionExternId { get; set; }
		public string OptionValueExternId { get; set; }
		public Guid? AttributeNoteId { get; set; }
		public Guid? AttributeValueNoteId { get; set; }
	}

	/// <summary>
	/// Represent the status of the mapping for a particular Template
	/// </summary>
	public enum OptionMappingStatus
	{
		/// <summary>
		/// Valid mapping. 
		/// </summary>
		Valid,
		/// <summary>
		/// Not all options/values are mapped
		/// </summary>
		NotOrPartiallyMapped,
		/// <summary>
		/// The mapping has duplicated mapped values for the same attribute
		/// </summary>
		HasDuplicates
	}

	public class OptionMappingValidationResult
	{
		public OptionMappingStatus Status { get; set; }
		public string AttributeWithDuplicate { get; set; }
		public string FirstDuplicatedValue { get; set; }

		public static OptionMappingValidationResult ValidResult = new OptionMappingValidationResult() { Status = OptionMappingStatus.Valid };
	}
	#endregion

	/// <summary>
	/// CommerceHelper GrahExtension that manages the Options Mapping for Template Items import.
	/// </summary>
	public class TemplateItemsMappingService : PXGraphExtension<CommerceHelper>
	{
		public static bool IsActive() => CommerceFeaturesHelper.CommerceEdition;

		public SelectFrom<BCMatrixOptionsMapping>
		  .InnerJoin<BCSyncStatus>.On<BCSyncStatus.syncID.IsEqual<BCMatrixOptionsMapping.syncID>>
		  .Where<BCSyncStatus.status.IsNotEqual<BCSyncStatusAttribute.synchronized>> OptionMappings;

		/// <summary>
		/// Returns the list of Mapped Option for the Templated related to the SyncId
		/// </summary>
		/// <param name="syncId"></param>
		/// <returns></returns>
		public virtual List<TemplateMappedOptions> GetMappedOptionsForTemplate(int? syncId)
		{
			var mappings = SelectFrom<BCMatrixOptionsMapping>
								.Where<BCMatrixOptionsMapping.syncID.IsEqual<P.AsInt>>.View.Select(Base, syncId).ToList();

			var result = new List<TemplateMappedOptions>();
			if (mappings.Count == 0)
				return result;

			foreach (var mapping in mappings)
			{
				var row = mapping.GetItem<BCMatrixOptionsMapping>();
				result.Add(new TemplateMappedOptions()
				{
					OptionID = row.ExternalOptionID,
					OptionValue = row.ExternalOptionValue,
					AttributeID = row.MappedAttributeID,
					OptionName = row.ExternalOptionName,
					OptionSortOrder = row.ExternalOptionSortOrder,
					Value = row.MappedValue,
					ItemClassID = row.ItemClassID
				});
			}

			return result;
		}

		/// <summary>
		/// Validates whether the mappings for a particular template item are valid.
		/// 1 - All options have been mapped to an attribute and an attribute value
		/// 2 - There are no duplicates in the mapping (same attribute and value)
		/// </summary>
		/// <param name="syncId"></param>
		/// <returns></returns>
		public virtual OptionMappingValidationResult ValidateOptionsMappings(int syncId)
		{

			var result = new OptionMappingValidationResult() { Status = OptionMappingStatus.Valid };

			var mappings = SelectFrom<BCMatrixOptionsMapping>
				.Where<BCMatrixOptionsMapping.syncID.IsEqual<P.AsInt>>.View.Select(Base, syncId)
				.Select(mapping => new
				{
					mapping.GetItem<BCMatrixOptionsMapping>().ItemClassID,
					mapping.GetItem<BCMatrixOptionsMapping>().MappedAttributeID,
					mapping.GetItem<BCMatrixOptionsMapping>().MappedValue,
				})
				.ToList();

			if (!mappings.Any())
				return new OptionMappingValidationResult() { Status = OptionMappingStatus.NotOrPartiallyMapped };

			if (mappings.Any(m => !m.ItemClassID.HasValue
								|| string.IsNullOrWhiteSpace(m.MappedAttributeID)
								|| string.IsNullOrWhiteSpace(m.MappedValue)))
				return new OptionMappingValidationResult() { Status = OptionMappingStatus.NotOrPartiallyMapped };

			var groupBy = mappings
				.GroupBy(x => new
				{
					x.ItemClassID,
					x.MappedAttributeID,
					x.MappedValue
				})
				.Select(x => new { x.Key.MappedAttributeID, x.Key.MappedValue, count = x.Count() });

			var duplicate = groupBy.FirstOrDefault(x => x.count > 1);

			if (duplicate != null)
				return new OptionMappingValidationResult() { AttributeWithDuplicate = duplicate.MappedAttributeID, FirstDuplicatedValue = duplicate.MappedValue, Status = OptionMappingStatus.HasDuplicates };

			return OptionMappingValidationResult.ValidResult;
		}

		/// <summary>
		/// Return the Mapped Item Class to the Template
		/// </summary>
		/// <param name="syncId"></param>
		/// <returns></returns>
		public virtual INItemClass GetTemplateMappedItemClass(int? syncId)
		{
			var itemClass = SelectFrom<BCMatrixOptionsMapping>
								.InnerJoin<INItemClass>.On<BCMatrixOptionsMapping.itemClassID.IsEqual<INItemClass.itemClassID>>
								.Where<BCMatrixOptionsMapping.syncID.IsEqual<P.AsInt>>.View.Select(Base, syncId).FirstOrDefault()?.GetItem<INItemClass>();
			return itemClass;
		}

		/// <summary>
		/// Add/Remove options in the mapping table for a Template.
		/// Add options that are not in the table.
		/// Remove options that are not coming from the external system but were inserted before in the options mapping table
		/// </summary>
		/// <param name="options"></param>
		public virtual void UpdateInsertOptions(List<ExternalTemplateOptions> options)
		{
			if (options == null || options.Count == 0)
				return;

			var syncId = options[0].SyncID;
			var externalId = options[0].ExternalID;

			//First verify whether we do have any existing mappings			
			var existingMappedOptions = SelectFrom<BCMatrixOptionsMapping>
								.LeftJoin<BCSyncStatus>.On<BCSyncStatus.syncID.IsEqual<BCMatrixOptionsMapping.syncID>>
								.Where<BCMatrixOptionsMapping.syncID.IsEqual<P.AsInt>>.View.Select(Base, syncId);


			var itemClassId = existingMappedOptions.FirstOrDefault()?.GetItem<BCMatrixOptionsMapping>().ItemClassID;	
			var update = false;
			PXCache cache = Base.Caches[typeof(BCMatrixOptionsMapping)];

			//Insert new options/values
			foreach (var option in options)
			{
				var existingOption = existingMappedOptions.FirstOrDefault(x => x.GetItem<BCMatrixOptionsMapping>().ExternalOptionValue == option.OptionValue &&
																			(x.GetItem<BCMatrixOptionsMapping>().ExternalOptionID == option.OptionID ||
																			 x.GetItem<BCMatrixOptionsMapping>().ExternalOptionName == option.OptionDisplayName));

				if (existingOption == null)
				{
					//Find class and similar option in the mapping table
					//And map it to the same attribute
					var similarOption = existingMappedOptions.FirstOrDefault(x => x.GetItem<BCMatrixOptionsMapping>().ExternalOptionID == option.OptionID ||
																			 x.GetItem<BCMatrixOptionsMapping>().ExternalOptionName == option.OptionDisplayName)?
																			 .GetItem<BCMatrixOptionsMapping>();

					var newMapping = new BCMatrixOptionsMapping()
					{
						SyncID = option.SyncID,
						ExternID = option.ExternalID,
						ExternalOptionID = option.OptionID,
						ExternalOptionName = option.OptionDisplayName,
						ExternalOptionValue = option.OptionValue,
						ExternalOptionSortOrder = option.OptionSortOrder,
						ExternalOptionValueID = option.OptionValueExternalId,
						ItemClassID = itemClassId,
						MappedAttributeID = similarOption?.MappedAttributeID,
						MappedValue = GetSimilarValueForAttribute(similarOption?.MappedAttributeID, option.OptionValue, option.OptionValueExternalId)
					};
					update = true;
					cache.Insert(newMapping);
				}
				else
				{
					var row = existingOption.GetItem<BCMatrixOptionsMapping>();
					if (row.ExternalOptionName != option.OptionDisplayName
						|| row.ExternalOptionSortOrder != option.OptionSortOrder
						|| row.ExternalOptionValueID != option.OptionValueExternalId
						|| string.IsNullOrEmpty(row.ExternalOptionID) ||row.ExternalOptionID!=option.OptionID)
					{
						row.ExternalOptionName = option.OptionDisplayName;
						row.ExternalOptionSortOrder = option.OptionSortOrder;
						row.ExternalOptionValueID = option.OptionValueExternalId;
						row.ExternalOptionID = option.OptionID;
						update = true;
						cache.Update(row);
					}

				}
			}

			cache.Persist(PXDBOperation.Insert);
			cache.Persist(PXDBOperation.Update);
		}

		/// <summary>
		/// Try to find a similar value for an attribute.
		/// </summary>
		/// <param name="attributeId"></param>
		/// <param name="optionValue"></param>
		/// <param name="optionValueID"></param>
		/// <returns></returns>
		protected virtual string GetSimilarValueForAttribute(string attributeId, string optionValue, string optionValueID)
		{
			var attributeValues = SelectFrom<CSAttributeDetail>
									.Where<CSAttributeDetail.attributeID.IsEqual<P.AsString>
									.And<CSAttributeDetail.disabled.IsNotEqual<True>>
									.And<Brackets<CSAttributeDetail.valueID.IsEqual<P.AsString>
											.Or<CSAttributeDetail.description.IsEqual<P.AsString>
											.Or<CSAttributeDetail.valueID.IsEqual<P.AsString>
											.Or<CSAttributeDetail.description.IsEqual<P.AsString>>>>>>>
									.View.Select(Base, attributeId, optionValue, optionValue, optionValueID, optionValueID)
									.Select(x => x.GetItem<CSAttributeDetail>()).FirstOrDefault();

			return attributeValues?.ValueID;
		}

		/// <summary>
		/// Returns the list of attributes/values for a particular syncid
		/// Use the resulting list to insert sync details for PO an PU entities
		/// </summary>
		/// <param name="syncid"></param>
		/// <returns></returns>
		public virtual List<AttributeDetails> GetAttributesDetails(int? syncid)
		{
			if (!syncid.HasValue)
				return null;

			var details = new List<AttributeDetails>();

			var mappingsWithAttributeDetails = SelectFrom<BCMatrixOptionsMapping>
											   .InnerJoin<CSAttribute>.On<CSAttribute.attributeID.IsEqual<BCMatrixOptionsMapping.mappedAttributeID>>
											   .InnerJoin<CSAttributeDetail>.On<CSAttributeDetail.attributeID.IsEqual<CSAttribute.attributeID>
																			   .And<CSAttributeDetail.valueID.IsEqual<BCMatrixOptionsMapping.mappedValue>>>
											   .Where<BCMatrixOptionsMapping.syncID.IsEqual<P.AsInt>>.View.Select(Base, syncid);

			foreach (var row in mappingsWithAttributeDetails)
			{
				var mapping = row.GetItem<BCMatrixOptionsMapping>();
				var attribute = row.GetItem<CSAttribute>();
				var attributeDetail = row.GetItem<CSAttributeDetail>();
				details.Add(new AttributeDetails()
				{
					OptionExternId = mapping.ExternalOptionID,
					OptionValueExternId = mapping.ExternalOptionValueID,
					AttributeNoteId = attribute.NoteID,
					AttributeValueNoteId = attributeDetail.NoteID
				});
			}

			return details;
		}

		/// <summary>
		/// Look for an already synced item in the sync details table.
		/// </summary>
		/// <param name="parentSyncId"></param>
		/// <param name="externalId"></param>
		/// <returns></returns>
		public virtual Guid? GetSyncedMatrixItemNoteID(int? parentSyncId, string externalId)
		{
			var syncDetails = SelectFrom<BCSyncDetail>.Where<BCSyncDetail.syncID.IsEqual<P.AsInt>
													  .And<BCSyncDetail.entityType.IsEqual<P.AsString>>
													  .And<BCSyncDetail.externID.IsEqual<P.AsString>>>
													  .View.Select(Base, parentSyncId, BCEntitiesAttribute.Variant, externalId)
													  .FirstOrDefault()?.GetItem<BCSyncDetail>();
			return syncDetails?.LocalID;
		}

		/// <summary>
		/// Returns the list of attribute values for an ItemClass
		/// </summary>
		/// <param name="itemClassId"></param>
		/// <returns></returns>
		public virtual IEnumerable<AttributeValue> GetAttributesDefitionsForClassID(int itemClassId)
		{
			var attributes = SelectFrom<CSAttribute>
							 .InnerJoin<CSAttributeGroup>.On<CSAttributeGroup.attributeID.IsEqual<CSAttribute.attributeID>>
							 .InnerJoin<INItemClass>.On<CSAttributeGroup.entityClassID.IsEqual<Use<INItemClass.itemClassID>.AsString>>
							 .Where<INItemClass.itemClassID.IsEqual<P.AsInt>
							 .And<CSAttribute.controlType.IsEqual<P.AsInt>>
							 .And<CSAttributeGroup.isActive.IsEqual<True>>
							 .And<CSAttributeGroup.attributeCategory.IsEqual<CSAttributeGroup.attributeCategory.variant>>>
							 .View.Select(Base, itemClassId, 2);

			foreach (var att in attributes)
			{
				AttributeDefinition def = new AttributeDefinition();

				var attribute = (CSAttribute)att;
				def.AttributeID = attribute.AttributeID.ValueField();
				def.Description = attribute.Description.ValueField();
				def.NoteID = attribute.NoteID.ValueField();
				def.Values = new List<AttributeDefinitionValue>();
				var group = att.GetItem<CSAttributeGroup>();
				def.Order = group.SortOrder.ValueField();
				var attributedetails = PXSelect<CSAttributeDetail, Where<CSAttributeDetail.attributeID, Equal<Required<CSAttributeDetail.attributeID>>>>
									   .Select(Base, def.AttributeID.Value);

				foreach (CSAttributeDetail value in attributedetails)
				{
					AttributeDefinitionValue defValue = new AttributeDefinitionValue();
					defValue.NoteID = value.NoteID.ValueField();
					defValue.ValueID = value.ValueID.ValueField();
					defValue.Description = value.Description.ValueField();
					defValue.SortOrder = value.SortOrder.ToInt().ValueField();
					def.Values.Add(defValue);
				}

				yield return new AttributeValue()
				{
					Id = attribute.NoteID,
					AttributeID = attribute.AttributeID.ValueField(),
					AttributeDescription = attribute.Description.ValueField(),
					Required = false.ValueField(),
					IsActive = false.ValueField()
				};
			}
		}

		/// <summary>
		/// When exporting, we may not have the external id at the time we map the options/values.
		/// This method must then be called after the export to update the external id in the mapping table.
		/// </summary>
		/// <param name="syncId"></param>
		/// <param name="externalId"></param>
		public virtual void UpdateExternalIdForMappings(int? syncId, string externalId)
		{
			var mappedOptions = SelectFrom<BCMatrixOptionsMapping>
								.Where<BCMatrixOptionsMapping.syncID.IsEqual<P.AsInt>>
								.View.Select(Base, syncId);
			foreach(var option in mappedOptions)
			{
				var mappedOption = option.GetItem<BCMatrixOptionsMapping>();
				if (mappedOption.ExternID == null || mappedOption.ExternID != externalId)
				{
					mappedOption.ExternID = externalId;
					Base.Caches[typeof(BCMatrixOptionsMapping)].Update(mappedOption);
				}
			}
			Base.Caches[typeof(BCMatrixOptionsMapping)].Persist(PXDBOperation.Update);
		}

		/// <summary>
		/// Updates options' mapping for a template items before the export.
		/// It will add new options / attributes to existing mapping or create new set of mappings so that we can import the template
		/// items after the export without having to remap the options.
		/// </summary>
		/// <param name="templateItem"></param>
		/// <param name="syncId"></param>
		/// <param name="externalId"></param>
		public virtual void UpdateMappingOptionsForExport(TemplateItems templateItem, int? syncId, string externalId)
		{
			var mappedOptions = SelectFrom<BCMatrixOptionsMapping>
									   .LeftJoin<BCSyncStatus>.On<BCSyncStatus.syncID.IsEqual<BCMatrixOptionsMapping.syncID>>
									   .Where<BCMatrixOptionsMapping.syncID.IsEqual<P.AsInt>>.View.Select(Base, syncId).ToList();

			var itemClass = SelectFrom<INItemClass>.Where<INItemClass.itemClassCD.IsEqual<P.AsString>>
							.View.Select(Base, templateItem.ItemClass.Value).FirstOrDefault()?.GetItem<INItemClass>();

			var itemClassId = itemClass?.ItemClassID;

			var matrixItemAttributes = templateItem.Matrix.SelectMany(matrixItem => matrixItem.Attributes);

			foreach (var attrDef in templateItem.AttributesDef)
			{
				var order = (int?)attrDef.Order?.Value;

				if (order == null)
					order = mappedOptions.FirstOrDefault(x => x.GetItem<BCMatrixOptionsMapping>().MappedAttributeID == attrDef.AttributeID.Value)?.GetItem<BCMatrixOptionsMapping>().ExternalOptionSortOrder;

				var templateItemAttributeValues = matrixItemAttributes
					.Where(attributeValue => attributeValue.AttributeID.Value == attrDef.AttributeID.Value)
					.Select(attributeValue => attributeValue.Value.Value)
					.ToHashSet();

				//Add missing values 
				foreach (var attrValue in attrDef.Values)
				{
					//look whether the value exists.
					var mappedValue = mappedOptions.FirstOrDefault(x => x.GetItem<BCMatrixOptionsMapping>().MappedAttributeID == attrDef.AttributeID.Value &&
																		x.GetItem<BCMatrixOptionsMapping>().MappedValue == attrValue.ValueID.Value);
					//add it in case it does not.
					if (mappedValue == null)
					{
						if (!templateItemAttributeValues.Contains(attrValue.ValueID.Value))
							continue;

						var newMapping = new BCMatrixOptionsMapping()
						{
							SyncID = syncId,
							ExternID = externalId,
							ExternalOptionName = attrDef.Description.Value,
							ExternalOptionValue = attrValue.Description.Value,
							ExternalOptionSortOrder = order,
							ExternalOptionValueID = attrValue.Description.Value,
							ItemClassID = itemClassId,
							MappedAttributeID = attrDef.AttributeID.Value,
							MappedValue = attrValue.ValueID.Value,
							ExternalOptionID = string.Empty
						};
						Base.Caches[typeof(BCMatrixOptionsMapping)].Insert(newMapping);
					}
					else
					{
						var mapping = mappedValue.GetItem<BCMatrixOptionsMapping>();
						mapping.ExternalOptionName = attrDef.Description.Value;
						mapping.ExternalOptionValue = attrValue.Description.Value;
						mapping.ExternalOptionValueID = attrValue.Description.Value;
						mapping.MappedAttributeID = attrDef.AttributeID.Value;
						mapping.MappedValue = attrValue.ValueID.Value;
						Base.Caches[typeof(BCMatrixOptionsMapping)].Update(mapping);
					}
				}
			}

			Base.Caches[typeof(BCMatrixOptionsMapping)].Persist(PXDBOperation.Insert);
			Base.Caches[typeof(BCMatrixOptionsMapping)].Persist(PXDBOperation.Update);
		}

		/// <summary>
		/// Updates the IsAcive field for the list of the attributes that belong to the specified <paramref name="templateItem"/>.
		/// </summary>
		/// <param name="templateItem">The template item with attributes to be updated.</param>
		/// <param name="productItem">The product item with a set of attributes.</param>
		public virtual void UpdateAttributeActivity(ProductItem templateItem, ProductItem productItem)
		{
			foreach (AttributeValue templateAttribute in templateItem.Attributes)
			{
				foreach (AttributeValue itemAttribute in productItem.Attributes)
				{
					if (templateAttribute.AttributeID.Value == itemAttribute.AttributeID.Value)
						templateAttribute.IsActive = true.ValueField();
				}
			}
		}

		/// <summary>
		/// Remap all Attributes/values to the original option/value from the external system.
		/// This method is called in the case of bidrectional sync of matrix items.
		/// In this case, we need to send back the same options used in the external system and not the the attributes names/values used in the ERP.
		/// </summary>
		/// <param name="syncid">The SyncId in the BCSyncHistory table</param>
		/// <param name="templateItems">The template item to export</param>
		[Obsolete]
		public virtual void RemapVariantOptionsForExport(int? syncid, TemplateItems templateItems) { }

		/// <summary>
		/// Remap all Attributes/values to the original option/value from the external system.
		/// This method is called in the case of bidrectional sync of matrix items.
		/// In this case, we need to send back the same options used in the external system and not the the attributes names/values used in the ERP.
		/// </summary>
		/// <param name="syncid">The SyncId in the BCSyncHistory table</param>
		/// <param name="templateItems">The template item to export</param>
		/// <param name="externalId">The external id</param>
		public virtual void RemapVariantOptionsForExport(int? syncid, TemplateItems templateItems, string externalId)
		{
			if (!syncid.HasValue)
				return;

			var mappedOptions = GetMappedOptionsForTemplate(syncid);

			if (mappedOptions == null || !mappedOptions.Any())
				return;

			foreach (var attrDef in templateItems.AttributesDef)
			{
				var id = attrDef.AttributeID.Value;
				var mappedOption = mappedOptions.FirstOrDefault(x => x.AttributeID == id);
				if (mappedOption == null)
					continue;

				attrDef.AttributeID = mappedOption.AttributeID.ValueField();
				attrDef.Description = mappedOption.OptionName.ValueField();
				foreach (var attrValue in attrDef.Values)
				{
					var mappedValue = mappedOptions.FirstOrDefault(x => x.AttributeID == id && x.Value == attrValue.ValueID.Value);
					if (mappedValue == null)
						continue;
					attrValue.ValueID = mappedValue.Value.ValueField();
					attrValue.Description = mappedValue.OptionValue.ValueField();
				}
			}

			foreach (var attr in templateItems.Attributes)
			{
				var id = attr.AttributeID.Value;
				var mappedOption = mappedOptions.FirstOrDefault(x => x.AttributeID == id);
				if (mappedOption == null)
					continue;
				attr.AttributeID = mappedOption.AttributeID.ValueField();
				attr.AttributeDescription = mappedOption.OptionName.ValueField();
			}

			foreach (var attrValue in templateItems.AttributesValues)
			{
				var mappedValue = mappedOptions.FirstOrDefault(x => x.AttributeID == attrValue.AttributeID.Value && x.Value == attrValue.Value.Value);
				if (mappedValue == null)
					continue;
				attrValue.AttributeID = mappedValue.AttributeID.ValueField();
				attrValue.Value = mappedValue.Value.ValueField();
				attrValue.ValueDescription = mappedValue.OptionValue.ValueField();
			}

			foreach (var variant in templateItems.Matrix)
			{
				if (variant.Attributes == null)
					continue;

				foreach (var attr in variant.Attributes)
				{
					var mappedValue = mappedOptions.FirstOrDefault(x => x.AttributeID == attr.AttributeID.Value && x.Value == attr.Value.Value);
					if (mappedValue == null)
						continue;
					attr.AttributeID = mappedValue.AttributeID.ValueField();
					attr.AttributeDescription = mappedValue.OptionName.ValueField();
					attr.Value = mappedValue.Value.ValueField();
					attr.ValueDescription = mappedValue.OptionValue.ValueField();
				}
			}
		}
	}
}
