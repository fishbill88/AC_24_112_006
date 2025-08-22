import {
	createSingle,
	viewInfo,
	PXView,
	PXFieldState,
	PXFieldOptions,
} from "client-controls";

export abstract class FormCreateContactBase {
	@viewInfo({ containerName: "Create Contact" })
	ContactInfo = createSingle(ContactFilter);
}

export class ContactFilter extends PXView {
	FirstName: PXFieldState<PXFieldOptions.CommitChanges>;
	LastName: PXFieldState<PXFieldOptions.CommitChanges>;
	FullName: PXFieldState<PXFieldOptions.CommitChanges>;
	Salutation: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone1Type: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone1: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone2Type: PXFieldState<PXFieldOptions.CommitChanges>;
	Phone2: PXFieldState<PXFieldOptions.CommitChanges>;
	Email: PXFieldState<PXFieldOptions.CommitChanges>;
	ContactClass: PXFieldState<PXFieldOptions.CommitChanges>;
}
