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
using PX.Objects.AM.Attributes;

namespace PX.Objects.AM
{
    /// <summary>
    /// The BOM material reference designators.
    /// </summary>
    [Serializable]
    [PXCacheName(Messages.BOMRef)]
    public class AMBomRef : PXBqlTable, IBqlTable, IBomOper, INotable, IBomMatlDetail
    {
        #region Keys

        public class PK : PrimaryKeyOf<AMBomRef>.By<bOMID, revisionID, operationID, lineID>
        {
            public static AMBomRef Find(PXGraph graph, string bOMID, string revisionID, int? operationID, int? lineID, PKFindOptions options = PKFindOptions.None)
                => FindBy(graph, bOMID, revisionID, operationID, lineID, options);
            public static AMBomRef FindDirty(PXGraph graph, string bOMID, string revisionID, int? operationID, int? lineID)
                => PXSelect<AMBomRef,
                    Where<bOMID, Equal<Required<bOMID>>,
                        And<revisionID, Equal<Required<revisionID>>,
                        And<operationID, Equal<Required<operationID>>,
                        And<lineID, Equal<Required<lineID>>>>>>>
                    .SelectWindowed(graph, 0, 1, bOMID, revisionID, operationID, lineID);
        }

        public static class FK
        {
            public class BOM : AMBomItem.PK.ForeignKeyOf<AMBomRef>.By<bOMID, revisionID> { }
            public class OperationMatl : AMBomMatl.PK.ForeignKeyOf<AMBomRef>.By<bOMID, revisionID, operationID, matlLineID> { }
        }

        #endregion

        #region BOMID
        public abstract class bOMID : PX.Data.BQL.BqlString.Field<bOMID> { }

		protected string _BOMID;

		/// <summary>
		/// The identifier of the bill of material.
		/// </summary>
		[BomID(IsKey = true, Visible = false, Enabled = false)]
        [PXDBDefault(typeof(AMBomMatl.bOMID))]
		public virtual string BOMID
		{
			get
			{
				return this._BOMID;
			}
			set
			{
				this._BOMID = value;
			}
		}
        #endregion
	    #region RevisionID
	    public abstract class revisionID : PX.Data.BQL.BqlString.Field<revisionID> { }

	    protected string _RevisionID;

		/// <summary>
		/// The identifier of the BOM revision, which is the modification of the bill of material.
		/// </summary>
		[RevisionIDField(IsKey = true, Visibility = PXUIVisibility.SelectorVisible, Visible = false, Enabled = false)]
        [PXDBDefault(typeof(AMBomMatl.revisionID))]
	    public virtual string RevisionID
	    {
	        get
	        {
	            return this._RevisionID;
	        }
	        set
	        {
	            this._RevisionID = value;
	        }
	    }
	    #endregion
	    #region OperationID
	    public abstract class operationID : PX.Data.BQL.BqlInt.Field<operationID> { }

	    protected int? _OperationID;

		/// <summary>
		/// The numeric identifier of the operation.
		/// </summary>
		[OperationIDField(IsKey = true, Visible = false, Enabled = false)]
        [PXDefault(typeof(AMBomMatl.operationID))]
	    public virtual int? OperationID
	    {
	        get
	        {
	            return this._OperationID;
	        }
	        set
	        {
	            this._OperationID = value;
	        }
	    }
	    #endregion
		#region MatlLineID
		public abstract class matlLineID : PX.Data.BQL.BqlInt.Field<matlLineID> { }

		protected Int32? _MatlLineID;

		/// <summary>
		/// The material line ID.
		/// </summary>
		[PXDBInt(IsKey = true)]
        [PXDBDefault(typeof(AMBomMatl.lineID))]
		[PXUIField(DisplayName = "Material Line ID", Enabled = false, Visible = false)]
        [PXParent(typeof(Select<AMBomMatl, 
            Where<AMBomMatl.bOMID, Equal<Current<AMBomRef.bOMID>>,
                And<AMBomMatl.revisionID, Equal<Current<AMBomRef.revisionID>>,
                And<AMBomMatl.operationID, Equal<Current<AMBomRef.operationID>>, 
                And<AMBomMatl.lineID, Equal<Current<AMBomRef.matlLineID>>>>>>>)
                , UseCurrent = true)]
		public virtual Int32? MatlLineID
		{
			get
			{
				return this._MatlLineID;
			}
			set
			{
				this._MatlLineID = value;
			}
		}
        #endregion
        #region LineID
        public abstract class lineID : PX.Data.BQL.BqlInt.Field<lineID> { }

        protected Int32? _LineID;

		/// <summary>
		/// The line ID.
		/// </summary>
		[PXDBInt(IsKey = true)]
        [PXLineNbr(typeof(AMBomMatl.lineCntrRef))]
        [PXUIField(DisplayName = "Line ID", Visible = false, Enabled = false)]
        public virtual Int32? LineID
        {
            get
            {
                return this._LineID;
            }
            set
            {
                this._LineID = value;
            }
        }
        #endregion
        #region Descr
        public abstract class descr : PX.Data.BQL.BqlString.Field<descr> { }

		protected String _Descr;

		/// <summary>
		/// A description.
		/// </summary>
		[PXDBString(256, IsUnicode = true)]
		[PXDefault("")]
		[PXUIField(DisplayName = "Description")]
		public virtual String Descr
		{
			get
			{
				return this._Descr;
			}
			set
			{
				this._Descr = value;
			}
		}
        #endregion
        #region NoteID
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        protected Guid? _NoteID;
        [PXNote]
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
		#region RefDes
		public abstract class refDes : PX.Data.BQL.BqlString.Field<refDes> { }

		protected String _RefDes;

		/// <summary>
		/// A reference description.
		/// </summary>
		[PXDBString(30, IsUnicode = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
		[PXUIField(DisplayName = "Ref Des")]
		public virtual String RefDes
		{
			get
			{
				return this._RefDes;
			}
			set
			{
				this._RefDes = value;
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
        #region RowStatus
        public abstract class rowStatus : PX.Data.BQL.BqlInt.Field<rowStatus> { }
        protected int? _RowStatus;

		/// <summary>
		/// The change status.
		/// </summary>
		[PXDBInt]
        [PXUIField(DisplayName = "Change Status", Enabled = false)]
        [AMRowStatus.List]
        public virtual int? RowStatus
        {
            get
            {
                return this._RowStatus;
            }
            set
            {
                this._RowStatus = value;
            }
        }
        #endregion
    }
}
