<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="CA508000.aspx.cs" Inherits="Page_CA508000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="server">
    <px:PXDataSource ID="ds" runat="server" Visible="true" Width="100%" PrimaryView="Filter" TypeName="PX.Objects.CC.UpdateLevel3Data" PageLoadBehavior="PopulateSavedValues">
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" DataMember="Filter" TabIndex="100">
		<Template>
			<px:PXLayoutRule runat="server" StartRow="True" StartColumn="True"/>
            <px:PXSelector ID="edProcessingCenterID" runat="server" DataField="ProcessingCenterID" CommitChanges="True">
            </px:PXSelector>
            <px:PXDropDown ID="edProcessingStatus" runat="server" DataField="ProcessingStatus" CommitChanges="true">
            </px:PXDropDown>
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="l3Payments" ContentPlaceHolderID="phG" runat="server">
        <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" Height="150px" SkinID="PrimaryInquire" 
        AllowPaging="true" AdjustPageSize="Auto" FilesIndicator="false" NoteIndicator="false" SyncPosition="true">
		<Levels>
			<px:PXGridLevel DataMember="L3Payments">
                <Columns>
                    <px:PXGridColumn DataField="Selected" Label="Selected" TextAlign="Center" Type="CheckBox" AutoCallBack="true" AllowCheckAll="true" />
                    <px:PXGridColumn DataField="DocType" Width="100px"></px:PXGridColumn>
                    <px:PXGridColumn DataField="RefNbr" LinkCommand="ViewDocument" Width="140px"></px:PXGridColumn>
                    <px:PXGridColumn DataField="DocDate" />
                    <px:PXGridColumn DataField="FinPeriodID" />
                    <px:PXGridColumn DataField="CustomerID" DisplayFormat="&gt;AAAAAAAAAA"></px:PXGridColumn>
                    <px:PXGridColumn DataField="Customer__AcctName" />
                    <px:PXGridColumn DataField="Status" />
                    <px:PXGridColumn DataField="CuryOrigDocAmt" />
                    <px:PXGridColumn DataField="CuryID" />
                    <px:PXGridColumn DataField="ProcessingCenterID" />
                    <px:PXGridColumn DataField="PaymentMethodID" />
                    <px:PXGridColumn DataField="ExternalTransaction__L3Status" />
                    <px:PXGridColumn DataField="ExternalTransaction__L3Error" />
                </Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXGrid>
</asp:Content>
