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
using PX.Objects.CR;
using PX.Objects.CS;

namespace PX.Objects.FS
{
  [System.SerializableAttribute]
  public class RouteAppointmentForecastingFilter : PXBqlTable, PX.Data.IBqlTable
  {
        #region ServiceID
        public abstract class serviceID : PX.Data.BQL.BqlInt.Field<serviceID> { }
  
        [PXInt]
        [PXUIField(DisplayName = "Service ID")]
        [ServiceAttribute]
        public virtual int? ServiceID { get; set; }
        #endregion
        #region DateBegin
        public abstract class dateBegin : PX.Data.BQL.BqlDateTime.Field<dateBegin> { }
  
        [PXDate]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "From Date", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual DateTime? DateBegin { get; set; }
        #endregion
        #region DateEnd
  
        public abstract class dateEnd : PX.Data.BQL.BqlDateTime.Field<dateEnd> { }
  
        [PXDate]
        [PXUIField(DisplayName = "To Date", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual DateTime? DateEnd { get; set; }
        #endregion
        #region CustomerID
        public abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }

        [PXDBInt]
        [PXUIField(DisplayName = "Customer ID", Visibility = PXUIVisibility.SelectorVisible)]
        [FSSelectorBAccountTypeCustomerOrCombined]
        public virtual int? CustomerID { get; set; }
        #endregion
        #region CustomerLocationID
        public abstract class customerLocationID : PX.Data.BQL.BqlInt.Field<customerLocationID> { }

        [PXDBInt]
        [PXFormula(typeof(Default<RouteAppointmentForecastingFilter.customerID>))]
        [LocationID(typeof(Where<Location.bAccountID, Equal<Current<RouteAppointmentForecastingFilter.customerID>>>),
                    DescriptionField = typeof(Location.descr))]
        [PXUIField(DisplayName = "Customer Location")]
        public virtual int? CustomerLocationID { get; set; }
        #endregion
        #region RouteID
        public abstract class routeID : PX.Data.BQL.BqlInt.Field<routeID> { }

        [PXInt]
        [PXUIField(DisplayName = "Route ID")]
        [FSSelectorRouteID]
        public virtual int? RouteID { get; set; }
        #endregion
    }
}