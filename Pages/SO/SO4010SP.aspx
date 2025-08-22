<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SO4010SP.aspx.cs" Inherits="Pages_SO_SO4010SP" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.SO.SOOrderRelatedReturnsSP" PrimaryView="Filter" PageLoadBehavior="PopulateSavedValues"/>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Filter" Caption="Selection" DefaultControlID="edAction">
        <Activity HighlightColor="" SelectedColor="" Width="" Height=""></Activity>
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="S" />
            <px:PXSelector ID="edOrderType" aurelia="true" runat="server" DataField="OrderType" DataSourceID="ds" CommitChanges="true" >
                <GridProperties>
                    <Layout ColumnsMenu="False" />
                </GridProperties>
            </px:PXSelector>
            <px:PXSelector ID="edOrderNbr" runat="server" DataField="OrderNbr" AutoRefresh="True" DataSourceID="ds" CommitChanges="true">
                <GridProperties FastFilterFields="CustomerOrderNbr,CustomerID,CustomerID_Customer_acctName,CustomerRefNbr">
                    <Layout ColumnsMenu="False" />
                </GridProperties>
            </px:PXSelector>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
            <px:PXNumberEdit ID="edShippedQty" runat="server" DataField="ShippedQty" />
            <px:PXNumberEdit ID="edReturnedQty" runat="server" DataField="ReturnedQty" />
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXFormView ID="edForm" runat="server" DataSourceID="ds">
        <Template>

            <px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="S" SuppressLabel="True" />
            <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Width="1150px" Height="150px" AllowPaging="true" AdjustPageSize="Auto" SkinID="Inquire"
                AllowSearch="true" NoteIndicator="false" FilesIndicator="false" Caption="Related Return Documents" CaptionVisible="true">
                <Levels>
                    <px:PXGridLevel DataMember="RelatedReturnDocuments">
                        <RowTemplate>
                            <px:PXSelector ID="edReturnOrderNbr" runat="server" DataField="ReturnOrderNbr" AllowEdit="true" />
                            <px:PXSelector ID="edReturnInvoiceNbr" runat="server" DataField="ReturnInvoiceNbr" AllowEdit="true" />
                            <px:PXSelector ID="edShipmentNbr" runat="server" DataField="ShipmentNbr" AllowEdit="true" />
                            <px:PXSelector ID="edARRefNbr" runat="server" DataField="ARRefNbr" AllowEdit="true" />
                            <px:PXSelector ID="edAPRefNbr" runat="server" DataField="APRefNbr" AllowEdit="true" />
                        </RowTemplate>
                        <Columns>
                            <px:PXGridColumn DataField="ReturnOrderType" Width="110px" />
                            <px:PXGridColumn DataField="ReturnOrderNbr" />
                            <px:PXGridColumn DataField="ReturnInvoiceType" />
                            <px:PXGridColumn DataField="ReturnInvoiceNbr" />
                            <px:PXGridColumn DataField="ShipmentType" />
                            <px:PXGridColumn DataField="ShipmentNbr" />
                            <px:PXGridColumn DataField="ARDocType" />
                            <px:PXGridColumn DataField="ARRefNbr" />
                            <px:PXGridColumn DataField="APDocType" />
                            <px:PXGridColumn DataField="APRefNbr" />
                        </Columns>
                    </px:PXGridLevel>
                </Levels>
            </px:PXGrid>

            <px:PXGrid ID="grid2" runat="server" DataSourceID="ds" Width="800px" Height="295px" AllowPaging="true" AdjustPageSize="Auto" SkinID="Inquire"
                AllowSearch="true" NoteIndicator="false" FilesIndicator="false" Caption="Return Documents by Line" CaptionVisible="true">
                <Levels>
                    <px:PXGridLevel DataMember="ReturnsByLine">
                        <RowTemplate>
                            <px:PXSegmentMask ID="edInventoryID" runat="server" DataField="InventoryID" AllowEdit="true" />
                            <px:PXSelector ID="edDisplayReturnOrderNbr" runat="server" DataField="DisplayReturnOrderNbr" AllowEdit="true" />
                            <px:PXSelector ID="edDisplayReturnInvoiceNbr" runat="server" DataField="DisplayReturnInvoiceNbr" AllowEdit="true" />
                        </RowTemplate>
                        <Columns>
                            <px:PXGridColumn DataField="LineNbr" />
                            <px:PXGridColumn DataField="InventoryID" />
                            <px:PXGridColumn DataField="BaseUnit" Width="90px" />
                            <px:PXGridColumn DataField="ReturnedQty" Width="110px" />
                            <px:PXGridColumn DataField="DisplayReturnOrderType" Width="110px" />
                            <px:PXGridColumn DataField="DisplayReturnOrderNbr" />
                            <px:PXGridColumn DataField="DisplayReturnInvoiceType" />
                            <px:PXGridColumn DataField="DisplayReturnInvoiceNbr" />
                        </Columns>
                    </px:PXGridLevel>
                </Levels>
            </px:PXGrid>

        </Template>
    </px:PXFormView>
</asp:Content>
