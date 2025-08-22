import {
	PXScreen,
	graphInfo,
	PXView,
	createCollection,
	PXFieldState,
	gridConfig,
	PXActionState,
	viewInfo,
	createSingle,
	PXFieldOptions,
	columnConfig,
	GridColumnShowHideMode,
	linkCommand,
	GridPreset,
} from "client-controls";

@graphInfo({
	graphType: "PX.Objects.EP.EmployeeMaint",
	primaryView: "Employee",
	bpEventsIndicator: false,
	udfTypeField: "",
})
export class EP203000 extends PXScreen {
	AddressLookup: PXActionState;
	ResetPassword: PXActionState;
	ActivateLogin: PXActionState;
	EnableLogin: PXActionState;
	DisableLogin: PXActionState;
	UnlockLogin: PXActionState;
	ResetPasswordOK: PXActionState;
	AddressLookupSelectAction: PXActionState;
	ViewMap: PXActionState;
	EmployeeSchedule: PXActionState;
	OpenLicenseDocument: PXActionState;
	ViewContact: PXActionState;

	@viewInfo({ containerName: "Employee Info" })
	Employee = createSingle(EPEmployee);
	CurrentEmployee = createSingle(EPEmployee2);
	@viewInfo({ containerName: "Contact Info" })
	Contact = createSingle(Contact);
	@viewInfo({ containerName: "Address info" })
	Address = createSingle(Address);

	@viewInfo({ containerName: "Financial" })
	DefLocation = createSingle(Location);
	@viewInfo({ containerName: "Payment Instructions" })
	PaymentDetails = createCollection(VendorPaymentMethodDetail);

	@viewInfo({ containerName: "Attributes" })
	Answers = createCollection(CSAnswers);

	@viewInfo({ containerName: "Mailing && Printing" })
	NWatchers = createCollection(NotificationRecipient);

	@viewInfo({ containerName: "Workgroups" })
	CompanyTree = createCollection(EPCompanyTreeMember);

	@viewInfo({ containerName: "Assignment and Approval Maps" })
	AssigmentAndApprovalMaps = createCollection(EPRule);

	@viewInfo({ containerName: "Delegates" })
	Delegates = createCollection(EPWingman);

	@viewInfo({ containerName: "User" })
	User = createSingle(Users);
	Roles = createCollection(EPLoginTypeAllowsRole);

	@viewInfo({ containerName: "Corporate Cards" })
	EmployeeCorpCards = createCollection(EPEmployeeCorpCardLink);

	@viewInfo({ containerName: "Labor Item Overrides" })
	LaborMatrix = createCollection(EPEmployeeClassLaborMatrix);

	@viewInfo({ containerName: "Skills" })
	EmployeeSkills = createCollection(FSEmployeeSkill);

	@viewInfo({ containerName: "Service Areas" })
	EmployeeGeoZones = createCollection(FSGeoZoneEmp);

	@viewInfo({ containerName: "Licenses" })
	EmployeeLicenses = createCollection(FSLicense);

	@viewInfo({ containerName: "Generate Time Cards" })
	GenTimeCardFilter = createSingle(GenTimeCardFilter);
	@viewInfo({ containerName: "Specify New ID" })
	ChangeIDDialog = createSingle(ChangeIDParam);
	@viewInfo({ containerName: "Address Lookup" })
	AddressLookupFilter = createSingle(AddressLookupFilter);
}

export class EPEmployee extends PXView {
	AcctCD: PXFieldState;
	AcctName: PXFieldState;
	VStatus: PXFieldState;
	VendorClassID: PXFieldState<PXFieldOptions.CommitChanges>;
	ChkServiceManagement: PXFieldState;
}

export class EPEmployee2 extends PXView {
	AcctReferenceNbr: PXFieldState;
	ParentBAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	DepartmentID: PXFieldState;
	CalendarID: PXFieldState;
	DefaultWorkgroupID: PXFieldState;
	HoursValidation: PXFieldState;
	SupervisorID: PXFieldState;
	SalesPersonID: PXFieldState;
	UserID: PXFieldState<PXFieldOptions.Disabled>;
	CuryID: PXFieldState;
	AllowOverrideCury: PXFieldState;
	CuryRateTypeID: PXFieldState;
	AllowOverrideRate: PXFieldState;
	BaseCuryID: PXFieldState<PXFieldOptions.Disabled>;
	LabourItemID: PXFieldState;
	ShiftID: PXFieldState;
	UnionID: PXFieldState;
	RouteEmails: PXFieldState;
	TimeCardRequired: PXFieldState;
	AMProductionEmployee: PXFieldState;
	SDEnabled: PXFieldState<PXFieldOptions.CommitChanges>;
	PrepaymentAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	PrepaymentSubID: PXFieldState;
	ExpenseAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpenseSubID: PXFieldState;
	SalesAcctID: PXFieldState<PXFieldOptions.CommitChanges>;
	SalesSubID: PXFieldState;
	TermsID: PXFieldState;
}

export class Contact extends PXView {
	Title: PXFieldState;
	FirstName: PXFieldState;
	MidName: PXFieldState;
	LastName: PXFieldState;
	Phone1Type: PXFieldState;
	Phone1: PXFieldState;
	Phone2Type: PXFieldState;
	Phone2: PXFieldState;
	Phone3Type: PXFieldState;
	Phone3: PXFieldState;
	FaxType: PXFieldState;
	Fax: PXFieldState;
	EMail: PXFieldState<PXFieldOptions.CommitChanges>;
	WebSite: PXFieldState<PXFieldOptions.CommitChanges>;
	Synchronize: PXFieldState;
	Salutation: PXFieldState;
	DateOfBirth: PXFieldState;
}

export class Address extends PXView {
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState<PXFieldOptions.CommitChanges>;
	State: PXFieldState;
	PostalCode: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class Location extends PXView {
	VAPAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
	VAPSubID: PXFieldState;
	VTaxZoneID: PXFieldState;
	VPaymentMethodID: PXFieldState<PXFieldOptions.CommitChanges>;
	VCashAccountID: PXFieldState<PXFieldOptions.CommitChanges>;
}

@gridConfig({
	preset: GridPreset.Attributes,
	adjustPageSize: true,
})
export class VendorPaymentMethodDetail extends PXView {
	@columnConfig({ allowUpdate: false })
	PaymentMethodDetail__descr: PXFieldState;
	DetailValue: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	allowUpdate: false,
	adjustPageSize: true,
	fastFilterByAllFields: false,
})
export class CSAnswers extends PXView {
	@columnConfig({
		allowUpdate: false,
		allowShowHide: GridColumnShowHideMode.False,
	})
	AttributeID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	isRequired: PXFieldState;
	@columnConfig({
		allowUpdate: false,
		allowShowHide: GridColumnShowHideMode.False,
	})
	Value: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	allowUpdate: false,
	adjustPageSize: true,
	fastFilterByAllFields: false,
})
export class NotificationRecipient extends PXView {
	NotificationID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Format: PXFieldState;
	@columnConfig({ allowUpdate: false })
	EntityDescription: PXFieldState;
	@columnConfig({ allowUpdate: false })
	ReportID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	NotificationSetup__Module: PXFieldState;
	@columnConfig({ allowUpdate: false })
	NotificationSetup__SourceCD: PXFieldState;
	@columnConfig({ allowUpdate: false })
	NotificationSetup__NotificationCD: PXFieldState;
	@columnConfig({ allowUpdate: false })
	ClassID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	TemplateID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Hidden: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Active: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	fastFilterByAllFields: false,
})
export class EPCompanyTreeMember extends PXView {
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	WorkGroupID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	IsOwner: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Active: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Inquiry,
	syncPosition: true,
	allowUpdate: false,
	adjustPageSize: true,
	suppressNoteFiles: true,
	fastFilterByAllFields: false,
})
export class EPRule extends PXView {
	@linkCommand("ViewMap")
	@columnConfig({ allowUpdate: false })
	EPAssignmentMap__Name: PXFieldState;
	@columnConfig({ allowUpdate: false })
	StepName: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Name: PXFieldState;
}

@gridConfig({
	initNewRow: true,
	adjustPageSize: true,
	fastFilterByAllFields: false,
})
export class EPWingman extends PXView {
	DelegationOf: PXFieldState<PXFieldOptions.CommitChanges>;
	WingmanID: PXFieldState<PXFieldOptions.CommitChanges>;
	WingmanID_EPEmployee_acctName: PXFieldState;
	IsActive: PXFieldState<PXFieldOptions.CommitChanges>;
	StartsOn: PXFieldState<PXFieldOptions.CommitChanges>;
	ExpiresOn: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class Users extends PXView {
	State: PXFieldState<PXFieldOptions.Disabled>;
	LoginTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	Username: PXFieldState<PXFieldOptions.CommitChanges>;
	Password: PXFieldState;
	GeneratePassword: PXFieldState<PXFieldOptions.CommitChanges>;
	NewPassword: PXFieldState;
	ConfirmPassword: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	fastFilterByAllFields: false,
})
export class EPLoginTypeAllowsRole extends PXView {
	@columnConfig({ allowUpdate: false })
	Selected: PXFieldState;
	@columnConfig({ allowUpdate: false, hideViewLink: true })
	Rolename: PXFieldState;
	@columnConfig({ allowUpdate: false })
	Rolename_Roles_descr: PXFieldState;
}

@gridConfig({
	initNewRow: true,
	adjustPageSize: true,
	fastFilterByAllFields: false,
})
export class EPEmployeeCorpCardLink extends PXView {
	@columnConfig({ allowUpdate: false })
	CorpCardID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowUpdate: false })
	CACorpCard__Name: PXFieldState;
	@columnConfig({ allowUpdate: false })
	CACorpCard__CardNumber: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	fastFilterByAllFields: false,
})
export class EPEmployeeClassLaborMatrix extends PXView {
	EarningType: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowUpdate: false })
	LabourItemID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowUpdate: false })
	EPEarningType__Description: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	fastFilterByAllFields: false,
})
export class FSEmployeeSkill extends PXView {
	@columnConfig({ hideViewLink: true })
	SkillID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	FSSkill__Descr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	FSSkill__IsDriverSkill: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	fastFilterByAllFields: false,
})
export class FSGeoZoneEmp extends PXView {
	@columnConfig({ allowUpdate: false })
	GeoZoneID: PXFieldState;
	@columnConfig({ allowUpdate: false })
	FSGeoZone__Descr: PXFieldState;
}

@gridConfig({
	preset: GridPreset.Details,
	adjustPageSize: true,
	fastFilterByAllFields: false,
})
export class FSLicense extends PXView {
	@linkCommand("OpenLicenseDocument")
	@columnConfig({ allowUpdate: false })
	RefNbr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	LicenseTypeID: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowUpdate: false })
	Descr: PXFieldState;
	@columnConfig({ allowUpdate: false })
	IssueDate: PXFieldState;
	@columnConfig({ allowUpdate: false })
	NeverExpires: PXFieldState<PXFieldOptions.CommitChanges>;
	@columnConfig({ allowUpdate: false })
	ExpirationDate: PXFieldState;
}

export class GenTimeCardFilter extends PXView {
	LastDateGenerated: PXFieldState;
	GenerateUntil: PXFieldState<PXFieldOptions.CommitChanges>;
}

export class ChangeIDParam extends PXView {
	CD: PXFieldState;
}

export class AddressLookupFilter extends PXView {
	SearchAddress: PXFieldState;
	ViewName: PXFieldState;
	AddressLine1: PXFieldState;
	AddressLine2: PXFieldState;
	AddressLine3: PXFieldState;
	City: PXFieldState;
	CountryID: PXFieldState;
	State: PXFieldState;
	PostalCode: PXFieldState;
	Latitude: PXFieldState;
	Longitude: PXFieldState;
}
