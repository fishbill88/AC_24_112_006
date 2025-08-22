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

using PX.Objects.AM.Attributes;
using PX.Data;

namespace PX.Objects.AM
{
    public class LaborCodeMaint : PXGraph<LaborCodeMaint>
    {
		[PXImport(typeof(AMLaborCode))]
        public PXSelect<AMLaborCode> LaborCodeRecords;
        public PXSavePerRow<AMLaborCode> Save;
		public PXCancel<AMLaborCode> Cancel;

        protected virtual void AMLaborCode_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            AMLaborCode amLaborCode = (AMLaborCode) e.Row;
            if (amLaborCode == null)
            {
                return;
            }

            PXUIFieldAttribute.SetEnabled<AMLaborCode.overheadAccountID>(sender, e.Row, amLaborCode.LaborType == AMLaborType.Indirect);
            PXUIFieldAttribute.SetEnabled<AMLaborCode.overheadSubID>(sender, e.Row, amLaborCode.LaborType == AMLaborType.Indirect);
        }

        protected virtual void AMLaborCode_LaborType_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            var amLaborCode = (AMLaborCode)e.Row;
            if (amLaborCode == null || string.IsNullOrWhiteSpace(amLaborCode.LaborType))
            {
                return;
            }

            if (amLaborCode.LaborType == AMLaborType.Direct && (amLaborCode.OverheadAccountID != null || amLaborCode.OverheadSubID != null))
            {
                amLaborCode.OverheadAccountID = null;
                amLaborCode.OverheadSubID = null;
            }
        }

        protected virtual void AMLaborCode_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            var amLaborCode = (AMLaborCode) e.Row;
            if (amLaborCode != null && amLaborCode.LaborType == AMLaborType.Indirect)
            {
                if (amLaborCode.OverheadAccountID == null)
                {
                    sender.RaiseExceptionHandling<AMLaborCode.overheadAccountID>(amLaborCode,
                        amLaborCode.OverheadAccountID,
                        new PXSetPropertyException(Messages.OverheadAccountRequiredIndirectLabor, PXErrorLevel.Error));
                }
                if (amLaborCode.OverheadSubID == null)
                {
                    sender.RaiseExceptionHandling<AMLaborCode.overheadSubID>(amLaborCode, amLaborCode.OverheadSubID,
                        new PXSetPropertyException(Messages.OverheadAccountRequiredIndirectLabor, PXErrorLevel.Error));
                }
            }
        }

        protected virtual void AMLaborCode_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
        {
            var amLaborCode = (AMLaborCode) e.Row;
            if (amLaborCode == null)
            {
                return;
            }

            // Check to see if the Direct Labor code is used in a work center
            AMShift amShift = PXSelect<AMShift, Where<AMShift.laborCodeID, Equal<Required<AMShift.laborCodeID>>>>.Select(this, amLaborCode.LaborCodeID);
            if (amShift != null)
            {
                e.Cancel = true;
                throw new PXException(Messages.GetLocal(Messages.LaborCodeNoDeleteUsedInWC), amShift.WcID, amShift.ShiftCD);
            }
        }
    }
}
