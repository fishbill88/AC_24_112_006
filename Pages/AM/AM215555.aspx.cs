using System;
using System.Web.UI.WebControls;
using PX.Web.UI;

public partial class Page_AM215555 : PX.Web.UI.PXPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
	}

    const string BoldTextStyle = "BoldText";

    protected void Page_Init(object sender, EventArgs e)
    {
        Style style = new Style();
        style.Font.Bold = true;
        this.Page.Header.StyleSheet.CreateStyleRule(style, this, "." + BoldTextStyle);

        this.Master.PopupWidth = 960;
        this.Master.PopupHeight = 600;
    }
    
    protected void form_OnRowUpdated(object sender, PXDBUpdatedEventArgs e)
    {
	    var mdControl = (PXManufacturingDiagram) this.WorkflowFictiveDiagram.DataControls["mdiagram"];
	    mdControl.Value = Guid.NewGuid().ToString();
    }
}
