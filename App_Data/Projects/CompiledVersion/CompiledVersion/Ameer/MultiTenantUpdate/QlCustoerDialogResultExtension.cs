using PX.Data;
using PX.Objects.AR;

namespace QLTenantCopyItems
{
    public sealed class QLCustomerDialogResultsExt : PXCacheExtension<Customer>
    {
        public static bool IsActive() => true;

        [PXUnboundDefault(WebDialogResult.None)]
        public WebDialogResult? ConsolidateStatementsFieldUpdated
        {
            get;
            set;
        }

        [PXUnboundDefault(WebDialogResult.None)]
        public WebDialogResult? ConsolidateToParentFieldUpdated
        {
            get;
            set;
        }

        [PXUnboundDefault(WebDialogResult.None)]
        public WebDialogResult? CustomerClassIDFieldVerifying
        {
            get;
            set;
        }

        [PXUnboundDefault(WebDialogResult.None)]
        public WebDialogResult? GenerateOnDemandStatementDialogResult
        {
            get;
            set;
        }

        [PXUnboundDefault(WebDialogResult.None)]
        public WebDialogResult? MaintVisibilityRestrictionDialogResult
        {
            get;
            set;
        }

        [PXUnboundDefault(WebDialogResult.None)]
        public WebDialogResult? SharedCreditPolicyFieldUpdated
        {
            get;
            set;
        }

        public abstract class consolidateStatementsFieldUpdated : IBqlField, IBqlOperand
        {
            protected consolidateStatementsFieldUpdated()
            {
            }
        }

        public abstract class consolidateToParentFieldUpdated : IBqlField, IBqlOperand
        {
            protected consolidateToParentFieldUpdated()
            {
            }
        }

        public abstract class customerClassIDFieldVerifying : IBqlField, IBqlOperand
        {
            protected customerClassIDFieldVerifying()
            {
            }
        }

        public abstract class generateOnDemandStatementDialogResult : IBqlField, IBqlOperand
        {
            protected generateOnDemandStatementDialogResult()
            {
            }
        }

        public abstract class maintVisibilityRestrictionDialogResult : IBqlField, IBqlOperand
        {
            protected maintVisibilityRestrictionDialogResult()
            {
            }
        }

        public abstract class sharedCreditPolicyFieldUpdated : IBqlField, IBqlOperand
        {
            protected sharedCreditPolicyFieldUpdated()
            {
            }
        }
    }
}