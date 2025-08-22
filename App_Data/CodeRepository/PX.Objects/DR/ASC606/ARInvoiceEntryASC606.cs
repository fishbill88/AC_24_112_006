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
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.GL;
using System.Collections;

namespace PX.Objects.DR
{
	public class ARInvoiceEntryASC606 : PXGraphExtension<ARInvoiceEntry>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.aSC606>();
		}

		public PXAction<ARInvoice> viewSchedule;
		[PXUIField(DisplayName = "View Schedule", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable ViewSchedule(PXAdapter adapter)
		{
			ARTran currentLine = Base.Transactions.Current;

			if (currentLine != null &&
				Base.Transactions.Cache.GetStatus(currentLine) == PXEntryStatus.Notchanged)
			{
				ValidateTransactions();
				Base.Save.Press();
				ViewScheduleForDocument(Base, Base.Document.Current);
			}

			return adapter.Get();
		}

		protected void ValidateTransactions()
		{
			foreach(ARTran tran in Base.Transactions.Select())
			{
				object defCode = tran.DeferredCode;

				try
				{
					Base.Transactions.Cache.RaiseFieldVerifying<ARTran.deferredCode>(tran, ref defCode);
				}
				catch(PXSetPropertyException e)
				{
					Base.Transactions.Cache.RaiseExceptionHandling<ARTran.deferredCode>(tran, defCode, e);
					throw e;
				}
			}
		}

		public static void ViewScheduleForDocument(PXGraph graph, ARInvoice document)
		{
			PXSelectBase<DRSchedule> correspondingScheduleView = new PXSelect<
				DRSchedule,
				Where<
				DRSchedule.module, Equal<BatchModule.moduleAR>,
					And<DRSchedule.docType, Equal<Required<ARTran.tranType>>,
					And<DRSchedule.refNbr, Equal<Required<ARTran.refNbr>>>>>>
				(graph);

			DRSchedule correspondingSchedule = correspondingScheduleView.Select(document.DocType, document.RefNbr);

			if(correspondingSchedule?.LineNbr != null && document.Released == false)
			{
				throw new PXException(Messages.CantCompleteBecauseASC606FeatureIsEnabled);
			}

			if (correspondingSchedule?.IsOverridden != true && document.Released == false)
			{
				var netLinesAmount = ASC606Helper.CalculateNetAmount(graph, document);

				ARTran artran = new PXSelect<
					ARTran,
					Where<ARTran.tranType, Equal<Required<ARTran.tranType>>,
						And<ARTran.refNbr, Equal<Required<ARTran.refNbr>>,
						And<ARTran.defScheduleID, IsNotNull>>>>(graph).SelectWindowed(0, 1, document.DocType, document.RefNbr);
				int? defScheduleID = artran?.DefScheduleID;

				if (netLinesAmount.Cury != 0m)
				{
					DRSingleProcess process = PXGraph.CreateInstance<DRSingleProcess>();

					process.CreateSingleSchedule(document, netLinesAmount, defScheduleID, true);
					process.Actions.PressSave();

					correspondingScheduleView.Cache.Clear();
					correspondingScheduleView.Cache.ClearQueryCacheObsolete();
					correspondingScheduleView.View.Clear();

					correspondingSchedule = correspondingScheduleView.Select(document.DocType, document.RefNbr);
				}
			}

			if (correspondingSchedule != null)
			{
				PXRedirectHelper.TryRedirect(
					graph.Caches[typeof(DRSchedule)],
					correspondingSchedule,
					"View Schedule",
					PXRedirectHelper.WindowMode.NewWindow);
			}
			else throw new PXException(AR.Messages.DRScheduleNotExist);
		}

		public delegate void ReverseDRScheduleDelegate(ARRegister doc, ARTran tran);
		[PXOverride]
		public virtual void ReverseDRSchedule(ARRegister doc, ARTran tran, ReverseDRScheduleDelegate baseDelegate)
		{
			if (string.IsNullOrEmpty(tran.DeferredCode))
			{
				return;
			}

			DRSchedule schedule = PXSelect<DRSchedule,
				Where<DRSchedule.module, Equal<BatchModule.moduleAR>,
				And<DRSchedule.docType, Equal<Required<DRSchedule.docType>>,
				And<DRSchedule.refNbr, Equal<Required<DRSchedule.refNbr>>>>>>.
									Select(Base, doc.DocType, doc.RefNbr, tran.LineNbr);

			if (schedule != null)
			{
				tran.DefScheduleID = schedule.ScheduleID;
			}
		}

		protected virtual void _(Events.FieldVerifying<ARTran, ARTran.deferredCode> e)
		{
			if (e.Row == null)
				return;

			if (e.Row.InventoryID == null && e.NewValue != null)
			{
				throw new PXSetPropertyException(AR.Messages.InventoryIDCouldNotBeEmpty);
			}
		}
	}
}
