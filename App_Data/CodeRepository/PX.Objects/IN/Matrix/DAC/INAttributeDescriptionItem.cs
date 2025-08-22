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
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CS;
using System;

namespace PX.Objects.IN.Matrix.DAC
{
	[PXCacheName(Messages.AttributeDescriptionItemDAC)]
	public class INAttributeDescriptionItem : PXBqlTable, IBqlTable
	{
		#region Keys
		public class PK : PrimaryKeyOf<INAttributeDescriptionItem>.By<templateID, groupID, attributeID, valueID>
		{
			public static INAttributeDescriptionItem Find(PXGraph graph, int? templateID, int? groupID, string attributeID, string valueID, PKFindOptions options = PKFindOptions.None)
				=> FindBy(graph, templateID, groupID, attributeID, valueID, options);
		}
		public static class FK
		{
			public class TemplateInventoryItem : InventoryItem.PK.ForeignKeyOf<INAttributeDescriptionItem>.By<templateID> { }
			public class Group : INAttributeDescriptionGroup.PK.ForeignKeyOf<INAttributeDescriptionItem>.By<templateID, groupID> { }
			public class Attribute : CSAttribute.PK.ForeignKeyOf<INAttributeDescriptionItem>.By<attributeID> { }
		}
		#endregion

		#region TemplateID
		public abstract class templateID : PX.Data.BQL.BqlInt.Field<templateID> { }
		/// <summary>
		/// References to Inventory Item which is template.
		/// </summary>
		[PXUIField(DisplayName = "Template Item")]
		[TemplateInventory(IsKey = true)]
		[PXParent(typeof(FK.TemplateInventoryItem))]
		public virtual int? TemplateID
		{
			get;
			set;
		}
		#endregion
		#region GroupID
		public abstract class groupID : PX.Data.BQL.BqlInt.Field<groupID> { }
		/// <summary>
		/// Identifier group of attributes
		/// </summary>
		[PXDBInt(IsKey = true)]
		public virtual int? GroupID
		{
			get;
			set;
		}
		#endregion
		#region AttributeID
		public abstract class attributeID : PX.Data.BQL.BqlString.Field<attributeID> { }
		/// <summary>
		/// Identifier of attribute
		/// </summary>
		[PXDBString(10, IsUnicode = true, IsKey = true, InputMask = "")]
		public virtual string AttributeID
		{
			get;
			set;
		}
		#endregion
		#region ValueID
		public abstract class valueID : PX.Data.BQL.BqlString.Field<valueID> { }
		/// <summary>
		/// Value of attribute
		/// </summary>
		[PXAttributeValue]
		public virtual string ValueID
		{
			get;
			set;
		}
		#endregion

		#region CreatedByID
		public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
		[PXDBCreatedByID()]
		public virtual Guid? CreatedByID
		{
			get;
			set;
		}
		#endregion
		#region CreatedByScreenID
		public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
		[PXDBCreatedByScreenID()]
		public virtual String CreatedByScreenID
		{
			get;
			set;
		}
		#endregion
		#region CreatedDateTime
		public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
		[PXDBCreatedDateTime()]
		public virtual DateTime? CreatedDateTime
		{
			get;
			set;
		}
		#endregion
		#region LastModifiedByID
		public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
		[PXDBLastModifiedByID()]
		public virtual Guid? LastModifiedByID
		{
			get;
			set;
		}
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
		[PXDBLastModifiedByScreenID()]
		public virtual String LastModifiedByScreenID
		{
			get;
			set;
		}
		#endregion
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
		[PXDBLastModifiedDateTime()]
		public virtual DateTime? LastModifiedDateTime
		{
			get;
			set;
		}
		#endregion
		#region tstamp
		public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }
		[PXDBTimestamp(VerifyTimestamp = VerifyTimestampOptions.BothFromGraphAndRecord)]
		public virtual Byte[] tstamp
		{
			get;
			set;
		}
		#endregion
	}
}
