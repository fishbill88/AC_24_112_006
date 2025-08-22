<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="CR306000.aspx.cs" Inherits="Page_CR306000"
    Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource EnableAttributes="true" ID="ds" runat="server" UDFTypeField="CaseClassID" Visible="True" Width="100%" TypeName="PX.Objects.CR.CRCaseMaint" PrimaryView="Case">
        <DataTrees>
            <px:PXTreeDataMember TreeView="_EPCompanyTree_Tree_" TreeKeys="WorkgroupID" />
        </DataTrees>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">

    <px:PXSmartPanel ID="panelCreateServiceOrder" runat="server" Style="z-index: 108; left: 27px; position: absolute; top: 99px;" Caption="Create Service Order" Width="390px" Height="180px" AutoRepaint="true"
        CaptionVisible="true" DesignView="Hidden" LoadOnDemand="true" Key="CreateServiceOrderFilter" AutoCallBack-Enabled="true" AutoCallBack-Command="Refresh"
        CallBackMode-CommitChanges="True" CallBackMode-PostData="Page">
        <px:PXFormView ID="formCreateServiceOrder" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" CaptionVisible="False" DataMember="CreateServiceOrderFilter">
            <ContentStyle BackColor="Transparent" BorderStyle="None" />
            <Template>
                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
                <px:PXSelector ID="edSrvOrdType" runat="server" AllowNull="False" DataField="SrvOrdType" DisplayMode="Text" CommitChanges="True" AutoRefresh="true" AllowEdit="True" />
                <px:PXSelector ID="edBranchLocationID" runat="server" DataField="BranchLocationID" DisplayMode="Text" CommitChanges="True" AutoRefresh="true" AllowEdit="True"/>
                <px:PXSelector ID="edAssignedEmpID" runat="server" DataField="AssignedEmpID" DisplayMode="Text" CommitChanges="True" AutoRefresh="true" AllowEdit="True"/>
                <px:PXSelector ID="edProblemID" runat="server" DataField="ProblemID" DisplayMode="Text" CommitChanges="True" AutoRefresh="true" AllowEdit="True"/>
            </Template>
        </px:PXFormView>

        <div style="padding: 5px; text-align: right;">
                <px:PXButton ID="btnSave2" runat="server" CommitChanges="True" DialogResult="OK" Text="OK" Height="20"/>
                <px:PXButton ID="btnCancel2" runat="server" DialogResult="Cancel" Text="Cancel" Height="20" />
        </div>
    </px:PXSmartPanel>

    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Case" Caption="Case Summary" NoteIndicator="True" FilesIndicator="True"
        LinkIndicator="True" BPEventsIndicator="True" DefaultControlID="edCaseCD">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
            <px:PXSelector ID="edCaseCD" runat="server" DataField="CaseCD" FilterByAllFields="True" AutoRefresh="True" />
            <px:PXSelector CommitChanges="True" ID="edCaseClassID" runat="server" DataField="CaseClassID" AllowEdit="True" FilterByAllFields="True" TextMode="Search" DisplayMode="Hint" AutoRefresh="True" />
            <px:PXDropDown CommitChanges="True" ID="edStatus" runat="server" DataField="Status" AllowNull="False" />
            <px:PXDropDown CommitChanges="True" ID="edResolution" runat="server" DataField="Resolution" AllowNull="False"/>

            <px:PXLayoutRule runat="server" ColumnSpan="2" />
            <px:PXTextEdit ID="edSubject" runat="server" DataField="Subject" />

            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" StartGroup="True" />
            <px:PXSegmentMask CommitChanges="True" ID="edCustomerID" runat="server" DataField="CustomerID" AllowEdit="True" FilterByAllFields="True" TextMode="Search" DisplayMode="Hint"
                              AutoRefresh="True" />
            <px:PXSegmentMask CommitChanges="True" ID="edLocationID" runat="server" DataField="LocationID" AutoRefresh="True" AllowEdit="True" FilterByAllFields="True" TextMode="Search"
                              DisplayMode="Hint" />

            <px:PXLayoutRule runat="server" Merge="True" />
            <px:PXSelector CommitChanges="True" ID="edContactID" runat="server" DataField="ContactID" DisplayMode="Text" TextMode="Search" TextField="displayName" AutoRefresh="True"
                           AllowEdit="True" FilterByAllFields="True"  OnEditRecord="edContactID_EditRecord"/>
            <px:PXButton runat="server" CommandSourceID="ds" CommandName="ContactTeamsCardOffline" ID="edContactTeamsCardOffline" CssClass="Button teamsImageButton" Style="margin-left: 28px" Width="20" Height="24">
                <Images Normal="svg:teams@teams_offline" />
            </px:PXButton>
            <px:PXButton runat="server" CommandSourceID="ds" CommandName="ContactTeamsCardAvailable" ID="edContactTeamsCardAvailable" CssClass="Button teamsImageButton" Style="margin-left: 28px" Width="20" Height="24">
                <Images Normal="svg:teams@teams_available" />
            </px:PXButton>
            <px:PXButton runat="server" CommandSourceID="ds" CommandName="ContactTeamsCardBusy" ID="edContactTeamsCardBusy" CssClass="Button teamsImageButton" Style="margin-left: 28px" Width="20" Height="24">
                <Images Normal="svg:teams@teams_busy" />
            </px:PXButton>
            <px:PXButton runat="server" CommandSourceID="ds" CommandName="ContactTeamsCardAway" ID="edContactTeamsCardAway" CssClass="Button teamsImageButton" Style="margin-left: 28px" Width="20" Height="24">
                <Images Normal="svg:teams@teams_away" />
            </px:PXButton>

            <px:PXLayoutRule runat="server" Merge="True" />
            <px:PXSelector ID="OwnerID" runat="server" DataField="OwnerID" TextMode="Search" DisplayMode="Text" FilterByAllFields="True" AutoRefresh="true" CommitChanges="True"/>
            <px:PXButton runat="server" CommandSourceID="ds" CommandName="OwnerTeamsCardOffline" ID="edOwnerTeamsCardOffline" CssClass="Button teamsImageButton" Width="20" Height="24">
                <Images Normal="svg:teams@teams_offline" />
            </px:PXButton>
            <px:PXButton runat="server" CommandSourceID="ds" CommandName="OwnerTeamsCardAvailable" ID="edOwnerTeamsCardAvailable" CssClass="Button teamsImageButton" Width="20" Height="24">
                <Images Normal="svg:teams@teams_available" />
            </px:PXButton>
            <px:PXButton runat="server" CommandSourceID="ds" CommandName="OwnerTeamsCardBusy" ID="edOwnerTeamsCardBusy" CssClass="Button teamsImageButton" Width="20" Height="24">
                <Images Normal="svg:teams@teams_busy" />
            </px:PXButton>
            <px:PXButton runat="server" CommandSourceID="ds" CommandName="OwnerTeamsCardAway" ID="edOwnerTeamsCardAway" CssClass="Button teamsImageButton" Width="20" Height="24">
                <Images Normal="svg:teams@teams_away" />
            </px:PXButton>

            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="SM" StartGroup="True" />
            <px:PXDateTimeEdit ID="edReportedOnDateTime" runat="server" DataField="ReportedOnDateTime" DisplayFormat="g" EditFormat="g" Size="SM" />
            <px:PXDropDown CommitChanges="True" ID="edSeverity" runat="server" DataField="Severity" SelectedIndex="-1" AllowNull="False" />
            <px:PXDropDown ID="edPriority" runat="server" DataField="Priority" SelectedIndex="-1" AllowNull="False" />
            <px:PXDateTimeEdit ID="edResolutionDate" runat="server" DataField="ResolutionDate" Size="SM"/>
            <px:PXDateTimeEdit ID="edInitialResponseDueDateTime" runat="server" DataField="HeaderInitialResponseDueDateTime" Size="SM" />
            <px:PXDateTimeEdit ID="edResponseDueDateTime" runat="server" DataField="HeaderResponseDueDateTime" Size="SM" />
            <px:PXDateTimeEdit ID="edResolutionDueDateTime" runat="server" DataField="HeaderResolutionDueDateTime" Size="SM" />

        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXTab ID="tab" runat="server" Width="100%" Height="400px" DataSourceID="ds" DataMember="CaseCurrent" MarkRequired="Dynamic">
        <Items>
            <px:PXTabItem Text="Details">
                <Template>
                    <px:PXRichTextEdit ID="edDescription" runat="server" Style="border-width: 0px; width: 100%;" DataField="Description" 
						AllowLoadTemplate="false"  AllowDatafields="false"  AllowMacros="true" AllowSourceMode="true" AllowAttached="true" AllowSearch="true">
						<AutoSize Enabled="True" MinHeight="100" />
						<LoadTemplate TypeName="PX.SM.SMNotificationMaint" DataMember="Notifications" ViewName="NotificationTemplate" ValueField="notificationID" TextField="Name" DataSourceID="ds" Size="M"/>
					</px:PXRichTextEdit>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="CRM Info">
                <Template>
                    <px:PXLayoutRule ID="PXLayoutRule5" runat="server" StartGroup="True" LabelsWidth="SM" ControlSize="XM" GroupCaption="CRM" />
                    <px:PXSelector ID="WorkgroupID" runat="server" DataField="WorkgroupID" CommitChanges="True" TextMode="Search" DisplayMode="Text" FilterByAllFields="True" />
                    <px:PXCheckBox ID="edIsActive" runat="server" DataField="IsActive" />

                    <px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartGroup="True" LabelsWidth="SM" ControlSize="XM" GroupCaption="Entitlement" />
                    <px:PXSelector CommitChanges="True" ID="edContractID" runat="server" DataField="ContractID" DisplayMode="Hint" TextMode="Search" AutoRefresh="True" AllowEdit="True"
                        FilterByAllFields="True" />

                    <px:PXLayoutRule ID="PXLayoutRule9" runat="server" StartGroup="True" LabelsWidth="SM" ControlSize="XM" GroupCaption="Billing" />
                    <px:PXCheckBox CommitChanges="True" ID="chkIsBillable" runat="server" DataField="IsBillable" />
                    <px:PXCheckBox CommitChanges="True" ID="chkManualBillableTimes" runat="server" DataField="ManualBillableTimes" />
                    <px:PXTimeEdit ID="edTimeBillable" runat="server" DataField="TimeBillable"/>
                    <px:PXTimeEdit ID="edOvertimeBillable" runat="server" DataField="OvertimeBillable"/>

                    <px:PXLayoutRule ID="PXLayoutRule4" runat="server" StartGroup="True" LabelsWidth="SM" ControlSize="XM" GroupCaption="Statistics" />
                    <px:PXMaskEdit ID="edTimeSpent" runat="server" DataField="TimeSpent" InputMask="### hrs ## mins" Size="SM" />
                    <px:PXMaskEdit ID="edOvertimeSpent" runat="server" DataField="OvertimeSpent" InputMask="### hrs ## mins" Size="SM" />
                    <px:PXMaskEdit ID="edInitResponse" runat="server" DataField="InitResponse" InputMask="### hrs ## mins" Size="SM" />
                    <px:PXMaskEdit ID="edTimeResolution" runat="server" DataField="TimeResolution" InputMask="### hrs ## mins" Size="SM" />

                    <px:PXLayoutRule ID="PXLayoutRule2" runat="server" StartGroup="True" LabelsWidth="SM" ControlSize="XM" StartColumn="True" GroupCaption="Commitments" />
                    <px:PXDateTimeEdit ID="edInitialResponseDueDateTime" runat="server" DataField="InitialResponseDueDateTime" Size="XM" />
                    <px:PXDateTimeEdit ID="edResponseDueDateTime" runat="server" DataField="ResponseDueDateTime" Size="XM" />
                    <px:PXDateTimeEdit ID="edResolutionDueDateTime" runat="server" DataField="ResolutionDueDateTime" Size="XM" />
                    <px:PXSelector ID="edSolutionActivityNoteID" runat="server" DataField="SolutionActivityNoteID" AutoRefresh="True" AllowEdit="true" />

                    <px:PXLayoutRule ID="PXLayoutRule3" runat="server" StartGroup="True" LabelsWidth="SM" ControlSize="XM" GroupCaption="Activities" />
                    <px:PXDateTimeEdit ID="edFirstOutgoingDate" runat="server" DataField="CaseActivityStatistics.InitialOutgoingActivityCompletedAtDate" Enabled="False" Size="XM"/>
                    <px:PXDateTimeEdit ID="edLastActivity" runat="server" DataField="LastActivity" Enabled="False" Size="XM"/>
                    <px:PXDateTimeEdit ID="edLastIncomingDate" runat="server" DataField="CaseActivityStatistics.LastIncomingActivityDate" Enabled="False" Size="XM"/>
                    <px:PXDateTimeEdit ID="edLastOutgoingDate" runat="server" DataField="CaseActivityStatistics.LastOutgoingActivityDate" Enabled="False" Size="XM"/>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Attributes">
                <Template>
                    <px:PXGrid ID="PXGridAnswers" runat="server" DataSourceID="ds" SkinID="Inquire" Width="100%" Height="200px" MatrixMode="True">
                        <Levels>
                            <px:PXGridLevel DataMember="Answers">
                                <Columns>
                                    <px:PXGridColumn DataField="AttributeID" TextAlign="Left" AllowShowHide="False" TextField="AttributeID_description" />
    								<px:PXGridColumn DataField="isRequired" TextAlign="Center" Type="CheckBox" />
                                    <px:PXGridColumn DataField="Value" AllowShowHide="False" AllowSort="False" />
                                </Columns>
                                <Layout FormViewHeight="" />
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" MinHeight="200" />
                        <ActionBar>
                            <Actions>
                                <Search Enabled="False" />
                            </Actions>
                        </ActionBar>
                        <Mode AllowAddNew="False" AllowColMoving="False" AllowDelete="False" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>

            <!--#include file="~\Pages\CR\Includes\ActivityDetailsExt\ActivityDetailsExt_WithBillable.inc"-->

            <px:PXTabItem Text="Related Cases" LoadOnDemand="true">
                <Template>
                    <px:PXGrid ID="gridCaseReferencesDependsOn" runat="server" DataSourceID="ds" Height="162px" Style="z-index: 101; left: 0px; position: absolute; top: 0px;" AllowSearch="True"
                        ActionsPosition="Top" SkinID="Details" Width="100%" BorderWidth="0px" SyncPosition="True">
                        <Levels>
                            <px:PXGridLevel DataMember="CaseRefs">
                                <Columns>
                                    <px:PXGridColumn DataField="ChildCaseCD" Width="100px" RenderEditorText="True" AutoCallBack="True" LinkCommand="CaseRefs_CRCase_ViewDetails" />
                                    <px:PXGridColumn DataField="RelationType" Width="100px" RenderEditorText="True" AutoCallBack="true" />
                                    <px:PXGridColumn DataField="CRCaseRelated__Subject" Width="250px" />
                                    <px:PXGridColumn DataField="CRCaseRelated__Status" Width="100px" />
                                    <px:PXGridColumn DataField="CRCaseRelated__OwnerID" Width="150px" DisplayMode="Text" />
                                    <px:PXGridColumn DataField="CRCaseRelated__WorkgroupID" Width="150px" />
                                </Columns>
                                <Mode InitNewRow="true" />
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" MinHeight="100" MinWidth="300" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>

            <!--#include file="~\Pages\CR\Includes\CRRelationDetailsExt.inc"-->

			<px:PXTabItem Text="Closure Notes">
				<Template>
					<px:PXRichTextEdit ID="edClosureNotes" runat="server" Style="border-width: 0px; width: 100%;" DataField="ClosureNotes" 
						AllowLoadTemplate="false"  AllowDatafields="false"  AllowMacros="true" AllowSourceMode="true" AllowAttached="true" AllowSearch="true">
						<AutoSize Enabled="True" MinHeight="100" />
					</px:PXRichTextEdit>
				</Template>
			</px:PXTabItem>

			<px:PXTabItem Text="Sync Status">
		    <Template>
		        <px:PXGrid ID="syncGrid" runat="server" DataSourceID="ds" Height="150px" Width="100%" ActionsPosition="Top" SkinID="Inquire" SyncPosition="true">
		            <Levels>
		                <px:PXGridLevel DataMember="SyncRecs" DataKeyNames="SyncRecordID">
		                    <Columns>
		                        <px:PXGridColumn DataField="SYProvider__Name" />
		                        <px:PXGridColumn DataField="RemoteID" CommitChanges="True" LinkCommand="GoToSalesforce" />
		                        <px:PXGridColumn DataField="Status" />
		                        <px:PXGridColumn DataField="Operation" />
		                        <px:PXGridColumn DataField="SFEntitySetup__ImportScenario" />
		                        <px:PXGridColumn DataField="SFEntitySetup__ExportScenario" />
		                        <px:PXGridColumn DataField="LastErrorMessage" />
		                        <px:PXGridColumn DataField="LastAttemptTS" DisplayFormat="g" />
		                        <px:PXGridColumn DataField="AttemptCount" />
		                    </Columns>                               
		                    <Layout FormViewHeight="" />
		                </px:PXGridLevel>
		            </Levels>
		            <ActionBar>                        
		                <CustomItems>
		                    <px:PXToolBarButton Key="SyncSalesforce">
		                        <AutoCallBack Command="SyncSalesforce" Target="ds"/>
		                    </px:PXToolBarButton>
		                </CustomItems>
		            </ActionBar>
		            <Mode InitNewRow="true" />
		            <AutoSize Enabled="True" MinHeight="150" />
		        </px:PXGrid>
		    </Template>
		</px:PXTabItem>
			<px:PXTabItem Text="Owner User" Visible="False">
				<Template>
					<px:PXFormView ID="frmOwnerUser" runat="server" DataMember="OwnerUser" DataSourceID="ds">
						<Template>
							<px:PXTextEdit ID="edPKID" runat="server" DataField="PKID" />
						</Template>
					</px:PXFormView>
				</Template>
			</px:PXTabItem>
        </Items>
        <AutoSize Container="Window" Enabled="True" MinHeight="100" MinWidth="300" />
    </px:PXTab>


	<!--#include file="~\Pages\CR\Includes\TeamsContactPanel.inc"-->

    <px:PXSmartPanel ID="panelCreateReturnOrder" runat="server" Style="z-index: 108; left: 27px; position: absolute; top: 99px;" Caption="Create Return Order" AutoRepaint="true"
        CaptionVisible="true" DesignView="Hidden" LoadOnDemand="true" Key="CreateOrderParams" AutoCallBack-Enabled="true" AutoCallBack-Command="Refresh"
        CallBackMode-CommitChanges="True" CallBackMode-PostData="Page">
        <px:PXFormView ID="frmCreateReturnOrder" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" CaptionVisible="False" 
                DataMember="CreateOrderParams" DefaultControlID="edOrderType">
            <ContentStyle BackColor="Transparent" BorderStyle="None" />
            <Template>
                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
                <px:PXSelector ID="edOrderType" runat="server" AllowNull="False" DataField="OrderType" DisplayMode="Hint" CommitChanges="True" />
            </Template>
        </px:PXFormView>

        <px:PXPanel ID="pnlCreateReturnOrderButtons" runat="server" SkinID="Transparent">

            <px:PXButton ID="btnCreateReturnOrderCancel" RightButtonMenu="true" runat="server" 
                style="float: right; margin: 2px; margin-right: 18px;" 
                DialogResult="Cancel" Text="Cancel"/>

            <px:PXButton ID="btnCreateReturnOrderOK" runat="server" CommandSourceID="ds" CommandName="CreateReturnOrderInPanel" 
                style="float: right; margin: 2px" 
                Text="Create" DialogResult="OK"/>

        </px:PXPanel>

    </px:PXSmartPanel>

</asp:Content>
