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
using System.Collections.Generic;
using System.Linq;
using PX.Api;
using PX.Objects.AM.Attributes;
using PX.Common;
using PX.Data;
using PX.Objects.Common;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.SO;
using PX.Data.BQL.Fluent;
using static PX.Objects.AM.MRPProcessCache;
using System.Reflection;
using static PX.Objects.AM.MRPEngineFirstPass;
using static PX.Data.BQL.BqlPlaceholder;

namespace PX.Objects.AM
{
    /// <summary>
    /// MRP Regeneration process engine
    /// </summary>
    public class MRPEngine : PXGraph<MRPEngine>
    {
        public PXSelect<AMRPDetail> MrpDetailRecs;
        public PXSelect<AMRPAuditTable> MrpAuditRecs;
        public PXSelect<AMRPExceptions> MrpExceptionRecs;
        public PXSelect<AMRPDetailFP> MrpDetailFirstPassRecs;
        public PXSelect<AMRPDetailFPSupply> MrpDetailFirstPassSupply;
        public PXSelect<AMRPDetailPlan> MrpDetailPlanRecs;
        public PXSelect<AMRPPlan> MrpPlan;
        public PXSelect<Standalone.AMRPItemSite> MrpInventory;
        public PXSelect<AMSchdItem> SchdItem;
        public PXSelect<AMSchdOper> SchdOper;
        public PXSelect<AMSchdOperDetail> SchdOperDetail;
        public PXSelect<AMWCSchd> WCSChd;
        public PXSelect<AMWCSchdDetail> WCSChdDetail;
		public PXSelect<AMRPAuditHistory> MrpAuditHistory;
		public PXSelect<AMRPHistory> MrpHistory;


		[PXHidden]
        public PXFilter<AMRPRunTime> MrpRunTime;
        [PXHidden]
        public PXSelect<AMForecast> ForecastRecs;
        [PXHidden]
        public PXSelect<AMMPS> MPSRecs;

        //for cache attached only
        [PXHidden]
        public PXSelect<POVendorInventory> DummyVendorInventory;
        [PXHidden]
        public PXSelect<SOLine> DummySalesLine;
		[PXHidden]
        public PXSelect<InventoryItem> InventoryItems;

        public PXSetup<AMRPSetup> Setup;
        public PXSetup<AMOrderType>.Where<AMOrderType.orderType.IsEqual<AMRPSetup.planOrderType.FromCurrent>> PlanOrderType;
        public PXSetup<INSetup> InventorySetup;
        [PXHidden]
        public PXSetup<AMPSetup> ProdSetup;

        protected const int HIGHRECORDCOUNT = 10000;
		
		protected class SiteItemSubItem
		{
			public int? InventoryID;
			public int? SiteID;
			public int? SubItemID;

		}

		public MRPEngine()
        {
            PXCache setupCache = Setup.Cache;

            if (setupCache?.Current == null)
            {
                throw new PXSetupNotEnteredException(AM.Messages.GetLocal(AM.Messages.SetupNotEntered),
                    typeof(AMRPSetup), AM.Messages.GetLocal(AM.Messages.MrpSetup));
            }

            if (!Features.MRPOrDRPEnabled())
            {
                throw new PXException(Messages.UnableToProcess, this.GetType().Name);
            }

		}

		private string ConsolidateMakeKeyId(int? siteId, int? inventoryId, int? subItemId)
		{
			return string.Join(":",
				siteId?.ToString() ?? "0",
				inventoryId?.ToString() ?? "0",
				SubItemEnabled ? subItemId?.ToString() ?? "0" : "0");

		}

		protected virtual void MRPConsolidationByLowLevelBySite(List<INSite> sites, int lowLevel, MRPProcessCache mrpProcessCache)
		{
			if (sites == null || sites.Count == 0)
			{
				return;
			}

			foreach (var site in sites)
			{
				MRPConsolidationByLowLevelBySite(site, lowLevel, mrpProcessCache);
			}
		}

		protected virtual void MRPConsolidationByLowLevelBySite(INSite site, int lowLevel, MRPProcessCache mrpProcessCache)
		{
			MRPConsolidationByLowLevelBySite(site, lowLevel, mrpProcessCache, GetMrpDetailFPDemandByWarehouse(lowLevel, site?.SiteID));
		}

		protected virtual void MRPConsolidationByLowLevelBySite(INSite site, int lowLevel, MRPProcessCache mrpProcessCache, IEnumerable<AMRPDetailFP> mrpDetailFPDemands)
		{
			var origRecs = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.SortedList<DateTime?, System.Collections.Generic.List<AMRPDetailFP>>>();
			var siteItemSubItemRecs = new System.Collections.Generic.List<SiteItemSubItem>();
			foreach (var mrpDetailFp in mrpDetailFPDemands.Where(r => r.Processed == false))
			{
				string key = ConsolidateMakeKeyId(mrpDetailFp.SiteID, mrpDetailFp.InventoryID, mrpDetailFp.SubItemID);

				if (!origRecs.ContainsKey(key))
				{
					origRecs.Add(key, new System.Collections.Generic.SortedList<DateTime?, System.Collections.Generic.List<AMRPDetailFP>>());
					var rec = new SiteItemSubItem();
					rec.SiteID = mrpDetailFp.SiteID;
					rec.InventoryID = mrpDetailFp.InventoryID;
					rec.SubItemID = mrpDetailFp.SubItemID;
					siteItemSubItemRecs.Add(rec);
				}

				if (!origRecs[key].ContainsKey(mrpDetailFp.PlanDate))
				{
					origRecs[key].Add(mrpDetailFp.PlanDate, new System.Collections.Generic.List<AMRPDetailFP>());
				}

				origRecs[key][mrpDetailFp.PlanDate].Add(mrpDetailFp);
			}

			var longTermConsolidateAfterDays = Setup.Current.ConsolidateAfterDays.HasValue ? Setup.Current.ConsolidateAfterDays.GetValueOrDefault() : 5000;
			var longTermBucketDays = Setup.Current.BucketDays.HasValue ? Setup.Current.BucketDays.GetValueOrDefault() : 5000;
			var longTermBucketStart = Setup.Current.UseLongTermConsolidationBucket.HasValue && Setup.Current.UseLongTermConsolidationBucket.Value ?
											ProcessDateTime.Date.AddDays(longTermConsolidateAfterDays) : DateTime.MaxValue;

			foreach (var itemSite in siteItemSubItemRecs)
			{
				string key = ConsolidateMakeKeyId(itemSite.SiteID, itemSite.InventoryID, itemSite.SubItemID);

				if (!origRecs.ContainsKey(key))
				{
					continue;
				}

				var cache = mrpProcessCache.MrpItems.GetCurrentItemSiteCache(itemSite.InventoryID, itemSite.SiteID);
				if(cache == null) continue;

				var daysSupply = cache.GroupWindow;
				if (daysSupply <= 0)
				{
					continue;
				}

				var bucketCutoff = DateTime.MinValue;
				AMRPDetailFP lastRec = null;
				AMRPDetailFP consFPRec = null;
				bool firstConsolidatedRec = true;

				var recsbydate = origRecs[key];
				foreach (DateTime planDate in recsbydate.Keys)
				{
					foreach (AMRPDetailFP detailRec in recsbydate[planDate])
					{
						if (lastRec == null | detailRec.PlanDate > bucketCutoff)
						{
							lastRec = detailRec;
							consFPRec = null;
							firstConsolidatedRec = true;

							if (lastRec.PlanDate >= longTermBucketStart)
							{
								// We are in the long-term consolidation bucket
								bucketCutoff = lastRec.PlanDate.Value.AddDays(longTermBucketDays);
							}
							else
							{
								// We are still in the normal dayssupply bucket
								bucketCutoff = lastRec.PlanDate.Value.AddDays(daysSupply);

								if (bucketCutoff > longTermBucketStart)
								{
									// Cutoff grouping at Longterm Bucket Start
									bucketCutoff = longTermBucketStart.AddDays(-1);
								}
							}
						}

						else
						{
							if (firstConsolidatedRec == true)
							{
								firstConsolidatedRec = false;

								consFPRec = new AMRPDetailFP
								{
									OrderQtyConsumed = lastRec.OrderQtyConsumed,
									InventoryID = lastRec.InventoryID,
									SubItemID = lastRec.SubItemID,
									BranchID = lastRec.BranchID,
									LowLevel = lastRec.LowLevel,
									PlanDate = lastRec.PlanDate,
									OriginalQty = lastRec.OriginalQty,
									Qty = lastRec.Qty,
									ParentInventoryID = lastRec.ParentInventoryID,
									ParentSubItemID = lastRec.ParentSubItemID,
									ProductInventoryID = lastRec.ProductInventoryID,
									ProductSubItemID = lastRec.ProductSubItemID,
									Processed = false,
									OnHoldStatus = lastRec.OnHoldStatus,
									RefType = null,
									RequiredDate = lastRec.RequiredDate,
									SDFlag = lastRec.SDFlag,
									SiteID = lastRec.SiteID,
									SiteSequence = lastRec.SiteSequence,
									SuppliedQty = lastRec.SuppliedQty,
									Type = MRPPlanningType.Consolidated,
									ItemClassID = lastRec.ItemClassID
								};

								consFPRec = MrpDetailFirstPassRecs.Insert(consFPRec);

								// Flag Consolidated Record with itself as a Parent so that any Child records are flagged as well
								consFPRec.RefNbr = GetNextConsolidatedNbr();

								lastRec.Processed = true;  // Flag as Processed so Blowdown routine skips it
								lastRec.ConsolidatedRecordID = consFPRec.RecordID;

								consFPRec.ConsolidatedSafetyQty = 0;
								if (lastRec.Type == MRPPlanningType.SafetyStock)
								{
									consFPRec.ConsolidatedSafetyQty = consFPRec.ConsolidatedSafetyQty + lastRec.Qty;
								}

								lastRec = MrpDetailFirstPassRecs.Update(lastRec);

							}

							detailRec.Processed = true;  // Flag as Processed so Blowdown routine skips it
							detailRec.ConsolidatedRecordID = consFPRec.RecordID;
							MrpDetailFirstPassRecs.Update(detailRec);

							if (consFPRec == null)
							{
								continue;
							}

							consFPRec.OriginalQty = consFPRec.OriginalQty + detailRec.OriginalQty;
							consFPRec.Qty = consFPRec.Qty + detailRec.Qty;
							consFPRec.SuppliedQty = consFPRec.SuppliedQty + detailRec.SuppliedQty;

							if (detailRec.Type == MRPPlanningType.SafetyStock)
							{
								consFPRec.ConsolidatedSafetyQty = consFPRec.ConsolidatedSafetyQty.GetValueOrDefault() + detailRec.Qty.GetValueOrDefault();
							}

							consFPRec = MrpDetailFirstPassRecs.Update(consFPRec);
						}
					}
				}		
			}
		}

		/// <summary>
		/// To record additional messages into the MRP audit results.
		/// </summary>
		public bool DebugMessages =
#if DEBUG
			true;
#else
			false;
#endif

		public virtual DateTime ProcessDateTime
        {
            get
            {
                if (MrpRunTime.Current == null)
                {
                    return Common.Dates.Now;
                }

                return MrpRunTime.Current.RunDateTime.GetValueOrDefault(Common.Dates.Now);
            }
        }

        /// <summary>
        /// Should the scheduling process use fixed manufacturing lead times.
        /// </summary>
        public virtual bool UseFixLeadTime
        {
            get
            {
                if (Setup.Current == null)
                {
                    Setup.Current = Setup.Select();
                }

                return Setup.Current != null && Setup.Current.UseFixMfgLeadTime.GetValueOrDefault();
            }
        }

        public virtual bool SubItemEnabled => PXAccess.FeatureInstalled<FeaturesSet.subItem>();

        /// <summary>
        /// When scheduling orders do we want to lookup and reuse the planning numbers used during the last run?
        /// Default = True
        /// </summary>
        public bool ReuseScheduleNumbering = true;

        /// <summary>
        /// To correctly get a BOM revision, are we looking at BOM dates?
        /// this in its current implementation will run sub query for all items for all bom lookups (performance issue)
        /// Default = True
        /// </summary>
        public bool LookupBomsByDate = true;

		private MRPProcessCache _mrpProcessCache;

		/// <summary>
		/// Will the scheduled results of planned orders be saved to the database when true.
		/// </summary>

		public bool PersistPlannedOrderSchedule = true;

		#region CACHE ATTACHED

		[PXDBInt(IsKey = true)]
		[PXUIField(DisplayName = "Kit Inventory ID")]
		protected virtual void _(Events.CacheAttached<INKitSpecHdr.kitInventoryID> e) { }

		[PXDBInt(IsKey = true)]
		[PXUIField(DisplayName = "Inventory ID")]
		protected virtual void _(Events.CacheAttached<INKitSpecStkDet.kitInventoryID> e) { }

		[PXInt]
		protected virtual void _(Events.CacheAttached<InventoryItem.parentItemClassID> e) { }

		[PXDBInt]
		[PXUIField(DisplayName = "Item Class")]
		protected virtual void _(Events.CacheAttached<InventoryItem.itemClassID> e) { }

		[PXDBInt]
		[PXUIField(DisplayName = "Item Class")]
		protected virtual void _(Events.CacheAttached<AMRPDetailPlan.itemClassID> e) { }

		[PXDBInt]
		[PXUIField(DisplayName = "Item Class")]
		protected virtual void _(Events.CacheAttached<AMRPDetailFP.itemClassID> e) { }

		[PXDBInt]
		[PXUIField(DisplayName = "Item Class")]
		protected virtual void _(Events.CacheAttached<AMRPDetail.itemClassID> e) { }

		/// <summary>
		/// Removing PODefaultVendorAttribute to avoid unnecessary sub-quires during MRP processing
		/// </summary>
		[PXBool]
		protected virtual void _(Events.CacheAttached<POVendorInventory.isDefault> e) { }

        [PXDBInt]
		protected virtual void _(Events.CacheAttached<SOLine.siteID> e) { }

        [PXDBInt(IsKey = true)]
        [PXDefault]
        [PXUIField(DisplayName = "FP Record ID")]
        [PXLineNbr(typeof(AMRPRunTime.firstPassRecordID))]
		protected virtual void _(Events.CacheAttached<AMRPDetailFP.recordID> e) { }

        [PXDBInt(IsKey = true)]
        [PXDefault]
        [PXUIField(DisplayName = "Record ID")]
        [PXLineNbr(typeof(AMRPRunTime.detailRecordID))]
		protected virtual void _(Events.CacheAttached<AMRPDetail.recordID> e) { }

        [PXDBInt]
		protected virtual void _(Events.CacheAttached<AMRPDetail.subItemID> e) { }

        [PXDBInt]
		protected virtual void _(Events.CacheAttached<AMRPDetail.parentSubItemID> e) { }

        [PXDBInt]
		protected virtual void _(Events.CacheAttached<AMRPDetail.productSubItemID> e) { }

        [PXDBInt]
		protected virtual void _(Events.CacheAttached<AMRPDetail.preferredVendorID> e) { }

        [PXDBInt]
        protected virtual void _(Events.CacheAttached<AMRPDetail.productManagerID> e) { }

        [PXDBInt]
		protected virtual void _(Events.CacheAttached<AMRPDetailFP.subItemID> e) { }

        [PXDBInt]
		protected virtual void _(Events.CacheAttached<AMRPDetailFP.parentSubItemID> e) { }

        [PXDBInt]
		protected virtual void _(Events.CacheAttached<AMRPDetailFP.productSubItemID> e) { }

        [PXDBInt]
		protected virtual void _(Events.CacheAttached<AMRPExceptions.subItemID> e) { }

        [PXDBInt]
		protected virtual void _(Events.CacheAttached<AMRPPlan.subItemID> e) { }
		[PXString]
		protected virtual void _(Events.CacheAttached<AMRPPlan.uOM> e) { }

        [PXDBInt]
		protected virtual void _(Events.CacheAttached<AMRPPlan.parentSubItemID> e) { }

        [PXDBInt]
		protected virtual void _(Events.CacheAttached<AMRPPlan.productSubItemID> e) { }

        [PXDBInt(IsKey = true)]
        [PXDefault]
        [PXLineNbr(typeof(AMRPRunTime.planRecordID))]
		protected virtual void _(Events.CacheAttached<AMRPPlan.recordID> e) { }

        [PXDBInt(IsKey = true)]
        [PXDefault]
        [PXLineNbr(typeof(AMRPRunTime.exceptionRecordID))]
		protected virtual void _(Events.CacheAttached<AMRPExceptions.recordID> e) { }

        [PXUIField(DisplayName = "Related Document", Enabled = false)]
        [PXDBGuid] //turn off refnote attribute
        protected virtual void _(Events.CacheAttached<AMRPDetail.refNoteID> e) { }

        [PXUIField(DisplayName = "Related Parent Document", Enabled = false)]
        [PXDBGuid] //turn off refnote attribute
        protected virtual void _(Events.CacheAttached<AMRPDetail.parentRefNoteID> e) { }

        [PXUIField(DisplayName = "Related Product Document", Enabled = false)]
        [PXDBGuid] //turn off refnote attribute
        protected virtual void _(Events.CacheAttached<AMRPDetail.productRefNoteID> e) { }

        [PXUIField(DisplayName = "Related Document", Enabled = false)]
        [PXDBGuid] //turn off refnote attribute
        protected virtual void _(Events.CacheAttached<AMRPDetailFP.refNoteID> e) { }

        [PXUIField(DisplayName = "Related Parent Document", Enabled = false)]
        [PXDBGuid] //turn off refnote attribute
        protected virtual void _(Events.CacheAttached<AMRPDetailFP.parentRefNoteID> e) { }

        [PXUIField(DisplayName = "Related Product Document", Enabled = false)]
        [PXDBGuid] //turn off refnote attribute
        protected virtual void _(Events.CacheAttached<AMRPDetailFP.productRefNoteID> e) { }

        // ALL INVENTORY ATTRIBUTES NEED REMOVED AS ITEMS OF ANY STATUS WITH ACTIVITY NEEDS SAVED TO THE RECORD 
        //  (HOWEVER THERE IS NO PLANNING DONE FOR THE INVALID STATUS ITEMS)
        //  Only for non plan order tables

        [PXDBInt]
        [PXDefault]
        [PXUIField(DisplayName = "Inventory ID")]
		protected virtual void _(Events.CacheAttached<AMRPExceptions.inventoryID> e) { }

        [PXDBInt]
        [PXDefault]
        [PXUIField(DisplayName = "Inventory ID")]
		protected virtual void _(Events.CacheAttached<AMRPPlan.inventoryID> e) { }

        [PXDBInt]
		protected virtual void _(Events.CacheAttached<AMRPPlan.parentInventoryID> e) { }

        [PXDBInt]
		protected virtual void _(Events.CacheAttached<AMRPPlan.productInventoryID> e) { }

        [PXDBInt]
        [PXDefault]
        [PXUIField(DisplayName = "Inventory ID")]
		protected virtual void _(Events.CacheAttached<AMRPDetailFP.inventoryID> e) { }

        [PXDBInt]
		protected virtual void _(Events.CacheAttached<AMRPDetailFP.parentInventoryID> e) { }

        [PXDBInt]
		protected virtual void _(Events.CacheAttached<AMRPDetailFP.productInventoryID> e) { }

        [PXDBInt]
		protected virtual void _(Events.CacheAttached<AMSchdItem.inventoryID> e) { }

        [PXDBInt]
		protected virtual void _(Events.CacheAttached<AMSchdItem.siteID> e) { }

        [PXDBInt]
		protected virtual void _(Events.CacheAttached<AMRPDetail.inventoryID> e) { }

        [PXDBString(6, InputMask = ">aaaaaa", IsUnicode = true)]
        [PXUIField(DisplayName = "UOM", Visibility = PXUIVisibility.Visible)]
		protected virtual void _(Events.CacheAttached<AMRPDetail.baseUOM> e) { }

        [PXDBInt]
		protected virtual void _(Events.CacheAttached<AMRPDetail.parentInventoryID> e) { }

        [PXDBInt]
		protected virtual void _(Events.CacheAttached<AMRPDetail.productInventoryID> e) { }

        [PXDBInt]
		protected virtual void _(Events.CacheAttached<AMRPDetailPlan.inventoryID> e) { }

        [PXDBInt]
		protected virtual void _(Events.CacheAttached<AMRPDetailPlan.siteID> e) { }

		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMRPDetail.siteID> e) { }

		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMRPDetailFP.siteID> e) { }

		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMRPExceptions.siteID> e) { }

		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMRPExceptions.supplySiteID> e) { }

		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMRPPlan.siteID> e) { }

		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMWCSchd.siteID> e) { }

		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMWCSchdDetail.siteID> e) { }

		[PXDBInt(IsKey = true)]
		protected virtual void _(Events.CacheAttached<Standalone.AMRPItemSite.siteID> e) { }

		[PXDBInt]
		[PXUIField(DisplayName = "Replenishment Warehouse")]
		protected virtual void _(Events.CacheAttached<AMRPItemSite.replenishmentSiteID> e) { }

		[PXDBInt]
		[PXUIField(DisplayName = "Branch")]
		protected virtual void _(Events.CacheAttached<AMRPDetailFP.branchID> e) { }

		[PXDBInt]
		[PXUIField(DisplayName = "Branch")]
		protected virtual void _(Events.CacheAttached<AMRPDetail.branchID> e) { }

		[PXDBInt]
		[PXUIField(DisplayName = "Branch")]
		protected virtual void _(Events.CacheAttached<AMRPExceptions.branchID> e) { }

		[PXDBInt]
		[PXUIField(DisplayName = "Branch")]
		protected virtual void _(Events.CacheAttached<AMRPPlan.branchID> e) { }

		[PXDBInt]
		[PXUIField(DisplayName = "Branch")]
		protected virtual void _(Events.CacheAttached<AMRPDetailPlan.branchID> e) { }

		protected virtual void _(Events.FieldDefaulting<AMRPAuditTable, AMRPAuditTable.processID> e)
        {
            e.NewValue = e.Cache.Graph.UID;
        }

		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMRPDetail.transferSiteID> e) { }

		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMRPDetailFP.transferSiteID> e) { }

		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMRPDetailPlan.transferSiteID> e) { }

		[PXMergeAttributes]
		[PXRemoveBaseAttribute(typeof(OperationDBTimeAttribute))]
		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMProdOper.setupTime> e) { }

		[PXMergeAttributes]
		[PXRemoveBaseAttribute(typeof(OperationDBTimeAttribute))]
		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMProdOper.runUnitTime> e) { }

		[PXMergeAttributes]
		[PXRemoveBaseAttribute(typeof(OperationDBTimeAttribute))]
		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMProdOper.machineUnitTime> e) { }

		[PXMergeAttributes]
		[PXRemoveBaseAttribute(typeof(OperationDBTimeAttribute))]
		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMProdOper.queueTime> e) { }

		[PXMergeAttributes]
		[PXRemoveBaseAttribute(typeof(OperationDBTimeAttribute))]
		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMProdOper.finishTime> e) { }

		[PXMergeAttributes]
		[PXRemoveBaseAttribute(typeof(OperationDBTimeAttribute))]
		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMProdOper.moveTime> e) { }

		[PXMergeAttributes]
		[PXRemoveBaseAttribute(typeof(OperationDBTimeAttribute))]
		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMProdOper.planMachineTime> e) { }

		[PXMergeAttributes]
		[PXRemoveBaseAttribute(typeof(OperationDBTimeAttribute))]
		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMProdOper.actualMachineTime> e) { }

		[PXMergeAttributes]
		[PXRemoveBaseAttribute(typeof(OperationDBTimeAttribute))]
		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMBomOper.setupTime> e) { }

		[PXMergeAttributes]
		[PXRemoveBaseAttribute(typeof(OperationDBTimeAttribute))]
		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMBomOper.runUnitTime> e) { }

		[PXMergeAttributes]
		[PXRemoveBaseAttribute(typeof(OperationDBTimeAttribute))]
		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMBomOper.machineUnitTime> e) { }

		[PXMergeAttributes]
		[PXRemoveBaseAttribute(typeof(OperationDBTimeAttribute))]
		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMBomOper.queueTime> e) { }

		[PXMergeAttributes]
		[PXRemoveBaseAttribute(typeof(OperationDBTimeAttribute))]
		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMBomOper.finishTime> e) { }

		[PXMergeAttributes]
		[PXRemoveBaseAttribute(typeof(OperationDBTimeAttribute))]
		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMBomOper.moveTime> e) { }

		[PXMergeAttributes]
		[PXRemoveBaseAttribute(typeof(OperationDBTimeAttribute))]
		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMSchdOper.queueTime> e) { }

		[PXMergeAttributes]
		[PXRemoveBaseAttribute(typeof(OperationDBTimeAttribute))]
		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMSchdOper.setupTime> e) { }

		[PXMergeAttributes]
		[PXRemoveBaseAttribute(typeof(OperationDBTimeAttribute))]
		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMSchdOper.finishTime> e) { }

		[PXMergeAttributes]
		[PXRemoveBaseAttribute(typeof(OperationDBTimeAttribute))]
		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMSchdOper.moveTime> e) { }

		[PXMergeAttributes]
		[PXRemoveBaseAttribute(typeof(OperationDBTimeAttribute))]
		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMWC.defaultQueueTime> e) { }

		[PXMergeAttributes]
		[PXRemoveBaseAttribute(typeof(OperationDBTimeAttribute))]
		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMWC.defaultFinishTime> e) { }

		[PXMergeAttributes]
		[PXRemoveBaseAttribute(typeof(OperationDBTimeAttribute))]
		[PXDBInt]
		protected virtual void _(Events.CacheAttached<AMWC.defaultMoveTime> e) { }

		#endregion

		/// <summary>
		/// Call MRP Engine using the calling graph allowing the calling graph to update when the long operation is complete
		/// </summary>
		/// <param name="graph"></param>
		public virtual void Run(PXGraph graph)
        {
            MrpRunTime.Cache.Clear();
			MrpRunTime.Current = MrpRunTime.Insert();
			MrpRunTime.Cache.SetValueExt<AMRPRunTime.runDateTime>(MrpRunTime.Current, Common.Dates.Now);
			MrpRunTime.Cache.SetValueExt<AMRPRunTime.callingGraphUid>(MrpRunTime.Current, (Guid?)graph.UID);

			// Acuminator disable once PX1008 LongOperationDelegateSynchronousExecution [refactoring needed]
			PXLongOperation.StartOperation(graph, () =>
            {
                PXLongOperationHelper.SetCustomInfoCompanyID();
                PXLongOperationHelper.CheckForProcessIsRunningByCompany(graph);
                Exception mrpProcessException = null;

                try
                {
                    this.MrpDetailPlanRecs.Cache.DisableReadItem = true;
                    this.MrpDetailFirstPassRecs.Cache.DisableReadItem = true;
                    this.MrpDetailRecs.Cache.DisableReadItem = true;
                    this.MrpInventory.Cache.DisableReadItem = true;
                    this.ForecastRecs.Cache.DisableReadItem = true;
                    this.MPSRecs.Cache.DisableReadItem = true;
                    this.MrpDetailFirstPassSupply.Cache.DisableReadItem = true;
                    this.MrpPlan.Cache.DisableReadItem = true;
                    this.SchdItem.Cache.DisableReadItem = true;
                    this.SchdOper.Cache.DisableReadItem = true;
                    this.SchdOperDetail.Cache.DisableReadItem = true;

                    DeleteAuditTable();
                    InsertAuditRec(Messages.GetLocal(Messages.MRPEngineStarting), MrpRunTime.Current.CallingGraphUid, AMRPAuditTable.MsgTypes.Start);
                    CheckPreferences();
                    ClearAllMRPData(false);

                    if (SubItemEnabled)
                    {
                        InsertAuditRec(Messages.GetLocal(Messages.MRPSubitemsEnabled));
                    }

					if (!PersistPlannedOrderSchedule)
					{
						InsertAuditRec(Messages.GetLocal(Messages.MRPEnginePersistScheduleDisabled));
					}

                    if (!ReuseScheduleNumbering)
                    {
                        InsertAuditRec(Messages.GetLocal(Messages.MRPEngineReuseScheduleNumbersDisabled));
                    }

                    if (!LookupBomsByDate)
                    {
                        InsertAuditRec(Messages.GetLocal(Messages.MRPEngineBomLookupByDateDisabled));
                    }

					var maxLowLevel = 0;
					if (Features.MRPEnabled() || (Features.KitFeatureEnabled()))
					{
						var llProcessed = TryProcessLowLevel(out maxLowLevel);
						var lowLevelMsg = Messages.GetLocal(llProcessed ? Messages.MRPEngineMaxLowLevel : Messages.MRPEngineMaxLowLevelProcessSkipped, maxLowLevel);
						InsertAuditRec(lowLevelMsg);
					}

					_mrpProcessCache = new MRPProcessCache(maxLowLevel) {MrpItems = CacheInventoryItems()};
					_mrpProcessCache.LoadPurchaseCalendar(this);

                    InsertAuditRec(Messages.GetLocal(Messages.MRPEngineProcessFirstPass));
                    FirstPass(_mrpProcessCache.MrpItems);
                    InsertAuditRec(Messages.GetLocal(Messages.MRPEngineFirstPassCompleted));

#if DEBUG
                    InsertAuditRec($"Inserting {MrpDetailFirstPassRecs.Cache?.Inserted?.Count() ?? 0} {Common.Cache.GetCacheName(typeof(AMRPDetailFP))} records");
#endif
                    MRPSave(true);

					// Cache not used after FirstPass()
					MrpDetailPlanRecs.Cache.Clear();
					MrpDetailPlanRecs.Cache.ClearQueryCache();

                    RequirementsByLowLevel(_mrpProcessCache);
                    MRPSave();

                    MrpExceptionsAll();
                    LoadMrpPlan();
                    ClearUnusedPlanDetail();

                    MRPSave();
                }
                catch (Exception ex)
                {
                    var source = ex.InnerException != null ? ex.InnerException.Source : ex.Source;
                    var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
					var st = ex.InnerException != null ? ex.InnerException.StackTrace : ex.StackTrace;

                    var sourceMsg = string.IsNullOrWhiteSpace(source)
                        ? string.Empty
                        : Messages.GetLocal(Messages.ExceptionIn, source);

                    var msg2 = ex is MRPRegenException && ex.InnerException != null
                        ? ex.Message
                        : string.Empty;

                    InsertAuditRec($"** {Messages.GetLocal(Messages.NotComplete)}. {sourceMsg}");
                    InsertAuditRec($"** {Messages.GetLocal(Messages.Exception)}: {msg}", AMRPAuditTable.MsgTypes.Error);
                    if (!string.IsNullOrWhiteSpace(msg2))
                    {
                        InsertAuditRec($"** {Messages.GetLocal(Messages.Exception)}: {msg2}", AMRPAuditTable.MsgTypes.Error);
                    }

					var oe = ex as PXOuterException;
					if (oe == null && ex.InnerException != null)
					{
						oe = ex.InnerException as PXOuterException;
					}
                    if (oe != null && PXTraceHelper.TryGetOuterExceptionFieldError(oe, out var fieldMessage))
                    {
						InsertAuditRec($"** {fieldMessage}");
                    }

					InsertAuditRec($"** {st}", AMRPAuditTable.MsgTypes.Error);

                    mrpProcessException = ex;
                    throw;
                }
                finally
                {
                    var failed = mrpProcessException != null;
                    var endingMsg = failed
                        ? Messages.GetLocal(Messages.Failed)
                        : Messages.GetLocal(Messages.Completed);

                    try
                    {
                        var regenCompletedDateTime = Common.Dates.Now;
                        InsertAuditRec(endingMsg, MrpRunTime.Current.CallingGraphUid, graph.GetLongRunningTimeSpan(), AMRPAuditTable.MsgTypes.End);

                        if (failed)
                        {
                            MrpInventory.Cache.Clear();
                            MrpDetailRecs.Cache.Clear();
                            MrpExceptionRecs.Cache.Clear();
                            MrpDetailFirstPassRecs.Cache.Clear();
                            MrpDetailFirstPassSupply.Cache.Clear();
                            MrpDetailPlanRecs.Cache.Clear();

                            //if exception... clear out any processed records
                            ClearAllMRPData(false);
                        }
                        else
                        {
                            var mrpSetup = Setup.Current ?? Setup.Select();
                            if (mrpSetup != null)
                            {
                                mrpSetup.LastMrpRegenCompletedDateTime = regenCompletedDateTime;
                                mrpSetup.LastMrpRegenCompletedByID = Accessinfo.UserID;
                                Setup.Cache.Update(mrpSetup);
                                Setup.Cache.Persist(PXDBOperation.Update);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        PXTraceHelper.PxTraceException(e);
                    }
                }
            });
        }

		/// <summary>
		/// Check for preferences/configuration issues related to running MRP.
		/// Any issues throw an exception
		/// </summary>
		private void CheckPreferences()
        {
            var mrpPrefMsg = $"{Messages.GetLocal(Messages.IncorrectConfiguration)}: {Common.Cache.GetCacheName(typeof(AMRPSetup))}";
            if (Setup.Current == null)
            {
                throw new MRPRegenException(mrpPrefMsg,
                    new PXException(Messages.RecordMissing, Common.Cache.GetCacheName(typeof(AMRPSetup))));
            }

			if (!Features.MRPEnabled())
			{
				return;
			}

			if (string.IsNullOrWhiteSpace(Setup.Current.PlanOrderType))
			{
				throw new MRPRegenException(mrpPrefMsg,
					new PXException(ErrorMessages.FieldIsEmpty,
						PXUIFieldAttribute.GetDisplayName<AMRPSetup.planOrderType>(Setup.Cache)));
			}

			Numbering planOrderTypeNumbering = PXSelectJoin<
				Numbering,
				InnerJoin<AMOrderType,
					On<Numbering.numberingID, Equal<AMOrderType.prodNumberingID>>>,
				Where<AMOrderType.orderType, Equal<Current<AMRPSetup.planOrderType>>>>
				.SelectWindowed(this, 0, 1);

			if (planOrderTypeNumbering == null)
			{
				throw new MRPRegenException(mrpPrefMsg,
					new PXException(Messages.NumberingMissingExceptionProduction,
						PXUIFieldAttribute.GetDisplayName<AMRPSetup.planOrderType>(Setup.Cache)));
			}

			if (planOrderTypeNumbering.UserNumbering.GetValueOrDefault())
			{
				throw new MRPRegenException(mrpPrefMsg,
					new PXException(Messages.NumberingSequenceMustUseAutoNumbering,
						PXUIFieldAttribute.GetDisplayName<AMRPSetup.planOrderType>(Setup.Cache)));
			}

            try
            {
                var ampSetup = ProdSetup?.Current;
                if (AMPSetup.CheckSetup(ampSetup, out var ampException))
                {
                    throw ampException;
                }

                if (ampSetup?.FixMfgCalendarID == null && Setup.Current?.UseFixMfgLeadTime == true)
                {
                    throw new Exception(Messages.GetLocal(Messages.MrpFixedLeadTimeRequiresProdPreferencesCalendar));
                }
            }
            catch (Exception e)
            {
                throw new MRPRegenException(Messages.IncorrectConfiguration, e);
            }
        }

		protected Dictionary<int, int> GetWarehouseBranches()
		{
			var d = new Dictionary<int, int>();
			foreach (INSite warehouse in PXSelect<INSite>.Select(this))
			{
				var branch = warehouse?.BranchID;
				if(branch == null)
				{
					continue;
				}

				d.Add(warehouse.SiteID.GetValueOrDefault(), branch.GetValueOrDefault());
			}
			return d;
		}

        /// <summary>
        /// Writes all MRP Exception messages
        /// </summary>
        protected virtual void MrpExceptionsAll()
        {
            InsertAuditRec(Messages.GetLocal(Messages.MRPEngineCheckinExceptions));

#if DEBUG
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var counterExpedite = 0;
            var counterDefer = 0;
            var counterDelete = 0;
            var counterLate = 0;
#endif
	        foreach (AMRPDetailFP mrpDetailFP in PXSelect<
                AMRPDetailFP,
                Where<AMRPDetailFP.sDFlag, Equal<MRPSDFlag.supply>,
                    And<AMRPDetailFP.type, NotEqual<MRPPlanningType.stockAdjustment>,
                    And<Where<AMRPDetailFP.onHoldStatus, Equal<OnHoldStatus.notOnHold>,
                        Or<AMRPDetailFP.onHoldStatus, Equal<OnHoldStatus.onHoldInclude>>>>>>>
                .Select(this))
            {
                if (mrpDetailFP.Qty.GetValueOrDefault() > 0
                    && mrpDetailFP.Qty.GetValueOrDefault() != mrpDetailFP.SuppliedQty.GetValueOrDefault())
                {
                    var daysBetween = Common.Dates.DaysBetween(mrpDetailFP.PlanDate, mrpDetailFP.RequiredDate);

                    //Expedite - Supply used is beyond the promise date
                    if (daysBetween != 0 && daysBetween >= Setup.Current.ExceptionDaysAfter.GetValueOrDefault())
                    {
                        InsertMrpSupplyException(mrpDetailFP, MRPExceptionType.Expedite);
#if DEBUG
                        counterExpedite++;
#endif
                    }

                    //Defer - Supply is due prior to the promise date
                    if (daysBetween < 0 &&
                        Math.Abs(daysBetween) >= Setup.Current.ExceptionDaysBefore.GetValueOrDefault())
                    {
                        InsertMrpSupplyException(mrpDetailFP, MRPExceptionType.Defer);
#if DEBUG
                        counterDefer++;
#endif
                    }
                }

                //Delete - Supply Items that are not required for any demand
                if (mrpDetailFP.SuppliedQty.GetValueOrDefault() > 0
                    && mrpDetailFP.SuppliedQty.GetValueOrDefault() == mrpDetailFP.OriginalQty.GetValueOrDefault())
                {
                    InsertMrpSupplyException(mrpDetailFP, MRPExceptionType.Delete);
#if DEBUG
                    counterDelete++;
#endif
                }

                //Late - Supply items that should have been received by now
                if (Common.Dates.Compare(mrpDetailFP.PlanDate, ProcessDateTime) < 0)
                {
                    InsertMrpSupplyException(mrpDetailFP, MRPExceptionType.Late);
#if DEBUG
                    counterLate++;
#endif
                }
            }
#if DEBUG
            InsertAuditRec(PXTraceHelper.CreateTimespanMessage(sw.Elapsed, $"{Messages.MRPEngineCheckinExceptions} - Expedite [{counterExpedite}], Defer [{counterDefer}], Delete [{counterDelete}], Late [{counterLate}]"));
            sw.Restart();
#endif
			var warehouseBranches = GetWarehouseBranches();
			var branchID = 0;
            // Type: order on hold
            foreach (AMRPDetailFP detailFPOnHold in PXSelectGroupBy<
                AMRPDetailFP,
                Where<AMRPDetailFP.onHoldStatus, Equal<OnHoldStatus.onHoldExclude>>,
                Aggregate<
                    GroupBy<AMRPDetailFP.inventoryID,
                    GroupBy<AMRPDetailFP.subItemID,
                    GroupBy<AMRPDetailFP.siteID,
                    GroupBy<AMRPDetailFP.type,
                    GroupBy<AMRPDetailFP.refNoteID,
                        Sum<AMRPDetailFP.originalQty>>>>>>>>
                .Select(this))
            {
				warehouseBranches.TryGetValue(detailFPOnHold.SiteID.GetValueOrDefault(), out branchID);
                var onHoldOrderException = new AMRPExceptions
                {
                    Type = MRPExceptionType.OrderOnHold,
                    Qty = detailFPOnHold.OriginalQty,
                    InventoryID = detailFPOnHold.InventoryID,
                    SubItemID = detailFPOnHold.SubItemID,
                    PromiseDate = detailFPOnHold.PlanDate ?? ProcessDateTime,
                    RequiredDate = detailFPOnHold.RequiredDate ?? ProcessDateTime,
                    RefNbr = detailFPOnHold.RefNbr,
                    RefNoteID = detailFPOnHold.RefNoteID,
                    RefType = detailFPOnHold.Type,
                    SiteID = detailFPOnHold.SiteID,
                    ItemClassID = detailFPOnHold.ItemClassID,
					BranchID = branchID
                };
                InsertMrpException(onHoldOrderException);
            }
#if DEBUG
            InsertAuditRec(PXTraceHelper.CreateTimespanMessage(sw.Elapsed, $"{Messages.MRPEngineCheckinExceptions} - Order on hold"));
            sw.Restart();
#endif

			var plannedSupplyResults = PXSelectGroupBy<
					AMRPDetailItemSite,
					Where<AMRPDetailItemSite.promiseDate, LessEqual<Current<AccessInfo.businessDate>>>,
					Aggregate<
						GroupBy<AMRPDetailItemSite.inventoryID,
						GroupBy<AMRPDetailItemSite.subItemID,
						GroupBy<AMRPDetailItemSite.siteID,
							Sum<AMRPDetailItemSite.baseQty
							>>>>>>
					.Select(this).ToFirstTableList();

			// Transfer Available - Supply items that have inventory beyond site requirements
			foreach (AMRPDetailFPOpenSupply detailFPSumQty in PXSelectGroupBy<
				AMRPDetailFPOpenSupply,
                Where<AMRPDetailFPOpenSupply.planDate, LessEqual<Current<AccessInfo.businessDate>>>,
                Aggregate<
                    GroupBy<AMRPDetailFPOpenSupply.inventoryID,
                    GroupBy<AMRPDetailFPOpenSupply.subItemID,
                    GroupBy<AMRPDetailFPOpenSupply.siteID,
                        Sum<AMRPDetailFPOpenSupply.suppliedQty>>>>>>
                .Select(this))
			{
				foreach (var detailSumQty in plannedSupplyResults
					.Where(r => r.InventoryID == detailFPSumQty.InventoryID
						&& r.SubItemID == detailFPSumQty.SubItemID
						&& r.SiteID != detailFPSumQty.SiteID))
				{
					if (detailSumQty?.InventoryID == null || detailSumQty.BaseQty.GetValueOrDefault() <= 0)
					{
						continue;
					}

					var mrpException = new AMRPExceptions
					{
						Qty = detailSumQty.BaseQty.GetValueOrDefault(),
						InventoryID = detailSumQty.InventoryID,
						SubItemID = detailSumQty.SubItemID,
						PromiseDate = detailSumQty.PromiseDate,
						RequiredDate = detailSumQty.PromiseDate,
						Type = MRPExceptionType.Transfer,
						SiteID = detailSumQty.SiteID,
						SupplySiteID = detailFPSumQty.SiteID,
						SupplyQty = detailFPSumQty.SuppliedQty.GetValueOrDefault(),
						ItemClassID = detailSumQty.ItemClassID,
						BranchID = detailSumQty.BranchID
					};

					InsertMrpException(mrpException);
				}
			}

			CreateNoTRReplenishmentWarehouseMRPException(warehouseBranches);

#if DEBUG
			InsertAuditRec(PXTraceHelper.CreateTimespanMessage(sw.Elapsed, $"{Messages.MRPEngineCheckinExceptions} - Transfer Available"));
#endif
            InsertAuditRec(Messages.GetLocal(Messages.MRPEngineCheckinExceptionsComplete));
        }

        protected virtual void InsertMrpSupplyException(AMRPDetailFP mrpDetailFp, string mrpExceptionType)
        {
            if (mrpDetailFp?.InventoryID == null)
            {
                return;
            }

            var exception = MapToMrpException(mrpDetailFp);
            exception.SupplyQty = mrpDetailFp.Qty.GetValueOrDefault() - mrpDetailFp.SuppliedQty.GetValueOrDefault();
            exception.Type = mrpExceptionType;
            InsertMrpException(exception);
        }

        public virtual void InsertMrpException(AMRPExceptions row)
        {
            if (row?.InventoryID == null)
            {
                return;
            }

            try
            {
                var locateMrpInventory = MrpInventory.Locate(new Standalone.AMRPItemSite
                {
                    InventoryID = row.InventoryID,
                    SiteID = row.SiteID,
                    SubItemID = SubItemEnabled ? row.SubItemID : 0
                });

                if (locateMrpInventory?.ProductManagerID != null)
                {
                    row.ProductManagerID = locateMrpInventory.ProductManagerID;
                }

                MrpExceptionRecs.Insert(row);
            }
            catch (Exception e)
            {
                PXTrace.WriteError(e);

                var sb = new System.Text.StringBuilder();

                sb.Append($"{Messages.GetLocal(Messages.InsertErrorRowSkipped, Common.Cache.GetCacheName(typeof(AMRPExceptions)))}. ");

                if (TryCreateInventoryIdDisplayValue<AMRPExceptions.inventoryID>(MrpExceptionRecs.Cache, row.InventoryID,
                    out var inventoryIdDisplay))
                {
                    sb.Append($"{inventoryIdDisplay}. ");
                }

                if (TryCreateSiteIdDisplayValue<AMRPExceptions.siteID>(MrpExceptionRecs.Cache, row.SiteID,
                    out var siteIdDisplay))
                {
                    sb.Append($"{siteIdDisplay}. ");
                }

                if (TryCreateRefNoteIdDisplayValue<AMRPExceptions.refNoteID>(MrpExceptionRecs.Cache, row.RefNoteID,
                    out var refNoteDisplay))
                {
                    sb.Append($"{refNoteDisplay}. ");
                }

                InsertAuditRec(sb.ToString(), AMRPAuditTable.MsgTypes.Warning);

                sb.Append(e.Message);

                PXTrace.WriteInformation(sb.ToString());
                InsertAuditRec($"** {e.Message} **", AMRPAuditTable.MsgTypes.Warning);
            }
        }

        protected virtual AMRPExceptions MapToMrpException(AMRPDetailFP mrpDetailFp)
        {
            return new AMRPExceptions
            {
                Qty = mrpDetailFp.Qty,
                InventoryID = mrpDetailFp.InventoryID,
                SubItemID = mrpDetailFp.SubItemID,
                PromiseDate = mrpDetailFp.PlanDate ?? mrpDetailFp.RequiredDate ?? ProcessDateTime,
                RequiredDate = mrpDetailFp.RequiredDate ?? mrpDetailFp.PlanDate ?? ProcessDateTime,
                RefNbr = mrpDetailFp.RefNbr,
                RefNoteID = mrpDetailFp.RefNoteID,
                RefType = mrpDetailFp.Type == MRPPlanningType.MrpRequirement ? mrpDetailFp.RefType : mrpDetailFp.Type,
                SiteID = mrpDetailFp.SiteID,
                ItemClassID = mrpDetailFp.ItemClassID,
				BranchID = mrpDetailFp.BranchID
            };
        }

        /// <summary>
        /// Bring all MRP Info into a single table (AMRPPlan)
        /// </summary>
        protected virtual void LoadMrpPlan()
        {
            InsertAuditRec(Messages.GetLocal(Messages.MRPEngineCreateMRPPlan));

            foreach (AMRPDetail mrpDetail in PXSelect<AMRPDetail,
                Where<AMRPDetail.type, NotEqual<MRPPlanningType.mps>>>.Select(this))
            {
                var mrpPlan = new AMRPPlan
                {
                    InventoryID = mrpDetail.InventoryID,
                    SubItemID = mrpDetail.SubItemID,
                    SiteID = mrpDetail.SiteID,
                    UOM = mrpDetail.BaseUOM,
                    ActionDate = mrpDetail.ActionDate,
                    BaseQty = mrpDetail.BaseQty,
                    ParentInventoryID = mrpDetail.ParentInventoryID,
                    ParentSubItemID = mrpDetail.ParentSubItemID,
                    ProductInventoryID = mrpDetail.ProductInventoryID,
                    ProductSubItemID = mrpDetail.ProductSubItemID,
                    PromiseDate = mrpDetail.PromiseDate,
                    RefType = mrpDetail.RefType,
                    RefNbr = mrpDetail.RefNbr,
                    RefNoteID = mrpDetail.RefNoteID,
                    Type =
                        mrpDetail.Type == MRPPlanningType.SafetyStock ||
						mrpDetail.Type == MRPPlanningType.ReorderPoint ||
                        mrpDetail.Type == MRPPlanningType.StockAdjustment ||
                        mrpDetail.Type == MRPPlanningType.PlannedTransferDemand ||
                        mrpDetail.Type == MRPPlanningType.MrpRequirement
                            ? mrpDetail.Type
                            : MRPPlanningType.MrpPlan,
                    SDflag = MRPSDFlag.Supply,
					IsPlan = true,
					BranchID = mrpDetail.BranchID
                };

                if (mrpDetail.ProductRefNoteID != null)
                {
                    mrpPlan.RefNoteID = mrpDetail.ProductRefNoteID;
                    mrpPlan.RefNbr = mrpDetail.ProductRefNbr;
                }
                else if (mrpDetail.ParentRefNoteID != null)
                {
                    mrpPlan.RefNoteID = mrpDetail.ParentRefNoteID;
                    mrpPlan.RefNbr = mrpDetail.ParentRefNbr;
                }
                              
                this.MrpPlan.Insert(mrpPlan);
            }

            foreach (PXResult<AMRPDetailFP, InventoryItemMRP> result in PXSelectJoin<AMRPDetailFP,
                InnerJoin<InventoryItemMRP, On<InventoryItemMRP.inventoryID, Equal<AMRPDetailFP.inventoryID>>>,
                Where<AMRPDetailFP.type, NotEqual<MRPPlanningType.stockAdjustment>,
                    And<AMRPDetailFP.type, NotEqual<MRPPlanningType.safetyStock>,
					And<AMRPDetailFP.type, NotEqual<MRPPlanningType.reorderPoint>,
						And<AMRPDetailFP.onHoldStatus, NotEqual<OnHoldStatus.onHoldExclude>,
							 And<AMRPDetailFP.type, NotEqual<MRPPlanningType.consolidated>>
						>>>>>.Select(this))
            {
                var mrpDetailFp = result.GetItem<AMRPDetailFP>();
                var inventoryItem = result.GetItem<InventoryItemMRP>();

                if (inventoryItem?.BaseUnit == null)
                {
                    continue;
                }

                var mrpPlan = new AMRPPlan
                {
                    InventoryID = mrpDetailFp.InventoryID,
                    SubItemID = mrpDetailFp.SubItemID,
                    SiteID = mrpDetailFp.SiteID,
                    UOM = inventoryItem.BaseUnit,
                    ActionDate = mrpDetailFp.RequiredDate,
                    BaseQty = mrpDetailFp.OriginalQty * (mrpDetailFp.SDFlag == MRPSDFlag.Supply ? 1 : -1),
                    ParentInventoryID = mrpDetailFp.ParentInventoryID,
                    ParentSubItemID = mrpDetailFp.ParentSubItemID,
                    ProductInventoryID = mrpDetailFp.ProductInventoryID,
                    ProductSubItemID = mrpDetailFp.ProductSubItemID,
                    PromiseDate = mrpDetailFp.PlanDate,
                    RefType = mrpDetailFp.RefType,
                    Type = mrpDetailFp.Type == MRPPlanningType.MrpRequirement ? mrpDetailFp.RefType : mrpDetailFp.Type,
                    SDflag = mrpDetailFp.SDFlag,
                    RefNbr = mrpDetailFp.RefNbr,
					RefNoteID = mrpDetailFp.RefNoteID,
					IsPlan = mrpDetailFp.Type == MRPPlanningType.MrpPlan
					   || mrpDetailFp.Type == MRPPlanningType.MrpRequirement
					   || mrpDetailFp.Type == MRPPlanningType.MPS
					   || mrpDetailFp.Type == MRPPlanningType.ForecastDemand
					   || mrpDetailFp.Type == MRPPlanningType.SafetyStock
					   || mrpDetailFp.Type == MRPPlanningType.ReorderPoint
					   || mrpDetailFp.Type == MRPPlanningType.StockAdjustment,
					BranchID = mrpDetailFp.BranchID
				};

                if (mrpDetailFp.ProductRefNoteID != null)
                {
                    mrpPlan.RefNoteID = mrpDetailFp.ProductRefNoteID;
                    mrpPlan.RefNbr = mrpDetailFp.ProductRefNbr;
                }
                else if (mrpDetailFp.ParentRefNoteID != null)
                {
                    mrpPlan.RefNoteID = mrpDetailFp.ParentRefNoteID;
                    mrpPlan.RefNbr = mrpDetailFp.ParentRefNbr;
                }

				if (mrpDetailFp.Type == MRPPlanningType.Consolidated)
				{
					mrpPlan.BaseQty = mrpPlan.BaseQty + mrpDetailFp.ConsolidatedSafetyQty;
				}

				this.MrpPlan.Insert(mrpPlan);
            }
        }

        protected virtual MRPProcessCache.MrpItemDictionary CacheInventoryItems()
        {
            return MRPProcessCache.BuildMrpItemDictionary(this, SubItemEnabled);
        }

        /// <summary>
        /// Call requirements by each low level
        /// </summary>
        protected virtual void RequirementsByLowLevel(MRPProcessCache mrpProcessCache)
        {
            var sites = PXSelect<INSite>.Select(this).ToFirstTableList();

            for (var lowLevel = 0; lowLevel <= mrpProcessCache.MaxLowLevel; lowLevel++)
            {
                //Avoid checking small levels as most likely has items...
                if (lowLevel > 1 && !LevelHasItems(lowLevel))
                {
                    continue;
                }

                RequirementsByLowLevel(mrpProcessCache, lowLevel, sites);
            }
        }

		protected virtual void RequirementsByLowLevel(MRPProcessCache mrpProcessCache, int lowLevel, List<INSite> sites)
		{
			InsertAuditRec(Messages.GetLocal(Messages.MRPEngineProcessingLevel, lowLevel));

			AdjustFirstPassDemandQty(lowLevel, sites);
			if (Setup.Current.UseDaysSupplytoConsolidateOrders == true)
			{
				MRPConsolidationByLowLevelBySite(sites, lowLevel, mrpProcessCache);
			}

			InsertAuditRec(Messages.GetLocal(Messages.MRPEngineProcessingLevelAdjustComplete, lowLevel));

			Blowdown(mrpProcessCache, lowLevel);

			InsertDebugMemoryAuditRec();
			InsertDebugCachedCounts();

			PersistFirstPassRecs();
			PersistFirstPassRecsCleanup(lowLevel);
			PersistSchedule();
		}

		private void TransferSupplyFpToFpCache()
        {
            foreach (AMRPDetailFPSupply supplyRow in MrpDetailFirstPassSupply.Cache.Cached)
            {
                if (supplyRow?.RecordID == null)
                {
                    continue;
                }

                var supplyStatus = MrpDetailFirstPassSupply.Cache.GetStatus(supplyRow);
                if (supplyStatus == PXEntryStatus.Notchanged)
                {
                    continue;
                }

				var fpRow = (AMRPDetailFP)MrpDetailFirstPassRecs.Cache.Locate(new AMRPDetailFP { RecordID = supplyRow.RecordID });
				if(fpRow?.RecordID == null)
				{
					continue;
				}

                var fpRowCopy = CopyValues(fpRow, supplyRow);
				var isTransfered = false;
                if (supplyStatus == PXEntryStatus.Inserted)
                {
                    MrpDetailFirstPassRecs.Insert(fpRowCopy);
					isTransfered = true;
                }

                if (supplyStatus == PXEntryStatus.Updated)
                {
                    MrpDetailFirstPassRecs.Update(fpRowCopy);
					isTransfered = true;
                }

                if (supplyStatus == PXEntryStatus.Deleted)
                {
                    MrpDetailFirstPassRecs.Delete(fpRowCopy);
					isTransfered = true;
                }

				if(isTransfered)
				{
					MrpDetailFirstPassSupply.Cache.Remove(supplyRow);
				}
            }
        }

		private AMRPDetailFP CopyValues(AMRPDetailFP fpRow, AMRPDetailFPSupply supplyRow)
        {
            if (fpRow?.RecordID == null || supplyRow?.RecordID == null || fpRow.RecordID != supplyRow.RecordID)
            {
                return fpRow;
            }

            var copy = (AMRPDetailFP)MrpDetailFirstPassRecs.Cache.CreateCopy(fpRow);
            copy.InventoryID = supplyRow.InventoryID;
            copy.LowLevel = supplyRow.LowLevel;
            copy.OriginalQty = supplyRow.OriginalQty;
            copy.Qty = supplyRow.Qty;
            copy.SuppliedQty = supplyRow.SuppliedQty;
            copy.SiteID = supplyRow.SiteID;
            copy.SubItemID = supplyRow.SubItemID;
            copy.Type = supplyRow.Type;
            copy.PlanDate = supplyRow.PlanDate;
            copy.RequiredDate = supplyRow.RequiredDate;

            return copy;
        }

		protected virtual void PersistFirstPassRecs()
		{
			if (MrpDetailFirstPassRecs.Cache.Inserted.Count() > HIGHRECORDCOUNT ||
				MrpDetailFirstPassRecs.Cache.Updated.Count() > HIGHRECORDCOUNT ||
				MrpDetailFirstPassRecs.Cache.Deleted.Count() > HIGHRECORDCOUNT)
			{
				InsertAuditRec(Messages.GetLocal(Messages.SavingDacName, Common.Cache.GetCacheName(typeof(AMRPDetailFP))));
			}
			TransferSupplyFpToFpCache();

			var plansInserted = MrpDetailFirstPassRecs.Cache.Inserted.Count();
			var plansUpdated = MrpDetailFirstPassRecs.Cache.Updated.Count();
			var plansDeleted = MrpDetailFirstPassRecs.Cache.Deleted.Count();
			
			if (plansDeleted > 0)
			{
				if (DebugMessages)
				{
					InsertAuditRec(String.Format("Plans Deleted:{0}", plansDeleted)); 
				}
				MrpDetailFirstPassRecs.Cache.Persist(PXDBOperation.Delete);
			}

			if (plansUpdated > 0)
			{
				if (DebugMessages)
				{
					InsertAuditRec(String.Format("Plans Updated:{0}", plansUpdated)); 
				}
				long batchSize = 1000;
				if (plansUpdated < 100)
				{
					MrpDetailFirstPassRecs.Cache.Persist(PXDBOperation.Update);
					plansUpdated = 0;
				}
				else
				{
					long i = 0;
					long batchCount = plansUpdated / batchSize;
					var parms = new PXDataFieldRestrict[plansUpdated > batchSize ? batchSize : plansUpdated];
					foreach (AMRPDetailFP rec in MrpDetailFirstPassRecs.Cache.Updated)
					{
						var batchID = i / batchSize;
						var id = i % batchSize;
						parms[id] = new PXDataFieldRestrict("RecordID", PXDbType.Int, 0, rec.RecordID);
						parms[id].OrOperator = true;
						MrpDetailFirstPassRecs.Cache.SetStatus(rec, PXEntryStatus.Inserted);
						if (id == batchSize - 1)
						{
							PXDatabase.Delete<AMRPDetailFP>(parms);
							if (batchID == batchCount-1)
							{
								long rest = plansUpdated - batchSize * batchCount;
								parms = (rest >0 ) ? new PXDataFieldRestrict[rest] : null;
							}
						}

						i++;
					}

					if (parms != null)
					{
						PXDatabase.Delete<AMRPDetailFP>(parms);
					}

				}
			}
			
			if (plansInserted > 0 || plansUpdated > 0)
			{
				if (DebugMessages)
				{
					InsertAuditRec(String.Format("Plans Inserted:{0}", plansInserted)); 
				}
				PXBulkInsert<AMRPDetailFP>.Insert(this, MrpDetailFirstPassRecs.Cache.Inserted);
			}

			if (MrpDetailFirstPassSupply.Cache.Inserted.Any_())
            {
                MrpDetailFirstPassSupply.Cache.Persist(PXDBOperation.Insert);
            }

			if (MrpDetailFirstPassSupply.Cache.Updated.Any_())
            {
                MrpDetailFirstPassSupply.Cache.Persist(PXDBOperation.Update);
            }

			if (MrpDetailFirstPassSupply.Cache.Deleted.Any_())
            {
                MrpDetailFirstPassSupply.Cache.Persist(PXDBOperation.Delete);
            }

			// Required to clear persistedItems for repeat cache update
			MrpDetailFirstPassRecs.Cache.Persisted(false);
			MrpDetailFirstPassSupply.Cache.Persisted(false);

			this.TimeStamp = PXDatabase.SelectTimeStamp();
		}

		/// <summary>
		/// Run after PersistFirstPassRecs to cleanup cached AMRPDetailFP
		/// </summary>
		/// <param name="lowLevel">The current low level being processed</param>
		protected virtual void PersistFirstPassRecsCleanup(int lowLevel)
		{
			foreach (AMRPDetailFP row in MrpDetailFirstPassRecs.Cache.Cached)
			{
				if (row.LowLevel > lowLevel && !MrpDetailFirstPassRecs.Cache.GetStatus(row).IsIn(PXEntryStatus.InsertedDeleted, PXEntryStatus.Deleted))
				{
					MrpDetailFirstPassRecs.Cache.SetStatus(row, PXEntryStatus.Held);
					continue;
				}

				MrpDetailFirstPassRecs.Cache.Remove(row);
			}

			MrpDetailFirstPassSupply.Cache.Clear();
			MrpDetailFirstPassRecs.Cache.ClearQueryCache();
			MrpDetailFirstPassSupply.Cache.ClearQueryCache();
		}

		protected virtual void PersistSchedule()
		{
			var persisted = false;
			if (ReuseScheduleNumbering || PersistPlannedOrderSchedule)
			{
				PersistCache<AMSchdItem>(true);
				persisted = true;
			}

			if (PersistPlannedOrderSchedule)
			{
				PersistCache<AMSchdOper>(true);
				PersistCache<AMSchdOperDetail>(true);
				PersistCache<AMWCSchdDetail>(true);
				PersistCache<AMWCSchd>();
				persisted = true;
			}

			if (!persisted)
			{
				return;
			}

			this.TimeStamp = PXDatabase.SelectTimeStamp();
		}

		private void PersistCache<Table>(bool clearCache = false)
			where Table : IBqlTable
		{
			var cache = Caches[typeof(Table)];
			if (cache == null)
			{
				return;
			}

			if (cache.Updated.Any_() || cache.Inserted.Any_() || cache.Deleted.Any_())
			{
				if (DebugMessages)
				{
					InsertAuditRec($"{cache.GetName()}, Inserted:{cache.Inserted.Count()}, Updated:{cache.Updated.Count()}, Deleted:{cache.Deleted.Count()}");
				}

				cache.Persist(PXDBOperation.Delete);
				cache.Persist(PXDBOperation.Update);
				if (cache.Identity == null)
				{
					PXBulkInsert<Table>.Insert(this, cache.Inserted);
				}
				else
				{
					cache.Insert(PXDBOperation.Update);
				}
				cache.Persisted(false);
			}

			if (clearCache)
			{
				cache.Clear();
			}
		}

		protected virtual DateTime GracePeriodDate(DateTime date, AMRPDetailFP detailFp)
        {
            try
            {
                return GracePeriodDate(date);
            }
            catch (ArgumentOutOfRangeException e)
            {
                InsertAuditRec(CreateDetailFpExceptionMessage(detailFp, e, Messages.GetLocal(Messages.UnableToCalcGracePeriod, Common.Dates.ToCultureShortDateString(this, date), Setup?.Current?.GracePeriod)), AMRPAuditTable.MsgTypes.Warning);
            }

            return date;
        }

        public virtual DateTime GracePeriodDate(DateTime date)
        {
            if (Setup.Current == null)
            {
                return date;
            }

            var gracePeriod = Setup.Current.GracePeriod.GetValueOrDefault();

            return date.AddDays(gracePeriod);
        }

        /// <summary>
        /// Storage for first pass supply.
        /// </summary>
        protected class FirstPassSupplyCollection
        {
            /// <summary>
            /// Storage by SiteID, InventoryID, SubitemID (if feature enabled)
            /// </summary>
            private Dictionary<string, List<AMRPDetailFPSupply>> _supplyDictionary;
            private readonly bool _storeBySubItem;
            private const string JoinKeySeperator = ":";
            private const string KeyDefault = "0";

            public FirstPassSupplyCollection(bool storeBySubItem)
            {
                _storeBySubItem = storeBySubItem;
                Clear();
            }

            public void Clear()
            {
                _supplyDictionary = new Dictionary<string, List<AMRPDetailFPSupply>>();
            }

            private string MakeKeyId(AMRPDetailFPSupply detailFp)
            {
                if (detailFp?.SiteID == null)
                {
                    throw new ArgumentNullException(nameof(detailFp));
                }

                return MakeKeyId(detailFp.SiteID, detailFp.InventoryID, detailFp.SubItemID);
            }

            private string MakeKeyId(AMRPDetailFP detailFp)
            {
                if (detailFp?.SiteID == null)
                {
                    throw new ArgumentNullException(nameof(detailFp));
                }

                return MakeKeyId(detailFp.SiteID, detailFp.InventoryID, detailFp.SubItemID);
            }

            private string MakeKeyId(int? siteId, int? inventoryId, int? subItemId)
            {
                return string.Join(JoinKeySeperator,
                    siteId?.ToString() ?? KeyDefault,
                    inventoryId?.ToString() ?? KeyDefault,
                    _storeBySubItem ? subItemId?.ToString() ?? KeyDefault : KeyDefault);
            }

            public void MergeSupply(IEnumerable<AMRPDetailFPSupply> detailFps)
            {
                if (detailFps == null)
                {
                    return;
                }

                foreach (var detailFp in detailFps)
                {
                    MergeSupply(detailFp);
                }
            }

            public void MergeSupply(AMRPDetailFPSupply detailFp)
            {
                var key = MakeKeyId(detailFp);
                if (_supplyDictionary.TryGetValue(key, out var value))
                {
                    _supplyDictionary[key] = MergeSupplyList(detailFp, value);
                    return;
                }

                _supplyDictionary.Add(key, new List<AMRPDetailFPSupply> { detailFp });
            }

            private List<AMRPDetailFPSupply> MergeSupplyList(AMRPDetailFPSupply newDetailFp, List<AMRPDetailFPSupply> existingDetailFp)
            {
                if (existingDetailFp == null || newDetailFp?.RecordID == null)
                {
                    return existingDetailFp ?? new List<AMRPDetailFPSupply>();
                }

                var newList = new List<AMRPDetailFPSupply>();
                var added = false;
                foreach (var amrpDetailFp in existingDetailFp)
                {
                    if (amrpDetailFp?.RecordID == newDetailFp.RecordID)
                    {
                        newList.Add(newDetailFp);
                        added = true;
                        continue;
                    }
                    newList.Add(amrpDetailFp);
                }

                if (!added)
                {
                    newList.Add(newDetailFp);
                }

                return newList;
            }

            private IEnumerable<AMRPDetailFPSupply> FindSupply(AMRPDetailFP demand)
            {
                if (_supplyDictionary.TryGetValue(MakeKeyId(demand), out var supply))
                {
                    return supply ?? new List<AMRPDetailFPSupply>();
                }

                return new List<AMRPDetailFPSupply>();
            }

            public IEnumerable<AMRPDetailFPSupply> GetMatchedSupply(PXCache supplyCache, AMRPDetailFP demand, DateTime gracePeriodDate)
            {
                var list = new List<AMRPDetailFPSupply>();
                foreach (var supply in FindSupply(demand))
                {
                    if (supply?.PlanDate == null || !supply.PlanDate.LessThanOrEqualTo(gracePeriodDate))
                    {
                        continue;
                    }

                    var cachedSupply = supplyCache.LocateElse(supply);
                    if (supply.SuppliedQty.GetValueOrDefault() <= 0)
                    {
                        continue;
                    }

                    list.Add(cachedSupply);
                }

                return list.OrderBy(x => x.PlanDate);
            }
        }

        protected virtual void AdjustFirstPassDemandQty(int lowLevel, List<INSite> sites)
        {
            if (sites == null || sites.Count == 0)
            {
                return;
            }

			AdjustFirstPassDemandQtyByWarehouse(lowLevel, null, GetFirstPassSupply(lowLevel));
		}

		protected virtual void AdjustFirstPassDemandQtyByWarehouse(int lowLevel, INSite site, FirstPassSupplyCollection supplyCollection)
		{
			AdjustFirstPassDemandQtyByWarehouse(lowLevel, site, supplyCollection, GetMrpDetailFPDemand(lowLevel));
		}

		/// <summary>
		/// Adjust MRP data with matching supply and demand qty over dates.
		/// </summary>
		/// <param name="lowLevel">Current low level being processed.</param>
		/// <param name="site">Not used and will be removed in a future release.</param>
		/// <param name="supplyCollection">Supply data to match with demand.</param>
		/// <param name="mrpDetailFPDemands">Demand data to match to supply.</param>
		protected virtual void AdjustFirstPassDemandQtyByWarehouse(int lowLevel, INSite site, FirstPassSupplyCollection supplyCollection, IEnumerable<AMRPDetailFP> mrpDetailFPDemands)
        {
			// INSite no longer used
			AdjustFirstPassDemandQty(_mrpProcessCache, lowLevel, supplyCollection, mrpDetailFPDemands, 0);
		}

		protected virtual void AdjustFirstPassDemandQty(MRPProcessCache mrpProcessCache, int lowLevel, FirstPassSupplyCollection supplyCollection, IEnumerable<AMRPDetailFP> mrpDetailFPDemands, int adjustmentCounter)
		{ 
            int? previousInventoryID = 0;
            int? previousSubItemID = 0;
            int? previousSiteID = 0;
            decimal remainingSupplyQty = 0;
			var pendingDemandAdjustments = new List<AMRPDetailFP>();
			var newTransferDemands = new List<AMRPDetailFP>();
			var isSkipItem = false;
			var isTransferItemSite = false;
			var isTransferPossible = mrpProcessCache?.MrpItems != null && mrpProcessCache.MrpItems.HasTransferSites;

            //Loop assumes sort order of InventoryID, SubItemID, Siteid/SiteSequence, PlanDate, RecordID
            foreach (var mrpDetailFp in mrpDetailFPDemands)
            {
				if (isTransferPossible)
				{
					var isNewItem = previousInventoryID != mrpDetailFp.InventoryID;
					var isNewSite = previousSiteID != mrpDetailFp.SiteID;
					if (isNewItem)
					{
						isSkipItem = false;
					}

					if (isNewItem || isNewSite)
					{
						if (isTransferItemSite)
						{
							var newTRDemand = CreatePlanedTransfers(mrpProcessCache, lowLevel, previousInventoryID, previousSiteID);
							if (newTRDemand != null && newTRDemand.Any())
							{
								newTransferDemands.AddRange(newTRDemand);
								isSkipItem = !isNewItem;
								isTransferItemSite = false;
							}
						}

						if (!isSkipItem)
						{
							var itemCache = mrpProcessCache.MrpItems.GetCurrentItemCache(mrpDetailFp.InventoryID, mrpDetailFp.SiteID);
							isTransferItemSite = itemCache != null && itemCache.ReplenishmentSource == INReplenishmentSource.Transfer && itemCache.ReplenishmentSiteID != null;
						}
					}

					if (isSkipItem)
					{
						pendingDemandAdjustments.Add(mrpDetailFp);
						continue;
					}
				}

                var oldqty = mrpDetailFp.Qty.GetValueOrDefault();

                if (previousSiteID == mrpDetailFp.SiteID
                    && previousInventoryID == mrpDetailFp.InventoryID
                    && (previousSubItemID == mrpDetailFp.SubItemID || !SubItemEnabled)
                    && remainingSupplyQty >= oldqty)
                {
                    remainingSupplyQty -= oldqty;
                    mrpDetailFp.Qty = 0;
                    mrpDetailFp.SuppliedQty = mrpDetailFp.Qty.GetValueOrDefault();
                }
                else
                {
                    var newQty = AdjustFirstPassSupplyQty(mrpDetailFp, supplyCollection);

                    if (newQty < 0)
                    {
                        remainingSupplyQty = Math.Abs(newQty);
                        newQty = 0;
                    }

                    if (newQty != oldqty)
                    {
                        mrpDetailFp.Qty = newQty;
                        mrpDetailFp.SuppliedQty += oldqty - newQty;
                        if (newQty == 0)
                        {
                            mrpDetailFp.Processed = true;
                        }
                    }
                }

                previousInventoryID = mrpDetailFp.InventoryID;
                previousSubItemID = mrpDetailFp.SubItemID;
                previousSiteID = mrpDetailFp.SiteID;

                MrpDetailFirstPassRecs.Update(mrpDetailFp);
            }

			// Covers last item
			if (isTransferItemSite)
			{
				var newTRDemand = CreatePlanedTransfers(mrpProcessCache, lowLevel, previousInventoryID, previousSiteID);
				if (newTRDemand != null && newTRDemand.Any())
				{
					newTransferDemands.AddRange(newTRDemand);
				}
			}

			if (!pendingDemandAdjustments.Any() && !newTransferDemands.Any())
			{
				return;
			}

			pendingDemandAdjustments.AddRange(newTransferDemands);

			AdjustFirstPassDemandQty(mrpProcessCache, lowLevel, supplyCollection, OrderDetailFP(pendingDemandAdjustments), ++adjustmentCounter);
        }

		protected virtual IEnumerable<AMRPDetailFPSupply> GetMrpDetailFPSupply(int lowLevel)
		{
			return MrpDetailFirstPassRecs.Cache.Cached
				.RowCast<AMRPDetailFP>()
				.Where(r =>
					r.LowLevel == lowLevel
					&& r.SuppliedQty > 0m
					&& r.SDFlag == MRPSDFlag.Supply
					&& MrpDetailFirstPassRecs.Cache.GetStatus(r)
								.IsNotIn(PXEntryStatus.Deleted, PXEntryStatus.InsertedDeleted))
				.OrderBy(r => r.SiteSequence).ThenBy(r => r.SiteID).ThenBy(r => r.PlanDate)
				.Select(r =>
					Utilities.Clone<AMRPDetailFP, AMRPDetailFPSupply>(this, r))
				.ToList();
		}

        /// <summary>
        /// Uses SQL Index: AMRPDetailFP_IX_MRPEngine_GetMrpDetailFPSupplyByWarehouse
        /// </summary>
		[Obsolete("This method has been deprecated and will be removed in Acumatica ERP 2024R2.")]
        protected virtual IEnumerable<AMRPDetailFPSupply> GetMrpDetailFPSupplyByWarehouse(int lowLevel, int? siteId)
        {
			return MrpDetailFirstPassRecs.Cache.Cached
				.RowCast<AMRPDetailFP>()
				.Where(r =>
					r.LowLevel == lowLevel
					&& r.SiteID == siteId
					&& r.SuppliedQty > 0m
					&& r.SDFlag == MRPSDFlag.Supply
					&& MrpDetailFirstPassRecs.Cache.GetStatus(r)
								.IsNotIn(PXEntryStatus.Deleted, PXEntryStatus.InsertedDeleted))
				.OrderBy(r => r.PlanDate)
				.Select(r =>
					Utilities.Clone<AMRPDetailFP, AMRPDetailFPSupply>(this, r))
				.ToList();
		}

		protected virtual IEnumerable<AMRPDetailFP> GetMrpDetailFPDemandByWarehouse(int lowLevel, int? siteId)
		{
			if (siteId == null)
			{
				return null;
			}

			return GetMrpDetailFPDemandByWarehouseBase(lowLevel, siteId)
				?.OrderBy(r => r.InventoryID).ThenBy(r => r.SubItemID).ThenBy(r => r.PlanDate).ThenBy(r => r.RecordID);
		}

		[Obsolete("This method has been deprecated and will be removed in Acumatica ERP 2024R2.")]
		protected virtual IEnumerable<AMRPDetailFP> GetMrpDetailFPLateDemandByWarehouse(int lowLevel, int? siteId)
		{
			if (siteId == null)
			{
				return null;
			}

			return GetMrpDetailFPDemandByWarehouseBase(lowLevel, siteId)
				?.Where(r => r.LateDemand == true && r.Processed == false)
				?.OrderBy(r => r.InventoryID).ThenBy(r => r.SubItemID).ThenBy(r => r.PlanDate).ThenBy(r => r.RecordID);
		}

		internal IEnumerable<AMRPDetailFP> GetMrpDetailFPDemandByWarehouseBase(int lowLevel, int? siteId)
		{
			return MrpDetailFirstPassRecs.Cache.Cached.RowCast<AMRPDetailFP>()
				.Where(r => r.LowLevel == lowLevel && r.SiteID == siteId && r.SDFlag == MRPSDFlag.Demand &&
					(r.OnHoldStatus == OnHoldStatus.NotOnHold || r.OnHoldStatus == OnHoldStatus.OnHoldInclude) &&
					MrpDetailFirstPassRecs.Cache.GetStatus(r).IsNotIn(PXEntryStatus.Deleted, PXEntryStatus.InsertedDeleted));
		}

		protected virtual IEnumerable<AMRPDetailFP> GetMrpDetailFPDemand(int lowLevel) => OrderDetailFP(GetMrpDetailFPDemandBase(lowLevel));

		public virtual IEnumerable<AMRPDetailFP> OrderDetailFP(IEnumerable<AMRPDetailFP> detailFP)
			=> detailFP?.OrderBy(r => r.InventoryID)
			.ThenBy(r => r.SubItemID)
			.ThenBy(r => r.SiteSequence)
			.ThenBy(r => r.SiteID)
			.ThenBy(r => r.PlanDate)
			.ThenBy(r => r.RecordID);

		internal IEnumerable<AMRPDetailFP> GetMrpDetailFPDemandBase(int lowLevel)
		{
			return MrpDetailFirstPassRecs.Cache.Cached.RowCast<AMRPDetailFP>()
				.Where(r => r.LowLevel == lowLevel && r.SDFlag == MRPSDFlag.Demand &&
					(r.OnHoldStatus == OnHoldStatus.NotOnHold || r.OnHoldStatus == OnHoldStatus.OnHoldInclude) &&
					MrpDetailFirstPassRecs.Cache.GetStatus(r).IsNotIn(PXEntryStatus.Deleted, PXEntryStatus.InsertedDeleted));
		}

		protected virtual decimal AdjustFirstPassSupplyQty(AMRPDetailFP mrpDetailFpDemand, FirstPassSupplyCollection supplyCollection)
        {
            if (mrpDetailFpDemand?.InventoryID == null || mrpDetailFpDemand.SiteID == null || Common.Dates.IsDateNull(mrpDetailFpDemand.PlanDate) || supplyCollection == null)
            {
                return 0;
            }

            var qtyAdjust = mrpDetailFpDemand.Qty.GetValueOrDefault();
            var gracePeriodDate = GracePeriodDate(mrpDetailFpDemand?.PlanDate ?? ProcessDateTime.Date, mrpDetailFpDemand);

            try
            {
                foreach (var mrpDetailFpSupply in supplyCollection.GetMatchedSupply(MrpDetailFirstPassSupply.Cache, mrpDetailFpDemand, gracePeriodDate))
                {
                    try
                    {
                        var suppliedQty = mrpDetailFpSupply.SuppliedQty.GetValueOrDefault();
                        if (suppliedQty <= 0)
                        {
                            continue;
                        }

                        var firstSupply = suppliedQty == mrpDetailFpSupply.OriginalQty.GetValueOrDefault();
#if DEBUG
                        AMDebug.TraceWriteLine(
                            $"Matching demand [{mrpDetailFpDemand.RecordID}] date {mrpDetailFpDemand.PlanDate.GetValueOrDefault().ToShortDateString()} to supply [{mrpDetailFpSupply.RecordID}] date {mrpDetailFpSupply.PlanDate.GetValueOrDefault().ToShortDateString()}");
#endif
                        if (qtyAdjust >= suppliedQty)
                        {
                            qtyAdjust -= suppliedQty;
                            suppliedQty = 0;
                        }
                        else
                        {
                            suppliedQty -= qtyAdjust;
                            qtyAdjust = 0;
                        }

                        if (firstSupply || mrpDetailFpSupply.RequiredDate.GreaterThan(mrpDetailFpDemand.PlanDate))
                        {
                            // We want the required date to be the earliest date matched
                            mrpDetailFpSupply.RequiredDate = mrpDetailFpDemand.PlanDate;
                        }

                        mrpDetailFpSupply.SuppliedQty = suppliedQty;
                        MrpDetailFirstPassSupply.Update(mrpDetailFpSupply);

                        if (qtyAdjust <= 0)
                        {
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        PXTrace.WriteError(CreateDetailFpExceptionMessage(mrpDetailFpSupply, e, Messages.GetLocal(Messages.ErrorProcessingMrpDetailFpSupply)));
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                PXTrace.WriteError(e); 
                throw new Exception(CreateDetailFpExceptionMessage(mrpDetailFpDemand, e, Messages.GetLocal(Messages.ErrorAdjustingFirstPassSupplyQty)));
            }

            return qtyAdjust;
        }

		protected virtual IEnumerable<AMRPDetailFP> GetBlowdownItemTransferData(int lowLevel, int? inventoryID, int? siteID)
			=> GetBlowdownData(lowLevel)
			.Where(r => r.InventoryID == inventoryID && r.SiteID == siteID)
			.OrderBy(r => r.PlanDate).ThenBy(r => r.RecordID);

		protected virtual IEnumerable<AMRPDetailFP> GetBlowdownData(int lowLevel)
			=> MrpDetailFirstPassRecs.Cache.Cached.RowCast<AMRPDetailFP>()
				.Where(r => r.LowLevel == lowLevel && r.Processed != true &&
					(
						(r.SDFlag == MRPSDFlag.Demand && r.Qty > 0m)
						||
						(r.SDFlag == MRPSDFlag.Supply && r.Type == MRPPlanningType.MPS)
					) &&
					!MrpDetailFirstPassRecs.Cache.GetStatus(r).IsIn(PXEntryStatus.InsertedDeleted, PXEntryStatus.Deleted)
				);

		protected virtual void Blowdown(MRPProcessCache mrpProcessCache, int lowLevel)
		{
			var rows = GetBlowdownData(lowLevel)
				.OrderBy(r => r.PlanDate).ThenBy(r => r.RecordID).ToList();
			Blowdown(mrpProcessCache, lowLevel, rows);
		}

		protected virtual ProcessType CalculateProcessType(AMRPDetailFP detailFp, ItemCache itemCache)
		{
			if (detailFp == null || itemCache == null)
			{
				return ProcessType.Purchase;
			}

			var source = itemCache.ReplenishmentSource;
			var isNonStockKit = !itemCache.StkItem && itemCache.IsKitItem;
			if (isNonStockKit)
			{
				// Non stock items do not have source enabled/visible, so in this case want to run the same as a stock kit item
				source = INReplenishmentSource.KitAssembly;
			}

			switch (source)
			{
				case INReplenishmentSource.Transfer:
					if (itemCache.ReplenishmentSiteID != null)
					{
						return ProcessType.Transfer;
					}
					break;

				case INReplenishmentSource.KitAssembly:
					if (itemCache.IsKitItem && !string.IsNullOrWhiteSpace(itemCache.KitRevision))
					{
						return ProcessType.Kit;
					}
					break;

				case INReplenishmentSource.Manufactured:
					if (itemCache.IsManufacturedItem && !string.IsNullOrWhiteSpace(detailFp.BOMID))
					{
						return ProcessType.Bom;
					}
					break;
			}

			return ProcessType.Purchase;
		}

		protected virtual void Blowdown(MRPProcessCache mrpProcessCache, int lowLevel, List<AMRPDetailFP> rows)
		{
            var blowdownCounter = 0;
            foreach (var amrpDetailFp in rows)
			{
                if (MrpDetailFirstPassRecs.Cache.GetStatus(amrpDetailFp).IsIn(PXEntryStatus.Deleted, PXEntryStatus.InsertedDeleted))
                {
                    continue;
                }

				var itemCache = mrpProcessCache.MrpItems.GetCurrentItemCache(amrpDetailFp.InventoryID,
					amrpDetailFp.SiteID, amrpDetailFp.SubItemID);

				if (itemCache == null)
				{
					PXTrace.WriteWarning(Messages.GetLocal(Messages.MRPEngineUnableToFindItemInMRPCache,
						amrpDetailFp.InventoryID.GetValueOrDefault(), amrpDetailFp.RecordID));
                    continue;
				}

				if (itemCache.InvalidItemStatus)
				{
					continue;
				}

				if (string.IsNullOrWhiteSpace(amrpDetailFp.BOMID) && itemCache.IsManufacturedItem)
				{
					amrpDetailFp.BOMID = itemCache.BomID;
					amrpDetailFp.BOMRevisionID = itemCache.BomRevisionID;
				}

				var processType = CalculateProcessType(amrpDetailFp, itemCache);
				if (processType == ProcessType.Kit)
				{
					amrpDetailFp.KitRevisionID = itemCache.KitRevision;
				}

				if (PXAccess.FeatureInstalled<FeaturesSet.manufacturingMRP>()
					|| (PXAccess.FeatureInstalled<FeaturesSet.distributionReqPlan>() && processType != ProcessType.Bom))
				{
					if (processType != ProcessType.Transfer)
					{
						blowdownCounter++;
					}
					ProcessMrpDetailFP(mrpProcessCache, processType, amrpDetailFp);
				}

                amrpDetailFp.Processed = true;
                this.MrpDetailFirstPassRecs.Update(amrpDetailFp);

                // Message to help indicate process as still running for large MRP runs
                if (blowdownCounter % HIGHRECORDCOUNT == 0 && blowdownCounter != 0)
                {
                    InsertAuditRec(Messages.GetLocal(Messages.ProcessedRecordCountMessage, blowdownCounter));
                }
            }
			if(blowdownCounter > 0)
				InsertAuditRec(Messages.GetLocal(Messages.MRPEngineProcessingLevelBlowdownComplete, lowLevel, blowdownCounter));
        }

		[Obsolete("This method has been deprecated and will be removed in Acumatica ERP 2024R2.")]
		protected virtual INKitSpecHdr GetActiveKitAssembly(int? inventoryID)
		{
			return PXSelect<
				INKitSpecHdr,
				Where<INKitSpecHdr.kitInventoryID, Equal<Required<INKitSpecHdr.kitInventoryID>>,
					And<INKitSpecHdr.isActive.
					IsEqual<True>>>,
				OrderBy<
					Desc<INKitSpecHdr.createdDateTime, Desc<INKitSpecHdr.revisionID>>>>
				.SelectSingleBound(this, null, inventoryID);
		}

		public enum ProcessType
		{
			Purchase,
			Bom,
			Kit,
			Transfer
		}

		public Lazy<SchedulePlanBomCopy> scheduleBom = new Lazy<SchedulePlanBomCopy>(() => PXGraph.CreateInstance<SchedulePlanBomCopy>() );
		public MRPBomCache bomCache = null;
		public List<AMSchdItem> oldMRPSchdReferences = null;
		
        protected virtual void ProcessMrpDetailFP(MRPProcessCache mrpProcessCache, ProcessType processType, AMRPDetailFP amrpDetailFp)
        {
            if (amrpDetailFp == null)
            {
                throw new ArgumentNullException(Common.Cache.GetCacheName(typeof(AMRPDetailFP)));
            }

            if (amrpDetailFp.InventoryID == null)
            {
                var fieldDacName =
                    $"{Common.Cache.GetCacheName(typeof(AMRPDetailFP))} - {PXUIFieldAttribute.GetDisplayName<AMRPDetailFP.inventoryID>(this.MrpDetailFirstPassRecs.Cache)}";
                throw new ArgumentException(Messages.GetLocal(ErrorMessages.FieldIsEmpty, typeof(ErrorMessages)),
                    fieldDacName);
            }

            if (amrpDetailFp.SiteID == null)
            {
                var fieldDacName =
                    $"{Common.Cache.GetCacheName(typeof(AMRPDetailFP))} - {PXUIFieldAttribute.GetDisplayName<AMRPDetailFP.siteID>(this.MrpDetailFirstPassRecs.Cache)}";
                throw new ArgumentException(Messages.GetLocal(ErrorMessages.FieldIsEmpty, typeof(ErrorMessages)),
                    fieldDacName);
            }

            try
            {
                var itemCache = mrpProcessCache.MrpItems.GetCurrentItemCache(amrpDetailFp.InventoryID,
                    amrpDetailFp.SiteID, amrpDetailFp.SubItemID);

                //Calc action date from leadtime
                var planDate = amrpDetailFp.PlanDate ?? ProcessDateTime.Date;

				var leadTime = processType == ProcessType.Transfer ? itemCache.TransferLeadTime : itemCache.LeadTime;
				var actionDate = processType == ProcessType.Purchase
                    ? GetPurchaseActionDate(mrpProcessCache.PurchaseCalendar, planDate, itemCache.LeadTime)
                    : GetActionDate(planDate, leadTime);

                var qty = mrpProcessCache.DetailSupply.GetSupply(amrpDetailFp.InventoryID, amrpDetailFp.SiteID,
                    amrpDetailFp.SubItemID, GracePeriodDate(planDate, amrpDetailFp), amrpDetailFp.Qty);
                var totalQty = 0m;

                if (qty <= 0)
                {
                    //No qty = no planning
                    return;
                }

				if (!itemCache.StkItem)
				{
					totalQty = qty;
				}

                AMSchdItem firstSchdItem = null;
                List<AMSchdItem> reuseSchdItems = null;

                while (qty > 0 && itemCache.StkItem)
                {
                    var reorderQty = InventoryHelper.ReorderQuantity(qty,
                        itemCache.MinOrderQty,
                        itemCache.MaxOrderQty,
                        itemCache.LotSize);

                    if (reorderQty <= 0)
                    {
                        break;
                    }

					if (reorderQty > qty)
                    {
                        // Cache excess qty
                        mrpProcessCache.DetailSupply.AddSupply(amrpDetailFp.InventoryID, amrpDetailFp.SiteID,
                            amrpDetailFp.SubItemID, planDate, reorderQty - qty);
                    }

                    var amrpDetail = new AMRPDetail
                    {
                        InventoryID = amrpDetailFp.InventoryID,
                        SiteID = amrpDetailFp.SiteID,
                        SubItemID = amrpDetailFp.SubItemID,
                        Type = amrpDetailFp.Type,
                        RefType = amrpDetailFp.RefType,
                        SDFlag = amrpDetailFp.SDFlag,
                        BOMLevel = 0,
                        IsSub = false,
                        ParentInventoryID = amrpDetailFp.ParentInventoryID,
                        ParentSubItemID = amrpDetailFp.ParentSubItemID,
                        ProductInventoryID = amrpDetailFp.ProductInventoryID,
                        ProductSubItemID = amrpDetailFp.ProductSubItemID,
                        BaseQty = reorderQty,
                        PromiseDate = planDate,
                        ActionDate = actionDate,
                        ActionLeadTime = leadTime,
                        ReplenishmentSource = itemCache.ReplenishmentSource,
                        PreferredVendorID = itemCache.PreferredVendorID,
                        ProductManagerID = itemCache.ProductManagerID,
                        BaseUOM = itemCache.BaseUnit,
                        DetailFPRecordID = amrpDetailFp.RecordID,
                        BOMID = amrpDetailFp.BOMID,
                        BOMRevisionID = amrpDetailFp.BOMRevisionID,
                        RefNbr = amrpDetailFp.RefNbr,
                        RefNoteID = amrpDetailFp.RefNoteID,
                        ParentRefNbr = amrpDetailFp.ParentRefNbr ?? amrpDetailFp.RefNbr,
                        ParentRefNoteID = amrpDetailFp.ParentRefNoteID ?? amrpDetailFp.RefNoteID,
                        ProductRefNbr = amrpDetailFp.ProductRefNbr ?? amrpDetailFp.RefNbr,
                        ProductRefNoteID = amrpDetailFp.ProductRefNoteID ?? amrpDetailFp.RefNoteID,
                        ItemClassID = itemCache.IemClassID,
						BranchID = amrpDetailFp.BranchID,
						KitRevisionID = amrpDetailFp.KitRevisionID
					};

                    //splitting occurs for min orders
                    var amrpDetail2 = this.MrpDetailRecs.Insert(amrpDetail);
					if (processType == ProcessType.Transfer && itemCache.ReplenishmentSiteID != null && itemCache.ReplenishmentSiteID != amrpDetail2.SiteID)
					{
						if (mrpProcessCache.MrpItems.HasRecursiveTransferWarehouses(amrpDetail2.InventoryID))
						{
							// Create MRP Exception (check cache to make sure one doesn't yet exist for the item)
							var existing = MrpExceptionRecs.Cache.Cached.RowCast<AMRPExceptions>().Where(x => x.InventoryID == amrpDetail2.InventoryID
								&& x.SiteID == amrpDetail2.SiteID && x.Type == MRPExceptionType.ReplenishmentWarehouseLoop).FirstOrDefault_();
							if(existing == null)
							{
								InsertMrpException(new AMRPExceptions
								{
									InventoryID = amrpDetail2.InventoryID,
									SubItemID = amrpDetail2.SubItemID,
									PromiseDate = amrpDetail2.PromiseDate,
									RequiredDate = amrpDetail2.PromiseDate,
									Type = MRPExceptionType.ReplenishmentWarehouseLoop,
									SiteID = amrpDetail2.SiteID,
									ItemClassID = amrpDetail2.ItemClassID,
									BranchID = amrpDetail2.BranchID
								});
							}
						}
						else
						{
							amrpDetail2.TransferSiteID = itemCache.ReplenishmentSiteID;
							amrpDetail2 = this.MrpDetailRecs.Update(amrpDetail2);
							var transferDemandDetailFP = CreatePlannedTransferDemand(amrpDetail2, itemCache);
							if (transferDemandDetailFP != null)
							{
								var transferItemCache = mrpProcessCache.MrpItems.GetCurrentItemCache(transferDemandDetailFP.InventoryID,
									transferDemandDetailFP.SiteID, transferDemandDetailFP.SubItemID);
								transferDemandDetailFP.SiteSequence = transferItemCache?.SiteSequence ?? 0;
								this.MrpDetailFirstPassRecs.Insert(transferDemandDetailFP);
								InsertMRPInventory(transferDemandDetailFP, mrpProcessCache.MrpItems);
							}
						}
					}

					// PROCESS BOM ITEMS
					if (processType == ProcessType.Bom && Features.MRPEnabled())
                    {
	                    if (bomCache == null)
	                    {
							bomCache = new MRPBomCache(this, true);
							scheduleBom.Value.MaterialCache = bomCache.AllMaterialCache;
						}
                        amrpDetail2.IsSub = true;

                        // Get the BOM Revision By Date
                        var bomItem = LookupBomsByDate ? bomCache.GetActiveRevisionBomItemByDate(amrpDetailFp.BOMID, actionDate) : null;

                        amrpDetail2.BOMID = bomItem != null ? bomItem.BOMID : amrpDetailFp.BOMID;
                        amrpDetail2.BOMRevisionID = bomItem != null ? bomItem.RevisionID : amrpDetailFp.BOMRevisionID;

                        var refOrderType = Setup?.Current?.PlanOrderType;
                        var refOrderNbr = string.Empty;

					 	var reuseSchdItem = SetRefOrderNumber(reuseSchdItems, ref refOrderType, ref refOrderNbr, amrpDetailFp, amrpDetail2);

                        if (string.IsNullOrWhiteSpace(amrpDetail2.BOMRevisionID) && !string.IsNullOrWhiteSpace(amrpDetail2.BOMID))
                        {
                            if (!string.IsNullOrWhiteSpace(itemCache?.BomID) &&
                                itemCache.BomID.Equals(amrpDetail2.BOMID) &&
                                (Common.Dates.IsDateNull(actionDate) || itemCache.IsDateBetweenBomDates(actionDate)))
                            {
                                amrpDetail2.BOMRevisionID = itemCache.BomRevisionID;
                            }

                            if (string.IsNullOrWhiteSpace(amrpDetail2.BOMRevisionID))
                            {
								bomItem = bomCache.GetActiveRevisionBomItemByDate(amrpDetail2.BOMID, actionDate);

                                if (bomItem?.BOMID == null)
                                {
									bomItem = bomCache.GetActiveRevisionBomItem(amrpDetail2.BOMID);
                                }
                                if (bomItem != null)
                                {
                                    amrpDetail2.BOMRevisionID = bomItem.RevisionID;
                                }
                            }
                        }

                        scheduleBom.Value.Clear();
                        scheduleBom.Value.ProcessingGraph = this;
                        scheduleBom.Value.RefNoteID = amrpDetailFp.RefNoteID ?? amrpDetailFp.ParentSchdNoteID;
                        scheduleBom.Value.BomItem.Current = bomItem;

						bomCache.ActivateBomRevision(amrpDetail2.BOMID, amrpDetail2.BOMRevisionID);
						var scheduledMrpDetail = ScheduleBom(scheduleBom.Value, amrpDetail2, refOrderType, refOrderNbr, reuseSchdItem);
                        if (scheduledMrpDetail == null)
                        {
                            break;
                        }
                        MrpDetailRecs.Update(scheduledMrpDetail);
                        var schdItem = scheduleBom.Value.CurrentSchdItem;
                        if (amrpDetailFp.PlanID != null && schdItem != null)
                        {
                            schdItem.MRPPlanID = amrpDetailFp.PlanID;
                            schdItem = SchdItem.Update(schdItem);
                        }
                        if(schdItem != null && firstSchdItem == null)
                        {
                            firstSchdItem = schdItem;
                        }
                    }

                    totalQty += reorderQty;
                    qty -= reorderQty;
                }

                //GENERATE REQUIREMENTS FOR COMPONENTS
                switch (processType)
                {
                    case ProcessType.Bom:
                        //Save material goes against the first planned bom reference order
                        SaveBomMaterial(mrpProcessCache.MrpItems,
                            UseFixLeadTime ? scheduleBom.Value.GetFixLeadTimeOrderMaterial() : scheduleBom.Value.GetOrderMaterial(),
                            totalQty, amrpDetailFp, firstSchdItem);
                        break;
                    case ProcessType.Kit:
						if (_kitMaterialCache == null)
						{
							LoadKitMaterialCache();
						}
						SaveKitMaterial(mrpProcessCache.MrpItems, amrpDetailFp, itemCache, totalQty);
						break;
                    default:
                        //purchase items -- all done
                        return;
                }

                // Transfer any audit messages
                foreach (AMRPAuditTable row in scheduleBom.Value.MrpAudit.Cache.Inserted)
                {
                    InsertAuditRec(row?.MsgText);
                }
                scheduleBom.Value.ClearMrpAudit();
            }
            catch (Exception e)
            {
                if (amrpDetailFp?.InventoryID == null)
                {
                    throw;
                }

                var msg = CreateDetailFpExceptionMessage(amrpDetailFp, e, Messages.GetLocal(Messages.MRPErrorProcessingDetailFP));
                throw new MRPRegenException(msg, e);
			}

            if (!PersistPlannedOrderSchedule)
            {
                if (!ReuseScheduleNumbering)
                {
                    Caches[typeof(AMSchdItem)].Clear();
                    Caches[typeof(AMSchdItem)].ClearQueryCache();
                }
                Caches[typeof(AMSchdOper)].Clear();
                Caches[typeof(AMSchdOper)].ClearQueryCache();
                Caches[typeof(AMSchdOperDetail)].Clear();
                Caches[typeof(AMSchdOperDetail)].ClearQueryCache();
                Caches[typeof(AMWCSchd)].Clear();
                Caches[typeof(AMWCSchdDetail)].Clear();
            }
        }

		protected AMSchdItem SetRefOrderNumber(List<AMSchdItem> reuseSchdItems, ref string refOrderType, ref string refOrderNbr, AMRPDetailFP amrpDetailFp, AMRPDetail amrpDetail2)
		{
			AMSchdItem reuseSchdItem = null;

			if (ReuseScheduleNumbering)
			{
				if(reuseSchdItems == null)
					reuseSchdItems = ReuseSchdItem(amrpDetailFp).ToList();

				if (reuseSchdItems?.Any() == true)
				{
					reuseSchdItem = reuseSchdItems.FirstOrDefault(x => this.Caches<AMSchdItem>()
						.GetStatus(this.Caches[typeof(AMSchdItem)].LocateElse(x)) == PXEntryStatus.Notchanged);
					if (reuseSchdItem != null)
					{
						refOrderType = reuseSchdItem?.OrderType;
						refOrderNbr = reuseSchdItem?.ProdOrdID;
						reuseSchdItems.Remove(reuseSchdItem);
					}
				}
			}

			if (string.IsNullOrWhiteSpace(refOrderNbr))
			{
				refOrderNbr = GetNextPlanOrderNumber(MrpDetailRecs.Cache, amrpDetail2);
			}

			return reuseSchdItem;
		}

		private List<INKitSpecStkDet> _kitMaterialCache;
		protected virtual void LoadKitMaterialCache()
		{
			_kitMaterialCache = PXSelectJoin<
				INKitSpecStkDet,
				InnerJoin<INKitSpecHdr,
					On<INKitSpecHdr.kitInventoryID, Equal<INKitSpecStkDet.kitInventoryID>,
					And<INKitSpecHdr.revisionID, Equal<INKitSpecStkDet.revisionID>>>>,
				Where<INKitSpecHdr.isActive, Equal<True>>>
				.Select(this).ToFirstTableList();
		}

		protected virtual IEnumerable<INKitSpecStkDet> GetKitMaterial(int? kitInventoryID, string kitRevisionID)
			=> _kitMaterialCache?.Where(r => r.KitInventoryID == kitInventoryID && r.RevisionID == kitRevisionID);

		protected virtual void SaveKitMaterial(MRPProcessCache.MrpItemDictionary mrpItemDictionary, AMRPDetailFP productDetailFp, ItemCache parentItemCache, decimal totalOrderQty)
		{
			var KitMaterialList = GetKitMaterial(productDetailFp.InventoryID, productDetailFp.KitRevisionID)?.ToList();
			if ((KitMaterialList?.Count ?? 0) == 0)
			{
				return;
			}

			string plannedKitRefNbr = GetNextPlannedKitAssemblyNbr();

			foreach (INKitSpecStkDet stkdetDetails in KitMaterialList)
			{
				var itemCache = mrpItemDictionary.GetCurrentItemCache(stkdetDetails.CompInventoryID, parentItemCache.SiteID, stkdetDetails.CompSubItemID);
				if (itemCache == null || itemCache.InvalidItemStatus)
				{
					continue;
				}

				var baseCompQty = stkdetDetails.DfltCompQty;
				if (itemCache.BaseUnit != stkdetDetails.UOM)
				{
					UomHelper.TryConvertToBaseQty<INKitSpecStkDet.compInventoryID>(this.Caches<INKitSpecStkDet>(), stkdetDetails, stkdetDetails.UOM, stkdetDetails.DfltCompQty.GetValueOrDefault(), out baseCompQty); 
				}

				var planningType = MRPPlanningType.PlannedKitDemand;
				if (!parentItemCache.StkItem)
				{
					planningType = productDetailFp.Type == MRPPlanningType.SalesOrder
						? MRPPlanningType.SOrderNonStockKitDemand
						: MRPPlanningType.ForecastNonStockKitDemand;
				}

				var row = new AMRPDetailFP
				{
					SDFlag = MRPSDFlag.Demand,
					Type = planningType,
					LowLevel = itemCache.LowLevel,
					InventoryID = stkdetDetails.CompInventoryID,
					SubItemID = stkdetDetails.CompSubItemID,
					SiteID = itemCache.SiteID,
					RefType = productDetailFp.RefType ?? productDetailFp.Type,
					ParentInventoryID = productDetailFp.InventoryID,
					ParentSubItemID = productDetailFp.SubItemID,
					ParentRefNbr = productDetailFp.RefNbr,
					ParentRefNoteID = productDetailFp.RefNoteID,
					ProductInventoryID = productDetailFp.ProductInventoryID,
					ProductSubItemID = productDetailFp.ProductSubItemID,
					ProductRefNbr = productDetailFp.ProductRefNbr ?? productDetailFp.ParentRefNbr ?? productDetailFp.RefNbr,
					ProductRefNoteID = productDetailFp.ProductRefNoteID ?? productDetailFp.ParentRefNoteID ?? productDetailFp.RefNoteID,
					PlanDate = productDetailFp.PlanDate,
					SuppliedQty = 0,
					Qty = totalOrderQty * baseCompQty.GetValueOrDefault(),
					BOMID = null,
					BOMRevisionID = null,
					RefNbr = parentItemCache.StkItem ? plannedKitRefNbr : productDetailFp.RefNbr,
					BAccountID = productDetailFp.BAccountID,
					ItemClassID = itemCache.IemClassID,
					BranchID = productDetailFp.BranchID
				};

				if (row.Qty.GetValueOrDefault() == 0)
				{
					continue;
				}

				if (row.Qty.GetValueOrDefault() < 0)
				{
					row.Qty = Math.Abs(row.Qty.GetValueOrDefault());
					row.SDFlag = MRPSDFlag.Supply;
					row.SuppliedQty = row.Qty.GetValueOrDefault();
				}
				row.OriginalQty = row.Qty.GetValueOrDefault();

				InsertMRPInventory(row, mrpItemDictionary);
				MrpDetailFirstPassRecs.Insert(row);
			}
		}

        protected virtual IEnumerable<AMRPDetailFP> CreatePlanedTransfers(MRPProcessCache mrpProcessCache, int lowLevel, int? inventoryID, int? siteID)
        {
            var before = GetInsertedPlannedTransferDemand(inventoryID, siteID);
            var hasBeforeEntires = (before?.Count() ?? 0) > 0;

            Blowdown(mrpProcessCache, lowLevel, GetBlowdownItemTransferData(lowLevel, inventoryID, siteID).ToList());

            var after = GetInsertedPlannedTransferDemand(inventoryID, siteID);
            if (after == null || !hasBeforeEntires)
            {
                return after;
            }

            var modifiedResults = new List<AMRPDetailFP>();
            foreach (var transferDemand in after)
            {
                if (transferDemand?.RecordID == null || before.Any(a => a.RecordID == transferDemand.RecordID))
                {
                    continue;
                }

                modifiedResults.Add(transferDemand);
            }
            return modifiedResults;
        }

        protected virtual IEnumerable<AMRPDetailFP> GetInsertedPlannedTransferDemand(int? inventoryID, int? siteID) => MrpDetailFirstPassRecs
            .Cache.Inserted.RowCast<AMRPDetailFP>()
            .Where(r => r.InventoryID == inventoryID && r.TransferSiteID == siteID && r.Type == MRPPlanningType.PlannedTransferDemand);

        /// <summary>
        /// Get the next plan order type numbering value based on the order type's prod numbering ID.
        /// Each call will use up the next number in the numbering sequence.
        /// </summary>
        /// <param name="cache">cache related to dataRow</param>
        /// <param name="dataRow">dataRow receiving the next number</param>
        /// <returns>Calculated next number</returns>
        private string GetNextPlanOrderNumber(PXCache cache, object dataRow)
        {
            if (PlanOrderType.Current == null)
            {
                throw new PXException(Messages.RecordMissing, Common.Cache.GetCacheName(typeof(AMOrderType)));
            }

            try
            {
                return AutoNumberAttribute.GetNextNumber(cache, dataRow, PlanOrderType.Current.ProdNumberingID, ProcessDateTime) ?? string.Empty;
            }
            catch (Exception e)
            {
                throw new MRPRegenException(Messages.GetLocal(Messages.UnableToGetNextNumber, PlanOrderType.Current.ProdNumberingID.TrimIfNotNullEmpty()), e);
            }
        }

        protected virtual IEnumerable<AMSchdItem> ReuseSchdItem(AMRPDetailFP detFp)
        {
            if(detFp?.InventoryID == null)
            {
                return null;
            }

			if (oldMRPSchdReferences == null)
			{
				oldMRPSchdReferences = SelectFrom<AMSchdItem>
					.Where<AMSchdItem.isPlan.IsEqual<True>>
					.View.Select(this).ToFirstTableList();
			}

			if (detFp.Type == MRPPlanningType.MrpRequirement)
            {
				return oldMRPSchdReferences
					.Where(r => r.MRPPlanID == null
					&& r.InventoryID == detFp.InventoryID
					&& r.SiteID == detFp.SiteID
					&& r.RefNoteID == (detFp.RefNoteID ?? detFp.ParentSchdNoteID));
			}

            if(detFp.PlanID == null)
            {
				return oldMRPSchdReferences
					.Where(r => r.MRPPlanID == null
					&& r.InventoryID == detFp.InventoryID
					&& r.SiteID == detFp.SiteID);
			}


			return oldMRPSchdReferences.Where(r => r.MRPPlanID == detFp.PlanID);
		}

        protected virtual void SaveBomMaterial(MRPProcessCache.MrpItemDictionary mrpItemDictionary, List<SchedulePlanBomCopy.AMPlanMaterial> planMaterialList, decimal totalOrderQty,
            AMRPDetailFP productDetailFp, AMSchdItem schdItem)
        {
            if (planMaterialList == null)
            {
                return;
            }

            foreach (var amPlanMaterial in planMaterialList)
            {
                var itemCache = mrpItemDictionary.GetCurrentItemCache(amPlanMaterial.InventoryID, amPlanMaterial.SiteID, amPlanMaterial.SubItemID);

                if (itemCache == null || itemCache.InvalidItemStatus)
                {
                    continue;
                }

                var totalQtyRequired = amPlanMaterial.GetTotalBaseReqQty(totalOrderQty, itemCache.QtyRoundUp);

                var row = new AMRPDetailFP
                {
                    SDFlag = MRPSDFlag.Demand,
                    Type = MRPPlanningType.MrpRequirement,
                    LowLevel = itemCache.LowLevel,
                    InventoryID = amPlanMaterial.InventoryID,
                    SubItemID = amPlanMaterial.SubItemID,
                    SiteID = amPlanMaterial.SiteID,
					SiteSequence = itemCache.SiteSequence,
                    RefType = productDetailFp.RefType ?? productDetailFp.Type,
                    ParentInventoryID = amPlanMaterial.ParentInventoryID,
                    ParentSubItemID = amPlanMaterial.ParentSubItemID,
                    ParentRefNbr = productDetailFp.RefNbr,
                    ParentRefNoteID = productDetailFp.RefNoteID,
                    ProductInventoryID = productDetailFp.ProductInventoryID,
                    ProductSubItemID = productDetailFp.ProductSubItemID,
                    ProductRefNbr = productDetailFp.ProductRefNbr ?? productDetailFp.ParentRefNbr ?? productDetailFp.RefNbr,
                    ProductRefNoteID = productDetailFp.ProductRefNoteID ?? productDetailFp.ParentRefNoteID ?? productDetailFp.RefNoteID,
                    PlanDate = amPlanMaterial.PlanDate,
                    SuppliedQty = 0,
                    Qty = totalQtyRequired,
                    BOMID = amPlanMaterial.CompBOMID,
                    BOMRevisionID = amPlanMaterial.CompBOMRevisionID,
                    RefNbr = schdItem?.OrderType == null ? null : RefNbrFieldAttribute.FormatFieldNbr(schdItem.OrderType, schdItem?.ProdOrdID),
                    //RefNoteID = schdItem?.NoteID, //skip the noteid so the refnbr is displayed
                    BAccountID = productDetailFp.BAccountID,
                    ItemClassID = itemCache.IemClassID,
					BranchID = productDetailFp.BranchID,
					ParentSchdNoteID = schdItem?.NoteID
				};

                if ( row.Qty.GetValueOrDefault() == 0)
                {
                    continue;
                }

                if (row.Qty.GetValueOrDefault() < 0)
                {
                    row.Qty = Math.Abs(row.Qty.GetValueOrDefault());
                    row.SDFlag = MRPSDFlag.Supply;
                    row.SuppliedQty = row.Qty.GetValueOrDefault();
                }
                row.OriginalQty = row.Qty.GetValueOrDefault();

                InsertMRPInventory(row, mrpItemDictionary);
                MrpDetailFirstPassRecs.Insert(row);
            }
        }

        protected virtual AMRPDetail ScheduleBom(SchedulePlanBomCopy scheduleBom, AMRPDetail amrpDetail, string orderType, string OrderNbr, AMSchdItem schdItem)
        {
            if (amrpDetail == null || string.IsNullOrWhiteSpace(orderType) || string.IsNullOrWhiteSpace(OrderNbr))
            {
                return null;
            }

            try
            {
                var order = new ProductionBomCopyBase.AMOrder
                {
                    OrderType = orderType,
                    OrderNbr = OrderNbr,
                    SourceID = amrpDetail.BOMID,
                    RevisionID = amrpDetail.BOMRevisionID,
                    InventoryID = amrpDetail.InventoryID,
                    SiteID = amrpDetail.SiteID,
                    PlanDate = amrpDetail.PromiseDate,
                    OrderQty = amrpDetail.BaseQty
                };

                if (order.SourceID == null || order.RevisionID == null)
                {
                    return null;
                }

                if (UseFixLeadTime)
                {
                    scheduleBom.CreateSchedule(order, schdItem, amrpDetail.ActionLeadTime.GetValueOrDefault());
                }
                else
                {
                    scheduleBom.CreateSchedule(order, schdItem);
                }

                amrpDetail.ActionDate = scheduleBom.CurrentSchdItem == null ? amrpDetail.PromiseDate : scheduleBom.CurrentSchdItem.StartDate;
                amrpDetail.ActionLeadTime = Common.Dates.DaysBetween(amrpDetail.PromiseDate, amrpDetail.ActionDate);
            }
            catch (InvalidBOMException bomException)
            {
                PXTrace.WriteWarning(bomException.Message);
            }
            catch (Exception exception)
            {
                AMDebug.TraceException(exception);

                InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, amrpDetail.InventoryID);

                var msg = Messages.GetLocal(Messages.MRPEngineUnableToScheduleFPRecordID, amrpDetail.DetailFPRecordID);
#if DEBUG
                AMDebug.TraceWriteMethodName(msg);
#endif
                if (item != null)
                {
                    msg = Messages.GetLocal(Messages.MRPEngineUnableToSchedule,
                        item.InventoryCD.TrimIfNotNullEmpty(), amrpDetail.BOMID, amrpDetail.BOMRevisionID,
                        amrpDetail.PromiseDate.GetValueOrDefault().ToShortDateString(),
                        amrpDetail.BaseQty.GetValueOrDefault());
                }

                PXTrace.WriteWarning(msg);
                PXTraceHelper.PxTraceException(exception);

                msg = $"{msg}. {Messages.GetLocal(Messages.Error)}: {exception.Message}";

                throw new PXException(msg);
            }

            if (Common.Dates.IsDefaultDate(amrpDetail.ActionDate))
            {
                amrpDetail.ActionDate = Common.Dates.IsDefaultDate(amrpDetail.PromiseDate) ? ProcessDateTime.Date : amrpDetail.PromiseDate;
                amrpDetail.ActionLeadTime = 0;
            }

            return amrpDetail;
        }

        /// <summary>
        /// Calculate the purchase action date
        /// </summary>
        /// <param name="purchaseCalendar"></param>
        /// <param name="dateTime">Plan Date</param>
        /// <param name="leadTime">Purchase Lead Time</param>
        /// <returns>MRP Action Date</returns>
        public virtual DateTime GetPurchaseActionDate(CalendarHelper purchaseCalendar, DateTime dateTime, int? leadTime)
        {
            var actionDateTime = GetActionDate(dateTime, leadTime);

            if (purchaseCalendar != null && !string.IsNullOrWhiteSpace(purchaseCalendar.CurrentCalendarId))
            {
				//if using the purchase calendar we will find the first available working day after calculating using the lead time
				//  This way the action date is the first working date as lead time is simply calendar days only (not working days)
				purchaseCalendar.ResetCalendarException();
				DateTime? adjustedActionDateTime = purchaseCalendar.GetNextWorkDay(actionDateTime, true);
                actionDateTime = adjustedActionDateTime ?? actionDateTime;
            }

            return actionDateTime;
        }

        public virtual DateTime GetActionDate(DateTime dateTime, int? leadTime)
        {
            try
            {
                return dateTime.AddDays(leadTime.GetValueOrDefault() * -1);
            }
            catch (ArgumentOutOfRangeException)
            {
                return dateTime;
            }
        }

        /// <summary>
        /// Create all of the first pass records
        /// </summary>
        /// <param name="mrpItemDictionary"></param>
        protected virtual void FirstPass(MRPProcessCache.MrpItemDictionary mrpItemDictionary)
        {
            MRPEngineFirstPass.LoadAll(this, mrpItemDictionary);
        }

        /// <summary>
        /// Insert related MRP Inventory record
        /// </summary>
        internal Standalone.AMRPItemSite InsertMRPInventory(AMRPDetailFP row, MRPProcessCache.MrpItemDictionary mrpItemDictionary)
        {
            return row == null ? null : InsertMRPInventory(row.InventoryID, row.SiteID, row.SubItemID, mrpItemDictionary);
        }

        /// <summary>
        /// Insert related MRP Inventory record
        /// </summary>
        internal Standalone.AMRPItemSite InsertMRPInventory(int? inventoryID, int? siteID, int? subitemID, MRPProcessCache.MrpItemDictionary mrpItemDictionary)
        {
            if (inventoryID == null
                || siteID == null
                || mrpItemDictionary == null)
            {
                return null;
            }

            if (SubItemEnabled && subitemID == null)
            {
                return null;
            }

            var newMrpInventory = new Standalone.AMRPItemSite
            {
                InventoryID = inventoryID,
                SiteID = siteID,
                SubItemID = SubItemEnabled ? subitemID : 0
            };

            if (MRPInventoryCacheExists(newMrpInventory))
            {
                return null;
            }

            try
            {
                var itemCache = mrpItemDictionary.GetCurrentItemCache(
                    newMrpInventory.InventoryID,
                    newMrpInventory.SiteID,
                    newMrpInventory.SubItemID);

                if (itemCache == null)
                {
                    return null;
                }

                newMrpInventory.LeadTime = itemCache.LeadTime;
                newMrpInventory.PreferredVendorID = itemCache.PreferredVendorID;
                newMrpInventory.ProductManagerID = itemCache.ProductManagerID;
                newMrpInventory.ReorderPoint = itemCache.ReorderPoint;
                newMrpInventory.ReplenishmentSource = itemCache.ReplenishmentSource;
                newMrpInventory.SafetyStock = itemCache.SafetyStock;
                newMrpInventory.LotSize = itemCache.LotSize;
                newMrpInventory.MinOrdQty = itemCache.MinOrderQty;
                newMrpInventory.MaxOrdQty = itemCache.MaxOrderQty;
				newMrpInventory.TransferLeadTime = itemCache.TransferLeadTime;
				newMrpInventory.ReplenishmentSiteID = itemCache.ReplenishmentSiteID;
				newMrpInventory.AMGroupWindow = itemCache.GroupWindow;

				return MrpInventory.Insert(newMrpInventory);
            }
            catch (Exception exception)
            {
                var sb = new System.Text.StringBuilder("Error inserting MRP inventory. ");

                InventoryItem inventoryItem = PXSelect<InventoryItem,
                    Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>
                        >.Select(this, newMrpInventory.InventoryID);

                if (inventoryItem == null)
                {
                    sb.AppendFormat("InventoryID [{0}] ", newMrpInventory.InventoryID);
                }
                else
                {
                    sb.AppendFormat("Inventory ID {0} ", inventoryItem.InventoryCD.TrimIfNotNullEmpty());
                }

                if (PXAccess.FeatureInstalled<FeaturesSet.warehouse>())
                {
                    INSite inSite = PXSelect<INSite,
                        Where<INSite.siteID, Equal<Required<INSite.siteID>>>
                    >.Select(this, newMrpInventory.SiteID);

                    if (inSite == null)
                    {
                        sb.AppendFormat("SiteID [{0}] ", newMrpInventory.SiteID);
                    }
                    else
                    {
                        sb.AppendFormat("Warehouse {0} ", inSite.SiteCD.TrimIfNotNullEmpty());
                    }
                }

                throw new MRPRegenException(sb.ToString(), exception);
            }
        }

        private bool MRPInventoryCacheExists(Standalone.AMRPItemSite mrpInventory)
        {
            return FindMRPInventory(mrpInventory) != null;
        }

        private Standalone.AMRPItemSite FindMRPInventory(Standalone.AMRPItemSite mrpInventory)
        {
            return (Standalone.AMRPItemSite)MrpInventory.Cache.Locate(mrpInventory);
        }

        /// <summary>
        /// Rebuild the low level codes
        /// </summary>
        protected virtual bool TryProcessLowLevel(out int maxLowLevel)
        {
#if DEBUG
            var sw = System.Diagnostics.Stopwatch.StartNew();
#endif
            maxLowLevel = 0;
            try
            {
                var ll = LowLevel.Construct();
                ll.SetAll();
                maxLowLevel = ll.CurrentMaxLowLevel;
                return !ll.ProcessLevelsSkipped;
            }
            catch (Exception exception)
            {
                throw new MRPRegenException(Messages.UnableToSetLL, exception);
            }
#if DEBUG
            finally
            {
                sw.Stop();
                PXTraceHelper.WriteTimespan(sw.Elapsed, "Set Low Level");
            }
#endif
        }

        /// <summary>
        /// Determine if the given level has low level items
        /// </summary>
        /// <param name="level">processing low level</param>
        /// <returns>True when an item with the given level is found</returns>
        protected virtual bool LevelHasItems(int level)
        {
            return (InventoryItemMfgOnly)PXSelect<InventoryItemMfgOnly,
                        Where<InventoryItemMfgOnly.aMLowLevel,
                            Equal<Required<InventoryItemMfgOnly.aMLowLevel>>>>
                    .SelectWindowed(this, 0, 1, level) != null;
        }

        /// <summary>
        /// Delete the old MRP data using PXDatabase.Delete
        /// </summary>
        /// <param name="includingAuditRecords">Should the AMRPAuditTable data be deleted (when true)</param>
        protected virtual void ClearAllMRPData(bool includingAuditRecords = true)
        {
            // Delete AMRPDetail first. This is the main planning table so if this is gone then MRP is really not functional (now that the delete is not in a Transaction)
            PXDatabase.Delete<AMRPDetail>();
            PXDatabase.Delete<AMRPExceptions>();

            if (ReuseScheduleNumbering)
            {
                // we will reuse schditems so no need to delete
                PXDatabase.Update<AMSchdItem>(
                    new PXDataFieldAssign<AMSchdItem.isMRP>(PXDbType.Bit, false),
                    new PXDataFieldRestrict<AMSchdItem.isPlan>(PXDbType.Bit, true)); 
            }
            else
            {
                PXDatabase.Delete<AMSchdItem>(new PXDataFieldRestrict<AMSchdItem.isPlan>(true));
            }

            // we want to wack all schd opers to always rebuild
            PXDatabase.Delete<AMSchdOper>(new PXDataFieldRestrict<AMSchdOper.isPlan>(PXDbType.Bit, true));
            // to correctly delete the AMWCSchdDetail records and update the parent we will delete these records later
            PXDatabase.Update<AMSchdOperDetail>(
                new PXDataFieldAssign<AMSchdOperDetail.isMRP>(PXDbType.Bit, false),
                new PXDataFieldRestrict<AMSchdOperDetail.isPlan>(PXDbType.Bit, true));

            PXDatabase.Delete<AMRPDetailFP>();
            PXDatabase.Delete<AMRPPlan>();
            PXDatabase.Delete<Standalone.AMRPItemSite>();
            PXDatabase.Delete<AMMRPBucketInq>();
            PXDatabase.Delete<AMMRPBucketDetailInq>();
            PXDatabase.Delete<AMRPDetailPlan>();

            if (includingAuditRecords)
            {
                DeleteAuditTable();
            }

            MRPSave();
        }

        protected virtual void MRPSave()
        {
            MRPSave(false);
        }

        protected virtual void MRPSave(bool asPersist)
        {
#if DEBUG
            AMDebug.TraceDirtyCaches(this);
            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
#endif
            if (IsHighRecordCount())
            {
                InsertAuditRec(Messages.GetLocal(Messages.SavingResults));
            }
                
            if (asPersist)
            {
                Persist();
                return;
            }

            this.Actions.PressSave();
#if DEBUG
            }
            finally
            {
                sw.Stop();
                if (sw.Elapsed.TotalSeconds > 5)
                {
                    var saveMsg = PXTraceHelper.CreateTimespanMessage(sw.Elapsed, "Press Save");
                    PXTraceHelper.WriteInformation(saveMsg);
                    InsertAuditRec(saveMsg);
                }
            }
#endif
			this.ClearUnusedCache();
		}

		public virtual void ClearUnusedCache()
		{
			this.ForecastRecs.Cache.Clear();
			this.ForecastRecs.Cache.ClearQueryCache();
			this.MPSRecs.Cache.Clear();
			this.MPSRecs.Cache.ClearQueryCache();

			Caches[typeof(AMSchdItem)].Clear();
			Caches[typeof(AMSchdItem)].ClearQueryCache();
			Caches[typeof(AMSchdOper)].Clear();
			Caches[typeof(AMSchdOper)].ClearQueryCache();
			Caches[typeof(AMSchdOperDetail)].Clear();
			Caches[typeof(AMSchdOperDetail)].ClearQueryCache();
			Caches[typeof(AMWCSchdDetail)].Clear();
			Caches[typeof(AMWCSchdDetail)].ClearQueryCache();
			Caches[typeof(AMProdItem)].Clear();
			Caches[typeof(AMProdItem)].ClearQueryCache();

			var mtdClearOriginalsAndExtensions = typeof(GraphHelper).GetMethod("ClearOriginalsAndExtensions", BindingFlags.Static | BindingFlags.NonPublic);
			mtdClearOriginalsAndExtensions?.Invoke(null, new object[] { this });
		}

		public override void Persist()
		{
			PXBulkInsert<AMRPDetailFP>.Insert(this, MrpDetailFirstPassRecs.Cache.Inserted);
			PXBulkInsert<AMRPDetailPlan>.Insert(this, MrpDetailPlanRecs.Cache.Inserted);
			PXBulkInsert<Standalone.AMRPItemSite>.Insert(this, MrpInventory.Cache.Inserted);
			PXBulkInsert<AMRPPlan>.Insert(this, MrpPlan.Cache.Inserted);
			PXBulkInsert<AMRPExceptions>.Insert(this, MrpExceptionRecs.Cache.Inserted);
			PXBulkInsert<AMRPDetail>.Insert(this, MrpDetailRecs.Cache.Inserted);

			base.Persist();
			this.SelectTimeStamp();
		}

		protected virtual bool IsHighRecordCount()
		{
			var totalRecords = MrpDetailFirstPassRecs.Cache.Inserted.Count() +
				MrpDetailFirstPassRecs.Cache.Updated.Count() +
				MrpDetailRecs.Cache.Inserted.Count() +
				MrpPlan.Cache.Inserted.Count() +
				MrpExceptionRecs.Cache.Inserted.Count();
			return totalRecords > HIGHRECORDCOUNT;
		}

        /// <summary>
        /// As we are trying to reuse plan order numbers we need to delete any unused plan records.
        /// </summary>
        protected virtual void ClearUnusedPlanDetail()
        {
            InsertAuditRec(Messages.GetLocal(Messages.Cleanup));

            PXDatabase.Delete<AMSchdItem>(
                new PXDataFieldRestrict<AMSchdItem.isPlan>(PXDbType.Bit, true),
                new PXDataFieldRestrict<AMSchdItem.isMRP>(PXDbType.Bit, false)
                );

            PXDatabase.Delete<AMSchdOper>(
                new PXDataFieldRestrict<AMSchdOper.isPlan>(PXDbType.Bit, true),
                new PXDataFieldRestrict<AMSchdOper.isMRP>(PXDbType.Bit, false)
            );

            //We want to update in cache to allow the sumcalc to work for the parent for plan totals
            foreach (AMWCSchdDetail wcSChdDetail in PXSelectJoin<AMWCSchdDetail,
                InnerJoin<AMSchdOperDetail,
                    On<AMWCSchdDetail.schdKey, Equal<AMSchdOperDetail.schdKey>>>,
                Where<AMSchdOperDetail.isPlan, Equal<True>,
                    And<AMSchdOperDetail.isMRP, Equal<False>>>>.Select(this))
            {
                if (wcSChdDetail?.RecordID == null)
                {
                    continue;
                }

                WCSChdDetail.Delete(wcSChdDetail);
            }

            PXDatabase.Delete<AMSchdOperDetail>(
                new PXDataFieldRestrict<AMSchdOperDetail.isPlan>(PXDbType.Bit, true),
                new PXDataFieldRestrict<AMSchdOperDetail.isMRP>(PXDbType.Bit, false)
            );
        }

        protected virtual void DeleteAuditTable()
        {
            PXDatabase.Delete<AMRPAuditTable>();
		}

        protected virtual void InsertAuditRec(string message, Guid? processGuid, TimeSpan timeSpan, int msgType = 0)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            if (timeSpan.Ticks == 0)
            {
                InsertAuditRec(message, processGuid, msgType);
                return;
            }

            InsertAuditRec($"{message} [{Common.Strings.TimespanFormatedDisplay(timeSpan)}]", processGuid, msgType);
        }

		public virtual void InsertAuditRec(string message)
        {
            InsertAuditRec(message, MrpRunTime.Current.CallingGraphUid);
        }

        protected virtual void InsertAuditRec(string message, int msgType)
        {
            InsertAuditRec(message, MrpRunTime.Current.CallingGraphUid, msgType);
        }

        protected virtual void InsertAuditRec(string message, Guid? processGuid)
        {
            InsertAuditRec(message, processGuid, AMRPAuditTable.MsgTypes.Default);
        }

		protected virtual void InsertAuditRec(string message, Guid? processGuid, int msgType)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            var row = new AMRPAuditTable
            {
                Selected = true,
                MsgText = message,
                MsgType = msgType
            };
#if DEBUG
            AMDebug.TraceWriteMethodName(message);
#endif
            //Inserting and then removing to get all of the fields initialed for a direct persist of the row
            var inserted = MrpAuditRecs.Insert(row);
            var copy = PXCache<AMRPAuditTable>.CreateCopy(inserted);
            MrpAuditRecs.Cache.Remove(inserted);
            MrpAuditRecs.Cache.PersistInserted(copy);

			MrpHistory.Insert(new AMRPHistory
			{
				ProcessID = (Guid?)UID,
				StartDateTime = MrpRunTime.Current.RunDateTime
			});

			var insertedAuditHistory = MrpAuditHistory.Insert(new AMRPAuditHistory
			{
				ProcessID = (Guid?)UID,
				MsgText = message,
				MsgType = msgType,
				CreatedDateTime = Common.Dates.Now
			});
			var copyAuditHistory = PXCache<AMRPAuditHistory>.CreateCopy(insertedAuditHistory);
			MrpAuditHistory.Cache.Remove(insertedAuditHistory);
			MrpAuditHistory.Cache.PersistInserted(copyAuditHistory);

			if (msgType == AMRPAuditTable.MsgTypes.Error)
			{
				var mrpHistory = GetCachedAMRPHistory((Guid?)UID);
				if (mrpHistory != null && mrpHistory.HasError != true)
				{
					mrpHistory.HasError = true;
					MrpHistory.Update(mrpHistory);
				}
				return;
			}

			if (msgType == AMRPAuditTable.MsgTypes.End)
			{
				EndCurrentMrpHistory((Guid?)UID);
			}

		}

		protected virtual void EndCurrentMrpHistory(Guid? processID)
		{
			var mrpHistory = GetCachedAMRPHistory(processID);
			if (mrpHistory != null)
			{
				mrpHistory.EndDateTime = Common.Dates.Now;
				mrpHistory.Duration = AMDateInfo.GetDateMinutes(mrpHistory.StartDateTime.GetValueOrDefault(), mrpHistory.EndDateTime.GetValueOrDefault());
				PersistMrpHistory(MrpHistory.Update(SetRecordCounts(mrpHistory)));
			}
		}

		protected virtual AMRPHistory SetRecordCounts(AMRPHistory mrpHistory)
		{
			mrpHistory.CountAMRPDetailPlan = GetRecordCount<AMRPDetailPlan, AMRPDetailPlan.planID>();
			mrpHistory.CountAMRPDetailFP = GetRecordCount<AMRPDetailFP, AMRPDetailFP.recordID>();
			mrpHistory.CountAMRPDetail = GetRecordCount<AMRPDetail, AMRPDetail.recordID>();
			mrpHistory.CountAMRPExceptions = GetRecordCount<AMRPExceptions, AMRPExceptions.recordID>();
			mrpHistory.CountAMRPPlan = GetRecordCount<AMRPPlan, AMRPPlan.recordID>();
			mrpHistory.CountAMRPItemSite = GetRecordCount<AMRPItemSite, AMRPItemSite.Tstamp>();

			return mrpHistory;
		}

		protected virtual int GetRecordCount<TTable, TField>()
			where TTable : class, IBqlTable, new()
			where TField : class, IBqlField
		{
			var results = PXSelectGroupBy<TTable, Aggregate<Count<TField>>>.Select(this);
			var cacheCount = this.Caches[typeof(TTable)].Inserted.Count();
			return (results?.RowCount ?? 0) + (int)cacheCount;
		}

		protected virtual void PersistMrpHistory(AMRPHistory mrpHistory)
		{
			var cacheStatus = MrpHistory.Cache.GetStatus(mrpHistory);
			var mrpHistoryCopy = PXCache<AMRPHistory>.CreateCopy(mrpHistory);
			if (cacheStatus == PXEntryStatus.Inserted)
			{
				MrpHistory.Cache.Remove(mrpHistory);
				MrpHistory.Cache.PersistInserted(mrpHistoryCopy);
			}

			if (cacheStatus == PXEntryStatus.Updated)
			{
				MrpHistory.Cache.Remove(mrpHistory);
				MrpHistory.Cache.PersistUpdated(mrpHistoryCopy);
			}
		}

		protected virtual AMRPHistory GetCachedAMRPHistory(Guid? processID)
		{
			if (MrpHistory.Current != null && MrpHistory.Current.ProcessID == processID)
			{
				return (AMRPHistory)MrpHistory.Cache.Locate(MrpHistory.Current);
			}

			return (AMRPHistory)MrpHistory.Cache.Locate(new AMRPHistory { ProcessID = processID });

		}

        protected virtual string CreateDetailFpExceptionMessage(AMRPDetailFP detailFp, Exception exception, string additionalMessage)
        {
            if (detailFp?.InventoryID == null)
            {
                return null;
            }

            var sb = new System.Text.StringBuilder();
            if (!string.IsNullOrWhiteSpace(additionalMessage))
            {
                sb.Append($"{additionalMessage}. ");
            }

            sb.Append($"{PXUIFieldAttribute.GetDisplayName<AMRPDetailFP.type>(MrpDetailFirstPassRecs.Cache)} = {MRPPlanningType.GetDescription(detailFp.Type)}. ");
            
            if (TryCreateInventoryIdDisplayValue<AMRPDetailFP.inventoryID>(MrpDetailFirstPassRecs.Cache, detailFp.InventoryID,
                out var inventoryIdDisplay))
            {
                sb.Append($"{inventoryIdDisplay}. ");
            }

            if (TryCreateSiteIdDisplayValue<AMRPDetailFP.siteID>(MrpDetailFirstPassRecs.Cache, detailFp.SiteID,
                out var siteIdDisplay))
            {
                sb.Append($"{siteIdDisplay}. ");
            }

            sb.Append($"{PXUIFieldAttribute.GetDisplayName<AMRPDetailFP.planDate>(MrpDetailFirstPassRecs.Cache)} = {detailFp.PlanDate?.ToShortDateString()}. ");
            sb.Append($"{PXUIFieldAttribute.GetDisplayName<AMRPDetailFP.qty>(MrpDetailFirstPassRecs.Cache)} = {detailFp.Qty.GetValueOrDefault()}. ");

            if (TryCreateRefNoteIdDisplayValue<AMRPDetailFP.refNoteID>(MrpDetailFirstPassRecs.Cache, detailFp.RefNoteID,
                out var refNoteDisplay))
            {
                sb.Append($"{refNoteDisplay}. ");
            }

            sb.Append($"{PXUIFieldAttribute.GetDisplayName<AMRPDetailFP.recordID>(MrpDetailFirstPassRecs.Cache)} = {detailFp.RecordID}. ");

            if (!string.IsNullOrWhiteSpace(detailFp.BOMID))
            {
                sb.Append($"{PXUIFieldAttribute.GetDisplayName<AMRPDetailFP.bOMID>(MrpDetailFirstPassRecs.Cache)} = {detailFp.BOMID}. ");
            }

            if (exception?.Message != null)
            {
                sb.Append(exception.Message);
            }

            return sb.ToString();
        }

        private static bool TryCreateInventoryIdDisplayValue<Field>(PXCache cache, int? inventoryId, out string msg)
            where Field : IBqlField
        {
            msg = null;
            if (inventoryId == null)
            {
                return false;
            }

            var inventoryCd = ((InventoryItem)PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(cache.Graph, inventoryId))?.InventoryCD;
            if (!string.IsNullOrWhiteSpace(inventoryCd))
            {
                msg = $"{PXUIFieldAttribute.GetDisplayName<Field>(cache)} = {inventoryCd.Trim()}";
            }

            return !string.IsNullOrWhiteSpace(msg);
        }

        private static bool TryCreateSiteIdDisplayValue<Field>(PXCache cache, int? siteId, out string msg)
            where Field : IBqlField
        {
            msg = null;
            if (siteId == null || !InventoryHelper.MultiWarehousesFeatureEnabled)
            {
                return false;
            }

            var siteCd = ((INSite)PXSelect<INSite, Where<INSite.siteID, Equal<Required<INSite.siteID>>>>.Select(cache.Graph, siteId))?.SiteCD;
            if (!string.IsNullOrWhiteSpace(siteCd))
            {
                msg = $"{PXUIFieldAttribute.GetDisplayName<Field>(cache)} = {siteCd.Trim()}";
            }

            return !string.IsNullOrWhiteSpace(msg);
        }

        private static bool TryCreateRefNoteIdDisplayValue<Field>(PXCache cache, Guid? refNoteId, out string msg) 
            where Field : IBqlField
        {
            msg = null;
            if (refNoteId == null)
            {
                return false;
            }

            var helper = new EntityHelper(cache.Graph);
            var entity = helper.GetEntityRow(refNoteId);
            if (entity == null)
            {
                return false;
            }

            var keys = helper.GetEntityRowKeys(entity.GetType(), entity);
            if (keys == null || keys.Length == 0)
            {
                return false;
            }

            var refNoteDisplay = string.Join(";", keys);
            if (!string.IsNullOrWhiteSpace(refNoteDisplay))
            {
                msg = $"{PXUIFieldAttribute.GetDisplayName<Field>(cache)} = {Common.Cache.GetCacheName(entity.GetType())} {refNoteDisplay}";
            }

            return !string.IsNullOrWhiteSpace(msg);
        }

        private string CreateDetailFpExceptionMessage(AMRPDetailFPSupply detailFp, Exception exception, string additionalMessage)
        {
            if (detailFp?.InventoryID == null)
            {
                return null;
            }

            var sb = new System.Text.StringBuilder();
            if (!string.IsNullOrWhiteSpace(additionalMessage))
            {
                sb.Append($"{additionalMessage}. ");
            }

            sb.Append($"{PXUIFieldAttribute.GetDisplayName<AMRPDetailFPSupply.type>(MrpDetailFirstPassRecs.Cache)} = {MRPPlanningType.GetDescription(detailFp.Type)}. ");

            var inventoryCd = ((InventoryItem)PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, detailFp.InventoryID))?.InventoryCD;
            if (!string.IsNullOrWhiteSpace(inventoryCd))
            {
                var displayName = PXUIFieldAttribute.GetDisplayName<AMRPDetailFPSupply.inventoryID>(MrpDetailFirstPassRecs.Cache);
                sb.Append($"{displayName} = {inventoryCd}. ");
            }

            if (InventoryHelper.MultiWarehousesFeatureEnabled)
            {
                var siteCd = ((INSite)PXSelect<INSite, Where<INSite.siteID, Equal<Required<INSite.siteID>>>>.Select(this, detailFp.SiteID))?.SiteCD;
                if (!string.IsNullOrWhiteSpace(siteCd))
                {
                    var displayName = PXUIFieldAttribute.GetDisplayName<AMRPDetailFPSupply.siteID>(MrpDetailFirstPassRecs.Cache);
                    sb.Append($"{displayName} = {siteCd}. ");
                }
            }

            sb.Append($"{PXUIFieldAttribute.GetDisplayName<AMRPDetailFPSupply.recordID>(MrpDetailFirstPassRecs.Cache)} = {detailFp.RecordID}. ");
            sb.Append($"{PXUIFieldAttribute.GetDisplayName<AMRPDetailFPSupply.planDate>(MrpDetailFirstPassRecs.Cache)} = {detailFp.PlanDate?.ToShortDateString()}. ");
            sb.Append($"{PXUIFieldAttribute.GetDisplayName<AMRPDetailFPSupply.qty>(MrpDetailFirstPassRecs.Cache)} = {detailFp.Qty.GetValueOrDefault()}. ");

            if (detailFp.RefNoteID != null)
            {
                var refNoteDisplay = MrpDetailFirstPassRecs.Cache.GetStateExt<AMRPDetailFPSupply.refNoteID>(detailFp) as string;
                if (!string.IsNullOrWhiteSpace(refNoteDisplay))
                {
                    sb.Append($"{PXUIFieldAttribute.GetDisplayName<AMRPDetailFPSupply.refNoteID>(MrpDetailFirstPassRecs.Cache)} = {refNoteDisplay}. ");
                }
            }

            if (exception?.Message != null)
            {
                sb.Append(exception.Message);
            }

            return sb.ToString();
        }

		protected virtual string GetNextPlannedTransferNbr()
		{
			var nextNbr = (MrpRunTime.Current.PlanTransferLastOrderNbr ?? 0) + 1;
			MrpRunTime.Current.PlanTransferLastOrderNbr = nextNbr;
			return FormatReferenceNumber(MrpRunTime.Current.PlanTransferOrderNbrConstant, nextNbr, MrpRunTime.Current.AutoNumberSize ?? 5);
		}

		protected virtual string GetNextPlannedKitAssemblyNbr()
		{
			var nextNbr = (MrpRunTime.Current.PlanAssemblyLastOrderNbr ?? 0) + 1;
			MrpRunTime.Current.PlanAssemblyLastOrderNbr = nextNbr;
			return FormatReferenceNumber(MrpRunTime.Current.PlanAssemblyLastOrderNbrConstant, nextNbr, MrpRunTime.Current.AutoNumberSize ?? 5);
		}

		protected virtual string GetNextConsolidatedNbr()
		{
			var nextNbr = (MrpRunTime.Current.ConsolidatedLastOrderNbr ?? 0) + 1;
			MrpRunTime.Current.ConsolidatedLastOrderNbr = nextNbr;
			return FormatReferenceNumber(MrpRunTime.Current.ConsolidatedOrderNbrConstant, nextNbr, MrpRunTime.Current.AutoNumberSize ?? 5);
		}

		protected virtual string FormatReferenceNumber(string constant, int number, int padWidth)
		{
			return $"{constant}{number.ToString().PadLeft(padWidth, '0')}";
		}

		protected virtual AMRPDetailFP CreatePlannedTransferDemand(AMRPDetail plannedTransferSupply, MRPProcessCache.ItemCache itemCache)
		{
			var branchID = INSite.PK.Find(this, plannedTransferSupply.TransferSiteID)?.BranchID;
			return new AMRPDetailFP
			{
				SDFlag = MRPSDFlag.Demand,
				Type = MRPPlanningType.PlannedTransferDemand,
				LowLevel = itemCache.LowLevel,
				InventoryID = plannedTransferSupply.InventoryID,
				SubItemID = plannedTransferSupply.SubItemID,
				SiteID = plannedTransferSupply.TransferSiteID,
				TransferSiteID = plannedTransferSupply.SiteID,
				RefType = plannedTransferSupply.Type,
				ParentInventoryID = plannedTransferSupply.ParentInventoryID,
				ParentSubItemID = plannedTransferSupply.ParentSubItemID,
				ParentRefNbr = plannedTransferSupply.RefNbr,
				ParentRefNoteID = plannedTransferSupply.RefNoteID,
				ProductInventoryID = plannedTransferSupply.ProductInventoryID,
				ProductSubItemID = plannedTransferSupply.ProductSubItemID,
				ProductRefNbr = plannedTransferSupply.ProductRefNbr ?? plannedTransferSupply.ParentRefNbr ?? plannedTransferSupply.RefNbr,
				ProductRefNoteID = plannedTransferSupply.ProductRefNoteID ?? plannedTransferSupply.ParentRefNoteID ?? plannedTransferSupply.RefNoteID,
				PlanDate = plannedTransferSupply.ActionDate,
				SuppliedQty = 0,
				Qty = plannedTransferSupply.BaseQty,
				OriginalQty = plannedTransferSupply.BaseQty,
				RefNbr = GetNextPlannedTransferNbr(),
				ItemClassID = itemCache.IemClassID,
				BranchID = branchID
			};
		}

		[Obsolete("This method has been deprecated and will be removed in Acumatica ERP 2024R2.")]
		protected virtual void AdjustFirstPassLateDemandQty(int lowLevel, List<INSite> sites, MRPProcessCache mrpProcessCache)
		{
			if (sites == null || sites.Count == 0)
			{
				return;
			}

			var hasMoreLateDemand = true;
			// possible with bad item warehouse details setup
			var safetyNet = sites.Count + 1;
			var loopCounter = 1;
			while (hasMoreLateDemand)
			{
				hasMoreLateDemand = false;
				foreach (var site in sites)
				{
					var lateDemand = GetMrpDetailFPDemandByWarehouse(lowLevel, site.SiteID);
					if (lateDemand?.Any() != true)
					{
						continue;
					}
					hasMoreLateDemand = true;
					AdjustFirstPassDemandQtyByWarehouse(lowLevel, site, GetFirstPassSupplyByWarehouse(lowLevel, site), lateDemand);
					if (Setup.Current.UseDaysSupplytoConsolidateOrders == true)
					{
						MRPConsolidationByLowLevelBySite(sites, lowLevel, mrpProcessCache);
					}
				}

				if (hasMoreLateDemand)
				{
					Blowdown(mrpProcessCache, lowLevel);
				}

				loopCounter++;
				if (loopCounter >= safetyNet)
				{
					hasMoreLateDemand = false;
				}
			}
		}

		[Obsolete("This method has been deprecated and will be removed in Acumatica ERP 2024R2. Use GetFirstPassSupply(int).")]
		protected virtual FirstPassSupplyCollection GetFirstPassSupplyByWarehouse(int lowLevel, INSite site)
		{
			var supplyCollection = new FirstPassSupplyCollection(SubItemEnabled);
			supplyCollection.MergeSupply(GetMrpDetailFPSupplyByWarehouse(lowLevel, site?.SiteID));
			return supplyCollection;
		}

		protected virtual FirstPassSupplyCollection GetFirstPassSupply(int lowLevel)
		{
			var supplyCollection = new FirstPassSupplyCollection(SubItemEnabled);
			supplyCollection.MergeSupply(GetMrpDetailFPSupply(lowLevel));
			return supplyCollection;
		}

		protected virtual void CreateNoTRReplenishmentWarehouseMRPException(Dictionary<int, int> warehouseBranches)
		{
			var branchID = 0;
			var date = ProcessDateTime.Date;
			foreach (PXResult<AMRPItemSite, InventoryItem> result in SelectFrom<AMRPItemSite>
				.InnerJoin<InventoryItem>.On<AMRPItemSite.inventoryID.IsEqual<InventoryItem.inventoryID>>
				.Where<AMRPItemSite.replenishmentSource.IsEqual<INReplenishmentSource.transfer>
				.And<AMRPItemSite.replenishmentSiteID.IsNull>>.View.Select(this))
			{
				var mrpItemSite = (AMRPItemSite)result;
				var item = (InventoryItem)result;
				warehouseBranches.TryGetValue(mrpItemSite.SiteID.GetValueOrDefault(), out branchID);
				InsertMrpException(new AMRPExceptions
				{
					InventoryID = mrpItemSite.InventoryID,
					SubItemID = mrpItemSite.SubItemID,
					PromiseDate = date,
					RequiredDate = date,
					Type = MRPExceptionType.NoTRReplenishmentWarehouse,
					SiteID = mrpItemSite.SiteID,
					ItemClassID = item.ItemClassID,
					BranchID = branchID
				});
			}
		}

		public bool TryGetPlanningHorizonDate(out DateTime planningHorizonDate)
		{
			var horizon = Setup.Current?.AMPlanningHorizon ?? 0;
			planningHorizonDate = Common.Current.BusinessDate(this).AddDays(horizon);
			return horizon > 0;
		}

		public virtual DateTime GetForecastHorizonDate()
		{
			var planningStartHorizonDays = 0;
			if (Setup.Current != null)
			{
				planningStartHorizonDays = Setup.Current?.ForecastPlanHorizon ?? 0;
			}

			return Common.Current.BusinessDate(this).AddDays(planningStartHorizonDays);
		}

		public virtual DateTime GetMPSTimeFenceDate()
		{
			var mpsTimeFence = 0;
			var setup = this.Setup?.Current;
			if (setup != null)
			{
				mpsTimeFence = setup.MPSFence.GetValueOrDefault();
			}

			return Common.Current.BusinessDate(this).AddDays(mpsTimeFence);
		}

		public List<AMProdItemBaseQtyByEndDate> MPSCachedOrders;

		public virtual decimal GetDependentMPSQuantity(AMMPS mps, DateTime startDate)
		{
			if (MPSCachedOrders == null)
			{
				LoadMPSCachedOrders();
			}

			var endDate = mps.PlanDate ?? Common.Dates.BeginOfTimeDate;
			var results = MPSCachedOrders
					?.Where(r => r.InventoryID == mps.InventoryID && r.SiteID == mps.SiteID && r.EndDate > startDate && r.EndDate <= endDate);

			if (InventoryHelper.SubItemFeatureEnabled)
			{
				results = results.Where(r => r.SubItemID == mps.SubItemID);
			}

			var qty = results?.Sum(c => c.BaseQtytoProd.GetValueOrDefault()) ?? 0;
			return qty.NotLessZero();
		}

		protected virtual void LoadMPSCachedOrders()
		{
			var mpsCachedOrdersCmd = GetMPSCachedOrdersBaseSelect();
			if (Setup.Current != null && !Setup.Current.IncludeOnHoldProductionOrder.GetValueOrDefault())
			{
				mpsCachedOrdersCmd.WhereAnd<Where<AMProdItemBaseQtyByEndDate.hold, Equal<False>>>();
			}

			var cmdParams = new List<object>();
			mpsCachedOrdersCmd.WhereAnd<Where<AMProdItemBaseQtyByEndDate.endDate, GreaterEqual<Required<AMProdItemBaseQtyByEndDate.endDate>>>>();
			cmdParams.Add(GetMPSTimeFenceDate());

			if (TryGetPlanningHorizonDate(out var endDate))
			{
				mpsCachedOrdersCmd.WhereAnd<Where<AMProdItemBaseQtyByEndDate.endDate, LessEqual<Required<AMProdItemBaseQtyByEndDate.endDate>>>>();
				cmdParams.Add(endDate);
			}

			MPSCachedOrders = mpsCachedOrdersCmd.Select(cmdParams.ToArray()).ToFirstTableList();
		}

		protected virtual PXSelectBase<AMProdItemBaseQtyByEndDate> GetMPSCachedOrdersBaseSelect()
		{
			if (InventoryHelper.SubItemFeatureEnabled)
			{
				return new PXSelectGroupBy<AMProdItemBaseQtyByEndDate,
						Aggregate<
							GroupBy<AMProdItemBaseQtyByEndDate.inventoryID,
							GroupBy<AMProdItemBaseQtyByEndDate.subItemID,
							GroupBy<AMProdItemBaseQtyByEndDate.siteID,
							GroupBy<AMProdItemBaseQtyByEndDate.endDate,
							Sum<AMProdItemBaseQtyByEndDate.baseQtytoProd>>>>>>>(this);
			}

			return new PXSelectGroupBy<AMProdItemBaseQtyByEndDate,
						Aggregate<
							GroupBy<AMProdItemBaseQtyByEndDate.inventoryID,
							GroupBy<AMProdItemBaseQtyByEndDate.siteID,
							GroupBy<AMProdItemBaseQtyByEndDate.endDate,
							Sum<AMProdItemBaseQtyByEndDate.baseQtytoProd>>>>>>(this);
		}

		private bool? _hasActiveBlanketSalesOrder;
		public virtual bool HasActiveBlanketSalesOrder()
		{
			if (_hasActiveBlanketSalesOrder == null)
			{
				var ot = (SOOrderType)SelectFrom<SOOrderType>.Where<SOOrderType.behavior.IsEqual<SOBehavior.bL>
				.And<SOOrderType.active.IsEqual<True>>>
				.View.SelectWindowed(this, 0, 1);
				_hasActiveBlanketSalesOrder = ot?.OrderType != null;
			}
			return _hasActiveBlanketSalesOrder == true;
		}

		public List<SimpleSalesLine> ForecastCachedOrders;
		public virtual decimal GetDependentForecastQuantity(AMForecast forecast, DateTime beginDate, DateTime endDate)
		{
			if (ForecastCachedOrders == null)
			{
				LoadForecastCachedOrders();
			}

			var results = ForecastCachedOrders
					?.Where(r => r.InventoryID == forecast.InventoryID && r.SiteID == forecast.SiteID && r.ShipDate.BetweenInclusive(beginDate, endDate));

			if (InventoryHelper.SubItemFeatureEnabled && forecast.SubItemID != null)
			{
				results = results.Where(r => r.SubItemID == forecast.SubItemID);
			}

			if (forecast.CustomerID != null)
			{
				results = results.Where(r => r.CustomerID == forecast.CustomerID);
			}

			var qty = results?.Sum(c => c.BaseOrderQty.GetValueOrDefault()) ?? 0;
			return qty.NotLessZero();
		}

		protected virtual void LoadForecastCachedOrders()
		{
			var forcastHorizonDate = GetForecastHorizonDate();
			var cmd = GetForecastCachedOrdersBaseSelect();
			if (Setup.Current != null && Setup?.Current?.IncludeOnHoldSalesOrder != true)
			{
				cmd.WhereAnd<Where<SimpleSalesLine.hold, Equal<False>>>();
			}

			var dateParameters = new List<object>();
			cmd.WhereAnd<Where<SimpleSalesLine.shipDate, GreaterEqual<Required<SimpleSalesLine.shipDate>>>>();
			dateParameters.Add(forcastHorizonDate);

			var usingHorizon = TryGetPlanningHorizonDate(out var endDate);
			if (usingHorizon)
			{
				cmd.WhereAnd<Where<SimpleSalesLine.shipDate, LessEqual<Required<SimpleSalesLine.shipDate>>>>();
				dateParameters.Add(endDate);
			}

			ForecastCachedOrders = cmd.Select(dateParameters.ToArray()).ToFirstTableList();

			if (!HasActiveBlanketSalesOrder())
			{
				return;
			}

			var cmdBlankets = GetForecastCachedBlanketOrdersBaseSelect();
			if (Setup?.Current?.IncludeOnHoldSalesOrder != true)
			{
				cmdBlankets.WhereAnd<Where<SimpleBlanketLineSplit.hold, Equal<False>>>();
			}
			if (Setup?.Current?.IncludeExpiredBlanketSalesOrders != true)
			{
				cmdBlankets.WhereAnd<Where<SimpleBlanketLineSplit.status, NotEqual<SOOrderStatus.expired>>>();
			}

			cmdBlankets.WhereAnd<Where<SimpleBlanketLineSplit.schedShipDate, GreaterEqual<Required<SimpleBlanketLineSplit.schedShipDate>>>>();
			if (usingHorizon)
			{
				cmdBlankets.WhereAnd<Where<SimpleBlanketLineSplit.schedShipDate, LessEqual<Required<SimpleBlanketLineSplit.schedShipDate>>>>();
			}

			foreach (SimpleBlanketLineSplit blanketLine in cmdBlankets.Select(dateParameters.ToArray()))
			{
				ForecastCachedOrders.Add(Convert(blanketLine));
			}
		}

		protected virtual SimpleSalesLine Convert(SimpleBlanketLineSplit blanketLine)
			=> new SimpleSalesLine
				{
					InventoryID = blanketLine?.InventoryID,
					SiteID = blanketLine?.SiteID,
					SubItemID = blanketLine?.SubItemID,
					CustomerID = blanketLine?.CustomerID,
					ShipDate = blanketLine?.SchedShipDate,
					BaseOrderQty = blanketLine?.OpenOrderQty ?? 0m,
					Hold = blanketLine?.Hold,
				};

		protected virtual PXSelectBase<SimpleSalesLine> GetForecastCachedOrdersBaseSelect()
		{
			if (InventoryHelper.SubItemFeatureEnabled)
			{
				return new PXSelectGroupBy<SimpleSalesLine,
						Aggregate<
							GroupBy<SimpleSalesLine.inventoryID,
							GroupBy<SimpleSalesLine.subItemID,
							GroupBy<SimpleSalesLine.siteID,
							GroupBy<SimpleSalesLine.customerID,
							GroupBy<SimpleSalesLine.shipDate,
							Sum<SimpleSalesLine.baseOrderQty>>>>>>>>(this);
			}

			return new PXSelectGroupBy<SimpleSalesLine,
						Aggregate<
							GroupBy<SimpleSalesLine.inventoryID,
							GroupBy<SimpleSalesLine.siteID,
							GroupBy<SimpleSalesLine.customerID,
							GroupBy<SimpleSalesLine.shipDate,
							Sum<SimpleSalesLine.baseOrderQty>>>>>>>(this);
		}

		protected virtual PXSelectBase<SimpleBlanketLineSplit> GetForecastCachedBlanketOrdersBaseSelect()
		{
			if (InventoryHelper.SubItemFeatureEnabled)
			{
				return new PXSelectGroupBy<SimpleBlanketLineSplit,
						Aggregate<
							GroupBy<SimpleBlanketLineSplit.inventoryID,
							GroupBy<SimpleBlanketLineSplit.subItemID,
							GroupBy<SimpleBlanketLineSplit.siteID,
							GroupBy<SimpleBlanketLineSplit.customerID,
							GroupBy<SimpleBlanketLineSplit.schedShipDate,
							Sum<SimpleBlanketLineSplit.openOrderQty
							//Sum<SimpleBlanketLineSplit.shippedQty,
							//Sum<SimpleBlanketLineSplit.qtyOnOrders>>
							>>>>>>>>(this);
			}

			return new PXSelectGroupBy<SimpleBlanketLineSplit,
						Aggregate<
							GroupBy<SimpleBlanketLineSplit.inventoryID,
							GroupBy<SimpleBlanketLineSplit.siteID,
							GroupBy<SimpleBlanketLineSplit.customerID,
							GroupBy<SimpleBlanketLineSplit.schedShipDate,
							Sum<SimpleBlanketLineSplit.openOrderQty
							//Sum<SimpleBlanketLineSplit.shippedQty,
							//Sum<SimpleBlanketLineSplit.qtyOnOrders>>
							>>>>>>>(this);
		}

		/// <summary>
		/// Used to control the line counters of mrp records that have no direct parent and where the line counters reset for each run
		/// </summary>
		[Serializable]
        [PXHidden]
        public class AMRPRunTime : PXBqlTable, IBqlTable
        {
            #region RunDateTime
            public abstract class runDateTime : PX.Data.BQL.BqlDateTime.Field<runDateTime> { }

            [PXDate]
            public virtual DateTime? RunDateTime { get; set; }
            #endregion
            #region FirstPassRecordID
            public abstract class firstPassRecordID : PX.Data.BQL.BqlInt.Field<firstPassRecordID> { }

            [PXInt]
            [PXUnboundDefault(0)]
            public virtual int? FirstPassRecordID { get; set; }
            #endregion
            #region DetailRecordID
            public abstract class detailRecordID : PX.Data.BQL.BqlInt.Field<detailRecordID> { }

            [PXInt]
            [PXUnboundDefault(0)]
            public virtual int? DetailRecordID { get; set; }
            #endregion
            #region PlanRecordID
            public abstract class planRecordID : PX.Data.BQL.BqlInt.Field<planRecordID> { }

            [PXInt]
            [PXUnboundDefault(0)]
            public virtual int? PlanRecordID { get; set; }
            #endregion
            #region ExceptionRecordID
            public abstract class exceptionRecordID : PX.Data.BQL.BqlInt.Field<exceptionRecordID> { }

            [PXInt]
            [PXUnboundDefault(0)]
            public virtual int? ExceptionRecordID { get; set; }
            #endregion
            #region CallingGraphUid

            public abstract class callingGraphUid : PX.Data.BQL.BqlGuid.Field<callingGraphUid> { }

            public virtual Guid? CallingGraphUid { get; set; }

			#endregion
			#region PlanTransferOrderNbrConstant
			public abstract class planTransferOrderNbrConstant : PX.Data.BQL.BqlString.Field<planTransferOrderNbrConstant> { }

			[PXString]
			[PXUnboundDefault("PLTR")]
			public virtual string PlanTransferOrderNbrConstant { get; set; }
			#endregion
			#region ConsolidatedOrderNbrConstant
			public abstract class consolidatedOrderNbrConstant : PX.Data.BQL.BqlString.Field<consolidatedOrderNbrConstant> { }

			[PXString]
			[PXUnboundDefault("CONS")]
			public virtual string ConsolidatedOrderNbrConstant { get; set; }
			#endregion
			#region PlanTransferLastOrderNbr
			public abstract class planTransferLastOrderNbr : PX.Data.BQL.BqlInt.Field<planTransferLastOrderNbr> { }

			[PXInt]
			[PXUnboundDefault(0)]
			public virtual int? PlanTransferLastOrderNbr { get; set; }
			#endregion
			#region ConsolidatedLastOrderNbr
			public abstract class consolidatedLastOrderNbr : PX.Data.BQL.BqlInt.Field<consolidatedLastOrderNbr> { }

			[PXInt]
			[PXUnboundDefault(0)]
			public virtual int? ConsolidatedLastOrderNbr { get; set; }
			#endregion
			#region AutoNumberSize
			public abstract class autoNumberSize : PX.Data.BQL.BqlInt.Field<autoNumberSize> { }

			[PXInt]
			[PXUnboundDefault(0)]
			public virtual int? AutoNumberSize { get; set; }
			#endregion
			#region PlanAssemblyLastOrderNbrConstant
			public abstract class planAssemblyLastOrderNbrConstant : PX.Data.BQL.BqlString.Field<planAssemblyLastOrderNbrConstant> { }

			[PXString]
			[PXUnboundDefault("PLSK")]
			public virtual string PlanAssemblyLastOrderNbrConstant { get; set; }
			#endregion
			#region PlanAssemblyLastOrderNbr
			public abstract class planAssemblyLastOrderNbr : PX.Data.BQL.BqlInt.Field<planAssemblyLastOrderNbr> { }

			[PXInt]
			[PXUnboundDefault(0)]
			public virtual int? PlanAssemblyLastOrderNbr { get; set; }
			#endregion
		}

		private static IEnumerable<string> GetCachedCountsAllMessage(PXGraph graph, int minCount = 1)
		{
			var cacheHash = new HashSet<Type>();
			foreach (var cache in graph.Caches)
			{
				if (cache.Value.Cached.Count() >= minCount && cacheHash.Add(cache.Value.GetItemType()))
				{
					yield return cache.Value.ToString();
				}
			}
		}

		[PXInternalUseOnly]
		public virtual void InsertDebugCachedCounts()
		{
			foreach (var cacheMsg in GetCachedCountsAllMessage(this, 5))
			{
				InsertDebugAuditRec(cacheMsg);
			}
		}

		[PXInternalUseOnly]
		public virtual void InsertDebugAuditRec(string message)
		{
			if (DebugMessages)
			{
				InsertAuditRec(message);
			}
		}

		[PXInternalUseOnly]
		public virtual void InsertDebugMemoryAuditRec()
		{
			InsertDebugAuditRec(string.Format("Memory: {0} MB", PX.SM.PXPerformanceMonitor.GetWorkingSet()));
		}
	}
}
