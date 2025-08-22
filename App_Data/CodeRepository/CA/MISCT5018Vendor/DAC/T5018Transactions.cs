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
using PX.Data.ReferentialIntegrity.Attributes;
using System;

namespace PX.Objects.Localizations.CA
{
	/// <summary>
	/// The transaction reference that will be used for creation T5018 report.
	/// </summary>
	[Serializable]
	[PXCacheName("T5018Transactions")]
	public class T5018Transactions : PXBqlTable, IBqlTable
	{
		#region Keys
		public class PK : PrimaryKeyOf<T5018Transactions>.By<branchID, vendorID, docDate, docType, refNbr>
		{
			public static T5018Transactions Find(PXGraph graph, int branchID, int vendorID, DateTime docDate, string docType, string refNbr) =>
				FindBy(graph, branchID, vendorID, docDate, docType, refNbr);
		}
		#endregion

		#region BranchID
		public abstract class branchID : BqlInt.Field<branchID> { }
		/// <summary>
		/// The branch ID of associated payments.
		/// </summary>
		[PXDBInt(IsKey = true)]
		public virtual int? BranchID { get; set; }
		#endregion

		#region VendorID
		public abstract class vendorID : BqlInt.Field<vendorID> { }
		/// <summary>
		/// The vendor ID of associated payments.
		/// </summary>
		[PXDBInt(IsKey = true)]
		public virtual int? VendorID { get; set; }
		#endregion

		#region DocDate
		public abstract class docDate : BqlDateTime.Field<docDate> { }
		/// <summary>
		/// Either the date when the adjusted document was created or the date of the original vendor's document.
		/// </summary>
		[PXDBDate(IsKey = true)]
		public virtual DateTime? DocDate { get; set; }
		#endregion

		#region DocType
		public abstract class docType : BqlString.Field<docType> { }
		/// <summary>
		/// The type of the adjusted document.
		/// </summary>
		[PXDBString(3, IsKey = true, IsFixed = true)]
		public virtual String DocType { get; set; }
		#endregion

		#region RefNbr
		public new abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
		/// <summary>
		/// Reference number of the document.
		/// </summary>
		[PXDBString(15, IsKey = true)]
		public virtual String RefNbr { get; set; }
		#endregion

		#region CreatedDateTime
		public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
		[PXDBCreatedDateTime()]
		public virtual DateTime? CreatedDateTime { get; set; }
		#endregion

		#region tstamp
		public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }
		protected Byte[] _tstamp;
		[PXDBTimestamp()]
		public virtual Byte[] tstamp
		{
			get
			{
				return this._tstamp;
			}
			set
			{
				this._tstamp = value;
			}
		}
		#endregion

	}
}
