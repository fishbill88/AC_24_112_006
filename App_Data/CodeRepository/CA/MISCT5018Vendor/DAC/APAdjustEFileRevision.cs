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
	/// The transaction reference that is used to create a row for the T5018 revision.
	/// </summary>
	[Serializable]
	[PXCacheName("APAdjust EFileRevision")]
	public class APAdjustEFileRevision : PXBqlTable, IBqlTable
	{
		#region Keys
		public class PK : PrimaryKeyOf<APAdjustEFileRevision>.By<adjgDocType, adjgRefNbr, adjNbr, adjdDocType, adjdRefNbr, adjdLineNbr, revision> {
			public static APAdjustEFileRevision Find(PXGraph graph, string adjgDocType, string adjgRefNbr, int adjNbr, string adjdDocType, string adjdRefNbr, int adjdLineNbr, string revision) =>
				FindBy(graph, adjgDocType, adjgRefNbr, adjNbr, adjdDocType, adjdRefNbr, adjdLineNbr, revision);
		}

		public static class FK {
			public class T5018MasterTable : Localizations.CA.T5018MasterTable.PK.ForeignKeyOf<APAdjustEFileRevision>.By<organizationID, year, revision> { }

			public class APAdjust : PX.Objects.AP.APAdjust.PK.ForeignKeyOf<APAdjustEFileRevision>.By<adjgDocType, adjgRefNbr, adjNbr, adjdDocType, adjdRefNbr, adjdLineNbr> { }
		}
		#endregion

		#region AdjgDocType
		public abstract class adjgDocType : BqlString.Field<adjgDocType> { }
		/// <summary>
		/// The type of the adjusting document.
		/// </summary>
		[PXDBString(3, IsKey = true, IsFixed = true)]
		[PXDefault]
		[PXParent(typeof(FK.T5018MasterTable))]
		public virtual string AdjgDocType
		{
			get;
			set;
		}
		#endregion

		#region AdjgRefNbr
		public abstract class adjgRefNbr : PX.Data.BQL.BqlString.Field<adjgRefNbr> { }
		/// <summary>
		/// The reference number of the adjusting document.
		/// </summary>
		[PXDBString(15, IsKey = true)]
		[PXDefault()]
		public virtual String AdjgRefNbr
		{
			get;
			set;
		}
		#endregion

		#region AdjNbr
		public abstract class adjNbr : PX.Data.BQL.BqlInt.Field<adjNbr> { }
		/// <summary>
		/// The number of the adjustment.
		/// </summary>
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public virtual Int32? AdjNbr { get; set; }
		#endregion

		#region AdjdDocType
		public abstract class adjdDocType : PX.Data.BQL.BqlString.Field<adjdDocType> { }
		/// <summary>
		/// The type of the adjusted document.
		/// </summary>
		[PXDBString(3, IsKey = true)]
		[PXDefault()]
		public virtual String AdjdDocType { get; set; }
		#endregion

		#region AdjdRefNbr
		public abstract class adjdRefNbr : PX.Data.BQL.BqlString.Field<adjdRefNbr> { }
		/// <summary>
		/// The reference number of the adjusted document.
		/// </summary>
		[PXDBString(15, IsKey = true)]
		[PXDefault()]
		public virtual string AdjdRefNbr { get; set; }
		#endregion

		#region AdjdLineNbr
		public abstract class adjdLineNbr : PX.Data.BQL.BqlInt.Field<adjdLineNbr> { }
		/// <summary>
		/// The line number of the adjusted amount.
		/// </summary>
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public virtual int? AdjdLineNbr
		{
			get;
			set;
		}
		#endregion

		#region OrganizationID
		public abstract class organizationID : BqlInt.Field<organizationID> { }
		/// <summary>
		/// The organization ID of the associated organization.
		/// </summary>
		/// <value>
		/// The value of this field corresponds to the value of the <see cref="Organization.OrganizationID"/> field.
		/// </value>
		[PXDBInt]
		[PXDefault()]
		public virtual int? OrganizationID
		{
			get;
			set;
		}
		#endregion

		#region Year
		public abstract class year : BqlString.Field<year> { };
		/// <summary>
		/// The year of the revision.
		/// </summary>
		[PXDBString(4)]
		[PXDefault()]
		public virtual string Year
		{
			get;
			set;
		}
		#endregion

		#region Revision
		public abstract class revision : BqlString.Field<revision> { }
		/// <summary>
		/// The revision number.
		/// </summary>
		[PXDBString(3, IsKey = true)]
		[PXDefault()]
		public virtual string Revision
		{
			get;
			set;
		}
		#endregion

		#region Include in Report
		public abstract class includeInReport : BqlBool.Field<includeInReport> { }

		/// <summary>
		/// Saved value of <see cref="APPaymentExt.IncludeInT5018Report"/> field.
		/// </summary>
		[PXDBBool]
		[PXDefault()]
		public virtual bool? IncludeInReport { get; set; }
		#endregion

		#region T5018 Service
		public abstract class t5018Service : BqlBool.Field<t5018Service> { }

		/// <summary>
		/// Saved value of <see cref="APTranExt.T5018Service"/> field.
		/// </summary>
		[PXDBBool]
		[PXDefault()]
		public virtual bool? T5018Service { get; set; }
		#endregion

		#region Voided
		public abstract class voided : BqlBool.Field<voided> { }
		/// <summary>
		/// Flag for voided payments
		/// </summary>
		[PXDBBool]
		[PXDefault()]
		public virtual bool? Voided { get; set; }
		#endregion
	}
}
