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

namespace PX.Objects.WZ
{
    [Serializable]
	[PXHidden]
	public partial class WZTaskFeature : PXBqlTable, IBqlTable
    {
        #region ScenarioID
        public abstract class scenarioID : PX.Data.BQL.BqlGuid.Field<scenarioID> { }

        protected Guid? _ScenarioID;

        [PXDBGuid(IsKey = true)]
        [PXParent(typeof(Select<WZScenario, Where<WZScenario.scenarioID, Equal<Current<WZTaskFeature.scenarioID>>>>))]
        public virtual Guid? ScenarioID
        {
            get
            {
                return this._ScenarioID;
            }
            set
            {
                this._ScenarioID = value;
            }
        }
        #endregion
        #region TaskID
        public abstract class taskID : PX.Data.BQL.BqlGuid.Field<taskID> { }

        protected Guid? _TaskID;

        [PXDBGuid(IsKey = true)]
        [PXUIField(Visible = false, Visibility = PXUIVisibility.Invisible)]
        [PXParent(typeof(Select<WZTask, Where<WZTask.taskID, Equal<Current<WZTaskFeature.taskID>>>>))]
        public virtual Guid? TaskID
        {
            get
            {
                return this._TaskID;
            }
            set
            {
                this._TaskID = value;
            }
        }
        #endregion

        #region Required

        public abstract class required : PX.Data.BQL.BqlBool.Field<required> { }

        protected bool? _Required;

        [PXBool]
        [PXUIField(DisplayName = "Required")]
        public virtual bool? Required
        {
            get
            {
                return this._Required;
            }
            set
            {
                this._Required = value;
            }
        }

        #endregion

        #region Feature
        public abstract class feature : PX.Data.BQL.BqlString.Field<feature> { }

        protected String _Feature;
        [PXDBString(50, IsUnicode = true, IsKey = true)]
        public virtual String Feature
        {
            get
            {
                return this._Feature;
            }
            set
            {
                this._Feature = value;
            }

        }
        #endregion
        #region DisplayName
        public abstract class displayName : PX.Data.BQL.BqlString.Field<displayName> { }

        protected String _DisplayName;
        [PXString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Feature", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual String DisplayName
        {
            get
            {
                return this._DisplayName;
            }
            set
            {
                this._DisplayName = value;
            }

        }
        #endregion

        #region Order
        public abstract class order : PX.Data.BQL.BqlInt.Field<order> { }

        protected Int32? _Order;

        [PXInt]
        public virtual Int32? Order
        {
            get
            {
                return this._Order;
            }
            set
            {
                this._Order = value;
            }
        }
        #endregion
        #region Offset
        public abstract class offset : PX.Data.BQL.BqlInt.Field<offset> { }

        protected Int32? _Offset;

        [PXInt]
        public virtual Int32? Offset
        {
            get
            {
                return this._Offset;
            }
            set
            {
                this._Offset = value;
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
    }
}
