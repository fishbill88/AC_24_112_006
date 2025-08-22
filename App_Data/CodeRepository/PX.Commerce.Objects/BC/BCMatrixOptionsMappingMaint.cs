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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Windows.Forms;
using PX.Api;
using PX.Commerce.Core;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.SQLTree;
using PX.Objects.Common;
using PX.Objects.CS;
using PX.Objects.IN;


namespace PX.Commerce.Objects
{

	/// <summary>
	/// Filter for the Matrix Options Mapping screen
	/// </summary>
	[Serializable]
	[PXCacheName("Mapping Update Filter")]
	public class MappingUpdateFilter : PXBqlTable, IBqlTable
	{
		#region ItemClassID
		/// <summary>
		/// The Item Class Id used for the mapping
		/// </summary>
		[PXInt]
		[PXUIField(DisplayName = "Item Class to Assign")]
		[PXSelector(typeof(Search<INItemClass.itemClassID>),
						SubstituteKey = typeof(INItemClass.itemClassCD),
						DescriptionField = typeof(INItemClass.descr))]
		public virtual int? ItemClassID { get; set; }
		public abstract class itemClassID : PX.Data.BQL.BqlInt.Field<itemClassID> { }
		#endregion	
	}

	public class BCMatrixOptionsMappingMaint: PXGraph<BCMatrixOptionsMappingMaint, BCMatrixOptionsMapping>
	{
		#region Views

		[PXNotCleanable]
		public PXFilter<MappingUpdateFilter> MasterView;

		public SelectFrom<BCMatrixOptionsMapping>
			  .InnerJoin<BCSyncStatus>.On<BCSyncStatus.syncID.IsEqual<BCMatrixOptionsMapping.syncID>>
			  .Where<BCSyncStatus.status.IsNotEqual<BCSyncStatusAttribute.synchronized>>
			  .OrderBy<BCSyncStatus.externDescription.Asc, BCMatrixOptionsMapping.syncID.Asc, BCMatrixOptionsMapping.externalOptionName.Asc, BCMatrixOptionsMapping.externalOptionValue.Asc>
			  .View OptionMappings;

		public SelectFrom<INItemClass>.View ItemClasses;
		#endregion

		public BCMatrixOptionsMappingMaint()
		{
			OptionMappings.Cache.AllowInsert = false;
			OptionMappings.Cache.AllowDelete = false;		
		}

		#region Actions
		public PXSave<MappingUpdateFilter> Save;
		public PXCancel<MappingUpdateFilter> Cancel;
		#endregion

		#region View Attributes button
		public PXAction<MappingUpdateFilter> ViewAttributes;
		[PXButton(Category = BCConstants.Other, DisplayOnMainToolbar = true)]
		[PXUIField(DisplayName = "View Attributes")]
		public IEnumerable viewAttributes(PXAdapter adapter)
		{
			var graph = CreateInstance<CSAttributeMaint>();
			throw new PXPopupRedirectException(graph, null);
		}
		#endregion

		#region Update Selected Button
		public PXAction<MappingUpdateFilter> UpdateItemClass;
		[PXButton(Category = BCConstants.Processing, DisplayOnMainToolbar = true, Connotation = Data.WorkflowAPI.ActionConnotation.Success)]
		[PXUIField(DisplayName = "Update Item Class")]
		public IEnumerable updateItemClass(PXAdapter adapter)
		{			
			var classId = MasterView.Current.ItemClassID;

			if (!classId.HasValue)
				return adapter.Get();

			var row = OptionMappings.Current;
			if (row == null)
				return adapter.Get();

			CascadeUpdateItemClassId(row, classId);
			
			return adapter.Get();			
		}
		#endregion

		#region Events

		protected virtual void _(Events.FieldUpdating<BCMatrixOptionsMapping, BCMatrixOptionsMapping.mappedAttributeID> e)
		{
			if (e.NewValue == null && e.OldValue!=null)
			{
				ClearMappedAttribute(e.Row, e.OldValue as string);
				return;
			}

			e.Row.MappedValue = null;

			string newValue = e.NewValue as string;

			if (newValue == null) return;

			//Check all similar.
			var rowsFromSameTemplate = SelectFrom<BCMatrixOptionsMapping>
										.Where<BCMatrixOptionsMapping.syncID.IsEqual<P.AsInt>										
										.And<BCMatrixOptionsMapping.externalOptionID.IsEqual<P.AsString>>>
										.View.Select(this, e.Row.SyncID, e.Row.ExternalOptionID);

			var attributeValues = SelectFrom<CSAttributeDetail>
										.Where<CSAttributeDetail.attributeID.IsEqual<P.AsString>
										.And<CSAttributeDetail.disabled.IsNotEqual<True>>>
										.View.Select(this, newValue).Select(x => x.GetItem<CSAttributeDetail>()).ToList();
		
			foreach (var row in rowsFromSameTemplate)
			{
				var line = row.GetItem<BCMatrixOptionsMapping>();

				var mappedValue = attributeValues.Where(x => string.Equals(line.ExternalOptionValue, x.ValueID, StringComparison.OrdinalIgnoreCase) ||
														 string.Equals(line.ExternalOptionValue, x.Description, StringComparison.OrdinalIgnoreCase) ||
														 string.Equals(line.ExternalOptionValueID, x.ValueID, StringComparison.OrdinalIgnoreCase) ||
														 string.Equals(line.ExternalOptionValueID, x.Description, StringComparison.OrdinalIgnoreCase)).FirstOrDefault()?.ValueID;

				if (line.MappedAttributeID != newValue || (line.MappedValue == null && mappedValue != null))
				{
					line.MappedAttributeID = newValue;
					line.MappedValue = mappedValue;
					base.Caches[typeof(BCMatrixOptionsMapping)].Update(line);
				}
			}

			OptionMappings.View.RequestRefresh();
		}

		protected virtual void _(Events.FieldUpdating<BCMatrixOptionsMapping, BCMatrixOptionsMapping.itemClassID> e)
		{
			if (e.NewValue == null && e.OldValue!=null)
			{
				ClearItemClassID(e.Row);
				return;
			}

			var newId = GetClassIDByItemCD((string)e.NewValue);

			if (newId == null || (e.OldValue!=null && (int)e.OldValue == newId))
				return;

			CascadeUpdateItemClassId(e.Row, newId);

			if (OptionMappings.Cache.Updated.Any_())
				OptionMappings.View.RequestRefresh();
		}

		#endregion

		public virtual int? GetClassIDByItemCD(string cd)
		{
			var itemClass = SelectFrom<INItemClass>
								  .Where<INItemClass.itemClassCD.IsEqual<P.AsString>>
								 .View.Select(this, cd).FirstOrDefault()?.GetItem<INItemClass>();
			
			return itemClass?.ItemClassID.Value;
		}

		public virtual void ClearItemClassID(BCMatrixOptionsMapping row)
		{
			var sameSyncId = SelectFrom<BCMatrixOptionsMapping>
									.Where<BCMatrixOptionsMapping.syncID.IsEqual<P.AsInt>>									
									.View.Select(this, row.SyncID);			

			foreach (var item in sameSyncId)
			{
				var mappingRow = item.GetItem<BCMatrixOptionsMapping>();
				mappingRow.ItemClassID = null;
				mappingRow.MappedAttributeID = mappingRow.MappedValue = null;
				OptionMappings.Update(mappingRow);
			}

			OptionMappings.View.RequestRefresh();
		}

		public virtual void ClearMappedAttribute(BCMatrixOptionsMapping row, string oldValue)
		{
			var sameSyncId = SelectFrom<BCMatrixOptionsMapping>
									.Where<BCMatrixOptionsMapping.syncID.IsEqual<P.AsInt>
									.And<BCMatrixOptionsMapping.mappedAttributeID.IsEqual<P.AsString>>>
									.View.Select(this, row.SyncID, oldValue).Select(x => x.GetItem<BCMatrixOptionsMapping>()).ToList();	

			foreach (var mappedRow in sameSyncId)
			{
				mappedRow.MappedAttributeID = mappedRow.MappedValue = null;
				OptionMappings.Update(mappedRow);
			}

			OptionMappings.View.RequestRefresh();
		}


		/// <summary>
		/// Updates the classitem for the current row and all the rows with the same syncID.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="itemClassId"></param>
		public virtual void CascadeUpdateItemClassId(BCMatrixOptionsMapping row, int? itemClassId)
		{
			if (row == null || itemClassId == null)
				return;
	
			var sameSyncId = SelectFrom<BCMatrixOptionsMapping>
									.Where<BCMatrixOptionsMapping.syncID.IsEqual<P.AsInt>>								
									.View.Select(this, row.SyncID);

			var matchingAttributes = SelectFrom<CSAttributeGroup>
						.InnerJoin<CSAttribute>.On<CSAttributeGroup.attributeID.IsEqual<CSAttribute.attributeID>>
						.InnerJoin<CSAttributeDetail>.On<CSAttributeDetail.attributeID.IsEqual<CSAttributeGroup.attributeID>>
						.InnerJoin<BCMatrixOptionsMapping>
							.On<Brackets<BCMatrixOptionsMapping.externalOptionName.IsEqual<CSAttribute.attributeID>
									.Or<BCMatrixOptionsMapping.externalOptionName.IsEqual<CSAttribute.description>>>>							  
						.Where<CSAttributeGroup.entityClassID.IsEqual<P.AsString>
						.And<BCMatrixOptionsMapping.syncID.IsEqual<P.AsInt>
						.And<CSAttributeDetail.disabled.IsEqual<False>
						.And<CSAttributeGroup.entityType.IsEqual<PX.Objects.Common.Constants.DACName<InventoryItem>>
						.And<CSAttributeGroup.attributeCategory.IsEqual<CSAttributeGroup.attributeCategory.variant>>>>>>
						.View.Select(this, itemClassId.Value, row.SyncID).ToList();

			var flat = matchingAttributes.Select(x => new { optionId = x.GetItem<BCMatrixOptionsMapping>().ExternalOptionID,
															optionValue = x.GetItem<BCMatrixOptionsMapping>().ExternalOptionValue,
															attributeId = x.GetItem<CSAttributeGroup>().AttributeID,
															attributeValue = x.GetItem<CSAttributeDetail>().ValueID,
															attributeDescription = x.GetItem<CSAttributeDetail>().Description }).ToList();

			foreach (var optionMapping in sameSyncId)
			{
				var mappingRow = optionMapping.GetItem<BCMatrixOptionsMapping>();

				mappingRow.ItemClassID = itemClassId.Value;

				var matchingAttribute = flat.FirstOrDefault(x => x.optionId == mappingRow.ExternalOptionID);
				if (matchingAttribute != null)
				{
					var matchingAttributeValue = flat.FirstOrDefault(x => x.optionId == mappingRow.ExternalOptionID
											  && (string.Equals(mappingRow.ExternalOptionValue, x.attributeValue, StringComparison.OrdinalIgnoreCase) ||
											  string.Equals(mappingRow.ExternalOptionID, x.attributeValue, StringComparison.OrdinalIgnoreCase) ||
											  string.Equals(mappingRow.ExternalOptionValue, x.attributeDescription, StringComparison.OrdinalIgnoreCase) ||
											  string.Equals(mappingRow.ExternalOptionID, x.attributeDescription, StringComparison.OrdinalIgnoreCase)));
					mappingRow.MappedAttributeID = matchingAttribute?.attributeId.Trim();
					mappingRow.MappedValue = matchingAttributeValue?.attributeValue;
				}
				OptionMappings.Update(mappingRow);
			}
		}
	}
}
