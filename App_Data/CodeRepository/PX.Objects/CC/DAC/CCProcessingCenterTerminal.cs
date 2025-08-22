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

#nullable enable
using System;

using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CA;

namespace PX.Objects.CC
{
	/// <summary>
	/// Represents a POS Terminal set up for the proceesing center
	/// </summary>
	[PXCacheName(Messages.CCProcessingCenterTerminal)]
	[Serializable]
	public class CCProcessingCenterTerminal : PXBqlTable, IBqlTable
	{
		public class PK : PrimaryKeyOf<CCProcessingCenterTerminal>.By<CCProcessingCenterTerminal.terminalID, CCProcessingCenterTerminal.processingCenterID>
		{
			public static CCProcessingCenterTerminal Find(PXGraph graph, string terminalID, string processingCenterID) => FindBy(graph, terminalID, processingCenterID);
		}

		#region ProcessingCenterID
		public abstract class processingCenterID : PX.Data.BQL.BqlString.Field<processingCenterID> { }

		/// <summary>Processing Center ID</summary>
		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXDBDefault(typeof(CCProcessingCenter.processingCenterID))]
		[PXParent(typeof(Select<CCProcessingCenter, Where<CCProcessingCenter.processingCenterID, Equal<Current<CCProcessingCenterTerminal.processingCenterID>>>>))]
		[PXUIField(DisplayName = "Processing Center ID", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual string? ProcessingCenterID
		{
			get;
			set;
		}
		#endregion

		#region TerminalID
		public abstract class terminalID : PX.Data.BQL.BqlString.Field<terminalID> { }

		/// <summary>POS Terminal ID</summary>
		[PXDBString(36, IsUnicode = true, IsKey = true)]
		[PXDefault]
		[PXUIField(DisplayName = "Terminal ID", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual string? TerminalID
		{
			get;
			set;
		}
		#endregion

		#region TerminalName
		public abstract class terminalName : PX.Data.BQL.BqlString.Field<terminalName> { }

		/// <summary>Terminal Name</summary>
		[PXDBString(64, IsUnicode = true)]
		[PXDefault]
		[PXUIField(DisplayName = "Terminal Name", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual string? TerminalName
		{
			get;
			set;
		}
		#endregion

		#region DisplayName
		public abstract class displayName : PX.Data.BQL.BqlString.Field<displayName> { }

		/// <summary>Display Name</summary>
		[PXDBString(64, IsUnicode = true)]
		[PXDefault]
		[PXUIField(DisplayName = "Display Name", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual string? DisplayName
		{
			get;
			set;
		}
		#endregion

		#region IsActive
		public abstract class isActive : PX.Data.BQL.BqlBool.Field<isActive> { }

		/// <summary>Indicates that the terminal is active</summary>
		[PXDBBool]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Active")]
		public virtual bool? IsActive
		{
			get;
			set;
		}
		#endregion

		#region CanBeEnabled
		public abstract class canBeEnabled : PX.Data.BQL.BqlBool.Field<canBeEnabled> { }

		/// <summary>Indicates that the terminal can be set active</summary>
		[PXDBBool]
		[PXDefault(true)]
		public virtual bool? CanBeEnabled
		{
			get;
			set;
		}
		#endregion

		#region CreatedByID
		public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }

		[PXDBCreatedByID]
		public virtual Guid? CreatedByID
		{
			get;
			set;
		}
		#endregion
		#region CreatedByScreenID
		public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }

		[PXDBCreatedByScreenID]
		public virtual string CreatedByScreenID
		{
			get;
			set;
		}
		#endregion
		#region CreatedDateTime
		public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }

		[PXDBCreatedDateTime]
		public virtual DateTime? CreatedDateTime
		{
			get;
			set;
		}
		#endregion
		#region LastModifiedByID
		public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }

		[PXDBLastModifiedByID]
		public virtual Guid? LastModifiedByID
		{
			get;
			set;
		}
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }

		[PXDBLastModifiedByScreenID]
		public virtual string LastModifiedByScreenID
		{
			get;
			set;
		}
		#endregion
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }

		[PXDBLastModifiedDateTime]
		public virtual DateTime? LastModifiedDateTime
		{
			get;
			set;
		}
		#endregion
		#region tstamp
		public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }

		[PXDBTimestamp]
		public virtual byte[] tstamp
		{
			get;
			set;
		}
		#endregion
	}
}
