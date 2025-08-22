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

namespace PX.Objects.PM
{
	using System;
	using PX.Data;
	
	[System.SerializableAttribute()]
	[PXCacheName(Messages.PMAllocationAuditTran)]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public partial class PMAllocationAuditTran : PXBqlTable, PX.Data.IBqlTable
	{
		#region AllocationID
		public abstract class allocationID : PX.Data.BQL.BqlString.Field<allocationID> { }
		protected String _AllocationID;
		[PXDBString(PMAllocation.allocationID.Length, IsKey = true, IsUnicode = true)]
		[PXDefault()]
		public virtual String AllocationID
		{
			get
			{
				return this._AllocationID;
			}
			set
			{
				this._AllocationID = value;
			}
		}
		#endregion
		#region TranID
		public abstract class tranID : PX.Data.BQL.BqlLong.Field<tranID> { }
		protected Int64? _TranID;
		[PXDBLong(IsKey = true)]
		[PXDBChildIdentity(typeof(PMTran.tranID))]
		public virtual Int64? TranID
		{
			get
			{
				return this._TranID;
			}
			set
			{
				this._TranID = value;
			}
		}
		#endregion
		#region SourceTranID
		public abstract class sourceTranID : PX.Data.BQL.BqlLong.Field<sourceTranID> { }
		protected Int64? _SourceTranID;
		[PXDBLong(IsKey = true)]
		[PXDBChildIdentity(typeof(PMTran.tranID))]
		public virtual Int64? SourceTranID
		{
			get
			{
				return this._SourceTranID;
			}
			set
			{
				this._SourceTranID = value;
			}
		}
		#endregion
		
		
	}
}
