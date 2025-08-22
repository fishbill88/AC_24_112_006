using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using PX.CS;
using PX.Common;
using PX.Data;
using PX.Web.UI;
using Messages = PX.Objects.Common.Messages;

public partial class Page_CS205020 : PXPage
{
	public PXSelect<CSScreenAttribute, Where<CSScreenAttribute.screenID, Equal<Required<CSScreenAttribute.screenID>>,
		And<CSScreenAttribute.attributeID, Equal<Required<CSScreenAttribute.attributeID>>,
		And<CSScreenAttribute.typeValue, Equal<Required<CSScreenAttribute.typeValue>>>>>> udfTypedAttribute;
	public PXSelect<CSScreenAttribute, Where<CSScreenAttribute.screenID, Equal<Required<CSScreenAttribute.screenID>>,
		And<CSScreenAttribute.attributeID, Equal<Required<CSScreenAttribute.attributeID>>>>> udfAttributes;

	PXDropDown newDD = null;
	PXSelector newSel = null;
	PXFieldState selState = null;

	private const string AttributeID = nameof(CSScreenAttribute.AttributeID);
	private const string AttributeName = "Name";
	private const string Required = nameof(CSScreenAttribute.Required);
	private const string DefaultValue = nameof(CSScreenAttribute.DefaultValue);
	private const string Hidden = nameof(CSScreenAttribute.Hidden);
	private const string ScreenSettings = nameof(CSAttributeMaint2.ScreenSettings);
	private const string AllValue = "ALL";
	private const string AllLabel = "<All>";
	private const string Visibility = "visibility";

	protected override void OnPreLoad(EventArgs e)
	{
		base.OnPreLoad(e);
		// we need this here in order to generate grid columns
		if (selState != null) (newSel as IFieldEditor).SynchronizeState(selState);
	}

	protected override void OnInitComplete(EventArgs e)
	{
		base.OnInitComplete(e);

		form.ControlsCreating += Form_ControlsCreating;
		form.DataBinding += Form_DataBinding;
		var prms = Session[CSAttributeMaint2.SessionKey] as CSAttributeMaint2.ControlParams;
		WebControl templateContainer = form.Items.Count > 0 ? form.Items[1].TemplateContainer : null;

		if (templateContainer != null)
		{
			newDD = ControlHelper.FindControl(templateContainer, nameof(newDD)) as PXDropDown;
			newSel = ControlHelper.FindControl(templateContainer, nameof(newSel)) as PXSelector;
		}

		var cache = ds.DataGraph.Views[ScreenSettings].Cache;
		var scrn = cache.Current as AttribParams;

		if (scrn != null)
		{
			var oldScrnID = scrn.ScreenID;
			if (null != prms && !string.IsNullOrEmpty(prms.ScreenId))
			{
				cache.SetValue<AttribParams.screenID>(cache.Current, prms.ScreenId);
				Session[sessionKey] = prms.ScreenId;
			}
			else if (string.IsNullOrEmpty(scrn.ScreenID))
			{
				cache.SetValue<AttribParams.screenID>(cache.Current, Session[sessionKey]);
			}
			if (scrn.ScreenID != oldScrnID) scrn.TypeName = string.Empty;
		}

		if (prms is CSAttributeMaint2.BoundParams bndPrms && !string.IsNullOrEmpty(bndPrms.UDFTypeField))
		{
			string viewName = CSAttributeMaint2.CreateViewName(prms.ScreenId, bndPrms.ViewName);
			var siteMapNode = PXSiteMap.Provider.FindSiteMapNodeByScreenID(prms.ScreenId);
			var type = System.Web.Compilation.BuildManager.GetType(siteMapNode.GraphType, false);
			var graph = PXGraph.CreateInstance(type);
			udfTypedAttribute = new PXSelect<CSScreenAttribute,
				Where<CSScreenAttribute.screenID, Equal<Required<CSScreenAttribute.screenID>>,
				And<CSScreenAttribute.attributeID, Equal<Required<CSScreenAttribute.attributeID>>,
				And<CSScreenAttribute.typeValue, Equal<Required<CSScreenAttribute.typeValue>>>>>>
				(graph);
			udfAttributes = new PXSelect<CSScreenAttribute,
				Where<CSScreenAttribute.screenID, Equal<Required<CSScreenAttribute.screenID>>,
				And<CSScreenAttribute.attributeID, Equal<Required<CSScreenAttribute.attributeID>>>>>
				(graph);
			var dataSource = GetDefaultDataSource(this);

			var view = new PXView(dataSource.DataGraph, true, BqlCommand.CreateInstance(typeof(Select<>),
								  graph.GetItemType(bndPrms.ViewName)), new PXSelectDelegate(delegate ()
			{
				return Enumerable.Empty<object>();
			}));

			dataSource.DataGraph.Views.Add(viewName, view);

			if (prms is CSAttributeMaint2.ComboBoxParams dd)
			{
				newDD.DataMember = viewName;
				newDD.Size = "XXL";
				newDD.Hidden = false;
				newSel.DataField = string.Empty;

				var state = dataSource.DataGraph.Views[viewName].Cache.GetStateExt(null, bndPrms.UDFTypeField) as PXStringState;
				state.ValueLabelDic[AllLabel] = AllValue;

				if (!string.IsNullOrEmpty(bndPrms.ViewName))
					state.DisplayName = bndPrms.ViewName;

				(newDD as IFieldEditor).SynchronizeState(state);
				newDD.Items.Insert(0, new PXListItem(AllLabel, AllValue));
				newDD.ValueChanged += (object sender, EventArgs e1) =>
				{
					if (scrn != null && sender is PXDropDown dDown)
						scrn.TypeName = dDown.Value as string;
				};
				newDD.DataBind();
			}
			else
			{
				newSel.DataMember = viewName;
				newSel.Hidden = false;
				newDD.DataField = string.Empty;

				newSel.TextChanged += (object sender, EventArgs e1) =>
				{
					if (scrn != null && sender is PXSelector selector)
						scrn.TypeName = selector.Value as string;
				};
				var state = dataSource.DataGraph.Views[viewName].Cache.GetStateExt(null, bndPrms.UDFTypeField) as PXFieldState;
				(newSel as IFieldEditor).SynchronizeState(this.selState = state);
				newSel.Value = scrn.TypeName;
				newSel.DataBind();
			}
		}
		else if(null != prms)
		{
			var siteMapNode = PXSiteMap.Provider.FindSiteMapNodeByScreenID(prms.ScreenId);
			var type = System.Web.Compilation.BuildManager.GetType(siteMapNode.GraphType, false);
			var graph = PXGraph.CreateInstance(type);
			udfTypedAttribute = new PXSelect<CSScreenAttribute,
					Where<CSScreenAttribute.screenID, Equal<Required<CSScreenAttribute.screenID>>,
					And<CSScreenAttribute.attributeID, Equal<Required<CSScreenAttribute.attributeID>>,
					And<CSScreenAttribute.typeValue, Equal<Required<CSScreenAttribute.typeValue>>>>>>
					(graph);
			udfAttributes = new PXSelect<CSScreenAttribute,
				Where<CSScreenAttribute.screenID, Equal<Required<CSScreenAttribute.screenID>>,
				And<CSScreenAttribute.attributeID, Equal<Required<CSScreenAttribute.attributeID>>>>>
				(graph);
		}

		var visibility = ControlHelper.FindControl(form.Items[1].TemplateContainer, Visibility) as PXGrid;
		var visibilityItems = visibility.Levels.Items.FirstOrDefault().Columns.Items;
		visibilityItems.FirstOrDefault(n => n.DataField == AttributeName).Header.Text =
			PXLocalizer.Localize(UserDefinedFieldsMessages.UserDefinedAttribute, typeof(UserDefinedFieldsMessages).FullName);
		visibilityItems.FirstOrDefault(n => n.DataField == Required).Header.Text =
			PXLocalizer.Localize(UserDefinedFieldsMessages.Required, typeof(UserDefinedFieldsMessages).FullName);
		visibilityItems.FirstOrDefault(n => n.DataField == Hidden).Header.Text =
			PXLocalizer.Localize(UserDefinedFieldsMessages.Hidden, typeof(UserDefinedFieldsMessages).FullName);
		visibilityItems.FirstOrDefault(n => n.DataField == DefaultValue).Header.Text =
			PXLocalizer.Localize(UserDefinedFieldsMessages.Default, typeof(UserDefinedFieldsMessages).FullName);
		if (ControlHelper.IsCallbackOwner(visibility) && ControlHelper.GetCommandName(this)=="Refresh")
		{
			Form_DataBinding(this, EventArgs.Empty);
		}
	}

	private void Form_DataBinding(object sender, EventArgs e)
	{
		var cache = ds.DataGraph.Views[ScreenSettings].Cache;
		var scrn = cache.Current as AttribParams;
		if (scrn.ScreenID != null || PXGraph.GeneratorIsActive)
		{
			if (form.Items.Count > 1)
			{
				var visibility = ControlHelper.FindControl(form.Items[1].TemplateContainer, Visibility) as PXGrid;
				if (null != visibility)
				{
					string ddVal = null;
					if (null != newDD)
					{
						if (Context != null && Context.Request != null && Context.Request.Form != null )
						{
							ddVal = Context.Request.Form[newDD.ClientID.Replace("_", "$") + "$text"];
						}
						if (!string.IsNullOrEmpty(ddVal))
						{
							if (AllLabel == ddVal) scrn.TypeName = string.Empty;
						}

					}

					string screen = PXGraph.GeneratorIsActive ? PXSiteMap.DefaultScreenID : scrn.ScreenID.Replace(".", "");

					if (newDD != null && newDD.SelectedValue != null)
						visibility.DataSource = PXAttribPanel.GetVisibility(screen, newDD.SelectedValue);
					else if (newSel != null && newSel.Text != null)
						visibility.DataSource = PXAttribPanel.GetVisibility(screen, newSel.Text);

					visibility.Columns[Hidden].DataType = TypeCode.Boolean;
					visibility.Columns[Required].DataType = TypeCode.Boolean;
					visibility.Columns[DefaultValue].DataType = TypeCode.String;
					visibility.DataBind();
				}
			}
		}
	}

	const string sessionKey = "LastAttribScreen";
	private void Form_ControlsCreating(object sender, System.ComponentModel.CancelEventArgs e)
	{
		var cache = ds.DataGraph.Views[ScreenSettings].Cache;
		var scrn = cache.Current as AttribParams;
		if (scrn.ScreenID != null || PXGraph.GeneratorIsActive)
		{
			string screen = PXGraph.GeneratorIsActive ? PXSiteMap.DefaultScreenID : scrn.ScreenID.Replace(".", "");
			var controls = new List<Control>();
			var panel = new PXAttribPanel() { ID = "atPanel" };
			panel.Controls.Add(PXAttribPanel.CreateDesignAttribTable(screen, controls, this));
			var ctrls = form.Items[0].TemplateContainer.Controls;
			bool found = false;
			foreach (Control c in ctrls)
			{
				if (c.ID == panel.ID && c is PXAttribPanel)
				{
					panel = c as PXAttribPanel;
					found = true;
					break;
				}
			}
			if (!found)
				form.Items[0].TemplateContainer.Controls.Add(panel);

			var node = PXSiteMap.Provider.FindSiteMapNodeByScreenID(screen);
			if (null != node) panel.ScreenTitle = node.Title;

			if (form.Items.Count > 1)
			{
				var visibility = ControlHelper.FindControl(form.Items[1].TemplateContainer, Visibility) as PXGrid;
				if (null != visibility)
				{
					visibility.Columns[Hidden].DataType = TypeCode.Boolean;
					visibility.Columns[Required].DataType = TypeCode.Boolean;
					visibility.Columns[DefaultValue].DataType = TypeCode.String;
					visibility.RefetchRow += Visibility_RefetchRow;
					visibility.RowUpdating += Visibility_RowUpdating;
				}
			}
		}
	}

	private void Visibility_RowUpdating(object sender, PXDBUpdateEventArgs e)
	{
		UpdateVisibility(e.NewValues);
	}


	private void Visibility_RefetchRow(object sender, PXDBUpdateEventArgs e)
	{
		UpdateVisibility(e.NewValues);
	}

	private void UpdateVisibility(IOrderedDictionary newValues)
	{
		var cache = ds.DataGraph.Views[ScreenSettings].Cache;
		var attribParams = cache.Current as AttribParams;
		(bool needUpdate, bool needUpdateForOtherScreens) = GetUpdateConditions(newValues, attribParams);

		if (!needUpdate)
			return;

		string screenID = attribParams.ScreenID,
			   typeValue = attribParams.TypeName ?? string.Empty,
			   attributeID = (string)newValues[AttributeID],
			   defaultValue = (string)newValues[DefaultValue] ?? string.Empty;

		UpdateAttributeSettings(screenID, typeValue, newValues);

		if (needUpdateForOtherScreens)
			UpdateUdfDefaultOnRelatedScreens(attributeID, screenID, typeValue, defaultValue);
	}

	private (bool, bool) GetUpdateConditions(IOrderedDictionary newValues, AttribParams attribParams)
	{
		var attribute = GetAttribute(newValues[AttributeID].ToString().ToUpper(), attribParams);

		if (attribute == null)
			return (true, true);

		bool oldHidden = attribute.Hidden.Value,
			 newHidden = (bool)newValues[Hidden],
			 oldRequired = attribute.Required.Value,
			 newRequired = (bool)newValues[Required];

		string oldDefault = attribute.DefaultValue.NullIfWhitespace(),
			   newDefault = newValues[DefaultValue] as string;

		// Hidden UDFs should not be required
		if (newHidden && newRequired)
			newValues[Required] = newRequired = false;

		bool needUpdate = oldRequired != newRequired ||
						  oldHidden != newHidden ||
						  oldDefault != newDefault;
		bool needUpdateForOtherScreens = needUpdate && !string.Equals(oldDefault, newDefault, StringComparison.CurrentCulture);

		return (needUpdate, needUpdateForOtherScreens);
	}

	private void UpdateAttributeSettings(string screenID, string typeValue, IOrderedDictionary newValues)
	{
		string attributeID = newValues[AttributeID].ToString().ToUpper();

		if (string.Equals(typeValue, AllValue, StringComparison.CurrentCultureIgnoreCase))
			typeValue = string.Empty;

		var attribute = udfTypedAttribute.SelectSingle(screenID, attributeID, typeValue);

		if (attribute == null)
			InsertAttributeSettings(attributeID, screenID, typeValue, newValues);
		else
			UpdateAttributeSettings(attribute, newValues);
	}

	private void InsertAttributeSettings(string attributeID, string screenID, string typeValue, IOrderedDictionary newValues)
	{
		var commonAttribute = udfTypedAttribute.SelectSingle(screenID, attributeID, string.Empty);
		InsertAttributeSettings(attributeID, screenID, typeValue, commonAttribute,
			(bool)newValues[Hidden], (bool)newValues[Required], (string)newValues[DefaultValue]);
	}

	private void UpdateAttributeSettings(CSScreenAttribute attribute, IOrderedDictionary newValues)
	{
		if (!string.IsNullOrEmpty(attribute.TypeValue))
			UpdateVisibility(attribute, newValues);
		else
		{
			var itemsToDelete = udfAttributes.Select(attribute.ScreenID, attribute.AttributeID)
				.Select(attr => attr.GetItem<CSScreenAttribute>())
				.Where(attr => !string.IsNullOrEmpty(attr.TypeValue));

			if (!itemsToDelete.Any())
				UpdateVisibility(attribute, newValues);
			else
				RewriteVisibility(attribute, newValues, itemsToDelete);
		}
	}

	private void RewriteVisibility(CSScreenAttribute attribute, IOrderedDictionary newValues, IQueryable<CSScreenAttribute> itemsToDelete)
	{
		try
		{
			if (((CSAttributeMaint2)ds.DataGraph).ScreenSettings.Ask(Messages.Warning,
					Messages.UDFVisibilityChanging,
					MessageButtons.YesNo, MessageIcon.Warning) == WebDialogResult.Yes)
			{
				UpdateVisibility(attribute, newValues);

				foreach (var attr in itemsToDelete)
				{
					udfAttributes.Cache.PersistDeleted(attr);
				}
			}
		}
		catch (PXDialogRequiredException ex)
		{
			ex.DataSourceID = ds.ID;
			throw;
		}
	}

	private void UpdateVisibility(CSScreenAttribute attribute, IOrderedDictionary newValues) =>
		UpdateVisibility(attribute, (bool)newValues[Hidden], (bool)newValues[Required], (string)newValues[DefaultValue]);

	private void UpdateVisibility(CSScreenAttribute attribute, bool hidden, bool required, string defaultValue)
	{
		attribute.Hidden = hidden;
		attribute.Required = required;
		attribute.DefaultValue = defaultValue;
		udfTypedAttribute.Cache.Update(attribute);
		udfTypedAttribute.Cache.Persist(PXDBOperation.Update);
	}

	private CSScreenAttribute GetAttribute(string attributeID, AttribParams attribParams)
	{
		string screenID = attribParams.ScreenID,
			   typeValue = attribParams.TypeName ?? string.Empty;
		var attribute = udfTypedAttribute.SelectSingle(screenID, attributeID, typeValue);

		if (attribute == null && !string.IsNullOrEmpty(typeValue))
			attribute = udfTypedAttribute.SelectSingle(screenID, attributeID, string.Empty);

		return attribute;
	}

	private void UpdateUdfDefaultOnRelatedScreens(string attributeID, string screenID, string typeValue, string defaultValue)
	{
		var screens = GetScreensWithTheSameCaheType(screenID);

		foreach (var screen in screens)
		{
			var attribute = udfTypedAttribute.SelectSingle(screen, attributeID, typeValue);

			if (attribute != null)
				UpdateVisibility(attribute, attribute.Hidden.Value, attribute.Required.Value, defaultValue);
			else
				InsertAttributeSettings(attributeID, screen, typeValue, defaultValue);
		}
	}

	private void InsertAttributeSettings(string attributeID, string screenID, string typeValue, string defaultValue)
	{
		var commonAttribute = udfTypedAttribute.SelectSingle(screenID, attributeID, string.Empty);
		InsertAttributeSettings(attributeID, screenID, typeValue, commonAttribute,
								commonAttribute.Hidden.Value, commonAttribute.Required.Value, defaultValue);
	}

	private void InsertAttributeSettings(string attributeID, string screenID, string typeValue, CSScreenAttribute commonAttribute,
										bool hidden, bool required, string defaultValue)
	{
		CSScreenAttribute attribute = (CSScreenAttribute)udfTypedAttribute.Cache.Insert();

		attribute.ScreenID = screenID;
		attribute.AttributeID = attributeID;
		attribute.TypeValue = typeValue;
		attribute.Hidden = hidden;
		attribute.Required = required;
		attribute.DefaultValue = defaultValue;
		attribute.Column = commonAttribute.Column;
		attribute.Row = commonAttribute.Row;

		udfTypedAttribute.Cache.Persist(PXDBOperation.Insert);
	}

	private List<string> GetScreensWithTheSameCaheType(string screenID)
	{
		var sitemap = PXSiteMap.Provider.FindSiteMapNodeByScreenID(screenID);
		var cacheType = GraphHelper.GetPrimaryCache(sitemap.GraphType)?.CacheType;

		if (cacheType == null)
			return new List<string>();

		List<string> screens = KeyValueHelper.Def?.GetScreensWithAttributesForTable(cacheType) ?? new List<string>();
		screens.Remove(screenID);

		return screens;
	}

	protected void visibility_RowDataBound(object sender, PXGridRowEventArgs e)
	{
		var state = (e.Row.DataItem as VisibilityInfo).State;
		switch (state.DataType.Name)
		{
			case nameof(Boolean):
				e.Row.Cells[DefaultValue].DataType = TypeCode.Boolean;
				break;
			case nameof(DateTime):
				e.Row.Cells[DefaultValue].DataType = TypeCode.DateTime;
				break;
			case nameof(String):
				e.Row.Cells[DefaultValue].DataType = TypeCode.String;
				var sState = ((PXStringState)state);
				if (sState.ValueLabelDic != null)
				{
					foreach (var item in sState.ValueLabelDic)
						e.Row.Cells[DefaultValue].ValueItems.Items.Add(new PXValueItem(item.Key, item.Value));
					e.Row.Cells[DefaultValue].ValueItems.MultiSelect = sState.MultiSelect;
					e.Row.Cells[DefaultValue].ValueField = sState.ValueField;
				}
				break;
			case nameof(Object):
				e.Row.Cells[DefaultValue].DataType = TypeCode.Object;
				if (state is PXSelectorState selState)
				{
					var graph = (sender as PXGrid).DataGraph;
					graph = udfTypedAttribute.View.Graph;
					var type = System.Web.Compilation.PXBuildManager.GetType(selState.SchemaObject, true);
					if (graph != null)
					{
						var cache = graph.Caches[type];
						var state1 = cache.GetStateExt(null, selState.SchemaField) as PXFieldState;
						e.Row.Cells[DefaultValue].ViewName = state1.ViewName;
						e.Row.Cells[DefaultValue].ValueField = selState.SchemaField;
					}
					else
					{
						e.Row.Cells[DefaultValue].ViewName = selState.SchemaObject;
						e.Row.Cells[DefaultValue].ValueField = selState.SchemaField;
					}
				}
				break;
		}
	}

	protected void visibility_SyncCellState(object sender, PXSyncCellStateEventArgs e)
	{
		var cache = ds.DataGraph.Views[ScreenSettings].Cache;
		var scrn = cache.Current as AttribParams;
		PXSelectorState udfAttrState = null;
		if (scrn.ScreenID != null || PXGraph.GeneratorIsActive)
		{
			string screen = PXGraph.GeneratorIsActive ? PXSiteMap.DefaultScreenID : scrn.ScreenID.Replace(".", "");
			List<VisibilityInfo> l = null;

			if (newDD != null && newDD.SelectedValue != null)
			{
				l = PXAttribPanel.GetVisibility(screen, newDD.SelectedValue);
			}
			else if (newSel != null && newSel.Text != null)
			{
				l = PXAttribPanel.GetVisibility(screen, newSel.Text);
			}

			if (l == null) return;
			udfAttrState = l.Where(n => n.AttributeID == e.RowValues.Value.ToString()).Select(n => n.State).FirstOrDefault() as PXSelectorState;
			if (udfAttrState == null) return;
		}

		var graph = udfTypedAttribute.View.Graph;
		var type = System.Web.Compilation.PXBuildManager.GetType(udfAttrState.SchemaObject, true);
		cache = graph.Caches[type];
		var state1 = cache.GetStateExt(null, udfAttrState.SchemaField) as PXStringState;

		e.State = state1;
	}
}
