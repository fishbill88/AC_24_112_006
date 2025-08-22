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

#nullable enable
using System;
using System.Collections.Generic;

using PX.CCProcessingBase.Interfaces.V2;
using PX.Data;
using PX.Objects.AR.CCPaymentProcessing.Common;
using PX.Objects.AR.CCPaymentProcessing.Helpers;
using PX.Objects.AR.CCPaymentProcessing.Repositories;
using PX.Objects.AR.CCPaymentProcessing.Wrappers.Interfaces;

using V1 = PX.CCProcessingBase;
using V2 = PX.CCProcessingBase.Interfaces.V2;

namespace PX.Objects.AR.CCPaymentProcessing.Wrappers
{
	public class TerminalProcessingWrapper
	{
		public static ITerminalProcessingWrapper GetTerminalProcessingWrapper(object pluginObject, ICardProcessingReadersProvider provider)
		{
			ITerminalProcessingWrapper wrapper = GetTerminalProcessingWrapper(pluginObject);
			ISetCardProcessingReadersProvider? setProviderBehaviour = wrapper as ISetCardProcessingReadersProvider;
			if (setProviderBehaviour == null)
			{
				throw new ApplicationException(NotLocalizableMessages.ERR_CardProcessingReadersProviderSetting);
			}
			setProviderBehaviour.SetProvider(provider);
			return wrapper;
		}
		private static ITerminalProcessingWrapper GetTerminalProcessingWrapper(object pluginObject)
		{
			bool isV1Interface = CCProcessingHelper.IsV1ProcessingInterface(pluginObject.GetType());
			if (isV1Interface)
			{
				throw new PXException(Messages.TryingToUseNotSupportedPlugin);
			}
			var v2ProcessingInterface = CCProcessingHelper.IsV2ProcessingInterface(pluginObject);
			if (v2ProcessingInterface != null)
			{
				return new V2TerminalProcessor(v2ProcessingInterface);
			}
			throw new PXException(V1.Messages.UnknownPluginType, pluginObject.GetType().Name);
		}

		protected class V2TerminalProcessor : V2ProcessorBase, ITerminalProcessingWrapper, ISetCardProcessingReadersProvider
		{
			private readonly V2.ICCProcessingPlugin _plugin;

			public V2TerminalProcessor(V2.ICCProcessingPlugin v2Plugin)
			{
				_plugin = v2Plugin;
			}

			public V2.POSTerminalData GetTerminal(string terminalID)
			{
				V2.ICCTerminalGetter terminalGetter = GetProcessor<V2.ICCTerminalGetter>(CCProcessingFeature.TerminalGetter);
				var result = terminalGetter.GetTerminal(terminalID);
				return result;
			}

			public IEnumerable<POSTerminalData> GetTerminals()
			{
				V2.ICCTerminalGetter terminalGetter = GetProcessor<V2.ICCTerminalGetter>(CCProcessingFeature.TerminalGetter);
				var result = terminalGetter.GetTerminals();
				return result;
			}

			private T GetProcessor<T>(CCProcessingFeature feature) where T : class
			{
				V2SettingsGenerator seetingsGen = new V2SettingsGenerator(_provider);
				T processor = _plugin.CreateProcessor<T>(seetingsGen.GetSettings());
				if (processor == null)
				{
					string errorMessage = PXMessages.LocalizeFormatNoPrefixNLA(
							Messages.FeatureNotSupportedByProcessing,
							feature);
					throw new PXException(errorMessage);
				}
				return processor;
			}
		}
	}
}
