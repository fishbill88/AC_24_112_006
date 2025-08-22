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
using PX.Objects.IN;
using System;

namespace PX.Objects.AM.Attributes
{
	public class AMSiteAvailAttribute : SiteAvailAttribute
	{
		public AMSiteAvailAttribute(Type InventoryType, Type SubItemType) : base(InventoryType, SubItemType, typeof(CostCenter.freeStock))
		{
			if (PXAccess.FeatureInstalled<CS.FeaturesSet.multipleBaseCurrencies>())
			{
				var restrictor = new PX.Objects.IN.Attributes.RestrictorWithParametersAttribute(typeof(Where<INSite.baseCuryID, Equal<Current<AccessInfo.baseCuryID>>>),
				Messages.BranchBaseCurrencyDifference,
				typeof(INSite.branchID), typeof(Current<AccessInfo.branchID>));
				_Attributes.Add(restrictor);
			}
		}
	}
}
