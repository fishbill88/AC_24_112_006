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
using PX.Objects.AR;
using PX.Objects.CR;

namespace PX.Objects.FS
{
    #region PXProjection
    [Serializable]
    [PXProjection(typeof(
            Select5<FSAppointmentEmployee,
                        InnerJoin<FSAppointment,
                            On<FSAppointment.appointmentID, Equal<FSAppointmentEmployee.appointmentID>>,
                        InnerJoin<FSServiceOrder,
                            On<FSServiceOrder.sOID, Equal<FSAppointment.sOID>>,
                        LeftJoin<Customer,
                            On<Customer.bAccountID, Equal<FSServiceOrder.customerID>>,
                        LeftJoin<Location,
                            On<Location.locationID, Equal<FSServiceOrder.locationID>>,
                        InnerJoin<FSContact,
                            On<FSContact.contactID, Equal<FSServiceOrder.serviceOrderContactID>>,
                        LeftJoin<FSWFStage,
                            On<FSWFStage.wFStageID, Equal<FSAppointment.wFStageID>>>>>>>>,
                        Aggregate<
                            GroupBy<FSAppointmentEmployee.appointmentID,
                            GroupBy<FSAppointmentEmployee.employeeID,
                            GroupBy<FSAppointment.validatedByDispatcher,
                            GroupBy<FSAppointment.confirmed>>>>>>))]
    #endregion
    public class FSAppointmentStaffScheduleBoard : FSAppointmentScheduleBoard
    {
        public virtual FSAppointmentStaffScheduleBoard Clone()
        {
            return (FSAppointmentStaffScheduleBoard)MemberwiseClone();
        }

        #region EmployeeID
        public new abstract class employeeID : PX.Data.BQL.BqlInt.Field<employeeID> { }

        [PXDBInt(BqlField = typeof(FSAppointmentEmployee.employeeID))]
        public override int? EmployeeID { get; set; }
        #endregion
    }
}
