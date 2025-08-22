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
using PX.Objects.GL;

namespace PX.Objects.AR
{
	public class ARFinChargesMaint : PXGraph<ARFinChargesMaint, ARFinCharge>
	{
		public PXSelect<ARFinCharge> ARFinChargesList;
		public PXSelect<ARFinChargePercent,
			Where<ARFinChargePercent.finChargeID, Equal<Current<ARFinCharge.finChargeID>>>,
			OrderBy<Asc<ARFinChargePercent.beginDate>>> PercentList;

		public ARFinChargesMaint()
		{
			GLSetup setup = GLSetup.Current;
		}

		public PXSetup<GLSetup> GLSetup;
		#region events
		protected virtual void ARFinCharge_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
		{
			ARFinCharge row = (ARFinCharge)e.Row;
			PXDefaultAttribute.SetPersistingCheck<ARFinCharge.feeAccountID>(cache, row, row.FeeAmount !=0m ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
			PXDefaultAttribute.SetPersistingCheck<ARFinCharge.feeSubID>(cache, row, row.FeeAmount != 0m ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
			if (row.PercentFlag == true)
			{
 				if(PercentList.Select().Count<1 && e.Operation!=PXDBOperation.Delete)
				{
                    cache.RaiseExceptionHandling<ARFinCharge.chargingMethod>(row, row.ChargingMethod,
                            new PXSetPropertyException(Messages.PercentListEmpty, PXErrorLevel.Error));
                }
			}
			else if (row.MinFinChargeAmount == 0 || row.MinFinChargeFlag == false)
			{
				cache.RaiseExceptionHandling<ARFinCharge.fixedAmount>(row, row.FixedAmount, new PXSetPropertyException(Messages.FixedAmountBelowMin, PXErrorLevel.RowWarning));
			}
		}
		protected virtual void ARFinCharge_RowInserting(PXCache cache, PXRowInsertingEventArgs e)
		{
			ARFinCharge c = PXSelect<ARFinCharge, Where<ARFinCharge.finChargeID, Equal<Required<ARFinCharge.finChargeID>>>>.SelectWindowed(this, 0, 1, ((ARFinCharge)e.Row).FinChargeID);
			if (c != null)
			{
                cache.RaiseExceptionHandling<ARFinCharge.finChargeID>(e.Row, ((ARFinCharge)e.Row).FinChargeID, new PXException(Messages.RecordAlreadyExists));
				e.Cancel = true;
			}
		}

		protected virtual void ARFinCharge_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			ARFinCharge fin = e.Row as ARFinCharge;

			if (fin == null)
			{
				return;
			}

			bool useFee = fin.FeeAmount != null && fin.FeeAmount != 0m;
			PXUIFieldAttribute.SetEnabled<ARFinCharge.feeAccountID>(cache, fin, useFee);
			PXUIFieldAttribute.SetEnabled<ARFinCharge.feeSubID>(cache, fin, useFee);
			PXUIFieldAttribute.SetEnabled<ARFinCharge.feeDesc>(cache, fin, useFee);
            PXUIFieldAttribute.SetVisible<ARFinCharge.fixedAmount>(cache, fin, fin.ChargingMethod == OverdueChargingMethod.FixedAmount);
            PXUIFieldAttribute.SetVisible<ARFinCharge.lineThreshold>(cache, fin, fin.ChargingMethod == OverdueChargingMethod.PercentWithThreshold);
            PXUIFieldAttribute.SetVisible<ARFinCharge.minFinChargeAmount>(cache, fin, fin.ChargingMethod == OverdueChargingMethod.PercentWithMinAmount);
            PercentList.AllowSelect = fin.PercentFlag == true;
            PercentList.AllowInsert = fin.PercentFlag == true;
			PercentList.AllowUpdate = fin.PercentFlag == true;
			PercentList.AllowDelete = fin.PercentFlag == true;
        }

		protected virtual void ARFinChargePercent_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			ARFinChargePercent rateDetail = e.Row as ARFinChargePercent;

			if (rateDetail == null) return;

			if (rateDetail.FinChargePercent < 0m)
			{
				sender.RaiseExceptionHandling<ARFinChargePercent.finChargePercent>(
					rateDetail,
					rateDetail.FinChargePercent,
					new PXSetPropertyException<ARFinChargePercent.finChargePercent>(
						CS.Messages.Entry_GE, 0.ToString()));
			}
		}

        #endregion

    }
}
