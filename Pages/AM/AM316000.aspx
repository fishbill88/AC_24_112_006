<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="AM316000.aspx.cs" Inherits="Page_AM316000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.AM.MultipleProductionClockEntry" PrimaryView="Document">
        <CallbackCommands>
            <px:PXDSCallbackCommand CommitChanges="True" Visible="False" Name="AMClockTranLineSplittingMultipleProductionExtension_GenerateNumbers" />
            <px:PXDSCallbackCommand CommitChanges="True" Visible="False" Name="AMClockTranLineSplittingMultipleProductionExtension_ShowSplits" DependOnGrid="grid" />
            <px:PXDSCallbackCommand CommitChanges="True" Name="FillCurrentUser" Visible="false" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Document" Width="100%" Height="65px" >
        <Template>
            <px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
            <px:PXSegmentMask runat="server" ID="edEmployeeID" CommitChanges="true" DataField="EmployeeID"></px:PXSegmentMask>
            <px:PXButton ID="btnCurrent" runat="server" Text="Current User" Height="20px" CommandName="FillCurrentUser" CommandSourceID="ds"></px:PXButton>
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXTab ID="tab" runat="server" style="z-index: 100;" width="100%">
        <Items>
            <px:PXTabItem Text="Clock Entries">
                <Template>
                    <px:PXGrid ID="gridClockEntires" OnRowDataBound="Grid_RowDataBound" runat="server" DataSourceID="ds" Style="z-index: 100; left: 0px; top: 0px; height: 250px;"
                        Width="100%" SkinID="DetailsInTab" SyncPosition="True">
                    <AutoSize Enabled="True" MinHeight="250" />
                    <Mode InitNewRow="True" AllowUpload="True" />
                        <Levels>
                            <px:PXGridLevel DataMember="Transactions">
                                <Columns>
	                                <px:PXGridColumn CommitChanges="True" DataField="Selected" Type="CheckBox" TextAlign="Center" Width="70"></px:PXGridColumn>
                                    <px:PXGridColumn Type="DropDownList" DataField="Status" Width="70"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="TranDate" Width="90"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="StartTime_Time" Width="90"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="EndTime_Time" Width="90"></px:PXGridColumn>
	                                <px:PXGridColumn DataField="Duration" Width="70" ></px:PXGridColumn>
                                    <px:PXGridColumn CommitChanges="True" DataField="OrderType" Width="70"></px:PXGridColumn>
                                    <px:PXGridColumn CommitChanges="True" DataField="ProdOrdID" Width="140"></px:PXGridColumn>
                                    <px:PXGridColumn CommitChanges="True" DataField="OperationID" Width="120"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="WcID" Width="70"></px:PXGridColumn>
                                    <px:PXGridColumn CommitChanges="True" DataField="InventoryID" Width="70"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="ShiftCD" Width="140"></px:PXGridColumn>
                                    <px:PXGridColumn CommitChanges="True" DataField="Qty" Width="100"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="UOM" Width="72"></px:PXGridColumn>
                                    <px:PXGridColumn CommitChanges="True" DataField="QtyScrapped" Width="70"></px:PXGridColumn>
                                    <px:PXGridColumn CommitChanges="True" DataField="ReasonCodeID" Width="70"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="ScrapAction" Width="70"></px:PXGridColumn>
                                    <px:PXGridColumn CommitChanges="True" AllowShowHide="True" DataField="LaborTime" Width="72" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="SiteID" Width="70" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="LocationID" Width="140" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="EmployeeID" Width="140" ></px:PXGridColumn>
                                    <px:PXGridColumn Type="CheckBox" DataField="Closeflg" Width="60" TextAlign="Center"  ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="LineNbr" Width="70" ></px:PXGridColumn>
                                </Columns>
                                <RowTemplate>
                                    <px:PXMaskEdit ID="Duration" runat="server" DataField="Duration" Width="150px"></px:PXMaskEdit>
                                    <px:PXSegmentMask ID="eInventoryID" runat="server" DataField="InventoryID" AllowEdit="True"></px:PXSegmentMask>
                                    <px:PXSegmentMask ID="eEmployeeID" runat="server" DataField="EmployeeID" AllowEdit="True"></px:PXSegmentMask>
                                    <px:PXSelector AutoRefresh="True" ID="eProdOrdID" runat="server" DataField="ProdOrdID" AllowEdit="True"></px:PXSelector>
                                    <px:PXMaskEdit ID="edLaborTime" runat="server" DataField="LaborTime" AutoRefresh="True"/>
                                </RowTemplate>
                            </px:PXGridLevel>
                        </Levels>
                       
                        <ActionBar>
                            <CustomItems>
                                <px:PXToolBarButton Text="CLOCK IN" DependOnGrid="gridClockEntires">
                                    <AutoCallBack Target="ds" Command="clockEntriesClockIn"></AutoCallBack>
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Text="CLOCK OUT" DependOnGrid="gridClockEntires">
                                    <AutoCallBack Enabled="True" Target="ds" Command="clockEntriesClockOut"></AutoCallBack>
                                    <AutoCallBack Target="ds"></AutoCallBack>
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Text="Line Details" Key="cmdLS" CommandName="AMClockTranLineSplittingMultipleProductionExtension_ShowSplits" CommandSourceID="ds" DependOnGrid="gridClockEntires" />
                            </CustomItems>
                            <Actions>
                            </Actions>
                        </ActionBar>
	                </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Operations">
                <Template>
                    <px:PXGrid ID="gridOperations" DataSourceID="ds" Style="z-index: 100; left: 0px; top: 0px; height: 250px;" AllowSearch="True" AllowFilter="True" AllowPaging="True" AdjustPageSize="Auto" SyncPosition="True" 
                    Width="100%" SkinID="Details" runat="server" OnRowDataBound="OperationsGrid_RowDataBound">
                    <AutoSize Enabled="True" MinHeight="250" />
                        <Levels>
                            <px:PXGridLevel DataMember="Operations">
                                <Columns>
                                    <px:PXGridColumn TextAlign="Center" Type="CheckBox" DataField="Selected" Width="120"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="WcID" Width="90"></px:PXGridColumn>
                                    <px:PXGridColumn Type="CheckBox" DataField="AMWC__AllowMultiClockEntry" Width="110" TextAlign="Center" ></px:PXGridColumn>
	                                <px:PXGridColumn Type="CheckBox" DataField="ClockedInByOperation__Active" Width="60" TextAlign="Center" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="AMProdItem__OrderType" Width="90"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="ProdOrdID" Width="90"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="AMProdItem__StatusID" Width="90"></px:PXGridColumn>                                    
                                    <px:PXGridColumn DataField="OperationCD" Width="90"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="AMProdItem__Descr" Width="140"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="StatusID" Width="90"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="TotalQty" Width="70"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="QtytoProd" Width="70"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="QtyComplete" Width="70"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="QtyScrapped" Width="70"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="StartDate_Date" Width="70"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="StartDate_Time" Width="70"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="EndDate_Date" Width="70"></px:PXGridColumn>
	                                <px:PXGridColumn DataField="EndDate_Time" Width="90" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="AMProdItem__InventoryID" Width="90"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="AMProdItem__InventoryID_InventoryItem_descr" Width="140"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="AMProdItem__SiteID" Width="70"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="AMProdItem__OrdTypeRef" Width="70"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="AMProdItem__OrdNbr" Width="70"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="AMProdItem__OrdLineRef" Width="70"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="AMProdItem__CustomerID" Width="90"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="AMProdItem__CustomerID_Customer_acctName" Width="90"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="AMProdItem__SchPriority" Width="70"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="AMProdItem__ScheduleStatus" Width="70"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="ControlPoint" Type="CheckBox" Width="70" TextAlign="Center" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="AMProdItem__BranchID" Width="70"></px:PXGridColumn></Columns>
                                <RowTemplate>
                                    <px:PXSegmentMask ID="eOperInventoryID" runat="server" DataField="AMProdItem__InventoryID" AllowEdit="True"></px:PXSegmentMask>
                                    <px:PXSelector ID="eOperProdOrdID" runat="server" DataField="AMProdItem__ProdOrdID" AllowEdit="True"/>
                                </RowTemplate>
                            </px:PXGridLevel>
                        </Levels>
                        <ActionBar>
                            <CustomItems>
                                <px:PXToolBarButton Text="CLOCK IN" DependOnGrid="gridOperations">
                                    <AutoCallBack Enabled="True" Command="operationsClockIn" Target="ds"></AutoCallBack>
                                </px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
                        <Mode AllowUpdate="True" AllowAddNew="False"/>               
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
        </Items>
        <AutoSize Container="Window" Enabled="True" MinHeight="180" />
    </px:PXTab>
    <px:PXSmartPanel ID="PanelLS" runat="server" Width="764px" Caption="Line Details" DesignView="Content" CaptionVisible="True"
        Key="AMClockTranLineSplittingMultipleProductionExtension_lsselect" AutoCallBack-Command="Refresh" AutoCallBack-Enabled="True" AutoCallBack-Target="optform" Height="500px">
        <px:PXFormView ID="optform" runat="server" Width="100%" CaptionVisible="False" DataMember="AMClockTranLineSplittingMultipleProductionExtension_LotSerOptions" DataSourceID="ds"
            SkinID="Transparent" TabIndex="700">
            <Template>
                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" ></px:PXLayoutRule>
                <px:PXNumberEdit ID="edUnassignedQty" runat="server" DataField="UnassignedQty" Enabled="False" ></px:PXNumberEdit>
                <px:PXNumberEdit ID="edQty1" runat="server" DataField="Qty">
                    <AutoCallBack>
                        <Behavior CommitChanges="True" ></Behavior>
                    </AutoCallBack>
                </px:PXNumberEdit>
                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" ></px:PXLayoutRule>
                <px:PXMaskEdit ID="edStartNumVal" runat="server" DataField="StartNumVal" ></px:PXMaskEdit>
                <px:PXButton ID="btnGenerate" runat="server" Text="Generate" Height="20px" CommandName="AMClockTranLineSplittingMultipleProductionExtension_GenerateNumbers" CommandSourceID="ds">
                </px:PXButton>
            </Template>
        </px:PXFormView>
        <px:PXGrid ID="grid2" runat="server" Width="100%" AutoAdjustColumns="True" DataSourceID="ds" Height="192px" SkinID="Details">
            <Mode InitNewRow="True" ></Mode>
            <AutoSize Enabled="true" ></AutoSize>
            <Levels>
                <px:PXGridLevel DataMember="Splits">
                    <Columns>
                        <px:PXGridColumn DataField="InventoryID" Width="108px" ></px:PXGridColumn>
                        <px:PXGridColumn DataField="SubItemID" Width="108px" ></px:PXGridColumn>
                        <px:PXGridColumn DataField="LocationID" Width="108px" ></px:PXGridColumn>
                        <px:PXGridColumn DataField="LotSerialNbr" Width="108px" ></px:PXGridColumn>
                        <px:PXGridColumn DataField="Qty" Width="108px" TextAlign="Right" ></px:PXGridColumn>
                        <px:PXGridColumn DataField="UOM" Width="108px" ></px:PXGridColumn>
                        <px:PXGridColumn DataField="ExpireDate" Width="90px" ></px:PXGridColumn>
                    </Columns>
                    <RowTemplate>
                        <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" ></px:PXLayoutRule>
                        <px:PXSegmentMask ID="edSubItemID2" runat="server" DataField="SubItemID" AutoRefresh="True">
                            <Parameters>
                                <px:PXControlParam ControlID="grid2" Name="AMClockTranSplit.inventoryID" PropertyName="DataValues[&quot;InventoryID&quot;]" Type="String" ></px:PXControlParam>
                            </Parameters>
                        </px:PXSegmentMask>
                        <px:PXSegmentMask ID="edLocationID2" runat="server" DataField="LocationID" AutoRefresh="True">
                            <Parameters>
                                <px:PXControlParam ControlID="grid2" Name="AMClockTranSplit.siteID" PropertyName="DataValues[&quot;SiteID&quot;]" Type="String" ></px:PXControlParam>
                                <px:PXControlParam ControlID="grid2" Name="AMClockTranSplit.inventoryID" PropertyName="DataValues[&quot;InventoryID&quot;]" Type="String" ></px:PXControlParam>
                                <px:PXControlParam ControlID="grid2" Name="AMClockTranSplit.subItemID" PropertyName="DataValues[&quot;SubItemID&quot;]" Type="String" ></px:PXControlParam>
                            </Parameters>
                        </px:PXSegmentMask>
                        <px:PXNumberEdit ID="edQty2" runat="server" DataField="Qty" ></px:PXNumberEdit>
                        <px:PXSelector ID="edUOM2" runat="server" DataField="UOM" AutoRefresh="True">
                            <Parameters>
                                <px:PXControlParam ControlID="grid" Name="AMClockTran.inventoryID" PropertyName="DataValues[&quot;InventoryID&quot;]" Type="String" ></px:PXControlParam>
                            </Parameters>
                        </px:PXSelector>
                        <px:PXSelector ID="edLotSerialNbr2" runat="server" DataField="LotSerialNbr" AutoRefresh="True">
                            <Parameters>
                                <px:PXControlParam ControlID="grid2" Name="AMClockTranSplit.inventoryID" PropertyName="DataValues[&quot;InventoryID&quot;]" Type="String" ></px:PXControlParam>
                                <px:PXControlParam ControlID="grid2" Name="AMClockTranSplit.subItemID" PropertyName="DataValues[&quot;SubItemID&quot;]" Type="String" ></px:PXControlParam>
                                <px:PXControlParam ControlID="grid2" Name="AMClockTranSplit.locationID" PropertyName="DataValues[&quot;LocationID&quot;]" Type="String" ></px:PXControlParam>
                            </Parameters>
                        </px:PXSelector>
                        <px:PXDateTimeEdit ID="edExpireDate2" runat="server" DataField="ExpireDate" ></px:PXDateTimeEdit>
                    </RowTemplate>
                    <Layout ColumnsMenu="False" ></Layout>
                </px:PXGridLevel>
            </Levels>
        </px:PXGrid>
        <px:PXPanel ID="PXPanel1" runat="server" SkinID="Buttons">
            <px:PXButton ID="btnSave" runat="server" DialogResult="OK" Text="OK" ></px:PXButton>
        </px:PXPanel>
    </px:PXSmartPanel>
</asp:Content>
