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

using PX.Api;
using PX.Api.ContractBased;
using PX.Api.ContractBased.Models;
using PX.Data;
using PX.Objects.IN;

namespace PX.Objects.EndpointAdapters.WorkflowAdapters.IN
{
	[PXVersion("20.200.001", "Default")]
	[PXVersion("22.200.001", "Default")]
	[PXVersion("23.200.001", "Default")]
	internal class TransferOrderAdapter : InventoryRegisterAdapterBase
	{
		[FieldsProcessed(new[] {
			"ReferenceNbr",
			"Hold"
		})]
		protected virtual void TransferOrder_Insert(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
			=> INRegisterInsert((INRegisterEntryBase)graph, entity, targetEntity);

		[FieldsProcessed(new[] {
			"Hold"
		})]
		protected virtual void TransferOrder_Update(PXGraph graph, EntityImpl entity, EntityImpl targetEntity)
			=> INRegisterUpdate((INRegisterEntryBase)graph, entity, targetEntity);
	}
}
