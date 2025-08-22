<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SO402020.aspx.cs"
    Inherits="Page_SO402020" Title="Efficiency of Picking and Packing" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PageLoadBehavior="PopulateSavedValues" PrimaryView="Filter" TypeName="PX.Objects.SO.SOPickingEfficiencyEnq"/>
 </asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds"  DataMember="Filter" Width="100%" CaptionAlign="Justify" DefaultControlID="edStartDate">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
            <px:PXSelector CommitChanges="True" ID="edSiteID" runat="server" DataField="SiteID" />
            <px:PXDateTimeEdit ID="edStartDate" runat="server" CommitChanges="True" DataField="StartDate" DisplayFormat="d"/>
            <px:PXDateTimeEdit ID="edEndDate" runat="server" CommitChanges="True" DataField="EndDate" DisplayFormat="d"/>
            <px:PXCheckBox CommitChanges="True" ID="chkByDay" runat="server" DataField="ExpandByDay" />

            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
            <px:PXCheckBox CommitChanges="True" ID="chkByUser" runat="server" DataField="ExpandByUser" AlignLeft="True"/>
            <px:PXSelector CommitChanges="True" ID="edUser" runat="server" DataField="UserID" />

            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
            <px:PXCheckBox CommitChanges="True" ID="chkByShipment" runat="server" DataField="ExpandByShipment" AlignLeft="True" />
            <px:PXSelector CommitChanges="True" ID="edStandaloneShipment" runat="server" DataField="StandaloneShipmentNbr" AutoRefresh="True" />
            <px:PXGroupBox ID="gbDocType" runat="server" DataField="DocType" CommitChanges="True" RenderStyle="Simple">
                <Template>
                    <px:PXLayoutRule runat="server" StartGroup="False" StartColumn="True" SuppressLabel="False" />
                    <px:PXRadioButton ID="gbDocType_Shipment" runat="server" Value="SHPT" GroupName="gbDocType" />
                    <px:PXRadioButton ID="gbDocType_PickList" runat="server" Value="PLST" GroupName="gbDocType" />

                    <px:PXLayoutRule runat="server" StartGroup="False" StartColumn="True" SuppressLabel="True" />
                    <px:PXSelector ID="gbShipment" runat="server" DataField="ShipmentNbr" CommitChanges="True" AutoRefresh="True" />
                    <px:PXSelector ID="gbWorksheet" runat="server" DataField="WorksheetNbr" CommitChanges="True" AutoRefresh="True" />
                </Template>
                <ContentLayout Layout="Stack" />
            </px:PXGroupBox>
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Height="144px" Width="100%" AdjustPageSize="Auto"
        AllowPaging="True" AllowSearch="True" Caption="Efficiency" SkinID="PrimaryInquire" RestrictFields="True"
        SyncPosition="true">
        <Levels>
            <px:PXGridLevel DataMember="Efficiency">
                <Columns>
                    <px:PXGridColumn DataField="Day" AllowShowHide="Server" />
                    <px:PXGridColumn DataField="StartDate" DisplayFormat="g" AllowShowHide="Server" />
                    <px:PXGridColumn DataField="EndDate" DisplayFormat="g" AllowShowHide="Server" />
                    <px:PXGridColumn DataField="TotalTime" AllowShowHide="Server" />
                    <px:PXGridColumn DataField="JobType" AllowShowHide="Server" />
                    <px:PXGridColumn DataField="UserID" AllowShowHide="Server" />
                    <px:PXGridColumn DataField="SiteID" AllowShowHide="Server" />
                    <px:PXGridColumn DataField="PickListNbr" LinkCommand="ViewDocument" AllowShowHide="Server" />
                    <px:PXGridColumn DataField="NumberOfShipments" AllowShowHide="Server" />
                    <px:PXGridColumn DataField="NumberOfLines" AllowShowHide="Server" />
                    <px:PXGridColumn DataField="NumberOfPackages" AllowShowHide="Server" />
                    <px:PXGridColumn DataField="NumberOfInventories" AllowShowHide="Server" />
                    <px:PXGridColumn DataField="NumberOfLocations" AllowShowHide="Server" />
                    <px:PXGridColumn DataField="TotalQty" AllowShowHide="Server" />
                    <px:PXGridColumn DataField="NumberOfUsefulOperations" AllowShowHide="Server" />
                    <px:PXGridColumn DataField="NumberOfScans" AllowShowHide="Server" />
                    <px:PXGridColumn DataField="NumberOfFailedScans" AllowShowHide="Server" />
                    <px:PXGridColumn DataField="EffectiveTime" AllowShowHide="Server" />
                    <px:PXGridColumn DataField="Efficiency" AllowShowHide="Server" />
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
    </px:PXGrid>
</asp:Content>