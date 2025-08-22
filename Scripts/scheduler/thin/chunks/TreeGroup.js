/*!
 *
 * Bryntum Scheduler Pro 5.3.7
 *
 * Copyright(c) 2023 Bryntum AB
 * https://bryntum.com/contact
 * https://bryntum.com/license
 *
 */
import { GridFeatureManager } from './GridBase.js';
import { DragHelper } from './MessageDialog.js';
import { Delayable, InstancePlugin, Rectangle, DomHelper, ObjectHelper } from './Editor.js';
import { TreeColumn } from './Tree.js';

/**
 * @module Grid/feature/RowReorder
 */
/**
 * Object with information about a tree position
 * @typedef {Object} RecordPositionContext
 * @property {Core.data.Model} record Tree node
 * @property {Number} parentIndex Index among parents children
 * @property {String|Number} parentId Parent node's id
 */
/**
 * Allows user to reorder rows by dragging them. To get notified about row reorder listen to `change` event
 * on the grid {@link Core.data.Store store}.
 *
 * This feature is **off** by default. For info on enabling it, see {@link Grid.view.mixin.GridFeatures}.
 * This feature is **enabled** by default for Gantt.
 *
 * {@inlineexample Grid/feature/RowReorder.js}
 *
 * If the grid is set to {@link Grid.view.Grid#config-readOnly}, reordering is disabled. Inside all event listeners you
 * have access a `context` object which has a `record` property (the dragged record).
 *
 * ## Validation
 * You can validate the drag drop flow by listening to the `gridrowdrag` event. Inside this listener you have access to
 * the `index` property which is the target drop position. For trees you get access to the `parent` record and `index`,
 * where index means the child index inside the parent.
 *
 * You can also have an async finalization step using the {@link #event-gridRowBeforeDropFinalize}, for showing a
 * confirmation dialog or making a network request to decide if drag operation is valid (see code snippet below)
 *
 * ```javascript
 * features : {
 *     rowReorder : {
 *         showGrip : true
 *     },
 *     listeners : {
 *        gridRowDrag : ({ context }) => {
 *           // Here you have access to context.insertBefore, and additionally context.parent for trees
 *        },
 *
 *        gridRowBeforeDropFinalize : async ({ context }) => {
 *           const result = await MessageDialog.confirm({
 *               title   : 'Please confirm',
 *               message : 'Did you want the row here?'
 *           });
 *
 *           // true to accept the drop or false to reject
 *           return result === MessageDialog.yesButton;
 *        }
 *    }
 * }
 * ```
 *
 * Note, that this feature uses the concept of "insert before" when choosing a drop point in the data. So the dropped
 * record's position is *before the visual next record's position*.
 *
 * This may look like a pointless distinction, but consider the case when a Store is filtered. The record *above* the
 * drop point may have several filtered out records below it. When unfiltered, the dropped record will be *below* these
 * because of the "insert before" behaviour.
 *
 * ## Behavior with multiple subgrids
 *
 * For grids with multiple subgrids, row reordering is only enabled for the first subgrid.
 *
 * NOTE: This feature cannot be used simultaneously with the `enableTextSelection` config.
 *
 * @extends Core/mixin/InstancePlugin
 * @demo Grid/rowreordering
 * @classtype rowReorder
 * @feature
 */
class RowReorder extends Delayable(InstancePlugin) {
  //region Events
  /**
   * Fired before dragging starts, return false to prevent the drag operation.
   * @preventable
   * @event gridRowBeforeDragStart
   * @param {Core.helper.DragHelper} source
   * @param {Object} context
   * @param {Core.data.Model[]} context.records The dragged row records
   * @param {MouseEvent|TouchEvent} event
   * @on-owner
   */
  /**
   * Fired when dragging starts.
   * @event gridRowDragStart
   * @param {Core.helper.DragHelper} source
   * @param {Object} context
   * @param {Core.data.Model[]} context.records The dragged row records
   * @param {MouseEvent|TouchEvent} event
   * @on-owner
   */
  /**
   * Fired while the row is being dragged, in the listener function you have access to `context.insertBefore` a grid /
   * tree record, and additionally `context.parent` (a TreeNode) for trees. You can signal that the drop position is
   * valid or invalid by setting `context.valid = false;`
   * @event gridRowDrag
   * @param {Core.helper.DragHelper} source
   * @param {Object} context
   * @param {Boolean} context.valid Set this to true or false to indicate whether the drop position is valid.
   * @param {Core.data.Model} context.insertBefore The record to insert before (`null` if inserting at last position of a parent node)
   * @param {Core.data.Model} context.parent The parent record of the current drop position (only applicable for trees)
   * @param {Core.data.Model[]} context.records The dragged row records
   * @param {MouseEvent} event
   * @on-owner
   */
  /**
   * Fired before the row drop operation is finalized. You can return false to abort the drop operation, or a
   * Promise yielding `true` / `false` which allows for asynchronous abort (e.g. first show user a confirmation dialog).
   * @event gridRowBeforeDropFinalize
   * @preventable
   * @async
   * @param {Core.helper.DragHelper} source
   * @param {Object} context
   * @param {Boolean} context.valid Set this to true or false to indicate whether the drop position is valid
   * @param {Core.data.Model} context.insertBefore The record to insert before (`null` if inserting at last position of a parent node)
   * @param {Core.data.Model} context.parent The parent record of the current drop position (only applicable for trees)
   * @param {Core.data.Model[]} context.records The dragged row records
   * @param {RecordPositionContext[]} context.oldPositionContext An array of objects with information about the previous tree position.
   * Objects contain the `record`, and its original `parentIndex` and `parentId` values
   * @param {MouseEvent} event
   * @on-owner
   */
  /**
   * Fired after the row drop operation has completed, regardless of validity
   * @event gridRowDrop
   * @param {Core.helper.DragHelper} source
   * @param {Object} context
   * @param {Boolean} context.valid true or false depending on whether the drop position was valid
   * @param {Core.data.Model} context.insertBefore The record to insert before (`null` if inserting at last position of a parent node)
   * @param {Core.data.Model} context.parent The parent record of the current drop position (only applicable for trees)
   * @param {Core.data.Model} context.record [DEPRECATED] The dragged row record
   * @param {Core.data.Model[]} context.records The dragged row records
   * @param {RecordPositionContext[]} context.oldPositionContext An array of objects with information about the previous tree position.
   * Objects contain the record, and its original `parentIndex` and `parentId` values
   * @param {MouseEvent} event
   * @on-owner
   */
  /**
   * Fired when a row drag operation is aborted
   * @event gridRowAbort
   * @param {Core.helper.DragHelper} source
   * @param {Object} context
   * @param {MouseEvent} event
   * @on-owner
   */
  static get deprecatedEvents() {
    return {
      gridRowBeforeDragStart: {
        product: 'Grid',
        invalidAsOfVersion: '6.0.0',
        message: '`gridRowBeforeDragStart` event is deprecated, listen on this event on the Grid instead.'
      },
      gridRowDragStart: {
        product: 'Grid',
        invalidAsOfVersion: '6.0.0',
        message: '`gridRowDragStart` event is deprecated, listen on this event on the Grid instead.'
      },
      gridRowDrag: {
        product: 'Grid',
        invalidAsOfVersion: '6.0.0',
        message: '`gridRowDrag` event is deprecated, listen on this event on the Grid instead.'
      },
      gridRowBeforeDropFinalize: {
        product: 'Grid',
        invalidAsOfVersion: '6.0.0',
        message: '`gridRowBeforeDropFinalize` event is deprecated, listen on this event on the Grid instead.'
      },
      gridRowDrop: {
        product: 'Grid',
        invalidAsOfVersion: '6.0.0',
        message: '`gridRowDrop` event is deprecated, listen on this event on the Grid instead.'
      },
      gridRowAbort: {
        product: 'Grid',
        invalidAsOfVersion: '6.0.0',
        message: '`gridRowAbort` event is deprecated, listen on this event on the Grid instead.'
      }
    };
  }
  //endregion
  //region Init
  static $name = 'RowReorder';
  static configurable = {
    /**
     * Set to `true` to show a grip icon on the left side of each row.
     * @config {Boolean}
     */
    showGrip: null,
    /**
     * Set to `true` to only allow reordering by the {@link #config-showGrip} config
     * @config {Boolean}
     */
    gripOnly: null,
    /**
     * If hovering over a parent node for this period of a time in a tree, the node will expand
     * @config {Number}
     */
    hoverExpandTimeout: 1000,
    /**
     * The amount of milliseconds to wait after a touchstart, before a drag gesture will be allowed to start.
     * @config {Number}
     * @default
     */
    touchStartDelay: 300,
    /**
     * Enables creation of parents by dragging a row and dropping it onto a leaf row. Only works in a Grid with
     * a tree store.
     * @config {Boolean}
     */
    dropOnLeaf: false,
    /**
     * An object used to configure the internal {@link Core.helper.DragHelper} class
     * @config {DragHelperConfig}
     */
    dragHelperConfig: null
  };
  construct(grid, config) {
    this.grid = grid;
    super.construct(...arguments);
  }
  doDestroy() {
    var _this$dragHelper;
    (_this$dragHelper = this.dragHelper) === null || _this$dragHelper === void 0 ? void 0 : _this$dragHelper.destroy();
    super.doDestroy();
  }
  /**
   * Initialize drag & drop (called on first paint)
   * @private
   */
  init() {
    const me = this,
      {
        grid
      } = me;
    me.dragHelper = DragHelper.new({
      name: 'rowReorder',
      cloneTarget: true,
      dragThreshold: 10,
      proxyTopOffset: 10,
      targetSelector: '.b-grid-row',
      lockX: true,
      dragWithin: grid.bodyContainer,
      allowDropOutside: true,
      scrollManager: grid.scrollManager,
      outerElement: me.targetSubGridElement,
      touchStartDelay: me.touchStartDelay,
      isElementDraggable: me.isElementDraggable.bind(me),
      monitoringConfig: {
        scrollables: [{
          element: grid.scrollable.element,
          direction: 'vertical'
        }]
      },
      setXY(element, x, y) {
        const {
          context
        } = this;
        if (!context.started) {
          const elementRect = Rectangle.from(context.element, this.dragWithin),
            pointerDownOffset = context.startPageY - globalThis.pageYOffset - context.element.getBoundingClientRect().top;
          // manually position the row a bit below the cursor
          y = elementRect.top + pointerDownOffset + this.proxyTopOffset;
        }
        DomHelper.setTranslateXY(element, x, y);
      },
      // Since parent nodes can expand after hovering, meaning original drag start position now refers to a different point in the tree
      ignoreSamePositionDrop: false,
      createProxy(element) {
        const clone = element.cloneNode(true),
          container = document.createElement('div');
        container.classList.add('b-row-reorder-proxy');
        clone.removeAttribute('id');
        // The containing element will be positioned instead, and sized using CSS
        clone.style.transform = '';
        clone.style.width = '';
        container.appendChild(clone);
        if (grid.selectedRecords.length > 1) {
          const clone2 = clone.cloneNode(true);
          clone2.classList.add('b-row-dragging-multiple');
          container.appendChild(clone2);
        }
        DomHelper.removeClsGlobally(container, 'b-selected', 'b-hover', 'b-focused');
        return container;
      },
      internalListeners: {
        beforedragstart: 'onBeforeDragStart',
        dragstart: 'onDragStart',
        drag: 'onDrag',
        drop: 'onDrop',
        reset: 'onReset',
        prio: 10000,
        // To ensure our listener is run before the relayed listeners (for the outside world)
        thisObj: me
      }
    }, me.dragHelperConfig);
    // Remove in 6.0
    me.relayEvents(me.dragHelper, ['beforeDragStart', 'dragStart', 'drag', 'abort'], 'gridRow');
    grid.relayEvents(me.dragHelper, ['beforeDragStart', 'dragStart', 'drag', 'abort'], 'gridRow');
    me.dropIndicator = DomHelper.createElement({
      className: 'b-row-drop-indicator'
    });
    me.dropOverTargetCls = ['b-row-reordering-target', 'b-hover'];
  }
  //endregion
  //region Plugin config
  static pluginConfig = {
    after: ['onPaint']
  };
  get targetSubGridElement() {
    const targetSubGrid = this.grid.regions[0];
    return this.grid.subGrids[targetSubGrid].element;
  }
  //endregion
  //region Events (drop)
  isElementDraggable(el, event) {
    if (!el.closest('.b-grid-cell .b-widget')) {
      if (this.gripOnly) {
        const firstCell = el.closest('.b-grid-cell:first-child');
        // Event is in the first cell. Now check if it's on the handle
        if (firstCell) {
          const gripperStyle = getComputedStyle(firstCell, ':before'),
            offsetX = this.grid.rtl ? firstCell.getBoundingClientRect().width - event.borderOffsetX : event.borderOffsetX,
            onGrip = DomHelper.roundPx(offsetX) <= DomHelper.roundPx(parseFloat(gripperStyle.width));
          // Prevent drag select if mousedown on grip, would collide with reordering
          // (reset by GridSelection)
          if (onGrip) {
            this.client.preventDragSelect = true;
          }
          return onGrip;
        }
      } else {
        return true;
      }
    }
  }
  onBeforeDragStart({
    event,
    source,
    context
  }) {
    const me = this,
      {
        grid
      } = me,
      subGridEl = me.targetSubGridElement;
    // Only dragging enabled in the leftmost grid section
    if (me.disabled || grid.readOnly || grid.isTreeGrouped || !subGridEl.contains(context.element)) {
      return false;
    }
    const startRecord = context.startRecord = grid.getRecordFromElement(context.element);
    // Don't allow starting drag on a readOnly record nor on special rows
    if (startRecord.readOnly || startRecord.isSpecialRow) {
      return false;
    }
    context.originalRowTop = grid.rowManager.getRowFor(startRecord).top;
    // Don't select row if checkboxOnly is set
    if (!grid.selectionMode.checkboxOnly) {
      if (source.startEvent.pointerType === 'touch') {
        // Touchstart doesn't focus/navigate on its own, so we do it at the last moment before drag start
        if (!grid.isSelected(startRecord)) {
          grid.selectRow({
            record: startRecord,
            addToSelection: false
          });
        }
      } else if (!grid.isSelected(startRecord) && !event.shiftKey && !event.ctrlKey) {
        // If record is not selected and shift/ctrl is not pressed then select single row
        grid.selectRow({
          record: startRecord
        });
      }
    }
    // Read-only records will not be moved
    const selectedRecords = grid.selectedRecords.filter(r => !r.readOnly);
    context.records = [startRecord];
    // If clicked record is selected, move all selected records
    if (selectedRecords.includes(startRecord)) {
      context.records.push(...selectedRecords.filter(r => r !== startRecord));
      context.records.sort((r1, r2) => grid.store.indexOf(r1) - grid.store.indexOf(r2));
    }
    return true;
  }
  onDragStart({
    context
  }) {
    var _cellMenu$hideContext, _headerMenu$hideConte;
    const me = this,
      {
        grid
      } = me,
      {
        cellEdit,
        cellMenu,
        headerMenu
      } = grid.features;
    if (cellEdit) {
      me.cellEditDisabledState = cellEdit.disabled;
      cellEdit.disabled = true; // prevent editing from being started through keystroke during row reordering
    }

    cellMenu === null || cellMenu === void 0 ? void 0 : (_cellMenu$hideContext = cellMenu.hideContextMenu) === null || _cellMenu$hideContext === void 0 ? void 0 : _cellMenu$hideContext.call(cellMenu, false);
    headerMenu === null || headerMenu === void 0 ? void 0 : (_headerMenu$hideConte = headerMenu.hideContextMenu) === null || _headerMenu$hideConte === void 0 ? void 0 : _headerMenu$hideConte.call(headerMenu, false);
    grid.element.classList.add('b-row-reordering');
    const focusedCell = context.element.querySelector('.b-focused');
    focusedCell === null || focusedCell === void 0 ? void 0 : focusedCell.classList.remove('b-focused');
    context.element.firstElementChild.classList.remove('b-selected', 'b-hover');
    grid.bodyContainer.appendChild(me.dropIndicator);
  }
  onDrag({
    context,
    event
  }) {
    const me = this,
      {
        grid
      } = me,
      {
        store,
        rowManager
      } = grid,
      {
        clientY
      } = event;
    let valid = true,
      row = rowManager.getRowAt(clientY),
      overRecord,
      dataIndex,
      after,
      over,
      insertBefore;
    if (row) {
      const rowTop = row.top + grid.scrollable.element.getBoundingClientRect().top - grid.scrollable.y,
        quarter = row.height / 4,
        topQuarter = rowTop + quarter,
        middleY = rowTop + row.height / 2,
        bottomQuarter = rowTop + quarter * 3;
      dataIndex = row.dataIndex;
      overRecord = store.getAt(dataIndex);
      // If Tree and pointer is in quarter 2 and 3, add as child of hovered row
      if (store.tree) {
        over = (overRecord.isParent || me.dropOnLeaf) && clientY > topQuarter && clientY < bottomQuarter;
      } else if (store.isGrouped) {
        over = overRecord.isGroupHeader && overRecord.meta.collapsed;
      }
      // Else, drop after row below if mouse is in bottom half of hovered row
      after = !over && event.clientY >= middleY;
    }
    // User dragged below last row or above the top row.
    else {
      if (event.pageY < grid._bodyRectangle.y) {
        dataIndex = 0;
        overRecord = store.first;
        after = false;
      } else {
        dataIndex = store.count - 1;
        overRecord = store.last;
        after = true;
      }
      row = grid.rowManager.getRow(dataIndex);
    }
    if (overRecord === me.overRecord && me.after === after && me.over === over) {
      context.valid = me.reorderValid;
      // nothing's changed
      return;
    }
    if (me.overRecord !== overRecord) {
      var _rowManager$getRowByI;
      (_rowManager$getRowByI = rowManager.getRowById(me.overRecord)) === null || _rowManager$getRowByI === void 0 ? void 0 : _rowManager$getRowByI.removeCls(me.dropOverTargetCls);
    }
    me.overRecord = overRecord;
    me.after = after;
    me.over = over;
    if (
    // Hovering the dragged record. This is a no-op.
    // But still gather the contextual data.
    overRecord === context.startRecord ||
    // Not allowed to drop above topmost group header or below a collapsed header
    !after && !over && dataIndex === 0 && store.isGrouped ||
    // Not allowed to drop after last collapsed group
    after && overRecord.isGroupHeader && overRecord.meta.collapsed && store.indexOf(overRecord) === store.count - 1) {
      valid = false;
    }
    if (store.tree) {
      insertBefore = after ? overRecord.nextSibling : overRecord;
      // For trees, prevent moving a parent into its own hierarchy
      if (context.records.some(rec => rec.contains(overRecord))) {
        valid = false;
      }
      context.parent = valid && over ? overRecord : overRecord.parent;
      me.clearTimeout(me.hoverTimer);
      if (overRecord && overRecord.isParent && !overRecord.isExpanded(store)) {
        me.hoverTimer = me.setTimeout(() => grid.expand(overRecord), me.hoverExpandTimeout);
      }
    } else {
      insertBefore = after ? store.getAt(dataIndex + 1) : overRecord;
    }
    row.toggleCls(me.dropOverTargetCls, valid && over);
    // If hovering results in same dataIndex, regardless of what row is hovered, and parent has not changed
    if (!over && dataIndex === store.indexOf(context.startRecord) + (after ? -1 : 1) && context.parent && context.startRecord.parent === context.parent) {
      valid = false;
    }
    // Provide visual clue to user of the drop position
    // In FF (in tests) it might not have had time to redraw rows after scroll before getting here
    row && DomHelper.setTranslateY(me.dropIndicator, Math.max(row.top + (after ? row.element.getBoundingClientRect().height : 0), 1));
    // Don't show dropIndicator if holding over a row
    me.dropIndicator.style.visibility = over ? 'hidden' : 'visible';
    me.dropIndicator.classList.toggle('b-drag-invalid', !valid);
    // Public property used for validation
    context.insertBefore = insertBefore;
    context.valid = me.reorderValid = valid;
  }
  /**
   * Handle drop
   * @private
   */
  async onDrop(event) {
    const me = this,
      {
        client
      } = me,
      {
        context
      } = event;
    context.valid = context.valid && me.reorderValid;
    if (context.valid) {
      context.async = true;
      if (client.store.tree) {
        // For tree scenario, add context about previous positions of dragged tree nodes
        context.oldPositionContext = context.records.map(record => {
          var _record$parent;
          return {
            record,
            parentId: (_record$parent = record.parent) === null || _record$parent === void 0 ? void 0 : _record$parent.id,
            parentIndex: record.parentIndex
          };
        });
      }
      // Remove for 6.0
      let result = await me.trigger('gridRowBeforeDropFinalize', event);
      if (result === false) {
        context.valid = false;
      }
      // Outside world provided us one or more Promises to wait for
      result = await client.trigger('gridRowBeforeDropFinalize', event);
      if (result === false) {
        context.valid = false;
      }
      await me.dragHelper.animateProxyTo(me.dropIndicator, {
        align: 'l0-l0'
      });
      await me.finalizeReorder(context);
    }
    // already dropped the node, don't have to expand any node hovered anymore
    // (cancelling expand action after timeout)
    me.clearTimeout(me.hoverTimer);
    me.overRecord = me.after = me.over = null;
    me.trigger('gridRowDrop', event);
    client.trigger('gridRowDrop', event);
  }
  async finalizeReorder(context) {
    const me = this,
      {
        grid
      } = me,
      {
        store,
        focusedCell
      } = grid;
    let {
      records
    } = context;
    context.valid = context.valid && !records.some(rec => !store.includes(rec));
    if (context.valid) {
      let result;
      if (store.tree) {
        var _context$parent$child, _context$parent$child2;
        // Remove any selected child records of parent nodes
        records = records.filter(record => !record.parent || record.bubbleWhile(parent => !records.includes(parent), true));
        result = await context.parent.tryInsertChild(records, me.over ? (_context$parent$child = context.parent.children) === null || _context$parent$child === void 0 ? void 0 : _context$parent$child[0] : context.insertBefore);
        // remove reorder cls from preview parent element dropped
        grid.rowManager.forEach(r => r.removeCls(me.dropOverTargetCls));
        // If parent wasn't expanded, expand it if it now has children
        if (!context.parent.isExpanded() && (_context$parent$child2 = context.parent.children) !== null && _context$parent$child2 !== void 0 && _context$parent$child2.length) {
          grid.expand(context.parent);
        }
        context.valid = result !== false;
      } else if (store.isGrouped && me.over) {
        store.move(records, store.getAt(store.indexOf(context.insertBefore) + 1));
      } else {
        store.move(records, context.insertBefore);
      }
      if ((focusedCell === null || focusedCell === void 0 ? void 0 : focusedCell._rowIndex) >= 0) {
        grid._focusedCell = null;
        // Refresh focused cell
        grid.focusCell({
          grid,
          record: focusedCell.record,
          columnId: focusedCell.columnId
        });
      }
      store.clearSorters();
    }
    context.finalize(context.valid);
    grid.element.classList.remove('b-row-reordering');
  }
  /**
   * Clean up on reset
   * @private
   */
  onReset() {
    const me = this,
      {
        grid
      } = me,
      cellEdit = grid.features.cellEdit;
    grid.element.classList.remove('b-row-reordering');
    if (cellEdit) {
      cellEdit.disabled = me.cellEditDisabledState;
    }
    me.dropIndicator.remove();
    DomHelper.removeClsGlobally(grid.element, ...me.dropOverTargetCls);
  }
  //endregion
  //region Render
  onPaint({
    firstPaint
  }) {
    // columns shown, hidden or reordered
    if (firstPaint) {
      this.init();
    }
  }
  //endregion
  updateShowGrip(show) {
    this.grid.element.classList.toggle('b-row-reorder-with-grip', show);
  }
}
RowReorder.featureClass = '';
RowReorder._$name = 'RowReorder';
GridFeatureManager.registerFeature(RowReorder, false);
GridFeatureManager.registerFeature(RowReorder, true, 'Gantt');

/**
 * @module Grid/feature/TreeGroup
 */
/**
 * A feature that allows transforming a flat dataset (or the leaves of a hierarchical) into a tree by specifying a
 * record field per parent level. Parents are generated based on each leaf's value for those fields.
 *
 * {@inlineexample Grid/feature/TreeGroup.js}
 *
 * This feature can be used to mimic multi grouping or to generate another view for hierarchical data. The actual
 * transformation happens in a new store, that contains links to the original records. The original stores structure is
 * kept intact and will be plugged back in when calling {@link #function-clearGroups}.
 *
 * Any modification of the links is relayed to the original store. So cell editing etc. will work as expected and the
 * original data will be updated.
 *
 * <div class="note">
 * Please note that this feature requires using a {@link Grid.view.TreeGrid} or having the {@link Grid.feature.Tree}
 * feature enabled.
 * </div>
 *
 * This snippet shows how the sample dataset used in the demo above is transformed:
 *
 * ```javascript
 * const grid = new TreeGrid({
 *     // Original data
 *     data : [
 *         { id : 1, name : 'Project 1', children : [
 *             { id : 11, name : 'Task 11', status : 'wip', prio : 'high' },
 *             { id : 12, name : 'Task 12', status : 'done', prio : 'low' },
 *             { id : 13, name : 'Task 13', status : 'done', prio : 'high' }
 *         ]},
 *         { id : 2, name : 'Project 2', children : [
 *             { id : 21, name : 'Task 21', status : 'wip', prio : 'high' },
 *         ]}
 *     ],
 *
 *     features : {
 *         treeGroup : {
 *             // Fields to build a new tree from
 *             levels : [ 'prio', 'status' ]
 *         }
 *     }
 * });
 *
 * // Resulting data
 * [
 *     { name : 'low', children : [
 *         { name : 'done', children : [
 *             { id : 12, name : 'Task 12', status : 'done', prio : 'low' }
 *         ]}
 *     ]},
 *     { name : 'high', children : [
 *         { name : 'done', children : [
 *             { id : 13, name : 'Task 13', status : 'done', prio : 'high' }
 *         ]},
 *         { name : 'wip', children : [
 *             { id : 11, name : 'Task 11', status : 'wip', prio : 'high' },
 *             { id : 21, name : 'Task 21', status : 'wip', prio : 'low' }
 *         ]}
 *     ]}
 * ]
 * ```
 *
 * ## Important information
 *
 * Using the TreeGroup feature comes with some caveats:
 *
 * * Generated parents are read-only, they cannot be edited using the default UI.
 * * Moving nodes manually in the tree is not supported while it is grouped. The linked records have their own
 *   `parentId` fields, not linked to the original records value.
 * * The generated structure is not meant to be persisted.
 *
 * <div class="note">
 * Please note that this feature is not supported in vertical mode in Scheduler.
 * </div>
 *
 * This feature is <strong>disabled</strong> by default.
 *
 * @extends Core/mixin/InstancePlugin
 *
 * @classtype treeGroup
 * @feature
 */
class TreeGroup extends InstancePlugin {
  static $name = 'TreeGroup';
  static configurable = {
    /**
     * An array of model field names or functions used to determine the levels in the resulting tree.
     *
     * When supplying a function, it will be called for each leaf in the original data, and it is expected to return
     * an atomic value used to determine which parent the leaf will be added to at that level.
     *
     * ```javascript
     * const grid = new TreeGrid({
     *     features : {
     *         treeGroup : {
     *             levels : [
     *                 // First level is determined by the value of the status field
     *                 'status',
     *                 // Second level by the result of this function
     *                 // (which puts percentdone 0-9 in one group, 10-19 into another and so on)
     *                 record => (record.percentDone % 10) * 10
     *             ]
     *         }
     *     }
     * });
     * ```
     *
     * The function form can also be used as a formatter/renderer of sorts, simply by returning a string:
     *
     * ```javascript
     * const grid = new TreeGrid({
     *     features : {
     *         treeGroup : {
     *             levels : [
     *                 record => `Status: ${record.status}`
     *             ]
     *         }
     *     }
     * });
     * ```
     *
     * Assigning `null` restores data to its original state.
     *
     * @prp {Array<String|Function(Core.data.Model) : any>} levels
     */
    levels: null,
    /**
     * CSS class to apply to the generated parents.
     *
     * @config {String}
     * @default
     */
    parentCls: 'b-generated-parent'
  };
  /**
   * A reference to the original store when a tree grouping has been applied. Use this to modify / load data
   * while tree grouping is active.
   * @member {Core.data.Store} originalStore
   * @readonly
   */
  static pluginConfig = {
    assign: ['group', 'clearGroups']
  };
  static properties = {
    isApplying: 0,
    originalStore: null
  };
  construct(grid, config) {
    super.construct(grid, config);
    if (!grid.hasFeature('tree')) {
      throw new Error('The TreeGroup feature requires the Tree feature to be enabled');
    }
  }
  processParentData(parentData) {
    // Apply cls to allow custom styling of generated parents
    if (this.parentCls) {
      parentData.cls = this.parentCls;
    }
  }
  processTransformedData(transformedData) {}
  async waitForReadiness() {
    // Wait for store to finish loading before transforming the data
    if (this.originalStore.isLoading) {
      await this.originalStore.await('load', false);
      if (this.isDestroyed) {
        return;
      }
    }
    // For Scheduler, Pro & Gantt, to not have to implement a TreeGroup in Scheduler just to add this
    const {
      crudManager
    } = this.client;
    if (crudManager) {
      if (crudManager.isLoadingOrSyncing || crudManager._autoLoadPromise) {
        await crudManager.await('requestDone');
        if (this.isDestroyed) {
          return;
        }
      }
      await this.client.project.commitAsync();
    }
  }
  async applyLevels(levels) {
    const me = this,
      {
        client
      } = me,
      treeColumn = client.columns.find(col => col instanceof TreeColumn);
    let {
        store
      } = client,
      result = null;
    me._levels = levels;
    me.isApplying++;
    client.suspendRefresh();
    if (levels && !me.originalStore) {
      var _store$load;
      me.originalStore = store;
      store = new store.constructor({
        tree: true,
        modelClass: store.modelClass,
        load: (_store$load = store.load) === null || _store$load === void 0 ? void 0 : _store$load.bind(store),
        commit: store.commit.bind(store)
      });
      client.store = store;
      me.originalStore.ion({
        name: 'originalStore',
        refresh: me.onOriginalStoreRefresh,
        add: me.onOriginalStoreChanged,
        remove: me.onOriginalStoreChanged,
        removeAll: me.onOriginalStoreChanged,
        thisObj: me
      });
    }
    await me.waitForReadiness();
    if (me.isDestroyed) {
      return;
    }
    // Applying custom levels
    if (levels) {
      // Plug links in to allow transforming them below
      store.data = me.originalStore.allRecords.flatMap(record => record.isLeaf ? [record.link()] : []);
      // Transform it according to levels
      result = store.treeify(levels, parentData => {
        // Use group key as tree columns content
        ObjectHelper.setPath(parentData, store.modelClass.getFieldDataSource(treeColumn.field), parentData.key);
        // Let the outside world manipulate generated parents data before turning it into a record
        me.processParentData(parentData);
      });
      me.processTransformedData(result);
      // Load the transformed result into the "display store"
      store.data = result.children;
    }
    // Clearing custom levels
    else {
      client.store = me.originalStore;
      me.detachListeners('originalStore');
      me.originalStore = null;
    }
    me.isApplying--;
    client.resumeRefresh();
    client.rowManager.reinitialize();
    return result;
  }
  onOriginalStoreChanged() {
    this.applyLevels(this._levels);
  }
  onOriginalStoreRefresh({
    action
  }) {
    if (action === 'dataset') {
      this.applyLevels(this.levels);
    }
  }
  updateLevels(levels) {
    if (levels || !this.isConfiguring) {
      this.applyLevels(levels);
    }
  }
  /**
   * Transforms the data according to the supplied levels.
   *
   * Yields the same result as assigning to {@link #property-levels}.
   *
   * ```javascript
   * // Transform into a tree with two parent levels
   * grid.group('status', record => (record.percentDone % 10) * 10);
   * ```
   *
   * @param {Array<String|Function(Core.data.Model) : any>} levels Field names or functions use to generate parents in resulting tree.
   * @on-owner
   * @category Common
   */
  async group(levels) {
    ObjectHelper.assertArray(levels, 'group()');
    await this.applyLevels(levels);
  }
  /**
   * Clears the previously applied transformation, restoring data to its initial state.
   *
   * Yields the same result as assigning `null` to {@link #property-levels}.
   *
   * ```javascript
   * // Restore original data
   * grid.clearGroupers();
   * ```
   * @on-owner
   * @category Common
   */
  async clearGroups() {
    if (this.isGrouped) {
      await this.applyLevels(null);
    }
  }
  get isGrouped() {
    return Boolean(this._levels);
  }
}
TreeGroup._$name = 'TreeGroup';
GridFeatureManager.registerFeature(TreeGroup);

export { RowReorder, TreeGroup };
//# sourceMappingURL=TreeGroup.js.map
