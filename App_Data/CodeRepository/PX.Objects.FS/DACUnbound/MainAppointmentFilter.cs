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
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PX.Data.PXGenericInqGrph;
using static PX.Objects.FS.ListField;
using static PX.Objects.FS.MainAppointmentFilter;

namespace PX.Objects.FS
{
	using SOFilter =
		Where2<Where<FSServiceOrder.sOID, Equal<Current<MainAppointmentFilter.sOID>>,
			And<MainAppointmentFilter.sORefNbr, IsNull>>,
		Or<Where<FSServiceOrder.srvOrdType, Equal<Current<MainAppointmentFilter.srvOrdType>>,
			And<FSServiceOrder.refNbr, Equal<Current<MainAppointmentFilter.sORefNbr>>>>>>;
	/// <summary>
	/// Main filter for Scheduler Board
	/// </summary>
	[System.SerializableAttribute]
	[PXCacheName(TX.TableName.MainAppointmentFilter)]
	public class MainAppointmentFilter : PXBqlTable, IBqlTable
	{
		#region SrvOrdType
		public abstract class srvOrdType : PX.Data.BQL.BqlString.Field<srvOrdType> { }

		/// <summary>
		/// Service Order type
		/// </summary>
		[PXString(4, IsFixed = true)]
		[PXUIField(DisplayName = "Order Type", Visibility = PXUIVisibility.SelectorVisible, Required = true)]
		[FSSelectorSrvOrdTypeNOTQuote]
		[PXRestrictor(typeof(Where<FSSrvOrdType.active, Equal<True>>), null)]
		[PXUIVerify(typeof(Where<Current<FSSrvOrdType.active>, Equal<True>>),
					PXErrorLevel.Warning, TX.Error.SRVORDTYPE_INACTIVE, CheckOnRowSelected = true)]
		[PXUnboundDefault(typeof(Coalesce<
			Search<FSxUserPreferences.dfltSrvOrdType,
				Where<PX.SM.UserPreferences.userID.IsEqual<AccessInfo.userID.FromCurrent>>>,
			Search<FSSetup.dfltSrvOrdType>>), PersistingCheck = PXPersistingCheck.Null)]
		[PX.Data.EP.PXFieldDescription]
		public virtual string SrvOrdType { get; set; }
		#endregion

		#region SORefNbr
		public abstract class sORefNbr : PX.Data.BQL.BqlString.Field<sORefNbr> { }

		/// <summary>
		/// Service Order reference number
		/// </summary>
		[PXString]
		[PXUIField(DisplayName = "Order Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		[PXFormula(typeof(Default<MainAppointmentFilter.srvOrdType>))]
		[PXSelector(typeof(
			Search<FSServiceOrder.refNbr,
				Where2<Where<FSServiceOrder.srvOrdType, Equal<Current<MainAppointmentFilter.srvOrdType>>,
					Or<Current<MainAppointmentFilter.srvOrdType>, IsNull>>,
				And<FSServiceOrder.status, Equal<ServiceOrderStatus.open>>>,
			OrderBy<Desc<FSServiceOrder.refNbr>>>),
			DescriptionField = typeof(FSServiceOrder.docDesc))]
		public virtual string SORefNbr { get; set; }
		#endregion

		#region RefNbr
		public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }

		/// <summary>
		/// Appointment reference number
		/// </summary>
		[PXString]
		[PXUIField(DisplayName = "Ref. Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
		[PXFormula(typeof(Default<MainAppointmentFilter.srvOrdType>))]
		[PX.Data.EP.PXFieldDescription]
		public virtual string RefNbr { get; set; }
		#endregion

		#region SOID
		public abstract class sOID : PX.Data.BQL.BqlInt.Field<sOID> { }

		/// <summary>
		/// Service Order ID
		/// </summary>
		[PXInt]
		public virtual int? SOID { get; set; }
		#endregion

		#region Status
		public abstract class status : PX.Data.BQL.BqlString.Field<status>
		{
			public abstract class Values : ListField.ServiceOrderStatus { }
		}

		/// <summary>
		/// Service Order Status
		/// </summary>

		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[status.Values.List]
		[PXUnboundDefault(typeof(Search<FSServiceOrder.status, SOFilter>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual string Status { get; set; }
		#endregion

		#region Description
		public abstract class description : PX.Data.BQL.BqlString.Field<description> { }

		/// <summary>
		/// Service Order Description
		/// </summary>
		[PXString]
		[PXUIField(DisplayName = "Description", Required = true)]
		[PXUnboundDefault("", PersistingCheck = PXPersistingCheck.NullOrBlank)]
		public virtual string Description { get; set; }
		#endregion

		#region CustomerID
		public abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }

		/// <summary>
		/// Customer ID
		/// </summary>
		[PXInt]
		[PXUIField(DisplayName = "Customer", Visibility = PXUIVisibility.SelectorVisible, Required = true)]
		[PXUnboundDefault((typeof(Coalesce<
			Search<FSServiceOrder.customerID, SOFilter>,
			Search<Customer.bAccountID, Where<Customer.bAccountID, Equal<CurrentValue<MainAppointmentFilter.customerID>>>>
		>)), PersistingCheck = PXPersistingCheck.Null)]
		[FSSelectorBusinessAccount_CU_PR_VC]
		public virtual int? CustomerID { get; set; }

		#endregion

		#region LocationID
		public abstract class locationID : PX.Data.BQL.BqlInt.Field<locationID> { }

		/// <summary>
		/// Customer Location ID
		/// </summary>
		[PXUnboundDefault(typeof(Coalesce<
			Search<FSServiceOrder.locationID, SOFilter>,
			Search<Customer.defLocationID, Where<Customer.bAccountID, Equal<CurrentValue<MainAppointmentFilter.customerID>>>>
		>))]
		[PXUIField(DisplayName = "Customer Location")]
		[LocationActive(typeof(Where<Location.bAccountID, Equal<Current<MainAppointmentFilter.customerID>>>),
					DescriptionField = typeof(Location.descr), DirtyRead = true)]
		public virtual int? LocationID { get; set; }
		#endregion

		#region BranchLocationID
		public abstract class branchLocationID : PX.Data.BQL.BqlInt.Field<branchLocationID> { }

		/// <summary>
		/// Branch Location ID
		/// </summary>
		[PXUnboundDefault(typeof(Coalesce<
			Search<FSServiceOrder.branchLocationID, SOFilter>,
			Search<FSxUserPreferences.dfltBranchLocationID,
				Where<PX.SM.UserPreferences.userID, Equal<CurrentValue<AccessInfo.userID>>>>
		>))]
		[PXSelector(typeof(FSBranchLocation.branchLocationID), SubstituteKey = typeof(FSBranchLocation.branchLocationCD), DescriptionField = typeof(FSBranchLocation.descr))]
		[PXUIField(DisplayName = "Branch Location")]
		public virtual int? BranchLocationID { get; set; }
		#endregion


		#region ContactID
		public abstract class contactID : PX.Data.BQL.BqlInt.Field<contactID> { }

		/// <summary>
		/// Contact ID
		/// </summary>
		[PXUIField(DisplayName = "Contact", Required = false)]
		[FSSelectorContact(typeof(MainAppointmentFilter.customerID))]
		// TODO: make FSSelectorContact work the same way as ContactRaw for FSContact
		// [ContactRaw(typeof(CreateAppointmentFilter.customerID), WithContactDefaultingByBAccount = true)]
		[PXUnboundDefault(typeof(Coalesce<
			Search<FSServiceOrder.contactID, SOFilter>,
			Search<Customer.defContactID, Where<Customer.bAccountID, Equal<CurrentValue<MainAppointmentFilter.customerID>>>>
		>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual int? ContactID { get; set; }
		#endregion

		#region ScheduledDateTimeBegin
		public abstract class scheduledDateTimeBegin : PX.Data.BQL.BqlDateTime.Field<scheduledDateTimeBegin> { }

		/// <summary>
		/// Scheduled Start date and time
		/// </summary>
		[PXDateAndTime(UseTimeZone = true)]
		[PXUIField(DisplayName = "Scheduled Start", Required = true)]
		public virtual DateTime? ScheduledDateTimeBegin { get; set; }
		#endregion

		//#region ScheduledTimeBegin
		//public abstract class scheduledTimeBegin : PX.Data.BQL.BqlDateTime.Field<scheduledTimeBegin> { }

		//[PXDateAndTime(UseTimeZone = true)]
		//[PXUIField(DisplayName = "Scheduled Start Time", Required = true)]
		//public virtual DateTime? ScheduledTimeBegin { get; set; }
		//#endregion

		#region Duration
		public abstract class duration : PX.Data.BQL.BqlInt.Field<duration> { }

		/// <summary>
		/// Scheduled Duration
		/// </summary>
		[PXTimeSpanLong(Format = TimeSpanFormatType.LongHoursMinutes)]
		[PXUIField(DisplayName = "Duration", Required = true)]
		[PXUnboundDefault(typeof(Current<FSAppointment.scheduledDuration>))]
		public virtual int? Duration { get; set; }
		#endregion

		#region Details
		public abstract class details : PX.Data.BQL.BqlString.Field<details> { }

		/// <summary>
		/// Long Description ("Other")
		/// </summary>
		[PXDBText(IsUnicode = true)]
		[PXUIField(DisplayName = "Details")]
		public virtual String LongDescr { get; set; }
		#endregion

		#region Resources
		public abstract class resources : PX.Data.BQL.BqlString.Field<resources> { }

		/// <summary>
		/// Employees for Service Order
		/// </summary>
		[PXString]
		public virtual string Resources { get; set; }
		#endregion

		#region SODetID
		public abstract class sODetID : PX.Data.BQL.BqlInt.Field<sODetID> { }

		/// <summary>
		/// Service Order Detail ID
		/// </summary>
		[PXInt]
		public virtual int? SODetID { get; set; }
		#endregion

		#region OnHold
		public abstract class onHold : PX.Data.BQL.BqlBool.Field<onHold> { }

		/// <summary>
		/// Specifies if the appointment to be creates in On Hold state
		/// </summary>
		[PXBool]
		public virtual bool? OnHold { get; set; }
		#endregion

		#region InitialRefNbr
		public abstract class initialRefNbr : PX.Data.BQL.BqlString.Field<initialRefNbr> { }

		/// <summary>
		/// Reference number of an appointment to schedule
		/// </summary>
		[PXString]
		public virtual string InitialRefNbr { get; set; }
		#endregion

		#region InitialSORefNbr
		public abstract class initialSORefNbr : PX.Data.BQL.BqlString.Field<initialSORefNbr> { }

		/// <summary>
		/// Reference number of a SO to schedule
		/// </summary>
		[PXString]
		public virtual string InitialSORefNbr { get; set; }
		#endregion

		#region InitialCustomerID
		public abstract class initialCustomerID : PX.Data.BQL.BqlString.Field<initialCustomerID> { }

		/// <summary>
		/// Customer ID to filter SOs with
		/// </summary>
		[PXString]
		[PXSelector(typeof(FSServiceOrder.customerID), SubstituteKey = typeof(FSServiceOrder.customerDisplayName))]
		public virtual string InitialCustomerID { get; set; }
		#endregion


		#region OpenEditor
		public abstract class openEditor : PX.Data.BQL.BqlBool.Field<openEditor> { }

		/// <summary>
		/// Specifies if App. Entry is to be opened
		/// </summary>
		[PXBool]
		public virtual bool? OpenEditor { get; set; }
		#endregion

		#region ResetFilters
		public abstract class resetFilters : PX.Data.BQL.BqlBool.Field<resetFilters> { }

		/// <summary>
		/// Set by server to true, if client needs to switch to "All Records" in order to show the search results
		/// </summary>
		[PXBool]
		public virtual bool? ResetFilters { get; set; }
		#endregion
	}

}

