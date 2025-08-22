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
using PX.Objects.Common.Exceptions;

namespace PX.Objects.IN.GraphExtensions
{
	public abstract class INRegisterItemAvailabilityExtension<TRegisterGraph> : ItemAvailabilityExtension<TRegisterGraph, INTran, INTranSplit>
		where TRegisterGraph : INRegisterEntryBase
	{
		protected override INTranSplit EnsureSplit(ILSMaster row)
			=> Base.FindImplementation<INRegisterLineSplittingExtension<TRegisterGraph>>().EnsureSplit(row);

		protected override decimal GetUnitRate(INTran line) => GetUnitRate<INTran.inventoryID, INTran.uOM>(line);

		protected override string GetStatus(INTran line)
		{
			string status = string.Empty;
						
			INRegister currentINRegister = (INRegister)Base.Caches[typeof(INRegister)].Current;

			bool excludeCurrent = line?.Released != true && ( line.OrigModule != GL.BatchModule.IN || ( Base.insetup.Current.AllocateDocumentsOnHold == true || currentINRegister.Hold == false));

			if (FetchWithLineUOM(line, excludeCurrent, line.CostCenterID) is IStatus availability)
			{
				status = FormatStatus(availability, line.UOM);
				Check(line, availability);
			}

			return status;
		}

		private string FormatStatus(IStatus availability, string uom)
		{
			return PXMessages.LocalizeFormatNoPrefixNLA(
				Messages.Availability_ActualInfo,
				uom,
				FormatQty(availability.QtyOnHand),
				FormatQty(availability.QtyAvail),
				FormatQty(availability.QtyHardAvail),
				FormatQty(availability.QtyActual));
		}

		protected override IEnumerable<PXExceptionInfo> GetCheckErrors(ILSMaster row, IStatus availability)
		{
			foreach (var errorInfo in base.GetCheckErrors(row, availability))
				yield return errorInfo;

			foreach (var errorInfo in GetCheckErrorsQtyOnHand(row, availability))
				yield return errorInfo;
		}

		protected virtual IEnumerable<PXExceptionInfo> GetCheckErrorsQtyOnHand(ILSMaster row, IStatus availability)
		{
			if (!IsAvailableOnHandQty(row, availability))
			{
				string message = GetErrorMessageQtyOnHand(GetStatusLevel(availability));

				if (message != null)
					yield return new PXExceptionInfo(PXErrorLevel.RowWarning, message);
			}
		}

		protected virtual bool IsAvailableOnHandQty(ILSMaster row, IStatus availability)
		{
			if (row.InvtMult == -1 && row.BaseQty > 0m && availability != null)
				if (availability.QtyOnHand - row.Qty < 0m && Base.INRegisterDataMember.Current?.Released == false)
					return false;

			return true;
		}

		protected override void RaiseQtyExceptionHandling(INTran line, PXExceptionInfo ei, decimal? newValue)
		{
			LineCache.RaiseExceptionHandling<INTran.qty>(line, newValue,
				new PXSetPropertyException(ei.MessageFormat, ei.ErrorLevel ?? PXErrorLevel.Warning,
					LineCache.GetStateExt<INTran.inventoryID>(line),
					LineCache.GetStateExt<INTran.subItemID>(line),
					LineCache.GetStateExt<INTran.siteID>(line),
					LineCache.GetStateExt<INTran.locationID>(line),
					LineCache.GetValue<INTran.lotSerialNbr>(line)));
		}

		protected override void RaiseQtyExceptionHandling(INTranSplit split, PXExceptionInfo ei, decimal? newValue)
		{
			SplitCache.RaiseExceptionHandling<INTranSplit.qty>(split, newValue,
				new PXSetPropertyException(ei.MessageFormat, ei.ErrorLevel ?? PXErrorLevel.Warning,
					SplitCache.GetStateExt<INTranSplit.inventoryID>(split),
					SplitCache.GetStateExt<INTranSplit.subItemID>(split),
					SplitCache.GetStateExt<INTranSplit.siteID>(split),
					SplitCache.GetStateExt<INTranSplit.locationID>(split),
					SplitCache.GetValue<INTranSplit.lotSerialNbr>(split)));
		}
	}
}
