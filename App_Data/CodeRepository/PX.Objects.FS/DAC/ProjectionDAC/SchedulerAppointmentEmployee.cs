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
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.PM;
using PX.Objects.GL;
using PX.Data.EP;
using System;
using System.Collections.Generic;
using static PX.Objects.FS.SchedulerServiceOrder;
using static PX.Objects.FS.RouteAppointmentAssignmentHelper;

namespace PX.Objects.FS
{
	/// <exclude/>
	[Serializable]
	[PXProjection(typeof(Select<FSAppointmentEmployee>))]
	[PXHidden]
	public class SchedulerAppointmentEmployee : PXBqlTable, IBqlTable
	{
		#region EmployeeID
		public abstract class employeeID : PX.Data.BQL.BqlInt.Field<employeeID> { }

		[PXDBInt(BqlField = typeof(FSAppointmentEmployee.employeeID))]
		public virtual int? EmployeeID { get; set; }
		#endregion

		#region AppointmentID
		public abstract class appointmentID : PX.Data.BQL.BqlInt.Field<appointmentID> { }

		[PXDBInt(BqlField = typeof(FSAppointmentEmployee.appointmentID))]
		public virtual int? AppointmentID { get; set; }
		#endregion

		#region IsFilteredOut
		public abstract class isFilteredOut : Data.BQL.BqlBool.Field<isFilteredOut> { }

		[PXBool]
		[PXUIField(DisplayName = "", Enabled = false, Visible = false)]
		public virtual bool? IsFilteredOut { get; set; }
		#endregion
	}
}
