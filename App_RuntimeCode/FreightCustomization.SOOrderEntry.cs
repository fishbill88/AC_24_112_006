using PX.Data;
using PX.Data.Licensing;
using PX.Objects.CA.Light;
using PX.Objects.PO;
using PX.Objects.SO;

namespace POInventoryCustomization
{
    public class SOOrderEntry_Extension : PXGraphExtension<PX.Objects.SO.SOOrderEntry>
    {
        #region Event Handlers
        protected virtual void SOOrder_OrderQty_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            SOOrder row = (SOOrder)e.Row;
            if (row == null) return;
            SOOrderExt rowExt = row.GetExtension<SOOrderExt>();
            rowExt.UsrRTHOrderQty = row.OrderQty;
        }

        protected virtual void _(Events.RowSelected<SOOrder> e, PXRowSelected baseMethod)
        {
            baseMethod?.Invoke(e.Cache, e.Args);
            SOOrder order = (SOOrder)e.Row;
            if (order == null) return;

            SOOrderType orderType = SOOrderType.PK.Find(Base, order.OrderType);
            SOOrderTypeExt typeExt = orderType.GetExtension<SOOrderTypeExt>();


            PXUIFieldAttribute.SetVisible<SOLineExt.usrVendorID>(Base.Transactions.Cache, null, typeExt?.UsrShowVendorID ?? false);
            PXUIFieldAttribute.SetVisible<SOLineExt.usrVendorLocationID>(Base.Transactions.Cache, null, typeExt?.UsrShowVendorLocationID ?? false);
            PXUIFieldAttribute.SetVisible<SOLineExt.usrVendorAddress>(Base.Transactions.Cache, null, typeExt?.UsrShowVendorAddress ?? false);
            PXUIFieldAttribute.SetVisibility<SOLineExt.usrVendorID>(Base.Transactions.Cache, null,
                (typeExt?.UsrShowVendorID ?? false) ? PXUIVisibility.Visible : PXUIVisibility.Invisible);
            PXUIFieldAttribute.SetVisibility<SOLineExt.usrVendorLocationID>(Base.Transactions.Cache, null,
                (typeExt?.UsrShowVendorLocationID ?? false) ? PXUIVisibility.Visible : PXUIVisibility.Invisible);

            PXUIFieldAttribute.SetVisibility<SOLineExt.usrVendorAddress>(Base.Transactions.Cache, null,
                (typeExt?.UsrShowVendorAddress ?? false) ? PXUIVisibility.Visible : PXUIVisibility.Invisible);
        }

        //protected virtual void _(Events.FieldUpdated<SOLine.inventoryID> e, PXFieldUpdated baseMethod)
        //{
        //    baseMethod?.Invoke(e.Cache, e.Args);
        //    SOLine line = (SOLine)e.Row;
        //    if (line == null) return;

        //    POVendorInventory defaultVendor = null;
        //    foreach (POVendorInventory defVen in PXSelect<POVendorInventory,
        //        Where<POVendorInventory.inventoryID, Equal<Required<POVendorInventory.inventoryID>>>>
        //        .Select(Base, line.InventoryID))
        //    {
        //        if (defVen.IsDefault == true)
        //        {
        //            defaultVendor = defVen;
        //            break;
        //        }
        //    }

        //    Location location = PXSelect<Location,
        //        Where<Location.bAccountID, Equal<Required<Location.bAccountID>>,
        //            And<Location.locationID, Equal<Required<Location.locationID>>>>>
        //        .Select(Base, defaultVendor?.VendorID, defaultVendor?.VendorLocationID);


        //    SOLineExt lineExt = line.GetExtension<SOLineExt>();
        //    lineExt.UsrVendorID = defaultVendor?.VendorID;
        //    lineExt.UsrVendorLocationID = defaultVendor?.VendorLocationID;

        //    Address address = PXSelect<Address,
        //        Where<Address.bAccountID, Equal<Required<Address.bAccountID>>>>
        //        .Select(Base, defaultVendor?.VendorID);

        //    if (address != null)
        //    {
        //        lineExt.UsrVendorAddress = string.Format("{0}{1}{2}, {3} {4}",
        //            address.AddressLine1 ?? "",
        //            string.IsNullOrWhiteSpace(address.AddressLine2) ? "" : " " + address.AddressLine2,
        //            string.IsNullOrWhiteSpace(address.City) ? "" : ", " + address.City,
        //            address.State ?? "",
        //            address.PostalCode ?? ""
        //        ).Trim();
        //    }
        //    else
        //    {
        //        lineExt.UsrVendorAddress = null;
        //    }
        //}


        protected virtual void _(Events.FieldUpdated<SOLineExt.usrVendorID> e, PXFieldUpdated baseMethod)
        {
            baseMethod?.Invoke(e.Cache, e.Args);
            SOLine line = (SOLine)e.Row;
            if (line == null) return;

            POVendorInventory vendorInv = PXSelect<POVendorInventory,
                Where<POVendorInventory.inventoryID, Equal<Required<SOLine.inventoryID>>,
                    And<POVendorInventory.vendorID, Equal<Required<SOLine.vendorID>>>>>
                .Select(Base, line.InventoryID, line.VendorID);

            SOLineExt lineExt = line.GetExtension<SOLineExt>();
            lineExt.UsrVendorLocationID = vendorInv?.VendorLocationID;

            Location location = PXSelect<Location,
                Where<Location.bAccountID, Equal<Required<Location.bAccountID>>,
                    And<Location.locationID, Equal<Required<Location.locationID>>>>>
                .Select(Base, vendorInv?.VendorID, vendorInv?.VendorLocationID);
            Address address = PXSelect<Address,
                Where<Address.bAccountID, Equal<Required<Address.bAccountID>>>>
                .Select(Base, vendorInv?.VendorID);

            if (address != null)
            {
                lineExt.UsrVendorAddress = string.Format("{0}{1}{2}, {3} {4}",
                    address.AddressLine1 ?? "",
                    string.IsNullOrWhiteSpace(address.AddressLine2) ? "" : " " + address.AddressLine2,
                    string.IsNullOrWhiteSpace(address.City) ? "" : ", " + address.City,
                    address.State ?? "",
                    address.PostalCode ?? ""
                ).Trim();
            }
            else
            {
                lineExt.UsrVendorAddress = null;
            }
        }

        protected virtual void _(Events.FieldUpdated<SOLineExt.usrVendorLocationID> e, PXFieldUpdated baseMethod)
        {
            baseMethod?.Invoke(e.Cache, e.Args);
            SOLine line = (SOLine)e.Row;
            if (line == null) return;
            SOLineExt lineExt = line.GetExtension<SOLineExt>();

            Address address = PXSelect<Address,
                Where<Address.bAccountID, Equal<Required<Address.bAccountID>>>>
                .Select(Base, lineExt?.UsrVendorID);

            if (address != null)
            {
                lineExt.UsrVendorAddress = string.Format("{0}{1}{2}, {3} {4}",
                    address.AddressLine1 ?? "",
                    string.IsNullOrWhiteSpace(address.AddressLine2) ? "" : " " + address.AddressLine2,
                    string.IsNullOrWhiteSpace(address.City) ? "" : ", " + address.City,
                    address.State ?? "",
                    address.PostalCode ?? ""
                ).Trim();
            }
            else
            {
                lineExt.UsrVendorAddress = null;
            }
        }

        protected virtual void SOOrder_CuryDetailExtPriceTotal_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            SOOrder row = (SOOrder)e.Row;
            if (row == null) return;
            SOOrderExt rowExt = row.GetExtension<SOOrderExt>();
            rowExt.UsrRTHCuryDetailExtPriceTotal = row.CuryDetailExtPriceTotal;
        }

        protected virtual void SOOrder_CuryLineDiscTotal_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            SOOrder row = (SOOrder)e.Row;
            if (row == null) return;
            SOOrderExt rowExt = row.GetExtension<SOOrderExt>();
            rowExt.UsrRTHCuryLineDiscTotal = row.CuryLineDiscTotal;
        }

        protected virtual void SOOrder_CuryDiscTot_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            SOOrder row = (SOOrder)e.Row;
            if (row == null) return;
            SOOrderExt rowExt = row.GetExtension<SOOrderExt>();
            rowExt.UsrRTHCuryDiscTot = row.CuryDiscTot;
        }

        protected virtual void SOOrder_CuryFreightTot_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            SOOrder row = (SOOrder)e.Row;
            if (row == null) return;
            SOOrderExt rowExt = row.GetExtension<SOOrderExt>();
            rowExt.UsrRTHCuryFreightTot = row.CuryFreightTot;
        }

        protected virtual void SOOrder_CuryTaxTotal_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            SOOrder row = (SOOrder)e.Row;
            if (row == null) return;
            SOOrderExt rowExt = row.GetExtension<SOOrderExt>();
            rowExt.UsrRTHCuryTaxTotal = row.CuryTaxTotal;
        }

        protected virtual void SOOrder_CuryOrderTotal_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            SOOrder row = (SOOrder)e.Row;
            if (row == null) return;
            SOOrderExt rowExt = row.GetExtension<SOOrderExt>();
            rowExt.UsrRTHCuryOrderTotal = row.CuryOrderTotal;
        }
        #endregion
    }
}