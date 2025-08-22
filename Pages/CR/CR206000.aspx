<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="CR206000.aspx.cs" Inherits="Page_CR206000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" AutoCallBack="True" Visible="True" Width="100%" PrimaryView="CaseClasses" TypeName="PX.Objects.CR.CRCaseClassMaint">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Insert" PostData="Self" />
			<px:PXDSCallbackCommand Name="Save" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="First" PostData="Self" StartNewGroup="True" />
			<px:PXDSCallbackCommand Name="Last" PostData="Self" />			
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView
			ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="CaseClasses" Caption="Case Class Summary"
			FilesIndicator="True" NoteIndicator="True" ActivityIndicator="true" ActivityField="NoteActivity" DefaultControlID="edCaseClassID">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True" LabelsWidth="SM" ControlSize="XM" ColumnWidth="435"/>
			<px:PXSelector DataField="CaseClassID" ID="edCaseClassID" runat="server" FilterByAllFields="True" DisplayMode="Value" />
			<px:PXTextEdit DataField="Description" ID="edDescription" runat="server" />
			<px:PXCheckBox DataField="IsInternal" ID="chkInternal" runat="server" />
			<px:PXLayoutRule ID="PXLayoutRule2" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
			<px:PXSelector DataField="CalendarID" ID="edCalendarID" runat="server" CommitChanges="True" AllowEdit="True" />
			<px:PXMaskEdit ID="edWorkdayTime" runat="server" DataField="WorkCalendar.WorkdayTime" Enabled="False" />
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXTab ID="tab" runat="server" Width="100%" Height="253px" DataSourceID="ds" DataMember="CaseClassesCurrent" LoadOnDemand="True">
		<Items>
			<px:PXTabItem Text="Details" RepaintOnDemand="False">
				<Template>
					<px:PXLayoutRule ID="PXLayoutRule7" runat="server" LabelsWidth="SM" ControlSize="XM" GroupCaption="Data Entry Settings" />
					<px:PXCheckBox DataField="RequireCustomer" CommitChanges="True" SuppressLabel="True" ID="chkRequireCustomer" runat="server" />
					<px:PXCheckBox DataField="RequireContact" SuppressLabel="True" ID="chkRequireContact" runat="server" />
					<px:PXCheckBox DataField="AllowEmployeeAsContact" CommitChanges="True" SuppressLabel="True" ID="chkAllowEmployeeAsContact" runat="server" />
					<px:PXCheckBox DataField="RequireContract" CommitChanges="True" SuppressLabel="True" ID="chkRequireContract" runat="server" />
					<px:PXCheckBox DataField="RequireClosureNotes" SuppressLabel="True" ID="chkRequireClosureNotes" runat="server" />
					<px:PXLayoutRule ID="PXLayoutRule2" runat="server" LabelsWidth="SM" ControlSize="XM" GroupCaption="Billing Settings" />
					<px:PXCheckBox DataField="IsBillable" CommitChanges="True" SuppressLabel="True" ID="chkIsBillable" runat="server" />
					<px:PXCheckBox DataField="AllowOverrideBillable" CommitChanges="True" SuppressLabel="True" ID="chkAllowOverrideBillable" runat="server" />
					<px:PXDropDown DataField="PerItemBilling" ID="edPerItemBilling" runat="server" CommitChanges="True" AllowNull="False" />
					<px:PXSegmentMask DataField="OvertimeItemID" ID="edOvertimeItemID" runat="server" AllowEdit="True" DisplayMode="Hint" />
					<px:PXSegmentMask DataField="LabourItemID" ID="edLabourItemID" runat="server" AllowEdit="True" DisplayMode="Hint" />
					<px:PXTimeSpan DataField="MinBillTimeInMinutes" TimeMode="True" ID="edMinBillTimeInMinutes" runat="server" InputMask="hh:mm" AllowNull="False" Size="S" />
					<px:PXTimeSpan DataField="RoundingInMinutes" TimeMode="True" ID="edRoundingInMinutes" runat="server" InputMask="hh:mm" AllowNull="False" Size="S" />

					<px:PXLayoutRule ID="PXLayoutRule3" runat="server" LabelsWidth="SM" ControlSize="XM" StartColumn="True" GroupCaption="Activity Settings" />
					<px:PXSelector DataField="DefaultEMailAccountID" ID="edDefaultEMailAccount" runat="server" DisplayMode="Text" />
					<px:PXCheckBox DataField="TrackSolutionsInActivities" ID="chkTrackSolutionsInActivities" runat="server" SuppressLabel="True" CommitChanges="True" />
					<px:PXNumberEdit DataField="ReopenCaseTimeInDays" ID="edReopenCaseTimeInDays" runat="server" Size="S" AllowNull="True" />
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Commitments">
				<Template>
					<px:PXPanel runat="server" Width="100%" DataMember="CaseClassesCurrent" DataSourceID="ds" CaptionVisible="False" SkinID="Transparent">
						<px:PXLayoutRule ID="PXLayoutRule1" runat="server" LabelsWidth="SM" ControlSize="XM" GroupCaption="Resolution Time" />
						<px:PXDropDown ID="edStopTimeCounterType" runat="server" DataField="StopTimeCounterType" CommitChanges="True" AllowNull="False" />
					</px:PXPanel>
					<px:PXPanel runat="server" Width="100%" DataMember="CaseClassesReaction" DataSourceID="ds" CaptionVisible="False" SkinID="Transparent">
						<px:PXGrid ID="gridCaseClassesReaction" runat="server" SkinID="Details" DataSourceID="ds" Height="100px" Style="z-index: 100; left: 0px; position: absolute; top: 0px" Width="100%" BorderWidth="0" MatrixMode="true" >
							<Levels>
								<px:PXGridLevel DataMember="CaseClassesReaction">
									<Columns>
										<px:PXGridColumn DisplayFormat="&gt;a" DataField="Severity" Type="DropDownList" AllowMove="false" />

										<px:PXGridColumn DataField="TrackInitialResponseTime" CommitChanges="true" Type="CheckBox" AllowNull="False"  Width="130px" AllowMove="false" TextAlign="Right" Header-Tooltip="Initial Response Time Tracking" />
										<px:PXGridColumn DataField="InitialResponseTimeTarget" CommitChanges="true" AllowNull="False" TextAlign="left" Width="130px" AllowMove="false"/>
										<px:PXGridColumn DataField="InitialResponseGracePeriod" CommitChanges="true" AllowNull="False" TextAlign="left" Width="130px" AllowMove="false" />

										<px:PXGridColumn DataField="TrackResponseTime" CommitChanges="true" Type="CheckBox" AllowNull="False"  Width="130px" AllowMove="false"  TextAlign="Right" Header-Tooltip="Response Time Tracking" />
										<px:PXGridColumn DataField="ResponseTimeTarget" CommitChanges="true" AllowNull="False" TextAlign="left" Width="130px" AllowMove="false" />
										<px:PXGridColumn DataField="ResponseGracePeriod" CommitChanges="true" AllowNull="False" TextAlign="left" Width="130px" AllowMove="false" />

										<px:PXGridColumn DataField="TrackResolutionTime" CommitChanges="true" Type="CheckBox" AllowNull="False"  Width="130px"  AllowMove="false" TextAlign="Right" Header-Tooltip="Resolution Time Tracking" />
										<px:PXGridColumn DataField="ResolutionTimeTarget" CommitChanges="true" AllowNull="False" TextAlign="left" Width="130px" AllowMove="false" />
										<px:PXGridColumn DataField="ResolutionGracePeriod" CommitChanges="true" AllowNull="False" TextAlign="left" Width="130px" AllowMove="false" />

									</Columns>
								</px:PXGridLevel>
							</Levels>
							<AutoSize Enabled="True" MinHeight="100" MinWidth="100" />
						</px:PXGrid>
					</px:PXPanel>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Labor Items" VisibleExp="DataControls[&quot;edPerItemBilling&quot;].Value = 1" >
				<Template>
					<px:PXGrid ID="LaborClassesGrid" runat="server" SkinID="Details" ActionsPosition="Top" DataSourceID="ds" Width="100%" BorderWidth="0px" MatrixMode="True">
						<Levels>
							<px:PXGridLevel DataMember="LaborMatrix">
								<Columns>
									<px:PXGridColumn DataField="EarningType" CommitChanges="True" />
									<px:PXGridColumn DataField="LabourItemID" CommitChanges="True" />
								</Columns>
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Attributes">
				<Template>
					<px:PXGrid ID="AttributesGrid" runat="server" SkinID="Details" ActionsPosition="Top" DataSourceID="ds" Width="100%" BorderWidth="0px" MatrixMode="True">
						<Levels>
							<px:PXGridLevel DataMember="Mapping">
								<RowTemplate>
									<px:PXSelector CommitChanges="True" ID="edAttributeID" runat="server" DataField="AttributeID" AllowEdit="True" FilterByAllFields="True" AutoRefresh="True" />
								</RowTemplate>
								<Columns>
									<px:PXGridColumn DataField="IsActive" AllowNull="False" TextAlign="Center" Type="CheckBox" />
									<px:PXGridColumn DataField="AttributeID" AutoCallBack="True" LinkCommand="CRAttribute_ViewDetails" />
									<px:PXGridColumn DataField="Description" AllowNull="False" />
									<px:PXGridColumn DataField="SortOrder" TextAlign="Right" />
									<px:PXGridColumn DataField="Required" AllowNull="False" TextAlign="Center" Type="CheckBox" />
									<px:PXGridColumn DataField="CSAttribute__IsInternal" AllowNull="True" TextAlign="Center" Type="CheckBox" />
									<px:PXGridColumn DataField="ControlType" AllowNull="False" Type="DropDownList"/>
									<px:PXGridColumn DataField="DefaultValue" AllowNull="True" RenderEditorText="False" />
								</Columns>
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
		</Items>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXTab>
</asp:Content>
