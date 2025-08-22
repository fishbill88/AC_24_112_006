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

using PX.Data.ReferentialIntegrity.Attributes;
using System;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.AM.Attributes;
using PX.Objects.CA;

namespace PX.Objects.AM
{
	/// <summary>
	/// Bill of material attributes for both BOM and Operation level. BOM attributes are optional for a bill of material.
	/// Parent:  <see cref = "AMBomItem"/> 
	/// </summary>
	[Serializable]
    [PXPrimaryGraph(typeof(BOMAttributeMaint))]
    [PXCacheName(Messages.BOMAttributes)]
    [System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class AMBomAttribute : PXBqlTable, IBqlTable, IBomAttr
    {
        internal string DebuggerDisplay => $"BOMID = {BOMID}, RevisionID = {RevisionID}, LineNbr = {LineNbr}, Label = {Label}, OperationID = {OperationID}";

        #region Keys

        public class PK : PrimaryKeyOf<AMBomAttribute>.By<bOMID, revisionID, operationID, lineNbr>
        {
            public static AMBomAttribute Find(PXGraph graph, string bOMID, string revisionID, int? operationID, int? lineNbr, PKFindOptions options = PKFindOptions.None)
                => FindBy(graph, bOMID, revisionID, operationID, lineNbr, options);
            public static AMBomAttribute FindDirty(PXGraph graph, string bOMID, string revisionID, int? operationID, int? lineNbr)
                => PXSelect<AMBomAttribute,
                    Where<bOMID, Equal<Required<bOMID>>,
                        And<revisionID, Equal<Required<revisionID>>,
                        And<operationID, Equal<Required<operationID>>,
                        And<lineNbr, Equal<Required<lineNbr>>>>>>>
                    .SelectWindowed(graph, 0, 1, bOMID, revisionID, operationID, lineNbr);
        }

        public static class FK
        {
            public class BOM : AMBomItem.PK.ForeignKeyOf<AMBomAttribute>.By<bOMID, revisionID> { }
            public class Operation : AMBomOper.PK.ForeignKeyOf<AMBomAttribute>.By<bOMID, revisionID, operationID> { }
        }

        #endregion

        #region BOMID
        public abstract class bOMID : PX.Data.BQL.BqlString.Field<bOMID> { }
        protected string _BOMID;

		/// <summary>
		/// The ID of the bill of material.
		/// </summary>
		[BomID(IsKey = true, Visible = false, Enabled = false, Visibility = PXUIVisibility.SelectorVisible)]
        [PXDBDefault(typeof(AMBomItem.bOMID))]
        [PXParent(typeof(Select<AMBomItem,
            Where<AMBomItem.bOMID, Equal<Current<AMBomAttribute.bOMID>>,
                And<AMBomItem.revisionID, Equal<Current<AMBomAttribute.revisionID>>>>>))]
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
		/// The revision of the bill of material.
		/// </summary>
		[RevisionIDField(IsKey = true, Visibility = PXUIVisibility.SelectorVisible, Visible = false, Enabled = false)]
        [PXDBDefault(typeof(AMBomItem.revisionID))]
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
        #region LineNbr
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        protected Int32? _LineNbr;

		/// <summary>
		/// The line number.
		/// </summary>
		[PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false, Enabled = false)]
        [PXLineNbr(typeof(AMBomItem.lineCntrAttribute))]
        public virtual Int32? LineNbr
        {
            get
            {
                return this._LineNbr;
            }
            set
            {
                this._LineNbr = value;
            }
        }
        #endregion
        #region Level
        public abstract class level : PX.Data.BQL.BqlInt.Field<level> { }
        protected int? _Level;

		/// <summary>
		/// The attribute level, which is specified automatically.
		/// </summary>
		[PXDBInt]
        [PXDefault(AMAttributeLevels.BOM)]
        [PXUIField(DisplayName = "Level", Enabled = false)]
        [AMAttributeLevels.BomList]
        [PXFormula(typeof(Switch<Case<Where<AMBomAttribute.operationID, IsNull>, AMAttributeLevels.bOM>, AMAttributeLevels.operation>))]
        public virtual int? Level
        {
            get
            {
                return this._Level;
            }
            set
            {
                this._Level = value;
            }
        }
        #endregion
        #region AttributeID
        public abstract class attributeID : PX.Data.BQL.BqlString.Field<attributeID> { }
        protected string _AttributeID;

		/// <summary>
		/// The ID of the attribute.
		/// </summary>
		[PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "Attribute ID")]
        [PXDefault]
		[PXSelector(typeof(Search<CSAttribute.attributeID,
			Where<CSAttribute.controlType, NotEqual<CSAttribute.AttrType.giSelector>>>))]
        public virtual string AttributeID
        {
            get
            {
                return this._AttributeID;
            }
            set
            {
                this._AttributeID = value;
            }
        }
        #endregion
        #region OperationID
        public abstract class operationID : PX.Data.BQL.BqlInt.Field<operationID> { }
        protected int? _OperationID;

		/// <summary>
		/// The operation number from the bill of material.
		/// </summary>
		[OperationIDField]
        [OperationIDSelector(typeof(Search<AMBomOper.operationID,
            Where<AMBomOper.bOMID, Equal<Current<AMBomAttribute.bOMID>>,
                And<AMBomOper.revisionID, Equal<Current<AMBomAttribute.revisionID>>>>>),
			new[] {typeof(AMBomOper.bOMID), typeof(AMBomOper.revisionID)},
            SubstituteKey = typeof(AMBomOper.operationCD))]
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
        #region Label
        public abstract class label : PX.Data.BQL.BqlString.Field<label> { }
        protected string _Label;

		/// <summary>
		/// The unique label.
		/// </summary>
		[PXDBString(30, IsUnicode = true)]
        [PXDefault]
        [PXUIField(DisplayName = "Label", Visibility = PXUIVisibility.SelectorVisible)]
        [PXCheckUnique(typeof(AMBomAttribute.bOMID), typeof(AMBomAttribute.revisionID))]
        public virtual string Label
        {
            get
            {
                return this._Label;
            }
            set
            {
                this._Label = value;
            }
        }
        #endregion
        #region Descr
        public abstract class descr : PX.Data.BQL.BqlString.Field<descr> { }
        protected string _Descr;

		/// <summary>
		/// A description for the attribute.
		/// </summary>
		[PXDBString(256, IsUnicode = true)]
        [PXDefault]
        [PXUIField(DisplayName = "Description")]
        public virtual string Descr
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
        #region Enabled
        public abstract class enabled : PX.Data.BQL.BqlBool.Field<enabled> { }
        protected bool? _Enabled;

		/// <summary>
		/// A Boolean value that indicates (if set to <see langword="true" />) that the value can be changed in the production order and entered when reporting production.
		/// </summary>
		[PXDBBool]
        [PXDefault(true)]
        [PXUIField(DisplayName = "Enabled", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual bool? Enabled
        {
            get
            {
                return this._Enabled;
            }
            set
            {
                this._Enabled = value;
            }
        }
        #endregion
        #region TransactionRequired
        public abstract class transactionRequired : PX.Data.BQL.BqlBool.Field<transactionRequired> { }
        protected bool? _TransactionRequired;

		/// <summary>
		/// A Boolean value that indicates (if set to <see langword="true" />) that the value must be specified before the production transaction batch is released.
		/// </summary>
		[PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Transaction Required", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual bool? TransactionRequired
        {
            get
            {
                return this._TransactionRequired;
            }
            set
            {
                this._TransactionRequired = value;
            }
        }
        #endregion
        #region Value
        public abstract class value : PX.Data.BQL.BqlString.Field<value> { }
        protected string _Value;

		/// <summary>
		/// The default value that is copied onto the production order or operation.
		/// </summary>
		[PXDBString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Value")]
        [AMAttributeValue(typeof(attributeID))]
        [DynamicValueValidation(typeof(Search<CSAttribute.regExp, Where<CSAttribute.attributeID, Equal<Current<attributeID>>>>))]
		[PXDependsOnFields(typeof(AMBomAttribute.attributeID))]
        public virtual string Value
        {
            get
            {
                return this._Value;
            }
            set
            {
                this._Value = value;
            }
        }
        #endregion
        #region OrderFunction
        public abstract class orderFunction : PX.Data.BQL.BqlInt.Field<orderFunction> { }
        protected int? _OrderFunction;

		/// <summary>
		/// The field that specifies where the attribute can be used.
		/// </summary>
		[PXDBInt]
        [PXDefault(OrderTypeFunction.All)]
        [PXUIField(DisplayName = "Order Function", Visibility = PXUIVisibility.SelectorVisible)]
        [OrderTypeFunction.ListAll]
        public virtual int? OrderFunction
        {
            get
            {
                return this._OrderFunction;
            }
            set
            {
                this._OrderFunction = value;
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
        #region CreatedByScreenID
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        protected string _CreatedByScreenID;
        [PXDBCreatedByScreenID]
        public virtual string CreatedByScreenID
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
        #region LastModifiedByScreenID
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        protected string _LastModifiedByScreenID;
        [PXDBLastModifiedByScreenID]
        public virtual string LastModifiedByScreenID
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
        #region RowStatus
        public abstract class rowStatus : PX.Data.BQL.BqlInt.Field<rowStatus> { }
        protected int? _RowStatus;

		/// <summary>
		/// The row status.
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
        #region tstamp
        public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }
        protected byte[] _tstamp;
        [PXDBTimestamp]
        public virtual byte[] tstamp
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
    }
}
