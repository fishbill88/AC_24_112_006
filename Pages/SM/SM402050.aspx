<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SM402050.aspx.cs" Inherits="Page_SM402050" Title="Business Events" %>
<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
    <px:PXDataSource ID="ds" Runat="server" TypeName="PX.BusinessProcess.UI.BusinessProcessEventInq" PrimaryView="Filter" Width="100%">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="createBusinessEvent" Visible="False" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="viewBusinessEvent" Visible="False" DependOnGrid="grid" />
            <px:PXDSCallbackCommand Name="viewEventFromHistory" Visible="False" DependOnGrid="grid1" />
            <px:PXDSCallbackCommand Name="viewEventSubscriber" Visible="False" DependOnGrid="grid2" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:content ID="cont2" ContentPlaceHolderID="phL" Runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Filter" Style="z-index: 100" Width="100%">
        <Template>
            <px:PXTextEdit ID="chkIsHistoryVisible" Visible="False" runat="server" DataField="IsHistoryVisible" />
        </Template>
    </px:PXFormView>
    <px:PXTab ID="tab" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%">
        <Items>
            <px:PXTabItem Text="Configuration">
                <Template>
                    <px:PXGrid ID="grid" Runat="server" SkinID="Inquire" Height="450" Width="100%" AdjustPageSize="Auto"
                        AutoAdjustColumns="True" AllowPaging="True" AllowSearch="True">
                        <Levels>
                            <px:PXGridLevel DataMember="Events">
                                <Columns>
                                    <px:PXGridColumn DataField="Type" Width="200" />
					                <px:PXGridColumn DataField="Name" Width="450" LinkCommand="ViewBusinessEvent" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <ActionBar>
                            <Actions>
                                <ExportExcel ToolBarVisible="False" MenuVisible="False" />
                            </Actions>
                            <CustomItems>
                                <px:PXToolBarButton Text="Create Business Event">
                                    <AutoCallBack Command="CreateBusinessEvent" Target="ds" />
                                </px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
                        <AutoSize Container="Window" Enabled="True" MinHeight="250" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="History" BindingContext="form" VisibleExp="DataControls[&quot;chkIsHistoryVisible&quot;].Value">
                <Template>
                    <px:PXSplitContainer runat="server" ID="sp1" SkinID="Horizontal" SplitterPosition="400">
                        <AutoSize Enabled="True" Container="Window" />
                        <Template1>
                            <px:PXGrid ID="grid1" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" AllowFilter="True" ActionsPosition="Top"
                                SkinID="PrimaryInquire" SyncPosition="true" AdjustPageSize="Auto" AllowPaging="True" Caption="Business Events" KeepPosition="True"
                                CaptionVisible="true" AutoAdjustColumns="True" PreserveSortsAndFilters="True" PreservePageIndex="True">
                                <Levels>
                                    <px:PXGridLevel DataMember="EventsHistory">
                                        <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="True" AllowRowSelect="True" />
                                        <Columns>
                                            <px:PXGridColumn DataField="LastRunStatus" Width="60" Type="Icon" TextAlign="Center" AllowResize="false" />
                                            <px:PXGridColumn DataField="BPEvent__Name" Width="200" LinkCommand="ViewEventFromHistory" CommitChanges="True" />
                                            <px:PXGridColumn DataField="CreatedDateTime" DisplayFormat="g" Width="150" />
                                            <px:PXGridColumn DataField="LastModifiedDateTime" DisplayFormat="g" Width="150" />
                                            <px:PXGridColumn DataField="ErrorText" Width="300" />
                                        </Columns>
                                    </px:PXGridLevel>
                                </Levels>
                                <AutoSize Enabled="True" Container="Parent" />
                                <AutoCallBack Command="Refresh" Target="grid2" />
                                <ActionBar>
                                    <Actions>
                                        <Refresh ToolBarVisible="Top" />
                                    </Actions>
                                </ActionBar>
                            </px:PXGrid>
                        </Template1>
                        <Template2>
                            <px:PXGrid ID="grid2" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" Height="260px" SkinID="Inquire"
                                AdjustPageSize="Auto" AllowPaging="True" Caption="Subscribers" CaptionVisible="true" SyncPosition="True" AutoAdjustColumns="True">
                                <Levels>
                                    <px:PXGridLevel DataMember="EventSubscribers">
                                        <Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="True" AllowRowSelect="False" />
                                        <Columns>
                                            <px:PXGridColumn DataField="LastRunStatus" Width="60" Type="Icon" TextAlign="Center" AllowResize="false" />
                                            <px:PXGridColumn DataField="HandlerID" TextField="HandlerID_Description" Width="150" LinkCommand="ViewEventSubscriber" CommitChanges="True" />
                                            <px:PXGridColumn DataField="Type" Width="125" AllowUpdate="False" />
                                            <px:PXGridColumn DataField="ErrorText" Width="300" AllowUpdate="False" />
                                        </Columns>
                                    </px:PXGridLevel>
                                </Levels>
                                <AutoSize Enabled="True" Container="Parent" MinHeight="150" />
                            </px:PXGrid>
                        </Template2>
                    </px:PXSplitContainer>
                </Template>
            </px:PXTabItem>
        </Items>
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
</asp:content>
