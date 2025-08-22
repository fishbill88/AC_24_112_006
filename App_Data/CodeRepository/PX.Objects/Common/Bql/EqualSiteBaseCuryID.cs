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
using PX.Data.SQLTree;
using System;
using System.Collections.Generic;

namespace PX.Objects.Common.Bql
{
	/// <exclude/>
	public class EqualSiteBaseCuryID<Operand> : In<Operand> where Operand : IBqlParameter
	{
		public override void Verify(PXCache cacheBase, object item, List<object> pars, ref bool? result, ref object value)
		{
			result = false;
			object baseCuryID = null;

			if (_operand1 is IBqlParameter parameter1)
			{
				Type fieldType = parameter1.GetReferencedType();
				if (fieldType.IsNested && cacheBase.Graph != null)
				{
					Type cacheType = BqlCommand.GetItemType(fieldType);
					PXCache cache = cacheBase.Graph.Caches[cacheType];
					if (cache.InternalCurrent != null)
					{
						var id = cache.GetValue(cache.Current, fieldType.Name) as int?;
						if (id != null)
							baseCuryID = IN.INSite.PK.Find(cacheBase.Graph, id)?.BaseCuryID;
					}
					else
					{
						result = true;
						return;
					}
				}
				if (pars.Count <= 0) return;
				pars.RemoveAt(0);
			}
			else
			{
				if (_operand1 == null) _operand1 = _operand1.createOperand<Operand>();
				_operand1.Verify(cacheBase, item, pars, ref result, ref baseCuryID);
			}

			result = string.Compare((string)baseCuryID, (string)value, StringComparison.InvariantCultureIgnoreCase) == 0;
		}

		public override bool AppendExpression(ref SQLExpression exp, PXGraph graph, BqlCommandInfo info, BqlCommand.Selection selection)
		{
			bool status = true;

			if (_operand1 == null) _operand1 = _operand1.createOperand<Operand>();
			SQLExpression right = null;
			status &= _operand1.AppendExpression(ref right, graph, info, selection);
			if (graph == null || !info.BuildExpression) return status;

			// Optimization for the case when Operand is Required<>
			object baseCuryID = null;
			if (_operand1 is IBqlParameter parameter1)
			{
				Type fieldType = parameter1.GetReferencedType();
				if (fieldType.IsNested)
				{
					Type cacheType = BqlCommand.GetItemType(fieldType);
					PXCache cache = graph.Caches[cacheType];
					if (cache.Current != null)
					{
						var id = cache.GetValue(cache.Current, fieldType.Name) as int?;
						if (id != null)
							baseCuryID = IN.INSite.PK.Find(graph, id)?.BaseCuryID;
					}
				}
				else
				{
					//parameter not defined, allow any branch.
					exp = new SQLConst(1).EQ(1);
					return status;
				}
			}
			exp = baseCuryID != null
				? exp.In(new SQLConst(baseCuryID))
				: new SQLConst(1).EQ(0);

			return status;
		}
	}
}
