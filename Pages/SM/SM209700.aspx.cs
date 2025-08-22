using PX.Web.UI;
using System;

public partial class Page_SM209700 : PXPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		var textEdit = form.FindControl("edFileId") as PXTextEdit;
		if (textEdit == null)
		{
			return;
		}

		var variable = "fileControlId";
		var script = string.Format("let {0} = '{1}';", variable, textEdit.ClientID);
		Page.ClientScript.RegisterClientScriptBlock(GetType(), variable, script, true);
	}
}
