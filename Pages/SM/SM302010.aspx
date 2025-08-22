<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SM302010.aspx.cs" Inherits="Page_SM302000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" TypeName="PX.BusinessProcess.UI.QueueDispatchersMonitor" SuspendUnloading="False" PrimaryView="Status">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="viewLog"  Visible="False" PopupPanel="PanelLogDetails" RepaintControls="All"/>
            <px:PXDSCallbackCommand Name="clearStatistics"  Visible="False" RepaintControls="All"/>
            <px:PXDSCallbackCommand Name="showErrors"  Visible="True" PopupPanel="pnlErrors" RepaintControls="All"/>
            <px:PXDSCallbackCommand Name="viewPerMinuteStatisticDetails"  Visible="false" PopupPanel="PanelStatisticDetails" RepaintControls="All"/>
            <px:PXDSCallbackCommand Name="viewPerHourStatisticDetails"  Visible="False" PopupPanel="PanelStatisticDetails" RepaintControls="All"/>
            <px:PXDSCallbackCommand Name="purgeQueue"  Visible="True" RepaintControls="All"/>
            <px:PXDSCallbackCommand Name="restartDispatcher"  Visible="True" RepaintControls="All"/>
            <px:PXDSCallbackCommand Name="clearErrors" Visible="False" DependOnGrid="grid1"/>
            <px:PXDSCallbackCommand Name="showSourceData" Visible="False" DependOnGrid="grid1" PopupPanel="pnlViewCurrentError"/>
            <px:PXDSCallbackCommand Name="clearLog"  Visible="False" RepaintControls="All"/>
            <px:PXDSCallbackCommand Name="setNotificationSettings"  Visible="True" PopupPanel="pnlSettings" RepaintControls="All" CommitChanges="True" />
        </CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
    <px:PXGrid ID="grid1" runat="server" DataSourceID="ds" Style="z-index: 100" 
                                   Width="100%" Height="120px" SkinID="Details" TabIndex="5700" AutoAdjustColumns="true"
               SyncPosition="True" KeepPosition="True" AllowPaging="True" AdjustPageSize="Auto">
        <ActionBar>
            <Actions>
                <AddNew Enabled="False" />
                <Delete Enabled="False"/>
            </Actions>
        </ActionBar>
        <Levels>
            <px:PXGridLevel  DataMember="Status">
                <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" AllowRowSelect="True" />
                <Columns>
                    <px:PXGridColumn DataField="QueueType"/>
                    <px:PXGridColumn DataField="Status" Width="20px" />
                    <px:PXGridColumn DataField="QueueCount" />
                    <px:PXGridColumn DataField="QueueSize" />
                    <px:PXGridColumn DataField="QueueName" />
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Parent" Enabled="True" MinHeight="120" />
        <AutoCallBack Command="Refresh" Target="grid0,grid01,grid10,form10" ActiveBehavior="True">
            <Behavior RepaintControlsIDs="grid0,grid01,grid10,form10" BlockPage="True" CommitChanges="True" />
        </AutoCallBack>
    </px:PXGrid>
    <px:PXSmartPanel ID="PanelLogDetails" runat="server" style="height:460px;width:800px;" CaptionVisible="True" Caption="Queue Processing Log" Key="DispatcherLogDetail"
         AutoReload="True" AutoRepaint="True">
        <px:PXFormView ID="LogDetailView" runat="server" DataSourceID="ds" Style="z-index: 500" 
                       Width="100%" Height="400px" DataMember="DispatcherLogDetail" TabIndex="5500">
            <Template>
                <px:PXLayoutRule runat="server" ControlSize="M" LabelsWidth="SM"/>
                <px:PXTextEdit ID="edNProcessTime" runat="server" DataField="ProcessingTime" AlreadyLocalized="False"/>
                <px:PXLayoutRule runat="server" StartRow="True" ColumnWidth="750px" />
                <px:PXTextEdit ID="edNLog" runat="server" DataField="Log" Enabled="False" TextMode="MultiLine" Height="350" SuppressLabel="True" ></px:PXTextEdit>
            </Template>
        </px:PXFormView>
        <px:PXPanel ID="PXPanel5" SkinID="Buttons" runat="server">
            <px:PXButton ID="PanelNqdLogDetailsSubmit" runat="server" DialogResult="OK" Text="Close" />
		</px:PXPanel>
    </px:PXSmartPanel>
    <px:PXSmartPanel ID="PanelStatisticDetails" runat="server" style="height:600px;width:800px;" CaptionVisible="True" Caption="Trigger Details" Key="DetailsFilter"
                     AutoReload="True" AutoRepaint="True">
        <px:PXFormView ID="StatisticDetailsView" runat="server" DataSourceID="ds" Style="z-index: 500" 
                       Width="100%" DataMember="DetailsFilter" TabIndex="5500">
            <Template>
                <px:PXLayoutRule runat="server" StartRow="true" ControlSize="S" LabelsWidth="XS"/>
                <px:PXLabel ID="PXLabel1" runat="server" Style="z-index: 100; padding-left: 9px; position: relative;" 
                    Text="Details are logged only if the Log Trigger Details check box is selected on the Statistics tab."/>
                <px:PXLayoutRule runat="server" StartRow="true" ControlSize="S" LabelsWidth="XS"/>
                <px:PXLayoutRule ID="PXLayoutRule3" runat="server" Merge="True" />
                <px:PXDateTimeEdit ID="edFromDate" runat="server" DataField="From_Date" CommitChanges="true"/>
                <px:PXDateTimeEdit ID="edFromTime" runat="server" DataField="From_Time" CommitChanges="true" Width="84" TimeMode="true" SuppressLabel="true"/>
                <px:PXLayoutRule ID="PXLayoutRule1" runat="server" Merge="True" StartColumn="True" ControlSize="S" LabelsWidth="XS"/>
                <px:PXDateTimeEdit ID="edToDate" runat="server" DataField="To_Date" CommitChanges="true"/>
                <px:PXDateTimeEdit ID="edToTime" runat="server" DataField="To_Time" CommitChanges="true" Width="84" TimeMode="true" SuppressLabel="true"/>
                <px:PXTextEdit ID="edCurrentQueueType" runat="server" DataField="QueueType"></px:PXTextEdit>
            </Template>
        </px:PXFormView>
        <px:PXTab ID="StatisticDetailsTab" runat="server" Width="100%" Height="500px">
            <Items>
                <px:PXTabItem Text="Triggering Fields"  LoadOnDemand="true" VisibleExp="DataControls[&quot;edCurrentQueueType&quot;].Value == PN" BindingContext="StatisticDetailsView">
                    <Template>
                        <px:PXGrid ID="grid40" runat="server" DataSourceID="ds" Style="z-index: 100" 
                                   Width="100%" SkinID="Details" TabIndex="5700" AutoAdjustColumns="true" SyncPosition="True" 
                                   AllowPaging="True">
                            <ActionBar>
                                <Actions>
                                    <AddNew Enabled="False" />
                                    <Delete Enabled="False"/>
                                </Actions>
                            </ActionBar>
                            <Levels>
                                <px:PXGridLevel  DataMember="TriggeredByFields">
                                    <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" AllowRowSelect="True" />
                                    <Columns>
                                        <px:PXGridColumn DataField="Query" />
                                        <px:PXGridColumn DataField="Field"/>
                                        <px:PXGridColumn DataField="Count"/>
                                    </Columns>
                                </px:PXGridLevel>
                            </Levels>
                            <AutoSize Container="Parent" Enabled="True" MinHeight="150" />
                        </px:PXGrid>
                    </Template>
                </px:PXTabItem>
                <px:PXTabItem Text="Source DACs"  LoadOnDemand="true" VisibleExp="DataControls[&quot;edCurrentQueueType&quot;].Value == PN" BindingContext="StatisticDetailsView">
                    <Template>
                        <px:PXGrid ID="grid41" runat="server" DataSourceID="ds" Style="z-index: 100" 
                                   Width="100%" SkinID="Details" TabIndex="5700" AutoAdjustColumns="true" SyncPosition="True" 
                                   AllowPaging="True">
                            <ActionBar>
                                <Actions>
                                    <AddNew Enabled="False" />
                                    <Delete Enabled="False"/>
                                </Actions>
                            </ActionBar>
                            <Levels>
                                <px:PXGridLevel  DataMember="TriggeredBySources">
                                    <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" AllowRowSelect="True" />
                                    <Columns>
                                        <px:PXGridColumn DataField="ScreenID" />
                                        <px:PXGridColumn DataField="TableName"/>
                                        <px:PXGridColumn DataField="Count"/>
                                    </Columns>
                                </px:PXGridLevel>
                            </Levels>
                            <AutoSize Container="Parent" Enabled="True" MinHeight="150" />
                        </px:PXGrid>
                    </Template>
                </px:PXTabItem>
                <px:PXTabItem Text="Business Event messages"  LoadOnDemand="true" VisibleExp="DataControls[&quot;edCurrentQueueType&quot;].Value == BE" BindingContext="StatisticDetailsView">
                    <Template>
                        <px:PXGrid ID="grid43" runat="server" DataSourceID="ds" Style="z-index: 100" 
                                   Width="100%" SkinID="Details" TabIndex="5700" AutoAdjustColumns="true" SyncPosition="True" 
                                   AllowPaging="True">
                            <ActionBar>
                                <Actions>
                                    <AddNew Enabled="False" />
                                    <Delete Enabled="False"/>
                                </Actions>
                            </ActionBar>
                            <Levels>
                                <px:PXGridLevel  DataMember="TriggeredByEvent">
                                    <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" AllowRowSelect="True" />
                                    <Columns>
                                        <px:PXGridColumn DataField="BusinessEventName" />
                                        <px:PXGridColumn DataField="Count"/>
                                    </Columns>
                                </px:PXGridLevel>
                            </Levels>
                            <AutoSize Container="Parent" Enabled="True" MinHeight="150" />
                        </px:PXGrid>
                    </Template>
                </px:PXTabItem>
                <px:PXTabItem Text="Commerce messages"  LoadOnDemand="true" VisibleExp="DataControls[&quot;edCurrentQueueType&quot;].Value == CO" BindingContext="StatisticDetailsView">
                    <Template>
                        <px:PXGrid ID="grid42" runat="server" DataSourceID="ds" Style="z-index: 100" 
                                   Width="100%" SkinID="Details" TabIndex="5700" AutoAdjustColumns="true" SyncPosition="True" 
                                   AllowPaging="True">
                            <ActionBar>
                                <Actions>
                                    <AddNew Enabled="False" />
                                    <Delete Enabled="False"/>
                                </Actions>
                            </ActionBar>
                            <Levels>
                                <px:PXGridLevel  DataMember="StatisticCommerceDetail">
                                    <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" AllowRowSelect="True" />
                                    <Columns>
                                        <px:PXGridColumn DataField="Connector" />
                                        <px:PXGridColumn DataField="Direction"/>
                                        <px:PXGridColumn DataField="Count"/>
                                    </Columns>
                                </px:PXGridLevel>
                            </Levels>
                            <AutoSize Container="Parent" Enabled="True" MinHeight="150" />
                        </px:PXGrid>
                    </Template>
                </px:PXTabItem>
            </Items>
            <AutoSize Container="Parent" Enabled="True" MinHeight="250" />
        </px:PXTab>
    </px:PXSmartPanel>
    <px:PXSmartPanel ID="pnlErrors" runat="server" Height="600px" Width="800px" Caption="Errors"
                     CaptionVisible="true" Key="Errors" AutoCallBack-Enabled="true" AllowResize="true">
        <px:PXGrid ID="grid60" runat="server" DataSourceID="ds" Style="z-index: 100" 
                   Width="100%" SkinID="Details" TabIndex="5700" AutoAdjustColumns="true" SyncPosition="True" AllowPaging="True" AdjustPageSize="Auto">
            <ActionBar>
                <CustomItems>
                    <px:PXToolBarButton CommandSourceID="ds" CommandName="clearErrors" />
                    <px:PXToolBarButton CommandSourceID="ds" CommandName="showSourceData" PopupPanel="pnlViewCurrentError"/>
                </CustomItems>
                <Actions>
                    <AddNew Enabled="False" />
                    <Delete Enabled="False"/>
                </Actions>
            </ActionBar>
            <Levels>
                <px:PXGridLevel  DataMember="Errors">
                    <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" AllowRowSelect="True" />
                    <Columns>
                        <px:PXGridColumn DataField="HookId"/>
                        <px:PXGridColumn DataField="Source" Width="150px" />
                        <px:PXGridColumn DataField="SourceEvent" />
                        <px:PXGridColumn DataField="ErrorMessage" />
                        <px:PXGridColumn DataField="TimeStamp" DisplayFormat="G"  />
                    </Columns>
                </px:PXGridLevel>
            </Levels>
            <AutoSize Container="Parent" Enabled="True" MinHeight="150" />
        </px:PXGrid>
    </px:PXSmartPanel>
    <px:PXSmartPanel ID="pnlViewCurrentError" runat="server" Height="600px" Width="800px" Caption="Source Data"
                     CaptionVisible="true" Key="CurrentError" AutoCallBack-Enabled="true"
                     AutoCallBack-Command="Refresh" AutoCallBack-Target="frmViewNotification" AllowResize="false">
        <px:PXFormView ID="frmViewNotification" runat="server" DataSourceID="ds" Width="100%"
                       CaptionVisible="False" DataMember="CurrentError">
            <ContentStyle BackColor="Transparent" BorderStyle="None">
            </ContentStyle>
            <Template>
                <px:PXTextEdit ID="edDetailsNotification" runat="server" DataField="SourceData" Height="550px" Style="z-index: 101; border-style: none;" TextMode="MultiLine"
                               Width="100%" SelectOnFocus="false">
					
                </px:PXTextEdit>
            </Template>
        </px:PXFormView>
        <px:PXPanel ID="PXPanel2" runat="server" SkinID="Buttons" >
            <px:PXButton ID="PXButton1" runat="server" DialogResult="OK" Text="Close" Width="63px" Height="20px">
            </px:PXButton>
        </px:PXPanel>
    </px:PXSmartPanel>
    <px:PXSmartPanel ID="pnlSettings" runat="server" Caption="Notification Settings" CaptionVisible="True"
                     LoadOnDemand="True" Width="540px"
                     Key="NotificationSettings"
                     ShowAfterLoad="true"
                     AutoCallBack-Enabled="true"
                     AutoCallBack-Command="Refresh"
                     AcceptButtonID="btnOk" DependsOnView="CurrentDispatcherSettings" 
                     CancelButtonID="btnCancel" AutoReload="True">
        <px:PXFormView ID="frmNotificationSettings1" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" SkinID="Transparent" DataMember="CurrentNotificationSettings" >
             <Template>
                 <px:PXNumberEdit ID="fillThreshold" runat="server" DataField="FillThreshold" LabelWidth="60%" CommitChanges="True" />
                 <px:PXLayoutRule runat="server" GroupCaption="Send" />
                 <px:PXFormView ID="frmNotificationSettings2" runat="server" DataSourceID="ds" Style="z-index: 100" Width="545px" SkinID="Transparent" DataMember="CurrentNotificationSettings" >
                     <Template>
                         <px:PXLayoutRule runat="server" Merge="True" />
                         <px:PXCheckBox runat="server" ID="isEmailActive" DataField="IsEmailActive" Size="SM" AlignLeft="true" CommitChanges="True" />
                         <px:PXSelector runat="server" ID="edEmailNotificationID" DataField="EmailNotificationID" LabelWidth="30%" Size="XM"  CommitChanges="True" AllowEdit="True" AllowAddNew="true" />
                         <px:PXLayoutRule runat="server" Merge="True" />
                         <px:PXCheckBox runat="server" ID="isMobileSmsActive" DataField="IsMobileSmsActive" Size="SM" AlignLeft="true" CommitChanges="True"/>
                         <px:PXSelector runat="server" ID="edMobileSmsNotificationID" DataField="MobileSmsNotificationID" LabelWidth="30%" Size="XM" AlignLeft="true" CommitChanges="True" AllowEdit="True" AllowAddNew="true"/>
                         <px:PXLayoutRule runat="server" Merge="True" />
                         <px:PXCheckBox runat="server" ID="isMobilePushActive" DataField="IsMobilePushActive" Size="SM" AlignLeft="true" CommitChanges="True"/>
                         <px:PXSelector runat="server" ID="edMobilePushNotificationID" DataField="MobilePushNotificationID" LabelWidth="30%" Size="XM" CommitChanges="True" AllowEdit="True" AllowAddNew="true"/>
                     </Template> 
                 </px:PXFormView>
             </Template> 
         </px:PXFormView>
        <px:PXPanel ID="PXPanel1" runat="server" SkinID="Buttons">
            <px:PXButton ID="btnOk" runat="server" DialogResult="OK" IsClientControl="False" Text="OK" CommandName="setNotificationSettingsOk" CommandSourceID="ds" />
			<px:PXButton ID="btnCancel" runat="server" DialogResult="Cancel" Text="Cancel" CommandName="setNotificationSettingsCancel" CommandSourceID="ds" />
        </px:PXPanel>
    </px:PXSmartPanel>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
    <px:PXTab ID="tab" runat="server" Width="100%" Height="400px">
		<Items>
            <px:PXTabItem Text="Statistics"  LoadOnDemand="true">
                <Template>
                    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100"
                                   Width="100%" DataMember="Settings" TabIndex="5500">
                        <Template>
                                <px:PXLayoutRule ID="PXLayoutRuleG1" runat="server" StartColumn="True" Merge="true" LabelsWidth="XM" ControlSize="M"/>
                                <px:PXNumberEdit ID="edKeepStatisticsForPeriod" runat="server" DataField="KeepStatisticsForPeriod" CommitChanges="true"/>
                                <px:PXLayoutRule ID="PXLayoutRuleG2" runat="server" StartColumn="True" Merge="true" LabelsWidth="M" ControlSize="M"/>
                                <px:PXCheckBox ID="edLogDetails" runat="server" DataField="LogDetails" CommitChanges="true"/>
                            </Template>
                    </px:PXFormView>
                    <px:PXTab ID="tab2" runat="server" Width="100%" Height="250px">
                        <Items>
                            <px:PXTabItem Text="Grouped by Hour" LoadOnDemand="true">
                                <Template>
                                    <px:PXGrid ID="grid0" runat="server" DataSourceID="ds" Style="z-index: 100" 
                                               Width="100%" Height="150px" SkinID="Details" TabIndex="5700" AutoAdjustColumns="true" SyncPosition="True" KeepPosition="True" AllowPaging="True" AdjustPageSize="Auto">
                                        <ActionBar>
                                            <CustomItems>
                                                <px:PXToolBarButton CommandSourceID="ds" CommandName="clearStatistics"/>
                                            </CustomItems>
                                            <Actions>
                                                <AddNew Enabled="False" />
                                                <Delete Enabled="False"/>
                                            </Actions>
                                        </ActionBar>
                                        <Levels>
                                            <px:PXGridLevel  DataMember="DispatcherStatisticsPerHour">
                                                <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" AllowRowSelect="True" />
                                                <Columns>
                                                    <px:PXGridColumn DataField="CreatedDateTime" Width="60px" DisplayFormat="dd-MM-yyyy HH:mm" LinkCommand="viewPerHourStatisticDetails"/>
                                                    <px:PXGridColumn DataField="Queued" />
                                                    <px:PXGridColumn DataField="Processed"/>
                                                    <px:PXGridColumn DataField="QueueSize"/>
                                                    <px:PXGridColumn DataField="AverageProcessingTime"/>
                                                    <px:PXGridColumn DataField="MaxProcessingTime"/>
                                                </Columns>
                                            </px:PXGridLevel>
                                        </Levels>
                                        <AutoSize Container="Parent" Enabled="True" MinHeight="150" />
                                    </px:PXGrid>
                                </Template>
                                <AutoCallBack Command="Refresh" Target="grid0" ActiveBehavior="True">
                                    <Behavior RepaintControlsIDs="grid0" BlockPage="True" CommitChanges="True" />
                                </AutoCallBack>
                            </px:PXTabItem>
                            <px:PXTabItem Text="Grouped by Minute"  LoadOnDemand="true">
                                <Template>
                                    <px:PXGrid ID="grid01" runat="server" DataSourceID="ds" Style="z-index: 100" 
                                               Width="100%" Height="150px" SkinID="Details" TabIndex="5700" AutoAdjustColumns="true" SyncPosition="True" KeepPosition="True" AllowPaging="True" AdjustPageSize="Auto">
                                        <ActionBar>
                                            <CustomItems>
                                                <px:PXToolBarButton CommandSourceID="ds" CommandName="clearStatistics"/>
                                            </CustomItems>
                                            <Actions>
                                                <AddNew Enabled="False" />
                                                <Delete Enabled="False"/>
                                            </Actions>
                                        </ActionBar>
                                        <Levels>
                                            <px:PXGridLevel  DataMember="DispatcherStatistics">
                                                <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" AllowRowSelect="True" />
                                                <Columns>
                                                    <px:PXGridColumn DataField="CreatedDateTime" Width="60px" DisplayFormat="dd-MM-yyyy HH:mm" CommitChanges="true" LinkCommand="viewPerMinuteStatisticDetails"/>
                                                    <px:PXGridColumn DataField="Queued" />
                                                    <px:PXGridColumn DataField="Processed"/>
                                                    <px:PXGridColumn DataField="QueueSize"/>
                                                    <px:PXGridColumn DataField="AverageProcessingTime"/>
                                                    <px:PXGridColumn DataField="MaxProcessingTime"/>
                                                </Columns>
                                            </px:PXGridLevel>
                                        </Levels>
                                        <AutoSize Container="Parent" Enabled="True" MinHeight="150" />
                                    </px:PXGrid>
                                </Template>
                                <AutoCallBack Command="Refresh" Target="grid01" ActiveBehavior="True">
                                    <Behavior RepaintControlsIDs="grid01" BlockPage="True" CommitChanges="True" />
                                </AutoCallBack>
                            </px:PXTabItem>
                        </Items>
                        <AutoSize Container="Parent" Enabled="True" MinHeight="250" />
                    </px:PXTab>
                </Template>
                <AutoCallBack Command="Refresh" Target="grid0,grid01" ActiveBehavior="True">
                    <Behavior RepaintControlsIDs="grid0,grid01" BlockPage="True" CommitChanges="True" />
                </AutoCallBack>
            </px:PXTabItem>
            <px:PXTabItem Text="Performance Issues"  LoadOnDemand="true">
                <Template>
                    <px:PXFormView ID="form10" runat="server" DataSourceID="ds" Style="z-index: 100"
                                              Width="100%" DataMember="CurrentDispatcherSettings" TabIndex="5500">
                        <Template>
                                <px:PXLayoutRule ID="PXLayoutRuleS1" runat="server" StartColumn="True" Merge="true" LabelsWidth="XM" ControlSize="M"/>
                                <px:PXNumberEdit ID="edLogMaxLength" runat="server" DataField="LogMaxLength" CommitChanges="true"/>
                                <px:PXLayoutRule ID="PXLayoutRuleS2" runat="server" StartColumn="True" Merge="true" LabelsWidth="XM" ControlSize="M"/>
                                <px:PXNumberEdit ID="edLongProcessingThreshold" runat="server" CommitChanges="true" DataField="LongProcessingThreshold"/>
                            </Template>
                    </px:PXFormView>
                    <px:PXGrid ID="grid10" runat="server" DataSourceID="ds" Style="z-index: 100" 
                            Width="100%" Height="150px" SkinID="Details" TabIndex="5700" AutoAdjustColumns="true" 
                               SyncPosition="True"  KeepPosition="True" AllowPaging="True" AdjustPageSize="Auto">
                        <ActionBar>
                            <CustomItems>
                                <px:PXToolBarButton CommandSourceID="ds" CommandName="clearLog" />
                            </CustomItems>
                            <Actions>
                                <AddNew Enabled="False" />
                                <Delete Enabled="False"/>
                            </Actions>
                        </ActionBar>
                        <Levels>
                            <px:PXGridLevel  DataMember="SlowLogs">
                                <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" AllowRowSelect="True" />
                                <Columns>
                                    <px:PXGridColumn DataField="CreatedDateTime" Width="60px" DisplayFormat="dd-MM-yyyy HH:mm:ss" LinkCommand="viewLog"/>
                                    <px:PXGridColumn DataField="ProcessingTime"/>
                                    <px:PXGridColumn DataField="Queries"/>
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Container="Parent" Enabled="True" MinHeight="150" />
                    </px:PXGrid>
                </Template>
                <AutoCallBack Command="Refresh" Target="grid10,form10" ActiveBehavior="True">
                    <Behavior RepaintControlsIDs="grid10,form10" BlockPage="True" CommitChanges="True" />
                </AutoCallBack>
            </px:PXTabItem>
            <%--<px:PXTabItem Text="Settings">
                <Template>
                    <px:PXFormView ID="SettingsForm" runat="server" DataSourceID="ds" Style="z-index: 100" 
                                   Width="100%" DataMember="settings" TabIndex="5900">
                        <Template>
                            <px:PXPanel runat="server" ID="PXPanel3" Caption="General Settings">
                                <px:PXLayoutRule runat="server" ControlSize="XXL" LabelsWidth="215" ColumnSpan="2"/>
                                <px:PXNumberEdit runat="server" ID="ed2KeepStatisticsForPeriod" DataField="KeepStatisticsForPeriod" CommitChanges="True"/>
                                <px:PXCheckBox runat="server" ID="ed2LogDetails" DataField="LogDetails" CommitChanges="True"/>
                            </px:PXPanel>
                            <px:PXPanel runat="server" ID="PXPanel1" Caption="Push Notification Queue">
                                <px:PXLayoutRule runat="server" ControlSize="XXL" LabelsWidth="215" ColumnSpan="2"/>
                                <px:PXNumberEdit runat="server" ID="edNqdLongProcessingThreshold" DataField="NqdLongProcessingThreshold" CommitChanges="True"/>
                                <px:PXNumberEdit runat="server" ID="edNqdLogMaxLength" DataField="NqdLogMaxLength" CommitChanges="True"/>
                            </px:PXPanel>
                            <px:PXLayoutRule runat="server" StartColumn="True" />
                            <px:PXPanel runat="server" ID="PXPanel2" Caption="Business Event Queue" >
                                <px:PXLayoutRule runat="server" ControlSize="XXL" LabelsWidth="215" ColumnSpan="2"/>
                                <px:PXNumberEdit runat="server" ID="edEqdLongProcessingThreshold" DataField="EqdLongProcessingThreshold" CommitChanges="True"/>
                                <px:PXNumberEdit runat="server" ID="edEqdLogMaxLength" DataField="EqdLogMaxLength" CommitChanges="True"/>
                            </px:PXPanel>
							<px:PXLayoutRule runat="server" StartColumn="True" />
                            <px:PXPanel runat="server" ID="PXPanel7" Caption="Commerce Queue" >
                                <px:PXLayoutRule runat="server" ControlSize="XXL" LabelsWidth="215" ColumnSpan="2"/>
                                <px:PXNumberEdit runat="server" ID="edCqdLongProcessingThreshold" DataField="CqdLongProcessingThreshold" CommitChanges="True"/>
                                <px:PXNumberEdit runat="server" ID="edCqdLogMaxLength" DataField="CqdLogMaxLength" CommitChanges="True"/>
                            </px:PXPanel>
                        </Template>
                    </px:PXFormView>
                </Template>
            </px:PXTabItem>--%>
        </Items>
	    <AutoSize Container="Window" Enabled="True" MinHeight="300" />
    </px:PXTab>
</asp:Content>
