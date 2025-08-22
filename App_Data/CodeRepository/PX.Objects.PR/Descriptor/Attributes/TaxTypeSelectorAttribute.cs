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
using System.Collections;
using PX.Payroll;
using PX.Payroll.Data;
using PX.Payroll.Proxy;

namespace PX.Objects.PR
{
	[PXCacheName(Messages.PRTaxType)]
	[Serializable]
	public class PRTaxType : PXBqlTable, IBqlTable
	{
		#region TypeName
		public abstract class typeName : IBqlField { }
		[PXString(50, IsKey = true, IsUnicode = true)]
		[PXUIField(DisplayName = "Tax Name", Visibility = PXUIVisibility.Invisible, Visible = false)]
		public string TypeName { get; set; }
		#endregion
		#region TaxCode
		public abstract class taxCode : IBqlField { }
		[PXString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "Tax Code", Visibility = PXUIVisibility.SelectorVisible)]
		public string TaxCode { get; set; }
		#endregion
		#region Description
		public abstract class description : IBqlField { }
		[PXString(250, IsUnicode = true)]
		[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible)]
		public string Description { get; set; }
		#endregion
		#region TaxCategory
		public abstract class taxCategory : IBqlField { }
		[PXString(3, IsFixed = true)]
		[PXUIField(DisplayName = "Tax Category")]
		[PXDefault]
		[TaxCategory.List]
		public string TaxCategory { get; set; }
		#endregion
		#region TaxJurisdiction
		public abstract class taxJurisdiction : IBqlField { }
		[PXString(3, IsFixed = true)]
		[PXUIField(DisplayName = "Jurisdiction Level", Visibility = PXUIVisibility.SelectorVisible)]
		[TaxJurisdiction.List]
		public string TaxJurisdiction { get; set; }
		#endregion
		#region IsUserDefined
		public abstract class isUserDefined : IBqlField { }
		[PXBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Is User-Defined")]
		public bool? IsUserDefined { get; set; }
		#endregion
		#region State
		public abstract class state : PX.Data.BQL.BqlString.Field<state> { }
		[PXString(2, IsFixed = true)]
		[PXUIField(DisplayName = "Tax State", Visibility = PXUIVisibility.SelectorVisible)]
		public string State { get; set; }
		#endregion
	}

	public class TaxTypeSelectorAttribute : PXCustomSelectorAttribute, IDynamicSettingsUpdateObserver
	{
		[InjectDependency]
		protected IPayrollSettingsCache SettingsCache { get; set; }

		[Obsolete(Common.Messages.ItemIsObsoleteAndWillBeRemoved2024R2)]
		public TaxTypeSelectorAttribute(Type taxCategoryField) : this() { }

		public TaxTypeSelectorAttribute()
			: base(typeof(PRTaxType.typeName))
		{
			this.SubstituteKey = typeof(PRTaxType.taxCode);
			this.DescriptionField = typeof(PRTaxType.description);
		}

		[Obsolete(Common.Messages.ItemIsObsoleteAndWillBeRemoved2024R2)]
        public override void CacheAttached(PXCache sender)
        {
            base.CacheAttached(sender);
        }

        protected virtual IEnumerable GetRecords()
		{
			return SettingsCache.GetTaxTypeRecords();
		}

		[Obsolete(Common.Messages.ItemIsObsoleteAndWillBeRemoved2024R2)]
		public static MetaDynamicEntityDictionary<PRTax.Meta> GetAllMeta()
		{
			using (var payrollAssemblyScope = new PXPayrollAssemblyScope<PayrollMetaProxy>())
			{
				return payrollAssemblyScope.Proxy.GetAllMeta<PRTax, TaxTypeAttribute>();
			}
		}

		[Obsolete(Common.Messages.ItemIsObsoleteAndWillBeRemoved2024R2)]
		public void DynamicSettingsUpdated()
		{
		}
	}
}
