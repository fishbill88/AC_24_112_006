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
using System.Diagnostics;
using PX.Data;
using PX.Objects.CR;
using PX.SM;
using PX.Objects.CS;
using PX.Objects.CS.DAC;
using PX.Objects.AP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CM;
using PX.Objects.GL.Attributes;

namespace PX.Objects.GL.DAC
{
    [PXCacheName(CS.Messages.Company)]
	[Serializable]
	public partial class Organization : PXBqlTable, IBqlTable, IIncludable, I1099Settings
	{
		#region Keys
		public class PK : PrimaryKeyOf<Organization>.By<organizationID>
		{
			public static Organization Find(PXGraph graph, int? organizationID, PKFindOptions options = PKFindOptions.None) => FindBy(graph, organizationID, options);
		}
		public class UK : PrimaryKeyOf<Organization>.By<organizationCD>
		{
			public static Organization Find(PXGraph graph, string organizationCD, PKFindOptions options = PKFindOptions.None) => FindBy(graph, organizationCD, options);
		}
		public static class FK
		{
			public class BAccount : CR.BAccount.PK.ForeignKeyOf<Organization>.By<bAccountID> { }
			public class ActualLedger : GL.Ledger.PK.ForeignKeyOf<Organization>.By<actualLedgerID> { }
			public class Country : CS.Country.PK.ForeignKeyOf<Organization>.By<countryID> { }
			
		}
		#endregion

		#region Selected
		public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
	    protected bool? _Selected = false;
	    [PXBool]
	    [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
	    [PXUIField(DisplayName = "Selected")]
	    public virtual bool? Selected
	    {
		    get
		    {
			    return _Selected;
		    }
		    set
		    {
			    _Selected = value;
		    }
	    }
	    #endregion
		#region OrganizationID
		public abstract class organizationID : PX.Data.BQL.BqlInt.Field<organizationID> { }

		[PXDBIdentity]
        public virtual int? OrganizationID { get; set; }
		#endregion
		#region OrganizationCD
		public abstract class organizationCD : PX.Data.BQL.BqlString.Field<organizationCD> { }

		[PXDimension("COMPANY")]
        [PXDBString(30, IsUnicode = true, IsKey = true, InputMask = "")]
		[PXUIField(DisplayName = "Company ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDBDefault(typeof(OrganizationBAccount.acctCD))]
        public virtual string OrganizationCD { get; set; }
        #endregion
        #region OrganizationType
        public abstract class organizationType : PX.Data.BQL.BqlString.Field<organizationType> { }

        [PXDBString(15)]
        [OrganizationTypes.List]
		[PXDefault(OrganizationTypes.WithoutBranches)]
		[PXUIField(DisplayName = "Company Type")]
        public virtual string OrganizationType { get; set; }
		#endregion

		#region FileTaxesByBranches
		public abstract class fileTaxesByBranches : PX.Data.BQL.BqlBool.Field<fileTaxesByBranches> { }

	    [PXDBBool]
	    [PXDefault(false)]
		[PXUIField(DisplayName = "File Taxes by Branch")]
	    public virtual bool? FileTaxesByBranches { get; set; }
		#endregion

		#region BaseCuryID
		public abstract class baseCuryID : PX.Data.BQL.BqlString.Field<baseCuryID> { }

		/// <summary>
		/// The base <see cref="Currency"/> of the company.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="Currency.CuryID"/> field.
		/// </value>
		[PXDBString(5, IsUnicode = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
		[PXUIField(DisplayName = "Base Currency ID")]
		[PXSelector(typeof(Search<CurrencyList.curyID>), DescriptionField = typeof(CurrencyList.description))]
		public virtual String BaseCuryID { get; set; }
		#endregion
		#region DunningFeeBranchID
		public abstract class dunningFeeBranchID : PX.Data.BQL.BqlInt.Field<dunningFeeBranchID> { }
		[Branch(addDefaultAttribute: false, useDefaulting: false, DisplayName = "Dunning Letter Branch", FieldClass = "COMPANYBRANCH")]
		public virtual int? DunningFeeBranchID { get; set; }
		#endregion

		// duplicate fields from the Branch DAC
		#region ActualLedgerID
		public abstract class actualLedgerID : PX.Data.BQL.BqlInt.Field<actualLedgerID> { }

        [PXDBInt()]
        public virtual int? ActualLedgerID { get; set; }
        #endregion
        #region ActualLedgerCD
        public abstract class actualLedgerCD : PX.Data.BQL.BqlString.Field<actualLedgerCD> { }

        [PXString(10, IsUnicode = true)]
        public virtual string ActualLedgerCD { get; set; }
        #endregion
        #region Active
        public abstract class active : PX.Data.BQL.BqlBool.Field<active> { }

        [PXDBBool()]
        [PXUIField(DisplayName = "Active", FieldClass = "MULTICOMPANY")]
		[PXDefault(true)]
        public virtual bool? Active { get; set; }
		#endregion
		#region Status
		public new abstract class status : PX.Data.BQL.BqlString.Field<status> { }
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Status", IsReadOnly = true)]
		[PXDefault(OrganizationStatus.Active)]
		[OrganizationStatus.List]
		public String Status { get; set; }
		#endregion
		#region OrganizationName
		public abstract class organizationName : PX.Data.BQL.BqlString.Field<organizationName> { }

	    /// <summary>
	    /// The name of the organization.
	    /// </summary>
	    [PXDBString(60, IsUnicode = true)]
	    [PXUIField(DisplayName = "Company Name", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
	    public virtual String OrganizationName { get; set; }
	    #endregion
		#region RoleName
		public abstract class roleName : PX.Data.BQL.BqlString.Field<roleName> { }

        /// <summary>
        /// The name of the <see cref="Roles">Role</see> to be used to grant users access to the data of the Organization. 
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="Roles.Rolename"/> field.
        /// </value>
		[PXDBString(64, IsUnicode = true, InputMask = "")]
        [PXSelector(typeof(Search<Roles.rolename, Where<Roles.guest, Equal<False>>>), DescriptionField = typeof(Roles.descr))]
        [PXUIField(DisplayName = "Access Role")]
        public virtual string RoleName { get; set; }
        #endregion
        #region PhoneMask
        public abstract class phoneMask : PX.Data.BQL.BqlString.Field<phoneMask> { }

        /// <summary>
        /// The mask used to display phone numbers for the objects, which belong to this Organization.
        /// See also the <see cref="Company.PhoneMask"/>.
        /// </summary>
		[PXDBString(50)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Phone Mask")]
        public virtual string PhoneMask { get; set; }
        #endregion
        #region CountryID
        public abstract class countryID : PX.Data.BQL.BqlString.Field<countryID> { }

        /// <summary>
        /// Identifier of the default Country of the Organization.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="Country.CountryID"/> field.
        /// </value>
		[PXDBString(2, IsFixed = true)]
        [PXUIField(DisplayName = "Default Country")]
        [PXSelector(typeof(Country.countryID), DescriptionField = typeof(Country.description))]
        public virtual string CountryID { get; set; }
		#endregion

		#region OrganizationLocalizationCode
		public abstract class organizationLocalizationCode : PX.Data.BQL.BqlString.Field<PX.Objects.GL.DAC.Organization.organizationLocalizationCode> { }
		[PXDBString(2)]
		[PXUIField(DisplayName = "Localization")]
		[PXDefault(PX.Data.Localization.Constants.StandardLocalizationCode)]
		[OrganizationLocalizationList]
		public virtual string OrganizationLocalizationCode
		{
			get;
			set;
		}

		#endregion

		#region CashDiscountBase
		public abstract class cashDiscountBase : PX.Data.BQL.BqlString.Field<cashDiscountBase> { }

		/// <summary>
		/// Represents the type of cash discount base.
		/// </summary>
		/// <value>
		/// The field can have one of the values listed in the <see cref="CashDiscountBases"/> class.
		/// </value>
		[PXDBString(2)]
		[CashDiscountBases.List]
		[PXDefault(CashDiscountBases.DocumentAmount)]
		[PXUIField(DisplayName = "Cash Discount Base")]
		public virtual string CashDiscountBase { get; set; }
		#endregion

		#region DefaultPrinterID
		public abstract class defaultPrinterID : PX.Data.BQL.BqlGuid.Field<defaultPrinterID> { }
		protected Guid? _DefaultPrinterID;
		[PXPrinterSelector(DisplayName = "Default Printer", Visibility = PXUIVisibility.SelectorVisible)]
		[PXForeignReference(typeof(Field<defaultPrinterID>.IsRelatedTo<SMPrinter.printerID>))]
		public virtual Guid? DefaultPrinterID
		{
			get
			{
				return this._DefaultPrinterID;
			}
			set
			{
				this._DefaultPrinterID = value;
			}
		}
		#endregion

		#region CarrierFacility  
		public abstract class carrierFacility : PX.Data.BQL.BqlString.Field<carrierFacility> { }

		[PXDBString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "Carrier Facility")]
		public virtual string CarrierFacility { get; set; }
		#endregion

	    #region OverrideThemeVariables
	    [PXDBBool]
		[PXDefault(false)]
		[PXUIVisible(typeof(PXThemeVariableAttribute.ThemeHasVariables))]
		[PXUIField(DisplayName = "Override Colors for the Selected Company")]
	    public bool? OverrideThemeVariables { get; set; }
	    public abstract class overrideThemeVariables : IBqlField { }
	    #endregion

		#region PrimaryColor
	    public abstract class primaryColor : PX.Data.BQL.BqlString.Field<primaryColor> { }

	    [PXString(30, IsUnicode = true, InputMask = "")]
	    [PXUIField(DisplayName = "Primary Color")]
	    [PXThemeVariable("--primary-color", PersistDefaultValue = typeof(overrideThemeVariables))]
	    [PXUIEnabled(typeof(overrideThemeVariables))]
		public string PrimaryColor { get; set; }
	    #endregion

	    #region BackgroundColor
	    public abstract class backgroundColor : PX.Data.BQL.BqlString.Field<backgroundColor> { }

	    [PXString(30, IsUnicode = true, InputMask = "")]
	    [PXUIField(DisplayName = "Background Color")]
	    [PXThemeVariable("--background-color", PersistDefaultValue = typeof(overrideThemeVariables))]
	    [PXUIEnabled(typeof(overrideThemeVariables))]
		public string BackgroundColor { get; set; }
	    #endregion

		// "General Info" tab
		#region BAccountID
		public abstract class bAccountID : PX.Data.BQL.BqlInt.Field<bAccountID> { }

        /// <summary>
        /// Identifier of the <see cref="BAccount">Business Account</see> of the Organization.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="BAccount.BAccountID"/> field.
        /// </value>
        [PXDBInt()]
        [PXUIField(Visible = true, Enabled = false)]
        [PXSelector(typeof(CR.BAccountR.bAccountID), ValidateValue = false)]
		[PXDBDefault(typeof(OrganizationBAccount.bAccountID))]
        public virtual int? BAccountID { get; set; }
        #endregion

        // "Logo" tab
        #region LogoName
        public abstract class logoName : PX.Data.BQL.BqlString.Field<logoName> { }

        /// <summary>
        /// The name of the logo image file.
        /// </summary>
		[PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Logo File")]
        public string LogoName { get; set; }

	    [PXString]
	    [PXUIField(DisplayName = "Logo File")]
	    public string LogoNameGetter { get { return LogoName; } set { } }
		#endregion
		#region LogoNameReport
		public abstract class logoNameReport : PX.Data.BQL.BqlString.Field<logoNameReport> { }

        /// <summary>
        /// The name of the report logo image file.
        /// </summary>
		[PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Report Logo File")]
        public string LogoNameReport { get; set; }

	    [PXString]
	    [PXUIField(DisplayName = "Logo File")]
	    public string LogoNameReportGetter { get { return LogoNameReport; } set { } }
		#endregion

		// "1099 Settings" tab
		#region TCC
		public abstract class tCC : PX.Data.BQL.BqlString.Field<tCC> { }

        /// <summary>
        /// Transmitter Control Code (TCC) for the 1099 form.
        /// </summary>
        [PXDBString(5, IsUnicode = true)]
        [PXUIField(DisplayName = "Transmitter Control Code (TCC)")]
        public virtual string TCC { get; set; }
        #endregion
        #region ForeignEntity
        public abstract class foreignEntity : PX.Data.BQL.BqlBool.Field<foreignEntity> { }

        /// <summary>
        /// Indicates whether the Organization is considered a Foreign Entity in the context of 1099 form.
        /// </summary>
        [PXDBBool()]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Foreign Entity")]
        public virtual bool? ForeignEntity { get; set; }
        #endregion
        #region CFSFiler
        public abstract class cFSFiler : PX.Data.BQL.BqlBool.Field<cFSFiler> { }

        /// <summary>
        /// Combined Federal/State Filer for the 1099 form.
        /// </summary>
        [PXDBBool()]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Combined Federal/State Filer")]
        public virtual bool? CFSFiler { get; set; }
        #endregion
        #region ContactName
        public abstract class contactName : PX.Data.BQL.BqlString.Field<contactName> { }

        /// <summary>
        /// Contact Name for the 1099 form.
        /// </summary>
		[PXDBString(40, IsUnicode = true)]
        [PXUIField(DisplayName = "Contact Name")]
        public virtual string ContactName { get; set; }
        #endregion
        #region CTelNumber
        public abstract class cTelNumber : PX.Data.BQL.BqlString.Field<cTelNumber> { }

        /// <summary>
        /// Contact Phone Number for the 1099 form.
        /// </summary>
		[PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "Contact Telephone Number")]
        public virtual string CTelNumber { get; set; }
        #endregion
        #region CEmail
        public abstract class cEmail : PX.Data.BQL.BqlString.Field<cEmail> { }

        /// <summary>
        /// Contact E-mail for the 1099 form.
        /// </summary>
		[PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "Contact E-mail")]
        public virtual string CEmail { get; set; }
        #endregion
        #region NameControl
        public abstract class nameControl : PX.Data.BQL.BqlString.Field<nameControl> { }

        /// <summary>
        /// Name Control for the 1099 form.
        /// </summary>
        [PXDBString(4, IsUnicode = true)]
        [PXUIField(DisplayName = "Name Control")]
        public virtual string NameControl { get; set; }
        #endregion
        #region Reporting1099
        public abstract class reporting1099 : PX.Data.BQL.BqlBool.Field<reporting1099> { }

        [PXDBBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "1099-MISC Reporting Entity")]
		[PXUIEnabled(typeof(Where<Organization.reporting1099ByBranches.IsNotEqual<True>>))]
		public virtual bool? Reporting1099 { get; set; }
		#endregion
		#region Reporting1099ByBranches
		public abstract class reporting1099ByBranches : PX.Data.BQL.BqlBool.Field<reporting1099ByBranches> { }

		[PXDBBool]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIVisible(typeof(Where<Organization.organizationType.IsEqual<OrganizationTypes.withBranchesBalancing>>))]
		[PXUIField(DisplayName = "File 1099-MISC by Branch")]
		public virtual bool? Reporting1099ByBranches { get; set; }
		#endregion

		// Technical fields
		#region CreatedByID
		public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }

		[PXDBCreatedByID()]
		public virtual Guid? CreatedByID { get; set; }
		#endregion
		#region CreatedByScreenID
		public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }

		[PXDBCreatedByScreenID()]
		public virtual String CreatedByScreenID { get; set; }
		#endregion
		#region CreatedDateTime
		public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }

		[PXDBCreatedDateTime]
		[PXUIField(
			DisplayName = PXDBLastModifiedByIDAttribute.DisplayFieldNames.CreatedDateTime,
			Enabled = false,
			IsReadOnly = true)]
		public virtual DateTime? CreatedDateTime { get; set; }
		#endregion
		#region LastModifiedByID
		public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }

		[PXDBLastModifiedByID()]
		public virtual Guid? LastModifiedByID { get; set; }
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }

		[PXDBLastModifiedByScreenID()]
		public virtual String LastModifiedByScreenID { get; set; }
		#endregion
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }

		[PXDBLastModifiedDateTime]
		[PXUIField(
			DisplayName = PXDBLastModifiedByIDAttribute.DisplayFieldNames.LastModifiedDateTime,
			Enabled = false,
			IsReadOnly = true)]
		public virtual DateTime? LastModifiedDateTime { get; set; }
		#endregion

		#region tstamp
		public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }

        [PXDBTimestamp()]
        public virtual Byte[] tstamp { get; set; }
		#endregion

		//TODO 444 Pank Org GroupMask: I think it should be eliminated 
		#region GroupMask
		public abstract class groupMask : PX.Data.BQL.BqlByteArray.Field<groupMask> { }
        protected Byte[] _GroupMask;

        /// <summary>
        /// The group mask showing which <see cref="PX.SM.RelationGroup">restriction groups</see> the Branch belongs to.
        /// To learn more about the way restriction groups are managed, see the documentation for the GL Account Access (GL104000) form
        /// (which corresponds to the <see cref="GLAccess"/> graph).
        /// </summary>
        [PXDBGroupMask()]
        public virtual Byte[] GroupMask { get; set; }
        #endregion
        #region Included
        public abstract class included : PX.Data.BQL.BqlBool.Field<included> { }

        /// <summary>
        /// An unbound field used in the User Interface to include the Organization into a <see cref="PX.SM.RelationGroup">restriction group</see>.
        /// </summary>
        [PXUnboundDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXBool]
        [PXUIField(DisplayName = "Included")]
        public virtual bool? Included { get; set; }
		#endregion
		#region NoteID
	    public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }

	    [PXNote]
	    public virtual Guid? NoteID { get; set; }
	    #endregion
	}
	[Serializable]
	[PXHidden]
	[DebuggerDisplay("BranchItem: ID = {BAccountID,nq}, AcctCD = {AcctCD}, AcctName = {AcctName}, ParentID={ParentBAccountID, nq}")]
	[PXBreakInheritance]
	public partial class BranchItem : BAccount
	{
		#region Keys
		public new class PK : PrimaryKeyOf<BranchItem>.By<bAccountID>
		{
			public static BranchItem Find(PXGraph graph, int? bAccountID, PKFindOptions options = PKFindOptions.None) => FindBy(graph, bAccountID, options);
		}
		public new class UK : PrimaryKeyOf<BranchItem>.By<acctCD>
		{
			public static BranchItem Find(PXGraph graph, string acctCD, PKFindOptions options = PKFindOptions.None) => FindBy(graph, acctCD, options);
		}
		public new static class FK
		{
			public class ParentBAccount : CR.BAccount.PK.ForeignKeyOf<BranchItem>.By<parentBAccountID> { }
		}
		#endregion

		#region BAccountID
		public new abstract class bAccountID : PX.Data.BQL.BqlInt.Field<bAccountID> { }

		#endregion

		#region AcctCD

		public new abstract class acctCD : PX.Data.BQL.BqlString.Field<acctCD> { }

		[PXDBString(30, IsUnicode = true, IsKey = true, InputMask = "")]
		[PXDefault]
		[PXDimension("COMPANY")]
		[PXUIField(DisplayName = "Cd", Visibility = PXUIVisibility.SelectorVisible)]
		public override String AcctCD
		{
			get { return this._AcctCD; }
			set { this._AcctCD = value; }
		}

		#endregion

		#region AcctName

		public new abstract class acctName : PX.Data.BQL.BqlString.Field<acctName> { }

		#endregion

		#region ParentBAccountID
		public new abstract class parentBAccountID : PX.Data.BQL.BqlInt.Field<BAccount.parentBAccountID> { }
		#endregion

		#region PrimaryGroupID
		public abstract class primaryGroupID : PX.Data.BQL.BqlInt.Field<primaryGroupID> { }

		[PXInt]
		public virtual Int32? PrimaryGroupID { get; set; }
		#endregion

		#region NoteID

		public new abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }

		#endregion

		#region Type

		public new abstract class type : PX.Data.BQL.BqlString.Field<type>
		{
			public const string Group = "group";
			public const string Organization = "organization";
			public const string Branch = "branch";
			public class ListAttribute : PX.Data.PXStringListAttribute
			{
				public ListAttribute()
					: base(
						new string[] { Group, Organization, Branch },
						new string[]
							{Messages.Group, Messages.Organization, Messages.Branch})
				{
					;
				}
			}
		}
		[PXString]
		public new string Type { get; set; }

		#endregion

		#region CanSelect
		public abstract class canSelect : PX.Data.BQL.BqlBool.Field<canSelect> { }

		[PXBool]
		public virtual bool? CanSelect { get; set; } = true;
		#endregion
	}

	[Serializable]
	[PXHidden]
	public partial class GroupOrganizationLink : PXBqlTable, IBqlTable
	{
		#region GroupID
		public abstract class groupID : PX.Data.BQL.BqlInt.Field<groupID> { }

		[PXDBInt(IsKey = true)]
		public virtual int? GroupID { get; set; }
		#endregion

		#region OrganizationID
		public abstract class organizationID : PX.Data.BQL.BqlInt.Field<organizationID> { }

		[PXDBInt(IsKey = true)]
		public virtual int? OrganizationID { get; set; }
		#endregion

		#region PrimaryGroup
		public abstract class primaryGroup : PX.Data.BQL.BqlBool.Field<primaryGroup> { }

		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Primary", Enabled = false)]
		public virtual bool? PrimaryGroup { get; set; }
		#endregion
	}

	[PXHidden]
	[PXProjection(typeof(Select<GroupOrganizationLink, Where<GroupOrganizationLink.primaryGroup.IsEqual<True>>>))]
	public partial class PrimaryGroup : GroupOrganizationLink
	{
		public abstract new class organizationID : PX.Data.BQL.BqlInt.Field<organizationID> { }

		public abstract new class groupID : PX.Data.BQL.BqlInt.Field<groupID> { }

		[PXDBInt(IsKey = true)]
		[PXUIField(DisplayName = "Primary Group")]
		[PXSelector(typeof(Search<Organization.organizationID, Where<Organization.organizationType.IsNotEqual<OrganizationTypes.group>>>),
			typeof(Organization.organizationCD),
			typeof(Organization.organizationName),
			SubstituteKey = typeof(Organization.organizationCD))]
		public override int? GroupID { get; set; }
	}

	public class OrganizationStatus
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute() : base(
				new[]
				{
					Pair(Active, CR.Messages.Active),
					Pair(Inactive, CR.Messages.Inactive)
				})
			{ }
		}

		public const string Active = "A";
		public const string Inactive = "I";

		public const string Initial = Active;

		public class active : PX.Data.BQL.BqlString.Constant<active>
		{
			public active() : base(Active) { }
		}
		public class inactive : PX.Data.BQL.BqlString.Constant<inactive>
		{
			public inactive() : base(Inactive) { }
		}
	}
}
