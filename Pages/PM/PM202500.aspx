<%@ Page
	Language="C#"
	MasterPageFile="~/MasterPages/ListView.master"
	AutoEventWireup="true"
	ValidateRequest="false"
	CodeFile="PM202500.aspx.cs"
	Inherits="Page_PM202500"
	Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource
		ID="ds"
		Width="100%"
		runat="server"
		TypeName="PX.Objects.PM.PMProjectGroupMaint"
		PrimaryView="ProjectGroups"
		Visible="true">
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXGrid
		ID="grid"
		runat="server"
		Height="400px"
		Width="100%"
		SyncPosition="true"
		AllowPaging="true"
		AllowSearch="true"
		AdjustPageSize="Auto"
		DataSourceID="ds"
		SkinID="Primary">
		<Levels>
			<px:PXGridLevel DataMember="ProjectGroups">
				<RowTemplate>
					<px:PXLayoutRule runat="server" StartColumn="true" LabelsWidth="M" ControlSize="XM" />
					<px:PXMaskEdit ID="editProjectGroupID" runat="server" DataField="ProjectGroupID" />
					<px:PXTextEdit ID="editDescription" runat="server" DataField="Description" />
					<px:PXCheckBox ID="checkIsActive" runat="server" DataField="IsActive" />
				</RowTemplate>
				<Columns>
					<px:PXGridColumn DataField="ProjectGroupID" CommitChanges="true" />
					<px:PXGridColumn DataField="Description" />
					<px:PXGridColumn DataField="IsActive" TextAlign="Center" Type="CheckBox" />
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<Mode AllowUpload="true" />
		<AutoSize Container="Window" Enabled="true" MinHeight="200" />
	</px:PXGrid>
</asp:Content>