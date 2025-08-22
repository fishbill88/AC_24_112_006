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

using PX.Payroll;
using PX.Payroll.Data;
using PX.Payroll.Proxy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace PX.Objects.PR
{
	public class PayrollSettingsCache : IPayrollSettingsCache, IDynamicSettingsUpdateObserver
	{
		#region Cache for TaxSplitWageTypeListAttribute
		private List<PRTypeMeta> _USWageTypes;
		private object _USWageTypesLock = new object();
		#endregion

		#region Cache for TaxTypeSelectorAttribute
		private List<PRTaxType> _TaxTypeRecords;
		private object _TaxTypeRecordsLock = new object();
		#endregion

		#region Cache for PRTypeSelectorBaseAttribute
		// Key (PR Type Class, Use Reporting Type)
		private ConcurrentDictionary<(Type, bool), List<PRTypeMeta>> _PRTypeCache = new ConcurrentDictionary<(Type, bool), List<PRTypeMeta>>();
		#endregion

		public PayrollSettingsCache()
		{
			PXPayrollAssemblyScope.SubscribeDynamicSettingsUpdate(this);
		}

		public void DynamicSettingsUpdated()
		{
			lock (_USWageTypesLock)
			{
				_USWageTypes = null;
			}

			lock (_TaxTypeRecordsLock)
			{
				_TaxTypeRecords = null;
			}

			_PRTypeCache.Clear();
		}

		#region Cache for TaxSplitWageTypeListAttribute
		public int[] GetUSWageTypeValues()
		{
			return GetUSWageTypes().Select(x => x.ID).ToArray();
		}

		public string[] GetUSWageTypeLabels()
		{
			return GetUSWageTypes().Select(x => x.Name.ToUpper()).ToArray();
		}

		private List<PRTypeMeta> GetUSWageTypes()
		{
			lock (_USWageTypesLock)
			{
				if (_USWageTypes == null)
				{
					_USWageTypes = PRTypeSelectorAttribute.GetAll<PRWage>(LocationConstants.USCountryCode);
				}

				return _USWageTypes;
			}
		}
		#endregion Cache for TaxSplitWageTypeListAttribute

		#region Cache for TaxTypeSelectorAttribute
		public IEnumerable<PRTaxType> GetTaxTypeRecords()
		{
			lock (_TaxTypeRecordsLock)
			{
				if (_TaxTypeRecords == null)
				{
					List<PRTaxType> taxTypeRecords = new List<PRTaxType>();

					foreach (var taxType in GetAllMeta().GetEnumerator())
					{
						taxTypeRecords.Add(
							new PRTaxType
							{
								TypeName = taxType.TypeName,
								TaxCode = PRTax.GetUserFriendlyCode(taxType.Code),
								Description = taxType.TypeMeta.Description,
								TaxCategory = TaxCategory.GetTaxCategory(taxType.TypeMeta.TaxCategory),
								TaxJurisdiction = TaxJurisdiction.GetTaxJurisdiction(taxType.TypeMeta.TaxJurisdiction),
								State = (taxType.TypeMeta as IStateSpecific)?.State
							});
					}

					_TaxTypeRecords = taxTypeRecords;
				}

				return _TaxTypeRecords;
			}
		}

		private MetaDynamicEntityDictionary<PRTax.Meta> GetAllMeta()
		{
			using (var payrollAssemblyScope = new PXPayrollAssemblyScope<PayrollMetaProxy>())
			{
				return payrollAssemblyScope.Proxy.GetAllMeta<PRTax, TaxTypeAttribute>();
			}
		}
		#endregion Cache for TaxTypeSelectorAttribute

		#region Cache for PRTypeSelectorBaseAttribute
	    public List<PRTypeMeta> GetPRTypeList(Type prTypeClass, bool useReportingType)
		{
			if (!_PRTypeCache.TryGetValue((prTypeClass, useReportingType), out List<PRTypeMeta> prTypeList))
			{
				prTypeList = GetPRTypeMeta(prTypeClass, useReportingType);
				_PRTypeCache[(prTypeClass, useReportingType)] = prTypeList;
			}

			return prTypeList;
		}

		private List<PRTypeMeta> GetPRTypeMeta(Type prTypeClass, bool useReportingType)
		{
			using (var payrollAssemblyScope = new PXPayrollAssemblyScope<PayrollMetaProxy>())
			{
				return payrollAssemblyScope.Proxy.GetPRTypeMeta(prTypeClass, useReportingType);
			}
		}
		#endregion
	}
}
