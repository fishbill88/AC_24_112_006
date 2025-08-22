<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="AR303000.aspx.cs" Inherits="Page_AR303000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" UDFTypeField="CustomerClassID" EnableAttributes="true" Width="100%" TypeName="PX.Objects.AR.CustomerMaint" PrimaryView="BAccount">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Save" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="First" StartNewGroup="True" />
			<px:PXDSCallbackCommand Name="ViewRestrictionGroups" Visible="False" />
			<px:PXDSCallbackCommand Name="ExtendToVendor" Visible="false" />
			<px:PXDSCallbackCommand Name="ViewVendor" Visible="false" />
			<px:PXDSCallbackCommand Name="ViewBusnessAccount" Visible="True" />
			<px:PXDSCallbackCommand Name="CustomerDocuments" Visible="False" />
			<px:PXDSCallbackCommand Name="StatementForCustomer" Visible="False" />
			<px:PXDSCallbackCommand Name="ViewPaymentMethod" DependOnGrid="grdPaymentMethods" Visible="false" StartNewGroup="True" CommitChanges="true" />
			<px:PXDSCallbackCommand Name="AddPaymentMethod" Visible="False" CommitChanges="true" />
			<px:PXDSCallbackCommand Name="NewInvoiceMemo" Visible="False" />
			<px:PXDSCallbackCommand Name="NewSalesOrder" Visible="False" />
			<px:PXDSCallbackCommand Name="NewPayment" Visible="False" />
			<px:PXDSCallbackCommand Name="WriteOffBalance" Visible="False" />
			<px:PXDSCallbackCommand Name="Action" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="Inquiry" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="Report" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="ARBalanceByCustomer" Visible="False" />
			<px:PXDSCallbackCommand Name="CustomerHistory" Visible="False" />
			<px:PXDSCallbackCommand Name="ARAgedPastDue" Visible="False" />
			<px:PXDSCallbackCommand Name="ARAgedOutstanding" Visible="False" />
			<px:PXDSCallbackCommand Name="ARRegister" Visible="False" />
			<px:PXDSCallbackCommand Name="CustomerDetails" Visible="False" />
			<px:PXDSCallbackCommand Name="CustomerStatement" Visible="False" />
			<px:PXDSCallbackCommand Name="SalesPrice" Visible="False" />
			<px:PXDSCallbackCommand Name="ViewServiceOrderHistory" Visible="False" />
			<px:PXDSCallbackCommand Name="ViewAppointmentHistory" Visible="False" />
			<px:PXDSCallbackCommand Name="ViewEquipmentSummary" Visible="False" />
			<px:PXDSCallbackCommand Name="ViewContractScheduleSummary" Visible="False" />
			<px:PXDSCallbackCommand Name="ScheduleAppointment" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="ViewEquipmentSummary" Visible="False" />
			<px:PXDSCallbackCommand Name="ViewContractScheduleSummary" Visible="False" />
			<px:PXDSCallbackCommand Name="ValidateAddresses" StartNewGroup="True" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="RegenerateLastStatement" StartNewGroup="True" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="NewTask" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="NewEvent" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="NewActivity" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="NewMailActivity" DependOnGrid="gridActivities" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="ViewActivity" DependOnGrid="gridActivities" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="OpenActivityOwner" Visible="False" CommitChanges="True" DependOnGrid="gridActivities" />

			<%-- Address --%>
			<px:PXDSCallbackCommand Name="ViewMainOnMap" Visible="false" />
			<px:PXDSCallbackCommand Name="ViewBillAddressOnMap" Visible="false" />
			<px:PXDSCallbackCommand Name="ViewDefLocationAddressOnMap" Visible="false" />
			<px:PXDSCallbackCommand Name="AddressLookupSelectAction" CommitChanges="true" Visible="false" />
			<px:PXDSCallbackCommand Name="AddressLookup" SelectControlsIDs="BAccount" RepaintControls="None" RepaintControlsIDs="ds,DefAddress" CommitChanges="true" Visible="false" />
			<px:PXDSCallbackCommand Name="BillingAddressLookup" SelectControlsIDs="BAccount" RepaintControls="None" RepaintControlsIDs="ds,BillAddress" CommitChanges="true" Visible="false" />
			<px:PXDSCallbackCommand Name="DefLocationAddressLookup"  SelectControlsIDs="BAccount" RepaintControls="None" RepaintControlsIDs="ds,DefLocationAddress" CommitChanges="true" Visible="false" />

			<%-- Contacts grid --%>
			<px:PXDSCallbackCommand Name="CreateContact" Visible="False"/>
			<px:PXDSCallbackCommand Name="MakeContactPrimary" DependOnGrid="grdContacts" Visible="False" RepaintControlsIDs="grdContacts" />

			<%-- Locations grid --%>
			<px:PXDSCallbackCommand Name="NewLocation" Visible="False" CommitChanges="true" />
			<px:PXDSCallbackCommand Name="SetDefaultLocation" DependOnGrid="grdLocations" Visible="False" />

            <px:PXDSCallbackCommand Name="ComplianceDocument$PurchaseOrder$Link" Visible="false" DependOnGrid="grid" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="ComplianceDocument$Subcontract$Link" CommitChanges="True" Visible="false" DependOnGrid="grid" />
			<px:PXDSCallbackCommand Name="ComplianceDocument$InvoiceID$Link" Visible="false" DependOnGrid="grdComplianceDocuments" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="ComplianceDocument$BillID$Link" Visible="false" DependOnGrid="grdComplianceDocuments" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="ComplianceDocument$ApCheckID$Link" Visible="false" DependOnGrid="grdComplianceDocuments" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="ComplianceDocument$ArPaymentID$Link" Visible="false" DependOnGrid="grdComplianceDocuments" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="ComplianceDocument$ProjectTransactionID$Link" Visible="false" DependOnGrid="grdComplianceDocuments" CommitChanges="True" />

            <px:PXDSCallbackCommand Name="RefreshCertificates" Visible="False" />
            <px:PXDSCallbackCommand Name="RequestCertificate" Visible="False" />

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXSmartPanel ID="smpCPMInstance" runat="server" Key="PMInstanceEditor" InnerPageUrl="~/Pages/AR/AR303010.aspx?PopupPanel=On" CaptionVisible="true" Caption="Card Definition" RenderIFrame="true" Visible="False" DesignView="Content">
	</px:PXSmartPanel>
	<px:PXSmartPanel ID="pnlChangeID" runat="server" Caption="Specify New ID"
		CaptionVisible="true" DesignView="Hidden" LoadOnDemand="true" Key="ChangeIDDialog" CreateOnDemand="false" AutoCallBack-Enabled="true"
		AutoCallBack-Target="formChangeID" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True" CallBackMode-PostData="Page"
		AcceptButtonID="btnOK">
		<px:PXFormView ID="formChangeID" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" CaptionVisible="False"
			DataMember="ChangeIDDialog">
			<ContentStyle BackColor="Transparent" BorderStyle="None" />
			<Template>
				<px:PXLayoutRule ID="rlAcctCD" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
				<px:PXSegmentMask ID="edAcctCD" runat="server" DataField="CD" />
			</Template>
		</px:PXFormView>
		<px:PXPanel ID="pnlChangeIDButton" runat="server" SkinID="Buttons">
			<px:PXButton ID="btnOK" runat="server" DialogResult="OK" Text="OK">
				<AutoCallBack Target="formChangeID" Command="Save" />
			</px:PXButton>
			<px:PXButton ID="btnCancel" runat="server" DialogResult="Cancel" Text="Cancel" />						
		</px:PXPanel>
	</px:PXSmartPanel>
    <px:PXSmartPanel ID="pnlOnDemandStatement" runat="server" Caption="Generate On-Demand Statement" 
        CaptionVisible="true" DesignView="Hidden" LoadOnDemand="true" Key="OnDemandStatementDialog" CreateOnDemand="false" 
        AutoCallBack-Enabled="true" AutoCallBack-Target="formOnDemandStatement" AutoCallBack-Command="Refresh"
        CallBackMode-CommitChanges="true" CallBackMode-PostData="Page" AcceptButtonID="btnOK">
        <px:PXFormView ID="formOnDemandStatement" runat="server" DataSourceID="ds" Style="z-index: 100" 
            Width="100%" CaptionVisible="False" DataMember="OnDemandStatementDialog">
            <ContentStyle BackColor="Transparent" BorderStyle="None" />
            <Template>
                <px:PXLayoutRule ID="lrStatementDate" runat="server" StartColumn="true" LabelsWidth="S" ControlSize="XM" />
                <px:PXDateTimeEdit ID="dteStatementDate" runat="server" DataField="StatementDate" />
            </Template>
        </px:PXFormView>
        <px:PXPanel ID="pnlOnDemandStatementButton" runat="server" SkinID="Buttons">
            <px:PXButton ID="btnOK1" runat="server" DialogResult="OK" Text="OK">
                <AutoCallBack Target="formOnDemandStatement" Command="Save" />
            </px:PXButton>
        </px:PXPanel>
    </px:PXSmartPanel>
    <px:PXSmartPanel ID="pnlCreateCustomerInECM" runat="server" Caption="Select Company Code"
        CaptionVisible="true" DesignView="Hidden" LoadOnDemand="true" Key="CreateCustomerFilter" CreateOnDemand="false" AutoCallBack-Enabled="true"
        AutoCallBack-Target="formCreateCustomer" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True" CallBackMode-PostData="Page"
        AcceptButtonID="btnOK">
        <px:PXFormView ID="formCreateCustomer" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" CaptionVisible="False"
            DataMember="CreateCustomerFilter">
            <ContentStyle BackColor="Transparent" BorderStyle="None" />
            <Template>
                <px:PXLayoutRule ID="PXLayoutRule44" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                <px:PXSelector ID="edCompanyCode" runat="server" DataField="CompanyCode" AutoRefresh="true" >
                </px:PXSelector>
            </Template>
        </px:PXFormView>
        <px:PXPanel ID="PXPanel5" runat="server" SkinID="Buttons">
            <px:PXButton ID="btnSave" runat="server" DialogResult="OK" Text="OK" />
            <px:PXButton ID="PXButton6" runat="server" DialogResult="Cancel" Text="Cancel" />
        </px:PXPanel>
    </px:PXSmartPanel>
    <px:PXSmartPanel ID="pnlExemptionCertificates" runat="server" Caption="Exemption Certificates" CaptionVisible="true" DesignView="Hidden" LoadOnDemand="true" Key="ExemptionCertificates"
            CreateOnDemand="false" AutoCallBack-Enabled="true" AutoCallBack-Target="gridCertificates" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True" 
            CallBackMode-PostData="Page" AutoReload="true" Width="850px" Height="350px" CloseButtonDialogResult="OK">
        <px:PXGrid ID="gridCertificates" runat="server" DataSourceID="ds" SkinID="Details" SyncPosition="true" AllowPaging="true" AdjustPageSize="Auto">
            <AutoSize Enabled="true" />
            <Levels>
                <px:PXGridLevel DataMember="ExemptionCertificates">
                    <Columns>
                        <px:PXGridColumn DataField="CertificateID" TextAlign="Center" LinkCommand="viewCertificate"/>
                        <px:PXGridColumn DataField="State"/>
                        <px:PXGridColumn DataField="ExemptionReason"/>
                        <px:PXGridColumn DataField="EffectiveDate"/>
                        <px:PXGridColumn DataField="ExpirationDate"/>
                        <px:PXGridColumn DataField="Status"/>
                        <px:PXGridColumn DataField="CompanyCode"/>
                    </Columns>
                </px:PXGridLevel>
            </Levels>
            <ActionBar Position="Top" PagerVisible="Bottom" ActionsVisible="True">
                <CustomItems>
                    <px:PXToolBarButton Text="Request Certificate">
                        <AutoCallBack Command="RequestCertificate" Target="ds" />
                    </px:PXToolBarButton>
                    <px:PXToolBarButton DisplayStyle="Image" ImageKey="Refresh">
						<AutoCallBack Command="RefreshCertificates" Target="ds">
						</AutoCallBack>
					</px:PXToolBarButton>
                </CustomItems>
                <Actions>
                    <AddNew ToolBarVisible="False" />
					<Delete ToolBarVisible="False" />
                    <Refresh ToolBarVisible="False" />
                </Actions>
            </ActionBar>
            <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
        </px:PXGrid>
        <px:PXPanel runat="server" SkinID="Buttons" ID="pnlButtons">
        </px:PXPanel>
    </px:PXSmartPanel>
    <px:PXSmartPanel ID="pnlRequestExemptCertificate" runat="server" Caption="Request Certificate" CaptionVisible="true" DesignView="Hidden" LoadOnDemand="true" Key="RequestCertificateFilter"
            CreateOnDemand="false" AutoCallBack-Enabled="true" AutoCallBack-Target="formRequestCertificate" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True" 
            CallBackMode-PostData="Page" AcceptButtonID="btnReqOK" AutoReload="true">
        <px:PXFormView ID="formRequestCertificate" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" CaptionVisible="False"
            DataMember="RequestCertificateFilter">
            <ContentStyle BackColor="Transparent" BorderStyle="None" />
            <Template>
                <px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM"/>
                <px:PXSelector ID="edCompanyCode" runat="server" DataField="CompanyCode" CommitChanges="True"/>
                <px:PXTextEdit ID="edEmail" runat="server" DataField="EmailId" CommitChanges="True"/>
                <px:PXSelector ID="edTemplate" runat="server" DataField="Template" CommitChanges="True"/>
                <px:PXSelector ID="edCountryID" runat="server" DataField="CountryID" CommitChanges="True" AutoRefresh="True"/>
                <px:PXSelector ID="edState" runat="server" DataField="State" CommitChanges="True" AutoRefresh="True"/>
                <px:PXSelector ID="edExemptReason" runat="server" DataField="ExemptReason" DisplayMode="Text"/>
            </Template>
        </px:PXFormView>
        <px:PXPanel ID="PXPanel1" runat="server" SkinID="Buttons">
            <px:PXButton ID="btnReqOK" runat="server" DialogResult="OK" Text="Request" />
            <px:PXButton ID="PXButton2" runat="server" DialogResult="Cancel" Text="Cancel" />
        </px:PXPanel>
    </px:PXSmartPanel>

	<px:PXFormView ID="BAccount" runat="server" Width="100%" Caption="Customer Summary" DataMember="BAccount" NoteIndicator="True" FilesIndicator="True" ActivityIndicator="false" ActivityField="NoteActivity" LinkIndicator="true" BPEventsIndicator="true" DefaultControlID="edAcctCD">
		<Template>

			<%-- column 1 --%>

			<px:PXLayoutRule runat="server" StartColumn="True" ControlSize="XM" LabelsWidth="SM" />
				<px:PXSegmentMask ID="edAcctCD" runat="server" DataField="AcctCD" FilterByAllFields="True" />
				<px:PXDropDown ID="edStatus" runat="server" DataField="Status" CommitChanges="True" />
				<px:PXSelector CommitChanges="True" ID="edCustomerClassID" runat="server" DataField="CustomerClassID" AllowEdit="True" />
				<px:PXDropDown ID="bcCusomterKind" runat="server" DataField="CustomerCategory" AllowEdit="false" />

			<%-- column 2 --%>

			<px:PXLayoutRule runat="server" StartColumn="True" />
				<px:PXFormView ID="CustomerBalance" runat="server" DataMember="CustomerBalance" RenderStyle="Simple">
					<Template>
						<px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="M" />
						<px:PXLayoutRule ID="edMerge01" runat="server" Merge="true" />
	                        <px:PXTextEdit ID="edBalanceLabel" runat="server" 
	                                       Style="background-color: transparent; border-width:0px; padding-left:0px; color:#5c5c5c"
	                                       DataField="Balance_Label" 
	                                       SuppressLabel="true" 
	                                       Width="154px"  
	                                       Enabled="false"
	                                       IsClientControl="false" />
	                        <px:PXNumberEdit ID="edBBalance" runat="server" DataField="Balance" SuppressLabel="true" />
                        <px:PXLayoutRule ID="edMerge02" runat="server" Merge="false" />
						<px:PXLayoutRule ID="edMerge21" runat="server" Merge="true" />
	                        <px:PXTextEdit ID="edConsolidatedBalanceLabel" runat="server" 
	                                       Style="background-color: transparent; border-width:0px; padding-left:0px; color:#5c5c5c"
	                                       DataField="ConsolidatedBalance_Label" 
	                                       SuppressLabel="true" 
	                                       Width="154px"  
	                                       Enabled="false"
	                                       IsClientControl="false" />
	                        <px:PXNumberEdit ID="edConsolidatedBalance" runat="server" DataField="ConsolidatedBalance" SuppressLabel="true" />
                        <px:PXLayoutRule ID="edMerge22" runat="server" Merge="false" />
						<px:PXLayoutRule ID="edMerge31" runat="server" Merge="true" />
                            <px:PXTextEdit ID="edSignedDepositsBalanceLabel" runat="server" 
                                           Style="background-color: transparent; border-width:0px; padding-left:0px; color:#5c5c5c"
                                           DataField="SignedDepositsBalance_Label" 
                                           SuppressLabel="true" 
                                           Width="154px"  
                                           Enabled="false"
                                           IsClientControl="false" />
                            <px:PXNumberEdit ID="edSignedDepositsBalance" runat="server" DataField="SignedDepositsBalance" SuppressLabel="true" />
                        <px:PXLayoutRule ID="edMerge32" runat="server" Merge="false" />
						<px:PXLayoutRule ID="edMerge41" runat="server" Merge="true" />
	                        <px:PXTextEdit ID="edRetainageBalanceLabel" runat="server" 
	                                       Style="background-color: transparent; border-width:0px; padding-left:0px; color:#5c5c5c"
	                                       DataField="RetainageBalance_Label" 
	                                       SuppressLabel="true" 
	                                       Width="154px"  
	                                       Enabled="false"
	                                       IsClientControl="false" />
	                        <px:PXNumberEdit ID="edRetainageBalance" runat="server" DataField="RetainageBalance" SuppressLabel="true" />
                        <px:PXLayoutRule ID="edMerge42" runat="server" Merge="false" />						
						
					</Template>
				</px:PXFormView>

			<px:PXLayoutRule runat="server" />
				<px:PXCheckBox ID="chkServiceManagement" runat="server" DataField="ChkServiceManagement" Visible="False"/>

		</Template>
	</px:PXFormView>
</asp:Content>

<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXTab ID="tab" runat="server" Height="580px" Style="z-index: 100" Width="100%" DataMember="CurrentCustomer" DataSourceID="ds" MarkRequired="Dynamic">
		<AutoSize Enabled="True" Container="Window" MinWidth="300" MinHeight="250" />
		<Activity HighlightColor="" SelectedColor="" Width="" Height="" />
		<Items>

			<px:PXTabItem Text="General" RepaintOnDemand="false">
				<Template>

					<%-- column 1 --%>

					<px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="XM" />
					<px:PXLayoutRule runat="server" GroupCaption="Account Info" />
						<px:PXTextEdit ID="edAcctName" runat="server" DataField="AcctName" CommitChanges="True" TabIndex="100" />

					<px:PXLayoutRule runat="server" GroupCaption="Account Address" StartGroup="True" />
						<px:PXFormView ID="DefAddress" runat="server" DataMember="DefAddress" RenderStyle="Simple" TabIndex="200">
							<Template>
								<px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" />
									<px:PXButton ID="btnAddressLookup" runat="server" CommandName="AddressLookup" CommandSourceID="ds" Size="xs" TabIndex="-1" />
									<px:PXButton ID="btnViewMainOnMap" runat="server" CommandName="ViewMainOnMap" CommandSourceID="ds" Size="xs" Text="View on Map" TabIndex="-1" />
									<px:PXTextEdit ID="edDefAddressAddressLine1" runat="server" DataField="AddressLine1" CommitChanges="True" />
									<px:PXTextEdit ID="edDefAddressAddressLine2" runat="server" DataField="AddressLine2" CommitChanges="True" />
									<px:PXTextEdit ID="edDefAddressCity" runat="server" DataField="City" CommitChanges="True" />
									<px:PXSelector ID="edDefAddressState" runat="server" DataField="State" CommitChanges="True" AutoRefresh="True" AllowAddNew="True" />
									<px:PXMaskEdit ID="edDefAddressPostalCode" runat="server" DataField="PostalCode" CommitChanges="True" />
									<px:PXSelector ID="edDefAddressCountryID" runat="server" DataField="CountryID" CommitChanges="True" AllowAddNew="True" />
									<px:PXCheckBox ID="edDefAddressIsValidated" runat="server" DataField="IsValidated" Enabled="False" />
							</Template>
						</px:PXFormView>

					<px:PXLayoutRule runat="server" GroupCaption="Additional Account Info" StartGroup="True" />
						<px:PXFormView ID="DefContact1" runat="server" DataMember="DefContact" DataSourceID="ds" RenderStyle="Simple" TabIndex="300">
							<Template>
								<px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="XM" />
								<px:PXLayoutRule runat="server" Merge="True"/>
									<px:PXDropDown ID="edDefContactPhone1Type" runat="server" DataField="Phone1Type" Size="S" SuppressLabel="True" TabIndex="-1" />
									<px:PXLabel ID="edDefContactlblPhone1" runat="server" Text=" " SuppressLabel="true" />
									<px:PXMaskEdit ID="edDefContactPXMaskEdit1" runat="server" DataField="Phone1" SuppressLabel="True" LabelWidth="34px" />
								<px:PXLayoutRule runat="server" Merge="True" />	
									<px:PXDropDown ID="edDefContactPhone2Type" runat="server" DataField="Phone2Type" Size="S" SuppressLabel="True" TabIndex="-1" />
									<px:PXLabel ID="edDefContactlblPhone2" runat="server" Text=" " SuppressLabel="true" />
									<px:PXMaskEdit ID="edDefContactPXMaskEdit2" runat="server" DataField="Phone2" SuppressLabel="True" LabelWidth="34px" />
								<px:PXLayoutRule runat="server" Merge="True" />
									<px:PXDropDown ID="edDefContactFaxType" runat="server" DataField="FaxType" Size="S" SuppressLabel="True" TabIndex="-1" />
									<px:PXLabel ID="edDefContactlblFax" runat="server" Text=" " SuppressLabel="true" />
									<px:PXMaskEdit ID="edDefContactPXMaskEdit3" runat="server" DataField="Fax" SuppressLabel="True" LabelWidth="34px" />
								<px:PXLayoutRule runat="server" Merge="True" />
									<px:PXLabel ID="lblDefContactAccountEmail" runat="server" Text="Account Email:" />
									<px:PXMailEdit ID="edDefContactEMail" runat="server" DataField="EMail" CommandName="NewMailActivity" CommandSourceID="ds" CommitChanges="True" SuppressLabel="True"/>
								<px:PXLayoutRule runat="server" />
								<px:PXLinkEdit ID="edDefContactWebSite" runat="server" DataField="WebSite" CommitChanges="True" />
							</Template>
						</px:PXFormView>

						<px:PXFormView ID="ReferenceNbr" runat="server" DataMember="CurrentCustomer" DataSourceID="ds" RenderStyle="Simple" TabIndex="350">
							<Template>
								<px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="XM" />
						<px:PXTextEdit ID="edExtRefNbr" runat="server" DataField="AcctReferenceNbr" CommitChanges="True" />
						<px:PXSelector ID="edLocaleName" runat="server" DataField="LocaleName" CommitChanges="True" />
							</Template>
						</px:PXFormView>
					
					<px:PXLayoutRule runat="server" GroupCaption="Account Personal Data Privacy" StartGroup="True" />
						<px:PXFormView ID="DefContact2" runat="server" DataMember="DefContact" DataSourceID="ds" RenderStyle="Simple" TabIndex="400">
							<Template>
								<px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="XM" />
									<px:PXCheckBox ID="edDefContactConsentAgreement" runat="server" DataField="ConsentAgreement" AlignLeft="True" CommitChanges="True" TabIndex="-1"/>
									<px:PXDateTimeEdit ID="edDefContactConsentDate" runat="server" DataField="ConsentDate" CommitChanges="True"/>
									<px:PXDateTimeEdit ID="edDefContactConsentExpirationDate" runat="server" DataField="ConsentExpirationDate" CommitChanges="True"/>

							</Template>
						</px:PXFormView>

					<%-- column 2 --%>

					<px:PXLayoutRule runat="server" StartColumn="True" />
						<px:PXLayoutRule runat="server" GroupCaption="Primary Contact" StartGroup="True" ControlSize="XM" LabelsWidth="SM" />
	<Template>
	<px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" Merge="True" />
		<px:PXSelector ID="edPrmCntID" runat="server" DataField="PrimaryContactID" AutoRefresh="true" DisplayMode="Text" CommitChanges="True" AllowEdit="True" OnEditRecord="edPrmCntID_EditRecord" >
		<GridProperties FastFilterFields="DisplayName, Salutation, Phone1, EMail" />	
	</px:PXSelector>
	<px:PXButton runat="server" CommandSourceID="ds" CommandName="ContactTeamsCardOffline" ID="edContactTeamsCardOffline" CssClass="Button teamsImageButton" Style="margin-left: 28px" Width="20" Height="24">
		<Images Normal="svg:teams@teams_offline" />
	</px:PXButton>
	<px:PXButton runat="server" CommandSourceID="ds" CommandName="ContactTeamsCardAvailable" ID="edContactTeamsCardAvailable" CssClass="Button teamsImageButton" Style="margin-left: 28px" Width="20" Height="24">
		<Images Normal="svg:teams@teams_available" />
	</px:PXButton>
	<px:PXButton runat="server" CommandSourceID="ds" CommandName="ContactTeamsCardBusy" ID="edContactTeamsCardBusy" CssClass="Button teamsImageButton" Style="margin-left: 28px" Width="20" Height="24">
		<Images Normal="svg:teams@teams_busy" />
	</px:PXButton>
	<px:PXButton runat="server" CommandSourceID="ds" CommandName="ContactTeamsCardAway" ID="edContactTeamsCardAway" CssClass="Button teamsImageButton" Style="margin-left: 28px" Width="20" Height="24">
		<Images Normal="svg:teams@teams_away" />
	</px:PXButton>
<px:PXLayoutRule runat="server" Merge="False" />
	</Template>
	<ContentLayout OuterSpacing="None" />
	<ContentStyle BackColor="Transparent" BorderStyle="None" />


<px:PXFormView ID="frmPrimaryContact" runat="server" CaptionVisible="False" DataSourceID="ds" DataMember="PrimaryContactCurrent" SkinID="Transparent">
	<Template>
		<px:PXLayoutRule runat="server" />
		<px:PXLayoutRule runat="server" Merge="True" ControlSize="XM" LabelsWidth="SM" />
			<px:PXLabel ID="lblPrimaryContactName" runat="server" Text="Name:" />
			<px:PXTextEdit ID="edPrmCntFirstName" runat="server" CommitChanges="True" DataField="FirstName" SuppressLabel="True" Width="84px" Placeholder="First Name" />
			<px:PXTextEdit ID="edPrmCntLastName" runat="server" CommitChanges="True" DataField="LastName" SuppressLabel="True" Width="150px" Placeholder="Last Name" />
		<px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" />
			<px:PXTextEdit ID="edPrmCntJobTitle" runat="server" DataField="Salutation" CommitChanges="True" />
			<px:PXMailEdit ID="edPrmCntEmail" runat="server" DataField="EMail" CommandName="NewMailActivity" CommandSourceID="ds" CommitChanges="True" />
		<px:PXLayoutRule runat="server" Merge="True" ControlSize="XM" LabelsWidth="SM" />
			<px:PXDropDown ID="ddPrmCntPhone1Type" runat="server" DataField="Phone1Type" CommitChanges="True" Size="S" SuppressLabel="True" TabIndex="-1" />
			<px:PXLabel ID="lblPrmCntPhone1" runat="server" Text=" " SuppressLabel="true" />
			<px:PXMaskEdit ID="mePrmCntPhone1" runat="server" DataField="Phone1" CommitChanges="True" SuppressLabel="True" LabelWidth="34px" />
		<px:PXLayoutRule runat="server" Merge="True" ControlSize="XM" LabelsWidth="SM" />
			<px:PXDropDown ID="ddPrmCntPhone2Type" runat="server" DataField="Phone2Type" CommitChanges="True" Size="S" SuppressLabel="True" TabIndex="-1" />
			<px:PXLabel ID="lblPrmCntPhone2" runat="server" Text=" " SuppressLabel="true" />
			<px:PXMaskEdit ID="mePrmCntPhone2" runat="server" DataField="Phone2" CommitChanges="True" SuppressLabel="True" LabelWidth="34px" />
		<px:PXLayoutRule runat="server" GroupCaption="Contact Personal Data Privacy" ControlSize="XM" LabelsWidth="SM" />
			<px:PXCheckBox ID="cbPrmCntConsent" runat="server" DataField="ConsentAgreement" AlignLeft="True" CommitChanges="True" TabIndex="-1" />
			<px:PXDateTimeEdit ID="edPrmCntConsentDate" runat="server" DataField="ConsentDate" CommitChanges="True" />
			<px:PXDateTimeEdit ID="edPrmCntConsentExpDate" runat="server" DataField="ConsentExpirationDate" CommitChanges="True" />
	</Template>
	<ContentLayout OuterSpacing="None" />
	<ContentStyle BackColor="Transparent" BorderStyle="None">
	</ContentStyle>
</px:PXFormView>

				</Template>
			</px:PXTabItem>
			
			<px:PXTabItem Text="Financial">
				<Template>

					<%-- column 1 --%>
					
					<px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="XM" />
					<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Financial Settings" />
						<px:PXSelector ID="edTermsID" runat="server" DataField="TermsID" AllowEdit="True" />
						<px:PXSelector ID="edStatementCycleId" runat="server" DataField="StatementCycleId" AllowEdit="True" CommitChanges="true" />
						<px:PXBranchSelector CommitChanges="True" ID="edCOrgBAccountID" runat="server" DataField="COrgBAccountID" InitialExpandLevel="0" />
						<px:PXCheckBox SuppressLabel="True" ID="chkAutoApplyPayments" runat="server" DataField="AutoApplyPayments" TabIndex="-1" />
						<px:PXCheckBox CommitChanges="True" ID="chkFinChargeApply" runat="server" DataField="FinChargeApply" TabIndex="-1" />
						<px:PXCheckBox CommitChanges="True" ID="chkSmallBalanceAllow" runat="server" DataField="SmallBalanceAllow" TabIndex="-1" />
						<px:PXLayoutRule ID="edMerge31" runat="server" Merge="true" />
	                        <px:PXTextEdit ID="edSmallBalanceLimitLabel" runat="server" 
	                                       Style="background-color: transparent; border-width:0px; padding-left:0px; color:#5c5c5c"
	                                       DataField="SmallBalanceLimit_Label" 
	                                       SuppressLabel="true" 
	                                       Width="134px"  
	                                       Enabled="false"
	                                       IsClientControl="false" />
	                        <px:PXNumberEdit ID="edSmallBalanceLimit" runat="server" DataField="SmallBalanceLimit" SuppressLabel="true" />
	                    <px:PXLayoutRule ID="edMerge32" runat="server" Merge="false" />
					<px:PXLayoutRule runat="server" Merge="True" />
						<px:PXSelector ID="edCuryID" runat="server" DataField="CuryID" Size="S" CommitChanges="True" />
						<px:PXCheckBox ID="chkAllowOverrideCury" runat="server" DataField="AllowOverrideCury" TabIndex="-1" CommitChanges="True" />
					<px:PXLayoutRule runat="server" />
					<px:PXLayoutRule runat="server" Merge="True" />
						<px:PXSelector ID="edCuryRateTypeID" runat="server" DataField="CuryRateTypeID" Size="S" CommitChanges="True" />
						<px:PXCheckBox ID="chkAllowOverrideRate" runat="server" DataField="AllowOverrideRate" TabIndex="-1" CommitChanges="True" />
					<px:PXLayoutRule runat="server" />
						<px:PXCheckBox ID="chkPaymentsByLinesAllowed" runat="server" DataField="PaymentsByLinesAllowed" TabIndex="-1" />

					<%-- column 2 --%>

					<px:PXLayoutRule runat="server" StartColumn="True" />
					<px:PXLayoutRule runat="server" GroupCaption="Retainage Settings" StartGroup="True" />
					<px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="M" />
						<px:PXCheckBox runat="server" ID="edRetainageApply" DataField="RetainageApply" CommitChanges="True" TabIndex="-1" />
						<px:PXNumberEdit ID="edRetainagePct" runat="server" DataField="RetainagePct" CommitChanges="True" />

					<px:PXLayoutRule runat="server" GroupCaption="Credit Verification Rules" StartGroup="True" />
					<px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="M" />
						<px:PXDropDown CommitChanges="True" ID="edCreditRule" runat="server" DataField="CreditRule" />
						<px:PXLayoutRule ID="edMerge41" runat="server" Merge="true" />
                            <px:PXTextEdit ID="edCreditLimitLabel" runat="server" 
                                           Style="background-color: transparent; border-width:0px; padding-left:0px; color:#5c5c5c"
                                           DataField="CreditLimit_Label" 
                                           SuppressLabel="true" 
                                           Width="184px"  
                                           Enabled="false"
                                           IsClientControl="false" />
                            <px:PXNumberEdit ID="edCreditLimit" runat="server" DataField="CreditLimit" SuppressLabel="true" CommitChanges="true"/>
                        <px:PXLayoutRule ID="edMerge42" runat="server" Merge="false" />
					<px:PXNumberEdit ID="edCreditDaysPastDue" runat="server" DataField="CreditDaysPastDue" />
						<px:PXFormView ID="CustomerBalanceFin" runat="server" DataMember="CustomerBalance" RenderStyle="Simple">
							<Template>
								<px:PXLayoutRule runat="server" LabelsWidth="M" ControlSize="XM" />
								<px:PXLayoutRule ID="edMerge01" runat="server" Merge="true" />
                                    <px:PXTextEdit ID="edUnreleasedBalanceLabel" runat="server" 
                                                   Style="background-color: transparent; border-width:0px; padding-left:0px; color:#5c5c5c"
                                                   DataField="UnreleasedBalance_Label" 
                                                   SuppressLabel="true" 
                                                   Width="184px"  
                                                   Enabled="false"
                                                   IsClientControl="false" />
                                    <px:PXNumberEdit ID="edUnreleasedBalance" runat="server" DataField="UnreleasedBalance" SuppressLabel="true" />
                                <px:PXLayoutRule ID="edMerge02" runat="server" Merge="false" />
								<px:PXLayoutRule ID="edMerge11" runat="server" Merge="true" />
                                    <px:PXTextEdit ID="edOpenOrdersBalanceLabel" runat="server" 
                                                   Style="background-color: transparent; border-width:0px; padding-left:0px; color:#5c5c5c"
                                                   DataField="OpenOrdersBalance_Label" 
                                                   SuppressLabel="true" 
                                                   Width="184px"  
                                                   Enabled="false"
                                                   IsClientControl="false" />
                                    <px:PXNumberEdit ID="edOpenOrdersBalance" runat="server" DataField="OpenOrdersBalance" SuppressLabel="true" />
                                <px:PXLayoutRule ID="edMerge12" runat="server" Merge="false" />
								<px:PXLayoutRule ID="edMerge21" runat="server" Merge="true" />
                                    <px:PXTextEdit ID="edRemainingCreditLimitLabel" runat="server" 
                                                   Style="background-color: transparent; border-width:0px; padding-left:0px; color:#5c5c5c"
                                                   DataField="RemainingCreditLimit_Label" 
                                                   SuppressLabel="true" 
                                                   Width="184px"  
                                                   Enabled="false"
                                                   IsClientControl="false" />
                                    <px:PXNumberEdit ID="edRemainingCreditLimit" runat="server" DataField="RemainingCreditLimit" SuppressLabel="true" />
                                <px:PXLayoutRule ID="edMerge22" runat="server" Merge="false" />
                               <px:PXDateTimeEdit ID="edOldInvoiceDate" runat="server" DataField="OldInvoiceDate" Enabled="False" />
							</Template>
						</px:PXFormView>

				</Template>
			</px:PXTabItem>

			<px:PXTabItem Text="Billing">
				<Template>

					<%-- column 1 --%>

					<px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="XM" />
					<px:PXLayoutRule runat="server" GroupCaption="Bill-To Address" StartGroup="True" />
						<px:PXCheckBox ID="chkOverrideBillAddress" runat="server" DataField="OverrideBillAddress" CommitChanges="True" SuppressLabel="True" TabIndex="-1" />
						<px:PXFormView ID="BillAddress" runat="server" DataMember="BillAddress" RenderStyle="Simple" SyncPosition="true" DataSourceID="ds">
							<Template>
								<px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" StartColumn="True" />
									<px:PXButton ID="btnBillingAddressLookup" runat="server" CommandName="BillingAddressLookup" CommandSourceID="ds" Size="xs" TabIndex="-1" />
									<px:PXButton ID="btnViewBillAddressOnMap" runat="server" CommandName="ViewBillAddressOnMap" CommandSourceID="ds" Size="xs" Text="View on Map" TabIndex="-1" />
									<px:PXTextEdit ID="edAddressLine1" runat="server" DataField="AddressLine1" CommitChanges="True" />
									<px:PXTextEdit ID="edAddressLine2" runat="server" DataField="AddressLine2" CommitChanges="True" />
									<px:PXTextEdit ID="edCity" runat="server" DataField="City" CommitChanges="True" />
									<px:PXSelector ID="edState" runat="server" AllowAddNew="True" AutoRefresh="True" DataField="State" CommitChanges="True" />
									<px:PXMaskEdit ID="edPostalCode" runat="server" CommitChanges="True" DataField="PostalCode" />
									<px:PXSelector ID="edCountryID" runat="server" AutoRefresh="True" CommitChanges="True" DataField="CountryID" />
									<px:PXCheckBox ID="edIsValidated" runat="server" DataField="IsValidated" Enabled="False" />
							</Template>
						</px:PXFormView>

					<px:PXLayoutRule runat="server" GroupCaption="Bill-To Info" StartGroup="True" />
						<px:PXCheckBox ID="chkOverrideBillContact" runat="server" DataField="OverrideBillContact" CommitChanges="True" SuppressLabel="True" TabIndex="-1" />
						<px:PXFormView ID="BillContact" runat="server" DataMember="BillContact" RenderStyle="Simple" DataSourceID="ds">
							<Template>
								<px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" StartColumn="True" />
									<px:PXTextEdit ID="edFullName" runat="server" DataField="FullName" />
									<px:PXTextEdit ID="edAttention" runat="server" DataField="Attention" />
									<px:PXLayoutRule runat="server" Merge="True"/>
										<px:PXDropDown ID="Phone1Type" runat="server" DataField="Phone1Type" Size="S" SuppressLabel="True" TabIndex="-1" />
										<px:PXLabel ID="lblPhone1" runat="server" Text=" " SuppressLabel="true" />
										<px:PXMaskEdit ID="PXMaskEdit1" runat="server" DataField="Phone1" SuppressLabel="True" LabelWidth="34px" />
									<px:PXLayoutRule runat="server" Merge="True" />
										<px:PXDropDown ID="Phone2Type" runat="server" DataField="Phone2Type" Size="S" SuppressLabel="True" TabIndex="-1" />
										<px:PXLabel ID="lblPhone2" runat="server" Text=" " SuppressLabel="true" />
										<px:PXMaskEdit ID="PXMaskEdit2" runat="server" DataField="Phone2" SuppressLabel="True" LabelWidth="34px" />
									<px:PXLayoutRule runat="server" Merge="True" />
										<px:PXDropDown ID="FaxType" runat="server" DataField="FaxType" Size="S" SuppressLabel="True" TabIndex="-1" />
										<px:PXLabel ID="lblFax" runat="server" Text=" " SuppressLabel="true" />
										<px:PXMaskEdit ID="PXMaskEdit3" runat="server" DataField="Fax" SuppressLabel="True" LabelWidth="34px" />
									<px:PXLayoutRule runat="server" />
									<px:PXMailEdit ID="edEMail" runat="server" DataField="EMail" CommitChanges="True" />
									<px:PXLinkEdit ID="edWebSite" runat="server" DataField="WebSite" CommitChanges="True" />
							</Template>
						</px:PXFormView>

					<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Service Management" />
						<px:PXCheckBox runat="server" ID="edRequireCustomerSignature" DataField="RequireCustomerSignature" TabIndex="-1" />
					<px:PXLayoutRule runat="server" />
						<px:PXSelector runat="server" ID="edBillingCycleID1" DataField="BillingCycleID" CommitChanges="True" AllowEdit="True" />
						<px:PXDropDown runat="server" ID="edSendInvoicesTo" DataField="SendInvoicesTo" />
						<px:PXDropDown runat="server" ID="edBillShipmentSource" DataField="BillShipmentSource" />
	                    <px:PXDropDown runat="server" ID="edDefaultBillingCustomerSource" DataField="DefaultBillingCustomerSource" CommitChanges="True" />
	                    <px:PXSegmentMask runat="server" ID="edBillCustomerID" DataField="BillCustomerID" CommitChanges="True" AutoRefresh="True" />
	                    <px:PXSegmentMask runat="server" ID="edBillLocationID" DataField="BillLocationID" AutoRefresh="True" />

					<%-- column 2 --%>

					<px:PXLayoutRule runat="server" StartColumn="True" />
					<px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" />
					<px:PXLayoutRule runat="server" GroupCaption="Parent Info" StartGroup="True" />
						<px:PXSegmentMask ID="edParentBAccountID" runat="server" DataField="ParentBAccountID" AllowEdit="True" CommitChanges="true" AutoRefresh="true" />
						<px:PXCheckBox ID="edConsolidateToParent" runat="server" DataField="ConsolidateToParent" CommitChanges="true" TabIndex="-1" />
						<px:PXCheckBox ID="edConsolidateStatements" runat="server" DataField="ConsolidateStatements" CommitChanges="true" TabIndex="-1" />
						<px:PXCheckBox ID="edSharedCreditPolicy" runat="server" DataField="SharedCreditPolicy" CommitChanges="True" TabIndex="-1" />

					<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Print and Email Settings" />
					<px:PXLayoutRule runat="server" ControlSize="M" LabelsWidth="M" Merge="True" SuppressLabel="true" />
						<px:PXCheckBox ID="chkMailInvoices" runat="server" DataField="MailInvoices" Size="M" TabIndex="-1" />
						<px:PXCheckBox CommitChanges="True" ID="chkPrintInvoices" runat="server" DataField="PrintInvoices" Size="M" TabIndex="-1" />
					<px:PXLayoutRule runat="server" />
					<px:PXLayoutRule runat="server" ControlSize="M" LabelsWidth="M" Merge="True" SuppressLabel="true" />
						<px:PXCheckBox ID="chkMailDunningLetters" runat="server" DataField="MailDunningLetters" Size="M" CommitChanges="true" TabIndex="-1" />
						<px:PXCheckBox ID="chkPrintDunningLetters" runat="server" DataField="PrintDunningLetters" Size="M" CommitChanges="true" TabIndex="-1" />
					<px:PXLayoutRule runat="server" />
					<px:PXLayoutRule runat="server" ControlSize="M" LabelsWidth="M" Merge="True" SuppressLabel="true" />
						<px:PXCheckBox ID="chkSendStatementByEmails" runat="server" DataField="SendStatementByEmail" Size="M" CommitChanges="true" TabIndex="-1" />
						<px:PXCheckBox ID="chkPrintStatements" runat="server" DataField="PrintStatements" Size="M" CommitChanges="true" TabIndex="-1" />
					<px:PXLayoutRule runat="server" />
						<px:PXDropDown CommitChanges="True" ID="edStatementType" runat="server" DataField="StatementType" />
						<px:PXCheckBox ID="chkPrintCuryStatements" runat="server" DataField="PrintCuryStatements" CommitChanges="true" TabIndex="-1" />

					<px:PXLayoutRule runat="server" GroupCaption="Default Payment Method" StartGroup="True" />
					<px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="XM" />
						<px:PXSelector ID="edDefPaymentMethodID" runat="server" DataField="DefPaymentMethodID" />
						<px:PXFormView ID="DefPaymentMethodInstance" runat="server" DataMember="DefPaymentMethodInstance" RenderStyle="Simple">
							<Template>
								<px:PXLayoutRule runat="server" StartGroup="True" ControlSize="XM" LabelsWidth="SM" />
									<px:PXSelector ID="edProcessingCenter" runat="server" AutoRefresh="True" DataField="CCProcessingCenterID" CommitChanges="True" />
									<px:PXSelector ID="edCustomerCCPID" runat="server" AutoRefresh="True" DataField="CustomerCCPID" CommitChanges="True" />
									<px:PXSegmentMask CommitChanges="True" ID="edCashAccountID" runat="server" DataField="CashAccountID" AutoRefresh ="true"/>
									<px:PXTextEdit ID="edDescr" runat="server" DataField="Descr" />
									<px:PXGrid ID="grdPMInstanceDetails" runat="server" MatrixMode="True" Caption="Payment Method Details" SkinID="Attributes" Height="160px" Width="400px" DataSourceID="ds">
										<Levels>
											<px:PXGridLevel DataMember="DefPaymentMethodInstanceDetails" DataKeyNames="PMInstanceID,PaymentMethodID,DetailID">
												<Columns>
													<px:PXGridColumn DataField="DetailID_PaymentMethodDetail_descr" />
													<px:PXGridColumn DataField="Value" />
												</Columns>
												<Layout FormViewHeight="" />
											</px:PXGridLevel>
										</Levels>
									</px:PXGrid>
							</Template>
						</px:PXFormView>
				</Template>
			</px:PXTabItem>

			<px:PXTabItem Text="Shipping" LoadOnDemand="True">
				<Template>

					<px:PXFormView ID="DefLocation" runat="server" DataMember="DefLocation" SkinID="Transparent" Width="100%">
						<Template>

							<px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" ></px:PXLayoutRule>
							<px:PXLayoutRule runat="server" GroupCaption="Ship-to Address" StartGroup="True" ></px:PXLayoutRule>

							<px:PXCheckBox ID="chkOverrideAddress" runat="server" DataField="OverrideAddress" CommitChanges="True" SuppressLabel="True"></px:PXCheckBox>

<px:PXFormView ID="DefLocationAddress" runat="server" DataMember="DefLocationAddress" RenderStyle="Simple">
	<Template>
		<px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" StartColumn="True" ></px:PXLayoutRule>
			<px:PXButton		ID="btnDefLocationAddressLookup"		runat="server" CommandName="DefLocationAddressLookup" CommandSourceID="ds" Size="xs" TabIndex=-1></px:PXButton>
			<px:PXButton		ID="btnViewDefLocationAddressOnMap"		runat="server" CommandName="ViewDefLocationAddressOnMap" CommandSourceID="ds" Size="xs" Text="View on Map" TabIndex=-1></px:PXButton>
			<px:PXTextEdit		ID="edDefLocationAddressAddressLine1"	runat="server" DataField="AddressLine1"	CommitChanges="True" ></px:PXTextEdit>
			<px:PXTextEdit		ID="edDefLocationAddressAddressLine2"	runat="server" DataField="AddressLine2"	CommitChanges="True" ></px:PXTextEdit>
			<px:PXTextEdit		ID="edDefLocationAddressCity"			runat="server" DataField="City"			CommitChanges="True" ></px:PXTextEdit>
			<px:PXSelector		ID="edDefLocationAddressState"			runat="server" DataField="State"		CommitChanges="True" AutoRefresh="True" AllowAddNew="True" ></px:PXSelector>
			<px:PXMaskEdit		ID="edPostalCode"						runat="server" DataField="PostalCode"	CommitChanges="True" ></px:PXMaskEdit>
			<px:PXSelector		ID="edDefLocationAddressCountryID"		runat="server" DataField="CountryID"	CommitChanges="True" AllowAddNew="True" ></px:PXSelector>
			<px:PXNumberEdit	ID="edLatitude"							runat="server" DataField="Latitude" AllowNull="True" ></px:PXNumberEdit>
			<px:PXNumberEdit	ID="edLongitude"						runat="server" DataField="Longitude" AllowNull="True" ></px:PXNumberEdit>

		<px:PXLayoutRule runat="server" ></px:PXLayoutRule>
			<px:PXCheckBox		ID="edDefLocationAddressIsValidated"	runat="server" DataField="IsValidated"	Enabled="False" ></px:PXCheckBox>
	</Template>
</px:PXFormView>

							<px:PXLayoutRule runat="server" GroupCaption="Ship-to Info" StartGroup="True" ></px:PXLayoutRule>

							<px:PXCheckBox ID="chkOverrideContact" runat="server" DataField="OverrideContact" CommitChanges="True"></px:PXCheckBox>

<px:PXFormView ID="DefLocationContact" runat="server" DataMember="DefLocationContact" RenderStyle="Simple">
	<Template>
		<px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" StartColumn="True" ></px:PXLayoutRule>
			<px:PXTextEdit ID="edDefLocationContactFullName"	runat="server" DataField="FullName" ></px:PXTextEdit>
			<px:PXTextEdit ID="edDefLocationContactAttention"	runat="server" DataField="Attention" ></px:PXTextEdit>
		<px:PXLayoutRule runat="server" Merge="True"></px:PXLayoutRule>
			<px:PXDropDown ID="Phone1Type"						runat="server" DataField="Phone1Type" Size="S" SuppressLabel="True"></px:PXDropDown>
			<px:PXLabel ID="lblPhone1"							runat="server" Text=" " SuppressLabel="true" ></px:PXLabel>
			<px:PXMaskEdit ID="PXMaskEdit1"						runat="server" DataField="Phone1" SuppressLabel="True" LabelWidth="34px" ></px:PXMaskEdit>
		<px:PXLayoutRule runat="server" Merge="True" ></px:PXLayoutRule>
			<px:PXDropDown ID="Phone2Type"						runat="server" DataField="Phone2Type" Size="S" SuppressLabel="True"></px:PXDropDown>
			<px:PXLabel ID="lblPhone2"							runat="server" Text=" " SuppressLabel="true" ></px:PXLabel>
			<px:PXMaskEdit ID="PXMaskEdit2"						runat="server" DataField="Phone2" SuppressLabel="True" LabelWidth="34px" ></px:PXMaskEdit>
		<px:PXLayoutRule runat="server" Merge="True" ></px:PXLayoutRule>
			<px:PXDropDown ID="FaxType"							runat="server" DataField="FaxType" Size="S" SuppressLabel="True"></px:PXDropDown>
			<px:PXLabel ID="lblFax"								runat="server" Text=" " SuppressLabel="true" ></px:PXLabel>
			<px:PXMaskEdit ID="PXMaskEdit3"						runat="server" DataField="Fax" SuppressLabel="True" LabelWidth="34px" ></px:PXMaskEdit>
		<px:PXLayoutRule runat="server" ></px:PXLayoutRule>
			<px:PXMailEdit ID="edDefLocationContactEMail"		runat="server" DataField="EMail" CommitChanges="True" ></px:PXMailEdit>
			<px:PXLinkEdit ID="edDefLocationContactWebSite"		runat="server" DataField="WebSite" CommitChanges="True" ></px:PXLinkEdit>
	</Template>
</px:PXFormView>

							<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Other Settings" ></px:PXLayoutRule>
								<px:PXSelector ID="edCBranchID" runat="server" DataField="CBranchID" AllowEdit="True" AutoRefresh="true"></px:PXSelector>
								<px:PXSelector ID="edCPriceClassID" runat="server" DataField="CPriceClassID" AllowEdit="True" ></px:PXSelector>
								<px:PXSelector ID="edCDefProjectID" runat="server" DataField="CDefProjectID" AllowEdit="True" ></px:PXSelector>
								<px:PXCheckBox ID="chkSuggestRelatedItems" runat="server" DataField="CurrentCustomer.SuggestRelatedItems" ></px:PXCheckBox>
							
							<px:PXLayoutRule runat="server" StartColumn="True" ></px:PXLayoutRule>
							<px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" ></px:PXLayoutRule>
							<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Tax Settings" ></px:PXLayoutRule>
								<px:PXTextEdit ID="edTaxRegistrationID" runat="server" DataField="TaxRegistrationID" ></px:PXTextEdit>
								<px:PXSelector ID="edCTaxZoneID" runat="server" DataField="CTaxZoneID" AllowEdit="True" ></px:PXSelector>
								<px:PXDropDown ID="edCTaxCalcMode" runat="server" DataField="CTaxCalcMode" ></px:PXDropDown>
								<px:PXTextEdit ID="edCAvalaraExemptionNumber" runat="server" DataField="CAvalaraExemptionNumber" ></px:PXTextEdit>
								<px:PXDropDown ID="edCAvalaraCustomerUsageType" runat="server" DataField="CAvalaraCustomerUsageType" ></px:PXDropDown>

							<px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" ></px:PXLayoutRule>
							<px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Shipping Instructions" ></px:PXLayoutRule>
								<px:PXSegmentMask ID="edCSiteID" runat="server" DataField="CSiteID" AllowEdit="True" CommitChanges="True" AutoRefresh="True" ></px:PXSegmentMask>
								<px:PXSelector CommitChanges="True" ID="edCarrierID" runat="server" DataField="CCarrierID" AllowEdit="True" ></px:PXSelector>
								<px:PXSelector ID="edShipTermsID" runat="server" DataField="CShipTermsID" AllowEdit="True" ></px:PXSelector>
								<px:PXSelector ID="edShipZoneID" runat="server" DataField="CShipZoneID" AllowEdit="True" ></px:PXSelector>
								<px:PXSelector ID="edFOBPointID" runat="server" DataField="CFOBPointID" AllowEdit="True" ></px:PXSelector>
								<px:PXCheckBox ID="chkResedential" runat="server" DataField="CResedential" TabIndex="-1" ></px:PXCheckBox>
								<px:PXCheckBox ID="chkSaturdayDelivery" runat="server" DataField="CSaturdayDelivery" TabIndex="-1" ></px:PXCheckBox>
								<px:PXCheckBox ID="chkInsurance" runat="server" DataField="CInsurance" TabIndex="-1" ></px:PXCheckBox>
								<px:PXDropDown ID="edCShipComplete" runat="server" DataField="CShipComplete" ></px:PXDropDown>
								<px:PXNumberEdit ID="edCOrderPriority" runat="server" DataField="COrderPriority" ></px:PXNumberEdit>
								<px:PXNumberEdit ID="edLeadTime" runat="server" DataField="CLeadTime" ></px:PXNumberEdit>
								<px:PXSelector ID="edCCalendarID" runat="server" DataField="CCalendarID" ></px:PXSelector>

								<px:PXGrid ID="PXGridAccounts" runat="server" DataSourceID="ds" AllowFilter="False" Height="80px" Width="400px" SkinID="ShortList" FastFilterFields="CustomerID,CustomerID_description,CarrierAccount,CustomerLocationID"
									CaptionVisible="True" Caption="Carrier Accounts">
									<Levels>
										<px:PXGridLevel DataMember="Carriers">
											<RowTemplate>
												<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" ></px:PXLayoutRule>
												<px:PXCheckBox ID="chkIsActive" runat="server" Checked="True" DataField="IsActive" ></px:PXCheckBox>
												<px:PXTextEdit ID="edCarrierAccount" runat="server" DataField="CarrierAccount"  ></px:PXTextEdit>
												<px:PXSegmentMask ID="edCustomerLocationID" runat="server" DataField="CustomerLocationID" AutoRefresh="True" ></px:PXSegmentMask>
												<px:PXMaskEdit ID="edPostalCode" runat="server" DataField="PostalCode" ></px:PXMaskEdit>
												<px:PXSelector ID="edCarrierPluginID"  runat="server" DataField="CarrierPluginID" AutoRefresh="True"  ></px:PXSelector>
											</RowTemplate>
											<Columns>
												<px:PXGridColumn DataField="IsActive" Label="Active" TextAlign="Center" Type="CheckBox" ></px:PXGridColumn>
												<px:PXGridColumn AutoCallBack="True" DataField="CarrierPluginID" Label="Carrier" ></px:PXGridColumn>
												<px:PXGridColumn DataField="CarrierAccount" ></px:PXGridColumn>
												<px:PXGridColumn DataField="CustomerLocationID" Label="Location" ></px:PXGridColumn>
												<px:PXGridColumn DataField="CountryID" ></px:PXGridColumn>
												<px:PXGridColumn DataField="PostalCode" ></px:PXGridColumn>
											</Columns>
											<Layout FormViewHeight="" ></Layout>
										</px:PXGridLevel>
									</Levels>
								</px:PXGrid>
							<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True" />
							<px:PXFormView runat="server" ID="CstFormView2" DataMember="CurrentCustomer" SkinID="Transparent">
								<Template>
									<px:PXTextEdit runat="server" ID="edUsrShippingInstructions" DataField="UsrShippingInstructions" Height="350" Width="300" LabelWidth="200" TextMode="MultiLine" />
									<px:PXLayoutRule runat="server" ID="CstPXLayoutRule5" StartRow="True" /></Template></px:PXFormView></Template>
					</px:PXFormView>
				</Template>
			</px:PXTabItem>

			<px:PXTabItem Text="Balances" LoadOnDemand="True" RepaintOnDemand="false">
				<Template>
					<px:PXGrid ID="PXGrid1" runat="server" Height="300px" Width="100%" SkinID="DetailsInTab" DataSourceID="ds">
						<Levels>
							<px:PXGridLevel DataMember="Balances">
								<Columns>
									<px:PXGridColumn DataField="BaseCuryID" />
									<px:PXGridColumn DataField="CurrentBal" />
									<px:PXGridColumn DataField="TotalPrepayments" />
									<px:PXGridColumn DataField="ConsolidatedBalance" />
									<px:PXGridColumn DataField="RetainageBalance" />
								</Columns>
								<RowTemplate>
									<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
									<px:PXTextEdit ID="edBaseCuryID" runat="server" DataField="BaseCuryID" />
									<px:PXNumberEdit ID="edCurrentBal" runat="server" DataField="CurrentBal" />
									<px:PXNumberEdit ID="edTotalPrepayments" runat="server" DataField="TotalPrepayments" />
									<px:PXNumberEdit ID="edConsolidatedBalance" runat="server" DataField="ConsolidatedBalance" />
									<px:PXNumberEdit ID="edRetainageBalance" runat="server" DataField="RetainageBalance" />

								</RowTemplate>
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" MinHeight="100" MinWidth="100" />
						<ActionBar>
							<Actions>
								<Save Enabled="False" />
								<AddNew Enabled="false" />
								<Delete Enabled="false" />
							</Actions>
						</ActionBar>
					</px:PXGrid>
				</Template>
			</px:PXTabItem>

			<px:PXTabItem Text="Locations" LoadOnDemand="True">
				<Template>
					<px:PXGrid ID="grdLocations" runat="server" DataSourceID="ds" Height="100%" Width="100%" SkinID="Inquire" AllowSearch="True"
							 SyncPosition="true" AllowFilter="True" FastFilterFields="LocationCD,Descr,Address__City,Address__State,Address__CountryID" >
						<Levels>
							<px:PXGridLevel DataMember="Locations">
								<Columns>

									<%-- common fields --%>
									<px:PXGridColumn DataField="IsActive" TextAlign="Center" Type="CheckBox" />
									<px:PXGridColumn DataField="LocationCD" LinkCommand="ViewLocation" />
									<px:PXGridColumn DataField="Descr" />
									<px:PXGridColumn DataField="IsDefault" TextAlign="Center" Type="CheckBox" />
									<px:PXGridColumn DataField="Address__City" />
									<px:PXGridColumn DataField="Address__State" RenderEditorText="True" />
									<px:PXGridColumn DataField="Address__CountryID" RenderEditorText="True" AutoCallBack="True" />

									<%-- hidden by default --%>
									<px:PXGridColumn DataField="Address__PostalCode" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="Address__State_description" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="Address__CountryID_description" Visible="false" SyncVisible="false" />



									<%-- custom fields --%>
									<px:PXGridColumn DataField="CPriceClassID" />

									<%-- hidden by default --%>
									<px:PXGridColumn DataField="CreatedByID_Description" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="CreatedDateTime" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="LastModifiedByID_Description" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="LastModifiedDateTime" Visible="false" SyncVisible="false" />

									<%-- other settings --%>
									<px:PXGridColumn DataField="CDefProjectID" Visible="false" SyncVisible="false" />

									<%-- tax settings --%>
									<px:PXGridColumn DataField="TaxRegistrationID" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="CTaxZoneID" />
									<px:PXGridColumn DataField="CTaxCalcMode" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="CAvalaraExemptionNumber" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="CAvalaraCustomerUsageType" Visible="false" SyncVisible="false" />

									<%-- shipping instructions --%>
									<px:PXGridColumn DataField="CSiteID" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="CCarrierID" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="CShipTermsID" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="CShipZoneID" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="CFOBPointID" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="CResedential" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="CSaturdayDelivery" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="CInsurance" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="CShipComplete" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="COrderPriority" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="CLeadTime" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="CCalendarID" Visible="false" SyncVisible="false" />

									<%-- GL accounts --%>
									<px:PXGridColumn DataField="CSalesAcctID" AutoCallBack="True" />
									<px:PXGridColumn DataField="CSalesSubID" />
									<px:PXGridColumn DataField="CARAccountID" />
									<px:PXGridColumn DataField="CARSubID" />
									<px:PXGridColumn DataField="CDiscountAcctID" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="CDiscountSubID" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="CFreightAcctID" Visible="false" SyncVisible="false" />
									<px:PXGridColumn DataField="CFreightSubID" Visible="false" SyncVisible="false" />

									<px:PXGridColumn DataField="CBranchID" />
									<px:PXGridColumn DataField="CBranchID_description" Visible="false" SyncVisible="false" />

								</Columns>
								<Layout FormViewHeight="" />
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" MinHeight="100" MinWidth="100" />
						<ActionBar>
							<Actions>
								<FilterBar ToolBarVisible="Top" Order="0" GroupIndex="3" />
							</Actions>
							<CustomItems>
								<px:PXToolBarButton ImageKey="AddNew" Tooltip="Add New Location" DisplayStyle="Image">
									<AutoCallBack Command="NewLocation" Target="ds" />
									<PopupCommand Command="RefreshLocation" Target="ds" />
								</px:PXToolBarButton>
								<px:PXToolBarButton Key="cmdViewLocation" Text="Location Details">
									<AutoCallBack Command="ViewLocation" Target="ds" />
									<PopupCommand Command="RefreshLocation" Target="ds" />
								</px:PXToolBarButton>
								<px:PXToolBarButton Text="Set as Default">
									<AutoCallBack Command="SetDefaultLocation" Target="ds" />
								</px:PXToolBarButton>
							</CustomItems>
						</ActionBar>
						<Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>

			<px:PXTabItem Text="Payment Methods">
				<Template>
					<px:PXGrid ID="grdPaymentMethods" runat="server" Height="300px" Width="100%" SkinID="DetailsInTab" DataSourceID="ds">
						<Levels>
							<px:PXGridLevel DataMember="PaymentMethods">
								<Columns>
									<px:PXGridColumn DataField="IsDefault" TextAlign="Center" Type="CheckBox" />
									<px:PXGridColumn DataField="PaymentMethodID" />
									<px:PXGridColumn DataField="Descr" />
									<px:PXGridColumn DataField="CashAccountID" />
									<px:PXGridColumn DataField="IsActive" TextAlign="Center" Type="CheckBox" />
									<px:PXGridColumn DataField="IsCustomerPaymentMethod" TextAlign="Center" Type="CheckBox" />
								</Columns>
								<Layout FormViewHeight="" />
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" MinHeight="150" />
						<ActionBar PagerVisible="False" DefaultAction="cmdViewPaymentMethod" PagerActionsText="True">
							<Actions>
								<Save Enabled="False" />
								<AddNew Enabled="False" />
								<Delete Enabled="False" />
								<EditRecord Enabled="False" />
							</Actions>
							<CustomItems>
								<px:PXToolBarButton Text="Add Payment Method">
									<AutoCallBack Command="AddPaymentMethod" Target="ds" />
									<PopupCommand Command="Cancel" Target="ds" />
								</px:PXToolBarButton>
								<px:PXToolBarButton Key="cmdViewPaymentMethod" Text="View Payment Method">
									<AutoCallBack Command="ViewPaymentMethod" Target="ds" />
									<PopupCommand Command="Cancel" Target="ds" />
								</px:PXToolBarButton>
							</CustomItems>
						</ActionBar>
					</px:PXGrid>
				</Template>
			</px:PXTabItem>

			<px:PXTabItem Text="Contacts" LoadOnDemand="true">
	<Template>
		<px:PXGrid ID="grdContacts" runat="server" DataSourceID="ds" Width="100%" SkinID="Inquire" AllowSearch="True"
			SyncPosition="true" ActionsPosition="Top" AllowFilter="True" FastFilterFields="DisplayName,Salutation,EMail,Phone1">
			<Levels>
				<px:PXGridLevel DataMember="Contacts">
					<Columns>
						<px:PXGridColumn DataField="IsActive" TextAlign="Center" Type="CheckBox" />
						<px:PXGridColumn DataField="DisplayName" LinkCommand="ViewContact" />
						<px:PXGridColumn DataField="Salutation" />
						<px:PXGridColumn DataField="IsPrimary" TextAlign="Center" Type="CheckBox"  />
						<px:PXGridColumn DataField="EMail" />
						<px:PXGridColumn DataField="Phone1" />

						<%-- hidden by default --%>
						<px:PXGridColumn DataField="OwnerID" DisplayMode="Text" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="FullName" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="ClassID" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="LastModifiedDateTime" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="CreatedDateTime" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="Source" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="AssignDate" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="DuplicateStatus" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="Phone2" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="Phone3" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="DateOfBirth" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="Fax" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="Gender" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="Method" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="NoCall" TextAlign="Center" Type="CheckBox" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="NoEMail" TextAlign="Center" Type="CheckBox" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="NoFax" TextAlign="Center" Type="CheckBox" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="NoMail" TextAlign="Center" Type="CheckBox" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="NoMarketing" TextAlign="Center" Type="CheckBox" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="NoMassMail" TextAlign="Center" Type="CheckBox" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="CampaignID" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="Phone1Type" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="Phone2Type" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="Phone3Type" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="FaxType" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="MaritalStatus" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="Spouse" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="Status" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="Resolution" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="LanguageID" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="ContactID" Visible="false" SyncVisible="false" />

						<px:PXGridColumn DataField="Address__CountryID" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="Address__State" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="Address__City" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="Address__AddressLine1" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="Address__AddressLine2" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="Address__PostalCode" Visible="false" SyncVisible="false" />

						<%-- hidden at all --%>
						<px:PXGridColumn DataField="CanBeMadePrimary" TextAlign="Center" Type="CheckBox"/>
					</Columns>
					<Layout FormViewHeight="" />
				</px:PXGridLevel>
			</Levels>
			<AutoSize Enabled="True" MinHeight="100" MinWidth="100" />
			<ActionBar DefaultAction="cmdViewContact" PagerVisible="False">
				<Actions>
					<FilterBar ToolBarVisible="Top" Order="0" GroupIndex="3" />
				</Actions>
				<CustomItems>
					<px:PXToolBarButton ImageKey="AddNew" Tooltip="Add New Contact" DisplayStyle="Image">
						<AutoCallBack Command="CreateContactToolBar" Target="ds" />
						<PopupCommand Command="refreshContact" Target="ds"/>
					</px:PXToolBarButton>
					<px:PXToolBarButton Text="Contact Details" Key="cmdViewContact" Visible="False">
						<AutoCallBack Command="ViewContact" Target="ds" />
						<PopupCommand Command="refreshContact" Target="ds" />
					</px:PXToolBarButton>
					<px:PXToolBarButton DependOnGrid="grdContacts" StateColumn="CanBeMadePrimary" >
						<AutoCallBack Command="MakeContactPrimary" Target="ds" />
					</px:PXToolBarButton>
				</CustomItems>
			</ActionBar>
			<Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
		</px:PXGrid>
	</Template>
</px:PXTabItem>

			<px:PXTabItem Text="Salespersons" LoadOnDemand="True">
				<Template>
					<px:PXGrid ID="gridSalespersons" runat="server" Height="300px" Width="100%" SkinID="DetailsInTab" DataSourceID="ds">
						<Levels>
							<px:PXGridLevel DataMember="SalesPersons" DataKeyNames="SalesPersonID,LocationID">
								<Columns>
									<px:PXGridColumn DataField="SalesPersonID" CommitChanges="True" />
									<px:PXGridColumn DataField="SalesPersonID_SalesPerson_descr" />
									<px:PXGridColumn DataField="LocationID" CommitChanges="True" />
									<px:PXGridColumn DataField="LocationID_description" />
									<px:PXGridColumn DataField="CommisionPct" TextAlign="Right" />
									<px:PXGridColumn DataField="IsDefault" Type="CheckBox" TextAlign="Center" />
								</Columns>
								<RowTemplate>
									<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
									<px:PXSegmentMask ID="edSalesPersonID" runat="server" DataField="SalesPersonID" AutoRefresh="True" AllowEdit="True" />
									<px:PXSegmentMask ID="edLocationID" runat="server" DataField="LocationID" AutoRefresh="True" AllowEdit="True" />
									<px:PXTextEdit ID="edLocation_descr" runat="server" DataField="LocationID_description" Enabled="False" />
									<px:PXNumberEdit ID="edCommisionPct" runat="server" DataField="CommisionPct" />
								</RowTemplate>
								<Mode InitNewRow="False" />
								<Layout FormViewHeight="" />
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" MinHeight="100" MinWidth="100" />
						<ActionBar>
							<Actions>
								<Save Enabled="False" />
							</Actions>
						</ActionBar>
					</px:PXGrid>
				</Template>
			</px:PXTabItem>

			<px:PXTabItem Text="Child Accounts" RepaintOnDemand="False" BindingContext="BAccount">
				<Template>
					<px:PXGrid ID="gridChildAccounts" runat="server" DataSourceID="ds" SkinID="DetailsInTab" Width="100%" Height="300px" AllowPaging="True" AdjustPageSize="Auto" >
						<Levels>
							<px:PXGridLevel DataMember="ChildAccounts">
								<Columns>
									<px:PXGridColumn DataField="CustomerID" />
									<px:PXGridColumn DataField="CustomerName" />
									<px:PXGridColumn DataField="BaseCuryID" />									
									<px:PXGridColumn DataField="Balance" TextAlign="Right" />
									<px:PXGridColumn DataField="SignedDepositsBalance" TextAlign="Right" />
									<px:PXGridColumn DataField="UnreleasedBalance" TextAlign="Right" />
									<px:PXGridColumn DataField="OpenOrdersBalance" TextAlign="Right" />
									<px:PXGridColumn DataField="OldInvoiceDate" />
									<px:PXGridColumn DataField="ConsolidateToParent" Type="CheckBox" TextAlign="Center" />
									<px:PXGridColumn DataField="ConsolidateStatements" Type="CheckBox" TextAlign="Center" />
									<px:PXGridColumn DataField="SharedCreditPolicy" Type="CheckBox" TextAlign="Center" />
									<px:PXGridColumn DataField="StatementCycleId" TextAlign="Right" />
								</Columns>
								<RowTemplate>
									<px:PXLayoutRule runat="server" StartColumn="True" />
									<px:PXSegmentMask ID="edChildCustomerID" runat="server" DataField="CustomerID" AllowEdit="True" />
								</RowTemplate>
								<Mode InitNewRow="False" />
								<Layout FormViewHeight="" />
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" MinHeight="100" MinWidth="100" />
						<ActionBar>
							<Actions>
								<AddNew ToolBarVisible="False" />
								<Delete ToolBarVisible="False" />
							</Actions>
						</ActionBar>
					</px:PXGrid>
				</Template>
			</px:PXTabItem>

			<px:PXTabItem Text="Attributes">
				<Template>
					<px:PXGrid ID="PXGridAnswers" runat="server" DataSourceID="ds" SkinID="Inquire" Width="100%"
						Height="200px" MatrixMode="True">
						<Levels>
							<px:PXGridLevel DataMember="Answers">
								<Columns>
									<px:PXGridColumn DataField="AttributeID" TextAlign="Left" AllowShowHide="False"
										TextField="AttributeID_description" />
									<px:PXGridColumn DataField="isRequired" TextAlign="Center" Type="CheckBox" />
									<px:PXGridColumn DataField="Value" AllowShowHide="False" AllowSort="False" />
								</Columns>
								<Layout FormViewHeight="" />
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" MinHeight="200" />
						<ActionBar>
							<Actions>
								<Search Enabled="False" />
							</Actions>
						</ActionBar>
						<Mode AllowAddNew="False" AllowColMoving="False" AllowDelete="False" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>

			<px:PXTabItem Text="Activities" LoadOnDemand="True" RepaintOnDemand="False">
	<Template>
		<pxa:PXGridWithPreview
				ID="gridActivities" runat="server" DataSourceID="ds" Width="100%" AllowSearch="True" DataMember="Activities" AllowPaging="true" NoteField="NoteText"
				FilesField="NoteFiles" BorderWidth="0px" GridSkinID="Inquire" SplitterStyle="z-index: 100; border-top: solid 1px Gray;  border-bottom: solid 1px Gray"
				PreviewPanelStyle="z-index: 100; background-color: Window" PreviewPanelSkinID="Preview" SyncPosition="True"	BlankFilterHeader="All Activities" MatrixMode="true" PrimaryViewControlID="form">

			<AutoSize Enabled="true" />
			<GridMode AllowAddNew="False" AllowDelete="False" AllowFormEdit="False" AllowUpdate="False" AllowUpload="False" />

			<ActionBar DefaultAction="cmdViewActivity" CustomItemsGroup="0">
				<CustomItems>
					<px:PXToolBarButton Key="cmdAddTask">
						<AutoCallBack Command="NewTask" Target="ds" />
						<PopupCommand Command="RefreshActivities" Target="ds" />
					</px:PXToolBarButton>
					<px:PXToolBarButton Key="cmdAddEvent">
						<AutoCallBack Command="NewEvent" Target="ds" />
						<PopupCommand Command="RefreshActivities" Target="ds" />
					</px:PXToolBarButton>
					<px:PXToolBarButton Key="cmdAddEmail">
						<AutoCallBack Command="NewMailActivity" Target="ds" />
						<PopupCommand Command="RefreshActivities" Target="ds" />
					</px:PXToolBarButton>
					<px:PXToolBarButton Key="cmdAddActivity">
						<AutoCallBack Command="NewActivity" Target="ds" />
						<PopupCommand Command="RefreshActivities" Target="ds" />
					</px:PXToolBarButton>
					<px:PXToolBarButton Key="cmdViewActivity">
						<AutoCallBack Command="ViewActivity" Target="ds" />
						<PopupCommand Command="RefreshActivities" Target="ds" />
					</px:PXToolBarButton>
					<px:PXToolBarButton Tooltip="Pin or unpin the activity">
						<AutoCallBack Command="TogglePinActivity" Target="ds" />
					</px:PXToolBarButton>
				</CustomItems>
			</ActionBar>

			<Levels>
				<px:PXGridLevel DataMember="Activities">
					<Columns>
						<px:PXGridColumn DataField="IsPinned"					Width="21px" AllowFilter="False" AllowResize="False" AllowSort="False" />
						<px:PXGridColumn DataField="IsCompleteIcon"				Width="21px" AllowFilter="False" AllowResize="False" AllowSort="False" />
						<px:PXGridColumn DataField="PriorityIcon"				Width="21px" AllowFilter="False" AllowResize="False" AllowSort="False" />
						<px:PXGridColumn DataField="CRReminder__ReminderIcon"	Width="21px" AllowFilter="False" AllowResize="False" AllowSort="False" />
						<px:PXGridColumn DataField="ClassIcon"					Width="35px" AllowFilter="False" AllowResize="False" AllowSort="False" />
						<px:PXGridColumn DataField="ClassInfo" Width="90px" />
						<px:PXGridColumn DataField="Subject" LinkCommand="ViewActivity" Width="400px" />
						<px:PXGridColumn DataField="UIStatus" />
						<px:PXGridColumn DataField="Released" TextAlign="Center" Type="CheckBox" />
						<px:PXGridColumn DataField="StartDate" DisplayFormat="g" Width="125px" />
						<px:PXGridColumn DataField="CompletedDate" DisplayFormat="g" Width="125px" />
						<px:PXGridColumn DataField="TimeSpent" />
						<px:PXGridColumn DataField="CreatedByID" Visible="False" AllowShowHide="False" />
						<px:PXGridColumn DataField="CreatedByID_Creator_Username" Visible="False" SyncVisible="False" SyncVisibility="False" Width="108px">
							<NavigateParams>
								<px:PXControlParam Name="PKID" ControlID="gridActivities" PropertyName="DataValues[&quot;CreatedByID&quot;]" />
							</NavigateParams>
						</px:PXGridColumn>
						<px:PXGridColumn DataField="OwnerID" LinkCommand="OpenActivityOwner" DisplayMode="Text" />
						<px:PXGridColumn DataField="Source" Visible="False" SyncVisible="False" />
						<px:PXGridColumn DataField="BAccountID" LinkCommand="<stub>" Visible="False" SyncVisible="False" />
						<px:PXGridColumn DataField="ContactID" DisplayMode="Text" LinkCommand="<stub>" Visible="False" SyncVisible="False" />
						<px:PXGridColumn DataField="ProjectID" Visible="False" SyncVisible="False" />
						<px:PXGridColumn DataField="ProjectTaskID" Visible="False" SyncVisible="False" />
						<px:PXGridColumn DataField="CreatedDateTime" DisplayFormat="g" Width="125px" Visible="False" SyncVisible="False" />
						<px:PXGridColumn DataField="WorkgroupID" Visible="False" SyncVisible="False" />
					</Columns>
				</px:PXGridLevel>
			</Levels>

			<PreviewPanelTemplate>
				<px:PXHtmlView ID="edBody" runat="server" DataField="body" TextMode="MultiLine" MaxLength="50" Width="100%" Height="100px" SkinID="Label" >
					<AutoSize Container="Parent" Enabled="true" />
				</px:PXHtmlView>
			</PreviewPanelTemplate>

		</pxa:PXGridWithPreview>
	</Template>
</px:PXTabItem>

			<px:PXTabItem Text="GL Accounts">
				<Template>
					<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
					<px:PXFormView ID="DefLocationAccount" runat="server" DataMember="DefLocation" RenderStyle="Simple" TabIndex="2500">
						<Template>
							<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
							<px:PXSegmentMask CommitChanges="True" ID="edCARAccountID" runat="server" DataField="CARAccountID" />
							<px:PXSegmentMask CommitChanges="True" ID="edCARSubID" runat="server" DataField="CARSubID" AutoRefresh="True" AllowEdit="True" />
							<px:PXSegmentMask CommitChanges="True" ID="edCSalesAcctID" runat="server" DataField="CSalesAcctID" />
							<px:PXSegmentMask CommitChanges="True" ID="edCSalesSubID" runat="server" DataField="CSalesSubID" AutoRefresh="True" AllowEdit="True" />
							<px:PXSegmentMask CommitChanges="True" ID="edCDiscountAcctID" runat="server" DataField="CDiscountAcctID" />
							<px:PXSegmentMask CommitChanges="True" ID="edCDiscountSubID" runat="server" DataField="CDiscountSubID" AutoRefresh="True" AllowEdit="True" />
							<px:PXSegmentMask CommitChanges="True" ID="edCFreightAcctID" runat="server" DataField="CFreightAcctID" />
							<px:PXSegmentMask CommitChanges="True" ID="edCFreightSubID" runat="server" DataField="CFreightSubID" AutoRefresh="True" AllowEdit="True" />
						</Template>
					</px:PXFormView>
					<px:PXSegmentMask CommitChanges="True" ID="edDiscTakenAcctID" runat="server" DataField="DiscTakenAcctID" />
					<px:PXSegmentMask CommitChanges="True" ID="edDiscTakenSubID" runat="server" DataField="DiscTakenSubID" AutoRefresh="True" AllowEdit="True" />
					<px:PXSegmentMask CommitChanges="True" ID="edPrepaymentAcctID" runat="server" DataField="PrepaymentAcctID" />
					<px:PXSegmentMask CommitChanges="True" ID="edPrepaymentSubID" runat="server" DataField="PrepaymentSubID" AutoRefresh="True" AllowEdit="True" />
					<px:PXSegmentMask CommitChanges="True" ID="edCOGSAcctID" runat="server" DataField="COGSAcctID" />
					<px:PXFormView ID="formRetainage" runat="server" DataMember="DefLocation" RenderStyle="Simple" >
						<Template>
							<px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="M" StartColumn="True" />
							<px:PXSegmentMask ID="edCRetainageAcctID" runat="server" CommitChanges="True" DataField="CRetainageAcctID" />
							<px:PXSegmentMask ID="edCRetainageSubID" runat="server" DataField="CRetainageSubID" CommitChanges="True" />
						</Template>
					</px:PXFormView>
				</Template>
			</px:PXTabItem>

			<px:PXTabItem Text="Mailing && Printing" Overflow="Hidden" LoadOnDemand="True">
				<Template>
					<px:PXSplitContainer runat="server" ID="sp1" SplitterPosition="300" SkinID="Horizontal" Height="500px">
						<AutoSize Enabled="true" />
						<Template1>
							<px:PXGrid ID="gridNS" runat="server" SkinID="DetailsInTab" Width="100%" Height="150px" Caption="Mailings"
								AdjustPageSize="Auto" AllowPaging="True" DataSourceID="ds">
								<AutoSize Enabled="True" />
								<AutoCallBack Target="gridNR" Command="Refresh" />
								<Levels>
									<px:PXGridLevel DataMember="NotificationSources" DataKeyNames="SourceID,SetupID">
										<RowTemplate>
											<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
											<px:PXDropDown ID="edFormat" runat="server" DataField="Format" />
											<px:PXSegmentMask ID="edNBranchID" runat="server" DataField="NBranchID" />
											<px:PXCheckBox ID="chkActive" runat="server" Checked="True" DataField="Active" />
											<px:PXSelector ID="edSetupID" runat="server" DataField="SetupID" />
											<px:PXSelector ID="edReportID" runat="server" DataField="ReportID" ValueField="ScreenID" />
											<px:PXSelector ID="edNotificationID" runat="server" DataField="NotificationID" ValueField="Name" />
											<px:PXSelector ID="edEMailAccountID" runat="server" DataField="EMailAccountID" DisplayMode="Text" />
										</RowTemplate>
										<Columns>
											<px:PXGridColumn DataField="Active" TextAlign="Center" Type="CheckBox" />
											<px:PXGridColumn DataField="OverrideSource" TextAlign="Center" Type="CheckBox" AutoCallBack="True" />
											<px:PXGridColumn DataField="SetupID" AutoCallBack="True" />
											<px:PXGridColumn DataField="NBranchID" AutoCallBack="True" Label="Branch" />
											<px:PXGridColumn DataField="EMailAccountID" DisplayMode="Text" />
											<px:PXGridColumn DataField="ReportID" AutoCallBack="True" />
											<px:PXGridColumn DataField="NotificationID" AutoCallBack="True" />
											<px:PXGridColumn DataField="Format" RenderEditorText="True" AutoCallBack="True" />
											<px:PXGridColumn DataField="RecipientsBehavior" CommitChanges="True" />
										</Columns>
										<Layout FormViewHeight="" />
									</px:PXGridLevel>
								</Levels>
							</px:PXGrid>
						</Template1>
						<Template2>
							<px:PXGrid ID="gridNR" runat="server" SkinID="DetailsInTab" Width="100%" Caption="Recipients" AdjustPageSize="Auto"
								AllowPaging="True" DataSourceID="ds">
								<AutoSize Enabled="True" />
								<Mode InitNewRow="True"></Mode>
								<Parameters>
									<px:PXSyncGridParam ControlID="gridNS" />
								</Parameters>
								<CallbackCommands>
									<Save RepaintControls="None" RepaintControlsIDs="ds" />
									<FetchRow RepaintControls="None" />
								</CallbackCommands>
								<Levels>
									<px:PXGridLevel DataMember="NotificationRecipients" DataKeyNames="NotificationID">
										<Mode InitNewRow="True"></Mode>
										<Columns>
											<px:PXGridColumn DataField="Active" TextAlign="Center" Type="CheckBox" />
											<px:PXGridColumn DataField="ContactType" RenderEditorText="True" AutoCallBack="True" />
											<px:PXGridColumn DataField="OriginalContactID" Visible="False" AllowShowHide="False" />
											<px:PXGridColumn DataField="ContactID">
												<NavigateParams>
													<px:PXControlParam Name="ContactID" ControlID="gridNR" PropertyName="DataValues[&quot;OriginalContactID&quot;]" />
												</NavigateParams>
											</px:PXGridColumn>
											<px:PXGridColumn DataField="Email" />
											<px:PXGridColumn DataField="Format" RenderEditorText="True" AutoCallBack="True" />
											<px:PXGridColumn DataField="AddTo" />
										</Columns>
										<RowTemplate>
											<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
											<px:PXDropDown ID="edContactType" runat="server" DataField="ContactType" />
											<px:PXSelector ID="edContactID" runat="server" DataField="ContactID" AutoRefresh="True" ValueField="DisplayName"
												AllowEdit="True" />
										</RowTemplate>
										<Layout FormViewHeight="" />
									</px:PXGridLevel>
								</Levels>
							</px:PXGrid>
						</Template2>
					</px:PXSplitContainer>
				</Template>
			</px:PXTabItem>

			<px:PXTabItem LoadOnDemand="True" Text="Service Billing" RepaintOnDemand="False" VisibleExp="DataControls[&quot;chkServiceManagement&quot;].Value == 1" BindingContext="BAccount">
				<Template>
					<px:PXGrid runat="server" ID="gridBillingCycles" AutoGenerateColumns="None" SkinID="DetailsInTab" DataSourceID="ds" SyncPosition="True" RepaintColumns="True" KeepPosition="True" Style='height:300px;width:100%;'>
						<Levels>
							<px:PXGridLevel DataMember="CustomerBillingCycles">
								<Columns>
									<px:PXGridColumn DataField="SrvOrdType" CommitChanges="True" />
									<px:PXGridColumn DataField="BillingCycleID" CommitChanges="True" />
									<px:PXGridColumn DataField="SendInvoicesTo" CommitChanges="True" />
									<px:PXGridColumn DataField="BillShipmentSource" CommitChanges="True" />
									<px:PXGridColumn DataField="FrequencyType" CommitChanges="True" />
									<px:PXGridColumn DataField="WeeklyFrequency" />
									<px:PXGridColumn DataField="MonthlyFrequency" />
								</Columns>
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" MinHeight="100" MinWidth="100" />
						<Mode InitNewRow="True" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>

            <px:PXTabItem Text="Compliance">
                <Template>
                    <px:PXGrid runat="server" ID="grdComplianceDocuments" Height="300px" DataSourceID="ds" Width="100%" SkinID="DetailsInTab" AutoGenerateColumns="Append" KeepPosition="True" SyncPosition="True" AllowPaging="True" PageSize="12">
                        <Levels>
                            <px:PXGridLevel DataMember="ComplianceDocuments">
                                <Columns>
                                    <px:PXGridColumn DataField="ExpirationDate" CommitChanges="True" TextAlign="Left" />
                                    <px:PXGridColumn DataField="DocumentType" CommitChanges="True" />
                                    <px:PXGridColumn DataField="CreationDate" TextAlign="Left" />
                                    <px:PXGridColumn DataField="Status" CommitChanges="True" />
                                    <px:PXGridColumn DataField="Required" Type="CheckBox" TextAlign="Center" />
                                    <px:PXGridColumn DataField="Received" Type="CheckBox" TextAlign="Center" />
                                    <px:PXGridColumn DataField="ReceivedDate" TextAlign="Left" />
                                    <px:PXGridColumn DataField="IsProcessed" Type="CheckBox" TextAlign="Center" />
                                    <px:PXGridColumn DataField="IsVoided" Type="CheckBox" TextAlign="Center" />
                                    <px:PXGridColumn DataField="IsCreatedAutomatically" Type="CheckBox" TextAlign="Center" />
                                    <px:PXGridColumn DataField="SentDate" TextAlign="Left" />
                                    <px:PXGridColumn DataField="ProjectID" LinkCommand="ComplianceViewProject" />
                                    <px:PXGridColumn DataField="CostTaskID" LinkCommand="ComplianceViewCostTask" CommitChanges="True" />
                                    <px:PXGridColumn DataField="RevenueTaskID" LinkCommand="ComplianceViewRevenueTask" CommitChanges="True" />
                                    <px:PXGridColumn DataField="CostCodeID" LinkCommand="ComplianceViewCostCode" CommitChanges="True" />
                                    <px:PXGridColumn DataField="VendorID" LinkCommand="ComplianceViewVendor" CommitChanges="True" />
                                    <px:PXGridColumn DataField="VendorName" TextAlign="Left" />
                                    <px:PXGridColumn DataField="BillID" LinkCommand="ComplianceDocument$BillID$Link" CommitChanges="True" DisplayMode="Text" />
                                    <px:PXGridColumn DataField="BillAmount" TextAlign="Right" />
                                    <px:PXGridColumn DataField="CustomerID" CommitChanges="True" LinkCommand="ComplianceViewCustomer" />
                                    <px:PXGridColumn DataField="ApCheckID" LinkCommand="ComplianceDocument$ApCheckID$Link" DisplayMode="Text" CommitChanges="True" TextAlign="Left" />
                                    <px:PXGridColumn DataField="CheckNumber" TextAlign="Left" />
                                    <px:PXGridColumn DataField="ArPaymentID" LinkCommand="ComplianceDocument$ArPaymentID$Link" DisplayMode="Text" TextAlign="Left" />
                                    <px:PXGridColumn DataField="CertificateNumber" TextAlign="Left" />
                                    <px:PXGridColumn DataField="CreatedByID" />
                                    <px:PXGridColumn DataField="CustomerName" TextAlign="Left" />
                                    <px:PXGridColumn DataField="AccountID" TextAlign="Left" CommitChanges="True" />
                                    <px:PXGridColumn DataField="DateIssued" TextAlign="Left" />
                                    <px:PXGridColumn DataField="EffectiveDate" TextAlign="Left" />
                                    <px:PXGridColumn DataField="InsuranceCompany" TextAlign="Left" />
                                    <px:PXGridColumn DataField="InvoiceAmount" TextAlign="Right" />
                                    <px:PXGridColumn DataField="InvoiceID" LinkCommand="ComplianceDocument$InvoiceID$Link" CommitChanges="True" DisplayMode="Text" />
                                    <px:PXGridColumn DataField="IsExpired" Type="CheckBox" TextAlign="Center" />
                                    <px:PXGridColumn DataField="IsRequiredJointCheck" Type="CheckBox" TextAlign="Center" />
                                    <px:PXGridColumn DataField="JointAmount" TextAlign="Right" />
                                    <px:PXGridColumn DataField="JointRelease" TextAlign="Left" />
                                    <px:PXGridColumn DataField="JointReleaseReceived" Type="CheckBox" TextAlign="Center" />
                                    <px:PXGridColumn DataField="JointVendorInternalId" LinkCommand="ComplianceViewJointVendor" TextAlign="Left" />
                                    <px:PXGridColumn DataField="JointVendorExternalName" TextAlign="Left" />
                                    <px:PXGridColumn DataField="LastModifiedByID" />
                                    <px:PXGridColumn DataField="LienWaiverAmount" TextAlign="Right" />
                                    <px:PXGridColumn DataField="Limit" TextAlign="Right" />
                                    <px:PXGridColumn DataField="MethodSent" TextAlign="Left" />
                                    <px:PXGridColumn DataField="PaymentDate" TextAlign="Left" />
                                    <px:PXGridColumn DataField="ArPaymentMethodID" />
                                    <px:PXGridColumn DataField="ApPaymentMethodID" />
                                    <px:PXGridColumn DataField="Policy" TextAlign="Left" />
                                    <px:PXGridColumn DataField="ProjectTransactionID" LinkCommand="ComplianceDocument$ProjectTransactionID$Link" CommitChanges="True" DisplayMode="Text" TextAlign="Left" />
                                    <px:PXGridColumn DataField="PurchaseOrder" TextAlign="Left" DisplayMode="Text" CommitChanges="True" LinkCommand="ComplianceDocument$PurchaseOrder$Link" />
                                    <px:PXGridColumn DataField="PurchaseOrderLineItem" TextAlign="Left" />
                                    <px:PXGridColumn DataField="Subcontract" DisplayMode="Text" CommitChanges="True" LinkCommand="ComplianceDocument$Subcontract$Link" />
                                    <px:PXGridColumn DataField="SubcontractLineItem" TextAlign="Left" />
                                    <px:PXGridColumn DataField="ChangeOrderNumber" DisplayMode="Text" LinkCommand="ComplianceDocument$ChangeOrderNumber$Link" CommitChanges="True" />
                                    <px:PXGridColumn DataField="ReceiptDate" TextAlign="Left" />
                                    <px:PXGridColumn DataField="ReceiveDate" TextAlign="Left" />
                                    <px:PXGridColumn DataField="ReceivedBy" TextAlign="Left" />
                                    <px:PXGridColumn DataField="SecondaryVendorID" LinkCommand="ComplianceViewSecondaryVendor" CommitChanges="True" />
                                    <px:PXGridColumn DataField="SecondaryVendorName" TextAlign="Left" />
                                    <px:PXGridColumn DataField="SourceType" TextAlign="Left" />
                                    <px:PXGridColumn DataField="SponsorOrganization" TextAlign="Left" />
                                    <px:PXGridColumn DataField="ThroughDate" TextAlign="Left" />
                                    <px:PXGridColumn DataField="DocumentTypeValue" CommitChanges="True" />
                                </Columns>
                                <RowTemplate>
                                    <px:PXSegmentMask runat="server" DataField="CostCodeID" AutoRefresh="True" ID="edCostCode2" />
                                    <px:PXSelector runat="server" DataField="DocumentTypeValue" AutoRefresh="True" ID="edDocumentTypeValue" />
                                    <px:PXSelector runat="server" DataField="BillID" FilterByAllFields="True" AutoRefresh="True" ID="edBillID" />
                                    <px:PXSelector runat="server" DataField="InvoiceID" FilterByAllFields="True" AutoRefresh="True" ID="edInvoiceID" />
                                    <px:PXSelector runat="server" DataField="ApCheckID" FilterByAllFields="True" AutoRefresh="True" ID="edApCheckID" />
                                    <px:PXSelector runat="server" DataField="ArPaymentID" FilterByAllFields="True" AutoRefresh="True" ID="edArPaymentID" />
                                    <px:PXSelector runat="server" DataField="ProjectTransactionID" FilterByAllFields="True" AutoRefresh="True" ID="edProjectTransactionID" />
                                    <px:PXSelector runat="server" DataField="PurchaseOrder" FilterByAllFields="True" AutoRefresh="True" ID="edPurchaseOrder" CommitChanges="True" />
                                    <px:PXSelector runat="server" DataField="PurchaseOrderLineItem" AutoRefresh="True" ID="edPurchaseOrderLineItem" />
                                    <px:PXSelector runat="server" DataField="Subcontract" FilterByAllFields="True" AutoRefresh="True" CommitChanges="True" ID="edSubcontract" />
                                    <px:PXSelector runat="server" DataField="SubcontractLineItem" AutoRefresh="True" ID="edSubcontractLineItem" />
                                    <px:PXSelector runat="server" DataField="ChangeOrderNumber" AutoRefresh="True" ID="edChangeOrderNumber" />
                                    <px:PXSelector runat="server" DataField="CostTaskID" FilterByAllFields="True" AutoRefresh="True" ID="edCostTaskID" />
                                    <px:PXSelector runat="server" DataField="RevenueTaskID" FilterByAllFields="True" AutoRefresh="True" ID="edRevenueTaskID" />
                                </RowTemplate>
                            </px:PXGridLevel>
                        </Levels>
                        <ActionBar>
                            <CustomItems />
                        </ActionBar>
                        <CallbackCommands>
                            <InitRow CommitChanges="True" />
                        </CallbackCommands>
                        <Mode InitNewRow="True" />
                        <AutoSize Enabled="True" MinHeight="150" />
                    </px:PXGrid>
                </Template>
                <AutoCallBack>
                    <Behavior CommitChanges="True" />
                </AutoCallBack>
            </px:PXTabItem>

		</Items>
		<AutoSize Container="Window" Enabled="True" MinHeight="300" MinWidth="300" />
	</px:PXTab>

	<px:PXSmartPanel ID="PanelCreateContact" runat="server" Style="z-index: 108; position: absolute; left: 27px; top: 99px;" Caption="Create Contact"
	CaptionVisible="True" LoadOnDemand="true" AutoReload="true" Key="ContactInfo" AutoCallBack-Enabled="true" AutoCallBack-Target="formCreateContact" AutoCallBack-Command="Refresh" CloseButtonDialogResult="Abort"
	Width="520px" Height="314px" AllowResize="False">
	
	<px:PXTab ID="tabCreateContact" runat="server" Width="100%" DataSourceID="ds" DataMember="ContactInfo">
	
		<Items>

			<px:PXTabItem Text="Main" RepaintOnDemand="True">
				<Template>
					<px:PXFormView ID="formCreateContact" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" Height="242px" Caption="Creation Dialog" CaptionVisible="False" SkinID="Transparent"
									DataMember="ContactInfo">
						<Template>
							<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM"/>
							<px:PXTextEdit ID="edFirstName" runat="server" DataField="FirstName" CommitChanges="True" TabIndex="10"/>
							<px:PXTextEdit ID="edLastName" runat="server" DataField="LastName" CommitChanges="True" TabIndex="20"/>
							<px:PXTextEdit ID="edFullName" runat="server" DataField="FullName" CommitChanges="True" TabIndex="30"/>
							<px:PXTextEdit ID="edSalutation" runat="server" DataField="Salutation" CommitChanges="True" TabIndex="40"/>
							<px:PXLayoutRule runat="server" Merge="True" />
								<px:PXDropDown ID="edPhone1Type" runat="server" DataField="Phone1Type" Size="S" SuppressLabel="True" CommitChanges="True" TabIndex="-1"/>
								<px:PXLabel ID="lblPhone1" runat="server" Text=" " SuppressLabel="true" />
								<px:PXMaskEdit ID="edPhone1" runat="server" DataField="Phone1" SuppressLabel="True" LabelWidth="84px" CommitChanges="True" TabIndex="50"/>
							<px:PXLayoutRule runat="server" />
							<px:PXLayoutRule runat="server" Merge="True" />
								<px:PXDropDown ID="edPhone2Type" runat="server" DataField="Phone2Type" Size="S" SuppressLabel="True" CommitChanges="True" TabIndex="-1"/>
								<px:PXLabel ID="lblPhone2" runat="server" Text=" " SuppressLabel="true" />
								<px:PXMaskEdit ID="edPhone2" runat="server" DataField="Phone2" SuppressLabel="True" LabelWidth="84px" CommitChanges="True" TabIndex="60"/>
							<px:PXLayoutRule runat="server" />
							<px:PXTextEdit ID="edEmail" runat="server" DataField="Email" CommitChanges="True" TabIndex="70"/>
							<px:PXSelector ID="edContactClass" runat="server" DataField="ContactClass" CommitChanges="True" TabIndex="80" AutoRefresh="True"/>
						</Template>
					</px:PXFormView>
				</Template>
			</px:PXTabItem>

			<px:PXTabItem Text="Attributes" RepaintOnDemand="False">
				<Template>
					<px:PXGrid ID="grdContactInfoAttributes" runat="server" Width="100%" Height="211px" DataSourceID="ds" MatrixMode="True" AutoAdjustColumns="true">
						<Levels>
							<px:PXGridLevel DataMember="ContactInfoAttributes">
								<Columns>
									<px:PXGridColumn DataField="DisplayName" Width="244px"/>
									<px:PXGridColumn DataField="Value" AutoCallBack="True" CommitChanges="True" Width="244px"/>
								</Columns>
								<Layout ColumnsMenu="False" />
								<Mode AllowAddNew="false" AllowDelete="false" />
							</px:PXGridLevel>
						</Levels>
						<Mode InitNewRow="true" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>

			<px:PXTabItem Text="User-Defined Fields" RepaintOnDemand="false">
				<Template>
					<px:PXGrid ID="grdContactInfoUDF" runat="server" Width="100%" Height="211px" DataSourceID="ds" MatrixMode="True" AutoAdjustColumns="true">
						<Levels>
							<px:PXGridLevel DataMember="ContactInfoUDF">
								<Columns>
									<px:PXGridColumn DataField="DisplayName" Width="244px"/>
									<px:PXGridColumn DataField="Value" AutoCallBack="True" CommitChanges="True" Width="244px"/>
								</Columns>
								<Layout ColumnsMenu="False" />
								<Mode AllowAddNew="false" AllowDelete="false" />
							</px:PXGridLevel>
						</Levels>
						<Mode InitNewRow="true" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>

		</Items>

	</px:PXTab>

	<px:PXPanel ID="CreateContactBtn" runat="server" SkinID="Transparent">
		<px:PXButton ID="CreateContactBtnBack" runat="server" CommandSourceID="ds" CallbackUpdatable="true"
		             style="float: left; margin: 2px" Text="Back" DialogResult="No" Visible="false"/>
		
		<px:PXButton ID="CreateContactBtnCancel" runat="server" CommandName="CreateContactCancel"
		             style="float: right; margin: 2px" Text="Cancel" DialogResult="Abort"/>

		<px:PXButton ID="CreateContactBtnConvert" runat="server" CommandSourceID="ds" CommandName="CreateContactFinish"
		             style="float: right; margin: 2px" Text="Create" DialogResult="OK"/>
		<px:PXButton ID="CreateContactBtnReview" runat="server"  CommandSourceID="ds" CommandName="CreateContactFinishRedirect"
		             style="float: right; margin: 2px" Text="Create and review" DialogResult="Yes"/>
	</px:PXPanel>

</px:PXSmartPanel>

	<%
PX.Objects.CS.PXAddressLookup.RegisterClientScript(this, ds.DataGraph);
%>

<px:PXSmartPanel ID="AddressLookupPanel" runat="server" 
	Style="z-index: 108; position: absolute; left: 27px; top: 99px;" 
	Caption="Address Lookup"
	Width="75%"
	DefaultControlID="searchBox"
	height="440px"
	DataSourceID="ds"
	CaptionVisible="True" 
	Key="AddressLookupFilter" 
	CancelButtonID="AddressEntityBtnCancel"
	AllowResize="true"
	ShowMaximizeButton="True" 
	AutoRepaint="true"
	ClientEvents-AfterShow="addressLookupAPI.initializeMaps"
	ClientEvents-BeforeHide="addressLookupAPI.clearSearch">
<style>
.flex-container {
	display: flex; /* or inline-flex */
	flex-direction: column;
	height: 100%;
}
.flex-item {
}
.mapContainer, #addressautocompletemap{
	z-index: 1000;
}
.customNavBar {
	position:absolute;
	top: 10px;
	left: 10px;
	z-index: 10001;
}
.pac-container {
	z-index: 10001;
}
.pac-container:after {
	/* Disclaimer: not needed to show 'powered by Google' if also a Google Map is shown */

	background-image: none !important;
	height: 0px;
}
</style>
<script type='text/javascript'>
var addressLookupVars = (function () {
	var _searchQuery = "";
	return {
		setSearchQuery: function (v) { 
				_searchQuery = v;
			},
		getSearchQuery: function () { 
				return _searchQuery;
			}
	}
})();

var addressLookupPanel = (function() {

	function _addHhandleCallback(context, error) {
		if (context == null || context.info == null)
			return;

		var _searchQuery = _GetSearchQuery();
		if ((typeof _searchQuery == 'undefined') ||
			(_searchQuery == null) ||
			(_searchQuery == "") ||
			_searchQuery == addressLookupVars.getSearchQuery()) {
			addressLookupVars.setSearchQuery(_searchQuery);
			addressLookupAPI.disableInfoWindows();
			return;
		}
		addressLookupVars.setSearchQuery(_searchQuery);
		addressLookupAPI.geocodeQuery(_searchQuery);
	}

	function _GetFormattedAddress() {
		if(	!(px_alls.SearchResponseAddressLine1.getValue() + 
			px_alls.SearchResponseAddressLine2.getValue() +
			px_alls.SearchResponseCity.getValue() +
			px_alls.SearchResponseState.getValue() + 
			px_alls.SearchResponsePostalCode.getValue())
		) {
			return null;
		}

		return _GetFormattedAddressHelper(
			", ",
			[
				px_alls.SearchResponseAddressLine1.getValue(),
				px_alls.SearchResponseAddressLine2.getValue(),
				px_alls.SearchResponseCity.getValue(),
				_GetFormattedAddressHelper(
					" ", 
					[
						px_alls.SearchResponseState.getValue(), 
						px_alls.SearchResponsePostalCode.getValue()
					]
				),
				px_alls.SearchResponseCountry.getValue()
			]
		);
	}

	function _GetFormattedAddressHelper(separator, args) {
		var result = "";
		args.forEach(
			function(arg) {
				if (arg != null && arg != "") {
					if (result != "") {
						result = result + separator;
					}
					result = result + arg;
				}
			});
		return result;
	}

	function _GetSearchQuery() {
		var _addrBeginning = _GetFormattedAddressHelper(
				" ", 
				[
					px_alls.SearchResponseAddressLine1.getValue(),
					px_alls.SearchResponseAddressLine2.getValue(),
					px_alls.SearchResponseCity.getValue(),
					px_alls.SearchResponseState.getValue(),
					px_alls.SearchResponsePostalCode.getValue()
				]
			);

		if(_addrBeginning == null || _addrBeginning == "") {
			return "";
		}
		return _GetFormattedAddressHelper(
			" ",
			[
				_addrBeginning,
				px_alls.SearchResponseCountry.getValue()
			]
		);
	}

	function _CleanSearchResponseValues() {
		px_alls.SearchResponseAddressLine1.updateValue("");
		px_alls.SearchResponseAddressLine2.updateValue("");
		px_alls.SearchResponseAddressLine3.updateValue("");
		px_alls.SearchResponseCity.updateValue("");
		px_alls.SearchResponseState.updateValue("");
		px_alls.SearchResponseLongitude.updateValue("");
	}

	return {
		addHhandleCallback: _addHhandleCallback,
		GetFormattedAddressHelper: _GetFormattedAddressHelper,
		GetFormattedAddress: _GetFormattedAddress,
		CleanSearchResponseValues: _CleanSearchResponseValues
	}
})();
</script>

<div class="flex-container" >
	<div class="flex-item" style="height: inherit;">
			<div class="mapContainer" id="mapcontainer"  style="height: inherit;" >
				<div id='searchBoxContainer' class="customNavBar" style="position: absolute;top:10px; left:10px;right:200px; max-width: 600px">
					<px:PXFormView AutoRepaint="true" DefaultControlID="searchBox"  ID="AddressLookupPanelformAddress" runat="server" DataSourceID="ds"
						CaptionVisible="False" SkinID="Transparent"
						DataMember="AddressLookupFilter">
						<Template>
							<px:PXTextEdit ID="searchBox" SuppressLabel="True" style="width: 100%;" runat="server" DataField="SearchAddress">
								<ClientEvents Initialize="addressLookupAPI.bindAutocompleteSearchControl" />
							</px:PXTextEdit>
							<px:PXTextEdit ID="addressLookupViewName" runat="server" DataField="ViewName" />

							<px:PXTextEdit ID="SearchResponseAddressLine1" runat="server" DataField="AddressLine1" />
							<px:PXTextEdit ID="SearchResponseAddressLine2" runat="server" DataField="AddressLine2" />
							<px:PXTextEdit ID="SearchResponseAddressLine3" runat="server" DataField="AddressLine3" />
							<px:PXTextEdit ID="SearchResponseCity" runat="server" DataField="City" />
							<px:PXTextEdit ID="SearchResponseCountry" runat="server" DataField="CountryID" />
							<px:PXTextEdit ID="SearchResponseState" runat="server" DataField="State" />
							<px:PXTextEdit ID="SearchResponsePostalCode" runat="server" DataField="PostalCode" />
							<px:PXTextEdit ID="SearchResponseLatitude" runat="server" DataField="Latitude" />
							<px:PXTextEdit ID="SearchResponseLongitude" runat="server" DataField="Longitude" />
						</Template>
					</px:PXFormView>
				</div>
				<div id="addressautocompletemap" style="height:100%;"></div>
			</div>
	</div>
	<div class="flex-item">
		<px:PXPanel ID="AddressEntityBtn" Style="height: 40px;" runat="server"  height="40px" SkinID="Buttons">
			<px:PXButton CommandName="AddressLookupSelectAction" CommandSourceID="ds" ID="AddressLookupSelectAction" runat="server" Text="Select" DialogResult="Cancel">
				<ClientEvents Click="addressLookupAPI.fillInAddress" />
			</px:PXButton>
			<px:PXButton ID="AddressEntityBtnCancel" runat="server" DialogResult="Cancel" Text="Cancel" Style="margin-left: 5px;margin-right: 5px;" />
		</px:PXPanel>
	</div>
</div>
</px:PXSmartPanel>

	<px:PXSmartPanel ID="panelTeamsContact" runat="server"  Caption="Teams Contact" Width="400px" Height="290px" AutoRepaint="true"
	CaptionVisible="true" Key="TeamsContactCard" AutoCallBack-Enabled="true" AutoCallBack-Command="Refresh" 
	CallBackMode-CommitChanges="True" CallBackMode-PostData="Page">
	<px:PXFormView ID="formTeamsContact" runat="server" DataSourceID="ds" DataMember="TeamsContactCard" AllowCollapse="false" Width="100%" >
		<ContentStyle BackColor="Transparent" BorderStyle="None" />
		<Template>
			<px:PXImageView ID="edTeamsMemberPhoto" runat="server" DataField="PhotoFileName" Width="64" Height="64" />
			<px:PXLayoutRule runat="server" StartColumn="True" ControlSize="M" />
			<px:PXTextEdit ID="edDisplayName" runat="server" DataField="DisplayName" SuppressLabel="True" Width="190px" />
			<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="XXS" ControlSize="XXS" />
			<px:PXButton ID="edStatusIconContactOffline" runat="server" CommandSourceID="ds" CommandName="StatusIconContactOffline" CssClass="Button teamsImageButton teamsStatusIcon" Width="32" Height="32">
				<Images Normal="svg:teams@offline" />
			</px:PXButton>
			<px:PXButton ID="edStatusIconContactAvailable" runat="server" CommandSourceID="ds" CommandName="StatusIconContactAvailable" CssClass="Button teamsImageButton teamsStatusIcon" Width="32" Height="32">
				<Images Normal="svg:teams@available" />
			</px:PXButton>
			<px:PXButton ID="edStatusIconContactBusy" runat="server" CommandSourceID="ds" CommandName="StatusIconContactBusy" CssClass="Button teamsImageButton teamsStatusIcon" Width="32" Height="32">
				<Images Normal="svg:teams@busy" />
			</px:PXButton>
			<px:PXButton ID="edStatusIconContactAway" runat="server" CommandSourceID="ds" CommandName="StatusIconContactAway" CssClass="Button teamsImageButton teamsStatusIcon" Width="32" Height="32">
				<Images Normal="svg:teams@away" />
			</px:PXButton>
			<px:PXTextEdit ID="edStatus" runat="server" DataField="TeamsStatus"  ControlSize="M" CssClass="teamsStatusLabel" SuppressLabel="True" Width="132px" />
			<px:PXLayoutRule runat="server" Merge="False" StartRow="True" StartColumn="True" LabelsWidth="S" ControlSize="M" />
			<px:PXTextEdit ID="edJobTitle" runat="server" DataField="JobTitle" Style="margin-top:20px" />
			<px:PXTextEdit ID="edCompanyName" runat="server" DataField="CompanyName" />
			<px:PXTextEdit ID="edEmail" runat="server" DataField="Email" />
			<px:PXTextEdit ID="edMobilePhone" runat="server" DataField="MobilePhone" />
		</Template>
	</px:PXFormView>

	<div style="padding-left:74px; text-align: center;">
		<px:PXButton ID="edContactChat" runat="server" CommandSourceID="ds" CommandName="ContactChat" DialogResult="OK" CssClass="Button teamsLargeButton" width="40" height="50" ToolTip="Open Contact Chat">
			<Images Normal="svg:teams@teams_chat" />
		</px:PXButton>
		<px:PXButton ID="edContactCall" runat="server" CommandSourceID="ds" CommandName="ContactCall" DialogResult="OK" CssClass="Button teamsLargeButton" width="40" height="50" ToolTip="Open Contact Call">
			<Images Normal="svg:teams@teams_call" />
		</px:PXButton>
		<px:PXButton ID="edContactMeeting" runat="server" CommandSourceID="ds" CommandName="ContactMeeting" DialogResult="OK" CssClass="Button teamsLargeButton" width="40" height="50" ToolTip="Open Contact Meeting">
			<Images Normal="svg:teams@teams_event" />
		</px:PXButton>
	</div>
</px:PXSmartPanel>

<px:PXSmartPanel ID="panelTeamsOwner" runat="server"  Caption="Teams Contact" Width="400px" Height="290px" AutoRepaint="true"
	CaptionVisible="true" Key="TeamsOwnerCard" AutoCallBack-Enabled="true" AutoCallBack-Command="Refresh" 
	CallBackMode-CommitChanges="True" CallBackMode-PostData="Page">
	<px:PXFormView ID="formTeamsOwner" runat="server" DataSourceID="ds" DataMember="TeamsOwnerCard" AllowCollapse="false" Width="100%" >
		<ContentStyle BackColor="Transparent" BorderStyle="None" />
		<Template>
			<px:PXImageView ID="edTeamsMemberPhoto" runat="server" DataField="PhotoFileName" Width="64" Height="64" />
			<px:PXLayoutRule runat="server" StartColumn="True" ControlSize="M" />
			<px:PXTextEdit ID="edDisplayName" runat="server" DataField="DisplayName" SuppressLabel="True" Width="190px" />
			<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="XXS" ControlSize="XXS" />
			<px:PXButton ID="edStatusIconOwnerOffline" runat="server" CommandSourceID="ds" CommandName="StatusIconOwnerOffline" CssClass="Button teamsImageButton teamsStatusIcon" Width="32" Height="32">
				<Images Normal="svg:teams@offline" />
			</px:PXButton>
			<px:PXButton ID="edStatusIconOwnerAvailable" runat="server" CommandSourceID="ds" CommandName="StatusIconOwnerAvailable" CssClass="Button teamsImageButton teamsStatusIcon" Width="32" Height="32">
				<Images Normal="svg:teams@available" />
			</px:PXButton>
			<px:PXButton ID="edStatusIconOwnerBusy" runat="server" CommandSourceID="ds" CommandName="StatusIconOwnerBusy" CssClass="Button teamsImageButton teamsStatusIcon" Width="32" Height="32">
				<Images Normal="svg:teams@busy" />
			</px:PXButton>
			<px:PXButton ID="edStatusIconOwnerAway" runat="server" CommandSourceID="ds" CommandName="StatusIconOwnerAway" CssClass="Button teamsImageButton teamsStatusIcon" Width="32" Height="32">
				<Images Normal="svg:teams@away" />
			</px:PXButton>
			<px:PXTextEdit ID="edStatus" runat="server" DataField="TeamsStatus" ControlSize="M" CssClass="teamsStatusLabel" SuppressLabel="True" Width="132px" />
			<px:PXLayoutRule runat="server" Merge="False" StartRow="True" StartColumn="True" LabelsWidth="S" ControlSize="M" />
			<px:PXTextEdit ID="edJobTitle" runat="server" DataField="JobTitle" Style="margin-top:20px" />
			<px:PXTextEdit ID="edCompanyName" runat="server" DataField="CompanyName" />
			<px:PXTextEdit ID="edEmail" runat="server" DataField="Email" />
			<px:PXTextEdit ID="edMobilePhone" runat="server" DataField="MobilePhone" />
		</Template>
	</px:PXFormView>

	<div style="padding-left:74px; text-align: center;">
		<px:PXButton ID="edOwnerChat" runat="server" CommandSourceID="ds" CommandName="OwnerChat" DialogResult="OK" CssClass="Button teamsLargeButton" width="40" height="50" ToolTip="Open Teams Chat">
			<Images Normal="svg:teams@teams_chat" />
		</px:PXButton>
		<px:PXButton ID="edOwnerCall" runat="server" CommandSourceID="ds" CommandName="OwnerCall" DialogResult="OK" CssClass="Button teamsLargeButton" width="40" height="50" ToolTip="Open Teams Call">
			<Images Normal="svg:teams@teams_call" />
		</px:PXButton>
		<px:PXButton ID="edOwnerMeeting" runat="server" CommandSourceID="ds" CommandName="OwnerMeeting" DialogResult="OK" CssClass="Button teamsLargeButton" width="40" height="50" ToolTip="Open Teams Meeting">
			<Images Normal="svg:teams@teams_event" />
		</px:PXButton>
	</div>
</px:PXSmartPanel>

</asp:Content>
