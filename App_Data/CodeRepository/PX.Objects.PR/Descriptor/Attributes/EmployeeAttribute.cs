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
using PX.Data.BQL.Fluent;
using PX.Objects.AP;
using PX.Objects.EP;
using System;

namespace PX.Objects.PR
{
	[PXDBInt]
	[PXUIField(DisplayName = "Employee")]
	public abstract class EmployeeAttributeBase : PXEntityAttribute
	{
		private int? _EPActiveRestrictorAttributeIndex = null;
		private int? _PRActiveRestrictorAttributeIndex = null;

		public bool FilterActive
		{
			set
			{
				if (_EPActiveRestrictorAttributeIndex != null && _EPActiveRestrictorAttributeIndex < _Attributes.Count)
				{
					DisablableRestrictorAttribute attr = _Attributes[_EPActiveRestrictorAttributeIndex.Value] as DisablableRestrictorAttribute;
					if (attr != null)
					{
						attr.Enabled = value;
					}
				}

				if (_PRActiveRestrictorAttributeIndex != null && _PRActiveRestrictorAttributeIndex < _Attributes.Count)
				{
					DisablableRestrictorAttribute attr = _Attributes[_PRActiveRestrictorAttributeIndex.Value] as DisablableRestrictorAttribute;
					if (attr != null)
					{
						attr.Enabled = value;
					}
				}
			}
		}

		public EmployeeAttributeBase(bool filterActive, Type searchType, params Type[] fieldList)
		{
			PXDimensionSelectorAttribute attr;
			_Attributes.Add(attr = new PXDimensionSelectorAttribute(PX.Objects.EP.EmployeeRawAttribute.DimensionName, searchType, typeof(EPEmployee.acctCD), fieldList));
			attr.DescriptionField = typeof(EPEmployee.acctName);
			_SelAttrIndex = _Attributes.Count - 1;

			var countryRestrictor = new PXRestrictorAttribute(typeof(Where<MatchPRCountry<PREmployee.countryID>>), Messages.EmployeeCountryNotActive, typeof(PREmployee.countryID));
			countryRestrictor.ShowWarning = true;
			_Attributes.Add(countryRestrictor);

			if (filterActive)
			{
				DisablableRestrictorAttribute epRestrictor = new DisablableRestrictorAttribute(typeof(Where<EPEmployee.vStatus.IsEqual<VendorStatus.active>>), Messages.InactiveEPEmployee, typeof(EPEmployee.acctName));
				epRestrictor.ShowWarning = true;
				_Attributes.Add(epRestrictor);
				_EPActiveRestrictorAttributeIndex = _Attributes.Count - 1;

				DisablableRestrictorAttribute prRestrictor = new DisablableRestrictorAttribute(typeof(Where<PREmployee.activeInPayroll, Equal<True>>), Messages.InactivePREmployee, typeof(PREmployee.acctName));
				prRestrictor.ShowWarning = true;
				_Attributes.Add(prRestrictor);
				_PRActiveRestrictorAttributeIndex = _Attributes.Count - 1;
			}
			FilterActive = filterActive;

			this.Filterable = true;
		}

		private class DisablableRestrictorAttribute : PXRestrictorAttribute
		{
			public bool Enabled { get; set; } = false;

			public DisablableRestrictorAttribute(Type where, string message, params Type[] pars) :
				base(where, message, pars) { }

			public override void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
			{
				if (Enabled)
				{
					base.FieldVerifying(sender, e);
				}
			}
		}
	}

	public class EmployeeAttribute : EmployeeAttributeBase
	{
		public EmployeeAttribute(bool filterActive = false) : base(filterActive, typeof(SelectFrom<PREmployee>
			.InnerJoin<GL.Branch>.On<PREmployee.parentBAccountID.IsEqual<GL.Branch.bAccountID>>
			.LeftJoin<EPEmployeePosition>.On<EPEmployeePosition.employeeID.IsEqual<PREmployee.bAccountID>
				.And<EPEmployeePosition.isActive.IsEqual<True>>>
			.Where<MatchWithBranch<GL.Branch.branchID>
				.And<MatchWithPayGroup<PREmployee.payGroupID>>>
			.SearchFor<PREmployee.bAccountID>),
			typeof(PREmployee.bAccountID), typeof(PREmployee.acctCD), typeof(PREmployee.acctName),
			typeof(PREmployee.vStatus), typeof(PREmployee.employeeClassID), typeof(EPEmployeePosition.positionID), typeof(PREmployee.departmentID))
		{
		}
	}

	public class EmployeeActiveAttribute : EmployeeAttribute
	{
		public EmployeeActiveAttribute() : base(true) { }
	}

	public class EmployeeActiveInPayGroupAttribute : EmployeeAttributeBase
	{
		public EmployeeActiveInPayGroupAttribute() : base(true, typeof(SelectFrom<PREmployee>
			.InnerJoin<GL.Branch>.On<PREmployee.parentBAccountID.IsEqual<GL.Branch.bAccountID>>
			.LeftJoin<EPEmployeePosition>.On<EPEmployeePosition.employeeID.IsEqual<PREmployee.bAccountID>
				.And<EPEmployeePosition.isActive.IsEqual<True>>>
			.Where<MatchWithBranch<GL.Branch.branchID> 
				.And<MatchWithPayGroup<PREmployee.payGroupID>>
				.And<PRPayment.payGroupID.FromCurrent.IsNull
					.Or<PRPayment.payGroupID.FromCurrent.IsEqual<PREmployee.payGroupID>>
					.Or<PRPayment.docType.FromCurrent.IsEqual<PayrollType.voidCheck>>>>
			.SearchFor<PREmployee.bAccountID>),
			typeof(PREmployee.bAccountID), typeof(PREmployee.acctCD), typeof(PREmployee.acctName),
			typeof(PREmployee.employeeClassID), typeof(EPEmployeePosition.positionID), typeof(PREmployee.departmentID))
		{
		}
	}

	public class EmployeeActiveInPayrollBatchAttribute : EmployeeAttributeBase
	{
		public EmployeeActiveInPayrollBatchAttribute() : base(true, typeof(Search2<PREmployee.bAccountID,
			InnerJoin<GL.Branch,
				On<PREmployee.parentBAccountID, Equal<GL.Branch.bAccountID>>,
			LeftJoin<EPEmployeePosition,
				On<EPEmployeePosition.employeeID, Equal<PREmployee.bAccountID>,
				And<EPEmployeePosition.isActive, Equal<True>>>>>,
			Where2<MatchWithBranch<GL.Branch.branchID>,
				And<Where2<MatchWithPayGroup<PREmployee.payGroupID>,
					And<Current<PRBatch.payGroupID>, Equal<PREmployee.payGroupID>>>>>>),
			typeof(PREmployee.bAccountID), typeof(PREmployee.acctCD), typeof(PREmployee.acctName),
			typeof(PREmployee.employeeClassID), typeof(EPEmployeePosition.positionID), typeof(PREmployee.departmentID))
		{
		}
	}

	public class PREmployeeRawAttribute : PXEntityAttribute
	{
		public PREmployeeRawAttribute()
		{
			Type searchType = typeof(SelectFrom<PREmployee>
				.LeftJoin<EmployeeRawAttribute.EmployeeLogin>.On<EmployeeRawAttribute.EmployeeLogin.pKID.IsEqual<EPEmployee.userID>>
				.InnerJoin<GL.Branch>.On<GL.Branch.bAccountID.IsEqual<PREmployee.parentBAccountID>>
				.LeftJoin<EPEmployeePosition>.On<EPEmployeePosition.employeeID.IsEqual<PREmployee.bAccountID>
					.And<EPEmployeePosition.isActive.IsEqual<True>>>
				.Where<MatchWithBranch<GL.Branch.branchID>
					.And<MatchWithPayGroup<PREmployee.payGroupID>>>
				.SearchFor<PREmployee.acctCD>);

			PXDimensionSelectorAttribute attr;
			_Attributes.Add(attr = new PXDimensionSelectorAttribute(EmployeeRawAttribute.DimensionName, searchType, typeof(PREmployee.acctCD),
									typeof(PREmployee.bAccountID), typeof(PREmployee.acctCD), typeof(EPEmployee.acctName),
									typeof(EPEmployee.vStatus), typeof(EPEmployeePosition.positionID), typeof(EPEmployee.departmentID),
									typeof(EPEmployee.defLocationID), typeof(EmployeeRawAttribute.EmployeeLogin.username)));
			attr.DescriptionField = typeof(EPEmployee.acctName);
			_SelAttrIndex = _Attributes.Count - 1;

			var countryRestrictor = new PXRestrictorAttribute(typeof(Where<MatchPRCountry<PREmployee.countryID>>), Messages.EmployeeCountryNotActive, typeof(PREmployee.countryID));
			countryRestrictor.ShowWarning = true;
			_Attributes.Add(countryRestrictor);

			this.Filterable = true;
		}
	}
}
