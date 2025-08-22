<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SO301000.aspx.cs"
    Inherits="Page_SO301000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource EnableAttributes="true" ID="ds" UDFTypeField="OrderType" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.SO.SOOrderEntry" 
			PrimaryView="Document">
        <DataTrees>
            <px:PXTreeDataMember TreeView="_EPCompanyTree_Tree_" TreeKeys="WorkgroupID" />
        </DataTrees>
        <CallbackCommands>
			<px:PXDSCallbackCommand Name="OverrideBlanketTaxZone" CommitChanges="True" Visible="false" />
            <px:PXDSCallbackCommand Name="SOOrderLineSplittingExtension_ShowSplits" Visible="false" CommitChanges="True" RepaintControls="None" RepaintControlsIDs="ds,optform,grid2" SelectControlsIDs="form" />
		</CallbackCommands>
    </px:PXDataSource>
    <%-- Add Invoice Details --%>
    <px:PXSmartPanel ID="PanelAddInvoice" runat="server" Width="1200px" Key="invoicesplits" Caption="Add Invoice Details" CaptionVisible="True"
        LoadOnDemand="True" CallBackMode-CommitChanges="True" CallBackMode-PostData="Page" AutoCallBack-Command="Refresh" AutoCallBack-Target="form4" Height="600px">
        <px:PXFormView ID="form4" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="addinvoicefilter"
            CaptionVisible="False">
            <Template>
                <px:PXLayoutRule runat="server" ControlSize="SM" LabelsWidth="S" StartColumn="True" />
                <px:PXDropDown ID="edARDocType" runat="server" DataField="ARDocType" CommitChanges="true">
                </px:PXDropDown>
                <px:PXSelector ID="edARRefNbr" runat="server" AutoRefresh="True" DataField="ARRefNbr" DataSourceID="ds" CommitChanges="true">
                    <GridProperties>
                        <Layout ColumnsMenu="False" />
                    </GridProperties>
                </px:PXSelector>
                <px:PXSelector ID="edOrderType" runat="server" DataField="OrderType" CommitChanges="true" />
                <px:PXSelector ID="edOrderNbr" runat="server" AutoRefresh="True" DataField="OrderNbr" CommitChanges="true" />
                <px:PXLayoutRule runat="server" ControlSize="SM" LabelsWidth="S" StartColumn="True" />
                <px:PXSelector ID="edInventoryID" runat="server" DataField="InventoryID" CommitChanges="true" AllowEdit="true" />
                <px:PXSelector ID="edLotSerialNbr" runat="server" AutoRefresh="True" DataField="LotSerialNbr" CommitChanges="true" />
                <px:PXCheckBox ID="chkExpand" runat="server" DataField="Expand" CommitChanges="true" />
                <px:PXLayoutRule runat="server" ControlSize="SM" LabelsWidth="S" StartColumn="True" />
                <px:PXDateTimeEdit ID="edStartDate" runat="server" DataField="StartDate" CommitChanges="true" />
                <px:PXDateTimeEdit ID="edEndDate" runat="server" DataField="EndDate" CommitChanges="true" />
            </Template>
            <ContentStyle BackColor="Transparent" BorderStyle="None" />
        </px:PXFormView>
        <px:PXGrid ID="grid4" runat="server" Width="100%" DataSourceID="ds" Style="height: 250px;"
            FeedbackMode="ForceInquiry" SkinID="Inquire" FilesIndicator="false" NoteIndicator="false" AdjustPageSize="Auto" SyncPosition="true">
            <Parameters>
                <px:PXControlParam ControlID="form4" Name="AddInvoiceFilter.refNbr" PropertyName="DataControls[&quot;edARRefNbr&quot;].Value"
                    Type="String" />
            </Parameters>
            <Levels>
                <px:PXGridLevel DataMember="invoicesplits">
                    <RowTemplate>
                        <px:PXNumberEdit ID="edQtyToReturn" runat="server" DataField="QtyToReturn" MinValue="0" />
                        <px:PXSelector ID="edSOOrderNbr" runat="server" DataField="SOOrderNbr" AllowEdit="true" />
                        <px:PXSelector ID="edARRefNbr" runat="server" DataField="ARRefNbr" AllowEdit="true" />
                    </RowTemplate>
                    <Columns>
                        <px:PXGridColumn AllowCheckAll="True" AllowNull="False" DataField="Selected" TextAlign="Center" Type="CheckBox" AllowFilter="false" AllowSort="false" CommitChanges="true" Width="30px" />
                        <px:PXGridColumn DataField="InventoryID" Width="130px" />
                        <px:PXGridColumn DataField="TranDesc"  Width="170px" />
                        <px:PXGridColumn DataField="ComponentID" Width="120px" />
                        <px:PXGridColumn DataField="ComponentDesc" Width="150px" />
                        <px:PXGridColumn DataField="SubItemID" />
                        <px:PXGridColumn DataField="LotSerialNbr" AllowShowHide="Server" Width="140px" />
                        <px:PXGridColumn DataField="UOM" Width="60px" />
                        <px:PXGridColumn DataField="QtyAvailForReturn" DataType="Decimal" TextAlign="Right" DefValueText="0.0" AllowFilter="false" AllowSort="false" Width="83px" />
                        <px:PXGridColumn DataField="QtyToReturn" DataType="Decimal" TextAlign="Right" DefValueText="0.0" AllowFilter="false" AllowSort="false" CommitChanges="true" Width="63px" />
                        <px:PXGridColumn DataField="Qty" DataType="Decimal" TextAlign="Right" DefValueText="0.0" Width="69px" />
                        <px:PXGridColumn DataField="QtyReturned" DataType="Decimal" TextAlign="Right" DefValueText="0.0" AllowFilter="false" AllowSort="false" Width="60px" />
                        <px:PXGridColumn DataField="SOOrderDate" />
                        <px:PXGridColumn DataField="SOOrderType" />
                        <px:PXGridColumn DataField="SOOrderNbr" Width="100px" />
                        <px:PXGridColumn DataField="ARTranDate" Width="80px" />
                        <px:PXGridColumn DataField="ARDocType" Width="80px" />
                        <px:PXGridColumn DataField="ARRefNbr" Width="100px" />
                        <px:PXGridColumn DataField="SiteID" />
                        <px:PXGridColumn DataField="LocationID" />
                        <px:PXGridColumn DataField="DropShip" Type="CheckBox" Width="55px" />
                    </Columns>
                    <Layout ColumnsMenu="True" FormViewHeight="" />
                </px:PXGridLevel>
            </Levels>
            <AutoSize Enabled="True" />
            <Mode AllowAddNew="False" AllowDelete="False" />
        </px:PXGrid>
        <px:PXPanel ID="PXPanel1" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton1" runat="server" CommandName="AddInvoiceOK" CommandSourceID="ds" Text="Add" SyncVisible="false" />
            <px:PXButton ID="PXButton2" runat="server" DialogResult="OK" Text="Add &amp; Close" />
            <px:PXButton ID="PXButton3" runat="server" DialogResult="Cancel" Text="Cancel" />
        </px:PXPanel>
    </px:PXSmartPanel>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Document" Caption="Order Summary"
        NoteIndicator="True" FilesIndicator="True" LinkIndicator="True" EmailingGraph="PX.Objects.CR.CREmailActivityMaint,PX.Objects"
        ActivityIndicator="True" ActivityField="NoteActivity" DefaultControlID="edOrderType" BPEventsIndicator="True"
        TabIndex="14900">
        <CallbackCommands>
            <Save PostData="Self" ></Save>
        </CallbackCommands>
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="S" ></px:PXLayoutRule>
            <px:PXSelector ID="edOrderType" aurelia="true" runat="server" DataField="OrderType" AutoRefresh="True" DataSourceID="ds">
                <GridProperties>
                    <Layout ColumnsMenu="False" ></Layout>
                </GridProperties>
            </px:PXSelector>
            <px:PXSelector ID="edOrderNbr" runat="server" DataField="OrderNbr" AutoRefresh="True" DataSourceID="ds">
                <GridProperties FastFilterFields="CustomerOrderNbr,CustomerID,CustomerID_Customer_acctName,CustomerRefNbr">
                    <Layout ColumnsMenu="False" ></Layout>
                </GridProperties>
            </px:PXSelector>
            <px:PXDropDown ID="edStatus" runat="server" DataField="Status" Enabled="False" ></px:PXDropDown>
            <px:PXDateTimeEdit ID="edOrderDate" runat="server" DataField="OrderDate">
                <AutoCallBack Command="Save" Target="form" ></AutoCallBack>
            </px:PXDateTimeEdit>
            <px:PXDateTimeEdit CommitChanges="True" ID="edRequestDate" runat="server" DataField="RequestDate" ></px:PXDateTimeEdit>
            <px:PXDateTimeEdit CommitChanges="True" ID="edExpireDate" runat="server" DataField="ExpireDate" ></px:PXDateTimeEdit>
            <px:PXLayoutRule runat="server" ></px:PXLayoutRule>
            <px:PXTextEdit ID="edCustomerOrderNbr" runat="server" DataField="CustomerOrderNbr" CommitChanges="True" ></px:PXTextEdit>
            <px:PXTextEdit ID="edCustomerRefNbr" runat="server" DataField="CustomerRefNbr" MaxLength="40" ></px:PXTextEdit>
            
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" ></px:PXLayoutRule>
            <px:PXSegmentMask CommitChanges="True" ID="edCustomerID" runat="server" DataField="CustomerID" AllowAddNew="True"
                AllowEdit="True" DataSourceID="ds" AutoRefresh="True"></px:PXSegmentMask>
            <px:PXSegmentMask CommitChanges="True" ID="edCustomerLocationID" runat="server" AutoRefresh="True"
                DataField="CustomerLocationID" DataSourceID="ds" AllowEdit="true" ></px:PXSegmentMask>
            <px:PXSelector runat="server" ID="edContactID" DataField="ContactID" AllowEdit="True" DataSourceID="ds" AllowAddNew="True" AutoRefresh="True" CommitChanges="True" ></px:PXSelector>
            <pxa:PXCurrencyRate DataField="CuryID" ID="edCury" runat="server" DataSourceID="ds" RateTypeView="_SOOrder_CurrencyInfo_"
                DataMember="_Currency_"></pxa:PXCurrencyRate>
            <px:PXLayoutRule runat="server" Merge="True" ></px:PXLayoutRule>
            <px:PXSegmentMask CommitChanges="True" ID="edDestinationSiteID" runat="server" DataField="DestinationSiteID" AutoRefresh="True" DataSourceID="ds" ></px:PXSegmentMask>
            <px:PXLayoutRule runat="server" ></px:PXLayoutRule>
            <px:PXSegmentMask CommitChanges="True" ID="edProjectID" runat="server" DataField="ProjectID" AutoRefresh="True"
                DataSourceID="ds" AllowAddNew="True" AllowEdit="True"></px:PXSegmentMask>
            <px:PXTextEdit ID="edOrderDesc" runat="server" DataField="OrderDesc" TextMode="MultiLine" Height="55" ></px:PXTextEdit>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" StartGroup="True" ></px:PXLayoutRule>
            <px:PXPanel ID="XX" runat="server" RenderStyle="Simple">
                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" ></px:PXLayoutRule>
                <px:PXNumberEdit ID="edOrderQty" runat="server" DataField="OrderQty" Enabled="False" ></px:PXNumberEdit>
                <px:PXNumberEdit ID="edCuryDetailExtPriceTotal" runat="server" DataField="CuryDetailExtPriceTotal" Enabled="False" ></px:PXNumberEdit>
                <px:PXNumberEdit ID="edCuryLineDiscTotal" runat="server" DataField="CuryLineDiscTotal" Enabled="False" ></px:PXNumberEdit>
                <px:PXNumberEdit ID="edCuryDiscTot" runat="server" DataField="CuryDiscTot" CommitChanges="true" ></px:PXNumberEdit>
                <px:PXNumberEdit ID="edCuryFreightTot" runat="server" DataField="CuryFreightTot" Enabled="False" ></px:PXNumberEdit>
                <px:PXNumberEdit ID="edCuryTaxTotal" runat="server" DataField="CuryTaxTotal" Enabled="False" ></px:PXNumberEdit>
                <px:PXNumberEdit ID="edCuryOrderTotal" runat="server" DataField="CuryOrderTotal" Enabled="False" ></px:PXNumberEdit>
                <px:PXNumberEdit ID="edCuryControlTotal" runat="server" CommitChanges="True" DataField="CuryControlTotal" ></px:PXNumberEdit>
            </px:PXPanel>
            
            <px:PXCheckBox runat="server" DataField="ArePaymentsApplicable" CommitChanges="True" ID="chkPaymentsApplicable" AlignLeft="true" ></px:PXCheckBox>
            <px:PXCheckBox runat="server" DataField="IsFSIntegrated"     SuppressLabel="True" ID="chkIsFSIntegrated" Enabled="False" Visible="False" ></px:PXCheckBox>
            <px:PXCheckBox runat="server" DataField="ShowDiscountsTab" ID="chkShowDiscountsTab" ></px:PXCheckBox>
            <px:PXCheckBox runat="server" DataField="ShowShipmentsTab" ID="chkShowShipmentsTab" ></px:PXCheckBox>
            <px:PXCheckBox runat="server" DataField="ShowOrdersTab" ID="chkShowOrdersTab" ></px:PXCheckBox>
	<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartColumn="True" ControlSize="M" LabelsWidth="SM" />
	<px:PXTextEdit runat="server" ID="edUsrHubspotDealID" DataField="UsrHubspotDealID" TextMode="MultiLine" Height="60px" /></Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <style type="text/css">
        table[id$=_tabRelatedItems] {
            border-bottom:solid 1px #D2D4D7;
        }

        .RelatedItemsCell, td.GridRow[tooltip*=RelatedItemsRequired], td.GridRow[tooltip*=RelatedItemsAvailable]
        {
          padding-top: 1px !important;
          padding-bottom: 1px !important;
        }
        .RelatedItemsCell > div[error], td.GridRow[tooltip*=RelatedItemsAvailable] > div[error], td.GridRow[tooltip*=RelatedItemsRequired] > div[error]
        {
          display: none;
        }

        #tooltip.RelatedItemsAvailable, #tooltip.RelatedItemsAvailable .arrow:after
        {
          color: black;
          background-color: white;
          border-color: #9DD58E;
          font-weight: normal;
        }        

        #tooltip.RelatedItemsRequired, #tooltip.RelatedItemsRequired .arrow:after
        {
          color: white;
          background-color: red;
          border-color: red;
        }
    </style> 
    <px:PXTab ID="tab" runat="server" Height="540px" Style="z-index: 100;" Width="100%">
        <Items>
            <px:PXTabItem Text="Details">
                <Template>
                    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Width="100%" 
                        TabIndex="100" SkinID="DetailsInTab" StatusField="Availability" SyncPosition="True" Height="473px"
                        OnRowDataBound="grid_RowDataBound" >
                        <Levels>
                            <px:PXGridLevel DataMember="Transactions">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="SM" />
                                    <px:PXSegmentMask CommitChanges="True" ID="edInventoryID" runat="server" DataField="InventoryID" AllowEdit="True" />
                                   
									<px:PXCheckBox runat="server" ID="edSDSelected" DataField="SDSelected" Text="SDSelected" />
                                    <px:PXCheckBox runat="server" ID="edExcludedFromExport" DataField="ExcludedFromExport" AllowEdit="False"/>
									<px:PXDropDown runat="server" ID="edEquipmentAction" DataField="EquipmentAction" CommitChanges="True" />
									<px:PXTextEdit runat="server" ID="edSMComment" DataField="Comment" />
									<px:PXSelector runat="server" ID="edSMEquipmentID" DataField="SMEquipmentID" CommitChanges="True" AutoRefresh="True" />
									<px:PXSelector runat="server" ID="edNewEquipmentLineNbr" DataField="NewEquipmentLineNbr" CommitChanges="True" AutoRefresh="True" />
									<px:PXSelector runat="server" ID="edSMComponentID" DataField="ComponentID" CommitChanges="True" AutoRefresh="True" />
									<px:PXSelector runat="server" ID="edEquipmentComponentLineNbr" DataField="EquipmentComponentLineNbr" CommitChanges="True" AutoRefresh="True" />
                                    <px:PXSegmentMask CommitChanges="True" ID="edSubItemID" runat="server" DataField="SubItemID" AutoRefresh="True">
                                        <Parameters>
                                            <px:PXControlParam ControlID="grid" Name="SOLine.inventoryID" PropertyName="DataValues[&quot;InventoryID&quot;]" Type="String" />
                                        </Parameters>
                                    </px:PXSegmentMask>
                                    <px:PXCheckBox ID="chkIsFree" runat="server" DataField="IsFree" />
                                    <px:PXSegmentMask CommitChanges="True" ID="edSiteID" runat="server" DataField="SiteID" AutoRefresh="True">
                                        <Parameters>
                                            <px:PXControlParam ControlID="grid" Name="SOLine.inventoryID" PropertyName="DataValues[&quot;InventoryID&quot;]" Type="String" />
                                            <px:PXControlParam ControlID="grid" Name="SOLine.subItemID" PropertyName="DataValues[&quot;SubItemID&quot;]" Type="String" />
                                        </Parameters>
                                    </px:PXSegmentMask>
                                    <px:PXSegmentMask ID="edLocationID" runat="server" DataField="LocationID" AutoRefresh="True">
                                        <Parameters>
                                            <px:PXControlParam ControlID="grid" Name="SOLine.siteID" PropertyName="DataValues[&quot;SiteID&quot;]" Type="String" />
                                            <px:PXControlParam ControlID="grid" Name="SOLine.inventoryID" PropertyName="DataValues[&quot;InventoryID&quot;]" Type="String" />
                                            <px:PXControlParam ControlID="grid" Name="SOLine.subItemID" PropertyName="DataValues[&quot;SubItemID&quot;]" Type="String" />
                                        </Parameters>
                                    </px:PXSegmentMask>
                                    <px:PXSelector CommitChanges="True" ID="edUOM" runat="server" DataField="UOM" AutoRefresh="True">
                                        <Parameters>
                                            <px:PXControlParam ControlID="grid" Name="SOLine.inventoryID" PropertyName="DataValues[&quot;InventoryID&quot;]" Type="String" />
                                        </Parameters>
                                    </px:PXSelector>
                                    <px:PXNumberEdit ID="edCuryUnitCost" runat="server" DataField="CuryUnitCost" CommitChanges="True" />
                                    <px:PXNumberEdit ID="edOrderQty" runat="server" DataField="OrderQty" CommitChanges="True" />
                                    <px:PXNumberEdit ID="edQtyOnOrders" runat="server" DataField="QtyOnOrders" />
                                    <px:PXNumberEdit ID="edBlanketOpenQty" runat="server" DataField="BlanketOpenQty" />
                                    <px:PXNumberEdit ID="edUnshippedQty" runat="server" DataField="UnshippedQty" />
                                    <px:PXNumberEdit ID="edShippedQty" runat="server" DataField="ShippedQty" Enabled="False" />
                                    <px:PXNumberEdit ID="edOpenQty" runat="server" DataField="OpenQty" Enabled="False" />
                                    <px:PXNumberEdit ID="edUnitPrice" runat="server" DataField="CuryUnitPrice"  CommitChanges="True"/>
                                    <px:PXCheckBox ID="chkManualPrice" runat="server" DataField="ManualPrice" CommitChanges="True" />
                                    <px:PXSelector ID="edManualDiscountID" runat="server" DataField="DiscountID" AllowEdit="True" edit="1" />
                                    <px:PXNumberEdit ID="edDiscPct" runat="server" DataField="DiscPct" />
                                    <px:PXNumberEdit ID="edCuryDiscAmt" runat="server" DataField="CuryDiscAmt" />
                                    <px:PXCheckBox ID="chkManualDisc" runat="server" DataField="ManualDisc" CommitChanges="True" />
                                    <px:PXCheckBox ID="chkAutomaticDiscountsDisabled" runat="server" DataField="AutomaticDiscountsDisabled" />
                                    <px:PXLayoutRule runat="server" ColumnSpan="3" />
                                    <px:PXTextEdit ID="edTranDesc" runat="server" DataField="TranDesc" />
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="SM" />
                                    <px:PXCheckBox ID="chkCompleted" runat="server" DataField="Completed" CommitChanges="true" />
                                    <px:PXNumberEdit ID="edCuryExtPrice" runat="server" DataField="CuryExtPrice" CommitChanges="True" />
                                    <px:PXNumberEdit ID="edCuryLineAmt" runat="server" DataField="CuryLineAmt" />
                                    <px:PXNumberEdit ID="edLineMarginPct" runat="server" DataField="MarginPct" />
                                    <px:PXNumberEdit ID="edLineCuryMarginAmt" runat="server" DataField="CuryMarginAmt" />
                                    <px:PXDateTimeEdit ID="edSchedOrderDate" runat="server" DataField="SchedOrderDate" />
                                    <px:PXTextEdit ID="edCustomerOrderNbr" runat="server" DataField="CustomerOrderNbr" />
                                    <px:PXSegmentMask CommitChanges="True" ID="edCustomerLocationIDLine" runat="server" DataField="CustomerLocationID" 
                                        AutoRefresh="True" />
                                    <px:PXDateTimeEdit ID="edSchedShipDate" runat="server" DataField="SchedShipDate" />
                                    <px:PXNumberEdit ID="edCuryUnbilledAmt" runat="server" DataField="CuryUnbilledAmt" Enabled="False" />
                                    <px:PXDateTimeEdit ID="edRequestDate1" runat="server" DataField="RequestDate" />
                                    <px:PXDateTimeEdit ID="edShipDate" runat="server" DataField="ShipDate" />
                                    <px:PXDropDown ID="edShipComplete" runat="server" AllowNull="False" DataField="ShipComplete" SelectedIndex="2" />
                                    <px:PXNumberEdit ID="edCompleteQtyMin" runat="server" DataField="CompleteQtyMin" />
                                    <px:PXNumberEdit ID="edCompleteQtyMax" runat="server" DataField="CompleteQtyMax" />
                                    <px:PXDateTimeEdit ID="edExpireDate" runat="server" DataField="ExpireDate" DisplayFormat="d" />
                                    <px:PXDateTimeEdit ID="edDRTermStartDate" runat="server" DataField="DRTermStartDate" CommitChanges="true" />
                                    <px:PXDateTimeEdit ID="edDRTermEndDate" runat="server" DataField="DRTermEndDate" CommitChanges="true" />
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="SM" />
                                    <px:PXSegmentMask CommitChanges="True" ID="edBranchID" runat="server" DataField="BranchID" AutoRefresh="true" />
                                    <px:PXSegmentMask CommitChanges="True" ID="edSalesAcctID" runat="server" DataField="SalesAcctID" AutoRefresh="True" />
                                    <px:PXSegmentMask ID="edSalesSubID" runat="server" DataField="SalesSubID" AutoRefresh="True" />
                                    <px:PXCheckBox CommitChanges="True" ID="chkPOCreate" runat="server" DataField="POCreate" />
                                    <px:PXDropDown ID="edPOSource" runat="server" DataField="POSource" />
                                    <px:PXDateTimeEdit ID="edPOCreateDate" runat="server" DataField="POCreateDate" DisplayFormat="d" />
                                    <px:PXSelector runat="server" DataField="POOrderNbr" AllowEdit="true" ID="edDSPOOrderNbr" />
                                    <px:PXDropDown runat="server" DataField="POOrderStatus" ID="edPOOrderStatus" />
                                    <px:PXNumberEdit runat="server" DataField="POLineNbr" ID="edDSPOLineNbr" />
                                    <px:PXCheckBox runat="server" DataField="POLinkActive" CommitChanges="True" ID="edPOLinkActive" />
                                    <px:PXSelector ID="edReasonCode" runat="server" DataField="ReasonCode" AutoRefresh="true">
                                        <Parameters>
                                            <px:PXSyncGridParam ControlID="grid" />
                                        </Parameters>
                                    </px:PXSelector>
                                    <px:PXSegmentMask ID="edSalesPersonID" runat="server" DataField="SalesPersonID" />
                                    <px:PXSelector ID="edTaxZoneID" runat="server" DataField="TaxZoneID" AutoRefresh="True" CommitChanges="true" />
                                    <px:PXSelector ID="edTaxCategoryID" runat="server" DataField="TaxCategoryID" AutoRefresh="True" CommitChanges="true" />
									<px:PXDropDown ID="edAvalaraCustomerUsageTypeID1" runat="server" DataField="AvalaraCustomerUsageType" CommitChanges="true" />
                                    <px:PXTextEdit ID="edAlternateID" runat="server" DataField="AlternateID" />
                                    <px:PXLayoutRule runat="server" Merge="True" />
                                    <px:PXSelector Size="xm" ID="edLotSerialNbr" runat="server" AllowNull="False" DataField="LotSerialNbr" AutoRefresh="True">
                                        <Parameters>
                                            <px:PXControlParam ControlID="grid" Name="SOLine.inventoryID" PropertyName="DataValues[&quot;InventoryID&quot;]" Type="String" />
                                            <px:PXControlParam ControlID="grid" Name="SOLine.subItemID" PropertyName="DataValues[&quot;SubItemID&quot;]" Type="String" />
                                            <px:PXControlParam ControlID="grid" Name="SOLine.locationID" PropertyName="DataValues[&quot;LocationID&quot;]" Type="String" />
                                        </Parameters>
                                    </px:PXSelector>
                                    <px:PXCheckBox CommitChanges="True" ID="chkCommissionable" runat="server" Checked="True" DataField="Commissionable" />
                                    <px:PXLayoutRule runat="server" />
                                    <px:PXSelector ID="edBlanketNbr" runat="server" DataField="BlanketNbr" />
                                    <px:PXSegmentMask ID="edTaskID" runat="server" DataField="TaskID" AutoRefresh="True" />
                                    <px:PXSegmentMask ID="edCostCode" runat="server" DataField="CostCodeID" AutoRefresh="True" />
                                   <px:PXCheckBox runat="server" CommitChanges="True" DataField="AMProdCreate" ID="chkAMProdCreate" />
									<px:PXSelector runat="server" ID="edAMOrderType" DataField="AMOrderType" AutoRefresh="True" AllowEdit="True" />
									<px:PXSelector runat="server" ID="edAMProdOrdID" DataField="AMProdOrdID" AutoRefresh="True" AllowEdit="True" />
									<px:PXSelector runat="server" ID="edAMEstimateID" DataField="AMEstimateID" AutoRefresh="True" />
									<px:PXSelector runat="server" ID="edAMEstimateRevisionID" DataField="AMEstimateRevisionID" AutoRefresh="True" AllowEdit="True" />
									<px:PXSelector runat="server" ID="edAMConfigKeyID" DataField="AMConfigKeyID" AutoRefresh="True" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="Availability" SyncVisible="False" Visible="false" />
                                    <px:PXGridColumn DataField="ExcludedFromExport" AllowShowHide="Server" Type="CheckBox"/>                                    
									<px:PXGridColumn DataField="IsConfigurable" Type="CheckBox" TextAlign="Center" Width="85px" />
                                    <px:PXGridColumn DataField="BranchID" RenderEditorText="True" CommitChanges="True" />
                                    <px:PXGridColumn DataField="OrderType" />
                                    <px:PXGridColumn DataField="OrderNbr" />
                                    <px:PXGridColumn DataField="LineNbr" TextAlign="Right" />
                                    <px:PXGridColumn DataField="AssociatedOrderLineNbr" />
                                    <px:PXGridColumn DataField="GiftMessage" />
                                    <px:PXGridColumn DataField="SortOrder" TextAlign="Right" />
                                    <px:PXGridColumn DataField="LineType" RenderEditorText="True" />
                                    <px:PXGridColumn DataField="InvoiceNbr" AllowShowHide="Server" />
                                    <px:PXGridColumn DataField="Operation" AllowShowHide="Server" Label="Operation" RenderEditorText="True" CommitChanges="True" />
                                    <px:PXGridColumn DataField="InventoryID" CommitChanges="True" AllowDragDrop="true" />
                                    <px:PXGridColumn DataField="RelatedItems" Type="Icon" LinkCommand="AddRelatedItems" AllowShowHide="Server" AllowFilter="false" AllowSort="false" ForceExport="true" DisplayMode="Value"/>
                                    <px:PXGridColumn DataField="SubstitutionRequired" Type="CheckBox" AllowNull="False" TextAlign="Center" />
                                    <px:PXGridColumn DataField="IsSpecialOrder" Type="CheckBox" AllowNull="False" TextAlign="Center" CommitChanges="true" />
                                    <px:PXGridColumn DataField="EquipmentAction" CommitChanges="True" />
                                    <px:PXGridColumn DataField="Comment" />
                                    <px:PXGridColumn DataField="SMEquipmentID" CommitChanges="True" />
                                    <px:PXGridColumn DataField="NewEquipmentLineNbr" CommitChanges="True" />
                                    <px:PXGridColumn DataField="ComponentID" CommitChanges="True" />
                                    <px:PXGridColumn DataField="EquipmentComponentLineNbr" CommitChanges="True" />
                                    <px:PXGridColumn DataField="RelatedDocument" RenderEditorText="True" LinkCommand="SOLine$RelatedDocument$Link"/>
									<px:PXGridColumn DataField="SDSelected" CommitChanges="True" Type="CheckBox" />
                                    <px:PXGridColumn DataField="SubItemID" NullText="<SPLIT>" CommitChanges="True" />
                                    <px:PXGridColumn AllowNull="False" DataField="AutoCreateIssueLine" TextAlign="Center" Type="CheckBox" AllowShowHide="Server" />
                                    <px:PXGridColumn AllowNull="False" DataField="IsFree" TextAlign="Center" CommitChanges="true" Type="CheckBox" />
                                    <px:PXGridColumn DataField="SiteID" CommitChanges="True" />
                                    <px:PXGridColumn DataField="LocationID" AllowShowHide="Server" NullText="<SPLIT>" />
                                    <px:PXGridColumn DataField="TranDesc" />
                                    <px:PXGridColumn DataField="UOM" CommitChanges="True" AllowDragDrop="true"/>
                                    <px:PXGridColumn AllowNull="False" DataField="OrderQty" TextAlign="Right" CommitChanges="True" AllowDragDrop="true"/>
                                    <px:PXGridColumn DataField="BaseOrderQty" TextAlign="Right" />
                                    <px:PXGridColumn DataField="QtyOnOrders" />
                                    <px:PXGridColumn DataField="BlanketOpenQty" />
                                    <px:PXGridColumn DataField="UnshippedQty" />
                                    <px:PXGridColumn AllowNull="False" DataField="ShippedQty" TextAlign="Right" />
                                    <px:PXGridColumn DataField="OpenQty" TextAlign="Right" AllowNull="False" />
                                    <px:PXGridColumn DataField="CuryUnitCost" TextAlign="Right" CommitChanges="True" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryUnitPrice" TextAlign="Right" CommitChanges="True"/>
                                    <px:PXGridColumn DataField="ManualPrice" TextAlign="Center" AllowNull="False" Type="CheckBox" CommitChanges="True"/>
                                    <px:PXGridColumn AllowNull="False" DataField="CuryExtPrice" TextAlign="Right" CommitChanges="True" />
                                    <px:PXGridColumn AllowNull="False" DataField="DiscPct" TextAlign="Right" CommitChanges="True" />
                                    <px:PXGridColumn DataField="CuryDiscAmt" TextAlign="Right" AllowNull="False" CommitChanges="True" />
                                    <px:PXGridColumn DataField="DiscountID" TextAlign="Left" CommitChanges="True" RenderEditorText="True" />
                                    <px:PXGridColumn DataField="DiscountSequenceID" TextAlign="Left" />
                                    <px:PXGridColumn DataField="ManualDisc" TextAlign="Center" AllowNull="False" CommitChanges="True" Type="CheckBox" />
                                    <px:PXGridColumn DataField="AutomaticDiscountsDisabled" TextAlign="Center" AllowNull="False" Type="CheckBox" />
                                    <px:PXGridColumn DataField="CuryDiscPrice" NullText="0.0" TextAlign="Right" />
                                    <px:PXGridColumn DataField="SkipLineDiscounts" Type="CheckBox" TextAlign="Center" />
                                    <px:PXGridColumn DataField="AvgCost" TextAlign="Right" NullText="0.0" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryLineAmt" TextAlign="Right" />
                                    <px:PXGridColumn DataField="MarginPct" TextAlign="Right" />
                                    <px:PXGridColumn DataField="CuryMarginAmt" TextAlign="Right" />
                                    <px:PXGridColumn DataField="SchedOrderDate" CommitChanges="True" NullText="<SPLIT>" />
                                    <px:PXGridColumn DataField="CustomerOrderNbr" CommitChanges="True" />
                                    <px:PXGridColumn DataField="CustomerLocationID" CommitChanges="true" />
                                    <px:PXGridColumn DataField="CustomerLocationID_Location_descr" />
                                    <px:PXGridColumn DataField="ShipVia" AllowShowHide="Server" />
                                    <px:PXGridColumn DataField="FOBPoint" AllowShowHide="Server" />
                                    <px:PXGridColumn DataField="ShipTermsID" AllowShowHide="Server" />
                                    <px:PXGridColumn DataField="ShipZoneID" AllowShowHide="Server" />
                                    <px:PXGridColumn DataField="SchedShipDate" CommitChanges="True" NullText="<SPLIT>" />
                                    <px:PXGridColumn DataField="TaxZoneID" CommitChanges="true" />
                                    <px:PXGridColumn DataField="DRTermStartDate" CommitChanges="true" />
                                    <px:PXGridColumn DataField="DRTermEndDate" CommitChanges="true" />
                                    <px:PXGridColumn DataField="CuryUnbilledAmt" AllowNull="False" TextAlign="Right" />
                                    <px:PXGridColumn DataField="RequestDate" />
                                    <px:PXGridColumn DataField="ShipDate" />
                                    <px:PXGridColumn DataField="ShipComplete" RenderEditorText="True" />
                                    <px:PXGridColumn AllowNull="False" DataField="CompleteQtyMin" TextAlign="Right" />
                                    <px:PXGridColumn AllowNull="False" DataField="CompleteQtyMax" TextAlign="Right" />
                                    <px:PXGridColumn AllowNull="False" DataField="Completed" TextAlign="Center" Type="CheckBox" CommitChanges="true" />
                                    <px:PXGridColumn DataField="OrigOrderType" />
                                    <px:PXGridColumn DataField="OrigOrderNbr" LinkCommand="ViewOrigOrder" />
                                    <px:PXGridColumn DataField="POCreate" AllowNull="False" CommitChanges="True" TextAlign="Center" Type="CheckBox" />
                                    <px:PXGridColumn DataField="IsPOLinkAllowed" Type="CheckBox" />
                                    <px:PXGridColumn DataField="POSource" CommitChanges="True" RenderEditorText="True" />
                                    <px:PXGridColumn DataField="POCreateDate" CommitChanges="True" NullText="<SPLIT>" />
                                    <px:PXGridColumn DataField="POOrderNbr" />
                                    <px:PXGridColumn DataField="POOrderStatus" />
                                    <px:PXGridColumn DataField="POLineNbr" />
                                    <px:PXGridColumn DataField="POLinkActive" Type="CheckBox" TextAlign="Center" CommitChanges="True" />
                                    <px:PXGridColumn DataField="LotSerialNbr" AllowShowHide="Server" NullText="&lt;SPLIT&gt;" CommitChanges="True" />
                                    <px:PXGridColumn DataField="ExpireDate" AllowShowHide="Server" />
                                    <px:PXGridColumn DataField="ReasonCode" CommitChanges ="true" />
                                    <px:PXGridColumn DataField="SalesPersonID" RenderEditorText="True" />
                                    <px:PXGridColumn DataField="TaxCategoryID" CommitChanges="true" />
                                    <px:PXGridColumn DataField="AvalaraCustomerUsageType" CommitChanges="true" />
                                    <px:PXGridColumn DataField="Commissionable" AllowNull="False" CommitChanges="True" TextAlign="Center" Type="CheckBox" />
                                    <px:PXGridColumn DataField="BlanketNbr" LinkCommand="ViewBlanketOrder" />
                                    <px:PXGridColumn DataField="AlternateID" />
                                    <px:PXGridColumn DataField="SalesAcctID" CommitChanges ="True" />
                                    <px:PXGridColumn DataField="SalesSubID" />
                                    <px:PXGridColumn DataField="TaskID" Label="Task" CommitChanges ="True" />
                                    <px:PXGridColumn DataField="CostCodeID" Label="Task" CommitChanges="true" />
                                    <px:PXGridColumn DataField="CuryUnitPriceDR" AllowShowHide="Server" />
                                    <px:PXGridColumn DataField="DiscPctDR" AllowShowHide="Server" />
                                    <px:PXGridColumn DataField="AMProdCreate" Type="Checkbox" TextAlign="Center" Width="100px" AutoCallBack="True" CommitChanges="true" />
									<px:PXGridColumn DataField="AMorderType" Width="80px" />
									<px:PXGridColumn DataField="AMProdOrdID" Width="90" />
									<px:PXGridColumn DataField="AMEstimateID" Width="90" />
									<px:PXGridColumn DataField="AMEstimateRevisionID" Width="80" />
									<px:PXGridColumn DataField="AMParentLineNbr" Width="85px" />
									<px:PXGridColumn DataField="AMIsSupplemental" Type="CheckBox" TextAlign="Center" Width="85px" />
									<px:PXGridColumn DataField="AMConfigKeyID" Width="90" CommitChanges="true" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" />
                        <ActionBar>
                            <CustomItems>
                                <px:PXToolBarButton Text="Add Items" Key="cmdASI">
                                    <AutoCallBack Command="ShowItems" Target="ds">
                                        <Behavior CommitChanges="True" PostData="Page" />
                                    </AutoCallBack>
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Text="Add Matrix Items" CommandSourceID="ds" CommandName="ShowMatrixPanel" />
                                <px:PXToolBarButton Text="Add Invoice" CommandSourceID="ds" CommandName="AddInvoice" />
                                <px:PXToolBarButton CommandSourceID="ds" CommandName="AddBlanketLine" />
                                <px:PXToolBarButton Text="Line Details" Key="cmdLS" CommandName="SOOrderLineSplittingExtension_ShowSplits" CommandSourceID="ds" DependOnGrid="grid">
                                    <AutoCallBack>
                                        <Behavior CommitChanges="True" PostData="Page" />
                                    </AutoCallBack>
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Text="PO Link" DependOnGrid="grid" StateColumn="IsPOLinkAllowed">
                                    <AutoCallBack Command="POSupplyOK" Target="ds" />
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Text="Item Availability" DependOnGrid="grid" StateColumn="IsStockItem">
                                    <AutoCallBack Command="ItemAvailability" Target="ds" />
                                </px:PXToolBarButton>

                                <px:PXToolBarButton Text="Insert Row" SyncText="false" ImageSet="main" ImageKey="AddNew">
																	<AutoCallBack Target="grid" Command="AddNew" Argument="1"></AutoCallBack>
																	<ActionBar ToolBarVisible="External" MenuVisible="true" />
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Text="Cut Row" SyncText="false" ImageSet="main" ImageKey="Copy">
																	<AutoCallBack Target="grid" Command="Copy"></AutoCallBack>
																	<ActionBar ToolBarVisible="External" MenuVisible="true" />
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Text="Insert Cut Row" SyncText="false" ImageSet="main" ImageKey="Paste">
																	<AutoCallBack Target="grid" Command="Paste"></AutoCallBack>
																	<ActionBar ToolBarVisible="External" MenuVisible="true" />
                                </px:PXToolBarButton>
								<px:PXToolBarButton Text="Configure" DependOnGrid="grid" StateColumn="IsConfigurable">
									<AutoCallBack Command="ConfigureEntry" Target="ds" />
								</px:PXToolBarButton>
                                <px:PXToolBarButton Text="Link Prod Order" DependOnGrid="grid" StateColumn="AMProdCreate" >
                                    <AutoCallBack Command="linkProdOrder" Target="ds" />
                                </px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
                        <CallbackCommands PasteCommand="PasteLine">
                            <Save PostData="Container" />
                        </CallbackCommands>
                        <Mode InitNewRow="True" AllowFormEdit="True" AllowUpload="True" AllowDragRows="true" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
	        <px:PXTabItem Text="Estimates" BindingContext="form" RepaintOnDemand="false">
		        <Template>
			        <px:PXGrid runat="server" ID="gridEstimates" SyncPosition="True" Height="200px" SkinID="DetailsInTab" Width="100%" AutoCallBack="Refresh" DataSourceID="ds">
				        <AutoSize Enabled="True" />
				        <AutoCallBack Command="Refresh" Enabled="True" Target="gridEstimates" />
				        <Levels>
					        <px:PXGridLevel DataMember="OrderEstimateRecords" DataKeyNames="EstimateID">
						        <RowTemplate>
							        <px:PXSelector runat="server" ID="edEstBranch" DataField="AMEstimateItem__BranchID" />
							        <px:PXSelector runat="server" ID="edEstInventoryCD" DataField="AMEstimateItem__InventoryCD" />
							        <px:PXTextEdit runat="server" ID="edEstItemDesc" DataField="AMEstimateItem__ItemDesc" />
							        <px:PXSegmentMask runat="server" ID="EstimateSubItemID" DataField="AMEstimateItem__SubItemID" CommitChanges="True" />
							        <px:PXSelector runat="server" ID="edEstSiteID" DataField="AMEstimateItem__SiteID" />
							        <px:PXSelector runat="server" ID="edEstUOM" DataField="AMEstimateItem__UOM" />
							        <px:PXNumberEdit runat="server" DataField="OrderQty" ID="edEstOrderQty" />
							        <px:PXNumberEdit runat="server" DataField="CuryUnitPrice" ID="edEstCuryUnitPrice" />
							        <px:PXNumberEdit runat="server" DataField="CuryExtPrice" ID="edEstCuryExtPrice" />
							        <px:PXSelector runat="server" ID="edEstEstimateID" DataField="EstimateID" />
							        <px:PXSelector runat="server" ID="edEstRevisionID" DataField="RevisionID" CommitChanges="True" />
							        <px:PXSelector runat="server" ID="edEstTaxCategoryID" DataField="TaxCategoryID" />
							        <px:PXSelector runat="server" ID="edEstOwnerID" DataField="AMEstimateItem__OwnerID" />
							        <px:PXSelector runat="server" ID="edEstEngineerID" DataField="AMEstimateItem__EngineerID" />
							        <px:PXDateTimeEdit runat="server" ID="edEstRequestDate" DataField="AMEstimateItem__RequestDate" />
							        <px:PXDateTimeEdit runat="server" ID="edEstPromiseDate" DataField="AMEstimateItem__PromiseDate" />
							        <px:PXSelector runat="server" ID="edEstEstimateClassID" DataField="AMEstimateItem__EstimateClassID" />
                                </RowTemplate>
						        <Columns>
							        <px:PXGridColumn DataField="AMEstimateItem__BranchID" />
							        <px:PXGridColumn DataField="AMEstimateItem__InventoryCD" />
							        <px:PXGridColumn DataField="AMEstimateItem__ItemDesc" />
							        <px:PXGridColumn DataField="AMEstimateItem__SubItemID" />
							        <px:PXGridColumn DataField="AMEstimateItem__SiteID" />
							        <px:PXGridColumn DataField="AMEstimateItem__UOM" />
							        <px:PXGridColumn DataField="OrderQty" TextAlign="Right" />
							        <px:PXGridColumn DataField="CuryUnitPrice" TextAlign="Right" />
							        <px:PXGridColumn DataField="CuryExtPrice" TextAlign="Right" />
							        <px:PXGridColumn DataField="EstimateID" LinkCommand="ViewEstimate" />
							        <px:PXGridColumn DataField="RevisionID" />
							        <px:PXGridColumn DataField="TaxCategoryID" />
							        <px:PXGridColumn DataField="AMEstimateItem__OwnerID" />
							        <px:PXGridColumn DataField="AMEstimateItem__EngineerID" />
							        <px:PXGridColumn DataField="AMEstimateItem__RequestDate" />
							        <px:PXGridColumn DataField="AMEstimateItem__PromiseDate" />
							        <px:PXGridColumn DataField="AMEstimateItem__EstimateClassID" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
				        <ActionBar>
					        <CustomItems>
						        <px:PXToolBarButton Text="Add" CommandSourceID="ds" CommandName="AddEstimate">
							        <AutoCallBack>
								        <Behavior CommitChanges="True" RepaintControlsIDs="gridEstimates" PostData="Self" />
                                    </AutoCallBack>
                                </px:PXToolBarButton>
						        <px:PXToolBarButton Text="Quick Estimate" DependOnGrid="gridEstimates" StateColumn="EstimateID">
							        <AutoCallBack Command="QuickEstimate" Target="ds" />
                                </px:PXToolBarButton>
						        <px:PXToolBarButton Text="Remove" CommandSourceID="ds" CommandName="RemoveEstimate">
							        <AutoCallBack>
								        <Behavior CommitChanges="True" RepaintControlsIDs="gridEstimates" PostData="Self" />
                                    </AutoCallBack>
                                </px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Taxes" VisibleExp="DataControls[&quot;edOrderType&quot;].Value!=TR" BindingContext="form" RepaintOnDemand="false">
                <Template>
                    <px:PXGrid ID="grid1" runat="server" DataSourceID="ds" Height="150px" Style="z-index: 100" TabIndex="200" Width="100%" BorderWidth="0px"
                        SkinID="Details">
                        <Levels>
                            <px:PXGridLevel DataMember="Taxes">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                                    <px:PXTextEdit SuppressLabel="True" ID="edTaxZoneID2" runat="server" DataField="TaxZoneID" CommitChanges="true" AutoRefresh="true"/>
                                    <px:PXSelector SuppressLabel="True" ID="edTaxID" runat="server" DataField="TaxID" CommitChanges="true" AutoRefresh="true"/>
                                    <px:PXNumberEdit SuppressLabel="True" ID="edTaxRate" runat="server" DataField="TaxRate" Enabled="False" />
                                    <px:PXNumberEdit SuppressLabel="True" ID="edCuryTaxableAmt" runat="server" DataField="CuryTaxableAmt" />
                                    <px:PXNumberEdit SuppressLabel="True" ID="edCuryTaxAmt" runat="server" DataField="CuryTaxAmt" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="TaxZoneID" AllowUpdate="False" CommitChanges="true" />
                                    <px:PXGridColumn DataField="TaxID" AllowUpdate="False" CommitChanges="true" />
                                    <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="TaxRate" TextAlign="Right" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryTaxableAmt" TextAlign="Right" />
									<px:PXGridColumn DataField="CuryExemptedAmt" TextAlign="Right" Width="100px" />
									<px:PXGridColumn DataField="TaxUOM" TextAlign="Right" />
									<px:PXGridColumn DataField="TaxableQty" TextAlign="Right" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryTaxAmt" TextAlign="Right" />
                                    <px:PXGridColumn AllowNull="False" DataField="Tax__TaxType" Label="Tax Type" RenderEditorText="True" />
                                    <px:PXGridColumn AllowNull="False" DataField="Tax__PendingTax" Label="Pending VAT" TextAlign="Center" Type="CheckBox" />
                                    <px:PXGridColumn AllowNull="False" DataField="Tax__ReverseTax" Label="Reverse VAT" TextAlign="Center" Type="CheckBox" />
                                    <px:PXGridColumn AllowNull="False" DataField="Tax__ExemptTax" Label="Exempt From VAT" TextAlign="Center" Type="CheckBox" />
                                    <px:PXGridColumn AllowNull="False" DataField="Tax__StatisticalTax" Label="Statistical VAT" TextAlign="Center" Type="CheckBox" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" />
                        <ActionBar PagerGroup="3" PagerOrder="2">
                        </ActionBar>
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Commissions" VisibleExp="DataControls[&quot;edOrderType&quot;].Value!=TR" BindingContext="form" RepaintOnDemand="false">
                <Template>
                    <px:PXFormView ID="Commission" runat="server" DataMember="CurrentDocument" RenderStyle="Simple" SkinID="Transparent">
                        <Template>
                            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
                            <px:PXSegmentMask CommitChanges="True" ID="edSalesPersonID" runat="server" DataField="SalesPersonID" />
                        </Template>
                    </px:PXFormView>
                    <px:PXGrid ID="gridSalesPerTran" runat="server" Height="200px" Width="100%" DataSourceID="ds" BorderWidth="0px" SkinID="Details">
                        <Levels>
                            <px:PXGridLevel DataMember="SalesPerTran">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                                    <px:PXNumberEdit ID="edCommnPct" runat="server" DataField="CommnPct" AllowNull="True" />
                                    <px:PXNumberEdit ID="edCommnAmt" runat="server" DataField="CommnAmt" />
                                    <px:PXNumberEdit ID="edCuryCommnAmt" runat="server" DataField="CuryCommnAmt" />
                                    <px:PXNumberEdit ID="edCommnblAmt" runat="server" DataField="CommnblAmt" />
                                    <px:PXNumberEdit ID="edCuryCommnblAmt" runat="server" DataField="CuryCommnblAmt" AllowNull="True" />
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                                    <px:PXSegmentMask ID="edSalesPersonID_1" runat="server" DataField="SalespersonID" AutoRefresh="True" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="SalespersonID" CommitChanges="True" />
                                    <px:PXGridColumn DataField="CommnPct" TextAlign="Right" />
                                    <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="CuryCommnAmt" TextAlign="Right" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="CuryCommnblAmt" TextAlign="Right" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" MinWidth="100" />
                        <Mode AllowAddNew="False" AllowDelete="False" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Financial">
                <Template>
                    <px:PXLayoutRule runat="server" StartGroup="True" />
                    <px:PXFormView ID="formFinancialInformation" runat="server" DataMember="CurrentDocument" DataSourceID="ds" MarkRequired="Dynamic" RenderStyle="Fieldset" Caption="Financial Information">
                        <Template>
                            <px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" />
                            <px:PXSegmentMask ID="edBranchID" runat="server" CommitChanges="True" DataField="BranchID" DataSourceID="ds" />
                            <px:PXTextEdit ID="edBranchBaseCuryID" runat="server" DataField="BranchBaseCuryID" />
                            <px:PXCheckBox ID="chkDisableAutomaticTaxCalculation" runat="server" DataField="DisableAutomaticTaxCalculation" CommitChanges="True" />
                            <px:PXCheckBox ID="chkOverrideTaxZone" runat="server" DataField="OverrideTaxZone">
                                <AutoCallBack Command="OverrideBlanketTaxZone" Target="ds">
                                    <Behavior CommitChanges="True" />
                                </AutoCallBack>
                            </px:PXCheckBox>
                            <px:PXSelector ID="edTaxZoneID" runat="server" CommitChanges="True" DataField="TaxZoneID" DataSourceID="ds" />
                            <px:PXDropDown ID="edTaxCalcMode" runat="server" CommitChanges="True" DataField="TaxCalcMode" />
                                <px:PXTextEdit ID="edExternalTaxExemptionNumber" runat="server" CommitChanges="True" DataField="ExternalTaxExemptionNumber" />
                            <px:PXDropDown ID="edAvalaraCustomerUsageTypeID" runat="server" CommitChanges="True" DataField="AvalaraCustomerUsageType" />
                            <px:PXCheckBox ID="chkBillSeparately" runat="server" CommitChanges="True" DataField="BillSeparately" />
                            <px:PXTextEdit ID="edInvoiceNbr" runat="server" DataField="InvoiceNbr" />
                            <px:PXDateTimeEdit ID="edInvoiceDate" runat="server" CommitChanges="True" DataField="InvoiceDate" />
                            <px:PXSelector ID="edTermsID" runat="server" CommitChanges="True" DataField="TermsID" DataSourceID="ds" />
                            <px:PXDateTimeEdit ID="edDueDate" runat="server" DataField="DueDate" />
                            <px:PXDateTimeEdit ID="edDiscDate" runat="server" DataField="DiscDate" />
                            <px:PXSelector ID="edFinPeriodID" runat="server" DataField="FinPeriodID" DataSourceID="ds" />
                            <px:PXLayoutRule runat="server" StartColumn="True" />
                        </Template>
                        <ContentStyle BackColor="Transparent" BorderStyle="None" />
                    </px:PXFormView>
                    <px:PXLayoutRule runat="server" StartColumn="True" />
                    <px:PXFormView ID="formPaymentInformation" runat="server" DataMember="CurrentDocument" DataSourceID="ds" MarkRequired="Dynamic" RenderStyle="Fieldset" Caption="Payment Information">
                        <Template>
                            <px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" />
                            <px:PXCheckBox ID="chkOverridePrepayment" runat="server" DataField="OverridePrepayment" CommitChanges="True" />
							<px:PXNumberEdit ID="edPrepaymentReqPct" runat="server" DataField="PrepaymentReqPct" CommitChanges="true" />
							<px:PXNumberEdit ID="edCuryPrepaymentReqAmt" runat="server" DataField="CuryPrepaymentReqAmt" CommitChanges="true" />
							<px:PXCheckBox ID="chkPrepaymentReqSatisfied" runat="server" DataField="PrepaymentReqSatisfied" />
                            <px:PXSelector CommitChanges="True" ID="edPaymentMethodID" runat="server" DataField="PaymentMethodID" AutoRefresh="True" DataSourceID="ds" />
                            <px:PXSelector CommitChanges="True" ID="edPMInstanceID" runat="server" DataField="PMInstanceID" TextField="Descr" AutoRefresh="True" AutoGenerateColumns="True" DataSourceID="ds" />
                            <px:PXSegmentMask CommitChanges="True" ID="edCashAccountID" runat="server" DataField="CashAccountID" DataSourceID="ds" />
                            <px:PXTextEdit ID="edExtRefNbr" runat="server" DataField="ExtRefNbr" CommitChanges="true" />
                            <px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" GroupCaption="Ownership" />
                            <px:PXSelector ID="edWorkgroupID" runat="server" AutoRefresh="True" DataField="WorkgroupID" DataSourceID="ds" CommitChanges="true" />
                            <px:PXSelector ID="edOwnerID" runat="server" AutoRefresh="True" DataField="OwnerID" DataSourceID="ds" CommitChanges="true" />
                            
                            <px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" GroupCaption="Other Information" />
                            <px:PXTextEdit ID="edOrigOrderType" runat="server" DataField="OrigOrderType" Enabled="False" />
                            <px:PXSelector ID="edOrigOrderNbr" runat="server" DataField="OrigOrderNbr" Enabled="False" AllowEdit="true" />
                            <px:PXCheckBox ID="chkEmailed" runat="server" DataField="Emailed" Height="18px" Enabled="False" />
                            <px:PXCheckBox ID="chkPrinted" runat="server" DataField="Printed" Height="18px" Enabled="False" />
                        </Template>
                        <ContentStyle BackColor="Transparent" BorderStyle="None" />
                    </px:PXFormView>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Shipping" VisibleExp="DataControls[&quot;chkShowShipmentsTab&quot;].Value == 1" BindingContext="form">
                <Template>
                    <px:PXLayoutRule runat="server" StartGroup="True" />
                    <px:PXFormView ID="formDeliverySettings" runat="server" DataSourceID="ds" DataMember="CurrentDocument" RenderStyle="Fieldset" Caption="Delivery Settings">
                        <Template>
                            <px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="XM" Merge="True" />
                            <px:PXSelector CommitChanges="True" Size="s" ID="edShipVia" runat="server" DataField="ShipVia" DataSourceID="ds" />
                            <px:PXButton ID="shopRates" runat="server" Text="Shop For Rates" CommandName="ShopRates" CommandSourceID="ds" />
                            <px:PXLayoutRule runat="server" />
                            <px:PXCheckBox ID="edWillCall" runat="server" DataField="WillCall" Tooltip="The Will Call flag depends on the Common Carrier selection in the Ship Via field." Width="80px" />
							<px:PXDropDown runat="server" ID="edDeliveryConfirmation" DataField="DeliveryConfirmation" CommitChanges="True" />
							<px:PXDropDown runat="server" ID="edEndorsementService" DataField="EndorsementService" CommitChanges="True" />
                            <px:PXDropDown runat="server" ID="edFreightClass" DataField="FreightClass" />
                            <px:PXSelector ID="edFOBPoint" runat="server" DataField="FOBPoint" DataSourceID="ds" />
                            <px:PXNumberEdit ID="edPriority" runat="server" DataField="Priority" />
                            <px:PXSelector CommitChanges="True" ID="edShipTermsID" runat="server" DataField="ShipTermsID" DataSourceID="ds" AutoRefresh="True" />
                            <px:PXSelector CommitChanges="True" ID="edShipZoneID" runat="server" DataField="ShipZoneID" DataSourceID="ds" />
                            <px:PXCheckBox ID="chkResedential" runat="server" DataField="Resedential" />
                            <px:PXCheckBox ID="chkSaturdayDelivery" runat="server" DataField="SaturdayDelivery" />
                            <px:PXCheckBox ID="chkInsurance" runat="server" DataField="Insurance" CommitChanges="True" />
                            <px:PXCheckBox CommitChanges="True" ID="chkUseCustomerAccount" runat="server" DataField="UseCustomerAccount" />
                            <px:PXCheckBox CommitChanges="True" ID="chkGroundCollect" runat="server" DataField="GroundCollect" />
							<px:PXFormView runat="server" ID="formPacejet" DataMember="CarrierData" SkinID="Inside" RenderStyle="Simple">
								<Template>
									<px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="XM" />
									<px:PXCheckBox runat="server" ID="chkAdditionalHandling" DataField="AdditionalHandling" />
									<px:PXCheckBox runat="server" ID="chkLiftGate" DataField="LiftGate" />
									<px:PXCheckBox runat="server" ID="chkInsideDelivery" DataField="InsideDelivery" />
									<px:PXCheckBox runat="server" ID="chkLimitedAccess" DataField="LimitedAccess" />
								</Template>
							</px:PXFormView>
                        </Template>
                        <ContentStyle BackColor="Transparent" BorderStyle="None" />
                    </px:PXFormView>
                    <px:PXFormView ID="formI" runat="server" DataSourceID="ds" DataMember="CurrentDocument" Caption="Intercompany Purchase" RenderStyle="Fieldset">
                        <Template>
                            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                            <px:PXDropDown ID="edIntercompanyPOType" runat="server" DataField="IntercompanyPOType" />
                            <px:PXSelector ID="edIntercompanyPONbr" runat="server" DataField="IntercompanyPONbr" AllowEdit="True" Enabled="False" />
                            <px:PXSelector ID="edIntercompanyPOReturnNbr" runat="server" DataField="IntercompanyPOReturnNbr" AllowEdit="True" Enabled="False" />
                        </Template>
                        <ContentStyle BackColor="Transparent" BorderStyle="None" />
                    </px:PXFormView>
                    <px:PXLayoutRule runat="server" StartColumn="True" Merge="True"/>
                    <px:PXFormView ID="formShippingSettings" runat="server" DataSourceID="ds" DataMember="CurrentDocument" RenderStyle="Fieldset" Caption="Order Shipping Settings">
                        <Template>
                            <px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="XM" />
                            <px:PXDateTimeEdit CommitChanges="True" ID="edShipDate" runat="server" DataField="ShipDate" />
                            <px:PXCheckBox ID="chkShipSeparately" runat="server" DataField="ShipSeparately" />
                            <px:PXLayoutRule runat="server" />
                            <px:PXDropDown ID="edShipComplete" runat="server" AllowNull="False" DataField="ShipComplete" SelectedIndex="2" CommitChanges="True" />
                            <px:PXLayoutRule runat="server" Merge="True" />
                            <px:PXDateTimeEdit ID="edCancelDate" runat="server" DataField="CancelDate" />
                            <px:PXCheckBox ID="chkCancelled" runat="server" DataField="Cancelled" Enabled="false"/>
                            <px:PXLayoutRule runat="server" />
                            <px:PXSegmentMask CommitChanges="True" ID="edDefaultSiteID" runat="server" DataField="DefaultSiteID" DataSourceID="ds" />
                            <px:PXLayoutRule runat="server" Merge="True" />
                        </Template>
                        <ContentStyle BackColor="Transparent" BorderStyle="None" />
                    </px:PXFormView>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Addresses" VisibleExp="DataControls[&quot;chkShowShipmentsTab&quot;].Value == 1" BindingContext="form">
                <Template>
                    <px:PXLayoutRule runat="server" StartGroup="True" />
                    <px:PXFormView ID="formD" runat="server" Caption="Ship-To Contact" DataMember="Shipping_Contact" DataSourceID="ds" RenderStyle="Fieldset">
                        <Template>
                            <px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="XM" />
                            <px:PXCheckBox CommitChanges="True" ID="chkOverrideContact" runat="server" DataField="OverrideContact" />
                            <px:PXTextEdit ID="edFullName" runat="server" DataField="FullName" />
                            <px:PXTextEdit ID="edAttention" runat="server" DataField="Attention" />
                            <px:PXMaskEdit ID="edPhone1" runat="server" DataField="Phone1" />
                            <px:PXMailEdit ID="edEmail" runat="server" DataField="Email" CommitChanges="True" />
                        </Template>
                        <ContentStyle BackColor="Transparent" BorderStyle="None" />
                    </px:PXFormView>
                    <px:PXFormView ID="formB" DataMember="Shipping_Address" runat="server" DataSourceID="ds" Caption="Ship-To Address" RenderStyle="Fieldset" SyncPosition="True">
                        <Template>
                            <px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="XM" />
                            <px:PXCheckBox CommitChanges="True" ID="chkOverrideAddress" runat="server" DataField="OverrideAddress" />
                            <px:PXButton ID="btnShippingAddressLookup" runat="server" CommandName="ShippingAddressLookup" CommandSourceID="ds" Size="xs" TabIndex="-1" />
                            <px:PXTextEdit ID="edAddressLine1" runat="server" DataField="AddressLine1" />
                            <px:PXTextEdit ID="edAddressLine2" runat="server" DataField="AddressLine2" />
                            <px:PXTextEdit ID="edCity" runat="server" DataField="City" />
                            <px:PXSelector ID="edCountryID" runat="server" DataField="CountryID" AutoRefresh="True" DataSourceID="ds" CommitChanges="true" />
                            <px:PXSelector ID="edState" runat="server" DataField="State" AutoRefresh="True" DataSourceID="ds" CommitChanges="True">
                                <CallBackMode PostData="Container" />
                                <Parameters>
                                    <px:PXControlParam ControlID="formB" Name="SOShippingAddress.countryID" PropertyName="DataControls[&quot;edCountryID&quot;].Value"
                                        Type="String" />
                                </Parameters>
                            </px:PXSelector>
                            <px:PXMaskEdit CommitChanges="True" ID="edPostalCode" runat="server" DataField="PostalCode" />
                            <px:PXNumberEdit ID="edLatitude" runat="server" DataField="Latitude" AllowNull="True" />
                            <px:PXNumberEdit ID="edLongitude" runat="server" DataField="Longitude" AllowNull="True" />
                            <px:PXCheckBox ID="chkIsValidated" runat="server" DataField="IsValidated" Enabled="False" />
                        </Template>
                        <ContentStyle BackColor="Transparent" BorderStyle="None" />
                    </px:PXFormView>
                    <px:PXLayoutRule runat="server" StartColumn="True" />
                    <px:PXFormView ID="formC" runat="server" Caption="Bill-To Contact" DataMember="Billing_Contact" DataSourceID="ds" RenderStyle="Fieldset">
                        <Template>
                            <px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="XM" />
                            <px:PXCheckBox ID="chkOverrideContact" runat="server" CommitChanges="True" DataField="OverrideContact" />
                            <px:PXTextEdit ID="edFullName" runat="server" DataField="FullName" />
                            <px:PXTextEdit ID="edAttention" runat="server" DataField="Attention" />
                            <px:PXMaskEdit ID="edPhone1" runat="server" DataField="Phone1" />
                            <px:PXMailEdit ID="edEmail" runat="server" DataField="Email" CommitChanges="True" />
                        </Template>
                        <ContentStyle BackColor="Transparent" BorderStyle="None" />
                    </px:PXFormView>
                    <px:PXFormView ID="formA" DataMember="Billing_Address" runat="server" DataSourceID="ds" Caption="Bill-To Address" RenderStyle="Fieldset" SyncPosition="True">
                        <Template>
                            <px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="XM" />
                            <px:PXCheckBox CommitChanges="True" ID="chkOverrideAddress" runat="server" DataField="OverrideAddress" />
                            <px:PXButton ID="btnAddressLookup" runat="server" CommandName="AddressLookup" CommandSourceID="ds" Size="xs" TabIndex="-1" />
                            <px:PXTextEdit ID="edAddressLine1" runat="server" DataField="AddressLine1" />
                            <px:PXTextEdit ID="edAddressLine2" runat="server" DataField="AddressLine2" />
                            <px:PXTextEdit ID="edCity" runat="server" DataField="City" />
                            <px:PXSelector ID="edCountryID" runat="server" DataField="CountryID" AutoRefresh="True" DataSourceID="ds" CommitChanges="true" />
                            <px:PXSelector ID="edState" runat="server" DataField="State" AutoRefresh="True" DataSourceID="ds">
                                <CallBackMode PostData="Container" />
                                <Parameters>
                                    <px:PXControlParam ControlID="formA" Name="SOBillingAddress.countryID" PropertyName="DataControls[&quot;edCountryID&quot;].Value"
                                        Type="String" />
                                </Parameters>
                            </px:PXSelector>
                            <px:PXMaskEdit CommitChanges="True" ID="edPostalCode" runat="server" DataField="PostalCode" />
                            <px:PXCheckBox ID="chkIsValidated" runat="server" DataField="IsValidated" Enabled="False" />
                        </Template>
                        <ContentStyle BackColor="Transparent" BorderStyle="None" />
                    </px:PXFormView>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Approvals" BindingContext="form" RepaintOnDemand="false">
                <Template>
                    <px:PXGrid ID="gridApproval" runat="server" DataSourceID="ds" Width="100%" SkinID="DetailsInTab" NoteIndicator="True" Style="left: 0px; top: 0px;">
                        <AutoSize Enabled="True" />
                        <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
                        <Levels>
                            <px:PXGridLevel DataMember="Approval">
                                <Columns>
                                    <px:PXGridColumn DataField="ApproverEmployee__AcctCD" />
                                    <px:PXGridColumn DataField="ApproverEmployee__AcctName" />
                                    <px:PXGridColumn DataField="WorkgroupID" />
                                    <px:PXGridColumn DataField="ApprovedByEmployee__AcctCD" />
                                    <px:PXGridColumn DataField="ApprovedByEmployee__AcctName" />
                                    <px:PXGridColumn DataField="OrigOwnerID" Visible="false" SyncVisible="false" />
                                    <px:PXGridColumn DataField="ApproveDate" />
                                    <px:PXGridColumn DataField="Status" AllowNull="False" AllowUpdate="False" RenderEditorText="True" />
                                    <px:PXGridColumn DataField="Reason" AllowUpdate="False" />
                                    <px:PXGridColumn DataField="AssignmentMapID"  Visible="false" SyncVisible="false" />
                                    <px:PXGridColumn DataField="RuleID" Visible="false" SyncVisible="false" />
                                    <px:PXGridColumn DataField="StepID" Visible="false" SyncVisible="false" />
                                    <px:PXGridColumn DataField="CreatedDateTime" Visible="false" SyncVisible="false" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Discounts" VisibleExp="DataControls[&quot;chkShowDiscountsTab&quot;].Value == 1" BindingContext="form" RepaintOnDemand="false">
                <Template>
                    <px:PXFormView ID="DiscountParameters" runat="server" DataMember="CurrentDocument" RenderStyle="Simple" SkinID="Transparent">
                        <Template>
                            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
                            <px:PXCheckBox ID="chkDisableAutomaticDiscountCalculation" runat="server" DataField="DisableAutomaticDiscountCalculation" AlignLeft="true" CommitChanges="true" />
                        </Template>
                    </px:PXFormView>
                    <px:PXGrid ID="formDiscountDetail" runat="server" DataSourceID="ds" Width="100%" SkinID="Details" BorderStyle="None" SyncPosition="True">
                        <Levels>
                            <px:PXGridLevel DataMember="DiscountDetails">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                                    <px:PXCheckBox ID="chkSkipDiscount" runat="server" DataField="SkipDiscount" />
                                    <px:PXSelector ID="edDiscountID" runat="server" DataField="DiscountID"
                                        AllowEdit="True" edit="1" />
                                    <px:PXSelector ID="edDiscountSequenceID" runat="server" DataField="DiscountSequenceID" AllowEdit="True" AutoRefresh="True" edit="1" />
                                    <px:PXDropDown ID="edType" runat="server" DataField="Type" Enabled="False" />
                                    <px:PXCheckBox ID="chkIsManual" runat="server" DataField="IsManual" />
                                    <px:PXNumberEdit ID="edCuryDiscountableAmt" runat="server" DataField="CuryDiscountableAmt" />
                                    <px:PXNumberEdit ID="edDiscountableQty" runat="server" DataField="DiscountableQty" />
                                    <px:PXNumberEdit ID="edCuryDiscountAmt" runat="server" DataField="CuryDiscountAmt" CommitChanges="true" />
                                    <px:PXNumberEdit ID="edDiscountPct" runat="server" DataField="DiscountPct" CommitChanges="true" />
                                    <px:PXSegmentMask ID="edFreeItemID" runat="server" DataField="FreeItemID" AllowEdit="True" />
                                    <px:PXNumberEdit ID="edFreeItemQty" runat="server" DataField="FreeItemQty" />
                                    <px:PXTextEdit ID="edExtDiscCode" runat="server" DataField="ExtDiscCode" />
                                    <px:PXTextEdit ID="edDescription" runat="server" DataField="Description" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="SkipDiscount" Type="CheckBox" TextAlign="Center" />
                                    <px:PXGridColumn DataField="LineNbr" />
                                    <px:PXGridColumn DataField="DiscountID" CommitChanges="True" />
                                    <px:PXGridColumn DataField="DiscountSequenceID" CommitChanges="True" />
                                    <px:PXGridColumn DataField="Type" RenderEditorText="True" />
                                    <px:PXGridColumn DataField="IsManual" Type="CheckBox" TextAlign="Center" />
                                    <px:PXGridColumn DataField="CuryDiscountableAmt" TextAlign="Right" />
                                    <px:PXGridColumn DataField="DiscountableQty" TextAlign="Right" />
                                    <px:PXGridColumn DataField="CuryDiscountAmt" TextAlign="Right" CommitChanges="true" />
                                    <px:PXGridColumn DataField="DiscountPct" TextAlign="Right" CommitChanges="true" />
                                    <px:PXGridColumn DataField="FreeItemID" DisplayFormat="&gt;CCCCC-CCCCCCCCCCCCCCC" />
                                    <px:PXGridColumn DataField="FreeItemQty" TextAlign="Right" />
                                    <px:PXGridColumn DataField="ExtDiscCode" />
                                    <px:PXGridColumn DataField="Description" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Shipments" VisibleExp="DataControls[&quot;chkShowShipmentsTab&quot;].Value == 1" BindingContext="form" LoadOnDemand="True" RepaintOnDemand="True" >
                <Template>
                    <px:PXGrid ID="grid5" runat="server" DataSourceID="ds" Height="150px" Style="z-index: 100" Width="100%" SkinID="Details"
                        BorderWidth="0px" SyncPosition="true" AdjustPageSize="Auto">
                        <Levels>
                            <px:PXGridLevel DataMember="ShipmentList">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                                    <px:PXTextEdit ID="edOrderType3" runat="server" DataField="OrderType" Enabled="False" />
                                    <px:PXTextEdit ID="edOrderNbr3" runat="server" DataField="OrderNbr" Enabled="False" />
                                    <px:PXSelector SuppressLabel="True" Size="s" ID="edInvoiceNbr3" runat="server"
                                        DataField="InvoiceNbr" AutoRefresh="True"
                                        AllowEdit="True" edit="1" />
                                    <px:PXSelector SuppressLabel="True" Size="s" ID="edInvtRefNbr3" runat="server"
                                        DataField="InvtRefNbr" AutoRefresh="True"
                                        AllowEdit="True" edit="1">
                                        <Parameters>
                                            <px:PXSyncGridParam ControlID="grid5" />
                                        </Parameters>
                                    </px:PXSelector>
                                    <px:PXLayoutRule runat="server" />
                                    <px:PXNumberEdit ID="edShipmentQty3" runat="server" DataField="ShipmentQty" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="ShipmentType" />
                                    <px:PXGridColumn DataField="ShipmentNbr" AllowUpdate="False" />
                                    <px:PXGridColumn DataField="DisplayShippingRefNoteID" RenderEditorText="True"
                                        LinkCommand="SOOrderShipment~DisplayShippingRefNoteID~Link" />
                                    <px:PXGridColumn DataField="SOShipment__StatusIsNull" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="Operation" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="OrderType" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="OrderNbr" />
                                    <px:PXGridColumn DataField="ShipDate" Label="Ship Date" />
                                    <px:PXGridColumn AllowNull="False" DataField="ShipmentQty" TextAlign="Right" />
                                    <px:PXGridColumn AllowNull="False" DataField="ShipmentWeight" TextAlign="Right" />
                                    <px:PXGridColumn AllowNull="False" DataField="ShipmentVolume" TextAlign="Right" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="InvoiceType" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="InvoiceNbr" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="InvtDocType" />
                                    <px:PXGridColumn AllowUpdate="False" DataField="InvtRefNbr" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" />
                        <ActionBar PagerGroup="3" PagerOrder="2">
                        </ActionBar>
                        <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Child Orders" VisibleExp="DataControls[&quot;chkShowOrdersTab&quot;].Value == 1" BindingContext="form">
                <Template>
                    <px:PXGrid ID="ordersGrid" runat="server" DataSourceID="ds" Height="150px" Style="z-index: 100" Width="100%" SkinID="Details"
                        BorderWidth="0px" SyncPosition="true" AdjustPageSize="Auto">
                        <Levels>
                            <px:PXGridLevel DataMember="BlanketOrderChildrenDisplayList">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                                    <px:PXTextEdit ID="edChildOrderType" runat="server" DataField="OrderType" Enabled="False" />
                                    <px:PXSelector ID="edChildOrderNbr" runat="server" DataField="OrderNbr" Enabled="False" AllowEdit="true" />
                                    <px:PXSelector ID="edChildInvoiceNbr" runat="server" DataField="InvoiceNbr" AutoRefresh="True" AllowEdit="True" edit="1" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="CustomerLocationID" />
                                    <px:PXGridColumn DataField="OrderNbr" AllowUpdate="False" 
                                        LinkCommand="ViewChildOrder" />
                                    <px:PXGridColumn DataField="OrderDate" AllowUpdate="False" />
                                    <px:PXGridColumn DataField="OrderStatus" />
                                    <px:PXGridColumn DataField="OrderedQty" AllowNull="False" TextAlign="Right" />
                                    <px:PXGridColumn DataField="CuryOrderedAmt" AllowNull="False" TextAlign="Right" />
                                    <px:PXGridColumn DataField="ShipmentType" />
                                    <px:PXGridColumn DataField="ShipmentNbr" AllowUpdate="False" />
                                    <px:PXGridColumn DataField="DisplayShippingRefNoteID" RenderEditorText="True"
                                        LinkCommand="SOBlanketOrderDisplayLink~DisplayShippingRefNoteID~Link" />
                                    <px:PXGridColumn DataField="ShipmentDate" AllowUpdate="False" />
                                    <px:PXGridColumn DataField="ShipmentStatus" />
                                    <px:PXGridColumn DataField="ShippedQty" AllowUpdate="False" />
                                    <px:PXGridColumn DataField="InvoiceType" AllowUpdate="False" />
                                    <px:PXGridColumn DataField="InvoiceNbr" AllowUpdate="False" />
                                    <px:PXGridColumn DataField="InvoiceDate" AllowUpdate="False" />
                                    <px:PXGridColumn DataField="InvoiceStatus" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" />
                        <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Payment Links" RepaintOnDemand="false">
				<Template>
                    <px:PXFormView ID="PayLinks" runat="server" DataMember="CurrentDocument" TabIndex="5000" RenderStyle="Simple" SkinID="Transparent">
                        <Template>
                            <px:PXLayoutRule runat="server" ColumnWidth="400px" StartColumn="true" ControlSize="SM" LabelsWidth="SM" />
                            <px:PXSelector CommitChanges="True" ID="PXSelProcCenter1" runat="server" DataField="ProcessingCenterID" AutoRefresh="true"  />
					        <px:PXDropDown CommitChanges="True" ID="cmbDeliveryMethod1" runat="server"  DataField="DeliveryMethod" />
                            <px:PXFormView ID="PXFormPayLink2" runat="server" DataMember="PayLink" RenderStyle="Simple">
						    <Template>
							    <px:PXLayoutRule runat="server" ControlSize="SM" LabelsWidth="SM" />
							    <px:PXTextEdit Width="300px" ID="txtUrl" runat="server" DataField="Url" />
							    <px:PXDropDown ID="cmbLinkStatus1" runat="server"  DataField="LinkStatus" />
						    </Template>
					        </px:PXFormView>
                            <px:PXLayoutRule runat="server" LabelsWidth="XS" StartColumn="True"  />
					        <px:PXButton ID="btnCreateLink" runat="server" CommandName="CreateLink" Width="165px" CommandSourceID="ds" />
					        <px:PXButton ID="btnSyncLink" runat="server" CommandName="SyncLink" Width="165px" CommandSourceID="ds" />
                            <px:PXButton ID="btnDeactivateLink" runat="server" CommandName="DeactivateLink" Width="165px" CommandSourceID="ds" />
					        <px:PXButton ID="btnResend" runat="server" CommandName="ResendLink" Width="165px" CommandSourceID="ds" />
                        </Template>
					</px:PXFormView>
				</Template>
			</px:PXTabItem>
            <px:PXTabItem Text="Payments" VisibleExp="DataControls[&quot;chkPaymentsApplicable&quot;].Value == 1" BindingContext="form" RepaintOnDemand="false">
                <Template>
                    <div style="margin-right:230px" resize-top="1">
                        <px:PXGrid ID="detgrid" runat="server" DataSourceID="ds"  Width="100%" Height="300px"
                            BorderWidth="0px" SkinID="Details" SyncPosition="True">
                            <Mode InitNewRow="true" />
                            <Levels>
                                <px:PXGridLevel DataMember="Adjustments">
                                    <RowTemplate>
                                        <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                                        <px:PXTextEdit ID="edAdjdOrderType" runat="server" DataField="AdjdOrderType" />
                                        <px:PXDropDown ID="edARPayment__DocType" runat="server" DataField="ARPayment__DocType" />
                                        <px:PXSelector ID="edPaymentMethodID" runat="server" DataField="PaymentMethodID" Enabled="False" />
                                        <px:PXTextEdit ID="edAdjdOrderNbr" runat="server" DataField="AdjdOrderNbr" />
                                        <px:PXSelector ID="edARPayment__RefNbr" runat="server" DataField="ARPayment__RefNbr" AllowEdit="True" edit="1" />
                                        <px:PXDropDown ID="edAdjgDocType" runat="server" DataField="AdjgDocType" />
                                        <px:PXDropDown ID="edARPayment__Status" runat="server" AllowNull="False" DataField="ARPayment__Status" Enabled="False" />
                                        <px:PXSelector ID="edAdjgRefNbr" runat="server" AutoRefresh="True" DataField="AdjgRefNbr">
                                            <Parameters>
                                                <px:PXControlParam ControlID="detgrid" Name="SOAdjust.adjgDocType" PropertyName="DataValues[&quot;AdjgDocType&quot;]" />
                                            </Parameters>
                                        </px:PXSelector>
                                        <px:PXSegmentMask ID="edCashAccountID" runat="server" DataField="CashAccountID" />
                                        <px:PXTextEdit ID="edARPayment__ExtRefNbr" runat="server" DataField="ARPayment__ExtRefNbr" />
                                        <px:PXNumberEdit ID="edCustomerID" runat="server" DataField="CustomerID" />
                                        <px:PXNumberEdit ID="edCuryAdjdAmt" runat="server" DataField="CuryAdjdAmt" />
                                        <px:PXNumberEdit ID="edCuryAdjdBilledAmt" runat="server" DataField="CuryAdjdBilledAmt" />
                                        <px:PXNumberEdit ID="edCuryAdjdTransferredToChildrenAmt" runat="server" DataField="CuryAdjdTransferredToChildrenAmt" />
                                        <px:PXNumberEdit ID="edAdjAmt" runat="server" DataField="AdjAmt" />
                                        <px:PXNumberEdit ID="edCuryDocBal" runat="server" DataField="CuryDocBal" Enabled="False" />
                                    </RowTemplate>
                                    <Columns>
                                        <px:PXGridColumn DataField="AdjgDocType" Label="ARPayment-Type" RenderEditorText="True" />
                                        <px:PXGridColumn DataField="AdjgRefNbr" DisplayFormat="&gt;CCCCCCCCCCCCCCC" RenderEditorText="True" Label="Reference Nbr." CommitChanges="True" LinkCommand="ViewPayment" PopupCommand="Cancel" />
                                        <px:PXGridColumn DataField="BlanketNbr" />
                                        <px:PXGridColumn DataField="CuryAdjdAmt" Label="Applied To Order" AllowNull="False" TextAlign="Right" CommitChanges="true" />
                                        <px:PXGridColumn DataField="CuryAdjdBilledAmt" Label="Transferred to Invoice" AllowNull="False" TextAlign="Right" />
                                        <px:PXGridColumn DataField="CuryAdjdTransferredToChildrenAmt" AllowNull="False" TextAlign="Right" />
                                        <px:PXGridColumn DataField="CuryDocBal" Label="Balance" AllowNull="False" AllowUpdate="False" TextAlign="Right" />
                                        <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="ARPayment__Status" Label="Status" RenderEditorText="True" />
                                            <px:PXGridColumn DataField="ExtRefNbr" />
                                            <px:PXGridColumn AllowUpdate="False" DataField="PaymentMethodID" DisplayFormat="&gt;aaaaaaaaaa" />
                                            <px:PXGridColumn DataField="CashAccountID" DisplayFormat="&gt;######" />
                                            <px:PXGridColumn DataField="CuryOrigDocAmt" />
                                        <px:PXGridColumn DataField="ARPayment__CuryID" Label="Currency ID" />
                                        <px:PXGridColumn DataField="ExternalTransaction__ProcStatus" />
                                        <px:PXGridColumn DataField="CanVoid" Type="CheckBox" />
                                        <px:PXGridColumn DataField="CanCapture" Type="CheckBox" />
                                        <px:PXGridColumn DataField="CanDeletePayment" Type="CheckBox" />
                                        <px:PXGridColumn DataField="CanDeleteRefund" Type="CheckBox" />
                                    </Columns>
                                </px:PXGridLevel>
                            </Levels>
                            <AutoSize Enabled="True" />
                            <ActionBar>
                                <CustomItems>
                                    <px:PXToolBarButton>
                                        <AutoCallBack Command="CreatePrepaymentInvoice" Target="ds">
                                            <Behavior CommitChanges="True" />
                                        </AutoCallBack>
                                        <PopupCommand Target="detgrid" Command="Refresh">
                                        </PopupCommand>
                                    </px:PXToolBarButton>
                                    <px:PXToolBarButton>
                                            <AutoCallBack Command="CreateDocumentPayment" Target="ds">
                                            <Behavior CommitChanges="True" />
                                        </AutoCallBack>
                                        <PopupCommand Target="detgrid" Command="Refresh">
                                        </PopupCommand>
                                    </px:PXToolBarButton>
                                    <px:PXToolBarButton>
                                        <AutoCallBack Command="CreateOrderPrepayment" Target="ds">
                                            <Behavior CommitChanges="True" />
                                        </AutoCallBack>
                                        <PopupCommand Target="detgrid" Command="Refresh">
                                        </PopupCommand>
                                    </px:PXToolBarButton>
                                    <px:PXToolBarButton DependOnGrid="detgrid" StateColumn="CanDeletePayment">
                                        <AutoCallBack Command="DeletePayment" Target="ds">
                                            <Behavior CommitChanges="True" />
                                        </AutoCallBack>
                                        <PopupCommand Target="detgrid" Command="Refresh" />
                                    </px:PXToolBarButton>
                                    <px:PXToolBarButton DependOnGrid="detgrid" StateColumn="CanCapture">
                                            <AutoCallBack Command="CaptureDocumentPayment" Target="ds">
                                            <Behavior CommitChanges="True" />
                                        </AutoCallBack>
                                        <PopupCommand Target="detgrid" Command="Refresh">
                                        </PopupCommand>
                                    </px:PXToolBarButton>
                                    <px:PXToolBarButton DependOnGrid="detgrid" StateColumn="CanVoid">
                                            <AutoCallBack Command="VoidDocumentPayment" Target="ds">
                                            <Behavior CommitChanges="True" />
                                        </AutoCallBack>
                                        <PopupCommand Target="detgrid" Command="Refresh">
                                        </PopupCommand>
                                    </px:PXToolBarButton>
                                    <px:PXToolBarButton>
                                            <AutoCallBack Command="ImportDocumentPayment" Target="ds">
                                            <Behavior CommitChanges="True" />
                                        </AutoCallBack>
                                        <PopupCommand Target="detgrid" Command="Refresh">
                                        </PopupCommand>
                                    </px:PXToolBarButton>
                                    <px:PXToolBarButton>
                                        <AutoCallBack Command="CreateDocumentRefund" Target="ds">
                                            <Behavior CommitChanges="True" />
                                        </AutoCallBack>
                                        <PopupCommand Target="detgrid" Command="Refresh">
                                        </PopupCommand>
                                    </px:PXToolBarButton>
                                    <px:PXToolBarButton DependOnGrid="detgrid" StateColumn="CanDeleteRefund">
                                        <AutoCallBack Command="DeleteRefund" Target="ds">
                                            <Behavior CommitChanges="True" />
                                        </AutoCallBack>
                                        <PopupCommand Target="detgrid" Command="Refresh" />
                                    </px:PXToolBarButton>
                                </CustomItems>
                            </ActionBar>
                        </px:PXGrid>
                    </div>
                    <px:PXFormView ID="formPT" runat="server" Style="position:absolute;top:0px;right:0px;" DataSourceID="ds" Width="230px" DataMember="CurrentDocument"
                        Caption="Payment Total" CaptionVisible="False" SkinID="Transparent">
                        <Template>
                            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                            <px:PXNumberEdit ID="edCuryUnreleasedPaymentAmt" runat="server" Enabled="False" DataField="CuryUnreleasedPaymentAmt"/>
                            <px:PXNumberEdit ID="edCuryCCAuthorizedAmt" runat="server" Enabled="False" DataField="CuryCCAuthorizedAmt"/>
                            <px:PXNumberEdit ID="edCuryPaidAmt" runat="server" Enabled="False" DataField="CuryPaidAmt"/>
                            <px:PXNumberEdit ID="edCuryPaymentTotal1" runat="server" Enabled="False" DataField="CuryPaymentTotal"/>
                            <px:PXNumberEdit ID="edCuryBilledPaymentTotal" runat="server" Enabled="False" DataField="CuryBilledPaymentTotal"/>
                            <px:PXNumberEdit ID="edCuryTransferredToChildrenPaymentTotal" runat="server" Enabled="False" DataField="CuryTransferredToChildrenPaymentTotal"/>
                            <px:PXLabel runat="server" ID="space" />
                            <px:PXNumberEdit ID="edCuryUnpaidBalance" runat="server" Enabled="False" DataField="CuryUnpaidBalance"/>
                            <px:PXNumberEdit ID="edCuryUnrefundedBalance" runat="server" Enabled="False" DataField="CuryUnrefundedBalance"/>
                            <px:PXNumberEdit ID="edCuryUnbilledOrderTotal1" runat="server" Enabled="False" DataField="CuryUnbilledOrderTotal"/>
                        </Template>
                    </px:PXFormView>
                </Template>
            </px:PXTabItem>
             <px:PXTabItem Text="Risks"  BindingContext="form"  RepaintOnDemand="false">
				<Template>
					<px:PXFormView ID="Risk" runat="server"  DataMember="CurrentDocument" RenderStyle="Simple" SkinID="Transparent">
						<Template>
							<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XS" />
							<px:PXTextEdit runat="server" ID="CstPXTextEdit1" DataField="RiskStatus" Enabled="False" />
					 </Template>
					</px:PXFormView>
					<px:PXGrid runat="server" ID="gridOrderRisks" DataSourceID="ds"  Width="100%" SkinID="Details"
						BorderStyle="None" SyncPosition="true" FilesIndicator="false" NoteIndicator="false">
						<Levels>
							<px:PXGridLevel DataMember="OrderRisks">
								<Columns>
                                    <px:PXGridColumn DataField="LineNbr" />
									<px:PXGridColumn DataField="Score"  CommitChanges="true" Width="100px" />
									<px:PXGridColumn DataField="Recommendation" Width="200px" />
									<px:PXGridColumn DataField="Message" Width="500px" />
								</Columns>
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" />
						<ActionBar PagerGroup="3" PagerOrder="2">
						</ActionBar>
						<Mode AllowAddNew="false" AllowDelete="False" AllowUpdate="false" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>

            <px:PXTabItem Text="Relations" LoadOnDemand="True" RepaintOnDemand="False">
	<Template>
		<px:PXGrid ID="grdRelations" runat="server" Height="400px" Width="100%" AllowPaging="True" SyncPosition="True" MatrixMode="True"
			ActionsPosition="Top" AllowSearch="true" DataSourceID="ds" SkinID="Details" AdjustPageSize = "Auto">
			<Levels>
				<px:PXGridLevel DataMember="Relations">
				<Columns>
					<px:PXGridColumn DataField="Role" CommitChanges="True" Width="130px" />
					<px:PXGridColumn DataField="IsPrimary" Type="CheckBox" TextAlign="Center" CommitChanges="True" />
					<px:PXGridColumn DataField="TargetType" CommitChanges="True" Width="120px" />
					<px:PXGridColumn DataField="TargetNoteID" DisplayMode="Text" LinkCommand="RelationsViewTargetDetails" CommitChanges="True" Width="200px" />
					<px:PXGridColumn DataField="Description" Width="250px" />
					<px:PXGridColumn DataField="Status" Width="150px" AllowFilter="False" AllowSort="False"/>
					<px:PXGridColumn DataField="OwnerID" Width="200px" />
					<px:PXGridColumn DataField="EntityID" AutoCallBack="true" LinkCommand="RelationsViewEntityDetails" CommitChanges="True" Width="200px" />
					<px:PXGridColumn DataField="Name" Width="200px" />
					<px:PXGridColumn DataField="ContactID" AutoCallBack="true" TextAlign="Left" TextField="ContactName" DisplayMode="Text" LinkCommand="RelationsViewContactDetails" Width="200px" />
					<px:PXGridColumn DataField="Email" Width="200px" />
					<px:PXGridColumn DataField="AddToCC" Type="CheckBox" TextAlign="Center" Width="90px" />

					<px:PXGridColumn DataField="DocumentDate" Width="200px" Visible="false" SyncVisible="false" />
					<px:PXGridColumn DataField="CreatedDateTime" Width="200px" Visible="false" SyncVisible="false" />
					<px:PXGridColumn DataField="CreatedByID" Width="200px" Visible="false" SyncVisible="false" />
					<px:PXGridColumn DataField="LastModifiedByID" Width="200px" Visible="false" SyncVisible="false" />
				</Columns>
				<RowTemplate>
					<px:PXSelector ID="edTargetNoteID" runat="server" DataField="TargetNoteID" FilterByAllFields="True" AutoRefresh="True" />
					<px:PXSelector ID="edRelEntityID" runat="server" DataField="EntityID" FilterByAllFields="True" AutoRefresh="True" />
					<px:PXSelector ID="edRelContactID" runat="server" DataField="ContactID" FilterByAllFields="True" AutoRefresh="True" />
					<px:PXSelector ID="edOwnerID" runat="server" DataField="OwnerID" AutoRefresh="True" />

					<px:PXDateTimeEdit ID="edCreatedDateTime" runat="server" DataField="CreatedDateTime_Date" />
				</RowTemplate>
				</px:PXGridLevel>
			</Levels>
			<Mode InitNewRow="True" ></Mode>
			<AutoSize Enabled="True" MinHeight="100" MinWidth="100" ></AutoSize>
		</px:PXGrid>
	</Template>
</px:PXTabItem>

            <px:PXTabItem Text="Totals">
                <Template>
                    <px:PXLayoutRule runat="server" StartGroup="True" />
                    <px:PXFormView ID="formFreightInfo" runat="server" Caption="Freight Info" DataMember="CurrentDocument" DataSourceID="ds" RenderStyle="Fieldset">
                        <Template>
                            <px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" />
                            <px:PXNumberEdit ID="edOrderWeight" runat="server" DataField="OrderWeight" Enabled="False" Size="XM" />
                            <px:PXNumberEdit ID="edOrderVolume" runat="server" DataField="OrderVolume" Enabled="False" Size="XM" />
                            <px:PXNumberEdit CommitChanges="True" ID="edPackageWeight" runat="server" DataField="PackageWeight" Enabled="False" Size="XM" />
                            <px:PXNumberEdit ID="edCuryFreightCost" runat="server" DataField="CuryFreightCost" CommitChanges="true" Size="XM" />
                            <px:PXButton ID="checkFreightRate" runat="server" Text="Check Freight Rate" CommandName="CalculateFreight" CommandSourceID="ds" />
                            <px:PXCheckBox ID="chkFreightCostIsValid" runat="server" DataField="FreightCostIsValid" />
							<px:PXCheckBox ID="chkOverrideFreightAmount" runat="server" DataField="OverrideFreightAmount" CommitChanges="True" />
							<px:PXDropDown ID="edFreightAmountSource" runat="server" DataField="FreightAmountSource" />
							<px:PXNumberEdit CommitChanges="True" ID="edCuryFreightAmt" runat="server" DataField="CuryFreightAmt" Size="XM" />
                            <px:PXNumberEdit CommitChanges="True" ID="edCuryPremiumFreightAmt" runat="server" DataField="CuryPremiumFreightAmt" Size="XM" />
                            <px:PXSelector CommitChanges="True" ID="edFreightTaxCategoryID" runat="server" DataField="FreightTaxCategoryID" DataSourceID="ds" Size="XM"/>
                        </Template>
                        <ContentStyle BackColor="Transparent" BorderStyle="None" />
                    </px:PXFormView>
                     <px:PXFormView ID="formVATTotals" runat="server" Caption="VAT Totals" DataMember="CurrentDocument" DataSourceID="ds" RenderStyle="Fieldset">
                        <Template>
                            <px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" />
                            <px:PXNumberEdit ID="edCuryVatExemptTotal" runat="server" DataField="CuryVatExemptTotal" Enabled="False" Size="XM" />
                            <px:PXNumberEdit ID="edCuryVatTaxableTotal" runat="server" DataField="CuryVatTaxableTotal" Enabled="False" Size="XM" />
                        </Template>
                        <ContentStyle BackColor="Transparent" BorderStyle="None" />
                    </px:PXFormView>
                    <px:PXLayoutRule runat="server" StartColumn="True" />
                    <px:PXFormView ID="formOrderTotals" runat="server" Caption="Order Totals" DataMember="CurrentDocument" DataSourceID="ds" RenderStyle="Fieldset">
                        <Template>
                            <px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" />
							<px:PXNumberEdit runat="server" Enabled="False" Size="XM" DataField="AMCuryEstimateTotal" ID="edAMCuryEstimateTotal" />
                            <px:PXNumberEdit ID="edCuryGoodsExtPriceTotal" runat="server" DataField="CuryGoodsExtPriceTotal" Enabled="False" Size="XM" />
                            <px:PXNumberEdit ID="edCuryMiscExtPriceTotal" runat="server" Enabled="False" DataField="CuryMiscExtPriceTotal" Size="XM" />
                            <px:PXNumberEdit ID="edCuryLineDiscTotal2" runat="server" DataField="CuryLineDiscTotal" Enabled="False" Size="XM" />
                            <px:PXNumberEdit ID="edCuryDiscTot2" runat="server" DataField="CuryDiscTot" Enabled ="false" Size="XM"/>
                            <px:PXNumberEdit ID="edCuryTaxTotal2" runat="server" DataField="CuryTaxTotal" Enabled="False" Size="XM"/>
                            <px:PXNumberEdit ID="edMarginPct" runat="server" DataField="MarginPct" Enabled="False" Size="XM" />
                            <px:PXNumberEdit ID="edCuryMarginAmt" runat="server" DataField="CuryMarginAmt" Enabled="False" Size="XM" />
                        </Template>
                        <ContentStyle BackColor="Transparent" BorderStyle="None" />
                    </px:PXFormView>
                    <px:PXFormView ID="formCalculatedAmounts" runat="server" Caption="Shipment and Invoice Info" DataMember="CurrentDocument" DataSourceID="ds" RenderStyle="Fieldset">
                        <Template>
                            <px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" />
							<px:PXNumberEdit runat="server" Enabled="False" Size="XM" DataField="AMEstimateQty" ID="edAMEstimateQty" />
                            <px:PXNumberEdit ID="edBlanketOpenQty" runat="server" Enabled="False" DataField="BlanketOpenQty" Size="XM" />
                            <px:PXNumberEdit ID="edOpenOrderQty" runat="server" Enabled="False" DataField="OpenOrderQty" Size="XM" />
                            <px:PXNumberEdit ID="edCuryOpenOrderTotal" runat="server" Enabled="False" DataField="CuryOpenOrderTotal" Size="XM" />
                            <px:PXNumberEdit ID="edUnbilledOrderQty" runat="server" Enabled="False" DataField="UnbilledOrderQty" Size="XM" />
                            <px:PXNumberEdit ID="edCuryUnbilledOrderTotal" runat="server" Enabled="False" DataField="CuryUnbilledOrderTotal" Size="XM" />
                            <px:PXNumberEdit ID="edCuryPaymentTotal" runat="server" Enabled="False" DataField="CuryPaymentTotal" Size="XM" />
                            <px:PXNumberEdit ID="edCuryUnpaidBalance1" runat="server" DataField="CuryUnpaidBalance" Enabled="False" Size="XM" />
                            <px:PXNumberEdit ID="edCuryUnrefundedBalance1" runat="server" DataField="CuryUnrefundedBalance" Enabled="False" Size="XM" />
                        </Template>
                        <ContentStyle BackColor="Transparent" BorderStyle="None" />
                    </px:PXFormView>
                </Template>
            </px:PXTabItem>
        </Items>
        <CallbackCommands>
            <Search CommitChanges="True" PostData="Page" />
            <Refresh CommitChanges="True" PostData="Page" />
            <Refresh CommitChanges="True" PostData="Page" />
            <Search CommitChanges="True" PostData="Page" />
        </CallbackCommands>
        <AutoSize Enabled="True" Container="Window" />
    </px:PXTab>
    <%-- PanelPOSupply / POLink --%>
    <px:PXSmartPanel ID="PanelPOSupply" runat="server" Width="960px" Height="360px" Caption="Purchasing Details" CaptionVisible="True"
        LoadOnDemand="True" ShowAfterLoad="True" AutoCallBack-Target="formSOLineDemand" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True"
        CallBackMode-PostData="Page" Key="SOLineDemand" TabIndex="3100">
        <px:PXFormView ID="formSOLineDemand" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="SOLineDemand"
            Caption="Purchasing Settings" CaptionVisible="False" SkinID="Transparent">
            <Parameters>
                <px:PXSyncGridParam ControlID="grid" />
            </Parameters>
            <Template>
                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                <px:PXDropDown CommitChanges="True" ID="edPnlPOSource" runat="server" DataField="POSource" />
                <px:PXSegmentMask CommitChanges="True" ID="edPnlVendorID" runat="server" DataField="VendorID" />
                <px:PXSegmentMask CommitChanges="True" ID="edPnlPOSiteID" runat="server" DataField="POSiteID" AutoRefresh="True" />
            </Template>
        </px:PXFormView>
        <px:PXGrid ID="gridPOSupply" runat="server" Height="360px"  Width="100%" DataSourceID="ds" AutoAdjustColumns="true" >
            <Parameters>
                <px:PXSyncGridParam  ControlID="grid" />
            </Parameters>
            <Levels>
                <px:PXGridLevel DataMember="SupplyPOLines">
                    <Columns>
                        <px:PXGridColumn DataField="Selected" Type="CheckBox" TextAlign="Center" CommitChanges="true"  />
                        <px:PXGridColumn DataField="OrderType" />
                        <px:PXGridColumn DataField="OrderNbr"  />
                        <px:PXGridColumn DataField="VendorRefNbr" />
                        <px:PXGridColumn DataField="LineType" AllowNull="False" />
                        <px:PXGridColumn DataField="InventoryID" DisplayFormat="&gt;AAAAAAAAAA" />
                        <px:PXGridColumn DataField="SubItemID" DisplayFormat="&gt;AA-A-A" />
                        <px:PXGridColumn DataField="VendorID" />
                        <px:PXGridColumn DataField="VendorID_Vendor_AcctName" />
                        <px:PXGridColumn DataField="PromisedDate" />
                        <px:PXGridColumn DataField="UOM" DisplayFormat="&gt;aaaaaa" />
                        <px:PXGridColumn DataField="OrderQty" TextAlign="Right" />
                        <px:PXGridColumn DataField="OpenQty" TextAlign="Right" />
                        <px:PXGridColumn DataField="TranDesc" />
                    </Columns>
                    <RowTemplate>
                        <px:PXSelector ID="edPOOrderNbr" runat="server" DataField="OrderNbr" AllowEdit="True" />
                        <px:PXSegmentMask ID="edVendorID" runat="server" DataField="VendorID" AllowEdit="True" />
                    </RowTemplate>
                    <Layout ColumnsMenu="False" />
                    <Mode AllowAddNew="false" AllowDelete="false" />
                </px:PXGridLevel>
            </Levels>
            <AutoSize Enabled="true" />
        </px:PXGrid>
        <px:PXGrid ID="gridPOSupplyLegacy" runat="server" Height="360px" Width="100%" DataSourceID="ds" AutoAdjustColumns="true">
            <Parameters>
                <px:PXSyncGridParam ControlID="grid" />
            </Parameters>
            <Levels>
                <px:PXGridLevel DataMember="posupply">
                    <Columns>
                        <px:PXGridColumn DataField="Selected" Type="CheckBox" TextAlign="Center" />
                        <px:PXGridColumn DataField="OrderType" />
                        <px:PXGridColumn DataField="OrderNbr" />
                        <px:PXGridColumn DataField="VendorRefNbr" />
                        <px:PXGridColumn AllowNull="False" DataField="LineType" />
                        <px:PXGridColumn DataField="InventoryID" DisplayFormat="&gt;AAAAAAAAAA" />
                        <px:PXGridColumn DataField="SubItemID" DisplayFormat="&gt;AA-A-A" />
                        <px:PXGridColumn DataField="VendorID" />
                        <px:PXGridColumn DataField="VendorID_Vendor_AcctName" />
                        <px:PXGridColumn DataField="PromisedDate" />
                        <px:PXGridColumn DataField="UOM" DisplayFormat="&gt;aaaaaa" />
                        <px:PXGridColumn DataField="OrderQty" TextAlign="Right" />
                        <px:PXGridColumn DataField="OpenQty" TextAlign="Right" />
                        <px:PXGridColumn DataField="TranDesc" />
                    </Columns>
                    <RowTemplate>
                        <px:PXSelector ID="edPOOrderNbrLegacy" runat="server" DataField="OrderNbr" AllowEdit="True" />
                    </RowTemplate>
                    <Layout ColumnsMenu="False" />
                    <Mode AllowAddNew="false" AllowDelete="false" />
                </px:PXGridLevel>
            </Levels>
            <AutoSize Enabled="true" />
        </px:PXGrid>
        <px:PXPanel ID="PXPanel3" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton7" runat="server" DialogResult="OK" Text="Save" />
            <px:PXButton ID="PXButton8" runat="server" DialogResult="Cancel" Text="Cancel" />
        </px:PXPanel>
    </px:PXSmartPanel>
    <%-- Bin/Lot/Serial Numbers --%>
    <px:PXSmartPanel ID="PanelLS" runat="server" Width="90%" Height="500px" Caption="Line Details" CaptionVisible="True" Key="SOOrderLineSplittingExtension_lsselect"
        AutoCallBack-Command="Refresh" AutoCallBack-Target="optform" AutoCallBack-ActiveBehavior="true" 
        AutoCallBack-Behavior-RepaintControls="None" AutoCallBack-Behavior-RepaintControlsIDs="ds,grid2" DesignView="Content" TabIndex="3200">
        <px:PXFormView ID="optform" runat="server" CaptionVisible="False" DataMember="SOOrderLineSplittingExtension_LotSerOptions" DataSourceID="ds" SkinID="Transparent"
            TabIndex="-3236" Width="100%">
            <Template>
                <px:PXLayoutRule runat="server" ControlSize="SM" LabelsWidth="SM" StartColumn="True" />
                <px:PXNumberEdit ID="edUnassignedQty" runat="server" DataField="UnassignedQty" Enabled="False" />
                <px:PXNumberEdit ID="edQty" runat="server" DataField="Qty">
                    <AutoCallBack>
                        <Behavior CommitChanges="True" />
                    </AutoCallBack>
                </px:PXNumberEdit>
                <px:PXLayoutRule runat="server" ControlSize="SM" LabelsWidth="SM" StartColumn="True" />
                <px:PXMaskEdit ID="edStartNumVal" runat="server" DataField="StartNumVal" />
                <px:PXButton ID="btnGenerate" runat="server" CommandName="SOOrderLineSplittingExtension_GenerateNumbers" CommandSourceID="ds" Height="20px"
                    Text="Generate" />
            </Template>
            <Parameters>
                <px:PXSyncGridParam ControlID="grid" />
            </Parameters>
        </px:PXFormView>
        <px:PXGrid ID="grid2" runat="server" AutoAdjustColumns="True" DataSourceID="ds" TabIndex="-3036" Width="100%" AllowFilter="true" SkinID="Details" SyncPosition="true">
            <Parameters>
                <px:PXSyncGridParam ControlID="grid" />
            </Parameters>
            <Levels>
                <px:PXGridLevel DataKeyNames="OrderType,OrderNbr,LineNbr,SplitLineNbr" DataMember="splits">
                    <RowTemplate>
                        <px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="M" StartColumn="True" />
                        <px:PXSegmentMask ID="edSubItemID2" runat="server" AutoRefresh="True" DataField="SubItemID" />
                        <px:PXSegmentMask ID="edSiteID2" runat="server" AutoRefresh="True" DataField="SiteID" />
                        <px:PXSegmentMask ID="edLocationID2" runat="server" AutoRefresh="True" DataField="LocationID">
                            <Parameters>
                                <px:PXControlParam ControlID="grid2" Name="SOLineSplit.siteID" PropertyName="DataValues[&quot;SiteID&quot;]" Type="String" />
                                <px:PXControlParam ControlID="grid2" Name="SOLineSplit.inventoryID" PropertyName="DataValues[&quot;InventoryID&quot;]" Type="String" />
                                <px:PXControlParam ControlID="grid2" Name="SOLineSplit.subItemID" PropertyName="DataValues[&quot;SubItemID&quot;]" Type="String" />
                            </Parameters>
                        </px:PXSegmentMask>
                        <px:PXNumberEdit ID="edQty2" runat="server" DataField="Qty" CommitChanges="true" />
                        <px:PXSelector ID="edUOM2" runat="server" AutoRefresh="True" DataField="UOM">
                            <Parameters>
                                <px:PXControlParam ControlID="grid" Name="SOLine.inventoryID" PropertyName="DataValues[&quot;InventoryID&quot;]" Type="String" />
                            </Parameters>
                        </px:PXSelector>
                        <px:PXSelector ID="edLotSerialNbr2" runat="server" AutoRefresh="True" DataField="LotSerialNbr">
                            <Parameters>
                                <px:PXControlParam ControlID="grid2" Name="SOLineSplit.inventoryID" PropertyName="DataValues[&quot;InventoryID&quot;]" Type="String" />
                                <px:PXControlParam ControlID="grid2" Name="SOLineSplit.subItemID" PropertyName="DataValues[&quot;SubItemID&quot;]" Type="String" />
                                <px:PXControlParam ControlID="grid2" Name="SOLineSplit.locationID" PropertyName="DataValues[&quot;LocationID&quot;]" Type="String" />
                            </Parameters>
                        </px:PXSelector>
                        <px:PXDateTimeEdit ID="edExpireDate2" runat="server" DataField="ExpireDate" />
                    </RowTemplate>
                    <Columns>
                        <px:PXGridColumn DataField="SplitLineNbr" TextAlign="Right" />
                        <px:PXGridColumn DataField="ParentSplitLineNbr" TextAlign="Right" />
                        <px:PXGridColumn DataField="InventoryID" />
                        <px:PXGridColumn DataField="SubItemID" />
                        <px:PXGridColumn DataField="SchedOrderDate" />
                        <px:PXGridColumn DataField="SchedShipDate" />
                        <px:PXGridColumn DataField="CustomerOrderNbr" />
                        <px:PXGridColumn DataField="ShipDate" />
                        <px:PXGridColumn AllowNull="False" AllowShowHide="Server" AutoCallBack="True" DataField="IsAllocated" TextAlign="Center"
                            Type="CheckBox" />
                        <px:PXGridColumn DataField="SiteID" AutoCallBack="True" />
                        <px:PXGridColumn AllowNull="False" AllowShowHide="Server" DataField="Completed" TextAlign="Center" Type="CheckBox" />
                        <px:PXGridColumn AllowShowHide="Server" DataField="LocationID" CommitChanges="true" />
                        <px:PXGridColumn DataField="LotSerialNbr" AutoCallBack="True" />
                        <px:PXGridColumn DataField="Qty" TextAlign="Right" CommitChanges="true" />
                        <px:PXGridColumn DataField="QtyOnOrders" TextAlign="Right" />
                        <px:PXGridColumn DataField="ShippedQty" TextAlign="Right" />
                        <px:PXGridColumn DataField="ReceivedQty" TextAlign="Right" />
                        <px:PXGridColumn DataField="BlanketOpenQty" />
                        <px:PXGridColumn DataField="UOM" />
                        <px:PXGridColumn DataField="ExpireDate" />
                        <px:PXGridColumn AllowNull="False" DataField="POCreate" TextAlign="Center" Type="CheckBox" CommitChanges="true" />
                        <px:PXGridColumn DataField="POCreateDate" CommitChanges="true" />
                        <px:PXGridColumn DataField="RefNoteID" RenderEditorText="True" LinkCommand="SOLineSplit$RefNoteID$Link" />
                    </Columns>
                    <Layout FormViewHeight="" />
                </px:PXGridLevel>
            </Levels>
            <AutoSize Enabled="True" />
            <Mode InitNewRow="True" />
        </px:PXGrid>
        <px:PXPanel ID="PXPanel4" runat="server" SkinID="Buttons">
            <px:PXButton ID="btnSave" runat="server" DialogResult="OK" Text="OK" />
        </px:PXPanel>
    </px:PXSmartPanel>
    <%-- Specify Shipment Parameters --%>
    <px:PXSmartPanel ID="pnlCreateShipment" runat="server" Caption="Specify Shipment Parameters"
        CaptionVisible="true" DesignView="Hidden" LoadOnDemand="true" Key="soparamfilter" CreateOnDemand="false" AutoCallBack-Enabled="true"
        AutoCallBack-Target="formCreateShipment" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True" CallBackMode-PostData="Page"
        AcceptButtonID="btnOK">
        <px:PXFormView ID="formCreateShipment" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" CaptionVisible="False"
            DataMember="soparamfilter">
            <ContentStyle BackColor="Transparent" BorderStyle="None" />
            <Template>
                <px:PXLayoutRule ID="PXLayoutRule44" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                <px:PXDateTimeEdit ID="edShipDate" runat="server" DataField="ShipDate" />
                <px:PXSelector ID="edSiteID" runat="server" DataField="SiteID" AutoRefresh="true" ValueField="INSite__SiteCD" HintField="INSite__descr">
                    <GridProperties FastFilterFields="Descr">
                    </GridProperties>
                </px:PXSelector>
            </Template>
        </px:PXFormView>
        <px:PXPanel ID="PXPanel5" runat="server" SkinID="Buttons">
            <px:PXButton ID="btnOK" runat="server" DialogResult="OK" Text="OK">
                <AutoCallBack Target="formCreateShipment" Command="Save" />
            </px:PXButton>
        </px:PXPanel>
    </px:PXSmartPanel>
    <%-- Specify Parameters for Quick Process--%>
    <px:PXSmartPanel ID="PXSmartPanel1" runat="server" Caption="Process Order" ShowAfterLoad="true"
        CaptionVisible="true" DesignView="Hidden" Key="QuickProcessParameters"
        AutoCallBack-Enabled="true" AutoCallBack-Target="fromQuickProcess" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True" CallBackMode-PostData="Page"
        AcceptButtonID="btnOK">
        <px:PXFormView ID="fromQuickProcess" runat="server" DataSourceID="ds" Style="z-index: 100" Width="600px" CaptionVisible="False" DataMember="QuickProcessParameters" AllowCollapse="False">
            <ContentStyle BackColor="Transparent" BorderStyle="None" />
            <Template>
	            <px:PXLayoutRule runat="server" StartGroup="True" ColumnSpan="2" LabelsWidth="100px" />
                <px:PXSelector ID="edSiteID" runat="server" DataField="SiteID" AutoRefresh="true" ValueField="INSite__SiteCD" HintField="INSite__descr" CommitChanges="True" Style="margin-bottom: 20px">
                    <GridProperties FastFilterFields="Descr">
                    </GridProperties>
                </px:PXSelector>
	            <px:PXLayoutRule runat="server" StartRow="True" StartGroup="True" ColumnWidth="260"/>
                <px:PXGroupBox ID="gbShipDate" runat="server" Caption="Shipment Date" DataField="ShipDateMode" CommitChanges="True" RenderStyle="Fieldset">
                    <Template>
                        <px:PXRadioButton ID="gbShipDate_Today" runat="server" Text="Today" Value="0" GroupName="gbShipDate"/>
                        <px:PXRadioButton ID="gbShipDate_Tommorow" runat="server" Text="Tomorrow" Value="1" GroupName="gbShipDate" />
                        <px:PXLayoutRule runat="server" StartGroup="False" StartColumn="False" SuppressLabel="False" Merge="True" />
                        <px:PXRadioButton ID="gbShipDate_Custom" runat="server" Text="Custom" Value="2" GroupName="gbShipDate" />
                        <px:PXDateTimeEdit ID="edShipDate" runat="server" DataField="ShipDate" CommitChanges="True" SuppressLabel="True" />
                    </Template>
                    <ContentLayout Layout="Stack" />
                </px:PXGroupBox>
	            <px:PXLayoutRule runat="server" GroupCaption="Printing settings" StartColumn="True" LabelsWidth="XS" ControlSize="SM" ColumnWidth="260" StartGroup="True" />
	            <px:PXCheckBox ID="edPrintWithDeviceHub" runat="server" DataField="PrintWithDeviceHub" CommitChanges="True" AlignLeft="true"/>
	            <px:PXCheckBox ID="PXDefinePrinterAutomatically" runat="server" DataField="DefinePrinterManually" CommitChanges="True" AlignLeft="true"/>
	            <px:PXSelector ID="edPrinterID" runat="server" DataField="PrinterID" CommitChanges="True"/>
				<px:PXTextEdit CommitChanges="true" ID="edNumberOfCopies" runat="server" DataField="NumberOfCopies" />

	            <px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Availability" ColumnSpan="2" StartRow="True" />
	            <px:PXLayoutRule runat="server" Merge="True"/>
	            <px:PXCheckBox ID="edGreenStatus" runat="server" DataField="GreenStatus" RenderStyle="Button" AlignLeft="True" Enabled="False">
		            <UncheckImages Normal="main@Success" />
		            <CheckImages Normal="main@Success" />
	            </px:PXCheckBox>
	            <px:PXCheckBox ID="edYellowStatus" runat="server" DataField="YellowStatus" RenderStyle="Button" AlignLeft="True" Enabled="False">
		            <UncheckImages Normal="control@Warning" />
		            <CheckImages Normal="control@Warning" />
	            </px:PXCheckBox>
	            <px:PXCheckBox ID="edRedStatus" runat="server" DataField="RedStatus" RenderStyle="Button" AlignLeft="True" Enabled="False">
		            <UncheckImages Normal="main@Fail" />
		            <CheckImages Normal="main@Fail" />
	            </px:PXCheckBox>
	            <px:PXTextEdit ID="edAvailMsg" runat="server" DataField="AvailabilityMessage" TextMode="MultiLine" SuppressLabel="True" Height="60" Width="500" TextAlign="Justify" Style="border-width:0; resize:none"/>
	            <px:PXLayoutRule runat="server" EndGroup="True" />
	            <px:PXLayoutRule runat="server" EndGroup="True" />

                <px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Shipping" StartRow="True" SuppressLabel="True" ColumnWidth="260" />
                <px:PXCheckBox ID="edCreateShipment" runat="server" DataField="CreateShipment" CommitChanges="True" />
                <px:PXCheckBox ID="edPrintPickList" runat="server" DataField="PrintPickList" CommitChanges="True"/>
                <px:PXCheckBox ID="edConfirmShipment" runat="server" DataField="ConfirmShipment" CommitChanges="True"/>
                <px:PXCheckBox ID="edPrintLabels" runat="server" DataField="PrintLabels" CommitChanges="True"/>
                <px:PXCheckBox ID="edPrintShipmentConfirmation" runat="server" DataField="PrintConfirmation" CommitChanges="True"/>
                <px:PXCheckBox ID="edUpdateIN" runat="server" DataField="UpdateIN" CommitChanges="True"/>
	            <px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Invoicing" StartColumn="True" SuppressLabel="True" ColumnWidth="260" />
	            <px:PXCheckBox ID="edPrepareInvoiceFromShipment" runat="server" DataField="PrepareInvoiceFromShipment" CommitChanges="True"/>
	            <px:PXCheckBox ID="edPrepareInvoice" runat="server" DataField="PrepareInvoice" CommitChanges="True"/>
	            <px:PXCheckBox ID="edPrintInvoice" runat="server" DataField="PrintInvoice" CommitChanges="True"/>
	            <px:PXCheckBox ID="edEmailInvoice" runat="server" DataField="EmailInvoice" CommitChanges="True"/>
	            <px:PXCheckBox ID="edReleaseInvoice" runat="server" DataField="ReleaseInvoice" CommitChanges="True"/>
            </Template>
        </px:PXFormView>
        <px:PXPanel ID="PXPanel9" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXProcess" runat="server" DialogResult="OK" Text="OK" CommandName="QuickProcessOk" CommandSourceID="ds" SyncVisible="False"/>
        </px:PXPanel>
    </px:PXSmartPanel>
    <%-- Add Blanket Line --%>
    <px:PXSmartPanel ID="PanelAddBlanketLine" runat="server" Width="873px" Key="BlanketSplits" Caption="Add Blanket Sales Order Line" CaptionVisible="True"
        LoadOnDemand="True" CallBackMode-CommitChanges="True" CallBackMode-PostData="Page" AutoCallBack-Command="Refresh" AutoCallBack-Target="gridBlanketSplits" Height="400px">
        <px:PXGrid ID="gridBlanketSplits" runat="server" Width="100%" DataSourceID="ds" BatchUpdate="True" Style="height: 250px;"
            AutoAdjustColumns="True" SkinID="Inquire" FilesIndicator="false" NoteIndicator="false" SyncPosition="true">
            <Levels>
                <px:PXGridLevel DataMember="BlanketSplits">
                    <Columns>
                        <px:PXGridColumn AllowCheckAll="True" AllowNull="False" DataField="Selected" TextAlign="Center" Type="CheckBox" CommitChanges="true" />
                        <px:PXGridColumn DataField="OrderType" />
                        <px:PXGridColumn DataField="OrderNbr" />
                        <px:PXGridColumn DataField="SchedOrderDate" />
                        <px:PXGridColumn DataField="InventoryID" />
                        <px:PXGridColumn DataField="SubItemID" />
                        <px:PXGridColumn DataField="TranDesc" />
                        <px:PXGridColumn DataField="SiteID" />
                        <px:PXGridColumn DataField="CustomerOrderNbr" />
                        <px:PXGridColumn DataField="UOM" />
                        <px:PXGridColumn DataField="BlanketOpenQty" DataType="Decimal" TextAlign="Right" />
                        <px:PXGridColumn DataField="CustomerLocationID" />
                        <px:PXGridColumn DataField="TaxZoneID" />
                    </Columns>
                    <Layout ColumnsMenu="True" FormViewHeight="" />
                </px:PXGridLevel>
            </Levels>
            <AutoSize Enabled="True" />
            <Mode AllowAddNew="False" AllowDelete="False" />
        </px:PXGrid>
        <px:PXPanel ID="PXPanel10" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton11" runat="server" CommandName="AddBlanketLineOK" CommandSourceID="ds" Text="Add" SyncVisible="false" />
            <px:PXButton ID="PXButton12" runat="server" DialogResult="OK" Text="Add &amp; Close" />
            <px:PXButton ID="PXButton13" runat="server" DialogResult="Cancel" Text="Cancel" />
        </px:PXPanel>
    </px:PXSmartPanel>
    <%-- Override Tax Zone Parameters --%>
    <px:PXSmartPanel ID="PanelOverrideTaxZone" runat="server" Caption="Override Tax Zone"
        CaptionVisible="true" DesignView="Hidden" LoadOnDemand="true" Key="BlanketTaxZoneOverrideFilter" CreateOnDemand="false" AutoCallBack-Enabled="true"
        AutoCallBack-Target="formOverrideTaxZone" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True" CallBackMode-PostData="Page"
        AcceptButtonID="btnOverrideOK" CloseButtonDialogResult="No">
        <px:PXFormView ID="formOverrideTaxZone" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" CaptionVisible="False"
            DataMember="BlanketTaxZoneOverrideFilter">
            <ContentStyle BackColor="Transparent" BorderStyle="None" />
            <Template>
                <px:PXLayoutRule ID="PXLayoutRule44" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
                <px:PXSelector ID="edTaxZoneID" runat="server" DataField="TaxZoneID" AutoRefresh="true" />
                <px:PXLabel runat="server" ID="lblRecalcTaxes" Text="Taxes will be recalculated based on the overridden tax zone. Do you want to continue?" Height="40px" Width="350px"></px:PXLabel>
            </Template>
        </px:PXFormView>
        <px:PXPanel ID="PXPanel11" runat="server" SkinID="Buttons">
            <px:PXButton ID="btnOverrideOK" runat="server" DialogResult="Yes" Text="Yes" />
            <px:PXButton ID="btnOverrideCancel" runat="server" DialogResult="No" Text="No" />
        </px:PXPanel>
    </px:PXSmartPanel>
    <%-- Copy To --%>
    <px:PXSmartPanel ID="panelCopyTo" runat="server" Caption="Copy To" CaptionVisible="true" LoadOnDemand="true" Key="copyparamfilter"
        AutoCallBack-Enabled="true" AutoCallBack-Target="formCopyTo" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True"
        CallBackMode-PostData="Page">
        <div style="padding: 5px">
            <px:PXFormView ID="formCopyTo" runat="server" DataSourceID="ds" CaptionVisible="False" DataMember="copyparamfilter">
                <Activity Height="" HighlightColor="" SelectedColor="" Width="" />
                <ContentStyle BackColor="Transparent" BorderStyle="None" />
                <Template>
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="SM" />
                    <px:PXSelector CommitChanges="True" ID="edOrderType" runat="server" DataField="OrderType"
                        Text="SO" DataSourceID="ds" />
                    <px:PXMaskEdit CommitChanges="True" ID="edOrderNbr" runat="server" DataField="OrderNbr" InputMask="&gt;CCCCCCCCCCCCCCC" />
                    <px:PXCheckBox CommitChanges="True" ID="chkRecalcUnitPrices" runat="server" DataField="RecalcUnitPrices" />
                    <px:PXCheckBox CommitChanges="True" ID="chkOverrideManualPrices" runat="server" DataField="OverrideManualPrices" Style="margin-left: 25px" />
                    <px:PXCheckBox CommitChanges="True" ID="chkRecalcDiscounts" runat="server" DataField="RecalcDiscounts" />
                    <px:PXCheckBox CommitChanges="True" ID="chkOverrideManualDiscounts" runat="server" DataField="OverrideManualDiscounts" Style="margin-left: 25px" />
                    <px:PXCheckBox runat="server" CommitChanges="True" DataField="AMIncludeEstimate" ID="edAMIncludeEstimate" />
					<px:PXCheckBox runat="server" CommitChanges="True" DataField="CopyConfigurations" ID="chkCopyConfigurations" />
                </Template>
            </px:PXFormView>
        </div>
        <px:PXPanel ID="PXPanel7" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton9" runat="server" DialogResult="OK" Text="OK" CommandName="CheckCopyParams" CommandSourceID="ds" />
        </px:PXPanel>
    </px:PXSmartPanel>
    <%-- Carrier Rates --%>
    <px:PXSmartPanel ID="PanelCarrierRates" Width="900" runat="server" Caption="Shop For Rates" CaptionVisible="True" LoadOnDemand="True" ShowAfterLoad="True" Key="DocumentProperties"
        AutoCallBack-Target="formCarrierRates" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True" CallBackMode-PostData="Page"
        AcceptButtonID="PXButtonRatesOK" AllowResize="True" ShowMaximizeButton="True">
        <px:PXFormView ID="formCarrierRates" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="DocumentProperties"
            Caption="Services Settings" CaptionVisible="False" SkinID="Transparent">
            <Template>
                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                <px:PXNumberEdit ID="edOrderWeight" runat="server" DataField="OrderWeight" Enabled="False" />
                <px:PXLayoutRule ID="PXLayoutRule2" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                <px:PXNumberEdit ID="PXNumberEdit1" runat="server" DataField="PackageWeight" Enabled="False" />
            </Template>
        </px:PXFormView>
        <px:PXGrid ID="gridRates" runat="server" Width="100%" DataSourceID="ds" Style="border-width: 1px 1px; left: 0px; top: 0px;"
            AutoAdjustColumns="true" Caption="Carrier Rates" Height="120px" AllowFilter="False" SkinID="Details" CaptionVisible="True" AllowPaging="False">
            <Mode AllowAddNew="False" AllowDelete="False" AllowFormEdit="False" />
            <ActionBar Position="Top" PagerVisible="False" CustomItemsGroup="1" ActionsVisible="True">
                <CustomItems>
                    <px:PXToolBarButton Text="Get Rates">
                        <AutoCallBack Command="RefreshRates" Target="ds" />
                    </px:PXToolBarButton>
                </CustomItems>
            </ActionBar>
            <Levels>
                <px:PXGridLevel DataMember="CarrierRates">
                    <Columns>
                        <px:PXGridColumn DataField="Selected" Type="CheckBox" AutoCallBack="true" TextAlign="Center" />
                        <px:PXGridColumn DataField="Method" Label="Code" />
                        <px:PXGridColumn DataField="Description" Label="Description" />
                        <px:PXGridColumn AllowUpdate="False" DataField="Amount" />
                        <px:PXGridColumn AllowUpdate="False" DataField="DaysInTransit" Label="Days in Transit" />
                        <px:PXGridColumn AllowUpdate="False" DataField="DeliveryDate" Label="Delivery Date" />
                    </Columns>
                </px:PXGridLevel>
            </Levels>
        </px:PXGrid>
        <px:PXFormView ID="PXFormView1" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="DocumentProperties"
            Caption="Services Settings" CaptionVisible="False" SkinID="Transparent">
            <Template>
                <px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                <px:PXCheckBox ID="edIsManualPackage" runat="server" DataField="IsManualPackage" AlignLeft="true" CommitChanges="true" />
            </Template>
        </px:PXFormView>
        <px:PXGrid ID="gridPackages" runat="server" Width="100%" DataSourceID="ds" Style="border-width: 1px 1px; left: 0px; top: 0px;"
            Caption="Packages" SkinID="Details" Height="100px" CaptionVisible="True" AllowPaging="False">
            <ActionBar Position="Top">
                <CustomItems>
                    <px:PXToolBarButton Text="Recalculate Packages">
                        <AutoCallBack Command="RecalculatePackages" Target="ds" />
                    </px:PXToolBarButton>
                </CustomItems>
            </ActionBar>
            <Levels>
                <px:PXGridLevel DataMember="Packages">
                    <Columns>
                        <px:PXGridColumn DataField="BoxID" CommitChanges="True" />
                        <px:PXGridColumn DataField="Description" Label="Description" />
                        <px:PXGridColumn DataField="SiteID" />
                        <px:PXGridColumn DataField="AllowOverrideDimension" TextAlign="Center" Type="CheckBox" />
                        <px:PXGridColumn DataField="Length" />
                        <px:PXGridColumn DataField="Width" />
                        <px:PXGridColumn DataField="Height" />
                        <px:PXGridColumn DataField="LinearUOM" />
                        <px:PXGridColumn DataField="WeightUOM" />
                        <px:PXGridColumn DataField="Weight" />
                        <px:PXGridColumn DataField="BoxWeight" />
                        <px:PXGridColumn DataField="GrossWeight" />
                        <px:PXGridColumn DataField="DeclaredValue" />
                        <px:PXGridColumn DataField="COD" Type="CheckBox" />
                        <px:PXGridColumn DataField="StampsAddOns" Type="DropDownList" />
                    </Columns>
                    <RowTemplate>
                        <px:PXDropDown runat="server" ID="edStampsAddOns" DataField="StampsAddOns" AllowMultiSelect="True" />
                    </RowTemplate>
                </px:PXGridLevel>
            </Levels>
        </px:PXGrid>
        <px:PXPanel ID="PXPanel8" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButtonRatesOK" runat="server" DialogResult="OK" Text="OK" />
        </px:PXPanel>
    </px:PXSmartPanel>
    <%-- Recalculate Prices and Discounts --%>
    <px:PXSmartPanel ID="PanelRecalcDiscounts" runat="server" Caption="Recalculate Prices" CaptionVisible="True" LoadOnDemand="True" Key="recalcdiscountsfilter" AutoCallBack-Target="formRecalcDiscounts" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True"
        CallBackMode-PostData="Page" TabIndex="5500">
            <px:PXFormView ID="formRecalcDiscounts" runat="server" DataSourceID="ds" CaptionVisible="False" DataMember="recalcdiscountsfilter">
                <Activity Height="" HighlightColor="" SelectedColor="" Width="" />
                <ContentStyle BackColor="Transparent" BorderStyle="None" />
                <Template>
                    <px:PXLayoutRule ID="PXLayoutRule3" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="SM" />
                    <px:PXDropDown ID="edRecalcTerget" runat="server" DataField="RecalcTarget" CommitChanges="true" />
                    <px:PXCheckBox CommitChanges="True" ID="chkRecalcUnitPrices" runat="server" DataField="RecalcUnitPrices" />
                    <px:PXCheckBox CommitChanges="True" ID="chkOverrideManualPrices" runat="server" DataField="OverrideManualPrices" Style="margin-left: 25px" />
                    <px:PXCheckBox CommitChanges="True" ID="chkRecalcDiscounts" runat="server" DataField="RecalcDiscounts" />
                    <px:PXCheckBox CommitChanges="True" ID="chkOverrideManualDiscounts" runat="server" DataField="OverrideManualDiscounts" Style="margin-left: 25px" />
                    <px:PXCheckBox CommitChanges="True" ID="chkOverrideManualDocGroupDiscounts" runat="server" DataField="OverrideManualDocGroupDiscounts" Style="margin-left: 25px" />
                    <px:PXCheckBox CommitChanges="True" ID="chkCalcDiscountsOnLinesWithDisabledAutomaticDiscounts" runat="server" DataField="CalcDiscountsOnLinesWithDisabledAutomaticDiscounts" Style="margin-left: 25px" />
                </Template>
            </px:PXFormView>
        <px:PXPanel ID="PXPanel2" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton10" runat="server" DialogResult="OK" Text="OK" CommandName="RecalcOk" CommandSourceID="ds" />
        </px:PXPanel>
    </px:PXSmartPanel>
    <px:PXSmartPanel ID="panelCreateServiceOrder" runat="server" Style="z-index: 108; left: 27px; position: absolute; top: 99px;" Caption="Create Service Order" Width="450px" Height="150px" AutoRepaint="true"
        CaptionVisible="true" DesignView="Hidden" LoadOnDemand="true" Key="CreateServiceOrderFilter" AutoCallBack-Enabled="true" AutoCallBack-Command="Refresh"
        CallBackMode-CommitChanges="True" CallBackMode-PostData="Page">
        <px:PXFormView ID="formCreateServiceOrder" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" CaptionVisible="False" DataMember="CreateServiceOrderFilter">
            <ContentStyle BackColor="Transparent" BorderStyle="None" />
            <Template>
                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="SL" />
                <px:PXSelector ID="edSrvOrdType" runat="server" AllowNull="False" DataField="SrvOrdType" DisplayMode="Text" CommitChanges="True" AutoRefresh="True" AllowEdit="True" />
                <px:PXSelector ID="edAssignedEmpID" runat="server" AllowNull="False" DataField="AssignedEmpID" DisplayMode="Text" CommitChanges="True" AutoRefresh="True" AllowEdit="True"/>
                <px:PXLayoutRule runat="server" Merge="True">
                </px:PXLayoutRule>
                <px:PXDateTimeEdit ID="edSLAETA_Date" runat="server" DataField="SLAETA_Date">
                </px:PXDateTimeEdit>
                <px:PXDateTimeEdit ID="edSLAETA_Time" runat="server" DataField="SLAETA_Time"
                    TimeMode="True" SuppressLabel = "True">
                </px:PXDateTimeEdit>
            </Template>
        </px:PXFormView>

        <div style="padding: 5px; text-align: right;">
                <px:PXButton ID="btnSave2" runat="server" CommitChanges="True" DialogResult="OK" Text="OK" Height="20"/>
                <px:PXButton ID="btnCancel2" runat="server" DialogResult="Cancel" Text="Cancel" Height="20" />
        </div>
    </px:PXSmartPanel>
    <px:PXSmartPanel ID="panelReason" runat="server" Caption="Enter Reason" CaptionVisible="true" LoadOnDemand="true" Key="ReasonApproveRejectParams"
	AutoCallBack-Enabled="true" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True" Width="600px"
	CallBackMode-PostData="Page" AcceptButtonID="btnReasonOk" CancelButtonID="btnReasonCancel" AllowResize="False"  AutoRepaint="true">
	<px:PXFormView ID="PXFormViewPanelReason" runat="server" DataSourceID="ds" CaptionVisible="False" DataMember="ReasonApproveRejectParams">
		<ContentStyle BackColor="Transparent" BorderStyle="None" Width="100%" Height="100%"  CssClass="" /> 
		<Template>
			<px:PXLayoutRule ID="PXLayoutRulePanelReason" runat="server" StartColumn="True" />
			<px:PXPanel ID="PXPanelReason" runat="server" RenderStyle="Simple" >
				<px:PXLayoutRule ID="PXLayoutRuleReason" runat="server" StartColumn="True" SuppressLabel="True" />
				<px:PXTextEdit ID="edReason" runat="server" DataField="Reason" TextMode="MultiLine" LabelWidth="0" Height="200px" Width="600px" CommitChanges="True" />
			</px:PXPanel>
			<px:PXPanel ID="PXPanelReasonButtons" runat="server" SkinID="Buttons">
				<px:PXButton ID="btnReasonOk" runat="server" Text="OK" DialogResult="Yes" CommandSourceID="ds" />
				<px:PXButton ID="btnReasonCancel" runat="server" Text="Cancel" DialogResult="No" CommandSourceID="ds" />
			</px:PXPanel>
		</Template>
	</px:PXFormView>
</px:PXSmartPanel>
	<px:PXSmartPanel ID="InventoryMatrixEntrySmartPanel" runat="server" Caption="Add Matrix Item: Table View" CaptionVisible="True" LoadOnDemand="True" Key="Header"
	AutoCallBack-Target="MatrixEntryFormView" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True" CallBackMode-PostData="Page" Width="100%">

	<px:PXLayoutRule runat="server" StartColumn="True" />

    <px:PXFormView ID="MatrixEntryFormView" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Header"
        CaptionVisible="False" SkinID="Transparent">
        <Template>
            <px:PXLayoutRule ID="InventoryMatrixEntrySmartPanelFormLayout" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
			<px:PXSelector ID="edEntryTemplateItemID" runat="server" DataField="TemplateItemID" CommitChanges="true" />
			<px:PXTextEdit ID="edEntryDescription" runat="server" DataField="Description" />
			<px:PXSelector ID="selEntrySiteID" runat="server" DataField="SiteID" CommitChanges="true" AutoRefresh="true" />
        </Template>
    </px:PXFormView>
    
	<px:PXLayoutRule runat="server" StartColumn="True" />

    <px:PXGrid ID="MatrixItems" runat="server" DataSourceID="ds" SkinID="DetailsInTab" SyncPosition="True" Height="400px"
		RepaintColumns="True" OnAfterSyncState="MatrixItems_AfterSyncState" OnInit="MatrixItems_OnInit">
        <Levels>
			<px:PXGridLevel DataMember="MatrixItems">
				<RowTemplate>
					<px:PXSegmentMask ID="matrixItemsInventoryCD" runat="server" DataField="InventoryCD" AllowEdit="True" />
					<px:PXSegmentMask ID="matrixItemsDfltSiteID" runat="server" DataField="DfltSiteID" AllowEdit="True" />
					<px:PXSegmentMask ID="matrixItemsItemClassID" runat="server" DataField="ItemClassID" AllowEdit="True" />
					<px:PXSelector ID="matrixItemsTaxCategoryID" runat="server" DataField="TaxCategoryID" AllowEdit="True" />
                </RowTemplate>
                <Columns>
					<px:PXGridColumn DataField="AttributeValue0" CommitChanges="true" />
					<px:PXGridColumn DataField="UOM" />
					<px:PXGridColumn DataField="Qty" TextAlign="Right" />
					<px:PXGridColumn DataField="InventoryCD" DisplayFormat="&gt;CCCCC-CCCCCCCCCCCCCCC" />
					<px:PXGridColumn DataField="Descr" />
					<px:PXGridColumn DataField="New" Type="CheckBox" AllowShowHide="False" />
					<px:PXGridColumn DataField="StkItem" Type="CheckBox" />
					<px:PXGridColumn DataField="BasePrice" AllowShowHide="False" />
					<px:PXGridColumn DataField="ItemClassID" />
					<px:PXGridColumn DataField="TaxCategoryID" />
                </Columns>
			</px:PXGridLevel>
        </Levels>
		<Mode InitNewRow="True" />
    </px:PXGrid>

    <px:PXPanel runat="server" ID="InventoryMatrixEntrySmartButtons" SkinID="Buttons">
		<px:PXButton ID="InventoryMatrixEntrySmartPanelButtonToLookup" runat="server" DialogResult="Yes" Text="Open Matrix View" Height="20" />
		<px:PXButton ID="InventoryMatrixEntrySmartPanelButtonOK" runat="server" CommitChanges="True" DialogResult="OK" Text="Add and close" Height="20">
			<AutoCallBack Command="Commit" Target="ds">
				<Behavior CommitChanges="True" />
			</AutoCallBack>
		</px:PXButton>
		<px:PXButton ID="InventoryMatrixEntrySmartPanelButtonCancel" runat="server" DialogResult="Cancel" Text="Cancel" Height="20" />
	</px:PXPanel>
</px:PXSmartPanel>

<px:PXSmartPanel ID="InventoryMatrixLookupSmartPanel" runat="server" Caption="Add Matrix Item: Matrix View" CaptionVisible="True" LoadOnDemand="True" Key="Matrix"
	AutoCallBack-Target="MatrixFormView" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True" CallBackMode-PostData="Page" Width="100%">

	<px:PXLayoutRule runat="server" StartColumn="True" />

    <px:PXFormView ID="MatrixFormView" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Header"
        CaptionVisible="False" SkinID="Transparent">
        <Template>
            <px:PXLayoutRule ID="InventoryMatrixLookupSmartPanelFormLayout" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
			<px:PXSelector ID="edTemplateItemID" runat="server" DataField="TemplateItemID" CommitChanges="true" />
			<px:PXSelector ID="selColumnAttributeID" runat="server" DataField="ColAttributeID" CommitChanges="true" AutoRefresh="true" />
			<px:PXSelector ID="selRowAttributeID" runat="server" DataField="RowAttributeID" CommitChanges="true" AutoRefresh="true" />
            <px:PXCheckBox ID="edShowAvailable" runat="server" DataField="ShowAvailable" AlignLeft="true" CommitChanges="true" />
			<px:PXSelector ID="selSiteID" runat="server" DataField="SiteID" CommitChanges="true" AutoRefresh="true" />
        </Template>
    </px:PXFormView>
    
	<px:PXLayoutRule runat="server" StartColumn="True" />

    <px:PXGrid ID="MatrixAttributes" runat="server" DataSourceID="ds" SkinID="Inquire" SyncPosition="True" Height="50px" Width="600px"
		AutoGenerateColumns="Recreate" RepaintColumns="True" GenerateColumnsBeforeRepaint="True" OnAfterSyncState="MatrixAttributes_AfterSyncState">
        <Levels>
            <px:PXGridLevel DataMember="AdditionalAttributes" />
        </Levels>
        <Mode AllowAddNew="False" AllowDelete="False" />
    </px:PXGrid>
	
	<px:PXLayoutRule runat="server" StartColumn="True" StartRow="true" />

    <px:PXGrid ID="MatrixMatrix" runat="server" DataSourceID="ds" SkinID="Inquire" SyncPosition="True" Height="400px" StatusField="MatrixAvailability"
		AutoGenerateColumns="Recreate" RepaintColumns="True" GenerateColumnsBeforeRepaint="True" OnAfterSyncState="MatrixMatrix_AfterSyncState" OnRowDataBound="MatrixMatrix_RowDataBound">
        <Levels>
            <px:PXGridLevel DataMember="Matrix" />
        </Levels>
        
        <Mode AllowAddNew="False" AllowDelete="False" />
		<ClientEvents AfterCellChange="matrixGrid_cellClick" />
    </px:PXGrid>

    <px:PXPanel runat="server" ID="InventoryMatrixLookupSmartButtons" SkinID="Buttons">
		<px:PXButton ID="InventoryMatrixLookupSmartPanelButtonToEntry" runat="server" DialogResult="Yes" Text="Open Table View" Height="20" />
		<px:PXButton ID="InventoryMatrixLookupSmartPanelButtonOK" runat="server" CommitChanges="True" DialogResult="OK" Text="Add and close" Height="20">
			<AutoCallBack Command="Commit" Target="ds">
				<Behavior CommitChanges="True" />
			</AutoCallBack>
		</px:PXButton>
		<px:PXButton ID="InventoryMatrixLookupSmartPanelButtonCancel" runat="server" DialogResult="Cancel" Text="Cancel" Height="20" />
	</px:PXPanel>
</px:PXSmartPanel>

<script type="text/javascript">
	// Updates availability when user clicks matrix cell
	var matrixGridOldColumnName = null;
	function matrixGrid_cellClick(grid, ev) {
		var ds = px_alls["ds"];
		var columnName = ev.cell.column.dataField;
		if (ds != null && columnName != null && (matrixGridOldColumnName == null || matrixGridOldColumnName != columnName)) {
			var showavail = px_alls["edShowAvailable"];
			if (showavail != null && showavail.getValue() == true) {
				ds.executeCallback("MatrixGridCellChanged", columnName); matrixGridOldColumnName = columnName;
			}
		}
	}
</script>
    <px:PXSmartPanel ID="spAddRelatedItems" runat="server" Key="RelatedItemsFilter" LoadOnDemand="true" Width="1150px" Height="470px"
    Caption="Add Related Items" CaptionVisible="true" AutoRepaint="true" DesignView="Content" ShowAfterLoad="True" AutoReload="true" CloseButtonDialogResult="No" AcceptButtonID="btnRIOK">
    <px:PXFormView ID="fvRelatedItemsHeader" runat="server" CaptionVisible="False" DataMember="RelatedItemsFilter" DataSourceID="ds"
        Width="100%" SkinID="Transparent">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
            <px:PXSegmentMask ID="smOrigInventory" runat="server" DataField="InventoryID" AllowEdit="False" Width="216px" />

            <px:PXLayoutRule runat="server" ControlSize="S" Merge="True" />
                <px:PXNumberEdit ID="neOrigInventoryPrice" runat="server" DataField="CuryUnitPrice" />
                <px:PXTextEdit ID="txtOrigInventoryCurrency" runat="server" DataField="CuryID" SuppressLabel="true" />

            <px:PXLayoutRule runat="server" ControlSize="S" Merge="True" />
                <px:PXNumberEdit ID="neOrigInventoryQty" runat="server" DataField="Qty" CommitChanges="true" />
                <px:PXTextEdit ID="lblOrigInventoryUom" runat="server" DataField="UOM" SuppressLabel="true" />

            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="S" />
            <px:PXLayoutRule runat="server" ControlSize="S" Merge="True" />
                <px:PXNumberEdit ID="neOrigInventoryExtPrice" runat="server" DataField="CuryExtPrice" />
                <px:PXTextEdit ID="txtOrigInventoryCurrency2" runat="server" DataField="CuryID" SuppressLabel="true" />
            <px:PXLayoutRule runat="server" />
            
            <px:PXLayoutRule runat="server" ControlSize="S" Merge="True"/>
                <px:PXNumberEdit ID="neOrigInventoryAvailQty" runat="server" DataField="AvailableQty" />
                <px:PXTextEdit ID="lblOrigInventoryUom2" runat="server" DataField="UOM" SuppressLabel="true" />
            <px:PXLayoutRule runat="server" LabelsWidth="S" ControlSize="XM" />
            <px:PXSegmentMask ID="smOrigInventotySite" runat="server" DataField="SiteID" Width="216px" />

            <px:PXLayoutRule runat="server" StartColumn="True" SuppressLabel="True" ControlSize="S" />
                <px:PXCheckBox ID="cbKeepOriginalPrice" runat="server" DataField="KeepOriginalPrice" CommitChanges="True" />
                <px:PXCheckBox ID="cbOnlyAvailableRelatedItems" runat="server" DataField="OnlyAvailableItems" CommitChanges="True" />
                <px:PXCheckBox ID="cbshowForAllWarehouses" runat="server" DataField="ShowForAllWarehouses" CommitChanges="True" />

            <px:PXCheckBox ID="cbShowSubstituteItems" runat="server" DataField="ShowSubstituteItems" Enabled="False" Visible="False" />
            <px:PXCheckBox ID="cbShowUpSellItems" runat="server" DataField="ShowUpSellItems" Enabled="False" Visible="False" />
            <px:PXCheckBox ID="cbShowCrossSellItems" runat="server" DataField="ShowCrossSellItems" Enabled="False" Visible="False" />
            <px:PXCheckBox ID="cbShowOtherRelatedItems" runat="server" DataField="ShowOtherRelatedItems" Enabled="False" Visible="False" />
            <px:PXCheckBox ID="cbShowAllRelatedItems" runat="server" DataField="ShowAllRelatedItems" Enabled="False" Visible="False" />
        </Template>
        <CallbackCommands>
	        <Save RepaintControls="None" RepaintControlsIDs="tabRelatedItems,gridAllRelatedItems,gridSubstituteItems,gridUpSellItems,gridCrossSellItems,gridOtherRelatedItems"/>
        </CallbackCommands>
    </px:PXFormView>
    <px:PXTab ID="tabRelatedItems" runat="server" Style="z-index: 100;" Width="100%" SyncPosition="True" >
        <Items>
            <px:PXTabItem Text="All Related Items" RepaintOnDemand="false" BindingContext="fvRelatedItemsHeader" VisibleExp="DataControls[&quot;cbShowAllRelatedItems&quot;].Value == true" >
                <Template>
                    <px:PXGrid ID="gridAllRelatedItems" runat="server" DataSourceID="ds"
                        AdjustPageSize="Auto" AutoAdjustColumns="true" Width="100%" SkinID="DetailsInTab" SyncPosition="True" NoteIndicator="True" FilesIndicator="True">
                        <CallbackCommands>
	                        <Save RepaintControls="None" RepaintControlsIDs="fvRelatedItemsHeader,gridSubstituteItems,gridUpSellItems,gridCrossSellItems,gridOtherRelatedItems" />
                        </CallbackCommands>
                        <Levels>
                            <px:PXGridLevel DataMember="allRelatedItems">
                                <Mode AllowAddNew="false" AllowDelete="false" />
                                <Columns>
                                    <px:PXGridColumn DataField="Selected"
                                        Type="CheckBox" TextAlign="Center" 
                                        AllowNull="False" AllowCheckAll="true"
                                        AutoCallBack="true" />
                                    <px:PXGridColumn DataField="QtySelected" />
                                    <px:PXGridColumn DataField="Rank" />
                                    <px:PXGridColumn DataField="Relation" />
                                    <px:PXGridColumn DataField="Tag" Type="DropDownList" />
                                    <px:PXGridColumn DataField="RelatedInventoryID" LinkCommand="ViewRelatedItem" />
                                    <px:PXGridColumn DataField="SubItemID" DisplayFormat="&gt;AA-A-A" />
                                    <px:PXGridColumn DataField="SubItemCD"
	                                    AllowNull="False" SyncNullable ="false" 
	                                    Visible="False" SyncVisible="false" 
	                                    AllowShowHide ="False" SyncVisibility="false" />
                                    <px:PXGridColumn DataField="Desc" />
                                    <px:PXGridColumn DataField="UOM" DisplayFormat="&gt;aaaaaa" CommitChanges="true" />
                                    <px:PXGridColumn DataField="CuryUnitPrice" />
                                    <px:PXGridColumn DataField="CuryExtPrice" />
                                    <px:PXGridColumn DataField="PriceDiff" />
                                    <px:PXGridColumn DataField="AvailableQty" />
                                    <px:PXGridColumn DataField="SiteID" />
                                    <px:PXGridColumn DataField="SiteCD" 
	                                    AllowNull="False" SyncNullable ="false" 
	                                    Visible="False" SyncVisible="false" 
	                                    AllowShowHide ="False" SyncVisibility="false" />
                                    <px:PXGridColumn DataField="Interchangeable" Type="CheckBox" TextAlign="Center"/>
                                    <px:PXGridColumn DataField="Required" Type="CheckBox" TextAlign="Center" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="true" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Substitute Items" RepaintOnDemand="false" BindingContext="fvRelatedItemsHeader" VisibleExp="DataControls[&quot;cbShowSubstituteItems&quot;].Value == true" >
                <Template>
                    <px:PXGrid ID="gridSubstituteItems" runat="server" DataSourceID="ds"
                        AdjustPageSize="Auto" AutoAdjustColumns="true" Width="100%" SkinID="DetailsInTab" SyncPosition="True" NoteIndicator="True" FilesIndicator="True">
                        <CallbackCommands>
	                        <Save RepaintControls="None" RepaintControlsIDs="fvRelatedItemsHeader,gridAllRelatedItems,gridUpSellItems,gridCrossSellItems,gridOtherRelatedItems"/>
                        </CallbackCommands>
                        <Levels>
                            <px:PXGridLevel DataMember="substituteItems">
                                <Mode AllowAddNew="false" AllowDelete="false" />
                                <Columns>
                                    <px:PXGridColumn DataField="Selected"
                                        Type="CheckBox" TextAlign="Center" 
                                        AllowNull="False" AllowCheckAll="true"
                                        AutoCallBack="true" />
                                    <px:PXGridColumn DataField="QtySelected" />
                                    <px:PXGridColumn DataField="Rank" />
                                    <px:PXGridColumn DataField="Relation" />
                                    <px:PXGridColumn DataField="Tag" Type="DropDownList" />
                                    <px:PXGridColumn DataField="RelatedInventoryID" LinkCommand="ViewRelatedItem" />
                                    <px:PXGridColumn DataField="SubItemID" DisplayFormat="&gt;AA-A-A" />
                                    <px:PXGridColumn DataField="SubItemCD"
	                                    AllowNull="False" SyncNullable ="false" 
	                                    Visible="False" SyncVisible="false" 
	                                    AllowShowHide ="False" SyncVisibility="false" />
                                    <px:PXGridColumn DataField="Desc" />
                                    <px:PXGridColumn DataField="UOM" DisplayFormat="&gt;aaaaaa" CommitChanges="true" />
                                    <px:PXGridColumn DataField="CuryUnitPrice" />
                                    <px:PXGridColumn DataField="CuryExtPrice" />
                                    <px:PXGridColumn DataField="PriceDiff" />
                                    <px:PXGridColumn DataField="AvailableQty" />
                                    <px:PXGridColumn DataField="SiteID" />
                                    <px:PXGridColumn DataField="SiteCD" 
	                                    AllowNull="False" SyncNullable ="false" 
	                                    Visible="False" SyncVisible="false" 
	                                    AllowShowHide ="False" SyncVisibility="false" />
                                    <px:PXGridColumn DataField="Interchangeable" Type="CheckBox" TextAlign="Center"/>
                                    <px:PXGridColumn DataField="Required" Type="CheckBox" TextAlign="Center" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="true" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Up-Sell Items" RepaintOnDemand="false" BindingContext="fvRelatedItemsHeader" VisibleExp="DataControls[&quot;cbShowUpSellItems&quot;].Value == true" >
                <Template>
                    <px:PXGrid ID="gridUpSellItems" runat="server" DataSourceID="ds"
                        AdjustPageSize="Auto" AutoAdjustColumns="true" Width="100%" SkinID="DetailsInTab" SyncPosition="True" NoteIndicator="True" FilesIndicator="True">
                        <CallbackCommands>
	                        <Save RepaintControls="None" RepaintControlsIDs="fvRelatedItemsHeader,gridAllRelatedItems,gridSubstituteItems,gridCrossSellItems,gridOtherRelatedItems"/>
                        </CallbackCommands>
                        <Levels>
                            <px:PXGridLevel DataMember="upSellItems">
                                <Mode AllowAddNew="false" AllowDelete="false" />
                                <Columns>
                                    <px:PXGridColumn DataField="Selected"
                                        Type="CheckBox" TextAlign="Center" 
                                        AllowNull="False" AllowCheckAll="true"
                                        AutoCallBack="true" />
                                    <px:PXGridColumn DataField="QtySelected" />
                                    <px:PXGridColumn DataField="Rank" />
                                    <px:PXGridColumn DataField="Relation" />
                                    <px:PXGridColumn DataField="Tag" Type="DropDownList" />
                                    <px:PXGridColumn DataField="RelatedInventoryID" LinkCommand="ViewRelatedItem" />
                                    <px:PXGridColumn DataField="SubItemID" DisplayFormat="&gt;AA-A-A" />
                                    <px:PXGridColumn DataField="SubItemCD"
	                                    AllowNull="False" SyncNullable ="false" 
	                                    Visible="False" SyncVisible="false" 
	                                    AllowShowHide ="False" SyncVisibility="false" />
                                    <px:PXGridColumn DataField="Desc" />
                                    <px:PXGridColumn DataField="UOM" DisplayFormat="&gt;aaaaaa" CommitChanges="true" />
                                    <px:PXGridColumn DataField="CuryUnitPrice" />
                                    <px:PXGridColumn DataField="CuryExtPrice" />
                                    <px:PXGridColumn DataField="PriceDiff" />
                                    <px:PXGridColumn DataField="AvailableQty" />
                                    <px:PXGridColumn DataField="SiteID" />
                                    <px:PXGridColumn DataField="SiteCD" 
	                                    AllowNull="False" SyncNullable ="false" 
	                                    Visible="False" SyncVisible="false" 
	                                    AllowShowHide ="False" SyncVisibility="false" />
                                    <px:PXGridColumn DataField="Interchangeable" Type="CheckBox" TextAlign="Center"/>
                                    <px:PXGridColumn DataField="Required" Type="CheckBox" TextAlign="Center" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="true" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Cross-Sell Items" RepaintOnDemand="false" BindingContext="fvRelatedItemsHeader" VisibleExp="DataControls[&quot;cbShowCrossSellItems&quot;].Value == true" >
                <Template>
                    <px:PXGrid ID="gridCrossSellItems" runat="server" DataSourceID="ds"
                        AdjustPageSize="Auto" AutoAdjustColumns="true" Width="100%" SkinID="DetailsInTab" SyncPosition="True" NoteIndicator="True" FilesIndicator="True">
                        <CallbackCommands>
	                        <Save RepaintControls="None" RepaintControlsIDs="fvRelatedItemsHeader,gridAllRelatedItems,gridSubstituteItems,gridUpSellItems,gridOtherRelatedItems"/>
                        </CallbackCommands>
                        <Levels>
                            <px:PXGridLevel DataMember="crossSellItems">
                                <Mode AllowAddNew="false" AllowDelete="false" />
                                <Columns>
                                    <px:PXGridColumn DataField="Selected"
                                        Type="CheckBox" TextAlign="Center" 
                                        AllowNull="False" AllowCheckAll="true"
                                        AutoCallBack="true" />
                                    <px:PXGridColumn DataField="QtySelected" />
                                    <px:PXGridColumn DataField="Rank" />
                                    <px:PXGridColumn DataField="Relation" />
                                    <px:PXGridColumn DataField="Tag" Type="DropDownList" />
                                    <px:PXGridColumn DataField="RelatedInventoryID" LinkCommand="ViewRelatedItem" />
                                    <px:PXGridColumn DataField="SubItemID" DisplayFormat="&gt;AA-A-A" />
                                    <px:PXGridColumn DataField="SubItemCD"
	                                    AllowNull="False" SyncNullable ="false" 
	                                    Visible="False" SyncVisible="false" 
	                                    AllowShowHide ="False" SyncVisibility="false" />
                                    <px:PXGridColumn DataField="Desc" />
                                    <px:PXGridColumn DataField="UOM" DisplayFormat="&gt;aaaaaa" CommitChanges="true" />
                                    <px:PXGridColumn DataField="CuryUnitPrice" />
                                    <px:PXGridColumn DataField="CuryExtPrice" />
                                    <px:PXGridColumn DataField="AvailableQty" />
                                    <px:PXGridColumn DataField="SiteID" />
                                    <px:PXGridColumn DataField="SiteCD" 
	                                    AllowNull="False" SyncNullable ="false" 
	                                    Visible="False" SyncVisible="false" 
	                                    AllowShowHide ="False" SyncVisibility="false" />
                                    <px:PXGridColumn DataField="Interchangeable" Type="CheckBox" TextAlign="Center"/>
                                    <px:PXGridColumn DataField="Required" Type="CheckBox" TextAlign="Center" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="true" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Other Related Items" RepaintOnDemand="false" BindingContext="fvRelatedItemsHeader" VisibleExp="DataControls[&quot;cbShowOtherRelatedItems&quot;].Value == true" >
                <Template>
                    <px:PXGrid ID="gridOtherRelatedItems" runat="server" DataSourceID="ds"
                        AdjustPageSize="Auto" AutoAdjustColumns="true" Width="100%" SkinID="DetailsInTab" SyncPosition="True" NoteIndicator="True" FilesIndicator="True">
                        <CallbackCommands>
	                        <Save RepaintControls="None" RepaintControlsIDs="fvRelatedItemsHeader,gridAllRelatedItems,gridSubstituteItems, gridUpSellItems,gridCrossSellItems"/>
                        </CallbackCommands>
                        <Levels>
                            <px:PXGridLevel DataMember="otherRelatedItems">
                                <Mode AllowAddNew="false" AllowDelete="false" />
                                <Columns>
                                    <px:PXGridColumn DataField="Selected"
                                        Type="CheckBox" TextAlign="Center" 
                                        AllowNull="False" AllowCheckAll="true"
                                        AutoCallBack="true" />
                                    <px:PXGridColumn DataField="QtySelected" />
                                    <px:PXGridColumn DataField="Rank" />
                                    <px:PXGridColumn DataField="Relation" />
                                    <px:PXGridColumn DataField="Tag" Type="DropDownList" />
                                    <px:PXGridColumn DataField="RelatedInventoryID" LinkCommand="ViewRelatedItem" />
                                    <px:PXGridColumn DataField="SubItemID" DisplayFormat="&gt;AA-A-A" />
                                    <px:PXGridColumn DataField="SubItemCD"
	                                    AllowNull="False" SyncNullable ="false" 
	                                    Visible="False" SyncVisible="false" 
	                                    AllowShowHide ="False" SyncVisibility="false" />
                                    <px:PXGridColumn DataField="Desc" />
                                    <px:PXGridColumn DataField="UOM" DisplayFormat="&gt;aaaaaa" CommitChanges="true" />
                                    <px:PXGridColumn DataField="CuryUnitPrice" />
                                    <px:PXGridColumn DataField="CuryExtPrice" />
                                    <px:PXGridColumn DataField="PriceDiff" />
                                    <px:PXGridColumn DataField="AvailableQty" />
                                    <px:PXGridColumn DataField="SiteID" />
                                    <px:PXGridColumn DataField="SiteCD" 
	                                    AllowNull="False" SyncNullable ="false" 
	                                    Visible="False" SyncVisible="false" 
	                                    AllowShowHide ="False" SyncVisibility="false" />
                                    <px:PXGridColumn DataField="Interchangeable" Type="CheckBox" TextAlign="Center"/>
                                    <px:PXGridColumn DataField="Required" Type="CheckBox" TextAlign="Center" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="true" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
        </Items>
        <CallbackCommands>
            <Search CommitChanges="True" PostData="Page" />
            <Refresh CommitChanges="True" PostData="Page" />
        </CallbackCommands>
        <AutoSize Enabled="True" Container="Parent" />
    </px:PXTab>
    <px:PXPanel ID="panel" runat="server" SkinID="Buttons">
        <px:PXButton ID="btnRIOk" runat="server" Text="Add & Close" DialogResult="OK" />
        <px:PXButton ID="btnRICancel" runat="server" DialogResult="No" Text="Cancel" />
    </px:PXPanel>
</px:PXSmartPanel>
	<px:PXSmartPanel runat="server" Height="400px" TabIndex="5500" Width="1100px" ID="PanelCreateProdOrder" LoadOnDemand="True" CaptionVisible="True" Caption="Production Orders" Key="AMSOLineRecords" AutoCallBack-Command="Refresh" AutoCallBack-Target="CreateProdgrid">
        <px:PXGrid runat="server" ID="CreateProdgrid" SyncPosition="True" Height="250px" SkinID="Inquire" TabIndex="1100" Width="1070px" DataSourceID="ds">
			<AutoSize Enabled="True" Container="Parent" />
			<ActionBar ActionsText="False" />
			<Levels>
				<px:PXGridLevel DataMember="AMSOLineRecords" DataKeyNames="OrderType,OrderNbr,LineNbr">
					<RowTemplate>
						<px:PXCheckBox runat="server" DataField="AMSelected" ID="chkCPOSelected" />
						<px:PXTextEdit runat="server" ID="edCPOLineNbr" DataField="LineNbr" />
						<px:PXSegmentMask runat="server" ID="edCPOInventoryID" DataField="InventoryID" DataKeyNames="InventoryCD" AllowEdit="true" DataMember="_InventoryItem_AccessInfo.userName_" DataSourceID="ds" />
						<px:PXSegmentMask runat="server" ID="edCPOSubItemID" DataField="SubItemID" AutoRefresh="True" />
						<px:PXNumberEdit runat="server" DataField="AMQtyReadOnly" ID="edCPOOrderQty" />
						<px:PXTextEdit runat="server" ID="edCPOUOM" DataField="AMUOMReadOnly" />
						<px:PXSelector runat="server" ID="edCPOAMorderType" DataField="AMOrderType" AllowEdit="True" CommitChanges="True" />
						<px:PXSelector runat="server" ID="edCPOProdOrdID" DataField="AMProdOrdID" AllowEdit="True" CommitChanges="True" />
                        <px:PXTextEdit runat="server" ID="edCPOStatus" DataField="AMProdItem__StatusID" />
                        <px:PXNumberEdit runat="server" DataField="AMProdItem__QtytoProd" ID="edCPOQtytoProd" />
						<px:PXNumberEdit runat="server" DataField="AMProdItem__QtyComplete" ID="edCPOQuantityComplete" />
						<px:PXTextEdit runat="server" ID="edCPOProdItemUOM" DataField="AMProdItem__UOM"  />
                    </RowTemplate>
					<Columns>
						<px:PXGridColumn DataField="AMSelected" Type="CheckBox" TextAlign="Center" AutoCallBack="True" AllowCheckAll="True" />
						<px:PXGridColumn DataField="LineNbr" Width="50px" />
						<px:PXGridColumn DataField="InventoryID" Width="150px" />
						<px:PXGridColumn DataField="SubItemID" Width="65px" />
						<px:PXGridColumn DataField="AMQtyReadOnly" TextAlign="Right" />
						<px:PXGridColumn DataField="AMUOMReadOnly" />
						<px:PXGridColumn DataField="AMorderType" Width="55px" AutoCallBack="True" />
						<px:PXGridColumn DataField="AMProdOrdID" Width="130px" AutoCallBack="True" />
						<px:PXGridColumn DataField="AMProdItem__StatusID" />
                        <px:PXGridColumn DataField="AMProdItem__QtytoProd" TextAlign="Right" Width="75px" />
						<px:PXGridColumn DataField="AMProdItem__QtyComplete" TextAlign="Right" Width="75px" />
						<px:PXGridColumn DataField="AMProdItem__UOM" Width="80px" />
						<px:PXGridColumn DataField="AMConfigurationResults__Completed" Type="CheckBox" TextAlign="Center" Width="85px" />
					</Columns>
				</px:PXGridLevel>
			</Levels>
		</px:PXGrid>
		<px:PXPanel runat="server" ID="PanelCreateProdOrderButtons" SkinID="Buttons">
			<px:PXButton runat="server" ID="CreateProd" Text="Create" DialogResult="OK" CommandSourceID="ds" CommandName="Ok" />
			<px:PXButton runat="server" ID="CancelProd" Text="Cancel" DialogResult="Cancel" CommandSourceID="ds" CommandName="Cancel" />
		</px:PXPanel>
	</px:PXSmartPanel>
	<px:PXSmartPanel runat="server" ID="AddEstimatePanel" LoadOnDemand="True" CaptionVisible="True" Caption="Add Estimate" Key="OrderEstimateItemFilter">
		<px:PXFormView runat="server" ID="estimateAddForm" SkinID="Transparent" CaptionVisible="False" Width="100%" DataSourceID="ds" DataMember="OrderEstimateItemFilter">
			<Template>
				<px:PXLayoutRule runat="server" ID="estimateAddFormpanelrule01" StartColumn="True" Merge="True" LabelsWidth="S" ControlSize="M" />
				<px:PXSelector runat="server" ID="panelEstimateID" DataField="EstimateID" AutoRefresh="True" AutoCallBack="True" CommitChanges="True" />
				<px:PXCheckBox runat="server" AutoCallBack="True" CommitChanges="True" DataField="AddExisting" ID="panelAddExisting" />
				<px:PXLayoutRule runat="server" ID="estimateAddFormpanelrule02" LabelsWidth="S" ControlSize="M" />
				<px:PXSelector runat="server" ID="panelRevisionID" DataField="RevisionID" AutoRefresh="True" AutoCallBack="True" CommitChanges="True" />
				<px:PXLayoutRule runat="server" ID="estimateAddFormpanelrule03" Merge="True" LabelsWidth="S" ControlSize="M" />
				<px:PXSelector runat="server" ID="panelInventoryCD" DataField="InventoryCD" AutoRefresh="True" AutoCallBack="True" CommitChanges="True" />
				<px:PXCheckBox runat="server" DataField="IsNonInventory" ID="panelEstimateIsNonInventory" />
				<px:PXLayoutRule runat="server" StartColumn="False" LabelsWidth="SM" ControlSize="M" />
				<px:PXSegmentMask runat="server" DataField="SubItemID" ID="edSubItemID" />
				<px:PXSelector runat="server" DataField="SiteID" ID="edSiteID" />
				<px:PXLayoutRule runat="server" ID="estimateAddFormpanelrule04" LabelsWidth="S" ControlSize="XL" />
				<px:PXTextEdit runat="server" ID="panelItemDesc" CommitChanges="True" DataField="ItemDesc" />
				<px:PXLayoutRule runat="server" ID="estimateAddFormpanelrule05" LabelsWidth="S" ControlSize="M" />
				<px:PXSelector runat="server" ID="panelEstimateClassID" DataField="EstimateClassID" AutoRefresh="True" AutoCallBack="True" CommitChanges="True" />
				<px:PXSelector runat="server" ID="panelItemClassID" DataField="ItemClassID" AutoRefresh="True" AutoCallBack="True" CommitChanges="True" />
				<px:PXSelector runat="server" ID="panelEstimateUOM" DataField="UOM" CommitChanges="True" />
				<px:PXSelector runat="server" ID="panelBranchID" DataField="BranchID" CommitChanges="True" />
			</Template>
		</px:PXFormView>
		<px:PXPanel runat="server" ID="AddEstButtonPanel" SkinID="Buttons">
			<px:PXButton runat="server" ID="AddEstButton1" Text="OK" DialogResult="OK" CommandSourceID="ds" />
			<px:PXButton runat="server" ID="AddEstButton2" Text="Cancel" DialogResult="Cancel" CommandSourceID="ds" />
		</px:PXPanel>
	</px:PXSmartPanel>
	<px:PXSmartPanel runat="server" ID="QuickEstimatePanel" LoadOnDemand="True" CloseButtonDialogResult="Abort" AutoReload="True" CaptionVisible="True" Caption="Quick Estimate" Key="SelectedEstimateRecord">
		<px:PXFormView runat="server" ID="QuickEstimateForm" SkinID="Transparent" CaptionVisible="False" Width="100%" DefaultControlID="EstimateID" SyncPosition="True" DataSourceID="ds" DataMember="SelectedEstimateRecord">
			<Template>
				<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
				<px:PXSelector runat="server" ID="panelQuickEstimateID" DataField="EstimateID" />
				<px:PXSelector runat="server" ID="panelQuickRevisionID" DataField="RevisionID" />
				<px:PXLayoutRule runat="server" Merge="true" LabelsWidth="SM" ControlSize="M" />
				<px:PXSelector runat="server" ID="panelInventoryCD" DataField="InventoryCD" />
				<px:PXCheckBox runat="server" DataField="IsNonInventory" ID="panelIsNonInventory" />
				<px:PXLayoutRule runat="server" StartColumn="False" LabelsWidth="SM" ControlSize="M" />
				<px:PXSegmentMask runat="server" DataField="SubItemID" ID="edSubItemID" />
				<px:PXSelector runat="server" DataField="SiteID" ID="edSiteID" />
				<px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="L" />
				<px:PXTextEdit runat="server" ID="panelItemDesc" DataField="ItemDesc" />
				<px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="M" />
				<px:PXSelector runat="server" ID="panelEstimateClassID" DataField="EstimateClassID" CommitChanges="True" />
				<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="SM" ControlSize="XL" />
				<px:PXNumberEdit runat="server" CommitChanges="True" DataField="FixedLaborCost" ID="edFixedLaborCost" />
				<px:PXCheckBox runat="server" CommitChanges="True" DataField="FixedLaborOverride" ID="edFixedLaborOverride" />
				<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="SM" ControlSize="XL" />
				<px:PXNumberEdit runat="server" CommitChanges="True" DataField="VariableLaborCost" ID="edVariableLaborCost" />
				<px:PXCheckBox runat="server" CommitChanges="True" DataField="VariableLaborOverride" ID="edVariableLaborOverride" />
				<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="SM" ControlSize="XL" />
				<px:PXNumberEdit runat="server" CommitChanges="True" DataField="MachineCost" ID="edMachineCost" />
				<px:PXCheckBox runat="server" CommitChanges="True" DataField="MachineOverride" ID="edMachineOverride" />
				<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="SM" ControlSize="XL" />
				<px:PXNumberEdit runat="server" CommitChanges="True" DataField="MaterialCost" ID="edMaterialCost" />
				<px:PXCheckBox runat="server" CommitChanges="True" DataField="MaterialOverride" ID="edMaterialOverride" />
				<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="SM" ControlSize="XL" />
				<px:PXNumberEdit runat="server" CommitChanges="True" DataField="ToolCost" ID="edToolCost" />
				<px:PXCheckBox runat="server" CommitChanges="True" DataField="ToolOverride" ID="edToolOverride" />
				<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="SM" ControlSize="XL" />
				<px:PXNumberEdit runat="server" CommitChanges="True" DataField="FixedOverheadCost" ID="edFixedOverheadCost" />
				<px:PXCheckBox runat="server" CommitChanges="True" DataField="FixedOverheadOverride" ID="edFixedOverheadOverride" />
				<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="SM" ControlSize="XL" />
				<px:PXNumberEdit runat="server" CommitChanges="True" DataField="VariableOverheadCost" ID="edVariableOverheadCost" />
				<px:PXCheckBox runat="server" CommitChanges="True" DataField="VariableOverheadOverride" ID="edVariableOverheadOverride" />
				<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="SM" ControlSize="XL" />
				<px:PXNumberEdit runat="server" CommitChanges="True" DataField="SubcontractCost" ID="edSubcontractCost" />
				<px:PXCheckBox runat="server" CommitChanges="True" DataField="SubcontractOverride" ID="edSubcontractOverride" />
				<px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="XL" />
				<px:PXNumberEdit runat="server" DataField="ExtCostDisplay" ID="edCuryExtCost" />
				<px:PXNumberEdit runat="server" CommitChanges="True" DataField="ReferenceMaterialCost" ID="edReferenceMaterialCost" />
				<px:PXNumberEdit runat="server" AutoCallBack="True" CommitChanges="True" DataField="OrderQty" ID="panelQuickOrderQty" />
				<px:PXLayoutRule runat="server" ControlSize="M" />
				<px:PXSelector runat="server" ID="panelQuickUOM" DataField="UOM" />
				<px:PXLayoutRule runat="server" ControlSize="XL" />
				<px:PXNumberEdit runat="server" AutoCallBack="True" CommitChanges="True" DataField="CuryUnitCost" ID="panelQuickCuryUnitCost" />
				<px:PXNumberEdit runat="server" AutoCallBack="True" CommitChanges="True" DataField="MarkupPct" ID="panelQuickMarkupPct" />
				<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="SM" ControlSize="XL" />
				<px:PXNumberEdit runat="server" AutoCallBack="True" CommitChanges="True" DataField="CuryUnitPrice" ID="panelQuickCuryUnitPrice" />
				<px:PXCheckBox runat="server" CommitChanges="True" DataField="PriceOverride" ID="edQuick1" />
				<px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="XL" />
				<px:PXNumberEdit runat="server" CommitChanges="True" DataField="CuryExtPrice" ID="panelQuickCuryExtPrice" />
			</Template>
		</px:PXFormView>
		<px:PXPanel runat="server" ID="QuickEstButtonPanel" SkinID="Buttons">
			<px:PXButton runat="server" ID="QuickEstButton1" Text="OK" DialogResult="OK" CommandSourceID="ds" />
			<px:PXButton runat="server" ID="QuickEstButton2" Text="Cancel" DialogResult="Abort" CommandSourceID="ds" />
		</px:PXPanel>
	</px:PXSmartPanel>
    <px:PXSmartPanel runat="server" Height="360px" TabIndex="5501" Width="960px" ID="edPanelLinkProd" LoadOnDemand="True" CaptionVisible="True" CloseButtonDialogResult="Abort"
            Caption="Production Details" Key="AMSOLineLinkRecords" AutoCallBack-Command="Refresh" AutoCallBack-Target="LinkProdGrid" CallBackMode-CommitChanges="True" > 
        <px:PXFormView ID="formLinkFilter" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="linkProdOrderSelectFilter"
            Caption="None" CaptionVisible="false" SkinID="Transparent">
            <Template>
                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="S" />
                <px:PXSelector runat="server" ID="LinkOrderType" DataField="OrderType" AllowEdit="False" CommitChanges="true" />
                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="S"  />
				<px:PXSelector runat="server" ID="LinkProdNbr" DataField="ProdOrdID" AllowEdit="False"  CommitChanges="true" />
                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="S" />
                <px:PXDropDown CommitChanges="True" ID="LinkStatusID" runat="server" DataField="StatusID" />
            </Template>
        </px:PXFormView>
        <px:PXGrid ID="LinkProdGrid" runat="server" Height="360px" Width="100%" DataSourceID="ds" AutoAdjustColumns="true" SyncPosition="True" SkinID="Details" > 
            <Levels>
                <px:PXGridLevel DataMember="AMSOLineLinkRecords" DataKeyNames="OrderType,OrderNbr,LineNbrOrderType,OrderNbr,LineNbr" > 
                    <RowTemplate>
						<px:PXCheckBox runat="server" DataField="Selected" ID="LinkGridCheckSelect"  CommitChanges="true"  />
						<px:PXSelector runat="server" ID="LinkGridOrderType" DataField="OrderType" AllowEdit="true" />
						<px:PXSelector runat="server" ID="LinkGridProdNbr" DataField="ProdOrdID" AllowEdit="true" />
                        <px:PXTextEdit runat="server" ID="LinkGridStatus" DataField="StatusID" />
						<px:PXNumberEdit runat="server" DataField="QtytoProd" ID="LinkGridQtytoProd" />
						<px:PXNumberEdit runat="server" DataField="QtyComplete" ID="LinkGridQtyComplete" />
						<px:PXTextEdit runat="server" ID="LinkGridUOM" DataField="UOM" />
                        <px:PXDateTimeEdit runat="server" ID="LinkGridStartDate" DataField="StartDate" CommitChanges="true" />
                	</RowTemplate>
					<Columns>
						<px:PXGridColumn DataField="Selected" Type="CheckBox" TextAlign="Center" AutoCallBack="True" AllowCheckAll="false" />
						<px:PXGridColumn DataField="OrderType" Width="55px"/>
						<px:PXGridColumn DataField="ProdOrdID" Width="130px"/>
						<px:PXGridColumn DataField="StatusID" />
                        <px:PXGridColumn DataField="QtytoProd" TextAlign="Right" Width="75px" />
						<px:PXGridColumn DataField="QtyComplete" TextAlign="Right" Width="75px" />
						<px:PXGridColumn DataField="UOM" Width="80px" />
                     </Columns>
                    <Mode AllowAddNew="false" AllowDelete="false" />
                </px:PXGridLevel>
            </Levels>
            <AutoSize Enabled="true" />
        </px:PXGrid>
        <px:PXPanel ID="PanelLinkProdOrderButtons" runat="server" SkinID="Buttons">
            <px:PXButton ID="LinkProd" runat="server" DialogResult="OK" Text="Save" />
            <px:PXButton ID="CancelLink" runat="server" DialogResult="Abort" Text="Cancel" />
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
	<px:PXSmartPanel ID="PanelReassignApproval" runat="server" Style="z-index: 108; position: absolute; left: 27px; top: 99px;"
    Caption="Reassign Approval"
    CaptionVisible="True" LoadOnDemand="true" AutoReload="true" Key="ReassignApprovalFilter"
    AutoCallBack-Enabled="true" AutoCallBack-Command="Refresh" CloseButtonDialogResult="Abort">

    <px:PXFormView ID="formReassignApproval" runat="server" DataMember="ReassignApprovalFilter" DataSourceID="ds" Style="z-index: 100" Width="100%"
        Caption="Reassign Approval" CaptionVisible="False" SkinID="Transparent">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
            <px:PXSelector DataField="NewApprover" CommitChanges="True" ID="edNewApprover" runat="server" AutoRefresh="True" />
            <px:PXCheckBox DataField="IgnoreApproversDelegations" CommitChanges="True" ID="chkIgnoreApproversDelegations" runat="server" />
        </Template>
    </px:PXFormView>

    <div style="padding: 10px 10px 5px 5px; text-align: right;">
        <px:PXButton ID="btnReassignApprovalOK" runat="server" Text="Reassign" DialogResult="OK"></px:PXButton>
        <px:PXButton ID="btnReassignApprovalCancel" runat="server" DialogResult="Cancel" Text="Cancel" Style="margin-left: 5px" />
    </div>

</px:PXSmartPanel>
	<px:PXSmartPanel ID="CreatePaymentSmartPanel" runat="server" Caption="Create Payment" CaptionVisible="True" LoadOnDemand="True" Key="QuickPayment"
	AutoCallBack-Target="CreatePaymentFormView" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True" CallBackMode-PostData="Page" Height="295px">

	<px:PXLayoutRule runat="server" StartColumn="True" />

	<px:PXFormView ID="CreatePaymentFormView" runat="server" DataSourceID="ds" Width="100%" DataMember="QuickPayment" CaptionVisible="False" SkinID="Transparent" Height="230px">
		<Template>
			<px:PXLayoutRule ID="CreatePaymentFormLayout" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
            <px:PXLayoutRule runat="server" Merge="True" />
			<px:PXNumberEdit ID="edCuryOrigDocAmt" runat="server" DataField="CuryOrigDocAmt" CommitChanges="True"/>
			<px:PXNumberEdit ID="edCuryRefundAmt" runat="server" DataField="CuryRefundAmt" CommitChanges="True"/>
			<px:PXSelector ID="edCuryID" runat="server" DataField="CuryID" Enabled="False" SuppressLabel="True" />
			<px:PXLayoutRule runat="server" />
			<px:PXTextEdit ID="edDocDesc" runat="server" DataField="DocDesc" />
			<px:PXSelector ID="selPaymentMethodID" runat="server" DataField="PaymentMethodID" CommitChanges="true" />
			<px:PXSelector ID="selRefTranExtNbr" runat="server" DataField="RefTranExtNbr" AutoRefresh="True" CommitChanges="true" />
			<px:PXLayoutRule runat="server" Merge="True" />
			<px:PXCheckBox ID="chkNewCard" runat="server" CommitChanges="true" DataField="NewCard" />
			<px:PXCheckBox ID="chkSaveCard" runat="server" CommitChanges="true" DataField="SaveCard" />
			<px:PXLayoutRule runat="server" Merge="True" />
			<px:PXCheckBox ID="chkNewAccount" runat="server" CommitChanges="true" DataField="NewAccount" />
			<px:PXCheckBox ID="chkSaveAccount" runat="server" CommitChanges="true" DataField="SaveAccount" />
            <px:PXLayoutRule runat="server" />
			<px:PXSelector CommitChanges="True" ID="edPMInstanceID" runat="server" DataField="PMInstanceID" TextField="Descr" AutoRefresh="True" AutoGenerateColumns="True" />
			<px:PXSegmentMask CommitChanges="True" ID="edCashAccountID" runat="server" DataField="CashAccountID" AutoRefresh="True" />
			<px:PXSelector ID="edProcessingCenterID" runat="server" AutoRefresh="True" CommitChanges="True" DataField="ProcessingCenterID" />
			<px:PXSelector ID="edPosTerminalID" runat="server" AutoRefresh="True" CommitChanges="True" DataField="TerminalID" />
			<px:PXTextEdit ID="edExtRefNbr" runat="server" DataField="ExtRefNbr" CommitChanges="True" />
		</Template>
	</px:PXFormView>

	<px:PXPanel runat="server" ID="CreatePaymentSmartButtons" SkinID="Buttons">
		<px:PXButton ID="CreatePaymentRefundButton" runat="server" CommitChanges="True" CommandName="CreatePaymentRefund" CommandSourceID="ds" Height="20" SyncVisible="true" DialogResult="Abort" />
		<px:PXButton ID="CreatePaymentCaptureButton" runat="server" CommitChanges="True" CommandName="CreatePaymentCapture" CommandSourceID="ds" Height="20" SyncVisible="true" DialogResult="Yes" />
		<px:PXButton ID="CreatePaymentAuthorizeButton" runat="server" CommitChanges="True" CommandName="CreatePaymentAuthorize" CommandSourceID="ds" Height="20" SyncVisible="true" DialogResult="No" />
		<px:PXButton ID="CreatePaymentOKButton" runat="server" CommitChanges="True" CommandName="CreatePaymentOK" CommandSourceID="ds" Height="20" SyncVisible="true" DialogResult="OK" />
		<px:PXButton ID="CreatePaymentCancelButton" runat="server" DialogResult="Cancel" Text="Cancel" Height="20" />
	</px:PXPanel>

</px:PXSmartPanel>
	<px:PXSmartPanel ID="CreatePrepaymentInvoiceSmartPanel" runat="server" Caption="Create Prepayment Invoice" CaptionVisible="True" LoadOnDemand="True" Key="QuickPrepaymentInvoice"
	AutoCallBack-Target="CreatePrepaymentInvoiceFormView" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True" CallBackMode-PostData="Page">

	<px:PXLayoutRule runat="server" StartColumn="True" />

	<px:PXFormView ID="CreatePrepaymentInvoiceFormView" runat="server" DataSourceID="ds" Width="100%" DataMember="QuickPrepaymentInvoice" CaptionVisible="False" SkinID="Transparent" Height="70px">
		<Template>
			<px:PXLayoutRule runat="server" LabelsWidth="130px"/>
			<px:PXNumberEdit ID="edPrepaymentPct" runat="server" DataField="PrepaymentPct" CommitChanges="True"/>
			<px:PXLayoutRule runat="server" Merge="True" />
			<px:PXNumberEdit ID="edCuryPrepaymentAmt" runat="server" DataField="CuryPrepaymentAmt" CommitChanges="True"/>
			<px:PXSelector ID="edCuryID" runat="server" DataField="CuryID" Enabled="False" SuppressLabel="True" Width="60px"/>
		</Template>
	</px:PXFormView>

	<px:PXPanel runat="server" ID="CreatePrepaymentInvoiceSmartButtons" SkinID="Buttons">
		<px:PXButton ID="CreatePrepaymentInvoiceOKButton" runat="server" CommitChanges="True" CommandName="CreatePrepaymentInvoiceOK" CommandSourceID="ds" Height="20" SyncVisible="true" DialogResult="OK" />
		<px:PXButton ID="CreatePrepaymentInvoiceCancelButton" runat="server" DialogResult="Cancel" Text="Cancel" Height="20" />
	</px:PXPanel>

</px:PXSmartPanel>
	<px:PXSmartPanel ID="ImportPaymentSmartPanel" runat="server" Caption="Import CC Payment" CaptionVisible="True" LoadOnDemand="True" Key="ImportExternalTran"
	AutoCallBack-Target="ImportPaymentFormView" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True" CallBackMode-PostData="Page">

	<px:PXLayoutRule runat="server" StartColumn="True" />

	<px:PXFormView ID="ImportPaymentFormView" runat="server" DataSourceID="ds" Width="100%" DataMember="ImportExternalTran" CaptionVisible="False" SkinID="Transparent">
		<Template>
			<px:PXLayoutRule ID="ImportPaymentFormLayout" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
			<px:PXTextEdit ID="edTranNumber" runat="server" DataField="TranNumber" CommitChanges="True" />
			<px:PXSelector ID="edProcessingCenterID" runat="server" AutoRefresh="True" CommitChanges="True" DataField="ProcessingCenterID" />
		</Template>
	</px:PXFormView>

	<px:PXPanel runat="server" ID="ImportPaymentSmartButtons" SkinID="Buttons">
		<px:PXButton ID="ImportPaymentCreateButton" runat="server" CommitChanges="True" CommandName="ImportDocumentPaymentCreate" CommandSourceID="ds" Height="20" SyncVisible="true" DialogResult="OK" />
		<px:PXButton ID="ImportPaymentCancelButton" runat="server" DialogResult="Cancel" Text="Cancel" Height="20" />
	</px:PXPanel>

</px:PXSmartPanel>
	<px:PXSmartPanel ID="PanelAddSiteStatus" runat="server" Key="ItemInfo" LoadOnDemand="true" Width="1100px" Height="500px"
	Caption="Inventory Lookup" CaptionVisible="true" AutoRepaint="true" DesignView="Content" ShowAfterLoad="true">
	<px:PXFormView ID="formSitesStatus" runat="server" CaptionVisible="False" DataMember="ItemFilter" DataSourceID="ds"
		Width="100%" SkinID="Transparent">
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
			<px:PXTextEdit ID="PanelAddSiteStatus_edInventory" runat="server" DataField="Inventory" IsClientControl="False"/>
			<px:PXTextEdit CommitChanges="True" ID="PanelAddSiteStatus_edBarCode" runat="server" DataField="BarCode" />
			<px:PXSegmentMask CommitChanges="True" ID="PanelAddSiteStatus_edSiteID" runat="server" DataField="SiteID" AutoRefresh="true" />
			<px:PXSegmentMask CommitChanges="True" ID="PanelAddSiteStatus_edItemClassID" runat="server" DataField="ItemClass" />
			<px:PXSegmentMask CommitChanges="True" ID="PanelAddSiteStatus_edSubItem" runat="server" DataField="SubItem" AutoRefresh="true" />

			<px:PXLayoutRule runat="server" LabelsWidth="S" ControlSize="S" GroupCaption="Selected Mode" StartColumn="true" />
			<px:PXGroupBox CommitChanges="True" RenderStyle="Simple" ID="PanelAddSiteStatus_gpMode" runat="server" Caption="Selected Mode"
				DataField="Mode" Width="280px" Height="25px">
				<Template>
					<px:PXLayoutRule runat="server" Merge="True" LabelsWidth="S" ControlSize="S" />
					<px:PXRadioButton runat="server" ID="PanelAddSiteStatus_rModeSite" Value="0" Text="By Site State" />
					<px:PXRadioButton runat="server" ID="PanelAddSiteStatus_rModeCustomer" Value="1" Text="By Last Sale" />
					<px:PXDateTimeEdit CommitChanges="True" ID="PanelAddSiteStatus_edHistoryDate" runat="server" DataField="HistoryDate" SuppressLabel="true" />
				</Template>
			</px:PXGroupBox>

			<px:PXPanel runat="server" Height="50px" RenderStyle="Simple" ID="PanelAddSiteStatus_pnlOnlyAvailable" >
				<px:PXLayoutRule runat="server" LabelsWidth="S" ControlSize="M" />
				<px:PXCheckBox CommitChanges="True" ID="PanelAddSiteStatus_chkOnlyAvailable" AlignLeft="true" runat="server" Checked="True" DataField="OnlyAvailable" />
				<px:PXCheckBox CommitChanges="True" ID="PanelAddSiteStatus_chkDropShipSales" AlignLeft="true" runat="server" Checked="True" DataField="DropShipSales" />
			</px:PXPanel>

			<px:PXLayoutRule runat="server" StartColumn="true" StartGroup="true" LabelsWidth="SM" ControlSize="M" GroupCaption="Shipping Parameters to Apply" />
			<px:PXSegmentMask CommitChanges="True" ID="PanelAddSiteStatus_edCustomerLocationIDApply" runat="server" AutoRefresh="True"
					DataField="CustomerLocationID" DataSourceID="ds" />
		</Template>
	</px:PXFormView>
	<script type="text/javascript">
	    function UpdateItemSiteCell(n, c) {
			var activeRow = c.cell.row;
			var sCell = activeRow.getCell("Selected");
			var qCell = activeRow.getCell("QtySelected");
			if (sCell == c.cell) {
				if (sCell.getValue() == true)
					qCell.setValue("1");
				else
					qCell.setValue("0");
			}
			if (qCell == c.cell) {
				if (qCell.getValue() == "0")
					sCell.setValue(false);
				else
					sCell.setValue(true);
			}
		}
    </script>
	<px:PXGrid ID="PanelAddSiteStatus_gripSiteStatus" runat="server" DataSourceID="ds" Style="height: 189px;"
		AutoAdjustColumns="true" Width="100%" SkinID="Details" AdjustPageSize="Auto" AllowSearch="True" FastFilterID="PanelAddSiteStatus_edInventory"
		FastFilterFields="InventoryCD,Descr,AlternateID">
		<CallbackCommands>
			<Refresh CommitChanges="true"></Refresh>
		</CallbackCommands>
		<ClientEvents AfterCellUpdate="UpdateItemSiteCell" />
		<ActionBar PagerVisible="False">
			<PagerSettings Mode="NextPrevFirstLast" />
		</ActionBar>
		<Levels>
			<px:PXGridLevel DataMember="ItemInfo">
				<Mode AllowAddNew="false" AllowDelete="false" />
				<RowTemplate>
					<px:PXSegmentMask ID="PanelAddSiteStatus_editemClass" runat="server" DataField="ItemClassID" />
				</RowTemplate>
				<Columns>
					<px:PXGridColumn AllowNull="False" DataField="Selected" TextAlign="Center" Type="CheckBox" AutoCallBack="true"
						AllowCheckAll="true" />
					<px:PXGridColumn AllowNull="False" DataField="QtySelected" TextAlign="Right" />
					<px:PXGridColumn DataField="SiteID" />
					<px:PXGridColumn DataField="SiteCD" 
						AllowNull="False" SyncNullable ="false" 
						Visible="False" SyncVisible="false" 
						AllowShowHide ="False" SyncVisibility="false" />
					<px:PXGridColumn DataField="ItemClassID" />
					<px:PXGridColumn DataField="ItemClassDescription" />
					<px:PXGridColumn DataField="PriceClassID" />
					<px:PXGridColumn DataField="PriceClassDescription" />
					<px:PXGridColumn DataField="PreferredVendorID" />
					<px:PXGridColumn DataField="PreferredVendorDescription" />
					<px:PXGridColumn DataField="InventoryCD" DisplayFormat="&gt;AAAAAAAAAA" />
					<px:PXGridColumn DataField="SubItemID" DisplayFormat="&gt;AA-A-A" />
					<px:PXGridColumn DataField="SubItemCD"
						AllowNull="False" SyncNullable ="false" 
						Visible="False" SyncVisible="false" 
						AllowShowHide ="False" SyncVisibility="false" />
					<px:PXGridColumn DataField="Descr" />
					<px:PXGridColumn DataField="SalesUnit" DisplayFormat="&gt;aaaaaa" />
					<px:PXGridColumn AllowNull="False" DataField="QtyAvailSale" TextAlign="Right" />
					<px:PXGridColumn AllowNull="False" DataField="QtyOnHandSale" TextAlign="Right" />
					<px:PXGridColumn DataField="CuryID" DisplayFormat="&gt;LLLLL" />
					<px:PXGridColumn AllowNull="False" DataField="QtyLastSale" TextAlign="Right" />
					<px:PXGridColumn AllowNull="False" DataField="CuryUnitPrice" TextAlign="Right" />
					<px:PXGridColumn AllowNull="False" DataField="LastSalesDate" TextAlign="Right" />
					<px:PXGridColumn AllowNull="False" DataField="DropShipLastQty" TextAlign="Right" />
					<px:PXGridColumn AllowNull="False" DataField="DropShipCuryUnitPrice" TextAlign="Right" />
					<px:PXGridColumn AllowNull="False" DataField="DropShipLastDate" TextAlign="Right" />
					<px:PXGridColumn AllowNull="False" DataField="AlternateID" AllowFilter="false" AllowSort="False" />
					<px:PXGridColumn AllowNull="False" DataField="AlternateType" AllowFilter="false" AllowSort="False" />
					<px:PXGridColumn AllowNull="False" DataField="AlternateDescr" AllowFilter="false" AllowSort="False" />
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Enabled="true" />
	</px:PXGrid>
	<px:PXPanel ID="PanelAddSiteStatus_pnlButtons" runat="server" SkinID="Buttons">
		<px:PXButton ID="PanelAddSiteStatus_btnAdd" runat="server" CommandName="AddSelectedItems" CommandSourceID="ds" Text="Add" SyncVisible="false" />
		<px:PXButton ID="PanelAddSiteStatus_btnAddClose" runat="server" Text="Add & Close" DialogResult="OK" />
		<px:PXButton ID="PanelAddSiteStatus_btnCancel" runat="server" DialogResult="Cancel" Text="Cancel" />
	</px:PXPanel>
</px:PXSmartPanel>
</asp:Content>
