/*!
 *
 * Bryntum Scheduler Pro 5.3.7
 *
 * Copyright(c) 2023 Bryntum AB
 * https://bryntum.com/contact
 * https://bryntum.com/license
 *
 */
import { stripDuplicates, CalendarCache, IntervalCache, TimeSpan, DependencyBaseModel, DependencyModel } from './ProjectModel.js';
import { ColumnStore, WidgetColumn, GridFeatureManager, HeaderMenu, Header as Header$1, SubGrid, GridBase } from './GridBase.js';
import { NumberColumn } from './RegionResize.js';
import { ObjectHelper, Duration, DateHelper, Widget, DomSync, EventHelper, DomHelper, StringHelper, DomClassList, Events, Model, IdHelper, Localizable, Store, unitMagnitudes, InstancePlugin, Objects, Tooltip, Rectangle, BrowserHelper, VersionHelper, Delayable, Base as Base$1, ArrayHelper, WalkHelper, GlobalEvents, DomDataStore, Scroller, ResizeMonitor, Collection, FunctionHelper } from './Editor.js';
import { DragHelper, ResizeHelper } from './MessageDialog.js';
import { AvatarRendering, Draggable, Droppable } from './AvatarRendering.js';
import { ClockTemplate, TaskEditStm, AttachToProjectMixin, RecurringEvents } from './EventNavigation.js';
import './Slider.js';

//---------------------------------------------------------------------------------------------------------------------
/**
 * This is a base class, providing the type-safe static constructor [[new]]. This is very convenient when using
 * [[Mixin|mixins]], as mixins can not have types in the constructors.
 */
class Base {
  /**
   * This method applies its 1st argument (if any) to the current instance using `Object.assign()`.
   *
   * Supposed to be overridden in the subclasses to customize the instance creation process.
   *
   * @param props
   */
  initialize(props) {
    props && Object.assign(this, props);
  }
  /**
   * This is a type-safe static constructor method, accepting a single argument, with the object, corresponding to the
   * class properties. It will generate a compilation error, if unknown property is provided.
   *
   * For example:
   *
   * ```ts
   * class MyClass extends Base {
   *     prop     : string
   * }
   *
   * const instance : MyClass = MyClass.new({ prop : 'prop', wrong : 11 })
   * ```
   *
   * will produce:
   *
   * ```plaintext
   * TS2345: Argument of type '{ prop: string; wrong: number; }' is not assignable to parameter of type 'Partial<MyClass>'.
   * Object literal may only specify known properties, and 'wrong' does not exist in type 'Partial<MyClass>'
   * ```
   *
   * The only thing this constructor does is create an instance and call the [[initialize]] method on it, forwarding
   * the first argument. The customization of instance is supposed to be performed in that method.
   *
   * @param props
   */
  static new(props) {
    const instance = new this();
    instance.initialize(props);
    return instance;
  }
}

class CalendarCacheIntervalMultiple {
  constructor(config) {
    this.intervalGroups = [];
    config && Object.assign(this, config);
  }
  combineWith(interval) {
    const copy = this.intervalGroups.slice();
    copy.push([interval.calendar, interval]);
    return new CalendarCacheIntervalMultiple({
      intervalGroups: copy
    });
  }
  getIsWorkingForEvery() {
    if (this.isWorkingForEvery != null) return this.isWorkingForEvery;
    for (let [_calendar, intervals] of this.getGroups()) {
      if (!intervals[0].isWorking) return this.isWorkingForEvery = false;
    }
    return this.isWorkingForEvery = true;
  }
  getIsWorkingForSome() {
    if (this.isWorkingForSome != null) return this.isWorkingForSome;
    for (let [_calendar, intervals] of this.getGroups()) {
      if (intervals[0].isWorking) return this.isWorkingForSome = true;
    }
    return this.isWorkingForSome = false;
  }
  getCalendars() {
    this.getGroups();
    return this.calendars;
  }
  isCalendarWorking(calendar) {
    return this.getCalendarsWorkStatus().get(calendar);
  }
  getCalendarsWorkStatus() {
    if (this.calendarsWorkStatus) return this.calendarsWorkStatus;
    const res = new Map();
    for (let [calendar, intervals] of this.getGroups()) {
      res.set(calendar, intervals[0].isWorking);
    }
    return this.calendarsWorkStatus = res;
  }
  getCalendarsWorking() {
    if (this.calendarsWorking) return this.calendarsWorking;
    const calendars = [];
    for (let [calendar, intervals] of this.getGroups()) {
      if (intervals[0].isWorking) calendars.push(calendar);
    }
    return this.calendarsWorking = calendars;
  }
  getCalendarsNonWorking() {
    if (this.calendarsNonWorking) return this.calendarsNonWorking;
    const calendars = [];
    for (let [calendar, intervals] of this.getGroups()) {
      if (!intervals[0].isWorking) calendars.push(calendar);
    }
    return this.calendarsNonWorking = calendars;
  }
  getGroups() {
    if (this.intervalsByCalendar) return this.intervalsByCalendar;
    const calendars = this.calendars = [];
    const intervalsByCalendar = new Map();
    this.intervalGroups.forEach(([calendar, interval]) => {
      let data = intervalsByCalendar.get(calendar);
      if (!data) {
        calendars.push(calendar);
        data = [];
        intervalsByCalendar.set(calendar, data);
      }
      data.push.apply(data, interval.intervals);
    });
    intervalsByCalendar.forEach((intervals, calendar) => {
      const unique = stripDuplicates(intervals);
      unique.sort(
      // sort in decreasing order
      (interval1, interval2) => interval2.getPriorityField() - interval1.getPriorityField());
      intervalsByCalendar.set(calendar, unique);
    });
    return this.intervalsByCalendar = intervalsByCalendar;
  }
}

/**
 * The calendar cache for combination of multiple calendars
 */
class CalendarCacheMultiple extends CalendarCache {
  constructor(config) {
    super(config);
    this.calendarCaches = stripDuplicates(this.calendarCaches);
    this.intervalCache = new IntervalCache({
      emptyInterval: new CalendarCacheIntervalMultiple(),
      combineIntervalsFn: (interval1, interval2) => {
        return interval1.combineWith(interval2);
      }
    });
  }
  fillCache(startDate, endDate) {
    this.calendarCaches.forEach(calendarCache => {
      calendarCache.fillCache(startDate, endDate);
      this.includeWrappingRangeFrom(calendarCache, startDate, endDate);
    });
  }
}
const COMBINED_CALENDARS_CACHE = new Map();
const combineCalendars = calendars => {
  const uniqueOnly = stripDuplicates(calendars);
  if (uniqueOnly.length === 0) throw new Error("No calendars to combine");
  uniqueOnly.sort((calendar1, calendar2) => {
    if (calendar1.internalId < calendar2.internalId) return -1;else return 1;
  });
  const hash = uniqueOnly.map(calendar => calendar.internalId + '/').join('');
  const versionsHash = uniqueOnly.map(calendar => calendar.version + '/').join('');
  let cached = COMBINED_CALENDARS_CACHE.get(hash);
  let res;
  if (cached && cached.versionsHash === versionsHash) res = cached.cache;else {
    res = new CalendarCacheMultiple({
      calendarCaches: uniqueOnly.map(calendar => calendar.calendarCache)
    });
    // COMBINED_CALENDARS_CACHE.set(hash, {
    //     versionsHash    : versionsHash,
    //     cache           : res
    // })
  }

  return res;
};

/**
 * @module Scheduler/column/DurationColumn
 */
/**
 * A column showing the task {@link Scheduler/model/TimeSpan#field-fullDuration duration}. Please note, this column
 * is preconfigured and expects its field to be of the {@link Core.data.Duration} type.
 *
 * The default editor is a {@link Core.widget.DurationField}. It parses time units, so you can enter "4d"
 * indicating 4 days duration, or "4h" indicating 4 hours, etc. The numeric magnitude can be either an integer or a
 * float value. Both "," and "." are valid decimal separators. For example, you can enter "4.5d" indicating 4.5 days
 * duration, or "4,5h" indicating 4.5 hours.
 *
 * {@inlineexample Scheduler/column/DurationColumn.js}
 * @extends Grid/column/NumberColumn
 * @classType duration
 * @column
 */
class DurationColumn extends NumberColumn {
  compositeField = true;
  //region Config
  static get $name() {
    return 'DurationColumn';
  }
  static get type() {
    return 'duration';
  }
  static get isGanttColumn() {
    return true;
  }
  static get fields() {
    return [
    /**
     * Precision of displayed duration, defaults to use {@link Scheduler.view.Scheduler#config-durationDisplayPrecision}.
     * Specify an integer value to override that setting, or `false` to use raw value
     * @config {Number|Boolean} decimalPrecision
     */
    {
      name: 'decimalPrecision',
      defaultValue: 1
    }];
  }
  static get defaults() {
    return {
      /**
       * Min value
       * @config {Number}
       */
      min: null,
      /**
       * Max value
       * @config {Number}
       */
      max: null,
      /**
       * Step size for spin button clicks.
       * @member {Number} step
       */
      /**
       * Step size for spin button clicks. Also used when pressing up/down keys in the field.
       * @config {Number}
       * @default
       */
      step: 1,
      /**
       * Large step size, defaults to 10 * `step`. Applied when pressing SHIFT and stepping either by click or
       * using keyboard.
       * @config {Number}
       * @default 10
       */
      largeStep: 0,
      field: 'fullDuration',
      text: 'L{Duration}',
      instantUpdate: true,
      // Undocumented, used by Filter feature to get type of the filter field
      filterType: 'duration',
      sortable(durationEntity1, durationEntity2) {
        const ms1 = durationEntity1[this.field],
          ms2 = durationEntity2[this.field];
        return ms1 - ms2;
      }
    };
  }
  construct() {
    super.construct(...arguments);
    const sortFn = this.sortable;
    this.sortable = (...args) => sortFn.call(this, ...args);
  }
  get defaultEditor() {
    const {
      max,
      min,
      step,
      largeStep
    } = this;
    // Remove any undefined configs, to allow config system to use default values instead
    return ObjectHelper.cleanupProperties({
      type: 'duration',
      name: this.field,
      max,
      min,
      step,
      largeStep
    });
  }
  //endregion
  //region Internal
  get durationUnitField() {
    return `${this.field}Unit`;
  }
  roundValue(duration) {
    const nbrDecimals = typeof this.grid.durationDisplayPrecision === 'number' ? this.grid.durationDisplayPrecision : this.decimalPrecision,
      multiplier = Math.pow(10, nbrDecimals),
      rounded = Math.round(duration * multiplier) / multiplier;
    return rounded;
  }
  formatValue(duration, durationUnit) {
    if (duration instanceof Duration) {
      durationUnit = duration.unit;
      duration = duration.magnitude;
    }
    duration = this.roundValue(duration);
    return duration + ' ' + DateHelper.getLocalizedNameOfUnit(durationUnit, duration !== 1);
  }
  //endregion
  //region Render
  defaultRenderer({
    record,
    isExport
  }) {
    const value = record[this.field],
      type = typeof value,
      durationValue = type === 'number' ? value : value === null || value === void 0 ? void 0 : value.magnitude,
      durationUnit = type === 'number' ? record[this.durationUnitField] : value === null || value === void 0 ? void 0 : value.unit;
    // in case of bad input (for instance NaN, undefined or NULL value)
    if (typeof durationValue !== 'number') {
      return isExport ? '' : null;
    }
    return this.formatValue(durationValue, durationUnit);
  }
  //endregion
  // Used with CellCopyPaste as fullDuration doesn't work via record.get
  toClipboardString({
    record
  }) {
    return record[this.field].toString();
  }
  fromClipboardString({
    string,
    record
  }) {
    const duration = DateHelper.parseDuration(string, true, this.durationUnit);
    if (duration && 'magnitude' in duration) {
      return duration;
    }
    return record.fullDuration;
  }
  calculateFillValue({
    value,
    record
  }) {
    return this.fromClipboardString({
      string: value,
      record
    });
  }
}
ColumnStore.registerColumnType(DurationColumn);
DurationColumn._$name = 'DurationColumn';

/**
 * @module Scheduler/view/TimeAxisBase
 */
function isLastLevel(level, levels) {
  return level === levels.length - 1;
}
function isLastCell(level, cell) {
  return cell === level.cells[level.cells.length - 1];
}
/**
 * Base class for HorizontalTimeAxis and VerticalTimeAxis. Contains shared functionality to only render ticks in view,
 * should not be used directly.
 *
 * @extends Core/widget/Widget
 * @private
 * @abstract
 */
class TimeAxisBase extends Widget {
  static $name = 'TimeAxisBase';
  //region Config
  static configurable = {
    /**
     * The minimum width for a bottom row header cell to be considered 'compact', which adds a special CSS class
     * to the row (for special styling). Copied from Scheduler/Gantt.
     * @config {Number}
     * @default
     */
    compactCellWidthThreshold: 15,
    // TimeAxisViewModel
    model: null,
    cls: null,
    /**
     * Style property to use as cell size. Either width or height depending on orientation
     * @config {'width'|'height'}
     * @private
     */
    sizeProperty: null,
    /**
     * Style property to use as cells position. Either left or top depending on orientation
     * @config {'left'|'top'}
     * @private
     */
    positionProperty: null
  };
  startDate = null;
  endDate = null;
  levels = [];
  size = null;
  // Set visible date range
  set range({
    startDate,
    endDate
  }) {
    const me = this;
    // Only process a change
    if (me.startDate - startDate || me.endDate - endDate) {
      me.startDate = startDate;
      me.endDate = endDate;
      me.refresh();
    }
  }
  //endregion
  //region Html & rendering
  // Generates element configs for all levels defined by the current ViewPreset
  buildCells(start = this.startDate, end = this.endDate) {
    var _me$client;
    const me = this,
      {
        sizeProperty
      } = me,
      {
        stickyHeaders,
        isVertical
      } = me.client || {},
      featureHeaderConfigs = [],
      {
        length
      } = me.levels;
    const cellConfigs = me.levels.map((level, i) => {
      var _level$cells;
      const stickyHeader = stickyHeaders && (isVertical || i < length - 1);
      return {
        className: {
          'b-sch-header-row': 1,
          [`b-sch-header-row-${level.position}`]: 1,
          'b-sch-header-row-main': i === me.model.viewPreset.mainHeaderLevel,
          'b-lowest': isLastLevel(i, me.levels),
          'b-sticky-header': stickyHeader
        },
        syncOptions: {
          // Keep a maximum of 5 released cells. Might be fine with fewer since ticks are fixed width.
          // Prevents an unnecessary amount of cells from sticking around when switching from narrow to
          // wide tickSizes
          releaseThreshold: 5,
          syncIdField: 'tickIndex'
        },
        dataset: {
          headerFeature: `headerRow${i}`,
          headerPosition: level.position
        },
        // Only include cells in view
        children: (_level$cells = level.cells) === null || _level$cells === void 0 ? void 0 : _level$cells.filter(cell => cell.start < end && cell.end > start).map((cell, j) => ({
          role: 'presentation',
          className: {
            'b-sch-header-timeaxis-cell': 1,
            [cell.headerCellCls]: cell.headerCellCls,
            [`b-align-${cell.align}`]: cell.align,
            'b-last': isLastCell(level, cell)
          },
          dataset: {
            tickIndex: cell.index,
            // Used in export tests to resolve dates from tick elements
            ...(globalThis.DEBUG && {
              date: cell.start.getTime()
            })
          },
          style: {
            // DomHelper appends px to numeric dimensions
            [me.positionProperty]: cell.coord,
            [sizeProperty]: cell.width,
            [`min-${sizeProperty}`]: cell.width
          },
          children: [{
            tag: 'span',
            role: 'presentation',
            className: {
              'b-sch-header-text': 1,
              'b-sticky-header': stickyHeader
            },
            html: cell.value
          }]
        }))
      };
    });
    // When tested in isolation there is no client
    (_me$client = me.client) === null || _me$client === void 0 ? void 0 : _me$client.getHeaderDomConfigs(featureHeaderConfigs);
    cellConfigs.push(...featureHeaderConfigs);
    // noinspection JSSuspiciousNameCombination
    return {
      className: me.widgetClassList,
      syncOptions: {
        // Do not keep entire levels no longer used, for example after switching view preset
        releaseThreshold: 0
      },
      children: cellConfigs
    };
  }
  render(targetElement) {
    super.render(targetElement);
    this.refresh(true);
  }
  /**
   * Refresh the UI
   * @param {Boolean} [rebuild] Specify `true` to force a rebuild of the underlying header level definitions
   */
  refresh(rebuild = !this.levels.length) {
    const me = this,
      {
        columnConfig
      } = me.model,
      {
        levels
      } = me,
      oldLevelsCount = levels.length;
    if (rebuild) {
      levels.length = 0;
      columnConfig.forEach((cells, position) => levels[position] = {
        position,
        cells
      });
      me.size = levels[0].cells.reduce((sum, cell) => sum + cell.width, 0);
      const {
        parentElement
      } = me.element;
      // Don't mutate a classList unless necessary. Browsers invalidate the style.
      if (parentElement && (levels.length !== oldLevelsCount || rebuild)) {
        parentElement.classList.remove(`b-sch-timeaxiscolumn-levels-${oldLevelsCount}`);
        parentElement.classList.add(`b-sch-timeaxiscolumn-levels-${levels.length}`);
      }
    }
    if (!me.startDate || !me.endDate) {
      return;
    }
    // Boil down levels to only show what is in view
    DomSync.sync({
      domConfig: me.buildCells(),
      targetElement: me.element,
      syncIdField: 'headerFeature'
    });
    me.trigger('refresh');
  }
  //endregion
  // Our widget class doesn't include "base".
  get widgetClass() {
    return 'b-timeaxis';
  }
}
TimeAxisBase._$name = 'TimeAxisBase';

/**
 * @module Scheduler/view/HorizontalTimeAxis
 */
/**
 * A visual horizontal representation of the time axis described in the
 * {@link Scheduler.preset.ViewPreset#field-headers} field.
 * Normally you should not interact with this class directly.
 *
 * @extends Scheduler/view/TimeAxisBase
 * @private
 */
class HorizontalTimeAxis extends TimeAxisBase {
  //region Config
  static $name = 'HorizontalTimeAxis';
  static type = 'horizontaltimeaxis';
  static configurable = {
    model: null,
    sizeProperty: 'width'
  };
  //endregion
  get positionProperty() {
    var _this$owner;
    return (_this$owner = this.owner) !== null && _this$owner !== void 0 && _this$owner.rtl ? 'right' : 'left';
  }
  get width() {
    return this.size;
  }
  onModelUpdate() {
    // Force rebuild when availableSpace has changed, to recalculate width and maybe apply compact styling
    if (this.model.availableSpace > 0 && this.model.availableSpace !== this.width) {
      this.refresh(true);
    }
  }
  updateModel(timeAxisViewModel) {
    this.detachListeners('tavm');
    timeAxisViewModel === null || timeAxisViewModel === void 0 ? void 0 : timeAxisViewModel.ion({
      name: 'tavm',
      update: 'onModelUpdate',
      thisObj: this
    });
  }
}
HorizontalTimeAxis._$name = 'HorizontalTimeAxis';

/**
 * @module Scheduler/view/ResourceHeader
 */
/**
 * Header widget that renders resource column headers and acts as the interaction point for resource columns in vertical
 * mode. Note that it uses virtual rendering and element reusage to gain performance, only headers in view are available
 * in DOM. Because of this you should avoid direct element manipulation, any such changes can be discarded at any time.
 *
 * By default, it displays resources `name` and also applies its `iconCls` if any, like this:
 *
 * ```html
 * <i class="iconCls">name</i>
 * ```
 *
 * If Scheduler is configured with a {@link Scheduler.view.Scheduler#config-resourceImagePath} the
 * header will render miniatures for the resources, using {@link Scheduler.model.ResourceModel#field-imageUrl}
 * or {@link Scheduler.model.ResourceModel#field-image} with fallback to
 * {@link Scheduler.model.ResourceModel#field-name} + {@link Scheduler.view.Scheduler#config-resourceImageExtension}
 * for unset values.
 *
 * The contents and styling of the resource cells in the header can be customized using {@link #config-headerRenderer}:
 *
 * ```javascript
 * new Scheduler({
 *     mode            : 'vertical',
 *     resourceColumns : {
 *         headerRenderer : ({ resourceRecord }) => `Hello ${resourceRecord.name}`
 *     }
 * }
 *```
 *
 * The width of the resource columns is determined by the {@link #config-columnWidth} config.
 *
 * @extends Core/widget/Widget
 */
class ResourceHeader extends Widget {
  //region Config
  static $name = 'ResourceHeader';
  static type = 'resourceheader';
  static configurable = {
    /**
     * Resource store used to render resource headers. Assigned from Scheduler.
     * @config {Scheduler.data.ResourceStore}
     * @private
     */
    resourceStore: null,
    /**
     * Custom header renderer function. Can be used to manipulate the element config used to create the element
     * for the header:
     *
     * ```javascript
     * new Scheduler({
     *   resourceColumns : {
     *     headerRenderer({ elementConfig, resourceRecord }) {
     *       elementConfig.dataset.myExtraData = 'extra';
     *       elementConfig.style.fontWeight = 'bold';
     *     }
     *   }
     * });
     * ```
     *
     * See {@link DomConfig} for more information.
     * Please take care to not break the default configs :)
     *
     * Or as a template by returning HTML from the function:
     *
     * ```javascript
     * new Scheduler({
     *   resourceColumns : {
     *     headerRenderer : ({ resourceRecord }) => `
     *       <div class="my-custom-template">
     *       ${resourceRecord.firstName} {resourceRecord.surname}
     *       </div>
     *     `
     *   }
     * });
     * ```
     *
     * NOTE: When using `headerRenderer` no default internal markup is applied to the resource header cell,
     * `iconCls` and {@link Scheduler.model.ResourceModel#field-imageUrl} or {@link Scheduler.model.ResourceModel#field-image}
     * will have no effect unless you supply custom markup for them.
     *
     * @config {Function}
     * @param {Object} params Object containing the params below
     * @param {Scheduler.model.ResourceModel} params.resourceRecord Resource whose header is being rendered
     * @param {DomConfig} params.elementConfig A config object used to create the element for the resource
     */
    headerRenderer: null,
    /**
     * Set to `false` to render just the resource name, `true` to render an avatar (or initials if no image exists)
     * @config {Boolean}
     * @default true
     */
    showAvatars: {
      value: true,
      $config: 'nullify'
    },
    /**
     * Assign to toggle resource columns **fill* mode. `true` means they will stretch (grow) to fill viewport, `false`
     * that they will respect their configured `columnWidth`.
     *
     * This is ignored if *any* resources are loaded with {@link Scheduler.model.ResourceModel#field-columnWidth}.
     * @member {Boolean} fillWidth
     */
    /**
     * Automatically resize resource columns to **fill** available width. Set to `false` to always respect the
     * configured `columnWidth`.
     *
     * This is ignored if *any* resources are loaded with {@link Scheduler.model.ResourceModel#field-columnWidth}.
     * @config {Boolean}
     * @default
     */
    fillWidth: true,
    /**
     * Assign to toggle resource columns **fit* mode. `true` means they will grow or shrink to always fit viewport,
     * `false` that they will respect their configured `columnWidth`.
     *
     * This is ignored if *any* resources are loaded with {@link Scheduler.model.ResourceModel#field-columnWidth}.
     * @member {Boolean} fitWidth
     */
    /**
     * Automatically resize resource columns to always **fit** available width.
     *
     * This is ignored if *any* resources are loaded with {@link Scheduler.model.ResourceModel#field-columnWidth}.
     * @config {Boolean}
     * @default
     */
    fitWidth: false,
    /**
     * Width for each resource column.
     *
     * This is used for resources which are not are loaded with a {@link Scheduler.model.ResourceModel#field-columnWidth}.
     * @config {Number}
     */
    columnWidth: 150,
    // Copied from Scheduler#resourceImagePath on creation in TimeAxisColumn.js
    imagePath: null,
    // Copied from Scheduler#resourceImageExtension on creation in TimeAxisColumn.js
    imageExtension: null,
    // Copied from Scheduler#defaultResourceImageName on creation in TimeAxisColumn.js
    defaultImageName: null,
    availableWidth: null
  };
  /**
   * An index of the first visible resource in vertical mode
   * @property {Number}
   * @readonly
   * @private
   */
  firstResource = -1;
  /**
   * An index of the last visible resource in vertical mode
   * @property {Number}
   * @readonly
   * @private
   */
  lastResource = -1;
  //endregion
  //region Init
  construct(config) {
    const me = this;
    // Inject this into owning Scheduler early because code further down
    // can call code which uses scheduler.resourceColumns.
    config.scheduler._resourceColumns = me;
    super.construct(config);
    if (me.imagePath != null) {
      // Need to increase height a bit when displaying images
      me.element.classList.add('b-has-images');
    }
    EventHelper.on({
      element: me.element,
      delegate: '.b-resourceheader-cell',
      capture: true,
      click: 'onResourceMouseEvent',
      dblclick: 'onResourceMouseEvent',
      contextmenu: 'onResourceMouseEvent',
      thisObj: me
    });
  }
  changeShowAvatars(show) {
    var _this$avatarRendering;
    (_this$avatarRendering = this.avatarRendering) === null || _this$avatarRendering === void 0 ? void 0 : _this$avatarRendering.destroy();
    if (show) {
      this.avatarRendering = new AvatarRendering({
        element: this.element
      });
    }
    return show;
  }
  updateShowAvatars() {
    if (!this.isConfiguring) {
      this.refresh();
    }
  }
  //endregion
  //region ResourceStore
  updateResourceStore(store) {
    const me = this;
    me.detachListeners('resourceStore');
    if (store) {
      store.ion({
        name: 'resourceStore',
        changePreCommit: 'onResourceStoreDataChange',
        thisObj: me
      });
      // Already have data? Update width etc
      if (store.count) {
        me.onResourceStoreDataChange({});
      }
    }
  }
  // Redraw resource headers on any data change
  onResourceStoreDataChange({
    action
  }) {
    const me = this;
    // These must be ingested before we assess the source of column widths
    // so that they can be cleared *after* their values have been cached.
    me.getConfig('fillWidth');
    me.getConfig('fitWidth');
    me.updateWidthCache();
    const {
        element
      } = me,
      width = me.totalWidth;
    // If we have some defined columnWidths in the resourceStore
    // we must then bypass configured fitWidth and fillWidth behaviour.
    if (me.scheduler.variableColumnWidths) {
      me._fillWidth = me._fitWidth = false;
    } else {
      me._fillWidth = me.configuredFillWidth;
      me._fitWidth = me.configuredFitWidth;
    }
    if (width !== me.width) {
      DomHelper.setLength(element, 'width', width);
      // During setup, silently set the width. It will then render correctly. After setup, let the world know...
      me.column.set('width', width, me.column.grid.isConfiguring);
    }
    if (action === 'removeall') {
      // Keep nothing
      element.innerHTML = '';
    }
    if (action === 'remove' || action === 'add' || action === 'filter' || me.fitWidth || me.fillWidth) {
      me.refreshWidths();
    }
    me.column.grid.toggleEmptyText();
  }
  get totalWidth() {
    return this.updateWidthCache();
  }
  updateWidthCache() {
    let result = 0;
    const {
      scheduler
    } = this;
    // Flag so that VerticalRendering#getResourceRange knows
    // whether to use fast or slow mode to ascertain visible columns.
    scheduler.variableColumnWidths = false;
    scheduler.resourceStore.forEach(resource => {
      // Set the start position for each resource with respect to the widths
      resource.instanceMeta(scheduler).insetStart = result;
      resource.instanceMeta(scheduler).insetEnd = result + (resource.columnWidth || scheduler.resourceColumnWidth);
      if (resource.columnWidth == null) {
        result += scheduler.resourceColumnWidth;
      } else {
        result += resource.columnWidth;
        scheduler.variableColumnWidths = true;
      }
    });
    return result;
  }
  //endregion
  //region Properties
  changeColumnWidth(columnWidth) {
    // Cache configured value, because if *all* resources have their own columnWidths
    // the property will be nulled, but if we ever recieve a new resource with no
    // columnWidth, or a columnWidth is nulled, we then have to fall back to using this.
    if (!this.refreshingWidths) {
      this.configuredColumnWidth = columnWidth;
    }
    return columnWidth;
  }
  updateColumnWidth(width, oldWidth) {
    const me = this;
    // Flag set in refreshWidths, do not want to create a loop
    if (!me.refreshingWidths) {
      me.refreshWidths();
    }
    if (!me.isConfiguring) {
      // If resources are grouped, I need to refresh manually the cached width of resource header columns
      if (me.resourceStore.isGrouped) {
        me.updateWidthCache();
      }
      me.refresh();
      // Cannot trigger with requested width, might have changed because of fit/fill
      me.trigger('columnWidthChange', {
        width,
        oldWidth
      });
    }
  }
  changeFillWidth(fillWidth) {
    return this.configuredFillWidth = fillWidth;
  }
  updateFillWidth() {
    if (!this.isConfiguring) {
      this.refreshWidths();
    }
  }
  changeFitWidth(fitWidth) {
    return this.configuredFitWidth = fitWidth;
  }
  updateFitWidth() {
    if (!this.isConfiguring) {
      this.refreshWidths();
    }
  }
  getImageURL(imageName) {
    return StringHelper.joinPaths([this.imagePath || '', imageName || '']);
  }
  updateImagePath() {
    if (!this.isConfiguring) {
      this.refresh();
    }
  }
  //endregion
  //region Fit to width
  updateAvailableWidth(width) {
    this.refreshWidths();
  }
  // Updates the column widths according to fill and fit settings
  refreshWidths() {
    var _me$resourceStore;
    const me = this,
      {
        availableWidth,
        configuredColumnWidth
      } = me,
      count = (_me$resourceStore = me.resourceStore) === null || _me$resourceStore === void 0 ? void 0 : _me$resourceStore.count;
    // Bail out if availableWidth not yet set or resource store not assigned/loaded
    // or column widths are defined in the resources.
    if (!availableWidth || !count || me.scheduler.variableColumnWidths) {
      return;
    }
    me.refreshingWidths = true;
    const
      // Fit width if configured to do so or if configured to fill and used width is less than available width
      fit = me.fitWidth || me.fillWidth && configuredColumnWidth * count < availableWidth,
      useWidth = fit ? Math.floor(availableWidth / count) : configuredColumnWidth,
      shouldAnimate = me.column.grid.enableEventAnimations && Math.abs(me._columnWidth - useWidth) > 30;
    DomHelper.addTemporaryClass(me.element, 'b-animating', shouldAnimate ? 300 : 0, me);
    me.columnWidth = useWidth;
    me.refreshingWidths = false;
  }
  //endregion
  //region Rendering
  // Visual resource range, set by VerticalRendering + its buffer
  set visibleResources({
    firstResource,
    lastResource
  }) {
    this.firstResource = firstResource;
    this.lastResource = lastResource;
    this.updateWidthCache();
    this.refresh();
  }
  /**
   * Refreshes the visible headers
   */
  refresh() {
    const me = this,
      {
        firstResource,
        scheduler,
        resourceStore,
        lastResource
      } = me,
      {
        variableColumnWidths
      } = scheduler,
      groupField = resourceStore.isGrouped && resourceStore.groupers[0].field,
      configs = [];
    me.element.classList.toggle('b-grouped', Boolean(groupField));
    if (!me.column.grid.isConfiguring && firstResource > -1 && lastResource > -1 && lastResource < resourceStore.count) {
      let currentGroup;
      // Gather element configs for resource headers in view
      for (let i = firstResource; i <= lastResource; i++) {
        var _currentGroup;
        const resourceRecord = resourceStore.allResourceRecords[i],
          groupRecord = resourceRecord.instanceMeta(resourceStore).groupParent,
          groupChildren = groupRecord === null || groupRecord === void 0 ? void 0 : groupRecord.groupChildren;
        if (groupField && groupRecord.id !== ((_currentGroup = currentGroup) === null || _currentGroup === void 0 ? void 0 : _currentGroup.dataset.resourceId)) {
          const groupLeft = groupChildren[0].instanceMeta(scheduler).insetStart,
            groupWidth = groupChildren[groupChildren.length - 1].instanceMeta(scheduler).insetEnd - groupLeft;
          currentGroup = {
            className: 'b-resourceheader-group-cell',
            dataset: {
              resourceId: groupRecord.id
            },
            style: {
              left: groupLeft,
              width: groupWidth
            },
            children: [{
              tag: 'span',
              html: StringHelper.encodeHtml(groupChildren[0][groupField])
            }, {
              className: 'b-resourceheader-group-children',
              children: []
            }]
          };
          configs.push(currentGroup);
        }
        const instanceMeta = resourceRecord.instanceMeta(scheduler),
          // Possible variable column width taken from the resources, fallback to scheduler's default
          width = resourceRecord.columnWidth || me.columnWidth,
          position = groupField ? instanceMeta.insetStart - currentGroup.style.left //groupChildren[0].instanceMeta(scheduler).insetStart
          : variableColumnWidths ? instanceMeta.insetStart : i * me.columnWidth,
          elementConfig = {
            // Might look like overkill to use DomClassList here, but can be used in headerRenderer
            className: new DomClassList({
              'b-resourceheader-cell': 1
            }),
            dataset: {
              resourceId: resourceRecord.id
            },
            style: {
              [scheduler.rtl ? 'right' : 'left']: position,
              width
            },
            children: []
          };
        // Let a configured headerRenderer have a go at it before applying
        if (me.headerRenderer) {
          const value = me.headerRenderer({
            elementConfig,
            resourceRecord
          });
          if (value != null) {
            elementConfig.html = value;
          }
        }
        // No headerRenderer, apply default markup
        else {
          let imageUrl;
          if (resourceRecord.imageUrl) {
            imageUrl = resourceRecord.imageUrl;
          } else {
            if (me.imagePath != null) {
              if (resourceRecord.image !== false) {
                var _resourceRecord$name;
                const imageName = resourceRecord.image || ((_resourceRecord$name = resourceRecord.name) === null || _resourceRecord$name === void 0 ? void 0 : _resourceRecord$name.toLowerCase()) + me.imageExtension;
                imageUrl = me.getImageURL(imageName);
              }
            }
          }
          // By default showing resource name and optionally avatar
          elementConfig.children.push(me.showAvatars && me.avatarRendering.getResourceAvatar({
            resourceRecord,
            initials: resourceRecord.initials,
            color: resourceRecord.eventColor,
            iconCls: resourceRecord.iconCls,
            defaultImageUrl: me.defaultImageName && me.getImageURL(me.defaultImageName),
            imageUrl
          }), {
            tag: 'span',
            className: 'b-resource-name',
            html: StringHelper.encodeHtml(resourceRecord.name)
          });
        }
        if (groupField) {
          currentGroup.children[1].children.push(elementConfig);
        } else {
          configs.push(elementConfig);
        }
      }
    }
    // Sync changes to the header
    DomSync.sync({
      domConfig: {
        onlyChildren: true,
        children: configs
      },
      targetElement: me.element,
      syncIdField: 'resourceId'
    });
  }
  //endregion
  onResourceMouseEvent(event) {
    const resourceCell = event.target.closest('.b-resourceheader-cell'),
      resourceRecord = this.resourceStore.getById(resourceCell.dataset.resourceId);
    this.trigger('resourceHeader' + StringHelper.capitalize(event.type), {
      resourceRecord,
      event
    });
  }
  // This function is not meant to be called by any code other than Base#getCurrentConfig().
  // It extracts the current configs for the header, removing irrelevant ones
  getCurrentConfig(options) {
    const result = super.getCurrentConfig(options);
    // Assigned from Scheduler
    delete result.resourceStore;
    delete result.column;
    delete result.type;
    return result;
  }
}
ResourceHeader._$name = 'ResourceHeader';

/**
 * @module Scheduler/column/TimeAxisColumn
 */
/**
 * A column containing the timeline "viewport", in which events, dependencies etc. are drawn.
 * Normally you do not need to interact with or create this column, it is handled by Scheduler.
 *
 * If you wish to output custom contents inside the time axis row cells, you can provide your custom column configuration
 * using the {@link #config-renderer} like so:
 *
 * ```javascript
 * const scheduler = new Scheduler({
 *    appendTo         : document.body
 *    columns          : [
 *       { text : 'Name', field : 'name', width : 130 },
 *       {
 *           type : 'timeAxis',
 *           renderer({ record, cellElement }) {
 *               return '<div class="cool-chart"></div>';
 *           }
 *       }
 *    ]
 * });
 * ```
 *
 * @extends Grid/column/WidgetColumn
 * @column
 */
class TimeAxisColumn extends Events(WidgetColumn) {
  //region Config
  static $name = 'TimeAxisColumn';
  static get fields() {
    return [
    // Exclude some irrelevant fields from getCurrentConfig()
    {
      name: 'locked',
      persist: false
    }, {
      name: 'flex',
      persist: false
    }, {
      name: 'width',
      persist: false
    }, {
      name: 'cellCls',
      persist: false
    }, {
      name: 'field',
      persist: false
    }, 'mode'];
  }
  static get defaults() {
    return {
      /**
       * Set to false to prevent this column header from being dragged.
       * @config {Boolean} draggable
       * @category Interaction
       * @default false
       */
      draggable: false,
      /**
       * Set to false to prevent grouping by this column.
       * @config {Boolean} groupable
       * @category Interaction
       * @default false
       */
      groupable: false,
      /**
       * Allow column visibility to be toggled through UI.
       * @config {Boolean} hideable
       * @default false
       * @category Interaction
       */
      hideable: false,
      /**
       * Show column picker for the column.
       * @config {Boolean} showColumnPicker
       * @default false
       * @category Menu
       */
      showColumnPicker: false,
      /**
       * Allow filtering data in the column (if Filter feature is enabled)
       * @config {Boolean} filterable
       * @default false
       * @category Interaction
       */
      filterable: false,
      /**
       * Allow sorting of data in the column
       * @config {Boolean} sortable
       * @category Interaction
       * @default false
       */
      sortable: false,
      /**
       * Set to `false` to prevent the column from being drag-resized when the ColumnResize plugin is enabled.
       * @config {Boolean} resizable
       * @default false
       * @category Interaction
       */
      resizable: false,
      /**
       * Allow searching in the column (respected by QuickFind and Search features)
       * @config {Boolean} searchable
       * @default false
       * @category Interaction
       */
      searchable: false,
      /**
       * @config {String} editor
       * @hide
       */
      editor: false,
      /**
       * Set to `true` to show a context menu on the cell elements in this column
       * @config {Boolean} enableCellContextMenu
       * @default false
       * @category Menu
       */
      enableCellContextMenu: false,
      /**
       * @config {Function|Boolean} tooltipRenderer
       * @hide
       */
      tooltipRenderer: false,
      /**
       * CSS class added to the header of this column
       * @config {String} cls
       * @category Rendering
       * @default 'b-sch-timeaxiscolumn'
       */
      cls: 'b-sch-timeaxiscolumn',
      // needs to have width specified, flex-basis messes measurements up
      needWidth: true,
      mode: null,
      region: 'normal',
      exportable: false,
      htmlEncode: false
    };
  }
  static get type() {
    return 'timeAxis';
  }
  //region Init
  construct(config) {
    const me = this;
    super.construct(...arguments);
    me.thisObj = me;
    me.timeAxisViewModel = me.grid.timeAxisViewModel;
    // A bit hacky, because mode is a field and not a config
    // eslint-disable-next-line no-self-assign
    me.mode = me.mode;
    me.grid.ion({
      paint: 'onTimelinePaint',
      thisObj: me,
      once: true
    });
  }
  static get autoExposeFields() {
    return true;
  }
  // endregion
  doDestroy() {
    var _this$resourceColumns, _this$timeAxisView;
    (_this$resourceColumns = this.resourceColumns) === null || _this$resourceColumns === void 0 ? void 0 : _this$resourceColumns.destroy();
    (_this$timeAxisView = this.timeAxisView) === null || _this$timeAxisView === void 0 ? void 0 : _this$timeAxisView.destroy();
    super.doDestroy();
  }
  set mode(mode) {
    const me = this,
      {
        grid
      } = me;
    me.set('mode', mode);
    // In horizontal mode this column has a time axis header on top, with timeline ticks
    if (mode === 'horizontal') {
      me.timeAxisView = new HorizontalTimeAxis({
        model: me.timeAxisViewModel,
        compactCellWidthThreshold: me.compactCellWidthThreshold,
        owner: grid,
        client: grid
      });
    }
    // In vertical mode, it instead displays resources at top
    else if (mode === 'vertical') {
      me.resourceColumns = ResourceHeader.new({
        column: me,
        scheduler: grid,
        resourceStore: grid.resourceStore,
        imagePath: grid.resourceImagePath,
        imageExtension: grid.resourceImageExtension,
        defaultImageName: grid.defaultResourceImageName
      }, grid.resourceColumns || {});
      me.relayEvents(me.resourceColumns, ['resourceheaderclick', 'resourceheaderdblclick', 'resourceheadercontextmenu']);
    }
  }
  get mode() {
    return this.get('mode');
  }
  //region Events
  onViewModelUpdate({
    source: viewModel
  }) {
    const me = this;
    if (me.grid.timeAxisSubGrid.collapsed) {
      return;
    }
    if (me.mode === 'horizontal') {
      // render the time axis view into the column header element
      me.refreshHeader(true);
      me.width = viewModel.totalSize;
      me.grid.refresh();
      // When width is set above, that ends up on a columnsResized listener, but the refreshing of the fake
      // scrollers to accommodate the new width is not done in this timeframe, so the upcoming centering related
      // to preset change cannot work. So we have to refresh the fake scrollers now
      me.subGrid.refreshFakeScroll();
    } else if (me.mode === 'vertical') {
      // Refresh to rerender cells, in the process updating the vertical timeaxis to reflect view model changes
      me.grid.refreshRows();
    }
  }
  // Called on paint. SubGrid has its width so this is the earliest time to configure the TimeAxisViewModel with
  // correct width
  onTimelinePaint({
    firstPaint
  }) {
    const me = this;
    if (!me.subGrid.insertRowsBefore) {
      return;
    }
    if (firstPaint) {
      me.subGridElement.classList.add('b-timeline-subgrid');
      if (me.mode === 'vertical') {
        var _me$grid;
        me.refreshHeader();
        // The above operation can cause height change.
        (_me$grid = me.grid) === null || _me$grid === void 0 ? void 0 : _me$grid.onHeightChange();
      }
    }
  }
  //endregion
  //region Rendering
  /**
   * Refreshes the columns header contents (which is either a HorizontalTimeAxis or a ResourceHeader). Useful if you
   * have rendered some extra meta data that depends on external data such as the EventStore or ResourceStore.
   */
  refreshHeader(internal) {
    const me = this,
      {
        element
      } = me;
    if (element) {
      if (me.mode === 'horizontal') {
        // Force timeAxisViewModel to regenerate its column config, which calls header renderers etc.
        !internal && me.timeAxisViewModel.update(undefined, undefined, true);
        if (!me.timeAxisView.rendered) {
          // Do not need the normal header markup
          element.innerHTML = '';
          me.timeAxisView.render(element);
        } else {
          // Force rebuild of cells in case external data has changed (cheap since it still syncs to DOM)
          me.timeAxisView.refresh(true);
        }
      } else if (me.mode === 'vertical') {
        if (!me.resourceColumns.currentElement) {
          // Do not need the normal header markup
          element.innerHTML = '';
          me.resourceColumns.render(element);
        }
        // Vertical's resourceColumns is redrawn with the events, no need here
      }
    }
  }

  internalRenderer(renderData) {
    const {
      grid
    } = this;
    // No drawing of events before engines initial commit
    if (grid.project.isInitialCommitPerformed || grid.project.isDelayingCalculation) {
      grid.currentOrientation.renderer(renderData);
      return super.internalRenderer(renderData);
    }
  }
  //endregion
  get timeAxisViewModel() {
    return this._timeAxisViewModel;
  }
  set timeAxisViewModel(timeAxisViewModel) {
    const me = this;
    me.detachListeners('tavm');
    timeAxisViewModel === null || timeAxisViewModel === void 0 ? void 0 : timeAxisViewModel.ion({
      name: 'tavm',
      update: 'onViewModelUpdate',
      prio: -10000,
      thisObj: me
    });
    me._timeAxisViewModel = timeAxisViewModel;
    if (me.timeAxisView) {
      me.timeAxisView.model = timeAxisViewModel;
    }
  }
  // Width of the time axis column is solely determined by the zoom level. We should not keep it part of the state
  // otherwise restoring the state might break the normal zooming process.
  // Covered by SchedulerState.t
  // https://github.com/bryntum/support/issues/5545
  getState() {
    const state = super.getState();
    delete state.width;
    delete state.flex;
    return state;
  }
}
ColumnStore.registerColumnType(TimeAxisColumn);
TimeAxisColumn._$name = 'TimeAxisColumn';

/**
 * @module Scheduler/preset/ViewPreset
 */
/**
 * An object containing a unit identifier and an increment variable, used to define the `timeResolution` of a
 * `ViewPreset`.
 * @typedef {Object} ViewPresetTimeResolution
 * @property {String} unit The unit of the resolution, e.g. 'minute'
 * @property {Number} increment The increment of the resolution, e.g. 15
 */
/**
 * Defines a header level for a ViewPreset.
 *
 * A sample header configuration can look like below
 * ```javascript
 * headers    : {
 *     {
 *         unit        : "month",
 *         renderer : function(start, end, headerConfig, index) {
 *             var month = start.getMonth();
 *             // Simple alternating month in bold
 *             if (start.getMonth() % 2) {
 *                 return '<strong>' + month + '</strong>';
 *             }
 *             return month;
 *         },
 *         align       : 'start' // `start` or `end`, omit to center content (default)
 *     },
 *     {
 *         unit        : "week",
 *         increment   : 1,
 *         renderer    : function(start, end, headerConfig, index) {
 *             return 'foo';
 *         }
 *     },
 * }
 * ```
 *
 * @typedef {Object} ViewPresetHeaderRow
 * @property {'start'|'center'|'end'} align The text alignment for the cell. Valid values are `start` or `end`, omit
 * this to center text content (default). Can also be added programmatically in `the renderer`.
 * @property {'millisecond'|'second'|'minute'|'hour'|'day'|'week'|'month'|'quarter'|'year'} unit The unit of time
 * represented by each cell in this header row. See also increment property. Valid values are "millisecond", "second",
 * "minute", "hour", "day", "week", "month", "quarter", "year".
 * @property {String} headerCellCls A CSS class to add to the cells in the time axis header row. Can also be added
 * programmatically in the `renderer`.
 * @property {Number} increment The number of units each header cell will represent (e.g. 30 together with unit:
 * "minute" for 30 minute cells)
 * @property {String} dateFormat Defines how the cell date will be formatted
 * @property {Function} renderer A custom renderer function used to render the cell content. It should return text/HTML
 * to put in the header cell.
 *
 * ```javascript
 * function (startDate, endDate, headerConfig, i) {
 *   // applies special CSS class to align header left
 *   headerConfig.align = "start";
 *   // will be added as a CSS class of the header cell DOM element
 *   headerConfig.headerCellCls = "myClass";
 *
 *   return DateHelper.format(startDate, 'YYYY-MM-DD');
 * }
 * ```
 *
 * The render function is called with the following parameters:
 *
 * @property {Date} renderer.startDate The start date of the cell.
 * @property {Date} renderer.endDate The end date of the cell.
 * @property {Object} renderer.headerConfig An object containing the header config.
 * @property {'start'|'center'|'end'} [renderer.headerConfig.align] The text alignment for the cell. See `align` above.
 * @property {String} [renderer.headerConfig.headerCellCls] A CSS class to add to the cells in the time axis header row.
 * See `headerCellCls` above.
 * @property {Number} renderer.index The index of the cell in the row.
 * @property {Object} thisObj `this` reference for the renderer function
 * @property {Function} cellGenerator A function that should return an array of objects containing 'start', 'end' and
 * 'header' properties. Use this if you want full control over how the header rows are generated.
 *
 * **Note:** `cellGenerator` cannot be used for the bottom level of your headers.
 *
 * Example :
 * ```javascript
 * viewPreset : {
 *     displayDateFormat : 'H:mm',
 *     shiftIncrement    : 1,
 *     shiftUnit         : 'WEEK',
 *     timeResolution    : {
 *         unit      : 'MINUTE',
 *         increment : 10
 *     },
 *     headers           : [
 *         {
 *             unit          : 'year',
 *             // Simplified scenario, assuming view will always just show one US fiscal year
 *             cellGenerator : (viewStart, viewEnd) => [{
 *                 start  : viewStart,
 *                 end    : viewEnd,
 *                 header : `Fiscal Year ${viewStart.getFullYear() + 1}`
 *             }]
 *         },
 *         {
 *             unit : 'quarter',
 *             renderer(start, end, cfg) {
 *                 const
 *                     quarter       = Math.floor(start.getMonth() / 3) + 1,
 *                     fiscalQuarter = quarter === 4 ? 1 : (quarter + 1);
 *
 *                 return `FQ${fiscalQuarter} ${start.getFullYear() + (fiscalQuarter === 1 ? 1 : 0)}`;
 *             }
 *         },
 *         {
 *             unit       : 'month',
 *             dateFormat : 'MMM Y'
 *         }
 *     ]
 *  },
 * ```
 */
/**
 * A ViewPreset is a record of {@link Scheduler.preset.PresetStore PresetStore} which describes the granularity
 * of the timeline view of a {@link Scheduler.view.Scheduler Scheduler} and the layout and subdivisions of the timeline header.
 *
 * You can create a new instance by specifying all fields:
 *
 * ```javascript
 * const myViewPreset = new ViewPreset({
 *     id   : 'myPreset',              // Unique id value provided to recognize your view preset. Not required, but having it you can simply set new view preset by id: scheduler.viewPreset = 'myPreset'
 *
 *     name : 'My view preset',        // A human-readable name provided to be used in GUI, e.i. preset picker, etc.
 *
 *     tickWidth  : 24,                // Time column width in horizontal mode
 *     tickHeight : 50,                // Time column height in vertical mode
 *     displayDateFormat : 'HH:mm',    // Controls how dates will be displayed in tooltips etc
 *
 *     shiftIncrement : 1,             // Controls how much time to skip when calling shiftNext and shiftPrevious.
 *     shiftUnit      : 'day',         // Valid values are 'millisecond', 'second', 'minute', 'hour', 'day', 'week', 'month', 'quarter', 'year'.
 *     defaultSpan    : 12,            // By default, if no end date is supplied to a view it will show 12 hours
 *
 *     timeResolution : {              // Dates will be snapped to this resolution
 *         unit      : 'minute',       // Valid values are 'millisecond', 'second', 'minute', 'hour', 'day', 'week', 'month', 'quarter', 'year'.
 *         increment : 15
 *     },
 *
 *     headers : [                     // This defines your header rows from top to bottom
 *         {                           // For each row you can define 'unit', 'increment', 'dateFormat', 'renderer', 'align', and 'thisObj'
 *             unit       : 'day',
 *             dateFormat : 'ddd DD/MM'
 *         },
 *         {
 *             unit       : 'hour',
 *             dateFormat : 'HH:mm'
 *         }
 *     ],
 *
 *     columnLinesFor : 1              // Defines header level column lines will be drawn for. Defaults to the last level.
 * });
 * ```
 *
 * Or you can extend one of view presets registered in {@link Scheduler.preset.PresetManager PresetManager}:
 *
 * ```javascript
 * const myViewPreset2 = new ViewPreset({
 *     id   : 'myPreset',                  // Unique id value provided to recognize your view preset. Not required, but having it you can simply set new view preset by id: scheduler.viewPreset = 'myPreset'
 *     name : 'My view preset',            // A human-readable name provided to be used in GUI, e.i. preset picker, etc.
 *     base : 'hourAndDay',                // Extends 'hourAndDay' view preset provided by PresetManager. You can pick out any of PresetManager's view presets: PresetManager.records
 *
 *     timeResolution : {                  // Override time resolution
 *         unit      : 'minute',
 *         increment : 15                  // Make it increment every 15 mins
 *     },
 *
 *     headers : [                         // Override headers
 *         {
 *             unit       : 'day',
 *             dateFormat : 'DD.MM.YYYY'   // Use different date format for top header 01.10.2020
 *         },
 *         {
 *             unit       : 'hour',
 *             dateFormat : 'LT'
 *         }
 *     ]
 * });
 * ```
 *
 * See {@link Scheduler.preset.PresetManager PresetManager} for the list of base presets. You may add your own
 * presets to this global list:
 *
 * ```javascript
 * PresetManager.add(myViewPreset);     // Adds new preset to the global scope. All newly created scheduler instances will have it too.
 *
 * const scheduler = new Scheduler({
 *     viewPreset : 'myPreset'
 *     // other configs...
 * });
 * ```
 *
 * Or add them on an individual basis to Scheduler instances:
 *
 * ```javascript
 * const scheduler = new Scheduler({...});
 *
 * scheduler.presets.add(myViewPreset); // Adds new preset to the scheduler instance only. All newly created scheduler instances will **not** have it.
 *
 * scheduler.viewPreset = 'myPreset';
 * ```
 *
 * ## Defining custom header rows
 *
 * You can have any number of header rows by specifying {@link #field-headers}, see {@link #typedef-ViewPresetHeaderRow}
 * for the config object format and {@link Core.helper.DateHelper} for the supported date formats, or use to render
 * custom contents into the row cells.
 *
 * ```javascript
 *  headers : [
 *      {
 *          unit       : 'month',
 *          dateFormat : 'MM.YYYY'
 *      },
 *      {
 *          unit       : 'week',
 *          renderer   : ({ startDate }) => `Week ${DateHelper.format(startDate, 'WW')}`
 *      }
 *  ]
 * ```
 *
 * {@inlineexample Scheduler/preset/CustomHeader.js}
 *
 * This live demo shows a custom ViewPreset with AM/PM time format:
 * @inlineexample Scheduler/preset/amPmPreset.js
 * @extends Core/data/Model
 */
class ViewPreset extends Model {
  static $name = 'ViewPreset';
  static get fields() {
    return [
    /**
     * The name of an existing view preset to extend
     * @field {String} base
     */
    {
      name: 'base',
      type: 'string'
    },
    /**
     * The name of the view preset
     * @field {String} name
     */
    {
      name: 'name',
      type: 'string'
    },
    /**
     * The height of the row in horizontal orientation
     * @field {Number} rowHeight
     * @default
     */
    {
      name: 'rowHeight',
      defaultValue: 24
    },
    /**
     * The width of the time tick column in horizontal orientation
     * @field {Number} tickWidth
     * @default
     */
    {
      name: 'tickWidth',
      defaultValue: 50
    },
    /**
     * The height of the time tick column in vertical orientation
     * @field {Number} tickHeight
     * @default
     */
    {
      name: 'tickHeight',
      defaultValue: 50
    },
    /**
     * Defines how dates will be formatted in tooltips etc
     * @field {String} displayDateFormat
     * @default
     */
    {
      name: 'displayDateFormat',
      defaultValue: 'HH:mm'
    },
    /**
     * The unit to shift when calling shiftNext/shiftPrevious to navigate in the chart.
     * Valid values are "millisecond", "second", "minute", "hour", "day", "week", "month", "quarter", "year".
     * @field {'millisecond'|'second'|'minute'|'hour'|'day'|'week'|'month'|'quarter'|'year'} shiftUnit
     * @default
     */
    {
      name: 'shiftUnit',
      defaultValue: 'hour'
    },
    /**
     * The amount to shift (in shiftUnits)
     * @field {Number} shiftIncrement
     * @default
     */
    {
      name: 'shiftIncrement',
      defaultValue: 1
    },
    /**
     * The amount of time to show by default in a view (in the unit defined by {@link #field-mainUnit})
     * @field {Number} defaultSpan
     * @default
     */
    {
      name: 'defaultSpan',
      defaultValue: 12
    },
    /**
     * Initially set to a unit. Defaults to the unit defined by the middle header.
     * @field {'millisecond'|'second'|'minute'|'hour'|'day'|'week'|'month'|'quarter'|'year'} mainUnit
     */
    {
      name: 'mainUnit'
    },
    /**
     * Note: Currently, this field only applies when changing viewPreset with the {@link Scheduler.widget.ViewPresetCombo}.
     *
     * Set to a number and that amount of {@link #field-mainUnit} will be added to the startDate. For example: A
     * start value of `5` together with the mainUnit `hours` will add 5 hours to the startDate. This can achieve
     * a "day view" that starts 5 AM.
     *
     * Set to a string unit (for example week, day, month) and the startDate will be the start of that unit
     * calculated from current startDate. A start value of `week` will result in a startDate in the first day of
     * the week.
     *
     * If set to a number or not set at all, the startDate will be calculated at the beginning of current
     * mainUnit.
     * @field {Number|String} start
     */
    {
      name: 'start'
    },
    /**
     * An object containing a unit identifier and an increment variable. This value means minimal task duration
     * you can create using UI. For example when you drag create a task or drag & drop a task, if increment is 5
     * and unit is 'minute' that means that you can create a 5-minute-long task, or move it 5 min
     * forward/backward. This config maps to scheduler's
     * {@link Scheduler.view.mixin.TimelineDateMapper#property-timeResolution} config.
     *
     * ```javascript
     * timeResolution : {
     *   unit      : 'minute',  //Valid values are "millisecond", "second", "minute", "hour", "day", "week", "month", "quarter", "year".
     *   increment : 5
     * }
     * ```
     *
     * @field {ViewPresetTimeResolution} timeResolution
     */
    'timeResolution',
    /**
     * An array containing one or more {@link #typedef-ViewPresetHeaderRow} config objects, each of
     * which defines a level of headers for the scheduler.
     * The `main` unit will be the last header's unit, but this can be changed using the
     * {@link #field-mainHeaderLevel} field.
     * @field {ViewPresetHeaderRow[]} headers
     */
    'headers',
    /**
     * Index of the {@link #field-headers} array to define which header level is the `main` header.
     * Defaults to the bottom header.
     * @field {Number} mainHeaderLevel
     */
    'mainHeaderLevel',
    /**
     * Index of a header level in the {@link #field-headers} array for which column lines are drawn. See
     * {@link Scheduler.feature.ColumnLines}.
     * Defaults to the bottom header.
     * @field {Number} columnLinesFor
     */
    'columnLinesFor'];
  }
  construct() {
    super.construct(...arguments);
    this.normalizeUnits();
  }
  generateId(owner) {
    const me = this,
      {
        headers
      } = me,
      parts = [];
    // If we were subclassed from a base, use that id as the basis of our.
    let result = Object.getPrototypeOf(me.data).id;
    if (!result) {
      for (let {
          length
        } = headers, i = length - 1; i >= 0; i--) {
        const {
            unit,
            increment
          } = headers[i],
          multiple = increment > 1;
        parts.push(`${multiple ? increment : ''}${i ? unit : StringHelper.capitalize(unit)}${multiple ? 's' : ''}`);
      }
      // Use upwards header units so eg "monthAndYear"
      result = parts.join('And');
    }
    // If duplicate, decorate the generated by adding details.
    // For example make it "hourAndDay-50by80"
    // Only interrogate the store if it is loaded. If consulted during
    // a load, the idMap will be created empty
    if (owner.count && owner.includes(result)) {
      result += `-${me.tickWidth}by${me.tickHeight || me.tickWidth}`;
      // If still duplicate use increment
      if (owner.includes(result)) {
        result += `-${me.bottomHeader.increment}`;
        // And if STILL duplicate, make it unique with a suffix
        if (owner.includes(result)) {
          result = IdHelper.generateId(`${result}-`);
        }
      }
    }
    return result;
  }
  normalizeUnits() {
    const me = this,
      {
        timeResolution,
        headers,
        shiftUnit
      } = me;
    if (headers) {
      // Make sure date "unit" constant specified in the preset are resolved
      for (let i = 0, {
          length
        } = headers; i < length; i++) {
        const header = headers[i];
        header.unit = DateHelper.normalizeUnit(header.unit);
        if (header.splitUnit) {
          header.splitUnit = DateHelper.normalizeUnit(header.splitUnit);
        }
        if (!('increment' in header)) {
          headers[i] = Object.assign({
            increment: 1
          }, header);
        }
      }
    }
    if (timeResolution) {
      timeResolution.unit = DateHelper.normalizeUnit(timeResolution.unit);
    }
    if (shiftUnit) {
      me.shiftUnit = DateHelper.normalizeUnit(shiftUnit);
    }
  }
  // Process legacy columnLines config into a headers array.
  static normalizeHeaderConfig(data) {
    const {
        headerConfig,
        columnLinesFor,
        mainHeaderLevel
      } = data,
      headers = data.headers = [];
    if (headerConfig.top) {
      if (columnLinesFor === 'top') {
        data.columnLinesFor = 0;
      }
      if (mainHeaderLevel === 'top') {
        data.mainHeaderLevel = 0;
      }
      headers[0] = headerConfig.top;
    }
    if (headerConfig.middle) {
      if (columnLinesFor === 'middle') {
        data.columnLinesFor = headers.length;
      }
      if (mainHeaderLevel === 'middle') {
        data.mainHeaderLevel = headers.length;
      }
      headers.push(headerConfig.middle);
    } else {
      throw new Error('ViewPreset.headerConfig must be configured with a middle');
    }
    if (headerConfig.bottom) {
      // Main level is middle when using headerConfig object.
      data.mainHeaderLevel = headers.length - 1;
      // There *must* be a middle above this bottom header
      // so that is the columnLines one by default.
      if (columnLinesFor == null) {
        data.columnLinesFor = headers.length - 1;
      } else if (columnLinesFor === 'bottom') {
        data.columnLinesFor = headers.length;
      }
      // There *must* be a middle above this bottom header
      // so that is the main one by default.
      if (mainHeaderLevel == null) {
        data.mainHeaderLevel = headers.length - 1;
      }
      if (mainHeaderLevel === 'bottom') {
        data.mainHeaderLevel = headers.length;
      }
      headers.push(headerConfig.bottom);
    }
  }
  // These are read-only once configured.
  set() {}
  inSet() {}
  get columnLinesFor() {
    return 'columnLinesFor' in this.data ? this.data.columnLinesFor : this.headers.length - 1;
  }
  get tickSize() {
    return this._tickSize || this.tickWidth;
  }
  get tickWidth() {
    return 'tickWidth' in this.data ? this.data.tickWidth : 50;
  }
  get tickHeight() {
    return 'tickHeight' in this.data ? this.data.tickHeight : 50;
  }
  get headerConfig() {
    // Configured in the legacy manner, just return the configured value.
    if (this.data.headerConfig) {
      return this.data.headerConfig;
    }
    // Rebuild the object based upon the configured headers array.
    const result = {},
      {
        headers
      } = this,
      {
        length
      } = headers;
    switch (length) {
      case 1:
        result.middle = headers[0];
        break;
      case 2:
        if (this.mainHeaderLevel === 0) {
          result.middle = headers[0];
          result.bottom = headers[1];
        } else {
          result.top = headers[0];
          result.middle = headers[1];
        }
        break;
      case 3:
        result.top = headers[0];
        result.middle = headers[1];
        result.bottom = headers[2];
        break;
      default:
        throw new Error('headerConfig object not supported for >3 header levels');
    }
    return result;
  }
  set mainHeaderLevel(mainHeaderLevel) {
    this.data.mainHeaderLevel = mainHeaderLevel;
  }
  get mainHeaderLevel() {
    if ('mainHeaderLevel' in this.data) {
      return this.data.mainHeaderLevel;
    }
    // 3 headers, then it's the middle
    if (this.data.headers.length === 3) {
      return 1;
    }
    // Assume it goes top, middle.
    // If it's middle, top, use mainHeaderLevel : 0
    return this.headers.length - 1;
  }
  get mainHeader() {
    return this.headers[this.mainHeaderLevel];
  }
  get topHeader() {
    return this.headers[0];
  }
  get topUnit() {
    return this.topHeader.unit;
  }
  get topIncrement() {
    return this.topHeader.increment;
  }
  get bottomHeader() {
    return this.headers[this.headers.length - 1];
  }
  get leafUnit() {
    return this.bottomHeader.unit;
  }
  get leafIncrement() {
    return this.bottomHeader.increment;
  }
  get mainUnit() {
    if ('mainUnit' in this.data) {
      return this.data.mainUnit;
    }
    return this.mainHeader.unit;
  }
  get msPerPixel() {
    const {
      bottomHeader
    } = this;
    return Math.round(DateHelper.asMilliseconds(bottomHeader.increment || 1, bottomHeader.unit) / this.tickWidth);
  }
  get isValid() {
    const me = this;
    let valid = true;
    // Make sure all date "unit" constants are valid
    for (const header of me.headers) {
      valid = valid && Boolean(DateHelper.normalizeUnit(header.unit));
    }
    if (me.timeResolution) {
      valid = valid && DateHelper.normalizeUnit(me.timeResolution.unit);
    }
    if (me.shiftUnit) {
      valid = valid && DateHelper.normalizeUnit(me.shiftUnit);
    }
    return valid;
  }
}
ViewPreset._$name = 'ViewPreset';

/**
 * @module Scheduler/preset/PresetStore
 */
/**
 * A special Store subclass which holds {@link Scheduler.preset.ViewPreset ViewPresets}.
 * Each ViewPreset in this store represents a zoom level. The store data is sorted in special
 * zoom order. That is zoomed out to zoomed in. The first Preset will produce the narrowest event bars
 * the last one will produce the widest event bars.
 *
 * To specify view presets (zoom levels) please provide set of view presets to the scheduler:
 *
 * ```javascript
 * const myScheduler = new Scheduler({
 *     presets : [
 *         {
 *             base : 'hourAndDay',
 *             id   : 'MyHourAndDay',
 *             // other preset configs....
 *         },
 *         {
 *             base : 'weekAndMonth',
 *             id   : 'MyWeekAndMonth',
 *             // other preset configs....
 *         }
 *     ],
 *     viewPreset : 'MyHourAndDay',
 *     // other scheduler configs....
 *     });
 * ```
 *
 * @extends Core/data/Store
 */
class PresetStore extends Localizable(Store) {
  static get $name() {
    return 'PresetStore';
  }
  static get defaultConfig() {
    return {
      useRawData: true,
      modelClass: ViewPreset,
      /**
       * Specifies the sort order of the presets in the store.
       * By default they are in zoomed out to zoomed in order. That is
       * presets which will create widest event bars to presets
       * which will produce narrowest event bars.
       *
       * Configure this as `-1` to reverse this order.
       * @config {Number}
       * @default
       */
      zoomOrder: 1
    };
  }
  set storage(storage) {
    super.storage = storage;
    // Maintained in order automatically while adding.
    this.storage.addSorter((lhs, rhs) => {
      const leftBottomHeader = lhs.bottomHeader,
        rightBottomHeader = rhs.bottomHeader;
      // Sort order:
      //  Milliseconds per pixel.
      //  Tick size.
      //  Unit magnitude.
      //  Increment size.
      const order = rhs.msPerPixel - lhs.msPerPixel || unitMagnitudes[leftBottomHeader.unit] - unitMagnitudes[rightBottomHeader.unit] || leftBottomHeader.increment - rightBottomHeader.increment;
      return order * this.zoomOrder;
    });
  }
  get storage() {
    return super.storage;
  }
  getById(id) {
    // If we do not know about the id, inherit it from the PresetManager singleton
    return super.getById(id) || !this.isPresetManager && pm.getById(id);
  }
  createRecord(data, ...args) {
    let result;
    if (data.isViewPreset) {
      return data;
    }
    if (typeof data === 'string') {
      result = this.getById(data);
    } else if (typeof data === 'number') {
      result = this.getAt(data);
    }
    // Its a ViewPreset data object
    else {
      // If it's extending an existing ViewPreset, inherit then override
      // the data from the base.
      if (data.base) {
        data = this.copyBaseValues(data);
      }
      // Model constructor will call generateId if no id is provided
      return super.createRecord(data, ...args);
    }
    if (!result) {
      throw new Error(`ViewPreset ${data} does not exist`);
    }
    return result;
  }
  updateLocalization() {
    super.updateLocalization();
    const me = this;
    // Collect presets from store...
    let presets = me.allRecords;
    // and basePresets if we are the PresetManager
    if (me.isPresetManager) {
      presets = new Set(presets.concat(Object.values(me.basePresets)));
    }
    presets.forEach(preset => {
      let localePreset = me.optionalL(`L{PresetManager.${preset.id}}`, null, true);
      // Default presets generated from base presets use localization of base if they have no own
      if (typeof localePreset === 'string' && preset.baseId) {
        localePreset = me.optionalL(`L{PresetManager.${preset.baseId}}`, null, true);
      }
      // Apply any custom format defined in locale, or the original format if none exists
      if (localePreset && typeof localePreset === 'object') {
        if (!preset.originalDisplayDateFormat) {
          preset.originalDisplayDateFormat = preset.displayDateFormat;
        }
        // it there is a topDateFormat but preset.mainHeaderLevel is 0, means the middle header is the top header actually,
        // so convert property to middle (if middle doesn't exists) to localization understand (topDateFormat for weekAndDay for example)
        // topDateFormat doesn't work when mainHeaderLevel is 0 because it doesn't have top config
        // but has top header visually (Check on get headerConfig method in ViewPreset class)
        if (preset.mainHeaderLevel === 0 && localePreset.topDateFormat) {
          localePreset.middleDateFormat = localePreset.middleDateFormat || localePreset.topDateFormat;
        }
        preset.setData('displayDateFormat', localePreset.displayDateFormat || preset.originalDisplayDateFormat);
        ['top', 'middle', 'bottom'].forEach(level => {
          const levelConfig = preset.headerConfig[level],
            localeLevelDateFormat = localePreset[level + 'DateFormat'];
          if (levelConfig) {
            if (!levelConfig.originalDateFormat) {
              levelConfig.originalDateFormat = levelConfig.dateFormat;
            }
            // if there was defined topDateFormat on locale file for example, use it instead of renderer from basePresets (https://github.com/bryntum/support/issues/1307)
            if (localeLevelDateFormat && levelConfig.renderer) {
              levelConfig.renderer = null;
            }
            levelConfig.dateFormat = localeLevelDateFormat || levelConfig.originalDateFormat;
          }
        });
        // The preset names are used in ViewPresetCombo and are localized by default
        if (localePreset.name) {
          if (!preset.unlocalizedName) {
            preset.unlocalizedName = preset.name;
          }
          preset.setData('name', localePreset.name);
        } else if (preset.unlocalizedName && preset.unlocalizedName !== preset.name) {
          preset.name = preset.unlocalizedName;
          preset.unlocalizedName = null;
        }
      }
    });
  }
  // This function is not meant to be called by any code other than Base#getCurrentConfig().
  // Preset config on Scheduler/Gantt expects array of presets and not store config
  getCurrentConfig(options) {
    return super.getCurrentConfig(options).data;
  }
  copyBaseValues(presetData) {
    let base = this.getById(presetData.base);
    if (!base) {
      throw new Error(`ViewPreset base '${presetData.base}' does not exist.`);
    }
    base = ObjectHelper.clone(base.data);
    delete base.id;
    delete base.name;
    // Merge supplied data into a clone of the base ViewPreset's data
    // so that the new one overrides the base.
    return ObjectHelper.merge(base, presetData);
  }
  add(preset) {
    preset = Array.isArray(preset) ? preset : [preset];
    preset.forEach(preset => {
      // If a ViewPreset instance that extends another preset was added
      // Only in add we can apply the base data
      if (preset.isViewPreset && preset.base) {
        preset.data = this.copyBaseValues(preset.originalData);
      }
    });
    return super.add(...arguments);
  }
}
PresetStore._$name = 'PresetStore';

// No module tag here. That stops the singleton from being included by the docs.
/**
 * ## Intro
 * This is a global Store of {@link Scheduler.preset.ViewPreset ViewPresets}, required to supply initial data to
 * Scheduler's {@link Scheduler.view.mixin.TimelineViewPresets#config-presets}.
 *
 * You can provide new view presets globally or for a specific scheduler.
 *
 * **NOTE:** You **cannot** modify existing records in the PresetManager store. You can either remove
 * preset records from the store or add new records to the store.
 * Also please keep in mind, all changes provided to the PresetManager store are not reflected to the
 * {@link Scheduler.view.mixin.TimelineViewPresets#config-presets} of schedulers that already exist!
 *
 * If you want to have just a few presets (also known as zoom levels) in your Scheduler, you can slice corresponding records
 * from the `PresetManager` and apply them to the Scheduler `presets` config.
 * ```javascript
 * const newPresets = PresetManager.records.slice(10, 12);
 *
 * const scheduler = new Scheduler({
 *     presets    : newPresets, // Only 2 zoom levels are available
 *     viewPreset : newPresets[0].id
 * });
 * ```
 *
 * If you want to adjust all default presets and assign to a specific scheduler you are going to create,
 * you can extend them and pass as an array to the Scheduler `presets` config.
 * Here is an example of how to set the same `timeResolution` to all `ViewPresets`.
 * ```javascript
 * const newPresets = PresetManager.map(preset => {
 *     return {
 *         id             : 'my_' + preset.id,
 *         base           : preset.id, // Based on an existing preset
 *         timeResolution : {
 *             unit      : 'day',
 *             increment : 1
 *         }
 *     };
 * });
 *
 * const scheduler = new Scheduler({
 *     presets     : newPresets,
 *     viewPreset : 'my_hourAndDay'
 * });
 * ```
 *
 * If you want to do the same for **all** schedulers which will be created next, you can register new presets in a loop.
 * ```javascript
 * PresetManager.records.forEach(preset => {
 *     // Pass the same ID, so when a new preset is added to the store,
 *     // it will replace the current one.
 *     PresetManager.registerPreset(preset.id, {
 *        id             : preset.id,
 *        base           : preset.id,
 *        timeResolution : {
 *            unit      : 'day',
 *            increment : 1
 *        }
 *    });
 * });
 * ```
 *
 * Here is an example of how to add a new `ViewPreset` to the global `PresetManager` store and to the already created
 * scheduler `presets`.
 * ```javascript
 * const scheduler = new Scheduler({...});
 *
 * const newGlobalPresets = PresetManager.add({
 *     id              : 'myNewPreset',
 *     base            : 'hourAndDay', // Based on an existing preset
 *     columnLinesFor  : 0,
 *     // Override headers
 *     headers : [
 *         {
 *             unit       : 'day',
 *             // Use different date format for top header 01.10.2020
 *             dateFormat : 'DD.MM.YYYY'
 *         },
 *         {
 *             unit       : 'hour',
 *             dateFormat : 'LT'
 *         }
 *     ]
 * });
 *
 * // Add new presets to the scheduler that has been created before changes
 * // to PresetManager are applied
 * scheduler.presets.add(newGlobalPresets);
 * ```
 *
 * ## Predefined presets
 *
 * Predefined presets are:
 *
 * - `secondAndMinute` - creates a 2 level header - minutes and seconds:
 * {@inlineexample Scheduler/preset/secondAndMinute.js}
 * - `minuteAndHour` - creates a 2 level header - hours and minutes:
 * {@inlineexample Scheduler/preset/minuteAndHour.js}
 * - `hourAndDay` - creates a 2 level header - days and hours:
 * {@inlineexample Scheduler/preset/hourAndDay.js}
 * - `dayAndWeek` - creates a 2 level header - weeks and days:
 * {@inlineexample Scheduler/preset/dayAndWeek.js}
 * - `dayAndMonth` - creates a 2 level header - months and days:
 * {@inlineexample Scheduler/preset/dayAndMonth.js}
 * - `weekAndDay` - just like `dayAndWeek` but with different formatting:
 * {@inlineexample Scheduler/preset/weekAndDay.js}
 * - `weekAndDayLetter` - creates a 2 level header - weeks and day letters:
 * {@inlineexample Scheduler/preset/weekAndDayLetter.js}
 * - `weekAndMonth` - creates a 2 level header - months and weeks:
 * {@inlineexample Scheduler/preset/weekAndMonth.js}
 * - `weekDateAndMonth` - creates a 2 level header - months and weeks (weeks shown by first day only):
 * {@inlineexample Scheduler/preset/weekDateAndMonth.js}
 * - `monthAndYear` - creates a 2 level header - years and months:
 * {@inlineexample Scheduler/preset/monthAndYear.js}
 * - `year` - creates a 2 level header - years and quarters:
 * {@inlineexample Scheduler/preset/year.js}
 * - `manyYears` - creates a 2 level header - 5-years and years:
 * {@inlineexample Scheduler/preset/manyYears.js}
 *
 * See the {@link Scheduler.preset.ViewPreset} and {@link Scheduler.preset.ViewPresetHeaderRow} classes for a
 * description of the view preset properties.
 *
 * ## Localizing View Presets
 * Bryntum Scheduler uses locales for translations including date formats for view presets.
 *
 * To translate date format for view presets just define the date format for the specified region
 * for your locale file, like this:
 * ```javascript
 * const locale = {
 *
 *     localeName : 'En',
 *
 *     // ... Other translations here ...
 *
 *     PresetManager : {
 *         // Translation for the "weekAndDay" ViewPreset
 *         weekAndDay : {
 *             // Change the date format for the top and middle levels
 *             topDateFormat    : 'MMM',
 *             middleDateFormat : 'D'
 *         },
 *         // Translation for the "dayAndWeek" ViewPreset
 *         dayAndWeek : {
 *             // Change the date format for the top level
 *             topDateFormat : 'MMMM YYYY'
 *         }
 *     }
 * }
 *
 * LocaleManager.applyLocale(locale);
 * ```
 *
 * Check the <a target="_blank" href="../examples/localization/">localization demo</a> and [this guide](#Scheduler/guides/customization/localization.md) for more details.
 *
 * @singleton
 * @extends Scheduler/preset/PresetStore
 */
class PresetManager extends PresetStore {
  static get $name() {
    return 'PresetManager';
  }
  static get defaultConfig() {
    return {
      // To not break CSP demo
      preventSubClassingModel: true,
      basePresets: {
        secondAndMinute: {
          name: 'Seconds',
          tickWidth: 30,
          // Time column width
          tickHeight: 40,
          displayDateFormat: 'll LTS',
          // Controls how dates will be displayed in tooltips etc
          shiftIncrement: 10,
          // Controls how much time to skip when calling shiftNext and shiftPrevious.
          shiftUnit: 'minute',
          // Valid values are "millisecond", "second", "minute", "hour", "day", "week", "month", "quarter", "year".
          defaultSpan: 24,
          // By default, if no end date is supplied to a view it will show 24 hours
          timeResolution: {
            // Dates will be snapped to this resolution
            unit: 'second',
            // Valid values are "millisecond", "second", "minute", "hour", "day", "week", "month", "quarter", "year".
            increment: 5
          },
          // This defines your header rows.
          // For each row you can define "unit", "increment", "dateFormat", "renderer", "align", and "thisObj"
          headers: [{
            unit: 'minute',
            dateFormat: 'lll'
          }, {
            unit: 'second',
            increment: 10,
            dateFormat: 'ss'
          }]
        },
        minuteAndHour: {
          name: 'Minutes',
          tickWidth: 60,
          // Time column width
          tickHeight: 60,
          displayDateFormat: 'll LT',
          // Controls how dates will be displayed in tooltips etc
          shiftIncrement: 1,
          // Controls how much time to skip when calling shiftNext and shiftPrevious.
          shiftUnit: 'hour',
          // Valid values are "MILLI", "SECOND", "minute", "HOUR", "DAY", "WEEK", "MONTH", "QUARTER", "YEAR".
          defaultSpan: 24,
          // By default, if no end date is supplied to a view it will show 24 hours
          timeResolution: {
            // Dates will be snapped to this resolution
            unit: 'minute',
            // Valid values are "MILLI", "SECOND", "minute", "HOUR", "DAY", "WEEK", "MONTH", "QUARTER", "YEAR".
            increment: 15
          },
          headers: [{
            unit: 'hour',
            dateFormat: 'ddd MM/DD, hA'
          }, {
            unit: 'minute',
            increment: 30,
            dateFormat: 'mm'
          }]
        },
        hourAndDay: {
          name: 'Day',
          tickWidth: 70,
          tickHeight: 40,
          displayDateFormat: 'll LT',
          shiftIncrement: 1,
          shiftUnit: 'day',
          defaultSpan: 24,
          timeResolution: {
            unit: 'minute',
            increment: 30
          },
          headers: [{
            unit: 'day',
            dateFormat: 'ddd DD/MM' //Mon 01/10
          }, {
            unit: 'hour',
            dateFormat: 'LT'
          }]
        },
        day: {
          name: 'Day/hours',
          displayDateFormat: 'LT',
          shiftIncrement: 1,
          shiftUnit: 'day',
          defaultSpan: 1,
          timeResolution: {
            unit: 'minute',
            increment: 30
          },
          mainHeaderLevel: 0,
          headers: [{
            unit: 'day',
            dateFormat: 'ddd DD/MM',
            // Mon 01/02
            splitUnit: 'day'
          }, {
            unit: 'hour',
            renderer(value) {
              return `
                                    <div class="b-sch-calendarcolumn-ct"><span class="b-sch-calendarcolumn-hours">${DateHelper.format(value, 'HH')}</span>
                                    <span class="b-sch-calendarcolumn-minutes">${DateHelper.format(value, 'mm')}</span></div>
                                `;
            }
          }]
        },
        week: {
          name: 'Week/hours',
          displayDateFormat: 'LT',
          shiftIncrement: 1,
          shiftUnit: 'week',
          defaultSpan: 24,
          timeResolution: {
            unit: 'minute',
            increment: 30
          },
          mainHeaderLevel: 0,
          headers: [{
            unit: 'week',
            dateFormat: 'D d',
            splitUnit: 'day'
          }, {
            unit: 'hour',
            dateFormat: 'LT',
            // will be overridden by renderer
            renderer(value) {
              return `
                                    <div class="sch-calendarcolumn-ct">
                                    <span class="sch-calendarcolumn-hours">${DateHelper.format(value, 'HH')}</span>
                                    <span class="sch-calendarcolumn-minutes">${DateHelper.format(value, 'mm')}</span>
                                    </div>
                                `;
            }
          }]
        },
        dayAndWeek: {
          name: 'Days & Weeks',
          tickWidth: 100,
          tickHeight: 80,
          displayDateFormat: 'll LT',
          shiftUnit: 'day',
          shiftIncrement: 1,
          defaultSpan: 5,
          timeResolution: {
            unit: 'hour',
            increment: 1
          },
          headers: [{
            unit: 'week',
            renderer(start) {
              return DateHelper.getShortNameOfUnit('week') + '.' + DateHelper.format(start, 'WW MMM YYYY');
            }
          }, {
            unit: 'day',
            dateFormat: 'dd DD'
          }]
        },
        // dayAndMonth : {
        //     name              : 'Days & Months',
        //     tickWidth         : 100,
        //     tickHeight        : 80,
        //     displayDateFormat : 'll LT',
        //     shiftUnit         : 'day',
        //     shiftIncrement    : 1,
        //     defaultSpan       : 5,
        //     timeResolution    : {
        //         unit      : 'hour',
        //         increment : 1
        //     },
        //     headers : [
        //         {
        //             unit       : 'month',
        //             dateFormat : 'MMMM YYYY',
        //             align      : 'start'
        //         },
        //         {
        //             unit       : 'day',
        //             dateFormat : 'dd DD'
        //         }
        //     ]
        // },
        dayAndMonth: {
          name: 'Month',
          tickWidth: 100,
          tickHeight: 80,
          displayDateFormat: 'll LT',
          shiftUnit: 'month',
          shiftIncrement: 1,
          defaultSpan: 1,
          mainUnit: 'month',
          timeResolution: {
            unit: 'hour',
            increment: 1
          },
          headers: [{
            unit: 'month',
            dateFormat: 'MMMM YYYY'
          }, {
            unit: 'day',
            dateFormat: 'DD'
          }]
        },
        weekAndDay: {
          name: 'Week',
          tickWidth: 100,
          tickHeight: 80,
          displayDateFormat: 'll hh:mm A',
          shiftUnit: 'week',
          shiftIncrement: 1,
          defaultSpan: 1,
          timeResolution: {
            unit: 'day',
            increment: 1
          },
          mainHeaderLevel: 0,
          headers: [{
            unit: 'week',
            dateFormat: 'YYYY MMMM DD' // 2017 January 01
          }, {
            unit: 'day',
            increment: 1,
            dateFormat: 'DD MMM'
          }]
        },
        weekAndMonth: {
          name: 'Weeks',
          tickWidth: 100,
          tickHeight: 105,
          displayDateFormat: 'll',
          shiftUnit: 'week',
          shiftIncrement: 5,
          defaultSpan: 6,
          timeResolution: {
            unit: 'day',
            increment: 1
          },
          headers: [{
            unit: 'month',
            dateFormat: 'MMM YYYY' //Jan 2017
          }, {
            unit: 'week',
            dateFormat: 'DD MMM'
          }]
        },
        weekAndDayLetter: {
          name: 'Weeks/weekdays',
          tickWidth: 20,
          tickHeight: 50,
          displayDateFormat: 'll',
          shiftUnit: 'week',
          shiftIncrement: 1,
          defaultSpan: 10,
          timeResolution: {
            unit: 'day',
            increment: 1
          },
          mainHeaderLevel: 0,
          headers: [{
            unit: 'week',
            dateFormat: 'ddd DD MMM YYYY',
            verticalColumnWidth: 115
          }, {
            unit: 'day',
            dateFormat: 'd1',
            verticalColumnWidth: 25
          }]
        },
        weekDateAndMonth: {
          name: 'Months/weeks',
          tickWidth: 30,
          tickHeight: 40,
          displayDateFormat: 'll',
          shiftUnit: 'week',
          shiftIncrement: 1,
          defaultSpan: 10,
          timeResolution: {
            unit: 'day',
            increment: 1
          },
          headers: [{
            unit: 'month',
            dateFormat: 'YYYY MMMM'
          }, {
            unit: 'week',
            dateFormat: 'DD'
          }]
        },
        monthAndYear: {
          name: 'Months',
          tickWidth: 110,
          tickHeight: 110,
          displayDateFormat: 'll',
          shiftIncrement: 3,
          shiftUnit: 'month',
          defaultSpan: 12,
          timeResolution: {
            unit: 'day',
            increment: 1
          },
          headers: [{
            unit: 'year',
            dateFormat: 'YYYY' //2017
          }, {
            unit: 'month',
            dateFormat: 'MMM YYYY' //Jan 2017
          }]
        },

        year: {
          name: 'Years',
          tickWidth: 100,
          tickHeight: 100,
          resourceColumnWidth: 100,
          displayDateFormat: 'll',
          shiftUnit: 'year',
          shiftIncrement: 1,
          defaultSpan: 1,
          timeResolution: {
            unit: 'month',
            increment: 1
          },
          headers: [{
            unit: 'year',
            dateFormat: 'YYYY'
          }, {
            unit: 'quarter',
            renderer(start, end, cfg) {
              return DateHelper.getShortNameOfUnit('quarter').toUpperCase() + (Math.floor(start.getMonth() / 3) + 1);
            }
          }]
        },
        manyYears: {
          name: 'Multiple years',
          tickWidth: 40,
          tickHeight: 50,
          displayDateFormat: 'll',
          shiftUnit: 'year',
          shiftIncrement: 1,
          defaultSpan: 10,
          timeResolution: {
            unit: 'year',
            increment: 1
          },
          mainHeaderLevel: 0,
          headers: [{
            unit: 'year',
            increment: 5,
            renderer: (start, end) => start.getFullYear() + ' - ' + end.getFullYear()
          }, {
            unit: 'year',
            dateFormat: 'YY',
            increment: 1
          }]
        }
      },
      // This is a list of bryntum-supplied preset adjustments used to create the Scheduler's
      // default initial set of ViewPresets.
      defaultPresets: [
      // Years over years
      'manyYears', {
        width: 80,
        increment: 1,
        resolution: 1,
        base: 'manyYears',
        resolutionUnit: 'YEAR'
      },
      // Years over quarters
      'year', {
        width: 30,
        increment: 1,
        resolution: 1,
        base: 'year',
        resolutionUnit: 'MONTH'
      }, {
        width: 50,
        increment: 1,
        resolution: 1,
        base: 'year',
        resolutionUnit: 'MONTH'
      }, {
        width: 200,
        increment: 1,
        resolution: 1,
        base: 'year',
        resolutionUnit: 'MONTH'
      },
      // Years over months
      'monthAndYear',
      // Months over weeks
      'weekDateAndMonth',
      // Months over weeks
      'weekAndMonth',
      // Months over weeks
      'weekAndDayLetter',
      // Months over days
      'dayAndMonth',
      // Weeks over days
      'weekAndDay', {
        width: 54,
        increment: 1,
        resolution: 1,
        base: 'weekAndDay',
        resolutionUnit: 'HOUR'
      },
      // Days over hours
      'hourAndDay', {
        width: 64,
        increment: 6,
        resolution: 30,
        base: 'hourAndDay',
        resolutionUnit: 'MINUTE'
      }, {
        width: 100,
        increment: 6,
        resolution: 30,
        base: 'hourAndDay',
        resolutionUnit: 'MINUTE'
      }, {
        width: 64,
        increment: 2,
        resolution: 30,
        base: 'hourAndDay',
        resolutionUnit: 'MINUTE'
      },
      // Hours over minutes
      'minuteAndHour', {
        width: 60,
        increment: 15,
        resolution: 5,
        base: 'minuteAndHour'
      }, {
        width: 130,
        increment: 15,
        resolution: 5,
        base: 'minuteAndHour'
      }, {
        width: 60,
        increment: 5,
        resolution: 5,
        base: 'minuteAndHour'
      }, {
        width: 100,
        increment: 5,
        resolution: 5,
        base: 'minuteAndHour'
      },
      // Minutes over seconds
      'secondAndMinute', {
        width: 60,
        increment: 10,
        resolution: 5,
        base: 'secondAndMinute'
      }, {
        width: 130,
        increment: 5,
        resolution: 5,
        base: 'secondAndMinute'
      }],
      internalListeners: {
        locale: 'updateLocalization'
      }
    };
  }
  set basePresets(basePresets) {
    const presetCache = this._basePresets = {};
    for (const id in basePresets) {
      basePresets[id].id = id;
      presetCache[id] = this.createRecord(basePresets[id]);
    }
  }
  get basePresets() {
    return this._basePresets;
  }
  set defaultPresets(defaultPresets) {
    for (let i = 0, {
        length
      } = defaultPresets; i < length; i++) {
      const presetAdjustment = defaultPresets[i],
        isBase = typeof presetAdjustment === 'string',
        baseType = isBase ? presetAdjustment : presetAdjustment.base;
      let preset;
      // The default was just a string, so it's an unmodified instance of a base type.
      if (isBase) {
        preset = this.basePresets[baseType];
      }
      // If it's an object, it's an adjustment to a base type
      else {
        const config = Object.setPrototypeOf(ObjectHelper.clone(this.basePresets[baseType].data), {
            id: baseType
          }),
          {
            timeResolution
          } = config,
          bottomHeader = config.headers[config.headers.length - 1];
        config.id = undefined;
        if ('width' in presetAdjustment) {
          config.tickWidth = presetAdjustment.width;
        }
        if ('height' in presetAdjustment) {
          config.tickHeight = presetAdjustment.height;
        }
        if ('increment' in presetAdjustment) {
          bottomHeader.increment = presetAdjustment.increment;
        }
        if ('resolution' in presetAdjustment) {
          timeResolution.increment = presetAdjustment.resolution;
        }
        if ('resolutionUnit' in presetAdjustment) {
          timeResolution.unit = DateHelper.getUnitByName(presetAdjustment.resolutionUnit);
        }
        preset = this.createRecord(config);
        // Keep id of original preset around, used with localization in PresetStore
        preset.baseId = baseType;
      }
      this.add(preset);
    }
  }
  getById(id) {
    // Look first in the default set, and if it's one of the base types that is not imported into the
    // default set, then look at the bases.
    return super.getById(id) || this.basePresets[id];
  }
  /**
   * Registers a new view preset base to be used by any scheduler grid or tree on the page.
   * @param {String} id The unique identifier for this preset
   * @param {ViewPresetConfig} config The configuration properties of the view preset (see
   * {@link Scheduler.preset.ViewPreset} for more information)
   * @returns {Scheduler.preset.ViewPreset} A new ViewPreset based upon the passed configuration.
   */
  registerPreset(id, config) {
    const preset = this.createRecord(Object.assign({
        id
      }, config)),
      existingDuplicate = this.find(p => p.equals(preset));
    if (existingDuplicate) {
      return existingDuplicate;
    }
    if (preset.isValid) {
      this.add(preset);
    } else {
      throw new Error('Invalid preset, please check your configuration');
    }
    return preset;
  }
  getPreset(preset) {
    if (typeof preset === 'number') {
      preset = this.getAt(preset);
    }
    if (typeof preset === 'string') {
      preset = this.getById(preset);
    } else if (!(preset instanceof ViewPreset)) {
      preset = this.createRecord(preset);
    }
    return preset;
  }
  /**
   * Applies preset customizations or fetches a preset view preset using its name.
   * @param {String|ViewPresetConfig} presetOrId Id of a predefined preset or a preset config object
   * @returns {Scheduler.preset.ViewPreset} Resulting ViewPreset instance
   */
  normalizePreset(preset) {
    const me = this;
    if (!(preset instanceof ViewPreset)) {
      if (typeof preset === 'string') {
        preset = me.getPreset(preset);
        if (!preset) {
          throw new Error('You must define a valid view preset. See PresetManager for reference');
        }
      } else if (typeof preset === 'object') {
        // Look up any existing ViewPreset that it is based upon.
        if (preset.base) {
          const base = this.getById(preset.base);
          if (!base) {
            throw new Error(`ViewPreset base '${preset.base}' does not exist`);
          }
          // The config is based upon the base's data with the new config object merged in.
          preset = ObjectHelper.merge(ObjectHelper.clone(base.data), preset);
        }
        // Ensure the new ViewPreset has a legible, logical id which does not already
        // exist in our store.
        if (preset.id) {
          preset = me.createRecord(preset);
        } else {
          preset = me.createRecord(ObjectHelper.assign({}, preset));
          preset.id = preset.generateId(preset);
        }
      }
    }
    return preset;
  }
  /**
   * Deletes a view preset
   * @param {String} id The id of the preset, or the preset instance.
   */
  deletePreset(presetOrId) {
    if (typeof presetOrId === 'string') {
      presetOrId = this.getById(presetOrId);
    } else if (typeof presetOrId === 'number') {
      presetOrId = this.getAt(presetOrId);
    }
    if (presetOrId) {
      this.remove(presetOrId);
      // ALso remove it from our base list
      delete this.basePresets[presetOrId.id];
    }
  }
}
const pm = new PresetManager();

/**
 * @module Scheduler/data/TimeAxis
 */
// Micro-optimized version of TimeSpan for faster reading. Hit a lot and since it is internal fields are guaranteed to
// not be remapped and changes won't be batches, so we can always return raw value from data avoiding all additional
// checks and logic
class Tick extends TimeSpan {
  // Only getters on purpose, we do not support manipulating ticks
  get startDate() {
    return this.data.startDate;
  }
  get endDate() {
    return this.data.endDate;
  }
}
/**
 * A class representing the time axis of the scheduler. The scheduler timescale is based on the ticks generated by this
 * class. This is a pure "data" (model) representation of the time axis and has no UI elements.
 *
 * The time axis can be {@link #config-continuous} or not. In continuous mode, each timespan starts where the previous
 * ended, and in non-continuous mode there can be gaps between the ticks.
 * A non-continuous time axis can be used when want to filter out certain periods of time (like weekends) from the time
 * axis.
 *
 * To create a non-continuous time axis you have 2 options. First, you can create a time axis containing only the time
 * spans of interest. To do that, subclass this class and override the {@link #property-generateTicks} method.
 *
 * The other alternative is to call the {@link #function-filterBy} method, passing a function to it which should return
 * `false` if the time tick should be filtered out. Calling {@link Core.data.mixin.StoreFilter#function-clearFilters}
 * will return you to a full time axis.
 *
 * @extends Core/data/Store
 */
class TimeAxis extends Store {
  //region Events
  /**
   * Fires before the timeaxis is about to be reconfigured (e.g. new start/end date or unit/increment). Return `false`
   * to abort the operation.
   * @event beforeReconfigure
   * @param {Scheduler.data.TimeAxis} source The time axis instance
   * @param {Date} startDate The new time axis start date
   * @param {Date} endDate The new time axis end date
   */
  /**
   * Event that is triggered when we end reconfiguring and everything UI-related should be done
   * @event endReconfigure
   * @private
   */
  /**
   * Fires when the timeaxis has been reconfigured (e.g. new start/end date or unit/increment)
   * @event reconfigure
   * @param {Scheduler.data.TimeAxis} source The time axis instance
   */
  /**
   * Fires if all the ticks in the timeaxis are filtered out. After firing the filter is temporarily disabled to
   * return the time axis to a valid state. A disabled filter will be re-enabled the next time ticks are regenerated
   * @event invalidFilter
   * @param {Scheduler.data.TimeAxis} source The time axis instance
   */
  //endregion
  //region Default config
  static get defaultConfig() {
    return {
      modelClass: Tick,
      /**
       * Set to false if the timeline is not continuous, e.g. the next timespan does not start where the previous ended (for example skipping weekends etc).
       * @config {Boolean}
       * @default
       */
      continuous: true,
      originalContinuous: null,
      /**
       * Include only certain hours or days in the time axis (makes it `continuous : false`). Accepts and object
       * with `day` and `hour` properties:
       * ```
       * const scheduler = new Scheduler({
       *     timeAxis : {
       *         include : {
       *              // Do not display hours after 17 or before 9 (only display 9 - 17). The `to´ value is not
       *              // included in the time axis
       *              hour : {
       *                  from : 9,
       *                  to   : 17
       *              },
       *              // Do not display sunday or saturday
       *              day : [0, 6]
       *         }
       *     }
       * }
       * ```
       * In most cases we recommend that you use Scheduler's workingTime config instead. It is easier to use and
       * makes sure all parts of the Scheduler gets updated.
       * @config {Object}
       */
      include: null,
      /**
       * Automatically adjust the timespan when generating ticks with {@link #property-generateTicks} according to
       * the `viewPreset` configuration. Setting this to false may lead to shifting time/date of ticks.
       * @config {Boolean}
       * @default
       */
      autoAdjust: true,
      //isConfigured : false,
      // in case of `autoAdjust : false`, the 1st and last ticks can be truncated, containing only part of the normal tick
      // these dates will contain adjusted start/end (like if the tick has not been truncated)
      adjustedStart: null,
      adjustedEnd: null,
      // the visible position in the first tick, can actually be > 1 because the adjustment is done by the `mainUnit`
      visibleTickStart: null,
      // the visible position in the first tick, is always ticks count - 1 < value <= ticks count, in case of autoAdjust, always = ticks count
      visibleTickEnd: null,
      tickCache: {},
      viewPreset: null,
      maxTraverseTries: 100,
      useRawData: {
        disableDuplicateIdCheck: true,
        disableDefaultValue: true,
        disableTypeConversion: true
      }
    };
  }
  static get configurable() {
    return {
      /**
       * Method generating the ticks for this time axis. Should return an array of ticks. Each tick is an object of the following structure:
       * ```
       * {
       *    startDate : ..., // start date
       *    endDate   : ...  // end date
       * }
       * ```
       * Take notice, that this function either has to be called with `start`/`end` parameters, or create those variables.
       *
       * To see it in action please check out our [TimeAxis](https://bryntum.com/products/scheduler/examples/timeaxis/) example and navigate to "Compressed non-working time" tab.
       *
       * @param {Date} axisStartDate The start date of the interval
       * @param {Date} axisEndDate The end date of the interval
       * @param {String} unit The unit of the time axis
       * @param {Number} increment The increment for the unit specified.
       * @returns {Array} ticks The ticks representing the time axis
       * @config {Function}
       */
      generateTicks: null,
      unit: null,
      increment: null,
      resolutionUnit: null,
      resolutionIncrement: null,
      mainUnit: null,
      shiftUnit: null,
      shiftIncrement: 1,
      defaultSpan: 1,
      weekStartDay: null,
      // Used to force resolution to match whole ticks, to snap accordingly when using fillTicks in the UI
      forceFullTicks: null
    };
  }
  //endregion
  //region Init
  // private
  construct(config) {
    const me = this;
    super.construct(config);
    me.originalContinuous = me.continuous;
    me.ion({
      change: ({
        action
      }) => {
        // If the change was due to filtering, there will be a refresh event
        // arriving next, so do not reconfigure
        if (action !== 'filter') {
          me.trigger('reconfigure', {
            supressRefresh: false
          });
        }
      },
      refresh: () => me.trigger('reconfigure', {
        supressRefresh: false
      }),
      endreconfigure: event => me.trigger('reconfigure', event)
    });
    if (me.startDate) {
      me.internalOnReconfigure();
      me.trigger('reconfigure');
    } else if (me.viewPreset) {
      const range = me.getAdjustedDates(new Date());
      me.startDate = range.startDate;
      me.endDate = range.endDate;
    }
  }
  get isTimeAxis() {
    return true;
  }
  //endregion
  //region Configuration (reconfigure & consumePreset)
  /**
   * Reconfigures the time axis based on the config object supplied and generates the new 'ticks'.
   * @param {Object} config
   * @param {Boolean} [suppressRefresh]
   * @private
   */
  reconfigure(config, suppressRefresh = false, preventThrow = false) {
    const me = this,
      normalized = me.getAdjustedDates(config.startDate, config.endDate),
      oldConfig = {};
    if (me.trigger('beforeReconfigure', {
      startDate: normalized.startDate,
      endDate: normalized.endDate,
      config
    }) !== false) {
      me.trigger('beginReconfigure');
      me._configuredStartDate = config.startDate;
      me._configuredEndDate = config.endDate;
      // Collect old values for end event
      for (const propName in config) {
        oldConfig[propName] = me[propName];
      }
      const viewPresetChanged = config.viewPreset && config.viewPreset !== me.viewPreset;
      // If changing viewPreset, try to gracefully recover if an applied filter results in an empty view
      if (viewPresetChanged) {
        preventThrow = me.isFiltered;
        me.filters.forEach(f => f.disabled = false);
      }
      Object.assign(me, config);
      if (me.internalOnReconfigure(preventThrow, viewPresetChanged) === false) {
        return false;
      }
      me.trigger('endReconfigure', {
        suppressRefresh,
        config,
        oldConfig
      });
    }
  }
  internalOnReconfigure(preventThrow = false, viewPresetChanged) {
    const me = this;
    me.isConfigured = true;
    const adjusted = me.getAdjustedDates(me.startDate, me.endDate, true),
      normalized = me.getAdjustedDates(me.startDate, me.endDate),
      start = normalized.startDate,
      end = normalized.endDate;
    if (start >= end) {
      throw new Error(`Invalid start/end dates. Start date must be less than end date. Start date: ${start}. End date: ${end}.`);
    }
    const {
        unit,
        increment = 1
      } = me,
      ticks = me.generateTicks(start, end, unit, increment);
    // Suspending to be able to detect an invalid filter
    me.suspendEvents();
    me.maintainFilter = preventThrow;
    me.data = ticks;
    me.maintainFilter = false;
    const {
      count
    } = me;
    if (count === 0) {
      if (preventThrow) {
        if (viewPresetChanged) {
          me.disableFilters();
        }
        me.resumeEvents();
        return false;
      }
      throw new Error('Invalid time axis configuration or filter, please check your input data.');
    }
    // start date is cached, update it to fill after generated ticks
    me.startDate = me.first.startDate;
    me.endDate = me.last.endDate;
    me.resumeEvents();
    if (me.isContinuous) {
      me.adjustedStart = adjusted.startDate;
      me.adjustedEnd = DateHelper.getNext(count > 1 ? ticks[count - 1].startDate : adjusted.startDate, unit, increment, me.weekStartDay);
    } else {
      me.adjustedStart = me.startDate;
      me.adjustedEnd = me.endDate;
    }
    me.updateVisibleTickBoundaries();
    me.updateTickCache(true);
  }
  updateVisibleTickBoundaries() {
    const me = this,
      {
        count,
        unit,
        startDate,
        endDate,
        weekStartDay,
        increment = 1
      } = me;
    // Denominator is amount of milliseconds in a full tick (unit * increment). Normally we use 30 days in a month
    // and 365 days in a year. But if month is 31 day long or year is a leap one standard formula might calculate
    // wrong value. e.g. if we're rendering 1 day from August, formula goes like (2021-08-31 - 2021-08-02) / 30 = 1
    // and renders full tick which is incorrect. For such cases we need to adjust denominator to a correct one.
    // Thankfully there are only a few of them - month, year and day with DST transition.
    const startDenominator = DateHelper.getNormalizedUnitDuration(startDate, unit) * increment,
      endDenominator = DateHelper.getNormalizedUnitDuration(endDate, unit) * increment;
    // if visibleTickStart > 1 this means some tick is fully outside of the view - we are not interested in it and want to
    // drop it and adjust "adjustedStart" accordingly
    do {
      me.visibleTickStart = (startDate - me.adjustedStart) / startDenominator;
      if (me.autoAdjust) me.visibleTickStart = Math.floor(me.visibleTickStart);
      if (me.visibleTickStart >= 1) me.adjustedStart = DateHelper.getNext(me.adjustedStart, unit, increment, weekStartDay);
    } while (me.visibleTickStart >= 1);
    do {
      me.visibleTickEnd = count - (me.adjustedEnd - endDate) / endDenominator;
      if (count - me.visibleTickEnd >= 1) me.adjustedEnd = DateHelper.getNext(me.adjustedEnd, unit, -1, weekStartDay);
    } while (count - me.visibleTickEnd >= 1);
    // This flag indicates that the time axis starts exactly on a tick boundary and finishes on a tick boundary
    // This is used as an optimization flag by TimeAxisViewModel.createHeaderRow
    me.fullTicks = !me.visibleTickStart && me.visibleTickEnd === count;
  }
  /**
   * Get the currently used time unit for the ticks
   * @readonly
   * @member {'millisecond'|'second'|'minute'|'hour'|'day'|'week'|'month'|'quarter'|'year'} unit
   */
  /**
   * Get/set currently used preset
   * @property {Scheduler.preset.ViewPreset}
   */
  get viewPreset() {
    return this._viewPreset;
  }
  set viewPreset(preset) {
    const me = this;
    preset = pm.getPreset(preset);
    if (!(preset instanceof ViewPreset)) {
      throw new Error('TimeAxis must be configured with the ViewPreset instance that the Scheduler is using');
    }
    me._viewPreset = preset;
    Object.assign(me, {
      unit: preset.bottomHeader.unit,
      increment: preset.bottomHeader.increment || 1,
      resolutionUnit: preset.timeResolution.unit,
      resolutionIncrement: preset.timeResolution.increment,
      mainUnit: preset.mainHeader.unit,
      shiftUnit: preset.shiftUnit || preset.mainHeader.unit,
      shiftIncrement: preset.shiftIncrement || 1,
      defaultSpan: preset.defaultSpan || 1,
      presetName: preset.id,
      // Weekview columns are updated upon 'datachanged' event on this object.
      // We have to pass headers in order to render them correctly (timeAxisViewModel is incorrect in required time)
      headers: preset.headers
    });
  }
  //endregion
  //region Getters & setters
  get weekStartDay() {
    return this._weekStartDay ?? DateHelper.weekStartDay;
  }
  // private
  get resolution() {
    return {
      unit: this.resolutionUnit,
      increment: this.resolutionIncrement
    };
  }
  // private
  set resolution(resolution) {
    this.resolutionUnit = resolution.unit;
    this.resolutionIncrement = resolution.increment;
  }
  get resolutionUnit() {
    return this.forceFullTicks ? this.unit : this._resolutionUnit;
  }
  get resolutionIncrement() {
    return this.forceFullTicks ? this.increment : this._resolutionIncrement;
  }
  //endregion
  //region Timespan & resolution
  /**
   * Changes the time axis timespan to the supplied start and end dates.
   *
   * **Note** This does **not** preserve the temporal scroll position. You may use
   * {@link Scheduler.view.Scheduler#function-setTimeSpan} to set the time axis and
   * maintain temporal scroll position (if possible).
   * @param {Date} newStartDate The new start date
   * @param {Date} [newEndDate] The new end date
   */
  setTimeSpan(newStartDate, newEndDate, preventThrow = false) {
    // If providing a 0 span range, add default range
    if (newEndDate && newStartDate - newEndDate === 0) {
      newEndDate = null;
    }
    const me = this,
      {
        startDate,
        endDate
      } = me.getAdjustedDates(newStartDate, newEndDate);
    if (me.startDate - startDate !== 0 || me.endDate - endDate !== 0) {
      return me.reconfigure({
        startDate,
        endDate
      }, false, preventThrow);
    }
  }
  /**
   * Moves the time axis by the passed amount and unit.
   *
   * NOTE: When using a filtered TimeAxis the result of `shift()` cannot be guaranteed, it might shift into a
   * filtered out span. It tries to be smart about it by shifting from unfiltered start and end dates.
   * If that solution does not work for your filtering setup, please call {@link #function-setTimeSpan} directly
   * instead.
   *
   * @param {Number} amount The number of units to jump
   * @param {String} [unit] The unit (Day, Week etc)
   */
  shift(amount, unit = this.shiftUnit) {
    const me = this;
    let {
      startDate,
      endDate
    } = me;
    // Use unfiltered start and end dates when shifting a filtered time axis, to lessen risk of messing it up.
    // Still not guaranteed to work though
    if (me.isFiltered) {
      startDate = me.allRecords[0].startDate;
      endDate = me.allRecords[me.allCount - 1].endDate;
    }
    // Hack for filtered time axis, for example if weekend is filtered out and you shiftPrev() day from monday
    let tries = 0;
    do {
      startDate = DateHelper.add(startDate, amount, unit);
      endDate = DateHelper.add(endDate, amount, unit);
    } while (tries++ < me.maxTraverseTries && me.setTimeSpan(startDate, endDate, {
      preventThrow: true
    }) === false);
  }
  /**
   * Moves the time axis forward in time in units specified by the view preset `shiftUnit`, and by the amount specified by the `shiftIncrement`
   * config of the current view preset.
   *
   * NOTE: When using a filtered TimeAxis the result of `shiftNext()` cannot be guaranteed, it might shift into a
   * filtered out span. It tries to be smart about it by shifting from unfiltered start and end dates.
   * If that solution does not work for your filtering setup, please call {@link #function-setTimeSpan} directly
   * instead.
   *
   * @param {Number} [amount] The number of units to jump forward
   */
  shiftNext(amount = this.shiftIncrement) {
    this.shift(amount);
  }
  /**
   * Moves the time axis backward in time in units specified by the view preset `shiftUnit`, and by the amount specified by the `shiftIncrement` config of the current view preset.
   *
   * NOTE: When using a filtered TimeAxis the result of `shiftPrev()` cannot be guaranteed, it might shift into a
   * filtered out span. It tries to be smart about it by shifting from unfiltered start and end dates.
   * If that solution does not work for your filtering setup, please call {@link #function-setTimeSpan} directly
   * instead.
   *
   * @param {Number} [amount] The number of units to jump backward
   */
  shiftPrevious(amount = this.shiftIncrement) {
    this.shift(-amount);
  }
  //endregion
  //region Filter & continuous
  /**
   * Filter the time axis by a function (and clears any existing filters first). The passed function will be called with each tick in time axis.
   * If the function returns `true`, the 'tick' is included otherwise it is filtered. If all ticks are filtered out
   * the time axis is considered invalid, triggering `invalidFilter` and then removing the filter.
   * @param {Function} fn The function to be called, it will receive an object with `startDate`/`endDate` properties, and `index` of the tick.
   * @param {Object} [thisObj] `this` reference for the function
   * @typings {Promise<any|null>}
   */
  filterBy(fn, thisObj = this) {
    const me = this;
    me.filters.clear();
    super.filterBy((tick, index) => fn.call(thisObj, tick.data, index));
  }
  filter() {
    const me = this,
      retVal = super.filter(...arguments);
    if (!me.maintainFilter && me.count === 0) {
      me.resumeEvents();
      me.trigger('invalidFilter');
      me.disableFilters();
    }
    return retVal;
  }
  disableFilters() {
    this.filters.forEach(f => f.disabled = true);
    this.filter();
  }
  triggerFilterEvent(event) {
    const me = this;
    if (!event.filters.count) {
      me.continuous = me.originalContinuous;
    } else {
      me.continuous = false;
    }
    // Filters has been applied (or cleared) but listeners are not informed yet, update tick cache to have start and
    // end dates correct when later redrawing events & header
    me.updateTickCache();
    super.triggerFilterEvent(event);
  }
  /**
   * Returns `true` if the time axis is continuous (will return `false` when filtered)
   * @property {Boolean}
   */
  get isContinuous() {
    return this.continuous !== false && !this.isFiltered;
  }
  //endregion
  //region Dates
  getAdjustedDates(startDate, endDate, forceAdjust = false) {
    const me = this;
    // If providing a 0 span range, add default range
    if (endDate && startDate - endDate === 0) {
      endDate = null;
    }
    startDate = startDate || me.startDate;
    endDate = endDate || DateHelper.add(startDate, me.defaultSpan, me.mainUnit);
    return me.autoAdjust || forceAdjust ? {
      startDate: me.floorDate(startDate, false, me.autoAdjust ? me.mainUnit : me.unit, 1),
      endDate: me.ceilDate(endDate, false, me.autoAdjust ? me.mainUnit : me.unit, 1)
    } : {
      startDate,
      endDate
    };
  }
  /**
   * Method to get the current start date of the time axis.
   * @property {Date}
   */
  get startDate() {
    return this._start || (this.first ? new Date(this.first.startDate) : null);
  }
  set startDate(start) {
    this._start = DateHelper.parse(start);
  }
  /**
   * Method to get a the current end date of the time axis
   * @property {Date}
   */
  get endDate() {
    return this._end || (this.last ? new Date(this.last.endDate) : null);
  }
  set endDate(end) {
    if (end) this._end = DateHelper.parse(end);
  }
  // used in performance critical code for comparisons
  get startMS() {
    return this._startMS;
  }
  // used in performance critical code for comparisons
  get endMS() {
    return this._endMS;
  }
  // Floors a date and optionally snaps it to one of the following resolutions:
  // 1. 'resolutionUnit'. If param 'resolutionUnit' is passed, the date will simply be floored to this unit.
  // 2. If resolutionUnit is not passed: If date should be snapped relative to the timeaxis start date,
  // the resolutionUnit of the timeAxis will be used, or the timeAxis 'mainUnit' will be used to snap the date
  //
  // returns a copy of the original date
  // private
  floorDate(date, relativeToStart, resolutionUnit, incr) {
    relativeToStart = relativeToStart !== false;
    const me = this,
      relativeTo = relativeToStart ? DateHelper.clone(me.startDate) : null,
      increment = incr || me.resolutionIncrement,
      unit = resolutionUnit || (relativeToStart ? me.resolutionUnit : me.mainUnit),
      snap = (value, increment) => Math.floor(value / increment) * increment;
    if (relativeToStart) {
      const snappedDuration = snap(DateHelper.diff(relativeTo, date, unit), increment);
      return DateHelper.add(relativeTo, snappedDuration, unit, false);
    }
    const dt = DateHelper.clone(date);
    if (unit === 'week') {
      const day = dt.getDay() || 7,
        startDay = me.weekStartDay || 7;
      DateHelper.add(DateHelper.startOf(dt, 'day', false), day >= startDay ? startDay - day : -(7 - startDay + day), 'day', false);
      // Watch out for Brazil DST craziness (see test 028_timeaxis_dst.t.js)
      if (dt.getDay() !== startDay && dt.getHours() === 23) {
        DateHelper.add(dt, 1, 'hour', false);
      }
    } else {
      // removes "smaller" units from date (for example minutes; removes seconds and milliseconds)
      DateHelper.startOf(dt, unit, false);
      // day and year are 1-based so need to make additional adjustments
      const modifier = ['day', 'year'].includes(unit) ? 1 : 0,
        useUnit = unit === 'day' ? 'date' : unit,
        snappedValue = snap(DateHelper.get(dt, useUnit) - modifier, increment) + modifier;
      DateHelper.set(dt, useUnit, snappedValue);
    }
    return dt;
  }
  /**
   * Rounds the date to nearest unit increment
   * @private
   */
  roundDate(date, relativeTo, resolutionUnit = this.resolutionUnit, increment = this.resolutionIncrement || 1) {
    const me = this,
      dt = DateHelper.clone(date);
    relativeTo = DateHelper.clone(relativeTo || me.startDate);
    switch (resolutionUnit) {
      case 'week':
        {
          DateHelper.startOf(dt, 'day', false);
          let distanceToWeekStartDay = dt.getDay() - me.weekStartDay,
            toAdd;
          if (distanceToWeekStartDay < 0) {
            distanceToWeekStartDay = 7 + distanceToWeekStartDay;
          }
          if (Math.round(distanceToWeekStartDay / 7) === 1) {
            toAdd = 7 - distanceToWeekStartDay;
          } else {
            toAdd = -distanceToWeekStartDay;
          }
          return DateHelper.add(dt, toAdd, 'day', false);
        }
      case 'month':
        {
          const nbrMonths = DateHelper.diff(relativeTo, dt, 'month') + DateHelper.as('month', dt.getDay() / DateHelper.daysInMonth(dt)),
            //*/DH.as('month', DH.diff(relativeTo, dt)) + (dt.getDay() / DH.daysInMonth(dt)),
            snappedMonths = Math.round(nbrMonths / increment) * increment;
          return DateHelper.add(relativeTo, snappedMonths, 'month', false);
        }
      case 'quarter':
        DateHelper.startOf(dt, 'month', false);
        return DateHelper.add(dt, 3 - dt.getMonth() % 3, 'month', false);
      default:
        {
          const duration = DateHelper.as(resolutionUnit, DateHelper.diff(relativeTo, dt)),
            // Need to find the difference of timezone offsets between relativeTo and original dates. 0 if timezone offsets are the same.
            offset = DateHelper.as(resolutionUnit, relativeTo.getTimezoneOffset() - dt.getTimezoneOffset(), 'minute'),
            // Need to add the offset to the whole duration, so the divided value will take DST into account
            snappedDuration = Math.round((duration + offset) / increment) * increment;
          // Now when the round is done, we need to subtract the offset, so the result also will take DST into account
          return DateHelper.add(relativeTo, snappedDuration - offset, resolutionUnit, false);
        }
    }
  }
  // private
  ceilDate(date, relativeToStart, resolutionUnit, increment) {
    const me = this;
    relativeToStart = relativeToStart !== false;
    increment = increment || (relativeToStart ? me.resolutionIncrement : 1);
    const unit = resolutionUnit || (relativeToStart ? me.resolutionUnit : me.mainUnit),
      dt = DateHelper.clone(date);
    let doCall = false;
    switch (unit) {
      case 'minute':
        doCall = !DateHelper.isStartOf(dt, 'minute');
        break;
      case 'hour':
        doCall = !DateHelper.isStartOf(dt, 'hour');
        break;
      case 'day':
      case 'date':
        doCall = !DateHelper.isStartOf(dt, 'day');
        break;
      case 'week':
        DateHelper.startOf(dt, 'day', false);
        doCall = dt.getDay() !== me.weekStartDay || !DateHelper.isEqual(dt, date);
        break;
      case 'month':
        DateHelper.startOf(dt, 'day', false);
        doCall = dt.getDate() !== 1 || !DateHelper.isEqual(dt, date);
        break;
      case 'quarter':
        DateHelper.startOf(dt, 'day', false);
        doCall = dt.getMonth() % 3 !== 0 || dt.getDate() !== 1 || !DateHelper.isEqual(dt, date);
        break;
      case 'year':
        DateHelper.startOf(dt, 'day', false);
        doCall = dt.getMonth() !== 0 || dt.getDate() !== 1 || !DateHelper.isEqual(dt, date);
        break;
    }
    if (doCall) {
      return DateHelper.getNext(dt, unit, increment, me.weekStartDay);
    }
    return dt;
  }
  //endregion
  //region Ticks
  get include() {
    return this._include;
  }
  set include(include) {
    const me = this;
    me._include = include;
    me.continuous = !include;
    if (!me.isConfiguring) {
      me.startDate = me._configuredStartDate;
      me.endDate = me._configuredEndDate;
      me.internalOnReconfigure();
      me.trigger('includeChange');
    }
  }
  // Check if a certain date is included based on timeAxis.include rules
  processExclusion(startDate, endDate, unit) {
    const {
      include
    } = this;
    if (include) {
      return Object.entries(include).some(([includeUnit, rule]) => {
        if (!rule) {
          return false;
        }
        const {
          from,
          to
        } = rule;
        // Including the closest smaller unit with a { from, to} rule should affect start & end of the
        // generated tick. Currently only works for days or smaller.
        if (DateHelper.compareUnits('day', unit) >= 0 && DateHelper.getLargerUnit(includeUnit) === unit) {
          if (from) {
            DateHelper.set(startDate, includeUnit, from);
          }
          if (to) {
            let stepUnit = unit;
            // Stepping back base on date, not day
            if (unit === 'day') {
              stepUnit = 'date';
            }
            // Since endDate is not inclusive it points to the next day etc.
            // Turns for example 2019-01-10T00:00 -> 2019-01-09T18:00
            DateHelper.set(endDate, {
              [stepUnit]: DateHelper.get(endDate, stepUnit) - 1,
              [includeUnit]: to
            });
          }
        }
        // "Greater" unit being included? Then we need to care about it
        // (for example excluding day will also affect hour, minute etc)
        if (DateHelper.compareUnits(includeUnit, unit) >= 0) {
          const datePart = includeUnit === 'day' ? startDate.getDay() : DateHelper.get(startDate, includeUnit);
          if (from && datePart < from || to && datePart >= to) {
            return true;
          }
        }
      });
    }
    return false;
  }
  // Calculate constants used for exclusion when scaling within larger ticks
  initExclusion() {
    Object.entries(this.include).forEach(([unit, rule]) => {
      if (rule) {
        const {
          from,
          to
        } = rule;
        // For example for hour:
        // 1. Get the next bigger unit -> day, get ratio -> 24
        // 2. to 20 - from 8 = 12 hours visible each day. lengthFactor 24 / 12 = 2 means that each hour used
        // needs to represent 2 hours when drawn (to stretch)
        // |    ████    | -> |  ████████  |
        rule.lengthFactor = DateHelper.getUnitToBaseUnitRatio(unit, DateHelper.getLargerUnit(unit)) / (to - from);
        rule.lengthFactorExcl = DateHelper.getUnitToBaseUnitRatio(unit, DateHelper.getLargerUnit(unit)) / (to - from - 1);
        // Calculate weighted center to stretch around |   ██x█ |
        rule.center = from + from / (rule.lengthFactor - 1);
      }
    });
  }
  /**
   * Method generating the ticks for this time axis. Should return an array of ticks. Each tick is an object of the following structure:
   * ```
   * {
   *    startDate : ..., // start date
   *    endDate   : ...  // end date
   * }
   * ```
   * Take notice, that this function either has to be called with `start`/`end` parameters, or create those variables.
   *
   * To see it in action please check out our [TimeAxis](https://bryntum.com/products/scheduler/examples/timeaxis/) example and navigate to "Compressed non-working time" tab.
   *
   * @member {Function} generateTicks
   * @param {Date} axisStartDate The start date of the interval
   * @param {Date} axisEndDate The end date of the interval
   * @param {String} unit The unit of the time axis
   * @param {Number} increment The increment for the unit specified.
   * @returns {Array} ticks The ticks representing the time axis
   */
  updateGenerateTicks() {
    if (!this.isConfiguring) {
      this.reconfigure(this);
    }
  }
  _generateTicks(axisStartDate, axisEndDate, unit = this.unit, increment = this.increment) {
    const me = this,
      ticks = [],
      usesExclusion = Boolean(me.include);
    let intervalEnd,
      tickEnd,
      isExcluded,
      dstDiff = 0,
      {
        startDate,
        endDate
      } = me.getAdjustedDates(axisStartDate, axisEndDate);
    me.tickCache = {};
    if (usesExclusion) {
      me.initExclusion();
    }
    while (startDate < endDate) {
      intervalEnd = DateHelper.getNext(startDate, unit, increment, me.weekStartDay);
      if (!me.autoAdjust && intervalEnd > endDate) {
        intervalEnd = endDate;
      }
      // Handle hourly increments crossing DST boundaries to keep the timescale looking correct
      // Only do this for HOUR resolution currently, and only handle it once per tick generation.
      if (unit === 'hour' && increment > 1 && ticks.length > 0 && dstDiff === 0) {
        const prev = ticks[ticks.length - 1];
        dstDiff = (prev.startDate.getHours() + increment) % 24 - prev.endDate.getHours();
        if (dstDiff !== 0) {
          // A DST boundary was crossed in previous tick, adjust this tick to keep timeaxis "symmetric".
          intervalEnd = DateHelper.add(intervalEnd, dstDiff, 'hour');
        }
      }
      isExcluded = false;
      if (usesExclusion) {
        tickEnd = new Date(intervalEnd.getTime());
        isExcluded = me.processExclusion(startDate, intervalEnd, unit);
      } else {
        tickEnd = intervalEnd;
      }
      if (!isExcluded) {
        ticks.push({
          id: ticks.length + 1,
          startDate,
          endDate: intervalEnd
        });
        me.tickCache[startDate.getTime()] = ticks.length - 1;
      }
      startDate = tickEnd;
    }
    return ticks;
  }
  /**
   * How many ticks are visible across the TimeAxis.
   *
   * Usually, this is an integer because {@link #config-autoAdjust} means that the start and end
   * dates are adjusted to be on tick boundaries.
   * @property {Number}
   * @internal
   */
  get visibleTickTimeSpan() {
    const me = this;
    return me.isContinuous ? me.visibleTickEnd - me.visibleTickStart : me.count;
  }
  /**
   * Gets a tick "coordinate" representing the date position on the time scale. Returns -1 if the date is not part of the time axis.
   * @param {Date} date the date
   * @returns {Number} the tick position on the scale or -1 if the date is not part of the time axis
   */
  getTickFromDate(date) {
    var _date$getTime;
    const me = this,
      ticks = me.records,
      dateMS = ((_date$getTime = date.getTime) === null || _date$getTime === void 0 ? void 0 : _date$getTime.call(date)) ?? date;
    let begin = 0,
      end = ticks.length - 1,
      middle,
      tick,
      tickStart,
      tickEnd;
    // Quickly eliminate out of range dates or if we have not been set up with a time range yet
    if (!ticks.length || dateMS < ticks[0].startDateMS || dateMS > ticks[end].endDateMS) {
      return -1;
    }
    if (me.isContinuous) {
      // Chop tick cache in half until we find a match
      while (begin < end) {
        middle = begin + end + 1 >> 1;
        if (dateMS > ticks[middle].endDateMS) {
          begin = middle + 1;
        } else if (dateMS < ticks[middle].startDateMS) {
          end = middle - 1;
        } else {
          begin = middle;
        }
      }
      tick = ticks[begin];
      tickStart = tick.startDateMS;
      // Part way though, calculate the fraction
      if (dateMS > tickStart) {
        tickEnd = tick.endDateMS;
        begin += (dateMS - tickStart) / (tickEnd - tickStart);
      }
      return Math.min(Math.max(begin, me.visibleTickStart), me.visibleTickEnd);
    } else {
      for (let i = 0; i <= end; i++) {
        tickEnd = ticks[i].endDateMS;
        if (dateMS <= tickEnd) {
          tickStart = ticks[i].startDateMS;
          // date < tickStart can occur in filtered case
          tick = i + (dateMS > tickStart ? (dateMS - tickStart) / (tickEnd - tickStart) : 0);
          return tick;
        }
      }
    }
  }
  /**
   * Gets the time represented by a tick "coordinate".
   * @param {Number} tick the tick "coordinate"
   * @param {'floor'|'round'|'ceil'} [roundingMethod] Rounding method to use. 'floor' to take the tick (lowest header
   * in a time axis) start date, 'round' to round the value to nearest increment or 'ceil' to take the tick end date
   * @returns {Date} The date to represented by the tick "coordinate", or null if invalid.
   */
  getDateFromTick(tick, roundingMethod) {
    const me = this;
    if (tick === me.visibleTickEnd) {
      return me.endDate;
    }
    const wholeTick = Math.floor(tick),
      fraction = tick - wholeTick,
      t = me.getAt(wholeTick);
    if (!t) {
      return null;
    }
    const
      // if we've filtered timeaxis using filterBy, then we cannot trust to adjustedStart property and should use tick start
      start = wholeTick === 0 && me.isContinuous ? me.adjustedStart : t.startDate,
      // if we've filtered timeaxis using filterBy, then we cannot trust to adjustedEnd property and should use tick end
      end = wholeTick === me.count - 1 && me.isContinuous ? me.adjustedEnd : t.endDate;
    let date = DateHelper.add(start, fraction * (end - start), 'millisecond');
    if (roundingMethod) {
      date = me[roundingMethod + 'Date'](date);
    }
    return date;
  }
  /**
   * Returns the ticks of the timeaxis in an array of objects with a "startDate" and "endDate".
   * @property {Scheduler.model.TimeSpan[]}
   */
  get ticks() {
    return this.records;
  }
  /**
   * Caches ticks and start/end dates for faster processing during rendering of events.
   * @private
   */
  updateTickCache(onlyStartEnd = false) {
    const me = this;
    if (me.count) {
      me._start = me.first.startDate;
      me._end = me.last.endDate;
      me._startMS = me.startDate.getTime();
      me._endMS = me.endDate.getTime();
    } else {
      me._start = me._end = me._startMs = me._endMS = null;
    }
    // onlyStartEnd is true prior to clearing filters, to get start and end dates correctly during that process.
    // No point in filling tickCache yet in that case, it will be done after the filters are cleared
    if (!onlyStartEnd) {
      me.tickCache = {};
      me.forEach((tick, i) => me.tickCache[tick.startDate.getTime()] = i);
    }
  }
  //endregion
  //region Axis
  /**
   * Returns true if the passed date is inside the span of the current time axis.
   * @param {Date} date The date to query for
   * @returns {Boolean} true if the date is part of the time axis
   */
  dateInAxis(date, inclusiveEnd = false) {
    const me = this,
      axisStart = me.startDate,
      axisEnd = me.endDate;
    // Date is between axis start/end and axis is not continuous - need to perform better lookup
    if (me.isContinuous) {
      return inclusiveEnd ? DateHelper.betweenLesserEqual(date, axisStart, axisEnd) : DateHelper.betweenLesser(date, axisStart, axisEnd);
    } else {
      const length = me.getCount();
      let tickStart, tickEnd, tick;
      for (let i = 0; i < length; i++) {
        tick = me.getAt(i);
        tickStart = tick.startDate;
        tickEnd = tick.endDate;
        if (inclusiveEnd && date <= tickEnd || !inclusiveEnd && date < tickEnd) {
          return date >= tickStart;
        }
      }
    }
    return false;
  }
  /**
   * Returns true if the passed timespan is part of the current time axis (in whole or partially).
   * @param {Date} start The start date
   * @param {Date} end The end date
   * @returns {Boolean} true if the timespan is part of the timeaxis
   */
  timeSpanInAxis(start, end) {
    const me = this;
    if (!end || end.getTime() === start.getTime()) {
      return this.dateInAxis(start, true);
    }
    if (me.isContinuous) {
      return DateHelper.intersectSpans(start, end, me.startDate, me.endDate);
    }
    return start < me.startDate && end > me.endDate || me.getTickFromDate(start) !== me.getTickFromDate(end);
  }
  // Accepts a TimeSpan model (uses its cached MS values to be a bit faster during rendering)
  isTimeSpanInAxis(timeSpan) {
    const me = this,
      {
        startMS,
        endMS
      } = me,
      {
        startDateMS
      } = timeSpan,
      endDateMS = timeSpan.endDateMS ?? timeSpan.meta.endDateCached;
    // only consider fully scheduled ranges
    if (!startDateMS || !endDateMS) return false;
    if (endDateMS === startDateMS) {
      return me.dateInAxis(timeSpan.startDate, true);
    }
    if (me.isContinuous) {
      return endDateMS > startMS && startDateMS < endMS;
    }
    const startTick = me.getTickFromDate(timeSpan.startDate),
      endTick = me.getTickFromDate(timeSpan.endDate);
    // endDate is not inclusive
    if (startTick === me.count && DateHelper.isEqual(timeSpan.startDate, me.last.endDate) || endTick === 0 && DateHelper.isEqual(timeSpan.endDate, me.first.startDate)) {
      return false;
    }
    return (
      // Spanning entire axis
      startDateMS < startMS && endDateMS > endMS ||
      // Unintentionally 0 wide (ticks excluded or outside)
      startTick !== endTick
    );
  }
  //endregion
  //region Iteration
  /**
   * Calls the supplied iterator function once per interval. The function will be called with four parameters, startDate endDate, index, isLastIteration.
   * @internal
   * @param {String} unit The unit to use when iterating over the timespan
   * @param {Number} increment The increment to use when iterating over the timespan
   * @param {Function} iteratorFn The function to call
   * @param {Object} [thisObj] `this` reference for the function
   */
  forEachAuxInterval(unit, increment = 1, iteratorFn, thisObj = this) {
    const end = this.endDate;
    let dt = this.startDate,
      i = 0,
      intervalEnd;
    if (dt > end) throw new Error('Invalid time axis configuration');
    while (dt < end) {
      intervalEnd = DateHelper.min(DateHelper.getNext(dt, unit, increment, this.weekStartDay), end);
      iteratorFn.call(thisObj, dt, intervalEnd, i, intervalEnd >= end);
      dt = intervalEnd;
      i++;
    }
  }
  //endregion
}

TimeAxis._$name = 'TimeAxis';

/**
 * @module Scheduler/feature/base/DragBase
 */
/**
 * Base class for EventDrag (Scheduler) and TaskDrag (Gantt) features. Contains shared code. Not to be used directly.
 *
 * @extends Core/mixin/InstancePlugin
 * @abstract
 */
class DragBase extends InstancePlugin {
  //region Config
  static get defaultConfig() {
    return {
      // documented on Schedulers EventDrag feature and Gantt's TaskDrag
      tooltipTemplate: data => `
                <div class="b-sch-tip-${data.valid ? 'valid' : 'invalid'}">
                    ${data.startClockHtml}
                    ${data.endClockHtml}
                    <div class="b-sch-tip-message">${data.message}</div>
                </div>
            `,
      /**
       * Specifies whether or not to show tooltip while dragging event
       * @config {Boolean}
       * @default
       */
      showTooltip: true,
      /**
       * When enabled, the event being dragged always "snaps" to the exact start date that it will have after drop.
       * @config {Boolean}
       * @default
       */
      showExactDropPosition: false,
      /*
       * The store from which the dragged items are mapped to the UI.
       * In Scheduler's implementation of this base class, this will be
       * an EventStore, in Gantt's implementations, this will be a TaskStore.
       * Because both derive from this base, we must refer to it as this.store.
       * @private
       */
      store: null,
      /**
       * An object used to configure the internal {@link Core.helper.DragHelper} class
       * @config {DragHelperConfig}
       */
      dragHelperConfig: null,
      tooltipCls: 'b-eventdrag-tooltip'
    };
  }
  static get configurable() {
    return {
      /**
       * Set to `false` to allow dragging tasks outside the client Scheduler.
       * Useful when you want to drag tasks between multiple Scheduler instances
       * @config {Boolean}
       * @default
       */
      constrainDragToTimeline: true,
      // documented on Schedulers EventDrag feature, not used for Gantt
      constrainDragToResource: true,
      constrainDragToTimeSlot: false,
      /**
       * Yields the {@link Core.widget.Tooltip} which tracks the event during a drag operation.
       * @member {Core.widget.Tooltip} tip
       */
      /**
       * A config object to allow customization of the {@link Core.widget.Tooltip} which tracks
       * the event during a drag operation.
       * @config {TooltipConfig}
       */
      tip: {
        $config: ['lazy', 'nullify'],
        value: {
          align: {
            align: 'b-t',
            allowTargetOut: true
          },
          autoShow: true,
          updateContentOnMouseMove: true
        }
      },
      /**
       * The `eventDrag`and `taskDrag` events are normally only triggered when the drag operation will lead to a
       * change in date or assignment. By setting this config to `false`, that logic is bypassed to trigger events
       * for each native mouse move event.
       * @prp {Boolean}
       */
      throttleDragEvent: true
    };
  }
  // Plugin configuration. This plugin chains some of the functions in Grid.
  static get pluginConfig() {
    return {
      chain: ['onPaint']
    };
  }
  //endregion
  //region Init
  internalSnapToPosition(snapTo) {
    var _this$snapToPosition;
    const {
      dragData
    } = this;
    (_this$snapToPosition = this.snapToPosition) === null || _this$snapToPosition === void 0 ? void 0 : _this$snapToPosition.call(this, {
      assignmentRecord: dragData.assignmentRecord,
      eventRecord: dragData.eventRecord,
      resourceRecord: dragData.newResource || dragData.resourceRecord,
      startDate: dragData.startDate,
      endDate: dragData.endDate,
      snapTo
    });
  }
  buildDragHelperConfig() {
    const me = this,
      {
        client,
        constrainDragToTimeline,
        constrainDragToResource,
        constrainDragToTimeSlot,
        dragHelperConfig = {}
      } = me,
      {
        timeAxisViewModel,
        isHorizontal
      } = client,
      lockY = isHorizontal ? constrainDragToResource : constrainDragToTimeSlot,
      lockX = isHorizontal ? constrainDragToTimeSlot : constrainDragToResource;
    // If implementer wants to allow users dragging outside the timeline element, setup the internal dropTargetSelector
    if (me.externalDropTargetSelector) {
      dragHelperConfig.dropTargetSelector = `.b-timeaxissubgrid,${me.externalDropTargetSelector}`;
    }
    return Objects.merge({
      name: me.constructor.name,
      // useful when debugging with multiple draggers
      positioning: 'absolute',
      lockX,
      lockY,
      minX: true,
      // Allows dropping with start before time axis
      maxX: true,
      // Allows dropping with end after time axis
      constrain: false,
      cloneTarget: !constrainDragToTimeline,
      // If we clone event dragged bars, we assume ownership upon drop so we can reuse the element and have animations
      removeProxyAfterDrop: false,
      dragWithin: constrainDragToTimeline ? null : document.body,
      hideOriginalElement: true,
      dropTargetSelector: '.b-timelinebase',
      // A CSS class added to drop target while dragging events
      dropTargetCls: me.externalDropTargetSelector ? 'b-drop-target' : '',
      outerElement: client.timeAxisSubGridElement,
      targetSelector: client.eventSelector,
      scrollManager: constrainDragToTimeline ? client.scrollManager : null,
      createProxy: el => me.createProxy(el),
      snapCoordinates: ({
        element,
        newX,
        newY
      }) => {
        const {
          dragData
        } = me;
        // Snapping not supported when dragging outside a scheduler
        if (me.constrainDragToTimeline && !me.constrainDragToTimeSlot && (me.showExactDropPosition || timeAxisViewModel.snap)) {
          const draggedEventRecord = dragData.draggedEntities[0],
            coordinate = me.getCoordinate(draggedEventRecord, element, [newX, newY]),
            snappedDate = timeAxisViewModel.getDateFromPosition(coordinate, 'round'),
            {
              calendar
            } = draggedEventRecord;
          if (!calendar || snappedDate && calendar.isWorkingTime(snappedDate, DateHelper.add(snappedDate, draggedEventRecord.fullDuration))) {
            const snappedPosition = snappedDate && timeAxisViewModel.getPositionFromDate(snappedDate);
            if (snappedDate && snappedDate >= client.startDate && snappedPosition != null) {
              if (isHorizontal) {
                newX = snappedPosition;
              } else {
                newY = snappedPosition;
              }
            }
          }
        }
        const snapTo = {
          x: newX,
          y: newY
        };
        me.internalSnapToPosition(snapTo);
        return snapTo;
      },
      internalListeners: {
        beforedragstart: 'onBeforeDragStart',
        dragstart: 'onDragStart',
        afterdragstart: 'onAfterDragStart',
        drag: 'onDrag',
        drop: 'onDrop',
        abort: 'onDragAbort',
        abortFinalized: 'onDragAbortFinalized',
        reset: 'onDragReset',
        thisObj: me
      }
    }, dragHelperConfig, {
      isElementDraggable: (el, event) => {
        return (!dragHelperConfig || !dragHelperConfig.isElementDraggable || dragHelperConfig.isElementDraggable(el, event)) && me.isElementDraggable(el, event);
      }
    });
  }
  /**
   * Called when scheduler is rendered. Sets up drag and drop and hover tooltip.
   * @private
   */
  onPaint({
    firstPaint
  }) {
    var _me$drag;
    const me = this,
      {
        client
      } = me;
    (_me$drag = me.drag) === null || _me$drag === void 0 ? void 0 : _me$drag.destroy();
    me.drag = DragHelper.new(me.buildDragHelperConfig());
    if (firstPaint) {
      client.rowManager.ion({
        changeTotalHeight: () => {
          var _me$dragData;
          return me.updateYConstraint((_me$dragData = me.dragData) === null || _me$dragData === void 0 ? void 0 : _me$dragData[`${client.scheduledEventName}Record`]);
        },
        thisObj: me
      });
    }
    if (me.showTooltip) {
      me.clockTemplate = new ClockTemplate({
        scheduler: client
      });
    }
  }
  doDestroy() {
    var _this$drag, _this$clockTemplate, _this$tip;
    (_this$drag = this.drag) === null || _this$drag === void 0 ? void 0 : _this$drag.destroy();
    (_this$clockTemplate = this.clockTemplate) === null || _this$clockTemplate === void 0 ? void 0 : _this$clockTemplate.destroy();
    (_this$tip = this.tip) === null || _this$tip === void 0 ? void 0 : _this$tip.destroy();
    super.doDestroy();
  }
  get tipId() {
    return `${this.client.id}-event-drag-tip`;
  }
  changeTip(tip, oldTip) {
    const me = this;
    if (tip) {
      const result = Tooltip.reconfigure(oldTip, Tooltip.mergeConfigs({
        forElement: me.element,
        id: me.tipId,
        getHtml: me.getTipHtml.bind(me),
        cls: me.tooltipCls,
        owner: me.client
      }, tip), {
        owner: me.client,
        defaults: {
          type: 'tooltip'
        }
      });
      result.ion({
        innerHtmlUpdate: 'updateDateIndicator',
        thisObj: me
      });
      return result;
    } else {
      oldTip === null || oldTip === void 0 ? void 0 : oldTip.destroy();
    }
  }
  //endregion
  //region Drag events
  createProxy(element) {
    const proxy = element.cloneNode(true);
    delete proxy.id;
    proxy.classList.add(`b-sch-${this.client.mode}`);
    return proxy;
  }
  onBeforeDragStart({
    context,
    event
  }) {
    const me = this,
      {
        client
      } = me,
      dragData = me.getMinimalDragData(context, event),
      eventRecord = dragData === null || dragData === void 0 ? void 0 : dragData[`${client.scheduledEventName}Record`],
      resourceRecord = dragData.resourceRecord;
    if (client.readOnly || me.disabled || !eventRecord || eventRecord.isDraggable === false || eventRecord.readOnly || resourceRecord !== null && resourceRecord !== void 0 && resourceRecord.readOnly) {
      return false;
    }
    // Cache the date corresponding to the drag start point so that on drag, we can always
    // perform the same calculation to then find the time delta without having to calculate
    // the new start and end times from the position that the element is.
    context.pointerStartDate = client.getDateFromXY([context.startClientX, context.startPageY], null, false);
    const result = me.triggerBeforeEventDrag(`before${client.capitalizedEventName}Drag`, {
      ...dragData,
      event,
      // to be deprecated
      context: {
        ...context,
        ...dragData
      }
    }) !== false;
    if (result) {
      var _client;
      me.updateYConstraint(eventRecord, resourceRecord);
      // Hook for features that need to react to drag starting, used by NestedEvents
      (_client = client[`before${client.capitalizedEventName}DragStart`]) === null || _client === void 0 ? void 0 : _client.call(client, context, dragData);
    }
    return result;
  }
  onAfterDragStart({
    context,
    event
  }) {}
  /**
   * Returns true if a drag operation is active
   * @property {Boolean}
   * @readonly
   */
  get isDragging() {
    var _this$drag2;
    return (_this$drag2 = this.drag) === null || _this$drag2 === void 0 ? void 0 : _this$drag2.isDragging;
  }
  // Checked by dependencies to determine if live redrawing is needed
  get isActivelyDragging() {
    return this.isDragging && !this.finalizing;
  }
  /**
   * Triggered when dragging of an event starts. Initializes drag data associated with the event being dragged.
   * @private
   */
  onDragStart({
    context,
    event
  }) {
    var _client2, _menuFeature$hideCont;
    const me = this,
      // When testing with Selenium, it simulates drag and drop with a single mousemove event, we might be over
      // another client already
      client = me.findClientFromTarget(event, context) ?? me.client;
    me.currentOverClient = client;
    me.drag.unifiedProxy = me.unifiedDrag;
    me.onMouseOverNewTimeline(client, true);
    const dragData = me.dragData = me.getDragData(context);
    // Do not let DomSync reuse the element
    me.suspendElementRedrawing(context.element);
    if (me.showTooltip && me.tip) {
      const tipTarget = dragData.context.dragProxy ? dragData.context.dragProxy.firstChild : context.element;
      me.tip.showBy(tipTarget);
    }
    me.triggerDragStart(dragData);
    // Hook for features that need to take action after drag starts
    (_client2 = client[`after${client.capitalizedEventName}DragStart`]) === null || _client2 === void 0 ? void 0 : _client2.call(client, context, dragData);
    const {
        eventMenu,
        taskMenu
      } = client.features,
      menuFeature = eventMenu || taskMenu;
    // If this is a touch action, hide the context menu which may have shown
    menuFeature === null || menuFeature === void 0 ? void 0 : (_menuFeature$hideCont = menuFeature.hideContextMenu) === null || _menuFeature$hideCont === void 0 ? void 0 : _menuFeature$hideCont.call(menuFeature, false);
  }
  updateDateIndicator() {
    const {
        startDate,
        endDate
      } = this.dragData,
      {
        tip,
        clockTemplate
      } = this,
      endDateElement = tip.element.querySelector('.b-sch-tooltip-enddate');
    clockTemplate.updateDateIndicator(tip.element, startDate);
    endDateElement && clockTemplate.updateDateIndicator(endDateElement, endDate);
  }
  findClientFromTarget(event, context) {
    let {
      target
    } = event;
    // Can't detect target under a touch event
    if (/^touch/.test(event.type)) {
      const center = Rectangle.from(context.element, null, true).center;
      target = DomHelper.elementFromPoint(center.x, center.y);
    }
    const client = Widget.fromElement(target, 'timelinebase');
    // Do not allow drops on histogram widgets
    return client !== null && client !== void 0 && client.isResourceHistogram ? null : client;
  }
  /**
   * Triggered while dragging an event. Updates drag data, validation etc.
   * @private
   */
  onDrag({
    context,
    event
  }) {
    const me = this,
      dd = me.dragData,
      start = dd.startDate;
    let client;
    if (me.constrainDragToTimeline) {
      client = me.client;
    } else {
      client = me.findClientFromTarget(event, dd.context);
    }
    me.updateDragContext(context, event);
    if (!client) {
      return;
    }
    if (client !== me.currentOverClient) {
      me.onMouseOverNewTimeline(client);
    }
    //this.checkShiftChange();
    // Let product specific implementations trigger drag event (eventDrag, taskDrag)
    if (dd.dirty || !me.throttleDragEvent) {
      const valid = dd.valid;
      me.triggerEventDrag(dd, start);
      if (valid !== dd.valid) {
        dd.context.valid = dd.externalDragValidity = dd.valid;
      }
    }
    if (me.showTooltip && me.tip) {
      // If we've an error message to show, force the tip to be visible
      // even if the target is not in view.
      me.tip.lastAlignSpec.allowTargetOut = !dd.valid;
      me.tip.realign();
    }
  }
  onMouseOverNewTimeline(newTimeline, initial) {
    const me = this,
      {
        drag: {
          lockX,
          lockY
        }
      } = me,
      scrollables = [];
    me.currentOverClient.element.classList.remove('b-dragging-' + me.currentOverClient.scheduledEventName);
    newTimeline.element.classList.add('b-dragging-' + newTimeline.scheduledEventName);
    if (!initial) {
      me.currentOverClient.scrollManager.stopMonitoring();
    }
    if (!lockX) {
      scrollables.push({
        element: newTimeline.timeAxisSubGrid.scrollable.element,
        direction: 'horizontal'
      });
    }
    if (!lockY) {
      scrollables.push({
        element: newTimeline.scrollable.element,
        direction: 'vertical'
      });
    }
    newTimeline.scrollManager.startMonitoring({
      scrollables,
      callback: me.drag.onScrollManagerScrollCallback
    });
    me.currentOverClient = newTimeline;
  }
  triggerBeforeEventDropFinalize(eventType, eventData, client) {
    client.trigger(eventType, eventData);
  }
  /**
   * Triggered when dropping an event. Finalizes the operation.
   * @private
   */
  onDrop({
    context,
    event
  }) {
    var _me$tip;
    const me = this,
      {
        currentOverClient,
        dragData
      } = me;
    let modified = false;
    (_me$tip = me.tip) === null || _me$tip === void 0 ? void 0 : _me$tip.hide();
    context.valid = context.valid && me.isValidDrop(dragData);
    // If dropping outside scheduler, we opt in on DragHelper removing the proxy element
    me.drag.removeProxyAfterDrop = Boolean(dragData.externalDropTarget);
    if (context.valid && dragData.startDate && dragData.endDate) {
      let beforeDropTriggered = false;
      dragData.finalize = async valid => {
        if (beforeDropTriggered || dragData.async) {
          await me.finalize(valid);
        } else {
          // If user finalized operation synchronously in the beforeDropFinalize listener, just use
          // the valid param and carry on
          // but ignore it, if the context is already marked as invalid
          context.valid = context.valid && valid;
        }
      };
      me.triggerBeforeEventDropFinalize(`before${currentOverClient.capitalizedEventName}DropFinalize`, {
        context: dragData,
        domEvent: event
      }, currentOverClient);
      beforeDropTriggered = true;
      // Allow implementer to take control of the flow, by returning false from this listener,
      // to show a confirmation popup etc. This event is documented in EventDrag and TaskDrag
      context.async = dragData.async;
      // Internal validation, making sure all dragged records fit inside the view
      if (!context.async && !dragData.externalDropTarget) {
        modified = dragData.startDate - dragData.origStart !== 0 || dragData.newResource !== dragData.resourceRecord;
      }
    }
    if (!context.async) {
      me.finalize(dragData.valid && context.valid && modified);
    }
  }
  onDragAbort({
    context
  }) {
    var _me$tip2;
    const me = this;
    me.client.currentOrientation.onDragAbort({
      context,
      dragData: me.dragData
    });
    // otherwise the event disappears on next refresh (#62)
    me.resetDraggedElements();
    (_me$tip2 = me.tip) === null || _me$tip2 === void 0 ? void 0 : _me$tip2.hide();
    // Trigger eventDragAbort / taskDragAbort depending on product
    me.triggerDragAbort(me.dragData);
  }
  // Fired after any abort animation has completed (the point where we want to trigger redraw of progress lines etc)
  onDragAbortFinalized({
    context
  }) {
    var _me$client, _me$client2;
    const me = this;
    me.triggerDragAbortFinalized(me.dragData);
    // Hook for features that need to react on drag abort, used by NestedEvents
    (_me$client = (_me$client2 = me.client)[`after${me.client.capitalizedEventName}DragAbortFinalized`]) === null || _me$client === void 0 ? void 0 : _me$client.call(_me$client2, context, me.dragData);
  }
  // For the drag across multiple schedulers, tell all involved scroll managers to stop monitoring
  onDragReset({
    source: dragHelper
  }) {
    var _dragHelper$context;
    const me = this,
      currentTimeline = me.currentOverClient;
    currentTimeline === null || currentTimeline === void 0 ? void 0 : currentTimeline.scrollManager.stopMonitoring();
    if ((_dragHelper$context = dragHelper.context) !== null && _dragHelper$context !== void 0 && _dragHelper$context.started) {
      me.resetDraggedElements();
      currentTimeline.trigger(`${currentTimeline.scheduledEventName}DragReset`);
    }
    currentTimeline === null || currentTimeline === void 0 ? void 0 : currentTimeline.element.classList.remove('b-dragging-' + me.currentOverClient.scheduledEventName);
    me.dragData = null;
  }
  resetDraggedElements() {
    const {
        dragData
      } = this,
      {
        eventBarEls,
        draggedEntities
      } = dragData;
    this.resumeRecordElementRedrawing(dragData.record);
    draggedEntities.forEach((record, i) => {
      this.resumeRecordElementRedrawing(record);
      eventBarEls[i].classList.remove(this.drag.draggingCls);
      eventBarEls[i].retainElement = false;
    });
    // Code expects 1:1 ratio between eventBarEls & dragged assignments, but when dragging an event of a linked
    // resource that is not the case, and we need to clean up some more
    dragData.context.element.retainElement = false;
  }
  /**
   * Triggered internally on invalid drop.
   * @private
   */
  onInternalInvalidDrop(abort) {
    var _me$tip3;
    const me = this,
      {
        context
      } = me.drag;
    (_me$tip3 = me.tip) === null || _me$tip3 === void 0 ? void 0 : _me$tip3.hide();
    me.triggerAfterDrop(me.dragData, false);
    context.valid = false;
    if (abort) {
      me.drag.abort();
    }
  }
  //endregion
  //region Finalization & validation
  /**
   * Called on drop to update the record of the event being dropped.
   * @private
   * @param {Boolean} updateRecords Specify true to update the record, false to treat as invalid
   */
  async finalize(updateRecords) {
    const me = this,
      {
        dragData
      } = me;
    // Drag could've been aborted by window blur event. If it is aborted - we have nothing to finalize.
    if (!dragData || me.finalizing) {
      return;
    }
    const {
      context,
      draggedEntities,
      externalDropTarget
    } = dragData;
    let result;
    me.finalizing = true;
    draggedEntities.forEach((record, i) => {
      me.resumeRecordElementRedrawing(record);
      dragData.eventBarEls[i].classList.remove(me.drag.draggingCls);
      dragData.eventBarEls[i].retainElement = false;
    });
    // Code expects 1:1 ratio between eventBarEls & dragged assignments, but when dragging an event of a linked
    // resource that is not the case, and we need to clean up some more
    context.element.retainElement = false;
    if (externalDropTarget && dragData.valid || updateRecords) {
      // updateRecords may or may not be async.
      // We see if it returns a Promise.
      result = me.updateRecords(dragData);
      // If updateRecords is async, the calling DragHelper must know this and
      // go into a awaitingFinalization state.
      if (!externalDropTarget && Objects.isPromise(result)) {
        context.async = true;
        await result;
      }
      // If the finalize handler decided to change the dragData's validity...
      if (!dragData.valid) {
        me.onInternalInvalidDrop(true);
      } else {
        if (context.async) {
          context.finalize();
        }
        if (externalDropTarget) {
          // Force a refresh early so that removed events will not temporary be visible while engine is
          // recalculating (the row below clears the 'b-hidden' CSS class of the original drag element)
          me.client.refreshRows(false);
        }
        me.triggerAfterDrop(dragData, true);
      }
    } else {
      me.onInternalInvalidDrop(context.async || dragData.async);
    }
    me.finalizing = false;
    return result;
  }
  //endregion
  //region Drag data
  /**
   * Updates drag data's dates and validity (calls #validatorFn if specified)
   * @private
   */
  updateDragContext(info, event) {
    const me = this,
      {
        drag
      } = me,
      dd = me.dragData,
      client = me.currentOverClient,
      {
        isHorizontal
      } = client,
      [record] = dd.draggedEntities,
      eventRecord = record.isAssignment ? record.event : record,
      lastDragStartDate = dd.startDate,
      constrainToTimeSlot = me.constrainDragToTimeSlot || (isHorizontal ? drag.lockX : drag.lockY);
    dd.browserEvent = event;
    // getProductDragContext may switch valid flag, need to keep it here
    Object.assign(dd, me.getProductDragContext(dd));
    if (constrainToTimeSlot) {
      dd.timeDiff = 0;
    } else {
      let timeDiff;
      // Time diff is calculated differently for continuous and non-continuous time axis
      if (client.timeAxis.isContinuous) {
        const timeAxisPosition = client.isHorizontal ? info.pageX ?? info.startPageX : info.pageY ?? info.startPageY,
          // Use the localized coordinates to ask the TimeAxisViewModel what date the mouse is at.
          // Pass allowOutOfRange as true in case we have dragged out of either side of the timeline viewport.
          pointerDate = client.getDateFromCoordinate(timeAxisPosition, null, false, true);
        timeDiff = dd.timeDiff = pointerDate - info.pointerStartDate;
      } else {
        const range = me.resolveStartEndDates(info.element);
        // if dragging is out of timeAxis rect bounds, we will not be able to get dates
        dd.valid = Boolean(range.startDate && range.endDate);
        if (dd.valid) {
          timeDiff = range.startDate - dd.origStart;
        }
      }
      // If we got a time diff, we calculate new dates the same way no matter if it's continuous or not.
      // This prevents no-change drops in non-continuous time axis from being processed by updateAssignments()
      if (timeDiff !== null) {
        // calculate and round new startDate based on actual timeDiff
        dd.startDate = me.adjustStartDate(dd.origStart, timeDiff);
        dd.endDate = DateHelper.add(dd.startDate, eventRecord.fullDuration);
        if (dd.valid) {
          dd.timeDiff = dd.startDate - dd.origStart;
        }
      }
    }
    const positionDirty = dd.dirty = dd.dirty || lastDragStartDate - dd.startDate !== 0;
    if (dd.valid) {
      // If it's fully outside, we don't allow them to drop it - the event would disappear from their control.
      if (me.constrainDragToTimeline && (dd.endDate <= client.timeAxis.startDate || dd.startDate >= client.timeAxis.endDate)) {
        dd.valid = false;
        dd.context.message = me.L('L{EventDrag.noDropOutsideTimeline}');
      } else if (positionDirty || dd.externalDropTarget) {
        // Used to rely on faulty code above that would not be valid initially. With that changed we ignore
        // checking validity here on drag start, which is detected by not having a pageX
        const result = dd.externalDragValidity = !event || info.pageX && me.checkDragValidity(dd, event);
        if (!result || typeof result === 'boolean') {
          dd.valid = result !== false;
          dd.context.message = '';
        } else {
          dd.valid = result.valid !== false;
          dd.context.message = result.message;
        }
      } else {
        var _dd$externalDragValid;
        // Apply cached value from external drag validation
        dd.valid = dd.externalDragValidity !== false && ((_dd$externalDragValid = dd.externalDragValidity) === null || _dd$externalDragValid === void 0 ? void 0 : _dd$externalDragValid.valid) !== false;
      }
    } else {
      dd.valid = false;
    }
    dd.context.valid = dd.valid;
  }
  suspendRecordElementRedrawing(record, suspend = true) {
    this.suspendElementRedrawing(this.getRecordElement(record), suspend);
    record.instanceMeta(this.client).retainElement = suspend;
  }
  resumeRecordElementRedrawing(record) {
    this.suspendRecordElementRedrawing(record, false);
  }
  suspendElementRedrawing(element, suspend = true) {
    if (element) {
      element.retainElement = suspend;
    }
  }
  resumeElementRedrawing(element) {
    this.suspendElementRedrawing(element, false);
  }
  /**
   * Initializes drag data (dates, constraints, dragged events etc). Called when drag starts.
   * @private
   * @param info
   * @returns {*}
   */
  getDragData(info) {
    const me = this,
      {
        client,
        drag
      } = me,
      productDragData = me.setupProductDragData(info),
      {
        record,
        eventBarEls,
        draggedEntities
      } = productDragData,
      {
        startEvent
      } = drag,
      timespan = record.isAssignment ? record.event : record,
      origStart = timespan.startDate,
      origEnd = timespan.endDate,
      timeAxis = client.timeAxis,
      startsOutsideView = origStart < timeAxis.startDate,
      endsOutsideView = origEnd > timeAxis.endDate,
      multiSelect = client.isSchedulerBase ? client.multiEventSelect : client.selectionMode.multiSelect,
      coordinate = me.getCoordinate(timespan, info.element, [info.elementStartX, info.elementStartY]),
      clientCoordinate = me.getCoordinate(timespan, info.element, [info.startClientX, info.startClientY]);
    me.suspendRecordElementRedrawing(record);
    // prevent elements from being released when out of view
    draggedEntities.forEach(record => me.suspendRecordElementRedrawing(record));
    // Make sure the dragged event is selected (no-op for already selected)
    // Preserve other selected events if ctrl/meta is pressed
    if (record.isAssignment) {
      client.selectAssignment(record, startEvent.ctrlKey && multiSelect);
    } else {
      client.selectEvent(record, startEvent.ctrlKey && multiSelect);
    }
    const dragData = {
      context: info,
      ...productDragData,
      sourceDate: startsOutsideView ? origStart : client.getDateFromCoordinate(coordinate),
      screenSourceDate: client.getDateFromCoordinate(clientCoordinate, null, false),
      startDate: origStart,
      endDate: origEnd,
      timeDiff: 0,
      origStart,
      origEnd,
      startsOutsideView,
      endsOutsideView,
      duration: origEnd - origStart,
      browserEvent: startEvent // So we can know if SHIFT/CTRL was pressed
    };

    eventBarEls.forEach(el => el.classList.remove('b-sch-event-hover', 'b-active'));
    if (eventBarEls.length > 1) {
      // RelatedElements are secondary elements moved by the same delta as the grabbed element
      info.relatedElements = eventBarEls.slice(1);
    }
    return dragData;
  }
  //endregion
  //region Constraints
  // private
  setupConstraints(constrainRegion, elRegion, tickSize, constrained) {
    const me = this,
      xTickSize = !me.showExactDropPosition && tickSize > 1 ? tickSize : 0,
      yTickSize = 0;
    // If `constrained` is false then we have no date constraints and should constrain mouse position to scheduling area
    // else we have specified date constraints and so we should limit mouse position to smaller region inside of constrained region using offsets and width.
    if (constrained) {
      me.setXConstraint(constrainRegion.left, constrainRegion.right - elRegion.width, xTickSize);
    }
    // And if not constrained, release any constraints from the previous drag.
    else {
      // minX being true means allow the start to be before the time axis.
      // maxX being true means allow the end to be after the time axis.
      me.setXConstraint(true, true, xTickSize);
    }
    me.setYConstraint(constrainRegion.top, constrainRegion.bottom - elRegion.height, yTickSize);
  }
  updateYConstraint(eventRecord, resourceRecord) {
    const me = this,
      {
        client
      } = me,
      {
        context
      } = me.drag,
      tickSize = client.timeAxisViewModel.snapPixelAmount;
    // If we're dragging when the vertical size is recalculated by the host grid,
    // we must update our Y constraint unless we are locked in the Y axis.
    if (context && !me.drag.lockY) {
      let constrainRegion;
      // This calculates a relative region which the DragHelper uses within its outerElement
      if (me.constrainDragToTimeline) {
        constrainRegion = client.getScheduleRegion(resourceRecord, eventRecord);
      }
      // Not constraining to timeline.
      // Unusual configuration, but this must mean no Y constraining.
      else {
        me.setYConstraint(null, null, tickSize);
        return;
      }
      me.setYConstraint(constrainRegion.top, constrainRegion.bottom - context.element.offsetHeight, tickSize);
    } else {
      me.setYConstraint(null, null, tickSize);
    }
  }
  setXConstraint(iLeft, iRight, iTickSize) {
    const {
      drag
    } = this;
    drag.minX = iLeft;
    drag.maxX = iRight;
  }
  setYConstraint(iUp, iDown, iTickSize) {
    const {
      drag
    } = this;
    drag.minY = iUp;
    drag.maxY = iDown;
  }
  //endregion
  //region Other stuff
  adjustStartDate(startDate, timeDiff) {
    const rounded = this.client.timeAxis.roundDate(new Date(startDate - 0 + timeDiff), this.client.snapRelativeToEventStartDate ? startDate : false);
    return this.constrainStartDate(rounded);
  }
  resolveStartEndDates(draggedElement) {
    const timeline = this.currentOverClient,
      {
        timeAxis
      } = timeline,
      proxyRect = Rectangle.from(draggedElement.querySelector(timeline.eventInnerSelector), timeline.timeAxisSubGridElement),
      dd = this.dragData,
      [record] = dd.draggedEntities,
      eventRecord = record.isAssignment ? record.event : record,
      {
        fullDuration
      } = eventRecord,
      fillSnap = timeline.fillTicks && timeline.snapRelativeToEventStartDate;
    // Non-continuous time axis will return null instead of date for a rectangle outside of the view unless
    // told to estimate date.
    // When using fillTicks, we need exact dates for calculations below
    let {
      start: startDate,
      end: endDate
    } = timeline.getStartEndDatesFromRectangle(proxyRect, fillSnap ? null : 'round', fullDuration, true);
    // if dragging is out of timeAxis rect bounds, we will not be able to get dates
    if (startDate && endDate) {
      // When filling ticks, proxy start does not represent actual start date.
      // Need to compensate to get expected result
      if (fillSnap) {
        const
          // Events offset into the tick, in MS
          offsetMS = eventRecord.startDate - DateHelper.startOf(eventRecord.startDate, timeAxis.unit),
          // Proxy length in MS
          proxyMS = endDate - startDate,
          // Part of proxy that is "filled" and needs to be removed
          offsetPx = offsetMS / proxyMS * proxyRect.width;
        // Deflate top for vertical mode, left for horizontal mode
        proxyRect.deflate(offsetPx, 0, 0, offsetPx);
        const proxyStart = proxyRect.getStart(timeline.rtl, !timeline.isVertical);
        // Get date from offset proxy start
        startDate = timeline.getDateFromCoordinate(proxyStart, null, true);
        // Snap relative to event start date
        startDate = timeAxis.roundDate(startDate, eventRecord.startDate);
      }
      startDate = this.adjustStartDate(startDate, 0);
      if (!dd.startsOutsideView) {
        // Make sure we didn't target a start date that is filtered out, if we target last hour cell (e.g. 21:00) of
        // the time axis, and the next tick is 08:00 following day. Trying to drop at end of 21:00 cell should target start of next cell
        if (!timeAxis.dateInAxis(startDate, false)) {
          const tick = timeAxis.getTickFromDate(startDate);
          if (tick >= 0) {
            startDate = timeAxis.getDateFromTick(tick);
          }
        }
        endDate = startDate && DateHelper.add(startDate, fullDuration);
      } else if (!dd.endsOutsideView) {
        startDate = endDate && DateHelper.add(endDate, -fullDuration);
      }
    }
    return {
      startDate,
      endDate
    };
  }
  //endregion
  //region Dragtip
  /**
   * Gets html to display in tooltip while dragging event. Uses clockTemplate to display start & end dates.
   */
  getTipHtml() {
    const me = this,
      {
        dragData,
        client,
        tooltipTemplate
      } = me,
      {
        startDate,
        endDate,
        draggedEntities
      } = dragData,
      startText = client.getFormattedDate(startDate),
      endText = client.getFormattedEndDate(endDate, startDate),
      {
        valid,
        message,
        element,
        dragProxy
      } = dragData.context,
      tipTarget = dragProxy ? dragProxy.firstChild : element,
      dragged = draggedEntities[0],
      // Scheduler always drags assignments
      timeSpanRecord = dragged.isTask ? dragged : dragged.event;
    // Keep align target up to date in case of derendering the target when
    // dragged outside render window, and re-entry into the render window.
    me.tip.lastAlignSpec.target = tipTarget;
    return tooltipTemplate({
      valid,
      startDate,
      endDate,
      startText,
      endText,
      dragData,
      message: message || '',
      [client.scheduledEventName + 'Record']: timeSpanRecord,
      startClockHtml: me.clockTemplate.template({
        date: startDate,
        text: startText,
        cls: 'b-sch-tooltip-startdate'
      }),
      endClockHtml: timeSpanRecord.isMilestone ? '' : me.clockTemplate.template({
        date: endDate,
        text: endText,
        cls: 'b-sch-tooltip-enddate'
      })
    });
  }
  //endregion
  //region Configurable
  // Constrain to time slot means lockX if we're horizontal, otherwise lockY
  updateConstrainDragToTimeSlot(value) {
    const axis = this.client.isHorizontal ? 'lockX' : 'lockY';
    if (this.drag) {
      this.drag[axis] = value;
    }
  }
  // Constrain to resource means lockY if we're horizontal, otherwise lockX
  updateConstrainDragToResource(constrainDragToResource) {
    const me = this;
    if (me.drag) {
      const {
          constrainDragToTimeSlot
        } = me,
        {
          isHorizontal
        } = me.client;
      if (constrainDragToResource) {
        me.constrainDragToTimeline = true;
      }
      me.drag.lockY = isHorizontal ? constrainDragToResource : constrainDragToTimeSlot;
      me.drag.lockX = isHorizontal ? constrainDragToTimeSlot : constrainDragToResource;
    }
  }
  updateConstrainDragToTimeline(constrainDragToTimeline) {
    if (!this.isConfiguring) {
      Object.assign(this.drag, {
        cloneTarget: !constrainDragToTimeline,
        dragWithin: constrainDragToTimeline ? null : document.body,
        scrollManager: constrainDragToTimeline ? this.client.scrollManager : null
      });
    }
  }
  constrainStartDate(startDate) {
    const {
        dragData
      } = this,
      {
        dateConstraints
      } = dragData,
      scheduleableRecord = dragData.eventRecord || dragData.taskRecord || dragData.draggedEntities[0];
    if (dateConstraints !== null && dateConstraints !== void 0 && dateConstraints.start) {
      startDate = DateHelper.max(dateConstraints.start, startDate);
    }
    if (dateConstraints !== null && dateConstraints !== void 0 && dateConstraints.end) {
      startDate = DateHelper.min(new Date(dateConstraints.end - scheduleableRecord.durationMS), startDate);
    }
    return startDate;
  }
  //endregion
  //region Product specific, implemented in subclasses
  getElementFromContext(context) {
    return context.grabbed || context.dragProxy || context.element;
  }
  // Provide your custom implementation of this to allow additional selected records to be dragged together with the original one.
  getRelatedRecords(record) {
    return [];
  }
  getMinimalDragData(info, event) {
    // Can be overridden in subclass
    return {};
  }
  // Check if element can be dropped at desired location
  isValidDrop(dragData) {
    throw new Error('Implement in subclass');
  }
  // Similar to the fn above but also calls validatorFn
  checkDragValidity(dragData) {
    throw new Error('Implement in subclass');
  }
  // Update records being dragged
  updateRecords(context) {
    throw new Error('Implement in subclass');
  }
  // Determine if an element can be dragged
  isElementDraggable(el, event) {
    throw new Error('Implement in subclass');
  }
  // Get coordinate for correct axis
  getCoordinate(record, element, coord) {
    throw new Error('Implement in subclass');
  }
  // Product specific drag data
  setupProductDragData(info) {
    throw new Error('Implement in subclass');
  }
  // Product specific data in drag context
  getProductDragContext(dd) {
    throw new Error('Implement in subclass');
  }
  getRecordElement(record) {
    throw new Error('Implement in subclass');
  }
  //endregion
}

DragBase._$name = 'DragBase';

/**
 * @module Scheduler/feature/EventResize
 */
const tipAlign = {
  top: 'b-t',
  right: 'b100-t100',
  bottom: 't-b',
  left: 'b0-t0'
};
/**
 * Feature that allows resizing an event by dragging its end.
 *
 * By default it displays a tooltip with the new start and end dates, formatted using
 * {@link Scheduler/view/mixin/TimelineViewPresets#config-displayDateFormat}.
 *
 * ## Customizing the resize tooltip
 *
 * To show custom HTML in the tooltip, please see the {@link #config-tooltipTemplate} config. Example:
 *
 * ```javascript
 * eventResize : {
 *     // A minimal end date tooltip
 *     tooltipTemplate : ({ record, endDate }) => {
 *         return DateHelper.format(endDate, 'MMM D');
 *     }
 * }
 * ```
 *
 * This feature is **enabled** by default
 *
 * This feature is extended with a few overrides by the Gantt's `TaskResize` feature.
 *
 * This feature updates the event's `startDate` or `endDate` live in order to leverage the
 * rendering pathway to always yield a correct appearance. The changes are done in
 * {@link Core.data.Model#function-beginBatch batched} mode so that changes do not become
 * eligible for data synchronization or propagation until the operation is completed.
 *
 * @extends Core/mixin/InstancePlugin
 * @demo Scheduler/basic
 * @inlineexample Scheduler/feature/EventResize.js
 * @classtype eventResize
 * @feature
 */
class EventResize extends InstancePlugin.mixin(Draggable, Droppable) {
  //region Events
  /**
   * Fired on the owning Scheduler before resizing starts. Return `false` to prevent the action.
   * @event beforeEventResize
   * @on-owner
   * @preventable
   * @param {Scheduler.view.Scheduler} source Scheduler instance
   * @param {Scheduler.model.EventModel} eventRecord Event record being resized
   * @param {Scheduler.model.ResourceModel} resourceRecord Resource record the resize starts within
   * @param {MouseEvent} event Browser event
   */
  /**
   * Fires on the owning Scheduler when event resizing starts
   * @event eventResizeStart
   * @on-owner
   * @param {Scheduler.view.Scheduler} source Scheduler instance
   * @param {Scheduler.model.EventModel} eventRecord Event record being resized
   * @param {Scheduler.model.ResourceModel} resourceRecord Resource record the resize starts within
   * @param {MouseEvent} event Browser event
   */
  /**
   * Fires on the owning Scheduler on each resize move event
   * @event eventPartialResize
   * @on-owner
   * @param {Scheduler.view.Scheduler} source Scheduler instance
   * @param {Scheduler.model.EventModel} eventRecord Event record being resized
   * @param {Date} startDate
   * @param {Date} endDate
   * @param {HTMLElement} element
   */
  /**
   * Fired on the owning Scheduler to allow implementer to prevent immediate finalization by setting
   * `data.context.async = true` in the listener, to show a confirmation popup etc
   *
   * ```javascript
   *  scheduler.on('beforeeventresizefinalize', ({context}) => {
   *      context.async = true;
   *      setTimeout(() => {
   *          // async code don't forget to call finalize
   *          context.finalize();
   *      }, 1000);
   *  })
   * ```
   *
   * @event beforeEventResizeFinalize
   * @on-owner
   * @param {Scheduler.view.Scheduler} source Scheduler instance
   * @param {Object} context
   * @param {Scheduler.model.EventModel} context.eventRecord Event record being resized
   * @param {Date} context.startDate New startDate (changed if resizing start side)
   * @param {Date} context.endDate New endDate (changed if resizing end side)
   * @param {Date} context.originalStartDate Start date before resize
   * @param {Date} context.originalEndDate End date before resize
   * @param {Boolean} context.async Set true to handle resize asynchronously (e.g. to wait for user confirmation)
   * @param {Function} context.finalize Call this method to finalize resize. This method accepts one argument:
   *                   pass `true` to update records, or `false`, to ignore changes
   * @param {Event} event Browser event
   */
  /**
   * Fires on the owning Scheduler after the resizing gesture has finished.
   * @event eventResizeEnd
   * @on-owner
   * @param {Scheduler.view.Scheduler} source Scheduler instance
   * @param {Boolean} changed Shows if the record has been changed by the resize action
   * @param {Scheduler.model.EventModel} eventRecord Event record being resized
   */
  //endregion
  //region Config
  static get $name() {
    return 'EventResize';
  }
  static get configurable() {
    return {
      draggingItemCls: 'b-sch-event-wrap-resizing',
      resizingItemInnerCls: 'b-sch-event-resizing',
      /**
       * Use left handle when resizing. Only applies when owning client's `direction` is 'horizontal'
       * @config {Boolean}
       * @default
       */
      leftHandle: true,
      /**
       * Use right handle when resizing. Only applies when owning client's `direction` is 'horizontal'
       * @config {Boolean}
       * @default
       */
      rightHandle: true,
      /**
       * Use top handle when resizing. Only applies when owning client's direction` is 'vertical'
       * @config {Boolean}
       * @default
       */
      topHandle: true,
      /**
       * Use bottom handle when resizing. Only applies when owning client's `direction` is 'vertical'
       * @config {Boolean}
       * @default
       */
      bottomHandle: true,
      /**
       * Resizing handle size to use instead of that determined by CSS
       * @config {Number}
       * @deprecated Since 5.2.7. The handle size is determined from responsive CSS. Will be removed in 6.0
       */
      handleSize: null,
      /**
       * Automatically shrink virtual handles when available space < handleSize. The virtual handles will
       * decrease towards width/height 1, reserving space between opposite handles to for example leave room for
       * dragging. To configure reserved space, see {@link #config-reservedSpace}.
       * @config {Boolean}
       * @default false
       */
      dynamicHandleSize: true,
      /**
       * Set to true to allow resizing to a zero-duration span
       * @config {Boolean}
       * @default false
       */
      allowResizeToZero: null,
      /**
       * Room in px to leave unoccupied by handles when shrinking them dynamically (see
       * {@link #config-dynamicHandleSize}).
       * @config {Number}
       * @default
       */
      reservedSpace: 5,
      /**
       * Resizing handle size to use instead of that determined by CSS on touch devices
       * @config {Number}
       * @deprecated Since 5.2.7. The handle size is determined from responsive CSS. Will be removed in 6.0
       */
      touchHandleSize: null,
      /**
       * The amount of pixels to move pointer/mouse before it counts as a drag operation.
       * @config {Number}
       * @default
       */
      dragThreshold: 0,
      dragTouchStartDelay: 0,
      draggingClsSelector: '.b-timeline-base',
      /**
       * `false` to not show a tooltip while resizing
       * @config {Boolean}
       * @default
       */
      showTooltip: true,
      /**
       * true to see exact event length during resizing
       * @config {Boolean}
       * @default
       */
      showExactResizePosition: false,
      /**
       * An empty function by default, but provided so that you can perform custom validation on
       * the item being resized. Return true if the new duration is valid, false to signal that it is not.
       * @param {Object} context The resize context, contains the record & dates.
       * @param {Scheduler.model.TimeSpan} context.record The record being resized.
       * @param {Date} context.startDate The new start date.
       * @param {Date} context.endDate The new start date.
       * @param {Date} context.originalStartDate Start date before resize
       * @param {Date} context.originalEndDate End date before resize
       * @param {Event} event The browser Event object
       * @returns {Boolean}
       * @config {Function}
       */
      validatorFn: () => true,
      /**
       * `this` reference for the validatorFn
       * @config {Object}
       */
      validatorFnThisObj: null,
      /**
       * Setting this property may change the configuration of the {@link #config-tip}, or
       * cause it to be destroyed if `null` is passed.
       *
       * Reading this property returns the Tooltip instance.
       * @member {Core.widget.Tooltip|TooltipConfig} tip
       */
      /**
       * If a tooltip is required to illustrate the resize, specify this as `true`, or a config
       * object for the {@link Core.widget.Tooltip}.
       * @config {Core.widget.Tooltip|TooltipConfig}
       */
      tip: {
        $config: ['lazy', 'nullify'],
        value: {
          autoShow: false,
          axisLock: true,
          trackMouse: false,
          updateContentOnMouseMove: true,
          hideDelay: 0
        }
      },
      /**
       * A template function returning the content to show during a resize operation.
       * @param {Object} context A context object
       * @param {Date} context.startDate New start date
       * @param {Date} context.endDate New end date
       * @param {Scheduler.model.TimeSpan} context.record The record being resized
       * @config {Function} tooltipTemplate
       */
      tooltipTemplate: context => `
                <div class="b-sch-tip-${context.valid ? 'valid' : 'invalid'}">
                    ${context.startClockHtml}
                    ${context.endClockHtml}
                    <div class="b-sch-tip-message">${context.message}</div>
                </div>
            `,
      ignoreSelector: '.b-sch-terminal',
      dragActiveCls: 'b-resizing-event'
    };
  }
  static get pluginConfig() {
    return {
      chain: ['render', 'onEventDataGenerated', 'isEventElementDraggable']
    };
  }
  //endregion
  //region Init & destroy
  doDestroy() {
    var _this$dragging;
    super.doDestroy();
    (_this$dragging = this.dragging) === null || _this$dragging === void 0 ? void 0 : _this$dragging.destroy();
  }
  render() {
    const me = this,
      {
        client
      } = me;
    // Only active when in these items
    me.dragSelector = me.dragItemSelector = client.eventSelector;
    // Set up elements and listeners
    me.dragRootElement = me.dropRootElement = client.timeAxisSubGridElement;
    // Drag only in time dimension
    me.dragLock = client.isVertical ? 'y' : 'x';
  }
  // Prevent event dragging when it happens over a resize handle
  isEventElementDraggable(eventElement, eventRecord, el, event) {
    const me = this,
      eventResizable = eventRecord === null || eventRecord === void 0 ? void 0 : eventRecord.resizable;
    // ALLOW event drag:
    // - if resizing is disabled or event is not resizable
    // - if it's a milestone Milestones cannot be resized
    if (me.disabled || !eventResizable || eventRecord.isMilestone) {
      return true;
    }
    // not over the event handles
    return (eventResizable !== true && eventResizable !== 'start' || !me.isOverStartHandle(event, eventElement)) && (eventResizable !== true && eventResizable !== 'end' || !me.isOverEndHandle(event, eventElement));
  }
  // Called for each event during render, allows manipulation of render data.
  onEventDataGenerated({
    eventRecord,
    wrapperCls,
    cls
  }) {
    var _this$dragging2, _this$dragging2$conte;
    if (eventRecord === ((_this$dragging2 = this.dragging) === null || _this$dragging2 === void 0 ? void 0 : (_this$dragging2$conte = _this$dragging2.context) === null || _this$dragging2$conte === void 0 ? void 0 : _this$dragging2$conte.eventRecord)) {
      wrapperCls['b-active'] = wrapperCls[this.draggingItemCls] = wrapperCls['b-over-resize-handle'] = cls['b-resize-handle'] = cls[this.resizingItemInnerCls] = 1;
    }
  }
  // Sneak a first peek at the drag event to put necessary date values into the context
  onDragPointerMove(event) {
    var _dragging$context;
    const {
        client,
        dragging
      } = this,
      {
        visibleDateRange,
        isHorizontal
      } = client,
      rtl = isHorizontal && client.rtl,
      dimension = isHorizontal ? 'X' : 'Y',
      pageScroll = globalThis[`page${dimension}Offset`],
      coord = event[`page${dimension}`] + (((_dragging$context = dragging.context) === null || _dragging$context === void 0 ? void 0 : _dragging$context.offset) || 0),
      clientRect = Rectangle.from(client.timeAxisSubGridElement, null, true),
      startCoord = clientRect.getStart(rtl, isHorizontal),
      endCoord = clientRect.getEnd(rtl, isHorizontal);
    let date = client.getDateFromCoord({
      coord,
      local: false
    });
    if (rtl) {
      // If we're dragging off the start side, fix at the visible startDate
      if (coord - pageScroll > startCoord) {
        date = visibleDateRange.startDate;
      }
      // If we're dragging off the end side, fix at the visible endDate
      else if (coord < endCoord) {
        date = visibleDateRange.endDate;
      }
    }
    // If we're dragging off the start side, fix at the visible startDate
    else if (coord - pageScroll < startCoord) {
      date = visibleDateRange.startDate;
    }
    // If we're dragging off the end side, fix at the visible endDate
    else if (coord - pageScroll > endCoord) {
      date = visibleDateRange.endDate;
    }
    dragging.clientStartCoord = startCoord;
    dragging.clientEndCoord = endCoord;
    dragging.date = date;
    super.onDragPointerMove(event);
  }
  /**
   * Returns true if a resize operation is active
   * @property {Boolean}
   * @readonly
   */
  get isResizing() {
    return Boolean(this.dragging);
  }
  beforeDrag(drag) {
    const {
        client
      } = this,
      eventRecord = client.resolveTimeSpanRecord(drag.itemElement),
      resourceRecord = !client.isGanttBase && client.resolveResourceRecord(client.isVertical ? drag.startEvent : drag.itemElement);
    // Events not part of project are transient records in a Gantt display store and not meant to be modified
    if (this.disabled || client.readOnly || resourceRecord !== null && resourceRecord !== void 0 && resourceRecord.readOnly || eventRecord && (eventRecord.readOnly || !(eventRecord.project || eventRecord.isOccurrence)) || super.beforeDrag(drag) === false) {
      return false;
    }
    drag.mousedownDate = drag.date = client.getDateFromCoordinate(drag.event[`page${client.isHorizontal ? 'X' : 'Y'}`], null, false);
    // trigger beforeEventResize or beforeTaskResize depending on product
    return this.triggerBeforeResize(drag);
  }
  dragStart(drag) {
    var _client$features$even, _client$resolveAssign;
    const me = this,
      {
        client,
        tip
      } = me,
      {
        startEvent,
        itemElement
      } = drag,
      name = client.scheduledEventName,
      eventRecord = client.resolveEventRecord(itemElement),
      {
        isBatchUpdating,
        wrapStartDate,
        wrapEndDate
      } = eventRecord,
      useEventBuffer = (_client$features$even = client.features.eventBuffer) === null || _client$features$even === void 0 ? void 0 : _client$features$even.enabled,
      eventStartDate = isBatchUpdating ? eventRecord.get('startDate') : eventRecord.startDate,
      eventEndDate = isBatchUpdating ? eventRecord.get('endDate') : eventRecord.endDate,
      horizontal = me.dragLock === 'x',
      rtl = horizontal && client.rtl,
      draggingEnd = me.isOverEndHandle(startEvent, itemElement),
      toSet = draggingEnd ? 'endDate' : 'startDate',
      wrapToSet = !useEventBuffer ? null : draggingEnd ? 'wrapEndDate' : 'wrapStartDate',
      otherEnd = draggingEnd ? 'startDate' : 'endDate',
      setMethod = draggingEnd ? 'setEndDate' : 'setStartDate',
      setOtherMethod = draggingEnd ? 'setStartDate' : 'setEndDate',
      elRect = Rectangle.from(itemElement),
      startCoord = horizontal ? startEvent.clientX : startEvent.clientY,
      endCoord = draggingEnd ? elRect.getEnd(rtl, horizontal) : elRect.getStart(rtl, horizontal),
      context = drag.context = {
        eventRecord,
        element: itemElement,
        timespanRecord: eventRecord,
        taskRecord: eventRecord,
        owner: me,
        valid: true,
        oldValue: draggingEnd ? eventEndDate : eventStartDate,
        startDate: eventStartDate,
        endDate: eventEndDate,
        offset: useEventBuffer ? 0 : endCoord - startCoord,
        edge: horizontal ? draggingEnd ? 'right' : 'left' : draggingEnd ? 'bottom' : 'top',
        finalize: me.finalize,
        event: drag.event,
        // these two are public
        originalStartDate: eventStartDate,
        originalEndDate: eventEndDate,
        wrapStartDate,
        wrapEndDate,
        draggingEnd,
        toSet,
        wrapToSet,
        otherEnd,
        setMethod,
        setOtherMethod
      };
    // The record must know that it is being resized.
    eventRecord.meta.isResizing = true;
    client.element.classList.add(...me.dragActiveCls.split(' '));
    // During this batch we want the client's UI to update itself using the proposed changes
    // Only if startDrag has not already done it
    if (!client.listenToBatchedUpdates) {
      client.beginListeningForBatchedUpdates();
    }
    // No changes must get through to data.
    // Only if startDrag has not already started the batch
    if (!isBatchUpdating) {
      me.beginEventRecordBatch(eventRecord);
    }
    // Let products do their specific stuff
    me.setupProductResizeContext(context, startEvent);
    // Trigger eventResizeStart or taskResizeStart depending on product
    // Subclasses (like EventDragCreate) won't actually fire this event.
    me.triggerEventResizeStart(`${name}ResizeStart`, {
      [`${name}Record`]: eventRecord,
      event: startEvent,
      ...me.getResizeStartParams(context)
    }, context);
    // Scheduler renders assignments, Gantt renders Tasks
    context.resizedRecord = ((_client$resolveAssign = client.resolveAssignmentRecord) === null || _client$resolveAssign === void 0 ? void 0 : _client$resolveAssign.call(client, context.element)) || eventRecord;
    if (tip) {
      // Tip needs to be shown first for getTooltipTarget to be able to measure anchor size
      tip.show();
      tip.align = tipAlign[context.edge];
      tip.showBy(me.getTooltipTarget(drag));
    }
  }
  // Subclasses may override this
  triggerBeforeResize(drag) {
    const {
        client
      } = this,
      eventRecord = client.resolveTimeSpanRecord(drag.itemElement);
    return client.trigger(`before${client.capitalizedEventName}Resize`, {
      [`${client.scheduledEventName}Record`]: eventRecord,
      event: drag.event,
      ...this.getBeforeResizeParams({
        event: drag.startEvent,
        element: drag.itemElement
      })
    });
  }
  // Subclasses may override this
  triggerEventResizeStart(eventType, event, context) {
    var _this$client, _this$client2;
    this.client.trigger(eventType, event);
    // Hook for features that needs to react on resize start
    (_this$client = (_this$client2 = this.client)[`after${StringHelper.capitalize(eventType)}`]) === null || _this$client === void 0 ? void 0 : _this$client.call(_this$client2, context, event);
  }
  triggerEventResizeEnd(eventType, event) {
    this.client.trigger(eventType, event);
  }
  triggerEventPartialResize(eventType, event) {
    // Trigger eventPartialResize or taskPartialResize depending on product
    this.client.trigger(eventType, event);
  }
  triggerBeforeEventResizeFinalize(eventType, event) {
    this.client.trigger(eventType, event);
  }
  dragEnter(drag) {
    var _drag$context;
    // We only respond to our own DragContexts
    return ((_drag$context = drag.context) === null || _drag$context === void 0 ? void 0 : _drag$context.owner) === this;
  }
  resizeEventPartiallyInternal(eventRecord, context) {
    var _client$features$even2;
    const {
        client
      } = this,
      {
        toSet
      } = context;
    if ((_client$features$even2 = client.features.eventBuffer) !== null && _client$features$even2 !== void 0 && _client$features$even2.enabled) {
      if (toSet === 'startDate') {
        const diff = context.startDate.getTime() - context.originalStartDate.getTime();
        eventRecord.wrapStartDate = new Date(context.wrapStartDate.getTime() + diff);
      } else if (toSet === 'endDate') {
        const diff = context.endDate.getTime() - context.originalEndDate.getTime();
        eventRecord.wrapEndDate = new Date(context.wrapEndDate.getTime() + diff);
      }
    }
    eventRecord.set(toSet, context[toSet]);
  }
  applyDateConstraints(date, eventRecord, context) {
    var _context$dateConstrai, _context$dateConstrai2;
    const minDate = (_context$dateConstrai = context.dateConstraints) === null || _context$dateConstrai === void 0 ? void 0 : _context$dateConstrai.start,
      maxDate = (_context$dateConstrai2 = context.dateConstraints) === null || _context$dateConstrai2 === void 0 ? void 0 : _context$dateConstrai2.end;
    // Keep desired date within constraints
    if (minDate || maxDate) {
      date = DateHelper.constrain(date, minDate, maxDate);
      context.snappedDate = DateHelper.constrain(context.snappedDate, minDate, maxDate);
    }
    return date;
  }
  // Override the draggable interface so that we can update the bar while dragging outside
  // the Draggable's rootElement (by default it stops notifications when outside rootElement)
  moveDrag(drag) {
    const me = this,
      {
        client,
        tip
      } = me,
      horizontal = me.dragLock === 'x',
      dimension = horizontal ? 'X' : 'Y',
      name = client.scheduledEventName,
      {
        visibleDateRange,
        enableEventAnimations,
        timeAxis,
        weekStartDay
      } = client,
      rtl = horizontal && client.rtl,
      {
        resolutionUnit,
        resolutionIncrement
      } = timeAxis,
      {
        event,
        context
      } = drag,
      {
        eventRecord
      } = context,
      offset = context.offset * (rtl ? -1 : 1),
      {
        isOccurrence
      } = eventRecord,
      eventStart = eventRecord.get('startDate'),
      eventEnd = eventRecord.get('endDate'),
      coord = event[`client${dimension}`] + offset,
      clientRect = Rectangle.from(client.timeAxisSubGridElement, null, true),
      startCoord = clientRect.getStart(rtl, horizontal),
      endCoord = clientRect.getEnd(rtl, horizontal);
    context.event = event;
    // If this is the last move event recycled because of a scroll, refresh the date
    if (event.isScroll) {
      drag.date = client.getDateFromCoordinate(event[`page${dimension}`] + offset, null, false);
    }
    let crossedOver,
      avoidedZeroSize,
      // Use the value set up in onDragPointerMove by default
      {
        date
      } = drag,
      {
        toSet,
        otherEnd,
        draggingEnd
      } = context;
    if (rtl) {
      // If we're dragging off the start side, fix at the visible startDate
      if (coord > startCoord) {
        date = drag.date = visibleDateRange.startDate;
      }
      // If we're dragging off the end side, fix at the visible endDate
      else if (coord < endCoord) {
        date = drag.date = visibleDateRange.endDate;
      }
    }
    // If we're dragging off the start side, fix at the visible startDate
    else if (coord < startCoord) {
      date = drag.date = visibleDateRange.startDate;
    }
    // If we're dragging off the end side, fix at the visible endDate
    else if (coord > endCoord) {
      date = drag.date = visibleDateRange.endDate;
    }
    // Detect crossover which some subclasses might need to process
    if (toSet === 'endDate') {
      if (date < eventStart) {
        crossedOver = -1;
      }
    } else {
      if (date > eventEnd) {
        crossedOver = 1;
      }
    }
    // If we dragged the dragged end over to the opposite side of the start end.
    // Some subclasses allow this and need to respond. EventDragCreate does this.
    if (crossedOver && me.onDragEndSwitch) {
      me.onDragEndSwitch(context, date, crossedOver);
      otherEnd = context.otherEnd;
      toSet = context.toSet;
    }
    if (client.snapRelativeToEventStartDate) {
      date = timeAxis.roundDate(date, context.oldValue);
    }
    // The displayed and eventual data value
    context.snappedDate = DateHelper.round(date, timeAxis.resolution, null, weekStartDay);
    const duration = DateHelper.diff(date, context[otherEnd], resolutionUnit) * (draggingEnd ? -1 : 1);
    // Narrower than half resolutionIncrement will abort drag creation, set flag to have UI reflect this
    if (me.isEventDragCreate) {
      context.tooNarrow = duration < resolutionIncrement / 2;
    }
    // The mousepoint date means that the duration is less than resolutionIncrement resolutionUnits.
    // Ensure that the dragged end is at least resolutionIncrement resolutionUnits from the other end.
    else if (duration < resolutionIncrement) {
      // Snap to zero if allowed
      if (me.allowResizeToZero) {
        context.snappedDate = date = context[otherEnd];
      } else {
        const sign = otherEnd === 'startDate' ? 1 : -1;
        context.snappedDate = date = timeAxis.roundDate(DateHelper.add(eventRecord.get(otherEnd), resolutionIncrement * sign, resolutionUnit));
        avoidedZeroSize = true;
      }
    }
    // take dateConstraints into account
    date = me.applyDateConstraints(date, eventRecord, context);
    // If the mouse move has changed the detected date
    if (!context.date || date - context.date || avoidedZeroSize) {
      context.date = date;
      // The validityFn needs to see the proposed value.
      // Consult our snap config to see if we should be dragging in snapped mode
      context[toSet] = me.showExactResizePosition || client.timeAxisViewModel.snap ? context.snappedDate : date;
      // Snapping would take it to zero size - this is not allowed in drag resizing.
      context.valid = me.allowResizeToZero || context[toSet] - context[toSet === 'startDate' ? 'endDate' : 'startDate'] !== 0;
      // If the date to push into the record is new...
      if (eventRecord.get(toSet) - context[toSet]) {
        context.valid = me.checkValidity(context, event);
        context.message = '';
        if (context.valid && typeof context.valid !== 'boolean') {
          context.message = context.valid.message;
          context.valid = context.valid.valid;
        }
        // If users returns nothing, that's interpreted as valid
        context.valid = context.valid !== false;
        if (context.valid) {
          const partialResizeEvent = {
            [`${name}Record`]: eventRecord,
            startDate: eventStart,
            endDate: eventEnd,
            element: drag.itemElement,
            context
          };
          // Update the event we are about to fire and the context *before* we update the record
          partialResizeEvent[toSet] = context[toSet];
          // Trigger eventPartialResize or taskPartialResize depending on product
          me.triggerEventPartialResize(`${name}PartialResize`, partialResizeEvent);
          // An occurrence must have a store to announce its batched changes through.
          // They must usually never have a store - they are transient, but we
          // need to update the UI.
          if (isOccurrence) {
            eventRecord.stores.push(client.eventStore);
          }
          // Update the eventRecord.
          // Use setter rather than accessor so that in a Project, the entity's
          // accessor doesn't propagate the change to the whole project.
          // Scheduler must not animate this.
          client.enableEventAnimations = false;
          this.resizeEventPartiallyInternal(eventRecord, context);
          client.enableEventAnimations = enableEventAnimations;
          if (isOccurrence) {
            eventRecord.stores.length = 0;
          }
        }
        // Flag drag created too narrow events as invalid late, want all code above to execute for them
        // to get the proper size rendered
        if (context.tooNarrow) {
          context.valid = false;
        }
      }
    }
    if (tip) {
      // In case of edge flip (EventDragCreate), the align point may change
      tip.align = tipAlign[context.edge];
      tip.alignTo(me.getTooltipTarget(drag));
    }
    super.moveDrag(drag);
  }
  dragEnd(drag) {
    const {
      context
    } = drag;
    if (context) {
      context.event = drag.event;
    }
    if (drag.aborted) {
      context === null || context === void 0 ? void 0 : context.finalize(false);
    }
    // 062_resize.t.js specifies that if drag was not started but the mouse has moved,
    // the eventresizestart and eventresizeend must fire
    else if (!this.isEventDragCreate && !drag.started && !EventHelper.getPagePoint(drag.event).equals(EventHelper.getPagePoint(drag.startEvent))) {
      this.dragStart(drag);
      this.cleanup(drag.context, false);
    }
  }
  async dragDrop({
    context,
    event
  }) {
    var _this$tip;
    // Set the start/end date, whichever we were dragging
    // to the correctly rounded value before updating.
    context[context.toSet] = context.snappedDate;
    const {
        client
      } = this,
      {
        startDate,
        endDate
      } = context;
    let modified;
    (_this$tip = this.tip) === null || _this$tip === void 0 ? void 0 : _this$tip.hide();
    context.valid = startDate && endDate && (this.allowResizeToZero || endDate - startDate > 0) &&
    // Input sanity check
    context[context.toSet] - context.oldValue &&
    // Make sure dragged end changed
    context.valid !== false;
    if (context.valid) {
      // Seems to be a valid resize operation, ask outside world if anyone wants to take control over the finalizing,
      // to show a confirm dialog prior to applying the new values. Triggers beforeEventResizeFinalize or
      // beforeTaskResizeFinalize depending on product
      this.triggerBeforeEventResizeFinalize(`before${client.capitalizedEventName}ResizeFinalize`, {
        context,
        event,
        [`${client.scheduledEventName}Record`]: context.eventRecord
      });
      modified = true;
    }
    // If a handler has set the async flag, it means that they are going to finalize
    // the operation at some time in the future, so we should not call it.
    if (!context.async) {
      await context.finalize(modified);
    }
  }
  // This is called with a thisObj of the context object
  // We set "me" to the owner, and "context" to the thisObj so that it
  // reads as if it were a method of this class.
  async finalize(updateRecord) {
    const me = this.owner,
      context = this,
      {
        eventRecord,
        oldValue,
        toSet
      } = context,
      {
        snapRelativeToEventStartDate,
        timeAxis
      } = me.client;
    let wasChanged = false;
    if (updateRecord) {
      if (snapRelativeToEventStartDate) {
        context[toSet] = context.snappedDate = timeAxis.roundDate(context.date, oldValue);
      }
      // Each product updates the record differently
      wasChanged = await me.internalUpdateRecord(context, eventRecord);
    } else {
      // Reverts the changes, a batchedUpdate event will fire which will reset the UI
      me.cancelEventRecordBatch(eventRecord);
      // Manually trigger redraw of occurrences since they are not part of any stores
      if (eventRecord.isOccurrence) {
        eventRecord.resources.forEach(resource => me.client.repaintEventsForResource(resource));
      }
    }
    me.cleanup(context, wasChanged);
  }
  // This is always called on drop or abort.
  cleanup(context, changed) {
    var _me$tip;
    const me = this,
      {
        client
      } = me,
      {
        element,
        eventRecord
      } = context,
      name = client.scheduledEventName;
    // The record must know that it is being resized.
    eventRecord.meta.isResizing = false;
    client.endListeningForBatchedUpdates();
    (_me$tip = me.tip) === null || _me$tip === void 0 ? void 0 : _me$tip.hide();
    me.unHighlightHandle(element);
    client.element.classList.remove(...me.dragActiveCls.split(' '));
    // if (dependencies) {
    //     // When resizing is done and mouse is over element, we show terminals
    //     if (element.matches(':hover')) {
    //         dependencies.showTerminals(eventRecord, element);
    //     }
    // }
    // Triggers eventResizeEnd or taskResizeEnd depending on product
    me.triggerEventResizeEnd(`${name}ResizeEnd`, {
      changed,
      [`${name}Record`]: eventRecord,
      ...me.getResizeEndParams(context)
    });
  }
  async internalUpdateRecord(context, timespanRecord) {
    const {
        client
      } = this,
      {
        generation
      } = timespanRecord;
    // Special handling of occurrences, they need normalization since that is not handled by engine at the moment
    if (timespanRecord.isOccurrence) {
      client.endListeningForBatchedUpdates();
      // If >1 level deep, just unwind one level.
      timespanRecord[timespanRecord.batching > 1 ? 'endBatch' : 'cancelBatch']();
      timespanRecord.set(TimeSpan.prototype.inSetNormalize.call(timespanRecord, {
        startDate: context.startDate,
        endDate: context.endDate
      }));
    } else {
      const toSet = {
        [context.toSet]: context[context.toSet]
      };
      // If we have the Engine available, consult it to calculate a corrected duration.
      // Adjust the dragged date point to conform with the calculated duration.
      if (timespanRecord.isEntity) {
        var _client$features$even3;
        const {
          startDate,
          endDate,
          draggingEnd
        } = context;
        // Fix the duration according to the Entity's rules.
        context.duration = toSet.duration = timespanRecord.run('calculateProjectedDuration', startDate, endDate);
        // Fix the dragged date point according to the Entity's rules.
        toSet[context.toSet] = timespanRecord.run('calculateProjectedXDateWithDuration', draggingEnd ? startDate : endDate, draggingEnd, context.duration);
        const setOtherEnd = !timespanRecord[context.otherEnd];
        // Set all values, start and end in case they had never been set
        // ie, we're now scheduling a previously unscheduled event.
        if (setOtherEnd) {
          toSet[context.otherEnd] = context[context.otherEnd];
        }
        // Update the record to its final correct state using *batched changes*
        // These will *not* be propagated, it's just to force the dragged event bar
        // into its corrected shape before the real changes which will propagate are applied below.
        // We MUST do it like this because the final state may not be a net change if the changes
        // got rejected, and in that case, the engine will not end up firing any change events.
        timespanRecord.set(toSet);
        // Quit listening for batchedUpdate *before* we cancel the batch so that the
        // change events from the revert do not update the UI.
        client.endListeningForBatchedUpdates();
        this.cancelEventRecordBatch(timespanRecord);
        // Clear estimated wrap date, exact wrap date will be calculated when referred to from renderer
        if ((_client$features$even3 = client.features.eventBuffer) !== null && _client$features$even3 !== void 0 && _client$features$even3.enabled) {
          timespanRecord[context.wrapToSet] = null;
        }
        const promisesToWait = [];
        // Really update the data after cancelling the batch
        if (setOtherEnd) {
          promisesToWait.push(timespanRecord[context.setOtherMethod](toSet[context.otherEnd], false));
        }
        promisesToWait.push(timespanRecord[context.setMethod](toSet[context.toSet], false));
        await Promise.all(promisesToWait);
        timespanRecord.endBatch();
      } else {
        // Collect any changes (except the start/end date) that happened during the resize operation
        const batchChanges = Object.assign({}, timespanRecord.meta.batchChanges);
        delete batchChanges[context.toSet];
        client.endListeningForBatchedUpdates();
        this.cancelEventRecordBatch(timespanRecord);
        timespanRecord.set(batchChanges);
        timespanRecord[context.setMethod](toSet[context.toSet], false);
      }
    }
    // wait for project data update
    await client.project.commitAsync();
    // If the record has been changed
    return timespanRecord.generation !== generation;
  }
  onDragItemMouseMove(event) {
    if (event.pointerType !== 'touch' && !this.handleSelector) {
      this.checkResizeHandles(event);
    }
  }
  /**
   * Check if mouse is over a resize handle (virtual). If so, highlight.
   * @private
   * @param {MouseEvent} event
   */
  checkResizeHandles(event) {
    const me = this,
      {
        overItem
      } = me;
    // mouse over a target element and allowed to resize?
    if (overItem && !me.client.readOnly && (!me.allowResize || me.allowResize(overItem, event))) {
      const eventRecord = me.client.resolveTimeSpanRecord(overItem);
      if (eventRecord !== null && eventRecord !== void 0 && eventRecord.readOnly) {
        return;
      }
      if (me.isOverAnyHandle(event, overItem)) {
        me.highlightHandle(); // over handle
      } else {
        me.unHighlightHandle(); // not over handle
      }
    }
  }

  onDragItemMouseLeave(event, oldOverItem) {
    this.unHighlightHandle(oldOverItem);
  }
  /**
   * Highlights handles (applies css that changes cursor).
   * @private
   */
  highlightHandle() {
    var _item$syncIdMap;
    const {
        overItem: item,
        client
      } = this,
      handleTargetElement = ((_item$syncIdMap = item.syncIdMap) === null || _item$syncIdMap === void 0 ? void 0 : _item$syncIdMap[client.scheduledEventName]) ?? item.querySelector(client.eventInnerSelector);
    // over a handle, add cls to change cursor
    handleTargetElement.classList.add('b-resize-handle');
    item.classList.add('b-over-resize-handle');
  }
  /**
   * Unhighlight handles (removes css).
   * @private
   */
  unHighlightHandle(item = this.overItem) {
    if (item) {
      var _item$syncIdMap2;
      const me = this,
        inner = ((_item$syncIdMap2 = item.syncIdMap) === null || _item$syncIdMap2 === void 0 ? void 0 : _item$syncIdMap2[me.client.scheduledEventName]) ?? item.querySelector(me.client.eventInnerSelector);
      if (inner) {
        inner.classList.remove('b-resize-handle', me.resizingItemInnerCls);
      }
      item.classList.remove('b-over-resize-handle', me.draggingItemCls);
    }
  }
  isOverAnyHandle(event, target) {
    return this.isOverStartHandle(event, target) || this.isOverEndHandle(event, target);
  }
  isOverStartHandle(event, target) {
    var _this$getHandleRect;
    return (_this$getHandleRect = this.getHandleRect('start', event, target)) === null || _this$getHandleRect === void 0 ? void 0 : _this$getHandleRect.contains(EventHelper.getPagePoint(event));
  }
  isOverEndHandle(event, target) {
    var _this$getHandleRect2;
    return (_this$getHandleRect2 = this.getHandleRect('end', event, target)) === null || _this$getHandleRect2 === void 0 ? void 0 : _this$getHandleRect2.contains(EventHelper.getPagePoint(event));
  }
  getHandleRect(side, event, eventEl) {
    if (this.overItem) {
      eventEl = event.target.closest(`.${this.client.eventCls}`) || eventEl.querySelector(`.${this.client.eventCls}`);
      if (!eventEl) {
        return;
      }
      const me = this,
        start = side === 'start',
        {
          client
        } = me,
        rtl = Boolean(client.rtl),
        axis = me.dragLock,
        horizontal = axis === 'x',
        dim = horizontal ? 'width' : 'height',
        handleSpec = `${horizontal ? start && !rtl ? 'left' : 'right' : start ? 'top' : 'bottom'}Handle`,
        {
          offsetWidth
        } = eventEl,
        timespanRecord = client.resolveTimeSpanRecord(eventEl),
        resizable = timespanRecord === null || timespanRecord === void 0 ? void 0 : timespanRecord.isResizable,
        eventRect = Rectangle.from(eventEl),
        result = eventRect.clone(),
        handleStyle = globalThis.getComputedStyle(eventEl, ':before'),
        // Larger draggable zones on pure touch devices with no mouse
        touchHandleSize = !me.handleSelector && !BrowserHelper.isHoverableDevice ? me.touchHandleSize : undefined,
        handleSize = touchHandleSize || me.handleSize || parseFloat(handleStyle[dim]),
        handleVisThresh = me.handleVisibilityThreshold || 2 * me.handleSize,
        centerGap = me.dynamicHandleSize ? me.reservedSpace / 2 : 0,
        deflateArgs = [0, 0, 0, 0];
      // To decide if we are over a valid handle, we first check disabled state
      // Then this.leftHandle/this.rightHandle/this.topHandle/this.bottomHandle
      // Then whether there's enough event bar width to accommodate separate handles
      // Then whether the event itself allows resizing at the specified side.
      if (!me.disabled && me[handleSpec] && (offsetWidth >= handleVisThresh || me.dynamicHandleSize) && (resizable === true || resizable === side)) {
        const oppositeEnd = !horizontal && !start || horizontal && rtl === start;
        if (oppositeEnd) {
          // Push handle start point to other end and clip result to other end
          result[axis] += eventRect[dim] - handleSize;
          deflateArgs[horizontal ? 3 : 0] = eventRect[dim] / 2 + centerGap;
        } else {
          deflateArgs[horizontal ? 1 : 2] = eventRect[dim] / 2 + centerGap;
        }
        // Deflate the event bar rectangle to encapsulate 2px less than the side's own half
        // so that we can constrain the handle zone to be inside its own half when bar is small.
        eventRect.deflate(...deflateArgs);
        result[dim] = handleSize;
        // Constrain handle rectangles to each side so that they can never collide.
        // Each handle is constrained into its own half.
        result.constrainTo(eventRect);
        // Zero sized handles cannot be hovered
        if (result[dim]) {
          return result;
        }
      }
    }
  }
  setupDragContext(event) {
    const me = this;
    // Only start a drag if we are over a handle zone.
    if (me.overItem && me.isOverAnyHandle(event, me.overItem) && me.isElementResizable(me.overItem, event)) {
      const result = super.setupDragContext(event);
      result.scrollManager = me.client.scrollManager;
      return result;
    }
  }
  changeHandleSize() {
    VersionHelper.deprecate('Scheduler', '6.0.0', 'Handle size is from CSS');
  }
  changeTouchHandleSize() {
    VersionHelper.deprecate('Scheduler', '6.0.0', 'Handle size is from CSS');
  }
  changeTip(tip, oldTip) {
    const me = this;
    if (!me.showTooltip) {
      return null;
    }
    if (tip) {
      if (tip.isTooltip) {
        tip.owner = me;
      } else {
        tip = Tooltip.reconfigure(oldTip, Tooltip.mergeConfigs({
          id: me.tipId
        }, tip, {
          getHtml: me.getTipHtml.bind(me),
          owner: me.client
        }, me.tip), {
          owner: me,
          defaults: {
            type: 'tooltip'
          }
        });
      }
      tip.ion({
        innerhtmlupdate: 'updateDateIndicator',
        thisObj: me
      });
      me.clockTemplate = new ClockTemplate({
        scheduler: me.client
      });
    } else if (oldTip) {
      var _me$clockTemplate;
      oldTip.destroy();
      (_me$clockTemplate = me.clockTemplate) === null || _me$clockTemplate === void 0 ? void 0 : _me$clockTemplate.destroy();
    }
    return tip;
  }
  //endregion
  //region Events
  isElementResizable(element, event) {
    var _element;
    const me = this,
      {
        client
      } = me,
      timespanRecord = client.resolveTimeSpanRecord(element);
    if (client.readOnly) {
      return false;
    }
    let resizable = timespanRecord === null || timespanRecord === void 0 ? void 0 : timespanRecord.isResizable;
    // Not resizable if the mousedown is on a resizing handle of
    // a percent bar.
    const handleHoldingElement = ((_element = element) === null || _element === void 0 ? void 0 : _element.syncIdMap[client.scheduledEventName]) ?? element,
      handleEl = event.target.closest('[class$="-handle"]');
    if (!resizable || handleEl && handleEl !== handleHoldingElement) {
      return false;
    }
    element = event.target.closest(me.dragSelector);
    if (!element) {
      return false;
    }
    const startsOutside = element.classList.contains('b-sch-event-startsoutside'),
      endsOutside = element.classList.contains('b-sch-event-endsoutside');
    if (resizable === true) {
      if (startsOutside && endsOutside) {
        return false;
      } else if (startsOutside) {
        resizable = 'end';
      } else if (endsOutside) {
        resizable = 'start';
      } else {
        return me.isOverStartHandle(event, element) || me.isOverEndHandle(event, element);
      }
    }
    if (startsOutside && resizable === 'start' || endsOutside && resizable === 'end') {
      return false;
    }
    if (me.isOverStartHandle(event, element) && resizable === 'start' || me.isOverEndHandle(event, element) && resizable === 'end') {
      return true;
    }
    return false;
  }
  updateDateIndicator() {
    const {
        clockTemplate
      } = this,
      {
        eventRecord,
        draggingEnd,
        snappedDate
      } = this.dragging.context,
      startDate = draggingEnd ? eventRecord.get('startDate') : snappedDate,
      endDate = draggingEnd ? snappedDate : eventRecord.get('endDate'),
      {
        element
      } = this.tip;
    clockTemplate.updateDateIndicator(element.querySelector('.b-sch-tooltip-startdate'), startDate);
    clockTemplate.updateDateIndicator(element.querySelector('.b-sch-tooltip-enddate'), endDate);
  }
  getTooltipTarget({
    itemElement,
    context
  }) {
    const me = this,
      {
        rtl
      } = me.client,
      target = Rectangle.from(itemElement, null, true);
    if (me.dragLock === 'x') {
      // Align to the dragged edge of the proxy, and then bump right so that the anchor aligns perfectly.
      if (!rtl && context.edge === 'right' || rtl && context.edge === 'left') {
        target.x = target.right - 1;
      } else {
        target.x -= me.tip.anchorSize[0] / 2;
      }
      target.width = me.tip.anchorSize[0] / 2;
    } else {
      // Align to the dragged edge of the proxy, and then bump bottom so that the anchor aligns perfectly.
      if (context.edge === 'bottom') {
        target.y = target.bottom - 1;
      }
      target.height = me.tip.anchorSize[1] / 2;
    }
    return {
      target
    };
  }
  basicValidityCheck(context, event) {
    return context.startDate && (context.endDate > context.startDate || this.allowResizeToZero) && this.validatorFn.call(this.validatorFnThisObj || this, context, event);
  }
  //endregion
  //region Tooltip
  getTipHtml({
    tip
  }) {
    const me = this,
      {
        startDate,
        endDate,
        toSet,
        snappedDate,
        valid,
        message = '',
        timespanRecord
      } = me.dragging.context;
    // Empty string hides the tip - we get called before the Resizer, so first call will be empty
    if (!startDate || !endDate) {
      return tip.html;
    }
    // Set whichever one we are moving
    const tipData = {
      record: timespanRecord,
      valid,
      message,
      startDate,
      endDate,
      [toSet]: snappedDate
    };
    // Format the two ends. This has to be done outside of the object initializer
    // because they use properties that are only in the tipData object.
    tipData.startText = me.client.getFormattedDate(tipData.startDate);
    tipData.endText = me.client.getFormattedDate(tipData.endDate);
    tipData.startClockHtml = me.clockTemplate.template({
      date: tipData.startDate,
      text: tipData.startText,
      cls: 'b-sch-tooltip-startdate'
    });
    tipData.endClockHtml = me.clockTemplate.template({
      date: tipData.endDate,
      text: tipData.endText,
      cls: 'b-sch-tooltip-enddate'
    });
    return me.tooltipTemplate(tipData);
  }
  //endregion
  //region Product specific, may be overridden in subclasses
  beginEventRecordBatch(eventRecord) {
    eventRecord.beginBatch();
  }
  cancelEventRecordBatch(eventRecord) {
    // Reverts the changes, a batchedUpdate event will fire which will reset the UI
    eventRecord.cancelBatch();
  }
  getBeforeResizeParams(context) {
    const {
      client
    } = this;
    return {
      resourceRecord: client.resolveResourceRecord(client.isVertical ? context.event : context.element)
    };
  }
  getResizeStartParams(context) {
    return {
      resourceRecord: context.resourceRecord
    };
  }
  getResizeEndParams(context) {
    return {
      resourceRecord: context.resourceRecord,
      event: context.event
    };
  }
  setupProductResizeContext(context, event) {
    var _client$resolveResour, _client$resolveAssign2, _client$getDateConstr;
    const {
        client
      } = this,
      {
        element
      } = context,
      eventRecord = client.resolveEventRecord(element),
      resourceRecord = (_client$resolveResour = client.resolveResourceRecord) === null || _client$resolveResour === void 0 ? void 0 : _client$resolveResour.call(client, element),
      assignmentRecord = (_client$resolveAssign2 = client.resolveAssignmentRecord) === null || _client$resolveAssign2 === void 0 ? void 0 : _client$resolveAssign2.call(client, element);
    Object.assign(context, {
      eventRecord,
      taskRecord: eventRecord,
      resourceRecord,
      assignmentRecord,
      dateConstraints: (_client$getDateConstr = client.getDateConstraints) === null || _client$getDateConstr === void 0 ? void 0 : _client$getDateConstr.call(client, resourceRecord, eventRecord)
    });
  }
  checkValidity({
    startDate,
    endDate,
    eventRecord,
    resourceRecord
  }) {
    const {
      client
    } = this;
    if (!client.allowOverlap) {
      if (eventRecord.resources.some(resource => !client.isDateRangeAvailable(startDate, endDate, eventRecord, resource))) {
        return {
          valid: false,
          message: this.L('L{EventDrag.eventOverlapsExisting}')
        };
      }
    }
    return this.basicValidityCheck(...arguments);
  }
  get tipId() {
    return `${this.client.id}-event-resize-tip`;
  }
  //endregion
}

EventResize._$name = 'EventResize';
GridFeatureManager.registerFeature(EventResize, true, 'Scheduler');
GridFeatureManager.registerFeature(EventResize, false, 'ResourceHistogram');

/**
 * @module Scheduler/feature/base/DragCreateBase
 */
const getDragCreateDragDistance = function (event) {
  var _this$source, _this$source$client$f;
  // Do not allow the drag to begin if the taskEdit feature (if present) is in the process
  // of canceling. We must wait for it to have cleaned up its data manipulations before
  // we can add the new, drag-created record
  if ((_this$source = this.source) !== null && _this$source !== void 0 && (_this$source$client$f = _this$source.client.features.taskEdit) !== null && _this$source$client$f !== void 0 && _this$source$client$f._canceling) {
    return false;
  }
  return EventHelper.getDistanceBetween(this.startEvent, event);
};
/**
 * Base class for EventDragCreate (Scheduler) and TaskDragCreate (Gantt) features. Contains shared code. Not to be used directly.
 *
 * @extends Scheduler/feature/EventResize
 */
class DragCreateBase extends TaskEditStm(EventResize) {
  //region Config
  static configurable = {
    /**
     * true to show a time tooltip when dragging to create a new event
     * @config {Boolean}
     * @default
     */
    showTooltip: true,
    /**
     * Number of pixels the drag target must be moved before dragging is considered to have started. Defaults to 2.
     * @config {Number}
     * @default
     */
    dragTolerance: 2,
    // used by gantt to only allow one task per row
    preventMultiple: false,
    dragTouchStartDelay: 300,
    /**
     * `this` reference for the validatorFn
     * @config {Object}
     */
    validatorFnThisObj: null,
    tipTemplate: data => `
            <div class="b-sch-tip-${data.valid ? 'valid' : 'invalid'}">
                ${data.startClockHtml}
                ${data.endClockHtml}
                <div class="b-sch-tip-message">${data.message}</div>
            </div>
        `,
    dragActiveCls: 'b-dragcreating'
  };
  static pluginConfig = {
    chain: ['render', 'onEventDataGenerated'],
    before: ['onElementContextMenu']
  };
  construct(scheduler, config) {
    if ((config === null || config === void 0 ? void 0 : config.showTooltip) === false) {
      config.tip = null;
    }
    super.construct(...arguments);
  }
  //endregion
  changeValidatorFn(validatorFn) {
    // validatorFn property is used by the EventResize base to validate each mousemove
    // We change the property name to createValidatorFn
    this.createValidatorFn = validatorFn;
  }
  render() {
    const me = this,
      {
        client
      } = me;
    // Set up elements and listeners
    me.dragRootElement = me.dropRootElement = client.timeAxisSubGridElement;
    // Drag only in time dimension
    me.dragLock = client.isVertical ? 'y' : 'x';
  }
  onDragEndSwitch(context) {
    const {
        client
      } = this,
      {
        enableEventAnimations
      } = client,
      {
        eventRecord,
        draggingEnd
      } = context,
      horizontal = this.dragLock === 'x',
      {
        initialDate
      } = this.dragging;
    // Setting the new opposite end should not animate
    client.enableEventAnimations = false;
    // Zero duration at the moment of the flip
    eventRecord.set({
      startDate: initialDate,
      endDate: initialDate
    });
    // We're switching to dragging the start
    if (draggingEnd) {
      Object.assign(context, {
        endDate: initialDate,
        toSet: 'startDate',
        otherEnd: 'endDate',
        setMethod: 'setStartDate',
        setOtherMethod: 'setEndDate',
        edge: horizontal ? 'left' : 'top'
      });
    } else {
      Object.assign(context, {
        startDate: initialDate,
        toSet: 'endDate',
        otherEnd: 'startDate',
        setMethod: 'setEndDate',
        setOtherMethod: 'setStartDate',
        edge: horizontal ? 'right' : 'bottom'
      });
    }
    context.draggingEnd = this.draggingEnd = !draggingEnd;
    client.enableEventAnimations = enableEventAnimations;
  }
  beforeDrag(drag) {
    const me = this,
      result = super.beforeDrag(drag),
      {
        pan,
        eventDragSelect
      } = me.client.features;
    // Superclass's handler may also veto
    if (result !== false && (
    // used by gantt to only allow one task per row
    me.preventMultiple && !me.isRowEmpty(drag.rowRecord) || me.disabled ||
    // If Pan is enabled, it has right of way
    pan && !pan.disabled ||
    // If EventDragSelect is enabled, it has right of way
    eventDragSelect && !eventDragSelect.disabled)) {
      return false;
    }
    // Prevent drag select if drag-creating, could collide otherwise
    // (reset by GridSelection)
    me.client.preventDragSelect = true;
    return result;
  }
  startDrag(drag) {
    const result = super.startDrag(drag);
    // Returning false means operation is aborted.
    if (result !== false) {
      const {
        context
      } = drag;
      // Date to flip around when changing direction
      drag.initialDate = context.eventRecord.get(this.draggingEnd ? 'startDate' : 'endDate');
      this.client.trigger('dragCreateStart', {
        proxyElement: drag.element,
        eventElement: drag.element,
        eventRecord: context.eventRecord,
        resourceRecord: context.resourceRecord
      });
      // We are always dragging the exact edge of the event element.
      drag.context.offset = 0;
      drag.context.oldValue = drag.mousedownDate;
    }
    return result;
  }
  // Used by our EventResize superclass to know whether the drag point is the end or the beginning.
  isOverEndHandle() {
    return this.draggingEnd;
  }
  setupDragContext(event) {
    const {
      client
    } = this;
    // Only mousedown on an empty cell can initiate drag-create
    if (client.matchScheduleCell(event.target)) {
      var _client$resolveResour;
      const resourceRecord = (_client$resolveResour = client.resolveResourceRecord(event)) === null || _client$resolveResour === void 0 ? void 0 : _client$resolveResour.$original;
      // And there must be a resource backing the cell.
      if (resourceRecord && !resourceRecord.isSpecialRow) {
        // Skip the EventResize's setupDragContext. We want the base one.
        const result = Draggable().prototype.setupDragContext.call(this, event),
          scrollables = [];
        if (client.isVertical) {
          scrollables.push({
            element: client.scrollable.element,
            direction: 'vertical'
          });
        } else {
          scrollables.push({
            element: client.timeAxisSubGrid.scrollable.element,
            direction: 'horizontal'
          });
        }
        result.scrollManager = client.scrollManager;
        result.monitoringConfig = {
          scrollables
        };
        result.resourceRecord = result.rowRecord = resourceRecord;
        // We use a special method to get the distance moved.
        // If the TaskEdit feature is still in its canceling phase, then
        // it returns false which inhibits the start of the drag-create
        // until the cancelation is complete.
        result.getDistance = getDragCreateDragDistance;
        return result;
      }
    }
  }
  async dragDrop({
    context,
    event
  }) {
    var _this$tip;
    // Set the start/end date, whichever we were dragging
    // to the correctly rounded value before updating.
    context[context.toSet] = context.snappedDate;
    const {
        client
      } = this,
      {
        startDate,
        endDate,
        eventRecord
      } = context,
      {
        generation
      } = eventRecord;
    let modified;
    (_this$tip = this.tip) === null || _this$tip === void 0 ? void 0 : _this$tip.hide();
    // Handle https://github.com/bryntum/support/issues/3210.
    // The issue arises when the mouseup arrives very quickly and the commit kicked off
    // at event add has not yet completed. If it now completes *after* we finalize
    // the drag, it will reset the event to its initial state.
    // If that commit has in fact finished, this will be a no-op
    await client.project.commitAsync();
    // If the above commit in fact reset the event back to the initial state, we have to
    // force the event rendering to bring it back to the currently known context state.
    if (eventRecord.generation !== generation) {
      context.eventRecord[context.toSet] = context.oldValue;
      context.eventRecord[context.toSet] = context[context.toSet];
    }
    context.valid = startDate && endDate && endDate - startDate > 0 &&
    // Input sanity check
    context[context.toSet] - context.oldValue &&
    // Make sure dragged end changed
    context.valid !== false;
    if (context.valid) {
      // Seems to be a valid drag-create operation, ask outside world if anyone wants to take control over the finalizing,
      // to show a confirm dialog prior to finalizing the create.
      client.trigger('beforeDragCreateFinalize', {
        context,
        event,
        proxyElement: context.element,
        eventElement: context.element,
        eventRecord: context.eventRecord,
        resourceRecord: context.resourceRecord
      });
      modified = true;
    }
    // If a handler has set the async flag, it means that they are going to finalize
    // the operation at some time in the future, so we should not call it.
    if (!context.async) {
      await context.finalize(modified);
    }
  }
  updateDragTolerance(dragTolerance) {
    this.dragThreshold = dragTolerance;
  }
  //region Tooltip
  changeTip(tip, oldTip) {
    return super.changeTip(!tip || tip.isTooltip ? tip : ObjectHelper.assign({
      id: `${this.client.id}-drag-create-tip`
    }, tip), oldTip);
  }
  //endregion
  //region Finalize (create EventModel)
  // this method is actually called on the `context` object,
  // so `this` object inside might not be what you think (see `me = this.owner` below)
  // not clear what was the motivation for such design
  async finalize(doCreate) {
    // only call this method once, do not re-enter
    if (this.finalized) {
      return;
    }
    this.finalized = true;
    const me = this.owner,
      context = this,
      completeFinalization = () => {
        if (!me.isDestroyed) {
          me.client.trigger('afterDragCreate', {
            proxyElement: context.element,
            eventElement: context.element,
            eventRecord: context.eventRecord,
            resourceRecord: context.resourceRecord
          });
          me.cleanup(context);
        }
      };
    if (doCreate) {
      // Call product specific implementation
      await me.finalizeDragCreate(context);
      completeFinalization();
    }
    // Aborting without going ahead with create - we must deassign and remove the event
    else {
      var _me$onAborted;
      await me.cancelDragCreate(context);
      (_me$onAborted = me.onAborted) === null || _me$onAborted === void 0 ? void 0 : _me$onAborted.call(me, context);
      completeFinalization();
    }
  }
  async cancelDragCreate(context) {}
  async finalizeDragCreate(context) {
    var _this$client;
    // EventResize base class applies final changes to the event record
    await this.internalUpdateRecord(context, context.eventRecord);
    const stmCapture = {
      stmInitiallyAutoRecord: this.stmInitiallyAutoRecord,
      stmInitiallyDisabled: this.stmInitiallyDisabled,
      // this flag indicates whether the STM capture has been transferred to
      // another feature, which will be responsible for finalizing the STM transaction
      // (otherwise we'll do it ourselves)
      transferred: false
    };
    this.client.trigger('dragCreateEnd', {
      eventRecord: context.eventRecord,
      resourceRecord: context.resourceRecord,
      event: context.event,
      eventElement: context.element,
      stmCapture
    });
    // Part of the Scheduler API. Triggered by its createEvent method.
    // Auto-editing features can use this to edit new events.
    // Note that this may be destroyed by a listener of the previous event.
    (_this$client = this.client) === null || _this$client === void 0 ? void 0 : _this$client.trigger('eventAutoCreated', {
      eventRecord: context.eventRecord,
      resourceRecord: context.resourceRecord
    });
    return stmCapture.transferred;
  }
  cleanup(context) {
    var _this$tip2;
    const {
        client
      } = this,
      {
        eventRecord
      } = context;
    // Base class's cleanup is not called, we have to clear this flag.
    // The isCreating flag is only set if the event is to be handed off to the
    // eventEdit feature and that feature then has responsibility for clearing it.
    eventRecord.meta.isResizing = false;
    client.endListeningForBatchedUpdates();
    (_this$tip2 = this.tip) === null || _this$tip2 === void 0 ? void 0 : _this$tip2.hide();
    client.element.classList.remove(...this.dragActiveCls.split(' '));
    context.element.parentElement.classList.remove('b-sch-dragcreating');
  }
  //endregion
  //region Events
  /**
   * Prevent right click when drag creating
   * @returns {Boolean}
   * @private
   */
  onElementContextMenu() {
    if (this.proxy) {
      return false;
    }
  }
  prepareCreateContextForFinalization(createContext, event, finalize, async = false) {
    return {
      ...createContext,
      async,
      event,
      finalize
    };
  }
  // Apply drag create "proxy" styling
  onEventDataGenerated(renderData) {
    var _this$dragging, _this$dragging$contex;
    if (((_this$dragging = this.dragging) === null || _this$dragging === void 0 ? void 0 : (_this$dragging$contex = _this$dragging.context) === null || _this$dragging$contex === void 0 ? void 0 : _this$dragging$contex.eventRecord) === renderData.eventRecord) {
      // Allow custom styling for drag creation element
      renderData.wrapperCls['b-sch-dragcreating'] = true;
      // Styling when drag create will be aborted on drop (because it would yield zero duration)
      renderData.wrapperCls['b-too-narrow'] = this.dragging.context.tooNarrow;
    }
  }
  //endregion
  //region Product specific, implemented in subclasses
  // Empty implementation here. Only base EventResize class triggers this
  triggerBeforeResize() {}
  // Empty implementation here. Only base EventResize class triggers this
  triggerEventResizeStart() {}
  checkValidity(context, event) {
    throw new Error('Implement in subclass');
  }
  handleBeforeDragCreate(dateTime, event) {
    throw new Error('Implement in subclass');
  }
  isRowEmpty(rowRecord) {
    throw new Error('Implement in subclass');
  }
  //endregion
}

DragCreateBase._$name = 'DragCreateBase';

/**
 * @module Scheduler/feature/base/TooltipBase
 */
/**
 * Base class for `EventTooltip` (Scheduler) and `TaskTooltip` (Gantt) features. Contains shared code. Not to be used directly.
 *
 * @extends Core/mixin/InstancePlugin
 * @extendsconfigs Core/widget/Tooltip
 */
class TooltipBase extends InstancePlugin {
  //region Config
  static get defaultConfig() {
    return {
      /**
       * Specify true to have tooltip updated when mouse moves, if you for example want to display date at mouse
       * position.
       * @config {Boolean}
       * @default
       * @category Misc
       */
      autoUpdate: false,
      /**
       * The amount of time to hover before showing
       * @config {Number}
       * @default
       */
      hoverDelay: 250,
      /**
       * The time (in milliseconds) for which the Tooltip remains visible when the mouse leaves the target.
       *
       * May be configured as `false` to persist visible after the mouse exits the target element. Configure it
       * as 0 to always retrigger `hoverDelay` even when moving mouse inside `fromElement`
       * @config {Number}
       * @default
       */
      hideDelay: 100,
      template: null,
      cls: null,
      align: {
        align: 'b-t'
      },
      clockTemplate: null,
      // Set to true to update tooltip contents if record changes while tip is open
      monitorRecordUpdate: null,
      testConfig: {
        hoverDelay: 0
      }
    };
  }
  // Plugin configuration. This plugin chains some of the functions in Grid.
  static get pluginConfig() {
    return {
      chain: ['onPaint']
    };
  }
  //endregion
  //region Events
  /**
   * Triggered before a tooltip is shown. Return `false` to prevent the action.
   * @preventable
   * @event beforeShow
   * @param {Core.widget.Tooltip} source The tooltip being shown.
   * @param {Scheduler.model.EventModel} source.eventRecord The event record.
   */
  /**
   * Triggered after a tooltip is shown.
   * @event show
   * @param {Core.widget.Tooltip} source The tooltip.
   * @param {Scheduler.model.EventModel} source.eventRecord The event record.
   */
  //endregion
  //region Init
  construct(client, config) {
    const me = this;
    // process initial config into an actual config object
    config = me.processConfig(config);
    super.construct(client, config);
    // Default triggering selector is the client's inner element selector
    if (!me.forSelector) {
      me.forSelector = `${client.eventInnerSelector}:not(.b-dragproxy)`;
    }
    me.clockTemplate = new ClockTemplate({
      scheduler: client
    });
    client.ion({
      [`before${client.scheduledEventName}drag`]: () => {
        var _me$tooltip;
        // Using {} on purpose to not return the promise
        (_me$tooltip = me.tooltip) === null || _me$tooltip === void 0 ? void 0 : _me$tooltip.hide();
      }
    });
  }
  // TooltipBase feature handles special config cases, where user can supply a function to use as template
  // instead of a normal config object
  processConfig(config) {
    if (typeof config === 'function') {
      return {
        template: config
      };
    }
    return config;
  }
  // override setConfig to process config before applying it (used mainly from ReactScheduler)
  setConfig(config) {
    super.setConfig(this.processConfig(config));
  }
  doDestroy() {
    this.destroyProperties('clockTemplate', 'tooltip');
    super.doDestroy();
  }
  doDisable(disable) {
    if (this.tooltip) {
      this.tooltip.disabled = disable;
    }
    super.doDisable(disable);
  }
  //endregion
  onPaint({
    firstPaint
  }) {
    if (firstPaint) {
      var _me$tooltip2;
      const me = this,
        {
          client
        } = me,
        ignoreSelector = ['.b-dragselecting', '.b-eventeditor-editing', '.b-taskeditor-editing', '.b-resizing-event', '.b-task-percent-bar-resizing-task', '.b-dragcreating', `.b-dragging-${client.scheduledEventName}`, '.b-creating-dependency', '.b-dragproxy'].map(cls => `:not(${cls})`).join('');
      (_me$tooltip2 = me.tooltip) === null || _me$tooltip2 === void 0 ? void 0 : _me$tooltip2.destroy();
      /**
       * A reference to the tooltip instance, which will have a special `eventRecord` property that
       * you can use to get data from the contextual event record to which this tooltip is related.
       * @member {Core.widget.Tooltip} tooltip
       * @readonly
       * @category Misc
       */
      const tip = me.tooltip = new Tooltip({
        axisLock: 'flexible',
        id: me.tipId || `${me.client.id}-event-tip`,
        cls: me.tipCls,
        forSelector: `.b-timelinebase${ignoreSelector} .b-grid-body-container:not(.b-scrolling) ${me.forSelector}`,
        scrollAction: 'realign',
        forElement: client.timeAxisSubGridElement,
        showOnHover: true,
        anchorToTarget: true,
        getHtml: me.getTipHtml.bind(me),
        disabled: me.disabled,
        // on Core/mixin/Events constructor, me.config.listeners is deleted and attributed its value to me.configuredListeners
        // to then on processConfiguredListeners it set me.listeners to our TooltipBase
        // but since we need our initial config.listeners to set to our internal tooltip, we leave processConfiguredListeners empty
        // to avoid lost our listeners to apply for our internal tooltip here and force our feature has all Tooltip events firing
        ...me.config,
        internalListeners: me.configuredListeners
      });
      tip.ion({
        innerhtmlupdate: 'updateDateIndicator',
        overtarget: 'onOverNewTarget',
        show: 'onTipShow',
        hide: 'onTipHide',
        thisObj: me
      });
      // Once instantiated, any Tooltip configs are relayed through the feature directly to the tip
      Object.keys(tip.$meta.configs).forEach(name => {
        Object.defineProperty(this, name, {
          set: v => tip[name] = v,
          get: () => tip[name]
        });
      });
    }
  }
  //region Listeners
  // leave configuredListeners alone until render time at which they are used on the tooltip
  processConfiguredListeners() {}
  addListener(...args) {
    var _this$tooltip;
    const
      // Call super method to handle enable/disable feature events
      defaultDetacher = super.addListener(...args),
      // Add listener to the `tooltip` instance
      tooltipDetacher = (_this$tooltip = this.tooltip) === null || _this$tooltip === void 0 ? void 0 : _this$tooltip.addListener(...args);
    if (defaultDetacher || tooltipDetacher) {
      return () => {
        defaultDetacher === null || defaultDetacher === void 0 ? void 0 : defaultDetacher();
        tooltipDetacher === null || tooltipDetacher === void 0 ? void 0 : tooltipDetacher();
      };
    }
  }
  removeListener(...args) {
    var _this$tooltip2;
    super.removeListener(...args);
    // Remove listener from the `tooltip` instance
    (_this$tooltip2 = this.tooltip) === null || _this$tooltip2 === void 0 ? void 0 : _this$tooltip2.removeListener(...args);
  }
  //endregion
  updateDateIndicator() {
    const me = this,
      tip = me.tooltip,
      endDateElement = tip.element.querySelector('.b-sch-tooltip-enddate');
    if (!me.record) {
      return;
    }
    me.clockTemplate.updateDateIndicator(tip.element, me.record.startDate);
    endDateElement && me.clockTemplate.updateDateIndicator(endDateElement, me.record.endDate);
  }
  resolveTimeSpanRecord(forElement) {
    return this.client.resolveTimeSpanRecord(forElement);
  }
  getTipHtml({
    tip,
    activeTarget
  }) {
    const me = this,
      {
        client
      } = me,
      recordProp = me.recordType || `${client.scheduledEventName}Record`,
      timeSpanRecord = me.resolveTimeSpanRecord(activeTarget);
    // If user has mouseovered a fading away element of a deleted event,
    // an event record will not be found. In this case the tip must hide.
    // Instance of check is to not display while propagating
    if ((timeSpanRecord === null || timeSpanRecord === void 0 ? void 0 : timeSpanRecord.startDate) instanceof Date) {
      const {
          startDate,
          endDate
        } = timeSpanRecord,
        startText = client.getFormattedDate(startDate),
        endDateValue = client.getDisplayEndDate(endDate, startDate),
        endText = client.getFormattedDate(endDateValue);
      tip.eventRecord = timeSpanRecord;
      return me.template({
        tip,
        // eventRecord for Scheduler, taskRecord for Gantt
        [`${recordProp}`]: timeSpanRecord,
        startDate,
        endDate,
        startText,
        endText,
        startClockHtml: me.clockTemplate.template({
          date: startDate,
          text: startText,
          cls: 'b-sch-tooltip-startdate'
        }),
        endClockHtml: timeSpanRecord.isMilestone ? '' : me.clockTemplate.template({
          date: endDateValue,
          text: endText,
          cls: 'b-sch-tooltip-enddate'
        })
      });
    } else {
      tip.hide();
      return '';
    }
  }
  get record() {
    return this.tooltip.eventRecord;
  }
  onTipShow() {
    const me = this;
    if (me.monitorRecordUpdate && !me.updateListener) {
      me.updateListener = me.client.eventStore.ion({
        change: me.onRecordUpdate,
        buffer: 300,
        thisObj: me
      });
    }
  }
  onTipHide() {
    var _this$updateListener;
    // To not retain full project when changing project
    this.tooltip.eventRecord = null;
    (_this$updateListener = this.updateListener) === null || _this$updateListener === void 0 ? void 0 : _this$updateListener.call(this);
    this.updateListener = null;
  }
  onOverNewTarget({
    newTarget
  }) {
    this.tooltip.eventRecord = this.resolveTimeSpanRecord(newTarget);
  }
  onRecordUpdate({
    record
  }) {
    const {
      tooltip
    } = this;
    // Make sure the record we are showing the tip for is still relevant
    // If the change moved the element out from under the mouse, we will be hidden.
    if (tooltip !== null && tooltip !== void 0 && tooltip.isVisible && record === this.record) {
      tooltip.updateContent();
      // If we were aligning to the event bar, realign to it.
      if (tooltip.lastAlignSpec.aligningToElement) {
        tooltip.realign();
      }
      // The pointer is still over the target (otherwise tooltip would be hidden)
      // So invoke the tooltip's positioning
      else {
        tooltip.internalOnPointerOver(this.client.lastPointerEvent);
      }
    }
  }
}
TooltipBase._$name = 'TooltipBase';

/**
 * @module Scheduler/feature/AbstractTimeRanges
 */
/**
 * Abstract base class, you should not use this class directly.
 * @abstract
 * @mixes Core/mixin/Delayable
 * @extends Core/mixin/InstancePlugin
 */
class AbstractTimeRanges extends InstancePlugin.mixin(Delayable) {
  //region Config
  /**
   * Fired on the owning Scheduler when a click happens on a time range header element
   * @event timeRangeHeaderClick
   * @on-owner
   * @param {Scheduler.view.Scheduler} source Scheduler instance
   * @param {Scheduler.model.TimeSpan} timeRangeRecord The record
   * @param {MouseEvent} event DEPRECATED 5.3.0 Use `domEvent` instead
   * @param {MouseEvent} domEvent Browser event
   */
  /**
   * Fired on the owning Scheduler when a double click happens on a time range header element
   * @event timeRangeHeaderDblClick
   * @on-owner
   * @param {Scheduler.view.Scheduler} source Scheduler instance
   * @param {Scheduler.model.TimeSpan} timeRangeRecord The record
   * @param {MouseEvent} event DEPRECATED 5.3.0 Use `domEvent` instead
   * @param {MouseEvent} domEvent Browser event
   */
  /**
   * Fired on the owning Scheduler when a right click happens on a time range header element
   * @event timeRangeHeaderContextMenu
   * @on-owner
   * @param {Scheduler.view.Scheduler} source Scheduler instance
   * @param {Scheduler.model.TimeSpan} timeRangeRecord The record
   * @param {MouseEvent} event DEPRECATED 5.3.0 Use `domEvent` instead
   * @param {MouseEvent} domEvent Browser event
   */
  static get defaultConfig() {
    return {
      // CSS class to apply to range elements
      rangeCls: 'b-sch-range',
      // CSS class to apply to line elements (0-duration time range)
      lineCls: 'b-sch-line',
      /**
       * Set to `true` to enable dragging and resizing of range elements in the header. Only relevant when
       * {@link #config-showHeaderElements} is `true`.
       * @config {Boolean}
       * @default
       * @category Common
       */
      enableResizing: false,
      /**
       * A Boolean specifying whether to show tooltip while resizing range elements, or a
       * {@link Core.widget.Tooltip} config object which is applied to the tooltip
       * @config {Boolean|TooltipConfig}
       * @default
       * @category Common
       */
      showTooltip: true,
      /**
       * Template used to generate the tooltip contents when hovering a time range header element.
       * ```
       * const scheduler = new Scheduler({
       *   features : {
       *     timeRanges : {
       *       tooltipTemplate({ timeRange }) {
       *         return `${timeRange.name}`
       *       }
       *     }
       *   }
       * });
       * ```
       * @config {Function} tooltipTemplate
       * @param {Object} data Tooltip data
       * @param {Scheduler.model.TimeSpan} data.timeRange
       * @category Common
       */
      tooltipTemplate: null,
      dragTipTemplate: data => `
                <div class="b-sch-tip-${data.valid ? 'valid' : 'invalid'}">
                    <div class="b-sch-tip-name">${StringHelper.encodeHtml(data.name) || ''}</div>
                    ${data.startClockHtml}
                    ${data.endClockHtml || ''}
                </div>
            `,
      baseCls: 'b-sch-timerange',
      /**
       * Function used to generate the HTML content for a time range header element.
       * ```
       * const scheduler = new Scheduler({
       *   features : {
       *     timeRanges : {
       *       headerRenderer({ timeRange }) {
       *         return `${timeRange.name}`
       *       }
       *     }
       *   }
       * });
       * ```
       * @config {Function} headerRenderer
       * @param {Object} data Render data
       * @param {Scheduler.model.TimeSpan} data.timeRange
       * @category Common
       */
      headerRenderer: null,
      /**
       * Function used to generate the HTML content for a time range body element.
       * ```
       * const scheduler = new Scheduler({
       *   features : {
       *     timeRanges : {
       *       bodyRenderer({ timeRange }) {
       *         return `${timeRange.name}`
       *       }
       *     }
       *   }
       * });
       * ```
       * @config {Function} bodyRenderer
       * @param {Object} data Render data
       * @param {Scheduler.model.TimeSpan} data.timeRange
       * @category Common
       */
      bodyRenderer: null,
      // a unique cls used by subclasses to get custom styling of the elements rendered
      cls: null,
      narrowThreshold: 80
    };
  }
  static configurable = {
    /**
     * Set to `false` to not render range elements into the time axis header
     * @prp {Boolean}
     * @default
     * @category Common
     */
    showHeaderElements: true
  };
  // Plugin configuration. This plugin chains some functions in Grid.
  static pluginConfig = {
    chain: ['onPaint', 'populateTimeAxisHeaderMenu', 'onSchedulerHorizontalScroll', 'afterScroll', 'onInternalResize']
  };
  //endregion
  //region Init & destroy
  construct(client, config) {
    const me = this;
    super.construct(client, config);
    if (client.isVertical) {
      client.ion({
        renderRows: me.onUIReady,
        thisObj: me,
        once: true
      });
    }
    // Add a unique cls used by subclasses to get custom styling of the elements rendered
    // This makes sure that each class only removed its own elements from the DOM
    me.cls = me.cls || `b-sch-${me.constructor.$$name.toLowerCase()}`;
    me.baseSelector = `.${me.baseCls}.${me.cls}`;
    // header elements are required for interaction
    if (me.enableResizing) {
      me.showHeaderElements = true;
    }
  }
  doDestroy() {
    var _me$clockTemplate, _me$tip, _me$drag, _me$resize;
    const me = this;
    me.detachListeners('timeAxisViewModel');
    me.detachListeners('timeAxis');
    (_me$clockTemplate = me.clockTemplate) === null || _me$clockTemplate === void 0 ? void 0 : _me$clockTemplate.destroy();
    (_me$tip = me.tip) === null || _me$tip === void 0 ? void 0 : _me$tip.destroy();
    (_me$drag = me.drag) === null || _me$drag === void 0 ? void 0 : _me$drag.destroy();
    (_me$resize = me.resize) === null || _me$resize === void 0 ? void 0 : _me$resize.destroy();
    super.doDestroy();
  }
  doDisable(disable) {
    this.renderRanges();
    super.doDisable(disable);
  }
  setupTimeAxisViewModelListeners() {
    const me = this;
    me.detachListeners('timeAxisViewModel');
    me.detachListeners('timeAxis');
    me.client.timeAxisViewModel.ion({
      name: 'timeAxisViewModel',
      update: 'onTimeAxisViewModelUpdate',
      thisObj: me
    });
    me.client.timeAxis.ion({
      name: 'timeAxis',
      includeChange: 'renderRanges',
      thisObj: me
    });
    me.updateLineBuffer();
  }
  onUIReady() {
    const me = this,
      {
        client
      } = me;
    // If timeAxisViewModel is swapped, re-setup listeners to new instance
    client.ion({
      timeAxisViewModelChange: me.setupTimeAxisViewModelListeners,
      thisObj: me
    });
    me.setupTimeAxisViewModelListeners();
    if (!client.hideHeaders) {
      if (me.headerContainerElement) {
        EventHelper.on({
          click: me.onTimeRangeClick,
          dblclick: me.onTimeRangeClick,
          contextmenu: me.onTimeRangeClick,
          delegate: me.baseSelector,
          element: me.headerContainerElement,
          thisObj: me
        });
      }
      if (me.enableResizing) {
        me.drag = DragHelper.new({
          name: 'rangeDrag',
          lockX: client.isVertical,
          lockY: client.isHorizontal,
          constrain: true,
          outerElement: me.headerContainerElement,
          targetSelector: `${me.baseSelector}`,
          isElementDraggable: (el, event) => !client.readOnly && me.isElementDraggable(el, event),
          rtlSource: client,
          internalListeners: {
            dragstart: 'onDragStart',
            drag: 'onDrag',
            drop: 'onDrop',
            reset: 'onDragReset',
            abort: 'onInvalidDrop',
            thisObj: me
          }
        }, me.dragHelperConfig);
        me.resize = ResizeHelper.new({
          direction: client.mode,
          targetSelector: `${me.baseSelector}.b-sch-range`,
          outerElement: me.headerContainerElement,
          isElementResizable: (el, event) => !el.matches('.b-dragging,.b-readonly') && !event.target.matches('.b-fa'),
          internalListeners: {
            resizestart: 'onResizeStart',
            resizing: 'onResizeDrag',
            resize: 'onResize',
            cancel: 'onInvalidResize',
            reset: 'onResizeReset',
            thisObj: me
          }
        }, me.resizeHelperConfig);
      }
    }
    me.renderRanges();
    if (me.tooltipTemplate) {
      me.hoverTooltip = new Tooltip({
        forElement: me.headerContainerElement,
        getHtml({
          activeTarget
        }) {
          const timeRange = me.resolveTimeRangeRecord(activeTarget);
          return me.tooltipTemplate({
            timeRange
          });
        },
        forSelector: '.' + me.baseCls + (me.cls ? '.' + me.cls : '')
      });
    }
  }
  //endregion
  //region Draw
  refresh() {
    this._timeRanges = null;
    this.renderRanges();
  }
  getDOMConfig(startDate, endDate) {
    const me = this,
      bodyConfigs = [],
      headerConfigs = [];
    if (!me.disabled) {
      // clear label rotation map cache here, used to prevent height calculations for every timeRange entry to
      // speed up using recurrences
      me._labelRotationMap = {};
      for (const range of me.timeRanges) {
        const result = me.renderRange(range, startDate, endDate);
        if (result) {
          bodyConfigs.push(result.bodyConfig);
          headerConfigs.push(result.headerConfig);
        }
      }
    }
    return [bodyConfigs, headerConfigs];
  }
  renderRanges() {
    const me = this,
      {
        client
      } = me,
      {
        foregroundCanvas
      } = client;
    // Scheduler/Gantt might not yet be rendered
    if (foregroundCanvas && client.isPainted && !client.timeAxisSubGrid.collapsed) {
      const {
          headerContainerElement
        } = me,
        updatedBodyElements = [],
        [bodyConfigs, headerConfigs] = me.getDOMConfig();
      if (!me.bodyCanvas) {
        me.bodyCanvas = DomHelper.createElement({
          className: `b-timeranges-canvas ${me.cls}-canvas`,
          parent: foregroundCanvas,
          retainElement: true
        });
      }
      DomSync.sync({
        targetElement: me.bodyCanvas,
        childrenOnly: true,
        domConfig: {
          children: bodyConfigs,
          syncOptions: {
            releaseThreshold: 0,
            syncIdField: 'id'
          }
        },
        callback: me.showHeaderElements ? null : ({
          targetElement,
          action
        }) => {
          // Might need to rotate label when not showing header elements
          if (action === 'reuseElement' || action === 'newElement' || action === 'reuseOwnElement') {
            // Collect all here, to not force reflows in the middle of syncing
            updatedBodyElements.push(targetElement);
          }
        }
      });
      if (me.showHeaderElements && !me.headerCanvas) {
        me.headerCanvas = DomHelper.createElement({
          className: `${me.cls}-canvas`,
          parent: headerContainerElement,
          retainElement: true
        });
      }
      if (me.headerCanvas) {
        DomSync.sync({
          targetElement: me.headerCanvas,
          childrenOnly: true,
          domConfig: {
            children: headerConfigs,
            syncOptions: {
              releaseThreshold: 0,
              syncIdField: 'id'
            }
          }
        });
      }
      // Rotate labels last, to not force reflows. First check if rotation is needed
      for (const bodyElement of updatedBodyElements) {
        me.cacheRotation(bodyElement.elementData.timeRange, bodyElement);
      }
      // Then apply rotation
      for (const bodyElement of updatedBodyElements) {
        me.applyRotation(bodyElement.elementData.timeRange, bodyElement);
      }
    }
  }
  // Implement in subclasses
  get timeRanges() {
    return [];
  }
  /**
   * Based on this method result the feature decides whether the provided range should
   * be rendered or not.
   * The method checks that the range intersects the current viewport.
   *
   * Override the method to implement your custom range rendering vetoing logic.
   * @param {Scheduler.model.TimeSpan} range Range to render.
   * @param {Date} [startDate] Specifies view start date. Defaults to view visible range start
   * @param {Date} [endDate] Specifies view end date. Defaults to view visible range end
   * @returns {Boolean} `true` if the range should be rendered and `false` otherwise.
   */
  shouldRenderRange(range, startDate = this.client.visibleDateRange.startDate, endDate = this.client.visibleDateRange.endDate) {
    const {
        timeAxis
      } = this.client,
      {
        startDate: rangeStart,
        endDate: rangeEnd,
        duration
      } = range;
    return Boolean(rangeStart && (timeAxis.isContinuous || timeAxis.isTimeSpanInAxis(range)) && DateHelper.intersectSpans(startDate, endDate, rangeStart,
    // Lines are included longer, to make sure label does not disappear
    duration ? rangeEnd : DateHelper.add(rangeStart, this._lineBufferDurationMS)));
  }
  getRangeDomConfig(timeRange, minDate, maxDate, relativeTo = 0) {
    const me = this,
      {
        client
      } = me,
      {
        rtl
      } = client,
      startPos = client.getCoordinateFromDate(DateHelper.max(timeRange.startDate, minDate), {
        respectExclusion: true
      }) - relativeTo,
      endPos = timeRange.endDate ? client.getCoordinateFromDate(DateHelper.min(timeRange.endDate, maxDate), {
        respectExclusion: true,
        isEnd: true
      }) - relativeTo : startPos,
      size = Math.abs(endPos - startPos),
      isRange = size > 0,
      translateX = rtl ? `calc(${startPos}px - 100%)` : `${startPos}px`;
    return {
      className: {
        [me.baseCls]: 1,
        [me.cls]: me.cls,
        [me.rangeCls]: isRange,
        [me.lineCls]: !isRange,
        [timeRange.cls]: timeRange.cls,
        'b-narrow-range': isRange && size < me.narrowThreshold,
        'b-readonly': timeRange.readOnly,
        'b-rtl': rtl
      },
      dataset: {
        id: timeRange.id
      },
      elementData: {
        timeRange
      },
      style: client.isVertical ? `transform: translateY(${translateX}); ${isRange ? `height:${size}px` : ''};` : `transform: translateX(${translateX}); ${isRange ? `width:${size}px` : ''};`
    };
  }
  renderRange(timeRange, startDate, endDate) {
    const me = this,
      {
        client
      } = me,
      {
        timeAxis
      } = client;
    if (me.shouldRenderRange(timeRange, startDate, endDate) && timeAxis.startDate) {
      const config = me.getRangeDomConfig(timeRange, timeAxis.startDate, timeAxis.endDate),
        icon = timeRange.iconCls && StringHelper.xss`<i class="${timeRange.iconCls}"></i>`,
        name = timeRange.name && StringHelper.encodeHtml(timeRange.name),
        labelTpl = name || icon ? `<label>${icon || ''}${name || '&nbsp;'}</label>` : '',
        bodyConfig = {
          ...config,
          style: config.style + (timeRange.style || ''),
          html: me.bodyRenderer ? me.bodyRenderer({
            timeRange
          }) : me.showHeaderElements && !me.showLabelInBody ? '' : labelTpl
        };
      let headerConfig;
      if (me.showHeaderElements) {
        headerConfig = {
          ...config,
          html: me.headerRenderer ? me.headerRenderer({
            timeRange
          }) : me.showLabelInBody ? '' : labelTpl
        };
      }
      return {
        bodyConfig,
        headerConfig
      };
    }
  }
  // Cache label rotation to not have to calculate for each occurrence when using recurring timeranges
  cacheRotation(range, bodyElement) {
    // Lines have no label. Do not check label content to do not force DOM layout!
    if (!range.iconCls && !range.name || !range.duration) {
      return;
    }
    const label = bodyElement.firstElementChild;
    if (label && !range.recurringTimeSpan) {
      this._labelRotationMap[range.id] = this.client.isVertical ? label.offsetHeight < bodyElement.offsetHeight : label.offsetWidth > bodyElement.offsetWidth;
    }
  }
  applyRotation(range, bodyElement) {
    var _range$recurringTimeS, _bodyElement$firstEle;
    const rotate = this._labelRotationMap[((_range$recurringTimeS = range.recurringTimeSpan) === null || _range$recurringTimeS === void 0 ? void 0 : _range$recurringTimeS.id) ?? range.id];
    (_bodyElement$firstEle = bodyElement.firstElementChild) === null || _bodyElement$firstEle === void 0 ? void 0 : _bodyElement$firstEle.classList.toggle('b-vertical', Boolean(rotate));
  }
  getBodyElementByRecord(idOrRecord) {
    const id = typeof idOrRecord === 'string' ? idOrRecord : idOrRecord === null || idOrRecord === void 0 ? void 0 : idOrRecord.id;
    return id != null && DomSync.getChild(this.bodyCanvas, id);
  }
  // Implement in subclasses
  resolveTimeRangeRecord(el) {}
  get headerContainerElement() {
    const me = this,
      {
        isVertical,
        timeView,
        timeAxisColumn
      } = me.client;
    if (!me._headerContainerElement) {
      // Render into the subGrid´s header element or the vertical timeaxis depending on mode
      if (isVertical && timeView.element) {
        me._headerContainerElement = timeView.element.parentElement;
      } else if (!isVertical) {
        me._headerContainerElement = timeAxisColumn.element;
      }
    }
    return me._headerContainerElement;
  }
  //endregion
  //region Settings
  get showHeaderElements() {
    return !this.client.hideHeaders && this._showHeaderElements;
  }
  updateShowHeaderElements(show) {
    const {
      client
    } = this;
    if (!this.isConfiguring) {
      client.element.classList.toggle('b-sch-timeranges-with-headerelements', Boolean(show));
      this.renderRanges();
    }
  }
  //endregion
  //region Menu items
  /**
   * Adds menu items for the context menu, and may mutate the menu configuration.
   * @param {Object} options Contains menu items and extra data retrieved from the menu target.
   * @param {Grid.column.Column} options.column Column for which the menu will be shown
   * @param {Object<String,MenuItemConfig|Boolean|null>} options.items A named object to describe menu items
   * @internal
   */
  populateTimeAxisHeaderMenu({
    column,
    items
  }) {}
  //endregion
  //region Events & hooks
  onPaint({
    firstPaint
  }) {
    if (firstPaint && this.client.isHorizontal) {
      this.onUIReady();
    }
  }
  onSchedulerHorizontalScroll() {
    // Don't need a refresh, ranges are already available. Just need to draw those now in view
    this.client.isHorizontal && this.renderRanges();
  }
  afterScroll() {
    this.client.isVertical && this.renderRanges();
  }
  updateLineBuffer() {
    const {
      timeAxisViewModel
    } = this.client;
    // Lines have no duration, but we want them to be visible longer for the label to not suddenly disappear.
    // We use a 300px buffer for that, recalculated as an amount of ms
    this._lineBufferDurationMS = timeAxisViewModel.getDateFromPosition(300) - timeAxisViewModel.getDateFromPosition(0);
  }
  onInternalResize(element, newWidth, newHeight, oldWidth, oldHeight) {
    if (this.client.isVertical && oldHeight !== newHeight) {
      this.renderRanges();
    }
  }
  onTimeAxisViewModelUpdate() {
    this.updateLineBuffer();
    this.refresh();
  }
  onTimeRangeClick(event) {
    const timeRangeRecord = this.resolveTimeRangeRecord(event.target);
    this.client.trigger(`timeRangeHeader${StringHelper.capitalize(event.type)}`, {
      event,
      domEvent: event,
      timeRangeRecord
    });
  }
  //endregion
  //region Drag drop
  showTip(context) {
    const me = this;
    if (me.showTooltip) {
      me.clockTemplate = new ClockTemplate({
        scheduler: me.client
      });
      me.tip = new Tooltip(ObjectHelper.assign({
        id: `${me.client.id}-time-range-tip`,
        cls: 'b-interaction-tooltip',
        align: 'b-t',
        autoShow: true,
        updateContentOnMouseMove: true,
        forElement: context.element,
        getHtml: () => me.getTipHtml(context.record, context.element)
      }, me.showTooltip));
    }
  }
  destroyTip() {
    if (this.tip) {
      this.tip.destroy();
      this.tip = null;
    }
  }
  isElementDraggable(el) {
    el = el.closest(this.baseSelector + ':not(.b-resizing):not(.b-readonly)');
    return el && !el.classList.contains('b-over-resize-handle');
  }
  onDragStart({
    context
  }) {
    const {
      client,
      drag
    } = this;
    if (client.isVertical) {
      drag.minY = 0;
      // Moving the range, you can drag the start marker down until the end of the range hits the time axis end
      drag.maxY = client.timeAxisViewModel.totalSize - context.element.offsetHeight;
      // Setting min/max for X makes drag right of the header valid, but visually still constrained vertically
      drag.minX = 0;
      drag.maxX = Number.MAX_SAFE_INTEGER;
    } else {
      drag.minX = 0;
      // Moving the range, you can drag the start marker right until the end of the range hits the time axis end
      drag.maxX = client.timeAxisViewModel.totalSize - context.element.offsetWidth;
      // Setting min/max for Y makes drag below header valid, but visually still constrained horizontally
      drag.minY = 0;
      drag.maxY = Number.MAX_SAFE_INTEGER;
    }
    client.element.classList.add('b-dragging-timerange');
  }
  onDrop({
    context
  }) {
    this.client.element.classList.remove('b-dragging-timerange');
  }
  onInvalidDrop() {
    this.drag.reset();
    this.client.element.classList.remove('b-dragging-timerange');
    this.destroyTip();
  }
  onDrag() {}
  onDragReset() {}
  // endregion
  // region Resize
  onResizeStart() {}
  onResizeDrag() {}
  onResize() {}
  onInvalidResize() {}
  onResizeReset() {}
  //endregion
  //region Tooltip
  /**
   * Generates the html to display in the tooltip during drag drop.
   *
   */
  getTipHtml(record, element) {
    const me = this,
      {
        client
      } = me,
      box = Rectangle.from(element),
      startPos = box.getStart(client.rtl, client.isHorizontal),
      endPos = box.getEnd(client.rtl, client.isHorizontal),
      startDate = client.getDateFromCoordinate(startPos, 'round', false),
      endDate = record.endDate && client.getDateFromCoordinate(endPos, 'round', false),
      startText = client.getFormattedDate(startDate),
      endText = endDate && client.getFormattedEndDate(endDate, startDate);
    return me.dragTipTemplate({
      name: record.name || '',
      startDate,
      endDate,
      startText,
      endText,
      startClockHtml: me.clockTemplate.template({
        date: startDate,
        text: startText,
        cls: 'b-sch-tooltip-startdate'
      }),
      endClockHtml: endText && me.clockTemplate.template({
        date: endDate,
        text: endText,
        cls: 'b-sch-tooltip-enddate'
      })
    });
  }
  //endregion
}

AbstractTimeRanges._$name = 'AbstractTimeRanges';

/**
 * @module Scheduler/feature/ColumnLines
 */
const emptyObject$1 = Object.freeze({});
/**
 * Displays column lines for ticks, with a different styling for major ticks (by default they are darker). If this
 * feature is disabled, no lines are shown. If it's enabled, line are shown for the tick level which is set in current
 * ViewPreset. Please see {@link Scheduler.preset.ViewPreset#field-columnLinesFor} config for details.
 *
 * The lines are drawn as divs, with only visible lines available in DOM. The color and style of the lines are
 * determined the css rules for `.b-column-line` and `.b-column-line-major`.
 *
 * For vertical mode, this features also draws vertical resource column lines if scheduler is configured with
 * `columnLines : true` (which is the default, see {@link Grid.view.GridBase#config-columnLines}).
 *
 * This feature is **enabled** by default
 *
 * @extends Core/mixin/InstancePlugin
 * @mixes Core/mixin/Delayable
 * @demo Scheduler/basic
 * @inlineexample Scheduler/feature/ColumnLines.js
 * @classtype columnLines
 * @feature
 */
class ColumnLines extends InstancePlugin.mixin(AttachToProjectMixin, Delayable) {
  //region Config
  static get $name() {
    return 'ColumnLines';
  }
  static get delayable() {
    return {
      refresh: {
        type: 'raf',
        cancelOutstanding: true
      }
    };
  }
  // Plugin configuration. This plugin chains some of the functions in Grid.
  static get pluginConfig() {
    return {
      after: ['render', 'updateCanvasSize', 'onVisibleDateRangeChange', 'onVisibleResourceRangeChange']
    };
  }
  //endregion
  //region Init & destroy
  construct(client, config) {
    client.useBackgroundCanvas = true;
    super.construct(client, config);
  }
  attachToResourceStore(resourceStore) {
    const {
      client
    } = this;
    super.attachToResourceStore(resourceStore);
    if (client.isVertical) {
      client.resourceStore.ion({
        name: 'resourceStore',
        group({
          groupers
        }) {
          if (groupers.length === 0) {
            this.refresh();
          }
        },
        thisObj: this
      });
    }
  }
  doDisable(disable) {
    super.doDisable(disable);
    if (!this.isConfiguring) {
      this.refresh();
    }
  }
  //endregion
  //region Draw
  /**
   * Draw lines when scheduler/gantt is rendered.
   * @private
   */
  render() {
    this.refresh();
  }
  getColumnLinesDOMConfig(startDate, endDate) {
    const me = this,
      {
        client
      } = me,
      {
        rtl
      } = client,
      m = rtl ? -1 : 1,
      {
        timeAxisViewModel,
        isHorizontal,
        resourceStore,
        variableColumnWidths
      } = client,
      {
        columnConfig
      } = timeAxisViewModel;
    const linesForLevel = timeAxisViewModel.columnLinesFor,
      majorLinesForLevel = Math.max(linesForLevel - 1, 0),
      start = startDate.getTime(),
      end = endDate.getTime(),
      domConfigs = [],
      dates = new Set(),
      dimension = isHorizontal ? 'X' : 'Y';
    if (!me.disabled) {
      const addLineConfig = (tick, isMajor) => {
        const tickStart = tick.start.getTime();
        // Only start of tick matters.
        // Each tick has an exact calculated start position along the time axis
        // and carries a border on its left, so column lines follow from
        // tick 1 (zero-based) onwards.
        if (tickStart > start && tickStart < end && !dates.has(tickStart)) {
          dates.add(tickStart);
          domConfigs.push({
            role: 'presentation',
            className: isMajor ? 'b-column-line-major' : 'b-column-line',
            style: {
              transform: `translate${dimension}(${tick.coord * m}px)`
            },
            dataset: {
              line: isMajor ? `major-${tick.index}` : `line-${tick.index}`
            }
          });
        }
      };
      // Collect configs for major lines
      if (linesForLevel !== majorLinesForLevel) {
        for (let i = 1; i <= columnConfig[majorLinesForLevel].length - 1; i++) {
          addLineConfig(columnConfig[majorLinesForLevel][i], true);
        }
      }
      // And normal lines, skipping dates already occupied by major lines
      for (let i = 1; i <= columnConfig[linesForLevel].length - 1; i++) {
        addLineConfig(columnConfig[linesForLevel][i], false);
      }
      // Add vertical resource column lines, if grid is configured to show column lines
      if (!isHorizontal && client.columnLines) {
        const {
          columnWidth
        } = client.resourceColumns;
        let {
          first: firstResource,
          last: lastResource
        } = client.currentOrientation.getResourceRange(true);
        let nbrGroupHeaders = 0;
        if (firstResource > -1) {
          for (let i = firstResource; i < lastResource + 1; i++) {
            var _instanceMeta$groupPa, _instanceMeta$groupPa2;
            const resourceRecord = resourceStore.getAt(i);
            // Only add lines for group children
            if (resourceRecord.isGroupHeader) {
              lastResource++;
              nbrGroupHeaders++;
              continue;
            }
            const instanceMeta = resourceRecord.instanceMeta(resourceStore),
              left = variableColumnWidths ? instanceMeta.insetStart + resourceRecord.columnWidth - 1 : (i - nbrGroupHeaders + 1) * columnWidth - 1;
            domConfigs.push({
              className: {
                'b-column-line': 1,
                'b-resource-column-line': 1,
                'b-resource-group-divider': resourceStore.isGrouped && ((_instanceMeta$groupPa = instanceMeta.groupParent) === null || _instanceMeta$groupPa === void 0 ? void 0 : _instanceMeta$groupPa.groupChildren[((_instanceMeta$groupPa2 = instanceMeta.groupParent) === null || _instanceMeta$groupPa2 === void 0 ? void 0 : _instanceMeta$groupPa2.groupChildren.length) - 1]) === resourceRecord
              },
              style: {
                transform: `translateX(${left * m}px)`
              },
              dataset: {
                line: `resource-${i}`
              }
            });
          }
        }
      }
    }
    return domConfigs;
  }
  /**
   * Draw column lines that are in view
   * @private
   */
  refresh() {
    const me = this,
      {
        client
      } = me,
      {
        timeAxis
      } = client,
      {
        startDate,
        endDate
      } = client.visibleDateRange || emptyObject$1,
      axisStart = timeAxis.startDate;
    // Early bailout for timeaxis without start date or when starting with schedule collapsed
    if (!axisStart || !startDate || me.client.timeAxisSubGrid.collapsed) {
      return;
    }
    if (!me.element) {
      me.element = DomHelper.createElement({
        parent: client.backgroundCanvas,
        className: 'b-column-lines-canvas'
      });
    }
    const domConfigs = me.getColumnLinesDOMConfig(startDate, endDate);
    DomSync.sync({
      targetElement: me.element,
      onlyChildren: true,
      domConfig: {
        children: domConfigs,
        syncOptions: {
          // When zooming in and out we risk getting a lot of released lines if we do not limit it
          releaseThreshold: 4
        }
      },
      syncIdField: 'line'
    });
  }
  //endregion
  //region Events
  // Called when visible date range changes, for example from zooming, scrolling, resizing
  onVisibleDateRangeChange() {
    this.refresh();
  }
  // Called when visible resource range changes, for example on scroll and resize
  onVisibleResourceRangeChange({
    firstResource,
    lastResource
  }) {
    this.refresh();
  }
  updateCanvasSize() {
    this.refresh();
  }
  //endregion
}

ColumnLines._$name = 'ColumnLines';
GridFeatureManager.registerFeature(ColumnLines, true, ['Scheduler', 'Gantt']);

/**
 * @module Scheduler/feature/mixin/DependencyCreation
 */
/**
 * Mixin for Dependencies feature that handles dependency creation (drag & drop from terminals which are shown on hover).
 * Requires {@link Core.mixin.Delayable} to be mixed in alongside.
 *
 * @mixin
 */
var DependencyCreation = (Target => class DependencyCreation extends (Target || Base$1) {
  static get $name() {
    return 'DependencyCreation';
  }
  //region Config
  static get defaultConfig() {
    return {
      /**
       * `false` to require a drop on a target event bar side circle to define the dependency type.
       * If dropped on the event bar, the `defaultValue` of the DependencyModel `type` field will be used to
       * determine the target task side.
       *
       * @member {Boolean} allowDropOnEventBar
       */
      /**
       * `false` to require a drop on a target event bar side circle to define the dependency type.
       * If dropped on the event bar, the `defaultValue` of the DependencyModel `type` field will be used to
       * determine the target task side.
       *
       * @config {Boolean}
       * @default
       */
      allowDropOnEventBar: true,
      /**
       * `false` to not show a tooltip while creating a dependency
       * @config {Boolean}
       * @default
       */
      showCreationTooltip: true,
      /**
       * A tooltip config object that will be applied to the dependency creation {@link Core.widget.Tooltip}
       * @config {TooltipConfig}
       */
      creationTooltip: null,
      /**
       * A template function that will be called to generate the HTML contents of the dependency creation tooltip.
       * You can return either an HTML string or a {@link DomConfig} object.
       * @prp {Function} creationTooltipTemplate
       * @param {Object} data Data about the dependency being created
       * @param {Scheduler.model.TimeSpan} data.source The from event
       * @param {Scheduler.model.TimeSpan} data.target The target event
       * @param {String} data.fromSide The from side (start, end, top, bottom)
       * @param {String} data.toSide The target side (start, end, top, bottom)
       * @param {Boolean} data.valid The validity of the dependency
       * @returns {String|DomConfig}
       */
      /**
       * CSS class used for terminals
       * @config {String}
       * @default
       */
      terminalCls: 'b-sch-terminal',
      /**
       * Where (on event bar edges) to display terminals. The sides are `'start'`, `'top'`,
       * `'end'` and `'bottom'`
       * @config {String[]}
       */
      terminalSides: ['start', 'top', 'end', 'bottom'],
      /**
       * Set to `false` to not allow creating dependencies
       * @config {Boolean}
       * @default
       */
      allowCreate: true
    };
  }
  //endregion
  //region Init & destroy
  construct(view, config) {
    super.construct(view, config);
    const me = this;
    me.view = view;
    me.eventName = view.scheduledEventName;
    view.ion({
      readOnly: () => me.updateCreateListeners()
    });
    me.updateCreateListeners();
    me.chain(view, 'onElementTouchMove', 'onElementTouchMove');
  }
  doDestroy() {
    var _me$pointerUpMoveDeta, _me$creationTooltip;
    const me = this;
    me.detachListeners('view');
    me.creationData = null;
    (_me$pointerUpMoveDeta = me.pointerUpMoveDetacher) === null || _me$pointerUpMoveDeta === void 0 ? void 0 : _me$pointerUpMoveDeta.call(me);
    (_me$creationTooltip = me.creationTooltip) === null || _me$creationTooltip === void 0 ? void 0 : _me$creationTooltip.destroy();
    super.doDestroy();
  }
  updateCreateListeners() {
    const me = this;
    if (!me.view) {
      return;
    }
    me.detachListeners('view');
    if (me.isCreateAllowed) {
      me.view.ion({
        name: 'view',
        [`${me.eventName}mouseenter`]: 'onTimeSpanMouseEnter',
        [`${me.eventName}mouseleave`]: 'onTimeSpanMouseLeave',
        thisObj: me
      });
    }
  }
  set allowCreate(value) {
    this._allowCreate = value;
    this.updateCreateListeners();
  }
  get allowCreate() {
    return this._allowCreate;
  }
  get isCreateAllowed() {
    return this.allowCreate && !this.view.readOnly && !this.disabled;
  }
  //endregion
  //region Events
  /**
   * Show terminals when mouse enters event/task element
   * @private
   */
  onTimeSpanMouseEnter({
    event,
    source,
    [`${this.eventName}Record`]: record,
    [`${this.eventName}Element`]: element
  }) {
    if (!record.isCreating && !record.readOnly && (!this.client.features.nestedEvents || record.parent.isRoot)) {
      const me = this,
        {
          creationData
        } = me,
        eventBarElement = DomHelper.down(element, source.eventInnerSelector);
      // When we enter a different event than the one we started on
      if (record !== (creationData === null || creationData === void 0 ? void 0 : creationData.source)) {
        me.showTerminals(record, eventBarElement);
        if (creationData && event.target.closest(me.client.eventSelector)) {
          creationData.timeSpanElement = eventBarElement;
          me.onOverTargetEventBar(event);
        }
      }
    }
  }
  /**
   * Hide terminals when mouse leaves event/task element
   * @private
   */
  onTimeSpanMouseLeave(event) {
    var _event$event;
    const me = this,
      {
        creationData
      } = me,
      element = event[`${me.eventName}Element`],
      timeSpanLeft = DomHelper.down(element, me.view.eventInnerSelector),
      target = (_event$event = event.event) === null || _event$event === void 0 ? void 0 : _event$event.relatedTarget,
      timeSpanElement = creationData === null || creationData === void 0 ? void 0 : creationData.timeSpanElement;
    // Can happen when unhovering an occurrence during update
    if (!target) {
      return;
    }
    if (!creationData || !timeSpanElement || !target || !DomHelper.isDescendant(timeSpanElement, target)) {
      // We cannot hide the terminals for non-trusted events because non-trusted means it's
      // synthesized from a touchmove event and if the source element of a touchmove
      // leaves the DOM, the touch gesture is ended.
      if (event.event.isTrusted || timeSpanLeft !== (creationData === null || creationData === void 0 ? void 0 : creationData.sourceElement)) {
        me.hideTerminals(element);
      }
    }
    if (creationData && !creationData.finalizing) {
      creationData.timeSpanElement = null;
      me.onOverNewTargetWhileCreating(undefined, undefined, event);
    }
  }
  onTerminalMouseOver(event) {
    if (this.creationData) {
      this.onOverTargetEventBar(event);
    }
  }
  /**
   * Remove hover styling when mouse leaves terminal. Also hides terminals when mouse leaves one it and not creating a
   * dependency.
   * @private
   */
  onTerminalMouseOut(event) {
    const me = this,
      {
        creationData
      } = me,
      eventElement = event.target.closest(me.view.eventSelector);
    if (eventElement && (!me.showingTerminalsFor || !DomHelper.isDescendant(eventElement, me.showingTerminalsFor)) && (!creationData || eventElement !== creationData.timeSpanElement)) {
      me.hideTerminals(eventElement);
      me.view.unhover(eventElement, event);
    }
    if (creationData) {
      me.onOverNewTargetWhileCreating(event.relatedTarget, creationData.target, event);
    }
  }
  /**
   * Start creating a dependency when mouse is pressed over terminal
   * @private
   */
  onTerminalPointerDown(event) {
    const me = this;
    // ignore non-left button clicks
    if (event.button === 0 && !me.creationData) {
      var _scheduler$resolveRes;
      const scheduler = me.view,
        timeAxisSubGridElement = scheduler.timeAxisSubGridElement,
        terminalNode = event.target,
        timeSpanElement = terminalNode.closest(scheduler.eventInnerSelector),
        viewBounds = Rectangle.from(scheduler.element, document.body);
      event.stopPropagation();
      me.creationData = {
        sourceElement: timeSpanElement,
        source: scheduler.resolveTimeSpanRecord(timeSpanElement).$original,
        fromSide: terminalNode.dataset.side,
        startPoint: Rectangle.from(terminalNode, timeAxisSubGridElement).center,
        startX: event.pageX - viewBounds.x + scheduler.scrollLeft,
        startY: event.pageY - viewBounds.y + scheduler.scrollTop,
        valid: false,
        sourceResource: (_scheduler$resolveRes = scheduler.resolveResourceRecord) === null || _scheduler$resolveRes === void 0 ? void 0 : _scheduler$resolveRes.call(scheduler, event),
        tooltip: me.creationTooltip
      };
      me.pointerUpMoveDetacher = EventHelper.on({
        pointerup: {
          element: scheduler.element.getRootNode(),
          handler: 'onMouseUp',
          passive: false
        },
        pointermove: {
          element: timeAxisSubGridElement,
          handler: 'onMouseMove',
          passive: false
        },
        thisObj: me
      });
      // If root element is anything but Document (it could be Document Fragment or regular Node in case of LWC)
      // then we should also add listener to document to cancel dependency creation
      me.documentPointerUpDetacher = EventHelper.on({
        pointerup: {
          element: document,
          handler: 'onDocumentMouseUp'
        },
        keydown: {
          element: document,
          handler: ({
            key
          }) => {
            if (key === 'Escape') {
              me.abort();
            }
          }
        },
        thisObj: me
      });
    }
  }
  onElementTouchMove(event) {
    var _super$onElementTouch;
    (_super$onElementTouch = super.onElementTouchMove) === null || _super$onElementTouch === void 0 ? void 0 : _super$onElementTouch.call(this, event);
    if (this.connector) {
      // Prevent touch scrolling while dragging a connector
      event.preventDefault();
    }
  }
  /**
   * Update connector line showing dependency between source and target when mouse moves. Also check if mouse is over
   * a valid target terminal
   * @private
   */
  onMouseMove(event) {
    const me = this,
      {
        view,
        creationData: data
      } = me,
      viewBounds = Rectangle.from(view.element, document.body),
      deltaX = event.pageX - viewBounds.x + view.scrollLeft - data.startX,
      deltaY = event.pageY - viewBounds.y + view.scrollTop - data.startY,
      length = Math.round(Math.sqrt(deltaX * deltaX + deltaY * deltaY)) - 3,
      angle = Math.atan2(deltaY, deltaX);
    let {
      connector
    } = me;
    if (!connector) {
      if (me.onRequestDragCreate(event) === false) {
        return;
      }
      connector = me.connector;
    }
    connector.style.width = `${length}px`;
    connector.style.transform = `rotate(${angle}rad)`;
    me.lastMouseMoveEvent = event;
  }
  onRequestDragCreate(event) {
    const me = this,
      {
        view,
        creationData: data
      } = me;
    /**
     * Fired on the owning Scheduler/Gantt before a dependency creation drag operation starts. Return `false` to
     * prevent it
     * @event beforeDependencyCreateDrag
     * @on-owner
     * @param {Scheduler.model.TimeSpan} source The source task
     */
    if (view.trigger('beforeDependencyCreateDrag', {
      data,
      source: data.source
    }) === false) {
      me.abort();
      return false;
    }
    view.element.classList.add('b-creating-dependency');
    me.createConnector(data.startPoint.x, data.startPoint.y);
    /**
     * Fired on the owning Scheduler/Gantt when a dependency creation drag operation starts
     * @event dependencyCreateDragStart
     * @on-owner
     * @param {Scheduler.model.TimeSpan} source The source task
     */
    view.trigger('dependencyCreateDragStart', {
      data,
      source: data.source
    });
    if (me.showCreationTooltip) {
      const tip = me.creationTooltip || (me.creationTooltip = me.createDragTooltip());
      me.creationData.tooltip = tip;
      tip.disabled = false;
      tip.show();
      tip.onMouseMove(event);
    }
    view.scrollManager.startMonitoring({
      scrollables: [{
        element: view.timeAxisSubGrid.scrollable.element,
        direction: 'horizontal'
      }, {
        element: view.scrollable.element,
        direction: 'vertical'
      }],
      callback: () => me.lastMouseMoveEvent && me.onMouseMove(me.lastMouseMoveEvent)
    });
  }
  onOverTargetEventBar(event) {
    var _overEventRecord;
    const me = this,
      {
        view,
        creationData: data,
        allowDropOnEventBar
      } = me,
      {
        target
      } = event;
    let overEventRecord = view.resolveTimeSpanRecord(target).$original;
    // use main event if a segment resolved
    if ((_overEventRecord = overEventRecord) !== null && _overEventRecord !== void 0 && _overEventRecord.isEventSegment) {
      overEventRecord = overEventRecord.event;
    }
    if (Objects.isPromise(data.valid) || !allowDropOnEventBar && !target.classList.contains(me.terminalCls)) {
      return;
    }
    if (overEventRecord !== data.source) {
      me.onOverNewTargetWhileCreating(target, overEventRecord, event);
    }
  }
  async onOverNewTargetWhileCreating(targetElement, overEventRecord, event) {
    const me = this,
      {
        view,
        creationData: data,
        allowDropOnEventBar,
        connector
      } = me;
    if (Objects.isPromise(data.valid)) {
      return;
    }
    // stop target updating if dependency finalizing in progress
    if (data.finalizing) {
      return;
    }
    // Connector might not exist at this point because `pointerout` on the terminal might fire before `pointermove`
    // on the time axis subgrid. This is difficult to reproduce, so shouldn't be triggered often.
    // https://github.com/bryntum/support/issues/3116#issuecomment-894256799
    if (!connector) {
      return;
    }
    connector.classList.remove('b-valid', 'b-invalid');
    data.timeSpanElement && DomHelper.removeClsGlobally(data.timeSpanElement, 'b-sch-terminal-active');
    if (!overEventRecord || overEventRecord === data.source || !allowDropOnEventBar && !targetElement.classList.contains(me.terminalCls)) {
      data.target = data.toSide = null;
      data.valid = false;
      connector.classList.add('b-invalid');
    } else {
      var _data$timeSpanElement, _data$timeSpanElement2;
      const target = data.target = overEventRecord,
        {
          source
        } = data;
      let toSide = targetElement.dataset.side;
      // If we allow dropping anywhere on a task, resolve target side based on the default type of the
      // dependency model used
      if (allowDropOnEventBar && !targetElement.classList.contains(me.terminalCls)) {
        toSide = me.getTargetSideFromType(me.dependencyStore.modelClass.fieldMap.type.defaultValue || DependencyBaseModel.Type.EndToStart);
      }
      if (view.resolveResourceRecord) {
        data.targetResource = view.resolveResourceRecord(event);
      }
      let dependencyType;
      data.toSide = toSide;
      const fromSide = data.fromSide,
        updateValidity = valid => {
          if (!me.isDestroyed) {
            data.valid = valid;
            targetElement.classList.add(valid ? 'b-valid' : 'b-invalid');
            connector.classList.add(valid ? 'b-valid' : 'b-invalid');
            /**
             * Fired on the owning Scheduler/Gantt when asynchronous dependency validation completes
             * @event dependencyValidationComplete
             * @on-owner
             * @param {Scheduler.model.TimeSpan} source The source task
             * @param {Scheduler.model.TimeSpan} target The target task
             * @param {Number} dependencyType The dependency type, see {@link Scheduler.model.DependencyBaseModel#property-Type-static}
             */
            view.trigger('dependencyValidationComplete', {
              data,
              source,
              target,
              dependencyType
            });
          }
        };
      // NOTE: Top/Bottom sides are not taken into account due to
      //       scheduler doesn't check for type value anyway, whereas
      //       gantt will reject any other dependency types undefined in
      //       DependencyBaseModel.Type enumeration.
      switch (true) {
        case fromSide === 'start' && toSide === 'start':
          dependencyType = DependencyBaseModel.Type.StartToStart;
          break;
        case fromSide === 'start' && toSide === 'end':
          dependencyType = DependencyBaseModel.Type.StartToEnd;
          break;
        case fromSide === 'end' && toSide === 'start':
          dependencyType = DependencyBaseModel.Type.EndToStart;
          break;
        case fromSide === 'end' && toSide === 'end':
          dependencyType = DependencyBaseModel.Type.EndToEnd;
          break;
      }
      /**
       * Fired on the owning Scheduler/Gantt when asynchronous dependency validation starts
       * @event dependencyValidationStart
       * @on-owner
       * @param {Scheduler.model.TimeSpan} source The source task
       * @param {Scheduler.model.TimeSpan} target The target task
       * @param {Number} dependencyType The dependency type, see {@link Scheduler.model.DependencyBaseModel#property-Type-static}
       */
      view.trigger('dependencyValidationStart', {
        data,
        source,
        target,
        dependencyType
      });
      let valid = data.valid = me.dependencyStore.isValidDependency(source, target, dependencyType);
      // Promise is returned when using the engine
      if (Objects.isPromise(valid)) {
        valid = await valid;
        updateValidity(valid);
      } else {
        updateValidity(valid);
      }
      const validityCls = valid ? 'b-valid' : 'b-invalid';
      connector.classList.add(validityCls);
      (_data$timeSpanElement = data.timeSpanElement) === null || _data$timeSpanElement === void 0 ? void 0 : (_data$timeSpanElement2 = _data$timeSpanElement.querySelector(`.b-sch-terminal[data-side=${toSide}]`)) === null || _data$timeSpanElement2 === void 0 ? void 0 : _data$timeSpanElement2.classList.add('b-sch-terminal-active', validityCls);
    }
    me.updateCreationTooltip();
  }
  /**
   * Create a new dependency if mouse release over valid terminal. Hides connector
   * @private
   */
  async onMouseUp() {
    var _me$pointerUpMoveDeta2;
    const me = this,
      data = me.creationData;
    data.finalizing = true;
    (_me$pointerUpMoveDeta2 = me.pointerUpMoveDetacher) === null || _me$pointerUpMoveDeta2 === void 0 ? void 0 : _me$pointerUpMoveDeta2.call(me);
    if (data.valid) {
      /**
       * Fired on the owning Scheduler/Gantt when a dependency drag creation operation is about to finalize
       *
       * @event beforeDependencyCreateFinalize
       * @on-owner
       * @preventable
       * @async
       * @param {Scheduler.model.TimeSpan} source The source task
       * @param {Scheduler.model.TimeSpan} target The target task
       * @param {'start'|'end'|'top'|'bottom'} fromSide The from side (start / end / top / bottom)
       * @param {'start'|'end'|'top'|'bottom'} toSide The to side (start / end / top / bottom)
       */
      const result = await me.view.trigger('beforeDependencyCreateFinalize', data);
      if (result === false) {
        data.valid = false;
      }
      // Await any async validation logic before continuing
      else if (Objects.isPromise(data.valid)) {
        data.valid = await data.valid;
      }
      if (data.valid) {
        let dependency = me.createDependency(data);
        if (dependency !== null) {
          if (Objects.isPromise(dependency)) {
            dependency = await dependency;
          }
          data.dependency = dependency;
          /**
           * Fired on the owning Scheduler/Gantt when a dependency drag creation operation succeeds
           * @event dependencyCreateDrop
           * @on-owner
           * @param {Scheduler.model.TimeSpan} source The source task
           * @param {Scheduler.model.TimeSpan} target The target task
           * @param {Scheduler.model.DependencyBaseModel} dependency The created dependency
           */
          me.view.trigger('dependencyCreateDrop', {
            data,
            source: data.source,
            target: data.target,
            dependency
          });
          me.doAfterDependencyDrop(data);
        }
      } else {
        me.doAfterDependencyDrop(data);
      }
    } else {
      data.valid = false;
      me.doAfterDependencyDrop(data);
    }
    me.abort();
  }
  doAfterDependencyDrop(data) {
    /**
     * Fired on the owning Scheduler/Gantt after a dependency drag creation operation finished, no matter to outcome
     * @event afterDependencyCreateDrop
     * @on-owner
     * @param {Scheduler.model.TimeSpan} source The source task
     * @param {Scheduler.model.TimeSpan} target The target task
     * @param {Scheduler.model.DependencyBaseModel} dependency The created dependency
     */
    this.view.trigger('afterDependencyCreateDrop', {
      data,
      ...data
    });
  }
  onDocumentMouseUp({
    target
  }) {
    if (!this.view.timeAxisSubGridElement.contains(target)) {
      this.abort();
    }
  }
  /**
   * Aborts dependency creation, removes proxy and cleans up listeners
   */
  abort() {
    var _me$pointerUpMoveDeta3, _me$documentPointerUp;
    const me = this,
      {
        view,
        creationData
      } = me;
    // Remove terminals from source and target events.
    if (creationData) {
      const {
        source,
        sourceResource,
        target,
        targetResource
      } = creationData;
      if (source) {
        const el = view.getElementFromEventRecord(source, sourceResource);
        if (el) {
          me.hideTerminals(el);
        }
      }
      if (target) {
        const el = view.getElementFromEventRecord(target, targetResource);
        if (el) {
          me.hideTerminals(el);
        }
      }
    }
    if (me.creationTooltip) {
      me.creationTooltip.disabled = true;
    }
    me.creationData = me.lastMouseMoveEvent = null;
    (_me$pointerUpMoveDeta3 = me.pointerUpMoveDetacher) === null || _me$pointerUpMoveDeta3 === void 0 ? void 0 : _me$pointerUpMoveDeta3.call(me);
    (_me$documentPointerUp = me.documentPointerUpDetacher) === null || _me$documentPointerUp === void 0 ? void 0 : _me$documentPointerUp.call(me);
    me.removeConnector();
  }
  //endregion
  //region Connector
  /**
   * Creates a connector line that visualizes dependency source & target
   * @private
   */
  createConnector(x, y) {
    const me = this,
      {
        view
      } = me;
    me.clearTimeout(me.removeConnectorTimeout);
    me.connector = DomHelper.createElement({
      parent: view.timeAxisSubGridElement,
      className: `${me.baseCls}-connector`,
      style: `left:${x}px;top:${y}px`
    });
    view.element.classList.add('b-creating-dependency');
  }
  createDragTooltip() {
    const me = this,
      {
        view
      } = me;
    return me.creationTooltip = Tooltip.new({
      id: `${view.id}-dependency-drag-tip`,
      cls: 'b-sch-dependency-creation-tooltip',
      loadingMsg: '',
      anchorToTarget: false,
      // Keep tip visible until drag drop operation is finalized
      forElement: view.timeAxisSubGridElement,
      trackMouse: true,
      // Do not constrain at all, want it to be able to go outside of the viewport to not get in the way
      constrainTo: null,
      header: {
        dock: 'right'
      },
      internalListeners: {
        // Show initial content immediately
        beforeShow: 'updateCreationTooltip',
        thisObj: me
      }
    }, me.creationTooltip);
  }
  /**
   * Remove connector
   * @private
   */
  removeConnector() {
    const me = this,
      {
        connector,
        view
      } = me;
    if (connector) {
      connector.classList.add('b-removing');
      connector.style.width = '0';
      me.removeConnectorTimeout = me.setTimeout(() => {
        connector.remove();
        me.connector = null;
      }, 200);
    }
    view.element.classList.remove('b-creating-dependency');
    me.creationTooltip && me.creationTooltip.hide();
    view.scrollManager.stopMonitoring();
  }
  //endregion
  //region Terminals
  /**
   * Show terminals for specified event at sides defined in #terminalSides.
   * @param {Scheduler.model.TimeSpan} timeSpanRecord Event/task to show terminals for
   * @param {HTMLElement} element Event/task element
   */
  showTerminals(timeSpanRecord, element) {
    const me = this;
    // Record not part of project is a transient record in a display store, not meant to be manipulated
    if (!me.isCreateAllowed || !timeSpanRecord.project) {
      return;
    }
    const cls = me.terminalCls,
      terminalsVisibleCls = `${cls}s-visible`;
    // We operate on the event bar, not the wrap
    element = DomHelper.down(element, me.view.eventInnerSelector);
    // bail out if terminals already shown or if view is readonly
    // do not draw new terminals if we are resizing event
    if (!element.classList.contains(terminalsVisibleCls) && !me.view.element.classList.contains('b-resizing-event') && !me.view.readOnly) {
      /**
       * Fired on the owning Scheduler/Gantt before showing dependency terminals on a task or event. Return `false` to
       * prevent it
       * @event beforeShowTerminals
       * @on-owner
       * @param {Scheduler.model.TimeSpan} source The hovered task
       */
      if (me.client.trigger('beforeShowTerminals', {
        source: timeSpanRecord
      }) === false) {
        return;
      }
      // create terminals for desired sides
      me.terminalSides.forEach(side => {
        // Allow code to use left for the start side and right for the end side
        side = me.fixSide(side);
        const terminal = DomHelper.createElement({
          parent: element,
          className: `${cls} ${cls}-${side}`,
          dataset: {
            side,
            feature: true
          }
        });
        terminal.detacher = EventHelper.on({
          element: terminal,
          mouseover: 'onTerminalMouseOver',
          mouseout: 'onTerminalMouseOut',
          // Needs to be pointerdown to match DragHelper, otherwise will be preventing wrong event
          pointerdown: {
            handler: 'onTerminalPointerDown',
            capture: true
          },
          thisObj: me
        });
      });
      element.classList.add(terminalsVisibleCls);
      timeSpanRecord.internalCls.add(terminalsVisibleCls);
      me.showingTerminalsFor = element;
    }
  }
  fixSide(side) {
    if (side === 'left') {
      return 'start';
    }
    if (side === 'right') {
      return 'end';
    }
    return side;
  }
  /**
   * Hide terminals for specified event
   * @param {HTMLElement} eventElement Event element
   */
  hideTerminals(eventElement) {
    // remove all terminals
    const me = this,
      eventParams = me.client.getTimeSpanMouseEventParams(eventElement),
      timeSpanRecord = eventParams === null || eventParams === void 0 ? void 0 : eventParams[`${me.eventName}Record`],
      terminalsVisibleCls = `${me.terminalCls}s-visible`;
    DomHelper.forEachSelector(eventElement, `.${me.terminalCls}`, terminal => {
      terminal.detacher && terminal.detacher();
      terminal.remove();
    });
    DomHelper.down(eventElement, me.view.eventInnerSelector).classList.remove(terminalsVisibleCls);
    timeSpanRecord.internalCls.remove(terminalsVisibleCls);
    me.showingTerminalsFor = null;
  }
  //endregion
  //region Dependency creation
  /**
   * Create a new dependency from source terminal to target terminal
   * @internal
   */
  createDependency(data) {
    const {
        source,
        target,
        fromSide,
        toSide
      } = data,
      type = (fromSide === 'start' ? 0 : 2) + (toSide === 'end' ? 1 : 0);
    const newDependency = this.dependencyStore.add({
      from: source.id,
      to: target.id,
      type,
      fromSide,
      toSide
    });
    return newDependency !== null ? newDependency[0] : null;
  }
  getTargetSideFromType(type) {
    if (type === DependencyBaseModel.Type.StartToStart || type === DependencyBaseModel.Type.EndToStart) {
      return 'start';
    }
    return 'end';
  }
  //endregion
  //region Tooltip
  /**
   * Update dependency creation tooltip
   * @private
   */
  updateCreationTooltip() {
    const me = this,
      data = me.creationData,
      {
        valid
      } = data,
      tip = me.creationTooltip,
      {
        classList
      } = tip.element;
    // Promise, when using engine
    if (Objects.isPromise(valid)) {
      classList.remove('b-invalid');
      classList.add('b-checking');
      return new Promise(resolve => valid.then(valid => {
        data.valid = valid;
        if (!tip.isDestroyed) {
          resolve(me.updateCreationTooltip());
        }
      }));
    }
    tip.html = me.creationTooltipTemplate(data);
  }
  creationTooltipTemplate(data) {
    var _data$target;
    const me = this,
      {
        tooltip,
        valid
      } = data,
      {
        classList
      } = tooltip.element;
    Object.assign(data, {
      fromText: StringHelper.encodeHtml(data.source.name),
      toText: StringHelper.encodeHtml(((_data$target = data.target) === null || _data$target === void 0 ? void 0 : _data$target.name) ?? ''),
      fromSide: data.fromSide,
      toSide: data.toSide || ''
    });
    let tipTitleIconClsSuffix, tipTitleText;
    classList.toggle('b-invalid', !valid);
    classList.remove('b-checking');
    // Valid
    if (valid === true) {
      tipTitleIconClsSuffix = 'valid';
      tipTitleText = me.L('L{Dependencies.valid}');
    }
    // Invalid
    else {
      tipTitleIconClsSuffix = 'invalid';
      tipTitleText = me.L('L{Dependencies.invalid}');
    }
    tooltip.title = `<i class="b-icon b-icon-${tipTitleIconClsSuffix}"></i>${tipTitleText}`;
    return {
      children: [{
        className: 'b-sch-dependency-tooltip',
        children: [{
          dataset: {
            ref: 'fromLabel'
          },
          tag: 'label',
          text: me.L('L{Dependencies.from}')
        }, {
          dataset: {
            ref: 'fromText'
          },
          text: data.fromText
        }, {
          dataset: {
            ref: 'fromBox'
          },
          className: `b-sch-box b-${data.fromSide}`
        }, {
          dataset: {
            ref: 'toLabel'
          },
          tag: 'label',
          text: me.L('L{Dependencies.to}')
        }, {
          dataset: {
            ref: 'toText'
          },
          text: data.toText
        }, {
          dataset: {
            ref: 'toBox'
          },
          className: `b-sch-box b-${data.toSide}`
        }]
      }]
    };
  }
  //endregion
  doDisable(disable) {
    if (!this.isConfiguring) {
      this.updateCreateListeners();
    }
    super.doDisable(disable);
  }
});

const ROWS_PER_CELL = 25;
// Mixin that handles the dependency grid cache
//
// Grid cache explainer
// ────────────────────
// The purpose of the grid cache is to reduce the amount of dependencies we have to iterate over when drawing by
// partitioning them into a virtual grid. With for example 10k deps we would have to iterate over all 10k on
// each draw since any of them might be intersecting the view.
//
// The cells are horizontally based on ticks (50 per cell) and vertically on rows (also 50 per cell. Each cell
// lists which dependencies intersect it. When drawing we only have to iterate over the dependencies for the
// cells that intersect the viewport.
//
// The grid cache is populated when dependencies are drawn. Any change to deps, resources, events or assignments
// clears the cache.
//
// The dependency drawn below will be included in the set that is considered for drawing if tickCell 0 or
// tickCell 1 and rowCell 0 intersects the current view (it is thus represented twice in the grid cache)
//
//       tickCell 0           tickCell 1
//       tick 0-49            tick 50-99
//    ┌────────────────────┬─────────────────────┐
// r r│0,0                 │1,0                  │
// o o│     │              │                     │
// w w│     │     !!!!!!!!!│!!!!!!!!!!!          │
// C  │     │     ! View   │          !          │
// e 0│     │     ! port   │          !          │
// l -│     │     !        │          !          │
// l 4│     └─────!────────┼──────────!────►     │
// 0 9│           !        │          !          │
//    ├────────────────────┼─────────────────────┤
// r r│0,1        !        │1,1       !          │
// o o│           !        │          !          │
// w w│           !!!!!!!!!│!!!!!!!!!!!          │
// C  │                    │                     │
// e 5│                    │                     │
// l 0│                    │                     │
// l -│                    │                     │
// 1 9│                    │                     │
//   9└────────────────────┴─────────────────────┘
//               uoᴉʇɐɹʇsnꞁꞁᴉ əɥɔɐɔ pᴉɹ⅁
var DependencyGridCache = (Target => class DependencyGridCache extends Target {
  static $name = 'DependencyGridCache';
  gridCache = null;
  // Dependencies that might intersect the current viewport and thus should be considered for drawing
  getDependenciesToConsider(startMS, endMS, startIndex, endIndex) {
    const me = this,
      {
        gridCache
      } = me,
      {
        timeAxis
      } = me.client;
    if (gridCache) {
      const dependencies = new Set(),
        fromMSCell = Math.floor((startMS - timeAxis.startMS) / me.MS_PER_CELL),
        toMSCell = Math.floor((endMS - timeAxis.startMS) / me.MS_PER_CELL),
        fromRowCell = Math.floor(startIndex / ROWS_PER_CELL),
        toRowCell = Math.floor(endIndex / ROWS_PER_CELL);
      for (let i = fromMSCell; i <= toMSCell; i++) {
        const msCell = gridCache[i];
        if (msCell) {
          for (let j = fromRowCell; j <= toRowCell; j++) {
            const intersectingDependencies = msCell[j];
            if (intersectingDependencies) {
              for (let i = 0; i < intersectingDependencies.length; i++) {
                dependencies.add(intersectingDependencies[i]);
              }
            }
          }
        }
      }
      return dependencies;
    }
  }
  // A (single) dependency was drawn, we might want to store info about it in the grid cache
  afterDrawDependency(dependency, fromIndex, toIndex, fromDateMS, toDateMS) {
    const me = this;
    if (me.constructGridCache) {
      const {
          MS_PER_CELL
        } = me,
        {
          startMS: timeAxisStartMS,
          endMS: timeAxisEndMS
        } = me.client.timeAxis,
        timeAxisCells = Math.ceil((timeAxisEndMS - timeAxisStartMS) / MS_PER_CELL),
        fromMSCell = Math.floor((fromDateMS - timeAxisStartMS) / MS_PER_CELL),
        toMSCell = Math.floor((toDateMS - timeAxisStartMS) / MS_PER_CELL),
        fromRowCell = Math.floor(fromIndex / ROWS_PER_CELL),
        toRowCell = Math.floor(toIndex / ROWS_PER_CELL),
        firstMSCell = Math.min(fromMSCell, toMSCell),
        lastMSCell = Math.max(fromMSCell, toMSCell),
        firstRowCell = Math.min(fromRowCell, toRowCell),
        lastRowCell = Math.max(fromRowCell, toRowCell);
      // Ignore dependencies fully outside of the time axis
      if (firstMSCell < 0 && lastMSCell < 0 || firstMSCell > timeAxisCells && lastMSCell > timeAxisCells) {
        return;
      }
      // Cache from time axis start, to time axis end ("cropping" deps starting or ending outside)
      const startMSCell = Math.max(firstMSCell, 0),
        endMSCell = Math.min(lastMSCell, timeAxisCells);
      for (let i = startMSCell; i <= endMSCell; i++) {
        const msCell = me.gridCache[i] ?? (me.gridCache[i] = {});
        for (let j = firstRowCell; j <= lastRowCell; j++) {
          const rowCell = msCell[j] ?? (msCell[j] = []);
          rowCell.push(dependency);
        }
      }
    }
  }
  // All dependencies are about to be drawn, check if we need to build the grid cache
  beforeDraw() {
    const me = this;
    if (!me.gridCache) {
      const {
        visibleDateRange
      } = me.client;
      me.constructGridCache = true;
      // Adjust number of ms used in grid cache to match viewport
      me.MS_PER_CELL = Math.max(visibleDateRange.endMS - visibleDateRange.startMS, 1000);
      // Start with empty cache, will be populated as deps are drawn
      me.gridCache = {};
    }
  }
  // All dependencies are drawn, we no longer need to rebuild the cache
  afterDraw() {
    this.constructGridCache = false;
  }
  reset() {
    this.gridCache = null;
  }
});

// Start adjusting if there is system scaling > 130%
const THRESHOLD = Math.min(1 / globalThis.devicePixelRatio, 0.75),
  BOX_PROPERTIES = ['start', 'end', 'top', 'bottom'],
  equalEnough = (a, b) => Math.abs(a - b) < 0.1;
/**
 * @module Scheduler/util/RectangularPathFinder
 */
/**
 * Class which finds rectangular path, i.e. path with 90 degrees turns, between two boxes.
 * @private
 */
class RectangularPathFinder extends Base$1 {
  static get configurable() {
    return {
      /**
       * Default start connection side: 'left', 'right', 'top', 'bottom'
       * @config {'top'|'bottom'|'left'|'right'}
       * @default
       */
      startSide: 'right',
      // /**
      //  * Default start arrow size in pixels
      //  * @config {Number}
      //  * @default
      //  */
      // startArrowSize : 0,
      /**
       * Default start arrow staff size in pixels
       * @config {Number}
       * @default
       */
      startArrowMargin: 12,
      /**
       * Default starting connection point shift from box's arrow pointing side middle point
       * @config {Number}
       * @default
       */
      startShift: 0,
      /**
       * Default end arrow pointing direction, possible values are: 'left', 'right', 'top', 'bottom'
       * @config {'top'|'bottom'|'left'|'right'}
       * @default
       */
      endSide: 'left',
      // /**
      //  * Default end arrow size in pixels
      //  * @config {Number}
      //  * @default
      //  */
      // endArrowSize : 0,
      /**
       * Default end arrow staff size in pixels
       * @config {Number}
       * @default
       */
      endArrowMargin: 12,
      /**
       * Default ending connection point shift from box's arrow pointing side middle point
       * @config {Number}
       * @default
       */
      endShift: 0,
      /**
       * Start / End box vertical margin, the amount of pixels from top and bottom line of a box where drawing
       * is prohibited
       * @config {Number}
       * @default
       */
      verticalMargin: 2,
      /**
       * Start / End box horizontal margin, the amount of pixels from left and right line of a box where drawing
       * @config {Number}
       * @default
       */
      horizontalMargin: 5,
      /**
       * Other rectangular areas (obstacles) to search path through
       * @config {Object[]}
       * @default
       */
      otherBoxes: null,
      /**
       * The owning Scheduler. Mandatory so that it can determin RTL state.
       * @config {Scheduler.view.Scheduler}
       * @private
       */
      client: {}
    };
  }
  /**
   * Returns list of horizontal and vertical segments connecting two boxes
   * <pre>
   *    |    | |  |    |       |
   *  --+----+----+----*-------*---
   *  --+=>Start  +----*-------*--
   *  --+----+----+----*-------*--
   *    |    | |  |    |       |
   *    |    | |  |    |       |
   *  --*----*-+-------+-------+--
   *  --*----*-+         End <=+--
   *  --*----*-+-------+-------+--
   *    |    | |  |    |       |
   * </pre>
   * Path goes by lines (-=) and turns at intersections (+), boxes depicted are adjusted by horizontal/vertical
   * margin and arrow margin, original boxes are smaller (path can't go at original box borders). Algorithm finds
   * the shortest path with minimum amount of turns. In short it's mix of "Lee" and "Dijkstra pathfinding"
   * with turns amount taken into account for distance calculation.
   *
   * The algorithm is not very performant though, it's O(N^2), where N is amount of
   * points in the grid, but since the maximum amount of points in the grid might be up to 34 (not 36 since
   * two box middle points are not permitted) that might be ok for now.
   *
   * @param {Object} lineDef An object containing any of the class configuration option overrides as well
   *                         as `startBox`, `endBox`, `startHorizontalMargin`, `startVerticalMargin`,
   *                         `endHorizontalMargin`, `endVerticalMargin` properties
   * @param {Object} lineDef.startBox An object containing `start`, `end`, `top`, `bottom` properties
   * @param {Object} lineDef.endBox   An object containing `start`, `end`, `top`, `bottom` properties
   * @param {Number} lineDef.startHorizontalMargin Horizontal margin override for start box
   * @param {Number} lineDef.startVerticalMargin   Vertical margin override for start box
   * @param {Number} lineDef.endHorizontalMargin   Horizontal margin override for end box
   * @param {Number} lineDef.endVerticalMargin     Vertical margin override for end box
   *
   *
   * @returns {Object[]|Boolean} Array of line segments or false if path cannot be found
   * @returns {Number} return.x1
   * @returns {Number} return.y1
   * @returns {Number} return.x2
   * @returns {Number} return.y2
   */
  //
  //@ignore
  //@privateparam {Function[]|Function} noPathFallbackFn
  //     A function or array of functions which will be tried in case a path can't be found
  //     Each function will be given a line definition it might try to adjust somehow and return.
  //     The new line definition returned will be tried to find a path.
  //     If a function returns false, then next function will be called if any.
  //
  findPath(lineDef, noPathFallbackFn) {
    const me = this,
      originalLineDef = lineDef;
    let lineDefFull, startBox, endBox, startShift, endShift, startSide, endSide,
      // startArrowSize,
      // endArrowSize,
      startArrowMargin, endArrowMargin, horizontalMargin, verticalMargin, startHorizontalMargin, startVerticalMargin, endHorizontalMargin, endVerticalMargin, otherHorizontalMargin, otherVerticalMargin, otherBoxes, connStartPoint, connEndPoint, pathStartPoint, pathEndPoint, gridStartPoint, gridEndPoint, startGridBox, endGridBox, grid, path, tryNum;
    noPathFallbackFn = ArrayHelper.asArray(noPathFallbackFn);
    for (tryNum = 0; lineDef && !path;) {
      lineDefFull = Object.assign(me.config, lineDef);
      startBox = lineDefFull.startBox;
      endBox = lineDefFull.endBox;
      startShift = lineDefFull.startShift;
      endShift = lineDefFull.endShift;
      startSide = lineDefFull.startSide;
      endSide = lineDefFull.endSide;
      // startArrowSize        = lineDefFull.startArrowSize;
      // endArrowSize          = lineDefFull.endArrowSize;
      startArrowMargin = lineDefFull.startArrowMargin;
      endArrowMargin = lineDefFull.endArrowMargin;
      horizontalMargin = lineDefFull.horizontalMargin;
      verticalMargin = lineDefFull.verticalMargin;
      startHorizontalMargin = lineDefFull.hasOwnProperty('startHorizontalMargin') ? lineDefFull.startHorizontalMargin : horizontalMargin;
      startVerticalMargin = lineDefFull.hasOwnProperty('startVerticalMargin') ? lineDefFull.startVerticalMargin : verticalMargin;
      endHorizontalMargin = lineDefFull.hasOwnProperty('endHorizontalMargin') ? lineDefFull.endHorizontalMargin : horizontalMargin;
      endVerticalMargin = lineDefFull.hasOwnProperty('endVerticalMargin') ? lineDefFull.endVerticalMargin : verticalMargin;
      otherHorizontalMargin = lineDefFull.hasOwnProperty('otherHorizontalMargin') ? lineDefFull.otherHorizontalMargin : horizontalMargin;
      otherVerticalMargin = lineDefFull.hasOwnProperty('otherVerticalMargin') ? lineDefFull.otherVerticalMargin : verticalMargin;
      otherBoxes = lineDefFull.otherBoxes;
      startSide = me.normalizeSide(startSide);
      endSide = me.normalizeSide(endSide);
      connStartPoint = me.getConnectionCoordinatesFromBoxSideShift(startBox, startSide, startShift);
      connEndPoint = me.getConnectionCoordinatesFromBoxSideShift(endBox, endSide, endShift);
      startGridBox = me.calcGridBaseBoxFromBoxAndDrawParams(startBox, startSide /*, startArrowSize*/, startArrowMargin, startHorizontalMargin, startVerticalMargin);
      endGridBox = me.calcGridBaseBoxFromBoxAndDrawParams(endBox, endSide /*, endArrowSize*/, endArrowMargin, endHorizontalMargin, endVerticalMargin);
      // Iterate over points and merge those which are too close to each other (e.g. if difference is less than one
      // over devicePixelRatio we won't even see this effect in GUI)
      // https://github.com/bryntum/support/issues/3923
      BOX_PROPERTIES.forEach(property => {
        // We're talking subpixel precision here, so it doesn't really matter which value we choose
        if (Math.abs(startGridBox[property] - endGridBox[property]) <= THRESHOLD) {
          endGridBox[property] = startGridBox[property];
        }
      });
      if (me.shouldLookForPath(startBox, endBox, startGridBox, endGridBox)) {
        var _otherBoxes;
        otherBoxes = (_otherBoxes = otherBoxes) === null || _otherBoxes === void 0 ? void 0 : _otherBoxes.map(box => me.calcGridBaseBoxFromBoxAndDrawParams(box, false /*, 0*/, 0, otherHorizontalMargin, otherVerticalMargin));
        pathStartPoint = me.getConnectionCoordinatesFromBoxSideShift(startGridBox, startSide, startShift);
        pathEndPoint = me.getConnectionCoordinatesFromBoxSideShift(endGridBox, endSide, endShift);
        grid = me.buildPathGrid(startGridBox, endGridBox, pathStartPoint, pathEndPoint, startSide, endSide, otherBoxes);
        gridStartPoint = me.convertDecartPointToGridPoint(grid, pathStartPoint);
        gridEndPoint = me.convertDecartPointToGridPoint(grid, pathEndPoint);
        path = me.findPathOnGrid(grid, gridStartPoint, gridEndPoint, startSide, endSide);
      }
      // Loop if
      // - path is still not found
      // - have no next line definition (which should be obtained from call to one of the functions from noPathFallbackFn array
      // - have noPathFallBackFn array
      // - current try number is less then noPathFallBackFn array length
      for (lineDef = false; !path && !lineDef && noPathFallbackFn && tryNum < noPathFallbackFn.length; tryNum++) {
        lineDef = noPathFallbackFn[tryNum](lineDefFull, originalLineDef);
      }
    }
    if (path) {
      path = me.prependPathWithArrowStaffSegment(path, connStartPoint /*, startArrowSize*/, startSide);
      path = me.appendPathWithArrowStaffSegment(path, connEndPoint /*, endArrowSize*/, endSide);
      path = me.optimizePath(path);
    }
    return path;
  }
  // Compares boxes relative position in the given direction.
  //  0 - 1 is to the left/top of 2
  //  1 - 1 overlaps with left/top edge of 2
  //  2 - 1 is inside 2
  // -2 - 2 is inside 1
  //  3 - 1 overlaps with right/bottom edge of 2
  //  4 - 1 is to the right/bottom of 2
  static calculateRelativePosition(box1, box2, vertical = false) {
    const startProp = vertical ? 'top' : 'start',
      endProp = vertical ? 'bottom' : 'end';
    let result;
    if (box1[endProp] < box2[startProp]) {
      result = 0;
    } else if (box1[endProp] <= box2[endProp] && box1[endProp] >= box2[startProp] && box1[startProp] < box2[startProp]) {
      result = 1;
    } else if (box1[startProp] >= box2[startProp] && box1[endProp] <= box2[endProp]) {
      result = 2;
    } else if (box1[startProp] < box2[startProp] && box1[endProp] > box2[endProp]) {
      result = -2;
    } else if (box1[startProp] <= box2[endProp] && box1[endProp] > box2[endProp]) {
      result = 3;
    } else {
      result = 4;
    }
    return result;
  }
  // Checks if relative position of the original and marginized boxes is the same
  static boxOverlapChanged(startBox, endBox, gridStartBox, gridEndBox, vertical = false) {
    const calculateOverlap = RectangularPathFinder.calculateRelativePosition,
      originalOverlap = calculateOverlap(startBox, endBox, vertical),
      finalOverlap = calculateOverlap(gridStartBox, gridEndBox, vertical);
    return originalOverlap !== finalOverlap;
  }
  shouldLookForPath(startBox, endBox, gridStartBox, gridEndBox) {
    let result = true;
    // Only calculate overlap if boxes are narrow in horizontal direction
    if (
    // We refer to the original arrow margins because during lookup those might be nullified and we need some
    // criteria to tell if events are too narrow
    (startBox.end - startBox.start <= this.startArrowMargin || endBox.end - endBox.start <= this.endArrowMargin) && Math.abs(RectangularPathFinder.calculateRelativePosition(startBox, endBox, true)) === 2) {
      result = !RectangularPathFinder.boxOverlapChanged(startBox, endBox, gridStartBox, gridEndBox);
    }
    return result;
  }
  getConnectionCoordinatesFromBoxSideShift(box, side, shift) {
    let coords;
    // Note that we deal with screen geometry here, not logical dependency sides
    // Possible 'start' and 'end' have been resolved to box sides.
    switch (side) {
      case 'left':
        coords = {
          x: box.start,
          y: (box.top + box.bottom) / 2 + shift
        };
        break;
      case 'right':
        coords = {
          x: box.end,
          y: (box.top + box.bottom) / 2 + shift
        };
        break;
      case 'top':
        coords = {
          x: (box.start + box.end) / 2 + shift,
          y: box.top
        };
        break;
      case 'bottom':
        coords = {
          x: (box.start + box.end) / 2 + shift,
          y: box.bottom
        };
        break;
    }
    return coords;
  }
  calcGridBaseBoxFromBoxAndDrawParams(box, side /*, arrowSize*/, arrowMargin, horizontalMargin, verticalMargin) {
    let gridBox;
    switch (this.normalizeSide(side)) {
      case 'left':
        gridBox = {
          start: box.start - Math.max( /*arrowSize + */arrowMargin, horizontalMargin),
          end: box.end + horizontalMargin,
          top: box.top - verticalMargin,
          bottom: box.bottom + verticalMargin
        };
        break;
      case 'right':
        gridBox = {
          start: box.start - horizontalMargin,
          end: box.end + Math.max( /*arrowSize + */arrowMargin, horizontalMargin),
          top: box.top - verticalMargin,
          bottom: box.bottom + verticalMargin
        };
        break;
      case 'top':
        gridBox = {
          start: box.start - horizontalMargin,
          end: box.end + horizontalMargin,
          top: box.top - Math.max( /*arrowSize + */arrowMargin, verticalMargin),
          bottom: box.bottom + verticalMargin
        };
        break;
      case 'bottom':
        gridBox = {
          start: box.start - horizontalMargin,
          end: box.end + horizontalMargin,
          top: box.top - verticalMargin,
          bottom: box.bottom + Math.max( /*arrowSize + */arrowMargin, verticalMargin)
        };
        break;
      default:
        gridBox = {
          start: box.start - horizontalMargin,
          end: box.end + horizontalMargin,
          top: box.top - verticalMargin,
          bottom: box.bottom + verticalMargin
        };
    }
    return gridBox;
  }
  normalizeSide(side) {
    const {
      rtl
    } = this.client;
    if (side === 'start') {
      return rtl ? 'right' : 'left';
    }
    if (side === 'end') {
      return rtl ? 'left' : 'right';
    }
    return side;
  }
  buildPathGrid(startGridBox, endGridBox, pathStartPoint, pathEndPoint, startSide, endSide, otherGridBoxes) {
    let xs, ys, y, x, ix, iy, xslen, yslen, ib, blen, box, permitted, point;
    const points = {},
      linearPoints = [];
    xs = [startGridBox.start, startSide === 'left' || startSide === 'right' ? (startGridBox.start + startGridBox.end) / 2 : pathStartPoint.x, startGridBox.end, endGridBox.start, endSide === 'left' || endSide === 'right' ? (endGridBox.start + endGridBox.end) / 2 : pathEndPoint.x, endGridBox.end];
    ys = [startGridBox.top, startSide === 'top' || startSide === 'bottom' ? (startGridBox.top + startGridBox.bottom) / 2 : pathStartPoint.y, startGridBox.bottom, endGridBox.top, endSide === 'top' || endSide === 'bottom' ? (endGridBox.top + endGridBox.bottom) / 2 : pathEndPoint.y, endGridBox.bottom];
    if (otherGridBoxes) {
      otherGridBoxes.forEach(box => {
        xs.push(box.start, (box.start + box.end) / 2, box.end);
        ys.push(box.top, (box.top + box.bottom) / 2, box.bottom);
      });
    }
    xs = [...new Set(xs.sort((a, b) => a - b))];
    ys = [...new Set(ys.sort((a, b) => a - b))];
    for (iy = 0, yslen = ys.length; iy < yslen; ++iy) {
      points[iy] = points[iy] || {};
      y = ys[iy];
      for (ix = 0, xslen = xs.length; ix < xslen; ++ix) {
        x = xs[ix];
        permitted = (x <= startGridBox.start || x >= startGridBox.end || y <= startGridBox.top || y >= startGridBox.bottom) && (x <= endGridBox.start || x >= endGridBox.end || y <= endGridBox.top || y >= endGridBox.bottom);
        if (otherGridBoxes) {
          for (ib = 0, blen = otherGridBoxes.length; permitted && ib < blen; ++ib) {
            box = otherGridBoxes[ib];
            permitted = x <= box.start || x >= box.end || y <= box.top || y >= box.bottom ||
            // Allow point if it is a path start/end even if point is inside any box
            x === pathStartPoint.x && y === pathStartPoint.y || x === pathEndPoint.x && y === pathEndPoint.y;
          }
        }
        point = {
          distance: Number.MAX_SAFE_INTEGER,
          permitted,
          x,
          y,
          ix,
          iy
        };
        points[iy][ix] = point;
        linearPoints.push(point);
      }
    }
    return {
      width: xs.length,
      height: ys.length,
      xs,
      ys,
      points,
      linearPoints
    };
  }
  convertDecartPointToGridPoint(grid, point) {
    const x = grid.xs.indexOf(point.x),
      y = grid.ys.indexOf(point.y);
    return grid.points[y][x];
  }
  findPathOnGrid(grid, gridStartPoint, gridEndPoint, startSide, endSide) {
    const me = this;
    let path = false;
    if (gridStartPoint.permitted && gridEndPoint.permitted) {
      grid = me.waveForward(grid, gridStartPoint, 0);
      path = me.collectPath(grid, gridEndPoint, endSide);
    }
    return path;
  }
  // Returns neighbors from Von Neiman ambit (see Lee pathfinding algorithm description)
  getGridPointNeighbors(grid, gridPoint, predicateFn) {
    const ix = gridPoint.ix,
      iy = gridPoint.iy,
      result = [];
    let neighbor;
    // NOTE:
    // It's important to push bottom neighbors first since this method is used
    // in collectPath(), which recursively collects path from end to start node
    // and if bottom neighbors are pushed first in result array then collectPath()
    // will produce a line which is more suitable (pleasant looking) for our purposes.
    if (iy < grid.height - 1) {
      neighbor = grid.points[iy + 1][ix];
      (!predicateFn || predicateFn(neighbor)) && result.push(neighbor);
    }
    if (iy > 0) {
      neighbor = grid.points[iy - 1][ix];
      (!predicateFn || predicateFn(neighbor)) && result.push(neighbor);
    }
    if (ix < grid.width - 1) {
      neighbor = grid.points[iy][ix + 1];
      (!predicateFn || predicateFn(neighbor)) && result.push(neighbor);
    }
    if (ix > 0) {
      neighbor = grid.points[iy][ix - 1];
      (!predicateFn || predicateFn(neighbor)) && result.push(neighbor);
    }
    return result;
  }
  waveForward(grid, gridStartPoint, distance) {
    const me = this;
    // I use the WalkHelper here because a point on a grid and it's neighbors might be considered as a hierarchy.
    // The point is the parent node, and it's neighbors are the children nodes. Thus the grid here is hierarchical
    // data structure which can be walked. WalkHelper walks non-recursively which is exactly what I need as well.
    WalkHelper.preWalkUnordered(
    // Walk starting point - a node is a grid point and it's distance from the starting point
    [gridStartPoint, distance],
    // Children query function
    // NOTE: It's important to fix neighbor distance first, before waving to a neighbor, otherwise waving might
    //       get through a neighbor point setting it's distance to a value more than (distance + 1) whereas we,
    //       at the children querying moment in time, already know that the possibly optimal distance is (distance + 1)
    ([point, distance]) => me.getGridPointNeighbors(grid, point, neighborPoint => neighborPoint.permitted && neighborPoint.distance > distance + 1).map(neighborPoint => [neighborPoint, distance + 1] // Neighbor distance fixation
    ),
    // Walk step iterator function
    ([point, distance]) => point.distance = distance // Neighbor distance applying
    );

    return grid;
  }
  collectPath(grid, gridEndPoint, endSide) {
    const me = this,
      path = [];
    let pathFound = true,
      neighbors,
      lowestDistanceNeighbor,
      xDiff,
      yDiff;
    while (pathFound && gridEndPoint.distance) {
      neighbors = me.getGridPointNeighbors(grid, gridEndPoint, point => point.permitted && point.distance === gridEndPoint.distance - 1);
      pathFound = neighbors.length > 0;
      if (pathFound) {
        // Prefer turnless neighbors first
        neighbors = neighbors.sort((a, b) => {
          let xDiff, yDiff;
          xDiff = a.ix - gridEndPoint.ix;
          yDiff = a.iy - gridEndPoint.iy;
          const resultA = (endSide === 'left' || endSide === 'right') && yDiff === 0 || (endSide === 'top' || endSide === 'bottom') && xDiff === 0 ? -1 : 1;
          xDiff = b.ix - gridEndPoint.ix;
          yDiff = b.iy - gridEndPoint.iy;
          const resultB = (endSide === 'left' || endSide === 'right') && yDiff === 0 || (endSide === 'top' || endSide === 'bottom') && xDiff === 0 ? -1 : 1;
          if (resultA > resultB) return 1;
          if (resultA < resultB) return -1;
          // apply additional sorting to be sure to pick bottom path in IE
          if (resultA === resultB) return a.y > b.y ? -1 : 1;
        });
        lowestDistanceNeighbor = neighbors[0];
        path.push({
          x1: lowestDistanceNeighbor.x,
          y1: lowestDistanceNeighbor.y,
          x2: gridEndPoint.x,
          y2: gridEndPoint.y
        });
        // Detecting new side, either xDiff or yDiff must be 0 (but not both)
        xDiff = lowestDistanceNeighbor.ix - gridEndPoint.ix;
        yDiff = lowestDistanceNeighbor.iy - gridEndPoint.iy;
        switch (true) {
          case !yDiff && xDiff > 0:
            endSide = 'left';
            break;
          case !yDiff && xDiff < 0:
            endSide = 'right';
            break;
          case !xDiff && yDiff > 0:
            endSide = 'top';
            break;
          case !xDiff && yDiff < 0:
            endSide = 'bottom';
            break;
        }
        gridEndPoint = lowestDistanceNeighbor;
      }
    }
    return pathFound && path.reverse() || false;
  }
  prependPathWithArrowStaffSegment(path, connStartPoint /*, startArrowSize*/, startSide) {
    if (path.length > 0) {
      const firstSegment = path[0],
        prependSegment = {
          x2: firstSegment.x1,
          y2: firstSegment.y1
        };
      switch (startSide) {
        case 'left':
          prependSegment.x1 = connStartPoint.x /* - startArrowSize*/;
          prependSegment.y1 = firstSegment.y1;
          break;
        case 'right':
          prependSegment.x1 = connStartPoint.x /* + startArrowSize*/;
          prependSegment.y1 = firstSegment.y1;
          break;
        case 'top':
          prependSegment.x1 = firstSegment.x1;
          prependSegment.y1 = connStartPoint.y /* - startArrowSize*/;
          break;
        case 'bottom':
          prependSegment.x1 = firstSegment.x1;
          prependSegment.y1 = connStartPoint.y /* + startArrowSize*/;
          break;
      }
      path.unshift(prependSegment);
    }
    return path;
  }
  appendPathWithArrowStaffSegment(path, connEndPoint /*, endArrowSize*/, endSide) {
    if (path.length > 0) {
      const lastSegment = path[path.length - 1],
        appendSegment = {
          x1: lastSegment.x2,
          y1: lastSegment.y2
        };
      switch (endSide) {
        case 'left':
          appendSegment.x2 = connEndPoint.x /* - endArrowSize*/;
          appendSegment.y2 = lastSegment.y2;
          break;
        case 'right':
          appendSegment.x2 = connEndPoint.x /* + endArrowSize*/;
          appendSegment.y2 = lastSegment.y2;
          break;
        case 'top':
          appendSegment.x2 = lastSegment.x2;
          appendSegment.y2 = connEndPoint.y /* - endArrowSize*/;
          break;
        case 'bottom':
          appendSegment.x2 = lastSegment.x2;
          appendSegment.y2 = connEndPoint.y /* + endArrowSize*/;
          break;
      }
      path.push(appendSegment);
    }
    return path;
  }
  optimizePath(path) {
    const optPath = [];
    let prevSegment, curSegment;
    if (path.length > 0) {
      prevSegment = path.shift();
      optPath.push(prevSegment);
      while (path.length > 0) {
        curSegment = path.shift();
        // both segments are as good as equal
        if (equalEnough(prevSegment.x1, curSegment.x1) && equalEnough(prevSegment.y1, curSegment.y1) && equalEnough(prevSegment.x2, curSegment.x2) && equalEnough(prevSegment.y2, curSegment.y2)) {
          prevSegment = curSegment;
        }
        // both segments are horizontal or very nearly so
        else if (equalEnough(prevSegment.y1, prevSegment.y2) && equalEnough(curSegment.y1, curSegment.y2)) {
          prevSegment.x2 = curSegment.x2;
        }
        // both segments are vertical or very nearly so
        else if (equalEnough(prevSegment.x1, prevSegment.x2) && equalEnough(curSegment.x1, curSegment.x2)) {
          prevSegment.y2 = curSegment.y2;
        }
        // segments have different orientation (path turn)
        else {
          optPath.push(curSegment);
          prevSegment = curSegment;
        }
      }
    }
    return optPath;
  }
}
RectangularPathFinder._$name = 'RectangularPathFinder';

// Determine a line segments drawing direction
function drawingDirection(pointSet) {
  if (pointSet.x1 === pointSet.x2) {
    return pointSet.y2 > pointSet.y1 ? 'd' : 'u';
  }
  return pointSet.x2 > pointSet.x1 ? 'r' : 'l';
}
// Determine a line segments length
function segmentLength(pointSet) {
  return pointSet.x1 === pointSet.x2 ? pointSet.y2 - pointSet.y1 : pointSet.x2 - pointSet.x1;
}
// Define an arc to tie two line segments together
function arc(pointSet, nextPointSet, radius) {
  const corner = drawingDirection(pointSet) + drawingDirection(nextPointSet),
    // Flip x if this or next segment is drawn right to left
    rx = radius * (corner.includes('l') ? -1 : 1),
    // Flip y if this or next segment is drawn bottom to top
    ry = radius * (corner.includes('u') ? -1 : 1),
    // Positive (0) or negative (1) angle
    sweep = corner === 'ur' || corner === 'lu' || corner === 'dl' || corner === 'rd' ? 1 : 0;
  return `a${rx},${ry} 0 0 ${sweep} ${rx},${ry}`;
}
// Define a line for a set of points, tying it together with the next set with an arc when applicable
function line(pointSet, nextPointSet, location, radius, prevRadius) {
  // Horizontal or vertical line
  let line = pointSet.x1 === pointSet.x2 ? 'v' : 'h',
    useRadius = radius;
  // Add an arc?
  if (radius) {
    const
      // Length of this line segment
      length = segmentLength(pointSet),
      // Length of the next one. Both are needed to determine max radius (half of the shortest delta)
      nextLength = nextPointSet ? Math.abs(segmentLength(nextPointSet)) : Number.MAX_SAFE_INTEGER,
      // Line direction
      sign = Math.sign(length);
    // If we are not passed a radius from the previous line drawn, we use the configured radius. It is used to shorten
    // this lines length to fit the arc that connects it to the previous line
    if (prevRadius == null) {
      prevRadius = radius;
    }
    // We cannot use a radius larger than half our or our successor's length, doing so would make the segment too long
    // when the arc is created
    if (Math.abs(length) < radius * 2 || nextLength < radius * 2) {
      useRadius = Math.min(Math.abs(length), nextLength) / 2;
    }
    const
      // Radius of neighbouring arcs, subtracted from length below...
      subtract = location === 'single' ? 0 : location === 'first' ? useRadius : location === 'between' ? prevRadius + useRadius : /*last*/prevRadius,
      // ...to produce the length of the line segment to draw
      useLength = length - subtract * sign;
    // Apply line segment length, unless it passed over 0 in which case we stick to 0
    line += Math.sign(useLength) !== sign ? 0 : useLength;
    // Add an arc if applicable
    if (location !== 'last' && location !== 'single' && useRadius > 0) {
      line += ` ${arc(pointSet, nextPointSet, useRadius)}`;
    }
  }
  // Otherwise take a shorter code path
  else {
    line += segmentLength(pointSet);
  }
  return {
    line,
    currentRadius: radius !== useRadius ? useRadius : null
  };
}
// Define an SVG path base on points from the path finder.
// Each segment in the path can be joined by an arc
function pathMapper(radius, points) {
  const {
    length
  } = points;
  if (!length) {
    return '';
  }
  let currentRadius = null;
  return `M${points[0].x1},${points[0].y1} ${points.map((pointSet, i) => {
    // Segment placement among all segments, used to determine if an arc should be added
    const location = length === 1 ? 'single' : i === length - 1 ? 'last' : i === 0 ? 'first' : 'between',
      lineSpec = line(pointSet, points[i + 1], location, radius, currentRadius);
    ({
      currentRadius
    } = lineSpec);
    return lineSpec.line;
  }).join(' ')}`;
}
// Mixin that holds the code needed to generate DomConfigs for dependency lines
var DependencyLineGenerator = (Target => class DependencyLineGenerator extends Target {
  static $name = 'DependencyLineGenerator';
  lineCache = {};
  onSVGReady() {
    const me = this;
    me.pathFinder = new RectangularPathFinder({
      ...me.pathFinderConfig,
      client: me.client
    });
    me.lineDefAdjusters = me.createLineDefAdjusters();
    me.createMarker();
  }
  changeRadius(radius) {
    if (radius !== null) {
      ObjectHelper.assertNumber(radius, 'radius');
    }
    return radius;
  }
  updateRadius() {
    if (!this.isConfiguring) {
      this.reset();
    }
  }
  updateRenderer() {
    if (!this.isConfiguring) {
      this.reset();
    }
  }
  changeClickWidth(width) {
    if (width !== null) {
      ObjectHelper.assertNumber(width, 'clickWidth');
    }
    return width;
  }
  updateClickWidth() {
    if (!this.isConfiguring) {
      this.reset();
    }
  }
  //region Marker
  createMarker() {
    var _me$marker;
    const me = this,
      {
        markerDef
      } = me,
      svg = this.client.svgCanvas,
      // SVG markers has to use an id, we want the id to be per scheduler when using multiple
      markerId = markerDef ? `${me.client.id}-arrowEnd` : 'arrowEnd';
    (_me$marker = me.marker) === null || _me$marker === void 0 ? void 0 : _me$marker.remove();
    svg.style.setProperty('--scheduler-dependency-marker', `url(#${markerId})`);
    me.marker = DomHelper.createElement({
      parent: svg,
      id: markerId,
      tag: 'marker',
      className: 'b-sch-dependency-arrow',
      ns: 'http://www.w3.org/2000/svg',
      markerHeight: 11,
      markerWidth: 11,
      refX: 8.5,
      refY: 3,
      viewBox: '0 0 9 6',
      orient: 'auto-start-reverse',
      markerUnits: 'userSpaceOnUse',
      retainElement: true,
      children: [{
        tag: 'path',
        ns: 'http://www.w3.org/2000/svg',
        d: me.markerDef ?? 'M3,0 L3,6 L9,3 z'
      }]
    });
  }
  updateMarkerDef() {
    if (!this.isConfiguring) {
      this.createMarker();
    }
  }
  //endregion
  //region DomConfig
  getAssignmentElement(assignment) {
    var _this$client$features, _this$client$features2;
    // If we are dragging an event, we need to use the proxy element
    // (which is not the original element if we are not constrained to timeline)
    const proxyElement = (_this$client$features = this.client.features.eventDrag) === null || _this$client$features === void 0 ? void 0 : (_this$client$features2 = _this$client$features.getProxyElement) === null || _this$client$features2 === void 0 ? void 0 : _this$client$features2.call(_this$client$features, assignment);
    return proxyElement || this.client.getElementFromAssignmentRecord(assignment);
  }
  // Generate a DomConfig for a dependency line between two assignments (tasks in Gantt)
  getDomConfigs(dependency, fromAssignment, toAssignment, forceBoxes) {
    const me = this,
      key = me.getDependencyKey(dependency, fromAssignment, toAssignment),
      // Under certain circumstances (scrolling) we might be able to reuse the previous DomConfig.
      cached = me.lineCache[key];
    // Create line def if not cached, or we are live drawing and have event elements (dragging, transitioning etc)
    if (me.constructLineCache || !cached || forceBoxes || me.drawingLive && (me.getAssignmentElement(fromAssignment) || me.getAssignmentElement(toAssignment))) {
      const lineDef = me.prepareLineDef(dependency, fromAssignment, toAssignment, forceBoxes),
        points = lineDef && me.pathFinder.findPath(lineDef, me.lineDefAdjusters),
        {
          client,
          clickWidth
        } = me,
        {
          toEvent
        } = dependency;
      if (points) {
        var _me$renderer;
        const highlighted = me.highlighted.get(dependency),
          domConfig = {
            tag: 'path',
            ns: 'http://www.w3.org/2000/svg',
            d: pathMapper(me.radius ?? 0, points),
            role: 'presentation',
            dataset: {
              syncId: key,
              depId: dependency.id,
              fromId: fromAssignment.id,
              toId: toAssignment.id
            },
            elementData: {
              dependency,
              points
            },
            class: {
              [me.baseCls]: 1,
              [dependency.cls]: dependency.cls,
              // Data highlight
              [dependency.highlighted]: dependency.highlighted,
              // Feature highlight
              [highlighted && [...highlighted].join(' ')]: highlighted,
              [me.noMarkerCls]: lineDef.hideMarker,
              'b-inactive': dependency.active === false,
              'b-sch-bidirectional-line': dependency.bidirectional,
              'b-readonly': dependency.readOnly,
              // If target event is outside the view add special CSS class to hide marker (arrow)
              'b-sch-dependency-ends-outside': !toEvent.milestone && (toEvent.endDate <= client.startDate || client.endDate <= toEvent.startDate) || toEvent.milestone && (toEvent.endDate < client.startDate || client.endDate < toEvent.startDate)
            }
          };
        (_me$renderer = me.renderer) === null || _me$renderer === void 0 ? void 0 : _me$renderer.call(me, {
          domConfig,
          points,
          dependencyRecord: dependency,
          fromAssignmentRecord: fromAssignment,
          toAssignmentRecord: toAssignment,
          fromBox: lineDef.startBox,
          toBox: lineDef.endBox,
          fromSide: lineDef.startSide,
          toSide: lineDef.endSide
        });
        const configs = [domConfig];
        if (clickWidth > 1) {
          configs.push({
            ...domConfig,
            // Shallow on purpose, to not waste perf cloning deeply
            class: {
              ...domConfig.class,
              'b-click-area': 1
            },
            dataset: {
              ...domConfig.dataset,
              syncId: `${domConfig.dataset.syncId}-click-area`
            },
            style: {
              strokeWidth: clickWidth
            }
          });
        }
        return me.lineCache[key] = configs;
      }
      // Nothing to draw or cache
      return me.lineCache[key] = null;
    }
    return cached;
  }
  //endregion
  //region Bounds
  // Generates `otherBoxes` config for rectangular path finder, which push dependency line to the row boundary.
  // It should be enough to return single box with top/bottom taken from row top/bottom and left/right taken from source
  // box, extended by start arrow margin to both sides.
  generateBoundaryBoxes(box, side) {
    // We need two boxes for the bottom edge, because otherwise path cannot be found. Ideally that shouldn't be
    // necessary. Other solution would be to adjust bottom by -1px, but that would make some dependency lines to take
    // 1px different path on a row boundary, which doesn't look nice (but slightly more performant)
    if (side === 'bottom') {
      return [{
        start: box.left,
        end: box.left + box.width / 2,
        top: box.rowTop,
        bottom: box.rowBottom
      }, {
        start: box.left + box.width / 2,
        end: box.right,
        top: box.rowTop,
        bottom: box.rowBottom
      }];
    } else {
      return [{
        start: box.left - this.pathFinder.startArrowMargin,
        end: box.right + this.pathFinder.startArrowMargin,
        top: box.rowTop,
        bottom: box.rowBottom
      }];
    }
  }
  // Bounding box for an assignment, uses elements bounds if rendered
  getAssignmentBounds(assignment) {
    const {
        client
      } = this,
      element = this.getAssignmentElement(assignment);
    if (element && !client.isExporting) {
      const rectangle = Rectangle.from(element, this.relativeTo);
      if (client.isHorizontal) {
        let row = client.getRowById(assignment.resource.id);
        // Outside of its row? It is being dragged, resolve new row
        if (rectangle.y < row.top || rectangle.bottom > row.bottom) {
          const overRow = client.rowManager.getRowAt(rectangle.center.y, true);
          if (overRow) {
            row = overRow;
          }
        }
        rectangle.rowTop = row.top;
        rectangle.rowBottom = row.bottom;
      }
      return rectangle;
    }
    return client.isEngineReady && client.getAssignmentEventBox(assignment, true);
  }
  //endregion
  //region Sides
  getConnectorStartSide(timeSpanRecord) {
    return this.client.currentOrientation.getConnectorStartSide(timeSpanRecord);
  }
  getConnectorEndSide(timeSpanRecord) {
    return this.client.currentOrientation.getConnectorEndSide(timeSpanRecord);
  }
  getDependencyStartSide(dependency) {
    const {
      fromEvent,
      type,
      fromSide
    } = dependency;
    if (fromSide) {
      return fromSide;
    }
    switch (true) {
      case type === DependencyModel.Type.StartToEnd:
      case type === DependencyModel.Type.StartToStart:
        return this.getConnectorStartSide(fromEvent);
      case type === DependencyModel.Type.EndToStart:
      case type === DependencyModel.Type.EndToEnd:
        return this.getConnectorEndSide(fromEvent);
      default:
        // Default value might not be applied yet when rendering early in Pro / Gantt
        return this.getConnectorEndSide(fromEvent);
    }
  }
  getDependencyEndSide(dependency) {
    const {
      toEvent,
      type,
      toSide
    } = dependency;
    if (toSide) {
      return toSide;
    }
    // Fallback to view trait if dependency end side is not given /*or can be obtained from type*/
    switch (true) {
      case type === DependencyModel.Type.EndToEnd:
      case type === DependencyModel.Type.StartToEnd:
        return this.getConnectorEndSide(toEvent);
      case type === DependencyModel.Type.EndToStart:
      case type === DependencyModel.Type.StartToStart:
        return this.getConnectorStartSide(toEvent);
      default:
        // Default value might not be applied yet when rendering early in Pro / Gantt
        return this.getConnectorStartSide(toEvent);
    }
  }
  //endregion
  //region Line def
  // An array of functions used to alter path config when no path found.
  // It first tries to shrink arrow margins and secondly hides arrows entirely
  createLineDefAdjusters() {
    const {
      client
    } = this;
    function shrinkArrowMargins(lineDef) {
      const {
        barMargin
      } = client;
      let adjusted = false;
      if (lineDef.startArrowMargin > barMargin || lineDef.endArrowMargin > barMargin) {
        lineDef.startArrowMargin = lineDef.endArrowMargin = barMargin;
        adjusted = true;
      }
      return adjusted ? lineDef : adjusted;
    }
    function resetArrowMargins(lineDef) {
      let adjusted = false;
      if (lineDef.startArrowMargin > 0 || lineDef.endArrowMargin > 0) {
        lineDef.startArrowMargin = lineDef.endArrowMargin = 0;
        adjusted = true;
      }
      return adjusted ? lineDef : adjusted;
    }
    function shrinkHorizontalMargin(lineDef, originalLineDef) {
      let adjusted = false;
      if (lineDef.horizontalMargin > 2) {
        lineDef.horizontalMargin = 1;
        adjusted = true;
        originalLineDef.hideMarker = true;
      }
      return adjusted ? lineDef : adjusted;
    }
    return [shrinkArrowMargins, resetArrowMargins, shrinkHorizontalMargin];
  }
  // Overridden in Gantt
  adjustLineDef(dependency, lineDef) {
    return lineDef;
  }
  // Prepare data to feed to the path finder
  prepareLineDef(dependency, fromAssignment, toAssignment, forceBoxes) {
    const me = this,
      startSide = me.getDependencyStartSide(dependency),
      endSide = me.getDependencyEndSide(dependency),
      startRectangle = (forceBoxes === null || forceBoxes === void 0 ? void 0 : forceBoxes.from) ?? me.getAssignmentBounds(fromAssignment),
      endRectangle = (forceBoxes === null || forceBoxes === void 0 ? void 0 : forceBoxes.to) ?? me.getAssignmentBounds(toAssignment),
      otherBoxes = [];
    if (!startRectangle || !endRectangle) {
      return null;
    }
    let {
      startArrowMargin,
      verticalMargin
    } = me.pathFinder;
    if (me.client.isHorizontal) {
      // Only add otherBoxes if assignments are in different resources
      if (startRectangle.rowTop != null && startRectangle.rowTop !== endRectangle.rowTop) {
        otherBoxes.push(...me.generateBoundaryBoxes(startRectangle, startSide));
      }
      // Do not change start arrow margin in case dependency is bidirectional
      if (!dependency.bidirectional) {
        if (/(top|bottom)/.test(startSide)) {
          startArrowMargin = me.client.barMargin / 2;
        }
        verticalMargin = me.client.barMargin / 2;
      }
    }
    return me.adjustLineDef(dependency, {
      startBox: startRectangle,
      endBox: endRectangle,
      otherBoxes,
      startArrowMargin,
      verticalMargin,
      otherVerticalMargin: 0,
      otherHorizontalMargin: 0,
      startSide,
      endSide
    });
  }
  //endregion
  //region Cache
  // All dependencies are about to be drawn, check if we need to build the line cache
  beforeDraw() {
    super.beforeDraw();
    if (!Object.keys(this.lineCache).length) {
      this.constructLineCache = true;
    }
  }
  // All dependencies are drawn, we no longer need to rebuild the cache
  afterDraw() {
    super.afterDraw();
    this.constructLineCache = false;
  }
  reset() {
    super.reset();
    this.lineCache = {};
  }
  //endregion
});

/**
 * @module Scheduler/feature/mixin/DependencyTooltip
 */
const
  // Map dependency type to side of a box, for displaying an icon in the tooltip
  fromBoxSide = ['start', 'start', 'end', 'end'],
  toBoxSide = ['start', 'end', 'start', 'end'];
/**
 * Mixin that adds tooltip support to the {@link Scheduler/feature/Dependencies} feature.
 * @mixin
 */
var DependencyTooltip = (Target => class DependencyTooltip extends Target {
  static $name = 'DependencyTooltip';
  static configurable = {
    /**
     * Set to true to show a tooltip when hovering a dependency line
     * @config {Boolean}
     */
    showTooltip: true,
    /**
     * A template function allowing you to configure the contents of the tooltip shown when hovering a
     * dependency line. You can return either an HTML string or a {@link DomConfig} object.
     * @prp {Function} tooltipTemplate
     * @param {Scheduler.model.DependencyBaseModel} dependency The dependency record
     * @returns {String|DomConfig}
     */
    tooltipTemplate(dependency) {
      return {
        children: [{
          className: 'b-sch-dependency-tooltip',
          children: [{
            tag: 'label',
            text: this.L('L{Dependencies.from}')
          }, {
            text: dependency.fromEvent.name
          }, {
            className: `b-sch-box b-${dependency.fromSide || fromBoxSide[dependency.type]}`
          }, {
            tag: 'label',
            text: this.L('L{Dependencies.to}')
          }, {
            text: dependency.toEvent.name
          }, {
            className: `b-sch-box b-${dependency.toSide || toBoxSide[dependency.type]}`
          }]
        }]
      };
    },
    /**
     * A tooltip config object that will be applied to the dependency hover tooltip. Can be used to for example
     * customize delay
     * @config {TooltipConfig}
     */
    tooltip: {
      $config: 'nullify',
      value: {}
    }
  };
  changeTooltip(tooltip, old) {
    const me = this;
    old === null || old === void 0 ? void 0 : old.destroy();
    if (!me.showTooltip || !tooltip) {
      return null;
    }
    return Tooltip.new({
      align: 'b-t',
      id: `${me.client.id}-dependency-tip`,
      forSelector: `.b-timelinebase:not(.b-eventeditor-editing,.b-taskeditor-editing,.b-resizing-event,.b-dragcreating,.b-dragging-event,.b-creating-dependency) .${me.baseCls}`,
      forElement: me.client.timeAxisSubGridElement,
      showOnHover: true,
      hoverDelay: 0,
      hideDelay: 0,
      anchorToTarget: false,
      textContent: false,
      // Skip max-width setting
      trackMouse: false,
      getHtml: me.getHoverTipHtml.bind(me)
    }, tooltip);
  }
  /**
   * Generates DomConfig content for the tooltip shown when hovering a dependency
   * @param {Object} tooltipConfig
   * @returns {DomConfig} DomConfig used as tooltips content
   * @private
   */
  getHoverTipHtml({
    activeTarget
  }) {
    return this.tooltipTemplate(this.resolveDependencyRecord(activeTarget));
  }
});

const eventNameMap$1 = {
  click: 'Click',
  dblclick: 'DblClick',
  contextmenu: 'ContextMenu'
};
/**
 * @module Scheduler/feature/Dependencies
 */
const collectLinkedAssignments = assignment => {
  var _assignment$resource;
  const result = [assignment];
  if ((_assignment$resource = assignment.resource) !== null && _assignment$resource !== void 0 && _assignment$resource.hasLinks) {
    // Fake linked assignments
    result.push(...assignment.resource.$links.map(l => ({
      id: `${l.id}_${assignment.id}`,
      resource: l,
      event: assignment.event,
      drawDependencies: assignment.drawDependencies
    })));
  }
  return result;
};
/**
 * Feature that draws dependencies between events. Uses a {@link Scheduler.data.DependencyStore} to determine which
 * dependencies to draw, if none is defined one will be created automatically. Dependencies can also be specified as
 * `scheduler.dependencies`, see example below:
 *
 * {@inlineexample Scheduler/feature/Dependencies.js}
 *
 * Dependencies also work in vertical mode:
 *
 * {@inlineexample Scheduler/feature/DependenciesVertical.js}
 *
 * To customize the dependency tooltip, you can provide the {@link #config-tooltip} config and specify a
 * {@link Core.widget.Tooltip#config-getHtml} function. For example:
 *
 * ```javascript
 * const scheduler = new Scheduler({
 *     features : {
 *         dependencies : {
 *             tooltip : {
 *                 getHtml({ activeTarget }) {
 *                     const dependencyModel = scheduler.resolveDependencyRecord(activeTarget);
 *
 *                     if (!dependencyModel) return null;
 *
 *                     const { fromEvent, toEvent } = dependencyModel;
 *
 *                     return `${fromEvent.name} (${fromEvent.id}) -> ${toEvent.name} (${toEvent.id})`;
 *                 }
 *             }
 *         }
 *     }
 * }
 * ```
 *
 * ## Styling dependency lines
 *
 * You can easily customize the arrows drawn between events. To change all arrows, apply the following basic SVG CSS:
 *
 * ```css
 * .b-sch-dependency {
 *    stroke-width: 2;
 *    stroke : red;
 * }
 *
 * .b-sch-dependency-arrow {
 *     fill: red;
 * }
 * ```
 *
 * To style an individual dependency line, you can provide a [cls](#Scheduler/model/DependencyModel#field-cls) in your
 * data:
 *
 * ```json
 * {
 *     "id"   : 9,
 *     "from" : 7,
 *     "to"   : 8,
 *     "cls"  : "special-dependency"
 * }
 * ```
 *
 * ```scss
 * // Make line dashed
 * .b-sch-dependency {
 *    stroke-dasharray: 5, 5;
 * }
 * ```
 *
 * To customize the marker used for the lines (the arrow header), you can supply a SVG path definition to the
 * {@link #config-markerDef} config:
 *
 * {@inlineexample Scheduler/feature/DependenciesMarker.js}
 *
 * You can also specify a {@link #config-radius} to get lines with rounded "corners", for a less boxy look:
 *
 * {@inlineexample Scheduler/feature/DependenciesRadius.js}
 *
 * For advanced use cases, you can also manipulate the {@link DomConfig} used to create a dependency line in a
 * {@link #config-renderer} function.
 *
 * This feature is **off** by default. For info on enabling it, see {@link Grid.view.mixin.GridFeatures}.
 *
 * @mixes Core/mixin/Delayable
 * @mixes Scheduler/feature/mixin/DependencyCreation
 * @mixes Scheduler/feature/mixin/DependencyTooltip
 *
 * @extends Core/mixin/InstancePlugin
 * @demo Scheduler/dependencies
 * @classtype dependencies
 * @feature
 */
class Dependencies extends InstancePlugin.mixin(AttachToProjectMixin, Delayable, DependencyCreation, DependencyGridCache, DependencyLineGenerator, DependencyTooltip) {
  static $name = 'Dependencies';
  /**
   * Fired when dependencies are rendered
   * @on-owner
   * @event dependenciesDrawn
   */
  //region Config
  static configurable = {
    /**
     * The CSS class to add to a dependency line when hovering over it
     * @config {String}
     * @default
     * @private
     */
    overCls: 'b-sch-dependency-over',
    /**
     * The CSS class applied to dependency lines
     * @config {String}
     * @default
     * @private
     */
    baseCls: 'b-sch-dependency',
    /**
     * The CSS class applied to a too narrow dependency line (to hide markers)
     * @config {String}
     * @default
     * @private
     */
    noMarkerCls: 'b-sch-dependency-markerless',
    /**
     * SVG path definition used as marker (arrow head) for the dependency lines.
     * Should fit in a viewBox that is 9 x 6.
     *
     * ```javascript
     * const scheduler = new Scheduler({
     *     features : {
     *         dependencies : {
     *             // Circular marker
     *             markerDef : 'M 2,3 a 3,3 0 1,0 6,0 a 3,3 0 1,0 -6,0'
     *         }
     *     }
     * });
     * ```
     *
     * @config {String}
     * @default 'M3,0 L3,6 L9,3 z'
     */
    markerDef: null,
    /**
     * Radius (in px) used to draw arcs where dependency line segments connect. Specify it to get a rounded look.
     * The radius will during drawing be reduced as needed on a per segment basis to fit lines.
     *
     * ```javascript
     * const scheduler = new Scheduler({
     *     features : {
     *         dependencies : {
     *             // Round the corner where line segments connect, similar to 'border-radius: 5px'
     *             radius : 5
     *         }
     *     }
     * });
     * ```
     *
     * <div class="note">Using a radius slightly degrades dependency rendering performance. If your app displays
     * a lot of dependencies, it might be worth taking this into account when deciding if you want to use radius
     * or not</div>
     *
     * @config {Number}
     */
    radius: null,
    /**
     * Renderer function, supply one if you want to manipulate the {@link DomConfig} object used to draw a
     * dependency line between two assignments.
     *
     * ```javascript
     * const scheduler = new Scheduler({
     *     features : {
     *         dependencies : {
     *             renderer({ domConfig, fromAssignmentRecord : from, toAssignmentRecord : to }) {
     *                 // Add a custom CSS class to dependencies between important assignments
     *                 domConfig.class.important = from.important || to.important;
     *                 domConfig.class.veryImportant = from.important && to.important;
     *             }
     *         }
     *     }
     * }
     * ```
     *
     * @param {Object} renderData
     * @param {DomConfig} renderData.domConfig that will be used to create the dependency line, can be manipulated by the
     * renderer
     * @param {Scheduler.model.DependencyModel} renderData.dependencyRecord The dependency being rendered
     * @param {Scheduler.model.AssignmentModel} renderData.fromAssignmentRecord Drawing line from this assignment
     * @param {Scheduler.model.AssignmentModel} renderData.toAssignmentRecord Drawing line to this assignment
     * @param {Object[]} renderData.points A collection of points making up the line segments for the dependency
     * line. Read-only in the renderer, any manipulation should be done to `domConfig`
     * @param {Core.helper.util.Rectangle} renderData.fromBox Bounds for the fromAssignment's element
     * @param {Core.helper.util.Rectangle} renderData.toBox Bounds for the toAssignment's element
     * @param {'top'|'right'|'bottom'|'left'} renderData.fromSide Drawn from this side of the fromAssignment
     * @param {'top'|'right'|'bottom'|'left'} renderData.toSide Drawn to this side of the fromAssignment
     * @prp {Function}
     */
    renderer: null,
    /**
     * Specify `true` to highlight incoming and outgoing dependencies when hovering an event.
     * @prp {Boolean}
     */
    highlightDependenciesOnEventHover: null,
    /**
     * Specify `false` to prevent dependencies from being drawn during scroll, for smoother scrolling in schedules
     * with lots of dependencies. Dependencies will be drawn when scrolling stops instead.
     * @prp {Boolean}
     * @default
     */
    drawOnScroll: true,
    /**
     * The clickable/touchable width of the dependency line in pixels. Setting this to a number greater than 1 will
     * draw an invisible but clickable line along the same path as the dependency line, making it easier to click.
     * The tradeoff is that twice as many lines will be drawn, which can affect performance.
     * @prp {Number}
     */
    clickWidth: null
  };
  static delayable = {
    doRefresh: 10
  };
  static get pluginConfig() {
    return {
      chain: ['render', 'onPaint', 'onElementClick', 'onElementDblClick', 'onElementContextMenu', 'onElementMouseOver', 'onElementMouseOut', 'bindStore'],
      assign: ['getElementForDependency', 'getElementsForDependency', 'resolveDependencyRecord']
    };
  }
  domConfigs = new Map();
  drawingLive = false;
  lastScrollX = null;
  highlighted = new Map();
  // Cached lookups
  visibleResources = null;
  usingLinks = null;
  visibleDateRange = null;
  relativeTo = null;
  //endregion
  //region Init & destroy
  construct(client, config) {
    super.construct(client, config);
    const {
      scheduledEventName
    } = client;
    client.ion({
      svgCanvasCreated: 'onSVGReady',
      // These events trigger live refresh behaviour
      animationStart: 'refresh',
      // eventDrag in Scheduler, taskDrag in Gantt
      [scheduledEventName + 'DragStart']: 'refresh',
      [scheduledEventName + 'DragAbort']: 'refresh',
      [scheduledEventName + 'ResizeStart']: 'refresh',
      [scheduledEventName + 'SegmentDragStart']: 'refresh',
      [scheduledEventName + 'SegmentResizeStart']: 'refresh',
      // These events shift the surroundings to such extent that grid cache needs rebuilding to be sure that
      // all dependencies are considered
      timelineViewportResize: 'reset',
      timeAxisViewModelUpdate: 'reset',
      toggleNode: 'reset',
      thisObj: this
    });
    client.rowManager.ion({
      refresh: 'reset',
      // For example when changing barMargin or rowHeight
      changeTotalHeight: 'reset',
      // For example when collapsing groups
      thisObj: this
    });
    this.bindStore(client.store);
  }
  doDisable(disable) {
    if (!this.isConfiguring) {
      // Need a flag to clear dependencies when disabled, since drawing is otherwise disabled too
      this._isDisabling = disable;
      this.draw();
      this._isDisabling = false;
    }
    super.doDisable(disable);
  }
  //endregion
  //region RefreshTriggers
  get rowStore() {
    return this.client.isVertical ? this.client.resourceStore : this.client.store;
  }
  // React to replacing or refreshing a display store
  bindStore(store) {
    const me = this;
    if (!me.client.isVertical) {
      me.detachListeners('store');
      if (me.client.usesDisplayStore) {
        store === null || store === void 0 ? void 0 : store.ion({
          name: 'store',
          refresh: 'onStoreRefresh',
          thisObj: me
        });
        me.reset();
      }
    }
  }
  onStoreRefresh() {
    this.reset();
  }
  attachToProject(project) {
    super.attachToProject(project);
    project === null || project === void 0 ? void 0 : project.ion({
      name: 'project',
      commitFinalized: 'reset',
      thisObj: this
    });
  }
  attachToResourceStore(resourceStore) {
    super.attachToResourceStore(resourceStore);
    resourceStore === null || resourceStore === void 0 ? void 0 : resourceStore.ion({
      name: 'resourceStore',
      change: 'onResourceStoreChange',
      refresh: 'onResourceStoreChange',
      thisObj: this
    });
  }
  onResourceStoreChange() {
    // Might have added or removed links, need to re-cache the flag
    this.usingLinks = null;
    this.reset();
  }
  attachToEventStore(eventStore) {
    super.attachToEventStore(eventStore);
    eventStore === null || eventStore === void 0 ? void 0 : eventStore.ion({
      name: 'eventStore',
      refresh: 'reset',
      thisObj: this
    });
  }
  attachToAssignmentStore(assignmentStore) {
    super.attachToAssignmentStore(assignmentStore);
    assignmentStore === null || assignmentStore === void 0 ? void 0 : assignmentStore.ion({
      name: 'assignmentStore',
      refresh: 'reset',
      thisObj: this
    });
  }
  attachToDependencyStore(dependencyStore) {
    super.attachToDependencyStore(dependencyStore);
    dependencyStore === null || dependencyStore === void 0 ? void 0 : dependencyStore.ion({
      name: 'dependencyStore',
      change: 'reset',
      refresh: 'reset',
      thisObj: this
    });
  }
  updateDrawOnScroll(drawOnScroll) {
    const me = this;
    me.detachListeners('scroll');
    if (drawOnScroll) {
      me.client.ion({
        name: 'scroll',
        scroll: 'doRefresh',
        horizontalScroll: 'onHorizontalScroll',
        prio: -100,
        // After Scheduler draws on scroll, since we target elements
        thisObj: me
      });
    } else {
      me.client.scrollable.ion({
        name: 'scroll',
        scrollEnd: 'draw',
        thisObj: me
      });
      me.client.timeAxisSubGrid.scrollable.ion({
        name: 'scroll',
        scrollEnd: 'draw',
        thisObj: me
      });
    }
  }
  onHorizontalScroll({
    subGrid,
    scrollX
  }) {
    if (scrollX !== this.lastScrollX && subGrid === this.client.timeAxisSubGrid) {
      this.lastScrollX = scrollX;
      this.draw();
    }
  }
  onPaint() {
    this.refresh();
  }
  //endregion
  //region Dependency types
  // Used by DependencyField
  static getLocalizedDependencyType(type) {
    return type ? this.L(`L{DependencyType.${type}}`) : '';
  }
  //endregion
  //region Elements
  getElementForDependency(dependency, fromAssignment, toAssignment) {
    return this.getElementsForDependency(dependency, fromAssignment, toAssignment)[0];
  }
  // NOTE: If we ever make this public we should change it to use the syncIdMap. Currently not needed since only
  // used in tests
  getElementsForDependency(dependency, fromAssignment, toAssignment) {
    // Selector targeting all instances of a dependency
    let selector = `[data-dep-id="${dependency.id}"]`;
    // Optionally narrow it down to a single instance (assignment)
    if (fromAssignment) {
      selector += `[data-from-id="${fromAssignment.id}"]`;
    }
    if (toAssignment) {
      selector += `[data-to-id="${toAssignment.id}"]`;
    }
    return Array.from(this.client.svgCanvas.querySelectorAll(selector));
  }
  /**
   * Returns the dependency record for a DOM element
   * @param {HTMLElement} element The dependency line element
   * @returns {Scheduler.model.DependencyModel} The dependency record
   */
  resolveDependencyRecord(element) {
    var _element$elementData;
    return (_element$elementData = element.elementData) === null || _element$elementData === void 0 ? void 0 : _element$elementData.dependency;
  }
  isDependencyElement(element) {
    return element.matches(`.${this.baseCls}`);
  }
  //endregion
  //region DOM Events
  onElementClick(event) {
    const dependency = this.resolveDependencyRecord(event.target);
    if (dependency) {
      const eventName = eventNameMap$1[event.type];
      /**
       * Fires on the owning Scheduler/Gantt when a context menu event is registered on a dependency line.
       * @event dependencyContextMenu
       * @on-owner
       * @param {Scheduler.view.Scheduler} source The scheduler
       * @param {Scheduler.model.DependencyModel} dependency
       * @param {MouseEvent} event
       */
      /**
       * Fires on the owning Scheduler/Gantt when a click is registered on a dependency line.
       * @event dependencyClick
       * @on-owner
       * @param {Scheduler.view.Scheduler} source The scheduler
       * @param {Scheduler.model.DependencyModel} dependency
       * @param {MouseEvent} event
       */
      /**
       * Fires on the owning Scheduler/Gantt when a double click is registered on a dependency line.
       * @event dependencyDblClick
       * @on-owner
       * @param {Scheduler.view.Scheduler} source The scheduler
       * @param {Scheduler.model.DependencyModel} dependency
       * @param {MouseEvent} event
       */
      this.client.trigger(`dependency${eventName}`, {
        dependency,
        event
      });
    }
  }
  onElementDblClick(event) {
    return this.onElementClick(event);
  }
  onElementContextMenu(event) {
    return this.onElementClick(event);
  }
  onElementMouseOver(event) {
    const me = this,
      dependency = me.resolveDependencyRecord(event.target);
    if (dependency) {
      /**
       * Fires on the owning Scheduler/Gantt when the mouse moves over a dependency line.
       * @event dependencyMouseOver
       * @on-owner
       * @param {Scheduler.view.Scheduler} source The scheduler
       * @param {Scheduler.model.DependencyModel} dependency
       * @param {MouseEvent} event
       */
      me.client.trigger('dependencyMouseOver', {
        dependency,
        event
      });
      if (me.overCls) {
        me.highlight(dependency);
      }
    }
  }
  onElementMouseOut(event) {
    const me = this,
      dependency = me.resolveDependencyRecord(event.target);
    if (dependency) {
      /**
       * Fires on the owning Scheduler/Gantt when the mouse moves out of a dependency line.
       * @event dependencyMouseOut
       * @on-owner
       * @param {Scheduler.view.Scheduler} source The scheduler
       * @param {Scheduler.model.DependencyModel} dependency
       * @param {MouseEvent} event
       */
      me.client.trigger('dependencyMouseOut', {
        dependency,
        event
      });
      if (me.overCls) {
        me.unhighlight(dependency);
      }
    }
  }
  //endregion
  //region Export
  // Export calls this fn to determine if a dependency should be included or not
  isDependencyVisible(dependency) {
    const me = this,
      {
        rowStore
      } = me,
      {
        fromEvent,
        toEvent
      } = dependency;
    // Bail out early in case source or target doesn't exist
    if (!fromEvent || !toEvent) {
      return false;
    }
    const fromResource = fromEvent.resource,
      toResource = toEvent.resource;
    // Verify these are real existing Resources and not collapsed away (resource not existing in resource store)
    if (!rowStore.isAvailable(fromResource) || !rowStore.isAvailable(toResource)) {
      return false;
    }
    return fromEvent.isModel && !fromResource.instanceMeta(rowStore).hidden && !toResource.instanceMeta(rowStore).hidden;
  }
  //endregion
  //region Highlight
  updateHighlightDependenciesOnEventHover(enable) {
    const me = this;
    if (enable) {
      const {
        client
      } = me;
      client.ion({
        name: 'highlightOnHover',
        [`${client.scheduledEventName}MouseEnter`]: params => me.highlightEventDependencies(params.eventRecord || params.taskRecord),
        [`${client.scheduledEventName}MouseLeave`]: params => me.unhighlightEventDependencies(params.eventRecord || params.taskRecord),
        thisObj: me
      });
    } else {
      me.detachListeners('highlightOnHover');
    }
  }
  highlight(dependency, cls = this.overCls) {
    let classes = this.highlighted.get(dependency);
    if (!classes) {
      this.highlighted.set(dependency, classes = new Set());
    }
    classes.add(cls);
    // Update element directly instead of refreshing and letting DomSync handle it,
    // to optimize highlight performance with many dependencies on screen
    for (const element of this.getElementsForDependency(dependency)) {
      element.classList.add(cls);
    }
  }
  unhighlight(dependency, cls = this.overCls) {
    const classes = this.highlighted.get(dependency);
    if (classes) {
      classes.delete(cls);
      if (!classes.size) {
        this.highlighted.delete(dependency);
      }
    }
    // Update element directly instead of refreshing and letting DomSync handle it,
    // to optimize highlight performance with many dependencies on screen
    for (const element of this.getElementsForDependency(dependency)) {
      element.classList.remove(cls);
    }
  }
  highlightEventDependencies(timespan, cls) {
    timespan.dependencies.forEach(dep => this.highlight(dep, cls));
  }
  unhighlightEventDependencies(timespan, cls) {
    timespan.dependencies.forEach(dep => this.unhighlight(dep, cls));
  }
  //endregion
  //region Drawing
  // Implemented in DependencyGridCache to return dependencies that might intersect the current viewport and thus
  // should be considered for drawing. Fallback value here is used when there is no grid cache (which happens when it
  // is reset. Also useful in case we want to have it configurable or opt out automatically for small datasets)
  getDependenciesToConsider(startMS, endMS, startIndex, endIndex) {
    var _super$getDependencie;
    // Get records from grid cache
    return ((_super$getDependencie = super.getDependenciesToConsider) === null || _super$getDependencie === void 0 ? void 0 : _super$getDependencie.call(this, startMS, endMS, startIndex, endIndex)) ??
    // Falling back to using all valid deps (fix for not trying to draw conflicted deps)
    this.project.dependencyStore.records.filter(d => d.isValid);
  }
  // String key used as syncId
  getDependencyKey(dependency, fromAssignment, toAssignment) {
    return `dep:${dependency.id};from:${fromAssignment.id};to:${toAssignment.id}`;
  }
  drawDependency(dependency, batch = false, forceBoxes = null) {
    var _fromAssigned, _toAssigned;
    const me = this,
      {
        domConfigs,
        client,
        rowStore,
        topIndex,
        bottomIndex
      } = me,
      {
        eventStore,
        useInitialAnimation
      } = client,
      {
        idMap
      } = rowStore,
      {
        startMS,
        endMS
      } = me.visibleDateRange,
      {
        fromEvent,
        toEvent
      } = dependency;
    let fromAssigned = fromEvent.assigned,
      toAssigned = toEvent.assigned;
    if (
    // No point in trying to draw dep between unscheduled/non-existing events
    fromEvent.isScheduled && toEvent.isScheduled &&
    // Or between filtered out events
    eventStore.includes(fromEvent) && eventStore.includes(toEvent) && // Or unassigned ones
    (_fromAssigned = fromAssigned) !== null && _fromAssigned !== void 0 && _fromAssigned.size && (_toAssigned = toAssigned) !== null && _toAssigned !== void 0 && _toAssigned.size) {
      // Add links, if used
      if (me.usingLinks) {
        fromAssigned = [...fromAssigned].flatMap(collectLinkedAssignments);
        toAssigned = [...toAssigned].flatMap(collectLinkedAssignments);
      }
      for (const from of fromAssigned) {
        for (const to of toAssigned) {
          var _idMap$from$resource$, _from$resource, _idMap$to$resource$id, _to$resource;
          const
            // Using direct lookup in idMap instead of indexOf() for performance.
            // Resource might be filtered out or not exist at all
            fromIndex = (_idMap$from$resource$ = idMap[(_from$resource = from.resource) === null || _from$resource === void 0 ? void 0 : _from$resource.id]) === null || _idMap$from$resource$ === void 0 ? void 0 : _idMap$from$resource$.index,
            toIndex = (_idMap$to$resource$id = idMap[(_to$resource = to.resource) === null || _to$resource === void 0 ? void 0 : _to$resource.id]) === null || _idMap$to$resource$id === void 0 ? void 0 : _idMap$to$resource$id.index,
            fromDateMS = Math.min(fromEvent.startDateMS, toEvent.startDateMS),
            toDateMS = Math.max(fromEvent.endDateMS, toEvent.endDateMS);
          // Draw only if dependency intersects view, unless it is part of an export
          if (client.isExporting || fromIndex != null && toIndex != null && from.drawDependencies !== false && to.drawDependencies !== false && rowStore.isAvailable(from.resource) && rowStore.isAvailable(to.resource) && !(
          // Both ends above view
          fromIndex < topIndex && toIndex < topIndex ||
          // Both ends below view
          fromIndex > bottomIndex && toIndex > bottomIndex ||
          // Both ends before view
          fromDateMS < startMS && toDateMS < startMS ||
          // Both ends after view
          fromDateMS > endMS && toDateMS > endMS)) {
            const key = me.getDependencyKey(dependency, from, to),
              lineDomConfigs = me.getDomConfigs(dependency, from, to, forceBoxes);
            if (lineDomConfigs) {
              // Allow deps to match animation delay of their events (the bottommost one) when fading in
              if (useInitialAnimation) {
                lineDomConfigs[0].style = {
                  animationDelay: `${Math.max(fromIndex, toIndex) / 20 * 1000}ms`
                };
              }
              domConfigs.set(key, lineDomConfigs);
            }
            // No room to draw a line
            else {
              domConfigs.delete(key);
            }
          }
          // Give mixins a shot at running code after a dependency is drawn. Used by grid cache to cache the
          // dependency (when needed)
          me.afterDrawDependency(dependency, fromIndex, toIndex, fromDateMS, toDateMS);
        }
      }
    }
    if (!batch) {
      me.domSync();
    }
  }
  // Hooks used by grid cache, to keep code in this file readable
  afterDrawDependency(dependency, fromIndex, toIndex, fromDateMS, toDateMS) {
    var _super$afterDrawDepen;
    (_super$afterDrawDepen = super.afterDrawDependency) === null || _super$afterDrawDepen === void 0 ? void 0 : _super$afterDrawDepen.call(this, dependency, fromIndex, toIndex, fromDateMS, toDateMS);
  }
  beforeDraw() {
    var _super$beforeDraw;
    (_super$beforeDraw = super.beforeDraw) === null || _super$beforeDraw === void 0 ? void 0 : _super$beforeDraw.call(this);
  }
  afterDraw() {
    var _super$afterDraw;
    (_super$afterDraw = super.afterDraw) === null || _super$afterDraw === void 0 ? void 0 : _super$afterDraw.call(this);
  }
  // Update DOM
  domSync(targetElement = this.client.svgCanvas) {
    DomSync.sync({
      targetElement,
      domConfig: {
        onlyChildren: true,
        children: Array.from(this.domConfigs.values()).flat()
      },
      syncIdField: 'syncId',
      releaseThreshold: 0,
      strict: true,
      callback() {}
    });
  }
  fillDrawingCache() {
    const me = this,
      {
        client
      } = me;
    // Cache subgrid bounds for the duration of this draw call to not have to figure it out per dep
    me.relativeTo = Rectangle.from(client.foregroundCanvas);
    // Cache other lookups too
    me.visibleResources = client.visibleResources;
    me.visibleDateRange = client.visibleDateRange;
    me.topIndex = me.rowStore.indexOf(me.visibleResources.first);
    me.bottomIndex = me.rowStore.indexOf(me.visibleResources.last);
    // Cache link lookup, to avoid semi-expensive flatMap calls in drawDependency
    if (me.usingLinks == null) {
      me.usingLinks = client.resourceStore.some(r => r.hasLinks);
    }
  }
  // Draw all dependencies intersecting the current viewport immediately
  draw() {
    const me = this,
      {
        client
      } = me;
    if (client.refreshSuspended || !client.foregroundCanvas || !client.isEngineReady || me.disabled && !me._isDisabling || client.isExporting) {
      return;
    }
    me.fillDrawingCache();
    me.domConfigs.clear();
    // Nothing to draw if there are no rows or no ticks or we are disabled
    if (client.firstVisibleRow && client.lastVisibleRow && client.timeAxis.count && !me.disabled && me.visibleDateRange.endMS - me.visibleDateRange.startMS > 0) {
      const {
          visibleDateRange
        } = client,
        {
          topIndex,
          bottomIndex
        } = me,
        dependencies = me.getDependenciesToConsider(visibleDateRange.startMS, visibleDateRange.endMS, topIndex, bottomIndex);
      // Give mixins a shot at doing something before deps are drawn. Used by grid cache to determine if
      // the cache should be rebuilt
      me.beforeDraw();
      for (const dependency of dependencies) {
        me.drawDependency(dependency, true);
      }
      // Give mixins a shot at doing something after all deps are drawn
      me.afterDraw();
    }
    me.domSync();
    client.trigger('dependenciesDrawn');
  }
  //endregion
  //region Refreshing
  // Performs a draw on next frame, not intended to be called directly, call refresh() instead
  doRefresh() {
    var _client$features, _client$features2, _client$features3, _client$features4;
    const me = this,
      {
        client
      } = me,
      {
        scheduledEventName
      } = client;
    me.draw();
    // Refresh each frame during animations, during dragging & resizing  (if we have dependencies)
    me.drawingLive = client.dependencyStore.count && (client.isAnimating || client.useInitialAnimation && client.eventStore.count || ((_client$features = client.features[`${scheduledEventName}Drag`]) === null || _client$features === void 0 ? void 0 : _client$features.isActivelyDragging) || ((_client$features2 = client.features[`${scheduledEventName}Resize`]) === null || _client$features2 === void 0 ? void 0 : _client$features2.isResizing) || ((_client$features3 = client.features[`${scheduledEventName}SegmentDrag`]) === null || _client$features3 === void 0 ? void 0 : _client$features3.isActivelyDragging) || ((_client$features4 = client.features[`${scheduledEventName}SegmentResize`]) === null || _client$features4 === void 0 ? void 0 : _client$features4.isResizing));
    me.drawingLive && me.refresh();
  }
  /**
   * Redraws dependencies on the next animation frame
   */
  refresh() {
    const {
      client
    } = this;
    // Queue up a draw unless refresh is suspended
    if (!client.refreshSuspended && !this.disabled && client.isPainted && !client.timeAxisSubGrid.collapsed) {
      this.doRefresh();
    }
  }
  // Resets grid cache and performs a draw on next frame. Conditions when it should be called:
  // * Zooming
  // * Shifting time axis
  // * Resizing window
  // * CRUD
  // ...
  reset() {
    var _super$reset;
    (_super$reset = super.reset) === null || _super$reset === void 0 ? void 0 : _super$reset.call(this);
    this.refresh();
  }
  /**
   * Draws all dependencies for the specified task.
   * @deprecated 5.1 The Dependencies feature was refactored and this fn is no longer needed
   */
  drawForEvent() {
    VersionHelper.deprecate('Scheduler', '6.0.0', 'Dependencies.drawForEvent() is no longer needed');
    this.refresh();
  }
  //endregion
  //region Scheduler hooks
  render() {
    // Pull in the svg canvas early to have it available during drawing
    this.client.getConfig('svgCanvas');
  }
  //endregion
}

Dependencies._$name = 'Dependencies';
GridFeatureManager.registerFeature(Dependencies, false, ['Scheduler', 'ResourceHistogram']);
GridFeatureManager.registerFeature(Dependencies, true, 'SchedulerPro');

/**
 * @module Scheduler/feature/EventFilter
 */
/**
 * Adds event filter menu items to the timeline header context menu.
 *
 * {@inlineexample Scheduler/feature/EventFilter.js}
 *
 * ```javascript
 * const scheduler = new Scheduler({
 *   features : {
 *     eventFilter : true // `true` by default, set to `false` to disable the feature and remove the menu item from the timeline header
 *   }
 * });
 * ```
 *
 * This feature is **enabled** by default
 *
 * @extends Core/mixin/InstancePlugin
 * @classtype eventFilter
 * @feature
 */
class EventFilter extends InstancePlugin {
  static get $name() {
    return 'EventFilter';
  }
  static get pluginConfig() {
    return {
      chain: ['populateTimeAxisHeaderMenu']
    };
  }
  /**
   * Populates the header context menu items.
   * @param {Object} options Contains menu items and extra data retrieved from the menu target.
   * @param {Object<String,MenuItemConfig|Boolean|null>} options.items A named object to describe menu items
   * @internal
   */
  populateTimeAxisHeaderMenu({
    items
  }) {
    const me = this;
    items.eventsFilter = {
      text: 'L{filterEvents}',
      icon: 'b-fw-icon b-icon-filter',
      disabled: me.disabled,
      localeClass: me,
      weight: 100,
      menu: {
        type: 'popup',
        localeClass: me,
        items: {
          nameFilter: {
            weight: 110,
            type: 'textfield',
            cls: 'b-eventfilter b-last-row',
            clearable: true,
            keyStrokeChangeDelay: 300,
            label: 'L{byName}',
            localeClass: me,
            width: 200,
            internalListeners: {
              change: me.onEventFilterChange,
              thisObj: me
            }
          }
        },
        onBeforeShow({
          source: menu
        }) {
          const [filterByName] = menu.items,
            filter = me.store.filters.getBy('property', 'name');
          filterByName.value = (filter === null || filter === void 0 ? void 0 : filter.value) || '';
        }
      }
    };
  }
  onEventFilterChange({
    value
  }) {
    if (value !== '') {
      this.store.filter('name', value);
    } else {
      this.store.removeFilter('name');
    }
  }
  get store() {
    const {
      client
    } = this;
    return client.isGanttBase ? client.store : client.eventStore;
  }
}
EventFilter.featureClass = 'b-event-filter';
EventFilter._$name = 'EventFilter';
GridFeatureManager.registerFeature(EventFilter, true, ['Scheduler', 'Gantt']);
GridFeatureManager.registerFeature(EventFilter, false, 'ResourceHistogram');

/**
 * @module Scheduler/feature/mixin/NonWorkingTimeMixin
 */
/**
 * Mixin with functionality shared between {@link Scheduler/feature/NonWorkingTime} and
 * {@link Scheduler/feature/EventNonWorkingTime}.
 * @mixin
 */
var NonWorkingTimeMixin = (Target => class NonWorkingTimeMixin extends Target {
  static $name = 'NonWorkingTimeMixin';
  static configurable = {
    /**
     * The maximum time axis unit to display non-working ranges for ('hour' or 'day' etc).
     * When zooming to a view with a larger unit, no non-working time elements will be rendered.
     *
     * **Note:** Be careful with setting this config to big units like 'year'. When doing this,
     * make sure the timeline {@link Scheduler/view/TimelineBase#config-startDate start} and
     * {@link Scheduler/view/TimelineBase#config-endDate end} dates are set tightly.
     * When using a long range (for example many years) with non-working time elements rendered per hour,
     * you will end up with millions of elements, impacting performance.
     * When zooming, use the {@link Scheduler/view/mixin/TimelineZoomable#config-zoomKeepsOriginalTimespan} config.
     * @config {'millisecond'|'second'|'minute'|'hour'|'day'|'week'|'month'|'quarter'|'year'}
     * @default
     */
    maxTimeAxisUnit: 'week'
  };
  getNonWorkingTimeRanges(calendar, startDate, endDate) {
    if (!calendar.getNonWorkingTimeRanges) {
      const result = [];
      calendar.forEachAvailabilityInterval({
        startDate,
        endDate,
        isForward: true
      }, (intervalStartDate, intervalEndDate, calendarCacheInterval) => {
        for (const [entry, cache] of calendarCacheInterval.intervalGroups) {
          if (!cache.getIsWorking()) {
            result.push({
              name: entry.name,
              iconCls: entry.iconCls,
              cls: entry.cls,
              startDate: intervalStartDate,
              endDate: intervalEndDate
            });
          }
        }
      });
      return result;
    }
    return calendar.getNonWorkingTimeRanges(startDate, endDate);
  }
  getCalendarTimeRanges(calendar, ignoreName = false) {
    const me = this,
      {
        timeAxis,
        fillTicks
      } = me.client,
      {
        unit,
        increment
      } = timeAxis,
      shouldPaint = !me.maxTimeAxisUnit || DateHelper.compareUnits(unit, me.maxTimeAxisUnit) <= 0;
    if (calendar && shouldPaint && timeAxis.count) {
      const allRanges = me.getNonWorkingTimeRanges(calendar, timeAxis.startDate, timeAxis.endDate),
        timeSpans = allRanges.map(interval => new TimeSpan({
          name: interval.name,
          cls: `b-nonworkingtime ${interval.cls || ''}`,
          startDate: interval.startDate,
          endDate: interval.endDate
        })),
        mergedSpans = [];
      let prevRange = null;
      // intervals returned by the calendar are not merged, let's combine them to yield fewer elements
      for (const range of timeSpans) {
        if (prevRange && range.startDate <= prevRange.endDate && (ignoreName || range.name === prevRange.name) && range.duration > 0) {
          prevRange.endDate = range.endDate;
        } else {
          mergedSpans.push(range);
          range.setData('id', `nonworking-${mergedSpans.length}`);
          prevRange = range;
        }
      }
      // When filling ticks, non-working-time ranges are cropped to full ticks too
      if (fillTicks) {
        mergedSpans.forEach(span => {
          span.setStartEndDate(DateHelper.ceil(span.startDate, {
            magnitude: increment,
            unit
          }), DateHelper.floor(span.endDate, {
            magnitude: increment,
            unit
          }));
        });
      }
      return mergedSpans;
    } else {
      return [];
    }
  }
  //region Basic scheduler calendar
  setupDefaultCalendar() {
    const {
      client,
      project
    } = this;
    if (
    // Might have been set up by NonWorkingTime / EventNonWorkingTime already
    !this.autoGeneratedWeekends &&
    // For basic scheduler...
    !client.isSchedulerPro && !client.isGantt &&
    // ...that uses the default calendar...
    project.effectiveCalendar === project.defaultCalendar &&
    // ...and has no defined intervals
    !project.defaultCalendar.intervalStore.count) {
      this.autoGeneratedWeekends = true;
      this.updateDefaultCalendar();
    }
  }
  updateDefaultCalendar() {
    if (this.autoGeneratedWeekends) {
      const calendar = this.client.project.effectiveCalendar,
        intervals = this.defaultNonWorkingIntervals,
        hasIntervals = Boolean(intervals.length);
      calendar.clearIntervals(hasIntervals);
      // Update weekends as non-working time
      if (hasIntervals) {
        calendar.addIntervals(intervals);
      }
    }
  }
  updateLocalization() {
    var _super$updateLocaliza;
    (_super$updateLocaliza = super.updateLocalization) === null || _super$updateLocaliza === void 0 ? void 0 : _super$updateLocaliza.call(this);
    this.autoGeneratedWeekends && this.updateDefaultCalendar();
  }
  get defaultNonWorkingIntervals() {
    const dayNames = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
    return DateHelper.nonWorkingDaysAsArray.map(dayIndex => ({
      recurrentStartDate: `on ${dayNames[dayIndex]} at 0:00`,
      recurrentEndDate: `on ${dayNames[(dayIndex + 1) % 7]} at 0:00`,
      isWorking: false
    }));
  }
  //endregion
});

/**
 * @module Scheduler/feature/NonWorkingTime
 */
/**
 * Feature that allows styling of weekends (and other non working time) by adding timeRanges for those days.
 *
 * {@inlineexample Scheduler/feature/NonWorkingTime.js}
 *
 * By default, the basic Scheduler's calendar is empty. When enabling this feature in the basic Scheduler, it injects
 * Saturday and Sunday weekend intervals if no intervals are encountered. For Scheduler Pro, it visualizes the project
 * calendar and does not automatically inject anything. You have to define a Calendar in the application and assign it
 * to the project, for more information on how to do that, please see Scheduler Pro's Scheduling/Calendars guide.
 *
 * Please note that to not clutter the view (and have a large negative effect on performance) the feature does not
 * render ranges shorter than the base unit used by the time axis. The behavior can be disabled with
 * {@link #config-hideRangesOnZooming} config.
 *
 * The feature also bails out of rendering ranges for very zoomed out views completely for the same reasons (see
 * {@link #config-maxTimeAxisUnit} for details).
 *
 * Also note that the feature uses virtualized rendering, only the currently visible non-working-time ranges are
 * available in the DOM.
 *
 * This feature is **off** by default for Scheduler, but **enabled** by default for Scheduler Pro.
 * For info on enabling it, see {@link Grid/view/mixin/GridFeatures}.
 *
 * @extends Scheduler/feature/AbstractTimeRanges
 * @demo Scheduler/nonworkingdays
 * @classtype nonWorkingTime
 * @mixes Scheduler/feature/mixin/NonWorkingTimeMixin
 * @feature
 */
class NonWorkingTime extends AbstractTimeRanges.mixin(AttachToProjectMixin, NonWorkingTimeMixin) {
  //region Default config
  static $name = 'NonWorkingTime';
  /** @hideconfigs enableResizing, store*/
  static get defaultConfig() {
    return {
      /**
       * Set to `true` to highlight non working periods of time
       * @config {Boolean}
       * @deprecated Since 5.2.0, will be removed since the feature is pointless if set to false
       */
      highlightWeekends: null,
      /**
       * The feature by default does not render ranges smaller than the base unit used by the time axis.
       * Set this config to `false` to disable this behavior.
       *
       * <div class="note">The {@link #config-maxTimeAxisUnit} config defines a zoom level at which to bail out of
       * rendering ranges completely.</div>
       * @config {Boolean}
       * @default
       */
      hideRangesOnZooming: true,
      showHeaderElements: true,
      showLabelInBody: true,
      autoGeneratedWeekends: false
    };
  }
  static pluginConfig = {
    chain: ['onPaint', 'attachToProject', 'updateLocalization', 'onConfigChange', 'onSchedulerHorizontalScroll']
  };
  //endregion
  //region Init & destroy
  doDestroy() {
    this.attachToCalendar(null);
    super.doDestroy();
  }
  set highlightWeekends(highlight) {
    VersionHelper.deprecate('Scheduler', '6.0.0', 'Deprecated in favour of disabling the feature');
    this.disabled = !highlight;
  }
  get highlightWeekends() {
    return !this.disabled;
  }
  onConfigChange({
    name
  }) {
    if (!this.isConfiguring && name === 'fillTicks') {
      this.refresh();
    }
  }
  //endregion
  //region Project
  attachToProject(project) {
    super.attachToProject(project);
    this.attachToCalendar(project.effectiveCalendar);
    // if there's no graph yet - need to delay this call until it appears, but not for scheduler
    if (!project.graph && !this.client.isScheduler) {
      project.ion({
        name: 'project',
        dataReady: {
          fn: () => this.attachToCalendar(project.effectiveCalendar),
          once: true
        },
        thisObj: this
      });
    }
    project.ion({
      name: 'project',
      calendarChange: () => this.attachToCalendar(project.effectiveCalendar),
      thisObj: this
    });
  }
  //endregion
  //region TimeAxisViewModel
  onTimeAxisViewModelUpdate(...args) {
    this._timeAxisUnitDurationMs = null;
    return super.onTimeAxisViewModelUpdate(...args);
  }
  //endregion
  //region Calendar
  attachToCalendar(calendar) {
    const me = this,
      {
        project,
        client
      } = me;
    me.detachListeners('calendar');
    me.autoGeneratedWeekends = false;
    if (calendar) {
      // Sets up a default weekend calendar for basic Scheduler, when no calendar is set
      me.setupDefaultCalendar();
      calendar.intervalStore.ion({
        name: 'calendar',
        change: () => me.setTimeout(() => me.refresh(), 1)
      });
    }
    // On changing calendar we react to a data level event which is triggered after project refresh.
    // Redraw right away
    if (client.isEngineReady && !client.project.isDelayingCalculation && !client.isDestroying) {
      me.refresh();
    }
    // Initially there is no guarantee we are ready to draw, wait for refresh
    else if (!project.isDestroyed) {
      me.detachListeners('initialProjectListener');
      project.ion({
        name: 'initialProjectListener',
        refresh({
          isCalculated
        }) {
          // Cant render early, have to wait for calculations
          if (isCalculated !== false) {
            me.refresh();
            me.detachListeners('initialProjectListener');
          }
        },
        thisObj: me
      });
    }
  }
  get calendar() {
    var _this$project;
    return (_this$project = this.project) === null || _this$project === void 0 ? void 0 : _this$project.effectiveCalendar;
  }
  //endregion
  //region Draw
  get timeAxisUnitDurationMs() {
    // calculate and cache duration of the timeAxis unit in milliseconds
    if (!this._timeAxisUnitDurationMs) {
      this._timeAxisUnitDurationMs = DateHelper.as('ms', 1, this.client.timeAxis.unit);
    }
    return this._timeAxisUnitDurationMs;
  }
  /**
   * Based on this method result the feature decides whether the provided non-working period should
   * be rendered or not.
   * The method checks that the range has non-zero {@link Scheduler.model.TimeSpan#field-duration},
   * lays in the visible timespan and its duration is longer or equal the base timeaxis unit
   * (if {@link #config-hideRangesOnZooming} is `true`).
   *
   * Override the method to implement your custom range rendering vetoing logic.
   * @param {Scheduler.model.TimeSpan} range Range to render.
   * @returns {Boolean} `true` if the range should be rendered and `false` otherwise.
   */
  shouldRenderRange(range) {
    // if the range is longer or equal than one timeAxis unit then render it
    return super.shouldRenderRange(range) && (!this.hideRangesOnZooming || range.durationMS >= this.timeAxisUnitDurationMs);
  }
  // Calendar intervals as TimeSpans, with adjacent intervals merged to create fewer
  get timeRanges() {
    const me = this;
    if (!me._timeRanges) {
      me._timeRanges = me.getCalendarTimeRanges(me.calendar);
    }
    return me._timeRanges;
  }
  //endregion
}

NonWorkingTime._$name = 'NonWorkingTime';
GridFeatureManager.registerFeature(NonWorkingTime, false, 'Scheduler');
GridFeatureManager.registerFeature(NonWorkingTime, true, ['SchedulerPro', 'Gantt', 'ResourceHistogram']);

/**
 * @module Scheduler/feature/ScheduleTooltip
 */
/**
 * Feature that displays a tooltip containing the time at the mouse position when hovering empty parts of the schedule.
 * To hide the schedule tooltip, just disable this feature:
 *
 * ```javascript
 * const scheduler = new Scheduler({
 *     features : {
 *         scheduleTooltip : false
 *     }
 * });
 * ```
 *
 * You can also output a message along with the default time indicator (to indicate resource availability etc)
 *
 * ```javascript
 * const scheduler = new Scheduler({
 *    features : {
 *       scheduleTooltip : {
 *           getText(date, event, resource) {
 *               return 'Hovering ' + resource.name;
 *           }
 *       }
 *   }
 * });
 * ```
 *
 * To take full control over the markup shown in the tooltip you can override the {@link #function-generateTipContent} method:
 *
 * ```javascript
 * const scheduler = new Scheduler({
 *     features : {
 *         scheduleTooltip : {
 *             generateTipContent({ date, event, resourceRecord }) {
 *                 return `
 *                     <dl>
 *                         <dt>Date</dt><dd>${date}</dd>
 *                         <dt>Resource</dt><dd>${resourceRecord.name}</dd>
 *                     </dl>
 *                 `;
 *             }
 *         }
 *     }
 * });
 * ```
 *
 * Configuration properties from the feature are passed down into the resulting {@link Core.widget.Tooltip} instance.
 *
 * ```javascript
 * const scheduler = new Scheduler({
 *     features : {
 *         scheduleTooltip : {
 *             // Don't show the tip until the mouse has been over the schedule for three seconds
 *             hoverDelay : 3000
 *         }
 *     }
 * });
 * ```
 *
 * @extends Core/mixin/InstancePlugin
 * @demo Scheduler/basic
 * @inlineexample Scheduler/feature/ScheduleTooltip.js
 * @classtype scheduleTooltip
 * @feature
 */
class ScheduleTooltip extends InstancePlugin {
  //region Config
  static get $name() {
    return 'ScheduleTooltip';
  }
  static get configurable() {
    return {
      messageTemplate: data => `<div class="b-sch-hovertip-msg">${data.message}</div>`,
      /**
       * Set to `true` to hide this tooltip when hovering non-working time. Defaults to `false` for Scheduler,
       * `true` for SchedulerPro
       * @config {Boolean}
       */
      hideForNonWorkingTime: null
    };
  }
  // Plugin configuration. This plugin chains some of the functions in Grid.
  static get pluginConfig() {
    return {
      chain: ['onPaint']
    };
  }
  //endregion
  //region Init
  /**
   * Set up drag and drop and hover tooltip.
   * @private
   */
  onPaint({
    firstPaint
  }) {
    if (!firstPaint) {
      return;
    }
    const me = this,
      {
        client
      } = me;
    if (client.isSchedulerPro && me.hideForNonWorkingTime === undefined) {
      me.hideForNonWorkingTime = true;
    }
    let reshowListener;
    const tip = me.hoverTip = new Tooltip({
      id: `${client.id}-schedule-tip`,
      cls: 'b-sch-scheduletip',
      allowOver: true,
      hoverDelay: 0,
      hideDelay: 100,
      showOnHover: true,
      forElement: client.timeAxisSubGridElement,
      anchorToTarget: false,
      trackMouse: true,
      updateContentOnMouseMove: true,
      // disable text content and monitor resize for tooltip, otherwise it doesn't
      // get sized properly on first appearance
      monitorResize: false,
      textContent: false,
      forSelector: '.b-schedulerbase:not(.b-dragging-event):not(.b-dragcreating) .b-grid-body-container:not(.b-scrolling) .b-timeline-subgrid:not(.b-scrolling) > :not(.b-sch-foreground-canvas):not(.b-group-footer):not(.b-group-row) *',
      // Do not constrain at all, want it to be able to go outside of the viewport to not get in the way
      getHtml: me.getHoverTipHtml.bind(me),
      onDocumentMouseDown(event) {
        // Click on the scheduler hides until the very next
        // non-button-pressed mouse move!
        if (tip.forElement.contains(event.event.target)) {
          reshowListener = EventHelper.on({
            thisObj: me,
            element: client.timeAxisSubGridElement,
            mousemove: e => tip.internalOnPointerOver(e),
            capture: true
          });
        }
        const hideAnimation = tip.hideAnimation;
        tip.hideAnimation = false;
        tip.constructor.prototype.onDocumentMouseDown.call(tip, event);
        tip.hideAnimation = hideAnimation;
      },
      // on Core/mixin/Events constructor, me.config.listeners is deleted and attributed its value to me.configuredListeners
      // to then on processConfiguredListeners it set me.listeners to our TooltipBase
      // but since we need our initial config.listeners to set to our internal tooltip, we leave processConfiguredListeners empty
      // to avoid lost our listeners to apply for our internal tooltip here and force our feature has all Tooltip events firing
      ...me.config,
      internalListeners: me.configuredListeners
    });
    // We have to add our own listener after instantiation because it may conflict with a configured listener
    tip.ion({
      pointerover({
        event
      }) {
        const buttonsPressed = 'buttons' in event ? event.buttons > 0 : event.which > 0; // fallback for Safari which doesn't support 'buttons'
        // This is the non-button-pressed mousemove
        // after the document mousedown
        if (!buttonsPressed && reshowListener) {
          reshowListener();
        }
        // Never any tooltip while interaction is ongoing and a mouse button is pressed
        return !me.disabled && !buttonsPressed;
      },
      innerhtmlupdate({
        source
      }) {
        me.clockTemplate.updateDateIndicator(source.element, me.lastTime);
      }
    });
    // Update tooltip after zooming
    client.ion({
      timeAxisViewModelUpdate: 'updateTip',
      thisObj: me
    });
    me.clockTemplate = new ClockTemplate({
      scheduler: client
    });
  }
  // leave configuredListeners alone until render time at which they are used on the tooltip
  processConfiguredListeners() {}
  updateTip() {
    if (this.hoverTip.isVisible) {
      this.hoverTip.updateContent();
    }
  }
  doDestroy() {
    this.destroyProperties('clockTemplate', 'hoverTip');
    super.doDestroy();
  }
  //endregion
  //region Contents
  /**
   * @deprecated Use {@link #function-generateTipContent} instead.
   * Gets html to display in hover tooltip (tooltip displayed on empty parts of scheduler)
   * @private
   */
  getHoverTipHtml({
    tip,
    event
  }) {
    const me = this,
      scheduler = me.client,
      date = event && scheduler.getDateFromDomEvent(event, 'floor', true);
    let html = me.lastHtml;
    // event.target might be null in the case of being hosted in a web component https://github.com/bryntum/bryntum-suite/pull/4488
    if (date && event.target) {
      const resourceRecord = scheduler.resolveResourceRecord(event);
      // resourceRecord might be null if user hover over the tooltip, but we shouldn't hide the tooltip in this case
      if (resourceRecord && (date - me.lastTime !== 0 || resourceRecord.id !== me.lastResourceId)) {
        if (me.hideForNonWorkingTime) {
          const isWorkingTime = resourceRecord.isWorkingTime(date);
          tip.element.classList.toggle('b-nonworking-time', !isWorkingTime);
        }
        me.lastResourceId = resourceRecord.id;
        html = me.lastHtml = me.generateTipContent({
          date,
          event,
          resourceRecord
        });
      }
    } else {
      tip.hide();
      me.lastTime = null;
      me.lastResourceId = null;
    }
    return html;
  }
  /**
   * Called as mouse pointer is moved over a new resource or time block. You can override this to show
   * custom HTML in the tooltip.
   * @param {Object} context
   * @param {Date} context.date The date of the hovered point
   * @param {Event} context.event The DOM event that triggered this tooltip to show
   * @param {Scheduler.model.ResourceModel} context.resourceRecord The resource record
   * @returns {String} The HTML contents to show in the tooltip (an empty return value will hide the tooltip)
   */
  generateTipContent({
    date,
    event,
    resourceRecord
  }) {
    const me = this,
      clockHtml = me.clockTemplate.generateContent({
        date,
        text: me.client.getFormattedDate(date)
      }),
      messageHtml = me.messageTemplate({
        message: me.getText(date, event, resourceRecord) || ''
      });
    me.lastTime = date;
    return clockHtml + messageHtml;
  }
  /**
   * Override this to render custom text to default hover tip
   * @param {Date} date
   * @param {Event} event Browser event
   * @param {Scheduler.model.ResourceModel} resourceRecord The resource record
   * @returns {String}
   */
  getText(date, event, resourceRecord) {}
  //endregion
}

ScheduleTooltip.featureClass = 'b-scheduletip';
ScheduleTooltip._$name = 'ScheduleTooltip';
GridFeatureManager.registerFeature(ScheduleTooltip, true, 'Scheduler');
GridFeatureManager.registerFeature(ScheduleTooltip, false, 'ResourceUtilization');

/**
 * @module Scheduler/feature/TimeAxisHeaderMenu
 */
const setTimeSpanOptions = {
  maintainVisibleStart: true
};
/**
 * Adds scheduler specific menu items to the timeline header context menu.
 *
 * ## Default timeaxis header menu items
 *
 * Here is the list of menu items provided by this and other features:
 *
 * | Reference          | Text                  | Weight | Feature                                           | Description                  |
 * |--------------------|-----------------------|--------|---------------------------------------------------|------------------------------|
 * | `eventsFilter`     | Filter tasks          | 100    | {@link Scheduler.feature.EventFilter EventFilter} | Submenu for event filtering  |
 * | \>`nameFilter`     | By name               | 110    | {@link Scheduler.feature.EventFilter EventFilter} | Filter by `name`             |
 * | `zoomLevel`        | Zoom                  | 200    | *This feature*                                    | Submenu for timeline zooming |
 * | \>`zoomSlider`     | -                     | 210    | *This feature*                                    | Changes current zoom level   |
 * | `dateRange`        | Date range            | 300    | *This feature*                                    | Submenu for timeline range   |
 * | \>`startDateField` | Start date            | 310    | *This feature*                                    | Start date for the timeline  |
 * | \>`endDateField`   | End date              | 320    | *This feature*                                    | End date for the timeline    |
 * | \>`leftShiftBtn`   | <                     | 330    | *This feature*                                    | Shift backward               |
 * | \>`todayBtn`       | Today                 | 340    | *This feature*                                    | Go to today                  |
 * | \>`rightShiftBtn`  | \>                    | 350    | *This feature*                                    | Shift forward                |
 * | `currentTimeLine`  | Show current timeline | 400    | {@link Scheduler.feature.TimeRanges TimeRanges}   | Show current time line       |
 *
 * \> - first level of submenu
 *
 * ## Customizing the menu items
 *
 * The menu items in the TimeAxis Header menu can be customized, existing items can be changed or removed,
 * and new items can be added. This is handled using the `items` config of the feature.
 *
 * ### Add extra items:
 *
 * ```javascript
 * const scheduler = new Scheduler({
 *     features : {
 *         timeAxisHeaderMenu : {
 *             items : {
 *                 extraItem : {
 *                     text : 'Extra',
 *                     icon : 'b-fa b-fa-fw b-fa-flag',
 *                     onItem() {
 *                         ...
 *                     }
 *                 }
 *             }
 *         }
 *     }
 * });
 * ```
 *
 * ### Remove existing items:
 *
 * ```javascript
 * const scheduler = new Scheduler({
 *     features : {
 *         timeAxisHeaderMenu : {
 *             items : {
 *                 zoomLevel : false
 *             }
 *         }
 *     }
 * });
 * ```
 *
 * ### Customize existing item:
 *
 * ```javascript
 * const scheduler = new Scheduler({
 *     features : {
 *         timeAxisHeaderMenu : {
 *             items : {
 *                 zoomLevel : {
 *                     text : 'Scale'
 *                 }
 *             }
 *         }
 *     }
 * });
 * ```
 *
 * ### Customizing submenu items:
 *
 * ```javascript
 * const scheduler = new Scheduler({
 *      features : {
 *          timeAxisHeaderMenu : {
 *              items : {
 *                  dateRange : {
 *                      menu : {
 *                          items : {
 *                              todayBtn : {
 *                                  text : 'Now'
 *                              }
 *                          }
 *                      }
 *                  }
 *              }
 *          }
 *      }
 * });
 * ```
 *
 * ### Manipulate existing items:
 *
 * ```javascript
 * const scheduler = new Scheduler({
 *     features : {
 *         timeAxisHeaderMenu : {
 *             // Process items before menu is shown
 *             processItems({ items }) {
 *                  // Add an extra item dynamically
 *                 items.coolItem = {
 *                     text : 'Cool action',
 *                     onItem() {
 *                           // ...
 *                     }
 *                 }
 *             }
 *         }
 *     }
 * });
 * ```
 *
 * Full information of the menu customization can be found in the ["Customizing the Event menu, the Schedule menu, and the TimeAxisHeader menu"](#Scheduler/guides/customization/contextmenu.md)
 * guide.
 *
 * This feature is **enabled** by default
 *
 * @extends Grid/feature/HeaderMenu
 * @demo Scheduler/basic
 * @classtype timeAxisHeaderMenu
 * @feature
 * @inlineexample Scheduler/feature/TimeAxisHeaderMenu.js
 */
class TimeAxisHeaderMenu extends HeaderMenu {
  //region Config
  static get $name() {
    return 'TimeAxisHeaderMenu';
  }
  static get defaultConfig() {
    return {
      /**
       * A function called before displaying the menu that allows manipulations of its items.
       * Returning `false` from this function prevents the menu being shown.
       *
       * ```javascript
       *   features         : {
       *       timeAxisHeaderMenu : {
       *           processItems({ items }) {
       *               // Add or hide existing items here as needed
       *               items.myAction = {
       *                   text   : 'Cool action',
       *                   icon   : 'b-fa b-fa-fw b-fa-ban',
       *                   onItem : () => console.log('Some coolness'),
       *                   weight : 300 // Move to end
       *               };
       *
       *               // Hide zoom slider
       *               items.zoomLevel.hidden = true;
       *           }
       *       }
       *   },
       * ```
       *
       * @param {Object} context An object with information about the menu being shown
       * @param {Object<String,MenuItemConfig>} context.items An object containing the {@link Core.widget.MenuItem menu item} configs keyed by their id
       * @param {Event} context.event The DOM event object that triggered the show
       * @config {Function}
       * @preventable
       */
      processItems: null,
      /**
       * This is a preconfigured set of items used to create the default context menu.
       *
       * The `items` provided by this feature are listed in the intro section of this class. You can
       * configure existing items by passing a configuration object to the keyed items.
       *
       * To remove existing items, set corresponding keys `null`:
       *
       * ```javascript
       * const scheduler = new Scheduler({
       *     features : {
       *         timeAxisHeaderMenu : {
       *             items : {
       *                 eventsFilter : null
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
      type: 'timeAxisHeader'
    };
  }
  static get pluginConfig() {
    const config = super.pluginConfig;
    config.chain.push('populateTimeAxisHeaderMenu');
    return config;
  }
  //endregion
  //region Events
  /**
   * This event fires on the owning Scheduler before the context menu is shown for the time axis header.
   * Allows manipulation of the items to show in the same way as in the {@link #config-processItems}.
   *
   * Returning `false` from a listener prevents the menu from being shown.
   *
   * @event timeAxisHeaderMenuBeforeShow
   * @on-owner
   * @preventable
   * @param {Scheduler.view.Scheduler} source The scheduler
   * @param {Core.widget.Menu} menu The menu
   * @param {Object<String,MenuItemConfig>} items Menu item configs
   * @param {Grid.column.Column} column Time axis column
   */
  /**
   * This event fires on the owning Scheduler after the context menu is shown for a header
   * @event timeAxisHeaderMenuShow
   * @on-owner
   * @param {Scheduler.view.Scheduler} source The scheduler
   * @param {Core.widget.Menu} menu The menu
   * @param {Object<String,MenuItemConfig>} items Menu item configs
   * @param {Grid.column.Column} column Time axis column
   */
  /**
   * This event fires on the owning Scheduler when an item is selected in the header context menu.
   * @event timeAxisHeaderMenuItem
   * @on-owner
   * @param {Scheduler.view.Scheduler} source The scheduler
   * @param {Core.widget.Menu} menu The menu
   * @param {Core.widget.MenuItem} item Selected menu item
   * @param {Grid.column.Column} column Time axis column
   */
  //endregion
  construct() {
    super.construct(...arguments);
    if (this.triggerEvent.includes('click') && this.client.zoomOnTimeAxisDoubleClick) {
      this.client.zoomOnTimeAxisDoubleClick = false;
    }
  }
  shouldShowMenu(eventParams) {
    const {
        column,
        targetElement
      } = eventParams,
      {
        client
      } = this;
    if (client.isHorizontal) {
      return (column === null || column === void 0 ? void 0 : column.enableHeaderContextMenu) !== false && (column === null || column === void 0 ? void 0 : column.isTimeAxisColumn);
    }
    return targetElement.matches('.b-sch-header-timeaxis-cell');
  }
  showContextMenu(eventParams) {
    super.showContextMenu(...arguments);
    if (this.menu) {
      // the TimeAxis's context menu probably will cause scrolls because it manipulates the dates.
      // The menu should not hide on scroll when for a TimeAxisColumn
      this.menu.scrollAction = 'realign';
    }
  }
  populateTimeAxisHeaderMenu({
    items
  }) {
    const me = this,
      {
        client
      } = me,
      dateStep = {
        magnitude: client.timeAxis.shiftIncrement,
        unit: client.timeAxis.shiftUnit
      };
    Object.assign(items, {
      zoomLevel: {
        text: 'L{pickZoomLevel}',
        localeClass: me,
        icon: 'b-fw-icon b-icon-search-plus',
        disabled: !client.presets.count || me.disabled,
        weight: 200,
        menu: {
          type: 'popup',
          items: {
            zoomSlider: {
              weight: 210,
              type: 'slider',
              minWidth: 130,
              showValue: false,
              // so that we can use the change event which is easier to inject in tests
              triggerChangeOnInput: true
            }
          },
          onBeforeShow({
            source: menu
          }) {
            const [zoom] = menu.items;
            zoom.min = client.minZoomLevel;
            zoom.max = client.maxZoomLevel;
            zoom.value = client.zoomLevel;
            // Default slider value is 50 which causes the above to trigger onZoomSliderChange (when
            // maxZoomLevel < 50) if we add our listener prior to this point.
            me.zoomDetatcher = zoom.ion({
              change: 'onZoomSliderChange',
              thisObj: me
            });
          },
          onHide() {
            if (me.zoomDetatcher) {
              me.zoomDetatcher();
              me.zoomDetatcher = null;
            }
          }
        }
      },
      dateRange: {
        text: 'L{activeDateRange}',
        localeClass: me,
        icon: 'b-fw-icon b-icon-calendar',
        weight: 300,
        menu: {
          type: 'popup',
          cls: 'b-sch-timeaxis-menu-daterange-popup',
          defaults: {
            localeClass: me
          },
          items: {
            startDateField: {
              type: 'datefield',
              label: 'L{startText}',
              weight: 310,
              labelWidth: '6em',
              required: true,
              step: dateStep,
              internalListeners: {
                change: me.onRangeDateFieldChange,
                thisObj: me
              }
            },
            endDateField: {
              type: 'datefield',
              label: 'L{endText}',
              weight: 320,
              labelWidth: '6em',
              required: true,
              step: dateStep,
              internalListeners: {
                change: me.onRangeDateFieldChange,
                thisObj: me
              }
            },
            leftShiftBtn: {
              type: 'button',
              weight: 330,
              cls: 'b-left-nav-btn',
              icon: 'b-icon b-icon-previous',
              color: 'b-blue b-raised',
              flex: 1,
              margin: 0,
              internalListeners: {
                click: me.onLeftShiftBtnClick,
                thisObj: me
              }
            },
            todayBtn: {
              type: 'button',
              weight: 340,
              cls: 'b-today-nav-btn',
              color: 'b-blue b-raised',
              text: 'L{todayText}',
              flex: 4,
              margin: '0 8',
              internalListeners: {
                click: me.onTodayBtnClick,
                thisObj: me
              }
            },
            rightShiftBtn: {
              type: 'button',
              weight: 350,
              cls: 'b-right-nav-btn',
              icon: 'b-icon b-icon-next',
              color: 'b-blue b-raised',
              flex: 1,
              internalListeners: {
                click: me.onRightShiftBtnClick,
                thisObj: me
              }
            }
          },
          internalListeners: {
            paint: me.initDateRangeFields,
            thisObj: me
          }
        }
      }
    });
  }
  onZoomSliderChange({
    value
  }) {
    const me = this;
    // Zooming maintains timeline center point by scrolling the newly rerendered timeline to the
    // correct point to maintain the visual center. Temporarily inhibit context menu hide on scroll
    // of its context element.
    me.menu.scrollAction = 'realign';
    me.client.zoomLevel = value;
    me.menu.setTimeout({
      fn: () => me.menu.scrollAction = 'hide',
      delay: 100,
      cancelOutstanding: true
    });
  }
  initDateRangeFields({
    source: dateRange,
    firstPaint
  }) {
    if (firstPaint) {
      const {
        widgetMap
      } = dateRange;
      this.startDateField = widgetMap.startDateField;
      this.endDateField = widgetMap.endDateField;
    }
    this.initDates();
  }
  initDates() {
    const me = this;
    me.startDateField.suspendEvents();
    me.endDateField.suspendEvents();
    // The actual scheduler start dates may include time, but our Date field cannot currently handle
    // a time portion and throws it away, so when we need the value from an unchanged field, we need
    // to use the initialValue set from the timeAxis values.
    // Until our DateField can optionally include a time value, this is the solution.
    me.startDateField.value = me.startDateFieldInitialValue = me.client.startDate;
    me.endDateField.value = me.endDateFieldInitialValue = me.client.endDate;
    me.startDateField.resumeEvents();
    me.endDateField.resumeEvents();
  }
  onRangeDateFieldChange({
    source
  }) {
    const me = this,
      startDateChanged = source === me.startDateField,
      {
        client
      } = me,
      {
        timeAxis
      } = client,
      startDate = me.startDateFieldInitialValue && !startDateChanged ? me.startDateFieldInitialValue : me.startDateField.value;
    let endDate = me.endDateFieldInitialValue && startDateChanged ? me.endDateFieldInitialValue : me.endDateField.value;
    // When either of the fields is changed, we no longer use its initialValue from the timeAxis start or end
    // so that gets nulled to indicate that it's unavailable and the real field value is to be used.
    if (startDateChanged) {
      me.startDateFieldInitialValue = null;
    } else {
      me.endDateFieldInitialValue = null;
    }
    // Because the start and end dates are exclusive, avoid a zero
    // length time axis by incrementing the end by one tick unit
    // if they are the same.
    if (!(endDate - startDate)) {
      endDate = DateHelper.add(endDate, timeAxis.shiftIncrement, timeAxis.shiftUnit);
    }
    // if start date got bigger than end date set end date to start date plus one tick
    else if (endDate < startDate) {
      endDate = DateHelper.add(startDate, timeAxis.shiftIncrement, timeAxis.shiftUnit);
    }
    // setTimeSpan will try to keep the scroll position the same.
    client.setTimeSpan(startDate, endDate, setTimeSpanOptions);
    me.initDates();
  }
  onLeftShiftBtnClick() {
    this.client.timeAxis.shiftPrevious();
    this.initDates();
  }
  onTodayBtnClick() {
    const today = DateHelper.clearTime(new Date());
    this.client.setTimeSpan(today, DateHelper.add(today, 1, 'day'));
    this.initDates();
  }
  onRightShiftBtnClick() {
    this.client.timeAxis.shiftNext();
    this.initDates();
  }
}
TimeAxisHeaderMenu._$name = 'TimeAxisHeaderMenu';
GridFeatureManager.registerFeature(TimeAxisHeaderMenu, true, ['Scheduler', 'Gantt']);
GridFeatureManager.registerFeature(TimeAxisHeaderMenu, false, 'ResourceHistogram');

/**
 * @module Scheduler/view/model/TimeAxisViewModel
 */
/**
 * This class is an internal view model class, describing the visual representation of a {@link Scheduler.data.TimeAxis}.
 * The config for the header rows is described in the {@link Scheduler.preset.ViewPreset#field-headers headers}.
 * To calculate the size of each cell in the time axis, this class requires:
 *
 * - availableSpace  - The total width or height available for the rendering
 * - tickSize       - The fixed width or height of each cell in the lowest header row. This value is normally read from the
 * {@link Scheduler.preset.ViewPreset viewPreset} but this can also be updated programmatically using the {@link #property-tickSize} setter
 *
 * Normally you should not interact with this class directly.
 *
 * @extends Core/mixin/Events
 */
class TimeAxisViewModel extends Events() {
  //region Default config
  static get defaultConfig() {
    return {
      /**
       * The time axis providing the underlying data to be visualized
       * @config {Scheduler.data.TimeAxis}
       * @internal
       */
      timeAxis: null,
      /**
       * The available width/height, this is normally not known by the consuming UI component using this model
       * class until it has been fully rendered. The consumer of this model should set
       * {@link #property-availableSpace} when its width has changed.
       * @config {Number}
       * @internal
       */
      availableSpace: null,
      /**
       * The "tick width" for horizontal mode or "tick height" for vertical mode, to use for the cells in the
       * bottom most header row.
       * This value is normally read from the {@link Scheduler.preset.ViewPreset viewPreset}
       * @config {Number}
       * @default
       * @internal
       */
      tickSize: 100,
      /**
       * true if there is a requirement to be able to snap events to a certain view resolution.
       * This has implications of the {@link #config-tickSize} that can be used, since all widths must be in even pixels.
       * @config {Boolean}
       * @default
       * @internal
       */
      snap: false,
      /**
       * true if cells in the bottom-most row should be fitted to the {@link #property-availableSpace available space}.
       * @config {Boolean}
       * @default
       * @internal
       */
      forceFit: false,
      headers: null,
      mode: 'horizontal',
      // or 'vertical'
      //used for Exporting. Make sure the tick columns are not recalculated when resizing.
      suppressFit: false,
      // cache of the config currently used.
      columnConfig: [],
      // the view preset name to apply initially
      viewPreset: null,
      // The default header level to draw column lines for
      columnLinesFor: null,
      originalTickSize: null,
      headersDatesCache: []
    };
  }
  //endregion
  //region Init & destroy
  construct(config) {
    const me = this;
    // getSingleUnitInPixels results are memoized because of frequent calls during rendering.
    me.unitToPixelsCache = {};
    super.construct(config);
    const viewPreset = me.timeAxis.viewPreset || me.viewPreset;
    if (viewPreset) {
      if (viewPreset instanceof ViewPreset) {
        me.consumeViewPreset(viewPreset);
      } else {
        const preset = pm.getPreset(viewPreset);
        preset && me.consumeViewPreset(preset);
      }
    }
    // When time axis is changed, reconfigure the model
    me.timeAxis.ion({
      reconfigure: 'onTimeAxisReconfigure',
      thisObj: me
    });
    me.configured = true;
  }
  doDestroy() {
    this.timeAxis.un('reconfigure', this.onTimeAxisReconfigure, this);
    super.doDestroy();
  }
  /**
   * Used to calculate the range to extend the TimeAxis to during infinite scroll.
   * @param {Date} date
   * @param {Boolean} centered
   * @param {Scheduler.preset.ViewPreset} [preset] Optional, the preset for which to calculate the range.
   * defaults to the currently active ViewPreset
   * @returns {Object} `{ startDate, endDate }`
   * @internal
   */
  calculateInfiniteScrollingDateRange(date, centered, preset = this.viewPreset) {
    const {
        timeAxis,
        availableSpace
      } = this,
      {
        bufferCoef
      } = this.owner,
      {
        leafUnit,
        leafIncrement,
        topUnit,
        topIncrement,
        tickSize
      } = preset,
      // If the units are the same and the increments are integer, snap to the top header's unit & increment
      useTop = leafUnit === topUnit && Math.round(topIncrement) === topIncrement && Math.round(leafIncrement) === leafIncrement,
      snapSize = useTop ? topIncrement : leafIncrement,
      snapUnit = useTop ? topUnit : leafUnit;
    // if provided date is the central point on the timespan
    if (centered) {
      const halfSpan = Math.ceil((availableSpace * bufferCoef + availableSpace / 2) / tickSize);
      return {
        startDate: timeAxis.floorDate(DateHelper.add(date, -halfSpan * leafIncrement, leafUnit), false, snapUnit, snapSize),
        endDate: timeAxis.ceilDate(DateHelper.add(date, halfSpan * leafIncrement, leafUnit), false, snapUnit, snapSize)
      };
    }
    // if provided date is the left coordinate of the visible timespan area
    else {
      const bufferedTicks = Math.ceil(availableSpace * bufferCoef / tickSize);
      return {
        startDate: timeAxis.floorDate(DateHelper.add(date, -bufferedTicks * leafIncrement, leafUnit), false, snapUnit, snapSize),
        endDate: timeAxis.ceilDate(DateHelper.add(date, Math.ceil((availableSpace / tickSize + bufferedTicks) * leafIncrement), leafUnit), false, snapUnit, snapSize)
      };
    }
  }
  /**
   * Returns an array representing the headers of the current timeAxis. Each element is an array representing the cells for that level in the header.
   * @returns {Object[]} An array of headers, each element being an array representing each cell (with start date and end date) in the timeline representation.
   * @internal
   */
  get columnConfig() {
    return this._columnConfig;
  }
  set columnConfig(config) {
    this._columnConfig = config;
  }
  get headers() {
    return this._headers;
  }
  set headers(headers) {
    if (headers && headers.length && headers[headers.length - 1].cellGenerator) {
      throw new Error('`cellGenerator` cannot be used for the bottom level of your headers. Use TimeAxis#generateTicks() instead.');
    }
    this._headers = headers;
  }
  get isTimeAxisViewModel() {
    return true;
  }
  //endregion
  //region Events
  /**
   * Fires after the model has been updated.
   * @event update
   * @param {Scheduler.view.model.TimeAxisViewModel} source The model instance
   */
  /**
   * Fires after the model has been reconfigured.
   * @event reconfigure
   * @param {Scheduler.view.model.TimeAxisViewModel} source The model instance
   */
  //endregion
  //region Mode
  /**
   * Using horizontal mode?
   * @returns {Boolean}
   * @readonly
   * @internal
   */
  get isHorizontal() {
    return this.mode !== 'vertical';
  }
  /**
   * Using vertical mode?
   * @returns {Boolean}
   * @readonly
   * @internal
   */
  get isVertical() {
    return this.mode === 'vertical';
  }
  /**
   * Gets/sets the forceFit value for the model. Setting it will cause it to update its contents and fire the
   * {@link #event-update} event.
   * @property {Boolean}
   * @internal
   */
  set forceFit(value) {
    if (value !== this._forceFit) {
      this._forceFit = value;
      this.update();
    }
  }
  //endregion
  //region Reconfigure & update
  reconfigure(config) {
    // clear the cached headers
    this.headers = null;
    // Ensure correct ordering
    this.setConfig(config);
    this.trigger('reconfigure');
  }
  onTimeAxisReconfigure({
    source: timeAxis,
    suppressRefresh
  }) {
    if (this.viewPreset !== timeAxis.viewPreset) {
      this.consumeViewPreset(timeAxis.viewPreset);
    }
    if (!suppressRefresh && timeAxis.count > 0) {
      this.update();
    }
  }
  /**
   * Updates the view model current timeAxis configuration and available space.
   * @param {Number} [availableSpace] The available space for the rendering of the axis (used in forceFit mode)
   * @param {Boolean} [silent] Pass `true` to suppress the firing of the `update` event.
   * @param {Boolean} [forceUpdate] Pass `true` to fire the `update` event even if the size has not changed.
   * @internal
   */
  update(availableSpace, silent = false, forceUpdate = false) {
    const me = this,
      {
        timeAxis,
        headers
      } = me,
      spaceAvailable = availableSpace !== 0;
    // We're in configuration, or no change, quit
    if (me.isConfiguring || spaceAvailable && me._availableSpace === availableSpace) {
      if (forceUpdate) {
        me.trigger('update');
      }
      return;
    }
    me._availableSpace = Math.max(availableSpace || me.availableSpace || 0, 0);
    if (typeof me.availableSpace !== 'number') {
      throw new Error('Invalid available space provided to TimeAxisModel');
    }
    me.columnConfig = [];
    // The "column width" is considered to be the width of each tick in the lowest header row and this width
    // has to be same for all cells in the lowest row.
    const tickSize = me._tickSize = me.calculateTickSize(me.originalTickSize);
    if (typeof tickSize !== 'number' || tickSize <= 0) {
      throw new Error('Invalid timeAxis tick size');
    }
    // getSingleUnitInPixels results are memoized because of frequent calls during rendering.
    me.unitToPixelsCache = {};
    // totalSize is cached because of frequent calls which calculate it.
    me._totalSize = null;
    // Generate the underlying date ranges for each header row, which will provide input to the cell rendering
    for (let pos = 0, {
        length
      } = headers; pos < length; pos++) {
      const header = headers[pos];
      if (header.cellGenerator) {
        const headerCells = header.cellGenerator.call(me, timeAxis.startDate, timeAxis.endDate);
        me.columnConfig[pos] = me.createHeaderRow(pos, header, headerCells);
      } else {
        me.columnConfig[pos] = me.createHeaderRow(pos, header);
      }
    }
    if (!silent) {
      me.trigger('update');
    }
  }
  //endregion
  //region Date / position mapping
  /**
   * Returns the distance in pixels for a timespan with the given start and end date.
   * @param {Date} start start date
   * @param {Date} end end date
   * @returns {Number} The length of the time span
   * @category Date mapping
   */
  getDistanceBetweenDates(start, end) {
    return this.getPositionFromDate(end) - this.getPositionFromDate(start);
  }
  /**
   * Returns the distance in pixels for a time span
   * @param {Number} durationMS Time span duration in ms
   * @returns {Number} The length of the time span
   * @category Date mapping
   */
  getDistanceForDuration(durationMs) {
    return this.getSingleUnitInPixels('millisecond') * durationMs;
  }
  /**
   * Gets the position of a date on the projected time axis or -1 if the date is not in the timeAxis.
   * @param {Date} date the date to query for.
   * @returns {Number} the coordinate representing the date
   * @category Date mapping
   */
  getPositionFromDate(date, options = {}) {
    const tick = this.getScaledTick(date, options);
    if (tick === -1) {
      return -1;
    }
    return this.tickSize * (tick - this.timeAxis.visibleTickStart);
  }
  // Translates a tick along the time axis to facilitate scaling events when excluding certain days or hours
  getScaledTick(date, {
    respectExclusion,
    snapToNextIncluded,
    isEnd,
    min,
    max
  }) {
    const {
        timeAxis
      } = this,
      {
        include,
        unit
      } = timeAxis;
    let tick = timeAxis.getTickFromDate(date);
    if (tick !== -1 && respectExclusion && include) {
      let tickChanged = false;
      // Stretch if we are using a larger unit than 'hour', except if it is 'day'. If so, it is already handled
      // by a cheaper reconfiguration of the ticks in `generateTicks`
      if (include.hour && DateHelper.compareUnits(unit, 'hour') > 0 && unit !== 'day') {
        const {
            from,
            to,
            lengthFactor,
            center
          } = include.hour,
          // Original hours
          originalHours = date.getHours(),
          // Crop to included hours
          croppedHours = Math.min(Math.max(originalHours, from), to);
        // If we are not asked to snap (when other part of span is not included) any cropped away hour
        // should be considered excluded
        if (!snapToNextIncluded && croppedHours !== originalHours) {
          return -1;
        }
        const
          // Should scale hour and smaller units (seconds will hardly affect visible result...)
          fractionalHours = croppedHours + date.getMinutes() / 60,
          // Number of hours from the center    |xxxx|123c----|xxx|
          hoursFromCenter = center - fractionalHours,
          // Step from center to stretch event  |x|112233c----|xxx|
          newHours = center - hoursFromCenter * lengthFactor;
        // Adding instead of setting to get a clone of the date, to not affect the original
        date = DateHelper.add(date, newHours - originalHours, 'h');
        tickChanged = true;
      }
      if (include.day && DateHelper.compareUnits(unit, 'day') > 0) {
        const {
          from,
          to,
          lengthFactor,
          center
        } = include.day;
        //region Crop
        let checkDay = date.getDay();
        // End date is exclusive, check the day before if at 00:00
        if (isEnd && date.getHours() === 0 && date.getMinutes() === 0 && date.getSeconds() === 0 && date.getMilliseconds() === 0) {
          if (--checkDay < 0) {
            checkDay = 6;
          }
        }
        let addDays = 0;
        if (checkDay < from || checkDay >= to) {
          // If end date is in view but start date is excluded, snap to next included day
          if (snapToNextIncluded) {
            // Step back to "to-1" (not inclusive) for end date
            if (isEnd) {
              addDays = (to - checkDay - 8) % 7;
            }
            // Step forward to "from" for start date
            else {
              addDays = (from - checkDay + 7) % 7;
            }
            date = DateHelper.add(date, addDays, 'd');
            date = DateHelper.startOf(date, 'd', false);
            // Keep end after start and vice versa
            if (max && date.getTime() >= max || min && date.getTime() <= min) {
              return -1;
            }
          } else {
            // day excluded at not snapping to next
            return -1;
          }
        }
        //endregion
        const {
            weekStartDay
          } = timeAxis,
          // Center to stretch around, for some reason pre-calculated cannot be used for sundays :)
          fixedCenter = date.getDay() === 0 ? 0 : center,
          // Should scale day and smaller units (minutes will hardly affect visible result...)
          fractionalDay = date.getDay() + date.getHours() / 24,
          //+ dateClone.getMinutes() / (24 * 1440),
          // Number of days from the calculated center
          daysFromCenter = fixedCenter - fractionalDay,
          // Step from center to stretch event
          newDay = fixedCenter - daysFromCenter * lengthFactor;
        // Adding instead of setting to get a clone of the date, to not affect the original
        date = DateHelper.add(date, newDay - fractionalDay + weekStartDay, 'd');
        tickChanged = true;
      }
      // Now the date might start somewhere else (fraction of ticks)
      if (tickChanged) {
        // When stretching date might end up outside of time axis, making it invalid to use. Clip it to time axis
        // to circumvent this
        date = DateHelper.constrain(date, timeAxis.startDate, timeAxis.endDate);
        // Get a new tick based on the "scaled" date
        tick = timeAxis.getTickFromDate(date);
      }
    }
    return tick;
  }
  /**
   * Gets the date for a position on the time axis
   * @param {Number} position The page X or Y coordinate
   * @param {'floor'|'round'|'ceil'} [roundingMethod] Rounding method to use. 'floor' to take the tick (lowest header
   * in a time axis) start date, 'round' to round the value to nearest increment or 'ceil' to take the tick end date
   * @param {Boolean} [allowOutOfRange=false] By default, this returns `null` if the position is outside
   * of the time axis. Pass `true` to attempt to calculate a date outside of the time axis.
   * @returns {Date} the Date corresponding to the xy coordinate
   * @category Date mapping
   */
  getDateFromPosition(position, roundingMethod, allowOutOfRange = false) {
    const me = this,
      {
        timeAxis
      } = me,
      tick = me.getScaledPosition(position) / me.tickSize + timeAxis.visibleTickStart;
    if (tick < 0 || tick > timeAxis.count) {
      if (allowOutOfRange) {
        let result;
        // Subtract the correct number of tick units from the start date
        if (tick < 0) {
          result = DateHelper.add(timeAxis.startDate, tick, timeAxis.unit);
        } else {
          // Add the correct number of tick units to the end date
          result = DateHelper.add(timeAxis.endDate, tick - timeAxis.count, timeAxis.unit);
        }
        // Honour the rounding requested
        if (roundingMethod) {
          result = timeAxis[roundingMethod + 'Date'](result);
        }
        return result;
      }
      return null;
    }
    return timeAxis.getDateFromTick(tick, roundingMethod);
  }
  // Translates a position along the time axis to facilitate scaling events when excluding certain days or hours
  getScaledPosition(position) {
    const {
      include,
      unit,
      weekStartDay
    } = this.timeAxis;
    // Calculations are
    if (include) {
      const dayWidth = this.getSingleUnitInPixels('day');
      // Have to calculate day before hour to get end result correct
      if (include.day && DateHelper.compareUnits(unit, 'day') > 0) {
        const {
            from,
            lengthFactor
          } = include.day,
          // Scaling happens within a week, determine position within it
          positionInWeek = position % (dayWidth * 7),
          // Store were the week starts to be able to re-add it after scale
          weekStartPosition = position - positionInWeek;
        // Scale position using calculated length per day factor, adding the width of excluded days
        position = positionInWeek / lengthFactor + (from - weekStartDay) * dayWidth + weekStartPosition;
      }
      // Hours are not taken into account when viewing days, since the day ticks are reconfigured in
      // `generateTicks` instead
      if (include.hour && DateHelper.compareUnits(unit, 'hour') > 0 && unit !== 'day') {
        const {
            from,
            lengthFactorExcl
          } = include.hour,
          hourWidth = this.getSingleUnitInPixels('hour'),
          // Scaling happens within a day, determine position within it
          positionInDay = position % dayWidth,
          // Store were the day starts to be able to re-add it after scale
          dayStartPosition = position - positionInDay;
        // Scale position using calculated length per day factor, adding the width of excluded hours
        position = positionInDay / lengthFactorExcl + from * hourWidth + dayStartPosition;
      }
    }
    return position;
  }
  /**
   * Returns the amount of pixels for a single unit
   * @internal
   * @returns {Number} The unit in pixel
   */
  getSingleUnitInPixels(unit) {
    const me = this;
    return me.unitToPixelsCache[unit] || (me.unitToPixelsCache[unit] = DateHelper.getUnitToBaseUnitRatio(me.timeAxis.unit, unit, true) * me.tickSize / me.timeAxis.increment);
  }
  /**
   * Returns the pixel increment for the current view resolution.
   * @internal
   * @returns {Number} The increment
   */
  get snapPixelAmount() {
    if (this.snap) {
      const {
        resolution
      } = this.timeAxis;
      return (resolution.increment || 1) * this.getSingleUnitInPixels(resolution.unit);
    }
    return 1;
  }
  //endregion
  //region Sizes
  /**
   * Get/set the current time column size (the width or height of a cell in the bottom-most time axis header row,
   * depending on mode)
   * @internal
   * @property {Number}
   */
  get tickSize() {
    return this._tickSize;
  }
  set tickSize(size) {
    this.setTickSize(size, false);
  }
  setTickSize(size, suppressEvent) {
    this._tickSize = this.originalTickSize = size;
    this.update(undefined, suppressEvent);
  }
  get timeResolution() {
    return this.timeAxis.resolution;
  }
  // Calculates the time column width/height based on the value defined viewPreset "tickWidth/tickHeight". It also
  // checks for the forceFit view option and the snap, both of which impose constraints on the time column width
  // configuration.
  calculateTickSize(proposedSize) {
    const me = this,
      {
        forceFit,
        timeAxis,
        suppressFit
      } = me,
      timelineUnit = timeAxis.unit;
    let size = 0,
      ratio = 1; //Number.MAX_VALUE;
    if (me.snap) {
      const resolution = timeAxis.resolution;
      ratio = DateHelper.getUnitToBaseUnitRatio(timelineUnit, resolution.unit) * resolution.increment;
    }
    if (!suppressFit) {
      const fittingSize = me.availableSpace / timeAxis.visibleTickTimeSpan;
      size = forceFit || proposedSize < fittingSize ? fittingSize : proposedSize;
      if (ratio > 0 && (!forceFit || ratio < 1)) {
        size = Math.max(1, ratio * size) / ratio;
      }
    } else {
      size = proposedSize;
    }
    return size;
  }
  /**
   * Returns the total width/height of the time axis representation, depending on mode.
   * @returns {Number} The width or height
   * @internal
   * @readonly
   */
  get totalSize() {
    // Floor the space to prevent spurious overflow
    return this._totalSize || (this._totalSize = Math.floor(this.tickSize * this.timeAxis.visibleTickTimeSpan));
  }
  /**
   * Get/set the available space for the time axis representation. If size changes it will cause it to update its
   * contents and fire the {@link #event-update} event.
   * @internal
   * @property {Number}
   */
  get availableSpace() {
    return this._availableSpace;
  }
  set availableSpace(space) {
    const me = this;
    // We should only need to repaint fully if the tick width has changed (which will happen if forceFit is set, or if the full size of the time axis doesn't
    // occupy the available space - and gets stretched
    me._availableSpace = Math.max(0, space);
    if (me._availableSpace > 0) {
      const newTickSize = me.calculateTickSize(me.originalTickSize);
      if (newTickSize > 0 && newTickSize !== me.tickSize) {
        me.update();
      }
    }
  }
  //endregion
  //region Fitting & snapping
  /**
   * Returns start dates for ticks at the specified level in format { date, isMajor }.
   * @param {Number} level Level in headers array, `0` meaning the topmost...
   * @param {Boolean} useLowestHeader Use lowest level
   * @param getEnd
   * @returns {Array}
   * @internal
   */
  getDates(level = this.columnLinesFor, useLowestHeader = false, getEnd = false) {
    const me = this,
      ticks = [],
      linesForLevel = useLowestHeader ? me.lowestHeader : level,
      majorLevel = me.majorHeaderLevel,
      levelUnit = me.headers && me.headers[level].unit,
      majorUnit = majorLevel != null && me.headers && me.headers[majorLevel].unit,
      validMajor = majorLevel != null && DateHelper.doesUnitsAlign(majorUnit, levelUnit),
      hasGenerator = !!(me.headers && me.headers[linesForLevel].cellGenerator);
    if (hasGenerator) {
      const cells = me.columnConfig[linesForLevel];
      for (let i = 1, l = cells.length; i < l; i++) {
        ticks.push({
          date: cells[i].startDate
        });
      }
    } else {
      me.forEachInterval(linesForLevel, (start, end) => {
        ticks.push({
          date: getEnd ? end : start,
          // do not want to consider tick to be major tick, hence the check for majorHeaderLevel
          isMajor: majorLevel !== level && validMajor && me.isMajorTick(getEnd ? end : start)
        });
      });
    }
    return ticks;
  }
  get forceFit() {
    return this._forceFit;
  }
  /**
   * This function fits the time columns into the available space in the time axis column.
   * @param {Boolean} suppressEvent `true` to skip firing the 'update' event.
   * @internal
   */
  fitToAvailableSpace(suppressEvent) {
    const proposedSize = Math.floor(this.availableSpace / this.timeAxis.visibleTickTimeSpan);
    this.setTickSize(proposedSize, suppressEvent);
  }
  get snap() {
    return this._snap;
  }
  /**
   * Gets/sets the snap value for the model. Setting it will cause it to update its contents and fire the
   * {@link #event-update} event.
   * @property {Boolean}
   * @internal
   */
  set snap(value) {
    if (value !== this._snap) {
      this._snap = value;
      if (this.configured) {
        this.update();
      }
    }
  }
  //endregion
  //region Headers
  // private
  createHeaderRow(position, headerRowConfig, headerCells) {
    const me = this,
      cells = [],
      {
        align,
        headerCellCls = ''
      } = headerRowConfig,
      today = DateHelper.clearTime(new Date()),
      {
        timeAxis
      } = me,
      tickLevel = me.headers.length - 1,
      createCellContext = (start, end, i, isLast, data) => {
        let value = DateHelper.format(start, headerRowConfig.dateFormat);
        const
          // So that we can use shortcut tickSize as the tickLevel cell width.
          // We can do this if the TimeAxis is aligned to start and end on tick boundaries
          // or if it's not the first or last tick.
          // getDistanceBetweenDates is an expensive operation.
          isInteriorTick = i > 0 && !isLast,
          cellData = {
            align,
            start,
            end,
            value: data ? data.header : value,
            headerCellCls,
            width: tickLevel === position && me.owner && (timeAxis.fullTicks || isInteriorTick) ? me.owner.tickSize : me.getDistanceBetweenDates(start, end),
            index: i
          };
        if (cellData.width === 0) {
          return;
        }
        // Vertical mode uses absolute positioning for header cells
        cellData.coord = size - 1;
        size += cellData.width;
        me.headersDatesCache[position][start.getTime()] = 1;
        if (headerRowConfig.renderer) {
          value = headerRowConfig.renderer.call(headerRowConfig.thisObj || me, start, end, cellData, i);
          cellData.value = value == null ? '' : value;
        }
        // To be able to style individual day cells, weekends or other important days
        if (headerRowConfig.unit === 'day' && (!headerRowConfig.increment || headerRowConfig.increment === 1)) {
          cellData.headerCellCls += ' b-sch-dayheadercell-' + start.getDay();
          if (DateHelper.clearTime(start, true) - today === 0) {
            cellData.headerCellCls += ' b-sch-dayheadercell-today';
          }
        }
        cells.push(cellData);
      };
    let size = 0;
    me.headersDatesCache[position] = {};
    if (headerCells) {
      headerCells.forEach((cellData, i) => createCellContext(cellData.start, cellData.end, i, i === headerCells.length - 1, cellData));
    } else {
      me.forEachInterval(position, createCellContext);
    }
    return cells;
  }
  get mainHeader() {
    return 'mainHeaderLevel' in this ? this.headers[this.mainHeaderLevel] : this.bottomHeader;
  }
  get bottomHeader() {
    return this.headers[this.headers.length - 1];
  }
  get lowestHeader() {
    return this.headers.length - 1;
  }
  /**
   * This method is meant to return the level of the header which 2nd lowest.
   * It is used for {@link #function-isMajorTick} method
   * @returns {String}
   * @private
   */
  get majorHeaderLevel() {
    const {
      headers
    } = this;
    if (headers) {
      return Math.max(headers.length - 2, 0);
    }
    return null;
  }
  //endregion
  //region Ticks
  /**
   * For vertical view (and column lines plugin) we sometimes want to know if current tick starts along with the
   * upper header level.
   * @param {Date} date
   * @returns {Boolean}
   * @private
   */
  isMajorTick(date) {
    const nextLevel = this.majorHeaderLevel;
    // if forceFit is used headersDatesCache won´t have been generated yet on the first call here,
    // since no size is set yet
    return nextLevel != null && this.headersDatesCache[nextLevel] && this.headersDatesCache[nextLevel][date.getTime()] || false;
  }
  /**
   * Calls the supplied iterator function once per interval. The function will be called with three parameters, start date and end date and an index.
   * Return false to break the iteration.
   * @param {Number} position The index of the header in the headers array.
   * @param {Function} iteratorFn The function to call, will be called with start date, end date and "tick index"
   * @param {Object} [thisObj] `this` reference for the function
   * @internal
   */
  forEachInterval(position, iteratorFn, thisObj = this) {
    const {
      headers,
      timeAxis
    } = this;
    if (headers) {
      // This is the lowest header row, which should be fed the data in the tickStore (or a row above using same unit)
      if (position === headers.length - 1) {
        timeAxis.forEach((r, index) => iteratorFn.call(thisObj, r.startDate, r.endDate, index, index === timeAxis.count - 1));
      }
      // All other rows
      else {
        const header = headers[position];
        timeAxis.forEachAuxInterval(header.unit, header.increment, iteratorFn, thisObj);
      }
    }
  }
  /**
   * Calls the supplied iterator function once per interval. The function will be called with three parameters, start date and end date and an index.
   * Return false to break the iteration.
   * @internal
   * @param {Function} iteratorFn The function to call
   * @param {Object} [thisObj] `this` reference for the function
   */
  forEachMainInterval(iteratorFn, thisObj) {
    this.forEachInterval(this.mainHeaderLevel, iteratorFn, thisObj);
  }
  //endregion
  //region ViewPreset
  consumeViewPreset(preset) {
    const me = this;
    // clear the cached headers
    me.headers = null;
    me.getConfig('tickSize');
    // Since we are bypassing the tickSize setter below, ensure that
    // the config initial setter has been removed by referencing the property.
    // We only do this to avoid multiple updates from this.
    me.viewPreset = preset;
    Object.assign(me, {
      headers: preset.headers,
      columnLinesFor: preset.columnLinesFor,
      mainHeaderLevel: preset.mainHeaderLevel,
      _tickSize: me.isHorizontal ? preset.tickWidth : preset.tickHeight
    });
    me.originalTickSize = me.tickSize;
  }
  //endregion
}

TimeAxisViewModel._$name = 'TimeAxisViewModel';

// Used to avoid having to create huge amounts of Date objects
const tempDate = new Date();
/**
 * @module Scheduler/view/mixin/TimelineDateMapper
 */
/**
 * Mixin that contains functionality to convert between coordinates and dates etc.
 *
 * @mixin
 */
var TimelineDateMapper = (Target => class TimelineDateMapper extends (Target || Base$1) {
  static $name = 'TimelineDateMapper';
  static configurable = {
    /**
     * Set to `true` to snap to the current time resolution increment while interacting with scheduled events.
     *
     * The time resolution increment is either determined by the currently applied view preset, or it can be
     * overridden using {@link #property-timeResolution}.
     *
     * <div class="note">When the {@link Scheduler/view/mixin/TimelineEventRendering#config-fillTicks} option is
     * enabled, snapping will align to full ticks, regardless of the time resolution.</div>
     *
     * @prp {Boolean}
     * @default
     * @category Scheduled events
     */
    snap: false
  };
  //region Coordinate <-> Date
  getRtlX(x) {
    if (this.rtl && this.isHorizontal) {
      x = this.timeAxisViewModel.totalSize - x;
    }
    return x;
  }
  /**
   * Gets the date for an X or Y coordinate, either local to the view element or the page based on the 3rd argument.
   * If the coordinate is not in the currently rendered view, null will be returned unless the `allowOutOfRange`
   * parameter is passed a `true`.
   * @param {Number} coordinate The X or Y coordinate
   * @param {'floor'|'round'|'ceil'} [roundingMethod] Rounding method to use. 'floor' to take the tick (lowest header
   * in a time axis) start date, 'round' to round the value to nearest increment or 'ceil' to take the tick end date
   * @param {Boolean} [local] true if the coordinate is local to the scheduler view element
   * @param {Boolean} [allowOutOfRange] By default, this returns `null` if the position is outside
   * of the time axis. Pass `true` to attempt to calculate a date outside of the time axis.
   * @returns {Date} The Date corresponding to the X or Y coordinate
   * @category Dates
   */
  getDateFromCoordinate(coordinate, roundingMethod, local = true, allowOutOfRange = false, ignoreRTL = false) {
    if (!local) {
      coordinate = this.currentOrientation.translateToScheduleCoordinate(coordinate);
    }
    // Time axis is flipped for RTL
    if (!ignoreRTL) {
      coordinate = this.getRtlX(coordinate);
    }
    return this.timeAxisViewModel.getDateFromPosition(coordinate, roundingMethod, allowOutOfRange);
  }
  getDateFromCoord(options) {
    return this.getDateFromCoordinate(options.coord, options.roundingMethod, options.local, options.allowOutOfRange, options.ignoreRTL);
  }
  /**
   * Gets the date for an XY coordinate regardless of the orientation of the time axis.
   * @param {Array} xy The page X and Y coordinates
   * @param {'floor'|'round'|'ceil'} [roundingMethod] Rounding method to use. 'floor' to take the tick (lowest header
   * in a time axis) start date, 'round' to round the value to nearest increment or 'ceil' to take the tick end date
   * @param {Boolean} [local] true if the coordinate is local to the scheduler element
   * @param {Boolean} [allowOutOfRange] By default, this returns `null` if the position is outside
   * of the time axis. Pass `true` to attempt to calculate a date outside of the time axis.
   * @returns {Date} the Date corresponding to the xy coordinate
   * @category Dates
   */
  getDateFromXY(xy, roundingMethod, local = true, allowOutOfRange = false) {
    return this.currentOrientation.getDateFromXY(xy, roundingMethod, local, allowOutOfRange);
  }
  /**
   * Gets the time for a DOM event such as 'mousemove' or 'click' regardless of the orientation of the time axis.
   * @param {Event} e the Event instance
   * @param {'floor'|'round'|'ceil'} [roundingMethod] Rounding method to use. 'floor' to take the tick (lowest header
   * in a time axis) start date, 'round' to round the value to nearest increment or 'ceil' to take the tick end date
   * @param {Boolean} [allowOutOfRange] By default, this returns `null` if the position is outside
   * of the time axis. Pass `true` to attempt to calculate a date outside of the time axis.
   * @returns {Date} The date corresponding to the EventObject's position along the orientation of the time axis.
   * @category Dates
   */
  getDateFromDomEvent(e, roundingMethod, allowOutOfRange = false) {
    return this.getDateFromXY([e.pageX, e.pageY], roundingMethod, false, allowOutOfRange);
  }
  /**
   * Gets the start and end dates for an element Region
   * @param {Core.helper.util.Rectangle} rect The rectangle to map to start and end dates
   * @param {'floor'|'round'|'ceil'} roundingMethod Rounding method to use. 'floor' to take the tick (lowest header
   * in a time axis) start date, 'round' to round the value to nearest increment or 'ceil' to take the tick end date
   * @param {Number} duration The duration in MS of the underlying event
   * @returns {Object} an object containing start/end properties
   */
  getStartEndDatesFromRectangle(rect, roundingMethod, duration, allowOutOfRange = false) {
    const me = this,
      {
        isHorizontal
      } = me,
      startPos = isHorizontal ? rect.x : rect.top,
      endPos = isHorizontal ? rect.right : rect.bottom;
    let start, end;
    // Element within bounds
    if (startPos >= 0 && endPos < me.timeAxisViewModel.totalSize) {
      start = me.getDateFromCoordinate(startPos, roundingMethod, true);
      end = me.getDateFromCoordinate(endPos, roundingMethod, true);
    }
    // Starts before, start is worked backwards from end
    else if (startPos < 0) {
      end = me.getDateFromCoordinate(endPos, roundingMethod, true, allowOutOfRange);
      start = end && DateHelper.add(end, -duration, 'ms');
    }
    // Ends after, end is calculated from the start
    else {
      start = me.getDateFromCoordinate(startPos, roundingMethod, true, allowOutOfRange);
      end = start && DateHelper.add(start, duration, 'ms');
    }
    return {
      start,
      end
    };
  }
  //endregion
  //region Date display
  /**
   * Method to get a displayed end date value, see {@link #function-getFormattedEndDate} for more info.
   * @private
   * @param {Date} endDate The date to format
   * @param {Date} startDate The start date
   * @returns {Date} The date value to display
   */
  getDisplayEndDate(endDate, startDate) {
    if (
    // If time is midnight,
    endDate.getHours() === 0 && endDate.getMinutes() === 0 && (
    // and end date is greater then start date
    !startDate || !(endDate.getYear() === startDate.getYear() && endDate.getMonth() === startDate.getMonth() && endDate.getDate() === startDate.getDate())) &&
    // and UI display format doesn't contain hour info (in this case we'll just display the exact date)
    !DateHelper.formatContainsHourInfo(this.displayDateFormat)) {
      // format the date inclusively as 'the whole previous day'.
      endDate = DateHelper.add(endDate, -1, 'day');
    }
    return endDate;
  }
  /**
   * Method to get a formatted end date for a scheduled event, the grid uses the "displayDateFormat" property defined in the current view preset.
   * End dates are formatted as 'inclusive', meaning when an end date falls on midnight and the date format doesn't involve any hour/minute information,
   * 1ms will be subtracted (e.g. 2010-01-08T00:00:00 will first be modified to 2010-01-07 before being formatted).
   * @private
   * @param {Date} endDate The date to format
   * @param {Date} startDate The start date
   * @returns {String} The formatted date
   */
  getFormattedEndDate(endDate, startDate) {
    return this.getFormattedDate(this.getDisplayEndDate(endDate, startDate));
  }
  //endregion
  //region Other date functions
  /**
   * Gets the x or y coordinate relative to the scheduler element, or page coordinate (based on the 'local' flag)
   * If the coordinate is not in the currently rendered view, -1 will be returned.
   * @param {Date|Number} date the date to query for (or a date as ms)
   * @param {Boolean|Object} options true to return a coordinate local to the scheduler view element (defaults to true),
   * also accepts a config object like { local : true }.
   * @returns {Number} the x or y position representing the date on the time axis
   * @category Dates
   */
  getCoordinateFromDate(date, options = true) {
    const me = this,
      {
        timeAxisViewModel
      } = me,
      {
        isContinuous,
        startMS,
        endMS,
        startDate,
        endDate,
        unit
      } = me.timeAxis,
      dateMS = date.valueOf();
    // Avoiding to break the API while allowing passing options through to getPositionFromDate()
    if (options === true) {
      options = {
        local: true
      };
    } else if (!options) {
      options = {
        local: false
      };
    } else if (!('local' in options)) {
      options.local = true;
    }
    let pos;
    if (!(date instanceof Date)) {
      tempDate.setTime(date);
      date = tempDate;
    }
    // Shortcut for continuous time axis that is using a unit that can be reliably translated to days (or smaller)
    if (isContinuous && date.getTimezoneOffset() === startDate.getTimezoneOffset() && startDate.getTimezoneOffset() === endDate.getTimezoneOffset() && DateHelper.getUnitToBaseUnitRatio(unit, 'day') !== -1) {
      if (dateMS < startMS || dateMS > endMS) {
        return -1;
      }
      pos = (dateMS - startMS) / (endMS - startMS) * timeAxisViewModel.totalSize;
    }
    // Non-continuous or using for example months (vary in length)
    else {
      pos = timeAxisViewModel.getPositionFromDate(date, options);
    }
    // RTL coords from the end of the time axis
    if (me.rtl && me.isHorizontal) {
      pos = timeAxisViewModel.totalSize - pos;
    }
    if (!options.local) {
      pos = me.currentOrientation.translateToPageCoordinate(pos);
    }
    return pos;
  }
  /**
   * Returns the distance in pixels for the time span in the view.
   * @param {Date} startDate The start date of the span
   * @param {Date} endDate The end date of the span
   * @returns {Number} The distance in pixels
   * @category Dates
   */
  getTimeSpanDistance(startDate, endDate) {
    return this.timeAxisViewModel.getDistanceBetweenDates(startDate, endDate);
  }
  /**
   * Returns the center date of the currently visible timespan of scheduler.
   *
   * @property {Date}
   * @readonly
   * @category Dates
   */
  get viewportCenterDate() {
    const {
      timeAxis,
      timelineScroller
    } = this;
    // Take the easy way if the axis is continuous.
    // We can just work out how far along the time axis the viewport center is.
    if (timeAxis.isContinuous) {
      // The offset from the start of the whole time axis
      const timeAxisOffset = (timelineScroller.position + timelineScroller.clientSize / 2) / timelineScroller.scrollSize;
      return new Date(timeAxis.startMS + (timeAxis.endMS - timeAxis.startMS) * timeAxisOffset);
    }
    return this.getDateFromCoordinate(timelineScroller.position + timelineScroller.clientSize / 2);
  }
  get viewportCenterDateCached() {
    return this.cachedCenterDate || (this.cachedCenterDate = this.viewportCenterDate);
  }
  //endregion
  //region TimeAxis getters/setters
  /**
   * Gets/sets the current time resolution object, which contains a unit identifier and an increment count
   * `{ unit, increment }`. This value means minimal task duration you can create using UI.
   *
   * For example when you drag create a task or drag & drop a task, if increment is 5 and unit is 'minute'
   * that means that you can create tasks in 5 minute increments, or move it in 5 minute steps.
   *
   * This value is taken from viewPreset {@link Scheduler.preset.ViewPreset#field-timeResolution timeResolution}
   * config by default. When supplying a `Number` to the setter only the `increment` is changed and the `unit` value
   * remains untouched.
   *
   * ```javascript
   * timeResolution : {
   *   unit      : 'minute',  //Valid values are "millisecond", "second", "minute", "hour", "day", "week", "month", "quarter", "year".
   *   increment : 5
   * }
   * ```
   *
   * <div class="note">When the {@link Scheduler/view/mixin/TimelineEventRendering#config-fillTicks} option is
   * enabled, the resolution will be in full ticks regardless of configured value.</div>
   *
   * @property {Object|Number}
   * @category Dates
   */
  get timeResolution() {
    return this.timeAxis.resolution;
  }
  set timeResolution(resolution) {
    this.timeAxis.resolution = typeof resolution === 'number' ? {
      increment: resolution,
      unit: this.timeAxis.resolution.unit
    } : resolution;
  }
  //endregion
  //region Snap
  get snap() {
    var _this$_timeAxisViewMo;
    return ((_this$_timeAxisViewMo = this._timeAxisViewModel) === null || _this$_timeAxisViewMo === void 0 ? void 0 : _this$_timeAxisViewMo.snap) ?? this._snap;
  }
  updateSnap(snap) {
    if (!this.isConfiguring) {
      this.timeAxisViewModel.snap = snap;
      this.timeAxis.forceFullTicks = snap && this.fillTicks;
    }
  }
  //endregion
  onSchedulerHorizontalScroll({
    subGrid,
    scrollLeft,
    scrollX
  }) {
    // Invalidate cached center date unless we are scrolling to center on it.
    if (!this.scrollingToCenter) {
      this.cachedCenterDate = null;
    }
  }
  // This does not need a className on Widgets.
  // Each *Class* which doesn't need 'b-' + constructor.name.toLowerCase() automatically adding
  // to the Widget it's mixed in to should implement thus.
  get widgetClass() {}
});

/**
 * @module Scheduler/view/mixin/TimelineDomEvents
 */
const {
  eventNameMap
} = EventHelper;
/**
 * An object which encapsulates a schedule timeline tick context based on a DOM event. This will include
 * the row and resource information and the tick and time information for a DOM pointer event detected
 * in the timeline.
 * @typedef {Object} TimelineContext
 * @property {Event} domEvent The DOM event which triggered the context change.
 * @property {HTMLElement} eventElement If the `domEvent` was on an event bar, this will be the event bar element.
 * @property {HTMLElement} cellElement The cell element under the `domEvent`
 * @property {Date} date The date corresponding to the `domEvent` position in the timeline
 * @property {Scheduler.model.TimeSpan} tick A {@link Scheduler.model.TimeSpan} record which encapsulates the contextual tick
 * @property {Number} tickIndex The contextual tick index. This may be fractional.
 * @property {Number} tickParentIndex The integer contextual tick index.
 * @property {Date} tickStartDate The start date of the contextual tick.
 * @property {Date} tickEndDate The end date of the contextual tick.
 * @property {Grid.row.Row} row The contextual {@link Grid.row.Row}
 * @property {Number} index The contextual row index
 * @property {Scheduler.model.EventModel} [eventRecord] The contextual event record (if any) if the event source is a `Scheduler`
 * @property {Scheduler.model.AssignmentModel} [assignmentRecord] The contextual assignment record (if any) if the event source is a `Scheduler`
 * @property {Scheduler.model.ResourceModel} [resourceRecord] The contextual resource record(if any)  if the event source is a `Scheduler`
 */
/**
 * Mixin that handles dom events (click etc) for scheduler and rendered events.
 *
 * @mixin
 */
var TimelineDomEvents = (Target => class TimelineDomEvents extends (Target || Base$1) {
  /**
   * Fires after a click on a time axis cell
   * @event timeAxisHeaderClick
   * @param {Scheduler.column.TimeAxisColumn|Scheduler.column.VerticalTimeAxisColumn} source The column object
   * @param {Date} startDate The start date of the header cell
   * @param {Date} endDate The end date of the header cell
   * @param {Event} event The event object
   */
  /**
   * Fires after a double click on a time axis cell
   * @event timeAxisHeaderDblClick
   * @param {Scheduler.column.TimeAxisColumn|Scheduler.column.VerticalTimeAxisColumn} source The column object
   * @param {Date} startDate The start date of the header cell
   * @param {Date} endDate The end date of the header cell
   * @param {Event} event The event object
   */
  /**
   * Fires after a right click on a time axis cell
   * @event timeAxisHeaderContextMenu
   * @param {Scheduler.column.TimeAxisColumn|Scheduler.column.VerticalTimeAxisColumn} source The column object
   * @param {Date} startDate The start date of the header cell
   * @param {Date} endDate The end date of the header cell
   * @param {Event} event The event object
   */
  static $name = 'TimelineDomEvents';
  //region Default config
  static configurable = {
    /**
     * The currently hovered timeline context. This is updated as the mouse or pointer moves over the timeline.
     * @member {TimelineContext} timelineContext
     * @readonly
     * @category Dates
     */
    timelineContext: {
      $config: {
        // Reject non-changes so that when set from scheduleMouseMove and EventMouseMove,
        // we only update the context and fire events when it changes.
        equal(c1, c2) {
          // index is the resource index, tickParentIndex is the
          // tick's index in the TimeAxis.
          return (c1 === null || c1 === void 0 ? void 0 : c1.index) === (c2 === null || c2 === void 0 ? void 0 : c2.index) && (c1 === null || c1 === void 0 ? void 0 : c1.tickParentIndex) === (c2 === null || c2 === void 0 ? void 0 : c2.tickParentIndex) && !(((c1 === null || c1 === void 0 ? void 0 : c1.tickStartDate) || 0) - ((c2 === null || c2 === void 0 ? void 0 : c2.tickStartDate) || 0));
        }
      }
    },
    /**
     * By default, scrolling the schedule will update the {@link #property-timelineContext} to reflect the new
     * currently hovered context. When displaying a large number of events on screen at the same time, this will
     * have a slight impact on scrolling performance. In such scenarios, opt out of this behavior by setting
     * this config to `false`.
     * @default
     * @prp {Boolean}
     * @category Misc
     */
    updateTimelineContextOnScroll: true
  };
  static properties = {
    schedulerEvents: {
      pointermove: 'handleScheduleEvent',
      mouseover: 'handleScheduleEvent',
      mousedown: 'handleScheduleEvent',
      mouseup: 'handleScheduleEvent',
      click: 'handleScheduleEvent',
      dblclick: 'handleScheduleEvent',
      contextmenu: 'handleScheduleEvent',
      mousemove: 'handleScheduleEvent',
      mouseout: 'handleScheduleEvent'
    },
    schedulerEnterLeaveEvents: {
      mouseenter: 'handleScheduleEnterLeaveEvent',
      mouseleave: 'handleScheduleEnterLeaveEvent',
      capture: true
    }
  };
  static delayable = {
    // Allow the scroll event to complete in its thread, and dispatch the mousemove event next AF
    onScheduleScroll: 'raf'
  };
  // Currently hovered events (can be parent + child)
  hoveredEvents = new Set();
  //endregion
  //region Init
  /**
   * Adds listeners for DOM events for the scheduler and its events.
   * Which events is specified in Scheduler#schedulerEvents.
   * @private
   */
  initDomEvents() {
    const me = this,
      {
        schedulerEvents,
        schedulerEnterLeaveEvents
      } = me;
    // Set thisObj and element of the configured listener specs.
    schedulerEvents.element = schedulerEnterLeaveEvents.element = me.timeAxisSubGridElement;
    schedulerEvents.thisObj = schedulerEnterLeaveEvents.thisObj = me;
    EventHelper.on(schedulerEvents);
    EventHelper.on(schedulerEnterLeaveEvents);
    // This is to handle scroll events while the mouse is over the schedule.
    // For example magic mouse or touchpad scrolls, or scrolls caused by keyboard
    // navigation while the mouse happens to be over the schedule.
    // The context must update. We must consider any scroll because the document
    // or some other wrapping element could be scrolling the Scheduler under the mouse.
    if (BrowserHelper.supportsPointerEventConstructor) {
      EventHelper.on({
        element: document,
        scroll: 'onScheduleScroll',
        capture: true,
        thisObj: me
      });
    }
  }
  //endregion
  //region Event handling
  getTimeSpanMouseEventParams(eventElement, event) {
    throw new Error('Implement in subclass');
  }
  getScheduleMouseEventParams(cellData, event) {
    throw new Error('Implement in subclass');
  }
  /**
   * Wraps dom Events for the scheduler and event bars and fires as our events.
   * For example click -> scheduleClick or eventClick
   * @private
   * @param event
   */
  handleScheduleEvent(event) {
    const me = this,
      timelineContext = me.getTimelineEventContext(event);
    // Cache the last pointer event so that  when scrolling below the mouse
    // we can inject mousemove events at that point.
    me.lastPointerEvent = event;
    // We are over the schedule region
    if (timelineContext) {
      // Only fire a scheduleXXXX event if we are *not* over an event.
      // If over an event fire (event|task)XXXX.
      me.trigger(`${timelineContext.eventElement ? me.scheduledEventName : 'schedule'}${eventNameMap[event.type] || StringHelper.capitalize(event.type)}`, timelineContext);
    }
    // If the context has changed, updateTimelineContext will fire events
    me.timelineContext = timelineContext;
  }
  handleScheduleEnterLeaveEvent(event) {
    if (event.target.parentElement === this.foregroundCanvas) {
      this.handleScheduleEvent(event);
    }
  }
  /**
   * This handles the scheduler being scrolled below the mouse by trackpad or keyboard events.
   * The context, if present needs to be recalculated.
   * @private
   */
  onScheduleScroll({
    target
  }) {
    var _me$features$pan;
    const me = this;
    // If the latest mouse event resulted in setting a context, we need to reproduce that event at the same clientX,
    // clientY in order to keep the context up to date while scrolling.
    // If the scroll is because of a pan feature drag (on us or a partner), we must not do this.
    // Target might be removed in salesforce by Locker Service if scroll event occurs on body
    if (target && me.updateTimelineContextOnScroll && !((_me$features$pan = me.features.pan) !== null && _me$features$pan !== void 0 && _me$features$pan.isActive) && !me.partners.some(p => {
      var _p$features$pan;
      return (_p$features$pan = p.features.pan) === null || _p$features$pan === void 0 ? void 0 : _p$features$pan.isActive;
    }) && (target.contains(me.element) || me.bodyElement.contains(target))) {
      const {
        timelineContext,
        lastPointerEvent
      } = me;
      if (timelineContext) {
        var _GlobalEvents$current, _GlobalEvents$current2;
        const targetElement = DomHelper.elementFromPoint(timelineContext.domEvent.clientX, timelineContext.domEvent.clientY),
          pointerEvent = new BrowserHelper.PointerEventConstructor('pointermove', lastPointerEvent),
          mouseEvent = new MouseEvent('mousemove', lastPointerEvent);
        // See https://github.com/bryntum/support/issues/6274
        // The pointerId does not propagate correctly on the synthetic PointerEvent, but also is readonly, so
        // redefine the property. This is required by Ext JS gesture publisher which tracks pointer movements
        // while a pointer is down. Without the correct pointerId, Ext JS would see this move as a "missed"
        // pointerdown and forever await its pointerup (i.e., it would get stuck in the activeTouches). This
        // would cause all future events to be perceived as part of or the end of a drag and would never again
        // dispatch pointer events correctly. Finally, lastPointerEvent.pointerId is often incorrect (undefined
        // in fact), so check the most recent pointerdown/touchstart event and default to 1
        Object.defineProperty(pointerEvent, 'pointerId', {
          value: ((_GlobalEvents$current = GlobalEvents.currentPointerDown) === null || _GlobalEvents$current === void 0 ? void 0 : _GlobalEvents$current.pointerId) ?? ((_GlobalEvents$current2 = GlobalEvents.currentTouch) === null || _GlobalEvents$current2 === void 0 ? void 0 : _GlobalEvents$current2.identifier) ?? 1
        });
        // Drag code should ignore these synthetic events
        pointerEvent.scrollInitiated = mouseEvent.scrollInitiated = true;
        // Emulate the correct browser sequence for mouse move events
        targetElement === null || targetElement === void 0 ? void 0 : targetElement.dispatchEvent(pointerEvent);
        targetElement === null || targetElement === void 0 ? void 0 : targetElement.dispatchEvent(mouseEvent);
      }
    }
  }
  updateTimelineContext(context, oldContext) {
    /**
     * Fired when the pointer-activated {@link #property-timelineContext} has changed.
     * @event timelineContextChange
     * @param {TimelineContext} oldContext The tick/resource context being deactivated.
     * @param {TimelineContext} context The tick/resource context being activated.
     */
    this.trigger('timelineContextChange', {
      oldContext,
      context
    });
    if (!context) {
      this.trigger('scheduleMouseLeave');
    }
  }
  /**
   * Gathers contextual information about the schedule contextual position of the passed event.
   *
   * Used by schedule mouse event handlers, but also by the scheduleContext feature.
   * @param {Event} domEvent The DOM event to gather context for.
   * @returns {TimelineContext} the schedule DOM event context
   * @internal
   */
  getTimelineEventContext(domEvent) {
    const me = this,
      eventElement = domEvent.target.closest(me.eventInnerSelector),
      cellElement = me.getCellElementFromDomEvent(domEvent);
    if (cellElement) {
      const clickedDate = me.getDateFromDomEvent(domEvent, 'floor');
      if (!clickedDate) {
        return;
      }
      const cellData = DomDataStore.get(cellElement),
        mouseParams = eventElement ? me.getTimeSpanMouseEventParams(eventElement, domEvent) : me.getScheduleMouseEventParams(cellData, domEvent);
      if (!mouseParams) {
        return;
      }
      const index = me.isVertical ? me.resourceStore.indexOf(mouseParams.resourceRecord) : cellData.row.dataIndex,
        tickIndex = me.timeAxis.getTickFromDate(clickedDate),
        tick = me.timeAxis.getAt(Math.floor(tickIndex));
      if (tick) {
        return {
          isTimelineContext: true,
          domEvent,
          eventElement,
          cellElement,
          index,
          tick,
          tickIndex,
          date: clickedDate,
          tickStartDate: tick.startDate,
          tickEndDate: tick.endDate,
          tickParentIndex: tick.parentIndex,
          row: cellData.row,
          event: domEvent,
          ...mouseParams
        };
      }
    }
  }
  getCellElementFromDomEvent({
    target,
    clientY,
    type
  }) {
    const me = this,
      {
        isVertical,
        foregroundCanvas
      } = me,
      eventElement = target.closest(me.eventSelector);
    // If event was on an event bar, calculate the cell.
    if (eventElement) {
      return me.getCell({
        [isVertical ? 'row' : 'record']: isVertical ? 0 : me.resolveRowRecord(eventElement),
        column: me.timeAxisColumn
      });
    }
    // If event was triggered by an element in the foreground canvas, but not an event element
    // we need to ascertain the cell "behind" that element to be able to create the context.
    else if (foregroundCanvas.contains(target)) {
      // Only trigger a Scheduler event if the event was on the background itself.
      // Otherwise, we will trigger unexpected events on things like dependency lines which historically
      // have never triggered scheduleXXXX events. The exception to this is the mousemove event which
      // needs to always fire so that timelineContext and scheduleTooltip correctly track the mouse
      if (target === foregroundCanvas || type === 'mousemove') {
        var _me$rowManager$getRow;
        return (_me$rowManager$getRow = me.rowManager.getRowAt(clientY, false)) === null || _me$rowManager$getRow === void 0 ? void 0 : _me$rowManager$getRow.getCell(me.timeAxisColumn.id);
      }
    } else {
      // Event was inside a row, or on a row border.
      return target.matches('.b-grid-row') ? target.firstElementChild : target.closest(me.timeCellSelector);
    }
  }
  // Overridden by ResourceTimeRanges to "pass events through" to the schedule
  matchScheduleCell(element) {
    return element.closest(this.timeCellSelector);
  }
  onElementMouseButtonEvent(event) {
    const targetCell = event.target.closest('.b-sch-header-timeaxis-cell');
    if (targetCell) {
      const me = this,
        position = targetCell.parentElement.dataset.headerPosition,
        headerCells = me.timeAxisViewModel.columnConfig[position],
        index = me.timeAxis.isFiltered ? headerCells.findIndex(cell => cell.index == targetCell.dataset.tickIndex) : targetCell.dataset.tickIndex,
        cellConfig = headerCells[index],
        contextMenu = me.features.contextMenu;
      // Skip same events with Grid context menu triggerEvent
      if (!contextMenu || event.type !== contextMenu.triggerEvent) {
        this.trigger(`timeAxisHeader${StringHelper.capitalize(event.type)}`, {
          startDate: cellConfig.start,
          endDate: cellConfig.end,
          event
        });
      }
    }
  }
  onElementMouseDown(event) {
    this.onElementMouseButtonEvent(event);
    super.onElementMouseDown(event);
  }
  onElementClick(event) {
    this.onElementMouseButtonEvent(event);
    super.onElementClick(event);
  }
  onElementDblClick(event) {
    this.onElementMouseButtonEvent(event);
    super.onElementDblClick(event);
  }
  onElementContextMenu(event) {
    this.onElementMouseButtonEvent(event);
    super.onElementContextMenu(event);
  }
  /**
   * Relays mouseover events as eventmouseenter if over rendered event.
   * Also adds Scheduler#overScheduledEventClass to the hovered element.
   * @private
   */
  onElementMouseOver(event) {
    var _me$features$eventDra;
    super.onElementMouseOver(event);
    const me = this,
      {
        target
      } = event,
      {
        hoveredEvents
      } = me;
    // We must be over the event bar
    if (target.closest(me.eventInnerSelector) && !((_me$features$eventDra = me.features.eventDrag) !== null && _me$features$eventDra !== void 0 && _me$features$eventDra.isDragging)) {
      const eventElement = target.closest(me.eventSelector);
      if (!hoveredEvents.has(eventElement) && !me.preventOverCls) {
        hoveredEvents.add(eventElement);
        eventElement.classList.add(me.overScheduledEventClass);
        const params = me.getTimeSpanMouseEventParams(eventElement, event);
        if (params) {
          // do not fire this event if model cannot be found
          // this can be the case for "b-sch-dragcreator-proxy" elements for example
          me.trigger(`${me.scheduledEventName}MouseEnter`, params);
        }
      }
    } else if (hoveredEvents.size) {
      me.unhoverAll(event);
    }
  }
  /**
   * Relays mouseout events as eventmouseleave if out from rendered event.
   * Also removes Scheduler#overScheduledEventClass from the hovered element.
   * @private
   */
  onElementMouseOut(event) {
    var _me$features$eventDra2;
    super.onElementMouseOut(event);
    const me = this,
      {
        target,
        relatedTarget
      } = event,
      eventInner = target.closest(me.eventInnerSelector),
      eventWrap = target.closest(me.eventSelector),
      timeSpanRecord = me.resolveTimeSpanRecord(target);
    // We must be over the event bar
    if (eventInner && timeSpanRecord && me.hoveredEvents.has(eventWrap) && !((_me$features$eventDra2 = me.features.eventDrag) !== null && _me$features$eventDra2 !== void 0 && _me$features$eventDra2.isDragging)) {
      // out to child shouldn't count...
      if (relatedTarget && DomHelper.isDescendant(eventInner, relatedTarget)) {
        return;
      }
      me.unhover(eventWrap, event);
    }
  }
  unhover(element, event) {
    const me = this;
    element.classList.remove(me.overScheduledEventClass);
    me.trigger(`${me.scheduledEventName}MouseLeave`, me.getTimeSpanMouseEventParams(element, event));
    me.hoveredEvents.delete(element);
  }
  unhoverAll(event) {
    for (const element of this.hoveredEvents) {
      !element.isReleased && !element.classList.contains('b-released') && this.unhover(element, event);
    }
    // Might not be empty because of conditional unhover above
    this.hoveredEvents.clear();
  }
  //endregion
  // This does not need a className on Widgets.
  // Each *Class* which doesn't need 'b-' + constructor.name.toLowerCase() automatically adding
  // to the Widget it's mixed in to should implement thus.
  get widgetClass() {}
});

/**
 * @module Scheduler/view/mixin/TimelineViewPresets
 */
const datesDiffer = (d1 = 0, d2 = 0) => d2 - d1;
/**
 * View preset handling.
 *
 * A Scheduler's {@link #config-presets} are loaded with a default set of {@link Scheduler.preset.ViewPreset ViewPresets}
 * which are defined by the system in the {@link Scheduler.preset.PresetManager PresetManager}.
 *
 * The zooming feature works by reconfiguring the Scheduler with a new {@link Scheduler.preset.ViewPreset ViewPreset} selected
 * from the {@link #config-presets} store.
 *
 * {@link Scheduler.preset.ViewPreset ViewPresets} can be added and removed from the store to change the amount of available steps.
 * Range of zooming in/out can be also modified with {@link Scheduler.view.mixin.TimelineZoomable#config-maxZoomLevel} / {@link Scheduler.view.mixin.TimelineZoomable#config-minZoomLevel} properties.
 *
 * This mixin adds additional methods to the column : {@link Scheduler.view.mixin.TimelineZoomable#property-maxZoomLevel}, {@link Scheduler.view.mixin.TimelineZoomable#property-minZoomLevel}, {@link Scheduler.view.mixin.TimelineZoomable#function-zoomToLevel}, {@link Scheduler.view.mixin.TimelineZoomable#function-zoomIn},
 * {@link Scheduler.view.mixin.TimelineZoomable#function-zoomOut}, {@link Scheduler.view.mixin.TimelineZoomable#function-zoomInFull}, {@link Scheduler.view.mixin.TimelineZoomable#function-zoomOutFull}.
 *
 * **Notice**: Zooming is not supported when `forceFit` option is set to true for the Scheduler or for filtered timeaxis.
 *
 * @mixin
 */
var TimelineViewPresets = (Target => class TimelineViewPresets extends (Target || Base$1) {
  static get $name() {
    return 'TimelineViewPresets';
  }
  //region Default config
  static get configurable() {
    return {
      /**
       * A string key used to lookup a predefined {@link Scheduler.preset.ViewPreset} (e.g. 'weekAndDay', 'hourAndDay'),
       * managed by {@link Scheduler.preset.PresetManager}. See {@link Scheduler.preset.PresetManager} for more information.
       * Or a config object for a viewPreset.
       *
       * Options:
       * - 'secondAndMinute'
       * - 'minuteAndHour'
       * - 'hourAndDay'
       * - 'dayAndWeek'
       * - 'dayAndMonth'
       * - 'weekAndDay'
       * - 'weekAndMonth',
       * - 'monthAndYear'
       * - 'year'
       * - 'manyYears'
       * - 'weekAndDayLetter'
       * - 'weekDateAndMonth'
       * - 'day'
       * - 'week'
       *
       * If passed as a config object, the settings from the viewPreset with the provided `base` property will be used along
       * with any overridden values in your object.
       *
       * To override:
       * ```javascript
       * viewPreset : {
       *   base    : 'hourAndDay',
       *   id      : 'myHourAndDayPreset',
       *   headers : [
       *       {
       *           unit      : "hour",
       *           increment : 12,
       *           renderer  : (startDate, endDate, headerConfig, cellIdx) => {
       *               return "";
       *           }
       *       }
       *   ]
       * }
       * ```
       * or set a new valid preset config if the preset is not registered in the {@link Scheduler.preset.PresetManager}.
       *
       * When you use scheduler in weekview mode, this config is used to pick view preset. If passed view preset is not
       * supported by weekview (only 2 supported by default - 'day' and 'week') default preset will be used - 'week'.
       * @config {String|ViewPresetConfig}
       * @default
       * @category Common
       */
      viewPreset: 'weekAndDayLetter',
      /**
       * Get the {@link Scheduler.preset.PresetStore} created for the Scheduler,
       * or set an array of {@link Scheduler.preset.ViewPreset} config objects.
       * @member {Scheduler.preset.PresetStore|ViewPresetConfig[]} presets
       * @category Common
       */
      /**
       * An array of {@link Scheduler.preset.ViewPreset} config objects
       * which describes the available timeline layouts for this scheduler.
       *
       * By default, a predefined set is loaded from the {@link Scheduler.preset.PresetManager}.
       *
       * A {@link Scheduler.preset.ViewPreset} describes the granularity of the
       * timeline view and the layout and subdivisions of the timeline header.
       * @config {ViewPresetConfig[]} presets
       *
       * @category Common
       */
      presets: true,
      /**
       * Defines how dates will be formatted in tooltips etc. This config has priority over similar config on the
       * view preset. For allowed values see {@link Core.helper.DateHelper#function-format-static}.
       *
       * By default, this is ingested from {@link Scheduler.preset.ViewPreset} upon change of
       * {@link Scheduler.preset.ViewPreset} (Such as when zooming in or out). But Setting this
       * to your own value, overrides that behaviour.
       * @prp {String}
       * @category Scheduled events
       */
      displayDateFormat: null
    };
  }
  //endregion
  /**
   * Get/set the current view preset
   * @member {Scheduler.preset.ViewPreset|ViewPresetConfig|String} viewPreset
   * @param [viewPreset.options]
   * @param {Date} [viewPreset.options.startDate] A new start date for the time axis
   * @param {Date} [viewPreset.options.endDate] A new end date for the time axis
   * @param {Date} [viewPreset.options.centerDate] Where to center the new time axis
   * @category Common
  */
  //region Get/set
  changePresets(presets) {
    const config = {
      owner: this
    };
    let data = [];
    // By default includes all presets
    if (presets === true) {
      data = pm.allRecords;
    }
    // Accepts an array of presets
    else if (Array.isArray(presets)) {
      for (const preset of presets) {
        // If we got a presetId
        if (typeof preset === 'string') {
          const presetRecord = pm.getById(preset);
          if (presetRecord) {
            data.push(presetRecord);
          }
        } else {
          data.push(preset);
        }
      }
    }
    // Or a store config object
    else {
      ObjectHelper.assign(config, presets);
    }
    // Creates store first and then adds data, because data config does not support a mix of raw objects and records.
    const presetStore = new PresetStore(config);
    presetStore.add(data);
    return presetStore;
  }
  changeViewPreset(viewPreset, oldViewPreset) {
    const me = this,
      {
        presets
      } = me;
    if (viewPreset) {
      viewPreset = presets.createRecord(viewPreset);
      // If an existing ViewPreset id is used, this will replace it.
      if (!presets.includes(viewPreset)) {
        presets.add(viewPreset);
      }
    } else {
      viewPreset = presets.first;
    }
    const lastOpts = me.lastViewPresetOptions || {},
      options = viewPreset.options || (viewPreset.options = {}),
      event = options.event = {
        startDate: options.startDate,
        endDate: options.endDate,
        from: oldViewPreset,
        to: viewPreset,
        preset: viewPreset
      },
      presetChanged = !me._viewPreset || !me._viewPreset.equals(viewPreset),
      optionsChanged = datesDiffer(options.startDate, lastOpts.startDate) || datesDiffer(options.endDate, lastOpts.endDate) || datesDiffer(options.centerDate, lastOpts.centerDate) || options.startDate && datesDiffer(options.startDate, me.startDate) || options.endDate && datesDiffer(options.endDate, me.endDate);
    // Only return the value for onward processing if there's a change
    if (presetChanged || optionsChanged) {
      // Bypass the no-change check if the viewPreset is the same and we only got in here
      // because different options were asked for.
      if (!presetChanged) {
        me._viewPreset = null;
      }
      /**
       * Fired before the {@link #config-viewPreset} is changed.
       * @event beforePresetChange
       * @param {Scheduler.view.Scheduler} source This Scheduler instance.
       * @param {Date} startDate The new start date of the timeline.
       * @param {Date} endDate The new end date of the timeline.
       * @param {Scheduler.preset.ViewPreset} from The outgoing ViewPreset.
       * @param {Scheduler.preset.ViewPreset} to The ViewPreset being switched to.
       * @preventable
       */
      // Do not trigger events for the initial preset
      if (me.isConfiguring || me.trigger('beforePresetChange', event) !== false) {
        return viewPreset;
      }
    }
  }
  get displayDateFormat() {
    return this._displayDateFormat || this.viewPreset.displayDateFormat;
  }
  updateDisplayDateFormat(format) {
    // Start/EndDateColumn listens for this to change their format to match
    this.trigger('displayDateFormatChange', {
      format
    });
  }
  /**
   * Method to get a formatted display date
   * @private
   * @param {Date} date The date
   * @returns {String} The formatted date
   */
  getFormattedDate(date) {
    return DateHelper.format(date, this.displayDateFormat);
  }
  updateViewPreset(preset) {
    const me = this,
      {
        options
      } = preset,
      {
        event,
        startDate,
        endDate
      } = options,
      {
        isHorizontal,
        _timeAxis: timeAxis,
        // Do not tickle the getter, we are just peeking to see if it's there yet.
        _timeAxisViewModel: timeAxisViewModel // Ditto
      } = me,
      rtl = isHorizontal && me.rtl;
    let {
        centerDate,
        zoomDate,
        zoomPosition
      } = options,
      forceUpdate = false;
    // Options must not be reused when this preset is used again.
    delete preset.options;
    // Raise flag to prevent partner from changing view preset if one is in progress
    me._viewPresetChanging = true;
    if (timeAxis && !me.isConfiguring) {
      const {
        timelineScroller
      } = me;
      // Cache options only when they are applied so that non-change vetoing in changeViewPreset is accurate
      me.lastViewPresetOptions = options;
      // Timeaxis may already be configured (in case of sharing with the timeline partner), no need to reconfigure it
      if (timeAxis.isConfigured) {
        // None of this reconfiguring should cause a refresh
        me.suspendRefresh();
        // Set up these configs only if we actually have them.
        const timeAxisCfg = ObjectHelper.copyProperties({}, me, ['weekStartDay', 'startTime', 'endTime']);
        if (me.infiniteScroll) {
          Object.assign(timeAxisCfg, timeAxisViewModel.calculateInfiniteScrollingDateRange(centerDate || new Date((startDate.getTime() + endDate.getTime()) / 2), true, preset));
        }
        // if startDate is provided we use it and the provided endDate
        else if (startDate) {
          timeAxisCfg.startDate = startDate;
          timeAxisCfg.endDate = endDate;
          // if both dates are provided we can calculate centerDate for the viewport
          if (!centerDate && endDate) {
            centerDate = new Date((startDate.getTime() + endDate.getTime()) / 2);
          }
          // when no start/end dates are provided we use the current timespan
        } else {
          timeAxisCfg.startDate = timeAxis.startDate;
          timeAxisCfg.endDate = endDate || timeAxis.endDate;
          if (!centerDate) {
            centerDate = me.viewportCenterDate;
          }
        }
        timeAxis.isConfigured = false;
        timeAxisCfg.viewPreset = preset;
        timeAxis.reconfigure(timeAxisCfg, true);
        timeAxisViewModel.reconfigure({
          viewPreset: preset,
          headers: preset.headers,
          // This was hardcoded to 'middle' prior to the Preset refactor.
          // In the old code, the default headers were 'top' and 'middle', which
          // meant that 'middle' meant the lowest header.
          // So this is now length - 1.
          columnLinesFor: preset.columnLinesFor != null ? preset.columnLinesFor : preset.headers.length - 1,
          tickSize: isHorizontal ? preset.tickWidth : preset.tickHeight || preset.tickWidth || 60
        });
        // Allow refresh to run after the reconfiguring, without refreshing since we will do that below anyway
        me.resumeRefresh(false);
      }
      me.refresh();
      // if view is rendered and scroll is not disabled by "notScroll" option
      if (!options.notScroll && me.isPainted) {
        if (options.visibleDate) {
          me.visibleDate = options.visibleDate;
        }
        // If a zoom at a certain date position is being requested, scroll the zoomDate
        // to the required zoomPosition so that the zoom happens centered where the
        // pointer events that are driving it targeted.
        else if (zoomDate && zoomPosition) {
          const unitMagnitude = unitMagnitudes[timeAxis.resolutionUnit],
            unit = unitMagnitude > 3 ? 'hour' : 'minute',
            milliseconds = DateHelper.asMilliseconds(unit === 'minute' ? 15 : 1, unit),
            // Round the date to either 15 minutes for fine levels or 1 hour for coarse levels
            targetDate = new Date(Math.round(zoomDate / milliseconds) * milliseconds);
          // setViewPreset method on partner panels should be executed with same arguments.
          // if one partner was provided with zoom info, other one has to be too to generate exact
          // header and set same scroll
          event.zoomDate = zoomDate;
          event.zoomPosition = zoomPosition;
          event.zoomLevel = options.zoomLevel;
          // Move the targetDate back under the mouse position as indicated by zoomPosition.
          // That is the offset into the TimeAxisSubGridElement.
          if (rtl) {
            timelineScroller.position = timelineScroller.scrollWidth - (me.getCoordinateFromDate(targetDate) + zoomPosition);
          } else {
            timelineScroller.position = me.getCoordinateFromDate(targetDate) - zoomPosition;
          }
        }
        // and we have centerDate to scroll to
        else if (centerDate) {
          // remember the central date we scroll to (it gets reset after user scroll)
          me.cachedCenterDate = centerDate;
          // setViewPreset method on partner panels should be executed with same arguments.
          // if one partner was provided with a centerDate, other one has to be too to generate exact
          // header and set same scroll
          event.centerDate = centerDate;
          const viewportSize = me.timelineScroller.clientSize,
            centerCoord = rtl ? me.timeAxisViewModel.totalSize - me.getCoordinateFromDate(centerDate, true) : me.getCoordinateFromDate(centerDate, true),
            coord = Math.max(centerCoord - viewportSize / 2, 0);
          // The horizontal scroll handler must not invalidate the cached center
          // when this scroll event rolls round on the next frame.
          me.scrollingToCenter = true;
          // If preset change does not lead to a scroll we have to "refresh" manually at the end
          if (coord === (me.isHorizontal ? me.scrollLeft : me.scrollTop)) {
            forceUpdate = true;
          } else if (me.isHorizontal) {
            me.scrollHorizontallyTo(coord, false);
          } else {
            me.scrollVerticallyTo(coord, false);
          }
          // Release the lock on scrolling invalidating the cached center.
          me.setTimeout(() => {
            me.scrollingToCenter = false;
          }, 100);
        } else {
          // If preset change does not lead to a scroll we have to "refresh" manually at the end
          if ((me.isHorizontal ? me.scrollLeft : me.scrollTop) === 0) {
            forceUpdate = true;
          }
          // If we don't have a center date to scroll to, we reset scroll (this is bw compatible behavior)
          else {
            me.timelineScroller.scrollTo(0);
          }
        }
      }
    }
    // Update Scheduler element showing what preset is applied
    me.dataset.presetId = preset.id;
    /**
     * Fired after the {@link #config-viewPreset} has changed.
     * @event presetChange
     * @param {Scheduler.view.Scheduler} source This Scheduler instance.
     * @param {Date} startDate The new start date of the timeline.
     * @param {Date} centerDate The new center date of the timeline.
     * @param {Date} endDate The new end date of the timeline.
     * @param {Scheduler.preset.ViewPreset} from The outgoing ViewPreset.
     * @param {Scheduler.preset.ViewPreset} to The ViewPreset being switched to.
     * @preventable
     */
    me.trigger('presetChange', event);
    me._viewPresetChanging = false;
    if (forceUpdate) {
      if (me.isHorizontal) {
        me.currentOrientation.updateFromHorizontalScroll(me.scrollLeft, me.scrollX);
      } else {
        me.currentOrientation.updateFromVerticalScroll(me.scrollTop);
      }
    }
  }
  //endregion
  doDestroy() {
    if (this._presets.owner === this) {
      this._presets.destroy();
    }
    super.doDestroy();
  }
  // This function is not meant to be called by any code other than Base#getCurrentConfig().
  getCurrentConfig(options) {
    const result = super.getCurrentConfig(options);
    // Cannot store name, will not be allowed when reapplying
    if (result.viewPreset && result.viewPreset.name && !result.viewPreset.base) {
      delete result.viewPreset.name;
    }
    return result;
  }
  // This does not need a className on Widgets.
  // Each *Class* which doesn't need 'b-' + constructor.name.toLowerCase() automatically adding
  // to the Widget it's mixed in to should implement thus.
  get widgetClass() {}
});

/**
 * @module Scheduler/view/mixin/TimelineZoomable
 */
/**
 * Options which may be used when changing the {@link Scheduler.view.Scheduler#property-viewPreset} property.
 *
 * @typedef {Object} ChangePresetOptions
 * @property {VisibleDate} [visibleDate] A `visibleDate` specification to bring into view after the new
 * `ViewPreset` is applied.
 * @property {Date} [startDate] New time frame start. If provided along with end, view will be centered in this
 * time interval, ignoring centerDate config. __Ignored if {@link Scheduler.view.Scheduler#config-infiniteScroll} is used.__
 * @property {Date} [endDate] New time frame end. __Ignored if {@link Scheduler.view.Scheduler#config-infiniteScroll} is used.__
 * @property {Date} [centerDate] Date to keep in center. Is ignored when start and end are provided.
 * @property {Date} [zoomDate] The date that should be positioned at the passed `datePosition` client offset.
 * @property {Number} [zoomPosition] The client offset at which the passed `zoomDate` should be positioned.
 * @property {Number} [width] Lowest tick width. Might be increased automatically
 */
/**
 * Mixin providing "zooming" functionality.
 *
 * The zoom levels are stored as instances of {@link Scheduler.preset.ViewPreset}s, and are
 * cached centrally in the {@link Scheduler.preset.PresetManager}.
 *
 * The default presets are loaded into the {@link Scheduler.view.mixin.TimelineViewPresets#config-presets}
 * store upon Scheduler instantiation. Preset selection is covered in the
 * {@link Scheduler.view.mixin.TimelineViewPresets} mixin.
 *
 * To specify custom zoom levels please provide a set of view presets to the global PresetManager store **before**
 * scheduler creation, or provide a set of view presets to a specific scheduler only:
 *
 * ```javascript
 * const myScheduler = new Scheduler({
 *     presets : [
 *         {
 *             base : 'hourAndDay',
 *             id   : 'MyHourAndDay',
 *             // other preset configs....
 *         },
 *         {
 *             base : 'weekAndMonth',
 *             id   : 'MyWeekAndMonth',
 *             // other preset configs....
 *         }
 *     ],
 *     viewPreset : 'MyHourAndDay',
 *     // other scheduler configs....
 *     });
 * ```
 *
 * @mixin
 */
var TimelineZoomable = (Target => class TimelineZoomable extends (Target || Base$1) {
  static $name = 'TimelineZoomable';
  static defaultConfig = {
    /**
     * If true, you can zoom in and out on the time axis using CTRL-key + mouse wheel.
     * @config {Boolean}
     * @default
     * @category Zoom
     */
    zoomOnMouseWheel: true,
    /**
     * True to zoom to time span when double-clicking a time axis cell.
     * @config {Boolean}
     * @default
     * @category Zoom
     */
    zoomOnTimeAxisDoubleClick: true,
    /**
     * The minimum zoom level to which {@link #function-zoomOut} will work. Defaults to 0 (year ticks)
     * @config {Number}
     * @category Zoom
     * @default
     */
    minZoomLevel: 0,
    /**
     * The maximum zoom level to which {@link #function-zoomIn} will work. Defaults to the number of
     * {@link Scheduler.preset.ViewPreset ViewPresets} available, see {@link Scheduler/view/mixin/TimelineViewPresets#property-presets}
     * for information. Unless you have modified the collection of available presets, the max zoom level is
     * milliseconds.
     * @config {Number}
     * @category Zoom
     * @default 23
     */
    maxZoomLevel: null,
    /**
     * Integer number indicating the size of timespan during zooming. When zooming, the timespan is adjusted to make
     * the scrolling area `visibleZoomFactor` times wider than the timeline area itself. Used in
     * {@link #function-zoomToSpan} and {@link #function-zoomToLevel} functions.
     * @config {Number}
     * @default
     * @category Zoom
     */
    visibleZoomFactor: 5,
    /**
     * Whether the originally rendered timespan should be preserved while zooming. By default, it is set to `false`,
     * meaning the timeline panel will adjust the currently rendered timespan to limit the amount of HTML content to
     * render. When setting this option to `true`, be careful not to allow to zoom a big timespan in seconds
     * resolution for example. That will cause **a lot** of HTML content to be rendered and affect performance. You
     * can use {@link #config-minZoomLevel} and {@link #config-maxZoomLevel} config options for that.
     * @config {Boolean}
     * @default
     * @category Zoom
     */
    zoomKeepsOriginalTimespan: null
  };
  // We cache the last mousewheel position, so that during zooming we can
  // maintain a stable zoom point even if the mouse moves a little.
  lastWheelTime = -1;
  lastZoomPosition = -1;
  construct(config) {
    const me = this;
    super.construct(config);
    if (me.zoomOnMouseWheel) {
      EventHelper.on({
        element: me.timeAxisSubGridElement,
        wheel: 'onWheel',
        // Throttle zooming with the wheel a bit to have greater control of it
        throttled: {
          buffer: 100,
          // Prevent events from slipping through the throttle, causing scroll
          alt: e => e.ctrlKey && e.preventDefault()
        },
        thisObj: me,
        capture: true,
        passive: false
      });
    }
    if (me.zoomOnTimeAxisDoubleClick) {
      me.ion({
        timeaxisheaderdblclick: ({
          startDate,
          endDate
        }) => {
          if (!me.forceFit) {
            me.zoomToSpan({
              startDate,
              endDate
            });
          }
        }
      });
    }
  }
  get maxZoomLevel() {
    return this._maxZoomLevel || this.presets.count - 1;
  }
  /**
   * Get/set the {@link #config-maxZoomLevel} value
   * @property {Number}
   * @category Zoom
   */
  set maxZoomLevel(level) {
    if (typeof level !== 'number') {
      level = this.presets.count - 1;
    }
    if (level < 0 || level >= this.presets.count) {
      throw new Error('Invalid range for `maxZoomLevel`');
    }
    this._maxZoomLevel = level;
  }
  get minZoomLevel() {
    return this._minZoomLevel;
  }
  /**
   * Sets the {@link #config-minZoomLevel} value
   * @property {Number}
   * @category Zoom
   */
  set minZoomLevel(level) {
    if (typeof level !== 'number') {
      level = 0;
    }
    if (level < 0 || level >= this.presets.count) {
      throw new Error('Invalid range for `minZoomLevel`');
    }
    this._minZoomLevel = level;
  }
  /**
   * Current zoom level, which is equal to the {@link Scheduler.preset.ViewPreset ViewPreset} index
   * in the provided array of {@link Scheduler.view.mixin.TimelineViewPresets#config-presets zoom levels}.
   * @property {Number}
   * @category Zoom
   */
  get zoomLevel() {
    return this.presets.indexOf(this.viewPreset);
  }
  // noinspection JSAnnotator
  set zoomLevel(level) {
    this.zoomToLevel(level);
  }
  /**
   * Returns number of milliseconds per pixel.
   * @param {Object} level Element from array of {@link Scheduler.view.mixin.TimelineViewPresets#config-presets}.
   * @param {Boolean} ignoreActualWidth If true, then density will be calculated using default zoom level settings.
   * Otherwise, density will be calculated for actual tick width.
   * @returns {Number} Return number of milliseconds per pixel.
   * @private
   */
  getMilliSecondsPerPixelForZoomLevel(preset, ignoreActualWidth) {
    const {
        bottomHeader
      } = preset,
      // Scheduler uses direction independent tickSize, but presets are allowed to define different sizes for
      // vertical and horizontal -> cant use preset.tickSize here
      width = this.isHorizontal ? preset.tickWidth : preset.tickHeight;
    // trying to convert the unit + increment to a number of milliseconds
    // this number is not fixed (month can be 28, 30 or 31 day), but at least this conversion
    // will be consistent (should be no DST changes at year 1)
    return Math.round((DateHelper.add(new Date(1, 0, 1), bottomHeader.increment || 1, bottomHeader.unit) - new Date(1, 0, 1)) / (
    // `actualWidth` is a column width after view adjustments applied to it (see `calculateTickWidth`)
    // we use it if available to return the precise index value from `getCurrentZoomLevelIndex`
    ignoreActualWidth ? width : preset.actualWidth || width));
  }
  /**
   * Zooms to passed view preset, saving center date. Method accepts config object as a first argument, which can be
   * reduced to primitive type (string,number) when no additional options required. e.g.:
   * ```javascript
   * // zooming to preset
   * scheduler.zoomTo({ preset : 'hourAndDay' })
   * // shorthand
   * scheduler.zoomTo('hourAndDay')
   *
   * // zooming to level
   * scheduler.zoomTo({ level : 0 })
   * // shorthand
   * scheduler.zoomTo(0)
   * ```
   *
   * It is also possible to zoom to a time span by omitting `preset` and `level` configs, in which case scheduler sets
   * the time frame to a specified range and applies zoom level which allows to fit all columns to this range. The
   * given time span will be centered in the scheduling view (unless `centerDate` config provided). In the same time,
   * the start/end date of the whole time axis will be extended to allow scrolling for user.
   * ```javascript
   * // zooming to time span
   * scheduler.zoomTo({
   *     startDate : new Date(..),
   *     endDate : new Date(...)
   * });
   * ```
   *
   * @param {ViewPresetConfig|Object|String|Number} config Config object, preset name or zoom level index.
   * @param {String} [config.preset] Preset name to zoom to. Ignores level config in this case
   * @param {Number} [config.level] Zoom level to zoom to. Is ignored, if preset config is provided
   * @param {VisibleDate} [config.visibleDate] A `visibleDate` specification to bring into view after the zoom.
   * @param {Date} [config.startDate] New time frame start. If provided along with end, view will be centered in this
   * time interval (unless `centerDate` is present)
   * @param {Date} [config.endDate] New time frame end
   * @param {Date} [config.centerDate] Date that should be kept in the center. Has priority over start and end params
   * @param {Date} [config.zoomDate] The date that should be positioned at the passed `datePosition` client offset.
   * @param {Number} [config.zoomPosition] The client offset at which the passed `date` should be positioned.
   * @param {Number} [config.width] Lowest tick width. Might be increased automatically
   * @param {Number} [config.leftMargin] Amount of pixels to extend span start on (used, when zooming to span)
   * @param {Number} [config.rightMargin] Amount of pixels to extend span end on (used, when zooming to span)
   * @param {Number} [config.adjustStart] Amount of units to extend span start on (used, when zooming to span)
   * @param {Number} [config.adjustEnd] Amount of units to extend span end on (used, when zooming to span)
   * @category Zoom
   */
  zoomTo(config) {
    const me = this;
    if (typeof config === 'object') {
      if (config.preset) {
        me.zoomToLevel(config.preset, config);
      } else if (config.level != null) {
        me.zoomToLevel(config.level, config);
      } else {
        me.zoomToSpan(config);
      }
    } else {
      me.zoomToLevel(config);
    }
  }
  /**
   * Allows zooming to certain level of {@link Scheduler.view.mixin.TimelineViewPresets#config-presets} array.
   * Automatically limits zooming between {@link #config-maxZoomLevel} and {@link #config-minZoomLevel}. Can also set
   * time axis timespan to the supplied start and end dates.
   *
   * @param {Number} preset Level to zoom to.
   * @param {ChangePresetOptions} [options] Object containing options which affect how the new preset is applied.
   * @returns {Number|null} level Current zoom level or null if it hasn't changed.
   * @category Zoom
   */
  zoomToLevel(preset, options = {}) {
    if (this.forceFit) {
      console.warn('Warning: The forceFit setting and zooming cannot be combined');
      return;
    }
    // Sanitize numeric zooming.
    if (typeof preset === 'number') {
      preset = Math.min(Math.max(preset, this.minZoomLevel), this.maxZoomLevel);
    }
    const me = this,
      {
        presets
      } = me,
      tickSizeProp = me.isVertical ? 'tickHeight' : 'tickWidth',
      newPreset = presets.createRecord(preset),
      configuredTickSize = newPreset[tickSizeProp],
      startDate = options.startDate ? new Date(options.startDate) : null,
      endDate = options.endDate ? new Date(options.endDate) : null;
    // If an existing ViewPreset id is used, this will replace it.
    presets.add(newPreset);
    let span = startDate && endDate ? {
      startDate,
      endDate
    } : null;
    const centerDate = options.centerDate ? new Date(options.centerDate) : span ? new Date((startDate.getTime() + endDate.getTime()) / 2) : me.viewportCenterDateCached;
    let scrollableViewportSize = me.isVertical ? me.scrollable.clientHeight : me.timeAxisSubGrid.width;
    if (scrollableViewportSize === 0) {
      const {
        _beforeCollapseState
      } = me.timeAxisSubGrid;
      if (me.isHorizontal && me.timeAxisSubGrid.collapsed && _beforeCollapseState !== null && _beforeCollapseState !== void 0 && _beforeCollapseState.width) {
        scrollableViewportSize = _beforeCollapseState.width;
      } else {
        return null;
      }
    }
    // Always calculate an optimal date range for the new zoom level
    if (!span) {
      span = me.calculateOptimalDateRange(centerDate, scrollableViewportSize, newPreset);
    }
    // Temporarily override tick size while reconfiguring the TimeAxisViewModel
    if ('width' in options) {
      newPreset.setData(tickSizeProp, options.width);
    }
    me.isZooming = true;
    // Passed through to the viewPreset changing method
    newPreset.options = {
      ...options,
      startDate: span.startDate || me.startDate,
      endDate: span.endDate || me.endDate,
      centerDate
    };
    me.viewPreset = newPreset;
    // after switching the view preset the `width` config of the zoom level may change, because of adjustments
    // we will save the real value in the `actualWidth` property, so that `getCurrentZoomLevelIndex` method
    // will return the exact level index after zooming
    newPreset.actualWidth = me.timeAxisViewModel.tickSize;
    me.isZooming = false;
    // Restore the tick size because the default presets are shared.
    newPreset.setData(tickSizeProp, configuredTickSize);
    return me.zoomLevel;
  }
  /**
   * Changes the range of the scheduling chart to fit all the events in its event store.
   * @param {Object} [options] Options object for the zooming operation.
   * @param {Number} [options.leftMargin] Defines margin in pixel between the first event start date and first visible
   * date
   * @param {Number} [options.rightMargin] Defines margin in pixel between the last event end date and last visible
   * date
   */
  zoomToFit(options) {
    const eventStore = this.eventStore,
      span = eventStore.getTotalTimeSpan();
    options = {
      leftMargin: 0,
      rightMargin: 0,
      ...options,
      ...span
    };
    // Make sure we received a time span, event store might be empty
    if (options.startDate && options.endDate) {
      if (options.endDate > options.startDate) {
        this.zoomToSpan(options);
      } else {
        // If we only had a zero time span, just scroll it into view
        this.scrollToDate(options.startDate);
      }
    }
  }
  /**
   * Sets time frame to specified range and applies zoom level which allows to fit all columns to this range.
   *
   * The given time span will be centered in the scheduling view, in the same time, the start/end date of the whole
   * time axis will be extended in the same way as {@link #function-zoomToLevel} method does, to allow scrolling for
   * user.
   *
   * @param {Object} config The time frame.
   * @param {Date} config.startDate The time frame start.
   * @param {Date} config.endDate The time frame end.
   * @param {Date} [config.centerDate] Date that should be kept in the center. Has priority over start and end params
   * @param {Number} [config.leftMargin] Amount of pixels to extend span start on
   * @param {Number} [config.rightMargin] Amount of pixels to extend span end on
   * @param {Number} [config.adjustStart] Amount of units to extend span start on
   * @param {Number} [config.adjustEnd] Amount of units to extend span end on
   *
   * @returns {Number|null} level Current zoom level or null if it hasn't changed.
   * @category Zoom
   */
  zoomToSpan(config = {}) {
    if (config.leftMargin || config.rightMargin) {
      config.adjustStart = 0;
      config.adjustEnd = 0;
    }
    if (!config.leftMargin) config.leftMargin = 0;
    if (!config.rightMargin) config.rightMargin = 0;
    if (!config.startDate || !config.endDate) throw new Error('zoomToSpan: must provide startDate + endDate dates');
    const me = this,
      {
        timeAxis
      } = me,
      // this config enables old zoomToSpan behavior which we want to use for zoomToFit in Gantt
      needToAdjust = config.adjustStart >= 0 || config.adjustEnd >= 0;
    let {
      startDate,
      endDate
    } = config;
    if (needToAdjust) {
      startDate = DateHelper.add(startDate, -config.adjustStart, timeAxis.mainUnit);
      endDate = DateHelper.add(endDate, config.adjustEnd, timeAxis.mainUnit);
    }
    if (startDate <= endDate) {
      // get scheduling view width
      const {
          availableSpace
        } = me.timeAxisViewModel,
        presets = me.presets.allRecords,
        diffMS = endDate - startDate || 1;
      // if potential width of col is less than col width provided by zoom level
      //   - we'll zoom out panel until col width fit into width from zoom level
      // and if width of column is more than width from zoom level
      //   - we'll zoom in until col width fit won't fit into width from zoom level
      let currLevel = me.zoomLevel,
        inc,
        range;
      // if we zoomed out even more than the highest zoom level - limit it to the highest zoom level
      if (currLevel === -1) currLevel = 0;
      let msPerPixel = me.getMilliSecondsPerPixelForZoomLevel(presets[currLevel], true),
        // increment to get next zoom level:
        // -1 means that given timespan won't fit the available width in the current zoom level, we need to zoom out,
        // so that more content will "fit" into 1 px
        //
        // +1 mean that given timespan will already fit into available width in the current zoom level, but,
        // perhaps if we'll zoom in a bit more, the fitting will be better
        candidateLevel = currLevel + (inc = diffMS / msPerPixel + config.leftMargin + config.rightMargin > availableSpace ? -1 : 1),
        zoomLevel,
        levelToZoom = null;
      // loop over zoom levels
      while (candidateLevel >= 0 && candidateLevel <= presets.length - 1) {
        // get zoom level
        zoomLevel = presets[candidateLevel];
        msPerPixel = me.getMilliSecondsPerPixelForZoomLevel(zoomLevel, true);
        const spanWidth = diffMS / msPerPixel + config.leftMargin + config.rightMargin;
        // if zooming out
        if (inc === -1) {
          // if columns fit into available space, then all is fine, we've found appropriate zoom level
          if (spanWidth <= availableSpace) {
            levelToZoom = candidateLevel;
            // stop searching
            break;
          }
          // if zooming in
        } else {
          // if columns still fits into available space, we need to remember the candidate zoom level as a potential
          // resulting zoom level, the indication that we've found correct zoom level will be that timespan won't fit
          // into available view
          if (spanWidth <= availableSpace) {
            // if it's not currently active level
            if (currLevel !== candidateLevel - inc) {
              // remember this level as applicable
              levelToZoom = candidateLevel;
            }
          } else {
            // Sanity check to find the following case:
            // If we're already zoomed in at the appropriate level, but the current zoomLevel is "too small" to fit and had to be expanded,
            // there is an edge case where we should actually just stop and use the currently selected zoomLevel
            break;
          }
        }
        candidateLevel += inc;
      }
      // If we didn't find a large/small enough zoom level, use the lowest/highest level
      levelToZoom = levelToZoom != null ? levelToZoom : candidateLevel - inc;
      // presets is the array of all ViewPresets this Scheduler is using
      zoomLevel = presets[levelToZoom];
      const unitToZoom = zoomLevel.bottomHeader.unit;
      // Extract the correct msPerPixel value for the new zoom level
      msPerPixel = me.getMilliSecondsPerPixelForZoomLevel(zoomLevel, true);
      if (config.leftMargin || config.rightMargin) {
        // time axis doesn't yet know about new view preset (zoom level) so it cannot round/ceil date correctly
        startDate = new Date(startDate.getTime() - msPerPixel * config.leftMargin);
        endDate = new Date(endDate.getTime() + msPerPixel * config.rightMargin);
      }
      const tickCount = DateHelper.getDurationInUnit(startDate, endDate, unitToZoom, true) / zoomLevel.bottomHeader.increment;
      if (tickCount === 0) {
        return null;
      }
      const customWidth = Math.floor(availableSpace / tickCount),
        centerDate = config.centerDate || new Date((startDate.getTime() + endDate.getTime()) / 2);
      if (needToAdjust) {
        range = {
          startDate,
          endDate
        };
      } else {
        range = me.calculateOptimalDateRange(centerDate, availableSpace, zoomLevel);
      }
      let result = me.zoomLevel;
      // No change of zoom level needed, just move to the date range
      if (me.zoomLevel === levelToZoom) {
        timeAxis.reconfigure(range);
      } else {
        result = me.zoomToLevel(levelToZoom, Object.assign(range, {
          width: customWidth,
          centerDate
        }));
      }
      if (me.infiniteScroll) {
        me.scrollToDate(startDate, {
          block: 'start'
        });
      }
      return result;
    }
    return null;
  }
  /**
   * Zooms in the timeline according to the array of zoom levels. If the amount of levels to zoom is given, the view
   * will zoom in by this value. Otherwise, a value of `1` will be used.
   *
   * @param {Number} [levels] (optional) amount of levels to zoom in
   * @param {ChangePresetOptions} [options] Object containing options which affect how the new preset is applied.
   * @returns {Number|null} currentLevel New zoom level of the panel or null if level hasn't changed.
   * @category Zoom
   */
  zoomIn(levels = 1, options) {
    // Allow zoomIn({ visibleDate : ... })
    if (typeof levels === 'object') {
      options = levels;
      levels = 1;
    }
    const currentZoomLevelIndex = this.zoomLevel;
    if (currentZoomLevelIndex >= this.maxZoomLevel) {
      return null;
    }
    return this.zoomToLevel(currentZoomLevelIndex + levels, options);
  }
  /**
   * Zooms out the timeline according to the array of zoom levels. If the amount of levels to zoom is given, the view
   * will zoom out by this value. Otherwise, a value of `1` will be used.
   *
   * @param {Number} levels (optional) amount of levels to zoom out
   * @param {ChangePresetOptions} [options] Object containing options which affect how the new preset is applied.
   * @returns {Number|null} currentLevel New zoom level of the panel or null if level hasn't changed.
   * @category Zoom
   */
  zoomOut(levels = 1, options) {
    // Allow zoomOut({ visibleDate : ... })
    if (typeof levels === 'object') {
      options = levels;
      levels = 1;
    }
    const currentZoomLevelIndex = this.zoomLevel;
    if (currentZoomLevelIndex <= this.minZoomLevel) {
      return null;
    }
    return this.zoomToLevel(currentZoomLevelIndex - levels, options);
  }
  /**
   * Zooms in the timeline to the {@link #config-maxZoomLevel} according to the array of zoom levels.
   *
   * @param {ChangePresetOptions} [options] Object containing options which affect how the new preset is applied.
   * @returns {Number|null} currentLevel New zoom level of the panel or null if level hasn't changed.
   * @category Zoom
   */
  zoomInFull(options) {
    return this.zoomToLevel(this.maxZoomLevel, options);
  }
  /**
   * Zooms out the timeline to the {@link #config-minZoomLevel} according to the array of zoom levels.
   *
   * @param {ChangePresetOptions} [options] Object containing options which affect how the new preset is applied.
   * @returns {Number|null} currentLevel New zoom level of the panel or null if level hasn't changed.
   * @category Zoom
   */
  zoomOutFull(options) {
    return this.zoomToLevel(this.minZoomLevel, options);
  }
  /*
   * Adjusts the timespan of the panel to the new zoom level. Used for performance reasons,
   * as rendering too many columns takes noticeable amount of time so their number is limited.
   * @category Zoom
   * @private
   */
  calculateOptimalDateRange(centerDate, viewportSize, viewPreset, userProvidedSpan) {
    // this line allows us to always use the `calculateOptimalDateRange` method when calculating date range for zooming
    // (even in case when user has provided own interval)
    // other methods may override/hook into `calculateOptimalDateRange` to insert own processing
    // (infinite scrolling feature does)
    if (userProvidedSpan) return userProvidedSpan;
    const me = this,
      {
        timeAxis
      } = me,
      {
        bottomHeader
      } = viewPreset,
      tickWidth = me.isHorizontal ? viewPreset.tickWidth : viewPreset.tickHeight;
    if (me.zoomKeepsOriginalTimespan) {
      return {
        startDate: timeAxis.startDate,
        endDate: timeAxis.endDate
      };
    }
    const unit = bottomHeader.unit,
      difference = Math.ceil(viewportSize / tickWidth * bottomHeader.increment * me.visibleZoomFactor / 2),
      startDate = DateHelper.add(centerDate, -difference, unit),
      endDate = DateHelper.add(centerDate, difference, unit);
    if (me.infiniteScroll) {
      return me.timeAxisViewModel.calculateInfiniteScrollingDateRange(centerDate, true);
    } else {
      return {
        startDate: timeAxis.floorDate(startDate, false, unit, bottomHeader.increment),
        endDate: timeAxis.ceilDate(endDate, false, unit, bottomHeader.increment)
      };
    }
  }
  onElementMouseMove(event) {
    const {
      isHorizontal,
      zoomContext
    } = this;
    super.onElementMouseMove(event);
    if (event.isTrusted && zoomContext) {
      // Invalidate the zoomContext if mouse has strayed away from it
      if (Math.abs(event[`client${isHorizontal ? 'X' : 'Y'}`] - zoomContext.coordinate) > 10) {
        this.zoomContext = null;
      }
    }
  }
  async onWheel(event) {
    if (event.ctrlKey && !this.forceFit) {
      event.preventDefault();
      const me = this,
        {
          zoomContext,
          isHorizontal,
          timelineScroller,
          zoomLevel
        } = me,
        now = performance.now(),
        coordinate = event[`client${isHorizontal ? 'X' : 'Y'}`];
      let zoomPosition = coordinate - timelineScroller.viewport[`${isHorizontal ? 'x' : 'y'}`];
      // zoomPosition is the offset into the TimeAxisSubGridElement.
      if (isHorizontal && me.rtl) {
        zoomPosition = timelineScroller.viewport.width + timelineScroller.viewport.x - coordinate;
      }
      // If we are in a fast-arriving stream of wheel events, we use the same zoomDate as last time.
      // If it's a new zoom gesture or the pointer has strayed away from the context then ascertain
      // the gesture's center date
      if (now - me.lastWheelTime > 200 || !zoomContext || Math.abs(coordinate - me.zoomContext.coordinate) > 20) {
        // We're creating a zoom gesture which lasts as long as the
        // wheel events keep arriving at the same timeline position
        me.zoomContext = {
          // So we can track if we're going in (to finer resolutions)
          zoomLevel,
          // Pointer client(X|Y)
          coordinate,
          // Full TimeAxis offset position at which to place the date
          zoomPosition,
          // The date to place at the position
          zoomDate: me.getDateFromDomEvent(event)
        };
      }
      // Use the current zoomContext's zoomDate, but at each level, the relative position of that date
      // in the TimeAxis has to be corrected as the TimeAxis grows and scrolls to keep the zoomPosition
      // stable.
      else {
        // If we zoom in to a finer resolution, get a more accurate centering date.
        // If gesture was started at a years/months level, the date will be inaccurate.
        if (zoomLevel > zoomContext.zoomLevel) {
          zoomContext.zoomDate = me.getDateFromDomEvent(event);
          zoomContext.zoomLevel = zoomLevel;
        }
        zoomContext.zoomPosition = zoomPosition;
      }
      me.lastWheelTime = now;
      me[`zoom${event.deltaY > 0 ? 'Out' : 'In'}`](undefined, me.zoomContext);
    }
  }
  /**
   * Changes the time axis timespan to the supplied start and end dates.
   * @param {Date} startDate The new start date
   * @param {Date} [endDate] The new end date. If omitted or equal to startDate, the
   * {@link Scheduler.preset.ViewPreset#field-defaultSpan} property of the current view preset will be used to
   * calculate the new end date.
   */
  setTimeSpan(startDate, endDate) {
    this.timeAxis.setTimeSpan(startDate, endDate);
  }
  /**
   * Moves the time axis by the passed amount and unit.
   *
   * NOTE: If using a filtered time axis, see {@link Scheduler.data.TimeAxis#function-shift} for more information.
   *
   * @param {Number} amount The number of units to jump
   * @param {'ms'|'s'|'m'|'h'|'d'|'w'|'M'|'y'} [unit] The unit (Day, Week etc)
   */
  shift(amount, unit) {
    this.timeAxis.shift(amount, unit);
  }
  /**
   * Moves the time axis forward in time in units specified by the view preset `shiftUnit`, and by the amount
   * specified by the `shiftIncrement` config of the current view preset.
   *
   * NOTE: If using a filtered time axis, see {@link Scheduler.data.TimeAxis#function-shiftNext} for more information.
   *
   * @param {Number} [amount] The number of units to jump forward
   */
  shiftNext(amount) {
    this.timeAxis.shiftNext(amount);
  }
  /**
   * Moves the time axis backward in time in units specified by the view preset `shiftUnit`, and by the amount
   * specified by the `shiftIncrement` config of the current view preset.
   *
   * NOTE: If using a filtered time axis, see {@link Scheduler.data.TimeAxis#function-shiftPrevious} for more
   * information.
   *
   * @param {Number} [amount] The number of units to jump backward
   */
  shiftPrevious(amount) {
    this.timeAxis.shiftPrevious(amount);
  }
  // This does not need a className on Widgets.
  // Each *Class* which doesn't need 'b-' + constructor.name.toLowerCase() automatically adding
  // to the Widget it's mixed in to should implement thus.
  get widgetClass() {}
});

/**
 * @module Scheduler/view/mixin/TimelineEventRendering
 */
/**
 * Functions to handle event rendering (EventModel -> dom elements).
 *
 * @mixin
 */
var TimelineEventRendering = (Target => class TimelineEventRendering extends (Target || Base$1) {
  static get $name() {
    return 'TimelineEventRendering';
  }
  //region Default config
  static get defaultConfig() {
    return {
      resourceMargin: null,
      /**
       * When `true`, events are sized and positioned based on rowHeight, resourceMargin and barMargin settings.
       * Set this to `false` if you want to control height and vertical position using CSS instead.
       *
       * Note that events always get an absolute top position, but when this setting is enabled that position
       * will match row's top. To offset within the row using CSS, use `transform : translateY(y)`.
       *
       * @config {Boolean}
       * @default
       * @category Scheduled events
       */
      managedEventSizing: true,
      /**
       * The CSS class added to an event/assignment when it is newly created
       * in the UI and unsynced with the server.
       * @config {String}
       * @default
       * @private
       * @category CSS
       */
      generatedIdCls: 'b-sch-dirty-new',
      /**
       * The CSS class added to an event when it has unsaved modifications
       * @config {String}
       * @default
       * @private
       * @category CSS
       */
      dirtyCls: 'b-sch-dirty',
      /**
       * The CSS class added to an event when it is currently committing changes
       * @config {String}
       * @default
       * @private
       * @category CSS
       */
      committingCls: 'b-sch-committing',
      /**
       * The CSS class added to an event/assignment when it ends outside of the visible time range.
       * @config {String}
       * @default
       * @private
       * @category CSS
       */
      endsOutsideViewCls: 'b-sch-event-endsoutside',
      /**
       * The CSS class added to an event/assignment when it starts outside of the visible time range.
       * @config {String}
       * @default
       * @private
       * @category CSS
       */
      startsOutsideViewCls: 'b-sch-event-startsoutside',
      /**
       * The CSS class added to an event/assignment when it is not draggable.
       * @config {String}
       * @default
       * @private
       * @category CSS
       */
      fixedEventCls: 'b-sch-event-fixed',
      /**
       * Event style used by default. Events and resources can specify their own style, with priority order being:
       * Event -> Resource -> Scheduler default. Determines the appearance of the event by assigning a CSS class
       * to it. Available styles are:
       *
       * * `'plain'` (default) - flat look
       * * `'border'` - has border in darker shade of events color
       * * `'colored'` - has colored text and wide left border in same color
       * * `'hollow'` - only border + text until hovered
       * * `'line'` - as a line with the text below it
       * * `'dashed'` - as a dashed line with the text below it
       * * `'minimal'` - as a thin line with small text above it
       * * `'rounded'` - minimalistic style with rounded corners
       * * `null` - do not apply a default style and take control using custom CSS (easily overridden basic styling will be used).
       *
       * In addition, there are two styles intended to be used when integrating with Bryntum Calendar. To match
       * the look of Calendar events, you can use:
       *
       * * `'calendar'` - a variation of the "colored" style matching the default style used by Calendar
       * * `'interday'` - a variation of the "plain" style, for interday events
       *
       * @config {'plain'|'border'|'colored'|'hollow'|'line'|'dashed'|'minimal'|'rounded'|'calendar'|'interday'|null}
       * @default
       * @category Scheduled events
       */
      eventStyle: 'plain',
      /**
       * Event color used by default. Events and resources can specify their own color, with priority order being:
       * Event -> Resource -> Scheduler default. Available colors are:
       * * `HEX colors`
       * * `'red'`
       * * `'pink'`
       * * `'purple'`
       * * `'violet'`
       * * `'indigo'`
       * * `'blue'`
       * * `'cyan'`
       * * `'teal'`
       * * `'green'`
       * * `'lime'`
       * * `'yellow'`
       * * `'orange'`
       * * `'deep-orange'`
       * * `'gray'`
       * * `'gantt-green'` (Useful when you want to match the color to the default color in Gantt)
       * * `null` - do not apply a default color and take control using custom CSS (an easily overridden color will be used to make sure events are still visible).
       *
       * @member {'red'|'pink'|'purple'|'violet'|'indigo'|'blue'|'cyan'|'teal'|'green'|'lime'|'yellow'|'orange'|'deep-orange'|'gray'|'gantt-green'|String|null} eventColor
       * @category Scheduled events
       */
      /**
       * The event color used by the Scheduler. Events and resources can specify their own color. See
       * {@link #property-eventColor} for more details.
       *
       * @config {'red'|'pink'|'purple'|'violet'|'indigo'|'blue'|'cyan'|'teal'|'green'|'lime'|'yellow'|'orange'|'deep-orange'|'gray'|'gantt-green'|String|null} eventColor
       * @default 'green'
       * @category Scheduled events
       */
      eventColor: 'green',
      /**
       * The width/height (depending on vertical / horizontal mode) of all the time columns.
       * @config {Number}
       * @category Scheduled events
       */
      tickSize: null
    };
  }
  static configurable = {
    /**
     * Controls how much space to leave between stacked event bars in px.
     *
     * Value will be constrained by half the row height in horizontal mode.
     *
     * @prp {Number}
     * @default
     * @category Scheduled events
     */
    barMargin: 10,
    /**
     * Specify `true` to force rendered events/tasks to fill entire ticks. This only affects rendering, start
     * and end dates retain their value on the data level.
     *
     * When enabling `fillTicks` you should consider either disabling EventDrag/TaskDrag and EventResize/TaskResize,
     * or enabling {@link Scheduler/view/mixin/TimelineDateMapper#config-snap}. Otherwise their behaviour might not
     * be what a user expects.
     *
     * @prp {Boolean}
     * @default
     * @category Scheduled events
     */
    fillTicks: false
  };
  //endregion
  //region Settings
  updateFillTicks(fillTicks) {
    if (!this.isConfiguring) {
      this.timeAxis.forceFullTicks = fillTicks && this.snap;
      this.refreshWithTransition();
      this.trigger('stateChange');
    }
  }
  changeBarMargin(margin) {
    ObjectHelper.assertNumber(margin, 'barMargin');
    // bar margin should not exceed half of the row height
    if (this.isHorizontal && this.rowHeight) {
      return Math.min(Math.ceil(this.rowHeight / 2), margin);
    }
    return margin;
  }
  updateBarMargin() {
    if (this.rendered) {
      this.currentOrientation.onBeforeRowHeightChange();
      this.refreshWithTransition();
      this.trigger('stateChange');
    }
  }
  // Documented in SchedulerEventRendering to not show up in Gantt docs
  get resourceMargin() {
    return this._resourceMargin == null ? this.barMargin : this._resourceMargin;
  }
  set resourceMargin(margin) {
    const me = this;
    ObjectHelper.assertNumber(margin, 'resourceMargin');
    // bar margin should not exceed half of the row height
    if (me.isHorizontal && me.rowHeight) {
      margin = Math.min(Math.ceil(me.rowHeight / 2), margin);
    }
    if (me._resourceMargin !== margin) {
      me._resourceMargin = margin;
      if (me.rendered) {
        me.currentOrientation.onBeforeRowHeightChange();
        me.refreshWithTransition();
      }
    }
  }
  /**
   * Get/set the width/height (depending on mode) of all the time columns to the supplied value.
   * There is a limit for the tick size value. Its minimal allowed value is calculated so ticks would fit the available space.
   * Only applicable when {@link Scheduler.view.TimelineBase#config-forceFit} is set to `false`.
   * To set `tickSize` freely skipping that limitation please set {@link Scheduler.view.TimelineBase#config-suppressFit} to `true`.
   * @property {Number}
   * @category Scheduled events
   */
  set tickSize(width) {
    ObjectHelper.assertNumber(width, 'tickSize');
    this.timeAxisViewModel.tickSize = width;
  }
  get tickSize() {
    return this.timeAxisViewModel.tickSize;
  }
  /**
   * Predefined event colors, useful in combos etc.
   * @type {String[]}
   * @category Scheduled events
   */
  static get eventColors() {
    return ['red', 'pink', 'purple', 'violet', 'indigo', 'blue', 'cyan', 'teal', 'green', 'lime', 'yellow', 'orange', 'deep-orange', 'gray'];
  }
  /**
   * Predefined event styles , useful in combos etc.
   * @type {String[]}
   * @category Scheduled events
   */
  static get eventStyles() {
    return ['plain', 'border', 'hollow', 'colored', 'line', 'dashed', 'minimal', 'rounded'];
  }
  //endregion
  // This does not need a className on Widgets.
  // Each *Class* which doesn't need 'b-' + constructor.name.toLowerCase() automatically adding
  // to the Widget it's mixed in to should implement thus.
  get widgetClass() {}
});

/**
 * @module Scheduler/view/mixin/TimelineScroll
 */
const maintainVisibleStart = {
    maintainVisibleStart: true
  },
  defaultScrollOptions = {
    block: 'nearest'
  };
/**
 * Functions for scrolling to events, dates etc.
 *
 * @mixin
 */
var TimelineScroll = (Target => class TimelineScroll extends (Target || Base$1) {
  static get $name() {
    return 'TimelineScroll';
  }
  static get configurable() {
    return {
      /**
       * This config defines the size of the start and end invisible parts of the timespan when {@link #config-infiniteScroll} set to `true`.
       *
       * It should be provided as a coefficient, which will be multiplied by the size of the scheduling area.
       *
       * For example, if `bufferCoef` is `5` and the panel view width is 200px then the timespan will be calculated to
       * have approximately 1000px (`5 * 200`) to the left and 1000px to the right of the visible area, resulting
       * in 2200px of totally rendered content.
       *
       * @config {Number}
       * @category Infinite scroll
       * @default
       */
      bufferCoef: 5,
      /**
       * This config defines the scroll limit, which, when exceeded will cause a timespan shift.
       * The limit is calculated as the `panelWidth * {@link #config-bufferCoef} * bufferThreshold`. During scrolling, if the left or right side
       * has less than that of the rendered content - a shift is triggered.
       *
       * For example if `bufferCoef` is `5` and the panel view width is 200px and `bufferThreshold` is 0.2, then the timespan
       * will be shifted when the left or right side has less than 200px (5 * 200 * 0.2) of content.
       * @config {Number}
       * @category Infinite scroll
       * @default
       */
      bufferThreshold: 0.2,
      /**
       * Configure as `true` to automatically adjust the panel timespan during scrolling in the time dimension,
       * when the scroller comes close to the start/end edges.
       *
       * The actually rendered timespan in this mode (and thus the amount of HTML in the DOM) is calculated based
       * on the {@link #config-bufferCoef} option, and is thus not controlled by the {@link Scheduler/view/TimelineBase#config-startDate}
       * and {@link Scheduler/view/TimelineBase#config-endDate} configs. The moment when the timespan shift
       * happens is determined by the {@link #config-bufferThreshold} value.
       *
       * To specify initial point in time to view, supply the
       * {@link Scheduler/view/TimelineBase#config-visibleDate} config.
       *
       * @config {Boolean} infiniteScroll
       * @category Infinite scroll
       * @default
       */
      infiniteScroll: false
    };
  }
  initScroll() {
    const me = this,
      {
        isHorizontal,
        visibleDate
      } = me;
    super.initScroll();
    const {
      scrollable
    } = isHorizontal ? me.timeAxisSubGrid : me;
    scrollable.ion({
      scroll: 'onTimelineScroll',
      thisObj: me
    });
    // Ensure the TimeAxis starts life at the correct size with buffer zones
    // outside the visible window.
    if (me.infiniteScroll) {
      const setTimeSpanOptions = visibleDate ? {
          ...visibleDate,
          visibleDate: visibleDate.date
        } : {
          visibleDate: me.viewportCenterDate,
          block: 'center'
        },
        {
          startDate,
          endDate
        } = me.timeAxisViewModel.calculateInfiniteScrollingDateRange(setTimeSpanOptions.visibleDate, setTimeSpanOptions.block === 'center');
      // Don't ask to maintain visible start - we're initializing - there's no visible start yet.
      // If there's a visibleDate set, it will execute its scroll on paint.
      me.setTimeSpan(startDate, endDate, setTimeSpanOptions);
    }
  }
  /**
   * A {@link Core.helper.util.Scroller} which scrolls the time axis in whatever {@link Scheduler.view.Scheduler#config-mode} the
   * Scheduler is configured, either `horiontal` or `vertical`.
   *
   * The width and height dimensions are replaced by `size`. So this will expose the following properties:
   *
   *    - `clientSize` The size of the time axis viewport.
   *    - `scrollSize` The full scroll size of the time axis viewport
   *    - `position` The position scrolled to along the time axis viewport
   *
   * @property {Core.helper.util.Scroller}
   * @readonly
   * @category Scrolling
   */
  get timelineScroller() {
    const me = this;
    if (!me.scrollInitialized) {
      me.initScroll();
    }
    return me._timelineScroller || (me._timelineScroller = new TimelineScroller({
      widget: me,
      scrollable: me.isHorizontal ? me.timeAxisSubGrid.scrollable : me.scrollable,
      isHorizontal: me.isHorizontal
    }));
  }
  doDestroy() {
    var _this$_timelineScroll;
    (_this$_timelineScroll = this._timelineScroller) === null || _this$_timelineScroll === void 0 ? void 0 : _this$_timelineScroll.destroy();
    super.doDestroy();
  }
  onTimelineScroll({
    source
  }) {
    // On scroll, check if we are nearing the end to see if the sliding window needs moving.
    // onSchedulerHorizontalScroll is delayed to animationFrame
    if (this.infiniteScroll) {
      this.checkTimeAxisScroll(source[this.isHorizontal ? 'x' : 'y']);
    }
  }
  checkTimeAxisScroll(scrollPos) {
    const me = this,
      scrollable = me.timelineScroller,
      {
        clientSize
      } = scrollable,
      requiredSize = clientSize * me.bufferCoef,
      limit = requiredSize * me.bufferThreshold,
      maxScroll = scrollable.maxPosition,
      {
        style
      } = me.timeAxisSubGrid.virtualScrollerElement;
    // if scroll violates limits let's shift timespan
    if (maxScroll - scrollPos < limit || scrollPos < limit) {
      // If they were dragging the thumb, this must be a one-time thing. They *must* lose contact
      // with the thumb when the window shift occurs and the thumb zooms back to the center.
      // Changing for a short time to overflow:hidden terminates the thumb drag.
      // They can start again from the center, the reset happens very quickly.
      style.overflow = 'hidden';
      style.pointerEvents = 'none';
      // Avoid content height changing when scrollbar disappears
      style.paddingBottom = `${DomHelper.scrollBarWidth}px`;
      me.setTimeout(() => {
        style.overflow = '';
        style.paddingBottom = '';
        style.pointerEvents = '';
      }, 100);
      me.shiftToDate(me.getDateFromCoordinate(scrollPos, null, true, false, true));
    }
  }
  shiftToDate(date, centered) {
    const newRange = this.timeAxisViewModel.calculateInfiniteScrollingDateRange(date, centered);
    // this will trigger a refresh (`refreshKeepingScroll`) which will perform `restoreScrollState` and sync the scrolling position
    this.setTimeSpan(newRange.startDate, newRange.endDate, maintainVisibleStart);
  }
  // If we change to infinite scrolling dynamically, it should create the buffer zones.
  updateInfiniteScroll(infiniteScroll) {
    // At construction time, this is handled in initScroll.
    // This is just here to handle dynamic updates.
    if (!this.isConfiguring && infiniteScroll) {
      this.checkTimeAxisScroll(this.timelineScroller.position);
    }
  }
  //region Scroll to date
  /**
   * Scrolls the timeline "tick" encapsulating the passed `Date` into view according to the passed options.
   * @param {Date} date The date to which to scroll the timeline
   * @param {ScrollOptions} [options] How to scroll.
   * @returns {Promise} A Promise which resolves when the scrolling is complete.
   * @category Scrolling
   */
  async scrollToDate(date, options = {}) {
    const me = this,
      {
        timeAxis,
        visibleDateRange,
        infiniteScroll
      } = me,
      {
        unit,
        increment
      } = timeAxis,
      edgeOffset = options.edgeOffset || 0,
      visibleWidth = DateHelper.ceil(visibleDateRange.endDate, increment + ' ' + unit) - DateHelper.floor(visibleDateRange.startDate, increment + ' ' + unit),
      direction = date > me.viewportCenterDate ? 1 : -1,
      extraScroll = (infiniteScroll ? visibleWidth * me.bufferCoef * me.bufferThreshold : options.block === 'center' ? visibleWidth / 2 : edgeOffset ? me.getMilliSecondsPerPixelForZoomLevel(me.viewPreset) * edgeOffset : 0) * direction,
      visibleDate = new Date(date.getTime() + extraScroll),
      shiftDirection = visibleDate > timeAxis.endDate ? 1 : visibleDate < timeAxis.startDate ? -1 : 0;
    // Required visible date outside TimeAxis and infinite scrolling, that has opinions about how
    // much scroll range has to be created after the target date.
    if (shiftDirection && me.infiniteScroll) {
      me.shiftToDate(new Date(date - extraScroll), null, true);
      // shift to date could trigger a native browser async scroll out of our control. If a scroll
      // happens during scrollToCoordinate, the scrolling is cancelled so we wait a bit here
      await me.nextAnimationFrame();
    }
    const scrollerViewport = me.timelineScroller.viewport,
      localCoordinate = me.getCoordinateFromDate(date, true),
      // Available space can be less than tick size (Export.t.js in Gantt)
      width = Math.min(me.timeAxisViewModel.tickSize, me.timeAxisViewModel.availableSpace),
      target = me.isHorizontal
      // In RTL coordinate is for the right edge of the tick, so we need to subtract width
      ? new Rectangle(me.getCoordinateFromDate(date, false) - (me.rtl ? width : 0), scrollerViewport.y, width, scrollerViewport.height) : new Rectangle(scrollerViewport.x, me.getCoordinateFromDate(date, false), scrollerViewport.width, me.timeAxisViewModel.tickSize);
    await me.scrollToCoordinate(localCoordinate, target, date, options);
  }
  /**
   * Scrolls to current time.
   * @param {ScrollOptions} [options] How to scroll.
   * @returns {Promise} A Promise which resolves when the scrolling is complete.
   * @category Scrolling
   */
  scrollToNow(options = {}) {
    return this.scrollToDate(new Date(), options);
  }
  /**
   * Used by {@link #function-scrollToDate} to scroll to correct coordinate.
   * @param {Number} localCoordinate Coordinate to scroll to
   * @param {Date} date Date to scroll to, used for reconfiguring the time axis
   * @param {ScrollOptions} [options] How to scroll.
   * @returns {Promise} A Promise which resolves when the scrolling is complete.
   * @private
   * @category Scrolling
   */
  async scrollToCoordinate(localCoordinate, target, date, options = {}) {
    const me = this;
    // Not currently have this date in a timeaxis. Ignore negative scroll in weekview, it can be just 'filtered' with
    // startTime/endTime config
    if (localCoordinate < 0) {
      // adjust the timeaxis first
      const visibleSpan = me.endDate - me.startDate,
        {
          unit,
          increment
        } = me.timeAxis,
        newStartDate = DateHelper.floor(new Date(date.getTime() - visibleSpan / 2), increment + ' ' + unit),
        newEndDate = DateHelper.add(newStartDate, visibleSpan);
      // We're trying to reconfigure time span to current dates, which means we are as close to center as it
      // could be. Do nothing then.
      // covered by 1102_panel_api
      if (newStartDate - me.startDate !== 0 && newEndDate - me.endDate !== 0) {
        me.setTimeSpan(newStartDate, newEndDate);
        return me.scrollToDate(date, options);
      }
      return;
    }
    await me.timelineScroller.scrollIntoView(target, options);
    // Horizontal scroll is triggered on next frame in SubGrid.js, view is not up to date yet. Resolve on next frame
    return !me.isDestroyed && me.nextAnimationFrame();
  }
  //endregion
  //region Relative scrolling
  // These methods are important to users because although they are mixed into the top level Grid/Scheduler,
  // for X scrolling the explicitly target the SubGrid that holds the scheduler.
  /**
   * Get/set the `scrollLeft` value of the SubGrid that holds the scheduler.
   *
   * This may be __negative__ when the writing direction is right-to-left.
   * @property {Number}
   * @category Scrolling
   */
  set scrollLeft(left) {
    this.timeAxisSubGrid.scrollable.element.scrollLeft = left;
  }
  get scrollLeft() {
    return this.timeAxisSubGrid.scrollable.element.scrollLeft;
  }
  /**
   * Get/set the writing direction agnostic horizontal scroll position.
   *
   * This is always the __positive__ offset from the scroll origin whatever the writing
   * direction in use.
   *
   * Applies to the SubGrid that holds the scheduler
   * @property {Number}
   * @category Scrolling
   */
  set scrollX(x) {
    this.timeAxisSubGrid.scrollable.x = x;
  }
  get scrollX() {
    return this.timeAxisSubGrid.scrollable.x;
  }
  /**
   * Get/set vertical scroll
   * @property {Number}
   * @category Scrolling
   */
  set scrollTop(top) {
    this.scrollable.y = top;
  }
  get scrollTop() {
    return this.scrollable.y;
  }
  /**
   * Horizontal scrolling. Applies to the SubGrid that holds the scheduler
   * @param {Number} x
   * @param {ScrollOptions|Boolean} [options] How to scroll. May be passed as `true` to animate.
   * @returns {Promise} A promise which is resolved when the scrolling has finished.
   * @category Scrolling
   */
  scrollHorizontallyTo(coordinate, options = true) {
    return this.timeAxisSubGrid.scrollable.scrollTo(coordinate, null, options);
  }
  /**
   * Vertical scrolling
   * @param {Number} y
   * @param {ScrollOptions|Boolean} [options] How to scroll. May be passed as `true` to animate.
   * @returns {Promise} A promise which is resolved when the scrolling has finished.
   * @category Scrolling
   */
  scrollVerticallyTo(y, options = true) {
    return this.scrollable.scrollTo(null, y, options);
  }
  /**
   * Scrolls the subgrid that contains the scheduler
   * @param {Number} x
   * @param {ScrollOptions|Boolean} [options] How to scroll. May be passed as `true` to animate.
   * @returns {Promise} A promise which is resolved when the scrolling has finished.
   * @category Scrolling
   */
  scrollTo(x, options = true) {
    return this.timeAxisSubGrid.scrollable.scrollTo(x, null, options);
  }
  //endregion
  // This does not need a className on Widgets.
  // Each *Class* which doesn't need 'b-' + constructor.name.toLowerCase() automatically adding
  // to the Widget it's mixed in to should implement thus.
  get widgetClass() {}
});
// Internal class used to interrogate and manipulate the timeline scroll position.
// This delegates all operations to the appropriate Scroller, horizontal or vertical.
class TimelineScroller extends Scroller {
  static get configurable() {
    return {
      position: null,
      x: null,
      y: null
    };
  }
  // This class is passive about configuring the element.
  // It has no opinions about *how* the overflow is handled.
  updateOverflowX() {}
  updateOverflowY() {}
  onScroll(e) {
    super.onScroll(e);
    this._position = null;
  }
  syncPartners(force) {
    this.scrollable.syncPartners(force);
  }
  updatePosition(position) {
    this.scrollable[this.isHorizontal ? 'x' : 'y'] = position;
  }
  get viewport() {
    return this.scrollable.viewport;
  }
  get position() {
    return this._position = this.scrollable[this.isHorizontal ? 'x' : 'y'];
  }
  get clientSize() {
    return this.scrollable[`client${this.isHorizontal ? 'Width' : 'Height'}`];
  }
  get scrollSize() {
    return this.scrollable[`scroll${this.isHorizontal ? 'Width' : 'Height'}`];
  }
  get maxPosition() {
    return this.scrollable[`max${this.isHorizontal ? 'X' : 'Y'}`];
  }
  scrollTo(position, options) {
    return this.isHorizontal ? this.scrollable.scrollTo(position, null, options) : this.scrollable.scrollTo(null, position, options);
  }
  scrollBy(xDelta = 0, yDelta = 0, options = defaultScrollOptions) {
    // Use the correct delta by default, but if it's zero, accommodate axis error.
    return this.isHorizontal ? this.scrollable.scrollBy(xDelta || yDelta, 0, options) : this.scrollable.scrollBy(0, yDelta || xDelta, options);
  }
  scrollIntoView() {
    return this.scrollable.scrollIntoView(...arguments);
  }
  // We accommodate mistakes. Setting X and Y sets the appropriate scroll axis position
  changeX(x) {
    this.position = x;
  }
  changeY(y) {
    this.position = y;
  }
  get x() {
    return this.position;
  }
  set x(x) {
    this.scrollable[this.isHorizontal ? 'x' : 'y'] = x;
  }
  get y() {
    return this.position;
  }
  set y(y) {
    this.scroller[this.isHorizontal ? 'x' : 'y'] = y;
  }
  get clientWidth() {
    return this.clientSize;
  }
  get clientHeight() {
    return this.clientSize;
  }
  get scrollWidth() {
    return this.scrollSize;
  }
  get scrollHeight() {
    return this.scrollSize;
  }
  get maxX() {
    return this.maxPosition;
  }
  get maxY() {
    return this.maxPosition;
  }
}

/**
 * @module Scheduler/view/mixin/TimelineState
 */
const copyProperties = ['barMargin'];
/**
 * Mixin for Timeline base that handles state. It serializes the following timeline properties:
 *
 * * barMargin
 * * zoomLevel
 *
 * See {@link Grid.view.mixin.GridState} and {@link Core.mixin.State} for more information on state.
 *
 * @mixin
 */
var TimelineState = (Target => class TimelineState extends (Target || Base$1) {
  static get $name() {
    return 'TimelineState';
  }
  /**
   * Gets or sets timeline's state. Check out {@link Scheduler.view.mixin.TimelineState} mixin for details.
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
   * @property {Number} state.barMargin
   * @property {Number} state.zoomLevel
   * @category State
   */
  /**
   * Get timeline's current state for serialization. State includes rowHeight, headerHeight, readOnly, selectedCell,
   * selectedRecordId, column states and store state etc.
   * @returns {Object} State object to be serialized
   * @private
   */
  getState() {
    const me = this,
      state = ObjectHelper.copyProperties(super.getState(), me, copyProperties);
    state.zoomLevel = me.zoomLevel;
    state.zoomLevelOptions = {
      startDate: me.startDate,
      endDate: me.endDate,
      // With infinite scroll reading viewportCenterDate too early will lead to exception
      centerDate: !me.infiniteScroll || me.timeAxisViewModel.availableSpace ? me.viewportCenterDate : undefined,
      width: me.tickSize
    };
    return state;
  }
  /**
   * Apply previously stored state.
   * @param {Object} state
   * @private
   */
  applyState(state) {
    const me = this;
    me.suspendRefresh();
    ObjectHelper.copyProperties(me, state, copyProperties);
    super.applyState(state);
    if (state.zoomLevel != null) {
      // Do not restore left scroll, infinite scroll should do all the work
      if (me.infiniteScroll) {
        var _state$scroll;
        if (state !== null && state !== void 0 && (_state$scroll = state.scroll) !== null && _state$scroll !== void 0 && _state$scroll.scrollLeft) {
          state.scroll.scrollLeft = {};
        }
      }
      if (me.isPainted) {
        me.zoomToLevel(state.zoomLevel, state.zoomLevelOptions);
      } else {
        me._zoomAfterPaint = {
          zoomLevel: state.zoomLevel,
          zoomLevelOptions: state.zoomLevelOptions
        };
      }
    }
    me.resumeRefresh(true);
  }
  onPaint(...args) {
    super.onPaint(...args);
    if (this._zoomAfterPaint) {
      const {
        zoomLevel,
        zoomLevelOptions
      } = this._zoomAfterPaint;
      this.zoomToLevel(zoomLevel, zoomLevelOptions);
      delete this._zoomAfterPaint;
    }
  }
  // This does not need a className on Widgets.
  // Each *Class* which doesn't need 'b-' + constructor.name.toLowerCase() automatically adding
  // to the Widget it's mixed in to should implement thus.
  get widgetClass() {}
});

/**
 * @module Scheduler/view/Header
 */
/**
 * Custom header subclass which handles the existence of the special TimeAxisColumn
 *
 * @extends Grid/view/Header
 * @private
 */
class Header extends Header$1 {
  static get $name() {
    return 'SchedulerHeader';
  }
  refreshContent() {
    var _this$headersElement;
    // Only render contents into the header once as it contains the special rendering of the TimeAxisColumn
    // In case ResizeObserver polyfill is used headers element will have resize monitors inserted and we should
    // take that into account
    // https://github.com/bryntum/support/issues/3444
    if (!((_this$headersElement = this.headersElement) !== null && _this$headersElement !== void 0 && _this$headersElement.querySelector('.b-sch-timeaxiscolumn'))) {
      super.refreshContent();
    }
  }
}
Header._$name = 'Header';

/**
 * @module Scheduler/view/TimeAxisSubGrid
 */
/**
 * Widget that encapsulates the SubGrid part of the scheduler which houses the timeline view.
 * @extends Grid/view/SubGrid
 * @private
 */
class TimeAxisSubGrid extends SubGrid {
  static get $name() {
    return 'TimeAxisSubGrid';
  }
  // Factoryable type name
  static get type() {
    return 'timeaxissubgrid';
  }
  static get configurable() {
    return {
      // A Scheduler's SubGrid doesn't accept external columns moving in
      sealedColumns: true,
      // Use Scheduler's Header class
      headerClass: Header
    };
  }
  startConfigure(config) {
    const {
      grid: scheduler
    } = config;
    // Scheduler references its TimeAxisSubGrid instance through this property.
    scheduler.timeAxisSubGrid = this;
    super.startConfigure(config);
    if (scheduler.isHorizontal) {
      config.header = {
        cls: {
          'b-sticky-headers': scheduler.stickyHeaders
        }
      };
      // We don't use what the GridSubGrids mixin tells us to.
      // We use the Sheduler's Header class.
      delete config.headerClass;
    }
    // If user have not specified a width or flex for scheduler region, default to flex=1
    if (!('flex' in config || 'width' in config)) {
      config.flex = 1;
    }
  }
  changeScrollable() {
    const me = this,
      scrollable = super.changeScrollable(...arguments);
    // TimeAxisSubGrid's X axis is stretched by its canvas.
    // We don't need the Scroller's default stretching implementation.
    if (scrollable) {
      Object.defineProperty(scrollable, 'scrollWidth', {
        get() {
          var _this$element;
          return ((_this$element = this.element) === null || _this$element === void 0 ? void 0 : _this$element.scrollWidth) ?? 0;
        },
        set() {
          // Setting the scroll width to be wide just updates the canvas side in Scheduler.
          // We do not need the Scroller's default stretcher element to be added.
          // Note that "me" here is the TimeAxisSubGrid, so we are calling Scheduler.
          me.grid.updateCanvasSize();
        }
      });
    }
    return scrollable;
  }
  syncScrollingPartners(addCls = true) {
    // Swallow scroll syncing calls that happen during view preset changes, that process triggers multiple when
    // it first changes tickWidth, then scrolls to center and then an additional sync on scroll end
    if (!this.grid._viewPresetChanging) {
      super.syncScrollingPartners(addCls);
    }
  }
  /**
   * This is an event handler triggered when the TimeAxisSubGrid changes size.
   * Its height changes when content height changes, and that is not what we are
   * interested in here. If the *width* changes, that means the visible viewport
   * has changed size.
   * @param {HTMLElement} element
   * @param {Number} width
   * @param {Number} height
   * @param {Number} oldWidth
   * @param {Number} oldHeight
   * @private
   */
  onInternalResize(element, width, height, oldWidth, oldHeight) {
    const me = this;
    // We, as the TimeAxisSubGrid dictate the scheduler viewport width
    if (me.isPainted && width !== oldWidth) {
      const scheduler = me.grid,
        bodyHeight = scheduler._bodyRectangle.height;
      // Avoid ResizeObserver errors when this operation may create a scrollbar
      if (DomHelper.scrollBarWidth && width < oldWidth) {
        me.monitorResize = false;
      }
      scheduler.onSchedulerViewportResize(width, bodyHeight, oldWidth, bodyHeight);
      // Revert to monitoring on the next animation frame.
      // This is to avoid "ResizeObserver loop completed with undelivered notifications."
      if (!me.monitorResize) {
        me.requestAnimationFrame(() => me.monitorResize = true);
      }
    }
    super.onInternalResize(...arguments);
  }
  // When restoring state we need to update time axis size immediately, resize event is not triggered fast enough to
  // restore center date consistently
  clearWidthCache() {
    super.clearWidthCache();
    // Check if we are in horizontal mode
    if (this.owner.isHorizontal) {
      this.owner.updateViewModelAvailableSpace(this.width);
    }
  }
  async expand() {
    const {
      owner
    } = this;
    await super.expand();
    if (owner.isPainted) {
      owner.timeAxisViewModel.update(this.width, false, true);
    }
  }
}
// Register this widget type with its Factory
TimeAxisSubGrid.initClass();
TimeAxisSubGrid._$name = 'TimeAxisSubGrid';

const exitTransition = {
    fn: 'exitTransition',
    delay: 0,
    cancelOutstanding: true
  },
  emptyObject = {};
/**
 * @module Scheduler/view/TimelineBase
 */
/**
 * Options accepted by the Scheduler's {@link Scheduler.view.Scheduler#config-visibleDate} property.
 *
 * @typedef {Object} VisibleDate
 * @property {Date} date The date to bring into view.
 * @property {'start'|'end'|'center'|'nearest'} [block] How far to scroll the date.
 * @property {Number} [edgeOffset] edgeOffset A margin around the date to bring into view.
 * @property {AnimateScrollOptions|Boolean|Number} [animate] Set to `true` to animate the scroll by 300ms,
 * or the number of milliseconds to animate over, or an animation config object.
 */
/**
 * Abstract base class used by timeline based components such as Scheduler and Gantt. Based on Grid, supplies a "locked"
 * region for columns and a "normal" for rendering of events etc.
 * @abstract
 *
 * @mixes Scheduler/view/mixin/TimelineDateMapper
 * @mixes Scheduler/view/mixin/TimelineDomEvents
 * @mixes Scheduler/view/mixin/TimelineEventRendering
 * @mixes Scheduler/view/mixin/TimelineScroll
 * @mixes Scheduler/view/mixin/TimelineState
 * @mixes Scheduler/view/mixin/TimelineViewPresets
 * @mixes Scheduler/view/mixin/TimelineZoomable
 * @mixes Scheduler/view/mixin/RecurringEvents
 *
 * @extends Grid/view/Grid
 */
class TimelineBase extends GridBase.mixin(TimelineDateMapper, TimelineDomEvents, TimelineEventRendering, TimelineScroll, TimelineState, TimelineViewPresets, TimelineZoomable, RecurringEvents) {
  //region Config
  static get $name() {
    return 'TimelineBase';
  }
  // Factoryable type name
  static get type() {
    return 'timelinebase';
  }
  static configurable = {
    partnerSharedConfigs: {
      value: ['timeAxisViewModel', 'timeAxis', 'viewPreset'],
      $config: {
        merge: 'distinct'
      }
    },
    /**
     * Get/set startDate. Defaults to current date if none specified.
     *
     * When using {@link #config-infiniteScroll}, use {@link #config-visibleDate} to control initially visible date
     * instead.
     *
     * **Note:** If you need to set start and end date at the same time, use {@link #function-setTimeSpan} method.
     * @member {Date} startDate
     * @category Common
     */
    /**
     * The start date of the timeline (if not configure with {@link #config-infiniteScroll}).
     *
     * If omitted, and a TimeAxis has been set, the start date of the provided {@link Scheduler.data.TimeAxis} will
     * be used. If no TimeAxis has been configured, it'll use the start/end dates of the loaded event dataset. If no
     * date information exists in the event data set, it defaults to the current date and time.
     *
     * If a string is supplied, it will be parsed using
     * {@link Core/helper/DateHelper#property-defaultFormat-static DateHelper.defaultFormat}.
     *
     * When using {@link #config-infiniteScroll}, use {@link #config-visibleDate} to control initially visible date
     * instead.
     *
     * **Note:** If you need to set start and end date at the same time, use the {@link #function-setTimeSpan} method.
     * @config {Date|String}
     * @category Common
     */
    startDate: {
      $config: {
        equal: 'date'
      },
      value: null
    },
    /**
     * Get/set endDate. Defaults to startDate + default span of the used ViewPreset.
     *
     * **Note:** If you need to set start and end date at the same time, use {@link #function-setTimeSpan} method.
     * @member {Date} endDate
     * @category Common
     */
    /**
     * The end date of the timeline (if not configure with {@link #config-infiniteScroll}).
     *
     * If omitted, it will be calculated based on the {@link #config-startDate} setting and the 'defaultSpan'
     * property of the current {@link #config-viewPreset}.
     *
     * If a string is supplied, it will be parsed using
     * {@link Core/helper/DateHelper#property-defaultFormat-static DateHelper.defaultFormat}.
     *
     * **Note:** If you need to set start and end date at the same time, use the {@link #function-setTimeSpan} method.
     * @config {Date|String}
     * @category Common
     */
    endDate: {
      $config: {
        equal: 'date'
      },
      value: null
    },
    /**
     * When set, the text in the major time axis header sticks in the scrolling viewport as long as possible.
     * @config {Boolean}
     * @default
     * @category Time axis
     */
    stickyHeaders: true,
    /**
     * A scrolling `options` object describing the scroll action, including a `date` option
     * which references a `Date`. See {@link #function-scrollToDate} for details about scrolling options.
     *
     * ```javascript
     *     // The date we want in the center of the Scheduler viewport
     *     myScheduler.visibleDate = {
     *         date    : new Date(2023, 5, 17, 12),
     *         block   : 'center',
     *         animate : true
     *     };
     * ```
     * @member {Object} visibleDate
     * @category Common
     */
    /**
     * A date to bring into view initially on the scrollable timeline.
     *
     * This may be configured as either a `Date` or a scrolling `options` object describing
     * the scroll action, including a `date` option which references a `Date`.
     *
     * See {@link #function-scrollToDate} for details about scrolling options.
     *
     * Note that if a naked `Date` is passed, it will be stored internally as a scrolling options object
     * using the following defaults:
     *
     * ```javascript
     * {
     *     date  : <The Date object>,
     *     block : 'nearest'
     * }
     * ```
     *
     * This moves the date into view by the shortest scroll, so that it just appears at an edge.
     *
     * To bring your date of interest to the center of the viewport, configure your
     * Scheduler thus:
     *
     * ```javascript
     *     visibleDate : {
     *         date  : new Date(2023, 5, 17, 12),
     *         block : 'center'
     *     }
     * ```
     * @config {Date|VisibleDate}
     * @category Common
     */
    visibleDate: null,
    /**
     * CSS class to add to rendered events
     * @config {String}
     * @category CSS
     * @private
     */
    eventCls: null,
    /**
     * Set to `true` to force the time columns to fit to the available space (horizontal or vertical depends on mode).
     * Note that setting {@link #config-suppressFit} to `true`, will disable `forceFit` functionality. Zooming
     * cannot be used when `forceFit` is set.
     * @prp {Boolean}
     * @default
     * @category Time axis
     */
    forceFit: false,
    /**
     * Set to a time zone or a UTC offset. This will set the projects
     * {@link Scheduler.model.ProjectModel#config-timeZone} config accordingly. As this config is only a referer,
     * please se project's config {@link Scheduler.model.ProjectModel#config-timeZone documentation} for more
     * information.
     *
     * ```javascript
     * new Calendar(){
     *   timeZone : 'America/Chicago'
     * }
     * ```
     * @prp {String|Number} timeZone
     * @category Misc
     */
    timeZone: null
  };
  static get defaultConfig() {
    return {
      /**
       * A valid JS day index between 0-6 (0: Sunday, 1: Monday etc.) to be considered the start day of the week.
       * When omitted, the week start day is retrieved from the active locale class.
       * @config {Number} weekStartDay
       * @category Time axis
       */
      /**
       * An object with format `{ fromDay, toDay, fromHour, toHour }` that describes the working days and hours.
       * This object will be used to populate TimeAxis {@link Scheduler.data.TimeAxis#config-include} property.
       *
       * Using it results in a non-continuous time axis. Any ticks not covered by the working days and hours will
       * be excluded. Events within larger ticks (for example if using week as the unit for ticks) will be
       * stretched to fill the gap otherwise left by the non working hours.
       *
       * As with end dates, `toDay` and `toHour` are exclusive. Thus `toDay : 6` means that day 6 (saturday) will
       * not be included.
       *
       *
       * **NOTE:** When this feature is enabled {@link Scheduler.view.mixin.TimelineZoomable Zooming feature} is
       * not supported. It's recommended to disable zooming controls:
       *
       * ```javascript
       * new Scheduler({
       *     zoomOnMouseWheel          : false,
       *     zoomOnTimeAxisDoubleClick : false,
       *     ...
       * });
       * ```
       *
       * @config {Object}
       * @category Time axis
       */
      workingTime: null,
      /**
       * A backing data store of 'ticks' providing the input date data for the time axis of timeline panel.
       * @member {Scheduler.data.TimeAxis} timeAxis
       * @readonly
       * @category Time axis
       */
      /**
       * A {@link Scheduler.data.TimeAxis} config object or instance, used to create a backing data store of
       * 'ticks' providing the input date data for the time axis of timeline panel. Created automatically if none
       * supplied.
       * @config {TimeAxisConfig|Scheduler.data.TimeAxis}
       * @category Time axis
       */
      timeAxis: null,
      /**
       * The backing view model for the visual representation of the time axis.
       * Either a real instance or a simple config object.
       * @private
       * @config {Scheduler.view.model.TimeAxisViewModel|TimeAxisViewModelConfig}
       * @category Time axis
       */
      timeAxisViewModel: null,
      /**
       * You can set this option to `false` to make the timeline panel start and end on the exact provided
       * {@link #config-startDate}/{@link #config-endDate} w/o adjusting them.
       * @config {Boolean}
       * @default
       * @category Time axis
       */
      autoAdjustTimeAxis: true,
      /**
       * Affects drag drop and resizing of events when {@link Scheduler/view/mixin/TimelineDateMapper#config-snap}
       * is enabled.
       *
       * If set to `true`, dates will be snapped relative to event start. e.g. for a zoom level with
       * `timeResolution = { unit: "s", increment: "20" }`, an event that starts at 10:00:03 and is dragged would
       * snap its start date to 10:00:23, 10:00:43 etc.
       *
       * When set to `false`, dates will be snapped relative to the timeAxis startDate (tick start)
       * - 10:00:03 -> 10:00:20, 10:00:40 etc.
       *
       * @config {Boolean}
       * @default
       * @category Scheduled events
       */
      snapRelativeToEventStartDate: false,
      /**
       * Set to `true` to prevent auto calculating of a minimal {@link Scheduler.view.mixin.TimelineEventRendering#property-tickSize}
       * to always fit the content to the screen size. Setting this property on `true` will disable {@link #config-forceFit} behaviour.
       * @config {Boolean}
       * @default false
       * @category Time axis
       */
      suppressFit: false,
      /**
       * CSS class to add to cells in the timeaxis column
       * @config {String}
       * @category CSS
       * @private
       */
      timeCellCls: null,
      scheduledEventName: null,
      //dblClickTime : 200,
      /**
       * A CSS class to apply to each event in the view on mouseover.
       * @config {String}
       * @category CSS
       * @private
       */
      overScheduledEventClass: null,
      // allow the panel to prevent adding the hover CSS class in some cases - during drag drop operations
      preventOverCls: false,
      // This setting is set to true by features that need it
      useBackgroundCanvas: false,
      /**
       * Set to `false` if you don't want event bar DOM updates to animate.
       * @prp {Boolean}
       * @default true
       * @category Scheduled events
       */
      enableEventAnimations: true,
      disableGridRowModelWarning: true,
      // does not look good with locked columns and also interferes with event animations
      animateRemovingRows: false,
      /**
       * Partners this Timeline panel with another Timeline in order to sync their region sizes (sub-grids like locked, normal will get the same width),
       * start and end dates, view preset, zoom level and scrolling position. All these values will be synced with the timeline defined as the `partner`.
       *
       * - To add a new partner dynamically see {@link #function-addPartner} method.
       * - To remove existing partner see {@link #function-removePartner} method.
       * - To check if timelines are partners see {@link #function-isPartneredWith} method.
       *
       * Column widths and hide/show state are synced between partnered schedulers when the column set is identical.
       * @config {Scheduler.view.TimelineBase}
       * @category Time axis
       */
      partner: null,
      schedulerRegion: 'normal',
      transitionDuration: 200,
      // internal timer id reference
      animationTimeout: null,
      /**
       * Region to which columns are added when they have none specified
       * @config {String}
       * @default
       * @category Misc
       */
      defaultRegion: 'locked',
      /**
       * Decimal precision used when displaying durations, used by tooltips and DurationColumn.
       * Specify `false` to use raw value
       * @config {Number|Boolean}
       * @default
       * @category Common
       */
      durationDisplayPrecision: 1,
      asyncEventSuffix: 'PreCommit',
      viewportResizeTimeout: 250,
      /**
       * An object with configuration for the {@link Scheduler.column.TimeAxisColumn} in horizontal
       * {@link Scheduler.view.SchedulerBase#config-mode}.
       *
       * Example:
       *
       * ```javascript
       * new Scheduler({
       *     timeAxisColumn : {
       *         renderer : ({ record, cellElement }) => {
       *             // output some markup as a layer below the events layer, you can draw a chart for example
       *         }
       *     },
       *     ...
       * });
       * ```
       *
       * @config {TimeAxisColumnConfig} timeAxisColumn
       * @category Time axis
       */
      testConfig: {
        viewportResizeTimeout: 50
      }
    };
  }
  timeCellSelector = null;
  updateTimeZone(timeZone) {
    if (this.isConfiguring) {
      this.project._isConfiguringTimeZone = true;
    }
    this.project.timeZone = timeZone;
  }
  get timeZone() {
    return this.project.timeZone;
  }
  //endregion
  //region Feature hooks
  /**
   * Populates the event context menu. Chained in features to add menu items.
   * @param {Object} options Contains menu items and extra data retrieved from the menu target.
   * @param {Grid.column.Column} options.column Column for which the menu will be shown.
   * @param {Scheduler.model.EventModel} options.eventRecord The context event.
   * @param {Scheduler.model.ResourceModel} options.resourceRecord The context resource.
   * @param {Scheduler.model.AssignmentModel} options.assignmentRecord The context assignment if any.
   * @param {Object<String,MenuItemConfig|Boolean|null>} options.items A named object to describe menu items.
   * @internal
   */
  populateEventMenu() {}
  /**
   * Populates the time axis context menu. Chained in features to add menu items.
   * @param {Object} options Contains menu items and extra data retrieved from the menu target.
   * @param {Grid.column.Column} options.column Column for which the menu will be shown.
   * @param {Scheduler.model.ResourceModel} options.resourceRecord The context resource.
   * @param {Date} options.date The Date corresponding to the mouse position in the time axis.
   * @param {Object<String,MenuItemConfig|Boolean|null>} options.items A named object to describe menu items.
   * @internal
   */
  populateScheduleMenu() {}
  // Called when visible date range potentially changes such as when scrolling in
  // the time axis.
  onVisibleDateRangeChange(range) {
    if (!this.handlingVisibleDateRangeChange) {
      const me = this,
        {
          _visibleDateRange
        } = me,
        dateRangeChange = !_visibleDateRange || _visibleDateRange.startDate - range.startDate || _visibleDateRange.endDate - range.endDate;
      if (dateRangeChange) {
        me.timeView.range = range;
        me.handlingVisibleDateRangeChange = true;
        /**
         * Fired when the range of dates visible within the viewport changes. This will be when
         * scrolling along a time axis.
         *
         * __Note__ that this event will fire frequently during scrolling, so any listener
         * should probably be added with the `buffer` option to slow down the calls to your
         * handler function :
         *
         * ```javascript
         * listeners : {
         *     visibleDateRangeChange({ old, new }) {
         *         this.updateRangeRequired(old, new);
         *     },
         *     // Only call once. 300 ms after the last event was detected
         *     buffer : 300
         * }
         * ```
         * @event visibleDateRangeChange
         * @param {Scheduler.view.Scheduler} source This Scheduler instance.
         * @param {Object} old The old date range
         * @param {Date} old.startDate the old start date.
         * @param {Date} old.endDate the old end date.
         * @param {Object} new The new date range
         * @param {Date} new.startDate the new start date.
         * @param {Date} new.endDate the new end date.
         */
        me.trigger('visibleDateRangeChange', {
          old: _visibleDateRange,
          new: range
        });
        me.handlingVisibleDateRangeChange = false;
        me._visibleDateRange = range;
      }
    }
  }
  // Called when visible resource range changes in vertical mode
  onVisibleResourceRangeChange() {}
  //endregion
  //region Init
  construct(config = {}) {
    const me = this;
    super.construct(config);
    me.$firstVerticalOverflow = true;
    me.initDomEvents();
    me.currentOrientation.init();
    me.rowManager.ion({
      refresh: () => {
        me.forceLayout = false;
      }
    });
  }
  // Override from Grid.view.GridSubGrids
  createSubGrid(region, config = {}) {
    const me = this,
      {
        stickyHeaders
      } = me;
    // We are creating the TimeAxisSubGrid
    if (region === (me.schedulerRegion || 'normal')) {
      config.type = 'timeaxissubgrid';
    }
    // The assumption is that if we are in vertical mode, the locked SubGrid
    // is used to house the verticalTimeAxis, and so it must all be overflow:visible
    else if (region === 'locked' && stickyHeaders && me.isVertical) {
      config.scrollable = {
        overflowX: 'visible',
        overflowY: 'visible'
      };
      // It's the child of the overflowElement
      me.bodyContainer.classList.add('b-sticky-headers');
    }
    return super.createSubGrid(region, config);
  }
  doDestroy() {
    const me = this,
      {
        partneredWith,
        currentOrientation
      } = me;
    currentOrientation === null || currentOrientation === void 0 ? void 0 : currentOrientation.destroy();
    // Break links between this TimeLine and any partners.
    if (partneredWith) {
      partneredWith.forEach(p => {
        me.removePartner(p);
      });
      partneredWith.destroy();
    } else {
      me.timeAxisViewModel.destroy();
      me.timeAxis.destroy();
    }
    super.doDestroy();
  }
  startConfigure(config) {
    super.startConfigure(config);
    // When the body height changes, we must update the SchedulerViewport's height
    ResizeMonitor.addResizeListener(this.bodyContainer, this.onBodyResize.bind(this));
    // partner needs to be initialized first so that the various shared
    // configs are assigned first before we default them in.
    this.getConfig('partner');
  }
  changeStartDate(startDate) {
    if (typeof startDate === 'string') {
      startDate = DateHelper.parse(startDate);
    }
    return startDate;
  }
  onPaint({
    firstPaint
  }) {
    // Upon first paint we need to pass the forceUpdate flag in case we are sharing the TimAxisViewModel
    // with another Timeline which will already have done this.
    if (firstPaint) {
      // Take height from container element
      const me = this,
        scrollable = me.isHorizontal ? me.timeAxisSubGrid.scrollable : me.scrollable,
        // Use exact subpixel available space so that tick size calculation is correct.
        availableSpace = scrollable.element.getBoundingClientRect()[me.isHorizontal ? 'width' : 'height'];
      // silent = true if infiniteScroll. If that is set, TimelineScroll.initScroll which is
      // called by the base class's onPaint reconfigures the TAVM when it initializes.
      me.timeAxisViewModel.update(availableSpace, me.infiniteScroll, true);
      // If infiniteScroll caused the TAVM update to be silent, force the rendering to
      // get hold of the scroll state and visible range
      if (me.infiniteScroll) {
        var _me$currentOrientatio, _me$currentOrientatio2;
        (_me$currentOrientatio = (_me$currentOrientatio2 = me.currentOrientation).doUpdateTimeView) === null || _me$currentOrientatio === void 0 ? void 0 : _me$currentOrientatio.call(_me$currentOrientatio2);
      }
    }
    super.onPaint(...arguments);
  }
  onSchedulerHorizontalScroll(subGrid, scrollLeft, scrollX) {
    // rerender cells in scheduler column on horizontal scroll to display events in view
    this.currentOrientation.updateFromHorizontalScroll(scrollX);
    super.onSchedulerHorizontalScroll(subGrid, scrollLeft, scrollX);
  }
  /**
   * Overrides initScroll from Grid, listens for horizontal scroll to do virtual event rendering
   * @private
   */
  initScroll() {
    const me = this;
    let frameCount = 0;
    super.initScroll();
    me.ion({
      horizontalScroll: ({
        subGrid,
        scrollLeft,
        scrollX
      }) => {
        if (me.isPainted && subGrid === me.timeAxisSubGrid && !me.isDestroying && !me.refreshSuspended) {
          me.onSchedulerHorizontalScroll(subGrid, scrollLeft, scrollX);
        }
        frameCount++;
      }
    });
    if (me.testPerformance === 'horizontal') {
      me.setTimeout(() => {
        const start = performance.now();
        let scrollSpeed = 5,
          direction = 1;
        const scrollInterval = me.setInterval(() => {
          scrollSpeed = scrollSpeed + 5;
          me.scrollX += (10 + Math.floor(scrollSpeed)) * direction;
          if (direction === 1 && me.scrollX > 5500) {
            direction = -1;
            scrollSpeed = 5;
          }
          if (direction === -1 && me.scrollX <= 0) {
            const done = performance.now(),
              // eslint-disable-line no-undef
              elapsed = done - start;
            const timePerFrame = elapsed / frameCount,
              fps = Math.round(1000 / timePerFrame * 10) / 10;
            clearInterval(scrollInterval);
            console.log(me.eventPositionMode, me.eventScrollMode, fps + 'fps');
          }
        }, 0);
      }, 500);
    }
  }
  //endregion
  /**
   * Calls the specified function (returning its return value) and preserves the timeline center
   * point. This is a useful way of retaining the user's visual context while making updates
   * and changes to the view which require major changes or a full refresh.
   * @param {Function} fn The function to call.
   * @param {Object} thisObj The `this` context for the function.
   * @param {...*} args Parameters to the function.
   */
  preserveViewCenter(fn, thisObj = this, ...args) {
    const me = this,
      centerDate = me.viewportCenterDate,
      result = fn.apply(thisObj, args),
      scroller = me.timelineScroller,
      {
        clientSize
      } = scroller,
      scrollStart = Math.max(Math.floor(me.getCoordinateFromDate(centerDate, true) - clientSize / 2), 0);
    me.scrollingToCenter = true;
    scroller.scrollTo(scrollStart, false).then(() => me.scrollingToCenter = false);
    return result;
  }
  /**
   * Changes this Scheduler's time axis timespan to the supplied start and end dates.
   *
   * @async
   * @param {Date} newStartDate The new start date
   * @param {Date} newEndDate The new end date
   * @param {Object} [options] An object containing modifiers for the time span change operation.
   * @param {Boolean} [options.maintainVisibleStart] Specify as `true` to keep the visible start date stable.
   * @param {Date} [options.visibleDate] The date inside the range to scroll into view
   */
  setTimeSpan(newStartDate, newEndDate, options = emptyObject) {
    const me = this,
      {
        timeAxis
      } = me,
      {
        preventThrow = false,
        // Private, only used by the shift method.
        maintainVisibleStart = false,
        visibleDate
      } = options,
      {
        startDate,
        endDate
      } = timeAxis.getAdjustedDates(newStartDate, newEndDate),
      startChanged = timeAxis.startDate - startDate !== 0,
      endChanged = timeAxis.endDate - endDate !== 0;
    if (startChanged || endChanged) {
      if (maintainVisibleStart) {
        const {
            timeAxisViewModel
          } = me,
          {
            totalSize
          } = timeAxisViewModel,
          oldTickSize = timeAxisViewModel.tickSize,
          scrollable = me.timelineScroller,
          currentScroll = scrollable.position,
          visibleStart = timeAxisViewModel.getDateFromPosition(currentScroll);
        // If the current visibleStart is in the new range, maintain it
        // So that there is no visual jump.
        if (visibleStart >= startDate && visibleStart < endDate) {
          // We need to correct the scroll position as soon as the TimeAxisViewModel
          // has updated itself and before any other UI updates which that may trigger.
          timeAxisViewModel.ion({
            update() {
              const tickSizeChanged = timeAxisViewModel.tickSize !== oldTickSize;
              // Ensure the canvas element matches the TimeAxisViewModel's new totalSize.
              // This creates the required scroll range to be able to have the scroll
              // position correct before any further UI updates.
              me.updateCanvasSize();
              // If *only* the start moved, we can keep scroll position the same
              // by adjusting it by the amount the start moved.
              if (startChanged && !endChanged && !tickSizeChanged) {
                scrollable.position += timeAxisViewModel.totalSize - totalSize;
              }
              // If only the end has changed, and tick size is same, we can maintain
              // the same scroll position.
              else if (!startChanged && !tickSizeChanged) {
                scrollable.position = currentScroll;
              }
              // Fall back to restoring the position by restoring the visible start time
              else {
                scrollable.position = timeAxisViewModel.getPositionFromDate(visibleStart);
              }
              // Force partners to sync with what we've just done to reset the scroll.
              // We are now in control.
              scrollable.syncPartners(true);
            },
            prio: 10000,
            once: true
          });
        }
      }
      const returnValue = timeAxis.reconfigure({
        startDate,
        endDate
      }, false, preventThrow);
      if (visibleDate) {
        return me.scrollToDate(visibleDate, options).then(() => returnValue);
      }
      return returnValue;
    }
  }
  //region Config getters/setters
  /**
   * Returns `true` if any of the events/tasks or feature injected elements (such as ResourceTimeRanges) are within
   * the {@link #config-timeAxis}
   * @property {Boolean}
   * @readonly
   * @category Scheduled events
   */
  get hasVisibleEvents() {
    return !this.noFeatureElementsInAxis() || this.eventStore.storage.values.some(t => this.timeAxis.isTimeSpanInAxis(t));
  }
  // Template function to be chained in features to determine if any elements are in time axis (needed since we cannot
  // currently chain getters). Negated to not break chain. First feature that has elements visible returns false,
  // which prevents other features from being queried.
  noFeatureElementsInAxis() {}
  // Private getter used to piece together event names such as beforeEventDrag / beforeTaskDrag. Could also be used
  // in templates.
  get capitalizedEventName() {
    if (!this._capitalizedEventName) {
      this._capitalizedEventName = StringHelper.capitalize(this.scheduledEventName);
    }
    return this._capitalizedEventName;
  }
  set partner(partner) {
    this._partner = partner;
    this.addPartner(partner);
  }
  /**
   * Partners this Timeline with the passed Timeline in order to sync the horizontal scrolling position and zoom level.
   *
   * - To remove existing partner see {@link #function-removePartner} method.
   * - To get the list of partners see {@link #property-partners} getter.
   *
   * @param {Scheduler.view.TimelineBase} otherTimeline The timeline to partner with
   */
  addPartner(partner) {
    const me = this;
    if (!me.isPartneredWith(partner)) {
      const partneredWith = me.partneredWith || (me.partneredWith = new Collection());
      // Each must know about the other so that they can sync others upon region resize
      partneredWith.add(partner);
      (partner.partneredWith || (partner.partneredWith = new Collection())).add(me);
      // Flush through viewPreset initGetter so that the setup in setConfig doesn't
      // take them to be the class's defined getters.
      me.getConfig('viewPreset');
      partner.ion({
        presetchange: 'onPartnerPresetChange',
        thisObj: me
      });
      partner.scrollable.ion({
        overflowChange: 'onPartnerOverflowChange',
        thisObj: me
      });
      // collect configs that are meant to be shared between partners
      const partnerSharedConfig = me.partnerSharedConfigs.reduce((config, configName) => {
        config[configName] = partner[configName];
        return config;
      }, {});
      me.setConfig(partnerSharedConfig);
      me.ion({
        presetchange: 'onPartnerPresetChange',
        thisObj: partner
      });
      me.scrollable.ion({
        overflowChange: 'onPartnerOverflowChange',
        thisObj: partner
      });
      if (me.isPainted) {
        me.scrollable.addPartner(partner.scrollable, me.isHorizontal ? 'x' : 'y');
        partner.syncPartnerSubGrids();
      } else {
        // When initScroll comes round, make sure it syncs with the partner
        me.initScroll = FunctionHelper.createSequence(me.initScroll, () => {
          me.scrollable.addPartner(partner.scrollable, me.isHorizontal ? 'x' : 'y');
          partner.syncPartnerSubGrids();
        }, me);
      }
    }
  }
  /**
   * Breaks the link between current Timeline and the passed Timeline
   *
   * - To add a new partner see {@link #function-addPartner} method.
   * - To get the list of partners see {@link #property-partners} getter.
   *
   * @param {Scheduler.view.TimelineBase} otherTimeline The timeline to unlink from
   */
  removePartner(partner) {
    const me = this,
      {
        partneredWith
      } = me;
    if (me.isPartneredWith(partner)) {
      partneredWith.remove(partner);
      me.scrollable.removePartner(partner.scrollable);
      me.un({
        presetchange: 'onPartnerPresetChange',
        thisObj: partner
      });
      me.scrollable.un({
        overflowChange: 'onPartnerOverflowChange',
        thisObj: partner
      });
      partner.removePartner(me);
    }
  }
  /**
   * Checks whether the passed timeline is partnered with the current timeline.
   * @param {Scheduler.view.TimelineBase} partner The timeline to check the partnering with
   * @returns {Boolean} Returns `true` if the timelines are partnered
   */
  isPartneredWith(partner) {
    var _this$partneredWith;
    return Boolean((_this$partneredWith = this.partneredWith) === null || _this$partneredWith === void 0 ? void 0 : _this$partneredWith.includes(partner));
  }
  /**
   * Called when a partner scheduler changes its overflowing state. The scrollable
   * of a Grid/Scheduler only handles overflowY, so this will mean the addition
   * or removal of a vertical scrollbar.
   *
   * All partners must stay in sync. If another parter has a vertical scrollbar
   * and we do not, we must set our overflowY to 'scroll' so that we show an empty
   * scrollbar to keep widths synchronized.
   * @param {Object} event A {@link Core.helper.util.Scroller#event-overflowChange} event
   * @internal
   */
  onPartnerOverflowChange({
    source: otherScrollable,
    y
  }) {
    const {
        scrollable
      } = this,
      ourY = scrollable.hasOverflow('y');
    // If we disagree with our partner, the partner which doesn't have
    // overflow, has to become overflowY : scroll
    if (ourY !== y) {
      if (ourY) {
        otherScrollable.overflowY = 'scroll';
      } else {
        otherScrollable.overflowY = true;
        scrollable.overflowY = 'scroll';
        this.refreshVirtualScrollbars();
      }
    }
    // If we agree with our partner, we can reset ourselves to overflowY : auto
    else {
      scrollable.overflowY = true;
    }
  }
  onPartnerPresetChange({
    preset,
    startDate,
    endDate,
    centerDate,
    zoomDate,
    zoomPosition,
    zoomLevel
  }) {
    if (!this._viewPresetChanging && this.viewPreset !== preset) {
      // Passed through to the viewPreset changing method
      preset.options = {
        startDate,
        endDate,
        centerDate,
        zoomDate,
        zoomPosition,
        zoomLevel
      };
      this.viewPreset = preset;
    }
  }
  get partner() {
    return this._partner;
  }
  /**
   * Returns the partnered timelines.
   *
   * - To add a new partner see {@link #function-addPartner} method.
   * - To remove existing partner see {@link #function-removePartner} method.
   *
   * @readonly
   * @member {Scheduler.view.TimelineBase} partners
   * @category Time axis
   */
  get partners() {
    const partners = this.partner ? [this.partner] : [];
    if (this.partneredWith) {
      partners.push.apply(partners, this.partneredWith.allValues);
    }
    return partners;
  }
  get timeAxisColumn() {
    return this.columns && this._timeAxisColumn;
  }
  changeColumns(columns, currentStore) {
    const me = this;
    let timeAxisColumnIndex, timeAxisColumnConfig;
    // No columns means destroy
    if (columns) {
      const isArray = Array.isArray(columns);
      let cols = columns;
      if (!isArray) {
        cols = columns.data;
      }
      timeAxisColumnIndex = cols && cols.length;
      cols.some((col, index) => {
        if (col.type === 'timeAxis') {
          timeAxisColumnIndex = index;
          timeAxisColumnConfig = ObjectHelper.assign(col, me.timeAxisColumn);
          return true;
        }
        return false;
      });
      if (me.isVertical) {
        cols = [ObjectHelper.assign({
          type: 'verticalTimeAxis'
        }, me.verticalTimeAxisColumn),
        // Make space for a regular TimeAxisColumn after the VerticalTimeAxisColumn
        cols[timeAxisColumnIndex]];
        timeAxisColumnIndex = 1;
      } else {
        // We're going to mutate this array which we do not own, so copy it first.
        cols = cols.slice();
      }
      // Fix up the timeAxisColumn config in place
      cols[timeAxisColumnIndex] = this._timeAxisColumn || {
        type: 'timeAxis',
        cellCls: me.timeCellCls,
        mode: me.mode,
        ...timeAxisColumnConfig
      };
      // If we are passed a raw array, or the Store we are passed is owned by another
      // Scheduler, pass the raw column data ro the Grid's changeColumns
      if (isArray || columns.isStore && columns.owner !== this) {
        columns = cols;
      } else {
        columns.data = cols;
      }
    }
    return super.changeColumns(columns, currentStore);
  }
  updateColumns(columns, was) {
    super.updateColumns(columns, was);
    // Extract the known columns by type. Sorting will have placed them into visual order.
    if (columns) {
      const me = this,
        timeAxisColumn = me._timeAxisColumn = me.columns.find(c => c.isTimeAxisColumn);
      if (me.isVertical) {
        me.verticalTimeAxisColumn = me.columns.find(c => c.isVerticalTimeAxisColumn);
        me.verticalTimeAxisColumn.relayAll(me);
      }
      // Set up event relaying early
      timeAxisColumn.relayAll(me);
    }
  }
  onColumnsChanged({
    action,
    changes,
    record: column,
    records
  }) {
    var _this$partneredWith2;
    const {
      timeAxisColumn,
      columns
    } = this;
    // If someone replaces the column set (syncing leads to batch), ensure time axis is always added
    if ((action === 'dataset' || action === 'batch') && !columns.includes(timeAxisColumn)) {
      columns.add(timeAxisColumn, true);
    } else if (column === timeAxisColumn && 'width' in changes) {
      this.updateCanvasSize();
    }
    column && ((_this$partneredWith2 = this.partneredWith) === null || _this$partneredWith2 === void 0 ? void 0 : _this$partneredWith2.forEach(partner => {
      const partnerColumn = partner.columns.getAt(column.allIndex);
      if (partnerColumn !== null && partnerColumn !== void 0 && partnerColumn.shouldSync(column)) {
        const partnerChanges = {};
        for (const k in changes) {
          partnerChanges[k] = changes[k].value;
        }
        partnerColumn.set(partnerChanges);
      }
    }));
    super.onColumnsChanged(...arguments);
  }
  get timeView() {
    var _me$verticalTimeAxisC, _me$timeAxisColumn;
    const me = this;
    // Maintainer, we need to ensure that the columns property is initialized
    // if this getter is called at configuration time before columns have been ingested.
    return me.columns && me.isVertical ? (_me$verticalTimeAxisC = me.verticalTimeAxisColumn) === null || _me$verticalTimeAxisC === void 0 ? void 0 : _me$verticalTimeAxisC.view : (_me$timeAxisColumn = me.timeAxisColumn) === null || _me$timeAxisColumn === void 0 ? void 0 : _me$timeAxisColumn.timeAxisView;
  }
  updateEventCls(eventCls) {
    const me = this;
    if (!me.eventSelector) {
      // No difference with new rendering, released have 'b-released' only
      me.unreleasedEventSelector = me.eventSelector = `.${eventCls}-wrap`;
    }
    if (!me.eventInnerSelector) {
      me.eventInnerSelector = `.${eventCls}`;
    }
  }
  set timeAxisViewModel(timeAxisViewModel) {
    var _timeAxisViewModel;
    const me = this,
      currentModel = me._timeAxisViewModel,
      tavmListeners = {
        name: 'timeAxisViewModel',
        update: 'onTimeAxisViewModelUpdate',
        prio: 100,
        thisObj: me
      };
    if (me.partner && !timeAxisViewModel || currentModel && currentModel === timeAxisViewModel) {
      return;
    }
    if ((currentModel === null || currentModel === void 0 ? void 0 : currentModel.owner) === me) {
      // We created this model, destroy it
      currentModel.destroy();
    }
    me.detachListeners('timeAxisViewModel');
    // Getting rid of instanceof check to allow using code from different bundles
    if ((_timeAxisViewModel = timeAxisViewModel) !== null && _timeAxisViewModel !== void 0 && _timeAxisViewModel.isTimeAxisViewModel) {
      timeAxisViewModel.ion(tavmListeners);
    } else {
      timeAxisViewModel = TimeAxisViewModel.new({
        mode: me._mode,
        snap: me.snap,
        forceFit: me.forceFit,
        timeAxis: me.timeAxis,
        suppressFit: me.suppressFit,
        internalListeners: tavmListeners,
        owner: me
      }, timeAxisViewModel);
    }
    // Replace in dependent classes relying on the model
    if (!me.isConfiguring) {
      if (me.isHorizontal) {
        me.timeAxisColumn.timeAxisViewModel = timeAxisViewModel;
      } else {
        me.verticalTimeAxisColumn.view.model = timeAxisViewModel;
      }
    }
    me._timeAxisViewModel = timeAxisViewModel;
    me.relayEvents(timeAxisViewModel, ['update'], 'timeAxisViewModel');
    if (currentModel && timeAxisViewModel) {
      me.trigger('timeAxisViewModelChange', {
        timeAxisViewModel
      });
    }
  }
  /**
   * The internal view model, describing the visual representation of the time axis.
   * @property {Scheduler.view.model.TimeAxisViewModel}
   * @readonly
   * @category Time axis
   */
  get timeAxisViewModel() {
    if (!this._timeAxisViewModel) {
      this.timeAxisViewModel = null;
    }
    return this._timeAxisViewModel;
  }
  get suppressFit() {
    var _this$_timeAxisViewMo;
    return ((_this$_timeAxisViewMo = this._timeAxisViewModel) === null || _this$_timeAxisViewMo === void 0 ? void 0 : _this$_timeAxisViewMo.suppressFit) ?? this._suppressFit;
  }
  set suppressFit(value) {
    if (this._timeAxisViewModel) {
      this.timeAxisViewModel.suppressFit = value;
    } else {
      this._suppressFit = value;
    }
  }
  set timeAxis(timeAxis) {
    var _timeAxis;
    const me = this,
      currentTimeAxis = me._timeAxis,
      timeAxisListeners = {
        name: 'timeAxis',
        reconfigure: 'onTimeAxisReconfigure',
        thisObj: me
      };
    if (me.partner && !timeAxis || currentTimeAxis && currentTimeAxis === timeAxis) {
      return;
    }
    if (currentTimeAxis) {
      if (currentTimeAxis.owner === me) {
        // We created this model, destroy it
        currentTimeAxis.destroy();
      }
    }
    me.detachListeners('timeAxis');
    // Getting rid of instanceof check to allow using code from different bundles
    if (!((_timeAxis = timeAxis) !== null && _timeAxis !== void 0 && _timeAxis.isTimeAxis)) {
      timeAxis = ObjectHelper.assign({
        owner: me,
        viewPreset: me.viewPreset,
        autoAdjust: me.autoAdjustTimeAxis,
        weekStartDay: me.weekStartDay,
        forceFullTicks: me.fillTicks && me.snap
      }, timeAxis);
      if (me.startDate) {
        timeAxis.startDate = me.startDate;
      }
      if (me.endDate) {
        timeAxis.endDate = me.endDate;
      }
      if (me.workingTime) {
        me.applyWorkingTime(timeAxis);
      }
      timeAxis = new TimeAxis(timeAxis);
    }
    // Inform about reconfiguring the timeaxis, to allow users to react to start & end date changes
    timeAxis.ion(timeAxisListeners);
    me._timeAxis = timeAxis;
  }
  onTimeAxisReconfigure({
    config,
    oldConfig
  }) {
    if (config) {
      const dateRangeChange = !oldConfig || oldConfig.startDate - config.startDate || oldConfig.endDate - config.endDate;
      if (dateRangeChange) {
        /**
         * Fired when the range of dates encapsulated by the UI changes. This will be when
         * moving a view in time by reconfiguring its {@link #config-timeAxis}. This will happen
         * when zooming, or changing {@link #config-viewPreset}.
         *
         * Contrast this with the {@link #event-visibleDateRangeChange} event which fires much
         * more frequently, during scrolling along the time axis and changing the __visible__
         * date range.
         * @event dateRangeChange
         * @param {Scheduler.view.TimelineBase} source This Scheduler/Gantt instance.
         * @param {Object} old The old date range
         * @param {Date} old.startDate the old start date.
         * @param {Date} old.endDate the old end date.
         * @param {Object} new The new date range
         * @param {Date} new.startDate the new start date.
         * @param {Date} new.endDate the new end date.
         */
        this.trigger('dateRangeChange', {
          old: {
            startDate: oldConfig.startDate,
            endDate: oldConfig.endDate
          },
          new: {
            startDate: config.startDate,
            endDate: config.endDate
          }
        });
      }
    }
    /**
     * Fired when the timeaxis has changed, for example by zooming or configuring a new time span.
     * @event timeAxisChange
     * @param {Scheduler.view.Scheduler} source - This Scheduler
     * @param {Object} config Config object used to reconfigure the time axis.
     * @param {Date} config.startDate New start date (if supplied)
     * @param {Date} config.endDate New end date (if supplied)
     */
    this.trigger('timeAxisChange', {
      config
    });
  }
  get timeAxis() {
    if (!this._timeAxis) {
      this.timeAxis = null;
    }
    return this._timeAxis;
  }
  updateForceFit(value) {
    if (this._timeAxisViewModel) {
      this._timeAxisViewModel.forceFit = value;
    }
  }
  /**
   * Get/set working time. Assign `null` to stop using working time. See {@link #config-workingTime} config for details.
   * @property {Object}
   * @category Scheduled events
   */
  set workingTime(config) {
    this._workingTime = config;
    if (!this.isConfiguring) {
      this.applyWorkingTime(this.timeAxis);
    }
  }
  get workingTime() {
    return this._workingTime;
  }
  // Translates the workingTime configs into TimeAxis#include rules, applies them and then refreshes the header and
  // redraws the events
  applyWorkingTime(timeAxis) {
    const me = this,
      config = me._workingTime;
    if (config) {
      let hour = null;
      // Only use valid values
      if (config.fromHour >= 0 && config.fromHour < 24 && config.toHour > config.fromHour && config.toHour <= 24 && config.toHour - config.fromHour < 24) {
        hour = {
          from: config.fromHour,
          to: config.toHour
        };
      }
      let day = null;
      // Only use valid values
      if (config.fromDay >= 0 && config.fromDay < 7 && config.toDay > config.fromDay && config.toDay <= 7 && config.toDay - config.fromDay < 7) {
        day = {
          from: config.fromDay,
          to: config.toDay
        };
      }
      if (hour || day) {
        timeAxis.include = {
          hour,
          day
        };
      } else {
        // No valid rules, restore timeAxis
        timeAxis.include = null;
      }
    } else {
      // No rules, restore timeAxis
      timeAxis.include = null;
    }
    if (me.isPainted) {
      var _me$features$columnLi;
      // Refreshing header, which also recalculate tickSize and header data
      me.timeAxisColumn.refreshHeader();
      // Update column lines
      (_me$features$columnLi = me.features.columnLines) === null || _me$features$columnLi === void 0 ? void 0 : _me$features$columnLi.refresh();
      // Animate event changes
      me.refreshWithTransition();
    }
  }
  updateStartDate(date) {
    this.setStartDate(date);
  }
  /**
   * Sets the timeline start date.
   *
   * **Note:**
   * - If you need to set start and end date at the same time, use the {@link #function-setTimeSpan} method.
   * - If keepDuration is false and new start date is greater than end date, it will throw an exception.
   *
   * @param {Date} date The new start date
   * @param {Boolean} keepDuration Pass `true` to keep the duration of the timeline ("move" the timeline),
   * `false` to change the duration ("resize" the timeline). Defaults to `true`.
   */
  setStartDate(date, keepDuration = true) {
    const me = this,
      ta = me._timeAxis,
      {
        startDate,
        endDate,
        mainUnit
      } = ta || emptyObject;
    if (typeof date === 'string') {
      date = DateHelper.parse(date);
    }
    if (ta && endDate) {
      if (date) {
        let calcEndDate = endDate;
        if (keepDuration && startDate) {
          const diff = DateHelper.diff(startDate, endDate, mainUnit, true);
          calcEndDate = DateHelper.add(date, diff, mainUnit);
        }
        me.setTimeSpan(date, calcEndDate);
      }
    } else {
      me._tempStartDate = date;
    }
  }
  get startDate() {
    const me = this;
    if (me._timeAxis) {
      return me._timeAxis.startDate;
    }
    return me._tempStartDate || new Date();
  }
  changeEndDate(date) {
    if (typeof date === 'string') {
      date = DateHelper.parse(date);
    }
    this.setEndDate(date);
  }
  /**
   * Sets the timeline end date
   *
   * **Note:**
   * - If you need to set start and end date at the same time, use the {@link #function-setTimeSpan} method.
   * - If keepDuration is false and new end date is less than start date, it will throw an exception.
   *
   * @param {Date} date The new end date
   * @param {Boolean} keepDuration Pass `true` to keep the duration of the timeline ("move" the timeline),
   * `false` to change the duration ("resize" the timeline). Defaults to `false`.
   */
  setEndDate(date, keepDuration = false) {
    const me = this,
      ta = me._timeAxis,
      {
        startDate,
        endDate,
        mainUnit
      } = ta || emptyObject;
    if (typeof date === 'string') {
      date = DateHelper.parse(date);
    }
    if (ta && startDate) {
      if (date) {
        let calcStartDate = startDate;
        if (keepDuration && endDate) {
          const diff = DateHelper.diff(startDate, endDate, mainUnit, true);
          calcStartDate = DateHelper.add(date, -diff, mainUnit);
        }
        me.setTimeSpan(calcStartDate, date);
      }
    } else {
      me._tempEndDate = date;
    }
  }
  get endDate() {
    const me = this;
    if (me._timeAxis) {
      return me._timeAxis.endDate;
    }
    return me._tempEndDate || DateHelper.add(me.startDate, me.viewPreset.defaultSpan, me.viewPreset.mainHeader.unit);
  }
  changeVisibleDate(options) {
    if (options instanceof Date) {
      return {
        date: options,
        block: 'nearest'
      };
    }
    if (options instanceof Object) {
      return {
        date: options.date,
        ...options
      };
    }
  }
  updateVisibleDate(options) {
    const me = this;
    // Infinite scroll initialization takes care of its visibleDate after
    // calculating the optimum scroll range in TimelineScroll#initScroll
    if (!(me.infiniteScroll && me.isConfiguring)) {
      if (me.isPainted) {
        me.scrollToDate(options.date, options);
      } else {
        me.ion({
          paint: () => me.scrollToDate(options.date, options),
          once: true
        });
      }
    }
  }
  get features() {
    return super.features;
  }
  // add region resize by default
  set features(features) {
    features = features === true ? {} : features;
    if (!('regionResize' in features)) {
      features.regionResize = true;
    }
    super.features = features;
  }
  get eventStyle() {
    return this._eventStyle;
  }
  set eventStyle(style) {
    this._eventStyle = style;
    this.refreshWithTransition();
    this.trigger('stateChange');
  }
  get eventColor() {
    return this._eventColor;
  }
  set eventColor(color) {
    this._eventColor = color;
    this.refreshWithTransition();
    this.trigger('stateChange');
  }
  //endregion
  //region Event handlers
  onLocaleChange() {
    super.onLocaleChange();
    const oldAutoAdjust = this.timeAxis.autoAdjust;
    // Time axis should rebuild as weekStartDay may have changed
    this.timeAxis.reconfigure({
      autoAdjust: false
    });
    // Silently set it back to what the user had for next view refresh
    this.timeAxis.autoAdjust = oldAutoAdjust;
  }
  /**
   * Called when the element which encapsulates the Scheduler's visible height changes size.
   * We only respond to *height* changes here. The TimeAxisSubGrid monitors its own width.
   * @param {HTMLElement} element
   * @param {DOMRect} oldRect
   * @param {DOMRect} newRect
   * @private
   */
  onBodyResize(element, oldRect, {
    width,
    height
  }) {
    // Uncache old value upon element resize, not upon initial sizing
    if (this.isVertical && oldRect && width !== oldRect.width) {
      delete this.timeAxisSubGrid._width;
    }
    const newWidth = this.timeAxisSubGrid.element.offsetWidth;
    // The Scheduler (The Grid) dictates the viewport height.
    // Don't react on first invocation which will be initial size.
    if (this._bodyRectangle && oldRect && height !== oldRect.height) {
      this.onSchedulerViewportResize(newWidth, height, newWidth, oldRect.height);
    }
  }
  // Note: This function is throttled in construct(), since it will do a full redraw per call
  onSchedulerViewportResize(width, height, oldWidth, oldHeight) {
    if (this.isPainted) {
      const me = this,
        {
          isHorizontal,
          partneredWith
        } = me;
      me.currentOrientation.onViewportResize(width, height, oldWidth, oldHeight);
      // Raw width is always correct for horizontal layout because the TimeAxisSubGrid
      // never shows a scrollbar. It's always contained by an owning Grid which shows
      // the vertical scrollbar.
      me.updateViewModelAvailableSpace(isHorizontal ? width : Math.floor(height));
      if (partneredWith && !me.isSyncingFromPartner) {
        me.syncPartnerSubGrids();
      }
      /**
       * Fired when the *scheduler* viewport (not the overall Scheduler element) changes size.
       * This happens when the grid changes height, or when the subgrid which encapsulates the
       * scheduler column changes width.
       * @event timelineViewportResize
       * @param {Core.widget.Widget} source - This Scheduler
       * @param {Number} width The new width
       * @param {Number} height The new height
       * @param {Number} oldWidth The old width
       * @param {Number} oldHeight The old height
       */
      me.trigger('timelineViewportResize', {
        width,
        height,
        oldWidth,
        oldHeight
      });
    }
  }
  updateViewModelAvailableSpace(space) {
    this.timeAxisViewModel.availableSpace = space;
  }
  onTimeAxisViewModelUpdate() {
    if (!this._viewPresetChanging && !this.timeAxisSubGrid.collapsed) {
      this.updateCanvasSize();
      this.currentOrientation.onTimeAxisViewModelUpdate();
    }
  }
  syncPartnerSubGrids() {
    this.partneredWith.forEach(partner => {
      if (!partner.isSyncingFromPartner) {
        partner.isSyncingFromPartner = true;
        this.eachSubGrid(subGrid => {
          const partnerSubGrid = partner.subGrids[subGrid.region];
          // If there is a difference, sync the partner SubGrid state
          if (partnerSubGrid.width !== subGrid.width) {
            if (subGrid.collapsed) {
              partnerSubGrid.collapse();
            } else {
              if (partnerSubGrid.collapsed) {
                partnerSubGrid.expand();
              }
              // When using flexed subgrid, make sure flex values has prio over width
              if (subGrid.flex) {
                // If flex values match, resize should be fine without changing anything
                if (subGrid.flex !== partnerSubGrid.flex) {
                  partnerSubGrid.flex = subGrid.flex;
                }
              } else {
                partnerSubGrid.width = subGrid.width;
              }
            }
          }
        });
        partner.isSyncingFromPartner = false;
      }
    });
  }
  //endregion
  //region Mode
  get currentOrientation() {
    throw new Error('Implement in subclass');
  }
  // Horizontal is the default, overridden in scheduler
  get isHorizontal() {
    return true;
  }
  //endregion
  //region Canvases and elements
  get backgroundCanvas() {
    return this._backgroundCanvas;
  }
  get foregroundCanvas() {
    return this._foregroundCanvas;
  }
  get svgCanvas() {
    const me = this;
    if (!me._svgCanvas) {
      const svg = me._svgCanvas = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
      svg.setAttribute('id', IdHelper.generateId('svg'));
      // To not be recycled by DomSync
      svg.retainElement = true;
      me.foregroundCanvas.appendChild(svg);
      me.trigger('svgCanvasCreated', {
        svg
      });
    }
    return me._svgCanvas;
  }
  /**
   * Returns the subGrid containing the time axis
   * @member {Grid.view.SubGrid} timeAxisSubGrid
   * @readonly
   * @category Time axis
   */
  /**
   * Returns the html element for the subGrid containing the time axis
   * @property {HTMLElement}
   * @readonly
   * @category Time axis
   */
  get timeAxisSubGridElement() {
    // Hit a lot, caching the element (it will never change)
    if (!this._timeAxisSubGridElement) {
      var _this$timeAxisColumn;
      // We need the TimeAxisSubGrid to exist, so regions must be initialized
      this.getConfig('regions');
      this._timeAxisSubGridElement = (_this$timeAxisColumn = this.timeAxisColumn) === null || _this$timeAxisColumn === void 0 ? void 0 : _this$timeAxisColumn.subGridElement;
    }
    return this._timeAxisSubGridElement;
  }
  updateCanvasSize() {
    const me = this,
      {
        totalSize
      } = me.timeAxisViewModel,
      width = me.isHorizontal ? totalSize : me.timeAxisColumn.width;
    let result = false;
    if (me.isVertical) {
      // Ensure vertical scroll range accommodates the TimeAxis
      if (me.isPainted) {
        // We used to have a bug here from not including the row border in the total height. Border is now
        // removed, but leaving code here just in case some client is using border
        me.refreshTotalHeight(totalSize + me._rowBorderHeight, true);
      }
      // Canvas might need a height in vertical mode, if ticks does not fill height (suppressFit : true)
      if (me.suppressFit) {
        DomHelper.setLength(me.foregroundCanvas, 'height', totalSize);
      }
      result = true;
    }
    if (width !== me.$canvasWidth && me.foregroundCanvas) {
      if (me.backgroundCanvas) {
        DomHelper.setLength(me.backgroundCanvas, 'width', width);
      }
      DomHelper.setLength(me.foregroundCanvas, 'width', width);
      me.$canvasWidth = width;
      result = true;
    }
    return result;
  }
  /**
   * A chainable function which Features may hook to add their own content to the timeaxis header.
   * @param {Array} configs An array of domConfigs, append to it to have the config applied to the header
   */
  getHeaderDomConfigs(configs) {}
  /**
   * A chainable function which Features may hook to add their own content to the foreground canvas
   * @param {Array} configs An array of domConfigs, append to it to have the config applied to the foreground canvas
   */
  getForegroundDomConfigs(configs) {}
  //endregion
  //region Grid overrides
  async onStoreDataChange({
    action
  }) {
    const me = this;
    // Only update the UI immediately if we are visible
    if (me.isVisible) {
      // When repopulating stores (pro and up on data reload), the engine is not in a valid state until committed.
      // Don't want to commit here, since it might be repopulating multiple stores.
      // Instead delay grids refresh until project is ready
      if (action === 'dataset' && me.project.isRepopulatingStores) {
        await me.project.await('refresh', false);
      }
      super.onStoreDataChange(...arguments);
    }
    // Otherwise wait till next time we get painted (shown, or a hidden ancestor shown)
    else {
      me.whenVisible('refresh', me, [true]);
    }
  }
  refresh(forceLayout = true) {
    const me = this;
    if (me.isPainted && !me.refreshSuspended) {
      // We need to refresh if there are Features laying claim to the visible time axis.
      // Or there are events which fall inside the time axis.
      // Or (if no events fall inside the time axis) there are event elements to remove.
      if (me.isVertical || me.hasVisibleEvents || me.timeAxisSubGridElement.querySelector(me.eventSelector)) {
        if (me.isEngineReady) {
          me.refreshRows(false, forceLayout);
        } else {
          me.refreshAfterProjectRefresh = true;
          me.currentOrientation.refreshAllWhenReady = true;
        }
      }
      // Even if there are no events in our timeline, Features
      // assume there will be a refresh event from the RowManager
      // after a refresh request so fire it here.
      else {
        me.rowManager.trigger('refresh');
      }
    }
  }
  render() {
    const me = this,
      schedulerEl = me.timeAxisSubGridElement;
    if (me.useBackgroundCanvas) {
      me._backgroundCanvas = DomHelper.createElement({
        className: 'b-sch-background-canvas',
        parent: schedulerEl,
        nextSibling: schedulerEl.firstElementChild
      });
    }
    // The font-size trick is no longer used by scheduler, since it allows per resource margins
    const fgCanvas = me._foregroundCanvas = DomHelper.createElement({
      className: 'b-sch-foreground-canvas',
      style: `font-size:${me.rowHeight - me.resourceMargin * 2}px`,
      parent: schedulerEl
    });
    me.timeAxisSubGrid.insertRowsBefore = fgCanvas;
    // Size correctly in case ticks does not fill height
    if (me.isVertical && me.suppressFit) {
      me.updateCanvasSize();
    }
    super.render(...arguments);
  }
  refreshRows(returnToTop = false, reLayoutEvents = true) {
    const me = this;
    if (me.isConfiguring) {
      return;
    }
    me.currentOrientation.refreshRows(reLayoutEvents);
    super.refreshRows(returnToTop);
  }
  getCellDataFromEvent(event, includeSingleAxisMatch) {
    if (includeSingleAxisMatch) {
      includeSingleAxisMatch = !Boolean(event.target.closest('.b-sch-foreground-canvas'));
    }
    return super.getCellDataFromEvent(event, includeSingleAxisMatch);
  }
  // This GridSelection override disables drag-selection in timeaxis column for scheduler and gantt
  onCellNavigate(me, from, to) {
    var _to$cell, _GlobalEvents$current;
    if ((_to$cell = to.cell) !== null && _to$cell !== void 0 && _to$cell.classList.contains('b-timeaxis-cell') && !((_GlobalEvents$current = GlobalEvents.currentMouseDown) !== null && _GlobalEvents$current !== void 0 && _GlobalEvents$current.target.classList.contains('b-grid-cell'))) {
      this.preventDragSelect = true;
    }
    super.onCellNavigate(...arguments);
  }
  //endregion
  //region Other
  // duration = false prevents transition
  runWithTransition(fn, duration) {
    const me = this;
    // Do not attempt to enter animating state if we are not visible
    if (me.isVisible) {
      // Allow calling with true/false to keep code simpler in other places
      if (duration == null || duration === true) {
        duration = me.transitionDuration;
      }
      // Ask Grid superclass to enter the animated state if requested and enabled.
      if (duration && me.enableEventAnimations) {
        if (!me.hasTimeout('exitTransition')) {
          me.isAnimating = true;
        }
        // Exit animating state in duration milliseconds.
        exitTransition.delay = duration;
        me.setTimeout(exitTransition);
      }
    }
    fn();
  }
  exitTransition() {
    this.isAnimating = false;
    this.trigger('transitionend');
  }
  // Awaited by CellEdit to make sure that the editor is not moved until row heights have transitioned, to avoid it
  // ending up misaligned
  async waitForAnimations() {
    // If project is calculating, we should await that too. It might lead to transitions
    if (!this.isEngineReady) {
      await this.project.await('dataReady', false);
    }
    await super.waitForAnimations();
  }
  /**
   * Refreshes the grid with transitions enabled.
   */
  refreshWithTransition(forceLayout, duration) {
    const me = this;
    // No point in starting a transition if we cant refresh anyway
    if (!me.refreshSuspended && me.isPainted) {
      // Since we suspend refresh when loading with CrudManager, rows might not have been initialized yet
      if (!me.rowManager.topRow) {
        me.rowManager.reinitialize();
      } else {
        me.runWithTransition(() => me.refresh(forceLayout), duration);
      }
    }
  }
  /**
   * Returns an object representing the visible date range
   * @property {Object}
   * @property {Date} visibleDateRange.startDate
   * @property {Date} visibleDateRange.endDate
   * @readonly
   * @category Dates
   */
  get visibleDateRange() {
    return this.currentOrientation.visibleDateRange;
  }
  // This override will force row selection on timeaxis column selection, effectively disabling cell selection there
  isRowNumberSelecting(...selectors) {
    return super.isRowNumberSelecting(...selectors) || selectors.some(cs => {
      var _cs$cell;
      return cs.column ? cs.column.isTimeAxisColumn : (_cs$cell = cs.cell) === null || _cs$cell === void 0 ? void 0 : _cs$cell.closest('.b-timeaxis-cell');
    });
  }
  //endregion
  /**
   * Returns a rounded duration value to be displayed in UI (tooltips, labels etc)
   * @param {Number} The raw duration value
   * @param {Number} [nbrDecimals] The number of decimals, defaults to {@link #config-durationDisplayPrecision}
   * @returns {Number} The rounded duration
   */
  formatDuration(duration, nbrDecimals = this.durationDisplayPrecision) {
    const multiplier = Math.pow(10, nbrDecimals);
    return Math.round(duration * multiplier) / multiplier;
  }
  beginListeningForBatchedUpdates() {
    this.listenToBatchedUpdates = (this.listenToBatchedUpdates || 0) + 1;
  }
  endListeningForBatchedUpdates() {
    if (this.listenToBatchedUpdates) {
      this.listenToBatchedUpdates -= 1;
    }
  }
  onConnectedCallback(connected, initialConnect) {
    if (connected && !initialConnect) {
      this.timeAxisSubGrid.scrollable.x += 0.5;
    }
  }
  updateRtl(rtl) {
    const me = this,
      {
        isConfiguring
      } = me;
    let visibleDateRange;
    if (!isConfiguring) {
      visibleDateRange = me.visibleDateRange;
    }
    super.updateRtl(rtl);
    if (!isConfiguring) {
      me.currentOrientation.clearAll();
      if (me.infiniteScroll) {
        me.shiftToDate(visibleDateRange.startDate);
        me.scrollToDate(visibleDateRange.startDate, {
          block: 'start'
        });
      } else {
        me.timelineScroller.position += 0.5;
      }
    }
  }
  /**
   * Applies the start and end date to each event store request (formatted in the same way as the start date field,
   * defined in the EventStore Model class).
   * @category Data
   * @private
   */
  applyStartEndParameters(params) {
    const me = this,
      field = me.eventStore.modelClass.fieldMap.startDate;
    if (me.passStartEndParameters) {
      params[me.startParamName] = field.print(me.startDate);
      params[me.endParamName] = field.print(me.endDate);
    }
  }
}
// Register this widget type with its Factory
TimelineBase.initClass();
// Has to be here because Gantt extends TimelineBase
VersionHelper.setVersion('scheduler', '5.3.7');
TimelineBase._$name = 'TimelineBase';

export { AbstractTimeRanges, Base, CalendarCacheIntervalMultiple, CalendarCacheMultiple, ColumnLines, Dependencies, DependencyCreation, DependencyTooltip, DragBase, DragCreateBase, DurationColumn, EventFilter, EventResize, HorizontalTimeAxis, NonWorkingTime, NonWorkingTimeMixin, PresetStore, RectangularPathFinder, ResourceHeader, ScheduleTooltip, TimeAxis, TimeAxisBase, TimeAxisColumn, TimeAxisHeaderMenu, TimeAxisSubGrid, TimeAxisViewModel, TimelineBase, TimelineDateMapper, TimelineDomEvents, TimelineEventRendering, TimelineScroll, TimelineState, TimelineViewPresets, TimelineZoomable, TooltipBase, ViewPreset, combineCalendars, pm };
//# sourceMappingURL=TimelineBase.js.map
