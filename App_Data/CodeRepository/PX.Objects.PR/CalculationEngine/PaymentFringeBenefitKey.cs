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

namespace PX.Objects.PR
{
	[PXHidden]
	public partial class PRCalculationEngine : PXGraph<PRCalculationEngine>
	{
		protected struct PaymentFringeBenefitKey
		{
			public PaymentFringeBenefitKey(PRPaymentFringeBenefit paymentFringeBenefit) : this(
				paymentFringeBenefit.ProjectID,
				paymentFringeBenefit.LaborItemID,
				paymentFringeBenefit.ProjectTaskID)
			{ }

			public PaymentFringeBenefitKey(int? projectID, int? laborItemID, int? projectTaskID)
			{
				this.ProjectID = projectID;
				this.LaborItemID = laborItemID;
				this.ProjectTaskID = projectTaskID;
			}

			public int? ProjectID;
			public int? LaborItemID;
			public int? ProjectTaskID;
		}
	}
}
