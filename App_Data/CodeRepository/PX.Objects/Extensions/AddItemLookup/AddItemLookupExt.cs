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
using System.Collections;
using Messages = PX.Objects.AR.Messages;

namespace PX.Objects.Extensions.AddItemLookup
{
	public abstract class AddItemLookupExt<TGraph, TDocument, TItemInfo, TItemFilter, TAddItemParameters> : AddItemLookupBaseExt<TGraph, TDocument, TItemInfo, TItemFilter>
		where TGraph : PXGraph
		where TDocument : class, IBqlTable, new()
		where TItemInfo : class, IPXSelectable, IBqlTable, new()
		where TItemFilter : INSiteStatusFilter, IBqlTable, new()
		where TAddItemParameters : class, IBqlTable, new()
	{
		[PXCopyPasteHiddenView]
		public PXFilter<TAddItemParameters> addItemParameters;

		protected override IEnumerable ShowItemsHandler(PXAdapter adapter)
		{
			if (ItemInfo.AskExt() == WebDialogResult.OK)
			{
				return AddSelectedItems(adapter);
			}

			return adapter.Get();
		}

		public PXAction<TDocument> addAllItems;

		[PXUIField(DisplayName = Messages.AddAll, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable AddAllItems(PXAdapter adapter)
		{
			return AddAllItemsHandler(adapter);
		}

		protected abstract IEnumerable AddAllItemsHandler(PXAdapter adapter);
	}
}
