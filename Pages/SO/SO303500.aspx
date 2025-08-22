<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SO303500.aspx.cs" Inherits="Page_SO303500" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Width="100%" Visible="True" TypeName="PX.ExternalCarriersHelper.BrokerMaint" PrimaryView="Brokers" HeaderDescriptionField="Description">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="AddressLookupSelectAction" CommitChanges="true" Visible="false" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" Caption="Broker Accounts" DataMember="Brokers" FilesIndicator="True" NoteIndicator="True" TemplateContainer=""
		TabIndex="5100">
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
			<px:PXSelector ID="edBrokerID" runat="server" DataField="ContactID" NullText="<NEW>" DisplayMode="Text" TextMode="Search" FilterByAllFields="True" AutoRefresh="True" />
			<px:PXDropDown ID="edStatus" runat="server" DataField="Status" CommitChanges="True" AllowNull="True" NullText="New" />
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXTab ID="tab" runat="server" Width="100%" Height="400px" DataSourceID="ds" DataMember="BrokerCurrent">
		<Items>
			<px:PXTabItem Text="Details" RepaintOnDemand="False">
				<Template>
					<px:PXLayoutRule runat="server" StartRow="True" />
					<px:PXFormView ID="formBroker" runat="server" DataMember="BrokerCurrent" DataSourceID="ds" RenderStyle="Simple" DefaultControlID="edFirstName">
						<Template>
							<px:PXLayoutRule runat="server" GroupCaption="Contact" StartGroup="True" />
							<px:PXLayoutRule runat="server" StartRow="True" ControlSize="XM" LabelsWidth="SM" />
								<px:PXTextEdit ID="edFirstName" runat="server" DataField="FirstName" CommitChanges="True" />
								<px:PXTextEdit ID="edLastName" runat="server" DataField="LastName" SuppressLabel="False" CommitChanges="True" />
								<px:PXTextEdit ID="edCompanyName" runat="server" DataField="FullName" CommitChanges="True" />
								<px:PXTextEdit ID="edSalutation" runat="server" DataField="Salutation" SuppressLabel="False" CommitChanges="True" />
								<px:PXMailEdit ID="EMail" runat="server" CommandName="NewMailActivity" CommandSourceID="ds" DataField="EMail" CommitChanges="True" />
								<px:PXLayoutRule runat="server" Merge="True" ControlSize="XM" />
									<px:PXDropDown ID="Phone1Type" runat="server" DataField="Phone1Type" Size="S" SuppressLabel="True" TabIndex="-1" />
									<px:PXLabel ID="lblPhone1" runat="server" Text=" " SuppressLabel="true" />
									<px:PXMaskEdit ID="Phone1" runat="server" DataField="Phone1" SuppressLabel="True" LabelWidth="34px" />
								<px:PXLayoutRule runat="server" />
								<px:PXLinkEdit ID="WebSite" runat="server" DataField="WebSite" CommitChanges="True" />
						</Template>
						<ContentStyle BackColor="Transparent" />
					</px:PXFormView>
					<px:PXLayoutRule runat="server" StartColumn="True" />
					<px:PXFormView ID="formA" runat="server" DataMember="AddressCurrent" DataSourceID="ds" SkinID="Transparent" >
						<Template>
							<px:PXLayoutRule runat="server" GroupCaption="Address" StartGroup="True" ControlSize="XM" LabelsWidth="SM" />
								<px:PXButton ID="btnAddressLookup"  runat="server" CommandName="AddressLookup" CommandSourceID="ds" Size="xs" Height="20px" TabIndex="-1" />
								<px:PXTextEdit ID="edAddressLine1" runat="server" DataField="AddressLine1" CommitChanges="True" />
								<px:PXTextEdit ID="edAddressLine2" runat="server" DataField="AddressLine2" CommitChanges="True" />
								<px:PXTextEdit ID="edCity" runat="server" DataField="City" CommitChanges="True" />
								<px:PXSelector ID="edState" runat="server" AutoRefresh="True" DataField="State" CommitChanges="True" FilterByAllFields="True" TextMode="Search" DataSourceID="ds" />
								<px:PXMaskEdit ID="edPostalCode" runat="server" DataField="PostalCode" CommitChanges="true" />
								<px:PXSelector ID="edCountryID" runat="server" AllowEdit="True" DataField="CountryID"
									FilterByAllFields="True" TextMode="Search" CommitChanges="True" DataSourceID="ds" edit="1" />
								<px:PXCheckBox ID="chkIsValidated" runat="server" DataField="IsValidated" />
						</Template>
						<ContentLayout OuterSpacing="None" />
						<ContentStyle BackColor="Transparent" BorderStyle="None" />
					</px:PXFormView>
				</Template>
			</px:PXTabItem>
		</Items>
	</px:PXTab>
	<!--#include file="~\Pages\Includes\AddressLookupPanel.inc"-->
</asp:Content>