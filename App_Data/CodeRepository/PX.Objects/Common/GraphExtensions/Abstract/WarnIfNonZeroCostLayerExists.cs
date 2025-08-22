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

using PX.Data.BQL.Fluent;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.GL;

namespace PX.Objects.Common.GraphExtensions.Abstract
{
	public abstract class WarnIfNonZeroCostLayerExists<TField, TGraph> : PreventEditOf<TField>.On<TGraph>
		where TField : class, IBqlField
		where TGraph : PXGraph
	{
		protected override void OnPreventEdit(GetEditPreventingReasonArgs e)
		{
			if (e.Row == null) e.Cancel = true;
		}

		protected override string GetEditPreventingReasonImpl(GetEditPreventingReasonArgs e)
		{
			if (e.Cancel) return null;

			var originalAccountID = (int?)e.Cache.GetValueOriginal<TField>(e.Row);
			if(originalAccountID == (int?) e.NewValue) return null;

			var inventoryID = (int?)e.Cache.GetValue(e.Row, nameof(InventoryItem.inventoryID));
			INCostStatus nonZeroCostStatus =
				SelectFrom<INCostStatus>.
					Where<INCostStatus.inventoryID.IsEqual<INCostStatus.inventoryID.AsOptional>.
						And<INCostStatus.qtyOnHand.IsNotEqual<decimal0>>.
						And<INCostStatus.accountID.IsEqual<INCostStatus.accountID.AsOptional>>>
					.View
					.SelectSingleBound(e.Graph, null, inventoryID, originalAccountID);
			if (nonZeroCostStatus == null) return null;

			ShowError(e, originalAccountID, inventoryID);
			return null;
		}

		protected void ShowError(GetEditPreventingReasonArgs e, int? originalAccountID, int? inventoryID)
		{
			var account = Account.PK.Find(e.Graph, originalAccountID);
			var item = InventoryItem.PK.Find(e.Graph, inventoryID);
			e.Cache.RaiseExceptionHandling<TField>(e.Row, e.NewValue,
								new PXSetPropertyException(IN.Messages.NonZeroBalanceOnTheInventoryAccount, PXErrorLevel.Warning, account.AccountCD, item.InventoryCD));
		}
	}
}
