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

using System.Linq;

using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.Common.Extensions;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.PJ.Common.Services;
using PX.Objects.PJ.DailyFieldReports.Descriptor;
using PX.Objects.PJ.DailyFieldReports.PJ.DAC;

namespace PX.Objects.PJ.DailyFieldReports.EP.GraphExtensions
{
	public class EquipmentTimeCardMaintExtension : PXGraphExtension<EquipmentTimeCardMaint>
	{
		public static bool IsActive() => PXAccess.FeatureInstalled<FeaturesSet.projectAccounting>();

		[PXCopyPasteHiddenView]
		public SelectFrom<DailyFieldReportEquipment>.View DailyFieldReportEquipment;


		public virtual void _(Events.RowDeleting<EPEquipmentTimeCard> e)
		{
			var row = e.Row;
			if (row == null
				|| SiteMapExtension.IsDailyFieldReportScreen()
				|| !DoesTimeCardHasRelatedDailyFieldReport(row.TimeCardCD))
				return;

			throw new PXException(CreateWarningMessage());
		}

		public virtual void _(Events.RowDeleting<EPEquipmentDetail> e)
		{
			var row = e.Row;
			if (row == null
				|| SiteMapExtension.IsDailyFieldReportScreen()
				|| !DoesDetailLineHaveRelatedDailyFieldReport(row))
				return;

			Base.Details.View.Ask(CreateWarningMessage(), MessageButtons.OK);
			e.Cancel = true;
		}

		public virtual void _(Events.RowDeleting<EPEquipmentSummary> e)
		{
			var row = e.Row;
			if (row == null)
				return;

			var equipmentDetail = GetEquipmentDetailOfEquipmentSummary(row);
			if (SiteMapExtension.IsDailyFieldReportScreen()
				|| !DoesDetailLineHaveRelatedDailyFieldReport(equipmentDetail))
				return;

			Base.Details.View.Ask(CreateWarningMessage(), MessageButtons.OK);
			e.Cancel = true;
		}

		private static string CreateWarningMessage()
			=> string.Format(DailyFieldReportMessages.EntityCannotBeDeletedBecauseItIsLinked,
				DailyFieldReportEntityNames.Equipment.Capitalize());

		private bool DoesDetailLineHaveRelatedDailyFieldReport(EPEquipmentDetail equipmentDetail)
			=> SelectFrom<DailyFieldReportEquipment>
				.Where<DailyFieldReportEquipment.equipmentTimeCardCd.IsEqual<P.AsString>
					.And<DailyFieldReportEquipment.equipmentDetailLineNumber.IsEqual<P.AsInt>>>
				.View
				.Select(Base, equipmentDetail?.TimeCardCD, equipmentDetail?.LineNbr)
				.Any();

		private bool DoesTimeCardHasRelatedDailyFieldReport(string timeCardCd)
			=> SelectFrom<DailyFieldReportEquipment>
				.Where<DailyFieldReportEquipment.equipmentTimeCardCd.IsEqual<P.AsString>>
				.View
				.Select(Base, timeCardCd)
				.Any();

		private EPEquipmentDetail GetEquipmentDetailOfEquipmentSummary(EPEquipmentSummary equipmentSummary)
		{
			var equipmentSummaryLineNbr = equipmentSummary.LineNbr;
			return SelectFrom<EPEquipmentDetail>
				.Where<EPEquipmentDetail.timeCardCD.IsEqual<P.AsString>
					.And<EPEquipmentDetail.setupSummarylineNbr.IsEqual<P.AsInt>
					.Or<EPEquipmentDetail.runSummarylineNbr.IsEqual<P.AsInt>>
					.Or<EPEquipmentDetail.suspendSummarylineNbr.IsEqual<P.AsInt>>>>
				.View
				.Select(Base,
					equipmentSummary.TimeCardCD, equipmentSummaryLineNbr,
					equipmentSummaryLineNbr, equipmentSummaryLineNbr);
		}
	}
}
