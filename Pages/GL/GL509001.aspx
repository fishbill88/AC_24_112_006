<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="GL509001.aspx.cs" Inherits="Page_GL509001"
	Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" AutoCallBack="True" Visible="True" Width="100%"
		TypeName="PX.Objects.GL.Consolidation.ConsolSourceDataMaint" PrimaryView="Filter" PageLoadBehavior="GoLastRecord">
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="form" runat="server" Width="100%"
		DataMember="Filter" Caption="Consolidation Data Parameters" NoteIndicator="False" FilesIndicator="False"
		ActivityIndicator="False" ActivityField="NoteActivity" DefaultControlID="edYear">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="S" />
			<px:PXSelector ID="edLedgerCD" runat="server" DataField="LedgerCD" AutoRefresh="true" CommitChanges="true" />
			<px:PXTextEdit ID="edBranchCD" runat="server" DataField="BranchCD" AutoRefresh="true" CommitChanges="true" />
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXGrid ID="grid" runat="server" Height="150px"
		Width="100%" Caption="Consolidation Data" SkinID="Details" >
		<Levels>
			<px:PXGridLevel DataMember="ConsolRecords">
				<Columns>
					<px:PXGridColumn DataField="AccountCD" />
					<px:PXGridColumn DataField="MappedValue" />
					<px:PXGridColumn DataField="MappedValueLength" />
					<px:PXGridColumn DataField="FinPeriodID" />
					<px:PXGridColumn DataField="ConsolAmtCredit" />
					<px:PXGridColumn DataField="ConsolAmtDebit" />
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXGrid>
</asp:Content>
