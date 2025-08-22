<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="IN409010.aspx.cs"
    Inherits="Page_IN409010" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Width="100%" TypeName="PX.Objects.IN.StoragePlaceEnq" PageLoadBehavior="PopulateSavedValues"
        PrimaryView="Filter" Visible="True" TabIndex="1">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="Cancel" PopupVisible="true" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" Caption="Selection" DataMember="Filter"
        CaptionAlign="Justify" DefaultControlID="edSiteID">
        <Template>
            <px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
            <px:PXSegmentMask ID="edSiteID" runat="server" DataField="SiteID" AutoRefresh="true" CommitChanges="true" AllowEdit="true" >
                <AutoCallBack Command="Save" Enabled="True" Target="form" />
                <GridProperties FastFilterFields="Descr">
                    <Layout ColumnsMenu="False" />
                    <PagerSettings Mode="NextPrevFirstLast" />
                </GridProperties>
                <Items>
                    <px:PXMaskItem EditMask="AlphaNumeric" Length="10" Separator="-" TextCase="Upper" />
                </Items>
            </px:PXSegmentMask>
            <px:PXCheckbox ID="edExpand" runat="server" DataField="ExpandByLotSerialNbr" CommitChanges="True">
                <AutoCallBack Command="Save" Enabled="True" Target="form" />
            </px:PXCheckbox>
            <px:PXLayoutRule ID="PXLayoutRule2" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
            <px:PXSelector ID="edStorageID" runat="server" DataField="StorageID" AutoRefresh="true" CommitChanges="true" >
                <GridProperties>
                    <Layout ColumnsMenu="False" />
                    <PagerSettings Mode="NextPrevFirstLast" />
                    <PagerSettings Mode="NextPrevFirstLast" />
                </GridProperties>
                <AutoCallBack Command="Save" Enabled="True" Target="form">
                </AutoCallBack>
            </px:PXSelector>
            <px:PXGroupBox ID="gbStorageType" runat="server" DataField="StorageType" CommitChanges="True" RenderStyle="Simple">
                <Template>
                    <px:PXLayoutRule runat="server" StartGroup="False" StartColumn="True" />
                    <px:PXRadioButton ID="gbStorageType_op0" runat="server" Value="L" GroupName="gbStorageType" />
                    <px:PXRadioButton ID="gbStorageType_op1" runat="server" Value="C" GroupName="gbStorageType" />
                    <px:PXLayoutRule runat="server" StartGroup="False" StartColumn="True" SuppressLabel="True" ControlSize="M"/>
                    <px:PXSelector ID="edLocationID" runat="server" DataField="LocationID" AutoRefresh="true" CommitChanges="true" >
                        <GridProperties>
                            <Layout ColumnsMenu="False" />
                            <PagerSettings Mode="NextPrevFirstLast" />
                            <PagerSettings Mode="NextPrevFirstLast" />
                        </GridProperties>
                        <AutoCallBack Command="Save" Enabled="True" Target="form">
                        </AutoCallBack>
                    </px:PXSelector>
                    <px:PXSelector ID="edCartID" runat="server" DataField="CartID" AutoRefresh="true" CommitChanges="true" >
                        <GridProperties>
                            <Layout ColumnsMenu="False" />
                            <PagerSettings Mode="NextPrevFirstLast" />
                            <PagerSettings Mode="NextPrevFirstLast" />
                        </GridProperties>
                        <AutoCallBack Command="Save" Enabled="True" Target="form">
                        </AutoCallBack>
                    </px:PXSelector>
                </Template>
                <ContentLayout Layout="Stack" />
            </px:PXGroupBox>
            <px:PXLayoutRule ID="PXLayoutRule3" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
            <px:PXSegmentMask ID="edInventoryID" runat="server" DataField="InventoryID" AutoRefresh="true" CommitChanges="true" AllowEdit="true">
                <GridProperties>
                    <Layout ColumnsMenu="False" />
                    <PagerSettings Mode="NextPrevFirstLast" />
                    <PagerSettings Mode="NextPrevFirstLast" />
                </GridProperties>
                <AutoCallBack Command="Save" Enabled="True" Target="form">
                </AutoCallBack>
            </px:PXSegmentMask>
            <px:PXSegmentMask ID="edSubItemCD" runat="server" DataField="SubItemID">
                <AutoCallBack Command="Save" Enabled="True" Target="form">
                </AutoCallBack>
                <GridProperties FastFilterFields="Descr">
                    <Layout ColumnsMenu="False" />
                    <PagerSettings Mode="NextPrevFirstLast" />
                </GridProperties>
                <Items>
                    <px:PXMaskItem EditMask="AlphaNumeric" Length="2" Separator="-" TextCase="Upper" />
                    <px:PXMaskItem EditMask="AlphaNumeric" Length="1" Separator="-" TextCase="Upper" />
                    <px:PXMaskItem EditMask="AlphaNumeric" Length="1" Separator="-" TextCase="Upper" />
                </Items>
            </px:PXSegmentMask>
            <px:PXTextEdit ID="edLotSerialNbr" runat="server" DataField="LotSerialNbr" CommitChanges="True">
                <AutoCallBack Command="Save" Enabled="True" Target="form" />
            </px:PXTextEdit>
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Height="144px" Style="z-index: 100; left: 0px; top: 0px;" Width="100%"
        AdjustPageSize="Auto" AllowPaging="True" AllowSearch="True" Caption="Storage Summary" BatchUpdate="True" SkinID="PrimaryInquire"
        TabIndex="8" RestrictFields="True" SyncPosition="true" FastFilterFields="InventoryID,SiteID" >
        <Mode AllowAddNew="False" AllowColMoving="False" AllowDelete="False" AllowSort="False" />
        <Levels>
            <px:PXGridLevel DataMember="storages">
                <Columns>
                    <px:PXGridColumn DataField="SiteID" />
                    <px:PXGridColumn DataField="StorageCD" />
                    <px:PXGridColumn DataField="InventoryID" />
                    <px:PXGridColumn DataField="InventoryDescr" />
                    <px:PXGridColumn DataField="SubItemID" />
                    <px:PXGridColumn DataField="LotSerialNbr" />
                    <px:PXGridColumn DataField="ExpireDate" DataType="DateTime" />
                    <px:PXGridColumn DataField="Qty" DataType="Decimal" Decimals="4" DefValueText="0.0" TextAlign="Right"  />
                    <px:PXGridColumn DataField="QtyPickedToCart" DataType="Decimal" Decimals="4" DefValueText="0.0" TextAlign="Right"  />
                    <px:PXGridColumn DataField="BaseUnit" />
                </Columns>
                <RowTemplate>
                    <px:PXCheckbox ID="edIsCart" runat="server" DataField="IsCart" CommitChanges="True"/>
                </RowTemplate>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
        <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
    </px:PXGrid>
</asp:Content>