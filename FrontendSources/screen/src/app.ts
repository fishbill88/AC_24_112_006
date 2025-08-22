import { observable } from 'aurelia-framework';
import { Disposable, bindable, autoinject, BindingEngine } from 'aurelia-framework';
import { getLogger } from "aurelia-logging";
import { EventAggregator } from 'aurelia-event-aggregator';
import { EventManager, delegationStrategy, computedFrom } from "aurelia-binding";
import { Container } from 'aurelia-dependency-injection';
import {
	QpEventManager, OpenPopupEvent, ExecuteCommandEvent, PXScreen,
	IDataComponentParams, IScreenService, RefreshScreenEvent, ReloadPageEvent, ScreenApiClientSettings, RedirectHelper,
	DefaultPreferencesDataComponent,
	UserPreferencesDataComponent,
	QpDataComponent,
	QpSplitterCustomElement,
	ClosePopupEvent,
	ScreenUpdateParams,
	QpSidePanelCustomElement,
	SidePanelDataComponent,
	showInformer,
	TranslationValidationDataComponent,
	getCompanyFromUrl
} from 'client-controls';
import { IScreenPreferences, PreferencesService } from "client-controls/services/preferences";

import { getScreenIdFromUrl, getScreenPath } from './screen-utils';
import { localizable, QpTranslator } from 'client-controls/services/localization';
import { AlertService } from 'client-controls/services/alert-service';
import { ScreenService } from 'client-controls/services/screen-service';
import { PopupMessageOpenEvent } from 'client-controls/services/screen-service';
import { KeyboardService } from 'client-controls/services/keyboard-service';
import { DialogHelper } from 'client-controls/controls/dialog/base-dialog/dialog-helper';
import { BusyCounter } from 'client-controls/utils';
import { IQpDialogConfig, IGeneralDialogConfig } from 'client-controls/controls/dialog/base-dialog/qp-dialog';
import { DialogController, DialogOpenResult } from 'aurelia-dialog';

const logger = getLogger('qp-app');

@localizable
class Messages {
	static ScreenID = "Screen ID";
	static Note = "Note";
}

declare const HTML_MERGED: boolean;

@autoinject
export class App {
	@bindable screenName?: string;
	@bindable useStaticRendering: boolean = HTML_MERGED;
	initialized = false;
	loading = false;
	viewModel?: PXScreen;
	screenService?: IScreenService;
	contentElement!: HTMLElement;
	forceUI?: string;
	keyboardService: KeyboardService;
	busyService: BusyCounter;
	isMainFrame: boolean = true;

	eventSubscriptions: Disposable[] = [];
	bindingSubscriptions: Disposable[] = [];
	baseHref: string = "";
	editMode: boolean = false;
	siteMapScreenID?: string;

	@observable sidePanelComponent?: QpDataComponent<SidePanelDataComponent>;
	@observable splitterVM?: QpSplitterCustomElement;

	private Msg = Messages;
	private screenEventManager: QpEventManager;
	private giScreenID = 'GenericInquiry';
	private translationValidationComponent = new TranslationValidationDataComponent();
	private openDialogs: DialogController[] = [];

	constructor(private container: Container,
		private eventAggregator: EventAggregator,
		protected dialogHelper: DialogHelper,
		private translator: QpTranslator,
		private alertServce: AlertService,
		private preferencesService: PreferencesService,
		protected eventManager: EventManager,
		private redirectHelper: RedirectHelper,
		private bindingEngine: BindingEngine,
	) {
		this.screenEventManager = container.get(QpEventManager);
		this.keyboardService = container.invoke(KeyboardService);
		container.registerInstance(KeyboardService, this.keyboardService);
		this.busyService = container.get(BusyCounter);
		this.busyService.increment();
	}

	@computedFrom(
		'sidePanelComponent',
		'sidePanelComponent.component',
		'sidePanelComponent.component.sidePanelVM',
		'sidePanelComponent.component.collapsed',
		'sidePanelComponent.component.maximized'
	)
	private get sidebarDisabled(): boolean {
		const { sidePanelComponent } = this;

		return !sidePanelComponent || !sidePanelComponent.component || !sidePanelComponent.component.sidePanelVM
			|| sidePanelComponent.component.collapsed || sidePanelComponent.component.maximized;
	}

	viewModelActivated() {
		this.screenService = this.viewModel.getScreenService();
		if (window.frameElement) {
			if (this.siteMapScreenID !== this.giScreenID && this.isMainFrame) {
				if (this.screenService) this.screenService.siteMapScreenID = this.siteMapScreenID;
			}
		}
		this.container.registerInstance(ScreenService, this.screenService);

		// all default screen preferences come on first GET request
		this.screenService.registerDataComponentOneTime("DefaultScreenPreferences",
			() => new DefaultPreferencesDataComponent(this.preferencesService));
		// this data component works only on first POST request - all user preferences come on first POST request
		this.screenService.registerDataComponentOneTime("UserScreenPreferences",
			new UserPreferencesDataComponent(this.preferencesService));

		this.screenService.registerDataComponentOneTime("TranslationValidation",
			this.translationValidationComponent);
	}

	screenIsDirty(): boolean {
		if (this.viewModel) return this.viewModel.isDirty;
		return false;
	}

	applyScreenPreferences(result: IScreenPreferences, mode: "user" | "def"): void {
		for (const cid in result) {
			this.preferencesService.setPreferences(cid, result[cid], mode);
		}
	}

	getQueryParams(): IDataComponentParams {
		return {};
	}

	openDialog(msg: OpenPopupEvent) {
		const generalDialogConfig: IGeneralDialogConfig = {
			context: this.viewModel,
			rootElement: this.contentElement,
			command: msg.commandName,
			autoRepaint: msg.autoRepaint,
			caption: msg.dialogMessage
		};
		const dialogConfig: IQpDialogConfig = {
			...generalDialogConfig, ...msg.detail?.content, ...{ id: msg.dialogId, data: msg.detail?.data }
		};

		this.dialogHelper.openDialog(dialogConfig, {
			overlayDismiss: false
		}, (r: DialogOpenResult) => {
			this.openDialogs.push(r.controller);
		}).whenClosed(() => this.openDialogs.length = this.openDialogs.length - 1);
	}

	serializeParameters(p: { [key: string]: string }): string {
		let res = "";
		for (const key in p) {
			res += `&${key}=${p[key]}`;
		}
		return res;
	}

	private async attached() {
		const parts = window.location.search.split("&");
		if (parts[0][0] === "?") parts[0] = parts[0].substr(1);

		const params: { [key: string]: string } = {};
		let forceUI = null;
		let isRedirect = null;
		for (const part of parts) {
			const idx = part.indexOf("=");
			if (idx < 0) continue;
			const key = part.substr(0, idx);
			const value = part.substr(idx + 1);

			switch (key) {
				case "ScreenId":
				case "unum":
				case "HideScript":
				case "timeStamp":
				case "id":
					break;
				case "PopupPanel":
					if (value) isRedirect = true;
					if (value === "Layer") this.isMainFrame = false;
					break;
				case "InLayer":
					if (value === "On") this.isMainFrame = false;
					break;
				case "isRedirect":
					isRedirect = Boolean(value);
					break;
				case "ui":
				case "UI":
					forceUI = value;
					break;
				default:
					params[key] = decodeURIComponent(value);
			}
		}

		this.siteMapScreenID = getScreenIdFromUrl();
		if (!this.siteMapScreenID) this.screenName = "not-found";
		else {
			this.screenName = getScreenPath(this.siteMapScreenID);
		}

		const companyId = getCompanyFromUrl();

		if (forceUI || (<any>window).isRedirect || (<any>window.parent)?.isRedirect || isRedirect) {
			this.forceUI = forceUI;
			const screenApiClientSettings = this.container.get(ScreenApiClientSettings);
			screenApiClientSettings.forceUI = forceUI;
			screenApiClientSettings.companyId = companyId;
			screenApiClientSettings.dueToRedirect = (<any>window).isRedirect || (<any>window.parent)?.isRedirect || isRedirect;

			delete (<any>window).isRedirect;
			delete (<any>window.parent)?.isRedirect;
		}

		let mainHref = window.top.location.href;

		const queryParamSplitterIdx = mainHref.indexOf('?');
		if (queryParamSplitterIdx >= 0) mainHref = mainHref.substr(0, queryParamSplitterIdx);
		this.baseHref = `${mainHref}?`;
		if (companyId) {
			this.baseHref += `&CompanyID=${companyId}&`;
		}
		if (forceUI) this.baseHref += `ui=${forceUI}&`;

		if (window.frameElement) {
			if (this.siteMapScreenID !== this.giScreenID && this.isMainFrame) {
				mainHref = `${this.baseHref}ScreenId=${this.siteMapScreenID}${this.serializeParameters(params)}`;
				window.top.history.replaceState({}, "Title", mainHref);
			}
		}

		let ready = false;

		this.screenEventManager.subscribe("OpenPopup", (message: OpenPopupEvent) => {
			message.stop();
			this.openDialog(message);
		});

		this.screenEventManager.subscribe(ClosePopupEvent.EventName, () => {
			if (this.openDialogs.length) {
				this.openDialogs[this.openDialogs.length - 1].close(false);
			}
		});

		this.screenEventManager.subscribe("ExecuteCommand", (evt: ExecuteCommandEvent) => {
			if (evt.Command) {
				this.screenService.executeCommand(evt.Command, evt.Params);
				evt.stop();
			}
		});
		this.eventSubscriptions.push(this.screenEventManager.subscribe("RefreshScreen", (message: RefreshScreenEvent) => {
			this.screenService.update();
		}));

		this.eventSubscriptions.push(this.screenEventManager.subscribe("ReloadPage", (message: ReloadPageEvent) => {
			this.redirectHelper.reload();
		}));

		const ea = this.eventAggregator;
		this.eventSubscriptions.push(
			ea.subscribe("screen-initialize-data-ready", () => {
				this.viewModel.setupParameters(params);
			})
		);

		this.eventSubscriptions.push(
			ea.subscribe("screen-initialized", (data: any) => {
				if (this.viewModel?.screenID === data?.screenID) ready = true;
				if (data?.failure) this.initialized = true;

				if (this.screenService.isDraft && data.screenID === this.viewModel.siteMapScreenID && !((<any> window).DisableWhitelistWarning || (<any> window.top).DisableWhitelistWarning )) showInformer("The Modern UI is turned on for the screen. This preview version of the UI is not for production use", "warning");
			})
		);

		this.eventSubscriptions.push(
			ea.subscribe("screen-updated", (data: any) => {
				if (ready && this.viewModel?.screenID === data?.screenID) {
					this.initialized = true;
					const screenId =  this.viewModel.siteMapScreenID;

					if (this.isMainFrame) {
						let mainHref = `${this.baseHref}ScreenId=${screenId}`;
						if (!this.viewModel.isNewEntry()) {
							const params = this.viewModel.getKeys();
							mainHref += this.serializeParameters(params);
						}
						window.top.history.replaceState({}, "Title", mainHref);
					}
				}
			})
		);

		this.eventSubscriptions.push(
			this.screenEventManager.subscribe("LoadingStarted", () => {
				this.loading = true;
			})
		);

		this.eventSubscriptions.push(
			this.screenEventManager.subscribe("LoadingFinished", () => {
				this.loading = false;
			})
		);

		this.eventSubscriptions.push(
			ea.subscribe(
				PopupMessageOpenEvent,
				(eventArgs: PopupMessageOpenEvent) => {
					const model = {
						title: this.Msg.Note,
						message: eventArgs.popupMessage,
					};
					this.alertServce.openAlert(model).then((result) => undefined);
				}
			)
		);

		this.eventSubscriptions.push(this.eventManager.addEventListener(document.body, "startScreenConfiguration", () => {
			this.editMode = true;
		}, delegationStrategy.none, true));

		this.eventSubscriptions.push(this.eventManager.addEventListener(document.body, "endScreenConfiguration", () => {
			this.editMode = false;
		}, delegationStrategy.none, true));

		this.keyboardService.setActiveArea(this.contentElement);

		this.busyService.decrement();
		return;
	}

	private detached(): void {
		this.eventSubscriptions.forEach((s) => s.dispose());
		this.eventSubscriptions = [];
		this.bindingSubscriptions.forEach((s) => s.dispose());
		this.bindingSubscriptions = [];
	}
}
