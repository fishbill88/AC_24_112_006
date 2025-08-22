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

using PX.Common;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.Common.Extensions;
using PX.Objects.CS;

namespace PX.Objects.CR.CRCaseMaint_Extensions
{
	[PXInternalUseOnly]
	public sealed class CRCase_CaseCommitmentsCacheExtension : PXCacheExtension<CRCase>
	{
		public static bool IsActive() => PXAccess.FeatureInstalled<FeaturesSet.caseCommitmentsTracking>();

		#region CaseRevisionIDAttribute

		public class CaseRevisionIDAttribute : PXEventSubscriberAttribute, IPXRowPersistedSubscriber, IPXRowPersistingSubscriber
		{
			public virtual void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
			{
				if (e.Row is not CRCase row) return;

				if (e.Operation.Command() == PXDBOperation.Update)
				{
					CRCase caseCommitment =
						SelectFrom<CRCase>
						.Where<CRCase.caseCD.IsEqual<CRCase.caseCD.FromCurrent>>
						.View
						.ReadOnly
						.SelectSingleBound(sender.Graph, new object[] { row });

					if (sender.ObjectsEqual<CRCase.statusRevision>(row, caseCommitment)
					 && sender.ObjectsEqualExceptFields<
							CRCase.initialResponseDueDateTime,
							CRCase.responseDueDateTime,
							CRCase.resolutionDueDateTime>(row, caseCommitment) == false)
					{
						int? revision = row.StatusRevision ?? 0;
						revision++;
						sender.SetValue<CRCase.statusRevision>(row, revision);
					}
				}
			}

			public virtual void RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
			{
				if (e.TranStatus == PXTranStatus.Aborted && e.Operation.Command() == PXDBOperation.Update)
				{
					int? revision = (int?) sender.GetValue(e.Row, _FieldOrdinal);
					revision--;
					sender.SetValue(e.Row, _FieldOrdinal, revision);
				}
			}
		}

		#endregion

		#region StatusRevision

		[PXMergeAttributes(Method = MergeMethod.Replace)]
		[PXDBInt]
		[PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
		[CaseRevisionID]
		public int? StatusRevision { get; set; }

		#endregion
	}
}
