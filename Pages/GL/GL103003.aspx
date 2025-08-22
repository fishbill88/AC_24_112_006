<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="GL103003.aspx.cs" Inherits="Page_GL103003"
	Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" Width="100%" runat="server" PrimaryView="LedgerRecords" TypeName="PX.Objects.GL.Consolidation.ConsolLedgerMaint"  Visible="True">
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXGrid ID="grid" runat="server" Height="350px" Width="100%" AdjustPageSize="Auto"
		SkinID="Primary" AllowPaging="True" AllowSearch="True" FastFilterFields="AccountCD" SyncPosition="True">
		<Levels>
			<px:PXGridLevel DataMember="LedgerRecords">
				<RowTemplate>
					<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
					<px:PXTextEdit ID="edLedgerCD" runat="server" DataField="LedgerCD" />
					<px:PXTextEdit ID="edDescr" runat="server" DataField="Descr" />
					<px:PXTextEdit ID="edBalanceType" runat="server" DataField="BalanceType" />
				</RowTemplate>
				<Columns>
					<px:PXGridColumn DataField="LedgerCD" />
					<px:PXGridColumn DataField="Descr" />
					<px:PXGridColumn DataField="BalanceType" />
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
