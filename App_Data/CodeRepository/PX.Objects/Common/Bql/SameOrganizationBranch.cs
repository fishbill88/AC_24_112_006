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
using System.Linq;
using PX.Data;
using PX.Data.SQLTree;

namespace PX.Objects.Common.Bql
{
	public sealed class SameOrganizationBranch<Field, Parameter> : IBqlUnary
		where Field : IBqlOperand
		where Parameter : IBqlParameter, new()
	{
		IBqlParameter _parameter;

		public void Verify(PXCache cache, object item, List<object> pars, ref bool? result, ref object value)
		{
			object val = null;

			if (_parameter == null)
			{
				_parameter = new Parameter();
			}

			if (_parameter.HasDefault)
			{
				Type ft = _parameter.GetReferencedType();
				if (ft.IsNested)
				{
					Type ct = BqlCommand.GetItemType(ft);
					PXCache paramcache = cache.Graph.Caches[ct];
					if (paramcache.Current != null)
					{
						val = paramcache.GetValue(paramcache.Current, ft.Name);
					}
				}
			}

			if (typeof(IBqlField).IsAssignableFrom(typeof(Field)))
			{
				if ((cache.GetItemType() == BqlCommand.GetItemType(typeof(Field)) || BqlCommand.GetItemType(typeof(Field)).IsAssignableFrom(cache.GetItemType())))
				{
					value = cache.GetValue(item, typeof(Field).Name);
				}
				else
				{
					value = null;
				}
			}
			else
			{
				throw new PXArgumentException("Operand", ErrorMessages.OperandNotClassFieldAndNotIBqlCreator);
			}

			List<int> branches = null;

			if (val != null)
			{
				branches = GetSameOrganizationBranches((int?)val);
			}
			if (branches != null && branches.Count > 0 && value != null)
			{
				result = branches.Contains((int)value);
			}
		}

		public bool AppendExpression(ref SQLExpression exp, PXGraph graph, BqlCommandInfo info, BqlCommand.Selection selection)
		{
			bool status = true;
			object val = null;

			if (_parameter == null) _parameter = new Parameter();

			if (graph != null && info.BuildExpression)
			{
				if (_parameter.HasDefault)
				{
					Type ft = _parameter.GetReferencedType();
					if (ft.IsNested)
					{
						Type ct = BqlCommand.GetItemType(ft);
						PXCache cache = graph.Caches[ct];
						if (cache.Current != null)
						{
							val = cache.GetValue(cache.Current, ft.Name);
						}
					}
				}

				SQLExpression fld = BqlCommand.GetSingleExpression(typeof(Field), graph, info.Tables, selection, BqlCommand.FieldPlace.Condition);
				exp = fld.IsNull();

				List<int> branches = null;

				if (val != null) branches = GetSameOrganizationBranches((int?)val);
				if (branches != null && branches.Count > 0)
				{
					SQLExpression inn = null;
					foreach (int numBranch in branches)
					{
						if (null == inn) inn = new SQLConst(numBranch);
						else inn = inn.Seq(numBranch);
					}
					exp = exp.Or(fld.In(inn)).Embrace();
				}
			}

			if (info.Fields?.Contains(typeof(Field)) == false) info.Fields.Add(typeof(Field));			

			if (!selection.FromProjection) exp = AddParameterExpression(exp, info, selection, val);

			return status;
		}

		/// <summary>
		/// Method adds a parameter to SQLExpression. This is necessary for correct work with the platform cache,
		/// otherwise you can get the result from the cache, even if SOOrder.branchId (or your Parameter) has changed.
		/// </summary>
		/// <returns>currentExpression and the new expression that contains the parameter, the last expression is alwaya true.</returns>
		private SQLExpression AddParameterExpression(SQLExpression currentExpression, BqlCommandInfo info, BqlCommand.Selection selection, object parameterValue)
		{
			SQLExpression parameterExpression = Literal.NewParameter(selection.ParamCounter++);
			SQLExpression parameterTrueExpression = (parameterValue != null) ? parameterExpression.IsNotNull() : parameterExpression.IsNull();

			info.Parameters?.Add(_parameter);

			return parameterTrueExpression.And(currentExpression).Embrace();
		}


		private List<int> GetSameOrganizationBranches(int? branchID)
		{
			int? organizationID = PXAccess.GetParentOrganizationID(branchID);

			return PXAccess.GetChildBranchIDs(organizationID, false).ToList();
		}
	}
}
