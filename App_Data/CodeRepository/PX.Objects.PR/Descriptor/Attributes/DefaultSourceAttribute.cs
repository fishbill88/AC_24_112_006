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
using System;
using System.Linq;

namespace PX.Objects.PR
{
	[AttributeUsage(AttributeTargets.Property)]
	public class DefaultSourceAttribute : PXEventSubscriberAttribute, IPXFieldSelectingSubscriber
	{
		private Type _DefaultFlagExpression;
		private Type _DefaultSourceField;
		private Type[] _SourceKeys;
		private Type[] _DestinationKeys;
		private Type _ShowCondition;

		private Type[] Search
		{
			get
			{
				return new Type[]
				{
					typeof(Search<>),
					_DefaultSourceField,
				};
			}
		}

		public DefaultSourceAttribute(Type defaultFlagExpression, Type defaultSourceField, Type[] sourceKeys, Type[] destinationKeys, Type showCondition = null)
		{
			_DefaultFlagExpression = defaultFlagExpression;
			_DefaultSourceField = defaultSourceField;
			_SourceKeys = sourceKeys;
			_DestinationKeys = destinationKeys;
			_ShowCondition = showCondition;
		}

		public void FieldSelecting(PXCache cache, PXFieldSelectingEventArgs e)
		{
			object row = e.Row;

			if (row != null && _ShowCondition != null && !ConditionEvaluator.GetResult(cache, row, _ShowCondition))
			{
				e.ReturnValue = null;
				return;
			}

			if (ConditionEvaluator.GetResult(cache, row, _DefaultFlagExpression) == true)
			{
				Type searchBql = BqlCommand.Compose(Search);
				BqlCommand cmd = BqlCommand.CreateInstance(searchBql);
				System.Collections.Generic.IEnumerable<object> keyValues = _DestinationKeys.Select(x => cache.GetValue(row, x.Name));

				if (keyValues.Any(x => x == null))
				{
					return;
				}

				foreach (Type key in _SourceKeys)
				{
					cmd = cmd.WhereAnd(BqlCommand.Compose(typeof(Where<,>), key, typeof(Equal<>), typeof(Required<>), key));
				}

				PXView view = new PXView(cache.Graph, false, cmd);
				object result = view.SelectSingle(keyValues.ToArray());
				object value = cache.Graph.Caches[_DefaultSourceField.DeclaringType.Name].GetValue(result, _DefaultSourceField.Name);
				cache.SetValue(row, FieldName, value);
				e.ReturnValue = value;
			}
		}
	}
}
