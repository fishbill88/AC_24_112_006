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

namespace PX.Objects.GL
{
	using System;
	using PX.Data;
	using PX.Data.ReferentialIntegrity.Attributes;

	[System.SerializableAttribute()]
	[PXCacheName(Messages.GLAllocationAccountHistory)]
	public partial class GLAllocationAccountHistory : PXBqlTable, PX.Data.IBqlTable
	{
		#region Keys
		public class PK : PrimaryKeyOf<GLAllocationAccountHistory>.By<module, batchNbr, branchID, accountID, subID>
		{
			public static GLAllocationAccountHistory Find(PXGraph graph, String module, String batchNbr, Int32? branchID, Int32? accountID, Int32? subID, PKFindOptions options = PKFindOptions.None) => FindBy(graph, module, batchNbr, branchID, accountID, subID, options);
		}
		public static class FK
		{
			public class Branch : GL.Branch.PK.ForeignKeyOf<GLAllocationAccountHistory>.By<branchID> { }
			public class Batch : GL.Batch.PK.ForeignKeyOf<GLAllocationAccountHistory>.By<module, batchNbr> { }
			public class Account : GL.Account.PK.ForeignKeyOf<GLAllocationAccountHistory>.By<accountID> { }
			public class Subaccount : GL.Sub.PK.ForeignKeyOf<GLAllocationAccountHistory>.By<subID> { }
			public class ContraAccount : GL.Account.PK.ForeignKeyOf<GLAllocationAccountHistory>.By<contrAccontID> { }
			public class ContraSubaccount : GL.Sub.PK.ForeignKeyOf<GLAllocationAccountHistory>.By<contrSubID> { }
			public class SourceLedger : GL.Ledger.PK.ForeignKeyOf<GLAllocationAccountHistory>.By<sourceLedgerID> { }
		}
		#endregion

		#region Module
		public abstract class module : PX.Data.BQL.BqlString.Field<module> { }
		protected String _Module;
		[PXDBString(2, IsKey = true, IsFixed = true)]
		[PXDBDefault(typeof(Batch))]
		public virtual String Module
		{
			get
			{
				return this._Module;
			}
			set
			{
				this._Module = value;
			}
		}
		#endregion
		#region BatchNbr
		public abstract class batchNbr : PX.Data.BQL.BqlString.Field<batchNbr> { }
		protected String _BatchNbr;
		[PXDBString(15, IsUnicode = true, IsKey = true)]
		[PXDBDefault(typeof(Batch))]
		[PXParent(typeof(Select<Batch, Where<Batch.batchNbr, Equal<Current<GLAllocationAccountHistory.batchNbr>>, And<Batch.module, Equal<Current<GLAllocationAccountHistory.module>>>>>))]
		public virtual String BatchNbr
		{
			get
			{
				return this._BatchNbr;
			}
			set
			{
				this._BatchNbr = value;
			}
		}
		#endregion
        #region BranchID
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        protected Int32? _BranchID;
        [Branch(IsKey = true)]
        public virtual Int32? BranchID
        {
            get
            {
                return this._BranchID;
            }
            set
            {
                this._BranchID = value;
            }
        }
        #endregion
		#region AccountID
		public abstract class accountID : PX.Data.BQL.BqlInt.Field<accountID> { }
		protected Int32? _AccountID;
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public virtual Int32? AccountID
		{
			get
			{
				return this._AccountID;
			}
			set
			{
				this._AccountID = value;
			}
		}
		#endregion
		#region SubID
		public abstract class subID : PX.Data.BQL.BqlInt.Field<subID> { }
		protected Int32? _SubID;
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public virtual Int32? SubID
		{
			get
			{
				return this._SubID;
			}
			set
			{
				this._SubID = value;
			}
		}
		#endregion
		#region AllocatedAmount
		public abstract class allocatedAmount : PX.Data.BQL.BqlDecimal.Field<allocatedAmount> { }
		protected Decimal? _AllocatedAmount;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		public virtual Decimal? AllocatedAmount
		{
			get
			{
				return this._AllocatedAmount;
			}
			set
			{
				this._AllocatedAmount = value;
			}
		}
		#endregion
		#region PriorPeriodsAllocAmount
		public abstract class priorPeriodsAllocAmount : PX.Data.BQL.BqlDecimal.Field<priorPeriodsAllocAmount> { }
		protected Decimal? _PriorPeriodsAllocAmount;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? PriorPeriodsAllocAmount
		{
			get
			{
				return this._PriorPeriodsAllocAmount;
			}
			set
			{
				this._PriorPeriodsAllocAmount = value;
			}
		}
		#endregion
		#region ContrAccontID
		public abstract class contrAccontID : PX.Data.BQL.BqlInt.Field<contrAccontID> { }
		protected Int32? _ContrAccontID;
		[PXDBInt()]
		public virtual Int32? ContrAccontID
		{
			get
			{
				return this._ContrAccontID;
			}
			set
			{
				this._ContrAccontID = value;
			}
		}
		#endregion
		#region ContrSubID
		public abstract class contrSubID : PX.Data.BQL.BqlInt.Field<contrSubID> { }
		protected Int32? _ContrSubID;
		[PXDBInt()]
		public virtual Int32? ContrSubID
		{
			get
			{
				return this._ContrSubID;
			}
			set
			{
				this._ContrSubID = value;
			}
		}
		#endregion
		#region SourceLedgerID
		public abstract class sourceLedgerID : PX.Data.BQL.BqlInt.Field<sourceLedgerID> { }
		protected Int32? _SourceLedgerID;
		[PXDBInt()]
		public virtual Int32? SourceLedgerID
		{
			get
			{
				return this._SourceLedgerID;
			}
			set
			{
				this._SourceLedgerID = value;
			}
		}
		#endregion
	}
}
