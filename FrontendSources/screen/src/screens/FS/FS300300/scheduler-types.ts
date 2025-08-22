export interface ISchedulerApiClient {
	openAppointmentEditor(screenId: string, dataMember: string, value: string): Promise<void>;
}

export interface ISchedulerBaseQuery {
}

export interface IDataQuery extends ISchedulerBaseQuery {
}


export enum PeriodKind {
	Day = 1,
	Week = 2,
	Month = 3,
}


