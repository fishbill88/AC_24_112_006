import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	PXActionState,
	viewInfo
} from 'client-controls';

import {RQRequisitionContent,
	RQRequest,
	RQRequest2,
	RQRequestLine,
	POContact,
	POAddress,
	POContact2,
	POAddress2,
	EPApproval,
	RQBudget,
	RecalcDiscountsParamFilter,
	ReasonApproveRejectFilter,
	ReassignApprovalFilter,
	AddressLookupFilter,
	CurrencyInfo
} from './views';

@graphInfo({graphType: 'PX.Objects.RQ.RQRequestEntry', primaryView: 'Document', udfTypeField: 'ReqClassID', bpEventsIndicator: true, showUDFIndicator: true})
export class RQ301000 extends PXScreen {
	AddressLookup: PXActionState;
	AddSelectedItems: PXActionState;
	RecalculatePricesAction: PXActionState;
	RecalculatePricesActionOk: PXActionState;
	AddressLookupSelectAction: PXActionState;

	@viewInfo({containerName: 'Document Summary'})
	Document = createSingle(RQRequest);

	@viewInfo({containerName: 'Document'})
	CurrentDocument = createSingle(RQRequest2);

	@viewInfo({containerName: 'Currency'})
	_RQRequest_CurrencyInfo_ = createSingle(CurrencyInfo);

	@viewInfo({containerName: 'Details'})
	Lines = createCollection(RQRequestLine);

	@viewInfo({containerName: 'Ship-To Contact'})
	Shipping_Contact = createSingle(POContact);

	@viewInfo({containerName: 'Ship-To Address'})
	Shipping_Address = createSingle(POAddress);

	@viewInfo({containerName: 'Vendor Contact'})
	Remit_Contact = createSingle(POContact2);

	@viewInfo({containerName: 'Vendor Address'})
	Remit_Address = createSingle(POAddress2);

	@viewInfo({containerName: 'Approvals'})
	Approval = createCollection(EPApproval);

	@viewInfo({containerName: 'Details'})
	Budget = createCollection(RQBudget);

	@viewInfo({containerName: 'Recalculate Prices'})
	recalcPricesFilter = createSingle(RecalcDiscountsParamFilter);

	@viewInfo({containerName: 'Enter Reason'})
	ReasonApproveRejectParams = createSingle(ReasonApproveRejectFilter);

	@viewInfo({containerName: 'Reassign Approval'})
	ReassignApprovalFilter = createSingle(ReassignApprovalFilter);

	@viewInfo({containerName: 'Address Lookup'})
	AddressLookupFilter = createSingle(AddressLookupFilter);

	@viewInfo({containerName: 'Requisition Details'})
	Contents = createCollection(RQRequisitionContent);
}