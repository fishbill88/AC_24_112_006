<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="PO301000.aspx.cs"
    Inherits="Page_PO301000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <script type="text/javascript">
		function showPOReceiptsPanel() {
            var formReceipts = px_alls["formReceipts"];
            var formChildOrders = px_alls["formChildOrders"];
            var splitterPOHistory = px_alls["sp1"];

            if (splitterPOHistory && formReceipts && formChildOrders) {
                var formVisible = formReceipts.getVisible() || formChildOrders.getVisible();
				splitterPOHistory.setDisabledPanel(formVisible ? 0 : 1);
			}
		}

		function tabViewInit(sender) {
			var form = px_alls["form"];
			form.events.addEventHandler("afterRepaint", showPOReceiptsPanel);
		}
	</script>

	<px:PXDataSource EnableAttributes="true" UDFTypeField="OrderType" ID="ds" runat="server" Visible="True" TypeName="PX.Objects.PO.POOrderEntry" PrimaryView="Document" Width="100%">
		<CallbackCommands>
			<px:PXDSCallbackCommand StartNewGroup="True" Name="Action" />
			<px:PXDSCallbackCommand Visible="false" Name="AddPOOrder" CommitChanges="true" />
			<px:PXDSCallbackCommand Visible="false" Name="AddPOOrderLine" CommitChanges="true" />
			<px:PXDSCallbackCommand Name="CreatePOReceipt" CommitChanges="true" />
			<px:PXDSCallbackCommand Visible="False" Name="CurrencyView" />
			<px:PXDSCallbackCommand Name="ViewDemand" Visible="false" DependOnGrid="grid" />
			<px:PXDSCallbackCommand Name="NewTask" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="NewEvent" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="NewActivity" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="NewMailActivity" Visible="False" CommitChanges="True" PopupCommand="Cancel" PopupCommandTarget="ds" />
			<px:PXDSCallbackCommand StartNewGroup="True" Name="ValidateAddresses" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="RecalculateDiscountsAction" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="RecalcOk" PopupCommand="" PopupCommandTarget="" PopupPanel="" Text="" Visible="False" />       
            <px:PXDSCallbackCommand Name="CreatePrepayment" CommitChanges="True" PopupCommand="Cancel" PopupCommandTarget="ds" />   
            <px:PXDSCallbackCommand Name="PasteLine" Visible="False" CommitChanges="true" DependOnGrid="grid" />
            <px:PXDSCallbackCommand Name="ResetOrder" Visible="False" CommitChanges="true" DependOnGrid="grid" />
            <px:PXDSCallbackCommand Name="ViewChangeOrder" Visible="False" DependOnGrid="ChangeOrdersGrid" />
            <px:PXDSCallbackCommand Name="ViewOrigChangeOrder" Visible="False" DependOnGrid="ChangeOrdersGrid" />
			<px:PXDSCallbackCommand Name="ShowMatrixPanel" CommitChanges="true" Visible="False" />
            <px:PXDSCallbackCommand Name="ComplianceDocument$PurchaseOrder$Link" Visible="false" DependOnGrid="grid" CommitChanges="True" />
			<px:PXDSCallbackCommand CommitChanges="True" Name="ComplianceDocument$Subcontract$Link" Visible="false" DependOnGrid="grid" />
			<px:PXDSCallbackCommand Name="ComplianceDocument$InvoiceID$Link" Visible="false" DependOnGrid="grdComplianceDocuments" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="ComplianceDocument$BillID$Link" Visible="false" DependOnGrid="grdComplianceDocuments" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="ComplianceDocument$ApCheckID$Link" Visible="false" DependOnGrid="grdComplianceDocuments" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="ComplianceDocument$ArPaymentID$Link" Visible="false" DependOnGrid="grdComplianceDocuments" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="ComplianceDocument$ProjectTransactionID$Link" Visible="false" DependOnGrid="grdComplianceDocuments" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="AddressLookupSelectAction" CommitChanges="true" Visible="false" />
			<px:PXDSCallbackCommand Name="AddressLookup" SelectControlsIDs="form" RepaintControls="None" RepaintControlsIDs="ds,Shipping_Address" CommitChanges="true" Visible="false" />
			<px:PXDSCallbackCommand Name="RemitAddressLookup" SelectControlsIDs="form" RepaintControls="None" RepaintControlsIDs="ds,formVA" CommitChanges="true" Visible="false" />
		</CallbackCommands>
		<DataTrees>
			<px:PXTreeDataMember TreeView="_EPCompanyTree_Tree_" TreeKeys="WorkgroupID" />
		</DataTrees>
	</px:PXDataSource>
    <px:PXSmartPanel ID="PanelAddPOLine" runat="server" Height="415px" Style="z-index: 108; position: absolute; left: 660px;
        top: 99px" Width="960px" Key="poLinesSelection" Caption="Add Purchase Order Line" CaptionVisible="True" LoadOnDemand="true" AutoRepaint="true" 
        ShowAfterLoad="true" CloseButtonDialogResult="No">
        <px:PXFormView ID="frmPOFilter" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="filter" Caption="PO Selection"
            CaptionVisible="false" SkinID="Transparent">
            <Template>
                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                <px:PXDropDown CommitChanges="True" ID="edOrderType" runat="server" AllowNull="False" DataField="OrderType" SelectedIndex="2" />
                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                <px:PXSelector CommitChanges="True" ID="edOrderNbr" runat="server" DataField="OrderNbr" AutoRefresh="True" />
            </Template>
        </px:PXFormView>
        <px:PXGrid ID="gridOL" runat="server" Height="240px" Width="100%" DataSourceID="ds" Style="border-width: 1px 0px" AutoAdjustColumns="true" PageSize="200" SkinID="Inquire">
            <Levels>
                <px:PXGridLevel DataMember="poLinesSelection">
                    <Columns>
                        <px:PXGridColumn DataField="Selected" Type="CheckBox" AllowCheckAll="True" />
                        <px:PXGridColumn AllowNull="False" DataField="LineType" />
                        <px:PXGridColumn DataField="InventoryID" DisplayFormat="&gt;AAAAAAAAAA" />
                        <px:PXGridColumn DataField="SubItemID" DisplayFormat="&gt;AA-A-A" />
                        <px:PXGridColumn DataField="UOM" DisplayFormat="&gt;aaaaaa" />
                        <px:PXGridColumn DataField="OrderQty" TextAlign="Right" />
                        <px:PXGridColumn DataField="OpenQty" TextAlign="Right" />
                        <px:PXGridColumn DataField="TranDesc" />
                        <px:PXGridColumn AllowNull="False" DataField="RcptQtyMin" TextAlign="Right" />
                        <px:PXGridColumn AllowNull="False" DataField="RcptQtyMax" TextAlign="Right" />
                        <px:PXGridColumn AllowNull="False" DataField="RcptQtyAction" Type="DropDownList" />
                    </Columns>
                    <Mode AllowAddNew="false" AllowUpdate="false" AllowDelete="false" />
                </px:PXGridLevel>
            </Levels>
            <AutoSize Enabled="true" />
        </px:PXGrid>
        <px:PXPanel ID="PXPanel1" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton1" runat="server" DialogResult="OK" Text="Save" />
            <px:PXButton ID="PXButton2" runat="server" DialogResult="No" Text="Cancel" />
        </px:PXPanel>
    </px:PXSmartPanel>
    <px:PXSmartPanel ID="PanelAddPO" runat="server" Height="415px" Style="z-index: 108; left: 486px; position: absolute; top: 99px"
        Width="960px" Caption="Add Purchase Order" CaptionVisible="True" LoadOnDemand="true" Key="openOrders" ShowAfterLoad="true"
        AutoCallBack-Enabled="True" AutoCallBack-Target="grdOpenOrders" AutoCallBack-Command="Refresh">
        <px:PXGrid ID="grdOpenOrders" runat="server" Height="340px" Width="100%" DataSourceID="ds" Style="border-width: 1px 0px;
            left: 0px; top: 2px;" AutoAdjustColumns="true" PageSize="200" SkinID="Inquire">
            <AutoSize Enabled="true" />
            <Levels>
                <px:PXGridLevel DataMember="openOrders">
                    <Columns>
                        <px:PXGridColumn AllowCheckAll="True" AllowNull="False" AutoCallBack="True" DataField="Selected" TextAlign="Center" Type="CheckBox" />
                        <px:PXGridColumn DataField="OrderType" />
                        <px:PXGridColumn DataField="OrderNbr" />
                        <px:PXGridColumn DataField="OrderDate" />
                        <px:PXGridColumn DataField="ExpirationDate" />
                        <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="Status" />
                        <px:PXGridColumn DataField="CuryID" />
                        <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="CuryOrderTotal" TextAlign="Right" />
                        <px:PXGridColumn DataField="VendorRefNbr" />
                        <px:PXGridColumn DataField="TermsID" />
                        <px:PXGridColumn DataField="OrderDesc" />
                        <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="OpenOrderQty" TextAlign="Right" />
                        <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="CuryLeftToReceiveCost" TextAlign="Right" />
                    </Columns>
                </px:PXGridLevel>
            </Levels>
        </px:PXGrid>
        <px:PXPanel ID="PXPanel2" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton3" runat="server" DialogResult="OK" Text="Save" />
            <px:PXButton ID="PXButton4" runat="server" DialogResult="No" Text="Cancel" />
        </px:PXPanel>
    </px:PXSmartPanel>
    <px:PXSmartPanel ID="PanelFixedDemand" runat="server" Height="415px" Style="z-index: 108; left: 486px; position: absolute;
        top: 99px" Width="960px" Caption="Demand" CaptionVisible="True" LoadOnDemand="true" Key="FixedDemand" ShowAfterLoad="true"
        AutoCallBack-Enabled="True" AutoCallBack-Target="gridFixedDemand" AutoCallBack-Command="Refresh">
        <px:PXGrid ID="gridFixedDemand" runat="server" Height="340px" Width="100%" DataSourceID="ds" Style="border-width: 1px 0px;
            left: 0px; top: 2px;" AutoAdjustColumns="true" PageSize="200" SkinID="Inquire">
            <AutoSize Enabled="true" />
            <Levels>
                <px:PXGridLevel DataMember="FixedDemand">
                    <Columns>
                        <px:PXGridColumn DataField="OrderType" />
                        <px:PXGridColumn DataField="OrderNbr" />
                        <px:PXGridColumn DataField="RequestDate" />
                        <px:PXGridColumn DataField="CustomerID" />
                        <px:PXGridColumn DataField="SiteID" />
                        <px:PXGridColumn AllowUpdate="False" DataField="UOM" DisplayFormat="&gt;aaaaaa" Label="Orig. UOM" />
                        <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="OrderQty" Label="Orig. Quantity" TextAlign="Right" />
                        <px:PXGridColumn AllowUpdate="False" DataField="POUOM" DisplayFormat="&gt;aaaaaa" Label="UOM" />
                        <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="POUOMOrderQty" Label="Quantity" TextAlign="Right" />
                        <px:PXGridColumn DataField="Active" AllowNull="False" TextAlign="Center" Type="CheckBox" />
                    </Columns>
					<RowTemplate>
						<px:PXSelector ID="edOrderNbr" DataField="OrderNbr" runat="server" AllowEdit="True" />
					</RowTemplate>
                </px:PXGridLevel>
            </Levels>
        </px:PXGrid>
        <px:PXPanel ID="PXPanel3" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton5" runat="server" DialogResult="Cancel" Text="Close" />
        </px:PXPanel>
    </px:PXSmartPanel>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Document" Caption="Document Summary"
        NoteIndicator="True" FilesIndicator="True" LinkIndicator="true" ActivityIndicator="true" ActivityField="NoteActivity"
        emailinggraph="PX.Objects.CR.CREmailActivityMaint,PX.Objects" DefaultControlID="edOrderType" BPEventsIndicator="True">
        <CallbackCommands>
            <Save PostData="Self" ></Save>
        </CallbackCommands>
        <Activity HighlightColor="" SelectedColor="" Width="" Height=""></Activity>
        <Parameters>
            <px:PXControlParam ControlID="form" Name="POOrder.orderType" PropertyName="NewDataKey[&quot;OrderType&quot;]" Type="String" ></px:PXControlParam>
        </Parameters>
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="S" ></px:PXLayoutRule>
            <px:PXDropDown ID="edOrderType" runat="server" DataField="OrderType" SelectedIndex="-1"></px:PXDropDown>
            <px:PXSelector ID="edOrderNbr" runat="server" DataField="OrderNbr" AutoRefresh="true">
                <GridProperties FastFilterFields="VendorRefNbr,VendorID,VendorID_Vendor_acctName">
                    <PagerSettings Mode="NextPrevFirstLast" ></PagerSettings>
                </GridProperties>
            </px:PXSelector>
            <px:PXDropDown ID="edStatus" runat="server" AllowNull="False" DataField="Status" Enabled="False" ></px:PXDropDown>
            <px:PXDropDown ID="edBehavior" runat="server" AllowNull="False" DataField="Behavior"></px:PXDropDown>
            <px:PXCheckBox ID="chkRequestApproval" runat="server" DataField="RequestApproval" ></px:PXCheckBox>                    
            <px:PXDateTimeEdit CommitChanges="True" ID="edOrderDate" runat="server" DataField="OrderDate" ></px:PXDateTimeEdit>
            <px:PXDateTimeEdit CommitChanges="True" ID="edExpectedDate" runat="server" DataField="ExpectedDate" ></px:PXDateTimeEdit>
            <px:PXDateTimeEdit ID="edExpirationDate" runat="server" DataField="ExpirationDate" CommitChanges="true" ></px:PXDateTimeEdit>
	<px:PXLayoutRule runat="server" ID="CstLayoutRule2" ColumnSpan="2" />
	<px:PXTextEdit runat="server" ID="edUsrCustomerOrderNbr" DataField="UsrCustomerOrderNbr" />
	<px:PXLayoutRule runat="server" ID="CstLayoutRule3" />
            <px:PXLayoutRule runat="server" ColumnSpan="2" ></px:PXLayoutRule>
            <px:PXTextEdit ID="edOrderDesc" runat="server" DataField="OrderDesc" ></px:PXTextEdit>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" ></px:PXLayoutRule>
            <px:PXSegmentMask CommitChanges="True" ID="edVendorID" runat="server" DataField="VendorID" AllowAddNew="True" AllowEdit="True" AutoRefresh="True" ></px:PXSegmentMask>
            <px:PXSegmentMask CommitChanges="True" ID="edVendorLocationID" runat="server" AutoRefresh="True" DataField="VendorLocationID" ></px:PXSegmentMask>
            <px:PXSelector ID="edOwnerID" runat="server" DataField="OwnerID" TextMode="Search" DisplayMode="Text" FilterByAllFields="True" AutoRefresh="true" CommitChanges="True"></px:PXSelector>
			<px:PXSelector ID="edProjectID" runat="server" DataField="ProjectID" AllowEdit="True" CommitChanges="True" AutoRefresh="true" ></px:PXSelector>
            <pxa:PXCurrencyRate ID="edCury" DataField="CuryID" runat="server" DataSourceID="ds" RateTypeView="CurrencyInfo"
                DataMember="_Currency_" ></pxa:PXCurrencyRate>
            <px:PXTextEdit CommitChanges="True" ID="edVendorRefNbr" runat="server" DataField="VendorRefNbr" ></px:PXTextEdit>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" ></px:PXLayoutRule>
            <px:PXNumberEdit ID="edCuryDetailExtCostTotal" runat="server" DataField="CuryDetailExtCostTotal" Enabled="False" ></px:PXNumberEdit>
            <px:PXNumberEdit ID="edCuryLineDiscTotal" runat="server" DataField="CuryLineDiscTotal" Enabled="False" ></px:PXNumberEdit>
            <px:PXNumberEdit ID="edCuryDiscTot" runat="server" DataField="CuryDiscTot" CommitChanges="True" ></px:PXNumberEdit>
            <px:PXNumberEdit ID="edCuryTaxTotal" runat="server" DataField="CuryTaxTotal" Enabled="False" ></px:PXNumberEdit>
            <px:PXNumberEdit ID="edCuryOrderTotal" runat="server" DataField="CuryOrderTotal" Enabled="False" ></px:PXNumberEdit>
            <px:PXNumberEdit CommitChanges="True" ID="edCuryControlTotal" runat="server" DataField="CuryControlTotal" ></px:PXNumberEdit>
            <px:PXNumberEdit ID="edCuryRetainageTotal" runat="server" DataField="CuryRetainageTotal" Enabled="False" ></px:PXNumberEdit>
	<px:PXLayoutRule runat="server" ID="CstPXLayoutRule3" StartColumn="True" />
	<px:PXNumberEdit runat="server" ID="edUsrRTHDetailTotal" DataField="UsrRTHDetailTotal" Enabled="False" />
	<px:PXNumberEdit runat="server" ID="edUsrRTHLineDiscount" DataField="UsrRTHLineDiscount" Enabled="False" />
	<px:PXNumberEdit runat="server" ID="edUsrRTHDocDiscount" DataField="UsrRTHDocDiscount" Enabled="False" />
	<px:PXNumberEdit runat="server" ID="edUsrRTHTaxTotal" DataField="UsrRTHTaxTotal" Enabled="False" />
	<px:PXNumberEdit runat="server" ID="edUsrRTHOrderTotal" DataField="UsrRTHOrderTotal" Enabled="False" /></Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
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
    <px:PXTab ID="tab" runat="server" Height="504px" Style="z-index: 100;" Width="100%" DataSourceID="ds" DataMember="CurrentDocument">
        <ClientEvents Initialize="tabViewInit" />
        <Activity HighlightColor="" SelectedColor="" Width="" Height=""></Activity>
        <Items>
            <px:PXTabItem Text="Details">
                <Template>
                    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100; left: 0px; top: 0px; height: 384px;" Width="100%"
                        BorderWidth="0px" SkinID="Details" SyncPosition="True" Height="384px" TabIndex="2500">
                        <Levels>
                            <px:PXGridLevel DataMember="Transactions">
                                <Columns>
                                    <px:PXGridColumn DataField="BranchID" DisplayFormat="&gt;AAAAAAAAAA" RenderEditorText="True" CommitChanges="True" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="InventoryID" DisplayFormat="&gt;AAAAAAAAAA" CommitChanges="True" AllowDragDrop="true"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="SubItemID" DisplayFormat="&gt;AA-A-A" CommitChanges="True" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="IsSpecialOrder" Type="CheckBox" AllowNull="False" TextAlign="Center" ></px:PXGridColumn>
                                    <px:PXGridColumn AllowNull="False" DataField="LineType" Type="DropDownList" CommitChanges="True" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="SiteID" DisplayFormat="&gt;AAAAAAAAAA" CommitChanges="True" AllowDragDrop="true"></px:PXGridColumn>
	<px:PXGridColumn DataField="UsrItemSpecs" Width="280" />
	<px:PXGridColumn DataField="UsrPrepaymentLine" Width="60" Type="CheckBox" CommitChanges="True" />
	<px:PXGridColumn DataField="UsrVendorSpecTerms" Width="280" />
	<px:PXGridColumn DataField="UsrVendorNotes" Width="280" />
                                    <px:PXGridColumn DataField="TranDesc" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="UOM" DisplayFormat="&gt;aaaaaa" CommitChanges="True" AllowDragDrop="true"></px:PXGridColumn>
                                    <px:PXGridColumn AllowNull="False" DataField="OrderQty" TextAlign="Right" CommitChanges="True" AllowDragDrop="true"></px:PXGridColumn>
                                    <px:PXGridColumn AllowNull="False" DataField="BaseOrderQty" TextAlign="Right" ></px:PXGridColumn>
                                    <px:PXGridColumn AllowNull="False" DataField="OrderedQty" TextAlign="Right" ></px:PXGridColumn>
                                    <px:PXGridColumn AllowNull="False" DataField="NonOrderedQty" TextAlign="Right" ></px:PXGridColumn>
                                    <px:PXGridColumn AllowNull="False" DataField="ReceivedQty" TextAlign="Right" ></px:PXGridColumn>
	<px:PXGridColumn DataField="UsrSkipPrint" Width="60" Type="CheckBox" />
	<px:PXGridColumn DataField="UsrSWKRTHCost" Width="100" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryUnitCost" TextAlign="Right" CommitChanges="true" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="ManualPrice" TextAlign="Center" AllowNull="False" Type="CheckBox" CommitChanges="True"></px:PXGridColumn>   
                                    <px:PXGridColumn AllowNull="False" DataField="CuryLineAmt" TextAlign="Right" CommitChanges="true" ></px:PXGridColumn>
	<px:PXGridColumn DataField="UsrSWKSPCCode" Width="140" />
                                    <px:PXGridColumn CommitChanges="True" DataField="SOOrderNbr" AllowShowHide="true" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="SOOrderStatus" AllowShowHide="true" ></px:PXGridColumn>
	<px:PXGridColumn DataField="UsrShippingTerms" Width="70" />
                                    <px:PXGridColumn CommitChanges="True" DataField="SOLineNbr" AllowShowHide="true" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="SOLinkActive" AllowShowHide="true" Type="CheckBox" TextAlign="Center" CommitChanges="True" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="DiscPct" TextAlign="Right" ></px:PXGridColumn>
									<px:PXGridColumn DataField="CuryDiscAmt" TextAlign="Right" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="CuryDiscCost" TextAlign="Right" ></px:PXGridColumn>
									<px:PXGridColumn DataField="ManualDisc" TextAlign="Center" Type="CheckBox" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="DiscountID" RenderEditorText="True" TextAlign="Left" CommitChanges="True" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="DiscountSequenceID" TextAlign="Left" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="DisplayReqPrepaidQty" TextAlign="Right" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="CuryReqPrepaidAmt" TextAlign="Right" ></px:PXGridColumn>
                                    <px:PXGridColumn AllowNull="False" DataField="RetainagePct" TextAlign="Right" CommitChanges="true" ></px:PXGridColumn>
                                    <px:PXGridColumn AllowNull="False" DataField="CuryRetainageAmt" TextAlign="Right" CommitChanges="true" ></px:PXGridColumn>
                                    <px:PXGridColumn AllowNull="False" DataField="CuryExtCost" TextAlign="Right" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="AlternateID" CommitChanges="true" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="LotSerialNbr" AllowShowHide="Server" ></px:PXGridColumn>
                                    <px:PXGridColumn AllowNull="False" DataField="RcptQtyMin" TextAlign="Right" ></px:PXGridColumn>
                                    <px:PXGridColumn AllowNull="False" DataField="RcptQtyMax" TextAlign="Right" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="RcptQtyThreshold" TextAlign="Right" ></px:PXGridColumn>
                                    <px:PXGridColumn AllowNull="False" DataField="RcptQtyAction" Type="DropDownList" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="TaxCategoryID" DisplayFormat="&gt;aaaaaaaaaa" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="ExpenseAcctID" DisplayFormat="&gt;######" CommitChanges="True" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="ExpenseAcctID_Account_description" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="ExpenseSubID" DisplayFormat="&gt;AA-AA-AA-AA-AAA" ></px:PXGridColumn>
									<px:PXGridColumn DataField="POAccrualAcctID" AutoCallBack="True"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="POAccrualSubID" CommitChanges="True" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="ProjectID" Label="Project" CommitChanges="True" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="TaskID" DisplayFormat="&gt;AAAAAAAAAA" Label="Task" CommitChanges="True" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="CostCodeID" CommitChanges="True"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="OrderType" Visible="False" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="LineNbr" TextAlign="Right" Visible="False" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="SortOrder" TextAlign="Right" Visible="False" ></px:PXGridColumn>
	<px:PXGridColumn DataField="UsrVendorID" Width="140" />
	<px:PXGridColumn DataField="UsrVendorLocationID" Width="70" />
	<px:PXGridColumn DataField="UsrVendorAddress" Width="280" />
                                    <px:PXGridColumn DataField="RequestedDate" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="PromisedDate" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="DRTermStartDate" CommitChanges="true" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="DRTermEndDate" CommitChanges="true" ></px:PXGridColumn>
									<px:PXGridColumn DataField="CompletePOLine" Visible="False" ></px:PXGridColumn>
                                    <px:PXGridColumn AllowNull="False" DataField="Completed" TextAlign="Center" Type="CheckBox" ></px:PXGridColumn>
                                    <px:PXGridColumn AllowNull="False" DataField="Cancelled" TextAlign="Center" Type="CheckBox" ></px:PXGridColumn>
                                    <px:PXGridColumn AllowNull="False" DataField="Closed" TextAlign="Center" Type="CheckBox" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="BilledQty" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="CuryBilledAmt" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="UnbilledQty" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="CuryUnbilledAmt" ></px:PXGridColumn>
                                    <px:PXGridColumn AllowUpdate="False" DataField="POType" RenderEditorText="True" ></px:PXGridColumn>
                                    <px:PXGridColumn AllowUpdate="False" DataField="PONbr" DisplayFormat="&gt;CCCCCCCCCCCCCCC" LinkCommand="ViewBlanketOrder" ></px:PXGridColumn>
                                    <px:PXGridColumn AllowUpdate="False" DataField="POAccrualType" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="HasExpiredComplianceDocuments" Type="CheckBox" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="ViewDemandEnabled" Type="CheckBox" AllowShowHide="False" Visible="false" SyncVisible="false" ></px:PXGridColumn></Columns>
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" ></px:PXLayoutRule>
                                    <px:PXSegmentMask CommitChanges="True" ID="edInventoryID" runat="server" DataField="InventoryID" AllowEdit="True" 
                                        AutoRefresh="True">
                                        <Parameters>
                                            <px:PXControlParam ControlID="grid" Name="POLine.lineType" PropertyName="DataValues[&quot;LineType&quot;]" Type="String" ></px:PXControlParam>
                                        </Parameters>
                                    </px:PXSegmentMask>
                                    <px:PXSegmentMask CommitChanges="True" ID="edSubItemID" runat="server" DataField="SubItemID" AutoRefresh="true">
                                        <Parameters>
                                            <px:PXControlParam ControlID="grid" Name="POLine.inventoryID" PropertyName="DataValues[&quot;InventoryID&quot;]" Type="String" ></px:PXControlParam>
                                        </Parameters>
                                    </px:PXSegmentMask>
                                    <px:PXSegmentMask CommitChanges="True" ID="edSiteID" runat="server" DataField="SiteID" AutoRefresh="True" ></px:PXSegmentMask>
                                    <px:PXDropDown CommitChanges="True" ID="edLineType" runat="server" AllowNull="False" DataField="LineType" ></px:PXDropDown>
                                    <px:PXSelector CommitChanges="True" ID="edUOM" runat="server" DataField="UOM" AutoRefresh="True">
                                        <Parameters>
                                            <px:PXSyncGridParam ControlID="grid" ></px:PXSyncGridParam>
                                        </Parameters>
                                    </px:PXSelector>
                                    <px:PXNumberEdit ID="edOrderQty" runat="server" DataField="OrderQty" ></px:PXNumberEdit>
                                    <px:PXNumberEdit ID="edOrderedQty" runat="server" DataField="OrderedQty" ></px:PXNumberEdit>
                                    <px:PXNumberEdit ID="edNonOrderedQty" runat="server" DataField="NonOrderedQty" ></px:PXNumberEdit>
                                    <px:PXNumberEdit ID="edReceivedQty" runat="server" DataField="ReceivedQty" Enabled="False" ></px:PXNumberEdit>
                                    <px:PXNumberEdit ID="edCuryUnitCost" runat="server" DataField="CuryUnitCost" CommitChanges="true" ></px:PXNumberEdit>
                                    <px:PXCheckBox ID="chkManualPrice" runat="server" DataField="ManualPrice" CommitChanges="True" ></px:PXCheckBox>
                                    <px:PXSelector ID="edDiscountCode" runat="server" DataField="DiscountID" CommitChanges="True" AllowEdit="True" edit="1" ></px:PXSelector>
                                    <px:PXNumberEdit ID="edDiscPct" runat="server" DataField="DiscPct" ></px:PXNumberEdit>
									<px:PXNumberEdit ID="edCuryDiscAmt" runat="server" DataField="CuryDiscAmt" ></px:PXNumberEdit>
									<px:PXCheckBox ID="chkManualDisc" runat="server" DataField="ManualDisc" CommitChanges="True" ></px:PXCheckBox>
                                    <px:PXNumberEdit ID="edCuryLineAmt" runat="server" DataField="CuryLineAmt" CommitChanges="true" ></px:PXNumberEdit>
                                    <px:PXNumberEdit ID="edCuryExtCost" runat="server" DataField="CuryExtCost" ></px:PXNumberEdit>
                                    <px:PXLayoutRule runat="server" ColumnSpan="2" ></px:PXLayoutRule>
                                    <px:PXTextEdit ID="edTranDesc" runat="server" DataField="TranDesc" ></px:PXTextEdit>

                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="S" ></px:PXLayoutRule>
                                    <px:PXNumberEdit ID="edRcptQtyMin" runat="server" DataField="RcptQtyMin" ></px:PXNumberEdit>
                                    <px:PXNumberEdit ID="edRcptQtyMax" runat="server" DataField="RcptQtyMax" ></px:PXNumberEdit>
                                    <px:PXNumberEdit ID="edRcptQtyThreshold" runat="server" DataField="RcptQtyThreshold" ></px:PXNumberEdit>
                                    <px:PXDropDown ID="edRcptQtyAction" runat="server" DataField="RcptQtyAction" ></px:PXDropDown>
									<px:PXCheckBox ID="chkCompleted" runat="server" DataField="Completed" ></px:PXCheckBox>
                                    <px:PXCheckBox ID="chkCancelled" runat="server" DataField="Cancelled" ></px:PXCheckBox>
									<px:PXCheckBox ID="chkClosed" runat="server" DataField="Closed" ></px:PXCheckBox>
                                    <px:PXNumberEdit ID="edBilledQty" runat="server" DataField="BilledQty" ></px:PXNumberEdit>
                                    <px:PXNumberEdit ID="edCuryBilledAmt" runat="server" DataField="CuryBilledAmt" ></px:PXNumberEdit>
                                    <px:PXNumberEdit ID="edUnbilledQty" runat="server" DataField="UnbilledQty" ></px:PXNumberEdit>
                                    <px:PXNumberEdit ID="edCuryUnbilledAmt" runat="server" DataField="CuryUnbilledAmt" ></px:PXNumberEdit>

                                    <px:PXSelector runat="server" DataField="SOOrderNbr" AllowEdit="true" ID="edDSSOOrderNbr" ></px:PXSelector>
                                    <px:PXDropDown runat="server" DataField="SOOrderStatus" ID="edSOOrderStatus" ></px:PXDropDown>
                                    <px:PXNumberEdit runat="server" DataField="SOLineNbr" ID="edDSSOLineNbr" ></px:PXNumberEdit>
                                    <px:PXCheckBox runat="server" DataField="SOLinkActive" CommitChanges="True" ID="edSOLinkActive" ></px:PXCheckBox>

                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" ></px:PXLayoutRule>
                                    <px:PXSegmentMask CommitChanges="True" Height="19px" ID="edBranchID" runat="server" DataField="BranchID" AutoRefresh="true" ></px:PXSegmentMask>
                                    <px:PXSegmentMask CommitChanges="True" ID="edExpenseAcctID" runat="server" DataField="ExpenseAcctID" 
                                        AutoRefresh="True" ></px:PXSegmentMask>
                                    <px:PXSegmentMask ID="edExpenseSubID" runat="server" DataField="ExpenseSubID" AutoRefresh="True">
                                        <Parameters>
                                            <px:PXSyncGridParam ControlID="grid" ></px:PXSyncGridParam>
                                        </Parameters>
                                    </px:PXSegmentMask>
									<px:PXSegmentMask CommitChanges="True" ID="edPOAccrualAcctID" runat="server" DataField="POAccrualAcctID" AutoRefresh="true" ></px:PXSegmentMask>
									<px:PXSegmentMask CommitChanges="True" ID="edPOAccrualSubID" runat="server" DataField="POAccrualSubID" AutoRefresh="true" ></px:PXSegmentMask>
                                    <px:PXDateTimeEdit ID="edRequestedDate" runat="server" DataField="RequestedDate" ></px:PXDateTimeEdit>
                                    <px:PXDateTimeEdit ID="edPromisedDate" runat="server" DataField="PromisedDate" ></px:PXDateTimeEdit>
                                     <px:PXDateTimeEdit ID="edDRTermStartDate" runat="server" DataField="DRTermStartDate" CommitChanges="true" ></px:PXDateTimeEdit>
                                    <px:PXDateTimeEdit ID="edDRTermEndDate" runat="server" DataField="DRTermEndDate" CommitChanges="true" ></px:PXDateTimeEdit>
                                    <px:PXSelector ID="edTaxCategoryID" runat="server" DataField="TaxCategoryID" AutoRefresh="True"></px:PXSelector>
                                    <px:PXTextEdit ID="edAlternateID" runat="server" DataField="AlternateID" ></px:PXTextEdit>
                                    <px:PXSegmentMask CommitChanges="True" ID="edProjectID" runat="server" DataField="ProjectID" ></px:PXSegmentMask>
                                    <px:PXSegmentMask CommitChanges="True" ID="edTaskID" runat="server" DataField="TaskID" AutoRefresh="True">
                                        <Parameters>
                                            <px:PXSyncGridParam ControlID="grid" ></px:PXSyncGridParam>
                                        </Parameters>
                                    </px:PXSegmentMask>
                                    <px:PXSegmentMask ID="edCostCode" runat="server" DataField="CostCodeID" AutoRefresh="True" AllowAddNew="true" ></px:PXSegmentMask>
                                    <px:PXDropDown ID="edPOType" runat="server" DataField="POType" ></px:PXDropDown>
                                    <px:PXSelector ID="edPONbr" runat="server" DataField="PONbr" ></px:PXSelector>
                                    <px:PXDropDown ID="edPOAccrualType" runat="server" DataField="POAccrualType" ></px:PXDropDown>
                                   
                                </RowTemplate>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" MinHeight="150" />
                        <CallbackCommands PasteCommand="PasteLine">
                            <Save PostData="Container" />
                        </CallbackCommands>
                        <Mode InitNewRow="True" AllowFormEdit="True" AllowUpload="True" AllowDragRows="true"/>
                        <ActionBar>
                            <CustomItems>
                                <px:PXToolBarButton Text="Add Items" Key="cmdASI">
                                    <AutoCallBack Command="ShowItems" Target="ds">
                                        <Behavior PostData="Page" CommitChanges="True" />
                                    </AutoCallBack>
                                </px:PXToolBarButton>
								<px:PXToolBarButton Text="Add Matrix Items" CommandSourceID="ds" CommandName="ShowMatrixPanel" />
                                <px:PXToolBarButton Text="Add Project Item" Key="cmdAddProjectItem">
                                    <AutoCallBack Command="AddProjectItem" Target="ds">
                                        <Behavior PostData="Page" CommitChanges="True" />
                                    </AutoCallBack>
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Text="Add Order" Key="cmdPO" CommandSourceID="ds" CommandName="AddPOOrder" />
                                <px:PXToolBarButton Text="Add Order Line" Key="cmdAddPOLine" CommandSourceID="ds" CommandName="AddPOOrderLine" />
                                <px:PXToolBarButton Text="View Demand" DependOnGrid="grid" StateColumn="ViewDemandEnabled">
                                    <AutoCallBack Command="ViewDemand" Target="ds">
                                        <Behavior PostData="Page" CommitChanges="True" />
                                    </AutoCallBack>
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
                            </CustomItems>
                        </ActionBar>
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Taxes">
                <Template>
                    <px:PXGrid ID="gridTaxes" runat="server" DataSourceID="ds" Height="150px" Style="z-index: 100" Width="100%" ActionsPosition="Top"
                        BorderWidth="0px" SkinID="Details">
                        <AutoSize Enabled="True" MinHeight="150" />
                        <ActionBar>
                            <Actions>
                                <Search Enabled="False" />
                                <Save Enabled="False" />
                                <EditRecord Enabled="False" />
                            </Actions>
                        </ActionBar>
                        <Levels>
                            <px:PXGridLevel DataMember="Taxes">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                                    <px:PXSelector SuppressLabel="True" ID="edTaxID" runat="server" DataField="TaxID" CommitChanges="true" AutoRefresh="true"/>
                                    <px:PXNumberEdit SuppressLabel="True" ID="edTaxRate" runat="server" DataField="TaxRate" Enabled="False" />
                                    <px:PXNumberEdit SuppressLabel="True" ID="edCuryTaxableAmt" runat="server" DataField="CuryTaxableAmt" />
                                    <px:PXNumberEdit SuppressLabel="True" ID="edCuryTaxAmt" runat="server" DataField="CuryTaxAmt" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="TaxID" AllowUpdate="False" CommitChanges="true"/>
                                    <px:PXGridColumn AllowNull="False" AllowUpdate="False" DataField="TaxRate" TextAlign="Right" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryTaxableAmt" TextAlign="Right" />
									<px:PXGridColumn DataField="TaxUOM" TextAlign="Right" />
									<px:PXGridColumn DataField="TaxableQty" TextAlign="Right" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryTaxAmt" TextAlign="Right" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryRetainedTaxableAmt" TextAlign="Right" />
                                    <px:PXGridColumn AllowNull="False" DataField="CuryRetainedTaxAmt" TextAlign="Right" />
                                    <px:PXGridColumn DataField="NonDeductibleTaxRate" TextAlign="Right" />
                                    <px:PXGridColumn DataField="CuryExpenseAmt" TextAlign="Right" />
                                    <px:PXGridColumn AllowNull="False" DataField="Tax__TaxType" Label="Tax Type" RenderEditorText="True" />
                                    <px:PXGridColumn AllowNull="False" DataField="Tax__PendingTax" Label="Pending VAT" TextAlign="Center" Type="CheckBox" />
                                    <px:PXGridColumn AllowNull="False" DataField="Tax__ReverseTax" Label="Reverse VAT" TextAlign="Center" Type="CheckBox" />
                                    <px:PXGridColumn AllowNull="False" DataField="Tax__ExemptTax" Label="Exempt From VAT" TextAlign="Center" Type="CheckBox" />
                                    <px:PXGridColumn AllowNull="False" DataField="Tax__StatisticalTax" Label="Statistical VAT" TextAlign="Center" Type="CheckBox" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Shipping">
                <Template>
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" ></px:PXLayoutRule>
                    <px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Ship To:" ></px:PXLayoutRule>
                    <px:PXDropDown CommitChanges="True" ID="edShipDestType" runat="server" AllowNull="False" DataField="ShipDestType" 
                        SuppressLabel="False" ></px:PXDropDown>
                    <px:PXSelector CommitChanges="True" ID="edShipToBAccountID" runat="server" DataField="ShipToBAccountID" 
                        AutoRefresh="True"  SuppressLabel="False"></px:PXSelector>
                    <px:PXSegmentMask CommitChanges="True" SuppressLabel="False" ID="edSiteID" runat="server" DataField="SiteID" ></px:PXSegmentMask>
                    <px:PXSegmentMask CommitChanges="True" ID="edShipToLocationID" runat="server" AutoRefresh="True" DataField="ShipToLocationID" ></px:PXSegmentMask>
                    <px:PXSelector CommitChanges="True" ID="edSOOrderType" runat="server" DataField="SOOrderType" ></px:PXSelector>
                    <px:PXSelector ID="edSOOrderNbr" runat="server" AllowEdit="True" DataField="SOOrderNbr" ></px:PXSelector>
                    <px:PXFormView ID="formSC" runat="server" Caption="Ship-To Contact" DataMember="Shipping_Contact" DataSourceID="ds" RenderStyle="Fieldset">
                        <Template>
                            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" ></px:PXLayoutRule>
                            <px:PXCheckBox CommitChanges="True" SuppressLabel="True" ID="chkOverrideContact" runat="server" DataField="OverrideContact" ></px:PXCheckBox>
                            <px:PXTextEdit ID="edFullName" runat="server" DataField="FullName" ></px:PXTextEdit>
                            <px:PXTextEdit ID="edAttention" runat="server" DataField="Attention" ></px:PXTextEdit>
                            <px:PXMaskEdit ID="edPhone1" runat="server" DataField="Phone1" ></px:PXMaskEdit>
                            <px:PXMailEdit ID="edEmail" runat="server" DataField="Email" CommitChanges="True"></px:PXMailEdit>
                        </Template>
                        <ContentStyle BackColor="Transparent" BorderStyle="None">
                        </ContentStyle>
                        <AutoSize MinWidth="100" ></AutoSize>
                    </px:PXFormView>
                    <px:PXFormView ID="formSA" DataMember="Shipping_Address" runat="server" DataSourceID="ds" Caption="Ship-To Address" SyncPosition="True" RenderStyle="Fieldset">
                        <AutoSize MinHeight="100" MinWidth="100" ></AutoSize>
                        <Template>
                            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" ></px:PXLayoutRule>
                            <px:PXCheckBox CommitChanges="True" SuppressLabel="True" ID="chkOverrideAddress" runat="server" DataField="OverrideAddress" ></px:PXCheckBox>
                            <px:PXButton ID="btnAddressLookup" runat="server" CommandName="AddressLookup" CommandSourceID="ds" Size="xs" TabIndex="-1" ></px:PXButton>
                            <px:PXTextEdit ID="edAddressLine1" runat="server" DataField="AddressLine1" ></px:PXTextEdit>
                            <px:PXTextEdit ID="edAddressLine2" runat="server" DataField="AddressLine2" ></px:PXTextEdit>
                            <px:PXTextEdit ID="edCity" runat="server" DataField="City" ></px:PXTextEdit>
                            <px:PXSelector ID="edCountryID" runat="server" DataField="CountryID" AutoRefresh="True" DataSourceID="ds" CommitChanges="true" ></px:PXSelector>
                            <px:PXSelector ID="edState" runat="server" DataField="State" AutoRefresh="True" DataSourceID="ds">
                                <CallBackMode PostData="Container" ></CallBackMode>
                                <Parameters>
                                    <px:PXControlParam ControlID="formSA" Name="POShipAddress.countryID" PropertyName="DataControls[&quot;edCountryID&quot;].Value"
                                        Type="String" ></px:PXControlParam>
                                </Parameters>
                            </px:PXSelector>
                            <px:PXMaskEdit ID="edPostalCode" runat="server" DataField="PostalCode" CommitChanges="true" ></px:PXMaskEdit>
                            <px:PXCheckBox ID="chkIsValidated" runat="server" DataField="IsValidated" Enabled="False"></px:PXCheckBox>
                        </Template>
                        <ContentStyle BackColor="Transparent" BorderStyle="None" ></ContentStyle>
                    </px:PXFormView>
                    <px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="SM" StartColumn="True" ></px:PXLayoutRule>
                    <px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Ship Via:" ></px:PXLayoutRule>
                    <px:PXSelector ID="edFOBPoint" runat="server" DataField="FOBPoint" ></px:PXSelector>
                    <px:PXSelector ID="edShipVia" runat="server" DataField="ShipVia" ></px:PXSelector>
	<px:PXNumberEdit runat="server" ID="edUsrFreightCost" DataField="UsrFreightCost" />
	<px:PXSelector runat="server" ID="edUsrShipTermsID" DataField="UsrShipTermsID" />
	<px:PXNumberEdit runat="server" ID="edUsrFreightPrice" DataField="UsrFreightPrice" />
	<px:PXTextEdit runat="server" ID="edUsrCustomerAccount" DataField="UsrCustomerAccount" />
	<px:PXTextEdit runat="server" ID="edUsrShippingInstructions" DataField="UsrShippingInstructions" Height="100" TextMode="MultiLine" /></Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Vendor Info">
                <Template>
                    <px:PXLayoutRule runat="server" StartColumn="True" SuppressLabel="True" />
                    <px:PXLayoutRule runat="server" StartGroup="True" />
                    <px:PXFormView ID="formVC" runat="server" Caption="Vendor Contact" DataMember="Remit_Contact" DataSourceID="ds" RenderStyle="Fieldset">
                        <Template>
                            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                            <px:PXCheckBox CommitChanges="True" SuppressLabel="True" ID="chkOverrideContact" runat="server" DataField="OverrideContact" />
                            <px:PXTextEdit ID="edFullName" runat="server" DataField="FullName" />
                            <px:PXTextEdit ID="edAttention" runat="server" DataField="Attention" />
                            <px:PXMaskEdit ID="edPhone1" runat="server" DataField="Phone1" />
                            <px:PXMailEdit ID="edEmail" runat="server" DataField="Email" CommitChanges="True"/>
                        </Template>
                        <ContentStyle BackColor="Transparent" BorderStyle="None" />
                    </px:PXFormView>
                    <px:PXFormView ID="formVA" DataMember="Remit_Address" runat="server" Caption="Vendor Address" DataSourceID="ds" SyncPosition="True" RenderStyle="Fieldset">
                        <Template>
                            <px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="XM" />
                            <px:PXCheckBox CommitChanges="True" SuppressLabel="True" ID="chkOverrideAddress" runat="server" DataField="OverrideAddress" />
                            <px:PXButton ID="btnRemitAddressLookup" runat="server" CommandName="RemitAddressLookup" CommandSourceID="ds" Size="xs" TabIndex="-1" />
                            <px:PXTextEdit ID="edAddressLine1" runat="server" DataField="AddressLine1" />
                            <px:PXTextEdit ID="edAddressLine2" runat="server" DataField="AddressLine2" />
                            <px:PXTextEdit ID="edCity" runat="server" DataField="City" />
                            <px:PXSelector ID="edCountryID" runat="server" DataField="CountryID" AutoRefresh="True" DataSourceID="ds" CommitChanges="true" />
                            <px:PXSelector ID="edState" runat="server" DataField="State" AutoRefresh="True" DataSourceID="ds">
                                <CallBackMode PostData="Container" />
                                <Parameters>
                                    <px:PXControlParam ControlID="formVA" Name="PORemitAddress.countryID" PropertyName="DataControls[&quot;edCountryID&quot;].Value"
                                                       Type="String" />
                                </Parameters>
                            </px:PXSelector>
                            <px:PXMaskEdit ID="edPostalCode" runat="server" DataField="PostalCode" CommitChanges="true" />
                            <px:PXCheckBox ID="chkIsValidated" runat="server" DataField="IsValidated" Enabled="False"/>
                        </Template>
                        <ContentStyle BackColor="Transparent" BorderStyle="None" />
                    </px:PXFormView>
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                    <px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Info" />
                    <px:PXSelector CommitChanges="True" ID="edTermsID" runat="server" DataField="TermsID" />
                    <px:PXSelector CommitChanges="True" ID="edTaxZoneID" runat="server" DataField="TaxZoneID" Text="ZONE1" />
                    <px:PXDropDown CommitChanges="True" ID="edTaxCalcMode" runat="server" DataField="TaxCalcMode" />
					<px:PXSegmentMask CommitChanges="True" ID="edPayToVendorID" runat="server" DataField="PayToVendorID" AllowEdit="True" AutoRefresh="True" />
					<px:PXNumberEdit ID="edPrepaymentPct" runat="server" DataField="PrepaymentPct" AllowNull="True" />
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Approvals" BindingContext="form" VisibleExp="DataControls[&quot;chkRequestApproval&quot;].Value = 1">
                <Template>
                    <px:PXGrid ID="gridApproval" runat="server" DataSourceID="ds" Width="100%" SkinID="DetailsInTab" NoteIndicator="True" Style="left: 0px;
                        top: 0px;">
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
                                    <px:PXGridColumn DataField="Reason" AllowUpdate="False" Width="160px" />
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
            <px:PXTabItem Text="Discounts">
                <Template>
                    <px:PXGrid ID="formDiscountDetail" runat="server" DataSourceID="ds" Width="100%" SkinID="Details" BorderStyle="None">
                        <Levels>
                            <px:PXGridLevel DataMember="DiscountDetails">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                                    <px:PXCheckBox ID="chkSkipDiscount" runat="server" DataField="SkipDiscount" />
                                    <px:PXSelector ID="edDiscountID" runat="server" DataField="DiscountID" 
                                        AllowEdit="True" edit="1" />
                                    <px:PXDropDown ID="edType" runat="server" DataField="Type" Enabled="False" />
                                    <px:PXCheckBox ID="chkIsManual" runat="server" DataField="IsManual" />
                                    <px:PXSelector ID="edDiscountSequenceID" runat="server" DataField="DiscountSequenceID" AllowEdit="True" AutoRefresh="True" edit="1" />
                                    <px:PXNumberEdit ID="edCuryDiscountableAmt" runat="server" DataField="CuryDiscountableAmt" />
                                    <px:PXNumberEdit ID="edDiscountableQty" runat="server" DataField="DiscountableQty" />
                                    <px:PXNumberEdit ID="edCuryDiscountAmt" runat="server" DataField="CuryDiscountAmt" CommitChanges="true" />
                                    <px:PXNumberEdit ID="edDiscountPct" runat="server" DataField="DiscountPct" CommitChanges="true" />
                                    <px:PXTextEdit ID="edExtDiscCode" runat="server" DataField="ExtDiscCode" />
                                    <px:PXTextEdit ID="edDescription" runat="server" DataField="Description" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="SkipDiscount" Type="CheckBox" TextAlign="Center" />
                                    <px:PXGridColumn DataField="DiscountID" CommitChanges="True" />
                                    <px:PXGridColumn DataField="DiscountSequenceID" CommitChanges="True" />
                                    <px:PXGridColumn DataField="Type" RenderEditorText="True" />
                                    <px:PXGridColumn DataField="IsManual" Type="CheckBox" TextAlign="Center" />
                                    <px:PXGridColumn DataField="CuryDiscountableAmt" TextAlign="Right" />
                                    <px:PXGridColumn DataField="DiscountableQty" TextAlign="Right" />
                                    <px:PXGridColumn DataField="CuryDiscountAmt" TextAlign="Right" CommitChanges="true" />
                                    <px:PXGridColumn DataField="CuryRetainedDiscountAmt" TextAlign="Right" />
                                    <px:PXGridColumn DataField="DiscountPct" TextAlign="Right" CommitChanges="true" />
                                    <px:PXGridColumn DataField="ExtDiscCode" />
                                    <px:PXGridColumn DataField="Description" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" MinHeight="150" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
			<px:PXTabItem Text="PO History" RepaintOnDemand="False">
				<Template>
                    <px:PXSplitContainer runat="server" ID="sp1" SplitterPosition="620" SkinID="Transparent" Height="500px" DisabledPanel="Panel1">
					<AutoSize Enabled="true" />
					<Template1>
						<px:PXGrid ID="formReceipts" runat="server" DataSourceID="ds" Width="100%" SkinID="DetailsInTab" BorderStyle="None" StatusField="StatusText" AdjustPageSize="Auto">
							<Levels>
								<px:PXGridLevel DataMember="Receipts">
									<RowTemplate>
										<px:PXSelector SuppressLabel="True" Size="s" ID="edReceiptNbr" runat="server" DataField="ReceiptNbr" AutoRefresh="True" AllowEdit="True" edit="1" />
									</RowTemplate>
									<Columns>
										<px:PXGridColumn DataField="ReceiptType" CommitChanges="True" />
										<px:PXGridColumn DataField="ReceiptNbr" CommitChanges="True" />
										<px:PXGridColumn DataField="DocDate" RenderEditorText="True" />
										<px:PXGridColumn DataField="Status" RenderEditorText="True" />
										<px:PXGridColumn DataField="TotalQty" />
									</Columns>
								</px:PXGridLevel>
							</Levels>
							<AutoSize Enabled="True" MinHeight="150" />
							<Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
						</px:PXGrid>
						<px:PXGrid ID="formChildOrders" runat="server" DataSourceID="ds" Width="100%" SkinID="DetailsInTab" BorderStyle="None" StatusField="StatusText" AdjustPageSize="Auto">
							<Levels>
								<px:PXGridLevel DataMember="ChildOrdersReceipts">
									<RowTemplate>
                                        <px:PXSelector SuppressLabel="True" Size="s" ID="edChildOrdersReceiptsOrderNbr" runat="server" DataField="OrderNbr" AutoRefresh="True" AllowEdit="True" edit="1" />
										<px:PXSelector SuppressLabel="True" Size="s" ID="edChildOrdersReceiptsReceiptNbr" runat="server" DataField="POBlanketOrderPOReceipt__ReceiptNbr" AutoRefresh="True" AllowEdit="True" edit="1" />
									</RowTemplate>
									<Columns>
                                        <px:PXGridColumn DataField="OrderType" />
										<px:PXGridColumn DataField="OrderNbr" />
                                        <px:PXGridColumn DataField="OrderDate" RenderEditorText="True" />
										<px:PXGridColumn DataField="Status" RenderEditorText="True" />
										<px:PXGridColumn DataField="TotalQty" />

										<px:PXGridColumn DataField="POBlanketOrderPOReceipt__ReceiptType" />
                                        <px:PXGridColumn DataField="POBlanketOrderPOReceipt__ReceiptNbr">
										    <NavigateParams>
											    <px:PXControlParam ControlID="formChildOrders" Name="ReceiptType" PropertyName="DataValues[&quot;POBlanketOrderPOReceipt__ReceiptType&quot;]" Type="String" />
											    <px:PXControlParam ControlID="formChildOrders" Name="ReceiptNbr" PropertyName="DataValues[&quot;POBlanketOrderPOReceipt__ReceiptNbr&quot;]" Type="String" />
										    </NavigateParams>
                                        </px:PXGridColumn>
                                        <px:PXGridColumn DataField="POBlanketOrderPOReceipt__ReceiptDate" />
                                        <px:PXGridColumn DataField="POBlanketOrderPOReceipt__Status" />
                                        <px:PXGridColumn DataField="POBlanketOrderPOReceipt__TotalQty" />
									</Columns>
								</px:PXGridLevel>
							</Levels>
							<AutoSize Enabled="True" MinHeight="150" />
							<Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
						</px:PXGrid>
					</Template1>
					<Template2>
						<px:PXGrid ID="formAPDocs" runat="server" DataSourceID="ds" Width="100%" SkinID="DetailsInTab" BorderStyle="None" StatusField="StatusText" AdjustPageSize="Auto">
							<Levels>
								<px:PXGridLevel DataMember="APDocs" >
									<RowTemplate>
										<px:PXSelector SuppressLabel="True" Size="s" ID="edRefNbr" runat="server" DataField="RefNbr" AutoRefresh="True" AllowEdit="True" edit="1" />
									</RowTemplate>
									<Columns>
										<px:PXGridColumn DataField="DocType" />
										<px:PXGridColumn DataField="RefNbr" />
										<px:PXGridColumn DataField="DocDate" />
										<px:PXGridColumn DataField="Status" />
										<px:PXGridColumn DataField="TotalQty" />
										<px:PXGridColumn DataField="TotalAmt" />
										<px:PXGridColumn DataField="TotalPPVAmt" />
										<px:PXGridColumn DataField="CuryID" />
									</Columns>
								</px:PXGridLevel>
							</Levels>
							<AutoSize Enabled="True" MinHeight="150" />
							<Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
						</px:PXGrid>
						<px:PXGrid ID="formChildOrdersAPDocs" runat="server" DataSourceID="ds" Width="100%" SkinID="DetailsInTab" BorderStyle="None" StatusField="StatusText" AdjustPageSize="Auto">
							<Levels>
								<px:PXGridLevel DataMember="ChildOrdersAPDocs" >
									<RowTemplate>
										<px:PXSelector SuppressLabel="True" Size="s" ID="edChildOrdersReceiptsRefNbr" runat="server" DataField="RefNbr" AutoRefresh="True" AllowEdit="True" edit="1" />
									</RowTemplate>
									<Columns>
										<px:PXGridColumn DataField="DocType" />
										<px:PXGridColumn DataField="RefNbr" />
										<px:PXGridColumn DataField="DocDate" />
										<px:PXGridColumn DataField="Status" />
										<px:PXGridColumn DataField="TotalQty" />
										<px:PXGridColumn DataField="TotalAmt" />
										<px:PXGridColumn DataField="TotalPPVAmt" />
										<px:PXGridColumn DataField="CuryID" />
									</Columns>
								</px:PXGridLevel>
							</Levels>
							<AutoSize Enabled="True" MinHeight="150" />
							<Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
						</px:PXGrid>
					</Template2>
				</px:PXSplitContainer>
			</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Prepayments" LoadOnDemand="true" VisibleExp="DataControls[&quot;edOrderType&quot;].Value != SB" BindingContext="form">
				<Template>
					<px:PXGrid ID="PrepaymentGrid" runat="server" Height="350px" Width="100%" AllowPaging="True" AdjustPageSize="Auto" AllowSearch="true" DataSourceID="ds"
						SkinID="DetailsInTab" AllowFilter="true" StatusField="StatusText">
						<Levels>
							<px:PXGridLevel DataMember="PrepaymentDocuments">
								<RowTemplate>
									<px:PXSelector ID="edPrepaymentRefNbr" runat="server" DataField="APRefNbr" AllowEdit="True" />
									<px:PXSelector ID="edPaymentRefNbr" runat="server" DataField="PayRefNbr" AllowEdit="True" />
								</RowTemplate>
								<Columns>
									<px:PXGridColumn DataField="APDocType" />
									<px:PXGridColumn DataField="APRefNbr" />
									<px:PXGridColumn DataField="APRegister__DocDate" />
									<px:PXGridColumn DataField="CuryAppliedAmt" />
									<px:PXGridColumn DataField="APRegister__CuryDocBal" />
									<px:PXGridColumn DataField="APRegister__Status" />
									<px:PXGridColumn DataField="APRegister__CuryID" />
									<px:PXGridColumn DataField="PayRefNbr" />
								</Columns>
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" />
						<Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
            <px:PXTabItem Text="Change Orders" LoadOnDemand="true">
                <Template>
                    <px:PXGrid ID="ChangeOrdersGrid" runat="server" Height="350px" Width="100%" Style="z-index: 100" AllowPaging="True" AdjustPageSize="Auto" AllowSearch="true" DataSourceID="ds"
                        SkinID="Inquire" AllowFilter="true" SyncPosition="true" >
                        <Levels>
                            <px:PXGridLevel DataMember="ChangeOrderDetails">
                                <Columns>
                                    <px:PXGridColumn DataField="PMChangeOrder__RefNbr" LinkCommand="ViewChangeOrder" ViewName="ChangeOrder" />
                                    <px:PXGridColumn DataField="PMChangeOrder__ClassID" />
                                    <px:PXGridColumn DataField="PMChangeOrder__ProjectNbr" />
                                    <px:PXGridColumn DataField="PMChangeOrder__Status" />
                                    <px:PXGridColumn DataField="PMChangeOrder__Description" />
                                    <px:PXGridColumn DataField="PMChangeOrder__Date" />
                                    <px:PXGridColumn DataField="PMChangeOrder__CompletionDate" />
                                    <px:PXGridColumn DataField="PMChangeOrder__DelayDays" />
                                    <px:PXGridColumn DataField="PMChangeOrder__ReversingRefNbr" LinkCommand="ViewReversingChangeOrders" />
                                    <px:PXGridColumn DataField="PMChangeOrder__OrigRefNbr" LinkCommand="ViewOrigChangeOrder" />
                                    <px:PXGridColumn DataField="PMChangeOrder__ExtRefNbr" />
                                    <px:PXGridColumn DataField="PMChangeOrderLine__ProjectID" />
                                    <px:PXGridColumn DataField="PMChangeOrderLine__TaskID" />
                                    <px:PXGridColumn DataField="PMChangeOrderLine__InventoryID" />
                                    <px:PXGridColumn DataField="PMChangeOrderLine__CostCodeID" />
                                    <px:PXGridColumn DataField="PMChangeOrderLine__Description" />
                                    <px:PXGridColumn DataField="PMChangeOrderLine__Qty" TextAlign="Right" />
                                    <px:PXGridColumn DataField="PMChangeOrderLine__UOM" />
                                    <px:PXGridColumn DataField="PMChangeOrderLine__UnitCost" TextAlign="Right" />
                                    <px:PXGridColumn DataField="PMChangeOrderLine__Amount" TextAlign="Right" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" />
                        <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Other">
                <Template>
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XL" GroupCaption="Order Info" />
                    <px:PXSegmentMask CommitChanges="True" ID="edBranchID" runat="server" DataField="BranchID" />
                    <px:PXSelector ID="edRQReqNbr" runat="server" AllowEdit="True" DataField="RQReqNbr" Enabled="False" />
                    <px:PXSelector ID="edOriginalPONbr" runat="server" AllowEdit="True" DataField="OriginalPONbr" Enabled="False" />
                    <px:PXSelector ID="edSuccessorPONbr" runat="server" AllowEdit="True" DataField="SuccessorPONbr" Enabled="False" />
                    <px:PXSelector ID="edOwnerWorkgroupID" runat="server" DataField="OwnerWorkgroupID" />
                    <px:PXLayoutRule runat="server" Merge="True" />
                    <px:PXCheckBox ID="chkDontPrint" runat="server" Checked="True" DataField="DontPrint" Size="SM" CommitChanges="true" />
                    <px:PXCheckBox ID="chkPrinted" runat="server" DataField="Printed" Enabled="False" Size="SM" />
                    <px:PXLayoutRule runat="server" />
                    <px:PXLayoutRule runat="server" Merge="True" />
                    <px:PXCheckBox ID="chkDontEmail" runat="server" Checked="True" DataField="DontEmail" Size="SM" CommitChanges="true" />
                    <px:PXCheckBox ID="chkEmailed" runat="server" DataField="Emailed" Enabled="False" Size="SM" />
                    <px:PXLayoutRule runat="server" />
                    <px:PXCheckBox ID="chkOrderBasedAPBill" runat="server" DataField="OrderBasedAPBill" />

                    <px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="XL" GroupCaption="Retainage Settings" />
                    <px:PXCheckBox ID="chkRetainageApply" runat="server" DataField="RetainageApply" CommitChanges="true" />
                    <px:PXNumberEdit ID="edDefRetainagePct" runat="server" DataField="DefRetainagePct" />
                    <px:PXLayoutRule runat="server" ControlSize="XL" LabelsWidth="SM" GroupCaption="VAT Totals" />
                    <px:PXNumberEdit ID="edCuryVatExemptTotal" runat="server" DataField="CuryVatExemptTotal" Enabled="False" Size="XL" />
                    <px:PXNumberEdit ID="edCuryVatTaxableTotal" runat="server" DataField="CuryVatTaxableTotal" Enabled="False" Size="XL" />

                    <px:PXLayoutRule runat="server" StartColumn="true" ControlSize="XM" LabelsWidth="SM" GroupCaption="Order Totals" />
                    <px:PXNumberEdit ID="edCuryGoodsExtCostTotal" runat="server" DataField="CuryGoodsExtCostTotal" Enabled="False" Size="XM" />
                    <px:PXNumberEdit ID="edCuryServiceExtCostTotal" runat="server" DataField="CuryServiceExtCostTotal" Enabled="False" Size="XM" />
                    <px:PXNumberEdit ID="edCuryLineDiscTotal2" runat="server" DataField="CuryLineDiscTotal" Enabled="False" Size="XM" />
                    <px:PXNumberEdit ID="edCuryDiscTot2" runat="server" DataField="CuryDiscTot" Enabled="False" Size="XM" />

                    <px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="XM" GroupCaption="Billing Info" />
                    <px:PXNumberEdit ID="edUnbilledOrderQty" runat="server" DataField="UnbilledOrderQty" Enabled="false" Size="XM" />
                    <px:PXNumberEdit ID="edCuryUnbilledOrderTotal" runat="server" DataField="CuryUnbilledOrderTotal" Enabled="false" Size="XM" />
                    <px:PXNumberEdit ID="edCuryPrepaidTotal" runat="server" DataField="CuryPrepaidTotal" Enabled="false" Size="XM" />
                    <px:PXNumberEdit ID="edCuryUnprepaidTotal" runat="server" DataField="CuryUnprepaidTotal" Enabled="false" Size="XM" />

                    <px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="XM" GroupCaption="Intercompany Sale" />
                    <px:PXSelector ID="edIntercompanySOType" runat="server" DataField="IntercompanySOType" />
                    <px:PXSelector ID="edIntercompanySONbr" runat="server" DataField="IntercompanySONbr" AllowEdit="True" Enabled="False" />
                    <px:PXCheckBox ID="chkExcludeFromIntercompanyProc" runat="server" DataField="ExcludeFromIntercompanyProc" />
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Compliance">
                <Template>
                    <px:PXGrid runat="server" ID="grdComplianceDocuments" Height="300px" DataSourceID="ds" SkinID="DetailsInTab" Width="100%" AutoGenerateColumns="Append" KeepPosition="True" SyncPosition="True" AllowPaging="True" PageSize="12">
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
                                    <px:PXGridColumn DataField="PurchaseOrder" LinkCommand="ComplianceDocument$PurchaseOrder$Link" DisplayMode="Text" CommitChanges="True" />
                                    <px:PXGridColumn DataField="PurchaseOrderLineItem" TextAlign="Left" CommitChanges="True" />
                                    <px:PXGridColumn DataField="Subcontract" DisplayMode="Text" CommitChanges="True" LinkCommand="ComplianceDocument$Subcontract$Link" />
                                    <px:PXGridColumn DataField="SubcontractLineItem" TextAlign="Left" />
                                    <px:PXGridColumn DataField="ChangeOrderNumber" DisplayMode="Text" LinkCommand="ComplianceDocument$ChangeOrderNumber$Link" CommitChanges="True" />
                                    <px:PXGridColumn DataField="AccountID" TextAlign="Left" CommitChanges="True" />
                                    <px:PXGridColumn DataField="ApCheckID" LinkCommand="ComplianceDocument$ApCheckID$Link" CommitChanges="True" DisplayMode="Text" TextAlign="Left" />
                                    <px:PXGridColumn DataField="CheckNumber" TextAlign="Left" />
                                    <px:PXGridColumn DataField="ArPaymentID" LinkCommand="ComplianceDocument$ArPaymentID$Link" DisplayMode="Text" CommitChanges="True" TextAlign="Left" />
                                    <px:PXGridColumn DataField="BillAmount" TextAlign="Right" />
                                    <px:PXGridColumn DataField="BillID" LinkCommand="ComplianceDocument$BillID$Link" CommitChanges="True" DisplayMode="Text" />
                                    <px:PXGridColumn DataField="CertificateNumber" TextAlign="Left" />
                                    <px:PXGridColumn DataField="CreatedByID" />
                                    <px:PXGridColumn DataField="CustomerID" CommitChanges="True" LinkCommand="ComplianceViewCustomer" />
                                    <px:PXGridColumn DataField="CustomerName" TextAlign="Left" />
                                    <px:PXGridColumn DataField="DateIssued" TextAlign="Left" />
                                    <px:PXGridColumn DataField="DocumentTypeValue" CommitChanges="True" />
                                    <px:PXGridColumn DataField="EffectiveDate" TextAlign="Left" />
                                    <px:PXGridColumn DataField="InsuranceCompany" TextAlign="Left" />
                                    <px:PXGridColumn DataField="InvoiceAmount" TextAlign="Right" />
                                    <px:PXGridColumn DataField="InvoiceID" LinkCommand="ComplianceDocument$InvoiceID$Link" CommitChanges="True" DisplayMode="Text" />
                                    <px:PXGridColumn DataField="IsExpired" Type="CheckBox" TextAlign="Center" />
                                    <px:PXGridColumn DataField="IsRequiredJointCheck" Type="CheckBox" TextAlign="Center" />
                                    <px:PXGridColumn DataField="JointAmount" TextAlign="Right" />
                                    <px:PXGridColumn DataField="JointRelease" TextAlign="Left" />
                                    <px:PXGridColumn DataField="JointReleaseReceived" TextAlign="Center" />
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
                                    <px:PXGridColumn DataField="ReceiptDate" TextAlign="Left" />
                                    <px:PXGridColumn DataField="ReceiveDate" TextAlign="Left" />
                                    <px:PXGridColumn DataField="ReceivedBy" TextAlign="Left" />
                                    <px:PXGridColumn DataField="SecondaryVendorID" LinkCommand="ComplianceViewSecondaryVendor" CommitChanges="True" />
                                    <px:PXGridColumn DataField="SecondaryVendorName" TextAlign="Left" />
                                    <px:PXGridColumn DataField="SponsorOrganization" TextAlign="Left" />
                                    <px:PXGridColumn DataField="ThroughDate" TextAlign="Left" />
                                    <px:PXGridColumn DataField="SourceType" TextAlign="Left" />
                                </Columns>
                                <RowTemplate>
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
                                    <px:PXSelector runat="server" DataField="DocumentType" AutoRefresh="True" ID="edDocumentType" />
                                    <px:PXSelector runat="server" DataField="Status" AutoRefresh="True" ID="edStatus" />
                                    <px:PXSelector runat="server" DataField="VendorID" AutoRefresh="True" ID="edVendorID" />
                                    <px:PXSelector runat="server" DataField="AccountID" AutoRefresh="True" ID="edAccountID" />
                                    <px:PXSelector runat="server" DataField="CostTaskID" FilterByAllFields="True" AutoRefresh="True" ID="edCostTaskID" />
                                    <px:PXSelector runat="server" DataField="RevenueTaskID" FilterByAllFields="True" AutoRefresh="True" ID="edRevenueTaskID" />
                                    <px:PXSelector runat="server" ID="edComplianceProjectID" DataField="ProjectID" FilterByAllFields="True" AutoRefresh="True" />
                                    <px:PXSegmentMask runat="server" DataField="CostCodeID" AutoRefresh="True" ID="edCostCode2" />
                                </RowTemplate>
                            </px:PXGridLevel>
                        </Levels>
                        <Mode InitNewRow="True" />
                        <AutoSize Enabled="True" MinHeight="150" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
        </Items>
        <AutoSize Container="Window" Enabled="True" MinHeight="180" />
    </px:PXTab>
    <px:PXSmartPanel ID="PanelAddSiteStatus" runat="server" Key="ItemInfo" LoadOnDemand="true" Width="1100px" Height="500px"
        Caption="Inventory Lookup" CaptionVisible="true" AutoCallBack-Command='Refresh' AutoCallBack-Enabled="True" AutoCallBack-Target="formSitesStatus"
        DesignView="Hidden">
        <px:PXFormView ID="formSitesStatus" runat="server" CaptionVisible="False" DataMember="ItemFilter" DataSourceID="ds"
            Width="100%" SkinID="Transparent">
            <Template>
                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
                <px:PXTextEdit CommitChanges="True" ID="edInventory" runat="server" DataField="Inventory" />
                <px:PXTextEdit CommitChanges="True" ID="edBarCode" runat="server" DataField="BarCode" />
                <px:PXCheckBox CommitChanges="True" ID="chkOnlyAvailable" runat="server" Checked="True" DataField="OnlyAvailable" AutoCallBack="true"  />
                <px:PXLayoutRule  runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                <px:PXSegmentMask CommitChanges="True" ID="edSiteID" runat="server" DataField="SiteID" AutoRefresh="true" />
                <px:PXSegmentMask CommitChanges="True" ID="edItemClassID" runat="server" DataField="ItemClass" />
                <px:PXSegmentMask CommitChanges="True" ID="edSubItem" runat="server" DataField="SubItem" AutoRefresh="true" /></Template>
        </px:PXFormView>
        <px:PXGrid ID="gripSiteStatus" runat="server" DataSourceID="ds" Style="border-width: 1px 0px; top: 0px; left: 0px;" AutoAdjustColumns="true"
            Width="100%" SkinID="Details" AdjustPageSize="Auto" Height="135px" AllowSearch="True" BatchUpdate="true" FastFilterID="edInventory"
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
                        <px:PXSegmentMask ID="editemClass" runat="server" DataField="ItemClassID" />
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
                        <px:PXGridColumn DataField="PurchaseUnit" DisplayFormat="&gt;aaaaaa" />
                        <px:PXGridColumn AllowNull="False" DataField="QtyAvailExt" TextAlign="Right" />
                        <px:PXGridColumn AllowNull="False" DataField="QtyOnHandExt" TextAlign="Right" />
                        <px:PXGridColumn AllowNull="False" DataField="QtyPOOrdersExt" TextAlign="Right" />
                        <px:PXGridColumn AllowNull="False" DataField="QtyPOReceiptsExt" TextAlign="Right" />
                        <px:PXGridColumn AllowNull="False" DataField="AlternateID" />
                        <px:PXGridColumn AllowNull="False" DataField="AlternateType" />
                        <px:PXGridColumn AllowNull="False" DataField="AlternateDescr" />
                    </Columns>
                </px:PXGridLevel>
            </Levels>
            <AutoSize Enabled="true" />
        </px:PXGrid>
        <px:PXPanel ID="PXPanel5" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton6" runat="server" CommandName="AddSelectedItems" CommandSourceID="ds" Text="Add" SyncVisible="false"/>
            <px:PXButton ID="PXButton7" runat="server" Text="Add & Close" DialogResult="OK"/>
            <px:PXButton ID="PXButton8" runat="server" DialogResult="Cancel" Text="Cancel" />
        </px:PXPanel>
    </px:PXSmartPanel>
    <%-- Recalculate Prices and Discounts --%>
    <px:PXSmartPanel ID="PanelRecalcDiscounts" runat="server" Caption="Recalculate Prices" CaptionVisible="true" LoadOnDemand="true" Key="recalcdiscountsfilter"
        AutoCallBack-Enabled="true" AutoCallBack-Target="formRecalcDiscounts" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True"
        CallBackMode-PostData="Page">
            <px:PXFormView ID="formRecalcDiscounts" runat="server" DataSourceID="ds" CaptionVisible="False" DataMember="recalcdiscountsfilter">
                <Activity Height="" HighlightColor="" SelectedColor="" Width="" />
                <ContentStyle BackColor="Transparent" BorderStyle="None" />
                <Template>
                    <px:PXLayoutRule ID="PXLayoutRule3" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="SM" />
                    <px:PXDropDown ID="edRecalcTerget" runat="server" DataField="RecalcTarget" CommitChanges ="true" />
                    <px:PXCheckBox CommitChanges="True" ID="chkRecalcUnitPrices" runat="server" DataField="RecalcUnitPrices" />
                    <px:PXCheckBox CommitChanges="True" ID="chkOverrideManualPrices" runat="server" DataField="OverrideManualPrices" Style="margin-left: 25px" />
                    <px:PXCheckBox CommitChanges="True" ID="chkRecalcDiscounts" runat="server" DataField="RecalcDiscounts" />
                    <px:PXCheckBox CommitChanges="True" ID="chkOverrideManualDiscounts" runat="server" DataField="OverrideManualDiscounts" Style="margin-left: 25px" />
                    <px:PXCheckBox CommitChanges="True" ID="chkOverrideManualDocGroupDiscounts" runat="server" DataField="OverrideManualDocGroupDiscounts" Style="margin-left: 25px" />
                </Template>
            </px:PXFormView>
        <px:PXPanel ID="PXPanel6" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton10" runat="server" DialogResult="OK" Text="OK" CommandName="RecalcOk" CommandSourceID="ds" />
        </px:PXPanel>
    </px:PXSmartPanel>
    <%-- Create Sales Order --%>
    <px:PXSmartPanel ID="createSOOrderPanel" runat="server" Caption="Create Sales Order" CaptionVisible="true" LoadOnDemand="true" Key="createSOFilter"
        AutoCallBack-Enabled="true" AutoCallBack-Target="createSOOrderForm" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True"
        CallBackMode-PostData="Page">
        <px:PXFormView ID="createSOOrderForm" runat="server" DataSourceID="ds" CaptionVisible="False" DataMember="createSOFilter">
            <Activity Height="" HighlightColor="" SelectedColor="" Width="" />
            <ContentStyle BackColor="Transparent" BorderStyle="None" />
            <Template>
                <px:PXLayoutRule ID="PXLayoutRule3" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
                <px:PXSelector CommitChanges="True" ID="edSalesOrderType" runat="server" DataField="OrderType" />
                <px:PXMaskEdit CommitChanges="True" ID="edSalesOrderNbr" runat="server" DataField="OrderNbr" InputMask="&gt;CCCCCCCCCCCCCCC" />
                <px:PXSelector CommitChanges="True" ID="edCustomerID" runat="server" DataField="CustomerID" AutoRefresh="true" />
                <px:PXSelector CommitChanges="True" ID="edLocationID" runat="server" DataField="CustomerLocationID" AutoRefresh="true" />
                <px:PXTextEdit CommitChanges="True" ID="edCustomerOrderNbr" runat="server" DataField="CustomerOrderNbr" />
            </Template>
        </px:PXFormView>
        <px:PXPanel ID="PXPanel7" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton11" runat="server" DialogResult="OK" Text="OK" CommandSourceID="ds" />
        </px:PXPanel>
    </px:PXSmartPanel>
    <px:PXSmartPanel runat="server" ID="ReversingChangeOrdersPanel" Height="396px" Width="850px" LoadOnDemand="true" AutoRepaint="true" CaptionVisible="True" Caption="Reversing Change Orders" Key="ReversingChangeOrders" AutoCallBack-Enabled="True" AutoCallBack-Target="ReversingChangeOrdersGrid" AutoCallBack-Command="Refresh">
    <px:PXGrid runat="server" SyncPosition="true" Height="240px" SkinID="Inquire" Width="100%" ID="ReversingChangeOrdersGrid" DataSourceID="ds">
        <AutoSize Enabled="true" />
        <Mode AllowAddNew="False" AllowUpdate="False" AllowDelete="False" />
        <Levels>
            <px:PXGridLevel DataMember="ReversingChangeOrders" >
                <Columns>
                    <px:PXGridColumn DataField="RefNbr" LinkCommand="ViewCurrentReversingChangeOrder"/>
                    <px:PXGridColumn DataField="Description" />
                </Columns>
            </px:PXGridLevel>
        </Levels>
    </px:PXGrid>
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
    <px:PXSmartPanel ID="AddProjectItemPanel" runat="server" Height="396px" Width="850px" Caption="Add Project Items" CaptionVisible="True" Key="AvailableProjectItems" AutoCallBack-Command="Refresh"
        AutoCallBack-Enabled="True" AutoCallBack-Target="AvailableProjectItemsGrid" LoadOnDemand="true" AutoRepaint="true">
        <px:PXFormView ID="formProjectItemFilter" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="ProjectItemFilter" DefaultControlID="edProjectID" SkinID="Transparent">
            <Template>
                <px:PXLayoutRule runat="server" StartColumn="True"/>
                <px:PXSegmentMask CommitChanges="True" ID="edProjectID" runat="server" DataField="ProjectID" AllowEdit="True"/>
            </Template>
        </px:PXFormView>
        <px:PXGrid ID="AvailableProjectItemsGrid" runat="server" Height="240px" Width="100%" DataSourceID="ds" SkinID="Details" SyncPosition="true">
            <AutoSize Enabled="true" />
            <Levels>
                <px:PXGridLevel DataMember="AvailableProjectItems">
                    <Columns>
                        <px:PXGridColumn DataField="Selected" Label="Selected" Type="CheckBox" AllowCheckAll="true" />
                        <px:PXGridColumn AutoCallBack="True" DataField="ProjectTaskID" />
                        <px:PXGridColumn AutoCallBack="True" DataField="InventoryID" />
                        <px:PXGridColumn AutoCallBack="True" DataField="CostCodeID" />
                        <px:PXGridColumn AutoCallBack="True" DataField="AccountGroupID" />
                        <px:PXGridColumn DataField="Description" />
                        <px:PXGridColumn AutoCallBack="True" DataField="UOM" />
                        <px:PXGridColumn DataField="CuryUnitRate" TextAlign="Right" CommitChanges="true" />
                        <px:PXGridColumn DataField="Qty" TextAlign="Right" CommitChanges="true" />
                        <px:PXGridColumn DataField="CuryAmount" TextAlign="Right" CommitChanges="true" />
                        <px:PXGridColumn DataField="RevisedQty" TextAlign="Right" CommitChanges="true" />
                        <px:PXGridColumn DataField="CuryRevisedAmount" TextAlign="Right" CommitChanges="true" />
                        <px:PXGridColumn DataField="ChangeOrderQty" TextAlign="Right" />
                        <px:PXGridColumn DataField="CuryChangeOrderAmount" TextAlign="Right" />
                        <px:PXGridColumn DataField="CommittedQty" TextAlign="Right" />
                        <px:PXGridColumn DataField="CuryCommittedAmount" TextAlign="Right" />
                        <px:PXGridColumn DataField="CommittedReceivedQty" TextAlign="Right" />
                        <px:PXGridColumn DataField="CommittedInvoicedQty" TextAlign="Right" />
                        <px:PXGridColumn DataField="CuryCommittedInvoicedAmount" TextAlign="Right" />
                        <px:PXGridColumn DataField="CommittedOpenQty" TextAlign="Right" />
                        <px:PXGridColumn DataField="CuryCommittedOpenAmount" TextAlign="Right" />
                        <px:PXGridColumn DataField="ActualQty" TextAlign="Right" />
                        <px:PXGridColumn DataField="CuryActualAmount" TextAlign="Right" />
                        <px:PXGridColumn DataField="CuryActualPlusOpenCommittedAmount" TextAlign="Right" />
                        <px:PXGridColumn DataField="CuryVarianceAmount" TextAlign="Right" />
                        <px:PXGridColumn DataField="Performance" TextAlign="Right" />
                        <px:PXGridColumn DataField="IsProduction" AutoCallBack="True" TextAlign="Center" Type="CheckBox" />
                        <px:PXGridColumn DataField="CuryLastCostToComplete" TextAlign="Right" />
                        <px:PXGridColumn DataField="CuryCostToComplete" TextAlign="Right" />
                        <px:PXGridColumn DataField="LastPercentCompleted" TextAlign="Right" />
                        <px:PXGridColumn DataField="PercentCompleted" TextAlign="Right" />
                        <px:PXGridColumn DataField="CuryLastCostAtCompletion" TextAlign="Right" />
                        <px:PXGridColumn DataField="CuryCostAtCompletion" TextAlign="Right" />
                        <px:PXGridColumn DataField="RevenueTaskID" AutoCallBack="True" />
                        <px:PXGridColumn DataField="RevenueInventoryID" AutoCallBack="True" />
                    </Columns>
                </px:PXGridLevel>
            </Levels>
            <ActionBar>
                <Actions>
                    <AddNew MenuVisible="False" ToolBarVisible="False" />
                    <Delete MenuVisible="False" ToolBarVisible="Top" />
                    <NoteShow MenuVisible="False" ToolBarVisible="False" />
                </Actions>
            </ActionBar>
            <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
        </px:PXGrid>
         <px:PXPanel ID="PXPanelBtn" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButtonAdd" runat="server" Text="Add Lines" CommandName="AppendSelectedProjectItems"  CommandSourceID="ds" />
            <px:PXButton ID="PXButtonAddClose" runat="server" Text="Add Lines & Close" DialogResult="OK"  />
            <px:PXButton ID="PXButtonClose" runat="server" DialogResult="Cancel" Text="Close" />      
        </px:PXPanel>
    </px:PXSmartPanel>
</asp:Content>
