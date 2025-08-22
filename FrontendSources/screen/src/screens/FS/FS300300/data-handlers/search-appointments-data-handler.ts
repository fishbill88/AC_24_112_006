/* eslint-disable brace-style */
import { EventModel, StringHelper } from "@bryntum/schedulerpro";
import { SearchAppointmentModel } from "../view-models";

export class SearchAppointmentsDataHandler {

	protected source: SearchAppointmentModel[] = [];

	protected searchAppointmentsById: Map<string, SearchAppointmentModel>;

	public initializeWith(SearchAppointments: SearchAppointmentModel[]) {
		if (this.source.length === 0 && SearchAppointments.length === 0) return false;
		this.source = SearchAppointments.map(appointment => appointment);
		this.searchAppointmentsById = new Map(SearchAppointments.map(appointment => [appointment.appointmentID, appointment]));

		return true;
	}

	public createData() {
		const events = this.source.map(appointment => new EventModel({
			id: appointment.appointmentID,
			name: StringHelper.xss`${appointment.caption}`,
			cls: ``,
		}));
		return events;
	}

	public getEntry(id: string | number) {
		return this.searchAppointmentsById.get(id.toString());
	}

	public getFirstEntry() {
		return this.searchAppointmentsById.values().next().value;
	}
}
