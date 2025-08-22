<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="CA204500.aspx.cs" Inherits="Page_CA204500" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" AutoCallBack="True" Visible="True" Width="100%" PrimaryView="Rule" TypeName="PX.Objects.CA.CABankTranRuleMaint">
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataMember="Rule" DataSourceID="ds"
        Width="100%" >
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" ColumnWidth="L"/>
            <px:PXTextEdit ID="edRuleID" runat="server" Visible="False" DataField="RuleID" />
            <px:PXLayoutRule runat="server" ColumnSpan="2" />
            <px:PXTextEdit runat="server" DataField="Description" ID="edDescription" />
            <px:PXLayoutRule runat="server" ColumnSpan="2" />
            <px:PXCheckBox runat="server" DataField="IsActive" ID="edIsActive" />
                </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXTab ID="tab" runat="server" DataMember="Rule" DataSourceID="ds"
              Width="100%">
        <AutoSize Container="Window" Enabled="True"/>
        <Items>
            <px:PXTabItem Text="Matching Criteria">
                <Template>
                    <px:PXLayoutRule runat="server" StartColumn="true"/>
                    <px:PXLayoutRule runat="server" GroupCaption="General" StartGroup="True" />
            <px:PXDropDown runat="server" DataField="BankDrCr" ID="edBankDrCr" CommitChanges="true" />
            <px:PXSegmentMask runat="server" DataField="BankTranCashAccountID" ID="edCashAccountID" CommitChanges="true" />
            <px:PXSelector runat="server" DataField="TranCuryID" ID="edCuryID" />
            <px:PXMaskEdit runat="server" DataField="TranCode" ID="edTranCode" />
                    
                    <px:PXLayoutRule runat="server" GroupCaption="Description" StartGroup="True" />
            <px:PXTextEdit runat="server" DataField="BankTranDescription" ID="edBankTranDescription" />
            <px:PXCheckBox runat="server" DataField="MatchDescriptionCase" ID="edMatchDescriptionCase" />
            <px:PXCheckBox runat="server" DataField="UseDescriptionWildcards" ID="edUseDescriptionWildcards" />
                    
                    <px:PXLayoutRule runat="server" StartColumn="true"/>
                    <px:PXLayoutRule runat="server" GroupCaption="Payee/Payer" StartGroup="True" />
                    <px:PXTextEdit runat="server" DataField="PayeeName" ID="edPayeeName"/>
                    <px:PXCheckBox runat="server" DataField="UsePayeeNameWildcards" ID="edUsePayeeNameWildcards"/>
                    
                    <px:PXLayoutRule runat="server" GroupCaption="Amount" StartGroup="True" />
            <px:PXDropDown runat="server" DataField="AmountMatchingMode" ID="edAmountMatchingMode" CommitChanges="true" />
            <px:PXNumberEdit runat="server" DataField="CuryTranAmt" ID="edCuryTranAmt" AllowNull="true" />
            <px:PXNumberEdit runat="server" DataField="CuryMinTranAmt" ID="edCuryMinTranAmt" AllowNull="true" />
            <px:PXNumberEdit runat="server" DataField="MaxCuryTranAmt" ID="edMaxCuryTranAmt" AllowNull="true" />

                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Output">
                <Template>
                    <px:PXLayoutRule runat="server" StartColumn="true"/>
            <px:PXDropDown runat="server" DataField="Action" ID="edAction" CommitChanges="true" />
            <px:PXSelector runat="server" DataField="DocumentEntryTypeID" ID="edDocumentEntryType" AutoRefresh="true" />
        </Template>
            </px:PXTabItem>
        </Items>
    </px:PXTab>
</asp:Content>
