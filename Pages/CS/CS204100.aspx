<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="CS204100.aspx.cs" Inherits="Page_CS204100" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" HeaderDescriptionField="Name" runat="server" Visible="True" Width="100%" PrimaryView="SalesTerritory" TypeName="PX.Objects.CS.SalesTerritoryMaint">
		<CallbackCommands>
			<px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
			<px:PXDSCallbackCommand Name="CopyPaste" Visible="false" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" 
		DataMember="SalesTerritory" Caption="Sales Territory" 
		DefaultControlID="edSalesTerritoryID">
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" ControlSize="M" LabelsWidth="M" />
			<px:PXSelector ID="edSalesTerritoryID" runat="server" DataField="SalesTerritoryID" AutoRefresh ="True"/>
			<px:PXTextEdit ID="edName" runat="server" DataField="Name" />
			<px:PXCheckBox ID="edIsActive" runat="server" DataField="IsActive" />

			<px:PXLayoutRule runat="server" StartColumn="True" ControlSize="M" LabelsWidth="M" />
			<px:PXDropDown ID="edSalesTerritoryType" runat="server" DataField="SalesTerritoryType" AutoRefresh ="True" CommitChanges="true" />
			<px:PXSelector ID="edCountryID" runat="server" DataField="CountryID" AutoRefresh ="True" AllowEdit="true" CommitChanges="true" />
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">

	<px:PXGrid ID="gridStates" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%"
			SkinID="Details" Caption="States" CaptionVisible="false" FilesIndicator="false" NoteIndicator="false" AllowPaging="false"
			FastFilterFields="StateID,Name" >
		<Levels>
			<px:PXGridLevel DataMember="CountryStates" >
				<Columns>
					<px:PXGridColumn DataField="Selected" Type="CheckBox" AllowCheckAll="true" CommitChanges="true" />
					<px:PXGridColumn DataField="StateID" />
					<px:PXGridColumn DataField="Name" />
					<px:PXGridColumn DataField="SalesTerritoryID" Visible="false" SyncVisible="false" />
					<px:PXGridColumn DataField="SalesTerritoryID_Description" />
				</Columns>
				<Layout FormViewHeight="" />
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
		<ActionBar>
			<Actions>
				<FilterBar ToolBarVisible="Top" GroupIndex="3" />
				<AddNew Enabled="false" ToolBarVisible="False"/>
				<Delete Enabled="false" ToolBarVisible="False"/>
			</Actions>
		</ActionBar>
	</px:PXGrid>

	<px:PXGrid ID="gridCountries" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%"
			SkinID="Details" Caption="Countries" CaptionVisible="false" FilesIndicator="false" NoteIndicator="false" AllowPaging="false"
			FastFilterFields="CountryID,Description" >
		<Levels>
			<px:PXGridLevel DataMember="Countries" >
				<Columns>
					<px:PXGridColumn DataField="Selected" Type="CheckBox" AllowCheckAll="true" CommitChanges="true" />
					<px:PXGridColumn DataField="CountryID" LinkCommand="<stub>" />
					<px:PXGridColumn DataField="Description" />
					<px:PXGridColumn DataField="SalesTerritoryID" Visible="false" SyncVisible="false" />
					<px:PXGridColumn DataField="SalesTerritoryID_Description" />
				</Columns>
				<RowTemplate>
					<px:PXSelector ID="edCountryID" runat="server" DataField="CountryID" />
				</RowTemplate>
				<Layout FormViewHeight="" />
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
		<ActionBar>
			<Actions>
				<FilterBar ToolBarVisible="Top" GroupIndex="3"/>
				<AddNew Enabled="false" ToolBarVisible="False"/>
				<Delete Enabled="false" ToolBarVisible="False"/>
			</Actions>
		</ActionBar>
	</px:PXGrid>

	<px:PXGrid ID="gridEmpty" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%"
		SkinID="Details" CaptionVisible="false" FilesIndicator="false" NoteIndicator="false" AllowPaging="false"
		FastFilterFields="">
		<Levels>
			<px:PXGridLevel DataMember="EmptyView">
				<Columns>
					<px:PXGridColumn Width="0" />
				</Columns>
				<Layout FormViewHeight="" />
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
		<ActionBar>
			<Actions>
				<FilterBar ToolBarVisible="Top" GroupIndex="3" />
				<AddNew Enabled="false" ToolBarVisible="False" />
				<Delete Enabled="false" ToolBarVisible="False" />
				<ExportExcel Enabled="false" />
				<AdjustColumns Enabled="false" />
				<Refresh Enabled="false" />
			</Actions>
		</ActionBar>
	</px:PXGrid>

</asp:Content>
