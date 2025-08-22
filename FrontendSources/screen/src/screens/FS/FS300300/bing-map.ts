/* eslint-disable brace-style */
/* eslint-disable @typescript-eslint/dot-notation */
import { bindable, bindingMode, inlineView } from 'aurelia-framework';
import { Tools } from 'client-controls/controls/compound/rich-text-editor/common/tools';

const controlUrl = '//www.bing.com/api/maps/mapcontrol?callback=bingMapsLoaded';
const ready = new Promise(resolve => window['bingMapsLoaded'] = resolve);
const scriptTag: HTMLScriptElement = document.createElement('script');

scriptTag.async = true;
scriptTag.defer = true;
scriptTag.src = controlUrl;

document.head.appendChild(scriptTag);

export type MapPinClickFunc = (eventId: string, eventElement: HTMLElement) => void;

// eslint-disable-next-line no-template-curly-in-string
@inlineView('<template><div ref="container" css="width: ${width}; height: ${height};">' +
	'</div><div style="width: ${hotspotWidth}px; height: ${hotspotWidth}px; position: absolute; top: 0px; left: 0px; margin-top: 38px; opacity: 0%" ref="popupTargetElement"></div></template>') // eslint-disable-line no-template-curly-in-string
export class BingMapCustomElement {
	@bindable height = '600px';
	@bindable width = '400px';

	@bindable({ defaultBindingMode: bindingMode.twoWay }) location: Microsoft.Maps.Location | string;

	protected static baseColor = '#007ACC';
	protected static lineColors = [
		BingMapCustomElement.baseColor, "#c4121a", "#f58220", "#002d66",
		"#006991", "#ed3896", "#7857a1", "#810000",
		"#6aa84f", "#e60000", "#000000", "#247b61",
		"#83f090", "#ffae93", "#6600CC", "#0000cc"
	];

	protected hotspotWidth = 20; // eslint-disable-line @typescript-eslint/no-magic-numbers
	protected apiKey = '';
	protected container: HTMLElement;
	protected map: Microsoft.Maps.Map;
	protected viewChangeHandler: Microsoft.Maps.IHandlerId;
	protected pins = new Map<string, Microsoft.Maps.Location>();
	protected lastLineColorIndex = -1;

	protected locationCache = new Map<string, Microsoft.Maps.Location>();
	protected routesCache = new Map<string, any>();

	// protected popupTargetDiv = `<div style="width:1px; heiht:1px; position: absolute; opacity: 0%" ref="popupTargetElement"></div>`;
	protected popupTargetElement: HTMLElement;

	public async initialize(apiKey: string) {
		this.apiKey = apiKey;
		return ready.then(() => {
			this.map = new Microsoft.Maps.Map(this.container as HTMLElement, {
				credentials: apiKey,
				mapTypeId: Microsoft.Maps.MapTypeId.road,
				navigationBarMode: Microsoft.Maps.NavigationBarMode.compact,
				customMapStyle: {
					elements: {
						// area: { fillColor: '#b5db81' },
						// water: { fillColor: '#a3ccff' },
						tollRoad: { fillColor: '#50a964f4', strokeColor: '#50a964f4' },
						arterialRoad: { fillColor: '#ffffff', strokeColor: '#ffffff' },
						road: { fillColor: '#50fed89d', strokeColor: '#50eab671' },
						street: { fillColor: '#ffffff', strokeColor: '#ffffff' },
					},
					settings: {
						// landColor: '#dfdfdf'
					},
					version: ''
				}
			});

			this.location = this.map.getCenter();

			this.viewChangeHandler = Microsoft.Maps.Events.addHandler(this.map, 'viewchange', e => {
				this.location = this.map.getCenter();
			});
		});
	}

	public detached() {
		if (this.viewChangeHandler) {
			Microsoft.Maps.Events.removeHandler(this.viewChangeHandler);
		}

		if (this.map) {
			this.map.dispose();
			this.map = null;
		}
	}

	public addPin(location: Microsoft.Maps.Location, id: string, title: string, pinNumber: string, onClick: MapPinClickFunc) {
		if (this.pins.has(id)) return;

		this.pins.set(id, location);
		const pin = new Microsoft.Maps.Pushpin(location, {
			title: title,
			text: pinNumber,
			color: BingMapCustomElement.baseColor,
			// TODO: change text color
		});
		this.map.entities.push(pin);
		Microsoft.Maps.Events.addHandler(pin, 'click', () => {
			const point = this.map.tryLocationToPixel(location, Microsoft.Maps.PixelReference.control) as Microsoft.Maps.Point;
			if (point == null) return;
			this.popupTargetElement.style.top = `${point.y - this.hotspotWidth / 2}px`;
			this.popupTargetElement.style.left = `${point.x - this.hotspotWidth / 2}px`;
			setTimeout(() => { onClick?.(id, this.popupTargetElement); }, 1);
			//onClick?.(id, this.popupTargetElement);
		});
	}

	public addRouteSegment(segment: any, nextColor: boolean) { // TODO: make it typed
		const coordinates = segment?.routePath?.line?.coordinates as Array<Array<number>>;
		if (!coordinates) return;

		if (nextColor) {
			this.lastLineColorIndex = (this.lastLineColorIndex + 1) % BingMapCustomElement.lineColors.length;
		}
		const color = BingMapCustomElement.lineColors[this.lastLineColorIndex];
		const locations = coordinates.map(coord => new Microsoft.Maps.Location(coord[0], coord[1]));
		const line = new Microsoft.Maps.Polyline(locations, { strokeColor: color, strokeThickness: 3 });
		this.map.entities.push(line);
	}

	public clearAllPins() {
		if (!this.map) return;
		this.map.entities.clear();
		this.pins.clear();
		this.lastLineColorIndex = -1;
	}

	public updateView() {
		if (this.pins.size === 0) return;
		this.map.setView({ bounds: Microsoft.Maps.LocationRect.fromLocations([...this.pins.values()]), padding: 20 });
	}

	public async getLocationByAddress(address: string): Promise<Microsoft.Maps.Location> {
		if (this.locationCache.has(address)) {
			return this.locationCache.get(address);
		}

		const request = `Locations/${encodeURIComponent(address)}?`;
		const resource = await this.fetchBingData(request);

		const coordinates = resource?.point?.coordinates;
		if (!coordinates) return null;

		const location = new Microsoft.Maps.Location(coordinates[0], coordinates[1]);
		this.locationCache.set(address, location);
		return location;
	}

	public async getRouteBetweenLocations(fromPoint: Microsoft.Maps.Location, toPoint: Microsoft.Maps.Location) {
		const cacheKey = `${fromPoint.toString()} ${toPoint.toString()}`;
		if (this.routesCache.has(cacheKey)) {
			return this.routesCache.get(cacheKey);
		}

		const wp0 = `&wp.0=${fromPoint.latitude},${fromPoint.longitude}`;
		const wp1 = `&wp.1=${toPoint.latitude},${toPoint.longitude}`;

		// TODO: handle Driving/Walking and mi/km
		const request = `Routes/Driving?ra=routePath${wp0}${wp1}&du=mi&`;
		const route = await this.fetchBingData(request);
		if (!route) return null;

		this.routesCache.set(cacheKey, route);
		return route;
	}

	protected async fetchBingData(request: string) {
		const windowName = `bingmaps_callback_${Tools.newGuid().replace(/\-/gi, "_")}`;
		const url = `https://dev.virtualearth.net/REST/v1/${request}key=${this.apiKey}&jsonp=${windowName}`;
		const ready = new Promise(resolve => window[windowName] = resolve);
		const scriptTag: HTMLScriptElement = document.createElement('script');
		scriptTag.setAttribute("type", "text/javascript");
		scriptTag.setAttribute("src", url);
		document.body.appendChild(scriptTag);

		const resource = await ready.then((response) =>
			(response as any)?.resourceSets?.[0]?.resources?.[0]);

		document.body.removeChild(scriptTag);
		return resource;
	}
}
