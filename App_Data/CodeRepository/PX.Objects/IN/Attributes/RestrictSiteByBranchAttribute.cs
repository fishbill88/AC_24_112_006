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
using PX.Data.BQL;
using System;

namespace PX.Objects.IN.Attributes
{
	public class RestrictSiteByBranchAttribute : RestrictorWithParametersAttribute
	{
		public RestrictSiteByBranchAttribute(Type branchField = null, Type where = null)
			: base(GetWhere(branchField, where),
				  Messages.SiteBaseCurrencyDiffers,
				  typeof(INSite.branchID), typeof(INSite.siteCD), typeof(Current2<>).MakeGenericType(branchField))
		{
		}

		private static Type GetWhere(Type branchField, Type where)
		{
			if (where != null)
				return where;

			return BqlTemplate.OfCondition<Where<Current2<BqlPlaceholder.A>, IsNull,
				Or<INSite.baseCuryID, EqualBaseCuryID<Current2<BqlPlaceholder.A>>>>>
					.Replace<BqlPlaceholder.A>(branchField).ToType();
		}
	}
}
