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
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PX.Objects.PR
{
	public class CostAssignmentColumnVisibilityEvaluator
	{
		public abstract class ByProject : BqlFormulaEvaluator, IBqlOperand
		{
			protected static bool IsVisiblePerSetup(PXGraph graph)
			{
				PRSetup payrollPreferences = graph.Caches[typeof(PRSetup)]?.Current as PRSetup ??
					new SelectFrom<PRSetup>.View(graph).SelectSingle();

				return payrollPreferences?.ProjectCostAssignment == ProjectCostAssignmentType.WageLaborBurdenAssigned;
			}
		}

		public class BenefitProject : ByProject
		{
			public override object Evaluate(PXCache cache, object item, Dictionary<Type, object> parameters)
			{
				return Evaluate(cache.Graph, cache.Graph.Caches[typeof(PRPayment)]?.Current as PRPayment);
			}

			public static bool Evaluate(PXGraph graph, PRPayment currentPayment)
			{
				if (IsVisiblePerSetup(graph) 
					|| PRAccountSubHelper.IsVisiblePerSetup<PRSetup.benefitExpenseAcctDefault, PRSetup.benefitExpenseSubMask>(graph, GLAccountSubSource.Project)
					|| PRAccountSubHelper.IsVisiblePerSetup<PRSetup.benefitExpenseAcctDefault, PRSetup.benefitExpenseSubMask>(graph, GLAccountSubSource.Task))
				{
					return true;
				}

				return CostSplitTypeIsFixed(currentPayment) && CostAssignmentType.GetBenefitSetting(currentPayment.LaborCostSplitType).AssignCostToProject;
			}
		}

		public class TaxProject : ByProject
		{
			public override object Evaluate(PXCache cache, object item, Dictionary<Type, object> parameters)
			{
				return Evaluate(cache.Graph, cache.Graph.Caches[typeof(PRPayment)]?.Current as PRPayment);
			}

			public static bool Evaluate(PXGraph graph, PRPayment currentPayment)
			{
				if (IsVisiblePerSetup(graph)
					|| PRAccountSubHelper.IsVisiblePerSetup<PRSetup.taxExpenseAcctDefault, PRSetup.taxExpenseSubMask>(graph, GLAccountSubSource.Project)
					|| PRAccountSubHelper.IsVisiblePerSetup<PRSetup.taxExpenseAcctDefault, PRSetup.taxExpenseSubMask>(graph, GLAccountSubSource.Task))
				{
					return true;
				}

				return CostSplitTypeIsFixed(currentPayment) && CostAssignmentType.GetTaxSetting(currentPayment.LaborCostSplitType).AssignCostToProject;
			}
		}

		public class PTOProject : ByProject
		{
			public override object Evaluate(PXCache cache, object item, Dictionary<Type, object> parameters)
			{
				return Evaluate(cache.Graph, cache.Graph.Caches[typeof(PRPayment)]?.Current as PRPayment);
			}

			public static bool Evaluate(PXGraph graph, PRPayment currentPayment)
			{
				if (IsVisiblePerSetup(graph)
					|| PRAccountSubHelper.IsVisiblePerSetup<PRSetup.ptoExpenseAcctDefault, PRSetup.ptoExpenseSubMask>(graph, GLAccountSubSource.Project)
					|| PRAccountSubHelper.IsVisiblePerSetup<PRSetup.ptoExpenseAcctDefault, PRSetup.ptoExpenseSubMask>(graph, GLAccountSubSource.Task))
				{
					return true;
				}

				return PTOCostSplitTypeIsFixed(currentPayment) && CostAssignmentType.GetSetting(currentPayment.PTOCostSplitType?.FirstOrDefault()).AssignCostToProject;
			}
		}

		public abstract class ByAcctSubMaskCombo<TExpenseAcctDefault, TExpenseSubMask> : BqlFormulaEvaluator<TExpenseAcctDefault, TExpenseSubMask>, IBqlOperand
			where TExpenseAcctDefault : IBqlField
			where TExpenseSubMask : IBqlField
		{
			protected static bool IsVisiblePerSetup(PXGraph graph, string compareValue)
			{
				return PRAccountSubHelper.IsVisiblePerSetup<TExpenseAcctDefault, TExpenseSubMask>(graph, compareValue);
			}
		}

		public class BenefitEarningType : ByAcctSubMaskCombo<PRSetup.benefitExpenseAcctDefault, PRSetup.benefitExpenseSubMask>
		{
			public override object Evaluate(PXCache cache, object item, Dictionary<Type, object> parameters)
			{
				return Evaluate(cache.Graph, cache.Graph.Caches[typeof(PRPayment)]?.Current as PRPayment);
			}

			public static bool Evaluate(PXGraph graph, PRPayment currentPayment)
			{
				if (IsVisiblePerSetup(graph, PRBenefitExpenseAcctSubDefault.MaskEarningType) 
					|| PRAccountSubHelper.IsVisiblePerSetup<PRSetup.benefitExpenseAlternateAcctDefault, PRSetup.benefitExpenseAlternateSubMask>(graph, PRBenefitExpenseAcctSubDefault.MaskEarningType))
				{
					return true;
				}

				return CostSplitTypeIsFixed(currentPayment) && CostAssignmentType.GetBenefitSetting(currentPayment.LaborCostSplitType).AssignCostToEarningType;
			}
		}

		public class BenefitLaborItem : ByAcctSubMaskCombo<PRSetup.benefitExpenseAcctDefault, PRSetup.benefitExpenseSubMask>
		{
			public override object Evaluate(PXCache cache, object item, Dictionary<Type, object> parameters)
			{
				return Evaluate(cache.Graph, cache.Graph.Caches[typeof(PRPayment)]?.Current as PRPayment);
			}

			public static bool Evaluate(PXGraph graph, PRPayment currentPayment)
			{
				if (IsVisiblePerSetup(graph, PRBenefitExpenseAcctSubDefault.MaskLaborItem))
				{
					return true;
				}

				return CostSplitTypeIsFixed(currentPayment) && CostAssignmentType.GetBenefitSetting(currentPayment.LaborCostSplitType).AssignCostToLaborItem;
			}
		}

		public class TaxEarningType : ByAcctSubMaskCombo<PRSetup.taxExpenseAcctDefault, PRSetup.taxExpenseSubMask>
		{
			public override object Evaluate(PXCache cache, object item, Dictionary<Type, object> parameters)
			{
				return Evaluate(cache.Graph, cache.Graph.Caches[typeof(PRPayment)]?.Current as PRPayment);
			}

			public static bool Evaluate(PXGraph graph, PRPayment currentPayment)
			{
				if (IsVisiblePerSetup(graph, PRTaxExpenseAcctSubDefault.MaskEarningType)
					|| PRAccountSubHelper.IsVisiblePerSetup<PRSetup.taxExpenseAlternateAcctDefault, PRSetup.taxExpenseAlternateSubMask>(graph, PRBenefitExpenseAcctSubDefault.MaskEarningType))
				{
					return true;
				}

				return CostSplitTypeIsFixed(currentPayment) && CostAssignmentType.GetTaxSetting(currentPayment.LaborCostSplitType).AssignCostToEarningType;
			}
		}

		public class TaxLaborItem : ByAcctSubMaskCombo<PRSetup.taxExpenseAcctDefault, PRSetup.taxExpenseSubMask>
		{
			public override object Evaluate(PXCache cache, object item, Dictionary<Type, object> parameters)
			{
				return Evaluate(cache.Graph, cache.Graph.Caches[typeof(PRPayment)]?.Current as PRPayment);
			}

			public static bool Evaluate(PXGraph graph, PRPayment currentPayment)
			{
				if (IsVisiblePerSetup(graph, PRTaxExpenseAcctSubDefault.MaskLaborItem))
				{
					return true;
				}

				return CostSplitTypeIsFixed(currentPayment) && CostAssignmentType.GetTaxSetting(currentPayment.LaborCostSplitType).AssignCostToLaborItem;
			}
		}

		public class PTOEarningType : ByAcctSubMaskCombo<PRSetup.ptoExpenseAcctDefault, PRSetup.ptoExpenseSubMask>
		{
			public override object Evaluate(PXCache cache, object item, Dictionary<Type, object> parameters)
			{
				return Evaluate(cache.Graph, cache.Graph.Caches[typeof(PRPayment)]?.Current as PRPayment);
			}

			public static bool Evaluate(PXGraph graph, PRPayment currentPayment)
			{
				if (!PXAccess.FeatureInstalled<FeaturesSet.payrollCAN>())
				{
					return false;
				}	

				if (IsVisiblePerSetup(graph, GLAccountSubSource.EarningType)
					|| PRAccountSubHelper.IsVisiblePerSetup<PRSetup.ptoExpenseAlternateAcctDefault, PRSetup.ptoExpenseAlternateSubMask>(graph, GLAccountSubSource.EarningType))
				{
					return true;
				}

				return PTOCostSplitTypeIsFixed(currentPayment) && CostAssignmentType.GetSetting(currentPayment.PTOCostSplitType?.FirstOrDefault()).AssignCostToEarningType;
			}
		}

		public class PTOLaborItem : ByAcctSubMaskCombo<PRSetup.ptoExpenseAcctDefault, PRSetup.ptoExpenseSubMask>
		{
			public override object Evaluate(PXCache cache, object item, Dictionary<Type, object> parameters)
			{
				return Evaluate(cache.Graph, cache.Graph.Caches[typeof(PRPayment)]?.Current as PRPayment);
			}

			public static bool Evaluate(PXGraph graph, PRPayment currentPayment)
			{
				if (!PXAccess.FeatureInstalled<FeaturesSet.payrollCAN>())
				{
					return false;
				}

				if (IsVisiblePerSetup(graph, GLAccountSubSource.LaborItem))
				{
					return true;
				}

				return PTOCostSplitTypeIsFixed(currentPayment) && CostAssignmentType.GetSetting(currentPayment.PTOCostSplitType?.FirstOrDefault()).AssignCostToLaborItem;
			}
		}

		private static bool CostSplitTypeIsFixed(PRPayment currentPayment) => currentPayment != null &&
			(currentPayment.Paid == true || currentPayment.Released == true) &&
			!string.IsNullOrEmpty(currentPayment.LaborCostSplitType);

		private static bool PTOCostSplitTypeIsFixed(PRPayment currentPayment) => currentPayment != null &&
			(currentPayment.Paid == true || currentPayment.Released == true) &&
			!string.IsNullOrEmpty(currentPayment.PTOCostSplitType);
	}
}
