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

using System.Linq;
using PX.Api;
using PX.Api.ContractBased;
using PX.Api.ContractBased.Models;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.SO;

namespace PX.Objects.EndpointAdapters.WorkflowAdapters.SO
{
	[PXVersion("20.200.001", "Default")]
	[PXVersion("22.200.001", "Default")]
	[PXVersion("23.200.001", "Default")]
	internal class SalesInvoiceAdapter
	{
		[FieldsProcessed(new[] {
			"Type",
			"ReferenceNbr",
			"Hold",
			"CreditHold"
		})]
		protected virtual void SalesInvoice_Insert(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
		{
			var salesInvoiceGraph = (SOInvoiceEntry)graph;

			var typeField = targetEntity.Fields.SingleOrDefault(f => f.Name == "Type") as EntityValueField;
			var referenceNbrField = targetEntity.Fields.SingleOrDefault(f => f.Name == "ReferenceNbr") as EntityValueField;
			var holdField = targetEntity.Fields.SingleOrDefault(f => f.Name == "Hold") as EntityValueField;
			var creditHoldField = targetEntity.Fields.SingleOrDefault(f => f.Name == "CreditHold") as EntityValueField;

			var arInvoice = (ARInvoice)salesInvoiceGraph.Document.Cache.CreateInstance();

			if (typeField != null)
			{
				var arDocTypes = new ARInvoiceType.ListAttribute();
				if (arDocTypes.ValueLabelDic.ContainsValue(typeField.Value))
					arInvoice.DocType = arDocTypes.ValueLabelDic.First(x => x.Value == typeField.Value).Key;
				else
					arInvoice.DocType = typeField.Value;
			}

			if (referenceNbrField != null)
			{
				object refNbr = referenceNbrField.Value;

				PXFieldState state = salesInvoiceGraph.Document.Cache.GetStateExt<ARInvoice.refNbr>(null) as PXFieldState;
				if (state is PXStringState sstate && sstate.InputMask?.Length > 0)
				{
					if (sstate.InputMask.StartsWith(">"))
					{
						refNbr = referenceNbrField.Value.ToUpper();
					}
					else if (sstate.InputMask.StartsWith("<"))
					{
						refNbr = referenceNbrField.Value.ToLower();
					}
				}

				salesInvoiceGraph.Document.Cache.RaiseFieldUpdating<ARInvoice.refNbr>(arInvoice, ref refNbr);
				arInvoice.RefNbr = (string)refNbr;
			}

			salesInvoiceGraph.Document.Current = salesInvoiceGraph.Document.Insert(arInvoice);
			salesInvoiceGraph.SubscribeToPersistDependingOnBoolField(holdField, salesInvoiceGraph.putOnHold, salesInvoiceGraph.releaseFromHold);
			salesInvoiceGraph.SubscribeToPersistDependingOnBoolField(creditHoldField, salesInvoiceGraph.putOnCreditHold, salesInvoiceGraph.releaseFromCreditHold);
		}

		[FieldsProcessed(new[] {
			"Hold",
			"CreditHold"
		})]
		protected virtual void SalesInvoice_Update(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
		{
			var salesInvoiceGraph = (SOInvoiceEntry)graph;

			salesInvoiceGraph.Document.View.Answer = WebDialogResult.Yes;

			var holdField = targetEntity.Fields.SingleOrDefault(f => f.Name == "Hold") as EntityValueField;
			var creditHoldField = targetEntity.Fields.SingleOrDefault(f => f.Name == "CreditHold") as EntityValueField;

			if (holdField?.Value != null)
			{
				// fix for situation when cache.Current and record in cache collection - different objects by reference 
				salesInvoiceGraph.Document.Cache.Update(salesInvoiceGraph.Document.Current);
				salesInvoiceGraph.SubscribeToPersistDependingOnBoolField(holdField, salesInvoiceGraph.putOnHold, salesInvoiceGraph.releaseFromHold);
			}

			if (creditHoldField?.Value != null)
			{
				// fix for situation when cache.Current and record in cache collection - different objects by reference 
				salesInvoiceGraph.Document.Cache.Update(salesInvoiceGraph.Document.Current);
				salesInvoiceGraph.SubscribeToPersistDependingOnBoolField(creditHoldField, salesInvoiceGraph.putOnCreditHold, salesInvoiceGraph.releaseFromCreditHold);
			}
		}
	}
}
