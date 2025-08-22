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
using PX.Objects.CM.Extensions;
using System.Linq;

namespace PX.Objects.Extensions.MultiCurrency
{
	public class CuryField
	{
		private readonly ICurrencyAttribute PXCurrencyAttr;
		public string CuryName => PXCurrencyAttr.FieldName;
		public string BaseName => PXCurrencyAttr.ResultField?.Name;

		private bool _ForceBaseCalc = false;
		public bool BaseCalc
		{
			get => _ForceBaseCalc || PXCurrencyAttr.BaseCalc;
			set => _ForceBaseCalc = value;
		}

		public string CuryInfoIDName => PXCurrencyAttr.KeyField?.Name;
		public int? CustomPrecision => PXCurrencyAttr.CustomPrecision;

		public CuryField(ICurrencyAttribute PXCurrencyAttr)
		{
			this.PXCurrencyAttr = PXCurrencyAttr;
		}

		public override string ToString()
		{
			return string.Join(" : ", CuryName, BaseName, CuryInfoIDName);
		}

		public virtual void RecalculateFieldBaseValue(PXCache sender, object row, object curyValue, CurrencyInfo curyInfo, bool? baseCalc = null)
		{
			if (string.IsNullOrEmpty(BaseName) || !(baseCalc ?? BaseCalc)) return;
			if (curyValue == null)
			{
				sender.SetValue(row, BaseName, curyValue);
			}
			else
			{
				if (curyInfo != null && curyInfo.CuryRate != null && curyInfo.BaseCalc == true)
				{
					curyValue = curyInfo.CuryConvBase((decimal)curyValue, CustomPrecision ?? curyInfo.BasePrecision);
					sender.RaiseFieldUpdating(BaseName, row, ref curyValue);
					sender.SetValue(row, BaseName, curyValue);
				}
				else if (curyInfo == null || curyInfo.BaseCalc == true)
				{
					sender.RaiseFieldUpdating(BaseName, row, ref curyValue);
					sender.SetValue(row, BaseName, curyValue);
				}
			}
		}

		public static void SubscribeSimpleCopying(PXCache cache)
		{
			foreach (CuryField field in cache.GetAttributesReadonly(null).OfType<ICurrencyAttribute>().Select(_ => new CuryField(_)))
			{
				cache.Graph.FieldDefaulting.AddHandler(cache.GetItemType(), field.BaseName, (s, e) => e.NewValue = 0m);
				cache.Graph.FieldVerifying.AddHandler(cache.GetItemType(), field.CuryName, field.SimpleCopying);
			}
		}

		private void SimpleCopying(PXCache pXCache, PXFieldVerifyingEventArgs e)
		{
			// Acuminator disable once PX1047 RowChangesInEventHandlersForbiddenForArgs [Insert a new CuryInfo]
			pXCache.SetValue(e.Row, BaseName, e.NewValue);
		}
	}
}
