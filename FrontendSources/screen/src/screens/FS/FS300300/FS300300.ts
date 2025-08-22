import { TaskQueue } from 'aurelia-framework';
/* eslint-disable @typescript-eslint/member-ordering */
/* eslint-disable brace-style */
import 'bingmaps';
import { autoinject, bindable, ElementEvents, observable, BindingEngine, TemplatingEngine, Aurelia } from 'aurelia-framework';
import { SchedulerPro, EventModel, ResourceModel, AssignmentModel, DragHelper, Toast,
	StringHelper, Scheduler, ResourceTimeRangeModel, Tooltip, Popup, SchedulerEventModel, SchedulerAssignmentModel,
	SubGrid, Model, SchedulerResourceModel, SchedulerProjectModel, ProjectModel, ViewPresetConfig, CrudManager, ResourceStore, Grid
} from '@bryntum/schedulerpro/schedulerpro.module';
import {
	PXScreen, ScreenUpdateParams, createCollection, graphInfo, /*commitChanges,*/ PXView,
	PXFieldState, localizable, QpGridCustomElement, QpFieldsetCustomElement, IGridRow, GridPagerMode, QpTabbarCustomElement, GridFilter,
	showInformer,
	QpFieldCustomElement,
	createSingle,
	ICommandUpdateResult,
	QpHyperIconCustomElement,
	GridFilterBarVisibility,
	GridFastFilterVisibility
} from 'client-controls';
import { QpGridEventArgs } from 'client-controls/controls/compound/grid/qp-grid';
import { NewMenuButton, QpToolBarCustomElement } from 'client-controls/controls/compound/tool-bar/qp-tool-bar';
import { formatDate, getScreenID } from 'client-controls/utils';
import { dateFormatInfo } from 'client-controls/utils/platform-dependable-utils';
import { ISplitterConfig, QpSplitterCustomElement } from 'client-controls/controls/container/splitter/qp-splitter';
import { IPreferencesPair, IPreferencesTarget, PreferenceBase, PreferencesService } from 'client-controls/services/preferences';
import { ExternalEventDragHelper } from './drag-helpers/external-event-drag-helper';
import { ResourceDragHelper } from './drag-helpers/resource-drag-helper';
import { BingMapCustomElement } from './bing-map';
import { MapController } from './map-controller';
import { SODataHandler } from './data-handlers/so-data-handler';
import { AppointmentsDataHandler } from './data-handlers/appointments-data-handler';
import { SearchAppointmentsDataHandler } from './data-handlers/search-appointments-data-handler';
import { DataFieldDescriptor, FieldsetPreferences } from 'client-controls/controls/container/fieldset/types';
import { ISchedulerApiClient, PeriodKind, IDataQuery  } from './scheduler-types';
import { SchedulerApiClient } from './scheduler-api-client';
import { AppointmentEmployeeModel, AppointmentFilterModel,
	EmployeeModel, LastUpdatedAppointmentFilterModel,
	SelectedAppointmentModel, SelectedSOModel, ServiceOrderModel, UpdatedAppointmentModel, DatesFilterModel,
	SOFilterModel, InitDataModel, DraggableEntity, SearchAppointmentModel, RoomModel, MainAppointmentFilterModel, SetupModel
} from './view-models';
import { TimeConverter } from './data-handlers/time-converter';
import { TestHelperHTMLElement, testHelperScrollToXPercent, testHelperSimulateDragAndDrop } from './test-helpers';



@localizable
export class Captions {
	static ServiceOrdersDetails = "Details";
	static Employees = "Employees";
	static Staff = "Staff";
	static DefaultStaff = "Default Staff";
	static Unassigned = "Unassigned";
}

@localizable
class Labels {
	// static SwitchToVertical = "Switch to Vertical";
	static NonBusinessHours = "Nonworking Hours";
	static Appointment = "Appointment";
	static ServiceOrder = "Service Order";
	static AddAppointment = "Add Appointment";
	static NewAppointment = "New Appointment";
	static Schedule = "Schedule";
	static PutOnHold = "Put on Hold"
	static DeleteAppointment = "Delete";
	static CloneAppointment = "Clone...";
	static CreateAppointment = "Create Appointment..."
	static Unassign = "Unassign";
	static ViewAppointment = "View..."
	static EditAppointment = "Edit..."
	static Confirm = "Confirm";
	static Unconfirm = "Unconfirm";
	static ValidateByDispatcher = "Validate by Dispatcher";
	static ClearValidation = "Clear Validation";
	static PrevPeriod = "Previous";
	static NextPeriod = "Next";
	static Reset = "Discard Changes";
}

@localizable
class Messages {
	static ErrorLoadingData = "An error occurred while loading data from the server.";
	static NoWorkingCalendarSet = "No working calendar has been defined for the displayed employees.";
}

@localizable
export class Formats {
	static TimeAxisDay = "ddd, MMMM D, YYYY";
	static TimeAxisHour = "H:mm";
	static DayMonth = "MMMM d";
	static OnlyMonth = "MMMM";
	static DateRangeSameMonthP1 = "MMMM d";
	static DateRangeSameMonthP2 = "d ";
	static DateRangeDiffMonthP1 = "MMMM d";
	static DateRangeDiffMonthP2 = "MMMM d";
	static ShortDate = dateFormatInfo().shortDate;
}

@graphInfo({ graphType: 'PX.Objects.FS.SchedulerMaint', primaryView: 'MainAppointmentFilter', hideFilesIndicator: true, hideNotesIndicator: true })
@autoinject
export class FS300300 extends PXScreen implements IPreferencesTarget {

	readonly horizontalSnapDistance = 15; 	// eslint-disable-line @typescript-eslint/no-magic-numbers
	readonly verticalSnapDistance = 30; 	// eslint-disable-line @typescript-eslint/no-magic-numbers
	readonly prefid = "main_prefs";
	readonly serviceOrdersBrickLeftID = "serviceOrdersBrickLeftID";
	readonly serviceOrdersBrickRightID = "serviceOrdersBrickRightID";
	readonly searchAppointmentBrickLeftID = "searchAppointmentBrickLeftID";
	readonly searchAppointmentBrickRightID = "searchAppointmentBrickRightID";
	readonly appointmentsBrickLeftID = "appointmentsBrickLeftID";
	readonly appointmentsBrickRightID = "appointmentsBrickRightID";
	readonly allRecordsFilter = "00000000-0000-0000-0000-000000000000";

	private orientation: 'horizontal'|'vertical' = 'horizontal';
	private get isPeriodDay() { return this.DatesFilter?.periodKindValue === PeriodKind.Day; }
	private get isPeriodWeek() { return this.DatesFilter?.periodKindValue === PeriodKind.Week; }
	private get isPeriodMonth() { return this.DatesFilter?.periodKindValue === PeriodKind.Month; }

	private topBarConfig =  {
		// TODO: uncomment when/if we reintroduce orientation switching
		// rotate: { index: 1,
		// 	config: { ...NewMenuButton("rotate"),
		// 		commandName: "rotate",
		// 		images: { normal: "svg:main@changeOrientation" },
		// 		isSystem: false,
		// 		showInToolbar: true,
		// 		toggleMode: true,
		// 		target: this.element, // eslint-disable-line @typescript-eslint/no-invalid-this
		// 		text: Labels.SwitchToVertical,
		// 		cssClass: this.getCssClassForOrientation() // eslint-disable-line @typescript-eslint/no-invalid-this
		// 	}
		// },
		add: { index: 0,
			config: { ...NewMenuButton("add"),
				commandName: "add",
				images: { normal: "svg:main@plus" },
				toolTip: Labels.AddAppointment,
				isSystem: false, toggleMode: false, showInToolbar: true, target: this.element, pushed: false,
			}
		},
		hours: { index: 1,
			config: { ...NewMenuButton("hours"),
				commandName: "hours",
				images: { normal: "svg:main@aroundTheClock" },
				toolTip: Labels.NonBusinessHours,
				isSystem: false, toggleMode: false, showInToolbar: true, target: this.element, pushed: false,
			}
		},
	};

	private rightToolBarConfig = {
		id: "rightToolBarConfigID",
		items: {
			prevPeriod: {
				config: {
					commandName: "prevPeriod",
					images: { normal: "main@PagePrev" },
					toolTip: Labels.PrevPeriod,
				}
			},
			nextPeriod: {
				config: {
					commandName: "nextPeriod",
					images: { normal: "main@PageNext" },
					toolTip: Labels.NextPeriod,
				}
			},
			periodDay: {
				config: {
					text: "Day", commandName: "periodDay", cssClass: 'qp-sch-time-period qp-sch-period-day',
				}
			},
			periodWeek: {
				config: {
					text: "Week", commandName: "periodWeek", cssClass: 'qp-sch-time-period qp-sch-period-week',
				}
			},
			periodMonth: {
				config: {
					text: "Month", commandName: "periodMonth", cssClass: 'qp-sch-time-period qp-sch-period-month',
				}
			},
		}
	}

	private topBarAppointmentConfig =  {
		id: "topBarAppointmentConfigID",
		items: {
			// TODO: there's a bug in qp-tool-bar that hides the menu opener if it wasn't needed the previous time
			confirm: { config: { id: "confirm", text: "Confirm", showInToolbar: true, commandName: "confirm", connotation: "Success", target: this.element, hidden: false } },
			unconfirm: { config: { id: "unconfirm", text: "Unconfirm", showInToolbar: true, commandName: "unconfirm", target: this.element, hidden: false } },
			validate: { config: { id: "validate", text: "Validate by Dispatcher", showInToolbar: true, commandName: "validate", target: this.element, hidden: false } },
			invalidate: { config: { id: "invalidate", text: "Clear Validation", showInToolbar: true, commandName: "invalidate", target: this.element, hidden: false } },
			unassign: { config: { id: "unassign", text: "Unassign", showInToolbar: true, commandName: "unassign", target: this.element, hidden: false } },
			clone: { config: { id: "clone", text: "Clone", showInToolbar: true, commandName: "clone", target: this.element, hidden: false } },
			delete: { config: { ...NewMenuButton("delete"), id: "delete", text: "Delete", showInToolbar: true, commandName: "delete", target: this.element, hidden: false, visibleOnToolbar: false } },
		}
	}

	private topBarNewAppointmentConfig =  {
		id: "topBarNewAppointmentConfigID",
		items: {
			reset: {
				config: { ...NewMenuButton("reset"),
					commandName: "reset",
					images: { normal: "main@Cancel" },
					toolTip: Labels.Reset,
					isSystem: false, toggleMode: false, showInToolbar: true, target: this.element, pushed: false,
				}
			},
			create: {
				config: {
					text: Labels.CreateAppointment, commandName: "create", connotation: "Success", showInToolbar: true, target: this.element,
				}
			},
		}
	}

	private popupAppointmentEmployeesConfig = {
		suppressNoteFiles: true,
	};
	private popupNewAppointmentEmployeesConfig = {
		suppressNoteFiles: true,
	};
	private popupSOEmployeesConfig = {
		suppressNoteFiles: true,
	};

	Captions = Captions;
	Labels = Labels;

	ServiceOrders = createCollection(
		ServiceOrderModel,
		{
			pageSize: 50,
			adjustPageSize: false,
			pagerMode: GridPagerMode.InfiniteScroll,
			syncPosition: false,
			allowDelete: false,
			allowInsert: false,
			showFastFilter: GridFastFilterVisibility.ToolBar,
			fastFilterByAllFields: false,
			suppressNoteFiles: true,
		}
	);

	SchedulerGridFilters = Array<GridFilter>();

	SearchAppointments = createCollection(
		SearchAppointmentModel,
		{
			pageSize: 50,
			adjustPageSize: false,
			pagerMode: GridPagerMode.InfiniteScroll,
			syncPosition: false,
			allowDelete: false,
			allowInsert: false,
			showFastFilter: GridFastFilterVisibility.ToolBar,
			fastFilterByAllFields: false,
			suppressNoteFiles: true,
		}
	);

	AppointmentsAllStaff = createCollection(
		AppointmentEmployeeModel,
		{
			pageSize: 2000,
			adjustPageSize: false,
			syncPosition: false,
			allowDelete: false,
			allowInsert: true,
			showFastFilter: GridFastFilterVisibility.ToolBar,
			fastFilterByAllFields: false,
			suppressNoteFiles: true,
			topBarItems: this.topBarConfig,
		}
	);

	LastUpdatedAppointment = createCollection(
		AppointmentEmployeeModel,
		{
			pageSize: 100,
			adjustPageSize: false,
			syncPosition: false,
			allowDelete: false,
			allowInsert: false,
			suppressNoteFiles: true,
		}
	);

	AppointmentFilter = createSingle(AppointmentFilterModel);
	SOFilter = createSingle(SOFilterModel);
	LastUpdatedAppointmentFilter = createSingle(LastUpdatedAppointmentFilterModel);
	InitData = createSingle(InitDataModel);
	Setup = createSingle(SetupModel);

	SelectedAppointment = createSingle(SelectedAppointmentModel);
	SelectedSO = createSingle(SelectedSOModel);

	SelectedAppointmentEmployees = createCollection(
		EmployeeModel, {
			pageSize: 100,
			adjustPageSize: false,
			syncPosition: true,
			allowDelete: false,
			allowInsert: false,
			showFilterBar: GridFilterBarVisibility.False,
			suppressNoteFiles: true,
			pagerMode: GridPagerMode.InfiniteScroll,
		}
	);

	SelectedSOEmployees = createCollection(
		EmployeeModel, {
			pageSize: 100,
			adjustPageSize: false,
			syncPosition: true,
			allowDelete: false,
			allowInsert: false,
			showFilterBar: GridFilterBarVisibility.False,
			suppressNoteFiles: true,
			pagerMode: GridPagerMode.InfiniteScroll,
		}
	);

	EditedAppointmentEmployees = createCollection(
		EmployeeModel, {
			pageSize: 100,
			adjustPageSize: false,
			syncPosition: true,
			allowDelete: false,
			allowInsert: false,
			showFilterBar: GridFilterBarVisibility.False,
			pagerMode: GridPagerMode.InfiniteScroll,
			suppressNoteFiles: true,
		}
	);

	AllRooms = createCollection(
		RoomModel, {
			pageSize: 1,
			adjustPageSize: false,
			syncPosition: true,
			allowDelete: false,
			allowInsert: false,
			showFilterBar: GridFilterBarVisibility.False,
			pagerMode: GridPagerMode.InfiniteScroll,
			suppressNoteFiles: true,
		}
	);

	DatesFilter = createSingle(DatesFilterModel);

	MainAppointmentFilter = createSingle(MainAppointmentFilterModel);
	UpdatedAppointment = createSingle(UpdatedAppointmentModel);

	@bindable schedulerGrid!: QpGridCustomElement;
	@bindable lastUpdatedGrid!: QpGridCustomElement;
	@bindable serviceOrdersGrid!: QpGridCustomElement;
	@bindable searchAppointmentsGrid!: QpGridCustomElement;
	@bindable groupAppointmentFilter!: QpFieldsetCustomElement;
	@bindable groupLastUpdatedAppointmentFilter!: QpFieldsetCustomElement;
	@bindable serviceOrdersBrickLeft!: QpFieldsetCustomElement;
	@bindable serviceOrdersBrickRight!: QpFieldsetCustomElement;
	@bindable appointmentsBrickLeft!: QpFieldsetCustomElement;
	@bindable appointmentsBrickRight!: QpFieldsetCustomElement;
	@bindable searchAppointmentBrickLeft!: QpFieldsetCustomElement;
	@bindable searchAppointmentBrickRight!: QpFieldsetCustomElement;
	@bindable schedulerTabBar!: QpTabbarCustomElement;
	@bindable mainTabBar!: QpTabbarCustomElement;
	@bindable mainMap!: BingMapCustomElement;
	@bindable mapSplitter!: QpSplitterCustomElement;
	@bindable appointmentToolBar!: QpToolBarCustomElement;
	@bindable newAppointmentToolBar!: QpToolBarCustomElement;
	@bindable rightToolBar!: QpToolBarCustomElement;
	@bindable iconPriorityHigh!: QpHyperIconCustomElement;
	@bindable iconPriorityMedium!: QpHyperIconCustomElement;
	@bindable iconPriorityLow!: QpHyperIconCustomElement;
	@bindable iconSeverityHigh!: QpHyperIconCustomElement;
	@bindable iconSeverityMedium!: QpHyperIconCustomElement;
	@bindable iconSeverityLow!: QpHyperIconCustomElement;
	@bindable iconPriorityDarkHigh!: QpHyperIconCustomElement;
	@bindable iconPriorityDarkMedium!: QpHyperIconCustomElement;
	@bindable iconPriorityDarkLow!: QpHyperIconCustomElement;
	@bindable iconSeverityDarkHigh!: QpHyperIconCustomElement;
	@bindable iconSeverityDarkMedium!: QpHyperIconCustomElement;
	@bindable iconSeverityDarkLow!: QpHyperIconCustomElement;
	@bindable iconMultiEmployee!: QpHyperIconCustomElement;
	@bindable iconMultiEmployeeDark!: QpHyperIconCustomElement;
	@bindable iconDatePicker!: QpHyperIconCustomElement;

	@observable({ changeHandler: 'datePicked' }) pickerDate;
	private datePickerReentryPrevention = false;

	@observable({ changeHandler: 'tabServiceOrdersVisibleChanged'}) tabServiceOrdersVisible;
	@observable({ changeHandler: 'tabSearchAppointmentsVisibleChanged'}) tabSearchAppointmentsVisible;

	private preferencesBase = new FS300300_Preferences();
	private get preferences() { return this.preferencesBase?.preferences as PreferenceData; }
	protected get splitterPreferences() {
		return (this.orientation === 'horizontal') ? this.preferences?.horizontalModeSplitter : this.preferences?.verticalModeSplitter;
	}
	private actualizingPreferences = false;
	private updatingObservedItem = false;
	private backlogEventResizing = false;
	private initializationDataProcessed = false;
	private calendarRedrawSuppressed = false;

	private apiClient: ISchedulerApiClient;
	private backlogControl: SchedulerPro | null;
	private schedulerControl: SchedulerPro | null;
	private serviceOrdersControl: Grid | null;
	private searchAppointmentsControl: Grid | null;
	private lockedSubGridBacklog: SubGrid | null;
	private normalSubGridBacklog: SubGrid | null;
	private lockedSubGrid: SubGrid | null;
	private normalSubGrid: SubGrid | null;
	private preservedSelectedRows: Model[] | number[];

	// private map: Microsoft.Maps.Map;
	// private mapCredentials: string;

	private saveSplitterPositionTimer: ReturnType<typeof setTimeout>;
	private updateMapTimer: ReturnType<typeof setTimeout>;
	private eventPopup: Popup | null = null;
	private serviceOrderDragHelper: ExternalEventDragHelper | null;
	private sODetailsDragHelper: ExternalEventDragHelper | null;
	private appointmentsWithNoEmployeeDragHelper: ExternalEventDragHelper | null;
	private resourceDragHelper: ResourceDragHelper | null;
	private createdEventRecord: SchedulerEventModel | null = null;
	private defaultSrvOrdType: string | null = null;
	private prevHoveredResource: SchedulerResourceModel;
	private popupEntityId: string | number;
	private popupAssignmentId: string | number;

	private toolbarElement!: HTMLElement;
	private serviceOrderPopupInner!: HTMLElement;
	private serviceOrderPopupInnerHolder!: HTMLElement;
	private appointmentPopupInner!: HTMLElement;
	private appointmentPopupInnerHolder!: HTMLElement;
	private editPopupInner!: HTMLElement;
	private editPopupInnerHolder!: HTMLElement;
	private appointmentPopupEmployeesForm!: HTMLElement;
	private editedAppointmentPopupEmployeesForm!: HTMLElement;
	private serviceOrderEmployeesForm!: HTMLElement;
	private appointmentPopupEmployees!: HTMLElement;
	private mainTabBarHolder!: HTMLElement;
	private lastUpdatedGridElement!: HTMLElement;
	private schedulerFilter: HTMLElement;
	private prevPeriodButton: HTMLElement;
	private datePickerMain: HTMLElement;
	private serviceOrdersFastFilter!: HTMLInputElement;
	private searchAppointmentsFastFilter!: HTMLInputElement;
	private appointmentsAllStaffGridElement: HTMLElement;
	private calendarContainer: HTMLElement;

	private domEvents: ElementEvents;
	private tabObservers = new Map<string, MutationObserver>();
	private popupSizeObserver: ResizeObserver;

	private dragCreateResourceIDsSet: Set<string>;
	private aureliaEnhancedElements = new Set<HTMLElement>();

	private appointments = new AppointmentsDataHandler();
	private serviceOrders = new SODataHandler();
	private searchAppointments = new SearchAppointmentsDataHandler();
	private get graphName() { return this.screenService.getGraphInfo()?.graphType; }

	private workingHoursSignature: string;

	private currentSearchAppointment: SearchAppointmentModel | null = null;
	private ignoreNextSchedulerGridUpdate = false;
	private subscribedToAppointmentsBrickPreferences = false;
	private subscribedToSearchAppointmentsBrickPreferences = false;
	private subscribedToSOBrickPreferences = false;

	private alreadyDisplayedWorkingCalendarWarning = false;

	private startDatePickerConfig = {
		id: `scheduler_date`, class: 'dropDown auto-size',
		editMask: '04/08/03',
		displayMask: '04/08/03',
		allowCustomItems: false,
		// timeMode: this.col.timeMode
	};

	private mainSplitterConfig: ISplitterConfig = {
		split: 'width',
		initialSplit: "300px",
		initialState: 'normal',
		style: 'height: 100%',
		fading: true,
	};
	private mapSplitterConfig: ISplitterConfig = {
		split: 'width',
		initialSplit: "60%",
		initialState: 'collapsed-second',
		disableCollapseFirst: true,
		style: 'height: 100%',
		fading: true,
	};



	constructor(
		public element: Element,
		gridApiClient: SchedulerApiClient,
		private preferencesService: PreferencesService,
		private bindingEngine: BindingEngine,
		private templatingEngine: TemplatingEngine,
		private au: Aurelia,
	) {
		super();
		this.apiClient = gridApiClient;

		const curDate = new Date().clearHoursUTC();
		this.pickerDate = curDate;
	}

	async attached() {
		this.preferencesService.subscribe(this.prefid, this);

		await super.attached();

		const filterBoxInput = document.querySelector('.qp-sch-grid-container-inner qp-filter-box input') as HTMLInputElement;
		filterBoxInput.placeholder = Captions.Staff;

		this.calendarContainer = document.querySelector('.qp-sch-calendar-container') as HTMLElement;

		this.attachSOPane();
		this.attachCalendar();
		this.attachDragHelpers();
		this.attachToolbarHandling();

		await this.ReleaseUIControl(); // allow engine to position datepicker control before we update it
		this.onDatesFilterUpdated();
		this.attachMap();

		this.mainTabBarHolder = document.getElementById('mainTabBarHolderID');
		this.lastUpdatedGridElement = document.getElementById('lastUpdatedGridID');
		this.appointmentsAllStaffGridElement = document.getElementById('appointmentsAllStaffGridID');
		this.datePickerMain = document.getElementById('scheduler_date');
		this.appointmentPopupEmployeesForm = document.getElementById('appointmentPopupEmployeesFormID');
		this.editedAppointmentPopupEmployeesForm = document.getElementById('editedAppointmentPopupEmployeesFormID');
		this.serviceOrderEmployeesForm = document.getElementById('serviceOrderEmployeesFormID');
		this.serviceOrdersFastFilter = document.getElementById('serviceOrderGridID_fb_text') as HTMLInputElement;
		this.searchAppointmentsFastFilter = document.getElementById('searchAppointmentsGridID_fb_text') as HTMLInputElement;

		const testHelper = this.calendarContainer as TestHelperHTMLElement;
		testHelper.scrollToXPercent = testHelperScrollToXPercent;
		testHelper.simulateDragAndDrop = testHelperSimulateDragAndDrop;
		testHelper.schedulerControl = this.schedulerControl;

		this.setupPopups();
	}

	protected async onAfterInitialize() {
		await this.setupTabs();
		this.setupObservers();
	}

	detached() {
		super.detached();
		this.detachCalendar();
		this.detachTabObservers();
	}

	public applyPreferences(prefs: IPreferencesPair<FS300300_Preferences>) {
		this.preferencesBase = new FS300300_Preferences(prefs.user?.preferences);
		this.actualizePreferences();
	}

	public applySOBrickLeftPreferences(prefs: IPreferencesPair<FieldsetPreferences>) {
		this.serviceOrdersBrickLeft.applyPreferences(prefs);
		this.updateSOPane(false);
	}

	public applySOBrickRightPreferences(prefs: IPreferencesPair<FieldsetPreferences>) {
		this.serviceOrdersBrickRight.applyPreferences(prefs);
		this.updateSOPane(false);
	}

	public applySearchAppointmentsBrickLeftPreferences(prefs: IPreferencesPair<FieldsetPreferences>) {
		this.searchAppointmentBrickLeft.applyPreferences(prefs);
		this.updateSearchAppointmentsPane(false);
	}

	public applySearchAppointmentsBrickRightPreferences(prefs: IPreferencesPair<FieldsetPreferences>) {
		this.searchAppointmentBrickRight.applyPreferences(prefs);
		this.updateSearchAppointmentsPane(false);
	}

	public applyAppointmentsBrickLeftPreferences(prefs: IPreferencesPair<FieldsetPreferences>) {
		this.appointmentsBrickLeft.applyPreferences(prefs);
		if (this.schedulerControl) {
			this.updateCalendar();
		}
	}

	public applyAppointmentsBrickRightPreferences(prefs: IPreferencesPair<FieldsetPreferences>) {
		this.appointmentsBrickRight.applyPreferences(prefs);
		if (this.schedulerControl) {
			this.updateCalendar();
		}
	}

	protected actualizePreferences() {
		if (!this.lockedSubGrid) return;
		this.actualizingPreferences = true;
		try {
			const splitterPreferences = (this.orientation === 'horizontal')
				? this.preferences?.horizontalModeSplitter : this.preferences?.verticalModeSplitter;

			this.lockedSubGridBacklog.collapsed = splitterPreferences?.state === 'collapsed-first';
			this.normalSubGridBacklog.collapsed = splitterPreferences?.state === 'collapsed-second';
			this.lockedSubGridBacklog.width = splitterPreferences?.position ?? this.lockedSubGrid.width;
			this.lockedSubGrid.collapsed = splitterPreferences?.state === 'collapsed-first';
			this.normalSubGrid.collapsed = splitterPreferences?.state === 'collapsed-second';
			this.lockedSubGrid.width = splitterPreferences?.position ?? this.lockedSubGrid.width;
			if (splitterPreferences?.state === 'collapsed-first') {
				this.lockedSubGridBacklog.width = undefined;
				this.lockedSubGridBacklog.flex = undefined;
				this.lockedSubGrid.width = undefined;
				this.lockedSubGrid.flex = undefined;
			}
			if (splitterPreferences?.state === 'collapsed-second') {
				this.lockedSubGrid.width = undefined;
				this.lockedSubGrid.flex = '1 1 0%';
			}

			if (this.preferences?.periodKind === PeriodKind.Week) {
				this.setWeekPeriod();
			}
			if (this.preferences?.periodKind === PeriodKind.Month) {
				this.setMonthPeriod();
			}
		} finally {
			this.actualizingPreferences = false;
		}
	}

	protected async savePreferences() {
		if (this.actualizingPreferences) return;
		console.log(`savePreferences: ${this.preferences.toString()}`);
		await this.preferencesService.saveUserPreferences(getScreenID(), this.prefid, this.preferencesBase);
	}

	protected async setupTabs() {
		await this.setSchedulerTabs();
		//this.schedulerTabBar.tabsChanged();
	}

	protected attachToolbarHandling()
	{
		this.domEvents = new ElementEvents(this.element);
		this.domEvents.subscribe("buttonpressed", (e: CustomEvent) => this.processToolBarClick(e));
		this.schedulerFilter = document.querySelector('.qp-sch-grid-container-inner .grid-right-toolbar-cont');
		this.prevPeriodButton = document.querySelector('#rightToolBarConfigIDprevPeriod');
	}

	protected async attachSOPane() {
		const schedulerSOContainer = document.getElementById('schedulerSOContainer');
		const data = this.serviceOrders.createData();

		const oldServiceOrdersControl = this.serviceOrdersControl;
		this.serviceOrdersControl = new Grid({
			appendTo: schedulerSOContainer,
			features: {
				cellEdit: false,
				cellMenu: false,
			},
			columns: [{
				editable: false,
				text: 'serviceOrder',
				field: 'serviceOrder',
				flex: 1,
				cellCls: '',
				autoHeight: true,
				htmlEncode: false,
				renderer: this.serviceOrderRenderer.bind(this),
			}],
			data: data,
			onBeforeSelectionChange: () => false,
			onCellClick: this.onSOGridCellClick.bind(this),
			onScroll: this.onServiceOrdersScroll.bind(this),
			onCellMouseOver: this.onSOGridCellMouseOver.bind(this),
		});

		oldServiceOrdersControl?.destroy();
	}

	protected detachSOPane() {
		if (this.serviceOrdersControl) {
			this.serviceOrdersControl.destroy();
			this.serviceOrdersControl = null;
		}
	}


	protected async updateSOPane(fullUpdate = false) {
		if (!this.serviceOrdersControl || fullUpdate) {
			this.detachSOPane();
			this.attachSOPane();
		}
		else {
			const store = this.serviceOrdersControl?.storeScroll();
			this.serviceOrdersControl.data = this.serviceOrders.createData();
			this.serviceOrdersControl?.restoreScroll(store);
		}
	}

	public getCaptionText() { return ""; }

	protected serviceOrderRenderer({ record, expanderElement, rowElement, region }) {
		const serviceOrder = this.serviceOrders.getEntry(record.id);
		const children = this.serviceOrders.getChildren(serviceOrder.orderId);
		const onlyChild = children?.filter(x => !x.isScheduled).length === 1;
		const needShowChildren = children?.length > 1;
		const testHelperScripts = `
			onPointerEnter = "this.classList.toggle('qp-sch-hover', true);"
			onPointerLeave = "this.classList.toggle('qp-sch-hover', false);"
		`;

		const serviceParts = needShowChildren
			? this.serviceOrders.getChildren(serviceOrder.orderId)?.map(service =>
				`
				<div ${testHelperScripts} class="qp-sch-event-info qp-sch-service-info
					${onlyChild ? 'only-child' : ''}
					${service.isScheduled ? 'scheduled' : ''}"
					${onlyChild ? '' : `entryId="${service.serviceId}"`}><div><ul><li>
					<span>${this.applyMarks(StringHelper.xss `${service.FSSODet__TranDesc.cellText}`, this.serviceOrdersFastFilter?.value)}</span>
					</li></ul></div>
				</div>
				`
			)
			: [];
		const servicePartsHtml = serviceParts ? `<div><div></div>${serviceParts.join('')}</div>` : ""; // empty <div> is needed to make '+ .qp-sch-service-info' css selector work
		return `
			<div class="qp-sch-not-assigned qp-sch-so" entryId="${serviceOrder.orderId}">
				${this.getServiceOrderHtml(serviceOrder)}
				${servicePartsHtml}
			</div>
			`;
	}

	protected async tabServiceOrdersVisibleChanged() {
		if (!this.tabServiceOrdersVisible) return;
		await this.ReleaseUIControl(); // allow the engine to render tab first
		this.updateSOPane(false);
	}

	protected async attachSearchAppointmentsPane() {
		const searchAppointmentContainer = document.getElementById('searchAppointmentContainer');
		const data = this.searchAppointments.createData();

		const oldSearchAppointmentsControl = this.searchAppointmentsControl;
		this.searchAppointmentsControl = new Grid({
			appendTo: searchAppointmentContainer,
			features: {
				cellEdit: false,
				cellMenu: {
					items: this.getMenuItems(),
					processItems: this.processMenuItems.bind(this),
				},

			},
			columns: [{
				editable: false,
				text: 'searchAppointment',
				field: 'searchAppointment',
				flex: 1,
				cellCls: '',
				autoHeight: true,
				htmlEncode: false,
				renderer: this.searchAppointmentRenderer.bind(this),
			}],
			data: data,
			onBeforeSelectionChange: () => false,
			onCellClick: this.onSearchAppointmentGridCellClick.bind(this),
			onScroll: this.onSearchAppointmentsScroll.bind(this),
			onCellMouseOver: this.onSearchAppointmentsMouseOver.bind(this),
		});

		oldSearchAppointmentsControl?.destroy();
	}

	protected detachSearchAppointmentsPane() {
		if (this.searchAppointmentsControl) {
			this.searchAppointmentsControl.destroy();
			this.searchAppointmentsControl = null;
		}
	}


	protected async updateSearchAppointmentsPane(fullUpdate = false) {
		if (!this.searchAppointmentsControl || fullUpdate) {
			this.detachSearchAppointmentsPane();
			this.attachSearchAppointmentsPane();
		}
		else {
			const store = this.searchAppointmentsControl?.storeScroll();
			this.searchAppointmentsControl.data = this.searchAppointments.createData();
			this.searchAppointmentsControl?.restoreScroll(store);
		}
	}

	protected searchAppointmentRenderer({ record, row, expanderElement, rowElement, region }) {
		const appointment = this.searchAppointments.getEntry(record.id);
		let cls = `b-sch-event ${getAppointmentClass(appointment)}`;
		if (this.currentSearchAppointment?.appointmentID === appointment.appointmentID) {
			cls = `${cls} qp-sch-search-highlight`;
		}
		return `
			<div class="${cls}">
				<div class="b-sch-event-content">
					${this.renderAppointmentWithStatusBar(appointment, null, true)}
				</div>
			</div>`;

		function getAppointmentClass(assignment: SearchAppointmentModel) {
			if (assignment.isLocked) {
				return "qp-sch-locked";
			}
			const isConfirmed = appointment.Confirmed?.value || appointment.ValidatedByDispatcher?.value;
			const confirmedClass = isConfirmed ? "" : "qp-sch-not-confirmed";
			const assignedClass = appointment.StaffCntr.value ? "" : "qp-sch-not-assigned";
			return `${confirmedClass} ${assignedClass}`;
		}
	}

	protected async tabSearchAppointmentsVisibleChanged() {
		if (!this.tabSearchAppointmentsVisible) return;
		await this.ReleaseUIControl(); // allow the engine to render tab first
		this.updateSearchAppointmentsPane(false);
	}

	protected async attachCalendar() {
		if (this.schedulerControl) return;

		const schedulerMainContainer = document.getElementById('schedulerMainContainer');
		const [periodStartDate, periodEndDate] = AppointmentsDataHandler.getPeriodStartEndDates(this.DatesFilter);
		if (!periodStartDate) return; // still waiting for the date from the server

		const [backlogProject, mainProject] = this.appointments.createProjects(this.DatesFilter, this.currentSearchAppointment);
		if (!mainProject.resources?.length) return;

		this.workingHoursSignature = this.appointments.getWorkingHoursSignature();

		this.handleNoWorkingCalendarWarning();

		const oldBacklogControl = this.backlogControl;
		this.backlogControl = new SchedulerPro({
			appendTo: schedulerMainContainer,
			startDate: periodStartDate,
			endDate: periodEndDate,
			snap: true,
			createEventOnDblClick: false,
			cls: 'qp-sch-backlog',
			autoHeight: true,
			rowHeight: this.getAppointmentBrickHeight(),
			barMargin: 5,
			resourceMargin: 2,
			columns: [{ field: 'name', text: Captions.Staff, flex: 1 }],
			viewPreset: this.getViewPreset(),
			timeAxis: { continuous: false, },
			onEventClick: this.onEventClick.bind(this),
			onBeforeEventResize: () => { this.backlogEventResizing = true; },
			onEventResizeEnd: () => { this.backlogEventResizing = false; },
			onBeforeEventResizeFinalize: this.onBeforeEventResizeFinalize.bind(this),
			onBeforeSelectionChange: () => false,
			onTimeAxisHeaderClick: this.onTimeAxisHeaderClick.bind(this),
			onDragCreateEnd: this.onDragCreateEnd.bind(this),
			onDragCreateStart: this.onDragCreateStart.bind(this),
			// eventLayout: 'pack',
			project: backlogProject,
			features: {
				cellMenu: false,
				cellTooltip: false,
				cellCopyPaste: false,
				columnRename: false,
				eventMenu: {
					items: this.getMenuItems(),
					processItems: this.processMenuItems.bind(this),
				},
				headerMenu: false,
				resourceMenu: false,
				scheduleMenu: false,
				timeAxisHeaderMenu: false,
				resourceTimeRanges: true,
				dependencies: false,
				eventTooltip: false,
				eventDrag: false, /*{
					//snapToPosition: this.snapToPosition.bind(this),
					constrainDragToTimeline: false,
					dragHelperConfig: null,
				},*/
				eventDragCreate: {
					allowResizeToZero: false,
					showExactResizePosition: true,
					showTooltip: true,
				},
				timeRanges: {
					showCurrentTimeLine: false,
					showHeaderElements: false,
					enableResizing: false
				},
				scheduleTooltip: false,
			},
			eventRenderer: this.unassignedRenderer.bind(this),
		});

		const oldSchedulerControl = this.schedulerControl;
		this.schedulerControl = new SchedulerPro({
			appendTo: schedulerMainContainer,
			partner: this.backlogControl,
			hideHeaders: true,
			mode: this.orientation,
			enableEventAnimations: false,
			createEventOnDblClick: false,
			snap: true,
			cls: 'qp-sch-calendar',
			//displayDateFormat
			//durationDisplayPrecision
			//enableEventAnimations
			//eventColor
			//textAlign: 'start'
			// forceFit: true,
			// workingTime: {fromHour: 13, toHour: 14},
			project: mainProject,
			startDate: periodStartDate,
			endDate: periodEndDate,
			maxTimeAxisUnit: 'hour',
			columns: this.backlogControl.columns,
			viewPreset: this.getViewPreset(),
			// timeAxis: { continuous: false, },
			// timeAxis: this.backlogControl.timeAxis,
			onBeforeEventResizeFinalize: this.onBeforeEventResizeFinalize.bind(this),
			onBeforeEventDropFinalize: this.onBeforeEventDropFinalize.bind(this),
			onEventClick: this.onEventClick.bind(this),
			onEventMouseEnter: this.onEventMouseEnter.bind(this),
			onSelectionChange: this.onSelectionChange.bind(this),
			onSubGridCollapse: this.onSubGridCollapse.bind(this),
			onSubGridExpand: this.onSubGridExpand.bind(this),
			onBeforeCellEditStart: () => false,
			onGridRowBeforeDragStart: () => false,
			onVisibleDateRangeChange: this.onVisibleDateRangeChange.bind(this),
			onDragCreateEnd: this.onDragCreateEnd.bind(this),
			onDragCreateStart: this.onDragCreateStart.bind(this),
			onBeforeEventDrag: this.onBeforeEventDrag.bind(this),
			onEventDragReset: this.onEventDragReset.bind(this),
			eventRenderer: this.assignmentRenderer.bind(this),
			selectionMode: {
				deselectFilteredOutRecords: true,
				// column: true,
				// cell: true,
			},
			features: {
				cellMenu: false,
				cellTooltip: false,
				cellCopyPaste: false,
				columnRename: false,
				eventMenu: {
					items: this.getMenuItems(),
					processItems: this.processMenuItems.bind(this),
				},
				headerMenu: false,
				resourceMenu: false,
				scheduleMenu: false,
				timeAxisHeaderMenu: false,
				resourceTimeRanges: true,
				dependencies: false,
				eventTooltip: false,
				eventDrag: {
					snapToPosition: this.snapToPosition.bind(this),
					// constrainDragToTimeline: false,
				},
				eventDragCreate: {
					allowResizeToZero: false,
					showExactResizePosition: true,
					showTooltip: true,
				},
				scheduleTooltip: false,
				timeRanges: {
					showCurrentTimeLine: false,
					showHeaderElements: false,
					enableResizing: false
				}
			},
			rowHeight: this.getAppointmentBrickHeight(),
			barMargin: 5,
			resourceMargin: 2,
			// transitionDuration: 100,
			useInitialAnimation: false,
			// maxZoomLevel: 12,
			// minZoomLevel: 12,
		});

		oldSchedulerControl?.destroy();
		oldBacklogControl?.destroy();

		if (mainProject.resources.filter(r => r.id === AppointmentEmployeeModel.unassignedId).length === 0) {
			// this.backlogControl.contentElement.parentElement.style.display = 'none';
			// this.backlogControl.element.style.minHeight = '0px';
		}

		this.lockedSubGridBacklog = this.backlogControl.getSubGrid(`locked`);
		this.normalSubGridBacklog = this.backlogControl.getSubGrid(`normal`);
		this.lockedSubGrid = this.schedulerControl.getSubGrid(`locked`);
		this.normalSubGrid = this.schedulerControl.getSubGrid(`normal`);

		this.lockedSubGrid.onResize = this.onLockedSubGridResize.bind(this);

		this.attachMouseEventHandlers();
		this.setDatePickerText();
		this.restoreSelection();
		this.applyHoursFilter();
		this.actualizePreferences();
	}

	protected getViewPreset() : Partial<ViewPresetConfig> {
		return {
			tickWidth: 40,
			displayDateFormat: 'll HH:mm',
			shiftIncrement: 1,
			shiftUnit: 'day',
			defaultSpan: 24,
			timeResolution: { unit: 'minute', increment: Number(this.Setup.AppResizePrecision.value) },
			headers: [
				{ unit: 'day', dateFormat: Formats.TimeAxisDay },
				{ unit: 'hour', dateFormat: Formats.TimeAxisHour }
				// { unit: 'week', dateFormat: 'ddd MM/DD/YYYY' },
				// { unit: 'day', dateFormat: 'DD' },
			]
		};
	}

	protected restoreSelection() {
		if (this.preservedSelectedRows?.length > 0) {
			const filteredPreservedSelectedRows = (this.preservedSelectedRows as Model[])
				.filter(row => this.appointments.resources.has(row.id.toString()));
			this.schedulerControl.selectRows(filteredPreservedSelectedRows, null);
		}
		if (this.schedulerControl.selectedRows.length === 0 && this.appointments.resources.size > 0) {
			this.schedulerControl.selectRow(this.appointments.resources.values().next().value);
		}
	}

	protected handleNoWorkingCalendarWarning() {
		const warningNoCalendarNeeded = this.appointments.resources.size > 0 && !this.appointments.hasWorkingCalendarSet();
		if (!this.alreadyDisplayedWorkingCalendarWarning && warningNoCalendarNeeded) {
			showInformer(Messages.NoWorkingCalendarSet, "warning");
			this.alreadyDisplayedWorkingCalendarWarning = true;
		}
		if (!warningNoCalendarNeeded) {
			this.alreadyDisplayedWorkingCalendarWarning = false;
		}
	}

	protected attachMap() {
		this.mainMap.initialize(this.InitData.MapAPIKey.value);
	}

	protected detachCalendar() {
		if (this.schedulerControl) {
			this.schedulerControl.destroy();
			this.schedulerControl = null;
		}
		if (this.backlogControl) {
			this.backlogControl.destroy();
			this.backlogControl = null;
		}
	}

	protected async updateCalendar() {
		if (!this.schedulerControl) {
			this.attachCalendar();
		}
		else {
			const [newPeriodStartDate, newPeriodEndDate] = AppointmentsDataHandler.getPeriodStartEndDates(this.DatesFilter);
			const [backlogProject, mainProject] = this.appointments.createProjects(this.DatesFilter, this.currentSearchAppointment, this.calendarRedrawSuppressed);
			const newWorkingHoursSignature = this.appointments.getWorkingHoursSignature();

			const datesChanged = this.backlogControl.startDate !== newPeriodStartDate || this.backlogControl.endDate !== newPeriodEndDate;
			const workingHoursChanged = this.workingHoursSignature !== newWorkingHoursSignature;
			this.workingHoursSignature = newWorkingHoursSignature;

			if (this.calendarRedrawSuppressed) return;

			this.schedulerControl.suspendEvents();
			this.backlogControl.suspendEvents();
			this.schedulerControl.project = mainProject;
			this.schedulerControl.rowHeight = this.getAppointmentBrickHeight();

			this.backlogControl.project = backlogProject;
			this.backlogControl.rowHeight = this.getAppointmentBrickHeight();

			if (datesChanged || workingHoursChanged) {
				this.backlogControl.setTimeSpan(newPeriodStartDate, newPeriodEndDate);
				// this.applyHoursFilter();
			}

			this.schedulerControl.renderContents();

			this.backlogControl.resumeEvents();
			this.schedulerControl.resumeEvents();
			await this.ReleaseUIControl();
		}

		if (this.appointments.hasData()) {
			await this.processInitializationData();
		}
	}

	protected async processInitializationData() {
		if (this.initializationDataProcessed) return;

		if (this.MainAppointmentFilter.InitialRefNbr.value?.length > 0) {
			this.initializationDataProcessed = true;
			this.mainTabBar.showTab('tabAppointments');
			const appointmentID = this.MainAppointmentFilter.InitialRefNbr.value;
			await this.searchAppointmentsGrid.onFastFilter(appointmentID);
			const searchAppointment = this.searchAppointments.getFirstEntry();
			await this.selectSearchAppointment(searchAppointment);
		}
		if (this.MainAppointmentFilter.InitialSORefNbr.value?.length > 0) {
			this.initializationDataProcessed = true;
			this.mainTabBar.showTab('tabServiceOrders');
			await this.serviceOrdersGrid.onFastFilter(this.MainAppointmentFilter.InitialSORefNbr.value);
		}
	}

	protected getServiceEntryIdFromElement(element: HTMLElement) {
		while (element.parentElement && element.getAttribute('entryId') === null) {
			element = element.parentElement;
		}
		const entryId = element.getAttribute('entryId');
		return entryId;
	}

	protected getEventIdFromElement(element: HTMLElement) {
		while (element.parentElement && !(<any> element.parentElement).elementData) {
			element = element.parentElement;
		}
		const eventId = (<any> element.parentElement).elementData.assignment.eventId;
		return eventId;
	}

	protected attachDragHelpers() {
		this.serviceOrderDragHelper = new ExternalEventDragHelper(
			{
				owner: this,
				getSnapToResourcePosition: this.getSnapToResourcePosition,
				getScheduler: () => this.schedulerControl,
				getEntity: (element: HTMLElement) =>  {
					const eventId = this.getServiceEntryIdFromElement(element);
					return this.serviceOrders.getEntry(eventId.toString());
				},
				getProxyInner: (entity: DraggableEntity) => this.renderDraggedServiceOrder(entity),
				getEventInfo: (entity: DraggableEntity) => this.getEventInfoFromSO(entity as ServiceOrderModel, false),
				getAppointmentStart: (resourceId: string, entity: DraggableEntity) => this.getSuggestedTimePeriod(resourceId, this.getSODurationMS(entity as ServiceOrderModel, true)),
				getDurationMS: (entity: DraggableEntity) => this.getSODurationMS(entity as ServiceOrderModel, true),
				scheduleEntity: this.createAppointmentFromSO,
			}, {
				targetSelector: '.qp-sch-left-container .qp-sch-not-assigned:not(:has(.qp-sch-service-info:not(.only-child).qp-sch-hover))',
				hideOriginalElement: true,
			}
		);

		this.sODetailsDragHelper = new ExternalEventDragHelper(
			{
				owner: this,
				getSnapToResourcePosition: this.getSnapToResourcePosition,
				getScheduler: () => this.schedulerControl,
				getEntity: (element: HTMLElement) =>  {
					const eventId = this.getServiceEntryIdFromElement(element);
					return this.serviceOrders.getEntry(eventId.toString());
				},
				getProxyInner: (entity: DraggableEntity) => this.renderDraggedServiceOrder(entity, true),
				getEventInfo: (entity: DraggableEntity) => this.getEventInfoFromSO(entity as ServiceOrderModel, true),
				getAppointmentStart: (resourceId: string, entity: DraggableEntity) => this.getSuggestedTimePeriod(resourceId, this.getSODurationMS(entity as ServiceOrderModel, false)),
				getDurationMS: (entity: DraggableEntity) => this.getSODurationMS(entity as ServiceOrderModel, false),
				scheduleEntity: this.createAppointmentFromSODetail,
			}, {
				targetSelector: '.qp-sch-left-container .qp-sch-service-info:not(.only-child):not(.scheduled)',
				hideOriginalElement: true,
			}
		);

		this.appointmentsWithNoEmployeeDragHelper = new ExternalEventDragHelper(
			{
				owner: this,
				getSnapToResourcePosition: this.getSnapToResourcePosition,
				getScheduler: () => this.schedulerControl,
				getEntity: (element: HTMLElement) =>  {
					const eventId = this.getEventIdFromElement(element);
					return this.appointments.getUnassignedAppointment(eventId.toString());
				},
				getProxyInner: (entity: DraggableEntity) => this.renderAppointmentWithStatusBar(entity as AppointmentEmployeeModel),
				getEventInfo: (entity: DraggableEntity) => this.getEventInfoFromAppointment(entity as AppointmentEmployeeModel),
				getAppointmentStart: (resourceId: string, entity: DraggableEntity) => new Date((entity as AppointmentEmployeeModel)?.dateTimeBegin),
				getNewEventId: (entity: DraggableEntity) => (entity as AppointmentEmployeeModel).appointmentID,
				getDurationMS: (entity: DraggableEntity) => this.getAppointmentDurationMS(entity as AppointmentEmployeeModel),
				toggleTimeProjection: (start: Date, end: Date) => this.toggleTimeProjection(start, end),
				allowDragging: () => !this.backlogEventResizing,
				scheduleEvent: this.assignUnassignedAppointment,
			}, {
				targetSelector: '.qp-sch-main-container > .b-container:nth-child(2) .b-sch-event',
				hideOriginalElement: true,
			}
		);

		this.resourceDragHelper = new ResourceDragHelper(
			{
				owner: this,
				getScheduler: () => this.schedulerControl,
				// getProxyInner: (row: IGridRow) => StringHelper.xss`
				// 	<table><tr><td colspan="2" nowrap>${this.getSOName(row)}</td></tr></table>`,
				// Infoow: IGridRow) => this.getSOname(row),
				getEventInfo: (assignmentId: string) => this.getEventInfoFromAssignment(assignmentId),
				addResource: this.addResourceToAppointment,
			}, {
				targetSelector: (this.orientation === 'vertical') ? '.b-resourceheader-cell' : '.b-grid-subgrid-locked .b-grid-row'
			}
		);
	}

	protected getSODurationMS(serviceOrder: ServiceOrderModel, needTotal: boolean) {
		const duration = (needTotal
			? this.serviceOrders.getEstimatedDuration(serviceOrder.orderId)
			: TimeConverter.hMMtoMinutes(serviceOrder.FSSODet__EstimatedDuration.value)
		) * 60 * 1000; // eslint-disable-line @typescript-eslint/no-magic-numbers
		if (duration > 0) {
			return duration;
		}
		return 60 * 60 * 1000; // eslint-disable-line @typescript-eslint/no-magic-numbers
	}

	protected getAppointmentDurationMS(appointment: AppointmentEmployeeModel | SearchAppointmentModel) {
		const duration = appointment.dateTimeEnd.getTime() - appointment.dateTimeBegin.getTime();
		if (duration > 0) {
			return duration;
		}
		return 60 * 60 * 1000; // eslint-disable-line @typescript-eslint/no-magic-numbers
	}

	protected async setDayPeriod() {
		if (this.DatesFilter?.PeriodKind.value === PeriodKind.Day) return;
		this.DatesFilter?.PeriodKind.updateValue(PeriodKind.Day);
		await this.reloadForDate(this.pickerDate, true);
	}

	protected async setWeekPeriod() {
		if (this.DatesFilter?.PeriodKind.value === PeriodKind.Week) return;
		this.DatesFilter?.PeriodKind.updateValue(PeriodKind.Week);
		await this.reloadForDate(this.pickerDate, true);
	}

	protected async setMonthPeriod() {
		if (this.DatesFilter?.PeriodKind.value === PeriodKind.Month) return;
		this.DatesFilter?.PeriodKind.updateValue(PeriodKind.Month);
		await this.reloadForDate(this.pickerDate, true);
	}

	protected getSOName(row: IGridRow) {
		if (row == null) return "";
		return StringHelper.xss`${row.cells.SrvOrdType.cellText} ${row.cells.RefNbr.text}`;
	}

	protected getEventInfoFromSO(entity: ServiceOrderModel, sODetails: boolean): DragHelperEventInfo {
		return {
			name: sODetails ? entity.serviceId : entity.orderId,
			entity: entity,
			date: entity.OrderDate ? new Date(entity.OrderDate?.value) : null,
			customerName: entity.CustomerAcctName?.cellText ?? "",
		};
	}

	protected getEventInfoFromAppointment(appoointment: AppointmentEmployeeModel): DragHelperEventInfo {
		return {
			name: appoointment.caption,
			entity: appoointment,
			date: null,
			customerName: appoointment.SchedulerServiceOrder__CustomerAcctName?.cellText ?? "",
		};
	}

	protected getEventInfoFromAssignment(assignmentId: string): DragHelperEventInfo {
		const assignment = this.appointments.getAssignment(assignmentId);
		if (!assignment) return null;
		return {
			name: '',
			entity: assignment,
			date: null,
			customerName: assignment.SchedulerServiceOrder__CustomerAcctName?.cellText ?? "",
		};
	}

	protected getSuggestedTimePeriod(resourceId: string, duration: number) {
		if (resourceId == null) return null;

		const visibleRange = this.schedulerControl.visibleDateRange;
		if (visibleRange?.startDate == null || visibleRange?.startDate == null) return null;

		return this.appointments.findEmptyTimePeriod(resourceId, duration, visibleRange.startDate, visibleRange.endDate);
	}

	protected async onActiveFilterChanged(args: QpGridEventArgs) {
		await this.setSchedulerTabs();
		const filter = this.schedulerGrid.getActiveFilter();
		this.schedulerTabBar.showTab(filter.filterID);
	}

	protected async onFiltersChanged(args: QpGridEventArgs) {
		await this.setSchedulerTabs();
	}

	protected detachTabObservers() {
		this.tabObservers.forEach((value, key) => {
			value?.disconnect();
		});
		this.tabObservers.clear();
	}

	protected async setSchedulerTabs() {
		this.SchedulerGridFilters = this.schedulerGrid.getFilters();
		await this.ReleaseUIControl(); // allow the engine to redraw tabs

		this.detachTabObservers();

		this.SchedulerGridFilters?.forEach(tab => {
			const observer = attachObserver.bind(this)(`schedulerTabBarId_${tab.filterID}_tabbar`, () => {
				if (this.schedulerGrid.getActiveFilter()?.filterID !== tab.filterID) {
					this.schedulerGrid.setActiveFilter(tab.filterID);
				}
			});
			if (observer != null) {
				this.tabObservers.set(tab.filterID, observer);
			}
		});

		function attachObserver(id: string, listener: () => void): MutationObserver {
			const element = document.getElementById(id); // HACK: that's just a quick hack
			if (element == null) return null;

			const observer = new MutationObserver(mutations => {
				mutations.forEach(mutation => {
					if (mutation.attributeName === "class") {
						const previousClasses = mutation.oldValue;
						const classes = element.getAttribute("class");
						if (previousClasses.indexOf("qp-tabbar-active") < 0 && classes.indexOf("qp-tabbar-active") >= 0) {
							// eslint-disable-next-line @typescript-eslint/no-invalid-this
							listener.bind(this)();
						}
					}
				});
			});

			observer.observe(element, {
				attributes: true, attributeOldValue: true, attributeFilter: ['class']
			});

			return observer;
		}
	}

	protected serviceOrdersDataReadyHandler(grid: QpGridCustomElement, args: QpGridEventArgs) {
		if (!grid.view) return;

		const items = [...Array(grid.rows.length).keys()].filter(i => grid.rows[i]).map(i => grid.view.getRowModel(i) as ServiceOrderModel);
		if (!this.initializationDataProcessed && this.MainAppointmentFilter.InitialSORefNbr.value?.length > 0) {
			return; // wait for the initialization data to be processed
		}
		if (!this.serviceOrders.initializeWith(Array.from(items))) return;
		this.updateSOPane();

		if (this.subscribedToSOBrickPreferences) return;
		this.subscribedToSOBrickPreferences = true;
		this.preferencesService.subscribe(this.serviceOrdersBrickLeftID, {
			applyPreferences: (prefs) => this.applySOBrickLeftPreferences(prefs)
		});
		this.preferencesService.subscribe(this.serviceOrdersBrickRightID, {
			applyPreferences: (prefs) => this.applySOBrickRightPreferences(prefs)
		});
	}

	protected searchAppointmentsDataReadyHandler(grid: QpGridCustomElement, args: QpGridEventArgs) {
		if (!grid.view) return;

		const items = [...Array(grid.rows.length).keys()].filter(i => grid.rows[i]).map(i => grid.view.getRowModel(i) as SearchAppointmentModel);
		if (!this.initializationDataProcessed && this.MainAppointmentFilter.InitialRefNbr.value?.length > 0) {
			return; // wait for the initialization data to be processed
		}
		if (!this.searchAppointments.initializeWith(Array.from(items))) return;
		this.updateSearchAppointmentsPane();

		if (this.subscribedToSearchAppointmentsBrickPreferences) return;
		this.subscribedToSearchAppointmentsBrickPreferences = true;
		this.preferencesService.subscribe(this.searchAppointmentBrickLeftID, {
			applyPreferences: (prefs) => this.applySearchAppointmentsBrickLeftPreferences(prefs)
		});
		this.preferencesService.subscribe(this.searchAppointmentBrickRightID, {
			applyPreferences: (prefs) => this.applySearchAppointmentsBrickRightPreferences(prefs)
		});
	}

	protected createInstance<T>(dict: any, type: { new(): T }): T {
		const instance = new type();
		Object.assign(instance, dict);
		return instance;
	}

	protected async appointmentsDataReadyHandler(grid: QpGridCustomElement, args: QpGridEventArgs) {
		// TODO: It's a temporary kludge to ignore the data after resetting filters
		// A more correct way is to make grid not update itself on switching tabs
		if (grid === this.schedulerGrid && this.ignoreNextSchedulerGridUpdate) {
			this.ignoreNextSchedulerGridUpdate = false;
			return;
		}
		if (!grid.view) return;

		const items = [...Array(grid.rows.length).keys()].map(i => this.createInstance(grid.rows[i].cells, AppointmentEmployeeModel));
		if (grid === this.schedulerGrid) {
			if (!this.appointments.initializeWith(Array.from(items))) return;

			this.calendarContainer?.classList.toggle('qp-disable-filters', false);
			await this.updateCalendar();

			if (!this.subscribedToAppointmentsBrickPreferences) {
				this.subscribedToAppointmentsBrickPreferences = true;
				this.preferencesService.subscribe(this.appointmentsBrickLeftID, {
					applyPreferences: (prefs) => this.applyAppointmentsBrickLeftPreferences(prefs)
				});
				this.preferencesService.subscribe(this.appointmentsBrickRightID, {
					applyPreferences: (prefs) => this.applyAppointmentsBrickRightPreferences(prefs)
				});
			}
		}
		else if (grid === this.lastUpdatedGrid && items.length > 0) {
			this.appointments.mergeDataFrom(Array.from(items));
			await this.updateCalendar();
		}
		await this.scrollSearchResultIntoView(false, false);
	}

	protected async processToolBarClick(e: CustomEvent)
	{
		const btnConfig = e.detail?.config;
		const action = btnConfig?.commandName;
		let res: boolean;
		let promise: Promise<boolean> | Promise<void> | Promise<boolean | void> | null = null;

		// TODO: We don't want events for other targets and the target is incorrect for some actions, so we setup a narrow filter for now
		// (Find out what's wrong with the target for "hours")
		if (action === "refresh" && e.target !== this.appointmentsAllStaffGridElement) return;

		let processed = true;
		switch (action)
		{
			case "add": promise = this.addAppointment(); break;
			// case "rotate": promise = this.switchOrientation(); break;
			case "hours": promise = this.switchHoursFilter(); break;
			case "refresh": promise = this.refreshCalendar(); break;
			case "prevPeriod": promise = this.processPrevPeriod(); break;
			case "nextPeriod": promise = this.processNextPeriod(); break;
			case "periodDay": promise = this.setDayPeriod(); break;
			case "periodWeek": promise = this.setWeekPeriod(); break;
			case "periodMonth": promise = this.setMonthPeriod(); break;
			case "reset": promise = this.resetCreateAppointmentForm(); break;
			case "create": promise = this.createApointmentFromPopup(false, true); break;
			case "confirm":
				promise = this.confirmAppointment(this.popupEntityId.toString(), true)
					.then((res) => { if (res) this.closePopup(); });
				break;
			case "unconfirm":
				promise = this.confirmAppointment(this.popupEntityId.toString(), false)
					.then((res) => { if (res) this.closePopup(); });
				break;
			case "validate":
				promise = this.validateAppointment(this.popupEntityId.toString(), true)
					.then((res) => { if (res) this.closePopup(); });
				break;
			case "invalidate":
				promise = this.validateAppointment(this.popupEntityId.toString(), false)
					.then((res) => { if (res) this.closePopup(); });
				break;
			case "unassign":
				promise = this.unassignResource(this.popupAssignmentId.toString())
					.then((res) => { if (res) this.closePopup(); });
				break;
			case "clone":
				promise = this.cloneAppointment(this.popupEntityId.toString())
					.then(() => { this.closePopup(); });
				break;
			case "delete":
				promise = this.deleteAppointment(this.popupEntityId.toString())
					.then((res) => { if (res) this.closePopup(); });
				break;
			default:
				processed = false;
				break;
		}
		if (processed) {
			e.stopPropagation();
			(<any>e).propagationStopped = true;
		}
		await promise;
	}

	protected async processPrevPeriod()
	{
		let newDate = new Date(this.DatesFilter.DateSelected.value.valueOf());
		switch (this.DatesFilter.PeriodKind.value) {
			case PeriodKind.Day:
				newDate.setDate(newDate.getDate() - 1);
				break;

			case PeriodKind.Week:
				newDate.setDate(newDate.getDate() - 7); // eslint-disable-line @typescript-eslint/no-magic-numbers
				break;

			case PeriodKind.Month:
				newDate = new Date(newDate.getFullYear(), newDate.getMonth() - 1, newDate.getDate());
				break;
		}
		await this.reloadForDate(newDate);
	}

	protected async processNextPeriod()
	{
		let newDate = new Date(this.DatesFilter.DateSelected.value.valueOf());
		switch (this.DatesFilter.PeriodKind.value) {
			case PeriodKind.Day:
				newDate.setDate(newDate.getDate() + 1);
				break;

			case PeriodKind.Week:
				newDate.setDate(newDate.getDate() + 7); // eslint-disable-line @typescript-eslint/no-magic-numbers
				break;

			case PeriodKind.Month:
				newDate = new Date(newDate.getFullYear(), newDate.getMonth() + 1, newDate.getDate());
				break;
		}
		await this.reloadForDate(newDate);
	}

	protected async datePicked()
	{
		if (!this.DatesFilter.DateSelected) return; // not loaded yet, it was not the user who picked the date
		if (this.datePickerReentryPrevention) return;
		try {
			this.datePickerReentryPrevention = true;
			if (this.pickerDate == null
				|| this.pickerDate.withoutTimeInMsec() === this.DatesFilter?.DateSelected?.value?.withoutTimeInMsec()) return;
			await this.reloadForDate(this.pickerDate);
		}
		finally {
			this.datePickerReentryPrevention = false;
		}
	}

	protected async reloadForDate(date: Date, forceSetPeriod = false)
	{
		this.setDateFilterToDate(date, forceSetPeriod);

		const storedSchedulerScroll = this.schedulerControl?.storeScroll();
		//const storedBacklogScroll = this.backlogControl?.storeScroll();
		// await this.schedulerControl?.scrollTo(1, false); // HACK: the control doesn't update without it --- fix??
		// await this.schedulerControl?.scrollVerticallyTo(1, false);
		// await this.backlogControl?.scrollTo(1, false); // HACK: the control doesn't update without it --- fix??
		// await this.backlogControl?.scrollVerticallyTo(1, false);
		// this.schedulerControl?.restoreScroll(storedSchedulerScroll);
		// this.backlogControl?.restoreScroll(storedBacklogScroll);
		await this.screenService.update(undefined, new ScreenUpdateParams({ blockPage: false,
			views: [nameof("AppointmentsAllStaff"), nameof("DatesFilter")]
		}));
		// this.schedulerControl?.restoreScroll(storedSchedulerScroll);
		// this.backlogControl?.restoreScroll(storedBacklogScroll);
		this.schedulerControl?.scrollVerticallyTo((<any>storedSchedulerScroll).scrollTop, false);
	}

	protected setDateFilterToDate(date: Date, forceSetPeriod = false) {
		let newDate = date.clearHoursUTC();
		newDate = newDate.fromView();
		if (forceSetPeriod || newDate.withoutTimeInMsec() !== this.DatesFilter.DateSelected?.value?.withoutTimeInMsec()) {
			this.DatesFilter.DateSelected.updateValue(newDate);
			const [periodStartDate, periodEndDate] = AppointmentsDataHandler.getPeriodStartEndDates(this.DatesFilter);
			this.DatesFilter.DateBegin.updateValue(periodStartDate.fromView());
			this.DatesFilter.DateEnd.updateValue(periodEndDate.fromView());
			this.onDatesFilterUpdated();
		}
	}

	protected onDatesFilterUpdated() {
		// We can't change the styles through the config, as setupViewModel() removes the date picker from the toolbar
		document.querySelector('.qp-sch-period-day')?.classList.toggle('qp-sch-selected-time-period', this.isPeriodDay);
		document.querySelector('.qp-sch-period-week')?.classList.toggle('qp-sch-selected-time-period', this.isPeriodWeek);
		document.querySelector('.qp-sch-period-month')?.classList.toggle('qp-sch-selected-time-period', this.isPeriodMonth);

		this.pickerDate = new Date(this.DatesFilter?.DateSelected?.value?.clearHoursUTC() ?? new Date());
		this.setDatePickerText();
	}

	protected setDatePickerText() {
		const datePickerInput = document.querySelector('.qp-sch-calendar-container .qp-datetime__editor input') as HTMLInputElement;
		const datePickerTextSpan = document.querySelector('.qp-sch-calendar-container .qp-datetime__editor .buttonsCont span') as HTMLSpanElement;
		if (!datePickerTextSpan || !datePickerInput) return;

		const date = this.DatesFilter.DateSelected.value.clearHoursUTC();

		let formattedDate = "";
		switch (this.DatesFilter.PeriodKind.value) {
			case PeriodKind.Day:
				formattedDate = formatDate(date, Formats.DayMonth, dateFormatInfo());
				break;

			case PeriodKind.Week:
				const dateCopy = new Date(date);
				const day = date.getDay();
				const diff = dateCopy.getDate() - day + (day === 0 ? -6 : 1); // eslint-disable-line @typescript-eslint/no-magic-numbers
				const firstDayOfWeek = new Date(dateCopy.setDate(diff));
				const lastDayOfWeek = new Date(dateCopy.setDate(diff + 6)); // eslint-disable-line @typescript-eslint/no-magic-numbers
				const sameMonth = firstDayOfWeek.getMonth() === lastDayOfWeek.getMonth();
				const periodStart = formatDate(firstDayOfWeek, sameMonth ? Formats.DateRangeSameMonthP1 : Formats.DateRangeDiffMonthP1, dateFormatInfo());
				const periodEnd = formatDate(lastDayOfWeek, sameMonth ? Formats.DateRangeSameMonthP2 : Formats.DateRangeDiffMonthP2, dateFormatInfo());
				formattedDate = `${periodStart} - ${periodEnd}`;
				break;

			case PeriodKind.Month:
				formattedDate = formatDate(date, Formats.OnlyMonth, dateFormatInfo());
				break;
		}

		datePickerTextSpan.innerText = formattedDate;
		datePickerInput.style.width = `${datePickerTextSpan.offsetWidth + 20}px`; // eslint-disable-line @typescript-eslint/no-magic-numbers
	}

	protected async switchHoursFilter() {
		this.DatesFilter.FilterBusinessHours.updateValue(!this.DatesFilter.FilterBusinessHours.value);
		this.topBarConfig.hours.config.pushed = !this.DatesFilter.FilterBusinessHours.value;
		this.schedulerGrid.setTopBarButtonsState();
		this.applyHoursFilter();
	}

	protected applyHoursFilter() {
		if ((this.DatesFilter?.FilterBusinessHours?.value ?? true)) {
			this.schedulerControl.timeAxis.filter(t => !this.appointments.hasWorkingCalendarSet() || this.appointments.isWorkingTime(t.startDate, t.endDate));
			// TODO: use this code as a base for the bird-eye view mode
			// this.schedulerControl.timeAxis.forEach(t => {
			// 	//t.startDate = new Date(t.startDate);
			// 	// eslint-disable-next-line @typescript-eslint/no-magic-numbers
			// 	t.startDate.setHours(10);
			// 	//t.endDate = new Date(t.endDate);
			// 	//t.endDate.setDate(t.startDate.getDate());
			// 	// eslint-disable-next-line @typescript-eslint/no-magic-numbers
			// 	t.endDate.setHours(18);
			// });
		}
		else {
			this.schedulerControl.timeAxis.clearFilters();
		}
	}

	protected async refreshCalendar() {
		await this.screenService.update(undefined, new ScreenUpdateParams({ blockPage: false,
			views: [nameof("AppointmentsAllStaff"), nameof("AppointmentFilter")]
		}));
	}

	protected toggleTimeProjection(start: Date, end: Date) {
		// null values turn the projection off
		this.backlogControl.timeRanges[0].startDate = start;
		this.backlogControl.timeRanges[0].endDate = end;
		this.schedulerControl.timeRanges[0].startDate = start;
		this.schedulerControl.timeRanges[0].endDate = end;
	}

	protected getCssClassForOrientation() {
		return (this.orientation === "horizontal") ? "" : "reversed";
	}

	protected getCssClassForPeriodInfo() {
		return (this.DatesFilter?.FilterBusinessHours?.value ?? true) ? "business-hours" : "around-the-clock";
	}

	protected snapToPosition({ resourceRecord, eventRecord, snapTo }) {
		[snapTo.x, snapTo.y] = this.getSnapToResourcePosition(resourceRecord, snapTo.x, snapTo.y, true);
	}

	protected getSnapToResourcePosition(resourceRecord : ResourceModel, x: number, y: number, localCoord = false): [snapX: number, snapY: number] {
		const scheduler = this.schedulerControl;
		const resourceRect = scheduler.getResourceRegion(resourceRecord, null, null);
		if (!resourceRect) {
			return [x, y];
		}
		if (this.orientation === 'horizontal') {
			return snapToGridHorizontal(this.horizontalSnapDistance);
		}
		else {
			return snapToGridVertical(this.verticalSnapDistance);
		}

		function snapToGridHorizontal(distance: number): [snapX: number, snapY: number] {
			const leftOffset = localCoord ? 0 : (scheduler.y + scheduler.headerHeight - scheduler.scrollTop);
			const rowTop = leftOffset + resourceRect.top;
			const normalResHeight = resourceRect.height - 1;
			const snapLine = rowTop + normalResHeight / 2 + scheduler.resourceMargin / 2;
			const distanceFromSnapLine = Math.abs(y + normalResHeight / 2 - snapLine);

			const snapY = (distanceFromSnapLine >= distance) ? y : snapLine - normalResHeight / 2;
			return [x, snapY];
		}

		function snapToGridVertical(distance: number): [snapX: number, snapY: number] {
			const columnWidth = scheduler.resourceColumnWidth;
			const topOffset = localCoord ? 0 : (scheduler.x + scheduler.timeAxisSubGridElement.offsetLeft - scheduler.scrollX);
			const columnLeft = topOffset + resourceRect.left;
			const snapLine = columnLeft + columnWidth / 2 + scheduler.resourceMargin;
			const distanceFromSnapLine = Math.abs(x + columnWidth / 2 - snapLine);

			const snapX = (distanceFromSnapLine >= distance) ? x : snapLine - columnWidth / 2;
			return [snapX, y];
		}
	}

	protected async onEventClick({ eventRecord, eventElement, assignmentRecord }) {
		await this.openPopup(eventRecord.id, assignmentRecord.id, eventElement);
	}

	protected onEventMouseEnter({ eventRecord, event }) {
		this.aureliaEnhance(event.target);
	}

	protected aureliaEnhance(element) {
		if (this.aureliaEnhancedElements.has(element)) return;
		this.aureliaEnhancedElements.add(element);

		const view = this.templatingEngine.enhance({
			element: element,
			container: this.au.container,
			resources: this.au.resources
		});
		view.bind({});
		view.attached();
	}

	protected async onSOGridCellClick({ record, cellElement, target }) {
		console.log(target);
		const serviceOrder = this.serviceOrders.getEntry(record.id);
		await this.openPopup(serviceOrder.SOID.value, null, cellElement, 'view', 'serviceOrder');
	}

	protected async onServiceOrdersScroll({ source, scrollTop }) {
		const items = this.serviceOrdersControl.data;
		const lastIndex = this.serviceOrdersControl.lastVisibleRow.dataIndex;
		if (lastIndex < items.length - 1) return;

		this.serviceOrdersGrid.getMoreRows(0, true, false);
	}

	protected onSOGridCellMouseOver({ record, cellElement, target }) {
		this.aureliaEnhance(cellElement);
	}

	protected async onSearchAppointmentsScroll({ source, scrollTop }) {
		const items = this.searchAppointmentsControl.data;
		const lastIndex = this.searchAppointmentsControl.lastVisibleRow.dataIndex;
		if (lastIndex < items.length - 1) return;

		this.searchAppointmentsGrid.getMoreRows(0, true, false);
	}

	protected onSearchAppointmentsMouseOver({ record, cellElement, target }) {
		this.aureliaEnhance(cellElement);
	}

	protected async onSearchAppointmentGridCellClick({ record, cellElement, target }) {
		const targetElement = target as HTMLElement;
		const searchAppointment = this.searchAppointments.getEntry(record.id);

		let fieldName = targetElement.getAttribute('sch-field');
		if (!fieldName) {
			fieldName = targetElement.parentElement.getAttribute('sch-field');
		}
		if (fieldName === 'SelectedAppointment.RefNbr') {
			await this.openPopup(searchAppointment.appointmentID, searchAppointment.assignmentID, cellElement);
			return;
		}
		await this.selectSearchAppointment(searchAppointment);
	}

	protected async selectSearchAppointment(searchAppointment: SearchAppointmentModel) {
		if (this.currentSearchAppointment?.appointmentID === searchAppointment.appointmentID) {
			// Clear search selection
			this.currentSearchAppointment = null;
			this.updateSearchAppointmentsPane();
			await this.updateCalendar();
			return;
		}

		this.currentSearchAppointment = searchAppointment;
		this.updateSearchAppointmentsPane();

		// await this.openPopup(searchAppointment.appointmentID, cellElement);
		if (await this.scrollSearchResultIntoView()) return;

		// Load from server
		this.setDateFilterToDate(searchAppointment.ScheduledDateTimeBegin.value, true);
		this.AppointmentFilter.SearchAppointmentID.updateValue(Number(searchAppointment.appointmentID));
		this.MainAppointmentFilter.ResetFilters.updateValue(false);

		let res:ICommandUpdateResult;
		try {
			res = await this.screenService.update(undefined, new ScreenUpdateParams({ blockPage: false,
				views: [nameof("AppointmentsAllStaff"), nameof("AppointmentFilter")]
			}));
		}
		finally {
			//
		}
		if (!res?.succeeded) return;
		await this.scrollSearchResultIntoView(false, false);

		if (this.schedulerGrid.getActiveFilter().filterID === this.allRecordsFilter) return;

		await this.screenService.update(undefined, new ScreenUpdateParams({ blockPage: false, views: [nameof("MainAppointmentFilter")] }));
		const resetFilters = this.MainAppointmentFilter.ResetFilters.value;
		if (resetFilters) {
			this.schedulerGrid.onFilterApplied("", this.allRecordsFilter);
			const calendarContaier = document.querySelector('.qp-sch-calendar-container .qp-datetime__editor input') as HTMLInputElement;
			this.calendarContainer.classList.toggle('qp-disable-filters', true);
			this.ignoreNextSchedulerGridUpdate = true;
		}
	}

	protected async scrollSearchResultIntoView(needRedraw = true, needAnimation = true) {
		const appointment = this.currentSearchAppointment;
		if (!appointment) return false;

		if (appointment.StaffCntr.value === 0) {
			const loadedAppointment = this.appointments.getUnassignedAppointment(appointment.appointmentID);
			if (loadedAppointment && !loadedAppointment.isFilteredOut) {
				if (needRedraw) {
					await this.updateCalendar();
				}
				const controlEvent = this.backlogControl.eventStore.getById(appointment.appointmentID) as SchedulerEventModel;
				await this.backlogControl.scrollEventIntoView(controlEvent, { animate: needAnimation, block: 'start', edgeOffset: 20 });
				return true;
			}
		}
		else {
			const loadedAssignments = this.appointments.getAssignmentsByAppointmentId(appointment.appointmentID);
			if (loadedAssignments.length === appointment.StaffCntr.value && !loadedAssignments.some(x => x.isFilteredOut)) {
				if (needRedraw) {
					await this.updateCalendar();
				}
				const assignmentId = this.appointments.getFullId(loadedAssignments[0].assignmentID);
				const assignment = this.schedulerControl.assignmentStore.getById(assignmentId) as SchedulerAssignmentModel;
				await this.schedulerControl.scrollAssignmentIntoView(assignment, { animate: needAnimation, block: 'start', edgeOffset: 20 });
				await this.schedulerControl.scrollResourceEventIntoView(assignment.resource as SchedulerResourceModel, assignment.event as SchedulerEventModel, { animate: needAnimation, block: 'start', edgeOffset: 20 });

				const element = this.schedulerControl.getElementFromAssignmentRecord(assignment);
				if (element?.parentElement?.getAttribute('data-assignment-id') !== assignmentId) {
					// TODO: That's a kludge for Bryntum's bug -- we should've had the assignment's element exist
					// instead, it might not exist or exist with a null assignment ID
					console.log(`assignment not found, going to try another method`);
					await this.updateCalendar();
					const assignment = this.schedulerControl.assignmentStore.getById(assignmentId) as SchedulerAssignmentModel;
					await this.schedulerControl.scrollResourceEventIntoView(assignment.resource as SchedulerResourceModel, assignment.event as SchedulerEventModel, { animate: needAnimation, block: 'start', edgeOffset: 20 });
				}
				return true;
			}
		}
		return false;
	}

	protected async openPopup(eventId: string, assignmentId: string, eventElement: HTMLElement, mode: 'view' | 'edit' = 'view', kind: 'appointment' | 'serviceOrder' = 'appointment') {
		if (this.popupEntityId === eventId) {
			this.eventPopup.close();
			this.popupEntityId = undefined;
			return;
		}
		this.popupEntityId = eventId;
		this.popupAssignmentId = assignmentId;

		const popupInner = (mode === 'edit') ?  this.editPopupInner
			: ((kind === 'appointment') ? this.appointmentPopupInner : this.serviceOrderPopupInner);
		const popupInnerHolder = (mode === 'edit') ? this.editPopupInnerHolder
			: ((kind === 'appointment') ? this.appointmentPopupInnerHolder : this.serviceOrderPopupInnerHolder);

		let neededViews: string[];
		if (popupInner === this.appointmentPopupInner) {
			neededViews = [nameof("AppointmentFilter"), nameof("SelectedAppointment"), nameof("SelectedSO"), nameof("SelectedAppointmentEmployees")];
		}
		else if (popupInner === this.serviceOrderPopupInner) {
			neededViews = [nameof("SOFilter"), nameof("SelectedSO"), nameof("SelectedSOEmployees")];
		}
		else if (popupInner === this.editPopupInner) {
			neededViews = [nameof("MainAppointmentFilter"), nameof("EditedAppointmentEmployees")];
		}

		this.SelectedAppointment.ScheduledDateTimeBegin.displayMask = "g"; // TODO: it doesn't work ATM

		const rect = popupInner.getBoundingClientRect();
		this.eventPopup = new Popup({
			forElement: eventElement,
			align: 'l-r',
			cls: `qp-sch-popup ${mode === 'view' ? "qp-sch-view-popup" : "qp-sch-edit-popup"}`,
			autoShow: true,
			autoClose: false,
			monitorResize: true,
			scrollAction: 'realign',
			onBeforeHide: () => {
				popupInnerHolder.appendChild(popupInner);
				this.popupEntityId = 0;
				if (this.createdEventRecord != null) {
					this.schedulerControl.eventStore.remove(this.createdEventRecord);
					this.backlogControl.eventStore.remove(this.createdEventRecord);
				}
				//this.eventPopup.destroy();
			},
			hideWhenEmpty: false,
			floating: true,
			anchor: true,
			// eslint-disable-next-line @typescript-eslint/no-magic-numbers
			html: `
				<div class="event-info" style="width:${rect.width}px; height:${rect.height}px">
					<div class="splash loading"></div>
				</div>`,
		});

		document.getElementById('page-caption').onpointerdown = (event) => { this.closePopup(event); };
		document.getElementById('customizationMenu_menu').onpointerdown = (event) => { event.stopPropagation(); };

		if (mode === 'edit') {
			this.initCreateAppointmentForm();
		}
		else if (kind === 'appointment') {
			const appointmentId = this.appointments.getPartialId(eventId);
			const appointment = this.appointments.getAppointment(eventId);
			let sOId = appointment?.SchedulerServiceOrder__SOID.value;
			if (!appointment) {
				const searchRes = this.searchAppointments.getEntry(appointmentId);
				sOId = searchRes?.SchedulerServiceOrder__SOID.value;
			}
			this.AppointmentFilter.AppointmentID.updateValue(Number(appointmentId));
			this.SOFilter.SOID.updateValue(Number(sOId));
		}
		else if (kind === 'serviceOrder') {
			this.SOFilter.SOID.updateValue(Number(eventId));
		}

		let res:ICommandUpdateResult;
		try {
			res = await this.screenService.update(undefined, new ScreenUpdateParams({ blockPage: false, views: neededViews }));
		}
		finally {
			this.AppointmentFilter.AppointmentID.updateValue(0);
			this.SOFilter.SOID.updateValue(0);
		}

		this.defaultSrvOrdType ??= this.MainAppointmentFilter.SrvOrdType?.value?.id;

		const popupOuter = this.eventPopup.element.getElementsByClassName('event-info')[0] as HTMLElement;
		if (!res?.succeeded) {
			popupOuter.innerHTML = Messages.ErrorLoadingData;
			return;
		}

		popupOuter.innerHTML = "";
		popupOuter.appendChild(popupInner);

		if (this.popupSizeObserver != null) {
			this.popupSizeObserver.disconnect();
		}
		this.popupSizeObserver = new ResizeObserver((entries, observer) => {
			const rect = popupInner.getBoundingClientRect();
			popupOuter.style.width = `${rect.width}px`;
			popupOuter.style.height = `${rect.height}px`;
			this.eventPopup.element.style.height = ``;
			console.log(`new height: ${rect.height}`);
		});
		this.popupSizeObserver.observe(popupInner);

		if (popupInner === this.appointmentPopupInner) {
			const noEmployees = this.SelectedAppointmentEmployees.getRowModel(0) === undefined;
			this.topBarAppointmentConfig.items.unassign.config.hidden = noEmployees || this.SelectedAppointment.isLocked;
			this.topBarAppointmentConfig.items.confirm.config.hidden = !this.SelectedAppointment.canConfirm;
			this.topBarAppointmentConfig.items.unconfirm.config.hidden = !this.SelectedAppointment.canUnconfirm;
			this.topBarAppointmentConfig.items.validate.config.hidden = !this.SelectedAppointment.canValidate;
			this.topBarAppointmentConfig.items.invalidate.config.hidden = !this.SelectedAppointment.canInvalidate;
			this.topBarAppointmentConfig.items.delete.config.hidden = this.SelectedAppointment.isLocked;
			this.appointmentToolBar.setupViewModel();

			const employeeFieldset = document.querySelector('#groupAppointmentPopupEmployeesID') as HTMLElement;
			this.appointmentPopupEmployeesForm?.classList.toggle('hidden', noEmployees);
			employeeFieldset?.classList.toggle('hidden', noEmployees);

		}
		if (popupInner === this.editPopupInner) {
			const employeeFieldset = document.querySelector('#groupEditedAppointmentPopupEmployeesID') as HTMLElement;
			const noEmployees = (this.dragCreateResourceIDsSet?.size ?? 0) === 0;
			this.editedAppointmentPopupEmployeesForm?.classList.toggle('hidden', noEmployees);
			employeeFieldset?.classList.toggle('hidden', noEmployees);
		}
		if (popupInner === this.serviceOrderPopupInner) {
			const employeeFieldset = document.querySelector('#groupSOPopupEmployeesID') as HTMLElement;
		}
	}

	protected async resetCreateAppointmentForm() {
		const fields = Object.keys(this.MainAppointmentFilter).filter(x => !!(this.MainAppointmentFilter[x] as PXFieldState));
		fields.forEach(field => this.screenService.setScreenParameter(nameof("MainAppointmentFilter"), field, null));
		this.MainAppointmentFilter.SrvOrdType.updateValue(this.defaultSrvOrdType || null);

		this.initCreateAppointmentForm();
		// TODO: replace html element with an array of views
		const res = await this.screenService.update(undefined, new ScreenUpdateParams({ blockPage: false }, this.editPopupInner));
	}

	protected initCreateAppointmentForm() {
		if (this.createdEventRecord) {
			const startDate = (this.createdEventRecord.startDate as Date)?.fromView();
			this.MainAppointmentFilter.ScheduledDateTimeBegin_Date.updateValue(startDate);
			this.MainAppointmentFilter.ScheduledDateTimeBegin_Time.updateValue(startDate);
			this.MainAppointmentFilter.Duration.updateValue(TimeConverter.minutesTohMM(this.createdEventRecord.durationMS / 1000 / 60)); 	// eslint-disable-line @typescript-eslint/no-magic-numbers
			this.MainAppointmentFilter.Resources.updateValue([...this.dragCreateResourceIDsSet].join(","));
		}
		this.MainAppointmentFilter.SODetID.updateValue(0);
		console.log(`initCreateAppointmentForm: ${this.MainAppointmentFilter.Resources.value}`);
	}

	protected setupObservers() {
		[ this.MainAppointmentFilter.SrvOrdType,
			this.MainAppointmentFilter.SORefNbr,
			this.MainAppointmentFilter.CustomerID,
			this.MainAppointmentFilter.LocationID,
			this.MainAppointmentFilter.ContactID
		].forEach(field => {
			this.observeEditPanelChange(field);
		});
	}

	protected setupPopups() {
		setupAppViewPopup(this.appointmentPopupInner, "appointment");
		setupAppViewPopup(this.editPopupInner, "edit");
		setupAppViewPopup(this.serviceOrderPopupInner, "serviceOrder");

		function setupAppViewPopup(popupInner: HTMLElement, idPrefix: string) {
			const collapseButton = document.querySelector(`#${idPrefix}PopupForm_toggle`) as HTMLElement;
			const headerButtons = popupInner.querySelector('.qp-dialog-header-buttons') as HTMLElement;
			const firstHeaderButton = headerButtons?.querySelector('qp-icon:first-child') as HTMLElement;

			// TODO: Fix collapse butons
			return;

			if (firstHeaderButton) {
				headerButtons.insertBefore(collapseButton, firstHeaderButton);
			}
			else {
				headerButtons.appendChild(collapseButton);
			}
		}
	}

	protected observeEditPanelChange(obj: object) {
		this.bindingEngine.propertyObserver(obj, "value").subscribe((newValue: any, oldValue: any) => {
			if (!newValue || newValue?.id === oldValue?.id) return;
			if (this.updatingObservedItem) return;
			if (!this.popupEntityId) return; // the update came not from UI, ignore

			this.updatingObservedItem = true;
			// TODO: replace html element with an array of views
			this.screenService.update(undefined, new ScreenUpdateParams({ blockPage: false }, this.editPopupInner))
				.finally(() => { this.updatingObservedItem = false; });
		});
	}

	protected closePopup(event: PointerEvent = null) {
		if (!this.popupEntityId) return true;
		const targetClassList = (event?.target as HTMLElement)?.classList;
		if (targetClassList?.contains('qp-menu-spacer') || targetClassList?.contains('qp-menu-item')) {
			event.stopPropagation();
			return true;
		}

		this.eventPopup.close();
		this.popupEntityId = undefined;
		return true;
	}

	protected async onSelectionChange() {
		if (this.mapSplitter.getSplitterState() === 'collapsed-second') return;
		this.updateMapController();
	}

	protected mapSplitterStateChanged() {
		if (this.mapSplitter.getSplitterState() === 'collapsed-second') return;
		this.updateMapController();
	}

	protected async updateMapController() {
		// TODO: get rid of this ignore rule (#AC-317449)
		// eslint-disable-next-line @typescript-eslint/no-misused-promises
		const mapController = new MapController(this.mainMap, this.schedulerControl, this.appointments, this.openPopup.bind(this));
		await mapController.updateMap();
	}

	protected async onSubGridCollapse({ source, subGrid }) {
		if (subGrid === this.lockedSubGrid || subGrid === this.lockedSubGridBacklog) {
			this.splitterPreferences.state = 'collapsed-first';
		}
		else if (subGrid === this.normalSubGrid || subGrid === this.normalSubGridBacklog) {
			this.splitterPreferences.state = 'collapsed-second';
		}
		await this.savePreferences();
	}

	protected async onSubGridExpand({ source, subGrid }) {
		this.splitterPreferences.state = 'normal';
		await this.savePreferences();
	}

	protected async onLockedSubGridResize({ width }) {
		const movingElement = document.getElementsByClassName('b-moving')?.[0] as HTMLElement;
		if (!movingElement) return; // fake event

		if (this.saveSplitterPositionTimer) {
			clearTimeout(this.saveSplitterPositionTimer);
		}
		this.saveSplitterPositionTimer = setTimeout(() => {
			this.splitterPreferences.position = width;
			this.savePreferences();
		}, 300); // eslint-disable-line @typescript-eslint/no-magic-numbers
	}

	protected onVisibleDateRangeChange() {
		if (this.mapSplitter.getSplitterState() === 'collapsed-second') return;
		if (this.updateMapTimer) {
			clearTimeout(this.updateMapTimer);
		}
		this.updateMapTimer = setTimeout(() => {
			const mapController = new MapController(this.mainMap, this.schedulerControl, this.appointments, this.openPopup.bind(this));
			mapController.updateMap();
		}, 300); // eslint-disable-line @typescript-eslint/no-magic-numbers
	}

	protected onDragCreateStart({ source, eventRecord, resourceRecord, eventElement }) {
		const eventResourceId = eventRecord.resourceId.toString();
		if (eventResourceId === AppointmentEmployeeModel.unassignedId) {
			this.dragCreateResourceIDsSet = new Set();
			return;
		}

		this.dragCreateResourceIDsSet = new Set((this.schedulerControl.selectedRows as SchedulerResourceModel[])?.map(r => r.id.toString()));
		this.dragCreateResourceIDsSet.add(eventResourceId);

		this.dragCreateResourceIDsSet.forEach((resourceID) => {
			if (resourceID === eventResourceId) return;
			this.schedulerControl.eventStore.assignEventToResource(eventRecord, resourceID);
		});
	}

	protected onDragCreateEnd({ source, eventRecord, resourceRecord, eventElement }) {
		this.createdEventRecord = eventRecord;
		this.openPopup("new", null, eventElement, 'edit');
		return false;
	}

	protected onBeforeEventDrag({ source, eventRecord, resourceRecord }) {
		this.toggleTimeProjection(eventRecord.startDate, eventRecord.endDate);
	}

	protected onEventDragReset({ source, eventRecord, resourceRecord }) {
		this.toggleTimeProjection(null, null);
	}

	protected async onTimeAxisHeaderClick({ startDate, endDate }) {
		await this.schedulerControl.scrollToDate(startDate,  { animate: true, block: 'start', edgeOffset: 0 });
	}

	protected attachMouseEventHandlers() {
		this.schedulerControl.element.addEventListener("mousemove", (e) => {
			const element = document.elementFromPoint(e.pageX, e.pageY) as HTMLElement;
			const resourceAtCursor = this.schedulerControl.resolveResourceRecord(element);
			this.hightlightResource(resourceAtCursor);
		}, false);

		this.schedulerControl.element.addEventListener("mouseout", (e) => {
			this.clearResourceHightlight(this.prevHoveredResource);
		}, false);
	}

	protected hightlightResource(resourceAtCursor: SchedulerResourceModel) {
		if (this.prevHoveredResource?.id !== resourceAtCursor?.id) {
			this.clearResourceHightlight(this.prevHoveredResource);
		}
		if (!resourceAtCursor?.id) return;
		const element = this.getResourceElement(resourceAtCursor);
		element?.setAttribute('qp-sch-dragged-over', 'true');
		this.prevHoveredResource = resourceAtCursor;
	}

	protected clearResourceHightlight(prevHoveredResource: SchedulerResourceModel) {
		if (!prevHoveredResource?.id) return;
		const element = this.getResourceElement(prevHoveredResource);
		element?.removeAttribute('qp-sch-dragged-over');
		this.prevHoveredResource = null;
	}

	protected getResourceElement(resource: SchedulerResourceModel) {
		return document.querySelector<HTMLElement>(`.b-grid-row[data-id="${resource.id}"]`);
	}

	protected unassignedRenderer({ eventRecord, resourceRecord, renderData }) {
		const appointment = this.appointments.getUnassignedAppointment(eventRecord.id);
		return this.renderAppointmentWithStatusBar(appointment, renderData);
	}

	protected assignmentRenderer({ eventRecord, resourceRecord, renderData }) {
		// We need to be able to render the following types of objects:
		// - Event in calendar (assignment)
		//          assignment = renderData.assignment
		// - Projection of a new assignment: Resource dragged to an existing appointment
		//          existing appointment (where?)
		// - Projection of a new assignment: External object dragged onto a Resource
		//          external object - from backlog
		//          external object - from SO / SODet
		// - New appointment (being created via drag-creating)

		// If cases of New appointment or Resource, we override the standard rendering

		const assignment = this.appointments.getAssignment(renderData.assignment.id);
		const eventInfo = eventRecord.get('eventInfo') as DragHelperEventInfo;
		const entity = assignment ?? eventInfo?.entity;
		return this.renderAppointmentWithStatusBar(entity, renderData);
	}

	protected renderDraggedServiceOrder(entity: DraggableEntity, detailOnly = false) {
		const statusBar = '<div class="qp-sch-event-empty-status-bar"></div>';
		const appointmentHtml = this.getAppointmentHtml(entity, detailOnly);
		return `${statusBar}${appointmentHtml}`;
	}

	protected renderAppointmentWithStatusBar(entity: DraggableEntity, renderData: Object = null, useMarks = false) {
		const appointment = (entity instanceof AppointmentEmployeeModel || entity instanceof SearchAppointmentModel) ? entity : null;
		const statusColor = appointment?.bandColor ?? '#000000'; // TODO: black is hardcoded as not visible
		const whiteStripe = '#FFFFFFB0';
		const isFilteredOut = (entity as AppointmentEmployeeModel)?.isFilteredOut;
		const statusBar = (statusColor === '#000000' || isFilteredOut) ? '<div class="qp-sch-event-empty-status-bar"></div>' : `
			<div class="qp-sch-event-status-bar" style=
				"background: repeating-linear-gradient(90deg, ${statusColor}, ${statusColor} 9px, ${whiteStripe} 9px, ${whiteStripe} 10px, transparent 10px, transparent);">
			</div>
			`;
		const appointmentHtml = entity ? this.getAppointmentHtml(entity, false, useMarks, isFilteredOut) : this.getNewAppointmentHtml();
		const searchedID = this.currentSearchAppointment?.appointmentID;
		if (searchedID && appointment) {
			(<any>renderData)?.cls.add(searchedID === appointment.appointmentID
				? "qp-sch-search-highlight-event"
				: "qp-sch-search-dimmed-event");
		}
		return `${statusBar}${appointmentHtml}`;
	}

	protected getAppointmentBrickHeight() {
		const fieldsLeft = getRegularFields(this.appointmentsBrickLeft);
		const fieldsRight = getRegularFields(this.appointmentsBrickRight);
		// eslint-disable-next-line @typescript-eslint/no-magic-numbers
		return Math.max(fieldsLeft.length, fieldsRight.length) * 16 + 22;

		function getRegularFields(fieldset: QpFieldsetCustomElement) {
			return fieldset.Fields
				?.filter(x => fieldset.getFieldVisibility(true, x))
				?.filter(x => !['Priority', 'Severity', 'StaffCntr'].includes(x.name)) ?? [];
		}
	}

	protected getServiceOrderHtml(entity: ServiceOrderModel, detailOnly = false) {
		const templateLeft = this.serviceOrdersBrickLeft;
		const templateRight = this.serviceOrdersBrickRight;
		const fieldsText = this.getEntityHtmlPart(templateLeft, templateRight, (field) =>  this.getServiceOrderFieldHtml(field, entity, detailOnly));
		return `<div class="qp-sch-event-info">${fieldsText}</div>`;
	}

	protected getEntityHtmlPart(templateLeft: QpFieldsetCustomElement, templateRight: QpFieldsetCustomElement,
		getFieldHtml: (entity: DataFieldDescriptor) => string)
	{
		const fieldsLeft = templateLeft.Fields?.filter(x => templateLeft.getFieldVisibility(true, x));
		const fieldsRight = templateRight.Fields?.filter(x => templateRight.getFieldVisibility(true, x));
		if (!fieldsLeft?.length) return "";

		const leftHtmls = fieldsLeft.map(field => getFieldHtml(field));
		const rightHtmls = fieldsRight.map(field => getFieldHtml(field));

		let htmlPart = "";
		let leftFieldNum = 0;
		let rightFieldNum = 0;
		while (leftFieldNum < fieldsLeft.length || rightFieldNum < fieldsRight.length) {
			let [leftAddOn, rightAddOn] = ["", ""];
			leftAddOn = leftFieldNum < fieldsLeft.length ? leftHtmls[leftFieldNum] : null;
			leftFieldNum ++;

			let rightHtml = "";
			let nextRightHtml = "";
			let iconsNum = 0;
			do {
				rightHtml = rightFieldNum < fieldsRight.length ? rightHtmls[rightFieldNum] : null;
				nextRightHtml = rightFieldNum + 1 < fieldsRight.length ? rightHtmls[rightFieldNum + 1] : null;
				rightAddOn += rightHtml ?? "";
				rightFieldNum ++;
				iconsNum += rightHtml?.startsWith("<span><svg") ? 1 : 0;
			} while (nextRightHtml != null && (rightAddOn === "" || rightAddOn?.startsWith("<span><svg") && (nextRightHtml?.startsWith("<span><svg") || nextRightHtml === "")));

			const rightPart = !rightAddOn ? ""
				: !rightAddOn?.startsWith('<span><svg') ? `<span>${rightAddOn}</span>`
					// eslint-disable-next-line @typescript-eslint/no-magic-numbers
					: `	<div class="qp-sch-tile-icon" style="max-width: max(0px, calc((100% - ${iconsNum * 16 + Math.max(iconsNum - 1, 0) * 6 + 10}px)*999));">
							<span></span>
							<span>${rightAddOn}</span>
						</div>`;

			htmlPart += `<div>${leftAddOn}${rightPart}</div>`;
		}

		return htmlPart;
	}


	protected getServiceOrderFieldHtml(field: DataFieldDescriptor, entity: ServiceOrderModel, detailOnly) {
		let cellText = "";
		let value: any = null;
		let isIcon = false;

		if (field.name === 'RefNbr') {
			cellText = detailOnly ? entity.serviceId : entity.orderId;
		}
		else if (field.viewName === nameof("SelectedSO")) {
			value = entity[field.name];
			isIcon = ['Priority', 'Severity'].includes(field.name);
		}
		const fieldFullName = `${field.viewName}.${field.name}`;
		const fieldIDs = `sch-field="${fieldFullName}" sch-value="${value?.value ?? cellText}"`;

		if (isIcon) {
			const iconUrl = this.getIconUrl(field.name, value?.cellText);
			if (!iconUrl) return "";
			const tooltip = ['Priority', 'Severity'].includes(field.name) ? `<qp-tooltip>${field.name}: ${value?.cellText}</qp-tooltip>` : "";
			return `<span><svg class="qp-sch-tile-icon" ${fieldIDs}> <use href="${iconUrl}"></use></svg>${tooltip}</span>`;
		}

		cellText = cellText || (value?.cellText ?? "");
		const escapedValue = StringHelper.xss `${cellText}`;
		const valueWithMarks = this.applyMarks(escapedValue, this.serviceOrdersFastFilter?.value);
		return `<span ${fieldIDs}>${valueWithMarks}</span>`;
	}

	protected getAppointmentHtml(entity: DraggableEntity, detailOnly = false, useMarks = false, isFilteredOut = false) {
		const templateLeft = (entity instanceof SearchAppointmentModel) ? this.searchAppointmentBrickLeft : this.appointmentsBrickLeft;
		const templateRight = (entity instanceof SearchAppointmentModel) ? this.searchAppointmentBrickRight : this.appointmentsBrickRight;
		const fieldsText = this.getEntityHtmlPart(templateLeft, templateRight, (field) =>  this.getAppointmentFieldHtml(field, entity, useMarks, detailOnly, isFilteredOut));

		return `<div class="qp-sch-event-info">${fieldsText}</div>`;
	}

	protected getAppointmentFieldHtml(field: DataFieldDescriptor, entity: DraggableEntity, useMarks, detailOnly, isFilteredOut = false) {
		const isAppointment = entity instanceof AppointmentEmployeeModel || entity instanceof SearchAppointmentModel;
		const darkBackground = isAppointment && !(entity.isConfirmed || entity.isValidatedByDispatcher);
		let cellText = "";
		let value: any = null;
		let isIcon = false;
		let colorStyle = "";
		if (field.name === 'RefNbr') {
			cellText = detailOnly ? (entity as ServiceOrderModel).serviceId : entity.caption;
		}
		else {switch (field.viewName) {
			case nameof("SelectedAppointment"): {
				value = entity[`SchedulerAppointment__${field.name}`]
					?? entity[`FSSODet__${field.name}`]
					?? entity[field.name];
				if (field.name === 'Status' && isAppointment && entity.bandColor !== '#000000') {
					colorStyle = entity.bandColor ?? "";
					colorStyle = colorStyle !== "" ? `style="color: ${colorStyle};"` : "";
				}
				isIcon = field.name === 'StaffCntr';
				break;
			}
			case nameof("SelectedSO"): {
				value = entity[`SchedulerServiceOrder__${field.name}`]
					?? entity[field.name];
				isIcon = ['Priority', 'Severity'].includes(field.name);
				break;
			}
		}}
		const fieldFullName = `${field.viewName}.${field.name}`;
		const fieldIDs = `sch-field="${fieldFullName}" sch-value="${value?.value ?? cellText}"`;

		if (isIcon) {
			const iconUrl = this.getIconUrl(field.name, value?.cellText, darkBackground);
			if (!iconUrl) return "";
			const tooltip = ['Priority', 'Severity'].includes(field.name) ? `<qp-tooltip>${field.name}: ${value?.cellText}</qp-tooltip>` : "";
			return `<span><svg class="qp-sch-tile-icon" ${fieldIDs}> <use href="${iconUrl}"></use></svg>${tooltip}</span>`;
		}

		cellText = cellText || (value?.cellText ?? "");
		const escapedValue = StringHelper.xss `${cellText}`;
		const valueWithMarks = useMarks ? this.applyMarks(escapedValue, this.searchAppointmentsFastFilter?.value) : escapedValue;
		const renderedValue = isFilteredOut ? "" : valueWithMarks;
		const linkClass = (fieldFullName === 'SelectedAppointment.RefNbr') ? "class='qp-sch-popup-link'" : "";
		const fieldSpan = `<span ${colorStyle} ${linkClass} ${fieldIDs}"><span>${renderedValue}</span></span>`;
		return fieldSpan;
	}

	protected getIconUrl(field: string, value: string, darkBackground = false) {
		switch (field) {
			case 'Priority': {
				switch (value) {
					case 'High': return darkBackground ? this.iconPriorityDarkHigh.SvgUrl : this.iconPriorityHigh.SvgUrl;
					case 'Medium': return darkBackground ? this.iconPriorityDarkMedium.SvgUrl : this.iconPriorityMedium.SvgUrl;
					case 'Low': return darkBackground ? this.iconPriorityDarkLow.SvgUrl : this.iconPriorityLow.SvgUrl;
				}
			}
			case 'Severity': {
				switch (value) {
					case 'High': return darkBackground ? this.iconSeverityDarkHigh.SvgUrl : this.iconSeverityHigh.SvgUrl;
					case 'Medium': return darkBackground ? this.iconSeverityDarkMedium.SvgUrl : this.iconSeverityMedium.SvgUrl;
					case 'Low': return darkBackground ? this.iconSeverityDarkLow.SvgUrl : this.iconSeverityLow.SvgUrl;
				}
			}
			case 'StaffCntr':
				if (Number(value) > 1) {
					return darkBackground ? this.iconMultiEmployeeDark.SvgUrl : this.iconMultiEmployee.SvgUrl;
				}
		}
		return "";
	}

	protected getNewAppointmentHtml() {
		return StringHelper.xss`
			<div class="qp-sch-event-info"><div>
			<b>${Labels.NewAppointment}</b><br>
			</div></div>
			`;
	}

	protected applyMarks(field: string, search: string) {
		if (!search) return field;

		const escapedSearch = search.replace(/[-\/\\^$*+?.()|[\]{}]/g, '\\$&');
		const regExp = new RegExp(escapedSearch, 'gi');
		return field.replace(regExp, (match) => `<mark>${match}</mark>`);
	}

	protected getDataItemFromMenuCommand(record, eventRecord, resourceRecord, assignmentRecord): AppointmentEmployeeModel | SearchAppointmentModel {
		if (record) {
			return this.searchAppointments.getEntry(record.id);
		}

		return (resourceRecord.id !== AppointmentEmployeeModel.unassignedId)
			? this.appointments.getAssignment(assignmentRecord.id)
			: this.appointments.getUnassignedAppointment(eventRecord.id);
	}

	protected getMenuItems() {
		return {
			copyEvent: false,
			cutEvent: false,
			splitEvent: false,
			removeRow: false,
			editEvent: { text: Labels.EditAppointment, onItem: this.onEditAppointment.bind(this), cls: 'qp-sch-menu', icon: "" },
			deleteEvent: { text: Labels.DeleteAppointment, onItem: this.onDeleteAppointment.bind(this), cls: 'qp-sch-menu', icon: "" },
			cloneEvent: { text: Labels.CloneAppointment, onItem: this.onCloneAppointment.bind(this), cls: 'qp-sch-menu', icon: "" },
			unassignEvent: { text: Labels.Unassign, onItem: this.onUnassignResource.bind(this), cls: 'qp-sch-menu', icon: "" },
		};
	}

	protected processMenuItems({record, eventRecord, resourceRecord, assignmentRecord, items}) {
		const item = this.getDataItemFromMenuCommand(record, eventRecord, resourceRecord, assignmentRecord);

		items.editEvent.text = item.isLocked ? Labels.ViewAppointment : Labels.EditAppointment;
		items.validate = item.isLocked ? false : { text: item.isValidatedByDispatcher ? Labels.ClearValidation : Labels.ValidateByDispatcher,
			onItem: this.onValidateAppointment.bind(this), cls: 'qp-sch-menu' };
		items.confirm = item.isLocked ? false : { text: item.isConfirmed ? Labels.Unconfirm : Labels.Confirm,
			onItem: this.onConfirmAppointment.bind(this), cls: 'qp-sch-menu' };
		items.unassignEvent.hidden = item instanceof SearchAppointmentModel
			|| item.isLocked || item.resourceId === AppointmentEmployeeModel.unassignedId;
		items.deleteEvent.hidden = item.isLocked;
	}

	protected async addAppointment() {
		this.dragCreateResourceIDsSet = new Set();
		this.openPopup("new", null, null, 'edit');
	}

	protected async onEditAppointment({ record, eventRecord, resourceRecord, assignmentRecord }) {
		const item = this.getDataItemFromMenuCommand(record, eventRecord, resourceRecord, assignmentRecord);

		this.AppointmentFilter.AppointmentID.updateValue(Number(item.appointmentID));
		let res:ICommandUpdateResult;
		try {
			// Sync the service order type
			res = await this.screenService.update(undefined, new ScreenUpdateParams({ blockPage: false, views: [nameof("SelectedAppointment")] }));
		}
		finally {
			this.AppointmentFilter.AppointmentID.updateValue(0);
		}

		this.apiClient.openAppointmentEditor(this.screenID, this.SelectedAppointment.RefNbr.viewName, item.refNbr);
	}

	protected async onCloneAppointment({ record, eventRecord, resourceRecord, assignmentRecord }) {
		const item = this.getDataItemFromMenuCommand(record, eventRecord, resourceRecord, assignmentRecord);
		await this.cloneAppointment(item.appointmentID);
	}

	protected async cloneAppointment(appointmentID: string) {
		const appointment = this.getAppointment(appointmentID);
		this.MainAppointmentFilter.RefNbr.updateValue(appointment.refNbr);
		this.MainAppointmentFilter.SrvOrdType.updateValue(appointment.srvOrdType);
		try {
			const res = await this.screenService.update("CloneAppointment", new ScreenUpdateParams({ blockPage: false }, this.mainTabBarHolder));
			return res.succeeded;
		}
		catch (error) {
			return false;
		}
	}

	protected async onDeleteAppointment({ record, eventRecord, resourceRecord, assignmentRecord }) {
		const item = this.getDataItemFromMenuCommand(record, eventRecord, resourceRecord, assignmentRecord);
		return await this.deleteAppointment(item.appointmentID);
	}

	protected async deleteAppointment(appointmentID: string) {
		this.UpdatedAppointment.AppointmentID.updateValue(Number(appointmentID));

		try {
			// TODO: replace html element with an array of views
			const res = await this.screenService.update("DeleteAppointment", new ScreenUpdateParams({ blockPage: false }, this.mainTabBarHolder));
			if (res.succeeded) {
				this.appointments.removeAppointment(appointmentID);
				await this.updateCalendar();
			}
			return res.succeeded;
		}
		catch (error) {
			return false;
		}
	}

	protected async onUnassignResource({ eventRecord, resourceRecord, assignmentRecord }) {
		return await this.unassignResource(assignmentRecord.id);
	}

	protected async unassignResource(assignmentID: string) {
		const assignment = this.appointments.getAssignment(assignmentID);
		const res = await this.sendUpdatedAppointment({
			appointmentID: assignment.appointmentID, oldResourceID: assignment.resourceId, newResourceID: null });
		if (!res) return false;

		this.appointments.removeAssignment(assignment.assignmentID);
		await this.updateCalendar();
		return true;
	}

	protected async onConfirmAppointment({ record, eventRecord, resourceRecord, assignmentRecord }) {
		const item = this.getDataItemFromMenuCommand(record, eventRecord, resourceRecord, assignmentRecord);
		return await this.confirmAppointment(item.appointmentID, !item.isConfirmed);
	}

	protected async confirmAppointment(appointmentID: string, confirmed: boolean) {
		return await this.sendUpdatedAppointment({ appointmentID: appointmentID, confirmed: confirmed });
	}

	protected async onValidateAppointment({ record, eventRecord, resourceRecord, assignmentRecord }) {
		const item = this.getDataItemFromMenuCommand(record, eventRecord, resourceRecord, assignmentRecord);
		return await this.validateAppointment(item.appointmentID, !item.isValidatedByDispatcher);
	}

	protected async validateAppointment(appointmentID: string, validatedByDispatcher: boolean) {
		return await this.sendUpdatedAppointment({ appointmentID: appointmentID, validatedByDispatcher: validatedByDispatcher });
	}

	protected async onBeforeEventResizeFinalize({ source, context }) {
		context.async = true;
		let res = false;
		this.calendarRedrawSuppressed = true; // prevent excessive flickering
		try {
			res = await this.sendUpdatedAppointment({
				appointmentID: context.eventRecord.id, newBegin: context.startDate, newEnd: context.endDate });
		}
		finally {
			this.calendarRedrawSuppressed = false;
		}
		context.finalize(res);
		if (!res) return;
	}

	protected async onBeforeEventDropFinalize({ source: scheduler, context}) {
		if (!isAppointmentChanged(context.eventRecord, context)) return;

		context.async = true;
		const oldResourceID = context.resourceRecord.id;
		const newResourceID = context.newResource.id;

		this.calendarRedrawSuppressed = true; // prevent excessive flickering

		let res = false;
		try {
			res = await this.sendUpdatedAppointment({
				appointmentID: context.eventRecord.id, newBegin: context.startDate, newEnd: context.endDate,
				newResourceID: newResourceID, oldResourceID: oldResourceID});

			if (newResourceID !== oldResourceID) {
				const oldAssignment = this.appointments.getAssignmentByAppointmentAndResource(context.eventRecord.id, oldResourceID);
				this.appointments.removeAssignment(oldAssignment.assignmentID);
				this.calendarRedrawSuppressed = false; // we need to redraw the calendar to remove the old assignment
				this.updateCalendar();
			}
		}
		finally {
			this.calendarRedrawSuppressed = false;
		}
		context.finalize(res);

		function isAppointmentChanged(event: EventModel, context) {
			if (context.resourceRecord.id !== context.newResource.id) return true;
			if ((event.startDate as Date).toISOString() !== (context.startDate as Date).toISOString()) return true;
			if ((event.endDate as Date).toISOString() !== (context.endDate as Date).toISOString()) return true;
			return false;
		}
	}

	protected async assignUnassignedAppointment(eventId: number, resourceId: number, dateBegin: Date, dateEnd: Date) {
		const res = await this.sendUpdatedAppointment({
			appointmentID: eventId, newBegin: dateBegin, newEnd: dateEnd, newResourceID: resourceId });
		if (!res) return false;

		this.appointments.removeFromUnassigned(eventId.toString());
		this.updateCalendar();
		return true;
	}

	protected async addResourceToAppointment(resourceId: number, eventId: number) {
		return await this.sendUpdatedAppointment({ appointmentID: eventId, newResourceID: resourceId });
	}

	protected async sendUpdatedAppointment(
		request: {
			appointmentID: number | string;
			newBegin?: any;
			newEnd?: any;
			newResourceID?: number | string;
			oldResourceID?: number | string;
			confirmed?: boolean;
			validatedByDispatcher?: boolean;
		}
	) {
		const appointment = this.getAppointment(request.appointmentID.toString());

		this.UpdatedAppointment.AppointmentID.updateValue(Number(appointment.appointmentID));
		this.UpdatedAppointment.NewResourceID.updateValue(Number(request.newResourceID ?? 0));
		this.UpdatedAppointment.OldResourceID.updateValue(Number(request.oldResourceID ?? 0));
		this.UpdatedAppointment.NewBegin.updateValue((request.newBegin as Date)?.fromView());
		this.UpdatedAppointment.NewEnd.updateValue((request.newEnd as Date)?.fromView());
		this.UpdatedAppointment.Confirmed.updateValue(request.confirmed);
		this.UpdatedAppointment.ValidatedByDispatcher.updateValue(request.validatedByDispatcher);

		const canAffectSearchResults = (this.searchAppointments.getEntry(appointment.appointmentID));
		const views = (canAffectSearchResults
			? [nameof("LastUpdatedAppointment"), nameof("LastUpdatedAppointmentFilter"), nameof("SearchAppointments")]
			: [nameof("LastUpdatedAppointment"), nameof("LastUpdatedAppointmentFilter")]);
		try {
			const res = await this.screenService.update("UpdateAppointment", new ScreenUpdateParams({ blockPage: false, views: views }));
			return res.succeeded;
		}
		catch (error) {
			return false;
		}
	}

	protected getAppointment(id: string) {
		const appointment = this.appointments.getAppointment(id);
		if (appointment) return appointment;

		return this.searchAppointments.getEntry(id);
	}

	protected async createAppointmentFromSODetail(entity: ServiceOrderModel, resourceId: number, dateBegin: Date, dateEnd: Date) {
		return await this.createAppointmentFromSO(entity, resourceId, dateBegin, dateEnd, true);
	}

	protected async createAppointmentFromSO(entity: ServiceOrderModel, resourceId: number, dateBegin: Date, dateEnd: Date, fromDetail = false) {
		const fields = Object.keys(this.MainAppointmentFilter).filter(x => !!(this.MainAppointmentFilter[x] as PXFieldState));
		fields.forEach(field => this.screenService.setScreenParameter(nameof("MainAppointmentFilter"), field, null));

		this.MainAppointmentFilter.OnHold.updateValue(false);
		this.MainAppointmentFilter.OpenEditor.updateValue(false);

		this.MainAppointmentFilter.SrvOrdType.updateValue(entity.SrvOrdType?.cellText);
		this.MainAppointmentFilter.SOID.updateValue(Number(entity.SOID?.cellText ?? 0));
		this.MainAppointmentFilter.Resources.updateValue(resourceId.toString());
		this.MainAppointmentFilter.ScheduledDateTimeBegin_Date.updateValue(dateBegin.fromView());
		this.MainAppointmentFilter.ScheduledDateTimeBegin_Time.updateValue(dateBegin.fromView());
		this.MainAppointmentFilter.Duration.updateValue(TimeConverter.minutesTohMM(dateEnd.fromView().getTime() - dateBegin.fromView().getTime()) / 1000 / 60); // eslint-disable-line @typescript-eslint/no-magic-numbers
		this.MainAppointmentFilter.Description.updateValue(fromDetail ? entity.FSSODet__TranDesc?.cellText : entity.DocDesc?.cellText);

		if (fromDetail) {
			this.MainAppointmentFilter.SODetID.updateValue(Number(entity.FSSODet__SODetID?.cellText));
		}

		try {
			this.LastUpdatedAppointmentFilter.AppointmentID.updateValue(0);
			const res = await this.screenService.update("ScheduleAppointment", new ScreenUpdateParams({ blockPage: false,
				views: [nameof("MainAppointmentFilter"), nameof("EditedAppointmentEmployees"), nameof("LastUpdatedAppointment"),
					nameof("LastUpdatedAppointmentFilter")]
			}));
			const appointmentId = this.LastUpdatedAppointmentFilter?.AppointmentID?.value;
			if (!appointmentId) {
				this.reportErrorsFromEditedAppointment();
				return 0;
			}

			this.serviceOrders.scheduleEntry(fromDetail ? entity.serviceId : entity.orderId);
			this.updateSOPane(false);

			return appointmentId;
		}
		catch (error) {
			this.reportErrorsFromEditedAppointment();
			return 0;
		}
	}

	protected async reportErrorsFromEditedAppointment() {
		const fields = Object.values(this.MainAppointmentFilter);
		fields.forEach(field => {
			if (field.error?.length > 0) {
				showInformer(field.error, "error");
			}
		});
	}

	protected async createApointmentFromPopup(putOnHold = false, openEditor = false) {
		await this.ReleaseUIControl();
		this.LastUpdatedAppointmentFilter.AppointmentID.updateValue(0);
		this.MainAppointmentFilter.OnHold.updateValue(putOnHold);
		this.MainAppointmentFilter.OpenEditor.updateValue(openEditor);
		const res = await this.screenService.update("ScheduleAppointment", new ScreenUpdateParams({ blockPage: false,
			views: [nameof("MainAppointmentFilter"), nameof("LastUpdatedAppointment"), nameof("LastUpdatedAppointmentFilter")]
		}));
		if (openEditor) {
			this.closePopup();
			this.resetCreateAppointmentForm();
			return 0;
		}
		const appointmentId = this.LastUpdatedAppointmentFilter?.AppointmentID?.value;
		if (!appointmentId) return 0;

		this.closePopup();
		return appointmentId;
	}

	protected async onBranchChanged() {
		//
	}

	protected async ReleaseUIControl() {
		await new Promise(resolve => setTimeout(resolve, 1));
	}
}


const nameof = (name: Extract<keyof FS300300, string>): string => name;

export class DragHelperEventInfo {
	name: string;
	entity?: DraggableEntity;
	date?: Date;
	customerName?: string;
}

type SplitterState = 'collapsed-first' | 'collapsed-second' | 'normal';

class SplitterPreferences {
	state?: SplitterState;
	position?: number;
	toString() { return `s: ${this.state} p: ${this.position}`; }

	constructor(preferences: SplitterPreferences = null) {
		if (preferences === null) return;
		this.state = preferences.state;
		this.position = preferences.position;
	}
}

class PreferenceData {
	horizontalModeSplitter = new SplitterPreferences();
	verticalModeSplitter = new SplitterPreferences();
	periodKind = PeriodKind.Day;
	toString() { return `h: (${this.horizontalModeSplitter.toString()}) v: (${this.verticalModeSplitter.toString()})`; }

	constructor(preferences: PreferenceData = null) {
		if (preferences === null) return;
		this.horizontalModeSplitter = new SplitterPreferences(preferences.horizontalModeSplitter);
		this.verticalModeSplitter = new SplitterPreferences(preferences.verticalModeSplitter);
		this.periodKind = preferences.periodKind;
	}
	// TODO: add vert/hor mode
	// TODO: add business hours/normal
	// TODO: add day/month/week filter???
}

class FS300300_Preferences extends PreferenceBase {
	preferences = new PreferenceData();

	constructor(preferences: PreferenceData = null) {
		super();
		this.type = "FS300300";
		if (preferences === null) return;
		this.preferences = new PreferenceData(preferences);
	}
}

declare global { // to access the global type String
	interface Date {
		withoutTimeInMsec(): number;
		toView(): Date;
		fromView(): Date;
		clearHoursUTC(): Date;
	}
}

Date.prototype.withoutTimeInMsec = function () {
	const d = this.clearHoursUTC();
	return d.getTime();
};


Date.prototype.toView = function () {
	// eslint-disable-next-line @typescript-eslint/no-magic-numbers
	return new Date(this.getTime() + this.getTimezoneOffset() * 60000);
};

Date.prototype.fromView = function () {
	// eslint-disable-next-line @typescript-eslint/no-magic-numbers
	return new Date(this.getTime() - this.getTimezoneOffset() * 60000);
};

Date.prototype.clearHoursUTC = function () {
	const newDate = (this.getTimezoneOffset() > 0)
		? new Date(new Date(this.toUTCString().substring(0, 25)))  // eslint-disable-line @typescript-eslint/no-magic-numbers
		: new Date(this);
	newDate.setHours(0, 0, 0, 0);
	return newDate;

};
