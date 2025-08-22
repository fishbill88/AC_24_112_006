<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="PR406000.aspx.cs"
	Inherits="Page_PR406000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.PR.PRPTOBalancesInq" PrimaryView="Filter" HeaderDescriptionField="HeaderDescription">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="ViewEmployeePTODetailsReport" Visible="False" DependOnGrid="gridPTOBalances" />
        </CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="headerForm" runat="server" DataSourceID="ds" DataMember="Filter" Width="100%">
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="SM" />
			<px:PXDateTimeEdit ID="edPeriodStartDate" runat="server" DataField="PeriodStartDate" CommitChanges="True" />
			<px:PXDateTimeEdit ID="edPeriodEndDate" runat="server" DataField="PeriodEndDate" CommitChanges="True" />
			
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="SM" />
			<px:PXSelector ID="edEmployeeID" runat="server" DataField="EmployeeID" CommitChanges="True" />
			<px:PXSelector ID="edBankID" runat="server" DataField="BankID" CommitChanges="True" />
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXGrid ID="gridPTOBalances" runat="server" DataSourceID="ds" SkinID="Inquire" Width="100%" SyncPosition="true" >
		<Levels>
			<px:PXGridLevel DataMember="PTOBalances">
				<Columns>
					<px:PXGridColumn DataField="EmployeeID" />
					<px:PXGridColumn DataField="EmployeeID_Description" />
					<px:PXGridColumn DataField="BankID" />
					<px:PXGridColumn DataField="BankID_Description" />
					<px:PXGridColumn DataField="EffectiveStartDate" />
					<px:PXGridColumn DataField="TotalHoursAccumulated" LinkCommand="ViewEmployeePTODetailsReport" />
					<px:PXGridColumn DataField="TotalHoursUsed" LinkCommand="ViewEmployeePTODetailsReport" />
					<px:PXGridColumn DataField="TotalHoursAvailable" LinkCommand="ViewEmployeePTODetailsReport" />
					<px:PXGridColumn DataField="AccumulatedAmount" LinkCommand="ViewEmployeePTODetailsReport" />
					<px:PXGridColumn DataField="UsedAmount" LinkCommand="ViewEmployeePTODetailsReport" />
					<px:PXGridColumn DataField="AvailableAmount" LinkCommand="ViewEmployeePTODetailsReport" />
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXGrid>
</asp:Content>
