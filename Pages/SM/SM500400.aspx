<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SM500400.aspx.cs" Inherits="Page_SM500400" Title="Archive Documents" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="contDataSource" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Data.Archiving.ArchiveProcess" PrimaryView="Header" PageLoadBehavior="PopulateSavedValues"/>
</asp:Content>
<asp:Content ID="contForm" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Width="100%" DataMember="Header" >
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="XM" ControlSize="XXS" />
            <px:PXNumberEdit CommitChanges="True" ID="edDuration" runat="server" DataField="ArchivingProcessDurationLimitInHours" />
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="contGrid" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Height="400px" Width="100%"
        AllowFilter="False" AllowPaging="True" AllowSearch="False"
        AdjustPageSize="Auto" SkinID="PrimaryInquire" AutoGenerateColumns="AppendDynamic"
        MatrixMode="True" RepaintColumns="True" BatchUpdate="True" SyncPosition="true"
        OnColumnsGenerated="grid_ColumnsGenerated">
        <Levels>
            <px:PXGridLevel DataMember="DatesToArchive">
                <Columns>
                    <px:PXGridColumn DataField="Selected" TextAlign="Center" Type="CheckBox" CommitChanges="True" />
                    <px:PXGridColumn DataField="Date" TextAlign="Right"/>
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <ActionBar>
            <Actions>
                <ExportExcel ToolBarVisible="False"/>
            </Actions>
        </ActionBar>
        <AutoSize Container="Window" Enabled="True" MinHeight="200" />
        <Mode AllowSort="False" />
    </px:PXGrid>
</asp:Content>
