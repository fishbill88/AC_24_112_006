import {
	createCollection,
	PXScreen,
	graphInfo,
	PXActionState,
} from "client-controls";
import { ComplianceDocument } from "./views";

@graphInfo({
	graphType: "PX.Objects.CN.Compliance.CL.Graphs.ComplianceDocumentEntry",
	primaryView: "Documents",
	hideNotesIndicator: true,
	hideFilesIndicator: true,
})
export class CL401000 extends PXScreen {
	ComplianceViewProject: PXActionState;
	ComplianceViewCostTask: PXActionState;
	ComplianceViewRevenueTask: PXActionState;
	ComplianceViewCostCode: PXActionState;
	ComplianceViewCustomer: PXActionState;
	ComplianceViewVendor: PXActionState;
	ComplianceViewSecondaryVendor: PXActionState;
	ComplianceDocument$PurchaseOrder$Link: PXActionState;
	ComplianceDocument$Subcontract$Link: PXActionState;
	ComplianceDocument$InvoiceID$Link: PXActionState;
	ComplianceDocument$BillID$Link: PXActionState;
	ComplianceDocument$ApCheckID$Link: PXActionState;
	ComplianceDocument$ArPaymentID$Link: PXActionState;
	ComplianceDocument$ProjectTransactionID$Link: PXActionState;
	ComplianceViewJointVendor: PXActionState;

	Documents = createCollection(ComplianceDocument);
}
