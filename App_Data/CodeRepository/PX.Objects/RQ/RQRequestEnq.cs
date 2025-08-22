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
using PX.Objects.AP;
using PX.Objects.EP;
using PX.Objects.CR;
using PX.Objects.IN;
using PX.Objects.GL;
using PX.Objects.AR;

namespace PX.Objects.RQ
{
    [Serializable]
	public class RQRequestEnq : PXGraph<RQRequestEnq>
	{
		public PXCancel<RQRequestSelection> Cancel;
		public PXFilter<RQRequestSelection> Filter;
		public PXFilter<BAccount> BAccount;
		public PXFilter<Vendor>   Vendor;
	    public PXSelect<RQRequest> RQRequest;
		public PXSelectJoin<RQRequestLine,
			InnerJoin<RQRequest, On<RQRequest.orderNbr, Equal<RQRequestLine.orderNbr>>,
			LeftJoinSingleTable<Customer, On<Customer.bAccountID, Equal<RQRequest.employeeID>>,			
			LeftJoin<INSubItem, On<RQRequestLine.FK.SubItem>>>>,
			Where<Customer.bAccountID, IsNull,
								Or<Match<Customer, Current<AccessInfo.userName>>>>> Records;
		public PXSetup<RQSetup> setup;

        [Serializable]
		public partial class RQRequestSelection : PXBqlTable, IBqlTable
		{
			#region ReqClassID
			public abstract class reqClassID : PX.Data.BQL.BqlString.Field<reqClassID> { }
			protected string _ReqClassID;
			[PXDBString(10, IsUnicode = true)]			
			[PXUIField(DisplayName = "Request Class", Visibility = PXUIVisibility.SelectorVisible)]
			[PXSelector(typeof(RQRequestClass.reqClassID), DescriptionField = typeof(RQRequestClass.descr))]
			public virtual string ReqClassID
			{
				get
				{
					return this._ReqClassID;
				}
				set
				{
					this._ReqClassID = value;
				}
			}
			#endregion
			#region EmployeeID
			public abstract class employeeID : PX.Data.BQL.BqlInt.Field<employeeID> { }
			protected Int32? _EmployeeID;
			[PXDBInt()]
			[RQRequesterSelector(typeof(RQRequestSelection.reqClassID))]
			[PXUIField(DisplayName = "Requester", Visibility = PXUIVisibility.SelectorVisible)]
			public virtual Int32? EmployeeID
			{
				get
				{
					return this._EmployeeID;
				}
				set
				{
					this._EmployeeID = value;
				}
			}
			#endregion
			#region DepartmentID
			public abstract class departmentID : PX.Data.BQL.BqlString.Field<departmentID> { }
			protected String _DepartmentID;
			[PXDBString(10, IsUnicode = true)]
			[PXSelector(typeof(EPDepartment.departmentID), DescriptionField = typeof(EPDepartment.description))]
			[PXUIField(DisplayName = "Department", Visibility = PXUIVisibility.SelectorVisible)]
			public virtual String DepartmentID
			{
				get
				{
					return this._DepartmentID;
				}
				set
				{
					this._DepartmentID = value;
				}
			}
			#endregion			
			#region Description
			public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
			protected String _Description;
			[PXDBString(60, IsUnicode = true)]
			[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
			public virtual String Description
			{
				get
				{
					return this._Description;
				}
				set
				{
					this._Description = value;
				}
			}
			#endregion
			#region DescriptionWildcard
			public abstract class descriptionWildcard : PX.Data.BQL.BqlString.Field<descriptionWildcard> { }
			protected String _DescriptionWildcard;
			[PXDBString(60, IsUnicode = true)]			
			public virtual String DescriptionWildcard
			{
				get
				{
					return "%" + this._Description + "%";
				}				
			}
			#endregion
			#region InventoryID
			public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
			protected Int32? _InventoryID;
			[RQRequestInventoryItem(typeof(RQRequestSelection.reqClassID))]
			public virtual Int32? InventoryID
			{
				get
				{
					return this._InventoryID;
				}
				set
				{
					this._InventoryID = value;
				}
			}
			#endregion
			#region SubItemCD
			public abstract class subItemCD : PX.Data.BQL.BqlString.Field<subItemCD> { }
			protected String _SubItemCD;
			[SubItemRawExt(typeof(InventorySummaryEnqFilter.inventoryID), DisplayName = "Subitem")]
			//[SubItemRaw(DisplayName = "Subitem")]
			public virtual String SubItemCD
			{
				get
				{
					return this._SubItemCD;
				}
				set
				{
					this._SubItemCD = value;
				}
			}
			#endregion
			#region SubItemCD Wildcard
			public abstract class subItemCDWildcard : PX.Data.BQL.BqlString.Field<subItemCDWildcard> { };
			[PXDBString(30, IsUnicode = true)]
			public virtual String SubItemCDWildcard
			{
				get
				{
					//return SubItemCDUtils.CreateSubItemCDWildcard(this._SubItemCD);
					return SubCDUtils.CreateSubCDWildcard(this._SubItemCD, SubItemAttribute.DimensionName);
				}
			}
			#endregion
		}

		public RQRequestEnq()
		{
			Records.View.WhereAndCurrent<RQRequestSelection>();
			PXStringListAttribute.SetList<InventoryItem.itemType>(
				this.Caches[typeof(InventoryItem)], null,
				new string[] { INItemTypes.FinishedGood, INItemTypes.Component, INItemTypes.SubAssembly, INItemTypes.NonStockItem, INItemTypes.LaborItem, INItemTypes.ServiceItem, INItemTypes.ChargeItem, INItemTypes.ExpenseItem },
				new string[] { IN.Messages.FinishedGood, IN.Messages.Component, IN.Messages.SubAssembly, IN.Messages.NonStockItem, IN.Messages.LaborItem, IN.Messages.ServiceItem, IN.Messages.ChargeItem, IN.Messages.ExpenseItem }
				);			
		}	
	
		protected virtual void RQRequestSelection_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			RQRequestSelection row = (RQRequestSelection)e.Row;
			if (row != null)
			{
				EPEmployee emp =
				PXSelect<EPEmployee,
				Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>.SelectWindowed(this, 0, 1);
				row.EmployeeID = emp?.BAccountID;
				row.ReqClassID = setup.Current.DefaultReqClassID;
			}
		}

		[PXDBInt]
		[PXDimensionSelector("BIZACCT",
			typeof(Search2<BAccount.bAccountID,
				LeftJoin<Contact, On<Contact.bAccountID, Equal<BAccount.bAccountID>,
						 And<Contact.contactID, Equal<BAccount.defContactID>>>,
				LeftJoin<Address, On<Address.bAccountID, Equal<BAccount.bAccountID>,
						 And<Address.addressID, Equal<BAccount.defAddressID>>>>>>),
			typeof(BAccount.acctCD),
			typeof(BAccount.acctCD),
			typeof(BAccount.acctName),
			typeof(BAccount.status),
			typeof(Contact.phone1),
			typeof(Address.city),
			typeof(Address.countryID),
			DescriptionField = typeof(BAccount.acctName))]
		[PXUIField(DisplayName = "Requested By", Visibility = PXUIVisibility.SelectorVisible)]
		protected void RQRequest_EmployeeID_CacheAttached(PXCache sender) { }
	}
}