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
using PX.Data.BQL.Fluent;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.GL;
using System;
using System.Diagnostics;

namespace PX.Objects.PR
{
	/// <summary>
	/// Includes the settings of the paid time off (PTO).
	/// </summary>
	[Serializable]
	[PXCacheName(Messages.PRPTOBank)]
	[DebuggerDisplay("{GetType().Name,nq}: BankID = {BankID,nq}, StartDate = {StartDate,nq}")]
	[PXPrimaryGraph(typeof(PTOBankMaint))]
	public class PRPTOBank : PXBqlTable, IBqlTable
	{
		#region Keys
		/// <summary>
		/// Primary Key
		/// </summary>
		public class PK : PrimaryKeyOf<PRPTOBank>.By<bankID>
		{
			public static PRPTOBank Find(PXGraph graph, string bankID, PKFindOptions options = PKFindOptions.None) => FindBy(graph, bankID, options);
		}

		/// <summary>
		/// Foreign Keys
		/// </summary>
		public static class FK
		{
			/// <summary>
			/// Disbursing Earning Type
			/// </summary>
			public class DisbursingEarningType : EPEarningType.PK.ForeignKeyOf<PRPTOBank>.By<earningTypeCD> { }
			/// <summary>
			/// PTO Expense Account
			/// </summary>
			public class PTOExpenseAccount : Account.PK.ForeignKeyOf<PRPTOBank>.By<ptoExpenseAcctID> { }
			/// <summary>
			/// PTO Expense Sub. account
			/// </summary>
			public class PTOExpenseSubaccount : Sub.PK.ForeignKeyOf<PRPTOBank>.By<ptoExpenseSubID> { }
			/// <summary>
			/// PTO Liability Account
			/// </summary>
			public class PTOLiabilityAccount : Account.PK.ForeignKeyOf<PRPTOBank>.By<ptoLiabilityAcctID> { }
			/// <summary>
			/// PTO Liability Sub. account
			/// </summary>
			public class PTOLiabilitySubaccount : Sub.PK.ForeignKeyOf<PRPTOBank>.By<ptoLiabilitySubID> { }
			/// <summary>
			/// PTO Asset Account
			/// </summary>
			public class PTOAssetAccount : Account.PK.ForeignKeyOf<PRPTOBank>.By<ptoAssetAcctID> { }
			/// <summary>
			/// PTO Asset Sub. account
			/// </summary>
			public class PTOAssetSubaccount : Sub.PK.ForeignKeyOf<PRPTOBank>.By<ptoAssetSubID> { }
		}
		#endregion

		#region BankID
		/// <summary>
		/// The unique identifier of a PTO bank to be used for the paid time off calculation.
		/// </summary>
		[PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXUIField(DisplayName = "Bank ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault]
		[PXSelector(typeof(SearchFor<PRPTOBank.bankID>), DescriptionField = typeof(PRPTOBank.description))]
		[PXReferentialIntegrityCheck]
		public virtual string BankID { get; set; }
		public abstract class bankID : PX.Data.BQL.BqlString.Field<bankID> { }
		#endregion

		#region Description
		/// <summary>
		/// The description.
		/// </summary>
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		[PTOBanksEditRestriction]
		public virtual string Description { get; set; }
		public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
		#endregion

		#region AccrualMethod
		/// <summary>
		/// The method of PTO hours accrual that defines whether PTO hours should be calculated 
		/// as a percentage or a specific number should be used for every pay period.
		/// </summary>
		/// <value>
		/// The field can have one of the values described in <see cref="PTOAccrualMethod.ListAttribute"/>.
		/// </value>
		[PXDBString(3, IsFixed = true)]
		[PXUIField(DisplayName = "Accrual Method")]
		[PXDefault(typeof(PTOAccrualMethod.percentage))]
		[PTOAccrualMethod.List]
		[PTOBanksEditRestriction]
		public virtual string AccrualMethod { get; set; }
		public abstract class accrualMethod : PX.Data.BQL.BqlString.Field<accrualMethod> { }
		#endregion

		#region EarningTypeCD
		/// <summary>
		/// The user-friendly unique identifier of the earning type used for dispersal on a paycheck.
		/// The field is included in <see cref="FK.DisbursingEarningType"/>.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="EPEarningType.typeCD"/> field.
		/// </value>
		[PXDBString(EPEarningType.typeCD.Length, IsUnicode = true, InputMask = EPEarningType.typeCD.InputMask)]
		[PXUIField(DisplayName = "Disbursing Earning Type")]
		[PXSelector(typeof(SearchFor<EPEarningType.typeCD>.
			Where<EPEarningType.isActive.IsEqual<True>.
				And<EPEarningType.typeCD.IsNotInSubselect<SearchFor<PRPTOBank.earningTypeCD>.Where<bankID.FromCurrent.IsNull.Or<PRPTOBank.bankID.IsNotEqual<bankID.FromCurrent>>>>>.
				And<PREarningType.isPTO.IsEqual<True>>>), DescriptionField = typeof(EPEarningType.description))]
		[PXDefault]
		[PXCheckUnique(ErrorMessage = Messages.DuplicateEarningType)]
		[PXForeignReference(typeof(Field<earningTypeCD>.IsRelatedTo<EPEarningType.typeCD>))]
		[PTOBanksEditRestriction]
		public virtual string EarningTypeCD { get; set; }
		public abstract class earningTypeCD : PX.Data.BQL.BqlString.Field<earningTypeCD> { }
		#endregion

		#region IsActive
		/// <summary>
		/// Indicates (if set to <see langword="true" />) that the PTO bank should be accruing during the paycheck process.
		/// </summary>
		[PXDBBool]
		[PXUIField(DisplayName = "Active")]
		[PXDefault(true)]
		public virtual bool? IsActive { get; set; } //ToDo AC-149516: Check that the Earning Type is still correct when the PTOBank is re-activated.
		public abstract class isActive : PX.Data.BQL.BqlBool.Field<isActive> { }
		#endregion

		#region ApplyBandingRules
		/// <summary>
		/// Indicates (if set to <see langword="true" />) that banding rules will be applied.
		/// </summary>
		[PXDBBool]
		[PXUIField(DisplayName = "Apply Banding Rules")]
		[PXDefault(false)]
		public virtual bool? ApplyBandingRules { get; set; }
		public abstract class applyBandingRules : PX.Data.BQL.BqlBool.Field<applyBandingRules> { }
		#endregion

		#region IsCertifiedJobAccrual
		/// <summary>
		/// Indicates (if set to <see langword="true" />) that hours are accumulated only for the earning lines with
		/// the selected Certified check box in the released paycheck.
		/// </summary>
		[PXDBBool]
		[PXUIField(DisplayName = "Accrue Only on Certified Job")]
		[PXDefault(false)]
		public virtual bool? IsCertifiedJobAccrual { get; set; }
		public abstract class isCertifiedJobAccrual : PX.Data.BQL.BqlBool.Field<isCertifiedJobAccrual> { }
		#endregion

		#region StartDate
		/// <summary>
		/// The date at which the system adds the front loading number of hours to an employee PTO bank. 
		/// You specify the number of hours in the Front Loading Amount box on the General Settings tab.
		/// </summary>
		[PXDBDate]
		[PXUIField(DisplayName = "Transfer Date", Required = true)]
		[PXDefault]
		[PXUIVisible(typeof(Where<transferDateType.IsEqual<TransferDateType.specificDate>>))]
		public virtual DateTime? StartDate { get; set; }
		public abstract class startDate : PX.Data.BQL.BqlDateTime.Field<startDate> { }
		#endregion

		#region StartDateMonth
		/// <summary>
		/// The month of the date at which the system adds the front loading number of hours to an employee PTO bank. 
		/// </summary>
		[PXInt]
		[PXUIField(DisplayName = "Start Date")]
		[Month.List]
		[PXUnboundDefault(typeof(Switch<Case<Where<startDate, IsNotNull>, DatePart<DatePart.month, startDate>>, int1>))]
		[PXUIVisible(typeof(Where<transferDateType.IsEqual<TransferDateType.specificDate>>))]
		[PTOBanksEditRestriction]
		public virtual int? StartDateMonth { get; set; }
		public abstract class startDateMonth : PX.Data.BQL.BqlInt.Field<startDateMonth> { }
		#endregion

		#region StartDateDay
		/// <summary>
		/// The day of the date at which the system adds the front loading number of hours to an employee PTO bank.
		/// </summary>
		[PXInt(MinValue = 1, MaxValue = 31)]
		[PXUIField(DisplayName = "Start Date")]
		[PXUnboundDefault(typeof(Switch<Case<Where<startDate, IsNotNull>, DatePart<DatePart.day, startDate>>, int1>))]
		[PXUIVisible(typeof(Where<transferDateType.IsEqual<TransferDateType.specificDate>>))]
		[PTOBanksEditRestriction]
		public virtual int? StartDateDay { get; set; }
		public abstract class startDateDay : PX.Data.BQL.BqlInt.Field<startDateDay> { }
		#endregion

		#region TransferDateType
		/// <summary>
		/// The transfer date type of the PTO bank.
		/// </summary>
		/// <value>
		/// The field can have one of the values described in <see cref="TransferDateType.List"/>.
		/// </value>
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Transfer Date Type")]
		[PXDefault(typeof(TransferDateType.specificDate))]
		[TransferDateType.List]
		[PTOBanksEditRestriction]
		public virtual string TransferDateType { get; set; }
		public abstract class transferDateType : PX.Data.BQL.BqlString.Field<transferDateType> { }
		#endregion

		#region PTOYearStartDate
		/// <summary>
		/// The year of the PTO start date.
		/// </summary>
		[PXDate]
		public virtual DateTime? PTOYearStartDate { get => StartDate; set => StartDate = value; }
		public abstract class pTOYearStartDate : PX.Data.BQL.BqlDateTime.Field<pTOYearStartDate> { }
		#endregion

		#region CarryoverType
		/// <summary>
		/// The way accruals are to be carried over from year to year starting the date specified in the Start Date box.
		/// </summary>
		/// <value>
		/// The field can have one of the values described in <see cref="CarryoverType.ListAttribute"/>.
		/// </value>
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Carryover Type")]
		[PXDefault(typeof(CarryoverType.none))]
		[CarryoverType.List]
		[PTOBanksEditRestriction]
		public virtual string CarryoverType { get; set; }
		public abstract class carryoverType : PX.Data.BQL.BqlString.Field<carryoverType> { }
		#endregion

		#region PTOExpenseAcctID
		/// <summary>
		/// The expense account.
		/// The field is included in <see cref="FK.PTOExpenseAccount"/>.
		/// </summary>
		public abstract class ptoExpenseAcctID : PX.Data.BQL.BqlInt.Field<ptoExpenseAcctID> { }
		[Account(DisplayName = "Expense Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description), AvoidControlAccounts = true, FieldClass = nameof(FeaturesSet.PayrollCAN))]
		[PXForeignReference(typeof(FK.PTOExpenseAccount))]
		[PRPTOExpenseAccountRequired(GLAccountSubSource.PTOBank, typeof(Where<createFinancialTransaction.IsEqual<True>.And<FeatureInstalled<FeaturesSet.payrollCAN>>>))]
		[PXUIVisible(typeof(Where<createFinancialTransaction.IsEqual<True>>))]
		public virtual Int32? PTOExpenseAcctID { get; set; }
		#endregion

		#region PTOExpenseSubID
		/// <summary>
		/// The expense sub. ID.
		/// The field is included in <see cref="FK.PTOExpenseSubaccount"/>.
		/// </summary>
		public abstract class ptoExpenseSubID : PX.Data.BQL.BqlInt.Field<ptoExpenseSubID> { }
		[SubAccount(typeof(ptoExpenseAcctID), DisplayName = "Expense Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description), FieldClass = nameof(FeaturesSet.PayrollCAN))]
		[PXForeignReference(typeof(FK.PTOExpenseSubaccount))]
		[PRPTOExpenseSubRequired(GLAccountSubSource.PTOBank, typeof(Where<createFinancialTransaction.IsEqual<True>.And<FeatureInstalled<FeaturesSet.payrollCAN>>>))]
		[PXUIVisible(typeof(Where<createFinancialTransaction.IsEqual<True>.And<FeatureInstalled<FeaturesSet.subAccount>>>))]
		public virtual Int32? PTOExpenseSubID { get; set; }
		#endregion

		#region PTOLiabilityAcctID
		/// <summary>
		/// The liability account.
		/// The field is included in <see cref="FK.PTOLiabilityAccount"/>.
		/// </summary>
		public abstract class ptoLiabilityAcctID : PX.Data.BQL.BqlInt.Field<ptoLiabilityAcctID> { }
		[Account(DisplayName = "Liability Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description), FieldClass = nameof(FeaturesSet.PayrollCAN))]
		[PXForeignReference(typeof(FK.PTOLiabilityAccount))]
		[PRPTOLiabilityAccountRequired(GLAccountSubSource.PTOBank, typeof(Where<createFinancialTransaction.IsEqual<True>.And<FeatureInstalled<FeaturesSet.payrollCAN>>>))]
		[PXUIVisible(typeof(Where<createFinancialTransaction.IsEqual<True>>))]
		public virtual Int32? PTOLiabilityAcctID { get; set; }
		#endregion

		#region PTOLiabilitySubID
		/// <summary>
		/// The liability sub. ID.
		/// The field is included in <see cref="FK.PTOLiabilitySubaccount"/>.
		/// </summary>
		public abstract class ptoLiabilitySubID : PX.Data.BQL.BqlInt.Field<ptoLiabilitySubID> { }
		[SubAccount(typeof(ptoLiabilityAcctID), DisplayName = "Liability Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description), FieldClass = nameof(FeaturesSet.PayrollCAN))]
		[PXForeignReference(typeof(FK.PTOLiabilitySubaccount))]
		[PRPTOLiabilitySubRequired(GLAccountSubSource.PTOBank, typeof(Where<createFinancialTransaction.IsEqual<True>.And<FeatureInstalled<FeaturesSet.payrollCAN>>>))]
		[PXUIVisible(typeof(Where<createFinancialTransaction.IsEqual<True>.And<FeatureInstalled<FeaturesSet.subAccount>>>))]
		public virtual Int32? PTOLiabilitySubID { get; set; }
		#endregion

		#region PTOAssetAcctID
		/// <summary>
		/// The asset account ID.
		/// The field is included in <see cref="FK.PTOAssetAccount"/>.
		/// </summary>
		public abstract class ptoAssetAcctID : PX.Data.BQL.BqlInt.Field<ptoAssetAcctID> { }
		[Account(DisplayName = "Asset Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description), FieldClass = nameof(FeaturesSet.PayrollCAN))]
		[PXForeignReference(typeof(FK.PTOAssetAccount))]
		[PRPTOAssetAccountRequired(GLAccountSubSource.PTOBank, typeof(Where<createFinancialTransaction.IsEqual<True>.And<FeatureInstalled<FeaturesSet.payrollCAN>>>))]
		[PXUIVisible(typeof(Where<createFinancialTransaction.IsEqual<True>>))]
		public virtual Int32? PTOAssetAcctID { get; set; }
		#endregion

		#region PTOAssetSubID
		/// <summary>
		/// The asset sub. ID.
		/// The field is included in <see cref="FK.PTOAssetSubaccount"/>.
		/// </summary>
		public abstract class ptoAssetSubID : PX.Data.BQL.BqlInt.Field<ptoAssetSubID> { }
		[SubAccount(typeof(ptoAssetAcctID), DisplayName = "Asset Sub.", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description), FieldClass = nameof(FeaturesSet.PayrollCAN))]
		[PXForeignReference(typeof(FK.PTOAssetSubaccount))]
		[PRPTOAssetSubRequired(GLAccountSubSource.PTOBank, typeof(Where<createFinancialTransaction.IsEqual<True>.And<FeatureInstalled<FeaturesSet.payrollCAN>>>))]
		[PXUIVisible(typeof(Where<createFinancialTransaction.IsEqual<True>.And<FeatureInstalled<FeaturesSet.subAccount>>>))]
		public virtual Int32? PTOAssetSubID { get; set; }
		#endregion

		#region CreateFinancialTransaction
		/// <summary>
		/// Enable the money calculation and the creation of general ledger transaction for paid time off on Paychecks and Adjustments
		/// </summary>
		[PXDBBool]
		[PXUIField(DisplayName = "Create GL Transactions on Accrual", FieldClass = nameof(FeaturesSet.PayrollCAN))]
		[PXDefault(false)]
		public virtual bool? CreateFinancialTransaction { get; set; }
		public abstract class createFinancialTransaction : PX.Data.BQL.BqlBool.Field<createFinancialTransaction> { }
		#endregion

		#region DisbursingType
		/// <summary>
		/// The disbursing type.
		/// </summary>
		/// <value>
		/// The field can have one of the values described in <see cref="PTODisbursingType.ListAttribute"/>.
		/// </value>
		[PXDBString(1, IsFixed = true)]
		[PXDefault(PTODisbursingType.CurrentRate)]
		[PXUIField(DisplayName = "Default Disbursing Type", FieldClass = nameof(FeaturesSet.PayrollCAN))]
		[PXUIVisible(typeof(Where<createFinancialTransaction.IsEqual<True>>))]
		[PTODisbursingType.List]
		[PTOBanksEditRestriction]
		public virtual string DisbursingType { get; set; }
		public abstract class disbursingType : PX.Data.BQL.BqlString.Field<disbursingType> { }
		#endregion

		#region SettlementBalanceType
		/// <summary>
		/// The rule that will be applied to the PTO bank when a final paycheck is 
		/// calculated for an employee who is assigned this PTO bank.
		/// </summary>
		/// <value>
		/// The field can have one of the values described in <see cref="SettlementBalanceType.ListAttribute"/>.
		/// </value>
		[PXDBString(3, IsFixed = true)]
		[PXUIField(DisplayName = "On Settlement")]
		[PXDefault(typeof(SettlementBalanceType.pay))]
		[SettlementBalanceType.List]
		[PTOBanksEditRestriction]
		public virtual string SettlementBalanceType { get; set; }
		public abstract class settlementBalanceType : PX.Data.BQL.BqlString.Field<settlementBalanceType> { }
		#endregion

		#region BandingRuleRoundingMethod
		/// <summary>
		/// The rounding method for Years of Service.
		/// </summary>
		/// /// <value>
		/// The field can have one of the values described in <see cref="BandingRuleRoundingMethod.ListAttribute"/>.
		/// </value>
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Rounding Method for Years of Service")]
		[PXUIVisible(typeof(Where<accrualMethod.IsEqual<PTOAccrualMethod.frontLoading>.And<transferDateType.IsNotEqual<TransferDateType.anniversary>>>))]
		[PXDefault(typeof(BandingRuleRoundingMethod.roundUp))]
		[BandingRuleRoundingMethod.List]
		[PTOBanksEditRestriction]
		public virtual string BandingRuleRoundingMethod { get; set; }
		public abstract class bandingRuleRoundingMethod : PX.Data.BQL.BqlString.Field<bandingRuleRoundingMethod> { }
		#endregion

		#region AllowNegativeBalance
		/// <summary>
		/// Indicates (if set to <see langword="true" />) that the system does not put restrictions on the disbursing amount.
		/// </summary>
		[Obsolete(Common.Messages.ItemIsObsoleteAndWillBeRemoved2024R2)]
		[PXDBBool]
		[PXUIField(DisplayName = "Allow Negative Balance", Visible = false)]
		[PXDefault(true)]
		public virtual bool? AllowNegativeBalance { get; set; }
		public abstract class allowNegativeBalance : PX.Data.BQL.BqlBool.Field<allowNegativeBalance> { }
		#endregion

		#region DisburseFromCarryover
		/// <summary>
		/// Indicates (if set to <see langword="true" />) that you can only use the carryover hours from the previous year.
		/// </summary>
		[Obsolete(Common.Messages.ItemIsObsoleteAndWillBeRemoved2024R2)]
		[PXDBBool]
		[PXUIField(DisplayName = "Can Only Disburse from Carryover", Visible = false)]
		[PXDefault(false)]
		public virtual bool? DisburseFromCarryover { get; set; }
		public abstract class disburseFromCarryover : PX.Data.BQL.BqlBool.Field<disburseFromCarryover> { }
		#endregion

		#region System Columns
		#region TStamp
		public abstract class tStamp : PX.Data.BQL.BqlByteArray.Field<tStamp> { }
		[PXDBTimestamp]
		public byte[] TStamp { get; set; }
		#endregion
		#region CreatedByID
		public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
		[PXDBCreatedByID]
		public Guid? CreatedByID { get; set; }
		#endregion
		#region CreatedByScreenID
		public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
		[PXDBCreatedByScreenID]
		public string CreatedByScreenID { get; set; }
		#endregion
		#region CreatedDateTime
		public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
		[PXDBCreatedDateTime]
		public DateTime? CreatedDateTime { get; set; }
		#endregion
		#region LastModifiedByID
		public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
		[PXDBLastModifiedByID]
		public Guid? LastModifiedByID { get; set; }
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
		[PXDBLastModifiedByScreenID]
		public string LastModifiedByScreenID { get; set; }
		#endregion
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
		[PXDBLastModifiedDateTime]
		public DateTime? LastModifiedDateTime { get; set; }
		#endregion
		#endregion
	}
}
