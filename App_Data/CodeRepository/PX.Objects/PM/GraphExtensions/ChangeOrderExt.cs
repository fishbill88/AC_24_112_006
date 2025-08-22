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
using System;
using System.Collections;
using System.Linq;

namespace PX.Objects.PM
{
	/// <summary>
	/// This class implements graph extension to use common logic for change orders
	/// </summary>
	/// <typeparam name="TGraph">The entry <see cref="PX.Data.PXGraph" /> type.</typeparam>
	/// <typeparam name="TPrimary">The primary DAC (a <see cref="PX.Data.IBqlTable" /> type) of the <typeparam name="TGraph" />graph.</typeparam>
	public abstract class ChangeOrderExt<TGraph, TPrimary> : PXGraphExtension<TGraph>
	where TGraph : PXGraph
	where TPrimary : class, IBqlTable, new()
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.changeOrder>();
		}

		public abstract PXSelectBase<PMChangeOrder> ChangeOrder { get; }

		public abstract PMChangeOrder CurrentChangeOrder { get; }

		[PXCopyPasteHiddenView]
		public PXSelect<ReversingChangeOrder, Where<ReversingChangeOrder.origRefNbr, Equal<Optional<PMChangeOrder.refNbr>>>> ReversingChangeOrders;

		public PXSetup<PMSetup> Setup;

		public PXAction<TPrimary> viewChangeOrder;
		[PXUIField(DisplayName = PM.Messages.ViewChangeOrder, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Enabled = true)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.Inquiry)]
		public virtual IEnumerable ViewChangeOrder(PXAdapter adapter)
		{
			if (CurrentChangeOrder != null && !string.IsNullOrEmpty(CurrentChangeOrder.RefNbr))
			{
				NavigateChangeOrder(CurrentChangeOrder.RefNbr, Messages.ViewChangeOrder, PXBaseRedirectException.WindowMode.NewWindow);
			}

			return adapter.Get();
		}

		public PXAction<PMProject> viewOrigChangeOrder;
		[PXUIField(DisplayName = Messages.ViewChangeOrder, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntry)]
		public virtual IEnumerable ViewOrigChangeOrder(PXAdapter adapter)
		{
			if (CurrentChangeOrder != null && !string.IsNullOrEmpty(CurrentChangeOrder.OrigRefNbr))
			{
				NavigateChangeOrder(CurrentChangeOrder.OrigRefNbr, Messages.ViewChangeOrder, PXBaseRedirectException.WindowMode.NewWindow);
			}

			return adapter.Get();
		}

		public PXAction<PMChangeOrder> viewCurrentReversingChangeOrder;
		[PXUIField(DisplayName = Messages.ViewReversingChangeOrder, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntry)]
		public virtual IEnumerable ViewCurrentReversingChangeOrder(PXAdapter adapter)
		{
			if (ReversingChangeOrders.Current != null && !string.IsNullOrEmpty(ReversingChangeOrders.Current.OrigRefNbr))
			{
				NavigateChangeOrder(ReversingChangeOrders.Current.RefNbr, Messages.ViewReversingChangeOrder, PXBaseRedirectException.WindowMode.NewWindow);
			}

			return adapter.Get();
		}

		public PXAction<PMChangeOrder> viewReversingChangeOrders;
		[PXUIField(DisplayName = Messages.ViewReversingChangeOrder, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntry)]
		public virtual IEnumerable ViewReversingChangeOrders(PXAdapter adapter)
		{
			if (CurrentChangeOrder == null)
				return adapter.Get();

			var reveresingOrderRefs = GetReversingOrderRefs(CurrentChangeOrder);

			if (reveresingOrderRefs.Length == 0)
				return adapter.Get();

			if (reveresingOrderRefs.Length == 1 && !string.IsNullOrEmpty(reveresingOrderRefs[0]))
			{
				NavigateChangeOrder(reveresingOrderRefs[0], Messages.ViewReversingChangeOrder, PXBaseRedirectException.WindowMode.NewWindow);
			}
			else
			{
				ChangeOrder.Current = ChangeOrder.Select();
				ReversingChangeOrders.AskExt();
			}

			return adapter.Get();
		}

		public virtual void NavigateChangeOrder(string refNbr, string message, PXBaseRedirectException.WindowMode windowMode)
		{
			if (string.IsNullOrWhiteSpace(refNbr))
				return;

			ChangeOrderEntry target = PXGraph.CreateInstance<ChangeOrderEntry>();
			target.Clear(PXClearOption.ClearAll);
			target.SelectTimeStamp();
			target.Document.Current = PMChangeOrder.PK.Find(this.Base, refNbr);
			throw new PXRedirectRequiredException(target, true, message) { Mode = windowMode };
		}

		protected virtual string[] GetReversingOrderRefs(PMChangeOrder changeOrder)
			=> ReversingChangeOrders
				.Select(changeOrder.RefNbr)
				.RowCast<ReversingChangeOrder>()
				.Select(x => x.RefNbr)
				.ToArray();

		protected virtual void _(Events.FieldSelecting<PMChangeOrder.reversingRefNbr> e)
		{
			if (!(e.Row is PMChangeOrder changeOrder))
				return;

			var reversingOrderRefs = GetReversingOrderRefs(changeOrder);

			e.ReturnValue = reversingOrderRefs.Length == 0 ? null : reversingOrderRefs.Length == 1 ? reversingOrderRefs[0] : "<LIST>";
		}

		public virtual bool ChangeOrderEnabled()
		{
			return ChangeOrderFeatureEnabled();
		}

		public virtual bool ChangeOrderVisible()
		{
			return ChangeOrderFeatureEnabled()
				&& !IsChangeOrderUserNumberingOn();
		}

		public virtual bool ChangeOrderFeatureEnabled()
		{
			return IsActive();
		}

		public bool IsChangeOrderUserNumberingOn()
		{
			if (string.IsNullOrWhiteSpace(Setup.Current?.ChangeOrderNumbering))
			{
				return false;
			}

			var numbering = Numbering.PK.Find(Base, Setup.Current?.ChangeOrderNumbering);

			return numbering?.UserNumbering ?? false;
		}
	}
}
