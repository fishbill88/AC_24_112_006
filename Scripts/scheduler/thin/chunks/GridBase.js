/*!
 *
 * Bryntum Scheduler Pro 5.3.7
 *
 * Copyright(c) 2023 Bryntum AB
 * https://bryntum.com/contact
 * https://bryntum.com/license
 *
 */
import { Widget, DomHelper, Model, Events, Localizable, Config, ObjectHelper, StringHelper, Store, Objects, Delayable, InstancePlugin, Editor, GlobalEvents, ContextMenuBase, BrowserHelper, EventHelper, TemplateHelper, ScrollManager, ArrayHelper, VersionHelper, Tooltip, CollectionFilter, DomSync, DomDataStore, DomClassList, Base, Rectangle, Scroller, Collection, LocaleHelper, Panel, Pluggable, State, LoadMaskable, LocaleManagerSingleton, AjaxStore, Mask } from './Editor.js';
import { WidgetHelper, Checkbox, MessageDialog, DragHelper, ResizeHelper, FieldFilterPicker, SUPPORTED_FIELD_DATA_TYPES, isSupportedDurationField, FieldFilterPickerGroup } from './MessageDialog.js';
import { GridRowModel } from './GridRowModel.js';

/**
 * @module Grid/util/Location
 */
/**
 * This class encapsulates a reference to a specific navigable grid location.
 *
 * This encapsulates a grid cell based upon the record and column, but in addition, it could represent
 * an actionable location *within a cell** if the {@link #property-target} is not the grid cell in
 * question.
 *
 * A Location is immutable. That is, once instantiated, the record and column which it references
 * cannot be changed. The {@link #function-move} method returns a new instance.
 *
 * A `Location` that encapsulates a cell within the body of a grid will have the following
 * read-only properties:
 *
 *  - grid        : `Grid` The Grid that owns the Location.
 *  - record      : `Model` The record of the row that owns the Location. (`null` if the header).
 *  - rowIndex    : `Number` The zero-based index of the row that owns the Location. (-1 means the header).
 *  - column      : `Column` The Column that owns the Location.
 *  - columnIndex : `Number` The zero-based index of the column that owns the Location.
 *  - cell        : `HTMLElement` The referenced cell element.
 *  - target      : `HTMLElement` The focusable element. This may be the cell, or a child of the cell.
 *
 * If the location is a column *header*, the `record` will be `null` and the `rowIndex` will be `-1`.
 *
 */
class Location {
  /**
   * The grid which this Location references.
   * @config {Grid.view.Grid} grid
   */
  /**
   * The record which this Location references. (unless {@link #config-rowIndex} is used to configure)
   * @config {Core.data.Model} record
   */
  /**
   *
   * The row index which this Location references. (unless {@link #config-record} is used to configure).
   *
   * `-1` means the header row, in which case the {@link #config-record} will be `null`.
   * @config {Number} rowIndex
   */
  /**
   * The Column which this location references. (unless {@link #config-columnIndex} or {@link #config-columnId} is used to configure)
   * @config {Grid.column.Column} column
   */
  /**
   * The column id which this location references. (unless {@link #config-column} or {@link #config-columnIndex} is used to configure)
   * @config {String|Number} columnId
   */
  /**
   * The column index which this location references. (unless {@link #config-column} or {@link #config-columnId} is used to configure)
   * @config {Number} columnIndex
   */
  /**
   * The field of the column index which this location references. (unless another column identifier is used to configure)
   * @config {String} field
   */
  /**
   * Initializes a new Location.
   * @param {LocationConfig|HTMLElement} location A grid location specifier. This may be:
   *  * An element inside a grid cell or a grid cell.
   *  * An object identifying a cell location using the following properties:
   *    * grid
   *    * record
   *    * rowIndex
   *    * column
   *    * columnIndex
   * @function constructor
   */
  constructor(location) {
    // Private usage of init means that we can create an un attached Location
    // The move method does this.
    if (location) {
      // They passed us a Location, so they already know where to go.
      if (location.isLocation) {
        return location;
      }
      // Passed a DOM node.
      if (location.nodeType === Node.ELEMENT_NODE) {
        const grid = Widget.fromElement(location, 'gridbase'),
          cell = grid && location.closest(grid.focusableSelector);
        // We are targeted on, or within a cell.
        if (cell) {
          const {
            dataset
          } = cell.parentNode;
          this.init({
            grid,
            // A .b-grid-row will have a data-index
            // If it' a column header, we use rowIndex -1
            rowIndex: grid.store.includes(dataset.id) ? grid.store.indexOf(dataset.id) : dataset.index || -1,
            columnId: cell.dataset.columnId
          });
          this.initialTarget = location;
        }
      } else {
        this.init(location);
      }
    }
  }
  init(config) {
    var _me$record;
    const me = this;
    const grid = me.grid = config.grid,
      {
        store,
        columns
      } = grid,
      {
        visibleColumns
      } = columns;
    // If we have a target. This is usually only for actionable locations.
    if (config.target) {
      me.actionTargets = [me._target = config.target];
    }
    // Determine our record and rowIndex
    if (config.record) {
      me._id = config.record.id;
    } else if ('id' in config) {
      me._id = config.id;
      // Null means that the Location is in the grid header, so rowIndex -1
      if (config.id == null) {
        me._rowIndex = -1;
      }
    } else {
      var _store$getAt;
      const rowIndex = !isNaN(config.row) ? config.row : !isNaN(config.rowIndex) ? config.rowIndex : NaN;
      me._rowIndex = Math.max(Math.min(Number(rowIndex), store.count - 1), grid.hideHeaders ? 0 : -1);
      me._id = (_store$getAt = store.getAt(me._rowIndex)) === null || _store$getAt === void 0 ? void 0 : _store$getAt.id;
    }
    if (!('_rowIndex' in me)) {
      me._rowIndex = store.indexOf(me.id);
    }
    // Cache value that we use now. We do not hold a reference to a record
    me.isSpecialRow = (_me$record = me.record) === null || _me$record === void 0 ? void 0 : _me$record.isSpecialRow;
    // Determine our column and columnIndex
    if ('columnId' in config) {
      me._column = columns.getById(config.columnId);
    } else if ('field' in config) {
      me._column = columns.get(config.field);
    } else {
      const columnIndex = !isNaN(config.column) ? config.column : !isNaN(config.columnIndex) ? config.columnIndex : NaN;
      if (!isNaN(columnIndex)) {
        me._columnIndex = Math.min(Number(columnIndex), visibleColumns.length - 1);
        me._column = visibleColumns[me._columnIndex];
      }
      // Fall back to using 'column' property either as index or the Column.
      // If no column property, use column zero.
      else {
        me._column = 'column' in config ? isNaN(config.column) ? config.column : visibleColumns[config.column] : visibleColumns[0];
      }
    }
    if (!('_columnIndex' in me)) {
      me._columnIndex = visibleColumns.indexOf(me._column);
    }
  }
  // Class identity indicator. Usually added by extending Base, but we don't do that for perf.
  get isLocation() {
    return true;
  }
  equals(other, shallow = false) {
    const me = this;
    return (other === null || other === void 0 ? void 0 : other.isLocation) && other.grid === me.grid && (
    // For a more performant check, use the shallow param
    shallow ? me.id === other.id && me._column === other._column : other.record === me.record && other.column === me.column && other.target === me.target);
  }
  /**
   * Yields the row index of this location.
   * @property {Number}
   * @readonly
   */
  get rowIndex() {
    const {
        _id
      } = this,
      {
        store
      } = this.grid;
    // Return the up to date row index for our record
    return store.includes(_id) ? store.indexOf(_id) : Math.min(this._rowIndex, store.count - 1);
  }
  /**
   * Used by GridNavigation.
   * @private
   */
  get visibleRowIndex() {
    const {
        rowManager
      } = this.grid,
      {
        rowIndex
      } = this;
    return rowIndex === -1 ? rowIndex : Math.max(Math.min(rowIndex, rowManager.lastFullyVisibleTow.dataIndex), rowManager.firstFullyVisibleTow.dataIndex);
  }
  /**
   * Yields `true` if the cell and row are selectable.
   *
   * That is if the record is present in the grid's store and it's not a group summary or group header record.
   * @property {Boolean}
   * @readonly
   */
  get isSelectable() {
    return this.grid.store.includes(this._id) && !this.isSpecialRow;
  }
  get record() {
    // -1 means the header row
    if (this._rowIndex > -1) {
      const {
        store
      } = this.grid;
      // Location's record no longer in store; fall back to record at same index.
      if (!store.includes(this._id)) {
        return store.getAt(this._rowIndex);
      }
      return store.getById(this._id);
    }
  }
  get id() {
    return this._id;
  }
  get column() {
    const {
      visibleColumns
    } = this.grid.columns;
    // Location's column no longer visible; fall back to column at same index.
    if (!(visibleColumns !== null && visibleColumns !== void 0 && visibleColumns.includes(this._column))) {
      return visibleColumns === null || visibleColumns === void 0 ? void 0 : visibleColumns[this.columnIndex];
    }
    return this._column;
  }
  get columnId() {
    var _this$column;
    return (_this$column = this.column) === null || _this$column === void 0 ? void 0 : _this$column.id;
  }
  get columnIndex() {
    var _this$grid$columns$vi;
    return Math.min(this._columnIndex, ((_this$grid$columns$vi = this.grid.columns.visibleColumns) === null || _this$grid$columns$vi === void 0 ? void 0 : _this$grid$columns$vi.length) - 1);
  }
  /**
   * Returns a __*new *__ `Location` instance having moved from the current location in the
   * mode specified.
   * @param {Number} where Where to move from this Location. May be:
   *
   *  - `Location.UP`
   *  - `Location.NEXT_CELL`
   *  - `Location.DOWN`
   *  - `Location.PREV_CELL`
   *  - `Location.FIRST_COLUMN`
   *  - `Location.LAST_COLUMN`
   *  - `Location.FIRST_CELL`
   *  - `Location.LAST_CELL`
   *  - `Location.PREV_PAGE`
   *  - `Location.NEXT_PAGE`
   * @returns {Grid.util.Location} A Location object encapsulating the target location.
   */
  move(where) {
    const me = this,
      {
        record,
        column,
        grid
      } = me,
      {
        store
      } = grid,
      columns = grid.columns.visibleColumns,
      result = new Location();
    let rowIndex = store.includes(record) ? store.indexOf(record) : me.rowIndex,
      columnIndex = columns.includes(column) ? columns.indexOf(column) : me.columnIndex;
    const rowMin = grid.hideHeaders ? 0 : -1,
      rowMax = store.count - 1,
      colMax = columns.length - 1,
      atFirstRow = rowIndex === rowMin,
      atLastRow = rowIndex === rowMax,
      atFirstColumn = columnIndex === 0,
      atLastColumn = columnIndex === colMax;
    switch (where) {
      case Location.PREV_CELL:
        if (atFirstColumn) {
          if (!atFirstRow) {
            columnIndex = colMax;
            rowIndex--;
          }
        } else {
          columnIndex--;
        }
        break;
      case Location.NEXT_CELL:
        if (atLastColumn) {
          if (!atLastRow) {
            columnIndex = 0;
            rowIndex++;
          }
        } else {
          columnIndex++;
        }
        break;
      case Location.UP:
        if (!atFirstRow) {
          rowIndex--;
        }
        break;
      case Location.DOWN:
        if (!atLastRow) {
          // From the col header, we drop to the topmost fully visible row.
          if (rowIndex === -1) {
            rowIndex = grid.rowManager.firstFullyVisibleRow.dataIndex;
          } else {
            rowIndex++;
          }
        }
        break;
      case Location.FIRST_COLUMN:
        columnIndex = 0;
        break;
      case Location.LAST_COLUMN:
        columnIndex = colMax;
        break;
      case Location.FIRST_CELL:
        rowIndex = rowMin;
        columnIndex = 0;
        break;
      case Location.LAST_CELL:
        rowIndex = rowMax;
        columnIndex = colMax;
        break;
      case Location.PREV_PAGE:
        rowIndex = Math.max(rowMin, rowIndex - Math.floor(grid.scrollable.clientHeight / grid.rowHeight));
        break;
      case Location.NEXT_PAGE:
        rowIndex = Math.min(rowMax, rowIndex + Math.floor(grid.scrollable.clientHeight / grid.rowHeight));
        break;
    }
    // Set the calculated coordinates in the result.
    result.init({
      grid,
      rowIndex,
      columnIndex
    });
    return result;
  }
  /**
   * The cell DOM element which this Location references.
   * @property {HTMLElement}
   * @readonly
   */
  get cell() {
    const me = this,
      {
        grid,
        id,
        _cell
      } = me;
    // Property value set
    if (_cell) {
      return _cell;
    }
    // On a header cell
    if (id == null) {
      var _grid$columns$getById;
      return (_grid$columns$getById = grid.columns.getById(me.columnId)) === null || _grid$columns$getById === void 0 ? void 0 : _grid$columns$getById.element;
    } else {
      const {
        row
      } = me;
      if (row) {
        var _grid$columns$getAt;
        return row.getCell(me.columnId) || row.getCell((_grid$columns$getAt = grid.columns.getAt(me.columnIndex)) === null || _grid$columns$getAt === void 0 ? void 0 : _grid$columns$getAt.id);
      }
    }
  }
  get row() {
    // Use our record ID by preference, but fall back to our row index if not present
    return this.grid.getRowById(this.id) || this.grid.getRow(this.rowIndex);
  }
  /**
   * The DOM element which encapsulates the focusable target of this Location.
   *
   * This is usually the {@link #property-cell}, but if this is an actionable location, this
   * may be another DOM element within the cell.
   * @property {HTMLElement}
   * @readonly
   */
  get target() {
    const {
        cell,
        _target
      } = this,
      {
        focusableFinder
      } = this.grid;
    // We might be asked for our focusElement before we're fully rendered and painted.
    if (cell) {
      // Location was created in disableActionable mode with the target
      // explicitly directed to the cell.
      if (_target) {
        return _target;
      }
      focusableFinder.currentNode = this.grid.focusableFinderCell = cell;
      return focusableFinder.nextNode() || cell;
    }
  }
  /**
   * This property is `true` if the focus target is not the cell itself.
   * @property {Boolean}
   * @readonly
   */
  get isActionable() {
    const {
        cell,
        _target
      } = this,
      activeEl = cell && DomHelper.getActiveElement(cell),
      containsFocus = activeEl && cell.compareDocumentPosition(activeEl) & Node.DOCUMENT_POSITION_CONTAINED_BY;
    // The actual target may be inside the cell, or just positioned to *appear* inside the cell
    // such as event/task rendering.
    return Boolean(containsFocus || _target && _target !== this.cell);
  }
  /**
   * This property is `true` if this location represents a column header.
   * @property {Boolean}
   * @readonly
   */
  get isColumnHeader() {
    return this.cell && this.rowIndex === -1;
  }
  /**
   * This property is `true` if this location represents a cell in the grid body.
   * @property {Boolean}
   * @readonly
   */
  get isCell() {
    return this.cell && this.record;
  }
}
Location.UP = 1;
Location.NEXT_CELL = 2;
Location.DOWN = 3;
Location.PREV_CELL = 4;
Location.FIRST_COLUMN = 5;
Location.LAST_COLUMN = 6;
Location.FIRST_CELL = 7;
Location.LAST_CELL = 8;
Location.PREV_PAGE = 9;
Location.NEXT_PAGE = 10;
Location._$name = 'Location';

/**
 * @module Grid/column/Column
 */
const validWidth = value => typeof value === 'number' || (value === null || value === void 0 ? void 0 : value.endsWith('px'));
/**
 * Base class for other column types, used if no type is specified on a column.
 *
 * Default editor is a {@link Core.widget.TextField}.
 *
 * ```javascript
 * const grid = new Grid({
 *   columns : [{
 *     field : 'name',
 *     text  : 'Name'
 *   }, {
 *     text  : 'Hobby',
 *     field : 'others.hobby', // reading nested field data
 *   }, {
 *     type  : 'number', // Will use NumberColumn
 *     field : 'age',
 *     text  : 'Age'
 *   }]
 * });
 * ```
 *
 * ## Column types
 *
 * Grid ships with multiple different column types. Which type to use for a column is specified by the `type` config.
 * The built-in types are:
 *
 * * {@link Grid.column.ActionColumn action} - displays actions (clickable icons) in the cell.
 * * {@link Grid.column.AggregateColumn aggregate} - a column, which, when used as part of a Tree, aggregates the values
 *   of this column's descendants using a configured function which defaults to `sum`.
 * * {@link Grid.column.CheckColumn check} - displays a checkbox in the cell.
 * * {@link Grid.column.DateColumn date} - displays a date in the specified format.
 * * {@link Grid.column.NumberColumn number} - a column for showing/editing numbers.
 * * {@link Grid.column.PercentColumn percent} - displays a basic progress bar.
 * * {@link Grid.column.RatingColumn rating} - displays a star rating.
 * * {@link Grid.column.RowNumberColumn rownumber} - displays the row number in each cell.
 * * {@link Grid.column.TemplateColumn template} - uses a template for cell content.
 * * {@link Grid.column.TimeColumn time} - displays a time in the specified format.
 * * {@link Grid.column.TreeColumn tree} - displays a tree structure when using the {@link Grid.feature.Tree tree}
 *   feature.
 * * {@link Grid.column.WidgetColumn widget} - displays widgets in the cells.
 *
 * ## Grouped columns / headers
 *
 * You can group headers by defining parent and children columns. A group can be dragged as a whole, or users can drag
 * individual columns between groups. The same applies to column visibility using the column picker in the header menu,
 * a group can be toggled as a whole or each column individually.
 *
 * ```javascript
 * const grid = new Grid({
 *     {
 *             text     : 'Parent',
 *             children : [
 *                 { text : 'Child 1', field : 'field1', flex : 1 },
 *                 { text : 'Child 2', field : 'field2', flex : 1 }
 *             ]
 *         },
 *         ...
 * }
 * ```
 *
 * {@inlineexample Grid/column/ColumnGrouped.js}
 *
 * ## Collapsible columns
 *
 * By configuring a parent column with `collapsible: true` it is made collapsible. When collapsing all child columns
 * except the first one are hidden. This behaviour is configurable using the {@link #config-collapseMode} config. To
 * make a column start collapsed, set the {@link #config-collapsed} config to `true`.
 *
 * {@inlineexample Grid/column/ColumnCollapse.js}
 *
 * ## Cell renderers
 *
 * You can affect the contents and styling of cells in a column using a
 * {@link Grid.column.Column#config-renderer} function.
 *
 * ```javascript
 * const grid = new Grid({
 *   columns : [
 *   ...
 *     {
 *       field      : 'approved',
 *       text       : 'Approved',
 *       htmlEncode : false, // allow to use HTML code
 *       renderer({ value }) {
 *         return value === true ? '<b>Yes</b>' : '<i>No</i>';
 *       }
 *     }
 *     ...
 *     ]
 * });
 * ```
 *
 * ## Menus
 *
 * You can add custom items to the context menu for a columns header and for its cells, using
 * {@link Grid.column.Column#config-headerMenuItems} and {@link Grid.column.Column#config-cellMenuItems}. Here is an
 * example:
 *
 * ```javascript
 * const grid = new Grid({
 *   columns : [
 *     ...
 *     {
 *       type  : 'number',
 *       field : 'age',
 *       text  : 'Age',
 *       headerMenuItems: [{
 *           text : 'My unique header item',
 *           icon : 'b-fa b-fa-paw',
 *           onItem() { console.log('item clicked'); }
 *       }],
 *       cellMenuItems: [{
 *           text : 'My unique cell item',
 *           icon : 'b-fa b-fa-plus',
 *           onItem() { console.log('item clicked'); }
 *       }]
 *     }
 *   ...
 *   ]
 * });
 * ```
 *
 * @extends Core/data/Model
 * @classType column
 * @mixes Core/mixin/Events
 * @mixes Core/localization/Localizable
 * @column
 */
class Column extends Model.mixin(Events, Localizable) {
  static get $name() {
    return 'Column';
  }
  /**
   * Column name alias which you can use in the `columns` array of a Grid.
   *
   * ```javascript
   * class MyColumn extends Column {
   *     static get type() {
   *        return 'mycolumn';
   *     }
   * }
   * ```
   *
   * ```javascript
   * const grid = new Grid({
   *    columns : [
   *       { type : 'mycolumn', text : 'The column', field : 'someField', flex : 1 }
   *    ]
   * });
   * ```
   *
   * @static
   * @member {String} type
   */
  static get type() {
    return 'column';
  }
  //region Config
  /**
   * Default settings for the column, applied in constructor. None by default, override in subclass.
   * @member {Object} defaults
   * @returns {Object}
   * @readonly
   */
  static get fields() {
    /**
     * @hideFields readOnly
     */
    return [
    //region Common
    'type',
    /**
     * Header text
     * @prp {String} text
     * @category Common
     */
    'text',
    /**
     * The {@link Core.data.field.DataField#config-name} of the {@link Core.data.Model data model} field to read
     * @prp {String} field
     * @readonly
     * @category Common
     */
    'field',
    // NOTE: This is duplicated in WidgetColumn and partly in TreeColumn so remember to change there too if
    // changing the signature of this function
    /**
     * Renderer function, used to format and style the content displayed in the cell. Return the cell text you
     * want to display. Can also affect other aspects of the cell, such as styling.
     *
     * **NOTE:** If you mutate `cellElement`, and you want to prevent cell content from being reset during
     * rendering, please return `undefined` from the renderer (or just omit the `return` statement) and make
     * sure that the {@link #config-alwaysClearCell} config is set to `false`.
     *
     * ```javascript
     * new Grid({
     *     columns : [
     *         // Returns an empty string if status field value is undefined
     *         { text : 'Status', renderer : ({ record }) => record.status ?? '' },
     *
     *         // From Grid v6.0 there is no need for the undefined check
     *         // { text : 'Status', renderer : ({ record }) => record.status }
     *     ]
     * });
     * ```
     *
     * You can also return a {@link Core.helper.DomHelper#typedef-DomConfig} object describing the markup
     * ```javascript
     * new Grid({
     *     columns : [
     *         {
     *              text : 'Status',
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
    'renderer',
    //'reactiveRenderer',
    /**
     * Column width. If value is Number then width is in pixels
     * @config {Number|String} width
     * @category Common
     */
    'width',
    /**
     * Column width as a flex weight. All columns with flex specified divide the available space (after
     * subtracting fixed widths) between them according to the flex value. Columns that have flex 2 will be
     * twice as wide as those with flex 1 (and so on)
     * @prp {Number} flex
     * @category Common
     */
    'flex',
    /**
     * This config sizes a column to fits its content. It is used instead of `width` or `flex`.
     *
     * This config requires the {@link Grid.feature.ColumnAutoWidth} feature which responds to changes in the
     * grid's store and synchronizes the widths' of all `autoWidth` columns.
     *
     * If this config is not a Boolean value, it is passed as the only argument to the `resizeToFitContent`
     * method to constrain the column's width.
     *
     * @config {Boolean|Number|Number[]} autoWidth
     * @category Common
     */
    'autoWidth',
    /**
     * This config enables automatic height for all cells in this column. It is achieved by measuring the height
     * a cell after rendering it to DOM, and then sizing the row using that height (if it is greater than other
     * heights used for the row).
     *
     * Heads up if you render your Grid on page load, if measurement happens before the font you are using is
     * loaded you might get slightly incorrect heights. For browsers that support it we detect that
     * and remeasure when fonts are available.
     *
     * **NOTE:** Enabling this config comes with a pretty big performance hit. To maintain good performance,
     * we recommend not using it. You can still set the height of individual rows manually, either through
     * {@link Grid.data.GridRowModel#field-rowHeight data} or via {@link #config-renderer renderers}.
     *
     * Also note that this setting only works fully as intended with non-flex columns.
     *
     * Rows will always be at least {@link Grid.view.Grid#config-rowHeight} pixels tall
     * even if an autoHeight cell contains no data.
     *
     * Manually setting a height from a {@link #config-renderer} in this column will take precedence over this
     * config.
     *
     * @config {Boolean} autoHeight
     * @category Common
     */
    'autoHeight',
    /**
     * Mode to use when measuring the contents of this column in calls to {@link #function-resizeToFitContent}.
     * Available modes are:
     *
     * * 'exact'       - Most precise, renders and measures all cells (Default, slowest)
     * * 'textContent' - Renders all cells but only measures the one with the longest `textContent`
     * * 'value'       - Renders and measures only the cell with the longest data (Fastest)
     * * 'none'/falsy  - Resize to fit content not allowed, a call does nothing
     *
     * @config {'exact'|'textContent'|'value'|'none'|null} fitMode
     * @default 'exact'
     * @category Common
     */
    {
      name: 'fitMode',
      defaultValue: 'exact'
    },
    //endregion
    //region Interaction
    /**
     * Set this to `true` to not allow any type of editing in this column.
     * @prp {Boolean} readOnly
     */
    'readOnly',
    /**
     * A config object used to create the input field which will be used for editing cells in the
     * column. Used when {@link Grid.feature.CellEdit} feature is enabled. The Editor refers to
     * {@link #config-field} for a data source.
     *
     * Configure this as `null` to prevent cell editing in this column.
     *
     * All subclasses of {@link Core.widget.Field} can be used as editors. The most popular are:
     * - {@link Core.widget.TextField}
     * - {@link Core.widget.NumberField}
     * - {@link Core.widget.DateField}
     * - {@link Core.widget.TimeField}
     * - {@link Core.widget.Combo}
     *
     * If record has method set + capitalized field, method will be called, e.g. if record has method named
     * `setFoobar` and the {@link #config-field} is `foobar`, then instead of `record.foobar = value`,
     * `record.setFoobar(value)` will be called.
     *
     * @config {Boolean|String|InputFieldConfig|Core.widget.Field} editor
     * @category Interaction
     */
    {
      name: 'editor',
      defaultValue: {}
    },
    /**
     * A config object used to configure an {@link Core.widget.Editor} which contains this Column's
     * {@link #config-editor input field} if {@link Grid.feature.CellEdit} feature is enabled.
     * @config {EditorConfig} cellEditor
     * @category Interaction
     */
    'cellEditor',
    /**
     * A function which is called when a cell edit is requested to finish.
     *
     * This may be an `async` function which performs complex validation. The edit will not
     * complete until it returns `false` to mean the edit cannot be finished, or `true` to go
     * ahead and complete.
     *
     * @param {Object} context An object describing the state of the edit at completion request time.
     * @param {Core.widget.Field} context.inputField The field configured as the column's `editor`.
     * @param {Core.data.Model} context.record The record being edited.
     * @param {*} context.oldValue The old value of the cell.
     * @param {*} context.value The new value of the cell.
     * @param {Grid.view.Grid} context.grid The host grid.
     * @param {Object} context.editorContext The {@link Grid.feature.CellEdit} context object.
     * @param {Grid.column.Column} context.editorContext.column The column being edited.
     * @param {Core.data.Model} context.editorContext.record The record being edited.
     * @param {HTMLElement} context.editorContext.cell The cell element hosting the editor.
     * @param {Core.widget.Editor} context.editorContext.editor The floating Editor widget which is hosting the
     * input field.
     * @config {Function} finalizeCellEdit
     * @category Interaction
     */
    'finalizeCellEdit',
    /**
     * Setting this option means that pressing the `ESCAPE` key after editing the field will
     * revert the field to the value it had when the edit began. If the value is _not_ changed
     * from when the edit started, the input field's {@link Core.widget.Field#config-clearable}
     * behaviour will be activated. Finally, the edit will be canceled.
     * @config {Boolean} revertOnEscape
     * @default true
     * @category Interaction
     */
    {
      name: 'revertOnEscape',
      defaultValue: true
    },
    /**
     * How to handle a request to complete a cell edit in this column if the field is invalid.
     * There are three choices:
     *  - `block` The default. The edit is not exited, the field remains focused.
     *  - `allow` Allow the edit to be completed.
     *  - `revert` The field value is reverted and the edit is completed.
     * @config {'block'|'allow'|'revert'} invalidAction
     * @default 'block'
     * @category Interaction
     */
    {
      name: 'invalidAction',
      defaultValue: 'block'
    },
    /**
     * Allow sorting of data in the column. You can pass true/false to enable/disable sorting, or provide a
     * custom sorting function, or a config object for a {@link Core.util.CollectionSorter}
     *
     * ```javascript
     * const grid = new Grid({
     *     columns : [
     *          {
     *              // Disable sorting for this column
     *              sortable : false
     *          },
     *          {
     *              field : 'name',
     *              // Custom sorting for this column
     *              sortable(user1, user2) {
     *                  return user1.name < user2.name ? -1 : 1;
     *              }
     *          },
     *          {
     *              // A config object for a Core.util.CollectionSorter
     *              sortable : {
     *                  property         : 'someField',
     *                  direction        : 'DESC',
     *                  useLocaleCompare : 'sv-SE'
     *              }
     *          }
     *     ]
     * });
     * ```
     * When providing a custom sorting function, if the sort feature is configured with
     * `prioritizeColumns : true` that function will also be used for programmatic sorting of the store:
     *
     * ```javascript
     * const grid = new Grid({
     *     features : {
     *       sort : {
     *           prioritizeColumns : true
     *       }
     *     },
     *
     *     columns : [
     *          {
     *              field : 'name',
     *              // Custom sorting for this column
     *              sortable(user1, user2) {
     *                  return user1.name < user2.name ? -1 : 1;
     *              }
     *          }
     *     ]
     * });
     *
     * // Will use sortable() from the column definition above
     * grid.store.sort('name');
     * ```
     *
     * @config {Boolean|Function|CollectionSorterConfig} sortable
     * @default true
     * @category Interaction
     */
    {
      name: 'sortable',
      defaultValue: true,
      // Normalize function/object forms
      convert(value, data, column) {
        if (!value) {
          return false;
        }
        if (value === true) {
          return true;
        }
        const sorter = {};
        if (typeof value === 'function') {
          sorter.originalSortFn = value;
          // Scope for sortable() expected to be the column
          sorter.sortFn = value.bind(column);
        } else if (typeof value === 'object') {
          Object.assign(sorter, value);
          if (sorter.fn) {
            sorter.sortFn = sorter.fn;
            delete sorter.fn;
          }
        }
        return sorter;
      }
    },
    /**
     * Allow searching in the column (respected by QuickFind and Search features)
     * @config {Boolean} searchable
     * @default true
     * @category Interaction
     */
    {
      name: 'searchable',
      defaultValue: true
    },
    /**
     * If `true`, this column will show a collapse/expand icon in its header, only applicable for parent columns
     * @config {Boolean} collapsible
     * @default false
     * @category Interaction
     */
    {
      name: 'collapsible',
      defaultValue: false
    },
    /**
     * The collapsed state of this column, only applicable for parent columns
     * @config {Boolean} collapsed
     * @default false
     * @category Interaction
     */
    {
      name: 'collapsed',
      defaultValue: false
    },
    /**
     * The collapse behavior when collapsing a parent column. Specify "toggleAll" or "showFirst".
     * * "showFirst" toggles visibility of all but the first columns.
     * * "toggleAll" toggles all children, useful if you have a special initially hidden column which gets shown
     * in collapsed state.
     * @config {String} collapseMode
     * @default 'showFirst'
     * @category Interaction
     */
    {
      name: 'collapseMode'
    },
    /**
     * Allow filtering data in the column (if {@link Grid.feature.Filter} or {@link Grid.feature.FilterBar}
     * feature is enabled).
     *
     * Also allows passing a custom filtering function that will be called for each record with a single
     * argument of format `{ value, record, [operator] }`. Returning `true` from the function includes the
     * record in the filtered set.
     *
     * Configuration object may be used for {@link Grid.feature.FilterBar} feature to specify `filterField`. See
     * an example in the code snippet below or check {@link Grid.feature.FilterBar} page for more details.
     *
     * ```
     * const grid = new Grid({
     *     columns : [
     *          {
     *              field : 'name',
     *              // Disable filtering for this column
     *              filterable : false
     *          },
     *          {
     *              field : 'age',
     *              // Custom filtering for this column
     *              filterable: ({ value, record }) => Math.abs(record.age - value) < 10
     *          },
     *          {
     *              field : 'start',
     *              // Changing default field type
     *              filterable: {
     *                  filterField : {
     *                      type : 'datetime'
     *                  }
     *              }
     *          },
     *          {
     *              field : 'city',
     *              // Filtering for a value out of a list of values
     *              filterable: {
     *                  filterField : {
     *                      type  : 'combo',
     *                      value : '',
     *                      items : [
     *                          'Paris',
     *                          'Dubai',
     *                          'Moscow',
     *                          'London',
     *                          'New York'
     *                      ]
     *                  }
     *              }
     *          },
     *          {
     *              field : 'score',
     *              filterable : {
     *                  // This filter fn doesn't return 0 values as matching filter 'less than'
     *                  filterFn : ({ record, value, operator, property }) => {
     *                      switch (operator) {
     *                          case '<':
     *                              return record[property] === 0 ? false : record[property] < value;
     *                          case '=':
     *                              return record[property] == value;
     *                          case '>':
     *                              return record[property] > value;
     *                      }
     *                  }
     *              }
     *          }
     *     ]
     * });
     * ```
     *
     * When providing a custom filtering function, if the filter feature is configured with
     * `prioritizeColumns : true` that function will also be used for programmatic filtering of the store:
     *
     * ```javascript
     * const grid = new Grid({
     *     features : {
     *         filter : {
     *             prioritizeColumns : true
     *         }
     *     },
     *
     *     columns : [
     *          {
     *              field : 'age',
     *              // Custom filtering for this column
     *              filterable: ({ value, record }) => Math.abs(record.age - value) < 10
     *          }
     *     ]
     * });
     *
     * // Will use filterable() from the column definition above
     * grid.store.filter({
     *     property : 'age',
     *     value    : 50
     * });
     * ```
     *
     * To use custom `FilterField` combo `store` it should contain one of these
     * {@link Core.data.Store#config-data} or {@link Core.data.AjaxStore#config-readUrl} configs.
     * Otherwise combo will get data from owner Grid store.
     *
     * ```javascript
     * const grid = new Grid({
     *     columns : [
     *          {
     *              field : 'name',
     *              filterable: {
     *                  filterField {
     *                      type  : 'combo',
     *                      store : new Store({
     *                          data : ['Adam', 'Bob', 'Charlie']
     *                      })
     *                  }
     *              }
     *          }
     *     ]
     * });
     * ```
     *
     * or
     *
     * ```javascript
     * const grid = new Grid({
     *     columns : [
     *          {
     *              field : 'name',
     *              filterable: {
     *                  filterField : {
     *                     type  : 'combo',
     *                     store : new AjaxStore({
     *                         readUrl  : 'data/names.json',
     *                         autoLoad : true
     *                     })
     *                  }
     *              }
     *          }
     *     ]
     * });
     * ```
     *
     * @config {Boolean|Function|Object} filterable
     * @default true
     * @category Interaction
     */
    {
      name: 'filterable',
      defaultValue: true,
      // Normalize function/object forms
      convert(value) {
        if (!value) {
          return false;
        }
        if (value === true) {
          return true;
        }
        const filter = {
          columnOwned: true
        };
        if (typeof value === 'function') {
          filter.filterFn = value;
        } else if (typeof value === 'object') {
          Object.assign(filter, value);
        }
        return filter;
      }
    },
    /**
     * Setting this flag to `true` will prevent dropping child columns into a group column
     * @config {Boolean} sealed
     * @default false
     * @category Interaction
     */
    {
      name: 'sealed'
    },
    /**
     * Allow column visibility to be toggled through UI
     * @config {Boolean} hideable
     * @default true
     * @category Interaction
     */
    {
      name: 'hideable',
      defaultValue: true
    },
    /**
     * Set to false to prevent this column header from being dragged
     * @config {Boolean} draggable
     * @category Interaction
     */
    {
      name: 'draggable',
      defaultValue: true
    },
    /**
     * Set to false to prevent grouping by this column
     * @config {Boolean} groupable
     * @category Interaction
     */
    {
      name: 'groupable',
      defaultValue: true
    },
    /**
     * Set to `false` to prevent the column from being drag-resized when the ColumnResize plugin is enabled.
     * @config {Boolean} resizable
     * @default true
     * @category Interaction
     */
    {
      name: 'resizable',
      defaultValue: true
    },
    //endregion
    //region Rendering
    /**
     * Renderer function for group headers (when using Group feature).
     * ```javascript
     * const grid = new Grid({
     *     columns : [
     *         {
     *             text : 'ABC',
     *             groupRenderer(renderData) {
     *                 return {
     *                      class : {
     *                          big   : true,
     *                          small : false
     *                      },
     *                      children : [
     *                          { tag : 'img', src : 'img.png' },
     *                          renderData.groupRowFor
     *                      ]
     *                 };
     *             }
     *         }
     *     ]
     * });
     * ```
     * @param {Object} renderData
     * @param {HTMLElement} renderData.cellElement Cell element, for adding CSS classes, styling etc.
     * @param {*} renderData.groupRowFor Current group value
     * @param {Core.data.Model} renderData.record Record for the row
     * @param {Core.data.Model[]} renderData.groupRecords Records in the group
     * @param {Grid.column.Column} renderData.column Current rendering column
     * @param {Grid.column.Column} renderData.groupColumn Column that the grid is grouped by
     * @param {Number} renderData.count Number of records in the group
     * @param {Grid.view.Grid} renderData.grid This grid
     * @config {Function} groupRenderer
     * @returns {String|DomConfig} The header grouping text or DomConfig object representing the HTML markup
     * @category Rendering
     */
    'groupRenderer',
    /**
     * Renderer function for the column header.
     * @param {Object} renderData
     * @param {Grid.column.Column} renderData.column This column
     * @param {HTMLElement} renderData.headerElement The header element
     * @config {Function} headerRenderer
     * @category Rendering
     */
    'headerRenderer',
    /**
     * A tooltip string to show when hovering the column header, or a config object which can
     * reconfigure the shared tooltip by setting boolean, numeric and string config values.
     * @config {String|TooltipConfig} tooltip
     * @category Rendering
     */
    'tooltip',
    /**
     * Renderer function for the cell tooltip (used with {@link Grid.feature.CellTooltip} feature).
     * Specify `false` to disable tooltip for this column.
     * @param {HTMLElement} cellElement Cell element
     * @param {Core.data.Model} record Record for cell row
     * @param {Grid.column.Column} column Cell column
     * @param {Grid.feature.CellTooltip} cellTooltip Feature instance, used to set tooltip content async
     * @param {MouseEvent} event The event that triggered the tooltip
     * @config {Function|Boolean} tooltipRenderer
     * @category Rendering
     */
    'tooltipRenderer',
    /**
     * CSS class added to each cell in this column
     * @prp {String} cellCls
     * @category Rendering
     */
    'cellCls',
    /**
     * CSS class added to the header of this column
     * @config {String} cls
     * @category Rendering
     */
    'cls',
    /**
     * Icon to display in header. Specifying an icon will render a `<i>` element with the icon as value for the
     * class attribute
     * @prp {String} icon
     * @category Rendering
     */
    'icon',
    //endregion
    //region Layout
    /**
     * Text align. Accepts `'left'`/`'center'`/`'right'` or direction neutral `'start'`/`'end'`
     * @config {'left'|'center'|'right'|'start'|'end'} align
     * @category Layout
     */
    'align',
    /**
     * Column minimal width. If value is `Number`, then minimal width is in pixels
     * @config {Number|String} minWidth
     * @default 60
     * @category Layout
     */
    {
      name: 'minWidth',
      defaultValue: 60
    },
    /**
     * Column maximal width. If value is Number, then maximal width is in pixels
     * @config {Number|String} maxWidth
     * @category Common
     */
    'maxWidth',
    /**
     * Columns hidden state. Specify `true` to hide the column, `false` to show it.
     * @prp {Boolean} hidden
     * @category Layout
     */
    {
      name: 'hidden',
      defaultValue: false
    },
    /**
     * Convenient way of putting a column in the "locked" region. Same effect as specifying region: 'locked'.
     * If you have defined your own regions (using {@link Grid.view.Grid#config-subGridConfigs}) you should use
     * {@link #config-region} instead of this one.
     * @config {Boolean} locked
     * @default false
     * @category Layout
     */
    {
      name: 'locked'
    },
    /**
     * Region (part of the grid, it can be configured with multiple) where to display the column. Defaults to
     * {@link Grid.view.Grid#config-defaultRegion}.
     *
     * A column under a grouped header automatically belongs to the same region as the grouped header.
     *
     * @config {String} region
     * @category Layout
     */
    {
      name: 'region'
    },
    /**
     * Specify `true` to merge cells within the column whose value match between rows, making the first
     * occurrence of the value span multiple rows.
     *
     * Only applies when using the {@link Grid/feature/MergeCells MergeCells feature}.
     *
     * This setting can also be toggled using the column header menu.
     *
     * @config {Boolean} mergeCells
     * @category Merge cells
     */
    {
      name: 'mergeCells',
      type: 'boolean'
    },
    /**
     * Set to `false` to prevent merging cells in this column using the column header menu.
     *
     * Only applies when using the {@link Grid/feature/MergeCells MergeCells feature}.
     *
     * @config {Boolean} mergeable
     * @default true
     * @category Merge cells
     */
    {
      name: 'mergeable',
      type: 'boolean',
      defaultValue: true
    },
    /**
     * An empty function by default, but provided so that you can override it. This function is called each time
     * a merged cell is rendered. It allows you to manipulate the DOM config object used before it is synced to
     * DOM, thus giving you control over styling and contents.
     *
     * NOTE: The function is intended for formatting, you should not update records in it since updating records
     * triggers another round of rendering.
     *
     * ```javascript
     * const grid = new Grid({
     *   columns : [
     *     {
     *       field      : 'project',
     *       text       : 'Project',
     *       mergeCells : 'true,
     *       mergedRenderer({ domConfig, value, fromIndex, toIndex }) {
     *         domConfig.className.highlight = value === 'Important project';
     *       }
     *    }
     *  ]
     * });
     * ```
     *
     * @config {Function}
     * @param {Object} detail An object containing the information needed to render a task.
     * @param {*} detail.value Value that will be displayed in the merged cell
     * @param {Number} detail.fromIndex Index in store of the first row of the merged cell
     * @param {Number} detail.toIndex Index in store of the last row of the merged cell
     * @param {Core.helper.DomHelper#typedef-DomConfig} detail.domConfig DOM config object for the merged cell
     * element
     * @category Merge cells
     */
    'mergedRenderer',
    //endregion
    // region Menu
    /**
     * Show column picker for the column
     * @config {Boolean} showColumnPicker
     * @default true
     * @category Menu
     */
    {
      name: 'showColumnPicker',
      defaultValue: true
    },
    /**
     * false to prevent showing a context menu on the column header element
     * @config {Boolean} enableHeaderContextMenu
     * @default true
     * @category Menu
     */
    {
      name: 'enableHeaderContextMenu',
      defaultValue: true
    },
    /**
     * Set to `false` to prevent showing a context menu on the cell elements in this column
     * @config {Boolean} enableCellContextMenu
     * @default true
     * @category Menu
     */
    {
      name: 'enableCellContextMenu',
      defaultValue: true
    },
    /**
     * Extra items to show in the header context menu for this column.
     *
     * ```javascript
     * headerMenuItems : {
     *     customItem : { text : 'Custom item' }
     * }
     * ```
     *
     * @config {Object<String,MenuItemConfig|Boolean|null>} headerMenuItems
     * @category Menu
     */
    'headerMenuItems',
    /**
     * Extra items to show in the cell context menu for this column, `null` or `false` to not show any menu items
     * for this column.
     *
     * ```javascript
     * cellMenuItems : {
     *     customItem : { text : 'Custom item' }
     * }
     * ```
     *
     * @config {Object<String,MenuItemConfig|Boolean|null>} cellMenuItems
     * @category Menu
     */
    'cellMenuItems',
    //endregion
    //region Summary
    /**
     * Summary type (when using Summary feature). Valid types are:
     * <dl class="wide">
     * <dt>sum <dd>Sum of all values in the column
     * <dt>add <dd>Alias for sum
     * <dt>count <dd>Number of rows
     * <dt>countNotEmpty <dd>Number of rows containing a value
     * <dt>average <dd>Average of all values in the column
     * <dt>function <dd>A custom function, used with store.reduce. Should take arguments (sum, record)
     * </dl>
     * @config {'sum'|'add'|'count'|'countNotEmpty'|'average'|Function} sum
     * @category Summary
     */
    'sum',
    /**
     * Summary configs, use if you need multiple summaries per column. Replaces {@link #config-sum} and
     * {@link #config-summaryRenderer} configs.
     * @config {ColumnSummaryConfig[]} summaries
     * @category Summary
     */
    'summaries',
    /**
     * Renderer function for summary (when using Summary feature). The renderer is called with an object having
     * the calculated summary `sum` as only member.
     * @config {Function} summaryRenderer
     * @param {Number} summaryRenderer.sum The sum
     * @category Summary
     */
    'summaryRenderer',
    //endregion
    //region Misc
    /**
     * Column settings at different responsive levels, see responsive demo under examples/
     * @config {Object} responsiveLevels
     * @category Misc
     */
    'responsiveLevels',
    /**
     * Tags, may be used by ColumnPicker feature for grouping columns by tag in the menu
     * @config {String[]} tags
     * @category Misc
     */
    'tags',
    /**
     * Column config to apply to normal config if viewed on a touch device
     * @config {ColumnConfig} touchConfig
     * @category Misc
     */
    'touchConfig',
    /**
     * When using the tree feature, exactly one column should specify { tree: true }
     * @config {Boolean} tree
     * @category Misc
     */
    'tree',
    /**
     * Determines which type of filtering to use for the column. Usually determined by the column type used,
     * but may be overridden by setting this field.
     * @config {'text'|'date'|'number'|'duration'} filterType
     * @category Misc
     */
    'filterType',
    /**
     * By default, any rendered column cell content is HTML-encoded. Set this flag to `false` disable this and
     * allow rendering html elements
     * @config {Boolean} htmlEncode
     * @default true
     * @category Misc
     */
    {
      name: 'htmlEncode',
      defaultValue: true
    },
    /**
     * By default, the header text is HTML-encoded. Set this flag to `false` disable this and allow html
     * elements in the column header
     * @config {Boolean} htmlEncodeHeaderText
     * @default true
     * @category Misc
     */
    {
      name: 'htmlEncodeHeaderText',
      defaultValue: true
    },
    /**
     * Set to `true`to automatically call DomHelper.sync for html returned from a renderer. Should in most cases
     * be more performant than replacing entire innerHTML of cell and also allows CSS transitions to work. Has
     * no effect unless {@link #config-htmlEncode} is disabled. Returned html must contain a single root element
     * (that can have multiple children). See PercentColumn for example usage.
     * @config {Boolean} autoSyncHtml
     * @default false
     * @category Misc
     */
    {
      name: 'autoSyncHtml',
      defaultValue: false
    },
    /**
     * Set to `false` to not always clear cell content if the {@link #config-renderer} returns `undefined`
     * or has no `return` statement. This is useful when you mutate the cellElement, and want to prevent
     * cell content from being reset during rendering. **This is the default behaviour until 6.0.**
     *
     * Set to `true` to always clear cell content regardless of renderer return value. **This will be default
     * behaviour from 6.0.**
     * @config {Boolean} alwaysClearCell
     * @default false
     * @category Misc
     */
    {
      name: 'alwaysClearCell',
      defaultValue: false
    },
    /**
     * An array of the widgets to append to the column header
     * ```javascript
     * columns : [
     * {
     *     text          : 'Name',
     *     field         : 'name',
     *     flex          : 1,
     *     headerWidgets : [
     *         {
     *             type   : 'button',
     *             text   : 'Add row',
     *             cls    : 'b-raised b-blue',
     *             async onAction() {
     *                 const [newRecord] = grid.store.add({
     *                     name : 'New user'
     *                 });
     *
     *                 await grid.scrollRowIntoView(newRecord);
     *
     *                 await grid.features.cellEdit.startEditing({
     *                     record : newRecord,
     *                     field  : 'name'
     *                 });
     *             }
     *         }
     *     ]
     * }]
     * ```
     * @config {ContainerItemConfig[]} headerWidgets
     * @private
     * @category Misc
     */
    {
      name: 'headerWidgets'
    },
    /**
     * Set to `true` to have the {@link Grid.feature.CellEdit} feature update the record being edited live upon
     * field edit instead of when editing is finished by using `TAB` or `ENTER`
     * @config {Boolean} instantUpdate
     * @category Misc
     */
    {
      name: 'instantUpdate',
      defaultValue: false
    }, {
      name: 'repaintOnResize',
      defaultValue: false
    },
    /**
     * An optional query selector to select a sub element within the cell being
     * edited to align a cell editor's `X` position and `width` to.
     * @config {String} editTargetSelector
     * @category Misc
     */
    'editTargetSelector',
    //endregion
    //region Export
    /**
     * Used by the Export feature. Set to `false` to omit a column from an exported dataset
     * @config {Boolean} exportable
     * @default true
     * @category Export
     */
    {
      name: 'exportable',
      defaultValue: true
    },
    /**
     * Column type which will be used by {@link Grid.util.TableExporter}. See list of available types in
     * TableExporter docs. Returns undefined by default, which means column type should be read from the record
     * field.
     * @config {String} exportedType
     * @category Export
     */
    {
      name: 'exportedType'
    }, {
      name: 'ariaLabel',
      defaultValue: 'L{Column.columnLabel}'
    }, {
      name: 'cellAriaLabel',
      defaultValue: 'L{cellLabel}'
    }
    //endregion
    ];
  }
  // prevent undefined fields from being exposed, to simplify spotting errors
  static get autoExposeFields() {
    return false;
  }
  //endregion
  //region Init
  construct(data, store) {
    var _me$field;
    const me = this;
    me.masterStore = store;
    // Store might be an array
    if (store) {
      me._grid = Array.isArray(store) ? store[0].grid : store.grid;
    }
    me.localizableProperties = Config.mergeMethods.distinct(data.localizableProperties, ['text', 'ariaLabel', 'cellAriaLabel']);
    if (data.localeClass) {
      me.localeClass = data.localeClass;
    }
    super.construct(...arguments);
    // Default value for region is assigned by the ColumnStore in createRecord(), same for `locked`
    // Allow field : null if the column does not rely on a record field.
    // For example the CheckColumn when used by GridSelection.
    if (me.isLeaf && !('field' in me.data)) {
      me.field = '_' + (me.type || '') + ++Column.emptyCount;
      me.noFieldSpecified = true;
    }
    // If our field is a dot separated path, we must use ObjectHelper.getPath to extract our value
    me.hasComplexMapping = (_me$field = me.field) === null || _me$field === void 0 ? void 0 : _me$field.includes('.');
    if (!me.width && !me.flex && !me.children) {
      // Set the width silently because we're in construction.
      me.set({
        width: Column.defaultWidth,
        flex: null
      }, null, true);
    }
    me.headerWidgets && me.initHeaderWidgets(me.headerWidgets);
    if (me.isParent) {
      me.meta.visibleChildren = new Set();
      // Trigger adding expand/collapse button
      if (me.collapsible) {
        me.collapsible = true;
      }
    }
  }
  /**
   * Checks whether the other column is in the same position and configured the same as this Column.
   * @param {Grid.column.Column} other The partner column to check
   * @returns {Boolean} `true` if these two Columns should be kept in sync.
   * @private
   */
  shouldSync(other) {
    // Two columns in different grids should be kept in sync if:
    //  they have the same header text
    //  and they have the same field or same renderer
    //  and they are positioned the same; after an identical set of previous siblings
    return other.isColumn && other.text === this.text && (other.field === this.field || String(other.renderer) === String(this.renderer)) && (!other.previousSibling && !this.previousSibling || other.previousSibling.shouldSync(this.previousSibling));
  }
  get isCollapsible() {
    var _this$children;
    return ((_this$children = this.children) === null || _this$children === void 0 ? void 0 : _this$children.length) > 1 && this.collapsible;
  }
  get collapsed() {
    return this.get('collapsed');
  }
  set collapsed(collapsed) {
    // Avoid triggering redraw
    this.set('collapsed', collapsed, true);
    // This triggers redraw
    this.onCollapseChange(!collapsed);
    this.trigger('toggleCollapse', {
      collapsed
    });
  }
  onCellFocus(location) {
    this.location = location;
    this.updateHeaderAriaLabel(this.localizeProperty('ariaLabel'));
    // Update cell if cell is in the grid
    if (location.rowIndex !== -1) {
      this.updateCellAriaLabel(this.localizeProperty('cellAriaLabel'));
    }
  }
  updateHeaderAriaLabel(headerAriaLabel) {
    DomHelper.setAttributes(this.element, {
      'aria-label': headerAriaLabel
    });
  }
  updateCellAriaLabel(cellAriaLabel) {
    var _this$location, _this$location2;
    if (!((_this$location = this.location) !== null && _this$location !== void 0 && _this$location.isSpecialRow) && (_this$location2 = this.location) !== null && _this$location2 !== void 0 && _this$location2.cell) {
      var _cellAriaLabel;
      if (!((_cellAriaLabel = cellAriaLabel) !== null && _cellAriaLabel !== void 0 && _cellAriaLabel.length)) {
        cellAriaLabel = this.location.column.text;
      }
      DomHelper.setAttributes(this.location.cell, {
        'aria-label': cellAriaLabel
      });
    }
  }
  doDestroy() {
    var _this$data, _this$data$editor, _this$data$editor$des;
    (_this$data = this.data) === null || _this$data === void 0 ? void 0 : (_this$data$editor = _this$data.editor) === null || _this$data$editor === void 0 ? void 0 : (_this$data$editor$des = _this$data$editor.destroy) === null || _this$data$editor$des === void 0 ? void 0 : _this$data$editor$des.call(_this$data$editor);
    this.destroyHeaderWidgets();
    super.doDestroy();
  }
  //endregion
  //region Header widgets
  set headerWidgets(widgets) {
    this.initHeaderWidgets(widgets);
    this.set('headerWidgets', widgets);
  }
  get headerWidgets() {
    return this.get('headerWidgets');
  }
  initHeaderWidgets(widgets) {
    this.destroyHeaderWidgets();
    const headerWidgetMap = this.headerWidgetMap = {};
    for (const config of widgets) {
      const widget = Widget.create({
        owner: this,
        ...config
      });
      headerWidgetMap[widget.ref || widget.id] = widget;
    }
  }
  destroyHeaderWidgets() {
    // Clean up any headerWidgets used
    for (const widget of Object.values(this.headerWidgetMap || {})) {
      var _widget$destroy;
      (_widget$destroy = widget.destroy) === null || _widget$destroy === void 0 ? void 0 : _widget$destroy.call(widget);
    }
  }
  //endregion
  //region Fields
  // Yields the automatic cell tagging class, eg b-number-cell from NumberColumn etc
  static generateAutoCls() {
    const classes = [];
    // Create the auto class for cells owned by this column class
    // For example NumberColumn cells get b-number-cell
    // DurationColumn cells get b-duration-cell b-number-cell
    for (let c = this; c !== Column; c = c.superclass) {
      c.type && c.type !== c.superclass.type && classes.push(`b-${c.type.toLowerCase()}-cell`);
    }
    const columnAutoCls = classes.join(' ');
    (Column.autoClsMap || (Column.autoClsMap = new Map())).set(this, columnAutoCls);
    return columnAutoCls;
  }
  /**
   * Returns the full CSS class set for a cell at the passed {@link Grid.util.Location}
   * as an object where property keys with truthy values denote a class to be added
   * to the cell.
   * @param {Grid.util.Location} cellContext
   * @returns {Object} An object in which property keys with truthy values are used as
   * the class names on the cell element.
   * @internal
   */
  getCellClass(cellContext) {
    var _Column$autoClsMap, _record$fieldMap$colu;
    const {
        record,
        column
      } = cellContext,
      {
        cellCls,
        internalCellCls,
        grid,
        constructor,
        align
      } = column,
      autoCls = ((_Column$autoClsMap = Column.autoClsMap) === null || _Column$autoClsMap === void 0 ? void 0 : _Column$autoClsMap.get(constructor)) || constructor.generateAutoCls(),
      isEditing = cellContext.cell.classList.contains('b-editing'),
      result = {
        [grid.cellCls]: grid.cellCls,
        [autoCls]: autoCls,
        [cellCls]: cellCls,
        [internalCellCls]: internalCellCls,
        'b-cell-dirty': record.isFieldModified(column.field) && (column.compositeField || ((_record$fieldMap$colu = record.fieldMap[column.field]) === null || _record$fieldMap$colu === void 0 ? void 0 : _record$fieldMap$colu.persist) !== false),
        [`b-grid-cell-align-${align}`]: align,
        'b-selected': grid.selectionMode.cell && grid.isCellSelected(cellContext),
        'b-focused': grid.isFocused(cellContext),
        'b-auto-height': column.autoHeight,
        'b-editing': isEditing
      };
    // Check cell CSS should not be applied to group header rows
    if (record.isSpecialRow && result['b-checkbox-selection']) {
      result['b-checkbox-selection'] = false;
    }
    return result;
  }
  get locked() {
    return this.data.region === 'locked';
  }
  set locked(locked) {
    this.region = locked ? 'locked' : 'normal';
  }
  // Children of grouped header always uses same region as the group
  get region() {
    if (!this.parent.isRoot) {
      return this.parent.region;
    }
    return this.get('region');
  }
  set region(region) {
    this.set('region', region);
  }
  // parent headers cannot be sorted by
  get sortable() {
    return this.isLeaf && this.data.sortable;
  }
  set sortable(sortable) {
    this.set('sortable', sortable);
  }
  // parent headers cannot be grouped by
  get groupable() {
    return this.isLeaf && this.data.groupable;
  }
  set groupable(groupable) {
    this.set('groupable', groupable);
  }
  /**
   * The Field to use as editor for this column
   * @private
   * @readonly
   */
  get editor() {
    const me = this;
    let {
      editor
    } = me.data;
    if (editor && !editor.isWidget) {
      // Give frameworks a shot at injecting their own editor, wrapped as a widget
      const result = me.grid.processCellEditor({
        editor,
        field: me.field
      });
      if (result) {
        // Use framework editor
        editor = me.data.editor = result.editor;
      } else {
        if (typeof editor === 'string') {
          editor = {
            type: editor
          };
        }
        // The two configs, default and configured must be deep merged.
        editor = me.data.editor = Widget.create(ObjectHelper.merge(me.defaultEditor, {
          owner: me.grid,
          // Field labels must be present for A11Y purposes, but are clipped out of visibility.
          // Screen readers will be able to access them and announce them.
          label: StringHelper.encodeHtml(me.text)
        }, editor));
      }
    }
    return editor;
  }
  set editor(editor) {
    this.data.editor = editor;
  }
  /**
   * A config object specifying the editor to use to edit this column.
   * @private
   * @readonly
   */
  get defaultEditor() {
    return {
      type: 'textfield',
      name: this.field
    };
  }
  //endregion
  //region Grid, SubGrid & Element
  /**
   * Extracts the value from the record specified by this Column's {@link #config-field} specification
   * in a format that can be used as a value to match by a {@link Grid.feature.Filter filtering} operation.
   *
   * The default implementation returns the {@link #function-getRawValue} value, but this may be
   * overridden in subclasses.
   * @param {Core.data.Model} record The record from which to extract the field value.
   * @returns {*} The value of the referenced field if any.
   */
  getFilterableValue(record) {
    return this.getRawValue(record);
  }
  // Create an ownership hierarchy which links columns up to their SubGrid if no owner injected.
  get owner() {
    return this._owner || this.subGrid;
  }
  set owner(owner) {
    this._owner = owner;
  }
  get grid() {
    var _this$parent;
    return this._grid || ((_this$parent = this.parent) === null || _this$parent === void 0 ? void 0 : _this$parent.grid);
  }
  // Private, only used in tests where standalone Headers are created with no grid
  // from which to lookup the associate SubGrid.
  set subGrid(subGrid) {
    this._subGrid = subGrid;
  }
  /**
   * Get the SubGrid to which this column belongs
   * @property {Grid.view.SubGrid}
   * @readonly
   */
  get subGrid() {
    var _this$grid;
    return this._subGrid || ((_this$grid = this.grid) === null || _this$grid === void 0 ? void 0 : _this$grid.getSubGridFromColumn(this));
  }
  /**
   * Get the element for the SubGrid to which this column belongs
   * @property {HTMLElement}
   * @readonly
   * @private
   */
  get subGridElement() {
    return this.subGrid.element;
  }
  /**
   * The header element for this Column. *Only available after the grid has been rendered*.
   *
   * **Note that column headers are rerendered upon mutation of Column values, so this
   * value is volatile and should not be cached, but should be read whenever needed.**
   * @property {HTMLElement}
   * @readonly
   */
  get element() {
    return this.grid.getHeaderElement(this);
  }
  get previousVisibleSibling() {
    // During move from one region to another, nextSibling might not be wired up to the new next sibling in region.
    // (Because the order in master store did not change)
    const region = this.region;
    let prev = this.previousSibling;
    while (prev && (prev.hidden || prev.region !== region)) {
      prev = prev.previousSibling;
    }
    return prev;
  }
  get nextVisibleSibling() {
    // During move from one region to another, nextSibling might not be wired up to the new next sibling in region.
    // (Because the order in master store did not change)
    const region = this.region;
    let next = this.nextSibling;
    while (next && (next.hidden || next.region !== region)) {
      next = next.nextSibling;
    }
    return next;
  }
  get isLastInSubGrid() {
    return !this.nextVisibleSibling && (this.parent.isRoot || this.parent.isLastInSubGrid);
  }
  get allowDrag() {
    return !this.parent.isRoot || Boolean(this.nextVisibleSibling || this.previousVisibleSibling);
  }
  /**
   * The text wrapping element for this Column. *Only available after the grid has been rendered*.
   *
   * This is the full-width element which *contains* the text-bearing element and any icons.
   *
   * **Note that column headers are rerendered upon mutation of Column values, so this
   * value is volatile and should not be cached, but should be read whenever needed.**
   * @property {HTMLElement}
   * @readonly
   */
  get textWrapper() {
    return DomHelper.getChild(this.element, '.b-grid-header-text');
  }
  /**
   * The text containing element for this Column. *Only available after the grid has been rendered*.
   *
   * **Note that column headers are rerendered upon mutation of Column values, so this
   * value is volatile and should not be cached, but should be read whenever needed.**
   * @property {HTMLElement}
   * @readonly
   */
  get textElement() {
    return DomHelper.down(this.element, '.b-grid-header-text-content');
  }
  /**
   * The child element into which content should be placed. This means where any
   * contained widgets such as filter input fields should be rendered. *Only available after the grid has been
   * rendered*.
   *
   * **Note that column headers are rerendered upon mutation of Column values, so this
   * value is volatile and should not be cached, but should be read whenever needed.**
   * @property {HTMLElement}
   * @readonly
   */
  get contentElement() {
    return DomHelper.down(this.element, '.b-grid-header-children');
  }
  //endregion
  //region Misc properties
  get isSorted() {
    return this.grid.store.sorters.some(s => s.field === this.field);
  }
  get isFocusable() {
    return this.isLeaf;
  }
  static get text() {
    return this.$meta.fields.defaults.text;
  }
  /**
   * Returns header text based on {@link #config-htmlEncodeHeaderText} config value.
   * @returns {String}
   * @internal
   */
  get headerText() {
    return this.htmlEncodeHeaderText ? StringHelper.encodeHtml(this.text) : this.text;
  }
  /**
   * An object which contains a map of the header widgets keyed by their {@link Core.widget.Widget#config-ref ref}.
   * @property {Object<String,Core.widget.Widget>} headerWidgetMap
   * @private
   * @readonly
   */
  //endregion
  //region Show/hide
  get isVisible() {
    return !this.hidden && (!this.parent || this.parent.isVisible);
  }
  /**
   * Hides this column.
   */
  hide(silent = false, hidingParent = false) {
    const me = this,
      {
        parent
      } = me;
    // Reject non-change
    if (!me.hidden) {
      me.hidden = true;
      if (parent && !parent.isRoot && !parent.isTogglingAll) {
        // check if all sub columns are hidden, if so hide parent
        const anyVisible = parent.children.some(child => child.hidden !== true);
        if (!anyVisible && !parent.hidden) {
          silent = true; // hiding parent will trigger event
          parent.hide();
        }
      }
      if (me.isParent) {
        me.children.forEach(child => child.hide(true, true));
      }
      // Keep state when hiding parent, to be able to restore when showing
      else if (!parent.isRoot) {
        parent.meta.visibleChildren[hidingParent ? 'add' : 'delete'](me);
      }
      if (!silent) {
        me.stores.forEach(store => store.trigger('columnHide', {
          column: me
        }));
      }
    }
  }
  /**
   * Shows this column.
   */
  show(silent = false) {
    const me = this,
      {
        parent
      } = me;
    // Reject non-change
    if (me.hidden) {
      me.hidden = false;
      if (parent !== null && parent !== void 0 && parent.hidden) {
        parent.show();
      }
      if (me.isParent) {
        var _me$meta$visibleChild;
        // Only show children
        (_me$meta$visibleChild = me.meta.visibleChildren) === null || _me$meta$visibleChild === void 0 ? void 0 : _me$meta$visibleChild.forEach(child => child.show(true));
      }
      // event is triggered on chained stores
      if (!silent) {
        me.stores.forEach(store => store.trigger('columnShow', {
          column: me
        }));
      }
    }
  }
  /**
   * Toggles the column visibility.
   * @param {Boolean} [force] Set to true (visible) or false (hidden) to force a certain state
   */
  toggle(forceVisible) {
    if (this.hidden && forceVisible === undefined || forceVisible === true) {
      return this.show();
    }
    if (!this.hidden && forceVisible === undefined || forceVisible === false) {
      return this.hide();
    }
  }
  /**
   * Toggles the column visibility of all children of a parent column.
   * @param {Grid.column.Column[]} [columns] The set of child columns to toggle, defaults to all children
   * @param {Boolean} [force] Set to true (visible) or false (hidden) to force a certain state
   */
  toggleChildren(columns = this.children, force = undefined) {
    var _me$grid$columns, _me$grid$columns2;
    const me = this;
    (_me$grid$columns = me.grid.columns) === null || _me$grid$columns === void 0 ? void 0 : _me$grid$columns.beginBatch();
    me.isTogglingAll = true;
    columns.forEach(childColumn => childColumn.toggle(force));
    me.isTogglingAll = false;
    (_me$grid$columns2 = me.grid.columns) === null || _me$grid$columns2 === void 0 ? void 0 : _me$grid$columns2.endBatch();
  }
  /**
   * Toggles the collapsed state of the column. Based on the {@link #config-collapseMode}, this either hides all
   * but the first child column, or toggles the visibility state of all children (if you want to have a special
   * column shown in collapsed mode).
   *
   * Only applicable for columns with child columns.
   * @private
   * @param {Boolean} [force] Set to true (expanded) or false (collapsed) to force a certain state
   */
  onCollapseChange(force = undefined) {
    const me = this;
    if (me.collapseMode === 'toggleAll') {
      me.toggleChildren();
    } else {
      var _me$grid$columns3, _me$grid$columns4;
      const {
        firstChild
      } = me;
      // For flexed child column, stamp a width on it in collapsed state
      if (firstChild.flex != null && me.collapsed) {
        firstChild.oldFlex = firstChild.flex;
        firstChild.width = firstChild.element.offsetWidth;
      } else if (!me.collapsed && firstChild.oldFlex) {
        // For previously flexed child column, restore the flex value;
        firstChild.flex = firstChild.oldFlex;
        firstChild.oldFlex = null;
      }
      (_me$grid$columns3 = me.grid.columns) === null || _me$grid$columns3 === void 0 ? void 0 : _me$grid$columns3.beginBatch();
      me.isTogglingAll = true;
      me.children.slice(1).forEach(childColumn => childColumn.toggle(force));
      me.isTogglingAll = false;
      (_me$grid$columns4 = me.grid.columns) === null || _me$grid$columns4 === void 0 ? void 0 : _me$grid$columns4.endBatch();
    }
  }
  set collapsible(collapsible) {
    const me = this;
    me.set('collapsible', collapsible);
    if (me.isParent) {
      const {
        headerWidgets = []
      } = me;
      if (collapsible) {
        headerWidgets.push({
          type: 'button',
          ref: 'collapseExpand',
          toggleable: true,
          pressed: me.collapsed,
          icon: `b-icon-collapse-${me.grid.rtl ? 'right' : 'left'}`,
          pressedIcon: `b-icon-collapse-${me.grid.rtl ? 'left' : 'right'}`,
          cls: 'b-grid-header-collapse-button b-transparent',
          onToggle: ({
            pressed
          }) => me.collapsed = pressed
        });
      } else {
        const index = headerWidgets.findIndex(w => w.ref === 'collapseExpand');
        index > -1 && headerWidgets.splice(index, 1);
      }
      me.headerWidgets = headerWidgets;
      if (me.collapsed) {
        me.onCollapseChange(false);
      }
    }
  }
  get collapsible() {
    return this.get('collapsible');
  }
  //endregion
  //region Index & id
  /**
   * Generates an id for the column when none is set. Generated ids are 'col1', 'col2' and so on. If a field is
   * specified (as it should be in most cases) the field name is used instead: 'name1', 'age2' ...
   * @private
   * @returns {String}
   */
  generateId() {
    if (!Column.generatedIdIndex) {
      Column.generatedIdIndex = 0;
    }
    return (this.field ? this.field.replace(/\./g, '-') : 'col') + ++Column.generatedIdIndex;
  }
  /**
   * Index among all flattened columns
   * @property {Number}
   * @readOnly
   * @internal
   */
  get allIndex() {
    return this.masterStore.indexOf(this);
  }
  //endregion
  //region Width
  // Returns size in pixels for measured value
  measureSize(value) {
    var _this$subGrid;
    return DomHelper.measureSize(value, (_this$subGrid = this.subGrid) === null || _this$subGrid === void 0 ? void 0 : _this$subGrid.element);
  }
  /**
   * Returns minimal width in pixels for applying to style according to the current `width` and `minWidth`.
   * @internal
   */
  get calcMinWidth() {
    const {
      width,
      minWidth
    } = this.data;
    if (validWidth(width) && validWidth(minWidth)) {
      return Math.max(parseInt(width) || 0, parseInt(minWidth) || 0);
    } else {
      return width;
    }
  }
  /**
   * Get/set columns width in px. If column uses flex, width will be undefined.
   * Setting a width on a flex column cancels out flex.
   *
   * **NOTE:** Grid might be configured to always stretch the last column, in which case the columns actual width
   * might deviate from the configured width.
   *
   * ```javascript
   * let grid = new Grid({
   *     appendTo : 'container',
   *     height   : 200,
   *     width    : 400,
   *     columns  : [{
   *         text  : 'First column',
   *         width : 100
   *     }, {
   *         text  : 'Last column',
   *         width : 100 // last column in the grid is always stretched to fill the free space
   *     }]
   * });
   *
   * grid.columns.last.element.offsetWidth; // 300 -> this points to the real element width
   * ```
   * @property {Number|String}
   */
  get width() {
    return this.data.width;
  }
  set width(width) {
    const data = {
      width
    };
    if (width && 'flex' in this.data) {
      data.flex = null; // remove flex when setting width to enable resizing flex columns
    }

    this.set(data);
  }
  set flex(flex) {
    const data = {
      flex
    };
    if (flex && 'width' in this.data) {
      data.width = null; // remove width when setting flex
    }

    this.set(data);
  }
  get flex() {
    return this.data.flex;
  }
  // This method is used to calculate minimum row width for edge and safari
  // It calculates minimum width of the row taking column hierarchy into account
  calculateMinWidth() {
    const me = this,
      width = me.measureSize(me.width),
      minWidth = me.measureSize(me.minWidth);
    let minChildWidth = 0;
    if (me.children) {
      minChildWidth = me.children.reduce((result, column) => {
        return result + column.calculateMinWidth();
      }, 0);
    }
    return Math.max(width, minWidth, minChildWidth);
  }
  /**
   * Resizes the column to match the widest string in it. By default it also measures the column header, this
   * behaviour can be configured by setting {@link Grid.view.Grid#config-resizeToFitIncludesHeader}.
   *
   * Called internally when you double click the edge between
   * column headers, but can also be called programmatically. For performance reasons it is limited to checking 1000
   * rows surrounding the current viewport.
   *
   * @param {Number|Number[]} widthMin Minimum allowed width. If content width is less than this, this width is used
   * instead. If this parameter is an array, the first element is `widthMin` and the seconds is `widthMax`.
   * @param {Number} widthMax Maximum allowed width. If the content width is greater than this number, this width
   * is used instead.
   */
  resizeToFitContent(widthMin, widthMax, batch = false) {
    const me = this,
      {
        grid,
        element,
        fitMode
      } = me,
      {
        rowManager,
        store
      } = grid,
      {
        count
      } = store;
    if (count <= 0 || me.fitMode === 'none' || !me.fitMode) {
      return;
    }
    const [row] = rowManager.rows,
      {
        rowElement,
        cellElement
      } = grid.beginGridMeasuring(),
      cellContext = new Location({
        grid,
        column: me,
        id: null
      });
    let maxWidth = 0,
      start,
      end,
      i,
      record,
      value,
      length,
      longest = {
        length: 0,
        record: null
      };
    // Fake element data to be able to use Row#renderCell()
    cellElement._domData = {
      columnId: me.id,
      row,
      rowElement
    };
    cellContext._cell = cellElement;
    cellContext.updatingSingleRow = true;
    cellContext.isMeasuring = true;
    // Clear cellElement, since it is being reused between columns
    cellElement.innerHTML = '';
    // Measure header unless configured not to
    if (grid.resizeToFitIncludesHeader) {
      // Cache the padding
      if (!grid.$headerPadding) {
        const style = globalThis.getComputedStyle(element);
        grid.$headerPadding = parseInt(style.paddingLeft);
      }
      // Grab the header text content element
      const headerText = element.querySelector('.b-grid-header-text-content');
      // Restyle it to shrinkwrap its text, measure and then restore
      headerText.style.cssText = 'flex: none; width: auto';
      maxWidth = headerText.offsetWidth + grid.$headerPadding * 2 + 2; // +2 to avoid overflow ellipsis
      headerText.style.cssText = '';
    }
    // If it's a very large dataset, measure the maxWidth of the field in the 1000 rows
    // surrounding the rendered block.
    if (count > 1000) {
      start = Math.max(Math.min(rowManager.topIndex + Math.round(rowManager.rowCount / 2) - 500, count - 1000), 0);
      end = start + 1000;
    } else {
      start = 0;
      end = count;
    }
    for (i = start; i < end; i++) {
      record = store.getAt(i);
      value = me.getRawValue(record);
      // In value mode we determine the record with the longest value, no rendering involved
      if (fitMode === 'value') {
        length = String(value).length;
      }
      // In exact and textContent modes we have to render the records
      else {
        cellContext._record = longest.record;
        cellContext._id = record.id;
        cellContext._rowIndex = i;
        row.renderCell(cellContext);
        // Reading textContent is "cheap", it does not require a layout
        if (fitMode === 'textContent') {
          length = cellElement.textContent.length;
        }
        // Using exact mode, measure the cell = expensive
        else {
          const width = cellElement.offsetWidth;
          if (width > maxWidth) {
            maxWidth = width;
          }
        }
      }
      if (length > longest.length) {
        longest = {
          record,
          length,
          rowIndex: i
        };
      }
    }
    // value mode and textContent mode both required us to render and measure the record determined to be the
    // longest above
    if (longest.length > 0 && (fitMode === 'value' || fitMode === 'textContent')) {
      cellContext._record = longest.record;
      cellContext._id = longest.record.id;
      cellContext._rowIndex = longest.rowIndex;
      row.renderCell(cellContext);
      maxWidth = Math.max(maxWidth, cellElement.offsetWidth);
    }
    if (Array.isArray(widthMin)) {
      [widthMin, widthMax] = widthMin;
    }
    maxWidth = Math.max(maxWidth, widthMin || 0);
    maxWidth = Math.min(maxWidth, widthMax || 1e6); // 1 million px default max
    // Batch mode saves a little time by not removing the measuring elements between columns
    if (!batch) {
      grid.endGridMeasuring();
    }
    me.width = me.maxWidth ? maxWidth = Math.min(maxWidth, me.maxWidth) : maxWidth;
    return maxWidth;
  }
  //endregion
  //region State
  /**
   * Get column state, used by State mixin
   * @private
   */
  getState() {
    const me = this,
      state = {
        id: me.id,
        // State should only store column attributes which user can modify via UI (except column index).
        // User can hide column, resize or move it to neighbor region
        hidden: me.hidden,
        region: me.region,
        locked: me.locked
      };
    if (!me.children) {
      state[me.flex ? 'flex' : 'width'] = me.flex || me.width;
    }
    if (me.isCollapsible) {
      state.collapsed = me.collapsed;
    }
    return state;
  }
  /**
   * Apply state to column, used by State mixin
   * @private
   */
  applyState(state) {
    const me = this;
    me.beginBatch();
    if ('locked' in state) {
      me.locked = state.locked;
    }
    if ('width' in state) {
      me.width = state.width;
    }
    if ('flex' in state) {
      me.flex = state.flex;
    }
    if ('width' in state && me.flex) {
      me.flex = undefined;
    } else if ('flex' in state && me.width) {
      me.width = undefined;
    }
    if ('region' in state) {
      me.region = state.region;
    }
    me.endBatch();
    if ('hidden' in state) {
      me.toggle(state.hidden !== true);
    }
    if ('collapsed' in state) {
      me.collapsed = state.collapsed;
    }
  }
  //endregion
  //region Other
  remove() {
    const {
        subGrid,
        grid
      } = this,
      focusedCell = subGrid && (grid === null || grid === void 0 ? void 0 : grid.focusedCell);
    // Prevent errors when removing the column that the owning grid has registered as focused.
    if ((focusedCell === null || focusedCell === void 0 ? void 0 : focusedCell.columnId) === this.id) {
      // Focus is in the grid, navigate before column is removed
      if (grid.owns(DomHelper.getActiveElement(grid))) {
        grid.navigateRight();
      }
      // Focus not in the grid, bump the focused cell pointer to the next visible column
      // for when focus returns so it can go as close as possible.
      else {
        grid._focusedCell = new Location({
          grid,
          rowIndex: focusedCell.rowIndex,
          column: subGrid.columns.getAdjacentVisibleLeafColumn(this.id, true, true)
        });
      }
    }
    super.remove();
  }
  /**
   * Extracts the value from the record specified by this Column's {@link #config-field} specification.
   *
   * This will work if the field is a dot-separated path to access fields in associated records, eg
   *
   * ```javascript
   *  field : 'resource.calendar.name'
   * ```
   *
   * **Note:** This is the raw field value, not the value returned by the {@link #config-renderer}.
   * @param {Core.data.Model} record The record from which to extract the field value.
   * @returns {*} The value of the referenced field if any.
   */
  getRawValue(record) {
    if (this.hasComplexMapping) {
      return ObjectHelper.getPath(record, this.field);
    }
    // Engine can change field value to null, in which case cell will render previous record value,
    // before project commit
    return record[this.field];
  }
  /**
   * Refresh the cell for supplied record in this column, if that cell is rendered.
   * @param {Core.data.Model} record Record used to get row to update the cell in
   */
  refreshCell(record) {
    this.grid.rowManager.refreshCell(record, this.id);
  }
  /**
   * Clear cell contents. Base implementation which just sets innerHTML to blank string.
   * Should be overridden in subclasses to clean up for examples widgets.
   * @param {HTMLElement} cellElement
   * @internal
   */
  clearCell(cellElement) {
    cellElement.innerHTML = '';
    delete cellElement._content;
  }
  /**
   * Override in subclasses to allow/prevent editing of certain rows.
   * @param {Core.data.Model} record
   * @internal
   */
  canEdit(record) {
    // the record can decide which column is editable
    if (record.isEditable) {
      const isEditable = record.isEditable(this.field);
      // returns undefined for unknown field
      if (isEditable !== undefined) {
        return isEditable;
      }
    }
    return true;
  }
  /**
   * Insert a child column(s) before an existing child column. Returns `null` if the parent column is
   * {@link #config-sealed}
   * @param {Core.data.Model|Core.data.Model[]} childColumn Column or array of columns to insert
   * @param {Core.data.Model} [before] Optional column to insert before, leave out to append to the end
   * @param {Boolean} [silent] Pass `true` to not trigger events during insert
   * @returns {Core.data.Model|Core.data.Model[]|null}
   * @category Parent & children
   */
  insertChild(childColumn, before = null, silent = false) {
    childColumn = Array.isArray(childColumn) ? childColumn : [childColumn];
    // If user dragged out only visible child of collapsed parent, make next sibling visible
    childColumn.forEach(col => {
      const {
        parent
      } = col;
      if (parent !== null && parent !== void 0 && parent.collapsed && col === parent.firstChild && parent.children.length > 1 && parent.children.filter(child => !child.hidden).length === 1) {
        col.nextSibling.hidden = false;
      }
    });
    return this.sealed && !this.inProcessChildren ? null : super.insertChild(...arguments);
  }
  /**
   * Override in subclasses to prevent this column from being filled with the {@link Grid.feature.FillHandle} feature
   * @param {Object} data Object containing information about current cell and fill value
   * @param {Grid.util.Location} data.cell Current cell data
   * @param {Grid.util.Location[]} data.range Range from where to calculate values
   * @param {Core.data.Model} data.record Current cell record
   * @returns {Boolean}
   * @internal
   */
  canFillValue() {
    return true;
  }
  //endregion
  // This function is not meant to be called by any code other than Base#getCurrentConfig().
  // It extracts the current configs (fields) for the column, with special handling for sortable, editor, renderer and
  // headerRenderer
  getCurrentConfig(options) {
    var _this$sortable;
    const result = super.getCurrentConfig(options);
    // Use unbound sort fn
    if ((_this$sortable = this.sortable) !== null && _this$sortable !== void 0 && _this$sortable.originalSortFn) {
      result.sortable = this.sortable.originalSortFn;
    }
    // Don't include internalRenderer in current config
    if (result.renderer === this.internalRenderer) {
      delete result.renderer;
    }
    // Same for headerRenderer
    if (result.headerRenderer === this.internalHeaderRenderer) {
      delete result.headerRenderer;
    }
    delete result.ariaLabel;
    delete result.cellAriaLabel;
    return result;
  }
}
// Registered in ColumnStore as we can't have this in Column due to circular dependencies
Column.emptyCount = 0;
Column.defaultWidth = 100;
Column.exposeProperties();
Column._$name = 'Column';

/**
 * @module Grid/data/ColumnStore
 */
const columnDefinitions = {
    boolean: {
      type: 'check'
    },
    date: {
      type: 'date'
    },
    integer: {
      type: 'number',
      format: {
        maximumFractionDigits: 0
      }
    },
    number: {
      type: 'number'
    }
  },
  lockedColumnSorters = [{
    field: 'region'
  }];
/**
 * A store specialized in handling columns. Used by the Grid to hold its columns and used as a chained store by each SubGrid
 * to hold theirs. Should not be instanced directly, instead access it through `grid.columns` or `subGrid.columns`
 *
 * ```javascript
 * // resize first column
 * grid.columns.first.width = 200;
 *
 * // remove city column
 * grid.columns.get('city').remove();
 *
 * // add new column
 * grid.columns.add({text : 'New column'});
 *
 * // add new column to specific region (SubGrid)
 * grid.columns.add({text : 'New column', region : 'locked'});
 *
 * // add new column to 'locked' region (SubGrid)
 * grid.columns.add({text : 'New column', locked : true});
 * ```
 *
 * @extends Core/data/Store
 */
class ColumnStore extends Localizable(Store) {
  //region Events
  /**
   * Fires when a column is shown.
   * @event columnShow
   * @param {Grid.data.ColumnStore} source The store which triggered the event.
   * @param {Grid.column.Column} column The column which status has been changed.
   */
  /**
   * Fires when a column has been hidden.
   * @event columnHide
   * @param {Grid.data.ColumnStore} source The store which triggered the event.
   * @param {Grid.column.Column} column The column which status has been changed.
   */
  //endregion
  static get defaultConfig() {
    return {
      modelClass: Column,
      tree: true,
      /**
       * Automatically adds a field definition to the store used by the Grid when adding a new Column displaying a
       * non-existing field.
       *
       * To enable this behaviour:
       *
       * ```javascript
       * const grid = new Grid({
       *     columns : {
       *         autoAddField : true,
       *         data         : [
       *             // Column definitions here
       *         ]
       *     }
       * }
       *
       * @config {Boolean}
       * @default
       */
      autoAddField: false,
      /**
       * `ColumnStore` uses `syncDataOnLoad` by default (with `threshold : 1`), to ensure good performance when
       * binding to columns in frameworks.
       *
       * See {@link Core/data/Store#config-syncDataOnLoad} for more information.
       *
       * @config {Boolean|SyncDataOnLoadOptions}
       * @default true
       * @readonly
       */
      syncDataOnLoad: {
        threshold: 1
      },
      // Locked columns must sort to before non-locked
      sorters: lockedColumnSorters,
      // Make sure regions stick together when adding columns
      reapplySortersOnAdd: true
    };
  }
  construct(config) {
    const me = this;
    // Consequences of ColumnStore construction can cause reading of grid.columns
    // so set the property early.
    if (config.grid) {
      config.grid._columnStore = me;
      me.id = `${config.grid.id}-columns`;
      // Visible columns must be invalidated on expand/collapse
      config.grid.ion({
        subGridCollapse: 'clearSubGridCaches',
        subGridExpand: 'clearSubGridCaches',
        thisObj: me
      });
    }
    super.construct(config);
    // So that we can invalidate cached collections which take computing so that we compute them
    // only when necessary. For example when asking for the visible leaf columns, we do not want
    // to compute that each time.
    me.ion({
      change: me.onStoreChange,
      sort: () => me.updateChainedStores(),
      thisObj: me,
      prio: 1
    });
  }
  // get modelClass() {
  //     return this._modelClass;
  // }
  //
  // set modelClass(ClassDef) {
  //     this._modelClass = ClassDef;
  // }
  doDestroy() {
    const allColumns = [];
    if (!this.isChained) {
      this.traverse(column => allColumns.push(column));
    }
    super.doDestroy();
    // Store's destroy unjoins all records. Destroy all columns *after* that.
    if (!this.isChained) {
      allColumns.forEach(column => column.destroy());
    }
  }
  // Overridden because the flat collection only contains top level columns,
  // not leaves - group columns are *not* expanded.
  /**
   * Get column by id.
   * @param {String|Number} id
   * @returns {Grid.column.Column}
   */
  getById(id) {
    return super.getById(id) || this.idRegister[id];
  }
  forEach(fn, thisObj = this) {
    // Override to omit root
    this.traverseWhile((n, i) => fn.call(thisObj, n, i), true);
  }
  get totalFixedWidth() {
    let result = 0;
    for (const col of this) {
      if (!col.hidden) {
        // if column has children (grouped header) use they to width increment
        if (col.children) {
          col.children.forEach(childCol => result += this.calculateFixedWidth(childCol));
        } else {
          result += this.calculateFixedWidth(col);
        }
      }
    }
    return result;
  }
  get hasFlex() {
    return this.visibleColumns.some(column => column.flex);
  }
  calculateFixedWidth(column) {
    if (column.flex) {
      return column.measureSize(Column.defaultWidth);
    } else {
      return Math.max(column.measureSize(column.width), column.measureSize(column.minWidth));
    }
  }
  /**
   * Returns the top level columns. If using grouped columns, this is the top level columns. If no grouped
   * columns are being used, this is the leaf columns.
   * @property {Grid.column.Column[]}
   * @readonly
   */
  get topColumns() {
    return this.isChained ? this.masterStore.rootNode.children.filter(this.chainedFilterFn) : this.rootNode.children;
  }
  /**
   * Returns the visible leaf headers which drive the rows' cell content.
   * @property {Grid.column.Column[]}
   * @readonly
   */
  get visibleColumns() {
    const me = this;
    if (!me._visibleColumns) {
      me._visibleColumns = me.leaves.filter(column => column.isVisible && (!column.subGrid || !column.subGrid.collapsed));
    }
    return me._visibleColumns;
  }
  onStoreChange({
    action,
    changes
  }) {
    // no need to clear cache while resizing, or if column changes name
    if (action === 'update' && !('hidden' in changes)) {
      return;
    }
    this.clearCaches();
  }
  clearSubGridCaches({
    subGrid
  }) {
    subGrid.columns.clearCaches();
    this.clearCaches();
  }
  clearCaches() {
    var _this$masterStore;
    this._visibleColumns = null;
    (_this$masterStore = this.masterStore) === null || _this$masterStore === void 0 ? void 0 : _this$masterStore.clearCaches();
  }
  onMasterDataChanged(event) {
    super.onMasterDataChanged(event);
    // If master store has changes we also need to clear cached columns, in case a column was hidden
    // no need to clear cache while resizing, or if column changes name
    if (event.action !== 'update' || 'hidden' in event.changes) {
      this.clearCaches();
    }
  }
  getAdjacentVisibleLeafColumn(columnOrId, next = true, wrap = false) {
    const columns = this.visibleColumns,
      column = columnOrId instanceof Column ? columnOrId : this.getById(columnOrId);
    let idx = columns.indexOf(column) + (next ? 1 : -1);
    // If we walked off either end, wrap if directed to do so,
    // otherwise, return null;
    if (!columns[idx]) {
      if (wrap) {
        idx = next ? 0 : columns.length - 1;
      } else {
        return null;
      }
    }
    return columns[idx];
  }
  /**
   * Bottom columns are the ones displayed in the bottom row of a grouped header, or all columns if not using a grouped
   * header. They are the columns that actually display any data.
   * @property {Grid.column.Column[]}
   * @readonly
   */
  get bottomColumns() {
    return this.leaves;
  }
  /**
   * Get column by field. To be sure that you are getting exactly the intended column, use {@link Core.data.Store#function-getById Store#getById()} with the
   * columns id instead.
   * @param {String} field Field name
   * @returns {Grid.column.Column}
   */
  get(field) {
    return this.findRecord('field', field, true);
  }
  /**
   * Used internally to create a new record in the store. Creates a column of the correct type by looking up the
   * specified type among registered columns.
   * @private
   */
  createRecord(data) {
    var _store$modelClass, _store$modelClass$fie;
    const {
        grid = {}
      } = this,
      // Some ColumnStore tests lacks Grid
      {
        store
      } = grid,
      dataField = store === null || store === void 0 ? void 0 : (_store$modelClass = store.modelClass) === null || _store$modelClass === void 0 ? void 0 : (_store$modelClass$fie = _store$modelClass.fieldMap) === null || _store$modelClass$fie === void 0 ? void 0 : _store$modelClass$fie[data.field];
    let columnClass = this.modelClass;
    // Use the DataField's column definition as a default into which the incoming data is merged
    if (dataField !== null && dataField !== void 0 && dataField.column) {
      data = Objects.merge({}, dataField.column, data);
    }
    if (data.type) {
      columnClass = ColumnStore.getColumnClass(data.type);
      if (!columnClass) {
        throw new Error(`Column type '${data.type}' not registered`);
      }
    }
    if (data.locked) {
      data.region = 'locked';
      delete data.locked;
    }
    const column = new columnClass(data, this);
    // Doing this after construction, in case the columnClass has a default value for region (Schedulers
    // TimeAxisColumn has)
    if (!column.data.region) {
      column.data.region = grid.defaultRegion || 'normal';
    }
    // Add missing fields to Grids stores model
    if (this.autoAddField && !column.noFieldSpecified && store && !dataField) {
      let fieldDefinition = column.field;
      // Some columns define the type to use for new fields (date, number etc)
      if (column.constructor.fieldType) {
        fieldDefinition = {
          name: column.field,
          type: column.constructor.fieldType
        };
      }
      store.modelClass.addField(fieldDefinition);
    }
    return column;
  }
  /**
   * indexOf extended to also accept a columns field, for backward compatibility.
   * ```
   * grid.columns.indexOf('name');
   * ```
   * @param {Core.data.Model|String} recordOrId
   * @returns {Number}
   */
  indexOf(recordOrId) {
    if (recordOrId == null) {
      return -1;
    }
    const index = super.indexOf(recordOrId);
    if (index > -1) return index;
    // no record found by id, find by field since old code relies on that instead of id
    return this.records.findIndex(r => r.field === recordOrId);
  }
  /**
   * Checks if any column uses autoHeight
   * @internal
   * @property {Boolean}
   * @readonly
   */
  get usesAutoHeight() {
    return this.find(column => column.autoHeight);
  }
  /**
   * Checks if any flex column uses autoHeight
   * @internal
   * @property {Boolean}
   * @readonly
   */
  get usesFlexAutoHeight() {
    return this.find(column => column.autoHeight && column.flex != null);
  }
  // Let syncDataOnLoad match on id, field or type (in that order)
  resolveSyncNode(rawData) {
    if (rawData.id) {
      return super.resolveSyncNode(rawData);
    }
    if (rawData.field) {
      return {
        id: rawData.field,
        node: this.allRecords.find(r => r.field === rawData.field)
      };
    }
    if (rawData.type) {
      return {
        id: rawData.type,
        node: this.allRecords.find(r => r.type === rawData.type)
      };
    }
    return {
      id: null,
      node: null
    };
  }
  //region Column types
  /**
   * Call from custom column to register it with ColumnStore. Required to be able to specify type in column config.
   * @param {Function} columnClass The {@link Grid.column.Column} subclass to register.
   * @param {Boolean} simpleRenderer Pass `true` if its default renderer does *not* use other fields from the passed
   * record than its configured {@link Grid.column.Column#config-field}. This enables more granular cell updating
   * upon record mutation.
   * @example
   * // create and register custom column
   * class CustomColumn {
   *  static get type() {
   *      return 'custom';
   *  }
   * }
   * ColumnStore.registerColumnType(CustomColumn, true);
   * // now possible to specify in column config
   * let grid = new Grid({
   *   columns: [
   *     { type: 'custom', field: 'id' }
   *   ]
   * });
   */
  static registerColumnType(columnClass, simpleRenderer = false) {
    columnClass.simpleRenderer = simpleRenderer;
    (ColumnStore.columnTypes || (ColumnStore.columnTypes = {}))[columnClass.type] = columnClass;
  }
  /**
   * Returns registered column class for specified type.
   * @param type Type name
   * @returns {Grid.column.Column}
   * @internal
   */
  static getColumnClass(type) {
    return ColumnStore.columnTypes && ColumnStore.columnTypes[type];
  }
  /**
   * Generates a <strong>new </strong> {@link Grid.column.Column} instance which may be subsequently added to this
   * store to represent the passed {@link Core.data.field.DataField} of the owning Grid's store.
   * @param {Core.data.field.DataField|String} dataField The {@link Core.data.field.DataField field}
   * instance or field name to generate a new {@link Grid.column.Column} for.
   * @param {Object} [defaults] Defaults to apply to the new column.
   * @returns {Grid.column.Column} A new Column which will render and edit the field correctly.
   * @example
   * // Add column for the "team" field.
   * grid.columns.add(grid.columns.generateColumnForField('team', {
   *     width : 200
   * }));
   * @internal
   */
  generateColumnForField(dataField, defaults) {
    if (typeof dataField === 'string' && this.grid) {
      var _this$grid$store;
      dataField = (_this$grid$store = this.grid.store) === null || _this$grid$store === void 0 ? void 0 : _this$grid$store.modelClass.fieldMap[dataField];
    }
    let column = dataField.column || columnDefinitions[dataField.type] || {};
    // Upgrade string to be the column tyope
    if (typeof column === 'string') {
      column = {
        type: column
      };
    }
    // Configure over defaults
    column = Object.assign({
      text: dataField.text || StringHelper.separate(dataField.name),
      field: dataField.name
    }, defaults, column);
    // Special formatting for columns which represent number and integer fields.
    if (dataField.precision != null) {
      column.format.maximumFractionDigits = dataField.precision;
    }
    if (dataField.columnType) {
      column.type = dataField.columnType;
    }
    // Upgrade object to a Column instance.
    return this.createRecord(column);
  }
  //endregion
}
/**
 * Custom {@link Grid.data.ColumnStore} event which triggers when a column is resized, i.e. its width has been changed
 *
 * @param {Function} handler
 * @param {Object} [thisObj]
 */
const columnResizeEvent = (handler, thisObj) => ({
  update: ({
    store,
    record,
    changes
  }) => {
    let result = true;
    if ('width' in changes || 'minWidth' in changes || 'maxWidth' in changes || 'flex' in changes) {
      result = handler.call(thisObj, {
        store,
        record,
        changes
      });
    }
    return result;
  }
});
// Can't have this in Column due to circular dependencies
ColumnStore.registerColumnType(Column, true);
ColumnStore._$name = 'ColumnStore';

/**
 * @module Grid/column/WidgetColumn
 */
/**
 * A column that displays widgets in the grid cells.
 *
 * {@inlineexample Grid/column/WidgetColumn.js}
 *
 * ```javascript
 * new Grid({
 *     appendTo : document.body,
 *
 *     columns : [
 *         {
 *              type: 'widget',
 *              text: 'Name',
 *              widgets: [
 *                  { type: 'textfield', name : 'firstName' },
 *                  { type: 'textfield', name : 'lastName' }
 *               ]
 *         }
 *     ]
 * });
 * ```
 *
 * If you use {@link Core.widget.Field Fields} inside this column, the field widget can optionally bind its value to a
 * field in the data model using the {@link Core/widget/Field#config-name} (as shown in the snippet above). This will
 * provide two-way data binding and update the underlying row record as you make changes in the field.
 *
 * If you use a {@link Core.widget.Button} and want it to display the value from the cell as its text, set its
 * {@link Core/widget/Widget#config-defaultBindProperty} to `'text'`:
 *
 * ```javascript
 * new Grid({
 *     columns : [
 *         {
 *              type: 'widget',
 *              widgets: [
 *                  { type: 'button', name : 'age', defaultBindProperty : 'text' },
 *               ]
 *         }
 *     ]
 * });
 * ```
 *
 * There is no `editor` provided. It is the configured widget's responsibility to provide editing if needed.
 *
 * @extends Grid/column/Column
 * @classType widget
 * @column
 */
class WidgetColumn extends Column {
  //region Config
  static $name = 'WidgetColumn';
  static type = 'widget';
  static fields = [
  /**
   * An array of {@link Core.widget.Widget} config objects
   * @config {ContainerItemConfig[]} widgets
   * @category Common
   */
  'widgets'];
  /**
   * A renderer function, which gives you access to render data like the current `record`, `cellElement` and the
   * {@link #config-widgets} of the column. See {@link #config-renderer}
   * for more information.
   *
   * ```javascript
   * new Grid({
   *     columns : [
   *         {
   *              type: 'check',
   *              field: 'allow',
   *              // In the column renderer, we get access to the record and column widgets
   *              renderer({ record, widgets }) {
   *                  // Hide checkboxes in certain rows
   *                  widgets[0].hidden = record.readOnly;
   *              }
   *         }
   *     ]
   * });
   * ```
   *
   * @param {Object} renderData Object containing renderer parameters
   * @param {HTMLElement|null} [renderData.cellElement] Cell element, for adding CSS classes, styling etc.
   *        Can be `null` in case of export
   * @param {*} renderData.value Value to be displayed in the cell
   * @param {Core.data.Model} renderData.record Record for the row
   * @param {Grid.column.Column} renderData.column This column
   * @param {Core.widget.Widget[]} renderData.widgets An array of the widgets rendered into this cell
   * @param {Grid.view.Grid} renderData.grid This grid
   * @param {Grid.row.Row} [renderData.row] Row object. Can be null in case of export. Use the
   * {@link Grid.row.Row#function-assignCls row's API} to manipulate CSS class names.
   * @param {Object} [renderData.size] Set `size.height` to specify the desired row height for the current row.
   *        Largest specified height is used, falling back to configured {@link Grid/view/Grid#config-rowHeight}
   *        in case none is specified. Can be null in case of export
   * @param {Number} [renderData.size.height] Set this to request a certain row height
   * @param {Number} [renderData.size.configuredHeight] Row height that will be used if none is requested
   * @param {Boolean} [renderData.isExport] True if record is being exported to allow special handling during export
   * @param {Boolean} [renderData.isMeasuring] True if the column is being measured for a `resizeToFitContent`
   *        call. In which case an advanced renderer might need to take different actions.
   * @config {Function} renderer
   * @category Rendering
   */
  static get defaults() {
    return {
      filterable: false,
      sortable: false,
      editor: false,
      searchable: false,
      fitMode: false,
      alwaysClearCell: false
    };
  }
  //endregion
  //region Init / Destroy
  construct(config, store) {
    const me = this;
    me.widgetMap = {};
    super.construct(...arguments);
    me.externalRenderer = me.renderer;
    me.renderer = me.internalRenderer;
  }
  doDestroy() {
    // Destroy all the widgets we created.
    for (const widget of Object.values(this.widgetMap)) {
      widget.destroy && widget.destroy();
    }
    super.doDestroy();
  }
  // Called by grid when its read-only state is toggled
  updateReadOnly(readOnly) {
    for (const widget of Object.values(this.widgetMap)) {
      if (!widget.cellInfo.record.readOnly) {
        widget.readOnly = readOnly;
      }
    }
  }
  //endregion
  //region Render
  /**
   * Renderer that displays a widget in the cell.
   * @param {Object} renderData Render data
   * @param {Grid.column.Column} renderData.column Rendered column
   * @param {Core.data.Model} renderData.record Rendered record
   * @private
   */
  internalRenderer(renderData) {
    var _this$externalRendere;
    const me = this,
      {
        cellElement,
        column,
        record,
        isExport
      } = renderData,
      {
        widgets
      } = column;
    // This renderer might be called from subclasses by accident
    // This condition saves us from investigating bug reports
    if (!isExport && widgets) {
      // If there is no widgets yet and we're going to add them,
      // need to make sure there is no content left in the cell after its previous usage
      // by grid features such as grouping feature or so.
      if (!cellElement.widgets) {
        // Reset cell content
        me.clearCell(cellElement);
      }
      cellElement.widgets = renderData.widgets = widgets.map((widgetCfg, i) => {
        var _me$onBeforeWidgetSet, _me$onAfterWidgetSetV;
        let widget, widgetNextSibling;
        // If cell element already has widgets, check if we need to destroy/remove one
        if (cellElement.widgets) {
          // Current widget
          widget = cellElement.widgets[i];
          // Store next element sibling to insert widget to correct position later
          widgetNextSibling = widget.element.nextElementSibling;
          // If we are not syncing content for present widget, remove it from cell and render again later
          if (widgetCfg.recreate && widget) {
            // destroy widget and remove reference to it
            delete me.widgetMap[widget.id];
            widget.destroy();
            cellElement.widgets[i] = null;
          }
        }
        // Ensure widget is created if first time through
        if (!widget) {
          me.onBeforeWidgetCreate(widgetCfg, renderData);
          widgetCfg.recomposeAsync = false;
          widgetCfg.owner = me.grid;
          widget = WidgetHelper.append(widgetCfg, widgetNextSibling ? {
            insertBefore: widgetNextSibling
          } : cellElement)[0];
          me.widgetMap[widget.id] = widget;
          me.onAfterWidgetCreate(widget, renderData);
          if (widget.name) {
            widget.ion({
              change: ({
                value,
                userAction
              }) => {
                if (userAction) {
                  widget.cellInfo.record[widget.name] = value;
                }
              }
            });
          }
        }
        widget.cellInfo = {
          record,
          column
        };
        if (me.grid && !me.meta.isSelectionColumn) {
          widget.readOnly = me.grid.readOnly || record.readOnly;
        }
        if (((_me$onBeforeWidgetSet = me.onBeforeWidgetSetValue) === null || _me$onBeforeWidgetSet === void 0 ? void 0 : _me$onBeforeWidgetSet.call(me, widget, renderData)) !== false) {
          const valueProperty = widgetCfg.valueProperty || 'value' in widget && 'value' || widget.defaultBindProperty;
          if (valueProperty) {
            const value = widget.name ? record[widget.name] : renderData.value;
            widget[valueProperty] = value;
          }
        }
        (_me$onAfterWidgetSetV = me.onAfterWidgetSetValue) === null || _me$onAfterWidgetSetV === void 0 ? void 0 : _me$onAfterWidgetSetV.call(me, widget, renderData);
        return widget;
      });
    }
    if (isExport) {
      return null;
    }
    return (_this$externalRendere = this.externalRenderer) === null || _this$externalRendere === void 0 ? void 0 : _this$externalRendere.call(this, renderData);
  }
  //endregion
  //region Other
  /**
   * Called before widget is created on rendering
   * @param {ContainerItemConfig} widgetCfg Widget config
   * @param {Object} renderData Render data
   * @private
   */
  onBeforeWidgetCreate(widgetCfg, renderData) {}
  /**
   * Called after widget is created on rendering
   * @param {Core.widget.Widget} widget Created widget
   * @param {Object} renderData Render data
   * @private
   */
  onAfterWidgetCreate(widget, renderData) {}
  /**
   * Called before the widget gets its value on rendering. Pass `false` to skip value setting while rendering
   * @preventable
   * @function onBeforeWidgetSetValue
   * @param {Core.widget.Widget} widget Created widget
   * @param {Object} renderData Render data
   * @param {Grid.column.Column} renderData.column Rendered column
   * @param {Core.data.Model} renderData.record Rendered record
   */
  /**
   * Called after the widget gets its value on rendering.
   * @function onAfterWidgetSetValue
   * @param {Core.widget.Widget} widget Created widget
   * @param {Object} renderData Render data
   * @param {Grid.column.Column} renderData.column Rendered column
   * @param {Core.data.Model} renderData.record Rendered record
   */
  // Overrides base implementation to cleanup widgets, for example when a cell is reused as part of group header
  clearCell(cellElement) {
    if (cellElement.widgets) {
      cellElement.widgets.forEach(widget => {
        // Destroy widget and remove reference to it
        delete this.widgetMap[widget.id];
        widget.destroy();
      });
      cellElement.widgets = null;
    }
    // Even if there is no widgets need to make sure there is no content left, for example after a cell has been reused as part of group header
    super.clearCell(cellElement);
  }
  // Null implementation because there is no way of ascertaining whether the widgets get their width from
  // the column, or the column shrinkwraps the Widget.
  // Remember that the widget could have a width from a CSS rule which we cannot read.
  // It might have width: 100%, or a flex which would mean it is sized by us, but we cannot read that -
  // getComputedStyle would return the numeric width.
  resizeToFitContent() {}
  //endregion
}

ColumnStore.registerColumnType(WidgetColumn);
WidgetColumn.exposeProperties();
WidgetColumn._$name = 'WidgetColumn';

/**
 * @module Grid/column/CheckColumn
 */
/**
 * A column that displays a checkbox in the cell. The value of the backing field is toggled by the checkbox.
 *
 * Toggling of the checkboxes is disabled if a record is readOnly or if the CellEdit feature is not enabled.
 *
 * This column renders a {@link Core.widget.Checkbox checkbox} into each cell, and it is not intended to be changed.
 * If you want to hide certain checkboxes, you can use the {@link #config-renderer} method to access the checkbox widget
 * as it is being rendered.
 *
 * <div class="note">
 * It is <strong>not valid</strong> to use this column without a {@link #config-field} setting because the
 * checked/unchecked state needs to be backed up in a record because rows are recycled and the state will be lost when a
 * row is reused.
 * </div>
 *
 * @extends Grid/column/WidgetColumn
 *
 * @example
 * new Grid({
 *     appendTo : document.body,
 *
 *     columns : [
 *         {
 *              type: 'check',
 *              field: 'allow',
 *              // In the column renderer, we get access to the record and CheckBox widget
 *              renderer({ record, widgets }) {
 *                  // Hide checkboxes in certain rows
 *                  widgets[0].hidden = record.readOnly;
 *              }
 *         }
 *     ]
 * });
 *
 * @classType check
 * @inlineexample Grid/column/CheckColumn.js
 * @column
 */
class CheckColumn extends WidgetColumn {
  //region Config
  static $name = 'CheckColumn';
  static type = 'check';
  static fields = ['checkCls', 'showCheckAll', 'onAfterWidgetSetValue', 'onBeforeWidgetSetValue', 'callOnFunctions', 'onBeforeToggle', 'onToggle', 'onToggleAll'];
  static defaults = {
    align: 'center',
    /**
     * CSS class name to add to checkbox
     * @config {String}
     * @category Rendering
     */
    checkCls: null,
    /**
     * True to show a checkbox in the column header to be able to select/deselect all rows
     * @config {Boolean}
     */
    showCheckAll: false,
    sortable: true,
    filterable: true,
    widgets: [{
      type: 'checkbox',
      valueProperty: 'checked'
    }]
  };
  construct(config, store) {
    super.construct(...arguments);
    const me = this;
    Object.assign(me, {
      externalHeaderRenderer: me.headerRenderer,
      externalOnBeforeWidgetSetValue: me.onBeforeWidgetSetValue,
      externalOnAfterWidgetSetValue: me.onAfterWidgetSetValue,
      onBeforeWidgetSetValue: me.internalOnBeforeWidgetSetValue,
      onAfterWidgetSetValue: me.internalOnAfterWidgetSetValue,
      headerRenderer: me.internalHeaderRenderer
    });
    if (!me.meta.isSelectionColumn) {
      var _me$grid;
      const modelClass = (_me$grid = me.grid) === null || _me$grid === void 0 ? void 0 : _me$grid.store.modelClass;
      if (!me.field) {
        console.warn('CheckColumn MUST be configured with a field, otherwise the checked state will not be persistent. Widgets are recycled and reused');
      } else if (modelClass && !modelClass.fieldMap[me.field] && !me.constructor.suppressNoModelFieldWarning) {
        console.warn(me.$$name + ' is configured with a field, but this is not part of your Model `fields` collection.');
        modelClass.addField({
          name: me.field,
          type: 'boolean'
        });
      }
    }
  }
  doDestroy() {
    var _this$headerCheckbox;
    (_this$headerCheckbox = this.headerCheckbox) === null || _this$headerCheckbox === void 0 ? void 0 : _this$headerCheckbox.destroy();
    super.doDestroy();
  }
  internalHeaderRenderer({
    headerElement,
    column
  }) {
    let returnValue;
    headerElement.classList.add('b-check-header');
    if (column.showCheckAll) {
      headerElement.classList.add('b-check-header-with-checkbox');
      if (column.headerCheckbox) {
        headerElement.appendChild(column.headerCheckbox.element);
      } else {
        column.headerCheckbox = new Checkbox({
          appendTo: headerElement,
          owner: this.grid,
          ariaLabel: 'L{Checkbox.toggleSelection}',
          internalListeners: {
            change: 'onCheckAllChange',
            thisObj: column
          }
        });
      }
    } else {
      returnValue = column.headerText;
    }
    returnValue = column.externalHeaderRenderer ? column.externalHeaderRenderer.call(this, ...arguments) : returnValue;
    return column.showCheckAll ? undefined : returnValue;
  }
  updateCheckAllState(value) {
    if (this.headerCheckbox) {
      this.headerCheckbox.suspendEvents();
      this.headerCheckbox.checked = value;
      this.headerCheckbox.resumeEvents();
    }
  }
  onCheckAllChange({
    checked
  }) {
    const me = this;
    // If this column is bound to a field, update all records
    if (me.field) {
      const {
        store
      } = me.grid;
      store.beginBatch();
      store.forEach(record => me.updateRecord(record, me.field, checked));
      store.endBatch();
    }
    /**
     * Fired when the header checkbox is clicked to toggle its checked status.
     * @event toggleAll
     * @param {Grid.column.CheckColumn} source This Column
     * @param {Boolean} checked The checked status of the header checkbox.
     */
    me.trigger('toggleAll', {
      checked
    });
  }
  //endregion
  internalRenderer({
    value,
    isExport,
    record,
    cellElement
  }) {
    if (isExport) {
      return value == null ? '' : value;
    }
    const result = super.internalRenderer(...arguments);
    if (record.readOnly && !this.meta.isSelectionColumn) {
      cellElement.widgets[0].readOnly = true;
    }
    // In export we're reusing widget, therefore we need to clean `checked` attribute by hand
    if (value) {
      cellElement.widgets[0].input.setAttribute('checked', true);
    } else {
      cellElement.widgets[0].input.removeAttribute('checked');
    }
    return result;
  }
  //region Widget rendering
  onBeforeWidgetCreate(widgetCfg, event) {
    widgetCfg.cls = this.checkCls;
  }
  onAfterWidgetCreate(widget, event) {
    event.cellElement.widget = widget;
    widget.ion({
      beforeChange: 'onBeforeCheckboxChange',
      change: 'onCheckboxChange',
      thisObj: this
    });
  }
  internalOnBeforeWidgetSetValue(widget) {
    var _this$externalOnBefor;
    widget.record = widget.cellInfo.record;
    this.isInitialSet = true;
    (_this$externalOnBefor = this.externalOnBeforeWidgetSetValue) === null || _this$externalOnBefor === void 0 ? void 0 : _this$externalOnBefor.call(this, ...arguments);
  }
  internalOnAfterWidgetSetValue(widget) {
    var _this$externalOnAfter;
    this.isInitialSet = false;
    (_this$externalOnAfter = this.externalOnAfterWidgetSetValue) === null || _this$externalOnAfter === void 0 ? void 0 : _this$externalOnAfter.call(this, ...arguments);
  }
  //endregion
  //region Events
  onBeforeCheckboxChange({
    source,
    checked,
    userAction
  }) {
    const me = this,
      {
        grid
      } = me,
      {
        record
      } = source.cellInfo;
    // If we are bound to a data field, ensure we respect cellEdit setting
    if (userAction && me.field && (!grid.features.cellEdit || grid.features.cellEdit.disabled) || me.meta.isSelectionColumn && !grid.isSelectable(record) && checked) {
      return false;
    }
    if (!me.isInitialSet) {
      /**
       * Fired when a cell is clicked to toggle its checked status. Returning `false` will prevent status change.
       * @event beforeToggle
       * @param {Grid.column.Column} source This Column
       * @param {Core.data.Model} record The record for the row containing the cell.
       * @param {Boolean} checked The new checked status of the cell.
       */
      return me.trigger('beforeToggle', {
        record,
        checked
      });
    }
  }
  onCheckboxChange({
    source,
    checked
  }) {
    if (!this.isInitialSet) {
      const me = this,
        {
          record
        } = source.cellInfo,
        {
          field
        } = me;
      if (field) {
        me.updateRecord(record, field, checked);
        // Keep header checkbox in sync with reality.
        if (checked) {
          // We check whether *all* records in the store are checked including filtered out ones.
          me.updateCheckAllState(me.grid.store.every(r => r[field], null, true));
        } else {
          me.updateCheckAllState(false);
        }
      }
      /**
       * Fired when a cell is clicked to toggle its checked status.
       * @event toggle
       * @param {Grid.column.Column} source This Column
       * @param {Core.data.Model} record The record for the row containing the cell.
       * @param {Boolean} checked The new checked status of the cell.
       */
      me.trigger('toggle', {
        record,
        checked
      });
    }
  }
  updateRecord(record, field, checked) {
    const setterName = `set${StringHelper.capitalize(field)}`;
    if (record[setterName]) {
      record[setterName](checked);
    } else {
      record.set(field, checked);
    }
  }
  //endregion
  onCellKeyDown({
    event,
    cellElement
  }) {
    // SPACE key toggles the checkbox
    if (event.key === ' ') {
      const checkbox = cellElement.widget;
      checkbox === null || checkbox === void 0 ? void 0 : checkbox.toggle();
      // Prevent native browser scrolling
      event.preventDefault();
      // KeyMap and other features (like context menu) must not process this.
      event.handled = true;
    }
  }
  // This function is not meant to be called by any code other than Base#getCurrentConfig().
  // It extracts the current configs (fields) for the column, with special handling for the hooks
  getCurrentConfig(options) {
    const result = super.getCurrentConfig(options);
    delete result.onBeforeWidgetSetValue;
    delete result.onAfterWidgetSetValue;
    if (this.externalOnBeforeWidgetSetValue) {
      result.onBeforeWidgetSetValue = this.externalOnBeforeWidgetSetValue;
    }
    if (this.externalOnAfterWidgetSetValue) {
      result.onAfterWidgetSetValue = this.externalOnAfterWidgetSetValue;
    }
    return result;
  }
}
ColumnStore.registerColumnType(CheckColumn, true);
CheckColumn._$name = 'CheckColumn';

/**
 * @module Grid/column/RowNumberColumn
 */
/**
 * A column that displays the row number in each cell.
 *
 * There is no `editor`, since value is read-only.
 *
 * ```javascript
 * const grid = new Grid({
 *   appendTo : targetElement,
 *   width    : 300,
 *   columns  : [
 *     { type : 'rownumber' }
 *   ]
 * });
 *
 * @extends Grid/column/Column
 *
 * @classType rownumber
 * @inlineexample Grid/column/RowNumberColumn.js
 * @column
 */
class RowNumberColumn extends Column {
  static $name = 'RowNumberColumn';
  static type = 'rownumber';
  static get defaults() {
    return {
      /**
       * @config {Boolean} groupable
       * @hide
       */
      groupable: false,
      /**
       * @config {Boolean} sortable
       * @hide
       */
      sortable: false,
      /**
       * @config {Boolean} filterable
       * @hide
       */
      filterable: false,
      /**
       * @config {Boolean} searchable
       * @hide
       */
      searchable: false,
      /**
       * @config {Boolean} resizable
       * @hide
       */
      resizable: false,
      /**
       * @config {Boolean} draggable
       * @hide
       */
      draggable: false,
      minWidth: 50,
      width: 50,
      align: 'center',
      text: '#',
      editor: false,
      readOnly: true
    };
  }
  construct(config) {
    super.construct(...arguments);
    const me = this,
      {
        grid
      } = me;
    me.internalCellCls = 'b-row-number-cell';
    me.externalHeaderRenderer = me.headerRenderer;
    me.headerRenderer = me.internalHeaderRenderer;
    if (grid) {
      // Update our width when the store mutates (tests test Columns in isolation with no grid, so we must handle that!)
      grid.ion({
        bindStore: 'bindStore',
        thisObj: me
      });
      me.bindStore({
        store: grid.store,
        initial: true
      });
      if (grid.store.count && !grid.rendered) {
        grid.ion({
          paint: 'resizeToFitContent',
          thisObj: me,
          once: true
        });
      }
    }
  }
  get groupHeaderReserved() {
    return true;
  }
  bindStore({
    store,
    initial
  }) {
    const me = this;
    me.detachListeners('grid');
    store.ion({
      name: 'grid',
      [`change${me.grid.asyncEventSuffix}`]: 'onStoreChange',
      thisObj: me
    });
    if (!initial) {
      me.resizeToFitContent();
    }
  }
  onStoreChange({
    action,
    isMove
  }) {
    if (action === 'dataset' || action === 'add' || action === 'remove' || action === 'removeall') {
      // Ignore remove phase of move operation, resize on add phase only
      if (action === 'remove' && isMove) {
        return;
      }
      const me = this,
        result = me.resizeToFitContent();
      // Gantt/Scheduler draws later when loading using CrudManager (refresh is suspended), catch first draw
      if (action === 'dataset' && !result && me.grid.store.count) {
        me.grid.rowManager.ion({
          renderDone() {
            this.resizeToFitContent();
          },
          once: true,
          thisObj: me
        });
      }
    }
  }
  /**
   * Renderer that displays the row number in the cell.
   * @private
   */
  renderer({
    record,
    grid
  }) {
    return record.isSpecialRow ? '' : grid.store.indexOf(record, true) + 1;
  }
  /**
   * Resizes the column to match the widest string in it. Called when you double click the edge between column
   * headers
   */
  resizeToFitContent() {
    const {
        grid
      } = this,
      {
        store
      } = grid,
      {
        count
      } = store;
    if (count && !this.hidden) {
      const cellElement = grid.element.querySelector(`.b-grid-cell[data-column-id="${this.id}"]`);
      if (cellElement) {
        const cellPadding = parseInt(DomHelper.getStyleValue(cellElement, 'padding-left')),
          maxWidth = DomHelper.measureText(count, cellElement);
        this.width = Math.max(this.minWidth, maxWidth + 2 * cellPadding);
        return true;
      }
    }
    return false;
  }
  set flex(f) {}
  internalHeaderRenderer({
    headerElement,
    column
  }) {
    var _column$externalHeade;
    headerElement.classList.add('b-rownumber-header');
    return ((_column$externalHeade = column.externalHeaderRenderer) === null || _column$externalHeade === void 0 ? void 0 : _column$externalHeade.call(this, ...arguments)) || column.headerText;
  }
}
ColumnStore.registerColumnType(RowNumberColumn, true);
RowNumberColumn._$name = 'RowNumberColumn';

/**
 * @module Grid/feature/GridFeatureManager
 */
const consumerToFeatureMap = new Map(),
  consumerToDefaultFeatureMap = new Map(),
  DEFAULT_FOR_TYPE = 'Grid',
  remapToBase = {
    Grid: 'GridBase',
    Scheduler: 'SchedulerBase',
    SchedulerPro: 'SchedulerProBase',
    Gantt: 'GanttBase'
  },
  classNameFix = /\$\d+$/;
/**
 * Static class intended to register and query grid features (also applies to Scheduler, Scheduler Pro and Gantt).
 *
 * A feature for Grid, Scheduler, Scheduler Pro or Gantt must extend {@link Core/mixin/InstancePlugin}.
 *
 * <div class="note"> Note that features for Calendar and TaskBoard differ, they should not be registered with
 * GridFeatureManager, and they use different base classes.
 * </div>
 *
 * ## Registering a custom feature
 *
 * First define a new feature, extending InstancePlugin:
 *
 * ```javascript
 * export default class MyFeature extends InstancePlugin {
 *    // Class name, needed since the actual class name might be mangled by the minifier
 *    static $name = 'MyFeature';
 *
 *    construct(client, config) {
 *        // Set things up here
 *    }
 * }
 * ```
 *
 * Then register it with GridFeatureManager:
 *
 * ```javascript
GridFeatureManager._$name = 'GridFeatureManager';  * GridFeatureManager.registerFeature(MyFeature);
 * ```
 *
 * After that it is ready to use:
 *
 * ```javascript
 * const grid = new Grid({
 *    features : {
 *      myFeature : true
 *    }
 * });
 *
 * @class
 */
class GridFeatureManager {
  /**
   * Register a feature class with the Grid. Enables it to be created and configured using config Grid#features.
   * @param {Function} featureClass The feature class constructor to register
   * @param {Boolean} [onByDefault] Specify true to have the feature enabled per default
   * @param {String|String[]} [forType] Specify a type to let the class applying the feature to determine if it should
   * use it
   */
  static registerFeature(featureClass, onByDefault = false, forType = null, as = null) {
    // Our built-in features should all define $name to survive minification/obfuscation, but user defined features might not
    as = StringHelper.uncapitalize(as || Object.prototype.hasOwnProperty.call(featureClass, '$name') && featureClass.$$name || featureClass.name);
    // Remove webpack's disambiguation suffix.
    // For example ExcelExporter in Scheduler will be called ExcelExporter$1
    // It must be found as ExcelExporter in the Scheduler's feature Map, so correct the name.
    as = as.replace(classNameFix, '');
    if (!Array.isArray(forType)) {
      forType = [forType || DEFAULT_FOR_TYPE];
    }
    forType.forEach(forType => {
      const type = remapToBase[forType] || forType,
        consumerFeaturesMap = consumerToFeatureMap.get(type) || new Map(),
        consumerDefaultFeaturesMap = consumerToDefaultFeatureMap.get(type) || new Map();
      consumerFeaturesMap.set(as, featureClass);
      consumerDefaultFeaturesMap.set(featureClass, onByDefault);
      consumerToFeatureMap.set(type, consumerFeaturesMap);
      consumerToDefaultFeatureMap.set(type, consumerDefaultFeaturesMap);
    });
  }
  /**
   * Get all the features registered for the given type name in an object where keys are feature names and values are
   * feature constructors.
   *
   * @param {String} [forType]
   * @returns {Object}
   */
  static getTypeNameFeatures(forType = DEFAULT_FOR_TYPE) {
    const type = remapToBase[forType] || forType,
      consumerFeaturesMap = consumerToFeatureMap.get(type),
      features = {};
    if (consumerFeaturesMap) {
      consumerFeaturesMap.forEach((featureClass, as) => features[as] = featureClass);
    }
    return features;
  }
  /**
   * Get all the default features registered for the given type name in an object where keys are feature names and
   * values are feature constructors.
   *
   * @param {String} [forType]
   * @returns {Object}
   */
  static getTypeNameDefaultFeatures(forType = DEFAULT_FOR_TYPE) {
    const type = remapToBase[forType] || forType,
      consumerFeaturesMap = consumerToFeatureMap.get(type),
      consumerDefaultFeaturesMap = consumerToDefaultFeatureMap.get(type);
    const features = {};
    if (consumerFeaturesMap && consumerDefaultFeaturesMap) {
      consumerFeaturesMap.forEach((featureClass, as) => {
        if (consumerDefaultFeaturesMap.get(featureClass)) {
          features[as] = featureClass;
        }
      });
    }
    return features;
  }
  /**
   * Gets all the features registered for the given instance type name chain. First builds the type name chain then
   * queries for features for each type name and combines them into one object, see
   * {@link #function-getTypeNameFeatures-static}() for returned object description.
   *
   * If feature is registered for both parent and child type name then feature for child overrides feature for parent.
   *
   * @param {Object} instance
   * @returns {Object}
   */
  static getInstanceFeatures(instance) {
    return instance.$meta.names.reduce((features, typeName) => Object.assign(features, this.getTypeNameFeatures(typeName)), {});
  }
  /**
   * Gets all the *default* features registered for the given instance type name chain. First builds the type name
   * chain then queries for features for each type name and combines them into one object, see
   * {@link #function-getTypeNameFeatures-static}() for returned object description.
   *
   * If feature is registered for both parent and child type name then feature for child overrides feature for parent.
   *
   * @param {Object} instance
   * @returns {Object}
   */
  static getInstanceDefaultFeatures(instance) {
    return instance.$meta.names.reduce((features, typeName) => Object.entries(this.getTypeNameFeatures(typeName)).reduce((features, [as, featureClass]) => {
      if (this.isDefaultFeatureForTypeName(featureClass, typeName)) {
        features[as] = featureClass;
      } else {
        delete features[as];
      }
      return features;
    }, features), {});
  }
  /**
   * Checks if the given feature class is default for the type name
   *
   * @param {Core.mixin.InstancePlugin} featureClass Feature to check
   * @param {String} [forType]
   * @returns {Boolean}
   */
  static isDefaultFeatureForTypeName(featureClass, forType = DEFAULT_FOR_TYPE) {
    const type = remapToBase[forType] || forType,
      consumerDefaultFeaturesMap = consumerToDefaultFeatureMap.get(type);
    return consumerDefaultFeaturesMap && consumerDefaultFeaturesMap.get(featureClass) || false;
  }
  /**
   * Checks if the given feature class is default for the given instance type name chain. If the feature is not
   * default for the parent type name but it is for the child type name, then the child setting overrides the parent
   * one.
   *
   * @param {Core.mixin.InstancePlugin} featureClass Feature to check
   * @param {String} [forType]
   * @returns {Boolean}
   */
  static isDefaultFeatureForInstance(featureClass, instance) {
    //const typeChain = ObjectHelper.getTypeNameChain(instance);
    const typeChain = instance.$meta.names.slice().reverse();
    let result = null;
    for (let i = 0, len = typeChain.length; i < len && result === null; ++i) {
      const consumerDefaultFeaturesMap = consumerToDefaultFeatureMap.get(typeChain[i]);
      if (consumerDefaultFeaturesMap && consumerDefaultFeaturesMap.has(featureClass)) {
        result = consumerDefaultFeaturesMap.get(featureClass);
      }
    }
    return result || false;
  }
  /**
   * Resets feature registration date, used in tests to reset state after test
   *
   * @internal
   */
  static reset() {
    consumerToFeatureMap.clear();
    consumerToDefaultFeatureMap.clear();
  }
}

const editingActions = {
  finishAndEditNextRow: 1,
  finishAndEditPrevRow: 1,
  finishEditing: 1,
  cancelEditing: 1,
  finishAndEditNextCell: 1,
  finishAndEditPrevCell: 1
};
/**
 * @module Grid/feature/CellEdit
 */
/**
 * Adding this feature to the grid and other Bryntum products which are based on the Grid (i.e. Scheduler, SchedulerPro, and Gantt)
 * enables cell editing. Any subclass of {@link Core.widget.Field Field} can be used
 * as editor for the {@link Grid.column.Column Column}. The most popular are:
 *
 * - {@link Core.widget.TextField TextField}
 * - {@link Core.widget.NumberField NumberField}
 * - {@link Core.widget.DateField DateField}
 * - {@link Core.widget.TimeField TimeField}
 * - {@link Core.widget.Combo Combo}
 *
 * Usage instructions:
 * ## Start editing
 * * Double click on a cell
 * * Press [ENTER] or [F2] with a cell selected (see {@link #keyboard-shortcuts Keyboard shortcuts} below)
 * * It is also possible to change double click to single click to start editing, using the {@link #config-triggerEvent} config
 *
 * ```javascript
 * new Grid({
 *    features : {
 *        cellEdit : {
 *            triggerEvent : 'cellclick'
 *        }
 *    }
 * });
 * ```
 *
 * ## Instant update
 * If {@link Grid.column.Column#config-instantUpdate} on the column is set to true, record will be
 * updated instantly as value in the editor is changed. In combination with {@link Core.data.Store#config-autoCommit} it
 * could result in excessive requests to the backend.
 * By default instantUpdate is false, but it is enabled for some special columns, such as Duration column in Scheduler
 * Pro and all date columns in Gantt.
 *
 * ## Keyboard shortcuts
 * ### While not editing
 * | Keys    | Action         | Action description                    |
 * |---------|--------------- |---------------------------------------|
 * | `Enter` | *startEditing* | Starts editing currently focused cell |
 * | `F2`    | *startEditing* | Starts editing currently focused cell |
 *
 * ### While editing
 * | Keys            | Action                  | Action description                                                                        |
 * |-----------------|-------------------------|-------------------------------------------------------------------------------------------|
 * | `Enter`         | *finishAndEditNextRow*  | Finish editing and start editing the same cell in next row                                 |
 * | `Shift`+`Enter` | *finishAndEditPrevRow*  | Finish editing and start editing the same cell in previous row                             |
 * | `F2`            | *finishEditing*         | Finish editing                                                                             |
 * | `Ctrl`+`Enter`  | *finishAllSelected*     | If {@link #config-multiEdit} is active, this applies new value on all selected rows/cells  |
 * | `Ctrl`+`Enter`  | *finishEditing*         | Finish editing                                                                             |
 * | `Escape`        | *cancelEditing*         | By default, first reverts the value back to its original value, next press cancels editing |
 * | `Tab`           | *finishAndEditNextCell* | Finish editing and start editing the next cell                                             |
 * | `Shift`+`Tab`   | *finishAndEditPrevCell* | Finish editing and start editing the previous cell                                         |
 *
 * <div class="note">Please note that <code>Ctrl</code> is the equivalent to <code>Command</code> and <code>Alt</code>
 * is the equivalent to <code>Option</code> for Mac users</div>
 *
 * For more information on how to customize keyboard shortcuts, please see
 * [our guide](#Grid/guides/customization/keymap.md).
 *
 * ## Editor configuration
 * Columns specify editor in their configuration. Editor can also by set by using a column type. Columns
 * may also contain these three configurations which affect how their cells are edited:
 * * {@link Grid.column.Column#config-invalidAction}
 * * {@link Grid.column.Column#config-revertOnEscape}
 * * {@link Grid.column.Column#config-finalizeCellEdit}
 *
 * ## Preventing editing of certain cells
 * You can prevent editing on a column by setting `editor` to false:
 *
 * ```javascript
 * new Grid({
 *    columns : [
 *       {
 *          type   : 'number',
 *          text   : 'Age',
 *          field  : 'age',
 *          editor : false
 *       }
 *    ]
 * });
 * ```
 *
 * To prevent editing in a specific cell, listen to the {@link #event-beforeCellEditStart} and return false:
 *
 * ```javascript
 * grid.on('beforeCellEditStart', ({ editorContext }) => {
 *     return editorContext.column.field !== 'id';
 * });
 * ```
 *
 * ## Choosing field on the fly
 * To use an alternative input field to edit a cell, listen to the {@link #event-beforeCellEditStart} and
 * set the `editor` property of the context to the input field you want to use:
 *
 * ```javascript
 * grid.on('beforeCellEditStart', ({ editorContext }) => {
 *     return editorContext.editor = myDateField;
 * });
 * ```
 *
 * ## Loading remote data into a combo box cell editor
 * If you need to prepare or modify the data shown by the cell editor, e.g. load remote data into the store used by a combo,
 * listen to the {@link #event-startCellEdit} event:
 * ```javascript
 * const employeeStore = new AjaxStore({ readUrl : '/cities' }); // A server endpoint returning data like:
 *                                                               // [{ id : 123, name : 'Bob Mc Bob' }, { id : 345, name : 'Lind Mc Foo' }]
 *
 * new Grid({
 *     // Example data including a city field which is an id used to look up entries in the cityStore above
 *     data : [
 *         { id : 1, name : 'Task 1', employeeId : 123 },
 *         { id : 2, name : 'Task 2', employeeId : 345 }
 *     ],
 *     columns : [
 *       {
 *          text   : 'Task',
 *          field  : 'name'
 *       },
 *       {
 *          text   : 'Assigned to',
 *          field  : 'employeeId',
 *          editor : {
 *               type : 'combo',
 *               store : employeeStore,
 *               // specify valueField'/'displayField' to match the data format in the employeeStore store
 *               valueField : 'id',
 *               displayField : 'name'
 *           },
 *           renderer : ({ value }) {
 *                // Use a renderer to show the employee name, which we find by querying employeeStore by the id of the grid record
 *                return employeeStore.getById(value)?.name;
 *           }
 *       }
 *    ],
 *    listeners : {
 *        // When editing, you might want to fetch data for the combo store from a remote resource
 *        startCellEdit({ editorContext }) {
 *            const { record, editor, column } = editorContext;
 *            if (column.field === 'employeeId') {
 *                // Load possible employees to assign to this particular task
 *                editor.inputField.store.load({ task : record.id });
 *            }
 *       }
 *    }
 * });
 * ```
 *
 * ## Editing on touch devices
 *
 * On touch devices, a single tap navigates and tapping an already selected cell after a short delay starts the editing.
 *
 * This feature is **enabled** by default.
 *
 * @extends Core/mixin/InstancePlugin
 *
 * @demo Grid/celledit
 * @classtype cellEdit
 * @inlineexample Grid/feature/CellEdit.js
 * @feature
 */
class CellEdit extends Delayable(InstancePlugin) {
  //region Config
  static $name = 'CellEdit';
  // Default configuration
  static get defaultConfig() {
    return {
      /**
       * Set to true to select the field text when editing starts
       * @config {Boolean}
       * @default
       */
      autoSelect: true,
      /**
       * What action should be taken when focus moves leaves the cell editor, for example when clicking outside.
       * May be `'complete'` or `'cancel`'.
       * @config {'complete'|'cancel'}
       * @default
       */
      blurAction: 'complete',
      /**
       * Set to `false` to stop editing when clicking another cell after a cell edit.
       * @config {Boolean}
       * @default
       */
      continueEditingOnCellClick: true,
      /**
       * Set to true to have TAB key on the last cell (and ENTER anywhere in the last row) in the data set create
       * a new record and begin editing it at its first editable cell.
       *
       * If a customized {@link #config-keyMap} is used, this setting will affect the customized keys instead of
       * ENTER and TAB.
       *
       * If this is configured as an object, it is used as the default data value set for each new record.
       * @config {Boolean|Object}
       */
      addNewAtEnd: null,
      /**
       * Set to `true` to add record to the parent of the last record, when configured with {@link #config-addNewAtEnd}.
       * Only applicable when using a tree view and store.
       *
       * By default, it adds records to the root.
       * @config {Boolean}
       * @default false
       */
      addToCurrentParent: false,
      /**
       * Set to `true` to start editing when user starts typing text on a focused cell (as in Excel)
       * @config {Boolean}
       * @default false
       */
      autoEdit: null,
      /**
       * Set to `false` to not start editing next record when user presses enter inside a cell editor (or previous
       * record if SHIFT key is pressed). This is set to `false` when {@link #config-autoEdit} is `true`. Please
       * note that these key combinations could be different if a customized {@link #config-keyMap} is used.
       * @config {Boolean}
       * @default
       */
      editNextOnEnterPress: true,
      /**
       * Class to use as an editor. Default value: {@link Core.widget.Editor}
       * @config {Core.widget.Widget}
       * @typings {typeof Widget}
       * @internal
       */
      editorClass: Editor,
      /**
       * The name of the grid event that will trigger cell editing. Defaults to
       * {@link Grid.view.mixin.GridElementEvents#event-cellDblClick celldblclick} but can be changed to any other event,
       * such as {@link Grid.view.mixin.GridElementEvents#event-cellClick cellclick}.
       *
       * ```javascript
       * features : {
       *     cellEdit : {
       *         triggerEvent : 'cellclick'
       *     }
       * }
       * ```
       *
       * @config {String}
       * @default
       */
      triggerEvent: 'celldblclick',
      // To edit a cell using a touch gesture, at least 300ms should have passed since last cell tap
      touchEditDelay: 300,
      focusCellAnimationDuration: false,
      /**
       * If set to `true` (which is default) this will make it possible to edit current column in multiple rows
       * simultaneously.
       *
       * This is achieved by:
       * 1. Select multiple rows or row's cells
       * 2. Start editing simultaneously as selecting the last row or cell
       * 3. When finished editing, press Ctrl+Enter to apply the new value to all selected rows.
       *
       * If a customized {@link #config-keyMap} is used, the Ctrl+Enter combination could map to something else.
       *
       * @config {Boolean}
       * @default
       */
      multiEdit: true,
      /**
       * See {@link #keyboard-shortcuts Keyboard shortcuts} for details
       * @config {Object<String,String>}
       */
      keyMap: {
        Enter: ['startEditing', 'finishAndEditNextRow'],
        'Ctrl+Enter': ['finishAllSelected', 'finishEditing'],
        'Shift+Enter': 'finishAndEditPrevRow',
        'Alt+Enter': 'finishEditing',
        F2: ['startEditing', 'finishEditing'],
        Escape: 'cancelEditing',
        Tab: 'finishAndEditNextCell',
        'Shift+Tab': 'finishAndEditPrevCell'
      },
      /**
       * A CSS selector for elements that when clicked, should not trigger editing. Useful if you render actionable
       * icons or buttons into a grid cell.
       * @config {String}
       * @default
       */
      ignoreCSSSelector: 'button,.b-icon,.b-fa,svg'
    };
  }
  // Plugin configuration. This plugin chains some of the functions in Grid.
  static get pluginConfig() {
    return {
      assign: ['startEditing', 'finishEditing', 'cancelEditing'],
      before: ['onElementKeyDown', 'onElementPointerUp'],
      chain: ['onElementClick', 'bindStore']
    };
  }
  //endregion
  //region Init
  construct(grid, config) {
    super.construct(grid, config);
    const me = this,
      gridListeners = {
        renderRows: 'onGridRefreshed',
        cellClick: 'onCellClick',
        thisObj: me
      };
    me.grid = grid;
    if (me.triggerEvent !== 'cellclick') {
      gridListeners[me.triggerEvent] = 'onTriggerEditEvent';
    }
    if (me.autoEdit && !('editNextOnEnterPress' in config)) {
      me.editNextOnEnterPress = false;
    }
    grid.ion(gridListeners);
    grid.rowManager.ion({
      changeTotalHeight: 'onGridRefreshed',
      thisObj: me
    });
    me.bindStore(grid.store);
  }
  bindStore(store) {
    this.detachListeners('store');
    store.ion({
      name: 'store',
      update: 'onStoreUpdate',
      beforeSort: 'onStoreBeforeSort',
      thisObj: this
    });
  }
  /**
   * Displays an OK / Cancel confirmation dialog box owned by the current Editor. This is intended to be
   * used by {@link Grid.column.Column#config-finalizeCellEdit} implementations. The returned promise resolves passing
   * `true` if the "OK" button is pressed, and `false` if the "Cancel" button is pressed. Typing `ESC` rejects.
   * @param {Object} options An options object for what to show.
   * @param {String} [options.title] The title to show in the dialog header.
   * @param {String} [options.message] The message to show in the dialog body.
   * @param {String|Object} [options.cancelButton] A text or a config object to apply to the Cancel button.
   * @param {String|Object} [options.okButton] A text or config object to apply to the OK button.
   */
  async confirm(options) {
    let result = true;
    if (this.editorContext) {
      // The input field must not lose containment of focus during this confirmation
      // so temporarily make the MessageDialog a descendant widget.
      MessageDialog.owner = this.editorContext.editor.inputField;
      options.rootElement = this.grid.rootElement;
      result = await MessageDialog.confirm(options);
      MessageDialog.owner = null;
    }
    return result === MessageDialog.yesButton;
  }
  doDestroy() {
    // To kill timeouts
    this.grid.columns.allRecords.forEach(column => {
      var _column$_cellEditor;
      (_column$_cellEditor = column._cellEditor) === null || _column$_cellEditor === void 0 ? void 0 : _column$_cellEditor.destroy();
    });
    super.doDestroy();
  }
  doDisable(disable) {
    if (disable && !this.isConfiguring) {
      this.cancelEditing(true);
    }
    super.doDisable(disable);
  }
  set disabled(disabled) {
    super.disabled = disabled;
  }
  get disabled() {
    const {
      grid
    } = this;
    return Boolean(super.disabled || grid.disabled || grid.readOnly);
  }
  //endregion
  //region Editing
  /**
   * Is any cell currently being edited?
   * @readonly
   * @property {Boolean}
   */
  get isEditing() {
    return Boolean(this.editorContext);
  }
  /**
   * Returns the record currently being edited, or `null`
   * @readonly
   * @property {Core.data.Model}
   */
  get activeRecord() {
    var _this$editorContext;
    return ((_this$editorContext = this.editorContext) === null || _this$editorContext === void 0 ? void 0 : _this$editorContext.record) || null;
  }
  /**
   * Internal function to create or get existing editor for specified cell.
   * @private
   * @param cellContext Cell to get or create editor for
   * @returns {Core.widget.Editor} An Editor container which displays the input field.
   * @category Internal
   */
  getEditorForCell({
    id,
    cell,
    column,
    columnId,
    editor
  }) {
    var _cellEditor;
    const me = this,
      {
        grid,
        editorClass
      } = me;
    // Reuse the Editor by caching it on the column.
    let cellEditor = column.cellEditor,
      leftOffset = 0; // Only applicable for tree cells to show editor right of the icons etc
    // Help Editor match size and position
    if (column.editTargetSelector) {
      const editorTarget = cell.querySelector(column.editTargetSelector);
      leftOffset = editorTarget.offsetLeft;
    }
    editor.autoSelect = me.autoSelect;
    // Still a config
    if (!((_cellEditor = cellEditor) !== null && _cellEditor !== void 0 && _cellEditor.isEditor)) {
      cellEditor = column.data.cellEditor = editorClass.create(editorClass.mergeConfigs({
        type: editorClass.type,
        constrainTo: null,
        cls: 'b-cell-editor',
        inputField: editor,
        blurAction: 'none',
        invalidAction: column.invalidAction,
        completeKey: false,
        cancelKey: false,
        owner: grid,
        align: {
          align: 't0-t0',
          offset: [leftOffset, 0]
        },
        internalListeners: me.getEditorListeners(),
        // Listen for cell edit control keys from the Editor
        onInternalKeyDown: me.onEditorKeydown.bind(me),
        // React editor wrapper code uses this flag to enable mouse events pass through to editor
        allowMouseEvents: editor.allowMouseEvents
      }, cellEditor));
    }
    // If matchSize auto heights it, ensure it at least covers the cell.
    cellEditor.minHeight = grid.rowHeight;
    // If the input field needs changing, change it.
    if (cellEditor.inputField !== editor) {
      cellEditor.remove(cellEditor.items[0]);
      cellEditor.add(editor);
    }
    // Ensure the X offset is set to clear TreeCell furniture
    cellEditor.align.offset[0] = leftOffset;
    // Keep the record synced with the value
    if (column.instantUpdate && !editor.cellEditValueSetter) {
      ObjectHelper.wrapProperty(editor, 'value', null, value => {
        const {
            editorContext
          } = me,
          inputField = editorContext === null || editorContext === void 0 ? void 0 : editorContext.editor.inputField;
        // Only tickle the record if the value has changed.
        if (editorContext !== null && editorContext !== void 0 && editorContext.editor.isValid && !ObjectHelper.isEqual(editorContext.record[editorContext.column.field], value) && (
        // If editor is a dateField, only allow picker input as not to trigger change on each keystroke.
        !(inputField !== null && inputField !== void 0 && inputField.isDateField) || inputField._isPickerInput)) {
          editorContext.record[editorContext.column.field] = value;
        }
      });
      editor.cellEditValueSetter = true;
    }
    Object.assign(cellEditor.element.dataset, {
      rowId: id,
      columnId,
      field: column.field
    });
    // First ESC press reverts
    cellEditor.inputField.revertOnEscape = column.revertOnEscape;
    return me.editor = cellEditor;
  }
  // Turned into function to allow overriding in Gantt, and make more configurable in general
  getEditorListeners() {
    return {
      focusOut: 'onEditorFocusOut',
      focusIn: 'onEditorFocusIn',
      start: 'onEditorStart',
      beforeComplete: 'onEditorBeforeComplete',
      complete: 'onEditorComplete',
      beforeCancel: 'onEditorBeforeCancel',
      cancel: 'onEditorCancel',
      beforeHide: 'onBeforeEditorHide',
      finishEdit: 'onEditorFinishEdit',
      thisObj: this
    };
  }
  onEditorStart({
    source: editor
  }) {
    const me = this,
      editorContext = me.editorContext = editor.cellEditorContext;
    if (editorContext) {
      var _me$removeEditingList;
      const {
        grid
      } = me;
      // Should move editing to new cell on click, unless click is configured to start editing - in which case it
      // will move anyway
      if (me.triggerEvent !== 'cellclick') {
        me.detachListeners('cellClickWhileEditing');
        grid.ion({
          name: 'cellClickWhileEditing',
          cellclick: 'onCellClickWhileEditing',
          thisObj: me
        });
      }
      (_me$removeEditingList = me.removeEditingListeners) === null || _me$removeEditingList === void 0 ? void 0 : _me$removeEditingList.call(me);
      // Handle tapping outside of the grid element. Use GlobalEvents
      // because it uses a capture:true listener before any other handlers
      // might stop propagation.
      // Cannot use delegate here. A tapped cell will match :not(#body-container)
      me.removeEditingListeners = GlobalEvents.addListener({
        globaltap: 'onTapOut',
        thisObj: me
      });
      /**
       * Fires on the owning Grid when editing starts
       * @event startCellEdit
       * @on-owner
       * @param {Grid.view.Grid} source Owner grid
       * @param {Grid.util.Location} editorContext Editing context
       * @param {Core.widget.Editor} editorContext.editor The Editor being used.
       * Will contain an `inputField` property which is the field being used to perform the editing.
       * @param {Grid.column.Column} editorContext.column Target column
       * @param {Core.data.Model} editorContext.record Target record
       * @param {HTMLElement} editorContext.cell Target cell
       * @param {*} editorContext.value Cell value
       */
      grid.trigger('startCellEdit', {
        grid,
        editorContext
      });
    }
  }
  onEditorBeforeComplete(context) {
    const {
        grid
      } = this,
      editor = context.source,
      editorContext = editor.cellEditorContext;
    context.grid = grid;
    context.editorContext = editorContext;
    /**
     * Fires on the owning Grid before the cell editing is finished, return false to signal that the value is invalid and editing should not be finalized.
     * @on-owner
     * @event beforeFinishCellEdit
     * @param {Grid.view.Grid} grid Target grid
     * @param {Grid.util.Location} editorContext Editing context
     * @param {Core.widget.Editor} editorContext.editor The Editor being used.
     * Will contain an `inputField` property which is the field being used to perform the editing.
     * @param {Grid.column.Column} editorContext.column Target column
     * @param {Core.data.Model} editorContext.record Target record
     * @param {HTMLElement} editorContext.cell Target cell
     * @param {*} editorContext.value Cell value
     */
    return grid.trigger('beforeFinishCellEdit', context);
  }
  onEditorComplete({
    source: editor
  }) {
    const {
        grid
      } = this,
      editorContext = editor.cellEditorContext;
    // Ensure the docs below are accurate!
    editorContext.value = editor.inputField.value;
    /**
     * Fires on the owning Grid when cell editing is finished
     * @event finishCellEdit
     * @on-owner
     * @param {Grid.view.Grid} grid Target grid
     * @param {Grid.util.Location} editorContext Editing context
     * @param {Core.widget.Editor} editorContext.editor The Editor being used.
     * Will contain an `inputField` property which is the field being used to perform the editing.
     * @param {Grid.column.Column} editorContext.column Target column
     * @param {Core.data.Model} editorContext.record Target record
     * @param {HTMLElement} editorContext.cell Target cell
     * @param {*} editorContext.value Cell value
     */
    grid.trigger('finishCellEdit', {
      grid,
      editorContext
    });
  }
  onEditorBeforeCancel() {
    const {
      editorContext
    } = this;
    /**
     * Fires on the owning Grid before the cell editing is canceled, return `false` to prevent cancellation.
     * @event beforeCancelCellEdit
     * @preventable
     * @on-owner
     * @param {Grid.view.Grid} source Owner grid
     * @param {Grid.util.Location} editorContext Editing context
     */
    return this.grid.trigger('beforeCancelCellEdit', {
      editorContext
    });
  }
  onEditorCancel({
    event
  }) {
    const {
      editorContext,
      muteEvents,
      grid
    } = this;
    if (!muteEvents) {
      /**
       * Fires on the owning Grid when editing is cancelled
       * @event cancelCellEdit
       * @on-owner
       * @param {Grid.view.Grid} source Owner grid
       * @param {Grid.util.Location} editorContext Editing context
       * @param {Event} event Included if the cancellation was triggered by a DOM event
       */
      grid.trigger('cancelCellEdit', {
        grid,
        editorContext,
        event
      });
    }
  }
  onBeforeEditorHide({
    source
  }) {
    const me = this,
      {
        row,
        cell
      } = source.cellEditorContext;
    // Clean up and restore cell to full visibility
    // before we hide and attempt to revert focus to the cell.
    cell === null || cell === void 0 ? void 0 : cell.classList.remove('b-editing');
    row === null || row === void 0 ? void 0 : row.removeCls('b-editing');
    me.detachListeners('cellClickWhileEditing');
    me.removeEditingListeners();
  }
  onEditorFinishEdit({
    source
  }) {
    // Clean up context objects so we know we are not editing
    source.cellEditorContext = this.editorContext = null;
  }
  /**
   * Find the next succeeding or preceding cell which is editable (column.editor != false)
   * @param {Object} cellInfo
   * @param {Boolean} isForward
   * @returns {Object}
   * @private
   * @category Internal
   */
  getAdjacentEditableCell(cellInfo, isForward) {
    const {
        grid
      } = this,
      {
        store,
        columns
      } = grid,
      {
        visibleColumns
      } = columns;
    let rowId = cellInfo.id,
      column = columns.getAdjacentVisibleLeafColumn(cellInfo.columnId, isForward);
    while (rowId) {
      if (column) {
        if (column.editor && column.canEdit(store.getById(rowId))) {
          return {
            id: rowId,
            columnId: column.id
          };
        }
        column = columns.getAdjacentVisibleLeafColumn(column, isForward);
      } else {
        const record = store.getAdjacent(cellInfo.id, isForward, false, true);
        rowId = record === null || record === void 0 ? void 0 : record.id;
        if (record) {
          column = isForward ? visibleColumns[0] : visibleColumns[visibleColumns.length - 1];
        }
      }
    }
    return null;
  }
  /**
   * Adds a new, empty record at the end of the TaskStore with the initial
   * data specified by the {@link Grid.feature.CellEdit#config-addNewAtEnd} setting.
   *
   * @private
   * @returns {Core.data.Model} Newly added record
   */
  doAddNewAtEnd() {
    const newRecordConfig = typeof this.addNewAtEnd === 'object' ? ObjectHelper.clone(this.addNewAtEnd) : {},
      {
        grid: {
          store,
          rowManager
        },
        addToCurrentParent
      } = this;
    let record;
    if (store.tree && addToCurrentParent) {
      record = store.last.parent.appendChild(newRecordConfig);
    } else {
      record = store.add(newRecordConfig)[0];
    }
    // If the new record was not added due to it being off the end of the rendered block
    // ensure we force it to be there before we attempt to edit it.
    if (!rowManager.getRowFor(record)) {
      rowManager.displayRecordAtBottom();
    }
    return record;
  }
  /**
   * Creates an editing context object for the passed cell context (target cell must be in the DOM).
   *
   * If the referenced cell is editable, a {@link Grid.util.Location} will
   * be returned containing the following extra properties:
   *
   *     - editor
   *     - value
   *
   * If the referenced cell is _not_ editable, `false` will be returned.
   * @param {Object} cellContext an object which encapsulates a cell.
   * @param {String} cellContext.id The record id of the row to edit
   * @param {String} cellContext.columnId The column id of the column to edit
   * @returns {Grid.util.Location}
   * @private
   */
  getEditingContext(cellContext) {
    cellContext = this.grid.normalizeCellContext(cellContext);
    const {
      column,
      record
    } = cellContext;
    // Cell must be in the DOM to edit.
    // Cannot edit hidden columns and columns without an editor.
    // Cannot edit special rows (groups etc).
    if (column !== null && column !== void 0 && column.isVisible && column.editor && !column.readOnly && record && !record.isSpecialRow && !record.readOnly && column.canEdit(record)) {
      // If the field name is a complex mapping (instead of using a field name with a dataSource)
      // set it correctly. Row#renderCell gets its contentValue in this way.
      const value = record ? column.getRawValue(record) : record;
      Object.assign(cellContext, {
        value: value === undefined ? null : value,
        editor: column.editor
      });
      return cellContext;
    } else {
      return false;
    }
  }
  /**
   * Start editing specified cell. If no cellContext is given it starts with the first cell in the first row.
   * This function is exposed on Grid and can thus be called as `grid.startEditing(...)`
   * @param {Object} cellContext Cell specified in format { id: 'x', columnId/column/field: 'xxx' }. See
   * {@link Grid.view.Grid#function-getCell} for details.
   * @fires startCellEdit
   * @returns {Promise} Resolved promise returns`true` if editing has been started, `false` if an {@link Core.widget.Editor#event-beforeStart} listener
   * has vetoed the edit.
   * @category Editing
   * @on-owner
   */
  async startEditing(cellContext = {}) {
    const me = this;
    // If disabled no can do.
    if (!me.disabled) {
      var _cellContext, _grid$focusedCell, _me$onCellEditStart;
      const {
        grid
      } = me;
      // If we got here from keyMap, start editing currently focused cell instead
      if ((_cellContext = cellContext) !== null && _cellContext !== void 0 && _cellContext.fromKeyMap) {
        cellContext = me.grid.focusedCell;
      }
      // When cell context is not available add the first cell context
      if (ObjectHelper.isEmpty(cellContext)) {
        cellContext.id = grid.firstVisibleRow.id;
      }
      // Has to expand before normalizing to a Location, since Location only maps to visible rows
      if (grid.store.isTree && grid.features.tree) {
        const record = cellContext.id ? grid.store.getById(cellContext.id) : cellContext.record ?? grid.store.getAt(cellContext.row);
        if (record) {
          await grid.expandTo(record);
        } else {
          return false;
        }
      }
      const editorContext = me.getEditingContext(cellContext);
      // Cannot edit hidden columns and columns without an editor
      // Cannot edit special rows (groups etc).
      if (!editorContext) {
        return false;
      }
      if (me.editorContext) {
        me.cancelEditing();
      }
      // Now that we know we can edit this cell, scroll the record into view and register it as last focusedCell
      // While any potential scroll may be async, the desired cell will be rendered immediately.
      if (!((_grid$focusedCell = grid.focusedCell) !== null && _grid$focusedCell !== void 0 && _grid$focusedCell.equals(editorContext))) {
        grid.focusCell(editorContext);
      }
      /**
       * Fires on the owning Grid before editing starts, return `false` to prevent editing
       * @event beforeCellEditStart
       * @on-owner
       * @preventable
       * @param {Grid.view.Grid} source Owner grid
       * @param {Grid.util.Location} editorContext Editing context
       * @param {Grid.column.Column} editorContext.column Target column
       * @param {Core.data.Model} editorContext.record Target record
       * @param {HTMLElement} editorContext.cell Target cell
       * @param {Core.widget.Field} editorContext.editor The input field that the column is configured
       * with (see {@link Grid.column.Column#config-field}). This property mey be replaced
       * to be a different {@link Core.widget.Field field} in the handler, to take effect
       * just for the impending edit.
       * @param {Function} [editorContext.finalize] An async function may be injected into this property
       * which performs asynchronous finalization tasks such as complex validation of confirmation. The
       * value `true` or `false` must be returned.
       * @param {Object} [editorContext.finalize.context] An object describing the editing context upon requested
       * completion of the edit.
       * @param {*} editorContext.value Cell value
       */
      if (grid.trigger('beforeCellEditStart', {
        grid,
        editorContext
      }) === false) {
        return false;
      }
      const editor = editorContext.editor = me.getEditorForCell(editorContext),
        {
          row,
          cell,
          record
        } = editorContext;
      // Prevent highlight when setting the value in the editor
      editor.inputField.highlightExternalChange = false;
      editor.cellEditorContext = editorContext;
      editor.render(cell);
      // CSS state must be set before the startEdit causes the Editor to align itself
      // because if its target is overflow:hidden, it automatically constrains its size.
      cell.classList.add('b-editing');
      row.addCls('b-editing');
      // Attempt to start edit.
      // We will set up our context in onEditorStart *if* the start was successful.
      if (!(await editor.startEdit({
        target: cell,
        field: editor.inputField.name || editorContext.column.field,
        value: editorContext.value,
        record
      }))) {
        // If the editor was vetoed, undo the CSS state.
        cell.classList.remove('b-editing');
        row.removeCls('b-editing');
      }
      (_me$onCellEditStart = me.onCellEditStart) === null || _me$onCellEditStart === void 0 ? void 0 : _me$onCellEditStart.call(me);
      return true;
    }
    return false;
  }
  /**
   * Cancel editing, destroys the editor
   * This function is exposed on Grid and can thus be called as `grid.cancelEditing(...)`
   * @param {Boolean} silent Pass true to prevent method from firing event
   * @fires cancelCellEdit
   * @category Editing
   * @on-owner
   */
  cancelEditing(silent = false, triggeredByEvent) {
    var _me$afterCellEdit;
    const me = this,
      {
        editor
      } = me;
    if (!me.isEditing) {
      return;
    }
    // If called from keyMap, first argument is an event, ignore that
    if (silent.fromKeyMap) {
      triggeredByEvent = silent;
      silent = false;
    }
    me.muteEvents = silent;
    editor.cancelEdit(triggeredByEvent);
    me.muteEvents = false;
    // In case editing is canceled while waiting for finishing promise
    me.finishEditingPromise = false;
    (_me$afterCellEdit = me.afterCellEdit) === null || _me$afterCellEdit === void 0 ? void 0 : _me$afterCellEdit.call(me);
  }
  /**
   * Finish editing, update the underlying record and destroy the editor
   * This function is exposed on Grid and can thus be called as `grid.finishEditing(...)`
   * @fires finishCellEdit
   * @category Editing
   * @returns {Promise} Resolved promise returns `false` if the edit could not be finished due to the value being invalid or the
   * Editor's `complete` event was vetoed.
   * @on-owner
   */
  async finishEditing() {
    const me = this,
      {
        editorContext,
        grid
      } = me;
    let result = false;
    // If already waiting for finishing promise, return that
    if (me.finishEditingPromise) {
      return me.finishEditingPromise;
    }
    if (editorContext) {
      var _me$afterCellEdit2;
      const {
        column
      } = editorContext;
      // If completeEdit finds that the editor context has a finalize method in it,
      // it will *await* the completion of that method before completing the edit
      // so we must await completeEdit.
      // We can override that finalize method by passing the column's own finalizeCellEdit.
      // Set a flag (promise) indicating that we are in the middle of editing finalization
      me.finishEditingPromise = editorContext.editor.completeEdit(column.bindCallback(column.finalizeCellEdit));
      result = await me.finishEditingPromise;
      // If grid is animating, wait for it to finish to not start a follow-up edit when things are moving
      // (only applies to Scheduler for now, tested in Schedulers CellEdit.t.js)
      await grid.waitForAnimations();
      // reset the flag
      me.finishEditingPromise = null;
      (_me$afterCellEdit2 = me.afterCellEdit) === null || _me$afterCellEdit2 === void 0 ? void 0 : _me$afterCellEdit2.call(me);
    }
    return result;
  }
  //endregion
  //region Events
  /**
   * Event handler added when editing is active called when user clicks a cell in the grid during editing.
   * It finishes editing and moves editor to the selected cell instead.
   * @private
   * @category Internal event handling
   */
  async onCellClickWhileEditing({
    event,
    cellSelector
  }) {
    const me = this;
    // React cell editor is configured with `allowMouseEvents=true` to prevent editor from swallowing mouse events
    // We ignore these events from editor here to not prevent editing
    if (event.target.closest('.b-editor')) {
      return;
    }
    if (DomHelper.isTouchEvent || event.target.matches(me.ignoreCSSSelector)) {
      await me.finishEditing();
      return;
    }
    // Ignore clicks if async finalization is running
    if (me.finishEditingPromise) {
      return;
    }
    // Ignore clicks in the editor.
    if (me.editorContext && !me.editorContext.editor.owns(event.target)) {
      if (me.getEditingContext(cellSelector)) {
        // Attempt to finish the current edit.
        // Will return false if the field is invalid.
        if (await me.finishEditing()) {
          if (me.continueEditingOnCellClick) {
            await me.startEditing(cellSelector);
          }
        }
        // Previous edit was invalid, return to it.
        else {
          me.grid.focusCell(me.editorContext);
          me.editor.inputField.focus();
        }
      } else {
        await me.finishEditing();
      }
    }
  }
  /**
   * Starts editing if user taps selected cell again on touch device. Chained function called when user clicks a cell.
   * @private
   * @category Internal event handling
   */
  async onCellClick({
    source: grid,
    cellSelector,
    target,
    event,
    column
  }) {
    if (column.onCellClick) {
      // Columns may provide their own handling of cell editing
      return;
    }
    const me = this,
      {
        focusedCell
      } = me.client;
    if (target.closest('.b-tree-expander')) {
      return false;
    } else if (DomHelper.isTouchEvent && me._lastCellClicked === (focusedCell === null || focusedCell === void 0 ? void 0 : focusedCell.cell) && event.timeStamp - me.touchEditDelay > me._lastCellClickedTime) {
      await me.startEditing(cellSelector);
    } else if (this.triggerEvent === 'cellclick') {
      await me.onTriggerEditEvent({
        cellSelector,
        target
      });
    }
    me._lastCellClicked = focusedCell === null || focusedCell === void 0 ? void 0 : focusedCell.cell;
    me._lastCellClickedTime = event.timeStamp;
  }
  // onElementPointerUp should be used to cancel editing before toggleCollapse handled
  // otherwise data collisions may be happened
  onElementPointerUp(event) {
    if (event.target.closest('.b-tree-expander')) {
      this.cancelEditing(undefined, event);
    }
  }
  /**
   * Called when the user triggers the edit action in {@link #config-triggerEvent} config. Starts editing.
   * @private
   * @category Internal event handling
   */
  async onTriggerEditEvent({
    cellSelector,
    target,
    event
  }) {
    var _client$features$cell;
    const {
      editorContext,
      client
    } = this;
    if (target.closest('.b-tree-expander') || DomHelper.isTouchEvent && event.type === 'dblclick') {
      return;
    }
    // Should not start editing if cellMenu configured to be shown on event
    if (event && ((_client$features$cell = client.features.cellMenu) === null || _client$features$cell === void 0 ? void 0 : _client$features$cell.triggerEvent) === event.type) {
      return;
    }
    if (editorContext) {
      // If we are already editing the cellSelector cell, or the editor cannot finish editing
      // then we must not attempt to start an edit.
      if (editorContext.equals(this.grid.normalizeCellContext(cellSelector)) || !(await this.finishEditing())) {
        return;
      }
    }
    await this.startEditing(cellSelector);
  }
  /**
   * Update the input field if underlying data changes during edit.
   * @private
   * @category Internal event handling
   */
  onStoreUpdate({
    changes,
    record
  }) {
    const {
      editorContext
    } = this;
    if (editorContext !== null && editorContext !== void 0 && editorContext.editor.isVisible) {
      if (record === editorContext.record && editorContext.editor.dataField in changes) {
        editorContext.editor.refreshEdit();
      }
    }
  }
  onStoreBeforeSort() {
    var _this$editorContext2;
    const editor = (_this$editorContext2 = this.editorContext) === null || _this$editorContext2 === void 0 ? void 0 : _this$editorContext2.editor;
    if (this.isEditing && !(editor !== null && editor !== void 0 && editor.isFinishing) && !editor.isValid) {
      this.cancelEditing();
    }
  }
  /**
   * Realign editor if grid renders rows while editing is ongoing (as a result to autoCommit or WebSocket data received).
   * @private
   * @category Internal event handling
   */
  onGridRefreshed() {
    const me = this,
      {
        grid,
        editorContext
      } = me;
    if (editorContext && grid.isVisible && grid.focusedCell) {
      const cell = grid.getCell(grid.focusedCell),
        {
          editor
        } = editorContext;
      // If refresh was triggered by the data change in onEditComplete
      // do not re-show the editor.
      if (cell && DomHelper.isInView(cell) && !editor.isFinishing) {
        editorContext._cell = cell;
        // Editor is inside the cell for A11Y reasons.
        // So any refresh will remove its DOM.
        // We need to silently restore and refocus it.
        GlobalEvents.suspendFocusEvents();
        editor.render(cell);
        editor.showBy(cell);
        editor.focus();
        GlobalEvents.resumeFocusEvents();
      } else {
        me.cancelEditing();
      }
    }
  }
  // Gets selected records or selected cells records
  get gridSelection() {
    return [...this.grid.selectedRows, ...this.grid.selectedCells];
  }
  // Tells keyMap what actions is available in certain conditions
  isActionAvailable({
    actionName,
    event
  }) {
    const me = this;
    if (!me.disabled && !event.target.closest('.b-grid-header')) {
      if (me.isEditing) {
        if (actionName === 'finishAllSelected') {
          return me.multiEdit && me.gridSelection.length > 1;
        } else if (editingActions[actionName]) {
          return true;
        }
      } else if (actionName === 'startEditing') {
        return me.grid.focusedCell.cell === event.target;
      }
    }
    return false;
  }
  // Will copy edited field value to all selected records
  async finishAllSelected() {
    const me = this,
      {
        dataField,
        record
      } = me.editor;
    if ((await me.finishEditing()) && !me.isDestroyed) {
      for (const selected of me.gridSelection) {
        if (selected.isModel) {
          if (selected !== record) {
            selected[dataField] = record[dataField];
          }
        } else {
          selected.record.set(selected.column.field, record[dataField]);
        }
      }
    }
  }
  // Will finish editing and start editing next row (unless it's a touch device)
  // If addNewAtEnd, it will create a new row and edit that one if currently editing last row
  async finishAndEditNextRow(event, previous = false) {
    const me = this,
      {
        grid
      } = me,
      {
        record
      } = me.editorContext;
    let nextCell;
    if (await me.finishEditing()) {
      // Might be destroyed during the async operation
      if (me.isDestroyed) {
        return;
      }
      // Finalizing might have been blocked by an invalid value
      if (!me.isEditing) {
        // Move to previous
        if (previous) {
          nextCell = grid.internalNextPrevRow(false, true, event, false);
        }
        // Move to next
        else {
          // If we are at the last editable cell, optionally add a new row
          if (me.addNewAtEnd && record === grid.store.last) {
            await me.doAddNewAtEnd();
          }
          if (!me.isDestroyed) {
            nextCell = grid.internalNextPrevRow(true, true, event);
          }
        }
        // If we have moved, and we are configure to edit the next cell on Enter key...
        if (nextCell && me.editNextOnEnterPress && !grid.touch) {
          me.startEditing(nextCell);
        }
      }
    }
  }
  // Will finish editing and start editing previous row
  finishAndEditPrevRow(event) {
    this.finishAndEditNextRow(event, true);
  }
  // Will finish editing and start editing next cell
  // If addNewAtEnd, it will create a new row and edit that one if currently editing last row
  async finishAndEditNextCell(event, previous = false) {
    const me = this,
      {
        focusedCell
      } = me.grid;
    if (focusedCell) {
      let cellInfo = me.getAdjacentEditableCell(focusedCell, !previous);
      // If we are at the last editable cell, optionally add a new row
      if (!cellInfo && !previous && me.addNewAtEnd) {
        const currentEditableFinalizationResult = await me.finishEditing();
        if (currentEditableFinalizationResult === true) {
          await this.doAddNewAtEnd();
          // Re-grab the next editable cell
          cellInfo = !me.isDestroyed && me.getAdjacentEditableCell(focusedCell, !previous);
        }
      }
      if (cellInfo) {
        let finalizationResult = true;
        if (me.isEditing) {
          finalizationResult = await me.finishEditing();
        }
        if (!me.isDestroyed && finalizationResult) {
          me.grid.focusCell(cellInfo, {
            animate: me.focusCellAnimationDuration
          });
          if (!(await me.startEditing(cellInfo))) {
            // if editing a cell was vetoed, move on and try again
            await me.finishAndEditNextCell(event, previous);
          }
        }
      }
    }
  }
  // Will finish editing and start editing next cell
  finishAndEditPrevCell(event) {
    this.finishAndEditNextCell(event, true);
  }
  // Handles autoedit
  async onElementKeyDown(event) {
    const me = this,
      {
        grid
      } = me,
      {
        focusedCell
      } = grid;
    // flagging event with handled = true used to signal that other features should probably not care about it
    if (event.handled || !me.autoEdit || me.isEditing || !focusedCell || focusedCell.isActionable || event.ctrlKey) {
      return;
    }
    const {
        key
      } = event,
      isDelete = event.key === 'Delete' || event.key === 'Backspace',
      {
        gridSelection
      } = isDelete ? me : {},
      isMultiDelete = me.multiEdit && (gridSelection === null || gridSelection === void 0 ? void 0 : gridSelection.length) > 1;
    // Any character or space starts editing while autoedit is true
    if ((key.length <= 1 || isDelete && !isMultiDelete) && (await me.startEditing(focusedCell))) {
      const {
          inputField
        } = me.editor,
        {
          input
        } = inputField;
      // if editing started with a keypress and the editor has an input field, set its value
      if (input) {
        // Simulate a keydown in an input field by setting input value
        // plus running our internal processing of that event
        inputField.internalOnKeyEvent(event);
        if (!event.defaultPrevented) {
          input.value = isDelete ? '' : key;
          inputField.internalOnInput(event);
        }
      }
      event.preventDefault();
    }
    // If deleting while selected multiple rows or cells, the records are changed directly
    else if (isMultiDelete) {
      /**
       * Fires on the owning Grid before deleting a range of selected cell values by pressing `Backspace` or `Del`
       * buttons while {@link #config-autoEdit} is set to `true`. Return `false` to prevent editing.
       * @event beforeCellDelete
       * @on-owner
       * @preventable
       * @param {Grid.view.Grid} source Owner grid
       * @param {Array<Grid.util.Location|Core.data.Model>} gridSelection An array of cell selectors or records
       * that will have their values deleted (the records themself will not get deleted, only visible column
       * values).
       */
      if (grid.trigger('beforeCellRangeDelete', {
        grid,
        gridSelection
      }) !== false) {
        for (const selected of gridSelection) {
          if (selected.isModel) {
            grid.columns.visibleColumns.forEach(col => {
              !col.readOnly && selected.set(col.field, null);
            });
          } else if (!selected.column.readOnly) {
            selected.record.set(selected.column.field, null);
          }
        }
      }
    }
  }
  // Prevents keys which the Grid handles from bubbling to the grid while editing
  onEditorKeydown(event) {
    if (event.key.length !== 1 && this.grid.matchKeyMapEntry(event) && !this.grid.matchKeyMapEntry(event, this.keyMap)) {
      // Don't allow browser key commands such as PAGE-UP, PAGE-DOWN to continue.
      if (!event.key.startsWith('Arrow') && !event.key === 'Backspace') {
        event.preventDefault();
      }
      event.handled = true;
      event.stopPropagation();
      return false;
    }
  }
  /**
   * Cancel editing on widget focusout
   * @private
   */
  async onEditorFocusOut(event) {
    const me = this,
      {
        grid,
        editor,
        editorContext
      } = me,
      toCell = new Location(event.relatedTarget),
      isEditableCellClick = toCell.grid === grid && me.getEditingContext(toCell);
    // If the editor is not losing focus as a result of its tidying up process
    // And focus is moving to outside of the editor, then explicitly terminate.
    if (editorContext && !editor.isFinishing && editor.owns(event._target)) {
      if (me.blurAction === 'cancel') {
        me.cancelEditing(undefined, event);
      }
      // If not already in the middle of editing finalization (that could be async)
      // and it's not a onCellClickWhileEditing situation, finish the edit.
      else if (!me.finishEditingPromise && (me.triggerEvent === 'cellclick' || me.triggerEvent !== 'cellclick' && !isEditableCellClick)) {
        await me.finishEditing();
      }
    }
  }
  onEditorFocusIn(event) {
    const widget = event.toWidget;
    if (widget === this.editor.inputField) {
      if (this.autoSelect && widget.selectAll && !widget.readOnly && !widget.disabled) {
        widget.selectAll();
      }
    }
  }
  /**
   * Cancel edit on touch outside of grid for mobile Safari (focusout not triggering unless you touch something focusable)
   * @private
   */
  async onTapOut({
    event
  }) {
    const me = this;
    if (!me.grid.bodyContainer.contains(event.target) || event.button) {
      if (!me.editor.owns(event.target)) {
        me.editingStoppedByTapOutside = true;
        if (me.blurAction === 'cancel') {
          me.cancelEditing(undefined, event);
        } else {
          await me.finishEditing();
        }
        delete me.editingStoppedByTapOutside;
      }
    }
  }
  /**
   * Finish editing if clicking below rows (only applies when grid is higher than rows).
   * @private
   * @category Internal event handling
   */
  async onElementClick(event) {
    if (event.target.classList.contains('b-grid-body-container') && this.editorContext) {
      await this.finishEditing();
    }
  }
  //endregion
}

CellEdit._$name = 'CellEdit';
GridFeatureManager.registerFeature(CellEdit, true);

/**
 * @module Grid/feature/CellMenu
 */
/**
 * Right click to display context menu for cells.
 *
 * To invoke the cell menu in a keyboard-accessible manner, use the `SPACE` key when the cell is focused.
 *
 * ### Default cell menu items
 *
 * The Cell menu feature provides only one item by default:
 *
 * | Reference              | Text   | Weight | Description         |
 * |------------------------|--------|--------|---------------------|
 * | `removeRow`            | Delete | 100    | Delete row record   |
 *
 * And all the other items are populated by the other features:
 *
 * | Reference              | Text             | Weight | Feature                           | Description                                           |
 * |------------------------|------------------|--------|-----------------------------------|-------------------------------------------------------|
 * | `cut`                  | Cut record       | 110    | {@link Grid/feature/RowCopyPaste} | Cut row record                                        |
 * | `copy`                 | Copy record      | 120    | {@link Grid/feature/RowCopyPaste} | Copy row record                                       |
 * | `paste`                | Paste record     | 130    | {@link Grid/feature/RowCopyPaste} | Paste copied row records                              |
 * | `search`               | Search for value | 200    | {@link Grid/feature/Search}       | Search for the selected cell text                     |
 * | `filterDateEquals`     | On               | 300    | {@link Grid/feature/Filter}       | Filters by the column field, equal to the cell value  |
 * | `filterDateBefore`     | Before           | 310    | {@link Grid/feature/Filter}       | Filters by the column field, less than the cell value |
 * | `filterDateAfter`      | After            | 320    | {@link Grid/feature/Filter}       | Filters by the column field, more than the cell value |
 * | `filterNumberEquals`   | Equals           | 300    | {@link Grid/feature/Filter}       | Filters by the column field, equal to the cell value  |
 * | `filterNumberLess`     | Less than        | 310    | {@link Grid/feature/Filter}       | Filters by the column field, less than the cell value |
 * | `filterNumberMore`     | More than        | 320    | {@link Grid/feature/Filter}       | Filters by the column field, more than the cell value |
 * | `filterDurationEquals` | Equals           | 300    | {@link Grid/feature/Filter}       | Filters by the column field, equal to the cell value  |
 * | `filterDurationLess`   | Less than        | 310    | {@link Grid/feature/Filter}       | Filters by the column field, less than the cell value |
 * | `filterDurationMore`   | More than        | 320    | {@link Grid/feature/Filter}       | Filters by the column field, more than the cell value |
 * | `filterStringEquals`   | Equals           | 300    | {@link Grid/feature/Filter}       | Filters by the column field, equal to the cell value  |
 * | `filterRemove`         | Remove filter    | 400    | {@link Grid/feature/Filter}       | Stops filtering by selected column field              |
 *
 * ### Customizing the menu items
 *
 * The menu items in the Cell menu can be customized, existing items can be changed or removed,
 * and new items can be added. This is handled using the `items` config of the feature.
 *
 * Add extra items for all columns:
 *
 * ```javascript
 * const grid = new Grid({
 *     features : {
 *         cellMenu : {
 *             items : {
 *                 extraItem : {
 *                     text   : 'My cell item',
 *                     icon   : 'fa fa-bus',
 *                     weight : 200,
 *                     onItem : () => ...
 *                 }
 *             }
 *         }
 *     }
 * });
 * ```
 *
 * It is also possible to add items using columns config. See examples below.
 *
 * Add extra items for a single column:
 *
 * ```javascript
 * const grid = new Grid({
 *     columns: [
 *         {
 *             field         : 'city',
 *             text          : 'City',
 *             cellMenuItems : {
 *                 columnItem : {
 *                     text   : 'My unique cell item',
 *                     icon   : 'fa fa-beer',
 *                     onItem : () => ...
 *                 }
 *             }
 *         }
 *     ]
 * });
 * ```
 *
 * Remove existing item:
 *
 * ```javascript
 * const scheduler = new Scheduler({
 *     features : {
 *         cellMenu : {
 *             items : {
 *                 removeRow : false
 *             }
 *         }
 *     }
 * });
 * ```
 *
 * Customize existing item:
 *
 * ```javascript
 * const scheduler = new Scheduler({
 *     features : {
 *         cellMenu : {
 *             items : {
 *                 removeRow : {
 *                     text : 'Throw away',
 *                     icon : 'b-fa b-fa-dumpster'
 *                 }
 *             }
 *         }
 *     }
 * });
 * ```
 *
 * It is also possible to manipulate the default items and add new items in the processing function:
 *
 * ```javascript
 * const grid = new Grid({
 *     features : {
 *         cellMenu : {
 *             processItems({items, record}) {
 *                 if (record.cost > 5000) {
 *                     items.myItem = { text : 'Split cost' };
 *                 }
 *             }
 *         }
 *     }
 * });
 * ```
 *
 * Full information of the menu customization can be found in the ["Customizing the Cell menu and the Header menu"](#Grid/guides/customization/contextmenu.md)
 * guide.
 *
 * This feature is **enabled** by default.
 *
 * ## Keyboard shortcuts
 * This feature has the following default keyboard shortcuts:
 *
 * | Keys         | Action                 | Action description                            |
 * |--------------|------------------------|-----------------------------------------------|
 * | `Space`      | *showContextMenuByKey* | Shows context menu for currently focused cell |
 * | `Ctrl+Space` | *showContextMenuByKey* | Shows context menu for currently focused cell |
 *
 * <div class="note">Please note that <code>Ctrl</code> is the equivalent to <code>Command</code> and <code>Alt</code>
 * is the equivalent to <code>Option</code> for Mac users</div>
 *
 * For more information on how to customize keyboard shortcuts, please see
 * [our guide](#Grid/guides/customization/keymap.md).
 *
 * @extends Core/feature/base/ContextMenuBase
 * @demo Grid/contextmenu
 * @classtype cellMenu
 * @inlineexample Grid/feature/CellMenu.js
 * @feature
 */
class CellMenu extends ContextMenuBase {
  //region Config
  static get $name() {
    return 'CellMenu';
  }
  static get defaultConfig() {
    return {
      /**
       * A function called before displaying the menu that allows manipulations of its items.
       * Returning `false` from this function prevents the menu being shown.
       *
       * ```javascript
       * features : {
       *     cellMenu : {
       *         processItems({ items, record, column }) {
       *             // Add or hide existing items here as needed
       *             items.myAction = {
       *                 text   : 'Cool action',
       *                 icon   : 'b-fa b-fa-fw b-fa-ban',
       *                 onItem : () => console.log(`Clicked ${record.name}`),
       *                 weight : 1000 // Move to end
       *             };
       *
       *             if (!record.allowDelete) {
       *                 items.removeRow.hidden = true;
       *             }
       *         }
       *     }
       * },
       * ```
       * @param {Object} context An object with information about the menu being shown
       * @param {Core.data.Model} context.record The record representing the current row
       * @param {Grid.column.Column} context.column The current column
       * @param {Object<String,MenuItemConfig>} context.items An object containing the
       * {@link Core.widget.MenuItem menu item} configs keyed by their id
       * @param {Event} context.event The DOM event object that triggered the show
       * @config {Function}
       * @preventable
       */
      processItems: null,
      /**
       * {@link Core.widget.Menu} items object containing named child menu items to apply to the feature's
       * provided context menu.
       *
       * This may add extra items as below, but you can also configure, or remove any of the default items by
       * configuring the name of the item as `null`:
       *
       * ```javascript
       * features : {
       *     cellMenu : {
       *         // This object is applied to the Feature's predefined default items
       *         items : {
       *             switchToDog : {
       *                 text : 'Dog',
       *                 icon : 'b-fa b-fa-fw b-fa-dog',
       *                 onItem({record}) {
       *                     record.dog = true;
       *                     record.cat = false;
       *                 },
       *                 weight : 500     // Make this second from end
       *             },
       *             switchToCat : {
       *                 text : 'Cat',
       *                 icon : 'b-fa b-fa-fw b-fa-cat',
       *                 onItem({record}) {
       *                     record.dog = false;
       *                     record.cat = true;
       *                 },
       *                 weight : 510     // Make this sink to end
       *             },
       *             removeRow : {
       *                 // Change icon for the delete item
       *                 icon : 'b-fa b-fa-times'
       *             },
       *             secretItem : null
       *         }
       *     }
       * },
       * ```
       *
       * @config {Object<String,MenuItemConfig|Boolean|null>}
       */
      items: null,
      type: 'cell'
      /**
       * See {@link #keyboard-shortcuts Keyboard shortcuts} for details
       * @config {Object<String,String>} keyMap
       */
    };
  }

  static get pluginConfig() {
    const config = super.pluginConfig;
    config.chain.push('populateCellMenu');
    return config;
  }
  //endregion
  //region Events
  /**
   * This event fires on the owning grid before the context menu is shown for a cell.
   * Allows manipulation of the items to show in the same way as in the {@link #config-processItems}.
   *
   * Returning `false` from a listener prevents the menu from being shown.
   *
   * @event cellMenuBeforeShow
   * @preventable
   * @on-owner
   * @param {Grid.view.Grid} source The grid
   * @param {Core.widget.Menu} menu The menu
   * @param {Object<String,MenuItemConfig>} items Menu item configs
   * @param {Grid.column.Column} column Column
   * @param {Core.data.Model} record Record
   */
  /**
   * This event fires on the owning grid after the context menu is shown for a cell.
   * @event cellMenuShow
   * @on-owner
   * @param {Grid.view.Grid} source The grid
   * @param {Core.widget.Menu} menu The menu
   * @param {Object<String,MenuItemConfig>} items Menu item configs
   * @param {Grid.column.Column} column Column
   * @param {Core.data.Model} record Record
   */
  /**
   * This event fires on the owning grid when an item is selected in the cell context menu.
   * @event cellMenuItem
   * @on-owner
   * @param {Grid.view.Grid} source The grid
   * @param {Core.widget.Menu} menu The menu
   * @param {Core.widget.MenuItem} item Selected menu item
   * @param {Grid.column.Column} column Column
   * @param {Core.data.Model} record Record
   */
  /**
   * This event fires on the owning grid when a check item is toggled in the cell context menu.
   * @event cellMenuToggleItem
   * @on-owner
   * @param {Grid.view.Grid} source The grid
   * @param {Core.widget.Menu} menu The menu
   * @param {Core.widget.MenuItem} item Selected menu item
   * @param {Grid.column.Column} column Column
   * @param {Core.data.Model} record Record
   * @param {Boolean} checked Checked or not
   */
  //endregion
  //region Menu handlers
  showContextMenu(eventParams) {
    const me = this,
      {
        cellSelector,
        event
      } = eventParams;
    // Process the gesture as navigation so that the use may select/multiselect
    // the items to include in their context menu operation.
    // Also select if not already selected.
    me.client.focusCell(cellSelector, {
      doSelect: !me.client.isSelected(cellSelector),
      event
    });
    super.showContextMenu(eventParams);
  }
  shouldShowMenu({
    column
  }) {
    return column && column.enableCellContextMenu !== false;
  }
  getDataFromEvent(event) {
    const cellData = this.client.getCellDataFromEvent(event);
    // Only yield data to show a menu if we are on a cell
    if (cellData) {
      return ObjectHelper.assign(super.getDataFromEvent(event), cellData);
    }
  }
  beforeContextMenuShow({
    record,
    items,
    column
  }) {
    if (column.cellMenuItems === false) {
      return false;
    }
    if (!record || record.isSpecialRow) {
      items.removeRow = false;
    }
  }
  //endregion
  //region Getters/Setters
  populateCellMenu({
    items,
    column,
    record
  }) {
    const {
      client
    } = this;
    if (column !== null && column !== void 0 && column.cellMenuItems) {
      ObjectHelper.merge(items, column.cellMenuItems);
    }
    if (!client.readOnly) {
      items.removeRow = {
        text: 'L{removeRow}',
        localeClass: this,
        icon: 'b-fw-icon b-icon-trash',
        cls: 'b-separator',
        weight: 100,
        disabled: record.readOnly,
        onItem: () => client.store.remove(client.selectedRecords.filter(r => !r.readOnly))
      };
    }
  }
  get showMenu() {
    return true;
  }
  //endregion
}

CellMenu.featureClass = '';
CellMenu._$name = 'CellMenu';
GridFeatureManager.registerFeature(CellMenu, true, ['Grid', 'Scheduler']);
GridFeatureManager.registerFeature(CellMenu, false, ['Gantt']);

/**
 * @module Grid/feature/ColumnDragToolbar
 */
/**
 * Displays a toolbar while dragging column headers. Drop on a button in the toolbar to activate a certain function,
 * for example to group by that column. This feature simplifies certain operations on touch devices.
 *
 * This feature is <strong>disabled</strong> by default, but turned on automatically on touch devices.
 *
 * @extends Core/mixin/InstancePlugin
 *
 * @classtype columnDragToolbar
 * @inlineexample Grid/feature/ColumnDragToolbar.js
 * @demo Grid/columndragtoolbar
 * @feature
 */
class ColumnDragToolbar extends Delayable(InstancePlugin) {
  //region Config
  static get $name() {
    return 'ColumnDragToolbar';
  }
  // Plugin configuration. This plugin chains some of the functions in Grid
  static get pluginConfig() {
    return {
      after: ['render']
    };
  }
  //endregion
  //region Init
  construct(grid, config) {
    if (grid.features.columnReorder) {
      grid.features.columnReorder.ion({
        beforeDestroy: 'onColumnReorderBeforeDestroy',
        thisObj: this
      });
    }
    this.grid = grid;
    super.construct(grid, config);
  }
  doDestroy() {
    const me = this;
    if (me.grid.features.columnReorder && !me.grid.features.columnReorder.isDestroyed) {
      me.detachFromColumnReorder();
    }
    me.element && me.element.remove();
    me.element = null;
    super.doDestroy();
  }
  doDisable(disable) {
    if (this.initialized) {
      if (disable) {
        this.detachFromColumnReorder();
      } else {
        this.init();
      }
    }
    super.doDisable(disable);
  }
  init() {
    const me = this,
      grid = me.grid;
    if (!grid.features.columnReorder) {
      return;
    }
    me.reorderDetacher = grid.features.columnReorder.ion({
      gridheaderdragstart({
        context
      }) {
        const column = grid.columns.getById(context.element.dataset.columnId);
        me.showToolbar(column);
      },
      gridheaderdrag: ({
        context
      }) => me.onDrag(context),
      gridheaderabort: () => {
        me.hideToolbar();
      },
      gridheaderdrop: ({
        context
      }) => {
        if (context.valid) {
          me.hideToolbar();
        } else {
          me.onDrop(context);
        }
      },
      thisObj: me
    });
    me.initialized = true;
  }
  onColumnReorderBeforeDestroy() {
    this.detachFromColumnReorder();
  }
  detachFromColumnReorder() {
    const me = this;
    me.grid.features.columnReorder.un('beforedestroy', me.onColumnReorderBeforeDestroy, me);
    me.reorderDetacher && me.reorderDetacher();
    me.reorderDetacher = null;
  }
  /**
   * Initializes this feature on grid render.
   * @private
   */
  render() {
    if (!this.initialized) {
      this.init();
    }
  }
  //endregion
  //region Toolbar
  showToolbar(column) {
    const me = this,
      buttons = me.grid.getColumnDragToolbarItems(column, []),
      groups = [];
    me.clearTimeout(me.buttonHideTimer);
    buttons.forEach(button => {
      button.text = button.localeClass.L(button.text);
      let group = groups.find(group => group.text === button.group);
      if (!group) {
        group = {
          text: button.localeClass.L(button.group),
          buttons: []
        };
        groups.push(group);
      }
      group.buttons.push(button);
    });
    me.element = DomHelper.append(me.grid.element, me.template(groups));
    me.groups = groups;
    me.buttons = buttons;
    me.column = column;
  }
  async hideToolbar() {
    const me = this,
      element = me.element;
    if (element) {
      element.classList.add('b-remove');
      await EventHelper.waitForTransitionEnd({
        element,
        mode: 'animation',
        thisObj: me.client
      });
      element.remove();
      me.element = null;
    }
  }
  //endregion
  //region Events
  onDrag(info) {
    var _info$targetElement;
    const me = this;
    if (info.dragProxy.getBoundingClientRect().top - me.grid.element.getBoundingClientRect().top > 100) {
      me.element.classList.add('b-closer');
    } else {
      me.element.classList.remove('b-closer');
    }
    if (me.hoveringButton) {
      me.hoveringButton.classList.remove('b-hover');
      me.hoveringButton = null;
    }
    if ((_info$targetElement = info.targetElement) !== null && _info$targetElement !== void 0 && _info$targetElement.closest('.b-columndragtoolbar')) {
      me.element.classList.add('b-hover');
      const button = info.targetElement.closest('.b-columndragtoolbar  .b-target-button:not([data-disabled=true])');
      if (button) {
        button.classList.add('b-hover');
        me.hoveringButton = button;
      }
    } else {
      me.element.classList.remove('b-hover');
    }
  }
  onDrop(info) {
    const me = this;
    if (info.targetElement && info.targetElement.matches('.b-columndragtoolbar .b-target-button:not([data-disabled=true])')) {
      const buttonEl = info.targetElement,
        button = me.buttons.find(button => button.ref === buttonEl.dataset.ref);
      if (button) {
        buttonEl.classList.add('b-activate');
        me.buttonHideTimer = me.setTimeout(() => {
          me.hideToolbar();
          button.onDrop({
            column: me.column
          });
        }, 100);
      }
    } else {
      me.hideToolbar();
    }
  }
  //endregion
  template(groups) {
    return TemplateHelper.tpl`
            <div class="b-columndragtoolbar">     
            <div class="b-title"></div>          
            ${groups.map(group => TemplateHelper.tpl`
                <div class="b-group">
                    <div class="b-buttons">
                    ${group.buttons.map(btn => TemplateHelper.tpl`
                        <div class="b-target-button" data-ref="${btn.ref}" data-disabled="${btn.disabled}">
                            <i class="${btn.icon}"></i>
                            ${btn.text}
                        </div>
                    `)}
                    </div>
                    <div class="b-title">${group.text}</div>
                </div>
            `)}
            </div>`;
  }
}
ColumnDragToolbar.featureClass = 'b-hascolumndragtoolbar';
// used by default on touch devices, can be enabled otherwise
ColumnDragToolbar._$name = 'ColumnDragToolbar';
GridFeatureManager.registerFeature(ColumnDragToolbar, BrowserHelper.isTouchDevice);

/**
 * @module Grid/feature/ColumnPicker
 */
/**
 * Displays a column picker (to show/hide columns) in the header context menu. Columns can be displayed in sub menus
 * by region or tag. Grouped headers are displayed as menu hierarchies.
 *
 * {@inlineexample Grid/feature/ColumnPicker.js}
 *
 * This feature is <strong>enabled</strong> by default.
 *
 * @extends Core/mixin/InstancePlugin
 * @demo Grid/columns
 * @classtype columnPicker
 * @feature
 */
class ColumnPicker extends InstancePlugin {
  //region Config
  static $name = 'ColumnPicker';
  static configurable = {
    /**
     * Groups columns in the picker by region (each region gets its own sub menu)
     * @config {Boolean}
     * @default
     */
    groupByRegion: false,
    /**
     * Groups columns in the picker by tag, each column may be shown under multiple tags. See
     * {@link Grid.column.Column#config-tags}
     * @config {Boolean}
     * @default
     */
    groupByTag: false,
    /**
     * Configure this as `true` to have the fields from the Grid's {@link Core.data.Store}'s
     * {@link Core.data.Store#config-modelClass} added to the menu to create __new__ columns
     * to display the fields.
     *
     * This may be combined with the {@link Grid.view.mixin.GridState stateful} ability of the grid
     * to create a self-configuring grid.
     * @config {Boolean}
     * @default
     */
    createColumnsFromModel: false,
    menuCls: 'b-column-picker-menu b-sub-menu'
  };
  static get pluginConfig() {
    return {
      chain: ['populateHeaderMenu', 'getColumnDragToolbarItems']
    };
  }
  get grid() {
    return this.client;
  }
  //endregion
  //region Context menu
  /**
   * Get menu items, either a straight list of columns or sub menus per subgrid
   * @private
   * @param columnStore Column store to traverse
   * @returns {MenuItemConfig[]} Menu item configs
   */
  getColumnPickerItems(columnStore) {
    const me = this,
      {
        createColumnsFromModel
      } = me;
    let result;
    if (me.groupByRegion) {
      // submenus for grids regions
      result = me.grid.regions.map(region => {
        const columns = me.grid.getSubGrid(region).columns.topColumns;
        return {
          text: StringHelper.capitalize(region),
          menu: me.buildColumnMenu(columns),
          disabled: columns.length === 0,
          region
        };
      });
      if (createColumnsFromModel) {
        result.push({
          text: me.L('L{newColumns}'),
          menu: me.createAutoColumnItems()
        });
      }
    } else if (me.groupByTag) {
      // submenus for column tags
      const tags = {};
      columnStore.topColumns.forEach(column => {
        column.tags && Array.isArray(column.tags) && column.hideable !== false && column.tags.forEach(tag => {
          if (!tags[tag]) {
            tags[tag] = 1;
          }
        });
      });
      result = Object.keys(tags).sort().map(tag => ({
        text: StringHelper.capitalize(tag),
        menu: me.buildColumnMenu(me.getColumnsForTag(tag)),
        tag,
        onBeforeSubMenu: ({
          item,
          itemEl
        }) => {
          me.refreshTagMenu(item, itemEl);
        }
      }));
      if (createColumnsFromModel) {
        result.push({
          text: me.L('L{newColumns}'),
          menu: me.createAutoColumnItems()
        });
      }
    } else {
      // all columns in same menu
      result = me.buildColumnMenu(columnStore.topColumns);
      if (createColumnsFromModel) {
        result.items.push(...ObjectHelper.transformNamedObjectToArray(me.createAutoColumnItems()));
      }
    }
    return result;
  }
  createAutoColumnItems() {
    const me = this,
      {
        grid
      } = me,
      {
        columns,
        store
      } = grid,
      {
        modelClass
      } = store,
      {
        allFields
      } = modelClass,
      result = {};
    for (let i = 0, {
        length
      } = allFields; i < length; i++) {
      const field = allFields[i],
        fieldName = field.name;
      if (!columns.get(fieldName)) {
        // Don't include system-level "internal" fields from the base Model classes like rowHeight or cls.
        if (!field.internal) {
          result[fieldName] = {
            text: field.text || StringHelper.separate(field.name),
            checked: false,
            onToggle: event => {
              const column = columns.get(fieldName);
              if (column) {
                column[event.checked ? 'show' : 'hide']();
              } else {
                columns.add(columns.generateColumnForField(field, {
                  region: me.forColumn.region
                }));
              }
              event.bubbles = false;
            }
          };
        }
      }
    }
    return result;
  }
  /**
   * Get all columns that has the specified tag.
   * @private
   * @param tag
   * @returns {Grid.column.Column[]}
   */
  getColumnsForTag(tag) {
    return this.grid.columns.records.filter(column => column.tags && Array.isArray(column.tags) && column.tags.includes(tag) && column.hideable !== false);
  }
  /**
   * Refreshes checked status for a tag menu. Needed since columns can appear under multiple tags.
   * @private
   */
  refreshTagMenu(item, itemEl) {
    const columns = this.getColumnsForTag(item.tag);
    columns.forEach(column => {
      const subItem = item.items.find(subItem => subItem.column === column);
      if (subItem) subItem.checked = column.hidden !== true;
    });
  }
  /**
   * Traverses columns to build menu items for the column picker.
   * @private
   */
  buildColumnMenu(columns) {
    let currentRegion = columns.length > 0 && columns[0].region;
    const {
        grid
      } = this,
      items = columns.reduce((items, column) => {
        const visibleInRegion = grid.columns.visibleColumns.filter(col => col.region === column.region);
        if (column.hideable !== false) {
          const itemConfig = {
            grid,
            column,
            text: column.headerText,
            checked: column.hidden !== true,
            disabled: column.hidden !== true && visibleInRegion.length === 1,
            cls: column.region !== currentRegion ? 'b-separator' : ''
          };
          currentRegion = column.region;
          if (column.children && !column.isCollapsible) {
            itemConfig.menu = this.buildColumnMenu(column.children);
          }
          items.push(itemConfig);
        }
        return items;
      }, []);
    return {
      cls: this.menuCls,
      items
    };
  }
  /**
   * Populates the header context menu items.
   * @param {Object} options Contains menu items and extra data retrieved from the menu target.
   * @param {Grid.column.Column} options.column Column for which the menu will be shown
   * @param {Object<String,MenuItemConfig|Boolean|null>} options.items A named object to describe menu items
   * @internal
   */
  populateHeaderMenu({
    column,
    items
  }) {
    const me = this,
      {
        columns
      } = me.grid;
    /**
     * The column on which the context menu was invoked.
     * @property {Grid.column.Column} forColumn
     * @readonly
     * @private
     */
    me.forColumn = column;
    if (column.showColumnPicker !== false && columns.some(col => col.hideable)) {
      // column picker
      items.columnPicker = {
        text: 'L{columnsMenu}',
        localeClass: me,
        icon: 'b-fw-icon b-icon-columns',
        cls: 'b-separator',
        weight: 200,
        menu: me.getColumnPickerItems(columns),
        onToggle: me.onColumnToggle,
        disabled: me.disabled
      };
    }
    // menu item for hiding this column
    if (column.hideable !== false && !column.parent.isCollapsible) {
      items.hideColumn = {
        text: 'L{hideColumn}',
        localeClass: me,
        icon: 'b-fw-icon b-icon-hide-column',
        weight: 210,
        disabled: !column.allowDrag || me.disabled,
        onItem: () => column.hide()
      };
    }
  }
  /**
   * Handler for column hide/show menu checkitems.
   * @private
   * @param {Object} event The {@link Core.widget.MenuItem#event-toggle} event.
   */
  onColumnToggle({
    menu,
    item,
    checked
  }) {
    if (Boolean(item.column.hidden) !== !checked) {
      var _features$headerMenu, _item$menu;
      item.column[checked ? 'show' : 'hide']();
      const {
          grid,
          column
        } = item,
        {
          columns,
          features
        } = grid,
        // Sibling items, needed to disable other item if it is the last one in region
        siblingItems = menu.items,
        // Columns left visible in same region as this items column
        visibleInRegion = columns.visibleColumns.filter(col => col.region === item.column.region),
        // Needed to access "hide-column" item outside of column picker
        hideItem = ((_features$headerMenu = features.headerMenu) === null || _features$headerMenu === void 0 ? void 0 : _features$headerMenu.enabled) && features.headerMenu.menu.widgetMap.hideColumn;
      // Do not allow user to hide the last column in any region
      if (visibleInRegion.length === 1) {
        const lastVisibleItem = siblingItems.find(menuItem => menuItem.column === visibleInRegion[0]);
        if (lastVisibleItem) {
          lastVisibleItem.disabled = true;
        }
        // Also disable "Hide column" item if only one column left in this region
        if (hideItem && column.region === item.column.region) {
          hideItem.disabled = true;
        }
      }
      // Multiple columns visible, enable "hide-column" and all items for that region
      else {
        visibleInRegion.forEach(col => {
          const siblingItem = siblingItems.find(sibling => sibling.column === col);
          if (siblingItem) {
            siblingItem.disabled = false;
          }
        });
        if (hideItem && column.region === item.column.region) {
          hideItem.disabled = false;
        }
      }
      // Reflect status in submenu.
      (_item$menu = item.menu) === null || _item$menu === void 0 ? void 0 : _item$menu.eachWidget(subItem => {
        subItem.checked = checked;
      });
      const parentItem = menu.owner;
      if (parentItem && parentItem.column === column.parent) {
        parentItem.checked = siblingItems.some(subItem => subItem.checked === true);
      }
    }
  }
  /**
   * Supply items to ColumnDragToolbar
   * @private
   */
  getColumnDragToolbarItems(column, items) {
    const visibleInRegion = this.grid.columns.visibleColumns.filter(col => col.region === column.region);
    if (column.hideable !== false && visibleInRegion.length > 1) {
      items.push({
        text: 'L{hideColumnShort}',
        ref: 'hideColumn',
        group: 'L{column}',
        localeClass: this,
        icon: 'b-fw-icon b-icon-hide-column',
        weight: 101,
        onDrop: ({
          column
        }) => column.hide()
      });
    }
    return items;
  }
  //endregion
}

ColumnPicker._$name = 'ColumnPicker';
GridFeatureManager.registerFeature(ColumnPicker, true);

/**
 * @module Grid/feature/ColumnReorder
 */
/**
 * Allows user to reorder columns by dragging headers. To get notified about column reorder listen to `change` event
 * on {@link Grid.data.ColumnStore columns} store.
 *
 * {@inlineexample Grid/feature/ColumnReorder.js}
 *
 * This feature is <strong>enabled</strong> by default.
 *
 * @extends Core/mixin/InstancePlugin
 * @demo Grid/columns
 * @classtype columnReorder
 * @feature
 */
class ColumnReorder extends Delayable(InstancePlugin) {
  //region Init
  static $name = 'ColumnReorder';
  ignoreSelectors = ['.b-grid-header-resize-handle', '.b-field'];
  doDestroy() {
    var _this$dragHelper, _this$dragHelper2;
    (_this$dragHelper = this.dragHelper) === null || _this$dragHelper === void 0 ? void 0 : _this$dragHelper.scrollManager.destroy();
    (_this$dragHelper2 = this.dragHelper) === null || _this$dragHelper2 === void 0 ? void 0 : _this$dragHelper2.destroy();
    super.doDestroy();
  }
  get grid() {
    return this.client;
  }
  /**
   * Initialize drag & drop (called from render)
   * @private
   */
  init() {
    const me = this,
      {
        grid
      } = me,
      gridEl = grid.element,
      containers = DomHelper.children(gridEl, '.b-grid-headers');
    containers.push(...DomHelper.children(gridEl, '.b-grid-header-children'));
    if (me.dragHelper) {
      // update the dragHelper with the new set of containers it should operate upon
      me.dragHelper.containers = containers;
    } else {
      me.dragHelper = new DragHelper({
        name: 'columnReorder',
        mode: 'container',
        dragThreshold: 10,
        targetSelector: '.b-grid-header',
        floatRootOwner: grid,
        rtlSource: grid,
        outerElement: grid.headerContainer,
        monitoringConfig: {
          scrollables: [{
            element: '.b-grid-headers'
          }]
        },
        scrollManager: ScrollManager.new({
          direction: 'horizontal',
          element: grid.headerContainer
        }),
        containers,
        isElementDraggable(element) {
          const abort = Boolean(element.closest(me.ignoreSelectors.join(',')));
          if (abort || me.disabled) {
            return false;
          }
          const columnEl = element.closest(this.targetSelector),
            column = columnEl && grid.columns.getById(columnEl.dataset.columnId),
            isLast = (column === null || column === void 0 ? void 0 : column.childLevel) === 0 && grid.subGrids[column.region].columns.count === 1;
          return Boolean(column) && column.draggable !== false && !isLast;
        },
        ignoreSelector: '.b-filter-icon,.b-grid-header-resize-handle',
        internalListeners: {
          beforeDragStart: me.onBeforeDragStart,
          dragstart: me.onDragStart,
          drag: me.onDrag,
          drop: me.onDrop,
          thisObj: me
        }
      });
      me.relayEvents(me.dragHelper, ['dragStart', 'drag', 'drop', 'abort'], 'gridHeader');
    }
  }
  //endregion
  //region Plugin config
  // Plugin configuration. This plugin chains some of the functions in Grid
  static get pluginConfig() {
    return {
      after: ['onPaint', 'renderContents']
    };
  }
  //endregion
  //region Events (drop)
  onBeforeDragStart({
    context,
    event
  }) {
    const {
        element
      } = context,
      column = context.column = this.client.columns.getById(element.dataset.columnId);
    /**
     * This event is fired prior to starting a column drag gesture. The drag is canceled if a listener returns `false`.
     * @on-owner
     * @event beforeColumnDragStart
     * @param {Grid.view.Grid} source The grid instance.
     * @param {Grid.column.Column} column The dragged column.
     * @param {Event} event The browser event.
     * @preventable
     */
    return column.allowDrag && this.client.trigger('beforeColumnDragStart', {
      column,
      event
    });
  }
  onDragStart({
    context,
    event
  }) {
    const me = this,
      {
        grid
      } = me,
      {
        column
      } = context;
    if (!grid.features.columnDragToolbar) {
      const headerContainerBox = grid.element.querySelector('.b-grid-header-container').getBoundingClientRect();
      me.dragHelper.minY = headerContainerBox.top;
      me.dragHelper.maxY = headerContainerBox.bottom;
    }
    grid.headerContainer.classList.add('b-dragging-header');
    context.dragProxy.style.fontSize = DomHelper.getStyleValue(context.element, 'fontSize');
    /**
     * This event is fired when a column drag gesture has started.
     * @on-owner
     * @event columnDragStart
     * @param {Grid.view.Grid} source The grid instance.
     * @param {Grid.column.Column} column The dragged column.
     * @param {Event} event The browser event.
     */
    grid.trigger('columnDragStart', {
      column,
      event
    });
  }
  onDrag({
    context,
    event
  }) {
    const me = this,
      grid = me.client,
      {
        insertBefore,
        column
      } = context,
      targetHeader = Widget.fromElement(event.target, 'gridheader');
    /**
     * This event is fired when a column is being dragged, and you can set the `valid` flag on t
     * @event columnDrag
     * @on-owner
     * @param {Grid.view.Grid} source The grid instance.
     * @param {Grid.column.Column} column The dragged column.
     * @param {Grid.column.Column} insertBefore The column before which the dragged column will be inserted.
     * @param {Event} event The browser event.
     * @param {Object} context
     * @param {Boolean} context.valid Set this to true or false to indicate whether the drop position is valid.
     */
    grid.trigger('columnDrag', {
      column,
      insertBefore,
      event,
      context
    });
    // If SubGrid is configured with a sealed column set, do not allow moving into it
    if (targetHeader !== null && targetHeader !== void 0 && targetHeader.subGrid.sealedColumns) {
      context.valid = false;
      return;
    }
    // Require that we drag inside grid header while dragging if we don't have a drag toolbar
    if (context.valid && !me.grid.features.columnDragToolbar) {
      context.valid = Boolean(event.target.closest('.b-grid-headers'));
    }
  }
  /**
   * Handle drop
   * @private
   */
  onDrop({
    context,
    event
  }) {
    if (!context.valid) {
      return this.onInvalidDrop({
        context
      });
    }
    const me = this,
      {
        grid
      } = me,
      element = context.dragging,
      onHeader = context.target.closest('.b-grid-header'),
      droppedInRegion = context.draggedTo.dataset.region,
      // If dropping on right edge of grid-headers element, append to that subgrid
      onColumn = onHeader ? grid.columns.get(onHeader.dataset.column) : grid.subGrids[droppedInRegion].columns.last,
      toRegion = droppedInRegion || onColumn.region,
      sibling = context.insertBefore,
      column = grid.columns.getById(element.dataset.columnId),
      oldParent = column.parent,
      insertBefore = sibling ? grid.columns.getById(sibling.dataset.columnId) : grid.subGrids[toRegion].columns.last.nextSibling;
    let newParent;
    if (insertBefore) {
      newParent = insertBefore.parent;
    } else {
      const groupNode = onHeader === null || onHeader === void 0 ? void 0 : onHeader.parentElement.closest('.b-grid-header');
      if (groupNode) {
        newParent = grid.columns.getById(groupNode.dataset.columnId);
      } else {
        newParent = grid.columns.rootNode;
      }
    }
    grid.headerContainer.classList.remove('b-dragging-header');
    // Clean up element used during drag drop as it will not be removed by Grid when it refreshes its header elements
    element.remove();
    // If dropped into its current position in the same SubGrid - abort
    let vetoed = toRegion === column.region && oldParent === newParent && (onColumn === column.previousSibling || insertBefore === column.nextSibling);
    /**
     * This event is fired when a column is dropped, and you can return false from a listener to abort the operation.
     * @event beforeColumnDropFinalize
     * @on-owner
     * @param {Grid.view.Grid} source The grid instance.
     * @param {Grid.column.Column} column The dragged column.
     * @param {Grid.column.Column} insertBefore The column before which the dragged column will be inserted.
     * @param {Grid.column.Column} newParent The new parent column.
     * @param {Event} event The browser event.
     * @preventable
     */
    vetoed = vetoed || grid.trigger('beforeColumnDropFinalize', {
      column,
      newParent,
      insertBefore,
      event
    }) === false;
    if (!vetoed) {
      // Insert the column into its new place, which might be vetoed if column is sealed
      vetoed = !newParent.insertChild(column, insertBefore);
    }
    context.valid = !vetoed;
    if (!vetoed) {
      column.region = toRegion;
      // Check if we should remove last child
      if (oldParent.children.length === 0) {
        oldParent.parent.removeChild(oldParent);
      }
    }
    /**
     * This event is always fired after a column is dropped. The `valid` param is true if the operation was not
     * vetoed and the column was moved in the column store.
     * @event columnDrop
     * @on-owner
     * @param {Grid.view.Grid} source The grid instance.
     * @param {Grid.column.Column} column The dragged column.
     * @param {Grid.column.Column} insertBefore The column before which the dragged column will be inserted.
     * @param {Grid.column.Column} newParent The new parent column.
     * @param {Boolean} valid true if the operation was not vetoed.
     * @param {Event} event The browser event.
     * @preventable
     */
    grid.trigger('columnDrop', {
      column,
      newParent,
      insertBefore,
      valid: context.valid,
      event
    });
  }
  /**
   * Handle invalid drop
   * @private
   */
  onInvalidDrop() {
    this.grid.headerContainer.classList.remove('b-dragging-header');
  }
  //endregion
  //region Render
  /**
   * Updates DragHelper with updated headers when grid contents is rerendered
   * @private
   */
  renderContents() {
    // columns shown, hidden or reordered
    this.init();
  }
  /**
   * Initializes this feature on grid paint.
   * @private
   */
  onPaint() {
    // always reinit on paint
    this.init();
  }
  /**
   * Returns true if a reorder operation is active
   * @property {Boolean}
   * @readonly
   */
  get isReordering() {
    var _this$dragHelper3;
    return Boolean((_this$dragHelper3 = this.dragHelper) === null || _this$dragHelper3 === void 0 ? void 0 : _this$dragHelper3.isDragging);
  }
  //endregion
}

ColumnReorder.featureClass = 'b-column-reorder';
ColumnReorder._$name = 'ColumnReorder';
GridFeatureManager.registerFeature(ColumnReorder, true);

/**
 * @module Grid/feature/ColumnResize
 */
/**
 * Enables user to resize columns by dragging a handle on the right hand side of the header. To get notified about column
 * resize listen to `change` event on {@link Grid.data.ColumnStore columns} store.
 *
 * This feature is <strong>enabled</strong> by default.
 *
 * @extends Core/mixin/InstancePlugin
 *
 * @demo Grid/columns
 * @classtype columnResize
 * @inlineexample Grid/feature/ColumnResize.js
 * @feature
 */
class ColumnResize extends InstancePlugin {
  static get $name() {
    return 'ColumnResize';
  }
  static get configurable() {
    return {
      /**
       * Resize all cells below a resizing header during dragging.
       * `'auto'` means `true` on non-mobile platforms.
       * @config {String|Boolean}
       * @default
       */
      liveResize: 'auto'
    };
  }
  //region Init
  construct(grid, config) {
    const me = this;
    me.grid = grid;
    super.construct(grid, config);
    me.resizer = new ResizeHelper({
      name: 'columnResize',
      targetSelector: '.b-grid-header',
      handleSelector: '.b-grid-header-resize-handle',
      outerElement: grid.element,
      rtlSource: grid,
      internalListeners: {
        beforeresizestart: me.onBeforeResizeStart,
        resizestart: me.onResizeStart,
        resizing: me.onResizing,
        resize: me.onResize,
        cancel: me.onCancel,
        thisObj: me
      }
    });
  }
  doDestroy() {
    var _this$resizer;
    (_this$resizer = this.resizer) === null || _this$resizer === void 0 ? void 0 : _this$resizer.destroy();
    super.doDestroy();
  }
  //endregion
  changeLiveResize(liveResize) {
    if (liveResize === 'auto') {
      return !BrowserHelper.isMobileSafari;
    }
    return liveResize;
  }
  //region Events
  onBeforeResizeStart() {
    return !this.disabled;
  }
  onResizeStart({
    context
  }) {
    const {
        grid,
        resizer
      } = this,
      column = context.column = grid.columns.getById(context.element.dataset.columnId);
    resizer.minWidth = column.minWidth;
    grid.element.classList.add('b-column-resizing');
  }
  /**
   * Handle drag event - resize the column live unless it's a touch gesture
   * @private
   */
  onResizing({
    context
  }) {
    if (context.valid && this.liveResize) {
      this.grid.resizingColumns = true;
      context.column.width = context.newWidth;
    }
  }
  /**
   * Handle drop event (only used for touch)
   * @private
   */
  onResize({
    context
  }) {
    const {
        grid
      } = this,
      {
        column
      } = context;
    grid.element.classList.remove('b-column-resizing');
    if (context.valid) {
      if (this.liveResize) {
        grid.resizingColumns = false;
        grid.afterColumnsResized(column);
      } else {
        column.width = context.newWidth;
      }
    }
  }
  /**
   * Restore column width on cancel (ESC)
   * @private
   */
  onCancel({
    context
  }) {
    const {
      grid
    } = this;
    grid.element.classList.remove('b-column-resizing');
    context.column.width = context.elementWidth;
    grid.resizingColumns = false;
  }
  //endregion
}

ColumnResize._$name = 'ColumnResize';
GridFeatureManager.registerFeature(ColumnResize, true);

/**
 * @module Grid/widget/GridFieldFilterPicker
 */
/**
 * Subclass of {@link Core.widget.FieldFilterPicker} allowing configuration using an
 * existing {@link Grid.view.Grid}.
 *
 * See also {@link Grid.widget.GridFieldFilterPickerGroup}.
 *
 * @extends Core/widget/FieldFilterPicker
 * @classtype gridfieldfilterpicker
 * @demo Grid/fieldfilters
 * @widget
 */
class GridFieldFilterPicker extends FieldFilterPicker {
  //region Config
  static get $name() {
    return 'GridFieldFilterPicker';
  }
  // Factoryable type name
  static get type() {
    return 'gridfieldfilterpicker';
  }
  /** @hideconfigs store */
  static configurable = {
    /**
     * {@link Grid.view.Grid} from which to read the available field list. In order to
     * appear as a selectable property for a filter, a column must have a `field` property.
     * If the column has a `text` property, that will be shown as the displayed text in the
     * selector; otherwise, the `field` property will be shown as-is.
     *
     * The grid's {@link Core.data.Store}'s {@link Core.data.Store#property-modelClass} will be
     * examined to find field data types.
     *
     * You can limit available fields to a subset of the grid's columns using the
     * {@link #config-allowedFieldNames} configuration property.
     *
     * @config {Grid.view.Grid}
     */
    grid: null,
    /**
     * Optional array of field names that are allowed as selectable properties for filters.
     * This is a subset of the field names found in the {@link #config-grid}'s columns. When supplied, only
     * the named fields will be shown in the property selector combo.
     *
     * Note that field names are case-sensitive and should match the data field name in the store
     * model.
     *
     * @config {String[]}
     */
    allowedFieldNames: null
  };
  //endregion
  afterConstruct() {
    const me = this;
    if (!me.grid) {
      throw new Error(`${me.constructor.$name} requires 'grid' to be configured.`);
    }
    me.fields = me.fields ?? {}; // Force `fields` changer if fields is left null, to merge w/ grid fields
    super.afterConstruct();
  }
  updateGrid(newGrid) {
    var _newGrid$store;
    if (!((_newGrid$store = newGrid.store) !== null && _newGrid$store !== void 0 && _newGrid$store.modelClass)) {
      throw new Error(`Grid does not have a store with a modelClass defined.`);
    }
    if (!newGrid.columns) {
      throw new Error(`Grid does not have a column store.`);
    }
  }
  /**
   * Returns a subset of the fields defined on the model class, excluding those considered internal or otherwise not
   * suitable for user-facing filtering.
   * @param {Core.data.Model} modelClass The Model subclass whose fields will be read
   * @returns {Core.data.field.DataField[]}
   * @private
   */
  static getModelClassFields(modelClass) {
    const ownFieldNames = new Set(modelClass.fields.map(({
      name
    }) => name));
    return (modelClass === null || modelClass === void 0 ? void 0 : modelClass.allFields.filter(field => !field.internal && (SUPPORTED_FIELD_DATA_TYPES.includes(field.type) || isSupportedDurationField(field)) && (field.definedBy !== Model || ownFieldNames.has(field.name)))) || [];
  }
  /**
   * Gets the filterable fields backing any of the configured `grid`'s columns, for those columns for which
   * it is possible to do so.
   * @private
   * @returns {Object} Filterable fields dictionary of the form { [fieldName]: { title, type } }
   */
  static getColumnFields(columnStore, modelClass, allowedFieldNames) {
    const modelFields = ArrayHelper.keyBy(GridFieldFilterPicker.getModelClassFields(modelClass), 'name'),
      allowedNameSet = allowedFieldNames && new Set(allowedFieldNames);
    return Object.fromEntries((columnStore === null || columnStore === void 0 ? void 0 : columnStore.records.filter(({
      field
    }) => field && modelFields[field] && (!allowedNameSet || allowedNameSet.has(field))).map(({
      field,
      text
    }) => [field, {
      title: text || field,
      type: isSupportedDurationField(modelFields[field]) ? 'duration' : modelFields[field].type
    }])) ?? []);
  }
  changeFields(newFields) {
    var _this$grid$store;
    let localFields = newFields;
    if (Array.isArray(newFields)) {
      VersionHelper.deprecate('Core', '6.0.0', 'FieldOption[] deprecated, use Object<String, FieldOption[]> keyed by field name instead');
      // Support old array syntax for `fields` during deprecation
      localFields = ArrayHelper.keyBy(localFields, 'name');
    }
    return ObjectHelper.merge({}, GridFieldFilterPicker.getColumnFields(this.grid.columns, (_this$grid$store = this.grid.store) === null || _this$grid$store === void 0 ? void 0 : _this$grid$store.modelClass, this.allowedFieldNames), localFields);
  }
}
GridFieldFilterPicker.initClass();
GridFieldFilterPicker._$name = 'GridFieldFilterPicker';

/**
 * @module Grid/widget/GridFieldFilterPickerGroup
 */
/**
 * Extends {@link Core.widget.FieldFilterPickerGroup} to allow providing a {@link Grid.view.Grid} from which
 * available fields will be read. This is useful when a grid is already configured with a set of columns
 * containing display names and type information.
 *
 * The grid should have a {@link Grid.data.ColumnStore} configured (see {@link Grid.view.Grid#config-columns})
 * and a {@link Core.data.Store} whose {@link Core.data.Store#property-modelClass} contains fields with
 * specific data types.
 *
 * Optionally, you can also use {@link #config-allowedFieldNames} to restrict the set of fields shown in the
 * widget.
 *
 * For example:
 *
 * ```javascript
 * new GridFieldFilterPickerGroup({
 *     appendTo : domElement,
 *
 *     grid : myGrid,
 *
 *     filters : [{
 *         property : 'startDate',
 *         operator : '<=',
 *         value    : new Date()
 *     }]
 * });
 * ```
 *
 * @classtype gridfieldfilterpickergroup
 * @extends Core/widget/FieldFilterPickerGroup
 * @demo Grid/fieldfilters
 * @widget
 */
class GridFieldFilterPickerGroup extends FieldFilterPickerGroup {
  //region Config
  static get $name() {
    return 'GridFieldFilterPickerGroup';
  }
  // Factoryable type name
  static get type() {
    return 'gridfieldfilterpickergroup';
  }
  /** @hideconfigs fields, store */
  static configurable = {
    /**
     * {@link Grid.view.Grid} from which to read the available field list. In order to
     * appear as a selectable property for a filter, a column must have a `field` property.
     * If the column has a `text` property, that will be shown as the displayed text in the
     * selector; otherwise, the `field` property will be shown as-is.
     *
     * The grid's {@link Core.data.Store}'s {@link Core.data.Store#property-modelClass} will be
     * examined to find field data types.
     *
     * You can limit available fields to a subset of the grid's columns using the
     * {@link #config-allowedFieldNames} configuration property.
     *
     * @config {Grid.view.Grid}
     */
    grid: null,
    /**
     * Optional array of field names that are allowed as selectable properties for filters.
     * This should be a subset of the field names found in the {@link #config-grid}'s store. When supplied,
     * only the named fields will be shown in the property selector combo.
     *
     * @config {String[]}
     */
    allowedFieldNames: null
  };
  //endregion
  static childPickerType = 'gridfieldfilterpicker';
  validateConfig() {
    if (!this.grid) {
      throw new Error(`${this.constructor.$name} requires the 'grid' config property.`);
    }
  }
  getFilterPickerConfig(filter) {
    const {
      grid,
      allowedFieldNames
    } = this;
    return {
      ...super.getFilterPickerConfig(filter),
      grid,
      allowedFieldNames
    };
  }
  updateGrid(newGrid) {
    this.store = this.grid.store;
  }
  /**
   * @private
   */
  canManage(filter) {
    const me = this;
    return super.canManage(filter) && (!me.allowedFieldNames || me.allowedFieldNames.includes(filter.property));
  }
}
GridFieldFilterPickerGroup.initClass();
GridFieldFilterPickerGroup._$name = 'GridFieldFilterPickerGroup';

/**
 * @module Grid/feature/Filter
 */
const fieldTypeMap = {
  date: 'date',
  int: 'number',
  integer: 'number',
  number: 'number',
  string: 'text',
  duration: 'duration'
};
/**
 * Feature that allows filtering of the grid by settings filters on columns. The actual filtering is done by the store.
 * For info on programmatically handling filters, see {@link Core.data.mixin.StoreFilter}.
 *
 * {@inlineexample Grid/feature/Filter.js}
 *
 * ```javascript
 * // Filtering turned on but no default filter
 * const grid = new Grid({
 *   features : {
 *     filter : true
 *   }
 * });
 *
 * // Using default filter
 * const grid = new Grid({
 *   features : {
 *     filter : { property : 'city', value : 'Gavle' }
 *   }
 * });
 * ```
 *
 * A column can supply a custom filtering function as its {@link Grid.column.Column#config-filterable} config. When
 * filtering by that column using the UI that function will be used to determine which records to include. See
 * {@link Grid.column.Column#config-filterable Column#filterable} for more information.
 *
 * ```javascript
 * // Custom filtering function for a column
 * const grid = new Grid({
 *    features : {
 *        filter : true
 *    },
 *
 *    columns: [
 *        {
 *          field      : 'age',
 *          text       : 'Age',
 *          type       : 'number',
 *          // Custom filtering function that checks "greater than" no matter
 *          // which field user filled in :)
 *          filterable : ({ record, value, operator }) => record.age > value
 *        }
 *    ]
 * });
 * ```
 *
 * If this feature is configured with `prioritizeColumns : true`, those functions will also be used when filtering
 * programmatically:
 *
 * ```javascript
 * const grid = new Grid({
 *    features : {
 *        filter : {
 *            prioritizeColumns : true
 *        }
 *    },
 *
 *    columns: [
 *        {
 *          field      : 'age',
 *          text       : 'Age',
 *          type       : 'number',
 *          filterable : ({ record, value, operator }) => record.age > value
 *        }
 *    ]
 * });
 *
 * // Because of the prioritizeColumns config above, any custom filterable function
 * // on a column will be used when programmatically filtering by that columns field
 * grid.store.filter({
 *     property : 'age',
 *     value    : 41
 * });
 * ```
 *
 * You can supply a field config to use for the filtering field displayed for string type columns:
 *
 * ```javascript
 * // For string-type columns you can also replace the filter UI with a custom field:
 * columns: [
 *     {
 *         field : 'city',
 *         // Filtering for a value out of a list of values
 *         filterable: {
 *             filterField : {
 *                 type  : 'combo',
 *                 items : [
 *                     'Paris',
 *                     'Dubai',
 *                     'Moscow',
 *                     'London',
 *                     'New York'
 *                 ]
 *             }
 *         }
 *     }
 * ]
 * ```
 *
 * You can also change default fields, for example this will use {@link Core.widget.DateTimeField} in filter popup:
 * ```javascript
 * columns : [
 *     {
 *         type       : 'date',
 *         field      : 'start',
 *         filterable : {
 *             filterField : {
 *                 type : 'datetime'
 *             }
 *         }
 *     }
 * ]
 * ```
 *
 * This feature is <strong>disabled</strong> by default.
 *
 * **Note:** This feature cannot be used together with {@link Grid.feature.FilterBar} feature, they are
 * mutually exclusive.
 *
 * ## Keyboard shortcuts
 * This feature has the following default keyboard shortcuts:
 *
 * | Keys   | Action                  | Action description                                                     |
 * |--------|-------------------------|------------------------------------------------------------------------|
 * | `F`    | *showFilterEditorByKey* | When the column header is focused, this shows the filter input field   |
 *
 * <div class="note">Please note that <code>Ctrl</code> is the equivalent to <code>Command</code> and <code>Alt</code>
 * is the equivalent to <code>Option</code> for Mac users</div>
 *
 * For more information on how to customize keyboard shortcuts, please see
 * [our guide](#Grid/guides/customization/keymap.md).
 *
 * To enable an alternative UI that uses {@link Core.widget.FieldFilterPickerGroup} to allow
 * specifying multiple filters on the column at once, set `isMulti` to `true`.
 *
 * @extends Core/mixin/InstancePlugin
 * @demo Grid/filtering
 * @classtype filter
 * @feature
 */
class Filter extends InstancePlugin {
  //region Init
  static get $name() {
    return 'Filter';
  }
  static get configurable() {
    return {
      /**
       * Use custom filtering functions defined on columns also when programmatically filtering by the columns
       * field.
       *
       * ```javascript
       * const grid = new Grid({
       *     columns : [
       *         {
       *             field : 'age',
       *             text : 'Age',
       *             filterable({ record, value }) {
       *               // Custom filtering, return true/false
       *             }
       *         }
       *     ],
       *
       *     features : {
       *         filter : {
       *             prioritizeColumns : true // <--
       *         }
       *     }
       * });
       *
       * // Because of the prioritizeColumns config above, any custom
       * // filterable function on a column will be used when
       * // programmatically filtering by that columns field
       * grid.store.filter({
       *     property : 'age',
       *     value    : 30
       * });
       * ```
       *
       * @config {Boolean}
       * @default
       * @category Common
       */
      prioritizeColumns: false,
      /**
       * See {@link #keyboard-shortcuts Keyboard shortcuts} for details
       * @config {Object<String,String>}
       */
      keyMap: {
        f: 'showFilterEditorByKey'
      },
      /**
       * Use {@link Grid.widget.GridFieldFilterPickerGroup} instead of the normal UI,
       * enabling multiple filters for the same column. To enable the multi-filter UI,
       * set `isMulti` to either `true` or a {@link Grid.widget.GridFieldFilterPickerGroup}
       * configuration object.
       *
       * @config {Boolean|GridFieldFilterPickerGroupConfig}
       * @default
       * @category Common
       */
      isMulti: false
    };
  }
  construct(grid, config) {
    if (grid.features.filterBar) {
      throw new Error('Grid.feature.Filter feature may not be used together with Grid.feature.FilterBar. These features are mutually exclusive.');
    }
    const me = this;
    me.grid = grid;
    me.closeFilterEditor = me.closeFilterEditor.bind(me);
    super.construct(grid, config);
    me.bindStore(grid.store);
    if (config && typeof config === 'object') {
      const clone = ObjectHelper.clone(config);
      // Feature accepts a filter config object, need to remove this config
      delete clone.prioritizeColumns;
      delete clone.isMulti;
      delete clone.dateFormat;
      if (!ObjectHelper.isEmpty(clone)) {
        grid.store.filter(clone, null, grid.isConfiguring);
      }
    }
  }
  doDestroy() {
    var _this$filterTip, _this$filterEditorPop;
    (_this$filterTip = this.filterTip) === null || _this$filterTip === void 0 ? void 0 : _this$filterTip.destroy();
    (_this$filterEditorPop = this.filterEditorPopup) === null || _this$filterEditorPop === void 0 ? void 0 : _this$filterEditorPop.destroy();
    super.doDestroy();
  }
  get store() {
    return this.grid.store;
  }
  bindStore(store) {
    this.detachListeners('store');
    store.ion({
      name: 'store',
      beforeFilter: 'onStoreBeforeFilter',
      filter: 'onStoreFilter',
      thisObj: this
    });
  }
  //endregion
  //region Plugin config
  // Plugin configuration. This plugin chains some of the functions in Grid.
  static get pluginConfig() {
    return {
      chain: ['renderHeader', 'populateCellMenu', 'populateHeaderMenu', 'onElementClick', 'bindStore']
    };
  }
  //endregion
  //region Refresh headers
  /**
   * Update headers to match stores filters. Called on store load and grid header render.
   * @param reRenderRows Also refresh rows?
   * @private
   */
  refreshHeaders(reRenderRows) {
    const me = this,
      grid = me.grid,
      element = grid.headerContainer;
    if (element) {
      // remove .latest from all filters, will be applied to actual latest
      DomHelper.children(element, '.b-filter-icon.b-latest').forEach(iconElement => iconElement.classList.remove('b-latest'));
      if (!me.filterTip) {
        me.filterTip = new Tooltip({
          forElement: element,
          forSelector: '.b-filter-icon',
          getHtml({
            activeTarget
          }) {
            return activeTarget.dataset.filterText;
          }
        });
      }
      if (!grid.store.isFiltered) {
        me.filterTip.hide();
      }
      grid.columns.visibleColumns.forEach(column => {
        if (column.filterable !== false) {
          const columnFilters = me.store.filters.allValues.filter(({
              property,
              disabled,
              internal
            }) => property === column.field && !disabled && !internal),
            isColumnFiltered = columnFilters.length > 0,
            headerEl = column.element;
          if (headerEl) {
            const textEl = column.textWrapper;
            let filterIconEl = textEl === null || textEl === void 0 ? void 0 : textEl.querySelector('.b-filter-icon'),
              filterText;
            if (isColumnFiltered) {
              const bullet = '&#x2022 ';
              filterText = `${me.L('L{filter}')}: ` + (columnFilters.length > 1 ? '<br/><br/>' : '') + columnFilters.map(columnFilter => {
                var _me$store, _me$store$modelRelati;
                let value = columnFilter.value ?? '';
                const isArray = Array.isArray(value),
                  relation = (_me$store = me.store) === null || _me$store === void 0 ? void 0 : (_me$store$modelRelati = _me$store.modelRelations) === null || _me$store$modelRelati === void 0 ? void 0 : _me$store$modelRelati.find(({
                    foreignKey
                  }) => foreignKey === columnFilter.property);
                if (columnFilter.displayValue) {
                  value = columnFilter.displayValue;
                } else {
                  if (me.isMulti && relation) {
                    var _me$isMulti$fields;
                    // Look up remote display value per filterable-field config (FieldFilterPicker.js#FieldOption)
                    const {
                      relatedDisplayField
                    } = (_me$isMulti$fields = me.isMulti.fields) === null || _me$isMulti$fields === void 0 ? void 0 : _me$isMulti$fields[columnFilter.property];
                    if (relatedDisplayField) {
                      const getDisplayValue = foreignId => {
                        var _relation$foreignStor;
                        return (_relation$foreignStor = relation.foreignStore.getById(foreignId)) === null || _relation$foreignStor === void 0 ? void 0 : _relation$foreignStor[relatedDisplayField];
                      };
                      if (isArray) {
                        value = value.map(getDisplayValue).sort((a, b) => (a ?? '').localeCompare(b ?? ''));
                      } else {
                        value = getDisplayValue(value);
                      }
                    }
                  } else if (column.formatValue && value) {
                    value = isArray ? value.map(val => column.formatValue(val)) : column.formatValue(value);
                  }
                  if (isArray) {
                    value = `[ ${value.join(', ')} ]`;
                  }
                }
                return (columnFilters.length > 1 ? bullet : '') + (typeof columnFilter === 'string' ? columnFilter : `${columnFilter.operator} ${value}`);
              }).join('<br/><br/>');
            } else {
              filterText = me.L('L{applyFilter}');
            }
            if (!filterIconEl) {
              // putting icon in header text to have more options for positioning it
              filterIconEl = DomHelper.createElement({
                parent: textEl,
                tag: 'div',
                className: 'b-filter-icon',
                dataset: {
                  filterText
                }
              });
            } else {
              filterIconEl.dataset.filterText = filterText;
            }
            // latest applied filter distinguished with class to enable highlighting etc.
            if (column.field === me.store.latestFilterField) filterIconEl.classList.add('b-latest');
            headerEl.classList.add('b-filterable');
            headerEl.classList.toggle('b-filter', isColumnFiltered);
          }
          column.meta.isFiltered = isColumnFiltered;
        }
      });
      if (reRenderRows) {
        grid.refreshRows();
      }
    }
  }
  //endregion
  //region Filter
  applyFilter(column, config) {
    const {
        store
      } = this,
      {
        filterFn
      } = column.filterable;
    // Must add the filter silently, so that the column gets a reference to its $filter
    // before the filter happens and events are broadcast.
    column.$filter = store.addFilter({
      ...column.filterable,
      ...config,
      property: column.field,
      // Only inject a filterBy configuration if the column has a custom filterBy
      [filterFn ? 'filterBy' : '_']: function (record) {
        return filterFn({
          value: this.value,
          record,
          operator: this.operator,
          property: this.property,
          column
        });
      }
    }, true);
    // Apply the new set of store filters.
    store.filter();
  }
  removeFilter(column) {
    if (this.isMulti) {
      for (const filter of this.getCurrentMultiFilters(column)) {
        this.store.removeFilter(filter);
      }
    } else {
      this.store.removeFilter(column.field);
    }
  }
  disableFilter(column) {
    for (const filter of this.getCurrentMultiFilters(column)) {
      filter.disabled = true;
      this.store.filter(filter);
    }
    this.store.filter();
  }
  getCurrentMultiFilters(column) {
    return this.store.filters.values.filter(filter => filter.property === column.field);
  }
  getPopupDateItems(column, fieldType, filter, initialValue, store, changeCallback, closeCallback, filterField) {
    const me = this,
      onClose = changeCallback;
    function onClear() {
      me.removeFilter(column);
    }
    function onKeydown({
      event
    }) {
      if (event.key === 'Enter') {
        changeCallback();
      }
    }
    function onChange({
      source,
      value
    }) {
      if (value == null) {
        onClear();
      } else {
        me.clearSiblingsFields(source);
        me.applyFilter(column, {
          operator: source.operator,
          value,
          displayValue: source._value,
          type: 'date'
        });
      }
    }
    return [ObjectHelper.assign({
      type: 'date',
      ref: 'on',
      placeholder: 'L{on}',
      localeClass: me,
      clearable: true,
      label: '<i class="b-fw-icon b-icon-filter-equal"></i>',
      value: (filter === null || filter === void 0 ? void 0 : filter.operator) === 'sameDay' ? filter.value : initialValue,
      operator: 'sameDay',
      onKeydown,
      onChange,
      onClose,
      onClear
    }, filterField), ObjectHelper.assign({
      type: 'date',
      ref: 'before',
      placeholder: 'L{before}',
      localeClass: me,
      clearable: true,
      label: '<i class="b-fw-icon b-icon-filter-before"></i>',
      value: (filter === null || filter === void 0 ? void 0 : filter.operator) === '<' ? filter.value : null,
      operator: '<',
      onKeydown,
      onChange,
      onClose,
      onClear
    }, filterField), ObjectHelper.assign({
      type: 'date',
      ref: 'after',
      cls: 'b-last-row',
      placeholder: 'L{after}',
      localeClass: me,
      clearable: true,
      label: '<i class="b-fw-icon b-icon-filter-after"></i>',
      value: (filter === null || filter === void 0 ? void 0 : filter.operator) === '>' ? filter.value : null,
      operator: '>',
      onKeydown,
      onChange,
      onClose,
      onClear
    }, filterField)];
  }
  getPopupNumberItems(column, fieldType, filter, initialValue, store, changeCallback, closeCallback, filterField) {
    const me = this,
      onEsc = changeCallback;
    function onClear() {
      me.removeFilter(column);
    }
    function onKeydown({
      event
    }) {
      if (event.key === 'Enter') {
        changeCallback();
      }
    }
    function onChange({
      source,
      value
    }) {
      if (value == null) {
        onClear();
      } else {
        me.clearSiblingsFields(source);
        me.applyFilter(column, {
          operator: source.operator,
          value
        });
      }
    }
    return [ObjectHelper.assign({
      type: 'number',
      placeholder: 'L{Filter.equals}',
      localeClass: me,
      clearable: true,
      label: '<i class="b-fw-icon b-icon-filter-equal"></i>',
      value: (filter === null || filter === void 0 ? void 0 : filter.operator) === '=' ? filter.value : initialValue,
      operator: '=',
      onKeydown,
      onChange,
      onEsc,
      onClear
    }, filterField), ObjectHelper.assign({
      type: 'number',
      placeholder: 'L{lessThan}',
      localeClass: me,
      clearable: true,
      label: '<i class="b-fw-icon b-icon-filter-less"></i>',
      value: (filter === null || filter === void 0 ? void 0 : filter.operator) === '<' ? filter.value : null,
      operator: '<',
      onKeydown,
      onChange,
      onEsc,
      onClear
    }, filterField), ObjectHelper.assign({
      type: 'number',
      cls: 'b-last-row',
      placeholder: 'L{moreThan}',
      localeClass: me,
      clearable: true,
      label: '<i class="b-fw-icon b-icon-filter-more"></i>',
      value: (filter === null || filter === void 0 ? void 0 : filter.operator) === '>' ? filter.value : null,
      operator: '>',
      onKeydown,
      onChange,
      onEsc,
      onClear
    }, filterField)];
  }
  clearSiblingsFields(sourceField) {
    var _this$filterEditorPop2;
    (_this$filterEditorPop2 = this.filterEditorPopup) === null || _this$filterEditorPop2 === void 0 ? void 0 : _this$filterEditorPop2.items.forEach(field => {
      field !== sourceField && (field === null || field === void 0 ? void 0 : field.clear());
    });
  }
  getPopupDurationItems(column, fieldType, filter, initialValue, store, changeCallback, closeCallback, filterField) {
    const me = this,
      onEsc = changeCallback,
      onClear = () => me.removeFilter(column);
    function onChange({
      source,
      value
    }) {
      if (value == null) {
        onClear();
      } else {
        me.clearSiblingsFields(source);
        me.applyFilter(column, {
          operator: source.operator,
          value
        });
      }
    }
    return [ObjectHelper.assign({
      type: 'duration',
      placeholder: 'L{Filter.equals}',
      localeClass: me,
      clearable: true,
      label: '<i class="b-fw-icon b-icon-filter-equal"></i>',
      value: (filter === null || filter === void 0 ? void 0 : filter.operator) === '=' ? filter.value : initialValue,
      operator: '=',
      onChange,
      onEsc,
      onClear
    }, filterField), ObjectHelper.assign({
      type: 'duration',
      placeholder: 'L{lessThan}',
      localeClass: me,
      clearable: true,
      label: '<i class="b-fw-icon b-icon-filter-less"></i>',
      value: (filter === null || filter === void 0 ? void 0 : filter.operator) === '<' ? filter.value : null,
      operator: '<',
      onChange,
      onEsc,
      onClear
    }, filterField), ObjectHelper.assign({
      type: 'duration',
      cls: 'b-last-row',
      placeholder: 'L{moreThan}',
      localeClass: me,
      clearable: true,
      label: '<i class="b-fw-icon b-icon-filter-more"></i>',
      value: (filter === null || filter === void 0 ? void 0 : filter.operator) === '>' ? filter.value : null,
      operator: '>',
      onChange,
      onEsc,
      onClear
    }, filterField)];
  }
  getPopupStringItems(column, fieldType, filter, initialValue, store, changeCallback, closeCallback, filterField) {
    const me = this;
    return [ObjectHelper.assign({
      type: fieldType,
      cls: 'b-last-row',
      placeholder: 'L{filter}',
      localeClass: me,
      clearable: true,
      label: '<i class="b-fw-icon b-icon-filter-equal"></i>',
      value: filter ? filter.value || filter : initialValue,
      operator: '*',
      onChange({
        source,
        value
      }) {
        if (value === '') {
          closeCallback();
        } else {
          me.applyFilter(column, {
            operator: source.operator,
            value,
            displayValue: source.displayField && source.records ? source.records.map(rec => rec[source.displayField]).join(', ') : undefined
          });
          // Leave multiselect filter combo visible to be able to select many items at once
          if (!source.multiSelect) {
            changeCallback();
          }
        }
      },
      onClose: changeCallback,
      onClear: closeCallback
    }, filterField)];
  }
  /**
   * Get fields to display in filter popup.
   * @param {Grid.column.Column} column Column
   * @param fieldType Type of field, number, date etc.
   * @param filter Current filter filter
   * @param initialValue
   * @param store Grid store
   * @param changeCallback Callback for when filter has changed
   * @param closeCallback Callback for when editor should be closed
   * @param filterField filter field
   * @returns {*}
   * @private
   */
  getPopupItems(column, fieldType, filter, initialValue, store, changeCallback, closeCallback, filterField) {
    const me = this;
    if (me.isMulti) {
      return me.getMultiFilterPopupItems(...arguments);
    }
    switch (fieldType) {
      case 'date':
        return me.getPopupDateItems(...arguments);
      case 'number':
        return me.getPopupNumberItems(...arguments);
      case 'duration':
        return me.getPopupDurationItems(...arguments);
      default:
        return me.getPopupStringItems(...arguments);
    }
  }
  getMultiFilterPopupItems(column) {
    var _grid$store;
    const {
        grid,
        isMulti
      } = this,
      existingFilter = (_grid$store = grid.store) === null || _grid$store === void 0 ? void 0 : _grid$store.filters.find(filter => filter.property === column.field);
    return [{
      ...(typeof isMulti === 'object' ? isMulti : undefined),
      type: 'gridfieldfilterpickergroup',
      ref: 'pickerGroup',
      limitToProperty: column.field,
      grid,
      filters: existingFilter ? [] : [{
        property: column.field
      }],
      propertyFieldCls: 'b-transparent property-field',
      operatorFieldCls: 'b-transparent operator-field',
      valueFieldCls: 'b-transparent value-field',
      width: '400px'
    }];
  }
  /**
   * Shows a popup where a filter can be edited.
   * @param {Grid.column.Column|String} column Column to show filter editor for
   * @param {*} [value] The initial value of the filter field
   */
  showFilterEditor(column, value) {
    column = this.grid.columns.getById(column);
    const me = this,
      {
        store,
        isMulti
      } = me,
      headerEl = column.element,
      filter = store.filters.getBy('property', column.field),
      fieldType = me.getFilterType(column);
    if (column.filterable === false) {
      return;
    }
    // Destroy previous filter popup
    me.closeFilterEditor();
    const items = me.getPopupItems(column, fieldType,
    // Only pass filter if it's not an internal filter
    filter !== null && filter !== void 0 && filter.internal ? null : filter, value, store, me.closeFilterEditor, () => {
      me.removeFilter(column);
      me.closeFilterEditor();
    }, column.filterable.filterField, isMulti);
    // Localize placeholders
    items.forEach(item => item.placeholder = item.placeholder ? this.L(item.placeholder) : item.placeholder);
    me.filterEditorPopup = WidgetHelper.openPopup(headerEl, {
      owner: me.grid,
      cls: 'b-filter-popup',
      scrollAction: 'realign',
      layout: {
        type: 'vbox',
        align: 'stretch'
      },
      items
    });
  }
  /**
   * Close the filter editor.
   */
  closeFilterEditor() {
    var _this$filterEditorPop3;
    // Must defer the destroy because it may be closed by an event like a "change" event where
    // there may be plenty of code left to execute which must not execute on destroyed objects.
    (_this$filterEditorPop3 = this.filterEditorPopup) === null || _this$filterEditorPop3 === void 0 ? void 0 : _this$filterEditorPop3.setTimeout(this.filterEditorPopup.destroy);
    this.filterEditorPopup = null;
  }
  //endregion
  //region Context menu
  getFilterType(column) {
    const fieldName = column.field,
      field = this.client.store.modelClass.getFieldDefinition(fieldName),
      type = column.filterType;
    return type ? fieldTypeMap[type] : fieldTypeMap[column.type] || field && fieldTypeMap[field.type] || 'text';
  }
  populateCellMenuWithDateItems({
    column,
    record,
    items
  }) {
    const property = column.field,
      type = this.getFilterType(column);
    if (type === 'date') {
      const me = this,
        value = record[property],
        filter = operator => {
          me.applyFilter(column, {
            operator,
            value,
            displayValue: column.formatValue ? column.formatValue(value) : value,
            type: 'date'
          });
        };
      items.filterDateEquals = {
        text: 'L{on}',
        localeClass: me,
        icon: 'b-fw-icon b-icon-filter-equal',
        cls: 'b-separator',
        weight: 300,
        disabled: me.disabled,
        onItem: () => filter('=')
      };
      items.filterDateBefore = {
        text: 'L{before}',
        localeClass: me,
        icon: 'b-fw-icon b-icon-filter-before',
        weight: 310,
        disabled: me.disabled,
        onItem: () => filter('<')
      };
      items.filterDateAfter = {
        text: 'L{after}',
        localeClass: me,
        icon: 'b-fw-icon b-icon-filter-after',
        weight: 320,
        disabled: me.disabled,
        onItem: () => filter('>')
      };
    }
  }
  populateCellMenuWithNumberItems({
    column,
    record,
    items
  }) {
    const property = column.field,
      type = this.getFilterType(column);
    if (type === 'number') {
      const me = this,
        value = record[property],
        filter = operator => {
          me.applyFilter(column, {
            operator,
            value
          });
        };
      items.filterNumberEquals = {
        text: 'L{equals}',
        localeClass: me,
        icon: 'b-fw-icon b-icon-filter-equal',
        cls: 'b-separator',
        weight: 300,
        disabled: me.disabled,
        onItem: () => filter('=')
      };
      items.filterNumberLess = {
        text: 'L{lessThan}',
        localeClass: me,
        icon: 'b-fw-icon b-icon-filter-less',
        weight: 310,
        disabled: me.disabled,
        onItem: () => filter('<')
      };
      items.filterNumberMore = {
        text: 'L{moreThan}',
        localeClass: me,
        icon: 'b-fw-icon b-icon-filter-more',
        weight: 320,
        disabled: me.disabled,
        onItem: () => filter('>')
      };
    }
  }
  populateCellMenuWithDurationItems({
    column,
    record,
    items
  }) {
    const type = this.getFilterType(column);
    if (type === 'duration') {
      const me = this,
        value = column.getFilterableValue(record),
        filter = operator => {
          me.applyFilter(column, {
            operator,
            value
          });
        };
      items.filterDurationEquals = {
        text: 'L{equals}',
        localeClass: me,
        icon: 'b-fw-icon b-icon-filter-equal',
        cls: 'b-separator',
        weight: 300,
        disabled: me.disabled,
        onItem: () => filter('=')
      };
      items.filterDurationLess = {
        text: 'L{lessThan}',
        localeClass: me,
        icon: 'b-fw-icon b-icon-filter-less',
        weight: 310,
        disabled: me.disabled,
        onItem: () => filter('<')
      };
      items.filterDurationMore = {
        text: 'L{moreThan}',
        localeClass: me,
        icon: 'b-fw-icon b-icon-filter-more',
        weight: 320,
        disabled: me.disabled,
        onItem: () => filter('>')
      };
    }
  }
  populateCellMenuWithStringItems({
    column,
    record,
    items
  }) {
    const type = this.getFilterType(column);
    if (!/(date|number|duration)/.test(type)) {
      var _column$filterable$fi;
      const me = this,
        value = column.getFilterableValue(record),
        operator = ((_column$filterable$fi = column.filterable.filterField) === null || _column$filterable$fi === void 0 ? void 0 : _column$filterable$fi.operator) ?? '*';
      items.filterStringEquals = {
        text: 'L{equals}',
        localeClass: me,
        icon: 'b-fw-icon b-icon-filter-equal',
        cls: 'b-separator',
        weight: 300,
        disabled: me.disabled,
        onItem: () => me.applyFilter(column, {
          value,
          operator
        })
      };
    }
  }
  /**
   * Add menu items for filtering.
   * @param {Object} options Contains menu items and extra data retrieved from the menu target.
   * @param {Grid.column.Column} options.column Column for which the menu will be shown
   * @param {Core.data.Model} options.record Record for which the menu will be shown
   * @param {Object<String,MenuItemConfig|Boolean|null>} options.items A named object to describe menu items
   * @internal
   */
  populateCellMenu({
    column,
    record,
    items
  }) {
    const me = this;
    if (column.filterable !== false && !record.isSpecialRow) {
      me.populateCellMenuWithDateItems(...arguments);
      me.populateCellMenuWithNumberItems(...arguments);
      me.populateCellMenuWithDurationItems(...arguments);
      me.populateCellMenuWithStringItems(...arguments);
      if (column.meta.isFiltered) {
        items.filterRemove = {
          text: 'L{removeFilter}',
          localeClass: me,
          icon: 'b-fw-icon b-icon-remove',
          cls: 'b-separator',
          weight: 400,
          disabled: me.disabled || me.isMulti && !me.columnHasRemovableFilters(column),
          onItem: () => me.removeFilter(column)
        };
      }
      if (me.isMulti) {
        items.filterDisable = {
          text: 'L{disableFilter}',
          localeClass: me,
          icon: 'b-fw-icon b-icon-filter-disable',
          cls: 'b-separator',
          weight: 400,
          disabled: me.disabled || !me.columnHasEnabledFilters(column),
          onItem: () => me.disableFilter(column)
        };
      }
    }
  }
  /**
   * Used by isMulti mode to determine whether the 'remove filters' menu item should be enabled.
   * @internal
   */
  columnHasRemovableFilters(column) {
    const me = this;
    return Boolean(me.getCurrentMultiFilters(column).find(filter => !me.canDeleteFilter || me.callback(me.canDeleteFilter, me, [filter]) !== false));
  }
  /**
   * Used by isMulti mode to determine whether the 'disable filters' menu item should be enabled.
   * @internal
   */
  columnHasEnabledFilters(column) {
    return Boolean(this.getCurrentMultiFilters(column).find(filter => !filter.disabled));
  }
  /**
   * Add menu item for removing filter if column is filtered.
   * @param {Object} options Contains menu items and extra data retrieved from the menu target.
   * @param {Grid.column.Column} options.column Column for which the menu will be shown
   * @param {Object<String,MenuItemConfig|Boolean|null>} options.items A named object to describe menu items
   * @internal
   */
  populateHeaderMenu({
    column,
    items
  }) {
    const me = this;
    if (column.meta.isFiltered) {
      items.editFilter = {
        text: 'L{editFilter}',
        localeClass: me,
        weight: 100,
        icon: 'b-fw-icon b-icon-filter',
        cls: 'b-separator',
        disabled: me.disabled,
        onItem: () => me.showFilterEditor(column)
      };
      items.removeFilter = {
        text: 'L{removeFilter}',
        localeClass: me,
        weight: 110,
        icon: 'b-fw-icon b-icon-remove',
        disabled: me.disabled || me.isMulti && !me.columnHasRemovableFilters(column),
        onItem: () => me.removeFilter(column)
      };
      if (me.isMulti) {
        items.disableFilter = {
          text: 'L{disableFilter}',
          localeClass: me,
          icon: 'b-fw-icon b-icon-filter-disable',
          weight: 115,
          disabled: me.disabled || !me.columnHasEnabledFilters(column),
          onItem: () => me.disableFilter(column)
        };
      }
    } else if (column.filterable !== false) {
      items.filter = {
        text: 'L{filter}',
        localeClass: me,
        weight: 100,
        icon: 'b-fw-icon b-icon-filter',
        cls: 'b-separator',
        disabled: me.disabled,
        onItem: () => me.showFilterEditor(column)
      };
    }
  }
  //endregion
  //region Events
  // Intercept filtering by a column that has a custom filtering fn, and inject that fn
  onStoreBeforeFilter({
    filters
  }) {
    const {
      columns
    } = this.client;
    for (let i = 0; i < filters.count; i++) {
      const filter = filters.getAt(i);
      // Only take ownership of filters which are not internal
      if (!filter.internal) {
        var _column$filterable;
        const column = (filter.columnOwned || this.prioritizeColumns) && columns.find(col => col.filterable !== false && col.field === filter.property);
        if (column !== null && column !== void 0 && (_column$filterable = column.filterable) !== null && _column$filterable !== void 0 && _column$filterable.filterFn) {
          // If the filter was sourced from the store, replace it with a filter which
          // uses the column's filterFn
          if (!column.$filter) {
            column.$filter = new CollectionFilter({
              columnOwned: true,
              property: filter.property,
              operator: filter.operator,
              value: filter.value,
              filterBy(record) {
                return column.filterable.filterFn({
                  value: this.value,
                  record,
                  operator: this.operator,
                  property: this.property,
                  column
                });
              }
            });
          }
          // Update value and operator used by filters filtering fn
          column.$filter.value = filter.value;
          column.$filter.displayValue = filter.displayValue;
          column.$filter.operator = filter.operator;
          filters.splice(i, 1, column.$filter);
        }
      }
    }
  }
  /**
   * Store filtered; refresh headers.
   * @private
   */
  onStoreFilter() {
    // Pass false to not refresh rows.
    // Store's refresh event will refresh the rows.
    this.refreshHeaders(false);
  }
  /**
   * Called after headers are rendered, make headers match stores initial sorters
   * @private
   */
  renderHeader() {
    this.refreshHeaders(false);
  }
  /**
   * Called when user clicks on the grid. Only care about clicks on the filter icon.
   * @param {MouseEvent} event
   * @private
   */
  onElementClick({
    target
  }) {
    if (this.filterEditorPopup) {
      this.closeFilterEditor();
    }
    if (target.classList.contains('b-filter-icon')) {
      const headerEl = target.closest('.b-grid-header');
      this.showFilterEditor(headerEl.dataset.columnId);
      return false;
    }
  }
  /**
   * Called when user presses F-key grid.
   * @param {MouseEvent} event
   * @private
   */
  showFilterEditorByKey({
    target
  }) {
    const headerEl = target.matches('.b-grid-header') && target;
    // Header must be focused
    if (headerEl) {
      this.showFilterEditor(headerEl.dataset.columnId);
    }
    return Boolean(headerEl);
  }
  // Only care about F key when a filterable header is focused
  isActionAvailable({
    event
  }) {
    const headerElement = event.target.closest('.b-grid-header'),
      column = headerElement && this.client.columns.find(col => col.id === headerElement.dataset.columnId);
    return Boolean(column === null || column === void 0 ? void 0 : column.filterable);
  }
  //endregion
}

Filter._$name = 'Filter';
GridFeatureManager.registerFeature(Filter);

const complexOperators = {
  '*': null,
  isIncludedIn: null,
  startsWith: null,
  endsWidth: null
};
/**
 * @module Grid/feature/FilterBar
 */
/**
 * Feature that allows filtering of the grid by entering filters on column headers.
 * The actual filtering is done by the store.
 * For info on programmatically handling filters, see {@link Core.data.mixin.StoreFilter StoreFilter}.
 *
 * {@inlineexample Grid/feature/FilterBar.js}
 *
 * ```javascript
 * // filtering turned on but no initial filter
 * const grid = new Grid({
 *   features: {
 *     filterBar : true
 *   }
 * });
 *
 * // using initial filter
 * const grid = new Grid({
 *   features : {
 *     filterBar : { filter: { property : 'city', value : 'Gavle' } }
 *   }
 * });
 * ```
 *
 * ## Enabling filtering for a column
 * The individual filterability of columns is defined by a `filterable` property on the column which defaults to `true`.
 * If `false`, that column is not filterable. Note: If you have multiple columns configured with the same `field` value,
 * assign an {@link Core.data.Model#field-id} to the columns to ensure filters work correctly.
 *
 * The property value may also be a custom filter function.
 *
 * The property value may also be an object which may contain the following two properties:
 *  - **filterFn** : `Function` A custom filtering function
 *  - **filterField** : `Object` A config object for the filter value input field. See {@link Core.widget.TextField} or
 *  the other field widgets for reference.
 *
 * ```javascript
 * // Custom filtering function for a column
 * const grid = new Grid({
 *   features : {
 *     filterBar : true
 *   },
 *
 *   columns: [
 *      {
 *        field      : 'age',
 *        text       : 'Age',
 *        type       : 'number',
 *        // Custom filtering function that checks "greater than"
 *        filterable : ({ record, value }) => record.age > value
 *      },
 *      {
 *        field : 'name',
 *        // Filterable may specify a filterFn and a config for the filtering input field
 *        filterable : {
 *          filterFn : ({ record, value }) => record.name.toLowerCase().indexOf(value.toLowerCase()) !== -1,
 *          filterField : {
 *            emptyText : 'Filter name'
 *          }
 *        }
 *      },
 *      {
 *        field : 'city',
 *        text : 'Visited',
 *        flex : 1,
 *        // Filterable with multiselect combo to pick several items to filter
 *        filterable : {
 *          filterField : {
 *            type        : 'combo',
 *            multiSelect : true,
 *            items       : ['Barcelona', 'Moscow', 'Stockholm']
 *          }
 *        }
 *      }
 *   ]
 * });
 * ```
 *
 * If this feature is configured with `prioritizeColumns : true`, those functions will also be used when filtering
 * programmatically:
 *
 * ```javascript
 * const grid = new Grid({
 *    features : {
 *        filterBar : {
 *            prioritizeColumns : true
 *        }
 *    },
 *
 *    columns: [
 *        {
 *          field      : 'age',
 *          text       : 'Age',
 *          type       : 'number',
 *          // Custom filtering function that checks "greater than" no matter
 *          // which field user filled in :)
 *          filterable : ({ record, value, operator }) => record.age > value
 *        }
 *    ]
 * });
 *
 * // Will be used when filtering programmatically or using the UI
 * grid.store.filter({
 *     property : 'age',
 *     value    : 41
 * });
 * ```
 *
 * ## Filtering using a multiselect combo
 *
 * To filter the grid by choosing values which should match with the store data, use a {@link Core.widget.Combo}, and configure
 * your grid like so:
 *
 * ```javascript
 * const grid = new Grid({
 *    features : {
 *        filterBar : true
 *    },
 *
 *    columns : [
 *        {
 *            id         : 'name',
 *            field      : 'name',
 *            text       : 'Name',
 *            filterable : {
 *                filterField : {
 *                    type         : 'combo',
 *                    multiSelect  : true,
 *                    valueField   : 'name',
 *                    displayField : 'name'
 *                }
 *            }
 *        }
 *    ]
 * });
 * ```
 *
 * You can also filter the {@link Core.widget.Combo} values, for example to filter out empty values. Example:
 *
 * ```javascript
 * const grid = new Grid({
 *    features : {
 *        filterBar : true
 *    },
 *
 *    columns : [
 *        {
 *            text       : 'Airline',
 *            field      : 'airline',
 *            flex       : 1,
 *            filterable : {
 *                filterField : {
 *                    type         : 'combo',
 *                    multiSelect  : true,
 *                    valueField   : 'airline',
 *                    displayField : 'airline',
 *                    store        : {
 *                        filters : {
 *                            // Filter out empty values
 *                            filterBy : record => !!record.airline
 *                        }
 *                    }
 *                }
 *            }
 *        }
 *    ]
 * });
 * ```
 *
 * This feature is <strong>disabled</strong> by default.
 *
 * **Note:** This feature cannot be used together with {@link Grid.feature.Filter filter} feature, they are mutually
 * exclusive.
 *
 * @extends Core/mixin/InstancePlugin
 * @demo Grid/filterbar
 * @classtype filterBar
 * @feature
 */
class FilterBar extends InstancePlugin {
  //region Config
  static get $name() {
    return 'FilterBar';
  }
  static get configurable() {
    return {
      /**
       * Use custom filtering functions defined on columns also when programmatically filtering by the columns
       * field.
       *
       * ```javascript
       * const grid = new Grid({
       *     columns : [
       *         {
       *             field : 'age',
       *             text : 'Age',
       *             filterable({ record, value }) {
       *               // Custom filtering, return true/false
       *             }
       *         }
       *     ],
       *
       *     features : {
       *         filterBar : {
       *             prioritizeColumns : true // <--
       *         }
       *     }
       * });
       *
       * // Because of the prioritizeColumns config above, any custom
       * // filterable function on a column will be used when
       * // programmatically filtering by that columns field
       * grid.store.filter({
       *     property : 'age',
       *     value    : 30
       * });
       * ```
       *
       * @config {Boolean}
       * @default
       * @category Common
       */
      prioritizeColumns: false,
      /**
       * The delay in milliseconds to wait after the last keystroke before applying filters.
       * Set to 0 to not trigger filtering from keystrokes, requires pressing ENTER instead
       * @config {Number}
       * @default
       * @category Common
       */
      keyStrokeFilterDelay: 300,
      /**
       * Toggle compact mode. In this mode the filtering fields are styled to transparently overlay the headers,
       * occupying no additional space.
       * @member {Boolean} compactMode
       * @category Common
       */
      /**
       * Specify `true` to enable compact mode for the filter bar. In this mode the filtering fields are styled
       * to transparently overlay the headers, occupying no additional space.
       * @config {Boolean}
       * @default
       * @category Common
       */
      compactMode: false,
      // Destroying data level filters when we hiding UI is supposed to be optional someday. So far this flag is private
      clearStoreFiltersOnHide: true,
      keyMap: {
        // Private
        ArrowUp: {
          handler: 'disableGridNavigation',
          preventDefault: false
        },
        ArrowRight: {
          handler: 'disableGridNavigation',
          preventDefault: false
        },
        ArrowDown: {
          handler: 'disableGridNavigation',
          preventDefault: false
        },
        ArrowLeft: {
          handler: 'disableGridNavigation',
          preventDefault: false
        },
        Enter: {
          handler: 'disableGridNavigation',
          preventDefault: false
        }
      }
    };
  }
  static get pluginConfig() {
    return {
      before: ['renderContents'],
      chain: ['afterColumnsChange', 'renderHeader', 'populateHeaderMenu', 'bindStore']
    };
  }
  static get properties() {
    return {
      filterFieldCls: 'b-filter-bar-field',
      filterFieldInputCls: 'b-filter-bar-field-input',
      filterableColumnCls: 'b-filter-bar-enabled',
      filterFieldInputSelector: '.b-filter-bar-field-input',
      filterableColumnSelector: '.b-filter-bar-enabled',
      filterParseRegExp: /^\s*([<>=*])?(.*)$/,
      storeTrackingSuspended: 0
    };
  }
  //endregion
  //region Init
  construct(grid, config) {
    if (grid.features.filter) {
      throw new Error('Grid.feature.FilterBar feature may not be used together with Grid.feature.Filter, These features are mutually exclusive.');
    }
    const me = this;
    me.grid = grid;
    me.onColumnFilterFieldChange = me.onColumnFilterFieldChange.bind(me);
    super.construct(grid, Array.isArray(config) ? {
      filter: config
    } : config);
    me.bindStore(grid.store);
    if (me.filter) {
      grid.store.filter(me.filter);
    }
    me.gridDetacher = grid.ion({
      beforeElementClick: 'onBeforeElementClick',
      thisObj: me
    });
  }
  bindStore(store) {
    this.detachListeners('store');
    store.ion({
      name: 'store',
      beforeFilter: 'onStoreBeforeFilter',
      filter: 'onStoreFilter',
      thisObj: this
    });
  }
  doDestroy() {
    var _this$gridDetacher;
    this.destroyFilterBar();
    (_this$gridDetacher = this.gridDetacher) === null || _this$gridDetacher === void 0 ? void 0 : _this$gridDetacher.call(this);
    super.doDestroy();
  }
  doDisable(disable) {
    const {
      columns
    } = this.grid;
    // Disable the fields
    columns === null || columns === void 0 ? void 0 : columns.forEach(column => {
      const widget = this.getColumnFilterField(column);
      if (widget) {
        widget.disabled = disable;
      }
    });
    super.doDisable(disable);
  }
  updateCompactMode(value) {
    this.client.headerContainer.classList[value ? 'add' : 'remove']('b-filter-bar-compact');
    for (const prop in this._columnFilters) {
      const field = this._columnFilters[prop];
      field.placeholder = value ? field.column.headerText : null;
    }
  }
  //endregion
  //region FilterBar
  destroyFilterBar() {
    var _this$grid$columns;
    (_this$grid$columns = this.grid.columns) === null || _this$grid$columns === void 0 ? void 0 : _this$grid$columns.forEach(this.destroyColumnFilterField, this);
  }
  /**
   * Hides the filtering fields.
   */
  hideFilterBar() {
    var _me$grid$columns;
    const me = this;
    // We don't want to hear back store "filter" event while we're resetting store filters
    me.clearStoreFiltersOnHide && me.suspendStoreTracking();
    // Hide the fields, each silently - no updating of the store's filtered state until the end
    (_me$grid$columns = me.grid.columns) === null || _me$grid$columns === void 0 ? void 0 : _me$grid$columns.forEach(col => me.hideColumnFilterField(col, true));
    // Now update the filtered state
    me.grid.store.filter();
    me.clearStoreFiltersOnHide && me.resumeStoreTracking();
    me.hidden = true;
  }
  /**
   * Shows the filtering fields.
   */
  showFilterBar() {
    this.suspendStoreTracking();
    this.renderFilterBar();
    this.resumeStoreTracking();
    this.hidden = false;
  }
  /**
   * Toggles the filtering fields visibility.
   */
  toggleFilterBar() {
    if (this.hidden) {
      this.showFilterBar();
    } else {
      this.hideFilterBar();
    }
  }
  /**
   * Renders the filtering fields for filterable columns.
   * @private
   */
  renderFilterBar() {
    if (this.grid.hideHeaders) {
      return;
    }
    this.grid.columns.visibleColumns.forEach(column => this.renderColumnFilterField(column));
    this.rendered = true;
  }
  //endregion
  //region FilterBar fields
  /**
   * Renders text field filter in the provided column header.
   * @param {Grid.column.Column} column Column to render text field filter for.
   * @private
   */
  renderColumnFilterField(column) {
    const me = this,
      {
        grid
      } = me,
      filterable = me.getColumnFilterable(column);
    // we render fields for filterable columns only
    if (filterable && column.isVisible) {
      const headerEl = column.element,
        filter = grid.store.filters.get(column.id) || grid.store.filters.getBy('property', column.field);
      let widget = me.getColumnFilterField(column);
      // if we don't haven't created a field yet
      // we build it from scratch
      if (!widget) {
        const type = `${column.filterType || 'text'}field`,
          {
            filterField
          } = filterable,
          externalCls = filterField === null || filterField === void 0 ? void 0 : filterField.cls;
        if (externalCls) {
          delete filterable.filterField.cls;
        }
        widget = WidgetHelper.append(ObjectHelper.assign({
          type,
          cls: {
            [me.filterFieldCls]: 1,
            [externalCls]: externalCls
          },
          // Simplifies debugging / testing
          dataset: {
            column: column.field
          },
          column,
          owner: grid,
          clearable: true,
          name: column.field,
          value: filter && !filter._filterBy && !filter.internal ? me.buildFilterValue(filter) : '',
          inputCls: me.filterFieldInputCls,
          keyStrokeChangeDelay: me.keyStrokeFilterDelay,
          onChange: me.onColumnFilterFieldChange,
          onClear: me.onColumnFilterFieldChange,
          disabled: me.disabled,
          placeholder: me.compactMode ? column.headerText : null,
          // Also copy formats, DateColumn, TimeColumn etc
          format: column.format
        }, filterField), headerEl)[0];
        if (!(filterField !== null && filterField !== void 0 && filterField.hasOwnProperty('min'))) {
          Object.defineProperty(widget, 'min', {
            get: () => {
              var _column$editor;
              return (_column$editor = column.editor) === null || _column$editor === void 0 ? void 0 : _column$editor.min;
            },
            set: () => null
          });
        }
        if (!(filterField !== null && filterField !== void 0 && filterField.hasOwnProperty('max'))) {
          Object.defineProperty(widget, 'max', {
            get: () => {
              var _column$editor2;
              return (_column$editor2 = column.editor) === null || _column$editor2 === void 0 ? void 0 : _column$editor2.max;
            },
            set: () => null
          });
        }
        if (!(filterField !== null && filterField !== void 0 && filterField.hasOwnProperty('strictParsing'))) {
          Object.defineProperty(widget, 'strictParsing', {
            get: () => {
              var _column$editor3;
              return (_column$editor3 = column.editor) === null || _column$editor3 === void 0 ? void 0 : _column$editor3.strictParsing;
            },
            set: () => null
          });
        }
        // Avoid DomSync cleaning up this widget as it syncs column headers
        widget.element.retainElement = true;
        me.setColumnFilterField(column, widget);
        const hasFilterFieldStoreData = (filterField === null || filterField === void 0 ? void 0 : filterField.store) && (filterField.store.readUrl || filterField.store.data || filterField.store.isChained);
        // If no store is provided for filterable or store is empty, load values lazily from the grid store upon showing the picker list
        if (widget.isCombo && !hasFilterFieldStoreData && widget.store.count === 0) {
          const configuredValue = widget.value,
            refreshData = () => {
              // Might have replaced the widgets store at runtime, make sure we should still force refresh
              if (!(widget.store.readUrl || widget.store.isChained)) {
                widget.store.data = grid.store.getDistinctValues(column.field, true).map(value => grid.store.modelClass.new({
                  id: value,
                  [column.field]: value
                }));
              }
            };
          widget.value = null;
          if (!widget.store.isSorted) {
            widget.store.sort({
              field: column.field,
              ascending: true
            });
          }
          widget.picker.ion({
            beforeShow: refreshData
          });
          refreshData();
          widget.value = configuredValue;
        }
        // If no initial filter exists but a value was provided to the widget, filter by it
        // unless the store is configured to not autoLoad
        if (!me.filter && widget.value && grid.store.autoLoad !== false) {
          me.onColumnFilterFieldChange({
            source: widget,
            value: widget.value
          });
        }
      }
      // if we have one...
      else {
        // re-apply widget filter
        me.onColumnFilterFieldChange({
          source: widget,
          value: widget.value
        });
        // re-append the widget to its parent node (in case the column header was redrawn (happens when resizing columns))
        widget.render(headerEl);
        // show widget in case it was hidden
        widget.show();
      }
      headerEl.classList.add(me.filterableColumnCls);
    }
  }
  /**
   * Fills in column filter fields with values from the grid store filters.
   * @private
   */
  updateColumnFilterFields() {
    const me = this,
      {
        columns,
        store
      } = me.grid;
    let field, filter;
    // During this phase we should not respond to field change events.
    // See onColumnFilterFieldChange.
    me._updatingFields = true;
    for (const column of columns.visibleColumns) {
      field = me.getColumnFilterField(column);
      if (field) {
        filter = store.filters.get(column.id) || store.filters.getBy('property', column.field);
        if (filter && !filter.internal) {
          // For filtering functions we keep what user typed into the field, we cannot construct a filter
          // string from them
          if (!filter._filterBy) {
            field.value = me.buildFilterValue(filter);
          } else {
            field.value = filter.value;
          }
        }
        // No filter, clear field
        else {
          field.value = '';
        }
      }
    }
    me._updatingFields = false;
  }
  getColumnFilterable(column) {
    if (!column.isRoot && column.filterable !== false && column.field && column.isLeaf) {
      if (typeof column.filterable === 'function') {
        column.filterable = {
          filterFn: column.filterable
        };
      }
      return column.filterable;
    }
  }
  destroyColumnFilterField(column) {
    const widget = this.getColumnFilterField(column);
    if (widget) {
      this.hideColumnFilterField(column, true);
      // destroy filter UI field
      widget.destroy();
      // remember there is no field bound anymore
      this.setColumnFilterField(column, undefined);
    }
  }
  hideColumnFilterField(column, silent) {
    const me = this,
      {
        store
      } = me.grid,
      columnEl = column.element,
      widget = me.getColumnFilterField(column);
    if (widget) {
      if (!me.isDestroying) {
        // hide field
        widget.hide();
      }
      const {
        $filter
      } = column;
      if (!store.isDestroyed && me.clearStoreFiltersOnHide && $filter) {
        store.removeFilter($filter, silent);
      }
      columnEl === null || columnEl === void 0 ? void 0 : columnEl.classList.remove(me.filterableColumnCls);
    }
  }
  /**
   * Returns column filter field instance.
   * @param {Grid.column.Column} column Column to get filter field for.
   * @returns {Core.widget.Widget}
   */
  getColumnFilterField(column) {
    var _this$_columnFilters;
    return (_this$_columnFilters = this._columnFilters) === null || _this$_columnFilters === void 0 ? void 0 : _this$_columnFilters[column.id];
  }
  setColumnFilterField(column, widget) {
    this._columnFilters = this._columnFilters || {};
    this._columnFilters[column.data.id] = widget;
  }
  //endregion
  //region Filters
  parseFilterValue(column, value, field) {
    var _column$filterable;
    if (Array.isArray(value)) {
      return {
        value
      };
    }
    if (ObjectHelper.isDate(value)) {
      return {
        operator: field.isDateField ? 'sameDay' : field.isTimeField ? 'sameTime' : '=',
        value
      };
    }
    const match = String(value).match(this.filterParseRegExp);
    return {
      operator: match[1] || ((_column$filterable = column.filterable) === null || _column$filterable === void 0 ? void 0 : _column$filterable.operator) || '*',
      value: match[2]
    };
  }
  buildFilterValue({
    operator,
    value
  }) {
    return value instanceof Date || Array.isArray(value) ? value : (operator in complexOperators ? '' : operator) + value;
  }
  //endregion
  // region Events
  // Intercept filtering by a column that has a custom filtering fn, and inject that fn
  onStoreBeforeFilter({
    filters
  }) {
    const {
      columns
    } = this.client;
    for (let i = 0; i < filters.count; i++) {
      var _column$filterable2;
      const filter = filters.getAt(i),
        column = (filter.columnOwned || this.prioritizeColumns) && columns.find(col => col.filterable !== false && col.field === filter.property);
      if (column !== null && column !== void 0 && (_column$filterable2 = column.filterable) !== null && _column$filterable2 !== void 0 && _column$filterable2.filterFn) {
        // If the filter was sourced from the store, replace it with a filter which
        // uses the column's filterFn
        if (!column.$filter) {
          column.$filter = new CollectionFilter({
            columnOwned: true,
            property: filter.property,
            id: column.id,
            filterBy(record) {
              return column.filterable.filterFn({
                value: this.value,
                record,
                property: this.property,
                column
              });
            }
          });
        }
        // Update value used by filters filtering fn
        column.$filter.value = filter.value;
        filters.splice(i, 1, column.$filter);
      }
    }
  }
  /**
   * Fires when store gets filtered. Refreshes field values in column headers.
   * @private
   */
  onStoreFilter() {
    if (!this.storeTrackingSuspended && this.rendered) {
      this.updateColumnFilterFields();
    }
  }
  afterColumnsChange({
    action,
    changes,
    column,
    columns
  }) {
    // Ignore if columns change while this filter bar is hidden, or if column changeset does not include hidden
    // state
    if (!this.hidden && changes !== null && changes !== void 0 && changes.hidden) {
      const hidden = changes.hidden.value;
      if (hidden) {
        this.destroyColumnFilterField(column);
      } else {
        this.renderColumnFilterField(column);
      }
    }
    if (action === 'remove') {
      columns.forEach(col => this.destroyColumnFilterField(col));
    }
  }
  suspendStoreTracking() {
    this.storeTrackingSuspended++;
  }
  resumeStoreTracking() {
    this.storeTrackingSuspended--;
  }
  /**
   * Called after headers are rendered, make headers match stores initial sorters
   * @private
   */
  renderHeader() {
    if (!this.hidden) {
      this.renderFilterBar();
    }
  }
  renderContents() {
    // Grid suspends events when restoring state, thus we are not informed about toggled columns and might end up
    // with wrong fields in headers. To prevent that, we remove all field elements here since they are restored in
    // renderColumnFilterField() later anyway
    if (this._columnFilters) {
      for (const field of Object.values(this._columnFilters)) {
        field === null || field === void 0 ? void 0 : field.element.remove();
      }
    }
  }
  disableGridNavigation(event) {
    /* If we have navigated (ArrowUp, ArrowLeft, ArrowDown, ArrowRight, Enter) in a filter field, "catch" the key
     * call.
     */
    return event.target.matches(this.filterFieldInputSelector);
  }
  onBeforeElementClick({
    event
  }) {
    // prevent other features reacting when clicking a filter field (or any element inside it)
    if (event.target.closest(`.${this.filterFieldCls}`)) {
      return false;
    }
  }
  /**
   * Called when a column text filter field value is changed by user.
   * @param  {Core.widget.TextField} field Filter text field.
   * @param  {String} value New filtering value.
   * @private
   */
  onColumnFilterFieldChange({
    source: field,
    value
  }) {
    const me = this,
      {
        column
      } = field,
      {
        filterFn
      } = column.filterable,
      {
        store
      } = me.grid,
      filter = column.$filter || store.filters.find(f => (f.id === column.id || f.property === column.field) && !f.internal);
    // Don't respond if we set the value in response to a filter
    if (me._updatingFields) {
      return;
    }
    const isClearingFilter = value == null || value === '' || Array.isArray(value) && value.length === 0;
    // Remove previous iteration of the column's filter
    store.removeFilter(filter, true);
    column.$filter = null;
    if (isClearingFilter) {
      // This is a no-op if there was no matching filter anyway
      if (!filter) {
        return;
      }
    } else {
      var _column$filterable3, _column$filterable4;
      // Must add the filter silently, so that the column gets a reference to its $filter
      // before events are broadcast
      column.$filter = store.addFilter({
        property: field.name,
        ...me.parseFilterValue(column, value, field),
        [typeof ((_column$filterable3 = column.filterable) === null || _column$filterable3 === void 0 ? void 0 : _column$filterable3.caseSensitive) === 'boolean' ? 'caseSensitive' : undefined]: (_column$filterable4 = column.filterable) === null || _column$filterable4 === void 0 ? void 0 : _column$filterable4.caseSensitive,
        // Only inject a filterBy configuration if the column has a custom filterBy
        [filterFn ? 'filterBy' : '_']: function (record) {
          return filterFn({
            value: this.value,
            record,
            operator: this.operator,
            property: this.property,
            column
          });
        }
      }, true);
    }
    // Apply the new set of store filters.
    store.filter();
  }
  //endregion
  //region Menu items
  /**
   * Adds a menu item to toggle filter bar visibility.
   * @param {Object} options Contains menu items and extra data retrieved from the menu target.
   * @param {Object<String,MenuItemConfig|Boolean|null>} options.items A named object to describe menu items
   * @internal
   */
  populateHeaderMenu({
    items
  }) {
    items.toggleFilterBar = {
      text: this.hidden ? 'L{enableFilterBar}' : 'L{disableFilterBar}',
      localeClass: this,
      weight: 120,
      icon: 'b-fw-icon b-icon-filter',
      cls: 'b-separator',
      onItem: () => this.toggleFilterBar()
    };
  }
  //endregion
}

FilterBar.featureClass = 'b-filter-bar';
FilterBar._$name = 'FilterBar';
GridFeatureManager.registerFeature(FilterBar);

/**
 * @module Grid/feature/Group
 */
/**
 * Enables rendering and handling of row groups. The actual grouping is done in the store, but triggered by [shift] +
 * clicking headers or by using two finger tap (one on header, one anywhere on grid). Use [shift] + [alt] + click to
 * remove a column grouper.
 *
 * Groups can be expanded/collapsed by clicking on the group row or pressing [space] when group row is selected.
 * The actual grouping is done by the store, see {@link Core.data.mixin.StoreGroup#function-group}.
 *
 * Grouping by a field performs sorting by the field automatically. It's not possible to prevent sorting.
 * If you group, the records have to be sorted so that records in a group stick together. You can either control sorting
 * direction, or provide a custom sorting function called {@link #config-groupSortFn} to your feature config object.
 *
 * For info on programmatically handling grouping, see {@link Core.data.mixin.StoreGroup StoreGroup}.
 *
 * Currently, grouping is not supported when using pagination, the underlying store cannot group data that is split into pages.
 *
 * **Note:** Custom height for group header rows cannot be set with CSS, should instead be defined in a renderer function using the `size` param. See the {@link #config-renderer} config for details.
 *
 * This feature is **enabled** by default.
 *
 * ## Keyboard shortcuts
 * This feature has the following default keyboard shortcuts:
 *
 * | Keys     | Action        | Action description                                                         |
 * |----------|---------------|----------------------------------------------------------------------------|
 * | `Space`  | *toggleGroup* | When a group header is focused, this expands or collapses the grouped rows |
 *
 * <div class="note">Please note that <code>Ctrl</code> is the equivalent to <code>Command</code> and <code>Alt</code>
 * is the equivalent to <code>Option</code> for Mac users</div>
 *
 * For more information on how to customize keyboard shortcuts, please see
 * [our guide](#Grid/guides/customization/keymap.md)
 *
 * @example
 * // grouping feature is enabled, no default value though
 * let grid = new Grid({
 *     features : {
 *         group : true
 *     }
 * });
 *
 * // use initial grouping
 * let grid = new Grid({
 *     features : {
 *         group : 'city'
 *     }
 * });
 *
 * // default grouper and custom renderer, which will be applied to each cell except the "group" cell
 * let grid = new Grid({
 *     features : {
 *       group : {
 *           field : 'city',
 *           ascending : false,
 *           renderer : ({ isFirstColumn, count, groupRowFor, record }) => isFirstColumn ? `${groupRowFor} (${count})` : ''
 *       }
 *     }
 * });
 *
 * // group using custom sort function
 * let grid = new Grid({
 *     features : {
 *         group       : {
 *             field       : 'city',
 *             groupSortFn : (a, b) => a.city.length < b.city.length ? -1 : 1
 *         }
 *     }
 * });
 *
 * // can also be specified on the store
 * let grid = new Grid({
 *     store : {
 *         groupers : [
 *             { field : 'city', ascending : false }
 *         ]
 *     }
 * });
 *
 * // custom sorting function can also be specified on the store
 * let grid = new Grid({
 *     store : {
 *         groupers : [{
 *             field : 'city',
 *             fn : (recordA, recordB) => {
 *                 // apply custom logic, for example:
 *                 return recordA.city.length < recordB.city.length ? -1 : 1;
 *             }
 *         }]
 *     }
 * });
 *
 * @extends Core/mixin/InstancePlugin
 *
 * @demo Grid/grouping
 * @classtype group
 * @feature
 *
 * @inlineexample Grid/feature/Group.js
 */
class Group extends InstancePlugin {
  static get $name() {
    return 'Group';
  }
  static get configurable() {
    return {
      /**
       * The name of the record field to group by.
       * @config {String}
       * @default
       */
      field: null,
      /**
       * A function used to sort the groups.
       * When grouping, the records have to be sorted so that records in a group stick together.
       * Technically that means that records having the same {@link #config-field} value
       * should go next to each other.
       * And this function (if provided) is responsible for applying such grouping order.
       * ```javascript
       * const grid = new Grid({
       *     features : {
       *         group : {
       *             // group by category
       *             field       : 'category',
       *             groupSortFn : (a, b) => {
       *                 const
       *                     aCategory = a.category || '',
       *                     bCategory = b.category || '';
       *
       *                 // 1st sort by "calegory" field
       *                 return aCategory > bCategory ? -1 :
       *                     aCategory < bCategory ? 1 :
       *                     // inside calegory groups we sort by "name" field
       *                     (a.name > b.name ? -1 : 1);
       *             }
       *         }
       *     }
       * });
       * ```
       * @config {Function}
       */
      groupSortFn: null,
      /**
       * A function which produces the HTML for a group header.
       * The function is called in the context of this Group feature object.
       * Default group renderer displays the `groupRowFor` and `count`.
       *
       * @config {Function}
       * @property {String} groupRowFor The value of the `field` for the group.
       * @property {Core.data.Model} record The group record representing the group.
       * @property {Object} record.meta Meta data with additional info about the grouping.
       * @property {Array} record.groupChildren The group child records.
       * @property {Number} count Number of records in the group.
       * @property {Grid.column.Column} column The column the renderer runs for.
       * @property {Boolean} isFirstColumn True, if `column` is the first column.
       * If `RowNumberColumn` is the real first column, it's not taken into account.
       * @property {Grid.column.Column} [groupColumn] The column under which the `field` is shown.
       * @property {Object} size Sizing information for the group header row, only `height` is relevant.
       * @property {Number} size.height The height of the row, set this if you want a custom height for the group header row
       * That is UI part, so do not rely on its existence.
       * @default
       */
      renderer: null,
      /**
       * See {@link #keyboard-shortcuts Keyboard shortcuts} for details
       * @config {Object<String,String>}
       */
      keyMap: {
        ' ': 'toggleGroup'
      },
      /**
       * By default, clicking anywhere in a group row toggles its expanded/collapsed state.
       *
       * Configure this as `false` to limit this to only toggling on click of the expanded/collapsed
       * state icon.
       * @prp {Boolean}
       * @default
       */
      toggleOnRowClick: true
    };
  }
  //region Init
  construct(grid, config) {
    const me = this;
    if (grid.features.tree) {
      return;
    }
    // groupSummary feature needs to be initialized first, if it is used
    me._thisIsAUsedExpression(grid.features.groupSummary);
    // process initial config into an actual config object
    config = me.processConfig(config);
    me.grid = grid;
    super.construct(grid, config);
    me.bindStore(grid.store);
    grid.rowManager.ion({
      beforeRenderRow: 'onBeforeRenderRow',
      renderCell: 'renderCell',
      // The feature gets to see cells being rendered before the GroupSummary feature
      // because this injects header content into group header rows and adds rendering
      // info to the cells renderData which GroupSummary must comply with.
      prio: 1100,
      thisObj: me
    });
  }
  // Group feature handles special config cases, where user can supply a string or a group config object
  // instead of a normal config object
  processConfig(config) {
    if (typeof config === 'string') {
      return {
        field: config,
        ascending: null
      };
    }
    return config;
  }
  // override setConfig to process config before applying it (used mainly from ReactGrid)
  setConfig(config) {
    if (config === null) {
      this.store.clearGroupers();
    } else {
      super.setConfig(this.processConfig(config));
    }
  }
  bindStore(store) {
    this.detachListeners('store');
    store.ion({
      name: 'store',
      group: 'onStoreGroup',
      change: 'onStoreChange',
      thisObj: this
    });
    this.onStoreGroup({
      groupers: store.groupers
    });
  }
  updateRenderer(renderer) {
    this.groupRenderer = renderer;
  }
  updateField(field) {
    this.store.group({
      field,
      ascending: this.ascending,
      fn: this.groupSortFn
    });
  }
  updateGroupSortFn(fn) {
    if (!this.isConfiguring) {
      this.store.group({
        field: this.field,
        ascending: this.ascending,
        fn
      });
    }
  }
  doDestroy() {
    super.doDestroy();
  }
  doDisable(disable) {
    const {
      store
    } = this;
    // Grouping mostly happens in store, need to clear groupers there to remove headers.
    // Use configured groupers as first sorters to somewhat maintain the order
    if (disable && store.isGrouped) {
      const {
        sorters
      } = store;
      sorters.unshift(...store.groupers);
      this.currentGroupers = store.groupers;
      store.clearGroupers();
      store.sort(sorters);
    } else if (!disable && this.currentGroupers) {
      store.group(this.currentGroupers[0]);
      this.currentGroupers = null;
    }
    super.doDisable(disable);
  }
  get store() {
    return this.grid.store;
  }
  //endregion
  //region Plugin config
  // Plugin configuration. This plugin chains some of the functions in Grid.
  static get pluginConfig() {
    return {
      assign: ['collapseAll', 'expandAll'],
      chain: ['renderHeader', 'populateHeaderMenu', 'getColumnDragToolbarItems', 'onElementTouchStart', 'onElementClick', 'bindStore']
    };
  }
  //endregion
  //region Expand/collapse
  /**
   * Collapses or expands a group depending on its current state
   * @param {Core.data.Model|String} recordOrId Record or records id for a group row to collapse or expand
   * @param {Boolean} collapse Force collapse (`true`) or expand (`false`)
   * @fires togglegroup
   */
  toggleCollapse(recordOrId, collapse) {
    this.internalToggleCollapse(recordOrId, collapse);
  }
  /**
   * Collapses or expands a group depending on its current state
   * @param {Core.data.Model|String} recordOrId Record or records id for a group row to collapse or expand
   * @param {Boolean} collapse Force collapse (true) or expand (true)
   * @param {Boolean} skipRender True to not render rows
   * @param {Event} domEvent The user interaction event (eg a `click` event) if the toggle request was
   * instigated by user interaction.
   * @internal
   * @fires togglegroup
   */
  internalToggleCollapse(recordOrId, collapse, skipRender = false, domEvent) {
    const me = this,
      {
        store,
        grid
      } = me,
      groupRecord = store.getById(recordOrId);
    if (!groupRecord.isGroupHeader) {
      return;
    }
    collapse = collapse === undefined ? !groupRecord.meta.collapsed : collapse;
    /**
     * Fired when a group is going to be expanded or collapsed using the UI.
     * Returning `false` from a listener prevents the operation
     * @event beforeToggleGroup
     * @on-owner
     * @preventable
     * @param {Core.data.Model} groupRecord Group record
     * @param {Boolean} collapse Collapsed (true) or expanded (false)
     * @param {Event} domEvent The user interaction event (eg a `click` event) if the toggle request was
     * instigated by user interaction.
     */
    if (grid.trigger('beforeToggleGroup', {
      groupRecord,
      collapse,
      domEvent
    }) === false) {
      return;
    }
    if (collapse) {
      store.collapse(groupRecord);
    } else {
      store.expand(groupRecord);
    }
    if (!skipRender) {
      // If collapsing the group reduces amount of records below amount of rendered rows, we need to refresh
      // entire view
      // https://github.com/bryntum/support/issues/5893
      if (grid.rowManager.rowCount > grid.store.count) {
        grid.rowManager.renderFromRow();
      } else {
        // render from group record and down, no need to touch those above
        grid.rowManager.renderFromRecord(groupRecord);
      }
    }
    /**
     * Group expanded or collapsed
     * @event toggleGroup
     * @on-owner
     * @param {Core.data.Model} groupRecord Group record
     * @param {Boolean} collapse Collapsed (true) or expanded (false)
     */
    grid.trigger('toggleGroup', {
      groupRecord,
      collapse
    });
    grid.afterToggleGroup();
  }
  /**
   * Collapse all groups. This function is exposed on Grid and can thus be called as `grid.collapseAll()`
   * @on-owner
   */
  collapseAll() {
    const me = this;
    if (me.store.isGrouped && !me.disabled) {
      me.store.groupRecords.forEach(r => me.internalToggleCollapse(r, true, true));
      me.grid.refreshRows(true);
    }
  }
  /**
   * Expand all groups. This function is exposed on Grid and can thus be called as `grid.expandAll()`
   * @on-owner
   */
  expandAll() {
    const me = this;
    if (me.store.isGrouped && !me.disabled) {
      me.store.groupRecords.forEach(r => me.internalToggleCollapse(r, false, true));
      me.grid.refreshRows();
    }
  }
  //endregion
  //region Rendering
  /**
   * Called before rendering row contents, used to reset rows no longer used as group rows
   * @private
   */
  onBeforeRenderRow({
    row
  }) {
    // row.id contains previous record id on before render
    const oldRecord = row.grid.store.getById(row.id);
    // force update of inner html if this row used for group data
    row.forceInnerHTML = row.forceInnerHTML || (oldRecord === null || oldRecord === void 0 ? void 0 : oldRecord.isGroupHeader);
  }
  /**
   * Called when a cell is rendered, styles the group rows first cell.
   * @private
   */
  renderCell(renderData) {
    const me = this,
      {
        cellElement,
        row,
        column
      } = renderData,
      {
        meta
      } = renderData.record,
      rowClasses = {
        'b-group-row': 0,
        'b-grid-group-collapsed': 0
      };
    if (!me.disabled && me.store.isGrouped && 'groupRowFor' in meta) {
      // do nothing with action column to make possible using actions for groups
      if (column.type === 'action') {
        return;
      }
      // let column clear the cell, in case it needs to do some cleanup
      column.clearCell(cellElement);
      // this is a group row, add css classes
      rowClasses['b-grid-group-collapsed'] = meta.collapsed;
      rowClasses['b-group-row'] = 1;
      me.buildGroupHeader(renderData);
      if (column === me.groupHeaderColumn) {
        DomHelper.createElement({
          parent: cellElement,
          tag: 'i',
          className: 'b-group-state-icon',
          nextSibling: cellElement.firstChild
        });
        cellElement.classList.add('b-group-title');
        cellElement.$groupHeader = cellElement._hasHtml = true;
      }
    } else if (cellElement.$groupHeader) {
      var _cellElement$querySel;
      (_cellElement$querySel = cellElement.querySelector('.b-group-state-icon')) === null || _cellElement$querySel === void 0 ? void 0 : _cellElement$querySel.remove();
      cellElement.classList.remove('b-group-title');
      cellElement.$groupHeader = false;
    }
    // Still need to sync row classes is disabled or not grouped.
    // Previous b-group-row and b-grid-group-collapsed classes must be removed.
    row.assignCls(rowClasses);
  }
  // renderData.cellElement is required
  buildGroupHeader(renderData) {
    const me = this,
      {
        record,
        cellElement,
        column,
        persist
      } = renderData,
      {
        grid
      } = me,
      meta = record.meta,
      {
        groupRowFor
      } = meta,
      {
        groupSummary
      } = grid.features,
      // Need to adjust count if group summary is used
      count = meta.childCount - (groupSummary && groupSummary.target !== 'header' ? 1 : 0);
    let html = null,
      applyDefault = true;
    if (persist || column) {
      const groupColumn = grid.columns.get(meta.groupField),
        isGroupHeaderColumn = renderData.isFirstColumn = column === me.groupHeaderColumn;
      // First try using columns groupRenderer (might not even have a column if grouping programmatically)
      if (groupColumn !== null && groupColumn !== void 0 && groupColumn.groupRenderer) {
        if (isGroupHeaderColumn) {
          // groupRenderer could return nothing and just apply changes directly to DOM element
          html = groupColumn.groupRenderer({
            ...renderData,
            groupRowFor,
            groupRecords: record.groupChildren,
            groupColumn,
            count
          });
          applyDefault = false;
        }
      }
      // Secondly use features groupRenderer, if configured with one
      else if (me.groupRenderer) {
        // groupRenderer could return nothing and just apply changes directly to DOM element
        html = me.groupRenderer({
          ...renderData,
          groupRowFor,
          groupRecords: record.groupChildren,
          groupColumn,
          count,
          isFirstColumn: isGroupHeaderColumn
        });
      }
      // Third, just display unformatted value and child count (also applied for features groupRenderer that do
      // not output any html of their own)
      if (isGroupHeaderColumn && html == null && applyDefault && DomHelper.getChildElementCount(cellElement) === 0) {
        html = StringHelper.encodeHtml(`${groupRowFor === '__novalue__' ? '' : groupRowFor} (${count})`);
      }
    } else if (me.groupRenderer) {
      // groupRenderer could return nothing and just apply changes directly to DOM element
      html = me.groupRenderer(renderData);
    }
    // Renderers could return nothing and just apply changes directly to DOM element
    if (typeof html === 'string') {
      cellElement.innerHTML = html;
    } else if (typeof html === 'object') {
      DomSync.sync({
        targetElement: cellElement,
        domConfig: {
          onlyChildren: true,
          children: ArrayHelper.asArray(html)
        }
      });
    }
    // If groupRenderer added elements to the cell, we need to remember that to clear it on re-usage as a normal cell
    if (DomHelper.getChildElementCount(cellElement) > 0) {
      cellElement._hasHtml = true;
    }
    return cellElement.innerHTML;
  }
  get groupHeaderColumn() {
    return this.grid.columns.visibleColumns.find(column => !column.groupHeaderReserved);
  }
  /**
   * Called when an header is rendered, adds grouping icon if grouped by that column.
   * @private
   * @param headerContainerElement
   */
  renderHeader(headerContainerElement) {
    const {
      store,
      grid
    } = this;
    if (store.isGrouped) {
      // Sorted from start, reflect in rendering
      for (const groupInfo of store.groupers) {
        // Might be grouping by field without column, which is valid
        const column = grid.columns.get(groupInfo.field),
          header = column && grid.getHeaderElement(column.id);
        header === null || header === void 0 ? void 0 : header.classList.add('b-group', groupInfo.ascending ? 'b-asc' : 'b-desc');
      }
    }
  }
  //endregion
  //region Context menu
  /**
   * Supply items for headers context menu.
   * @param {Object} options Contains menu items and extra data retrieved from the menu target.
   * @param {Grid.column.Column} options.column Column for which the menu will be shown
   * @param {Object<String,MenuItemConfig|Boolean|null>} options.items A named object to describe menu items
   * @internal
   */
  populateHeaderMenu({
    column,
    items
  }) {
    const me = this;
    if (column.groupable !== false) {
      items.groupAsc = {
        text: 'L{groupAscending}',
        localeClass: me,
        icon: 'b-fw-icon b-icon-group-asc',
        cls: 'b-separator',
        weight: 400,
        disabled: me.disabled,
        onItem: () => me.store.group(column.field, true)
      };
      items.groupDesc = {
        text: 'L{groupDescending}',
        localeClass: me,
        icon: 'b-fw-icon b-icon-group-desc',
        weight: 410,
        disabled: me.disabled,
        onItem: () => me.store.group(column.field, false)
      };
    }
    if (me.store.isGrouped) {
      items.groupRemove = {
        text: 'L{stopGrouping}',
        localeClass: me,
        icon: 'b-fw-icon b-icon-clear',
        cls: column.groupable ? '' : 'b-separator',
        weight: 420,
        disabled: me.disabled,
        onItem: () => me.store.clearGroupers()
      };
    }
  }
  /**
   * Supply items to ColumnDragToolbar
   * @private
   */
  getColumnDragToolbarItems(column, items) {
    var _store$groupers;
    const me = this,
      {
        store,
        disabled
      } = me;
    items.push({
      text: 'L{groupAscendingShort}',
      group: 'L{group}',
      localeClass: me,
      icon: 'b-icon b-icon-group-asc',
      ref: 'groupAsc',
      cls: 'b-separator',
      weight: 110,
      disabled,
      onDrop: ({
        column
      }) => store.group(column.field, true)
    });
    items.push({
      text: 'L{groupDescendingShort}',
      group: 'L{group}',
      localeClass: me,
      icon: 'b-icon b-icon-group-desc',
      ref: 'groupDesc',
      weight: 110,
      disabled,
      onDrop: ({
        column
      }) => store.group(column.field, false)
    });
    const grouped = ((_store$groupers = store.groupers) === null || _store$groupers === void 0 ? void 0 : _store$groupers.some(col => col.field === column.field)) && !disabled;
    items.push({
      text: 'L{stopGroupingShort}',
      group: 'L{group}',
      localeClass: me,
      icon: 'b-icon b-icon-clear',
      ref: 'groupRemove',
      disabled: !grouped,
      weight: 110,
      onDrop: ({
        column
      }) => store.removeGrouper(column.field)
    });
    return items;
  }
  //endregion
  //region Events - Store
  /**
   * Called when store grouping changes. Reflects on header and rerenders rows.
   * @private
   */
  onStoreGroup({
    groupers
  }) {
    const {
        grid
      } = this,
      {
        element
      } = grid,
      curGroupHeaders = element && DomHelper.children(element, '.b-grid-header.b-group');
    if (element) {
      for (const header of curGroupHeaders) {
        header.classList.remove('b-group', 'b-asc', 'b-desc');
      }
      if (groupers) {
        for (const groupInfo of groupers) {
          const header = grid.getHeaderElementByField(groupInfo.field);
          if (header) {
            header.classList.add('b-group', groupInfo.ascending ? 'b-asc' : 'b-desc');
          }
        }
      }
    }
  }
  onStoreChange({
    action,
    records
  }) {
    const {
        client
      } = this,
      {
        rowManager,
        store
      } = client;
    if (store.isGrouped && action === 'move') {
      const {
          field
        } = store.groupers[0],
        fromRow = Math.min(...records.reduce((result, record) => {
          // Get index of the new group
          result.push(store.indexOf(record.instanceMeta(store).groupParent));
          // Get index of the old group
          if (field in record.meta.modified) {
            const oldGroup = store.groupRecords.find(r => r.meta.groupRowFor === record.meta.modified[field]);
            if (oldGroup) {
              result.push(store.indexOf(oldGroup));
            }
          }
          return result;
        }, []));
      rowManager.renderFromRow(rowManager.getRow(fromRow));
    }
  }
  //endregion
  //region Events - Grid
  /**
   * Store touches when user touches header, used in onElementTouchEnd.
   * @private
   */
  onElementTouchStart(event) {
    const me = this,
      {
        target
      } = event,
      header = target.closest('.b-grid-header'),
      column = header && me.grid.getColumnFromElement(header);
    // If it's a multi touch, group.
    if (event.touches.length > 1 && column && column.groupable !== false && !me.disabled) {
      me.store.group(column.field);
    }
  }
  /**
   * React to click on headers (to group by that column if [alt] is pressed) and on group rows (expand/collapse).
   * @private
   * @param event
   * @returns {Boolean}
   */
  onElementClick(event) {
    const me = this,
      {
        store
      } = me,
      {
        target
      } = event,
      row = target.closest('.b-group-row'),
      header = target.closest('.b-grid-header'),
      field = header === null || header === void 0 ? void 0 : header.dataset.column;
    // prevent expand/collapse if disabled or clicked on item with own handler
    if (target.classList.contains('b-resizer') || me.disabled || target.classList.contains('b-action-item') || event.handled) {
      return;
    }
    // Header
    if (header && field) {
      var _store$groupers2;
      const columnGrouper = (_store$groupers2 = store.groupers) === null || _store$groupers2 === void 0 ? void 0 : _store$groupers2.find(g => g.field === field);
      // Store has a grouper for this column's field; flip grouper order
      if (columnGrouper && !event.shiftKey) {
        columnGrouper.ascending = !columnGrouper.ascending;
        store.group();
        return false;
      }
      // Group or ungroup
      else if (event.shiftKey) {
        const column = me.grid.columns.get(field);
        if (column.groupable !== false) {
          if (event.altKey) {
            store.removeGrouper(field);
          } else {
            store.group(field);
          }
        }
      }
    }
    // Anywhere on group-row if toggleOnRowClick set, otherwise only on icon
    if (row && (me.toggleOnRowClick || event.target.classList.contains('b-group-state-icon'))) {
      me.internalToggleCollapse(DomDataStore.get(row).id, undefined, undefined, event);
      return false;
    }
  }
  /**
   * Toggle groups with [space].
   * @private
   * @param event
   */
  toggleGroup(event) {
    var _focusedCell$record;
    const {
        grid
      } = this,
      {
        focusedCell
      } = grid;
    // only catch space when focus is on a group header cell
    if (!this.disabled && !focusedCell.isActionable && (_focusedCell$record = focusedCell.record) !== null && _focusedCell$record !== void 0 && _focusedCell$record.isGroupHeader) {
      this.internalToggleCollapse(focusedCell.id);
      // Other features (like context menu) must not process this.
      return true;
    }
    return false;
  }
  //endregion
}

Group._$name = 'Group';
GridFeatureManager.registerFeature(Group, true, ['Grid', 'Scheduler']);
GridFeatureManager.registerFeature(Group, false, ['TreeGrid']);

/**
 * @module Grid/feature/HeaderMenu
 */
/**
 * Right click column header or focus it and press SPACE key to show the context menu for headers.
 *
 * ### Default header menu items
 *
 * The Header menu has no default items provided by the `HeaderMenu` feature, but there are other features
 * that populate the header menu with the following items:
 *
 * | Reference         | Text                              | Weight | Feature                                        | Description                                       |
 * |-------------------|-----------------------------------|--------|------------------------------------------------|---------------------------------------------------|
 * | `filter`          | Filter                            | 100    | {@link Grid.feature.Filter Filter}             | Shows the filter popup to add a filter            |
 * | `editFilter`      | Edit filter                       | 100    | {@link Grid.feature.Filter Filter}             | Shows the filter popup to change/remove a filter  |
 * | `removeFilter`    | Remove filter                     | 110    | {@link Grid.feature.Filter Filter}             | Stops filtering by selected column field          |
 * | `toggleFilterBar` | Hide filter bar / Show filter bar | 120    | {@link Grid.feature.FilterBar FilterBar}       | Toggles filter bar visibility                     |
 * | `columnPicker`    | Columns                           | 200    | {@link Grid.feature.ColumnPicker ColumnPicker} | Shows a submenu to control columns visibility     |
 * | \>column.id*      | column.text*                      |        | {@link Grid.feature.ColumnPicker ColumnPicker} | Check item to hide/show corresponding column      |
 * | `hideColumn`      | Hide column                       | 210    | {@link Grid.feature.ColumnPicker ColumnPicker} | Hides selected column                             |
 * | `rename`          | Rename column text                | 215    | {@link Grid.feature.ColumnRename ColumnRename} | Edits the header text of the column               |
 * | `toggleCollapse`  | Collapse column / Expand column   | 215    | This feature                                   | Expands or collapses a collapsible column         |
 * | `movePrev  `      | Move previous                     | 220    | This feature                                   | Moves selected column before its previous sibling |
 * | `moveNext`        | Move next                         | 230    | This feature                                   | Moves selected column after its next sibling      |
 * | `sortAsc`         | Sort ascending                    | 300    | {@link Grid.feature.Sort Sort}                 | Sort by the column field in ascending order       |
 * | `sortDesc`        | Sort descending                   | 310    | {@link Grid.feature.Sort Sort}                 | Sort by the column field in descending order      |
 * | `multiSort`       | Multi sort                        | 320    | {@link Grid.feature.Sort Sort}                 | Shows a submenu to control multi-sorting          |
 * | \>`addSortAsc`    | Add ascending sorting             | 330    | {@link Grid.feature.Sort Sort}                 | Adds ascending sorter using the column field      |
 * | \>`addSortDesc`   | Add descending sorting            | 340    | {@link Grid.feature.Sort Sort}                 | Adds descending sorter using the column field     |
 * | \>`removeSorter`  | Remove sorter                     | 350    | {@link Grid.feature.Sort Sort}                 | Stops sorting by selected column field            |
 * | `groupAsc`        | Group ascending                   | 400    | {@link Grid.feature.Group Group}               | Group by the column field in ascending order      |
 * | `groupDesc`       | Group descending                  | 410    | {@link Grid.feature.Group Group}               | Group by the column field in descending order     |
 * | `groupRemove`     | Stop grouping                     | 420    | {@link Grid.feature.Group Group}               | Stops grouping                                    |
 * | `mergeCells`      | Merge cells                       | 500    | {@link Grid.feature.MergeCells}                | Merge cells with same value in a sorted column    |
 *
 * \* - items that are generated dynamically
 *
 * \> - first level of submenu
 *
 * ### Customizing the menu items
 *
 * The menu items in the Header menu can be customized, existing items can be changed or removed,
 * and new items can be added. This is handled using the `items` config of the feature.
 *
 * Add extra items for all columns:
 *
 * ```javascript
 * const grid = new Grid({
 *   features : {
 *     headerMenu : {
 *       items : {
 *         extraItem : { text: 'My header item', icon: 'fa fa-car', weight: 200, onItem : () => ... }
 *       }
 *     }
 *   }
 * });
 * ```
 *
 * It is also possible to add items using columns config. See examples below.
 *
 * Add extra items for a single column:
 *
 * ```javascript
 * const grid = new Grid({
 *   columns: [
 *     {
 *       field: 'name',
 *       text: 'Name',
 *       headerMenuItems: {
 *         columnItem : { text: 'My unique header item', icon: 'fa fa-flask', onItem : () => ... }
 *       }
 *     }
 *   ]
 * });
 * ```
 *
 * Remove built in item:
 *
 * ```javascript
 * const grid = new Grid({
 *   features : {
 *     headerMenu : {
 *       items : {
 *          // Hide 'Stop grouping'
 *          groupRemove : false
 *       }
 *     }
 *   }
 * });
 * ```
 *
 * Customize built in item:
 *
 * ```javascript
 * const grid = new Grid({
 *   features : {
 *     headerMenu : {
 *       items : {
 *          hideColumn : {
 *              text : 'Bye bye column'
 *          }
 *       }
 *     }
 *   }
 * });
 * ```
 *
 * Remove nested menu item:
 *
 * ```javascript
 * const grid = new Grid({
 *     features : {
 *         headerMenu : {
 *             items : {
 *                 multiSort : {
 *                     menu : { removeSorter : false }
 *                 }
 *             }
 *         }
 *     }
 * });
 * ```
 *
 * It is also possible to manipulate the default items and add new items in the processing function:
 *
 * ```javascript
 * const grid = new Grid({
 *   features : {
 *     headerMenu : {
 *       processItems({items, record}) {
 *           if (record.cost > 5000) {
 *              items.myItem = { text : 'Split cost' };
 *           }
 *       }
 *     }
 *   }
 * });
 * ```
 *
 * Full information of the menu customization can be found in the "Customizing the Cell menu and the Header menu" guide.
 *
 * This feature is <strong>enabled</strong> by default.
 *
 * ## Keyboard shortcuts
 * This feature has the following default keyboard shortcuts:
 *
 * | Keys           | Action                 | Action description                              |
 * |----------------|------------------------|-------------------------------------------------|
 * | `Space`        | *showContextMenuByKey* | Shows context menu for currently focused header |
 * | `Ctrl`+`Space` | *showContextMenuByKey* | Shows context menu for currently focused header |
 *
 * <div class="note">Please note that <code>Ctrl</code> is the equivalent to <code>Command</code> and <code>Alt</code>
 * is the equivalent to <code>Option</code> for Mac users</div>
 *
 * For more information on how to customize keyboard shortcuts, please see
 * [our guide](#Grid/guides/customization/keymap.md)
 *
 * @extends Core/feature/base/ContextMenuBase
 * @demo Grid/contextmenu
 * @classtype headerMenu
 * @feature
 *
 * @inlineexample Grid/feature/HeaderMenu.js
 */
class HeaderMenu extends ContextMenuBase {
  //region Config
  static get $name() {
    return 'HeaderMenu';
  }
  static get configurable() {
    return {
      type: 'header',
      /**
       * This is a preconfigured set of items used to create the default context menu.
       *
       * The `items` provided by this feature are listed in the intro section of this class. You can
       * configure existing items by passing a configuration object to the keyed items.
       *
       * To remove existing items, set corresponding keys to `null`:
       *
       * ```javascript
       * const scheduler = new Scheduler({
       *     features : {
       *         headerMenu : {
       *             items : {
       *                 filter        : null,
       *                 columnPicker  : null
       *             }
       *         }
       *     }
       * });
       * ```
       *
       * See the feature config in the above example for details.
       *
       * @config {Object<String,MenuItemConfig|Boolean|null>} items
       */
      items: null,
      /**
       * Configure as `true` to show two extra menu options to move the selected column to either
       * before its previous sibling, or after its next sibling.
       *
       * This is a keyboard-accessible version of drag/drop column reordering.
       * @config {Boolean}
       * @category Accessibility
       */
      moveColumns: null
      /**
       * See {@link #keyboard-shortcuts Keyboard shortcuts} for details
       * @config {Object<String,String>} keyMap
       */
    };
  }

  static get defaultConfig() {
    return {
      /**
       * A function called before displaying the menu that allows manipulations of its items.
       * Returning `false` from this function prevents the menu being shown.
       *
       * ```javascript
       *   features         : {
       *       headerMenu : {
       *           processItems({ column, items }) {
       *               // Add or hide existing items here as needed
       *               items.myAction = {
       *                   text   : 'Cool action',
       *                   icon   : 'b-fa b-fa-fw b-fa-ban',
       *                   onItem : () => console.log('Some coolness'),
       *                   weight : 300 // Move to end
       *               };
       *
       *               // Hide column picker
       *               items.columnPicker.hidden = true;
       *           }
       *       }
       *   },
       * ```
       * @param {Object} context An object with information about the menu being shown
       * @param {Grid.column.Column} context.column The current column
       * @param {Object<String,MenuItemConfig>} context.items An object containing the
       * {@link Core.widget.MenuItem menu item} configs keyed by their id
       * @param {Event} context.event The DOM event object that triggered the show
       * @config {Function}
       * @preventable
       */
      processItems: null
    };
  }
  static get pluginConfig() {
    const config = super.pluginConfig;
    config.chain.push('populateHeaderMenu');
    return config;
  }
  //endregion
  //region Events
  /**
   * This event fires on the owning Grid before the context menu is shown for a header.
   * Allows manipulation of the items to show in the same way as in the {@link #config-processItems}.
   *
   * Returning `false` from a listener prevents the menu from being shown.
   *
   * @event headerMenuBeforeShow
   * @on-owner
   * @preventable
   * @param {Grid.view.Grid} source The grid
   * @param {Core.widget.Menu} menu The menu
   * @param {Object<String,MenuItemConfig>} items Menu item configs
   * @param {Grid.column.Column} column Column
   */
  /**
   * This event fires on the owning Grid after the context menu is shown for a header
   * @event headerMenuShow
   * @on-owner
   * @param {Grid.view.Grid} source The grid
   * @param {Core.widget.Menu} menu The menu
   * @param {Object<String,MenuItemConfig>} items Menu item configs
   * @param {Grid.column.Column} column Column
   */
  /**
   * This event fires on the owning Grid when an item is selected in the header context menu.
   * @event headerMenuItem
   * @on-owner
   * @param {Grid.view.Grid} source The grid
   * @param {Core.widget.Menu} menu The menu
   * @param {Core.widget.MenuItem} item Selected menu item
   * @param {Grid.column.Column} column Column
   */
  /**
   * This event fires on the owning Grid when a check item is toggled in the header context menu.
   * @event headerMenuToggleItem
   * @on-owner
   * @param {Grid.view.Grid} source The grid
   * @param {Core.widget.Menu} menu The menu
   * @param {Core.widget.MenuItem} item Selected menu item
   * @param {Grid.column.Column} column Column
   * @param {Boolean} checked Checked or not
   */
  //endregion
  //region Menu handlers
  shouldShowMenu(eventParams) {
    const {
      column
    } = eventParams;
    return column && column.enableHeaderContextMenu !== false && column !== this.client.timeAxisColumn;
  }
  getDataFromEvent(event) {
    return ObjectHelper.assign(super.getDataFromEvent(event), this.client.getHeaderDataFromEvent(event));
  }
  populateHeaderMenu({
    items,
    column
  }) {
    const me = this;
    if (column) {
      if (column.headerMenuItems) {
        ObjectHelper.merge(items, column.headerMenuItems);
      }
      if (column.isCollapsible) {
        const {
            collapsed
          } = column,
          icon = collapsed ? me.client.rtl ? 'left' : 'right' : me.client.rtl ? 'right' : 'left';
        items.toggleCollapse = {
          weight: 215,
          icon: `b-fw-icon b-icon-collapse-${icon}`,
          text: me.L(collapsed ? 'L{expandColumn}' : 'L{collapseColumn}'),
          onItem: () => column.collapsed = !collapsed
        };
      }
      if (me.moveColumns) {
        const columnToMoveBefore = me.getColumnToMoveBefore(column),
          columnToMoveAfter = me.getColumnToMoveAfter(column);
        if (columnToMoveBefore) {
          items.movePrev = {
            weight: 220,
            icon: 'b-fw-icon b-icon-column-move-left',
            text: me.L('L{moveBefore}', StringHelper.encodeHtml(columnToMoveBefore.text)),
            onItem: () => {
              const {
                parent: oldParent
              } = column;
              // If the operation was successful, postprocess. Check for
              // parent being empty and set the new region.
              if (columnToMoveBefore.parent.insertChild(column, columnToMoveBefore)) {
                var _oldParent$children;
                column.region = columnToMoveBefore.region;
                // If we have removed the last child, remove the empty group.
                // Column#sealed may have vetoed the operation.
                if (!((_oldParent$children = oldParent.children) !== null && _oldParent$children !== void 0 && _oldParent$children.length)) {
                  oldParent.remove();
                }
              }
            }
          };
        }
        if (columnToMoveAfter) {
          items.moveNext = {
            weight: 230,
            icon: 'b-fw-icon b-icon-column-move-right',
            text: me.L('L{moveAfter}', StringHelper.encodeHtml(columnToMoveAfter.text)),
            onItem: () => {
              const {
                parent: oldParent
              } = column;
              // If the operation was successful, postprocess. Check for
              // parent being empty and set the new region.
              if (columnToMoveAfter.parent.insertChild(column, columnToMoveAfter.nextSibling)) {
                var _oldParent$children2;
                column.region = columnToMoveAfter.region;
                // If we have removed the last child, remove the empty group.
                // Column#sealed may have vetoed the operation.
                if (!((_oldParent$children2 = oldParent.children) !== null && _oldParent$children2 !== void 0 && _oldParent$children2.length)) {
                  oldParent.remove();
                }
              }
            }
          };
        }
      }
    }
    return items;
  }
  getColumnToMoveBefore(column) {
    const {
      previousSibling,
      parent
    } = column;
    if (previousSibling) {
      return previousSibling.children && !column.children ? previousSibling.children[previousSibling.children.length - 1] : previousSibling;
    }
    // Move to before parent
    if (!parent.isRoot) {
      return parent;
    }
  }
  getColumnToMoveAfter(column) {
    const {
      nextSibling,
      parent
    } = column;
    if (nextSibling) {
      return nextSibling;
    }
    // Move to before parent
    if (!parent.isRoot) {
      return parent;
    }
  }
}
HeaderMenu.featureClass = '';
HeaderMenu._$name = 'HeaderMenu';
GridFeatureManager.registerFeature(HeaderMenu, true);

/**
 * @module Grid/feature/Sort
 */
/**
 * Allows sorting of grid by clicking (or tapping) headers, also displays which columns grid is sorted by (numbered if
 * using multisort). Use modifier keys for multisorting: [Ctrl/CMD + click] to add sorter, [Ctrl/CMD + Alt + click] to remove sorter.
 * The actual sorting is done by the store, see {@link Core.data.mixin.StoreSort#function-sort Store.sort()}.
 *
 * {@inlineexample Grid/feature/Sort.js}
 *
 * ```javascript
 * // sorting feature is enabled, no default value though
 * const grid = new Grid({
 *     features : {
 *         sort : true
 *     }
 * });
 *
 * // use initial sorting
 * const grid = new Grid({
 *     features : {
 *         sort : 'name'
 *     }
 * });
 *
 * // can also be specified on the store
 * const grid = new Grid({
 *     store : {
 *         sorters : [
 *             { field : 'name', ascending : false }
 *         ]
 *     }
 * });
 *
 * // custom sorting function can also be specified on the store
 * const grid = new Grid({
 *     store : {
 *         sorters : [{
 *             fn : (recordA, recordB) => {
 *                 // apply custom logic, for example:
 *                 return recordA.name.length < recordB.name.length ? -1 : 1;
 *             }
 *         }]
 *     }
 * });
 * ```
 *
 * For info on programmatically handling sorting, see {@link Core.data.mixin.StoreSort StoreSort}:
 *
 * ```javascript
 * const grid = new Grid({ });
 * // Programmatic sorting of the store, Grids rows and UI will be updated
 * grid.store.sort('age');
 * ```
 *
 * Grid columns can define custom sorting functions (see {@link Grid.column.Column#config-sortable Column.sortable}).
 * If this feature is configured with `prioritizeColumns: true`, those functions will also be used when sorting
 * programmatically:
 *
 * ```javascript
 * const grid = new Grid({
 *     columns : [
 *         {
 *             field : 'age',
 *             text : 'Age',
 *             sortable(lhs, rhs) {
 *               // Custom sorting, see Array#sort
 *             }
 *         }
 *     ],
 *
 *     features : {
 *         sort : {
 *             prioritizeColumns : true
 *         }
 *     }
 * });
 *
 * // Sortable fn will also be used when sorting programmatically
 * grid.store.sort('age');
 * ```
 *
 * This feature is **enabled** by default.
 *
 * @extends Core/mixin/InstancePlugin
 * @demo Grid/sorting
 * @classtype sort
 * @feature
 */
class Sort extends InstancePlugin {
  //region Config
  static get $name() {
    return 'Sort';
  }
  static get configurable() {
    return {
      /**
       * Enable multi sort
       * @config {Boolean}
       * @default
       */
      multiSort: true,
      /**
       * Use custom sorting functions defined on columns also when programmatically sorting by the columns field.
       *
       * ```javascript
       * const grid = new Grid({
       *     columns : [
       *         {
       *             field : 'age',
       *             text : 'Age',
       *             sortable(lhs, rhs) {
       *               // Custom sorting, see Array#sort
       *             }
       *         }
       *     ],
       *
       *     features : {
       *         sort : {
       *             prioritizeColumns : true
       *         }
       *     }
       * });
       *
       * grid.store.sort('age');
       * ```
       *
       * @config {Boolean}
       * @default
       */
      prioritizeColumns: false,
      /**
       * By default, clicking anywhere on the header text toggles the sorting state of a column.
       *
       * Configure this as `false` to only toggle the sorting state of a column on click of the
       * "arrow" icon within the grid header.
       * @config {Boolean}
       * @default false
       */
      toggleOnHeaderClick: true
    };
  }
  static get properties() {
    return {
      ignoreRe: new RegExp([
      // Stop this feature from having to know the internals of two other optional features.
      'b-grid-header-resize-handle', 'b-filter-icon'].join('|')),
      sortableCls: 'b-sortable',
      sortedCls: 'b-sort',
      sortedAscCls: 'b-asc',
      sortedDescCls: 'b-desc'
    };
  }
  //endregion
  //region Init
  construct(grid, config) {
    // process initial config into an actual config object
    config = this.processConfig(config);
    this.grid = grid;
    this.bindStore(this.store);
    super.construct(grid, config);
  }
  // Sort feature handles special config cases, where user can supply a string or an array of sorters
  // instead of a normal config object
  processConfig(config) {
    if (typeof config === 'string' || Array.isArray(config)) {
      return {
        field: config,
        ascending: null
      };
    }
    return config;
  }
  // override setConfig to process config before applying it
  setConfig(config) {
    super.setConfig(this.processConfig(config));
  }
  bindStore(store) {
    this.detachListeners('store');
    store.ion({
      name: 'store',
      beforeSort: 'onStoreBeforeSort',
      sort: 'syncHeaderSortState',
      thisObj: this
    });
  }
  set field(field) {
    // Use columns sortable config for initial sorting if it is specified
    const column = this.grid.columns.get(field);
    if (column && typeof column.sortable === 'object') {
      // Normalization of Store & CollectionSorter differences
      column.sortable.field = column.sortable.property || field;
      field = column.sortable;
    }
    this.store.sort(field, this.ascending);
  }
  // Avoid caching store, it might change
  get store() {
    return this.grid[this.grid.sortFeatureStore];
  }
  //endregion
  //region Plugin config
  // Plugin configuration. This plugin chains some of the functions in Grid.
  static get pluginConfig() {
    return {
      chain: ['onElementClick', 'populateHeaderMenu', 'getColumnDragToolbarItems', 'renderHeader', 'onPaint', 'bindStore']
    };
  }
  //endregion
  //region Headers
  /**
   * Update headers to match stores sorters (displays sort icon in correct direction on them)
   * @private
   */
  syncHeaderSortState() {
    const me = this,
      {
        grid
      } = me;
    if (!grid.hideHeaders && grid.isPainted) {
      const storeSorters = me.store.sorters,
        sorterCount = storeSorters.length,
        classList = new DomClassList();
      let sorter;
      // Sync the sortable, sorted, and sortIndex state of each leaf header element
      for (const leafColumn of grid.columns.visibleColumns) {
        if (!leafColumn.sortable) {
          continue;
        }
        const leafHeader = leafColumn.element,
          textEl = leafColumn.textWrapper,
          // TimeAxisColumn in Scheduler has no textWrapper, since it has custom rendering,
          // but since it cannot be sorted by anyway lets just ignore it
          dataset = textEl === null || textEl === void 0 ? void 0 : textEl.dataset;
        let sortDirection = 'none';
        // data-sortIndex is 1-based, and only set if there is > 1 sorter.
        // iOS Safari throws a JS error if the requested delete property is not present.
        (dataset === null || dataset === void 0 ? void 0 : dataset.sortIndex) && delete dataset.sortIndex;
        classList.value = leafHeader.classList;
        if (leafColumn.sortable === false) {
          var _textEl$querySelector;
          classList.remove(me.sortableCls);
          textEl === null || textEl === void 0 ? void 0 : (_textEl$querySelector = textEl.querySelector('.b-sort-icon')) === null || _textEl$querySelector === void 0 ? void 0 : _textEl$querySelector.remove();
        } else {
          if (!(textEl !== null && textEl !== void 0 && textEl.querySelector('.b-sort-icon'))) {
            DomHelper.createElement({
              parent: textEl,
              className: 'b-sort-icon'
            });
          }
          classList.add(me.sortableCls);
          sorter = storeSorters.find(sort => sort.field === leafColumn.field || sort.sortFn && sort.sortFn === leafColumn.sortable.sortFn);
          if (sorter) {
            if (sorterCount > 1 && dataset) {
              dataset.sortIndex = storeSorters.indexOf(sorter) + 1;
            }
            classList.add(me.sortedCls);
            if (sorter.ascending) {
              classList.add(me.sortedAscCls);
              classList.remove(me.sortedDescCls);
              sortDirection = 'ascending';
            } else {
              classList.add(me.sortedDescCls);
              classList.remove(me.sortedAscCls);
              sortDirection = 'descending';
            }
          } else {
            classList.remove(me.sortedCls);
            // Not optimal, but easiest way to make sure sort feature does not remove needed classes.
            // Better solution would be to use different names for sorting and grouping
            if (!classList['b-group']) {
              classList.remove(me.sortedAscCls);
              classList.remove(me.sortedDescCls);
            }
          }
        }
        // Update the element's classList
        DomHelper.syncClassList(leafHeader, classList);
        DomHelper.setAttributes(leafHeader, {
          'aria-sort': sortDirection
        });
      }
    }
  }
  //endregion
  //region Context menu
  /**
   * Adds sort menu items to header context menu.
   * @param {Object} options Contains menu items and extra data retrieved from the menu target.
   * @param {Grid.column.Column} options.column Column for which the menu will be shown
   * @param {Object<String,MenuItemConfig|Boolean|null>} options.items A named object to describe menu items
   * @internal
   */
  populateHeaderMenu({
    column,
    items
  }) {
    const me = this,
      {
        store
      } = me,
      sortBy = {
        ...column.sortable,
        field: column.field,
        columnOwned: true
      };
    if (column.sortable !== false) {
      items.sortAsc = {
        text: 'L{sortAscending}',
        localeClass: me,
        icon: 'b-fw-icon b-icon-sort-asc',
        cls: 'b-separator',
        weight: 300,
        disabled: me.disabled,
        onItem: () => store.sort(sortBy, true)
      };
      items.sortDesc = {
        text: 'L{sortDescending}',
        localeClass: me,
        icon: 'b-fw-icon b-icon-sort-desc',
        weight: 310,
        disabled: me.disabled,
        onItem: () => store.sort(sortBy, false)
      };
      if (me.multiSort && me.grid.columns.records.some(col => col.sortable)) {
        const sorter = store.sorters.find(s => s.field === column.field || column.sortable.sortFn && column.sortable.sortFn === s.sortFn);
        items.multiSort = {
          text: 'L{multiSort}',
          localeClass: me,
          icon: 'b-fw-icon b-icon-sort',
          weight: 320,
          disabled: me.disabled,
          menu: {
            addSortAsc: {
              text: sorter ? 'L{toggleSortAscending}' : 'L{addSortAscending}',
              localeClass: me,
              icon: 'b-fw-icon b-icon-sort-asc',
              disabled: sorter && (sorter === null || sorter === void 0 ? void 0 : sorter.ascending),
              weight: 330,
              onItem: () => store.addSorter(sortBy, true)
            },
            addSortDesc: {
              text: sorter ? 'L{toggleSortDescending}' : 'L{addSortDescending}',
              localeClass: me,
              icon: 'b-fw-icon b-icon-sort-desc',
              disabled: sorter && !sorter.ascending,
              weight: 340,
              onItem: () => store.addSorter(sortBy, false)
            },
            removeSorter: {
              text: 'L{removeSorter}',
              localeClass: me,
              icon: 'b-fw-icon b-icon-remove',
              weight: 350,
              disabled: !sorter,
              onItem: () => {
                store.removeSorter(sortBy.field);
              }
            }
          }
        };
      }
    }
  }
  /**
   * Supply items to ColumnDragToolbar
   * @private
   */
  getColumnDragToolbarItems(column, items) {
    const me = this,
      {
        store,
        disabled
      } = me;
    if (column.sortable !== false) {
      const sorter = store.sorters.find(s => s.field === column.field);
      items.push({
        text: 'L{sortAscendingShort}',
        group: 'L{sort}',
        localeClass: me,
        icon: 'b-icon b-icon-sort-asc',
        ref: 'sortAsc',
        cls: 'b-separator',
        weight: 105,
        disabled,
        onDrop: ({
          column
        }) => store.sort(column.field, true)
      }, {
        text: 'L{sortDescendingShort}',
        group: 'L{sort}',
        localeClass: me,
        icon: 'b-icon b-icon-sort-desc',
        ref: 'sortDesc',
        weight: 105,
        disabled,
        onDrop: ({
          column
        }) => store.sort(column.field, false)
      }, {
        text: 'L{addSortAscendingShort}',
        group: 'L{multiSort}',
        localeClass: me,
        icon: 'b-icon b-icon-sort-asc',
        ref: 'multisortAddAsc',
        disabled: disabled || sorter && sorter.ascending,
        weight: 105,
        onDrop: ({
          column
        }) => store.addSorter(column.field, true)
      }, {
        text: 'L{addSortDescendingShort}',
        group: 'L{multiSort}',
        localeClass: me,
        icon: 'b-icon b-icon-sort-desc',
        ref: 'multisortAddDesc',
        disabled: disabled || sorter && !sorter.ascending,
        weight: 105,
        onDrop: ({
          column
        }) => store.addSorter(column.field, false)
      }, {
        text: 'L{removeSorterShort}',
        group: 'L{multiSort}',
        localeClass: me,
        icon: 'b-icon b-icon-remove',
        ref: 'multisortRemove',
        weight: 105,
        disabled: disabled || !sorter,
        onDrop: ({
          column
        }) => store.removeSorter(column.field)
      });
    }
    return items;
  }
  //endregion
  //region Events
  // Intercept sorting by a column that has a custom sorting fn, and inject that fn
  onStoreBeforeSort({
    sorters
  }) {
    const {
      columns
    } = this.client;
    for (let i = 0; i < sorters.length; i++) {
      var _column$sortable;
      const sorter = sorters[i],
        column = (sorter.columnOwned || this.prioritizeColumns) && columns.get(sorter.field);
      if (column !== null && column !== void 0 && (_column$sortable = column.sortable) !== null && _column$sortable !== void 0 && _column$sortable.sortFn) {
        sorters[i] = {
          ...sorter,
          ...column.sortable,
          columnOwned: true
        };
      }
    }
  }
  /**
   * Clicked on header, sort Store.
   * @private
   */
  onElementClick(event) {
    const me = this,
      {
        store
      } = me,
      {
        target
      } = event,
      header = target.closest('.b-grid-header.b-sortable'),
      field = header === null || header === void 0 ? void 0 : header.dataset.column;
    if (me.ignoreRe.test(target.className) || me.disabled || event.handled) {
      return;
    }
    //Header
    if (header && field && (me.toggleOnHeaderClick || target.closest('.b-sort-icon'))) {
      const column = me.grid.columns.getById(header.dataset.columnId),
        columnGrouper = store.isGrouped && store.groupers.find(g => g.field === field);
      // The Group feature will handle the change of the grouper's direction
      if (columnGrouper && !event.shiftKey) {
        return;
      }
      if (column.sortable && !event.shiftKey) {
        if (event.ctrlKey && event.altKey) {
          store.removeSorter(column.field);
        } else {
          const sortBy = {
            columnOwned: true,
            field: column.field
          };
          // sortable as a function is handled by onStoreBeforeSort() above
          if (typeof column.sortable === 'object') {
            ObjectHelper.assign(sortBy, column.sortable);
          }
          store.sort(sortBy, null, event.ctrlKey);
        }
      }
    }
  }
  /**
   * Called when grid headers are rendered, make headers match current sorters.
   * @private
   */
  renderHeader() {
    this.syncHeaderSortState();
  }
  onPaint() {
    this.syncHeaderSortState();
  }
  //endregion
}

Sort.featureClass = 'b-sort';
Sort._$name = 'Sort';
GridFeatureManager.registerFeature(Sort, true);

/**
 * @module Grid/feature/Stripe
 */
/**
 * Stripes rows by adding alternating CSS classes to all row elements (`b-even` and `b-odd`).
 *
 * This feature is <strong>disabled</strong> by default.
 *
 * @extends Core/mixin/InstancePlugin
 *
 * @example
 * let grid = new Grid({
 *   features: {
 *     stripe: true
 *   }
 * });
 *
 * @demo Grid/columns
 * @classtype stripe
 * @inlineexample Grid/feature/Stripe.js
 * @feature
 */
class Stripe extends InstancePlugin {
  static get $name() {
    return 'Stripe';
  }
  construct(grid, config) {
    super.construct(grid, config);
    grid.ion({
      renderrow: 'onRenderRow',
      thisObj: this
    });
  }
  doDisable(disable) {
    if (!this.isConfiguring) {
      // Refresh rows to add/remove even/odd classes
      this.client.refreshRows();
    }
    super.doDisable(disable);
  }
  /**
   * Applies even/odd CSS when row is rendered
   * @param {Grid.row.Row} rowModel
   * @private
   */
  onRenderRow({
    row
  }) {
    const {
        disabled
      } = this,
      even = row.dataIndex % 2 === 0;
    row.assignCls({
      'b-even': !disabled && even,
      'b-odd': !disabled && !even
    });
  }
}
Stripe._$name = 'Stripe';
GridFeatureManager.registerFeature(Stripe);

/**
 * @module Grid/row/Row
 */
const cellContentRange = document.createRange();
/**
 * Represents a single rendered row in the grid. Consists of one row element for each SubGrid in use. The grid only
 * creates as many rows as needed to fill the current viewport (and a buffer). As the grid scrolls
 * the rows are repositioned and reused, there is not a one-to-one relation between rows and records.
 *
 * For normal use cases you should not have to use this class directly. Rely on using renderers instead.
 * @extends Core/Base
 */
class Row extends Base {
  static $name = 'Row';
  static get configurable() {
    return {
      /**
       * When __read__, this a {@link Core.helper.util.DomClassList} of class names to be
       * applied to this Row's elements.
       *
       * It can be __set__ using Object notation where each property name with a truthy value is added as
       * a class, or as a regular space-separated string.
       *
       * @member {Core.helper.util.DomClassList} cls
       * @accepts {Core.helper.util.DomClassList|Object<String,Boolean|Number>}
       */
      /**
       * The class name to initially add to all row elements
       * @config {String|Core.helper.util.DomClassList|Object<String,Boolean|Number>}
       */
      cls: {
        $config: {
          equal: (c1, c2) => (c1 === null || c1 === void 0 ? void 0 : c1.isDomClassList) && (c2 === null || c2 === void 0 ? void 0 : c2.isDomClassList) && c1.isEqual(c2)
        }
      }
    };
  }
  //region Init
  /**
   * Constructs a Row setting its index.
   * @param {Object} config A configuration object which must contain the following two properties:
   * @param {Grid.view.Grid} config.grid The owning Grid.
   * @param {Grid.row.RowManager} config.rowManager The owning RowManager.
   * @param {Number} config.index The index of the row within the RowManager's cache.
   * @function constructor
   * @internal
   */
  construct(config) {
    // Set up defaults and properties
    Object.assign(this, {
      _elements: {},
      _elementsArray: [],
      _cells: {},
      _allCells: [],
      _regions: [],
      lastHeight: 0,
      lastTop: -1,
      _dataIndex: 0,
      _top: 0,
      _height: 0,
      _id: null,
      forceInnerHTML: false,
      isGroupFooter: false,
      // Create our cell rendering context
      cellContext: new Location({
        grid: config.grid,
        id: null,
        columnIndex: 0
      })
    });
    super.construct(config);
    // For performance, the element translation method is set at Row consruct time.
    // The default uses transform : translate(), it can be overridden if rows need
    // to be positioned using layout, such as when sticky elements are used in cells.
    if (this.grid.positionMode === 'position') {
      this.translateElements = this.positionElements;
    }
  }
  doDestroy() {
    const me = this;
    // No need to clean elements up if the entire thing is being destroyed
    if (!me.rowManager.isDestroying) {
      me.removeElements();
      if (me.rowManager.idMap[me.id] === me) {
        delete me.rowManager.idMap[me.id];
      }
    }
    super.doDestroy();
  }
  //endregion
  //region Data getters/setters
  /**
   * Get index in RowManagers rows array
   * @property {Number}
   * @readonly
   */
  get index() {
    return this._index;
  }
  set index(index) {
    this._index = index;
  }
  /**
   * Get/set this rows current index in grids store
   * @property {Number}
   */
  get dataIndex() {
    return this._dataIndex;
  }
  set dataIndex(dataIndex) {
    if (this._dataIndex !== dataIndex) {
      this._dataIndex = dataIndex;
      this.eachElement(element => {
        element.dataset.index = dataIndex;
        element.ariaRowIndex = this.grid.hideHeaders ? dataIndex + 1 : dataIndex + 2;
      });
    }
  }
  /**
   * Get/set id for currently rendered record
   * @property {String|Number}
   */
  get id() {
    return this._id;
  }
  set id(id) {
    const me = this,
      idObj = {
        id
      },
      idMap = me.rowManager.idMap;
    if (me._id !== id || idMap[id] !== me) {
      if (idMap[me._id] === me) delete idMap[me._id];
      idMap[id] = me;
      me._id = id;
      me.eachElement(element => {
        DomDataStore.assign(element, idObj);
        element.dataset.id = id;
      });
      me.eachCell(cell => DomDataStore.assign(cell, idObj));
    }
  }
  //endregion
  //region Row elements
  /**
   * Add a row element for specified region.
   * @param {String} region Region to add element for
   * @param {HTMLElement} element Element
   * @private
   */
  addElement(region, element) {
    const me = this;
    let cellElement = element.firstElementChild;
    me._elements[region] = element;
    me._elementsArray.push(element);
    me._regions.push(region);
    DomDataStore.assign(element, {
      index: me.index
    });
    me._cells[region] = [];
    while (cellElement) {
      me._cells[region].push(cellElement);
      me._allCells.push(cellElement);
      DomDataStore.set(cellElement, {
        column: cellElement.dataset.column,
        columnId: cellElement.dataset.columnId,
        rowElement: cellElement.parentNode,
        row: me
      });
      cellElement = cellElement.nextElementSibling;
    }
    // making css selectors simpler, dataset has bad performance but it is only set once and never read
    element.dataset.index = me.index;
    element.ariaRowIndex = me.grid.hideHeaders ? me.index + 1 : me.index + 2;
  }
  /**
   * Get the element for the specified region.
   * @param {String} region
   * @returns {HTMLElement}
   */
  getElement(region) {
    return this._elements[region];
  }
  /**
   * Get the {@link Core.helper.util.Rectangle element bounds} for the specified region of this Row.
   * @param {String} region
   * @returns {Core.helper.util.Rectangle}
   */
  getRectangle(region) {
    return Rectangle.from(this.getElement(region));
  }
  /**
   * Execute supplied function for each regions element.
   * @param {Function} fn
   */
  eachElement(fn) {
    this._elementsArray.forEach(fn);
  }
  /**
   * Execute supplied function for each cell.
   * @param {Function} fn
   */
  eachCell(fn) {
    this._allCells.forEach(fn);
  }
  /**
   * An object, keyed by region name (for example `locked` and `normal`) containing the elements which comprise the full row.
   * @type {Object<String,HTMLElement>}
   * @readonly
   */
  get elements() {
    return this._elements;
  }
  /**
   * The row element, only applicable when not using multiple grid sections (see {@link #property-elements})
   * @type {HTMLElement}
   * @readonly
   */
  get element() {
    const region = Object.keys(this._elements)[0];
    return this._elements[region];
  }
  //endregion
  //region Cell elements
  /**
   * Row cell elements
   * @property {HTMLElement[]}
   * @readonly
   */
  get cells() {
    return this._allCells;
  }
  /**
   * Get cell elements for specified region.
   * @param {String} region Region to get elements for
   * @returns {HTMLElement[]} Array of cell elements
   */
  getCells(region) {
    return this._cells[region];
  }
  /**
   * Get the cell element for the specified column.
   * @param {String|Number} columnId Column id
   * @returns {HTMLElement} Cell element
   */
  getCell(columnId, strict = false) {
    return this._allCells.find(cell => {
      const cellData = DomDataStore.get(cell);
      // cellData will always have String type, use == to handle a column with Number type
      return cellData.columnId == columnId || !strict && cellData.column == columnId;
    });
  }
  removeElements(onlyRelease = false) {
    const me = this;
    // Triggered before the actual remove to allow cleaning up elements etc.
    me.rowManager.trigger('removeRow', {
      row: me
    });
    if (!onlyRelease) {
      me.eachElement(element => element.remove());
    }
    me._elements = {};
    me._cells = {};
    me._elementsArray.length = me._regions.length = me._allCells.length = me.lastHeight = me.height = 0;
    me.lastTop = -1;
  }
  //endregion
  //region Height
  /**
   * Get/set row height
   * @property {Number}
   */
  get height() {
    return this._height;
  }
  set height(height) {
    this._height = height;
  }
  /**
   * Get row height including border
   * @property {Number}
   */
  get offsetHeight() {
    // me.height is specified height, add border height to it to get cells height to match specified rowHeight
    // border height is measured in Grid#get rowManager
    return this.height + this.grid._rowBorderHeight;
  }
  /**
   * Sync elements height to rows height
   * @private
   */
  updateElementsHeight(isExport) {
    const me = this;
    if (!isExport) {
      me.rowManager.storeKnownHeight(me.id, me.height);
    }
    // prevent unnecessary style updates
    if (me.lastHeight !== me.height) {
      this.eachElement(element => element.style.height = `${me.offsetHeight}px`);
      me.lastHeight = me.height;
    }
  }
  //endregion
  //region CSS
  /**
   * Add CSS classes to each element.
   * @param {...String|Object<String,Boolean|Number>|Core.helper.util.DomClassList} classes
   */
  addCls(classes) {
    this.updateCls(this.cls.add(classes));
  }
  /**
   * Remove CSS classes from each element.
   * @param {...String|Object<String,Boolean|Number>|Core.helper.util.DomClassList} classes
   */
  removeCls(classes) {
    this.updateCls(this.cls.remove(classes));
  }
  /**
   * Toggle CSS classes for each element.
   * @param {Object<String,Boolean|Number>|Core.helper.util.DomClassList|...String} classes
   * @param {Boolean} add
   * @internal
   */
  toggleCls(classes, add) {
    this.updateCls(this.cls[add ? 'add' : 'remove'](classes));
  }
  /**
   * Adds/removes class names according to the passed object's properties.
   *
   * Properties with truthy values are added.
   * Properties with false values are removed.
   * @param {Object<String,Boolean|Number>} classes Object containing properties to set/clear
   */
  assignCls(classes) {
    this.updateCls(this.cls.assign(classes));
  }
  changeCls(cls) {
    return cls !== null && cls !== void 0 && cls.isDomClassList ? cls : new DomClassList(cls);
  }
  updateCls(cls) {
    this.eachElement(element => DomHelper.syncClassList(element, cls));
  }
  setAttribute(attribute, value) {
    this.eachElement(element => element.setAttribute(attribute, value));
  }
  removeAttribute(attribute) {
    this.eachElement(element => element.removeAttribute(attribute));
  }
  //endregion
  //region Position
  /**
   * Is this the very first row?
   * @property {Boolean}
   * @readonly
   */
  get isFirst() {
    return this.dataIndex === 0;
  }
  /**
   * Row top coordinate
   * @property {Number}
   * @readonly
   */
  get top() {
    return this._top;
  }
  /**
   * Row bottom coordinate
   * @property {Number}
   * @readonly
   */
  get bottom() {
    return this._top + this._height + this.grid._rowBorderHeight;
  }
  /**
   * Sets top coordinate, translating elements position.
   * @param {Number} top Top coordinate
   * @param {Boolean} [silent] Specify `true` to not trigger translation event
   * @internal
   */
  setTop(top, silent) {
    if (this._top !== top) {
      this._top = top;
      this.translateElements(silent);
    }
  }
  /**
   * Sets bottom coordinate, translating elements position.
   * @param {Number} bottom Bottom coordinate
   * @param {Boolean} [silent] Specify `true` to not trigger translation event
   * @private
   */
  setBottom(bottom, silent) {
    this.setTop(bottom - this.offsetHeight, silent);
  }
  // Used by export feature to position individual row
  translate(top, silent = false) {
    this.setTop(top, silent);
    return top + this.offsetHeight;
  }
  /**
   * Sets css transform to position elements at correct top position (translateY)
   * @private
   */
  translateElements(silent) {
    const me = this,
      {
        top,
        _elementsArray
      } = me;
    if (me.lastTop !== top) {
      for (let i = 0, {
          length
        } = _elementsArray; i < length; i++) {
        _elementsArray[i].style.transform = `translate(0,${top}px)`;
      }
      !silent && me.rowManager.trigger('translateRow', {
        row: me
      });
      me.lastTop = top;
    }
  }
  /**
   * Sets css top to position elements at correct top position
   * @private
   */
  positionElements(silent) {
    const me = this,
      {
        top,
        _elementsArray
      } = me;
    if (me.lastTop !== top) {
      for (let i = 0, {
          length
        } = _elementsArray; i < length; i++) {
        _elementsArray[i].style.top = `${top}px`;
      }
      !silent && me.rowManager.trigger('translateRow', {
        row: me
      });
      me.lastTop = top;
    }
  }
  /**
   * Moves all row elements up or down and updates model.
   * @param {Number} offsetTop Pixels to offset the elements
   * @private
   */
  offset(offsetTop) {
    let newTop = this._top + offsetTop;
    // Not allowed to go below zero (won't be reachable on scroll in that case)
    if (newTop < 0) {
      offsetTop -= newTop;
      newTop = 0;
    }
    this.setTop(newTop);
    return offsetTop;
  }
  //endregion
  //region Render
  /**
   * Renders a record into this rows elements (trigger event that subgrids catch to do the actual rendering).
   * @param {Number} recordIndex
   * @param {Core.data.Model} record
   * @param {Boolean} [updatingSingleRow]
   * @param {Boolean} [batch]
   * @private
   */
  render(recordIndex, record, updatingSingleRow = true, batch = false, isExport = false) {
    var _record, _record2;
    const me = this,
      {
        cellContext,
        cls,
        elements,
        grid,
        rowManager,
        height: oldHeight,
        _id: oldId
      } = me,
      rowElData = DomDataStore.get(me._elementsArray[0]),
      rowHeight = rowManager._rowHeight,
      {
        store
      } = grid,
      {
        isTree
      } = store;
    let i = 0,
      size;
    // no record specified, try looking up in store (false indicates empty row, don't do lookup)
    if (!record && record !== false) {
      record = grid.store.getById(rowElData.id);
      recordIndex = grid.store.indexOf(record);
    }
    // Bail out if record is not resolved
    if (!record) {
      return;
    }
    // Now we have acquired a record, see what classes it requires on the
    const rCls = (_record = record) === null || _record === void 0 ? void 0 : _record.cls,
      recordCls = rCls ? rCls.isDomClassList ? rCls : new DomClassList(rCls) : null;
    cls.assign({
      // do not put updating class if we're exporting the row
      'b-grid-row-updating': updatingSingleRow && grid.transitionDuration && !isExport,
      'b-selected': grid.isSelected((_record2 = record) === null || _record2 === void 0 ? void 0 : _record2.id),
      'b-readonly': record.readOnly,
      'b-linked': record.isLinked,
      'b-original': record.hasLinks
    });
    // These are DomClassLists, so they have to have their properties processed by add/remove
    if (me.lastRecordCls) {
      cls.remove(me.lastRecordCls);
    }
    // Assign our record's cls to the row, and cache the value so it can be removed next time round
    if (recordCls) {
      cls.add(recordCls);
      me.lastRecordCls = Object.assign({}, recordCls);
    } else {
      me.lastRecordCls = null;
    }
    // used by GroupSummary feature to clear row before
    rowManager.trigger('beforeRenderRow', {
      row: me,
      record,
      recordIndex,
      oldId
    });
    grid.beforeRenderRow({
      row: me,
      record,
      recordIndex,
      oldId
    });
    // Flush any changes to our DomClassList to the Row's DOM
    me.updateCls(cls);
    if (updatingSingleRow && grid.transitionDuration && !isExport) {
      grid.setTimeout(() => {
        if (!me.isDestroyed) {
          cls.remove('b-grid-row-updating');
          me.updateCls(cls);
        }
      }, grid.transitionDuration);
    }
    me.id = record.id;
    me.dataIndex = recordIndex;
    // Configured height, used as row height if renderers do not specify otherwise
    const height = !grid.fixedRowHeight && grid.getRowHeight(record) || rowHeight;
    // Max height returned by renderers
    let maxRequestedHeight = me.maxRequestedHeight = null;
    // Keep ARIA ownership up to date
    if (isTree) {
      for (const region in elements) {
        const el = elements[region];
        el.id = `${grid.id}-${region}-${me.id}`;
        DomHelper.setAttributes(el, {
          'aria-level': record.childLevel + 1,
          'aria-setsize': record.parent.children.length,
          'aria-posinset': record.parentIndex + 1
        });
        if (record.isExpanded(store)) {
          var _record$children, _record$children2;
          DomHelper.setAttributes(el, {
            'aria-expanded': true,
            // A branch node may be configured expanded, but yet have no children.
            // They may be added dynamically.
            'aria-owns': (_record$children = record.children) !== null && _record$children !== void 0 && _record$children.length ? (_record$children2 = record.children) === null || _record$children2 === void 0 ? void 0 : _record$children2.map(r => `${grid.id}-${region}-${r.id}`).join(' ') : null
          });
        } else {
          if (record.isLeaf) {
            el.removeAttribute('aria-expanded');
          } else {
            el.setAttribute('aria-expanded', false);
          }
          el.removeAttribute('aria-owns');
        }
      }
    }
    cellContext._record = record;
    cellContext._id = record.id;
    cellContext._rowIndex = recordIndex;
    for (i = 0; i < grid.columns.visibleColumns.length; i++) {
      const column = grid.columns.visibleColumns[i];
      cellContext._columnId = column.id;
      cellContext._column = column;
      cellContext._columnIndex = i;
      cellContext._cell = me.getCell(column.id, true);
      cellContext.height = height;
      cellContext.maxRequestedHeight = maxRequestedHeight;
      cellContext.updatingSingleRow = updatingSingleRow;
      size = me.renderCell(cellContext);
      if (!rowManager.fixedRowHeight) {
        // We want to make row in all regions as tall as the tallest cell
        if (size.height != null) {
          maxRequestedHeight = Math.max(maxRequestedHeight, size.height);
          // Do not store a max height set by schedulers rendering, it has to base its layouts on the
          // original row height / that returned by other cells
          if (!size.transient) {
            me.maxRequestedHeight = maxRequestedHeight;
          }
        }
      }
    }
    const useHeight = maxRequestedHeight ?? height;
    me.height = grid.processRowHeight(record, useHeight) ?? useHeight;
    // Height gets set during render, reflect on elements
    me.updateElementsHeight(isExport);
    // Rerendering a row might change its height, which forces translation of all following rows
    if (updatingSingleRow && !isExport) {
      if (oldHeight !== me.height) {
        rowManager.translateFromRow(me, batch);
      }
      rowManager.trigger('updateRow', {
        row: me,
        record,
        recordIndex,
        oldId
      });
      rowManager.trigger('renderDone');
    }
    grid.afterRenderRow({
      row: me,
      record,
      recordIndex,
      oldId,
      oldHeight,
      isExport
    });
    rowManager.trigger('renderRow', {
      row: me,
      record,
      recordIndex,
      oldId,
      isExport
    });
    if (oldHeight && me.height !== oldHeight) {
      rowManager.trigger('rowRowHeight', {
        row: me,
        record,
        height: me.height,
        oldHeight
      });
    }
    me.forceInnerHTML = false;
  }
  /**
   * Renders a single cell, calling features to allow them to hook
   * @param {Grid.util.Location|HTMLElement} cellContext A {@link Grid.util.Location} which contains rendering
   * options, or a cell element which can be used to initialize a {@link Grid.util.Location}
   * @param {Number} [cellContext.height] Configured row height
   * @param {Number} [cellContext.maxRequestedHeight] Maximum proposed row height from renderers
   * @param {Boolean} [cellContext.updatingSingleRow] Rendered as part of updating a single row
   * @param {Boolean} [cellContext.isMeasuring] Rendered as part of a measuring operation
   * @internal
   */
  renderCell(cellContext) {
    var _grid$features, _grid$hasFrameworkRen;
    if (!cellContext.isLocation) {
      cellContext = new Location(cellContext);
    }
    let {
      cell: cellElement,
      record
    } = cellContext;
    const me = this,
      {
        grid,
        column,
        height,
        maxRequestedHeight,
        updatingSingleRow = true,
        isMeasuring = false
      } = cellContext,
      cellEdit = (_grid$features = grid.features) === null || _grid$features === void 0 ? void 0 : _grid$features.cellEdit,
      cellElementData = DomDataStore.get(cellElement),
      rowElement = cellElementData.rowElement,
      rowElementData = DomDataStore.get(rowElement);
    if (!record) {
      record = cellContext._record = grid.store.getById(rowElementData.id);
      if (!record) {
        return;
      }
    }
    let cellContent = column.getRawValue(record);
    const dataField = record.fieldMap[column.field],
      size = {
        configuredHeight: height,
        height: null,
        maxRequestedHeight
      },
      cellCls = column.getCellClass(cellContext),
      rendererData = {
        cellElement,
        dataField,
        rowElement,
        value: cellContent,
        record,
        column,
        size,
        grid,
        row: cellElementData.row,
        updatingSingleRow,
        isMeasuring
      },
      useRenderer = column.renderer || column.defaultRenderer;
    // Hook to allow processing cell before render, used by QuickFind & MergeCells
    grid.beforeRenderCell(rendererData);
    // Allow hook to redirect cell output
    if (rendererData.cellElement !== cellElement) {
      // Render to redirected target
      cellElement = rendererData.cellElement;
    }
    DomHelper.syncClassList(cellElement, cellCls);
    let shouldSetContent = true;
    // By default, `cellContent` is raw value extracted from Record based on Column field.
    // Call `renderer` if present, otherwise set innerHTML directly.
    if (useRenderer) {
      // `cellContent` could be anything here:
      // - null
      // - undefined when nothing is returned, used when column modifies cell content, for example Widget column
      // - number as cell value, to be converted to string
      // - string as cell value
      // - string which contains custom DOM element which is handled by Angular after we render it as cell value
      // - object with special $$typeof property equals to Symbol(react.element) handled by React when JSX is returned
      // - object which has no special properties but understood by Vue because the column is marked as "Vue" column
      // - object that should be passed to the `DomSync.sync` to update the cell content
      cellContent = column.callback(useRenderer, column, [rendererData]);
      if (cellContent === undefined && column.alwaysClearCell === false) {
        shouldSetContent = false;
      }
    } else if (dataField) {
      cellContent = dataField.print(cellContent);
    }
    // Check if the cell content is going to be rendered by framework
    const hasFrameworkRenderer = (_grid$hasFrameworkRen = grid.hasFrameworkRenderer) === null || _grid$hasFrameworkRen === void 0 ? void 0 : _grid$hasFrameworkRen.call(grid, {
      cellContent,
      column
    });
    // This is exceptional case, using framework rendering while grouping is not supported.
    // Need to reset the content in case of JSX is returned from the renderer.
    // Normally, if a renderer returns some content, the Grouping feature will overwrite it with the grouped value.
    // But useRenderer cannot be ignored completely, since a column might want to render additional content to the
    // grouped row. For example, Action Column may render an action button the grouped row.
    if (hasFrameworkRenderer && record.isSpecialRow) {
      cellContent = '';
    }
    // If present, framework may decide if it wants our renderer to prerender the cell content or not.
    // In case of normal cells in flat grids, React and Vue perform the full rendering into the root cell element.
    // But in case of tree cell in tree grids, React and Vue require our renderer to prerender internals,
    // and they perform rendering into inner "b-tree-cell-value" element. This way we can see our expand controls,
    // bullets, etc.
    const frameworkPerformsFullRendering = hasFrameworkRenderer && !column.data.tree && !record.isSpecialRow;
    // `shouldSetContent` false means content is already set by the column (i.e. Widget column).
    // `frameworkPerformsFullRendering` true means full cell content is set by framework renderer.
    if (shouldSetContent && !frameworkPerformsFullRendering) {
      var _cellEdit$editorConte;
      let renderTarget = cellElement;
      // If the cell is being edited, we render to a separate div and carefully
      // insert the contents into a Range which excludes the editor.
      if (cellEdit !== null && cellEdit !== void 0 && (_cellEdit$editorConte = cellEdit.editorContext) !== null && _cellEdit$editorConte !== void 0 && _cellEdit$editorConte.equals(cellContext) && !cellEdit.editor.isFinishing) {
        renderTarget = me.moveContentFromCell(cellElement, cellEdit.editor.element);
      }
      const hasObjectContent = cellContent != null && typeof cellContent === 'object',
        hasStringContent = typeof cellContent === 'string',
        text = hasObjectContent || cellContent == null ? '' : String(cellContent);
      // row might be flagged by GroupSummary to require full "redraw"
      if (me.forceInnerHTML) {
        // To allow minimal updates below, we must remove custom markup inserted by the GroupSummary feature
        renderTarget.innerHTML = '';
        // Delete cached content value
        delete renderTarget._content;
        cellElement.lastDomConfig = null;
      }
      // display cell contents as text or use actual html?
      // (disableHtmlEncode set by features that decorate cell contents)
      if (!hasObjectContent && column.htmlEncode && !column.disableHtmlEncode) {
        // Set innerText if cell currently has html content.
        if (cellElement._hasHtml) {
          renderTarget.innerText = text;
          cellElement._hasHtml = false;
        } else {
          DomHelper.setInnerText(renderTarget, text);
        }
      } else {
        if (column.autoSyncHtml && (!hasStringContent || DomHelper.getChildElementCount(renderTarget))) {
          // String content in html column is handled as a html template string
          if (hasStringContent) {
            // update cell with only changed attributes etc.
            DomHelper.sync(text, renderTarget.firstElementChild);
          }
          // Other content is considered to be a DomHelper config object
          else if (hasObjectContent) {
            DomSync.sync({
              domConfig: cellContent,
              targetElement: renderTarget
            });
          }
        }
        // Consider all returned plain objects to be DomHelper configs for cell content
        else if (hasObjectContent) {
          DomSync.sync({
            targetElement: renderTarget,
            domConfig: {
              onlyChildren: true,
              children: ArrayHelper.asArray(cellContent)
            }
          });
        }
        // Apply text as innerHTML only if it has changed
        else if (renderTarget._content !== text) {
          renderTarget.innerHTML = renderTarget._content = text;
        }
      }
      // If we had to render to a separate div to avoid the cell editor, insert the result now.
      if (renderTarget !== cellElement) {
        const {
          firstChild
        } = cellElement;
        for (const node of renderTarget.childNodes) {
          cellElement.insertBefore(node, firstChild);
        }
      }
    }
    // If present, framework renders content into the cell element.
    // Ignore special rows, like grouping.
    if (!record.isSpecialRow) {
      var _grid$processCellCont;
      // processCellContent is implemented in the framework wrappers
      (_grid$processCellCont = grid.processCellContent) === null || _grid$processCellCont === void 0 ? void 0 : _grid$processCellCont.call(grid, {
        cellElementData,
        rendererData,
        // In case of TreeColumn we should prerender inner cell content like expand controls, bullets, etc
        // Then the framework renders the content into the nested "b-tree-cell-value" element.
        // rendererHtml is set in TreeColumn.treeRenderer
        rendererHtml: rendererData.rendererHtml || cellContent
      });
    }
    if (column.autoHeight && size.height == null) {
      cellElement.classList.add('b-measuring-auto-height');
      // Shrinkwrap autoHeight must not allow a row's height to drop below the configured row height
      size.height = Math.max(cellElement.offsetHeight, grid.rowHeight);
      cellElement.classList.remove('b-measuring-auto-height');
    }
    if (!isMeasuring) {
      // Allow others to affect rendering
      me.rowManager.trigger('renderCell', rendererData);
    }
    return size;
  }
  //#region Hooks for salesforce
  moveContentFromCell(cellElement, editorElement) {
    cellContentRange.setStart(cellElement, 0);
    cellContentRange.setEndBefore(editorElement);
    const renderTarget = document.createElement('div');
    renderTarget.appendChild(cellContentRange.extractContents());
    return renderTarget;
  }
  //#endregion
  //endregion
}

Row.initClass();
Row._$name = 'Row';

/**
 * @module Grid/view/Bar
 */
/**
 * Base class used by Header and Footer. Holds an element for each column. Not intended to be used directly.
 *
 * @extends Core/widget/Widget
 * @internal
 * @abstract
 */
class Bar extends Widget {
  static get $name() {
    return 'Bar';
  }
  // Factoryable type name
  static get type() {
    return 'gridbar';
  }
  static get defaultConfig() {
    return {
      htmlCls: '',
      scrollable: {
        overflowX: 'hidden-scroll'
      }
    };
  }
  //region Init
  get columns() {
    return this._columns || this.subGrid.columns;
  }
  // Only needed for tests which create standalone Headers with no owning SubGrid.
  set columns(columns) {
    this._columns = columns;
  }
  //endregion
  /**
   * Fix cell widths (flex or fixed width) after rendering.
   * Not a part of template any longer because of CSP
   * @private
   */
  fixCellWidths() {
    const me = this,
      {
        hasFlex
      } = me.columns;
    let flexBasis;
    // single header "cell"
    me.columns.traverse(column => {
      const cellEl = me.getBarCellElement(column.id),
        domWidth = DomHelper.setLength(column.width),
        domMinWidth = DomHelper.setLength(column.minWidth),
        domMaxWidth = DomHelper.setLength(column.maxWidth);
      if (cellEl) {
        flexBasis = domWidth;
        cellEl.style.maxWidth = domMaxWidth;
        // Parent column without any specified width and flex should have flex calculated if any child has flex
        if (column.isParent && column.width == null && column.flex == null) {
          const flex = column.children.reduce((result, child) => result += !child.hidden && child.flex || 0, 0);
          // Do not want to store this flex value on the column since it is always calculated
          cellEl.style.flex = flex > 0 ? `${flex} 0 auto` : '';
          // minWidth might leak from other column when reordering, reset it
          cellEl.style.minWidth = null;
          if (flex > 0) {
            column.traverse(col => col.data.minWidth = null);
          }
        }
        // Normal case, set flex, width etc.
        else {
          if (parseInt(column.minWidth) >= 0) {
            cellEl.style.minWidth = domMinWidth;
          }
          // Clear all the things we might have to set to correct cell widths
          cellEl.style.flex = cellEl.style.flexBasis = cellEl.style.width = '';
          if (column.flex) {
            // If column has children we need to give it
            // flex-shrink: 0, flex-basis: auto so that it always
            // shrinkwraps its children without shrinking
            if (!isNaN(parseInt(column.flex)) && column.children) {
              cellEl.style.flex = `${column.flex} 0 auto`;
            } else {
              cellEl.style.flex = column.flex;
            }
          } else if (parseInt(column.width) >= 0) {
            const parent = column.parent;
            // Only grid header bar has a notion of group headers
            // Column is a child of an unwidthed group. We have to use width
            // to stretch it.
            if (me.isHeader && !parent.isRoot && !parent.width) {
              cellEl.style.width = domWidth;
            } else {
              // https://app.assembla.com/spaces/bryntum/tickets/8041
              // Column header widths must be set using flex-basis.
              // Using width means that wide widths cause a flexed SubGrid
              // to bust the flex rules.
              // Note that grid in Grid#onColumnsResized and SubGrid#fixCellWidths,
              // cells MUST still be sized using width since rows
              // are absolutely positioned and will not cause the busting out
              // problem, and rows will not stretch to shrinkwrap the cells
              // unless they are widthed with width.
              cellEl.style.flexBasis = flexBasis;
            }
          }
        }
        if (column.height >= 0) {
          cellEl.style.height = DomHelper.setLength(column.height);
        }
      }
    });
    me.scrollable.element.classList.toggle('b-has-flex', hasFlex);
  }
  getLrPadding(cellEl) {
    if (!this.cellLrPadding) {
      const s = cellEl.ownerDocument.defaultView.getComputedStyle(cellEl);
      this.cellLrPadding = parseInt(s.getPropertyValue('padding-left')) + parseInt(s.getPropertyValue('padding-right')) + parseInt(s.getPropertyValue('border-left-width')) + parseInt(s.getPropertyValue('border-right-width'));
    }
    return this.cellLrPadding;
  }
  /**
   * Get the header or footer cell element for the specified column.
   * @param {String} columnId Column id
   * @returns {HTMLElement} Header or footer element, depending on which subclass is in use.
   * @private
   */
  getBarCellElement(columnId) {
    return this.element.querySelector(`[data-column-id="${columnId}"]`);
  }
}
// Register this widget type with its Factory
Bar.initClass();
Bar._$name = 'Bar';

//import styles from '../../../resources/sass/grid/view/footer.scss';
/**
 * @module Grid/view/Footer
 */
/**
 * Grid footer, used by Summary feature. You should not need to create instances manually.
 *
 * @extends Grid/view/Bar
 * @internal
 */
class Footer extends Bar {
  static get $name() {
    return 'Footer';
  }
  // Factoryable type name
  static get type() {
    return 'gridfooter';
  }
  get subGrid() {
    return this._subGrid;
  }
  set subGrid(subGrid) {
    this._subGrid = this.owner = subGrid;
  }
  refreshContent() {
    this.element.firstElementChild.innerHTML = this.contentTemplate();
    this.fixFooterWidths();
  }
  onPaint({
    firstPaint
  }) {
    if (firstPaint) {
      this.refreshContent();
    }
  }
  template() {
    const region = this.subGrid.region;
    return TemplateHelper.tpl`
            <div class="b-grid-footer-scroller b-grid-footer-scroller-${region}" role="presentation">
                <div data-reference="footersElement" class="b-grid-footers b-grid-footers-${region}" data-region="${region}" role="presentation"></div>
            </div>
        `;
  }
  get overflowElement() {
    return this.footersElement;
  }
  //region Getters
  /**
   * Get the footer cell element for the specified column.
   * @param {String} columnId Column id
   * @returns {HTMLElement} Footer cell element
   */
  getFooter(columnId) {
    return this.getBarCellElement(columnId);
  }
  //endregion
  /**
   * Footer template. Iterates leaf columns to create content.
   * Style not included because of CSP. Widths are fixed up in
   * {@link #function-fixFooterWidths}
   * @private
   */
  contentTemplate() {
    const me = this;
    return me.columns.visibleColumns.map(column => {
      return TemplateHelper.tpl`
                <div
                    class="b-grid-footer ${column.align ? `b-grid-footer-align-${column.align}` : ''} ${column.cls || ''}"
                    data-column="${column.field || ''}" data-column-id="${column.id}" data-all-index="${column.allIndex}"
                    role="presentation">
                    ${column.footerText || ''}
                </div>`;
    }).join('');
  }
  /**
   * Fix footer widths (flex or fixed width) after rendering. Not a part of template any longer because of CSP
   * @private
   */
  fixFooterWidths() {
    this.fixCellWidths();
  }
}
// Register this widget type with its Factory
Footer.initClass();
Footer._$name = 'Footer';

/**
 * @module Grid/row/RowManager
 */
/**
 * Virtual representation of the grid, using {@link Grid.row.Row} to represent rows. Plugs into {@link Grid.view.Grid}
 * and exposes the following functions on grid itself:
 * * {@link #function-getRecordCoords()}
 * * {@link #function-getRowById()}
 * * {@link #function-getRow()}
 * * {@link #function-getRowFor()}
 * * {@link #function-getRowFromElement()}
 *
 * @example
 * let row = grid.getRowById(1);
 *
 * @plugin
 * @private
 */
class RowManager extends InstancePlugin {
  //region Config
  // Plugin configuration.
  static get pluginConfig() {
    return {
      chain: ['destroy'],
      assign: ['rowHeight', 'topRow', 'bottomRow', 'firstVisibleRow', 'lastVisibleRow', 'firstFullyVisibleRow', 'lastFullyVisibleRow', 'getRowById', 'getRecordCoords', 'getRow', 'getRowFor', 'getRowFromElement']
    };
  }
  static get defaultConfig() {
    return {
      /**
       * Number of rows to render above current viewport
       * @config {Number}
       * @default
       */
      prependRowBuffer: 5,
      /**
       * Number of rows to render below current viewport
       * @config {Number}
       * @default
       */
      appendRowBuffer: 5,
      /**
       * Default row height, assigned from Grid at construction (either from config
       * {@link Grid.view.Grid#config-rowHeight} or CSS). Can be set from renderers
       * @config {Number}
       * @default
       */
      rowHeight: null,
      /**
       * Set to `true` to get a small performance boost in applications that uses fixed row height
       * @config {Boolean}
       */
      fixedRowHeight: null,
      autoHeight: false
    };
  }
  static get properties() {
    return {
      idMap: {},
      topIndex: 0,
      lastScrollTop: 0,
      _rows: [],
      // Record id -> row height mapping
      heightMap: new Map(),
      // Sum of entries in heightMap
      totalKnownHeight: 0,
      // Will be calculated in `estimateTotalHeight()`, as totalKnownHeight + an estimate for unknown rows
      _totalHeight: 0,
      // Average of the known heights, kept up to date when entries in the heightMap are updated
      averageRowHeight: 0,
      scrollTargetRecordId: null,
      refreshDetails: {
        topRowIndex: 0,
        topRowTop: 0
      }
    };
  }
  //endregion
  //region Init
  construct(config) {
    config.grid._rowManager = this;
    super.construct(config.grid, config);
  }
  // Chained to grids doDestroy
  doDestroy() {
    // To remove timeouts
    this._rows.forEach(row => row.destroy());
    super.doDestroy();
  }
  /**
   * Initializes the RowManager with Rows to fit specified height.
   * @param {Number} height
   * @param {Boolean} [isRendering]
   * @private
   * @category Init
   */
  initWithHeight(height, isRendering = false) {
    const me = this;
    // no valid height, make room for all rows
    if (me.autoHeight) {
      height = me.store.allCount * me.preciseRowOffsetHeight;
    }
    me.viewHeight = height;
    me.calculateRowCount(isRendering);
    return height;
  }
  /**
   * Releases all elements (not from dom), calculates how many are needed, creates those and renders
   */
  reinitialize(returnToTop = false) {
    const me = this;
    // Calculate and correct the amount of rows needed (without triggering render)
    // Rows which are found to be surplus are destroyed.
    me.calculateRowCount(false, true, true);
    // If our row range is outside of the store's range, force a return to top
    if (me.topIndex + me.rowCount - 1 > me.store.count) {
      returnToTop = true;
    }
    const top = me.topRow && !returnToTop ? me.topRow.top : 0;
    me.scrollTargetRecordId = null;
    if (returnToTop) {
      me.topIndex = me.lastScrollTop = 0;
    }
    const {
      topRow
    } = me;
    if (topRow) {
      // Ensure rendering from the topRow starts at the correct position
      topRow.dataIndex = me.topIndex;
      topRow.setTop(top, true);
    }
    // Need to estimate height in case we have Grid using autoHeight
    me.estimateTotalHeight();
    me.renderFromRow(topRow);
  }
  //endregion
  //region Rows
  /**
   * Add or remove rows to fit row count
   * @private
   * @category Rows
   */
  matchRowCount(skipRender = false) {
    const me = this,
      {
        rows,
        grid
      } = me,
      numRows = rows.length,
      delta = numRows - me.rowCount;
    if (delta) {
      if (delta < 0) {
        const newRows = [];
        // add rows
        for (let index = numRows, dataIndex = numRows ? rows[numRows - 1].dataIndex + 1 : 0; index < me.rowCount; index++, dataIndex++) {
          newRows.push(new Row({
            cls: grid.rowCls,
            rowManager: me,
            grid,
            index,
            dataIndex
          }));
        }
        rows.push.apply(rows, newRows);
        // and elements (by triggering event used by SubGrid to add elements)
        me.trigger('addRows', {
          rows: newRows
        });
        if (!skipRender) {
          // render
          me.renderFromRow(rows[Math.max(0, numRows - 1)]);
        }
      } else {
        var _focusedCell$cell;
        // remove rows from bottom
        const {
            focusedCell
          } = grid,
          rowActive = (focusedCell === null || focusedCell === void 0 ? void 0 : focusedCell.id) != null && (focusedCell === null || focusedCell === void 0 ? void 0 : (_focusedCell$cell = focusedCell.cell) === null || _focusedCell$cell === void 0 ? void 0 : _focusedCell$cell.contains(DomHelper.getActiveElement(grid))),
          removedRows = rows.splice(numRows - delta, delta);
        if (rowActive) {
          var _me$getRowFor;
          // All rows going: move focus up to header to avoid unwanted focusout events.
          if (delta === numRows) {
            grid.onFocusedRowDerender();
          }
          // Focus is in the zone that's being removed: move to new last row
          else if (((_me$getRowFor = me.getRowFor(focusedCell._record)) === null || _me$getRowFor === void 0 ? void 0 : _me$getRowFor.index) >= rows.length) {
            rows[rows.length - 1].cells[focusedCell.columnIndex].focus();
          }
        }
        // trigger event in case some feature needs to cleanup when removing (widget column might be interested)
        me.trigger('removeRows', {
          rows: removedRows
        });
        removedRows.forEach(row => row.destroy());
        // no need to rerender or such when removing from bottom. all is good :)
      }
    }
  }
  /**
   * Calculates how many rows fit in the available height (view height)
   * @private
   * @category Rows
   */
  calculateRowCount(skipMatchRowCount = false, allowRowCountShrink = true, skipRender = false) {
    var _me$grid$columns;
    const me = this,
      {
        store
      } = me,
      visibleRowCount = Math.ceil(me.viewHeight / me.minRowOffsetHeight),
      // Want whole rows
      maxRenderRowCount = visibleRowCount + me.prependRowBuffer + me.appendRowBuffer;
    // If RowManager is reinitialized in a hidden state the view might not have a height
    if (!((_me$grid$columns = me.grid.columns) !== null && _me$grid$columns !== void 0 && _me$grid$columns.count) || isNaN(visibleRowCount)) {
      me.rowCount = 0;
      return 0;
    }
    // when for example jumping we do not want to remove excess rows,
    // since we know they are needed at other scroll locations
    if (maxRenderRowCount < me.rowCount && !allowRowCountShrink) {
      return me.rowCount;
    }
    me.visibleRowCount = visibleRowCount;
    me.rowCount = Math.min(store.count, maxRenderRowCount); // No need for more rows than data
    // If the row count doesn't match the calculated, ensure it matches,
    if (!skipMatchRowCount) {
      if (me.rows && me.rowCount !== me.rows.length) {
        var _me$bottomRow;
        me.matchRowCount(skipRender);
        // Rows might be pointing to data indices no longer available (when resetting to top topRow is already
        // adjusted, we don't need to take action here)
        if (((_me$bottomRow = me.bottomRow) === null || _me$bottomRow === void 0 ? void 0 : _me$bottomRow.dataIndex) >= store.count && me.topRow.dataIndex !== 0) {
          const indexDelta = me.bottomRow.dataIndex - store.count + 1;
          for (const row of me.rows) {
            row.dataIndex -= indexDelta;
          }
          me.topIndex -= indexDelta;
        }
      } else if (!me.rowCount) {
        me.trigger('changeTotalHeight', {
          totalHeight: me.totalHeight
        });
      }
      me.grid.toggleEmptyText();
    }
    return me.rowCount;
  }
  removeAllRows() {
    // remove rows from bottom
    const me = this,
      {
        topRow
      } = me,
      result = topRow ? me.refreshDetails = {
        topRowIndex: topRow.dataIndex,
        topRowTop: topRow.top
      } : me.refreshDetails,
      removedRows = me.rows.slice();
    // trigger event in case some feature needs to cleanup when removing (widget column might be interested)
    me.trigger('removeRows', {
      rows: removedRows
    });
    me.rows.forEach(row => row.destroy());
    me.rows.length = 0;
    me.idMap = {};
    // We return a descriptor of the last rendered block before the remove.
    // This is primarily for a full GridBase#renderContents to be able to perform a correct refresh.
    return result;
  }
  setPosition(refreshDetails) {
    // Sets up the rendering position for the next call to reinitialize
    const {
        topRow
      } = this,
      {
        topRowIndex,
        topRowTop
      } = refreshDetails;
    topRow.setTop(topRowTop);
    topRow.dataIndex = topRowIndex;
  }
  //endregion
  //region Rows - Getters
  get store() {
    return this.client.store;
  }
  /**
   * Get all Rows
   * @property {Grid.row.Row[]}
   * @readonly
   * @category Rows
   */
  get rows() {
    return this._rows;
  }
  /**
   * Get the Row at specified index. Returns `undefined` if the row index is not rendered.
   * @param {Number} index
   * @returns {Grid.row.Row}
   * @category Rows
   */
  getRow(index) {
    if (this.rowCount) {
      return this.rows[index - this.topIndex];
    }
  }
  /**
   * Get Row for specified record id
   * @param {Core.data.Model|String|Number} recordOrId Record id (or a record)
   * @returns {Grid.row.Row|null} Found Row or null if record not rendered
   * @category Rows
   */
  getRowById(recordOrId) {
    if (recordOrId && recordOrId.isModel) {
      recordOrId = recordOrId.id;
    }
    return this.idMap[recordOrId];
  }
  /**
   * Get a Row from an HTMLElement
   * @param {HTMLElement} element
   * @returns {Grid.row.Row|null} Found Row or null if record not rendered
   * @category Rows
   */
  getRowFromElement(element) {
    element = element.closest('.b-grid-row');
    return element && this.getRow(element.dataset.index);
  }
  /**
   * Get the row at the specified Y coordinate, which is by default viewport-based.
   * @param {Number} y The `Y` coordinate to find the Row for.
   * @param {Boolean} [local=false] Pass `true` if the `Y` coordinate is local to the SubGrid's element.
   * @returns {Grid.row.Row} Found Row or null if no row is rendered at that point.
   */
  getRowAt(y, local = false) {
    // Make it local.
    if (!local) {
      // Because this is used with event Y positions which are integers, we must
      // round the Rectangle to the closest integer.
      y -= Rectangle.from(this.grid.bodyContainer, null, true).roundPx(1).top;
      // Adjust for scrolling
      y += this.grid.scrollable.y;
    }
    y = DomHelper.roundPx(y);
    return this.rows.find(r => y >= r.top && y < r.bottom);
  }
  /**
   * Get a Row for either a record, a record id or an HTMLElement
   * @param {HTMLElement|Core.data.Model|String|Number} recordOrId Record or record id or HTMLElement
   * @returns {Grid.row.Row} Found Row or null if record not rendered
   * @category Rows
   */
  getRowFor(recordOrId) {
    if (recordOrId instanceof HTMLElement) {
      return this.getRowFromElement(recordOrId);
    }
    return this.getRowById(recordOrId);
  }
  /**
   * Gets the Row following the specified Row (by index or object). Wraps around the end.
   * @param {Number|Grid.row.Row} indexOrRow index or Row
   * @returns {Grid.row.Row}
   * @category Rows
   */
  getNextRow(indexOrRow) {
    const index = typeof indexOrRow === 'number' ? indexOrRow : indexOrRow.index;
    return this.getRow((index + 1) % this.rowCount);
  }
  /**
   * Get the Row that is currently displayed at top.
   * @property {Grid.row.Row}
   * @readonly
   * @category Rows
   */
  get topRow() {
    return this.rows[0];
  }
  /**
   * Get the Row currently displayed furthest down.
   * @property {Grid.row.Row}
   * @readonly
   * @category Rows
   */
  get bottomRow() {
    const rowCount = Math.min(this.rowCount, this.store.count);
    return this.rows[rowCount - 1];
  }
  /**
   * Get the topmost visible Row
   * @property {Grid.row.Row}
   * @readonly
   * @category Rows
   */
  get firstVisibleRow() {
    // Ceil scroll position to make behavior consistent on a scaled display
    return this.rows.find(r => r.bottom > Math.ceil(this.grid.scrollable.y));
  }
  get firstFullyVisibleRow() {
    // Ceil scroll position to make behavior consistent on a scaled display
    return this.rows.find(r => r.top >= Math.ceil(this.grid.scrollable.y));
  }
  /**
   * Get the last visible Row
   * @property {Grid.row.Row}
   * @readonly
   * @category Rows
   */
  get lastVisibleRow() {
    const {
      grid
    } = this;
    // We need the last row who's top is inside the scrolling viewport
    return ArrayHelper.findLast(this.rows, r => r.top < grid.scrollable.y + grid.bodyHeight);
  }
  get lastFullyVisibleRow() {
    const {
      grid
    } = this;
    // We need the last row who's bottom is inside the scrolling viewport
    return ArrayHelper.findLast(this.rows, r => r.bottom < grid.scrollable.y + grid.bodyHeight);
  }
  /**
   * Calls offset() for each Row passing along offset parameter
   * @param {Number} offset Pixels to translate Row elements.
   * @private
   * @category Rows
   */
  offsetRows(offset) {
    if (offset !== 0) {
      const {
          rows
        } = this,
        {
          length
        } = rows;
      for (let i = 0; i < length; i++) {
        rows[i].offset(offset);
      }
    }
    this.trigger('offsetRows', {
      offset
    });
  }
  //endregion
  //region Row height
  get prependBufferHeight() {
    return this.prependRowBuffer * this.rowOffsetHeight;
  }
  get appendBufferHeight() {
    return this.appendRowBuffer * this.rowOffsetHeight;
  }
  /**
   * Set a fixed row height (can still be overridden by renderers) or get configured row height. Setting refreshes all rows
   * @type {Number}
   * @on-owner
   * @category Rows
   */
  get rowHeight() {
    return this._rowHeight;
  }
  set rowHeight(height) {
    const me = this,
      {
        grid,
        fixedRowHeight
      } = me,
      oldHeight = me.rowHeight;
    // Do not force redraw if row height has not actually changed. Covered by GridState.t
    if (oldHeight === height) {
      return;
    }
    ObjectHelper.assertNumber(height, 'rowHeight');
    if (height < 10) {
      height = 10;
    }
    me.trigger('beforeRowHeight', {
      height
    });
    me.minRowHeight = me._rowHeight = height;
    if (fixedRowHeight) {
      me.averageRowHeight = height;
    }
    if (me.rows.length) {
      const oldY = grid.scrollable.y,
        topRow = me.getRowAt(oldY, true),
        // When changing rowHeight in a scrolled grid, there might no longer be a row at oldY
        edgeOffset = topRow ? topRow.top - oldY : 0;
      let average, oldAverage;
      // When using fixedRowHeight there is no need to update an average
      if (fixedRowHeight) {
        average = height;
        oldAverage = oldHeight;
      } else {
        oldAverage = average = me.averageRowHeight;
        me.clearKnownHeights();
        // Scale the average height in proportion to the row height change
        average *= height / oldHeight;
      }
      // Adjust number of rows, since it is only allowed to shrink in refresh()
      me.calculateRowCount(false, true, true);
      // Reposition the top row since it is used to position the rest
      me.topRow.setTop(me.topRow.dataIndex * (average + grid._rowBorderHeight), true);
      me.refresh();
      const newY = oldY * (average / oldAverage);
      // Scroll top row to the same position.
      if (newY !== oldY) {
        grid.scrollRowIntoView(topRow.id, {
          block: 'start',
          edgeOffset
        });
      }
    }
    // Note that `rowRowHeight` below is triggered in Row.js, but it needs to be documented here since it is
    // triggered on the RowManager
    /**
     * Triggered when an individual rendered {@link Grid.row.Row} has its height changed.
     * @event rowRowHeight
     * @param {Grid.row.RowManager} source The firing RowManager instance.
     * @param {Grid.row.Row} row The row which is changing.
     * @param {Core.data.Model} record The row's record.
     * @param {Number} height The row's new height.
     * @param {Number} oldHeight The row's old height.
     * @private
     */
    /**
     * Triggered when the owning Grid's {@link Grid.view.Grid#property-rowHeight} is changed.
     * @event rowHeight
     * @param {Grid.row.RowManager} source The firing RowManager instance.
     * @param {Number} height The RowManager's new default row height.
     * @param {Number} oldHeight  The RowManager's old default row height.
     * @private
     */
    me.trigger('rowHeight', {
      height,
      oldHeight
    });
  }
  /**
   * Get actually used row height, which includes any border and might be an average if using variable row height.
   * @property {Number}
   */
  get rowOffsetHeight() {
    return Math.floor(this.preciseRowOffsetHeight);
  }
  get preciseRowOffsetHeight() {
    return (this.averageRowHeight || this._rowHeight) + this.grid._rowBorderHeight;
  }
  get minRowOffsetHeight() {
    return (this.minRowHeight || this._rowHeight) + this.grid._rowBorderHeight;
  }
  /*
  * How store CRUD affects the height map:
  *
  * | Operation | Result                            |
  * |-----------|-----------------------------------|
  * | add       | No. Appears on render             |
  * | insert    | No. Appears on render             |
  * | remove    | Remove entry                      |
  * | removeAll | Clear                             |
  * | update    | No                                |
  * | replace   | Height might differ, remove entry |
  * | move      | No                                |
  * | filter    | No                                |
  * | sort      | No                                |
  * | group     | No                                |
  * | dataset   | Clear                             |
  *
  * The above is handled in GridBase
  */
  /**
   * Returns `true` if all rows have a known height. They do if all rows are visited, or if RowManager is configured
   * with `fixedRowHeight`. If so, all tops can be calculated exactly, no guessing needed
   * @property {Boolean}
   * @private
   */
  get allHeightsKnown() {
    return this.fixedRowHeight || this.heightMap.size >= this.store.count;
  }
  /**
   * Store supplied `height` using `id` as key in the height map. Called by `Row` when it gets its height.
   * Keeps `averageRowHeight` and `totalKnownHeight` up to date. Ignored when configured with `fixedRowHeight`
   * @param {String|Number} id
   * @param {Number} height
   * @internal
   */
  storeKnownHeight(id, height) {
    const me = this,
      {
        heightMap
      } = me;
    if (!me.fixedRowHeight) {
      // Decrease know height with old value
      if (heightMap.has(id)) {
        me.totalKnownHeight -= heightMap.get(id);
      }
      // Height here is "clientHeight"
      heightMap.set(id, height);
      // And increase with new
      me.totalKnownHeight += height;
      if (height < me.minRowHeight) {
        me.minRowHeight = height;
      }
      me.averageRowHeight = me.totalKnownHeight / heightMap.size;
    }
  }
  /**
   * Get the known or estimated offset height for the specified record id
   * @param {Core.data.Model} record
   * @returns {Number}
   * @private
   */
  getOffsetHeight(record) {
    const me = this;
    // record may not be there if height gets from row with already removed from the store record
    return (record && me.heightMap.get(record.id) || record && me.grid.getRowHeight(record) || me.averageRowHeight || me.rowHeight) + me.grid._rowBorderHeight;
  }
  /**
   * Invalidate cached height for a record. Removing it from `totalKnownHeight` and factoring it out of
   * `averageRowHeight`.
   * @param {Core.data.Model|Core.data.Model[]} records
   */
  invalidateKnownHeight(records) {
    const me = this;
    if (!me.fixedRowHeight) {
      const {
        heightMap
      } = me;
      records = ArrayHelper.asArray(records);
      records.forEach(record => {
        if (record) {
          if (heightMap.has(record.id)) {
            // Known height decreases when invalidating
            me.totalKnownHeight -= heightMap.get(record.id);
            heightMap.delete(record.id);
          }
        }
      });
      me.averageRowHeight = me.totalKnownHeight / heightMap.size;
    }
  }
  /**
   * Invalidates all cached height and resets `averageRowHeight` and `totalKnownHeight`
   */
  clearKnownHeights() {
    this.heightMap.clear();
    this.averageRowHeight = this.totalKnownHeight = 0;
  }
  /**
   * Calculates a row top from its data index. Uses known values from the height map, unknown are substituted with
   * the average row height. When configured with `fixedRowHeight`, it will always calculate a correct value
   * @param {Number} index Index in store
   * @private
   */
  calculateTop(index) {
    // When using fixed row height, life is easy
    if (this.fixedRowHeight) {
      return index * this.rowOffsetHeight;
    }
    const {
      store
    } = this;
    let top = 0;
    // When not using fixed row height, we make an educated guess at the top. The more rows have been visited, the
    // more correct the guess is (fully correct if all rows visited)
    for (let i = 0; i < index; i++) {
      const record = store.getAt(i);
      top += this.getOffsetHeight(record);
    }
    return Math.floor(top);
  }
  //endregion
  //region Calculations
  /**
   * Returns top and bottom for rendered row or estimated coordinates for unrendered.
   * @param {Core.data.Model|String|Number} recordOrId Record or record id
   * @param {Boolean} [local] Pass true to get relative record coordinates
   * @param {Boolean} [roughly] Pass true to allow a less exact but cheaper estimate
   * @returns {Core.helper.util.Rectangle} Record bounds with format { x, y, width, height, bottom, right }
   * @category Calculations
   */
  getRecordCoords(recordOrId, local = false, roughly = false) {
    const me = this,
      row = me.getRowById(recordOrId);
    let scrollingViewport = me.client._bodyRectangle;
    // _bodyRectangle is not updated on page/containing element scroll etc. Need to make sure it is correct in case
    // that has happend. This if-statement should be removed when fixing
    // https://app.assembla.com/spaces/bryntum/tickets/6587-cached-_bodyrectangle-should-be-updated-on--quot-external-quot--scroll/details
    if (!local) {
      scrollingViewport = me.client.refreshBodyRectangle();
    }
    // Rendered? Then we know position for certain
    if (row) {
      return new Rectangle(scrollingViewport.x, local ? Math.round(row.top) : Math.round(row.top + scrollingViewport.y - me.client.scrollable.y), scrollingViewport.width, row.offsetHeight);
    }
    return me.getRecordCoordsByIndex(me.store.indexOf(recordOrId), local, roughly);
  }
  /**
   * Returns estimated top and bottom coordinates for specified row.
   * @param {Number} recordIndex Record index
   * @param {Boolean} [local]
   * @returns {Core.helper.util.Rectangle} Estimated record bounds with format { x, y, width, height, bottom, right }
   * @category Calculations
   */
  getRecordCoordsByIndex(recordIndex, local = false, roughly = false) {
    const me = this,
      {
        topRow,
        bottomRow
      } = me,
      scrollingViewport = me.client._bodyRectangle,
      {
        id
      } = me.store.getAt(recordIndex),
      // Not using rowOffsetHeight since it floors the value and that rounding might give big errors far down
      height = me.preciseRowOffsetHeight,
      currentTopIndex = topRow.dataIndex,
      currentBottomIndex = bottomRow.dataIndex,
      // Instead of estimating top from the very top, use closest known coordinate. Makes sure a coordinate is not
      // estimated on wrong side of rendered rows, needed to correctly draw dependencies where one event is located
      // on a unrendered row
      calculateFrom =
      // bottomRow is closest, calculate from it
      recordIndex > currentBottomIndex ? {
        index: recordIndex - currentBottomIndex - 1,
        y: bottomRow.bottom,
        from: 'bottomRow'
      }
      //  closer to topRow than 0, use topRow
      : recordIndex > currentTopIndex / 2 ? {
        index: recordIndex - currentTopIndex,
        y: topRow.top,
        from: 'topRow'
      }
      // closer to the very top, use it
      : {
        index: recordIndex,
        y: 0,
        from: 'top'
      },
      top = me.allHeightsKnown && !roughly
      // All heights are known (all rows visited or fixed row height), get actual top coord
      ? me.calculateTop(recordIndex)
      // Otherwise estimate
      : Math.floor(calculateFrom.y + calculateFrom.index * height),
      result = new Rectangle(scrollingViewport.x, local ? top : top + scrollingViewport.y - me.client.scrollable.y, scrollingViewport.width,
      // Either known height or average
      Math.floor(me.heightMap.get(id) || height));
    // Signal that it's not based on an element, so is only approximate.
    // Grid.scrollRowIntoView will have to go round again using the block options below to ensure it's correct.
    result.virtual = true;
    // When the block becomes visible, scroll it to the logical position using the scrollIntoView's block
    // option. If it's above, use block: 'start', if below, use block: 'end'.
    result.block = result.bottom < scrollingViewport.y ? 'start' : result.y > scrollingViewport.bottom ? 'end' : 'nearest';
    return result;
  }
  /**
   * Total estimated grid height (used for scroller)
   * @property {Number}
   * @readonly
   * @category Calculations
   */
  get totalHeight() {
    return this._totalHeight;
  }
  //endregion
  //region Iteration etc.
  /**
   * Calls a function for each Row
   * @param {Function} fn Function that will be called with Row as first parameter
   * @category Iteration
   */
  forEach(fn) {
    this.rows.forEach(fn);
  }
  /**
   * Iterator that allows you to do for (let row of rowManager)
   * @category Iteration
   */
  [Symbol.iterator]() {
    return this.rows[Symbol.iterator]();
  }
  //endregion
  //region Scrolling & rendering
  /**
   * Refresh a single cell.
   * @param {Core.data.Model} record Record for row holding the cell that should be updated
   * @param {String|Number} columnId Column id to identify the cell within the row
   * @returns {Boolean} Returns `true` if cell was found and refreshed, `false` if not
   */
  refreshCell(record, columnId) {
    const cellContext = new Location({
      grid: this.grid,
      record,
      columnId
    });
    return Boolean(cellContext.cell && cellContext.row.renderCell(cellContext));
  }
  /**
   * Renders from the top of the grid, also resetting scroll to top. Used for example when collapsing all groups.
   * @category Scrolling & rendering
   */
  returnToTop() {
    const me = this;
    me.topIndex = 0;
    me.lastScrollTop = 0;
    if (me.topRow) {
      me.topRow.dataIndex = 0;
      // Force the top row to the top of the scroll range
      me.topRow.setTop(0, true);
    }
    me.refresh();
    // Rows rendered from top, make sure grid is scrolled to top also
    me.grid.scrollable.y = 0;
  }
  /**
   * Renders from specified records row and down (used for example when collapsing a group, does not affect rows above).
   * @param {Core.data.Model} record Record of first row to render
   * @category Scrolling & rendering
   */
  renderFromRecord(record) {
    const row = this.getRowById(record.id);
    if (row) {
      this.renderFromRow(row);
    }
  }
  /**
   * Renders from specified row and down (used for example when collapsing a group, does not affect rows above).
   * @param {Grid.row.Row} fromRow First row to render
   * @category Scrolling & rendering
   */
  renderFromRow(fromRow = null) {
    const me = this,
      {
        rows,
        store
      } = me,
      storeCount = store.count;
    // Calculate row count, adding rows if needed, but do not rerender - we are going to do that below.
    // Bail out if no rows. Allow removing rows if we have more than store have rows
    if (me.calculateRowCount(false, storeCount < rows.length, true) === 0) {
      // Reestimate total height. Possible if all tasks deleted
      me.estimateTotalHeight(true);
      return;
    }
    // render from this row
    const fromRowIndex = fromRow ? rows.indexOf(fromRow) : 0;
    // starting either from its specified dataIndex or from its index (happens on first render, no dataIndex yet)
    let dataIndex = fromRow ? fromRow.dataIndex : rows[0].dataIndex;
    const
      // amount of records after this one in store
      recordsAfter = storeCount - dataIndex - 1,
      // render to this row, either the last row or the row which will hold the last record available
      toRowIndex = Math.min(rows.length - 1, fromRowIndex + recordsAfter);
    let
      // amount of rows which won't be rendered below last record (if we have fewer records than topRow + row count)
      leftOverCount = rows.length - toRowIndex - 1,
      // Start with top correctly just below the previous row's bottom
      top = fromRowIndex > 0 ? rows[fromRowIndex - 1].bottom : rows[fromRowIndex].top,
      row;
    // _rows array is ordered in display order, just iterate to the end
    for (let i = fromRowIndex; i <= toRowIndex; i++) {
      row = rows[i];
      // Needed in scheduler when translating events, happens before render
      row.dataIndex = dataIndex;
      // Silent translation, render will update contents anyway
      row.setTop(top, true);
      row.render(dataIndex, store.getAt(dataIndex++), false);
      top += row.offsetHeight;
    }
    // if number for records to display has decreased, for example by collapsing a node, we might get unused rows
    // below bottom. move those to top to not have unused rows laying around
    while (leftOverCount-- > 0) {
      me.displayRecordAtTop();
    }
    // Renderers might yield a lower row height than the configured, leaving blank space at bottom
    if (me.bottomRow.bottom < me.viewHeight) {
      me.calculateRowCount();
    }
    // Reestimate total height
    me.estimateTotalHeight(true);
    me.trigger('renderDone');
  }
  /**
   * Renders the passed array (or [Set](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Set)) of {@link Grid.row.Row rows}
   * @param {Grid.row.Row[]|Set} rows The rows to render
   * @category Scrolling & rendering
   */
  renderRows(rows) {
    let oldHeight,
      heightChanged = false;
    rows = Array.from(rows);
    // Sort topmost row first
    rows.sort((a, b) => a.dataIndex - b.dataIndex);
    // Render the requested rows.
    for (const row of rows) {
      oldHeight = row.height;
      // Pass updatingSingleRow as false, so that it does not shuffle following
      // rows downwards on each render. We do that once here after the rows are all refreshed.
      row.render(null, null, false);
      heightChanged |= row.height !== oldHeight;
    }
    // If this caused a height change, shuffle following rows.
    if (heightChanged) {
      this.translateFromRow(rows[0]);
    }
    this.trigger('renderDone');
  }
  /**
   * Translates all rows after the specified row. Used when a single rows height is changed and the others should
   * rearrange. (Called from Row#render)
   * @param {Grid.row.Row} fromRow
   * @private
   * @category Scrolling & rendering
   */
  translateFromRow(fromRow, batch = false) {
    const me = this;
    let top = fromRow.bottom,
      row,
      index;
    for (index = fromRow.dataIndex + 1, row = me.getRow(index); row; row = me.getRow(++index)) {
      top = row.translate(top);
    }
    // Reestimate total height
    if (!batch) {
      me.estimateTotalHeight(true);
    }
  }
  /**
   * Rerender all rows
   * @category Scrolling & rendering
   */
  refresh() {
    const me = this,
      {
        topRow
      } = me;
    // too early
    if (!topRow || me.grid.refreshSuspended) {
      return;
    }
    me.idMap = {};
    me.renderFromRow(topRow);
    me.trigger('refresh');
  }
  /**
   * Makes sure that specified record is displayed in view
   * @param newScrollTop Top of visible section
   * @param [forceRecordIndex] Index of record to display at center
   * @private
   * @category Scrolling & rendering
   */
  jumpToPosition(newScrollTop, forceRecordIndex) {
    // There are two very different requirements here.
    // If there is a forceRecordIndex, that takes precedence to get it into the center of the
    // viewport, and wherever we render the calculated row block, we may then *adjust the scrollTop*
    // to get that row to the center.
    //
    // If there's no forceRecordIndex, then the scroll position is the primary objective and
    // we must render what we calculate to be correct at that viewport position.
    const me = this,
      {
        store,
        heightMap
      } = me,
      storeCount = store.count;
    if (me.allHeightsKnown && !me.fixedRowHeight) {
      const top = newScrollTop - me.prependBufferHeight,
        border = me.grid._rowBorderHeight;
      let accumulated = 0,
        targetIndex = 0;
      while (accumulated < top) {
        const record = store.getAt(targetIndex);
        accumulated += heightMap.get(record.id) + border;
        targetIndex++;
      }
      const startIndex = Math.max(Math.min(targetIndex, storeCount - me.rowCount), 0);
      me.lastScrollTop = newScrollTop;
      me.topRow.dataIndex = me.topIndex = startIndex;
      me.topRow.setTop(me.calculateTop(startIndex), false);
      // render entire buffer
      me.refresh();
    } else {
      const rowHeight = me.preciseRowOffsetHeight,
        // Calculate index of the top of the rendered block.
        // If we are targeting the scrollTop, this will be the top index at the scrollTop minus prepend count.
        // If we are targeting a recordIndex, this will attempt to place that in the center of the rendered block.
        targetIndex = forceRecordIndex == null ? Math.floor(newScrollTop / rowHeight) - me.prependRowBuffer : forceRecordIndex - Math.floor(me.rowCount / 2),
        startIndex = Math.max(Math.min(targetIndex, storeCount - me.rowCount), 0),
        viewportTop = me.client.scrollable.y,
        viewportBottom = Math.min(me.client._bodyRectangle.height + viewportTop + me.appendBufferHeight, me.totalHeight);
      me.lastScrollTop = newScrollTop;
      me.topRow.dataIndex = me.topIndex = startIndex;
      me.topRow.setTop(Math.floor(startIndex * rowHeight), false);
      // render entire buffer
      me.refresh();
      // Not filled all the way down?
      if (me.bottomRow.bottom < viewportBottom) {
        // Might have jumped into a section of low heights. Needs to be done after the refresh, since heights
        // are not known before it
        me.calculateRowCount(false, false, false);
        // Fill with available rows (might be available above buffer because of var row height), stop if we run out of records :)
        while (me.bottomRow.bottom < viewportBottom && me._rows[me.prependRowBuffer].top < viewportTop && me.bottomRow.dataIndex < storeCount - 1) {
          me.displayRecordAtBottom();
        }
      }
      me.estimateTotalHeight();
    }
    // If the row index is our priority, then scroll it into the center
    if (forceRecordIndex != null) {
      const {
          scrollable
        } = me.grid,
        targetRow = me.getRow(forceRecordIndex),
        // When coming from a block of high rowHeights to one with much lower we might still miss the target...
        rowCenter = targetRow && Rectangle.from(targetRow._elementsArray[0]).center.y,
        viewportCenter = scrollable.viewport.center.y;
      // Scroll the targetRow into the center of the viewport
      if (targetRow) {
        scrollable.y = newScrollTop = Math.floor(scrollable.y + (rowCenter - viewportCenter));
      }
    }
    return newScrollTop;
  }
  /**
   * Jumps to a position if it is far enough from current position. Otherwise does nothing.
   * @private
   * @category Scrolling & rendering
   */
  warpIfNeeded(newScrollTop) {
    const me = this,
      result = {
        newScrollTop,
        deltaTop: newScrollTop - me.lastScrollTop
      };
    // if gap to fill is large enough, better to jump there than to fill row by row
    if (Math.abs(result.deltaTop) > me.rowCount * me.rowOffsetHeight * 3) {
      // no specific record targeted
      let index;
      // Specific record specified as target of scroll?
      if (me.scrollTargetRecordId) {
        index = me.store.indexOf(me.scrollTargetRecordId);
        // since scroll is happening async record might have been removed after requesting scroll,
        // in that case we rely on calculated index (as when scrolling without target)
      }
      // We are jumping, so the focused row will derender
      me.grid.onFocusedRowDerender();
      // perform the jump and return results
      result.newScrollTop = me.jumpToPosition(newScrollTop, index);
      result.deltaTop = 0; // no extra filling needed
    }

    return result;
  }
  /**
   * Handles virtual rendering (only visible rows + buffer are in dom) for rows
   * @param {Number} newScrollTop The `Y` scroll position for which to render rows.
   * @param {Boolean} [force=false] Pass `true` to update the rendered row block even if the scroll position has not changed.
   * @returns {Number} Adjusted height required to fit rows
   * @private
   * @category Scrolling & rendering
   */
  updateRenderedRows(newScrollTop, force, ignoreError = false) {
    const me = this,
      clientRect = me.client._bodyRectangle;
    // Might be triggered after removing all records, should not crash
    if (me.rowCount === 0) {
      return 0;
    }
    let result = me.totalHeight;
    if (force ||
    // Only react if we have scrolled by one row or more
    Math.abs(newScrollTop - me.lastScrollTop) >= me.rowOffsetHeight ||
    // or if we have a gap at top/bottom (#9375)
    me.topRow.top > newScrollTop || me.bottomRow.bottom < newScrollTop + clientRect.height) {
      // If scrolled by a large amount, jump instead of rendering each row
      const posInfo = me.warpIfNeeded(newScrollTop);
      me.scrollTargetRecordId = null;
      // Cache the last correct render scrollTop before fill.
      // it can be adjusted to hide row position corrections.
      me.lastScrollTop = posInfo.newScrollTop;
      if (posInfo.deltaTop > 0) {
        // Scrolling down
        me.fillBelow(posInfo.newScrollTop);
      } else if (posInfo.deltaTop < 0) {
        // Scrolling up
        me.fillAbove(posInfo.newScrollTop);
      }
      if (!me.fixedRowHeight && !ignoreError) {
        me.correctError(posInfo, clientRect, newScrollTop);
      }
      // Calculate the new height based on new content
      result = me.estimateTotalHeight();
    }
    return result;
  }
  correctError(posInfo, clientRect, newScrollTop) {
    const me = this;
    let error = 0;
    // When we transition from not knowing all heights to doing so, the old estimate will likely have positioned
    // rows a bit off. Compensate for that here.
    if (me.allHeightsKnown) {
      error = me.topRow.top - me.calculateTop(me.topRow.dataIndex);
    }
    // If it's a temporary scroll, we can be told to ignore the drift.
    // Apart from that, we must correct keep the rendered block position correct.
    // Otherwise, when rolling upwards after a teleport, we may not be able to reach
    // the top. Some rows may end up at -ve positions.
    else {
      // Only correct the rendered block position if we are in danger of running out of scroll space.
      // That is if we are getting towards the top or bottom of the scroll range.
      if (
      // Scrolling up within top zone
      posInfo.deltaTop < 0 && newScrollTop < clientRect.height * 2 ||
      // Scrolling down within bottom zone
      posInfo.deltaTop > 0 && newScrollTop > me.totalHeight - clientRect.height * 2 - 3) {
        error = me.topRow.top - me.calculateTop(me.topRow.dataIndex); //me.topIndex * me.rowOffsetHeight;
      }
    }

    if (error) {
      // Correct the rendered block position if it's not at the calculated position.
      // Keep the visual position correct by adjusting the scrollTop by the same amount.
      // When variable row heights are used, this will keep the rendered block top correct.
      me.offsetRows(-error);
      me.grid.scrollable.y = me.lastScrollTop = me.grid.scrollable.y - error;
    }
  }
  /**
   * Moves as many rows from the bottom to the top that are needed to fill to current scroll pos.
   * @param newTop Scroll position
   * @private
   * @category Scrolling & rendering
   */
  fillAbove(newTop) {
    const me = this,
      fillHeight = newTop - me.topRow.top - me.prependBufferHeight;
    let accumulatedHeight = 0;
    while (accumulatedHeight > fillHeight && me.topIndex > 0) {
      // We want to show prev record at top of rows
      accumulatedHeight -= me.displayRecordAtTop();
    }
    me.trigger('renderDone');
  }
  /**
   * Moves as many rows from the top to the bottom that are needed to fill to current scroll pos.
   * @param newTop Scroll position
   * @private
   * @category Scrolling & rendering
   */
  fillBelow(newTop) {
    const me = this,
      fillHeight = newTop - me.topRow.top - me.prependBufferHeight,
      recordCount = me.store.count,
      rowCount = me.rowCount;
    let accumulatedHeight = 0;
    // Repeat until we have filled empty height
    while (accumulatedHeight < fillHeight &&
    // fill empty height
    me.topIndex + rowCount < recordCount &&
    // as long as we have records left
    me.topRow.top + me.topRow.offsetHeight < newTop // and do not move top row fully into view (can happen with var row height)
    ) {
      // We want to show next record at bottom of rows
      accumulatedHeight += me.displayRecordAtBottom();
    }
    me.trigger('renderDone');
  }
  /**
   * Estimates height needed to fit all rows, based on average row height. Also offsets rows if needed to not be above
   * the reachable area of the view.
   * @param {Boolean} [immediate] Specify true to pass the `immediate` flag on to any listeners (probably only Grid
   * cares. Used to bypass buffered element resize)
   * @returns {Number}
   * @private
   * @category Scrolling & rendering
   */
  estimateTotalHeight(immediate = false) {
    const me = this;
    if (me.grid.renderingRows) {
      return;
    }
    const recordCount = me.store.count,
      unknownCount = recordCount - me.heightMap.size,
      {
        bottomRow
      } = me;
    let estimate;
    // No need to estimate when using fixed row height
    if (me.fixedRowHeight) {
      estimate = recordCount * me.rowOffsetHeight;
    } else {
      estimate =
      // Known height, from entries in heightMap
      me.totalKnownHeight +
      // Those heights are "clientHeights", estimate needs to include borders
      me.heightMap.size * me.grid._rowBorderHeight +
      // Add estimate for rows with unknown height
      unknownCount * me.preciseRowOffsetHeight;
      // No bottomRow yet if estimating initial height in autoHeight grid
      if (bottomRow && unknownCount) {
        const bottom = bottomRow.bottom;
        // Too low estimate or reached the end with scroll left, adjust to fit current bottom
        if (bottom > estimate || me.topIndex + me.rowCount >= recordCount && estimate > bottom && bottom > 0) {
          estimate = bottom;
          // estimate all the way down
          if (bottomRow.dataIndex < recordCount - 1) {
            estimate += (recordCount - 1 - bottomRow.dataIndex) * me.preciseRowOffsetHeight;
          }
        }
      }
      estimate = Math.floor(estimate);
    }
    if (estimate !== me.totalHeight) {
      if (me.trigger('changeTotalHeight', {
        totalHeight: estimate,
        immediate
      }) !== false) {
        me._totalHeight = estimate;
      }
    }
    return estimate;
  }
  /**
   * Moves a row from bottom to top and renders the corresponding record to it.
   * @returns {Number} New row height
   * @private
   * @category Scrolling & rendering
   */
  displayRecordAtTop() {
    var _grid$focusedCell;
    const me = this,
      {
        grid
      } = me,
      recordIndex = me.topIndex - 1,
      record = me.store.getAt(recordIndex),
      // Row currently rendered at the bottom, the row we want to move
      bottomRow = me.bottomRow,
      bottomRowTop = bottomRow.top;
    me.trigger('beforeTranslateRow', {
      row: bottomRow,
      newRecord: record
    });
    // If focused cell is being scrolled off...
    if (bottomRow.dataIndex === ((_grid$focusedCell = grid.focusedCell) === null || _grid$focusedCell === void 0 ? void 0 : _grid$focusedCell.rowIndex)) {
      grid.onFocusedRowDerender();
    }
    // estimated top, for rendering that depends on having top
    bottomRow._top = me.topRow.top - me.getOffsetHeight(record);
    // if configured with fixed row height, it will be the correct value
    bottomRow.estimatedTop = !me.fixedRowHeight;
    // Render row
    bottomRow.render(recordIndex, record, false);
    // Move it to top. Restore top so that the setter won't reject non-change
    // if the estimate happened to be correct.
    bottomRow._top = bottomRowTop;
    bottomRow.setBottom(me.topRow.top);
    bottomRow.estimatedTop = false;
    // Prev row is now at top
    me.topIndex--;
    // move to start of array (bottomRow becomes topRow)
    me._rows.unshift(me._rows.pop());
    return bottomRow.offsetHeight;
  }
  /**
   * Moves a row from top to bottom and renders the corresponding record to it.
   * @returns {Number} New row height
   * @private
   * @category Scrolling & rendering
   */
  displayRecordAtBottom() {
    var _grid$focusedCell2;
    const me = this,
      {
        grid
      } = me,
      recordIndex = me.topIndex + me.rowCount,
      record = me.store.getAt(recordIndex),
      // Row currently rendered on the top, the row we want to move
      topRow = me.topRow;
    me.trigger('beforeTranslateRow', {
      row: topRow,
      newRecord: record
    });
    // If focused cell is being scrolled off...
    if (topRow.dataIndex === ((_grid$focusedCell2 = grid.focusedCell) === null || _grid$focusedCell2 === void 0 ? void 0 : _grid$focusedCell2.rowIndex)) {
      grid.onFocusedRowDerender();
    }
    topRow.dataIndex = recordIndex;
    // Move it to bottom
    topRow.setTop(me.bottomRow.bottom);
    // Render row
    topRow.render(recordIndex, record, false);
    // Next row is now at top
    me.topIndex++;
    // move to end of array (topRow becomes bottomRow)
    me._rows.push(me._rows.shift());
    return topRow.offsetHeight;
  }
  //endregion
}

RowManager.featureClass = '';
RowManager._$name = 'RowManager';

/**
 * @module Grid/util/GridScroller
 */
const xAxis = {
    x: 1
  },
  subGridFilter = w => w.isSubGrid;
/**
 * A Scroller subclass which handles scrolling in a grid.
 *
 * If the grid has no parallel scrolling grids (No locked columns), then this functions
 * transparently as a Scroller.
 *
 * If there are locked columns, then scrolling to an _element_ will invoke the scroller
 * of the subgrid which contains that element.
 * @internal
 */
class GridScroller extends Scroller {
  addScroller(scroller) {
    (this.xScrollers || (this.xScrollers = [])).push(scroller);
  }
  addPartner(otherScroller, axes = xAxis) {
    if (typeof axes === 'string') {
      axes = {
        [axes]: 1
      };
    }
    // Link up all our X scrollers
    if (axes.x) {
      // Ensure the other grid has set up its scrollers. This is done on first paint
      // so may not have been executed yet.
      otherScroller.owner.initScroll();
      const subGrids = this.widget.items.filter(subGridFilter),
        otherSubGrids = otherScroller.widget.items.filter(subGridFilter);
      // Loop through SubGrids to ensure that we partner their scrollers up correctly
      for (let i = 0, {
          length
        } = subGrids; i < length; i++) {
        subGrids[i].scrollable.addPartner(otherSubGrids[i].scrollable, 'x');
      }
    }
    // We are the only Y scroller
    if (axes.y) {
      super.addPartner(otherScroller, 'y');
    }
  }
  removePartner(otherScroller) {
    this.xScrollers.forEach((scroller, i) => {
      if (!scroller.isDestroyed) {
        scroller.removePartner(otherScroller.xScrollers[i]);
      }
    });
    super.removePartner(otherScroller);
  }
  updateOverflowX(overflowX) {
    var _this$xScrollers;
    const hideScroll = overflowX === false;
    (_this$xScrollers = this.xScrollers) === null || _this$xScrollers === void 0 ? void 0 : _this$xScrollers.forEach(s => s.overflowX = hideScroll ? 'hidden' : 'hidden-scroll');
    this.widget.virtualScrollers.classList.toggle('b-hide-display', hideScroll);
  }
  scrollIntoView(element, options) {
    // If we are after an element, we have to ask the scroller of the SubGrid
    // that the element is in. It will do the X scrolling and delegate the Y
    // scrolling up to this GridScroller.
    if (element.nodeType === Element.ELEMENT_NODE && this.element.contains(element)) {
      for (const subGridScroller of this.xScrollers) {
        if (subGridScroller.element.contains(element)) {
          return subGridScroller.scrollIntoView(element, options);
        }
      }
    } else {
      return super.scrollIntoView(element, options);
    }
  }
  hasOverflow(axis = 'y') {
    return axis === 'y' ? this.scrollHeight > this.clientHeight : false;
  }
  set x(x) {
    if (this.xScrollers) {
      this.xScrollers[0].x = x;
    }
  }
  get x() {
    // when trying to scroll grid with no columns xScrollers do not exist
    return this.xScrollers ? this.xScrollers[0].x : 0;
  }
}
GridScroller._$name = 'GridScroller';

/**
 * @module Grid/view/Header
 */
/**
 * The Grid header, which contains simple columns but also allows grouped columns. One instance is created and used per SubGrid
 * automatically, you should not need to instantiate this class manually. See {@link Grid.column.Column} for information about
 * column configuration.
 *
 * @extends Grid/view/Bar
 * @internal
 *
 * @inlineexample Grid/view/Header.js
 */
class Header extends Bar {
  static $name = 'Header';
  static type = 'gridheader';
  get subGrid() {
    return this._subGrid;
  }
  set subGrid(subGrid) {
    this._subGrid = this.owner = subGrid;
  }
  get region() {
    var _this$subGrid;
    return (_this$subGrid = this.subGrid) === null || _this$subGrid === void 0 ? void 0 : _this$subGrid.region;
  }
  changeElement(element, was) {
    const {
      region
    } = this;
    // Columns must be examined for maxDepth
    this.getConfig('columns');
    return super.changeElement({
      className: {
        'b-grid-header-scroller': 1,
        [`b-grid-header-scroller-${region}`]: region
      },
      children: [{
        reference: 'headersElement',
        className: {
          'b-grid-headers': 1,
          [`b-grid-headers-${region}`]: region
        },
        dataset: {
          region,
          reference: 'headersElement',
          maxDepth: this.maxDepth
        }
      }]
    }, was);
  }
  get overflowElement() {
    return this.headersElement;
  }
  /**
   * Recursive column header config creator.
   * Style not included because of CSP. Widths are fixed up in
   * {@link #function-fixHeaderWidths}
   * @private
   */
  getColumnConfig(column) {
    const {
        id,
        align,
        resizable,
        isLeaf,
        isParent,
        isLastInSubGrid,
        cls,
        childLevel,
        field,
        tooltip,
        children,
        isFocusable,
        grid
      } = column,
      // Headers tested standalone - may be no grid
      focusedCell = grid === null || grid === void 0 ? void 0 : grid.focusedCell,
      isFocused = (focusedCell === null || focusedCell === void 0 ? void 0 : focusedCell.rowIndex) === -1 && (focusedCell === null || focusedCell === void 0 ? void 0 : focusedCell.column) === column;
    if (column.isVisible) {
      return {
        className: {
          'b-grid-header': 1,
          'b-grid-header-parent': isParent,
          [`b-level-${childLevel}`]: 1,
          [`b-depth-${column.meta.depth}`]: 1,
          [`b-grid-header-align-${align}`]: align,
          'b-grid-header-resizable': resizable && isLeaf,
          [cls]: cls,
          'b-collapsible': column.collapsible,
          'b-last-parent': isParent && isLastInSubGrid,
          'b-last-leaf': isLeaf && isLastInSubGrid
        },
        role: isFocusable ? 'columnheader' : 'presentation',
        'aria-sort': 'none',
        'aria-label': column.ariaLabel,
        [isFocusable ? 'tabIndex' : '']: isFocused ? 0 : -1,
        dataset: {
          ...Tooltip.encodeConfig(tooltip),
          columnId: id,
          [field ? 'column' : '']: field
        },
        children: [{
          className: 'b-grid-header-text',
          children: [{
            [grid && isFocusable ? 'id' : '']: `${grid === null || grid === void 0 ? void 0 : grid.id}-column-${column.id}`,
            className: 'b-grid-header-text-content'
          }]
        }, children ? {
          className: 'b-grid-header-children',
          children: children.map(child => this.getColumnConfig(child)),
          syncOptions: {
            syncIdField: 'columnId'
          }
        } : null, {
          className: 'b-grid-header-resize-handle'
        }]
      };
    }
  }
  // used by safari to fix flex when rows width shrink below this value
  calculateMinWidthForSafari() {
    let minWidth = 0;
    this.columns.visibleColumns.forEach(column => {
      minWidth += column.calculateMinWidth();
    });
    return minWidth;
  }
  /**
   * Fix header widths (flex or fixed width) after rendering. Not a part of template any longer because of CSP
   * @private
   */
  fixHeaderWidths() {
    this.fixCellWidths();
  }
  refreshHeaders() {
    const me = this;
    // run renderers, not done from template to work more like cell rendering
    me.columns.traverse(column => {
      const headerElement = me.getBarCellElement(column.id);
      if (headerElement) {
        let html = column.headerText;
        if (column.headerRenderer) {
          html = column.headerRenderer.call(column.thisObj || me, {
            column,
            headerElement
          });
        }
        if (column.headerWidgetMap) {
          Object.values(column.headerWidgetMap).forEach(widget => {
            widget.render(column.textWrapper);
          });
        }
        if (column.icon) {
          html = `<i class="${StringHelper.encodeHtml(column.icon)}"></i>` + (html || '');
        }
        const innerEl = headerElement.querySelector('.b-grid-header-text-content');
        if (innerEl) {
          innerEl.innerHTML = html || '';
        }
      }
    });
    me.fixHeaderWidths();
  }
  get columns() {
    const me = this,
      result = super.columns;
    if (!me.columnsDetacher) {
      // columns is a chained store, it will be repopulated from master when columns change.
      // That action always triggers change with action dataset.
      me.columnsDetacher = result.ion({
        change() {
          me.initDepths();
        },
        thisObj: me
      });
      me.initDepths();
    }
    return result;
  }
  set columns(columns) {
    super.columns = columns;
  }
  /**
   * Depths are used for styling of grouped headers. Sets them on meta.
   * @private
   */
  initDepths(columns = this.columns.topColumns, parent = null) {
    const me = this;
    let maxDepth = 0;
    if (parent !== null && parent !== void 0 && parent.meta) {
      parent.meta.depth++;
    }
    for (const column of columns) {
      const {
        meta
      } = column;
      meta.depth = 0;
      if (column.children) {
        me.initDepths(column.children.filter(me.columns.chainedFilterFn), column);
        if (meta.depth && parent) {
          parent.meta.depth += meta.depth;
        }
      }
      if (meta.depth > maxDepth) {
        maxDepth = meta.depth;
      }
    }
    if (!parent) {
      me.maxDepth = maxDepth;
    }
    return maxDepth;
  }
  //endregion
  //region Getters
  /**
   * Get the header cell element for the specified column.
   * @param {String} columnId Column id
   * @returns {HTMLElement} Header cell element
   */
  getHeader(columnId) {
    return this.getBarCellElement(columnId);
  }
  //endregion
  get contentElement() {
    return this.element.firstElementChild;
  }
  refreshContent() {
    const me = this;
    DomSync.sync({
      domConfig: {
        children: me.columns.topColumns.map(col => me.getColumnConfig(col)),
        onlyChildren: true
      },
      targetElement: me.contentElement,
      strict: true,
      syncIdField: 'columnId',
      releaseThreshold: 0
    });
    me.refreshHeaders();
  }
  onPaint({
    firstPaint
  }) {
    if (firstPaint) {
      this.refreshContent();
    }
  }
}
// Register this widget type with its Factory
Header.initClass();
Header._$name = 'Header';

const gridBodyElementEventHandlers = {
    touchstart: 'onElementTouchStart',
    touchmove: 'onElementTouchMove',
    touchend: 'onElementTouchEnd',
    pointerover: 'onElementMouseOver',
    mouseout: 'onElementMouseOut',
    mousedown: 'onElementMouseDown',
    mousemove: 'onElementMouseMove',
    mouseup: 'onElementMouseUp',
    click: 'onHandleElementClick',
    dblclick: 'onElementDblClick',
    keyup: 'onElementKeyUp',
    keypress: 'onElementKeyPress',
    contextmenu: 'onElementContextMenu',
    pointerdown: 'onElementPointerDown',
    pointerup: 'onElementPointerUp'
  },
  eventProps = ['pageX', 'pageY', 'clientX', 'clientY', 'screenX', 'screenY'];
function toggleHover(element, add = true) {
  element === null || element === void 0 ? void 0 : element.classList.toggle('b-hover', add);
}
function setCellHover(columnId, row, add = true) {
  row && columnId && toggleHover(row.getCell(columnId), add);
}
/**
 * @module Grid/view/mixin/GridElementEvents
 */
/**
 * Mixin for Grid that handles dom events. Some listeners fire own events but all can be chained by features. None of
 * the functions in this class are indented to be called directly.
 *
 * See {@link Grid.view.Grid} for more information on grid keyboard interaction.
 *
 * @mixin
 */
var GridElementEvents = (Target => class GridElementEvents extends (Target || Base) {
  static get $name() {
    return 'GridElementEvents';
  }
  //region Config
  static get configurable() {
    return {
      /**
       * The currently hovered grid cell
       * @member {HTMLElement}
       * @readonly
       */
      hoveredCell: null,
      /**
       * Time in ms until a longpress is triggered
       * @prp {Number}
       * @default
       * @category Misc
       */
      longPressTime: 400,
      /**
       * Set to true to listen for CTRL-Z (CMD-Z on Mac OS) keyboard event and trigger undo (redo when SHIFT is
       * pressed). Only applicable when using a {@link Core.data.stm.StateTrackingManager}.
       * @prp {Boolean}
       * @default
       * @category Misc
       */
      enableUndoRedoKeys: true,
      keyMap: {
        'Ctrl+z': 'undoRedoKeyPress',
        'Ctrl+Shift+z': 'undoRedoKeyPress',
        ' ': {
          handler: 'clickCellByKey',
          weight: 1000
        }
      }
    };
  }
  //endregion
  //region Events
  /**
   * Fired when user clicks in a grid cell
   * @event cellClick
   * @param {Grid.view.Grid} grid The grid instance
   * @param {Core.data.Model} record The record representing the row
   * @param {Grid.column.Column} column The column to which the cell belongs
   * @param {HTMLElement} cellElement The cell HTML element
   * @param {HTMLElement} target The target element
   * @param {MouseEvent} event The native DOM event
   */
  /**
   * Fired when user double clicks a grid cell
   * @event cellDblClick
   * @param {Grid.view.Grid} grid The grid instance
   * @param {Core.data.Model} record The record representing the row
   * @param {Grid.column.Column} column The column to which the cell belongs
   * @param {HTMLElement} cellElement The cell HTML element
   * @param {HTMLElement} target The target element
   * @param {MouseEvent} event The native DOM event
   */
  /**
   * Fired when user activates contextmenu in a grid cell
   * @event cellContextMenu
   * @param {Grid.view.Grid} grid The grid instance
   * @param {Core.data.Model} record The record representing the row
   * @param {Grid.column.Column} column The column to which the cell belongs
   * @param {HTMLElement} cellElement The cell HTML element
   * @param {HTMLElement} target The target element
   * @param {MouseEvent} event The native DOM event
   */
  /**
   * Fired when user moves the mouse over a grid cell
   * @event cellMouseOver
   * @param {Grid.view.Grid} grid The grid instance
   * @param {Core.data.Model} record The record representing the row
   * @param {Grid.column.Column} column The column to which the cell belongs
   * @param {HTMLElement} cellElement The cell HTML element
   * @param {HTMLElement} target The target element
   * @param {MouseEvent} event The native DOM event
   */
  /**
   * Fired when a user moves the mouse out of a grid cell
   * @event cellMouseOut
   * @param {Grid.view.Grid} grid The grid instance
   * @param {Core.data.Model} record The record representing the row
   * @param {Grid.column.Column} column The column to which the cell belongs
   * @param {HTMLElement} cellElement The cell HTML element
   * @param {HTMLElement} target The target element
   * @param {MouseEvent} event The native DOM event
   */
  //endregion
  //region Event handling
  /**
   * Init listeners for a bunch of dom events. All events are handled by handleEvent().
   * @private
   * @category Events
   */
  initInternalEvents() {
    const handledEvents = Object.keys(gridBodyElementEventHandlers),
      len = handledEvents.length,
      listeners = {
        element: this.bodyElement,
        thisObj: this
      };
    // Route all events through handleEvent, so that we can capture this.event
    // before we route to the handlers
    for (let i = 0; i < len; i++) {
      const eventName = handledEvents[i];
      listeners[eventName] = {
        handler: 'handleEvent'
      };
      // Override default for touch events.
      // Other event types already have correct default.
      if (eventName.startsWith('touch')) {
        listeners[eventName].passive = false;
      }
    }
    EventHelper.on(listeners);
    EventHelper.on({
      focusin: 'onGridBodyFocusIn',
      element: this.bodyElement,
      thisObj: this,
      capture: true
    });
  }
  /**
   * This method finds the cell location of the passed event. It returns an object describing the cell.
   * @param {Event} event A Mouse, Pointer or Touch event targeted at part of the grid.
   * @param {Boolean} [includeSingleAxisMatch] Set to `true` to return a cell from xy either above or below the Grid's
   * body or to the left or right.
   * @returns {Object} An object containing the following properties:
   * - `cellElement` - The cell element clicked on.
   * - `column` - The {@link Grid.column.Column column} clicked under.
   * - `columnId` - The `id` of the {@link Grid.column.Column column} clicked under.
   * - `record` - The {@link Core.data.Model record} clicked on.
   * - `id` - The `id` of the {@link Core.data.Model record} clicked on.
   * @internal
   * @category Events
   */
  getCellDataFromEvent(event, includeSingleAxisMatch = false) {
    const me = this,
      {
        columns
      } = me,
      {
        target
      } = event;
    let cellElement = target.closest('.b-grid-cell');
    // If event coords outside of cell, this will match a cell so long as either x or y is inside a cell.
    if (!cellElement && includeSingleAxisMatch && !target.classList.contains('b-grid-row') && !target.classList.contains('b-grid-subgrid')) {
      const {
        top,
        left,
        right,
        bottom
      } = me.bodyContainer.getBoundingClientRect();
      let match,
        {
          x,
          y
        } = event;
      // X axis correct
      if (x >= left && x <= right) {
        // y will match row either at the top or at the bottom, dependent on what the provided x is
        y = match = Math.ceil(me[`${y < top ? 'first' : 'last'}FullyVisibleRow`].element.getBoundingClientRect().y);
      }
      // Y axis correct
      else if (y >= top && y <= bottom) {
        // x will match row either at to the left or to the right, dependent on what the provided y is
        x = match = Math.ceil(columns.visibleColumns[x < left ? 0 : columns.visibleColumns.length - 1].element.getBoundingClientRect().x);
      }
      if (match !== undefined) {
        var _DomHelper$childFromP;
        cellElement = (_DomHelper$childFromP = DomHelper.childFromPoint(event.target, event.offsetX, event.offsetY)) === null || _DomHelper$childFromP === void 0 ? void 0 : _DomHelper$childFromP.closest('.b-grid-cell');
      }
    }
    // There is a cell
    if (cellElement) {
      const cellData = DomDataStore.get(cellElement),
        {
          id,
          columnId
        } = cellData,
        record = me.store.getById(id),
        column = columns.getById(columnId);
      // Row might not have a record, since we transition record removal
      // https://app.assembla.com/spaces/bryntum/tickets/6805
      return record ? {
        cellElement,
        cellData,
        columnId,
        id,
        record,
        column,
        cellSelector: {
          id,
          columnId
        }
      } : null;
    }
  }
  /**
   * This method finds the header location of the passed event. It returns an object describing the header.
   * @param {Event} event A Mouse, Pointer or Touch event targeted at part of the grid.
   * @returns {Object} An object containing the following properties:
   * - `headerElement` - The header element clicked on.
   * - `column` - The {@link Grid.column.Column column} clicked under.
   * - `columnId` - The `id` of the {@link Grid.column.Column column} clicked under.
   * @internal
   * @category Events
   */
  getHeaderDataFromEvent(event) {
    const headerElement = event.target.closest('.b-grid-header');
    // There is a header
    if (headerElement) {
      const headerData = ObjectHelper.assign({}, headerElement.dataset),
        {
          columnId
        } = headerData,
        column = this.columns.getById(columnId);
      return column ? {
        headerElement,
        headerData,
        columnId,
        column
      } : null;
    }
  }
  /**
   * Handles all dom events, routing them to correct functions (touchstart -> onElementTouchStart)
   * @param event
   * @private
   * @category Events
   */
  handleEvent(event) {
    if (!this.disabled && gridBodyElementEventHandlers[event.type]) {
      this[gridBodyElementEventHandlers[event.type]](event);
    }
  }
  //endregion
  //region Touch events
  /**
   * Touch start, chain this function in features to handle the event.
   * @param event
   * @category Touch events
   * @internal
   */
  onElementTouchStart(event) {
    const me = this,
      cellData = me.getCellDataFromEvent(event);
    DomHelper.isTouchEvent = true;
    if (event.touches.length === 1) {
      me.longPressTimeout = me.setTimeout(() => {
        me.onElementLongPress(event);
        event.preventDefault();
        me.longPressPerformed = true;
      }, me.longPressTime);
    }
    if (cellData && !event.defaultPrevented) {
      me.onFocusGesture(event);
    }
  }
  /**
   * Touch move, chain this function in features to handle the event.
   * @param event
   * @category Touch events
   * @internal
   */
  onElementTouchMove(event) {
    const me = this,
      {
        lastTouchTarget
      } = me,
      touch = event.changedTouches[0],
      {
        pageX,
        pageY
      } = touch,
      touchTarget = document.elementFromPoint(pageX, pageY);
    if (me.longPressTimeout) {
      me.clearTimeout(me.longPressTimeout);
      me.longPressTimeout = null;
    }
    // Keep grid informed about mouseover/outs during touch-based dragging
    if (touchTarget !== lastTouchTarget) {
      if (lastTouchTarget) {
        const mouseoutEvent = new MouseEvent('mouseout', ObjectHelper.copyProperties({
          relatedTarget: touchTarget,
          pointerType: 'touch',
          bubbles: true
        }, touch, eventProps));
        mouseoutEvent.preventDefault = () => event.preventDefault();
        lastTouchTarget === null || lastTouchTarget === void 0 ? void 0 : lastTouchTarget.dispatchEvent(mouseoutEvent);
      }
      if (touchTarget) {
        const mouseoverEvent = new MouseEvent('mouseover', ObjectHelper.copyProperties({
          relatedTarget: lastTouchTarget,
          pointerType: 'touch',
          bubbles: true
        }, touch, eventProps));
        mouseoverEvent.preventDefault = () => event.preventDefault();
        touchTarget === null || touchTarget === void 0 ? void 0 : touchTarget.dispatchEvent(mouseoverEvent);
      }
    }
    me.lastTouchTarget = touchTarget;
  }
  /**
   * Touch end, chain this function in features to handle the event.
   * @param event
   * @category Touch events
   * @internal
   */
  onElementTouchEnd(event) {
    const me = this;
    if (me.longPressPerformed) {
      if (event.cancelable) {
        event.preventDefault();
      }
      me.longPressPerformed = false;
    }
    if (me.longPressTimeout) {
      me.clearTimeout(me.longPressTimeout);
      me.longPressTimeout = null;
    }
  }
  onElementLongPress(event) {}
  //endregion
  //region Mouse events
  // Trigger events in same style when clicking, dblclicking and for contextmenu
  triggerCellMouseEvent(name, event, cellData = this.getCellDataFromEvent(event)) {
    const me = this;
    // There is a cell
    if (cellData) {
      const column = me.columns.getById(cellData.columnId),
        eventData = {
          grid: me,
          record: cellData.record,
          column,
          cellSelector: cellData.cellSelector,
          cellElement: cellData.cellElement,
          target: event.target,
          event
        };
      me.trigger('cell' + StringHelper.capitalize(name), eventData);
      if (name === 'click') {
        var _column$onCellClick;
        (_column$onCellClick = column.onCellClick) === null || _column$onCellClick === void 0 ? void 0 : _column$onCellClick.call(column, eventData);
      }
    }
  }
  /**
   * Mouse down, chain this function in features to handle the event.
   * @param event
   * @category Mouse events
   * @internal
   */
  onElementMouseDown(event) {
    const me = this,
      cellData = me.getCellDataFromEvent(event);
    me.skipFocusSelection = true;
    // If click was on a scrollbar or splitter, preventDefault to not steal focus
    if (me.isScrollbarOrRowBorderOrSplitterClick(event)) {
      event.preventDefault();
    } else {
      me.triggerCellMouseEvent('mousedown', event, cellData);
      // Browser event unification fires a mousedown on touch tap prior to focus.
      if (cellData && !event.defaultPrevented) {
        me.onFocusGesture(event);
      }
    }
  }
  isScrollbarOrRowBorderOrSplitterClick({
    target,
    x,
    y
  }) {
    // Normally cells catch the click, directly on row = user clicked border, which we ignore.
    // Also ignore clicks on the virtual width element used to stretch fake scrollbar
    if (target.closest('.b-grid-splitter') || target.matches('.b-grid-row, .b-virtual-width')) {
      return true;
    }
    if (target.matches('.b-vertical-overflow')) {
      const rect = target.getBoundingClientRect();
      return x > rect.right - DomHelper.scrollBarWidth;
    } else if (target.matches('.b-horizontal-overflow')) {
      const rect = target.getBoundingClientRect();
      return y > rect.bottom - DomHelper.scrollBarWidth - 1; // -1 for height of virtualScrollerWidth element
    }
  }
  /**
   * Mouse move, chain this function in features to handle the event.
   * @param event
   * @category Mouse events
   * @internal
   */
  onElementMouseMove(event) {
    // Keep track of the last mouse position in case, due to OSX sloppy focusing,
    // focus is moved into the browser before a mousedown is delivered.
    // The cached mousemove event will provide the correct target in
    // GridNavigation#onGridElementFocus.
    this.mouseMoveEvent = event;
  }
  /**
   * Mouse up, chain this function in features to handle the event.
   * @param event
   * @category Mouse events
   * @internal
   */
  onElementMouseUp(event) {}
  onElementPointerDown(event) {}
  /**
   * Pointer up, chain this function in features to handle the event.
   * @param event
   * @category Mouse events
   * @internal
   */
  onElementPointerUp(event) {}
  /**
   * Called before {@link #function-onElementClick}.
   * Fires 'beforeElementClick' event which can return false to cancel further onElementClick actions.
   * @param event
   * @fires beforeElementClick
   * @category Mouse events
   * @internal
   */
  onHandleElementClick(event) {
    if (this.trigger('beforeElementClick', {
      event
    }) !== false) {
      this.onElementClick(event);
    }
  }
  /**
   * Click, select cell on click and also fire 'cellClick' event.
   * Chain this function in features to handle the dom event.
   * @param event
   * @fires cellClick
   * @category Mouse events
   * @internal
   */
  onElementClick(event) {
    const me = this,
      cellData = me.getCellDataFromEvent(event);
    // There is a cell
    if (cellData) {
      me.triggerCellMouseEvent('click', event, cellData);
    }
  }
  onFocusGesture(event) {
    const me = this,
      {
        navigationEvent
      } = me,
      {
        target
      } = event,
      isContextMenu = event.button === 2,
      // Interaction with tree expand/collapse icons doesn't focus
      isTreeExpander = !isContextMenu && target.matches('.b-icon-tree-expand, .b-icon-tree-collapse'),
      // Mac OS specific behaviour: when you right click a non-active window, the window does not receive focus, but the context menu is shown.
      // So for Mac OS we treat the right click as a non-focusable action, if window is not active
      isUnfocusedRightClick = !document.hasFocus() && BrowserHelper.isMac && isContextMenu;
    // Tree expander clicks and contextmenus on unfocused windows don't focus
    if (isTreeExpander || isUnfocusedRightClick) {
      event.preventDefault();
    } else {
      var _me$focusedCell;
      // Used by the GridNavigation mixin to detect what interaction event if any caused
      // the focus to be moved. If it's a programmatic focus, there won't be one.
      // Grid doesn't use a Navigator which maintains this property, so we need to set it.
      me.navigationEvent = event;
      const location = new Location(target);
      // Context menu doesn't focus by default, so that needs to explicitly focus.
      // If they're re-clicking the current focus, GridNavigation#focusCell
      // still needs to know. It's a no-op, but it informs the GridSelection of the event.
      if (isContextMenu || (_me$focusedCell = me.focusedCell) !== null && _me$focusedCell !== void 0 && _me$focusedCell.equals(location)) {
        let focusOptions;
        // If current event is a MouseEvent and previous event is a TouchEvent on same target, this event is
        // most likely a MouseEvent triggered by a TouchEvent, which should not trigger selection
        if (globalThis.TouchEvent && event instanceof MouseEvent && navigationEvent instanceof TouchEvent && target === navigationEvent.target) {
          focusOptions = {
            doSelect: false
          };
        }
        me.focusCell(location, focusOptions);
      }
    }
  }
  /**
   * Double click, fires 'cellDblClick' event.
   * Chain this function in features to handle the dom event.
   * @param {Event} event
   * @fires cellDblClick
   * @category Mouse events
   * @internal
   */
  onElementDblClick(event) {
    const {
      target
    } = event;
    this.triggerCellMouseEvent('dblClick', event);
    if (target.classList.contains('b-grid-header-resize-handle')) {
      const header = target.closest('.b-grid-header'),
        column = this.columns.getById(header.dataset.columnId);
      column.resizeToFitContent();
    }
  }
  /**
   * Mouse over, adds 'hover' class to elements.
   * @param event
   * @fires mouseOver
   * @category Mouse events
   * @internal
   */
  onElementMouseOver(event) {
    // bail out early if scrolling
    if (!this.scrolling) {
      const me = this,
        {
          hoveredCell
        } = me,
        // No hover effect needed if a mouse button is pressed (like when resizing window, region, or resizing something etc).
        // NOTE: 'buttons' not supported in Safari
        shouldHover = (typeof event.buttons !== 'number' || event.buttons === 0) && event.pointerType !== 'touch';
      let cellElement = event.target.closest('.b-grid-cell');
      // If we are entering a grid row (which probably is a row border), we should hover
      // cell/row above so not to get a blinking hovering, especially on column header
      if (!cellElement && event.target.classList.contains('b-grid-row')) {
        var _DomHelper$childFromP2;
        cellElement = (_DomHelper$childFromP2 = DomHelper.childFromPoint(event.target, event.offsetX, event.offsetY - 2)) === null || _DomHelper$childFromP2 === void 0 ? void 0 : _DomHelper$childFromP2.closest('.b-grid-cell');
      }
      if (cellElement) {
        if (shouldHover) {
          me.hoveredCell = cellElement;
        }
        if (!hoveredCell && me.hoveredCell) {
          me.triggerCellMouseEvent('mouseOver', event);
        }
      }
      /**
       * Mouse moved in over element in grid
       * @event mouseOver
       * @param {MouseEvent} event The native browser event
       */
      me.trigger('mouseOver', {
        event
      });
    }
  }
  /**
   * Mouse out, removes 'hover' class from elements.
   * @param event
   * @fires mouseOut
   * @category Mouse events
   * @internal
   */
  onElementMouseOut(event) {
    const me = this,
      {
        hoveredCell
      } = me,
      {
        target,
        relatedTarget
      } = event;
    if (!(relatedTarget !== null && relatedTarget !== void 0 && relatedTarget.closest('.b-grid-cell') && target.matches('.b-grid-row'))) {
      var _me$hoveredCell;
      // If we have not moved onto a grid row
      // (meaning over its border which is handled by getCellDataFromEvent)
      // hover any new cell we are over.
      if (relatedTarget && target.matches('.b-grid-cell') && !target.contains(relatedTarget)) {
        if (!(relatedTarget !== null && relatedTarget !== void 0 && relatedTarget.matches('.b-grid-row'))) {
          me.hoveredCell = relatedTarget.closest('.b-grid-cell');
        }
      } else if (!(relatedTarget !== null && relatedTarget !== void 0 && relatedTarget.matches('.b-grid-row,.b-grid-cell')) && !((_me$hoveredCell = me.hoveredCell) !== null && _me$hoveredCell !== void 0 && _me$hoveredCell.contains(relatedTarget))) {
        me.hoveredCell = null;
      }
    }
    // bail out early if scrolling
    if (!me.scrolling) {
      if (hoveredCell && !me.hoveredCell) {
        me.triggerCellMouseEvent('mouseOut', event);
      }
      /**
       * Mouse moved out from element in grid
       * @event mouseOut
       * @param {MouseEvent} event The native browser event
       */
      me.trigger('mouseOut', {
        event
      });
    }
  }
  // The may be chained in features
  updateHoveredCell(cellElement, was) {
    var _me$columns$find, _me$checkboxSelection;
    const me = this,
      {
        selectionMode
      } = me,
      rowNumberColumnId = selectionMode.rowNumber && ((_me$columns$find = me.columns.find(c => c.type == 'rownumber')) === null || _me$columns$find === void 0 ? void 0 : _me$columns$find.id),
      checkboxSelectionColumnId = selectionMode.checkbox && ((_me$checkboxSelection = me.checkboxSelectionColumn) === null || _me$checkboxSelection === void 0 ? void 0 : _me$checkboxSelection.id);
    // Clear last hovered cell
    if (was) {
      toggleHover(was, false);
      // Also remove hovered class on checkcol, rownumbercol and column header
      const prevSelector = DomDataStore.get(was),
        {
          row: prevRow
        } = prevSelector;
      if (prevRow && !prevRow.isDestroyed) {
        setCellHover(rowNumberColumnId, prevRow, false);
        setCellHover(checkboxSelectionColumnId, prevRow, false);
      }
      if (prevSelector !== null && prevSelector !== void 0 && prevSelector.columnId) {
        var _me$columns$getById;
        toggleHover((_me$columns$getById = me.columns.getById(prevSelector.columnId)) === null || _me$columns$getById === void 0 ? void 0 : _me$columns$getById.element, false);
      }
    }
    // Clears hovered row
    // Only remove cls if row isn't destroyed
    if (me._hoveredRow && !me._hoveredRow.isDestroyed) {
      me._hoveredRow.removeCls('b-hover');
    }
    me._hoveredRow = null;
    // Set hovered
    if (cellElement && !me.scrolling) {
      const selector = DomDataStore.get(cellElement),
        {
          row
        } = selector;
      if (row) {
        // Set cell if cell selection mode is on
        if (selectionMode.cell && selector.columnId !== rowNumberColumnId && selector.columnId !== checkboxSelectionColumnId) {
          var _me$columns$getById2;
          toggleHover(cellElement);
          // In cell selection mode:
          // Also "hover" checkcolumn cell if such exists
          setCellHover(checkboxSelectionColumnId, row);
          // And also rownumbercolumn cell
          setCellHover(rowNumberColumnId, row);
          // And also column header
          toggleHover((_me$columns$getById2 = me.columns.getById(selector.columnId)) === null || _me$columns$getById2 === void 0 ? void 0 : _me$columns$getById2.element);
        }
        // Else row
        else {
          me._hoveredRow = row;
          row.addCls('b-hover');
        }
      } else {
        me.hoveredCell = null;
      }
    }
  }
  //endregion
  //region Keyboard events
  // Hooks on to keyMaps keydown-listener to be able to run before
  keyMapOnKeyDown(event) {
    this.onElementKeyDown(event);
    super.keyMapOnKeyDown(event);
  }
  /**
   * To catch all keydowns. For more specific keydown actions, use keyMap.
   * @param event
   * @category Keyboard events
   * @internal
   */
  onElementKeyDown(event) {
    // If some other function flagged the event as handled, we ignore it.
    if (event.handled) {
      return;
    }
    const me = this,
      // Read this to refresh cached reference in case this keystroke lead to the removal of current row
      focusedCell = me.focusedCell;
    if (focusedCell !== null && focusedCell !== void 0 && focusedCell.isCell && !focusedCell.isActionable) {
      var _me$columns$getById$o, _me$columns$getById3;
      const cellElement = focusedCell.cell;
      // If a cell is focused and column is interested - call special callback
      (_me$columns$getById$o = (_me$columns$getById3 = me.columns.getById(cellElement.dataset.columnId)).onCellKeyDown) === null || _me$columns$getById$o === void 0 ? void 0 : _me$columns$getById$o.call(_me$columns$getById3, {
        event,
        cellElement
      });
    }
  }
  undoRedoKeyPress(event) {
    var _this$features$cellEd;
    const {
      stm
    } = this.store;
    if (stm && this.enableUndoRedoKeys && !((_this$features$cellEd = this.features.cellEdit) !== null && _this$features$cellEd !== void 0 && _this$features$cellEd.isEditing)) {
      stm.onUndoKeyPress(event);
      return true;
    }
    return false;
  }
  // Trigger column.onCellClick when space bar is pressed
  clickCellByKey(event) {
    const me = this,
      // Read this to refresh cached reference in case this keystroke lead to the removal of current row
      focusedCell = me.focusedCell,
      cellElement = focusedCell === null || focusedCell === void 0 ? void 0 : focusedCell.cell,
      column = me.columns.getById(cellElement.dataset.columnId);
    if (focusedCell !== null && focusedCell !== void 0 && focusedCell.isCell && !focusedCell.isActionable) {
      if (column.onCellClick) {
        column.onCellClick({
          grid: me,
          column,
          record: me.store.getById(focusedCell.id),
          cellElement,
          target: event.target,
          event
        });
        return true;
      }
    }
    return false;
  }
  /**
   * Key press, chain this function in features to handle the dom event.
   * @param event
   * @category Keyboard events
   * @internal
   */
  onElementKeyPress(event) {}
  /**
   * Key up, chain this function in features to handle the dom event.
   * @param event
   * @category Keyboard events
   * @internal
   */
  onElementKeyUp(event) {}
  //endregion
  //region Other events
  /**
   * Context menu, chain this function in features to handle the dom event.
   * In most cases, include ContextMenu feature instead.
   * @param event
   * @category Other events
   * @internal
   */
  onElementContextMenu(event) {
    const me = this,
      cellData = me.getCellDataFromEvent(event);
    // There is a cell
    if (cellData) {
      me.triggerCellMouseEvent('contextMenu', event, cellData);
      // Focus on tap for touch events.
      // Selection follows from focus.
      if (DomHelper.isTouchEvent) {
        me.onFocusGesture(event);
      }
    }
  }
  /**
   * Overrides empty base function in View, called when view is resized.
   * @fires resize
   * @param element
   * @param width
   * @param height
   * @param oldWidth
   * @param oldHeight
   * @category Other events
   * @internal
   */
  onInternalResize(element, width, height, oldWidth, oldHeight) {
    const me = this;
    if (me._devicePixelRatio && me._devicePixelRatio !== globalThis.devicePixelRatio) {
      // Pixel ratio changed, likely because of browser zoom. This affects the relative scrollbar width also
      DomHelper.resetScrollBarWidth();
    }
    me._devicePixelRatio = globalThis.devicePixelRatio;
    // cache to avoid recalculations in the middle of rendering code (RowManger#getRecordCoords())
    me._bodyRectangle = Rectangle.client(me.bodyContainer);
    super.onInternalResize(...arguments);
    if (height !== oldHeight) {
      me._bodyHeight = me.bodyContainer.offsetHeight;
      if (me.isPainted) {
        // initial height will be set from render(),
        // it reaches onInternalResize too early when rendering, headers/footers are not sized yet
        me.rowManager.initWithHeight(me._bodyHeight);
      }
    }
    me.refreshVirtualScrollbars();
    if (width !== oldWidth) {
      // Slightly delay to avoid resize loops.
      me.setTimeout(() => {
        if (!me.isDestroyed) {
          me.updateResponsive(width, oldWidth);
        }
      }, 0);
    }
  }
  //endregion
  // This does not need a className on Widgets.
  // Each *Class* which doesn't need 'b-' + constructor.name.toLowerCase() automatically adding
  // to the Widget it's mixed in to should implement thus.
  get widgetClass() {}
});

/**
 * @module Grid/view/mixin/GridFeatures
 */
const validConfigTypes = {
  string: 1,
  object: 1,
  function: 1 // used by CellTooltip
};
/**
 * Mixin for Grid that handles features. Features are plugins that add functionality to the grid. Feature classes should
 * register with Grid by calling {@link Grid.feature.GridFeatureManager#function-registerFeature-static registerFeature}. This
 * enables features to be specified and configured in grid
 * config.
 *
 * Define which features to use:
 *
 * ```javascript
 * // specify which features to use (note that some features are used by default)
 * const grid = new Grid({
 *   features: {
 *      sort: 'name',
 *      search: true
 *   }
 * });
 * ```
 *
 * Access a feature in use:
 *
 * ```javascript
 * grid.features.search.search('cat');
 * ```
 *
 * Basic example of implementing a feature:
 *
 * ```javascript
 * class MyFeature extends InstancePlugin {
 *
 * }
 *
 * GridFeatures.registerFeature(MyFeature);
 *
 * // using the feature
 * const grid = new Grid({
 *   features: {
 *     myFeature: true
 *   }
 * });
 * ```
 *
 * ## Enable and disable features at runtime
 *
 * Each feature is either "enabled" (included by default), or "off" (excluded completely). You can always check the docs
 * of a specific feature to find out how it is configured by default.
 *
 * Features which are "off" completely are not available and cannot be enabled at runtime.
 *
 * For a feature that is **off** by default that you want to enable later during runtime,
 * configure it with `disabled : true`:
 * ```javascript
 * const grid = new Grid({
 *      featureName : {
 *          disabled : true // on and disabled, can be enabled later
 *      }
 * });
 *
 * // enable the feature
 * grid.featureName.disabled = false;
 * ```
 *
 * If the feature is **off** by default, and you want to include and enable the feature, configure it as `true`:
 * ```javascript
 * const grid = new Grid({
 *      featureName : true // on and enabled, can be disabled later
 * });
 *
 * // disable the feature
 * grid.featureName.disabled = true;
 * ```
 *
 * If the feature is **on** by default, but you want to turn it **off**, configure it as `false`:
 * ```javascript
 * const grid = new Grid({
 *      featureName : false // turned off, not included at all
 * });
 * ```
 *
 * If the feature is **enabled** by default and you have no need of reconfiguring it,
 * you can omit the feature configuration.
 *
 * @mixin
 */
var GridFeatures = (Target => class GridFeatures extends (Target || Base) {
  static get $name() {
    return 'GridFeatures';
  }
  //region Init
  /**
   * Specify which features to use on the grid. Most features accepts a boolean, some also accepts a config object.
   * Please note that if you are not using the bundles you might need to import the features you want to use.
   *
   * ```javascript
   * const grid = new Grid({
   *     features : {
   *         stripe : true,   // Enable stripe feature
   *         sort   : 'name', // Configure sort feature
   *         group  : false   // Disable group feature
   *     }
   * }
   * ```
   *
   * @config {Object} features
   * @category Common
   */
  /**
   * Map of the features available on the grid. Use it to access them on your grid object
   *
   * ```javascript
   * grid.features.group.expandAll();
   * ```
   *
   * @readonly
   * @member {Object} features
   * @category Common
   */
  set features(features) {
    const me = this,
      defaultFeatures = GridFeatureManager.getInstanceDefaultFeatures(this);
    features = me._features = ObjectHelper.assign({}, features);
    // default features, enabled unless otherwise specified
    if (defaultFeatures) {
      Object.keys(defaultFeatures).forEach(feature => {
        if (!(feature in features)) {
          features[feature] = true;
        }
      });
    }
    // We *prime* the features so that if any configuration code accesses a feature, it
    // will self initialize, but if not, they will remain in a primed state until afterConfigure.
    const registeredInstanceFeatures = GridFeatureManager.getInstanceFeatures(this);
    for (const featureName of Object.keys(features)) {
      const config = features[featureName];
      // Create feature initialization property if config is truthy.
      // Config must be a valid configuration value for the feature class.
      if (config) {
        const throwIfError = !globalThis.__bryntum_code_editor_changed;
        // Feature configs name must start with lowercase letter to be valid
        if (StringHelper.uncapitalize(featureName) !== featureName) {
          const errorMessage = `Invalid feature name '${featureName}', must start with a lowercase letter`;
          if (throwIfError) {
            throw new Error(errorMessage);
          }
          console.error(errorMessage);
          me._errorDuringConfiguration = errorMessage;
        }
        const featureClass = registeredInstanceFeatures[featureName];
        if (!featureClass) {
          const errorMessage = `Feature '${featureName}' not available, make sure you have imported it`;
          if (throwIfError) {
            throw new Error(errorMessage);
          }
          console.error(errorMessage);
          me._errorDuringConfiguration = errorMessage;
          return;
        }
        // Create a self initializing property on the features object named by the feature name.
        // when accessed, it will create and return the real feature.
        // Now, if some Feature initialization code attempt to access a feature which has not yet been initialized
        // it will be initialized just in time.
        Reflect.defineProperty(features, featureName, me.createFeatureInitializer(features, featureName, featureClass, config));
      }
    }
  }
  get features() {
    return this._features;
  }
  createFeatureInitializer(features, featureName, featureClass, config) {
    const constructorArgs = [this],
      construct = featureClass.prototype.construct;
    // Config arg must be processed if feature is just requested with true
    // so that default configurable values are processed.
    if (config === true) {
      config = {};
    }
    // Only pass config if there is one.
    // The constructor(config = {}) only works for undefined config
    if (validConfigTypes[typeof config]) {
      constructorArgs[1] = config;
    }
    return {
      configurable: true,
      get() {
        // Delete this defined property and replace it with the Feature instance.
        delete features[featureName];
        // Ensure the feature is injected into the features object before initialization
        // so that it is available from call chains from its initialization.
        featureClass.prototype.construct = function (...args) {
          features[featureName] = this;
          construct.apply(this, args);
          featureClass.prototype.construct = construct;
        };
        // Return the Feature instance
        return new featureClass(...constructorArgs);
      }
    };
  }
  //endregion
  //region Other stuff
  /**
   * Check if a feature is included
   * @param {String} name Feature name, as registered with `GridFeatureManager.registerFeature()`
   * @returns {Boolean}
   * @category Misc
   */
  hasFeature(name) {
    const {
      features
    } = this;
    if (features) {
      const featureProp = Object.getOwnPropertyDescriptor(this.features, name);
      if (featureProp) {
        // Do not actually force creation of the feature
        return Boolean(featureProp.value || featureProp.get);
      }
    }
    return false;
  }
  hasActiveFeature(name) {
    var _this$features, _this$features2;
    return Boolean(((_this$features = this.features) === null || _this$features === void 0 ? void 0 : _this$features[name]) && !((_this$features2 = this.features) !== null && _this$features2 !== void 0 && _this$features2[name].disabled));
  }
  //endregion
  //region Extract config
  // This function is not meant to be called by any code other than Base#getCurrentConfig().
  // It extracts the current configs for the features
  getConfigValue(name, options) {
    if (name === 'features') {
      const result = {};
      for (const feature in this.features) {
        var _this$features$featur, _this$features$featur2, _this$features$featur3;
        // Feature might be configured as `false`
        const featureConfig = (_this$features$featur = this.features[feature]) === null || _this$features$featur === void 0 ? void 0 : (_this$features$featur2 = (_this$features$featur3 = _this$features$featur).getCurrentConfig) === null || _this$features$featur2 === void 0 ? void 0 : _this$features$featur2.call(_this$features$featur3, options);
        if (featureConfig) {
          // Use `true` for empty feature configs `{ stripe : true }`
          if (ObjectHelper.isEmpty(featureConfig)) {
            // Exclude default features to not spam the config
            if (!GridFeatureManager.isDefaultFeatureForInstance(this.features[feature].constructor, this)) {
              result[feature] = true;
            }
          } else {
            result[feature] = featureConfig;
          }
        } else {
          result[feature] = false;
        }
      }
      return result;
    }
    return super.getConfigValue(name, options);
  }
  //endregion
  // This does not need a className on Widgets.
  // Each *Class* which doesn't need 'b-' + constructor.name.toLowerCase() automatically adding
  // to the Widget it's mixed in to should implement thus.
  get widgetClass() {}
});

/**
 * @module Grid/view/mixin/GridNavigation
 */
const defaultFocusOptions = Object.freeze({}),
  disableScrolling = Object.freeze({
    x: false,
    y: false
  }),
  containedFocusable = function (e) {
    // When we step outside of the target cell, throw.
    // The TreeWalker silences the exception and terminates the traverse.
    if (!this.focusableFinderCell.contains(e)) {
      return DomHelper.NodeFilter.FILTER_REJECT;
    }
    if (DomHelper.isFocusable(e) && !e.disabled) {
      return DomHelper.NodeFilter.FILTER_ACCEPT;
    }
    return DomHelper.NodeFilter.FILTER_SKIP;
  };
/**
 * Mixin for Grid that handles cell to cell navigation.
 *
 * See {@link Grid.view.Grid} for more information on grid cell keyboard navigation.
 *
 * @mixin
 */
var GridNavigation = (Target => class GridNavigation extends (Target || Base) {
  static get $name() {
    return 'GridNavigation';
  }
  static configurable = {
    focusable: false,
    focusableSelector: '.b-grid-cell,.b-grid-header.b-depth-0',
    // Documented on Grid
    keyMap: {
      ArrowUp: {
        handler: 'navigateUp',
        weight: 10
      },
      ArrowRight: {
        handler: 'navigateRight',
        weight: 10
      },
      ArrowDown: {
        handler: 'navigateDown',
        weight: 10
      },
      ArrowLeft: {
        handler: 'navigateLeft',
        weight: 10
      },
      'Ctrl+Home': 'navigateFirstCell',
      Home: 'navigateFirstColumn',
      'Ctrl+End': 'navigateLastCell',
      End: 'navigateLastColumn',
      PageUp: 'navigatePrevPage',
      PageDown: 'navigateNextPage',
      Enter: 'activateHeader',
      // Private
      Escape: {
        handler: 'onEscape',
        weight: 10
      },
      'Shift+Tab': {
        handler: 'onShiftTab',
        preventDefault: false
      },
      Tab: {
        handler: 'onTab',
        preventDefault: false
      },
      ' ': {
        handler: 'onSpace',
        preventDefault: false
      }
    }
  };
  onStoreRecordIdChange(event) {
    var _super$onStoreRecordI;
    (_super$onStoreRecordI = super.onStoreRecordIdChange) === null || _super$onStoreRecordI === void 0 ? void 0 : _super$onStoreRecordI.call(this, event);
    const {
        focusedCell
      } = this,
      {
        oldValue,
        value
      } = event;
    // https://github.com/bryntum/support/issues/4935
    if (focusedCell && focusedCell.id === oldValue) {
      focusedCell._id = value;
    }
  }
  /**
   * Called by the RowManager when the row which contains the focus location is derendered.
   *
   * This keeps focus in a consistent place.
   * @protected
   */
  onFocusedRowDerender() {
    const me = this,
      {
        focusedCell
      } = me;
    if ((focusedCell === null || focusedCell === void 0 ? void 0 : focusedCell.id) != null && focusedCell.cell) {
      const isActive = focusedCell.cell.contains(DomHelper.getActiveElement(me));
      if (me.hideHeaders) {
        if (isActive) {
          me.revertFocus();
        }
      } else {
        const headerContext = me.normalizeCellContext({
          rowIndex: -1,
          columnIndex: isActive ? focusedCell.columnIndex : 0
        });
        // The row contained focus, focus the corresponding header
        if (isActive) {
          me.focusCell(headerContext);
        } else {
          headerContext.cell.tabIndex = 0;
        }
      }
      focusedCell.cell.tabIndex = -1;
    }
  }
  navigateFirstCell() {
    this.focusCell(Location.FIRST_CELL);
  }
  navigateFirstColumn() {
    this.focusCell(Location.FIRST_COLUMN);
  }
  navigateLastCell() {
    this.focusCell(Location.LAST_CELL);
  }
  navigateLastColumn() {
    this.focusCell(Location.LAST_COLUMN);
  }
  navigatePrevPage() {
    this.focusCell(Location.PREV_PAGE);
  }
  navigateNextPage() {
    this.focusCell(Location.NEXT_PAGE);
  }
  activateHeader(keyEvent) {
    if (keyEvent.target.classList.contains('b-grid-header') && this.focusedCell.isColumnHeader) {
      var _column$onKeyDown;
      const {
        column
      } = this.focusedCell;
      (_column$onKeyDown = column.onKeyDown) === null || _column$onKeyDown === void 0 ? void 0 : _column$onKeyDown.call(column, keyEvent);
      this.getHeaderElement(column.id).click();
    }
    return false;
  }
  onEscape(keyEvent) {
    const {
      focusedCell
    } = this;
    if (!keyEvent.target.closest('.b-dragging') && focusedCell !== null && focusedCell !== void 0 && focusedCell.isActionable) {
      // The escape must not be processed by handlers for the cell we are about to focus.
      // We need to just push focus upwards to the cell, and stop there.
      keyEvent.stopImmediatePropagation();
      // To prevent the focusCell from being rejected as a no-op
      this._focusedCell = null;
      // Focus the cell with an explicit request to not jump in
      this.focusCell({
        rowIndex: focusedCell.rowIndex,
        column: focusedCell.column
      }, {
        disableActionable: true
      });
    }
  }
  onTab(keyEvent) {
    const {
        target
      } = keyEvent,
      {
        focusedCell,
        bodyElement
      } = this,
      {
        isActionable,
        actionTargets
      } = focusedCell,
      isEditable = isActionable && DomHelper.isEditable(target) && !target.readOnly;
    // If we're on the last editable in a cell, TAB navigates right
    if (isEditable && target === actionTargets[actionTargets.length - 1]) {
      keyEvent.preventDefault();
      this.navigateRight(keyEvent);
    }
    // If we're *on* a cell, or on last subtarget, TAB moves off the grid.
    // Temporarily hide the grid body, and let TAB take effect from there
    else if (!isActionable || target === actionTargets[actionTargets.length - 1]) {
      bodyElement.style.display = 'none';
      this.requestAnimationFrame(() => bodyElement.style.display = '');
      // So that Navigator#onKeyDown does not continue to preventDefault;
      return false;
    }
  }
  onShiftTab(keyEvent) {
    const me = this,
      {
        target
      } = keyEvent,
      {
        focusedCell,
        bodyElement
      } = me,
      {
        cell,
        isActionable,
        actionTargets
      } = focusedCell,
      isEditable = isActionable && DomHelper.isEditable(target) && !target.readOnly,
      onFirstCell = focusedCell.columnIndex === 0 && focusedCell.rowIndex === (me.hideHeaders ? 0 : -1);
    // If we're on the first editable in a cell that is not the first cell, SHIFT+TAB navigates left
    if (!onFirstCell && isEditable && target === actionTargets[0]) {
      keyEvent.preventDefault();
      me.navigateLeft(keyEvent);
    }
    // If we're *on* a cell, or on first subtarget, SHIFT+TAB moves off the grid.
    else if (!isActionable || target === actionTargets[0]) {
      // Focus the first header cell and then let the key's default action take its course
      const f = !onFirstCell && !me.hideHeaders && me.focusCell({
        rowIndex: -1,
        column: 0
      }, {
        disableActionable: true
      });
      // If that was successful then reset the tabIndex
      if (f) {
        f.cell.tabIndex = -1;
        cell.tabIndex = 0;
        me._focusedCell = focusedCell;
      }
      // Otherwise, temporarily hide the grid body, and let TAB take effect from there
      else {
        bodyElement.style.display = 'none';
        me.requestAnimationFrame(() => bodyElement.style.display = '');
      }
      // So that Navigator#onKeyDown does not continue to preventDefault;
      return false;
    }
  }
  onSpace(keyEvent) {
    // SPACE scrolls, so disable that
    if (!this.focusedCell.isActionable) {
      keyEvent.preventDefault();
    }
    // Return false to tell keyMap that any other actions should be called
    return false;
  }
  //region Cell
  /**
   * Triggered when a user navigates to a grid cell
   * @event navigate
   * @param {Grid.view.Grid} grid The grid instance
   * @param {Grid.util.Location} last The previously focused location
   * @param {Grid.util.Location} location The new focused location
   * @param {Event} [event] The UI event which caused navigation.
   */
  /**
   * Grid Location which encapsulates the currently focused cell.
   * Set to focus a cell or use {@link #function-focusCell}.
   * @property {Grid.util.Location}
   */
  get focusedCell() {
    return this._focusedCell;
  }
  /**
   * This property is `true` if an element _within_ a cell is focused.
   * @property {Boolean}
   * @readonly
   */
  get isActionableLocation() {
    var _this$_focusedCell;
    return (_this$_focusedCell = this._focusedCell) === null || _this$_focusedCell === void 0 ? void 0 : _this$_focusedCell.isActionable;
  }
  set focusedCell(cellSelector) {
    this.focusCell(cellSelector);
  }
  get focusedRecord() {
    var _this$_focusedCell2;
    return (_this$_focusedCell2 = this._focusedCell) === null || _this$_focusedCell2 === void 0 ? void 0 : _this$_focusedCell2.record;
  }
  /**
   * CSS selector for currently focused cell. Format is "[data-index=index] [data-column-id=columnId]".
   * @property {String}
   * @readonly
   */
  get cellCSSSelector() {
    const cell = this._focusedCell;
    return cell ? `[data-index=${cell.rowIndex}] [data-column-id=${cell.columnId}]` : '';
  }
  afterHide() {
    super.afterHide(...arguments);
    // Do not scroll back to the last focused cell/last moused over cell upon reshow
    this.lastFocusedCell = null;
  }
  /**
   * Checks whether a cell is focused.
   * @param {LocationConfig|String|Number} cellSelector Cell selector { id: x, columnId: xx } or row id
   * @returns {Boolean} true if cell or row is focused, otherwise false
   */
  isFocused(cellSelector) {
    var _this$_focusedCell3;
    return Boolean((_this$_focusedCell3 = this._focusedCell) === null || _this$_focusedCell3 === void 0 ? void 0 : _this$_focusedCell3.equals(this.normalizeCellContext(cellSelector)));
  }
  get focusElement() {
    if (!this.isDestroying) {
      let focusCell;
      // If the store is not empty, focusedCell can return the closest cell
      if (this.store.count && this._focusedCell) {
        focusCell = this._focusedCell.target;
      }
      // If the store is empty, or we have had no focusedCell set, focus a column header.
      else {
        var _this$_focusedCell4;
        focusCell = this.normalizeCellContext({
          rowIndex: -1,
          columnIndex: ((_this$_focusedCell4 = this._focusedCell) === null || _this$_focusedCell4 === void 0 ? void 0 : _this$_focusedCell4.columnIndex) || 0
        }).target;
      }
      const superFocusEl = super.focusElement;
      // If there's no cell, or the Container's focus element is before the cell
      // use the Container's focus element.
      // For example, we may have a top toolbar.
      if (superFocusEl && (!focusCell || focusCell.compareDocumentPosition(superFocusEl) === Node.DOCUMENT_POSITION_PRECEDING)) {
        return superFocusEl;
      }
      return focusCell;
    }
  }
  onPaint({
    firstPaint
  }) {
    var _super$onPaint;
    const me = this;
    (_super$onPaint = super.onPaint) === null || _super$onPaint === void 0 ? void 0 : _super$onPaint.call(this, ...arguments);
    // Make the grid initally tabbable into.
    // The first cell has to have the initial roving tabIndex set into it.
    const defaultFocus = this.normalizeCellContext({
      rowIndex: me.hideHeaders ? 0 : -1,
      column: me.hideHeaders ? 0 : me.columns.find(col => !col.hidden && col.isFocusable)
    });
    if (defaultFocus.cell) {
      me._focusedCell = defaultFocus;
      const {
        target
      } = defaultFocus;
      // If cell doesn't contain a focusable target, it needs tabIndex 0.
      if (target === defaultFocus.cell) {
        defaultFocus.cell.tabIndex = 0;
      }
    }
  }
  /**
   * This function handles focus moving into, or within the grid.
   * @param {Event} focusEvent
   * @private
   */
  onGridBodyFocusIn(focusEvent) {
    const me = this,
      {
        bodyElement
      } = me,
      lastFocusedCell = me.focusedCell,
      lastTarget = (lastFocusedCell === null || lastFocusedCell === void 0 ? void 0 : lastFocusedCell.initialTarget) || (lastFocusedCell === null || lastFocusedCell === void 0 ? void 0 : lastFocusedCell.target),
      {
        target,
        relatedTarget
      } = focusEvent,
      targetCell = target.closest(me.focusableSelector);
    // If focus moved into a valid cell...
    // Only allows mouse left och right clicks (no other mouse buttons)
    if (targetCell && (!GlobalEvents.currentMouseDown || GlobalEvents.isMouseDown(0) || GlobalEvents.isMouseDown(2))) {
      var _me$onCellNavigate;
      const cellSelector = new Location(target),
        {
          cell
        } = cellSelector,
        lastCell = lastFocusedCell === null || lastFocusedCell === void 0 ? void 0 : lastFocusedCell.cell,
        actionTargets = cellSelector.actionTargets = me.findFocusables(targetCell),
        // Don't select on focus on a contained actionable location
        doSelect = (!me._fromFocusCell || me.selectOnFocus) && (target === cell || me._selectActionCell) && !(target !== null && target !== void 0 && target._isRevertingFocus);
      // https://github.com/bryntum/support/issues/4039
      // Only try focusing cell is current target cell is getting removed
      if (!me.store.getById(targetCell.parentNode.dataset.id) && cell !== targetCell) {
        cell.focus({
          preventScroll: true
        });
        return;
      }
      if (target.matches(me.focusableSelector)) {
        if (me.disableActionable) {
          cellSelector._target = cell;
        }
        // Focus first focusable target if we are configured to.
        else if (actionTargets.length) {
          var _GlobalEvents$current;
          me._selectActionCell = ((_GlobalEvents$current = GlobalEvents.currentMouseDown) === null || _GlobalEvents$current === void 0 ? void 0 : _GlobalEvents$current.target) === target;
          actionTargets[0].focus();
          delete me._selectActionCell;
          return;
        }
      } else {
        var _GlobalEvents$current2;
        // If we have tabbed in and *NOT* mousedowned in, and hit a tabbable element which was not our
        // last focused cell, go back to last focused cell.
        if (lastFocusedCell !== null && lastFocusedCell !== void 0 && lastFocusedCell.target && relatedTarget && (!GlobalEvents.isMouseDown() || !bodyElement.contains((_GlobalEvents$current2 = GlobalEvents.currentMouseDown) === null || _GlobalEvents$current2 === void 0 ? void 0 : _GlobalEvents$current2.target)) && !bodyElement.contains(relatedTarget) && !cellSelector.equals(lastFocusedCell)) {
          lastTarget.focus();
          return;
        }
        cellSelector._target = target;
      }
      if (lastCell) {
        lastCell.classList.remove('b-focused');
        lastCell.tabIndex = -1;
      }
      if (cell) {
        cell.classList.add('b-focused');
        // Column may update DOM on cell focus for A11Y purposes.
        cellSelector.column.onCellFocus(cellSelector);
        // Only switch the cell to be tabbable if focus was not directed to an inner focusable.
        if (cell === target) {
          cell.tabIndex = 0;
        }
        // Moving back to a cell from a cell-contained Editor
        if (cell.contains(focusEvent.relatedTarget)) {
          if (lastTarget === target) {
            return;
          }
        }
      }
      //Remember
      me._focusedCell = cellSelector;
      (_me$onCellNavigate = me.onCellNavigate) === null || _me$onCellNavigate === void 0 ? void 0 : _me$onCellNavigate.call(me, me, lastFocusedCell, cellSelector, doSelect);
      me.trigger('navigate', {
        lastFocusedCell,
        focusedCell: cellSelector,
        event: focusEvent
      });
    }
    // Focus not moved into a valid cell, refocus last cell's target
    // if there was a previously focused cell.
    // Allow text selection inside a row expander body
    else if (!target.closest('.b-rowexpander-body')) {
      lastTarget === null || lastTarget === void 0 ? void 0 : lastTarget.focus();
    }
  }
  findFocusables(cell) {
    const {
        focusableFinder
      } = this,
      result = [];
    focusableFinder.currentNode = this.focusableFinderCell = cell;
    for (let focusable = focusableFinder.nextNode(); focusable; focusable = focusableFinder.nextNode()) {
      result.push(focusable);
    }
    return result;
  }
  get focusableFinder() {
    const me = this;
    if (!me._focusableFinder) {
      me._focusableFinder = me.setupTreeWalker(me.bodyElement, DomHelper.NodeFilter.SHOW_ELEMENT, {
        acceptNode: containedFocusable.bind(me)
      });
    }
    return me._focusableFinder;
  }
  /**
   * Sets the passed record as the current focused record for keyboard navigation and selection purposes.
   * This API is used by Combo to activate items in its picker.
   * @param {Core.data.Model|Number|String} activeItem The record, or record index, or record id to highlight as the active ("focused") item.
   * @internal
   */
  restoreActiveItem(item = this._focusedCell) {
    if (this.rowManager.count) {
      // They sent a row number.
      if (!isNaN(item)) {
        item = this.store.getAt(item);
      }
      // Still not a record, treat it as a record ID.
      else if (!item.isModel) {
        item = this.store.getById(item);
      }
      return this.focusCell(item);
    }
  }
  /**
   * Navigates to a cell and/or its row (depending on selectionMode)
   * @param {LocationConfig} cellSelector Cell location descriptor
   * @param {Object} options Modifier options for how to deal with focusing the cell. These
   * are used as the {@link Core.helper.util.Scroller#function-scrollTo} options.
   * @param {ScrollOptions|Boolean} [options.scroll=true] Pass `false` to not scroll the cell into view, or a
   * scroll options object to affect the scroll.
   * @returns {Grid.util.Location} A Location object representing the focused location.
   * @fires navigate
   */
  focusCell(cellSelector, options = defaultFocusOptions) {
    var _cellSelector, _cellSelector2;
    const me = this,
      {
        _focusedCell
      } = me,
      {
        scroll,
        disableActionable
      } = options;
    // If we're being asked to go to a nonexistent header row, revert focus outwards
    if (((_cellSelector = cellSelector) === null || _cellSelector === void 0 ? void 0 : _cellSelector.rowIndex) === -1 && me.hideHeaders) {
      me.revertFocus();
      return;
    }
    // Get a Grid Location.
    // If the cellSelector is a number, it is taken to be a "relative" location as defined
    // in the Location class eg Location.UP, and we move the current focus accordingly.
    cellSelector = typeof cellSelector === 'number' && _focusedCell !== null && _focusedCell !== void 0 && _focusedCell.isLocation ? _focusedCell.move(cellSelector) : me.normalizeCellContext(cellSelector);
    const doSelect = 'doSelect' in options ? options.doSelect : !cellSelector.isActionable || cellSelector.initialTarget === cellSelector.cell;
    // Request is a no-op, but it's still a navigate request which selection processing needs to know about
    if (cellSelector.equals(_focusedCell)) {
      var _me$onCellNavigate2;
      (_me$onCellNavigate2 = me.onCellNavigate) === null || _me$onCellNavigate2 === void 0 ? void 0 : _me$onCellNavigate2.call(me, me, _focusedCell, cellSelector, doSelect);
      return _focusedCell;
    }
    const subGrid = me.getSubGridFromColumn(cellSelector.columnId),
      {
        cell
      } = cellSelector,
      testCell = cell || me.getCell({
        rowIndex: me.rowManager.topIndex,
        columnId: cellSelector.columnId
      }),
      subGridRect = Rectangle.from(subGrid.element),
      bodyRect = Rectangle.from(me.bodyElement),
      cellRect = Rectangle.from(testCell).moveTo(null, subGridRect.y);
    // No scrolling possible if we're moving to a column header
    if (scroll === false || cellSelector.rowIndex === -1) {
      options = Object.assign({}, options, disableScrolling);
    } else {
      options = Object.assign({}, options, scroll);
      // If the test cell is larger than the subGrid, in any dimension, disable scrolling
      if (cellRect.width > subGridRect.width || cellRect.height > bodyRect.height) {
        options.x = options.y = false;
      }
      // Else ask for the column to be scrolled into view
      else {
        options.column = cellSelector.columnId;
      }
      me.scrollRowIntoView(cellSelector.id, options);
    }
    // Clear hovering upon navigating so to not have hover style stick around when keyboard navigating away
    if (me._hoveredRow || me.hoveredCell) {
      me.hoveredCell = null;
    }
    // Disable auto stepping into the focused cell.
    me.disableActionable = disableActionable;
    // Go through select pathway upon focus
    me.selectOnFocus = doSelect;
    // To let onGridBodyFocusIn know where the focus originates
    me._fromFocusCell = true;
    // Focus the location's target, be it a cell, or an interior element.
    // The onFocusIn element in this module responds to this.
    (_cellSelector2 = cellSelector[disableActionable ? 'cell' : 'target']) === null || _cellSelector2 === void 0 ? void 0 : _cellSelector2.focus({
      preventScroll: true
    });
    me.disableActionable = me.selectOnFocus = false;
    delete me._fromFocusCell;
    return cellSelector;
  }
  blurCell(cellSelector) {
    const me = this,
      cell = me.getCell(cellSelector);
    if (cell) {
      cell.classList.remove('b-focused');
    }
  }
  clearFocus(fullClear) {
    const me = this;
    if (me._focusedCell) {
      // set last to have focus return to previous cell when alt tabbing
      me.lastFocusedCell = fullClear ? null : me._focusedCell;
      me.blurCell(me._focusedCell);
      me._focusedCell = null;
    }
  }
  /**
   * Selects the cell before or after currently focused cell.
   * @private
   * @param next Specify true to select the next cell, false to select the previous
   * @returns {Object} Used cell selector
   */
  internalNextPrevCell(next = true) {
    const me = this,
      cellSelector = me._focusedCell;
    if (cellSelector) {
      return me.focusCell({
        id: cellSelector.id,
        columnId: me.columns.getAdjacentVisibleLeafColumn(cellSelector.columnId, next, true).id
      });
    }
    return null;
  }
  /**
   * Select the cell after the currently focused one.
   * @param {Event} [event] [DEPRECATED] unused param
   * @returns {Grid.util.Location} Cell selector
   */
  navigateRight() {
    var _arguments$;
    if ((_arguments$ = arguments[0]) !== null && _arguments$ !== void 0 && _arguments$.fromKeyMap) {
      return this.focusCell(this.rtl ? Location.PREV_CELL : Location.NEXT_CELL);
    }
    if (arguments[0]) {
      VersionHelper.deprecate('Grid', '6.0.0', 'Event argument removed, unused param');
    }
    return this.internalNextPrevCell(!this.rtl);
  }
  /**
   * Select the cell before the currently focused one.
   * @param {Event} [event] [DEPRECATED] unused param
   * @returns {Grid.util.Location} Cell selector
   */
  navigateLeft() {
    var _arguments$2;
    if ((_arguments$2 = arguments[0]) !== null && _arguments$2 !== void 0 && _arguments$2.fromKeyMap) {
      return this.focusCell(this.rtl ? Location.NEXT_CELL : Location.PREV_CELL);
    }
    if (arguments[0]) {
      VersionHelper.deprecate('Grid', '6.0.0', 'Event argument removed, unused param');
    }
    return this.internalNextPrevCell(Boolean(this.rtl));
  }
  //endregion
  //region Row
  /**
   * Selects the next or previous record in relation to the current selection. Scrolls into view if outside.
   * @private
   * @param next Next record (true) or previous (false)
   * @param {Boolean} skipSpecialRows True to not return specialRows like headers
   * @returns {Grid.util.Location|Boolean} Selection context for the focused row (& cell) or false if no selection was made
   */
  internalNextPrevRow(next, skipSpecialRows = true, moveToHeader = true) {
    const me = this,
      cell = me._focusedCell;
    if (!cell) return false;
    const record = me.store[`get${next ? 'Next' : 'Prev'}`](cell.id, false, skipSpecialRows);
    if (record) {
      return me.focusCell({
        id: record.id,
        columnId: cell.columnId,
        scroll: {
          x: false
        }
      });
    } else if (!next && moveToHeader && !cell.isColumnHeader) {
      this.clearFocus();
      return this.getHeaderElement(cell.columnId).focus();
    }
  }
  /**
   * Navigates to the cell below the currently focused cell
   * @param {Event} [event] [DEPRECATED] unused param
   * @returns {Grid.util.Location} Selector for focused row (& cell)
   */
  navigateDown() {
    var _arguments$3;
    if ((_arguments$3 = arguments[0]) !== null && _arguments$3 !== void 0 && _arguments$3.fromKeyMap) {
      return this.focusCell(Location.DOWN);
    }
    if (arguments[0]) {
      VersionHelper.deprecate('Grid', '6.0.0', 'Event argument removed, unused param');
    }
    return this.internalNextPrevRow(true, false);
  }
  /**
   * Navigates to the cell above the currently focused cell
   * @param {Event} [event] [DEPRECATED] unused param
   * @returns {Grid.util.Location} Selector for focused row (& cell)
   */
  navigateUp() {
    var _arguments$4;
    if ((_arguments$4 = arguments[0]) !== null && _arguments$4 !== void 0 && _arguments$4.fromKeyMap) {
      return this.focusCell(Location.UP);
    }
    if (arguments[0]) {
      VersionHelper.deprecate('Grid', '6.0.0', 'Event argument removed, unused param');
    }
    return this.internalNextPrevRow(false, false);
  }
  //endregion
  // This does not need a className on Widgets.
  // Each *Class* which doesn't need 'b-' + constructor.name.toLowerCase() automatically adding
  // to the Widget it's mixed in to should implement thus.
  get widgetClass() {}
});

/**
 * @module Grid/view/mixin/GridResponsive
 */
/**
 * Simplifies making grid responsive. Supply levels as {@link #config-responsiveLevels} config, default levels are:
 * <dl>
 * <dt>small <dd>< 400px,
 * <dt>medium <dd>< 600px
 * <dt>large <dd>> 600px
 * </dl>
 *
 * Columns can define configs per level to be resized etc:
 *
 * ```javascript
 * let grid = new Grid({
 *   responsiveLevels: {
 *     small: 300,
 *     medium: 400,
 *     large: '*' // everything above 400
 *   },
 *
 *   columns: [
 *     {
 *       field: 'name',
 *       text: 'Name',
 *       responsiveLevels: {
 *         small: { hidden: true },
 *         '*': { hidden: false } // all other levels
 *       }
 *     },
 *     { field: 'xx', ... }
 *   ]
 * });
 * ```
 *
 * It is also possible to give a [Grid state](#Grid/view/mixin/GridState) object instead of a level width, but in that
 * case the object must contain a `levelWidth` property:
 *
 * ```javascript
 * let grid = new Grid({
 *   responsiveLevels: {
 *     small: {
 *       // Width is required
 *       levelWidth : 400,
 *       // Other configs are optional, see GridState for available options
 *       rowHeight  : 30
 *     },
 *     medium : {
 *       levelWidth : 600,
 *       rowHeight  : 40
 *     },
 *     large: {
 *       levelWidth : '*', // everything above 300
 *       rowHeight  : 45
 *     }
 *   }
 * });
 * ```
 *
 * @demo Grid/responsive
 * @inlineexample Grid/view/mixin/Responsive.js
 * @mixin
 */
var GridResponsive = (Target => class GridResponsive extends (Target || Base) {
  static get $name() {
    return 'GridResponsive';
  }
  static get defaultConfig() {
    return {
      /**
       * "Break points" for which responsive config to use for columns and css.
       * @config {Object<String,Number|String>}
       * @category Misc
       * @default { small : 400, medium : 600, large : '*' }
       */
      responsiveLevels: Object.freeze({
        small: 400,
        medium: 600,
        large: '*'
      })
    };
  }
  /**
   * Find closes bigger level, aka level we want to use.
   * @private
   * @category Misc
   */
  getClosestBiggerLevel(width) {
    const me = this,
      levels = Object.keys(ObjectHelper.assign({}, me.responsiveLevels));
    let useLevel = null,
      minDelta = 99995,
      biggestLevel = null;
    levels.forEach(level => {
      let levelSize = me.responsiveLevels[level];
      // responsiveLevels can contains config objects, in which case we should use width from it
      if (!['number', 'string'].includes(typeof levelSize)) {
        levelSize = levelSize.levelWidth;
      }
      if (levelSize === '*') {
        biggestLevel = level;
      } else if (width < levelSize) {
        const delta = levelSize - width;
        if (delta < minDelta) {
          minDelta = delta;
          useLevel = level;
        }
      }
    });
    return useLevel || biggestLevel;
  }
  /**
   * Get currently used responsive level (as string)
   * @property {String}
   * @readonly
   * @category Misc
   */
  get responsiveLevel() {
    return this.getClosestBiggerLevel(this.width);
  }
  /**
   * Check if resize lead to a new responsive level and take appropriate actions
   * @private
   * @fires responsive
   * @param width
   * @param oldWidth
   * @category Misc
   */
  updateResponsive(width, oldWidth) {
    const me = this,
      oldLevel = me.getClosestBiggerLevel(oldWidth),
      level = me.getClosestBiggerLevel(width);
    // On first render oldWidth is 0, in such case we need to apply level anyway
    if (oldWidth === 0 || oldLevel !== level) {
      // Level might be a state object
      const levelConfig = me.responsiveLevels[level];
      if (!['number', 'string'].includes(typeof levelConfig)) {
        me.applyState(levelConfig);
      }
      // check columns for responsive config
      me.columns.forEach(column => {
        const levels = column.responsiveLevels;
        if (levels) {
          if (levels[level]) {
            // using state to apply responsive config, since it already does what we want...
            column.applyState(levels[level]);
          } else if (levels['*']) {
            column.applyState(levels['*']);
          }
        }
      });
      me.element.classList.remove('b-responsive-' + oldLevel);
      me.element.classList.add('b-responsive-' + level);
      /**
       * Grid resize lead to a new responsive level being applied
       * @event responsive
       * @param {Grid.view.Grid} grid Grid that was resized
       * @param {String} level New responsive level (small, large, etc)
       * @param {Number} width New width in px
       * @param {String} oldLevel Old responsive level
       * @param {Number} oldWidth Old width in px
       */
      me.trigger('responsive', {
        level,
        width,
        oldLevel,
        oldWidth
      });
    }
  }
  // This does not need a className on Widgets.
  // Each *Class* which doesn't need 'b-' + constructor.name.toLowerCase() automatically adding
  // to the Widget it's mixed in to should implement thus.
  get widgetClass() {}
});

const validIdTypes = {
    string: 1,
    number: 1
  },
  isDataLoadAction = {
    dataset: 1,
    batch: 1
  };
/**
 * @module Grid/view/mixin/GridSelection
 */
/**
 * A mixin for Grid that handles row and cell selection. See {@link #config-selectionMode} for details on how to control
 * what should be selected (rows or cells)
 *
 * @example
 * // select a row
 * grid.selectedRow = 7;
 *
 * // select a cell
 * grid.selectedCell = { id: 5, columnId: 'column1' }
 *
 * // select a record
 * grid.selectedRecord = grid.store.last;
 *
 * // select multiple records by ids
 * grid.selectedRecords = [1, 2, 4, 6]
 *
 * @mixin
 */
var GridSelection = (Target => class GridSelection extends (Target || Base) {
  static get $name() {
    return 'GridSelection';
  }
  static configurable = {
    /**
     * The selection settings, where you can set these boolean flags to control what is selected. Options below:
     * @config {Object} selectionMode
     * @param {Boolean} selectionMode.cell Set to `true` to enable cell selection. This takes precedence over
     * row selection, but rows can still be selected programmatically or with checkbox or RowNumber selection.
     * Required for `column` selection
     * @param {Boolean} selectionMode.multiSelect Allow multiple selection with ctrl and shift+click or with
     * `checkbox` selection. Required for `dragSelect` and `column` selection
     * @param {Boolean|CheckColumnConfig} selectionMode.checkbox Set to `true` to add a checkbox selection column to
     * the grid, or pass a config object for the {@link Grid.column.CheckColumn}
     * @param {Number|String} selectionMode.checkboxIndex Positions the checkbox column at the provided index or to
     * the right of a provided column id. Defaults to 0 or to the right of an included `RowNumberColumn`
     * @param {Boolean} selectionMode.checkboxOnly Select rows only when clicking in the checkbox column. Requires
     * cell selection config to be `false` and checkbox to be set to `true`. This setting was previously named
     * `rowCheckboxSelection`
     * @param {Boolean} selectionMode.showCheckAll Set to `true` to add a checkbox to the selection column header to
     * select/deselect all rows. Requires checkbox to also be set to `true`
     * @param {Boolean} selectionMode.deselectFilteredOutRecords Set to `true` to deselect records when they are
     * filtered out
     * @param {Boolean|String} selectionMode.includeChildren Set to `true` to also select/deselect child nodes
     * when a parent node is selected by toggling the checkbox. Set to `always` to always select/deselect child
     * nodes.
     * @param {Boolean|'all'|'some'} selectionMode.includeParents Set to `all` or `true` to auto select
     * parent if all its children gets selected. If one gets deselected, the parent will also be deselected. Set to
     * 'some' to select parent if one of its children gets selected. The parent will be deselected if all children
     * gets deselected.
     * @param {Boolean} selectionMode.preserveSelectionOnPageChange In `row` selection mode, this flag controls
     * whether the Grid should preserve its selection when loading a new page of a paged data store. Defaults to
     * `false`
     * @param {Boolean} selectionMode.preserveSelectionOnDatasetChange In `row` selection mode, this flag
     * controls whether the Grid should preserve its selection of cells / rows when loading a new dataset
     * (assuming the selected records are included in the newly loaded dataset)
     * @param {Boolean} selectionMode.deselectOnClick Toggles whether the Grid should deselect a selected row or
     * cell when clicking it
     * @param {Boolean} selectionMode.dragSelect Set to `true` to enable multiple selection by dragging.
     * Requires `multiSelect` to also be set to `true`. Also requires the {@link Grid.feature.RowReorder} feature
     * to be set to {@link Grid.feature.RowReorder#config-gripOnly}.
     * @param {Boolean} selectionMode.selectOnKeyboardNavigation Set to `false` to disable auto-selection by keyboard
     * navigation. This will activate the `select` keyboard shortcut.
     * @param {Boolean} selectionMode.column Set to `true` to be able to select whole columns of cells by clicking the header.
     * Requires cell to be set to `true`
     * @param {Boolean|RowNumberColumnConfig} selectionMode.rowNumber Set to `true` or a config object to add a RowNumberColumn
     * which, when clicked, selects the row.
     * @param {Boolean} selectionMode.selectRecordOnCell Set to `false` not to include the record in the
     * `selectedRecords` array when one of the record row's cells is selected.
     * @default
     * @category Selection
     */
    selectionMode: {
      cell: false,
      multiSelect: true,
      checkboxOnly: false,
      checkbox: false,
      checkboxPosition: null,
      showCheckAll: false,
      deselectFilteredOutRecords: false,
      includeChildren: false,
      includeParents: false,
      preserveSelectionOnPageChange: false,
      preserveSelectionOnDatasetChange: true,
      deselectOnClick: false,
      dragSelect: false,
      selectOnKeyboardNavigation: true,
      column: false,
      rowNumber: false,
      selectRecordOnCell: true
    },
    keyMap: {
      'Shift+ArrowUp': 'extendSelectionUp',
      'Shift+ArrowDown': 'extendSelectionDown',
      'Shift+ArrowLeft': 'extendSelectionLeft',
      'Shift+ArrowRight': 'extendSelectionRight',
      ' ': {
        handler: 'toggleSelection',
        weight: 10
      }
    },
    selectedRecordCollection: {}
  };
  construct(config) {
    this._selectedCells = [];
    this._selectedRows = [];
    super.construct(config);
    if (config !== null && config !== void 0 && config.selectedRecords) {
      this.selectedRecords = config.selectedRecords;
    }
  }
  //region Init
  getDefaultGridSelection(clas) {
    if (clas.$name === 'GridSelection') {
      return clas.configurable.selectionMode;
    } else if (clas.superclass) {
      return this.getDefaultGridSelection(clas.superclass);
    }
  }
  changeSelectionMode(mode) {
    const me = this;
    // If changing the selectionMode config object after creation
    if (me.selectionMode) {
      ObjectHelper.assign(me.selectionMode, mode);
      return me.selectionMode;
    }
    me.$defaultGridSelection = me.getDefaultGridSelection(me.constructor);
    // Wraps changeSelectionMode object in a proxy to monitor property changes.
    return new Proxy(mode, {
      set(obj, prop, value) {
        const old = ObjectHelper.assign({}, obj);
        obj[prop] = value;
        // Calls selectionMode's update method on property change
        me.updateSelectionMode(obj, old);
        return true;
      }
    });
  }
  /**
   * The selectionMode configuration has been changed.
   * @event selectionModeChange
   * @param {Object} selectionMode The new {@link #config-selectionMode}
   */
  // Will be called if selectionMode config object changes or if one of its properties changes
  updateSelectionMode(mode, oldMode = this.$defaultGridSelection) {
    const me = this,
      {
        columns,
        checkboxSelectionColumn
      } = me,
      changed = {},
      {
        rowReorder
      } = me.features;
    for (const property in mode) {
      if (mode[property] != oldMode[property]) {
        changed[property] = mode[property];
      }
    }
    // Backwards compatibility. Remove on 7.X?
    if (mode.rowCheckboxSelection && !mode.checkboxOnly) {
      mode.checkboxOnly = true;
      delete mode.rowCheckboxSelection;
    }
    // If column config has been activated, activate cell and multiSelect
    if (changed.column) {
      mode.cell = true;
      mode.multiSelect = true;
    }
    // If cell config has been activated, deactivate checkboxOnly
    if (changed.cell) {
      mode.checkboxOnly = false;
    }
    // If cell config has been deactivated, deactivate column
    if (changed.cell === false) {
      mode.column = false;
    }
    // If checkboxOnly config has been activated, activate checkbox and deactivate cell
    if (changed.checkboxOnly) {
      if (!mode.checkbox) {
        // checkbox can be a CheckboxColumnConfig
        mode.checkbox = true;
      }
      mode.cell = false;
    }
    // If checkbox config has been deactivated, deactivate checkboxOnly and showCheckAll
    if (changed.checkbox === false) {
      changed.checkboxOnly = false;
      changed.showCheckAll = false;
    }
    // If showCheckAll has been activated, activate checkbox and multiselect
    if (changed.showCheckAll) {
      mode.checkbox = mode.checkbox || true;
      mode.multiSelect = true;
    }
    // If includeChildren config has been activated, activate multiselect
    if (changed.includeChildren || changed.includeParents) {
      mode.multiSelect = true;
    }
    // If multiSelect has been deactivated, deactivate column, showCheckAll, dragSelect and includeChildren
    if (changed.multiSelect === false) {
      mode.column = mode.showCheckAll = mode.dragSelect = mode.includeChildren = mode.includeParents = false;
    }
    if (changed.dragSelect) {
      if (rowReorder !== null && rowReorder !== void 0 && rowReorder.enabled && rowReorder.gripOnly !== true) {
        rowReorder.showGrip = rowReorder.gripOnly = true;
      }
      mode.multiSelect = true;
      me._selectionListenersDetachers = {};
    }
    if (changed.dragSelect === false && me._selectionListenersDetachers) {
      var _me$_selectionListene, _me$_selectionListene2;
      (_me$_selectionListene = (_me$_selectionListene2 = me._selectionListenersDetachers).selectiondrag) === null || _me$_selectionListene === void 0 ? void 0 : _me$_selectionListene.call(_me$_selectionListene2);
      delete me._selectionListenersDetachers.selectiondrag;
    }
    // Deselect all when switching between row or cell selection mode
    // Deselect all when switching from multiselect to singleselect
    // Deselect all when changing deselectFilteredOutRecords
    if (oldMode && (changed.cell !== undefined || changed.deselectFilteredOutRecords !== undefined || changed.multiSelect !== undefined)) {
      me.deselectAll();
    }
    // Row number selection
    if (changed.rowNumber) {
      if (!columns.findRecord('type', 'rownumber')) {
        columns.insert(0, {
          ...(typeof mode.rowNumber == 'object' ? mode.rowNumber : {}),
          type: 'rownumber'
        });
        me._selectionAddedRowNumberColumn = true;
      }
    } else if (changed.rowNumber === false && me._selectionAddedRowNumberColumn) {
      columns.remove(columns.findRecord('type', 'rownumber'));
      delete me._selectionAddedRowNumberColumn;
    }
    // Add or remove checkbox column
    if (mode.checkbox !== (oldMode === null || oldMode === void 0 ? void 0 : oldMode.checkbox) || mode.checkbox && mode.showCheckAll !== (oldMode === null || oldMode === void 0 ? void 0 : oldMode.showCheckAll)) {
      if (oldMode) {
        me.deselectAll();
      }
      // See to it that were done configuring when initCheckboxSelection is called.
      if (me.isConfiguring) {
        me.shouldInitCheckboxSelection = true;
      } else {
        me.initCheckboxSelection();
      }
    }
    // If only checkboxIndex has changed
    if (oldMode && mode.checkbox && oldMode.checkbox && mode.checkboxIndex !== oldMode.checkboxIndex && checkboxSelectionColumn) {
      checkboxSelectionColumn.parent.insertChild(checkboxSelectionColumn, columns.getAt(me.checkboxSelectionColumnInsertIndex));
    }
    me.trigger('selectionModeChange', ObjectHelper.clone(mode));
    me.afterSelectionModeChange(mode);
  }
  afterConfigure() {
    // See to it that were done configuring when initCheckboxSelection is called.
    if (this.shouldInitCheckboxSelection) {
      this.shouldInitCheckboxSelection = false;
      this.initCheckboxSelection();
    }
    super.afterConfigure();
  }
  initCheckboxSelection() {
    const me = this,
      {
        selectionMode,
        columns,
        checkboxSelectionColumn
      } = me,
      {
        checkbox
      } = selectionMode;
    // Always remove checkbox column when config changes
    if (checkboxSelectionColumn) {
      // Need to remove this handle because GridBase restores it if it exists.
      me.checkboxSelectionColumn = null;
      columns.remove(checkboxSelectionColumn);
    }
    // Inject our CheckColumn into the ColumnStore
    if (checkbox) {
      var _me$items, _me$items$;
      const checkColumnClass = ColumnStore.getColumnClass('check'),
        config = checkbox === true ? null : checkbox;
      if (!checkColumnClass) {
        throw new Error('CheckColumn must be imported for checkbox selection mode to work');
      }
      const col = me.checkboxSelectionColumn = new checkColumnClass(ObjectHelper.assign({
        id: `${me.id}-selection-column`,
        width: '4em',
        minWidth: '4em',
        // Needed because 4em is below Column's default minWidth
        field: null,
        sortable: false,
        filterable: false,
        hideable: false,
        cellCls: 'b-checkbox-selection',
        // Always put the checkcolumn in the first region
        region: (_me$items = me.items) === null || _me$items === void 0 ? void 0 : (_me$items$ = _me$items[0]) === null || _me$items$ === void 0 ? void 0 : _me$items$.region,
        showCheckAll: selectionMode.showCheckAll,
        draggable: false,
        resizable: false,
        widgets: [{
          type: 'checkbox',
          valueProperty: 'checked',
          ariaLabel: 'L{Checkbox.toggleRowSelect}'
        }]
      }, config), columns, {
        isSelectionColumn: true
      });
      col.meta.depth = 0;
      // This is assigned in Column.js for normal columns
      col._grid = me;
      // Override renderer to inject the rendered record's selected status into the value
      const checkboxRenderer = col.renderer;
      col.renderer = renderData => {
        renderData.value = me.isSelected(renderData.record);
        checkboxRenderer.call(col, renderData);
      };
      col.ion({
        toggle: 'onCheckChange',
        toggleAll: 'onCheckAllChange',
        thisObj: me
      });
      columns.insert(me.checkboxSelectionColumnInsertIndex, col);
    }
  }
  // Used internally to get the index where to insert checkboxselectioncolumn
  // Default : Insert the checkbox after any rownumber column. If not there, -1 means in at 0.
  // If provided, insert at provided index
  get checkboxSelectionColumnInsertIndex() {
    const {
      columns
    } = this;
    let {
      checkboxIndex
    } = this.selectionMode;
    if (!checkboxIndex) {
      checkboxIndex = columns.indexOf(columns.findRecord('type', 'rownumber')) + 1;
    } else if (typeof checkboxIndex === 'string') {
      checkboxIndex = columns.indexOf(columns.getById(checkboxIndex));
    }
    return checkboxIndex;
  }
  //endregion
  // region Events docs & Hooks
  /**
   * The selection has been changed.
   * @event selectionChange
   * @param {'select'|'deselect'} action `'select'`/`'deselect'`
   * @param {'row'|'cell'} mode `'row'`/`'cell'`
   * @param {Grid.view.Grid} source
   * @param {Core.data.Model[]} deselected The records deselected in this operation.
   * @param {Core.data.Model[]} selected The records selected in this operation.
   * @param {Core.data.Model[]} selection The records in the new selection.
   * @param {Grid.util.Location[]} deselectedCells The cells deselected in this operation.
   * @param {Grid.util.Location[]} selectedCells The cells selected in this operation.
   * @param {Grid.util.Location[]} cellSelection The cells in the new selection.
   */
  /**
   * Fires before the selection changes. Returning `false` from a listener prevents the change
   * @event beforeSelectionChange
   * @preventable
   * @param {String} action `'select'`/`'deselect'`
   * @param {'row'|'cell'} mode `'row'`/`'cell'`
   * @param {Grid.view.Grid} source
   * @param {Core.data.Model[]} deselected The records to be deselected in this operation.
   * @param {Core.data.Model[]} selected The records to be selected in this operation.
   * @param {Core.data.Model[]} selection The records in the current selection, before applying `selected` and
   * `deselected`
   * @param {Grid.util.Location[]} deselectedCells The cells to be deselected in this operation.
   * @param {Grid.util.Location[]} selectedCells The cells to be selected in this operation.
   * @param {Grid.util.Location[]} cellSelection  The cells in the current selection, before applying `selectedCells`
   * and `deselectedCells`
   */
  afterSelectionChange() {}
  afterSelectionModeChange() {}
  // endregion
  // region selectedRecordCollection
  changeSelectedRecordCollection(collection) {
    return collection instanceof Collection ? collection : new Collection(collection);
  }
  updateSelectedRecordCollection(collection) {
    collection.ion({
      change: 'onSelectedRecordCollectionChange',
      thisObj: this
    });
  }
  onSelectedRecordCollectionChange({
    added = [],
    removed
  }) {
    if (!this.selectedRecordCollection._fromSelection) {
      // Filter out unselectable rows
      added = added.filter(row => this.isSelectable(row));
      this.performSelection({
        selectedCells: [],
        deselectedCells: [],
        selectedRecords: added,
        deselectedRecords: removed
      });
    }
  }
  changeSelectedRecordCollectionSilent(fn) {
    this.selectedRecordCollection._fromSelection = true;
    const result = fn(this.selectedRecordCollection);
    delete this.selectedRecordCollection._fromSelection;
    return result;
  }
  // endregion
  // region Store
  bindStore(store) {
    var _super$bindStore;
    this.detachListeners('selectionStoreFilter');
    store.ion({
      name: 'selectionStoreFilter',
      filter: 'onStoreFilter',
      thisObj: this
    });
    (_super$bindStore = super.bindStore) === null || _super$bindStore === void 0 ? void 0 : _super$bindStore.call(this, store);
  }
  unbindStore(oldStore) {
    this.detachListeners('selectionStoreFilter');
    super.unbindStore(oldStore);
  }
  onStoreFilter({
    source
  }) {
    const me = this,
      deselect = [];
    // Look for selected records which is not in the store
    for (const selectedRecord of me.selectedRows) {
      if (!source.includes(selectedRecord)) {
        // Should be deselected
        deselect.push(selectedRecord);
      }
    }
    // Deselects
    const selectionChange = me.prepareSelection(me.selectionMode.deselectFilteredOutRecords ? deselect : []);
    // If cell mode, always deselect cells
    if (me.isCellSelectionMode) {
      const {
        deselectedCells
      } = me.prepareSelection(me.getSelectedCellsForRecords(deselect));
      if (deselectedCells !== null && deselectedCells !== void 0 && deselectedCells.length) {
        selectionChange.deselectedCells = (selectionChange.deselectedCells || []).concat(deselectedCells);
      }
    }
    if (selectionChange.deselectedCells.length || selectionChange.deselectedRecords.length) {
      // Trigger deselect event
      me.performSelection(selectionChange, false);
      me.updateCheckboxHeader();
    }
  }
  /**
   * Triggered from Grid view when the id of a record has changed.
   * Update the collection indices.
   * @private
   * @category Selection
   */
  onStoreRecordIdChange({
    record,
    oldValue
  }) {
    var _super$onStoreRecordI;
    // If the next mixin up the inheritance chain has an implementation, call it
    (_super$onStoreRecordI = super.onStoreRecordIdChange) === null || _super$onStoreRecordI === void 0 ? void 0 : _super$onStoreRecordI.call(this, ...arguments);
    const item = this.selectedRecordCollection.get(oldValue);
    // having the record registered by the oldValue means we need to rebuild indices
    if (item === record) {
      this.selectedRecordCollection.rebuildIndices();
    }
  }
  /**
   * Triggered from Grid view when records get removed from the store.
   * Deselects all records which have been removed.
   * @private
   * @category Selection
   */
  onStoreRemove(event) {
    var _super$onStoreRemove;
    // If the next mixin up the inheritance chain has an implementation, call it
    (_super$onStoreRemove = super.onStoreRemove) === null || _super$onStoreRemove === void 0 ? void 0 : _super$onStoreRemove.call(this, event);
    if (!event.isCollapse) {
      const me = this,
        deselectedRecords = event.records.filter(rec => this.isSelected(rec));
      if (deselectedRecords.length) {
        const selectionChange = me.prepareSelection(deselectedRecords);
        // If cell selection mode, also deselect cells for removed records
        // No need to update ui as grid will refresh
        if (me.isCellSelectionMode) {
          const {
            deselectedCells
          } = me.prepareSelection(me.getSelectedCellsForRecords(deselectedRecords));
          if (deselectedCells !== null && deselectedCells !== void 0 && deselectedCells.length) {
            selectionChange.deselectedCells = (selectionChange.deselectedCells || []).concat(deselectedCells);
          }
        }
        me.performSelection(selectionChange);
      }
    }
  }
  /**
   * Triggered from Grid view when the store changes. This might happen
   * if store events are batched and then resumed.
   * Deselects all records which have been removed.
   * @private
   * @category Selection
   */
  onStoreDataChange({
    action,
    source: store
  }) {
    var _super$onStoreDataCha;
    const me = this,
      {
        selectionMode
      } = me;
    let selectionChange;
    // If the next mixin up the inheritance chain has an implementation, call it
    (_super$onStoreDataCha = super.onStoreDataChange) === null || _super$onStoreDataCha === void 0 ? void 0 : _super$onStoreDataCha.call(this, ...arguments);
    if (action === 'pageLoad') {
      // on page load, clear selection if not `preserverSelectionOnPageChange` is true
      if (!selectionMode.preserveSelectionOnPageChange) {
        selectionChange = me.prepareSelection(null, null, true);
      }
      // For paged grid scenario, we need to update the check-all checkbox in the checkbox column header
      // as we move between store pages
      me.updateCheckboxHeader();
    } else if (isDataLoadAction[action]) {
      const deselect = [];
      if (selectionMode.preserveSelectionOnDatasetChange === false) {
        selectionChange = me.prepareSelection(null, null, true);
      } else {
        // Update selected records
        deselect.push(...me.changeSelectedRecordCollectionSilent(c => c.match(store.storage)));
        for (const selectedCell of me._selectedCells) {
          if (!store.getById(selectedCell.id)) {
            deselect.push(selectedCell);
          }
        }
        selectionChange = me.prepareSelection(deselect);
      }
    }
    if (selectionChange && (selectionChange.deselectAll || selectionChange.deselectedCells.length || selectionChange.deselectedRecords.length || selectionChange.selectedCells.length || selectionChange.selectedRecords.length)) {
      me.performSelection(selectionChange, false);
      me.updateCheckboxHeader();
    }
  }
  /**
   * Triggered from Grid view when all records get removed from the store.
   * Deselects all records.
   * @private
   * @category Selection
   */
  onStoreRemoveAll() {
    var _super$onStoreRemoveA;
    // If the next mixin up the inheritance chain has an implementation, call it
    (_super$onStoreRemoveA = super.onStoreRemoveAll) === null || _super$onStoreRemoveA === void 0 ? void 0 : _super$onStoreRemoveA.call(this);
    this.performSelection(this.prepareSelection(null, null, true), false);
  }
  //endregion
  // region Checkbox selection
  onCheckChange({
    checked,
    record
  }) {
    const me = this,
      deselectAll = !me.selectionMode.multiSelect && checked,
      deselect = !deselectAll && !checked ? [record] : null,
      select = checked ? [record] : null;
    // Saves previously non-shift checked checkbox
    if (checked && !GlobalEvents.shiftKeyDown) {
      me._lastSelectionChecked = record;
    }
    // Shift range select
    if (checked && me._lastSelectionChecked && GlobalEvents.shiftKeyDown) {
      me.performSelection(me.internalSelectRange(me._lastSelectionChecked, record, true));
    }
    // Regular selection
    else {
      // Updates UI and triggers events
      me.performSelection(me.prepareSelection(deselect, select, deselectAll, true));
    }
  }
  // Update header checkbox
  updateCheckboxHeader() {
    const {
      selectionMode,
      checkboxSelectionColumn,
      store
    } = this;
    if (selectionMode.checkbox && selectionMode.showCheckAll && checkboxSelectionColumn !== null && checkboxSelectionColumn !== void 0 && checkboxSelectionColumn.headerCheckbox) {
      const allSelected = store.count && !store.some(record => !this.isSelected(record));
      if (checkboxSelectionColumn.headerCheckbox.checked !== allSelected) {
        checkboxSelectionColumn.suspendEvents();
        checkboxSelectionColumn.headerCheckbox.checked = allSelected;
        checkboxSelectionColumn.resumeEvents();
      }
    }
  }
  onCheckAllChange({
    checked
  }) {
    this[checked ? 'selectAll' : 'deselectAll'](this.store.isPaged && this.selectionMode.preserveSelectionOnPageChange);
  }
  //endregion
  // region Selection drag
  // Hook for SalesForce-code to overwrite
  get selectionDragMouseEventListenerElement() {
    return globalThis;
  }
  // Creates new selection range on mouseover. Listener is initiated on mousedown
  onSelectionDrag(event) {
    const me = this,
      {
        _selectionStartCell
      } = me;
    // If we're here but there's no mouse button down for some reason, cancel
    if (!GlobalEvents.isMouseDown()) {
      me.onSelectionEnd();
    }
    // No start cell, ignore
    if (!_selectionStartCell) {
      return;
    }
    const {
        items,
        _lastSelectionDragRegion
      } = me,
      cellData = me.getCellDataFromEvent(event, true),
      region = cellData === null || cellData === void 0 ? void 0 : cellData.column.region,
      cellSelector = (cellData === null || cellData === void 0 ? void 0 : cellData.cellSelector) && me.normalizeCellContext(cellData.cellSelector);
    // If mouse enters new cell
    if (cellSelector && !cellSelector.equals(me._lastSelectionDragCell, true)) {
      if (!me._isSelectionDragging) {
        // When starting selection, start monitoring for near edge scrolling
        me.enableScrollingCloseToEdges(items);
      }
      // If we start a new selection drag on already selected cell, the default (de)selection is delayed until
      // mouseup. If we detect that a drag range is indeed what the user intends, deselect immediately
      if (me._clearSelectionOnSelectionDrag && !_selectionStartCell.equals(cellSelector, true)) {
        me.deselectAll();
        delete me._clearSelectionOnSelectionDrag;
      }
      // A grid with multiple regions need to handle selection and scrolling moving between regions
      if (_lastSelectionDragRegion && region !== _lastSelectionDragRegion) {
        var _me$scrollManager$_ac, _me$scrollManager$_ac2;
        const leavingSubGrid = me.subGrids[_lastSelectionDragRegion],
          enteringSubGrid = me.subGrids[region],
          leavingScrollable = leavingSubGrid.scrollable,
          enteringScrollable = enteringSubGrid.scrollable,
          goingForward = items.indexOf(leavingSubGrid) - items.indexOf(enteringSubGrid) < 0;
        // Immediately scrolls an entering subgrid to either start or end depending on direction
        enteringScrollable.x = goingForward ? 0 : enteringScrollable.maxX;
        // Waiting for grid to scroll to start/end (handled by scrollmanager)
        if (goingForward ? leavingScrollable.x < leavingScrollable.maxX - 1 : leavingScrollable.x > 1) {
          return;
        }
        // Forces the previous subgrid to stop reserving horizontal scroll
        (_me$scrollManager$_ac = me.scrollManager._activeScroll) === null || _me$scrollManager$_ac === void 0 ? void 0 : (_me$scrollManager$_ac2 = _me$scrollManager$_ac.horizontal) === null || _me$scrollManager$_ac2 === void 0 ? void 0 : _me$scrollManager$_ac2.stopScroll();
      }
      me._lastSelectionDragRegion = region;
      me._lastSelectionDragCell = cellSelector;
      me._isSelectionDragging = true;
      const selectionChange = me._lastSelectionDragChange = me.internalSelectRange(_selectionStartCell, cellSelector, me.isRowNumberSelecting(cellSelector) || me.isRowNumberSelecting(_selectionStartCell));
      // As selection at this point is UI only, we don't want to affect already selected records
      selectionChange.deselectedCells = selectionChange.deselectedCells.filter(cell => !me.isCellSelected(cell));
      selectionChange.deselectedRecords = selectionChange.deselectedRecords.filter(record => !me.isSelected(record));
      // selectionChange event fires onSelectionEnd
      me.refreshGridSelectionUI(selectionChange);
      /**
       * Fires while drag selecting. UI will update with current range, but the cells will not be selected until
       * mouse up. This event can be listened for to perform actions while drag selecting.
       * @event dragSelecting
       * @param {Grid.view.Grid} source
       * @param {Core.data.Model[]|Object} selectedCells The cells that is currently being dragged over
       */
      me.trigger('dragSelecting', selectionChange);
    }
  }
  // Tells onSelectionDrag that it's not dragging any longer
  onSelectionEnd() {
    var _me$_selectionListene3, _me$_selectionListene4;
    const me = this,
      lastChange = me._lastSelectionDragChange;
    if (me._isSelectionDragging && !me._selectionStartCell.equals(me._lastSelectionDragCell, true) && lastChange) {
      me.performSelection(lastChange, false);
    }
    me.disableScrollingCloseToEdges(me.items);
    me._isSelectionDragging = false;
    me._lastSelectionDragChange = me._lastSelectionDragCell = me._lastSelectionDragRegion = null;
    // Remove listeners
    (_me$_selectionListene3 = (_me$_selectionListene4 = me._selectionListenersDetachers).selectiondrag) === null || _me$_selectionListene3 === void 0 ? void 0 : _me$_selectionListene3.call(_me$_selectionListene4);
    delete me._selectionListenersDetachers.selectiondrag;
  }
  // endregion
  // region Column selection
  onHandleElementClick(event) {
    const me = this;
    // If rownumber column is clicked, toggle selectAll
    if (me.selectionMode.rowNumber && event.target.closest('.b-rownumber-header')) {
      event.handled = true;
      if (me.store.count && me.store.some(record => !me.isSelected(record))) {
        me.selectAll();
      } else {
        me.deselectAll();
      }
    }
    // In column selection mode, and we clicked a header, the column should be selected
    else if (me.selectionMode.column && event.target.closest('.b-grid-header')) {
      event.handled = true;
      me.selectColumn(event, event.ctrlKey);
    }
    super.onHandleElementClick(event);
  }
  selectColumn(event, addToSelection = false) {
    const me = this,
      {
        store
      } = me,
      {
        columnId
      } = me.getHeaderDataFromEvent(event);
    // internalSelectRange uses this to remember last range, we have no need for that here
    me._shiftSelectRange = null;
    if (!event.shiftKey) {
      me._shiftSelectColumn = columnId;
    }
    const fromColumnId = event.shiftKey && me._shiftSelectColumn || columnId,
      selectionChange = me.internalSelectRange(me.normalizeCellContext({
        id: store.first.id,
        columnId: fromColumnId
      }), me.normalizeCellContext({
        id: store.last.id,
        columnId
      }));
    // If we are selecting a column that is already selected, deselect it
    if (addToSelection && !selectionChange.selectedCells.some(sc => !me.isCellSelected(sc))) {
      selectionChange.deselectedCells = selectionChange.selectedCells;
      selectionChange.selectedCells = [];
    }
    if (!addToSelection) {
      selectionChange.deselectedCells = me._selectedCells;
    }
    me.cleanSelectionChange(selectionChange);
    me.performSelection(selectionChange);
  }
  // endregion
  // region Public row/record selection
  /**
   * Checks whether a row is selected. Will not check if any of a row's cells are selected.
   * @param {LocationConfig|String|Number|Core.data.Model} cellSelectorOrId Cell selector { id: x, column: xx } or row
   * id, or record
   * @returns {Boolean} true if row is selected, otherwise false
   * @category Selection
   */
  isSelected(cellSelectorOrId) {
    var _cellSelectorOrId;
    // Not a selected cell, check recoWds
    if ((_cellSelectorOrId = cellSelectorOrId) !== null && _cellSelectorOrId !== void 0 && _cellSelectorOrId.id) {
      cellSelectorOrId = cellSelectorOrId.id;
    }
    if (validIdTypes[typeof cellSelectorOrId]) {
      return this.selectedRows.some(rec => rec.id === cellSelectorOrId);
    }
    return false;
  }
  /**
   * Checks whether a cell is selected.
   * @param {LocationConfig|Location} cellSelector Cell selector { id: x, column: xx }
   * @param {Boolean} includeRow to also check if row is selected
   * @returns {Boolean} true if cell is selected, otherwise false
   * @category Selection
   */
  isCellSelected(cellSelector, includeRow) {
    cellSelector = this.normalizeCellContext(cellSelector);
    return this.isCellSelectionMode && this._selectedCells.some(cell => cellSelector.equals(cell, true)) || includeRow && this.isSelected(cellSelector);
  }
  /**
   * Checks whether a cell or row can be selected.
   * @param {Core.data.Model|LocationConfig|String|Number} recordCellOrId Record or cell or record id
   * @returns {Boolean} true if cell or row can be selected, otherwise false
   * @category Selection
   */
  isSelectable(recordCellOrId) {
    return this.normalizeCellContext({
      id: recordCellOrId.id || recordCellOrId
    }).isSelectable;
  }
  /**
   * The last selected record. Set to select a row or use Grid#selectRow. Set to null to
   * deselect all
   * @property {Core.data.Model}
   * @category Selection
   */
  get selectedRecord() {
    return this.selectedRecords[this.selectedRecords.length - 1] || null;
  }
  set selectedRecord(record) {
    this.selectRow({
      record
    });
  }
  /**
   * Selected records.
   *
   * If {@link #config-selectionMode deselectFilteredOutRecords} is `false` (default) this will include selected
   * records which has been filtered out.
   *
   * If {@link #config-selectionMode preserveSelectionOnPageChange} is `true` (defaults to `false`) this will include
   * selected records on all pages.
   *
   * If {@link #config-selectionMode selectRecordOnCell} is `true` (default) this will include any record which has at
   * least one cell selected.
   *
   * Can be set as array of ids:
   *
   * ```javascript
   * grid.selectedRecords = [1, 2, 4, 6]
   * ```
   *
   * @property {Core.data.Model[]}
   * @accepts {Core.data.Model[]|Number[]}
   * @category Selection
   */
  get selectedRecords() {
    return this.selectedRecordCollection.values;
  }
  set selectedRecords(selectedRecords) {
    this.selectRows(selectedRecords);
  }
  /**
   * Selected records. Records selected via cell selection is excluded.
   *
   * If {@link #config-selectionMode deselectFilteredOutRecords} is `false` (default) this will include selected
   * records which has been filtered out.
   *
   * If {@link #config-selectionMode preserveSelectionOnPageChange} is `true` (defaults to `false`) this will include
   * selected records on all pages.
   *
   * if {@link #config-selectionMode selectRecordOnCell} is `false` this will return same records as
   * {@link #property-selectedRecords}.
   *
   * Can be set as array of ids:
   *
   * ```javascript
   * grid.selectedRecords = [1, 2, 4, 6]
   * ```
   *
   * @property {Core.data.Model[]}
   * @accepts {Core.data.Model[]|Number[]}
   * @category Selection
   */
  get selectedRows() {
    return [...this._selectedRows];
  }
  set selectedRows(selectedRows) {
    this.selectRows(selectedRows);
  }
  /**
   * Removes and adds records to/from the selection at the same time. Analogous
   * to the `Array` `splice` method.
   *
   * Note that if items that are specified for removal are also in the `toAdd` array,
   * then those items are *not* removed then appended. They remain in the same position
   * relative to all remaining items.
   *
   * @param {Number} index Index at which to remove a block of items. Only valid if the
   * second, `toRemove` argument is a number.
   * @param {Object[]|Number} toRemove Either the number of items to remove starting
   * at the passed `index`, or an array of items to remove (If an array is passed, the `index` is ignored).
   * @param  {Object[]|Object} toAdd An item, or an array of items to add.
   * @category Selection
   */
  spliceSelectedRecords(index, toRemove, toAdd) {
    const me = this;
    if (typeof toRemove == 'number') {
      const select = [...me.selectedRecords];
      select.splice(index, toRemove, ...ArrayHelper.asArray(toAdd));
      me.performSelection(me.prepareSelection(null, select, true, true));
    } else {
      // Just add and remove
      me.performSelection(me.prepareSelection(toRemove, toAdd, false, true));
    }
  }
  /**
   * Select one row
   * @param {Object|Core.data.Model|String|Number} options A record or id to select or a config object describing the
   * selection
   * @param {Core.data.Model|String|Number} options.record Record or record id, specifying null will deselect all
   * @param {Grid.column.Column} [options.column] The column to scroll into view if `scrollIntoView` is not specified as
   * `false`. Defaults to the grid's first column.
   * @param {Boolean} [options.scrollIntoView] Specify `false` to prevent row from being scrolled into view
   * @param {Boolean} [options.addToSelection] Specify `true` to add to selection, defaults to `false` which replaces
   * @fires selectionChange
   * @category Selection
   */
  selectRow(options) {
    // Make sure we have an object
    if (typeof options === 'number' || options.isModel || !('record' in options)) {
      options = {
        records: [options]
      };
    }
    // scrollIntoView is default here
    ObjectHelper.assignIf(options, {
      scrollIntoView: true
    });
    this.selectRows(options);
  }
  /**
   * Select one or more rows
   * @param {Object|Core.data.Model[]|String[]|Number[]} options An array of records or ids for a record or a
   * config object describing the selection
   * @param {Core.data.Model[]|String[]|Number[]} records An array of records or ids for a record
   * @param {Grid.column.Column} options.column The column to scroll into view if `scrollIntoView` is not specified as
   * `false`. Defaults to the grid's first column.
   * @param {Boolean} [options.scrollIntoView] Specify `false` to prevent row from being scrolled into view
   * @param {Boolean} [options.addToSelection] Specify `true` to add to selection, defaults to `false` which replaces
   * @category Selection
   */
  selectRows(options) {
    // Got a single or an array of records/ids, convert it to an object
    if (!options || Array.isArray(options) || options.isModel || typeof options === 'number' || !('records' in options) && !('record' in options)) {
      options = {
        records: ArrayHelper.asArray(options) || []
      };
    }
    const me = this,
      {
        store
      } = me,
      toSelect = [],
      {
        records = options.record ? [options.record] : [],
        // Got a record instead of records
        column = me.columns.visibleColumns[0],
        // Default
        scrollIntoView,
        addToSelection = arguments[1] // Backwards compatibility
      } = options;
    for (let record of records) {
      record = store.getById(record);
      if (record) {
        toSelect.push(record);
      }
    }
    if (!addToSelection) {
      me._shiftSelectRange = null;
    }
    me.performSelection(me.prepareSelection(null, toSelect, !addToSelection, true));
    if (toSelect.length && scrollIntoView) {
      me.scrollRowIntoView(toSelect[0].id, {
        column
      });
    }
  }
  /**
   * This selects all rows. If store is filtered, this will merge the selection of all visible rows with any selection
   * made prior to filtering.
   * @privateparam {Boolean} [silent] Pass `true` not to fire any event upon selection change
   * @category Selection
   */
  selectAll(silent = false) {
    const {
        store
      } = this,
      records = (store.isGrouped ? store.allRecords : store.records).filter(r => !r.isSpecialRow);
    // If store is grouped, store.records excludes collapsed records and allRecords excludes filtered out records
    // Else, store records holds what we're after
    this.performSelection(this.prepareSelection(null, records, false, true), true, silent);
  }
  /**
   * Deselects all selected rows and cells. If store is filtered, this will unselect all visible rows only. Any
   * selections made prior to filtering remains.
   * @param {Boolean} [removeCurrentRecordsOnly] Pass `false` to clear all selected records, and `true` to only
   * clear selected records in the current set of records
   * @param {Boolean} [silent] Pass `true` not to fire any event upon selection change
   * @category Selection
   */
  deselectAll(removeCurrentRecordsOnly = false, silent = false) {
    const {
        store
      } = this,
      records = removeCurrentRecordsOnly ? (store.isGrouped ? store.allRecords : store.records).filter(r => !r.isSpecialRow) : null;
    this.performSelection(this.prepareSelection(records, null, !removeCurrentRecordsOnly), true, silent);
  }
  /**
   * Deselect one row
   * @param {Core.data.Model|String|Number} recordOrId Record or an id for a record
   * @category Selection
   */
  deselectRow(record) {
    this.deselectRows(record);
  }
  /**
   * Deselect one or more rows
   * @param {Core.data.Model|String|Number|Core.data.Model[]|String[]|Number[]} recordOrIds An array of records or ids
   * for a record
   * @category Selection
   */
  deselectRows(recordsOrIds) {
    // Ignore any non-existing row records passed
    const {
        store
      } = this,
      records = ArrayHelper.asArray(recordsOrIds).map(recordOrId => store.getById(recordOrId)).filter(rec => rec);
    this.performSelection(this.prepareSelection(records));
  }
  /**
   * Selects rows corresponding to a range of records (from fromId to toId)
   * @param {String|Number} fromId
   * @param {String|Number} toId
   * @category Selection
   */
  selectRange(fromId, toId, addToSelection = false) {
    const me = this,
      {
        store
      } = me,
      selection = me.internalSelectRange(store.getById(fromId), store.getById(toId), true);
    me._shiftSelectRange = null; // For below function to not replace last range with new one
    me.performSelection(selection);
  }
  // endregion
  // region Public cell selection
  /**
   * In cell selection mode, this will get the cell selector for the (last) selected cell. Set to an available cell
   * selector to select only that cell. Or use {@link #function-selectCell()} instead.
   * @property {Grid.util.Location}
   * @category Selection
   */
  get selectedCell() {
    return this._selectedCells[this._selectedCells.length - 1];
  }
  set selectedCell(cellSelector) {
    this.selectCells([cellSelector]);
  }
  /**
   * In cell selection mode, this will get the cell selectors for all selected cells. Set to an array of available
   * cell selectors. Or use {@link #function-selectCells()} instead.
   * @property {Grid.util.Location[]}
   * @category Selection
   */
  get selectedCells() {
    return [...this._selectedCells];
  }
  set selectedCells(cellSelectors) {
    this.selectCells(cellSelectors);
  }
  /**
   * CSS selector for the currently selected cell. Format is "[data-index=index] [data-column-id=column]".
   * @type {String}
   * @category Selection
   * @readonly
   */
  get selectedCellCSSSelector() {
    const cell = this.selectedCell,
      row = cell && this.getRowById(cell.id);
    if (!cell || !row) return '';
    return `[data-index=${row.dataIndex}] [data-column-id=${cell.columnId}]`;
  }
  /**
   * If in cell selection mode, this selects one cell. If not, this selects the cell's record.
   * @param {LocationConfig|Object} options A cell selector ({ id: rowId, columnId: 'columnId' }) or a config object
   * @param {LocationConfig} options.cell  A cell selector ({ id: rowId, columnId: 'columnId' })
   * @param {Boolean} [options.scrollIntoView] Specify `false` to prevent row from being scrolled into view
   * @param {Boolean} [options.addToSelection] Specify `true` to add to selection, defaults to `false` which replaces
   * @param {Boolean} [options.silent] Specify `true` to not trigger any events when selecting the cell
   * @returns {Grid.util.Location} Cell selector
   * @fires selectionChange
   * @category Selection
   */
  selectCell(options) {
    var _this$selectCells;
    // Got a cell selector as first argument
    if ('id' in options) {
      options = {
        cell: options
      };
      // Arguments backward's compability
      options = Object.assign({
        scrollIntoView: arguments[1],
        addToSelection: arguments[2],
        silent: arguments[3]
      }, options);
    }
    return (_this$selectCells = this.selectCells(options)) === null || _this$selectCells === void 0 ? void 0 : _this$selectCells[0];
  }
  /**
   * If in cell selection mode, this selects a number of cells. If not, this selects corresponding records.
   * @param {Object|LocationConfig[]} options An array of cell selectors ({ id: rowId, columnId: 'columnId' }) or a config
   * object
   * @param {LocationConfig[]} options.cells An array of cell selectors { id: rowId, columnId: 'columnId' }
   * @param {Boolean} [options.scrollIntoView] Specify `false` to prevent row from being scrolled into view
   * @param {Boolean} [options.addToSelection] Specify `true` to add to selection, defaults to `false` which replaces
   * @param {Boolean} [options.silent] Specify `true` to not trigger any events when selecting the cell
   * @returns {Grid.util.Location[]} Cell selectors
   * @returns {Grid.util.Location[]} Cell selectors
   * @fires selectionChange
   * @category Selection
   */
  selectCells(options) {
    // Got a cell selector array as first argument
    if (Array.isArray(options)) {
      options = {
        cells: options
      };
    }
    const me = this,
      {
        cells = options.cell ? [options.cell] : [],
        // Got a cell instead of cells
        scrollIntoView = true,
        addToSelection = false,
        silent = false
      } = options,
      selectionChange = me.prepareSelection(null, cells, !addToSelection);
    if (!addToSelection) {
      me._shiftSelectRange = null;
    }
    me.performSelection(selectionChange, true, silent);
    if (scrollIntoView) {
      me.scrollRowIntoView(cells[0].id, {
        column: cells[0].columnId
      });
    }
    return me.isCellSelectionMode ? selectionChange.selectedCells : selectionChange.selectedRecords;
  }
  /**
   * If in cell selection mode, this deselects one cell. If not, this deselects the cell's record.
   * @param {LocationConfig} cellSelector
   * @returns {Grid.util.Location} Normalized cell selector
   * @category Selection
   */
  deselectCell(cellSelector) {
    var _this$deselectCells;
    return (_this$deselectCells = this.deselectCells([cellSelector])) === null || _this$deselectCells === void 0 ? void 0 : _this$deselectCells[0];
  }
  /**
   * If in cell selection mode, this deselects a number of cells. If not, this deselects corresponding records.
   * @param {LocationConfig[]} cellSelectors
   * @returns {Grid.util.Location[]} Normalized cell selectors
   * @category Selection
   */
  deselectCells(cellSelectors) {
    const selectionChange = this.prepareSelection(cellSelectors);
    this.performSelection(selectionChange);
    return this.isCellSelectionMode ? selectionChange.deselectedCells : selectionChange.deselectedRecords;
  }
  // Used by keymap to toggle selection of currently focused cell.
  toggleSelection(keyEvent) {
    const me = this,
      {
        _focusedCell,
        selectionMode
      } = me,
      isRowNumber = me.isRowNumberSelecting(_focusedCell),
      isSelected = me.isCellSelected(_focusedCell, true);
    // Only if keyboardNavigation selection is deactivated and were not focusing an actionable cell
    if (selectionMode.selectOnKeyboardNavigation === true || _focusedCell.isActionable) {
      // Return false to ley keyMap know we didn't handle this event
      return false;
    }
    me.performSelection(me.prepareSelection(isSelected ? _focusedCell : null, isSelected ? null : _focusedCell, !selectionMode.multiSelect, isRowNumber));
    // Space key has preventDefault = false somewhere
    keyEvent.preventDefault();
  }
  /**
   * Selects a range of cells, from a cell selector (Location) to another
   * @param {Grid.util.Location|LocationConfig} from
   * @param {Grid.util.Location|LocationConfig} to
   * @category Selection
   */
  selectCellRange(from, to) {
    this.performSelection(this.internalSelectRange(from, to));
  }
  // endregion
  // region Private convenience functions & properties
  getSelection() {
    if (this.isRowSelectionMode) {
      return this.selectedRecords;
    } else {
      return this.selectedCells;
    }
  }
  // Makes sure the same record or cell isn't deselected and selected at the same time. Selection will take precedence
  cleanSelectionChange(selectionChange) {
    const {
      deselectedRecords,
      selectedRecords,
      deselectedCells,
      selectedCells,
      deselectedCellRecords,
      selectedCellRecords
    } = selectionChange;
    // Filter out records which is both selected and deselected
    if (deselectedRecords !== null && deselectedRecords !== void 0 && deselectedRecords.length && selectedRecords !== null && selectedRecords !== void 0 && selectedRecords.length) {
      selectionChange.deselectedRecords = deselectedRecords.filter(dr => !selectedRecords.some(sr => dr === sr));
    }
    // Filter out cells which is both selected and deselected
    if (deselectedCells !== null && deselectedCells !== void 0 && deselectedCells.length && selectedCells !== null && selectedCells !== void 0 && selectedCells.length) {
      selectionChange.deselectedCells = deselectedCells.filter(dc => !selectedCells.some(sc => dc.equals(sc, true)));
    }
    // Filter out cell-selected records that is being selected
    if (deselectedCellRecords.length && (selectedCellRecords !== null && selectedCellRecords !== void 0 && selectedCellRecords.length || selectedRecords !== null && selectedRecords !== void 0 && selectedRecords.length)) {
      selectionChange.deselectedCellRecords = deselectedCellRecords.filter(dcr => {
        return !selectedCellRecords.some(scr => dcr.id === scr.id) && !selectedRecords.some(sr => dcr.id === sr.id);
      });
    }
    return selectionChange;
  }
  getSelectedCellsForRecords(records) {
    return this._selectedCells.filter(cell => cell.id && records.some(record => record.id === cell.id));
  }
  delayUntilMouseUp(fn) {
    const detacher = EventHelper.on({
      element: globalThis,
      blur: ev => fn(ev, detacher),
      mouseup: ev => fn(ev, detacher),
      thisObj: this,
      once: true
    });
  }
  get isRowSelectionMode() {
    return !this.isCellSelectionMode;
  }
  get isCellSelectionMode() {
    return this.selectionMode.cell === true;
  }
  // Checks if rowNumber is activated and that all arguments (cellselectors) is of type rownumber
  isRowNumberSelecting(...selectors) {
    return this.selectionMode.rowNumber && !selectors.some(cs => cs.column.type !== 'rownumber');
  }
  // endregion
  //region Navigation
  // Used by keyMap to extend selection range
  extendSelectionLeft() {
    this.extendSelection('Left');
  }
  // Used by keyMap to extend selection range
  extendSelectionRight() {
    this.extendSelection('Right');
  }
  // Used by keyMap to extend selection range
  extendSelectionUp() {
    this.extendSelection('Up');
  }
  // Used by keyMap to extend selection range
  extendSelectionDown() {
    this.extendSelection('Down');
  }
  // Used by keyMap to extend selection range
  extendSelection(dir) {
    this._isKeyboardRangeSelecting = true;
    this['navigate' + dir]();
    this._isKeyboardRangeSelecting = false;
  }
  // Called from GridNavigation on mouse or keyboard events
  // Single entry point for all default user selection actions
  onCellNavigate(me, fromCellSelector, toCellSelector, doSelect) {
    var _toCellSelector$recor;
    const {
        selectionMode,
        _selectionListenersDetachers
      } = me,
      {
        multiSelect,
        deselectOnClick,
        dragSelect
      } = selectionMode,
      {
        ctrlKeyDown,
        shiftKeyDown
      } = GlobalEvents,
      isMouseLeft = GlobalEvents.isMouseDown(),
      isMouseRight = GlobalEvents.isMouseDown(2),
      currentEvent = GlobalEvents.currentMouseDown || GlobalEvents.currentKeyDown;
    // To be sure we got Locations
    toCellSelector = me.normalizeCellContext(toCellSelector);
    if (!doSelect ||
    // Do not affect selection if navigating into header row.
    toCellSelector.rowIndex === -1 || (_toCellSelector$recor = toCellSelector.record) !== null && _toCellSelector$recor !== void 0 && _toCellSelector$recor.isGroupHeader ||
    // Don't allow keyboard selection if keyboardNavigation is deactivated
    currentEvent !== null && currentEvent !== void 0 && currentEvent.fromKeyMap && !selectionMode.selectOnKeyboardNavigation ||
    // CheckColumn events are handled by the CheckColumn itself.
    me.columns.getById(toCellSelector.columnId) === me.checkboxSelectionColumn || selectionMode.checkboxOnly ||
    // Don't select if event was handled elsewhere
    (currentEvent === null || currentEvent === void 0 ? void 0 : currentEvent.handled) === true) {
      return;
    }
    // Save adding state unless shift key
    if (!shiftKeyDown) {
      me._isAddingToSelection = ctrlKeyDown && multiSelect;
      me._selectionStartCell = toCellSelector; // To be able to begin a new range
    }
    // Flags that it's possible for onSelectDrag to apply its logic if the right conditions are met
    // (preventDragSelect is set by RowReorder if mousedown on the grip with gripOnly configured. also set in
    // DragCreateBase and EventDragSelect)
    if (multiSelect && dragSelect && isMouseLeft && !_selectionListenersDetachers.selectiondrag && !me.preventDragSelect) {
      _selectionListenersDetachers.selectiondrag = EventHelper.on({
        name: 'selectiondrag',
        element: me.selectionDragMouseEventListenerElement,
        blur: 'onSelectionEnd',
        mouseup: {
          handler: 'onSelectionEnd',
          element: globalThis
        },
        mousemove: 'onSelectionDrag',
        thisObj: me
      });
    }
    me.preventDragSelect = false;
    const startCell = me._selectionStartCell,
      adding = me._isAddingToSelection;
    // Select range on shiftKey
    if ((shiftKeyDown && isMouseLeft || me._isKeyboardRangeSelecting) && startCell && multiSelect) {
      me.performSelection(me.internalSelectRange(startCell, toCellSelector, me.isRowNumberSelecting(startCell, toCellSelector)));
    } else {
      let delay = false,
        continueSelecting = true,
        deselect;
      // If current is already selected
      if (me.isCellSelected(toCellSelector, true)) {
        // Do nothing if we right-clicked already selected row/cell
        if (isMouseRight) {
          return;
        }
        // Deselect current if selected and multiselecting or deselect all if deselectOnClick is true
        if (adding || deselectOnClick) {
          deselect = deselectOnClick ? null : [toCellSelector];
          continueSelecting = false; // Only deselect at this code path
        }
        // If this is only row or cell that's selected
        else if (me.selectedRecords.length + (me.isCellSelectionMode ? me._selectedCells.length : 0) <= 1) {
          // Should stay selected, do no more
          return;
        }
        // Delay if click a selected cell which will be deselected (for dragging)
        delay = deselectOnClick || multiSelect;
      }
      // deselect all if not multiselecting
      if (!deselect && !adding) {
        deselect = null;
        // Set flag so that dragselection functionality know to clear selection if needed
        if (dragSelect && delay && _selectionListenersDetachers.selectiondrag) {
          me._clearSelectionOnSelectionDrag = true;
        }
      }
      // Wrapping selection in a function to be called either directly or on mouse up
      const finishSelection = (mouseUpEvent, detacher) => {
        var _mouseUpEvent$target;
        detacher === null || detacher === void 0 ? void 0 : detacher();
        if ((mouseUpEvent === null || mouseUpEvent === void 0 ? void 0 : (_mouseUpEvent$target = mouseUpEvent.target) === null || _mouseUpEvent$target === void 0 ? void 0 : _mouseUpEvent$target.nodeType) === Node.ELEMENT_NODE) {
          // If we are waiting for mouseUp and have moved to a different cell, abort selection change
          const mouseUpSelector = new Location(mouseUpEvent.target);
          if (mouseUpSelector !== null && mouseUpSelector !== void 0 && mouseUpSelector.grid && !mouseUpSelector.equals(toCellSelector, true)) {
            return;
          }
        }
        if (!shiftKeyDown) {
          me._shiftSelectRange = null; // Clear any previous range selected
        }

        me.performSelection(me.prepareSelection(deselect, continueSelecting && [toCellSelector], deselect === null, continueSelecting && me.isRowNumberSelecting(toCellSelector)));
      };
      // Delay doing the selection until mouse up for allowing drag of row in certain cases
      if (delay) {
        me.delayUntilMouseUp(finishSelection);
      } else {
        finishSelection();
      }
    }
  }
  // endregion
  // region Internal selection & deselection functions
  /**
   * Used internally to prepare a number of cells or records for selection/deselection depending on if cell
   * selectionMode is activated. This function will not select/deselect anything by itself
   * (that's done in performSelection).
   * @param {LocationConfig[]|Core.data.Model[]} cellSelectorsToDeselect Array of cell selectors or records.
   * @param {LocationConfig[]|Core.data.Model[]} cellSelectorsToSelect Array of cell selectors or records.
   * @param {Boolean} deselectAll Set to `true` to clear all selected records and cells.
   * @param {Boolean} forceRecordSelection Set to `true` to force record selection even if cell selection is active.
   * @returns {Object} selectionChange object to use for UI update
   * @private
   * @category Selection
   */
  prepareSelection(cellSelectorsToDeselect, cellSelectorsToSelect, deselectAll = false, forceRecordSelection = false) {
    const me = this,
      isDragging = me._isSelectionDragging,
      {
        includeParents,
        selectRecordOnCell
      } = me.selectionMode,
      selectedRecords = [],
      selectedCells = [];
    let deselectedCells = [],
      deselectedRecords = [],
      selectedCellRecords = [],
      deselectedCellRecords = [];
    if (deselectAll) {
      deselectedCells = me._selectedCells;
      deselectedRecords = me._selectedRows;
      deselectedCellRecords = me.selectedRecords.filter(r => !deselectedRecords.some(dr => dr.id === r.id));
    } else if (cellSelectorsToDeselect) {
      for (const selector of ArrayHelper.asArray(cellSelectorsToDeselect)) {
        const cellSelector = me.normalizeCellContext(selector),
          record = (cellSelector === null || cellSelector === void 0 ? void 0 : cellSelector.record) || (selector.isModel ? selector : me.store.getById(cellSelector.id));
        if (cellSelector.isSpecialRow) {
          continue;
        }
        deselectedCells.push(cellSelector);
        if (record && !deselectedRecords.some(r => r.id === record.id)) {
          var _record$allChildren;
          // When dragging, this path is taken but nothing is actually selected until mouseup
          // So should check if selected for dragselection (until mouseup)
          if (isDragging || me.isSelected(record)) {
            deselectedRecords.push(record);
          }
          // If not directly selected, but selected by cell, deselect by cell
          else if (selectRecordOnCell && me.selectedRecordCollection.get(record.id) && !deselectedCellRecords.some(dr => dr.id === record.id)) {
            deselectedCellRecords.push(record);
          }
          // If configured, also deselect children
          if (me.selectionMode.includeChildren && me.selectionMode.multiSelect && !record.isLeaf && (_record$allChildren = record.allChildren) !== null && _record$allChildren !== void 0 && _record$allChildren.length) {
            for (const child of record.allChildren) {
              if (!deselectedRecords.some(r => r.id === child.id) && (isDragging || me.isSelected(child))) {
                deselectedRecords.push(child);
              }
            }
          }
        }
      }
    }
    if (cellSelectorsToSelect) {
      for (const selector of ArrayHelper.asArray(cellSelectorsToSelect)) {
        const cellSelector = me.normalizeCellContext(selector),
          record = (cellSelector === null || cellSelector === void 0 ? void 0 : cellSelector.record) || (selector.isModel ? selector : me.store.getById(cellSelector.id));
        if (!record || cellSelector.isSpecialRow) {
          continue;
        }
        // Only select cells if in cell selection mode and not forcing record selection
        if (me.isCellSelectionMode && !forceRecordSelection) {
          selectedCells.push(cellSelector);
        }
        if ((me.isRowSelectionMode || forceRecordSelection) && !selectedRecords.some(r => r.id === record.id)) {
          var _record$allChildren2;
          selectedRecords.push(record);
          // If configured, also select children
          if (me.selectionMode.includeChildren && me.selectionMode.multiSelect && !record.isLeaf && (_record$allChildren2 = record.allChildren) !== null && _record$allChildren2 !== void 0 && _record$allChildren2.length) {
            for (const child of record.allChildren) {
              if (!selectedRecords.some(r => r.id === child.id)) {
                selectedRecords.push(child);
              }
            }
          }
        }
      }
      if (selectRecordOnCell && selectedCells.length) {
        selectedCellRecords = ArrayHelper.unique(selectedCells.map(c => c.record)).filter(r => !selectedRecords.some(sr => sr.id === r.id));
      }
    }
    // This setting could be either off, or true/'all' or 'some'
    if (includeParents && (deselectedRecords.length || selectedRecords.length)) {
      const allChanges = [...deselectedRecords, ...selectedRecords],
        lowestLevelParents = ArrayHelper.unique(allChanges.filter(rec => rec.parent && !rec.allChildren.some(child => allChanges.includes(child))).map(rec => rec.parent));
      lowestLevelParents.forEach(parent => me.toggleParentSelection(parent, selectedRecords, deselectedRecords));
    }
    return me.cleanSelectionChange({
      selectedCells,
      selectedRecords,
      deselectedCells,
      deselectedRecords,
      deselectAll,
      action: selectedRecords !== null && selectedRecords !== void 0 && selectedRecords.length || selectedCells !== null && selectedCells !== void 0 && selectedCells.length ? 'select' : 'deselect',
      selectedCellRecords,
      deselectedCellRecords
    });
  }
  toggleParentSelection(parent, toSelect, toDeselect) {
    if (!parent || parent.isRoot) {
      return;
    }
    const isSelected = this.isSelected(parent),
      inToSelect = toSelect.includes(parent),
      inToDeselect = toDeselect.includes(parent),
      childIsSelected = child => this.isSelected(child) && !toDeselect.includes(child) || toSelect.includes(child);
    if (this.selectionMode.includeParents === 'some') {
      // If any children are selected
      if (parent.allChildren.some(childIsSelected)) {
        // And parent is not being deselected => select
        if ((!isSelected || inToDeselect) && !inToSelect) {
          toSelect.push(parent);
        }
      }
      // No children are selected and parent is selected => deselect
      else if (isSelected && !inToDeselect) {
        toDeselect.push(parent);
      }
    } else {
      // includeParents = true/'all'
      if (isSelected) {
        // If previously selected, and some child is to be deselected => deselect
        if (!inToDeselect && !inToSelect && parent.allChildren.some(child => toDeselect.includes(child))) {
          toDeselect.push(parent);
        }
      } else if (!inToSelect) {
        // If not previously selected, select if all children are selected
        if (parent.allChildren.every(childIsSelected)) {
          toSelect.push(parent);
        }
      }
    }
    // Go up one level if it exists
    if (parent.parent) {
      this.toggleParentSelection(parent.parent, toSelect, toDeselect);
    }
  }
  /**
   * Used internally to select a range of cells or records depending on selectionMode. Used in both shift-selection
   * and for drag selection. Will remember current selection range and replace it with new one when it changes. But a
   * range which is completed (drag select mouse up or a new shift range starting point has been set) will remain.
   * This function will not update UI (that's done in refreshGridSelectionUI).
   * @param {LocationConfig} fromSelector
   * @param {LocationConfig} toSelector
   * @returns {Object} selectionChange object to use for UI update
   * @private
   * @category Selection
   */
  internalSelectRange(fromSelector, toSelector, forceRecordSelection = false) {
    const me = this,
      selectRecords = me.isRowSelectionMode || forceRecordSelection,
      selectionChange = me.prepareSelection(me._shiftSelectRange, me.getRange(fromSelector, toSelector, selectRecords), false, forceRecordSelection);
    me._shiftSelectRange = selectionChange[`selected${selectRecords ? 'Records' : 'Cells'}`];
    return selectionChange;
  }
  /**
   * Used internally to get a range of cell selectors from a start selector to an end selector.
   * @private
   */
  getRange(fromSelector, toSelector, selectRecords = false) {
    const me = this,
      {
        store
      } = me,
      fromCell = me.normalizeCellContext(fromSelector),
      toCell = me.normalizeCellContext(toSelector),
      startRowIndex = Math.min(fromCell.rowIndex, toCell.rowIndex),
      endRowIndex = Math.max(fromCell.rowIndex, toCell.rowIndex),
      toSelect = [],
      startColIndex = Math.min(fromCell.columnIndex, toCell.columnIndex),
      endColIndex = Math.max(fromCell.columnIndex, toCell.columnIndex);
    if (startRowIndex === -1 || endRowIndex === -1) {
      throw new Error('Record not found in selectRange');
    }
    // Row selection
    if (selectRecords) {
      const range = store.getRange(startRowIndex, endRowIndex + 1, false);
      // To make selectedRecords in correct order when range selecting upwards
      if (toCell.rowIndex < fromCell.rowIndex) {
        range.reverse();
      }
      toSelect.push(...range);
    }
    // Cell selection
    else {
      // Loops from start cell to end cell and creates selectors for all containing cells
      for (let rIx = startRowIndex; rIx <= endRowIndex; rIx++) {
        for (let cIx = startColIndex; cIx <= endColIndex; cIx++) {
          toSelect.push({
            rowIndex: rIx,
            columnIndex: cIx
          });
        }
      }
    }
    return toSelect.map(s => me.normalizeCellContext(s));
  }
  // endregion
  // region Update UI & trigger events
  performSelection(selectionChange, updateUI = true, silent = false) {
    const me = this,
      {
        selectedRecords = [],
        selectedCells = [],
        selectedCellRecords = [],
        deselectedRecords = [],
        deselectedCells = [],
        deselectedCellRecords = [],
        action
      } = selectionChange,
      allSelectedRecords = [...selectedRecords, ...selectedCellRecords],
      allDeselectedRecords = [...deselectedRecords, ...deselectedCellRecords],
      rowMode = me.isRowSelectionMode;
    // Fire event to be able to prevent selection
    if (me.trigger('beforeSelectionChange', {
      mode: rowMode ? 'row' : 'cell',
      action,
      selected: allSelectedRecords,
      deselected: allDeselectedRecords,
      selection: me.selectedRecords,
      selectedCells,
      deselectedCells,
      cellSelection: me.selectedCells
    }) === false) {
      return;
    }
    // If deselecting all cells
    if (me._selectedCells === deselectedCells) {
      me._selectedCells = [];
    }
    // Not deselecting all cells
    else {
      const keepCells = [];
      for (const selectedCell of me._selectedCells) {
        if (!deselectedCells.some(cellSelector => selectedCell.equals(cellSelector, true))) {
          keepCells.push(selectedCell);
        }
      }
      me._selectedCells = keepCells;
    }
    selectionChange.deselectedRecords = [...selectionChange.deselectedRecords];
    // If deselecting all rows
    if (deselectedRecords === me._selectedRows) {
      me.changeSelectedRecordCollectionSilent(c => c.clear());
      me._selectedRows.length = 0;
    }
    // Not deselecting all rows
    else {
      const keepRecords = [],
        keepInCollection = [];
      for (const selectedRecord of me.selectedRecords) {
        if (!allDeselectedRecords.some(record => selectedRecord.id === record.id)) {
          if (me.isSelected(selectedRecord)) {
            keepRecords.push(selectedRecord);
          } else {
            keepInCollection.push(selectedRecord);
          }
        }
      }
      me.changeSelectedRecordCollectionSilent(c => c.values = [...keepRecords, ...keepInCollection]);
      me._selectedRows = keepRecords;
    }
    // New selection
    if (selectedCells.length) {
      for (const selectedCell of selectedCells) {
        if (!me._selectedCells.some(cellSelector => cellSelector.equals(selectedCell, true))) {
          me._selectedCells.push(selectedCell);
        }
      }
    }
    if (selectedRecords.length) {
      me.changeSelectedRecordCollectionSilent(c => c.add(...selectedRecords));
      me._selectedRows.push(...selectedRecords.filter(r => !me._selectedRows.some(sr => sr.id === r.id)));
    }
    if (selectedCellRecords.length) {
      me.changeSelectedRecordCollectionSilent(c => c.add(...selectedCellRecords));
    }
    if (updateUI) {
      me.refreshGridSelectionUI(selectionChange);
    }
    me.afterSelectionChange(selectionChange);
    if (!silent) {
      me.triggerSelectionChangeEvent(selectionChange);
    }
  }
  // Makes sure the DOM is up-to-date with current selection.
  refreshGridSelectionUI({
    selectedRecords,
    selectedCells,
    deselectedRecords,
    deselectedCells
  }) {
    const me = this,
      {
        checkboxSelectionColumn
      } = me;
    // Row selection
    checkboxSelectionColumn === null || checkboxSelectionColumn === void 0 ? void 0 : checkboxSelectionColumn.suspendEvents();
    me.updateGridSelectionRecords(selectedRecords, true);
    me.updateGridSelectionRecords(deselectedRecords, false);
    me.updateCheckboxHeader();
    checkboxSelectionColumn === null || checkboxSelectionColumn === void 0 ? void 0 : checkboxSelectionColumn.resumeEvents();
    // Cell selection
    if (me.isCellSelectionMode) {
      me.updateGridSelectionCells(selectedCells, true);
      if (me.selectionMode.column) {
        me.updateGridSelectionColumns(selectedCells);
      }
    }
    me.updateGridSelectionCells(deselectedCells, false);
  }
  // Loops through records and updates Grid rows
  updateGridSelectionRecords(records, selected) {
    const {
      checkboxSelectionColumn
    } = this;
    if (records !== null && records !== void 0 && records.length) {
      for (let i = 0; i < records.length; i++) {
        const row = this.getRowFor(records[i]);
        if (row) {
          row.toggleCls('b-selected', selected);
          row.setAttribute('aria-selected', selected);
          if (checkboxSelectionColumn && !checkboxSelectionColumn.hidden && !records[i].isSpecialRow) {
            row.getCell(checkboxSelectionColumn.id).widget.checked = selected;
          }
        }
      }
    }
  }
  // Loops through cell selectors and updates Grid cell's
  updateGridSelectionCells(cells, selected) {
    if (cells !== null && cells !== void 0 && cells.length) {
      for (let i = 0; i < cells.length; i++) {
        const cell = this.getCell(cells[i]);
        if (cell) {
          cell.setAttribute('aria-selected', selected);
          cell.classList.toggle('b-selected', selected);
        }
      }
    }
  }
  // Loops through columns to toggle their selected state
  updateGridSelectionColumns(selectedCells) {
    const {
      count
    } = this.store;
    for (const column of this.columns.visibleColumns) {
      var _column$element;
      (_column$element = column.element) === null || _column$element === void 0 ? void 0 : _column$element.classList.toggle('b-selected', (selectedCells === null || selectedCells === void 0 ? void 0 : selectedCells.filter(s => s.columnId === column.id).length) === count);
    }
  }
  triggerSelectionChangeEvent(selectionChange) {
    const {
        selectedRecords = [],
        selectedCells = [],
        selectedCellRecords = [],
        deselectedRecords = [],
        deselectedCells = [],
        deselectedCellRecords = []
      } = selectionChange,
      allSelectedRecords = [...selectedRecords, ...selectedCellRecords],
      allDeselectedRecords = [...deselectedRecords, ...deselectedCellRecords],
      rowMode = this.isRowSelectionMode;
    this.trigger('selectionChange', {
      mode: rowMode ? 'row' : 'cell',
      action: selectionChange.action,
      selected: allSelectedRecords,
      deselected: allDeselectedRecords,
      selection: this.selectedRecords,
      selectedCells,
      deselectedCells,
      cellSelection: this.selectedCells
    });
  }
  //endregion
  doDestroy() {
    var _this$selectedRecordC;
    (_this$selectedRecordC = this.selectedRecordCollection) === null || _this$selectedRecordC === void 0 ? void 0 : _this$selectedRecordC.destroy();
    this._selectedCells.length = 0;
    this._selectedRows.length = 0;
    for (const detacher in this._selectionListenersDetachers) {
      this._selectionListenersDetachers[detacher]();
    }
    super.doDestroy();
  }
  // This does not need a className on Widgets.
  // Each *Class* which doesn't need 'b-' + constructor.name.toLowerCase() automatically adding
  // to the Widget it's mixed in to should implement thus.
  get widgetClass() {}
});

/**
 * @module Grid/view/mixin/GridState
 */
const suspendStoreEvents = subGrid => subGrid.columns.suspendEvents(),
  resumeStoreEvents = subGrid => subGrid.columns.resumeEvents(),
  fillSubGridColumns = subGrid => {
    subGrid.columns.clearCaches();
    subGrid.columns.fillFromMaster();
  },
  compareStateSortIndex = (a, b) => a.stateSortIndex - b.stateSortIndex;
/**
 * Mixin for Grid that handles state. It serializes the following grid properties:
 *
 * * rowHeight
 * * selectedCell
 * * selectedRecords
 * * columns (order, widths, visibility)
 * * store (sorters, groupers, filters)
 * * scroll position
 *
 * See {@link Core.mixin.State} for more information on state.
 *
 * @demo Grid/state
 * @inlineexample Grid/view/mixin/GridState.js
 * @mixin
 */
var GridState = (Target => class GridState extends (Target || Base) {
  static get $name() {
    return 'GridState';
  }
  static get configurable() {
    return {
      statefulEvents: ['subGridCollapse', 'subGridExpand', 'horizontalScroll', 'stateChange']
    };
  }
  /**
   * Gets or sets grid's state. Check out {@link Grid.view.mixin.GridState} mixin for details.
   * @member {Object} state
   * @property {Object[]} state.columns
   * @property {Number} state.rowHeight
   * @property {Object} state.scroll
   * @property {Number} state.scroll.scrollLeft
   * @property {Number} state.scroll.scrollTop
   * @property {Array} state.selectedRecords
   * @property {String} state.style
   * @property {String} state.selectedCell
   * @property {Object} state.store
   * @property {Object} state.store.sorters
   * @property {Object} state.store.groupers
   * @property {Object} state.store.filters
   * @property {Object} state.subGrids
   * @category State
   */
  updateStore(store, was) {
    var _super$updateStore;
    (_super$updateStore = super.updateStore) === null || _super$updateStore === void 0 ? void 0 : _super$updateStore.call(this, store, was);
    this.detachListeners('stateStoreListeners');
    store === null || store === void 0 ? void 0 : store.ion({
      name: 'stateStoreListeners',
      filter: 'triggerUpdate',
      group: 'triggerUpdate',
      sort: 'triggerUpdate',
      thisObj: this
    });
  }
  updateColumns(columns, was) {
    var _super$updateColumns;
    (_super$updateColumns = super.updateColumns) === null || _super$updateColumns === void 0 ? void 0 : _super$updateColumns.call(this, columns, was);
    this.detachListeners('stateColumnListeners');
    columns.ion({
      name: 'stateColumnListeners',
      change: 'triggerUpdate',
      thisObj: this
    });
  }
  updateRowManager(manager, was) {
    var _super$updateRowManag;
    (_super$updateRowManag = super.updateRowManager) === null || _super$updateRowManag === void 0 ? void 0 : _super$updateRowManag.call(this, manager, was);
    this.detachListeners('stateRowManagerListeners');
    manager.ion({
      name: 'stateRowManagerListeners',
      rowHeight: 'triggerUpdate',
      thisObj: this
    });
  }
  triggerUpdate() {
    this.trigger('stateChange');
  }
  finalizeInit() {
    super.finalizeInit();
    this.ion({
      selectionChange: 'triggerUpdate',
      thisObj: this
    });
  }
  /**
   * Get grid's current state for serialization. State includes rowHeight, headerHeight, selectedCell,
   * selectedRecordId, column states and store state etc.
   * @returns {Object} State object to be serialized
   * @private
   */
  getState() {
    const me = this,
      style = me.element.style.cssText,
      state = {
        rowHeight: me.rowHeight
      };
    if (style) {
      state.style = style;
    }
    if (me.selectedCell) {
      const {
        id,
        columnId
      } = me.selectedCell;
      state.selectedCell = {
        id,
        columnId
      };
    }
    state.selectedRecords = me.selectedRecords.map(entry => entry.id);
    state.columns = me.columns.allRecords.map(column => column.getState());
    state.store = me.store.state;
    state.scroll = me.storeScroll();
    state.subGrids = {};
    me.eachSubGrid(subGrid => {
      const config = state.subGrids[subGrid.region] = state.subGrids[subGrid.region] || {};
      if (subGrid.isPainted) {
        if (subGrid.flex == null) {
          config.width = subGrid.width;
        }
      } else {
        if (subGrid.config.width != null) {
          config.width = subGrid.config.width;
        } else {
          config.flex = subGrid.config.flex;
        }
      }
      config.collapsed = subGrid.collapsed ?? false;
      // Part of a collapsed SubGrid's state is the state to restore to when expanding again.
      if (config.collapsed) {
        config._beforeCollapseState = subGrid._beforeCollapseState;
      }
    });
    return state;
  }
  /**
   * Apply previously stored state.
   * @param {Object} state
   * @private
   */
  applyState(state) {
    const me = this;
    // Applying state will call row renderer at least 7 times. Suspending refresh helps to save some time.
    // Roughly on default testing grid apply state takes 26ms without suspend and 16ms with it.
    me.suspendRefresh();
    // Do this first since it might perform full rendering of contents, recreating filterbar header fields
    if ('columns' in state) {
      let columnsChanged = false,
        needSort = false;
      // We're going to renderContents anyway, so stop the ColumnStores from updating the UI
      me.columns.suspendEvents();
      me.eachSubGrid(suspendStoreEvents);
      // each column triggers rerender at least once...
      state.columns.forEach((columnState, index) => {
        const column = me.columns.getById(columnState.id);
        if (column) {
          const columnGeneration = column.generation;
          // If column region is missing in the current config, clear it from the column state and
          // stick to the default configuration
          if ('region' in columnState && !(columnState.region in me.subGrids)) {
            delete columnState.region;
            delete columnState.locked;
          }
          column.applyState(columnState);
          columnsChanged = columnsChanged || column.generation !== columnGeneration;
          // In case a sort is needed, stamp in the ordinal position.
          column.stateSortIndex = index;
          // If we find one out of order, only then do we need to sort
          if (column.allIndex !== index) {
            needSort = columnsChanged = true;
          }
        }
      });
      if (columnsChanged) {
        me.eachSubGrid(fillSubGridColumns);
      }
      if (needSort) {
        me.eachSubGrid(subGrid => {
          subGrid.columns.records.sort(compareStateSortIndex);
          subGrid.columns.allRecords.sort(compareStateSortIndex);
        });
        me.columns.sort({
          fn: compareStateSortIndex,
          // always sort ascending
          ascending: true
        });
      }
      // If we have been painted, and column restoration changed the column layout, refresh contents
      if (me.isPainted && columnsChanged) {
        me.renderContents();
      }
      // Allow ColumnStores to update the UI again
      me.columns.resumeEvents();
      me.eachSubGrid(resumeStoreEvents);
    }
    if ('subGrids' in state) {
      me.eachSubGrid(subGrid => {
        if (subGrid.region in state.subGrids) {
          const subGridState = state.subGrids[subGrid.region];
          if ('width' in subGridState) {
            subGrid.width = subGridState.width;
          } else if ('flex' in subGridState) {
            subGrid.flex = subGridState.flex;
          }
          if ('collapsed' in subGridState) {
            subGrid.collapsed = subGridState.collapsed;
            subGrid._beforeCollapseState = subGridState._beforeCollapseState;
          }
        }
        subGrid.clearWidthCache();
      });
    }
    if ('rowHeight' in state) {
      me.rowHeight = state.rowHeight;
    }
    if ('style' in state) {
      me.style = state.style;
    }
    if ('selectedCell' in state) {
      me.selectedCell = state.selectedCell;
    }
    if ('store' in state) {
      me.store.state = state.store;
    }
    if ('selectedRecords' in state) {
      me.selectedRecords = state.selectedRecords;
    }
    me.resumeRefresh(true);
    me.whenVisible(() => me.applyScrollState(state));
  }
  applyScrollState(state) {
    const me = this;
    // Update scroll state
    me.eachSubGrid(s => s.refreshFakeScroll());
    if ('scroll' in state) {
      me.restoreScroll(state.scroll);
      // We need to force resize handler on all observable elements, because vertical scroll triggered by the
      // previous method will suspend the listener. So by the time ResizeObserver triggers mutation handler
      // listener won't actually update widget size.
      // Handler works here because we haven't _yet_ suspended it, it will happen one animation frame after
      // scroll event is triggered
      if (state.scroll.scrollTop) {
        me.element.querySelectorAll('.b-resize-monitored').forEach(element => {
          const widget = WidgetHelper.fromElement(element);
          if (widget) {
            widget.onElementResize(element);
          }
        });
      }
    }
  }
  // This does not need a className on Widgets.
  // Each *Class* which doesn't need 'b-' + constructor.name.toLowerCase() automatically adding
  // to the Widget it's mixed in to should implement thus.
  get widgetClass() {}
});

/**
 * @module Grid/util/SubGridScroller
 */
const immediatePromise = Promise.resolve(),
  defaultScrollOptions$1 = {
    block: 'nearest'
  };
/**
 * A Scroller subclass which handles scrolling in a SubGrid. Needs special treatment since the SubGrid itself only
 * allows horizontal scrolling, while the vertical scrolling is done by an outer element containing all subgrids.
 *
 * @internal
 */
class SubGridScroller extends Scroller {
  // SubGrids do not drive the scrollWidth of their partners (Header and Footer)
  // SubGrids scrollWidth is propagated from the Header by SubGrid.refreshFakeScroll.
  static configurable = {
    propagate: false,
    overflowX: 'hidden-scroll',
    yScroller: null
  };
  // The Grid's main Y scroller keeps a list of the X scrollers
  updateYScroller(yScroller) {
    yScroller === null || yScroller === void 0 ? void 0 : yScroller.addScroller(this);
  }
  scrollIntoView(element, options = defaultScrollOptions$1) {
    const me = this,
      {
        xDelta,
        yDelta
      } = me.getDeltaTo(element, options),
      result = xDelta || yDelta ? me.scrollBy(xDelta, yDelta, options) : immediatePromise;
    if (options.highlight || options.focus) {
      result.then(() => {
        if (options.highlight) {
          if (element instanceof Rectangle) {
            element.translate(-xDelta, -yDelta).highlight();
          } else {
            DomHelper.highlight(element);
          }
        }
        options.focus && element.focus && element.focus();
      });
    }
    return result;
  }
  scrollBy(xDelta, yDelta, options) {
    const yPromise = yDelta && this.yScroller.scrollBy(0, yDelta, options),
      xPromise = xDelta && super.scrollBy(xDelta, 0, options);
    if (xPromise && xPromise.cancel && yPromise && yPromise.cancel) {
      const cancelX = xPromise.cancel,
        cancelY = yPromise.cancel;
      // Set up cross canceling
      xPromise.cancel = yPromise.cancel = () => {
        cancelX();
        cancelY();
      };
      return Promise.all([xPromise, yPromise]);
    }
    return xPromise || yPromise || immediatePromise;
  }
  scrollTo(toX, toY, options) {
    const yPromise = toY != null && this.yScroller.scrollTo(null, toY, options),
      xPromise = toX != null && super.scrollTo(toX, null, options);
    // Keep partners in sync immediately unless we are going to animate our position.
    // There are potentially three: The header, the footer and the docked fake horizontal scroller.
    // It will be more efficient and maintain correct state doing it now.
    if (!(options && options.animate)) {
      this.syncPartners();
    }
    if (xPromise && xPromise.cancel && yPromise && yPromise.cancel) {
      const cancelX = xPromise.cancel,
        cancelY = yPromise.cancel;
      // Set up cross canceling
      xPromise.cancel = yPromise.cancel = () => {
        cancelX();
        cancelY();
      };
      return Promise.all([xPromise, yPromise]);
    }
    return xPromise || yPromise || immediatePromise;
  }
  get viewport() {
    const elementBounds = Rectangle.from(this.element),
      viewport = elementBounds.intersect(Rectangle.from(this.yScroller.element));
    // For 0 height subgrids, viewport will be `false` but we still expect a Rectangle to be returned
    return viewport || new Rectangle(elementBounds.x, elementBounds.y, elementBounds.width, 0);
  }
  set y(y) {
    if (this.yScroller) {
      this.yScroller.y = y;
    }
  }
  get y() {
    return this.yScroller ? this.yScroller.y : 0;
  }
  get maxY() {
    return this.yScroller ? this.yScroller.maxY : 0;
  }
  get scrollHeight() {
    return this.yScroller ? this.yScroller.scrollHeight : 0;
  }
  get clientHeight() {
    return this.yScroller ? this.yScroller.clientHeight : 0;
  }
}
SubGridScroller._$name = 'SubGridScroller';

/**
 * @module Grid/view/SubGrid
 */
const sumWidths = (t, e) => t + e.getBoundingClientRect().width;
/**
 * A SubGrid is a part of the grid (it has at least one and normally no more than two, called locked and normal). It
 * has its own header, which holds the columns to display rows for in the SubGrid. SubGrids are created by Grid, you
 * should not need to create instances directly.
 *
 * If not configured with a width or flex, the SubGrid will be sized to fit its columns. In this case, if all columns
 * have a fixed width (not using flex) then toggling columns will also affect the width of the SubGrid.
 *
 * @extends Core/widget/Widget
 */
class SubGrid extends Widget {
  //region Config
  static get $name() {
    return 'SubGrid';
  }
  // Factoryable type name
  static get type() {
    return 'subgrid';
  }
  /**
   * Region (name) for this SubGrid
   * @config {String} region
   */
  /**
   * Column store, a store containing the columns for this SubGrid
   * @config {Grid.data.ColumnStore} columns
   */
  static get defaultConfig() {
    return {
      insertRowsBefore: null,
      appendTo: null,
      monitorResize: true,
      headerClass: null,
      footerClass: null,
      /**
       * The subgrid "weight" determines its position among its SubGrid siblings.
       * Higher weights go further right.
       * @config {Number}
       * @category Layout
       */
      weight: null,
      /**
       * Set `true` to start subgrid collapsed. To operate collapsed state on subgrid use
       * {@link #function-collapse}/{@link #function-expand} methods.
       * @config {Boolean}
       * @default false
       */
      collapsed: null,
      scrollable: {
        // Each SubGrid only handles scrolling in the X axis.
        // The owning Grid handles the Y axis.
        overflowX: 'hidden-scroll'
      },
      scrollerClass: SubGridScroller,
      // Will be set to true by GridSubGrids if it calculates the subgrids width based on its columns.
      // Used to determine if hiding a column should affect subgrids width
      hasCalculatedWidth: null,
      /**
       * Set `true` to disable moving columns into or out of this SubGrid.
       * @config {Boolean}
       * @default false
       * @private
       */
      sealedColumns: null
    };
  }
  static get configurable() {
    return {
      element: true,
      header: {},
      footer: {},
      virtualScrollerElement: true,
      splitterElement: true,
      headerSplitter: true,
      scrollerSplitter: true,
      footerSplitter: true,
      /**
       * Set to `false` to prevent this subgrid being resized with the {@link Grid.feature.RegionResize} feature
       * @config {Boolean}
       * @default true
       */
      resizable: null,
      role: 'presentation'
    };
  }
  static delayable = {
    // This uses a shorter delay for tests, see construct()
    hideOverlayScroller: 1000
  };
  //endregion
  //region Init
  /**
   * SubGrid constructor
   * @param config
   * @private
   */
  construct(config) {
    const me = this;
    super.construct(config);
    this.rowManager.ion({
      addRows: 'onAddRow',
      thisObj: this
    });
    if (BrowserHelper.isFirefox) {
      const {
          element
        } = me,
        verticalScroller = me.grid.scrollable;
      // Firefox cannot scroll vertically smoothly when using touch pad. Even a microscopic horizontal touch will
      // abort the vertical scrolling. To counter this we ignore pointer events on the subgrid element temporarily
      // until scroll stops. No test coverage.
      // https://github.com/bryntum/support/issues/3000
      let lastScrollTop = 0;
      element.addEventListener('wheel', ({
        ctrlKey,
        deltaY,
        deltaX
      }) => {
        const isVerticalScroll = Math.abs(deltaY) > Math.abs(deltaX);
        // Ignore wheel event with Control key pressed - it doesn't scroll, it either zooms scheduler or zooms
        // the page.
        if (!ctrlKey && isVerticalScroll && !me.scrollEndDetacher && verticalScroller.y !== lastScrollTop) {
          element.style.pointerEvents = 'none';
          lastScrollTop = verticalScroller.y;
          me.scrollEndDetacher = verticalScroller.ion({
            scrollEnd: async () => {
              lastScrollTop = verticalScroller.y;
              element.style.pointerEvents = '';
              me.scrollEndDetacher = null;
            },
            once: true
          });
        }
      });
    }
    if (VersionHelper.isTestEnv) {
      me.hideOverlayScroller.delay = 50;
    }
  }
  doDestroy() {
    var _me$fakeScroller;
    const me = this;
    me.header.destroy();
    me.footer.destroy();
    (_me$fakeScroller = me.fakeScroller) === null || _me$fakeScroller === void 0 ? void 0 : _me$fakeScroller.destroy();
    me.virtualScrollerElement.remove();
    me.splitterElements.forEach(element => element.remove());
    super.doDestroy();
  }
  get barConfig() {
    const me = this,
      {
        width,
        flex
      } = me.element.style,
      config = {
        subGrid: me,
        parent: me,
        // Contained widgets need to know their parents
        maxWidth: me.maxWidth || undefined,
        minWidth: me.minWidth || undefined
      };
    // If we have been configured with sizing, construct the Bar in sync.
    if (flex) {
      config.flex = flex;
    } else if (width) {
      config.width = width;
    }
    return config;
  }
  changeHeader(header) {
    return new this.headerClass(ObjectHelper.assign({
      id: this.id + '-header'
    }, this.barConfig, header));
  }
  changeFooter(footer) {
    return new this.footerClass(ObjectHelper.assign({
      id: this.id + '-footer'
    }, this.barConfig, footer));
  }
  //endregion
  //region Splitters
  get splitterElements() {
    return [this.splitterElement, this.headerSplitter, this.scrollerSplitter, this.footerSplitter];
  }
  /**
   * Toggle (add/remove) class for splitters
   * @param {String} cls class name
   * @param {Boolean} [add] actions. Set to `true` to add class, `false` to remove
   * @private
   */
  toggleSplitterCls(cls, add = true) {
    this.splitterElements.forEach(el => el === null || el === void 0 ? void 0 : el.classList[add ? 'add' : 'remove'](cls));
  }
  hideSplitter() {
    this.splitterElements.forEach(el => el.classList.add('b-hide-display'));
    this.$showingSplitter = false;
  }
  showSplitter() {
    this.splitterElements.forEach(el => el.classList.remove('b-hide-display'));
    this.$showingSplitter = true;
  }
  //endregion
  //region Template
  changeElement(element, was) {
    const {
      region
    } = this;
    return super.changeElement({
      'aria-label': region,
      className: {
        'b-grid-subgrid': 1,
        [`b-grid-subgrid-${region}`]: region,
        'b-grid-subgrid-collapsed': this.collapsed
      },
      dataset: {
        region
      }
    }, was);
  }
  get rowElementConfig() {
    const {
      grid
    } = this;
    return {
      role: 'row',
      className: grid.rowCls,
      children: this.columns.visibleColumns.map((column, columnIndex) => ({
        role: 'gridcell',
        'aria-colindex': columnIndex + 1,
        tabIndex: grid.cellTabIndex,
        className: 'b-grid-cell',
        dataset: {
          column: column.field || '',
          columnId: column.id
        }
      }))
    };
  }
  // Added to DOM in Grid `get bodyConfig`
  changeVirtualScrollerElement() {
    const references = DomHelper.createElement({
      role: 'presentation',
      reference: 'virtualScrollerElement',
      className: 'b-virtual-scroller',
      tabIndex: -1,
      dataset: {
        region: this.region
      },
      children: [{
        reference: 'virtualScrollerWidth',
        className: 'b-virtual-width'
      }]
    });
    this.virtualScrollerWidth = references.virtualScrollerWidth;
    return references.virtualScrollerElement;
  }
  changeSplitterElement() {
    const references = DomHelper.createElement({
      reference: 'splitterElement',
      className: {
        'b-grid-splitter': 1,
        'b-grid-splitter-collapsed': this.collapsed,
        'b-hide-display': 1 // GridSubGrids determines visibility
      },

      dataset: {
        region: this.region
      },
      children: [BrowserHelper.isTouchDevice ? {
        className: 'b-splitter-touch-area'
      } : null, {
        className: 'b-grid-splitter-inner b-grid-splitter-main',
        children: [{
          className: 'b-grid-splitter-buttons',
          reference: 'splitterButtons',
          children: [{
            className: 'b-grid-splitter-button-collapse',
            children: [BrowserHelper.isTouchDevice ? {
              className: 'b-splitter-button-touch-area'
            } : null, {
              tag: 'svg',
              ns: 'http://www.w3.org/2000/svg',
              version: '1.1',
              className: 'b-grid-splitter-button-icon b-gridregion-collapse-arrow',
              viewBox: '0 0 256 512',
              children: [{
                tag: 'path',
                d: 'M192 448c-8.188 0-16.38-3.125-22.62-9.375l-160-160c-12.5-1' + '2.5-12.5-32.75 0-45.25l160-160c12.5-12.5 32.75-12.5 45.25 0s' + '12.5 32.75 0 45.25L77.25 256l137.4 137.4c12.5 12.5 12.5 32.7' + '5 0 45.25C208.4 444.9 200.2 448 192 448z'
              }]
            }]
          }, {
            className: 'b-grid-splitter-button-expand',
            children: [BrowserHelper.isTouchDevice ? {
              className: 'b-splitter-button-touch-area'
            } : null, {
              tag: 'svg',
              ns: 'http://www.w3.org/2000/svg',
              version: '1.1',
              className: 'b-grid-splitter-button-icon b-gridregion-expand-arrow',
              viewBox: '0 0 256 512',
              children: [{
                tag: 'path',
                d: 'M64 448c-8.188 0-16.38-3.125-22.62-9.375c-12.5-12.5-12.5-3' + '2.75 0-45.25L178.8 256L41.38 118.6c-12.5-12.5-12.5-32.75 0-4' + '5.25s32.75-12.5 45.25 0l160 160c12.5 12.5 12.5 32.75 0 45.25' + 'l-160 160C80.38 444.9 72.19 448 64 448z'
              }]
            }]
          }]
        }]
      }]
    });
    this.splitterButtons = references.splitterButtons;
    return references.splitterElement;
  }
  get splitterConfig() {
    return {
      className: this.splitterElement.className.trim(),
      children: [BrowserHelper.isTouchDevice ? {
        className: 'b-splitter-touch-area'
      } : null, {
        className: 'b-grid-splitter-inner'
      }],
      dataset: {
        region: this.region
      }
    };
  }
  changeHeaderSplitter() {
    return DomHelper.createElement(this.splitterConfig);
  }
  changeScrollerSplitter() {
    return DomHelper.createElement(this.splitterConfig);
  }
  changeFooterSplitter() {
    return DomHelper.createElement(this.splitterConfig);
  }
  //endregion
  //region Render
  render(...args) {
    const me = this;
    super.render(...args);
    // Unit tests create naked SubGrids so we have to do this.
    if (me.grid) {
      me.updateHasFlex();
      me.element.parentNode.insertBefore(me.splitterElement, me.element.nextElementSibling);
      // Cant use "global" listener with delegate for mouseenter, since mouseenter only fires on target
      me.splitterElements.forEach(element => EventHelper.on({
        element,
        mouseenter: 'onSplitterMouseEnter',
        mouseleave: 'onSplitterMouseLeave',
        thisObj: me
      }));
      me._collapsed && me.collapse();
    }
  }
  refreshHeader() {
    this.header.refreshContent();
  }
  refreshFooter() {
    var _this$footer;
    (_this$footer = this.footer) === null || _this$footer === void 0 ? void 0 : _this$footer.refreshContent();
  }
  // Override to iterate header and footer.
  eachWidget(fn, deep = true) {
    const me = this,
      widgets = [me.header, me.footer];
    for (let i = 0; i < widgets.length; i++) {
      const widget = widgets[i];
      if (fn(widget) === false) {
        return;
      }
      if (deep && widget.eachWidget) {
        widget.eachWidget(fn, deep);
      }
    }
  }
  //endregion
  //region Size & resize
  /**
   * Sets cell widths. Cannot be done in template because of CSP
   * @private
   */
  fixCellWidths(rowElement) {
    const {
      visibleColumns
    } = this.columns;
    // fix cell widths, no longer allowed in template because of CSP
    let cell = rowElement.firstElementChild,
      i = 0;
    while (cell) {
      const column = visibleColumns[i],
        {
          element
        } = column;
      if (column.minWidth) {
        cell.style.minWidth = DomHelper.setLength(column.minWidth);
      }
      if (column.maxWidth) {
        cell.style.maxWidth = DomHelper.setLength(column.maxWidth);
      }
      // either flex or width, flex has precedence
      if (column.flex) {
        // Nested flex - we have to match the column's header width because it's flexing
        // a different available space - the space in its owning column header.
        if (column.childLevel && element) {
          cell.style.flex = `0 0 ${element.getBoundingClientRect().width}px`;
          cell.style.width = '';
        } else {
          cell.style.flex = column.flex;
          cell.style.width = '';
        }
      } else if (column.width) {
        // https://app.assembla.com/spaces/bryntum/tickets/8041
        // Although header and footer elements must be sized using flex-basis to avoid the busting out problem,
        // grid cells MUST be sized using width since rows are absolutely positioned and will not cause the
        // busting out problem, and rows will not stretch to shrinkwrap the cells unless they are widthed with
        // width.
        cell.style.width = DomHelper.setLength(column.width);
      } else {
        cell.style.flex = cell.style.width = cell.style.minWidth = '';
      }
      cell = cell.nextElementSibling;
      i++;
    }
  }
  get totalFixedWidth() {
    return this.columns.totalFixedWidth;
  }
  /**
   * Sets header width and scroller width (if needed, depending on if using flex). Might also change the subgrids
   * width, if it uses a width calculated from its columns.
   * @private
   */
  fixWidths() {
    const me = this,
      {
        element,
        header,
        footer
      } = me;
    if (!me.collapsed) {
      if (me.flex) {
        header.flex = me.flex;
        if (footer) {
          footer.flex = me.flex;
        }
        element.style.flex = me.flex;
      } else {
        // If width is calculated and no column is using flex, check if total width is less than width. If so,
        // recalculate width and bail out of further processing (since setting width will trigger again)
        if (me.hasCalculatedWidth && !me.columns.some(col => !col.hidden && col.flex) && me.totalFixedWidth !== me.width) {
          me.width = me.totalFixedWidth;
          // Setting width above clears the hasCalculatedWidth flag, but we want to keep it set to react
          // correctly next time
          me.hasCalculatedWidth = true;
          return;
        }
        let totalWidth = me.width;
        // Calculate width from our total column width if we are supposed to have a calculated width
        if (!totalWidth && me.hasCalculatedWidth) {
          totalWidth = 0;
          // summarize column widths, needed as container width when not using flex widths.
          for (const col of me.columns) {
            if (!col.flex && !col.hidden) totalWidth += col.width;
          }
        }
        // rows are absolutely positioned, meaning that their width won't affect container width
        // hence we must set it, if not using flex
        element.style.width = `${totalWidth}px`;
        header.width = totalWidth;
        if (footer) {
          footer.width = totalWidth;
        }
      }
      me.syncScrollingPartners(false);
    }
  }
  // Safari does not shrink cells the same way as chrome & ff does without having a width set on the row
  fixRowWidthsInSafariEdge() {
    if (BrowserHelper.isSafari) {
      const me = this,
        {
          region,
          header
        } = me,
        minWidth = header.calculateMinWidthForSafari();
      // fix row widths for safari, it does not size flex cells correctly at small widths otherwise.
      // there should be a css solution, but I have failed to find it
      me.rowManager.forEach(row => {
        // This function runs on resize and rendering a SubGrid triggers a resize. When adding a new SubGrid
        // on the fly elements won't exists for it yet, so ignore...
        const element = row.getElement(region);
        // it is worth noting that setting a width does not prevent the row from growing beyond that with
        // when making view wider, it is used in flex calculation more like a min-width
        if (element) {
          element.style.width = `${minWidth}px`;
        }
      });
      header.headersElement.style.width = `${minWidth}px`;
    }
  }
  /**
   * Get/set SubGrid width, which also sets header and footer width (if available).
   * @property {Number}
   */
  set width(width) {
    const me = this;
    // Width explicitly set, remember that
    me.hasCalculatedWidth = false;
    super.width = width;
    me.header.width = width;
    me.footer.width = width;
    // When we're live, we can't wait until the  throttled resize occurs - it looks bad.
    if (me.isPainted) {
      me.onElementResize();
    }
  }
  get width() {
    return super.width;
  }
  /**
   * Get/set SubGrid flex, which also sets header and footer flex (if available).
   * @property {Number|String}
   */
  set flex(flex) {
    const me = this;
    // Width explicitly set, remember that
    me.hasCalculatedWidth = false;
    me.header.flex = flex;
    me.footer.flex = flex;
    super.flex = flex;
    // When we're live, we can't wait until the  throttled resize occurs - it looks bad.
    if (me.isPainted) {
      me.onElementResize();
    }
  }
  get flex() {
    return super.flex;
  }
  /**
   * Called when grid changes size. SubGrid determines if it has changed size and triggers scroll (for virtual
   * rendering in cells to work when resizing etc.)
   * @private
   */
  onInternalResize(element, newWidth, newHeight, oldWidth, oldHeight) {
    const me = this,
      {
        grid
      } = me;
    // Widget caches dimensions
    super.onInternalResize(...arguments);
    // Unit tests create naked SubGrids so we have to do this.
    if (grid !== null && grid !== void 0 && grid.isPainted) {
      me.syncSplitterButtonPosition();
      if (newWidth !== oldWidth) {
        // trigger scroll, in case anything is done on scroll it needs to be done now also
        grid.trigger('horizontalScroll', {
          grid,
          subGrid: me,
          scrollLeft: me.scrollable.element.scrollLeft,
          scrollX: me.scrollable.x
        });
        // Update virtual scrollers, if they are ready
        me.fakeScroller && me.refreshFakeScroll();
        // Columns which are flexed, but as part of a grouped column cannot just have their flex
        // value reflected in the flex value of its cells. They are flexing a different available space.
        // These have to be set to the exact width and kept synced.
        grid.syncFlexedSubCols();
        me.fixRowWidthsInSafariEdge();
      }
      if (newHeight !== oldHeight) {
        // Call this to update cached _bodyHeight
        grid.onHeightChange();
      }
      me.trigger('afterInternalResize', me);
    }
  }
  /**
   * Keeps the parallel splitters in the header, footer and fake scroller synced in terms
   * of being collapsed or not.
   * @private
   */
  syncParallelSplitters(collapsed) {
    const me = this,
      {
        grid
      } = me;
    if (me.splitterElement && me.$showingSplitter) {
      me.toggleSplitterCls('b-grid-splitter-collapsed', collapsed);
    } else {
      // If we're the last, we don't own a splitter, we use the previous region's splitter
      const prevGrid = grid.getSubGrid(grid.getPreviousRegion(me.region));
      // If there's a splitter before us, sync it with our state.
      if (prevGrid && prevGrid.splitterElement) {
        prevGrid.syncParallelSplitters(collapsed);
      }
    }
  }
  onSplitterMouseEnter() {
    const me = this,
      {
        nextSibling
      } = me;
    // No hover effect when collapsed
    if (!me.collapsed && (!nextSibling || !nextSibling.collapsed)) {
      me.toggleSplitterCls('b-hover');
    }
    me.startSplitterButtonSyncing();
  }
  onSplitterMouseLeave() {
    const me = this,
      {
        nextSibling
      } = me;
    me.toggleSplitterCls('b-hover', false);
    if (!me.collapsed && (!nextSibling || !nextSibling.collapsed)) {
      me.stopSplitterButtonSyncing();
    }
  }
  startSplitterButtonSyncing() {
    const me = this;
    if (me.splitterElement) {
      me.syncSplitterButtonPosition();
      if (!me.splitterSyncScrollListener) {
        me.splitterSyncScrollListener = me.grid.scrollable.ion({
          scroll: 'syncSplitterButtonPosition',
          thisObj: me
        });
      }
    }
  }
  stopSplitterButtonSyncing() {
    if (this.splitterSyncScrollListener) {
      this.splitterSyncScrollListener();
      this.splitterSyncScrollListener = null;
    }
  }
  syncSplitterButtonPosition() {
    const {
      grid
    } = this;
    this.splitterButtons.style.top = `${grid.scrollable.y + (grid.bodyHeight - (this.headerSplitter ? grid.headerHeight : 0)) / 2}px`;
  }
  /**
   * Get the "viewport" for the SubGrid as a Rectangle
   * @property {Core.helper.util.Rectangle}
   * @readonly
   */
  get viewRectangle() {
    const {
      scrollable
    } = this;
    return new Rectangle(scrollable.x, scrollable.y, this.width || 0, this.rowManager.viewHeight);
  }
  /**
   * Called when updating column widths to apply 'b-has-flex' which is used when fillLastColumn is configured.
   * @internal
   */
  updateHasFlex() {
    this.scrollable.element.classList.toggle('b-has-flex', this.columns.hasFlex);
  }
  updateResizable(resizable) {
    this.splitterElements.forEach(splitter => DomHelper.toggleClasses(splitter, ['b-disabled'], !resizable));
  }
  /**
   * Resize all columns in the SubGrid to fit their width, according to their configured
   * {@link Grid.column.Column#config-fitMode}
   */
  resizeColumnsToFitContent() {
    this.grid.beginGridMeasuring();
    this.columns.visibleColumns.forEach(column => {
      column.resizeToFitContent(null, null, true);
    });
    this.grid.endGridMeasuring();
  }
  //endregion
  //region Scroll
  get overflowingHorizontally() {
    // We are not overflowing if collapsed
    return !this.collapsed && this.scrollable.hasOverflow('x');
  }
  get overflowingVertically() {
    // SubGrids never overflow vertically. They are full calculated content height.
    // The owning Grid scrolls all SubGrids vertically in its own overflowElement.
    return false;
  }
  /**
   * Fixes widths of fake scrollers
   * @private
   */
  refreshFakeScroll() {
    const me = this,
      {
        element,
        virtualScrollerElement,
        virtualScrollerWidth,
        header,
        footer,
        scrollable
      } = me,
      // Cannot use scrollWidth because its an integer and we need exact content size
      totalFixedWidth = [...header.contentElement.children].reduce(sumWidths, 0);
    // Use a fixed scroll width so that when grid is empty (e.g after filtering with no matches),
    // it is able to it maintain its scroll-x position and magic mouse swiping
    // in the grid area will produce horizontal scrolling.
    // https://github.com/bryntum/support/issues/3247
    scrollable.scrollWidth = totalFixedWidth;
    // Scroller lays out in the same way as subgrid.
    // If we are flexed, the scroller is flexed etc.
    virtualScrollerElement.style.width = element.style.width;
    virtualScrollerElement.style.flex = element.style.flex;
    virtualScrollerElement.style.minWidth = element.style.minWidth;
    virtualScrollerElement.style.maxWidth = element.style.maxWidth;
    header.scrollable.syncOverflowState();
    footer.scrollable.syncOverflowState();
    if (!me.collapsed) {
      if (me.overflowingHorizontally) {
        virtualScrollerWidth.style.width = `${scrollable.scrollWidth || 0}px`;
        // If *any* SubGrids have horizontal overflow, the main grid
        // has to show its virtual horizontal scrollbar.
        me.grid.virtualScrollers.classList.remove('b-hide-display');
      } else {
        virtualScrollerWidth.style.width = 0;
      }
    }
  }
  /**
   * Init scroll syncing for header and footer (if available).
   * @private
   */
  initScroll() {
    const me = this,
      {
        scrollable,
        virtualScrollerElement
      } = me;
    me.syncPartnersOnFrame = me.createOnFrame(me.syncScrollingPartners);
    if (BrowserHelper.isFirefox) {
      scrollable.element.addEventListener('wheel', event => {
        if (event.deltaX) {
          scrollable.x += event.deltaX;
          event.preventDefault();
        }
      });
    }
    // Create a Scroller for the fake horizontal scrollbar so that it can partner
    me.fakeScroller = new Scroller({
      element: virtualScrollerElement,
      overflowX: true,
      widget: me // To avoid more expensive style lookup for RTL
    });
    // Sync scrolling partners (header, footer) when our xScroller reports a scroll.
    // Also fires horizontalscroll
    scrollable.ion({
      scroll: 'onSubGridScroll',
      scrollend: 'onSubGridScrollEnd',
      thisObj: me
    });
    scrollable.addPartner(me.fakeScroller, 'x');
    scrollable.addPartner(me.header.scrollable, 'x');
    scrollable.addPartner(me.footer.scrollable, 'x');
    // Update virtual scrollers (previously updated too early from onInternalResize)
    me.refreshFakeScroll();
  }
  onSubGridScrollEnd() {
    const me = this;
    // If we do not have the direct update flag set which would ensure a sync in each scroll event
    // then ensure syncing happens on scroll end. This is for animated scrolls where the scroll
    // impulses come through animation frames.
    if (!me.forceScrollUpdate) {
      me.syncScrollingPartners();
    }
    me.scrolling = false;
    if (!DomHelper.scrollBarWidth) {
      me.grid.virtualScrollers.classList.remove('b-scrolling');
      // Remove interactivity a while after scrolling ended
      me.hideOverlayScroller();
    }
  }
  onSubGridScroll() {
    // Force direct update, without waiting for next animation frame
    if (this.forceScrollUpdate) {
      this.syncScrollingPartners();
      this.forceScrollUpdate = false;
    } else {
      this.syncPartnersOnFrame();
    }
  }
  showOverlayScroller() {
    this.hideOverlayScroller.cancel();
    this.virtualScrollerElement.classList.add('b-show-virtual-scroller');
  }
  // Buffered 1500ms, hides virtual scrollers after scrolling has ended
  hideOverlayScroller() {
    this.virtualScrollerElement.classList.remove('b-show-virtual-scroller');
  }
  set scrolling(scrolling) {
    this._scrolling = scrolling;
  }
  get scrolling() {
    return this._scrolling;
  }
  /**
   * This syncs the horizontal scroll position of the header and the footer with
   * the horizontal scroll position of the grid. Usually, this will be called automatically
   * when the grid scrolls. In some cases, such as a refresh caused by column changes
   * it will need to be called from elsewhere.
   * @internal
   */
  syncScrollingPartners(addCls = true) {
    const subGrid = this,
      {
        grid
      } = subGrid;
    if (!subGrid.scrolling && addCls) {
      subGrid.scrolling = true;
      // Allow interacting with overlaid scrollbar after scrolling starts
      if (!DomHelper.scrollBarWidth) {
        // Cls indicating that we are actively scrolling
        grid.virtualScrollers.classList.add('b-scrolling');
        // Cls sticking around longer to keep overlay scrollbar visible longer, allowing users to more easily
        // grab it to drag more
        subGrid.showOverlayScroller();
      }
    }
    grid.trigger('horizontalScroll', {
      subGrid,
      grid,
      scrollLeft: subGrid.scrollable.element.scrollLeft,
      scrollX: subGrid.scrollable.x
    });
  }
  /**
   * Scrolls a column into view (if it is not already). Called by Grid#scrollColumnIntoView, use it instead to not
   * have to care about which SubGrid contains a column.
   * @param {Grid.column.Column|String|Number} column Column name (data) or column index or actual column object.
   * @param {ScrollOptions} [options] How to scroll.
   * @returns {Promise} If the column exists, a promise which is resolved when the column header element has been
   * scrolled into view.
   */
  scrollColumnIntoView(column, options) {
    const {
        columns,
        header
      } = this,
      scroller = header.scrollable;
    // Allow column,column id,or column index to be passed
    column = column instanceof Column ? column : columns.get(column) || columns.getById(column) || columns.getAt(column);
    if (column) {
      // Get the current column header element.
      const columnHeaderElement = header.getHeader(column.id);
      if (columnHeaderElement) {
        return scroller.scrollIntoView(Rectangle.from(columnHeaderElement, null, true), options);
      }
    }
  }
  //endregion
  //region Rows
  /**
   * Creates elements for the new rows when RowManager has determined that more rows are needed
   * @private
   */
  onAddRow({
    rows,
    isExport
  }) {
    const me = this,
      config = me.rowElementConfig,
      frag = document.createDocumentFragment();
    rows.forEach(row => {
      const rowElement = DomHelper.createElement(config);
      frag.appendChild(rowElement);
      row.addElement(me.region, rowElement);
      me.fixCellWidths(rowElement);
    });
    // Do not insert elements to DOM if we're exporting them
    if (!isExport) {
      me.fixRowWidthsInSafariEdge();
      // Put the row elements into the SubGrid en masse.
      // If 2nd param is null, insertBefore appends.
      me.element.insertBefore(frag, me.insertRowsBefore);
    }
  }
  /**
   * Get all row elements for this SubGrid.
   * @property {HTMLElement[]}
   * @readonly
   */
  get rowElements() {
    return this.fromCache('.b-grid-row', true);
  }
  /**
   * Removes all row elements from the subgrids body and empties cache
   * @private
   */
  clearRows() {
    this.emptyCache();
    const all = this.element.querySelectorAll('.b-grid-row'),
      range = document.createRange();
    if (all.length) {
      range.setStartBefore(all[0]);
      range.setEndAfter(all[all.length - 1]);
      range.deleteContents();
    }
  }
  // only called when RowManager.rowScrollMode = 'dom', which is not intended to be used
  addNewRowElement() {
    const rowElement = DomHelper.append(this.element, this.rowElementConfig);
    this.fixCellWidths(rowElement);
    return rowElement;
  }
  get store() {
    return this.grid.store;
  }
  get rowManager() {
    var _this$grid;
    return (_this$grid = this.grid) === null || _this$grid === void 0 ? void 0 : _this$grid.rowManager;
  }
  //endregion
  // region Expand/collapse
  // All usages are commented, uncomment when this is resolved: https://app.assembla.com/spaces/bryntum/tickets/5472
  toggleTransitionClasses(doRemove = false) {
    const me = this,
      grid = me.grid,
      nextRegion = grid.getSubGrid(grid.getNextRegion(me.region)),
      splitter = grid.resolveSplitter(nextRegion);
    nextRegion.element.classList[doRemove ? 'remove' : 'add']('b-grid-subgrid-animate-collapse');
    nextRegion.header.element.classList[doRemove ? 'remove' : 'add']('b-grid-subgrid-animate-collapse');
    me.element.classList[doRemove ? 'remove' : 'add']('b-grid-subgrid-animate-collapse');
    me.header.element.classList[doRemove ? 'remove' : 'add']('b-grid-subgrid-animate-collapse');
    splitter.classList[doRemove ? 'remove' : 'add']('b-grid-splitter-animate');
  }
  /**
   * Get/set collapsed state
   * @property {Boolean}
   */
  get collapsed() {
    return this._collapsed;
  }
  set collapsed(collapsed) {
    if (this.isConfiguring) {
      this._collapsed = collapsed;
    } else {
      if (collapsed) {
        this.collapse();
      } else {
        this.expand();
      }
    }
  }
  /**
   * Collapses subgrid. If collapsing subgrid is the only one expanded, next subgrid to the right (or previous) will
   * be expanded.
   *
   * @example
   * let locked = grid.getSubGrid('locked');
   * locked.collapse().then(() => {
   *     console.log(locked.collapsed); // Logs 'True'
   * });
   *
   * let normal = grid.getSubGrid('normal');
   * normal.collapse().then(() => {
   *     console.log(locked.collapsed); // Logs 'False'
   *     console.log(normal.collapsed); // Logs 'True'
   * });
   *
   * @returns {Promise} A Promise which resolves when this SubGrid is fully collapsed.
   */
  async collapse() {
    const me = this,
      {
        grid,
        element
      } = me,
      nextRegion = grid.getSubGrid(grid.getNextRegion(me.region)),
      splitterOwner = me.splitterElement ? me : me.previousSibling;
    let {
        _beforeCollapseState
      } = me,
      // Count all expanded regions. Grid must always have at least one expanded region
      expandedRegions = 0;
    if (grid.rendered && me._collapsed === true) {
      return;
    }
    grid.eachSubGrid(subGrid => {
      subGrid !== me && !subGrid._collapsed && ++expandedRegions;
    });
    // Current region is the only one expanded, expand next region
    if (expandedRegions === 0) {
      // expandPromise = nextRegion.expand();
      await nextRegion.expand();
    }
    return new Promise(resolve => {
      if (!_beforeCollapseState) {
        _beforeCollapseState = me._beforeCollapseState = {};
        let widthChanged = false;
        // If current width is zero, the resize event will not be fired. In such case we want to trigger callback immediately
        if (me.width) {
          widthChanged = true;
          // Toggle transition classes here, we will actually change width below
          // me.toggleTransitionClasses();
          // afterinternalresize event is buffered, it will be fired only once after animation is finished
          // and element size is final
          me.ion({
            afterinternalresize: () => {
              // me.toggleTransitionClasses(true);
              resolve(me);
            },
            thisObj: me,
            once: true
          });
        }
        // When trying to collapse region we need its partner to occupy free space. Considering multiple
        // regions, several cases are possible:
        // 1) Both left and right regions have fixed width
        // 2) Left region has fixed width, right region is flexed
        // 3) Left region is flexed, right region has fixed width
        // 4) Both regions are flexed
        //
        // To collapse flexed region we need to remove flex style, remember it somehow and set fixed width.
        // If another region is flexed, it will fill the space. If it has fixed width, we need to increase
        // its width by collapsing region width. Same logic should be applied to headers.
        //
        // Save region width first
        _beforeCollapseState.width = me.width;
        _beforeCollapseState.elementWidth = element.style.width;
        // Next region is not flexed, need to make it fill the space
        if (nextRegion.element.style.flex === '') {
          _beforeCollapseState.nextRegionWidth = nextRegion.width;
          nextRegion.width = '';
          nextRegion.flex = '1';
        }
        // Current region is flexed, store style to restore on expand
        if (element.style.flex !== '') {
          _beforeCollapseState.flex = element.style.flex;
          // remove flex state to reduce width later
          me.header.element.style.flex = element.style.flex = '';
        }
        // Sets the grid to its collapsed width as defined in SASS: zero
        element.classList.add('b-grid-subgrid-collapsed');
        // The parallel elements which must be in sync width-wise must know about collapsing
        me.virtualScrollerElement.classList.add('b-collapsed');
        me.header.element.classList.add('b-collapsed');
        me.footer.element.classList.add('b-collapsed');
        me._collapsed = true;
        me.width = '';
        if (!widthChanged) {
          // sync splitters in case subGrid was collapsed by state (https://github.com/bryntum/support/issues/1857)
          me.syncParallelSplitters(true);
          resolve(false);
        }
      } else {
        resolve();
      }
      grid.trigger('subGridCollapse', {
        subGrid: me
      });
      grid.afterToggleSubGrid({
        subGrid: me,
        collapsed: true
      });
    }).then(value => {
      if (!me.isDestroyed) {
        if (value !== false) {
          var _splitterOwner$startS;
          grid.refreshVirtualScrollbars();
          me.syncParallelSplitters(true);
          // Our splitter is permanently visible when collapsed, so keep splitter button set
          // synced in the vertical centre of the view just in time for paint.
          // Uses translateY so will not cause a further layout.
          (_splitterOwner$startS = splitterOwner.startSplitterButtonSyncing) === null || _splitterOwner$startS === void 0 ? void 0 : _splitterOwner$startS.call(splitterOwner);
        }
      }
    });
  }
  /**
   * Expands subgrid.
   *
   * @example
   * grid.getSubGrid('locked').expand().then(() => console.log('locked grid expanded'));
   *
   * @returns {Promise} A Promise which resolves when this SubGrid is fully expanded.
   */
  async expand() {
    const me = this,
      {
        grid,
        _beforeCollapseState
      } = me,
      nextRegion = grid.getSubGrid(grid.getNextRegion(me.region)),
      splitterOwner = me.splitterElement ? me : me.previousSibling;
    if (grid.rendered && me._collapsed !== true) {
      return;
    }
    return new Promise(resolve => {
      if (_beforeCollapseState != null) {
        // If current width matches width expected after expand resize event will not be fired. In such case
        // we want to trigger callback immediately
        let widthChanged = false;
        // See similar clause in collapse method above
        if (me.width !== _beforeCollapseState.elementWidth) {
          widthChanged = true;
          // Toggle transition classes here, we will actually change width below
          // me.toggleTransitionClasses();
          me.ion({
            afterinternalresize() {
              // me.toggleTransitionClasses(true);
              // Delay the resolve to avoid "ResizeObserver loop limit exceeded" errors
              // collapsing the only expanded region and it has to expand its nextRegion
              // before it can collapse.
              me.setTimeout(() => resolve(me), 10);
            },
            thisObj: me,
            once: true
          });
        }
        // previous region is not flexed, reduce its width as it was increased in collapse
        if (_beforeCollapseState.nextRegionWidth) {
          nextRegion.width = _beforeCollapseState.nextRegionWidth;
          nextRegion.flex = null;
        }
        me.element.classList.remove('b-grid-subgrid-collapsed');
        me._collapsed = false;
        // The parallel elements which must be in sync width-wise must know about collapsing
        me.virtualScrollerElement.classList.remove('b-collapsed');
        me.header.element.classList.remove('b-collapsed');
        me.footer.element.classList.remove('b-collapsed');
        // This region used to be flex, let's restore it
        if (_beforeCollapseState.flex) {
          // Always restore width, restoring flex won't trigger resize otherwise
          me.width = _beforeCollapseState.width;
          // Widget flex setting clears style width
          me.header.flex = me.flex = _beforeCollapseState.flex;
          me.footer.flex = _beforeCollapseState.flex;
          me._width = null;
        } else {
          me.width = _beforeCollapseState.elementWidth;
        }
        me.element.classList.remove('b-grid-subgrid-collapsed');
        me._collapsed = false;
        if (!widthChanged) {
          resolve(false);
        } else {
          // Our splitter buttons are hidden when expanded, so we no longer need to keep splitter button set
          // synced in the vertical centre of the view.
          splitterOwner.stopSplitterButtonSyncing();
          me.syncParallelSplitters(false);
        }
        delete me._beforeCollapseState;
      } else {
        resolve();
      }
      grid.trigger('subGridExpand', {
        subGrid: me
      });
      grid.afterToggleSubGrid({
        subGrid: me,
        collapsed: false
      });
    });
  }
  hide() {
    var _this$header, _this$footer2;
    (_this$header = this.header) === null || _this$header === void 0 ? void 0 : _this$header.hide();
    (_this$footer2 = this.footer) === null || _this$footer2 === void 0 ? void 0 : _this$footer2.hide();
    this.hideSplitter();
    return super.hide();
  }
  show() {
    var _this$header2, _this$footer3;
    (_this$header2 = this.header) === null || _this$header2 === void 0 ? void 0 : _this$header2.show();
    (_this$footer3 = this.footer) === null || _this$footer3 === void 0 ? void 0 : _this$footer3.show();
    this.showSplitter();
    return super.show();
  }
  //endregion
}
// Register this widget type with its Factory
SubGrid.initClass();
SubGrid._$name = 'SubGrid';

/**
 * @module Grid/view/mixin/GridSubGrids
 */
/**
 * Mixin for grid that handles SubGrids. Each SubGrid is scrollable horizontally separately from the other SubGrids.
 * Having two SubGrids allows you to achieve what is usually called locked or frozen columns.
 *
 * By default a Grid has two SubGrids, one named 'locked' and one 'normal'. The `locked` region has fixed width, while
 * the `normal` region grows to fill all available width (flex).
 *
 * Which SubGrid a column belongs to is determined using its {@link Grid.column.Column#config-region} config. For
 * example to put a column into the locked region, specify `{ region: 'locked' }`. For convenience, a column can be put
 * in the locked region using `{ locked: true }`.
 *
 * ```javascript
 * new Grid({
 *   columns : [
 *     // These two columns both end up in the "locked" region
 *     { field: 'name', text: 'Name', locked: true }
 *     { field: 'age', text: 'Age', region: 'locked' }
 *   ]
 * });
 * ```
 *
 * To customize the SubGrids, use {@link Grid.view.Grid#config-subGridConfigs}:
 *
 * ```javascript
 * // change the predefined subgrids
 * new Grid({
 *   subGridConfigs : {
 *       locked : { flex : 1 } ,
 *       normal : { flex : 3 }
 *   }
 * })
 *
 * // or define your own entirely
 * new Grid({
 *   subGridConfigs : {
 *       a : { width : 150 } ,
 *       b : { flex  : 1 },
 *       c : { width : 150 }
 *   },
 *
 *   columns : [
 *       { field : 'name', text : 'Name', region : 'a' },
 *       ...
 *   ]
 * })
 * ```
 *
 * @demo Grid/lockedcolumns
 * @mixin
 */
var GridSubGrids = (Target => class GridSubGrids extends (Target || Base) {
  static get $name() {
    return 'GridSubGrids';
  }
  static get properties() {
    return {
      /**
       * An object containing the {@link Grid.view.SubGrid} region instances, indexed by subGrid id ('locked', normal'...)
       * @member {Object<String,Grid.view.SubGrid>} subGrids
       * @readonly
       * @category Common
       */
      subGrids: {}
    };
  }
  //region Init
  changeSubGridConfigs(configs) {
    const me = this,
      usedRegions = new Set();
    for (const column of me.columns) {
      const {
        region
      } = column;
      // Allow specifying regions for undefined subgrids
      if (region) {
        if (!configs[region]) {
          configs[region] = {};
        }
        usedRegions.add(region);
      }
    }
    // Implementer has provided configs for other subGrids but not normal, put defaults in place
    if (configs.normal && ObjectHelper.isEmpty(configs.normal)) {
      configs.normal = GridBase.defaultConfig.subGridConfigs.normal;
    }
    for (const region of usedRegions) {
      me.createSubGrid(region, configs[region]);
    }
    // Add them to Grid
    me.items = me.subGrids;
    return configs;
  }
  createSubGrid(region, config = null) {
    const me = this,
      subGridColumns = me.columns.makeChained(column => column.region === region, ['region']),
      subGridConfig = ObjectHelper.assign({
        type: 'subgrid',
        id: `${me.id}-${region}Subgrid`,
        parent: me,
        grid: me,
        region,
        headerClass: me.headerClass,
        footerClass: me.footerClass,
        columns: subGridColumns,
        // Sort by region unless weight is explicitly defined
        weight: region,
        // SubGridScrollers know about the main grid's scroller so that if asked to
        // scroll vertically they know how to do it.
        scrollable: {
          yScroller: me.scrollable
        }
      }, config || me.subGridConfigs[region]);
    let hasCalculatedWidth = false;
    if (!subGridConfig.flex && !subGridConfig.width) {
      subGridConfig.width = subGridColumns.totalFixedWidth;
      hasCalculatedWidth = true;
    }
    // Subclasses may inject a type property to create custom SubGrids
    const subGrid = me.subGrids[region] = SubGrid.create(subGridConfig);
    // Must be set after creation, otherwise reset in SubGrid#set width
    subGrid.hasCalculatedWidth = hasCalculatedWidth;
    if (region === me.regions[0]) {
      // Have already done lookups for this in a couple of places, might as well store it...
      subGrid.isFirstRegion = true;
    }
    return subGrid;
  }
  // A SubGrid is added to Grid, add its header etc. too
  onChildAdd(subGrid) {
    if (subGrid.isSubGrid) {
      const me = this,
        {
          items,
          headerContainer,
          virtualScrollers,
          footerContainer
        } = me,
        // 2 elements per index, actual element + splitter
        index = items.indexOf(subGrid) * 2;
      if (!me.hideHeaders) {
        DomHelper.insertAt(headerContainer, subGrid.headerSplitter, index);
        DomHelper.insertAt(headerContainer, subGrid.header.element, index);
      }
      DomHelper.insertAt(virtualScrollers, subGrid.scrollerSplitter, index);
      DomHelper.insertAt(virtualScrollers, subGrid.virtualScrollerElement, index);
      DomHelper.insertAt(footerContainer, subGrid.footerSplitter, index);
      DomHelper.insertAt(footerContainer, subGrid.footer.element, index);
      // Show splitter for all except last (new might not sort last, depending on weight)
      items.forEach((subGrid, i) => {
        if (i < items.length - 1) {
          subGrid.showSplitter();
        }
      });
      // Empty text should be displayed in the first subgrid
      if (index === 0 && me.emptyTextEl) {
        subGrid.element.appendChild(me.emptyTextEl);
      }
    }
    return super.onChildAdd(subGrid);
  }
  // A SubGrid is remove from grid, remove its header etc. too
  onChildRemove(subGrid) {
    super.onChildRemove(subGrid);
    if (subGrid.isSubGrid) {
      const {
        items
      } = this;
      delete this.subGrids[subGrid.region];
      ArrayHelper.remove(this.regions, subGrid.region);
      subGrid.destroy();
      // Make sure the new last splitter is hidden
      if (items.length) {
        items[items.length - 1].hideSplitter();
      }
    }
  }
  doDestroy() {
    this.eachSubGrid(subGrid => subGrid.destroy());
    super.doDestroy();
  }
  //endregion
  //region Iteration & calling
  /**
   * Iterate over all subGrids, calling the supplied function for each.
   * @param {Function} fn Function to call for each instance
   * @param {Object} thisObj `this` reference to call the function in, defaults to the subGrid itself
   * @category SubGrid
   * @internal
   */
  eachSubGrid(fn, thisObj = null) {
    this.items.forEach((subGrid, i) => {
      subGrid.isSubGrid && fn.call(thisObj || subGrid, subGrid, i++);
    });
  }
  /**
   * Call a function by name for all subGrids (that have the function).
   * @param {String} fnName Name of function to call, uses the subGrid itself as `this` reference
   * @param params Parameters to call the function with
   * @returns {*} Return value from first SubGrid is returned
   * @category SubGrid
   * @internal
   */
  callEachSubGrid(fnName, ...params) {
    let returnValue = null;
    this.items.forEach((subGrid, i) => {
      if (subGrid.isSubGrid && subGrid[fnName]) {
        const partialReturnValue = subGrid[fnName](...params);
        if (i === 0) returnValue = partialReturnValue;
      }
    });
    return returnValue;
  }
  //endregion
  //region Getters
  get regions() {
    return this.items.map(item => item.region);
  }
  /**
   * This method should return names of the two last regions in the grid as they are visible in the UI. In case
   * `regions` property cannot be trusted, use different approach. Used by SubGrid and RegionResize to figure out
   * which region should collapse or expand.
   * @returns {String[]}
   * @private
   * @category SubGrid
   */
  getLastRegions() {
    const result = this.regions.slice(-2);
    // ALWAYS return array of length 2 in order to avoid extra conditions. Normally should not be called with 1 region
    return result.length === 2 ? result : [result[0], result[0]];
  }
  /**
   * This method should return right neighbour for passed region, or left neighbour in case last visible region is passed.
   * This method is used to decide which subgrid should take space of the collapsed one.
   * @param {String} region
   * @returns {String}
   * @private
   * @category SubGrid
   */
  getNextRegion(region) {
    const regions = this.regions;
    // return next region or next to last
    return regions[regions.indexOf(region) + 1] || regions[regions.length - 2];
  }
  getPreviousRegion(region) {
    return this.regions[this.regions.indexOf(region) - 1];
  }
  /**
   * Returns the subGrid for the specified region.
   * @param {String} region Region, eg. locked or normal (per default)
   * @returns {Grid.view.SubGrid} A subGrid
   * @category SubGrid
   */
  getSubGrid(region) {
    return this.subGrids[region];
  }
  /**
   * Get the SubGrid that contains specified column
   * @param {String|Grid.column.Column} column Column "name" or column object
   * @returns {Grid.view.SubGrid}
   * @category SubGrid
   */
  getSubGridFromColumn(column) {
    column = column instanceof Column ? column : this.columns.getById(column) || this.columns.get(column);
    return this.getSubGrid(column.region);
  }
  //endregion
  /**
   * Returns splitter element for subgrid
   * @param {Grid.view.SubGrid|String} subGrid
   * @returns {HTMLElement}
   * @private
   * @category SubGrid
   */
  resolveSplitter(subGrid) {
    const regions = this.getLastRegions();
    let region = subGrid instanceof SubGrid ? subGrid.region : subGrid;
    if (regions[1] === region) {
      region = regions[0];
    }
    return this.subGrids[region].splitterElement;
  }
  // This does not need a className on Widgets.
  // Each *Class* which doesn't need 'b-' + constructor.name.toLowerCase() automatically adding
  // to the Widget it's mixed in to should implement thus.
  get widgetClass() {}
});

const emptyString = new String();
const locale = {
  localeName: 'En',
  localeDesc: 'English (US)',
  localeCode: 'en-US',
  ColumnPicker: {
    column: 'Column',
    columnsMenu: 'Columns',
    hideColumn: 'Hide column',
    hideColumnShort: 'Hide',
    newColumns: 'New columns'
  },
  Filter: {
    applyFilter: 'Apply filter',
    filter: 'Filter',
    editFilter: 'Edit filter',
    on: 'On',
    before: 'Before',
    after: 'After',
    equals: 'Equals',
    lessThan: 'Less than',
    moreThan: 'More than',
    removeFilter: 'Remove filter',
    disableFilter: 'Disable filter'
  },
  FilterBar: {
    enableFilterBar: 'Show filter bar',
    disableFilterBar: 'Hide filter bar'
  },
  Group: {
    group: 'Group',
    groupAscending: 'Group ascending',
    groupDescending: 'Group descending',
    groupAscendingShort: 'Ascending',
    groupDescendingShort: 'Descending',
    stopGrouping: 'Stop grouping',
    stopGroupingShort: 'Stop'
  },
  HeaderMenu: {
    moveBefore: text => `Move before "${text}"`,
    moveAfter: text => `Move after "${text}"`,
    collapseColumn: 'Collapse column',
    expandColumn: 'Expand column'
  },
  ColumnRename: {
    rename: 'Rename'
  },
  MergeCells: {
    mergeCells: 'Merge cells',
    menuTooltip: 'Merge cells with same value when sorted by this column'
  },
  Search: {
    searchForValue: 'Search for value'
  },
  Sort: {
    sort: 'Sort',
    sortAscending: 'Sort ascending',
    sortDescending: 'Sort descending',
    multiSort: 'Multi sort',
    removeSorter: 'Remove sorter',
    addSortAscending: 'Add ascending sorter',
    addSortDescending: 'Add descending sorter',
    toggleSortAscending: 'Change to ascending',
    toggleSortDescending: 'Change to descending',
    sortAscendingShort: 'Ascending',
    sortDescendingShort: 'Descending',
    removeSorterShort: 'Remove',
    addSortAscendingShort: '+ Ascending',
    addSortDescendingShort: '+ Descending'
  },
  Column: {
    columnLabel: column => `${column.text ? `${column.text} column. ` : ''}SPACE for context menu${column.sortable ? ', ENTER to sort' : ''}`,
    cellLabel: emptyString
  },
  Checkbox: {
    toggleRowSelect: 'Toggle row selection',
    toggleSelection: 'Toggle selection of entire dataset'
  },
  RatingColumn: {
    cellLabel: column => {
      var _column$location;
      return `${column.text ? column.text : ''} ${(_column$location = column.location) !== null && _column$location !== void 0 && _column$location.record ? `rating : ${column.location.record[column.field] || 0}` : ''}`;
    }
  },
  GridBase: {
    loadFailedMessage: 'Data loading failed!',
    syncFailedMessage: 'Data synchronization failed!',
    unspecifiedFailure: 'Unspecified failure',
    networkFailure: 'Network error',
    parseFailure: 'Failed to parse server response',
    serverResponse: 'Server response:',
    noRows: 'No records to display',
    moveColumnLeft: 'Move to left section',
    moveColumnRight: 'Move to right section',
    moveColumnTo: region => `Move column to ${region}`
  },
  CellMenu: {
    removeRow: 'Delete'
  },
  RowCopyPaste: {
    copyRecord: 'Copy',
    cutRecord: 'Cut',
    pasteRecord: 'Paste',
    rows: 'rows',
    row: 'row'
  },
  CellCopyPaste: {
    copy: 'Copy',
    cut: 'Cut',
    paste: 'Paste'
  },
  PdfExport: {
    'Waiting for response from server': 'Waiting for response from server...',
    'Export failed': 'Export failed',
    'Server error': 'Server error',
    'Generating pages': 'Generating pages...',
    'Click to abort': 'Cancel'
  },
  ExportDialog: {
    width: '40em',
    labelWidth: '12em',
    exportSettings: 'Export settings',
    export: 'Export',
    exporterType: 'Control pagination',
    cancel: 'Cancel',
    fileFormat: 'File format',
    rows: 'Rows',
    alignRows: 'Align rows',
    columns: 'Columns',
    paperFormat: 'Paper format',
    orientation: 'Orientation',
    repeatHeader: 'Repeat header'
  },
  ExportRowsCombo: {
    all: 'All rows',
    visible: 'Visible rows'
  },
  ExportOrientationCombo: {
    portrait: 'Portrait',
    landscape: 'Landscape'
  },
  SinglePageExporter: {
    singlepage: 'Single page'
  },
  MultiPageExporter: {
    multipage: 'Multiple pages',
    exportingPage: ({
      currentPage,
      totalPages
    }) => `Exporting page ${currentPage}/${totalPages}`
  },
  MultiPageVerticalExporter: {
    multipagevertical: 'Multiple pages (vertical)',
    exportingPage: ({
      currentPage,
      totalPages
    }) => `Exporting page ${currentPage}/${totalPages}`
  },
  RowExpander: {
    loading: 'Loading',
    expand: 'Expand',
    collapse: 'Collapse'
  }
};
LocaleHelper.publishLocale(locale);

//region Import
//endregion
/**
 * @module Grid/view/GridBase
 */
const resolvedPromise = new Promise(resolve => resolve()),
  storeListenerName = 'GridBase:store',
  defaultScrollOptions = {
    block: 'nearest',
    inline: 'nearest'
  },
  datasetReplaceActions = {
    dataset: 1,
    pageLoad: 1,
    filter: 1
  };
/**
 * A thin base class for {@link Grid.view.Grid}. Does not include any features by default, allowing smaller custom built
 * bundles if used in place of {@link Grid.view.Grid}.
 *
 * **NOTE:** In most scenarios you probably want to use Grid instead of GridBase.
 * @extends Core/widget/Panel
 *
 * @mixes Core/mixin/Pluggable
 * @mixes Core/mixin/State
 * @mixes Grid/view/mixin/GridElementEvents
 * @mixes Grid/view/mixin/GridFeatures
 * @mixes Grid/view/mixin/GridResponsive
 * @mixes Grid/view/mixin/GridSelection
 * @mixes Grid/view/mixin/GridState
 * @mixes Grid/view/mixin/GridSubGrids
 * @mixes Core/mixin/LoadMaskable
 *
 * @features Grid/feature/CellCopyPaste
 * @features Grid/feature/CellEdit
 * @features Grid/feature/CellMenu
 * @features Grid/feature/CellTooltip
 * @features Grid/feature/ColumnAutoWidth
 * @features Grid/feature/ColumnDragToolbar
 * @features Grid/feature/ColumnPicker
 * @features Grid/feature/ColumnRename
 * @features Grid/feature/ColumnReorder
 * @features Grid/feature/ColumnResize
 * @features Grid/feature/FillHandle
 * @features Grid/feature/Filter
 * @features Grid/feature/FilterBar
 * @features Grid/feature/Group
 * @features Grid/feature/GroupSummary
 * @features Grid/feature/HeaderMenu
 * @features Grid/feature/MergeCells
 * @features Grid/feature/QuickFind
 * @features Grid/feature/RegionResize
 * @features Grid/feature/RowCopyPaste
 * @features Grid/feature/RowExpander
 * @features Grid/feature/RowReorder
 * @features Grid/feature/Search
 * @features Grid/feature/Sort
 * @features Grid/feature/StickyCells
 * @features Grid/feature/Stripe
 * @features Grid/feature/Summary
 * @features Grid/feature/Tree
 * @features Grid/feature/TreeGroup
 *
 * @features Grid/feature/experimental/ExcelExporter
 *
 * @features Grid/feature/export/PdfExport
 * @features Grid/feature/export/exporter/MultiPageExporter
 * @features Grid/feature/export/exporter/MultiPageVerticalExporter
 * @features Grid/feature/export/exporter/SinglePageExporter
 *
 * @plugins Grid/row/RowManager
 * @widget
 */
class GridBase extends Panel.mixin(Pluggable, State, GridElementEvents, GridFeatures, GridNavigation, GridResponsive, GridSelection, GridState, GridSubGrids, LoadMaskable) {
  //region Config
  static get $name() {
    return 'GridBase';
  }
  // Factoryable type name
  static get type() {
    return 'gridbase';
  }
  static get delayable() {
    return {
      onGridVerticalScroll: {
        type: 'raf'
      },
      // These use a shorter delay for tests, see finishConfigure()
      bufferedAfterColumnsResized: 250,
      bufferedElementResize: 250
    };
  }
  static get configurable() {
    return {
      //region Hidden configs
      /**
       * @hideconfigs autoUpdateRecord, defaults, hideWhenEmpty, itemCls, items, layout, layoutStyle, lazyItems, namedItems, record, textContent, defaultAction, html, htmlCls, tag, textAlign, trapFocus, content, defaultBindProperty, ripple, defaultFocus, align, anchor, centered, constrainTo, draggable, floating, hideAnimation, positioned, scrollAction, showAnimation, x, y, localeClass, localizableProperties, showTooltipWhenDisabled, tooltip, strictRecordMapping,
       */
      /**
       * @hideproperties html, isSettingValues, isValid, items, record, values, content, layoutStyle, firstItem, lastItem, anchorSize, x, y, layout, strictRecordMapping, visibleChildCount,
       */
      /**
       * @hidefunctions attachTooltip, add, getWidgetById, insert, processWidgetConfig, remove, removeAll, getAt, alignTo, setXY, showBy, showByPoint, toFront
       */
      //endregion
      /**
       * Set to `true` to make the grid read-only, by disabling any UIs for modifying data.
       *
       * __Note that checks MUST always also be applied at the server side.__
       * @prp {Boolean} readOnly
       * @default false
       * @category Misc
       */
      /**
       * Automatically set grids height to fit all rows (no scrolling in the grid). In general you should avoid
       * using `autoHeight: true`, since it will bypass Grids virtual rendering and render all rows at once, which
       * in a larger grid is really bad for performance.
       * @config {Boolean}
       * @default false
       * @category Layout
       */
      autoHeight: null,
      /**
       * Configure this as `true` to allow elements within cells to be styled as `position: sticky`.
       *
       * Columns which contain sticky content will need to be configured with
       *
       * ```javascript
       *    cellCls : 'b-sticky-cell',
       * ```
       *
       * Or a custom renderer can add the class to the passed cell element.
       *
       * It is up to the application author how to style the cell content. It is recommended that
       * a custom renderer create content with CSS class names which the application author
       * will use to apply the `position`, and matching `margin-top` and `top` styles to keep the
       * content stuck at the grid's top.
       *
       * Note that not all browsers support this CSS feature. A cross browser alternative
       * is to use the {link Grid.feature.StickyCells StickyCells} Feature.
       * @config {Boolean}
       * @category Misc
       */
      enableSticky: null,
      /**
       * Set to true to allow text selection in the grid cells. Note, this cannot be used simultaneously with the
       * `RowReorder` feature.
       * @config {Boolean}
       * @default false
       * @category Selection
       */
      enableTextSelection: null,
      /**
       * Set to `true` to stretch the last column in a grid with all fixed width columns
       * to fill extra available space if the grid's width is wider than the sum of all
       * configured column widths.
       * @config {Boolean}
       * @default
       * @category Layout
       */
      fillLastColumn: true,
      /**
       * See {@link Grid.view.Grid#keyboard-shortcuts Keyboard shortcuts} for details
       * @config {Object<String,String>} keyMap
       * @category Common
       */
      positionMode: 'translate',
      // translate, translate3d, position
      /**
       * Configure as `true` to have the grid show a red "changed" tag in cells who's
       * field value has changed and not yet been committed.
       * @config {Boolean}
       * @default false
       * @category Misc
       */
      showDirty: null,
      /**
       * An object containing sub grid configuration objects keyed by a `region` property.
       * By default, grid has a 'locked' region (if configured with locked columns) and a 'normal' region.
       * The 'normal' region defaults to use `flex: 1`.
       *
       * This config can be used to reconfigure the "built in" sub grids or to define your own.
       *
       * Redefining the default regions:
       *
       * {@frameworktabs}
       * {@js}
       * ```javascript
       * new Grid({
       *   subGridConfigs : {
       *     locked : { flex : 1 },
       *     normal : { width : 100 }
       *   }
       * });
       * ```
       * {@endjs}
       * {@react}
       * ```jsx
       * const App = props => {
       *     const subGridConfigs = {
       *         locked : { flex : 1 },
       *         normal : { width : 100 }
       *     };
       *
       *     return <bryntum-grid subGridConfigs={subGridConfigs} />
       * }
       * ```
       * {@endreact}
       * {@vue}
       * ```html
       * <bryntum-grid :sub-grid-configs="subGridConfigs" />
       * ```
       * ```javascript
       * export default {
       *     setup() {
       *         return {
       *             subGridConfigs : [
       *                 locked : { flex : 1 },
       *                 normal : { width : 100 }
       *             ]
       *         };
       *     }
       * }
       * ```
       * {@endvue}
       * {@angular}
       * ```html
       * <bryntum-grid [subGridConfigs]="subGridConfigs"></bryntum-grid>
       * ```
       * ```typescript
       * export class AppComponent {
       *      subGridConfigs = [
       *          locked : { flex : 1 },
       *          normal : { width : 100 }
       *      ]
       *  }
       * ```
       * {@endangular}
       * {@endframeworktabs}
       *
       * Defining your own multi region grid:
       *
       * ```javascript
       * new Grid({
       *   subGridConfigs : {
       *     left   : { width : 100 },
       *     middle : { flex : 1 },
       *     right  : { width  : 100 }
       *   },
       *
       *   columns : [
       *     { field : 'manufacturer', text: 'Manufacturer', region : 'left' },
       *     { field : 'model', text: 'Model', region : 'middle' },
       *     { field : 'year', text: 'Year', region : 'middle' },
       *     { field : 'sales', text: 'Sales', region : 'right' }
       *   ]
       * });
       * ```
       * @config {Object<String,SubGridConfig>}
       * @category Misc
       */
      subGridConfigs: {
        normal: {
          flex: 1
        }
      },
      /**
       * Store that holds records to display in the grid, or a store config object. If the configuration contains
       * a `readUrl`, an `AjaxStore` will be created.
       *
       * Note that a store will be created during initialization if none is specified.
       *
       * Supplying a store config object at initialization time:
       *
       * ```javascript
       * const grid = new Grid({
       *     store : {
       *         fields : ['name', 'powers'],
       *         data   : [
       *             { id : 1, name : 'Aquaman', powers : 'Decent swimmer' },
       *             { id : 2, name : 'Flash', powers : 'Pretty fast' },
       *         ]
       *     }
       * });
       * ```
       *
       * Accessing the store at runtime:
       *
       * ```javascript
       * grid.store.sort('powers');
       * ```
       *
       * @prp {Core.data.Store}
       * @accepts {Core.data.Store|StoreConfig}
       * @category Common
       */
      store: {
        value: {},
        $config: 'nullify'
      },
      rowManager: {
        value: {},
        $config: ['nullify', 'lazy']
      },
      /**
       * Configuration values for the {@link Core.util.ScrollManager} class on initialization. Returns the
       * {@link Core.util.ScrollManager} at runtime.
       *
       * @prp {Core.util.ScrollManager}
       * @accepts {ScrollManagerConfig|Core.util.ScrollManager}
       * @readonly
       * @category Scrolling
       */
      scrollManager: {
        value: {},
        $config: ['nullify', 'lazy']
      },
      /**
       * Accepts column definitions for the grid during initialization. They will be used to create
       * {@link Grid/column/Column} instances that are added to a {@link Grid/data/ColumnStore}.
       *
       * At runtime it is read-only and returns the {@link Grid/data/ColumnStore}.
       *
       * Initialization using column config objects:
       *
       * ```javascript
       * new Grid({
       *   columns : [
       *     { text : 'Alias', field : 'alias' },
       *     { text : 'Superpower', field : 'power' }
       *   ]
       * });
       * ```
       *
       * Also accepts a store config object:
       *
       * ```javascript
       * new Grid({
       *   columns : {
       *     data : [
       *       { text : 'Alias', field : 'alias' },
       *       { text : 'Superpower', field : 'power' }
       *     ],
       *     listeners : {
       *       update() {
       *         // Some update happened
       *       }
       *     }
       *   }
       * });
       * ```
       *
       * Access the {@link Grid/data/ColumnStore} at runtime to manipulate columns:
       *
       * ```javascript
       * grid.columns.add({ field : 'column', text : 'New column' });
       * ```
       * @prp {Grid.data.ColumnStore}
       * @accepts {Grid.data.ColumnStore|GridColumnConfig[]|ColumnStoreConfig}
       * @readonly
       * @category Common
       */
      columns: {
        value: [],
        $config: 'nullify'
      },
      /**
       * Grid's `min-height`. Defaults to `10em` to be sure that the Grid always has a height wherever it is
       * inserted.
       *
       * Can be either a String or a Number (which will have 'px' appended).
       *
       * Note that _reading_ the value will return the numeric value in pixels.
       *
       * @config {String|Number}
       * @category Layout
       */
      minHeight: '10em',
      hideFooters: true,
      contentElMutationObserver: false,
      trapFocus: false,
      ariaElement: 'bodyElement',
      cellTabIndex: -1,
      rowCls: {
        value: 'b-grid-row',
        $config: {
          merge: this.mergeCls
        }
      },
      cellCls: {
        value: 'b-grid-cell',
        $config: {
          merge: this.mergeCls
        }
      },
      /**
       * Text or HTML to display when there is no data to display in the grid
       * @prp {String}
       * @default
       * @category Common
       */
      emptyText: 'L{noRows}',
      sortFeatureStore: 'store'
    };
  }
  // Default settings, applied in grids constructor.
  static get defaultConfig() {
    return {
      /**
       * Row height in pixels. This allows the default height for rows to be controlled. Note that it may be
       * overriden by specifying a {@link Grid/data/GridRowModel#field-rowHeight} on a per record basis, or from
       * a column {@link Grid/column/Column#config-renderer}.
       *
       * When initially configured as `null`, an empty row will be measured and its height will be used as default
       * row height, enabling it to be controlled using CSS
       *
       * @prp {Number}
       * @category Common
       */
      rowHeight: null,
      /**
       * Use fixed row height. Setting this to `true` will configure the underlying RowManager to use fixed row
       * height, which sacrifices the ability to use rows with variable height to gain a fraction better
       * performance.
       *
       * Using this setting also ignores the {@link Grid.view.GridBase#config-getRowHeight} function, and thus any
       * row height set in data. Only Grids configured {@link Grid.view.GridBase#config-rowHeight} is used.
       *
       * @config {Boolean}
       * @category Layout
       */
      fixedRowHeight: null,
      /**
       * A function called for each row to determine its height. It is passed a {@link Core.data.Model record} and
       * expected to return the desired height of that records row. If the function returns a falsy value, Grids
       * configured {@link Grid.view.GridBase#config-rowHeight} is used.
       *
       * The default implementation of this function returns the row height from the records
       * {@link Grid.data.GridRowModel#field-rowHeight rowHeight field}.
       *
       * Override this function to take control over how row heights are determined:
       *
       * ```javascript
       * new Grid({
       *    getRowHeight(record) {
       *        if (record.low) {
       *            return 20;
       *        }
       *        else if (record.high) {
       *            return 60;
       *        }
       *
       *        // Will use grids configured rowHeight
       *        return null;
       *    }
       * });
       * ```
       *
       * NOTE: Height set in a Column renderer takes precedence over the height returned by this function.
       *
       * @config {Function} getRowHeight
       * @param {Core.data.Model} getRowHeight.record Record to determine row height for
       * @returns {Number} Desired row height
       * @category Layout
       */
      // used if no rowHeight specified and none found in CSS. not public since our themes have row height
      // specified and this is more of an internal failsafe
      defaultRowHeight: 45,
      /**
       * Refresh entire row when a record changes (`true`) or, if possible, only the cells affected (`false`).
       *
       * When this is set to `false`, then if a column uses a renderer, cells in that column will still
       * be updated because it is impossible to know whether the cells value will be affected.
       *
       * If a standard, provided Column class is used with no custom renderer, its cells will only be updated
       * if the column's {@link Grid.column.Column#config-field} is changed.
       * @config {Boolean}
       * @default
       * @category Misc
       */
      fullRowRefresh: true,
      /**
       * True to not create any grid column headers
       * @config {Boolean}
       * @default false
       * @category Misc
       */
      hideHeaders: null,
      /**
       * Specify `true` to preserve vertical scroll position after store actions that trigger a `refresh` event,
       * such as loading new data and filtering.
       * @config {Boolean}
       * @default false
       * @category Misc
       */
      preserveScrollOnDatasetChange: null,
      /**
       * True to preserve focused cell after loading new data
       * @config {Boolean}
       * @default
       * @category Misc
       */
      preserveFocusOnDatasetChange: true,
      /**
       * Convenient shortcut to set data in grids store both during initialization and at runtime. Can also be
       * used to retrieve data at runtime, although we do recommend interacting with Grids store instead using
       * the {@link #property-store} property.
       *
       * Setting initial data during initialization:
       *
       * ```javascript
       * const grid = new Grid({
       *     data : [
       *       { id : 1, name : 'Batman' },
       *       { id : 2, name : 'Robin' },
       *       ...
       *     ]
       * });
       * ```
       *
       * Setting data at runtime:
       *
       * ```javascript
       * grid.data = [
       *     { id : 3, name : 'Joker' },
       *     ...
       * ];
       * ```
       *
       * Getting data at runtime:
       *
       * ```javascript
       * const records = store.data;
       * ```
       *
       * Note that a Store will be created during initialization if none is specified.
       *
       * @prp {Core.data.Model[]}
       * @accepts {Object[]|Core.data.Model[]}
       * @category Common
       */
      data: null,
      /**
       * Region to which columns are added when they have none specified
       * @config {String}
       * @default
       * @category Misc
       */
      defaultRegion: 'normal',
      /**
       * true to destroy the store when the grid is destroyed
       * @config {Boolean}
       * @default false
       * @category Misc
       */
      destroyStore: null,
      /**
       * Grids change the `maskDefaults` to cover only their `body` element.
       * @config {MaskConfig}
       * @category Misc
       */
      maskDefaults: {
        cover: 'body',
        target: 'element'
      },
      /**
       * Set to `false` to inhibit column lines during initialization or assign to it at runtime to toggle column
       * line visibility.
       *
       * End result might be overruled by/differ between themes.
       *
       * @prp {Boolean}
       * @default
       * @category Misc
       */
      columnLines: true,
      /**
       * Set to `false` to only measure cell contents when double clicking the edge between column headers.
       * @config {Boolean}
       * @default
       * @category Layout
       */
      resizeToFitIncludesHeader: true,
      /**
       * Set to `false` to prevent remove row animation and remove the delay related to that.
       * @config {Boolean}
       * @default
       * @category Misc
       */
      animateRemovingRows: true,
      /**
       * Set to `true` to not get a warning when using another base class than GridRowModel for your grid data. If
       * you do, and would like to use the full feature set of the grid then include the fields from GridRowModel
       * in your model definition.
       * @config {Boolean}
       * @default false
       * @category Misc
       */
      disableGridRowModelWarning: null,
      headerClass: Header,
      footerClass: Footer,
      testPerformance: false,
      rowScrollMode: 'move',
      // move, dom, all
      /**
       * Grid monitors window resize by default.
       * @config {Boolean}
       * @default true
       * @category Misc
       */
      monitorResize: true,
      /**
       * An object containing Feature configuration objects (or `true` if no configuration is required)
       * keyed by the Feature class name in all lowercase.
       * @config {Object}
       * @category Common
       */
      features: true,
      /**
       * Configures whether the grid is scrollable in the `Y` axis. This is used to configure a {@link Grid.util.GridScroller}.
       * See the {@link #config-scrollerClass} config option.
       * @config {Boolean|ScrollerConfig|Core.helper.util.Scroller}
       * @category Scrolling
       */
      scrollable: {
        // Just Y for now until we implement a special grid.view.Scroller subclass
        // Which handles the X scrolling of subgrids.
        overflowY: true
      },
      /**
       * The class to instantiate to use as the {@link #config-scrollable}. Defaults to {@link Grid.util.GridScroller}.
       * @config {Core.helper.util.Scroller}
       * @typings {typeof Scroller}
       * @category Scrolling
       */
      scrollerClass: GridScroller,
      refreshSuspended: 0,
      /**
       * Animation transition duration in milliseconds.
       * @prp {Number}
       * @default
       * @category Misc
       */
      transitionDuration: 500,
      /**
       * Event which is used to show context menus.
       * Available options are: 'contextmenu', 'click', 'dblclick'.
       * @config {'contextmenu'|'click'|'dblclick'}
       * @category Misc
       * @default
       */
      contextMenuTriggerEvent: 'contextmenu',
      localizableProperties: ['emptyText'],
      asyncEventSuffix: '',
      fixElementHeightsBuffer: 350,
      testConfig: {
        transitionDuration: 50,
        fixElementHeightsBuffer: 50
      }
    };
  }
  static get properties() {
    return {
      _selectedRecords: [],
      _verticalScrollHeight: 0,
      virtualScrollHeight: 0,
      _scrollTop: null
    };
  }
  // Keep this commented out to have easy access to the syntax next time we need to use it
  // static get deprecatedEvents() {
  //     return {
  //         cellContextMenuBeforeShow : {
  //             product            : 'Grid',
  //             invalidAsOfVersion : '5.0.0',
  //             message            : '`cellContextMenuBeforeShow` event is deprecated, in favor of `cellMenuBeforeShow` event. Please see https://bryntum.com/products/grid/docs/guide/Grid/upgrades/4.0.0 for more information.'
  //         }
  //     };
  // }
  //endregion
  //region Init-destroy
  finishConfigure(config) {
    const me = this,
      {
        initScroll
      } = me;
    // Make initScroll a one time only call
    me.initScroll = () => !me.scrollInitialized && initScroll.call(me);
    if (VersionHelper.isTestEnv) {
      me.bufferedAfterColumnsResized.delay = 50;
      me.bufferedElementResize.delay = 50;
    }
    super.finishConfigure(config);
    // When locale is applied columns react and change, which triggers `change` event on columns store for each
    // changed column, and every change normally triggers rendering view. This overhead becomes noticeable with
    // larger amount of columns. So we set two listeners to locale events: prioritized listener to be executed first
    // and suspend renderContents method and unprioritized one to resume method and call it immediately.
    LocaleManagerSingleton.ion({
      locale: 'onBeforeLocaleChange',
      prio: 1,
      thisObj: me
    });
    LocaleManagerSingleton.ion({
      locale: 'onLocaleChange',
      prio: -1,
      thisObj: me
    });
    GlobalEvents.ion({
      theme: 'onThemeChange',
      thisObj: me
    });
    me.ion({
      subGridExpand: 'onSubGridExpand',
      prio: -1,
      thisObj: me
    });
    // Buffered for scrolling, to be called
    me.bufferedFixElementHeights = me.buffer('fixElementHeights', me.fixElementHeightsBuffer, me);
    // Add the extra grid classes to the element
    me.setGridClassList(me.element.classList);
  }
  onSubGridExpand() {
    // Need to rerender all rows, because if the rows were rerendered (by adding a new column to another region for example)
    // while the region was collapsed, cells in the region will be empty.
    this.renderContents();
  }
  onBeforeLocaleChange() {
    this._suspendRenderContentsOnColumnsChanged = true;
  }
  onLocaleChange() {
    this._suspendRenderContentsOnColumnsChanged = false;
    if (this.isPainted) {
      this.renderContents();
    }
  }
  finalizeInit() {
    super.finalizeInit();
    if (this.store.isLoading) {
      // Maybe show loadmask if store is already loading when grid is constructed
      this.onStoreBeforeRequest();
    }
  }
  changeScrollManager(scrollManager, oldScrollManager) {
    oldScrollManager === null || oldScrollManager === void 0 ? void 0 : oldScrollManager.destroy();
    if (scrollManager) {
      return ScrollManager.new({
        element: this.element,
        owner: this
      }, scrollManager);
    } else {
      return null;
    }
  }
  /**
   * Cleanup
   * @private
   */
  doDestroy() {
    var _me$scrollManager;
    const me = this;
    me.detachListeners(storeListenerName);
    (_me$scrollManager = me.scrollManager) === null || _me$scrollManager === void 0 ? void 0 : _me$scrollManager.destroy();
    for (const feature of Object.values(me.features)) {
      var _feature$destroy;
      (_feature$destroy = feature.destroy) === null || _feature$destroy === void 0 ? void 0 : _feature$destroy.call(feature);
    }
    me._focusedCell = null;
    me.columns.destroy();
    super.doDestroy();
  }
  /**
   * Adds extra classes to the Grid element after it's been configured.
   * Also iterates through features, thus ensuring they have been initialized.
   * @private
   */
  setGridClassList(classList) {
    const me = this;
    Object.values(me.features).forEach(feature => {
      if (feature.disabled) {
        return;
      }
      let featureClass;
      if (Object.prototype.hasOwnProperty.call(feature.constructor, 'featureClass')) {
        featureClass = feature.constructor.featureClass;
      } else {
        featureClass = `b-${feature instanceof Base ? feature.$$name : feature.constructor.name}`;
      }
      if (featureClass) {
        classList.add(featureClass.toLowerCase());
      }
    });
  }
  //endregion
  // region Feature events
  // For documentation & typings purposes
  /**
   * Fires after a sub grid is collapsed.
   * @event subGridCollapse
   * @param {Grid.view.Grid} source The firing Grid instance
   * @param {Grid.view.SubGrid} subGrid The sub grid instance
   */
  /**
   * Fires after a sub grid is expanded.
   * @event subGridExpand
   * @param {Grid.view.Grid} source The firing Grid instance
   * @param {Grid.view.SubGrid} subGrid The sub grid instance
   */
  /**
   * Fires before a row is rendered.
   * @event beforeRenderRow
   * @param {Grid.view.Grid} source The firing Grid instance.
   * @param {Grid.row.Row} row The row about to be rendered.
   * @param {Core.data.Model} record The record for the row.
   * @param {Number} recordIndex The zero-based index of the record.
   */
  /**
   * Fires after a row is rendered.
   * @event renderRow
   * @param {Grid.view.Grid} source The firing Grid instance.
   * @param {Grid.row.Row} row The row that has been rendered.
   * @param {Core.data.Model} record The record for the row.
   * @param {Number} recordIndex The zero-based index of the record.
   */
  //endregion
  //region Grid template & elements
  compose() {
    const {
      autoHeight,
      enableSticky,
      enableTextSelection,
      fillLastColumn,
      positionMode,
      showDirty
    } = this;
    return {
      class: {
        [`b-grid-${positionMode}`]: 1,
        'b-enable-sticky': enableSticky,
        'b-grid-notextselection': !enableTextSelection,
        'b-autoheight': autoHeight,
        'b-fill-last-column': fillLastColumn,
        'b-show-dirty': showDirty
      }
    };
  }
  get cellCls() {
    const {
      _cellCls
    } = this;
    // It may have been merged to create a DomClassList, but 90% of the time will be a simple string.
    return _cellCls.value || _cellCls;
  }
  get bodyConfig() {
    const {
      autoHeight,
      hideFooters,
      hideHeaders
    } = this;
    return {
      reference: 'bodyElement',
      className: {
        'b-autoheight': autoHeight,
        'b-grid-panel-body': 1
      },
      // Only include aria-labelled-by if we have a header
      [this.hasHeader ? 'ariaLabelledBy' : '']: `${this.id}-panel-title`,
      children: {
        headerContainer: {
          tag: 'header',
          role: 'row',
          'aria-rowindex': 1,
          className: {
            'b-grid-header-container': 1,
            'b-hidden': hideHeaders
          }
        },
        bodyContainer: {
          className: 'b-grid-body-container',
          tabIndex: -1,
          // Explicitly needs this because it's in theory focusable
          // and DomSync won't add a default role
          role: 'presentation',
          children: {
            verticalScroller: {
              className: 'b-grid-vertical-scroller'
            }
          }
        },
        virtualScrollers: {
          className: 'b-virtual-scrollers b-hide-display',
          style: BrowserHelper.isFirefox && DomHelper.scrollBarWidth ? {
            height: `${DomHelper.scrollBarWidth}px`
          } : undefined
        },
        footerContainer: {
          tag: 'footer',
          className: {
            'b-grid-footer-container': 1,
            'b-hidden': hideFooters
          }
        }
      }
    };
  }
  get contentElement() {
    return this.verticalScroller;
  }
  get overflowElement() {
    return this.bodyContainer;
  }
  updateHideFooters(hide) {
    var _this$footerContainer;
    (_this$footerContainer = this.footerContainer) === null || _this$footerContainer === void 0 ? void 0 : _this$footerContainer.classList[hide ? 'add' : 'remove']('b-hidden');
  }
  //endregion
  //region Columns
  changeColumns(columns, currentStore) {
    const me = this;
    // Empty, clear or destroy store
    if (!columns && currentStore) {
      // Destroy when Grid is destroyed, if we created the ColumnStore
      if (me.isDestroying) {
        currentStore.owner === me && currentStore.destroy();
      }
      // Clear if set to falsy value at some other point
      else {
        currentStore.removeAll();
      }
      return currentStore;
    }
    // Keep store if configured with one
    if (columns.isStore) {
      (currentStore === null || currentStore === void 0 ? void 0 : currentStore.owner) === me && currentStore.destroy();
      columns.grid = me;
      return columns;
    }
    // Given an array of columns
    if (Array.isArray(columns)) {
      // If we have a store, plug them in
      if (currentStore) {
        const columnsBefore = currentStore.allRecords.slice();
        currentStore.data = columns;
        // Destroy any columns that were removed
        for (const oldColumn of columnsBefore) {
          if (!currentStore.includes(oldColumn)) {
            oldColumn.destroy();
          }
        }
        return currentStore;
      }
      // No store, use as data for a new store below
      columns = {
        data: columns
      };
    }
    if (currentStore) {
      throw new Error('Replacing ColumnStore is not supported');
    }
    // Assuming a store config object
    return ColumnStore.new({
      grid: me,
      owner: me
    }, columns);
  }
  updateColumns(columns, was) {
    var _super$updateColumns, _me$bodyElement;
    const me = this;
    (_super$updateColumns = super.updateColumns) === null || _super$updateColumns === void 0 ? void 0 : _super$updateColumns.call(this, columns, was);
    // changes might be triggered when applying state, before grid is rendered
    columns.ion({
      refresh: me.onColumnsChanged,
      change: me.onColumnsChanged,
      sort: me.onColumnsChanged,
      thisObj: me
    });
    columns.ion(columnResizeEvent(me.onColumnsResized, me));
    // Add touch class for touch devices
    if (BrowserHelper.isTouchDevice) {
      me.touch = true;
      // apply touchConfig for columns that defines it
      columns.forEach(column => {
        const {
          touchConfig
        } = column;
        if (touchConfig) {
          column.applyState(touchConfig);
        }
      });
    }
    (_me$bodyElement = me.bodyElement) === null || _me$bodyElement === void 0 ? void 0 : _me$bodyElement.setAttribute('aria-colcount', columns.visibleColumns.length);
  }
  onColumnsChanged({
    type,
    action,
    changes,
    record: column,
    records: changedColumns,
    isMove
  }) {
    const me = this,
      {
        columns,
        checkboxSelectionColumn
      } = me,
      isSingleFieldChange = changes && Object.keys(changes).length === 1;
    if (type === 'refresh' && action !== 'batch' ||
    // Ignore the update of parentIndex following a column move (we redraw on the insert)
    action === 'update' && isSingleFieldChange && 'parentIndex' in changes ||
    // Ignore sort caused by sync, will refresh on the batch instead
    action === 'sort' && columns.isSyncingDataOnLoad) {
      return;
    }
    const addingColumnToNonExistingSubGrid = action === 'add' && changedColumns.some(col => col.region && !me.subGrids[col.region]);
    // this.onPaint will handle changes caused by updateResponsive
    if (me.isConfiguring || !addingColumnToNonExistingSubGrid && (!me.isPainted || isMove && action === 'remove')) {
      return;
    }
    // See if we have to create and add new SubGrids to accommodate new columns.
    if (action === 'add') {
      for (const column of changedColumns) {
        const {
          region
        } = column;
        // See if there's a home for this column, if not, add one
        if (!me.subGrids[region]) {
          var _me$subGridConfigs;
          me.add(me.createSubGrid(region, (_me$subGridConfigs = me.subGridConfigs) === null || _me$subGridConfigs === void 0 ? void 0 : _me$subGridConfigs[region]));
        }
      }
    }
    if (action === 'update') {
      // Just updating width is already handled in a minimal way.
      if (('width' in changes || 'minWidth' in changes || 'maxWidth' in changes || 'flex' in changes) && !('region' in changes)) {
        // Update any leaf columns that want to be repainted on size change
        const {
          region
        } = column;
        // We must not capture visibleColumns from the columns var
        // at the top. It's a cached/recalculated value that we
        // are invalidating in the body of this function.
        columns.visibleColumns.forEach(col => {
          if (col.region === region && col.repaintOnResize) {
            me.refreshColumn(col);
          }
        });
        me.afterColumnsChange({
          action,
          changes,
          column
        });
        return;
      }
      // No repaint if only changing column text
      if ('text' in changes && isSingleFieldChange) {
        column.subGrid.refreshHeader();
        return;
      }
      // Column toggled, need to recheck if any visible column has flex
      if ('hidden' in changes) {
        const subGrid = me.getSubGridFromColumn(column.id);
        subGrid.header.fixHeaderWidths();
        subGrid.footer.fixFooterWidths();
        subGrid.updateHasFlex();
      }
    }
    // Might have to add or remove subgrids when assigning a new set of columns or when changing region
    if (action === 'dataset' || action === 'batch' || action === 'update' && 'region' in changes) {
      const regions = columns.getDistinctValues('region', true),
        {
          toRemove,
          toAdd
        } = ArrayHelper.delta(regions, me.regions, true);
      me.remove(toRemove.map(region => me.getSubGrid(region)));
      me.add(toAdd.map(region => me.createSubGrid(region, me.subGridConfigs[region])));
    }
    // Check if checkbox selection column was removed, if so insert it back as the first column
    if (checkboxSelectionColumn && !columns.includes(checkboxSelectionColumn)) {
      // Insert the checkbox after any rownumber column. If not there, -1 means in at 0.
      const insertIndex = columns.indexOf(columns.findRecord('type', 'rownumber')) + 1;
      columns.insert(insertIndex, checkboxSelectionColumn, true);
    }
    if (!me._suspendRenderContentsOnColumnsChanged) {
      me.renderContents();
    }
    // Columns which are flexed, but as part of a grouped column cannot just have their flex
    // value reflected in the flex value of its cells. They are flexing a different available space.
    // These have to be set to the exact width and kept synced.
    me.syncFlexedSubCols();
    // We must not capture visibleColumns from the columns var
    // at the top. It's a cached/recalculated value that we must
    // are invalidating in the body of this function.
    me.bodyElement.setAttribute('aria-colcount', columns.visibleColumns.length);
    me.afterColumnsChange({
      action,
      changes,
      column,
      columns: changedColumns
    });
  }
  onColumnsResized({
    changes,
    record: column
  }) {
    const me = this;
    if (me.isConfiguring) {
      return;
    }
    const domWidth = DomHelper.setLength(column.width),
      domMinWidth = DomHelper.setLength(column.minWidth),
      domMaxWidth = DomHelper.setLength(column.maxWidth),
      subGrid = me.getSubGridFromColumn(column.id);
    // Let header and footer fix their own widths
    subGrid.header.fixHeaderWidths();
    subGrid.footer.fixFooterWidths();
    subGrid.updateHasFlex();
    // We can't apply flex from flexed subColums - they are flexing inside a different available width.
    if (!(column.flex && column.childLevel)) {
      if (!me.cellEls || column !== me.lastColumnResized) {
        me.cellEls = DomHelper.children(me.element, `.b-grid-cell[data-column-id="${column.id}"]`);
        me.lastColumnResized = column;
      }
      for (const cell of me.cellEls) {
        if ('width' in changes) {
          // https://app.assembla.com/spaces/bryntum/tickets/8041
          // Although header and footer elements must be sized using flex-basis to avoid the busting out problem,
          // grid cells MUST be sized using width since rows are absolutely positioned and will not cause the
          // busting out problem, and rows will not stretch to shrinkwrap the cells unless they are widthed with
          // width.
          cell.style.width = domWidth;
        }
        if ('minWidth' in changes) {
          cell.style.minWidth = domMinWidth;
        }
        if ('maxWidth' in changes) {
          cell.style.maxWidth = domMaxWidth;
        }
        if ('flex' in changes) {
          cell.style.flex = column.flex ?? null;
        }
      }
    }
    // If we're being driven by the ColumnResizer or other bulk column resizer (like
    // ColumnAutoWidth), they will finish up with a call to afterColumnsResized.
    if (!me.resizingColumns) {
      me.afterColumnsResized(column);
    }
    // Columns which are flexed, but as part of a grouped column cannot just have their flex
    // value reflected in the flex value of its cells. They are flexing a different available space.
    // These have to be set to the exact width and kept synced.
    me.syncFlexedSubCols();
  }
  afterColumnsResized(column) {
    const me = this;
    me.eachSubGrid(subGrid => {
      // Only needed if the changed column is owned by the SubGrid
      if (!subGrid.collapsed && (!column || column.region === subGrid.region)) {
        subGrid.fixWidths();
        subGrid.fixRowWidthsInSafariEdge();
      }
    });
    me.lastColumnResized = me.cellEls = null;
    // Buffer some expensive operations, like updating the fake scrollers
    me.bufferedAfterColumnsResized(column);
    // Must happen immediately, not inside the bufferedAfterColumnsResized
    me.onHeightChange();
  }
  syncFlexedSubCols() {
    const flexedSubCols = this.columns.query(c => c.flex && c.childLevel && c.element);
    // Columns which are flexed, but as part of a grouped column cannot just have their flex
    // value reflected in the flex value of its cells. They are flexing a different available space.
    // These have to be set to the exact width and kept synced.
    if (flexedSubCols) {
      for (const column of flexedSubCols) {
        const width = column.element.getBoundingClientRect().width,
          cellEls = DomHelper.children(this.element, `.b-grid-cell[data-column-id="${column.id}"]`);
        for (const cell of cellEls) {
          cell.style.flex = `0 0 ${width}px`;
        }
      }
    }
  }
  bufferedAfterColumnsResized(column) {
    // Columns that allow their cell content to drive the row height requires a rerender after resize
    if (this.columns.usesAutoHeight) {
      this.refreshRows();
    }
    this.refreshVirtualScrollbars();
    this.eachSubGrid(subGrid => {
      // Only needed if the changed column is owned by the SubGrid
      if (!subGrid.collapsed && (!column || column.region === subGrid.region)) {
        subGrid.refreshFakeScroll();
      }
    });
  }
  bufferedElementResize() {
    this.refreshRows();
  }
  onInternalResize(element, newWidth, newHeight, oldWidth, oldHeight) {
    // If a flexed subGrid would be flexed *down* by a width reduction, allow it
    // to lay itself out before the refreshVirtualScrollbars called by GridElementEvents
    // asks them whether they are overflowingHorizontally.
    // This is to avoid an unnecessary extra layout with a horizontal
    // scrollbar which may be hidden when the subgrid adjusts itself when its ResizeMonitor
    // notification arrives - they are delivered outermost->innermost, we find out first here.
    // When the actualResizeMonitor notification arrives, it will be a no-op.
    if (DomHelper.scrollBarWidth && newWidth < oldWidth) {
      this.eachSubGrid(subGrid => {
        if (subGrid.flex) {
          subGrid.onElementResize(subGrid.element);
        }
      });
    }
    super.onInternalResize(...arguments);
    // Columns that allow their cell content to drive the row height requires a rerender after element resize
    if (this.isPainted && newWidth !== oldWidth && this.columns.usesFlexAutoHeight) {
      this.bufferedElementResize();
    }
  }
  //endregion
  //region Rows
  /**
   * Get the topmost visible grid row
   * @member {Grid.row.Row} firstVisibleRow
   * @readonly
   * @category Rows
   */
  /**
   * Get the last visible grid row
   * @member {Grid.row.Row} lastVisibleRow
   * @readonly
   * @category Rows
   */
  /**
   * Get the Row that is currently displayed at top.
   * @member {Grid.row.Row} topRow
   * @readonly
   * @category Rows
   * @private
   */
  /**
   * Get the Row currently displayed furthest down.
   * @member {Grid.row.Row} bottomRow
   * @readonly
   * @category Rows
   * @private
   */
  /**
   * Get Row for specified record id.
   * @function getRowById
   * @param {Core.data.Model|String|Number} recordOrId Record id (or a record)
   * @returns {Grid.row.Row} Found Row or null if record not rendered
   * @category Rows
   * @private
   */
  /**
   * Returns top and bottom for rendered row or estimated coordinates for unrendered.
   * @function getRecordCoords
   * @param {Core.data.Model|String|Number} recordOrId Record or record id
   * @returns {Object} Record bounds with format { top, height, bottom }
   * @category Calculations
   * @private
   */
  /**
   * Get the Row at specified index. "Wraps" index if larger than available rows.
   * @function getRow
   * @param {Number} index
   * @returns {Grid.row.Row}
   * @category Rows
   * @private
   */
  /**
   * Get a Row for either a record, a record id or an HTMLElement
   * @function getRowFor
   * @param {HTMLElement|Core.data.Model|String|Number} recordOrId Record or record id or HTMLElement
   * @returns {Grid.row.Row} Found Row or `null` if record not rendered
   * @category Rows
   */
  /**
   * Get a Row from an HTMLElement
   * @function getRowFromElement
   * @param {HTMLElement} element
   * @returns {Grid.row.Row} Found Row or `null` if record not rendered
   * @category Rows
   * @private
   */
  changeRowManager(rowManager, oldRowManager) {
    const me = this;
    // Use row height from CSS if not specified in config. Did not want to turn this into a getter/setter for
    // rowHeight since RowManager will plug its implementation into Grid when created below, and after initial
    // configuration that is what should be used
    if (!me._isRowMeasured) {
      me.measureRowHeight();
    }
    oldRowManager === null || oldRowManager === void 0 ? void 0 : oldRowManager.destroy();
    if (rowManager) {
      // RowManager is a plugin, it is configured with its grid as its "client".
      // It uses client.store as its record source.
      const result = RowManager.new({
        grid: me,
        rowHeight: me.rowHeight,
        rowScrollMode: me.rowScrollMode || 'move',
        autoHeight: me.autoHeight,
        fixedRowHeight: me.fixedRowHeight,
        internalListeners: {
          changeTotalHeight: 'onRowManagerChangeTotalHeight',
          requestScrollChange: 'onRowManagerRequestScrollChange',
          thisObj: me
        }
      }, rowManager);
      // The grid announces row rendering to allow customization of rows.
      me.relayEvents(result, ['beforeRenderRow', 'renderRow']);
      // RowManager injects itself as a property into the grid so that the grid
      // can reference it during RowManager's spin-up. We need to undo that now
      // otherwise updaters will not run.
      me._rowManager = null;
      return result;
    }
  }
  // Default implementation, documented in `defaultConfig`
  getRowHeight(record) {
    return record.rowHeight;
  }
  // Hook for features that need to alter the row height
  processRowHeight(record, height) {}
  //endregion
  //region Store
  /**
   * Hooks up data store listeners
   * @private
   * @category Store
   */
  bindStore(store) {
    const suffix = this.asyncEventSuffix;
    store.ion({
      name: storeListenerName,
      [`refresh${suffix}`]: 'onStoreDataChange',
      [`add${suffix}`]: 'onStoreAdd',
      [`remove${suffix}`]: 'onStoreRemove',
      [`replace${suffix}`]: 'onStoreReplace',
      [`removeAll${suffix}`]: 'onStoreRemoveAll',
      [`move${suffix}`]: store.tree ? null : 'onFlatStoreMove',
      change: 'relayStoreDataChange',
      idChange: 'onStoreRecordIdChange',
      update: 'onStoreUpdateRecord',
      beforeRequest: 'onStoreBeforeRequest',
      afterRequest: 'onStoreAfterRequest',
      exception: 'onStoreException',
      commit: 'onStoreCommit',
      loadSync: 'onStoreLoadSync',
      thisObj: this
    });
    super.bindStore(store);
  }
  unbindStore(oldStore) {
    this.detachListeners(storeListenerName);
    if (this.destroyStore) {
      oldStore.destroy();
    }
  }
  changeStore(store) {
    if (store == null) {
      return null;
    }
    if (typeof store === 'string') {
      store = Store.getStore(store);
    }
    if (!store.isStore) {
      var _this$initialConfig$f;
      store = ObjectHelper.assign({
        data: this.data,
        tree: Boolean((_this$initialConfig$f = this.initialConfig.features) === null || _this$initialConfig$f === void 0 ? void 0 : _this$initialConfig$f.tree)
      }, store);
      if (!store.data) {
        delete store.data;
      }
      if (!store.modelClass) {
        store.modelClass = GridRowModel;
      }
      store = new (store.readUrl ? AjaxStore : Store)(store);
    }
    return store;
  }
  updateStore(store, was) {
    var _super$updateStore;
    const me = this;
    (_super$updateStore = super.updateStore) === null || _super$updateStore === void 0 ? void 0 : _super$updateStore.call(this, store, was);
    if (was) {
      me.unbindStore(was);
    }
    if (store) {
      // Deselect all rows when replacing the store, otherwise selection retains old store
      if (was) {
        me.deselectAll();
      }
      me.bindStore(store);
    }
    me.trigger('bindStore', {
      store,
      oldStore: was
    });
    // Changing store when painted -> refresh rows to reflect new data
    if (!me.isDestroying && me.isPainted && !me.refreshSuspended) {
      var _me$_rowManager;
      (_me$_rowManager = me._rowManager) === null || _me$_rowManager === void 0 ? void 0 : _me$_rowManager.reinitialize();
    }
  }
  onStoreLoadSync() {
    // Redrawing is blocked during syncDataOnLoad, rerender what's in view here, to maintain scroll position
    this.rowManager.renderFromRow(this.topRow);
  }
  /**
   * Rerenders a cell if a record is updated in the store
   * @private
   * @category Store
   */
  onStoreUpdateRecord({
    source: store,
    record,
    changes
  }) {
    const me = this;
    if (me.refreshSuspended || store.isSyncingDataOnLoad) {
      return;
    }
    if (me.forceFullRefresh) {
      // flagged to need full refresh (probably from using GroupSummary)
      me.rowManager.refresh();
      me.forceFullRefresh = false;
    } else {
      let row;
      // Search for old row if id was changed
      if (record.isFieldModified('id')) {
        row = me.getRowFor(record.meta.modified.id);
      }
      row = row || me.getRowFor(record);
      // not rendered, bail out
      if (!row) {
        return;
      }
      // We must refresh the full row if it's a special row which has signalled
      // an update because it has no cells.
      if (me.fullRowRefresh || record.isSpecialRow) {
        const index = store.indexOf(record);
        if (index !== -1) {
          row.render(index, record);
        }
      } else {
        me.columns.visibleColumns.forEach(column => {
          const field = column.field,
            isSafe = column.constructor.simpleRenderer && !Object.prototype.hasOwnProperty.call(column.data, 'renderer');
          // If there's a  non-safe renderer, that is a renderer which draws values from elsewhere
          // than just its configured field, that column must be refreshed on every record update.
          // Obviously, if the column's configured field is changed that also means it's refreshed.
          if (!isSafe || changes[field]) {
            const cellElement = row.getCell(field);
            if (cellElement) {
              row.renderCell(cellElement);
            }
          }
        });
      }
    }
  }
  refreshFromRowOnStoreAdd(row, context) {
    const me = this,
      {
        rowManager
      } = me;
    rowManager.renderFromRow(row);
    rowManager.trigger('changeTotalHeight', {
      totalHeight: rowManager.totalHeight
    });
    // First record? Also update fake scrollers
    if (me.store.count === 1) {
      me.callEachSubGrid('refreshFakeScroll');
    }
  }
  onMaskAutoClose(mask) {
    super.onMaskAutoClose(mask);
    this.toggleEmptyText();
  }
  /**
   * Refreshes rows when data is added to the store
   * @private
   * @category Store
   */
  onStoreAdd({
    source: store,
    records,
    index,
    oldIndex,
    isChild,
    oldParent,
    parent,
    isMove,
    isExpandAll
  }) {
    var _store$project;
    const me = this,
      {
        rowManager
      } = me;
    // Do not react if the content has not been rendered, or refresh is suspended, or we are syncing data on load,
    // or we are part of a project that is having a changeset applied (a bit dirty to include that here, but
    // avoids having to add an override in TimelineBase)
    if (!me.isPainted || isExpandAll || me.refreshSuspended || store.isSyncingDataOnLoad || (_store$project = store.project) !== null && _store$project !== void 0 && _store$project.applyingChangeset) {
      return;
    }
    // If we move records check if some of their old parents is expanded
    const hasExpandedOldParent = isMove && records.some(record => {
      if (isMove[record.id]) {
        // When using TreeGroup there won't be an old parent
        const oldParent = store.getById(record.meta.modified.parentId);
        return (oldParent === null || oldParent === void 0 ? void 0 : oldParent.isExpanded(store)) && (oldParent === null || oldParent === void 0 ? void 0 : oldParent.ancestorsExpanded(store));
      }
    });
    // If it's the addition of a child to a collapsed zone (and old parents are also collapsed), the UI does not change.
    if (isChild && !records[0].ancestorsExpanded(store) && !hasExpandedOldParent) {
      // BUT it might change if parent had no children (expander made invisible) and it gets children added
      if (!parent.isLeaf) {
        const parentRow = rowManager.getRowById(parent);
        if (parentRow) {
          rowManager.renderRows([parentRow]);
        }
      }
      return;
    }
    rowManager.calculateRowCount(false, true, true);
    // When store is filtered need to update the index value
    if (store.isFiltered) {
      index = store.indexOf(records[0]);
    }
    const {
        topIndex,
        rows,
        rowCount
      } = rowManager,
      bottomIndex = rowManager.topIndex + rowManager.rowCount - 1,
      dataStart = index,
      dataEnd = index + records.length - 1,
      atEnd = bottomIndex >= store.count - records.length - 1;
    // When moving a node within a tree we might need the redraw to include its old parent and its children. Not
    // worth the complexity of trying to do a partial render for this, rerender all rows to be safe. Cannot solely
    // rely on presence of isMove, all keys might be false
    // Moving records within a flat store is handled elsewhere, in onFlatStoreMove
    if (oldParent || oldIndex > -1 || isChild && isMove && Object.values(isMove).some(v => v)) {
      rowManager.refresh();
    }
    // Added block starts in our visible block. Render from there downwards.
    else if (dataStart >= topIndex && dataStart < topIndex + rowCount) {
      me.refreshFromRowOnStoreAdd(rows[dataStart - topIndex], ...arguments);
    }
    // Added block ends in our visible block, render block
    else if (dataEnd >= topIndex && dataEnd < topIndex + rowCount) {
      rowManager.refresh();
    }
    // If added block is outside of the visible area, no visible change
    // but potentially a change in total dataset height.
    else {
      // If we are against the end of the dataset, and have appended records
      // ensure they are rendered below
      if (atEnd && index > bottomIndex) {
        rowManager.fillBelow(me._scrollTop || 0);
      }
      rowManager.estimateTotalHeight(true);
    }
  }
  /**
   * Responds to exceptions signalled by the store
   * @private
   * @category Store
   */
  onStoreException({
    action,
    type,
    response,
    exceptionType,
    error
  }) {
    var _response$parsedJson;
    const me = this;
    let message;
    switch (type) {
      case 'server':
        message = response.message || me.L('L{unspecifiedFailure}');
        break;
      case 'exception':
        message = exceptionType === 'network' ? me.L('L{networkFailure}') : (error === null || error === void 0 ? void 0 : error.message) || (response === null || response === void 0 ? void 0 : (_response$parsedJson = response.parsedJson) === null || _response$parsedJson === void 0 ? void 0 : _response$parsedJson.message) || me.L('L{parseFailure}');
        break;
    }
    me.applyMaskError(`<div class="b-grid-load-failure">
                <div class="b-grid-load-fail">${me.L(action === 'read' ? 'L{loadFailedMessage}' : 'L{syncFailedMessage}')}</div>
                ${response !== null && response !== void 0 && response.url ? `<div class="b-grid-load-fail">${response.url}</div>` : ''}
                <div class="b-grid-load-fail">${me.L('L{serverResponse}')}</div>
                <div class="b-grid-load-fail">${message}</div>
            </div>`);
  }
  /**
   * Refreshes rows when data is changed in the store
   * @private
   * @category Store
   */
  onStoreDataChange({
    action,
    changes,
    source: store,
    syncInfo
  }) {
    var _super$onStoreDataCha;
    // If the next mixin up the inheritance chain has an implementation, call it
    (_super$onStoreDataCha = super.onStoreDataChange) === null || _super$onStoreDataCha === void 0 ? void 0 : _super$onStoreDataCha.call(this, ...arguments);
    const me = this;
    if (me.refreshSuspended || !me.rowManager || store.isSyncingDataOnLoad) {
      return;
    }
    // If it's new data, the old calculation is invalidated.
    if (action === 'dataset') {
      me.rowManager.clearKnownHeights();
      // Initial filters on a tree store first triggers `dataset`, then `filter`.
      // Ignore `dataset`, only redraw on `filter`
      if (store.isTree && store.isFiltered) {
        return;
      }
    }
    const isGroupFieldChange = store.isGrouped && changes && store.groupers.some(grp => grp.field in changes);
    // No need to rerender if it's a change of the value of the group field which
    // will be responded to by StoreGroup
    if (me.isPainted && !isGroupFieldChange) {
      // Optionally scroll to top if setting new data or filtering based on preserveScrollOnDatasetChange setting
      me.renderRows(Boolean(!(action in datasetReplaceActions) || me.preserveScrollOnDatasetChange));
    }
    me.toggleEmptyText();
  }
  /**
   * The hook is called when the id of a record has changed.
   * @private
   * @category Store
   */
  onStoreRecordIdChange() {
    var _super$onStoreRecordI;
    // If the next mixin up the inheritance chain has an implementation, call it
    (_super$onStoreRecordI = super.onStoreRecordIdChange) === null || _super$onStoreRecordI === void 0 ? void 0 : _super$onStoreRecordI.call(this, ...arguments);
  }
  /**
   * Shows a load mask while the connected store is loading
   * @private
   * @category Store
   */
  onStoreBeforeRequest() {
    this.applyLoadMask();
  }
  /**
   * Hides load mask after a load request ends either in success or failure
   * @private
   * @category Store
   */
  onStoreAfterRequest(event) {
    if (this.loadMask && !event.exception) {
      this.masked = null;
      this.toggleEmptyText();
    }
  }
  needsFullRefreshOnStoreRemove({
    isCollapse
  }) {
    const features = this._features;
    return (features === null || features === void 0 ? void 0 : features.group) && !features.group.disabled || (features === null || features === void 0 ? void 0 : features.groupSummary) && !features.groupSummary.disabled ||
    // Need to redraw parents when children are removed since they might be converted to leaves
    this.store.tree && !isCollapse && this.store.modelClass.convertEmptyParentToLeaf;
  }
  /**
   * Animates removal of record.
   * @private
   * @category Store
   */
  onStoreRemove({
    source: store,
    records,
    isCollapse,
    isChild,
    isMove,
    isCollapseAll
  }) {
    var _super$onStoreRemove;
    // Do not react if the content has not been rendered,
    // or if it is a move, which will be handled by onStoreAdd
    if (!this.isPainted || isMove || isCollapseAll) {
      return;
    }
    // GridSelection mixin does its job on records removing
    (_super$onStoreRemove = super.onStoreRemove) === null || _super$onStoreRemove === void 0 ? void 0 : _super$onStoreRemove.call(this, ...arguments);
    const me = this,
      {
        rowManager
      } = me;
    // Remove cached heights
    rowManager.invalidateKnownHeight(records);
    // Will be refresh once in onStoreLoadSync after sync data on load
    if (store.isSyncingDataOnLoad) {
      return;
    }
    if (me.animateRemovingRows && !isCollapse && !isChild) {
      // Gather all visible rows which need to be removed.
      const rowsToRemove = records.reduce((result, record) => {
        const row = rowManager.getRowById(record.id);
        row && result.push(row);
        return result;
      }, []);
      if (rowsToRemove.length) {
        const topRow = rowsToRemove[0];
        me.isAnimating = true;
        // As soon as first row has disappeared, rerender the view
        EventHelper.onTransitionEnd({
          element: topRow._elementsArray[0],
          property: 'left',
          // Detach listener after timeout even if event wasn't fired
          duration: me.transitionDuration,
          thisObj: me,
          handler: () => {
            me.isAnimating = false;
            rowsToRemove.forEach(row => !row.isDestroyed && row.removeCls('b-removing'));
            rowManager.refresh();
            // undocumented internal event for scheduler
            me.trigger('rowRemove');
            me.afterRemove(arguments[0]);
          }
        });
        rowsToRemove.forEach(row => row.addCls('b-removing'));
        return;
      }
    }
    // Cannot do an update from the affected row and down here. Since group headers might be affected by
    // removing rows we need a full refresh
    if (me.needsFullRefreshOnStoreRemove(...arguments)) {
      rowManager.refresh();
      me.afterRemove(arguments[0]);
    } else {
      const oldTopIndex = rowManager.topIndex;
      // Potentially remove rows and change dataset height
      rowManager.calculateRowCount(false, true, true);
      // If collapsing lead to rows "shifting up" to fit in available rows, we have to rerender from top
      if (rowManager.topIndex !== oldTopIndex) {
        rowManager.renderFromRow(rowManager.topRow);
      } else {
        const {
            rows
          } = rowManager,
          topRowIndex = records.reduce((result, record) => {
            const row = rowManager.getRowById(record.id);
            if (row) {
              // Rows are repositioned in the array, it matches visual order. Need to find actual index in it
              result = Math.min(result, rows.indexOf(row));
            }
            return result;
          }, rows.length);
        // If there were rows below which have moved up into place
        // then repurpose them with their new records
        if (rows[topRowIndex]) {
          !me.refreshSuspended && rowManager.renderFromRow(rows[topRowIndex]);
        }
        // If nothing to render below, just update dataset height
        else {
          rowManager.trigger('changeTotalHeight', {
            totalHeight: rowManager.totalHeight
          });
        }
      }
      me.trigger('rowRemove', {
        isCollapse
      });
      me.afterRemove(arguments[0]);
    }
  }
  onFlatStoreMove({
    from,
    to
  }) {
    const {
        rowManager,
        store
      } = this,
      {
        topIndex,
        rowCount
      } = rowManager,
      // from and to are indices in the unfiltered collection, we need to convert them
      // to indices in a visible collection
      [dataStart, dataEnd] = [from, to].sort((a, b) => a - b),
      visibleStart = store.indexOf(store.getAt(dataStart, true)),
      visibleEnd = store.indexOf(store.getAt(dataEnd, true));
    // Changed block starts in our visible block. Render from there downwards.
    if (visibleStart >= topIndex && visibleStart < topIndex + rowCount) {
      rowManager.renderFromRow(rowManager.rows[visibleStart - topIndex]);
    }
    // Changed block ends in our visible block, render block
    else if (visibleEnd >= topIndex && visibleEnd < topIndex + rowCount) {
      rowManager.refresh();
    }
    // If the changed block is outside the visible area, this is a no-op
  }

  onStoreReplace({
    records,
    all
  }) {
    const {
      rowManager
    } = this;
    if (all) {
      rowManager.clearKnownHeights();
      rowManager.refresh();
    } else {
      const rows = records.reduce((rows, [, record]) => {
        const row = this.getRowFor(record);
        if (row) {
          rows.push(row);
        }
        return rows;
      }, []);
      // Heights will be stored on render, but some records might be out of view -> have to invalidate separately
      rowManager.invalidateKnownHeight(records);
      rowManager.renderRows(rows);
    }
  }
  relayStoreDataChange(event) {
    this.ariaElement.setAttribute('aria-rowcount', this.store.count + 1);
    /**
     * Fired when data in the store changes.
     *
     * Basically a relayed version of the store's own change event, decorated with a `store` property.
     * See the {@link Core.data.Store#event-change store change event} documentation for more information.
     *
     * @event dataChange
     * @param {Grid.view.Grid} source Owning grid
     * @param {Core.data.Store} store The originating store
     * @param {'remove'|'removeAll'|'add'|'updatemultiple'|'clearchanges'|'filter'|'update'|'dataset'|'replace'} action
     * Name of action which triggered the change. May be one of:
     * * `'remove'`
     * * `'removeAll'`
     * * `'add'`
     * * `'updatemultiple'`
     * * `'clearchanges'`
     * * `'filter'`
     * * `'update'`
     * * `'dataset'`
     * * `'replace'`
     * @param {Core.data.Model} record Changed record, for actions that affects exactly one record (`'update'`)
     * @param {Core.data.Model[]} records Changed records, passed for all actions except `'removeAll'`
     * @param {Object} changes Passed for the `'update'` action, info on which record fields changed
     */
    if (!this.project) {
      return this.trigger('dataChange', {
        ...event,
        store: event.source,
        source: this
      });
    }
  }
  /**
   * Rerenders grid when all records have been removed
   * @private
   * @category Store
   */
  onStoreRemoveAll() {
    var _super$onStoreRemoveA;
    // GridSelection mixin does its job on records removing
    (_super$onStoreRemoveA = super.onStoreRemoveAll) === null || _super$onStoreRemoveA === void 0 ? void 0 : _super$onStoreRemoveA.call(this, ...arguments);
    if (this.isPainted) {
      this.rowManager.clearKnownHeights();
      this.renderRows(false);
      this.toggleEmptyText();
    }
  }
  // Refresh dirty cells on commit
  onStoreCommit({
    changes
  }) {
    if (this.showDirty && changes.modified.length) {
      const rows = [];
      changes.modified.forEach(record => {
        const row = this.rowManager.getRowFor(record);
        row && rows.push(row);
      });
      this.rowManager.renderRows(rows);
    }
  }
  // Documented with config
  get data() {
    if (this._store) {
      return this._store.records;
    } else {
      return this._data;
    }
  }
  set data(data) {
    if (this._store) {
      this._store.data = data;
    } else {
      this._data = data;
    }
  }
  //endregion
  //region Context menu items
  /**
   * Populates the header context menu. Chained in features to add menu items.
   * @param {Object} options Contains menu items and extra data retrieved from the menu target.
   * @param {Grid.column.Column} options.column Column for which the menu will be shown
   * @param {Object<String,MenuItemConfig|Boolean|null>} options.items A named object to describe menu items
   * @internal
   */
  populateHeaderMenu({
    column,
    items
  }) {
    const me = this,
      {
        subGrids,
        regions
      } = me,
      {
        parent
      } = column;
    let first = true;
    Object.entries(subGrids).forEach(([region, subGrid]) => {
      // If SubGrid is configured with a sealed column set, do not allow moving into it
      if (subGrid.sealedColumns) {
        return;
      }
      if (column.draggable && region !== column.region && (!parent && subGrids[column.region].columns.count > 1 || parent && parent.children.length > 1)) {
        const preceding = subGrid.element.compareDocumentPosition(subGrids[column.region].element) === document.DOCUMENT_POSITION_PRECEDING,
          moveRight = me.rtl ? !preceding : preceding,
          // With 2 regions, use Move left, Move right. With multiple, include region name
          text = regions.length > 2 ? me.L('L{moveColumnTo}', me.optionalL(region)) : me.L(moveRight ? 'L{moveColumnRight}' : 'L{moveColumnLeft}');
        items[`${region}Region`] = {
          targetSubGrid: region,
          text,
          icon: 'b-fw-icon b-icon-column-move-' + (moveRight ? 'right' : 'left'),
          separator: first,
          disabled: !column.allowDrag,
          onItem: ({
            item
          }) => {
            column.traverse(col => col.region = region);
            // Changing region will move the column to the correct SubGrid, but we want it to go last
            me.columns.insert(me.columns.indexOf(subGrids[item.targetSubGrid].columns.last) + 1, column);
            me.scrollColumnIntoView(column);
          }
        };
        first = false;
      }
    });
  }
  /**
   * Populates the cell context menu. Chained in features to add menu items.
   * @param {Object} options Contains menu items and extra data retrieved from the menu target.
   * @param {Grid.column.Column} options.column Column for which the menu will be shown
   * @param {Core.data.Model} options.record Record for which the menu will be shown
   * @param {Object<String,MenuItemConfig|Boolean|null>} options.items A named object to describe menu items
   * @internal
   */
  populateCellMenu({
    record,
    items
  }) {}
  getColumnDragToolbarItems(column, items) {
    return items;
  }
  //endregion
  //region Getters
  normalizeCellContext(cellContext) {
    const grid = this,
      {
        columns
      } = grid;
    // Already have a Location
    if (cellContext.isLocation) {
      return cellContext;
    }
    // Create immutable Location object encapsulating the passed object.
    if (cellContext.isModel) {
      return new Location({
        grid,
        id: cellContext.id,
        columnId: columns.visibleColumns[0].id
      });
    }
    return new Location(ObjectHelper.assign({
      grid
    }, cellContext));
  }
  /**
   * Returns a cell if rendered or null if not found.
   * @param {LocationConfig} cellContext A cell location descriptor
   * @returns {HTMLElement|null}
   * @category Getters
   */
  getCell(cellContext) {
    const {
        store,
        columns
      } = this,
      {
        visibleColumns
      } = this.columns,
      rowIndex = !isNaN(cellContext.row) ? cellContext.row : !isNaN(cellContext.rowIndex) ? cellContext.rowIndex : store.indexOf(cellContext.record || cellContext.id),
      columnIndex = !isNaN(cellContext.column) ? cellContext.column : !isNaN(cellContext.columnIndex) ? cellContext.columnIndex : visibleColumns.indexOf(cellContext.column || columns.getById(cellContext.columnId) || columns.get(cellContext.field) || visibleColumns[0]);
    // Only return cell for valid address.
    // This code is more strict than Location which attempts to find the closest existing cell.
    // Here we MUST only return a cell if the passed context is fully valid.
    return rowIndex > -1 && rowIndex < store.count && columnIndex > -1 && columnIndex < visibleColumns.length && this.normalizeCellContext(cellContext).cell || null;
  }
  /**
   * Returns the header element for the column
   * @param {String|Number|Grid.column.Column} columnId or Column instance
   * @returns {HTMLElement} Header element
   * @category Getters
   */
  getHeaderElement(columnId) {
    if (columnId.isModel) {
      columnId = columnId.id;
    }
    return this.fromCache(`.b-grid-header[data-column-id="${columnId}"]`);
  }
  getHeaderElementByField(field) {
    const column = this.columns.get(field);
    return column ? this.getHeaderElement(column) : null;
  }
  /**
   * Body height
   * @member {Number}
   * @readonly
   * @category Layout
   */
  get bodyHeight() {
    return this._bodyHeight;
  }
  /**
   * Header height
   * @member {Number}
   * @readonly
   * @category Layout
   */
  get headerHeight() {
    const me = this;
    // measure header if rendered and not stored
    if (me.isPainted && !me._headerHeight) {
      me._headerHeight = me.headerContainer.offsetHeight;
    }
    return me._headerHeight;
  }
  /**
   * Footer height
   * @member {Number}
   * @readonly
   * @category Layout
   */
  get footerHeight() {
    const me = this;
    // measure footer if rendered and not stored
    if (me.isPainted && !me._footerHeight) {
      me._footerHeight = me.footerContainer.offsetHeight;
    }
    return me._footerHeight;
  }
  get isTreeGrouped() {
    var _this$features$treeGr;
    return Boolean((_this$features$treeGr = this.features.treeGroup) === null || _this$features$treeGr === void 0 ? void 0 : _this$features$treeGr.isGrouped);
  }
  /**
   * Searches up from the specified element for a grid row and returns the record associated with that row.
   * @param {HTMLElement} element Element somewhere within a row or the row container element
   * @returns {Core.data.Model} Record for the row
   * @category Getters
   */
  getRecordFromElement(element) {
    const el = element.closest('.b-grid-row');
    if (!el) return null;
    return this.store.getAt(el.dataset.index);
  }
  /**
   * Searches up from specified element for a grid cell or an header and returns the column which the cell belongs to
   * @param {HTMLElement} element Element somewhere in a cell
   * @returns {Grid.column.Column} Column to which the cell belongs
   * @category Getters
   */
  getColumnFromElement(element) {
    const cell = element.closest('.b-grid-cell, .b-grid-header');
    if (!cell) return null;
    if (cell.matches('.b-grid-header')) {
      return this.columns.getById(cell.dataset.columnId);
    }
    const cellData = DomDataStore.get(cell);
    return this.columns.getById(cellData.columnId);
  }
  // Only added for type checking, since it seems common to get it wrong in react/angular
  updateAutoHeight(autoHeight) {
    ObjectHelper.assertBoolean(autoHeight, 'autoHeight');
  }
  // Documented under configs
  get columnLines() {
    return this._columnLines;
  }
  set columnLines(columnLines) {
    ObjectHelper.assertBoolean(columnLines, 'columnLines');
    DomHelper.toggleClasses(this.element, 'b-no-column-lines', !columnLines);
    this._columnLines = columnLines;
  }
  get keyMapElement() {
    return this.bodyElement;
  }
  //endregion
  //region Fix width & height
  /**
   * Sets widths and heights for headers, rows and other parts of the grid as needed
   * @private
   * @category Width & height
   */
  fixSizes() {
    // subGrid width
    this.callEachSubGrid('fixWidths');
    // Get leaf headers.
    const colHeaders = this.headerContainer.querySelectorAll('.b-grid-header.b-depth-0');
    // Update leaf headers' ariaColIndex
    for (let i = 0, {
        length
      } = colHeaders; i < length; i++) {
      colHeaders[i].setAttribute('aria-colindex', i + 1);
    }
  }
  onRowManagerChangeTotalHeight({
    totalHeight,
    immediate
  }) {
    return this.refreshTotalHeight(totalHeight, immediate);
  }
  /**
   * Makes height of vertical scroller match estimated total height of grid. Called when scrolling vertically and
   * when showing/hiding rows.
   * @param {Number} [height] Total height supplied by RowManager
   * @param {Boolean} [immediate] Flag indicating if buffered element sizing should be bypassed
   * @private
   * @category Width & height
   */
  refreshTotalHeight(height = this.rowManager.totalHeight, immediate = false) {
    const me = this;
    // Veto change of estimated total height while rendering rows or if triggered while in a hidden state
    if (me.renderingRows || !me.isVisible) {
      return false;
    }
    const scroller = me.scrollable,
      delta = Math.abs(me.virtualScrollHeight - height),
      clientHeight = me._bodyRectangle.height,
      newMaxY = height - clientHeight;
    if (delta) {
      const
      // We must update immediately if we are nearing the end of the scroll range.
      isCritical = newMaxY - me._scrollTop < clientHeight * 2 ||
      // Or if we have scrolled pass visual height
      me._verticalScrollHeight && me._verticalScrollHeight - clientHeight < me._scrollTop;
      // Update the true scroll range using the scroller. This will not cause a repaint.
      scroller.scrollHeight = me.virtualScrollHeight = height;
      // If we are scrolling, put this off because it causes
      // a full document layout and paint.
      // Do not buffer calls for not yet painted grid
      if (me.isPainted && (me.scrolling && !isCritical || delta < 100) && !immediate) {
        me.bufferedFixElementHeights();
      } else {
        me.virtualScrollHeightDirty && me.virtualScrollHeightDirty();
        me.bufferedFixElementHeights.cancel();
        me.fixElementHeights();
      }
    }
  }
  fixElementHeights() {
    const me = this,
      height = me.virtualScrollHeight,
      heightInPx = `${height}px`;
    me._verticalScrollHeight = height;
    me.verticalScroller.style.height = heightInPx;
    me.virtualScrollHeightDirty = false;
    if (me.autoHeight) {
      me.bodyContainer.style.height = heightInPx;
      me._bodyHeight = height;
      me.refreshBodyRectangle();
    }
    me.refreshVirtualScrollbars();
  }
  refreshBodyRectangle() {
    return this._bodyRectangle = Rectangle.client(this.bodyContainer);
  }
  //endregion
  //region Scroll & virtual rendering
  set scrolling(scrolling) {
    this._scrolling = scrolling;
  }
  get scrolling() {
    return this._scrolling;
  }
  /**
   * Activates automatic scrolling of a subGrid when mouse is moved closed to the edges. Useful when dragging DOM
   * nodes from outside this grid and dropping on the grid.
   * @param {Grid.view.SubGrid|String|Grid.view.SubGrid[]|String[]} subGrid A subGrid instance or its region name or
   * an array of either
   * @category Scrolling
   */
  enableScrollingCloseToEdges(subGrids) {
    this.scrollManager.startMonitoring({
      scrollables: [{
        element: this.scrollable.element,
        direction: 'vertical'
      }, ...ArrayHelper.asArray(subGrids || []).map(subGrid => ({
        element: (typeof subGrid === 'string' ? this.subGrids[subGrid] : subGrid).scrollable.element
      }))],
      direction: 'horizontal'
    });
  }
  /**
   * Deactivates automatic scrolling of a subGrid when mouse is moved closed to the edges
   * @param {Grid.view.SubGrid|String|Grid.view.SubGrid[]|String[]} subGrid A subGrid instance or its region name or
   * an array of either
   * @category Scrolling
   */
  disableScrollingCloseToEdges(subGrids) {
    this.scrollManager.stopMonitoring([this.scrollable.element, ...ArrayHelper.asArray(subGrids || []).map(subGrid => (typeof subGrid === 'string' ? this.subGrids[subGrid] : subGrid).element)]);
  }
  /**
   * Responds to request from RowManager to adjust scroll position. Happens when jumping to a scroll position with
   * variable row height.
   * @param {Number} bottomMostRowY
   * @private
   * @category Scrolling
   */
  onRowManagerRequestScrollChange({
    bottom
  }) {
    this.scrollable.y = bottom - this.bodyHeight;
  }
  /**
   * Scroll syncing for normal headers & grid + triggers virtual rendering for vertical scroll
   * @private
   * @fires scroll
   * @category Scrolling
   */
  initScroll() {
    const me = this,
      {
        scrollable
      } = me;
    // This method may be called early, before render calls it, so ensure that it's
    // only executed once.
    if (!me.scrollInitialized) {
      me.scrollInitialized = true;
      // Allows FF to dynamically track scrollbar state change by reacting to content height changes.
      // Remove when https://bugzilla.mozilla.org/show_bug.cgi?id=1733042 is fixed
      scrollable.contentElement = me.contentElement;
      scrollable.ion({
        scroll: 'onGridVerticalScroll',
        scrollend: 'onGridVerticalScrollEnd',
        thisObj: me
      });
      me.callEachSubGrid('initScroll');
      // Fixes scroll freezing bug on iPad by putting scroller in its own layer
      if (BrowserHelper.isMobileSafari) {
        scrollable.element.style.transform = 'translate3d(0, 0, 0)';
      }
    }
  }
  onGridVerticalScroll({
    source: scrollable
  }) {
    const me = this,
      {
        y: scrollTop
      } = scrollable;
    // Was getting scroll events in FF where scrollTop was unchanged, ignore those
    if (scrollTop !== me._scrollTop) {
      me._scrollTop = scrollTop;
      if (!me.scrolling) {
        me.scrolling = true;
        // Vertical scroll may trigger resize if row height is variable
        me.eachSubGrid(s => s.suspendResizeMonitor = true);
      }
      me.rowManager.updateRenderedRows(scrollTop);
      // Hook for features that need to react to scroll
      me.afterScroll({
        scrollTop
      });
      /**
       * Grid has scrolled vertically
       * @event scroll
       * @param {Grid.view.Grid} source The firing Grid instance.
       * @param {Number} scrollTop The vertical scroll position.
       */
      me.trigger('scroll', {
        scrollTop
      });
    }
  }
  onGridVerticalScrollEnd() {
    this.scrolling = false;
    this.eachSubGrid(s => s.suspendResizeMonitor = false);
  }
  /**
   * Scrolls a row into view. If row isn't rendered it tries to calculate position. Accepts the {@link ScrollOptions}
   * `column` property
   * @param {Core.data.Model|String|Number} recordOrId Record or record id
   * @param {ScrollOptions} [options] How to scroll.
   * @returns {Promise} A promise which resolves when the specified row has been scrolled into view.
   * @category Scrolling
   */
  async scrollRowIntoView(recordOrId, options = defaultScrollOptions) {
    const me = this,
      blockPosition = options.block || 'nearest',
      {
        rowManager
      } = me,
      record = me.store.getById(recordOrId);
    if (record) {
      let scrollPromise;
      // check that record is "displayable", not filtered out or hidden by collapse
      if (me.store.indexOf(record) === -1) {
        return resolvedPromise;
      }
      let scroller = me.scrollable,
        recordRect = me.getRecordCoords(record);
      const scrollerRect = Rectangle.from(scroller.element);
      // If it was calculated from the index, update the rendered rowScrollMode
      // and scroll to the actual element. Note that this should only be necessary
      // for variableRowHeight.
      // But to "make the tests green", this is a workaround for a buffered rendering
      // bug when teleporting scroll. It does not render the rows at their correct
      // positions. Please do not try to "fix" this. I will do it. NGW
      if (recordRect.virtual) {
        const virtualBlock = recordRect.block,
          innerOptions = blockPosition !== 'nearest' ? options : {
            block: virtualBlock
          };
        // Scroll the calculated position **synchronously** to the center of the scrollingViewport
        // and then update the rendered block while asking the RowManager to
        // display the required recordOrId.
        scrollPromise = scroller.scrollIntoView(recordRect, {
          block: 'center'
        });
        rowManager.scrollTargetRecordId = record;
        rowManager.updateRenderedRows(scroller.y, true);
        recordRect = me.getRecordCoords(record);
        rowManager.lastScrollTop = scroller.y;
        if (recordRect.virtual) {
          // bail out to not get caught in infinite loop, since code above is cut out of bundle
          return resolvedPromise;
        }
        // Scroll the target just less than append/prepend buffer height out of view so that the animation looks good
        if (options.animate) {
          // Do not fire scroll events during this scroll sequence - it's a purely cosmetic operation.
          // We are scrolling the desired row out of view merely to *animate scroll* it to the requested position.
          scroller.suspendEvents();
          // Scroll to its final position
          if (blockPosition === 'end' || blockPosition === 'nearest' && virtualBlock === 'end') {
            scroller.y -= scrollerRect.bottom - recordRect.bottom;
          } else if (blockPosition === 'start' || blockPosition === 'nearest' && virtualBlock === 'start') {
            scroller.y += recordRect.y - scrollerRect.y;
          }
          // Ensure rendered block is correct at that position
          rowManager.updateRenderedRows(scroller.y, false, true);
          // Scroll away from final position to enable a cosmetic scroll to final position
          if (virtualBlock === 'end') {
            scroller.y -= rowManager.appendRowBuffer * rowManager.rowHeight - 1;
          } else {
            scroller.y += rowManager.prependRowBuffer * rowManager.rowHeight - 1;
          }
          // The row will still be rendered, so scroll it using the scroller directly
          scroller.scrollIntoView(me.getRecordCoords(record), Object.assign({}, options, innerOptions));
          // Now we're at the required position, resume events
          scroller.resumeEvents();
        } else {
          var _me$scrollRowIntoView;
          if (!options.recursive) {
            await scrollPromise;
          }
          // May already be destroyed at this point, hence ?.
          await ((_me$scrollRowIntoView = me.scrollRowIntoView) === null || _me$scrollRowIntoView === void 0 ? void 0 : _me$scrollRowIntoView.call(me, record, Object.assign({
            recursive: true
          }, options, innerOptions)));
        }
      } else {
        let {
          column
        } = options;
        if (column) {
          if (!column.isModel) {
            column = me.columns.getById(column) || me.columns.get(column);
          }
          // If we are targeting a column, we must use the scroller of that column's SubGrid
          if (column) {
            scroller = me.getSubGridFromColumn(column).scrollable;
            const cellRect = Rectangle.from(rowManager.getRowFor(record).getCell(column.id));
            recordRect.x = cellRect.x;
            recordRect.width = cellRect.width;
          }
        }
        // No column, then tell the scroller not to scroll in the X axis
        else {
          options.x = false;
        }
        return scroller.scrollIntoView(recordRect, options);
      }
    }
  }
  /**
   * Scrolls a column into view (if it is not already)
   * @param {Grid.column.Column|String|Number} column Column name (data) or column index or actual column object.
   * @param {ScrollOptions} [options] How to scroll.
   * @returns {Promise} If the column exists, a promise which is resolved when the column header element has been
   * scrolled into view.
   * @category Scrolling
   */
  scrollColumnIntoView(column, options) {
    column = column instanceof Column ? column : this.columns.get(column) || this.columns.getById(column) || this.columns.getAt(column);
    return this.getSubGridFromColumn(column).scrollColumnIntoView(column, options);
  }
  /**
   * Scrolls a cell into view (if it is not already)
   * @param {Object} cellContext Cell selector { id: recordId, column: 'columnName' }
   * @category Scrolling
   */
  scrollCellIntoView(cellContext, options) {
    return this.scrollRowIntoView(cellContext.id, Object.assign({
      column: cellContext.columnId
    }, typeof options === 'boolean' ? {
      animate: options
    } : options));
  }
  /**
   * Scroll all the way down
   * @returns {Promise} A promise which resolves when the bottom is reached.
   * @category Scrolling
   */
  scrollToBottom(options) {
    // triggers scroll to last record. not using current scroller height because we do not know if it is correct
    return this.scrollRowIntoView(this.store.last, options);
  }
  /**
   * Scroll all the way up
   * @returns {Promise} A promise which resolves when the top is reached.
   * @category Scrolling
   */
  scrollToTop(options) {
    return this.scrollable.scrollBy(0, -this.scrollable.y, options);
  }
  /**
   * Stores the scroll state. Returns an objects with a `scrollTop` number value for the entire grid and a `scrollLeft`
   * object containing a left position scroll value per sub grid.
   * @returns {Object}
   * @category Scrolling
   */
  storeScroll() {
    const me = this,
      state = me.storedScrollState = {
        scrollTop: me.scrollable.y,
        scrollLeft: {}
      };
    me.eachSubGrid(subGrid => {
      state.scrollLeft[subGrid.region] = subGrid.scrollable.x;
    });
    return state;
  }
  /**
   * Restore scroll state. If state is not specified, restores the last stored state.
   * @param {Object} [state] Scroll state, optional
   * @category Scrolling
   */
  restoreScroll(state = this.storedScrollState) {
    const me = this;
    me.eachSubGrid(subGrid => {
      const x = state.scrollLeft[subGrid.region];
      // Force scrollable to set its position to the underlying element in case it was removed and added back to
      // the DOM prior to restoring state
      if (x != null) {
        var _subGrid$fakeScroller;
        subGrid.scrollable.updateX(x);
        subGrid.header.scrollable.updateX(x);
        subGrid.footer.scrollable.updateX(x);
        (_subGrid$fakeScroller = subGrid.fakeScroller) === null || _subGrid$fakeScroller === void 0 ? void 0 : _subGrid$fakeScroller.updateX(x);
      }
    });
    me.scrollable.updateY(state.scrollTop);
  }
  //endregion
  //region Theme & measuring
  beginGridMeasuring() {
    const me = this;
    if (!me.$measureCellElements) {
      me.$measureCellElements = DomHelper.createElement({
        // For row height measuring, features are not yet there. Work around that for the stripe feature,
        // which removes borders
        className: 'b-grid-subgrid ' + (!me._isRowMeasured && me.hasFeature('stripe') ? 'b-stripe' : ''),
        reference: 'subGridElement',
        style: {
          position: 'absolute',
          top: '-10000px',
          left: '-100000px',
          visibility: 'hidden',
          contain: 'strict'
        },
        children: [{
          className: 'b-grid-row',
          reference: 'rowElement',
          children: [{
            className: 'b-grid-cell',
            reference: 'cellElement',
            style: {
              width: 'auto',
              contain: BrowserHelper.isFirefox ? 'layout paint' : 'layout style paint'
            }
          }]
        }]
      });
    }
    // Bring element into life if we get here early, to be able to access verticalScroller below
    me.getConfig('element');
    // Temporarily add to where subgrids live, to get have all CSS classes in play
    me.verticalScroller.appendChild(me.$measureCellElements.subGridElement);
    // Not yet on page, which prevents us from getting style values. Add it to the DOM temporarily
    if (!me.rendered) {
      const targetEl = me.appendTo || me.insertBefore || document.body,
        rootElement = DomHelper.getRootElement(typeof targetEl === 'string' ? document.getElementById(targetEl) : targetEl) || document.body;
      if (!me.adopt || !rootElement.contains(me.element)) {
        rootElement.appendChild(me.element);
        me.$removeAfterMeasuring = true;
      }
    }
    return me.$measureCellElements;
  }
  endGridMeasuring() {
    // Remove grid from DOM if it was added for measuring
    if (this.$removeAfterMeasuring) {
      this.element.remove();
      this.$removeAfterMeasuring = false;
    }
    // Remove measuring elements from grid
    this.$measureCellElements.subGridElement.remove();
  }
  /**
   * Creates a fake subgrid with one row and measures its height. Result is used as rowHeight.
   * @private
   */
  measureRowHeight() {
    const me = this,
      // Create a fake subgrid with one row, since styling for row is specified on .b-grid-subgrid .b-grid-row
      {
        rowElement
      } = me.beginGridMeasuring(),
      // Use style height or default height from config.
      // Not using clientHeight since it will have some value even if no height specified in CSS
      styles = DomHelper.getStyleValue(rowElement, ['height', 'border-top-width', 'border-bottom-width']),
      styleHeight = parseInt(styles.height),
      // FF reports border width adjusted to device pixel ration, e.g. on a 150% scaling it would tell 0.6667px width
      // for a 1px border. Dividing by the integer part to take base devicePixelRatio into account
      multiplier = BrowserHelper.isFirefox ? globalThis.devicePixelRatio / Math.max(Math.trunc(globalThis.devicePixelRatio), 1) : 1,
      borderTop = styles['border-top-width'] ? Math.round(multiplier * parseFloat(styles['border-top-width'])) : 0,
      borderBottom = styles['border-bottom-width'] ? Math.round(multiplier * parseFloat(styles['border-bottom-width'])) : 0;
    // Change rowHeight if specified in styling, also remember that value to replace later if theme changes and
    // user has not explicitly set some other height
    if (me.rowHeight == null || me.rowHeight === me._rowHeightFromStyle) {
      me.rowHeight = !isNaN(styleHeight) && styleHeight ? styleHeight : me.defaultRowHeight;
      me._rowHeightFromStyle = me.rowHeight;
    }
    // this measurement will be added to rowHeight during rendering, to get correct cell height
    me._rowBorderHeight = borderTop + borderBottom;
    me._isRowMeasured = true;
    me.endGridMeasuring();
    // There is a ticket about measuring the actual first row instead:
    // https://app.assembla.com/spaces/bryntum/tickets/5735-measure-first-real-rendered-row-for-rowheight/details
  }
  /**
   * Handler for global theme change event (triggered by shared.js). Remeasures row height.
   * @private
   */
  onThemeChange({
    theme
  }) {
    // Can only measure when we are visible, so do it next time we are.
    this.whenVisible('measureRowHeight');
    this.trigger('theme', {
      theme
    });
  }
  //endregion
  //region Rendering of rows
  /**
   * Triggers a render of records to all row elements. Call after changing order, grouping etc. to reflect changes
   * visually. Preserves scroll.
   * @category Rendering
   */
  refreshRows(returnToTop = false) {
    const {
      element,
      rowManager
    } = this;
    element.classList.add('b-notransition');
    if (returnToTop) {
      rowManager.returnToTop();
    } else {
      rowManager.refresh();
    }
    element.classList.remove('b-notransition');
  }
  /**
   * Triggers a render of all the cells in a column.
   * @param {Grid.column.Column} column
   * @category Rendering
   */
  refreshColumn(column) {
    if (column.isVisible) {
      if (column.isLeaf) {
        this.rowManager.forEach(row => row.renderCell(row.getCell(column.id)));
      } else {
        column.children.forEach(child => this.refreshColumn(child));
      }
    }
  }
  //endregion
  //region Render the grid
  /**
   * Recalculates virtual scrollbars widths and scrollWidth
   * @private
   */
  refreshVirtualScrollbars() {
    // NOTE: This was at some point changed to only run on platforms with width-occupying scrollbars, but it needs
    // to run with overlayed scrollbars also to make them show/hide as they should.
    const me = this,
      {
        headerContainer,
        footerContainer,
        virtualScrollers,
        scrollable,
        hasVerticalOverflow
      } = me,
      {
        classList
      } = virtualScrollers,
      hadHorizontalOverflow = !classList.contains('b-hide-display'),
      // We need to ask each subGrid if it has horizontal overflow.
      // If any do, we show the virtual scroller, otherwise we hide it.
      hasHorizontalOverflow = Object.values(me.subGrids).some(subGrid => subGrid.overflowingHorizontally),
      horizontalOverflowChanged = hasHorizontalOverflow !== hadHorizontalOverflow;
    // If horizontal overflow state changed, the docked horizontal scrollbar's visibility
    //  must be synced to match, and this may cause a height change;
    if (horizontalOverflowChanged) {
      virtualScrollers.classList.toggle('b-hide-display', !hasHorizontalOverflow);
    }
    // Auto-widthed padding element at end hides or shows to create matching margin.
    if (DomHelper.scrollBarWidth) {
      // Header will need its extra padding if we have overflow, *OR* if we are overflowY : scroll
      const needsPadding = hasVerticalOverflow || scrollable.overflowY === 'scroll';
      headerContainer.classList.toggle('b-show-yscroll-padding', needsPadding);
      footerContainer.classList.toggle('b-show-yscroll-padding', needsPadding);
      virtualScrollers.classList.toggle('b-show-yscroll-padding', needsPadding);
      // Do any measuring necessitated by show/hide of the docked horizontal scrollbar
      /// *after* mutating DOM classnames.
      if (horizontalOverflowChanged) {
        // If any subgrids reported they have horizontal overflow, then we have to ask them
        // to sync the widths of the scroll elements inside the docked horizontal scrollbar
        // so that it takes up the required scrollbar width at the bottom of our body element.
        if (hasHorizontalOverflow) {
          me.callEachSubGrid('refreshFakeScroll');
        }
        me.onHeightChange();
      }
    }
  }
  get hasVerticalOverflow() {
    return this.scrollable.hasOverflow('y');
  }
  /**
   * Returns content height calculated from row manager
   * @private
   */
  get contentHeight() {
    const rowManager = this.rowManager;
    return Math.max(rowManager.totalHeight, rowManager.bottomRow ? rowManager.bottomRow.bottom : 0);
  }
  onContentChange() {
    const me = this,
      rowManager = me.rowManager;
    if (me.isVisible) {
      rowManager.estimateTotalHeight();
      me.paintListener = null;
      me.refreshTotalHeight(me.contentHeight);
      me.callEachSubGrid('refreshFakeScroll');
      me.onHeightChange();
    }
    // If not visible, this operation MUST be done when we become visible.
    // This is announced by the paint event which is triggered when a Widget
    // really gains visibility, ie is shown or rendered, or it's not hidden,
    // and a hidden/non-rendered ancestor is shown or rendered.
    // See Widget#triggerPaint.
    else if (!me.paintListener) {
      me.paintListener = me.ion({
        paint: 'onContentChange',
        once: true,
        thisObj: me
      });
    }
  }
  triggerPaint() {
    if (!this.isPainted) {
      this.refreshBodyRectangle();
    }
    super.triggerPaint();
  }
  onHeightChange() {
    const me = this;
    // cache to avoid recalculations in the middle of rendering code (RowManger#getRecordCoords())
    me.refreshBodyRectangle();
    me._bodyHeight = me.autoHeight ? me.contentHeight : me.bodyContainer.offsetHeight;
  }
  suspendRefresh() {
    this.refreshSuspended++;
  }
  resumeRefresh(trigger) {
    if (this.refreshSuspended && ! --this.refreshSuspended) {
      if (trigger) {
        this.refreshRows();
      }
      this.trigger('resumeRefresh', {
        trigger
      });
    }
  }
  /**
   * Rerenders all grid rows, completely replacing all row elements with new ones
   * @category Rendering
   */
  renderRows(keepScroll = true) {
    const me = this,
      scrollState = keepScroll && me.storeScroll();
    if (me.refreshSuspended) {
      return;
    }
    /**
     * Grid rows are about to be rendered
     * @event beforeRenderRows
     * @param {Grid.view.Grid} source This grid.
     */
    me.trigger('beforeRenderRows');
    me.renderingRows = true;
    // This allows us to do things like disable animations on a refresh
    me.element.classList.add('b-grid-refreshing');
    if (!keepScroll) {
      me.scrollable.y = me._scrollTop = 0;
    }
    me.rowManager.reinitialize(!keepScroll);
    /**
     * Grid rows have been rendered
     * @event renderRows
     * @param {Grid.view.Grid} source This grid.
     */
    me.trigger('renderRows');
    me.renderingRows = false;
    me.onContentChange();
    if (keepScroll) {
      me.restoreScroll(scrollState);
    }
    me.element.classList.remove('b-grid-refreshing');
  }
  /**
   * Rerenders the grids rows, headers and footers, completely replacing all row elements with new ones
   * @category Rendering
   */
  renderContents() {
    const me = this,
      {
        element,
        headerContainer,
        footerContainer,
        rowManager
      } = me;
    me.emptyCache();
    // columns will be "drawn" on render anyway, bail out
    if (me.isPainted) {
      // reset measured header height, to make next call to get headerHeight measure it
      me._headerHeight = null;
      me.callEachSubGrid('refreshHeader', headerContainer);
      me.callEachSubGrid('refreshFooter', footerContainer);
      // Note that these are hook methods for features to plug in to. They do not do anything.
      me.renderHeader(headerContainer, element);
      me.renderFooter(footerContainer, element);
      me.fixSizes();
      // any elements currently used for rows should be released.
      // actual removal of elements is done in SubGrid#clearRows
      const refreshContext = rowManager.removeAllRows();
      rowManager.calculateRowCount(false, true, true);
      if (rowManager.rowCount) {
        // Sets up the RowManager's position for when renderRows calls RowManager#reinitialize
        // so that it renders the correct data block at the correct position.
        rowManager.setPosition(refreshContext);
        me.renderRows();
      }
    }
  }
  onPaintOverride() {
    // Internal procedure used for paint method overrides
    // Not used in onPaint() because it may be chained on instance and Override won't be applied
  }
  // Render rows etc. on first paint, to make sure Grids element has been laid out
  onPaint({
    firstPaint
  }) {
    var _super$onPaint;
    const me = this;
    me.ariaElement.setAttribute('aria-rowcount', me.store.count + 1);
    (_super$onPaint = super.onPaint) === null || _super$onPaint === void 0 ? void 0 : _super$onPaint.call(this, ...arguments);
    if (me.onPaintOverride() || !firstPaint) {
      return;
    }
    const {
        rowManager,
        store,
        element,
        headerContainer,
        bodyContainer,
        footerContainer
      } = me,
      scrollPad = DomHelper.scrollBarPadElement;
    let columnsChanged,
      maxDepth = 0;
    // ARIA. Update our ariaElement that encapsulates all rows.
    // The header is counted as a row, and column headers are cells.
    me.role = store !== null && store !== void 0 && store.isTree ? 'treegrid' : 'grid';
    // See if updateResponsive changed any columns.
    me.columns.ion({
      change: () => columnsChanged = true,
      once: true
    });
    // Apply any responsive configs before rendering rows.
    me.updateResponsive(me.width, 0);
    // If there were any column changes, apply them
    if (columnsChanged) {
      me.callEachSubGrid('refreshHeader', headerContainer);
      me.callEachSubGrid('refreshFooter', footerContainer);
    }
    // Note that these are hook methods for features to plug in to. They do not do anything.
    // SubGrids take care of their own rendering.
    me.renderHeader(headerContainer, element);
    me.renderFooter(footerContainer, element);
    // These padding elements are only visible on scrollbar showing platforms.
    // And then, only when the owning element as the b-show-yscroll-padding class added.
    // See refreshVirtualScrollbars where this is synced on the header, footer and scroller elements.
    DomHelper.append(headerContainer, scrollPad);
    DomHelper.append(footerContainer, scrollPad);
    DomHelper.append(me.virtualScrollers, scrollPad);
    // Cached, updated on resize. Used by RowManager and by the subgrids upon their render.
    // Measure after header and footer have been rendered and taken their height share.
    me.refreshBodyRectangle();
    const bodyOffsetHeight = me.bodyContainer.offsetHeight;
    if (me.autoHeight) {
      me._bodyHeight = rowManager.initWithHeight(element.offsetHeight - headerContainer.offsetHeight - footerContainer.offsetHeight, true);
      bodyContainer.style.height = me.bodyHeight + 'px';
    } else {
      me._bodyHeight = bodyOffsetHeight;
      rowManager.initWithHeight(me._bodyHeight, true);
    }
    me.eachSubGrid(subGrid => {
      if (subGrid.header.maxDepth > maxDepth) {
        maxDepth = subGrid.header.maxDepth;
      }
    });
    headerContainer.dataset.maxDepth = maxDepth;
    me.fixSizes();
    if (store.count || !store.isLoading) {
      me.renderRows();
    }
    // With autoHeight cells we need to refresh rows when fonts are loaded, to get correct measurements
    if (me.columns.usesAutoHeight) {
      const {
        fonts
      } = document;
      if ((fonts === null || fonts === void 0 ? void 0 : fonts.status) !== 'loaded') {
        fonts.ready.then(() => !me.isDestroyed && me.refreshRows());
      }
    }
    me.initScroll();
    me.initInternalEvents();
  }
  render() {
    const me = this;
    // When displayed inside one of our containers, require a size to be considered visible. Ensures it is painted
    // on display when for example in a tab
    me.requireSize = Boolean(me.owner);
    // Render as a container. This renders the child SubGrids
    super.render(...arguments);
    if (!me.autoHeight) {
      // Sanity check that main element has been given some sizing styles, unless autoHeight is used in which case
      // it will be sized programmatically instead
      if (me.headerContainer.offsetHeight && !me.bodyContainer.offsetHeight) {
        console.warn('Grid element not sized correctly, please check your CSS styles and review how you size the widget');
      }
      // Warn if height equals the predefined minHeight, likely that is not what the dev intended
      if (!('minHeight' in me.initialConfig) && !('height' in me.initialConfig) && parseInt(globalThis.getComputedStyle(me.element).minHeight) === me.height) {
        console.warn(`The ${me.$$name} is sized by its predefined minHeight, likely this is not intended. ` + `Please check your CSS and review how you size the widget, or assign a fixed height in the config. ` + `For more information, see the "Basics/Sizing the component" guide in docs.`);
      }
    }
  }
  //endregion
  //region Hooks
  /**
   * Called after headers have been rendered to the headerContainer.
   * This does not do anything, it's just for Features to hook in to.
   * @param {HTMLElement} headerContainer DOM element which contains the headers.
   * @param {HTMLElement} element Grid element
   * @private
   * @category Rendering
   */
  renderHeader(headerContainer, element) {}
  /**
   * Called after footers have been rendered to the footerContainer.
   * This does not do anything, it's just for Features to hook in to.
   * @param {HTMLElement} footerContainer DOM element which contains the footers.
   * @param {HTMLElement} element Grid element
   * @private
   * @category Rendering
   */
  renderFooter(footerContainer, element) {}
  // Hook for features to affect cell rendering before renderers are run
  beforeRenderCell() {}
  // Hooks for features to react to a row being rendered
  beforeRenderRow() {}
  afterRenderRow() {}
  // Hook for features to react to scroll
  afterScroll() {}
  // Hook that can be overridden to prepare custom editors, can be used by framework wrappers
  processCellEditor(editorConfig) {}
  // Hook for features to react to column changes
  afterColumnsChange() {}
  // Hook for features to react to record removal (which might be transitioned)
  afterRemove(removeEvent) {}
  // Hook for features to react to groups being collapsed/expanded
  afterToggleGroup() {}
  // Hook for features to react to subgrid being collapsed
  afterToggleSubGrid() {}
  //endregion
  //region Masking and Appearance
  syncMaskCover(mask = this.masked) {
    if (mask) {
      const bodyRect = mask.cover === 'body' && this.rectangleOf('bodyContainer'),
        scrollerRect = bodyRect && this.rectangleOf('virtualScrollers'),
        {
          style
        } = mask.element;
      // the width of the bodyCt covers the vscroll but the height does not cover the hscroll:
      style.marginTop = bodyRect ? `${bodyRect.y}px` : '';
      style.height = bodyRect ? `${bodyRect.height + ((scrollerRect === null || scrollerRect === void 0 ? void 0 : scrollerRect.height) || 0)}px` : '';
    }
  }
  /**
   * Show a load mask with a spinner and the specified message. When using an AjaxStore masking and unmasking is
   * handled automatically, but if you are loading data in other ways you can call this function manually when your
   * load starts.
   * ```
   * myLoadFunction() {
   *   // Show mask before initiating loading
   *   grid.maskBody('Loading data');
   *   // Your custom loading code
   *   load.then(() => {
   *      // Hide the mask when loading is finished
   *      grid.unmaskBody();
   *   });
   * }
   * ```
   * @param {String|MaskConfig} loadMask The message to show in the load mask (next to the spinner) or a config object
   * for a {@link Core.widget.Mask}.
   * @returns {Core.widget.Mask}
   * @category Misc
   */
  maskBody(loadMask) {
    let ret;
    if (this.bodyContainer) {
      this.masked = Mask.mergeConfigs(this.loadMaskDefaults, loadMask); // smart setter
      ret = this.masked; // read back
    }

    return ret;
  }
  /**
   * Hide the load mask.
   * @category Misc
   */
  unmaskBody() {
    this.masked = null;
  }
  updateEmptyText(emptyText) {
    var _this$emptyTextEl, _this$firstItem;
    (_this$emptyTextEl = this.emptyTextEl) === null || _this$emptyTextEl === void 0 ? void 0 : _this$emptyTextEl.remove();
    // Grid might be created without subgrids, will add element to first when it is added
    this.emptyTextEl = DomHelper.createElement({
      parent: (_this$firstItem = this.firstItem) === null || _this$firstItem === void 0 ? void 0 : _this$firstItem.element,
      className: 'b-empty-text',
      [emptyText !== null && emptyText !== void 0 && emptyText.includes('<') ? 'html' : 'text']: emptyText
    });
  }
  toggleEmptyText() {
    const {
      bodyContainer,
      store
    } = this;
    bodyContainer === null || bodyContainer === void 0 ? void 0 : bodyContainer.classList.toggle('b-grid-empty', !(store.count > 0 || store.isLoading || store.isCommitting));
  }
  // Notify columns when our read-only state is toggled
  updateReadOnly(readOnly, old) {
    super.updateReadOnly(readOnly, old);
    if (!this.isConfiguring) {
      for (const column of this.columns.bottomColumns) {
        var _column$updateReadOnl;
        (_column$updateReadOnl = column.updateReadOnly) === null || _column$updateReadOnl === void 0 ? void 0 : _column$updateReadOnl.call(column, readOnly);
      }
    }
  }
  //endregion
  //region Extract config
  // This function is not meant to be called by any code other than Base#getCurrentConfig().
  // It extracts the current configs for the grid, with special handling for inline data
  getCurrentConfig(options) {
    const result = super.getCurrentConfig(options),
      {
        store
      } = this,
      // Clean up inline data to not have group records in it
      data = store.getInlineData(options),
      // Get stores current state, in case it has filters etc. added at runtime
      storeState = store.getCurrentConfig(options) || result.store;
    if (data.length) {
      result.data = data;
    }
    // Don't include the default model class
    if (storeState && store.originalModelClass === GridRowModel) {
      delete storeState.modelClass;
    }
    if (!ObjectHelper.isEmpty(storeState)) {
      result.store = storeState;
    }
    if (result.store) {
      delete result.store.data;
    }
    return result;
  }
  //endregion
}
// Register this widget type with its Factory
GridBase.initClass();
VersionHelper.setVersion('grid', '5.3.7');
GridBase._$name = 'GridBase';

export { Bar, CellEdit, CellMenu, CheckColumn, Column, ColumnDragToolbar, ColumnPicker, ColumnReorder, ColumnResize, ColumnStore, Filter, FilterBar, Footer, GridBase, GridElementEvents, GridFeatureManager, GridFeatures, GridFieldFilterPicker, GridFieldFilterPickerGroup, GridResponsive, GridSelection, GridState, GridSubGrids, Group, Header, HeaderMenu, Location, Row, RowManager, RowNumberColumn, Sort, Stripe, SubGrid, WidgetColumn };
//# sourceMappingURL=GridBase.js.map
