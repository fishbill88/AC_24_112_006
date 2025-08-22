<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true"
    ValidateRequest="false" CodeFile="SM503000.aspx.cs" Inherits="Page_SM503000"
    Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="Filter"
        TypeName="PX.Data.Maintenance.TenantShapshotDeletion.TenantSnapshotDeletionProcess" PageLoadBehavior="InsertRecord">
        <CallbackCommands>
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" Width="100%" Caption="Filter" DataMember="Filter">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="true" LabelsWidth="S" ControlSize="M" />
            <px:PXDropDown runat="server" CommitChanges="True" ID="edAction" DataField="Name" />
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid ID="grid" runat="server" Width="100%" Caption="Records to Delete" CaptionVisible="false" AllowPaging="true"
        SkinID="PrimaryInquire" AdjustPageSize="Auto" AutoAdjustColumns="true" SyncPosition="true" NoteIndicator="true">
        <Levels>
            <px:PXGridLevel DataMember="Records">
                <Columns>
                    <px:PXGridColumn DataField="Selected" TextAlign="Center" Type="CheckBox" AllowCheckAll="true" AllowMove="false" AllowSort="true" Width="15px" />
                    <px:PXGridColumn DataField="TenantId" Width="50px" />
                    <px:PXGridColumn DataField="TenantName" Width="150px" LinkCommand="NavigateToTenant" />
                    <px:PXGridColumn DataField="SnapshotName" Width="150px" />
                    <px:PXGridColumn DataField="Description" Width="150px" />
                    <px:PXGridColumn DataField="Visibility" Width="100px" LinkCommand="NavigateToTenant" />
                    <px:PXGridColumn DataField="SizeMB" Width="100px" />
                    <px:PXGridColumn DataField="CreatedOn" Width="100px" DisplayFormat="g" />
                    <px:PXGridColumn DataField="Version" Width="100px" />
                    <px:PXGridColumn DataField="ExportMode" Width="100px" />
                    <px:PXGridColumn DataField="Status" Width="100px" />
                    <px:PXGridColumn DataField="DeletionStatus" Width="100px" />
                    <px:PXGridColumn DataField="DeletionProgress" Width="100px" />
                    <px:PXGridColumn DataField="DeletionHeartbeat" Width="100px" />
                    <px:PXGridColumn DataField="SnapshotId" Width="100px" />
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="true" MinHeight="400" />
        <ActionBar>
            <Actions>
                <ExportExcel Enabled="false" />
                <AdjustColumns Enabled="false" />
            </Actions>
        </ActionBar>
    </px:PXGrid>
</asp:Content>
