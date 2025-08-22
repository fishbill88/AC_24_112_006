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

using PX.CCProcessingUtility;
using PX.Data;
using PX.Data.BQL;
using System;
using System.Collections.Generic;

namespace PX.Objects.CC
{
	/// <summary>
	/// Helper for Level 3 codes for units of measure.
	/// </summary>
	public static class UoML3Helper
	{
		/// <summary>
		/// Unit of Measure Level 3 Data code.
		/// </summary>
		[PXCacheName("Level 3 Code Unit of Measure")]
		public class UoML3Code : PXBqlTable, IBqlTable
		{
			#region L3Code

			public abstract class l3Code : BqlString.Field<l3Code> { }

			/// <summary>
			/// The unit of measure.
			/// </summary>
			[PXUIField(DisplayName = "Level 3 Unit ID")]
			[PXString(3, IsKey = true)]
			[PXSelectorByMethod(typeof(UoML3Helper),
				nameof(GetCodes),
				typeof(Search<l3Code>),
				new Type[] { typeof(l3Code), typeof(description) },
				DescriptionField = typeof(description))]
			public virtual string L3Code
			{
				get;
				set;
			}

			#endregion

			#region Description

			public abstract class description : BqlString.Field<description> { }
			/// <summary>
			/// The description of Level 3 code.
			/// </summary>
			[PXUIField(DisplayName = "Description")]
			[PXString(256)]
			public virtual string Description
			{
				get;
				set;
			}

			#endregion
		}

		/// <summary>
		/// Return Level 3 Codes for selector.
		/// </summary>
		public static IEnumerable<UoML3Code> GetCodes()
		{
			List<UoML3Code> _codes = new List<UoML3Code>();
			foreach (var item in UnitOfMeasureL3Codes.L3Codes)
			{
				_codes.Add(new UoML3Code
				{
					L3Code = item.Item1,
					Description = item.Item2,
				});
			}
			return _codes;
		}
	}
}
