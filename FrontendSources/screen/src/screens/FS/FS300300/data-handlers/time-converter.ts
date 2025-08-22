/* eslint-disable @typescript-eslint/no-magic-numbers */

export class TimeConverter {
	static hMMtoMinutes(hMM: string) {
		const hours = Math.floor(Number(hMM) / 100);
		const minutes = Number(hMM) % 100;
		return hours * 60 + minutes;
	}

	static minutesTohMM(minutes: number) {
		const hours = Math.floor(minutes / 60);
		const minutesLeft = minutes % 60;
		return hours * 100 + minutesLeft;
	}
}
