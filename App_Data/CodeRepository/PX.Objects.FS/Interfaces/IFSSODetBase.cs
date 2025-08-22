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

namespace PX.Objects.FS
{
    public enum FieldType
    {
        EstimatedField,
        ActualField,
        BillableField
    }

    public interface IFSSODetBase
    {
        int? DocID { get; }

        int? LineID { get; }

        int? LineNbr { get; }

        string LineRef { get; set; }

        int? BranchID { get; set; }

        long? CuryInfoID { get; set; }

        string LineType { get; set; }

        bool? IsPrepaid { get; set; }

        int? InventoryID { get; set; }

        int? SubItemID { get; set; }

        string UOM { get; set; }

        string TranDesc { get; set; }

        bool? ManualPrice { get; set; }

        bool? IsBillable { get; set; }

        bool? IsFree { get; set; }

        string BillingRule { get; set; }

        decimal? CuryUnitPrice { get; set; }

        decimal? UnitPrice { get; set; }

        decimal? CuryBillableExtPrice { get; set; }

        decimal? BillableExtPrice { get; set; }

        int? SiteID { get; set; }

        int? SiteLocationID { get; set; }

        int? EstimatedDuration { get; set; }

        decimal? EstimatedQty { get; set; }

        decimal? Qty { get; set; }

        int? GetDuration(FieldType fieldType);

        int? GetApptDuration();

        decimal? GetQty(FieldType fieldType);

        decimal? GetApptQty();

        void SetDuration(FieldType fieldType, int? duration, PXCache cache, bool raiseEvents);

        void SetQty(FieldType fieldType, decimal? qty, PXCache cache, bool raiseEvents);

        decimal? GetTranAmt(FieldType fieldType);

        int? GetPrimaryDACDuration();

        decimal? GetPrimaryDACQty();

        decimal? GetPrimaryDACTranAmt();

        int? ProjectID { get; set; }

        int? ProjectTaskID { get; set; }

        int? CostCodeID { get; set; }

        int? AcctID { get; set; }

        int? SubID { get; set; }

        string Status { get; set; }

        string EquipmentAction { get; set; }

        int? SMEquipmentID { get; set; }

        int? ComponentID { get; set; }

        int? EquipmentLineRef {get; set;}

        bool? Warranty { get; set; }

        bool IsService { get; }

        bool IsInventoryItem { get; }

        bool? ContractRelated { get; set; }

        bool? EnablePO { get; set; }
        string POType { get; set; }
        string PONbr { get; set; }
        string POStatus { get; set; }
        bool? POCompleted { get; set; }
		int? POLineNbr { get; set; }
        string POSource { get; set; }
        int? POVendorID { get; set; }
        int? POVendorLocationID { get; set; }

        decimal? UnitCost { get; set; }

        decimal? CuryUnitCost { get; set; }

        decimal? ExtCost { get; set; }

        decimal? CuryExtCost { get; set; }

        String TaxCategoryID { get; set; }

        String LotSerialNbr { get; set; }

        decimal? DiscPct { get; set; }

        decimal? CuryDiscAmt { get; set; }

        string LinkedEntityType { get; set; }

        string LinkedDocType { get; set; }

        string LinkedDocRefNbr { get; set; }

        int? LinkedLineNbr { get; set; }

        bool IsExpenseReceiptItem { get; }

        bool IsAPBillItem { get; }

        bool IsLinkedItem { get; }
    }
}
