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

namespace PX.Objects.FS
{
	/// <exclude/>
	#region PXProjection
	[Serializable]
	[PXProjection(typeof(Select2<FSServiceOrder,
		InnerJoin<Customer, On<Customer.bAccountID.IsEqual<FSServiceOrder.customerID>>,
		InnerJoin<Location, On<Location.locationID.IsEqual<FSServiceOrder.locationID>>,
		InnerJoin<FSContact, On<FSContact.contactID.IsEqual<FSServiceOrder.serviceOrderContactID>>,
		InnerJoin<FSAddress, On<FSAddress.addressID.IsEqual<FSServiceOrder.serviceOrderAddressID>>,
		LeftJoin<Branch, On<Branch.branchID.IsEqual<FSServiceOrder.branchID>>,
		LeftJoin<FSBranchLocation, On<FSBranchLocation.branchLocationID.IsEqual<FSServiceOrder.branchLocationID>>,
		LeftJoin<FSProblem, On<FSProblem.problemID.IsEqual<FSServiceOrder.problemID>>,
		LeftJoin<FSServiceContract, On<FSServiceContract.serviceContractID.IsEqual<FSServiceOrder.billServiceContractID>>
		>>>>>>>>>))]

	#endregion

	[PXCacheName(TX.TableName.SchedulerServiceOrder)]
	public class SchedulerServiceOrder : PXBqlTable, IBqlTable
	{
		#region FSServiceOrder
		#region SrvOrdType
		public abstract class srvOrdType : PX.Data.BQL.BqlString.Field<srvOrdType> { }

		[PXDBString(4, IsFixed = true, IsKey = true, IsUnicode = true, BqlField = typeof(FSServiceOrder.srvOrdType))]
		[PXUIField(DisplayName = "Service Order Type")]
		[FSSelectorSrvOrdTypeNOTQuote]
		[PXRestrictor(typeof(Where<FSSrvOrdType.active, Equal<True>>), null)]
		[PX.Data.EP.PXFieldDescription]
		public virtual string SrvOrdType { get; set; }
		#endregion

		#region SOID
		public abstract class sOID : PX.Data.BQL.BqlInt.Field<sOID> { }

		[PXDBInt(BqlField = typeof(FSServiceOrder.sOID))]
		public virtual int? SOID { get; set; }
		#endregion

		#region RefNbr
		public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }

		[PXDBString(15, IsUnicode = true, BqlField = typeof(FSServiceOrder.refNbr))]
		[PXUIField(DisplayName = "Service Order Nbr.")]
		[FSSelectorSORefNbr]
		public virtual string RefNbr { get; set; }
		#endregion

		#region Status
		public abstract class status : PX.Data.BQL.BqlString.Field<status>
		{
			public abstract class Values : ListField.ServiceOrderStatus { }
		}

		[PXDBString(1, IsFixed = true, BqlField = typeof(FSServiceOrder.status))]
		[PXUIField(DisplayName = "Service Order Status", Enabled = false)]
		[status.Values.List]
		public virtual string Status { get; set; }
		#endregion

		#region RoomID
		public abstract class roomID : PX.Data.BQL.BqlString.Field<roomID> { }

		[PXDBString(1, IsFixed = true, BqlField = typeof(FSServiceOrder.roomID))]
		[PXSelector(typeof(FSRoom.roomID), CacheGlobal = true, DescriptionField = typeof(FSRoom.descr))]
		[PXUIField(DisplayName = "Room ID")]
		public virtual string RoomID { get; set; }
		#endregion

		#region CustomerID
		public abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }

		[PXDBInt(BqlField = typeof(FSServiceOrder.customerID))]
		[PXUIField(DisplayName = "Customer", Required = false)]
		[FSSelectorBusinessAccount_CU_PR_VC]
		public virtual int? CustomerID { get; set; }
		#endregion

		#region ContactID
		public abstract class contactID : PX.Data.BQL.BqlInt.Field<contactID> { }

		[PXDBInt(BqlField = typeof(FSServiceOrder.contactID))]
		[FSSelectorContact(typeof(FSServiceOrder.customerID))]
		[PXUIField(DisplayName = "Contact")]
		public virtual int? ContactID { get; set; }
		#endregion

		#region EstimatedDurationTotal
		public abstract class estimatedDurationTotal : PX.Data.BQL.BqlInt.Field<estimatedDurationTotal> { }

		[PXDBTimeSpanLong(Format = TimeSpanFormatType.LongHoursMinutes, BqlField = typeof(FSServiceOrder.estimatedDurationTotal))]
		[PXUIField(DisplayName = "Estimated Duration", Enabled = false)]
		public virtual int? EstimatedDurationTotal { get; set; }
		#endregion

		#region Priority
		public abstract class priority : ListField_Priority_ServiceOrder { }

		[PXDBString(1, IsFixed = true, BqlField = typeof(FSServiceOrder.priority))]
		[PXUIField(DisplayName = "Priority")]
		[priority.ListAtrribute]
		public virtual string Priority { get; set; }
		#endregion

		#region Severity
		public abstract class severity : ListField_Severity_ServiceOrder { }

		[PXDBString(1, IsFixed = true, BqlField = typeof(FSServiceOrder.severity))]
		[PXUIField(DisplayName = "Severity")]
		[severity.ListAtrribute]
		public virtual string Severity { get; set; }
		#endregion

		#region DocDesc
		public abstract class docDesc : PX.Data.BQL.BqlString.Field<docDesc> { }

		[PXDBString(Common.Constants.TranDescLength, IsUnicode = true, BqlField = typeof(FSServiceOrder.docDesc))]
		[PXUIField(DisplayName = "Description")]
		public virtual string DocDesc { get; set; }
		#endregion

		#region SLAETA
		public abstract class sLAETA : PX.Data.BQL.BqlDateTime.Field<sLAETA> { }

		[PXDBDateAndTime(UseTimeZone = true, PreserveTime = true, DisplayNameDate = "SLA", BqlField = typeof(FSServiceOrder.sLAETA))]
		[PXUIField(DisplayName = "SLA")]
		public virtual DateTime? SLAETA { get; set; }
		#endregion

		#region ProjectID
		public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }

		[ProjectBase(typeof(FSServiceOrder.billCustomerID), BqlField = typeof(FSServiceOrder.projectID))]
		public virtual int? ProjectID { get; set; }
		#endregion

		#region CustPORefNbr
		public abstract class custPORefNbr : PX.Data.BQL.BqlString.Field<custPORefNbr> { }

		[PXDBString(40, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC", BqlField = typeof(FSServiceOrder.custPORefNbr))]
		[NormalizeWhiteSpace]
		[FSSelectorBusinessAccount_CU_PR_VC]
		[PXUIField(DisplayName = "Customer Order")]
		public virtual string CustPORefNbr { get; set; }
		#endregion

		#region WaitingForParts
		public abstract class waitingForParts : PX.Data.BQL.BqlBool.Field<waitingForParts> { }

		[PXDBBool(BqlField = typeof(FSServiceOrder.waitingForParts))]
		[PXFormula(typeof(IIf<Where<pendingPOLineCntr, Greater<int0>>, True, False>))]
		[PXUIVisible(typeof(
			Where<Current<FSSrvOrdType.postTo>, Equal<FSPostTo.Sales_Order_Invoice>,
				Or<Current<FSSrvOrdType.postTo>, Equal<FSPostTo.Sales_Order_Module>,
				Or<Current<FSSrvOrdType.postTo>, Equal<FSPostTo.Projects>>>>))]
		[PXUIField(DisplayName = "Waiting for Purchased Items", Enabled = false, FieldClass = "DISTINV")]
		public virtual bool? WaitingForParts { get; set; }
		#endregion

		#region PendingPOLineCntr
		public abstract class pendingPOLineCntr : PX.Data.BQL.BqlInt.Field<pendingPOLineCntr> { }

		[PXDBInt(BqlField = typeof(FSServiceOrder.pendingPOLineCntr))]
		public virtual int? PendingPOLineCntr { get; set; }
		#endregion

		#region LocationID
		public abstract class locationID : PX.Data.BQL.BqlInt.Field<locationID> { }

		[FSLocationActive(typeof(
			Where<Location.bAccountID, Equal<Current<FSServiceOrder.customerID>>,
				And<MatchWithBranch<Location.cBranchID>>>),
					DescriptionField = typeof(Location.descr), DisplayName = "Location", DirtyRead = true, BqlField = typeof(FSServiceOrder.locationID))]
		public virtual int? LocationID { get; set; }
		#endregion

		#region OrderDate
		public abstract class orderDate : PX.Data.BQL.BqlDateTime.Field<orderDate> { }

		[PXDBDate(DisplayMask = "d", BqlField = typeof(FSServiceOrder.orderDate))]
		[PXUIField(DisplayName = "Date")]
		public virtual DateTime? OrderDate { get; set; }
		#endregion

		#region SourceType
		public abstract class sourceType : ListField_SourceType_ServiceOrder { }

		[PXDBString(2, IsFixed = true, BqlField = typeof(FSServiceOrder.sourceType))]
		[PXUIField(DisplayName = "Document Type", Enabled = false)]
		[sourceType.ListAtrribute]
		public virtual string SourceType { get; set; }
		#endregion

		#region AssignedEmpID
		public abstract class assignedEmpID : PX.Data.BQL.BqlInt.Field<assignedEmpID> { }

		[PXDBInt(BqlField = typeof(FSServiceOrder.assignedEmpID))]
		[PXUIField(DisplayName = "Supervisor")]
		public virtual int? AssignedEmpID { get; set; }
		#endregion

		#region CreatedDateTime
		public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }

		[PXDBCreatedDateTime(BqlField = typeof(FSServiceOrder.createdDateTime))]
		[PXUIField(DisplayName = PXDBLastModifiedByIDAttribute.DisplayFieldNames.CreatedDateTime, Enabled = false, IsReadOnly = true)]
		public virtual DateTime? CreatedDateTime { get; set; }
		#endregion

		#region Hidden in UI
		#region ServiceOrderContactID
		public abstract class serviceOrderContactID : PX.Data.BQL.BqlInt.Field<serviceOrderContactID> { }

		[PXDBInt(BqlField = typeof(FSServiceOrder.createdDateTime))]
		public virtual int? ServiceOrderContactID { get; set; }
		#endregion


		#region ServiceOrderAddressID
		public abstract class serviceOrderAddressID : PX.Data.BQL.BqlInt.Field<serviceOrderAddressID> { } // : serviceOrderContactID { }

		[PXDBInt(BqlField = typeof(FSServiceOrder.serviceOrderAddressID))]
		public virtual int? ServiceOrderAddressID { get; set; }
		#endregion

		#region ProblemID
		public abstract class problemID : PX.Data.BQL.BqlInt.Field<problemID> { }

		[PXDBInt(BqlField = typeof(FSServiceOrder.problemID))]
		public virtual int? ProblemID { get; set; }
		#endregion

		#region ServiceContractID
		public abstract class serviceContractID : PX.Data.BQL.BqlInt.Field<serviceContractID> { }

		[PXDBInt(BqlField = typeof(FSServiceOrder.serviceContractID))]
		public virtual int? ServiceContractID { get; set; }
		#endregion

		#region Quote
		public abstract class quote : PX.Data.BQL.BqlBool.Field<quote> { }

		[PXDBBool(BqlField = typeof(FSServiceOrder.quote))]
		public virtual bool? Quote { get; set; }
		#endregion

		#region Hold
		public abstract class hold : PX.Data.BQL.BqlBool.Field<hold> { }

		[PXDBBool(BqlField = typeof(FSServiceOrder.hold))]
		public virtual bool? Hold { get; set; }
		#endregion

		#region Closed
		public abstract class closed : PX.Data.BQL.BqlBool.Field<closed> { }

		[PXDBBool(BqlField = typeof(FSServiceOrder.closed))]
		public virtual bool? Closed { get; set; }
		#endregion

		#region Canceled
		public abstract class canceled : PX.Data.BQL.BqlBool.Field<canceled> { }

		[PXDBBool(BqlField = typeof(FSServiceOrder.canceled))]
		public virtual bool? Canceled { get; set; }
		#endregion

		#region Completed
		public abstract class completed : PX.Data.BQL.BqlBool.Field<completed> { }

		[PXDBBool(BqlField = typeof(FSServiceOrder.completed))]
		public virtual bool? Completed { get; set; }
		#endregion

		#region AppointmentsNeeded
		public abstract class appointmentsNeeded : PX.Data.BQL.BqlBool.Field<appointmentsNeeded> { }

		[PXDBBool(BqlField = typeof(FSServiceOrder.appointmentsNeeded))]
		public virtual bool? AppointmentsNeeded { get; set; }
		#endregion
		#endregion
		#endregion

		#region Customer
		#region CustomerAcctCD
		public new abstract class customerAcctCD : PX.Data.BQL.BqlString.Field<customerAcctCD> { }

		[CustomerRaw(IsKey = true, BqlField = typeof(Customer.acctCD))]
		[PXUIField(DisplayName = "Customer ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXFieldDescription]
		[PXPersonalDataWarning]
		public virtual string CustomerAcctCD { get; set; }
		#endregion

		#region CustomerAcctName
		public new abstract class customerAcctName : PX.Data.BQL.BqlString.Field<customerAcctName> { }
		[PXDBString(255, IsUnicode = true, BqlField = typeof(Customer.acctName))]
		[PXUIField(DisplayName = "Customer Name", Visibility = PXUIVisibility.SelectorVisible)]
		[PXFieldDescription]
		[PXPersonalDataField]
		public virtual string CustomerAcctName { get; set; }
		#endregion

		#region CustomerClassID
		public abstract class customerClassID : PX.Data.BQL.BqlString.Field<customerClassID> { }

		[PXDBString(10, IsUnicode = true, BqlField = typeof(Customer.customerClassID))]
		[PXSelector(typeof(CustomerClass.customerClassID),
			CacheGlobal = true,
			DescriptionField = typeof(CustomerClass.descr))]
		[PXUIField(DisplayName = "Customer Class")]
		public virtual string CustomerClassID { get; set; }
		#endregion
		#endregion

		#region FSContact
		#region Phone1
		public abstract class phone1 : PX.Data.BQL.BqlString.Field<phone1> { }

		[PXDBString(50, BqlField = typeof(FSContact.phone1))]
		[PXUIField(DisplayName = "Phone 1", Visibility = PXUIVisibility.SelectorVisible)]
		[PXPersonalDataField]
		public virtual string Phone1 { get; set; }
		#endregion

		#region ContactDisplayName
		public abstract class contactDisplayName : PX.Data.BQL.BqlString.Field<contactDisplayName> { }

		[PXUIField(DisplayName = "Display Name", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[PXDependsOnFields(typeof(lastName), typeof(firstName), typeof(title))]
		[CR.ContactDisplayName(typeof(lastName), typeof(firstName), typeof(midName), typeof(title), true)]
		[PXFieldDescription]
		[PXPersonalDataField]
		public virtual string ContactDisplayName { get; set; }
		#endregion

		#region Title
		public abstract class title : PX.Data.BQL.BqlString.Field<title> { }

		[PXDBString(50, IsUnicode = true, BqlField = typeof(FSContact.title))]
		public virtual string Title { get; set; }
		#endregion

		#region FirstName
		public abstract class firstName : PX.Data.BQL.BqlString.Field<firstName> { }

		[PXDBString(50, IsUnicode = true, BqlField = typeof(FSContact.firstName))]
		[PXPersonalDataField]
		public virtual string FirstName { get; set; }
		#endregion

		#region LastName
		public abstract class lastName : PX.Data.BQL.BqlString.Field<lastName> { }

		[PXDBString(100, IsUnicode = true, BqlField = typeof(FSContact.lastName))]
		[PXPersonalDataField]
		public virtual string LastName { get; set; }
		#endregion

		#region MidName
		public abstract class midName : PX.Data.BQL.BqlString.Field<midName> { }

		[PXDBString(50, IsUnicode = true, BqlField = typeof(FSContact.midName))]
		[PXPersonalDataField]
		public virtual string MidName { get; set; }
		#endregion

		#region Email
		public abstract class email : PX.Data.BQL.BqlString.Field<email> { }

		[PXDBEmail(BqlField = typeof(FSContact.email))]
		[PXUIField(DisplayName = "Email", Visibility = PXUIVisibility.SelectorVisible)]
		[PXPersonalDataField]
		public virtual string Email { get; set; }
		#endregion
		#endregion

		#region FSAddress
		#region AddressLine1
		public abstract class addressLine1 : PX.Data.BQL.BqlString.Field<addressLine1> { }

		[PXDBString(50, IsUnicode = true, BqlField = typeof(FSAddress.addressLine1))]
		[PXUIField(DisplayName = "Address Line 1", Visibility = PXUIVisibility.SelectorVisible)]
		[PXPersonalDataField]
		public virtual string AddressLine1 { get; set; }
		#endregion

		#region AddressLine2
		public abstract class addressLine2 : PX.Data.BQL.BqlString.Field<addressLine2> { }

		[PXDBString(50, IsUnicode = true, BqlField = typeof(FSAddress.addressLine2))]
		[PXUIField(DisplayName = "Address Line 2")]
		[PXPersonalDataField]
		public virtual string AddressLine2 { get; set; }
		#endregion
		#region City
		public abstract class city : PX.Data.BQL.BqlString.Field<city> { }

		[PXDBString(50, IsUnicode = true, BqlField = typeof(FSAddress.city))]
		[PXUIField(DisplayName = "City", Visibility = PXUIVisibility.SelectorVisible)]
		[PXPersonalDataField]
		public virtual string City { get; set; }
		#endregion

		#region CountryID
		public abstract class countryID : PX.Data.BQL.BqlString.Field<countryID> { }

		[PXDBString(100, BqlField = typeof(FSAddress.countryID))]
		[PXUIField(DisplayName = "Country")]
		public virtual string CountryID { get; set; }
		#endregion

		#region State
		public abstract class state : PX.Data.BQL.BqlString.Field<state> { }

		[PXDBString(50, IsUnicode = true, BqlField = typeof(FSAddress.state))]
		[PXUIField(DisplayName = "State")]
		public virtual string State { get; set; }
		#endregion

		#region PostalCode
		public abstract class postalCode : PX.Data.BQL.BqlString.Field<postalCode> { }

		[PXDBString(20, BqlField = typeof(FSAddress.postalCode))]
		[PXUIField(DisplayName = "Postal Code")]
		[PXPersonalDataField]
		public virtual String PostalCode { get; set; }
		#endregion

		#region FullAddress
		public new abstract class fullAddress : PX.Data.BQL.BqlString.Field<fullAddress> { }

		[PXString]
		[PXUIField(DisplayName = "Address", Enabled = false, Visible = true)]
		[PXFormula(typeof(SmartJoin<CommaSpace,
			SmartJoin<Space, addressLine1, addressLine2>,
			SmartJoin<Space, city, state, postalCode>>))]

		public virtual string FullAddress { get; set; }
		#endregion
		#endregion

		#region Branch
		#region BranchCD
		public abstract class branchCD : PX.Data.BQL.BqlString.Field<branchCD> { }

		[PXDBString(30, IsUnicode = true, IsKey = true, InputMask = "", BqlField = typeof(Branch.branchCD))]
		[PXUIField(DisplayName = "Branch ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Branch.branchCD), DescriptionField = typeof(Branch.acctName))]
		public virtual string BranchCD { get; set; }

		#endregion
		#region BranchName
		public abstract class branchName : PX.Data.BQL.BqlString.Field<branchName> { }

		[PXDBScalar(typeof(Search<BAccount.acctName, Where<BAccount.bAccountID, Equal<Branch.bAccountID>>>), BqlField = typeof(Branch.acctName))]
		[PXString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Branch Name", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual string BranchName { get; set; }
		#endregion
		#endregion

		#region FSBranchLocation
		#region BranchLocationCD
		public abstract class branchLocationCD : PX.Data.BQL.BqlString.Field<branchLocationCD> { }

		[PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC", IsFixed = true, BqlField = typeof(FSBranchLocation.branchLocationCD))]
		[PXSelector(typeof(FSBranchLocation.branchLocationCD), DescriptionField = typeof(FSBranchLocation.descr))]
		[PXUIField(DisplayName = "Branch Location ID", Visibility = PXUIVisibility.SelectorVisible)]
		[NormalizeWhiteSpace]
		public virtual string BranchLocationCD { get; set; }
		#endregion

		#region BranchLocationDescr
		public abstract class branchLocationDescr : PX.Data.BQL.BqlString.Field<branchLocationDescr> { }

		[PXDBLocalizableString(60, IsUnicode = true, BqlField = typeof(FSBranchLocation.descr))]
		[PXUIField(DisplayName = "Branch Location Description", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual string BranchLocationDescr { get; set; }
		#endregion
		#endregion

		#region FSProblem
		#region ProblemCD
		public abstract class problemCD : PX.Data.BQL.BqlString.Field<problemCD> { }

		[PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC", IsFixed = true, BqlField = typeof(FSProblem.problemCD))]
		[NormalizeWhiteSpace]
		[PXSelector(typeof(FSProblem.problemCD), CacheGlobal = true, DescriptionField = typeof(FSProblem.descr))]
		[PXUIField(DisplayName = "Problem ID", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual string ProblemCD { get; set; }
		#endregion

		#region ProblemDescr
		public abstract class problemDescr : PX.Data.BQL.BqlString.Field<problemDescr> { }

		[PXDBLocalizableString(60, IsUnicode = true, BqlField = typeof(FSProblem.descr))]
		[PXUIField(DisplayName = "Problem Description", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual string ProblemDescr { get; set; }
		#endregion
		#endregion

		#region FSServiceContract
		#region ServiceContractRefNbr
		public abstract class serviceContractRefNbr : PX.Data.IBqlField { }

		[PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC", BqlField = typeof(FSServiceContract.refNbr))]
		[PXUIField(DisplayName = "Service Contract", FieldClass = "FSCONTRACT", Visibility = PXUIVisibility.SelectorVisible, Visible = true, Enabled = true)]
		[FSSelectorContractRefNbrAttribute(typeof(ListField_RecordType_ContractSchedule.ServiceContract))]
		public virtual string ServiceContractRefNbr { get; set; }
		#endregion
		#endregion
	}
}
