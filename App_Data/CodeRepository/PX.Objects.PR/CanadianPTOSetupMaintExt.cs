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
using PX.Objects.CS;
using System.Linq;

namespace PX.Objects.PR
{
	public class CanadianPTOSetupMaintExt : PXGraphExtension<PRSetupMaint>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.payrollCAN>();
		}

		#region CacheAttached
		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXCustomizeBaseAttribute(typeof(PXDefaultAttribute), nameof(PXDefaultAttribute.PersistingCheck), PXPersistingCheck.Null)]
		public virtual void _(Events.CacheAttached<PRSetup.ptoExpenseAcctDefault> e) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXCustomizeBaseAttribute(typeof(PXDefaultAttribute), nameof(PXDefaultAttribute.PersistingCheck), PXPersistingCheck.Null)]
		public virtual void _(Events.CacheAttached<PRSetup.ptoExpenseSubMask> e) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXCustomizeBaseAttribute(typeof(PXDefaultAttribute), nameof(PXDefaultAttribute.PersistingCheck), PXPersistingCheck.Null)]
		public virtual void _(Events.CacheAttached<PRSetup.ptoExpenseAlternateAcctDefault> e) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXCustomizeBaseAttribute(typeof(PXDefaultAttribute), nameof(PXDefaultAttribute.PersistingCheck), PXPersistingCheck.Null)]
		public virtual void _(Events.CacheAttached<PRSetup.ptoExpenseAlternateSubMask> e) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXCustomizeBaseAttribute(typeof(PXDefaultAttribute), nameof(PXDefaultAttribute.PersistingCheck), PXPersistingCheck.Null)]
		public virtual void _(Events.CacheAttached<PRSetup.ptoLiabilityAcctDefault> e) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXCustomizeBaseAttribute(typeof(PXDefaultAttribute), nameof(PXDefaultAttribute.PersistingCheck), PXPersistingCheck.Null)]
		public virtual void _(Events.CacheAttached<PRSetup.ptoLiabilitySubMask> e) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXCustomizeBaseAttribute(typeof(PXDefaultAttribute), nameof(PXDefaultAttribute.PersistingCheck), PXPersistingCheck.Null)]
		public virtual void _(Events.CacheAttached<PRSetup.ptoAssetAcctDefault> e) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXCustomizeBaseAttribute(typeof(PXDefaultAttribute), nameof(PXDefaultAttribute.PersistingCheck), PXPersistingCheck.Null)]
		public virtual void _(Events.CacheAttached<PRSetup.ptoAssetSubMask> e) { }
		#endregion CacheAttached

		#region Events
		public virtual void _(Events.RowSelected<PRSetup> e)
		{
			if (e.Row == null)
			{
				return;
			}

			bool updated = false;
			if (string.IsNullOrEmpty(e.Row.PTOExpenseAcctDefault))
			{
				// Acuminator disable once PX1047 RowChangesInEventHandlersForbiddenForArgs [Executed once when PayrollCAN is activated]
				e.Cache.SetDefaultExt<PRSetup.ptoExpenseAcctDefault>(e.Row);
				updated = true;
			}

			if (string.IsNullOrEmpty(e.Row.PTOLiabilityAcctDefault))
			{
				// Acuminator disable once PX1047 RowChangesInEventHandlersForbiddenForArgs [Executed once when PayrollCAN is activated]
				e.Cache.SetDefaultExt<PRSetup.ptoLiabilityAcctDefault>(e.Row);
				updated = true;
			}

			if (string.IsNullOrEmpty(e.Row.PTOAssetAcctDefault))
			{
				// Acuminator disable once PX1047 RowChangesInEventHandlersForbiddenForArgs [Executed once when PayrollCAN is activated]
				e.Cache.SetDefaultExt<PRSetup.ptoAssetAcctDefault>(e.Row);
				updated = true;
			}

			if (PXAccess.FeatureInstalled<FeaturesSet.subAccount>())
			{
				if (string.IsNullOrEmpty(e.Row.PTOExpenseSubMask))
				{
					// Acuminator disable once PX1047 RowChangesInEventHandlersForbiddenForArgs [Executed once when PayrollCAN is activated]
					e.Cache.SetDefaultExt<PRSetup.ptoExpenseSubMask>(e.Row);
					updated = true;
				}

				if (string.IsNullOrEmpty(e.Row.PTOLiabilitySubMask))
				{
					// Acuminator disable once PX1047 RowChangesInEventHandlersForbiddenForArgs [Executed once when PayrollCAN is activated]
					e.Cache.SetDefaultExt<PRSetup.ptoLiabilitySubMask>(e.Row);
					updated = true;
				}

				if (string.IsNullOrEmpty(e.Row.PTOAssetSubMask))
				{
					// Acuminator disable once PX1047 RowChangesInEventHandlersForbiddenForArgs [Executed once when PayrollCAN is activated]
					e.Cache.SetDefaultExt<PRSetup.ptoAssetSubMask>(e.Row);
					updated = true;
				}
			}

			if (updated)
			{
				e.Cache.SetStatus(e.Row, PXEntryStatus.Modified);
			}
		}
		#endregion Events

		#region Base graph overrides
		public delegate void SetAlternateAccountSubVisibleDelegate(PXCache cache, PRSetup row);
		[PXOverride]
		public virtual void SetAlternateAccountSubVisible(PXCache cache, PRSetup row, SetAlternateAccountSubVisibleDelegate baseMethod)
		{
			baseMethod(cache, row);
			PXUIFieldAttribute.SetVisible<PRSetup.ptoExpenseAlternateAcctDefault>(cache, row, Base.IsAlternateFieldVisible(row.PTOExpenseAcctDefault));
			PXUIFieldAttribute.SetVisible<PRSetup.ptoExpenseAlternateSubMask>(cache, row, row.PTOExpenseSubMask.Any(x => Base.IsAlternateFieldVisible(x.ToString())));
		}
		#endregion Base graph overrides
	}
}
