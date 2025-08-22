<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="GL103004.aspx.cs" Inherits="Page_GL103004"
	Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" Width="100%" runat="server" PrimaryView="OrganizationRecords" TypeName="PX.Objects.GL.Consolidation.ConsolOrganizationMaint"  Visible="True">
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXGrid ID="grid" runat="server" Height="350px" Width="100%" AdjustPageSize="Auto"
		SkinID="Primary" AllowPaging="True" AllowSearch="True" FastFilterFields="OrganizationCD" SyncPosition="True">
		<Levels>
			<px:PXGridLevel DataMember="OrganizationRecords">
				<RowTemplate>
					<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
					<px:PXTextEdit ID="edOrganizationCD" runat="server" DataField="OrganizationCD" />
					<px:PXTextEdit ID="edOrganizationName" runat="server" DataField="OrganizationName" />
					<px:PXTextEdit ID="edLedgerCD" runat="server" DataField="Ledger__LedgerCD" />
				</RowTemplate>
				<Columns>
					<px:PXGridColumn DataField="OrganizationCD" />
					<px:PXGridColumn DataField="OrganizationName" />
					<px:PXGridColumn DataField="Ledger__LedgerCD" />
				</Columns>
				<Styles>
					<RowForm Height="250px">
					</RowForm>
				</Styles>
			</px:PXGridLevel>
		</Levels>
		<Layout FormViewHeight="250px" />
		<AutoSize Container="Window" Enabled="True" MinHeight="200" />
	</px:PXGrid>
</asp:Content>
