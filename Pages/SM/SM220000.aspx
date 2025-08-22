<%@ Page Language="C#" MasterPageFile="~/MasterPages/TabView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SM220000.aspx.cs"
	Inherits="Page_SM220000" Title="Graph Preference" %>

<%@ MasterType VirtualPath="~/MasterPages/TabView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.MSGraph.GraphSetupMaint" PrimaryView="GraphSetup">
		<CallbackCommands>
			<px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
			<px:PXDSCallbackCommand CommitChanges="True" Name="InsertRights" Visible="False" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="formGraphSetup" runat="server" DataSourceID="ds" Width="100%" DataMember="GraphSetup" AllowCollapse="False" >
		<Template>
			<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Microsoft Graph API" LabelsWidth="M" ControlSize="XL" />
			<px:PXTextEdit ID="edTenantID" runat="server" DataField="TenantID" />
			<px:PXTextEdit ID="edClientID" runat="server" DataField="ClientID" />
			<px:PXTextEdit ID="edClientSecret" runat="server" DataField="ClientSecret" />
			<px:PXNumberEdit ID="edTimeout" runat="server" DataField="Timeout" />
		</Template>
		<AutoSize Container="Window" Enabled="True" />
	</px:PXFormView>
	<px:PXGrid ID="gridGraphAccessRights" runat="server" DataSourceID="ds" Width="100%" Height="100%" Caption="Microsoft Graph Access Rights" SkinID="Details">
		<Mode AllowAddNew="False" AllowDelete="False" />
		<ActionBar>
			<Actions>
				<Refresh ToolBarVisible="False"></Refresh>
				<AddNew ToolBarVisible="False"></AddNew>
				<Delete ToolBarVisible="False"></Delete>
			</Actions>
			<CustomItems>
				<px:PXToolBarButton Key="cmdInsertRights">
					<AutoCallBack Command="InsertRights" Target="ds" />
				</px:PXToolBarButton>
			</CustomItems>	
		</ActionBar>	
		<Levels>
			<px:PXGridLevel DataMember="AccessRights">
				<RowTemplate>
					<px:PXDropDown ID="edApplicableFor" runat="server" DataField="ApplicableFor" AllowMultiSelect="True" />  
				</RowTemplate>
				<Columns>
					<px:PXGridColumn DataField="Enabled" TextAlign="Center" Type="CheckBox" />
					<px:PXGridColumn DataField="AccessRight" CommitChanges="True" />
					<px:PXGridColumn DataField="Description" Width="400" />
					<px:PXGridColumn DataField="AdminConsent" TextAlign="Center" Type="CheckBox" />
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="180" />
	</px:PXGrid>
</asp:Content>