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
		public struct FringeBenefitDecreasingRateKey
		{
			public FringeBenefitDecreasingRateKey(int? projectID, int? deductCodeID, int? laborItemID, int? projectTaskID)
			{
				this.ProjectID = projectID;
				this.DeductCodeID = deductCodeID;
				this.LaborItemID = laborItemID;
				this.ProjectTaskID = projectTaskID;
			}

			public int? ProjectID;
			public int? DeductCodeID;
			public int? LaborItemID;
			public int? ProjectTaskID;
		}
	}
}
