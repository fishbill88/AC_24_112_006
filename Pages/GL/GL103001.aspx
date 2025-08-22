<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="GL103001.aspx.cs" Inherits="Page_GL103001"
	Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" Width="100%" runat="server" PrimaryView="AccountRecords" TypeName="PX.Objects.GL.Consolidation.ConsolAccountMaint"  Visible="True">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Save" CommitChanges="True" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXGrid ID="grid" runat="server" Height="350px" Width="100%" AdjustPageSize="Auto"
		SkinID="Primary" AllowPaging="True" AllowSearch="True" FastFilterFields="AccountCD,Description" SyncPosition="True">
		<Levels>
			<px:PXGridLevel DataMember="AccountRecords">
				<RowTemplate>
					<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
					<px:PXTextEdit ID="edAccountCD" runat="server" DataField="AccountCD" />
					<px:PXTextEdit ID="edDescription" runat="server" DataField="Description" />
				</RowTemplate>
				<Columns>
					<px:PXGridColumn DataField="AccountCD" />
					<px:PXGridColumn DataField="Description" />
				</Columns>
				<Styles>
					<RowForm Height="250px">
					</RowForm>
				</Styles>
			</px:PXGridLevel>
		</Levels>
		<Layout FormViewHeight="250px" />
		<AutoSize Container="Window" Enabled="True" MinHeight="200" />
		<Mode AllowFormEdit="True" AllowUpload="True" />
		<CallbackCommands>
			<Save PostData="Content" />
		</CallbackCommands>
	</px:PXGrid>
</asp:Content>
