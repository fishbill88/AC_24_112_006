<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SM400400.aspx.cs" Inherits="Page_SM400400" Title="Document Archival History" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="contDataSource" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Data.Archiving.ArchivationHistoryEnq" PrimaryView="Header" PageLoadBehavior="PopulateSavedValues"/>
</asp:Content>
<asp:Content ID="contForm" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Width="100%" DataMember="Header" >
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="L" ControlSize="S" />
            <px:PXFormView ID="formExecutionDate" runat="server" DataSourceID="ds" DataMember="Header" RenderStyle="Fieldset" Caption="Execution Date">
                <Template>
                    <px:PXDateTimeEdit runat="server" ID="edStartDate" DataField="StartDate" CommitChanges="True" />
                    <px:PXDateTimeEdit runat="server" ID="edEndDate" DataField="EndDate" CommitChanges="True" />
                </Template>
            </px:PXFormView>
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="contGrid" ContentPlaceHolderID="phG" runat="Server">
    <px:PXSplitContainer runat="server" ID="sp1" CollapseDirection="Panel2" Orientation="Vertical" PositionInPercent="true" SplitterPosition="33">
        <AutoSize Enabled="true" Container="Window" />
        <Template1>
            <px:PXGrid ID="grid1" runat="server" DataSourceID="ds" Height="400px" Width="100%" SkinID="Inquire"
                AllowPaging="True" AdjustPageSize="Auto" SyncPosition="True" AutoAdjustColumns="True" Caption="Executions" CaptionVisible="true">
                <Levels>
                    <px:PXGridLevel DataMember="ArchivingExecutions">
                        <Columns>
                            <px:PXGridColumn DataField="ExecutionDate" TextAlign="Right" DisplayFormat="g" />
                            <px:PXGridColumn DataField="ExecutionTime" TextAlign="Right" Width="50px" />
                            <px:PXGridColumn DataField="CreatedByID" TextAlign="Right" Width="50px" />
                            <px:PXGridColumn DataField="ArchivedRowsCount" TextAlign="Right" AllowFilter="false"/>
                        </Columns>
                    </px:PXGridLevel>
                </Levels>
                <Mode AllowAddNew="false" AllowUpdate="false" AllowDelete="false"/>
                <AutoSize Container="Window" Enabled="True" MinHeight="200" />
                <AutoCallBack Target="grid2" Command="Refresh" />
            </px:PXGrid>
        </Template1>
        <Template2>
            <px:PXGrid ID="grid2" runat="server" DataSourceID="ds" Height="400px" Width="100%" SkinID="Inquire"
                AllowPaging="True" AdjustPageSize="Auto" AutoGenerateColumns="AppendDynamic" AutoAdjustColumns="True" SyncPosition="True" Caption="Execution Details" CaptionVisible="true">
                <Levels>
                    <px:PXGridLevel DataMember="ArchivedDates">
                        <Columns>
                            <px:PXGridColumn DataField="DateToArchive" TextAlign="Right"/>
                            <px:PXGridColumn DataField="ExecutionTime" TextAlign="Right"/>
                            <px:PXGridColumn DataField="ArchivedRowsCount" TextAlign="Right" AllowFilter="false"/>
                        </Columns>
                    </px:PXGridLevel>
                </Levels>
                <Mode AllowAddNew="false" AllowUpdate="false" AllowDelete="false"/>
                <AutoSize Container="Window" Enabled="True" MinHeight="200" />
            </px:PXGrid>
        </Template2>
    </px:PXSplitContainer>
</asp:Content>
