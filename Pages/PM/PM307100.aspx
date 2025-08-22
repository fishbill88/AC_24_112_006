<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="PM307100.aspx.cs"
    Inherits="Page_PM307100" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.PM.ProformaLinkMaint" PrimaryView="Filter">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="ViewDocument" Visible="false" DependOnGrid ="grid" />
            <px:PXDSCallbackCommand Name="ViewInventory" Visible="false" DependOnGrid ="grid" />
            <px:PXDSCallbackCommand Name="ViewVendor" Visible="false" DependOnGrid ="grid" />
            <px:PXDSCallbackCommand Name="ViewOrigDocument" Visible="False" DependOnGrid="grid" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" Caption="Selection" DataMember="Filter"
        DefaultControlID="edProjectID" NoteField="">
        <Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
            <px:PXSegmentMask CommitChanges="True" ID="edProjectID" runat="server" DataField="ProjectID"/>
            <px:PXSelector CommitChanges="True" ID="edRefNbr" runat="server" AutoRefresh="True" DataField="RefNbr" />
            <px:PXSelector CommitChanges="True" ID="edLineNbr" runat="server" AutoRefresh="True" DataField="LineNbr" />
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" Height="150px" SkinID="Inquire" AdjustPageSize="Auto"
        AllowPaging="True" Caption="Transactions" FastFilterFields="RefNbr, Description" SyncPosition="True">
        <Levels>
            <px:PXGridLevel DataMember="Transactions">
                <Columns>
                    <px:PXGridColumn DataField="ProformaLineNbr" />
                    <px:PXGridColumn DataField="OrigTranType" />
                    <px:PXGridColumn DataField="OrigRefNbr" LinkCommand="ViewBill" DisplayMode="Hint"/>
                    <px:PXGridColumn DataField="OrigLineNbr" />
                    <px:PXGridColumn DataField="BAccountID" LinkCommand ="ViewVendor" DisplayMode="Hint"/>
                    <px:PXGridColumn DataField="Date" DataType="DateTime"/>
                    <px:PXGridColumn DataField="FinPeriodID" DataType="DateTime"/>
                    <px:PXGridColumn DataField="Description" />
                    <px:PXGridColumn DataField="TranCuryAmount" TextAlign="Right" />
					<px:PXGridColumn DataField="TranCuryID" />
                    
                    <px:PXGridColumn DataField="AccountGroupID" />
					<px:PXGridColumn DataField="TaskID" />
                    <px:PXGridColumn DataField="InventoryID"/>
                    <px:PXGridColumn DataField="CostCodeID" AutoCallBack="true" />
                    <px:PXGridColumn DataField="RefNbr"/> 
                    <px:PXGridColumn DataField="BatchNbr" /> 
					<px:PXGridColumn DataField="UOM"  />
                    <px:PXGridColumn DataField="Qty" TextAlign="Right" />
                    <px:PXGridColumn DataField="BillableQty" TextAlign="Right" />
                    <px:PXGridColumn DataField="TranCuryUnitRate" TextAlign="Right" />
                    <px:PXGridColumn DataField="Billable" TextAlign="Center" Type="CheckBox" />                                       
                    <px:PXGridColumn DataField="BranchID" />
                    <px:PXGridColumn DataField="TranID" />
                </Columns>
                <RowTemplate>
                    <px:PXSelector runat="server" ID="edBatchNbr" DataField="BatchNbr" AllowEdit="True" Size="s" />
                    <px:PXSelector runat="server" ID="edRefNbr" DataField="RefNbr" AllowEdit="True" Size="s" />
                </RowTemplate>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
        <ActionBar>
            <CustomItems>                
                <px:PXToolBarButton Text="Add Transactions" Key="cmdAddTransactions" DisplayStyle="Image" ImageKey="AddNew" Tooltip="Add Transactions">
                    <AutoCallBack Command="AddTransactions" Target="ds">
                    </AutoCallBack>
                </px:PXToolBarButton>
                <px:PXToolBarButton Key="cmdRemoveTransaction" DisplayStyle="Image" ImageKey="RecordDel" Tooltip="Remove Transactions">
                    <AutoCallBack Command="RemoveTransaction" Target="ds" />
                 </px:PXToolBarButton>
            </CustomItems>
        </ActionBar>
        <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False"  />
    </px:PXGrid>
    <px:PXSmartPanel ID="AddTransactionsPanel" runat="server" Height="600px" Width="900px" Caption="Add Transactions" CaptionVisible="True" Key="AvailableTransactions" AutoCallBack-Command="Refresh"
        AutoCallBack-Enabled="True" AutoCallBack-Target="UnbilledGrid" LoadOnDemand="true" AutoRepaint="true">
        <px:PXFormView ID="AvailableTransactionsForm" runat="server" DataMember="AvailableTransactionsFilter" RenderStyle="Simple">
            <Template>
                <px:PXLayoutRule ID="PXLayoutRule1" runat="server" ControlSize="SM" LabelsWidth="S" StartColumn="True" />
                <px:PXSegmentMask ID="edProjectID" runat="server" DataField="ProjectID" AutoRefresh="true" CommitChanges="true" />
                <px:PXSegmentMask ID="edProjectTaskID" runat="server" DataField="ProjectTaskID" AutoRefresh="true" CommitChanges="true" />
                <px:PXSegmentMask ID="edCostCodeID" runat="server" DataField="CostCodeID" AutoRefresh="true" CommitChanges="true" AllowAddNew="true" />
                <px:PXLayoutRule ID="PXLayoutRule2" runat="server" ControlSize="SM" LabelsWidth="S" StartColumn="True" />
                <px:PXDateTimeEdit ID="edDateFrom" runat="server" DataField="DateFrom" CommitChanges="True"/>
                <px:PXDateTimeEdit ID="edDateTo" runat="server" DataField="DateTo" CommitChanges="True"/>
                <px:PXLayoutRule ID="PXLayoutRule10" runat="server" ControlSize="SM" LabelsWidth="S" StartColumn="True" />                
                <px:PXSegmentMask ID="edVendorID" runat="server" DataField="VendorID" AutoRefresh="true" CommitChanges="true" />
                <px:PXDropDown ID="edAPDocType" runat="server" DataField="APDocType" AutoRefresh="true" CommitChanges="true" />
                <px:PXSelector ID="edAPRefNbr" runat="server" DataField="APRefNbr" AutoRefresh="true" CommitChanges="true" />                
            </Template>
        </px:PXFormView>
        <px:PXGrid ID="AvailableTransactionsGrid" runat="server" Height="240px" Width="100%" DataSourceID="ds" SkinID="Details" SyncPosition="true">
            <AutoSize Enabled="true" />
            <Levels>
                <px:PXGridLevel DataMember="AvailableTransactions">
                    <Columns>
                        <px:PXGridColumn DataField="Selected" Label="Selected" Type="CheckBox" AllowCheckAll="true" />
                        <px:PXGridColumn DataField="OrigTranType" />
                        <px:PXGridColumn DataField="OrigRefNbr" LinkCommand="ViewBill" />
                        <px:PXGridColumn DataField="OrigLineNbr" />
                        <px:PXGridColumn DataField="BAccountID" LinkCommand ="ViewVendor" DisplayMode="Hint"/>
                        <px:PXGridColumn DataField="Date" DataType="DateTime"/>
                        <px:PXGridColumn DataField="FinPeriodID" DataType="DateTime"/>
                        <px:PXGridColumn DataField="Description" />
                        <px:PXGridColumn DataField="TranCuryAmount" TextAlign="Right" />
					    <px:PXGridColumn DataField="TranCuryID" />

                        
					    <px:PXGridColumn DataField="TaskID" />
                        <px:PXGridColumn DataField="InventoryID" />
                        <px:PXGridColumn DataField="CostCodeID" AutoCallBack="true" />
                        <px:PXGridColumn DataField="RefNbr"/> 
                        <px:PXGridColumn DataField="BatchNbr" /> 
					    <px:PXGridColumn DataField="AccountGroupID" />
                        <px:PXGridColumn DataField="UOM"  />
                        <px:PXGridColumn DataField="Qty" TextAlign="Right" />
                        <px:PXGridColumn DataField="BillableQty" TextAlign="Right" />
                        <px:PXGridColumn DataField="TranCuryUnitRate" TextAlign="Right" />
                        <px:PXGridColumn DataField="Billable" TextAlign="Center" Type="CheckBox" />                                       
                        <px:PXGridColumn DataField="BranchID" />
                        <px:PXGridColumn DataField="TranID" />
                    </Columns>
                    <RowTemplate>
                        <px:PXSelector runat="server" ID="edBatchNbr2" DataField="BatchNbr" AllowEdit="True" Size="s" />
                        <px:PXSelector runat="server" ID="edRefNbr2" DataField="RefNbr" AllowEdit="True" Size="s" />
                    </RowTemplate>
                </px:PXGridLevel>
            </Levels>
            <ActionBar>
                <Actions>
                    <AddNew MenuVisible="False" ToolBarVisible="False" />
                    <Delete MenuVisible="False" ToolBarVisible="False" />
                    <NoteShow MenuVisible="False" ToolBarVisible="False" />
                </Actions>
            </ActionBar>
            <Mode AllowAddNew="False" AllowDelete="True" AllowUpdate="False" />
        </px:PXGrid>
         <px:PXPanel ID="PXPanelBtn" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButtonAdd" runat="server" Text="Add" CommandName="AppendTransactions" CommandSourceID="ds" />
            <px:PXButton ID="PXButtonAddClose" runat="server" Text="Add & Close" DialogResult="OK" />
            <px:PXButton ID="PXButtonClose" runat="server" DialogResult="Cancel" Text="Close" />
        </px:PXPanel>
    </px:PXSmartPanel>
</asp:Content>
