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
using PX.Data;
using PX.Objects.IN.Attributes;

namespace PX.Objects.IN.GraphExtensions.INAdjustmentEntryExt
{
	public class SpecialOrderCostCenterSupport : INRegisterEntryBaseExt.SpecialOrderCostCenterSupport<INAdjustmentEntry>
	{
		public static bool IsActive()
			=> PXAccess.FeatureInstalled<CS.FeaturesSet.specialOrders>();

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXRemoveBaseAttribute(typeof(SpecialOrderCostCenterSelectorAttribute))]
		[AdjustmentSpecialOrderCostCenterSelector]
		protected virtual void _(Events.CacheAttached<INTran.specialOrderCostCenterID> e) { }

		protected virtual void _(Events.FieldVerifying<INTran, INTran.qty> e)
			=> VerifyQty(e.Row?.CostLayerType, e.NewValue as decimal?);

		protected virtual void _(Events.FieldVerifying<INTran, INTran.costLayerType> e)
			=> VerifyQty(e.NewValue as string, e.Row?.Qty);

		protected virtual void VerifyQty(string costLayerType, decimal? qty)
		{
			if (costLayerType == CostLayerType.Special && qty > 0m)
				throw new PXSetPropertyException(Messages.SpecialCostLayerPositiveAdjustment);
		}

		/// <summary>
		/// Overrides <see cref="INAdjustmentEntry.GetUOMEnabled(bool, INTran)" />
		/// </summary>
		[PXOverride]
		public virtual bool GetUOMEnabled(bool isPIAdjustment, INTran tran,
			Func<bool, INTran, bool> baseMethod)
		{
			return baseMethod(isPIAdjustment, tran) && tran?.CostLayerType != CostLayerType.Special;
		}
	}
}
