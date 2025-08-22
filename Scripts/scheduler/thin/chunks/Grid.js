/*!
 *
 * Bryntum Scheduler Pro 5.3.7
 *
 * Copyright(c) 2023 Bryntum AB
 * https://bryntum.com/contact
 * https://bryntum.com/license
 *
 */
import { GridFeatureManager, GridBase } from './GridBase.js';
import { Delayable, InstancePlugin } from './Editor.js';

const storeListenerName = 'store';
/**
 * @module Grid/feature/ColumnAutoWidth
 */
/**
 * Enables the {@link Grid.column.Column#config-autoWidth} config for a grid's columns.
 *
 * This feature is <strong>enabled</strong> by default.
 *
 * @extends Core/mixin/InstancePlugin
 * @mixes Core/mixin/Delayable
 * @classtype columnAutoWidth
 * @feature
 */
class ColumnAutoWidth extends Delayable(InstancePlugin) {
  static get $name() {
    return 'ColumnAutoWidth';
  }
  //region Config
  static get configurable() {
    return {
      /**
       * The default `autoWidth` option for columns with `autoWidth: true`. This can
       * be a single number for the minimum column width, or an array of two numbers
       * for the `[minWidth, maxWidth]`.
       * @config {Number|Number[]}
       */
      default: null,
      /**
       * The amount of time (in milliseconds) to delay after a store modification
       * before synchronizing `autoWidth` columns.
       * @config {Number}
       * @default
       */
      delay: 0
    };
  }
  //endregion
  //region Internals
  static get pluginConfig() {
    return {
      after: {
        bindStore: 'bindStore',
        unbindStore: 'unbindStore',
        renderRows: 'syncAutoWidthColumns',
        onInternalResize: 'onInternalResize'
      },
      assign: ['columnAutoWidthPending', 'syncAutoWidthColumns']
    };
  }
  construct(config) {
    super.construct(config);
    const {
      store
    } = this.client;
    // The initial bindStore can come super early such that our hooks won't catch it:
    store && this.bindStore(store);
  }
  doDestroy() {
    this.unbindStore();
    super.doDestroy();
  }
  bindStore(store) {
    this.lastSync = null;
    store.ion({
      name: storeListenerName,
      [`change${this.client.asyncEventSuffix}`]: 'onStoreChange',
      thisObj: this
    });
  }
  unbindStore() {
    this.detachListeners(storeListenerName);
  }
  get columnAutoWidthPending() {
    return this.lastSync === null || this.hasTimeout('syncAutoWidthColumns');
  }
  onStoreChange({
    action
  }) {
    if (action !== 'move') {
      const me = this,
        {
          cellEdit
        } = me.client.features;
      ++me.storeGeneration;
      // If we are editing, sync right away so cell editing can align correctly to next cell
      // unless editing is finished/canceled by tapping outside of grid body
      if (cellEdit !== null && cellEdit !== void 0 && cellEdit.isEditing && !cellEdit.editingStoppedByTapOutside) {
        me.syncAutoWidthColumns();
      } else if (!me.hasTimeout('syncAutoWidthColumns')) {
        me.setTimeout('syncAutoWidthColumns', me.delay);
      }
    }
  }
  // Handle scenario with Grid being inside DIV with display none, and no width. Sync column widths after being shown
  onInternalResize(element, newWidth, newHeight, oldWidth) {
    if (oldWidth === 0) {
      // Force remeasure after we get a width
      this.lastSync = null;
      this.syncAutoWidthColumns();
    }
  }
  syncAutoWidthColumns() {
    const me = this,
      grid = me.client,
      storeGeneration = me.storeGeneration;
    if (me.lastSync !== storeGeneration) {
      me.lastSync = storeGeneration;
      let autoWidth, resizingColumns;
      for (const column of grid.columns.visibleColumns) {
        autoWidth = column.autoWidth;
        if (autoWidth) {
          if (autoWidth === true) {
            autoWidth = me.default;
          }
          grid.resizingColumns = resizingColumns = true;
          column.resizeToFitContent(autoWidth);
        }
      }
      if (resizingColumns) {
        grid.resizingColumns = false;
        grid.afterColumnsResized();
      }
    }
    if (me.hasTimeout('syncAutoWidthColumns')) {
      me.clearTimeout('syncAutoWidthColumns');
    }
  }
  //endregion
}

ColumnAutoWidth.prototype.storeGeneration = 0;
ColumnAutoWidth._$name = 'ColumnAutoWidth';
GridFeatureManager.registerFeature(ColumnAutoWidth, true);

/**
 * @module Grid/feature/RowCopyPaste
 */
/**
 * Allow using [Ctrl/CMD + C/X] and [Ctrl/CMD + V] to copy/cut and paste rows. Also makes cut, copy and paste actions
 * available via the cell context menu.
 *
 * You can configure how a newly pasted record is named using {@link #function-generateNewName}
 *
 * This feature is **enabled** by default
 *
 * ```javascript
 * const grid = new Grid({
 *     features : {
 *         rowCopyPaste : true
 *     }
 * });
 * ```
 *
 * {@inlineexample Grid/feature/RowCopyPaste.js}
 *
 * This feature will work alongside with CellCopyPaste but there is differences on functionality.
 * * When used together, context menu options will be detailed so the user will know to copy the cell or the row.
 * * They will also detect what type of selection is present at the moment. If there is only rows selected, only row
 *   alternatives are shown in the context menu and the keyboard shortcuts will be processed by RowCopyPaste.
 * * If there is only cells selected, there will be context menu options for both row and cell but keyboard shortcuts
 *   will be handled by CellCopyPaste.
 * * They do not share clipboard, so it is possible to have rows and cells copied or cut at the same time.
 *
 * ## Keyboard shortcuts
 * The feature has the following default keyboard shortcuts:
 *
 * | Keys       | Action  | Weight ¹ | Action description                                                                      |
 * |------------|---------|:--------:|-----------------------------------------------------------------------------------------|
 * | `Ctrl`+`C` | *copy*  | 10       | Calls {@link #function-copyRows} which copies selected row(s) into the clipboard.       |
 * | `Ctrl`+`X` | *cut*   | 10       | Calls {@link #function-copyRows} which cuts out selected row(s) and saves in clipboard. |
 * | `Ctrl`+`V` | *paste* | 10       | Calls {@link #function-pasteRows} which inserts copied or cut row(s) from the clipboard.|
 *
 * **¹** Customization of keyboard shortcuts that has a `weight` can affect other features that also uses that
 * particular keyboard shortcut. Read more in [our guide](#Grid/guides/customization/keymap.md).
 *
 * <div class="note">Please note that <code>Ctrl</code> is the equivalent to <code>Command</code> and <code>Alt</code>
 * is the equivalent to <code>Option</code> for Mac users</div>
 *
 * For more information on how to customize keyboard shortcuts, please see
 * [our guide](#Grid/guides/customization/keymap.md).
 *
 * @extends Core/mixin/InstancePlugin
 * @classtype rowCopyPaste
 * @feature
 */
class RowCopyPaste extends InstancePlugin {
  static $name = 'RowCopyPaste';
  static type = 'rowCopyPaste';
  static pluginConfig = {
    assign: ['copyRows', 'pasteRows'],
    chain: ['populateCellMenu']
  };
  clipboardRecords = [];
  static configurable = {
    /**
     * The field to use as the name field when updating the name of copied records
     * @config {String}
     * @default
     */
    nameField: 'name',
    /**
     * See {@link #keyboard-shortcuts Keyboard shortcuts} for details
     * @config {Object<String,String>}
     */
    keyMap: {
      // Weight to give CellCopyPaste priority
      'Ctrl+C': {
        weight: 10,
        handler: 'copy'
      },
      'Ctrl+X': {
        weight: 10,
        handler: 'cut'
      },
      'Ctrl+V': {
        weight: 10,
        handler: 'paste'
      }
    },
    copyRecordText: 'L{copyRecord}',
    cutRecordText: 'L{cutRecord}',
    pasteRecordText: 'L{pasteRecord}',
    rowSpecifierText: 'L{row}',
    rowSpecifierTextPlural: 'L{rows}',
    localizableProperties: ['copyRecordText', 'cutRecordText', 'pasteRecordText', 'rowSpecifierText', 'rowSpecifierTextPlural'],
    /**
     * Adds `Cut (row)`, `Copy (row)` and `Paste (row)` options when opening a context menu on a selected cell when
     * {@link Grid.view.mixin.GridSelection#config-selectionMode cellSelection} and
     * {@link Grid.feature.CellCopyPaste} is active. Default behaviour will only provide row copy/paste actions on a
     * selected row.
     * @config {Boolean}
     * @default
     */
    rowOptionsOnCellContextMenu: false
  };
  construct(grid, config) {
    super.construct(grid, config);
    grid.rowManager.ion({
      beforeRenderRow: 'onBeforeRenderRow',
      thisObj: this
    });
    this.grid = grid;
  }
  // Used in events to separate events from different features from each other
  entityName = 'row';
  onBeforeRenderRow({
    row,
    record
  }) {
    row.cls['b-cut-row'] = this._isCut && this.clipboardRecords.includes(record);
  }
  isActionAvailable({
    event
  }) {
    var _grid$selectedRecords;
    const {
        grid
      } = this,
      {
        cellEdit
      } = grid.features,
      {
        target
      } = event;
    // No action if
    // 1. there is selected text on the page
    // 2. cell editing is active
    // 3. cursor is not in the grid (filter bar etc)
    return !this.disabled && globalThis.getSelection().toString().length === 0 && (!cellEdit || !cellEdit.isEditing) && ((_grid$selectedRecords = grid.selectedRecords) === null || _grid$selectedRecords === void 0 ? void 0 : _grid$selectedRecords.length) > 0 && (
    // No key action when no selected records
    !target || Boolean(target.closest('.b-gridbase:not(.b-schedulerbase) .b-grid-subgrid,.b-grid-subgrid:not(.b-timeaxissubgrid)')));
  }
  copy() {
    this.copyRows();
  }
  cut() {
    this.copyRows(true);
  }
  paste(referenceRecord) {
    this.pasteRows(referenceRecord !== null && referenceRecord !== void 0 && referenceRecord.isModel ? referenceRecord : null);
  }
  /**
   * Copy or cut rows to clipboard to paste later
   *
   * @fires beforeCopy
   * @param {Boolean} [isCut] Copies by default, pass `true` to cut
   * @category Common
   * @on-owner
   */
  copyRows(isCut = false) {
    const me = this,
      {
        client,
        entityName
      } = me,
      // Don't cut readOnly records
      records = me.selectedRecords.filter(r => !r.readOnly || !isCut);
    /**
     * Fires on the owning Grid before a copy action is performed, return `false` to prevent the action
     * @event beforeCopy
     * @preventable
     * @on-owner
     * @param {Grid.view.Grid} source Owner grid
     * @param {Core.data.Model[]} records The records about to be copied
     * @param {Boolean} isCut `true` if this is a cut action
     * @param {String} entityName 'row' to distinguish this event from other beforeCopy events
     */
    if (!records.length || client.readOnly || client.trigger('beforeCopy', {
      records,
      isCut,
      entityName
    }) === false) {
      return;
    }
    me._isCut = isCut;
    me.clipboardRecords.forEach(rec => {
      var _client$rowManager$ge;
      return (_client$rowManager$ge = client.rowManager.getRowById(rec)) === null || _client$rowManager$ge === void 0 ? void 0 : _client$rowManager$ge.removeCls('b-cut-row');
    });
    me.clipboardRecords = records.slice();
    client.store.forEach(rec => {
      rec.meta.isCut = me._isCut && me.clipboardRecords.includes(rec);
    });
    // refresh to call reapply the cls for records where the cut was canceled
    records.forEach(record => this.onRowCutOrCopy(record, isCut));
    /**
     * Fires on the owning Grid after a copy action is performed.
     * @event copy
     * @on-owner
     * @param {Grid.view.Grid} source Owner grid
     * @param {Core.data.Model[]} records The records that were copied
     * @param {Boolean} isCut `true` if this is a cut action
     * @param {String} entityName 'row' to distinguish this event from other copy events
     */
    client.trigger('copy', {
      records,
      isCut,
      entityName
    });
  }
  onRowCutOrCopy(record, isCut) {
    var _this$client$rowManag;
    (_this$client$rowManag = this.client.rowManager.getRowById(record)) === null || _this$client$rowManag === void 0 ? void 0 : _this$client$rowManag.toggleCls('b-cut-row', isCut);
  }
  /**
   * Paste rows below selected or passed record
   *
   * @fires beforePaste
   * @param {Core.data.Model} [record] Paste below this record, or currently selected record if left out
   * @category Common
   * @on-owner
   */
  pasteRows(record) {
    var _client$getRowFor, _client$getRowFor$cel;
    const me = this,
      records = me.clipboardRecords,
      {
        client,
        entityName
      } = me,
      {
        store
      } = client,
      referenceRecord = record || client.selectedRecord;
    /**
     * Fires on the owning Grid before a paste action is performed, return `false` to prevent the action
     * @event beforePaste
     * @preventable
     * @on-owner
     * @param {Grid.view.Grid} source Owner grid
     * @param {Core.data.Model} referenceRecord The reference record, the clipboard records will be pasted below
     * this record
     * @param {Core.data.Model[]} records The records about to be pasted
     * @param {Boolean} isCut `true` if this is a cut action
     * @param {String} entityName 'row' to distinguish this event from other beforePaste events
     */
    if (client.readOnly || client.isTreeGrouped || !records.length || store.tree && me._isCut && records.some(rec => rec.contains(referenceRecord, true)) || client.trigger('beforePaste', {
      records,
      referenceRecord,
      isCut: me._isCut,
      entityName
    }) === false) {
      return [];
    }
    // sort selected to move records to make sure it will be added in correct order independent of how it was selected.
    // Should be done with real records in the clipboard, after records are copied, all indexes will be changed
    me.sortByIndex(records);
    const idMap = {},
      // We need to go over selected records, find all top level nodes and reassemble the tree
      recordsToProcess = me.extractParents(records, idMap);
    me.insertCopiedRecords(recordsToProcess, referenceRecord);
    if (me._isCut) {
      // reset clipboard
      me.clearClipboard();
    } else {
      client.selectedRecords = recordsToProcess;
    }
    /**
     * Fires on the owning Grid after a paste action is performed.
     * @event paste
     * @on-owner
     * @param {Grid.view.Grid} source Owner grid
     * @param {Core.data.Model} referenceRecord The reference record, below which the records were pasted
     * @param {Core.data.Model[]} records Pasted records
     * @param {Core.data.Model[]} originalRecords For a copy action, these are the records that were copied.
     * For cut action, this is same as the `records` param.
     * @param {Boolean} isCut `true` if this is a cut action
     * @param {String} entityName 'row' to distinguish this event from other paste events
     */
    client.trigger('paste', {
      records: recordsToProcess,
      originalRecords: records,
      isCut: me._isCut,
      referenceRecord,
      entityName
    });
    // Focus first cell of last copied or cut row
    (_client$getRowFor = client.getRowFor(recordsToProcess[recordsToProcess.length - 1])) === null || _client$getRowFor === void 0 ? void 0 : (_client$getRowFor$cel = _client$getRowFor.cells) === null || _client$getRowFor$cel === void 0 ? void 0 : _client$getRowFor$cel[0].focus();
    return recordsToProcess;
  }
  /**
   * Clears the clipboard and refreshes the UI
   *
   * @category Common
   */
  clearClipboard() {
    if (this._isCut) {
      this.clipboardRecords.forEach(rec => {
        var _this$client$rowManag2;
        return (_this$client$rowManag2 = this.client.rowManager.getRowById(rec)) === null || _this$client$rowManag2 === void 0 ? void 0 : _this$client$rowManag2.removeCls('b-cut-row');
      });
    }
    this._isCut = false;
    this.clipboardRecords = [];
  }
  /**
   * A method used to generate the name for a copy-pasted record. By defaults appends "- 2", "- 3" as a suffix. Override
   * it to provide your own naming of pasted records.
   *
   * @param {Core.data.Model} record The new record being pasted
   * @returns {String}
   */
  generateNewName(record) {
    const originalName = record[this.nameField];
    let counter = 2;
    while (this.client.store.findRecord(this.nameField, `${originalName} - ${counter}`)) {
      counter++;
    }
    return `${originalName} - ${counter}`;
  }
  insertCopiedRecords(toInsert, recordReference) {
    const {
        store
      } = this.client,
      insertAt = store.indexOf(recordReference) + 1;
    if (store.tree) {
      return recordReference.parent.insertChild(toInsert, recordReference.nextSibling, false, {
        // Specify node to insert before in the ordered tree. It allows to paste to a
        // correct place both ordered and visual.
        // Covered by TaskOrderedWbs.t.js
        orderedBeforeNode: recordReference.nextOrderedSibling
      });
    } else if (this._isCut) {
      store.move(toInsert, store.getAt(insertAt));
    } else {
      return store.insert(insertAt, toInsert);
    }
  }
  get selectedRecords() {
    const records = [...this.client.selectedRecords];
    // Add eventual selected cells records
    this.client.selectedCells.forEach(cell => {
      if (!records.includes(cell.record)) {
        records.push(cell.record);
      }
    });
    return records;
  }
  getMenuItemText(action, addRowSpecifier = false) {
    const me = this;
    let text = me[action + 'RecordText'];
    // If cellCopyPaste is enabled and there is selected cells, add a row specifier text to menu options
    if (addRowSpecifier) {
      text += ` (${me.selectedRecords.length > 1 ? me.rowSpecifierTextPlural : me.rowSpecifierText})`;
    }
    return text;
  }
  populateCellMenu({
    record,
    items,
    cellSelector
  }) {
    var _client$features$cell;
    const me = this,
      {
        client,
        rowOptionsOnCellContextMenu
      } = me,
      cellCopyPaste = ((_client$features$cell = client.features.cellCopyPaste) === null || _client$features$cell === void 0 ? void 0 : _client$features$cell.enabled) === true,
      // If cellCopyPaste is active and contextmenu originates from a selected cell
      targetIsCell = cellCopyPaste && client.isCellSelected(cellSelector);
    if (!client.readOnly && !client.isTreeGrouped && (record === null || record === void 0 ? void 0 : record.isSpecialRow) === false && (cellCopyPaste ? client.selectedRows.length : client.selectedRecords.length) && (!targetIsCell || me.rowOptionsOnCellContextMenu)) {
      items.cut = {
        text: me.getMenuItemText('cut', targetIsCell && rowOptionsOnCellContextMenu),
        localeClass: me,
        icon: 'b-icon b-icon-cut',
        weight: 135,
        disabled: record.readOnly,
        onItem: () => me.cut()
      };
      items.copy = {
        text: me.getMenuItemText('copy', targetIsCell && rowOptionsOnCellContextMenu),
        localeClass: me,
        cls: 'b-separator',
        icon: 'b-icon b-icon-copy',
        weight: 120,
        onItem: () => me.copy()
      };
      items.paste = {
        text: me.getMenuItemText('paste', targetIsCell && rowOptionsOnCellContextMenu),
        localeClass: me,
        icon: 'b-icon b-icon-paste',
        weight: 140,
        disabled: !me.clipboardRecords.length,
        onItem: () => me.paste(record)
      };
    }
  }
  /**
   * Sort array of records ASC by its indexes stored in indexPath
   * @param {Core.data.Model[]} array array to sort
   * @private
   */
  sortByIndex(array) {
    const {
      store
    } = this.client;
    return array.sort((rec1, rec2) => {
      const idx1 = rec1.indexPath,
        idx2 = rec2.indexPath;
      // When a record is copied without its parent, its index in the visible tree should be used
      if (!array.includes(rec1.parent) && !array.includes(rec2.parent)) {
        // For row copy-paste feature both records are normally in store. Unless someone wants
        // to include invisible records. Which does not happen yet.
        return store.indexOf(rec1) - store.indexOf(rec2);
      }
      if (idx1.length === idx2.length) {
        for (let i = 0; i < idx1.length; i++) {
          if (idx1[i] < idx2[i]) {
            return -1;
          }
          if (idx1[i] > idx2[i]) {
            return 1;
          }
        }
        return 0;
      } else {
        return idx1.length - idx2.length;
      }
    });
  }
  /**
   * Iterates over passed pre-sorted list of records and reassembles hierarchy of records.
   * @param {Core.data.Model[]} taskRecords array of records to extract parents from
   * @param {Object} idMap Empty object which will contain map linking original id with copied record
   * @returns {Core.data.Model[]} Returns array of new top-level nodes with children filled
   * @private
   */
  extractParents(taskRecords, idMap) {
    const me = this,
      {
        client,
        _isCut: isCut
      } = me;
    // Unwrap children to pass them all through `generateNewName` function
    if (client.store.tree) {
      taskRecords.forEach(node => {
        node.traverse(n => {
          const parents = n.getTopParent(true);
          if (!taskRecords.includes(n) && (!isCut || !taskRecords.some(rec => parents.includes(rec)))) {
            taskRecords.push(n);
          }
        });
      });
    }
    const result = taskRecords.reduce((parents, node) => {
      let copy;
      // Fallback is for when the node was removed from the tree
      const parentId = node.parentId || node.meta.modified;
      if (isCut) {
        copy = node;
        // reset record cut state
        copy.meta.isCut = false;
      } else {
        copy = node.copy();
        copy[me.nameField] = me.generateNewName(copy);
        // Ensure initial expanded state in new node matches state that the client's
        // store has for source node.
        copy.data.expanded = node.isExpanded(client.store);
      }
      idMap[node.id] = copy;
      // If we're copying top level node, add it directly
      if (node.parent === client.store.rootNode) {
        parents.push(copy);
      }
      // If node parent is also copied, append copy to the copied parent. Parents
      // are always at the beginning of the array, so we know if there is a parent
      // it was already copied
      else if (parentId in idMap) {
        idMap[parentId].appendChild(copy, true); // Silent to not cause redraws
      }
      // If parent is not copied and record is not top level, then append it as a
      // sibling.
      else {
        parents.push(copy);
      }
      return parents;
    }, []);
    // Now when tree is assembled we want to restore ordered tree. Traverse the tree, sort children
    // by previous value of `orderedParentIndex`
    result.forEach(parent => {
      parent.sortOrderedChildren(true, true);
    });
    return result;
  }
}
RowCopyPaste.featureClass = 'b-row-copypaste';
RowCopyPaste._$name = 'RowCopyPaste';
GridFeatureManager.registerFeature(RowCopyPaste, true, 'Grid');
GridFeatureManager.registerFeature(RowCopyPaste, false, 'Gantt');
GridFeatureManager.registerFeature(RowCopyPaste, false, 'SchedulerPro');
GridFeatureManager.registerFeature(RowCopyPaste, false, 'ResourceHistogram');

//region Import
//endregion
/**
 * @module Grid/view/Grid
 */
/**
 * The Grid component is a very powerful and performant UI component that shows tabular data (or tree data using the
 * {@link Grid.view.TreeGrid}).
 *
 * <h2>Intro</h2>
 * The Grid widget has a wide range of features and a large API to allow users to work with data efficiently in the
 * browser. The two most important configs are {@link #config-store} and {@link #config-columns}. With the store config,
 * you decide which data to load into the grid. You can work with both in-memory arrays or load data using ajax. See the
 * {@link Core.data.Store} class to learn more about loading data into stores.
 *
 * The columns config accepts an array of {@link Grid.column.Column Column} descriptors defining which fields that will
 * be displayed in the grid. The {@link Grid.column.Column#config-field} property in the column descriptor maps to a
 * field in your dataset. The simplest grid configured with inline data and two columns would look like this:
 *
 * {@frameworktabs}
 * {@js}
 *
 *  ```javascript
 *  const grid = new Grid({
 *       appendTo : document.body,
 *
 *       columns: [
 *           { field: 'name', text: 'Name' },
 *           { field: 'job', text: 'Job', renderer: ({value}) => value || 'Unemployed' }
 *       ],
 *
 *       data: [
 *           { name: 'Bill', job: 'Retired' },
 *           { name: 'Elon', job: 'Visionary' },
 *           { name: 'Me' }
 *       ]
 * });
 * ```
 *
 * {@endjs}
 * {@react}
 *
 * ```jsx
 * const App = props => {
 *     const [columns, setColumns] = useState([
 *          { field: 'name', text: 'Name' },
 *          { field: 'job', text: 'Job', renderer: ({value}) => value || 'Unemployed' }
 *     ]);
 *
 *     const [data, setData] = useState([
 *          { name: 'Bill', job: 'Retired' },
 *          { name: 'Elon', job: 'Visionary' },
 *          { name: 'Me' }
 *     ]);
 *
 *     return <BryntumGrid column={columns} data={data} />
 * }
 * ```
 *
 * {@endreact}
 * {@vue}
 *
 *  ```html
 * <bryntum-grid :columns="columns" :data="data" />
 * ```
 *
 * ```javascript
 * export default {
 *    setup() {
 *      return {
 *        columns : [
 *          { field: 'name', text: 'Name' },
 *          { field: 'job', text: 'Job', renderer: ({value}) => value || 'Unemployed' }
 *        ]
 *        data : reactive([
 *          { name: 'Bill', job: 'Retired' },
 *          { name: 'Elon', job: 'Visionary' },
 *          { name: 'Me' }
 *        ])
 *      };
 *    }
 * }
 * ```
 *
 * {@endvue}
 * {@angular}
 * ```html
 * <bryntum-grid [columns]="columns" [data]="data"></bryntum-grid>
 * ```
 *
 * ```typescript
 * export class AppComponent {
 *      columns = [
 *          { field: 'name', text: 'Name' },
 *          { field: 'job', text: 'Job', renderer: ({value}) => value || 'Unemployed' }
 *      ]
 *
 *      data = [
 *          { name: 'Bill', job: 'Retired' },
 *          { name: 'Elon', job: 'Visionary' },
 *          { name: 'Me' }
 *      ]
 *  }
 * ```
 *
 * {@endangular}
 * {@endframeworktabs}
 *
 * {@inlineexample Grid/view/Grid.js}
 *
 * <h2>Features</h2>
 * To avoid the Grid core being bloated, its main features are implemented in separate `feature` classes. These can be
 * turned on and off based on your requirements. To configure (or disable) a feature, use the {@link #config-features}
 * object to provide your desired configuration for the features you want to use. Each feature has an ´id´ that you use
 * as a key in the features object:
 *
 * ```javascript
 * const grid = new Grid({
 *     features : {
 *         cellEdit     : false,
 *         regionResize : true,
 *         cellTooltip  : {
 *             tooltipRenderer : (data) => {
 *             }
 *         },
 *         ...
 *     }
 * });
 * ```
 *
 * {@region Column configuration options}
 * A grid contains a number of columns that control how your data is rendered. The simplest option is to simply point a
 * Column to a field in your dataset, or define a custom {@link Grid.column.Column#config-renderer}. The renderer
 * function receives one object parameter containing rendering data for the current cell being rendered.
 *
 * ```javascript
 * const grid = new Grid({
 *     columns: [
 *         {
 *             field: 'task',
 *             text: 'Task',
 *             renderer(renderData) {
 *                 const record = renderData.record;
 *
 *                 if (record.percentDone === 100) {
 *                     renderData.cellElement.classList.add('taskDone');
 *                     renderData.cellElement.style.background = 'green';
 *                 }
 *
 *                 return renderData.value;
 *             }
 *         }
 *     ]
 * });
 * ```
 *
 * {@endregion}
 * {@region Grid sections (aka "locked" or "frozen" columns)}
 * The grid can be divided horizontally into individually scrollable sections. This is great if you have lots of columns
 * that don't fit the available width of the screen. To enable this feature, simply mark the columns you want to `lock`.
 * Locked columns are then displayed in their own section to the left of the other columns:
 *
 * ```javascript
 * const grid = new Grid({
 *     width    : 500,
 *     subGridConfigs : {
 *         // set a fixed locked section width if desired
 *         locked : { width: 300 }
 *     },
 *     columns : [
 *         { field : 'name', text : 'Name', width : 200, locked : true },
 *         { field : 'firstName', text : 'First name', width : 100, locked : true },
 *         { field : 'surName', text : 'Last name', width : 100, locked : true },
 *         { field : 'city', text : 'City', width : 100 },
 *         { type : 'number', field : 'age', text : 'Age', width : 200 },
 *         { field : 'food', text : 'Food', width : 200 }
 *     ]
 * });
 * ```
 *
 * {@inlineexample Grid/view/LockedGrid.js}
 * You can also move columns between sections by using drag and drop, or use the built-in header context menu. If you
 * want to be able to resize the locked grid section, enable the {@link Grid.feature.RegionResize} feature.
 * {@endregion}
 * {@region Filtering}
 * One important requirement of a good Grid component is the ability to filter large datasets to quickly find what you
 * are looking for. To enable filtering (through the context menu), add the {@link Grid.feature.Filter} feature:
 *
 * ```javascript
 * const grid = new Grid({
 *     features: {
 *         filter: true
 *     }
 * });
 * ```
 *
 * Or activate a default filter at initial rendering:
 *
 * ```javascript
 * const grid = new Grid({
 *     features: {
 *         filter: { property : 'city', value : 'New York' }
 *     }
 * });
 * ```
 *
 * {@inlineexample Grid/feature/Filter.js}
 * {@endregion}
 * {@region Tooltips}
 * If you have a data models with many fields, and you want to show
 * additional data when hovering over a cell, use the {@link Grid.feature.CellTooltip} feature. To show a
 * tooltip for all cells:
 *
 * ```javascript
 * const grid = new Grid({
 *     features: {
 *         cellTooltip: ({value}) => value
 *     }
 * });
 * ```
 *
 * {@inlineexample Grid/feature/CellTooltip.js}
 * {@endregion}
 * {@region Inline Editing (default <strong>on</strong>)}
 * To enable inline cell editing in the grid, simply add the {@link Grid.feature.CellEdit} feature:
 *
 * ```javascript
 * const grid = new Grid({
 *     features : {
 *         cellEdit : true
 *     },
 *     columns: [
 *         {
 *             field: 'task',
 *             text: 'Task'
 *         }
 *     ]
 * });
 * ```
 *
 * {@inlineexample Grid/feature/CellEdit.js}
 * {@endregion}
 * {@region Context Menu}
 * Use {@link Grid.feature.CellMenu} and {@link Grid.feature.HeaderMenu} features if you want your users to be able to
 * interact with the data through the context menu:
 *
 * ```javascript
 * const grid = new Grid({
 *     features : {
 *         headerMenu : {
 *             items : {
 *                 showInfo : {
 *                     text   : 'Show info',
 *                     icon   : 'fa fa-info-circle',
 *                     weight : 200,
 *                     onItem : ({ item }) => console.log(item.text)
 *                 }
 *             }
 *         },
 *         cellMenu :  {
 *             items : {
 *                 showOptions : {
 *                     text   : 'Show options',
 *                     icon   : 'fa fa-gear',
 *                     weight : 200
 *                 }
 *             }
 *         }
 *     }
 * });
 * ```
 *
 * {@inlineexample Grid/feature/CellMenu.js}
 * {@endregion}
 * {@region Grouping}
 * To group rows by a field in your dataset, use the {@link Grid.feature.Group} feature.
 * {@inlineexample Grid/feature/Group.js}
 * {@endregion}
 * {@region Searching}
 * When working with lots of data, a quick alternative to filtering is the {@link Grid.feature.Search} feature.
 * It highlights matching values in the grid as you type.
 * {@inlineexample Grid/feature/Search2.js}
 * {@endregion}
 * {@region Loading and saving data}
 * The grid keeps all its data in a {@link Core.data.Store}, which is essentially an Array of {@link Core.data.Model}
 * items. You define your own Model representing your data entities and use the Model API to get and set values.
 *
 * ```javascript
 * class Person extends Model {}
 *
 * const person = new Person({
 *     name: 'Steve',
 *     age: 38
 * });
 *
 * person.name = 'Linda'; // person object is now `dirty`
 *
 * const store = new Store({
 *     data : [
 *         { name : 'Don', age : 40 }
 *     ]
 * });
 *
 * store.add(person);
 *
 * console.log(store.count()); // === 2
 *
 * store.remove(person); // Remove from store
 * ```
 *
 * When you update a record in a store, it's considered dirty, until you call {@link Core.data.mixin.StoreCRUD#function-commit}
 * on the containing Store. You can also configure your Store to commit automatically (like Google docs). If you use an
 * AjaxStore, it will send changes to your server when commit is called.
 *
 * Any changes you make to the Store or its records are immediately reflected in the Grid, so there is no need to tell
 * it to refresh manually.
 *
 * To create a custom load mask, subscribe to the grid's store events and {@link Core.widget.Widget#config-masked mask}
 * on {@link Core.data.AjaxStore#event-beforeRequest} and unmask on {@link Core.data.AjaxStore#event-afterRequest}. The
 * mask can also be used to display error messages if an {@link Core.data.AjaxStore#event-exception} occurs.
 *
 * ```javascript
 *  const grid = new Grid({
 *      loadMask : null
 *  });
 *
 *  grid.store.on({
 *      beforeRequest() {
 *          grid.masked = {
 *              text : 'Data is loading...'
 *          };
 *      },
 *      afterRequest() {
 *          grid.masked = null;
 *      },
 *      exception({ response }) {
 *          grid.masked.error = response.message || 'Load failed';
 *      }
 *  });
 *
 *  store.load();
 * ```
 *
 * To learn more about loading and saving data, please refer to [this guide](#Grid/guides/data/displayingdata.md).
 * {@endregion}
 * {@region Default configs}
 * There is a myriad of configs and features available for Grid, some of them on by default and some of them requiring
 * extra configuration. The code below tries to illustrate the major things that are used by default:
 *
 * ```javascript
 * const grid = new Grid({
 *    // The following features are enabled by default:
 *    features : {
 *        cellEdit      : true,
 *        columnPicker  : true,
 *        columnReorder : true,
 *        columnResize  : true,
 *        cellMenu      : true,
 *        headerMenu    : true,
 *        group         : true,
 *        rowCopyPaste  : true, // Allow using [Ctrl/CMD + C/X] and [Ctrl/CMD + V] to copy/cut and paste rows
 *        sort          : true
 *    },
 *
 *    animateRemovingRows       : true,  // Rows will slide out on removal
 *    autoHeight                : false, // Grid needs to have a height supplied through CSS (strongly recommended) or by specifying `height`
 *    columnLines               : true,  // Themes might override it to hide lines anyway
 *    emptyText                 : 'No rows to display',
 *    enableTextSelection       : false, // Not allowed to select text in cells by default,
 *    fillLastColumn            : true,  // By default the last column is stretched to fill the grid
 *    fullRowRefresh            : true,  // Refreshes entire row when a cell value changes
 *    loadMask                  : 'Loading...',
 *    resizeToFitIncludesHeader : true,  // Also measure header when auto resizing columns
 *    responsiveLevels : {
 *      small : 400,
 *      medium : 600,
 *      large : '*'
 *    },
 *    rowHeight                  : null,  // Determined using CSS, it will measure rowHeight
 *    showDirty                  : false, // No indicator for changed cells
 * });
 * ```
 *
 * {@endregion}
 * {@region Keyboard shortcuts}
 * Grid has the following default keyboard shortcuts:
 * <div class="compact">
 *
 * | Keys                 | Action                 | Weight ¹ | Action description                                                                                 |
 * |----------------------|------------------------|:--------:|----------------------------------------------------------------------------------------------------|
 * | `ArrowUp`            | *navigateUp*           | 10       | Focuses the cell above currently focused cell.                                                     |
 * | `ArrowRight`         | *navigateRight*        | 10       | Focuses the cell to the right of currently focused cell                                            |
 * | `ArrowDown`          | *navigateDown*         | 10       | Focuses the cell below currently focused cell                                                      |
 * | `ArrowLeft`          | *navigateLeft*         | 10       | Focuses the cell to the left of currently focused cell                                             |
 * | `Shift`+`ArrowUp`    | *extendSelectionUp*    |          | Extends the selection one row up from currently focused cell                                       |
 * | `Shift`+`ArrowRight` | *extendSelectionRight* |          | Extends the selection one column to the right from currently focused cell                          |
 * | `Shift`+`ArrowDown`  | *extendSelectionDown*  |          | Extends the selection one row down from currently focused cell                                     |
 * | `Shift`+`ArrowLeft`  | *extendSelectionLeft*  |          | Extends the selection one column to the left from currently focused cell                           |
 * | `Space`              | *toggleSelection*      | 10       | Toggles selection of currently focused cell if selectionMode.selectOnKeyboardNavigation is `false` |
 * | `Ctrl`+`Home`        | *navigateFirstCell*    |          | Focuses the first cell at the first row (including header)                                         |
 * | `Home`               | *navigateFirstColumn*  |          | Focuses the first cell of current focused row                                                      |
 * | `Ctrl`+`End`         | *navigateLastCell*     |          | Focuses the last cell of the last row                                                              |
 * | `End`                | *navigateLastColumn*   |          | Focuses the last cell of current focused row                                                       |
 * | `PageUp`             | *navigatePrevPage*     |          | Displays previous page                                                                             |
 * | `PageDown`           | *navigateNextPage*     |          | Displays next page                                                                                 |
 * | `Enter`              | *activateHeader*       |          | Equals to a header click                                                                           |
 * | `Space`              | *clickCellByKey*       | 1000     | Equals to a cell click                                                                             |
 * | `Ctrl`+`Z`           | *undoRedoKeyPress*     |          | Undo/redo (when using {@link Core.data.stm.StateTrackingManager})                                  |
 * | `Ctrl`+`Shift`+`Z`   | *undoRedoKeyPress*     |          | Undo/redo (when using {@link Core.data.stm.StateTrackingManager})                                  |
 *
 * **¹** Customization of keyboard shortcuts that has a `weight` can affect other features that also uses that
 * particular keyboard shortcut. Read more in [our guide](#Grid/guides/customization/keymap.md).
 *
 *</div>
 *
 * <div class="note" style="font-size: 0.9em">Please note that <code>Ctrl</code> is the equivalent to <code>Command</code> and <code>Alt</code>
 * is the equivalent to <code>Option</code> for Mac users</div>
 *
 * The following Grid features has their own keyboard shortcuts. Follow the links for details.
 * * {@link Grid.feature.CellCopyPaste#keyboard-shortcuts CellCopyPaste}
 * * {@link Grid.feature.CellEdit#keyboard-shortcuts CellEdit}
 * * {@link Grid.feature.CellMenu#keyboard-shortcuts CellMenu}
 * * {@link Grid.feature.ColumnRename#keyboard-shortcuts ColumnRename}
 * * {@link Grid.feature.Filter#keyboard-shortcuts Filter}
 * * {@link Grid.feature.Group#keyboard-shortcuts Group}
 * * {@link Grid.feature.HeaderMenu#keyboard-shortcuts HeaderMenu}
 * * {@link Grid.feature.QuickFind#keyboard-shortcuts QuickFind}
 * * {@link Grid.feature.RowCopyPaste#keyboard-shortcuts RowCopyPaste}
 * * {@link Grid.feature.Search#keyboard-shortcuts Search}
 * * {@link Grid.feature.Tree#keyboard-shortcuts Tree}
 *
 * For more information on how to customize keyboard shortcuts, please see
 * [our guide](#Grid/guides/customization/keymap.md)
 *
 * {@endregion}
 * {@region Performance}
 * In general the Grid widget has very good performance and you can try loading any amount of data in the
 * <a target="_blank" href="../examples/bigdataset/">bigdataset</a> demo.
 * The overall rendering performance is naturally affected by many other things than the data volume. Other important
 * factors that can impact performance: number of columns, complex cell renderers, locked columns, the number of
 * features enabled and of course the browser (Chrome fastest).
 * {@endregion}
 * {@region Accessibility}
 * As far as possible, the grid is accessible to WAI-ARIA standards. Every cell, including column header cells is
 * visitable. The arrow keys navigate, and if a cell contains focusable content, navigating to that cell focuses the
 * content. `Escape` will exit from that and focus the encapsulating cell.
 *
 * When tabbing back into a grid that has previously been entered, focus moves to the last focused cell.
 *
 * The column menu is invoked using the `Space` key when focused on a column header.
 *
 * The cell menu is invoked using the `Space` key when focused on a data cell.
 * {@endregion}
 *
 * @extends Grid/view/GridBase
 * @classType grid
 * @widget
 */
class Grid extends GridBase {
  static get $name() {
    return 'Grid';
  }
  // Factoryable type name
  static get type() {
    return 'grid';
  }
}
// Register this widget type with its Factory
Grid.initClass();
Grid._$name = 'Grid';

export { ColumnAutoWidth, Grid, RowCopyPaste };
//# sourceMappingURL=Grid.js.map
