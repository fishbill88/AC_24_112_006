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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PX.Commerce.Amazon.API.Rest
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class Filter
	{
		public const string ISO8601_DATETIME_FORMAT = "o";

		public virtual List<(string, string)> AddFilter()
		{
			var parameters = new List<(string, string)>();
			foreach (var propertyInfo in GetType().GetProperties())
			{
				DescriptionAttribute attr = propertyInfo.GetCustomAttribute<DescriptionAttribute>();
				if (attr == null) continue;
				String key = attr.Description;
				object value = propertyInfo.GetValue(this);
				if (value != null)
				{
					if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?))
					{
						parameters.Add((key,((DateTime)value).ToString(ISO8601_DATETIME_FORMAT)));
					}
					else if (value is IList || value is ICollection)
					{
						var flattenedString = new StringBuilder();
						foreach (var param in (IList)value)
						{
							if (flattenedString.Length > 0)
								flattenedString.Append(",");
							flattenedString.Append(param);
						}
						parameters.Add((key, flattenedString.ToString()));
					}
					else
						parameters.Add((key, value.ToString()));


				}
			}
			return parameters;
		}
	}
}
