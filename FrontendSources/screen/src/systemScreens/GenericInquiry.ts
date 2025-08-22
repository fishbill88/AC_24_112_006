import { autoinject, PLATFORM  } from 'aurelia-framework';
import {
	PXView, GridApiClient, GridGenericInquiryApiClient, createCollection, graphInfo, createSingle,
	GenericInquiryApiClient, ScreenApiClient, GridPagerMode, DynamicNewInstanceResolver, PXScreen,
	gridConfig, PXFieldState, columnConfig, QpGridCustomElement, GridPreset
} from 'client-controls';
import { Container } from 'aurelia-dependency-injection';
import { SearchParamsClass } from 'client-controls/controls/compound/inline-screen/qp-inline-screen';
import { GenericInquiryDataComponent, IGenericInquiryLayout } from './generic-inquiry-data-component';

@graphInfo({ graphType: 'PX.Data.PXGenericInqGrph', primaryView: 'Filter' })
@autoinject
export class GI000000 extends PXScreen {

	Filter = createSingle(Filter);

	Results = createCollection(Results);

	layout: IGenericInquiryLayout;
	gridVM: QpGridCustomElement;
	private readonly layoutComponentName = 'GenericInquiryLayout';
	private searchParams: string;

	constructor(protected container: Container, protected giSearchParams: SearchParamsClass) {
		super();

		this.searchParams = giSearchParams.searchParams;

		const gridApiClient = container.invoke(GridGenericInquiryApiClient, [this.genericInquiryId]);
		container.registerInstance(GridApiClient, gridApiClient);
		container.registerInstance(DynamicNewInstanceResolver, new GINewInstanceResolver(this.genericInquiryId));
	}

	// this method runs immediately after ctor executing and property injection
	afterConstructor() {
		super.afterConstructor();
		this.screenService.registerDataComponentOneTime(this.layoutComponentName,
			() => new GenericInquiryDataComponent(this));
	}

	get genericInquiryId(): string | undefined {
		const urlParams = new URLSearchParams(this.searchParams ?? PLATFORM.global.location.search.toLowerCase());
		return urlParams.get('id');
	}
}

export class Filter extends PXView {
}

@gridConfig({
	preset: GridPreset.PrimaryInquiry,
	allowUpdate: false,
	pagerMode: GridPagerMode.Numeric,
	defaultAction: "editDetail"
})
export class Results extends PXView {
	@columnConfig({allowCheckAll: true})
	Selected: PXFieldState;
}

class GINewInstanceResolver extends DynamicNewInstanceResolver {

	constructor(private genericInquiryId: string) {
		super();
	}

	resolveType(type: any, dynamicParams: any[])	{
		if (type?.name === ScreenApiClient.name) {
			dynamicParams?.push(this.genericInquiryId);
			return GenericInquiryApiClient;
		}
		else if (type?.name === GridApiClient.name) {
			dynamicParams?.push(this.genericInquiryId);
			return GridGenericInquiryApiClient;
		}
		return super.resolveType(type, dynamicParams);
	}
}
