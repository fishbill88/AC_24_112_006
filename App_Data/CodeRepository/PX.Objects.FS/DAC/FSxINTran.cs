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
using PX.Objects.CS;
using PX.Objects.IN;

namespace PX.Objects.FS
{
    public class FSxINTran : PXCacheExtension<INTran>, IFSRelatedDoc
    {
        public static bool IsActive()
        {
            return PXAccess.FeatureInstalled<FeaturesSet.serviceManagementModule>()
                && PXAccess.FeatureInstalled<FeaturesSet.distributionModule>();
        }

        #region SrvOrdType
        public abstract class srvOrdType : PX.Data.BQL.BqlString.Field<srvOrdType> { }
        [PXDBString(4, IsFixed = true)]
        [PXUIField(DisplayName = "Service Order Type", FieldClass = FSSetup.ServiceManagementFieldClass)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual string SrvOrdType { get; set; }
        #endregion
        #region AppointmentRefNbr
        public abstract class appointmentRefNbr : PX.Data.BQL.BqlString.Field<appointmentRefNbr> { }
        [PXDBString(20, IsUnicode = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Appointment Nbr.", Visibility = PXUIVisibility.SelectorVisible, Visible = true, Enabled = true, FieldClass = FSSetup.ServiceManagementFieldClass)]
        public virtual string AppointmentRefNbr { get; set; }
        #endregion
        #region AppointmentLineNbr
        public abstract class appointmentLineNbr : PX.Data.BQL.BqlInt.Field<appointmentLineNbr> { }
        [PXDBInt]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Appointment Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false, FieldClass = FSSetup.ServiceManagementFieldClass)]
        public virtual Int32? AppointmentLineNbr { get; set; }
        #endregion
        #region ServiceOrderRefNbr
        public abstract class serviceOrderRefNbr : PX.Data.BQL.BqlString.Field<serviceOrderRefNbr> { }
        [PXDBString(15, IsUnicode = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Service Order Nbr.", Visibility = PXUIVisibility.SelectorVisible, Visible = true, Enabled = true, FieldClass = FSSetup.ServiceManagementFieldClass)]
        public virtual string ServiceOrderRefNbr { get; set; }
        #endregion
        #region ServiceOrderLineNbr
        public abstract class serviceOrderLineNbr : PX.Data.BQL.BqlInt.Field<serviceOrderLineNbr> { }
        [PXDBInt]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Service Order Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false, FieldClass = FSSetup.ServiceManagementFieldClass)]
        public virtual Int32? ServiceOrderLineNbr { get; set; }
		#endregion
		#region ServiceContractRefNbr
		public abstract class serviceContractRefNbr : PX.Data.BQL.BqlString.Field<serviceContractRefNbr> { }
		[PXString]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public string ServiceContractRefNbr
        {
            get
            {
                return string.Empty;
            }
        }
		#endregion
		#region ServiceContractPeriodID
		public abstract class serviceContractPeriodID : PX.Data.BQL.BqlInt.Field<serviceContractPeriodID> { }
		[PXInt]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		public int? ServiceContractPeriodID
        {
            get
            {
                return null;
            }
        }
        #endregion
    }
}
