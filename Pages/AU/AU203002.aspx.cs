using System;
using System.Web;
using PX.Web.Customization;

public partial class  Page_AU203002 : PX.Web.UI.PXPage
{
	protected override void OnInit(EventArgs e)
	{

		this.Master.FindControl("usrCaption").Visible = false;
		ProjectBrowserMaint.InitSessionFromQueryString(HttpContext.Current);
		base.OnInit(e);
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		
		
		
	}

	/// <summary>
	/// The page PreRenderComplete event handler.
	/// </summary>
	protected override void OnPreRenderComplete(EventArgs e)
	{
		string query = ProjectBrowserMaint.ContextTableName;
		if (!string.IsNullOrEmpty(query))
		{
			this.ClientScript.RegisterStartupScript(this.GetType(), "query",
				string.Format("\nvar __queryString = '{0}={1}'; ", "TableName", query.Replace('+', '*')), true);
		}
		base.OnPreRenderComplete(e);
	}
}
