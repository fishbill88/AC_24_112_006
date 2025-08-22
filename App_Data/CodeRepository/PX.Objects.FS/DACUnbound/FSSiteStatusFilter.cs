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

using PX.Data;
using System;
using PX.Objects.IN;
using PX.Objects.SO;
using PX.Objects.Common.Bql;
using PX.Objects.Common;

namespace PX.Objects.FS
{
    [Serializable]
    public partial class FSSiteStatusFilter : INSiteStatusFilter
    {
        #region SiteID
        public new abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID> { }

        [PXUIField(DisplayName = "Warehouse")]
        [SiteAttribute]
        [InterBranchRestrictor(typeof(Where2<
                                        Where<SameOrganizationBranch<INSite.branchID, Current<FSServiceOrder.branchID>>>,
                                            Or<SameOrganizationBranch<INSite.branchID, Current<FSSchedule.branchID>>>>))]
        [PXDefault(typeof(INRegister.siteID), PersistingCheck = PXPersistingCheck.Nothing)]
        public override Int32? SiteID
        {
            get
            {
                return this._SiteID;
            }
            set
            {
                this._SiteID = value;
            }
        }
        #endregion
        #region Inventory
        public new abstract class inventory : PX.Data.BQL.BqlString.Field<inventory> { }
        #endregion
        #region Mode
        public abstract class mode : PX.Data.BQL.BqlInt.Field<mode> { }
        protected int? _Mode;
        [PXDBInt]
        [PXDefault(0)]
        [PXUIField(DisplayName = "Selection Mode")]
        [SOAddItemMode.List]
        public virtual int? Mode
        {
            get
            {
                return _Mode;
            }
            set
            {
                _Mode = value;
            }
        }
        #endregion
        #region HistoryDate
        public abstract class historyDate : PX.Data.BQL.BqlDateTime.Field<historyDate> { }
        protected DateTime? _HistoryDate;
        [PXDBDate()]
        [PXUIField(DisplayName = "Sold Since")]
        public virtual DateTime? HistoryDate
        {
            get
            {
                return this._HistoryDate;
            }
            set
            {
                this._HistoryDate = value;
            }
        }
        #endregion
        #region OnlyAvailable
        public new abstract class onlyAvailable : PX.Data.BQL.BqlBool.Field<onlyAvailable> { }
        [PXBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Available Stock Items Only")]
        public override bool? OnlyAvailable { get; set; }
        #endregion
        #region IncludeIN
        public abstract class includeIN : PX.Data.BQL.BqlBool.Field<includeIN> { }
        [PXBool]
        [PXUIField(DisplayName = "Show Stock Items")]
        public virtual bool? IncludeIN { get; set; }
        #endregion
        #region LineType
        public abstract class lineType : PX.Data.BQL.BqlString.Field<lineType>
        {
        }

        [PXString(5, IsFixed = true)]
        [PXUIField(DisplayName = "Line Type")]
        [FSLineType.List]
        [PXDefault(ID.LineType_InvLookup.ALL)]
        public virtual string LineType { get; set; }
        #endregion
    }
}

