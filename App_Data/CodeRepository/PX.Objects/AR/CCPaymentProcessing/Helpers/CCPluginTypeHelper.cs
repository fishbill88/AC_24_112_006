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

using CommonServiceLocator;
using PX.Data;
using PX.Licensing;
using PX.Objects.AR.CCPaymentProcessing.Common;
using PX.Objects.AR.CCPaymentProcessing.Specific;
using PX.Objects.CA;
using System;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;

namespace PX.Objects.AR.CCPaymentProcessing.Helpers
{
	/// <summary>A class that contains auxiliary methods for working with plug-in types.</summary>
	public static class CCPluginTypeHelper
	{
		/// <summary> The list of plug-in types that have been removed from the Acumatica ERP code in Version 2019 R2.</summary>
		public static string[] NotSupportedPluginTypeNames
		{
			get {
				return new[] {
					AuthnetConstants.AIMPluginFullName,
					AuthnetConstants.CIMPluginFullName }; 
			}
		}

		public static Type GetPluginType(string typeName)
		{
			return GetPluginType(typeName, true);
		}

		public static Type GetPluginType(string typeName, bool throwOnError)
		{
			Type pluginType = PXBuildManager.GetType(typeName, throwOnError);
			return pluginType;
		}

		public static object CreatePluginInstance(CCProcessingCenter procCenter)
		{
			Type pluginType = GetPluginTypeWithCheck(procCenter);
			return Activator.CreateInstance(pluginType);
		}

		/// <summary>
		/// Gets plug-in type by name and performs its validation.
		/// </summary>
		/// <param name="typeName">Name of the plug-in type to get.</param>
		/// <param name="pluginType">Contains type if found; otherwise, null/.</param>
		/// <returns>Result of plug-in type search and validation.</returns>
		public static CCPluginCheckResult TryGetPluginTypeWithCheck(string typeName, out Type pluginType)
		{
			pluginType = null;

			if (string.IsNullOrEmpty(typeName))
				return CCPluginCheckResult.Empty;

			if (InUnsupportedList(typeName))
				return CCPluginCheckResult.Unsupported;

			pluginType = GetPluginType(typeName, false);
			if (pluginType == null)
				return CCPluginCheckResult.Missing;
			
			if (CCProcessingHelper.IsV1ProcessingInterface(pluginType))
				return CCPluginCheckResult.Unsupported;

			return CCPluginCheckResult.Ok;
		}

		/// <summary>
		/// Checks whether the payment plug-in type that is configured for the processing center is supported by the processing center 
		/// and returns the Type object that corresponds to this plug-in type.
		/// </summary>
		public static Type GetPluginTypeWithCheck(CCProcessingCenter procCenter)
		{
			if (procCenter == null)
				throw new ArgumentNullException(nameof(procCenter));

			string typeName = procCenter.ProcessingTypeName;

			CCPluginCheckResult checkResult = TryGetPluginTypeWithCheck(typeName, out Type pluginType);
			switch (checkResult)
			{
				case CCPluginCheckResult.Empty:
					throw new PXException(AR.Messages.ERR_PluginTypeIsNotSelectedForProcessingCenter, procCenter.ProcessingCenterID);
				case CCPluginCheckResult.Missing:
					throw new PXException(CA.Messages.ProcCenterUsesMissingPlugin, procCenter.ProcessingCenterID);
				case CCPluginCheckResult.Unsupported:
					throw new PXException(CA.Messages.NotSupportedProcCenter, procCenter.ProcessingCenterID);
			}

			return pluginType;
		}

		/// <summary>
		/// Performs validation of the plug-in referenced by Processing Center.
		/// </summary>
		/// <param name="graph">A graph to perform Db queries</param>
		/// <param name="procCenterId">Procesing Center Id</param>
		/// <returns>Result of plug-in validation.</returns>
		public static CCPluginCheckResult CheckProcessingCenterPlugin(PXGraph graph, string procCenterId)
		{
			if (graph == null)
			{
				throw new ArgumentNullException(nameof(graph));
			}
			if (procCenterId == null)
			{
				throw new ArgumentNullException(nameof(procCenterId));
			}

			CCProcessingCenter procCenter = PXSelect<CCProcessingCenter,
				Where<CCProcessingCenter.processingCenterID, Equal<Required<CCProcessingCenter.processingCenterID>>>>
				.Select(graph, procCenterId);

			string typeName = procCenter?.ProcessingTypeName;
			return TryGetPluginTypeWithCheck(typeName, out _);
		}

		public static bool InUnsupportedList(string typeStr)
		{
			if (typeStr == null)
			{
				return false;
			}
			bool ret = NotSupportedPluginTypeNames.Any(i => i == typeStr);
			return ret;
		}

		/// <summary>Checks whether the plug-in type is inherited from the parent class.</summary>
		/// <param name="pluginType">The Type object that should be checked.</param>
		/// <param name="checkTypeName">The full name of the parent class.</param>
		/// <param name="currLevel">The current level of recursion iteration.</param>
		/// <param name="maxLevel">The maximum level of recursion iteration.</param>
		public static bool CheckParentClass(Type pluginType, string checkTypeName, int currLevel, int maxLevel)
		{
			if (pluginType == null)
			{
				return false;
			}
			if (maxLevel == currLevel)
			{
				return false;
			}
			string fName = pluginType.FullName;
			if (fName == checkTypeName)
			{
				return true;
			}
			return CheckParentClass(pluginType.BaseType, checkTypeName, currLevel + 1, maxLevel);
		}

		/// <summary>Checks whether the plug-in type implements the interface.</summary>
		/// <param name="pluginType">The Type object that should be checked.</param>
		/// <param name="interfaceTypeName">The full name of the interface.</param>
		public static bool CheckImplementInterface(Type pluginType, string interfaceTypeName)
		{
			TypeFilter filter = new TypeFilter(delegate (Type t, object o) {
				return t.FullName == o.ToString();
			});
			Type[] interfaces = pluginType.FindInterfaces(filter, interfaceTypeName);
			if (interfaces.Length > 0)
			{
				return true;
			}
			return false;
		}

		public static bool IsProcCenterFeatureDisabled(string typeName)
		{
			return IsProcCenterFeatureDisabled(typeName, false);
		}

		public static void ThrowIfProcCenterFeatureDisabled(string typeName)
		{
			IsProcCenterFeatureDisabled(typeName, true);
		}

		private static bool IsProcCenterFeatureDisabled(string typeName, bool throwOnError)
		{
			switch (typeName)
			{
				case null:
					return false;
				case FortisConstants.FortisType:
				case AcumaticaPaymentsConstants.AcumaticaPaymentsType:
					if (!PXAccess.FeatureInstalled<CS.FeaturesSet.acumaticaPayments>() || !VerifyAssemblyCodeSign(typeName))
					{
						return throwOnError ? throw new PXSetPropertyException(AR.Messages.AcumaticaPaymentsFeatureIsDisabled) : true;
					}
					break;
				case AuthnetConstants.APIPluginFullName:
					if (!PXAccess.FeatureInstalled<CS.FeaturesSet.authorizeNetIntegration>() || !VerifyAssemblyCodeSign(typeName))
					{
						return throwOnError ? throw new PXSetPropertyException(AR.Messages.AuthorizeNetFeatureIsDisabled) : true;
					}
					break;
				case StripeConstants.StripeType:
					if (!PXAccess.FeatureInstalled<CS.FeaturesSet.stripeIntegration>() || !VerifyAssemblyCodeSign(typeName))
					{
						return throwOnError ? throw new PXSetPropertyException(AR.Messages.StripeFeatureIsDisabled) : true;
					}
					break;
				case ShopifyConstants.ShopifyType:
					if (!PXAccess.FeatureInstalled<CS.FeaturesSet.shopifyIntegration>())
					{
						return true;
					}
					break;
				default:
					if (!PXAccess.FeatureInstalled<CS.FeaturesSet.customCCIntegration>())
					{
						return throwOnError ? throw new PXSetPropertyException(AR.Messages.CustomCCIntegrationFeatureIsDisabled) : true;
					}
					break;
			}
			return false;
		}

		private static bool VerifyAssemblyCodeSign(string typeName)
		{
			ICodeSigningManager codeSigningManager = ServiceLocator.Current.GetService(typeof(ICodeSigningManager)) as ICodeSigningManager;
			if (codeSigningManager != null)
			{
				Type pluginType = GetPluginType(typeName, false);
				return codeSigningManager.VerifyAssemblyCodeSign(pluginType.Assembly);
			}
			return true;
		}
	}
}
