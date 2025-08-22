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

using System;
using PX.Data;
using PX.Dashboards.Widgets;
using PX.Olap;

namespace PX.Objects.SM
{
	public class ChartSettingsMaintExt : PXGraphExtension<ChartSettingsMaint>
	{
		[PXOverride]
		public virtual SortType DetermineSortType(string field, Func<string, SortType> del)
		{
			if (PivotMaintExt.TryDetermineSortType(Base.DataScreen, field, out var sortType))
				return sortType;

			return del(field);
		}

		public virtual void ChartSettings_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			if (!e.ExternalCall) return;

			ChartSettings row = (ChartSettings)e.Row, oldRow = (ChartSettings)e.OldRow;

			if (row.CategoryField != oldRow.CategoryField
			    && PivotMaintExt.IsFinPeriod(Base.DataScreen, row.CategoryField))
			{
				sender.SetValue<ChartSettings.categorySortType>(row, SortTypeListAttribute.Legend);
				sender.SetValue<ChartSettings.categorySortOrder>(row, 0); // ascending
			}

			if (row.SeriesField != oldRow.SeriesField
				&& PivotMaintExt.IsFinPeriod(Base.DataScreen, row.SeriesField))
			{
				sender.SetValue<ChartSettings.seriesSortType>(row, SortTypeListAttribute.Legend);
				sender.SetValue<ChartSettings.seriesSortOrder>(row, 0); // ascending
			}
		}
	}
}
