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

using System;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.GL.Attributes;
using static PX.Objects.AP.APVendorBalanceEnq;

namespace PX.Objects.FS
{
	/// <exclude/>
	// Acuminator disable once PX1094 NoPXHiddenOrPXCacheNameOnDac - legacy code
	[Serializable]
	public class SchedulerScreenFilter : PXBqlTable, PX.Data.IBqlTable
	{
		#region OrganizationID
		public abstract class organizationID : PX.Data.BQL.BqlInt.Field<organizationID> { }

		[Organization(false, Required = false)]
		public int? OrganizationID { get; set; }
		#endregion
		#region BranchID
		public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }

		[BranchOfOrganization(typeof(SchedulerScreenFilter.organizationID))]
		public int? BranchID { get; set; }
		#endregion

		#region OrgBAccountID
		public abstract class orgBAccountID : IBqlField { }

		[OrganizationTree(typeof(organizationID), typeof(branchID))]
		public int? OrgBAccountID { get; set; }
		#endregion

		#region BranchLocationID
		public abstract class branchLocationID : PX.Data.BQL.BqlInt.Field<branchLocationID> { }

		[PXUnboundDefault(typeof(
			Search<
				FSxUserPreferences.dfltBranchLocationID,
				Where<
				PX.SM.UserPreferences.userID, Equal<CurrentValue<AccessInfo.userID>>,
					And<PX.SM.UserPreferences.defBranchID, Equal<Current<SchedulerScreenFilter.branchID>>>>>))]
		[PXSelector(typeof(
			Search<
				FSBranchLocation.branchLocationID,
				Where<
				FSBranchLocation.branchID, Equal<Current<SchedulerScreenFilter.branchID>>>>),
			SubstituteKey = typeof(FSBranchLocation.branchLocationCD),
			DescriptionField = typeof(FSBranchLocation.descr))]
		[PXFormula(typeof(Default<SchedulerScreenFilter.branchID>))]
		public int? BranchLocationID { get; set; }
		#endregion

		#region BranchLocationCount
		public abstract class branchLocationCount : PX.Data.BQL.BqlInt.Field<branchLocationCount> { }
		[PXInt]
		//[PXFormula(typeof(Search4<
		//		FSBranchLocation.branchLocationID,
		//		Where<
		//		FSBranchLocation.branchID, Equal<Current<SchedulerScreenFilter.branchID>>>,
		//	Aggregate<ScalarCount<FSBranchLocation.branchLocationID>>>))]
		public int? BranchLocationCount { get; set; }
		#endregion
	}
}
