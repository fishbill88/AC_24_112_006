<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true"
    ValidateRequest="false" CodeFile="ES504000.aspx.cs" Inherits="Page_ES504000"
    Title="Email Related Entity Process" %>

<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="EvolveSurplusCustomization.Graphs.ESEmailRelatedEntityProcess" PrimaryView="Filter">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="Process" CommitChanges="True" StartNewGroup="True" />
            <px:PXDSCallbackCommand Name="ProcessAll" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="viewEmail" Visible="False" DependOnGrid="grid" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>

<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100"
        Width="100%" DataMember="Filter" Caption="Selection" DefaultControlID="edEmailType">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />

            <px:PXDropDown ID="edEmailType" runat="server" DataField="EmailType" CommitChanges="True" />
            <px:PXSelector ID="edEmailFrom" runat="server" AutoRefresh="True" DataField="EmailFrom" CommitChanges="True" />

            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />

            <px:PXDropDown ID="edRefNoteIDType" runat="server" DataField="RefNoteIDType" CommitChanges="True" />
    
<!-- Entity-Specific Selectors - only one will be visible at a time based on RefNoteIDType -->
    <px:PXSelector ID="edBAccountID" runat="server" DataField="BAccountID" AutoRefresh="True" CommitChanges="True" />
            <px:PXSelector ID="edContactID" runat="server" DataField="ContactID" AutoRefresh="True" CommitChanges="True" />
        <px:PXSelector ID="edCustomerID" runat="server" DataField="CustomerID" AutoRefresh="True" CommitChanges="True" />
         <px:PXSelector ID="edVendorID" runat="server" DataField="VendorID" AutoRefresh="True" CommitChanges="True" />
 <px:PXSelector ID="edOpportunityID" runat="server" DataField="OpportunityID" AutoRefresh="True" CommitChanges="True" />
        <px:PXSelector ID="edLeadID" runat="server" DataField="LeadID" AutoRefresh="True" CommitChanges="True" />
  <px:PXSelector ID="edCaseID" runat="server" DataField="CaseID" AutoRefresh="True" CommitChanges="True" />
          <px:PXDropDown ID="edSalesOrderType" runat="server" DataField="SalesOrderType" CommitChanges="True" />
       <px:PXSelector ID="edSalesOrderNbr" runat="server" DataField="SalesOrderNbr" AutoRefresh="True" CommitChanges="True" />
<px:PXDropDown ID="edPurchaseOrderType" runat="server" DataField="PurchaseOrderType" CommitChanges="True" />
  <px:PXSelector ID="edPurchaseOrderNbr" runat="server" DataField="PurchaseOrderNbr" AutoRefresh="True" CommitChanges="True" />
    <px:PXSelector ID="edEmployeeID" runat="server" DataField="EmployeeID" AutoRefresh="True" CommitChanges="True" />
      
            <!-- AP Invoice -->
            <px:PXDropDown ID="edAPInvoiceDocType" runat="server" DataField="APInvoiceDocType" CommitChanges="True" />
            <px:PXSelector ID="edAPInvoiceRefNbr" runat="server" DataField="APInvoiceRefNbr" AutoRefresh="True" CommitChanges="True" />
            
    <!-- AP Payment -->
  <px:PXDropDown ID="edAPPaymentDocType" runat="server" DataField="APPaymentDocType" CommitChanges="True" />
 <px:PXSelector ID="edAPPaymentRefNbr" runat="server" DataField="APPaymentRefNbr" AutoRefresh="True" CommitChanges="True" />
       
      <!-- AR Invoice -->
 <px:PXDropDown ID="edARInvoiceDocType" runat="server" DataField="ARInvoiceDocType" CommitChanges="True" />
      <px:PXSelector ID="edARInvoiceRefNbr" runat="server" DataField="ARInvoiceRefNbr" AutoRefresh="True" CommitChanges="True" />
    
            <!-- AR Payment -->
          <px:PXDropDown ID="edARPaymentDocType" runat="server" DataField="ARPaymentDocType" CommitChanges="True" />
            <px:PXSelector ID="edARPaymentRefNbr" runat="server" DataField="ARPaymentRefNbr" AutoRefresh="True" CommitChanges="True" />
    
   <!-- Quote -->
            <px:PXSelector ID="edQuoteID" runat="server" DataField="QuoteID" AutoRefresh="True" CommitChanges="True" />
          
         <!-- Inventory Receipt -->
            <px:PXSelector ID="edInventoryReceiptRefNbr" runat="server" DataField="InventoryReceiptRefNbr" AutoRefresh="True" CommitChanges="True" />
    
            <!-- Project -->
            <px:PXSelector ID="edProjectID" runat="server" DataField="ProjectID" AutoRefresh="True" CommitChanges="True" />
            
<!-- Project Task -->
            <px:PXSelector ID="edProjectTaskID" runat="server" DataField="ProjectTaskID" AutoRefresh="True" CommitChanges="True" />
            
       <!-- Purchase Receipt -->
            <px:PXSelector ID="edPurchaseReceiptNbr" runat="server" DataField="PurchaseReceiptNbr" AutoRefresh="True" CommitChanges="True" />
        
            <!-- Shipment -->
            <px:PXDropDown ID="edShipmentType" runat="server" DataField="ShipmentType" CommitChanges="True" />
        <px:PXSelector ID="edShipmentNbr" runat="server" DataField="ShipmentNbr" AutoRefresh="True" CommitChanges="True" />

        </Template>
    </px:PXFormView>
</asp:Content>

<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100"
        Width="100%" Height="150px" SkinID="PrimaryInquire" Caption="Emails"
        FastFilterFields="Subject,MailFrom,MailTo" SyncPosition="True" AllowPaging="True"
        AdjustPageSize="Auto" TabIndex="300">
        <Levels>
            <px:PXGridLevel DataMember="Emails">
                <Columns>
                    <px:PXGridColumn DataField="Selected" Type="CheckBox" AllowCheckAll="True"
                        AllowSort="False" AllowMove="False" TextAlign="Center" Width="40px" />
                    <px:PXGridColumn DataField="Subject" Width="300px" LinkCommand="viewEmail" />
                    <px:PXGridColumn DataField="MailFrom" Width="200px" />
                    <px:PXGridColumn DataField="CreatedDateTime" Width="130px" DisplayFormat="g" />
                    <px:PXGridColumn DataField="MPStatus" Width="120px" />
                    <px:PXGridColumn DataField="MailTo" Width="200px" />
                    <px:PXGridColumn DataField="RefNoteIDType" Width="150px" />
                    <px:PXGridColumn DataField="WorkgroupID" Width="120px" DisplayMode="Text" />
                    <px:PXGridColumn DataField="OwnerID" Width="120px" DisplayMode="Text" />
                    <px:PXGridColumn DataField="CreatedByID" Width="120px" DisplayMode="Text" />
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
        <ActionBar DefaultAction="viewEmail">
            <CustomItems>
                <px:PXToolBarButton Text="View Email" Key="viewEmail" CommandSourceID="ds" CommandName="viewEmail" />
            </CustomItems>
        </ActionBar>
        <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
    </px:PXGrid>
</asp:Content>
