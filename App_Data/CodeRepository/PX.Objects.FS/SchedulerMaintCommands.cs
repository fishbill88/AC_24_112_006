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
using System.Collections;
using Autofac;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Common;
using PX.Objects.CR;
using PX.Objects.AR;
using PX.Objects.EP;
using PX.Objects.FS.Interfaces;
using PX.Objects.FS.Services;
using System.Linq;
using System.Collections.Generic;
using PX.Api;

namespace PX.Objects.FS
{
	using static PX.Objects.FS.ExternalControls;

	using AccessRightsCheckClause = Brackets<Customer.bAccountID.IsNull
		.Or<Match<Customer, Current<AccessInfo.userName>>>>;

	partial class SchedulerMaint
	{
		public SelectFrom<FSAppointment>
			.LeftJoin<Customer>.On<Customer.bAccountID.IsEqual<FSAppointment.customerID>>
			.Where<AccessRightsCheckClause
				.And<FSAppointment.appointmentID.IsEqual<@P.AsInt>>>
			.View AppointmentById;

		[PXHidden]
		public SelectFrom<FSServiceOrder>
			.Where<FSServiceOrder.srvOrdType.IsEqual<MainAppointmentFilter.srvOrdType.FromCurrent>
				.And<FSServiceOrder.refNbr.IsEqual<MainAppointmentFilter.sORefNbr.FromCurrent>>>
			.View ServiceOrderByFilter;

		// TODO: Do we need to check access rights here?
		[PXHidden]
		public SelectFrom<Customer>
			.Where<Customer.bAccountID.IsEqual<MainAppointmentFilter.customerID.FromCurrent>
				.And<AccessRightsCheckClause>>
			.View CustomerByFilter;


		#region EditedAppointment*
		#region EditedAppointment
		public PXFilter<MainAppointmentFilter> MainAppointmentFilter;
		public PXFilter<MainAppointmentFilter> MainAppointmentFilterBase;

		protected virtual IEnumerable mainAppointmentFilter()
		{
			var mainCache = MainAppointmentFilter.Cache;
			var serviceOrder = ServiceOrderByFilter.SelectSingle();
			var customer = CustomerByFilter.SelectSingle();
			var appointment = MainAppointmentFilterBase.SelectSingle();

			if (serviceOrder != null)
			{
				ServiceOrderByFilter.Cache.SetStatus(serviceOrder, PXEntryStatus.Notchanged); // Pushing into Cache
				appointment.CustomerID = serviceOrder.CustomerID;
				customer = CustomerByFilter.SelectSingle();
			}
			ServiceOrderByFilter.Current = serviceOrder;

			if (string.IsNullOrEmpty(appointment.SrvOrdType))
			{
				object srvOrdType;
				mainCache.RaiseFieldDefaulting<MainAppointmentFilter.srvOrdType>(appointment, out srvOrdType);
				appointment.SrvOrdType = (string)srvOrdType;
			}

			if (customer != null)
			{
				CustomerByFilter.Cache.SetStatus(customer, PXEntryStatus.Notchanged); // Pushing into Cache
			}
			CustomerByFilter.Current = customer;

			if (customer != null)
			{
				Location locationRecord = (appointment.LocationID != null) ? AllLocations.Search<Location.locationID>(appointment.LocationID).FirstOrDefault() : null;
				if (locationRecord == null || locationRecord.BAccountID != customer.BAccountID)
				{
					object locationId = appointment.LocationID;
					mainCache.RaiseFieldDefaulting<MainAppointmentFilter.locationID>(appointment, out locationId);
					appointment.LocationID = (int?)locationId;
				}
				Contact contactRecord = (appointment.ContactID != null) ? AllContacts.Search<Contact.contactID>(appointment.ContactID).FirstOrDefault() : null;
				if (contactRecord == null || contactRecord.BAccountID != customer.BAccountID)
				{
					object contactID = appointment.ContactID;
					mainCache.RaiseFieldDefaulting<MainAppointmentFilter.contactID>(appointment, out contactID);
					appointment.ContactID = (int?)contactID;
				}
			}
			else
			{
				appointment.LocationID = null;
				appointment.ContactID = null;
			}

			PXUIFieldAttribute.SetEnabled<MainAppointmentFilter.customerID>(mainCache, appointment, serviceOrder == null);
			PXUIFieldAttribute.SetVisible<MainAppointmentFilter.status>(mainCache, appointment, serviceOrder != null);

			PXUIFieldAttribute.SetEnabled<MainAppointmentFilter.contactID>(mainCache, appointment, customer != null);
			PXUIFieldAttribute.SetEnabled<MainAppointmentFilter.locationID>(mainCache, appointment, customer != null);
			PXUIFieldAttribute.SetEnabled(EditedAppointmentLocation.Cache, null, customer != null);
			PXUIFieldAttribute.SetEnabled(EditedAppointmentContact.Cache, null, customer != null);

			appointment.SOID = serviceOrder?.SOID;
			appointment.Status = serviceOrder?.Status;

			mainCache.Update(appointment);

			return new MainAppointmentFilter[] { appointment };
		}

		protected virtual void _(Events.RowUpdated<MainAppointmentFilter> e)
		{
			// TODO: Workaround for the time-zone issue introduced in the fix of AC-295418,AC-295424 (PR#66542)
			MainAppointmentFilter.Current.ScheduledDateTimeBegin = e.Row.ScheduledDateTimeBegin?.ToUniversalTime();
		}
		#endregion

		#region EditedAppointmentLocation
		public SelectFrom<Location>
			.Where<Location.locationID.IsEqual<MainAppointmentFilter.locationID.FromCurrent>>
			.View EditedAppointmentLocation;
		#endregion

		#region EditedAppointmentContact
		public SelectFrom<FSContact>
			.Where<FSContact.contactID.IsEqual<MainAppointmentFilter.contactID.FromCurrent>>
			.View EditedAppointmentContact;

		protected virtual IEnumerable editedAppointmentContact()
		{
			var view = new PXView(this, false, EditedAppointmentContact.View.BqlSelect);
			var res = view.SelectSingle() as FSContact;
			return new FSContact[] { res };
		}
		#endregion

		#region EditedAppointmentEmployees
		public SelectFrom<EPEmployee>
			.InnerJoin<Contact>.On<Contact.contactID.IsEqual<EPEmployee.defContactID>>
			.Where<EPEmployee.bAccountID.IsIn<@P.AsInt>>
			.View.ReadOnly EditedAppointmentEmployees;

		protected virtual IEnumerable editedAppointmentEmployees()
		{
			var delegateResult = new PXDelegateResult() { IsResultFiltered = true, IsResultTruncated = true, IsResultSorted = true };
			var appointment = MainAppointmentFilterBase.SelectSingle();
			var employeeIDs = !(appointment?.Resources?.IsNullOrEmpty() ?? true) ? appointment?.Resources?.Split(',').Select(x => int.Parse(x)).ToArray() : new int[] { } ;
			var employeeParams = new[] { employeeIDs };

			PXView select = new PXView(this, true, EditedAppointmentEmployees.View.BqlSelect);
			var resources = select.SelectMulti(employeeParams);
			delegateResult.AddRange(resources);

			return delegateResult;
		}
		#endregion
		#endregion

		#region ScheduleAppointment
		public PXAction<MainAppointmentFilter> ScheduleAppointment;
		[PXUIField(DisplayName = "Schedule", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable scheduleAppointment(PXAdapter adapter)
		{
			LastUpdatedAppointmentFilter.Current = null;
			WrkProcess graphWrkProcess = PXGraph.CreateInstance<WrkProcess>();
			var appointment = MainAppointmentFilterBase.SelectSingle();
			var mainCache = MainAppointmentFilter.Cache;

			if (appointment.CustomerID == null)
			{
				object customerID;
				mainCache.RaiseFieldDefaulting<MainAppointmentFilter.customerID>(appointment, out customerID);
				if (customerID != null)
				{
					appointment.CustomerID = (int)customerID;
				}
			}

			if (appointment.OpenEditor != true && !MainAppointmentFilter.VerifyRequired())
			{
				return adapter.Get();
			}

			FSWrkProcess fsWrkProcessRow = new FSWrkProcess()
			{
				RoomID = string.Empty,
				SOID = appointment.SOID,
				SrvOrdType = appointment.SrvOrdType,
				BranchID = null,
				BranchLocationID = appointment.BranchLocationID,
				CustomerID = appointment.CustomerID,
				SMEquipmentID = null,
				ScheduledDateTimeBegin = appointment.ScheduledDateTimeBegin,
				ScheduledDateTimeEnd = appointment.ScheduledDateTimeBegin + new TimeSpan((appointment.Duration ?? 60) * 60 * 1000 * 10000L),
				TargetScreenID = ID.ScreenID.APPOINTMENT,
				EmployeeIDList = appointment.Resources,
				LineRefList = (appointment.SODetID == 0) ? string.Empty : appointment.SODetID.ToString(),
				EquipmentIDList = string.Empty
			};

			if (appointment.OpenEditor == true)
			{
				// It's going to throw a PXRedirectRequiredException 
				graphWrkProcess.LaunchAppointmentEntryScreen(fsWrkProcessRow, true, true, appointment, PXBaseRedirectException.WindowMode.NewWindow);
				return adapter.Get();
			}

			int? appointmentID = graphWrkProcess.LaunchAppointmentEntryScreen(fsWrkProcessRow, false, true, appointment);
			SetLastUpdatedFilter(appointmentID);

			return adapter.Get();
		}
		#endregion

		#region UpdatedAppointment
		[PXHidden]
		public PXFilter<UpdateAppointmentFilter> UpdatedAppointment;

		public PXAction<MainAppointmentFilter> UpdateAppointment;
		[PXUIField(DisplayName = "Update", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable updateAppointment(PXAdapter adapter)
		{
			var request = UpdatedAppointment.SelectSingle();

			// TODO: Workaround for the time-zone issue introduced in the fix of AC-295418,AC-295424 (PR#66542)
			request.NewBegin = request.NewBegin?.ToUniversalTime();
			request.NewEnd = request.NewEnd?.ToUniversalTime();

			LastUpdatedAppointmentFilter.Current = null;

			// TODO: check if it's needed
			//using (new PXScreenIDScope(ID.ScreenID.WEB_METHOD))
			//{
			var appointment = AppointmentById.SelectSingle(request.AppointmentID);
			if (appointment == null)
			{
				throw new PXException(TX.Error.APPOINTMENT_NOT_FOUND);
			}

			var entryGraph = PXGraph.CreateInstance<AppointmentEntry>();
			entryGraph.AppointmentRecords.Cache.SetStatus(appointment, PXEntryStatus.Notchanged); // Pushing the appointment into Cache
			entryGraph.AppointmentRecords.Current = appointment;

			if (request.NewBegin != null && request.NewEnd != null
				&& (appointment.ScheduledDateTimeBegin != request.NewBegin || appointment.ScheduledDateTimeEnd != request.NewEnd))
			{
				appointment.HandleManuallyScheduleTime = true;
				appointment.ScheduledDateTimeBegin = request.NewBegin;
				appointment.ScheduledDateTimeEnd = request.NewEnd;
			}

			appointment.Confirmed = request.Confirmed ?? appointment.Confirmed;
			appointment.ValidatedByDispatcher = request.ValidatedByDispatcher ?? appointment.ValidatedByDispatcher;

			UpdateAppointmentEmployee(entryGraph, appointment, request.NewResourceID, request.OldResourceID);
			entryGraph.AppointmentRecords.Update(appointment);

			entryGraph.DisableServiceOrderUnboundFieldCalc = true;
			entryGraph.SkipEarningTypeCheck = true;
			entryGraph.SelectTimeStamp();

			DispatchBoardAppointmentMessages messages = new DispatchBoardAppointmentMessages();
			entryGraph.PressSave(messages);
			if (messages.ErrorMessages.Count > 0)
			{
				// Acuminator disable once PX1050 HardcodedStringInLocalizationMethod [ErrorMessages is localized within PressDelete]
				// Acuminator disable once PX1051 NonLocalizableString [ErrorMessages is localized within PressDelete]
				throw new PXException(messages.ErrorMessages[0]); // TODO: report other errors as well; report warnings
			}

			SetLastUpdatedFilter(appointment.AppointmentID);

			return adapter.Get();
		}
		#endregion

		#region CloneAppointment
		public PXAction<MainAppointmentFilter> cloneAppointment;
		[PXUIField(DisplayName = "Delete", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		// TODO: Refresh board upon closing of the clone window
		// [PXButton(OnClosingPopup = PXSpecialButtonType.Cancel)]

		public virtual IEnumerable CloneAppointment(PXAdapter adapter)
		{
			var request = MainAppointmentFilter.SelectSingle();
			CloneAppointmentProcess cloneGraph = PXGraph.CreateInstance<CloneAppointmentProcess>();

			cloneGraph.Filter.Current.SrvOrdType = request.SrvOrdType;
			cloneGraph.Filter.Current.RefNbr = request.RefNbr;
			cloneGraph.AppointmentSelected.Current = cloneGraph.AppointmentSelected.Select();
			cloneGraph.cancel.Press();

			throw new PXRedirectRequiredException(cloneGraph, null) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
		}
		#endregion

		#region DeleteAppointment
		public PXAction<MainAppointmentFilter> DeleteAppointment;
		[PXUIField(DisplayName = "Delete", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable deleteAppointment(PXAdapter adapter)
		{
			var request = UpdatedAppointment.SelectSingle();
			var appointmentFound = AppointmentById.SelectSingle(request.AppointmentID);

			var entryGraph = PXGraph.CreateInstance<AppointmentEntry>();
			entryGraph.AppointmentRecords.Cache.SetStatus(appointmentFound, PXEntryStatus.Notchanged); // Pushing the appointment into Cache
			entryGraph.AppointmentRecords.Current = appointmentFound;

			DispatchBoardAppointmentMessages messages = new DispatchBoardAppointmentMessages();
			entryGraph.PressDelete(messages);
			if (messages.ErrorMessages.Count > 0)
			{
				// Acuminator disable once PX1050 HardcodedStringInLocalizationMethod [ErrorMessages is localized within PressDelete]
				// Acuminator disable once PX1051 NonLocalizableString [ErrorMessages is localized within PressDelete]
				throw new PXException(messages.ErrorMessages[0]); // TODO: report other errors as well; report warnings
			}

			return adapter.Get();
		}
		#endregion

		// TODO: Verify if we don't need to use a special access right check procedure
		//public virtual bool CheckAccessRights(string screenName, Type graphType, Type cacheType, PXCacheRights accessRight)
		//{
		//	if (!PXAccess.VerifyRights(graphType)) return false;

		//	PXCacheRights rights;
		//	List<string> invisible;
		//	List<string> disabled;
		//	PXAccess.GetRights(screenName, graphType.Name, cacheType, out rights, out invisible, out disabled);
		//	return rights >= accessRight;
		//}

		protected void SetLastUpdatedFilter(int? appointmentID)
		{
			var lastUpdatedFilter = LastUpdatedAppointmentFilter.SelectSingle();
			lastUpdatedFilter.AppointmentID = appointmentID;
			if (lastUpdatedFilter != null)
			{
				LastUpdatedAppointmentFilter.Cache.SetStatus(lastUpdatedFilter, PXEntryStatus.Notchanged); // Pushing into Cache
			}
			LastUpdatedAppointmentFilter.Current = lastUpdatedFilter;
		}

		protected void UpdateAppointmentEmployee(AppointmentEntry entryGraph, FSAppointment appointment, int? newEmployeeId, int? oldEmployeeId)
		{
			if (newEmployeeId == oldEmployeeId) return; // nothing to change

			// TODO: process error reporting
			//FSServiceOrder fsServiceOrderRow = appointmentEntry.ServiceOrderRelated.Current;
			//if (string.IsNullOrEmpty(fsAppointmentRow.RoomID) == false
			//		&& string.IsNullOrWhiteSpace(fsAppointmentRow.RoomID) == false)
			//{
			//	fsServiceOrderRow.RoomID = fsAppointmentRow.RoomID;
			//}
			//appointmentEntry.ServiceOrderRelated.Update(fsServiceOrderRow);

			if (newEmployeeId > 0)
			{
				var appointmentEmployeeTestShared = entryGraph.AppointmentServiceEmployees.Search<FSAppointmentEmployee.appointmentID,
					FSAppointmentEmployee.employeeID>(appointment.AppointmentID, newEmployeeId)?.FirstOrDefault_();
				if (appointmentEmployeeTestShared != null)
				{
					throw new PXException(TX.Error.APPOINTMENT_SHARED);
				}
			}

			if (oldEmployeeId > 0)
			{
				var oldRecords = entryGraph.AppointmentServiceEmployees.Select()
					.Select(x => (FSAppointmentEmployee)x)
					.Where(x => x.EmployeeID == oldEmployeeId)
					.ToArray();

				var wasPrimaryDriver = oldRecords.ElementAtOrDefault(0)?.PrimaryDriver;
				foreach (FSAppointmentEmployee appointmentEmployeeOld in oldRecords)
				{
					if (newEmployeeId > 0)
					{
						FSAppointmentEmployee appointmentEmployeeNew = new FSAppointmentEmployee()
						{
							AppointmentID = appointment.AppointmentID,
							EmployeeID = newEmployeeId,
							ServiceLineRef = appointmentEmployeeOld.ServiceLineRef,
							PrimaryDriver = wasPrimaryDriver,
						};
						entryGraph.AppointmentServiceEmployees.Insert(appointmentEmployeeNew);
					}
					entryGraph.AppointmentServiceEmployees.Delete(appointmentEmployeeOld);
				}
			}
			else if (newEmployeeId > 0)
			{
				FSAppointmentEmployee appointmentEmployeeNew = new FSAppointmentEmployee()
				{
					AppointmentID = appointment.AppointmentID,
					EmployeeID = newEmployeeId,
				};
				entryGraph.AppointmentServiceEmployees.Insert(appointmentEmployeeNew);
			}
		}

		#region Registration of Data Handler service for Autofac
		public class ServiceRegistration : Module
		{
			protected override void Load(ContainerBuilder builder)
			{
				builder.RegisterType<SchedulerDataHandler>()
					.As<ISchedulerDataHandler>()
					.InstancePerLifetimeScope();
			}
		}
		#endregion
	}

}
