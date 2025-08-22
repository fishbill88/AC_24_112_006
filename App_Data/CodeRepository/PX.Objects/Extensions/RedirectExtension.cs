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
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.EP;
using System;
using System.Collections;

namespace PX.Objects.Extensions
{
	public class RedirectExtension<Graph> : PXGraphExtension<Graph>
		where Graph : PXGraph
	{
		public virtual IEnumerable ViewCustomerVendorEmployee<TBAccountID>(PXAdapter adapter)
			where TBAccountID : IBqlField
		{
			Type cacheType = BqlCommand.GetItemType(typeof(TBAccountID));
			PXCache cache = Base.Caches[cacheType];

			var row = cache.Current;

			if (row == null)
				return adapter.Get();

			BAccount businessAccount = PXSelect<BAccount, Where<BAccount.bAccountID, Equal<Current<TBAccountID>>>>.Select(Base);
			int? id = (int?)cache.GetValue(row, typeof(TBAccountID).Name);

			if (businessAccount != null)
			{
				if (businessAccount.Type == BAccountType.VendorType
					|| businessAccount.Type == BAccountType.CombinedType)
				{
					VendorMaint target = PXGraph.CreateInstance<VendorMaint>();

					target.BAccount.Current = target.BAccount.Search<BAccountR.bAccountID>(id);
					if (target.BAccount.Current != null)
					{
						throw new PXRedirectRequiredException(target, true, "redirect")
						{ Mode = PXBaseRedirectException.WindowMode.NewWindow };
					}
					else
					{
						VendorR vendor = PXSelect<VendorR, Where<VendorR.bAccountID, Equal<Current<TBAccountID>>>>.Select(Base);
						throw new PXException(PXMessages.LocalizeFormat(ErrorMessages.ElementDoesntExistOrNoRights, vendor.AcctCD));
					}
				}
				else if (businessAccount.Type == BAccountType.CustomerType)
				{
					CustomerMaint target = PXGraph.CreateInstance<CustomerMaint>();

					target.BAccount.Current = target.BAccount.Search<Customer.bAccountID>(id);
					if (target.BAccount.Current != null)
					{
						throw new PXRedirectRequiredException(target, true, "redirect")
						{ Mode = PXBaseRedirectException.WindowMode.NewWindow };
					}
					else
					{
						Customer customer = PXSelect<Customer, Where<Customer.bAccountID, Equal<Current<TBAccountID>>>>.Select(Base);
						throw new PXException(PXMessages.LocalizeFormat(ErrorMessages.ElementDoesntExistOrNoRights, customer.AcctCD));
					}
				}
				else if (businessAccount.Type == BAccountType.EmployeeType)
				{
					EmployeeMaint target = PXGraph.CreateInstance<EmployeeMaint>();

					target.Employee.Current = target.Employee.Search<EPEmployee.bAccountID>(id);
					if (target.Employee.Current != null)
					{
						throw new PXRedirectRequiredException(target, true, "redirect")
						{ Mode = PXBaseRedirectException.WindowMode.NewWindow };
					}
				}
			}
			return adapter.Get();
		}

		public virtual IEnumerable ViewVendorLocation<TLocationID, TBAccountID>(PXAdapter adapter)
			where TLocationID : IBqlField
			where TBAccountID : IBqlField
		{
			Type cacheType = BqlCommand.GetItemType(typeof(TBAccountID));
			PXCache cache = Base.Caches[cacheType];

			var row = cache.Current;

			if (row == null)
				return adapter.Get();

			BAccount businessAccount = PXSelect<BAccount, Where<BAccount.bAccountID, Equal<Current<TBAccountID>>>>.Select(Base);
			int? id = (int?)cache.GetValue(row, typeof(TLocationID).Name);

			VendorLocationMaint target = PXGraph.CreateInstance<VendorLocationMaint>();
			target.Location.Current = target.Location.Search<Location.locationID>(id, businessAccount?.AcctCD);
			if (target.Location.Current != null)
			{
				throw new PXRedirectRequiredException(target, true, "redirect")
				{ Mode = PXBaseRedirectException.WindowMode.NewWindow };
			}
			else
			{
				VendorR vendor = PXSelect<VendorR, Where<VendorR.bAccountID, Equal<Current<TBAccountID>>>>.Select(Base);
				throw new PXException(PXMessages.LocalizeFormat(ErrorMessages.ElementDoesntExistOrNoRights, vendor.AcctCD));
			}
		}
	}
}
