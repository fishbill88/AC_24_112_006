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

namespace PX.Objects.CS
{
	public class FOBPointMaint : PXGraph<FOBPointMaint>
	{
        public PXSavePerRow<FOBPoint> Save;
		public PXCancel<FOBPoint> Cancel;
		[PXImport(typeof(FOBPoint))]
		public PXSelect<FOBPoint> FOBPoint;

		protected virtual void FOBPoint_RowInserting(PXCache cache, PXRowInsertingEventArgs e)
		{
			FOBPoint fob = PXSelect<FOBPoint, Where<FOBPoint.fOBPointID, Equal<Required<FOBPoint.fOBPointID>>>>.SelectWindowed(this, 0, 1, ((FOBPoint)e.Row).FOBPointID);
			if (fob != null)
			{
				cache.RaiseExceptionHandling<FOBPoint.fOBPointID>(e.Row, ((FOBPoint)e.Row).FOBPointID, new PXException(Messages.RecordAlreadyExists));
				e.Cancel = true;
			}
		}
	}
}
