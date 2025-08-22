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
using PX.Objects.CS;
using PX.Objects.CM;
using PX.Objects.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.SM;

namespace PX.Objects.CR
{
	[SerializableAttribute]
    [PXPrimaryGraph(typeof(CRSetupMaint))]
    [PXCacheName(Messages.CRSetup)]
    public partial class CRSetup : PXBqlTable, IBqlTable, PXNoteAttribute.IPXCopySettings
	{
		#region CampaignNumberingID
		public abstract class campaignNumberingID : PX.Data.BQL.BqlString.Field<campaignNumberingID> { }
		protected String _CampaignNumberingID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault("CAMPAIGN")]
		[PXUIField(DisplayName = "Campaign Numbering Sequence")]
        [PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
		public virtual String CampaignNumberingID
		{
			get
			{
				return this._CampaignNumberingID;
			}
			set
			{
				this._CampaignNumberingID = value;
			}
		}
		#endregion
		#region OpportunityNumberingID
		public abstract class opportunityNumberingID : PX.Data.BQL.BqlString.Field<opportunityNumberingID> { }
		protected String _OpportunityNumberingID;
		[PXDBString(10, IsUnicode = true)]
        [PXDefault("OPPORTUNTY")]
		[PXUIField(DisplayName = "Opportunity Numbering Sequence")]
		[PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
		public virtual String OpportunityNumberingID
		{
			get
			{
				return this._OpportunityNumberingID;
			}
			set
			{
				this._OpportunityNumberingID = value;
			}
		}
        #endregion
	    #region QuoteNumberingID
	    public abstract class quoteNumberingID : PX.Data.BQL.BqlString.Field<quoteNumberingID> { }
	    protected String _QuoteNumberingID;
	    [PXDBString(10, IsUnicode = true)]
	    [PXDefault("CRQUOTE")]
	    [PXUIField(DisplayName = "Quote Numbering Sequence")]
	    [PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
	    public virtual String QuoteNumberingID
	    {
	        get
	        {
	            return this._QuoteNumberingID;
	        }
	        set
	        {
	            this._QuoteNumberingID = value;
	        }
	    }
	    #endregion
        #region CaseNumberingID
        public abstract class caseNumberingID : PX.Data.BQL.BqlString.Field<caseNumberingID> { }
		protected String _CaseNumberingID;
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXDefault("CASE")]
		[PXUIField(DisplayName = "Case Numbering Sequence")]
		[PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
		public virtual String CaseNumberingID
		{
			get
			{
				return this._CaseNumberingID;
			}
			set
			{
				this._CaseNumberingID = value;
			}
		}
		#endregion
		#region MassMailNumberingID
		public abstract class massMailNumberingID : PX.Data.BQL.BqlString.Field<massMailNumberingID> { }
		protected String _MassMailNumberingID;
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXDefault("MMAIL")]
		[PXUIField(DisplayName = "Mass Mail Numbering Sequence")]
		[PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
		public virtual String MassMailNumberingID
		{
			get
			{
				return this._MassMailNumberingID;
			}
			set
			{
				this._MassMailNumberingID = value;
			}
		}
		#endregion
		#region DefaultCaseClassID
		public abstract class defaultCaseClassID : PX.Data.BQL.BqlString.Field<defaultCaseClassID> { }
		protected String _DefaultCaseClassID;
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXUIField(DisplayName = "Default Case Class")]
		[PXSelector(typeof(CRCaseClass.caseClassID), DescriptionField = typeof(CRCaseClass.description), CacheGlobal = true)]
		public virtual String DefaultCaseClassID
		{
			get
			{
				return this._DefaultCaseClassID;
			}
			set
			{
				this._DefaultCaseClassID = value;
			}
		}
		#endregion
		#region DefaultOpportunityClassID
		public abstract class defaultOpportunityClassID : PX.Data.BQL.BqlString.Field<defaultOpportunityClassID> { }
		protected String _DefaultOpportunityClassID;
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXUIField(DisplayName = "Default Opportunity Class")]
		[PXSelector(typeof(CROpportunityClass.cROpportunityClassID), DescriptionField = typeof(CROpportunityClass.description), CacheGlobal = true)]
		public virtual String DefaultOpportunityClassID
		{
			get
			{
				return this._DefaultOpportunityClassID;
			}
			set
			{
				this._DefaultOpportunityClassID = value;
			}
		}
		#endregion
		#region DefaultRateTypeID
		public abstract class defaultRateTypeID : PX.Data.BQL.BqlString.Field<defaultRateTypeID> { }
		protected String _DefaultRateTypeID;
		[PXDBString(6, IsUnicode = true)]
		[PXSelector(typeof(PX.Objects.CM.CurrencyRateType.curyRateTypeID), DescriptionField = typeof(CurrencyRateType.descr))]
		[PXForeignReference(typeof(Field<defaultRateTypeID>.IsRelatedTo<CurrencyRateType.curyRateTypeID>))]
		[PXUIField(DisplayName = "Default Rate Type ")]
		public virtual String DefaultRateTypeID
		{
			get
			{
				return this._DefaultRateTypeID;
			}
			set
			{
				this._DefaultRateTypeID = value;
			}
		}
		#endregion
		#region AllowOverrideRate
		public abstract class allowOverrideRate : PX.Data.BQL.BqlBool.Field<allowOverrideRate> { }
		protected Boolean? _AllowOverrideRate;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Enable Rate Override")]
		public virtual Boolean? AllowOverrideRate
		{
			get
			{
				return this._AllowOverrideRate;
			}
			set
			{
				this._AllowOverrideRate = value;
			}
		}
		#endregion
		#region LeadDefaultAssignmentMapID
		public abstract class leaddefaultAssignmentMapID : PX.Data.BQL.BqlInt.Field<leaddefaultAssignmentMapID> { }

		[AssignmentMap(typeof(AssignmentMapType.AssignmentMapTypeLead), DisplayName = "Lead Assignment Map")]
		public virtual int? LeadDefaultAssignmentMapID { get; set; }
		#endregion

		#region ContactDefaultAssignmentMapID
		public abstract class contactdefaultAssignmentMapID : PX.Data.BQL.BqlInt.Field<contactdefaultAssignmentMapID> { }

		[AssignmentMap(typeof(AssignmentMapType.AssignmentMapTypeContact), DisplayName = "Contact Assignment Map")]
		public virtual int? ContactDefaultAssignmentMapID { get; set; }
		#endregion

		#region DefaultCaseAssignmentMapID
		public abstract class defaultCaseAssignmentMapID : PX.Data.BQL.BqlInt.Field<defaultCaseAssignmentMapID> { }

		[AssignmentMap(typeof(AssignmentMapType.AssignmentMapTypeCase), DisplayName = "Case Assignment Map")]
		public virtual int? DefaultCaseAssignmentMapID { get; set; }
		#endregion
		#region DefaultBAccountAssignmentMapID
		public abstract class defaultBAccountAssignmentMapID : PX.Data.BQL.BqlInt.Field<defaultBAccountAssignmentMapID> { }

		[AssignmentMap(typeof(AssignmentMapType.AssignmentMapTypeProspect), DisplayName = "Business Account Assignment Map")]
		public virtual int? DefaultBAccountAssignmentMapID { get; set; }
		#endregion

		#region DefaultOpportunityAssignmentMapID
		public abstract class defaultOpportunityAssignmentMapID : PX.Data.BQL.BqlInt.Field<defaultOpportunityAssignmentMapID> { }

		[AssignmentMap(typeof(AssignmentMapType.AssignmentMapTypeOpportunity), DisplayName = "Opportunity Assignment Map")]
		public virtual int? DefaultOpportunityAssignmentMapID { get; set; }
		#endregion

        #region AssignmentMapID
        public abstract class quoteApprovalMapID : PX.Data.BQL.BqlInt.Field<quoteApprovalMapID> { }

		[ApprovalMap(typeof(AssignmentMapType.AssignmentMapTypeQuotes))]
        public virtual int? QuoteApprovalMapID { get; set; }
        #endregion

        #region QuoteApprovalNotificationID
        public abstract class quoteApprovalNotificationID : PX.Data.BQL.BqlInt.Field<quoteApprovalNotificationID> { }

        [EmailNotification(DisplayName = "Pending Approval Notification")]
        public virtual int? QuoteApprovalNotificationID { get; set; }
		#endregion


		#region DefaultLeadClassID
		public abstract class defaultLeadClassID : PX.Data.BQL.BqlString.Field<defaultLeadClassID> { }
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXUIField(DisplayName = "Default Lead Class")]
		[PXSelector(typeof(CRLeadClass.classID), DescriptionField = typeof(CRLeadClass.description), CacheGlobal = true)]
		public virtual String DefaultLeadClassID { get; set; }
		#endregion

		#region DefaultContactClassID
		public abstract class defaultContactClassID : PX.Data.BQL.BqlString.Field<defaultContactClassID> { }
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXUIField(DisplayName = "Default Contact Class")]
		[PXSelector(typeof(CRContactClass.classID), DescriptionField = typeof(CRContactClass.description), CacheGlobal = true)]
		public virtual String DefaultContactClassID { get; set; }
		#endregion

		#region DefaultCustomerClassID
		public abstract class defaultCustomerClassID : PX.Data.BQL.BqlString.Field<defaultCustomerClassID> { }
		protected String _DefaultCustomerClassID;
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXUIField(DisplayName = "Default Business Account Class")]
		[PXSelector(typeof(CRCustomerClass.cRCustomerClassID), DescriptionField = typeof(CRCustomerClass.description), CacheGlobal = true)]
		public virtual String DefaultCustomerClassID
		{
			get
			{
				return this._DefaultCustomerClassID;
			}
			set
			{
				this._DefaultCustomerClassID = value;
			}
		}
		#endregion

		#region CopyNotes
		public abstract class copyNotes : PX.Data.BQL.BqlBool.Field<copyNotes> { }
		protected bool? _CopyNotes;
		[PXDBBool]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Copy Notes")]
		public virtual bool? CopyNotes
		{
			get
			{
				return _CopyNotes;
			}
			set
			{
				_CopyNotes = value;
			}
		}
		#endregion
		#region CopyFiles
		public abstract class copyFiles : PX.Data.BQL.BqlBool.Field<copyFiles> { }
		protected bool? _CopyFiles;
		[PXDBBool]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Copy Attachments")]
		public virtual bool? CopyFiles
		{
			get
			{
				return _CopyFiles;
			}
			set
			{
				_CopyFiles = value;
			}
		}
		#endregion

		#region DuplicateScoresNormalization
		public abstract class duplicateScoresNormalization : PX.Data.BQL.BqlBool.Field<duplicateScoresNormalization> { }
		
		[PXDBBool]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Normalize Validation Scores", FieldClass = FeaturesSet.contactDuplicate.FieldClass)]
		public virtual bool? DuplicateScoresNormalization { get; set; }
		#endregion

		#region DuplicateWordsDelimiters
		public abstract class duplicateCharsDelimiters : PX.Data.BQL.BqlString.Field<duplicateCharsDelimiters> { }

		/// <summary>
		/// Delimiters used for option: <see cref="TransformationRulesAttribute.SplitWords"/> during grams calculation.
		/// For delimiters different from default
		/// (<see cref="PX.Objects.CR.Extensions.CRDuplicateEntities.CRCharsDelimitersAttribute.DefaultDelimiters"/>
		/// attach custom <see cref="PX.Objects.CR.Extensions.CRDuplicateEntities.CRCharsDelimitersAttribute"/> with required pattern.
		/// For instance, " ,:^" will use 4 delimiters: ' ', ',', ':', '^'.
		/// </summary>
		[PXString]
		[PX.Objects.CR.Extensions.CRDuplicateEntities.CRCharsDelimiters]
		public virtual string DuplicateCharsDelimiters { get; set; }
		#endregion

		#region CreatedByID
		public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
		protected Guid? _CreatedByID;
		[PXDBCreatedByID()]
		public virtual Guid? CreatedByID
		{
			get
			{
				return this._CreatedByID;
			}
			set
			{
				this._CreatedByID = value;
			}
		}
		#endregion
		#region CreatedByScreenID
		public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
		protected String _CreatedByScreenID;
		[PXDBCreatedByScreenID()]
		public virtual String CreatedByScreenID
		{
			get
			{
				return this._CreatedByScreenID;
			}
			set
			{
				this._CreatedByScreenID = value;
			}
		}
		#endregion
		#region CreatedDateTime
		public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
		protected DateTime? _CreatedDateTime;
		[PXDBCreatedDateTime()]
		public virtual DateTime? CreatedDateTime
		{
			get
			{
				return this._CreatedDateTime;
			}
			set
			{
				this._CreatedDateTime = value;
			}
		}
		#endregion
		#region LastModifiedByID
		public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
		protected Guid? _LastModifiedByID;
		[PXDBLastModifiedByID()]
		public virtual Guid? LastModifiedByID
		{
			get
			{
				return this._LastModifiedByID;
			}
			set
			{
				this._LastModifiedByID = value;
			}
		}
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
		protected String _LastModifiedByScreenID;
		[PXDBLastModifiedByScreenID()]
		public virtual String LastModifiedByScreenID
		{
			get
			{
				return this._LastModifiedByScreenID;
			}
			set
			{
				this._LastModifiedByScreenID = value;
			}
		}
		#endregion
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
		protected DateTime? _LastModifiedDateTime;
		[PXDBLastModifiedDateTime()]
		public virtual DateTime? LastModifiedDateTime
		{
			get
			{
				return this._LastModifiedDateTime;
			}
			set
			{
				this._LastModifiedDateTime = value;
			}
		}
		#endregion
		#region tstamp
		public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }
		protected Byte[] _tstamp;
		[PXDBTimestamp()]
		public virtual Byte[] tstamp
		{
			get
			{
				return this._tstamp;
			}
			set
			{
				this._tstamp = value;
			}
		}
		#endregion
		#region NoteID
		public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }

		[PXNote]
		public virtual Guid? NoteID { get; set; }
		#endregion
	}

	[PXHidden]
	[PXBreakInheritance]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class CRSetupQuoteApproval : CRSetup, IAssignedMap
	{
		public int? AssignmentMapID { get => QuoteApprovalMapID; set => QuoteApprovalMapID = value; }
		public int? AssignmentNotificationID { get => QuoteApprovalNotificationID; set => QuoteApprovalNotificationID = value; }
		public bool? IsActive => (AssignmentMapID != null);
	}
}
