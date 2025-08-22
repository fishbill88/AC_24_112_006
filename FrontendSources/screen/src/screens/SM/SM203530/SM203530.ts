import { UPCompany } from './views';
import {PXScreen, graphInfo, createCollection, PXActionState, QpEventManager } from "client-controls";
import { autoinject } from 'aurelia-framework';

@graphInfo({graphType: 'PX.SM.CompanyInquire', primaryView: 'Companies' })
@autoinject
export class SM203530 extends PXScreen {
	UPCompany_Delete: PXActionState;
	UPCompany_New: PXActionState;

	Companies = createCollection(UPCompany);

/* 	constructor(private qpEventManager: QpEventManager) {
		super();
		this.screenEventManager = qpEventManager;
		qpEventManager.subscribe("MoveCompanyUp", () => console.log("testttt"));
		qpEventManager.subscribe("ExecuteCommand", () => console.log("testttt"));
	} */
}