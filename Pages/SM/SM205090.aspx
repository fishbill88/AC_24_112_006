<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="SM205090.aspx.cs" Inherits="Page_SM205090"
	Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.CloudServices.Diagnostic.CloudServiceDiagnosticsMaint"
		PrimaryView="ServiceInformationFilter">
		<CallbackCommands>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Width="100%" DataMember="ServiceInformationFilter" Caption="Cloud Service Information">
		<Template>
            <px:PXPanel runat="server" Caption="Service Information" RenderStyle="Fieldset">
                <px:PXLayoutRule runat="server" StartColumn="true" LabelsWidth="SM" ControlSize="L" />
                <px:PXTextEdit runat="server" ID="edDiscoveryUri" DataField="DiscoveryUri" />
                <px:PXTextEdit runat="server" ID="edCloudTenantID" DataField="CloudTenantID" />

                <px:PXLayoutRule runat="server" StartColumn="true" LabelsWidth="SM" ControlSize="L" />
                <px:PXTextEdit runat="server" ID="edClientId" DataField="ClientId" />
			    <px:PXTextEdit runat="server" ID="edClientSecret" DataField="ClientSecret" />
            </px:PXPanel>
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXGrid ID="edDiagnosticResults" runat="server" DataSourceID="ds" Width="100%" AutoAdjustColumns="True" AdjustPageSize="Auto" AllowPaging="True"
        Caption="Results" SkinID="Primary" AutoSize="True" CaptionVisible="True">
        <Levels>
            <px:PXGridLevel DataMember="DiagnosticResults">
                <Columns>
                    <px:PXGridColumn DataField="Enabled" Width="50px" Type="CheckBox" TextAlign="Center" />
                    <px:PXGridColumn DataField="ServiceName" Width="150px" />
                    <px:PXGridColumn DataField="StepName" Width="150px" />
					<px:PXGridColumn DataField="Status" Width="150px" />
					<px:PXGridColumn DataField="Message" Width="300px" />
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <Mode AllowAddNew="False" AllowDelete="False" AllowUpload="False" />
        <AutoSize Enabled="True" Container="Window" />
		<ActionBar>
			<Actions>
				<AddNew ToolBarVisible="False" />
				<Delete ToolBarVisible="False" />
				<ExportExcel ToolBarVisible="False" />
				<Refresh ToolBarVisible="False" />
                <AdjustColumns ToolBarVisible="False" />
			</Actions>
		</ActionBar>
    </px:PXGrid>
</asp:Content>
