using System.Web;
using Customization;

public partial class Api_ProcessPages : System.Web.UI.Page
{
	public override void ProcessRequest(HttpContext context)
	{
		var r = new PXResponse { 
			HtmlLog =
				m => { 
					context.Response.Write(m);
					context.Response.Flush();
				
				} };

		PXResponse.Current = r;
		//PXAspxCleanup.Run3();
		PXReportValidator.Run();

		PXResponse.Current = null;


		//base.ProcessRequest(context);
	}

}
