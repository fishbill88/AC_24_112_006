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
using System.Linq;
using PX.Common;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;

namespace PX.Objects.TX
{
	public class ManageExemptCustomer : PXGraph<ManageExemptCustomer>
	{
		public PXCancel<ExemptCustomerFilter> Cancel;
		public PXAction<ExemptCustomerFilter> viewDocument;
		public PXFilter<ExemptCustomerFilter> Filter;

		[PXFilterable]
		public PXFilteredProcessing<ExemptCustomer, ExemptCustomerFilter> CustomerList;

		public ManageExemptCustomer()
		{
			CustomerList.SetSelected<ExemptCustomer.selected>();
		}

		#region Data View Delegate
		public virtual IEnumerable customerList()
		{
			IEnumerable result = new List<ExemptCustomer>();
			if (!string.IsNullOrEmpty(Filter.Current?.Action))
			{
				if (Filter.Current?.Action == AR.Messages.UpdateCustomerInECM || !string.IsNullOrEmpty(Filter.Current?.CompanyCode))
				{
					result = GetCustomerRecords(Filter.Current);
				}
			}
			return result; 
		}
		#endregion

		protected virtual void _(Events.RowSelected<ExemptCustomerFilter> e)
		{
			ExemptCustomerFilter row = e.Row;
			if (row != null)
			{
				string companyCode = GetPreferedCompanyCode(out int count);
				if (count == 1)
				{
					PXUIFieldAttribute.SetEnabled<ExemptCustomerFilter.companyCode>(e.Cache, e.Row, false);
				}

				PXUIFieldAttribute.SetVisible<ExemptCustomerFilter.companyCode>(e.Cache, e.Row, row.Action != AR.Messages.UpdateCustomerInECM);

				ExemptCustomerFilter filter = (ExemptCustomerFilter)this.Filter.Cache.CreateCopy(row);
				CustomerList.SetProcessDelegate(list => ProcessECMCustomers(filter, list));
			}
		}

		public string GetPreferedCompanyCode(out int recCount)
		{
			string companyCode = string.Empty;

			var result = SelectFrom<TaxPluginMapping>
				.InnerJoin<TaxPlugin>.On<TaxPluginMapping.taxPluginID.IsEqual<TaxPlugin.taxPluginID>>
				.InnerJoin<TXSetup>.On<TaxPlugin.taxPluginID.IsEqual<TXSetup.eCMProvider>>
				.AggregateTo<GroupBy<TaxPluginMapping.companyCode>>.View.Select(this);

			recCount = result.Count();
			companyCode = recCount > 0 ? ((TaxPluginMapping)result[0]).CompanyCode : companyCode;

			return companyCode;
		}

		protected virtual void _(Events.FieldDefaulting<ExemptCustomerFilter.companyCode> e)
		{
			string companyCode = GetPreferedCompanyCode(out int count);
			if (count == 1)
			{
				e.NewValue = companyCode;
			}
		}

		protected virtual IEnumerable GetCustomerRecords(ExemptCustomerFilter filter)
		{
			if (string.IsNullOrEmpty(filter.Action))
				yield break;

			PXSelectBase<ExemptCustomer> select = new PXSelect<ExemptCustomer>(this);
			if (filter.Action == AR.Messages.UpdateCustomerInECM)
			{
				select.WhereAnd<Where<Customer.eCMCompanyCode.IsNotNull.And<Customer.isECMValid.IsEqual<False>>>>(); ;
			}

			foreach (ExemptCustomer res in select.Select())
			{
				yield return res;
			}
		}

		public PXAction<ExemptCustomer> viewCustomer;
		[PXUIField(Visible = false, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton()]
		public virtual IEnumerable ViewCustomer(PXAdapter adapter)
		{
			ExemptCustomer doc = CustomerList.Current;
			if (doc != null && !String.IsNullOrEmpty(doc.AcctCD) && doc.BAccountID != null)
			{
				CustomerMaint argraph = PXGraph.CreateInstance<CustomerMaint>();
				argraph.BAccount.Current = argraph.BAccount.Search<Customer.bAccountID>(doc.BAccountID);
				if (argraph.BAccount.Current != null)
				{
					throw new PXRedirectRequiredException(argraph, true, "View Document") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
				}
			}
			return adapter.Get();
		}

		public static void ProcessECMCustomers(ExemptCustomerFilter filter, List<ExemptCustomer> list)
		{
			CustomerMaint graph = PXGraph.CreateInstance<AR.CustomerMaint>();
			CustomerMaintExternalECMExt custGraphExt = graph.GetExtension<CustomerMaintExternalECMExt>();

			for (int i = 0; i < list.Count; i++)
			{
				try
				{
					graph.Clear();
					Customer customer = graph.BAccount.Search<Customer.bAccountID>(list[i].BAccountID);

					if (customer != null)
					{
						if (filter?.Action == AR.Messages.CreateCustomerInECM)
						{
							custGraphExt.CreateECMCustomer(customer, filter.CompanyCode, out string warning);

							if (!string.IsNullOrEmpty(warning))
							{
								PXProcessing<ExemptCustomer>.SetWarning(i, PXMessages.LocalizeFormatNoPrefixNLA(AR.Messages.WRN_CustomerExistsInECM, warning));
							}
							else
							{
								PXProcessing<ExemptCustomer>.SetInfo(i, ActionsMessages.RecordProcessed);
							}
						}
						else if (filter?.Action == AR.Messages.UpdateCustomerInECM)
						{
							custGraphExt.UpdateECMCustomer(customer);
							PXProcessing<ExemptCustomer>.SetInfo(i, ActionsMessages.RecordProcessed);
						}
					}
				}
				catch (Exception ex)
				{
					PXProcessing<ExemptCustomer>.SetError(i, ex.Message);
				}
			}
		}
	}
}
