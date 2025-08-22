/*!
 *
 * Bryntum Scheduler Pro 5.3.7
 *
 * Copyright(c) 2023 Bryntum AB
 * https://bryntum.com/contact
 * https://bryntum.com/license
 *
 */
import { ColumnStore, Column, GridFeatureManager } from './GridBase.js';
import { NumberFormat } from './MessageDialog.js';
import { ObjectHelper, InstancePlugin, EventHelper } from './Editor.js';

/**
 * @module Grid/column/NumberColumn
 */
/**
 * A column for showing/editing numbers.
 *
 * Default editor is a {@link Core.widget.NumberField NumberField}.
 *
 * ```javascript
 * new Grid({
 *     appendTo : document.body,
 *     columns : [
 *         { type: 'number', min: 0, max : 100, field: 'score' }
 *     ]
 * });
 * ```
 *
 * Provide a {@link Core/helper/util/NumberFormat} config as {@link #config-format} to be able to show currency. For
 * example:
 * ```javascript
 * new Grid({
 *     appendTo : document.body,
 *     columns : [
 *         {
 *             type   : 'number',
 *             format : {
 *                style    : 'currency'
 *                currency : 'USD',
 *             }
 *         }
 *     ]
 * });
 * ```
 *
 * @extends Grid/column/Column
 * @classType number
 * @inlineexample Grid/column/NumberColumn.js
 * @column
 */
class NumberColumn extends Column {
  //region Config
  static type = 'number';
  // Type to use when auto adding field
  static fieldType = 'number';
  static fields = ['format',
  /**
   * The minimum value for the field used during editing.
   * @config {Number} min
   * @category Common
   */
  'min',
  /**
   * The maximum value for the field used during editing.
   * @config {Number} max
   * @category Common
   */
  'max',
  /**
   * Step size for the field used during editing.
   * @config {Number} step
   * @category Common
   */
  'step',
  /**
   * Large step size for the field used during editing. In effect for `SHIFT + click/arrows`
   * @config {Number} largeStep
   * @category Common
   */
  'largeStep',
  /**
   * Unit to append to displayed value.
   * @config {String} unit
   * @category Common
   */
  'unit'];
  static get defaults() {
    return {
      filterType: 'number',
      /**
       * The format to use for rendering numbers.
       *
       * By default, the locale's default number formatter is used. For `en-US`, the
       * locale default is a maximum of 3 decimal digits, using thousands-based grouping.
       * This would render the number `1234567.98765` as `'1,234,567.988'`.
       *
       * @config {String|NumberFormatConfig}
       */
      format: ''
    };
  }
  //endregion
  //region Init
  get defaultEditor() {
    const {
      format,
      name,
      max,
      min,
      step,
      largeStep,
      align
    } = this;
    // Remove any undefined configs, to allow config system to use default values instead
    return ObjectHelper.cleanupProperties({
      type: 'numberfield',
      format,
      name,
      max,
      min,
      step,
      largeStep,
      textAlign: align
    });
  }
  get formatter() {
    const me = this,
      {
        format
      } = me;
    let formatter = me._formatter;
    if (!formatter || me._lastFormat !== format) {
      me._formatter = formatter = NumberFormat.get(me._lastFormat = format);
    }
    return formatter;
  }
  formatValue(value) {
    if (value != null) {
      value = this.formatter.format(value);
      if (this.unit) {
        value = `${value}${this.unit}`;
      }
    }
    return value ?? '';
  }
  /**
   * Renderer that displays value + optional unit in the cell
   * @private
   */
  defaultRenderer({
    value
  }) {
    return this.formatValue(value);
  }
}
ColumnStore.registerColumnType(NumberColumn, true);
NumberColumn.exposeProperties();
NumberColumn._$name = 'NumberColumn';

/**
 * @module Grid/feature/RegionResize
 */
/**
 * Makes the splitter between grid section draggable so you can resize grid sections.
 *
 * {@inlineexample Grid/feature/RegionResize.js}
 *
 * ```javascript
 * // enable RegionResize
 * const grid = new Grid({
 *   features: {
 *     regionResize: true
 *   }
 * });
 * ```
 *
 * This feature is <strong>disabled</strong> by default.
 *
 * @extends Core/mixin/InstancePlugin
 * @demo Grid/features
 * @classtype regionResize
 * @feature
 */
class RegionResize extends InstancePlugin {
  // region Init
  static $name = 'RegionResize';
  static get pluginConfig() {
    return {
      chain: ['onElementPointerDown', 'onElementDblClick', 'onElementTouchMove', 'onSubGridCollapse', 'onSubGridExpand', 'render']
    };
  }
  //endregion
  onElementDblClick(event) {
    const me = this,
      {
        client
      } = me,
      splitterEl = event.target.closest('.b-grid-splitter-collapsed');
    // If collapsed splitter is dblclicked and region is not expanding
    // It is unlikely that user might dblclick splitter twice and even if he does, nothing should happen.
    // But just in case lets not expand twice.
    if (splitterEl && !me.expanding) {
      me.expanding = true;
      let region = splitterEl.dataset.region,
        subGrid = client.getSubGrid(region);
      // Usually collapsed splitter means corresponding region is collapsed. But in case of last two regions one
      // splitter can be collapsed in two directions. So, if corresponding region is expanded then last one is collapsed
      if (!subGrid.collapsed) {
        region = client.getLastRegions()[1];
        subGrid = client.getSubGrid(region);
      }
      subGrid.expand().then(() => me.expanding = false);
    }
  }
  //region Move splitter
  /**
   * Begin moving splitter.
   * @private
   * @param splitterElement Splitter element
   * @param clientX Initial x position from which new width will be calculated on move
   */
  startMove(splitterElement, clientX) {
    const me = this,
      {
        client
      } = me,
      region = splitterElement.dataset.region,
      gridEl = client.element,
      nextRegion = client.regions[client.regions.indexOf(region) + 1],
      nextSubGrid = client.getSubGrid(nextRegion),
      splitterSubGrid = client.getSubGrid(region);
    let subGrid = splitterSubGrid,
      flip = 1;
    if (subGrid.flex != null) {
      // If subgrid has flex, check if next one does not
      if (nextSubGrid.flex == null) {
        subGrid = nextSubGrid;
        flip = -1;
      }
    }
    if (client.rtl) {
      flip *= -1;
    }
    if (splitterElement.classList.contains('b-grid-splitter-collapsed')) {
      return;
    }
    const availableWidth = subGrid.element.offsetWidth + nextSubGrid.element.offsetWidth;
    me.dragContext = {
      element: splitterElement,
      headerEl: subGrid.header.element,
      subGridEl: subGrid.element,
      subGrid,
      splitterSubGrid,
      originalWidth: subGrid.element.offsetWidth,
      originalX: clientX,
      minWidth: subGrid.minWidth || 0,
      maxWidth: Math.min(availableWidth, subGrid.maxWidth || availableWidth),
      flip
    };
    gridEl.classList.add('b-moving-splitter');
    splitterSubGrid.toggleSplitterCls('b-moving');
    me.pointerDetacher = EventHelper.on({
      element: document,
      pointermove: 'onPointerMove',
      pointerup: 'onPointerUp',
      thisObj: me
    });
  }
  /**
   * Stop moving splitter.
   * @private
   */
  endMove() {
    const me = this,
      {
        dragContext
      } = me;
    if (dragContext) {
      me.pointerDetacher();
      me.client.element.classList.remove('b-moving-splitter');
      dragContext.splitterSubGrid.toggleSplitterCls('b-moving', false);
      me.dragContext = null;
    }
  }
  onCollapseClick(subGrid, splitterEl, domEvent) {
    const me = this,
      {
        client
      } = me,
      region = splitterEl.dataset.region,
      regions = client.getLastRegions();
    /**
     * Fired by the Grid when the collapse icon is clicked. Return `false` to prevent the default collapse action,
     * if you want to implement your own behavior.
     * @event splitterCollapseClick
     * @on-owner
     * @preventable
     * @param {Grid.view.Grid} source The Grid instance.
     * @param {Grid.view.SubGrid} subGrid The subgrid
     * @param {Event} domEvent The native DOM event
     */
    if (client.trigger('splitterCollapseClick', {
      subGrid,
      domEvent
    }) === false) {
      return;
    }
    // Last splitter in the grid is responsible for collapsing/expanding last 2 regions and is always related to the
    // left one. Check if we are working with last splitter
    if (regions[0] === region) {
      const lastSubGrid = client.getSubGrid(regions[1]);
      if (lastSubGrid.collapsed) {
        lastSubGrid.expand();
        return;
      }
    }
    subGrid.collapse();
  }
  onExpandClick(subGrid, splitterEl, domEvent) {
    const me = this,
      {
        client
      } = me,
      region = splitterEl.dataset.region,
      regions = client.getLastRegions();
    /**
     * Fired by the Grid when the expand icon is clicked. Return `false` to prevent the default expand action,
     * if you want to implement your own behavior.
     * @event splitterExpandClick
     * @preventable
     * @param {Grid.view.Grid} source The Grid instance.
     * @param {Grid.view.SubGrid} subGrid The subgrid
     * @param {Event} domEvent The native DOM event
     */
    if (client.trigger('splitterExpandClick', {
      subGrid,
      domEvent
    }) === false) {
      return;
    }
    // Last splitter in the grid is responsible for collapsing/expanding last 2 regions and is always related to the
    // left one. Check if we are working with last splitter
    if (regions[0] === region) {
      if (!subGrid.collapsed) {
        const lastSubGrid = client.getSubGrid(regions[1]);
        lastSubGrid.collapse();
        return;
      }
    }
    subGrid.expand();
  }
  /**
   * Update splitter position.
   * @private
   * @param newClientX
   */
  updateMove(newClientX) {
    const {
      dragContext
    } = this;
    if (dragContext) {
      const difX = newClientX - dragContext.originalX,
        newWidth = Math.max(Math.min(dragContext.maxWidth, dragContext.originalWidth + difX * dragContext.flip), 0);
      // SubGrids monitor their own size and keep any splitters synced
      dragContext.subGrid.width = Math.max(newWidth, dragContext.minWidth);
    }
  }
  //endregion
  //region Events
  /**
   * Start moving splitter on mouse down (on splitter).
   * @private
   * @param event
   */
  onElementPointerDown(event) {
    const me = this,
      {
        target
      } = event,
      // Only care about left clicks, avoids a bug found by monkeys
      splitter = event.button === 0 && target.closest(':not(.b-row-reordering):not(.b-dragging-event):not(.b-dragging-task):not(.b-dragging-header):not(.b-dragselecting) .b-grid-splitter'),
      subGrid = splitter && me.client.getSubGrid(splitter.dataset.region);
    let toggle;
    if (splitter) {
      if (target.closest('.b-grid-splitter-button-collapse')) {
        me.onCollapseClick(subGrid, splitter, event);
      } else if (target.closest('.b-grid-splitter-button-expand')) {
        me.onExpandClick(subGrid, splitter, event);
      } else {
        me.startMove(splitter, event.clientX);
        toggle = splitter;
      }
    }
    if (event.pointerType === 'touch') {
      // Touch on splitter makes splitter wider, touch outside or expand/collapse makes it smaller again
      me.toggleTouchSplitter(toggle);
    }
  }
  /**
   * Move splitter on mouse move.
   * @private
   * @param event
   */
  onPointerMove(event) {
    if (this.dragContext) {
      this.updateMove(event.clientX);
      event.preventDefault();
    }
  }
  onElementTouchMove(event) {
    if (this.dragContext) {
      // Needed to prevent scroll in Mobile Safari, preventing pointermove is not enough
      event.preventDefault();
    }
  }
  /**
   * Stop moving splitter on mouse up.
   * @private
   * @param event
   */
  onPointerUp(event) {
    if (this.dragContext) {
      this.endMove();
      event.preventDefault();
    }
  }
  onSubGridCollapse({
    subGrid
  }) {
    const splitterEl = this.client.resolveSplitter(subGrid),
      regions = this.client.getLastRegions();
    // if last region was collapsed
    if (regions[1] === subGrid.region) {
      splitterEl.classList.add('b-grid-splitter-allow-collapse');
    }
  }
  onSubGridExpand({
    subGrid
  }) {
    const splitterEl = this.client.resolveSplitter(subGrid);
    splitterEl.classList.remove('b-grid-splitter-allow-collapse');
  }
  //endregion
  /**
   * Adds b-touching CSS class to splitterElements when touched. Removes when touched outside.
   * @private
   * @param splitterElement
   */
  toggleTouchSplitter(splitterElement) {
    const me = this,
      {
        touchedSplitter
      } = me;
    // If other splitter is touched, deactivate old one
    if (splitterElement && touchedSplitter && splitterElement.dataset.region !== touchedSplitter.dataset.region) {
      me.toggleTouchSplitter();
    }
    // Either we have touched a splitter (should activate) or touched outside (should deactivate)
    const splitterSubGrid = me.client.getSubGrid(splitterElement ? splitterElement.dataset.region : touchedSplitter === null || touchedSplitter === void 0 ? void 0 : touchedSplitter.dataset.region);
    if (splitterSubGrid) {
      splitterSubGrid.toggleSplitterCls('b-touching', Boolean(splitterElement));
      if (splitterElement) {
        splitterSubGrid.startSplitterButtonSyncing();
      } else {
        splitterSubGrid.stopSplitterButtonSyncing();
      }
    }
    me.touchedSplitter = splitterElement;
  }
  render() {
    const {
      regions,
      subGrids
    } = this.client;
    // Multiple regions, only allow collapsing to the edges by hiding buttons
    if (regions.length > 2) {
      // Only works in a 3 subgrid scenario. To support more subgrids we have to merge splitters or something
      // on collapse. Not going down that path currently...
      subGrids[regions[0]].splitterElement.classList.add('b-left-only');
      subGrids[regions[1]].splitterElement.classList.add('b-right-only');
    }
  }
}
RegionResize.featureClass = 'b-split';
RegionResize._$name = 'RegionResize';
GridFeatureManager.registerFeature(RegionResize);

export { NumberColumn, RegionResize };
//# sourceMappingURL=RegionResize.js.map
