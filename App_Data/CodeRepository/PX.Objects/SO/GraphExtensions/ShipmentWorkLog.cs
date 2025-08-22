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

using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;

namespace PX.Objects.SO.GraphExtensions
{
	public class ShipmentWorkLog<TGraph> : PXGraphExtension<TGraph>
		where TGraph : PXGraph
	{
		public SelectFrom<SOShipmentProcessedByUser>.View ShipmentWorkLogView;

		#region Ensure
		public virtual SOShipmentProcessedByUser EnsureFor(string shipmentNbr, Guid? userID, [SOShipmentProcessedByUser.jobType.List] string jobType)
		{
			DateTime touchDateTime = GetServerTime();

			SOShipmentProcessedByUser NewLink(DateTime? groupStartDateTime) => new SOShipmentProcessedByUser
			{
				JobType = jobType,
				DocType = SOShipmentProcessedByUser.docType.Shipment,
				ShipmentNbr = shipmentNbr,
				UserID = userID,
				OverallStartDateTime = groupStartDateTime ?? touchDateTime,
				StartDateTime = touchDateTime,
				LastModifiedDateTime = touchDateTime
			};

			SOShipmentProcessedByUser openLink =
				SelectFrom<SOShipmentProcessedByUser>.
				Where<
					SOShipmentProcessedByUser.jobType.IsEqual<@P.AsString.Fixed.ASCII>.
					And<SOShipmentProcessedByUser.shipmentNbr.IsEqual<@P.AsString>>.
					And<SOShipmentProcessedByUser.userID.IsEqual<@P.AsGuid>>.
					And<SOShipmentProcessedByUser.endDateTime.IsNull>>.
				View.Select(Base, jobType, shipmentNbr, userID);

			SOShipmentProcessedByUser link = Ensure(openLink, touchDateTime, NewLink);

			var otherOpenLinks =
				SelectFrom<SOShipmentProcessedByUser>.
				Where<
					SOShipmentProcessedByUser.shipmentNbr.IsNotEqual<@P.AsString>.
					And<SOShipmentProcessedByUser.userID.IsEqual<@P.AsGuid>>.
					And<SOShipmentProcessedByUser.endDateTime.IsNull>>.
				View.Select(Base, shipmentNbr, userID);
			foreach (SOShipmentProcessedByUser linkToSuspend in otherOpenLinks)
				Suspend(linkToSuspend, touchDateTime);

			return link;
		}

		public virtual SOShipmentProcessedByUser EnsureFor(string worksheetNbr, int pickerNbr, Guid? userID, [SOShipmentProcessedByUser.jobType.List] string jobType)
		{
			DateTime touchDateTime = GetServerTime();

			SOShipmentProcessedByUser NewLink(DateTime? groupStartDateTime) => new SOShipmentProcessedByUser
			{
				JobType = jobType,
				DocType = SOShipmentProcessedByUser.docType.PickList,
				WorksheetNbr = worksheetNbr,
				PickerNbr = pickerNbr,
				UserID = userID,
				OverallStartDateTime = groupStartDateTime ?? touchDateTime,
				StartDateTime = touchDateTime,
				LastModifiedDateTime = touchDateTime
			};

			SOShipmentProcessedByUser openLink =
				SelectFrom<SOShipmentProcessedByUser>.
				Where<
					SOShipmentProcessedByUser.jobType.IsEqual<@P.AsString.Fixed.ASCII>.
					And<SOShipmentProcessedByUser.worksheetNbr.IsEqual<@P.AsString>>.
					And<SOShipmentProcessedByUser.pickerNbr.IsEqual<@P.AsInt>>.
					And<SOShipmentProcessedByUser.userID.IsEqual<@P.AsGuid>>.
					And<SOShipmentProcessedByUser.endDateTime.IsNull>>.
				View.Select(Base, jobType, worksheetNbr, pickerNbr, userID);

			SOShipmentProcessedByUser resultLink = Ensure(openLink, touchDateTime, NewLink);

			var otherOpenLinks =
				SelectFrom<SOShipmentProcessedByUser>.
				Where<
					SOShipmentProcessedByUser.worksheetNbr.IsNotEqual<@P.AsString>.
					And<SOShipmentProcessedByUser.pickerNbr.IsEqual<@P.AsInt>>.
					And<SOShipmentProcessedByUser.userID.IsEqual<@P.AsGuid>>.
					And<SOShipmentProcessedByUser.endDateTime.IsNull>>.
				View.Select(Base, worksheetNbr, pickerNbr, userID);
			foreach (SOShipmentProcessedByUser linkToSuspend in otherOpenLinks)
				Suspend(linkToSuspend, touchDateTime);

			return resultLink;
		}

		private SOShipmentProcessedByUser Ensure(SOShipmentProcessedByUser link, DateTime touchDateTime, Func<DateTime?, SOShipmentProcessedByUser> linkFactory)
		{
			TimeSpan pickingTimeout = GetPickingTimeOut();

			if (link == null)
			{
				return ShipmentWorkLogView.Insert(linkFactory(touchDateTime));
			}
			if (link.LastModifiedDateTime.Value.Add(pickingTimeout) > touchDateTime)
			{
				link.LastModifiedDateTime = link.LastModifiedDateTime.Value.Add(pickingTimeout);
				return ShipmentWorkLogView.Update(link);
			}

			// open log has expired
			link.EndDateTime = link.LastModifiedDateTime.Value.Add(pickingTimeout);
			ShipmentWorkLogView.Update(link);

			return ShipmentWorkLogView.Insert(linkFactory(link.OverallStartDateTime));
		}
		#endregion

		#region Suspend
		public virtual bool SuspendFor(string shipmentNbr, Guid? userID, [SOShipmentProcessedByUser.jobType.List] string jobType)
		{
			DateTime suspendDateTime = GetServerTime();

			SOShipmentProcessedByUser openLink =
				SelectFrom<SOShipmentProcessedByUser>.
				Where<
					SOShipmentProcessedByUser.jobType.IsEqual<@P.AsString.Fixed.ASCII>.
					And<SOShipmentProcessedByUser.shipmentNbr.IsEqual<@P.AsString>>.
					And<SOShipmentProcessedByUser.userID.IsEqual<@P.AsGuid>>.
					And<SOShipmentProcessedByUser.endDateTime.IsNull>>.
				View.Select(Base, jobType, shipmentNbr, userID);

			if (openLink != null)
			{
				Suspend(openLink, suspendDateTime);
				return true;
			}

			return false;
		}

		public virtual bool SuspendFor(string worksheetNbr, int pickerNbr, Guid? userID, [SOShipmentProcessedByUser.jobType.List] string jobType)
		{
			DateTime suspendDateTime = GetServerTime();

			SOShipmentProcessedByUser openLink =
				SelectFrom<SOShipmentProcessedByUser>.
				Where<
					SOShipmentProcessedByUser.jobType.IsEqual<@P.AsString.Fixed.ASCII>.
					And<SOShipmentProcessedByUser.worksheetNbr.IsEqual<@P.AsString>>.
					And<SOShipmentProcessedByUser.pickerNbr.IsEqual<@P.AsInt>>.
					And<SOShipmentProcessedByUser.userID.IsEqual<@P.AsGuid>>.
					And<SOShipmentProcessedByUser.endDateTime.IsNull>>.
				View.Select(Base, jobType, worksheetNbr, pickerNbr, userID);

			if (openLink != null)
			{
				Suspend(openLink, suspendDateTime);
				return true;
			}

			return false;
		}

		private void Suspend(SOShipmentProcessedByUser link, DateTime suspendDateTime)
		{
			if (link.NumberOfScans <= 1)
			{
				ShipmentWorkLogView.Delete(link);
				return;
			}

			TimeSpan pickingTimeout = GetPickingTimeOut();
			link.EndDateTime = Tools.Min(suspendDateTime, link.LastModifiedDateTime.Value.Add(pickingTimeout));
			ShipmentWorkLogView.Update(link);
		}
		#endregion

		#region Close
		public virtual void CloseFor(string shipmentNbr)
		{
			DateTime now = GetServerTime();

			foreach (SOShipmentProcessedByUser linkToClose in
				SelectFrom<SOShipmentProcessedByUser>.
				Where<SOShipmentProcessedByUser.shipmentNbr.IsEqual<@P.AsString>>.
				View.Select(Base, shipmentNbr))
			{
				Close(linkToClose, now);
			}
		}

		public virtual void CloseFor(string worksheetNbr, int pickerNbr)
		{
			DateTime now = GetServerTime();

			foreach (SOShipmentProcessedByUser linkToClose in
				SelectFrom<SOShipmentProcessedByUser>.
				Where<
					SOShipmentProcessedByUser.worksheetNbr.IsEqual<@P.AsString>.
					And<SOShipmentProcessedByUser.pickerNbr.IsEqual<@P.AsInt>>>.
				View.Select(Base, worksheetNbr, pickerNbr))
			{
				Close(linkToClose, now);
			}
		}

		private void Close(SOShipmentProcessedByUser link, DateTime closeDateTime)
		{
			if (link.NumberOfScans <= 1)
			{
				ShipmentWorkLogView.Delete(link);
				return;
			}

			TimeSpan pickingTimeout = GetPickingTimeOut();

			link.Confirmed = true;

			DateTime endDate = Tools.Min(closeDateTime, link.LastModifiedDateTime.Value.Add(pickingTimeout));

			if (link.EndDateTime == null)
				link.EndDateTime = endDate;

			link.OverallEndDateTime = endDate;

			ShipmentWorkLogView.Update(link);
		}
		#endregion

		#region LogScan
		public void LogScanFor(string shipmentNbr, Guid? userID, [SOShipmentProcessedByUser.jobType.List] string jobType, bool isError)
		{
			SOShipmentProcessedByUser link = EnsureFor(shipmentNbr, userID, jobType);
			LogScan(link, isError);
		}

		public void LogScanFor(
			string worksheetNbr,
			int pickerNbr,
			Guid userID,
			[SOShipmentProcessedByUser.jobType.List] string jobType,
			bool isError)
		{
			SOShipmentProcessedByUser link = EnsureFor(worksheetNbr, pickerNbr, userID, jobType);
			LogScan(link, isError);
		}
		protected void LogScan(SOShipmentProcessedByUser link, bool isError)
		{
			link.NumberOfScans++;
			if (isError)
				link.NumberOfFailedScans++;
			ShipmentWorkLogView.Update(link);
		}
		#endregion
		public virtual void PersistWorkLog()
		{
			using (var tran = new PXTransactionScope())
			{
				ShipmentWorkLogView.Cache.Persist(PXDBOperation.Insert);
				ShipmentWorkLogView.Cache.Persist(PXDBOperation.Update);
				ShipmentWorkLogView.Cache.Persist(PXDBOperation.Delete);

				tran.Complete(Base);
			}

			ShipmentWorkLogView.Cache.Persisted(false);
		}

		protected virtual TimeSpan GetPickingTimeOut() => TimeSpan.FromMinutes(10);
		protected virtual DateTime GetServerTime()
		{
			PXDatabase.SelectDate(out DateTime _, out var dbNow);
			dbNow = PXTimeZoneInfo.ConvertTimeFromUtc(dbNow, LocaleInfo.GetTimeZone());
			return dbNow;
		}
	}
}
