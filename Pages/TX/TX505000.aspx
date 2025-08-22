<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="TX505000.aspx.cs" Inherits="Page_TX505000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" Width="100%" runat="server" Visible="True" PrimaryView="Filter" TypeName="PX.Objects.TX.ManageExemptCustomer"/>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Width="100%" DataMember="Filter"> 
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
            <px:PXDropDown CommitChanges="True" ID="edAction" runat="server" DataField="Action" />
            <px:PXMultiSelector runat="server" DataField="CompanyCode" ID="edCompCode" CommitChanges="True" ValuesSeparator=","></px:PXMultiSelector>
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid ID="grid" runat="server" Height="400px" Width="100%" Style="z-index: 100" AllowPaging="true" AdjustPageSize="Auto" 
        AllowSearch="true" DataSourceID="ds" BatchUpdate="True" SkinID="PrimaryInquire" Caption="Documents" SyncPosition="True" FastFilterFields="AcctCD">
        <Levels>
            <px:PXGridLevel DataMember="CustomerList">
                <Columns>
                    <px:PXGridColumn AllowNull="False" DataField="Selected" TextAlign="Center" Type="CheckBox" AllowCheckAll="True" AllowSort="False" AllowMove="False" />
                    <px:PXGridColumn AllowUpdate="False" DataField="CustomerID" LinkCommand="viewCustomer"/>
                    <px:PXGridColumn AllowUpdate="False" DataField="CustomerName"/>
                    <px:PXGridColumn AllowUpdate="False" DataField="CustomerTaxRegistrationID"/>
                    <px:PXGridColumn AllowUpdate="False" DataField="AddressLine1"/>
                    <px:PXGridColumn AllowUpdate="False" DataField="AddressLine2"/>
                    <px:PXGridColumn AllowUpdate="False" DataField="City"/>
                    <px:PXGridColumn AllowUpdate="False" DataField="State"/>
                    <px:PXGridColumn AllowUpdate="False" DataField="PostalCode"/>
                    <px:PXGridColumn AllowUpdate="False" DataField="CountryID"/>
                    <px:PXGridColumn AllowUpdate="False" DataField="PrimaryContact"/>
                    <px:PXGridColumn AllowUpdate="False" DataField="PhoneNumber"/>
                    <px:PXGridColumn AllowUpdate="False" DataField="Email"/>
                    <px:PXGridColumn AllowUpdate="False" DataField="Fax"/>
                    <px:PXGridColumn AllowUpdate="False" DataField="EcmCompanyCode"/>
                    <px:PXGridColumn AllowUpdate="False" DataField="NoteID"/>
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="400" />
    </px:PXGrid>
</asp:Content>