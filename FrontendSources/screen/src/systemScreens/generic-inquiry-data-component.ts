import { noView } from 'aurelia-framework';
import { IDataComponent, IDataComponentParams, PXScreen } from 'client-controls';
import { IFieldsetLayout, ScreenUpdateParams } from 'client-controls/descriptors';
import { GI000000 } from './GenericInquiry';

@noView
export class GenericInquiryDataComponent implements IDataComponent {
	constructor(private screenVM: PXScreen) {
	}
	getQueryParams(queryParams?: ScreenUpdateParams): IDataComponentParams {
		return {};
	}
	setComponentData(result: IGenericInquiryLayout): void {
		const giScreen = this.screenVM as GI000000;
		giScreen.layout = result;
		giScreen.layout.viewName = "Filter";

		if (result.pageSize) {
			giScreen.gridVM.config.adjustPageSize = false;
			giScreen.gridVM.config.pageSize = result.pageSize;
		}
		giScreen.gridVM.config.exportRowsLimit = result.exportTop;
	}
}

export interface IGenericInquiryLayout extends IFieldsetLayout {
	pageSize?: number;
	exportTop?: number;
}
