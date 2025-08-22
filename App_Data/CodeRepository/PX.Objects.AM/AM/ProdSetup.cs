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
using System.Collections;
using PX.Objects.CS;
using PX.Data.BQL.Fluent;
using PX.Data.BQL;
using System.Linq;

namespace PX.Objects.AM
{
    public class ProdSetup : PXGraph<ProdSetup>
    {
        public PXSelect<AMPSetup> ProdSetupRecord;
        public PXSave<AMPSetup> Save;
        public PXCancel<AMPSetup> Cancel;
        public PXSelect<AMScanSetup, Where<AMScanSetup.branchID, Equal<Current<AccessInfo.branchID>>>> ScanSetup;

        protected virtual IEnumerable scanSetup()
        {
            AMScanSetup result = PXSelect<AMScanSetup, Where<AMScanSetup.branchID, Equal<Current<AccessInfo.branchID>>>>.Select(this);
            return new AMScanSetup[] { result ?? ScanSetup.Insert() };
        }

        public PXSelect<AMScanUserSetup, Where<AMScanUserSetup.isOverridden, Equal<False>>> ScanUserSetups;

		protected virtual void _(Events.RowUpdated<AMScanSetup> e)
        {
            if (e.Row == null)
            {
                return;
            }

            foreach (AMScanUserSetup userSetup in ScanUserSetups.Select())
            {
                userSetup.DefaultWarehouse = e.Row.DefaultWarehouse;
				userSetup.DefaultLotSerialNumber = e.Row.DefaultLotSerialNumber;
				userSetup.DefaultExpireDate = e.Row.DefaultExpireDate;
				ScanUserSetups.Update(userSetup);
            }
        }

        public override void Persist()
        {
            var prodSetup = ProdSetupRecord?.Current;
            if (prodSetup == null)
            {
                return;
            }

            var setupRowStatus = ProdSetupRecord.Cache.GetStatus(prodSetup);
            var oldSchdBlockSize = (int?)ProdSetupRecord.Cache.GetValueOriginal<AMPSetup.schdBlockSize>(prodSetup);
            var newBlockSize = prodSetup.SchdBlockSize;
            var blockSizeChanged = setupRowStatus == PXEntryStatus.Updated && newBlockSize != null && oldSchdBlockSize != null && newBlockSize != oldSchdBlockSize;

            if (blockSizeChanged)
            {
                //Postpone the change of block size until the SyncBlockSizeChange process completes (will setup table there)
                prodSetup.SchdBlockSize = oldSchdBlockSize;
                ProdSetupRecord.Update(prodSetup);
            }
            else if (setupRowStatus == PXEntryStatus.Inserted || setupRowStatus == PXEntryStatus.Updated)
            {
                var setup = (AMAPSMaintenanceSetup)PXSelect<AMAPSMaintenanceSetup>.Select(this);
                if (setup == null)
                {
                    Common.Cache.AddCacheView<AMAPSMaintenanceSetup>(this);
                    this.Caches<AMAPSMaintenanceSetup>().Insert();
                }
            }

            base.Persist();

            if (blockSizeChanged)
            {
                PXLongOperation.StartOperation(this, () =>
                {
                    SyncBlockSizeChange(newBlockSize, oldSchdBlockSize.GetValueOrDefault());
                });
            }
        }

        protected static void SyncBlockSizeChange(int? newBlockSize, int? oldBlockSize)
        {
            if (newBlockSize == null || oldBlockSize == null)
            {
                return;
            }

            APSMaintenanceProcess.UpdateBlockSizeProcess(newBlockSize.GetValueOrDefault(), oldBlockSize.GetValueOrDefault());
        }

        protected virtual void AMPSetup_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {
            var row = (AMPSetup)e.Row;
            if (row == null)
            {
                return;
            }

            PXUIFieldAttribute.SetRequired<AMPSetup.fixMfgCalendarID>(cache, row.FMLTime.GetValueOrDefault());
            PXUIFieldAttribute.SetRequired<AMPSetup.fMLTimeUnits>(cache, false);
        }

        protected virtual void AMPSetup_SchdBlockSize_FieldUpdating(PXCache cache, PXFieldUpdatingEventArgs e)
        {
            var row = (AMPSetup) e.Row;
            if (row?.SchdBlockSize == null || IsImport || IsContractBasedAPI)
            {
                return;
            }

            var scheduleExists = (AMWCSchd)PXSelect<AMWCSchd>.SelectWindowed(this, 0, 1);

            if (scheduleExists == null)
            {
                return;
            }

            if (ProdSetupRecord.Ask(Messages.ChangingBlockSizeMsg, MessageButtons.YesNo) != WebDialogResult.Yes)
            {
                e.Cancel = true;
                e.NewValue = row.SchdBlockSize;
            }
        }

		protected virtual void _(Events.FieldUpdating<AMPSetup, AMPSetup.lockWorkflowEnabled> e)
		{
			if (e.Row == null && e.NewValue == null)
			{
				return;
			}
			if (PXSelect<AMProdItem, Where<AMProdItem.locked, Equal<True>,
					And<AMProdItem.closed, Equal<False>>>>.SelectWindowed(this, 0, 1).Any()
				&& (bool)e.NewValue == false)
			{				
				e.Cancel = true;
				throw new PXSetPropertyException(Messages.DisableLockProductionWorkflowExist, PXErrorLevel.Error);
			}
		}

		protected virtual void AMPSetup_FMLTime_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            var productionSetup = (AMPSetup)e.Row;
            if (productionSetup == null)
            {
                return;
            }

            if (productionSetup.FMLTime.GetValueOrDefault() && productionSetup.FMLTimeUnits == null)
            {
                //Default days
                productionSetup.FMLTimeUnits = Attributes.TimeUnits.Days;
            }
        }

        protected virtual void AMPSetup_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            var productionSetup = (AMPSetup)e.Row;
            if (productionSetup == null)
            {
                return;
            }

            if (productionSetup.FixMfgCalendarID == null && (productionSetup.FMLTime ?? false))
            {
                sender.RaiseExceptionHandling<AMPSetup.fixMfgCalendarID>(productionSetup, productionSetup.FixMfgCalendarID, new PXSetPropertyException(Messages.MissingFixMfgCalendar, PXErrorLevel.Error));
            }
        }

        protected virtual void AMPSetup_CTPOrderType_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {            
            if (e.NewValue == null)
                return;

            PXResult<AMOrderType, Numbering> result = (PXResult<AMOrderType, Numbering>)SelectFrom<AMOrderType>.InnerJoin<Numbering>.
                On<AMOrderType.prodNumberingID.IsEqual<Numbering.numberingID>>.
                Where<AMOrderType.orderType.IsEqual<@P.AsString>>.View.Select(this, e.NewValue).First();
            if(result != null && ((Numbering)result).UserNumbering == true)
            {
                throw new PXSetPropertyException(Messages.GetLocal(Messages.UnableToUpdateField,
                    PXUIFieldAttribute.GetDisplayName<AMPSetup.cTPOrderType>(ProdSetupRecord.Cache),
                    Messages.GetLocal(Messages.CTPOrderTypeCannotBeManual)));
            }
        }
    }
}
