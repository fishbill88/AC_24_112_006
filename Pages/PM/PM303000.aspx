<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="PM303000.aspx.cs" Inherits="Page_PM303000"
    Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource EnableAttributes="True" ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.PM.ProgressWorksheetEntry" PrimaryView="Document" BorderStyle="NotSet">
        <CallbackCommands>
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Document" Caption="Progress Worksheet Summary" FilesIndicator="True"
        NoteIndicator="True" BPEventsIndicator="true" ActivityIndicator="True" ActivityField="NoteActivity" LinkPage="">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SS" ControlSize="XM"/>
            <px:PXSelector ID="edRefNbr" runat="server" DataField="RefNbr" AutoRefresh="true" />
            <px:PXDropDown ID="edStatus" runat="server" DataField="Status" />
            <px:PXSelector CommitChanges="True" ID="edProjectID" runat="server" DataField="ProjectID" DataSourceID="ds" AllowEdit="True"/>
            <px:PXDateTimeEdit ID="edDate" runat="server" DataField="Date" CommitChanges="true"/>
            <px:PXLayoutRule runat="server" ColumnSpan="2" />
            <px:PXTextEdit ID="edDescription" runat="server" DataField="Description" />

            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SS" ControlSize="XM"/>
            <px:PXSelector CommitChanges="True" ID="edOwnerID" runat="server" DataField="Project.OwnerID" DataSourceID="ds" Enabled ="False"/>
            <px:PXSelector CommitChanges="True" ID="edCreatedByID" runat="server" DataField="CreatedByID" DataSourceID="ds" Enabled ="False"/>
            <px:PXSelector CommitChanges="True" ID="edLastModifiedByID" runat="server" DataField="LastModifiedByID" DataSourceID="ds" Enabled ="False"/>
            <px:PXTextEdit ID="edDFR" runat="server" DataField="DailyFieldReportCD" Enabled="false">
                <LinkCommand Target="ds" Command="ViewDailyFieldReport" ></LinkCommand>
            </px:PXTextEdit>
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXTab ID="tab" runat="server" Width="100%" Height="511px" DataSourceID="ds" DataMember="DocumentSettings">
        <Activity HighlightColor="" SelectedColor="" Width="" Height=""></Activity>
        <Items >
            <px:PXTabItem Text="Details">
                <Template>
                    <px:PXGrid ID="DetailsGrid" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" Height="150px" SkinID="DetailsInTab" SyncPosition="True" RepaintColumns="true">
                        <Levels>
                            <px:PXGridLevel DataMember="Details">
                                <RowTemplate>
                                    <px:PXSelector ID="edDTaskID" runat="server" DataField="TaskID" />
                                    <px:PXSelector ID="edDInventoryID" runat="server" DataField="InventoryID" />
                                    <px:PXSelector ID="edDCostCodeID" runat="server" DataField="CostCodeID" />
                                    <px:PXSelector ID="edDAccountGroupID" runat="server" DataField="AccountGroupID" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="OwnerID"/>
                                    <px:PXGridColumn DataField="WorkgroupID" />
                                    <px:PXGridColumn AutoCallBack="True" DataField="TaskID" />
                                    <px:PXGridColumn AutoCallBack="True" DataField="InventoryID" />
                                    <px:PXGridColumn AutoCallBack="True" DataField="CostCodeID" />
                                    <px:PXGridColumn AutoCallBack="True" DataField="AccountGroupID" />
                                    <px:PXGridColumn DataField="Description"/>
                                    <px:PXGridColumn DataField="UOM"/>
                                    <px:PXGridColumn DataField="PreviouslyCompletedQuantity" />
                                    <px:PXGridColumn AutoCallBack="True" DataField="Qty" />
                                    <px:PXGridColumn DataField="PriorPeriodQuantity" />
                                    <px:PXGridColumn DataField="CurrentPeriodQuantity" />
                                    <px:PXGridColumn DataField="TotalCompletedQuantity" />
                                    <px:PXGridColumn DataField="CompletedPercentTotalQuantity" />
                                    <px:PXGridColumn DataField="TotalBudgetedQuantity" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <ActionBar>
                            <CustomItems>
                                <px:PXToolBarButton Text="Load Template" Tooltip="Load Template">
                                    <AutoCallBack Command="LoadTemplate" Target="ds">
                                        <Behavior CommitChanges="True" />
                                    </AutoCallBack>
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Text="Select Budget Lines" Tooltip="Select Budget Lines">
                                    <AutoCallBack Command="SelectBudgetLines" Target="ds">
                                        <Behavior CommitChanges="True" />
                                    </AutoCallBack>
                                </px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
                        <AutoSize Enabled="True" MinHeight="150" />
                        <Mode InitNewRow="True"  AllowUpload="True" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Approvals">
                <Template>
                    <px:PXGrid ID="gridApproval" runat="server" DataSourceID="ds" Width="100%" SkinID="DetailsInTab" NoteIndicator="True">
                        <AutoSize Enabled="true" />
                        <Mode AllowAddNew="false" AllowDelete="false" AllowUpdate="false" />

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
                                    <px:PXGridColumn DataField="AssignmentMapID"  Visible="false" SyncVisible="false"/>
                                    <px:PXGridColumn DataField="RuleID" Visible="false" SyncVisible="false" />
                                    <px:PXGridColumn DataField="StepID" Visible="false" SyncVisible="false" />
                                    <px:PXGridColumn DataField="CreatedDateTime" Visible="false" SyncVisible="false" />
                                </Columns>
                                <Layout FormViewHeight="" />
                            </px:PXGridLevel>
                        </Levels>
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
        </Items>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
    </px:PXTab>

    <px:PXSmartPanel ID="PanelSelectBudgetLines" runat="server" Key="costBudgets" LoadOnDemand="true" Width="1100px" Height="500px"
        Caption="Select Budget Lines" CaptionVisible="true" AutoCallBack-Command='Refresh' AutoCallBack-Enabled="True" AutoCallBack-Target="formSelectBudgetLines"
        DesignView="Hidden" AutoRepaint="true">
        <px:PXFormView ID="formSelectBudgetLines" runat="server" CaptionVisible="False" DataMember="costBudgetfilter" DataSourceID="ds"
            Width="100%" SkinID="Transparent">
            <Template>
                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
                <px:PXSelector CommitChanges="True" ID="edTaskID" runat="server" DataField="TaskID" />
                <px:PXSelector CommitChanges="True" ID="edAccountGroupID" runat="server" DataField="AccountGroupID" />
                <px:PXSelector CommitChanges="True" ID="edInventoryID" runat="server" DataField="InventoryID" />
                <px:PXLayoutRule  runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" />
                <px:PXSelector CommitChanges="True" ID="edCostCodeFrom" runat="server" DataField="CostCodeFrom" />
                <px:PXSelector CommitChanges="True" ID="edCostCodeTo" runat="server" DataField="CostCodeTo" />
            </Template>
        </px:PXFormView>
        <px:PXGrid ID="gridCostBudgetLines" runat="server" DataSourceID="ds" Style="border-width: 1px 0px; top: 0px; left: 0px;" AutoAdjustColumns="true"
            Width="100%" SkinID="Details" AdjustPageSize="Auto" Height="135px" AllowSearch="True"  SyncPosition="true">
            <CallbackCommands>
                <Refresh CommitChanges="true"></Refresh>
            </CallbackCommands>
            <ActionBar PagerVisible="False">
                <PagerSettings Mode="NextPrevFirstLast" />
            </ActionBar>
            <Levels>
                <px:PXGridLevel DataMember="costBudgets">
                    <Mode AllowAddNew="false" AllowDelete="false" AllowUpdate="False"/>
                    <Columns>
                        <px:PXGridColumn DataField="Selected" TextAlign="Center" Type="CheckBox" AllowCheckAll="true" />
                        <px:PXGridColumn DataField="ProjectTaskID" />
                        <px:PXGridColumn DataField="InventoryID" />
                        <px:PXGridColumn DataField="CostCodeID" />
                        <px:PXGridColumn DataField="AccountGroupID" />
                        <px:PXGridColumn DataField="Description" />
                        <px:PXGridColumn DataField="UOM" />
                        <px:PXGridColumn DataField="RevisedQty" />
                    </Columns>
                </px:PXGridLevel>
            </Levels>
            <AutoSize Enabled="true" />
        </px:PXGrid>
        <px:PXPanel ID="PXPanel5" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton6" runat="server" CommandName="AddSelectedBudgetLines" CommandSourceID="ds" Text="Add" SyncVisible="false"/>
            <px:PXButton ID="PXButton7" runat="server" Text="Add & Close" DialogResult="OK"/>
            <px:PXButton ID="PXButton8" runat="server" DialogResult="Cancel" Text="Cancel" />
        </px:PXPanel>
    </px:PXSmartPanel>

    <!--#include file="~\Pages\Includes\CRApprovalReasonPanel.inc"-->
    <!--#include file="~\Pages\Includes\EPReassignApproval.inc"-->
</asp:Content>
