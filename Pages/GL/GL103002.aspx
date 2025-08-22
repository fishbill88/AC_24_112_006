<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="GL103002.aspx.cs" Inherits="Page_GL103002"
	Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" Width="100%" runat="server" PrimaryView="BranchRecords" TypeName="PX.Objects.GL.Consolidation.ConsolBranchMaint"  Visible="True">
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXGrid ID="grid" runat="server" Height="350px" Width="100%" AdjustPageSize="Auto"
		SkinID="Primary" AllowPaging="True" AllowSearch="True" FastFilterFields="BranchCD" SyncPosition="True">
		<Levels>
			<px:PXGridLevel DataMember="BranchRecords">
				<RowTemplate>
					<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
					<px:PXTextEdit ID="edBranchCD" runat="server" DataField="BranchCD" />
					<px:PXTextEdit ID="edOrganizationCD" runat="server" DataField="Organization__OrganizationCD" />
					<px:PXTextEdit ID="edAcctName" runat="server" DataField="AcctName" />
					<px:PXTextEdit ID="edLedgerCD" runat="server" DataField="Ledger__LedgerCD" />
				</RowTemplate>
				<Columns>
					<px:PXGridColumn DataField="BranchCD" />
					<px:PXGridColumn DataField="Organization__OrganizationCD" />
					<px:PXGridColumn DataField="AcctName" />
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
