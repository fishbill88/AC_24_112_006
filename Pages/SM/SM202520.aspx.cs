using System;
using PX.SM;
using PX.Web.UI;

public partial class Page_SM210001 : PX.Web.UI.PXPage
{
	protected override void OnInitComplete(EventArgs e)
	{
		base.OnInitComplete(e);
		if (!string.IsNullOrEmpty(Request.QueryString["ctrl"]))
		{
			var renderer = JSManager.GetRenderer(this);
			JSManager.RegisterVar(renderer, this.GetType(), "fileLinkDialog", Request.QueryString["ctrl"]);
		}
		var graph = ds.DataGraph as UploadFileInq;//.Views["SelectedScreen"].Cache;
		if (null != graph)
		{
			var cache = graph.FileDialog.Cache;
			if (null != cache && cache.Current != null)
			{
				var mode = cache.Current as PX.SM.FileDialog;
				if (null != mode)
				{
					if (mode.Entity == null)
					{
						Guid g;
						if (Guid.TryParse(Request.QueryString["note"], out g))
							mode.Entity = g;
					}
					if (mode.Entity != null)
					{
						graph.Actions["GetFile"].SetVisible(false);
						graph.Actions["GetFileLink"].SetVisible(false);
						graph.Actions["DeleteFile"].SetVisible(false);
					}
					else
					{
						graph.Actions["AddLink"].SetVisible(false);
						graph.Actions["AddLinkClose"].SetVisible(false);
					}
				}
			}
			//if (fromFileDialog)
			//{
			//	graph.Actions["GetFile"].SetVisible(false);
			//	graph.Actions["GetFileLink"].SetVisible(false);
			//	graph.Actions["DeleteFile"].SetVisible(false);
			//}
			//else
			//{
			//	graph.Actions["AddLink"].SetVisible(false);
			//	graph.Actions["AddLinkClose"].SetVisible(false);
			//}
		}
		//var scrn = cache.Current as PX.Data.AttribParams;

	}
}
