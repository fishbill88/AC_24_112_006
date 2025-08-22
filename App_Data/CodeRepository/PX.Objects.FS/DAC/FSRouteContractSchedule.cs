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
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.PM;
using PX.Objects.AR;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.IN;

namespace PX.Objects.FS
{
    [System.SerializableAttribute]
    [PXPrimaryGraph(typeof(RouteServiceContractScheduleEntry))]
    public class FSRouteContractSchedule : FSSchedule
	{
        #region Keys
        public new class PK : PrimaryKeyOf<FSRouteContractSchedule>.By<entityID, refNbr>
        {
            public static FSRouteContractSchedule Find(PXGraph graph, int? entityID, string refNbr, PKFindOptions options = PKFindOptions.None) => FindBy(graph, entityID, refNbr, options);
        }

        public new static class FK
        {
            public class Branch : GL.Branch.PK.ForeignKeyOf<FSRouteContractSchedule>.By<branchID> { }
            public class BranchLocation : FSBranchLocation.PK.ForeignKeyOf<FSRouteContractSchedule>.By<branchLocationID> { }
            public class Customer : AR.Customer.PK.ForeignKeyOf<FSRouteContractSchedule>.By<customerID> { }
            public class Vendor : AP.Vendor.PK.ForeignKeyOf<FSRouteContractSchedule>.By<vendorID> { }
            public class VehicleType : FSVehicleType.PK.ForeignKeyOf<FSRouteContractSchedule>.By<vehicleTypeID> { }
            public class Employee : EP.EPEmployee.PK.ForeignKeyOf<FSRouteContractSchedule>.By<employeeID> { }
            public class Project : PMProject.PK.ForeignKeyOf<FSRouteContractSchedule>.By<projectID> { }
            public class Task : PMTask.PK.ForeignKeyOf<FSRouteContractSchedule>.By<projectID, dfltProjectTaskID> { }
        }
        #endregion

        #region EntityID
        public new abstract class entityID : PX.Data.IBqlField
        {
        }

        [PXDBInt(IsKey = true)]
        [PXDefault]
        [PXUIField(DisplayName = "Service Contract ID")]
        [PXSelector(typeof(Search2<FSServiceContract.serviceContractID,
                            LeftJoinSingleTable<Customer,
                                On<Customer.bAccountID, Equal<FSServiceContract.customerID>>>,
                            Where<
                                FSServiceContract.recordType, Equal<FSServiceContract.recordType.RouteServiceContract>,
                                And<Where<Customer.bAccountID, IsNull, Or<Match<Customer, Current<AccessInfo.userName>>>>>>>),
                           SubstituteKey = typeof(FSServiceContract.refNbr),
                           CacheGlobal =  true)]
        public override int? EntityID { get; set; }
        #endregion
        #region RefNbr
        public new abstract class refNbr : PX.Data.IBqlField
        {
        }

        //Included in FSRouteContractScheduleFSServiceContract projection
        [PXDBString(15, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "Schedule ID", Visibility = PXUIVisibility.SelectorVisible)]
        [PXDefault]
        [PXSelector(typeof(Search<FSRouteContractSchedule.refNbr,
                           Where<FSRouteContractSchedule.entityID, Equal<Current<FSRouteContractSchedule.entityID>>,
                               And<FSRouteContractSchedule.entityType, Equal<FSRouteContractSchedule.entityType.Contract>>>,
                           OrderBy<Desc<FSRouteContractSchedule.refNbr>>>))]
        [AutoNumber(typeof(Search<FSSetup.scheduleNumberingID>), typeof(AccessInfo.businessDate))]
        public override string RefNbr { get; set; }
        #endregion
        #region CustomerID
        public abstract new class customerID : PX.Data.IBqlField
        {
        }

        [PXDBInt]
        [PXDefault]
        [PXUIField(DisplayName = "Customer", Enabled = false, Visibility = PXUIVisibility.SelectorVisible)]
        [PXFormula(typeof(Selector<FSContractSchedule.entityID, FSServiceContract.customerID>))]
        [FSSelectorContractScheduleCustomer(typeof(Where<FSServiceContract.recordType, Equal<FSServiceContract.recordType.RouteServiceContract>>),
			CacheGlobal = true)]
        [PXRestrictor(typeof(Where<Customer.status, IsNull,
               Or<Customer.status, Equal<CustomerStatus.active>,
               Or<Customer.status, Equal<CustomerStatus.oneTime>>>>),
               PX.Objects.AR.Messages.CustomerIsInStatus, typeof(Customer.status))]
        public override int? CustomerID { get; set; }
		#endregion
		#region CustomerLocationID
		public new abstract class customerLocationID : PX.Data.BQL.BqlInt.Field<customerLocationID> { }
		
		[FSLocationActive(
				typeof(Where<Location.bAccountID, Equal<Current<FSSchedule.customerID>>>),
				DescriptionField = typeof(Location.descr))]
		public override int? CustomerLocationID { get; set; }
		#endregion
		#region SrvOrdType
		public new abstract class srvOrdType : PX.Data.IBqlField
        {
        }

        [PXDBString(4, IsFixed = true)]
        [PXUIField(DisplayName = "Service Order Type")]
        [PXDefault(typeof(Coalesce<
            Search2<FSxUserPreferences.dfltSrvOrdType,
            InnerJoin<
                FSSrvOrdType, On<FSSrvOrdType.srvOrdType, Equal<FSxUserPreferences.dfltSrvOrdType>>>,
            Where<
                PX.SM.UserPreferences.userID, Equal<CurrentValue<AccessInfo.userID>>,
                And<FSSrvOrdType.behavior, Equal<FSSrvOrdType.behavior.Values.routeAppointment>>>>,
            Search<FSRouteSetup.dfltSrvOrdType>>))]
		[PXRestrictor(typeof(Where<FSSrvOrdType.active, Equal<True>>), null)]
        [FSSelectorRouteContractSrvOrdType]
        public override string SrvOrdType { get; set; }
        #endregion
        #region ScheduleGenType
        public new abstract class scheduleGenType : ListField_ScheduleGenType_ContractSchedule
        {
        }

        [PXDBString(2, IsUnicode = false)]
        [scheduleGenType.ListAtrribute]
        [PXUIField(DisplayName = "Schedule Generation Type")]
        [PXDefault(typeof(Search<FSServiceContract.scheduleGenType,
                                  Where<
                                       FSServiceContract.customerID, Equal<Current<FSRouteContractSchedule.customerID>>,
                                       And<FSServiceContract.serviceContractID, Equal<Current<FSRouteContractSchedule.entityID>>>>>))]
        public override string ScheduleGenType { get; set; }
        #endregion
        #region EntityType
        public new abstract class entityType : ListField_Schedule_EntityType
        {
        }
        #endregion
        #region ProjectID
        public new abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }

        [PXDefault(typeof(Search<FSServiceContract.projectID,
                          Where<
                              FSServiceContract.serviceContractID, Equal<Current<FSRouteContractSchedule.entityID>>,
                          And<
                              Current<FSRouteContractSchedule.entityType>, Equal<FSSchedule.entityType.Contract>>>>))]
        [PXRestrictor(typeof(Where<PMProject.isActive, Equal<True>>), PM.Messages.InactiveContract, typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), PM.Messages.CancelledContract, typeof(PMProject.contractCD))]
        [ProjectBase(typeof(customerID), Enabled = false)]
        public override int? ProjectID { get; set; }
        #endregion
        #region DfltProjectTaskID
        public new abstract class dfltProjectTaskID : PX.Data.BQL.BqlInt.Field<dfltProjectTaskID> { }

        [PXDBInt]
        [PXUIField(DisplayName = "Default Project Task", Enabled = false, FieldClass = ProjectAttribute.DimensionName)]
        [PXDefault(typeof(Search2<FSServiceContract.dfltProjectTaskID,
                            InnerJoin<FSSrvOrdType, On<FSSrvOrdType.srvOrdType, Equal<Current<srvOrdType>>>,
                            InnerJoin<PMTask, On<PMTask.taskID, Equal<FSServiceContract.dfltProjectTaskID>,
                                        And<PMTask.projectID, Equal<Current<projectID>>>>>>,
                          Where<
                              FSServiceContract.serviceContractID, Equal<Current<entityID>>,
                          And<
                              Current<entityType>, Equal<FSSchedule.entityType.Contract>,
                            And2<
                                Where<FSSrvOrdType.enableINPosting, Equal<False>, Or<PMTask.visibleInIN, Equal<True>>>,
                                And<
                                    Where2<
                                        Where<
                                            FSSrvOrdType.postTo, Equal<FSPostTo.None>>,
                                        Or<
                                            Where2<
                                                Where<
                                                    FSSrvOrdType.postTo, Equal<FSPostTo.Accounts_Receivable_Module>,
                                                    And<
                                                        Where<
                                                            PMTask.visibleInAR, Equal<True>>>>,
                                            Or<
                                                Where2<
                                                    Where<
                                                        FSSrvOrdType.postTo, Equal<FSPostTo.Sales_Order_Module>,
                                                            Or<FSSrvOrdType.postTo, Equal<FSPostTo.Sales_Order_Invoice>>>,
                                                    And<
                                                        Where<
                                                            PMTask.visibleInSO, Equal<True>>>>>>>>>>>>>),
            PersistingCheck = PXPersistingCheck.Nothing)]

        [FSSelectorActive_AR_SO_ProjectTask(typeof(Where<PMTask.projectID, Equal<Current<projectID>>>))]
        public override int? DfltProjectTaskID { get; set; }
        #endregion

        #region FormCaptionDescription
        [PXString]
        [PXFormula(typeof(Selector<customerID, Customer.acctName>))]
        public virtual string FormCaptionDescription { get; set; }
        #endregion
    }
}
