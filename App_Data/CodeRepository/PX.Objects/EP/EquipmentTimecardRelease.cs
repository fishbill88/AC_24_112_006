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

using System.Collections.Generic;
using PX.Data;
using System;

namespace PX.Objects.EP
{
    [GL.TableDashboardType]
    [Serializable]
    public class EquipmentTimeCardRelease : PXGraph<EquipmentTimeCardRelease>
    {
        #region Selects

        [PXViewName(Messages.EquipmentTimeCard)]
        [PXFilterable]
        public PXProcessingJoin<EPEquipmentTimeCard,
        LeftJoin<EPApproval, On<EPApproval.refNoteID, Equal<EPEquipmentTimeCard.noteID>>>,
        Where<EPEquipmentTimeCard.isApproved, Equal<True>, And<EPEquipmentTimeCard.isReleased, Equal<False>>>, OrderBy<Asc<EPEquipmentTimeCard.timeCardCD>>> FilteredItems;

        public PXSetup<EPSetup> Setup;

        #endregion

        
        [PXInt]
        [PXUIField(DisplayName = "Setup", Enabled = false)]
        protected virtual void EPEquipmentTimeCard_TimeSetupCalc_CacheAttached(PXCache sender)
        {
        }

        [PXInt]
        [PXUIField(DisplayName = "Billable Setup", Enabled = false)]
        protected virtual void EPEquipmentTimeCard_TimeBillableSetupCalc_CacheAttached(PXCache sender)
        {
        }

        #region Ctors

        public EquipmentTimeCardRelease()
        {
            FilteredItems.SetProcessCaption(Messages.Release);
            FilteredItems.SetProcessAllCaption(Messages.ReleaseAll);
            FilteredItems.SetSelected<EPTimeCard.selected>();

            FilteredItems.SetProcessDelegate(EquipmentTimeCardRelease.Release);

            Actions.Move("Process", "Cancel");
        }

        #endregion

        #region Actions

        public PXCancel<EPEquipmentTimeCard> Cancel;

        #endregion

        protected virtual void EPEquipmentTimeCard_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            EPEquipmentTimeCard row = e.Row as EPEquipmentTimeCard;
            if (row == null) return;

            RecalculateTotals(row);
        }

        protected virtual void RecalculateTotals(EPEquipmentTimeCard timecard)
        {
            if (timecard == null)
                throw new ArgumentNullException();
            
            PXSelectBase<EPEquipmentDetail> select = new PXSelect<EPEquipmentDetail, 
                Where<EPEquipmentDetail.timeCardCD, Equal<Required<EPEquipmentTimeCard.timeCardCD>>>>(this);


            int setup = 0;
            int run = 0;
            int suspend = 0;
            int setupBillable = 0;
            int runBillable = 0;
            int suspendBillable = 0;

            foreach (EPEquipmentDetail detail in select.Select(timecard.TimeCardCD))
            {
                setup += detail.SetupTime.GetValueOrDefault();
                run += detail.RunTime.GetValueOrDefault();
                suspend += detail.SuspendTime.GetValueOrDefault();

                if (detail.IsBillable == true)
                {
                    setupBillable += detail.SetupTime.GetValueOrDefault();
                    runBillable += detail.RunTime.GetValueOrDefault();
                    suspendBillable += detail.SuspendTime.GetValueOrDefault();
                }
            }


            timecard.TimeSetupCalc = setup;
            timecard.TimeRunCalc = run;
            timecard.TimeSuspendCalc = suspend;

            timecard.TimeBillableSetupCalc = setupBillable;
            timecard.TimeBillableRunCalc = runBillable;
            timecard.TimeBillableSuspendCalc = suspendBillable;

            List<EPEquipmentDetail> details = new List<EPEquipmentDetail>();
            foreach (EPEquipmentDetail detail in select.Select(timecard.TimeCardCD))
            {
                details.Add(detail);
            }
        }

        
        public static void Release(List<EPEquipmentTimeCard> timeCards)
        {
            EquipmentTimeCardMaint timeCardMaint = PXGraph.CreateInstance<EquipmentTimeCardMaint>();
            foreach (EPEquipmentTimeCard item in timeCards)
            {
                timeCardMaint.Clear();
                timeCardMaint.Document.Current = timeCardMaint.Document.Search<EPEquipmentTimeCard.timeCardCD>(item.TimeCardCD);
                timeCardMaint.release.Press();
            }
        }

        

    }
}
