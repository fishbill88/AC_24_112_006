import { Applications } from "./SM301000";

export default class RefreshHub {

	public static subscribeOnHub(applications: Applications, updateHandler: Function) {
		const signalR = require("@microsoft/signalr");

		const connection = new signalR.HubConnectionBuilder()
			.withUrl(this.normalizeSignalRUrl("signalr/hubs/refreshHub"),
				{
					transport: signalR.HttpTransportType.WebSockets,
					skipNegotiation: true
				})
			.configureLogging(signalR.LogLevel.Debug)
			.build();

		connection.on("refreshScreen",
			appToRefreshId => {
				const appId = String(applications.ApplicationID.value.id);
				if (appId === appToRefreshId) {
					console.log("goUpdate!");
					updateHandler();
				}
			});

		connection.start()
			.then(function () {
				console.log("refreshHub connection started");
			})
			.catch((e) => {
				console.log("refreshHub failed to init");
				console.error(e);
			});
	}

	private static normalizeSignalRUrl(url) {
		let base = window.location.href;
		const baseIndex = base.indexOf("Scripts");
		if (baseIndex > 0) base = base.substring(0, baseIndex);
		url = base + url;
		return url;
	}
}

