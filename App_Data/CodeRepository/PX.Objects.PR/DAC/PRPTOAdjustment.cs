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
using PX.Data.BQL.Fluent;
using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data.WorkflowAPI;
using PX.Objects.CS;
using System;

namespace PX.Objects.PR
{
	/// <summary>
	/// Stores the PTO adjustment information.
	/// </summary>
	[PXCacheName(Messages.PRPTOAdjustment)]
	[Serializable]
	[PXPrimaryGraph(typeof(PRPTOAdjustmentMaint))]
	public class PRPTOAdjustment : PXBqlTable, IBqlTable
	{
		#region Keys
		public class PK : PrimaryKeyOf<PRPTOAdjustment>.By<type, refNbr>
		{
			public static PRPTOAdjustment Find(PXGraph graph, string type, string refNbr) =>
				FindBy(graph, type, refNbr);
		}
		#endregion

		#region Events
		public class Events : PXEntityEvent<PRPTOAdjustment>.Container<Events>
		{
			public PXEntityEvent<PRPTOAdjustment> VoidingAdjustmentReleased;
		}
		#endregion

		#region Type
		public abstract class type : PX.Data.BQL.BqlString.Field<type> { }
		/// <summary>
		/// The type of the PTO adjustment.
		/// </summary>
		/// <value>
		/// The field can have one of the values described in <see cref="PTOAdjustmentType.ListAttribute"/>.
		/// </value>
		[PXDBString(3, IsKey = true, IsFixed = true)]
		[PXDefault(PTOAdjustmentType.Adjustment)]
		[PXUIField(DisplayName = "Type")]
		[PTOAdjustmentType.List]
		[PXFieldDescription]
		public virtual string Type { get; set; }
		#endregion

		#region RefNbr
		public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
		/// <summary>
		/// The user-friendly unique identifier of the PTO Adjustment.
		/// </summary>
		[PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(SearchFor<PRPTOAdjustment.refNbr>.Where<PRPTOAdjustment.type.IsEqual<PRPTOAdjustment.type.FromCurrent>>),
			typeof(type), typeof(refNbr), typeof(status))]
		[AutoNumber(typeof(PRPTOAdjustment.type), typeof(PRPTOAdjustment.date),
			new string[] { PTOAdjustmentType.Adjustment, PTOAdjustmentType.VoidingAdjustment }, new Type[] { typeof(PRSetup.ptoAdjustmentNumberingCD), null })]
		[PXFieldDescription]
		public virtual string RefNbr { get; set; }
		#endregion

		#region Status
		public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
		/// <summary>
		/// The status of the PTO adjustment.
		/// </summary>
		/// <value>
		/// The field can have one of the values described in <see cref="PTOAdjustmentStatus.ListAttribute"/>.
		/// </value>
		[PXDBString(3, IsFixed = true)]
		[PXUIField(DisplayName = "Status", Enabled = false, Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(PTOAdjustmentStatus.New)]
		[PTOAdjustmentStatus.List]
		public virtual string Status { get; set; }
		#endregion

		#region Date
		public abstract class date : PX.Data.BQL.BqlDateTime.Field<date> { }
		/// <summary>
		/// The date when PTO adjustment was created and added to the database.
		/// It is NOT the Business Date that can be set in the future or in the past causing unpredictable behaviour for PTO banks.
		/// </summary>
		[PXDBDate]
		[PXDefault]
		[PXUIField(DisplayName = "Date", Enabled = false)]
		public DateTime? Date { get; set; }
		#endregion

		#region Description
		public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
		/// <summary>
		/// The description of the PTO adjustment.
		/// </summary>
		[PXDBString(255, IsUnicode = true)]
		[PXUIField(DisplayName = "Description")]
		public virtual string Description { get; set; }
		#endregion


		#region System Columns
		#region TStamp
		[PXDBTimestamp]
		public virtual byte[] TStamp { get; set; }
		public abstract class tStamp : PX.Data.BQL.BqlByteArray.Field<tStamp> { }
		#endregion

		#region CreatedByID
		[PXDBCreatedByID()]
		public virtual Guid? CreatedByID { get; set; }
		public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
		#endregion

		#region CreatedByScreenID
		[PXDBCreatedByScreenID()]
		public virtual string CreatedByScreenID { get; set; }
		public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
		#endregion

		#region CreatedDateTime
		[PXDBCreatedDateTime()]
		public virtual DateTime? CreatedDateTime { get; set; }
		public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
		#endregion

		#region LastModifiedByID
		[PXDBLastModifiedByID()]
		public virtual Guid? LastModifiedByID { get; set; }
		public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
		#endregion

		#region LastModifiedByScreenID
		[PXDBLastModifiedByScreenID()]
		public virtual string LastModifiedByScreenID { get; set; }
		public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
		#endregion

		#region LastModifiedDateTime
		[PXDBLastModifiedDateTime()]
		public virtual DateTime? LastModifiedDateTime { get; set; }
		public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
		#endregion
		#endregion System Columns
	}
}
