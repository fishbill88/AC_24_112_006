using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using PX.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using AMMessages = PX.Web.UI.Msg;

/// <summary>
/// Provides client script configuration
/// </summary>
public class ClientSideAppsHelper
{
	
	[PXInternalUseOnly]
	private static string LocalizeString(string str)
	{
		if (string.IsNullOrEmpty(str)) return string.Empty;
		return HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(str)).Replace("\"", "\\\"");
	}

	[PXInternalUseOnly]
	[Obsolete]
	public static string RenderScriptConfiguration()
	{
		var lang = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
		var localizedNoResults = PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.SuggesterNothingFound);
		var cacheKey = "ClientSideAppsConfig:" + lang;
		string cachedResult = HttpContext.Current.Cache[cacheKey] as string;
		if (cachedResult != null)
		{
			return cachedResult;
		}

		var appRoot = HttpContext.Current.Request.ApplicationPath;
		if (appRoot.Equals("/"))
		{
			appRoot = "";
		}

		const string clientSideAppsPath = "/scripts/ca/";
		var clientSideAppsRoot = appRoot + clientSideAppsPath;

		Dictionary<string, string> ManufacturingStrings = new Dictionary<string, string>
		{
			{ "PRODUCTION_ORDERS", LocalizeString(AMMessages.UI_PRODUCTION_ORDERS) },
			{ "WORK_CENTER", LocalizeString(AMMessages.UI_WORK_CENTER) },
			{ "MACHINE", LocalizeString(AMMessages.UI_MACHINE) },
			{ "PERIOD", LocalizeString(AMMessages.UI_PERIOD) },
			{ "OPERATION_DESCRIPTION", LocalizeString(AMMessages.UI_OPERATION_DESCRIPTION) },
			{ "PRODUCTION_ORDER_INFORMATION", LocalizeString(AMMessages.UI_PRODUCTION_ORDER_INFORMATION) },
			{ "FULLSCREEN", LocalizeString(AMMessages.UI_FULLSCREEN) },
			{ "MAXIMIZE", LocalizeString(AMMessages.UI_MAXIMIZE) },
			{ "LATE_ORDERS", LocalizeString(AMMessages.UI_LATE_ORDERS) },
			{ "Column_Configuration", LocalizeString(AMMessages.UI_Column_Configuration)},
			{ "Column_Configuration_filter", LocalizeString(AMMessages.UI_Column_Configuration_filter) },
			{ "Available_Columns", LocalizeString(AMMessages.UI_Available_Columns) },
			{ "Selected_Columns", LocalizeString(AMMessages.UI_Selected_Columns) },
			{ "Reset_To_Default", LocalizeString(AMMessages.UI_Reset_To_Default) },
			{ "Confirm", LocalizeString(AMMessages.UI_Confirm) },
			{ "Confirm_Reset_Text", LocalizeString(AMMessages.UI_Confirm_Reset_Text) },
			{ "OK", LocalizeString(AMMessages.UI_OK) },
			{ "Cancel", LocalizeString(AMMessages.UI_Cancel) },
			{ "NO_RECORDS_TO_DISPLAY", LocalizeString(AMMessages.UI_NO_RECORDS_TO_DISPLAY) },
			{ "PRESET_NAME_hourAndDay", LocalizeString(AMMessages.UI_PRESET_NAME_hourAndDay) },
			{ "PRESET_NAME_monthAndYear", LocalizeString(AMMessages.UI_PRESET_NAME_monthAndYear) },
			{ "PRESET_NAME_weekAndDay", LocalizeString(AMMessages.UI_PRESET_NAME_weekAndDay) },
			{ "PRESET_NAME_weekAndMonth", LocalizeString(AMMessages.UI_PRESET_NAME_weekAndMonth) },
			{ "ResourceHistogram_Item_Tooltip", LocalizeString(AMMessages.UI_ResourceHistogram_Item_Tooltip) }
		};

		var sb = new StringBuilder();

		sb.AppendFormat("<script src={0}vendors.js></script>\n", clientSideAppsRoot);
		sb.Append(@"
<script>
");
		sb.AppendFormat("var __svg_icons_path = \"{0}/Content/svg_icons/\";\n", appRoot);
		sb.AppendFormat("var __site_root = \"{0}\";\n", appRoot);
		sb.Append(@"
window.ClientLocalizedStrings = {
");
		sb.AppendFormat("AM: {0}, \n", Newtonsoft.Json.JsonConvert.SerializeObject(ManufacturingStrings));
		sb.AppendFormat("currentLocale: \"{0}\", \n", System.Threading.Thread.CurrentThread.CurrentCulture.Name);
		sb.AppendFormat("noResultsFound: \"{0}\",\n", HttpUtility.HtmlDecode(localizedNoResults).Replace("\"", "\\\""));
		sb.AppendLine("lastUpdate: {");
		sb.AppendFormat("JustNow: \"{0}\",\n", HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.LastUpdateJustNow)).Replace("\"", "\\\""));
		sb.AppendFormat("MinsAgo: \"{0}\",\n", HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.LastUpdateMinsAgo)).Replace("\"", "\\\""));
		sb.AppendFormat("HoursAgo: \"{0}\",\n", HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.LastUpdateHoursAgo)).Replace("\"", "\\\""));
		sb.AppendFormat("DaysAgo: \"{0}\",\n", HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.LastUpdateDaysAgo)).Replace("\"", "\\\""));
		sb.AppendFormat("LongAgo: \"{0}\"\n", HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.LastUpdateLongAgo)).Replace("\"", "\\\""));
		sb.AppendLine("},");
		sb.AppendLine("payment: {");
		sb.AppendFormat("Amount: \"{0}\",\n", HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.PaymentAmount)).Replace("\"", "\\\""));
		sb.AppendFormat("TitlePay: \"{0}\",\n", HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.PaymentTitlePay)).Replace("\"", "\\\""));
		sb.AppendFormat("TitleCreateProfile: \"{0}\",\n", HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.PaymentTitleCreateProfile)).Replace("\"", "\\\""));
		sb.AppendFormat("TitlePayEFT: \"{0}\",\n", HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.PaymentTitlePayEFT)).Replace("\"", "\\\""));
		sb.AppendFormat("TitleCreateProfileEFT: \"{0}\",\n", HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.PaymentTitleCreateProfileEFT)).Replace("\"", "\\\""));
		sb.AppendFormat("CardNumber: \"{0}\",\n", HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.PaymentCardNumber)).Replace("\"", "\\\""));
		sb.AppendFormat("ExpirationDate: \"{0}\",\n", HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.PaymentExpirationDate)).Replace("\"", "\\\""));
		sb.AppendFormat("Cvc: \"{0}\",\n", HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.PaymentCvc)).Replace("\"", "\\\""));
		sb.AppendFormat("Name: \"{0}\",\n", HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.PaymentName)).Replace("\"", "\\\""));
		sb.AppendFormat("Phone: \"{0}\",\n", HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.PaymentPhone)).Replace("\"", "\\\""));
		sb.AppendFormat("Address: \"{0}\",\n", HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.PaymentAddress)).Replace("\"", "\\\""));
		sb.AppendFormat("AddressLine1: \"{0}\",\n", HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.PaymentAddressLine1)).Replace("\"", "\\\""));
		sb.AppendFormat("AddressLine2: \"{0}\",\n", HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.PaymentAddressLine2)).Replace("\"", "\\\""));
		sb.AppendFormat("Email: \"{0}\",\n", HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.PaymentEmail)).Replace("\"", "\\\""));
		sb.AppendFormat("City: \"{0}\",\n", HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.PaymentCity)).Replace("\"", "\\\""));
		sb.AppendFormat("State: \"{0}\",\n", HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.PaymentState)).Replace("\"", "\\\""));
		sb.AppendFormat("Zip: \"{0}\",\n", HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.PaymentZip)).Replace("\"", "\\\""));
		sb.AppendFormat("Pay: \"{0}\",\n", HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.PaymentPay)).Replace("\"", "\\\""));
		sb.AppendFormat("Save: \"{0}\",\n", HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.PaymentSave)).Replace("\"", "\\\""));
		sb.AppendFormat("ContactInfo: \"{0}\",\n", HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.PaymentContactInfo)).Replace("\"", "\\\""));
		sb.AppendLine("},");
		sb.AppendLine("tree: {");
		sb.AppendFormat("AddSibling: \"{0}\",\n", HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.TreeAddSibling)).Replace("\"", "\\\""));
		sb.AppendFormat("AddChild: \"{0}\",\n", HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.TreeAddChild)).Replace("\"", "\\\""));
		sb.AppendFormat("Rename: \"{0}\",\n", HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.TreeRename)).Replace("\"", "\\\""));
		sb.AppendFormat("Delete: \"{0}\",\n", HttpUtility.HtmlDecode(PX.Data.PXMessages.LocalizeNoPrefix(PX.Web.UI.Msg.TreeDelete)).Replace("\"", "\\\""));
		sb.AppendLine("}}");

		sb.AppendLine("window.globalControlsModules={ workflowDiagram: true, manufacturingDiagram: true };");
		sb.AppendLine("window.readyForAurelia=true;");
		sb.Append("</script>");
		sb.AppendFormat(@"<!--{0}-->", System.DateTime.UtcNow);

		sb.AppendFormat("<script src={0}app.js></script>\n", clientSideAppsRoot);

		var result = sb.ToString();

		//HttpContext.Current.Cache.Insert(cacheKey, result, new System.Web.Caching.CacheDependency(bundleFiles), System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration);

		return result;
	}
}
