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

namespace PX.Objects.IN
{
	public class INPICycleMaint : PXGraph<INPICycleMaint>
	{
		public PXSelect<INPICycle> PICycles;
        public PXSavePerRow<INPICycle> Save;
        public PXCancel<INPICycle> Cancel;

		public virtual void INPICycle_CountsPerYear_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
        {
            if (e.NewValue != null)
            {
				if ((((short)e.NewValue) < 0) || (((short)e.NewValue) > 365))
				{
					cache.RaiseExceptionHandling<INPICycle.countsPerYear>(e.Row, e.NewValue,
					new PXSetPropertyException(Messages.ThisValueShouldBeBetweenP0AndP1, PXErrorLevel.Error, 0, 365));
				}
            }
        }

		public virtual void INPICycle_MaxCountInaccuracyPct_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
        {
            if ((((Decimal)e.NewValue) < 0m) || (((Decimal)e.NewValue) > 100m))
            {
				throw new PXSetPropertyException(Messages.PercentageValueShouldBeBetween0And100);
            }
        }

		public virtual void INPICycle_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
		{
			INPICycle row = e.Row as INPICycle;
			if (row == null) return;
            if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Delete) return;
			if (row.CountsPerYear != null && (((short)row.CountsPerYear) < 0) || (((short)row.CountsPerYear) > 365))
			{
				cache.RaiseExceptionHandling<INPICycle.countsPerYear>(e.Row, row.CountsPerYear,
				new PXSetPropertyException(Messages.ThisValueShouldBeBetweenP0AndP1, PXErrorLevel.Error, 0, 365));
			}
		}

	}


}
