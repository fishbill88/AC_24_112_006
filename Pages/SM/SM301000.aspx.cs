using System;
using System.Web;

public partial class Page_SM301000 : PX.Web.UI.PXPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        this.ClientScript.RegisterClientScriptInclude(this.GetType(), "jq", VirtualPathUtility.ToAbsolute("~/Scripts/jquery-3.6.0.min.js"));
        this.ClientScript.RegisterClientScriptInclude(this.GetType(), "jqsr", VirtualPathUtility.ToAbsolute("~/Scripts/signalr.js"));
    }
}
