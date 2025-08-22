/*!
 *
 * Bryntum Scheduler Pro 5.3.7
 *
 * Copyright(c) 2023 Bryntum AB
 * https://bryntum.com/contact
 * https://bryntum.com/license
 *
 */
import { LocaleHelper, Base, TimeZoneHelper, StringHelper, DateHelper, BrowserHelper, ContextMenuBase, Popup, Combo, ArrayHelper, Panel, DomHelper, Objects, ObjectHelper, Delayable, Navigator } from './Editor.js';
import { GridFeatureManager, Location } from './GridBase.js';
import { ProjectModel, RecurrenceDayRuleEncoder } from './ProjectModel.js';
import './MessageDialog.js';
import { ButtonGroup } from './ButtonGroup.js';

const locale = {
  localeName: 'En',
  localeDesc: 'English (US)',
  localeCode: 'en-US',
  Object: {
    newEvent: 'New event'
  },
  ResourceInfoColumn: {
    eventCountText: data => data + ' event' + (data !== 1 ? 's' : '')
  },
  Dependencies: {
    from: 'From',
    to: 'To',
    valid: 'Valid',
    invalid: 'Invalid'
  },
  DependencyType: {
    SS: 'SS',
    SF: 'SF',
    FS: 'FS',
    FF: 'FF',
    StartToStart: 'Start-to-Start',
    StartToEnd: 'Start-to-Finish',
    EndToStart: 'Finish-to-Start',
    EndToEnd: 'Finish-to-Finish',
    short: ['SS', 'SF', 'FS', 'FF'],
    long: ['Start-to-Start', 'Start-to-Finish', 'Finish-to-Start', 'Finish-to-Finish']
  },
  DependencyEdit: {
    From: 'From',
    To: 'To',
    Type: 'Type',
    Lag: 'Lag',
    'Edit dependency': 'Edit dependency',
    Save: 'Save',
    Delete: 'Delete',
    Cancel: 'Cancel',
    StartToStart: 'Start to Start',
    StartToEnd: 'Start to End',
    EndToStart: 'End to Start',
    EndToEnd: 'End to End'
  },
  EventEdit: {
    Name: 'Name',
    Resource: 'Resource',
    Start: 'Start',
    End: 'End',
    Save: 'Save',
    Delete: 'Delete',
    Cancel: 'Cancel',
    'Edit event': 'Edit event',
    Repeat: 'Repeat'
  },
  EventDrag: {
    eventOverlapsExisting: 'Event overlaps existing event for this resource',
    noDropOutsideTimeline: 'Event may not be dropped completely outside the timeline'
  },
  SchedulerBase: {
    'Add event': 'Add event',
    'Delete event': 'Delete event',
    'Unassign event': 'Unassign event'
  },
  TimeAxisHeaderMenu: {
    pickZoomLevel: 'Zoom',
    activeDateRange: 'Date range',
    startText: 'Start date',
    endText: 'End date',
    todayText: 'Today'
  },
  EventCopyPaste: {
    copyEvent: 'Copy event',
    cutEvent: 'Cut event',
    pasteEvent: 'Paste event'
  },
  EventFilter: {
    filterEvents: 'Filter tasks',
    byName: 'By name'
  },
  TimeRanges: {
    showCurrentTimeLine: 'Show current timeline'
  },
  PresetManager: {
    secondAndMinute: {
      displayDateFormat: 'll LTS',
      name: 'Seconds'
    },
    minuteAndHour: {
      topDateFormat: 'ddd MM/DD, hA',
      displayDateFormat: 'll LST'
    },
    hourAndDay: {
      topDateFormat: 'ddd MM/DD',
      middleDateFormat: 'LST',
      displayDateFormat: 'll LST',
      name: 'Day'
    },
    day: {
      name: 'Day/hours'
    },
    week: {
      name: 'Week/hours'
    },
    dayAndWeek: {
      displayDateFormat: 'll LST',
      name: 'Week/days'
    },
    dayAndMonth: {
      name: 'Month'
    },
    weekAndDay: {
      displayDateFormat: 'll LST',
      name: 'Week'
    },
    weekAndMonth: {
      name: 'Weeks'
    },
    weekAndDayLetter: {
      name: 'Weeks/weekdays'
    },
    weekDateAndMonth: {
      name: 'Months/weeks'
    },
    monthAndYear: {
      name: 'Months'
    },
    year: {
      name: 'Years'
    },
    manyYears: {
      name: 'Multiple years'
    }
  },
  RecurrenceConfirmationPopup: {
    'delete-title': 'You are deleting an event',
    'delete-all-message': 'Do you want to delete all occurrences of this event?',
    'delete-further-message': 'Do you want to delete this and all future occurrences of this event, or only the selected occurrence?',
    'delete-further-btn-text': 'Delete All Future Events',
    'delete-only-this-btn-text': 'Delete Only This Event',
    'update-title': 'You are changing a repeating event',
    'update-all-message': 'Do you want to change all occurrences of this event?',
    'update-further-message': 'Do you want to change only this occurrence of the event, or this and all future occurrences?',
    'update-further-btn-text': 'All Future Events',
    'update-only-this-btn-text': 'Only This Event',
    Yes: 'Yes',
    Cancel: 'Cancel',
    width: 600
  },
  RecurrenceLegend: {
    ' and ': ' and ',
    Daily: 'Daily',
    'Weekly on {1}': ({
      days
    }) => `Weekly on ${days}`,
    'Monthly on {1}': ({
      days
    }) => `Monthly on ${days}`,
    'Yearly on {1} of {2}': ({
      days,
      months
    }) => `Yearly on ${days} of ${months}`,
    'Every {0} days': ({
      interval
    }) => `Every ${interval} days`,
    'Every {0} weeks on {1}': ({
      interval,
      days
    }) => `Every ${interval} weeks on ${days}`,
    'Every {0} months on {1}': ({
      interval,
      days
    }) => `Every ${interval} months on ${days}`,
    'Every {0} years on {1} of {2}': ({
      interval,
      days,
      months
    }) => `Every ${interval} years on ${days} of ${months}`,
    position1: 'the first',
    position2: 'the second',
    position3: 'the third',
    position4: 'the fourth',
    position5: 'the fifth',
    'position-1': 'the last',
    day: 'day',
    weekday: 'weekday',
    'weekend day': 'weekend day',
    daysFormat: ({
      position,
      days
    }) => `${position} ${days}`
  },
  RecurrenceEditor: {
    'Repeat event': 'Repeat event',
    Cancel: 'Cancel',
    Save: 'Save',
    Frequency: 'Frequency',
    Every: 'Every',
    DAILYintervalUnit: 'day(s)',
    WEEKLYintervalUnit: 'week(s)',
    MONTHLYintervalUnit: 'month(s)',
    YEARLYintervalUnit: 'year(s)',
    Each: 'Each',
    'On the': 'On the',
    'End repeat': 'End repeat',
    'time(s)': 'time(s)'
  },
  RecurrenceDaysCombo: {
    day: 'day',
    weekday: 'weekday',
    'weekend day': 'weekend day'
  },
  RecurrencePositionsCombo: {
    position1: 'first',
    position2: 'second',
    position3: 'third',
    position4: 'fourth',
    position5: 'fifth',
    'position-1': 'last'
  },
  RecurrenceStopConditionCombo: {
    Never: 'Never',
    After: 'After',
    'On date': 'On date'
  },
  RecurrenceFrequencyCombo: {
    None: 'No repeat',
    Daily: 'Daily',
    Weekly: 'Weekly',
    Monthly: 'Monthly',
    Yearly: 'Yearly'
  },
  RecurrenceCombo: {
    None: 'None',
    Custom: 'Custom...'
  },
  Summary: {
    'Summary for': date => `Summary for ${date}`
  },
  ScheduleRangeCombo: {
    completeview: 'Complete schedule',
    currentview: 'Visible schedule',
    daterange: 'Date range',
    completedata: 'Complete schedule (for all events)'
  },
  SchedulerExportDialog: {
    'Schedule range': 'Schedule range',
    'Export from': 'From',
    'Export to': 'To'
  },
  ExcelExporter: {
    'No resource assigned': 'No resource assigned'
  },
  CrudManagerView: {
    serverResponseLabel: 'Server response:'
  },
  DurationColumn: {
    Duration: 'Duration'
  }
};
LocaleHelper.publishLocale(locale);

/**
 * @module Scheduler/data/mixin/AttachToProjectMixin
 */
/**
 * Mixin that calls the target class `attachToProject()` function when a new project is assigned to Scheduler/Gantt.
 *
 * @mixin
 */
var AttachToProjectMixin = (Target => class AttachToProjectMixin extends Target {
  static get $name() {
    return 'AttachToProjectMixin';
  }
  async afterConstruct() {
    super.afterConstruct();
    const me = this,
      projectHolder = me.client || me.grid,
      {
        project
      } = projectHolder;
    projectHolder.projectSubscribers.push(me);
    // Attach to already existing stores
    if (project) {
      me.attachToProject(project);
      me.attachToResourceStore(project.resourceStore);
      me.attachToEventStore(project.eventStore);
      me.attachToAssignmentStore(project.assignmentStore);
      me.attachToDependencyStore(project.dependencyStore);
      me.attachToCalendarManagerStore(project.calendarManagerStore);
    }
  }
  /**
   * Override to take action when the project instance is replaced.
   *
   * @param {Scheduler.model.ProjectModel} project
   */
  attachToProject(project) {
    this.detachListeners('project');
    this._project = project;
  }
  /**
   * Override to take action when the EventStore instance is replaced, either from being replaced on the project or
   * from assigning a new project.
   *
   * @param {Scheduler.data.EventStore} store
   */
  attachToEventStore(store) {
    this.detachListeners('eventStore');
  }
  /**
   * Override to take action when the ResourceStore instance is replaced, either from being replaced on the project
   * or from assigning a new project.
   *
   * @param {Scheduler.data.ResourceStore} store
   */
  attachToResourceStore(store) {
    this.detachListeners('resourceStore');
  }
  /**
   * Override to take action when the AssignmentStore instance is replaced, either from being replaced on the project
   * or from assigning a new project.
   *
   * @param {Scheduler.data.AssignmentStore} store
   */
  attachToAssignmentStore(store) {
    this.detachListeners('assignmentStore');
  }
  /**
   * Override to take action when the DependencyStore instance is replaced, either from being replaced on the project
   * or from assigning a new project.
   *
   * @param {Scheduler.data.DependencyStore} store
   */
  attachToDependencyStore(store) {
    this.detachListeners('dependencyStore');
  }
  /**
   * Override to take action when the CalendarManagerStore instance is replaced, either from being replaced on the
   * project or from assigning a new project.
   *
   * @param {Core.data.Store} store
   */
  attachToCalendarManagerStore(store) {
    this.detachListeners('calendarManagerStore');
  }
  get project() {
    return this._project;
  }
  get calendarManagerStore() {
    return this.project.calendarManagerStore;
  }
  get assignmentStore() {
    return this.project.assignmentStore;
  }
  get resourceStore() {
    return this.project.resourceStore;
  }
  get eventStore() {
    return this.project.eventStore;
  }
  get dependencyStore() {
    return this.project.dependencyStore;
  }
});

/**
 * @module Scheduler/data/mixin/ProjectConsumer
 */
const engineStoreNames = ['assignmentStore', 'dependencyStore', 'eventStore', 'resourceStore'];
/**
 * Creates a Project using any configured stores, and sets the stores configured into the project into
 * the host object.
 *
 * @mixin
 */
var ProjectConsumer = (Target => class ProjectConsumer extends (Target || Base) {
  static get $name() {
    return 'ProjectConsumer';
  }
  //region Default config
  static get declarable() {
    return ['projectStores'];
  }
  static get configurable() {
    return {
      projectModelClass: ProjectModel,
      /**
       * The {@link Scheduler.model.ProjectModel} instance, containing the data visualized by the Scheduler.
       *
       * **Note:** In SchedulerPro the project is instance of SchedulerPro.model.ProjectModel class.
       * @member {Scheduler.model.ProjectModel} project
       * @typings {ProjectModel}
       * @category Data
       */
      /**
       * A {@link Scheduler.model.ProjectModel} instance or a config object. The project holds all Scheduler data.
       * Can be omitted in favor of individual store configs or {@link Scheduler.view.mixin.SchedulerStores#config-crudManager} config.
       *
       * **Note:** This config is **mandatory** in SchedulerPro. See SchedulerPro.model.ProjectModel class.
       * @config {Scheduler.model.ProjectModel|ProjectModelConfig} project
       * @category Data
       */
      project: {},
      /**
       * Configure as `true` to destroy the Project and stores when `this` is destroyed.
       * @config {Boolean}
       * @category Data
       */
      destroyStores: null,
      // Will be populated by AttachToProjectMixin which features mix in
      projectSubscribers: []
    };
  }
  #suspendedByRestore;
  //endregion
  startConfigure(config) {
    // process the project first which ingests any configured data sources,
    this.getConfig('project');
    super.startConfigure(config);
  }
  //region Project
  // This is where all the ingestion happens.
  // At config time, the changers inject incoming values into the project config object
  // that we are building. At the end we instantiate the project with all incoming
  // config values filled in.
  changeProject(project, oldProject) {
    const me = this,
      {
        projectStoreNames,
        projectDataNames
      } = me.constructor;
    me.projectCallbacks = new Set();
    if (project) {
      // Flag for changes to know what stage we are at
      me.buildingProjectConfig = true;
      if (!project.isModel) {
        // When configuring, prio order:
        // 1. If using an already existing CrudManager, it is assumed to already have the stores we should use,
        //    adopt them as ours.
        // 2. If a supplied store already has a project, it is assumed to be shared with another scheduler and
        //    that project is adopted as ours. Unless we are given some store not part of that project,
        //    in which case we create a new project.
        // 3. Use stores from a supplied project config.
        // 4. Use stores configured on scheduler.
        // + Pass on inline data (events, resources, dependencies, assignments -> xxData on the project config)
        //
        // What happens during project initialization is this:
        // this._project is the project *config* object.
        // changeXxxx methods put incoming values directly into it through this.project
        // to be used as its configuration.
        // So when it is instantiated, it has had all configs injected.
        if (me.isConfiguring) {
          // Set property for changers to put incoming values into
          me._project = project;
          // crudManager will be a clone of the raw config if it is a raw config.
          const {
            crudManager
          } = me;
          // Pull in stores from the crudManager config first
          if (crudManager) {
            const {
              isCrudManager
            } = crudManager;
            for (const storeName of projectStoreNames) {
              if (crudManager[storeName]) {
                // We configure the project with the stores, and *not* the CrudManager.
                // The CrudManager ends up having its project set and thereby adopting ours.
                me[storeName] = crudManager[storeName];
                // If it's just a config, take the stores out.
                // We will *configure* it with this project and it will ingest
                // its stores from there.
                if (!isCrudManager) {
                  delete crudManager[storeName];
                }
              }
            }
          }
          // Pull in all our configured stores into the project config object.
          // That also extracts any project into this._sharedProject
          me.getConfig('projectStores');
          // Referencing these data configs causes them to be pulled into
          // the _project.xxxData config property if they are present.
          for (const dataName of projectDataNames) {
            me.getConfig(dataName);
          }
        }
        const {
          eventStore
        } = project;
        let {
          _sharedProject: sharedProject
        } = me;
        // Delay autoLoading until listeners are set up, to be able to inject params
        if (eventStore && !eventStore.isEventStoreMixin && eventStore.autoLoad && !eventStore.count) {
          eventStore.autoLoad = false;
          me.delayAutoLoad = true;
        }
        // We should not adopt a project from a store if we are given any store not part of that project
        if (sharedProject && engineStoreNames.some(store => project[store] && project[store] !== sharedProject[store])) {
          // We have to chain any store used by the other project, they can only belong to one
          for (const store of engineStoreNames) {
            if (project[store] && project[store] === sharedProject[store]) {
              project[store] = project[store].chain();
            }
          }
          sharedProject = null;
        }
        // Use sharedProject if found, else instantiate our config.
        project = sharedProject || new me.projectModelClass(project);
        // Clear the property so that the updater is called.
        delete me._project;
      }
      // In the updater, configs are live
      me.buildingProjectConfig = false;
    }
    return project;
  }
  /**
   * Implement in subclass to take action when project is replaced.
   *
   * __`super.updateProject(...arguments)` must be called first.__
   *
   * @param {Scheduler.model.ProjectModel} project
   * @category Data
   */
  updateProject(project, oldProject) {
    const me = this,
      {
        projectListeners,
        crudManager
      } = me;
    me.detachListeners('projectConsumer');
    // When we set the crudManager now, it will go through to the CrudManagerVIew
    delete me._crudManager;
    if (project) {
      var _project$stm;
      projectListeners.thisObj = me;
      project.ion(projectListeners);
      // If the project is a CrudManager, use it as such.
      if (project.isCrudManager) {
        me.crudManager = project;
      }
      // Apply the project to CrudManager, making sure the same stores are used there and here
      else if (crudManager) {
        crudManager.project = project;
        // CrudManager goes through the changer as usual and is initialized
        // from the Project, not any stores it was originally configured with.
        me.crudManager = crudManager;
      }
      // Notifies classes that mix AttachToProjectMixin that we have a new project
      me.projectSubscribers.forEach(subscriber => subscriber.attachToProject(project));
      // Sets the project's stores into the host object
      for (const storeName of me.constructor.projectStoreNames) {
        me[storeName] = project[storeName];
      }
      // Listeners are set up, if EventStore was configured with autoLoad now is the time to load
      if (me.delayAutoLoad) {
        // Restore the flag, not needed but to look good on inspection
        project.eventStore.autoLoad = true;
        project.eventStore.load();
      }
      (_project$stm = project.stm) === null || _project$stm === void 0 ? void 0 : _project$stm.ion({
        name: 'projectConsumer',
        restoringStart: 'onProjectRestoringStart',
        restoringStop: 'onProjectRestoringStop',
        thisObj: me
      });
    }
    me.trigger('projectChange', {
      project
    });
  }
  // Implementation here because we need to get first look at it to adopt its stores
  changeCrudManager(crudManager) {
    // Set the property to be scanned for incoming stores.
    // If it's a config, it will be stripped of those stores prior to construction.
    if (this.buildingProjectConfig) {
      this._crudManager = crudManager.isCrudManager ? crudManager : Object.assign({}, crudManager);
    } else {
      return super.changeCrudManager(crudManager);
    }
  }
  // Called when project changes are committed, after data is written back to records
  onProjectDataReady() {
    const me = this;
    // Only update the UI when we are visible
    me.whenVisible(() => {
      if (me.projectCallbacks.size) {
        me.projectCallbacks.forEach(callback => callback());
        me.projectCallbacks.clear();
      }
    }, null, null, 'onProjectDataReady');
  }
  onProjectRestoringStart({
    stm
  }) {
    const {
      rawQueue
    } = stm;
    // Suspend refresh if undo/redo potentially leads to multiple refreshes
    if (rawQueue.length && rawQueue[rawQueue.length - 1].length > 1) {
      this.#suspendedByRestore = true;
      this.suspendRefresh();
    }
  }
  onProjectRestoringStop() {
    if (this.#suspendedByRestore) {
      this.#suspendedByRestore = false;
      this.resumeRefresh(true);
    }
  }
  // Overridden in CalendarStores.js
  onBeforeTimeZoneChange() {}
  // When project changes time zone, change start and end dates
  onTimeZoneChange({
    timeZone,
    oldTimeZone
  }) {
    const me = this;
    // The timeAxis timeZone could be equal to timeZone if we are a partnered scheduler
    if (me.startDate && me.timeAxis.timeZone !== timeZone) {
      const startDate = oldTimeZone != null ? TimeZoneHelper.fromTimeZone(me.startDate, oldTimeZone) : me.startDate;
      me.startDate = timeZone != null ? TimeZoneHelper.toTimeZone(startDate, timeZone) : startDate;
      // Saves the timeZone on the timeAxis as it is shared between partnered schedulers
      me.timeAxis.timeZone = timeZone;
    }
  }
  /**
   * Accepts a callback that will be called when the underlying project is ready (no commit pending and current commit
   * finalized)
   * @param {Function} callback
   * @category Data
   */
  whenProjectReady(callback) {
    // Might already be ready, call directly
    if (this.isEngineReady) {
      callback();
    } else {
      this.projectCallbacks.add(callback);
    }
  }
  /**
   * Returns `true` if engine is in a stable calculated state, `false` otherwise.
   * @property {Boolean}
   * @category Misc
   */
  get isEngineReady() {
    var _this$project$isEngin, _this$project;
    // NonWorkingTime calls this during destruction, hence the ?.
    return Boolean((_this$project$isEngin = (_this$project = this.project).isEngineReady) === null || _this$project$isEngin === void 0 ? void 0 : _this$project$isEngin.call(_this$project));
  }
  //endregion
  //region Destroy
  // Cleanup, destroys stores if this.destroyStores is true.
  doDestroy() {
    super.doDestroy();
    if (this.destroyStores) {
      // Shared project might already be destroyed
      !this.project.isDestroyed && this.project.destroy();
    }
  }
  //endregion
  get projectStores() {
    const {
      projectStoreNames
    } = this.constructor;
    return projectStoreNames.map(storeName => this[storeName]);
  }
  static get projectStoreNames() {
    return Object.keys(this.projectStores);
  }
  static get projectDataNames() {
    return this.projectStoreNames.reduce((result, storeName) => {
      const {
        dataName
      } = this.projectStores[storeName];
      if (dataName) {
        result.push(dataName);
      }
      return result;
    }, []);
  }
  static setupProjectStores(cls, meta) {
    const {
      projectStores
    } = cls;
    if (projectStores) {
      const projectListeners = {
          name: 'projectConsumer',
          dataReady: 'onProjectDataReady',
          change: 'relayProjectDataChange',
          beforeTimeZoneChange: 'onBeforeTimeZoneChange',
          timeZoneChange: 'onTimeZoneChange'
        },
        storeConfigs = {
          projectListeners
        };
      let previousDataName;
      // Create a property and updater for each dataName and a changer for each store
      for (const storeName in projectStores) {
        const {
          dataName
        } = projectStores[storeName];
        // Define "eventStore" and "events" configs
        storeConfigs[storeName] = storeConfigs[dataName] = null;
        // Define up the "events" property
        if (dataName) {
          // Getter to return store data
          Object.defineProperty(meta.class.prototype, dataName, {
            configurable: true,
            // So that Config can add its setter.
            get() {
              var _this$project$storeNa;
              // get events() { return this.project.eventStore.records; }
              return (_this$project$storeNa = this.project[storeName]) === null || _this$project$storeNa === void 0 ? void 0 : _this$project$storeNa.records;
            }
          });
          // Create an updater for the data name;
          this.createDataUpdater(storeName, dataName, previousDataName, meta);
        }
        this.createStoreDescriptor(meta, storeName, projectStores[storeName], projectListeners);
        // The next data updater must reference this data name
        previousDataName = dataName;
      }
      // Create the projectListeners config.
      this.setupConfigs(meta, storeConfigs);
    }
  }
  static createDataUpdater(storeName, dataName, previousDataName, meta) {
    // Create eg "updateEvents(data)".
    // We need it to call this.getConfig('resources') so that ordering of
    // data ingestion is corrected.
    meta.class.prototype[`update${StringHelper.capitalize(dataName)}`] = function (data) {
      const {
        project
      } = this;
      // Ensure a dataName that we depend on is called in.
      // For example dependencies must load in order after the events.
      previousDataName && this.getConfig(previousDataName);
      if (this.buildingProjectConfig) {
        // Set the property in the project config object.
        // eg project.eventsData = [...]
        project[`${dataName}Data`] = data;
      } else {
        // Live update the project when in use.
        project[storeName].data = data;
      }
    };
  }
  // eslint-disable-next-line bryntum/no-listeners-in-lib
  static createStoreDescriptor(meta, storeName, {
    listeners
  }, projectListeners) {
    const {
        prototype: clsProto
      } = meta.class,
      storeNameCap = StringHelper.capitalize(storeName);
    // Set up onProjectEventStoreChange to set this.eventStore
    projectListeners[`${storeName}Change`] = function ({
      store
    }) {
      this[storeName] = store;
    };
    // create changeEventStore
    clsProto[`change${storeNameCap}`] = function (store, oldStore) {
      var _store;
      const me = this,
        {
          project
        } = me,
        storeProject = (_store = store) === null || _store === void 0 ? void 0 : _store.project;
      if (me.buildingProjectConfig) {
        // Capture any project found at project config time
        // to use as our shared project
        if (storeProject !== null && storeProject !== void 0 && storeProject.isProjectModel) {
          me._sharedProject = storeProject;
        }
        // Set the property in the project config object.
        // Must not go through the updater. It's too early to
        // inform host of store change.
        project[storeName] = store;
        return;
      }
      // Live update the project when in use.
      if (!me.initializingProject) {
        if (project[storeName] !== store) {
          project[`set${storeNameCap}`](store);
          store = project[storeName];
        }
      }
      // Implement processing here instead of creating a separate updater.
      // Subclasses can implement updaters.
      if (store !== oldStore) {
        if (listeners) {
          listeners.thisObj = me;
          listeners.name = `${storeName}Listeners`;
          me.detachListeners(listeners.name);
          store.ion(listeners);
        }
        // Set backing var temporarily, so it can be accessed from AttachToProjectMixin subscribers
        me[`_${storeName}`] = store;
        // Notifies classes that mix AttachToProjectMixin that we have a new XxxxxStore
        me.projectSubscribers.forEach(subscriber => {
          var _subscriber;
          (_subscriber = subscriber[`attachTo${storeNameCap}`]) === null || _subscriber === void 0 ? void 0 : _subscriber.call(subscriber, store);
        });
        me[`_${storeName}`] = null;
      }
      return store;
    };
  }
  relayProjectDataChange(event) {
    // Don't trigger change event for tree node collapse/expand
    if ((event.isExpand || event.isCollapse) && !event.records[0].fieldMap.expanded.persist) {
      return;
    }
    /**
     * Fired when data in any of the projects stores changes.
     *
     * Basically a relayed version of each store's own change event, decorated with which store it originates from.
     * See the {@link Core.data.Store#event-change store change event} documentation for more information.
     *
     * @event dataChange
     * @param {Scheduler.data.mixin.ProjectConsumer} source Owning component
     * @param {Scheduler.model.mixin.ProjectModelMixin} project Project model
     * @param {Core.data.Store} store Affected store
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
    return this.trigger('dataChange', {
      project: event.source,
      ...event,
      source: this
    });
  }
  //region WidgetClass
  // This does not need a className on Widgets.
  // Each *Class* which doesn't need 'b-' + constructor.name.toLowerCase() automatically adding
  // to the Widget it's mixed in to should implement thus.
  get widgetClass() {}
  //endregion
});

/**
 * @module Scheduler/tooltip/ClockTemplate
 */
/**
 * A template showing a clock, it consumes an object containing a date and a text
 * @private
 */
class ClockTemplate extends Base {
  static get defaultConfig() {
    return {
      minuteHeight: 8,
      minuteTop: 2,
      hourHeight: 8,
      hourTop: 2,
      handLeft: 10,
      div: document.createElement('div'),
      scheduler: null,
      // may be passed to the constructor if needed
      // `b-sch-clock-day` for calendar icon
      // `b-sch-clock-hour` for clock icon
      template(data) {
        return `<div class="b-sch-clockwrap b-sch-clock-${data.mode || this.mode} ${data.cls || ''}">
                    <div class="b-sch-clock">
                        <div class="b-sch-hour-indicator">${DateHelper.format(data.date, 'MMM')}</div>
                        <div class="b-sch-minute-indicator">${DateHelper.format(data.date, 'D')}</div>
                        <div class="b-sch-clock-dot"></div>
                    </div>
                    <span class="b-sch-clock-text">${StringHelper.encodeHtml(data.text)}</span>
                </div>`;
      }
    };
  }
  generateContent(data) {
    return this.div.innerHTML = this.template(data);
  }
  updateDateIndicator(el, date) {
    const hourIndicatorEl = el === null || el === void 0 ? void 0 : el.querySelector('.b-sch-hour-indicator'),
      minuteIndicatorEl = el === null || el === void 0 ? void 0 : el.querySelector('.b-sch-minute-indicator');
    if (date && hourIndicatorEl && minuteIndicatorEl && BrowserHelper.isBrowserEnv) {
      if (this.mode === 'hour') {
        hourIndicatorEl.style.transform = `rotate(${date.getHours() % 12 * 30}deg)`;
        minuteIndicatorEl.style.transform = `rotate(${date.getMinutes() * 6}deg)`;
      } else {
        hourIndicatorEl.style.transform = 'none';
        minuteIndicatorEl.style.transform = 'none';
      }
    }
  }
  set mode(mode) {
    this._mode = mode;
  }
  // `day` mode for calendar icon
  // `hour` mode for clock icon
  get mode() {
    if (this._mode) {
      return this._mode;
    }
    const unitLessThanDay = DateHelper.compareUnits(this.scheduler.timeAxisViewModel.timeResolution.unit, 'day') < 0,
      formatContainsHourInfo = DateHelper.formatContainsHourInfo(this.scheduler.displayDateFormat);
    return unitLessThanDay && formatContainsHourInfo ? 'hour' : 'day';
  }
  set template(template) {
    this._template = template;
  }
  /**
   * Get the clock template, which accepts an object of format { date, text }
   * @property {function(*): string}
   */
  get template() {
    return this._template;
  }
}
ClockTemplate._$name = 'ClockTemplate';

/**
 * @module Scheduler/feature/mixin/TaskEditStm
 */
/**
 * Mixin adding STM transactable behavior to TaskEdit feature.
 *
 * @mixin
 */
var TaskEditStm = (Target => class TaskEditStm extends (Target || Base) {
  static get $name() {
    return 'TaskEditStm';
  }
  captureStm(startTransaction = false) {
    const me = this,
      project = me.project,
      stm = project.getStm();
    if (me.hasStmCapture) {
      return;
    }
    me.hasStmCapture = true;
    me.stmInitiallyDisabled = stm.disabled;
    me.stmInitiallyAutoRecord = stm.autoRecord;
    if (me.stmInitiallyDisabled) {
      stm.enable();
      // it seems this branch has never been exercised by tests
      // but the intention is to stop the auto-recording while
      // task editor is active (all editing is one manual transaction)
      stm.autoRecord = false;
    } else {
      if (me.stmInitiallyAutoRecord) {
        stm.autoRecord = false;
      }
      if (stm.isRecording) {
        stm.stopTransaction();
      }
    }
    if (startTransaction) {
      this.startStmTransaction();
    }
  }
  startStmTransaction() {
    this.project.getStm().startTransaction();
  }
  commitStmTransaction() {
    const me = this,
      stm = me.project.getStm();
    if (!me.hasStmCapture) {
      throw new Error('Does not have STM capture, no transaction to commit');
    }
    if (stm.enabled) {
      stm.stopTransaction();
      if (me.stmInitiallyDisabled) {
        stm.resetQueue();
      }
    }
  }
  async rejectStmTransaction() {
    const stm = this.project.getStm(),
      {
        client
      } = this;
    if (!this.hasStmCapture) {
      throw new Error('Does not have STM capture, no transaction to reject');
    }
    if (stm.enabled) {
      var _stm$transaction;
      if ((_stm$transaction = stm.transaction) !== null && _stm$transaction !== void 0 && _stm$transaction.length) {
        client.suspendRefresh();
        stm.rejectTransaction();
        await client.resumeRefresh(true);
      } else {
        stm.stopTransaction();
      }
    }
  }
  enableStm() {
    this.project.getStm().enable();
  }
  disableStm() {
    this.project.getStm().disable();
  }
  freeStm(commitOrReject = null) {
    if (commitOrReject === true) {
      this.commitStmTransaction();
    } else if (commitOrReject === false) {
      // Note - we don't wait for async to complete here
      this.rejectStmTransaction();
    }
    const me = this,
      stm = me.project.getStm();
    if (!me.hasStmCapture) {
      return;
    }
    stm.disabled = me.stmInitiallyDisabled;
    stm.autoRecord = me.stmInitiallyAutoRecord;
    me.hasStmCapture = false;
  }
});

/**
 * @module Scheduler/feature/base/TimeSpanMenuBase
 */
/**
 * Abstract base class used by other context menu features which show the context menu for TimeAxis.
 * Using this class you can make sure the menu expects the target to disappear,
 * since it can be scroll out of the scheduling zone.
 *
 * Features that extend this class are:
 *  * {@link Scheduler/feature/EventMenu};
 *  * {@link Scheduler/feature/ScheduleMenu};
 *  * {@link Scheduler/feature/TimeAxisHeaderMenu};
 *
 * @extends Core/feature/base/ContextMenuBase
 * @abstract
 */
class TimeSpanMenuBase extends ContextMenuBase {}
TimeSpanMenuBase._$name = 'TimeSpanMenuBase';

/**
 * @module Scheduler/view/recurrence/RecurrenceConfirmationPopup
 */
/**
 * A confirmation dialog shown when modifying a recurring event or some of its occurrences.
 * For recurring events, the dialog informs the user that the change will be applied to all occurrences.
 *
 * For occurrences, the dialog lets the user choose if the change should affect all future occurrences,
 * or this occurrence only.
 *
 * Usage example:
 *
 * ```javascript
 * const confirmation = new RecurrenceConfirmationPopup();
 *
 * confirmation.confirm({
 *     eventRecord : recurringEvent,
 *     actionType  : "delete",
 *     changerFn   : () => recurringEvent.remove(event)
 * });
 * ```
 *
 * @classType recurrenceconfirmation
 * @extends Core/widget/Popup
 */
class RecurrenceConfirmationPopup extends Popup {
  static get $name() {
    return 'RecurrenceConfirmationPopup';
  }
  // Factoryable type name
  static get type() {
    return 'recurrenceconfirmation';
  }
  static get defaultConfig() {
    return {
      localizableProperties: [],
      align: 'b-t',
      autoShow: false,
      autoClose: false,
      closeAction: 'onRecurrenceClose',
      modal: true,
      centered: true,
      scrollAction: 'realign',
      constrainTo: globalThis,
      draggable: true,
      closable: true,
      floating: true,
      eventRecord: null,
      cls: 'b-sch-recurrenceconfirmation',
      bbar: {
        defaults: {
          localeClass: this
        },
        items: {
          changeSingleButton: {
            weight: 100,
            cls: 'b-raised',
            color: 'b-blue',
            text: 'L{update-only-this-btn-text}',
            onClick: 'up.onChangeSingleButtonClick'
          },
          changeMultipleButton: {
            weight: 200,
            color: 'b-green',
            text: 'L{Object.Yes}',
            onClick: 'up.onChangeMultipleButtonClick'
          },
          cancelButton: {
            weight: 300,
            color: 'b-gray',
            text: 'L{Object.Cancel}',
            onClick: 'up.onCancelButtonClick'
          }
        }
      }
    };
  }
  /**
   * Reference to the "Apply changes to multiple occurrences" button, if used
   * @property {Core.widget.Button}
   * @readonly
   */
  get changeMultipleButton() {
    return this.widgetMap.changeMultipleButton;
  }
  /**
   * Reference to the button that causes changing of the event itself only, if used
   * @property {Core.widget.Button}
   * @readonly
   */
  get changeSingleButton() {
    return this.widgetMap.changeSingleButton;
  }
  /**
   * Reference to the cancel button, if used
   * @property {Core.widget.Button}
   * @readonly
   */
  get cancelButton() {
    return this.widgetMap.cancelButton;
  }
  /**
   * Handler for "Apply changes to multiple occurrences" {@link #property-changeMultipleButton button}.
   * It calls {@link #function-processMultipleRecords} and then hides the dialog.
   */
  onChangeMultipleButtonClick() {
    this.processMultipleRecords();
    this.hide();
  }
  /**
   * Handler for the {@link #property-changeSingleButton button} that causes changing of the event itself only.
   * It calls {@link #function-processSingleRecord} and then hides the dialog.
   */
  onChangeSingleButtonClick() {
    this.processSingleRecord();
    this.hide();
  }
  /**
   * Handler for {@link #property-cancelButton cancel button}.
   * It calls `cancelFn` provided to {@link #function-confirm} call and then hides the dialog.
   */
  onCancelButtonClick() {
    this.cancelFn && this.cancelFn.call(this.thisObj);
    this.hide();
  }
  onRecurrenceClose() {
    if (this.cancelFn) {
      this.cancelFn.call(this.thisObj);
    }
    this.hide();
  }
  /**
   * Displays the confirmation.
   * Usage example:
   *
   * ```javascript
   * const popup = new RecurrenceConfirmationPopup();
   *
   * popup.confirm({
   *     eventRecord,
   *     actionType : "delete",
   *     changerFn  : () => eventStore.remove(record)
   * });
   * ```
   *
   * @param {Object} config The following config options are supported:
   * @param {Scheduler.model.EventModel} config.eventRecord   Event being modified.
   * @param {'update'|'delete'} config.actionType Type of modification to be applied to the event. Can be
   * either "update" or "delete".
   * @param {Function} config.changerFn A function that should be called to apply the change to the event upon user
   * choice.
   * @param {Function} [config.thisObj] `changerFn` and `cancelFn` functions scope.
   * @param {Function} [config.cancelFn] Function called on `Cancel` button click.
   */
  confirm(config = {}) {
    const me = this;
    ['actionType', 'eventRecord', 'title', 'html', 'changerFn', 'cancelFn', 'finalizerFn', 'thisObj'].forEach(prop => {
      if (prop in config) me[prop] = config[prop];
    });
    me.updatePopupContent();
    return super.show(config);
  }
  updatePopupContent() {
    const me = this,
      {
        changeMultipleButton,
        changeSingleButton,
        cancelButton
      } = me.widgetMap,
      {
        eventRecord,
        actionType = 'update'
      } = me,
      isMaster = eventRecord === null || eventRecord === void 0 ? void 0 : eventRecord.isRecurring;
    if (isMaster) {
      changeMultipleButton.text = me.L('L{Object.Yes}');
      me.html = me.L(`${actionType}-all-message`);
    } else {
      changeMultipleButton.text = me.L(`${actionType}-further-btn-text`);
      me.html = me.L(`${actionType}-further-message`);
    }
    changeSingleButton.text = me.L(`${actionType}-only-this-btn-text`);
    cancelButton.text = me.L('L{Object.Cancel}');
    me.width = me.L('L{width}');
    me.title = me.L(`${actionType}-title`);
  }
  /**
   * Applies changes to multiple occurrences as reaction on "Apply changes to multiple occurrences"
   * {@link #property-changeMultipleButton button} click.
   */
  processMultipleRecords() {
    const {
      eventRecord,
      changerFn,
      thisObj,
      finalizerFn
    } = this;
    eventRecord.beginBatch();
    // Apply changes to the occurrence.
    // It is not joined to any stores, so this has no consequence.
    changerFn && this.callback(changerFn, thisObj, [eventRecord]);
    // afterChange will promote it to being an new recurring base because there's still recurrence
    eventRecord.endBatch();
    finalizerFn && this.callback(finalizerFn, thisObj, [eventRecord]);
  }
  /**
   * Applies changes to a single record by making it a "real" event and adding an exception to the recurrence.
   * The method is called as reaction on clicking the {@link #property-changeSingleButton button} that causes changing of the event itself only.
   */
  processSingleRecord() {
    var _firstOccurrence;
    const {
      eventRecord,
      changerFn,
      thisObj,
      finalizerFn
    } = this;
    eventRecord.beginBatch();
    let firstOccurrence;
    // If that's a master event get its very first occurrence
    if (eventRecord !== null && eventRecord !== void 0 && eventRecord.isRecurring) {
      eventRecord.recurrence.forEachOccurrence(eventRecord.startDate, null, (occurrence, isFirst, index) => {
        // index 1 is used by to the event itself, > 1 since there might be exceptions
        if (index > 1) {
          firstOccurrence = occurrence;
          return false;
        }
      });
    }
    // turn the 1st occurrence into a new "master" event
    (_firstOccurrence = firstOccurrence) === null || _firstOccurrence === void 0 ? void 0 : _firstOccurrence.convertToRealEvent();
    // When the changes apply, because there's no recurrence, it will become an exception
    eventRecord.recurrence = null;
    // Apply changes to the occurrence.
    // It is not joined to any stores, so this has no consequence.
    changerFn && this.callback(changerFn, thisObj, [eventRecord]);
    // Must also change after the callback in case the callback sets the rule.
    // This will update the batch update data block to prevent it being set back to recurring.
    eventRecord.recurrenceRule = null;
    // afterChange will promote it to being an exception because there's no recurrence
    eventRecord.endBatch();
    finalizerFn && this.callback(finalizerFn, thisObj, [eventRecord]);
  }
  updateLocalization() {
    this.updatePopupContent();
    super.updateLocalization();
  }
}
// Register this widget type with its Factory
RecurrenceConfirmationPopup.initClass();
RecurrenceConfirmationPopup._$name = 'RecurrenceConfirmationPopup';

/**
 * @module Scheduler/view/recurrence/field/RecurrenceFrequencyCombo
 */
/**
 * A combobox field allowing to pick frequency in the {@link Scheduler.view.recurrence.RecurrenceEditor recurrence dialog}.
 *
 * @extends Core/widget/Combo
 * @classType recurrencefrequencycombo
 */
class RecurrenceFrequencyCombo extends Combo {
  static $name = 'RecurrenceFrequencyCombo';
  // Factoryable type name
  static type = 'recurrencefrequencycombo';
  static configurable = {
    editable: false,
    displayField: 'text',
    valueField: 'value',
    localizeDisplayFields: true,
    addNone: false
  };
  buildItems() {
    return [...(this.addNone ? [{
      text: 'L{None}',
      value: 'NONE'
    }] : []), {
      value: 'DAILY',
      text: 'L{Daily}'
    }, {
      value: 'WEEKLY',
      text: 'L{Weekly}'
    }, {
      value: 'MONTHLY',
      text: 'L{Monthly}'
    }, {
      value: 'YEARLY',
      text: 'L{Yearly}'
    }];
  }
}
// Register this widget type with its Factory
RecurrenceFrequencyCombo.initClass();
RecurrenceFrequencyCombo._$name = 'RecurrenceFrequencyCombo';

/**
 * @module Scheduler/view/recurrence/field/RecurrenceDaysCombo
 */
/**
 * A combobox field allowing to pick days for the `Monthly` and `Yearly` mode in the {@link Scheduler.view.recurrence.RecurrenceEditor recurrence dialog}.
 *
 * @extends Core/widget/Combo
 * @classType recurrencedayscombo
 */
class RecurrenceDaysCombo extends Combo {
  static get $name() {
    return 'RecurrenceDaysCombo';
  }
  // Factoryable type name
  static get type() {
    return 'recurrencedayscombo';
  }
  static get defaultConfig() {
    const allDaysValueAsArray = ['SU', 'MO', 'TU', 'WE', 'TH', 'FR', 'SA'],
      allDaysValue = allDaysValueAsArray.join(',');
    return {
      allDaysValue,
      editable: false,
      defaultValue: allDaysValue,
      workingDaysValue: allDaysValueAsArray.filter((day, index) => !DateHelper.nonWorkingDays[index]).join(','),
      nonWorkingDaysValue: allDaysValueAsArray.filter((day, index) => DateHelper.nonWorkingDays[index]).join(','),
      splitCls: 'b-recurrencedays-split',
      displayField: 'text',
      valueField: 'value'
    };
  }
  buildItems() {
    const me = this;
    me._weekDays = null;
    return me.weekDays.concat([{
      value: me.allDaysValue,
      text: me.L('L{day}'),
      cls: me.splitCls
    }, {
      value: me.workingDaysValue,
      text: me.L('L{weekday}')
    }, {
      value: me.nonWorkingDaysValue,
      text: me.L('L{weekend day}')
    }]);
  }
  get weekDays() {
    const me = this;
    if (!me._weekDays) {
      const weekStartDay = DateHelper.weekStartDay;
      const dayNames = DateHelper.getDayNames().map((text, index) => ({
        text,
        value: RecurrenceDayRuleEncoder.encodeDay(index)
      }));
      // we should start week w/ weekStartDay
      me._weekDays = dayNames.slice(weekStartDay).concat(dayNames.slice(0, weekStartDay));
    }
    return me._weekDays;
  }
  set value(value) {
    const me = this;
    if (value && Array.isArray(value)) {
      value = value.join(',');
    }
    // if the value has no matching option in the store we need to use default value
    if (!value || !me.store.findRecord('value', value)) {
      value = me.defaultValue;
    }
    super.value = value;
  }
  get value() {
    let value = super.value;
    if (value && Array.isArray(value)) {
      value = value.join(',');
    }
    return value;
  }
}
// Register this widget type with its Factory
RecurrenceDaysCombo.initClass();
RecurrenceDaysCombo._$name = 'RecurrenceDaysCombo';

/**
 * @module Scheduler/view/recurrence/field/RecurrenceDaysButtonGroup
 */
/**
 * A segmented button field allowing to pick days for the "Weekly" mode in the {@link Scheduler.view.recurrence.RecurrenceEditor recurrence dialog}.
 *
 * {@inlineexample Scheduler/view/RecurrenceDaysButtonGroup.js}
 *
 * @extends Core/widget/ButtonGroup
 */
class RecurrenceDaysButtonGroup extends ButtonGroup {
  static get $name() {
    return 'RecurrenceDaysButtonGroup';
  }
  // Factoryable type name
  static get type() {
    return 'recurrencedaysbuttongroup';
  }
  static get defaultConfig() {
    return {
      defaults: {
        cls: 'b-raised',
        toggleable: true
      }
    };
  }
  construct(config = {}) {
    const me = this;
    config.columns = 7;
    config.items = me.buildItems();
    super.construct(config);
  }
  updateItemText(item) {
    const day = RecurrenceDayRuleEncoder.decodeDay(item.value)[0];
    item.text = DateHelper.getDayName(day).substring(0, 3);
  }
  buildItems() {
    const me = this;
    if (!me.__items) {
      const weekStartDay = DateHelper.weekStartDay;
      const dayNames = DateHelper.getDayNames().map((text, index) => ({
        text: text.substring(0, 3),
        value: RecurrenceDayRuleEncoder.encodeDay(index)
      }));
      // we should start week w/ weekStartDay
      me.__items = dayNames.slice(weekStartDay).concat(dayNames.slice(0, weekStartDay));
    }
    return me.__items;
  }
  set value(value) {
    if (value && Array.isArray(value)) {
      value = value.join(',');
    }
    super.value = value;
  }
  get value() {
    let value = super.value;
    if (value && Array.isArray(value)) {
      value = value.join(',');
    }
    return value;
  }
  onLocaleChange() {
    // update button texts on locale switch
    this.items.forEach(this.updateItemText, this);
  }
  updateLocalization() {
    this.onLocaleChange();
    super.updateLocalization();
  }
  get widgetClassList() {
    const classList = super.widgetClassList;
    // to look more like a real field
    classList.push('b-field');
    return classList;
  }
}
// Register this widget type with its Factory
RecurrenceDaysButtonGroup.initClass();
RecurrenceDaysButtonGroup._$name = 'RecurrenceDaysButtonGroup';

/**
 * A segmented button field allowing to pick month days for the `Monthly` mode in the {@link Scheduler.view.recurrence.RecurrenceEditor recurrence dialog}.
 *
 * @extends Core/widget/ButtonGroup
 */
class RecurrenceMonthDaysButtonGroup extends ButtonGroup {
  static get $name() {
    return 'RecurrenceMonthDaysButtonGroup';
  }
  // Factoryable type name
  static get type() {
    return 'recurrencemonthdaysbuttongroup';
  }
  static get defaultConfig() {
    return {
      defaults: {
        toggleable: true,
        cls: 'b-raised'
      }
    };
  }
  get minValue() {
    return 1;
  }
  get maxValue() {
    return 31;
  }
  construct(config = {}) {
    const me = this;
    config.columns = 7;
    config.items = me.buildItems();
    super.construct(config);
  }
  buildItems() {
    const me = this,
      items = [];
    for (let value = me.minValue; value <= me.maxValue; value++) {
      // button config
      items.push({
        text: value + '',
        value
      });
    }
    return items;
  }
  get widgetClassList() {
    const classList = super.widgetClassList;
    // to look more like a real field
    classList.push('b-field');
    return classList;
  }
}
// Register this widget type with its Factory
RecurrenceMonthDaysButtonGroup.initClass();
RecurrenceMonthDaysButtonGroup._$name = 'RecurrenceMonthDaysButtonGroup';

/**
 * A segmented button field allowing to pick months for the `Yearly` mode in the {@link Scheduler.view.recurrence.RecurrenceEditor recurrence dialog}.
 *
 * @extends Core/widget/ButtonGroup
 */
class RecurrenceMonthsButtonGroup extends ButtonGroup {
  static get $name() {
    return 'RecurrenceMonthsButtonGroup';
  }
  // Factoryable type name
  static get type() {
    return 'recurrencemonthsbuttongroup';
  }
  static get defaultConfig() {
    return {
      defaults: {
        toggleable: true,
        cls: 'b-raised'
      }
    };
  }
  construct(config = {}) {
    const me = this;
    config.columns = 4;
    config.items = me.buildItems();
    super.construct(config);
  }
  buildItems() {
    return DateHelper.getMonthNames().map((item, index) => ({
      text: item.substring(0, 3),
      value: index + 1 // 1-based
    }));
  }

  updateItemText(item) {
    item.text = DateHelper.getMonthName(item.value - 1).substring(0, 3);
  }
  onLocaleChange() {
    // update button texts on locale switch
    this.items.forEach(this.updateItemText, this);
  }
  updateLocalization() {
    this.onLocaleChange();
    super.updateLocalization();
  }
  get widgetClassList() {
    const classList = super.widgetClassList;
    // to look more like a real field
    classList.push('b-field');
    return classList;
  }
}
// Register this widget type with its Factory
RecurrenceMonthsButtonGroup.initClass();
RecurrenceMonthsButtonGroup._$name = 'RecurrenceMonthsButtonGroup';

/**
 * @module Scheduler/view/recurrence/field/RecurrenceStopConditionCombo
 */
/**
 * A combobox field allowing to choose stop condition for the recurrence in the {@link Scheduler.view.recurrence.RecurrenceEditor recurrence dialog}.
 *
 * @extends Core/widget/Combo
 * @classType recurrencestopconditioncombo
 */
class RecurrenceStopConditionCombo extends Combo {
  static get $name() {
    return 'RecurrenceStopConditionCombo';
  }
  // Factoryable type name
  static get type() {
    return 'recurrencestopconditioncombo';
  }
  static get defaultConfig() {
    return {
      editable: false,
      placeholder: 'Never',
      displayField: 'text',
      valueField: 'value'
    };
  }
  buildItems() {
    return [{
      value: 'never',
      text: this.L('L{Never}')
    }, {
      value: 'count',
      text: this.L('L{After}')
    }, {
      value: 'date',
      text: this.L('L{On date}')
    }];
  }
  set value(value) {
    // Use 'never' instead of falsy value
    value = value || 'never';
    super.value = value;
  }
  get value() {
    return super.value;
  }
  get recurrence() {
    return this._recurrence;
  }
  set recurrence(recurrence) {
    let value = null;
    if (recurrence.endDate) {
      value = 'date';
    } else if (recurrence.count) {
      value = 'count';
    }
    this._recurrence = recurrence;
    this.value = value;
  }
}
// Register this widget type with its Factory
RecurrenceStopConditionCombo.initClass();
RecurrenceStopConditionCombo._$name = 'RecurrenceStopConditionCombo';

/**
 * @module Scheduler/view/recurrence/field/RecurrencePositionsCombo
 */
/**
 * A combobox field allowing to specify day positions in the {@link Scheduler.view.recurrence.RecurrenceEditor recurrence editor}.
 *
 * @extends Core/widget/Combo
 * @classType recurrencepositionscombo
 */
class RecurrencePositionsCombo extends Combo {
  static get $name() {
    return 'RecurrencePositionsCombo';
  }
  // Factoryable type name
  static get type() {
    return 'recurrencepositionscombo';
  }
  static get defaultConfig() {
    return {
      editable: false,
      splitCls: 'b-sch-recurrencepositions-split',
      displayField: 'text',
      valueField: 'value',
      defaultValue: 1,
      maxPosition: 5
    };
  }
  buildItems() {
    return this.buildDayNumbers().concat([{
      value: '-1',
      text: this.L('L{position-1}'),
      cls: this.splitCls
    }]);
  }
  buildDayNumbers() {
    return ArrayHelper.populate(this.maxPosition, i => ({
      value: i + 1,
      text: this.L(`position${i + 1}`)
    }));
  }
  set value(value) {
    const me = this;
    if (value && Array.isArray(value)) {
      value = value.join(',');
    }
    // if the value has no matching option in the store we need to use default value
    if (!value || !me.store.findRecord('value', value)) {
      value = me.defaultValue;
    }
    super.value = value;
  }
  get value() {
    const value = super.value;
    return value ? `${value}`.split(',').map(item => parseInt(item, 10)) : [];
  }
}
// Register this widget type with its Factory
RecurrencePositionsCombo.initClass();
RecurrencePositionsCombo._$name = 'RecurrencePositionsCombo';

/**
 * @module Scheduler/view/recurrence/RecurrenceEditorPanel
 */
/**
 * Panel containing fields used to edit a {@link Scheduler.model.RecurrenceModel recurrence model}. Used by
 * {@link Scheduler/view/recurrence/RecurrenceEditor}, and by the recurrence tab in Scheduler Pro's event editor.
 *
 * Not intended to be used separately.
 *
 * @extends Core/widget/Panel
 * @classType recurrenceeditorpanel
 * @private
 */
class RecurrenceEditorPanel extends Panel {
  static $name = 'RecurrenceEditorPanel';
  static type = 'recurrenceeditorpanel';
  static configurable = {
    cls: 'b-recurrenceeditor',
    record: false,
    addNone: false,
    items: {
      frequencyField: {
        type: 'recurrencefrequencycombo',
        name: 'frequency',
        label: 'L{RecurrenceEditor.Frequency}',
        weight: 10,
        onChange: 'up.onFrequencyFieldChange',
        addNone: 'up.addNone'
      },
      intervalField: {
        type: 'numberfield',
        weight: 15,
        name: 'interval',
        label: 'L{RecurrenceEditor.Every}',
        min: 1,
        required: true
      },
      daysButtonField: {
        type: 'recurrencedaysbuttongroup',
        weight: 20,
        name: 'days',
        forFrequency: 'WEEKLY'
      },
      // the radio button enabling "monthDaysButtonField" in MONTHLY mode
      monthDaysRadioField: {
        type: 'checkbox',
        weight: 30,
        toggleGroup: 'radio',
        forFrequency: 'MONTHLY',
        label: 'L{RecurrenceEditor.Each}',
        checked: true,
        onChange: 'up.onMonthDaysRadioFieldChange'
      },
      monthDaysButtonField: {
        type: 'recurrencemonthdaysbuttongroup',
        weight: 40,
        name: 'monthDays',
        forFrequency: 'MONTHLY'
      },
      monthsButtonField: {
        type: 'recurrencemonthsbuttongroup',
        weight: 50,
        name: 'months',
        forFrequency: 'YEARLY'
      },
      // the radio button enabling positions & days combos in MONTHLY & YEARLY modes
      positionAndDayRadioField: {
        type: 'checkbox',
        weight: 60,
        toggleGroup: 'radio',
        forFrequency: 'MONTHLY|YEARLY',
        label: 'L{RecurrenceEditor.On the}',
        onChange: 'up.onPositionAndDayRadioFieldChange'
      },
      positionsCombo: {
        type: 'recurrencepositionscombo',
        weight: 80,
        name: 'positions',
        forFrequency: 'MONTHLY|YEARLY'
      },
      daysCombo: {
        type: 'recurrencedayscombo',
        weight: 90,
        name: 'days',
        forFrequency: 'MONTHLY|YEARLY',
        flex: 1
      },
      stopRecurrenceField: {
        type: 'recurrencestopconditioncombo',
        weight: 100,
        label: 'L{RecurrenceEditor.End repeat}',
        onChange: 'up.onStopRecurrenceFieldChange'
      },
      countField: {
        type: 'numberfield',
        weight: 110,
        name: 'count',
        min: 2,
        required: true,
        disabled: true,
        label: ' '
      },
      endDateField: {
        type: 'datefield',
        weight: 120,
        name: 'endDate',
        hidden: true,
        disabled: true,
        label: ' ',
        required: true
      }
    }
  };
  updateRecord(record) {
    super.updateRecord(record);
    const me = this,
      {
        frequencyField,
        daysButtonField,
        monthDaysButtonField,
        monthsButtonField,
        monthDaysRadioField,
        positionAndDayRadioField,
        stopRecurrenceField
      } = me.widgetMap;
    if (record) {
      const event = record.timeSpan,
        startDate = event === null || event === void 0 ? void 0 : event.startDate;
      // some fields default values are calculated based on event "startDate" value
      if (startDate) {
        // if no "days" value provided
        if (!record.days || !record.days.length) {
          daysButtonField.value = [RecurrenceDayRuleEncoder.encodeDay(startDate.getDay())];
        }
        // if no "monthDays" value provided
        if (!record.monthDays || !record.monthDays.length) {
          monthDaysButtonField.value = startDate.getDate();
        }
        // if no "months" value provided
        if (!record.months || !record.months.length) {
          monthsButtonField.value = startDate.getMonth() + 1;
        }
      }
      // if the record has both "days" & "positions" fields set check "On the" checkbox
      if (record.days && record.positions) {
        positionAndDayRadioField.check();
        if (!me.isPainted) {
          monthDaysRadioField.uncheck();
        }
      } else {
        monthDaysRadioField.check();
        if (!me.isPainted) {
          positionAndDayRadioField.uncheck();
        }
      }
      stopRecurrenceField.recurrence = record;
    } else {
      frequencyField.value = 'NONE';
    }
  }
  /**
   * Updates the provided recurrence model with the contained form data.
   * If recurrence model is not provided updates the last loaded recurrence model.
   * @internal
   */
  syncEventRecord(recurrence) {
    // get values relevant to the RecurrenceModel (from enabled fields only)
    const values = this.getValues(w => w.name in recurrence && !w.disabled);
    // Disabled field does not contribute to values, clear manually
    if (!('endDate' in values)) {
      values.endDate = null;
    }
    if (!('count' in values)) {
      values.count = null;
    }
    recurrence.set(values);
  }
  toggleStopFields() {
    const me = this,
      {
        countField,
        endDateField
      } = me.widgetMap;
    switch (me.widgetMap.stopRecurrenceField.value) {
      case 'count':
        countField.show();
        countField.enable();
        endDateField.hide();
        endDateField.disable();
        break;
      case 'date':
        countField.hide();
        countField.disable();
        endDateField.show();
        endDateField.enable();
        break;
      default:
        countField.hide();
        endDateField.hide();
        countField.disable();
        endDateField.disable();
    }
  }
  onMonthDaysRadioFieldChange({
    checked
  }) {
    const {
      monthDaysButtonField
    } = this.widgetMap;
    monthDaysButtonField.disabled = !checked || !this.isWidgetAvailableForFrequency(monthDaysButtonField);
  }
  onPositionAndDayRadioFieldChange({
    checked
  }) {
    const {
      daysCombo,
      positionsCombo
    } = this.widgetMap;
    // toggle day & positions combos
    daysCombo.disabled = positionsCombo.disabled = !checked || !this.isWidgetAvailableForFrequency(daysCombo);
  }
  onStopRecurrenceFieldChange() {
    this.toggleStopFields();
  }
  isWidgetAvailableForFrequency(widget, frequency = this.widgetMap.frequencyField.value) {
    return !widget.forFrequency || widget.forFrequency.includes(frequency);
  }
  onFrequencyFieldChange({
    value,
    oldValue,
    valid
  }) {
    const me = this,
      items = me.queryAll(w => 'forFrequency' in w),
      {
        intervalField,
        stopRecurrenceField
      } = me.widgetMap;
    if (valid && value) {
      for (let i = 0; i < items.length; i++) {
        const item = items[i];
        if (me.isWidgetAvailableForFrequency(item, value)) {
          item.show();
          item.enable();
        } else {
          item.hide();
          item.disable();
        }
      }
      // Special handling of NONE
      intervalField.hidden = stopRecurrenceField.hidden = value === 'NONE';
      if (value !== 'NONE') {
        intervalField.hint = me.L(`L{RecurrenceEditor.${value}intervalUnit}`);
      }
      // When a non-recurring record is loaded, intervalField is set to empty. We want it to default to 1 here
      // to not look weird (defaults to 1 on the data layer)
      if (oldValue === 'NONE' && intervalField.value == null) {
        intervalField.value = 1;
      }
      me.toggleFieldsState();
    }
  }
  toggleFieldsState() {
    const me = this,
      {
        widgetMap
      } = me;
    me.onMonthDaysRadioFieldChange({
      checked: widgetMap.monthDaysRadioField.checked
    });
    me.onPositionAndDayRadioFieldChange({
      checked: widgetMap.positionAndDayRadioField.checked
    });
    me.onStopRecurrenceFieldChange();
  }
  updateLocalization() {
    // do extra labels translation (not auto-translated yet)
    const {
      countField,
      intervalField,
      frequencyField
    } = this.widgetMap;
    countField.hint = this.L('L{RecurrenceEditor.time(s)}');
    if (frequencyField.value && frequencyField.value !== 'NONE') {
      intervalField.hint = this.L(`L{RecurrenceEditor.${frequencyField.value}intervalUnit}`);
    }
    super.updateLocalization();
  }
}
// Register this widget type with its Factory
RecurrenceEditorPanel.initClass();
RecurrenceEditorPanel._$name = 'RecurrenceEditorPanel';

/**
 * @module Scheduler/feature/EventMenu
 */
/**
 * Displays a context menu for events. Items are populated by other features and/or application code.
 *
 * {@inlineexample Scheduler/feature/EventMenu.js}
 *
 * ### Default event menu items
 *
 * Here is the list of menu items provided by the feature and populated by the other features:
 *
 * | Reference       | Text           | Weight | Feature                                  | Description                                                       |
 * |-----------------|----------------|--------|------------------------------------------|-------------------------------------------------------------------|
 * | `editEvent`     | Edit event     | 100    | {@link Scheduler/feature/EventEdit}      | Edit in the event editor. Hidden when read-only                   |
 * | `copyEvent`     | Copy event     | 110    | {@link Scheduler/feature/EventCopyPaste} | Copy event or assignment. Hidden when read-only                   |
 * | `cutEvent `     | Cut event      | 120    | {@link Scheduler/feature/EventCopyPaste} | Cut event or assignment. Hidden when read-only                    |
 * | `deleteEvent`   | Delete event   | 200    | *This feature*                           | Remove event. Hidden when read-only                               |
 * | `unassignEvent` | Unassign event | 300    | *This feature*                           | Unassign event. Hidden when read-only, shown for multi-assignment |
 * | `splitEvent`    | Split event    | 650    | *Scheduler Pro only*                     | Split an event into two segments at the mouse position            |
 * | `renameSegment` | Rename segment | 660    | *Scheduler Pro only*                     | Show an inline editor to rename the segment                       |
 *
 * ### Customizing the menu items
 *
 * The menu items in the Event menu can be customized, existing items can be changed or removed,
 * and new items can be added. This is handled using the `items` config of the feature.
 *
 * Add extra items for all events:
 *
 * ```javascript
 * const scheduler = new Scheduler({
 *     features : {
 *         eventMenu : {
 *             items : {
 *                 extraItem : {
 *                     text : 'Extra',
 *                     icon : 'b-fa b-fa-fw b-fa-flag',
 *                     onItem({eventRecord}) {
 *                         eventRecord.flagged = true;
 *                     }
 *                 }
 *             }
 *         }
 *     }
 * });
 * ```
 *
 * Remove existing items:
 *
 * ```javascript
 * const scheduler = new Scheduler({
 *     features : {
 *         eventMenu : {
 *             items : {
 *                 deleteEvent   : false,
 *                 unassignEvent : false
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
 *         eventMenu : {
 *             items : {
 *                 deleteEvent : {
 *                     text : 'Delete booking'
 *                 }
 *             }
 *         }
 *     }
 * });
 * ```
 *
 * Manipulate existing items for all events or specific events:
 *
 * ```javascript
 * const scheduler = new Scheduler({
 *     features : {
 *         eventMenu : {
 *             // Process items before menu is shown
 *             processItems({eventRecord, items}) {
 *                  // Push an extra item for conferences
 *                  if (eventRecord.type === 'conference') {
 *                      items.showSessionItem = {
 *                          text : 'Show sessions',
 *                          onItem({eventRecord}) {
 *                              // ...
 *                          }
 *                      };
 *                  }
 *
 *                  // Do not show menu for secret events
 *                  if (eventRecord.type === 'secret') {
 *                      return false;
 *                  }
 *             }
 *         }
 *     }
 * });
 * ```
 *
 * Note that the {@link #property-menuContext} is applied to the Menu's `item` event, so your `onItem`
 * handler's single event parameter also contains the following properties:
 *
 * - **source** The {@link Scheduler.view.Scheduler} who's UI was right clicked.
 * - **targetElement** The element right clicked on.
 * - **eventRecord** The {@link Scheduler.model.EventModel event record} clicked on.
 * - **resourceRecord** The {@link Scheduler.model.ResourceModel resource record} clicked on.
 * - **assignmentRecord** The {@link Scheduler.model.AssignmentModel assignment record} clicked on.
 *
 * Full information of the menu customization can be found in the "Customizing the Event menu, the Schedule menu, and the TimeAxisHeader menu" guide.
 *
 * This feature is **enabled** by default
 *
 * @extends Scheduler/feature/base/TimeSpanMenuBase
 * @demo Scheduler/eventmenu
 * @classtype eventMenu
 * @feature
 */
class EventMenu extends TimeSpanMenuBase {
  //region Config
  static get $name() {
    return 'EventMenu';
  }
  /**
   * @member {Object} menuContext
   * An informational object containing contextual information about the last activation
   * of the context menu. The base properties are listed below.
   * @property {Event} menuContext.domEvent The initiating event.
   * @property {Event} menuContext.event DEPRECATED: The initiating event.
   * @property {Number[]} menuContext.point The client `X` and `Y` position of the initiating event.
   * @property {HTMLElement} menuContext.targetElement The target to which the menu is being applied.
   * @property {Object<String,MenuItemConfig>} menuContext.items The context menu **configuration** items.
   * @property {Core.data.Model[]} menuContext.selection The record selection in the client (Grid, Scheduler, Gantt or Calendar).
   * @property {Scheduler.model.EventModel} menuContext.eventRecord The event record clicked on.
   * @property {Scheduler.model.ResourceModel} menuContext.resourceRecord The resource record clicked on.
   * @property {Scheduler.model.AssignmentModel} menuContext.assignmentRecord The assignment record clicked on.
   * @readonly
   */
  static get configurable() {
    return {
      /**
       * A function called before displaying the menu that allows manipulations of its items.
       * Returning `false` from this function prevents the menu being shown.
       *
       * ```javascript
       * features         : {
       *    eventMenu : {
       *         processItems({ items, eventRecord, assignmentRecord, resourceRecord }) {
       *             // Add or hide existing items here as needed
       *             items.myAction = {
       *                 text   : 'Cool action',
       *                 icon   : 'b-fa b-fa-fw b-fa-ban',
       *                 onItem : () => console.log(`Clicked ${eventRecord.name}`),
       *                 weight : 1000 // Move to end
       *             };
       *
       *            if (!eventRecord.allowDelete) {
       *                 items.deleteEvent.hidden = true;
       *             }
       *         }
       *     }
       * },
       * ```
       * @param {Object} context An object with information about the menu being shown
       * @param {Scheduler.model.EventModel} context.eventRecord The record representing the current event
       * @param {Scheduler.model.ResourceModel} context.resourceRecord The record representing the current resource
       * @param {Scheduler.model.AssignmentModel} context.assignmentRecord The assignment record
       * @param {Object<String,MenuItemConfig>} context.items An object containing the {@link Core.widget.MenuItem menu item} configs keyed by their id
       * @param {Event} context.event The DOM event object that triggered the show
       * @config {Function}
       * @preventable
       */
      processItems: null,
      type: 'event'
      /**
       * This is a preconfigured set of items used to create the default context menu.
       *
       * The `items` provided by this feature are listed below. These are the property names which you may
       * configure:
       *
       * - `deleteEvent` Deletes the context event.
       * - `unassignEvent` Unassigns the context event from the current resource (only added when multi assignment is used).
       *
       * To remove existing items, set corresponding keys `null`:
       *
       * ```javascript
       * const scheduler = new Scheduler({
       *     features : {
       *         eventMenu : {
       *             items : {
       *                 deleteEvent   : null,
       *                 unassignEvent : null
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
    };
  }

  static get pluginConfig() {
    const config = super.pluginConfig;
    config.chain.push('populateEventMenu');
    return config;
  }
  //endregion
  //region Events
  /**
   * This event fires on the owning Scheduler before the context menu is shown for an event. Allows manipulation of the items
   * to show in the same way as in `processItems`. Returning `false` from a listener prevents the menu from
   * being shown.
   * @event eventMenuBeforeShow
   * @on-owner
   * @preventable
   * @param {Scheduler.view.Scheduler} source
   * @param {Object<String,MenuItemConfig>} items Menu item configs
   * @param {Scheduler.model.EventModel} eventRecord Event record for which the menu was triggered
   * @param {Scheduler.model.ResourceModel} resourceRecord Resource record
   * @param {Scheduler.model.AssignmentModel} assignmentRecord Assignment record, if assignments are used
   * @param {HTMLElement} eventElement
   * @param {MouseEvent} [event] Pointer event which triggered the context menu (if any)
   */
  /**
   * This event fires on the owning Scheduler when an item is selected in the context menu.
   * @event eventMenuItem
   * @on-owner
   * @param {Scheduler.view.Scheduler} source
   * @param {Core.widget.MenuItem} item
   * @param {Scheduler.model.EventModel} eventRecord
   * @param {Scheduler.model.ResourceModel} resourceRecord
   * @param {Scheduler.model.AssignmentModel} assignmentRecord Assignment record, if assignments are used
   * @param {HTMLElement} eventElement
   */
  /**
   * This event fires on the owning Scheduler after showing the context menu for an event
   * @event eventMenuShow
   * @on-owner
   * @param {Scheduler.view.Scheduler} source
   * @param {Core.widget.Menu} menu The menu
   * @param {Scheduler.model.EventModel} eventRecord Event record for which the menu was triggered
   * @param {Scheduler.model.ResourceModel} resourceRecord Resource record
   * @param {Scheduler.model.AssignmentModel} assignmentRecord Assignment record, if assignments are used
   * @param {HTMLElement} eventElement
   */
  //endregion
  get resourceStore() {
    // In horizontal mode, we use store (might be a display store), in vertical & calendar we use resourceStore
    return this.client.isHorizontal ? this.client.store : this.client.resourceStore;
  }
  getDataFromEvent(event) {
    var _ref;
    const data = super.getDataFromEvent(event),
      eventElement = data.targetElement,
      {
        client
      } = this,
      eventRecord = client.resolveEventRecord(eventElement),
      // For vertical mode the resource must be resolved from the event
      resourceRecord = eventRecord && ((_ref = client.resolveResourceRecord(eventElement) || this.resourceStore.last) === null || _ref === void 0 ? void 0 : _ref.$original),
      assignmentRecord = eventRecord && client.resolveAssignmentRecord(eventElement);
    return Object.assign(data, {
      eventElement,
      eventRecord,
      resourceRecord,
      assignmentRecord
    });
  }
  getTargetElementFromEvent({
    target
  }) {
    return target.closest(this.client.eventSelector) || target;
  }
  shouldShowMenu(eventParams) {
    return eventParams.eventRecord;
  }
  /**
   * Shows context menu for the provided event. If record is not rendered (outside of time span/filtered)
   * menu won't appear.
   * @param {Scheduler.model.EventModel} eventRecord Event record to show menu for.
   * @param {Object} [options]
   * @param {HTMLElement} options.targetElement Element to align context menu to.
   * @param {MouseEvent} options.event Browser event.
   * If provided menu will be aligned according to clientX/clientY coordinates.
   * If omitted, context menu will be centered to event element.
   */
  showContextMenuFor(eventRecord, {
    targetElement,
    event
  } = {}) {
    if (this.disabled) {
      return;
    }
    if (!targetElement) {
      targetElement = this.getElementFromRecord(eventRecord);
      // If record is not rendered, do nothing
      if (!targetElement) {
        return;
      }
    }
    DomHelper.triggerMouseEvent(targetElement, this.tiggerEvent);
  }
  getElementFromRecord(record) {
    return this.client.getElementsFromEventRecord(record)[0];
  }
  populateEventMenu({
    items,
    eventRecord,
    assignmentRecord
  }) {
    const {
      client
    } = this;
    items.deleteEvent = {
      disabled: eventRecord.readOnly || (assignmentRecord === null || assignmentRecord === void 0 ? void 0 : assignmentRecord.readOnly),
      hidden: client.readOnly
    };
    items.unassignEvent = {
      disabled: eventRecord.readOnly || (assignmentRecord === null || assignmentRecord === void 0 ? void 0 : assignmentRecord.readOnly),
      hidden: client.readOnly || client.eventStore.usesSingleAssignment
    };
  }
  // This generates the fixed, unchanging part of the items and is only called once
  // to generate the baseItems of the feature.
  // The dynamic parts which are set by populateEventMenu have this merged into them.
  changeItems(items) {
    const {
      client
    } = this;
    return Objects.merge({
      deleteEvent: {
        text: 'L{SchedulerBase.Delete event}',
        icon: 'b-icon b-icon-trash',
        weight: 200,
        onItem({
          menu,
          eventRecord
        }) {
          var _menu$focusInEvent;
          // We must synchronously push focus back into the menu's triggering
          // event so that our beforeRemove handlers can move focus onwards
          // to the closest remaining event.
          // Otherwise, the menu's default hide processing on hide will attempt
          // to move focus back to the menu's triggering event which will
          // by then have been deleted.
          const revertTarget = (_menu$focusInEvent = menu.focusInEvent) === null || _menu$focusInEvent === void 0 ? void 0 : _menu$focusInEvent.relatedTarget;
          if (revertTarget) {
            revertTarget.focus();
            client.navigator.activeItem = revertTarget;
          }
          client.removeEvents(client.isEventSelected(eventRecord) ? client.selectedEvents : [eventRecord]);
        }
      },
      unassignEvent: {
        text: 'L{SchedulerBase.Unassign event}',
        icon: 'b-icon b-icon-unassign',
        weight: 300,
        onItem({
          menu,
          eventRecord,
          resourceRecord
        }) {
          var _menu$focusInEvent2;
          // We must synchronously push focus back into the menu's triggering
          // event so that our beforeRemove handlers can move focus onwards
          // to the closest remaining event.
          // Otherwise, the menu's default hide processing on hide will attempt
          // to move focus back to the menu's triggering event which will
          // by then have been deleted.
          const revertTarget = (_menu$focusInEvent2 = menu.focusInEvent) === null || _menu$focusInEvent2 === void 0 ? void 0 : _menu$focusInEvent2.relatedTarget;
          if (revertTarget) {
            revertTarget.focus();
            client.navigator.activeItem = revertTarget;
          }
          if (client.isEventSelected(eventRecord)) {
            client.assignmentStore.remove(client.selectedAssignments);
          } else {
            eventRecord.unassign(resourceRecord);
          }
        }
      }
    }, items);
  }
}
EventMenu.featureClass = '';
EventMenu._$name = 'EventMenu';
GridFeatureManager.registerFeature(EventMenu, true, 'Scheduler');
GridFeatureManager.registerFeature(EventMenu, false, 'ResourceHistogram');

/**
 * @module Scheduler/feature/ScheduleMenu
 */
/**
 * Displays a context menu for empty parts of the schedule. Items are populated in the first place
 * by configurations of this Feature, then by other features and/or application code.
 *
 * ### Default scheduler zone menu items
 *
 * The Scheduler menu feature provides only one item:
 *
 * | Reference      | Text        | Weight | Feature                                  | Description                                                           |
 * |----------------|-------------|--------|------------------------------------------|-----------------------------------------------------------------------|
 * | `addEvent`     | Add event   | 100    | *This feature*                           | Add new event at the target time and resource. Hidden when read-only  |
 * | `pasteEvent`   | Paste event | 110    | {@link Scheduler/feature/EventCopyPaste} | Paste event at the target time and resource. Hidden when is read-only |
 *
 * ### Customizing the menu items
 *
 * The menu items in the Scheduler menu can be customized, existing items can be changed or removed,
 * and new items can be added. This is handled using the `items` config of the feature.
 *
 * Add extra item:
 *
 * ```javascript
 * const scheduler = new Scheduler({
 *     features : {
 *         scheduleMenu : {
 *             items : {
 *                 extraItem : {
 *                     text : 'Extra',
 *                     icon : 'b-fa b-fa-fw b-fa-flag',
 *                     onItem({date, resourceRecord, items}) {
 *                         // Custom date based action
 *                     }
 *                 }
 *             }
 *         }
 *     }
 * });
 * ```
 *
 * Remove existing item:
 *
 * ```javascript
 * const scheduler = new Scheduler({
 *     features : {
 *         scheduleMenu : {
 *             items : {
 *                 addEvent : false
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
 *         scheduleMenu : {
 *             items : {
 *                 addEvent : {
 *                     text : 'Create new booking'
 *                 }
 *             }
 *         }
 *     }
 * });
 * ```
 *
 * Manipulate existing items:
 *
 * ```javascript
 * const scheduler = new Scheduler({
 *     features : {
 *         scheduleMenu : {
 *             // Process items before menu is shown
 *             processItems({date, resourceRecord, items}) {
 *                  // Add an extra item for ancient times
 *                  if (date < new Date(2018, 11, 17)) {
 *                      items.modernize = {
 *                          text : 'Modernize',
 *                          ontItem({date}) {
 *                              // Custom date based action
 *                          }
 *                      };
 *                  }
 *
 *                  // Do not show menu for Sundays
 *                  if (date.getDay() === 0) {
 *                      return false;
 *                  }
 *             }
 *         }
 *     }
 * });
 * ```
 *
 * Full information of the menu customization can be found in the "Customizing the Event menu, the Schedule menu, and the TimeAxisHeader menu" guide.
 *
 * This feature is **enabled** by default
 *
 * @demo Scheduler/basic
 * @extends Scheduler/feature/base/TimeSpanMenuBase
 * @classtype scheduleMenu
 * @feature
 */
class ScheduleMenu extends TimeSpanMenuBase {
  //region Config
  static get $name() {
    return 'ScheduleMenu';
  }
  static get defaultConfig() {
    return {
      type: 'schedule',
      /**
       * This is a preconfigured set of items used to create the default context menu.
       *
       * The `items` provided by this feature are listed below. These are the predefined property names which you may
       * configure:
       *
       * - `addEvent` Add an event for at the resource and time indicated by the `contextmenu` event.
       *
       * To remove existing items, set corresponding keys `null`:
       *
       * ```javascript
       * const scheduler = new Scheduler({
       *     features : {
       *         scheduleMenu : {
       *             items : {
       *                 addEvent : null
       *             }
       *         }
       *     }
       * });
       * ```
       *
       * @config {Object<String,MenuItemConfig|Boolean|null>} items
       */
      items: null,
      /**
       * A function called before displaying the menu that allows manipulations of its items.
       * Returning `false` from this function prevents the menu being shown.
       *
       * ```javascript
       * features         : {
       *    scheduleMenu : {
       *         processItems({ items, date, resourceRecord }) {
       *            // Add or hide existing items here as needed
       *            items.myAction = {
       *                text   : 'Cool action',
       *                icon   : 'b-fa b-fa-cat',
       *                onItem : () => console.log(`Clicked on ${resourceRecord.name} at ${date}`),
       *                weight : 1000 // Move to end
       *            };
       *
       *            if (!resourceRecord.allowAdd) {
       *                items.addEvent.hidden = true;
       *            }
       *        }
       *    }
       * },
       * ```
       * @param {Object} context An object with information about the menu being shown
       * @param {Scheduler.model.ResourceModel} context.resourceRecord The record representing the current resource
       * @param {Date} context.date The clicked date
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
    config.chain.push('populateScheduleMenu');
    return config;
  }
  //endregion
  //region Events
  /**
   * This event fires on the owning Scheduler before the context menu is shown for an event. Allows manipulation of the items
   * to show in the same way as in `processItems`. Returning `false` from a listener prevents the menu from
   * being shown.
   * @event scheduleMenuBeforeShow
   * @on-owner
   * @preventable
   * @param {Scheduler.view.Scheduler} source
   * @param {Object<String,MenuItemConfig>} items Menu item configs
   * @param {Scheduler.model.EventModel} eventRecord Event record for which the menu was triggered
   * @param {Scheduler.model.ResourceModel} resourceRecord Resource record
   * @param {Scheduler.model.AssignmentModel} assignmentRecord Assignment record, if assignments are used
   * @param {HTMLElement} eventElement
   */
  /**
   * This event fires on the owning Scheduler when an item is selected in the context menu.
   * @event scheduleMenuItem
   * @on-owner
   * @param {Scheduler.view.Scheduler} source
   * @param {Core.widget.MenuItem} item
   * @param {Scheduler.model.EventModel} eventRecord
   * @param {Scheduler.model.ResourceModel} resourceRecord
   * @param {Scheduler.model.AssignmentModel} assignmentRecord Assignment record, if assignments are used
   * @param {HTMLElement} eventElement
   */
  /**
   * This event fires on the owning Scheduler after showing the context menu for an event
   * @event scheduleMenuShow
   * @on-owner
   * @param {Scheduler.view.Scheduler} source
   * @param {Core.widget.Menu} menu The menu
   * @param {Scheduler.model.EventModel} eventRecord Event record for which the menu was triggered
   * @param {Scheduler.model.ResourceModel} resourceRecord Resource record
   * @param {Scheduler.model.AssignmentModel} assignmentRecord Assignment record, if assignments are used
   * @param {HTMLElement} eventElement
   */
  //endregion
  shouldShowMenu(eventParams) {
    const {
        client
      } = this,
      {
        targetElement,
        resourceRecord
      } = eventParams,
      isTimeAxisColumn = client.timeAxisSubGridElement.contains(targetElement);
    return !targetElement.closest(client.eventSelector) && isTimeAxisColumn && !(resourceRecord && resourceRecord.isSpecialRow);
  }
  getDataFromEvent(event) {
    // Process event if it wasn't yet processed
    if (DomHelper.isDOMEvent(event)) {
      var _client$getCellDataFr, _client$getDateFromDo;
      const {
          client
        } = this,
        cellData = (_client$getCellDataFr = client.getCellDataFromEvent) === null || _client$getCellDataFr === void 0 ? void 0 : _client$getCellDataFr.call(client, event),
        date = (_client$getDateFromDo = client.getDateFromDomEvent) === null || _client$getDateFromDo === void 0 ? void 0 : _client$getDateFromDo.call(client, event, 'floor'),
        // For vertical mode the resource must be resolved from the event
        resourceRecord = client.resolveResourceRecord(event) || client.isVertical && client.resourceStore.last;
      return ObjectHelper.assign(super.getDataFromEvent(event), cellData, {
        date,
        resourceRecord
      });
    }
    return event;
  }
  populateScheduleMenu({
    items,
    resourceRecord,
    date
  }) {
    const {
      client
    } = this;
    // Menu can work for ResourceHistogram which doesn't have event store
    if (!client.readOnly && client.eventStore) {
      items.addEvent = {
        text: 'L{SchedulerBase.Add event}',
        icon: 'b-icon b-icon-add',
        disabled: !resourceRecord || resourceRecord.readOnly || !resourceRecord.isWorkingTime(date),
        weight: 100,
        onItem() {
          client.createEvent(date, resourceRecord, client.getRowFor(resourceRecord));
        }
      };
    }
  }
}
ScheduleMenu.featureClass = '';
ScheduleMenu._$name = 'ScheduleMenu';
GridFeatureManager.registerFeature(ScheduleMenu, true, 'Scheduler');

/**
 * @module Scheduler/view/mixin/RecurringEvents
 */
/**
 * A mixin that adds recurring events functionality to the Scheduler.
 *
 * The main purpose of the code in here is displaying a {@link Scheduler.view.recurrence.RecurrenceConfirmationPopup special confirmation}
 * on user mouse dragging/resizing/deleting recurring events and their occurrences.
 *
 * @mixin
 */
var RecurringEvents = (Target => class RecurringEvents extends (Target || Base) {
  static $name = 'RecurringEvents';
  static configurable = {
    /**
     * Enables showing occurrences of recurring events across the scheduler's time axis.
     *
     * Enables extra recurrence UI fields in the system-provided event editor (not in Scheduler Pro's task editor).
     * @config {Boolean}
     * @default
     * @category Scheduled events
     */
    enableRecurringEvents: false,
    recurrenceConfirmationPopup: {
      $config: ['lazy'],
      value: {
        type: 'recurrenceconfirmation'
      }
    }
  };
  construct(config) {
    super.construct(config);
    this.ion({
      beforeEventDropFinalize: 'onRecurrableBeforeEventDropFinalize',
      beforeEventResizeFinalize: 'onRecurrableBeforeEventResizeFinalize',
      beforeAssignmentDelete: 'onRecurrableAssignmentBeforeDelete'
    });
  }
  changeRecurrenceConfirmationPopup(recurrenceConfirmationPopup, oldRecurrenceConfirmationPopup) {
    // Widget.reconfigure reither reconfigures an existing instance, or creates a new one, or,
    // if the configuration is null, destroys the existing instance.
    const result = this.constructor.reconfigure(oldRecurrenceConfirmationPopup, recurrenceConfirmationPopup, 'recurrenceconfirmation');
    result.owner = this;
    return result;
  }
  findRecurringEventToConfirmDelete(eventRecords) {
    // show confirmation if we deal with at least one recurring event (or its occurrence)
    // and if the record is not being edited by event editor (since event editor has its own confirmation)
    return eventRecords.find(eventRecord => eventRecord.supportsRecurring && (eventRecord.isRecurring || eventRecord.isOccurrence));
  }
  onRecurrableAssignmentBeforeDelete({
    assignmentRecords,
    context
  }) {
    const eventRecords = assignmentRecords.map(as => as.event),
      eventRecord = this.findRecurringEventToConfirmDelete(eventRecords);
    if (this.enableRecurringEvents && eventRecord) {
      this.recurrenceConfirmationPopup.confirm({
        actionType: 'delete',
        eventRecord,
        changerFn() {
          context.finalize(true);
        },
        cancelFn() {
          context.finalize(false);
        }
      });
      return false;
    }
  }
  onRecurrableBeforeEventDropFinalize({
    context
  }) {
    if (this.enableRecurringEvents) {
      const {
          eventRecords
        } = context,
        recurringEvents = eventRecords.filter(eventRecord => eventRecord.supportsRecurring && (eventRecord.isRecurring || eventRecord.isOccurrence));
      if (recurringEvents.length) {
        context.async = true;
        this.recurrenceConfirmationPopup.confirm({
          actionType: 'update',
          eventRecord: recurringEvents[0],
          changerFn() {
            context.finalize(true);
          },
          cancelFn() {
            context.finalize(false);
          }
        });
      }
    }
  }
  onRecurrableBeforeEventResizeFinalize({
    context
  }) {
    if (this.enableRecurringEvents) {
      const {
          eventRecord
        } = context,
        isRecurring = eventRecord.supportsRecurring && (eventRecord.isRecurring || eventRecord.isOccurrence);
      if (isRecurring) {
        context.async = true;
        this.recurrenceConfirmationPopup.confirm({
          actionType: 'update',
          eventRecord,
          changerFn() {
            context.finalize(true);
          },
          cancelFn() {
            context.finalize(false);
          }
        });
      }
    }
  }
  // Make sure occurrence cache is up-to-date when reassigning events
  onAssignmentChange({
    action,
    records: assignments
  }) {
    if (action !== 'dataset' && Array.isArray(assignments)) {
      for (const assignment of assignments) {
        var _assignment$event;
        if ((_assignment$event = assignment.event) !== null && _assignment$event !== void 0 && _assignment$event.isRecurring && !assignment.event.isBatchUpdating) {
          assignment.event.removeOccurrences();
        }
      }
    }
  }
  /**
   * Returns occurrences of the provided recurring event across the date range of this Scheduler.
   * @param  {Scheduler.model.TimeSpan} recurringEvent Recurring event for which occurrences should be retrieved.
   * @returns {Scheduler.model.TimeSpan[]} Array of the provided timespans occurrences.
   *
   * __Empty if the passed event is not recurring, or has no occurrences in the date range.__
   *
   * __If the date range encompasses the start point, the recurring event itself will be the first entry.__
   * @category Data
   */
  getOccurrencesFor(recurringEvent) {
    return this.eventStore.getOccurrencesForTimeSpan(recurringEvent, this.timeAxis.startDate, this.timeAxis.endDate);
  }
  /**
   * Internal utility function to remove events. Used when pressing [DELETE] or [BACKSPACE] or when clicking the
   * delete button in the event editor. Triggers a preventable `beforeEventDelete` or `beforeAssignmentDelete` event.
   * @param {Scheduler.model.EventModel[]|Scheduler.model.AssignmentModel[]} eventRecords Records to remove
   * @param {Function} [callback] Optional callback executed after triggering the event but before deletion
   * @returns {Boolean} Returns `false` if the operation was prevented, otherwise `true`
   * @internal
   * @fires beforeEventDelete
   * @fires beforeAssignmentDelete
   */
  async removeEvents(eventRecords, callback = null, popupOwner = this) {
    const me = this;
    if (!me.readOnly && eventRecords.length) {
      const context = {
        finalize(removeRecord = true) {
          if (callback) {
            callback(removeRecord);
          }
          if (removeRecord !== false) {
            if (eventRecords.some(record => {
              var _record$event;
              return record.isOccurrence || ((_record$event = record.event) === null || _record$event === void 0 ? void 0 : _record$event.isOccurrence);
            })) {
              eventRecords.forEach(record => record.isOccurrenceAssignment ? record.event.remove() : record.remove());
            } else {
              const store = eventRecords[0].isAssignment ? me.assignmentStore : me.eventStore;
              store.remove(eventRecords);
            }
          }
        }
      };
      let shouldFinalize;
      if (eventRecords[0].isAssignment) {
        /**
         * Fires before an assignment is removed. Can be triggered by user pressing [DELETE] or [BACKSPACE] or
         * by the event editor. Can for example be used to display a custom dialog to confirm deletion, in which
         * case records should be "manually" removed after confirmation:
         *
         * ```javascript
         * scheduler.on({
         *    beforeAssignmentDelete({ assignmentRecords, context }) {
         *        // Show custom confirmation dialog (pseudo code)
         *        confirm.show({
         *            listeners : {
         *                onOk() {
         *                    // Remove the assignments on confirmation
         *                    context.finalize(true);
         *                },
         *                onCancel() {
         *                    // do not remove the assignments if "Cancel" clicked
         *                    context.finalize(false);
         *                }
         *            }
         *        });
         *
         *        // Prevent default behaviour
         *        return false;
         *    }
         * });
         * ```
         *
         * @event beforeAssignmentDelete
         * @param {Scheduler.view.Scheduler} source  The Scheduler instance
         * @param {Scheduler.model.EventModel[]} eventRecords  The records about to be deleted
         * @param {Object} context  Additional removal context:
         * @param {Function} context.finalize  Function to call to finalize the removal.
         *      Used to asynchronously decide to remove the records or not. Provide `false` to the function to
         *      prevent the removal.
         * @param {Boolean} [context.finalize.removeRecords = true]   Provide `false` to the function to prevent
         *      the removal.
         * @preventable
         */
        shouldFinalize = me.trigger('beforeAssignmentDelete', {
          assignmentRecords: eventRecords,
          context
        });
      } else {
        /**
         * Fires before an event is removed. Can be triggered by user pressing [DELETE] or [BACKSPACE] or by the
         * event editor. Can for example be used to display a custom dialog to confirm deletion, in which case
         * records should be "manually" removed after confirmation:
         *
         * ```javascript
         * scheduler.on({
         *    beforeEventDelete({ eventRecords, context }) {
         *        // Show custom confirmation dialog (pseudo code)
         *        confirm.show({
         *            listeners : {
         *                onOk() {
         *                    // Remove the events on confirmation
         *                    context.finalize(true);
         *                },
         *                onCancel() {
         *                    // do not remove the events if "Cancel" clicked
         *                    context.finalize(false);
         *                }
         *            }
         *        });
         *
         *        // Prevent default behaviour
         *        return false;
         *    }
         * });
         * ```
         *
         * @event beforeEventDelete
         * @param {Scheduler.view.Scheduler} source  The Scheduler instance
         * @param {Scheduler.model.EventModel[]} eventRecords  The records about to be deleted
         * @param {Object} context  Additional removal context:
         * @param {Function} context.finalize  Function to call to finalize the removal.
         *      Used to asynchronously decide to remove the records or not. Provide `false` to the function to
         *      prevent the removal.
         * @param {Boolean} [context.finalize.removeRecords = true]  Provide `false` to the function to prevent
         *      the removal.
         * @preventable
         */
        shouldFinalize = me.trigger('beforeEventDelete', {
          eventRecords,
          context
        });
      }
      if (shouldFinalize !== false) {
        const recurringEventRecord = eventRecords.find(eventRecord => eventRecord.isRecurring || eventRecord.isOccurrence);
        if (recurringEventRecord) {
          me.recurrenceConfirmationPopup.owner = popupOwner;
          me.recurrenceConfirmationPopup.confirm({
            actionType: 'delete',
            eventRecord: recurringEventRecord,
            changerFn() {
              context.finalize(true);
            },
            cancelFn() {
              context.finalize(false);
            }
          });
        } else {
          context.finalize(true);
        }
        return true;
      }
    }
    return false;
  }
  // This does not need a className on Widgets.
  // Each *Class* which doesn't need 'b-' + constructor.name.toLowerCase() automatically adding
  // to the Widget it's mixed in to should implement thus.
  get widgetClass() {}
});

/**
 * @module Scheduler/view/mixin/CurrentConfig
 */
const stores = ['eventStore', 'taskStore', 'assignmentStore', 'resourceStore', 'dependencyStore', 'timeRangeStore', 'resourceTimeRangeStore'],
  inlineProperties = ['events', 'tasks', 'resources', 'assignments', 'dependencies', 'timeRanges', 'resourceTimeRanges'];
/**
 * Mixin that makes sure inline data & crud manager data are removed from current config for products using a project.
 * The data is instead inlined in the project (by ProjectModel.js)
 *
 * @mixin
 * @private
 */
var CurrentConfig = (Target => class CurrentConfig extends Target {
  static get $name() {
    return 'CurrentConfig';
  }
  preProcessCurrentConfigs(configs) {
    // Remove inline data on the component
    for (const prop of inlineProperties) {
      delete configs[prop];
    }
    super.preProcessCurrentConfigs(configs);
  }
  // This function is not meant to be called by any code other than Base#getCurrentConfig().
  getCurrentConfig(options) {
    const project = this.project.getCurrentConfig(options),
      result = super.getCurrentConfig(options);
    // Force project with inline data
    if (project) {
      result.project = project;
      const {
        crudManager
      } = result;
      // Transfer crud store configs to project (mainly fields)
      if (crudManager) {
        for (const store of stores) {
          if (crudManager[store]) {
            project[store] = crudManager[store];
          }
        }
      }
      if (Object.keys(project).length === 0) {
        delete result.project;
      }
    }
    // Store (resource store) data is included in project
    delete result.data;
    // Remove CrudManager, since data will be placed inline
    delete result.crudManager;
    return result;
  }
  get widgetClass() {}
});

/**
 * @module Scheduler/view/mixin/EventNavigation
 */
const preventDefault = e => e.preventDefault(),
  isArrowKey = {
    ArrowRight: 1,
    ArrowLeft: 1,
    ArrowUp: 1,
    ArrowDown: 1
  },
  animate100 = {
    animate: 100
  },
  emptyObject = Object.freeze({});
/**
 * Mixin that tracks event or assignment selection by clicking on one or more events in the scheduler.
 * @mixin
 */
var SchedulerEventNavigation = (Target => class EventNavigation extends Delayable(Target || Base) {
  static get $name() {
    return 'EventNavigation';
  }
  //region Default config
  static get configurable() {
    return {
      /**
       * A config object to use when creating the {@link Core.helper.util.Navigator}
       * to use to perform keyboard navigation in the timeline.
       * @config {NavigatorConfig}
       * @default
       * @category Misc
       * @internal
       */
      navigator: {
        allowCtrlKey: true,
        scrollSilently: true,
        keys: {
          Space: 'onEventSpaceKey',
          Enter: 'onEventEnterKey',
          Delete: 'onDeleteKey',
          Backspace: 'onDeleteKey',
          ArrowUp: 'onArrowUpKey',
          ArrowDown: 'onArrowDownKey',
          Escape: 'onEscapeKey',
          // These are processed by GridNavigation's handlers
          Tab: 'onTab',
          'SHIFT+Tab': 'onShiftTab'
        }
      },
      isNavigationKey: {
        ArrowDown: 1,
        ArrowUp: 1,
        ArrowLeft: 1,
        ArrowRight: 1
      }
    };
  }
  static get defaultConfig() {
    return {
      /**
       * A CSS class name to add to focused events.
       * @config {String}
       * @default
       * @category CSS
       * @private
       */
      focusCls: 'b-active',
      /**
       * Allow using [Delete] and [Backspace] to remove events/assignments
       * @config {Boolean}
       * @default
       * @category Misc
       */
      enableDeleteKey: true,
      // Number in milliseconds to buffer handlers execution. See `Delayable.throttle` function docs.
      onDeleteKeyBuffer: 500,
      navigatePreviousBuffer: 200,
      navigateNextBuffer: 200,
      testConfig: {
        onDeleteKeyBuffer: 1
      }
    };
  }
  //endregion
  //region Events
  /**
   * Fired when a user gesture causes the active item to change.
   * @event navigate
   * @param {Event} event The browser event which instigated navigation. May be a click or key or focus event.
   * @param {HTMLElement|null} item The newly active item, or `null` if focus moved out.
   * @param {HTMLElement|null} oldItem The previously active item, or `null` if focus is moving in.
   */
  //endregion
  construct(config) {
    const me = this;
    me.isInTimeAxis = me.isInTimeAxis.bind(me);
    me.onDeleteKey = me.throttle(me.onDeleteKey, me.onDeleteKeyBuffer, me);
    super.construct(config);
  }
  changeNavigator(navigator) {
    const me = this;
    me.getConfig('subGridConfigs');
    return new Navigator(me.constructor.mergeConfigs({
      ownerCmp: me,
      target: me.timeAxisSubGridElement,
      processEvent: me.processEvent,
      itemSelector: `.${me.eventCls}-wrap`,
      focusCls: me.focusCls,
      navigatePrevious: me.throttle(me.navigatePrevious, {
        delay: me.navigatePreviousBuffer,
        throttled: preventDefault
      }),
      navigateNext: me.throttle(me.navigateNext, {
        delay: me.navigateNextBuffer,
        throttled: preventDefault
      })
    }, navigator));
  }
  doDestroy() {
    this.navigator.destroy();
    super.doDestroy();
  }
  isInTimeAxis(record) {
    // If event is hidden by workingTime configs, horizontal mapper would raise a flag on instance meta
    // We still need to check if time span is included in axis
    return !record.instanceMeta(this).excluded && this.timeAxis.isTimeSpanInAxis(record);
  }
  onElementKeyDown(keyEvent) {
    var _me$focusedCell, _me$focusedCell2;
    const me = this,
      {
        navigator
      } = me;
    // If we're focused in the time axis, and *not* on an event, then ENTER means
    // jump down into the first visible assignment in the cell.
    if (((_me$focusedCell = me.focusedCell) === null || _me$focusedCell === void 0 ? void 0 : _me$focusedCell.rowIndex) !== -1 && ((_me$focusedCell2 = me.focusedCell) === null || _me$focusedCell2 === void 0 ? void 0 : _me$focusedCell2.column) === me.timeAxisColumn && !keyEvent.target.closest(navigator.itemSelector) && keyEvent.key === 'Enter') {
      const firstAssignment = me.getFirstVisibleAssignment();
      if (firstAssignment) {
        me.navigateTo(firstAssignment, {
          uiEvent: keyEvent
        });
        return false;
      }
    } else {
      var _super$onElementKeyDo;
      (_super$onElementKeyDo = super.onElementKeyDown) === null || _super$onElementKeyDo === void 0 ? void 0 : _super$onElementKeyDo.call(this, keyEvent);
    }
  }
  getFirstVisibleAssignment(location = this.focusedCell) {
    const me = this,
      {
        currentOrientation,
        rowManager,
        eventStore
      } = me;
    if (me.isHorizontal) {
      var _renderedEvents;
      let renderedEvents = currentOrientation.rowMap.get(rowManager.getRow(location.rowIndex));
      if ((_renderedEvents = renderedEvents) !== null && _renderedEvents !== void 0 && _renderedEvents.length) {
        var _renderedEvents$;
        return (_renderedEvents$ = renderedEvents[0]) === null || _renderedEvents$ === void 0 ? void 0 : _renderedEvents$.elementData.assignmentRecord;
      } else {
        var _currentOrientation$r, _renderedEvents2;
        renderedEvents = (_currentOrientation$r = currentOrientation.resourceMap.get(location.id)) === null || _currentOrientation$r === void 0 ? void 0 : _currentOrientation$r.eventsData;
        if ((_renderedEvents2 = renderedEvents) !== null && _renderedEvents2 !== void 0 && _renderedEvents2.length) {
          var _renderedEvents$filte;
          // When events are gathered from resource, we need to check they're available
          return (_renderedEvents$filte = renderedEvents.filter(e => eventStore.isAvailable(e.eventRecord))[0]) === null || _renderedEvents$filte === void 0 ? void 0 : _renderedEvents$filte.assignmentRecord;
        }
      }
    } else {
      const firstResource = [...currentOrientation.resourceMap.values()][0],
        renderedEvents = firstResource && Object.values(firstResource);
      if (renderedEvents !== null && renderedEvents !== void 0 && renderedEvents.length) {
        return renderedEvents.filter(e => eventStore.isAvailable(e.renderData.eventRecord))[0].renderData.assignmentRecord;
      }
    }
  }
  onGridBodyFocusIn(focusEvent) {
    const isGridCellFocus = focusEvent.target.closest(this.focusableSelector);
    // Event navigation only has a say when navigation is inside the TimeAxisSubGrid
    if (this.timeAxisSubGridElement.contains(focusEvent.target)) {
      const me = this,
        {
          navigationEvent
        } = me,
        {
          target
        } = focusEvent,
        eventFocus = target.closest(me.navigator.itemSelector),
        destinationCell = eventFocus ? me.normalizeCellContext({
          rowIndex: me.isVertical ? 0 : me.resourceStore.indexOf(me.resolveResourceRecord(target)),
          column: me.timeAxisColumn,
          target
        }) : new Location(target);
      // Don't take over what the event navigator does if it's doing event navigation.
      // Just silently cache our actionable location.
      if (eventFocus) {
        var _me$onCellNavigate;
        const {
          _focusedCell
        } = me;
        me._focusedCell = destinationCell;
        (_me$onCellNavigate = me.onCellNavigate) === null || _me$onCellNavigate === void 0 ? void 0 : _me$onCellNavigate.call(me, me, _focusedCell, destinationCell, navigationEvent, true);
        return;
      }
      // Depending on how we got here, try to focus the first event in the cell *if we're in a cell*.
      if (isGridCellFocus && (!navigationEvent || isArrowKey[navigationEvent.key])) {
        const firstAssignment = me.getFirstVisibleAssignment(destinationCell);
        if (firstAssignment) {
          me.navigateTo(firstAssignment, {
            // Only change scroll if focus came from key press
            scrollIntoView: Boolean(navigationEvent && navigationEvent.type !== 'mousedown'),
            uiEvent: navigationEvent || focusEvent
          });
          return;
        }
      }
    }
    // Grid-level focus movement, let superclass handle it.
    if (isGridCellFocus) {
      super.onGridBodyFocusIn(focusEvent);
    }
  }
  /*
   * Override of GridNavigation#focusCell method to handle the TimeAxisColumn.
   * Not needed until we implement full keyboard accessibility.
   */
  accessibleFocusCell(cellSelector, options) {
    const me = this;
    cellSelector = me.normalizeCellContext(cellSelector);
    if (cellSelector.columnId === me.timeAxisColumn.id) ; else {
      return super.focusCell(cellSelector, options);
    }
  }
  // Interface method to extract the navigated to record from a populated 'navigate' event.
  // Gantt, Scheduler and Calendar handle event differently, adding different properties to it.
  // This method is meant to be overridden to return correct target from event
  normalizeTarget(event) {
    return event.assignmentRecord;
  }
  getPrevious(assignmentRecord, isDelete) {
    const me = this,
      {
        resourceStore
      } = me,
      {
        eventSorter
      } = me.currentOrientation,
      // start/end dates are required to limit time span to look at in case recurrence feature is enabled
      {
        startDate,
        endDate
      } = me.timeAxis,
      eventRecord = assignmentRecord.event,
      resourceEvents = me.eventStore.getEvents({
        resourceRecord: assignmentRecord.resource,
        startDate,
        endDate
      }).filter(this.isInTimeAxis).sort(eventSorter);
    let resourceRecord = assignmentRecord.resource,
      previousEvent = resourceEvents[resourceEvents.indexOf(eventRecord) - 1];
    // At first event for resource, traverse up the resource store.
    if (!previousEvent) {
      // If we are deleting an event, skip other instances of the event which we may encounter
      // due to multi-assignment.
      for (let rowIdx = resourceStore.indexOf(resourceRecord) - 1; (!previousEvent || isDelete && previousEvent === eventRecord) && rowIdx >= 0; rowIdx--) {
        resourceRecord = resourceStore.getAt(rowIdx);
        const events = me.eventStore.getEvents({
          resourceRecord,
          startDate,
          endDate
        }).filter(me.isInTimeAxis).sort(eventSorter);
        previousEvent = events.length && events[events.length - 1];
      }
    }
    return me.assignmentStore.getAssignmentForEventAndResource(previousEvent, resourceRecord);
  }
  navigatePrevious(keyEvent) {
    const me = this,
      previousAssignment = me.getPrevious(me.normalizeTarget(keyEvent));
    keyEvent.preventDefault();
    if (previousAssignment) {
      if (!keyEvent.ctrlKey) {
        me.clearEventSelection();
      }
      return me.navigateTo(previousAssignment, {
        uiEvent: keyEvent
      });
    }
    // No previous event/task, fall back to Grid's handling of this gesture
    return me.doGridNavigation(keyEvent);
  }
  getNext(assignmentRecord, isDelete) {
    const me = this,
      {
        resourceStore
      } = me,
      {
        eventSorter
      } = me.currentOrientation,
      // start/end dates are required to limit time span to look at in case recurrence feature is enabled
      {
        startDate,
        endDate
      } = me.timeAxis,
      eventRecord = assignmentRecord.event,
      resourceEvents = me.eventStore.getEvents({
        resourceRecord: assignmentRecord.resource,
        // start/end are required to limit time
        startDate,
        endDate
      }).filter(this.isInTimeAxis).sort(eventSorter);
    let resourceRecord = assignmentRecord.resource,
      nextEvent = resourceEvents[resourceEvents.indexOf(eventRecord) + 1];
    // At last event for resource, traverse down the resource store
    if (!nextEvent) {
      // If we are deleting an event, skip other instances of the event which we may encounter
      // due to multi-assignment.
      for (let rowIdx = resourceStore.indexOf(resourceRecord) + 1; (!nextEvent || isDelete && nextEvent === eventRecord) && rowIdx < resourceStore.count; rowIdx++) {
        resourceRecord = resourceStore.getAt(rowIdx);
        const events = me.eventStore.getEvents({
          resourceRecord,
          startDate,
          endDate
        }).filter(me.isInTimeAxis).sort(eventSorter);
        nextEvent = events[0];
      }
    }
    return me.assignmentStore.getAssignmentForEventAndResource(nextEvent, resourceRecord);
  }
  navigateNext(keyEvent) {
    const me = this,
      nextAssignment = me.getNext(me.normalizeTarget(keyEvent));
    keyEvent.preventDefault();
    if (nextAssignment) {
      if (!keyEvent.ctrlKey) {
        me.clearEventSelection();
      }
      return me.navigateTo(nextAssignment, {
        uiEvent: keyEvent
      });
    }
    // No next event/task, fall back to Grid's handling of this gesture
    return me.doGridNavigation(keyEvent);
  }
  doGridNavigation(keyEvent) {
    if (!keyEvent.handled && keyEvent.key.indexOf('Arrow') === 0) {
      this[`navigate${keyEvent.key.substring(5)}ByKey`](keyEvent);
    }
  }
  async navigateTo(targetAssignment, {
    scrollIntoView = true,
    uiEvent = {}
  } = emptyObject) {
    const me = this,
      {
        navigator
      } = me,
      {
        skipScrollIntoView
      } = navigator;
    if (targetAssignment) {
      if (scrollIntoView) {
        // No key processing during scroll
        navigator.disabled = true;
        await me.scrollAssignmentIntoView(targetAssignment, animate100);
        navigator.disabled = false;
      } else {
        navigator.skipScrollIntoView = true;
      }
      // Panel can be destroyed before promise is resolved
      // Perform a sanity check to make sure element is still in the DOM (syncIdMap actually).
      if (!me.isDestroyed && this.getElementFromAssignmentRecord(targetAssignment)) {
        me.activeAssignment = targetAssignment;
        navigator.skipScrollIntoView = skipScrollIntoView;
        navigator.trigger('navigate', {
          event: uiEvent,
          item: me.getElementFromAssignmentRecord(targetAssignment).closest(navigator.itemSelector)
        });
      }
    }
  }
  set activeAssignment(assignmentRecord) {
    const assignmentEl = this.getElementFromAssignmentRecord(assignmentRecord, true);
    if (assignmentEl) {
      this.navigator.activeItem = assignmentEl;
    }
  }
  get activeAssignment() {
    const {
      activeItem
    } = this.navigator;
    if (activeItem) {
      return this.resolveAssignmentRecord(activeItem);
    }
  }
  get previousActiveEvent() {
    const {
      previousActiveItem
    } = this.navigator;
    if (previousActiveItem) {
      return this.resolveEventRecord(previousActiveItem);
    }
  }
  processEvent(keyEvent) {
    const me = this,
      eventElement = keyEvent.target.closest(me.eventSelector);
    if (!me.navigator.disabled && eventElement) {
      keyEvent.assignmentRecord = me.resolveAssignmentRecord(eventElement);
      keyEvent.eventRecord = me.resolveEventRecord(eventElement);
      keyEvent.resourceRecord = me.resolveResourceRecord(eventElement);
    }
    return keyEvent;
  }
  onDeleteKey(keyEvent) {
    const me = this;
    if (!me.readOnly && me.enableDeleteKey) {
      const records = me.eventStore.usesSingleAssignment ? me.selectedEvents : me.selectedAssignments;
      me.removeEvents(records.filter(r => !r.readOnly));
    }
  }
  onArrowUpKey(keyEvent) {
    this.focusCell({
      rowIndex: this.focusedCell.rowIndex - 1,
      column: this.timeAxisColumn
    });
    keyEvent.handled = true;
  }
  onArrowDownKey(keyEvent) {
    if (this.focusedCell.rowIndex < this.resourceStore.count - 1) {
      this.focusCell({
        rowIndex: this.focusedCell.rowIndex + 1,
        column: this.timeAxisColumn
      });
      keyEvent.handled = true;
    }
  }
  onEscapeKey(keyEvent) {
    if (!keyEvent.target.closest('.b-dragging')) {
      this.focusCell({
        rowIndex: this.focusedCell.rowIndex,
        column: this.timeAxisColumn
      });
      keyEvent.handled = true;
    }
  }
  onEventSpaceKey(keyEvent) {
    // Empty, to be chained by features
  }
  onEventEnterKey(keyEvent) {
    // Empty, to be chained by features
  }
  get isActionableLocation() {
    // Override from grid if the Navigator's location is an event (or task if we're in Gantt)
    // Being focused on a task/event means that it's *not* actionable. It's not valid to report
    // that we're "inside" the cell in a TimeLine, so ESC must not attempt to focus the cell.
    if (!this.navigator.activeItem) {
      return super.isActionableLocation;
    }
  }
  // This does not need a className on Widgets.
  // Each *Class* which doesn't need 'b-' + constructor.name.toLowerCase() automatically adding
  // to the Widget it's mixed in to should implement thus.
  get widgetClass() {}
});

export { AttachToProjectMixin, ClockTemplate, CurrentConfig, EventMenu, ProjectConsumer, RecurrenceConfirmationPopup, RecurrenceDaysButtonGroup, RecurrenceDaysCombo, RecurrenceEditorPanel, RecurrenceFrequencyCombo, RecurrenceMonthDaysButtonGroup, RecurrenceMonthsButtonGroup, RecurrencePositionsCombo, RecurrenceStopConditionCombo, RecurringEvents, ScheduleMenu, SchedulerEventNavigation, TaskEditStm, TimeSpanMenuBase };
//# sourceMappingURL=EventNavigation.js.map
