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
using PX.Data.Update.ExchangeService;
using PX.Objects.AR;
using PX.Objects.CR;
using System;
using System.Collections.Generic;

namespace PX.Objects.FS
{
	/// <exclude/>
	#region PXProjection
	[Serializable]
	[PXProjection(typeof(Select2<FSAppointment,
			InnerJoin<FSAppointmentStatusColor, On<FSAppointmentStatusColor.statusID.IsEqual<FSAppointment.status>>>>
	))]
	#endregion

	[PXCacheName(TX.TableName.SchedulerAppointment)]
	public class SchedulerAppointment : PXBqlTable, IBqlTable
	{
		#region AppointmentID
		public abstract class appointmentID : PX.Data.BQL.BqlInt.Field<appointmentID> { }

		[PXDBIdentity(BqlField = typeof(FSAppointment.appointmentID))]
		public virtual int? AppointmentID { get; set; }
		#endregion

		#region RefNbr
		public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }

		[PXDBString(20, IsKey = true, IsUnicode = true, InputMask = "CCCCCCCCCCCCCCCCCCCC", BqlField = typeof(FSAppointment.refNbr))]
		[PXUIField(DisplayName = "Appointment Nbr.", Visible = true, Enabled = true)]
		[PX.Data.EP.PXFieldDescription]
		[PXSelector(typeof(
			Search2<FSAppointment.refNbr,
				LeftJoin<FSServiceOrder,
					On<FSServiceOrder.sOID, Equal<FSAppointment.sOID>>,
				LeftJoin<Customer,
					On<Customer.bAccountID, Equal<FSServiceOrder.customerID>>,
				LeftJoin<Location,
					On<Location.bAccountID, Equal<FSServiceOrder.customerID>,
						And<Location.locationID, Equal<FSServiceOrder.locationID>>>>>>,
				Where2<
				Where<
					FSAppointment.srvOrdType, Equal<Optional<FSAppointment.srvOrdType>>>,
					And<Where<
						Customer.bAccountID, IsNull,
						Or<Match<Customer, Current<AccessInfo.userName>>>>>>,
				OrderBy<
					Desc<FSAppointment.refNbr>>>),
			new Type[] {
						typeof(FSAppointment.refNbr),
						typeof(Customer.acctCD),
						typeof(Customer.acctName),
						typeof(Location.locationCD),
						typeof(FSAppointment.docDesc),
						typeof(FSAppointment.status),
						typeof(FSAppointment.scheduledDateTimeBegin)
			})]
		public virtual string RefNbr { get; set; }
		#endregion

		#region SrvOrdType
		public abstract class srvOrdType : PX.Data.BQL.BqlString.Field<srvOrdType> { }

		[PXDBString(4, IsFixed = true, IsKey = true, InputMask = ">AAAA", BqlField = typeof(FSAppointment.srvOrdType))]
		[PXUIField(DisplayName = "Service Order Type")]
		[FSSelectorSrvOrdTypeNOTQuote]
		[PXRestrictor(typeof(Where<FSSrvOrdType.active, Equal<True>>), null)]
		[PX.Data.EP.PXFieldDescription]
		public virtual string SrvOrdType { get; set; }
		#endregion

		#region SOID
		public abstract class sOID : PX.Data.BQL.BqlInt.Field<sOID> { }

		[PXDBInt(BqlField = typeof(FSAppointment.sOID))]
		public virtual int? SOID { get; set; }
		#endregion

		#region Status
		public abstract class status : PX.Data.BQL.BqlString.Field<status>
		{
			public abstract class Values : ListField.AppointmentStatus { }
		}

		[PXDBString(1, IsFixed = true, BqlField = typeof(FSAppointment.status))]
		[status.Values.List]
		[PXUIField(DisplayName = "Appointment Status", Enabled = false)]
		public virtual string Status { get; set; }
		#endregion

		#region Closed
		public abstract class closed : PX.Data.BQL.BqlBool.Field<closed> { }

		[PXDBBool(BqlField = typeof(FSAppointment.closed))]
		[PXUIField(DisplayName = "Closed")]
		public virtual bool? Closed { get; set; }
		#endregion

		#region Canceled
		public abstract class canceled : PX.Data.BQL.BqlBool.Field<canceled> { }

		[PXDBBool(BqlField = typeof(FSAppointment.canceled))]
		[PXUIField(DisplayName = "Canceled")]
		public virtual bool? Canceled { get; set; }
		#endregion

		#region Completed
		public abstract class completed : PX.Data.BQL.BqlBool.Field<completed> { }

		[PXDBBool(BqlField = typeof(FSAppointment.completed))]
		[PXUIField(DisplayName = "Completed")]
		public virtual bool? Completed { get; set; }
		#endregion

		#region Billed
		public abstract class billed : PX.Data.BQL.BqlBool.Field<billed> { }

		[PXDBBool(BqlField = typeof(FSAppointment.billed))]
		[PXUIField(DisplayName = "Billed")]
		public virtual bool? Billed { get; set; }
		#endregion

		#region Confirmed
		public abstract class confirmed : PX.Data.BQL.BqlBool.Field<confirmed> { }

		[PXDBBool(BqlField = typeof(FSAppointment.confirmed))]
		[PXUIField(DisplayName = "Confirmed")]
		public virtual bool? Confirmed { get; set; }
		#endregion

		#region ValidatedByDispatcher
		public abstract class validatedByDispatcher : PX.Data.BQL.BqlBool.Field<validatedByDispatcher> { }

		[PXDBBool(BqlField = typeof(FSAppointment.validatedByDispatcher))]
		[PXUIField(DisplayName = "Validated by Dispatcher")]
		public virtual bool? ValidatedByDispatcher { get; set; }
		#endregion

		#region ScheduledDateTimeBegin
		public abstract class scheduledDateTimeBegin : PX.Data.BQL.BqlDateTime.Field<scheduledDateTimeBegin> { }

		[PXDBDateAndTime(UseTimeZone = true, PreserveTime = true, DisplayNameDate = "Start Date", DisplayNameTime = "Start Time", BqlField = typeof(FSAppointment.scheduledDateTimeBegin))]
		[PXUIField(DisplayName = "Scheduled Start Date")]
		public virtual DateTime? ScheduledDateTimeBegin { get; set; }
		#endregion

		#region ScheduledDateTimeEnd
		public abstract class scheduledDateTimeEnd : PX.Data.BQL.BqlDateTime.Field<scheduledDateTimeEnd> { }

		[PXDBDateAndTime(UseTimeZone = true, PreserveTime = true, DisplayNameDate = "End Date", DisplayNameTime = "End Time", BqlField = typeof(FSAppointment.scheduledDateTimeEnd))]
		[PXUIField(DisplayName = "Scheduled End Date")]
		public virtual DateTime? ScheduledDateTimeEnd { get; set; }
		#endregion

		#region DocDesc
		public abstract class docDesc : PX.Data.BQL.BqlString.Field<docDesc> { }

		[PXDBString(255, BqlField = typeof(FSAppointment.docDesc))]
		[PXUIField(DisplayName = "Description")]
		public virtual string DocDesc { get; set; }
		#endregion

		#region LongDescr
		public abstract class longDescr : PX.Data.BQL.BqlString.Field<longDescr> { }

		[PXDBString(int.MaxValue, IsUnicode = true, BqlField = typeof(FSAppointment.longDescr))]
		[PXUIField(DisplayName = "Other")]
		public virtual string LongDescr { get; set; }
		#endregion

		#region MapLatitude
		public abstract class mapLatitude : PX.Data.BQL.BqlDecimal.Field<mapLatitude> { }

		[PXDBDecimal(6, BqlField = typeof(FSAppointment.mapLatitude))]
		[PXUIField(DisplayName = "Latitude", Enabled = false)]
		public virtual decimal? MapLatitude { get; set; }
		#endregion

		#region MapLongitude
		public abstract class mapLongitude : PX.Data.BQL.BqlDecimal.Field<mapLongitude> { }

		[PXDBDecimal(6, BqlField = typeof(FSAppointment.mapLongitude))]
		[PXUIField(DisplayName = "Longitude", Enabled = false)]
		public virtual decimal? MapLongitude { get; set; }
		#endregion

		#region EstimatedDurationTotal
		public abstract class estimatedDurationTotal : PX.Data.BQL.BqlInt.Field<estimatedDurationTotal> { }

		[PXDBTimeSpanLong(Format = TimeSpanFormatType.LongHoursMinutes, BqlField = typeof(FSAppointment.estimatedDurationTotal))]
		[PXUIField(DisplayName = "Estimated Duration", Enabled = false)]
		public virtual int? EstimatedDurationTotal { get; set; }
		#endregion

		#region StaffCntr
		public abstract class staffCntr : PX.Data.BQL.BqlInt.Field<staffCntr> { }

		[PXDBInt(BqlField = typeof(FSAppointment.staffCntr))]
		[PXUIField(DisplayName = "Multiple Staff Members", Enabled = false, Visible = true)]
		public virtual Int32? StaffCntr { get; set; }
		#endregion

		#region CreatedDateTime
		public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }

		[PXDBCreatedDateTime(BqlField = typeof(FSAppointment.createdDateTime))]
		[PXUIField(DisplayName = "Created On")]
		public virtual DateTime? CreatedDateTime { get; set; }
		#endregion

		#region IsVisible
		[PXDBBool(BqlField = typeof(FSAppointmentStatusColor.isVisible))]
		public virtual bool? IsVisible { get; set; }
		public abstract class isVisible : PX.Data.BQL.BqlBool.Field<isVisible> { }
		#endregion

		#region BandColor
		public abstract class bandColor : PX.Data.BQL.BqlString.Field<bandColor> { }

		[PXDBString(7, IsUnicode = true, InputMask = "C<AAAAAA", BqlField = typeof(FSAppointmentStatusColor.bandColor))]
		public virtual string BandColor { get; set; }
		#endregion

		#region Locked
		public new abstract class locked : PX.Data.BQL.BqlBool.Field<locked> { }

		[PXBool]
		[PXUIField(DisplayName = "Locked", Enabled = false, Visible = true)]
		[PXFormula(typeof(IIf<Where<closed, Equal<True>,
			Or<completed, Equal<True>,
			Or<canceled, Equal<True>,
			Or<billed, Equal<True>>>>>, True, False>))]
		public virtual bool? Locked { get; set; }
		#endregion

	}
}
