/*!
 *
 * Bryntum Scheduler Pro 5.3.7
 *
 * Copyright(c) 2023 Bryntum AB
 * https://bryntum.com/contact
 * https://bryntum.com/license
 *
 */
import { Localizable, DateHelper, InstancePlugin, ObjectHelper, Objects, Config, Popup, Store, Button, Base, Widget, Combo, DomHelper, Delayable, List, StringHelper, ArrayHelper, VersionHelper } from './Editor.js';
import { DateField, DatePicker } from './MessageDialog.js';
import { GridFeatureManager } from './GridBase.js';
import { RecurrenceFrequencyCombo, TaskEditStm } from './EventNavigation.js';
import { RecurrenceDayRuleEncoder, TimeSpan } from './ProjectModel.js';

/**
 * @module Scheduler/data/util/recurrence/RecurrenceLegend
 */
/**
 * A static class allowing to get a human readable description of the provided recurrence.
 *
 * ```javascript
 * const event = new EventModel({
 *      startDate : new Date(2018, 6, 3),
 *      endDate   : new Date(2018, 6, 4)
 * });
 * const recurrence = new RecurrenceModel({
 *      frequency : 'WEEKLY',
 *      days : ['MO', 'TU', 'WE']
 * });
 * event.recurrence = recurrence;
 * // "Weekly on Mon, Tue and Wed"
 * RecurrenceLegend.getLegend(recurrence);
 * ```
 *
 * @mixes Core/localization/Localizable
 */
class RecurrenceLegend extends Localizable() {
  static get $name() {
    return 'RecurrenceLegend';
  }
  static get allDaysValueAsArray() {
    return ['SU', 'MO', 'TU', 'WE', 'TH', 'FR', 'SA'];
  }
  static get allDaysValue() {
    return this.allDaysValueAsArray.join(',');
  }
  static get workingDaysValue() {
    return this.allDaysValueAsArray.filter((day, index) => !DateHelper.nonWorkingDays[index]).join(',');
  }
  static get nonWorkingDaysValue() {
    return this.allDaysValueAsArray.filter((day, index) => DateHelper.nonWorkingDays[index]).join(',');
  }
  /**
   * Returns the provided recurrence description. The recurrence might be assigned to a timespan model,
   * in this case the timespan start date should be provided in the second argument.
   * @param {Scheduler.model.RecurrenceModel} recurrenceRecurrence model.
   * @param {Date} [timeSpanStartDate] The recurring timespan start date. Can be omitted if the recurrence is assigned
   * to a timespan model (and the timespan has {@link Scheduler.model.TimeSpan#field-startDate} filled). Then start
   * date will be retrieved from the model.
   * @returns {String} The recurrence description.
   */
  static getLegend(recurrence, timeSpanStartDate) {
    const me = this,
      {
        timeSpan,
        interval,
        days,
        monthDays,
        months,
        positions
      } = recurrence,
      startDate = timeSpanStartDate || timeSpan.startDate,
      tplData = {
        interval
      };
    let fn;
    switch (recurrence.frequency) {
      case 'DAILY':
        return interval === 1 ? me.L('L{Daily}') : me.L('L{Every {0} days}', tplData);
      case 'WEEKLY':
        if (days && days.length) {
          tplData.days = me.getDaysLegend(days);
        } else if (startDate) {
          tplData.days = DateHelper.getDayName(startDate.getDay());
        }
        return me.L(interval === 1 ? 'L{Weekly on {1}}' : 'L{Every {0} weeks on {1}}', tplData);
      case 'MONTHLY':
        if (days && days.length && positions && positions.length) {
          tplData.days = me.getDaysLegend(days, positions);
        } else if (monthDays && monthDays.length) {
          // sort dates to output in a proper order
          monthDays.sort((a, b) => a - b);
          tplData.days = me.arrayToText(monthDays);
        } else if (startDate) {
          tplData.days = startDate.getDate();
        }
        return me.L(interval === 1 ? 'L{Monthly on {1}}' : 'L{Every {0} months on {1}}', tplData);
      case 'YEARLY':
        if (days && days.length && positions && positions.length) {
          tplData.days = me.getDaysLegend(days, positions);
        } else {
          tplData.days = startDate.getDate();
        }
        if (months && months.length) {
          // sort months to output in a proper order
          months.sort((a, b) => a - b);
          if (months.length > 2) {
            fn = month => DateHelper.getMonthShortName(month - 1);
          } else {
            fn = month => DateHelper.getMonthName(month - 1);
          }
          tplData.months = me.arrayToText(months, fn);
        } else {
          tplData.months = DateHelper.getMonthName(startDate.getMonth());
        }
        return me.L(interval === 1 ? 'L{Yearly on {1} of {2}}' : 'L{Every {0} years on {1} of {2}}', tplData);
    }
  }
  static getDaysLegend(days, positions) {
    const me = this,
      tplData = {
        position: ''
      };
    let fn;
    if (positions && positions.length) {
      tplData.position = me.arrayToText(positions, position => me.L(`L{position${position}}`));
    }
    if (days.length) {
      days.sort((a, b) => RecurrenceDayRuleEncoder.decodeDay(a)[0] - RecurrenceDayRuleEncoder.decodeDay(b)[0]);
      switch (days.join(',')) {
        case me.allDaysValue:
          tplData.days = me.L('L{day}');
          break;
        case me.workingDaysValue:
          tplData.days = me.L('L{weekday}');
          break;
        case me.nonWorkingDaysValue:
          tplData.days = me.L('L{weekend day}');
          break;
        default:
          if (days.length > 2) {
            fn = day => DateHelper.getDayShortName(RecurrenceDayRuleEncoder.decodeDay(day)[0]);
          } else {
            fn = day => DateHelper.getDayName(RecurrenceDayRuleEncoder.decodeDay(day)[0]);
          }
          tplData.days = me.arrayToText(days, fn);
      }
    }
    return me.L('L{daysFormat}', tplData);
  }
  // Converts array of items to a human readable list.
  // For example: [1,2,3,4]
  // to: "1, 2, 3 and 4"
  static arrayToText(array, fn) {
    if (fn) {
      array = array.map(fn);
    }
    return array.join(', ').replace(/,(?=[^,]*$)/, this.L('L{ and }'));
  }
}
RecurrenceLegend._$name = 'RecurrenceLegend';

/**
 * @module Scheduler/feature/base/EditBase
 */
const DH = DateHelper,
  scheduleFields = ['startDate', 'endDate', 'resource', 'recurrenceRule'],
  makeDate = fields => {
    // single field, update record directly
    if (fields.length === 1) return fields[0].value;
    // two fields, date + time
    else if (fields.length === 2) {
      const [date, time] = fields[0] instanceof DateField ? fields : fields.reverse(),
        dateValue = DH.parse(date.value);
      if (dateValue && time.value) {
        dateValue.setHours(time.value.getHours(), time.value.getMinutes(), time.value.getSeconds(), time.value.getMilliseconds());
      }
      // Clone to not end up sharing dates
      return dateValue ? DateHelper.clone(dateValue) : null;
    }
    // shouldn't happen...
    return null;
  },
  copyTime = (dateTo, dateFrom) => {
    const d = new Date(dateTo.getTime());
    d.setHours(dateFrom.getHours(), dateFrom.getMinutes());
    return d;
  },
  adjustEndDate = (startDate, startTime, me) => {
    // The end datetime just moves in response to the changed start datetime, keeping the same duration.
    if (startDate && startTime && me.endDateField && me.endTimeField) {
      const newEndDate = DH.add(copyTime(me.startDateField.value, me.startTimeField.value), me.eventRecord.durationMS, 'milliseconds');
      me.endDateField.value = newEndDate;
      me.endTimeField.value = DH.clone(newEndDate);
    }
  };
/**
 * Base class for EventEdit. Not to be used directly.
 *
 * @extends Core/mixin/InstancePlugin
 */
class EditBase extends InstancePlugin {
  //region Config
  static get configurable() {
    return {
      /**
       * True to save and close this panel if ENTER is pressed in one of the input fields inside the panel.
       * @config {Boolean}
       * @default
       * @category Editor
       */
      saveAndCloseOnEnter: true,
      triggerEvent: null,
      /**
       * This config parameter is passed to the `startDateField` and `endDateField` constructor.
       * @config {String}
       * @default
       * @category Editor widgets
       */
      dateFormat: 'L',
      // date format that uses browser locale
      /**
       * This config parameter is passed to the `startTimeField` and `endTimeField` constructor.
       * @config {String}
       * @default
       * @category Editor widgets
       */
      timeFormat: 'LT',
      // date format that uses browser locale
      /**
       * Default editor configuration, which widgets it shows etc.
       *
       * This is the entry point into configuring any aspect of the editor.
       *
       * The {@link Core.widget.Container#config-items} configuration of a Container
       * is *deeply merged* with its default `items` value. This means that you can specify
       * an `editorConfig` object which configures the editor, or widgets inside the editor:
       * ```javascript
       * const scheduler = new Scheduler({
       *     features : {
       *         eventEdit  : {
       *             editorConfig : {
       *                 autoClose : false,
       *                 modal     : true,
       *                 cls       : 'editor-widget-cls',
       *                 items : {
       *                     resourceField : {
       *                         hidden : true
       *                     },
       *                     // Add our own event owner field at the top of the form.
       *                     // Weight -100 will make it sort top the top.
       *                     ownerField : {
       *                         weight : -100,
       *                         type   : 'usercombo',
       *                         name   : 'owner',
       *                         label  : 'Owner'
       *                     }
       *                 },
       *                 bbar : {
       *                     items : {
       *                         deleteButton : false
       *                     }
       *                 }
       *             }
       *         }
       *     }
       * });
       * ```
       * @config {PopupConfig}
       * @category Editor
       */
      editorConfig: null,
      /**
       * An object to merge with the provided items config of the editor to override the
       * configuration of provided fields, or add new fields.
       *
       * To remove existing items, set corresponding keys to `null`:
       *
       * ```javascript
       * const scheduler = new Scheduler({
       *     features : {
       *         eventEdit  : {
       *             items : {
       *                 // Merged with provided config of the resource field
       *                 resourceField : {
       *                     label : 'Calendar'
       *                 },
       *                 recurrenceCombo : null,
       *                 owner : {
       *                     weight : -100, // Will sort above system-supplied fields which are weight 0
       *                     type   : 'usercombo',
       *                     name   : 'owner',
       *                     label  : 'Owner'
       *                 }
       *             }
       *         }
       *     }
       * });
       *```
       *
       * The provided fields are called
       *  - `nameField`
       *  - `resourceField`
       *  - `startDateField`
       *  - `startTimeField`
       *  - `endDateField`
       *  - `endTimeField`
       *  - `recurrenceCombo`
       *  - `editRecurrenceButton`
       * @config {Object<String,ContainerItemConfig|Boolean|null>}
       * @category Editor widgets
       */
      items: null,
      /**
       * The week start day used in all date fields of the feature editor form by default.
       * 0 means Sunday, 6 means Saturday.
       * Defaults to the locale's week start day.
       * @config {Number}
       */
      weekStartDay: null
    };
  }
  //endregion
  //region Init & destroy
  construct(client, config) {
    const me = this;
    client.eventEdit = me;
    super.construct(client, ObjectHelper.assign({
      weekStartDay: client.weekStartDay
    }, config));
    me.clientListenersDetacher = client.ion({
      [me.triggerEvent]: 'onActivateEditor',
      dragCreateEnd: 'onDragCreateEnd',
      // Not fired at the Scheduler level.
      // Calendar, which inherits this, implements this event.
      eventAutoCreated: 'onEventAutoCreated',
      thisObj: me
    });
  }
  doDestroy() {
    var _this$_editor;
    this.clientListenersDetacher();
    (_this$_editor = this._editor) === null || _this$_editor === void 0 ? void 0 : _this$_editor.destroy();
    super.doDestroy();
  }
  //endregion
  //region Editing
  // Not implemented at this level.
  // Scheduler Editing relies on being called at point of event creation.
  onEventAutoCreated() {}
  changeEditorConfig(editorConfig) {
    const {
      items
    } = this;
    // Merge items which is an Object with the default editorConfig's items
    if (items) {
      editorConfig = Objects.clone(editorConfig);
      editorConfig.items = Config.merge(items, editorConfig.items);
    }
    return editorConfig;
  }
  changeItems(items) {
    this.cleanItemsConfig(items);
    return items;
  }
  // Remove any items configured as === true which just means default config options
  cleanItemsConfig(items) {
    for (const ref in items) {
      const itemCfg = items[ref];
      if (itemCfg === true) {
        delete items[ref];
      } else if (itemCfg !== null && itemCfg !== void 0 && itemCfg.items) {
        this.cleanItemsConfig(itemCfg.items);
      }
    }
  }
  onDatesChange(params) {
    var _me$startTimeField, _me$startDateField2;
    const me = this,
      field = params.source,
      value = params.value;
    // End date can never be less than start date
    if (me.startDateField && me.endDateField) {
      me.endDateField.min = me.startDateField.value;
    }
    if (me.endTimeField) {
      var _me$startDateField, _me$endDateField;
      // If the event starts and ends on the same day, the time fields need
      // to have their min and max set against each other.
      if (DH.isEqual(DH.clearTime((_me$startDateField = me.startDateField) === null || _me$startDateField === void 0 ? void 0 : _me$startDateField.value), DH.clearTime((_me$endDateField = me.endDateField) === null || _me$endDateField === void 0 ? void 0 : _me$endDateField.value))) {
        me.endTimeField.min = me.startTimeField.value;
      } else {
        me.endTimeField.min = null;
      }
    }
    switch (field.ref) {
      case 'startDateField':
        ((_me$startTimeField = me.startTimeField) === null || _me$startTimeField === void 0 ? void 0 : _me$startTimeField.value) && adjustEndDate(value, me.startTimeField.value, me);
        break;
      case 'startTimeField':
        ((_me$startDateField2 = me.startDateField) === null || _me$startDateField2 === void 0 ? void 0 : _me$startDateField2.value) && adjustEndDate(me.startDateField.value, value, me);
        break;
    }
  }
  //endregion
  //region Save
  async save() {
    throw new Error('Implement in subclass');
  }
  get values() {
    const me = this,
      {
        editor
      } = me,
      startFields = [],
      endFields = [],
      {
        values
      } = editor;
    // The standard values getter will produce (almost) what we want, however, there are some special fields that
    // we need to take over. Remove those fields:
    scheduleFields.forEach(f => delete values[f]);
    editor.eachWidget(widget => {
      var _editor$widgetMap$rec;
      const {
        name
      } = widget;
      // If the widget is part of the recurrence editor, we don't gather it.
      if (!name || widget.hidden || widget.up(w => w === me.recurrenceEditor)) {
        delete values[name];
        return;
      }
      switch (name) {
        case 'startDate':
          startFields.push(widget);
          break;
        case 'endDate':
          endFields.push(widget);
          break;
        case 'resource':
          values[name] = widget.record;
          break;
        case 'recurrenceRule':
          // If recurrence set to null, completely clear the recurrenceRule.
          // Otherwise it will still be perceived as recurring with the rule 'FREQ=none'
          values[name] = ((_editor$widgetMap$rec = editor.widgetMap.recurrenceCombo) === null || _editor$widgetMap$rec === void 0 ? void 0 : _editor$widgetMap$rec.value) === 'none' ? '' : widget.value;
          break;
        // Ignore other widgets and allow the standard values getter to provide them:
        // default:
        //     values[name] = widget.value;
      }
    }, true);
    // if is changing from not allDay to allDay should consider time fields to not change them on makeDate
    if (values.allDay && !me.eventRecord.allDay) {
      startFields.push(me.startTimeField);
      endFields.push(me.endTimeField);
    }
    // Handle fields being configured away
    if (startFields.length) {
      values.startDate = makeDate(startFields);
    }
    if (endFields.length) {
      values.endDate = makeDate(endFields);
    }
    // Since there is no duration field in the editor,
    // we don't need to recalc duration value on each date change.
    // It's enough to return correct duration value in `values`,
    // so the record will get updated with the correct data.
    if ('startDate' in values && 'endDate' in values) {
      values.duration = DH.diff(values.startDate, values.endDate, me.editor.record.durationUnit, true);
    }
    return values;
  }
  /**
   * Template method, intended to be overridden. Called before the event record has been updated.
   * @param {Scheduler.model.EventModel} eventRecord The event record
   *
   **/
  onBeforeSave(eventRecord) {}
  /**
   * Template method, intended to be overridden. Called after the event record has been updated.
   * @param {Scheduler.model.EventModel} eventRecord The event record
   *
   **/
  onAfterSave(eventRecord) {}
  /**
   * Updates record being edited with values from the editor
   * @private
   */
  updateRecord(record) {
    const {
      values
    } = this;
    // Clean resourceId / resources out of values when using assignment store, it will handle the assignment
    if (this.assignmentStore) {
      delete values.resource;
    }
    return record.set(values);
  }
  //endregion
  //region Events
  onBeforeEditorShow() {
    const {
        eventRecord,
        editor
      } = this.editingContext,
      {
        nameField
      } = editor.widgetMap;
    // Editing new event. Make sure user doesn't have to clear the input field.
    // Record field value still should be there because a rendered event block
    // looks bad with no text in it.
    // nameField may have been configured away.
    if (nameField && eventRecord.isCreating) {
      // Avoid initial invalid because required state.
      editor.assigningValues = true;
      nameField.value = '';
      editor.assigningValues = false;
      // Show new event text as a placeholder
      nameField._configuredPlaceholder = nameField.placeholder;
      nameField.placeholder = eventRecord.name;
    }
  }
  resetEditingContext() {
    const me = this;
    if (!me.editingContext) {
      return;
    }
    const {
        client
      } = me,
      {
        editor,
        eventRecord
      } = me.editingContext,
      {
        eventStore
      } = client,
      {
        nameField
      } = editor.widgetMap;
    // This will remove the record from the store, *and* from the added bag, so no sync will take place.
    if (eventRecord.isCreating) {
      // Ensure that during the engine's async processing of the remove, the element is non-interactive.
      // Mousedown on the just-created element itself passes through here, and the immediate mouseup
      // after that instigates a click which will find no corresponding event.
      if (client.isTimelineBase) {
        var _me$editingContext$ev;
        (_me$editingContext$ev = me.editingContext.eventElement) === null || _me$editingContext$ev === void 0 ? void 0 : _me$editingContext$ev.closest('[data-event-id]').classList.add('b-released');
      }
      eventStore.remove(eventRecord);
      // Clear isCreating *after* removal.
      // Store doesn't register as a removed record if isCreating is set
      eventRecord.isCreating = false;
    }
    // Revert any placeholder that we may have set
    // nameField may have been configured away.
    if (nameField) {
      nameField.placeholder = nameField._configuredPlaceholder;
    }
    client.element.classList.remove('b-eventeditor-editing');
    // Reset context
    me.targetEventElement = me.editingContext = editor._record = null;
  }
  onPopupKeyDown({
    event
  }) {
    const me = this;
    if (!me.readOnly && event.key === 'Enter' && me.saveAndCloseOnEnter && event.target.tagName.toLowerCase() === 'input') {
      // Need to prevent this key events from being fired on whatever receives focus after the editor is hidden
      event.preventDefault();
      // If enter key was hit in an input element of a start field, need to adjust end date fields (the same way as if #onDatesChange handler was called)
      if (event.target.name === 'startDate') {
        me.startTimeField && adjustEndDate(me.startDateField.value, me.startTimeField.value, me);
      }
      me.onSaveClick();
    }
  }
  async finalizeStmCapture(saved) {}
  async onSaveClick() {
    this.editor.focus();
    this.isFinalizingEventSave = true;
    const saved = await this.save();
    this.isFinalizingEventSave = false;
    if (saved) {
      await this.finalizeStmCapture(false);
      this.editor.close();
      /**
       * Fires on the owning Scheduler after editor is closed by any action - save, delete or cancel
       * @event afterEventEdit
       * @on-owner
       * @param {Scheduler.view.Scheduler} source The scheduler
       */
      this.client.trigger('afterEventEdit');
    }
    return saved;
  }
  async onDeleteClick() {
    // `deleteEvent` call actually additionally closes the editor for some reason
    // see the comment for `editor.revertFocus();` call in EventEdit.js feature
    // that triggers `resetEditingContext` in which by default we assume canceling flow
    // so we need to detect that context is being reset for delete action somehow
    this.isDeletingEvent = true;
    const removed = await this.deleteEvent();
    this.isDeletingEvent = false;
    if (removed) {
      await this.finalizeStmCapture(false);
      const {
        editor
      } = this;
      // We expect deleteEvent will trigger close if autoClose is true and focus has moved out,
      // otherwise need to call it manually
      if (!editor.autoClose || editor.containsFocus) {
        editor.close();
      }
      this.client.trigger('afterEventEdit');
    }
  }
  async onCancelClick() {
    this.isCancelingEdit = true;
    this.editor.close();
    this.isCancelingEdit = false;
    if (this.hasStmCapture) {
      await this.finalizeStmCapture(true);
    }
    this.client.trigger('afterEventEdit');
  }
  //endregion
}

EditBase._$name = 'EditBase';

/**
 * @module Scheduler/view/EventEditor
 */
/**
 * Provided event editor dialog.
 *
 * @extends Core/widget/Popup
 * @private
 */
class EventEditor extends Popup {
  // Factoryable type name
  static get type() {
    return 'eventeditor';
  }
  static get $name() {
    return 'EventEditor';
  }
  static get configurable() {
    return {
      items: [],
      draggable: {
        handleSelector: ':not(button,.b-field-inner)' // Ignore buttons and field inners
      },

      axisLock: 'flexible',
      scrollable: {
        // In case editor is very tall or window is small, make it scrollable
        overflowY: true
      },
      readOnly: null,
      /**
       * A Function (or *name* of a function) which produces a customized Panel header based upon the event being edited.
       * @config {Function|String}
       * @param {Scheduler.model.EventModel} eventRecord The record being edited
       * @returns {String} The Panel title.
       */
      titleRenderer: null
    };
  }
  updateLocalization() {
    super.updateLocalization(...arguments);
    // Use this if there's no titleRenderer
    this.initialTitle = this.title || '';
  }
  chainResourceStore() {
    return this.eventEditFeature.resourceStore.chain(record => !record.isSpecialRow, null, {
      // It doesn't need to be a Project-based Store
      storeClass: Store,
      // Need to show all records in the combo. Required in case resource store is a tree.
      excludeCollapsedRecords: false
    });
  }
  processWidgetConfig(widget) {
    var _widget$type;
    if ((_widget$type = widget.type) !== null && _widget$type !== void 0 && _widget$type.includes('date') && widget.weekStartDay == null) {
      widget.weekStartDay = this.weekStartDay;
    }
    if (widget.type === 'extraItems') {
      return false;
    }
    const {
        eventEditFeature
      } = this,
      fieldConfig = {};
    if (widget.ref === 'resourceField') {
      const {
        store
      } = widget;
      // Can't use store directly since it may be grouped and then contains irrelevant group records
      widget.store = this.chainResourceStore();
      // Allow the incoming widget's config to augment its store
      if (store) {
        widget.store.setConfig(store);
      }
      // When events are loaded with resourceId, we should only support single select.
      // Only override this if the widget has not been explicitly configured
      // with multiSelect.
      if (!('multiSelect' in widget)) {
        widget.multiSelect = !eventEditFeature.eventStore.usesSingleAssignment;
      }
    }
    if ((widget.name === 'startDate' || widget.name === 'endDate') && widget.type === 'date') {
      fieldConfig.format = eventEditFeature.dateFormat;
    }
    if ((widget.name === 'startDate' || widget.name === 'endDate') && widget.type === 'time') {
      fieldConfig.format = eventEditFeature.timeFormat;
    }
    Object.assign(widget, fieldConfig);
    return super.processWidgetConfig(widget);
  }
  onBeforeShow(...args) {
    var _super$onBeforeShow;
    const me = this,
      {
        record,
        titleRenderer
      } = me,
      {
        deleteButton
      } = me.widgetMap;
    // Hide delete button if we are readOnly or the event is in a create phase
    // which means we are editing a dblclick-created or drag-created event.
    if (deleteButton) {
      deleteButton.hidden = me.readOnly || record.isCreating;
    }
    if (titleRenderer) {
      me.title = me.callback(titleRenderer, me, [record]);
    } else {
      me.title = me.initialTitle;
    }
    (_super$onBeforeShow = super.onBeforeShow) === null || _super$onBeforeShow === void 0 ? void 0 : _super$onBeforeShow.call(this, ...args);
  }
  onInternalKeyDown(event) {
    this.trigger('keyDown', {
      event
    });
    super.onInternalKeyDown(event);
  }
  updateReadOnly(readOnly) {
    const {
      deleteButton,
      saveButton,
      cancelButton
    } = this.widgetMap;
    super.updateReadOnly(readOnly);
    if (deleteButton) {
      deleteButton.hidden = readOnly;
    }
    if (saveButton) {
      saveButton.hidden = readOnly;
    }
    if (cancelButton) {
      cancelButton.hidden = readOnly;
    }
  }
}
// Register this widget type with its Factory
EventEditor.initClass();
EventEditor._$name = 'EventEditor';

/**
 * @module Scheduler/view/recurrence/field/RecurrenceCombo
 */
/**
 * A combobox field for selecting a recurrence pattern: `Daily`, `Weekly`, `Monthly` or `Yearly` if the recurrence
 * has no other non-default settings, or `Custom...` if the recurrence has custom setting applied.
 *
 * {@inlineexample Scheduler/view/RecurrenceCombo.js}
 *
 * @extends Scheduler/view/recurrence/field/RecurrenceFrequencyCombo
 * @classType recurrencecombo
 */
class RecurrenceCombo extends RecurrenceFrequencyCombo {
  static get $name() {
    return 'RecurrenceCombo';
  }
  // Factoryable type name
  static get type() {
    return 'recurrencecombo';
  }
  static get defaultConfig() {
    return {
      customValue: 'custom',
      placeholder: 'None',
      splitCls: 'b-recurrencecombo-split',
      items: true,
      highlightExternalChange: false
    };
  }
  buildItems() {
    const me = this;
    return [{
      value: 'none',
      text: 'L{None}'
    }, ...super.buildItems(), {
      value: me.customValue,
      text: 'L{Custom}',
      cls: me.splitCls
    }];
  }
  set value(value) {
    // Use 'none' instead of falsy value
    value = value || 'none';
    super.value = value;
  }
  get value() {
    return super.value;
  }
  set recurrence(recurrence) {
    const me = this;
    if (recurrence) {
      me.value = me.isCustomRecurrence(recurrence) ? me.customValue : recurrence.frequency;
    } else {
      me.value = null;
    }
  }
  isCustomRecurrence(recurrence) {
    const {
      interval,
      days,
      monthDays,
      months
    } = recurrence;
    return Boolean(interval > 1 || days && days.length || monthDays && monthDays.length || months && months.length);
  }
}
// Register this widget type with its Factory
RecurrenceCombo.initClass();
RecurrenceCombo._$name = 'RecurrenceCombo';

/**
 * @module Scheduler/view/recurrence/RecurrenceLegendButton
 */
/**
 * A button which displays the associated {@link #property-recurrence} info in a human readable form.
 * @extends Core/widget/Button
 * @classType recurrencelegendbutton
 */
class RecurrenceLegendButton extends Button {
  static get $name() {
    return 'RecurrenceLegendButton';
  }
  // Factoryable type name
  static get type() {
    return 'recurrencelegendbutton';
  }
  static get defaultConfig() {
    return {
      localizableProperties: [],
      recurrence: null
    };
  }
  /**
   * Sets / gets the recurrence to display description for.
   * @property {Scheduler.model.RecurrenceModel}
   */
  set recurrence(recurrence) {
    this._recurrence = recurrence;
    this.updateLegend();
  }
  get recurrence() {
    return this._recurrence;
  }
  set eventStartDate(eventStartDate) {
    this._eventStartDate = eventStartDate;
    this.updateLegend();
  }
  get eventStartDate() {
    return this._eventStartDate;
  }
  updateLegend() {
    const {
      recurrence
    } = this;
    this.text = recurrence ? RecurrenceLegend.getLegend(recurrence, this.eventStartDate) : '';
  }
  onLocaleChange() {
    // on locale switch we update the button text to use proper language
    this.updateLegend();
  }
  updateLocalization() {
    this.onLocaleChange();
    super.updateLocalization();
  }
}
// Register this widget type with its Factory
RecurrenceLegendButton.initClass();
RecurrenceLegendButton._$name = 'RecurrenceLegendButton';

/**
 * @module Scheduler/view/recurrence/RecurrenceEditor
 */
/**
 * Class implementing a dialog to edit a {@link Scheduler.model.RecurrenceModel recurrence model}. The class is used by
 * the {@link Scheduler.view.mixin.RecurringEvents recurring events} feature, and you normally don't need to instantiate
 * it.
 *
 * Before showing the dialog need to use {@link Core.widget.Container#property-record} to load a
 * {@link Scheduler.model.RecurrenceModel recurrence model} data into the editor fields. For example:
 *
 * ```javascript
 * // make the editor instance
 * const editor = new RecurrenceEditor();
 * // load recurrence model into it
 * editor.record = new RecurrenceModel({ frequency : "WEEKLY" });
 * // display the editor
 * editor.show();
 * ```
 *
 * @extends Core/widget/Popup
 * @classType recurrenceeditor
 */
class RecurrenceEditor extends Popup {
  static get $name() {
    return 'RecurrenceEditor';
  }
  // Factoryable type name
  static get type() {
    return 'recurrenceeditor';
  }
  static get configurable() {
    return {
      draggable: true,
      closable: true,
      floating: true,
      cls: 'b-recurrenceeditor',
      title: 'L{Repeat event}',
      autoClose: true,
      width: 470,
      items: {
        recurrenceEditorPanel: {
          type: 'recurrenceeditorpanel',
          title: null
        }
      },
      bbar: {
        defaults: {
          localeClass: this
        },
        items: {
          foo: {
            type: 'widget',
            cls: 'b-label-filler',
            weight: 100
          },
          saveButton: {
            color: 'b-green',
            text: 'L{Save}',
            onClick: 'up.onSaveClick',
            weight: 200
          },
          cancelButton: {
            color: 'b-gray',
            text: 'L{Object.Cancel}',
            onClick: 'up.onCancelClick',
            weight: 300
          }
        }
      },
      scrollable: {
        overflowY: true
      }
    };
  }
  updateReadOnly(readOnly) {
    super.updateReadOnly(readOnly);
    // No save or cancel buttons. It's purely for information display when in readOnly mode
    this.bbar.hidden = readOnly;
  }
  get recurrenceEditorPanel() {
    return this.widgetMap.recurrenceEditorPanel;
  }
  updateRecord(record) {
    this.recurrenceEditorPanel.record = record;
  }
  onSaveClick() {
    const me = this;
    if (me.saveHandler) {
      me.saveHandler.call(me.thisObj || me, me, me.record);
    } else {
      me.recurrenceEditorPanel.syncEventRecord();
      me.close();
    }
  }
  onCancelClick() {
    const me = this;
    if (me.cancelHandler) {
      me.cancelHandler.call(me.thisObj || me, me, me.record);
    } else {
      me.close();
    }
  }
}
// Register this widget type with its Factory
RecurrenceEditor.initClass();
RecurrenceEditor._$name = 'RecurrenceEditor';

/**
 * @module Scheduler/feature/mixin/RecurringEventEdit
 */
/**
 * This mixin class provides recurring events functionality to the {@link Scheduler.feature.EventEdit event editor}.
 * @mixin
 */
var RecurringEventEdit = (Target => class RecurringEventEdit extends (Target || Base) {
  static get $name() {
    return 'RecurringEventEdit';
  }
  static get configurable() {
    return {
      recurringEventsItems: {
        /**
         * Reference to the `Repeat` event field, if used
         * @member {Scheduler.view.recurrence.field.RecurrenceCombo} recurrenceCombo
         * @readonly
         */
        recurrenceCombo: {
          type: 'recurrencecombo',
          label: 'L{EventEdit.Repeat}',
          ref: 'recurrenceCombo',
          weight: 700
        },
        /**
         * Reference to the button that opens the event repeat settings dialog, if used
         * @member {Scheduler.view.recurrence.RecurrenceLegendButton} editRecurrenceButton
         * @readonly
         */
        editRecurrenceButton: {
          type: 'recurrencelegendbutton',
          ref: 'editRecurrenceButton',
          name: 'recurrenceRule',
          color: 'b-gray',
          menuIcon: null,
          flex: 1,
          weight: 800,
          ignoreParentReadOnly: true
        }
      },
      /**
       * Set to `false` to hide recurring fields in event editor, even if the
       * {@link Scheduler.view.mixin.RecurringEvents#config-enableRecurringEvents Recurring Events} is `true`
       * and a recurring event is being edited.
       * @config {Boolean}
       * @category Recurring
       */
      showRecurringUI: null
    };
  }
  changeEditorConfig(editorConfig) {
    editorConfig.items = {
      ...editorConfig.items,
      ...this.recurringEventsItems
    };
    // EditBase inserts extraItems *after* all default items are in
    editorConfig = super.changeEditorConfig(editorConfig);
    return editorConfig;
  }
  construct(scheduler, config) {
    super.construct(scheduler, config);
    this.scheduler.ion({
      beforeEventSave: 'onRecurrableEventBeforeSave',
      thisObj: this
    });
  }
  doDestroy() {
    var _this$_recurrenceConf, _this$_recurrenceEdit;
    (_this$_recurrenceConf = this._recurrenceConfirmation) === null || _this$_recurrenceConf === void 0 ? void 0 : _this$_recurrenceConf.destroy();
    (_this$_recurrenceEdit = this._recurrenceEditor) === null || _this$_recurrenceEdit === void 0 ? void 0 : _this$_recurrenceEdit.destroy();
    super.doDestroy();
  }
  onEditorConstructed(editor) {
    var _me$recurrenceCombo;
    const me = this;
    editor.ion({
      hide: me.onRecurringEventEditorHide,
      thisObj: me
    });
    if (me.editRecurrenceButton) {
      me.editRecurrenceButton.menu = me.recurrenceEditor;
    }
    (_me$recurrenceCombo = me.recurrenceCombo) === null || _me$recurrenceCombo === void 0 ? void 0 : _me$recurrenceCombo.ion({
      change: me.onRecurrenceComboChange,
      thisObj: me
    });
  }
  updateReadOnly(readOnly) {
    if (this._recurrenceEditor) {
      this._recurrenceEditor.readOnly = readOnly;
    }
  }
  internalShowEditor() {
    this.toggleRecurringFieldsVisibility(this.client.enableRecurringEvents && this.showRecurringUI !== false);
  }
  toggleRecurringFieldsVisibility(show = true) {
    var _this$editRecurrenceB, _this$editRecurrenceB2, _this$recurrenceCombo, _this$recurrenceCombo2;
    const methodName = show ? 'show' : 'hide';
    (_this$editRecurrenceB = this.editRecurrenceButton) === null || _this$editRecurrenceB === void 0 ? void 0 : (_this$editRecurrenceB2 = _this$editRecurrenceB[methodName]) === null || _this$editRecurrenceB2 === void 0 ? void 0 : _this$editRecurrenceB2.call(_this$editRecurrenceB);
    (_this$recurrenceCombo = this.recurrenceCombo) === null || _this$recurrenceCombo === void 0 ? void 0 : (_this$recurrenceCombo2 = _this$recurrenceCombo[methodName]) === null || _this$recurrenceCombo2 === void 0 ? void 0 : _this$recurrenceCombo2.call(_this$recurrenceCombo);
  }
  onRecurringEventEditorHide() {
    var _this$recurrenceEdito, _this$recurrenceConfi;
    if ((_this$recurrenceEdito = this.recurrenceEditor) !== null && _this$recurrenceEdito !== void 0 && _this$recurrenceEdito.isVisible) {
      this.recurrenceEditor.hide();
    }
    if ((_this$recurrenceConfi = this.recurrenceConfirmation) !== null && _this$recurrenceConfi !== void 0 && _this$recurrenceConfi.isVisible) {
      this.recurrenceConfirmation.hide();
    }
  }
  // Builds RecurrenceModel to load into the recurrenceEditor
  // It builds the model based on either:
  // - recurrence rule string (if provided)
  // - or the event being edited recurrence (if the event is repeating)
  // - or simply make a recurrence model w/ default state (by default means: Frequency=Daily, Interval=1)
  makeRecurrence(rule) {
    const event = this.eventRecord,
      eventCopy = event.copy();
    let recurrence = event.recurrence;
    if (!rule && recurrence) {
      recurrence = recurrence.copy();
    } else {
      recurrence = new event.recurrenceModel(rule ? {
        rule
      } : {});
    }
    // bind cloned recurrence to the cloned event
    recurrence.timeSpan = eventCopy;
    // update cloned event w/ start date from the UI field
    eventCopy.setStartDate(this.values.startDate);
    recurrence.suspendTimeSpanNotifying();
    return recurrence;
  }
  onRecurrableEventBeforeSave({
    eventRecord,
    context
  }) {
    const me = this;
    // Other views features may trigger beforeEventSave, so only react when *we* are editing.
    if (me.isEditing && !eventRecord.isCreating && eventRecord.supportsRecurring && (eventRecord.isRecurring || eventRecord.isOccurrence)) {
      me.recurrenceConfirmation.confirm({
        actionType: 'update',
        eventRecord,
        changerFn() {
          context.finalize(true);
        },
        cancelFn() {
          context.finalize(false);
        }
      });
      // signalizes that we plan to decide save or not asynchronously
      context.async = true;
    }
  }
  set recurrenceConfirmation(recurrenceConfirmation) {
    this._recurrenceConfirmation = recurrenceConfirmation;
  }
  get recurrenceConfirmation() {
    const me = this;
    let recurrenceConfirmation = me._recurrenceConfirmation;
    if (!recurrenceConfirmation || !recurrenceConfirmation.$$name) {
      recurrenceConfirmation = Widget.create({
        type: 'recurrenceconfirmation',
        owner: me.editor,
        ...recurrenceConfirmation
      });
      me._recurrenceConfirmation = recurrenceConfirmation;
    }
    return recurrenceConfirmation;
  }
  set recurrenceEditor(recurrenceEditor) {
    this._recurrenceEditor = recurrenceEditor;
  }
  get recurrenceEditor() {
    const me = this;
    let recurrenceEditor = me._recurrenceEditor;
    // Recurrence editor is centered and modal.
    if (!recurrenceEditor || !recurrenceEditor.$$name) {
      me._recurrenceEditor = recurrenceEditor = Widget.create({
        type: 'recurrenceeditor',
        autoShow: false,
        centered: true,
        modal: true,
        constrainTo: globalThis,
        anchor: false,
        rootElement: me.rootElement,
        saveHandler: me.recurrenceEditorSaveHandler,
        onBeforeShow: me.onBeforeShowRecurrenceEditor.bind(me),
        thisObj: me,
        ...recurrenceEditor
      });
      // Must set *after* construction, otherwise it becomes the default state
      // to reset readOnly back to.  Must use direct property access because
      // getter consults state of editor.
      recurrenceEditor.readOnly = me._readOnly;
    }
    return recurrenceEditor;
  }
  onBeforeShowRecurrenceEditor() {
    const me = this,
      {
        recurrenceEditor,
        eventRecord
      } = me;
    if (recurrenceEditor && eventRecord !== null && eventRecord !== void 0 && eventRecord.supportsRecurring) {
      // if the event has no recurrence yet ..initialize it before showing recurrence editor
      if (!me.recurrence) {
        me.recurrence = me.makeRecurrence();
      }
      // update the cloned recurrence w/ up to date start date value
      me.recurrence.timeSpan.setStartDate(me.values.startDate);
      // load RecurrenceModel record into the recurrence editor
      recurrenceEditor.record = me.recurrence;
      // In case they drag it. Centered falls off if the widget has position set.
      recurrenceEditor.centered = true;
    }
  }
  loadRecurrenceData(recurrence) {
    this.recurrence = recurrence;
    this.updateRecurrenceFields(recurrence);
  }
  updateRecurrenceFields(recurrence) {
    const me = this,
      {
        editRecurrenceButton
      } = me;
    if (me.recurrenceCombo) {
      me.recurrenceCombo.recurrence = recurrence;
    }
    // update the recurrence legend
    if (editRecurrenceButton) {
      editRecurrenceButton.recurrence = recurrence;
      editRecurrenceButton.value = recurrence ? recurrence.rule : null;
      if (recurrence && me.client.enableRecurringEvents && me.showRecurringUI !== false) {
        editRecurrenceButton.show();
      } else {
        editRecurrenceButton.hide();
      }
    }
  }
  onRecurrenceComboChange({
    source,
    value,
    userAction
  }) {
    if (userAction) {
      const me = this,
        {
          recurrenceEditor
        } = me;
      if (value === source.customValue) {
        // if user picked "Custom" - show recurrence editor
        // This will recurse through the change event into the opposite side
        // of the value test which will call updateRecurrenceFields, where the
        // assignment to the value of the recurrenceCombo will be a non-change.
        // That will sync the state of the recurrenceButton.
        me.recurrenceCombo.recurrence = me.makeRecurrence();
        if (recurrenceEditor.centered) {
          recurrenceEditor.show();
        } else {
          recurrenceEditor.show((me.editRecurrenceButton || source).element);
        }
      }
      // user has picked some frequency -> make a new recurrence based on it
      else {
        me.loadRecurrenceData(value && value !== 'none' ? me.makeRecurrence(`FREQ=${value}`) : null);
      }
    }
  }
  recurrenceEditorSaveHandler(editor, recurrence) {
    // apply changes to the kept recurrence
    editor.recurrenceEditorPanel.syncEventRecord(recurrence);
    // update the recurrence related UI
    this.updateRecurrenceFields(recurrence);
    editor.close();
  }
  onDatesChange(...args) {
    super.onDatesChange(...args);
    if (!this.loadingRecord && this.editRecurrenceButton) {
      const {
        startDate
      } = this.values;
      if (startDate) {
        this.editRecurrenceButton.eventStartDate = startDate;
      }
    }
  }
  internalLoadRecord(eventRecord) {
    if (eventRecord !== null && eventRecord !== void 0 && eventRecord.supportsRecurring) {
      this.loadRecurrenceData(eventRecord.recurrence ? this.makeRecurrence() : null);
    }
  }
  updateRecord(record) {
    // Special handling for when setting recurrence to "None". Since button gets hidden its value is not picked up
    // by the normal flow.
    if (record.recurrenceRule && !this.recurrence) {
      record.recurrenceRule = null;
    }
    return super.updateRecord(record);
  }
});

/**
 * @module Scheduler/widget/ResourceCombo
 */
/**
 * A Combo subclass which selects resources, optionally displaying the {@link Scheduler.model.ResourceModel#field-eventColor}
 * of each resource in the picker and in the input area.
 *
 * {@inlineexample Scheduler/widget/ResourceCombo.js}
 *
 * @extends Core/widget/Combo
 * @classType resourcecombo
 * @inputfield
 */
class ResourceCombo extends Combo {
  static get $name() {
    return 'ResourceCombo';
  }
  // Factoryable type name
  static get type() {
    return 'resourcecombo';
  }
  static get configurable() {
    return {
      /**
       * Show the {@link Scheduler.model.ResourceModel#field-eventColor event color} for each resource
       * @config {Boolean}
       * @default
       */
      showEventColor: false,
      displayField: 'name',
      valueField: 'id',
      picker: {
        cls: 'b-resourcecombo-picker',
        itemIconTpl(record) {
          const {
              eventColor
            } = record,
            isStyleColor = !DomHelper.isNamedColor(eventColor),
            style = eventColor ? isStyleColor ? ` style="color:${eventColor}"` : '' : ' style="display:none"',
            colorClass = !eventColor || isStyleColor ? '' : ` b-sch-foreground-${eventColor}`;
          return `<div class="b-icon b-icon-square${colorClass}"${style}></div>`;
        }
      }
    };
  }
  changeShowEventColor(showEventColor) {
    return Boolean(showEventColor);
  }
  updateShowEventColor(showEventColor) {
    const {
        _picker
      } = this,
      methodName = showEventColor ? 'add' : 'remove';
    this.element.classList[methodName]('b-show-event-color');
    _picker === null || _picker === void 0 ? void 0 : _picker.element.classList[methodName]('b-show-event-color');
  }
  changePicker(picker, oldPicker) {
    var _picker2;
    picker = super.changePicker(picker, oldPicker);
    (_picker2 = picker) === null || _picker2 === void 0 ? void 0 : _picker2.element.classList[this.showEventColor ? 'add' : 'remove']('b-show-event-color');
    return picker;
  }
  // Implementation needed at this level because it has two inner elements in its inputWrap
  get innerElements() {
    return [{
      class: 'b-icon b-resource-icon b-icon-square b-hide-display',
      reference: 'resourceIcon'
    }, this.inputElement];
  }
  syncInputFieldValue() {
    var _me$selected;
    const me = this,
      {
        resourceIcon,
        lastResourceIconCls
      } = me,
      {
        classList
      } = resourceIcon,
      eventColor = ((_me$selected = me.selected) === null || _me$selected === void 0 ? void 0 : _me$selected.eventColor) ?? '';
    super.syncInputFieldValue();
    // Remove last colour whichever way it was done
    resourceIcon.style.color = '';
    lastResourceIconCls && classList.remove(lastResourceIconCls);
    me.lastResourceIconCls = null;
    if (eventColor) {
      if (DomHelper.isNamedColor(eventColor)) {
        me.lastResourceIconCls = `b-sch-foreground-${eventColor}`;
        classList.add(me.lastResourceIconCls);
      } else {
        resourceIcon.style.color = eventColor;
      }
      classList.remove('b-hide-display');
    } else {
      classList.add('b-hide-display');
    }
  }
}
// Register this widget type with its Factory
ResourceCombo.initClass();
ResourceCombo._$name = 'ResourceCombo';

/**
 * @module Scheduler/feature/EventEdit
 */
const punctuation = /[^\w\d]/g;
/**
 * Feature that displays a popup containing widgets for editing event data.
 *
 * {@inlineexample Scheduler/feature/EventEdit.js}
 *
 * To customize its contents you can:
 *
 * * Reconfigure built in widgets by providing override configs in the {@link Scheduler.feature.base.EditBase#config-items} config.
 * * Change the date format of the date & time fields: {@link Scheduler.feature.base.EditBase#config-dateFormat} and {@link Scheduler.feature.base.EditBase#config-timeFormat }
 * * Configure provided widgets in the editor and add your own in the {@link Scheduler.feature.base.EditBase#config-items} config.
 * * Remove fields related to recurring events configuration (such as `recurrenceCombo`) by setting {@link Scheduler.feature.mixin.RecurringEventEdit#config-showRecurringUI} config to `false`.
 * * Advanced: Reconfigure the whole editor widget using {@link #config-editorConfig}
 *
 * ## Built in widgets
 *
 * The built in widgets are:
 *
 * | Widget ref             | Type                                                     | Weight | Description                                                    |
 * |------------------------|----------------------------------------------------------|--------|----------------------------------------------------------------|
 * | `nameField`            | {@link Core.widget.TextField}                            | 100    | Edit name                                                      |
 * | `resourceField`        | {@link Scheduler.widget.ResourceCombo}                   | 200    | Pick resource(s)                                               |
 * | `startDateField`       | {@link Core.widget.DateField}                            | 300    | Edit startDate (date part)                                     |
 * | `startTimeField`       | {@link Core.widget.TimeField}                            | 400    | Edit startDate (time part)                                     |
 * | `endDateField`         | {@link Core.widget.DateField}                            | 500    | Edit endDate (date part)                                       |
 * | `endTimeField`         | {@link Core.widget.TimeField}                            | 600    | Edit endDate (time part)                                       |
 * | `recurrenceCombo`      | {@link Scheduler.view.recurrence.field.RecurrenceCombo}  | 700    | Select recurrence rule (only visible if recurrence is used)    |
 * | `editRecurrenceButton` | {@link Scheduler.view.recurrence.RecurrenceLegendButton} | 800    | Edit the recurrence rule  (only visible if recurrence is used) |
 *
 * The built in buttons are:
 *
 * | Widget ref             | Type                                                                     | Weight | Description                                                    |
 * |------------------------|--------------------------------------------------------------------------|--------|----------------------------------------------------------------|
 * | `saveButton`           | {@link Core.widget.Button}                                               | 100    | Save event button on the bbar                                  |
 * | `deleteButton`         | {@link Core.widget.Button}                                               | 200    | Delete event button on the bbar                                |
 * | `cancelButton`         | {@link Core.widget.Button}                                               | 300    | Cancel event editing button on the bbar                        |
 *
 * ## Removing a built in item
 *
 * To remove a built in widget, specify its `ref` as `null` in the `items` config:
 *
 * ```javascript
 * const scheduler = new Scheduler({
 *     features : {
 *         eventEdit : {
 *             items : {
 *                 // Remove the start time field
 *                 startTimeField : null
 *             }
 *         }
 *     }
 * })
 * ```
 *
 * Bottom buttons may be hidden using `bbar` config passed to `editorConfig`:
 * ```javascript
 * const scheduler = new Scheduler({
 *     features : {
 *         eventEdit : {
 *             editorConfig : {
 *                 bbar : {
 *                     items : {
 *                         deleteButton : null
 *                     }
 *                 }
 *             }
 *         }
 *     }
 * })
 * ```
 *
 * To remove fields related to recurring events configuration (such as `recurrenceCombo`), set {@link Scheduler.feature.mixin.RecurringEventEdit#config-showRecurringUI} config to `false`.
 *
 * ## Customizing a built in widget
 *
 * To customize a built in widget, use its `ref` as the key in the `items` config and specify the configs you want
 * to change (they will merge with the widgets default configs):
 *
 * ```javascript
 * const scheduler = new Scheduler({
 *     features : {
 *         eventEdit : {
 *             items : {
 *                 // ref for an existing field
 *                 nameField : {
 *                     // Change its label
 *                     label : 'Description'
 *                 }
 *             }
 *         }
 *     }
 * })
 * ```
 *
 * ## Adding custom widgets
 *
 * To add a custom widget, add an entry to the `items` config. The `name` property links the input field to a field in
 * the loaded event record:
 *
 * ```javascript
 * const scheduler = new Scheduler({
 *     features : {
 *         eventEdit : {
 *             items : {
 *                 // Key to use as fields ref (for easier retrieval later)
 *                 color : {
 *                     type  : 'combo',
 *                     label : 'Color',
 *                     items : ['red', 'green', 'blue'],
 *                     // name will be used to link to a field in the event record when loading and saving in the editor
 *                     name  : 'eventColor'
 *                 }
 *             }
 *         }
 *     }
 * })
 * ```
 *
 * For more info on customizing the event editor, please see "Customize event editor" guide.
 *
 * This feature is **enabled** by default
 *
 * @mixes Scheduler/feature/mixin/RecurringEventEdit
 * @extends Scheduler/feature/base/EditBase
 * @demo Scheduler/eventeditor
 * @classtype eventEdit
 * @feature
 */
class EventEdit extends EditBase.mixin(TaskEditStm, RecurringEventEdit, Delayable) {
  //region Config
  static get $name() {
    return 'EventEdit';
  }
  static get configurable() {
    return {
      /**
       * The event that shall trigger showing the editor. Defaults to `eventdblclick`, set to `''` or null to
       * disable editing of existing events.
       * @config {String}
       * @default
       * @category Editor
       */
      triggerEvent: 'eventdblclick',
      /**
       * The data field in the model that defines the eventType.
       * Applied as class (b-eventtype-xx) to the editors element, to allow showing/hiding fields depending on
       * eventType. Dynamic toggling of fields in the editor is activated by adding an `eventTypeField` field to
       * your widget:
       *
       * ```javascript
       * const scheduler = new Scheduler({
       *    features : {
       *       eventEdit : {
       *           items : {
       *               eventTypeField : {
       *                  type  : 'combo',
       *                  name  : 'eventType',
       *                  label : 'Type',
       *                  items : ['Appointment', 'Internal', 'Meeting']
       *               }
       *           }
       *        }
       *     }
       * });
       * ```
       * Note, your event model class also must declare this field:
       * ```javascript
       *  class MyEvent extends EventModel {
       *      static get fields() {
       *          return [
       *              { name : 'eventType' }
       *          ];
       *      }
       *  }
       * ```
       * @config {String}
       * @default
       * @category Editor
       */
      typeField: 'eventType',
      /**
       * The current {@link Scheduler.model.EventModel} record, which is being edited by the event editor.
       * @property {Scheduler.model.EventModel}
       * @readonly
       */
      eventRecord: null,
      /**
       * Specify `true` to put the editor in read only mode.
       * @config {Boolean}
       * @default false
       */
      readOnly: null,
      /**
       * The configuration for the internal editor widget. With this config you can control the *type*
       * of editor (defaults to `Popup`) and which widgets to show,
       * change the items in the `bbar`, or change whether the popup should be modal etc.
       *
       * ```javascript
       * const scheduler = new Scheduler({
       *     features : {
       *         eventEdit  : {
       *             editorConfig : {
       *                 modal  : true,
       *                 cls    : 'my-editor' // A CSS class,
       *                 items  : {
       *                     owner : {
       *                         weight : -100, // Will sort above system-supplied fields which are weight 100 to 800
       *                         type   : 'usercombo',
       *                         name   : 'owner',
       *                         label  : 'Owner'
       *                     },
       *                     agreement : {
       *                         weight : 1000, // Will sort below system-supplied fields which are weight 100 to 800
       *                         type   : 'checkbox',
       *                         name   : 'agreement',
       *                         label  : 'Agree to terms'
       *                     },
       *                     resourceField : {
       *                         // Apply a special filter to limit the Combo's access
       *                         // to resources.
       *                         store  {
       *                             filters : [{
       *                                 filterBy(resource) {
       *                                     return shouldShowResource(record);
       *                                 }
       *                             }]
       *                         }
       *                     }
       *                 },
       *                 bbar : {
       *                     items : {
       *                         deleteButton : {
       *                             hidden : true
       *                         }
       *                     }
       *                 }
       *             }
       *         }
       *     }
       * });
       * ```
       *
       * Or to use your own custom editor:
       *
       * ```javascript
       * const scheduler = new Scheduler({
       *     features : {
       *         eventEdit  : {
       *             editorConfig : {
       *                 type : 'myCustomEditorType'
       *             }
       *         }
       *     }
       * });
       * ```
       * @config {Object}
       * @category Editor
       */
      editorConfig: {
        type: 'eventeditor',
        title: 'L{EventEdit.Edit event}',
        closable: true,
        localeClass: this,
        defaults: {
          localeClass: this
        },
        items: {
          /**
           * Reference to the name field, if used
           * @member {Core.widget.TextField} nameField
           * @readonly
           */
          nameField: {
            type: 'text',
            label: 'L{Name}',
            clearable: true,
            name: 'name',
            weight: 100,
            required: true
          },
          /**
           * Reference to the resource field, if used
           * @member {Core.widget.Combo} resourceField
           * @readonly
           */
          resourceField: {
            type: 'resourcecombo',
            label: 'L{Resource}',
            name: 'resource',
            editable: true,
            valueField: 'id',
            displayField: 'name',
            highlightExternalChange: false,
            destroyStore: true,
            weight: 200
          },
          /**
           * Reference to the start date field, if used
           * @member {Core.widget.DateField} startDateField
           * @readonly
           */
          startDateField: {
            type: 'date',
            cls: 'b-inline',
            clearable: false,
            required: true,
            label: 'L{Start}',
            name: 'startDate',
            validateDateOnly: true,
            weight: 300
          },
          /**
           * Reference to the start time field, if used
           * @member {Core.widget.TimeField} startTimeField
           * @readonly
           */
          startTimeField: {
            type: 'time',
            clearable: false,
            required: true,
            name: 'startDate',
            cls: 'b-match-label',
            weight: 400
          },
          /**
           * Reference to the end date field, if used
           * @member {Core.widget.DateField} endDateField
           * @readonly
           */
          endDateField: {
            type: 'date',
            cls: 'b-inline',
            clearable: false,
            required: true,
            label: 'L{End}',
            name: 'endDate',
            validateDateOnly: true,
            weight: 500
          },
          /**
           * Reference to the end time field, if used
           * @member {Core.widget.TimeField} endTimeField
           * @readonly
           */
          endTimeField: {
            type: 'time',
            clearable: false,
            required: true,
            name: 'endDate',
            cls: 'b-match-label',
            weight: 600
          }
        },
        bbar: {
          // When readOnly, child buttons are hidden
          hideWhenEmpty: true,
          defaults: {
            localeClass: this
          },
          items: {
            /**
             * Reference to the save button, if used
             * @member {Core.widget.Button} saveButton
             * @readonly
             */
            saveButton: {
              color: 'b-blue',
              cls: 'b-raised',
              text: 'L{Save}',
              weight: 100
            },
            /**
             * Reference to the delete button, if used
             * @member {Core.widget.Button} deleteButton
             * @readonly
             */
            deleteButton: {
              text: 'L{Delete}',
              weight: 200
            },
            /**
             * Reference to the cancel button, if used
             * @member {Core.widget.Button} cancelButton
             * @readonly
             */
            cancelButton: {
              text: 'L{Object.Cancel}',
              weight: 300
            }
          }
        }
      },
      targetEventElement: null
    };
  }
  static get pluginConfig() {
    return {
      chain: ['populateEventMenu', 'onEventEnterKey', 'editEvent']
    };
  }
  //endregion
  //region Init & destroy
  construct(scheduler, config) {
    // Default to the scheduler's state, but configs may override
    this.readOnly = scheduler.readOnly;
    super.construct(scheduler, config);
    scheduler.ion({
      projectChange: 'onChangeProject',
      readOnly: 'onClientReadOnlyToggle',
      thisObj: this
    });
  }
  get scheduler() {
    return this.client;
  }
  get project() {
    return this.client.project;
  }
  //endregion
  //region Editing
  /**
   * Get/set readonly state
   * @property {Boolean}
   */
  get readOnly() {
    return this._editor ? this.editor.readOnly : this._readOnly;
  }
  updateReadOnly(readOnly) {
    super.updateReadOnly(readOnly);
    if (this._editor) {
      this.editor.readOnly = readOnly;
    }
  }
  onClientReadOnlyToggle({
    readOnly
  }) {
    this.readOnly = readOnly;
  }
  /**
   * Returns the editor widget representing this feature
   * @member {Core.widget.Popup}
   */
  get editor() {
    var _me$onEditorConstruct, _me$eventTypeField, _me$saveButton, _me$deleteButton, _me$cancelButton;
    const me = this,
      editorListeners = {
        beforehide: 'resetEditingContext',
        beforeshow: 'onBeforeEditorShow',
        keydown: 'onPopupKeyDown',
        thisObj: me
      };
    let {
      _editor: editor
    } = me;
    if (editor) {
      return editor;
    }
    editor = me._editor = Widget.create(me.getEditorConfig());
    const {
      startDateField,
      startTimeField,
      endDateField,
      endTimeField
    } = editor.widgetMap;
    // If the date field doesn't exist, the time field must encapsulate the
    // date component of the start/end points and must lay out right.
    if (!startDateField && startTimeField) {
      startTimeField.keepDate = true;
      startTimeField.label = me.L('Start');
      startTimeField.flex = '1 0 100%';
    }
    if (!endDateField && endTimeField) {
      endTimeField.keepDate = true;
      endTimeField.label = me.L('End');
      endTimeField.flex = '1 0 100%';
    }
    // If the default Popup has been reconfigured to be static, add it as a child of our client.
    if (!editor.floating && !editor.positioned) {
      // If not configured with an appendTo, we add it as a child of our client.
      if (!editor.element.parentNode) {
        me.client.add(editor);
      }
      delete editorListeners.beforehide;
      delete editorListeners.beforShow;
      editorListeners.beforeToggleReveal = 'onBeforeEditorToggleReveal';
    }
    // Must set *after* construction, otherwise it becomes the default state
    // to reset readOnly back to. Must use direct property access because
    // getter consults state of editor.
    editor.readOnly = me._readOnly;
    if (editor.items.length === 0) {
      console.warn('Event Editor configured without any `items`');
    }
    // add listeners programmatically so users cannot override them accidentally
    editor.ion(editorListeners);
    /**
     * Fired before the editor will load the event record data into its input fields. This is useful if you
     * want to modify the fields before data is loaded (e.g. set some input field to be readonly)
     * @on-owner
     * @event eventEditBeforeSetRecord
     * @param {Core.widget.Container} source The editor widget
     * @param {Scheduler.model.EventModel} record The record
     */
    me.scheduler.relayEvents(editor, ['beforeSetRecord'], 'eventEdit');
    // assign widget variables, using widget name: startDate -> me.startDateField
    // widgets with id set use that instead, id -> me.idField
    Object.values(editor.widgetMap).forEach(widget => {
      const ref = widget.ref || widget.id;
      // don't overwrite if already defined
      if (ref && !me[ref]) {
        me[ref] = widget;
        switch (widget.name) {
          case 'startDate':
          case 'endDate':
            widget.ion({
              change: 'onDatesChange',
              thisObj: me
            });
            break;
        }
      }
    });
    // launch onEditorConstructed hook if provided
    (_me$onEditorConstruct = me.onEditorConstructed) === null || _me$onEditorConstruct === void 0 ? void 0 : _me$onEditorConstruct.call(me, editor);
    (_me$eventTypeField = me.eventTypeField) === null || _me$eventTypeField === void 0 ? void 0 : _me$eventTypeField.ion({
      change: 'onEventTypeChange',
      thisObj: me
    });
    (_me$saveButton = me.saveButton) === null || _me$saveButton === void 0 ? void 0 : _me$saveButton.ion({
      click: 'onSaveClick',
      thisObj: me
    });
    (_me$deleteButton = me.deleteButton) === null || _me$deleteButton === void 0 ? void 0 : _me$deleteButton.ion({
      click: 'onDeleteClick',
      thisObj: me
    });
    (_me$cancelButton = me.cancelButton) === null || _me$cancelButton === void 0 ? void 0 : _me$cancelButton.ion({
      click: 'onCancelClick',
      thisObj: me
    });
    return editor;
  }
  getEditorConfig() {
    const me = this,
      // must result in longest format, ie 2 digits for date and all time parts.
      time = new Date(2000, 12, 31, 23, 55, 55),
      dateLength = DateHelper.format(time, me.dateFormat).replace(punctuation, '').length,
      timeLength = DateHelper.format(time, me.timeFormat).replace(punctuation, '').length,
      dateTimeLength = dateLength + timeLength,
      {
        cls,
        scheduler
      } = me,
      result = ObjectHelper.assign({
        owner: scheduler,
        eventEditFeature: me,
        weekStartDay: me.weekStartDay,
        align: 'b-t',
        id: `${scheduler.id}-event-editor`,
        autoShow: false,
        anchor: true,
        scrollAction: 'realign',
        constrainTo: globalThis,
        cls
      }, me.editorConfig);
    // User configuration may have included a render target which means the editor
    // will not be floating.
    if (Widget.prototype.getRenderContext(result)[0]) {
      result.floating = false;
    }
    // If the default Popup has been reconfigured to be static, ensure it starts
    // life as a visible but collapsed panel.
    if (result.floating === false && !result.positioned) {
      result.collapsible = {
        type: 'overlay',
        direction: 'right',
        autoClose: false,
        tool: null,
        recollapseTool: null
      };
      result.collapsed = true;
      result.hidden = result.anchor = false;
      result.hide = function () {
        this.collapsible.toggleReveal(false);
      };
    }
    // Layout-affecting props must be available early so that appendTo ends up with
    // correct layout.
    result.onElementCreated = function ({
      element
    }) {
      element.style.setProperty('--date-time-length', `${dateTimeLength}em`);
      element.style.setProperty('--date-width-difference', `${(dateLength - timeLength) / 2}em`);
    };
    return result;
  }
  // Called from editEvent() to actually show the editor
  internalShowEditor(eventRecord, resourceRecord, align = null) {
    var _align, _align$target;
    const me = this,
      {
        scheduler
      } = me,
      // Align to the element (b-sch-event) and not the wrapper
      eventElement = ((_align = align) === null || _align === void 0 ? void 0 : (_align$target = _align.target) === null || _align$target === void 0 ? void 0 : _align$target.nodeType) === Element.ELEMENT_NODE ? align.target : scheduler.getElementFromEventRecord(eventRecord, resourceRecord),
      isPartOfStore = eventRecord.isPartOfStore(scheduler.eventStore);
    align = align ?? {
      // Align to the element (b-sch-event) and not the wrapper
      target: eventElement,
      anchor: true
    };
    // Event not in current TimeAxis - cannot be edited without extending the TimeAxis.
    // If there's no event element and the eventRecord is not in the store, we still
    // edit centered on the Scheduler - we're adding a new event
    if (align.target || !isPartOfStore || eventRecord.isCreating) {
      var _super$internalShowEd;
      /**
       * Fires on the owning Scheduler before an event is displayed in an editor.
       * This may be listened for to allow an application to take over event editing duties. Returning `false`
       * stops the default editing UI from being shown.
       * @event beforeEventEdit
       * @on-owner
       * @param {Scheduler.view.Scheduler} source The scheduler
       * @param {Scheduler.feature.EventEdit} eventEdit The eventEdit feature
       * @param {Scheduler.model.EventModel} eventRecord The record about to be shown in the event editor.
       * @param {Scheduler.model.ResourceModel} resourceRecord The Resource record for the event. If the event
       * is being created, it will not contain a resource, so this parameter specifies the resource the
       * event is being created for.
       * @param {HTMLElement} eventElement The element which represents the event in the scheduler display.
       * @preventable
       */
      if (scheduler.trigger('beforeEventEdit', {
        eventEdit: me,
        eventRecord,
        resourceRecord,
        eventElement
      }) === false) {
        scheduler.element.classList.remove('b-eventeditor-editing');
        return false;
      }
      // need to add this css class as early as possible to prevent
      // the event tooltip from appearing
      me.scheduler.element.classList.add('b-eventeditor-editing');
      me.resourceRecord = resourceRecord;
      const {
        editor
      } = me;
      me.editingContext = {
        eventRecord,
        resourceRecord,
        eventElement,
        editor,
        isPartOfStore
      };
      (_super$internalShowEd = super.internalShowEditor) === null || _super$internalShowEd === void 0 ? void 0 : _super$internalShowEd.call(this, eventRecord, resourceRecord, align);
      if (me.typeField) {
        me.toggleEventType(eventRecord.get(me.typeField));
      }
      me.loadRecord(eventRecord, resourceRecord);
      // If it's a static child of the client which is collapsed, expand it.
      // Floating components focusOnShow by default, this will need to be focused.
      if (editor.collapsed) {
        // The *initial* reveal does not animate unless the toggleReveal call is delayed.
        editor.setTimeout(async () => {
          await editor.collapsible.toggleReveal(true);
          editor.focus();
        }, 100);
      }
      // Honour alignment settings "anchor" and "centered" which may be injected from editorConfig.
      else if (editor.centered || !editor.anchor || !editor.floating) {
        editor.show();
      } else if (eventElement) {
        me.targetEventElement = eventElement;
        editor.showBy(align);
      }
      // We are adding an unrendered event. Display the editor centered
      else {
        editor.show();
        // Must be done after show because show always reverts to its configured centered setting.
        editor.updateCentered(true);
      }
      // Adjust time field step increment based on timeAxis resolution
      const timeResolution = scheduler.timeAxisViewModel.timeResolution;
      if (timeResolution.unit === 'hour' || timeResolution.unit === 'minute') {
        const step = `${timeResolution.increment}${timeResolution.unit}`;
        if (me.startTimeField) {
          me.startTimeField.step = step;
        }
        if (me.endTimeField) {
          me.endTimeField.step = step;
        }
      }
      // Might end up here with the old listener still around in monkey test for stress demo in turbo mode.
      // Some action happening during edit, but cannot track down what is going on
      me.detachListeners('changesWhileEditing');
      scheduler.eventStore.ion({
        change: me.onChangeWhileEditing,
        refresh: me.onChangeWhileEditing,
        thisObj: me,
        name: 'changesWhileEditing'
      });
    }
  }
  onChangeWhileEditing() {
    const me = this;
    // If event was removed, cancel editing
    // however, there's one valid case when even can be removed during save finalization - that is when
    // all its assignments has been removed - in such case ignore the removal and do not call the `onCancelClick`
    // because that will reject the STM transaction and revert all changes
    if (!me.isFinalizingEventSave && me.isEditing && me.editingContext.isPartOfStore && !me.eventRecord.isPartOfStore(me.scheduler.eventStore)) {
      me.onCancelClick();
    }
  }
  // Fired in a listener so that it's after the auto-called onBeforeShow listeners so that
  // subscribers to the beforeEventEditShow are called at exactly the correct lifecycle point.
  onBeforeEditorShow() {
    super.onBeforeEditorShow(...arguments);
    /**
     * Fires on the owning Scheduler when the editor for an event is available but before it is populated with
     * data and shown. Allows manipulating fields etc.
     * @event beforeEventEditShow
     * @on-owner
     * @param {Scheduler.view.Scheduler} source The scheduler
     * @param {Scheduler.feature.EventEdit} eventEdit The eventEdit feature
     * @param {Scheduler.model.EventModel} eventRecord The record about to be shown in the event editor.
     * @param {Scheduler.model.ResourceModel} resourceRecord The Resource record for the event. If the event
     * is being created, it will not contain a resource, so this parameter specifies the resource the
     * event is being created for.
     * @param {HTMLElement} eventElement The element which represents the event in the scheduler display.
     * @param {Core.widget.Popup} editor The editor
     */
    this.scheduler.trigger('beforeEventEditShow', {
      eventEdit: this,
      ...this.editingContext
    });
  }
  updateTargetEventElement(targetEventElement, oldTargetEventElement) {
    targetEventElement === null || targetEventElement === void 0 ? void 0 : targetEventElement.classList.add('b-editing');
    oldTargetEventElement === null || oldTargetEventElement === void 0 ? void 0 : oldTargetEventElement.classList.remove('b-editing');
  }
  /**
   * Opens an editor for the passed event. This function is exposed on Scheduler and can be called as
   * `scheduler.editEvent()`.
   * @param {Scheduler.model.EventModel} eventRecord Event to edit
   * @param {Scheduler.model.ResourceModel} [resourceRecord] The Resource record for the event.
   * This parameter is needed if the event is newly created for a resource and has not been assigned, or when using
   * multi assignment.
   * @param {HTMLElement} [element] Element to anchor editor to (defaults to events element)
   * @on-owner
   */
  editEvent(eventRecord, resourceRecord, element = null, stmCapture = null) {
    const me = this,
      {
        simpleEventEdit
      } = me.client.features;
    if (me.isEditing) {
      // old editing flow already running, clean it up
      me.resetEditingContext();
    }
    // If simple edit feature is active, use it when a new event is created
    if (me.disabled || eventRecord.readOnly || eventRecord.isCreating && simpleEventEdit !== null && simpleEventEdit !== void 0 && simpleEventEdit.enabled) {
      return;
    }
    if (stmCapture) {
      me.stmInitiallyAutoRecord = stmCapture.stmInitiallyAutoRecord;
      me.stmInitiallyDisabled = stmCapture.stmInitiallyDisabled;
      me.hasStmCapture = true;
      // indicate that editor has been opened, and is now managing the "stm capture"
      stmCapture.transferred = true;
    }
    // it is set to `false` by calendar, to ignore the STM mechanism
    else if (stmCapture !== false && !me.client.isCalendar && !me.hasStmCapture) {
      me.captureStm(true);
    }
    return me.doEditEvent(...arguments).then(result => {
      if (!me.isDestroying) {
        // The Promise being async allows a mouseover to trigger the event tip
        // unless we add the editing class immediately (But only if we actually began editing).
        if (!me.isEditing && !me.client.isCalendar) {
          // probably a custom event editor was used or editing was vetoed for some other reason
          if (result !== false) {
            // Skip stm rejection if built-in editor is disabled in beforeEventEdit (using of custom event editor)
            me.rejectStmTransaction();
          }
          me.freeStm();
        }
      }
    });
  }
  /**
   * Returns true if the editor is currently active
   * @readonly
   * @property {Boolean}
   */
  get isEditing() {
    const {
      _editor
    } = this;
    return Boolean(
    // Editor is not visible if it is collapsed and not expanded
    (_editor === null || _editor === void 0 ? void 0 : _editor.isVisible) && !(_editor.collapsed && !_editor.revealed));
  }
  // editEvent is the single entry point in the base class.
  // Subclass implementations of the action may differ, so are implemented in doEditEvent
  async doEditEvent(eventRecord, resourceRecord, element = null) {
    const me = this,
      {
        scheduler
      } = me,
      isNewRecord = eventRecord.isCreating;
    if (!resourceRecord) {
      // Need to handle resourceId for edge case when creating an event with resourceId and editing it before
      // adding it to the EventStore
      resourceRecord = eventRecord.resource || me.resourceStore.getById(eventRecord.resourceId);
    }
    if (isNewRecord) {
      // Ensure temporal data fields are ready when the editor is shown
      TimeSpan.prototype.normalize.call(eventRecord);
    }
    // If element is specified (call triggered by EventDragCreate)
    // Then we can align to that, and no scrolling is necessary.
    // If we are simply being asked to edit a new event which is not
    // yet added, the editor is centered, and no scroll is necessary
    if (element || isNewRecord) {
      return me.internalShowEditor(eventRecord, resourceRecord, element ? {
        target: element
      } : null);
    } else {
      // Ensure event is in view before showing the editor.
      // Note that we first need to extend the time axis to include
      // currently out of range events.
      return scheduler.scrollResourceEventIntoView(resourceRecord, eventRecord, {
        animate: true,
        edgeOffset: 0,
        extendTimeAxis: false
      }).then(() => me.internalShowEditor(eventRecord, resourceRecord), () => scheduler.element.classList.remove('b-eventeditor-editing'));
    }
  }
  /**
   * Sets fields values from record being edited
   * @private
   */
  loadRecord(eventRecord, resourceRecord) {
    this.loadingRecord = true;
    this.internalLoadRecord(eventRecord, resourceRecord);
    this.loadingRecord = false;
  }
  get eventRecord() {
    var _this$_editor;
    return (_this$_editor = this._editor) === null || _this$_editor === void 0 ? void 0 : _this$_editor.record;
  }
  internalLoadRecord(eventRecord, resourceRecord) {
    var _resourceField$store;
    const me = this,
      {
        eventStore
      } = me.client,
      {
        editor,
        resourceField
      } = me;
    me.resourceRecord = resourceRecord;
    // Update chained store early, to have records in place when setting value below (avoids adding the resource to
    // empty combo store, https://github.com/bryntum/support/issues/5378). It is not done automatically for
    // grouping/trees or when project is replaced
    if (resourceField && ((_resourceField$store = resourceField.store) === null || _resourceField$store === void 0 ? void 0 : _resourceField$store.masterStore) !== me.resourceStore) {
      resourceField.store = editor.chainResourceStore();
    }
    editor.record = eventRecord;
    if (resourceField) {
      const resources = eventStore.assignmentStore.getResourcesForEvent(eventRecord);
      // Flag on parent Container to indicate that initially blank fields are valid
      editor.assigningValues = true;
      // If this is an unassigned event, select the resource we've been provided
      if (!eventRecord.isOccurrence && !eventStore.storage.includes(eventRecord, true) && resourceRecord) {
        me.resourceField.value = resourceRecord[me.resourceField.valueField];
      } else if (me.assignmentStore) {
        me.resourceField.value = resources.map(resource => resource[me.resourceField.valueField]);
      }
      editor.assigningValues = false;
    }
    super.internalLoadRecord(eventRecord, resourceRecord);
  }
  toggleEventType(eventType) {
    // expose eventType in dataset, for querying and styling
    this.editor.element.dataset.eventType = eventType || '';
    this.editor.eachWidget(widget => {
      var _widget$dataset;
      // need {}'s here so we don't return false and end iteration
      ((_widget$dataset = widget.dataset) === null || _widget$dataset === void 0 ? void 0 : _widget$dataset.eventType) && (widget.hidden = widget.dataset.eventType !== eventType);
    });
  }
  //endregion
  //region Save
  async finalizeEventSave(eventRecord, resourceRecords, resolve, reject) {
    const me = this,
      {
        scheduler,
        assignmentStore
      } = me;
    // Prevent multiple commits from this flow
    assignmentStore.suspendAutoCommit();
    // Avoid multiple redraws, from event changes + assignment changes
    scheduler.suspendRefresh();
    me.onBeforeSave(eventRecord);
    eventRecord.beginBatch();
    me.updateRecord(eventRecord);
    eventRecord.endBatch();
    if (!eventRecord.isOccurrence) {
      if (me.resourceField) {
        assignmentStore.assignEventToResource(eventRecord, resourceRecords, null, true);
      }
    }
    // An occurrence event record may have changed only resources value. In that case we'll never get into afterChange() method that
    // apply changed data and make an event "real", because resources is not a field and a record won't be marked as dirty.
    // We used temporary field to save updated resources list and get into afterChange() method.
    else if (resourceRecords) {
      eventRecord.set('resourceRecords', resourceRecords);
    }
    // If it was a provisional event, passed in here from drag-create or dblclick or contextmenu,
    // it's now it's no longer a provisional event and will not be removed in resetEditingContext
    // Also, when promoted to be permanent, auto syncing will kick in if configured.
    eventRecord.isCreating = false;
    {
      await scheduler.project.commitAsync();
    }
    assignmentStore.resumeAutoCommit();
    // Redraw once
    scheduler.resumeRefresh(true);
    {
      /**
       * Fires on the owning Scheduler after an event is successfully saved
       * @event afterEventSave
       * @on-owner
       * @param {Scheduler.view.Scheduler} source The scheduler instance
       * @param {Scheduler.model.EventModel} eventRecord The record about to be saved
       */
      scheduler.trigger('afterEventSave', {
        eventRecord
      });
      me.onAfterSave(eventRecord);
    }
    resolve(eventRecord);
  }
  /**
   * Saves the changes (applies them to record if valid, if invalid editor stays open)
   * @private
   * @fires beforeEventSave
   * @fires beforeEventAdd
   * @fires afterEventSave
   * @async
   */
  save() {
    return new Promise((resolve, reject) => {
      var _me$resourceField;
      const me = this,
        {
          scheduler,
          eventRecord
        } = me;
      if (!eventRecord || !me.editor.isValid) {
        resolve(false);
        return;
      }
      const {
          eventStore,
          values
        } = me,
        resourceRecords = ((_me$resourceField = me.resourceField) === null || _me$resourceField === void 0 ? void 0 : _me$resourceField.records) || (me.resourceRecord ? [me.resourceRecord] : []);
      // Check for potential overlap scenarios before saving
      if (!me.scheduler.allowOverlap && eventStore) {
        let {
          startDate,
          endDate
        } = values;
        // Should support using a duration field instead of the end date field
        if (!endDate) {
          if ('duration' in values) {
            endDate = DateHelper.add(startDate, values.duration, values.durationUnit || eventRecord.durationUnit);
          } else if ('fullDuration' in values) {
            endDate = DateHelper.add(startDate, values.fullDuration);
          } else {
            endDate = eventRecord.endDate;
          }
        }
        const abort = resourceRecords.some(resource => {
          return !eventStore.isDateRangeAvailable(startDate, endDate, eventRecord, resource);
        });
        if (abort) {
          resolve(false);
          return;
        }
      }
      const context = {
        finalize(saveEvent) {
          try {
            if (saveEvent !== false) {
              me.finalizeEventSave(eventRecord, resourceRecords, resolve, reject);
            } else {
              resolve(false);
            }
          } catch (e) {
            reject(e);
          }
        }
      };
      /**
       * Fires on the owning Scheduler before an event is saved, return `false` to prevent the save.
       * @event beforeEventSave
       * @on-owner
       * @param {Scheduler.view.Scheduler} source The scheduler instance
       * @param {Scheduler.model.EventModel} eventRecord The record about to be saved
       * @param {Scheduler.model.ResourceModel[]} resourceRecords The resources to which the event is assigned
       * @param {Object} values The new values
       * @param {Object} context Extended save context:
       * @param {Boolean} [context.async] Set this to `true` in a listener to indicate that the listener will asynchronously decide to prevent or not the event save.
       * @param {Function} context.finalize Function to call to finalize the save. Used when `async` is `true`. Provide `false` to the function to prevent the save.
       * @preventable
       */
      if (scheduler.trigger('beforeEventSave', {
        eventRecord,
        resourceRecords,
        values,
        context
      }) === false) {
        resolve(false);
        return;
      }
      // truthy context.async means than a listener will decide to approve saving asynchronously
      if (!context.async) {
        context.finalize();
      }
    });
  }
  //endregion
  //region Delete
  /**
   * Delete event being edited
   * @fires beforeEventDelete
   * @private
   * @async
   */
  deleteEvent() {
    this.detachListeners('changesWhileEditing');
    return new Promise((resolve, reject) => {
      const me = this,
        {
          eventRecord,
          editor
        } = me;
      me.scheduler.removeEvents([eventRecord], removeRecord => {
        // The reason it does it here is to move focus *before* it gets deleted,
        // and then there's code in the delete to see that it's deleting the focused one,
        // and jump forwards or backwards to move to the next or previous event
        // See 'Should allow key activation' test in tests/view/mixins/EventNavigation.t.js
        if (removeRecord && editor.containsFocus) {
          editor.revertFocus();
        }
        resolve(removeRecord);
      }, editor);
    });
  }
  //endregion
  //region Stores
  onChangeProject() {
    // Release resource store on project change, it will be re-chained on next show
    if (this.resourceField) {
      this.resourceField.store = {}; // Cannot use null
    }
  }

  get eventStore() {
    return this.scheduler.project.eventStore;
  }
  get resourceStore() {
    return this.scheduler.project.resourceStore;
  }
  get assignmentStore() {
    return this.scheduler.project.assignmentStore;
  }
  //endregion
  //endregion
  //region Events
  onActivateEditor({
    eventRecord,
    resourceRecord,
    eventElement
  }) {
    this.editEvent(eventRecord, resourceRecord, eventElement);
  }
  onDragCreateEnd({
    eventRecord,
    resourceRecord,
    proxyElement,
    stmCapture
  }) {
    this.editEvent(eventRecord, resourceRecord, proxyElement, stmCapture);
  }
  // chained from EventNavigation
  onEventEnterKey({
    assignmentRecord,
    eventRecord,
    target
  }) {
    const {
        client
      } = this,
      // Event can arrive from the wrap element in some products (such as Calendar)
      // so in these cases, we must use querySelector to look *inside* the element.
      element = target[target.matches(client.eventSelector) ? 'querySelector' : 'closest'](client.eventInnerSelector);
    if (assignmentRecord) {
      this.editEvent(eventRecord, assignmentRecord.resource, element);
    } else if (eventRecord) {
      this.editEvent(eventRecord, eventRecord.resource, element);
    }
  }
  // Toggle fields visibility when changing eventType
  onEventTypeChange({
    value
  }) {
    this.toggleEventType(value);
  }
  //endregion
  //region Context menu
  populateEventMenu({
    eventRecord,
    resourceRecord,
    items
  }) {
    if (!this.scheduler.readOnly && !this.disabled) {
      items.editEvent = {
        text: 'L{EventEdit.Edit event}',
        localeClass: this,
        icon: 'b-icon b-icon-edit',
        weight: 100,
        disabled: eventRecord.readOnly,
        onItem: () => {
          this.editEvent(eventRecord, resourceRecord);
        }
      };
    }
  }
  //endregion
  onBeforeEditorToggleReveal({
    reveal
  }) {
    // reveal true/false is analogous to show/hide
    this[reveal ? 'onBeforeEditorShow' : 'resetEditingContext']();
  }
  resetEditingContext() {
    const me = this;
    me.detachListeners('changesWhileEditing');
    // super call has to go before the `me.rejectStmTransaction();` below
    // because it can be removing an event manually, bypassing the stm
    super.resetEditingContext();
    // client does not use STM for task editing (at least yet)
    if (me.hasStmCapture && !me.isDeletingEvent && !me.isCancelingEdit) {
      me.freeStm(false);
    }
    // Clear to prevent retaining project
    me.resourceRecord = null;
  }
  async finalizeStmCapture(shouldReject) {
    if (shouldReject) {
      await this.rejectStmTransaction();
    } else {
      this.commitStmTransaction();
    }
    this.freeStm();
  }
}
EventEdit._$name = 'EventEdit';
GridFeatureManager.registerFeature(EventEdit, true, 'Scheduler');
GridFeatureManager.registerFeature(EventEdit, false, ['SchedulerPro', 'ResourceHistogram']);
EventEdit.initClass();

/**
 * @module Scheduler/widget/ResourceFilter
 */
/**
 * A List which allows selection of resources to filter a specified eventStore to only show
 * events for the selected resources.
 *
 * Because this widget maintains a state that can be changed through the UI, it offers some of the
 * API of an input field. It has a read only {@link #property-value} property, and it fires a
 * {@link #event-change} event.
 *
 * @extends Core/widget/List
 * @classType resourceFilter
 * @widget
 */
class ResourceFilter extends List {
  static get $name() {
    return 'ResourceFilter';
  }
  // Factoryable type name
  static get type() {
    return 'resourcefilter';
  }
  static get delayable() {
    return {
      applyFilters: 'raf'
    };
  }
  static get configurable() {
    return {
      /**
       * The {@link Scheduler.data.EventStore EventStore} to filter.
       * Events for resources which are deselected in this List will be filtered out.
       * @config {Scheduler.data.EventStore}
       */
      eventStore: null,
      multiSelect: true,
      toggleAllIfCtrlPressed: true,
      itemTpl: record => StringHelper.encodeHtml(record.name || ''),
      /**
       * An optional filter function to apply when loading resources from the project's
       * resource store. Defaults to loading all resources.
       *
       * **This is called using this `ResourceFilter` as the `this` object.**
       * @config {Function|String}
       * @default
       */
      masterFilter: () => true,
      /**
       * By default, deselecting list items filters only the {@link #config-eventStore} so that
       * events for the deselected resources are hidden from view. The `resourceStore` is __not__
       * filtered.
       *
       * Configure this as `true` to also filter the `resourceStore` so that deselected resources
       * are also hidden from view (They will remain in this `List`)
       * @config {Boolean}
       * @default false
       */
      filterResources: null
    };
  }
  itemIconTpl(record, i) {
    const {
        eventColor
      } = record,
      // Named colors are applied using CSS
      cls = DomHelper.isNamedColor(eventColor) ? ` b-sch-foreground-${eventColor}` : '',
      // CSS style color is used as is
      style = !cls && eventColor ? ` style="color:${eventColor}"` : '';
    return this.multiSelect ? `<div class="b-selected-icon b-icon${cls}"${style}></div>` : '';
  }
  updateEventStore(eventStore) {
    var _me$initialConfig$sto, _me$store;
    const me = this,
      // HACK: Temp workaround until List's store is dynamically updatable.
      chainedStoreConfig = (_me$initialConfig$sto = me.initialConfig.store) !== null && _me$initialConfig$sto !== void 0 && _me$initialConfig$sto.isStore ? me.initialConfig.store.initialConfig : (_me$store = me.store) === null || _me$store === void 0 ? void 0 : _me$store.config,
      // Allow configuration of the filter for loading records from the master store.
      {
        resourceStore
      } = eventStore,
      store = me.store = resourceStore.chain(me.masterFilter, null, {
        ...chainedStoreConfig,
        syncOrder: true
      }),
      changeListeners = {
        change: 'onStoreChange',
        thisObj: me
      };
    // We need to sync selection and rendering on changes fired from master store
    store.un(changeListeners);
    resourceStore.ion(changeListeners);
    if (!resourceStore.count) {
      resourceStore.project.ion({
        name: 'project',
        refresh: 'initFilter',
        thisObj: me
      });
    } else {
      me.initFilter();
    }
  }
  changeMasterFilter(masterFilter) {
    // Cannot use bind, otherwise fillFromMaster's check for whether its a filter function fails.
    const me = this;
    // If we are filtering the resource store, we cannot now fill ourselves from the filtered
    // view of the resource store. Otherwise the list would hide the list items as they are deselected.
    if (!me.filterResources) {
      return function (r) {
        return me.callback(masterFilter, me, [r]);
      };
    }
  }
  initFilter() {
    const {
      eventStore,
      selected
    } = this;
    if (eventStore.count && eventStore.resourceStore.count) {
      // We default to all resources selected unless this was configured with
      // an initialSelection. See List#changeSelection
      if (!this.initialSelection) {
        selected.add(this.store.getRange());
      }
      this.detachListeners('project');
    }
  }
  onStoreRefresh({
    source: store,
    action
  }) {
    // We need to re-enable the filter if the store becomes filtered.
    // We only disable the filter if we know that we have selected all available
    // resources.
    if (action === 'filter' && this.eventStoreFilter) {
      const {
          eventStoreFilter
        } = this,
        {
          disabled
        } = eventStoreFilter,
        newDisabled = !store.isFiltered && this.allSelected;
      if (newDisabled !== disabled) {
        eventStoreFilter.disabled = newDisabled;
        this.applyFilters();
      }
    }
    super.onStoreRefresh(...arguments);
  }
  onSelectionChange({
    source: selected,
    added,
    removed
  }) {
    // Filter disabled if all resources selected
    const me = this,
      // Only disable the filter if the allSelected method is seeing *all* of the
      // records from its masterStore with no filtering.
      disabled = !me.store.isFiltered && me.allSelected;
    super.onSelectionChange(...arguments);
    let filtersAdded = false;
    // If this is the first selection change triggered from the first project refresh
    // in which all the resources are selected, then we ony need to apply the filters.
    // if *not* all resources are selected, ie if added.length !== entire store length.
    if (!me.eventStoreFilter) {
      // Our client EventStore is filtered to only show events for our selected resources.
      // Events without an associated resource are filtered into visibility.
      // The addFilter function with silent param adds the filter but don't reevaluate filtering.
      me.eventStoreFilter = me.eventStore.addFilter({
        id: `${me.id}-filter-instance`,
        filterBy: e => !e.resource || me.selected.includes(e.resources),
        disabled
      }, (added === null || added === void 0 ? void 0 : added.length) === me.store.count);
      filtersAdded = true;
    }
    if (me.filterResources && !me.resourceStoreFilter) {
      // Our client EventStore is filtered to only show events for our selected resources.
      // Events without an associated resource are filtered into visibility.
      // The addFilter function with silent param adds the filter but don't reevaluate filtering.
      me.resourceStoreFilter = me.eventStore.resourceStore.addFilter({
        id: `${me.id}-filter-instance`,
        filterBy: r => me.selected.includes(r),
        disabled
      }, (added === null || added === void 0 ? void 0 : added.length) === me.store.count);
      filtersAdded = true;
    }
    // The filters have been just added and so will take effect. No need to call applyFilter.
    if (filtersAdded) {
      return;
    }
    // Filter disabled if all resources selected
    me.eventStoreFilter.disabled = disabled;
    me.resourceStoreFilter && (me.resourceStoreFilter.disabled = disabled);
    // Have the client EventStore refresh its filtering but after a small delay so the List UI updates immediately.
    me.applyFilters();
    if (me.eventListeners.change) {
      const value = selected.values,
        oldValue = value.concat(removed);
      ArrayHelper.remove(oldValue, added);
      /**
       * Fired when this widget's selection changes
       * @event change
       * @param {String} value - This field's value
       * @param {String} oldValue - This field's previous value
       * @param {Core.widget.Field} source - This ResourceFilter
       */
      me.triggerFieldChange({
        value,
        oldValue
      });
    }
  }
  /**
   * An array encapsulating the currently selected resources.
   * @member {Scheduler.model.ResourceModel[]}
   * @readonly
   */
  get value() {
    return this.selected.values;
  }
  applyFilters() {
    this.eventStore.filter();
    this.filterResources && this.eventStore.resourceStore.filter();
  }
  doDestroy() {
    var _this$store;
    (_this$store = this.store) === null || _this$store === void 0 ? void 0 : _this$store.destroy();
    super.doDestroy();
  }
}
// Register this widget type with its Factory
ResourceFilter.initClass();
ResourceFilter._$name = 'ResourceFilter';

/**
 * @module Scheduler/widget/SchedulerDatePicker
 */
/**
 * A subclass of {@link Core.widget.DatePicker} which is able to show the presence of
 * events in its cells if configured with an {@link #config-eventStore}, and
 * {@link #config-showEvents} is set to a truthy value.
 *
 * The `datepicker` Widget type is implemented by this class when this class is imported, or built
 * into a bundle, and so any {@link Core.widget.DateField} may have its
 * {@link Core.widget.PickerField#config-picker} configured to use its capabilities of showing
 * the presence of events in its date cells.
 *
 * @classtype datepicker
 * @extends Core/widget/DatePicker
 * @inlineexample Scheduler/widget/SchedulerDatePicker.js
 * @widget
 */
class SchedulerDatePicker extends DatePicker {
  static get $name() {
    return 'SchedulerDatePicker';
  }
  static get type() {
    return 'datepicker';
  }
  static get configurable() {
    return {
      /**
       * How to show presence of events in the configured {@link #config-eventStore} in the
       * day cells. Values may be:
       *
       * * `false` - Do not show events in cells.
       * * `true` - Show a themeable bullet to indicate the presence of events for a date.
       * * `'count'` - Show a themeable badge containing the event count for a date.
       * @config {Boolean|'count'}
       * @default false
       */
      showEvents: null,
      /**
       * The {@link Scheduler.data.EventStore event store} from which the in-cell event presence
       * indicators are drawn.
       * @config {Scheduler.data.EventStore}
       */
      eventStore: null,
      /**
       * A function, or the name of a function in the ownership hierarchy to filter which events
       * are collected into the day cell data blocks.
       *
       * Return `true` to include the passed event, or a *falsy* value to exclude the event.
       * @config {Function|String}
       */
      eventFilter: {
        $config: 'lazy',
        value: null
      }
    };
  }
  construct(config) {
    // Handle deprecated events config. It is now showEvents.
    // events conflicts with the events data which may be passed in
    if ('events' in config) {
      config = {
        ...config,
        showEvents: config.events
      };
      delete config.events;
      VersionHelper.deprecate(VersionHelper['calendar'] ? 'Calendar' : 'Scheduler', '6.0.0', 'DatePicker#events should be configured as showEvents');
    }
    super.construct(config);
  }
  changeEventFilter(eventFilter) {
    if (typeof eventFilter === 'string') {
      const {
        handler,
        thisObj
      } = this.resolveCallback(eventFilter);
      eventFilter = handler.bind(thisObj);
    }
    return eventFilter;
  }
  doRefresh() {
    // Hidden widgets must not query the EventStore for loading on demand to be able to use
    // the EventStore's dateRangeRequested event.
    if (this.isVisible || !this.showEvents) {
      this.refreshEventsMap();
      return super.doRefresh(...arguments);
    } else {
      this.whenVisible('doRefresh');
    }
  }
  updateShowEvents(showEvents, oldShowEvents) {
    const me = this,
      {
        classList
      } = me.contentElement;
    let {
      eventStore
    } = me;
    // Begin any animations in the next AF
    me.requestAnimationFrame(() => {
      var _me$owner;
      me.element.classList.toggle('b-datepicker-with-events', Boolean(showEvents));
      (_me$owner = me.owner) === null || _me$owner === void 0 ? void 0 : _me$owner.element.classList.toggle('b-datepicker-with-events', Boolean(showEvents));
      showEvents && classList.add(`b-show-events-${showEvents}`);
      classList.remove(`b-show-events-${oldShowEvents}`);
    });
    if (showEvents) {
      if (!eventStore) {
        const eventStoreOwner = me.up(w => w.eventStore);
        if (eventStoreOwner) {
          eventStore = eventStoreOwner.eventStore;
        } else {
          throw new Error('DatePicker configured with events but no eventStore');
        }
      }
    } else {
      me.eventsMap = null;
    }
    if (!me.isConfiguring) {
      me.updateEventStore(eventStore);
      me.doRefresh();
    }
  }
  refreshEventsMap() {
    const me = this;
    if (me.showEvents) {
      me.eventsMap = me.eventStore.getEventCounts({
        startDate: me.startDate,
        endDate: me.endDate,
        dateMap: me.eventsMap,
        filter: me.eventFilter
      });
    }
  }
  updateEventStore(eventStore) {
    // Add a listener to refresh on any event change unless the listener is already added.
    if (eventStore.findListener('change', 'refresh', this) === -1) {
      var _eventStore;
      eventStore === null || eventStore === void 0 ? void 0 : (_eventStore = eventStore[this.showEvents ? 'on' : 'un']) === null || _eventStore === void 0 ? void 0 : _eventStore.call(eventStore, {
        change: 'refresh',
        thisObj: this
      });
    }
  }
  cellRenderer({
    cell,
    date
  }) {
    var _this$eventCounts, _this$eventCounts$get;
    const {
        showEvents
      } = this,
      count = (_this$eventCounts = this.eventCounts) === null || _this$eventCounts === void 0 ? void 0 : (_this$eventCounts$get = _this$eventCounts.get) === null || _this$eventCounts$get === void 0 ? void 0 : _this$eventCounts$get.call(_this$eventCounts, DateHelper.makeKey(date)),
      isCount = showEvents === 'count';
    delete cell.dataset.btip;
    if (count) {
      if (!isCount && this.eventCountTip) {
        cell.dataset.btip = this.L('L{ResourceInfoColumn.eventCountText}', count);
      }
      DomHelper.createElement({
        dataset: {
          count
        },
        class: {
          [isCount ? 'b-cell-events-badge' : 'b-icon b-icon-circle']: 1,
          [SchedulerDatePicker.getEventCountClass(count)]: 1
        },
        parent: cell,
        [isCount ? 'text' : '']: count
      });
    }
  }
  static getEventCountClass(count) {
    if (count) {
      if (count < 4) {
        return 'b-datepicker-1-to-3-events';
      }
      if (count < 7) {
        return 'b-datepicker-4-to-6-events';
      }
      return 'b-calendar-7-or-more-events';
    }
    return '';
  }
  static setupClass(meta) {
    // We take over the type name 'datepicker' when we are in the app
    meta.replaceType = true;
    super.setupClass(meta);
  }
}
// Register this widget type with its Factory
SchedulerDatePicker.initClass();
SchedulerDatePicker._$name = 'SchedulerDatePicker';

export { EditBase, EventEdit, EventEditor, RecurrenceCombo, RecurrenceEditor, RecurrenceLegend, RecurrenceLegendButton, RecurringEventEdit, ResourceCombo, ResourceFilter, SchedulerDatePicker };
//# sourceMappingURL=SchedulerDatePicker.js.map
