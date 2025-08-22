/*!
 *
 * Bryntum Scheduler Pro 5.3.7
 *
 * Copyright(c) 2023 Bryntum AB
 * https://bryntum.com/contact
 * https://bryntum.com/license
 *
 */
import { CalendarField, SchedulerProEvent, PartOfProject, PercentDoneMixin, EventSegmentModel, ChronoEventStoreMixin, ProjectChangeHandlerMixin, ProjectCrudManager, SchedulerProProjectMixin, CalendarModel, DependencyModel, AssignmentModel, ResourceModel, CalendarManagerStore, DependencyStore, AssignmentStore, ResourceStore, StateTrackingManager, ProjectProgressMixin, SchedulingIssueResolution, CalculatedValueGen, BaseCalendarMixin } from './chunks/SchedulingIssueResolution.js';
export { AdvancedTab, AssignmentAllocationInterval, AssignmentModel, AssignmentStore, BaseAllocationInfo, BaseAllocationInterval, BaseAssignmentMixin, BaseCalendarMixin, BaseDependencyMixin, BaseDependencyResolution, BaseEmptyCalendarEffectResolution, BaseEventMixin, BaseHasAssignmentsMixin, BaseResourceMixin, BreakCurrentStackExecution, CalculateProposed, CalculatedValueGen, CalculatedValueGenC, CalculatedValueSync, CalculatedValueSyncC, CalculationGen, CalculationSync, CalendarField, CalendarIntervalModel, CalendarManagerStore, CalendarModel, CanCombineCalendarsMixin, ChangeLogAction, ChangeLogAssignmentEntity, ChangeLogDependencyEntity, ChangeLogEntity, ChangeLogStore, ChangeLogTransactionModel, ChronoAbstractProjectMixin, ChronoAssignmentStoreMixin, ChronoCalendarManagerStoreMixin, ChronoDependencyStoreMixin, ChronoEventStoreMixin, ChronoEventTreeStoreMixin, Field as ChronoField, ChronoGraph, ChronoModelFieldIdentifier, ChronoModelMixin, ChronoModelReferenceBucketFieldIdentifier, ChronoModelReferenceFieldIdentifier, ChronoModelReferenceFieldQuark, ChronoPartOfProjectGenericMixin, ChronoPartOfProjectModelMixin, ChronoPartOfProjectStoreMixin, ChronoResourceStoreMixin, ChronoStoreMixin, Transaction as ChronoTransaction, CommitZero, ComputationCycle, ConflictEffect, ConflictEffectDescription, ConflictResolution, ConflictSymbol, ConstrainedEarlyEventMixin, ConstraintInterval, ConstraintIntervalDescription, ConstraintTypePicker, ContextGen, ContextSync, CycleDescription, CycleEffect, CycleEffectDescription, CycleResolution, CycleResolutionInput, CycleResolutionInputChrono, CycleResolutionPopup, CycleSymbol, DateConstraintInterval, DateConstraintIntervalDescription, DateInterval, DeactivateDependencyCycleEffectResolution, DeactivateDependencyResolution, DependencyConstraintInterval, DependencyConstraintIntervalDescription, DependencyModel, DependencyStore, DependencyTab, DependencyTypePicker, DurationConverterMixin, DurationVar, EMPTY_INTERVAL, EarlyLateLazyness, EdgeType, EdgeTypeNormal, EdgeTypePast, EditorTab, Effect, EffectResolutionResult, EffortField, EffortVar, EmptyCalendarEffect, EmptyCalendarEffectDescription, EmptyCalendarSymbol, EndDateField, EndDateVar, EngineReplica, EngineRevision, EngineTransaction, Entity, EntityIdentifier, EntityMeta, EventLoader, EventResize, EventSegmentModel, EventSegmentResize, EventSegments, EventUpdateAction, FieldIdentifier, FixedDurationMixin, FormTab, Formula, FormulasCache, GanttTaskEditor, GeneralTab, GetTransaction, HasCalendarMixin, HasChildrenMixin, HasDateConstraintMixin, HasDependenciesMixin, HasEffortMixin, HasPercentDoneMixin, HasProposedNotPreviousValue, HasProposedNotPreviousValueEffect, HasProposedNotPreviousValueSymbol, HasProposedValue, HasProposedValueEffect, HasProposedValueSymbol, HasSchedulingModeMixin, HasSubEventsMixin, Identifier, IdentifierC, Instruction, IsChronoModelSymbol, Levels, Listener, Meta, MinimalChronoModelFieldIdentifierGen, MinimalChronoModelFieldIdentifierSync, MinimalChronoModelFieldVariable, MinimalEntityIdentifier, MinimalFieldIdentifierGen, MinimalFieldIdentifierSync, MinimalFieldVariable, MinimalReferenceBucketIdentifier, MinimalReferenceBucketQuark, MinimalReferenceIdentifier, ModelBucketField, ModelCombo, ModelField, ModelReferenceField, NOT_VISITED, NotesTab, OnCycleAction, OwnIdentifier, OwnIdentifierSymbol, OwnQuark, OwnQuarkSymbol, PartOfProject, PercentBar, PercentDoneMixin, PredecessorsTab, PreviousValueOf, PreviousValueOfEffect, PreviousValueOfSymbol, ProjectChangeHandlerMixin, ProjectCrudManager, ProjectProgressMixin, ProjectWebSocketHandlerMixin, ProposedArgumentsOf, ProposedArgumentsOfEffect, ProposedArgumentsOfSymbol, ProposedOrPrevious, ProposedOrPreviousSymbol, ProposedOrPreviousValueOf, ProposedOrPreviousValueOfEffect, ProposedOrPreviousValueOfSymbol, ProposedValueOf, ProposedValueOfEffect, ProposedValueOfSymbol, Quark, QuarkGen, QuarkSync, ReadMode, ReadyStatePropagator, RecurrenceTab, ReferenceBucketField, ReferenceBucketIdentifier, ReferenceBucketQuark, ReferenceField, ReferenceIdentifier, Reject, RejectEffect, RejectSymbol, RemoveDateConstraintConflictResolution, RemoveDependencyCycleEffectResolution, RemoveDependencyResolution, Replica, ResourceAllocationEventRangeCalendar, ResourceAllocationEventRangeCalendarIntervalMixin, ResourceAllocationEventRangeCalendarIntervalStore, ResourceAllocationInfo, ResourceAllocationInterval, ResourceModel, ResourceStore, ResourcesTab, Revision, SEDBackwardCycleResolutionContext, SEDDispatcher, SEDDispatcherIdentifier, SEDForwardCycleResolutionContext, SEDGraphDescription, SEDWUDispatcher, SEDWUDispatcherIdentifier, ScheduledByDependenciesEarlyEventMixin, SchedulerAdvancedTab, SchedulerBasicEvent, SchedulerBasicProjectMixin, SchedulerGeneralTab, SchedulerProAssignmentMixin, SchedulerProCycleEffect, SchedulerProDependencyMixin, SchedulerProEvent, SchedulerProHasAssignmentsMixin, SchedulerProProjectMixin, SchedulerProResourceMixin, SchedulerTaskEditor, SchedulingIssueEffectResolution, SchedulingIssueResolution, SchedulingIssueResolutionPopup, SchedulingModePicker, Schema, StartDateField, StartDateVar, StateTrackingManager, SuccessorsTab, SynchronousCalculationStarted, TaskEdit, TaskEditorBase, TombStone, TransactionCycleDetectionWalkContext, TransactionSymbol, TransactionWalkDepth, UnitsVar, UnsafePreviousValueOf, UnsafePreviousValueOfEffect, UnsafePreviousValueOfSymbol, UnsafeProposedOrPreviousValueOf, UnsafeProposedOrPreviousValueOfEffect, UnsafeProposedOrPreviousValueOfSymbol, Use24hrsEmptyCalendarEffectResolution, Use8hrsEmptyCalendarEffectResolution, VISITED_TOPOLOGICALLY, Variable, VariableC, VariableInputState, VariableWalkContext, VersionModel, VersionStore, Versions, WalkContext, WalkSource, WalkState, Write, WriteEffect, WriteSeveral, WriteSeveralEffect, WriteSeveralSymbol, WriteSymbol, bucket, build_proposed, calculate, calculateEffectiveEndDateConstraintInterval, calculateEffectiveStartDateConstraintInterval, createEntityOnPrototype, cycleInfo, dateConverter, durationFormula, effortFormula, endDateByEffortFormula, endDateFormula, ensureEntityOnPrototype, entity, entityDecoratorBody, field, fixedDurationAndEffortSEDWUGraphDescription, fixedDurationSEDWUBackwardEffortDriven, fixedDurationSEDWUBackwardNonEffortDriven, fixedDurationSEDWUForwardEffortDriven, fixedDurationSEDWUForwardNonEffortDriven, fixedDurationSEDWUGraphDescription, generic_field, getDecoratedModelFields, injectStaticFieldsProperty, intersectIntervals, isAtomicValue, isSerializableEqual, model_field, prototypeValue, reference, required, runGeneratorAsyncWithEffect, runGeneratorSyncWithEffect, startDateByEffortFormula, startDateFormula, throwUnknownIdentifier, unitsFormula, validateRequiredProperties, write } from './chunks/SchedulingIssueResolution.js';
import { TimeSpan, RecurringTimeSpan, EventModelMixin, SharedEventStoreMixin, RecurringEventsMixin, GetEventsMixin, DayIndexMixin, EventStoreMixin, ResourceTimeRangeModel, ProjectModelMixin, TimeUnit, AbstractPartOfProjectStoreMixin } from './chunks/ProjectModel.js';
export { AbstractPartOfProjectStoreMixin } from './chunks/ProjectModel.js';
import { Column, ColumnStore, GridFeatureManager } from './chunks/GridBase.js';
import { AttachToProjectMixin } from './chunks/EventNavigation.js';
import { Scale, Histogram } from './chunks/Scale.js';
import { Duration, DateHelper, AjaxStore, Base, ArrayHelper, InstancePlugin, Tooltip, Rectangle, Delayable, DomSync, DomHelper, EventHelper, VersionHelper, StringHelper, Model, ObjectHelper, Store } from './chunks/Editor.js';
import { HorizontalLayoutStack, HorizontalLayoutPack, ResourceTimeRangesBase, DependencyEdit as DependencyEdit$1, EventDrag, HorizontalRendering, VerticalRendering, SchedulerBase } from './chunks/SchedulerBase.js';
import { NumberFormat } from './chunks/MessageDialog.js';
import './chunks/TimelineBase.js';
import { Tree } from './chunks/Tree.js';
import './chunks/RegionResize.js';
import { GridRowModel } from './chunks/GridRowModel.js';
import { TreeGrid } from './chunks/TreeGrid.js';
import './chunks/AvatarRendering.js';
import './chunks/Grid.js';
import './chunks/TextAreaField.js';
import './chunks/TabPanel.js';
import './chunks/Card.js';
import './chunks/ButtonGroup.js';
import './chunks/EventSelection.js';
import './chunks/Slider.js';

/**
 * @module SchedulerPro/column/ResourceCalendarColumn
 */
/**
 * A column that displays (and allows user to update) the current {@link SchedulerPro.model.CalendarModel calendar} of
 * the resource.
 *
 * Default editor is a {@link SchedulerPro.widget.CalendarField CalendarField}.
 *
 * {@inlineexample SchedulerPro/column/ResourceCalendarColumn.js}
 * @mixes Scheduler/data/mixin/AttachToProjectMixin
 * @extends Grid/column/Column
 * @classType resourceCalendar
 * @column
 */
class ResourceCalendarColumn extends Column.mixin(AttachToProjectMixin) {
  //region Config
  static get $name() {
    return 'ResourceCalendarColumn';
  }
  static get type() {
    return 'resourceCalendar';
  }
  static get defaults() {
    return {
      field: 'calendar',
      text: 'Calendar',
      editor: {
        type: CalendarField.type,
        clearable: true,
        allowInvalid: false
      }
    };
  }
  //endregion
  //region Init
  attachToProject(project) {
    if (project) {
      // Store default calendar to filter out this value
      this.defaultCalendar = project.defaultCalendar;
      this.editor.store = project.calendarManagerStore;
    }
  }
  attachToResourceStore(resourceStore) {
    super.attachToResourceStore(resourceStore);
    if (resourceStore) {
      resourceStore.ion({
        name: 'resourceStore',
        update: 'onResourceUpdate',
        thisObj: this
      });
    }
  }
  //endregion
  //region Events
  // Event rendering does not update cells when engine updates a resource, instead we do a minimal update here
  onResourceUpdate({
    record,
    changes
  }) {
    const change = changes[this.field];
    if (change) {
      var _change$value;
      // Ignore "normalization" of id -> instance, won't affect our appearance
      if (typeof change.oldValue === 'string' && ((_change$value = change.value) === null || _change$value === void 0 ? void 0 : _change$value.id) === change.oldValue) {
        return;
      }
      this.refreshCell(record);
    }
  }
  //endregion
  //region Render
  renderer({
    value
  }) {
    if (value === this.defaultCalendar) {
      return '';
    } else if (value && value.id) {
      const record = this.editor.store.getById(value.id);
      return record && record[this.editor.displayField] || '';
    } else {
      return '';
    }
  }
  //endregion
}

ColumnStore.registerColumnType(ResourceCalendarColumn);
ResourceCalendarColumn._$name = 'ResourceCalendarColumn';

/**
 * @module SchedulerPro/column/ScaleColumn
 */
/**
 * An object representing a point on the scale displayed by {@link SchedulerPro.column.ScaleColumn}.
 *
 * @typedef {Object} ScalePoint
 * @property {Number} value Point value
 * @property {String} unit Point value unit
 * @property {String} text Point text label
 */
/**
 * A specialised column showing a graduated scale from a defined array of values
 * and labels. This column is used in the {@link SchedulerPro.view.ResourceHistogram} and is not editable. Normally
 * you should not need to interact with this class directly.
 *
 * @extends Grid/column/Column
 * @classType scale
 * @column
 */
class ScaleColumn extends Column {
  //region Config
  static get $name() {
    return 'ScaleColumn';
  }
  static get type() {
    return 'scale';
  }
  static get isScaleColumn() {
    return true;
  }
  static get fields() {
    return ['scalePoints'];
  }
  static get defaults() {
    return {
      text: '\xa0',
      width: 40,
      minWidth: 40,
      cellCls: 'b-scale-cell',
      editor: false,
      sortable: false,
      groupable: false,
      filterable: false,
      alwaysClearCell: false,
      scalePoints: [{
        value: 4
      }, {
        value: 8,
        text: 8
      }]
    };
  }
  //endregion
  //region Constructor/Destructor
  onDestroy() {
    this.scaleWidget.destroy();
  }
  //endregion
  //region Internal
  set width(width) {
    super.width = width;
    this.scaleWidget.width = width;
  }
  get width() {
    return super.width;
  }
  applyValue(useProp, key, value) {
    // pass value to scaleWidget
    if (key === 'scalePoints') {
      this.scaleWidget[key] = value;
    }
    return super.applyValue(...arguments);
  }
  buildScaleWidget() {
    const me = this;
    const scaleWidget = new Scale({
      owner: me.grid,
      appendTo: me.grid.floatRoot,
      cls: 'b-hide-offscreen',
      align: 'right',
      scalePoints: me.scalePoints,
      monitorResize: false
    });
    Object.defineProperties(scaleWidget, {
      width: {
        get() {
          return me.width;
        },
        set(width) {
          this.element.style.width = `${width}px`;
          this._width = me.width;
        }
      },
      height: {
        get() {
          return this._height;
        },
        set(height) {
          this.element.style.height = `${height}px`;
          this._height = height;
        }
      }
    });
    scaleWidget.width = me.width;
    return scaleWidget;
  }
  get scaleWidget() {
    const me = this;
    if (!me._scaleWidget) {
      me._scaleWidget = me.buildScaleWidget();
    }
    return me._scaleWidget;
  }
  //endregion
  //region Render
  renderer({
    cellElement,
    scaleWidget = this.scaleWidget
  }) {
    scaleWidget.height = this.grid.rowHeight;
    scaleWidget.refresh();
    // Clone the scale widget element since every row is supposed to have
    // the same scale settings
    const scaleCloneElement = scaleWidget.element.cloneNode(true);
    scaleCloneElement.removeAttribute('id');
    scaleCloneElement.classList.remove('b-hide-offscreen');
    cellElement.innerHTML = '';
    cellElement.appendChild(scaleCloneElement);
  }
  //endregion
}

ColumnStore.registerColumnType(ScaleColumn);
ScaleColumn._$name = 'ScaleColumn';

/**
 * @module SchedulerPro/model/EventModel
 */
/**
 * This class represent a single event in your schedule, usually added to a {@link SchedulerPro.data.EventStore}.
 *
 * It is a subclass of the {@link Scheduler.model.TimeSpan}, which is in turn subclass of {@link Core.data.Model}.
 * Please refer to documentation of that class to become familiar with the base interface of the event.
 *
 * ## Async date calculations
 *
 * A record created from an `EventModel` is normally part of an `EventStore`, which in turn is part of a project. When
 * dates or the duration of an event is changed, the project performs async calculations to normalize the other fields.
 * For example if `duration` is change, it will calculate `endDate`.
 *
 * As a result of this being an async operation, the values of other fields are not guaranteed to be up to date
 * immediately after a change. To ensure data is up to date, await the calculations to finish.
 *
 * For example, `endDate` is not up to date after this operation:
 *
 * ```javascript
 * eventRecord.duration = 5;
 * // endDate not yet calculated
 * ```
 *
 * But if calculations are awaited it is up to date:
 *
 * ```javascript
 * eventRecord.duration = 5;
 * await eventRecord.project.commitAsync();
 * // endDate is calculated
 * ```
 *
 * As an alternative, you can also use `setAsync()` to trigger calculations directly after the change:
 *
 * ```javascript
 * await eventRecord.setAsync({ duration : 5});
 * // endDate is calculated
 * ```
 *
 * ## Subclassing the Event model class
 * The Event model has a few predefined fields as seen below. If you want to add new fields or change the options for
 * the existing fields, you can do that by subclassing this class (see example below).
 *
 * ```javascript
 * class MyEvent extends EventModel {
 *
 *     static get fields() {
 *         return [
 *            // Add new field
 *            { name: 'myField', type : 'number', defaultValue : 0 }
 *         ];
 *     },
 *
 *     myCheckMethod() {
 *         return this.myField > 0
 *     },
 *
 *     ...
 * });
 * ```
 *
 * If you in your data want to use other names for the startDate, endDate, resourceId and name fields you can configure
 * them as seen below:
 * ```javascript
 * class MyEvent extends EventModel {
 *
 *     static get fields() {
 *         return [
 *            { name: 'startDate', dataSource 'taskStart' },
 *            { name: 'endDate', dataSource 'taskEnd', format: 'YYYY-MM-DD' },
 *            { name: 'resourceId', dataSource 'userId' },
 *            { name: 'name', dataSource 'taskTitle' },
 *         ];
 *     },
 *     ...
 * });
 * ```
 *
 * Please refer to {@link Core.data.Model} for additional details.
 *
 * @extends Scheduler/model/TimeSpan
 * @mixes Scheduler/model/mixin/RecurringTimeSpan
 * @mixes Scheduler/model/mixin/EventModelMixin
 * @mixes SchedulerPro/model/mixin/PercentDoneMixin
 * @mixes SchedulerPro/data/mixin/PartOfProject
 *
 * @typings Scheduler/model/EventModel -> Scheduler/model/SchedulerEventModel
 */
class EventModel extends SchedulerProEvent.derive(TimeSpan).mixin(RecurringTimeSpan, PartOfProject, EventModelMixin, PercentDoneMixin) {
  /**
   * Returns the event store this event is part of.
   *
   * @member {SchedulerPro.data.EventStore} eventStore
   * @readonly
   * @typings Scheduler/model/TimeSpan:eventStore -> {Scheduler.data.EventStore||SchedulerPro.data.EventStore}
   */
  /**
   * If given resource is assigned to this event, returns a {@link SchedulerPro.model.AssignmentModel} record.
   * Otherwise returns `null`
   *
   * @method getAssignmentFor
   * @param {SchedulerPro.model.ResourceModel} resource The instance of {@link SchedulerPro.model.ResourceModel}
   *
   * @returns {SchedulerPro.model.AssignmentModel|null}
   */
  /**
   * This method assigns a resource to this event.
   *
   * Will cause the schedule to be updated - returns a `Promise`
   *
   * @method assign
   * @param {SchedulerPro.model.ResourceModel} resource The instance of {@link SchedulerPro.model.ResourceModel}
   * @param {Number} [units=100] The `units` field of the new assignment
   *
   * @async
   * @propagating
   */
  /**
   * This method unassigns a resource from this event.
   *
   * Will cause the schedule to be updated - returns a `Promise`
   *
   * @method unassign
   * @param {SchedulerPro.model.ResourceModel} resource The instance of {@link SchedulerPro.model.ResourceModel}
   *
   * @async
   * @propagating
   */
  /**
   * Sets the calendar of the event. Will cause the schedule to be updated - returns a `Promise`
   *
   * @method setCalendar
   * @param {SchedulerPro.model.CalendarModel} calendar The new calendar. Provide `null` to fall back to the project calendar.
   * @async
   * @propagating
   */
  /**
   * Returns the event calendar.
   *
   * @method getCalendar
   * @returns {SchedulerPro.model.CalendarModel} The event calendar.
   */
  /**
   * Either activates or deactivates the task depending on the passed value.
   * Will cause the schedule to be updated - returns a `Promise`
   *
   * @method setInactive
   * @param {boolean} inactive `true` to deactivate the task, `false` to activate it.
   * @async
   * @propagating
   */
  /**
   * Sets the start date of the event. Will cause the schedule to be updated - returns a `Promise`
   *
   * Note, that the actually set start date may be adjusted, according to the calendar, by skipping the non-working time forward.
   *
   * @method setStartDate
   * @param {Date} date The new start date.
   * @param {Boolean} [keepDuration=true] Whether to keep the duration (and update the end date), while changing the start date, or vice-versa.
   * @async
   * @propagating
   */
  /**
   * Sets the end date of the event. Will cause the schedule to be updated - returns a `Promise`
   *
   * Note, that the actually set end date may be adjusted, according to the calendar, by skipping the non-working time backward.
   *
   * @method setEndDate
   * @param {Date} date The new end date.
   * @param {Boolean} [keepDuration=false] Whether to keep the duration (and update the start date), while changing the end date, or vice-versa.
   * @async
   * @propagating
   */
  /**
   * Updates the duration (and optionally unit) of the event. Will cause the schedule to be updated - returns a `Promise`
   *
   * @method setDuration
   * @param {Number} duration New duration value
   * @param {String} [unit] New duration unit
   * @async
   * @propagating
   */
  /**
   * Sets the constraint type and (optionally) constraining date to the event.
   *
   * @method setConstraint
   * @param {'finishnoearlierthan'|'finishnolaterthan'|'mustfinishon'|'muststarton'|'startnoearlierthan'|'startnolaterthan'|null} constraintType
   * Constraint type, please refer to the {@link #field-constraintType} for the valid
   * values.
   * @param {Date} [constraintDate] Constraint date.
   * @async
   * @propagating
   */
  /**
   * Updates the {@link #field-effort} (and optionally {@link #field-effortUnit unit}) of the event.
   * Will cause the schedule to be updated - returns a `Promise`
   *
   * @method setEffort
   * @param {Number} effort New effort value
   * @param {'millisecond'|'second'|'minute'|'hour'|'day'|'week'|'month'|'quarter'|'year'} [unit] New effort
   * unit
   * @async
   * @propagating
   */
  /**
   * Sets {@link #field-segments} field value.
   *
   * @method
   * @name setSegments
   * @param {SchedulerPro.model.EventSegmentModel[]} segments Array of segments or null to make the event not segmented.
   * @returns {Promise}
   * @propagating
   */
  /**
   * Splits the event into segments.
   * @method splitToSegments
   * @param {Date} from The date to split this event at.
   * @param {Number} [lag=1] Split duration.
   * @param {String} [lagUnit] Split duration unit.
   * @returns {Promise}
   * @propagating
   */
  /**
   * Merges the event segments.
   * The method merges two provided event segments (and all the segment between them if any).
   * @method mergeSegments
   * @param {SchedulerPro.model.EventSegmentModel} [segment1] First segment to merge.
   * @param {SchedulerPro.model.EventSegmentModel} [segment2] Second segment to merge.
   * @returns {Promise}
   * @propagating
   */
  /**
   * Sets the event {@link #field-ignoreResourceCalendar} field value and triggers rescheduling.
   *
   * @method setIgnoreResourceCalendar
   * @param {Boolean} ignore Provide `true` to ignore the calendars of the assigned resources
   * when scheduling the event. If `false` the event performs only when
   * its own {@link #field-calendar} and some of the assigned
   * resource calendars allow that.
   * @async
   * @propagating
   */
  /**
   * Returns the event {@link #field-ignoreResourceCalendar} field value.
   *
   * @method getIgnoreResourceCalendar
   * @returns {Boolean} The event {@link #field-ignoreResourceCalendar} field value.
   */
  /**
   * The event first segment or null if the event is not segmented.
   * @member {SchedulerPro.model.EventSegmentModel} firstSegment
   */
  /**
   * The event last segment or null if the event is not segmented.
   * @member {SchedulerPro.model.EventSegmentModel} lastSegment
   */
  //region Config
  static get $name() {
    return 'EventModel';
  }
  static isProEventModel = true;
  static get fields() {
    return [
    /**
     * This field is automatically set to `true` when the event is "unscheduled" - user has provided an empty
     * string in one of the UI editors for start date, end date or duration. Such event is not rendered,
     * and does not affect the schedule of its successors.
     *
     * To schedule the event back, enter one of the missing values, so that there's enough information
     * to calculate start date, end date and duration.
     *
     * Note, that setting this field manually does nothing. This field should be persisted, but not updated
     * manually.
     *
     * @field {Boolean} unscheduled
      * @readonly
      * @category Scheduling
     */
    /**
     * Segments of the event that appear when the event gets {@link #function-splitToSegments}.
     * @field {SchedulerPro.model.EventSegmentModel[]} segments
     * @category Scheduling
     */
    /**
     * The current status of a task, expressed as the percentage completed (integer from 0 to 100)
     *
     * UI fields representing this data field are disabled for summary events.
     * See {@link #function-isEditable} for details.
     *
     * @field {Number} percentDone
     * @category Scheduling
     */
    /**
     * The start date of a time span (or Event / Task).
     *
     * Uses {@link Core/helper/DateHelper#property-defaultFormat-static DateHelper.defaultFormat} to convert a
     * supplied string to a Date. To specify another format, either change that setting or subclass TimeSpan and
     * change the dateFormat for this field.
     *
     * UI fields representing this data field are disabled for summary events
     * except the {@link #field-manuallyScheduled manually scheduled} events.
     * See {@link #function-isEditable} for details.
     *
     * Note that the field always returns a `Date`.
     *
     * @field {Date} startDate
     * @accepts {String|Date}
     * @category Scheduling
     */
    /**
     * The end date of a time span (or Event / Task).
     *
     * Uses {@link Core/helper/DateHelper#property-defaultFormat-static DateHelper.defaultFormat} to convert a
     * supplied string to a Date. To specify another format, either change that setting or subclass TimeSpan and
     * change the dateFormat for this field.
     *
     * UI fields representing this data field are disabled for summary events
     * except the {@link #field-manuallyScheduled manually scheduled} events.
     * See {@link #function-isEditable} for details.
     *
     * Note that the field always returns a `Date`.
     *
     * @field {Date} endDate
     * @accepts {String|Date}
     * @category Scheduling
     */
    /**
     * The numeric part of the timespan's duration (the number of units).
     *
     * UI fields representing this data field are disabled for summary events
     * except the {@link #field-manuallyScheduled manually scheduled} events.
     * See {@link #function-isEditable} for details.
     *
     * @field {Number} duration
     * @category Scheduling
     */
    /**
     * Field storing the event constraint alias or NULL if not constraint set.
     * Valid values are:
     * - "finishnoearlierthan"
     * - "finishnolaterthan"
     * - "mustfinishon"
     * - "muststarton"
     * - "startnoearlierthan"
     * - "startnolaterthan"
     *
     * @field {'finishnoearlierthan'|'finishnolaterthan'|'mustfinishon'|'muststarton'|'startnoearlierthan'|'startnolaterthan'|null} constraintType
     * @category Scheduling
     */
    /**
     * Field defining the constraint boundary date, if applicable.
     * @field {Date} constraintDate
     * @category Scheduling
     */
    /**
     * When set to `true`, the `startDate` of the event will not be changed by any of its incoming dependencies
     * or constraints.
     *
     * @field {Boolean} manuallyScheduled
     * @category Scheduling
     */
    /**
     * When set to `true` the event becomes inactive and stops taking part in the project scheduling (doesn't
     * affect linked events and affect its assigned resources allocation).
     *
     * @field {Boolean} inactive
     * @category Scheduling
     */
    /**
     * When set to `true` the calendars of the assigned resources
     * are not taken into account when scheduling the event.
     *
     * By default the field value is `false` resulting in that the event performs only when
     * its own {@link #field-calendar} and some of the assigned
     * resource calendars allow that.
     * @field {Boolean} ignoreResourceCalendar
     * @category Scheduling
     */
    /**
     * A calculated field storing the _early start date_ of the event.
     * The _early start date_ is the earliest possible date the event can start.
     * This value is calculated based on the earliest dates of the event predecessors and the event own constraints.
     * If the event has no predecessors nor other constraints, its early start date matches the project start date.
     *
     * UI fields representing this data field are naturally disabled since the field is readonly.
     * See {@link #function-isEditable} for details.
     *
     * @field {Date} earlyStartDate
     * @calculated
     * @readonly
     * @category Scheduling
     */
    /**
     * A calculated field storing the _early end date_ of the event.
     * The _early end date_ is the earliest possible date the event can finish.
     * This value is calculated based on the earliest dates of the event predecessors and the event own constraints.
     * If the event has no predecessors nor other constraints, its early end date matches the project start date plus the event duration.
     *
     * UI fields representing this data field are naturally disabled since the field is readonly.
     * See {@link #function-isEditable} for details.
     *
     * @field {Date} earlyEndDate
     * @calculated
     * @readonly
     * @category Scheduling
     */
    /**
     * The calendar, assigned to the entity. Allows you to set the time when entity can perform the work.
     *
     * All entities are by default assigned to the project calendar, provided as the {@link SchedulerPro.model.ProjectModel#field-calendar} option.
     *
     * @field {SchedulerPro.model.CalendarModel} calendar
     * @category Scheduling
     */
    /**
     * The numeric part of the event effort (the number of units).
     *
     * @field {Number} effort
     * @category Scheduling
     */
    /**
     * The unit part of the event effort, defaults to "h" (hours). Valid values are:
     *
     * - "millisecond" - Milliseconds
     * - "second" - Seconds
     * - "minute" - Minutes
     * - "hour" - Hours
     * - "day" - Days
     * - "week" - Weeks
     * - "month" - Months
     * - "quarter" - Quarters
     * - "year"- Years
     *
     * This field is readonly after creation, to change it use the {@link #function-setEffort} call.
     * @field {'millisecond'|'second'|'minute'|'hour'|'day'|'week'|'month'|'quarter'|'year'} effortUnit
     * @default "hour"
     * @category Scheduling
     * @readonly
     */
    /**
     * This field defines the event scheduling mode. Based on this field some fields of the event
     * will be "fixed" (should be provided by the user) and some - computed.
     *
     * Possible values are:
     *
     * - `Normal` is the default (and backward compatible) mode. It means the event will be scheduled based on
     * information about its start/end dates, event own calendar (project calendar if there's no one) and
     * calendars of the assigned resources.
     *
     * - `FixedDuration` mode means, that event has fixed start and end dates, but its effort will be computed
     * dynamically, based on the assigned resources information. When duration of such event increases,
     * its effort is increased too. The mode tends to preserve user provided duration so changing effort
     * results adjusting assignment units and vise-versa assignment changes adjusts effort.
     *
     * @field {'Normal'|'FixedDuration'} schedulingMode
     * @category Scheduling
     */
    /**
     * This boolean flag defines what part the data should be updated in the `FixedDuration` scheduling
     * mode.
     * If it is `true`, then {@link #field-effort} is kept intact when new duration is provided and
     * assignment {@link SchedulerPro.model.AssignmentModel#field-units} is updated.
     * If it is `false`, then assignment {@link SchedulerPro.model.AssignmentModel#field-units} is kept
     * intact when new duration is provided and {@link #field-effort} is updated.
     *
     * @field {Boolean} effortDriven
     * @default false
     * @category Scheduling
     */
    /**
     * The event effective calendar. Returns the
     * {@link SchedulerPro.model.ProjectModel#field-calendar project calendar} if the event has no own
     * {@link #field-calendar} provided.
     * @member {SchedulerPro.model.CalendarModel} effectiveCalendar
     */
    /**
     * Set this to true if this task should be shown in the Timeline widget
     * @field {Boolean} showInTimeline
     * @category Common
     */
    {
      name: 'showInTimeline',
      type: 'boolean',
      defaultValue: false
    },
    /**
     * Note about the event
     * @field {String} note
     * @category Common
     */
    'note',
    /**
     * Buffer time before event start. Specified in a human-friendly form as accepted by
     * {@link Core.helper.DateHelper#function-parseDuration-static}:
     * ```javascript
     * // Create event model with a 30 minutes buffer time before the event start
     * new EventModel({ startDate : '2020-01-01', endDate : '2020-01-02', preamble : '30 minutes' })
     * ```
     *
     * Used by the {@link SchedulerPro.feature.EventBuffer} feature.
     *
     * @field {Core.data.Duration} preamble
     * @accepts {String}
     * @category Scheduling
     */
    {
      name: 'preamble',
      convert: value => value ? new Duration(value) : null
    },
    /**
     * Buffer time after event end. Specified in a human-friendly form as accepted by
     * {@link Core.helper.DateHelper#function-parseDuration-static}:
     * ```javascript
     * // Create event model with a 1 hour buffer time after the event end
     * new EventModel({ startDate : '2020-01-01', endDate : '2020-01-02', postamble : '1 hour' })
     * ```
     *
     * Used by the {@link SchedulerPro.feature.EventBuffer} feature.
     *
     * @field {String} postamble
     * @accepts {String}
     * @category Scheduling
     */
    {
      name: 'postamble',
      convert: value => value ? new Duration(value) : null
    }];
  }
  getDefaultSegmentModelClass() {
    return EventSegmentModel;
  }
  //endregion
  //region EventBuffer
  updateWrapDate(date, duration, forward = true) {
    duration = new Duration(duration);
    return new Date(date.getTime() + (forward ? 1 : -1) * duration.milliseconds);
  }
  get startDate() {
    let dt;
    if (this.isOccurrence) {
      dt = this.get('startDate');
    } else {
      // Micro optimization to avoid expensive super call. super will be hit in Scheduler Pro
      dt = this._startDate ?? super.startDate;
    }
    if (this.allDay) {
      dt = this.constructor.getAllDayStartDate(dt);
    }
    return dt;
  }
  set startDate(startDate) {
    const me = this;
    // Update children when parents startDate changes (ignoring initial data set)
    if (me.generation && me.isParent && !me.$ignoreChange) {
      const timeDiff = DateHelper.diff(me.startDate, startDate);
      if (timeDiff) {
        // Move all children same amount
        for (const child of this.children) {
          child.startDate = DateHelper.add(child.startDate, timeDiff);
        }
      }
    }
    if (me.batching) {
      me._startDate = startDate;
      me.set({
        startDate
      });
    } else {
      super.startDate = startDate;
      if (me.preamble) {
        me.wrapStartDate = null;
        me.wrapEndDate = null;
      }
    }
  }
  get endDate() {
    let dt;
    if (this.isOccurrence) {
      dt = this.get('endDate');
    } else {
      // Micro optimization to avoid expensive super call. super will be hit in Scheduler Pro
      dt = this._endDate ?? super.endDate;
    }
    if (this.allDay) {
      dt = this.constructor.getAllDayEndDate(dt);
    }
    return dt;
  }
  set endDate(endDate) {
    const me = this;
    if (me.batching) {
      me._endDate = endDate;
      me.set({
        endDate
      });
    } else {
      super.endDate = endDate;
      if (me.postamble) {
        me.wrapStartDate = null;
        me.wrapEndDate = null;
      }
    }
  }
  /**
   * Property which encapsulates the effort's magnitude and units.
   *
   * UI fields representing this property are disabled for summary events.
   * See {@link #function-isEditable} for details.
   *
   * @property {Core.data.Duration}
   */
  get fullEffort() {
    return new Duration({
      unit: this.effortUnit,
      magnitude: this.effort
    });
  }
  set fullEffort(effort) {
    this.setEffort(effort.magnitude, effort.unit);
  }
  // Cannot use `convert` method because it might be disabled by `useRawData : true` and we always need to calculate
  // that value
  get wrapStartDate() {
    const me = this,
      {
        preamble,
        startDate
      } = me,
      wrapStartDate = me._wrapStartDate;
    let result;
    if (wrapStartDate) {
      result = wrapStartDate;
    } else {
      if (preamble) {
        result = me.updateWrapDate(startDate, preamble, false);
        me._wrapStartDate = result;
      } else {
        result = startDate;
      }
    }
    return result;
  }
  set wrapStartDate(value) {
    this._wrapStartDate = value;
  }
  get wrapEndDate() {
    const me = this,
      {
        postamble,
        endDate
      } = me,
      wrapEndDate = me._wrapEndDate;
    let result;
    if (wrapEndDate) {
      result = wrapEndDate;
    } else {
      if (postamble) {
        result = me.updateWrapDate(endDate, postamble, true);
        me._wrapEndDate = result;
      } else {
        result = endDate;
      }
    }
    return result;
  }
  set wrapEndDate(value) {
    this._wrapEndDate = value;
  }
  set(data) {
    const isObject = typeof data === 'object';
    if (data === 'preamble' || isObject && 'preamble' in data) {
      this.wrapStartDate = null;
    }
    if (data === 'postamble' || isObject && 'postamble' in data) {
      this.wrapEndDate = null;
    }
    return super.set(...arguments);
  }
  /**
   * Returns event start date adjusted by {@link #field-preamble} (start date - duration).
   * @property {Date}
   * @readonly
   */
  get outerStartDate() {
    return this.wrapStartDate;
  }
  /**
   * Returns event end date adjusted by {@link #field-postamble} (end date + duration).
   * @property {Date}
   * @readonly
   */
  get outerEndDate() {
    return this.wrapEndDate;
  }
  //endregion
  /**
   * Defines if the given event field should be manually editable in UI.
   * You can override this method to provide your own logic.
   *
   * By default, the method defines:
   * - {@link #field-earlyStartDate}, {@link #field-earlyEndDate} as not editable;
   * - {@link #field-endDate}, {@link #field-duration} and {@link #field-fullDuration} fields
   *   as not editable for summary events except the {@link #field-manuallyScheduled manually scheduled} ones;
   * - {@link #field-percentDone} as not editable for summary events.
   *
   * @param {String} fieldName Name of the field
   * @returns {Boolean} Returns `true` if the field is editable, `false` if it is not and `undefined` if the event has
   * no such field.
   */
  isEditable(fieldName) {
    switch (fieldName) {
      // r/o fields
      case 'earlyStartDate':
      case 'earlyEndDate':
        return false;
      // disable percentDone editing for summary tasks
      case 'percentDone':
      case 'renderedPercentDone':
        return this.isLeaf;
      // end/duration is allowed to edit for leafs and manually scheduled summaries
      case 'endDate':
      case 'duration':
      case 'fullDuration':
        return this.isLeaf || this.manuallyScheduled;
    }
    return super.isEditable(fieldName);
  }
  // Occurrences are not part of the project, when requesting their stm we retrieve it from the master event instead
  get stm() {
    var _this$recurringEvent;
    return ((_this$recurringEvent = this.recurringEvent) === null || _this$recurringEvent === void 0 ? void 0 : _this$recurringEvent.stm) ?? super.stm;
  }
  set stm(stm) {
    super.stm = stm;
  }
  //region Early render
  get assigned() {
    const {
        project
      } = this,
      assigned = super.assigned;
    // Figure assigned events out before buckets are created  (if part of project)
    if (project !== null && project !== void 0 && project.isDelayingCalculation && !assigned) {
      return project.assignmentStore.storage.findItem('event', this);
    }
    return assigned;
  }
  set assigned(assigned) {
    super.assigned = assigned;
  }
  //endregion
  getCurrentConfig(options) {
    const {
        segments
      } = this,
      result = super.getCurrentConfig(options);
    // include segments
    if (result && segments) {
      result.segments = segments.map(segment => segment.getCurrentConfig(options));
    }
    return result;
  }
}
EventModel._$name = 'EventModel';

/**
 * @module SchedulerPro/data/EventStore
 */
/**
 * A store holding all the {@link SchedulerPro.model.EventModel events} to be rendered into a {@link SchedulerPro.view.SchedulerPro Scheduler Pro}.
 *
 * This store only accepts a model class inheriting from {@link SchedulerPro.model.EventModel}.
 *
 * An EventStore is usually connected to a project, which binds it to other related stores (AssignmentStore,
 * ResourceStore and DependencyStore). The project also handles normalization/calculation of the data on the records in
 * the store. For example if a record is added with a `startDate` and an `endDate`, it will calculate the `duration`.
 *
 * The calculations happens async, records are not guaranteed to have up to date data until they are finished. To be
 * certain that calculations have finished, call `await project.commitAsync()` after store actions. Or use one of the
 * `xxAsync` functions, such as `loadDataAsync()`.
 *
 * Using `commitAsync()`:
 *
 * ```javascript
 * eventStore.data = [{ startDate, endDate }, ...];
 *
 * // duration of the record is not yet calculated
 *
 * await eventStore.project.commitAsync();
 *
 * // now it is
 * ```
 *
 * Using `loadDataAsync()`:
 *
 * ```javascript
 * await eventStore.loadDataAsync([{ startDate, endDate }, ...]);
 *
 * // duration is calculated
 * ```
 *
 * @mixes SchedulerPro/data/mixin/PartOfProject
 * @mixes Scheduler/data/mixin/SharedEventStoreMixin
 * @mixes Scheduler/data/mixin/GetEventsMixin
 * @mixes Scheduler/data/mixin/EventStoreMixin
 * @mixes Scheduler/data/mixin/RecurringEventsMixin
 * @extends Core/data/AjaxStore
 *
 * @typings Scheduler/data/EventStore -> Scheduler/data/SchedulerEventStore
 */
class EventStore extends PartOfProject(SharedEventStoreMixin(RecurringEventsMixin(GetEventsMixin(DayIndexMixin(EventStoreMixin(ChronoEventStoreMixin.derive(AjaxStore))))))) {
  //region Config
  static $name = 'EventStore';
  static get defaultConfig() {
    return {
      modelClass: EventModel
    };
  }
  //endregion
}

EventStore._$name = 'EventStore';

const sortFn = (a, b) => {
  if (a < b) {
    return -1;
  } else if (a > b) {
    return 1;
  } else {
    return 0;
  }
};
/**
 * @module SchedulerPro/eventlayout/ProHorizontalLayout
 */
/**
 * Mixin for SchedulerPro horizontal layouts ({@link SchedulerPro.eventlayout.ProHorizontalLayoutPack} and
 * {@link SchedulerPro.eventlayout.ProHorizontalLayoutStack}). Should not be used directly, instead specify
 * {@link Scheduler.view.mixin.SchedulerEventRendering#config-eventLayout} in the SchedulerPro config (`stack`, `pack`
 * or `none`):
 *
 * ```javascript
 * new SchedulerPro({
 *   eventLayout: 'stack'
 * });
 * ```
 *
 * ## Grouping events
 *
 * By default events are not grouped and are laid out inside the row using start and end dates. Using
 * {@link #config-groupBy} config you can group events inside the resource row. Every group will be laid out on its own
 * band, as if layout was applied to each group of events separately.
 *
 * {@inlineexample SchedulerPro/eventlayout/ProHorizontalLayout.js}
 *
 * ### By field value
 *
 * You can specify field name to group events by. The following snippet would put *high* prio events at the top:
 *
 * ```javascript
 * new SchedulerPro({
 *     eventLayout : {
 *         type    : 'stack',
 *         groupBy : 'prio'
 *     },
 *     project : {
 *         eventsData : [
 *             { id : 1, startDate : '2017-02-08', duration : 1, prio : 'low' },
 *             { id : 2, startDate : '2017-02-09', duration : 1, prio : 'high' },
 *             { id : 3, startDate : '2017-02-10', duration : 1, prio : 'high' },
 *         ],
 *         resourcesData : [
 *             { id : 1, name : 'Resource 1' }
 *         ],
 *         assignmentsData : [
 *             { id : 1, resource : 1, event : 1 },
 *             { id : 2, resource : 1, event : 2 },
 *             { id : 3, resource : 1, event : 3 }
 *         ]
 *     }
 * })
 * ```
 *
 * ### Order of groups
 *
 * Groups are **always** sorted ascending. In the example above *high* prio events are above *low* prio events because:
 *
 * ```javascript
 * 'high' < 'low' // true
 * ```
 *
 * If you want to group events in a specific order, you can define it in a
 * special {@link #config-weights} config:
 *
 * ```javascript
 * new SchedulerPro({
 *     eventLayout : {
 *         type    : 'stack',
 *         weights : {
 *             low  : 100,
 *             high : 200
 *         },
 *         groupBy : 'prio'
 *     }
 * });
 * ```
 *
 * This will put *low* prio events at the top.
 *
 * The weight value defaults to `Infinity` unless specified in the weights config explicitly.
 *
 * ### Using a function
 *
 * You can use a custom function to group events. The group function receives an event record as a single argument and
 * is expected to return a non-null value for the group. This allows you to arrange events in any order you like,
 * including grouping by multiple properties at once.
 *
 * The snippet below groups events by duration and priority by creating 4 weights:
 *
 * |       | high prio | low prio |
 * |-------|-----------|----------|
 * | long  |     2     |    10    |
 * | short |     3     |    15    |
 *
 * ```javascript
 * new SchedulerPro({
 *     eventLayout : {
 *         type    : 'stack',
 *         groupBy : event => {
 *             return (event.duration > 2 ? 2 : 3) * (event.prio === 'high' ? 1 : 5);
 *         }
 *     }
 * })
 * ```
 *
 * This will divide events into 4 groups as seen in this demo:
 *
 * {@inlineexample SchedulerPro/eventlayout/ProHorizontalLayout2.js}
 *
 * ## Manual event layout
 *
 * You can provide a custom function to layout events inside the row and set the row size as required using
 * {@link #config-layoutFn}. The function is called with an array of {@link EventRenderData render data} objects. The
 * custom function can iterate over those objects and position them inside the row using `top` and `height` attributes.
 * The function should return the total row height in pixels.
 *
 * Please note that using a custom layout function makes {@link SchedulerPro.view.SchedulerPro#config-rowHeight}
 * obsolete.
 *
 * {@inlineexample SchedulerPro/eventlayout/ProHorizontalLayoutFn.js}
 *
 * ```javascript
 * new SchedulerPro({
 *     eventLayout : {
 *         layoutFn : items => {
 *             // Put event element at random top position
 *             item.top = 100 * Math.random();
 *         }
 *     }
 * });
 * ```
 *
 * @mixin
 */
var ProHorizontalLayout = (Target => class ProHorizontalLayout extends (Target || Base) {
  static get configurable() {
    return {
      /**
       * Type of horizontal layout. Supported values are `stack`, `pack` and `none`.
       * @config {'stack'|'pack'|'none'}
       */
      type: null,
      /**
       * The weights config allows you to specify order of the event groups inside the row. Higher weights are
       * placed further down in the row. If field value is not specified in the weights object, it will be
       * assigned `Infinity` value and pushed to the bottom.
       *
       * Only applicable when {@link #config-groupBy} config is not a function:
       *
       * ```javascript
       * new SchedulerPro({
       *     eventLayout : {
       *         type    : 'stack',
       *         weights : {
       *             // Events with high prio will be placed at the top, then medium,
       *             // then low prio events.
       *             high   : 100,
       *             medium : 150,
       *             low    : 200
       *         },
       *         groupBy : 'prio'
       *     }
       * });
       * ```
       *
       * Only explicitly defined groups are put in separate bands inside the row:
       *
       * ```javascript
       * new SchedulerPro({
       *     eventLayout : {
       *         // Pack layout is also supported
       *         type : 'pack',
       *         weights : {
       *             // Events with high prio will be placed at the top. All other
       *             // events will be put to the same group at the bottom
       *             high : 100
       *         },
       *         groupBy : 'prio'
       *     }
       * });
       * ```
       * @config {Object<String,Number>}
       */
      weights: null,
      /**
       * Specifies a way to group events inside the row. Can accept either a model field name or a function which
       * is provided with event record as a single argument and is expected to return group for the event.
       *
       * @config {String|Function}
       */
      groupBy: null,
      groupByThisObj: null,
      /**
       * Supply a function to manually layout events. It accepts event layout data and should set `top`
       * and `height` for every provided data item (left and width are calculated according to the event start
       * date and duration). The function should return the total row height in pixels.
       *
       * For example, we can arrange events randomly in the row:
       * ```javascript
       * new SchedulerPro({
       *     eventLayout : {
       *         layoutFn : items => {
       *             items.forEach(item => {
       *                 item.top = Math.round(Math.random() * 100);
       *                 item.height = Math.round(Math.random() * 100);
       *             });
       *
       *             return 50;
       *         }
       *     }
       * })
       * ```
       *
       * If you need a reference to the scheduler pro instance, you can get that from the function scope (arrow
       * function doesn't work here):
       *
       * ```javascript
       * new SchedulerPro({
       *     eventLayout : {
       *         layoutFn(items) {
       *             items.forEach(item => {
       *                 item.top = Math.round(Math.random() * 100);
       *                 item.height = Math.round(Math.random() * 100);
       *             });
       *
       *             // note `scheduler`, not `schedulerPro`
       *             return this.scheduler.rowHeight;
       *         }
       *     }
       * })
       * ```
       *
       * @config {Function}
       * @param {EventRenderData[]} events Unordered array of event render data, sorting may be required
       * @param {Scheduler.model.ResourceModel} resource The resource for which the events are being laid out.
       * @returns {Number} Returns total row height
       */
      layoutFn: null
    };
  }
  /**
   * This method performs layout on an array of event render data and returns amount of _bands_. Band is a multiplier of a
   * configured {@link Scheduler.view.Scheduler#config-rowHeight} to calculate total row height required to fit all
   * events.
   * This method should not be used directly, it is called by the Scheduler during the row rendering process.
   * @method applyLayout
   * @param {EventRenderData[]} events
   * @param {Scheduler.model.ResourceModel} resource
   * @returns {Number}
   */
  /**
   * This method iterates over events and calculates top position for each of them. Default layouts calculate
   * positions to avoid events overlapping horizontally (except for the 'none' layout). Pack layout will squeeze events to a single
   * row by reducing their height, Stack layout will increase the row height and keep event height intact.
   * This method should not be used directly, it is called by the Scheduler during the row rendering process.
   * @method layoutEventsInBands
   * @param {EventRenderData[]} events
   */
  /**
   * Returns `true` if event {@link #config-groupBy grouper} is defined.
   * @type {Boolean}
   * @readonly
   */
  get grouped() {
    return Boolean(this.groupBy);
  }
  /**
   * Returns group for the passed event render data.
   * @param {EventRenderData} layoutData
   * @returns {*}
   */
  getGroupValue(layoutData) {
    let result;
    if (layoutData.group != null) {
      result = layoutData.group;
    } else {
      const {
          groupBy,
          weights,
          groupByThisObj = this
        } = this,
        {
          eventRecord
        } = layoutData;
      if (typeof groupBy === 'function') {
        result = groupBy.call(groupByThisObj, eventRecord);
      } else {
        result = eventRecord[groupBy];
        if (weights) {
          // If record value is null or undefined, use infinite weight to move record to the bottom
          result = weights[result] ?? Infinity;
        }
      }
      layoutData.group = result;
    }
    return result;
  }
  /**
   * Sorts events by group and returns ordered array of groups, or empty array if events are not grouped.
   * @param {EventRenderData[]} events
   * @returns {String[]}
   */
  getEventGroups(events) {
    // If group fn is defined, we need to sort events array according to groups
    if (this.grouped) {
      const groups = new Set();
      events.sort((a, b) => {
        const aValue = this.getGroupValue(a),
          bValue = this.getGroupValue(b);
        groups.add(aValue);
        groups.add(bValue);
        return sortFn(aValue, bValue);
      });
      return Array.from(groups).sort(sortFn);
    } else {
      return [];
    }
  }
});

/**
 * @module SchedulerPro/eventlayout/ProHorizontalLayoutStack
 */
/**
 * Handles layout of events within a row (resource) in horizontal mode. Stacks events, increasing row height to fit
 * all overlapping events.
 *
 * This layout is used by default in horizontal mode.
 *
 * This layout supports grouping events inside the resource row. See
 * {@link SchedulerPro.eventlayout.ProHorizontalLayout} for more info.
 *
 * @mixes SchedulerPro/eventlayout/ProHorizontalLayout
 */
class ProHorizontalLayoutStack extends HorizontalLayoutStack.mixin(ProHorizontalLayout) {
  static get $name() {
    return 'ProHorizontalLayoutStack';
  }
  /**
   * @hideconfigs type, weights, groupBy, layoutFn
   */
  // heightRun is used when pre-calculating row heights, taking a cheaper path
  layoutEventsInBands(events, heightRun = false) {
    this.getEventGroups(events);
    return super.layoutEventsInBands(events, heightRun);
  }
}
ProHorizontalLayoutStack._$name = 'ProHorizontalLayoutStack';

/**
 * @module SchedulerPro/eventlayout/ProHorizontalLayoutPack
 */
/**
 * Handles layout of events within a row (resource) in horizontal mode. Packs events (adjusts their height) to fit
 * available row height.
 *
 * This layout supports grouping events inside the resource row. See
 * {@link SchedulerPro.eventlayout.ProHorizontalLayout} for more info.
 *
 * @mixes SchedulerPro/eventlayout/ProHorizontalLayout
 */
class ProHorizontalLayoutPack extends HorizontalLayoutPack.mixin(ProHorizontalLayout) {
  static get $name() {
    return 'ProHorizontalLayoutPack';
  }
  /**
   * @hideconfigs type, weights, groupBy, layoutFn
   */
  layoutEventsInBands(events) {
    const groups = this.getEventGroups(events),
      // If we don't have any groups, treat it like we have a single group including all events
      groupCount = groups.length || 1;
    const result = this.packEventsInBands(events, (event, j, slot, slotSize) => {
      const size = slotSize / groupCount,
        groupIndex = groupCount === 1 ? 0 : groups.indexOf(event.group),
        adjustedSlotStart = groupIndex / groupCount;
      // This height and top are used to position event in the grouped row
      event.height = size;
      event.top = adjustedSlotStart + slot.start / groupCount + j * size;
      // This height and top are used to layout events in the same band. They emulate a single row which is what
      // pack logic expects
      event.inBandHeight = slotSize;
      event.inBandTop = slot.start + j * slotSize;
    });
    events.forEach(event => {
      Object.assign(event, this.bandIndexToPxConvertFn.call(this.bandIndexToPxConvertThisObj || this, event.top, event.height, event.eventRecord, event.resourceRecord));
    });
    return result;
  }
}
ProHorizontalLayoutPack._$name = 'ProHorizontalLayoutPack';

/**
 * @module SchedulerPro/feature/CalendarHighlight
 */
let counter = 0;
class CalendarHighlightModel extends ResourceTimeRangeModel {
  static get $name() {
    return 'CalendarHighlightModel';
  }
  static domIdPrefix = 'calendarhighlight';
  // For nicer DOM, since the records are transient we do not need a fancy UUID
  static generateId() {
    return ++counter;
  }
}
/**
 * This feature temporarily visualizes {@link SchedulerPro/model/CalendarModel calendars} for the event or resource
 * calendar (controlled by the {@link #config-calendar} config). The calendars are highlighted while a user is creating,
 * dragging or resizing a task. Enabling this feature makes it easier for the end user to understand the underlying
 * rules of the schedule.
 *
 * {@inlineexample SchedulerPro/feature/CalendarHighlight.js}
 *
 * ## Example usage
 *
 * ```javascript
 * new SchedulerPro({
 *     features : {
 *         calendarHighlight : {
 *             // visualize resource calendars while interacting with events
 *             calendar : 'resource'
 *         }
 *     }
 * })
 * ```
 *
 * This feature is **disabled** by default.
 *
 * @extends Scheduler/feature/base/ResourceTimeRangesBase
 * @classtype calendarHighlight
 * @feature
 * @demo SchedulerPro/highlight-event-calendars
 */
class CalendarHighlight extends ResourceTimeRangesBase {
  //region Config
  static get $name() {
    return 'CalendarHighlight';
  }
  static get configurable() {
    return {
      /**
       * A string defining which calendar(s) to highlight during drag drop, resize or create flows.
       * Valid values are `event` or `resource`.
       *
       * @config {'event'|'resource'}
       * @default
       */
      calendar: 'event',
      /**
       * A string defining which calendar(s) to highlight during drag drop, resize or create flows.
       * Valid values are `event` or `resource`.
       *
       * @config {'event'|'resource'}
       */
      unhighlightOnDrop: null,
      /**
       * A callback function which is called when you interact with one or more events (e.g. drag drop) to
       * highlight only available resources.
       *
       * ```javascript
       * new SchedulerPro({
       *     features : {
       *         calendarHighlight : {
       *             collectAvailableResources({ scheduler, eventRecords }) {
       *                  const mainEvent = eventRecords[0];
       *                  return scheduler.resourceStore.query(resource => resource.role === mainEvent.requiredRole || !mainEvent.requiredRole);
       *              }
       *         }
       *     }
       * });
       * ```
       *
       * @param {Object} context A context object
       * @param {SchedulerPro.view.SchedulerPro} context.scheduler The scheduler instance
       * @param {Scheduler.model.EventModel[]} context.eventRecords The event records
       * @returns {Scheduler.model.ResourceModel[]} An array with the available resource records
       * @config {Function}
       */
      collectAvailableResources: null,
      rangeCls: 'b-sch-highlighted-calendar-range',
      resourceTimeRangeModelClass: CalendarHighlightModel,
      inflate: 3
    };
  }
  static get pluginConfig() {
    const config = super.pluginConfig;
    config.assign = ['highlightEventCalendars', 'highlightResourceCalendars', 'unhighlightCalendars'];
    return config;
  }
  afterConstruct() {
    super.afterConstruct();
    this.client.ion({
      eventDragStart: 'onEventDragStart',
      eventDragReset: 'unhighlightCalendars',
      eventResizeStart: 'onEventResizeStart',
      eventResizeEnd: 'unhighlightCalendars',
      dragCreateStart: 'onDragCreateStart',
      afterDragCreate: 'unhighlightCalendars',
      thisObj: this
    });
  }
  //endregion
  highlightCalendar(eventRecords, resourceRecords) {
    eventRecords = ArrayHelper.asArray(eventRecords);
    resourceRecords = ArrayHelper.asArray(resourceRecords);
    if (this.calendar === 'event') {
      this.highlightEventCalendars(eventRecords, resourceRecords);
    } else {
      this.highlightResourceCalendars(resourceRecords);
    }
  }
  // region public APIs
  /**
   * Highlights the time spans representing the calendars of the passed event records, and resource records.
   * @on-owner
   * @param {Scheduler.model.EventModel[]} eventRecords The event records
   * @param {Scheduler.model.ResourceModel[]} [resourceRecords] The resource records
   * @param {Boolean} [clearExisting] Provide `false` to leave previous highlight elements
   */
  highlightEventCalendars(eventRecords, resourceRecords, clearExisting = true) {
    const me = this,
      {
        client
      } = me,
      {
        startDate,
        endDate
      } = client;
    if (me.disabled) {
      return;
    }
    if (clearExisting) {
      me.unhighlightCalendars();
    }
    eventRecords = ArrayHelper.asArray(eventRecords);
    if (!resourceRecords) {
      resourceRecords = eventRecords.flatMap(event => event.$linkedResources);
    }
    me.highlight = new Map();
    resourceRecords = ArrayHelper.asArray(resourceRecords);
    eventRecords.forEach(eventRecord => {
      var _eventRecord$calendar;
      if (!eventRecord.calendar) {
        return;
      }
      const timespans = (_eventRecord$calendar = eventRecord.calendar) === null || _eventRecord$calendar === void 0 ? void 0 : _eventRecord$calendar.getWorkingTimeRanges(startDate, endDate).map(timespan => new CalendarHighlightModel(timespan));
      if (timespans) {
        for (const resourceRecord of resourceRecords) {
          me.highlight.set(resourceRecord, timespans);
          client.currentOrientation.refreshEventsForResource(resourceRecord, true, false);
        }
        if (resourceRecords.length > 0) {
          client.currentOrientation.onRenderDone();
        }
      }
    });
  }
  /**
   * Highlights the time spans representing the working time calendars of the passed resource records.
   * @on-owner
   * @param {Scheduler.model.ResourceModel[]} resourceRecords The resource records
   * @param {Boolean} [clearExisting] Provide `false` to leave previous highlight elements
   */
  highlightResourceCalendars(resourceRecords, clearExisting = true) {
    const me = this,
      {
        startDate,
        endDate,
        currentOrientation
      } = me.client;
    if (me.disabled) {
      return;
    }
    if (clearExisting) {
      me.unhighlightCalendars();
    }
    // Highlight resource calendars
    me.highlight = new Map();
    for (const resourceRecord of resourceRecords) {
      var _resourceRecord$calen;
      const timespans = (_resourceRecord$calen = resourceRecord.calendar) === null || _resourceRecord$calen === void 0 ? void 0 : _resourceRecord$calen.getWorkingTimeRanges(startDate, endDate).map(timespan => new CalendarHighlightModel(timespan));
      if (timespans) {
        me.highlight.set(resourceRecord, timespans);
        currentOrientation.refreshEventsForResource(resourceRecord, true, false);
      }
    }
    if (resourceRecords.length > 0) {
      currentOrientation.onRenderDone();
    }
  }
  /**
   * Removes all highlight elements.
   * @on-owner
   */
  unhighlightCalendars() {
    if (!this.highlight) {
      // We're not highlighting anything, bail out
      return;
    }
    const {
        currentOrientation
      } = this.client,
      resources = this.highlight.keys();
    this.highlight = null;
    for (const resource of resources) {
      currentOrientation.refreshEventsForResource(resource, true, false);
    }
    currentOrientation.onRenderDone();
  }
  // endregion
  // region event listeners
  onEventDragStart({
    context
  }) {
    var _me$collectAvailableR;
    if (this.disabled) {
      return;
    }
    const me = this,
      {
        client
      } = me,
      {
        eventRecords
      } = context,
      resourceRecords = context.availableResources = client.features.eventDrag.constrainDragToResource ? [context.resourceRecord] : ((_me$collectAvailableR = me.collectAvailableResources) === null || _me$collectAvailableR === void 0 ? void 0 : _me$collectAvailableR.call(me, {
        scheduler: client,
        eventRecords
      })) ?? client.resourceStore.records;
    me.highlightCalendar(eventRecords, resourceRecords);
  }
  onEventResizeStart({
    eventRecord,
    resourceRecord
  }) {
    if (!this.disabled) {
      this.highlightCalendar(eventRecord, [resourceRecord]);
    }
  }
  onDragCreateStart({
    eventRecord,
    resourceRecord
  }) {
    if (!this.disabled) {
      this.highlightCalendar(eventRecord, [resourceRecord]);
    }
  }
  // endregion
  // Called on render of resources events to get events to render. Add any ranges
  // (chained function from Scheduler)
  getEventsToRender(resource, events) {
    var _this$highlight;
    const timespans = (_this$highlight = this.highlight) === null || _this$highlight === void 0 ? void 0 : _this$highlight.get(resource);
    timespans && events.push(...timespans);
    return events;
  }
  onEventDataGenerated(renderData) {
    const {
      eventRecord
    } = renderData;
    if (eventRecord.isCalendarHighlightModel) {
      const {
        inflate
      } = this;
      // Flag that we should fill entire row/col
      renderData.fillSize = this.client.isVertical;
      // Add our own cls
      renderData.wrapperCls['b-sch-highlighted-calendar-range'] = 1;
      // Add label
      renderData.children.push({
        className: 'b-sch-event-content',
        html: eventRecord.name,
        dataset: {
          taskBarFeature: 'content'
        }
      });
      // Inflate
      renderData.width += inflate * 2;
      renderData.height += inflate * 2;
      renderData.left -= inflate;
      renderData.top -= inflate;
      // Event data for DOMSync comparison, unique per calendar & resource combination
      renderData.eventId = `${this.generateElementId(eventRecord)}-resource-${renderData.resourceRecord.id}`;
    }
  }
  updateDisabled(disabled, was) {
    super.updateDisabled(disabled, was);
    if (disabled) {
      this.unhighlightCalendars();
    }
  }
  shouldInclude(eventRecord) {
    return eventRecord.isCalendarHighlightModel;
  }
  // No classname on Scheduler's/Gantt's element
  get featureClass() {}
}
CalendarHighlight._$name = 'CalendarHighlight';
GridFeatureManager.registerFeature(CalendarHighlight, false, 'SchedulerPro');

/**
 * @module SchedulerPro/feature/DependencyEdit
 */
/**
 * Feature that displays a popup containing fields for editing dependency data.
 *
 * This feature is **off** by default. For info on enabling it, see {@link Grid/view/mixin/GridFeatures}.
 *
 * @extends Scheduler/feature/DependencyEdit
 * @inlineexample SchedulerPro/feature/DependencyEdit.js
 * @typings Scheduler/feature/DependencyEdit -> Scheduler/feature/SchedulerDependencyEdit
 * @demo SchedulerPro/dependencies/
 * @classtype dependencyEdit
 * @feature
 */
class DependencyEdit extends DependencyEdit$1 {
  //region Config
  static get $name() {
    return 'DependencyEdit';
  }
  static get configurable() {
    return {
      /**
       * True to show the lag field for the dependency
       * @config {Boolean}
       * @default
       * @category Editor widgets
       */
      showLagField: true,
      editorConfig: {
        items: {
          activeField: {
            type: 'checkbox',
            name: 'active',
            label: 'L{Active}'
          }
        }
      }
    };
  }
  //endregion
}

DependencyEdit._$name = 'DependencyEdit';
GridFeatureManager.registerFeature(DependencyEdit, false);

/**
 * @module SchedulerPro/feature/EventBuffer
 */
/**
 * Feature that allows showing additional time before & after an event, to visualize things like travel time - or the time you
 * need to prepare a room for a meeting + clean it up after.
 *
 * The feature relies on two model fields: {@link SchedulerPro.model.EventModel#field-preamble} and
 * {@link SchedulerPro.model.EventModel#field-postamble} which are used to calculate overall start and end dates used to
 * position the event. Buffer time overlaps the same way events overlap (as you can see in the inline demo below). It
 * should also be noted that buffer time is ignored for milestones.
 *
 * {@inlineexample SchedulerPro/feature/EventBuffer.js}
 *
 * This feature is **disabled** by default
 *
 * @extends Core/mixin/InstancePlugin
 * @classtype eventBuffer
 * @feature
 * @demo SchedulerPro/travel-time
 */
class EventBuffer extends InstancePlugin {
  static get $name() {
    return 'EventBuffer';
  }
  static get configurable() {
    return {
      /**
       * Show buffer duration labels
       * @config {Boolean}
       * @default
       */
      showDuration: true,
      /**
       * A function which receives data about the buffer time and returns a html string to show in a tooltip when
       * hovering a buffer time element
       * @param {Object} data Data
       * @param {Core.data.Duration} data.duration Buffer time duration
       * @param {Boolean} data.before `true` if this is a buffer time before the event start, `false` if after
       * @param {SchedulerPro.model.EventModel} data.eventRecord The event record
       * @config {Function}
       */
      tooltipTemplate: {
        value: null,
        $config: 'nullify'
      }
    };
  }
  static get pluginConfig() {
    return {
      chain: ['onEventDataGenerated']
    };
  }
  //region Chained methods
  updateTooltipTemplate(tooltipTemplate) {
    const me = this;
    if (tooltipTemplate) {
      me.tooltip = Tooltip.new({
        forElement: me.client.timeAxisSubGridElement,
        forSelector: '.b-sch-event-buffer-before,.b-sch-event-buffer-after',
        align: {
          align: 'b-t',
          offset: [0, 10]
        },
        getHtml({
          activeTarget
        }) {
          const eventRecord = me.client.resolveEventRecord(activeTarget),
            before = activeTarget.matches('.b-sch-event-buffer-before'),
            duration = before ? eventRecord.preamble : eventRecord.postamble;
          return me.tooltipTemplate({
            eventRecord,
            duration,
            before
          });
        }
      });
    } else {
      var _me$tooltip;
      (_me$tooltip = me.tooltip) === null || _me$tooltip === void 0 ? void 0 : _me$tooltip.destroy();
    }
  }
  onEventDataGenerated({
    useEventBuffer,
    bufferBeforeWidth,
    bufferAfterWidth,
    eventRecord,
    wrapperChildren
  }) {
    if (this.enabled && useEventBuffer) {
      const {
          isHorizontal
        } = this.client,
        {
          showDuration
        } = this,
        {
          preamble,
          postamble
        } = eventRecord,
        sizeProp = isHorizontal ? 'width' : 'height';
      // Buffer elements should always be there, otherwise animation might get wrong
      wrapperChildren.push({
        className: {
          'b-sch-event-buffer': 1,
          'b-sch-event-buffer-before': 1,
          'b-buffer-thin': !bufferBeforeWidth
        },
        style: `${sizeProp}: ${bufferBeforeWidth}px`,
        children: showDuration && preamble ? [{
          tag: 'span',
          className: 'b-buffer-label',
          html: preamble.toString(true)
        }] : undefined
      }, {
        className: {
          'b-sch-event-buffer': 1,
          'b-sch-event-buffer-after': 1,
          'b-buffer-thin': !bufferAfterWidth
        },
        style: `${sizeProp}: ${bufferAfterWidth}px`,
        children: showDuration && postamble ? [{
          tag: 'span',
          className: 'b-buffer-label',
          html: postamble.toString(true)
        }] : undefined
      });
    }
  }
  //endregion
  updateShowDuration() {
    if (!this.isConfiguring) {
      this.client.refreshWithTransition();
    }
  }
  doDisable(disable) {
    super.doDisable(disable);
    const {
      client
    } = this;
    if (!client.isConfiguring && client.isPainted) {
      // Add a special CSS class to disable certain transitions
      client.element.classList.add('b-eventbuffer-transition');
      client.refreshWithTransition();
      client.waitForAnimations().then(() => {
        client.element.classList.remove('b-eventbuffer-transition');
      });
    }
  }
}
EventBuffer._$name = 'EventBuffer';
GridFeatureManager.registerFeature(EventBuffer, false, 'SchedulerPro');

/**
 * @module SchedulerPro/feature/EventSegmentDrag
 */
/**
 * Allows user to drag and drop event segments within the row.
 *
 * {@inlineexample SchedulerPro/feature/EventSegments.js}
 *
 * This feature is **enabled** by default
 *
 * @extends Scheduler/feature/EventDrag
 * @classtype eventSegmentDrag
 * @feature
 */
class EventSegmentDrag extends EventDrag {
  //region Config
  static $name = 'EventSegmentDrag';
  static get defaultConfig() {
    return {
      constrainDragToResource: true
    };
  }
  static get configurable() {
    return {
      capitalizedEventName: 'EventSegment'
    };
  }
  static get pluginConfig() {
    return {
      chain: ['onPaint', 'isEventElementDraggable']
    };
  }
  //endregion
  //region Events
  /**
   * Fired on the owning Scheduler to allow implementer to use asynchronous finalization by setting
   * `context.async = true` in the listener, to show a confirmation popup etc.
   * ```javascript
   *  scheduler.on('beforeEventSegmentDropFinalize', ({ context }) => {
   *      context.async = true;
   *      setTimeout(() => {
   *          // async code don't forget to call finalize
   *          context.finalize();
   *      }, 1000);
   *  })
   * ```
   *
   * For synchronous one-time validation, simply set `context.valid` to true or false.
   * ```javascript
   *  scheduler.on('beforeEventSegmentDropFinalize', ({ context }) => {
   *      context.valid = false;
   *  })
   * ```
   * @event beforeEventSegmentDropFinalize
   * @on-owner
   * @param {Scheduler.view.Scheduler} source Scheduler instance
   * @param {Object} context
   * @param {Boolean} context.async Set true to not finalize the drag-drop operation immediately (e.g. to wait for user confirmation)
   * @param {Scheduler.model.EventModel[]} context.eventRecords Dragged segments
   * @param {Boolean} context.valid Set this to `false` to abort the drop immediately.
   * @param {Function} context.finalize Call this method after an **async** finalization flow, to finalize the drag-drop operation. This method accepts one
   * argument: pass `true` to update records, or `false` to ignore changes
   */
  /**
   * Fired on the owning Scheduler after an event segment is dropped
   * @event afterEventSegmentDrop
   * @on-owner
   * @param {Scheduler.view.Scheduler} source
   * @param {Scheduler.model.EventModel[]} eventRecords Dropped segments
   * @param {Boolean} valid
   * @param {Object} context
   */
  /**
   * Fired on the owning Scheduler when an event segment is dropped
   * @event eventSegmentDrop
   * @on-owner
   * @param {Scheduler.view.Scheduler} source
   * @param {Scheduler.model.EventModel[]} eventRecords Dropped segments
   */
  /**
   * Fired on the owning Scheduler before event segment dragging starts. Return `false` to prevent the action.
   * @event beforeEventSegmentDrag
   * @on-owner
   * @preventable
   * @param {Scheduler.view.Scheduler} source Scheduler instance
   * @param {Scheduler.model.EventModel[]} eventRecords Segments to drag
   * @param {MouseEvent} event Browser event
   */
  /**
   * Fired on the owning Scheduler when event segment dragging starts
   * @event eventSegmentDragStart
   * @on-owner
   * @param {Scheduler.view.Scheduler} source Scheduler instance
   * @param {Scheduler.model.EventModel[]} eventRecords Dragged segments
   * @param {MouseEvent} event Browser event
   */
  /**
   * Fired on the owning Scheduler when event segments are dragged
   * @event eventSegmentDrag
   * @on-owner
   * @param {Scheduler.view.Scheduler} source Scheduler instance
   * @param {Scheduler.model.EventModel[]} eventRecords Dragged segments
   * @param {Date} startDate Start date for the current location
   * @param {Date} endDate End date for the current location
   * @param {Object} context
   * @param {Boolean} context.valid Set this to `false` to signal that the current drop position is invalid.
   */
  /**
   * Fired on the owning Scheduler after an event segment drag operation has been aborted
   * @event eventSegmentDragAbort
   * @on-owner
   * @param {Scheduler.view.Scheduler} source Scheduler instance
   * @param {Scheduler.model.EventModel[]} eventRecords Dragged segments
   */
  /**
   * Fired on the owning Scheduler after an event segment drag operation regardless of the operation being cancelled
   * or not
   * @event eventSegmentDragReset
   * @on-owner
   * @param {Scheduler.view.Scheduler} source Scheduler instance
   */
  //endregion
  //region Drag events
  getTriggerParams(dragData) {
    const {
      assignmentRecords,
      eventRecords,
      resourceRecord,
      browserEvent: event
    } = dragData;
    return {
      // `context` is now private, but used in WebSocketHelper
      context: dragData,
      eventRecords,
      resourceRecord,
      assignmentRecords,
      event
    };
  }
  triggerEventDrag(dragData, start) {
    this.scheduler.trigger('eventSegmentDrag', Object.assign(this.getTriggerParams(dragData), {
      startDate: dragData.startDate,
      endDate: dragData.endDate
    }));
  }
  triggerDragStart(dragData) {
    this.scheduler.navigator.skipNextClick = true;
    this.scheduler.trigger('eventSegmentDragStart', this.getTriggerParams(dragData));
  }
  triggerDragAbort(dragData) {
    this.scheduler.trigger('eventSegmentDragAbort', this.getTriggerParams(dragData));
  }
  triggerDragAbortFinalized(dragData) {
    this.scheduler.trigger('eventSegmentDragAbortFinalized', this.getTriggerParams(dragData));
  }
  triggerAfterDrop(dragData, valid) {
    this.scheduler.trigger('afterEventSegmentDrop', Object.assign(this.getTriggerParams(dragData), {
      valid
    }));
    if (!valid) {
      // Edge cases:
      // 1. If this drag was a no-op, and underlying data was changed while drag was ongoing (e.g. web socket
      // push), we need to manually force a view refresh to ensure a correct render state
      //
      // or
      // 2. Events were removed before we dropped at an invalid point
      const {
          assignmentStore,
          eventStore
        } = this.client,
        needRefresh = this.dragData.initialAssignmentsState.find(({
          resource,
          assignment
        }, i) => {
          var _this$dragData$assign;
          return !assignmentStore.includes(assignment) || !eventStore.includes(assignment.event) || resource.id !== ((_this$dragData$assign = this.dragData.assignmentRecords[i]) === null || _this$dragData$assign === void 0 ? void 0 : _this$dragData$assign.resourceId);
        });
      if (needRefresh) {
        this.client.refresh();
      }
    }
  }
  //endregion
  //region Update records
  /**
   * Update events being dragged.
   * @private
   * @param context Drag data.
   * @async
   */
  async updateRecords(context) {
    const me = this,
      {
        client
      } = me,
      copyKeyPressed = false;
    let result;
    if (!context.externalDropTarget) {
      client.eventStore.suspendAutoCommit();
      result = await me.updateSegment(client, context, copyKeyPressed);
      client.eventStore.resumeAutoCommit();
    }
    // Tell the world there was a successful drop
    client.trigger('eventSegmentDrop', Object.assign(me.getTriggerParams(context), {
      isCopy: copyKeyPressed,
      event: context.browserEvent,
      targetEventRecord: context.targetEventRecord,
      targetResourceRecord: context.newResource,
      externalDropTarget: context.externalDropTarget
    }));
    return result;
  }
  /**
   * Update assignments being dragged
   * @private
   * @async
   */
  async updateSegment(client, context) {
    // The code is written to emit as few store events as possible
    const me = this,
      isVertical = client.mode === 'vertical',
      {
        eventRecords,
        assignmentRecords,
        timeDiff
      } = context;
    client.suspendRefresh();
    let updated = false;
    if (isVertical) {
      eventRecords.forEach((draggedEvent, i) => {
        const eventBar = context.eventBarEls[i];
        delete draggedEvent.instanceMeta(client).hasTemporaryDragElement;
        // If it was created by a call to scheduler.currentOrientation.addTemporaryDragElement
        // then release it back to be available to DomSync next time the rendered event block
        // is synced.
        if (eventBar.dataset.transient) {
          eventBar.remove();
        }
      });
    }
    const eventBarEls = context.eventBarEls.slice(),
      draggedEvent = context.eventRecord,
      newStartDate = me.adjustStartDate(context.origStart, timeDiff);
    if (!DateHelper.isEqual(draggedEvent.startDate, newStartDate)) {
      var _me$endBatchUpdate;
      client.endListeningForBatchedUpdates();
      me.cancelBatchUpdate(draggedEvent);
      draggedEvent.startDate = newStartDate;
      updated = true;
      await client.project.commitAsync();
      (_me$endBatchUpdate = me.endBatchUpdate) === null || _me$endBatchUpdate === void 0 ? void 0 : _me$endBatchUpdate.call(me, draggedEvent);
    }
    client.resumeRefresh();
    if (assignmentRecords.length > 0) {
      if (!updated) {
        context.valid = false;
      } else {
        // https://github.com/bryntum/support/issues/630
        // Force re-render when using fillTicks. If date changed within same tick the element won't actually
        // change and since we hijacked it for drag it won't be returned to its original position
        if (client.fillTicks) {
          eventBarEls.forEach(el => delete el.lastDomConfig);
        }
        // Not doing full refresh above, to allow for animations
        client.refreshWithTransition();
      }
    }
  }
  //endregion
  //region Drag data
  // Prevent event draggind when it starts over a resize handle
  isEventElementDraggable(eventElement, eventRecord, el, event) {
    const me = this;
    // ALLOW event drag:
    // - if segments dragging is disabled or event is not segmented
    if (me.disabled || !(eventRecord.isEventSegment || eventRecord.segments)) {
      return true;
    }
    // otherwise make sure EventDrag is not trying to handle a segment element drag
    return !el.closest(me.drag.targetSelector);
  }
  buildDragHelperConfig() {
    const config = super.buildDragHelperConfig();
    config.targetSelector = '.b-sch-event-segment:not(.b-first)';
    return config;
  }
  getMinimalDragData(info) {
    const me = this,
      {
        client
      } = me,
      element = me.getElementFromContext(info),
      eventRecord = client.resolveEventRecord(element),
      resourceRecord = client.resolveResourceRecord(element),
      assignmentRecord = client.resolveAssignmentRecord(element),
      assignmentRecords = assignmentRecord ? [assignmentRecord] : [],
      eventRecords = [eventRecord];
    return {
      eventRecord,
      resourceRecord,
      assignmentRecord,
      eventRecords,
      assignmentRecords
    };
  }
  beginBatchUpdate(eventRecord) {
    eventRecord.event.beginBatch();
    eventRecord.beginBatch();
  }
  endBatchUpdate(eventRecord) {
    var _eventRecord$event;
    // could be no "event" if segments got merged after dragging
    (_eventRecord$event = eventRecord.event) === null || _eventRecord$event === void 0 ? void 0 : _eventRecord$event.endBatch();
    eventRecord.endBatch();
  }
  cancelBatchUpdate(eventRecord) {
    var _eventRecord$event2;
    (_eventRecord$event2 = eventRecord.event) === null || _eventRecord$event2 === void 0 ? void 0 : _eventRecord$event2.cancelBatch();
    eventRecord.cancelBatch();
  }
  setupProductDragData(info) {
    var _me$getDateConstraint;
    const me = this,
      {
        client
      } = me,
      element = me.getElementFromContext(info),
      {
        eventRecord,
        resourceRecord
      } = me.getMinimalDragData(info),
      eventBarEls = [],
      mainEventElement = client.getElementsFromEventRecord(eventRecord.event, resourceRecord, true)[0];
    if (me.constrainDragToResource && !resourceRecord) {
      throw new Error('Resource could not be resolved for event: ' + eventRecord.id);
    }
    // We tweak last segment drag in RTL mode so its X-ccordinate is always zero
    // so we have to tell DragHelper to still process corresponding drop event though
    // the coordinate hasn't changed
    me.drag.ignoreSamePositionDrop = !client.rtl || eventRecord.nextSegment;
    // During this batch we want the client's UI to update itself using the proposed changes
    // Only if startDrag has not already done it
    if (!client.listenToBatchedUpdates) {
      client.beginListeningForBatchedUpdates();
    }
    // Do changes in batch mode while dragging
    me.beginBatchUpdate(eventRecord);
    const dateConstraints = (_me$getDateConstraint = me.getDateConstraints) === null || _me$getDateConstraint === void 0 ? void 0 : _me$getDateConstraint.call(me, resourceRecord, eventRecord),
      constrainRectangle = me.constrainRectangle = me.getConstrainingRectangle(dateConstraints, resourceRecord, eventRecord),
      eventRegion = Rectangle.from(element, client.foregroundCanvas, true),
      mainEventRegion = Rectangle.from(mainEventElement, client.foregroundCanvas, true);
    // For segment we shift constrainRectangle by the main event offset
    constrainRectangle.translate(-mainEventRegion.x);
    super.setupConstraints(constrainRectangle, eventRegion, client.timeAxisViewModel.snapPixelAmount, Boolean(dateConstraints.start));
    eventBarEls.push(element);
    return {
      record: eventRecord,
      draggedEntities: [eventRecord],
      dateConstraints: dateConstraints !== null && dateConstraints !== void 0 && dateConstraints.start ? dateConstraints : null,
      eventBarEls,
      mainEventElement
    };
  }
  suspendRecordElementRedrawing() {}
  suspendElementRedrawing() {}
  getDateConstraints(resourceRecord, eventRecord) {
    let {
      minDate,
      maxDate
    } = super.getDateConstraints(resourceRecord, eventRecord);
    // A segment movement is constrained by its neighbour segments if any
    if (eventRecord.previousSegment && (!minDate || minDate < eventRecord.previousSegment.endDate)) {
      minDate = eventRecord.previousSegment.endDate;
    }
    if (eventRecord.nextSegment && (!maxDate || maxDate < eventRecord.nextSegment.startDate)) {
      maxDate = eventRecord.nextSegment.startDate;
    }
    return {
      start: minDate,
      end: maxDate
    };
  }
  get tipId() {
    return `${this.client.id}-segment-drag-tip`;
  }
  internalSnapToPosition(snapTo) {
    super.internalSnapToPosition();
    // for RTL we pin last segment to 0px offset ..the main event element will get updated
    if (this.client.rtl && !this.dragData.eventRecord.nextSegment) {
      snapTo.x = 0;
    }
  }
  updateDragContext(context, event) {
    super.updateDragContext(...arguments);
    const {
        client
      } = this,
      {
        dirty,
        eventRecord,
        endDate
      } = this.dragData;
    // If dragging the last segment update the main event width accordingly
    // need this to update dependency properly while dragging
    if (dirty && !eventRecord.nextSegment) {
      var _client$features$even;
      const {
        enableEventAnimations
      } = client;
      client.enableEventAnimations = false;
      eventRecord.event.set('endDate', endDate);
      if ((_client$features$even = client.features.eventBuffer) !== null && _client$features$even !== void 0 && _client$features$even.enabled) {
        eventRecord.event.wrapEndDate = endDate;
      }
      client.enableEventAnimations = enableEventAnimations;
    }
  }
  //endregion
}

EventSegmentDrag._$name = 'EventSegmentDrag';
GridFeatureManager.registerFeature(EventSegmentDrag, true, 'SchedulerPro');
GridFeatureManager.registerFeature(EventSegmentDrag, false, 'ResourceHistogram');

/**
 * @module SchedulerPro/feature/NestedEvents
 */
const borderWidths = {
  border: 1,
  hollow: 2
};
// Future improvements might include:
// * Add info to EventTooltip, parent could display number of children, child could display parent name
// * Add parent picker to EventEdit
// * Handle reassigning in editor, what happens if you reassign to a resource that events parent is not assigned to...
/**
 * A feature that renders child events nested inside their parent. Requires Scheduler Pro to use a tree event store
 * (normally handled automatically when events in data has children) and it is limited to one level of nesting.
 *
 * {@inlineexample SchedulerPro/feature/NestedEvents.js}
 *
 * The feature has configs for {@link #config-eventLayout}, {@link #config-resourceMargin} and {@link #config-barMargin}
 * that are separate from those on Scheduler Pro and only affect nested events.
 *
 * You can by default drag nested events out of their parents and drop any event onto root level events to nest. The
 * drag and drop behaviour can be customized using the {@link #config-constrainDragToParent},
 * {@link #config-allowNestingOnDrop} and {@link #config-allowDeNestingOnDrop} configs.
 *
 * <div class="note">Note that for a nested event to show up for a resource both the parent and the nested event has to
 * be assigned to that resource.</div>
 *
 * ## Parent / children scheduling
 *
 * Scheduler Pro uses a scheduling engine closely related to the one used by Gantt (a subset of it). It for example
 * schedules based on calendars, dependencies and constraints. Part of its default logic is that parent events start and
 * end dates (and thus duration) is defined by their children. This means that if you remove the latest scheduled child
 * of a parent, the parents end date and duration will be adjusted to match the new latest scheduled child.
 *
 * Depending on what you plan to use nested events for in your application, this might not be the desired behaviour. If
 * you want the parent element to keep its dates regardless of its children, you should flag it as
 * {@link SchedulerPro/model/EventModel#field-manuallyScheduled}.
 *
 * A parent defined like this will shrink / grow with its children:
 *
 * ```json
 * {
 *     "id"        : 1,
 *     "startDate" : "2022-03-24",
 *     "children"  : [
 *         ...
 *     ]
 * }
 * ```
 *
 * Try removing an event here to see what happens:
 *
 * {@inlineexample SchedulerPro/feature/NestedEventsNotManually.js}
 *
 * A parent with `manuallyScheduled : true` will **not** shrink / grow with is children:
 *
 * ```json
 * {
 *     "id"                : 1,
 *     "startDate"         : "2022-03-24",
 *     "duration"          : 10,
 *     "manuallyScheduled" : true
 *     "children"          : [
 *         ...
 *     ]
 * }
 * ```
 *
 * Try the same thing here:
 *
 * {@inlineexample SchedulerPro/feature/NestedEventsManually.js}
 *
 * <div class="note">Note that this also makes resizing a parent event that is not manually scheduled useless, it would
 * only snap back to the dates defined by its children. To avoid confusion, resizing is therefor turned off for parent
 * events unless they have `manuallyScheduled: true`</div>
 *
 * ## Drag and drop for parent events
 *
 * Normally the dates of a parent event is defined by its children (as described above), with exception for when drag
 * dropping a parent event along the time axis. In this case the operation will update the dates of all the children,
 * which will thus also move the parent event in time.
 *
 * If a parent event is dragged to a new resource, all its children will also be assigned to that resource.
 *
 * ## Caveats
 *
 * Usage of the feature comes with some requirements/caveats:
 * * As already mentioned, it requires a tree event store
 * * Requires using an AssignmentStore, the legacy single assignment mode does not handle tree stores
 * * Scheduler must use stack or overlap {@link SchedulerPro/view/SchedulerPro#config-eventLayout}, pack not supported
 * * {@link Scheduler/feature/Dependencies} are not supported for nested events
 * * {@link Scheduler/feature/EventDragSelect} is not supported
 * * Multi event drag is not supported for nested events
 * * Cannot {@link Scheduler/feature/EventDragCreate} within parent events
 * * {@link Scheduler/feature/Labels} are not supported for nested events
 * * {@link SchedulerPro/feature/EventBuffer} won't work with nested events
 * * {@link SchedulerPro/feature/TaskEdit} does not allow assigning resources or dependencies to nested events
 *
 * This feature is **off** by default. For info on enabling it, see {@link Grid.view.mixin.GridFeatures}.
 *
 * @classtype nestedEvents
 * @feature
 */
class NestedEvents extends InstancePlugin.mixin(AttachToProjectMixin, Delayable) {
  static $name = 'NestedEvents';
  //region Config
  static configurable = {
    /**
     * This config defines how to handle overlapping nested events. Valid values are:
     * - `stack`, events use fixed height and stack on top of each other (not supported in vertical mode)
     * - `pack`, adjusts event height
     * - `none`, allows events to overlap
     *
     * <div class="note">Note that stacking works differently for nested events as compared to normal events (and
     * not at all in vertical mode). The height of the parent event will never change, all nested events use
     * {@link #config-eventHeight fixed height} and will stack until all available space is consumed, after which
     * they will overflow the parent.</div>
     *
     * <div class="note">Also note that stacked nested events are clipped by the parent, making it scrollable on
     * vertical overflow. This cannot be combined with sticky events. If stacking events in your app won't overflow
     * the parent, you can specify `overflow: visible` on `.b-nested-events-container.b-nested-events-layout-stack`
     * to not clip and make sticky events work.</div>
     *
     * @prp {'stack'|'pack'|'none'}
     * @default
     */
    eventLayout: 'pack',
    /**
     * Vertical (horizontal in vertical mode) space between nested event bars, in px
     * @prp {Number}
     * @default
     */
    barMargin: 5,
    /**
     * Margin above first nested event bar and below last (or before / after in vertical mode), in px
     * @prp {Number}
     * @default
     */
    resourceMargin: 0,
    /**
     * Fixed event height (width in vertical mode) to use when configured with `eventLayout : 'stack'`.
     * @prp {Number}
     * @default
     */
    eventHeight: 30,
    /**
     * Space (in px) in a parent element reserved for displaying a title etc. Used to compute available space for
     * the nested events container inside the parent.
     *
     * Setting this config updates the ` --schedulerpro-nested-event-header-height` CSS variable.
     *
     * @prp {Number}
     * @default
     */
    headerHeight: 20,
    /**
     * Constrains dragging of nested events within their parent when configured as `true`, allows them to be
     * dragged out of it when configured as `false` (the default).
     * @prp {Boolean}
     * @default
     */
    constrainDragToParent: false,
    /**
     * Allow an event to be dropped on another to nest it.
     *
     * Dropping an event on another will add the dropped event as a child of the target, turning the target into a
     * parent if it was not already.
     *
     * Parent events dropped on another event are ignored.
     *
     * @prp {Boolean}
     * @default
     */
    allowNestingOnDrop: true,
    /**
     * Allow dropping a nested event directly on a resource to de-nest it, turning it into an ordinary event.
     *
     * Requires {@link #config-constrainDragToParent} to be configured with `false` to be applicable.
     *
     * @prp {Boolean}
     * @default
     */
    allowDeNestingOnDrop: true,
    /**
     * Constrains resizing of nested events to their parents start and end dates when configured as `true` (the
     * default), preventing them from changing their parents dates.
     *
     * Configure as `false` if you want to allow resizing operations to extend the parents dates (only applies for
     * parents not configured with `manuallyScheduled: true`).
     *
     * <div class="note">Note that when using `eventLayout: stack` the nested events are clipped by the parent, the
     * part extending outside if not constrained to parent will not be shown until it re-renders after resize. If
     * stacking events in your app won't overflow the parent, you can specify `overflow: visible` on
     * `.b-nested-events-container.b-nested-events-layout-stack` to not clip.</div>
     *
     * @prp {Boolean}
     * @default
     */
    constrainResizeToParent: true
  };
  static pluginConfig = {
    before: ['onEventStoreBatchedUpdate'],
    chain: ['getEventsToRender', 'onEventDataGenerated', 'processEventDrop', 'processCrossSchedulerEventDrop', 'beforeEventDragStart', 'afterEventDragStart', 'afterEventDragAbortFinalized', 'checkEventDragValidity', 'afterEventResizeStart'],
    override: ['getResourceMargin', 'getBarMargin', 'getAppliedResourceHeight', 'getResourceWidth', 'getEventLayout', 'getElementFromAssignmentRecord', 'scheduleEvent']
  };
  static delayable = {
    refreshClient: 'raf'
  };
  //endregion
  construct(client, config) {
    super.construct(client, config);
    // EventStore has to be a tree store for the feature to work.
    // If it starts empty, it might not be flagged as such. Help it out.
    this.client.eventStore.tree = true;
  }
  refreshClient() {
    !this.client.isConfiguring && this.client.refreshWithTransition();
  }
  doDisable() {
    this.refreshClient();
  }
  //region Props
  updateEventLayout(layout) {
    if (layout === 'stack' && this.client.isVertical) {
      console.warn('Stacked nested events are not supported in vertical mode');
    }
    this.refreshClient();
  }
  updateBarMargin() {
    this.refreshClient();
  }
  updateResourceMargin() {
    this.refreshClient();
  }
  updateEventHeight() {
    this.refreshClient();
  }
  updateHeaderHeight(height) {
    this.client.element.style.setProperty('--schedulerpro-nested-event-header-height', `${height}px`);
    this.refreshClient();
  }
  // Nested events has their own layout setting
  getEventLayout(resourceRecord, parentEventRecord) {
    if (parentEventRecord) {
      return {
        type: this.eventLayout
      };
    }
    return this.overridden.getEventLayout(resourceRecord);
  }
  // Specific resource margin for nested events
  getResourceMargin(resourceRecord, parentEventRecord) {
    if (parentEventRecord && !parentEventRecord.isRoot) {
      return this.resourceMargin;
    }
    return this.overridden.getResourceMargin(resourceRecord);
  }
  // Specific bar margin for nested events
  getBarMargin(resourceRecord, parentEventRecord) {
    if (parentEventRecord && !parentEventRecord.isRoot) {
      return this.barMargin;
    }
    return this.overridden.getBarMargin(resourceRecord);
  }
  // Use height available inside the parent event
  getAppliedResourceHeight(resourceRecord, parentEventRecord) {
    const me = this;
    if (parentEventRecord && !parentEventRecord.isRoot) {
      if (me.eventLayout === 'stack') {
        // Layout subtracts resourceMargin * 2, added here to get eventHeight correct after
        return me.eventHeight + me.resourceMargin * 2;
      } else {
        const borderWidth = borderWidths[me.client.getEventStyle(parentEventRecord, resourceRecord)] ?? 0;
        return me.currentParentsHeight - me.headerHeight - borderWidth;
      }
    }
    return me.overridden.getAppliedResourceHeight(resourceRecord);
  }
  getResourceWidth(resourceRecord, parentEventRecord) {
    if (parentEventRecord && !parentEventRecord.isRoot) {
      return this.currentParentsWidth - this.headerHeight;
    }
    return this.overridden.getResourceWidth(resourceRecord);
  }
  //endregion
  //region CRUD listeners
  attachToEventStore(eventStore) {
    eventStore === null || eventStore === void 0 ? void 0 : eventStore.ion({
      name: 'eventStore',
      change: 'onEventStoreChange',
      thisObj: this
    });
  }
  onEventStoreChange({
    records
  }) {
    // Refresh if a nested event was changed
    if (records !== null && records !== void 0 && records.some(r => r.parent && !r.parent.isRoot)) {
      this.refreshClient();
    }
  }
  onEventStoreBatchedUpdate({
    records
  }) {
    // Refresh if a nested event was changed, and we are listening for batched changes (resizing)
    if (this.client.listenToBatchedUpdates && records !== null && records !== void 0 && records.some(r => r.parent && !r.parent.isRoot)) {
      this.refreshClient();
      // Prevent default handler
      return false;
    }
  }
  //endregion
  //region Drag
  // Move event element to foreground canvas during drag. Has to happen before drag starts for the feature to pick up
  // correct coordinates to resolve resource by, transition back to on abort etc.
  beforeEventDragStart(context, dragData) {
    const me = this,
      {
        client
      } = me,
      {
        eventRecord,
        assignmentRecords
      } = dragData,
      {
        parentElement
      } = context.element;
    // Dragging nested events?
    if (eventRecord.parent && parentElement !== client.foregroundCanvas) {
      me.isDraggingNestedEvent = true;
      // Remember origin to be able to restore on abort (success redraws so that will be covered anyway)
      context.originalParentElement = parentElement;
      context.originalBounds = [];
      for (const assignment of assignmentRecords) {
        const {
          event
        } = assignment;
        // UI should not allow selecting nested events from different parents, but it is programmatically
        // possible. We only include from the dragged events parent here, behaviour for mixed parents are for
        // now undefined
        if (event.parent === eventRecord.parent) {
          const eventElement = client.getElementFromAssignmentRecord(assignment, true);
          context.originalBounds.push({
            element: eventElement,
            bounds: Rectangle.from(eventElement, parentElement)
          });
          if (!me.constrainDragToParent && client.features.eventDrag.constrainDragToTimeline) {
            // Pull nested events out
            const relativeBounds = Rectangle.from(eventElement, client.timeAxisSubGridElement);
            eventElement.style.top = `${relativeBounds.top}px`;
            eventElement.style.left = `${relativeBounds.left}px`;
            DomSync.addChild(client.foregroundCanvas, eventElement, assignment.id);
          }
        }
      }
    } else {
      me.isDraggingNestedEvent = false;
    }
  }
  // Setup constraints when drag starts if needed
  afterEventDragStart(context, dragData) {
    // Constrain to current parent?
    if (this.isDraggingNestedEvent && this.constrainDragToParent) {
      const {
          eventDrag
        } = this.client.features,
        {
          parent
        } = dragData.eventRecord,
        parentBounds = context.originalParentElement.getBoundingClientRect();
      // Constrain top / bottom
      eventDrag.setYConstraint(0, parentBounds.height - context.originalBounds[0].bounds.height);
      // For left / right we also have to constrain the dates, otherwise only the element will be constrained
      eventDrag.setXConstraint(0, parentBounds.width - context.originalBounds[0].bounds.width);
      dragData.dateConstraints = {
        start: parent.startDate,
        end: parent.endDate
      };
    }
  }
  checkEventDragValidity({
    targetEventRecord,
    eventRecord,
    timeDiff,
    newResource,
    resourceRecord
  }) {
    const me = this;
    // Disallow dropping on a blank space in a resource if configured to not allow de-nesting
    // (ignore first round, targetEventRecord cannot be resolved until on next, which we determine here by checking
    // timeDiff or resource change)
    if (me.isDraggingNestedEvent && !me.allowDeNestingOnDrop && !targetEventRecord && (timeDiff || newResource !== resourceRecord)) {
      return {
        valid: false,
        message: me.L('L{deNestingNotAllowed}')
      };
    }
    // Disallow dropping on a new parent if configured to not allow nesting
    if (!me.allowNestingOnDrop && targetEventRecord && targetEventRecord !== eventRecord.parent) {
      return {
        valid: false,
        message: me.L('L{nestingNotAllowed}')
      };
    }
  }
  // Move event to new parent if dropped on a parent or moved out of one
  processEventDrop({
    context,
    toScheduler,
    eventRecord,
    resourceRecord,
    reassignedFrom,
    element,
    eventsToAdd,
    addedEvents,
    draggedAssignment
  }) {
    var _eventRecord$children;
    const {
        parent
      } = eventRecord,
      {
        targetEventRecord
      } = context;
    let newParent = parent;
    // targetEventRecord is resolved using mouse coords, it might be outside of parent when constrained thus
    // we have to check if constrained here to not move it out by mistake
    if (parent !== targetEventRecord && !this.constrainDragToParent && !((_eventRecord$children = eventRecord.children) !== null && _eventRecord$children !== void 0 && _eventRecord$children.length)) {
      // Dropped on a new parent and allowed to nest
      if (targetEventRecord && this.allowNestingOnDrop) {
        newParent = targetEventRecord.parent.isRoot ? targetEventRecord : targetEventRecord.parent;
        // We resolve resource and targetEventRecord differently (mouse vs element), might get next resource so
        // we re-resolve here to be sure it is correct
        const targetResource = this.client.resolveResourceRecord(context.browserEvent);
        if (targetResource !== resourceRecord) {
          resourceRecord = draggedAssignment.resource = targetResource;
        }
      }
      // Dropped directly on resource and allowed to de-nest (cant get here if not allowed, blocked in validation)
      else {
        newParent = toScheduler.eventStore.rootNode;
      }
      if (newParent && newParent !== parent) {
        addedEvents.push(newParent.appendChild(eventRecord));
        // Don't want to add it to root when dragging to another scheduler
        ArrayHelper.remove(eventsToAdd, eventRecord);
      }
    }
    // Moved parent to new resource, reassign all children assigned to its previous resource
    if (parent !== null && parent !== void 0 && parent.isRoot && eventRecord.isParent && reassignedFrom && reassignedFrom !== resourceRecord) {
      for (const child of eventRecord.children) {
        const existingAssignment = child.assignments.find(a => a.resource === reassignedFrom);
        if (existingAssignment) {
          existingAssignment.resource = resourceRecord;
        }
      }
    }
    // Add to new parent (or put back in old) matching outer position. If we don't do this element might get released
    // on DomSync of foregroundCanvas (also this lets it transition within the parent)
    if (newParent && !newParent.isRoot) {
      const newParentElement = this.client.getElementFromEventRecord(newParent, resourceRecord).syncIdMap.nestedEventsContainer,
        intersection = newParentElement && Rectangle.from(element, newParentElement);
      // If dropped on a root level leaf it has no nested events container yet
      if (newParentElement) {
        element.style.top = `${intersection.top}px`;
        element.style.left = `${intersection.left}px`;
        // If dropped at the same position in a new parent it won't transition into place if it thinks nothing
        // changed
        element.lastDomConfig = null;
        DomSync.addChild(newParentElement, element, element.dataset.syncId);
      }
    }
  }
  // Assign all children to same resource when dropping on another scheduler
  processCrossSchedulerEventDrop({
    eventRecord
  }) {
    if (eventRecord.isParent) {
      for (const child of eventRecord.children) {
        child.resource = eventRecord.resource;
      }
    }
  }
  // Restore element after abort (back to original parent and position)
  async afterEventDragAbortFinalized({
    originalParentElement,
    originalBounds
  }) {
    if (this.isDraggingNestedEvent) {
      // Wait for any position transition
      for (const animation of originalBounds[0].element.getAnimations()) {
        if (animation.transitionProperty === 'top' || animation.transitionProperty === 'left') {
          await animation.finished;
        }
      }
      for (const {
        element,
        bounds
      } of originalBounds) {
        // Move it back
        element.style.top = `${bounds.top}px`;
        element.style.left = `${bounds.left}px`;
        originalParentElement.appendChild(element);
      }
    }
  }
  // Limit resizing to parent bounds if configured to do so (it is the default)
  afterEventResizeStart(context) {
    if (this.constrainResizeToParent) {
      const {
        parent
      } = context.timespanRecord;
      if (parent && !parent.isRoot) {
        let {
          startDate,
          endDate
        } = parent;
        if (context.dateConstraints) {
          startDate = DateHelper.max(startDate, context.dateConstraints.start);
          endDate = DateHelper.min(endDate, context.dateConstraints.end);
        }
        context.dateConstraints = {
          start: startDate,
          end: endDate
        };
      }
    }
  }
  //endregion
  //region Overrides to make scheduler work with nested events
  // Let Scheduler resolve nested events too
  getElementFromAssignmentRecord(assignmentRecord, returnWrapper) {
    var _assignmentRecord$eve;
    if (assignmentRecord !== null && assignmentRecord !== void 0 && (_assignmentRecord$eve = assignmentRecord.event) !== null && _assignmentRecord$eve !== void 0 && _assignmentRecord$eve.parent && !assignmentRecord.event.parent.isRoot) {
      const parentElement = this.client.getElementFromEventRecord(assignmentRecord.event.parent, assignmentRecord.resource);
      return DomSync.getChild(parentElement, `nestedEventsContainer.${assignmentRecord.id}${returnWrapper ? '' : '.event'}`);
    }
    return this.overridden.getElementFromAssignmentRecord(assignmentRecord, returnWrapper);
  }
  // Allow scheduling nested events by overriding Schedulers implementation
  async scheduleEvent({
    eventRecord,
    parentEventRecord,
    startDate,
    element
  }) {
    // When passed a parent, append to it and assign to its resource
    if (parentEventRecord) {
      eventRecord.startDate = startDate;
      eventRecord = parentEventRecord.appendChild(eventRecord);
      eventRecord.assign(parentEventRecord.resource);
      // When given an element, it is positioned inside the parent and adopted by DomSync, letting it transition
      if (element) {
        const parentElement = this.client.getElementFromEventRecord(parentEventRecord).syncIdMap.nestedEventsContainer,
          eventRect = Rectangle.from(element, parentElement);
        // Clear translate styles used by DragHelper
        DomHelper.setTranslateXY(element, 0, 0);
        DomHelper.setTopLeft(element, eventRect.y, eventRect.x);
        DomSync.addChild(parentElement, element, eventRecord.assignments[0].id);
      }
      await this.client.project.commitAsync();
    } else {
      return this.overridden.scheduleEvent(...arguments);
    }
  }
  //endregion
  //region Rendering
  // Hook into event collection to filter out children, since they will be rendered inside their parents
  getEventsToRender(resourceRecord, eventRecords) {
    if (!this.disabled) {
      // Only keep direct children of the root (?. in case someone tries to use a flat store)
      ArrayHelper.remove(eventRecords, ...eventRecords.filter(eventRecord => eventRecord.isEventModel && !eventRecord.parent.isRoot));
    }
    return eventRecords;
  }
  // Hook into event render data generation to add nested events (by using mostly the same rendering path as normal
  // events)
  onEventDataGenerated({
    eventRecord,
    resourceRecord,
    wrapperCls,
    height,
    width,
    children,
    left,
    top
  }) {
    if (eventRecord.isParent) {
      wrapperCls['b-nested-events-parent'] = 1;
      const me = this;
      me.currentParentsHeight = height;
      me.currentParentsWidth = width;
      const {
          currentOrientation,
          isVertical
        } = me.client,
        assignedChildren = eventRecord.children.filter(e => {
          var _e$$linkedResources;
          return (_e$$linkedResources = e.$linkedResources) === null || _e$$linkedResources === void 0 ? void 0 : _e$$linkedResources.includes(resourceRecord);
        }),
        // This call uses the same render path as normal events, applying event layout etc. The layout is then
        // as needed patched up below (to be relative to parent etc)
        layouts = currentOrientation.layoutEvents(resourceRecord, assignedChildren, false, eventRecord, me.overlappingEventSorter),
        nestedEvents = [];
      let eventsData;
      if (isVertical) {
        eventsData = [];
        for (const layout of Object.values(layouts)) {
          eventsData.push(layout.renderData);
        }
      } else {
        eventsData = layouts === null || layouts === void 0 ? void 0 : layouts.eventsData;
      }
      if (eventsData) {
        for (const layout of eventsData) {
          // Positioned inside parent
          if (isVertical) {
            layout.left -= left;
            layout.top -= top;
            layout.absoluteTop = layout.top;
          } else {
            // Special handling for overlap, it does not use the same render path as other layouts
            if (me.eventLayout === 'none') {
              layout.top = 0;
              layout.height = me.getAppliedResourceHeight(resourceRecord, eventRecord);
            }
            // Stack also needs some special handling of height, since it uses fixed event height
            else if (me.eventLayout === 'stack') {
              layout.height = me.eventHeight;
            }
            layout.left -= left;
            layout.absoluteTop = layout.top;
          }
          const domConfig = currentOrientation.renderEvent(isVertical ? {
            renderData: layout
          } : layout, height);
          domConfig.className['b-nested-event'] = 1;
          nestedEvents.push(domConfig);
        }
      }
      // Nested event DomConfig
      children.push({
        className: {
          'b-nested-events-container': 1,
          [`b-nested-events-layout-${me.eventLayout}`]: 1
        },
        dataset: {
          taskBarFeature: 'nestedEventsContainer'
        },
        children: nestedEvents,
        syncOptions: {
          syncIdField: 'syncId',
          releaseThreshold: 0
        }
      });
    }
  }
  //endregion
}

NestedEvents._$name = 'NestedEvents';
GridFeatureManager.registerFeature(NestedEvents, false, 'SchedulerPro');

/**
 * @module SchedulerPro/feature/ResourceNonWorkingTime
 */
/**
 * Feature that highlights the non-working intervals for resources based on their {@link SchedulerPro.model.ResourceModel#field-calendar}.
 * If a resource has no calendar defined, the project's calendar will be used. The non-working time interval can
 * also be recurring. You can find a live example showing how to achieve this in the [Resource Non-Working Time Demo](../examples/resource-non-working-time/).
 *
 * {@inlineexample SchedulerPro/feature/ResourceNonWorkingTime.js}
 *
 * ## Data structure
 * Example data defining calendars and assigning the resources a calendar:
 * ```javascript
 * {
 *   "success"   : true,
 *   "calendars" : {
 *       "rows" : [
 *           {
 *               "id"                       : "day",
 *               "name"                     : "Day shift",
 *               "unspecifiedTimeIsWorking" : false,
 *               "cls"                      : "dayshift",
 *               "intervals"                : [
 *                   {
 *                       "recurrentStartDate" : "at 8:00",
 *                       "recurrentEndDate"   : "at 17:00",
 *                       "isWorking"          : true,
 *                   }
 *               ]
 *           }
 *    ],
 *    "resources" : {
 *       "rows" : [
 *           {
 *               "id"         : 1,
 *               "name"       : "George",
 *               "calendar"   : "day",
 *               "role"       : "Office",
 *               "eventColor" : "blue"
 *           },
 *           {
 *               "id"         : 2,
 *               "name"       : "Rob",
 *               "calendar"   : "day",
 *               "role"       : "Office",
 *               "eventColor" : "blue"
 *           }
 *        ]
 *   [...]
 * ```
 *
 * ```javascript
 * const scheduler = new SchedulerPro({
 *   // A Project holding the data and the calculation engine for Scheduler Pro. It also acts as a CrudManager, allowing
 *   // loading data into all stores at once
 *   project : {
 *       autoLoad  : true,
 *       transport : {
 *           load : {
 *               url : './data/data.json'
 *           }
 *       }
 *   },
 *   features : {
 *       resourceNonWorkingTime : true
 *   },
 *   [...]
 * }):
 * ```
 *
 * ## Styling non-working time interval elements
 *
 * To style the elements representing the non-working time elements you can set the {@link SchedulerPro.model.CalendarModel#field-cls}
 * field in your data. This will add a CSS class to all non-working time elements for the calendar. You can also add
 * an {@link SchedulerPro.model.CalendarModel#field-iconCls} value specifying an icon to display inside the interval.
 *
 * ```javascript
 * {
 *   "success"   : true,
 *   "calendars" : {
 *       "rows" : [
 *           {
 *               "id"                       : "day",
 *               "name"                     : "Day shift",
 *               "unspecifiedTimeIsWorking" : false,
 *               "cls"                      : "dayshift",
 *               "intervals"                : [
 *                   {
 *                       "recurrentStartDate" : "at 8:00",
 *                       "recurrentEndDate"   : "at 17:00",
 *                       "isWorking"          : true
 *                   }
 *               ]
 *           }
 *       ]
 *    }
 * }
 * ```
 *
 * You can also add a `cls` value and an `iconCls` to **individual** intervals:
 *
 * ```javascript
 * {
 *   "success"   : true,
 *   "calendars" : {
 *       "rows" : [
 *           {
 *               "id"                       : "day",
 *               "name"                     : "Day shift",
 *               "unspecifiedTimeIsWorking" : true,
 *               "intervals"                : [
 *                   {
 *                      "startDate"          : "2022-03-23T02:00",
 *                      "endDate"            : "2022-03-23T04:00",
 *                      "isWorking"          : false,
 *                      "cls"                : "factoryShutdown",
 *                      "iconCls"            : "warningIcon"
 *                  }
 *               ]
 *           }
 *       ]
 *    }
 * }
 * ```
 *
 * This feature is **off** by default. For info on enabling it, see {@link Grid.view.mixin.GridFeatures}.
 *
 * @extends Scheduler/feature/base/ResourceTimeRangesBase
 * @demo SchedulerPro/resource-non-working-time
 * @classtype resourceNonWorkingTime
 * @feature
 */
class ResourceNonWorkingTime extends ResourceTimeRangesBase {
  //region Config
  static $name = 'ResourceNonWorkingTime';
  static configurable = {
    rangeCls: 'b-sch-resourcenonworkingtime',
    /**
     * The largest time axis unit to display non working ranges for ('hour' or 'day' etc).
     * When zooming to a view with a larger unit, no non-working time elements will be rendered.
     *
     * **Note:** Be careful with setting this config to big units like 'year'. When doing this,
     * make sure the timeline {@link Scheduler.view.TimelineBase#config-startDate start} and
     * {@link Scheduler.view.TimelineBase#config-endDate end} dates are set tightly.
     * When using a long range (for example many years) with non-working time elements rendered per hour,
     * you will end up with millions of elements, impacting performance.
     * When zooming, use the {@link Scheduler.view.mixin.TimelineZoomable#config-zoomKeepsOriginalTimespan} config.
     * @config {'millisecond'|'second'|'minute'|'hour'|'day'|'week'|'month'|'quarter'|'year'}
     * @default
     */
    maxTimeAxisUnit: 'hour',
    /**
     * Set to `true` to allow mouse interactions with the rendered range elements. By default, the range elements
     * are not reachable with the mouse, and only serve as a static background.
     * @prp {Boolean}
     * @default
     */
    enableMouseEvents: false,
    /**
     * The Model class to use for representing a {@link Scheduler.model.ResourceTimeRangeModel}
     * @config {Function}
     */
    resourceTimeRangeModelClass: ResourceTimeRangeModel,
    entityName: 'resourceNonWorkingTime'
  };
  // Cannot use `static properties = {}`, new Map would pollute the prototype
  static get properties() {
    return {
      resourceMap: new Map()
    };
  }
  //endregion
  //region Constructor
  construct() {
    super.construct(...arguments);
    this.resourceTimeRangeModelClass = class ResourceNonWorkingTimeModel extends this.resourceTimeRangeModelClass {
      static $name = 'ResourceNonWorkingTimeModel';
      static domIdPrefix = 'resourcenonworkingtimemodel';
    };
    this.client.timeAxis.ion({
      name: 'timeAxis',
      reconfigure: 'onTimeAxisReconfigure',
      // should trigger before event rendering chain
      prio: 100,
      thisObj: this
    });
  }
  //endregion
  //region Events
  /**
   * Triggered for mouse down ona resource nonworking time range. Only triggered if the ResourceNonWorkingTime feature is configured
   * with `enableMouseEvents: true`.
   * @event resourceNonWorkingTimeMouseDown
   * @param {SchedulerPro.view.SchedulerPro} source This Scheduler
   * @param {SchedulerPro.feature.ResourceNonWorkingTime} feature The ResourceNonWorkingTime feature
   * @param {Scheduler.model.ResourceTimeRangeModel} resourceTimeRangeRecord Resource time range record
   * @param {Scheduler.model.ResourceModel} resourceRecord Resource record
   * @param {MouseEvent} domEvent Browser event
   * @on-owner
   */
  /**
   * Triggered for mouse up ona resource nonworking time range. Only triggered if the ResourceNonWorkingTime feature is configured
   * with `enableMouseEvents: true`.
   * @event resourceNonWorkingTimeMouseUp
   * @param {SchedulerPro.view.SchedulerPro} source This Scheduler
   * @param {SchedulerPro.feature.ResourceNonWorkingTime} feature The ResourceNonWorkingTime feature
   * @param {Scheduler.model.ResourceTimeRangeModel} resourceTimeRangeRecord Resource time range record
   * @param {Scheduler.model.ResourceModel} resourceRecord Resource record
   * @param {MouseEvent} domEvent Browser event
   * @on-owner
   */
  /**
   * Triggered for click on a resource nonworking time range. Only triggered if the ResourceNonWorkingTime feature is configured with
   * `enableMouseEvents: true`.
   * @event resourceNonWorkingTimeClick
   * @param {SchedulerPro.view.SchedulerPro} source This Scheduler
   * @param {SchedulerPro.feature.ResourceNonWorkingTime} feature The ResourceNonWorkingTime feature
   * @param {Scheduler.model.ResourceTimeRangeModel} resourceTimeRangeRecord Resource time range record
   * @param {Scheduler.model.ResourceModel} resourceRecord Resource record
   * @param {MouseEvent} domEvent Browser event
   * @on-owner
   */
  /**
   * Triggered for double-click on a resource nonworking time range. Only triggered if the ResourceNonWorkingTime feature is configured
   * with `enableMouseEvents: true`.
   * @event resourceNonWorkingTimeDblClick
   * @param {SchedulerPro.view.SchedulerPro} source This Scheduler
   * @param {SchedulerPro.feature.ResourceNonWorkingTime} feature The ResourceNonWorkingTime feature
   * @param {Scheduler.model.ResourceTimeRangeModel} resourceTimeRangeRecord Resource time range record
   * @param {Scheduler.model.ResourceModel} resourceRecord Resource record
   * @param {MouseEvent} domEvent Browser event
   * @on-owner
   */
  /**
   * Triggered for right-click on a resource nonworking time range. Only triggered if the ResourceNonWorkingTime feature is configured
   * with `enableMouseEvents: true`.
   * @event resourceNonWorkingTimeContextMenu
   * @param {SchedulerPro.view.SchedulerPro} source This Scheduler
   * @param {SchedulerPro.feature.ResourceNonWorkingTime} feature The ResourceNonWorkingTime feature
   * @param {Scheduler.model.ResourceTimeRangeModel} resourceTimeRangeRecord Resource time range record
   * @param {Scheduler.model.ResourceModel} resourceRecord Resource record
   * @param {MouseEvent} domEvent Browser event
   * @on-owner
   */
  /**
   * Triggered for mouse over on a resource nonworking time range. Only triggered if the ResourceNonWorkingTime feature is configured
   * with `enableMouseEvents: true`.
   * @event resourceNonWorkingTimeMouseOver
   * @param {SchedulerPro.view.SchedulerPro} source This Scheduler
   * @param {SchedulerPro.feature.ResourceNonWorkingTime} feature The ResourceNonWorkingTime feature
   * @param {Scheduler.model.ResourceTimeRangeModel} resourceTimeRangeRecord Resource time range record
   * @param {Scheduler.model.ResourceModel} resourceRecord Resource record
   * @param {MouseEvent} domEvent Browser event
   * @on-owner
   */
  /**
   * Triggered for mouse out of a resource nonworking time range. Only triggered if the ResourceNonWorkingTime feature is configured
   * with `enableMouseEvents: true`.
   * @event resourceNonWorkingTimeMouseOut
   * @param {SchedulerPro.view.SchedulerPro} source This Scheduler
   * @param {SchedulerPro.feature.ResourceNonWorkingTime} feature The ResourceNonWorkingTime feature
   * @param {Scheduler.model.ResourceTimeRangeModel} resourceTimeRangeRecord Resource time range record
   * @param {Scheduler.model.ResourceModel} resourceRecord Resource record
   * @param {MouseEvent} domEvent Browser event
   * @on-owner
   */
  //endregion
  //region Init
  attachToResourceStore(resourceStore) {
    super.attachToResourceStore(resourceStore);
    resourceStore === null || resourceStore === void 0 ? void 0 : resourceStore.ion({
      name: 'resourceStore',
      changePreCommit: 'onResourceChange',
      thisObj: this
    });
  }
  attachToCalendarManagerStore(calendarManagerStore) {
    super.attachToCalendarManagerStore(calendarManagerStore);
    calendarManagerStore === null || calendarManagerStore === void 0 ? void 0 : calendarManagerStore.ion({
      name: 'calendarManagerStore',
      changePreCommit: 'onCalendarChange',
      thisObj: this
    });
  }
  //endregion
  //region Events
  onTimeAxisReconfigure() {
    // reset ranges cache on timeAxis change
    this.resourceMap.clear();
  }
  onResourceChange({
    action,
    records,
    record,
    changes
  }) {
    const me = this;
    // Might need to redraw on update
    if (action === 'update') {
      var _change$value;
      const change = changes.calendar;
      // Ignore calendar normalization
      if (change && (typeof change.oldValue !== 'string' || ((_change$value = change.value) === null || _change$value === void 0 ? void 0 : _change$value.id) !== change.oldValue)) {
        me.resourceMap.delete(record.id);
        // Redraw row in case calendar change did not affect any events
        me.client.runWithTransition(() => {
          me.client.currentOrientation.refreshEventsForResource(record);
        });
      }
    }
    // Keep map up to date on removals (adds are handled through rendering in getEventsToRender)
    if (action === 'remove') {
      records.forEach(record => me.resourceMap.delete(record.id));
    }
    if (action === 'removeall') {
      me.resourceMap.clear();
    }
  }
  onCalendarChange({
    action,
    records,
    record,
    changes
  }) {
    this.resourceMap.clear();
    this.client.refresh();
  }
  //endregion
  //region Internal
  // Called on render of resources events to get events to render. Add any ranges
  // (chained function from Scheduler)
  getEventsToRender(resource, events) {
    const me = this,
      {
        resourceMap,
        client
      } = me,
      {
        timeAxis
      } = client,
      shouldPaint = !me.maxTimeAxisUnit || DateHelper.compareUnits(timeAxis.unit, me.maxTimeAxisUnit) <= 0;
    if (!me.disabled && shouldPaint && resource.effectiveCalendar) {
      if (!resourceMap.has(resource.id)) {
        const ranges = resource.effectiveCalendar.getNonWorkingTimeRanges(client.startDate, client.endDate),
          records = ranges.map((range, i) => new me.resourceTimeRangeModelClass({
            id: `r${resource.id}i${i}`,
            iconCls: range.iconCls || resource.effectiveCalendar.iconCls || '',
            cls: `${resource.effectiveCalendar.cls || ''} ${range.cls || ''}`,
            startDate: range.startDate,
            endDate: range.endDate,
            name: range.name || '',
            resourceId: resource.id,
            isNonWorking: true
          }));
        resourceMap.set(resource.id, records);
      }
      events.push(...resourceMap.get(resource.id));
    }
    return events;
  }
  shouldInclude({
    isNonWorking
  }) {
    return isNonWorking;
  }
  /**
   * Returns a resource nonworking time range record from the passed element
   * @param {HTMLElement} rangeElement
   * @returns {Scheduler.model.ResourceTimeRangeModel}
   * @category DOM
   */
  resolveResourceNonWorkingTimeInterval(rangeElement) {
    var _rangeElement$closest;
    return rangeElement === null || rangeElement === void 0 ? void 0 : (_rangeElement$closest = rangeElement.closest('.b-sch-resourcenonworkingtime')) === null || _rangeElement$closest === void 0 ? void 0 : _rangeElement$closest.elementData.eventRecord;
  }
  //endregion
}
// No feature based styling needed, do not add a cls to Scheduler
ResourceNonWorkingTime.featureClass = '';
ResourceNonWorkingTime._$name = 'ResourceNonWorkingTime';
GridFeatureManager.registerFeature(ResourceNonWorkingTime, false, 'SchedulerPro');

/**
 * @module SchedulerPro/feature/TimeSpanHighlight
 */
const timespanDefaults = {
  isHighlightConfig: true,
  clearExisting: false
};
/**
 * An object describing the time span region to highlight.
 *
 * @typedef {Object} HighlightTimeSpan
 * @property {Date} startDate A start date constraining the region
 * @property {Date} endDate An end date constraining the region
 * @property {String} name A name to show in the highlight element
 * @property {Scheduler.model.ResourceModel} [resourceRecord] The resource record (applicable for Scheduler only)
 * @property {Core.data.Model} [taskRecord] The task record (applicable for Gantt only)
 * @property {String} [cls] A CSS class to add to the highlight element
 * @property {Boolean} [clearExisting=true] `false` to keep existing highlight elements
 * @property {String} [animationId] An id to enable animation of highlight elements
 * @property {Boolean} [surround=false] True to shade the time axis areas before and after the time span
 * (adds a `b-unavailable` CSS class which you can use for styling)
 * @property {Number} [padding] Inflates the non-timeaxis sides of the region by this many pixels
 */
/**
 * This feature exposes methods on the owning timeline widget which you can use to highlight one or multiple time spans
 * in the schedule. Please see {@link #function-highlightTimeSpan} and {@link #function-highlightTimeSpans} to learn
 * more or try the demo below:
 *
 * {@inlineexample SchedulerPro/feature/TimeSpanHighlight.js}
 *
 * ## Example usage with Scheduler Pro
 *
 * ```javascript
 * const scheduler = new SchedulerPro({
 *     features : {
 *         timeSpanHighlight : true
 *     }
 * })
 *
 * scheduler.highlightTimeSpan({
 *      startDate : new Date(2022, 4, 1),
 *      endDate   : new Date(2022, 4, 5),
 *      name      : 'Time off'
 * });
 * ```
 *
 * ## Example usage with Gantt
 *
 * ```javascript
 * const gantt = new Gantt({
 *     features : {
 *         timeSpanHighlight : true
 *     }
 * })
 *
 * gantt.highlightTimeSpan({
 *      startDate : new Date(2022, 4, 1),
 *      endDate   : new Date(2022, 4, 5),
 *      padding   : 10, // Some "air" around the rectangle
 *      taskRecord, // You can also highlight an area specific to a Gantt task
 *      name      : 'Time off'
 * });
 * ```
 *
 * This feature is **disabled** by default.
 *
 * @extends Core/mixin/InstancePlugin
 * @classtype timeSpanHighlight
 * @feature
 * @demo SchedulerPro/highlight-time-spans
 */
class TimeSpanHighlight extends InstancePlugin {
  //region Config
  domConfigs = [];
  configs = [];
  static get $name() {
    return 'TimeSpanHighlight';
  }
  static get configurable() {
    return {
      padding: 0
    };
  }
  static get pluginConfig() {
    return {
      assign: ['highlightTimeSpan', 'highlightTimeSpans', 'unhighlightTimeSpans'],
      chain: ['onTimeAxisViewModelUpdate']
    };
  }
  //endregion
  construct() {
    super.construct(...arguments);
    this.client.rowManager.ion({
      renderDone: this.onViewChanged,
      thisObj: this
    });
  }
  /**
   * Highlights the region representing the passed time span and optionally for a single certain resource.
   * @on-owner
   * @param {HighlightTimeSpan} options A single options object describing the time span to highlight.
   */
  highlightTimeSpan(config, draw = true) {
    const me = this,
      {
        startDate,
        endDate,
        name,
        surround,
        padding = me.padding,
        clearExisting = true
      } = config,
      {
        client
      } = me,
      taskRecord = config.isTimeSpan ? config : config.taskRecord;
    // The resource property allows an actual TaskRecord to be used as a config.
    let resourceRecord = config.resourceRecord || config.resource;
    const {
      animationId
    } = config;
    if (animationId) {
      DomHelper.addTemporaryClass(client.element, 'b-transition-highlight', 500, client);
    }
    if (clearExisting) {
      me.domConfigs.length = me.configs.length = 0;
    }
    if (me.disabled) {
      // nothing to highlight
      return;
    }
    if (surround) {
      me.surroundTimeSpan(config);
      return;
    }
    me.configs.push(config);
    let rect;
    if (client.isGanttBase) {
      rect = client.getScheduleRegion(taskRecord, true, {
        start: startDate,
        end: endDate
      });
    } else {
      if (resourceRecord) {
        // Allows resolving link from original in TreeGrouped scheduler
        resourceRecord = client.store.getById(resourceRecord);
      }
      rect = client.getScheduleRegion(resourceRecord, null, true, {
        start: startDate,
        end: endDate
      }, !resourceRecord);
    }
    if (!rect) {
      // nothing to highlight
      return;
    }
    if (padding) {
      if (client.isHorizontal) {
        rect.inflate(padding, 0, padding, 0);
      } else {
        rect.inflate(0, padding, 0, padding);
      }
    }
    me.domConfigs.push(rect.visualize({
      children: [{
        class: 'b-sch-highlighted-range-name',
        html: name
      }],
      dataset: {
        syncId: animationId
      },
      class: {
        'b-sch-highlighted-range': 1,
        [config.cls]: config.cls,
        [config.class || 'b-sch-highlighted-range-default']: 1
      }
    }, true));
    if (draw) {
      me.draw();
    }
  }
  draw() {
    DomSync.sync({
      targetElement: this.containerEl,
      domConfig: {
        onlyChildren: true,
        children: this.domConfigs
      }
    });
  }
  surroundTimeSpan(timeSpan) {
    this.highlightTimeSpans([Object.assign({}, timeSpan, {
      animationId: (timeSpan.animationId || '') + 'Before',
      class: 'b-unavailable',
      surround: false,
      startDate: this.client.startDate,
      endDate: timeSpan.startDate
    }), Object.assign({}, timeSpan, {
      animationId: (timeSpan.animationId || '') + 'After',
      class: 'b-unavailable',
      surround: false,
      startDate: timeSpan.endDate,
      endDate: this.client.endDate
    })], {
      clearExisting: timeSpan.clearExisting
    });
  }
  /**
   * Highlights the regions representing the passed time spans.
   * @on-owner
   * @param {HighlightTimeSpan[]} timeSpans An array of objects with start/end dates describing the rectangle to highlight.
   * @param {Object} [options] A single options object
   * @param {Boolean} [options.clearExisting=true] Set to `false` to preserve previously highlighted elements
   */
  highlightTimeSpans(timeSpans, options = {}) {
    const me = this,
      {
        clearExisting = true
      } = options;
    if (clearExisting) {
      timeSpans = timeSpans.slice();
      me.domConfigs.length = me.configs.length = 0;
    }
    if (me.disabled) {
      return;
    }
    timeSpans.forEach(timeSpan => {
      // If we are *re*drawing a set of configs, they will have the isHighlightConfig
      // property, so we can pass them straight in. If its a config from the outside,
      // then apply the defaults and the isHighlightConfig flag.
      me.highlightTimeSpan(timeSpan.isHighlightConfig ? timeSpan : Object.setPrototypeOf(timespanDefaults, timeSpan), false);
    });
    me.draw();
  }
  /**
   * Removes any highlighting elements.
   * @param {Boolean} [fadeOut] `true` to fade out the highlight elements before removing
   * @on-owner
   */
  async unhighlightTimeSpans(fadeOut = false) {
    const me = this,
      {
        client
      } = me;
    if (fadeOut) {
      DomHelper.addTemporaryClass(client.element, 'b-transition-highlight', 500, client);
    }
    Array.from(me.containerEl.children).forEach(element => {
      if (fadeOut) {
        element.style.opacity = 0;
        me.fadeOutDetacher = EventHelper.onTransitionEnd({
          element,
          property: 'opacity',
          thisObj: client,
          handler: () => {
            me.domConfigs.length = me.configs.length = 0;
            me.draw();
          }
        });
      } else {
        me.domConfigs.length = me.configs.length = 0;
        me.draw();
      }
    });
  }
  get containerEl() {
    if (!this._containerEl) {
      this._containerEl = DomHelper.createElement({
        parent: this.client.foregroundCanvas,
        retainElement: true,
        class: 'b-sch-highlight-container'
      });
    }
    return this._containerEl;
  }
  onTimeAxisViewModelUpdate() {
    this.onViewChanged();
  }
  onViewChanged() {
    if (this.configs.length > 0) {
      this.highlightTimeSpans(this.configs);
    }
  }
  updateDisabled(disabled, was) {
    if (disabled) {
      this.unhighlightTimeSpans();
    }
    super.updateDisabled(disabled, was);
  }
  // No classname on Scheduler's/Gantt's element
  get featureClass() {}
}
TimeSpanHighlight._$name = 'TimeSpanHighlight';
GridFeatureManager.registerFeature(TimeSpanHighlight, false, ['SchedulerPro', 'Gantt']);

/**
 * @module SchedulerPro/model/changelog/ChangeLogPropertyUpdate
 */
/**
 * An immutable, serializable object that describes an update to a single object property from one value to another.
 */
class ChangeLogPropertyUpdate {
  static $name = 'ChangeLogPropertyUpdate';
  constructor({
    property,
    before,
    after
  }) {
    Object.assign(this, {
      /**
       * @member {String} property A descriptor for the entity (object) affected by this action.
       * @readonly
       * @category Common
       */
      property,
      /**
       * @member {String|Number|Object} before The property's value before the action.
       * @readonly
       * @immutable
       * @category Common
       */
      before,
      /**
       * @member {String|Number|Object} after The property's value after the action.
       * @readonly
       * @immutable
       * @category Common
       */
      after
    });
    Object.freeze(this);
  }
}
ChangeLogPropertyUpdate._$name = 'ChangeLogPropertyUpdate';

/**
 * @module SchedulerPro/model/ProjectModel
 */
/**
 * Scheduler Pro Project model class - a central place for all data.
 *
 * It holds and links the stores usually used by Scheduler Pro:
 *
 * - {@link SchedulerPro/data/EventStore}
 * - {@link SchedulerPro/data/ResourceStore}
 * - {@link SchedulerPro/data/AssignmentStore}
 * - {@link SchedulerPro/data/DependencyStore}
 * - {@link SchedulerPro/data/CalendarManagerStore}
 * - {@link Scheduler/data/ResourceTimeRangeStore}
 * - {@link #config-timeRangeStore TimeRangeStore}
 *
 * The project uses a scheduling engine to calculate dates, durations and such. It is also responsible for
 * handling references between models, for example to link an event via an assignment to a resource. These operations
 * are asynchronous, a fact that is hidden when working in the Scheduler Pro UI but which you must know about when
 * performing operations on the data level.
 *
 * When there is a change to data that requires something else to be recalculated, the project schedules a calculation
 * (a commit) which happens moments later. It is also possible to trigger these calculations directly. This flow
 * illustrates the process:
 *
 * 1. Something changes which requires the project to recalculate, for example adding a new task:
 *
 * ```javascript
 * const [event] = project.eventStore.add({ startDate, endDate });
 * ```
 *
 * 2. A recalculation is scheduled, thus:
 *
 * ```javascript
 * event.duration; // <- Not yet calculated
 * ```
 *
 * 3. Calculate now instead of waiting for the scheduled calculation
 *
 * ```javascript
 * await project.commitAsync();
 *
 * event.duration; // <- Now available
 * ```
 *
 * Please refer to [this guide](#SchedulerPro/guides/basics/project_data.md) for more information.
 *
 * ## Built in CrudManager
 *
 * Scheduler Pro's project has a {@link Scheduler/crud/AbstractCrudManagerMixin CrudManager} built in. Using it is the recommended
 * way of syncing data between Scheduler Pro and a backend. Example usage:
 *
 * ```javascript
 * const scheduler = new SchedulerPro({
 *     project : {
 *         // Configure urls used by the built in CrudManager
 *         transport : {
 *             load : {
 *                 url : 'php/load.php'
 *             },
 *             sync : {
 *                 url : 'php/sync.php'
 *             }
 *         }
 *     }
 * });
 *
 * // Load data from the backend
 * scheduler.project.load()
 * ```
 *
 * For more information on CrudManager, see Schedulers docs on {@link Scheduler/data/CrudManager}.
 * For a detailed description of the protocol used by CrudManager, see the [Crud manager guide](#Scheduler/guides/data/crud_manager.md)
 *
 * You can access the current Project data changes anytime using the {@link #property-changes} property.
 *
 * ## Working with inline data
 *
 * The project provides an {@link #property-inlineData} getter/setter that can
 * be used to manage data from all Project stores at once. Populating the stores this way can
 * be useful if you do not want to use the CrudManager for server communication but instead load data using Axios
 * or similar.
 *
 * ### Getting data
 * ```javascript
 * const data = scheduler.project.inlineData;
 *
 * // use the data in your application
 * ```
 *
 * ### Setting data
 * ```javascript
 * // Get data from server manually
 * const data = await axios.get('/project?id=12345');
 *
 * // Feed it to the project
 * scheduler.project.inlineData = data;
 * ```
 *
 * See also {@link #function-loadInlineData}
 *
 * ### Getting changed records
 *
 * You can access the changes in the current Project dataset anytime using the {@link #property-changes} property. It
 * returns an object with all changes:
 *
 * ```javascript
 * const changes = project.changes;
 *
 * console.log(changes);
 *
 * > {
 *   tasks : {
 *       updated : [{
 *           name : 'My task',
 *           id   : 12
 *       }]
 *   },
 *   assignments : {
 *       added : [{
 *           event      : 12,
 *           resource   : 7,
 *           units      : 100,
 *           $PhantomId : 'abc123'
 *       }]
 *     }
 * };
 * ```
 *
 * ## Monitoring data changes
 *
 * While it is possible to listen for data changes on the projects individual stores, it is sometimes more convenient
 * to have a centralized place to handle all data changes. By listening for the {@link #event-change change event} your
 * code gets notified when data in any of the stores changes. Useful for example to keep an external data model up to
 * date:
 *
 * ```javascript
 * const scheduler = new SchedulerPro({
 *     project: {
 *         listeners : {
 *             change({ store, action, records }) {
 *                 const { $name } = store.constructor;
 *
 *                 if (action === 'add') {
 *                     externalDataModel.add($name, records);
 *                 }
 *
 *                 if (action === 'remove') {
 *                     externalDataModel.remove($name, records);
 *                 }
 *             }
 *         }
 *     }
 * });
 * ```
 *
 * ## Processing the data loaded from the server
 *
 * If you want to process the data received from the server after loading, you can use
 * the {@link #event-beforeLoadApply} or {@link #event-beforeSyncApply} events:
 *
 * ```javascript
 * const gantt = new Gantt({
 *     project: {
 *         listeners : {
 *             beforeLoadApply({ response }) {
 *                 // do something with load-response object before data is fed to the stores
 *             }
 *         }
 *     }
 * });
 * ```
 *
 * ## Built in StateTrackingManager
 *
 * The project also has a built in {@link Core/data/stm/StateTrackingManager} (STM for short), that
 * handles undo/redo for the project stores (additional stores can also be added). By default, it is only used while
 * editing tasks using the task editor, the editor updates tasks live and uses STM to rollback changes if canceled. But
 * you can enable it to track all project store changes:
 *
 * ```javascript
 * // Enable automatic transaction creation and start recording
 * project.stm.autoRecord = true;
 * project.stm.enable();
 *
 * // Undo a transaction
 * project.stm.undo();
 *
 * // Redo
 * project.stm.redo();
 * ```
 *
 * Check out the `undoredo` demo to see it in action.
 *
 * @mixes Core/mixin/Events
 * @mixes SchedulerPro/data/mixin/PartOfProject
 * @mixes SchedulerPro/data/mixin/ProjectCrudManager
 * @mixes SchedulerPro/model/mixin/ProjectChangeHandlerMixin
 *
 * @extends Scheduler/model/mixin/ProjectModelMixin
 *
 * @typings Scheduler/model/ProjectModel -> Scheduler/model/SchedulerProjectModel
 */
class ProjectModel extends ProjectChangeHandlerMixin(ProjectCrudManager(ProjectModelMixin(SchedulerProProjectMixin))) {
  //region Events
  /**
   * Fired when the engine has finished its calculations and the results has been written back to the records.
   *
   * ```javascript
   * scheduler.project.on({
   *     dataReady() {
   *        console.log('Calculations finished');
   *     }
   * });
   *
   * scheduler.eventStore.first.duration = 10;
   *
   * // At some point a bit later it will log 'Calculations finished'
   * ```
   *
   * @event dataReady
   * @param {SchedulerPro.model.ProjectModel} source The project
   */
  /**
   * Fired during the Engine calculation if {@link #config-enableProgressNotifications enableProgressNotifications} config is `true`
   * @event progress
   * @param {Number} total The total number of operations
   * @param {Number} remaining The number of remaining operations
   * @param {'storePopulation'|'propagating'} phase The phase of the calculation, either 'storePopulation'
   * when data is getting loaded, or 'propagating' when data is getting calculated
   */
  /**
   * Fired when the Engine detects a computation cycle.
   * @event cycle
   * @param {Object} schedulingIssue Scheduling error describing the case:
   * @param {Function} schedulingIssue.getDescription Returns the cycle description
   * @param {Object} schedulingIssue.cycle Object providing the cycle info
   * @param {Function} schedulingIssue.getResolutions Returns possible resolutions
   * @param {Function} continueWithResolutionResult Function to call after a resolution is chosen to
   * proceed with the Engine calculations:
   * ```js
   * project.on('cycle', ({ continueWithResolutionResult }) => {
   *     // cancel changes in case of a cycle
   *     continueWithResolutionResult(EffectResolutionResult.Cancel);
   * })
   * ```
   */
  /**
   * Fired when the Engine detects a scheduling conflict.
   * @event schedulingConflict
   * @param {Object} schedulingIssue The conflict details:
   * @param {Function} schedulingIssue.getDescription Returns the conflict description
   * @param {Object[]} schedulingIssue.intervals Array of conflicting intervals
   * @param {Function} schedulingIssue.getResolutions Function to get possible resolutions
   * @param {Function} continueWithResolutionResult Function to call after a resolution is chosen to
   * proceed with the Engine calculations:
   * ```js
   * project.on('schedulingConflict', ({ schedulingIssue, continueWithResolutionResult }) => {
   *     // apply the first resolution and continue
   *     schedulingIssue.getResolutions()[0].resolve();
   *     continueWithResolutionResult(EffectResolutionResult.Resume);
   * })
   * ```
   */
  /**
   * Fired when the Engine detects a calendar misconfiguration when the calendar does
   * not provide any working periods of time which makes the calendar usage impossible.
   * @event emptyCalendar
   * @param {Object} schedulingIssue Scheduling error describing the case:
   * @param {Function} schedulingIssue.getDescription Returns the error description
   * @param {Function} schedulingIssue.getCalendar Returns the calendar that must be fixed
   * @param {Function} schedulingIssue.getResolutions Returns possible resolutions
   * @param {Function} continueWithResolutionResult Function to call after a resolution is chosen to
   * proceed with the Engine calculations:
   * ```js
   * project.on('emptyCalendar', ({ schedulingIssue, continueWithResolutionResult }) => {
   *     // apply the first resolution and continue
   *     schedulingIssue.getResolutions()[0].resolve();
   *     continueWithResolutionResult(EffectResolutionResult.Resume);
   * })
   * ```
   */
  //endregion
  //region Config
  static get $name() {
    return 'ProjectModel';
  }
  /**
   * Silences propagations caused by the project loading.
   *
   * Applying the loaded data to the project occurs in two basic stages:
   *
   * 1. Data gets into the engine graph which triggers changes propagation
   * 2. The changes caused by the propagation get written to related stores
   *
   * Setting this flag to `true` makes the component perform step 2 silently without triggering events causing reactions on those changes
   * (like sending changes back to the server if `autoSync` is enabled) and keeping stores in unmodified state.
   *
   * This is safe if the loaded data is consistent so propagation doesn't really do any adjustments.
   * By default the system treats the data as consistent so this option is `true`.
   *
   * ```js
   * new SchedulerPro{
   *     project : {
   *         // We want scheduling engine to recalculate the data properly
   *         // so then we could save it back to the server
   *         silenceInitialCommit : false,
   *         ...
   *     }
   *     ...
   * })
   * ```
   *
   * @config {Boolean} silenceInitialCommit
   * @default true
   * @category Advanced
   */
  /**
   * Maximum range the project calendars can iterate.
   * The value is defined in milliseconds and by default equals `5 years` roughly.
   * ```javascript
   * new SchedulerPro({
   *     project : {
   *         // adjust calendar iteration limit to 10 years roughly:
   *         // 10 years expressed in ms
   *         maxCalendarRange : 10 * 365 * 24 * 3600000,
   *         ...
   *     }
   * });
   * ```
   * @config {Number} maxCalendarRange
   * @default 157680000000
   * @category Advanced
   */
  /**
   * When `true` the project manually scheduled tasks will adjust their proposed start/end dates
   * to skip non working time.
   *
   * @field {Boolean} skipNonWorkingTimeWhenSchedulingManually
   * @default false
   */
  /**
   * When `true` the project's manually scheduled tasks adjust their duration by excluding the non-working time from it,
   * according to the calendar. However, this may lead to inconsistencies, when moving an event which both starts
   * and ends on the non-working time. For such cases you can disable this option.
   *
   * Default value is `true`
   *
   * IMPORTANT: Setting this option to `false` also forcefully sets the {@link #field-skipNonWorkingTimeWhenSchedulingManually} option
   * to `false`.
   * IMPORTANT: This option is going to be disabled by default from version 6.0.0.
   *
   * @field {Boolean} skipNonWorkingTimeInDurationWhenSchedulingManually
   * @default true
   */
  /**
   * This config manages DST correction in the scheduling engine. It only has effect when DST transition hour is
   * working time. Usually DST transition occurs on Sunday, so with non working weekends the DST correction logic
   * is not involved.
   *
   * If **true**, it will add/remove one hour when calculating end date. For example:
   * Assume weekends are working and on Sunday, 2020-10-25 at 03:00 clocks are set back 1 hour. Assume there is an event:
   *
   * ```javascript
   * {
   *     startDate    : '2020-10-20',
   *     duration     : 10 * 24 + 1,
   *     durationUnit : 'hour'
   * }
   * ```
   * It will end on 2020-10-30 01:00 (which is wrong) but duration will be reported correctly. Because of the DST
   * transition the SchedulerPro project will add one more hour when calculating the end date.
   *
   * Also this may occur when day with DST transition is working but there are non-working intervals between that day
   * and event end date.
   *
   * ```javascript
   * {
   *     calendar         : 1,
   *     calendarsData    : [
   *         {
   *             id           : 1,
   *             startDate    : '2020-10-26',
   *             endDate      : '2020-10-27',
   *             isWorking    : false
   *         }
   *     ],
   *     eventsData       : [
   *         {
   *             id           : 1,
   *             startDate    : '2020-10-20',
   *             endDate      : '2020-10-30'
   *         },
   *         {
   *             id           : 2,
   *             startDate    : '2020-10-20',
   *             duration     : 10 * 24 + 1,
   *             durationUnit : 'hour'
   *         }
   *     ]
   * }
   * ```
   *
   * Event 1 duration will be incorrectly reported as 9 days * 24 hours, missing 1 extra hour added by DST transition.
   * Event 2 end date will be calculated to 2020-10-30 01:00, adding one extra hour.
   *
   * If **false**, the SchedulerPro project will not add DST correction which fixes the quirk mentioned above.
   * Event 1 duration will be correctly reported as 9 days * 24 hours + 1 hour. Event 2 end date will be calculated
   * to 2020-10-30.
   *
   * Also, for those events days duration will be a floating point number due to extra (or missing) hour:
   *
   * ```javascript
   * eventStore.getById(1).getDuration('day')  // 10.041666666666666
   * eventStore.getById(1).getDuration('hour') // 241
   * ```
   *
   * @config {Boolean} adjustDurationToDST
   * @default false
   */
  /**
   * The number of hours per day.
   *
   * **Please note:** the value **does not define** the amount of **working** time per day
   * for that purpose one should use calendars.
   *
   * The value is used when converting the duration from one unit to another.
   * So when user enters a duration of, for example, `5 days` the system understands that it
   * actually means `120 hours` and schedules accordingly.
   * @field {Number} hoursPerDay
   * @default 24
   */
  /**
   * The number of days per week.
   *
   * **Please note:** the value **does not define** the amount of **working** time per week
   * for that purpose one should use calendars.
   *
   * The value is used when converting the duration from one unit to another.
   * So when user enters a duration of, for example, `2 weeks` the system understands that it
   * actually means `14 days` (which is then converted to {@link #field-hoursPerDay hours}) and
   * schedules accordingly.
   * @field {Number} daysPerWeek
   * @default 7
   */
  /**
   * The number of days per month.
   *
   * **Please note:** the value **does not define** the amount of **working** time per month
   * for that purpose one should use calendars.
   *
   * The value is used when converting the duration from one unit to another.
   * So when user enters a duration of, for example, `1 month` the system understands that it
   * actually means `30 days` (which is then converted to {@link #field-hoursPerDay hours}) and
   * schedules accordingly.
   * @field {Number} daysPerMonth
   * @default 30
   */
  /**
   * The scheduling direction of the project events.
   * Possible values are `Forward` and `Backward`. The `Forward` direction corresponds to the As-Soon-As-Possible scheduling (ASAP),
   * `Backward` - to As-Late-As-Possible (ALAP).
   * @field {'Forward'|'Backward'} direction
   * @default 'Forward'
   */
  /**
   * The source of the calendar for dependencies (the calendar used for taking dependencies lag into account).
   * Possible values are:
   *
   * - `ToEvent` - successor calendar will be used (default);
   * - `FromEvent` - predecessor calendar will be used;
   * - `Project` - the project calendar will be used.
   *
   * @field {'ToEvent'|'FromEvent'|'Project'} dependenciesCalendar
   * @default 'ToEvent'
   */
  /**
   * The project calendar.
   * @field {SchedulerPro.model.CalendarModel} calendar
   * @accepts {String|CalendarModelConfig|SchedulerPro.model.CalendarModel}
   */
  /**
   * Returns current Project changes as an object consisting of added/modified/removed arrays of records for every
   * managed store. Returns `null` if no changes exist. Format:
   *
   * ```javascript
   * {
   *     resources : {
   *         added    : [{ name : 'New guy' }],
   *         modified : [{ id : 2, name : 'Mike' }],
   *         removed  : [{ id : 3 }]
   *     },
   *     events : {
   *         modified : [{  id : 12, name : 'Cool task' }]
   *     },
   *     ...
   * }
   * ```
   *
   * @member {Object} changes
   * @readonly
   * @category Models & Stores
   */
  /**
   * Project changes (CRUD operations to records in its stores) are automatically committed on a buffer to the
   * underlying graph based calculation engine. The engine performs it calculations async.
   *
   * By calling this function, the commit happens right away. And by awaiting it you are sure that project
   * calculations are finished and that references between records are up to date.
   *
   * The returned promise is resolved with an object. If that object has `rejectedWith` set, there has been a conflict and the calculation failed.
   *
   * ```javascript
   * // Move an event in time
   * eventStore.first.shift(1);
   *
   * // Trigger calculations directly and wait for them to finish
   * const result = await project.commitAsync();
   *
   * if (result.rejectedWith) {
   *     // there was a conflict during the scheduling
   * }
   * ```
   *
   * @async
   * @function commitAsync
   * @category Common
   */
  /**
   * Set to `true` to enable calculation progress notifications.
   * When enabled the project fires {@link #event-progress progress} event.
   *
   * **Note**: Enabling progress notifications will impact calculation performance, since it needs to pause calculations to allow redrawing the UI.
   * @config {Boolean} enableProgressNotifications
   * @category Advanced
   */
  /**
   * Enables/disables the calculation progress notifications.
   * @member {Boolean} enableProgressNotifications
   * @category Advanced
   */
  /**
   * If this flag is set to `true` (default) when a start/end date is set on the event, a corresponding
   * `start-no-earlier/later-than` constraint is added, automatically. This is done in order to
   * keep the event "attached" to this date, according to the user intention.
   *
   * Depending on your use case, you might want to disable this behaviour.
   *
   * @field {Boolean} addConstraintOnDateSet
   * @default true
   */
  static get defaultConfig() {
    return {
      /**
       * @hideproperties project, taskStore
       */
      //region Inline data configs & properties
      /**
       * Get/set {@link #property-eventStore} data.
       *
       * Always returns an array of {@link SchedulerPro.model.EventModel EventModels} but also accepts an array of
       * its configuration objects as input.
       *
       * @member {SchedulerPro.model.EventModel[]} events
       * @accepts {SchedulerPro.model.EventModel[]|EventModelConfig[]}
       * @category Inline data
       */
      /**
       * Data use to fill the {@link #property-eventStore}. Should be an array of
       * {@link SchedulerPro.model.EventModel EventModels} or its configuration objects.
       *
       * @config {SchedulerPro.model.EventModel[]|EventModelConfig[]} events
       * @category Inline data
       */
      /**
       * Get/set {@link #property-resourceStore} data.
       *
       * Always returns an array of {@link SchedulerPro.model.ResourceModel ResourceModels} but also accepts an
       * array of its configuration objects as input.
       *
       * @member {SchedulerPro.model.ResourceModel[]} resources
       * @accepts {SchedulerPro.model.ResourceModel[]|ResourceModelConfig[]}
       * @category Inline data
       */
      /**
       * Data use to fill the {@link #property-resourceStore}. Should be an array of
       * {@link SchedulerPro.model.ResourceModel ResourceModels} or its configuration objects.
       *
       * @config {SchedulerPro.model.ResourceModel[]|ResourceModelConfig[]} resources
       * @category Inline data
       */
      /**
       * Get/set {@link #property-assignmentStore} data.
       *
       * Always returns an array of {@link SchedulerPro.model.AssignmentModel AssignmentModels} but also accepts
       * an array of its configuration objects as input.
       *
       * @member {SchedulerPro.model.AssignmentModel[]} assignments
       * @accepts {SchedulerPro.model.AssignmentModel[]|AssignmentModelConfig[]}
       * @category Inline data
       */
      /**
       * Data use to fill the {@link #property-assignmentStore}. Should be an array of
       * {@link SchedulerPro.model.AssignmentModel AssignmentModels} or its configuration objects.
       *
       * @config {SchedulerPro.model.AssignmentModel[]|AssignmentModelConfig[]} assignments
       * @category Inline data
       */
      /**
       * Get/set {@link #property-dependencyStore} data.
       *
       * Always returns an array of {@link SchedulerPro.model.DependencyModel DependencyModels} but also accepts an
       * array of its configuration objects as input.
       *
       * @member {SchedulerPro.model.DependencyModel[]} dependencies
       * @accepts {SchedulerPro.model.DependencyModel[]|DependencyModelConfig[]}
       * @category Inline data
       */
      /**
       * Data use to fill the {@link #property-dependencyStore}. Should be an array of
       * {@link SchedulerPro.model.DependencyModel DependencyModels} or its configuration objects.
       *
       * @config {SchedulerPro.model.DependencyModel[]|DependencyModelConfig[]} dependencies
       * @category Inline data
       */
      /**
       * Get/set {@link #property-timeRangeStore} data.
       *
       * Always returns an array of {@link Scheduler.model.TimeSpan TimeSpans} but also accepts an
       * array of its configuration objects as input.
       *
       * @member {Scheduler.model.TimeSpan[]} timeRanges
       * @accepts {Scheduler.model.TimeSpan[]|TimeSpanConfig[]}
       * @category Inline data
       */
      /**
       * Data use to fill the {@link #property-timeRangeStore}. Should be an array of
       * {@link Scheduler.model.TimeSpan TimeSpans} or its configuration objects.
       *
       * @config {Scheduler.model.TimeSpan[]|TimeSpanConfig[]} timeRanges
       * @category Inline data
       */
      /**
       * Get/set {@link #property-resourceTimeRangeStore} data.
       *
       * Always returns an array of {@link Scheduler.model.ResourceTimeRangeModel ResourceTimeRangeModels} but
       * also accepts an array of its configuration objects as input.
       *
       * @member {Scheduler.model.ResourceTimeRangeModel[]} resourceTimeRanges
       * @accepts {Scheduler.model.ResourceTimeRangeModel[]|ResourceTimeRangeModelConfig[]}
       * @category Inline data
       */
      /**
       * Data use to fill the {@link #property-resourceTimeRangeStore}. Should be an array
       * of {@link Scheduler.model.ResourceTimeRangeModel ResourceTimeRangeModels} or its configuration objects.
       *
       * @config {Scheduler.model.ResourceTimeRangeModel[]|ResourceTimeRangeModelConfig[]} resourceTimeRanges
       * @category Inline data
       */
      //endregion
      //region Legacy inline data configs & properties
      /**
       * The initial data, to fill the {@link #property-eventStore eventStore} with.
       * Should be an array of {@link SchedulerPro.model.EventModel EventModels} or its configuration objects.
       *
       * @config {SchedulerPro.model.EventModel[]} eventsData
       * @category Legacy inline data
       */
      /**
       * The initial data, to fill the {@link #property-dependencyStore dependencyStore} with.
       * Should be an array of {@link SchedulerPro.model.DependencyModel DependencyModels} or its configuration
       * objects.
       *
       * @config {SchedulerPro.model.DependencyModel[]} [dependenciesData]
       * @category Legacy inline data
       */
      /**
       * The initial data, to fill the {@link #property-resourceStore resourceStore} with.
       * Should be an array of {@link SchedulerPro.model.ResourceModel ResourceModels} or its configuration objects.
       *
       * @config {SchedulerPro.model.ResourceModel[]} [resourcesData]
       * @category Legacy inline data
       */
      /**
       * The initial data, to fill the {@link #property-assignmentStore assignmentStore} with.
       * Should be an array of {@link SchedulerPro.model.AssignmentModel AssignmentModels} or its configuration
       * objects.
       *
       * @config {SchedulerPro.model.AssignmentModel[]} [assignmentsData]
       * @category Legacy inline data
       */
      //endregion
      //region Store configs and properties
      /**
       * The {@link SchedulerPro.data.EventStore store} holding the event information.
       *
       * See also {@link SchedulerPro.model.EventModel}
       *
       * @member {SchedulerPro.data.EventStore} eventStore
       * @category Models & Stores
       */
      /**
       * An {@link SchedulerPro.data.EventStore} instance or a config object.
       * @config {SchedulerPro.data.EventStore|EventStoreConfig} eventStore
       * @category Models & Stores
       */
      /**
       * The {@link SchedulerPro.data.DependencyStore store} holding the dependency information.
       *
       * See also {@link SchedulerPro.model.DependencyModel}
       *
       * @member {SchedulerPro.data.DependencyStore} dependencyStore
       * @category Models & Stores
       */
      /**
       * A {@link SchedulerPro.data.DependencyStore} instance or a config object.
       * @config {SchedulerPro.data.DependencyStore|DependencyStoreConfig} dependencyStore
       * @category Models & Stores
       */
      /**
       * The {@link SchedulerPro.data.ResourceStore store} holding the resources that can be assigned to the
       * events in the event store.
       *
       * See also {@link SchedulerPro.model.ResourceModel}
       *
       * @member {SchedulerPro.data.ResourceStore} resourceStore
       * @category Models & Stores
       */
      /**
       * A {@link SchedulerPro.data.ResourceStore} instance or a config object.
       * @config {SchedulerPro.data.ResourceStore|ResourceStoreConfig} resourceStore
       * @category Models & Stores
       */
      /**
       * The {@link SchedulerPro.data.AssignmentStore store} holding the assignment information.
       *
       * See also {@link SchedulerPro.model.AssignmentModel}
       *
       * @member {SchedulerPro.data.AssignmentStore} assignmentStore
       * @category Models & Stores
       */
      /**
       * An {@link SchedulerPro.data.AssignmentStore} instance or a config object.
       * @config {SchedulerPro.data.AssignmentStore|AssignmentStoreConfig} assignmentStore
       * @category Models & Stores
       */
      /**
       * The {@link SchedulerPro.data.CalendarManagerStore store} holding the calendar information.
       *
       * See also {@link SchedulerPro.model.CalendarModel}
       * @member {SchedulerPro.data.CalendarManagerStore} calendarManagerStore
       * @category Models & Stores
       */
      /**
       * A {@link SchedulerPro.data.CalendarManagerStore} instance or a config object.
       * @config {SchedulerPro.data.CalendarManagerStore|CalendarManagerStoreConfig} calendarManagerStore
       * @category Models & Stores
       */
      //endregion
      //region Model & store class configs
      /**
       * The constructor of the calendar model class, to be used in the project. Will be set as the
       * {@link Core.data.Store#config-modelClass modelClass} property of the
       * {@link #property-calendarManagerStore}
       *
       * @config {SchedulerPro.model.CalendarModel} [calendarModelClass]
       * @typings {typeof CalendarModel}
       * @category Models & Stores
       */
      calendarModelClass: CalendarModel,
      /**
       * The constructor of the dependency model class, to be used in the project. Will be set as the
       * {@link Core.data.Store#config-modelClass modelClass} property of the {@link #property-dependencyStore}
       *
       * @config {SchedulerPro.model.DependencyModel}
       * @typings {typeof DependencyModel}
       * @category Models & Stores
       */
      dependencyModelClass: DependencyModel,
      /**
       * The constructor of the event model class, to be used in the project. Will be set as the
       * {@link Core.data.Store#config-modelClass modelClass} property of the {@link #property-eventStore}
       *
       * @config {SchedulerPro.model.EventModel}
       * @typings {typeof EventModel}
       * @category Models & Stores
       */
      eventModelClass: EventModel,
      /**
       * The constructor of the assignment model class, to be used in the project. Will be set as the
       * {@link Core.data.Store#config-modelClass modelClass} property of the {@link #property-assignmentStore}
       *
       * @config {SchedulerPro.model.AssignmentModel}
       * @typings {typeof AssignmentModel}
       * @category Models & Stores
       */
      assignmentModelClass: AssignmentModel,
      /**
       * The constructor of the resource model class, to be used in the project. Will be set as the
       * {@link Core.data.Store#config-modelClass modelClass} property of the {@link #property-resourceStore}
       *
       * @config {SchedulerPro.model.ResourceModel}
       * @typings {typeof ResourceModel}
       * @category Models & Stores
       */
      resourceModelClass: ResourceModel,
      /**
       * The constructor to create a calendar store instance with. Should be a class, subclassing the
       * {@link SchedulerPro.data.CalendarManagerStore}
       * @config {SchedulerPro.data.CalendarManagerStore|Object}
       * @typings {typeof CalendarManagerStore|object}
       * @category Models & Stores
       */
      calendarManagerStoreClass: CalendarManagerStore,
      /**
       * The constructor to create a dependency store instance with. Should be a class, subclassing the
       * {@link SchedulerPro.data.DependencyStore}
       * @config {SchedulerPro.data.DependencyStore|Object}
       * @typings {typeof DependencyStore|object}
       * @category Models & Stores
       */
      dependencyStoreClass: DependencyStore,
      /**
       * The constructor to create an event store instance with. Should be a class, subclassing the
       * {@link SchedulerPro.data.EventStore}
       * @config {SchedulerPro.data.EventStore|Object}
       * @typings {typeof EventStore|object}
       * @category Models & Stores
       */
      eventStoreClass: EventStore,
      /**
       * The constructor to create an assignment store instance with. Should be a class, subclassing the
       * {@link SchedulerPro.data.AssignmentStore}
       * @config {SchedulerPro.data.AssignmentStore|Object}
       * @typings {typeof AssignmentStore|object}
       * @category Models & Stores
       */
      assignmentStoreClass: AssignmentStore,
      /**
       * The constructor to create a resource store instance with. Should be a class, subclassing the
       * {@link SchedulerPro.data.ResourceStore}
       * @config {SchedulerPro.data.ResourceStore|Object}
       * @typings {typeof ResourceStore|object}
       * @category Models & Stores
       */
      resourceStoreClass: ResourceStore,
      //endregion
      /**
       * The initial data, to fill the {@link #property-calendarManagerStore} with.
       * Should be an array of {@link SchedulerPro.model.CalendarModel} or it's configuration objects.
       *
       * @config {SchedulerPro.model.CalendarModel[]}
       * @category Legacy inline data
       */
      calendarsData: null,
      /**
       * Set to `true` to reset the undo/redo queues of the internal {@link Core.data.stm.StateTrackingManager}
       * after the Project has loaded. Defaults to `false`
       * @config {Boolean} resetUndoRedoQueuesAfterLoad
       * @category Advanced
       */
      supportShortSyncResponseNote: 'Note: Please consider enabling "supportShortSyncResponse" option to allow less detailed sync responses (https://bryntum.com/products/schedulerpro/docs/api/SchedulerPro/model/ProjectModel#config-supportShortSyncResponse)',
      /**
       * Enables early rendering in SchedulerPro, by postponing calculations to after the first refresh.
       *
       * Requires event data loaded to be pre-normalized to function as intended, since it will be used to render
       * before engine has normalized the data. Given un-normalized data events will snap into place when
       * calculations are finished.
       *
       * The Gantt chart will be read-only until the initial calculations are finished.
       *
       * @config {Boolean}
       * @default
       * @category Advanced
       */
      delayCalculation: true,
      calendarManagerStore: {},
      stmClass: StateTrackingManager
    };
  }
  static get configurable() {
    return {
      /**
       * Get/set {@link #property-calendarManagerStore} data.
       *
       * Always returns a {@link SchedulerPro.model.CalendarModel} array but also accepts an array of
       * its configuration objects as input.
       *
       * @member {SchedulerPro.model.CalendarModel[]} calendars
       * @accepts {SchedulerPro.model.CalendarModel[]|CalendarModelConfig[]}
       * @category Inline data
       */
      /**
       * Data use to fill the {@link #property-eventStore}. Should be a {@link SchedulerPro.model.CalendarModel}
       * array or its configuration objects.
       *
       * @config {SchedulerPro.model.CalendarModel[]|CalendarModelConfig[]} calendars
       * @category Inline data
       */
      calendars: null
    };
  }
  // For TaskBoard compatibility
  get taskStore() {
    return this.eventStore;
  }
  //endregion
  //region Inline data
  get calendars() {
    return this.calendarManagerStore.allRecords;
  }
  updateCalendars(calendars) {
    this.calendarManagerStore.data = calendars;
  }
  //endregion
}

ProjectModel._$name = 'ProjectModel';

/**
 * @module SchedulerPro/view/mixin/SchedulerProEventRendering
 */
/**
 * Config for event layout
 * @typedef {Object} EventLayoutConfig
 * @property {'stack'|'pack'|'mixed'|'none'} type Event layout type. Possible values for horizontal mode are
 * `stack`, `pack` and `none`. For vertical mode: `pack`, `mixed` and `none`.
 * @property {Function} layoutFn Horizontal mode only. This function allows to manually position events inside the row.
 * @property {Object} weights Horizontal mode only. Specifies groups order.
 * @property {String|Function} groupBy Horizontal mode only. Specifies a way to group events inside a row.
 */
/**
 * Functions to handle event rendering in Scheduler Pro (EventModel -> dom elements).
 *
 * @mixin
 */
var SchedulerProEventRendering = (Target => class SchedulerProEventRendering extends (Target || Base) {
  static get $name() {
    return 'SchedulerProEventRendering';
  }
  static get configurable() {
    return {
      /**
       * This config defines how to handle overlapping events. Valid values are:
       * - `stack`, adjusts row height (only horizontal)
       * - `pack`, adjusts event height
       * - `mixed`, allows two events to overlap, more packs (only vertical)
       * - `none`, allows events to overlap
       *
       * You can also provide a configuration object accepted by
       * {@link SchedulerPro.eventlayout.ProHorizontalLayout} to group events or even take control over the
       * layout (i.e. vertical position and height):
       *
       * To group events:
       *
       * ```javascript
       * new SchedulerPro({
       *     eventLayout : {
       *         type    : 'stack',
       *         weights : {
       *             high   : 100,
       *             normal : 150,
       *             low    : 200
       *         },
       *         groupBy : 'prio'
       *     }
       * });
       * ```
       *
       * To take control over the layout:
       *
       * ```javascript
       * new SchedulerPro({
       *     eventLayout : {
       *         layoutFn : items => {
       *             items.forEach(item => {
       *                 item.top = 100 * Math.random();
       *                 item.height = 100 * Math.random();
       *             });
       *
       *             return 100;
       *         }
       *     }
       * });
       * ```
       *
       * For more info on grouping and layout please refer to {@link SchedulerPro.eventlayout.ProHorizontalLayout}
       * doc article.
       *
       * @prp {'stack'|'pack'|'mixed'|'none'|EventLayoutConfig}
       * @default
       * @category Scheduled events
       */
      eventLayout: 'stack',
      /**
       * The class responsible for the packing horizontal event layout process.
       * Override this to take control over the layout process.
       * @config {Scheduler.eventlayout.HorizontalLayout}
       * @typings {typeof HorizontalLayout}
       * @default
       * @private
       * @category Misc
       */
      horizontalLayoutPackClass: ProHorizontalLayoutPack,
      /**
       * The class name responsible for the stacking horizontal event layout process.
       * Override this to take control over the layout process.
       * @config {Scheduler.eventlayout.HorizontalLayout}
       * @typings {typeof HorizontalLayout}
       * @default
       * @private
       * @category Misc
       */
      horizontalLayoutStackClass: ProHorizontalLayoutStack
    };
  }
  //region Config
  updateInternalEventLayout(eventLayout, oldEventLayout) {
    const me = this;
    if (!me.isConfiguring) {
      me.clearLayouts();
    }
    super.updateInternalEventLayout(eventLayout, oldEventLayout);
  }
  //endregion
  getEventLayout(config) {
    config = super.getEventLayout(config);
    if ('layoutFn' in config) {
      config.type = 'layoutFn';
    }
    return config;
  }
  clearLayouts() {
    const me = this;
    if (me.layouts) {
      for (const key in me.layouts) {
        me.layouts[key].destroy();
        delete me.layouts[key];
      }
    }
  }
  /**
   * Get event layout handler. The handler decides the vertical placement of events within a resource.
   * Returns null if no eventLayout is used (if {@link #config-eventLayout} is set to "none")
   * @internal
   * @returns {Scheduler.eventlayout.HorizontalLayout}
   * @readonly
   * @category Scheduled events
   */
  getEventLayoutHandler(eventLayout) {
    const me = this;
    if (!me.isHorizontal) {
      return null;
    }
    const {
      timeAxisViewModel,
      horizontal
    } = me;
    if (!me.layouts) {
      me.layouts = {};
    }
    const {
      layouts
    } = me;
    switch (eventLayout.type) {
      // stack, adjust row height to fit all events
      case 'stack':
        {
          if (!layouts.horizontalStack) {
            layouts.horizontalStack = me.horizontalLayoutStackClass.new({
              scheduler: me,
              timeAxisViewModel,
              bandIndexToPxConvertFn: horizontal.layoutEventVerticallyStack,
              bandIndexToPxConvertThisObj: horizontal,
              groupByThisObj: me
            }, eventLayout);
          }
          return layouts.horizontalStack;
        }
      // pack, fit all events in available height by adjusting their height
      case 'pack':
        {
          if (!layouts.horizontalPack) {
            layouts.horizontalPack = me.horizontalLayoutPackClass.new({
              scheduler: me,
              timeAxisViewModel,
              bandIndexToPxConvertFn: horizontal.layoutEventVerticallyPack,
              bandIndexToPxConvertThisObj: horizontal,
              groupByThisObj: me
            }, eventLayout);
          }
          return layouts.horizontalPack;
        }
      case 'layoutFn':
        {
          // Both methods are called on a layout
          return {
            type: 'layoutFn',
            scheduler: me,
            applyLayout: eventLayout.layoutFn,
            layoutEventsInBands: eventLayout.layoutFn
          };
        }
      default:
        return null;
    }
  }
  get widgetClass() {}
});

/**
 * @module SchedulerPro/view/orientation/ProHorizontalRendering
 */
/**
 * Handles event rendering in Scheduler Pro horizontal mode. Populates render data with buffer duration.
 *
 * @internal
 */
class ProHorizontalRendering extends HorizontalRendering {
  static $name = 'ProHorizontalRendering';
  /**
   * Populates render data with buffer data rendering.
   * @param {HorizontalRenderData} renderData
   * @returns {Boolean}
   * @private
   */
  fillInnerSpanHorizontalPosition(renderData) {
    const me = this,
      {
        eventRecord
      } = renderData,
      {
        startMS: innerStartMS,
        endMS: innerEndMS,
        durationMS: innerDurationMS
      } = me.calculateMS(eventRecord, 'startDate', 'endDate'),
      position = me.calculateHorizontalPosition(renderData, innerStartMS, innerEndMS, innerDurationMS);
    if (position) {
      const {
        left,
        width
      } = position;
      Object.assign(renderData, {
        innerStartMS,
        innerEndMS,
        innerDurationMS,
        bufferBeforeWidth: Math.max(left - renderData.left, 0),
        // This could yield a really small number due to floating point accuracy, we can round the result
        bufferAfterWidth: Math.max(Math.floor(renderData.left + renderData.width - left - width), 0)
      });
      return true;
    } else {
      return false;
    }
  }
  getTimeSpanRenderData(timeSpan, rowRecord, includeOutside = false) {
    const data = super.getTimeSpanRenderData(timeSpan, rowRecord, includeOutside);
    if (data !== null && data !== void 0 && data.useEventBuffer) {
      if (!this.fillInnerSpanHorizontalPosition(data)) {
        return null;
      }
    }
    return data;
  }
}
ProHorizontalRendering._$name = 'ProHorizontalRendering';

/**
 * @module SchedulerPro/view/orientation/ProVerticalRendering
 */
/**
 * Handles event rendering in Scheduler Pro horizontal mode. Populates render data with buffer duration.
 *
 * @internal
 */
class ProVerticalRendering extends VerticalRendering {
  static $name = 'ProVerticalRendering';
  /**
   * Populates render data with buffer data rendering.
   * @param {HorizontalRenderData} renderData
   * @returns {Boolean}
   * @private
   */
  fillInnerSpanVerticalPosition(renderData) {
    const me = this,
      {
        scheduler
      } = me,
      {
        eventRecord
      } = renderData,
      {
        isBatchUpdating
      } = eventRecord,
      startDate = isBatchUpdating ? eventRecord.get('startDate') : eventRecord.startDate,
      endDate = isBatchUpdating ? eventRecord.get('endDate') : eventRecord.endDate,
      top = scheduler.getCoordinateFromDate(startDate),
      innerStartMS = startDate.getTime(),
      innerEndMS = endDate.getTime(),
      innerDurationMS = innerEndMS - innerStartMS;
    let bottom = scheduler.getCoordinateFromDate(endDate),
      height = bottom - top;
    // Below, estimate height
    if (bottom === -1) {
      height = Math.round(innerDurationMS * scheduler.timeAxisViewModel.getSingleUnitInPixels('millisecond'));
      bottom = top + height;
    }
    Object.assign(renderData, {
      innerStartMS,
      innerEndMS,
      innerDurationMS,
      bufferBeforeWidth: top - renderData.top,
      bufferAfterWidth: renderData.top + renderData.height - top - height
    });
    return true;
  }
  getTimeSpanRenderData(timeSpan, rowRecord, includeOutside = false) {
    const data = super.getTimeSpanRenderData(timeSpan, rowRecord, includeOutside);
    if (data !== null && data !== void 0 && data.useEventBuffer) {
      if (!this.fillInnerSpanVerticalPosition(data)) {
        return null;
      }
    }
    return data;
  }
}
ProVerticalRendering._$name = 'ProVerticalRendering';

/**
 * @module SchedulerPro/view/SchedulerProBase
 */
/**
 * A thin base class for {@link SchedulerPro/view/SchedulerPro}. Includes fewer features by default, allowing smaller
 * custom built bundles if used in place of {@link SchedulerPro/view/SchedulerPro}.
 *
 * **NOTE:** In most scenarios you should use SchedulerPro instead of SchedulerProBase.
 *
 * @mixes SchedulerPro/view/mixin/SchedulerProEventRendering
 * @mixes SchedulerPro/view/mixin/ProjectProgressMixin
 *
 * @features SchedulerPro/feature/CalendarHighlight
 * @features SchedulerPro/feature/DependencyEdit
 * @features SchedulerPro/feature/EventBuffer
 * @features SchedulerPro/feature/EventResize
 * @features SchedulerPro/feature/EventSegmentDrag
 * @features SchedulerPro/feature/EventSegmentResize
 * @features SchedulerPro/feature/EventSegments
 * @features SchedulerPro/feature/NestedEvents
 * @features SchedulerPro/feature/PercentBar
 * @features SchedulerPro/feature/ResourceNonWorkingTime
 * @features SchedulerPro/feature/TaskEdit
 * @features SchedulerPro/feature/TimeSpanHighlight
 * @features SchedulerPro/feature/Versions
 *
 * @extends Scheduler/view/SchedulerBase
 * @mixes SchedulerPro/view/mixin/SchedulingIssueResolution
 * @widget
 */
class SchedulerProBase extends SchedulerBase.mixin(ProjectProgressMixin, SchedulingIssueResolution, SchedulerProEventRendering) {
  //region Config
  static get $name() {
    return 'SchedulerProBase';
  }
  static get type() {
    return 'schedulerprobase';
  }
  static get configurable() {
    return {
      projectModelClass: ProjectModel,
      /**
       * A task field (id, wbsCode, sequenceNumber etc) that will be used when displaying and editing linked tasks.
       * @config {String} dependencyIdField
       * @default 'id'
       */
      dependencyIdField: 'id'
    };
  }
  static get isSchedulerPro() {
    return true;
  }
  //endregion
  //region Store & model docs
  // Configs
  /**
   * A {@link SchedulerPro.model.ProjectModel} instance or a config object. The project holds all SchedulerPro data.
   * @config {SchedulerPro.model.ProjectModel|ProjectModelConfig} project
   * @category Data
   */
  /**
   * Inline events, will be loaded into the backing project's EventStore.
   * @config {SchedulerPro.model.EventModel[]|Object[]} events
   * @category Data
   */
  /**
   * The {@link SchedulerPro.data.EventStore} holding the events to be rendered into the scheduler.
   * @config {SchedulerPro.data.EventStore|EventStoreConfig} eventStore
   * @category Data
   */
  /**
   * Inline resources, will be loaded into the backing project's ResourceStore.
   * @config {SchedulerPro.model.ResourceModel[]|ResourceModelConfig[]} resources
   * @category Data
   */
  /**
   * The {@link SchedulerPro.data.ResourceStore} holding the resources to be rendered into the scheduler.
   * @config {SchedulerPro.data.ResourceStore|ResourceStoreConfig} resourceStore
   * @category Data
   */
  // For some reason Typings won't accept AssignmentModelConfig here. Object will be turned into it though
  /**
   * Inline assignments, will be loaded into the backing project's AssignmentStore.
   * @config {SchedulerPro.model.AssignmentModel[]|Object[]} assignments
   * @category Data
   */
  /**
   * The optional {@link SchedulerPro.data.AssignmentStore}, holding assignments between resources and events.
   * Required for multi assignments.
   * @config {SchedulerPro.data.AssignmentStore|AssignmentStoreConfig} assignmentStore
   * @category Data
   */
  /**
   * Inline dependencies, will be loaded into the backing project's DependencyStore.
   * @config {SchedulerPro.model.DependencyModel[]|DependencyModelConfig[]} dependencies
   * @category Data
   */
  /**
   * The optional {@link SchedulerPro.data.DependencyStore}.
   * @config {SchedulerPro.data.DependencyStore|DependencyStoreConfig} dependencyStore
   * @category Data
   */
  /**
   * Inline calendars, will be loaded into the backing project's CalendarManagerStore.
   * @config {SchedulerPro.model.CalendarModel[]|CalendarModelConfig[]} calendars
   * @category Data
   */
  // Properties
  /**
   * Get/set ProjectModel instance, containing the data visualized by the SchedulerPro.
   * @member {SchedulerPro.model.ProjectModel} project
   * @typings {ProjectModel}
   * @category Data
   */
  /**
   * Get/set events, applies to the backing project's EventStore.
   * @member {SchedulerPro.model.EventModel[]} events
   * @accepts {SchedulerPro.model.EventModel[]|EventModelConfig[]}
   * @category Data
   */
  /**
   * Get/set the event store instance of the backing project.
   * @member {SchedulerPro.data.EventStore} eventStore
   * @typings Scheduler/view/SchedulerBase:eventStore -> {Scheduler.data.EventStore||SchedulerPro.data.EventStore}
   * @category Data
   */
  /**
   * Get/set resources, applies to the backing project's ResourceStore.
   * @member {SchedulerPro.model.ResourceModel[]} resources
   * @accepts {SchedulerPro.model.ResourceModel[]|ResourceModelConfig[]}
   * @category Data
   */
  /**
   * Get/set the resource store instance of the backing project
   * @member {SchedulerPro.data.ResourceStore} resourceStore
   * @typings Scheduler/view/SchedulerBase:resourceStore -> {Scheduler.data.ResourceStore||SchedulerPro.data.ResourceStore}
   * @category Data
   */
  // For some reason Typings won't accept AssignmentModelConfig here. Object will be turned into it though
  /**
   * Get/set assignments, applies to the backing project's AssignmentStore.
   * @member {SchedulerPro.model.AssignmentModel[]} assignments
   * @accepts {SchedulerPro.model.AssignmentModel[]|Object[]}
   * @category Data
   */
  /**
   * Get/set the event store instance of the backing project.
   * @member {SchedulerPro.data.AssignmentStore} assignmentStore
   * @typings Scheduler/view/SchedulerBase:assignmentStore -> {Scheduler.data.AssignmentStore||SchedulerPro.data.AssignmentStore}
   * @category Data
   */
  /**
   * Get/set dependencies, applies to the backing projects DependencyStore.
   * @member {SchedulerPro.model.DependencyModel[]} dependencies
   * @accepts {SchedulerPro.model.DependencyModel[]|DependencyModelConfig[]}
   * @category Data
   */
  /**
   * Get/set the dependencies store instance of the backing project.
   * @member {SchedulerPro.data.DependencyStore} dependencyStore
   * @typings Scheduler/view/SchedulerBase:dependencyStore -> {Scheduler.data.DependencyStore||SchedulerPro.data.DependencyStore}
   * @category Data
   */
  /**
   * Get/set calendars, applies to the backing projects CalendarManagerStore.
   * @member {SchedulerPro.model.CalendarModel[]} calendars
   * @accepts {SchedulerPro.model.CalendarModel[]|CalendarModelConfig[]}
   * @category Data
   */
  //endregion
  //region Overrides
  onPaintOverride() {
    // Internal procedure used for paint method overrides
    // Not used in onPaint() because it may be chained on instance and Override won't be applied
  }
  //endregion
  //region Inline data
  // Pro specific extension of SchedulerStores
  set calendars(calendars) {
    this.project.calendars = calendars;
  }
  get calendars() {
    return this.project.calendars;
  }
  //endregion
  //region Mode
  /**
   * Get mode (horizontal/vertical)
   * @property {'horizontal'|'vertical'}
   * @readonly
   * @category Common
   */
  get mode() {
    return this._mode;
  }
  set mode(mode) {
    const me = this;
    me._mode = mode;
    if (!me[mode]) {
      me.element.classList.add(`b-sch-${mode}`);
      if (mode === 'horizontal') {
        me.horizontal = new ProHorizontalRendering(me);
        if (me.isPainted) {
          me.horizontal.init();
        }
      } else if (mode === 'vertical') {
        me.vertical = new ProVerticalRendering(me);
        if (me.rendered) {
          me.vertical.init();
        }
      }
    }
  }
  //endregion
  //region Internal
  // Overrides grid to take project loading into account
  toggleEmptyText() {
    const me = this;
    if (me.bodyContainer && me.rowManager) {
      DomHelper.toggleClasses(me.bodyContainer, 'b-grid-empty', !(me.rowManager.rowCount || me.project.isLoadingOrSyncing));
    }
  }
  // Needed to work with Gantt features
  get taskStore() {
    return this.project.eventStore;
  }
  //endregion
  createEvent(startDate, resourceRecord, row) {
    // For resources with a calendar, ensure the date is inside a working time range
    if (!resourceRecord.isWorkingTime(startDate)) {
      return;
    }
    // If task editor is active dblclick will trigger number of async actions:
    // store add which would schedule project commit
    // editor cancel on next animation frame
    // editor hide
    // rejecting previous transaction
    // and there is also dependency feature listening to transitionend on scheduler to draw lines after
    // It can happen that user dblclicks too fast, then event will be added, then dependency will schedule itself
    // to render, and then event will be removed as part of transaction rejection from editor. So we cannot add
    // event before active transaction is done.
    if (this.taskEdit && this.taskEdit.isEditing) {
      this.ion({
        aftertaskedit: () => super.createEvent(startDate, resourceRecord, row),
        once: true
      });
    } else {
      return super.createEvent(startDate, resourceRecord, row);
    }
  }
}
SchedulerProBase.initClass();
VersionHelper.setVersion('schedulerpro', '5.3.7');
SchedulerProBase._$name = 'SchedulerProBase';

/**
 * @module SchedulerPro/view/ResourceHistogram
 */
const emptyFn = () => {};
/**
 * An object representing a certain time interval.
 *
 * @typedef {Object} TickInfo
 * @property {Date} startDate The interval start date
 * @property {Date} endDate The interval end date
 */
/**
 * An object containing info on the resource allocation in a certain time interval.
 *
 * The object is used when rendering interval bars and tooltips so it additionally provides a `rectConfig` property
 * which contains a configuration object for the `rect` SVG-element representing the interval bar.
 *
 * @typedef {Object} ResourceAllocationIntervalInfo
 * @property {SchedulerPro.model.ResourceModel} resource Resource model
 * @property {Set} assignments Set of ongoing assignments for the interval
 * @property {Map} assignmentIntervals Individual ongoing assignments allocation indexed by assignments
 * @property {Number} effort Resource effort in the interval (in milliseconds)
 * @property {Boolean} isOverallocated `true` if the interval contains a fact of the resource overallocation
 * @property {Boolean} isUnderallocated `true` if the resource is underallocated in the interval
 * @property {Number} maxEffort Maximum possible resource effort in the interval (in milliseconds)
 * @property {DomConfig} rectConfig The rectangle DOM configuration object
 * @property {TickInfo} tick The time interval
 * @property {Number} units Resource allocation in percents
 */
/**
 * This widget displays a read-only timeline report of the workload for the resources in a
 * {@link SchedulerPro/model/ProjectModel project}. The resource allocation is visualized as bars along the time axis
 * with an optional line indicating the maximum available time for each resource. A {@link SchedulerPro/column/ScaleColumn}
 * is also added automatically.
 *
 * To create a standalone histogram, simply configure it with a Project instance:
 *
 * ```javascript
 * const project = new ProjectModel({
 *     autoLoad  : true,
 *     transport : {
 *         load : {
 *             url : 'examples/schedulerpro/view/data.json'
 *         }
 *     }
 * });
 *
 * const histogram = new ResourceHistogram({
 *     project,
 *     appendTo    : 'targetDiv',
 *     rowHeight   : 60,
 *     minHeight   : '20em',
 *     flex        : '1 1 50%',
 *     showBarTip  : true,
 *     columns     : [
 *         {
 *             width : 200,
 *             field : 'name',
 *             text  : 'Resource'
 *         }
 *     ]
 * });
 * ```
 *
 * {@inlineexample SchedulerPro/view/ResourceHistogram.js}
 *
 * ## Pairing the component
 *
 * You can also pair the histogram with other timeline views such as the Gantt or Scheduler,
 * using the {@link Scheduler/view/TimelineBase#config-partner} config.
 *
 * You can configure (or hide completely) the built-in scale column easily:
 *
 * ```javascript
 * const histogram = new ResourceHistogram({
 *    project,
 *    appendTo    : 'targetDiv',
 *    columns     : [
 *        {
 *            width : 200,
 *            field : 'name',
 *            text  : 'Resource'
 *        },
 *        // Hide the scale column (or add any other column configs)
 *        {
 *            type   : 'scale',
 *            hidden : true
 *        }
 *    ]
 * });
 * ```
 *
 * ## Changing displayed values
 *
 * To change the histogram bar texts, supply a {@link #config-getBarText} function.
 * Here for example the provided function displays resources time **left** instead of
 * allocated time
 *
 * ```javascript
 * new ResourceHistogram({
 *     getBarText(datum) {
 *         const resourceHistogram = this.owner;
 *
 *         // get default bar text
 *         let result = resourceHistogram.getBarTextDefault();
 *
 *         // and if some work is done in the tick
 *         if (result) {
 *
 *             const unit = resourceHistogram.getBarTextEffortUnit();
 *
 *             // display the resource available time
 *             result = resourceHistogram.getEffortText(datum.maxEffort - datum.effort, unit);
 *         }
 *
 *         return result;
 *     },
 * })
 * ```
 *
 * @extends SchedulerPro/view/SchedulerProBase
 * @classtype resourcehistogram
 * @widget
 */
class ResourceHistogram extends SchedulerProBase {
  //region Config
  static $name = 'ResourceHistogram';
  static type = 'resourcehistogram';
  /**
   * @hideconfigs durationDisplayPrecision, resourceColumns, enableRecurringEvents, eventBarTextField,
   * eventBodyTemplate, eventColor, eventLayout, eventRenderer, eventRendererThisObj, eventStyle,
   * horizontalEventSorterFn, horizontalLayoutPackClass, horizontalLayoutStackClass, milestoneAlign,
   * milestoneTextPosition, highlightPredecessors, highlightSuccessors, removeUnassignedEvent,
   * eventAssignHighlightCls, eventCls, eventSelectedCls, fixedEventCls, overScheduledEventClass,
   * timeZone
   */
  static configurable = {
    sortFeatureStore: 'store',
    timeAxisColumnCellCls: 'b-sch-timeaxis-cell b-resourcehistogram-cell',
    /**
     * Effort value format string.
     * Must be a template supported by {@link Core/helper/util/NumberFormat} class.
     * @config {String}
     * @default
     */
    effortFormat: '0.#',
    /**
     * Specifies whether effort values should display units or not.
     * @config {Boolean}
     * @default
     */
    showEffortUnit: true,
    rowHeight: 50,
    useProjectTimeUnitsForScale: false,
    /**
     * Default time unit to display resources effort values.
     * The value is used as default when displaying effort in tooltips and bars text.
     * Yet the effective time unit used might change dynamically when zooming in the histogram
     * so its ticks unit gets smaller than the default unit.
     * Please use {@link #config-barTipEffortUnit} to customize default units for tooltips only
     * and {@link #config-barTextEffortUnit} to customize default units in bar texts.
     * @config {'millisecond'|'second'|'minute'|'hour'|'day'|'week'|'month'|'quarter'|'year'}
     * @default hour
     */
    effortUnit: TimeUnit.Hour,
    /**
     * Default time unit used for displaying resources effort in bars.
     * Yet the effective time unit used might change dynamically when zooming in the histogram
     * so its ticks unit gets smaller than the default unit.
     * Please use {@link #config-barTipEffortUnit} to customize default units for tooltips
     * (or {@link #config-effortUnit} to customize both texts and tooltips default units).
     * @config {'millisecond'|'second'|'minute'|'hour'|'day'|'week'|'month'|'quarter'|'year'}
     * @default hour
     */
    barTextEffortUnit: null,
    /**
     * Default time unit used when displaying resources effort in tooltips.
     * Yet the effective time unit used might change dynamically when zooming in the histogram
     * so its ticks unit gets smaller than the default unit.
     * Please use {@link #config-barTextEffortUnit} to customize default units for bar texts
     * (or {@link #config-effortUnit} to customize both texts and tooltips default units).
     * @config {'millisecond'|'second'|'minute'|'hour'|'day'|'week'|'month'|'quarter'|'year'}
     * @default hour
     */
    barTipEffortUnit: null,
    /**
     * Set to `true` if you want to display the maximum resource allocation line.
     * @config {Boolean}
     * @default
     */
    showMaxEffort: true,
    /**
     * Set to `true` if you want to display resources effort values in bars
     * (for example: `24h`, `7d`, `60min` etc.).
     * The text contents can be changed by providing {@link #config-getBarText} function.
     * @config {Boolean}
     */
    showBarText: false,
    /**
     * Set to `true` if you want to display a tooltip when hovering an allocation bar. You can also pass a
     * {@link Core/widget/Tooltip} config object.
     * Please use {@link #config-barTooltipTemplate} function to customize the tooltip contents.
     * @config {Boolean|TooltipConfig}
     */
    showBarTip: false,
    barTooltip: null,
    barTooltipClass: Tooltip,
    series: {
      maxEffort: {
        type: 'outline',
        field: 'maxEffort'
      },
      effort: {
        type: 'bar',
        field: 'effort'
      }
    },
    /**
     * A Function which returns a CSS class name to add to a rectangle element.
     * The following parameters are passed:
     * @param {Object} series - The series being rendered
     * @param {DomConfig} rectConfig - The rectangle configuration object
     * @param {Object} datum - The datum being rendered
     * @param {Number} index - The index of the datum being rendered
     * @config {Function}
     */
    getRectClass: null,
    createEventOnDblClick: false,
    readOnly: true,
    /**
     * A Function which returns the tooltip text to display when hovering a bar.
     * The following parameters are passed:
     * @param {Object} series - The series being rendered
     * @param {DomConfig} rectConfig - The rectangle configuration object
     * @param {Object} datum - The datum being rendered
     * @param {Number} index - The index of the datum being rendered
     * @deprecated Since 5.0.0. Please use {@link #config-barTooltipTemplate}
     * @config {Function}
     */
    getBarTip: null,
    /**
     * A Function which returns the tooltip text to display when hovering a bar.
     * The following parameters are passed:
     * @param {Object} context The tooltip context info
     * @param {ResourceAllocationIntervalInfo} context.datum The histogram bar being hovered info
     * @param {Core.widget.Tooltip} context.tip The tooltip instance
     * @param {HTMLElement} context.element The Element for which the Tooltip is monitoring mouse movement
     * @param {HTMLElement} context.activeTarget The target element that triggered the show
     * @param {Event} context.event The raw DOM event
     * @config {Function}
     */
    barTooltipTemplate({
      datum
    }) {
      let result = '';
      const {
        effort,
        isGroup
      } = datum;
      if (effort) {
        if (isGroup) {
          result = this.getGroupBarTip(...arguments);
        } else {
          result = this.getResourceBarTip(...arguments);
        }
      }
      return result;
    },
    /**
     * A Function which returns the text to render inside a bar.
     *
     * Here for example the provided function displays resources time **left** instead of
     * allocated time
     *
     * ```javascript
     * new ResourceHistogram({
     *     getBarText(datum) {
     *         const resourceHistogram = this.owner;
     *
     *         const { showBarText } = resourceHistogram;
     *
     *         let result = '';
     *
     *         // respect existing API - show bar texts only when "showBarText" is true
     *         // and if some work is done in the tick
     *         if (showBarText && datum.effort) {
     *
     *             const unit = resourceHistogram.getBarTextEffortUnit();
     *
     *             // display the resource available time
     *             result = resourceHistogram.getEffortText(datum.maxEffort - datum.effort, unit);
     *         }
     *
     *         return result;
     *     },
     * })
     * ```
     *
     * **Please note** that the function will be injected into the underlying
     * {@link Core/widget/graph/Histogram} component that is used under the hood
     * to render actual charts.
     * So `this` will refer to the {@link Core/widget/graph/Histogram} instance, not
     * this class instance.
     * To access the view please use `this.owner` in the function:
     *
     * ```javascript
     * new ResourceHistogram({
     *     getBarText(datum) {
     *         // "this" in the method refers core Histogram instance
     *         // get the view instance
     *         const resourceHistogram = this.owner;
     *
     *         .....
     *     },
     * })
     * ```
     * The following parameters are passed:
     * @param {ResourceAllocationIntervalInfo} datum The datum being rendered
     * @param {Number} index The index of the datum being rendered
     * @returns {String} Tdxt to render inside the bar
     * @config {Function}
     */
    getBarText: null,
    getBarTextRenderData: undefined,
    groupBarTipAssignmentLimit: 5,
    histogramWidgetClass: Histogram,
    histogramWidgetConfig: null,
    /**
     * Set to `true` to include inactive tasks allocation and `false` to not take such tasks into account.
     * @config {Boolean}
     * @default
     */
    includeInactiveEvents: false,
    fixedRowHeight: true
  };
  // Cannot use `static properties = {}`, new Map would pollute the prototype
  static get properties() {
    return {
      allocationReportByRecord: new Map(),
      allocationDataByRecord: new Map(),
      allocationObserverByRecord: new Map(),
      resourceGroupsToUpdate: new Set(),
      resourceGroupsAllocation: new Map()
    };
  }
  //endregion
  //region Constructor/Destructor
  construct(config) {
    super.construct(config);
    const me = this;
    // debounce refreshRows calls
    me.scheduleRefreshRows = me.createOnFrame(me.refreshRows, [], me, true);
    me.horizontal.refreshResourcesWhenReady = me.horizontal.onAssignmentStoreChange = me.horizontal.renderer = function () {};
    me.rowManager.ion({
      beforeRowHeight: 'onBeforeRowHeight',
      renderRow: 'onRowManagerRenderRow',
      thisObj: me
    });
  }
  get timeAxis() {
    return super.timeAxis;
  }
  set timeAxis(timeAxis) {
    const currentTimeAxis = this._timeAxis;
    super.timeAxis = timeAxis;
    if (this.partner && !timeAxis || currentTimeAxis && currentTimeAxis === timeAxis) {
      return;
    }
    this._timeAxis.ion({
      name: 'timeAxis',
      endReconfigure: 'onTimeAxisEndReconfigure',
      thisObj: this
    });
  }
  async onRowManagerRenderRow({
    record
  }) {
    // render group level histogram and scale (when project is calculated)
    if (record.isSpecialRow) {
      const me = this;
      if (me.project.isDelayingCalculation) {
        await me.project.commitAsync();
        if (me.isDestroyed) {
          return;
        }
      }
      me.renderGroupHistogram(record);
      me.renderGroupScale(record);
    }
  }
  onDestroy() {
    var _me$_histogramWidget, _me$_groupHistogramWi;
    const me = this;
    for (const [record, observer] of (_me$allocationObserve = me.allocationObserverByRecord) === null || _me$allocationObserve === void 0 ? void 0 : _me$allocationObserve.entries()) {
      var _me$allocationObserve;
      if (record.removeObserver) {
        record.removeObserver(observer);
        me.allocationObserverByRecord.delete(record);
      }
    }
    for (const [record, entity] of (_me$allocationReportB = me.allocationReportByRecord) === null || _me$allocationReportB === void 0 ? void 0 : _me$allocationReportB.entries()) {
      var _me$allocationReportB;
      if (record.removeEntity) {
        var _record$removeEntity;
        (_record$removeEntity = record.removeEntity) === null || _record$removeEntity === void 0 ? void 0 : _record$removeEntity.call(record, entity);
        me.allocationReportByRecord.delete(entity);
      }
    }
    me.allocationDataByRecord.clear();
    (_me$_histogramWidget = me._histogramWidget) === null || _me$_histogramWidget === void 0 ? void 0 : _me$_histogramWidget.destroy();
    (_me$_groupHistogramWi = me._groupHistogramWidget) === null || _me$_groupHistogramWi === void 0 ? void 0 : _me$_groupHistogramWi.destroy();
    me.barTooltip = null;
  }
  //endregion
  //region Project
  updateProject(project) {
    this.detachListeners('resourceHistogramProject');
    project.ion({
      name: 'resourceHistogramProject',
      refresh: 'internalOnProjectRefresh',
      delayCalculationStart: 'onProjectDelayCalculationStart',
      delayCalculationEnd: 'onProjectDelayCalculationEnd',
      repopulateReplica: 'onRepopulateReplica',
      thisObj: this
    });
    this.store = project.resourceStore;
  }
  //endregion
  //region Internal
  scheduleRefreshRows() {}
  getEventsToRender() {}
  getRowHeight() {
    return this.rowHeight;
  }
  convertEffortUnit(value, unit, toUnit) {
    return this.project.run('$convertDuration', value, unit, toUnit);
  }
  updateUseProjectTimeUnitsForScale() {
    const me = this;
    // Below this.scalePoints assignment of doesn't work until ResourceHistogram is painted
    // since ScaleWidget being constructed tries to read its rootElement which results:
    // "Floating Widgets must have "rootElement" to be ..."
    if (me.isPainted) {
      // we need to regenerate ScaleColumn points according to new unit values
      const eventParams = {
        scalePoints: me.generateScalePoints()
      };
      /**
       * Fires when the component generates points for the {@link #property-scaleColumn scale column}.
       *
       * Use a listeners to override the generated scale points:
       *
       * ```javascript
       * new ResourceHistogram({
       *     ...
       *     listeners : {
       *         generateScalePoints(params) {
       *             // provide text for each scale point (if not provided already)
       *             params.scalePoints.forEach(point => {
       *                 point.text = point.text || point.value;
       *             });
       *         }
       *     }
       * })
       * ```
       *
       * @param {SchedulerPro.view.ResourceHistogram} source The component instance
       * @param {ScalePoint[]} scalePoints Array of objects representing scale points. Each entry can have properties:
       * - `value` - point value
       * - `unit` - point value unit
       * - `text` - label text (if not provided the point will not have a label displayed)
       * @event generateScalePoints
       */
      me.trigger('generateScalePoints', eventParams);
      // allow to override the points in a listener
      me._generatedScalePoints = me.scalePoints = eventParams.scalePoints;
      me.scheduleRefreshRows();
    }
  }
  get eventStore() {
    var _this$project;
    return (_this$project = this.project) === null || _this$project === void 0 ? void 0 : _this$project.eventStore;
  }
  set eventStore(eventStore) {
    super.eventStore = eventStore;
  }
  /**
   * The locked grid scale column reference.
   * @member {SchedulerPro.column.ScaleColumn} scaleColumn
   * @readonly
   */
  get scaleColumn() {
    return this.columns.query(column => column.isScaleColumn)[0];
  }
  get scalePoints() {
    return this._scalePoints;
  }
  set scalePoints(scalePoints) {
    const {
        histogramWidget,
        scaleColumn
      } = this,
      lastPoint = scalePoints[scalePoints.length - 1],
      {
        value: scaleMax,
        unit: scaleUnit
      } = lastPoint;
    this.scaleUnit = scaleUnit;
    this._scalePoints = scalePoints;
    let maxInScaleUnits = scaleMax;
    if (scaleColumn) {
      const {
        scaleWidget
      } = scaleColumn;
      maxInScaleUnits += scaleWidget.scaleMaxPadding * scaleMax;
    }
    // Applying new maximum value to the histogram.
    // We have to convert scale units to milliseconds since allocation report provides values in milliseconds.
    histogramWidget.topValue = this.useProjectTimeUnitsForScale ? this.convertEffortUnit(maxInScaleUnits, scaleUnit, TimeUnit.Millisecond) : DateHelper.asMilliseconds(maxInScaleUnits, scaleUnit);
    // Applying new points to the scale column
    if (scaleColumn) {
      scaleColumn.scalePoints = scalePoints;
    }
  }
  buildScalePointText(scalePoint) {
    return `${scalePoint.value}${DateHelper.getShortNameOfUnit(scalePoint.unit)}`;
  }
  /**
   * Generates points for the {@link #property-scaleColumn scale column}.
   *
   * **Override the method to customize the scale column points.**
   *
   * @param {Number} [scaleMax] Maximum value for the scale. Uses current timeaxis increment if not provided.
   * @param {'millisecond'|'second'|'minute'|'hour'|'day'|'week'|'month'|'quarter'|'year'} [unit] Time
   * unit `scaleMax` argument is expressed in.
   * Uses current timeaxis unit if not provided.
   * @returns {ScalePoint[]} Array of objects representing scale points. Each entry can have properties:
   * - `value` - point value
   * - `unit` - point value unit
   * - `text` - label text (if not provided the point will not have a label displayed)
   */
  generateScalePoints(scaleMax, unit) {
    var _this$project2;
    // bail out if there is no project or it's not in the graph
    if (!((_this$project2 = this.project) !== null && _this$project2 !== void 0 && _this$project2.graph)) {
      return;
    }
    const {
        timeAxis
      } = this,
      scalePoints = [];
    scaleMax = scaleMax || timeAxis.increment;
    unit = unit || timeAxis.unit;
    let scaleStep;
    // If the ticks are defined as 1 unit let's break it down to smaller units
    if (scaleMax === 1) {
      // getting timeaxis tick sub-unit and number of them in a tick
      unit = DateHelper.getSmallerUnit(unit);
      scaleMax = Math.round(this.useProjectTimeUnitsForScale ? this.convertEffortUnit(scaleMax, timeAxis.unit, unit) : DateHelper.as(unit, scaleMax, timeAxis.unit));
    }
    // Let's try to guess how many points in the scale will work nicely
    for (const factor of [7, 5, 4, 3, 2]) {
      // unitsNumber is multiple of "factor" -> we generate "factor"-number of points
      if (!(scaleMax % factor)) {
        scaleStep = scaleMax / factor;
        break;
      }
    }
    // fallback to a single point equal to maximum value
    if (!scaleStep) {
      scaleStep = scaleMax;
    }
    for (let value = scaleStep; value <= scaleMax; value += scaleStep) {
      scalePoints.push({
        value
      });
    }
    const lastPoint = scalePoints[scalePoints.length - 1];
    // put unit and label to the last point
    lastPoint.unit = unit;
    lastPoint.text = this.buildScalePointText(lastPoint);
    return scalePoints;
  }
  updateViewPreset(viewPreset) {
    const me = this;
    // Set a flag indicating that we're inside of `updateViewPreset` so our `onTimeAxisEndReconfigure` will skip its call.
    // We call it here later.
    me._updatingViewPreset = true;
    super.updateViewPreset(...arguments);
    me._updatingViewPreset = false;
    // In `super,updateViewPreset` function `this.render` is called which checks if the engine is not dirty
    // ..and we modify `ticksIdentifier` atom in `onTimeAxisEndReconfigure`
    // so the engine state gets dirty and rendering gets delayed which ends up an exception.
    // So we call `onTimeAxisEndReconfigure` after super `updateViewPreset` code
    // to keep the engine non-dirty while zooming/setting a preset.
    // This scenario is covered w/ SchedulerPro/tests/pro/view/ResourceHistogramZoom.t.js
    if (me.project.isInitialCommitPerformed && me.isPainted) {
      me.onTimeAxisEndReconfigure();
    }
  }
  onPaint({
    firstPaint
  }) {
    super.onPaint({
      firstPaint
    });
    if (firstPaint && this.showBarTip) {
      this.barTooltip = {};
    }
  }
  updateGetBarTip(value) {
    // reset barTooltipTemplate if custom getBarTip function is provided
    if (value) {
      this.barTooltipTemplate = null;
    }
    return value;
  }
  changeBarTooltip(tooltip, oldTooltip) {
    oldTooltip === null || oldTooltip === void 0 ? void 0 : oldTooltip.destroy();
    if (tooltip) {
      return tooltip.isTooltip ? tooltip : this.barTooltipClass.new({
        forElement: this.timeAxisSubGridElement,
        forSelector: '.b-histogram rect',
        hoverDelay: 0,
        trackMouse: false,
        cls: 'b-celltooltip-tip',
        getHtml: this.getTipHtml.bind(this)
      }, this.showBarTip, tooltip);
    }
    return null;
  }
  onRepopulateReplica() {
    this.ticksIdentifier = null;
    this.allocationReportByRecord.clear();
    this.allocationDataByRecord.clear();
    this.allocationObserverByRecord.clear();
  }
  getTipHtml(args) {
    var _this$barTooltipTempl;
    const {
        activeTarget
      } = args,
      index = activeTarget.dataset.index,
      record = this.getRecordFromElement(activeTarget),
      histogramData = this.allocationDataByRecord.get(record);
    args = Object.assign({}, args);
    args.index = parseInt(index, 10);
    args.datum = histogramData[args.index];
    return (_this$barTooltipTempl = this.barTooltipTemplate) === null || _this$barTooltipTempl === void 0 ? void 0 : _this$barTooltipTempl.call(this, args);
  }
  buildTicksIdentifier() {
    const me = this,
      graph = me.project.getGraph();
    if (!me.ticksIdentifier) {
      me.ticksIdentifier = graph.addIdentifier(CalculatedValueGen.new());
    }
    me.ticksIdentifier.writeToGraph(graph, new BaseCalendarMixin({
      unspecifiedTimeIsWorking: false,
      intervals: me.timeAxis.ticks.map(tick => {
        return {
          startDate: tick.startDate,
          endDate: tick.endDate,
          isWorking: true
        };
      })
    }));
    // process ticks to detect if their widths are monotonous
    // or some tick has a different width value
    me.collectTicksWidth();
    return me.ticksIdentifier;
  }
  collectTicksWidth() {
    const {
        ticks
      } = this.timeAxis,
      prevDuration = ticks[0].endDate - ticks[0].startDate,
      tickDurations = {
        0: prevDuration
      };
    let totalDuration = prevDuration,
      isMonotonous = true;
    for (let i = 1, {
        length
      } = ticks; i < length; i++) {
      const tick = ticks[i],
        duration = tick.endDate - tick.startDate;
      // the ticks width is different -> reset isMonotonous flag
      if (prevDuration !== duration) {
        isMonotonous = false;
      }
      totalDuration += duration;
      tickDurations[i] = duration;
    }
    // if the ticks widths are not monotonous we need to calculate
    // each bar width to provide it to the histogram widget later
    if (!isMonotonous) {
      const ticksWidth = {};
      for (let i = 0, {
          length
        } = ticks; i < length; i++) {
        ticksWidth[i] = tickDurations[i] / totalDuration;
      }
      this.ticksWidth = ticksWidth;
    } else {
      this.ticksWidth = null;
    }
  }
  onProjectDelayCalculationStart() {
    this.suspendRefresh();
  }
  onProjectDelayCalculationEnd() {
    this.resumeRefresh(true);
  }
  projectUnitsHasChanged() {
    const {
      project
    } = this;
    return project.daysPerMonth !== this._projectDaysPerMonth || project.daysPerWeek !== this._projectDaysPerWeek || project.hoursPerDay !== this._projectHoursPerDay;
  }
  internalOnProjectRefresh({
    source,
    isCalculated
  }) {
    if (isCalculated) {
      const me = this;
      if (!me.ticksIdentifier) {
        me.onTimeAxisEndReconfigure();
      }
      // if project units has changed and we use them for scale points
      if (me.useProjectTimeUnitsForScale && me.projectUnitsHasChanged()) {
        me._projectDaysPerMonth = source.daysPerMonth;
        me._projectDaysPerWeek = source.daysPerWeek;
        me._projectHoursPerDay = source.hoursPerDay;
        // regenerate scale points
        const eventParams = {
          scalePoints: me.generateScalePoints()
        };
        me.trigger('generateScalePoints', eventParams);
        // allow to override the points in a listener
        me._generatedScalePoints = me.scalePoints = eventParams.scalePoints;
      }
    }
  }
  relayStoreDataChange(event) {
    super.relayStoreDataChange(event);
    if (this.store.count === 0) {
      // To clear histogram when no rows to refresh
      this.histogramWidget.data = [];
      this.histogramWidget.refresh();
    }
  }
  get columns() {
    return super.columns;
  }
  set columns(columns) {
    const me = this;
    super.columns = columns;
    if (!me.isDestroying) {
      me.timeAxisColumn.renderer = me.renderResourceHistogram;
      me.timeAxisColumn.cellCls = me.timeAxisColumnCellCls;
      // Unless provided from outside, insert the scale column in the correct place
      if (!columns.some(col => col.type === 'scale')) {
        me.insertScaleColumn();
      }
    }
  }
  insertScaleColumn() {
    this.columns.rootNode.insertChild({
      type: 'scale'
    }, this.timeAxisColumn);
  }
  buildHistogramWidget(config) {
    var _me$timeAxisColumn;
    const me = this;
    if (me.getBarTextRenderData && !config.getBarTextRenderData) {
      config.getBarTextRenderData = me.getBarTextRenderData;
    }
    const histogramWidget = me.histogramWidgetClass.new({
      owner: me,
      appendTo: me.element,
      cls: 'b-hide-offscreen b-resourcehistogram-histogram',
      height: me.rowHeight,
      width: ((_me$timeAxisColumn = me.timeAxisColumn) === null || _me$timeAxisColumn === void 0 ? void 0 : _me$timeAxisColumn.width) || 0,
      omitZeroHeightBars: true,
      data: [],
      getBarTip: !me.barTooltipTemplate && me.getBarTip || emptyFn,
      getRectClass: me.getRectClass || me.getRectClassDefault,
      getBarText: me.getBarText || me.getBarTextDefault,
      series: me.series
    }, me.histogramWidgetConfig, config);
    me.getBarTextDefault.bind(histogramWidget);
    return histogramWidget;
  }
  get histogramWidget() {
    const me = this;
    if (!me._histogramWidget) {
      const series = me.series;
      if (!me.showMaxEffort && series.maxEffort) {
        series.maxEffort = false;
      }
      me._histogramWidget = me.buildHistogramWidget();
    }
    return me._histogramWidget;
  }
  // Injectable method.
  getRectClassDefault(series, rectConfig, datum) {
    if (series.id === 'effort') {
      switch (true) {
        case datum.isOverallocated:
          return 'b-overallocated';
        case datum.isUnderallocated:
          return 'b-underallocated';
      }
    }
    return '';
  }
  get effortFormatter() {
    const me = this,
      format = me.effortFormat;
    let formatter = me._effortFormatter;
    if (!formatter || me._effortFormat !== format) {
      formatter = NumberFormat.get(me._lastFormat = format);
      me._effortFormatter = formatter;
    }
    return formatter;
  }
  /**
   * Formats effort value to display in the component bars and tooltips.
   * @param {Number} effort Effort value
   * @param {'millisecond'|'second'|'minute'|'hour'|'day'|'week'|'month'|'quarter'|'year'} unit Effort value unit
   * @param {Boolean} [showEffortUnit=this.showEffortUnit] Provide `true` to include effort unit. If not provided
   * uses {@link #config-showEffortUnit} value.
   * @returns {String} Formatted effort value.
   */
  getEffortText(effort, unit, showEffortUnit = this.showEffortUnit) {
    var _this$project3;
    // bail out if there is no project or it's not in the graph
    if (!((_this$project3 = this.project) !== null && _this$project3 !== void 0 && _this$project3.graph)) {
      return;
    }
    const {
      scaleUnit,
      effortFormatter
    } = this;
    unit = unit || scaleUnit;
    const localizedUnit = DateHelper.getShortNameOfUnit(unit),
      effortInUnits = this.convertEffortUnit(effort, TimeUnit.Millisecond, unit);
    return effortFormatter.format(effortInUnits) + (showEffortUnit ? localizedUnit : '');
  }
  getBarTipEffortUnit() {
    const {
        effortUnit,
        barTipEffortUnit,
        timeAxis
      } = this,
      defaultUnit = barTipEffortUnit || effortUnit;
    return DateHelper.compareUnits(timeAxis.unit, defaultUnit) < 0 ? timeAxis.unit : defaultUnit;
  }
  getGroupBarTip({
    datum
  }) {
    const me = this,
      {
        showBarTip,
        timeAxis
      } = me;
    let result = '';
    if (showBarTip && datum.effort) {
      const unit = me.getBarTipEffortUnit(...arguments),
        allocated = me.getEffortText(datum.effort, unit),
        available = me.getEffortText(datum.maxEffort, unit),
        assignmentTpl = me.L('L{groupBarTipAssignment}');
      let dateFormat = 'L',
        resultFormat = me.L('L{groupBarTipInRange}'),
        assignmentsSuffix = '';
      if (DateHelper.compareUnits(timeAxis.unit, TimeUnit.Day) === 0) {
        resultFormat = me.L('L{groupBarTipOnDate}');
      } else if (DateHelper.compareUnits(timeAxis.unit, TimeUnit.Second) <= 0) {
        dateFormat = 'HH:mm:ss A';
      } else if (DateHelper.compareUnits(timeAxis.unit, TimeUnit.Hour) <= 0) {
        dateFormat = 'LT';
      }
      let assignmentsArray = [...datum.resourceAllocation.entries()].filter(([resource, data]) => data.effort).sort(([key1, value1], [key2, value2]) => value1.effort > value2.effort ? -1 : 1);
      if (assignmentsArray.length > me.groupBarTipAssignmentLimit) {
        assignmentsSuffix = '<br>' + me.L('L{plusMore}').replace('{value}', assignmentsArray.length - me.groupBarTipAssignmentLimit);
        assignmentsArray = assignmentsArray.slice(0, this.groupBarTipAssignmentLimit);
      }
      const assignments = assignmentsArray.map(([resource, info]) => {
        return assignmentTpl.replace('{resource}', StringHelper.encodeHtml(resource.name)).replace('{allocated}', me.getEffortText(info.effort, unit)).replace('{available}', me.getEffortText(info.maxEffort, unit)).replace('{cls}', info.isOverallocated ? 'b-overallocated' : info.isUnderallocated ? 'b-underallocated' : '');
      }).join('<br>') + assignmentsSuffix;
      result = resultFormat.replace('{assignments}', assignments).replace('{startDate}', DateHelper.format(datum.tick.startDate, dateFormat)).replace('{endDate}', DateHelper.format(datum.tick.endDate, dateFormat)).replace('{allocated}', allocated).replace('{available}', available).replace('{cls}', datum.isOverallocated ? 'b-overallocated' : datum.isUnderallocated ? 'b-underallocated' : '');
      result = `<div class="b-histogram-bar-tooltip">${result}</div>`;
    }
    return result;
  }
  getResourceBarTip({
    datum
  }) {
    const me = this,
      {
        showBarTip,
        timeAxis
      } = me;
    let result = '';
    if (showBarTip && datum.effort) {
      const unit = me.getBarTipEffortUnit(),
        allocated = me.getEffortText(datum.effort, unit),
        available = me.getEffortText(datum.maxEffort, unit);
      let dateFormat = 'L',
        resultFormat = me.L('L{barTipInRange}');
      if (DateHelper.compareUnits(timeAxis.unit, TimeUnit.Day) === 0) {
        resultFormat = me.L('L{barTipOnDate}');
      } else if (DateHelper.compareUnits(timeAxis.unit, TimeUnit.Second) <= 0) {
        dateFormat = 'HH:mm:ss A';
      } else if (DateHelper.compareUnits(timeAxis.unit, TimeUnit.Hour) <= 0) {
        dateFormat = 'LT';
      }
      result = resultFormat.replace('{startDate}', DateHelper.format(datum.tick.startDate, dateFormat)).replace('{endDate}', DateHelper.format(datum.tick.endDate, dateFormat)).replace('{allocated}', allocated).replace('{available}', available).replace('{cls}', datum.isOverallocated ? 'b-overallocated' : datum.isUnderallocated ? 'b-underallocated' : '');
      if (datum.resource) {
        result = result.replace('{resource}', StringHelper.encodeHtml(datum.resource.name));
      }
      result = `<div class="b-histogram-bar-tooltip">${result}</div>`;
    }
    return result;
  }
  /**
   * Returns unit to display effort values in when rendering the histogram bars.
   * The method by default returns {@link #config-barTextEffortUnit} value if provided
   * and if not falls back to {@link #config-effortUnit} value.
   * But it also takes zooming into account and when
   * the timeaxis ticks unit gets smaller than the default value the ticks unit is returned.
   *
   * @returns {'millisecond'|'second'|'minute'|'hour'|'day'|'week'|'month'|'quarter'|'year'} Time unit to display
   * effort values in.
   */
  getBarTextEffortUnit() {
    const {
        effortUnit,
        barTextEffortUnit,
        timeAxis
      } = this,
      defaultUnit = barTextEffortUnit || effortUnit;
    return DateHelper.compareUnits(timeAxis.unit, defaultUnit) < 0 ? timeAxis.unit : defaultUnit;
  }
  /**
   * The default method that returns the text to render inside a bar if no
   * {@link #config-getBarText} function was provided.
   *
   * The method can be used in a {@link #config-getBarText} function
   * to invoke the default implementation:
   *
   * ```javascript
   * new ResourceHistogram({
   *     getBarText(datum) {
   *         const resourceHistogram = this.owner;
   *
   *         // get default bar text
   *         let result = resourceHistogram.getBarTextDefault();
   *
   *         // if the resource is overallocated in that tick display "Overallocated! " string
   *         // before the allocationvalue
   *         if (result && datum.maxEffort < datum.effort) {
   *             result = 'Overallocated! ' + result;
   *         }
   *
   *         return result;
   *     },
   * })
   * ```
   * The following parameters are passed:
   * @param {ResourceAllocationIntervalInfo} datum The data of the bar being rendered
   * @param {Number} index The index of the datum being rendered
   * @returns {String} Tdxt to render inside the bar
   */
  getBarTextDefault(datum, index) {
    const {
      showBarText
    } = this.owner;
    let result = '';
    if (showBarText && datum.effort) {
      const unit = this.owner.getBarTextEffortUnit();
      result = this.owner.getEffortText(datum.effort, unit);
    }
    return result;
  }
  updateShowBarText(value) {
    this.scheduleRefreshRows();
  }
  updateShowBarTip(value) {
    this.barTooltip = value;
  }
  updateShowMaxEffort(value) {
    const me = this;
    me._showMaxEffort = value;
    let needsRefresh = false;
    [me._histogramWidget, me._groupHistogramWidget].forEach(widget => {
      // bail out in case there is no widget constructed yet
      if (!widget) {
        return;
      }
      const {
        series
      } = widget;
      if (!value) {
        if (series.maxEffort) {
          widget._seriesMaxEffort = series.maxEffort;
          delete series.maxEffort;
        }
      } else if (typeof value === 'object') {
        series.maxEffort = value;
      } else if (typeof widget._seriesMaxEffort === 'object') {
        series.maxEffort = widget._seriesMaxEffort;
      } else {
        series.maxEffort = {
          type: 'outline',
          field: 'maxEffort'
        };
        series.maxEffort.id = 'maxEffort';
      }
      needsRefresh = true;
    });
    if (needsRefresh) {
      me.scheduleRefreshRows();
    }
  }
  updateIncludeInactiveEvents(value) {
    // update collected reports wih new includeInactiveEvents flag state
    this.allocationReportByRecord.forEach(allocationReport => allocationReport.includeInactiveEvents = value);
  }
  //endregion
  //region Events
  onTimeAxisEndReconfigureInternal() {
    const me = this;
    // Skip call triggered by viewPreset setting we have `updateViewPreset` method overridden where we call `onTimeAxisEndReconfigure` later
    if (!me._updatingViewPreset) {
      const {
        unit,
        increment
      } = me.timeAxis;
      // re-generate scale point on zooming in/out
      if (unit !== me._lastTimeAxisUnit || increment !== me._lastTimeAxisIncrement) {
        // remember last used unit & increment to distinguish zooming from timespan changes
        me._lastTimeAxisUnit = unit;
        me._lastTimeAxisIncrement = increment;
        // regenerate scale points
        const scalePoints = me.generateScalePoints(),
          eventParams = {
            scalePoints
          };
        // allow to override the points in a listener
        me.trigger('generateScalePoints', eventParams);
        me._generatedScalePoints = me.scalePoints = eventParams.scalePoints;
      }
      me.buildTicksIdentifier();
    }
  }
  onTimeAxisEndReconfigure() {
    const me = this;
    // Skip call triggered by viewPreset setting we have `updateViewPreset` method overridden where we call `onTimeAxisEndReconfigure` later
    if (!me._updatingViewPreset) {
      if (me.project.graph) {
        me.onTimeAxisEndReconfigureInternal();
      }
      // In delayed calculation mode (the default) we might not be in graph yet, postpone buildTicksIdentifier until we are
      else {
        me.project.ion({
          graphReady() {
            me.onTimeAxisEndReconfigureInternal();
          },
          thisObj: me,
          once: true
        });
      }
    }
  }
  onBeforeRowHeight({
    height
  }) {
    if (this._timeAxisColumn) {
      for (const widget of [this._histogramWidget, this._groupHistogramWidget]) {
        if (!widget) continue;
        widget.height = height;
        widget.onElementResize(widget.element);
      }
    }
  }
  onTimeAxisViewModelUpdate() {
    super.onTimeAxisViewModelUpdate(...arguments);
    for (const widget of [this._histogramWidget, this._groupHistogramWidget]) {
      if (!widget) continue;
      widget.width = this.timeAxisViewModel.totalSize;
      widget.onElementResize(widget.element);
    }
  }
  //endregion
  //region Render
  getRecordAllocationInfoRenderData(record, allocation, cellElement, histogramWidget = null) {
    allocation = Array.isArray(allocation) ? allocation : allocation.total;
    // if ticks widths are not monotonous
    // we provide width for each bar since in that case the histogram widget won't be able to calculate widths properly
    if (this.ticksWidth) {
      for (let i = 0, {
          length
        } = allocation; i < length; i++) {
        allocation[i].width = this.ticksWidth[i];
      }
    }
    return allocation;
  }
  renderRecordAllocationInfo(record, allocation, cellElement, histogramWidget = null) {
    // histogram pattern
    histogramWidget = histogramWidget || this.histogramWidget;
    const data = this.getRecordAllocationInfoRenderData(record, allocation, cellElement, histogramWidget);
    // skip render attempts if allocation is not collected yet
    if (!data) {
      return;
    }
    this.allocationDataByRecord.set(record, data);
    histogramWidget.data = data;
    histogramWidget.refresh();
    const histogramCloneElement = histogramWidget.element.cloneNode(true);
    histogramCloneElement.removeAttribute('id');
    histogramCloneElement.classList.remove('b-hide-offscreen');
    cellElement.innerHTML = '';
    cellElement.appendChild(histogramCloneElement);
  }
  renderRows() {
    const me = this;
    if (!me.ticksIdentifier && me.project.isInitialCommitPerformed) {
      // If we render rows but have no ticksIdentifier means data loading and 1st commit
      // happened before the histogram was created.
      // Handle timeaxis settings to build ticksIdentifier and scale column points.
      me.onTimeAxisEndReconfigure();
      // If timeView range is not defined then the timeaxis header looks empty so fill it in here (it triggers the column refresh)
      if (!me.timeView.startDate || !me.timeView.endDate) {
        me.timeView.range = {
          startDate: me.startDate,
          endDate: me.endDate
        };
      }
    }
    return super.renderRows(...arguments);
  }
  onRecordAllocationCalculated(record, allocation, allocationReport) {
    const me = this;
    if (!me.isDestroying) {
      const cell = me.getCell({
        record,
        columnId: me.timeAxisColumn.id
      });
      if (cell) {
        me.renderRecordAllocationInfo(record, allocation, cell);
      }
      // announce resource allocation got calculated
      me.trigger('allocationChange', {
        record,
        allocation
      });
      const groupParent = me.getResourceGroupParent(record);
      if (groupParent) {
        // reset cached allocation for the resource group
        me.resourceGroupsAllocation.delete(groupParent);
        // schedule updating of resource group histograms
        me.scheduleGroupRender(groupParent);
      }
    }
  }
  buildResourceAllocationReport(resource) {
    return this.project.resourceAllocationInfoClass.new({
      includeInactiveEvents: this.includeInactiveEvents,
      ticks: this.ticksIdentifier,
      resource
    });
  }
  registerRecordAllocationReport(record) {
    const me = this,
      graph = me.project.getGraph(),
      allocationReport = me.buildResourceAllocationReport(record);
    // store resource allocation report reference
    me.allocationReportByRecord.set(record, allocationReport);
    record.addEntity(allocationReport);
    // track allocation report changes
    const allocationObserver = graph.observe(function* () {
      return yield allocationReport.$.allocation;
    }, allocation => me.onRecordAllocationCalculated(record, allocation, allocationReport));
    me.allocationObserverByRecord.set(record, allocationObserver);
    // trigger rendering on allocation report changes
    record.addObserver(allocationObserver);
    return allocationReport;
  }
  renderResourceHistogram({
    grid: me,
    cellElement,
    record
  }) {
    const {
      project
    } = me;
    // No drawing before engine's initial commit
    // Skip special rows, e.g. group records
    if (me.ticksIdentifier && project.isInitialCommitPerformed && !record.isSpecialRow) {
      var _allocationReport;
      const {
        allocationReportByRecord,
        allocationObserverByRecord
      } = me;
      let allocationReport = allocationReportByRecord.get(record);
      // If we have no allocation report built for the resource yet
      // let's initialize it here
      if (!allocationReport) {
        allocationReport = me.registerRecordAllocationReport(record);
      }
      // rendering was triggered by not allocation report change so we render based on existing "resource.allocation"
      if ((_allocationReport = allocationReport) !== null && _allocationReport !== void 0 && _allocationReport.allocation) {
        if (allocationReport.graph) {
          me.renderRecordAllocationInfo(record, allocationReport.allocation, cellElement);
        }
        // allocation data had left the graph probably after the resource was removed
        else {
          allocationReportByRecord.delete(record);
          me.allocationDataByRecord.delete(record);
          allocationObserverByRecord.delete(record);
        }
        const groupParent = me.getResourceGroupParent(record);
        // if grouped - schedule updating of the resource group histograms
        if (groupParent && me.store.includes(groupParent)) {
          me.scheduleGroupRender(groupParent);
        }
      }
    }
  }
  renderScheduledGroups() {
    // Clone set to avoid infinite cycle when we add new entry to this.resourceGroupsToUpdate
    // in this.renderGroupHistogram() call
    for (const groupParent of Array.from(this.resourceGroupsToUpdate)) {
      this.renderGroupHistogram(groupParent);
    }
    this.clearTimeout(this.renderScheduledGroupTimer);
  }
  scheduleGroupRender(groupParent) {
    this.resourceGroupsToUpdate.add(groupParent);
    this.renderScheduledGroupTimer = this.setTimeout({
      fn: 'renderScheduledGroups',
      delay: 10,
      cancelOutstanding: true
    });
  }
  getResourceGroupParent(resource) {
    const instanceMeta = resource.instanceMeta(this.project.resourceStore.id);
    return instanceMeta === null || instanceMeta === void 0 ? void 0 : instanceMeta.groupParent;
  }
  calculateResourceGroupAllocation(groupParent) {
    var _allocationReports$, _allocationReports$$a;
    const me = this,
      {
        allocationReportByRecord
      } = me,
      {
        groupChildren
      } = groupParent,
      allocationReports = groupChildren.map(resource => allocationReportByRecord.get(resource)),
      newAllocation = (_allocationReports$ = allocationReports[0]) === null || _allocationReports$ === void 0 ? void 0 : (_allocationReports$$a = _allocationReports$.allocation) === null || _allocationReports$$a === void 0 ? void 0 : _allocationReports$$a.total,
      newAllocationLength = newAllocation === null || newAllocation === void 0 ? void 0 : newAllocation.length;
    // All child resource allocations are calculated (their lengths should be equal)
    if (newAllocation && allocationReports.every(allocationInfo => (allocationInfo === null || allocationInfo === void 0 ? void 0 : allocationInfo.allocation) && allocationInfo.allocation.total.length === newAllocationLength)) {
      const combinedAllocation = [];
      // Iterate over the group resources
      // and aggregate resource allocations to show the group level histogram
      allocationReports.forEach(({
        allocation
      }) => {
        // iterate over ticks
        allocation.total.forEach((a, index) => {
          let combined = combinedAllocation[index];
          if (!combined) {
            combined = combinedAllocation[index] = {
              tick: a.tick,
              effort: 0,
              maxEffort: 0,
              units: 0,
              isGroup: true,
              resourceAllocation: new Map()
            };
          }
          combined.resourceAllocation.set(a.resource, {
            effort: a.effort,
            maxEffort: a.maxEffort,
            units: a.units,
            isOverallocated: a.effort > a.maxEffort,
            isUnderallocated: a.effort < a.maxEffort
          });
          combined.isOverallocated = combined.isOverallocated || a.isOverallocated;
          combined.isUnderallocated = combined.isUnderallocated || a.isUnderallocated;
          combined.effort += a.effort;
          combined.maxEffort += a.maxEffort;
          if (a.assignments) {
            if (combined.assignments) {
              a.assignments.forEach(assignment => combined.assignments.add(assignment));
            } else {
              combined.assignments = new Set(a.assignments);
            }
          }
        });
      });
      return combinedAllocation;
    }
  }
  renderGroupHistogram(groupParent) {
    const me = this;
    me.resourceGroupsToUpdate.delete(groupParent);
    // if the group is not in the store
    if (!me.store.includes(groupParent)) {
      me.resourceGroupsAllocation.delete(groupParent);
    }
    const combinedAllocation = me.resourceGroupsAllocation.get(groupParent) || me.calculateResourceGroupAllocation(groupParent);
    if (combinedAllocation) {
      var _me$scaleColumn;
      // cache calculated allocation
      me.resourceGroupsAllocation.set(groupParent, combinedAllocation);
      const {
        groupChildren
      } = groupParent;
      let scalePoints = me.generateScalePoints(me.timeAxis.increment * groupChildren.length);
      const eventParams = {
        scalePoints,
        groupParent,
        isCalculatingTopValue: true
      };
      me.trigger('generateScalePoints', eventParams);
      scalePoints = eventParams.scalePoints;
      const lastPoint = scalePoints[scalePoints.length - 1],
        scaleMax = me.useProjectTimeUnitsForScale ? me.convertEffortUnit(lastPoint.value, lastPoint.unit, TimeUnit.Millisecond) : DateHelper.asMilliseconds(lastPoint.value, lastPoint.unit),
        topValue = scaleMax + (((_me$scaleColumn = me.scaleColumn) === null || _me$scaleColumn === void 0 ? void 0 : _me$scaleColumn.scaleWidget.scaleMaxPadding) || 0) * scaleMax,
        widget = me._groupHistogramWidget || me.buildHistogramWidget({
          topValue
        }),
        cellElement = me.getCell({
          id: groupParent.id,
          columnId: me.timeAxisColumn.id
        });
      // if we have group level histogram widget cached - update its topValue
      if (me._groupHistogramWidget) {
        widget.topValue = topValue;
      }
      // cache constructed histogram widget
      else {
        me._groupHistogramWidget = widget;
      }
      // render the group histogram
      if (cellElement) {
        me.renderRecordAllocationInfo(groupParent, combinedAllocation, cellElement, widget);
        me.trigger('groupRendered', {
          groupParent
        });
      }
    }
    // if some allocations are not recalculated yet - reschedule this group update
    else if (me.store.includes(groupParent)) {
      me.scheduleGroupRender(groupParent);
    }
  }
  renderGroupScale(groupParent) {
    const me = this,
      {
        scaleColumn
      } = me;
    // Render scale only if scale column is there
    if (scaleColumn) {
      const {
          groupChildren
        } = groupParent,
        scalePoints = me.generateScalePoints(me.timeAxis.increment * groupChildren.length),
        cellElement = me.getCell({
          id: groupParent.id,
          columnId: scaleColumn.id
        }),
        eventParams = {
          scalePoints,
          groupParent
        };
      if (!cellElement) {
        return;
      }
      let scaleWidget = me._groupScaleWidget;
      if (!scaleWidget) {
        scaleWidget = me._groupScaleWidget = scaleColumn.buildScaleWidget();
      }
      me.trigger('generateScalePoints', eventParams);
      scaleWidget.scalePoints = eventParams.scalePoints;
      return scaleColumn.renderer({
        cellElement,
        scaleWidget
      });
    }
  }
  //endregion
  //region Localization
  updateLocalization() {
    const me = this;
    // Translate scale points if we have them (update localization on construction step is called too early)
    // and the scale points is generated by the histogram which means their labels use localized unit abbreviations
    if (me._generatedScalePoints === me.scalePoints && me.scalePoints) {
      me.scalePoints.forEach(scalePoint => {
        // if the point is labeled let's rebuild its text using new locale
        if (scalePoint.text && scalePoint.unit) {
          scalePoint.text = me.buildScalePointText(scalePoint);
        }
      });
    }
    super.updateLocalization(...arguments);
  }
  //endregion
}

ResourceHistogram.initClass();
ResourceHistogram._$name = 'ResourceHistogram';

/**
 * @module SchedulerPro/model/ResourceUtilizationModel
 */
/**
 * A model representing a {@link SchedulerPro/view/ResourceUtilization} view row.
 * The view rows are of two possible types __resources__ and __assignments__.
 * The model wraps either a resource or an assignment model. And each wrapped resource keeps its corresponding
 * wrapped assignments as its __children__.
 *
 * **NOTE:** You don't normally need to construct this class instances. The view does that automatically
 * by processing the project resources and assignments, wrapping them with this model instances and
 * putting them to its {@link SchedulerPro/view/ResourceUtilization#property-store}.
 *
 * The wrapped model is provided to {@link #config-origin} config and can be retrieved from it:
 *
 * ```javascript
 * // get the real resource representing the first row of the view
 * resourceUtilizationView.store.first.origin
 * ```
 *
 * @extends Core/data/Model
 */
class ResourceUtilizationModel extends Model {
  static $name = 'ResourceUtilizationModel';
  static fields = [
  /**
   * Name of the represented resource or the assigned event.
   * If the model represents an assignment the field value is
   * automatically set to the assigned event {@link SchedulerPro/model/EventModel#field-name}.
   * @field {String} name
   * @category Common
   */
  'name',
  /**
   * Icon for the corresponding row.
   * If the model represents an assignment the field value is
   * automatically set to the assigned event {@link SchedulerPro/model/EventModel#field-iconCls}.
   * @field {String} iconCls
   * @category Styling
   */
  'iconCls'];
  /**
   * A resource or an assignment wrapped by this model.
   *
   * ```javascript
   * // get the real resource representing the first row of the view
   * resourceUtilizationView.store.first.origin
   * ```
   * @config {SchedulerPro.model.ResourceModel|SchedulerPro.model.AssignmentModel} origin
   */
  construct(data, ...args) {
    this._childrenIndex = new Map();
    // copy some field values from origin to this model
    if (data.origin) {
      Object.assign(data, this.mapOriginValues(data.origin));
    }
    super.construct(data, ...args);
    if (this.origin) {
      this.fillChildren();
    }
  }
  mapOriginValues(origin) {
    const result = {};
    if (origin.isResourceModel) {
      result.name = origin.name;
    } else if (origin.isAssignmentModel) {
      var _origin$event, _origin$event2;
      result.name = (_origin$event = origin.event) === null || _origin$event === void 0 ? void 0 : _origin$event.name;
      result.iconCls = (_origin$event2 = origin.event) === null || _origin$event2 === void 0 ? void 0 : _origin$event2.iconCls;
    }
    return result;
  }
  fillChildren() {
    var _me$origin;
    const me = this,
      {
        children
      } = me,
      toRemove = new Set(children),
      toAdd = [];
    if ((_me$origin = me.origin) !== null && _me$origin !== void 0 && _me$origin.isResourceModel) {
      const {
        assigned
      } = me.origin;
      for (const assignment of assigned) {
        if (!me._childrenIndex.has(assignment)) {
          toAdd.push(me.constructor.new({
            origin: assignment
          }));
        } else {
          toRemove.delete(me._childrenIndex.get(assignment));
        }
      }
    }
    if (toRemove.size) {
      this.removeChild([...toRemove]);
    }
    if (toAdd.length) {
      this.appendChild(toAdd);
    }
  }
  afterRemoveChild(records) {
    records.forEach(record => this._childrenIndex.delete(record.origin));
  }
  insertChild(...args) {
    let added = super.insertChild(...args);
    if (added) {
      var _this$origin;
      const {
        stores
      } = this;
      if (!Array.isArray(added)) {
        added = [added];
      }
      if ((_this$origin = this.origin) !== null && _this$origin !== void 0 && _this$origin.isResourceModel) {
        for (const record of added) {
          if (record.origin && !this._childrenIndex.has(record.origin)) {
            this._childrenIndex.set(record.origin, record);
          }
        }
      }
      // if the model is already in a store
      // fill the store real_model -> wrapper_model map
      if (stores !== null && stores !== void 0 && stores.length) {
        for (const store of stores) {
          for (const record of added) {
            record.traverse(node => store.setModelByOrigin(node.origin, node));
          }
        }
      }
    }
    return added;
  }
  getChildByOrigin(origin) {
    return this._childrenIndex.get(origin);
  }
}
ResourceUtilizationModel.exposeProperties();
ResourceUtilizationModel._$name = 'ResourceUtilizationModel';

/**
 * @module SchedulerPro/data/ResourceUtilizationStore
 */
/**
 * A store representing {@link SchedulerPro/view/ResourceUtilization} view records.
 * This store accepts a model class inheriting from {@link SchedulerPro/model/ResourceUtilizationModel}.
 *
 * The store is a tree of nodes representing resources on the root level with
 * sub-nodes representing corresponding resource assignments.
 * The store tracks changes made in the {@link #config-project} stores and rebuilds its content automatically.
 * Thus the project config is mandatory and has to be provided.
 *
 * @extends Core/data/AjaxStore
 */
class ResourceUtilizationStore extends AbstractPartOfProjectStoreMixin.derive(AjaxStore) {
  static configurable = {
    modelClass: ResourceUtilizationModel,
    /**
     * Project instance to retrieve resources and assignments data from.
     * @config {SchedulerPro.model.ProjectModel} project
     */
    project: null,
    tree: true
  };
  // Cannot use `static properties = {}`, new Map would pollute the prototype
  static get properties() {
    return {
      _modelByOrigin: new Map()
    };
  }
  updateProject(project) {
    this.setResourceStore(project === null || project === void 0 ? void 0 : project.resourceStore);
    this.setAssignmentStore(project === null || project === void 0 ? void 0 : project.assignmentStore);
    this.setEventStore(project === null || project === void 0 ? void 0 : project.eventStore);
    this.fillStoreFromProject();
  }
  setResourceStore(store) {
    this.detachListeners('resourceStore');
    store === null || store === void 0 ? void 0 : store.ion({
      name: 'resourceStore',
      change: this.onResourceStoreDataChanged,
      thisObj: this
    });
  }
  setEventStore(store) {
    this.detachListeners('eventStore');
    store === null || store === void 0 ? void 0 : store.ion({
      name: 'eventStore',
      update: this.onEventUpdate,
      thisObj: this
    });
  }
  setAssignmentStore(store) {
    this.detachListeners('assignmentStore');
    store === null || store === void 0 ? void 0 : store.ion({
      name: 'assignmentStore',
      change: this.onAssignmentsChange,
      refresh: this.onAssignmentsRefresh,
      add: this.onAssignmentsAdd,
      update: this.onAssignmentUpdate,
      remove: this.onAssignmentsRemove,
      thisObj: this
    });
  }
  onResourceStoreDataChanged(event) {
    // 'move' action triggers a remove event first, we wait for the 'add' - no need to fill twice
    if (event.isMove && event.action === 'remove') {
      return;
    }
    this.fillStoreFromProject();
  }
  onAssignmentsChange() {
    this.forEach(resourceWrapper => resourceWrapper.fillChildren());
  }
  onAssignmentsRefresh(event) {
    if (event.action === 'batch') {
      this.forEach(resourceWrapper => resourceWrapper.fillChildren());
    }
  }
  onAssignmentsAdd({
    records
  }) {
    records.forEach(record => {
      const resourceWrapper = this.getModelByOrigin(record === null || record === void 0 ? void 0 : record.resource);
      resourceWrapper === null || resourceWrapper === void 0 ? void 0 : resourceWrapper.fillChildren();
    });
  }
  onAssignmentUpdate({
    record,
    changes
  }) {
    // if assignment moved to another resource
    if ('resource' in changes) {
      const
        // get assignment wrapper record
        assignmentWrapper = this.getModelByOrigin(record),
        // get new resource wrapper record
        newResourceWrapper = this.getModelByOrigin(record === null || record === void 0 ? void 0 : record.resource);
      // move assignment wrapper to new resource wrapper
      if (assignmentWrapper && newResourceWrapper) {
        newResourceWrapper.appendChild(assignmentWrapper);
      }
    }
  }
  onAssignmentsRemove({
    records
  }) {
    this.remove(records.map(record => this.getModelByOrigin(record)));
  }
  onEventUpdate({
    record,
    changes
  }) {
    if ('name' in changes) {
      for (const assignment of record.assigned) {
        const assignmentWrapper = this.getModelByOrigin(assignment);
        assignmentWrapper.set('name', record.name);
      }
    }
  }
  fillStoreFromProject() {
    var _this$_project;
    const toAdd = [];
    (_this$_project = this._project) === null || _this$_project === void 0 ? void 0 : _this$_project.resourceStore.forEach(resource => {
      if (!resource.isSpecialRow) {
        toAdd.push(this.modelClass.new({
          origin: resource
        }));
      }
    });
    this.removeAll();
    this.add(toAdd);
    /**
     * Fires when store completes synchronization with original (Event/Resource/Assignment) stores
     * @event fillFromProject
     * @internal
     */
    this.trigger('fillFromProject');
  }
  remove() {
    const removed = super.remove(...arguments);
    // sanitize internal origin->wrapper Map
    removed === null || removed === void 0 ? void 0 : removed.forEach(record => {
      this._modelByOrigin.delete(record.origin);
    });
    return removed;
  }
  removeAll() {
    super.removeAll(...arguments);
    this._modelByOrigin.clear();
  }
  getModelByOrigin(origin) {
    return this._modelByOrigin.get(origin);
  }
  setModelByOrigin(origin, model) {
    return this._modelByOrigin.set(origin, model);
  }
}
ResourceUtilizationStore._$name = 'ResourceUtilizationStore';

/**
 * @module SchedulerPro/view/ResourceUtilization
 */
/**
 * An object containing info on the assignment effort in a certain time interval.
 *
 * The object is used when rendering interval bars and tooltips so it additionally provides a `rectConfig` property
 * which contains a configuration object for the`rect` SVG-element representing the interval bar.
 *
 * @typedef {Object} AssignmentAllocationIntervalInfo
 * @property {SchedulerPro.model.AssignmentModel} assignment The assignment which allocation is displayed.
 * @property {Number} effort Amount of work performed by the assigned resource in the interval
 * @property {TickInfo} tick The interval of time the allocation is collected for
 * @property {Number} units Assignment {@link SchedulerPro.model.AssignmentModel#field-units} value
 * @property {Object} rectConfig The rectangle DOM configuration object
 */
/**
 * Widget showing the utilization levels of the project resources.
 * The resources are displayed in a summary list where each row can
 * be expanded to show the events assigned for the resource.
 *
 * This demo shows the Resource utilization widget:
 * {@inlineexample SchedulerPro/view/ResourceUtilization.js}
 *
 * The view requires a {@link #config-project Project instance} to be provided:
 *
 * ```javascript
 * const project = new ProjectModel({
 *     autoLoad  : true,
 *     transport : {
 *         load : {
 *             url : 'examples/schedulerpro/view/data.json'
 *         }
 *     }
 * });
 *
 * const resourceUtilization = new ResourceUtilization({
 *     project,
 *     appendTo    : 'targetDiv',
 *     rowHeight   : 60,
 *     minHeight   : '20em',
 *     flex        : '1 1 50%',
 *     showBarTip  : true
 * });
 * ```
 *
 * ## Pairing the component
 *
 * You can also pair the view with other timeline views such as the Gantt or Scheduler,
 * using the {@link #config-partner} config.
 *
  * ## Changing displayed values
 *
 * To change the displayed bar texts, supply a {@link #config-getBarText} function.
 * Here for example the provided function displays resources time **left** instead of
 * allocated time
 *
 * ```javascript
 * new ResourceUtilization({
 *     getBarText(datum) {
 *         const view = this.owner;
 *
 *         // get default bar text
 *         let result = view.getBarTextDefault();
 *
 *         // For resource records we will display the time left for allocation
 *         if (result && datum.resource) {
 *
 *             const unit = view.getBarTextEffortUnit();
 *
 *             // display the resource available time
 *             result = view.getEffortText(datum.maxEffort - datum.effort, unit);
 *         }
 *
 *         return result;
 *     },
 * })
 * ```
 *
 * @extends SchedulerPro/view/ResourceHistogram
 * @classtype resourceutilization
 * @widget
 */
class ResourceUtilization extends ResourceHistogram {
  //region Config
  static $name = 'ResourceUtilization';
  static type = 'resourceutilization';
  static configurable = {
    /**
     * @hideconfigs crudManager, crudManagerClass, assignments, resources, events, dependencies, assignmentStore,
     * resourceStore, eventStore, dependencyStore, data, timeZone
     */
    /**
     * A Function which returns the text to render inside a bar.
     *
     * Here for example the provided function displays resources time **left** instead of
     * allocated time
     *
     * ```javascript
     * new ResourceUtilization({
     *     getBarText(datum) {
     *         const resourceUtilization = this.owner;
     *
     *         // get default bar text
     *         let result = view.getBarTextDefault();
     *
     *         // For resource records we will display the time left for allocation
     *         if (result && datum.resource) {
     *
     *             const unit = resourceUtilization.getBarTextEffortUnit();
     *
     *             // display the resource available time
     *             result = resourceUtilization.getEffortText(datum.maxEffort - datum.effort, unit);
     *         }
     *
     *         return result;
     *     },
     * })
     * ```
     *
     * **Please note** that the function will be injected into the underlying
     * {@link Core/widget/graph/Histogram} component that is used under the hood
     * to render actual charts.
     * So `this` in the function will refer to the {@link Core/widget/graph/Histogram} instance.
     * To access the `ResourceUtilization` instance please use `this.owner` in the function body:
     *
     * ```javascript
     * new ResourceUtilization({
     *     getBarText(datum) {
     *         // "this" in the method refers core Histogram instance
     *         // get the view instance
     *         const view = this.owner;
     *
     *         .....
     *     },
     * })
     * ```
     * The following parameters are passed:
     * @param {ResourceAllocationIntervalInfo|AssignmentAllocationIntervalInfo} datum The datum being rendered.
     * Either {@link SchedulerPro.view.ResourceHistogram#typedef-ResourceAllocationIntervalInfo} object for resource records (root level records)
     * or {@link #typedef-AssignmentAllocationIntervalInfo}object  for assignment records
     * @param {Number} index - The index of the datum being rendered
     * @returns {String} Tdxt to render inside the bar
     * @config {Function} getBarText
     */
    /* */
    timeAxisColumnCellCls: 'b-sch-timeaxis-cell b-resourceutilization-cell',
    /**
     * A ProjectModel instance (or a config object) to display resource allocation of.
     *
     * Note: This config is mandatory.
     * @config {ProjectModelConfig|SchedulerPro.model.ProjectModel} project
     */
    rowHeight: 30,
    showEffortUnit: false,
    /**
     * @config {Boolean} showMaxEffort
     * @hide
     */
    showMaxEffort: false,
    /**
     * Set to `true` if you want to display resources effort values in bars
     * (for example: `24h`, `7d`, `60min` etc.).
     * The text contents can be changed by providing {@link #config-getBarText} function.
     * @config {Boolean}
     * @default
     */
    showBarText: true,
    /**
     * A Function which returns the tooltip text to display when hovering a bar.
     * The following parameters are passed:
     * @param {Object} data - The backing data of the histogram rectangle
     * @param {Object} data.rectConfig - The rectangle configuration object
     * @param {Object} data.datum - The datum being rendered
     * @param {Number} data.index - The index of the datum being rendered
     * @config {Function}
     */
    barTooltipTemplate({
      effort,
      isGroup,
      resource,
      assignment
    }) {
      let result = '';
      // const barTip = this.callback('getBarTextTip', me, [renderData, data[index], index]);
      if (effort) {
        if (resource) {
          result = this.getResourceBarTip(...arguments);
        } else if (assignment) {
          result = this.getAssignmentBarTip(...arguments);
        } else if (isGroup) {
          result = this.getGroupBarTip(...arguments);
        }
      }
      return result;
    },
    series: {
      effort: {
        type: 'bar',
        field: 'effort'
      }
    },
    readOnly: true,
    columns: [{
      type: 'tree',
      field: 'name',
      text: 'L{nameColumnText}',
      localeClass: this
    }]
  };
  //endregion
  /**
   * @event generateScalePoints
   * @hide
   */
  /**
   * @function generateScalePoints
   * @hide
   */
  /**
   * @member {SchedulerPro.column.ScaleColumn} scaleColumn
   * @hide
   */
  updateProject(project) {
    super.updateProject(project);
    this.store = this.buildStore(project);
  }
  buildStore(project) {
    return ResourceUtilizationStore.new({
      project
    });
  }
  insertScaleColumn() {}
  //region Render
  getTipHtml({
    activeTarget
  }) {
    const index = activeTarget.dataset.index,
      record = this.getRecordFromElement(activeTarget),
      allocationData = this.allocationDataByRecord.get(record.origin),
      data = allocationData[parseInt(index, 10)];
    return this.barTooltipTemplate(data);
  }
  registerRecordAllocationReport(record) {
    if (record.isResourceModel) {
      return super.registerRecordAllocationReport(record);
    }
    if (record.isAssignmentModel) {
      return this.registerAssignmentAllocationReport(record);
    }
  }
  onDestroy() {
    const me = this;
    // destroy observers & entities made for assignments displayed by this view
    for (const [record, observer] of (_me$allocationObserve = me.allocationObserverByRecord) === null || _me$allocationObserve === void 0 ? void 0 : _me$allocationObserve.entries()) {
      var _me$allocationObserve;
      if (record.isAssignmentModel) {
        record.resource.removeObserver(observer);
        me.allocationObserverByRecord.delete(record);
      }
    }
    for (const [record, entity] of (_me$allocationReportB = me.allocationReportByRecord) === null || _me$allocationReportB === void 0 ? void 0 : _me$allocationReportB.entries()) {
      var _me$allocationReportB;
      if (record.isAssignmentModel) {
        record.resource.removeEntity(entity);
        me.allocationReportByRecord.delete(entity);
      }
    }
    if (me.destroyStores) {
      var _me$store;
      (_me$store = me.store) === null || _me$store === void 0 ? void 0 : _me$store.destroy();
    }
    super.onDestroy();
  }
  registerAssignmentAllocationReport(record) {
    const me = this,
      graph = me.project.getGraph(),
      allocationReport = me.allocationReportByRecord.get(record.resource);
    if (allocationReport) {
      // store resource allocation report reference
      me.allocationReportByRecord.set(record, allocationReport);
      // track allocation report changes
      const allocationObserver = graph.observe(function* () {
        return yield allocationReport.$.allocation;
      }, allocation => me.onRecordAllocationCalculated(record, allocation, allocationReport));
      me.allocationObserverByRecord.set(record, allocationObserver);
      // trigger rendering on allocation report changes
      record.resource.addObserver(allocationObserver);
    }
    return allocationReport;
  }
  onRecordAllocationCalculated(record, allocation, allocationReport) {
    // if that's an assignment row
    if (record.isAssignmentModel) {
      if (allocation.byAssignments.get(record)) {
        super.onRecordAllocationCalculated(record, allocation, allocationReport);
      }
      // If allocation report is calculated w/o the assignment
      // it means that the assignment was moved to another resource.
      // Then we drop linkage of that assignment to that allocation report
      else {
        // remove reference of the assignment to the allocation repoort
        this.allocationReportByRecord.delete(record);
        const observer = this.allocationObserverByRecord.get(record);
        // remove the allcation report observer that tracks changes and refreshes the assignment row
        if (observer) {
          allocationReport.resource.removeObserver(observer);
          this.allocationObserverByRecord.delete(record);
        }
      }
    } else {
      super.onRecordAllocationCalculated(record, allocation, allocationReport);
    }
  }
  onRowManagerRenderRow({
    row,
    record
  }) {
    var _record$origin, _record$origin2;
    // indicate row kinds
    row.assignCls({
      'b-resource-row': (_record$origin = record.origin) === null || _record$origin === void 0 ? void 0 : _record$origin.isResourceModel,
      'b-assignment-row': (_record$origin2 = record.origin) === null || _record$origin2 === void 0 ? void 0 : _record$origin2.isAssignmentModel
    });
  }
  getResourceGroupParent(resource) {
    const instanceMeta = resource.instanceMeta(this.store);
    return instanceMeta === null || instanceMeta === void 0 ? void 0 : instanceMeta.groupParent;
  }
  renderResourceHistogram(data) {
    const {
      project
    } = data.grid;
    if (project.isInitialCommitPerformed && !data.record.isSpecialRow) {
      // renderResourceHistogram() expects a real resource model
      // so get it from its wrapper model
      data.record = data.record.origin;
      return super.renderResourceHistogram(data);
    }
  }
  getCell(data) {
    var _data$record, _data$record2;
    // if real resource or assignment is provided
    if ((_data$record = data.record) !== null && _data$record !== void 0 && _data$record.isResourceModel || (_data$record2 = data.record) !== null && _data$record2 !== void 0 && _data$record2.isAssignmentModel) {
      // use its wrapper record to find proper cell
      data.record = this.store.getModelByOrigin(data.record);
    }
    return super.getCell(data);
  }
  buildHistogramWidget(config = {}, ...args) {
    if (!this.getBarTextRenderData && !(config !== null && config !== void 0 && config.getBarTextRenderData)) {
      config.getBarTextRenderData = this.getBarTextRenderDataDefault;
    }
    config.cls = 'b-hide-offscreen b-resourceutilization-histogram';
    config.height = this.rowHeight;
    return super.buildHistogramWidget(config, ...args);
  }
  getBarTextRenderDataDefault(renderData, datum, index) {
    // place effort text centered vertically
    renderData.y = '50%';
    return renderData;
  }
  getRecordAllocationInfoRenderData(record, allocation, cellElement, histogramWidget = null) {
    let data;
    if (record.isResourceModel) {
      data = allocation.total;
    } else if (record.isAssignmentModel) {
      data = allocation.byAssignments.get(record);
    }
    // if allocation is collected
    if (data) {
      // we don't want the histogram bar heights based on effort
      // so set heights to 1 here to fit row heights fully
      for (let i = 0, {
          length
        } = data; i < length; i++) {
        if (data[i].effort) data[i].height = 1;
      }
    }
    return data;
  }
  //endregion
  getResourceBarTip(datum) {
    const me = this,
      {
        showBarTip,
        timeAxis
      } = me;
    let result = '';
    if (showBarTip && datum.effort) {
      const unit = me.getBarTipEffortUnit(...arguments),
        allocated = me.getEffortText(datum.effort, unit, true),
        available = me.getEffortText(datum.maxEffort, unit, true),
        assignmentTpl = me.L('L{groupBarTipAssignment}');
      let dateFormat = 'L',
        resultFormat = me.L('L{groupBarTipInRange}'),
        assignmentsSuffix = '';
      if (DateHelper.compareUnits(timeAxis.unit, TimeUnit.Day) === 0) {
        resultFormat = me.L('L{groupBarTipOnDate}');
      } else if (DateHelper.compareUnits(timeAxis.unit, TimeUnit.Second) <= 0) {
        dateFormat = 'HH:mm:ss A';
      } else if (DateHelper.compareUnits(timeAxis.unit, TimeUnit.Hour) <= 0) {
        dateFormat = 'LT';
      }
      let assignmentsArray = [...datum.assignmentIntervals.entries()].filter(([assignment, data]) => data.effort).sort(([key1, value1], [key2, value2]) => value1.effort > value2.effort ? -1 : 1);
      if (assignmentsArray.length > me.groupBarTipAssignmentLimit) {
        assignmentsSuffix = '<br>' + me.L('L{plusMore}').replace('{value}', assignmentsArray.length - me.groupBarTipAssignmentLimit);
        assignmentsArray = assignmentsArray.slice(0, this.groupBarTipAssignmentLimit);
      }
      const assignments = assignmentsArray.map(([assignment, info]) => {
        return assignmentTpl.replace('{event}', StringHelper.encodeHtml(assignment.event.name)).replace('{allocated}', me.getEffortText(info.effort, unit, true)).replace('{available}', me.getEffortText(info.maxEffort, unit, true)).replace('{cls}', info.isOverallocated ? 'b-overallocated' : info.isUnderallocated ? 'b-underallocated' : '');
      }).join('<br>') + assignmentsSuffix;
      result = resultFormat.replace('{assignments}', assignments).replace('{startDate}', DateHelper.format(datum.tick.startDate, dateFormat)).replace('{endDate}', DateHelper.format(datum.tick.endDate, dateFormat)).replace('{allocated}', allocated).replace('{available}', available).replace('{cls}', datum.isOverallocated ? 'b-overallocated' : datum.isUnderallocated ? 'b-underallocated' : '');
      result = `<div class="b-histogram-bar-tooltip">${result}</div>`;
    }
    return result;
  }
  getAssignmentBarTip(datum) {
    const me = this,
      {
        showBarTip,
        timeAxis
      } = me;
    let result = '';
    if (showBarTip && datum.effort) {
      const unit = me.getBarTipEffortUnit(...arguments),
        allocated = me.getEffortText(datum.effort, unit, true),
        available = me.getEffortText(datum.maxEffort, unit, true);
      let dateFormat = 'L',
        resultFormat = me.L('L{barTipInRange}');
      if (DateHelper.compareUnits(timeAxis.unit, TimeUnit.Day) === 0) {
        resultFormat = me.L('L{barTipOnDate}');
      } else if (DateHelper.compareUnits(timeAxis.unit, TimeUnit.Second) <= 0) {
        dateFormat = 'HH:mm:ss A';
      } else if (DateHelper.compareUnits(timeAxis.unit, TimeUnit.Hour) <= 0) {
        dateFormat = 'LT';
      }
      result = resultFormat.replace('{startDate}', DateHelper.format(datum.tick.startDate, dateFormat)).replace('{endDate}', DateHelper.format(datum.tick.endDate, dateFormat)).replace('{allocated}', allocated).replace('{available}', available).replace('{cls}', datum.cls || '');
      if (datum.assignment) {
        result = result.replace('{event}', StringHelper.encodeHtml(datum.assignment.event.name));
      }
      result = `<div class="b-histogram-bar-tooltip">${result}</div>`;
    }
    return result;
  }
}
ResourceUtilization.initClass();
// enable tree feature for the utilization panel by default
ResourceUtilization._$name = 'ResourceUtilization';
GridFeatureManager.registerFeature(Tree, true, 'ResourceUtilization');

/**
 * @module SchedulerPro/view/SchedulerPro
 */
/**
 * ## Intro
 *
 * The Scheduler Pro is an extension of the [Bryntum Scheduler](#Scheduler/view/Scheduler), and combines the visualisation capabilities
 * of the Scheduler with the powerful scheduling engine from the Gantt. This means it can manage {@link SchedulerPro/model/ProjectModel project} data composed by
 * tasks, dependencies, resources, assignments and calendars (for working / non-working time). If you have inter-task dependencies,
 * task updates will be propagated to any successors after a task is moved. The engine will reschedule tasks
 * according to the constraints, dependencies and calendars defined in the project. To familiarize yourself with the various APIs and data structures
 * of the Scheduler Pro, we recommend starting with these resources:
 *
 * * [Project data model guide](#SchedulerPro/guides/basics/project_data.md)
 * * [Bryntum Scheduler API docs](#Scheduler/view/Scheduler)
 * * [Bryntum Grid API docs](#Grid/view/Grid)
 * * [Localization](#SchedulerPro/guides/customization/localization.md)
 *
 * ## Basic setup
 *
 * To create an instance of this class, simply configure it with:
 *
 * * The {@link Grid/column/Column columns} you want
 * * The {@link Grid/view/Grid#config-features} you want, quite a lot to choose from, and you can build your own too
 * * A {@link SchedulerPro/model/ProjectModel Project} instance:
 * * A {@link Scheduler/preset/ViewPreset viewPreset} identifier, specifying the granularity of the time axis.
 *
 * ```javascript
 * const scheduler = new SchedulerPro({
 *    // A Project holds the data and the calculation engine for Scheduler Pro. It also acts as a CrudManager, allowing
 *    // loading data into all stores at once
 *    project : {
 *        autoLoad  : true,
 *        transport : {
 *            load : {
 *                url : './data/data.json'
 *            }
 *       }
 *    },
 *
 *    adopt             : 'container',
 *    startDate         : '2020-05-01',
 *    endDate           : '2020-09-30',
 *    resourceImagePath : '../_shared/images/users/',
 *    viewPreset        : 'dayAndWeek'
 *    features : {
 *       columnLines  : false,
 *       dependencies : true
 *   },
 *
 *   columns : [
 *       {
 *           type           : 'resourceInfo',
 *           text           : 'Worker',
 *           showEventCount : true
 *       }
 *   ]
 * });
 * ```
 *
 * {@inlineexample SchedulerPro/view/SchedulerPro.js}
 *
 * ## Inheriting from Bryntum Grid
 * Bryntum Scheduler Pro inherits from Bryntum Grid, meaning that most features available in the grid are also available
 * for the scheduler. Common features include columns, cell editing, context menus, row grouping, sorting and more.
 * Note: If you want to use the Grid component standalone, e.g. to use drag-from-grid functionality, you need a separate
 * license for the Grid component.
 *
 * ## Customisation
 *
 * You can style any aspect of the Scheduler using plain CSS or modify our themes using our built-in SASS variables.
 * Using the {@link Scheduler/view/mixin/SchedulerEventRendering#config-eventRenderer} you can customize the HTML output for
 * each event bar. The Scheduler comes with a few different {@link #config-eventStyle event styles} which you can
 * define globally on the Scheduler, in the resource data, or on individual events.
 *
 * {@inlineexample SchedulerPro/view/EventStyles.js}
 *
 * For more information about styling, please refer to the [styling guide](#SchedulerPro/guides/customization/styling.md).
 *
 * ## Partnering with other timeline widgets
 *
 * You can also pair the Scheduler Pro with other timeline based widgets such as the {@link SchedulerPro/view/ResourceHistogram histogram widget}
 * to view resource allocation levels, using the {@link #config-partner} config.
 *
 * {@inlineexample SchedulerPro/view/ResourceHistogram.js}
 *
 * ### Differences between Scheduler and Scheduler Pro
 * Scheduler Pro extends Scheduler and schedules tasks based on the Project, Resource and Event calendars, while also taking into account
 * dependencies and constraints. Scheduler Pro also comes with more demos showing off advanced use cases. Below is a list
 * of technical differences between the two versions:
 *
 * - Scheduler uses an EventStore, ResourceStore (optionally an AssignmentStore and a DependencyStore), whereas Scheduler Pro always
 * uses an AssignmentStore to manage event assignments.
 * - Scheduler Pro uses the same data model as the Gantt and can visualise a Project side by side with the Gantt.
 * - Scheduler supports showing dependencies but they are just visual elements, they do not impact scheduling. In Scheduler Pro,
 * adding a dependency between two tasks will affect the scheduling of the successor task.
 * - Scheduler Pro supports visualising a task completion progress bar.
 * - Scheduler Pro includes a Timeline widget and a Resource Histogram widget.
 *
 * @extends SchedulerPro/view/SchedulerProBase
 * @classType schedulerpro
 * @widget
 */
class SchedulerPro extends SchedulerProBase {
  //region Config
  static get $name() {
    return 'SchedulerPro';
  }
  static get type() {
    return 'schedulerpro';
  }
  //endregion
}

SchedulerPro.initClass();
SchedulerPro._$name = 'SchedulerPro';

/**
 * @module SchedulerPro/widget/Timeline
 */
/**
 * A visual component showing an overview timeline of events having the {@link SchedulerPro.model.EventModel#field-showInTimeline showInTimeline}
 * field set to true. The timeline component subclasses the {@link Scheduler.view.Scheduler Scheduler} and to use it,
 * simply provide it with a {@link SchedulerPro.model.ProjectModel}:
 *
 * ```javascript
 * const timeline = new Timeline({
 *     appendTo  : 'container',
 *     project   : project
 * });
 * ```
 *
 * {@inlineexample SchedulerPro/widget/Timeline.js}
 *
 * @extends Scheduler/view/Scheduler
 * @classType timeline
 * @widget
 */
class Timeline extends SchedulerBase {
  static get $name() {
    return 'Timeline';
  }
  // Factoryable type name
  static get type() {
    return 'timeline';
  }
  static get configurable() {
    return {
      /**
       * Project config object or a Project instance
       *
       * @config {SchedulerPro.model.ProjectModel|ProjectModelConfig} project
       */
      /**
       * @hideconfigs timeZone
       */
      height: '13em',
      eventLayout: 'pack',
      barMargin: 1,
      // We need timeline width to be exact, because with `overflow: visible` content will look awful.
      // Flow is like this:
      // 1. zoomToFit is trying to set timespan to eventStore total time span. Assume start in on tuesday and end is on friday
      // 2. zooming mixin is calculating tick width, which is e.g. 37px to fit all the ticks to the available space
      // 3. timeAxis is configured with this new time span. By default it adjusts start and end to monday.
      // 4. since timespan was increased, it now overflows with original tick size of 37. It requires smth smaller, like 34.
      // 5. timeAxisViewModel is calculating fitting size. Which is correct value of 34, but value is ignored unless `forceFit` is true
      // But apparently forceFit + zoomToSpan IS NOT SUPPORTED. So alternative approach is to disable autoAdjust
      // on time axis to prevent increased size in #3. But then time axis start/end won't be even date, it could be
      // smth random like `Thu Feb 07 2019 22:13:20`.
      //
      // On the other hand, without force-fit content might overflow and timeline is styled to show overflowing content.
      // And that would require more additional configs
      forceFit: true,
      timeAxis: {
        autoAdjust: false
      },
      readOnly: true,
      zoomOnMouseWheel: false,
      zoomOnTimeAxisDoubleClick: false,
      // eventColor                : null,
      // eventStyle                : null,
      rowHeight: 48,
      displayDateFormat: 'L',
      // A fake resource
      resources: [{
        id: 1
      }],
      columns: []
    };
  }
  static get delayable() {
    return {
      fillFromTaskStore: 100
    };
  }
  construct(config = {}) {
    const me = this;
    me.startDateLabel = document.createElement('label');
    me.startDateLabel.className = 'b-timeline-startdate';
    me.endDateLabel = document.createElement('label');
    me.endDateLabel.className = 'b-timeline-enddate';
    let initialCommitPerformed = true;
    if ('project' in config) {
      if (!config.project) {
        throw new Error('You need to configure the Timeline with a Project');
      }
      // In case instance of project is provided, just take store right away and delete config, falling back to
      // default
      else if (config.project instanceof SchedulerProProjectMixin) {
        me.taskStore = config.project.eventStore;
        if (!config.project.isInitialCommitPerformed) {
          initialCommitPerformed = false;
          // For schedulerpro it is important to listen to first project commit
          config.project.ion({
            name: 'initialCommit',
            refresh({
              isInitialCommit
            }) {
              if (isInitialCommit) {
                me.fillFromTaskStore();
                me.detachListeners('initialCommit');
              }
            },
            thisObj: me
          });
        }
        delete config.project;
      }
    }
    // Despite the fact Timeline extends SchedulerBase, we still need to disable all these features.
    // Because in case timeline gets into the same scope as scheduler or gantt, some features might be enabled
    // by default. SchedulerBase jut means that we don't import anything extra. But other components might.
    config.features = ObjectHelper.assign({
      cellEdit: false,
      cellMenu: false,
      columnAutoWidth: false,
      columnLines: false,
      columnPicker: false,
      columnReorder: false,
      columnResize: false,
      contextMenu: false,
      eventContextMenu: false,
      eventDrag: false,
      eventDragCreate: false,
      eventEdit: false,
      eventFilter: false,
      eventMenu: false,
      eventResize: false,
      eventTooltip: false,
      group: false,
      headerMenu: false,
      regionResize: false,
      scheduleContextMenu: false,
      scheduleMenu: false,
      scheduleTooltip: false,
      sort: false,
      timeAxisHeaderMenu: false,
      timeRanges: false
    }, config.features);
    super.construct(config);
    if (me.features.timeRanges) {
      // We don't want to show timeRanges relating to Project
      me.features.timeRanges.store = new Store();
    }
    // If original project is not committed by this time, we should not try to fill timeline from the task store,
    // because project listener will do it itself. And also to not do extra suspendRefresh which would break project
    // refresh event listener behavior.
    // https://github.com/bryntum/support/issues/2665
    initialCommitPerformed && me.fillFromTaskStore.now();
    me.taskStore.ion({
      refreshPreCommit: me.fillFromTaskStore,
      changePreCommit: me.onTaskStoreChange,
      thisObj: me
    });
    me.ion({
      resize: me.onSizeChanged,
      thisObj: me
    });
    me.bodyContainer.appendChild(me.startDateLabel);
    me.bodyContainer.appendChild(me.endDateLabel);
  }
  onSizeChanged({
    width,
    oldWidth
  }) {
    const me = this,
      reFit = width !== oldWidth;
    // Save a refresh, will come from fit. Don't suspend if we won't re-fit, we need the refresh for events
    // to not disappear (since updating row height clears cache)
    reFit && me.suspendRefresh();
    me.updateRowHeight();
    if (reFit) {
      me.resumeRefresh();
      me.fitTimeline();
    }
  }
  updateRowHeight() {
    if (this.bodyContainer.isConnected) {
      this.rowHeight = this.bodyContainer.offsetHeight;
    }
  }
  fitTimeline() {
    if (this.eventStore.count > 0) {
      this.forceFit = false;
      this.zoomToFit({
        leftMargin: 50,
        rightMargin: 50
      });
      this.forceFit = true;
    }
    this.updateStartEndLabels();
  }
  updateStartEndLabels() {
    const me = this;
    me.startDateLabel.innerHTML = me.getFormattedDate(me.startDate);
    me.endDateLabel.innerHTML = me.getFormattedDate(me.endDate);
  }
  async onTaskStoreChange({
    action,
    record,
    records,
    changes,
    isCollapse
  }) {
    const me = this,
      eventStore = me.eventStore;
    let needsFit;
    switch (action) {
      case 'add':
        records.forEach(task => {
          if (task.showInTimeline) {
            eventStore.add(me.cloneTask(task));
            needsFit = true;
          }
        });
        break;
      case 'remove':
        if (!isCollapse) {
          records.forEach(task => {
            if (task.showInTimeline) {
              eventStore.remove(task.id);
              needsFit = true;
            }
          });
        }
        break;
      case 'removeall':
        me.fillFromTaskStore.now();
        break;
      case 'update':
        {
          const task = record;
          if (changes.showInTimeline) {
            // Add or remove from our eventStore
            if (task.showInTimeline) {
              eventStore.add(me.cloneTask(task));
            } else {
              const timelineEvent = eventStore.getById(task.id);
              if (timelineEvent) {
                eventStore.remove(timelineEvent);
              }
            }
            needsFit = true;
          } else if (task.showInTimeline) {
            // Just sync with existing clone
            const clone = eventStore.getById(task.id);
            if (clone) {
              // Fields might have been remapped
              clone.set(me.cloneTask(task));
              needsFit = true;
            }
          }
          break;
        }
    }
    if (needsFit) {
      me.fitTimeline();
    }
  }
  cloneTask(task) {
    return {
      id: task.id,
      resourceId: 1,
      name: task.name,
      startDate: task.startDate,
      endDate: task.endDate,
      cls: task.cls
    };
  }
  render() {
    super.render(...arguments);
    this.updateRowHeight();
  }
  async fillFromTaskStore() {
    const me = this,
      timelineTasks = [];
    me.taskStore.traverse(task => {
      if (task.showInTimeline && task.isScheduled) {
        timelineTasks.push(me.cloneTask(task));
      }
    });
    me.events = timelineTasks;
    await me.project.commitAsync();
    if (me.isDestroyed) {
      return;
    }
    me.fitTimeline();
  }
  onLocaleChange() {
    this.updateStartEndLabels();
    super.onLocaleChange();
  }
}
// Register this widget type with its Factory
Timeline.initClass();
Timeline._$name = 'Timeline';

/**
 * @module SchedulerPro/widget/VersionGrid
 */
const EMPTY_ARRAY = [],
  actionTypeOrder = {
    remove: 1,
    add: 2,
    update: 3
  },
  entityTypeOrder = {
    TaskModel: 1,
    DependencyModel: 2,
    AssignmentModel: 3,
    ProjectModel: 4
  },
  // For moves, describe the former and current locations
  describePosition = ({
    parent,
    index
  }) => `${parent.name}[${index}]`,
  knownEntityTypes = {
    AssignmentModel: 'Assignment',
    DependencyModel: 'Dependency'
  };
class VersionGridRow extends GridRowModel {
  static fields = [{
    name: 'description',
    type: 'string'
  }, {
    name: 'occurredAt',
    type: 'date'
  }, {
    name: 'versionModel'
  }, {
    name: 'transactionModel'
  }, {
    name: 'propertyUpdate'
  }, {
    name: 'action'
  }];
}
/**
 * Displays a list of versions and the transactions they contain. For use with the {@link SchedulerPro.feature.Versions}
 * feature.
 *
 * Configure the VersionGrid with a {@link SchedulerPro.model.ProjectModel} using the {@link #config-project} config.
 *
 * @extends Grid/view/TreeGrid
 * @classType versiongrid
 * @widget
 */
class VersionGrid extends TreeGrid {
  static $name = 'VersionGrid';
  static type = 'versiongrid';
  static configurable = {
    store: {
      tree: true,
      modelClass: VersionGridRow,
      sorters: [{
        field: 'occurredAt',
        ascending: false
      }, VersionGrid.sortActionRows],
      reapplySortersOnAdd: true
    },
    /**
     * The {@link SchedulerPro.model.ProjectModel} whose versions and changes are being observed in this grid.
     * @config {SchedulerPro.model.ProjectModel}
     */
    project: null,
    /**
     * Whether to display transactions not yet associated with a version.
     * @prp {Boolean}
     */
    showUnattachedTransactions: true,
    /**
     * Whether to show only versions that have been assigned a specific name.
     * @prp {Boolean}
     */
    showNamedVersionsOnly: false,
    /**
     * Whether to include version rows in the display.
     * @prp {Boolean}
     */
    showVersions: true,
    /**
     * The id of the version currently being compared, if any.
     * @prp {Boolean}
     */
    comparingVersionId: null,
    flex: 0,
    features: {
      group: {
        field: 'id'
      },
      cellEdit: {
        continueEditingOnCellClick: false,
        editNextOnEnterPress: false
      },
      cellMenu: {
        items: {
          removeRow: false,
          cut: false,
          copy: false,
          paste: false,
          renameButton: {
            text: 'L{VersionGrid.rename}',
            icon: 'b-icon b-icon-edit',
            onItem: ({
              record,
              source: grid
            }) => {
              grid.startEditing({
                id: record.id,
                column: grid.columns.get('description')
              });
            }
          },
          restoreButton: {
            text: 'L{VersionGrid.restore}',
            icon: 'b-icon b-icon-undo',
            onItem: ({
              record,
              source: grid
            }) => {
              grid.triggerRestore(record.versionModel);
            }
          },
          compareButton: {
            text: 'L{VersionGrid.compare}',
            icon: 'b-icon b-icon-compare',
            onItem: ({
              record,
              source: grid
            }) => {
              grid.triggerCompare(record.versionModel);
            }
          },
          stopComparingButton: {
            text: 'L{VersionGrid.stopComparing}',
            onItem: ({
              record,
              source: grid
            }) => {
              grid.triggerStopCompare();
            }
          }
        }
      },
      rowCopyPaste: false
    },
    columns: [{
      type: 'tree',
      text: 'L{VersionGrid.description}',
      field: 'description',
      flex: 4,
      groupable: false,
      renderer: ({
        grid,
        ...rest
      }) => grid.renderDescription({
        grid,
        ...rest
      }),
      autoHeight: true
    }, {
      text: 'L{VersionGrid.occurredAt}',
      field: 'occurredAt',
      type: 'date',
      flex: 1,
      groupable: false
    }],
    /**
     * The date format used for displaying date values in change actions.
     * @config {String}
     */
    dateFormat: 'M/D/YY h:mm a',
    internalListeners: {
      beforeCellEditStart({
        editorContext: {
          column,
          record
        }
      }) {
        // Only version descriptions are editable
        if (!(column.field === 'description' && record.versionModel)) {
          return false;
        }
      },
      finishCellEdit({
        editorContext: {
          record,
          value
        }
      }) {
        record.versionModel.name = value != null && value.trim() ? value : null;
      },
      cellMenuBeforeShow({
        source,
        record,
        items
      }) {
        items.stopComparingButton.disabled = !source.comparingVersionId;
        return Boolean(record.versionModel);
      },
      toggleNode({
        record,
        collapse
      }) {
        this._expandedById.set(record.id, !collapse);
      }
    }
  };
  static delayable = {
    processUpdates: {
      type: 'raf',
      cancelOutstanding: true
    }
  };
  // Bookkeeping fields
  static get properties() {
    return {
      _rowsByUnderlyingRecord: new WeakMap(),
      _expandedById: new Map()
    };
  }
  _transactionChanges = [];
  _versionChanges = [];
  comparingRowCls = `b-${VersionGrid.type}-comparing`;
  construct(config) {
    super.construct({
      ...config,
      features: ObjectHelper.merge({}, VersionGrid.configurable.features, config.features)
    });
  }
  afterConstruct() {
    if (!this.project) {
      throw new Error(`${VersionGrid.$name} requires the project config.`);
    }
    this.refreshGrid();
  }
  updateDateFormat(newDateFormat) {
    const occurredAtColumn = this.columns.get('occurredAt');
    if (occurredAtColumn) {
      occurredAtColumn.format = newDateFormat;
    }
  }
  updateProject(newProject) {
    const me = this;
    me.detachListeners('storeChange');
    me._versionStore = newProject.getCrudStore('versions');
    me._transactionStore = newProject.getCrudStore('changelogs');
    me._versionStore.ion({
      name: 'storeChange',
      change: me.onVersionStoreChange,
      thisObj: me
    });
    me._transactionStore.ion({
      name: 'storeChange',
      change: me.onTransactionStoreChange,
      thisObj: me
    });
  }
  updateShowNamedVersionsOnly() {
    if (this.isPainted) {
      this.refreshGrid();
    }
  }
  updateShowUnattachedTransactions() {
    if (this.isPainted) {
      this.refreshGrid();
    }
  }
  updateShowVersions() {
    if (this.isPainted) {
      this.refreshGrid();
    }
  }
  updateComparingVersionId(newVersionId, oldVersionId) {
    const [oldHighlightedRow, newHighlightedRow] = [oldVersionId, newVersionId].map(versionId => this.store.getById(`v-${versionId}`));
    if (oldHighlightedRow) {
      oldHighlightedRow.cls = '';
      oldHighlightedRow.iconCls = 'b-icon b-icon-version';
    }
    if (newHighlightedRow) {
      newHighlightedRow.cls = this.comparingRowCls;
      newHighlightedRow.iconCls = 'b-icon b-icon-compare';
    }
  }
  onVersionStoreChange({
    action,
    records
  }) {
    this._versionChanges.push({
      action,
      records
    });
    this.processUpdates();
  }
  onTransactionStoreChange({
    action,
    records
  }) {
    this._transactionChanges.push({
      action,
      records
    });
    this.processUpdates();
  }
  /**
   * This is an optimization to more efficiently replace grid rows when the underlying stores change.
   * We wait a tick, then replace the set of rows corresponding to the modified records with the new
   * projected rowset.
   *
   * The code below does not handle record remove, or updating transactions without their version in the
   * same tick. (Versions can be updated without their transactions, as when renamed.)
   * @private
   */
  processUpdates() {
    const me = this,
      versions = ArrayHelper.unique(me._versionChanges.flatMap(({
        records
      }) => records)),
      versionIds = new Set(versions.map(version => String(version.id))),
      transactions = ArrayHelper.unique(me._transactionChanges.flatMap(({
        records
      }) => records)
      // Expand to all transactions for incoming versions
      .concat(versions.length === 0 ? [] : me._transactionStore.query(txn => versionIds.has(txn.versionId))));
    // Expand to all versions for incoming transaction
    for (const transaction of transactions) {
      if (transaction.versionId && !versionIds.has(transaction.versionId)) {
        versions.push(me._versionStore.getById(transaction.versionId));
        versionIds.add(transaction.versionId);
      }
    }
    me.replaceRows(ArrayHelper.unique(versions), transactions);
    me._transactionChanges = [];
    me._versionChanges = [];
  }
  replaceRows(versions, transactions) {
    const me = this,
      {
        showNamedVersionsOnly,
        showUnattachedTransactions,
        store
      } = me,
      rowsToReplaceSet = new Set(),
      transactionsByVersionId = ArrayHelper.groupBy(transactions, 'versionId'),
      allRecords = transactions.concat(versions),
      versionsToShow = showNamedVersionsOnly ? versions.filter(version => version.name != null) : versions;
    for (const record of allRecords) {
      for (const row of me._rowsByUnderlyingRecord.get(record) ?? EMPTY_ARRAY) {
        rowsToReplaceSet.add(row);
      }
    }
    me.suspendRefresh();
    store.remove(Array.from(rowsToReplaceSet));
    for (const version of versionsToShow) {
      const newRows = store.add(me.getGridRows(version, transactionsByVersionId[version.id]));
      me._rowsByUnderlyingRecord.set(version, newRows);
    }
    if (showUnattachedTransactions) {
      for (const transaction of transactions.filter(txn => txn.versionId == null)) {
        const newRows = store.add(me.getGridRows(null, [transaction]));
        me._rowsByUnderlyingRecord.set(transaction, newRows);
      }
    }
    me.resumeRefresh();
    store.sort(store.sorters);
  }
  /**
   * Does a full replace of all rows in the grid using all records currently in the two stores.
   * @private
   */
  refreshGrid() {
    this.replaceRows(this._versionStore.records, this._transactionStore.records);
  }
  /**
   * Transform a set of transactions (and optional parent version) into tree structure needed by grid
   * @private
   */
  getGridRows(version, transactions) {
    const me = this,
      {
        showVersions,
        comparingVersionId
      } = me,
      transactionRows = (transactions === null || transactions === void 0 ? void 0 : transactions.map(transaction => {
        var _me$_expandedById;
        const id = `t-${transaction.id}`;
        return {
          id,
          expanded: Boolean((_me$_expandedById = me._expandedById) === null || _me$_expandedById === void 0 ? void 0 : _me$_expandedById.get(id)),
          description: transaction.description,
          occurredAt: transaction.occurredAt,
          transactionModel: transaction,
          rootVersionModel: version,
          children: transaction.actions.map((action, index) => {
            var _me$_expandedById2, _action$propertyUpdat;
            const id = `a-${transaction.id}-${index}`;
            return {
              id,
              expanded: Boolean((_me$_expandedById2 = me._expandedById) === null || _me$_expandedById2 === void 0 ? void 0 : _me$_expandedById2.get(id)),
              action,
              rootVersionModel: version,
              children: ((_action$propertyUpdat = action.propertyUpdates) === null || _action$propertyUpdat === void 0 ? void 0 : _action$propertyUpdat.map(propertyUpdate => ({
                rootVersionModel: version,
                propertyUpdate
              }))) ?? []
            };
          })
        };
      })) || [],
      id = `v-${version === null || version === void 0 ? void 0 : version.id}`;
    return version && showVersions ? {
      id,
      expanded: Boolean(me._expandedById.get(id)),
      description: version.description,
      occurredAt: version.savedAt,
      children: transactionRows,
      versionModel: version,
      iconCls: 'b-icon-version',
      cls: version.id === comparingVersionId ? me.comparingRowCls : null
    } : transactionRows;
  }
  renderDescription(event) {
    const {
      record
    } = event;
    if (record.propertyUpdate) {
      return this.renderPropertyUpdate(record.propertyUpdate);
    } else if (record.action) {
      return this.renderActionDescription(record.action);
    }
    return record.description;
  }
  renderPropertyUpdate(propertyUpdate) {
    const clsPrefix = VersionGrid.type,
      {
        property,
        before,
        after
      } = propertyUpdate;
    return {
      children: [{
        tag: 'div',
        class: `b-${clsPrefix}-property-update-desc`,
        children: [{
          tag: 'span',
          class: `b-${clsPrefix}-property-name`,
          html: `${this.formatPropertyName(property)}`
        }, this.renderPropertyValue(before, 'before'), {
          tag: 'i',
          class: 'b-icon b-icon-right'
        }, this.renderPropertyValue(after, 'after')]
      }]
    };
  }
  /**
   * Return DomConfig for an individual data value.
   * @param {*} value
   * @param {'before'|'after'} side
   * @returns {DomConfig}
   * @private
   */
  renderPropertyValue(value, side) {
    return {
      tag: 'span',
      class: [`b-${VersionGrid.type}-property-${side}`, value == null && `b-${VersionGrid.type}-empty-value`],
      html: value == null ? this.L('L{Versions.nullValue}') : this.formatValueString(value) ?? ``
    };
  }
  /**
   * Convert an individual data value to a string.
   * @param {*} value The raw data value
   * @returns {String} A string representing the value, for display
   * @private
   */
  formatValueString(value) {
    if (DateHelper.isDate(value)) {
      return DateHelper.format(value, this.dateFormat);
    } else if (typeof value === 'number') {
      return value.toFixed(2);
    }
    return value;
  }
  /**
   * Format a property name in the change log to a displayable string. By default,
   * converts e.g. "camelCase" to "Camel case".
   * @param {String} propertyName The raw field name
   * @returns {String} A string formatted for display
   * @private
   */
  formatPropertyName(propertyName) {
    return StringHelper.separate(propertyName);
  }
  getAssignmentTextTokens(assignmentChange) {
    return {
      event: assignmentChange.event.name,
      resource: assignmentChange.resource.name
    };
  }
  getDependencyTextTokens(dependencyChange) {
    return {
      from: dependencyChange.fromTask.name,
      to: dependencyChange.toTask.name
    };
  }
  /**
   * Produces a text description to show in the description column for an 'action' row.
   * @param {SchedulerPro.model.changelog.ChangeLogAction} action The action to describe
   * @returns DomConfig of description text with highlightable entity names
   * @private
   */
  renderActionDescription(action) {
    const me = this,
      {
        actionType,
        entity
      } = action,
      entityNames = me.L(`L{Versions.entityNames}`);
    let description,
      tokens = {
        type: entityNames[entity.type],
        name: entity.name
      };
    if (actionType === 'move') {
      tokens.from = describePosition(action.from);
      tokens.to = describePosition(action.to);
    }
    // Concatenate action and entity type to get description pattern from localizations
    // e.g. 'L{Versions.addDependency}' | 'L{Versions.updateEntity}'
    description = me.L(`L{Versions.${actionType}${knownEntityTypes[entity.type] ?? 'Entity'}}`);
    if (entity.type === 'DependencyModel') {
      tokens = me.getDependencyTextTokens(entity);
    } else if (entity.type === 'AssignmentModel') {
      tokens = me.getAssignmentTextTokens(entity);
    }
    description = description.replace(/\{(\w+)\}/g, (_, variable) => tokens[variable] ?? variable);
    if (action.isUser) {
      description = `[!] ${description}`;
    }
    return me.renderHighlightedTextElements(StringHelper.capitalize(description), tokens);
  }
  /**
   * Sorts the actions within a transaction using precedence heuristic to show most "significant"
   * actions first.
   * @param {SchedulerPro.model.changelog.ChangeLogAction[]} actions
   */
  static sortActionRows(row1, row2) {
    if (row1.parent === row2.parent && row1.action && row2.action) {
      const isUser1 = Boolean(row1.action.isUser),
        isUser2 = Boolean(row2.action.isUser),
        {
          actionType: type1,
          entity: {
            type: entityType1
          }
        } = row1.action,
        {
          actionType: type2,
          entity: {
            type: entityType2
          }
        } = row2.action;
      // Initial user actions first
      if (isUser1 !== isUser2) {
        return isUser1 ? -1 : 1;
      }
      // Adds/removes first, then updates; within those groups, tasks first
      return Math.sign(actionTypeOrder[type1] - actionTypeOrder[type2]) || Math.sign(entityTypeOrder[entityType1] - entityTypeOrder[entityType2]) || 0;
    }
    return 0;
  }
  triggerRestore(version) {
    /**
     * Fires when the user chooses to restore a selected version.
     * @event restore
     * @param {SchedulerPro.model.VersionModel} version The {@link SchedulerPro.model.VersionModel} being restored
     */
    this.trigger('restore', {
      version
    });
  }
  triggerCompare(version) {
    /**
     * Fires when the user chooses to compare a selected version.
     * @event compare
     * @param {SchedulerPro.model.VersionModel} version The {@link SchedulerPro.model.VersionModel} being restored
     */
    this.trigger('compare', {
      version
    });
  }
  triggerStopCompare(version) {
    /**
     * Fires when the user chooses to stop comparing a currently compared version.
     * @event stopCompare
     */
    this.trigger('stopCompare');
  }
  /**
   * Produce a DomConfig for cell text where **-delimited tokens are replaced by specified values. Used to
   * allow CSS styling of replaced tokens (e.g. task names) in the changelog.
   *
   * @param {String} text Text string containing optional **delimited tokens**, taken from localizations
   * @returns {DomConfig} DomConfig with text string broken into <span>s and tokens replaced
   * @internal
   */
  renderHighlightedTextElements(text) {
    const clsPrefix = this.constructor.type;
    return {
      children: [{
        tag: 'span',
        class: `b-${clsPrefix}-highlighted-text`,
        children: text.split(/\*\*/g).reduce((out, chunk) => {
          out.children.push({
            tag: 'span',
            text: chunk,
            class: out.isEntity ? `b-${clsPrefix}-highlighted-entity` : null
          });
          out.isEntity = !out.isEntity;
          return out;
        }, {
          children: [],
          isEntity: false
        }).children
      }]
    };
  }
}
VersionGrid.initClass();
VersionGrid._$name = 'VersionGrid';

export { CalendarHighlight, ChangeLogPropertyUpdate, DependencyEdit, EventBuffer, EventModel, EventSegmentDrag, EventStore, NestedEvents, ProHorizontalLayout, ProHorizontalLayoutPack, ProHorizontalLayoutStack, ProjectModel, ResourceCalendarColumn, ResourceHistogram, ResourceNonWorkingTime, ResourceUtilization, ScaleColumn, SchedulerPro, SchedulerProBase, SchedulerProEventRendering, TimeSpanHighlight, Timeline, VersionGrid };
//# sourceMappingURL=schedulerpro.module.thin.js.map
