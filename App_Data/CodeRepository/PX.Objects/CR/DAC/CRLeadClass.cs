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
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.EP;
using PX.SM;
using PX.Data.BQL.Fluent;
using PX.SM.Email;

namespace PX.Objects.CR
{
	/// <exclude/>
	[PXCacheName(Messages.LeadClass)]
	[PXPrimaryGraph(typeof(CRLeadClassMaint))]
	[Serializable]
	public class CRLeadClass : CRBaseClass, IBqlTable, ITargetToContact, ITargetToAccount, ITargetToOpportunity
	{
		#region Keys
		public class PK : PrimaryKeyOf<CRLeadClass>.By<classID>
		{
			public static CRLeadClass Find(PXGraph graph, string classID, PKFindOptions options = PKFindOptions.None) => FindBy(graph, classID, options);
		}
		public static class FK
		{
			public class DefaultAssignmentMap : EP.EPAssignmentMap.PK.ForeignKeyOf<CRLeadClass>.By<defaultAssignmentMapID> { }
			public class DefaultEmailAccount : PX.SM.EMailAccount.PK.ForeignKeyOf<CRLeadClass>.By<defaultEMailAccountID> { }

			public class TargetContactClass : CR.CRContactClass.PK.ForeignKeyOf<CRLeadClass>.By<targetContactClassID> { }
			public class TargetBusinessAccountClass : CR.CRCustomerClass.PK.ForeignKeyOf<CRLeadClass>.By<targetBAccountClassID> { }
			public class TargetOpportunityClass : CR.CROpportunityClass.PK.ForeignKeyOf<CRLeadClass>.By<targetOpportunityClassID> { }
		}
		#endregion

		#region ClassID
		public abstract class classID : PX.Data.BQL.BqlString.Field<classID> { }

		[PXSelector(typeof(CRLeadClass.classID))]
		[PXUIField(DisplayName = "Class ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXDefault]
		public virtual string ClassID { get; set; }
		#endregion

		#region IsInternal

		public abstract class isInternal : PX.Data.BQL.BqlBool.Field<isInternal> { }

		[PXDBBool]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Internal", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Boolean? IsInternal { get; set; }

		#endregion

		#region Description
		public abstract class description : PX.Data.BQL.BqlString.Field<description> { }

		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDBString(250, IsUnicode = true)]
		public virtual string Description { get; set; }
		#endregion

		#region DefaultSource
		public abstract class defaultSource : PX.Data.BQL.BqlString.Field<defaultSource> { }

		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Default Source")]
		[CRMSources(BqlField = typeof(CRLead.source))]
		public virtual string DefaultSource { get; set; }
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

		[AssignmentMap(typeof(AssignmentMapType.AssignmentMapTypeLead))]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIRequired(typeof(Where<defaultOwner, Equal<CRDefaultOwnerAttribute.assignmentMap>>))]
		[PXUIEnabled(typeof(Where<defaultOwner, Equal<CRDefaultOwnerAttribute.assignmentMap>>))]
		public override int? DefaultAssignmentMapID { get; set; }
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

		[PXSelector(typeof(CRCustomerClass.cRCustomerClassID), DescriptionField = typeof(CRCustomerClass.description), CacheGlobal = true)]
		[PXUIField(DisplayName = "Account Class ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDBString(10, IsUnicode = true)]
		public virtual string TargetBAccountClassID { get; set; }
		#endregion

		#region RequireBAccountCreation
		public abstract class requireBAccountCreation : PX.Data.BQL.BqlBool.Field<requireBAccountCreation> { }

		[PXDBBool]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Require Account for Conversion to Opportunity", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual bool? RequireBAccountCreation { get; set; }
		#endregion

		#region TargetOpportunityClassID
		public abstract class targetOpportunityClassID : PX.Data.BQL.BqlString.Field<targetOpportunityClassID> { }

		[PXSelector(typeof(CROpportunityClass.cROpportunityClassID), DescriptionField = typeof(CROpportunityClass.description), CacheGlobal = true)]
		[PXUIField(DisplayName = "Opportunity Class ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDBString(10, IsUnicode = true)]
		public virtual string TargetOpportunityClassID { get; set; }
		#endregion

		#region TargetOpportunityStage
		public abstract class targetOpportunityStage : PX.Data.BQL.BqlString.Field<targetOpportunityStage> { }

		[PXDBString(2)]
		[PXUIField(DisplayName = "Opportunity Stage")]
		[CROpportunityStages(typeof(targetOpportunityClassID), OnlyActiveStages = true)]
		[PXFormula(typeof(Switch<Case<Where<CRLeadClass.targetOpportunityClassID.IsNull>, Null>, CRLeadClass.targetOpportunityStage>))]
		[PXUIEnabled(typeof(Where<CRLeadClass.targetOpportunityClassID.IsNotNull>))]
		public virtual string TargetOpportunityStage { get; set; }
		#endregion

		#region DefaultEMailAccount
		public abstract class defaultEMailAccountID : PX.Data.BQL.BqlInt.Field<defaultEMailAccountID>
		{
			// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
			public class EmailAccountRule
				: EMailAccount.userID.PreventMakingPersonalIfUsedAsSystem<
					SelectFrom<CRLeadClass>.Where<FK.DefaultEmailAccount.SameAsCurrent>> {}
		}

		[EmailAccountRaw(emailAccountsToShow: EmailAccountsToShowOptions.OnlySystem, DisplayName = "Default Email Account")]
		[PXForeignReference(typeof(FK.DefaultEmailAccount), Data.ReferentialIntegrity.ReferenceBehavior.SetNull)]
		public virtual int? DefaultEMailAccountID { get; set; }
		#endregion

		#region NoteID
		public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }

		[PXNote]
		[PXUIField(Visible = false, Visibility = PXUIVisibility.Invisible)]
		public virtual Guid? NoteID { get; set; }
		#endregion

		#region System Columns
		#region tstamp
		public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }

		[PXDBTimestamp()]
		public virtual Byte[] tstamp { get; set; }
		#endregion
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

		[PXDBCreatedDateTime()]
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

		[PXDBLastModifiedDateTime()]
		public virtual DateTime? LastModifiedDateTime { get; set; }
		#endregion
		#endregion
	}

	/// <exclude/>
	[PXHidden]
	public partial class CRBaseClass : PXBqlTable
	{
		public virtual string DefaultOwner { get; set; }
		public virtual int? DefaultAssignmentMapID { get; set; }
	}

	/// <exclude/>
	public interface ITargetToLead
	{
		string TargetLeadClassID { get; set; }
	}
	/// <exclude/>
	public interface ITargetToContact
	{
		string TargetContactClassID { get; set; }
	}
	/// <exclude/>
	public interface ITargetToAccount
	{
		string TargetBAccountClassID { get; set; }
	}
	/// <exclude/>
	public interface ITargetToOpportunity
	{
		string TargetOpportunityClassID { get; set; }
	}
}
