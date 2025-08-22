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

namespace PX.Objects.AM.Attributes
{
	public class OperationIDSelectorAttribute : PXSelectorAttribute
	{
		private readonly Type[] _additionalPKFields;

		public OperationIDSelectorAttribute(Type type, Type[] additionalPKFields) : base(type)
		{
			_additionalPKFields = additionalPKFields;
		}

		public override void SubstituteKeyCommandPreparing(PXCache sender, PXCommandPreparingEventArgs e)
		{
			base.SubstituteKeyCommandPreparing(sender, e);
			var subQuery = (SubQuery)e.Expr;
			if (subQuery != null)
			{
				var query = subQuery.Query();
				for (int i = 0; _additionalPKFields.Length > i; i++)
				{
					var lftExpr = Column.SQLColumn(_additionalPKFields[i]);
					lftExpr.Table().Alias = _Type.Name + "Ext";
					query.AndWhere(SQLExpression.EQ(lftExpr, Column.SQLColumn(sender.GetBqlField(lftExpr.Name))));
				}
				e.Expr = new SubQuery(query);
			}
		}
	}
}
