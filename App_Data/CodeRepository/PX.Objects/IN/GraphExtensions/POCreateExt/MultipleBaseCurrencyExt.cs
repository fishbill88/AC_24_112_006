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
using PX.Objects.CS;
using PX.Objects.IN.Attributes;
using PX.Objects.PO;
using PX.Objects.GL;
using POCreateFilter = PX.Objects.PO.POCreate.POCreateFilter;
using PX.Common;

namespace PX.Objects.IN.GraphExtensions.POCreateExt
{
	public class MultipleBaseCurrencyExt : PXGraphExtension<POCreate>
	{
		public static bool IsActive()
			=> PXAccess.FeatureInstalled<FeaturesSet.multipleBaseCurrencies>();

		public override void Initialize()
		{
			base.Initialize();

			Base.FixedDemand.Join<InnerJoin<INSite, On<POFixedDemand.siteID, Equal<INSite.siteID>>>>();
			Base.FixedDemand.WhereAnd<Where<INSite.baseCuryID, EqualBaseCuryID<Current2<POCreateFilter.branchID>>>>();
		}

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[RestrictSiteByBranch(typeof(POCreateFilter.branchID))]
		protected virtual void _(Events.CacheAttached<POCreateFilter.siteID> e)
		{
		}

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXRestrictor(typeof(Where<Current2<POCreateFilter.branchID>, IsNull,
			Or<Customer.baseCuryID, EqualBaseCuryID<Current2<POCreateFilter.branchID>>,
			Or<Customer.baseCuryID, IsNull>>>),
			Messages.CustomerOrVendorHasDifferentBaseCurrency, typeof(Customer.cOrgBAccountID), typeof(Customer.acctCD))]
		protected virtual void _(Events.CacheAttached<POCreateFilter.customerID> e)
		{
		}

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXRestrictor(typeof(Where<Current2<POCreateFilter.branchID>, IsNull,
			Or<Vendor.baseCuryID, EqualBaseCuryID<Current2<POCreateFilter.branchID>>,
			Or<Vendor.baseCuryID, IsNull>>>),
			Messages.CustomerOrVendorHasDifferentBaseCurrency, typeof(Vendor.vOrgBAccountID), typeof(Vendor.acctCD))]
		protected virtual void _(Events.CacheAttached<POCreateFilter.vendorID> e)
		{
		}

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[RestrictSiteByBranch(typeof(POCreateFilter.branchID))]
		protected virtual void _(Events.CacheAttached<POFixedDemand.pOSiteID> e)
		{
		}

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[RestrictSiteByBranch(typeof(POCreateFilter.branchID))]
		protected virtual void _(Events.CacheAttached<POFixedDemand.sourceSiteID> e)
		{
		}

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXRestrictor(typeof(Where<Current2<POCreateFilter.branchID>, IsNull,
			Or<Vendor.baseCuryID, EqualBaseCuryID<Current2<POCreateFilter.branchID>>,
			Or<Vendor.baseCuryID, IsNull>>>),
			Messages.CustomerOrVendorHasDifferentBaseCurrency, typeof(Vendor.vOrgBAccountID), typeof(Vendor.acctCD))]
		protected virtual void _(Events.CacheAttached<POFixedDemand.vendorID> e)
		{
		}

		protected virtual void _(Events.RowSelected<POFixedDemand> e)
		{
			PXSetPropertyException exception = null;

			var vendor = Vendor.PK.Find(Base, e.Row?.VendorID);
			if (vendor?.VOrgBAccountID.IsNotIn(0, null) == true)
			{
				BqlCommand command = new Select<POCreateFilter,
					Where<POCreateFilter.branchID, InsideBranchesOf<Required<Vendor.vOrgBAccountID>>>>();

				if (!command.Meet(Base.Filter.Cache, Base.Filter.Current, vendor.VOrgBAccountID))
				{
					var branch = Branch.PK.Find(Base, Base.Filter.Current?.BranchID);

					exception = new PXSetPropertyException(AP.Messages.BranchRestrictedByVendor,
						PXErrorLevel.RowWarning, vendor.AcctCD.TrimEnd(), branch?.BranchCD);
				}
			}

			if (PXUIFieldAttribute.GetErrorOnly<POFixedDemand.vendorID>(e.Cache, e.Row) == null)
				e.Cache.RaiseExceptionHandling<POFixedDemand.vendorID>(e.Row, vendor?.AcctCD, exception);
		}
	}
}
