<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" 
    ValidateRequest="false" CodeFile="IN407010.aspx.cs" Inherits="Page_IN407010" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.IN.Turnover.TurnoverEnq" PrimaryView="Filter" PageLoadBehavior="PopulateSavedValues">
        <CallbackCommands>			
            <px:PXDSCallbackCommand Visible="false" Name="SelectItems" CommitChanges="true" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Filter"
        Caption="Selection" DefaultControlID="edOrgBAccountID" MarkRequired="Dynamic">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                <px:PXBranchSelector ID="edOrgBAccountID" runat="server" DataField="OrgBAccountID" CommitChanges="True" InitialExpandLevel="0" />
			    <px:PXSelector ID="edFromPeriod" runat="server" DataField="FromPeriodID" AutoRefresh="true" CommitChanges="True" />
			    <px:PXSelector ID="edToPeriod" runat="server" DataField="ToPeriodID" AutoRefresh="true" CommitChanges="True" />

            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                <px:PXSegmentMask ID="edSiteID" runat="server" DataField="SiteID" CommitChanges="True" AutoRefresh="true" />
                <px:PXSegmentMask ID="edItemClassID" runat="server" DataField="ItemClassID" CommitChanges="True" AutoRefresh="true" />
                <px:PXLayoutRule runat="server" Merge="True" LabelsWidth="SM" ControlSize="XM" />
                    <px:PXSegmentMask ID="edInventoryID" runat="server" DataField="InventoryID" CommitChanges="True" AutoRefresh="true" />
                    <px:PXButton runat="server" ID="btnItemList" Text="List" CommandName="SelectItems" CommandSourceID="ds"/>
                <px:PXLayoutRule runat="server" EndGroup="True" />
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Height="144px" 
        Style="z-index: 100" Width="100%" AdjustPageSize="Auto"
        AllowPaging="True" AllowSearch="True" BatchUpdate="True" Caption="Turnover Calculation Items" SkinID="PrimaryInquire" SyncPosition="True"
        FastFilterFields="InventoryID,Description,SiteID" RepaintColumns="True">
        <ActionBar>
			<Actions>
				<Refresh ToolBarVisible="False" MenuVisible="False"></Refresh>
			</Actions>
		</ActionBar>
        <Levels>
            <px:PXGridLevel DataMember="TurnoverCalcItems">
                <RowTemplate>
                    <px:PXSegmentMask ID="edInventoryID" runat="server" DataField="InventoryID" Enabled="false" AllowEdit="true" />
                </RowTemplate>
                <Columns>
                    <px:PXGridColumn DataField="InventoryID" />
                    <px:PXGridColumn DataField="Description" />
                    <px:PXGridColumn DataField="SiteID" />
                    <px:PXGridColumn DataField="UOM" />

                    <px:PXGridColumn DataField="BegCost" AllowNull="False" TextAlign="Right" />
                    <px:PXGridColumn DataField="BegQty" AllowNull="False" TextAlign="Right" />

                    <px:PXGridColumn DataField="YtdCost" AllowNull="False" TextAlign="Right" />
                    <px:PXGridColumn DataField="YtdQty" AllowNull="False" TextAlign="Right" />

                    <px:PXGridColumn DataField="AvgCost" AllowNull="False" TextAlign="Right" />
                    <px:PXGridColumn DataField="AvgQty" AllowNull="False" TextAlign="Right" />

                    <px:PXGridColumn DataField="SoldCost" AllowNull="False" TextAlign="Right" />
                    <px:PXGridColumn DataField="SoldQty" AllowNull="False" TextAlign="Right" />

                    <px:PXGridColumn DataField="CostRatio" AllowNull="False" TextAlign="Right" NullText="-" />
                    <px:PXGridColumn DataField="QtyRatio" AllowNull="False" TextAlign="Right" NullText="-" />

                    <px:PXGridColumn DataField="CostSellDays" AllowNull="False" TextAlign="Right" NullText="-" />
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
    </px:PXGrid>
    <!--#include file="~\Pages\IN\Includes\InventoryLinkFilter.inc"-->
</asp:Content>
