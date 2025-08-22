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
using PX.Objects.GL.DAC;
using PX.Objects.GL.Attributes;
using PX.Objects.CA;
using PX.Objects.CS;
using PX.Objects.GL.FinPeriods.TableDefinition;
using PX.Objects.GL.Descriptor;
using PX.Objects.GL.FinPeriods;
using PX.Data.BQL.Fluent;

namespace PX.Objects.Common.DAC.ReportParameters
{
	public class OrganizationBranchReportParameters : PXBqlTable, IBqlTable
	{
		#region OrganizationID
		public abstract class organizationID : PX.Data.BQL.BqlInt.Field<organizationID> { }

		[Organization(
			false,
			typeof(Search2<Organization.organizationID,
				InnerJoin<Branch,
					On<Organization.organizationID, Equal<Branch.organizationID>>>,
				Where<Branch.branchID, Equal<Current<AccessInfo.branchID>>,
					And2<FeatureInstalled<FeaturesSet.branch>,
					And<MatchWithBranch<Branch.branchID>>>>>))]
		public int? OrganizationID { get; set; }
		#endregion

		#region BranchID
		public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }

		[BranchOfOrganization(
			typeof(organizationID),
			onlyActive: false,
			sourceType: typeof(Search2<Branch.branchID,
				InnerJoin<Organization,
					On<Branch.organizationID, Equal<Organization.organizationID>>,
				CrossJoin<FeaturesSet>>,
				Where<FeaturesSet.branch, Equal<True>,
					And<Organization.organizationType, NotEqual<OrganizationTypes.withoutBranches>,
					And<Branch.branchID, Equal<Current<AccessInfo.branchID>>>>>>))]
		public int? BranchID { get; set; }
		#endregion

		#region OrgBAccountID
		public abstract class orgBAccountID : PX.Data.BQL.BqlInt.Field<orgBAccountID> { }

		[OrganizationTree(typeof(organizationID), typeof(branchID), onlyActive:false)]
		[PXUIRequired(typeof(FeatureInstalled<FeaturesSet.multipleBaseCurrencies>))]
		public int? OrgBAccountID { get; set; }
		#endregion

		#region LedgerID
		public abstract class ledgerID : PX.Data.BQL.BqlInt.Field<ledgerID> { }

		[LedgerOfOrganization(typeof(organizationID), typeof(branchID))]
		public virtual int? LedgerID { get; set; }
		#endregion

		#region NotBudgetLedgerID
		public abstract class notBudgetLedgerID : PX.Data.BQL.BqlInt.Field<notBudgetLedgerID> { }

		[LedgerOfOrganization(typeof(organizationID), typeof(branchID), typeof(Where<Ledger.balanceType, NotEqual<LedgerBalanceType.budget>>))]
		public virtual int? NotBudgetLedgerID { get; set; }
		#endregion

		#region BudgetLedgerID
		public abstract class budgetLedgerID : PX.Data.BQL.BqlInt.Field<budgetLedgerID> { }

		[LedgerOfOrganization(typeof(organizationID),
			typeof(branchID), null,
			typeof(Search<Ledger.ledgerID>),
			typeof(Where<Ledger.balanceType, Equal<LedgerBalanceType.budget>>))]
		public virtual int? BudgetLedgerID { get; set; }
		#endregion

		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.BQL.BqlString.Field<finPeriodID> { }

		[FinPeriodSelector(null,
			typeof(AccessInfo.businessDate),
		    branchSourceType: typeof(branchID),
		 	organizationSourceType: typeof(organizationID),
			takeBranchForSelectorFromQueryParams: true,
			takeOrganizationForSelectorFromQueryParams: true,
			useMasterOrganizationIDByDefault: true,
			masterPeriodBasedOnOrganizationPeriods: false)]
		public string FinPeriodID { get; set; }
		#endregion

		#region BranchFinPeriodID
		public abstract class branchFinPeriodID : IBqlField { }

		[FinPeriodSelector(null,
			typeof(AccessInfo.businessDate),
			typeof(branchID),
			takeBranchForSelectorFromQueryParams: true,
			useMasterOrganizationIDByDefault: true,
			masterPeriodBasedOnOrganizationPeriods: false)]
		public string BranchFinPeriodID { get; set; }
		#endregion

		#region FinPeriodIDByOrganization
		public abstract class finPeriodIDByOrganization : PX.Data.BQL.BqlString.Field<finPeriodIDByOrganization> { }

		[FinPeriodSelector(null,
			typeof(AccessInfo.businessDate),
			organizationSourceType: typeof(organizationID),
			takeOrganizationForSelectorFromQueryParams: true,
			useMasterOrganizationIDByDefault: true,
			masterPeriodBasedOnOrganizationPeriods: false)]
		public string FinPeriodIDByOrganization { get; set; }
		#endregion

		#region MasterFinPeriodID
		public abstract class masterFinPeriodID : PX.Data.BQL.BqlString.Field<masterFinPeriodID> { }

		[FinPeriodSelector(null,
			typeof(AccessInfo.businessDate),
			null,
			null,
			useMasterOrganizationIDByDefault: true,
			masterPeriodBasedOnOrganizationPeriods: false)]
		public string MasterFinPeriodID { get; set; }
		#endregion

		#region FinYear
		public abstract class finYear : PX.Data.BQL.BqlString.Field<finYear> { }

		[GenericFinYearSelector(null,
			typeof(AccessInfo.businessDate),
			typeof(branchID),
			typeof(organizationID),
			takeBranchForSelectorFromQueryParams: true,
			takeOrganizationForSelectorFromQueryParams: true,
			useMasterOrganizationIDByDefault: true)]
		public string FinYear { get; set; }
		#endregion

		#region StartYearPeriodID
		public abstract class startYearPeriodID : PX.Data.BQL.BqlString.Field<startYearPeriodID> { }

		[FinPeriodSelector(null,
			typeof(AccessInfo.businessDate),
			branchSourceType: typeof(branchID),
			organizationSourceType: typeof(organizationID),
			defaultType: typeof(Search2<
				FinPeriod.finPeriodID,
				InnerJoin<FinYear, On<FinPeriod.finYear, Equal<FinYear.year>,
					And<FinPeriod.organizationID, Equal<FinYear.organizationID>>>>,
				Where<FinYear.startDate, LessEqual<Current<AccessInfo.businessDate>>,
					And<FinYear.endDate, GreaterEqual<Current<AccessInfo.businessDate>>>>,
				OrderBy<
					Asc<FinPeriod.finPeriodID>>>),
			takeBranchForSelectorFromQueryParams: true,
			takeOrganizationForSelectorFromQueryParams: true,
			useMasterOrganizationIDByDefault: true,
			masterPeriodBasedOnOrganizationPeriods: false)]
		public string StartYearPeriodID { get; set; }
		#endregion

		#region EndYearPeriodID
		public abstract class endYearPeriodID : PX.Data.BQL.BqlString.Field<endYearPeriodID> { }

		[FinPeriodSelector(null,
			typeof(AccessInfo.businessDate),
			branchSourceType: typeof(branchID),
			organizationSourceType: typeof(organizationID),
			defaultType: typeof(Search2<
				FinPeriod.finPeriodID,
				InnerJoin<FinYear, On<FinPeriod.finYear, Equal<FinYear.year>,
					And<FinPeriod.startDate, NotEqual<FinPeriod.endDate>,
					And<FinPeriod.organizationID, Equal<FinYear.organizationID>>>>>,
				Where<FinYear.startDate, LessEqual<Current<AccessInfo.businessDate>>,
					And<FinYear.endDate, GreaterEqual<Current<AccessInfo.businessDate>>>>,
				OrderBy<
					Desc<FinPeriod.finPeriodID>>>),
			takeBranchForSelectorFromQueryParams: true,
			takeOrganizationForSelectorFromQueryParams: true,
			useMasterOrganizationIDByDefault: true,
			masterPeriodBasedOnOrganizationPeriods: false)]
		public string EndYearPeriodID { get; set; }
		#endregion

		#region UseMasterCalendar
		public abstract class useMasterCalendar : IBqlField { }

		[PXBool]
		[PXUIField(DisplayName = Messages.UseMasterCalendar)]
		[PXUIVisible(typeof(FeatureInstalled<FeaturesSet.multipleCalendarsSupport>))]
		public bool? UseMasterCalendar { get; set; }
		#endregion

		#region CashAccountPeriodID
		public abstract class cashAccountPeriodID : IBqlField { }

		[FinPeriodSelector(typeof(Search2<OrganizationFinPeriod.finPeriodID,
			InnerJoin<Branch, On<Branch.organizationID, Equal<FinPeriod.organizationID>>,
			InnerJoin<CashAccount, On<CashAccount.branchID, Equal<Branch.branchID>>>>,
			Where<CashAccount.cashAccountCD, Equal<Optional<CashAccount.cashAccountCD>>>>),
			null)]
		public string CashAccountPeriodID { get; set; }
		#endregion

		#region LedgerIDByBAccount
		public abstract class ledgerIDByBAccount : PX.Data.BQL.BqlInt.Field<ledgerIDByBAccount> { }

		[PXDBInt]
		[PXUIField(DisplayName = GL.Messages.Ledger)]
		[PXSelector(
			typeof(SearchFor<Ledger.ledgerID>
			.In<SelectFrom<Ledger>
				.LeftJoin<Branch>
					.On<Ledger.ledgerID.IsEqual<Branch.ledgerID>>
				.Where<Branch.branchID.IsNull
					.Or<Where<Branch.branchID, Inside<Optional<orgBAccountID>>>>>
				.AggregateTo<GroupBy<Ledger.ledgerID>>>),
			SubstituteKey = typeof(Ledger.ledgerCD))]
		public virtual int? LedgerIDByBAccount { get; set; }
		#endregion

		#region BudgetLedgerIDByBAccount
		public abstract class budgetLedgerIDByBAccount : PX.Data.BQL.BqlInt.Field<budgetLedgerIDByBAccount> { }

		[PXDBInt]
		[PXUIField(DisplayName = GL.Messages.Ledger)]
		[PXSelector(
			typeof(SearchFor<Ledger.ledgerID>
			.In<SelectFrom<Ledger>
				.LeftJoin<Branch>
					.On<Ledger.ledgerID.IsEqual<Branch.ledgerID>>
				.Where<Ledger.balanceType.IsEqual<LedgerBalanceType.budget>
					.And<Branch.branchID.IsNull
						.Or<Where<Branch.branchID, Inside<Optional<orgBAccountID>>>>>>
				.AggregateTo<GroupBy<Ledger.ledgerID>>>),
			SubstituteKey = typeof(Ledger.ledgerCD))]
		public virtual int? BudgetLedgerIDByBAccount { get; set; }
		#endregion

		#region NotBudgetLedgerIDByBAccount
		public abstract class notBudgetLedgerIDByBAccount : PX.Data.BQL.BqlInt.Field<notBudgetLedgerIDByBAccount> { }

		[PXDBInt]
		[PXUIField(DisplayName = GL.Messages.Ledger)]
		[PXSelector(
			typeof(SearchFor<Ledger.ledgerID>
			.In<SelectFrom<Ledger>
				.LeftJoin<Branch>
					.On<Ledger.ledgerID.IsEqual<Branch.ledgerID>>
				.Where<Ledger.balanceType.IsNotEqual<LedgerBalanceType.budget>
					.And<Branch.branchID.IsNull
						.Or<Where<Branch.branchID, Inside<Optional<orgBAccountID>>>>
						.Or<orgBAccountID.AsOptional.IsNull>>>
				.AggregateTo<GroupBy<Ledger.ledgerID>>>),
			SubstituteKey = typeof(Ledger.ledgerCD))]
		public virtual int? NotBudgetLedgerIDByBAccount { get; set; }
		#endregion

		#region FinPeriodIDByBAccont
		public abstract class finPeriodIDByBAccount : PX.Data.BQL.BqlString.Field<finPeriodIDByBAccount> { }

		[FinPeriodSelector(
			typeof(Search<OrganizationFinPeriod.finPeriodID,
				Where<Where2<Where<FinPeriod.organizationID, Suit<Optional2<orgBAccountID>>, And<IsNull<Optional2<OrganizationBranchReportParameters.useMasterCalendar>, False>, NotEqual<True>>>,
					Or<Where<FinPeriod.organizationID, Equal<FinPeriod.organizationID.masterValue>, And<Optional2<OrganizationBranchReportParameters.useMasterCalendar>, Equal<True>>>>>>>),
			null)]
		public string FinPeriodIDByBAccount { get; set; }
		#endregion

		#region EndFinPeriodIDByBAccountInSameYear
		public abstract class endFinPeriodIDByBAccountInSameYear : PX.Data.BQL.BqlString.Field<endFinPeriodIDByBAccountInSameYear> { }

		/// <summary>
		/// The last financial period of the date range.
		/// </summary>
		[FinPeriodSelector(
			typeof(Search<OrganizationFinPeriod.finPeriodID,
				Where<Where2<Where2<Where<FinPeriod.organizationID, Suit<Optional2<orgBAccountID>>, And<IsNull<Optional2<OrganizationBranchReportParameters.useMasterCalendar>, False>, NotEqual<True>>>,
							Or<Where<FinPeriod.organizationID, Equal<FinPeriod.organizationID.masterValue>, And<Optional2<OrganizationBranchReportParameters.useMasterCalendar>, Equal<True>>>>>,
						And<FinPeriod.finPeriodID, StartsWith<Substring<Optional2<OrganizationBranchReportParameters.finPeriodIDByBAccount>, int1, int4>>,
							And<FinPeriod.finPeriodID, GreaterEqual<Optional2<OrganizationBranchReportParameters.finPeriodIDByBAccount>>>>>>>),
			null)]
		public string EndFinPeriodIDByBAccountInSameYear { get; set; }
		#endregion

		#region OrgMatchBAccountID
		public abstract class orgMatchBAccountID : PX.Data.BQL.BqlInt.Field<orgMatchBAccountID> { }
		
		[MatchOrganization]
		public int? OrgMatchBAccountID { get; set; }
		#endregion

		#region AccountID
		public abstract class accountID : PX.Data.BQL.BqlInt.Field<accountID> { }
		protected int? _AccountID;
		[AccountAny(null, typeof(Search2<Account.accountID,
			LeftJoin<Branch, 
				  On<Branch.bAccountID, Equal<Optional2<OrganizationBranchReportParameters.orgBAccountID>>>>,
			Where2<Where<Branch.branchID, IsNull,
				Or<Match<Optional<OrganizationBranchReportParameters.orgMatchBAccountID>>>>,
			And<Match<Current<AccessInfo.userName>>>>>),
			DescriptionField = typeof(Account.description))]
		[PXDefault]
		public virtual int? AccountID
		{
			get
			{
				return this._AccountID;
			}
			set
			{
				this._AccountID = value;
			}
		}
		#endregion

		#region SubID
		public abstract class subID : PX.Data.BQL.BqlInt.Field<subID> { }
		protected int? _SubID;
		[SubAccount(typeof(Search2<Sub.subID,
			LeftJoin<Branch,
				  On<Branch.bAccountID, Equal<Optional2<OrganizationBranchReportParameters.orgBAccountID>>>>,
			Where2<Where<Branch.branchID, IsNull,
				Or<Match<Optional<OrganizationBranchReportParameters.orgMatchBAccountID>>>>,
			And<Match<Current<AccessInfo.userName>>>>>),
			null, null, DescriptionField = typeof(Sub.description))]
		[PXDefault]
		public virtual int? SubID
		{
			get
			{
				return this._SubID;
			}
			set
			{
				this._SubID = value;
			}
		}
		#endregion

		#region CashAccountID
		public abstract class cashAccountID : PX.Data.BQL.BqlInt.Field<cashAccountID> { }
		[CashAccount(typeof(Search<CashAccount.cashAccountID, Where<CashAccount.branchID, Inside<Optional2<orgBAccountID>>, And<CashAccount.restrictVisibilityWithBranch, Equal<True>,
			Or<CashAccount.restrictVisibilityWithBranch, NotEqual<True>>>>>))]
		public virtual int? CashAccountID { get; set; }
		#endregion

		#region PrepaymentInvoiceFullName
		public abstract class prepaymentInvoiceFullName : PX.Data.BQL.BqlInt.Field<prepaymentInvoiceFullName> { }

		/// <summary>
		/// A full name for the document of the prepayment invoice type
		/// that is used only on the Invoice/Memo(AR641000) report.
		/// The documents of the prepayment invoice type are a part of the <see cref= "FeaturesSet.VATRecognitionOnPrepayments"/> feature.
		/// </summary>
		/// <value>
		/// Long equivalent of the<see cref="PX.Objects.AR.ARDocType.PrintListAttribute"/> for the prepayment invoice document type.
		/// </value>
		[PXDefault(AR.Messages.PrintPrepaymentInvoiceFullName)]
		public string PrepaymentInvoiceFullName { get; set; }
		#endregion
	}
}
