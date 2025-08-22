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
using PX.Payroll.Data;

namespace PX.Objects.PR
{
	public class TaxSplitWageTypeListAttribute : PXIntListAttribute, IPXRowSelectedSubscriber
	{
		private static int[] _DefaultValues = new int[] { TaxSplitWageType.Tips, TaxSplitWageType.Others };
		private static string[] _DefaultLabels = new string[] { Messages.Tips, Messages.NotTips };

		[InjectDependency]
		protected IPayrollSettingsCache SettingsCache { get; set; }

		public TaxSplitWageTypeListAttribute() : base(
			new int[] { TaxSplitWageType.Tips, TaxSplitWageType.Others },
			new string[] { Messages.Tips, Messages.NotTips })
		{
		}

		public void RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			PRPayment currentPayment = (PRPayment)sender.Graph.Caches[typeof(PRPayment)].Current;
			if (currentPayment != null)
			{
				if (currentPayment.CountryID == LocationConstants.USCountryCode)
				{
					_AllowedValues = SettingsCache.GetUSWageTypeValues();
					_AllowedLabels = SettingsCache.GetUSWageTypeLabels();
				}
				else
				{
					_AllowedValues = _DefaultValues;
					_AllowedLabels = _DefaultLabels;
				}
			}
		}
	}
}
