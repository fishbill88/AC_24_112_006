using System;
using System.Web.UI.WebControls;
using PX.Web.UI;
using PX.Objects.CA;
using PX.Data;
using System.Drawing;

public partial class Page_CA204000 : PX.Web.UI.PXPage
{
	protected void Page_Init(object sender, EventArgs e)
	{
		RegisterStyle("CssRecordsHeader", null, null, true);
	}

	private void RegisterStyle(string name, Color? backColor, Color? foreColor, bool bold)
	{
		Style style = new Style();
		if (backColor != null) style.BackColor = backColor.Value;
		if (foreColor != null) style.ForeColor = foreColor.Value;
		style.Font.Bold = bold;
		Page.Header.StyleSheet.CreateStyleRule(style, this, "." + name);
	}

	private void RegisterStyle(string name)
	{
		Style style = new Style();
		
		Page.Header.StyleSheet.CreateStyleRule(style, this, "." + name);
	}

	protected void plugInSettingsGrid_OnRowDataBound(object sender, PX.Web.UI.PXGridRowEventArgs e)
	{
		ACHPlugInParameter parameter;

		PXResult record = e.Row.DataItem as PXResult;
		if (record != null)
		{
			parameter = (ACHPlugInParameter)record[typeof(ACHPlugInParameter)];
		}
		else
		{
			parameter = e.Row.DataItem as ACHPlugInParameter;
		}

		if (parameter == null)
			return;

		if (parameter.IsGroupHeader == true)
		{
			e.Row.Style.CssClass = "CssRecordsHeader GridActiveRow";
		}
		else
		{
			if (parameter.Required == true)
			{
				e.Row.Cells["ParameterCode"].Style.CssClass = "GridHeader ParamRequired";
			}
			else
			{
				e.Row.Cells["ParameterCode"].Style.CssClass = "GridHeader";
			}
		}
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		if (!this.Page.IsCallback)
		{
			if (this.tab.FindControl("plugInSettingsGrid") is PXGrid grid)
				this.Page.ClientScript.RegisterClientScriptBlock(GetType(), "gridID", "var gridID=\"" + grid.ClientID + "\";", true);
		}
	}

	protected void edFormulaExpression(object sender, PXCallBackEventArgs e)
	{
		PaymentMethodMaint graph = this.ds.DataGraph as PaymentMethodMaint;
		if (graph == null) return;

		string[] attributes = graph.GetAddendaInfoFields();
		e.Result = string.Join(";", attributes);
	}
}
