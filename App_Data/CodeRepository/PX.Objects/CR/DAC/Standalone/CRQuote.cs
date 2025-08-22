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

using PX.Data.EP;
using System;
using PX.Data;
using PX.Objects.CS;


namespace PX.Objects.CR.Standalone
{
    public partial class CRQuote : PXBqlTable, PX.Data.IBqlTable
    {
        #region Selected
        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }

        [PXBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Selected", Visibility = PXUIVisibility.Service)]
        public virtual bool? Selected { get; set; }
        #endregion

		#region QuoteID
		public abstract class quoteID : PX.Data.BQL.BqlGuid.Field<quoteID> { }
		[PXDBGuid(IsKey = true)]
		[PXDefault(typeof(Standalone.CROpportunityRevision.noteID))]
		public virtual Guid? QuoteID { get; set; }
		#endregion

        #region QuoteNbr
        public abstract class quoteNbr : PX.Data.BQL.BqlString.Field<quoteNbr> { }
        protected String _QuoteNbr;
        [AutoNumber(typeof(CRSetup.quoteNumberingID), typeof(AccessInfo.businessDate))]
        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXDefault]
        [PXUIField(DisplayName = "Quote Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
        [PX.Data.EP.PXFieldDescription]
        public virtual String QuoteNbr
        {
            get
            {
                return this._QuoteNbr;
            }
            set
            {
                this._QuoteNbr = value;
            }
        }
		#endregion

        #region QuoteType
        public abstract class quoteType : PX.Data.BQL.BqlString.Field<quoteType> { }

        [PXDBString(1, IsFixed = true)]
        [PXUIField(DisplayName = "Type", Visible = true)]
        [CRQuoteType()]
        [PXDefault(CRQuoteTypeAttribute.Distribution)]
        public virtual string QuoteType { get; set; }
        #endregion

		#region Subject
	    public abstract class subject : PX.Data.BQL.BqlString.Field<subject> { }
	    [PXDBString(255, IsUnicode = true)]
	    [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
	    [PXUIField(DisplayName = "Subject", Visibility = PXUIVisibility.SelectorVisible)]
	    [PXFieldDescription]
	    public virtual String Subject { get; set; }
	    #endregion

		#region ExpirationDate
		public abstract class expirationDate : PX.Data.BQL.BqlDateTime.Field<expirationDate> { }

        [PXDBDate()]        
        [PXUIField(DisplayName = "Expiration Date", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual DateTime? ExpirationDate { get; set; }
        #endregion       
        
        #region Status
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }

        [PXDBString(1, IsFixed = true)]
        [PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible)]
        [CRQuoteStatus()]        
        [PXDefault]
        public virtual string Status { get; set; }
		#endregion
				
		#region NoteID
		public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        [PXNote(
            DescriptionField = typeof(CRQuote.quoteNbr),
            Selector = typeof(CRQuote.quoteNbr)
        )]
        public virtual Guid? NoteID { get; set; }
        #endregion

        #region tstamp
        public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }

        [PXDBTimestamp()]
        public virtual Byte[] tstamp { get; set; }
		#endregion

		#region CreatedByScreenID
		public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }

        [PXDBCreatedByScreenID()]
        public virtual String CreatedByScreenID { get; set; }
        #endregion

        #region CreatedByID
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }

        [PXDBCreatedByID()]
        [PXUIField(DisplayName = "Created By")]
        public virtual Guid? CreatedByID { get; set; }
        #endregion

        #region CreatedDateTime
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }

        [PXDBCreatedDateTime]
        [PXUIField(DisplayName = "Date Created", Enabled = false)]
        public virtual DateTime? CreatedDateTime { get; set; }
        #endregion

        #region LastModifiedByID
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }

        [PXDBLastModifiedByID()]
        [PXUIField(DisplayName = "Last Modified By")]
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
        [PXUIField(DisplayName = "Last Modified Date", Enabled = false)]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        #endregion    


    }
}
