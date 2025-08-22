// TODO: Apply to Multiple Tenants smart panel loses its DialogResult (https://jira.acumatica.com/browse/AC-290473)
// TODO: missing RefreshSitemap functionality (https://jira.acumatica.com/browse/AC-290458)
// TODO: hide Import from Excel button (https://jira.acumatica.com/browse/AC-290457)
// TODO: toolbar actions are in invalid order (https://jira.acumatica.com/browse/AC-290471)


import { graphInfo } from "client-controls";
import { SM200520 } from "../SM200520/SM200520";

@graphInfo({
	graphType: "PX.SiteMap.Graph.PortalMapMaint",
	primaryView: "SiteMap",
})
export class SM200521 extends SM200520 { }
