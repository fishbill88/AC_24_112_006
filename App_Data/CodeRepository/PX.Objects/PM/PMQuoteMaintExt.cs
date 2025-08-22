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
using PX.Objects.Common.Discount;
using PX.Objects.CS;
using System.Linq;
using static PX.Objects.PM.PMQuoteMaint;

namespace PX.Objects.PM
{
	public class PMQuoteMaintExt : PXGraphExtension<PMDiscount, PMQuoteMaint>
	{
		public override void Initialize()
		{
			var sender = Base.Quote.Cache;
			var row = sender.Current as PMQuote;

			if (row != null)
			{
				VisibilityHandler(sender, row);
			}
		}

		protected virtual void PMQuote_RowSelected(PXCache sender, PXRowSelectedEventArgs e, PXRowSelected sel)
		{
			sel?.Invoke(sender, e);

			var row = e.Row as PMQuote;
			if (row == null) return;

			VisibilityHandler(sender, row);
		}

		private void VisibilityHandler(PXCache sender, PMQuote row)
		{
			CR.Standalone.CROpportunityRevision revisionInDb = PXSelectReadonly<CR.Standalone.CROpportunityRevision,
				Where<CR.Standalone.CROpportunityRevision.noteID, Equal<Required<CR.Standalone.CROpportunityRevision.noteID>>>>.Select(Base, row.QuoteID).FirstOrDefault();
			CR.Standalone.CROpportunity opportunityInDb = (revisionInDb == null) ? null : PXSelectReadonly<CR.Standalone.CROpportunity,
				Where<CR.Standalone.CROpportunity.opportunityID, Equal<Required<CR.Standalone.CROpportunity.opportunityID>>>>.Select(Base, revisionInDb.OpportunityID).FirstOrDefault();

			CR.Standalone.CROpportunity opportunity = PXSelect<CR.Standalone.CROpportunity,
				Where<CR.Standalone.CROpportunity.opportunityID, Equal<Required<CR.Standalone.CROpportunity.opportunityID>>>>.Select(Base, row.OpportunityID).FirstOrDefault();

			var opportunityIsClosed = opportunity?.IsActive == false;

			bool allowUpdate = row.IsDisabled != true && !opportunityIsClosed && row.Status != PMQuoteStatusAttribute.Closed;

			if (opportunityInDb?.OpportunityID == opportunity?.OpportunityID)
			{
				if (!DimensionMaint.IsAutonumbered(sender.Graph, ProjectAttribute.DimensionName))
				{
					var quoteCache = Base.Caches[typeof(PMQuote)];
					foreach (var field in quoteCache.Fields)
					{
						if (!quoteCache.Keys.Contains(field) &&
							field != quoteCache.GetField(typeof(PMQuote.isPrimary)) &&
							field != quoteCache.GetField(typeof(PMQuote.curyID)) &&
							field != quoteCache.GetField(typeof(PMQuote.locationID)))
							PXUIFieldAttribute.SetEnabled(sender, row, field, allowUpdate);
					}
				}
				else
				{
					PXUIFieldAttribute.SetEnabled(sender, row, allowUpdate);
				}
			}
			else
			{
				var quoteCache = Base.Caches[typeof(PMQuote)];
				foreach (var field in quoteCache.Fields)
				{
					if (!quoteCache.Keys.Contains(field) &&
						field != quoteCache.GetField(typeof(PMQuote.opportunityID)) &&
						field != quoteCache.GetField(typeof(PMQuote.isPrimary)))
						PXUIFieldAttribute.SetEnabled(sender, row, field, allowUpdate);
				}
			}

			Base.Caches[typeof(PMQuote)].AllowDelete = !opportunityIsClosed;
			foreach (var type in new[]
			{
					typeof(CR.CROpportunityDiscountDetail),
					typeof(CR.CROpportunityProducts),
					typeof(CR.CRTaxTran),
					typeof(CR.CRAddress),
					typeof(CR.CRContact),
					typeof(CR.CRPMTimeActivity),
					typeof(PM.PMQuoteTask)
				})
			{
				Base.Caches[type].AllowInsert = Base.Caches[type].AllowUpdate = Base.Caches[type].AllowDelete = allowUpdate;
			}

			if (!DimensionMaint.IsAutonumbered(sender.Graph, ProjectAttribute.DimensionName))
			{
				if (row.Status == PMQuoteStatusAttribute.Approved)
					PXUIFieldAttribute.SetEnabled<PMQuote.quoteProjectCD>(sender, row);
				else if (row.Status == PMQuoteStatusAttribute.Draft)
				{
					PXUIFieldAttribute.SetEnabled<PMQuote.opportunityID>(sender, row, row.OpportunityID == null || !Base.IsReadonlyPrimaryQuote(row.QuoteID));
					if (row.OpportunityID == null)
						PXUIFieldAttribute.SetEnabled<PMQuote.bAccountID>(sender, row, row.OpportunityID == null);
				}
			}		

			Base.Caches[typeof(CopyQuoteFilter)].AllowUpdate = true;
			Base.Caches[typeof(RecalcDiscountsParamFilter)].AllowUpdate = true;

			if (opportunityInDb?.OpportunityID != opportunity?.OpportunityID)
			{
				PXUIFieldAttribute.SetEnabled<PMQuote.subject>(sender, row, true);
				PXUIFieldAttribute.SetEnabled<PMQuote.status>(sender, row, false);
			}
		}
	}
}
