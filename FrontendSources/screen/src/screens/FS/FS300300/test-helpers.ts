import { SchedulerPro } from '@bryntum/schedulerpro';
/* eslint-disable @typescript-eslint/no-magic-numbers */
export function testHelperSimulateDragAndDrop(sourceElementCSS: string, targetElementCSS: string, offsetX: number, offsetY: number) : void {
	// eslint-disable-next-line @typescript-eslint/no-empty-function
	simulateDragAndDrop(sourceElementCSS, targetElementCSS, offsetX, offsetY).finally(() => { });
}

async function simulateDragAndDrop(sourceElementCSS: string, targetElementCSS: string, offsetX: number, offsetY: number) {
	const sourceElement = document.querySelector(sourceElementCSS) as HTMLElement;
	const targetElement = document.querySelector(targetElementCSS) as HTMLElement;
	const bodyElement = document.querySelector("body") as HTMLElement;

	const sourceElemCoords = sourceElement.getBoundingClientRect();
	const targetElemCoords = targetElement.getBoundingClientRect();

	const sourceCoords = {
		left: sourceElemCoords.left + 10,
		top: sourceElemCoords.top + 5,
	};
	const targetCoords = {
		left: targetElemCoords.left + targetElemCoords.width / 2 + offsetX,
		top: targetElemCoords.top + targetElemCoords.height / 2 + offsetY,
	};

	dispatchEvent("over", sourceElement, sourceCoords);
	dispatchEvent("enter", sourceElement, sourceCoords);
	dispatchEvent("down", sourceElement, sourceCoords);
	await emulateUserPause();

	const totalParts = 10;
	for (let part = 0; part < totalParts; part++) {
		const weight = part / totalParts;
		const middleCoords = {
			left: sourceElemCoords.left * (1 - weight) + targetCoords.left * weight,
			top: sourceElemCoords.top * (1 - weight) + targetCoords.top * weight,
		};
		dispatchEvent("move", sourceElement, middleCoords);
		await new Promise(resolve => setTimeout(resolve, 250 / totalParts));
	}

	dispatchEvent("move", sourceElement, targetCoords);
	await emulateUserPause();

	dispatchEvent("up", sourceElement, targetCoords);
}

async function emulateUserPause() {
	await new Promise(resolve => setTimeout(resolve, 100));
}


export function testHelperScrollToXPercent(percent: number): void {
	// eslint-disable-next-line @typescript-eslint/no-empty-function
	scrollToXPercent(percent).finally(() => { });
}

async function scrollToXPercent(percent: number) {
	const virtualScrollbar = document.querySelector(".qp-sch-calendar .b-virtual-scroller:not(:first-child)") as HTMLElement;
	const viewportWidth = virtualScrollbar.clientWidth;

	const virtualScrollbarView = document.querySelector(".qp-sch-calendar .b-virtual-scroller:not(:first-child) > div");
	const viewWidth = virtualScrollbarView.clientWidth;

	const scrollableWidth = viewWidth - viewportWidth;

	const position = scrollableWidth * percent / 100;
	const helperElement = window.document.querySelector('.qp-sch-calendar-container') as TestHelperHTMLElement;
	await helperElement.schedulerControl.scrollHorizontallyTo(position);
}

function dispatchEvent(eventName: string, element: HTMLElement, coords: { left: number; top: number }) {
	const pointerEvent = new PointerEvent(`pointer${eventName}`, {
		bubbles: true,
		cancelable: true,
		pointerType: "mouse",
		clientX: coords.left,
		clientY: coords.top,
		view: window,
	});
	element.dispatchEvent(pointerEvent);
}

export interface TestHelperHTMLElement extends HTMLElement {
	scrollToXPercent(percent: number): void;
	simulateDragAndDrop(sourceElementCSS: string, targetElementCSS: string, offsetX: number, offsetY: number): void;
	schedulerControl: SchedulerPro;
}

