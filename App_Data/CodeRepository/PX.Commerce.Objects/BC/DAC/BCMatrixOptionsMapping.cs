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
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.Common;
using PX.Commerce.Core;

namespace PX.Commerce.Objects
{

	/// <summary>
	/// Represents the mapping of Matrix Items external options to the ERP attributes.
	/// </summary>
	[Serializable]
	[PXCacheName("Matrix Options Mapping")]
	public class BCMatrixOptionsMapping : PXBqlTable, IBqlTable
	{
		#region Keys
		public class PK : PrimaryKeyOf<BCMatrixOptionsMapping>.By<optionMappingID>
		{
			public static BCMatrixOptionsMapping Find(PXGraph graph, int? optionMappingId, PKFindOptions options = PKFindOptions.None) => FindBy(graph, optionMappingId, options);
		}
		public static class FK
		{
			public class SyncStatus : BCSyncStatus.PK.ForeignKeyOf<BCMatrixOptionsMapping>.By<syncID> { }
		}

		public class UK : PrimaryKeyOf<BCMatrixOptionsMapping>.By<syncID>
		{
			public static BCMatrixOptionsMapping Find(PXGraph graph, int? syncId, PKFindOptions options = PKFindOptions.None) => FindBy(graph, syncId, options);
		}

		#endregion
		#region OptionMappingID
		/// <summary>
		/// Identity of the record
		/// </summary>
		[PXDBIdentity(IsKey = true)]
		public virtual int? OptionMappingID { get; set; }
		/// <inheritdoc cref="OptionMappingID" />
		public abstract class optionMappingID : BqlType<IBqlInt, int>.Field<optionMappingID> { }
		#endregion

		/// <summary>
		/// Sync Id of entity in cref="BCSyncStatus"
		/// </summary>
		[PXDBInt]
		[PXUIField(DisplayName = "Sync ID")]
		[PXDBDefault(typeof(BCSyncStatus.syncID))]
		[PXParent(typeof(Select<BCSyncStatus, Where<BCSyncStatus.syncID, Equal<Current<syncID>>>>))]
		public virtual int? SyncID { get; set; }

		/// <inheritdoc cref="SyncID" />
		public abstract class syncID : BqlType<IBqlInt, int>.Field<syncID> { }

		#region ExternID
		/// <summary>
		/// The unique identifier of the item in the external system.
		/// The property is a key field.
		/// </summary>
		[PXDBString(128, IsUnicode = true)]
		[PXUIField(DisplayName = "External ID", Enabled = false)]
		public virtual string ExternID { get; set; }
		/// <inheritdoc cref="ExternID" />
		public abstract class externID : BqlType<IBqlString, string>.Field<externID> { }
		#endregion

		#region ItemClassID
		/// <summary>
		/// The Item Class Id used for the mapping
		/// </summary>
		[PXDBInt]
		[PXUIField(DisplayName = "ERP Item Class")]
		[PXDimensionSelector(INItemClass.Dimension, typeof(Search<INItemClass.itemClassID>), typeof(INItemClass.itemClassCD), DescriptionField = typeof(INItemClass.descr), CacheGlobal = true)]
		public virtual int? ItemClassID { get; set; }
		public abstract class itemClassID : PX.Data.BQL.BqlInt.Field<itemClassID> { }
		#endregion

		#region ExternalOptionID
		/// <summary>
		/// The option id in the external system
		/// </summary>
		[PXDBString(IsUnicode = true)]
		public virtual string ExternalOptionID { get; set; }
		/// <inheritdoc cref="ExternalOptionID" />
		public abstract class externalOptionID : BqlType<IBqlString, string>.Field<externalOptionID> { }
		#endregion

		#region External Option Sort Order
		/// <summary>
		/// Sort order of the option.
		/// </summary>
		[PXDBInt]		
		public virtual int? ExternalOptionSortOrder { get; set; }
		public abstract class externalOptionSortOrder : BqlType<IBqlInt, int>.Field<externalOptionSortOrder> { }
		#endregion

		#region ExternalOptionName
		/// <summary>
		/// The option name in the external system
		/// </summary>
		[PXDBString(IsUnicode = true)]
		[PXUIField(DisplayName = "External Option", Enabled = false)]
		public virtual string ExternalOptionName { get; set; }
		/// <inheritdoc cref="ExternalOptionName" />
		public abstract class externalOptionName : BqlType<IBqlString, string>.Field<externalOptionName> { }
		#endregion

		#region External Value
		/// <summary>
		/// The original value from the external system
		/// </summary>
		[PXDBString(IsUnicode = true)]
		[PXUIField(DisplayName = "External Option Value", Enabled = false)]
		public virtual string ExternalOptionValue { get; set; }
		public abstract class externalOptionValue : BqlType<IBqlString, string>.Field<externalOptionValue> { }
		#endregion

		#region External Value Id
		/// <summary>
		/// The original value from the external system
		/// </summary>
		[PXDBString(IsUnicode = true)]
		public virtual string ExternalOptionValueID { get; set; }
		public abstract class externalOptionValueID : BqlType<IBqlString, string>.Field<externalOptionValueID> { }
		#endregion

		#region MappedAttributeID
		/// <summary>
		/// The mapped attribute id
		/// </summary>
		[PXDBString(IsUnicode = true)]
		[PXUIField(DisplayName = "ERP Attribute")]
		[PXSelector(typeof(
			SearchFor<CSAttributeGroup.attributeID>.
			Where<
				CSAttributeGroup.entityClassID.IsEqual<PX.Data.BQL.RTrim<Use<itemClassID.FromCurrent>.AsString>>.
				And<CSAttributeGroup.entityType.IsEqual<PX.Objects.Common.Constants.DACName<InventoryItem>>>.
				And<CSAttributeGroup.attributeCategory.IsEqual<CSAttributeGroup.attributeCategory.variant>>>),
				new Type[] { typeof(CSAttributeGroup.description) },
				DescriptionField = typeof(CSAttributeGroup.description))]
		[PXRestrictor(typeof(Where<CSAttributeGroup.isActive.IsEqual<True>>), PX.Objects.IN.Messages.AttributeIsInactive, typeof(CSAttributeGroup.attributeID))]
		public virtual string MappedAttributeID { get; set; }
		public abstract class mappedAttributeID : BqlType<IBqlString, string>.Field<mappedAttributeID> { }
		#endregion

		#region MappedValue
		/// <summary>
		/// The mapped value in the ERP
		/// </summary>
		[PXDBString(IsUnicode = true)]
		[PXUIField(DisplayName = "ERP Attribute Value")]
		[PXSelector(typeof(SearchFor<CSAttributeDetail.valueID>
						  .Where<CSAttributeDetail.attributeID.IsEqual<PX.Data.BQL.RTrim<Use<mappedAttributeID.FromCurrent>.AsString>>
						  .And<CSAttributeDetail.disabled.IsEqual<False>>>),
						  new Type[] { typeof(CSAttributeDetail.description) },
						  DescriptionField =typeof(CSAttributeDetail.description))]
		public virtual string MappedValue { get; set; }
		public abstract class mappedValue : BqlType<IBqlString, string>.Field<mappedValue> { }
		#endregion

		#region Selected
		/// <summary>
		/// Indicates if the this record is selected.
		/// </summary>
		[PXBool]
		[PXUIField(DisplayName = "Selected")]
		public virtual bool? Selected { get; set; }

		/// <inheritdoc cref="Selected"/>
		public abstract class selected : BqlType<IBqlBool, bool>.Field<selected> { }
		#endregion

		#region CreatedByID
		/// <summary>
		/// The ID of the account that created this record.
		/// </summary>
		[PXDBCreatedByID]
		public virtual Guid? CreatedByID { get; set; }
		/// <inheritdoc cref="CreatedByID"/>
		public abstract class createdByID : BqlType<IBqlGuid, Guid>.Field<createdByID> { }
		#endregion
		#region CreatedByScreenID
		/// <summary>
		/// The ID of the Screen that created this record.
		/// </summary>
		[PXDBCreatedByScreenID]
		public virtual String CreatedByScreenID { get; set; }
		/// <inheritdoc cref="CreatedByScreenID"/>
		public abstract class createdByScreenID : BqlType<IBqlString, string>.Field<createdByScreenID> { }
		#endregion	
		#region CreatedDateTime
		/// <summary>
		/// The Time this record was created
		/// </summary>
		[PXDBCreatedDateTime]
		[PXUIField(DisplayName = PXDBLastModifiedByIDAttribute.DisplayFieldNames.CreatedDateTime, Enabled = false, IsReadOnly = true)]
		public virtual DateTime? CreatedDateTime { get; set; }
		/// <inheritdoc cref="CreatedDateTime"/>
		public abstract class createdDateTime : BqlType<IBqlDateTime, DateTime>.Field<createdDateTime> { }
		#endregion
		#region LastModifiedByID
		/// <summary>
		/// The ID of the account that last modified this record.
		/// </summary>
		[PXDBLastModifiedByID]
		public virtual Guid? LastModifiedByID { get; set; }
		/// <inheritdoc cref="LastModifiedByID"/>
		public abstract class lastModifiedByID : BqlType<IBqlGuid, Guid>.Field<lastModifiedByID> { }
		#endregion
		#region LastModifiedByScreenID
		/// <summary>
		/// The ID of the screen that last modified this record.
		/// </summary>
		[PXDBLastModifiedByScreenID]
		public virtual String LastModifiedByScreenID { get; set; }
		/// <inheritdoc cref="LastModifiedByScreenID"/>
		public abstract class lastModifiedByScreenID : BqlType<IBqlString, string>.Field<lastModifiedByScreenID> { }
		#endregion
		#region LastModifiedDateTime
		/// <summary>
		/// The Time this record was last modified.
		/// </summary>
		[PXDBLastModifiedDateTime]
		[PXUIField(DisplayName = PXDBLastModifiedByIDAttribute.DisplayFieldNames.LastModifiedDateTime, Enabled = false, IsReadOnly = true)]
		public virtual DateTime? LastModifiedDateTime { get; set; }
		/// <inheritdoc cref="LastModifiedDateTime"/>
		public abstract class lastModifiedDateTime : BqlType<IBqlDateTime, DateTime>.Field<lastModifiedDateTime> { }
		#endregion
	}	
}
