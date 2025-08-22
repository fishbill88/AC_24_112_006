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
using System.Collections.Generic;
using System.Threading;
using PX.Data;
using PX.Data.SQLTree;
using PX.Objects.Common;
using PX.Objects.GL.FinPeriods.TableDefinition;

namespace PX.Objects.GL.BQL
{
	[Obsolete]
	public class CalendarRestriction<TOrganizationIDField, TBranchIDParameter, TOrganizationIDParameter, TUseMasterCalendarParameter>: IBqlUnary
		where TOrganizationIDField : IBqlOperand
		where TBranchIDParameter : IBqlParameter, new()
		where TOrganizationIDParameter : IBqlParameter, new()
		where TUseMasterCalendarParameter : IBqlParameter, new()
	{
		private IBqlParameter branchIDParameter;
		protected IBqlParameter BranchIDParameter => LazyInitializer.EnsureInitialized<IBqlParameter>(ref branchIDParameter, () => new TBranchIDParameter());

		private IBqlParameter organizationIDParameter;
		protected IBqlParameter OrganizationIDParameter => LazyInitializer.EnsureInitialized<IBqlParameter>(ref organizationIDParameter, () => new TOrganizationIDParameter());

		private IBqlParameter useMasterCalendarParameter;
		protected IBqlParameter UseMasterCalendarParameter => LazyInitializer.EnsureInitialized<IBqlParameter>(ref useMasterCalendarParameter, () => new TUseMasterCalendarParameter());

		public void Verify(PXCache cache, object item, List<object> pars, ref bool? result, ref object value)
		{
			value = BqlHelper.GetOperandValue<TOrganizationIDField>(cache, item);

			result = (int?)value == GetOrganizationIDRestrictinValue(cache.Graph);

			if (pars != null)
			{
				pars.Add(BqlHelper.GetParameterValue(cache.Graph, BranchIDParameter));
				pars.Add(BqlHelper.GetParameterValue(cache.Graph, OrganizationIDParameter));
				pars.Add(BqlHelper.GetParameterValue(cache.Graph, UseMasterCalendarParameter));
			}
		}

	    public bool AppendExpression(ref SQLExpression exp, PXGraph graph, BqlCommandInfo info, BqlCommand.Selection selection)
	    {
	        throw new NotImplementedException();
	    }

		protected int? GetOrganizationIDRestrictinValue(PXGraph graph)
		{
			bool? useMasterCalendarParamValue = (bool?)BqlHelper.GetParameterValue(graph, UseMasterCalendarParameter);

			if (useMasterCalendarParamValue == true)
			{
				return FinPeriod.organizationID.MasterValue;
			}

			return (int?)BqlHelper.GetParameterValue(graph, BranchIDParameter) 
					?? (int?)BqlHelper.GetParameterValue(graph, OrganizationIDParameter);
		}
	}
}
