import { AP303000 } from '../AP303000';
import { PXView, createCollection, PXFieldState, PXFieldOptions, gridConfig, createSingle, GridPreset } from 'client-controls';

export interface AR303000_CR_CreateContact extends AP303000 { }

// #include file="~\Pages\CR\Includes\CRCreateContactPanel.inc"
export class AR303000_CR_CreateContact {

	ContactInfoUDF = createCollection(PopupUDFAttributes);
	ContactInfoAttributes = createCollection(PopupAttributes);

	ContactInfo = createSingle(ContactFilter);

}

@gridConfig({ preset: GridPreset.Details })
export class PopupUDFAttributes extends PXView {

	DisplayName: PXFieldState;
	Value: PXFieldState<PXFieldOptions.CommitChanges>;

}

@gridConfig({ preset: GridPreset.Details })
export class PopupAttributes extends PXView {

	DisplayName: PXFieldState;
	Value: PXFieldState<PXFieldOptions.CommitChanges>;

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
