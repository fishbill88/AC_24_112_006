import { createCollection, createSingle, PXScreen, graphInfo, viewInfo } from "client-controls";
import { OAuthServerKey, OAuthServerKeyView } from "./views";

@graphInfo({graphType: "PX.Owin.IdentityServerIntegration.DAC.OAuthServerKeyMaint", primaryView: "AllKeys", })
export class SM303030 extends PXScreen {

	AllKeys = createCollection(OAuthServerKey);

   	@viewInfo({containerName: "Generate New Key"})
	GenerateOAuthServerKeyView = createSingle(OAuthServerKeyView);
}