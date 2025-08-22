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
using PX.Objects.Common.Exceptions;
using PX.Objects.IN;
using System.Collections.Generic;

namespace PX.Objects.AM
{
	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	public class AMVendorShipmentItemAvailabilityExtension : IN.GraphExtensions.ItemAvailabilityExtension<VendorShipmentEntry, AMVendorShipLine, AMVendorShipLineSplit>
	{
		protected override AMVendorShipLineSplit EnsureSplit(ILSMaster row)
			=> Base.FindImplementation<AMVendorShipmentLineSplittingExtension>().EnsureSplit(row);

		protected override decimal GetUnitRate(AMVendorShipLine line) => GetUnitRate<AMVendorShipLine.inventoryID, AMVendorShipLine.uOM>(line);

		protected override string GetStatus(AMVendorShipLine line)
		{
			string status = string.Empty;

			if (FetchWithLineUOM(line, excludeCurrent: line?.Released != true, CostCenter.FreeStock) is IStatus availability)
			{
				status = FormatStatus(availability, line.UOM);
			}

			return status;
		}

		private string FormatStatus(IStatus availability, string uom)
		{
			return PXMessages.LocalizeFormatNoPrefixNLA(
				Messages.LSTranStatus,
				uom,
				FormatQty(availability.QtyOnHand.GetValueOrDefault()),
				FormatQty(availability.QtyAvail.GetValueOrDefault()),
				FormatQty(availability.QtyHardAvail.GetValueOrDefault()));
		}

		protected override void Check(ILSMaster row, IStatus availability)
		{
			if (row != null && row.BaseQty.GetValueOrDefault() != 0)
			{
				foreach (var errorInfo in GetCheckErrorsQtyOnHand(row, availability))
					RaiseQtyExceptionHandling(row, errorInfo, row.Qty);
			}

			//base.Check(row, availability);
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
			AMBatch doc = (AMBatch)Base.Caches<AMBatch>().Current; // seems it should be AMVendorShipment instead (Base.Document.Current)
			if (row.InvtMult == -1 && row.BaseQty > 0m && availability != null)
				if (availability.QtyOnHand - row.Qty < 0m && doc?.Released == false)
					return false;

			return true;
		}

		protected override void RaiseQtyExceptionHandling(AMVendorShipLine line, PXExceptionInfo ei, decimal? newValue)
		{
#if DEBUG
			AMDebug.TraceWriteMethodName(ei.MessageFormat,
				LineCache.GetStateExt<AMVendorShipLine.inventoryID>(line),
				LineCache.GetStateExt<AMVendorShipLine.subItemID>(line),
				LineCache.GetStateExt<AMVendorShipLine.siteID>(line),
				LineCache.GetStateExt<AMVendorShipLine.locationID>(line),
				LineCache.GetValue<AMVendorShipLine.lotSerialNbr>(line));
#endif
			LineCache.RaiseExceptionHandling<AMVendorShipLine.qty>(line, newValue,
				new PXSetPropertyException(ei.MessageFormat, ei.ErrorLevel ?? PXErrorLevel.Warning,
					LineCache.GetStateExt<AMVendorShipLine.inventoryID>(line),
					LineCache.GetStateExt<AMVendorShipLine.subItemID>(line),
					LineCache.GetStateExt<AMVendorShipLine.siteID>(line),
					LineCache.GetStateExt<AMVendorShipLine.locationID>(line),
					LineCache.GetValue<AMVendorShipLine.lotSerialNbr>(line)));
		}

		protected override void RaiseQtyExceptionHandling(AMVendorShipLineSplit split, PXExceptionInfo ei, decimal? newValue)
		{
			SplitCache.RaiseExceptionHandling<AMVendorShipLineSplit.qty>(split, newValue,
				new PXSetPropertyException(ei.MessageFormat, ei.ErrorLevel ?? PXErrorLevel.Warning,
					SplitCache.GetStateExt<AMVendorShipLineSplit.inventoryID>(split),
					SplitCache.GetStateExt<AMVendorShipLineSplit.subItemID>(split),
					SplitCache.GetStateExt<AMVendorShipLineSplit.siteID>(split),
					SplitCache.GetStateExt<AMVendorShipLineSplit.locationID>(split),
					SplitCache.GetValue<AMVendorShipLineSplit.lotSerialNbr>(split)));
		}
	}
}
