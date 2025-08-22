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
using PX.Data.WorkflowAPI;
using PX.Objects.AR;
using PX.Objects.Common;

namespace PX.Objects.SO
{
	public class SOInvoiceEntry_ApprovalWorkflow : InvoiceEntry_ApprovalWorkflow<
		SOInvoiceEntry,
		SOInvoiceEntry_Workflow,
		SOInvoiceEntry_ApprovalWorkflow.SOConditions>
	{
		public class SOConditions : Conditions
		{
			public override BoundedTo<SOInvoiceEntry, ARInvoice>.Condition IsApprovalDisabled => GetOrCreate(b => b.FromBql<
				Not<SOSetupInvoiceApproval.EPSettings.IsDocumentApprovable<ARInvoice.docType, ARInvoice.status>>
			>());

			public override BoundedTo<SOInvoiceEntry, ARInvoice>.Condition NonEditable => GetOrCreate(b => b.FromBql<
				SOSetupInvoiceApproval.EPSettings.IsDocumentLockedByApproval<ARInvoice.docType, ARInvoice.status>
			>());
		}

		[PXWorkflowDependsOnType(typeof(SOSetupInvoiceApproval))]
		public sealed override void Configure(PXScreenConfiguration config) =>
			ConfigureBase(config, ctx => CommonActionCategories.Get(ctx).Approval);
	}
}
