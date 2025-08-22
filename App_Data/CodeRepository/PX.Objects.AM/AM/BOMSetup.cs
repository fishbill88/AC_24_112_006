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
using PX.Objects.AM.Attributes;

namespace PX.Objects.AM
{
    public class BOMSetup : PXGraph<BOMSetup>
    {
        public PXSave<AMBSetup> Save;
        public PXCancel<AMBSetup> Cancel;

        public PXSelect<AMBSetup> AMBSetupRecord;

        public PXSelect<AMECRSetupApproval> ECRSetupApproval;
        public PXSelect<AMECOSetupApproval> ECOSetupApproval;

        protected virtual void AMBSetup_ECRRequestApproval_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            // Same Logic used on SOSetupMaint (19R1) - but using FieldUpdated
            PXCache cache = this.Caches[typeof(AMECRSetupApproval)];
            PXResultset<AMECRSetupApproval> setups = PXSelect<AMECRSetupApproval>.Select(sender.Graph, null);
            foreach (AMECRSetupApproval setup in setups)
            {
                setup.IsActive = ((AMBSetup)e.Row)?.ECRRequestApproval ?? false;
                cache.Update(setup);
            }
        }

        protected virtual void AMBSetup_ECORequestApproval_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            // Same Logic used on SOSetupMaint (19R1) - but using FieldUpdated
            PXCache cache = this.Caches[typeof(AMECOSetupApproval)];
            PXResultset<AMECOSetupApproval> setups = PXSelect<AMECOSetupApproval>.Select(sender.Graph, null);
            foreach (AMECOSetupApproval setup in setups)
            {
                setup.IsActive = ((AMBSetup)e.Row)?.ECORequestApproval ?? false;
                cache.Update(setup);
            }
        }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXRemoveBaseAttribute(typeof(PXDBIntAttribute))]
		[OperationDBTime]
		protected virtual void _(Events.CacheAttached<AMBSetup.defaultQueueTime> e) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXRemoveBaseAttribute(typeof(PXDBIntAttribute))]
		[OperationDBTime]
		protected virtual void _(Events.CacheAttached<AMBSetup.defaultFinishTime> e) { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXRemoveBaseAttribute(typeof(PXDBIntAttribute))]
		[OperationDBTime]
		protected virtual void _(Events.CacheAttached<AMBSetup.defaultMoveTime> e) { }
	}
}
