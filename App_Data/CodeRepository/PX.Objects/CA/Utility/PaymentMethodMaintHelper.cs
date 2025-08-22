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

using PX.ACHPlugInBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Compilation;
using System.Windows.Forms;

namespace PX.Objects.CA
{
	public static class PaymentMethodMaintHelper
	{
		public static bool IsACHPlugIn(this PaymentMethodMaint graph) => graph.PaymentMethod.Current?.APBatchExportPlugInTypeName == ACHPlugInTypeAttribute.USACHPlugInType;
		public static IEnumerable<IACHPlugInParameter> GetParametersOfSelectedPlugIn(this PaymentMethodMaint graph)
		{
			var plugIn = graph.PaymentMethod.Current?.APBatchExportPlugInTypeName;

			if (string.IsNullOrEmpty(plugIn))
			{
				return Enumerable.Empty<ACHPlugInParameter>();
			}

			Type plugInType = PXBuildManager.GetType(plugIn, true);
			var plugInInstance = Activator.CreateInstance(plugInType) as PX.ACHPlugInBase.IACHPlugIn;

			return plugInInstance.GetACHPlugInParameters();
		}
		public static HashSet<string> GetRemittenceDetailsID(this PaymentMethodMaint graph)
		{
			var detailsID = new HashSet<string>();

			foreach (PaymentMethodDetail detail in graph.DetailsForCashAccount.Select())
			{
				var id = detail.DetailID.Trim();
				detailsID.Add(id);
			}

			return detailsID;
		}

		public static HashSet<string> GetVendorDetailsID(this PaymentMethodMaint graph, bool selectRemittanceOnly = false)
		{
			var detailsID = new HashSet<string>();

			foreach (PaymentMethodDetail detail in graph.DetailsForVendor.Select())
			{
				var id = detail.DetailID.Trim();
				detailsID.Add(id);
			}

			return detailsID;
		}

		public static void AppendDefaultPlugInParameters(this PaymentMethodMaint graph, Dictionary<int, string> mapping, bool useExportScenarioMapping = false)
		{
			var plugInParameter = graph.GetParametersOfSelectedPlugIn();
			var rsIDs = graph.GetRemittenceDetailsID();
			var viIDs = graph.GetVendorDetailsID();

			foreach (var parameter in plugInParameter)
			{
				var newParam = new ACHPlugInParameter
				{
					ParameterID = parameter.ParameterID.ToUpper(),
					ParameterCode = parameter.ParameterCode,
					Description = parameter.Description,
					Order = parameter.Order,
					Required = parameter.Required,
					Type = parameter.Type,
					UsedIn = parameter.UsedIn,
					Visible = parameter.Visible,
					IsGroupHeader = parameter.IsGroupHeader,
					IsAvailableInShortForm = parameter.IsAvailableInShortForm,
					DataElementSize = parameter.DataElementSize,
					IsFormula = parameter.IsFormula,
				};

				var detailIDs = parameter.Type == (int)SelectorType.RemittancePaymentMethodDetail ? rsIDs : new HashSet<string>();
				detailIDs = parameter.Type == (int)SelectorType.VendorPaymentMethodDetail ? viIDs : detailIDs;

				if (parameter.DetailMapping.HasValue &&
					mapping.TryGetValue(parameter.DetailMapping.Value, out var mappingID)
					&& detailIDs.Contains(mappingID))
				{
					if (useExportScenarioMapping)
					{
						if (parameter.ExportScenarioMapping.HasValue &&
						mapping.TryGetValue(parameter.ExportScenarioMapping.Value, out var scenarioMappingID)
						&& detailIDs.Contains(scenarioMappingID))
						{
							newParam.Value = scenarioMappingID;
						}
						else
						{
							newParam.Value = mappingID;
						}
					}
					else
					{
						newParam.Value = mappingID;
					}
				}
				else
				{
					newParam.Value = parameter.Value;
				}

				newParam = graph.aCHPlugInParameters.Insert(newParam);
			}

		}
	}
}
