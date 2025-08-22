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

namespace PX.Objects.IN
{
	/// <summary>
	/// The attribute is derived from <see cref="PXDBQuantityAttribute"/> and allows to turn off automatic
	/// conversion from the Line Unit of Measure to the Base Unit of Measure on row persisting.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
	public class DBConditionalQuantityAttribute : PXDBQuantityAttribute
	{
		protected Type _conditionValueField;
		protected object _expectedValue;

		public DBConditionalQuantityAttribute(Type keyField, Type resultField, Type conditionValueField, object expectedValue)
			: base(keyField, resultField)
		{
			_conditionValueField = conditionValueField ?? throw new PXArgumentException(nameof(conditionValueField));
			_expectedValue = expectedValue;
		}

		public DBConditionalQuantityAttribute(Type keyField, Type resultField, InventoryUnitType decimalVerifyUnits,
			Type conditionValueField, object expectedValue)
			: base(keyField, resultField, decimalVerifyUnits)
		{
			_conditionValueField = conditionValueField ?? throw new PXArgumentException(nameof(conditionValueField));
			_expectedValue = expectedValue;
		}

		public override void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			object currentConditionValue = sender.GetValue(e.Row, _conditionValueField.Name);
			if (Equals(currentConditionValue, _expectedValue))
			{
				base.RowPersisting(sender, e);
			}
		}
	}
}
