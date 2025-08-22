import {
	createCollection,
	createSingle,
	PXScreen,
	graphInfo,
	PXActionState,
	viewInfo,
} from "client-controls";
import {
	Users,
	Users2,
	EPLoginTypeAllowsRole,
	UserFilter,
	OidcUser,
	UserPreferences,
	Contact,
	EMailAccount,
	MobilePushNotificationRegToken,
	FSGPSTrackingRequest,
	ADUserFilter,
} from "./views";

@graphInfo({
	graphType: "PX.SM.AccessUsers",
	primaryView: "UserList",
	showActivitiesIndicator: true
})
export class SM201010 extends PXScreen {
	ResetPasswordOK: PXActionState;
	addADUserOK: PXActionState;

	@viewInfo({ containerName: "User Information" })
	UserList = createSingle(Users);
	@viewInfo({ containerName: "" })
	UserListCurrent = createSingle(Users2);
	@viewInfo({ containerName: "Roles" })
	AllowedRoles = createCollection(EPLoginTypeAllowsRole);

	@viewInfo({ containerName: "Allowed IP Address Ranges" })
	UserFilters = createCollection(UserFilter);

	@viewInfo({ containerName: "External Identities" })
	Identities = createCollection(OidcUser);

	@viewInfo({ containerName: "Personal Settings" })
	UserPrefs = createSingle(UserPreferences);
	@viewInfo({ containerName: "Personal Settings" })
	Contact = createSingle(Contact);

	@viewInfo({ containerName: "Email Accounts" })
	EMailAccounts = createCollection(EMailAccount);

	@viewInfo({ containerName: "Devices" })
	UserDevices = createCollection(MobilePushNotificationRegToken);

	@viewInfo({ containerName: "Location Tracking" })
	LocationTracking = createCollection(FSGPSTrackingRequest);

	@viewInfo({ containerName: "Active Directory User" })
	ADUser = createSingle(ADUserFilter);
}
