<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true" ValidateRequest="false"
    CodeFile="SM204007.aspx.cs" Inherits="Pages_SM_SM204007" Title="Action Execution Subscriber" %>
<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" Width="100%" runat="server" PrimaryView="ActionExecutions" TypeName="PX.BusinessProcess.UI.ActionExecutionMaint"  Visible="True">
        <CallbackCommands>
			<px:PXDSCallbackCommand Name="createBusinessEvent" Visible="False" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="viewBusinessEvent" Visible="False" DependOnGrid="grdCreatedByEvents" />
        </CallbackCommands>
	</px:PXDataSource>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="frmTask" runat="server" DataSourceID="ds" DataMember="ActionExecutions" Width="100%" DefaultControlID="edActionExecutionID">
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="L"  />
			<px:PXSelector runat="server" ID="edSubscriberID" DataField="ExecutionID" CommitChanges="True" TextField="Name" NullText="<NEW>" DataSourceID="ds">
                <GridProperties>
                    <Columns>
	                    <px:PXGridColumn DataField="Name" Width="120px" />
                    </Columns>
                </GridProperties>
            </px:PXSelector>
			<px:PXTextEdit runat="server" ID="edName" DataField="Name" AlreadyLocalized="False" DefaultLocale="" />
			<px:PXSelector ID="edActionScreenID" runat="server" DataField="ActionScreenID" DisplayMode="Hint" FilterByAllFields="true" CommitChanges="True"/>
			<px:PXDropDown runat="server" ID="edActionName" DataField="ActionName" AlreadyLocalized="False" DefaultLocale="" />
            <px:PXLayoutRule runat="server" ColumnSpan="2" />
            <px:PXLayoutRule runat="server" LabelsWidth="S" ControlSize="L" StartColumn="True" />
			<px:PXSelector runat="server" ID="edScreenID" DataField="ScreenID" DisplayMode="Hint" FilterByAllFields="true" CommitChanges="True" />
			<px:PXLayoutRule runat="server" LabelsWidth="S" ControlSize="M" />
            <px:PXCheckBox runat="server" ID="chkShowCreatedByEventsTabExpr" DataField="ShowCreatedByEventsTabExpr" SuppressLabel="True" />
		</Template>
	</px:PXFormView>
</asp:Content>

<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXTab ID="tab" runat="server" Height="150px" Style="z-index: 100" Width="100%" DataSourceID="ds" DataMember="CurrentActionExecution">
		<Activity HighlightColor="" SelectedColor="" Width="" Height="" />
		<Items>	
			<px:PXTabItem Text="Keys"  BindingContext="tab" >
				<Template>
					<px:PXGrid ID="grdMappingByActionScreenId" runat="server" DataSourceID="ds"
						 Style="z-index: 100"  AutoRefresh="True" AutoAdjustColumns="True" 
					          AllowPaging="False" SyncPosition="True"
					           Width="100%" Height="100%" AllowSearch="True" SkinID="Details" MatrixMode="true" OnEditorsCreated="grid_EditorsCreated">
						<Levels>
							<px:PXGridLevel DataMember="ActionExecutionMappings">
								<Columns>
									<px:PXGridColumn DataField="DisplayFieldName" Width="175px" />
									<px:PXGridColumn DataField="FromSchema" AllowNull="False" TextAlign="Center" Type="CheckBox" Width="75px" CommitChanges="True" />
									<px:PXGridColumn DataField="Value" Width="200px"  Key="valueMapping" AllowSort="False" MatrixMode="true" AllowStrings="True" DisplayMode="Value"/>
								</Columns>
							</px:PXGridLevel>
						</Levels>
						<Mode AllowAddNew="False" AllowUpdate="True" AllowDelete="False" />
						<AutoSize Enabled="True" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Field Values"  BindingContext="tab" >
				<Template>
					<px:PXGrid ID="grdParameterByActionScreenId" runat="server" DataSourceID="ds" Width="100%" SkinID="Details" MatrixMode="true"
						AutoAdjustColumns="True" AllowPaging="False"  OnEditorsCreated="grid_EditorsCreated" SyncPosition="true" >
						<Mode InitNewRow="True" AllowRowSelect="False" />
						<Levels>
							<px:PXGridLevel DataMember="ActionExecutionParameters"> 
								<Columns>
									<px:PXGridColumn Type="DropDownList" DataField="ObjectName" Width="175px"  CommitChanges="True" />
									<px:PXGridColumn Type="DropDownList" DataField="FieldName" Width="175px"  CommitChanges="True" />
									<px:PXGridColumn DataField="FromSchema" AllowNull="False" TextAlign="Center" Type="CheckBox" Width="75px" CommitChanges="True" />
									<px:PXGridColumn DataField="Value" Width="200px" Key="valueParameter" AllowSort="False" MatrixMode="true" AllowStrings="True" DisplayMode="Value"/>
								</Columns>
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" />
						<CallbackCommands>
                            <InitRow CommitChanges="true" />
                        </CallbackCommands>
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Executed By Events" BindingContext="frmTask" VisibleExp="DataControls[&quot;chkShowCreatedByEventsTabExpr&quot;].Value=1">
				<Template>
                    <px:PXGrid ID="grdCreatedByEvents" runat="server" DataSourceID="ds" Width="100%" SkinID="Details"
						AutoAdjustColumns="True" AllowPaging="False" SyncPosition="True">
						<ActionBar>
                            <CustomItems>
                                <px:PXToolBarButton Text="Create Business Event">
                                   <AutoCallBack Command="createBusinessEvent" Target="ds" />
                                </px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
						<Levels>
							<px:PXGridLevel DataMember="BusinessEvents">
								<RowTemplate>
									<px:PXSelector ID="edEventName" runat="server" DataField="Name" AutoRefresh="True" />
								</RowTemplate>
								<Columns>
									<px:PXGridColumn DataField="Name" LinkCommand="ViewBusinessEvent" />
									<px:PXGridColumn DataField="Description" />
									<px:PXGridColumn DataField="Active" Type="CheckBox" />
									<px:PXGridColumn DataField="Type" Width="200" />
								</Columns>
							</px:PXGridLevel>
						</Levels>
						<Mode AllowUpdate="False" />
						<AutoSize Enabled="True" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
		</Items>
		<AutoSize Container="Window" Enabled="True" MinHeight="250" />
	</px:PXTab>
	<px:PXSmartPanel ID="pnlCreateBusinessEvent" runat="server" Key="NewEventData" Caption="Create Business Event" CaptionVisible="True"
        AutoReload="True" AutoRepaint="True" Width="500">
	    <px:PXFormView ID="frmCreateBusinessEvent" runat="server" DataMember="NewEventData" DataSourceID="ds" Width="100%" SkinID="Transparent">
		    <Template>
			    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="L" />
			    <px:PXTextEdit runat="server" ID="txtName" DataField="Name" CommitChanges="True" />
		    </Template>
	    </px:PXFormView>
		<px:PXPanel ID="pnlButtons" runat="server" SkinID="Buttons">
            <px:PXButton ID="btnOK" runat="server" DialogResult="OK" Text="OK" />
			<px:PXButton ID="btnCancel" runat="server" DialogResult="Cancel" Text="Cancel" />
		</px:PXPanel>
	</px:PXSmartPanel>
</asp:Content>