using System;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CR;

namespace ACustom
{
    //skip documentation for this
    [Serializable]
    [PXCacheName("Opportunity Class Stage Reason")]
    public partial class CROpportunityClassStageReason : PXBqlTable, IBqlTable
    {
        #region Keys
        public class PK : PrimaryKeyOf<CROpportunityClassStageReason>.By<reasonID>
        {
            public static CROpportunityClassStageReason Find(PXGraph graph, int? reasonID)
            => FindBy(graph, reasonID);
        }
        public static class FK
        {
            public class OpportunityClass : PX.Objects.CR.CROpportunityClass.PK.ForeignKeyOf<CROpportunityClassStageReason>.By<classID> { }
        }
        #endregion

        #region ReasonID
        public abstract class reasonID : BqlInt.Field<reasonID> { }
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "ID")]
        public virtual int? ReasonID { get; set; }
        #endregion

        #region ClassID
        public abstract class classID : BqlString.Field<classID> { }

        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "Class ID", Visibility = PXUIVisibility.Invisible)]
        [PXDBDefault(typeof(CROpportunityClass.cROpportunityClassID))]
        [PXParent(typeof(Select<CROpportunityClass, Where<CROpportunityClass.cROpportunityClassID, Equal<Current<classID>>>>))]
        public virtual string ClassID { get; set; }
        #endregion

        #region StageCode
        public abstract class stageCode : BqlString.Field<stageCode> { }

        [PXDBString(2)]
        [PXUIField(DisplayName = "Stage", Required = true)]
        [PXDefault]
        [PXSelector(
        typeof(Search2<CROpportunityProbability.stageCode,
        InnerJoin<CROpportunityClassProbability, On<CROpportunityClassProbability.stageID, Equal<CROpportunityProbability.stageCode>>>,
        Where<CROpportunityClassProbability.classID, Equal<Current<classID>>>>),
        typeof(CROpportunityProbability.stageCode),
        typeof(CROpportunityProbability.name),
        typeof(CROpportunityProbability.probability),
        SubstituteKey = typeof(CROpportunityProbability.stageCode),
        DescriptionField = typeof(CROpportunityProbability.name))]
        public virtual string StageCode { get; set; }
        #endregion

        #region StageDescription
        public abstract class stageDescription : BqlString.Field<stageDescription> { }

        [PXString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "Stage Description", Enabled = false)]
        [PXFormula(typeof(Selector<stageCode, CROpportunityProbability.name>))]
        public virtual string StageDescription { get; set; }
        #endregion

        #region Reason
        public abstract class reason : BqlString.Field<reason> { }

        [PXDBString(2, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Reason Code")]
        public virtual string Reason { get; set; }
        #endregion

        #region ReasonDescription
        public abstract class reasonDescription : BqlString.Field<reasonDescription> { }

        [PXString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Reason Description", Enabled = false)]
        [PXUnboundDefault]
        public virtual string ReasonDescription { get; set; }
        #endregion

        #region NoteID
        public abstract class noteID : BqlGuid.Field<noteID> { }
        [PXNote]
        public virtual Guid? NoteID { get; set; }
        #endregion

        #region System Columns
        public abstract class createdByID : BqlGuid.Field<createdByID> { }
        [PXDBCreatedByID]
        public virtual Guid? CreatedByID { get; set; }

        public abstract class createdByScreenID : BqlString.Field<createdByScreenID> { }
        [PXDBCreatedByScreenID]
        public virtual string CreatedByScreenID { get; set; }

        public abstract class createdDateTime : BqlDateTime.Field<createdDateTime> { }
        [PXDBCreatedDateTime]
        public virtual DateTime? CreatedDateTime { get; set; }

        public abstract class lastModifiedByID : BqlGuid.Field<lastModifiedByID> { }
        [PXDBLastModifiedByID]
        public virtual Guid? LastModifiedByID { get; set; }

        public abstract class lastModifiedByScreenID : BqlString.Field<lastModifiedByScreenID> { }
        [PXDBLastModifiedByScreenID]
        public virtual string LastModifiedByScreenID { get; set; }

        public abstract class lastModifiedDateTime : BqlDateTime.Field<lastModifiedDateTime> { }
        [PXDBLastModifiedDateTime]
        public virtual DateTime? LastModifiedDateTime { get; set; }

        public abstract class Tstamp : BqlByteArray.Field<Tstamp> { }
        [PXDBTimestamp]
        public virtual byte[] tstamp { get; set; }
        #endregion
    }
}
