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
using System;

namespace PX.Objects.PM.DAC
{
	[PXCacheName(Messages.PMReportRowsMultiplier)]
	[Serializable()]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public partial class PMReportRowsMultiplier : PXBqlTable, IBqlTable
	{
		#region ID
		public abstract class id : PX.Data.BQL.BqlLong.Field<id>
		{
		}
		protected Int32? _RecordID;
		[PXUIField(DisplayName = "RecordID", Visible = false, Enabled = false)]
		[PXDBIdentity(IsKey = true)]
		public virtual Int32? RecordID
		{
			get
			{
				return this._RecordID;
			}
			set
			{
				this._RecordID = value;
			}
		}
		#endregion

		#region RowsCount
		public abstract class rowsCount : PX.Data.BQL.BqlInt.Field<rowsCount> { }
		protected Int32? _RowsCount;
		[PXDBInt]
		public virtual Int32? RowsCount
		{
			get
			{
				return this._RowsCount;
			}
			set
			{
				this._RowsCount = value;
			}
		}
		#endregion

		#region RowsCount
		public abstract class rowNumber : PX.Data.BQL.BqlInt.Field<rowNumber> { }
		protected Int32? _RowNumber;
		[PXDBInt]
		public virtual Int32? RowNumber
		{
			get
			{
				return this._RowNumber;
			}
			set
			{
				this._RowNumber = value;
			}
		}
		#endregion
	}
}
