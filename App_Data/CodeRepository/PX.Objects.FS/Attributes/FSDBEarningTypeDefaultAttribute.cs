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
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using PX.Objects.IN;
using System.Linq;

namespace PX.Objects.FS.Attributes
{
	public class FSDBEarningTypeDefaultAttribute : PXDefaultAttribute
	{
		public FSDBEarningTypeDefaultAttribute()
		{
			PersistingCheck = PXPersistingCheck.Nothing;
		}
		public override void FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			var appointmentLog = (FSAppointmentLog)e.Row;

			FSAppointmentEmployee appointmentEmployee = SelectFrom<FSAppointmentEmployee>
				.Where<FSAppointmentEmployee.appointmentID.IsEqual<P.AsInt>
					.And<FSAppointmentEmployee.employeeID.IsEqual<P.AsInt>
					.And<FSAppointmentEmployee.serviceLineRef.IsEqual<P.AsString>>>>
				.View
				.SelectSingleBound(sender.Graph, null, appointmentLog.DocID, appointmentLog.BAccountID, appointmentLog.DetLineRef);

			if (appointmentEmployee != null && appointmentEmployee.EarningType != null)
			{
				e.NewValue = appointmentEmployee.EarningType;
				return;
			}

			FSAppointmentDet detLine = SelectFrom<FSAppointmentDet>
				.Where<FSAppointmentDet.lineRef.IsEqual<P.AsString>
					.And<FSAppointmentDet.appointmentID.IsEqual<P.AsInt>
					.And<BAccountType.employeeType.IsEqual<P.AsString>>>>
				.View
				.SelectSingleBound(sender.Graph, null, appointmentLog.DetLineRef, appointmentLog.DocID, appointmentLog.BAccountType);

			if (detLine != null)
			{
				InventoryItem invItem = InventoryItem.PK.Find(sender.Graph, detLine.InventoryID);
				FSxService fsInvItem = PXCache<InventoryItem>.GetExtension<FSxService>(invItem);
				e.NewValue = fsInvItem.DfltEarningType;
				return;
			}

			FSSrvOrdType serviceOrderType = SelectFrom<FSSrvOrdType>
				.Where<FSSrvOrdType.srvOrdType.IsEqual<P.AsString>
					.And<BAccountType.employeeType.IsEqual<P.AsString>>>
				.View
				.SelectSingleBound(sender.Graph, null, appointmentLog.DocType, appointmentLog.BAccountType);

			if (serviceOrderType != null)
			{
				e.NewValue = serviceOrderType.DfltEarningType;
			}
		}
	}
}
