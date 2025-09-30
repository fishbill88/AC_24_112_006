using PX.Data;
using PX.Objects.PM;
using PX.Objects.CM.Extensions; // for BQL decimal constants like decimal0

namespace ProjectsCustomization
{
    // Adds an unbound calculated column for PM Cost Budget lines
    public sealed class PMCostBudgetExt : PXCacheExtension<PMCostBudget>
    {
        public static bool IsActive() => true;

        // Local BQL constant for 0m to avoid dependency on external decimal0
        public class decimal0 : PX.Data.BQL.BqlDecimal.Constant<decimal0>
        {
            public decimal0() : base(0m) { }
        }

        #region UsrPercentCommitted
        public abstract class usrPercentCommitted : PX.Data.BQL.BqlDecimal.Field<usrPercentCommitted> { }
        /// <summary>
        /// Percentage of Revised Committed Amount to Revised Budgeted Amount.
        /// Calculated as CuryCommittedAmount / CuryRevisedAmount.
        /// </summary>
        [PXDecimal(2)]
        [PXUIField(DisplayName = "Percent Committed", Enabled = false)]
        [PXFormula(typeof(Switch<
            Case<Where<PMCostBudget.curyRevisedAmount, Greater<decimal0>>,
                Div<PMCostBudget.curyCommittedAmount, PMCostBudget.curyRevisedAmount>>,
            decimal0>))]
        [PXFormula(typeof(Default<PMCostBudget.curyRevisedAmount, PMCostBudget.curyCommittedAmount>))]
        public decimal? UsrPercentCommitted { get; set; }
        #endregion
    }
}
