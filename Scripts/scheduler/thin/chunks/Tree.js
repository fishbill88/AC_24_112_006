/*!
 *
 * Bryntum Scheduler Pro 5.3.7
 *
 * Copyright(c) 2023 Bryntum AB
 * https://bryntum.com/contact
 * https://bryntum.com/license
 *
 */
import { DomHelper, InstancePlugin, Delayable } from './Editor.js';
import { ColumnStore, Column, GridFeatureManager } from './GridBase.js';

/**
 * @module Grid/column/TreeColumn
 */
let currentParentHasIcon = false;
/**
 * A column that displays a tree structure when using the {@link Grid.feature.Tree tree} feature.
 *
 * Default editor is a {@link Core.widget.TextField TextField}.
 *
 * TreeColumn provides configs to define icons for {@link #config-expandIconCls expanded} /
 * {@link #config-collapseIconCls collapsed} nodes, {@link #config-expandedFolderIconCls expanded folder} /
 * {@link #config-collapsedFolderIconCls collapsed folder} nodes and {@link #config-leafIconCls leaf} nodes.
 *
 * When the TreeColumn renders its cells, it will look for two special fields {@link Grid.data.GridRowModel#field-href}
 * and {@link Grid.data.GridRowModel#field-target}. Specifying `href` will produce a link for the TreeNode,
 * and `target` will have the same meaning as in an A tag:
 *
 * ```javascript
 * {
 *    id        : 1,
 *    name      : 'Some external link'
 *    href      : '//www.website.com",
 *    target    : '_blank"
 * }
 * ```
 *
 * ## Snippet
 * ```javascript
 * new TreeGrid({
 *     appendTo : document.body,
 *
 *     columns : [
 *          { type: 'tree', field: 'name' }
 *     ]
 * });
 * ```
 *
 * {@inlineexample Grid/column/TreeColumn.js}
 *
 * ## Cell renderers
 *
 * You can affect the contents and styling of cells in this column using a
 * {@link Grid.column.TreeColumn#config-renderer} function.
 *
 * ```javascript
 * const grid = new Grid({
 *   columns : [{
 *       type       : 'tree',
 *       field      : 'name',
 *       text       : 'Name',
 *       renderer({ value, record }) {
 *         return `${value} (${record.childLevel})`
 *       }
 *     }]
 * });
 * ```
 *
 * @classType tree
 * @extends Grid/column/Column
 * @column
 */
class TreeColumn extends Column {
  static $name = 'TreeColumn';
  static type = 'tree';
  static get defaults() {
    return {
      tree: true,
      hideable: false,
      minWidth: 150
    };
  }
  static get fields() {
    return [
    /**
     * The icon to use for the collapse icon in collapsed state
     * @config {String|null} expandIconCls
     */
    {
      name: 'expandIconCls',
      defaultValue: 'b-icon b-icon-tree-expand'
    },
    /**
     * The icon to use for the collapse icon in expanded state
     * @config {String|null} collapseIconCls
     */
    {
      name: 'collapseIconCls',
      defaultValue: 'b-icon b-icon-tree-collapse'
    },
    /**
     * The icon to use for the collapse icon in expanded state
     * @config {String|null} collapsedFolderIconCls
     */
    {
      name: 'collapsedFolderIconCls'
    },
    /**
     * The icon to use for the collapse icon in expanded state
     * @config {String|null} expandedFolderIconCls
     */
    {
      name: 'expandedFolderIconCls'
    },
    /**
     * Size of the child indent in em. Resulting indent is indentSize multiplied by child level.
     * @config {Number} indentSize
     * @default 1.7
     */
    {
      name: 'indentSize',
      defaultValue: 1.7
    },
    /**
     * The icon to use for the leaf nodes in the tree
     * @config {String|null} leafIconCls
     */
    {
      name: 'leafIconCls',
      defaultValue: 'b-icon b-icon-tree-leaf'
    }, {
      name: 'editTargetSelector',
      defaultValue: '.b-tree-cell-value'
    },
    /**
     * Renderer function, used to format and style the content displayed in the cell. Return the cell text you
     * want to display. Can also affect other aspects of the cell, such as styling.
     *
     * <div class="note">
     * As the TreeColumn adds its own cell content to the column, there is a limit to what is supported in the
     * renderer function in comparison with an ordinary
     * {@link Grid.column.Column#config-renderer Column renderer}. Most notably is that changing `cellElement`
     * content can yield unexpected results as it will be updated later in the rendering process.
     * </div>
     *
     * You can also return a {@link Core.helper.DomHelper#typedef-DomConfig} object describing the markup
     * ```javascript
     * new Grid({
     *     columns : [
     *         {
     *              type  : 'tree',
     *              field : 'name'
     *              text  : 'Name',
     *              renderer : ({ record }) => {
     *                  return {
     *                      class : 'myClass',
     *                      children : [
     *                          {
     *                              tag : 'i',
     *                              class : 'fa fa-pen'
     *                          },
     *                          {
     *                              tag : 'span',
     *                              html : record.name
     *                          }
     *                      ]
     *                  };
     *              }
     *         }
     *     ]
     * });
     * ```
     *
     * You can modify the row element too from inside a renderer to add custom CSS classes:
     *
     * ```javascript
     * new Grid({
     *     columns : [
     *         {
     *             type     : 'tree',
     *             field    : 'name',
     *             text     : 'Name',
     *             renderer : ({ record, row }) => {
     *                // Add special CSS class to new rows that have not yet been saved
     *               row.cls.newRow = record.isPhantom;
     *
     *               return record.name;
     *         }
     *     ]
     * });
     * ```
     *
     * @param {Object} renderData Object containing renderer parameters
     * @param {HTMLElement} [renderData.cellElement] Cell element, for adding CSS classes, styling etc.
     * Can be `null` in case of export
     * @param {*} renderData.value Value to be displayed in the cell
     * @param {Core.data.Model} renderData.record Record for the row
     * @param {Grid.column.Column} renderData.column This column
     * @param {Grid.view.Grid} renderData.grid This grid
     * @param {Grid.row.Row} [renderData.row] Row object. Can be null in case of export. Use the
     * {@link Grid.row.Row#function-assignCls row's API} to manipulate CSS class names.
     * @param {Object} [renderData.size] Set `size.height` to specify the desired row height for the current
     * row. Largest specified height is used, falling back to configured {@link Grid/view/Grid#config-rowHeight}
     * in case none is specified. Can be null in case of export
     * @param {Number} [renderData.size.height] Set this to request a certain row height
     * @param {Number} [renderData.size.configuredHeight] Row height that will be used if none is requested
     * @param {Boolean} [renderData.isExport] True if record is being exported to allow special handling during
     * export.
     * @param {Boolean} [renderData.isMeasuring] True if the column is being measured for a `resizeToFitContent`
     * call. In which case an advanced renderer might need to take different actions.
     * @config {Function} renderer
     * @category Common
     */
    'renderer'];
  }
  constructor(config, store) {
    super(...arguments);
    const me = this;
    // We handle htmlEncoding in this class rather than relying on the generic Row DOM manipulation
    // since this class requires quite a lot of DOM infrastructure around the actual rendered content
    me.shouldHtmlEncode = me.htmlEncode;
    me.setData('htmlEncode', false);
    // add tree renderer (which calls original renderer internally)
    if (me.renderer) {
      me.originalRenderer = me.renderer;
    }
    me.renderer = me.treeRenderer.bind(me);
  }
  /**
   * A column renderer that is automatically added to the column with { tree: true }. It adds padding and node icons
   * to the cell to make the grid appear to be a tree. The original renderer is called in the process.
   * @private
   */
  treeRenderer(renderData) {
    const me = this,
      {
        grid,
        column,
        cellElement,
        row,
        record,
        isExport
      } = renderData,
      gridMeta = record.instanceMeta(grid.store),
      isCollapsed = !record.isLeaf && gridMeta.collapsed,
      innerConfig = {
        className: 'b-tree-cell-value'
      },
      children = [innerConfig],
      result = {
        className: {
          'b-tree-cell-inner': 1
        },
        tag: record.href ? 'a' : 'div',
        href: record.href,
        target: record.target,
        children
      },
      rowClasses = {
        'b-tree-parent-row': 0,
        'b-tree-collapsed': 0,
        'b-tree-expanded': 0,
        'b-loading-children': 0
      };
    let outputIsObject,
      iconCls,
      {
        value
      } = renderData;
    if (me.originalRenderer) {
      var _grid$hasFrameworkRen;
      const rendererHtml = me.originalRenderer(renderData),
        // Check if the cell content is going to be rendered by framework
        hasFrameworkRenderer = (_grid$hasFrameworkRen = grid.hasFrameworkRenderer) === null || _grid$hasFrameworkRen === void 0 ? void 0 : _grid$hasFrameworkRen.call(grid, {
          cellContent: rendererHtml,
          column
        });
      outputIsObject = typeof rendererHtml === 'object' && !hasFrameworkRenderer;
      // Reset the value when framework is responsible for the cell content
      value = hasFrameworkRenderer ? '' : rendererHtml === false ? cellElement.innerHTML : rendererHtml;
      // Save content to the `rendererHtml` to be used in processCellContent implemented by framework
      renderData.rendererHtml = rendererHtml;
    }
    if (!outputIsObject) {
      value = String(value ?? '');
    }
    if (isExport) {
      return value;
    }
    if (!record.isLeaf) {
      var _record$children;
      const isCollapsed = !record.isExpanded(grid.store),
        expanderIconCls = isCollapsed ? me.expandIconCls : me.collapseIconCls,
        folderIconCls = isCollapsed ? me.collapsedFolderIconCls : me.expandedFolderIconCls;
      rowClasses['b-tree-parent-row'] = 1;
      rowClasses['b-tree-collapsed'] = isCollapsed;
      rowClasses['b-tree-expanded'] = !isCollapsed;
      rowClasses['b-loading-children'] = gridMeta.isLoadingChildren;
      cellElement.classList.add('b-tree-parent-cell');
      children.unshift({
        tag: 'i',
        className: {
          'b-tree-expander': 1,
          [expanderIconCls]: 1,
          'b-empty-parent': !gridMeta.isLoadingChildren && record.children !== true && !((_record$children = record.children) !== null && _record$children !== void 0 && _record$children.length)
        }
      });
      // Allow user to customize tree icon or opt out entirely
      currentParentHasIcon = iconCls = renderData.iconCls || record.iconCls || folderIconCls;
    } else {
      cellElement.classList.add('b-tree-leaf-cell');
      // Allow user to customize tree icon or opt out entirely
      iconCls = renderData.iconCls || record.iconCls || me.leafIconCls;
    }
    if (iconCls) {
      children.splice(children.length - 1, 0, {
        tag: 'i',
        className: {
          'b-tree-icon': 1,
          [iconCls]: 1
        }
      });
    }
    // Row can be just a dummy object for example when the renderer is called from Column#resizeToFitContent.
    // Add/remove the various tree node classes.
    // Keep row's aria state up to date
    if (row.isRow) {
      row.assignCls(rowClasses);
      if (!record.isLeaf) {
        row.setAttribute('aria-expanded', !isCollapsed);
        if (isCollapsed) {
          row.removeAttribute('aria-owns');
        } else {
          for (const region in grid.subGrids) {
            var _record$children2, _record$children3;
            const el = row.elements[region];
            // A branch node may be configured expanded, but yet have no children.
            // They may be added dynamically.
            DomHelper.setAttributes(el, {
              'aria-owns': (_record$children2 = record.children) !== null && _record$children2 !== void 0 && _record$children2.length ? (_record$children3 = record.children) === null || _record$children3 === void 0 ? void 0 : _record$children3.map(r => `${grid.id}-${region}-${r.id}`).join(' ') : null
            });
          }
        }
      }
    }
    // Array of DomConfigs
    if (Array.isArray(value)) {
      innerConfig.children = value;
    }
    // Single DomConfig
    else if (outputIsObject) {
      Object.assign(innerConfig, value);
    }
    // If we are encoding HTML, or there's no raw HTML, we can use the text property
    // as the raw value, and DomSync will create a TextNode from that.
    else if (me.shouldHtmlEncode || !value.includes('<')) {
      result.className['b-text-value'] = 1;
      innerConfig.text = value;
    }
    // If we are accepting HTML without encoding it, and there is HTML we must use html property
    else {
      innerConfig.html = value;
    }
    const padding = record.childLevel * me.indentSize + (record.isLeaf ? currentParentHasIcon ? 2.0 : iconCls ? 0.5 : 0.4 : 0);
    result.style = `padding-inline-start:${padding}em`;
    return result;
  }
  // This function is not meant to be called by any code other than Base#getCurrentConfig().
  // It extracts the current configs (fields) for the column, with special handling for the renderer
  getCurrentConfig(options) {
    const result = super.getCurrentConfig(options);
    // Use app renderer
    result.renderer = this.originalRenderer;
    return result;
  }
}
ColumnStore.registerColumnType(TreeColumn, true);
TreeColumn.exposeProperties();
TreeColumn._$name = 'TreeColumn';

/**
 * @module Grid/feature/Tree
 */
const immediatePromise = Promise.resolve();
/**
 * Feature that makes the grid work more like a tree. Included by default in {@link Grid.view.TreeGrid}. Requires
 * exactly one {@link Grid.column.TreeColumn} among grids columns. That column will have its renderer replaced with a
 * tree renderer that adds padding and icon to give the appearance of a tree. The original renderer is preserved and
 * also called.
 *
 * {@inlineexample Grid/feature/Tree.js}
 *
 * This feature is <strong>disabled</strong> by default. When enabled, the feature cannot be disabled during runtime.
 *
 * ## Keyboard shortcuts
 * This feature has the following default keyboard shortcuts:
 *
 * | Keys                 | Action                   | Action description                                                   |
 * |----------------------|--------------------------|----------------------------------------------------------------------|
 * | `Space`              | *toggleCollapseByKey*    | When focus on a parent node, this expands or collapses it's children |
 * | `ArrowRight`         | *expandIfSingleColumn*   | Expands a focused parent node if grid consist of one column only     |
 * | `Shift`+`ArrowRight` | *expandByKey*            | Expands a focused parent node                                        |
 * | `ArrowLeft`          | *collapseIfSingleColumn* | Collapses a focused parent node if grid consist of one column only   |
 * | `Shift`+`ArrowLeft`  | *collapseByKey*          | Collapses a focused parent node                                      |
 *
 * <div class="note">Please note that <code>Ctrl</code> is the equivalent to <code>Command</code> and <code>Alt</code>
 * is the equivalent to <code>Option</code> for Mac users</div>
 *
 * For more information on how to customize keyboard shortcuts, please see
 * [our guide](#Grid/guides/customization/keymap.md)
 *
 * @extends Core/mixin/InstancePlugin
 *
 * @demo Grid/tree
 * @classtype tree
 * @feature
 */
class Tree extends InstancePlugin.mixin(Delayable) {
  //region Config
  static $name = 'Tree';
  static configurable = {
    /**
     * Expand parent nodes when clicking on their cell
     * @prp {Boolean}
     * @default
     */
    expandOnCellClick: false,
    /**
     * See {@link #keyboard-shortcuts Keyboard shortcuts} for details
     * @config {Object<String,String>}
     */
    keyMap: {
      ' ': 'toggleCollapseByKey',
      ArrowRight: 'expandIfSingleColumn',
      'Shift+ArrowRight': 'expandByKey',
      ArrowLeft: 'collapseIfSingleColumn',
      'Shift+ArrowLeft': 'collapseByKey'
    }
  };
  // Plugin configuration. This plugin chains some functions in Grid.
  static get pluginConfig() {
    return {
      assign: ['collapseAll', 'expandAll', 'collapse', 'expand', 'expandTo', 'toggleCollapse'],
      chain: ['onElementPointerUp', 'onElementClick', 'bindStore']
    };
  }
  //endregion
  //region Init
  construct(client, config) {
    super.construct(client, config);
    // find column
    if (!this.treeColumn) {
      console.info('To use the tree feature, one column should be configured with `type: \'tree\'`');
    }
    client.store && this.bindStore(client.store);
  }
  doDisable(disable) {
    if (disable) {
      throw new Error('Tree feature cannot be disabled');
    }
  }
  get store() {
    return this.client.store;
  }
  get treeColumn() {
    const me = this,
      {
        columns
      } = me.client;
    if (!me._treeColumn || !columns.includes(me._treeColumn)) {
      me._treeColumn = columns.find(column => column.isTreeColumn);
    }
    return me._treeColumn;
  }
  bindStore(store) {
    this.detachListeners('store');
    store.ion({
      name: 'store',
      beforeLoadChildren: 'onBeforeLoadChildren',
      loadChildren: 'onLoadChildren',
      loadChildrenException: 'onLoadChildrenException',
      beforeToggleNode: 'onBeforeToggleNode',
      thisObj: this
    });
  }
  //endregion
  //region Expand & collapse
  /**
   * Collapse an expanded node or expand a collapsed. Optionally forcing a certain state.
   * This function is exposed on Grid and can thus be called as `grid.toggleCollapse()`
   * @param {String|Number|Core.data.Model} idOrRecord Record (the node itself) or id of a node to toggle
   * @param {Boolean} [collapse] Force collapse (true) or expand (false)
   * @on-owner
   */
  async toggleCollapse(idOrRecord, collapse) {
    if (idOrRecord == null) {
      throw new Error('Tree#toggleCollapse must be passed a record');
    }
    const me = this,
      {
        store,
        client
      } = me,
      {
        rowManager
      } = client,
      record = store.getById(idOrRecord),
      meta = record.instanceMeta(store);
    if (await store.toggleCollapse(record, collapse)) {
      const row = rowManager.getRowFor(record);
      if (row && record.ancestorsExpanded()) {
        const cellElement = me.treeColumn && !me.treeColumn.subGrid.collapsed && row.getCell(me.treeColumn.id);
        // Toggle cell's expanded/collapsed state
        cellElement && row.renderCell(cellElement);
      }
      // Add a temporary cls, used by Scheduler & Gantt to prevent transitions on events/tasks
      // Block multiple applications in the case of a recursive collapseAll operation
      if (!me.isTogglingNode) {
        client.element.classList.add('b-toggling-node');
        me.isTogglingNode = true;
        me.requestAnimationFrame(() => {
          client.element.classList.remove('b-toggling-node');
          me.isTogglingNode = false;
        });
      }
      /**
       * Fired before a parent node record is collapsed.
       * @event collapseNode
       * @param {Grid.view.Grid} source The firing Grid instance.
       * @param {Core.data.Model} record The record which has been collapsed.
       * @on-owner
       */
      /**
       * Fired after a parent node record is expanded.
       * @event expandNode
       * @param {Grid.view.Grid} source The firing Grid instance.
       * @param {Core.data.Model} record The record which has been expanded.
       * @on-owner
       */
      client.trigger(meta.collapsed ? 'collapseNode' : 'expandNode', {
        record
      });
      /**
       * Fired after a parent node record toggles its collapsed state.
       * @event toggleNode
       * @param {Core.data.Model} record The record being toggled.
       * @param {Boolean} collapse `true` if the node is being collapsed.
       * @on-owner
       */
      client.trigger('toggleNode', {
        record,
        collapse: meta.collapsed
      });
    }
  }
  /**
   * Collapse a single node.
   * This function is exposed on Grid and can thus be called as `grid.collapse()`
   * @param {String|Number|Core.data.Model} idOrRecord Record (the node itself) or id of a node to collapse
   * @on-owner
   */
  async collapse(idOrRecord) {
    return this.toggleCollapse(idOrRecord, true);
  }
  /**
   * Expand a single node.
   * This function is exposed on Grid and can thus be called as `grid.expand()`
   * @param {String|Number|Core.data.Model} idOrRecord Record (the node itself) or id of a node to expand
   * @on-owner
   */
  async expand(idOrRecord) {
    return this.toggleCollapse(idOrRecord, false);
  }
  onBeforeToggleNode({
    record,
    collapse
  }) {
    /**
     * Fired before a parent node record toggles its collapsed state.
     * @event beforeToggleNode
     * @param {Grid.view.Grid} source The firing Grid instance.
     * @param {Core.data.Model} record The record being toggled.
     * @param {Boolean} collapse `true` if the node is being collapsed.
     * @on-owner
     */
    this.client.trigger('beforeToggleNode', {
      record,
      collapse
    });
  }
  onBeforeLoadChildren({
    source: store,
    params
  }) {
    const parent = store.getById(params[store.modelClass.idField]),
      row = this.client.rowManager.getRowFor(parent);
    row === null || row === void 0 ? void 0 : row.addCls('b-loading-children');
  }
  onLoadChildren({
    source: store,
    params
  }) {
    const parent = store.getById(params[store.modelClass.idField]),
      row = this.client.rowManager.getRowFor(parent);
    row === null || row === void 0 ? void 0 : row.removeCls('b-loading-children');
  }
  onLoadChildrenException({
    record
  }) {
    const row = this.client.rowManager.getRowFor(record);
    row === null || row === void 0 ? void 0 : row.removeCls('b-loading-children');
  }
  /**
   * Expand or collapse all nodes, as specified by param, starting at the passed node (which defaults to the root node)
   * @param {Boolean} [collapse] Set to true to collapse, false to expand (defaults to true)
   * @param {Core.data.Model} [topNode] The topmost node from which to cascade a collapse.
   * Defaults to the {@link Core.data.Store#property-rootNode}. Not included in the cascade if
   * the root node is being used.
   */
  async expandOrCollapseAll(collapse = true, topNode = this.store.rootNode) {
    const {
        client,
        store
      } = this,
      promises = [],
      childRecords = [];
    client.trigger('beforeToggleAllNodes', {
      collapse
    });
    // Each collapse/expand will trigger events on store, avoid that by suspending
    store.suspendEvents();
    store.traverse(record => {
      const gridMeta = record.instanceMeta(store);
      if (!record.isLeaf) {
        if (collapse && !gridMeta.collapsed) {
          this.toggleCollapse(record, true);
          childRecords.push(...record.children);
        } else if (!collapse && gridMeta.collapsed) {
          if (Array.isArray(record.children)) {
            childRecords.push(...record.children);
          }
          promises.push(this.toggleCollapse(record, false));
        }
      }
    }, topNode, topNode === store.rootNode);
    store.resumeEvents();
    return (collapse ? immediatePromise : Promise.all(promises)).then(() => {
      // Return to top when collapsing all
      client.refreshRows(collapse);
      if (childRecords.length) {
        if (collapse) {
          store.trigger('remove', {
            records: childRecords,
            isCollapse: true,
            isCollapseAll: true
          });
        } else {
          store.trigger('add', {
            records: childRecords,
            isExpand: true,
            isExpandAll: true
          });
        }
      }
      client.trigger('toggleAllNodes', {
        collapse
      });
    });
  }
  /**
   * Collapse all nodes.
   * This function is exposed on Grid and can thus be called as `grid.collapseAll()`
   * @on-owner
   */
  async collapseAll() {
    return this.expandOrCollapseAll(true);
  }
  /**
   * Expand all nodes.
   * This function is exposed on Grid and can thus be called as `grid.expandAll()`
   * @on-owner
   */
  async expandAll() {
    return this.expandOrCollapseAll(false);
  }
  /**
   * Expands parent nodes to make this node "visible".
   * This function is exposed on Grid and can thus be called as `grid.expandTo()`
   * @param {String|Number|Core.data.Model|String[]|Number[]|Core.data.Model[]} idOrRecord Record (the node itself),
   * or id of a node. Also accepts arrays of the same types.
   * @param {Boolean} [scrollIntoView=true] A flag letting you control whether to scroll the record into view
   * @on-owner
   * @async
   */
  async expandTo(idOrRecord, scrollIntoView = true) {
    const me = this,
      {
        store,
        client
      } = me;
    if (Array.isArray(idOrRecord)) {
      if (idOrRecord.length > 0) {
        client.suspendRefresh();
        for (let i = idOrRecord.length - 1; i >= 0; i--) {
          var _me$expandTo;
          const record = store.getById(idOrRecord[i]);
          if (i === 0) {
            var _client$resumeRefresh;
            (_client$resumeRefresh = client.resumeRefresh) === null || _client$resumeRefresh === void 0 ? void 0 : _client$resumeRefresh.call(client);
            // Ensure all parents are rendered
            client.rowManager.refresh();
          }
          await ((_me$expandTo = me.expandTo) === null || _me$expandTo === void 0 ? void 0 : _me$expandTo.call(me, record, i === 0));
        }
      }
      return;
    }
    const record = store.getById(idOrRecord);
    // Hidden because it's in a collapsed Group: abort
    if (record.instanceMeta(me.store).hiddenByCollapse === false) {
      return;
    }
    // Expand any parents that need to be expanded to allow the record to be rendered.
    if (!record.ancestorsExpanded()) {
      var _client$resumeRefresh2, _client$refreshRows;
      const parents = [];
      // Collect parents which need expanding
      for (let parent = record.parent; parent && !parent.isRoot; parent = parent.parent) {
        if (!parent.isExpanded(store)) {
          parents.unshift(parent);
        }
      }
      client.suspendRefresh();
      // Expand them from the top-down
      for (const parent of parents) {
        if (!me.isDestroyed) {
          await me.toggleCollapse(parent, false);
        }
      }
      (_client$resumeRefresh2 = client.resumeRefresh) === null || _client$resumeRefresh2 === void 0 ? void 0 : _client$resumeRefresh2.call(client);
      // Refreshing on expand was inhibited in toggleCollapse calls
      (_client$refreshRows = client.refreshRows) === null || _client$refreshRows === void 0 ? void 0 : _client$refreshRows.call(client);
    }
    if (!me.isDestroyed && scrollIntoView) {
      await client.scrollRowIntoView(record);
    }
  }
  //endregion
  //region Events
  /**
   * Called when user clicks somewhere in the grid. Expand/collapse node on icon click.
   * @private
   */
  onElementPointerUp(event) {
    const me = this,
      target = event.target,
      cellData = me.client.getCellDataFromEvent(event),
      clickedExpander = target.closest('.b-tree-expander');
    // Checks if click is on node expander icon, then toggles expand/collapse. Also toggles on entire cell if expandOnCellClick is true
    if (clickedExpander || me.expandOnCellClick && cellData !== null && cellData !== void 0 && cellData.record.isParent) {
      me.toggleCollapse(cellData.record);
    }
  }
  onElementClick(event) {
    // Prevent default to avoid triggering navigation if the tree node is a link
    if (event.target.closest('.b-tree-expander')) {
      event.preventDefault();
    }
  }
  /**
   * Called on key down in grid. Expand/collapse node on [space]
   * @private
   */
  toggleCollapseByKey() {
    const {
      focusedCell
    } = this.client;
    // Only catch space on grid cell element, not in header, editors etc...
    if ((focusedCell === null || focusedCell === void 0 ? void 0 : focusedCell.rowIndex) > -1 && !focusedCell.isActionable) {
      this.toggleCollapse(focusedCell.id);
      return true;
    }
    return false;
  }
  //endregion
  // Expands tree if single column.
  // Called by default on ArrowRight
  expandIfSingleColumn() {
    if (this.client.columns.count === 1) {
      return this.expandByKey();
    }
    // Tells keymap to continue with other actions
    return false;
  }
  // Expands tree on Shift+ArrowRight by default.
  expandByKey() {
    const me = this,
      {
        client
      } = me,
      {
        focusedCell
      } = client,
      record = focusedCell === null || focusedCell === void 0 ? void 0 : focusedCell.record;
    // shift triggers tree navigation behaviour, also used by default for single column which is tree
    if (record && focusedCell !== null && focusedCell !== void 0 && focusedCell.column.tree && record.isParent && record.instanceMeta(client.store).collapsed) {
      me.expand(record);
      return true;
    }
    // Tells keymap to continue with other actions
    return false;
  }
  collapseIfSingleColumn() {
    if (this.client.columns.count === 1) {
      return this.collapseByKey();
    }
    // Tells keymap to continue with other actions
    return false;
  }
  collapseByKey() {
    const me = this,
      {
        client
      } = me,
      {
        focusedCell
      } = client,
      record = focusedCell === null || focusedCell === void 0 ? void 0 : focusedCell.record;
    // shift triggers tree navigation behaviour, also used by default for single column which is tree
    if (focusedCell !== null && focusedCell !== void 0 && focusedCell.column.tree && record) {
      // on expanded parent, collapse
      if (record.isParent && !record.instanceMeta(client.store).collapsed) {
        me.collapse(record);
        return true;
      }
      // otherwise go to parent
      if (record.parent && !record.parent.isRoot) {
        // Deselect everything before doing this.
        // Causes strange selection ranges otherwise
        client.deselectAll();
        client.focusCell({
          record: record.parent,
          column: focusedCell.column
        });
        return true;
      }
    }
    // Tells keymap to continue with other actions
    return false;
  }
}
Tree.featureClass = 'b-tree';
Tree._$name = 'Tree';
GridFeatureManager.registerFeature(Tree, false, 'Grid');
GridFeatureManager.registerFeature(Tree, true, 'TreeGrid');

export { Tree, TreeColumn };
//# sourceMappingURL=Tree.js.map
