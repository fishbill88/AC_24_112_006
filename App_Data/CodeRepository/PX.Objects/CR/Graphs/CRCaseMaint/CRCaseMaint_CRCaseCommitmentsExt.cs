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
using PX.Objects.CR.Extensions.CRCaseCommitments;
using PX.Data.BQL.Fluent;

namespace PX.Objects.CR.CRCaseMaint_Extensions
{
	public class CRCaseMaint_CRCaseCommitmentsExt : CRCaseCommitmentsExt<CRCaseMaint>
	{
		public static bool IsActive() => IsExtensionActive();

		#region Methods

		#endregion

		#region Events

		protected virtual void _(Events.RowPersisting<CRCase> e)
		{
			CRCase row = e.Row;

			if (row == null || e.Operation == PXDBOperation.Delete)
				return;

			var originalCaseEntity = e.Cache.CreateCopy(row);

			CalculateCaseCommitments(row, e.Cache.GetOriginal(row) as CRCase);

			if (row?.tstamp != null
				&& e.Cache.ObjectsEqual<
					CRCase.initialResponseDueDateTime,
					CRCase.responseDueDateTime,
					CRCase.resolutionDueDateTime,
					CRCase.solutionActivityNoteID>(row, originalCaseEntity) == false)
			{
				CRCase caseCommitment = SelectFrom<CRCase>
										.Where<CRCase.caseCD.IsEqual<CRCase.caseCD.FromCurrent>>
									.View
									.ReadOnly
									.SelectSingleBound(Base, new object[] { row });

				// hack: temp solution unless commitments in a separate table
				if (caseCommitment?.tstamp != null
					&& caseCommitment.StatusRevision <= row.StatusRevision
					&& Base.SqlDialect.CompareTimestamps(Base.TimeStamp, caseCommitment.tstamp) < 0)
				{
					Base.TimeStamp = caseCommitment.tstamp;
				}
			}
		}

		#endregion
	}
}
