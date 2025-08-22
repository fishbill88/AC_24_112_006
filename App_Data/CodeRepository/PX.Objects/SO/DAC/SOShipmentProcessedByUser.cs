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
using PX.Data.ReferentialIntegrity.Attributes;
using PX.SM;

namespace PX.Objects.SO
{
	[PXCacheName("SO Shipment Processed by User", PXDacType.History)]
	public class SOShipmentProcessedByUser : PXBqlTable, IBqlTable
	{
		#region Keys
		public class PK : PrimaryKeyOf<SOShipmentProcessedByUser>.By<recordID>
		{
			public static SOShipmentProcessedByUser Find(PXGraph graph, int? recordID, PKFindOptions options = PKFindOptions.None) => FindBy(graph, recordID, options);
		}
		public static class FK
		{
			public class Shipment : SOShipment.PK.ForeignKeyOf<SOShipmentProcessedByUser>.By<shipmentNbr> { }
			public class Worksheet : SOPickingWorksheet.PK.ForeignKeyOf<SOShipmentProcessedByUser>.By<worksheetNbr> { }
			public class Picker : SOPicker.PK.ForeignKeyOf<SOShipmentProcessedByUser>.By<worksheetNbr, pickerNbr> { }
			public class User : Users.PK.ForeignKeyOf<SOShipmentProcessedByUser>.By<userID> { }
		}
		#endregion

		#region RecordID
		[PXDBIdentity(IsKey = true)]
		public virtual Int32? RecordID { get; set; }
		public abstract class recordID : BqlInt.Field<recordID> { }
		#endregion
		#region JobType
		// Acuminator disable once PX1007 NoXmlCommentForPublicEntityOrDacProperty to be documented later
		[PXDBString(4, IsFixed = true, IsUnicode = false)]
		[PXDefault]
		[jobType.List]
		[PXUIField(DisplayName = "Operation Type", Enabled = false)]
		public virtual String JobType { get; set; }
		public abstract class jobType : BqlString.Field<jobType>
		{
			public const string Pick = "PICK";
			public const string Pack = "PACK";
			public const string PackOnly = "PPCK";

			[PX.Common.PXLocalizable]
			public static class DisplayNames
			{
				public const string Pick = "Pick";
				public const string Pack = "Pack";
				public const string PackOnly = "Pack Only";
			}

			public class ListAttribute : PXStringListAttribute
			{
				public ListAttribute() : base
				(
					Pair(Pick, DisplayNames.Pick),
					Pair(Pack, DisplayNames.Pack),
					Pair(PackOnly, DisplayNames.PackOnly)
				) { }
			}

			public class pick : BqlString.Constant<pick> { public pick() : base(Pick) { } }
			public class pack : BqlString.Constant<pack> { public pack() : base(Pack) { } }
			public class packOnly : BqlString.Constant<packOnly> { public packOnly() : base(PackOnly) { } }
		}
		#endregion
		#region DocType
		// Acuminator disable once PX1007 NoXmlCommentForPublicEntityOrDacProperty to be documented later
		[PXDBString(4, IsFixed = true, IsUnicode = false)]
		[PXDefault]
		[docType.List]
		public virtual String DocType { get; set; }
		public abstract class docType : BqlString.Field<docType>
		{
			public const string PickList = "PLST";
			public const string Shipment = "SHPT";

			[PX.Common.PXLocalizable]
			public static class DisplayNames
			{
				public const string PickList = "Pick List";
				public const string Shipment = "Shipment";
			}

			public class ListAttribute : PXStringListAttribute
			{
				public ListAttribute() : base
				(
					Pair(PickList, DisplayNames.PickList),
					Pair(Shipment, DisplayNames.Shipment)
				)
				{ }
			}

			public class pickList : BqlString.Constant<pickList> { public pickList() : base(PickList) { } }
			public class shipment : BqlString.Constant<shipment> { public shipment() : base(Shipment) { } }
		}
		#endregion

		#region ShipmentNbr
		[PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXParent(typeof(FK.Shipment))]
		public virtual String ShipmentNbr { get; set; }
		public abstract class shipmentNbr : BqlString.Field<shipmentNbr> { }
		#endregion
		#region WorksheetNbr
		// Acuminator disable once PX1007 NoXmlCommentForPublicEntityOrDacProperty to be documented later
		[PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXParent(typeof(FK.Worksheet))]
		public virtual String WorksheetNbr { get; set; }
		public abstract class worksheetNbr : BqlString.Field<worksheetNbr> { }
		#endregion
		#region PickerNbr
		// Acuminator disable once PX1007 NoXmlCommentForPublicEntityOrDacProperty to be documented later
		[PXDBInt]
		[PXParent(typeof(FK.Picker))]
		public virtual Int32? PickerNbr { get; set; }
		public abstract class pickerNbr : BqlInt.Field<pickerNbr> { }
		#endregion
		#region PickListNbr
		// Acuminator disable once PX1007 NoXmlCommentForPublicEntityOrDacProperty to be documented later
		[PXString]
		[PXUIField(DisplayName = "Pick List Nbr.", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[PXFormula(typeof(
			SOShipment.shipmentNbr.FromParent.
			IfNullThen<SOPickingWorksheet.singleShipmentNbr.FromParent>.
			IfNullThen<SOPicker.pickListNbr.FromParent>))]
		public virtual String PickListNbr { get; set; }
		public abstract class pickListNbr : BqlString.Field<pickListNbr> { }
		#endregion

		#region UserID
		[PXDBGuid]
		[PXDefault(typeof(AccessInfo.userID))]
		[PXParent(typeof(FK.User))]
		public virtual Guid? UserID { get; set; }
		public abstract class userID : BqlGuid.Field<userID> { }
		#endregion
		#region Confirmed
		[PXDBBool]
		[PXDefault(false)]
		public bool? Confirmed { get; set; }
		public abstract class confirmed : BqlBool.Field<confirmed> { }
		#endregion
		#region NumberOfScans
		/// <summary>
		/// The number of all the scans done by user for the pick list.
		/// </summary>
		[PXDBInt]
		[PXDefault(0)]
		public virtual int? NumberOfScans { get; set; }
		public abstract class numberOfScans : BqlInt.Field<numberOfScans> { }
		#endregion
		#region NumberOfFailedScans
		/// <summary>
		/// The number of scans that lead to errors.
		/// </summary>
		[PXDBInt]
		[PXDefault(0)]
		public virtual int? NumberOfFailedScans { get; set; }
		public abstract class numberOfFailedScans : BqlInt.Field<numberOfFailedScans> { }
		#endregion
		#region OverallStartDateTime
		// Acuminator disable once PX1007 NoXmlCommentForPublicEntityOrDacProperty to be documented later
		[PXDBDateAndTime]
		public virtual DateTime? OverallStartDateTime { get; set; }
		public abstract class overallStartDateTime : BqlDateTime.Field<overallStartDateTime> { }
		#endregion
		#region StartDateTime
		[PXDBDateAndTime]
		public virtual DateTime? StartDateTime { get; set; }
		public abstract class startDateTime : BqlDateTime.Field<startDateTime> { }
		#endregion
		#region LastModifiedDate
		[PXDBDateAndTime]
		public virtual DateTime? LastModifiedDateTime { get; set; }
		public abstract class lastModifiedDateTime : BqlDateTime.Field<lastModifiedDateTime> { }
		#endregion
		#region EndDateTime
		[PXDBDateAndTime]
		public virtual DateTime? EndDateTime { get; set; }
		public abstract class endDateTime : BqlDateTime.Field<endDateTime> { }
		#endregion
		#region OverallEndDateTime
		// Acuminator disable once PX1007 NoXmlCommentForPublicEntityOrDacProperty to be documented later
		[PXDBDateAndTime]
		public virtual DateTime? OverallEndDateTime { get; set; }
		public abstract class overallEndDateTime : BqlDateTime.Field<overallEndDateTime> { }
		#endregion
	}
}
