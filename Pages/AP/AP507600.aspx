<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="AP507600.aspx.cs" Inherits="Page_AP507600" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="PX.Objects.Localizations.CA.T5018Fileprocessing"
        PrimaryView="MasterView" PageLoadBehavior="GoLastRecord">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="ViewDocument" PostData="Self" Visible="false" DependOnGrid="grid" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="MasterView" Width="100%" Height="180px">
        <Template>
            <px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
            <px:PXSelector CommitChanges="True" runat="server" ID="CstPXSegmentMask9" DataField="OrganizationID"></px:PXSelector>
            <px:PXSelector runat="server" ID="T5018Year" DataField="Year" AutoRefresh="True" CommitChanges="true"></px:PXSelector>
            <px:PXSelector runat="server" ID="Revision" DataField="Revision" AutoRefresh="True" CommitChanges="true"></px:PXSelector>
            <px:PXCheckBox runat="server" ID="RevisionSubmitted" DataField="RevisionSubmitted" CommitChanges="true"></px:PXCheckBox>
            <px:PXDropDown CommitChanges="True" runat="server" ID="CstPXDropDown4" DataField="FilingType"></px:PXDropDown>
            <px:PXTextEdit runat="server" ID="PXThresholdLimit1" DataField="ThresholdAmount" CommitChanges="true"></px:PXTextEdit>
            <px:PXLayoutRule runat="server" ID="CstPXLayoutRule20" StartColumn="True"></px:PXLayoutRule>
            <px:PXDateTimeEdit runat="server" ID="PXFromDate" DataField="FromDate"></px:PXDateTimeEdit>
            <px:PXDateTimeEdit runat="server" ID="PXToDate" DataField="ToDate"></px:PXDateTimeEdit>
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXTab ID="tab" runat="server" Width="100%" DataMember ="MasterViewSummary" DataSourceID="ds">
        <Items>
            <px:PXTabItem Text="Details">
                <Template>
                    <px:PXGrid ID="grid" runat="server" Width="100%" DataSourceID="ds" SkinID="Inquire" AllowPaging="true" AdjustPageSize="Auto">
                        <Levels>
                            <px:PXGridLevel DataMember="DetailsView">
                                <Columns>
                                    <px:PXGridColumn DataField="VAcctCD" Width="100"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="VendorName" Width="220"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="OrganizationName" Width="140"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="TotalServiceAmount" Width="100"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="Amount" Width="100" LinkCommand="ViewDocument"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="ReportType"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="TaxRegistrationID" Width="180"></px:PXGridColumn>
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Container="Window" Enabled="True" MinHeight="150"></AutoSize>
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Summary">
                <Template>                    
                    <px:PXLayoutRule GroupCaption="Settings" runat="server" ID="PXLayoutRule2" StartColumn="True"></px:PXLayoutRule>
                    <px:PXDropDown runat="server" ID="PXDropDown2" DataField="Language"></px:PXDropDown>
                    <px:PXTextEdit runat="server" ID="ProgramNumber" DataField="ProgramNumber"></px:PXTextEdit>                    
                    <px:PXTextEdit runat="server" ID="TransmitterNumber" DataField="TransmitterNumber"></px:PXTextEdit>
                    <px:PXLayoutRule GroupCaption="Company" runat="server" ID="PXLayoutRule4" StartColumn="True"></px:PXLayoutRule>
                    <px:PXTextEdit runat="server" ID="CstPXTextEdit7" DataField="AcctName"></px:PXTextEdit>
                    <px:PXTextEdit runat="server" ID="AddressLine1" DataField="AddressLine1"></px:PXTextEdit>
                    <px:PXTextEdit runat="server" ID="AddressLine2" DataField="AddressLine2"></px:PXTextEdit>
                    <px:PXTextEdit runat="server" ID="City" DataField="City"></px:PXTextEdit>
                    <px:PXTextEdit runat="server" ID="PXTextEdit2" DataField="Province"></px:PXTextEdit>
                    <px:PXTextEdit runat="server" ID="PXTextEdit1" DataField="Country"></px:PXTextEdit>
                    <px:PXTextEdit runat="server" ID="CstPXTextEdit6" DataField="PostalCode"></px:PXTextEdit>
                    <px:PXLayoutRule GroupCaption="Contact" runat="server" ID="PXLayoutRule8" StartColumn="True"></px:PXLayoutRule>
                    <px:PXTextEdit runat="server" ID="CstPXTextEdit11" DataField="Name"></px:PXTextEdit>
                    <px:PXTextEdit runat="server" ID="CstPXTextEdit9" DataField="AreaCode"></px:PXTextEdit>
                    <px:PXTextEdit runat="server" ID="Phone" DataField="Phone"></px:PXTextEdit>
                    <px:PXTextEdit runat="server" ID="CstPXTextEdit13" DataField="ExtensionNbr"></px:PXTextEdit>
                    <px:PXTextEdit runat="server" ID="CstPXTextEdit10" DataField="EMail"></px:PXTextEdit>
                    <px:PXTextEdit runat="server" ID="CstPXTextEdit14" DataField="SecondEmail"></px:PXTextEdit>
                </Template>
            </px:PXTabItem>
        </Items>
    </px:PXTab>
</asp:Content>
