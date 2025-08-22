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

#nullable enable
using PX.Common;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.CR.Extensions.CRCaseCommitments;
using System;
using System.Linq;

namespace PX.Objects.EP
{
	[PXInternalUseOnly]
	public class MailReceiveProcessingGraph : PXGraph<MailReceiveProcessingGraph>
	{
		public MailReceiveProcessingGraph()
		{
			// hack for backward compatibility (Defaults are not set for dummy PXGraph)
			this.Defaults.Remove(typeof(AccessInfo));
		}

		public class CRCaseCommitments : CRCaseCommitmentsExt<MailReceiveProcessingGraph, CRActivity>
		{
			public static bool IsActive() => IsExtensionActive();
		}
	}

	[PXInternalUseOnly]
	public class MailSendProcessingGraph : PXGraph<MailSendProcessingGraph>
	{
		public MailSendProcessingGraph()
		{
			// hack for backward compatibility (Defaults are not set for dummy PXGraph)
			this.Defaults.Remove(typeof(AccessInfo));
		}

		public override void Persist()
		{
			using (PXTransactionScope ts = new PXTransactionScope())
			{
				ForceActivityStatisticsRecalculation();

				base.Persist();

				ts.Complete();
			}
		}

		public virtual void ForceActivityStatisticsRecalculation()
		{
			this.EnsureCachePersistence<CRActivityStatistics>();
			this.EnsureCachePersistence<CRActivity>();

			var emailCache = Caches[typeof(SMEmail)];
			if (emailCache.Current is SMEmail email)
			{
				var activityCache = Caches[typeof(CRActivity)];
				var activity = CRActivity.PK.Find(this, email.RefNoteID);
				if (activity != null)
				{
					activity.CompletedDate = PXFormulaAttribute.Evaluate<CRActivity.completedDate>(activityCache, activity) as DateTime?;
					activityCache.Update(activity);
				}
			}
		}

		public class CRCaseCommitments : CRCaseCommitmentsExt<MailSendProcessingGraph, CRActivity>
		{
			public static bool IsActive() => IsExtensionActive();

			public override CRActivity? TryGetActivity()
			{
				var emailCache = Base.Caches[typeof(SMEmail)];
				if (emailCache.Current is SMEmail email)
				{
					return CRActivity.PK.Find(Base, email.RefNoteID);
				}

				return null;
			}
		}
	}
}


