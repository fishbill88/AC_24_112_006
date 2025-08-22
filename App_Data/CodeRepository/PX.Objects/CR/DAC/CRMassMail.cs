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
using PX.Data.EP;
using PX.Objects.CS;
using PX.SM.Email;

namespace PX.Objects.CR
{
	[PXCacheName(Messages.MassMail)]
	[Serializable]
    [PXPrimaryGraph(typeof(CRMassMailMaint))]
	public partial class CRMassMail : PXBqlTable, IBqlTable, INotable
	{
		#region MassMailID
		public abstract class massMailID : PX.Data.BQL.BqlInt.Field<massMailID> { }
		protected Int32? _MassMailID;
		[PXDBIdentity]
		public virtual Int32? MassMailID
		{
			get
			{
				return this._MassMailID;
			}
			set
			{
				this._MassMailID = value;
			}
		}
		#endregion

		#region MassMailCD
		public abstract class massMailCD : PX.Data.BQL.BqlString.Field<massMailCD> { }
		protected String _MassMailCD;
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXDefault]
		[PXUIField(DisplayName = "Mass Mail ID", Visibility = PXUIVisibility.SelectorVisible)]
		[AutoNumber(typeof(CRSetup.massMailNumberingID), typeof(AccessInfo.businessDate))]
		[PXSelector(typeof(CRMassMail.massMailCD))]
		[PXFieldDescription]
		public virtual String MassMailCD
		{
			get
			{
				return this._MassMailCD;
			}
			set
			{
				this._MassMailCD = value;
			}
		}
		#endregion

		#region Status
		public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
		protected string _Status;
		[PXDBString]
		[PXDefault(CRMassMailStatusesAttribute.Hold)]
		[CRMassMailStatuses]
		[PXUIField(DisplayName = "Status")]
		public virtual string Status
		{
			get
			{
				return this._Status;
			}
			set
			{
				this._Status = value;
			}
		}
		#endregion

		#region Source
		public abstract class source : PX.Data.BQL.BqlInt.Field<source> { }
		protected int? _Source;
		[PXDBInt]
		[PXDefault(CRMassMailSourcesAttribute.MailList)]
		[CRMassMailSources]
        [PXUIField(DisplayName = "Source", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual int? Source
		{
			get
			{
				return this._Source;
			}
			set
			{
				this._Source = value;
			}
		}
		#endregion

		#region SourceType
		public abstract class sourceType : PX.Data.BQL.BqlString.Field<sourceType> { }
		protected string _SourceType;
		[PXString]
        [PXUIField(DisplayName = "Source Type")]
		public virtual string SourceType
		{
			get
			{
				return this._SourceType;
			}
			set
			{
				this._SourceType = value;
			}
		}
		#endregion

		#region PlannedDate
		public abstract class plannedDate : PX.Data.BQL.BqlDateTime.Field<plannedDate> { }
		protected DateTime? _PlannedDate;
		[PXDBDate]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Planned", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual DateTime? PlannedDate
		{
			get
			{
				return this._PlannedDate;
			}
			set
			{
				this._PlannedDate = value;
			}
		}
		#endregion

		#region SentDateTime
		public abstract class sentDateTime : PX.Data.BQL.BqlDateTime.Field<sentDateTime> { }
		protected DateTime? _SentDateTime;
		[PXDBDate(PreserveTime = true, InputMask = "g")]
		[PXUIField(DisplayName = "Sent", Visible = true, Enabled = false)]
		public virtual DateTime? SentDateTime
		{
			get
			{
				return this._SentDateTime;
			}
			set
			{
				this._SentDateTime = value;
			}
		}
		#endregion

		#region MailSubject
		public abstract class mailSubject : PX.Data.BQL.BqlString.Field<mailSubject> { }
		protected String _MailSubject;
		[PXDBString(998, IsUnicode = true)]
		[PXDefault]
		[PXUIField(DisplayName = "Subject")]
		public virtual String MailSubject
		{
			get
			{
				return this._MailSubject;
			}
			set
			{
				this._MailSubject = value;
			}
		}
		#endregion

		#region MailAccountID
		public abstract class mailAccountID : PX.Data.BQL.BqlInt.Field<mailAccountID> { }

		[EmailAccountRaw(emailAccountsToShow: EmailAccountsToShowOptions.MineAndSystem, DisplayName = "From", Required = true)]
		[PXDefault]
		public virtual int? MailAccountID { get; set; }
		#endregion

		#region MailTo
		public abstract class mailTo : PX.Data.BQL.BqlString.Field<mailTo> { }
		protected String _MailTo;
		[SMDBRecipient(isMultiple: true)]
		[PXDefault]
		[PXUIField(DisplayName = "To")]
		public virtual String MailTo
		{
			get
			{
				return this._MailTo;
			}
			set
			{
				this._MailTo = value;
			}
		}
		#endregion

		#region MailCc
		public abstract class mailCc : PX.Data.BQL.BqlString.Field<mailCc> { }
		protected String _MailCc;
		[SMDBRecipient(isMultiple: true)]
		[PXUIField(DisplayName = "CC")]
		public virtual String MailCc
		{
			get
			{
				return this._MailCc;
			}
			set
			{
				this._MailCc = value;
			}
		}
		#endregion

		#region MailBcc
		public abstract class mailBcc : PX.Data.BQL.BqlString.Field<mailBcc> { }
		protected String _MailBcc;
		[SMDBRecipient(isMultiple: true)]
		[PXUIField(DisplayName = "BCC")]
		public virtual String MailBcc
		{
			get
			{
				return this._MailBcc;
			}
			set
			{
				this._MailBcc = value;
			}
		}
		#endregion

		#region MailContent
		public abstract class mailContent : PX.Data.BQL.BqlString.Field<mailContent> { }
		protected String _MailContent;
		[PXDBText(IsUnicode = true)]
		[PXUIField(DisplayName = "Content")]
		public virtual String MailContent
		{
			get
			{
				return this._MailContent;
			}
			set
			{
				this._MailContent = value;
			}
		}
		#endregion

		#region NoteID
		public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
		protected Guid? _NoteID;
        [PXNote(
            DescriptionField = typeof(CRMassMail.massMailCD),
            Selector = typeof(CRMassMail.massMailCD),
            ShowInReferenceSelector = true)]
        public virtual Guid? NoteID
		{
			get
			{
				return this._NoteID;
			}
			set
			{
				this._NoteID = value;
			}
		}
		#endregion

		#region tstamp
		public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }
		protected Byte[] _tstamp;
		[PXDBTimestamp]
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

		#region CreatedByID
		public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
		protected Guid? _CreatedByID;
		[PXDBCreatedByID]
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
		[PXDBCreatedByScreenID]
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
        [PXDBCreatedDateTime]
        [PXUIField(DisplayName = PXDBLastModifiedByIDAttribute.DisplayFieldNames.CreatedDateTime, Enabled = false, IsReadOnly = true)]
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
		[PXDBLastModifiedByID]
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
		[PXDBLastModifiedByScreenID]
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
        [PXDBLastModifiedDateTime]
        [PXUIField(DisplayName = PXDBLastModifiedByIDAttribute.DisplayFieldNames.LastModifiedDateTime, Enabled = false, IsReadOnly = true)]
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
	}

    [Serializable]
	[PXCacheName(Messages.MassMailMarketingListMember)]
	public partial class CRMassMailMarketingList : PXBqlTable, IBqlTable
	{
		#region MassMailID
		public abstract class massMailID : PX.Data.BQL.BqlInt.Field<massMailID> { }
		protected Int32? _MassMailID;
		[PXDBInt(IsKey = true)]
		[PXDBDefault(typeof(CRMassMail.massMailID))]
		[PXParent(typeof(Select<CRMassMail, Where<CRMassMail.massMailID, Equal<Current<CRMassMailMarketingList.massMailID>>>>))]
		public virtual Int32? MassMailID
		{
			get
			{
				return this._MassMailID;
			}
			set
			{
				this._MassMailID = value;
			}
		}
		#endregion

		#region MailListID
		public abstract class mailListID : PX.Data.BQL.BqlInt.Field<mailListID> { }
		protected Int32? _MailListID;
		[PXDBInt(IsKey = true)]
		public virtual Int32? MailListID
		{
			get
			{
				return this._MailListID;
			}
			set
			{
				this._MailListID = value;
			}
		}
		#endregion

		#region CreatedByID
		public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
		protected Guid? _CreatedByID;
		[PXDBCreatedByID]
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
		[PXDBCreatedByScreenID]
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
		[PXDBCreatedDateTime]
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
		[PXDBLastModifiedByID]
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
		[PXDBLastModifiedByScreenID]
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
		[PXDBLastModifiedDateTime]
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
	}

    [Serializable]
	[PXCacheName(Messages.MassMailMembers)]
	public partial class CRMassMailMember : PXBqlTable, IBqlTable
	{
		#region MassMailID
		public abstract class massMailID : PX.Data.BQL.BqlInt.Field<massMailID> { }
		protected Int32? _MassMailID;
		[PXDBInt(IsKey = true)]
		[PXDBDefault(typeof(CRMassMail.massMailID))]
		[PXParent(typeof(Select<CRMassMail, Where<CRMassMail.massMailID, Equal<Current<CRMassMailMember.massMailID>>>>))]
		public virtual Int32? MassMailID
		{
			get
			{
				return this._MassMailID;
			}
			set
			{
				this._MassMailID = value;
			}
		}
		#endregion

		#region ContactID
		public abstract class contactID : PX.Data.BQL.BqlInt.Field<contactID> { }
		protected Int32? _ContactID;
		[PXDBInt(IsKey = true)]
		[PXSelector(typeof(Search<Contact.contactID>), DescriptionField = typeof(Contact.displayName), Filterable = true)]
		public virtual Int32? ContactID
		{
			get
			{
				return this._ContactID;
			}
			set
			{
				this._ContactID = value;
			}
		}
		#endregion

		#region CreatedByID
		public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
		protected Guid? _CreatedByID;
		[PXDBCreatedByID]
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
		[PXDBCreatedByScreenID]
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
		[PXDBCreatedDateTime]
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
		[PXDBLastModifiedByID]
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
		[PXDBLastModifiedByScreenID]
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
		[PXDBLastModifiedDateTime]
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
	}

    [Serializable]
	[PXCacheName(Messages.MassMailCampaignMember)]
	public partial class CRMassMailCampaign : PXBqlTable, IBqlTable
	{
		#region MassMailID
		public abstract class massMailID : PX.Data.BQL.BqlInt.Field<massMailID> { }
		protected Int32? _MassMailID;
		[PXDBInt(IsKey = true)]
		[PXDBDefault(typeof(CRMassMail.massMailID))]
		[PXParent(typeof(Select<CRMassMail, Where<CRMassMail.massMailID, Equal<Current<CRMassMailCampaign.massMailID>>>>))]
		public virtual Int32? MassMailID
		{
			get
			{
				return this._MassMailID;
			}
			set
			{
				this._MassMailID = value;
			}
		}
		#endregion

		#region CampaignID
		public abstract class campaignID : PX.Data.BQL.BqlString.Field<campaignID> { }
		protected string _CampaignID;
		[PXDBString(15, IsUnicode = true, IsKey = true)]
		public virtual string CampaignID
		{
			get
			{
				return this._CampaignID;
			}
			set
			{
				this._CampaignID = value;
			}
		}
		#endregion

		#region CreatedByID
		public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
		protected Guid? _CreatedByID;
		[PXDBCreatedByID]
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
		[PXDBCreatedByScreenID]
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
		[PXDBCreatedDateTime]
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
		[PXDBLastModifiedByID]
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
		[PXDBLastModifiedByScreenID]
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
		[PXDBLastModifiedDateTime]
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
	}

    [Serializable]
	[PXCacheName(Messages.MassMailMessage)]
	public partial class CRMassMailMessage : PXBqlTable, IBqlTable
	{
		#region MassMailID
		public abstract class massMailID : PX.Data.BQL.BqlInt.Field<massMailID> { }
		protected Int32? _MassMailID;
		[PXDBInt(IsKey = true)]
		[PXDBDefault(typeof(CRMassMail.massMailID))]
		[PXParent(typeof(Select<CRMassMail, Where<CRMassMail.massMailID, Equal<Current<CRMassMailMessage.massMailID>>>>))]
		public virtual Int32? MassMailID
		{
			get
			{
				return this._MassMailID;
			}
			set
			{
				this._MassMailID = value;
			}
		}
		#endregion

		#region MessageID
		public abstract class messageID : PX.Data.BQL.BqlGuid.Field<messageID> { }
		protected Guid? _MessageID;
		[PXDBGuid(IsKey = true)]
		public virtual Guid? MessageID
		{
			get
			{
				return this._MessageID;
			}
			set
			{
				this._MessageID = value;
			}
		}
		#endregion

	}

}
