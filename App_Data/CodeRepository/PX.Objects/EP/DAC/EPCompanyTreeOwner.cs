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
using PX.TM;

namespace PX.Objects.EP
{
	[PXProjection(typeof(Select2<EPCompanyTree,
		LeftJoin<EPCompanyTreeMember, On<EPCompanyTreeMember.workGroupID, Equal<EPCompanyTree.workGroupID>, And<EPCompanyTreeMember.isOwner, Equal<True>>>,
		LeftJoin<EPEmployee, On<EPEmployee.defContactID, Equal<EPCompanyTreeMember.contactID>>>>>))]
    [Serializable]
    [PXHidden]
	public class EPCompanyTreeOwner : PXBqlTable, PX.Data.IBqlTable
	{
		#region WorkGroupID
		public abstract class workGroupID : PX.Data.BQL.BqlInt.Field<workGroupID> { }
		protected int? _WorkGroupID;
		[PXDBIdentity(BqlTable = typeof(EPCompanyTree))]
		[PXUIField(DisplayName = "Work Group", Enabled = false, Visibility = PXUIVisibility.Invisible)]
		public virtual int? WorkGroupID
		{
			get
			{
				return this._WorkGroupID;
			}
			set
			{
				this._WorkGroupID = value;
			}
		}
		#endregion

		#region Description
		public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
		protected String _Description;
		[PXDBString(50, IsKey = true, InputMask = "",BqlTable = typeof(EPCompanyTree))]
		[PXDefault]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				this._Description = value;
			}
		}
		#endregion

		#region ParentWGID
		public abstract class parentWGID : PX.Data.BQL.BqlInt.Field<parentWGID> { }
		protected int? _ParentWGID;
		[PXDBInt(BqlTable = typeof(EPCompanyTree))]
		[PXDefault(0)]
		[PXDBDefault(typeof(EPCompanyTree.workGroupID))]
		public virtual int? ParentWGID
		{
			get
			{
				return this._ParentWGID;
			}
			set
			{
				this._ParentWGID = value ?? 0;
			}
		}
		#endregion

		#region SortOrder
		public abstract class sortOrder : PX.Data.BQL.BqlInt.Field<sortOrder> { }
		protected Int32? _SortOrder;
		[PXDefault(0)]
		[PXDBInt(BqlTable = typeof(EPCompanyTree))]
		public virtual Int32? SortOrder
		{
			get
			{
				return this._SortOrder;
			}
			set
			{
				this._SortOrder = value;
			}
		}
		#endregion

		#region WaitTime
		public abstract class waitTime : PX.Data.BQL.BqlInt.Field<waitTime> { }
		protected Int32? _WaitTime;
		[PXDBTimeSpanLong(Format = TimeSpanFormatType.DaysHoursMinites,BqlTable = typeof(EPCompanyTree))]
		[PXDefault(0)]
		[PXUIField(DisplayName = "Wait Time")]
		public virtual Int32? WaitTime
		{
			get
			{
				return this._WaitTime;
			}
			set
			{
				this._WaitTime = value;
			}
		}
		#endregion

		#region BypassEscalation
		public abstract class bypassEscalation : PX.Data.BQL.BqlBool.Field<bypassEscalation> { }
		protected bool? _BypassEscalation;
		[PXDBBool(BqlTable = typeof(EPCompanyTree))]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Bypass Escalation")]
		public virtual bool? BypassEscalation
		{
			get
			{
				return this._BypassEscalation;
			}
			set
			{
				this._BypassEscalation = value;
			}
		}
		#endregion

		#region UseCalendarTime
		public abstract class useCalendarTime : PX.Data.BQL.BqlBool.Field<useCalendarTime> { }
		protected bool? _UseCalendarTime;
		[PXDBBool(BqlTable = typeof(EPCompanyTree))]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Use Calendar Time")]
		public virtual bool? UseCalendarTime
		{
			get
			{
				return this._UseCalendarTime;
			}
			set
			{
				this._UseCalendarTime = value;
			}
		}
		#endregion

		#region AccessRights
		public abstract class accessRights : PX.Data.BQL.BqlShort.Field<accessRights> { }
		protected short? _AccessRights;
		[PXDBShort(BqlTable = typeof(EPCompanyTree))]
		[PXDefault((short)0)]
		public short? AccessRights
		{
			get
			{
				return this._AccessRights;
			}
			set
			{
				this._AccessRights = value;
			}
		}
		#endregion

		#region UserID
		public abstract class userID : PX.Data.BQL.BqlGuid.Field<userID> { }
		protected Guid? _UserID ;
		[PXDBGuid(BqlField = typeof(EPEmployee.userID))]
		[PXUIField(DisplayName = "User ID")]
		public virtual Guid? UserID
		{
			get
			{
				return this._UserID;
			}
			set
			{
				this._UserID = value;
			}
		}
		#endregion

		#region OwnerID
		public abstract class ownerID : PX.Data.BQL.BqlInt.Field<ownerID> { }
		protected int? _OwnerID;
		[PXDBInt(BqlField = typeof(EPEmployee.bAccountID))]
		[PXEPEmployeeSelector]
		[PXUIField(DisplayName = "Owner ID", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual int? OwnerID
		{
			get
			{
				return this._OwnerID;
			}
			set
			{
				this._OwnerID = value;
			}
		}
		#endregion
	}


}
