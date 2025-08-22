<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SO4050SP.aspx.cs" Inherits="Pages_SO_SO405000" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.SO.SOOrderInvoicesSP" PrimaryView="Filter" PageLoadBehavior="PopulateSavedValues"/>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Filter" Caption="Selection" DefaultControlID="edAction">
        <Activity HighlightColor="" SelectedColor="" Width="" Height=""></Activity>
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
            <px:PXSelector ID="edOrderType" aurelia="true" runat="server"  DataField="OrderType" DataSourceID="ds" CommitChanges="true" >
                <GridProperties>
                    <Layout ColumnsMenu="False" />
                </GridProperties>
            </px:PXSelector>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
            <px:PXSelector ID="edOrderNbr" runat="server" DataField="OrderNbr" AutoRefresh="True" DataSourceID="ds" CommitChanges="true">
                <GridProperties FastFilterFields="CustomerOrderNbr,CustomerID,CustomerID_Customer_acctName,CustomerRefNbr">
                    <Layout ColumnsMenu="False" />
                </GridProperties>
            </px:PXSelector>
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Width="100%" AllowPaging="true" AdjustPageSize="Auto" SkinID="PrimaryInquire"
        AllowSearch="true" NoteIndicator="false" FilesIndicator="false">
        <Levels>
            <px:PXGridLevel DataMember="Invoices">
                <RowTemplate>
                    <px:PXSelector ID="edRefNbr" runat="server" DataField="RefNbr" AllowEdit="true" />
                    <px:PXSelector ID="edSOOrderOrderNbr" runat="server" DataField="OrderNbr" AllowEdit="true" />
                    <px:PXSelector ID="edCustomerID" runat="server" DataField="CustomerID" AllowEdit="true" />
                </RowTemplate>
                <Columns>
                    <px:PXGridColumn DataField="DocType" />
                    <px:PXGridColumn DataField="RefNbr" />
                    <px:PXGridColumn DataField="Status" />
                    <px:PXGridColumn DataField="DocDate" />
                    <px:PXGridColumn DataField="DueDate" />
                    <px:PXGridColumn DataField="FinPeriodID" />
                    <px:PXGridColumn DataField="CuryOrigDocAmt" />
                    <px:PXGridColumn DataField="CuryDocBal" />
                    <px:PXGridColumn DataField="CuryID" />
                    <px:PXGridColumn DataField="OrderNbr" />
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="200" />
    </px:PXGrid>
</asp:Content>

