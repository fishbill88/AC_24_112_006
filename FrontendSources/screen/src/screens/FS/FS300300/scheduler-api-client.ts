import { BaseApiClient } from 'client-controls/services/base-api-client';
import { autoinject } from 'aurelia-framework';
import { json } from "aurelia-fetch-client";
import { Format } from "client-controls/services";
import { IDataQuery, ISchedulerApiClient, ISchedulerBaseQuery } from "./scheduler-types";

const RESOURCE_ROUTE = 'fs/screen/{0}/scheduler';

@autoinject
export class SchedulerApiClient implements ISchedulerApiClient {
	constructor(private client: BaseApiClient) {
	}

	public async openAppointmentEditor(screenId: string, dataMember: string, value: string) {
		await this.client.fetchExt(`ui/screen/${screenId}/selector/edit-screen`, { DataMember: dataMember, Value: value });
	}
}

