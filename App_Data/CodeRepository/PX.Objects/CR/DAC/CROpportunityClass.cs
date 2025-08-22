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

using PX.SM;
using System;
using PX.Data;
using PX.Objects.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data.BQL.Fluent;
using PX.SM.Email;

namespace PX.Objects.CR
{
	/// <exclude/>
	[PXCacheName(Messages.OpportunityClass)]
	[PXPrimaryGraph(typeof(CROpportunityClassMaint))]
	[Serializable]
	public partial class CROpportunityClass : CRBaseClass, IBqlTable, ITargetToAccount
	{
		#region Keys
		public class PK : PrimaryKeyOf<CROpportunityClass>.By<cROpportunityClassID>
		{
			public static CROpportunityClass Find(PXGraph graph, string cROpportunityClassID, PKFindOptions options = PKFindOptions.None) => FindBy(graph, cROpportunityClassID, options);
		}
		public static class FK
		{
			public class DefaultAssignmentMap : EP.EPAssignmentMap.PK.ForeignKeyOf<CROpportunityClass>.By<defaultAssignmentMapID> { }
			public class DefaultEmailAccount : PX.SM.EMailAccount.PK.ForeignKeyOf<CROpportunityClass>.By<defaultEMailAccountID> { }

			public class TargetContactClass : CR.CRContactClass.PK.ForeignKeyOf<CROpportunityClass>.By<targetContactClassID> { }
			public class TargetBusinessAccountClass : CR.CRCustomerClass.PK.ForeignKeyOf<CROpportunityClass>.By<targetBAccountClassID> { }
		}
		#endregion

		#region CROpportunityClassID
		public abstract class cROpportunityClassID : PX.Data.BQL.BqlString.Field<cROpportunityClassID> { }
		protected String _CROpportunityClassID;
		[PXSelector(typeof(CROpportunityClass.cROpportunityClassID))]
		[PXUIField(DisplayName = "Opportunity Class ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXDefault()]
		public virtual String CROpportunityClassID
		{
			get
			{
				return this._CROpportunityClassID;
			}
			set
			{
				this._CROpportunityClassID = value;
			}
		}
		#endregion
		#region Description
		public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
		protected String _Description;
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDBString(250, IsUnicode = true)]
		public virtual String Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				this._Description = value;
			}
		}
		#endregion

		#region DefaultOwner
		public abstract class defaultOwner : PX.Data.BQL.BqlInt.Field<defaultOwner> { }

		[PXDBString]
		[PXUIField(DisplayName = "Default Owner")]
		[PXDefault(CRDefaultOwnerAttribute.DoNotChange, PersistingCheck = PXPersistingCheck.Nothing)]
		[CRDefaultOwner]
		public override string DefaultOwner { get; set; }
		#endregion

		#region DefaultAssignmentMapID
		public abstract class defaultAssignmentMapID : PX.Data.BQL.BqlInt.Field<defaultAssignmentMapID> { }

		[AssignmentMap(typeof(AssignmentMapType.AssignmentMapTypeOpportunity))]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIRequired(typeof(Where<defaultOwner, Equal<CRDefaultOwnerAttribute.assignmentMap>>))]
		[PXUIEnabled(typeof(Where<defaultOwner, Equal<CRDefaultOwnerAttribute.assignmentMap>>))]
		public override int? DefaultAssignmentMapID { get; set; }
		#endregion

		#region DefaultEMailAccount
		public abstract class defaultEMailAccountID : PX.Data.BQL.BqlInt.Field<defaultEMailAccountID>
		{
			// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
			public class EmailAccountRule
				: EMailAccount.userID.PreventMakingPersonalIfUsedAsSystem<
					SelectFrom<CROpportunityClass>.Where<FK.DefaultEmailAccount.SameAsCurrent>> {}
		}

		[EmailAccountRaw(emailAccountsToShow: EmailAccountsToShowOptions.OnlySystem, DisplayName = "Default Email Account")]
		[PXForeignReference(typeof(FK.DefaultEmailAccount), Data.ReferentialIntegrity.ReferenceBehavior.SetNull)]
		public virtual int? DefaultEMailAccountID { get; set; }
		#endregion

		#region IsInternal

		public abstract class isInternal : PX.Data.BQL.BqlBool.Field<isInternal> { }
		protected Boolean? _IsInternal;
		[PXDBBool]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Internal", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Boolean? IsInternal
		{
			get
			{
				return this._IsInternal;
			}
			set
			{
				this._IsInternal = value;
			}
		}

		#endregion

        #region ShowContactActivities
        public abstract class showContactActivities : PX.Data.BQL.BqlBool.Field<showContactActivities> { }
        protected Boolean? _showContactActivities;
        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Show Activities from Source Lead")]
        public virtual Boolean? ShowContactActivities
        {
            get
            {
                return this._showContactActivities;
            }
            set
            {
                this._showContactActivities = value;
            }
        }
        #endregion
        #region NoteID
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }

		[PXNote]
		public virtual Guid? NoteID { get; set; }
		#endregion

		#region TargetContactClassID
		public abstract class targetContactClassID : PX.Data.BQL.BqlString.Field<targetContactClassID> { }

		[PXSelector(typeof(CRContactClass.classID), DescriptionField = typeof(CRContactClass.description), CacheGlobal = true)]
		[PXUIField(DisplayName = "Contact Class ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDBString(10, IsUnicode = true)]
		public virtual string TargetContactClassID { get; set; }
		#endregion

		#region TargetBAccountClassID
		public abstract class targetBAccountClassID : PX.Data.BQL.BqlString.Field<targetBAccountClassID> { }
        protected String _TargetBAccountClassID;
        [PXSelector(typeof(CRCustomerClass.cRCustomerClassID), DescriptionField = typeof(CRCustomerClass.description), CacheGlobal = true)]
        [PXUIField(DisplayName = "Account Class ID", Visibility = PXUIVisibility.SelectorVisible)]
        [PXDBString(10, IsUnicode = true)]
        public virtual String TargetBAccountClassID
        {
            get
            {
                return this._TargetBAccountClassID;
            }
            set
            {
                this._TargetBAccountClassID = value;
            }
        }
        #endregion

        #region System Columns
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
		#endregion	
	}
}
