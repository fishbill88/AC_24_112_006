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

using PX.CCProcessingBase.Interfaces.V2;
using PX.Data;
using PX.Objects.AR.CCPaymentProcessing.Common;
using PX.Objects.AR.CCPaymentProcessing.Helpers;
using PX.Objects.AR.CCPaymentProcessing.Repositories;
using PX.Objects.AR.CCPaymentProcessing.Wrappers;
using PX.Objects.AR.CCPaymentProcessing.Wrappers.Interfaces;
using PX.Objects.CA;
using PX.Objects.CS;

namespace PX.Objects.CC.GraphExtensions
{
	public class CCProcessingCenterMaintTerminal : PXGraphExtension<CCProcessingCenterMaint>
	{
		public static bool IsActive() => PXAccess.FeatureInstalled<FeaturesSet.integratedCardProcessing>();

		public PXSelect<CCProcessingCenterTerminal, Where<CCProcessingCenterTerminal.processingCenterID, Equal<Current<CCProcessingCenter.processingCenterID>>>> POSTerminals;
		public PXSelect<DefaultTerminal, Where<DefaultTerminal.processingCenterID, Equal<Current<CCProcessingCenter.processingCenterID>>>> DefaultTerminals;

		public PXAction<CCProcessingCenterTerminal> importTerminals;
		/// <summary>
		/// Import terminals from the processing center
		/// </summary>
		/// <param name="adapter"></param>
		/// <returns></returns>
		[PXUIField(DisplayName = "Import Terminals", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable ImportTerminals(PXAdapter adapter)
		{
			var procCenter = Base.ProcessingCenter.Current;
			Base.Save.Press();

			PXLongOperation.StartOperation(this, delegate
			{
				CCProcessingCenterMaint graph = PXGraph.CreateInstance<CCProcessingCenterMaint>();
				var ext = graph.GetExtension<CCProcessingCenterMaintTerminal>();
				graph.ProcessingCenter.Current = procCenter;

				var context = new CCProcessingContext();
				context.callerGraph = graph;
				context.processingCenter = procCenter;
				var plugin = CCPluginTypeHelper.CreatePluginInstance(procCenter);
				bool importTerminalSupported = CCProcessingFeatureHelper.IsFeatureSupported(procCenter, CCProcessingFeature.TerminalGetter);
				ITerminalProcessingWrapper wrapper = TerminalProcessingWrapper.GetTerminalProcessingWrapper(plugin, new CardProcessingReadersProvider(context));

				var terminalsFromProcCenter = new List<POSTerminalData>(wrapper.GetTerminals());

				foreach (CCProcessingCenterTerminal acumaticaTerminal in ext.POSTerminals.Select())
				{
					bool isDeleted = true;

					foreach (var procCenterTerminal in terminalsFromProcCenter)
					{
						if (acumaticaTerminal.TerminalID == procCenterTerminal.TerminalID)
						{
							isDeleted = false;
							acumaticaTerminal.CanBeEnabled = true;
							acumaticaTerminal.TerminalName = procCenterTerminal.TerminalName;
							ext.POSTerminals.Update(acumaticaTerminal);

							terminalsFromProcCenter.Remove(procCenterTerminal);
							break;
						}
					}

					if (isDeleted)
					{
						acumaticaTerminal.CanBeEnabled = false;
						acumaticaTerminal.IsActive = false;
						ext.POSTerminals.Update(acumaticaTerminal);
					}
				}

				if (terminalsFromProcCenter.Count > 0)
				{
					foreach (var procCenterTerminal in terminalsFromProcCenter)
					{
						string defaultDisplayName = procCenterTerminal.TerminalName;
						string displayName = defaultDisplayName;
						int increment = 0;
						while (ext.POSTerminals.Select().ToList().Any(r => string.Equals(((CCProcessingCenterTerminal)r).DisplayName, displayName, StringComparison.InvariantCultureIgnoreCase)))
						{
							increment++;
							displayName = defaultDisplayName + "-" + increment.ToString("D2");
						}

						var ccterminal = new CCProcessingCenterTerminal()
						{
							TerminalID = procCenterTerminal.TerminalID,
							TerminalName = procCenterTerminal.TerminalName,
							DisplayName = displayName,
							ProcessingCenterID = procCenter.ProcessingCenterID
						};

						ext.POSTerminals.Insert(ccterminal);
					}
				}

				graph.Save.Press();
			});

			return adapter.Get();
		}

		protected virtual void _(Events.RowSelected<CCProcessingCenter> e)
		{
			var row = e.Row;
			var cache = e.Cache;
			if (row == null) return;

			bool hasProcTypeName = row.ProcessingTypeName != null;
			PXUIFieldAttribute.SetVisible<CCProcessingCenter.acceptPOSPayments>(cache, row, hasProcTypeName && CCProcessingFeatureHelper.IsFeatureSupported(row, CCProcessingFeature.TerminalGetter));
		}

		protected virtual void _(Events.RowSelected<CCProcessingCenterTerminal> e)
		{
			var row = e.Row;
			var cache = e.Cache;
			if (row == null) return;

			PXUIFieldAttribute.SetEnabled<CCProcessingCenterTerminal.terminalID>(cache, e.Row, false);
			PXUIFieldAttribute.SetEnabled<CCProcessingCenterTerminal.terminalName>(cache, e.Row, false);
			PXUIFieldAttribute.SetEnabled<CCProcessingCenterTerminal.isActive>(cache, row, row.CanBeEnabled == true);
		}

		protected virtual void _(Events.FieldVerifying<CCProcessingCenterTerminal.displayName> e)
		{
			CCProcessingCenterTerminal row = e.Row as CCProcessingCenterTerminal;
			if (row == null) return;

			var newAcumaticaTerminalName = e.NewValue as string;

			if (newAcumaticaTerminalName != null && row.DisplayName != null)
			{
				foreach (CCProcessingCenterTerminal terminal in this.POSTerminals.Select())
				{
					if (terminal.DisplayName != null)
					{
						if (newAcumaticaTerminalName.ToUpper() == terminal.DisplayName.ToUpper())
						{
							throw new PXException(Messages.NotUniqueTermenalName);
						}
					}
				}
			}
		}

		protected virtual void _(Events.RowUpdated<CCProcessingCenterTerminal> e)
		{
			var newRow = e.Row;
			var oldRow = e.OldRow;

			if (oldRow?.IsActive == true && newRow?.IsActive == false)
			{
				var defaultTerminals = PXSelect<DefaultTerminal,
					Where<DefaultTerminal.processingCenterID, Equal<Required<DefaultTerminal.processingCenterID>>,
					And<DefaultTerminal.terminalID, Equal<Required<DefaultTerminal.terminalID>>>>>.Select(Base, newRow.ProcessingCenterID, newRow.TerminalID);
				foreach (var defaultTerminal in defaultTerminals)
				{
					DefaultTerminals.Delete(defaultTerminal);
				}
			}
		}
	}
}
