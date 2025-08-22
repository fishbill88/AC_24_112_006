<%@ Page Language="C#" MasterPageFile="~/MasterPages/TabView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="PR405020.aspx.cs" Inherits="Page_PR405020" Title="Payroll Stubs" %>

<%@ MasterType VirtualPath="~/MasterPages/TabView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.PR.PRPayStubInq" PrimaryView="PayChecks" />
</asp:Content>

<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXTab ID="tab" runat="server" Width="100%">
        <Items>
            <px:PXTabItem Text="Pay Stubs" BindingContext="fakeForm2" >
                <Template>
                    <px:PXGrid ID="gridPayStubs" runat="server" DataSourceID="ds" Style="z-index: 100"
                        Width="100%" Height="150px" SkinID="Inquire" SyncPosition="true">
                        <ActionBar DefaultAction="viewStubReport" />
                        <Levels>
                            <px:PXGridLevel DataMember="PayChecks">
                                <Columns>
                                    <px:PXGridColumn DataField="TransactionDate" />
                                    <px:PXGridColumn DataField="DocType" />
                                    <px:PXGridColumn DataField="RefNbr" LinkCommand="viewStubReport" />
                                    <px:PXGridColumn DataField="PayGroupID" />
                                    <px:PXGridColumn DataField="PayPeriodID" />
                                    <px:PXGridColumn DataField="NetAmount" />
                                    <px:PXGridColumn DataField="GrossAmount" />
                                    <px:PXGridColumn DataField="StartDate" />
                                    <px:PXGridColumn DataField="EndDate" />
                                    <px:PXGridColumn DataField="TotalHours" />
                                    <px:PXGridColumn DataField="AverageRate" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
        </Items>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
    </px:PXTab>
    <px:PXFormView ID="fakeForm2" runat="server" DataMember="Filter" DataSourceID="ds">
        <Template>
            <px:PXCheckBox runat="server" ID="edShowTaxFormsTab" DataField="ShowTaxFormsTab"></px:PXCheckBox>
        </Template>
    </px:PXFormView>
</asp:Content>