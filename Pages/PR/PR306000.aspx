<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="PR306000.aspx.cs"
	Inherits="Page_PR306000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.PR.PRPTOAdjustmentMaint" PrimaryView="Document" HeaderDescriptionField="HeaderDescription">
		<CallbackCommands>
            <px:PXDSCallbackCommand Name="CopyPaste" Visible="false" />
        </CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="headerForm" runat="server" DataSourceID="ds" DataMember="Document" Width="100%">
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="SM" />
			<px:PXDropDown ID="edType" runat="server" DataField="Type" CommitChanges="True" />
			<px:PXSelector ID="edRefNbr" runat="server" DataField="RefNbr" AutoRefresh="True" />
			<px:PXDropDown ID="edStatus" runat="server" DataField="Status" />
			<px:PXDateTimeEdit ID="edDate" runat="server" DataField="Date" />
			<px:PXTextEdit ID="edDescription" runat="server" DataField="Description" Width="500px"/>
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXGrid ID="gridPTOAdjustmentDetails" runat="server" DataSourceID="ds" SkinID="Details" Width="100%">
		<Levels>
			<px:PXGridLevel DataMember="PTOAdjustmentDetails">
				<Columns>
					<px:PXGridColumn DataField="BAccountID" CommitChanges="True" />
					<px:PXGridColumn DataField="BAccountID_Description" />
					<px:PXGridColumn DataField="BankID" CommitChanges="True" />
					<px:PXGridColumn DataField="InitialBalance" />
					<px:PXGridColumn DataField="AdjustmentHours" CommitChanges="True" />
					<px:PXGridColumn DataField="AdjustmentReason" CommitChanges="True" />
					<px:PXGridColumn DataField="ReasonDetails" CommitChanges="True" />
					<px:PXGridColumn DataField="NewBalance" />
					<px:PXGridColumn DataField="BalanceLimit" />
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<Mode AllowUpload="True" InitNewRow="True" />
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXGrid>
</asp:Content>
