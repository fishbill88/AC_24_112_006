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
using PX.Objects.CS;
using PX.Objects.IN.Matrix.GraphExtensions;
using PX.Objects.IN.Matrix.Graphs;
using PX.Objects.IN.Matrix.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PX.Objects.IN.GraphExtensions.INItemClassMaintExt
{
	public class VariantAttributesExt : PXGraphExtension<IN.INItemClassMaint>
	{
		protected Lazy<bool> _hasTemplateWithChild;
		protected Lazy<bool> _childItemClassHasTemplateWithItems;

		public override void Initialize()
		{
			_hasTemplateWithChild = new Lazy<bool>(HasTemplateWithChild);
			base.Initialize();
		}

		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.matrixItem>();
		}
		
		protected virtual void _(Events.RowSelected<CSAttributeGroup> eventArgs)
		{
			if (eventArgs.Row == null)
				return;

			PXUIFieldAttribute.SetEnabled<CSAttributeGroup.attributeCategory>(eventArgs.Cache, eventArgs.Row,
				eventArgs.Row.ControlType == CSAttribute.Combo);

			PXUIFieldAttribute.SetEnabled<CSAttributeGroup.required>(eventArgs.Cache, eventArgs.Row,
				eventArgs.Row.AttributeCategory != CSAttributeGroup.attributeCategory.Variant);
		}

		protected virtual void _(Events.FieldUpdated<CSAttributeGroup, CSAttributeGroup.attributeID> eventArgs)
		{
			eventArgs.Cache.SetDefaultExt<CSAttributeGroup.attributeCategory>(eventArgs.Row);
		}

		protected virtual void _(Events.FieldUpdated<CSAttributeGroup, CSAttributeGroup.attributeCategory> eventArgs)
		{
			if (eventArgs.Row?.AttributeCategory == CSAttributeGroup.attributeCategory.Variant)
			{
				eventArgs.Row.Required = false;
			}
		}

		protected virtual void _(Events.RowPersisting<CSAttributeGroup> eventArgs)
		{
			var row = eventArgs.Row;
			if (row == null)
				return;

			var command = eventArgs.Operation & PXDBOperation.Command;

			switch(command)
			{
				case PXDBOperation.Insert:
					ValidateInsert(eventArgs.Cache, _hasTemplateWithChild, row);
					break;
				case PXDBOperation.Update:
					ValidateUpdate(eventArgs.Cache, _hasTemplateWithChild, row);
					break;
				case PXDBOperation.Delete:
					ValidateDelete(eventArgs.Cache, _hasTemplateWithChild, row, true);
					break;
			}
		}

		protected virtual void ValidateInsert(PXCache cache, Lazy<bool> hasTemplateWithChild, CSAttributeGroup row, bool throwException = false)
		{
			if (row.AttributeCategory == CSAttributeGroup.attributeCategory.Variant && row.IsActive == true && hasTemplateWithChild.Value)
			{
				var exception = new PXSetPropertyException<CSAttributeGroup.attributeCategory>(Messages.CantAddVariantAttributeForMatrixItem, row.AttributeID);

				if (!throwException)
					cache.RaiseExceptionHandling<CSAttributeGroup.attributeCategory>(row, row.AttributeCategory, exception);
				else
					throw exception;
			}
		}
		
		protected virtual void ValidateUpdate(PXCache cache, Lazy<bool> hasTemplateWithChild, CSAttributeGroup row, bool throwException = false, CSAttributeGroup oldRow = null)
		{
			string templateIDs = string.Empty;

			if (oldRow == null)
				oldRow = CSAttributeGroup.PK.Find(Base, row.AttributeID, row.EntityClassID, row.EntityType);

			if (oldRow != null && oldRow.AttributeCategory != row.AttributeCategory &&
				row.IsActive == true && (hasTemplateWithChild.Value || IsAttributeDefaultRowColumnAttribute(row, out templateIDs)))
			{
				var exception = hasTemplateWithChild.Value ?
					new PXSetPropertyException<CSAttributeGroup.attributeCategory>(Messages.CantChangeAttributeCategoryForMatrixItem) :
					new PXSetPropertyException<CSAttributeGroup.attributeCategory>(Messages.CantChangeAttributeCategoryForMatrixTemplate, templateIDs);

				if (!throwException)
					cache.RaiseExceptionHandling<CSAttributeGroup.attributeCategory>(row, row.AttributeCategory, exception);
				else
					throw exception;
			}

			if (oldRow?.AttributeCategory == CSAttributeGroup.attributeCategory.Variant &&
				oldRow.IsActive != row.IsActive && (hasTemplateWithChild.Value || IsAttributeDefaultRowColumnAttribute(row, out templateIDs)))
			{
				var exception = hasTemplateWithChild.Value ?
					new PXSetPropertyException<CSAttributeGroup.isActive>(Messages.CantChangeAttributeIsActiveFlagForMatrixItem) :
					new PXSetPropertyException<CSAttributeGroup.isActive>(Messages.CantChangeAttributeIsActiveFlagForMatrixTemplate, templateIDs);

				if (!throwException)
					cache.RaiseExceptionHandling<CSAttributeGroup.isActive>(row, row.IsActive, exception);
				else
					throw exception;
			}
		}

		protected virtual void ValidateDelete(PXCache cache, Lazy<bool> hasTemplateWithChild, CSAttributeGroup row, bool throwException = false)
		{
			string templateIDs = string.Empty;

			var oldRow = CSAttributeGroup.PK.Find(Base, row.AttributeID, row.EntityClassID, row.EntityType);

			if (oldRow?.AttributeCategory == CSAttributeGroup.attributeCategory.Variant && oldRow.IsActive == true &&
				(hasTemplateWithChild.Value || IsAttributeDefaultRowColumnAttribute(row, out templateIDs)))
			{
				var exception = hasTemplateWithChild.Value ?
					new PXSetPropertyException<CSAttributeGroup.attributeCategory>(Messages.CantDeleteVariantAttributeForMatrixItem, row.AttributeID) :
					new PXSetPropertyException<CSAttributeGroup.attributeCategory>(Messages.CantDeleteVariantAttributeForMatrixTemplate, row.AttributeID, templateIDs);

				if (!throwException)
					cache.RaiseExceptionHandling<CSAttributeGroup.attributeCategory>(row, row.AttributeCategory, exception);
				else
					throw exception;
			}
		}

		protected virtual bool IsAttributeDefaultRowColumnAttribute(CSAttributeGroup attributeGroup, out string templateIDs)
		{
			const int MaxTemplates = 10;
			const string Separator = ", ";

			if (attributeGroup?.EntityType != typeof(InventoryItem).FullName)
			{
				templateIDs = null;
				return false;
			}

			PXResultset<InventoryItem> template = PXSelect<InventoryItem,
				Where<InventoryItem.isTemplate, Equal<True>, And<InventoryItem.itemClassID, Equal<Required<CSAttributeGroup.entityClassID>>,
				And<Where<InventoryItem.defaultColumnMatrixAttributeID, Equal<Required<CSAttributeGroup.attributeID>>,
					Or<InventoryItem.defaultRowMatrixAttributeID, Equal<Required<CSAttributeGroup.attributeID>>>>>>>>
				.SelectWindowed(Base, 0, MaxTemplates, attributeGroup.EntityClassID, attributeGroup.AttributeID, attributeGroup.AttributeID);

			if (template.Count == 0)
			{
				templateIDs = null;
				return false;
			}

			templateIDs = string.Join(Separator, template.RowCast<InventoryItem>().Select(s => s.InventoryCD));

			return true;
		}

		protected virtual void _(Events.RowDeleting<CSAttributeGroup> eventArgs)
		{
			string templateIDs = string.Empty;

			var row = eventArgs.Row;
			if (row == null)
				return;

			var oldRow = CSAttributeGroup.PK.Find(Base, row.AttributeID, row.EntityClassID, row.EntityType);
			if (oldRow?.AttributeCategory == CSAttributeGroup.attributeCategory.Variant)
			{
				if (_hasTemplateWithChild.Value)
					throw new PXSetPropertyException<CSAttributeGroup.attributeCategory>(Messages.CantDeleteVariantAttributeForMatrixItem, row.AttributeID);

				if (IsAttributeDefaultRowColumnAttribute(row, out templateIDs))
					throw new PXSetPropertyException<CSAttributeGroup.attributeCategory>(Messages.CantDeleteVariantAttributeForMatrixTemplate, row.AttributeID, templateIDs);
			}
		}

		protected virtual bool HasTemplateWithChild(int? itemClassID, bool clearQueryCache)
		{
			var childItemSelect = new PXSelectReadonly<InventoryItem,
				Where<InventoryItem.itemClassID, Equal<Required<IN.InventoryItem.itemClassID>>,
					And<InventoryItem.isTemplate, Equal<False>,
					And<InventoryItem.templateItemID, IsNotNull>>>>(Base);

			if (clearQueryCache)
				childItemSelect.Cache.ClearQueryCache();

			InventoryItem childItem = childItemSelect.SelectSingle(itemClassID);

			return childItem != null;
		}

		protected virtual bool HasTemplateWithChild()
			=> HasTemplateWithChild(Base.itemclass.Current.ItemClassID, true);

		[PXOverride]
		public virtual void MergeAttributes(INItemClass child, IEnumerable<CSAttributeGroup> attributesTemplate,
			Action<INItemClass, IEnumerable<CSAttributeGroup>> baseMethod)
		{
			int? childItemClassID = child.ItemClassID;
			_childItemClassHasTemplateWithItems = new Lazy<bool>(() => HasTemplateWithChild(childItemClassID, false));

			baseMethod(child, attributesTemplate);
		}

		[PXOverride]
		public virtual void MergeAttribute(INItemClass child, CSAttributeGroup existingAttribute, CSAttributeGroup attr,
			Action<INItemClass, CSAttributeGroup, CSAttributeGroup> baseMethod)
		{
			if (existingAttribute == null)
			{
				ValidateInsert(Base.Mapping.Cache, _childItemClassHasTemplateWithItems, attr, true);
			}
			else
			{
				ValidateUpdate(Base.Mapping.Cache, _childItemClassHasTemplateWithItems, attr, true, existingAttribute);
			}

			baseMethod.Invoke(child, existingAttribute, attr);
		}

		/// <summary>
		/// Overrides <see cref="INItemClassMaint.Persist()"/>
		/// </summary>
		[PXOverride]
		public virtual void Persist(Action basePersist)
		{
			PXCache groupCache = Base.Mapping.Cache;

			bool orderOfVariantsChanged = groupCache.Updated.RowCast<CSAttributeGroup>().Any(g =>
				g.AttributeCategory == CSAttributeGroup.attributeCategory.Variant &&
				g.SortOrder != (short?)groupCache.GetValueOriginal<CSAttributeGroup.sortOrder>(g));

			basePersist?.Invoke();

			if (orderOfVariantsChanged && _hasTemplateWithChild.Value)
				RecalcAttributeDescriptionGroup(Base.itemclass.Current.ItemClassID);
		}

		public virtual void RecalcAttributeDescriptionGroup(int? itemClassID)
		{
			PXLongOperation.StartOperation(Base, delegate ()
			{
				bool hasErrors = false;

				var newGraph = PXGraph.CreateInstance<TemplateInventoryItemMaint>();
				var varianAttributesExt = newGraph.FindImplementation<CreateMatrixItemsTabExt>();
				var attributeGroupHelper = varianAttributesExt.GetAttributeGroupHelper(newGraph);

				var templates = SelectFrom<InventoryItem>
					.Where<InventoryItem.isTemplate.IsEqual<True>.And<InventoryItem.itemClassID.IsEqual<@P.AsInt>>>
					.View.ReadOnly.Select(newGraph, itemClassID);

				foreach (InventoryItem template in templates)
				{
					try
					{
						newGraph.Clear();
						attributeGroupHelper.Recalculate(template);
						newGraph.Save.Press();
					}
					catch(Exception e)
					{
						hasErrors = true;
						PXTrace.WriteError(Messages.TemplateCouldNotUpdateAttributeDescriptionGroup, template.InventoryCD.Trim());
						PXTrace.WriteError(e);
					}
				}

				if (hasErrors)
					throw new PXException(Messages.ItemClassCouldNotUpdateAttributeDescriptionGroup);
			});
		}
	}
}
