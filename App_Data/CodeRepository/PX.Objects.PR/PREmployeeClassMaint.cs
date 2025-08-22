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

using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using PX.Objects.PM;
using System.Collections.Generic;
using System.Linq;

namespace PX.Objects.PR
{
	public class PREmployeeClassMaint : PXGraph<PREmployeeClassMaint, PREmployeeClass>
	{
		#region Views
		public SelectFrom<PREmployeeClass>
			.Where<MatchPRCountry<PREmployeeClass.countryID>>.View EmployeeClass;
		public PXSelect<PREmployeeClass, Where<PREmployeeClass.employeeClassID, Equal<Current<PREmployeeClass.employeeClassID>>>> CurEmployeeClassRecord;

		public SelectFrom<PREmployeeClassWorkLocation>
			.InnerJoin<PRLocation>.On<PRLocation.locationID.IsEqual<PREmployeeClassWorkLocation.locationID>>
			.InnerJoin<Address>.On<Address.addressID.IsEqual<PRLocation.addressID>>
			.Where<PREmployeeClassWorkLocation.employeeClassID.IsEqual<PREmployeeClass.employeeClassID.FromCurrent>
				.And<Address.countryID.IsEqual<PREmployeeClass.countryID.FromCurrent>>>.View WorkLocations;
		#endregion

		#region Event Handlers
		public virtual void _(Events.FieldVerifying<PREmployeeClassWorkLocation.isDefault> e)
		{
			if (!e.ExternalCall)
			{
				return;
			}

			bool? newValueBool = e.NewValue as bool?;
			bool requestRefresh = false;
			if (newValueBool == true)
			{
				WorkLocations.Select().FirstTableItems.Where(x => x.IsDefault == true).ForEach(x =>
				{
					x.IsDefault = false;
					WorkLocations.Update(x);
					requestRefresh = true;
				});
			}
			else if (newValueBool == false && !WorkLocations.Select().FirstTableItems.Any(x => x.IsDefault == true && !x.LocationID.Equals(e.Cache.GetValue<PREmployeeClassWorkLocation.locationID>(e.Row))))
			{
				e.NewValue = true;
			}

			if (requestRefresh)
			{
				WorkLocations.View.RequestRefresh();
			}
		}

		public virtual void _(Events.RowInserting<PREmployeeClassWorkLocation> e)
		{
			if (!WorkLocations.Select().FirstTableItems.Any(x => x.IsDefault == true))
			{
				e.Row.IsDefault = true;
			}
		}

		public virtual void _(Events.RowDeleted<PREmployeeClassWorkLocation> e)
		{
			IEnumerable<PREmployeeClassWorkLocation> remainingWorkLocations = WorkLocations.Select().FirstTableItems;
			if (!remainingWorkLocations.Any(x => x.IsDefault == true))
			{
				PREmployeeClassWorkLocation newDefault = remainingWorkLocations.FirstOrDefault();
				if (newDefault != null)
				{
					newDefault.IsDefault = true;
					WorkLocations.Update(newDefault);
					WorkLocations.View.RequestRefresh();
				}
			}
		}
		public void _(Events.FieldUpdated<PREmployeeClass, PREmployeeClass.empType> e)
		{
			if (e.Row == null)
			{
				return;
			}

			var newValue = (string)e.NewValue;
			if (newValue == EmployeeType.SalariedExempt)
			{
				e.Row.ExemptFromOvertimeRules = true;
			}
			else if (newValue == EmployeeType.SalariedNonExempt)
			{
				e.Row.ExemptFromOvertimeRules = false;
			}
		}

		public void _(Events.RowSelected<PREmployeeClass> e)
		{
			if (e.Row == null)
			{
				return;
			}

			bool isSalaried = EmployeeType.IsSalaried(e.Row.EmpType);
			PXUIFieldAttribute.SetEnabled<PREmployeeClass.exemptFromOvertimeRules>(e.Cache, e.Row, !isSalaried);
		}

		public virtual void _(Events.FieldSelecting<PREmployeeClass.workCodeID> e)
		{
			PREmployeeClass row = e.Row as PREmployeeClass;
			if (row == null || row.WorkCodeID == null)
			{
				return;
			}

			PMWorkCode workCode = PXSelectorAttribute.Select<PREmployeeClass.workCodeID>(e.Cache, row) as PMWorkCode;
			if (workCode == null)
			{
				e.ReturnValue = null;
				row.WorkCodeID = null;
				e.Cache.Update(row);
			}
		}
		#endregion
	}
}
