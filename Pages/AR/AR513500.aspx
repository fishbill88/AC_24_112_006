<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="AR513500.aspx.cs" Inherits="Page_AR513500" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" Width="100%" runat="server" Visible="True" PrimaryView="Filter" PageLoadBehavior="PopulateSavedValues" 
		TypeName="PX.Objects.CC.PayLinkProcessing">
		<CallbackCommands>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" DataMember="Filter" TabIndex="100">
		<Template>
			<px:PXLayoutRule runat="server" StartRow="True" StartColumn="True"/>
		    <px:PXDropDown ID="cmbPendingOperation" runat="server" DataField="Action" CommitChanges="True"></px:PXDropDown>
			<px:PXSelector ID="selCustomer" runat="server" DataField="CustomerID" CommitChanges="True"></px:PXSelector>
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXGrid ID="grid" runat="server" Height="400px" Width="100%" AllowPaging="true" AdjustPageSize="Auto" 
		AllowSearch="true" DataSourceID="ds" SkinID="PrimaryInquire" SyncPosition="true"  Caption="Process Payment Links">
		<Levels>
			<px:PXGridLevel DataMember="DocumentList">
				<Columns>
					<px:PXGridColumn DataField="Selected" TextAlign="Center" Type="CheckBox" AllowCheckAll="true"  />
					<px:PXGridColumn DataField="BranchID" />
					<px:PXGridColumn DataField="DocType" Width="100px" />
					<px:PXGridColumn DataField="RefNbr" LinkCommand="ViewDocument" Width="140px" />
					<px:PXGridColumn DataField="CustomerID" LinkCommand="ViewCust" Width="140px" />
					<px:PXGridColumn DataField="AcctName"  width="200px" />
					<px:PXGridColumn DataField="CustomerClassID"  />
					<px:PXGridColumn DataField="DocDate"  />
					<px:PXGridColumn DataField="DueDate"  />
					<px:PXGridColumn DataField="CuryOrigDocAmt"  />
					<px:PXGridColumn DataField="CuryDocBal"  />
					<px:PXGridColumn DataField="PayLinkAmt"  />
					<px:PXGridColumn DataField="CuryID"  />
					<px:PXGridColumn DataField="ProcessingCenterID"  Width="100px"  />
					<px:PXGridColumn DataField="StatusDate"  />
					<px:PXGridColumn Type="CheckBox" DataField="NeedSync" />
					<px:PXGridColumn DataField="ExternalID"  />
					<px:PXGridColumn DataField="ErrorMessage"  />
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="400" />
		<Layout ShowRowStatus="False" />
	</px:PXGrid>
</asp:Content>
