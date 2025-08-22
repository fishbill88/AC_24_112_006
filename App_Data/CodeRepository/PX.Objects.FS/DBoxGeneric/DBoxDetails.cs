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
using PX.Objects.CR;

namespace PX.Objects.FS
{
    public class DBoxDetails
    {
        #region SourceNoteID
        public virtual Guid? SourceNoteID { get; set; }
        #endregion
        #region LineType
        public virtual string LineType { get; set; }
        #endregion
        #region InventoryID
        public virtual int? InventoryID { get; set; }
        #endregion
		#region UOM
		public virtual string UOM { get; set; }
		#endregion
        #region IsFree
        public virtual bool? IsFree { get; set; }
        #endregion
        #region BillingRule
        public virtual string BillingRule { get; set; }
        #endregion
        #region TranDesc
        public virtual string TranDesc { get; set; }
        #endregion
        #region SiteID
        public virtual int? SiteID { get; set; }
        #endregion
        #region EstimatedDuration
        public virtual int? EstimatedDuration { get; set; }
        #endregion
        #region EstimatedQty
        public virtual decimal? EstimatedQty { get; set; }
        #endregion
        #region CuryUnitPrice
        public virtual decimal? CuryUnitPrice { get; set; }
        #endregion
        #region ManualPrice
        public virtual bool? ManualPrice { get; set; }
        #endregion
        #region ProjectID
        public virtual int? ProjectID { get; set; }
        #endregion
        #region ProjectTaskID
        public virtual int? ProjectTaskID { get; set; }
        #endregion
        #region CostCodeID
        public virtual int? CostCodeID { get; set; }
        #endregion
        #region CuryUnitCost
        public virtual decimal? CuryUnitCost { get; set; }
        #endregion
        #region ManualCost
        public virtual bool? ManualCost { get; set; }
        #endregion
        #region EnablePO
        public virtual bool? EnablePO { get; set; }
        #endregion
        #region POVendorID
        public virtual int? POVendorID { get; set; }
        #endregion
        #region POVendorLocationID
        public virtual int? POVendorLocationID { get; set; }
        #endregion
        #region TaxCategoryID
        public virtual string TaxCategoryID { get; set; }
        #endregion
        #region DiscPct
        public virtual decimal? DiscPct { get; set; }
        #endregion
        #region CuryDiscAmt
        public virtual Decimal? CuryDiscAmt { get; set; }
        #endregion
        #region CuryBillableExtPrice
        public virtual Decimal? CuryBillableExtPrice { get; set; }
		#region POCreate
		public virtual bool? POCreate { get; set; }
		#endregion
        #endregion

        public object sourceLine;

        public static implicit operator DBoxDetails(CROpportunityProducts line)
        {
            if (line == null)
            {
                return null;
            }

            DBoxDetails ret = new DBoxDetails();

            ret.SourceNoteID = line.NoteID;
            ret.LineType = line.LineType;
            ret.InventoryID = line.InventoryID;
			ret.UOM = line.UOM;
            ret.IsFree = line.IsFree;
            ret.TranDesc = line.Descr;
            ret.SiteID = line.SiteID;
            ret.EstimatedQty = line.Qty;
            ret.CuryUnitPrice = line.CuryUnitPrice;
            ret.ManualPrice = line.ManualPrice;
            ret.ProjectID = line.ProjectID;
            ret.ProjectTaskID = line.TaskID;
            ret.CostCodeID = line.CostCodeID;
            ret.CuryUnitCost = line.CuryUnitCost;
            ret.ManualCost = line.POCreate;
            ret.EnablePO = line.POCreate;
            ret.POVendorID = line.VendorID;
            ret.TaxCategoryID = line.TaxCategoryID;
            ret.DiscPct = line.DiscPct;
            ret.CuryDiscAmt = line.CuryDiscAmt;
            ret.CuryBillableExtPrice = line.CuryExtPrice;
            ret.POCreate = line.POCreate;

            ret.sourceLine = line;

            return ret;
        }
    }
}
