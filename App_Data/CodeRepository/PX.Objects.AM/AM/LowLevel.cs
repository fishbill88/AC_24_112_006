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
using PX.Objects.AM.Attributes;
using PX.Data;
using PX.Objects.AM.CacheExtensions;
using PX.Objects.CS;
using PX.Objects.IN;
using System.Linq;
using PX.Common;

namespace PX.Objects.AM
{
    public class LowLevel : PXGraph<LowLevel>
    {
        // 2021R2 swap primary with LowLevelInventoryItem
        public PXSelect<InventoryItem> InventoryItemRecs;
        public PXSetupOptional<AMBSetup> BomSetup;

        // MAIN CACHE USED FOR UPDATING LOWLEVEL VALUE
        [PXHidden]
        public PXSelect<LowLevelInventoryItem> LowLevelInventoryItemRecs;

        //Required as a workaround to AEF InventoryItemExt updates - Acumatica case 031594
        public PXSetup<INSetup> InvSetup;
        public PXSetup<CommonSetup> CSetup;

		#region CACHE ATTACHED

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
		/// Number of levels found
		/// </summary>
		public int CurrentMaxLowLevel;
        /// <summary>
        /// Was the process skipped (no boms changed from last run)
        /// </summary>
        public bool ProcessLevelsSkipped;
        public const int MaxLowLevel = 25;
        protected const int MaxNumberOfErrors = 50;
        private int _currentNumberOfErrors;

        /// <summary>
        /// Keeps track of all item low levels to call one DB update at the end of set all
        /// </summary>
        private Dictionary<int, int> _lowLevelDictionary;


        public LowLevel()
        {
            _lowLevelDictionary = new Dictionary<int, int>();
            CurrentMaxLowLevel = 0;
            ProcessLevelsSkipped = false;

            InventoryItemRecs.AllowDelete = false;
            InventoryItemRecs.AllowInsert = false;

            LowLevelInventoryItemRecs.AllowDelete = false;
            LowLevelInventoryItemRecs.AllowInsert = false;
        }

        public static LowLevel Construct()
        {
			var graph = CreateInstance<LowLevel>();
			graph.Clear(PXClearOption.ClearAll);
			return graph;
        }

        /// <summary>
        /// Determine if BOM data has changed. If not then no need to recalc low levels
        /// </summary>
        /// <param name="graph">calling graph</param>
        /// <param name="fromDateTime">Date and time to check from for bom changes</param>
        /// <returns></returns>
        public static bool BomDataChanged(PXGraph graph, DateTime? fromDateTime)
        {
            if (graph == null)
            {
                throw new ArgumentNullException(nameof(graph));
            }

            if (fromDateTime == null)
            {
                return true;
            }

            var bomItemAdded = (AMBomItem)PXSelect<AMBomItem,
                Where<AMBomItem.createdDateTime, GreaterEqual<Required<AMBomItem.createdDateTime>>>>
               .SelectWindowed(graph, 0, 1, fromDateTime);

            if (bomItemAdded?.BOMID != null)
            {
                return true;
            }
            // Joining AMBomItem as ECC reuses BOM Matl tables and changes to ECC should not impact low level logic
            var bomMatlAddedUpdated = (AMBomMatl)PXSelectJoin<
                    AMBomMatl,
                    InnerJoin<AMBomItem, On<AMBomMatl.bOMID, Equal<AMBomItem.bOMID>>>,
                    Where<AMBomMatl.lastModifiedDateTime, GreaterEqual<Required<AMBomMatl.lastModifiedDateTime>>>>
                .SelectWindowed(graph, 0, 1, fromDateTime);

			if (bomMatlAddedUpdated?.BOMID != null)
			{
				return true;
			}

			return KitDataChanged(graph, fromDateTime);

		}

		public static bool KitDataChanged(PXGraph graph, DateTime? fromDateTime)
		{
			if (Features.KitFeatureEnabled())
			{
				var stockKitItemChanged = (INKitSpecHdr)PXSelect<INKitSpecHdr,
					Where<INKitSpecHdr.lastModifiedDateTime, GreaterEqual<Required<INKitSpecHdr.lastModifiedDateTime>>>>
					.SelectWindowed(graph, 0, 1, fromDateTime);

				return stockKitItemChanged?.KitInventoryID != null;
			}

			return false;
		}

		#region Dictionary Methods

		protected void UpdateLowLevelDictionary(int? key, int? value)
        {
            if (key.GetValueOrDefault() == 0 || value.GetValueOrDefault() == 0)
            {
                return;
            }

            if (_lowLevelDictionary.ContainsKey(key.GetValueOrDefault()))
            {
                _lowLevelDictionary.Remove(key.GetValueOrDefault());
            }

            _lowLevelDictionary.Add(key.GetValueOrDefault(), value.GetValueOrDefault());
        }

        protected int GetLowLevelDictionaryValue(int? key)
        {
            return _lowLevelDictionary.TryGetValue(key.GetValueOrDefault(), out var lowLevelReturn) ? lowLevelReturn : 0;
        }
        #endregion

		protected virtual bool UpdateInventoryItem(int inventoryID, int lowLevel)
		{
			return PXDatabase.Update<InventoryItem>(
                new PXDataFieldAssign<InventoryItemExt.aMLowLevel>(PXDbType.Int, lowLevel),
				new PXDataFieldRestrict<InventoryItem.inventoryID>(PXDbType.Int, 4, inventoryID, PXComp.EQ));
		}

        protected virtual void CheckForMaxErrorsReached()
        {
            if (_currentNumberOfErrors >= MaxNumberOfErrors)
            {
                throw new PXException(Messages.LowLevelMaxErrorsReceived);
            }
        }

        protected virtual void PersistDictionary()
        {
			foreach(var kv in _lowLevelDictionary)
			{
				var inventoryID = kv.Key;
				var newLowLevel = kv.Value;

				if (newLowLevel >= MaxLowLevel)
                {
					var item = (BomInventoryItemSimple)this.Caches[typeof(BomInventoryItemSimple)].Locate(new BomInventoryItemSimple { InventoryID = inventoryID });
					if(item != null)
					{
						// to help in troubleshooting items related to circular reference
						PXTrace.WriteInformation(Messages.GetLocal(Messages.LowLevelMaxLevelReachedForItem, item.InventoryCD.TrimIfNotNullEmpty(), item.AMLowLevel.GetValueOrDefault()));
					}
                }

				UpdateInventoryItem(inventoryID, newLowLevel);
			}
        }

        /// <summary>
        /// Persist with a retry for each row vs first attempt at mass update.
        /// This exists due to various customer item table error that exist before this process runs preventing the update from occurring.
        /// </summary>
        protected virtual void PersistDictionaryWithRetry()
        {
            int retryCount = 1;
            for (int retry = 0; retry <= retryCount; retry++)
            {
                try
                {
                    PersistDictionary();
                    retry = retryCount;
                }
                catch
                {
                    if (retry >= retryCount)
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Sets Low Level for all Inventory Id's
        /// </summary>
        public virtual void SetAll()
        {
            var lastLowLevelDateTime = BomSetup?.Current?.LastLowLevelCompletedDateTime;
            if (lastLowLevelDateTime == null || BomDataChanged(this, lastLowLevelDateTime))
            {
                ProcessAllLevels();
                ProcessLevelsSkipped = false;
                return;
            }
            ProcessLevelsSkipped = true;
            CurrentMaxLowLevel = BomSetup?.Current?.LastMaxLowLevel ?? 0;
            PXTrace.WriteInformation($"No bom changes found from {lastLowLevelDateTime}. Low level process skipped");
        }

        protected virtual void ResetAllLowLevels()
        {
            PXDatabase.Update<InventoryItem>(
                new PXDataFieldAssign<InventoryItemExt.aMLowLevel>(PXDbType.Int, 0));
        }

		protected virtual List<ParentInventoryItemSimple> GetInventorySimple()
		{
			var bomList = PXSelect<ParentInventoryItemSimple>.Select(this).ToFirstTableList();
			if (Features.KitFeatureEnabled())
			{
				var uniqueKitItems = new HashSet<int>();
				foreach (PXResult<InventoryItem,INKitSpecHdr> kitInventory in PXSelectJoin<
														InventoryItem,
														InnerJoin<INKitSpecHdr,
															On<INKitSpecHdr.kitInventoryID, Equal<InventoryItem.inventoryID>>,
															InnerJoin<INKitSpecHdrLastRevisionDate, On<INKitSpecHdrLastRevisionDate.kitInventoryID, Equal<INKitSpecHdr.kitInventoryID>,
																And<INKitSpecHdrLastRevisionDate.createdDateTime, Equal<INKitSpecHdr.createdDateTime>>>>>,
														Where<INKitSpecHdr.isActive, Equal<True>>,
														OrderBy<Asc<InventoryItem.inventoryID, Desc<INKitSpecHdr.revisionID>>>>
														.Select(this))
				{
					var inventoryItem = (InventoryItem)kitInventory;
					var kitItem = (INKitSpecHdr)kitInventory;

					if (!uniqueKitItems.Add(kitItem?.KitInventoryID ?? 0))
					{
						continue;
					}

					var mrpPlan = new ParentInventoryItemSimple
					{
						InventoryID = inventoryItem.InventoryID,
						InventoryCD = inventoryItem.InventoryCD,
						AMLowLevel = 0,
						IsKit = true,
						KitRevision = kitItem.RevisionID
					};
					bomList.Add(mrpPlan);
				}
			}

			return bomList;
        }

        protected virtual void ProcessAllLevels()
        {
            _lowLevelDictionary = new Dictionary<int, int>();

            CurrentMaxLowLevel = 0;
            _currentNumberOfErrors = 0;

			// We could get rid of this reset if then on delete of either AMBomMatl or AMBomItem we reset the last low level data and made those related items set back to InventoryItem.AMLowLevel to zero.
            ResetAllLowLevels();

			var inventoryItemParents = GetInventorySimple();

			if (inventoryItemParents == null || inventoryItemParents.Count == 0)
            {
                return;
            }

			var matlItemQueryResults = new Dictionary<int, int[]>();

			// First loop pings the database for results where we will then store the results for each further loop to calculate each lower level
			foreach (var inventoryItemParent in inventoryItemParents)
            {
				this.Caches[typeof(ParentInventoryItemSimple)].Hold(inventoryItemParent);

                int currentlevel = GetLowLevelDictionaryValue(inventoryItemParent.InventoryID);
				var currentMaterial = new HashSet<int>();

				if (inventoryItemParent.IsKit == false)
				{
					// uses index: AMBomItem_InventoryID_Status
					foreach (LowLevelBomMatlBomItem row in PXSelectReadonly<LowLevelBomMatlBomItem,
						Where<LowLevelBomMatlBomItem.itemInventoryID, Equal<Required<LowLevelBomMatlBomItem.itemInventoryID>>>>
						.Select(this, inventoryItemParent.InventoryID))
					{
						currentMaterial.Add(row.MatlInventoryID.GetValueOrDefault());
						ProcessMaterialLowLevel(row.MatlInventoryID, currentlevel);
					}
				}
				else
				{
					foreach (INKitSpecStkDet row in PXSelectReadonly<INKitSpecStkDet,
						Where<INKitSpecStkDet.kitInventoryID, Equal<Required<INKitSpecStkDet.kitInventoryID>>,
							And<INKitSpecStkDet.revisionID, Equal<Required<INKitSpecStkDet.revisionID>>>>>
						.Select(this, inventoryItemParent.InventoryID, inventoryItemParent.KitRevision))
					{
						currentMaterial.Add(row.CompInventoryID.GetValueOrDefault());
						ProcessMaterialLowLevel(row.CompInventoryID, currentlevel);
					}
				}

				if (currentMaterial.Count > 0)
				{
					var key = inventoryItemParent.InventoryID.GetValueOrDefault();
					if (matlItemQueryResults.TryGetValue(key, out var existingMaterial))
					{
						if (existingMaterial != null)
						{
							currentMaterial.AddRange(existingMaterial);
						}
						matlItemQueryResults[key] = currentMaterial.ToArray();
						continue;
					}

					matlItemQueryResults.Add(key, currentMaterial.ToArray());
				}
			}

			var lowLevel = 1;
			var hasMoreLevels = true;
            while (hasMoreLevels)
            {
                foreach (var inventoryItemParent in inventoryItemParents)
                {
                    int currentlevel = GetLowLevelDictionaryValue(inventoryItemParent.InventoryID);
                    if (currentlevel >= MaxLowLevel)
                    {
                        continue;
                    }

					matlItemQueryResults.TryGetValue(inventoryItemParent.InventoryID.GetValueOrDefault(), out var bomMatlInventoryIDs);
					if (bomMatlInventoryIDs == null)
					{
						continue;
					}

					foreach (var matlInventoryID in bomMatlInventoryIDs)
                    {
						ProcessMaterialLowLevel(matlInventoryID, currentlevel);
                    }
                }

                lowLevel++;
                if (lowLevel > CurrentMaxLowLevel || lowLevel >= MaxLowLevel)
                {
                    //Either no more levels to process or the max has been reached
                    hasMoreLevels = false;
                }
            }

            PersistDictionaryWithRetry();

            if (CurrentMaxLowLevel >= MaxLowLevel)
            {
                PXTrace.WriteError(Messages.GetLocal(Messages.LowLevelMaxLevelReached, MaxLowLevel));
            }

            UpdateBomSetup();

            Clear();
        }

		private void ProcessMaterialLowLevel(int? matlInventoryID, int currentlevel)
		{
			var childLowLevel = GetLowLevelDictionaryValue(matlInventoryID);
			if (childLowLevel <= currentlevel)
			{
				childLowLevel = currentlevel + 1;

				if (childLowLevel > CurrentMaxLowLevel)
				{
					CurrentMaxLowLevel = childLowLevel;
				}

				UpdateLowLevelDictionary(matlInventoryID, childLowLevel);
			}
		}

		protected virtual void UpdateBomSetup()
        {
			if (!Features.MRPEnabled()) return;

            if (BomSetup?.Current == null)
            {
                BomSetup?.Select();
            }

            var setup = BomSetup?.Current;
            if (setup == null)
            {
                return;
            }

            setup.LastLowLevelCompletedDateTime = Common.Dates.Now;
            setup.LastMaxLowLevel = CurrentMaxLowLevel;

            BomSetup.Cache.PersistUpdated(setup);
        }

		[PXProjection(typeof(Select2<AMBomMatl,
			InnerJoin<AMBomItem, On<AMBomMatl.bOMID, Equal<AMBomItem.bOMID>>>,
			Where<AMBomItem.status, NotEqual<AMBomStatus.archived>>>), Persistent = false)]
        [Serializable]
        [PXHidden]
        public class LowLevelBomMatlBomItem : PXBqlTable, IBqlTable
        {
            #region BOMID
            public abstract class bOMID : PX.Data.BQL.BqlString.Field<bOMID> { }
            [BomID(BqlField = typeof(AMBomMatl.bOMID))]
            public virtual String BOMID { get; set; }
            #endregion
            #region MatlInventoryID
            public abstract class matlInventoryID : PX.Data.BQL.BqlInt.Field<matlInventoryID> { }
            [PXDBInt(BqlField = typeof(AMBomMatl.inventoryID))]
            [PXUIField(DisplayName = "Inventory ID")]
            public virtual Int32? MatlInventoryID { get; set; }
            #endregion

			#region ItemInventoryID
            public abstract class itemInventoryID : PX.Data.BQL.BqlInt.Field<itemInventoryID> { }
            [PXDBInt(BqlField = typeof(AMBomItem.inventoryID))]
            [PXUIField(DisplayName = "Inventory ID")]
            public virtual Int32? ItemInventoryID { get; set; }
            #endregion
        }

        /// <summary>
        /// PXProjection for <see cref="InventoryItem"/> only including the low level field to update
        /// </summary>
        [PXProjection(typeof(Select<InventoryItem>), Persistent = true)]
        [Serializable]
        [PXHidden]
        public class LowLevelInventoryItem : PXBqlTable, IBqlTable
        {
            #region InventoryID
            public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }

            [PXDBInt(BqlField = typeof(InventoryItem.inventoryID), IsKey = true)]
            [PXUIField(DisplayName = "Inventory ID", Enabled = false)]
            public virtual Int32? InventoryID { get; set; }

            #endregion
            #region InventoryCD
            public abstract class inventoryCD : PX.Data.BQL.BqlString.Field<inventoryCD> { }

            [PXDBString(InputMask = "", IsUnicode = true, BqlField = typeof(InventoryItem.inventoryCD))]
            [PXUIField(DisplayName = "Inventory CD", Enabled = false)]
            public virtual String InventoryCD { get; set; }
            #endregion

            #region AMLowLevel
            public abstract class aMLowLevel : PX.Data.BQL.BqlInt.Field<aMLowLevel> { }

            [PXDBInt(BqlField = typeof(InventoryItemExt.aMLowLevel))]
            [PXUIField(DisplayName = "Low Level")]
            public Int32? AMLowLevel { get; set; }
            #endregion
        }

		/// <summary>
		/// Group by Work kitInventoryID and get INKitSpecHdr records with max createdDateTime
		/// </summary>
		[Serializable]
		[PXHidden]
		[PXProjection(typeof(Select4<
			INKitSpecHdr,
			Where<INKitSpecHdr.isActive,Equal<True>>,
			Aggregate <
				GroupBy<INKitSpecHdr.kitInventoryID,				
					Max<INKitSpecHdr.createdDateTime>>>>), Persistent = false)]
		public class INKitSpecHdrLastRevisionDate : PXBqlTable, IBqlTable
		{
			#region KitInventoryID
			public abstract class kitInventoryID : PX.Data.BQL.BqlInt.Field<kitInventoryID> { }
			[PXDBInt(IsKey = true, BqlField = typeof(INKitSpecHdr.kitInventoryID))]
			[PXUIField(DisplayName = "Inventory ID")]
			public virtual Int32? KitInventoryID { get; set; }
			#endregion

			#region CreatedDateTime(Max)
			public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
			[PXDBDateAndTime(IsKey = true, BqlField = typeof(INKitSpecHdr.createdDateTime))]
			public virtual DateTime? CreatedDateTime { get; set; }
			#endregion
		}
	}
}
