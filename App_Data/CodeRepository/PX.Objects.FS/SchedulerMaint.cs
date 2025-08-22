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
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Common;
using PX.Objects.CR;
using PX.Objects.IN;
using PX.Objects.GL;
using PX.Objects.AR;
using PX.Objects.EP;
using PX.Objects.FS.Interfaces;
using System.Linq;
using System.Collections.Generic;
using PX.Objects.AP;
using PX.Objects.Common;
using PX.Objects.PM;
using JetBrains.Annotations;
// ReSharper disable InconsistentNaming

namespace PX.Objects.FS
{
	using AccessRightsCheckClause = Match</*Customer, */Current<AccessInfo.userName>>;

	using EmployeeSelectBase = SelectFrom<EPEmployee>
		.LeftJoin<FSEmployeeSkill>.On<FSEmployeeSkill.employeeID.IsEqual<EPEmployee.bAccountID>>
		.LeftJoin<FSSkill>.On<FSSkill.skillID.IsEqual<FSEmployeeSkill.skillID>>
		.LeftJoin<FSLicense>.On<FSLicense.employeeID.IsEqual<EPEmployee.bAccountID>>
		.LeftJoin<FSLicenseType>.On<FSLicense.licenseID.IsEqual<FSLicense.licenseID>>
		.LeftJoin<FSGeoZoneEmp>.On<FSGeoZoneEmp.employeeID.IsEqual<EPEmployee.bAccountID>>
		.LeftJoin<FSGeoZone>.On<FSGeoZone.geoZoneID.IsEqual<FSGeoZoneEmp.geoZoneID>>
		.LeftJoin<BranchAlias>.On<BranchAlias.bAccountID.IsEqual<EPEmployee.parentBAccountID>>;

	using AppointmentEmployeeSelectBase = SelectFrom<EPEmployee>
		.InnerJoin<SchedulerAppointmentEmployee>.On<SchedulerAppointmentEmployee.employeeID.IsEqual<EPEmployee.bAccountID>>
		.InnerJoin<SchedulerAppointment>.On<SchedulerAppointment.appointmentID.IsEqual<SchedulerAppointmentEmployee.appointmentID>>
		.InnerJoin<SchedulerServiceOrder>.On<SchedulerServiceOrder.sOID.IsEqual<SchedulerAppointment.sOID>>;

	using AppointmentsNoEmployeesSelectBase = SelectFrom<SchedulerAppointment>
		.LeftJoin<SchedulerAppointmentEmployee>.On<SchedulerAppointmentEmployee.appointmentID.IsEqual<SchedulerAppointment.appointmentID>>
		.InnerJoin<SchedulerServiceOrder>.On<SchedulerServiceOrder.sOID.IsEqual<SchedulerAppointment.sOID>>;

	using ServiceOrdersBase = SelectFrom<SchedulerServiceOrder>
		.InnerJoin<FSSODet>.On<FSSODet.sOID.IsEqual<SchedulerServiceOrder.sOID>>
		.LeftJoin<InventoryItem>.On<InventoryItem.inventoryID.IsEqual<FSSODet.inventoryID>>
		.LeftJoin<INItemClass>.On<INItemClass.itemClassID.IsEqual<InventoryItem.itemClassID>>;

	using SearchAppointmentsBase = SelectFrom<SchedulerAppointment>
		.InnerJoin<SchedulerServiceOrder>.On<SchedulerServiceOrder.sOID.IsEqual<SchedulerAppointment.sOID>>;

	internal static class SchedulerFilterSelector
	{
		public static PXFilterRow[] ForDACs(this PXView.PXFilterRowCollection filters, string[] dacNames, bool includeMainDAC = false)
		{
			return AccountForDACs(filters, dacNames, includeMainDAC);
		}
		public static PXFilterRow[] ForDACs(this PXFilterRow[] filters, string[] dacNames, bool includeMainDAC = false)
		{
			return AccountForDACs(filters, dacNames, includeMainDAC);
		}

		public static PXFilterRow[] ExceptForDACs(this PXFilterRow[] filters, string[] dacNames, bool exceptMainDAC = false)
		{
			return AccountForDACs(filters, dacNames, exceptMainDAC, false);
		}

		private static PXFilterRow[] AccountForDACs(PXFilterRow[] filters, string[] dacNames, bool accountForMainDAC = false, bool shouldInclude = true)
		{
			var result = filters.Where(x =>
			{
				foreach (var dac in dacNames)
				{
					if (x.DataField.StartsWith($"{dac}__")) return shouldInclude;
				}
				if (!x.DataField.Contains("__") && accountForMainDAC) return shouldInclude;
				return !shouldInclude;
			}).ToArray();
			return result;
		}
	}

	public partial class SchedulerMaint : PXGraph<SchedulerMaint>
	{
		[InjectDependency]
		private ISchedulerDataHandler _dataHandler { get; set; }

		[PXHidden]
		public SelectFrom<BAccount>.View BAccounts; // Not used directly -- needed here to load the descendants (employees, customers etc)
		[PXHidden]
		public SelectFrom<BAccountR>.View BAccountsR;

		[PXHidden]
		public SelectFrom<Branch>.View Branches; // Not used directly -- needed for loading priority

		[PXHidden]
		public SelectFrom<BranchAlias>.View BranchAliases; // Not used directly -- needed for loading priority

		public SelectFrom<FSServiceOrder>.View AllServiceOrders;
		public SelectFrom<FSAppointment>.View AllAppointments;
		public SelectFrom<Location>.View AllLocations;
		public SelectFrom<Contact>.View AllContacts;
		public SelectFrom<FSRoom>.View AllRooms;
		public SelectFrom<FSServiceContract>.View AllContracts;
		public SelectFrom<InventoryItem>.View InventoryItems;
		public SelectFrom<FSSetup>.View Setup;

		// TODO: Add check where needed: (see ExternalControls)
		//BAccount.type, NotEqual<BAccountType.prospectType>,

		#region Filters
		[PXHidden]
		public PXFilter<SchedulerDatesFilter> DatesFilter;
		protected virtual IEnumerable datesFilter()
		{
			if (DatesFilter.Current?.PeriodKind == null)
			{
				DatesFilter.Current = _dataHandler.LoadDatesFilter(PXContext.GetScreenID());
				DatesFilter.Cache.SetStatus(DatesFilter.Current, PXEntryStatus.Notchanged); // Pushing into Cache
			}
			else
			{
				PatchDatesFilter();
			}
			return new SchedulerDatesFilter[] { DatesFilter.Current };
		}

		protected void PatchDatesFilter()
		{
			// TODO: Workaround for the time-zone issue introduced in the fix of AC-295418,AC-295424 (PR#66542)
			DatesFilter.Current.DateBegin = DatesFilter.Current.DateBegin?.ToUniversalTime();
			DatesFilter.Current.DateEnd = DatesFilter.Current.DateEnd?.ToUniversalTime();
			DatesFilter.Cache.SetStatus(DatesFilter.Current, PXEntryStatus.Notchanged); // Pushing into Cache
		}

		[PXHidden]
		public PXFilter<SchedulerInitData> InitData;
		protected virtual IEnumerable initData()
		{
			if (InitData.Current?.MapAPIKey == null)
			{
				InitData.Current.MapAPIKey = SharedFunctions.GetMapApiKey(this);
				InitData.Cache.SetStatus(InitData.Current, PXEntryStatus.Notchanged); // Pushing into Cache
			}
			return new SchedulerInitData[] { InitData.Current };
		}

		[PXHidden]
		public PXFilter<SchedulerSOFilter> SOFilter;

		[PXHidden]
		public PXFilter<LastUpdatedAppointmentFilter> LastUpdatedAppointmentFilter;

		public PXFilter<SchedulerAppointmentFilter> AppointmentFilter;

		#endregion

		#region Service Order
		// TODO: Some details may not fit in the pageSize window, that could result in the last(?) SO not displayed correctly
		// => make a delegate that would strip the last SO if it's not a full read
		// Also make sure that the rest(preceding) of the SOs are fetched correctly
		[PXFilterable]
		public ServiceOrdersBase
			.LeftJoin<FSServiceLicenseType>.On<FSServiceLicenseType.serviceID.IsEqual<FSSODet.inventoryID>>
			.LeftJoin<FSServiceSkill>.On<FSServiceSkill.serviceID.IsEqual<FSSODet.inventoryID>>
			.View.ReadOnly ServiceOrders;

		[PXHidden]
		public ServiceOrdersBase
			.Where<FSSODet.lineType.IsEqual<ListField_LineType_UnifyTabs.Service>
				.And<FSSODet.status.IsNotEqual<FSSODet.status.Completed>>
				.And<FSSODet.status.IsNotEqual<FSSODet.status.Canceled>>
				.And<SchedulerServiceOrder.quote.IsEqual<False>>
				.And<SchedulerServiceOrder.hold.IsEqual<False>>
				.And<SchedulerServiceOrder.closed.IsEqual<False>>
				.And<SchedulerServiceOrder.canceled.IsEqual<False>>
				.And<SchedulerServiceOrder.completed.IsEqual<False>>
				.And<SchedulerServiceOrder.appointmentsNeeded.IsEqual<True>>
				.And<Where<Exists<SelectFrom<FSSODetAlias>
						.Where<FSSODetAlias.sOID.IsEqual<SchedulerServiceOrder.sOID>
						.And<FSSODetAlias.status.IsEqual<FSSODet.status.ScheduleNeeded>>
						.And<FSSODetAlias.lineType.IsEqual<ListField_LineType_UnifyTabs.Service>>>>>>>
			.View.ReadOnly ServiceOrdersDelegateQuery;

		protected virtual IEnumerable serviceOrders()
		{
			var delegateResult = new PXDelegateResult() { IsResultFiltered = true, IsResultTruncated = true, IsResultSorted = true };

			// Select InventoryItem if either FSLicenseType or FSSkill is set
			(int startRow, int totalRows) = (0, 0);
			var command = ServiceOrdersDelegateQuery.View.BqlSelect;
			var parameters = new object[] { };
			PXFilterRow[] filters = PXView.Filters;

			var serviceFilters = filters.ForDACs(new string[] { nameof(FSServiceLicenseType), nameof(FSServiceSkill) });
			if (serviceFilters.Length > 0)
			{
				var servicesSelect = new SelectFrom<InventoryItem>
					.LeftJoin<FSServiceLicenseType>.On<FSServiceLicenseType.serviceID.IsEqual<InventoryItem.inventoryID>>
					.LeftJoin<FSServiceSkill>.On<FSServiceSkill.serviceID.IsEqual<InventoryItem.inventoryID>>
					.View.ReadOnly(this);

				var services = servicesSelect.View.Select(PXView.Currents, null, null,
					null, null, serviceFilters, ref startRow, PXView.MaximumRows, ref totalRows).RowCast<InventoryItem>();
				parameters = parameters.Append(new[] { services.Select(obj => (object)obj.InventoryID).ToArray() });
				command = command.WhereAnd<Where<FSSODet.inventoryID.IsIn<@P.AsInt>>>();
			}

			var employeeFilters = filters.ForDACs(new string[] { nameof(EPEmployee), nameof(FSGeoZone), nameof(BranchAlias) });
			if (employeeFilters.Length > 0)
			{
				var employeeSelect = new SelectFrom<EPEmployee>
					.LeftJoin<FSGeoZoneEmp>.On<FSGeoZoneEmp.employeeID.IsEqual<EPEmployee.bAccountID>>
					.LeftJoin<FSGeoZone>.On<FSGeoZone.geoZoneID.IsEqual<FSGeoZoneEmp.geoZoneID>>
					.LeftJoin<BranchAlias>.On<BranchAlias.bAccountID.IsEqual<EPEmployee.parentBAccountID>>
					.View.ReadOnly(this);

				var employees = employeeSelect.View.Select(PXView.Currents, null, null,
					null, null, employeeFilters, ref startRow, PXView.MaximumRows, ref totalRows).RowCast<EPEmployee>();
				parameters = parameters.Append(new[] { employees.Select(obj => (object)obj.BAccountID).ToArray() });
				command = command.WhereAnd<Where<
					Exists<SelectFrom<FSSOEmployee>
						.Where<FSSOEmployee.employeeID.IsIn<@P.AsInt>
						.And<FSSOEmployee.sOID.IsEqual<SchedulerServiceOrder.sOID>>>>>>();
			}

			var view = new PXView(this, true, command);
			(startRow, totalRows) = (PXView.StartRow, 0);
			var mainFilters = filters.ExceptForDACs(new string[] {
				nameof(FSServiceLicenseType), nameof(FSServiceSkill), nameof(EPEmployee), nameof(FSGeoZone), nameof(BranchAlias) });
			using (new PXFieldScope(view, GetSOFieldScopeFields()))
			{
				var serviceOrderEntries = view.Select(PXView.Currents, parameters, PXView.Searches, PXView.SortColumns, PXView.Descendings,
					mainFilters, ref startRow, PXView.MaximumRows, ref totalRows);
				delegateResult.AddRange(serviceOrderEntries);
			}

			return delegateResult;
		}

		public SelectFrom<EPEmployee>
			.InnerJoin<Contact>.On<Contact.contactID.IsEqual<EPEmployee.defContactID>>
			.InnerJoin<FSSOEmployee>.On<FSSOEmployee.employeeID.IsEqual<EPEmployee.bAccountID>>
			.Where<FSSOEmployee.sOID.IsEqual<SchedulerSOFilter.sOID.FromCurrent>>
			.View.ReadOnly SelectedSOEmployees;
		#endregion

		#region SearchAppointments
		[PXFilterable]
		public SearchAppointmentsBase
			.LeftJoin<SchedulerAppointmentEmployee>.On<SchedulerAppointmentEmployee.appointmentID.IsEqual<SchedulerAppointment.appointmentID>>
			.LeftJoin<EPEmployee>.On<EPEmployee.bAccountID.IsEqual<SchedulerAppointmentEmployee.employeeID>>
			.LeftJoin<BranchAlias>.On<BranchAlias.bAccountID.IsEqual<EPEmployee.parentBAccountID>>
			.View.ReadOnly SearchAppointments;

		[PXHidden]
		public SearchAppointmentsBase.View.ReadOnly SearchAppointmentsBaseDelegate;

		protected virtual IEnumerable searchAppointments()
		{
			var delegateResult = new PXDelegateResult() { IsResultFiltered = true, IsResultTruncated = true, IsResultSorted = true };

			(int startRow, int totalRows) = (0, 0);
			var command = SearchAppointmentsBaseDelegate.View.BqlSelect;
			var parameters = new object[] { };
			PXFilterRow[] filters = PXView.Filters;

			var employeeFilters = filters.ForDACs(new string[] { nameof(EPEmployee), nameof(BranchAlias) });
			if (employeeFilters.Length > 0)
			{
				var employeeSelect = new SelectFrom<EPEmployee>
					.LeftJoin<BranchAlias>.On<BranchAlias.bAccountID.IsEqual<EPEmployee.parentBAccountID>>
					.View.ReadOnly(this);
				var employees = employeeSelect.View.Select(PXView.Currents, null, null,
					null, null, employeeFilters, ref startRow, PXView.MaximumRows, ref totalRows).RowCast<EPEmployee>();
				parameters = parameters.Append(new[] { employees.Select(obj => (object)obj.BAccountID).ToArray() });
				command = command.WhereAnd<Where<
					Exists<SelectFrom<SchedulerAppointmentEmployee>
						.Where<SchedulerAppointmentEmployee.employeeID.IsIn<@P.AsInt>
						.And<SchedulerAppointmentEmployee.appointmentID.IsEqual<SchedulerAppointment.appointmentID>>>>>>();
			}

			int keysNum = SearchAppointmentsBaseDelegate.Cache.Keys.Count;

			// Sort by <recent first> by default
			var sorts = PXView.SortColumns;
			var descs = PXView.Descendings;
			var count = sorts.Length - 2;
			sorts = sorts.Take(count).ToArray().Append("CreatedDateTime");
			descs = descs.Take(count).ToArray().Append(true);

			var view = new PXView(this, true, command);
			(startRow, totalRows) = (PXView.StartRow, 0);
			var mainFilters = filters.ExceptForDACs(new string[] { nameof(EPEmployee), nameof(BranchAlias) });
			using (new PXFieldScope(view, GetAppointmentFieldScopeFields()))
			{
				var searchResults = view.Select(PXView.Currents, parameters, PXView.Searches, sorts, descs,
					mainFilters, ref startRow, PXView.MaximumRows, ref totalRows);
				delegateResult.AddRange(searchResults);
			}

			return delegateResult;
		}
		#endregion

		#region HighlightedSearchAppointment
		[PXHidden]
		public SelectFrom<SchedulerAppointment>
			.Where<SchedulerAppointment.appointmentID.IsEqual<SchedulerAppointmentFilter.searchAppointmentID.FromCurrent>>
			.View.ReadOnly HighlightedSearchAppointment;
		#endregion

		#region Selected Appointment and related Views
		public SelectFrom<SchedulerAppointment>
			.Where<SchedulerAppointment.appointmentID.IsEqual<SchedulerAppointmentFilter.appointmentID.FromCurrent>>
			.View.ReadOnly SelectedAppointment;

		protected virtual IEnumerable selectedAppointment()
		{
			var view = new PXView(this, true, SelectedAppointment.View.BqlSelect);
			var appointment = view.SelectSingle() as SchedulerAppointment;
			if (appointment == null) return new SchedulerAppointment[] { null };

			// Store the last used service order type in a FSAppointment object for it to be accessible by EditScreen
			var storedAppointment = new FSAppointment()
			{
				SrvOrdType = appointment.SrvOrdType,
				RefNbr = appointment.RefNbr,
			};
			AllAppointments.Current = storedAppointment;

			return new SchedulerAppointment[] { appointment };
		}

		public SelectFrom<SchedulerServiceOrder>
			.Where<SchedulerServiceOrder.sOID.IsEqual<SchedulerSOFilter.sOID.FromCurrent>
				.And<AccessRightsCheckClause>>
			.View.ReadOnly SelectedSO;

		protected virtual IEnumerable selectedSO()
		{
			var view = new PXView(this, true, SelectedSO.View.BqlSelect);
			var serviceOrder = view.SelectSingle() as SchedulerServiceOrder;
			if (serviceOrder == null) return new SchedulerServiceOrder[] { null };

			// Store the last used service order type in a FSServiceOrder object for it to be accessible by EditScreen
			var storedServiceOrder = new FSServiceOrder()
			{
				SrvOrdType = serviceOrder.SrvOrdType,
				RefNbr = serviceOrder.RefNbr,
			};
			AllServiceOrders.Current = storedServiceOrder;

			return new SchedulerServiceOrder[] { serviceOrder };
		}

		public SelectFrom<EPEmployee>
			.InnerJoin<Contact>.On<Contact.contactID.IsEqual<EPEmployee.defContactID>>
			.InnerJoin<SchedulerAppointmentEmployee>.On<SchedulerAppointmentEmployee.employeeID.IsEqual<EPEmployee.bAccountID>>
			.Where<SchedulerAppointmentEmployee.appointmentID.IsEqual<SchedulerAppointmentFilter.appointmentID.FromCurrent>>
			.View.ReadOnly SelectedAppointmentEmployees;
		#endregion

		#region LastUpdatedAppointment
		public SelectFrom<EPEmployee>
			.InnerJoin<SchedulerAppointmentEmployee>.On<SchedulerAppointmentEmployee.employeeID.IsEqual<EPEmployee.bAccountID>>
			.InnerJoin<SchedulerAppointment>.On<SchedulerAppointment.appointmentID.IsEqual<SchedulerAppointmentEmployee.appointmentID>>
			.InnerJoin<SchedulerServiceOrder>.On<SchedulerServiceOrder.sOID.IsEqual<SchedulerAppointment.sOID>>
			.Where<SchedulerAppointment.appointmentID.IsEqual<LastUpdatedAppointmentFilter.appointmentID.FromCurrent>>
			.View.ReadOnly LastUpdatedAppointment;

		[PXHidden]
		public SelectFrom<SchedulerAppointment>
			.InnerJoin<SchedulerServiceOrder>.On<SchedulerServiceOrder.sOID.IsEqual<SchedulerAppointment.sOID>>
			.LeftJoin<SchedulerAppointmentEmployee>.On<SchedulerAppointmentEmployee.appointmentID.IsEqual<SchedulerAppointment.appointmentID>>
			.LeftJoin<EPEmployee>.On<EPEmployee.bAccountID.IsEqual<SchedulerAppointmentEmployee.employeeID>>
			.Where<SchedulerAppointment.appointmentID.IsEqual<LastUpdatedAppointmentFilter.appointmentID.FromCurrent>>
			.View.ReadOnly LastUpdatedAppointment_Reversed;

		protected virtual IEnumerable lastUpdatedAppointment()
		{
			var delegateResult = new PXDelegateResult() { IsResultFiltered = true, IsResultTruncated = true, IsResultSorted = true };

			(int startRow, int totalRows) = (0, 0);
			var appointmentEntries = LastUpdatedAppointment_Reversed.View.Select(PXView.Currents, PXView.Parameters, PXView.Searches,
				PXView.SortColumns, PXView.Descendings, PXView.Filters, ref startRow, PXView.MaximumRows, ref totalRows);

			foreach (PXResult<SchedulerAppointment, SchedulerServiceOrder, SchedulerAppointmentEmployee, EPEmployee> it in appointmentEntries)
			{
				delegateResult.Add(new PXResult<EPEmployee, SchedulerAppointmentEmployee, SchedulerAppointment, SchedulerServiceOrder>(
					it ?? new EPEmployee(), it ?? new SchedulerAppointmentEmployee(), it, it));
			}

			return delegateResult;
		}



		#endregion

		//#region AppointmentsWithNoRoom
		//[PXFilterable]
		//public SelectFrom<FSSchedulerAppointment>
		//	.InnerJoin<SchedulerFSServiceOrder>.On<SchedulerFSServiceOrder.sOID.IsEqual<FSSchedulerAppointment.sOID>>
		//	.Where<FSSchedulerAppointment.canceled.IsEqual<False>
		//		.And<FSSchedulerAppointment.completed.IsEqual<False>>
		//		.And<FSSchedulerAppointment.closed.IsEqual<False>>
		//		.And<SchedulerFSServiceOrder.roomID.IsNull>
		//		>
		//	.View.ReadOnly AppointmentsWithNoRoom;
		//#endregion

		#region AppointmentsAllStaff
		[PXFilterable]
		// Not a real query -- only here to provide names for UI
		public EmployeeSelectBase
			.LeftJoin<SchedulerAppointmentEmployee>.On<SchedulerAppointmentEmployee.employeeID.IsEqual<EPEmployee.bAccountID>>
			.LeftJoin<SchedulerAppointment>.On<SchedulerAppointment.appointmentID.IsEqual<SchedulerAppointmentEmployee.appointmentID>>
			.LeftJoin<SchedulerServiceOrder>.On<SchedulerServiceOrder.sOID.IsEqual<SchedulerAppointment.sOID>>
			.View.ReadOnly AppointmentsAllStaff;

		[PXHidden]
		public EmployeeSelectBase
			.Where<EPEmployee.parentBAccountID.IsNotNull
				.And<EPEmployee.vStatus.IsNotEqual<VendorStatus.inactive>>
				.And<FSxEPEmployee.sDEnabled.IsEqual<True>>>
			.AggregateTo<GroupBy<EPEmployee.bAccountID>>
			.View.ReadOnly AppointmentsAllStaff_Employees;

		[PXHidden]
		public SelectFrom<EPEmployee>
			.LeftJoin<FSTimeSlot>.On<FSTimeSlot.employeeID.IsEqual<EPEmployee.bAccountID>
				.And<FSTimeSlot.timeStart.IsLessEqual<SchedulerDatesFilter.dateEnd.FromCurrent>
				.And<FSTimeSlot.timeEnd.IsGreaterEqual<SchedulerDatesFilter.dateBegin.FromCurrent>>
				.And<FSTimeSlot.slotLevel.IsNotEqual<Zero>>>>
			.Where<EPEmployee.bAccountID.IsIn<@P.AsInt>>
			.View.ReadOnly AppointmentsAllStaff_TimeSlots;

		// TODO: Do we need to check access rights here? How to display appointments when access to Customer is restricted?
		[PXHidden]
		public AppointmentEmployeeSelectBase
			.Where<SchedulerAppointment.scheduledDateTimeBegin.IsLessEqual<SchedulerDatesFilter.dateEnd.FromCurrent>
				.And<SchedulerAppointment.scheduledDateTimeBegin.IsGreaterEqual<SchedulerDatesFilter.dateBegin.FromCurrent>> // TODO: it was changed from scheduledDateTimeEnd for optimization, but it may miss out longer-than-day appointments
				.And<Brackets<
					SchedulerAppointment.isVisible.IsEqual<True>
					.Or<SchedulerAppointment.appointmentID.IsEqual<SchedulerAppointmentFilter.searchAppointmentID.FromCurrent>>>
				.And<EPEmployee.bAccountID.IsIn<@P.AsInt>>>>
			.View.ReadOnly AppointmentsAllStaff_Appointments;

		[PXHidden]
		public AppointmentEmployeeSelectBase
			.Where<SchedulerAppointment.appointmentID.IsEqual<SchedulerAppointmentFilter.searchAppointmentID.FromCurrent>
				.And<EPEmployee.bAccountID.IsIn<@P.AsInt>>>
			.View.ReadOnly AppointmentsAllStaff_HighlightedAppointment;

		[PXHidden]
		public AppointmentsNoEmployeesSelectBase
			.Where<SchedulerAppointment.scheduledDateTimeBegin.IsLessEqual<SchedulerDatesFilter.dateEnd.FromCurrent>
				.And<SchedulerAppointment.scheduledDateTimeBegin.IsGreaterEqual<SchedulerDatesFilter.dateBegin.FromCurrent>> // TODO: it was changed from scheduledDateTimeEnd for optimization
				.And<SchedulerAppointmentEmployee.employeeID.IsNull>
				.And<Brackets<
					SchedulerAppointment.canceled.IsEqual<False>>
					.And<SchedulerAppointment.completed.IsEqual<False>>
					.And<SchedulerAppointment.closed.IsEqual<False>>
					.Or<SchedulerAppointment.appointmentID.IsEqual<SchedulerAppointmentFilter.searchAppointmentID.FromCurrent>>>
				.And<AccessRightsCheckClause>
			>
			.View.ReadOnly AppointmentsAllStaff_NoEmployee;

		[PXHidden]
		public AppointmentsNoEmployeesSelectBase
			.Where<SchedulerAppointmentEmployee.employeeID.IsNull
				.And<SchedulerAppointment.appointmentID.IsEqual<SchedulerAppointmentFilter.searchAppointmentID.FromCurrent>>
				.And<AccessRightsCheckClause>
			>
			.View.ReadOnly AppointmentsAllStaff_NoEmployeeHighlighted;

		protected virtual IEnumerable appointmentsAllStaff()
		{
			PatchDatesFilter();

			// We're packing TimeSlots (AppointmentsAllStaff_TimeSlots) and appointments (AppointmentsAllStaff_Appointments)
			// in a sinlgle view. That is due to the fact that these data are operated as a whole from the client grid: same filters, synchronous update
			// TODO: check how we can separate this view in 2 parts.

			var delegateResult = new PXDelegateResult() { IsResultFiltered = true, IsResultTruncated = true, IsResultSorted = true };

			_dataHandler.StoreDatesFilter(DatesFilter.Current, PXContext.GetScreenID());

			var employeeDACs = new string[] { nameof(EPEmployee), nameof(FSEmployeeSkill), nameof(FSSkill), nameof(FSLicense),
				nameof(FSLicenseType), nameof(FSGeoZoneEmp), nameof(FSGeoZone), nameof(BranchAlias), nameof(InventoryItem) };

			var employeeParams = GetEmployeeParams(PXView.Searches, PXView.Filters.ForDACs(employeeDACs, true));
			var filter = AppointmentFilter.SelectSingle();
			var mainFilter = MainAppointmentFilter.SelectSingle();
			var keepFilters = !(filter?.SearchAppointmentID > 0) || HighlightedEntriesCountMatches(employeeParams);
			var searches = keepFilters ? PXView.Searches : new object[] { };
			var filters = keepFilters ? PXView.Filters : new PXFilterRow[] { };
			var employeeFilters = filters.ForDACs(employeeDACs, true);
			var mainFilters = filters.ExceptForDACs(employeeDACs, true);

			if (!keepFilters)
			{
				employeeParams = GetEmployeeParams(searches, employeeFilters);
				mainFilter.ResetFilters = true;
				MainAppointmentFilter.Cache.Update(filter);
			}

			if (employeeParams[0].Count > 0)
			{
				using (new PXFieldScope(AppointmentsAllStaff_TimeSlots.View, GetAppointmentFieldScopeFields()))
				{
					(int startRow, int totalRows) = (0, 0);
					var timeSlots = AppointmentsAllStaff_TimeSlots.View.Select(PXView.Currents, employeeParams, searches,
						PXView.SortColumns, PXView.Descendings, new PXFilterRow[] { }, ref startRow, PXView.MaximumRows, ref totalRows);
					delegateResult.AddRange(timeSlots);
				}

				using (new PXFieldScope(AppointmentsAllStaff_Appointments.View, GetAppointmentFieldScopeFields()))
				{
					(int startRow, int totalRows) = (0, 0);
					var appointments = AppointmentsAllStaff_Appointments.View.Select(PXView.Currents, employeeParams, null,
						PXView.SortColumns, PXView.Descendings, null, ref startRow, PXView.MaximumRows, ref totalRows);

					(startRow, totalRows) = (0, 0);
					var filteredAppointments = AppointmentsAllStaff_Appointments.View.Select(PXView.Currents, employeeParams, searches,
						PXView.SortColumns, PXView.Descendings, mainFilters, ref startRow, PXView.MaximumRows, ref totalRows)
						.RowCast<SchedulerAppointment>();
					var filteredAppointmentsIDs = filteredAppointments.Select(obj => obj.AppointmentID).ToHashSet();
					foreach (PXResult<EPEmployee, SchedulerAppointmentEmployee, SchedulerAppointment, SchedulerServiceOrder> entry in appointments)
					{
						SchedulerAppointment appointment = entry;
						SchedulerAppointmentEmployee appointmentEmployee = entry;
						appointmentEmployee.IsFilteredOut = !filteredAppointmentsIDs.Contains(appointment.AppointmentID);
						delegateResult.Add(entry);
					}
				}

				using (new PXFieldScope(AppointmentsAllStaff_NoEmployee.View, GetAppointmentFieldScopeFields()))
				{
					(int startRow, int totalRows) = (0, 0);
					var appointments = AppointmentsAllStaff_NoEmployee.View.Select(PXView.Currents, PXView.Parameters, searches,
						PXView.SortColumns, PXView.Descendings, mainFilters, ref startRow, PXView.MaximumRows, ref totalRows);

					foreach (PXResult<SchedulerAppointment, SchedulerAppointmentEmployee, SchedulerServiceOrder> it in appointments)
					{
						delegateResult.Add(new PXResult<EPEmployee, SchedulerAppointment, SchedulerAppointmentEmployee, SchedulerServiceOrder>(
							new EPEmployee(), it, it, it));
					}
				}
			}

			// We need to clear it on the server side:
			// Switching to another tab issues a <grid> request, and it doesn't contain updated views from the client
			filter.SearchAppointmentID = 0;
			AppointmentFilter.Cache.SetStatus(filter, PXEntryStatus.Notchanged); // Push into Cache

			return delegateResult;
		}

		protected List<int?>[] GetEmployeeParams(object[] searches, PXFilterRow[] filters)
		{
			(int startRow, int totalRows) = (0, 0);
			var command = AppointmentsAllStaff_Employees.View.BqlSelect;
			var parameters = new object[] { };
			var serviceFilters = filters.ForDACs(new string[] { nameof(InventoryItem) });
			if (serviceFilters.Length > 0)
			{
				var servicesSelect = new SelectFrom<InventoryItem>
					.Where<InventoryItem.itemType.IsEqual<INItemTypes.serviceItem>
					.And<InventoryItem.itemStatus.IsNotEqual<InventoryItemStatus.inactive>>
					.And<InventoryItem.itemStatus.IsNotEqual<InventoryItemStatus.markedForDeletion>>
					.And<InventoryItem.itemStatus.IsNotEqual<InventoryItemStatus.noSales>>>
					.View.ReadOnly(this);

				var services = servicesSelect.View.Select(PXView.Currents, null, null,
						null, null, serviceFilters, ref startRow, PXView.MaximumRows, ref totalRows).RowCast<InventoryItem>();
				var serviceIDs = services.Select(obj => obj.InventoryID).ToList();

				foreach (var serviceId in serviceIDs)
				{
					// given Service, Employee
					//   not exists Service.Skill that
					//     not exists in the list of the Employee's skills

					command = command.WhereAnd<Where<
						Not<Exists<SelectFrom<FSServiceSkill>
							.Where<FSServiceSkill.serviceID.IsEqual<@P.AsInt>
							.And<Not<Exists<SelectFrom<FSEmployeeSkill>
								.Where<FSEmployeeSkill.skillID.IsEqual<FSServiceSkill.skillID>
								.And<FSEmployeeSkill.employeeID.IsEqual<EPEmployee.bAccountID>>>>>>>>>>>();

					command = command.WhereAnd<Where<
						Not<Exists<SelectFrom<FSServiceLicenseType>
							.Where<FSServiceLicenseType.serviceID.IsEqual<@P.AsInt>
							.And<Not<Exists<SelectFrom<FSLicense>
								.Where<FSLicense.licenseTypeID.IsEqual<FSServiceLicenseType.licenseTypeID>
								.And<FSLicense.employeeID.IsEqual<EPEmployee.bAccountID>>>>>>>>>>>();

					parameters = parameters.Append(serviceId, serviceId);
				}
			}
			var view = new PXView(this, true, command);

			(startRow, totalRows) = (0, 0);
			var employeeFilters = filters.ExceptForDACs(new string[] { nameof(InventoryItem) });
			var employees = view.Select(PXView.Currents, parameters, searches,
				PXView.SortColumns, PXView.Descendings, employeeFilters, ref startRow, PXView.MaximumRows, ref totalRows)
				.RowCast<EPEmployee>();
			var employeeIDs = employees.Select(obj => obj.BAccountID).ToList();

			return new[] { employeeIDs };
		}

		protected bool HighlightedEntriesCountMatches(List<int?>[] employeeParams)
		{
			var appointment = HighlightedSearchAppointment.SelectSingle();
			if (appointment == null) return false;

			(int startRow, int totalRows) = (0, 0);
			if (appointment.StaffCntr == 0)
			{
				var entries = AppointmentsAllStaff_NoEmployeeHighlighted.View.Select(PXView.Currents, null, PXView.Searches, PXView.SortColumns,
					PXView.Descendings, PXView.Filters, ref startRow, PXView.MaximumRows, ref totalRows);
				return entries.Count > 0;
			}
			else
			{
				var entries = AppointmentsAllStaff_HighlightedAppointment.View.Select(PXView.Currents, employeeParams, PXView.Searches,
					PXView.SortColumns, PXView.Descendings, PXView.Filters, ref startRow, PXView.MaximumRows, ref totalRows);
				return entries.Count == appointment.StaffCntr;
			}
		}
		#endregion

		#region CacheAttached
		//public virtual void _(Events.CacheAttached<FSSrvOrdType.postToSOSIPM> e)
		//{

		//}
		//public virtual void _(Events.CacheAttached<FSSrvOrdType.allowInventoryItems> e)
		//{

		//}
		#endregion

		#region MergeAttributes
		#region Remove not needed long-loading attributes

		#region BAccount.acctCD
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXRemoveBaseAttribute(typeof(PXDimensionSelectorAttribute))]
		public virtual void _(Events.CacheAttached<BAccount.acctCD> e) { }
		#endregion

		#region FSSODet.inventoryID
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXRemoveBaseAttribute(typeof(PXDimensionSelectorAttribute))]
		public virtual void _(Events.CacheAttached<FSSODet.inventoryID> e) { }
		#endregion

		#region FSSrvOrdType.postToSOSIPM
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXRemoveBaseAttribute(typeof(PXFormulaAttribute))]
		public virtual void _(Events.CacheAttached<FSSrvOrdType.postToSOSIPM> e) { }
		#endregion

		#region FSSrvOrdType.allowInventoryItems
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXRemoveBaseAttribute(typeof(PXFormulaAttribute))]
		public virtual void _(Events.CacheAttached<FSSrvOrdType.allowInventoryItems> e) { }
		#endregion

		#region Vendor.included
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXRemoveBaseAttribute(typeof(PXUnboundDefaultAttribute))]
		public virtual void _(Events.CacheAttached<Vendor.included> e) { }
		#endregion
		#endregion

		#region Location.locationID
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXRemoveBaseAttribute(typeof(PXDimensionSelectorAttribute))]
		[PXSelector(typeof(Search<Location.locationID,
			Where<Location.isActive, Equal<True>,
				And<Where<Location.locType, Equal<LocTypeList.customerLoc>, Or<Location.locType, Equal<LocTypeList.combinedLoc>>>>>
			>),
			new Type[] { typeof(Location.locationID), typeof(Location.descr), typeof(Location.bAccountID) },
			CacheGlobal = true,
			SubstituteKey = typeof(Location.locationCD),
			DescriptionField = typeof(Location.descr))]
		public virtual void _(Events.CacheAttached<SchedulerServiceOrder.locationID> e) { }
		#endregion

		#region InventoryItem.inventoryID
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXDBInt()]
		[PXUIField(DisplayName = "Inventory ID")]
		[PXSelector(typeof(Search<InventoryItem.inventoryID,
			Where<InventoryItem.itemStatus, NotEqual<InventoryItemStatus.inactive>,
				And<InventoryItem.itemStatus, NotEqual<InventoryItemStatus.markedForDeletion>,
				And<InventoryItem.itemStatus, NotEqual<InventoryItemStatus.noSales>>>>>),
			SubstituteKey = typeof(InventoryItem.inventoryCD),
			DescriptionField = typeof(InventoryItem.descr))]
		public virtual void _(Events.CacheAttached<InventoryItem.inventoryID> e) { }
		#endregion

		#region INItemClass.itemClassID
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXDBInt()]
		[PXUIField(DisplayName = "Service Class")]
		[PXSelector(typeof(Search<INItemClass.itemClassID,
			Where<INItemClass.itemType, Equal<INItemTypes.serviceItem>>>),
			SubstituteKey = typeof(INItemClass.itemClassCD),
			DescriptionField = typeof(INItemClass.descr))]
		public virtual void _(Events.CacheAttached<INItemClass.itemClassID> e) { }
		#endregion

		#region FSSODet.projectTaskID
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXSelector(typeof(Search<PMTask.taskID,
			Where<PMTask.isCancelled, Equal<False>,
				And<PMTask.isCompleted, Equal<False>>>>),
			SubstituteKey = typeof(PMTask.taskCD),
			DescriptionField = typeof(PMTask.description))]
		public virtual void _(Events.CacheAttached<FSSODet.projectTaskID> e) { }
		#endregion

		#region FSSODet.SMequipmentID
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXSelector(typeof(Search<FSEquipment.SMequipmentID>),
			CacheGlobal = true,
			SubstituteKey = typeof(FSEquipment.refNbr),
			DescriptionField = typeof(FSEquipment.descr))]
		public virtual void _(Events.CacheAttached<FSSODet.SMequipmentID> e) { }
		#endregion

		#region FSSODet.newTargetEquipmentLineNbr>
		//[PXMergeAttributes(Method = MergeMethod.Merge)]
		//[PXSelector(typeof(Search<FSSODet.SMequipmentID>), CacheGlobal = true, SubstituteKey = typeof(FSEquipment.refNbr),
		//	DescriptionField = typeof(FSEquipment.descr))]
		//public virtual void _(Events.CacheAttached<FSSODet.newTargetEquipmentLineNbr> e) { }
		#endregion

		#region FSSODet.componentID
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXSelector(typeof(Search<FSModelTemplateComponent.componentID,
			Where<FSModelTemplateComponent.active, Equal<True>>>),
			CacheGlobal = true,
			SubstituteKey = typeof(FSModelTemplateComponent.componentCD),
			DescriptionField = typeof(FSModelTemplateComponent.descr))]
		public virtual void _(Events.CacheAttached<FSSODet.componentID> e) { }
		#endregion

		#region FSSODet.costCodeID
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXRemoveBaseAttribute(typeof(CostCodeDimensionSelectorAttribute))]
		[PXSelector(typeof(Search<PMCostCode.costCodeID,
			Where<PMCostCode.isActive, Equal<True>>>),
			CacheGlobal = true,
			SubstituteKey = typeof(PMCostCode.costCodeCD),
			DescriptionField = typeof(PMCostCode.description))]
		public virtual void _(Events.CacheAttached<FSSODet.costCodeID> e) { }
		#endregion

		#endregion

		public SchedulerMaint()
		: base()
		{
			PXUIFieldAttribute.SetDisplayName<SchedulerServiceOrder.docDesc>(Caches[typeof(SchedulerServiceOrder)], TX.CalendarMessages.SO_DESCR);
			PXUIFieldAttribute.SetDisplayName<SchedulerServiceOrder.locationID>(Caches[typeof(SchedulerServiceOrder)], TX.CalendarMessages.SO_LOCATION);
			//PXUIFieldAttribute.SetDisplayName<SchedulerFSServiceOrder.sourceReferenceNbr>(Caches[typeof(SchedulerFSServiceOrder)], TX.CalendarMessages.SO_LOCATION);
			PXUIFieldAttribute.SetDisplayName<FSSODet.tranDesc>(Caches[typeof(FSSODet)], TX.CalendarMessages.SODET_DESCR);
			PXUIFieldAttribute.SetDisplayName<BranchAlias.branchCD>(Caches[typeof(BranchAlias)], TX.CalendarMessages.EMPLOYEE_BRANCH);
			PXUIFieldAttribute.SetDisplayName<BranchAlias.acctName>(Caches[typeof(BranchAlias)], TX.CalendarMessages.EMPLOYEE_BRANCH_NAME);
			PXUIFieldAttribute.SetDisplayName<FSGeoZone.descr>(Caches[typeof(FSGeoZone)], TX.CalendarMessages.GEOZONE_DESCR);

			PXUIFieldAttribute.SetVisible<FSTimeSlot.timeStart>(Caches[typeof(FSTimeSlot)], null, false);
			PXUIFieldAttribute.SetVisible<FSTimeSlot.timeEnd>(Caches[typeof(FSTimeSlot)], null, false);
			PXUIFieldAttribute.SetVisible<FSTimeSlot.employeeID>(Caches[typeof(FSTimeSlot)], null, false);
			PXUIFieldAttribute.SetVisible<FSSODet.estimatedDuration>(Caches[typeof(FSSODet)], null, false);
			PXUIFieldAttribute.SetVisible<FSSODet.lineNbr>(Caches[typeof(FSSODet)], null, false);
			PXUIFieldAttribute.SetVisible<FSSODet.inventoryID>(Caches[typeof(FSSODet)], null, false);
			PXUIFieldAttribute.SetVisible<FSSODet.status>(Caches[typeof(FSSODet)], null, false);
			//PXUIFieldAttribute.SetVisible<FSAppointment.appointmentID>(Caches[typeof(FSAppointment)], null, false);
			PXUIFieldAttribute.SetVisible<SchedulerServiceOrder.sOID>(Caches[typeof(SchedulerServiceOrder)], null, false);

			PXUIFieldAttribute.SetVisible<SchedulerServiceOrder.projectID>(Caches[typeof(SchedulerServiceOrder)], null, true);
			PXUIFieldAttribute.SetVisible<SchedulerServiceOrder.contactID>(Caches[typeof(SchedulerServiceOrder)], null, true);

			PXUIFieldAttribute.SetDisplayName<SchedulerAppointment.status>(SelectedAppointment.Cache, TX.CalendarMessages.APPT_STATUS_SHORT);
			PXUIFieldAttribute.SetDisplayName<SchedulerAppointment.scheduledDateTimeBegin>(SelectedAppointment.Cache, TX.CalendarMessages.APPT_START_SHORT);
			PXUIFieldAttribute.SetDisplayName<SchedulerAppointment.scheduledDateTimeEnd>(SelectedAppointment.Cache, TX.CalendarMessages.APPT_END_SHORT);
			PXUIFieldAttribute.SetDisplayName<SchedulerServiceOrder.refNbr>(SelectedSO.Cache, TX.CalendarMessages.SO_REFNBR_SHORT);
			PXUIFieldAttribute.SetDisplayName<SchedulerServiceOrder.status>(SelectedSO.Cache, TX.CalendarMessages.SO_STATUS_SHORT);
			PXUIFieldAttribute.SetDisplayName<SchedulerServiceOrder.docDesc>(SelectedSO.Cache, TX.CalendarMessages.SO_DESCR_SHORT);
			PXUIFieldAttribute.SetDisplayName<SchedulerServiceOrder.projectID>(SelectedSO.Cache, TX.CalendarMessages.SO_PROJECT);
			PXUIFieldAttribute.SetDisplayName<SchedulerServiceOrder.waitingForParts>(SelectedSO.Cache, TX.CalendarMessages.SO_WAITING_FOR_PARTS);

			PXUIFieldAttribute.SetRequired<SchedulerAppointment.scheduledDateTimeBegin>(SelectedAppointment.Cache, false);
			PXUIFieldAttribute.SetRequired<SchedulerAppointment.scheduledDateTimeEnd>(SelectedAppointment.Cache, false);
			PXUIFieldAttribute.SetRequired<SchedulerServiceOrder.projectID>(SelectedSO.Cache, false);
			PXUIFieldAttribute.SetRequired<SchedulerServiceOrder.branchLocationCD>(SelectedSO.Cache, false);

			//CacheGlobal
			//FSSelectorBusinessAccount_CU_PR_VC

			// TODO: sort it out -- do we need any of those
			//PXUIFieldAttribute.SetEnabled(SelectedAppointment.Cache, null, false);
			//PXUIFieldAttribute.SetEnabled(SelectedAppointment_SO.Cache, null, false);
			//PXUIFieldAttribute.SetEnabled(SelectedAppointment_Customer.Cache, null, false);
			//PXUIFieldAttribute.SetEnabled(SelectedAppointment_Contact.Cache, null, false);
			//PXUIFieldAttribute.SetEnabled(SelectedAppointment_Employees.Cache, null, false);
			//SelectedAppointment.Cache.AllowSelect = false;
			//SelectedAppointment.Cache.AllowUpdate = false;
			//SelectedAppointment.Cache.AllowInsert = false;
			//SelectedAppointment.Cache.AllowDelete = false;
		}

		public virtual List<Type> GetSOFieldScopeFields()
		{
			List<Type> appointmentsFields = new List<Type>();
			var fields = new[] {
				typeof(EPEmployee.bAccountID),
				typeof(EPEmployee.acctCD),
				typeof(EPEmployee.acctName),
				typeof(BranchAlias.branchCD),
				typeof(BranchAlias.acctName),
				// FSServiceOrder
				typeof(SchedulerServiceOrder.srvOrdType),
				typeof(SchedulerServiceOrder.refNbr),
				typeof(SchedulerServiceOrder.sOID),
				typeof(SchedulerServiceOrder.status),
				typeof(SchedulerServiceOrder.customerID),
				typeof(SchedulerServiceOrder.estimatedDurationTotal),
				typeof(SchedulerServiceOrder.priority),
				typeof(SchedulerServiceOrder.severity),
				typeof(SchedulerServiceOrder.docDesc),
				typeof(SchedulerServiceOrder.sLAETA),
				typeof(SchedulerServiceOrder.docDesc),
				typeof(SchedulerServiceOrder.projectID),
				typeof(SchedulerServiceOrder.custPORefNbr),
				typeof(SchedulerServiceOrder.branchLocationCD),
				typeof(SchedulerServiceOrder.serviceContractRefNbr),
				typeof(SchedulerServiceOrder.waitingForParts),
				typeof(SchedulerServiceOrder.orderDate),
				typeof(SchedulerServiceOrder.sourceType),
				typeof(SchedulerServiceOrder.createdDateTime),
				typeof(SchedulerServiceOrder.assignedEmpID),
				typeof(SchedulerServiceOrder.locationID),
				typeof(SchedulerServiceOrder.contactID),
				// Customer
				typeof(SchedulerServiceOrder.customerAcctCD),
				typeof(SchedulerServiceOrder.customerAcctName),
				typeof(SchedulerServiceOrder.customerClassID),
				// Contact
				typeof(SchedulerServiceOrder.phone1),
				typeof(SchedulerServiceOrder.contactDisplayName),
				typeof(SchedulerServiceOrder.email),
				// FSAddress
				typeof(SchedulerServiceOrder.addressLine1),
				typeof(SchedulerServiceOrder.addressLine2),
				typeof(SchedulerServiceOrder.city),
				typeof(SchedulerServiceOrder.state),
				typeof(SchedulerServiceOrder.city),
				typeof(SchedulerServiceOrder.postalCode),
				typeof(SchedulerServiceOrder.countryID),
				typeof(SchedulerServiceOrder.fullAddress),
				// Branch
				typeof(SchedulerServiceOrder.branchCD),
				typeof(SchedulerServiceOrder.branchName),
				// FSBranchLocation
				typeof(SchedulerServiceOrder.branchLocationCD),
				typeof(SchedulerServiceOrder.branchLocationDescr),
				// FSProblem
				typeof(SchedulerServiceOrder.problemCD),
				typeof(SchedulerServiceOrder.problemDescr),

				typeof(FSSODet.lineNbr),
				typeof(FSSODet.refNbr),
				typeof(FSSODet.estimatedDuration),
				typeof(FSSODet.operation),
				typeof(FSSODet.tranDesc),
				typeof(FSSODet.inventoryID),
				typeof(FSSODet.billingRule),
				typeof(FSSODet.sOLineType),
				typeof(FSSODet.isPrepaid), 
				typeof(FSSODet.contractRelated), 
				typeof(FSSODet.manualCost),
				typeof(FSSODet.manualPrice),
				typeof(FSSODet.isFree),
				typeof(FSSODet.projectTaskID),
				typeof(FSSODet.sourceLineID),
				typeof(FSSODet.sourceNoteID),
				typeof(FSSODet.sourceLineNbr),
				typeof(FSSODet.sODetID),
				typeof(FSSODet.equipmentAction),
				typeof(FSSODet.SMequipmentID),
				typeof(FSSODet.componentID),
				typeof(FSSODet.equipmentLineRef),
				typeof(FSSODet.vendorID),
				typeof(FSSODet.poType),
				typeof(FSSODet.curyUnitCost),
				typeof(FSSODet.curyExtCost),
				typeof(FSSODet.manualDisc),
				typeof(FSSODet.discPct),
				typeof(FSSODet.discountID),
				typeof(FSSODet.discountSequenceID),
				typeof(FSSODet.status),
				typeof(FSSODet.costCodeID),
				typeof(FSSODet.costCodeDescr),
				typeof(InventoryItem.inventoryID),
				typeof(INItemClass.itemClassID),
				typeof(FSServiceSkill.skillID),
				typeof(FSServiceLicenseType.licenseTypeID),
				typeof(FSGeoZone.geoZoneCD),
				typeof(FSGeoZone.descr),
			};
			fields.ForEach(f =>
			{
				appointmentsFields.Add(f);
			});

			return appointmentsFields;
		}


		public virtual List<Type> GetAppointmentFieldScopeFields()
		{
			List<Type> appointmentsFields = new List<Type>();
			var fields = new[] {
				typeof(EPEmployee.bAccountID),
				typeof(EPEmployee.acctCD),
				typeof(EPEmployee.acctName),
				typeof(EPEmployee.departmentID),
				typeof(BranchAlias.branchCD),
				typeof(BranchAlias.acctName),
				typeof(FSTimeSlot.employeeID),
				typeof(FSTimeSlot.timeStart),
				typeof(FSTimeSlot.timeEnd),
				// FSAppointment
				typeof(SchedulerAppointment.appointmentID),
				typeof(SchedulerAppointment.srvOrdType),
				typeof(SchedulerAppointment.refNbr),
				typeof(SchedulerAppointment.scheduledDateTimeBegin),
				typeof(SchedulerAppointment.scheduledDateTimeEnd),
				typeof(SchedulerAppointment.status),
				typeof(SchedulerAppointment.mapLatitude),
				typeof(SchedulerAppointment.mapLongitude),
				typeof(SchedulerAppointment.confirmed),
				typeof(SchedulerAppointment.validatedByDispatcher),
				typeof(SchedulerAppointment.estimatedDurationTotal),
				typeof(SchedulerAppointment.staffCntr),
				typeof(SchedulerAppointment.docDesc),
				typeof(SchedulerAppointment.createdDateTime),
				typeof(SchedulerAppointment.locked),
				// FSAppointmentStatusColor
				typeof(SchedulerAppointment.bandColor),
				// FSServiceOrder
				typeof(SchedulerServiceOrder.srvOrdType),
				typeof(SchedulerServiceOrder.refNbr),
				typeof(SchedulerServiceOrder.sOID),
				typeof(SchedulerServiceOrder.status),
				typeof(SchedulerServiceOrder.estimatedDurationTotal),
				typeof(SchedulerServiceOrder.priority),
				typeof(SchedulerServiceOrder.severity),
				typeof(SchedulerServiceOrder.docDesc),
				typeof(SchedulerServiceOrder.sLAETA),
				typeof(SchedulerServiceOrder.docDesc),
				typeof(SchedulerServiceOrder.projectID),
				typeof(SchedulerServiceOrder.custPORefNbr),
				typeof(SchedulerServiceOrder.branchLocationCD),
				typeof(SchedulerServiceOrder.serviceContractRefNbr),
				typeof(SchedulerServiceOrder.waitingForParts),
				typeof(SchedulerServiceOrder.locationID),
				typeof(SchedulerServiceOrder.orderDate),
				// Customer
				typeof(SchedulerServiceOrder.customerAcctCD),
				typeof(SchedulerServiceOrder.customerAcctName),
				typeof(SchedulerServiceOrder.customerClassID),
				// Contact
				typeof(SchedulerServiceOrder.phone1),
				typeof(SchedulerServiceOrder.contactDisplayName),
				typeof(SchedulerServiceOrder.email),
				// FSAddress
				typeof(SchedulerServiceOrder.addressLine1),
				typeof(SchedulerServiceOrder.addressLine2),
				typeof(SchedulerServiceOrder.city),
				typeof(SchedulerServiceOrder.state),
				typeof(SchedulerServiceOrder.city),
				typeof(SchedulerServiceOrder.postalCode),
				typeof(SchedulerServiceOrder.countryID),
				typeof(SchedulerServiceOrder.fullAddress),
				// Branch
				typeof(SchedulerServiceOrder.branchCD),
				typeof(SchedulerServiceOrder.branchName),
				// FSBranchLocation
				typeof(SchedulerServiceOrder.branchLocationCD),
				typeof(SchedulerServiceOrder.branchLocationDescr),
				// FSProblem
				typeof(SchedulerServiceOrder.problemCD),
				typeof(SchedulerServiceOrder.problemDescr),

				typeof(InventoryItem.inventoryID),
				typeof(InventoryItem.itemClassID),
				typeof(FSEmployeeSkill.skillID),
				typeof(FSSkill.isDriverSkill),
				typeof(FSLicense.licenseTypeID),
				typeof(FSGeoZone.geoZoneCD),
				typeof(FSGeoZone.descr),
			};
			fields.ForEach(f =>
			{
				appointmentsFields.Add(f);
			});

			return appointmentsFields;
		}
	}
}
