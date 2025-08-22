using PX.Data;
using PX.Objects.AR;
using PX.Objects.Common;
using PX.Objects.CS;
using PX.Objects.IN;
using System.Collections;
using System.Collections.Generic;
using Messages = PX.Objects.AR.Messages;

namespace ItemRequestCustomization
{
    /// <summary>
    /// PAGE : ST301001
    /// </summary>
    public class ItemRequestEntry : PXGraph<ItemRequestEntry>
    {

        //[PXCopyPasteHiddenFields(typeof(InventoryRequest.inventoryCD),typeof(InventoryRequest.inventoryID))]
        public PXSelect<InventoryRequest> Document;

        public PXSetup<INSetup> INSetup;
        public PXSetup<NumberingSequence> Numbering;

        public PXAction<InventoryRequest> CreateStockItem;
        [PXProcessButton()]
        [PXUIField(DisplayName = "Request Item")]
        public virtual IEnumerable createStockItem(PXAdapter adapter)
        {
            InventoryRequest request = Document.Current;
            if (request == null)
            {
                yield break;
            }

            if (ItemClassHasActiveProductBrand(request.ItemClassID))
            {
                throw new PXException(CustomMessages.NoActiveProductBrand);
            }

            PXCache cache = Document.Cache;
            List<InventoryRequest> list = new List<InventoryRequest>();
            foreach (InventoryRequest ardoc in adapter.Get<InventoryRequest>())
            {
                //if (ardoc.Hold == false && ardoc.Released == false)
                //{
                //    cache.MarkUpdated(ardoc);
                if (string.IsNullOrEmpty(ardoc.ItemDescription) ||
                        ardoc.ItemClassID == null || ardoc.PostClassID == null || ardoc.TaxCategoryID == null ||
                        string.IsNullOrEmpty(ardoc.StdUnitOfMeasure) || ardoc.DefaultWarehouse == null || string.IsNullOrEmpty(ardoc.PartNumber)
                         || string.IsNullOrEmpty(ardoc.ProductBrand))
                {
                    throw new PXException("Required fields are missing.");
                }
                list.Add(ardoc);
                //}
            }
            if (list.Count == 0)
            {
                throw new PXException(Messages.Document_Status_Invalid);
            }

            if (!HasInvAutoNumbering() && Document.Current.ItemClassID != null)
            {
                InventoryItem oldItem = PXSelect<InventoryItem,
                Where<InventoryItem.inventoryCD, Equal<Required<InventoryItem.inventoryCD>>>>
                .Select(this, request.InventoryCD.Trim());
                if (oldItem != null)
                    throw new PXException(CustomMessages.ItemAlreadyExists);
            }

            //PXLongOperation.StartOperation(this, delegate ()
            //{
            //var ie = PXGraph.CreateInstance<ItemRequestEntry>();
            //ie.ReleaseProcess(list);
            string inventoryCD = ReleaseProcess(list);
            if (inventoryCD != null)
            {
                Document.Ask(
                    "Item Created",
                    inventoryCD,
                    string.Format("Item {0} Created.", inventoryCD),
                    MessageButtons.OK);

            }
            Document.Cache.Clear();
            Document.Current = Document.Insert();
            Document.Cache.IsDirty = false;
            //yield return Helper.StartLongOperation(this, adapter, delegate ()
            //{
            //});
            //});
            //yield return adapter.Get();
        }

        public bool HasInvAutoNumbering()
        {
            bool hasAutoNumbering = false;
            Dimension dimension = PXSelect<Dimension,
                Where<Dimension.dimensionID, Equal<Required<Dimension.dimensionID>>>>
                .Select(this, "INVENTORY");

            if (dimension?.NumberingID != null)
            {
                Segment segment = PXSelect<Segment,
                    Where<Segment.dimensionID, Equal<Required<Segment.dimensionID>>,
                    And<Segment.autoNumber, Equal<True>>>>
                    .Select(this, dimension.DimensionID);

                if (segment != null)
                    hasAutoNumbering = true;
            }
            return hasAutoNumbering;
        }

        public string ReleaseProcess(List<InventoryRequest> list)
        {
            if (Document.Current.InventoryCD != null) return "";
            //PXTimeStampScope.SetRecordComesFirst(typeof(InventoryRequest), true);
            string result = null;
            foreach (var request in list)
            {

                Document.Current = request;
                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    // Create InventoryItem


                    PX.Objects.IN.InventoryItemMaint itemGraph = PXGraph.CreateInstance<PX.Objects.IN.InventoryItemMaint>();
                    itemGraph.Clear();

                    InventoryItem item = null;
                    if (!HasInvAutoNumbering())
                    {
                        item = itemGraph.Item.Insert(

                            new InventoryItem()
                            {
                                InventoryCD = request.InventoryCD.Trim()
                            }
                        );
                    }
                    else
                    {
                        item = itemGraph.Item.Insert();
                    }

                    //item.InventoryCD = request.RefNbr.Trim();
                    itemGraph.Item.Current = item;
                    item.Descr = request.ItemDescription;
                    //item.ItemClassID = request.ItemClassID;
                    itemGraph.Item.Cache.SetValueExt<InventoryItem.itemClassID>(itemGraph.Item.Current, request.ItemClassID);
                    itemGraph.Item.Cache.SetValueExt<InventoryItem.postClassID>(itemGraph.Item.Current, request.PostClassID);
                    itemGraph.Item.Cache.SetValueExt<InventoryItem.taxCategoryID>(itemGraph.Item.Current, request.TaxCategoryID);
                    //item.ItemClassID = request.ItemClassID;

                    //itemGraph.Item.Cache.RaiseFieldUpdated<InventoryItem.itemClassID>(itemGraph.Item.Current, null);
                    //itemGraph.Item.Current = itemGraph.Item.Update(item);

                    // Set currency settings (List Price, Default Warehouse)
                    InventoryItemCurySettings curySettings = itemGraph.ItemCurySettings.Current;
                    //curySettings.InventoryID = item.InventoryID;
                    curySettings.BasePrice = request.ListPrice;
                    curySettings.RecPrice = request.ListPrice;
                    curySettings.DfltSiteID = request.DefaultWarehouse;
                    itemGraph.ItemCurySettings.Current = itemGraph.ItemCurySettings.Update(curySettings);
                    //itemGraph.Caches[typeof(InventoryItemCurySettings)].Insert(curySettings);


                    //itemGraph.Actions.PressSave();

                    //InventoryItem item = itemGraph.Item.Insert();
                    //itemGraph.Item.Current = item;
                    //item.InventoryCD = request.RefNbr;
                    //itemGraph.Item.Current = item;
                    //item.Descr = request.ItemDescription;
                    //item.ItemClassID = request.ItemClassID;
                    //itemGraph.Item.Cache.RaiseFieldUpdated<InventoryItem.itemClassID>(item, null);
                    //itemGraph.Item.Cache.SetValueExt<InventoryItem.itemClassID>(item, request.ItemClassID);
                    item.BaseUnit = request.StdUnitOfMeasure;
                    item.SalesUnit = request.StdUnitOfMeasure;
                    item.PurchaseUnit = request.StdUnitOfMeasure;
                    item.CountryOfOrigin = request.CountryOfOrigin;
                    item.HSTariffCode = request.HTSCode;
                    item.KitItem = (request.IsAKit ?? false);
                    if(!string.IsNullOrEmpty(request.HTSCode))
                        item.CommodityCodeType = "HTS";
                    item.BaseItemWeight = request.WeightLbs;
                    item.PlanningMethod = request.PlanningMethod;
                    INSetupExt setupExt = INSetup.Cache.GetExtension<INSetupExt>(INSetup.Current);
                    //if (request.Serialized == "SERIAL")
                        item.LotSerClassID = request.SerialClassID;
                    //else
                    //    item.LotSerClassID = setupExt.UsrNonSerialClassID;

                    item.Body = request.NotesCatalogLink;

                    //var itemCache = itemGraph.Caches[typeof(InventoryItem)];
                    //item = (InventoryItem)itemCache.Insert(item);
                    InventoryItemExt inventoryItemExt = item.GetExtension<InventoryItemExt>();
                    inventoryItemExt.UsrSWKRTHCost = request.OurCost; // Set custom field value
                                                                      // Set cross reference for Part Number (Global)
                    if (!string.IsNullOrEmpty(request.PartNumber))
                    {
                        INItemXRef xref = new INItemXRef();
                        xref.InventoryID = item.InventoryID;
                        xref.AlternateType = INAlternateType.Global;
                        xref.AlternateID = request.PartNumber;
                        itemGraph.Caches[typeof(INItemXRef)].Insert(xref);
                    }

                    // Set cross reference for Customer Alternate ID
                    if (request.CustomerID != null && !string.IsNullOrEmpty(request.CustomerAlternateID))
                    {
                        INItemXRef custXref = new INItemXRef();
                        custXref.InventoryID = item.InventoryID;
                        custXref.AlternateType = INAlternateType.CPN; // Customer Part Number
                        custXref.BAccountID = request.CustomerID;
                        custXref.AlternateID = request.CustomerAlternateID;
                        itemGraph.Caches[typeof(INItemXRef)].Insert(custXref);
                    }

                    // Set replenishment settings

                    foreach (var itemRep in itemGraph.replenishment.Select())
                    {
                        itemGraph.replenishment.Delete(itemRep);
                    }
                    INItemRep rep1 = itemGraph.replenishment.Insert(new INItemRep()
                    {
                        //InventoryID = item.InventoryID,
                        ReplenishmentClassID = request.ReplenishmentClass1,
                        ReplenishmentPolicyID = request.Seasonality1,
                        ReplenishmentSource = request.Source1,
                        ReplenishmentMethod = request.Method1
                    });
                    //rep1.InventoryID = item.InventoryID;
                    //rep1.ReplenishmentClassID = request.ReplenishmentClass1;
                    //rep1.ReplenishmentPolicyID = request.Seasonality1;
                    //rep1.ReplenishmentSource = request.Source1;
                    //rep1.ReplenishmentMethod = request.Method1;
                    //itemGraph.Caches[typeof(INItemRep)].Insert(rep1);

                    //INItemRep rep2 = itemGraph.replenishment.Insert(new INItemRep()
                    //{
                    //    //InventoryID = item.InventoryID,
                    //    ReplenishmentClassID = request.ReplenishmentClass2,
                    //    ReplenishmentPolicyID = request.Seasonality2,
                    //    ReplenishmentSource = request.Source2,
                    //    ReplenishmentMethod = request.Method2
                    //});
                    //itemGraph.Caches[typeof(INItemRep)].Insert(rep2);

                    // Set vendor inventory (Min Order Qty)
                    // no vendor in fields
                    //if (request.MinOrderQty != null)
                    //{
                    //    POVendorInventory povendor = new POVendorInventory();
                    //    povendor.InventoryID = item.InventoryID;
                    //    povendor.MinOrdQty = request.MinOrderQty;
                    //    itemGraph.Caches[typeof(POVendorInventory)].Insert(povendor);
                    //}

                    // Set item class attributes
                    if (request.ItemClassID != null)
                    {
                        INItemClass itemClass = INItemClass.PK.Find(this, request.ItemClassID);
                        if (itemClass != null)
                        {
                            if (itemGraph.Answers.Select().Count == 0)
                            {
                                itemGraph.Answers.Current = itemGraph.Answers.Insert();
                                itemGraph.Answers.Current.RefNoteID = item.NoteID;
                                itemGraph.Answers.Current.AttributeID = setupExt.UsrProductBrandAttributeID;
                                itemGraph.Answers.Current.Value = request.ProductBrand;
                            }
                            else
                            {
                                foreach (CSAnswers _csAnswers in itemGraph.Answers.Select())
                                {
                                    if (_csAnswers.AttributeID == setupExt.UsrProductBrandAttributeID)
                                    {
                                        _csAnswers.Value = request.ProductBrand;
                                    }
                                }
                            }

                        }
                    }
                    //itemGraph.Item.UpdateCurrent();

                    // Save all changes
                    itemGraph.Save.Press();
                    Document.Current.InventoryCD = item.InventoryCD;
                    //// Set request status to Closed
                    //request.Status = "C";
                    //Document.Update(request);
                    //this.Actions.PressSave();

                    // Optionally, redirect to the new stock item (view only)
                    // PXRedirectHelper.TryRedirect(itemGraph, PXRedirectHelper.WindowMode.New);
                    ts.Complete();

                    result = item.InventoryCD;
                }
            }
            return result;
        }

        public bool ItemClassHasActiveProductBrand(int? itemClassID)
        {
            if (itemClassID == null)
            {
                return false;
            }

            INItemClass itemClass = INItemClass.PK.Find(this, itemClassID);

            INSetupExt iNSetupExt = INSetup.Current.GetExtension<INSetupExt>();

            CSAttributeGroup attributeGroup = PXSelect<CSAttributeGroup,
                Where<CSAttributeGroup.attributeID, Equal<Required<CSAttributeGroup.attributeID>>,
                And<CSAttributeGroup.isActive, Equal<True>,
                And<CSAttributeGroup.entityClassID, Equal<Required<CSAttributeGroup.entityClassID>>>>>>
                .Select(this, iNSetupExt.UsrProductBrandAttributeID, itemClass.ItemClassCD);
            return attributeGroup != null;
        }

        protected virtual void _(Events.RowSelected<InventoryRequest> e)
        {
            if (e.Row == null) return;

            if (HasInvAutoNumbering())
            {
                PXUIFieldAttribute.SetVisible<InventoryRequest.inventoryCD>(e.Cache, null, false);
                PXUIFieldAttribute.SetRequired<InventoryRequest.inventoryCD>(e.Cache, false);
            }
            else
            {
                PXUIFieldAttribute.SetVisible<InventoryRequest.inventoryCD>(e.Cache, null, true);
                PXUIFieldAttribute.SetRequired<InventoryRequest.inventoryCD>(e.Cache, true);
            }
        }

        protected virtual void _(Events.FieldDefaulting<InventoryRequest, InventoryRequest.defaultWarehouse> e)
        {
            if (e.Row == null) return;

            INSetupExt setupExt = INSetup.Cache.GetExtension<INSetupExt>(INSetup.Current);
            if (setupExt?.UsrDefaultWarehouse != null)
            {
                e.NewValue = setupExt.UsrDefaultWarehouse;
            }
        }
    }
}

// Use PXLocalizable attribute from PX.Common namespace
[PX.Common.PXLocalizable]
public static class CustomMessages
{
    public const string NoActiveProductBrand = "Selected Item Class does not have active Product Brand Attribute.";
    public const string ItemAlreadyExists = "Item with this ID already exists.";
}