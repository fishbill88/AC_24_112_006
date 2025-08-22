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

namespace PX.SM
{
	#region SMReportSubstituteAttribute

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class SMReportSubstituteAttribute : PXEventSubscriberAttribute
	{
		private readonly Type _valueField;
		private readonly Type _textField;

		public SMReportSubstituteAttribute(Type valueField, Type textField)
			: base()
		{
			CheckBqlField(valueField, "valueField");
			CheckBqlField(textField, "textField");

			_valueField = valueField;
			_textField = textField;
		}

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);

			var table = sender.GetItemType();
			var command = BqlCommand.CreateInstance(
				typeof(Select<,>), table,
				typeof(Where<,>),
				_valueField, typeof(Equal<>), typeof(Required<>), _valueField);
			var view = new PXView(sender.Graph, true, command);
			sender.Graph.FieldSelecting.AddHandler(table, _FieldName, 
				(cache, args) =>
					{
						var row = view.SelectSingle(args.ReturnValue);
						var text = cache.GetValue(row, _textField.Name);
						args.ReturnValue = text;
					});
		}

		private static void CheckBqlField(Type field, string argumentName)
		{
			if (field == null) throw new ArgumentNullException(argumentName);
			if (!typeof(IBqlField).IsAssignableFrom(field))
				throw new ArgumentException(PXLocalizer.LocalizeFormat(ErrorMessages.InvalidIBqlField, field.FullName), argumentName);
		}
	}

	#endregion

	#region userType

	public sealed class userType : PX.Data.BQL.BqlString.Constant<userType>
	{
		public userType()
			: base(typeof(Users).FullName)
		{
		}
	}

	#endregion
}
