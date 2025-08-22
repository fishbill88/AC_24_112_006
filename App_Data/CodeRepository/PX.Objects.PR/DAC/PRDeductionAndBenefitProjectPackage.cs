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
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Payroll.Data;
using System;

namespace PX.Objects.PR
{
	/// <summary>
	/// A deduction or benefit associated with a certified project.
	/// </summary>
	[PXCacheName(Messages.PRDeductionAndBenefitProjectPackage)]
	[Serializable]
	public class PRDeductionAndBenefitProjectPackage : PXBqlTable, IBqlTable
	{
		#region Keys
		public class PK : PrimaryKeyOf<PRDeductionAndBenefitProjectPackage>.By<recordID>
		{
			public static PRDeductionAndBenefitProjectPackage Find(PXGraph graph, int? recordID, PKFindOptions options = PKFindOptions.None) => FindBy(graph, recordID, options);
		}

		public static class FK
		{
			public class Project : PMProject.PK.ForeignKeyOf<PRDeductionAndBenefitProjectPackage>.By<projectID> { }
			public class DeductionCode : PRDeductCode.PK.ForeignKeyOf<PRDeductionAndBenefitProjectPackage>.By<deductionAndBenefitCodeID> { }
			public class LaborItem : InventoryItem.PK.ForeignKeyOf<PRDeductionAndBenefitProjectPackage>.By<laborItemID> { }
		}
		#endregion

		#region RecordID
		public abstract class recordID : BqlInt.Field<recordID> { }
		[PXDBIdentity(IsKey = true)]
		public virtual int? RecordID { get; set; }
		#endregion
		#region ProjectID
		public abstract class projectID : BqlInt.Field<projectID> { }
		/// <summary>
		/// The unique identifier of the certified project.
		/// The field is included in <see cref="FK.Project"/>.
		/// </summary>
		[PXDBInt]
		[PXDBDefault(typeof(PMProject.contractID))]
		[PXParent(typeof(FK.Project))]
		public int? ProjectID { get; set; }
		#endregion
		#region DeductionAndBenefitCodeID
		public abstract class deductionAndBenefitCodeID : BqlInt.Field<deductionAndBenefitCodeID> { }
		/// <summary>
		/// The unique identifier of the deduction or benefit code.
		/// The field is included in <see cref="FK.DeductionCode"/>.
		/// </summary>
		[PXDBInt]
		[PXDefault]
		[PXUIField(DisplayName = "Deduction and Benefit Code")]
		[DeductionActiveSelector(
			typeof(Where<PRDeductCode.isCertifiedProject.IsEqual<True>>),
			typeof(countryUS))]
		[PXRestrictor(
			typeof(Where<Brackets<PRDeductCode.contribType.IsEqual<ContributionTypeListAttribute.employerContribution>
					.Or<PRDeductCode.dedCalcType.IsNotEqual<DedCntCalculationMethod.percentOfNet>>>
				.And<PRDeductCode.contribType.IsEqual<ContributionTypeListAttribute.employeeDeduction>
					.Or<PRDeductCode.cntCalcType.IsNotEqual<DedCntCalculationMethod.percentOfNet>>>>),
			Messages.PercentOfNetInCertifiedProject)]
		[DedAndBenPackageEventSubscriber(typeof(deductionAmount), typeof(deductionRate), typeof(benefitAmount), typeof(benefitRate), typeof(effectiveDate))]
		[PXForeignReference(typeof(Field<deductionAndBenefitCodeID>.IsRelatedTo<PRDeductCode.codeID>))]
		public int? DeductionAndBenefitCodeID { get; set; }
		#endregion
		#region DeductionAmount
		public abstract class deductionAmount : BqlDecimal.Field<deductionAmount> { }
		/// <summary>
		/// The deduction amount to be used if the calculation method is set to either <see cref="DedCntCalculationMethod.FixedAmount"/> or <see cref="DedCntCalculationMethod.AmountPerHour"/>.
		/// </summary>
		[PXDBDecimal(MinValue = 0)]
		[DefaultDedBenValue(
			typeof(deductionAndBenefitCodeID),
			typeof(PRDeductCode.dedCalcType),
			new string[] { DedCntCalculationMethod.FixedAmount, DedCntCalculationMethod.AmountPerHour },
			typeof(PRDeductCode.contribType),
			new string[] { ContributionType.EmployeeDeduction, ContributionType.BothDeductionAndContribution },
			typeof(PRDeductCode.dedAmount))]
		[PXUIField(DisplayName = "Deduction Amount")]
		public decimal? DeductionAmount { get; set; }
		#endregion
		#region DeductionRate
		public abstract class deductionRate : BqlDecimal.Field<deductionRate> { }
		/// <summary>
		/// The deduction percentage to be used if the calculation method is set to either <see cref="DedCntCalculationMethod.PercentOfGross"/> or <see cref="DedCntCalculationMethod.PercentOfNet"/>.
		/// </summary>
		[PXDBDecimal(MinValue = 0)]
		[DefaultDedBenValue(
			typeof(deductionAndBenefitCodeID),
			typeof(PRDeductCode.dedCalcType),
			new string[] { DedCntCalculationMethod.PercentOfGross, DedCntCalculationMethod.PercentOfCustom },
			typeof(PRDeductCode.contribType),
			new string[] { ContributionType.EmployeeDeduction, ContributionType.BothDeductionAndContribution },
			typeof(PRDeductCode.dedPercent))]
		[PXUIField(DisplayName = "Deduction Percent")]
		public decimal? DeductionRate { get; set; }
		#endregion
		#region BenefitAmount
		public abstract class benefitAmount : BqlDecimal.Field<benefitAmount> { }
		/// <summary>
		/// The contribution amount to be used if the calculation method is set to either <see cref="DedCntCalculationMethod.FixedAmount"/> or <see cref="DedCntCalculationMethod.AmountPerHour"/>.
		/// </summary>
		[PXDBDecimal(MinValue = 0)]
		[DefaultDedBenValue(
			typeof(deductionAndBenefitCodeID),
			typeof(PRDeductCode.cntCalcType),
			new string[] { DedCntCalculationMethod.FixedAmount, DedCntCalculationMethod.AmountPerHour },
			typeof(PRDeductCode.contribType),
			new string[] { ContributionType.EmployerContribution, ContributionType.BothDeductionAndContribution },
			typeof(PRDeductCode.cntAmount))]
		[PXUIField(DisplayName = "Contribution Amount")]
		public decimal? BenefitAmount { get; set; }
		#endregion
		#region BenefitRate
		public abstract class benefitRate : BqlDecimal.Field<benefitRate> { }
		/// <summary>
		/// The contribution percentage to be used if the calculation method is set to either <see cref="DedCntCalculationMethod.PercentOfGross"/> or <see cref="DedCntCalculationMethod.PercentOfNet"/>.
		/// </summary>
		[PXDBDecimal(MinValue = 0)]
		[DefaultDedBenValue(
			typeof(deductionAndBenefitCodeID),
			typeof(PRDeductCode.cntCalcType),
			new string[] { DedCntCalculationMethod.PercentOfGross, DedCntCalculationMethod.PercentOfCustom },
			typeof(PRDeductCode.contribType),
			new string[] { ContributionType.EmployerContribution, ContributionType.BothDeductionAndContribution },
			typeof(PRDeductCode.cntPercent))]
		[PXUIField(DisplayName = "Contribution Percent")]
		public decimal? BenefitRate { get; set; }
		#endregion
		#region EffectiveDate
		public abstract class effectiveDate : BqlDateTime.Field<effectiveDate> { }
		/// <summary>
		/// A date when the deduction or benefit rate becomes effective.
		/// </summary>
		[PXDefault]
		[PXDBDate]
		[PXUIField(DisplayName = "Effective Date")]
		[PXCheckUnique(typeof(projectID), typeof(deductionAndBenefitCodeID), typeof(laborItemID), ClearOnDuplicate = false)]
		public virtual DateTime? EffectiveDate { get; set; }
		#endregion
		#region LaborItemID
		public abstract class laborItemID : Data.BQL.BqlInt.Field<laborItemID> { }
		/// <summary>
		/// A labor item associated with the project pay rate.
		/// The field is included in <see cref="FK.LaborItem"/>.
		/// </summary>
		[PMLaborItem(typeof(projectID), null, null)]
		[PXForeignReference(typeof(Field<laborItemID>.IsRelatedTo<InventoryItem.inventoryID>))]
		public virtual int? LaborItemID { get; set; }
		#endregion

		#region CountryUS
		[PXString(2)]
		[PXUnboundDefault(typeof(BQLLocationConstants.CountryUS))]
		public string CountryUS { get; set; }
		public abstract class countryUS : BqlString.Field<countryUS> { }
		#endregion

		#region System Columns
		#region TStamp
		public class tStamp : IBqlField { }
		[PXDBTimestamp()]
		public byte[] TStamp { get; set; }
		#endregion
		#region CreatedByID
		public class createdByID : IBqlField { }
		[PXDBCreatedByID()]
		public Guid? CreatedByID { get; set; }
		#endregion
		#region CreatedByScreenID
		public class createdByScreenID : IBqlField { }
		[PXDBCreatedByScreenID()]
		public string CreatedByScreenID { get; set; }
		#endregion
		#region CreatedDateTime
		public class createdDateTime : IBqlField { }
		[PXDBCreatedDateTime()]
		public DateTime? CreatedDateTime { get; set; }
		#endregion
		#region LastModifiedByID
		public class lastModifiedByID : IBqlField { }
		[PXDBLastModifiedByID()]
		public Guid? LastModifiedByID { get; set; }
		#endregion
		#region LastModifiedByScreenID
		public class lastModifiedByScreenID : IBqlField { }
		[PXDBLastModifiedByScreenID()]
		public string LastModifiedByScreenID { get; set; }
		#endregion
		#region LastModifiedDateTime
		public class lastModifiedDateTime : IBqlField { }
		[PXDBLastModifiedDateTime()]
		public DateTime? LastModifiedDateTime { get; set; }
		#endregion
		#endregion
	}
}
