<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="AP305000.aspx.cs" Inherits="Page_AP305000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource  EnableAttributes="true" ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="Document" TypeName="PX.Objects.CA.CABatchEntry" PageLoadBehavior="GoLastRecord" HeaderDescriptionField="FormCaptionDescription">
		<CallbackCommands>
			<px:PXDSCallbackCommand DependOnGrid="grid" Name="ViewAPDocument" Visible="False" CommitChanges="True" PopupCommand="Cancel" PopupCommandTarget="ds" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="form" runat="server" Style="z-index: 100" Width="100%" Caption="Batch Summary" DataMember="Document" DefaultControlID="edBatchNbr" FilesIndicator="True" NoteIndicator="True" LinkIndicator="True"
		BPEventsIndicator="True">
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="S" />
			<px:PXSelector runat="server" DataField="BatchNbr" ID="edBatchNbr" />
			<px:PXDropDown runat="server" DataField="Status" ID="edStatus" Enabled="False" />
			<px:PXDateTimeEdit CommitChanges="True" runat="server" DataField="TranDate" ID="edTranDate" />
			<px:PXTextEdit runat="server" DataField="ExtRefNbr" ID="edExtRefNbr" />
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
			<px:PXSegmentMask CommitChanges="True" runat="server" DataField="CashAccountID" ID="edCashAccountID" />
			<px:PXSelector CommitChanges="True" runat="server" DataField="PaymentMethodID" ID="edPaymentMethodID" />
			<px:PXSelector runat="server" DataField="ReferenceID" ID="edReferenceID" Enabled="False" />
            <px:PXDateTimeEdit  Size="M" runat="server" DataField="ExportTime" ID="edExportTime" />
			<px:PXLayoutRule runat="server" ColumnSpan="2" />
			<px:PXTextEdit runat="server" DataField="TranDesc" ID="edTranDesc" />
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="S" />
			<px:PXNumberEdit runat="server" Enabled="False" DataField="CuryDetailTotal" ID="edCuryDetailTotal" />
            <px:PXTextEdit runat="server" DataField="BatchSeqNbr" ID="edBatchSeqNbr" />
			<px:PXNumberEdit runat="server" DataField="DateSeqNbr" ID="edDateSeqNbr" Enabled="False" />
			<px:PXTextEdit runat="server" DataField="CountOfPayments" ID="edCountOfPayments" Enabled="false" />
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXGrid ID="grid" runat="server" Height="180px" AllowAutoHide="false" Width="100%" Caption="Payments" SkinID="Details" ActionsPosition="Top" SyncPosition="true" MarkRequired="Dynamic" RepaintColumns="true">
		<Levels>
			<px:PXGridLevel DataMember="BatchPayments">
				<RowTemplate>
					<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
					<px:PXDropDown ID="edAPPayment__DocType" runat="server" DataField="OrigDocType" />
					<px:PXSelector ID="edAPPayment__RefNbr" runat="server" DataField="APPayment__RefNbr" />
					<px:PXSegmentMask ID="edAPPayment__VendorID" runat="server" DataField="APPayment__VendorID" />
					<px:PXSegmentMask ID="edAPPayment__VendorLocationID" runat="server" DataField="APPayment__VendorLocationID" />
					<px:PXSelector ID="edAPPayment__CuryID" runat="server" DataField="APPayment__CuryID" />
					<px:PXTextEdit ID="edAPPayment__DocDesc" runat="server" DataField="APPayment__DocDesc" />
					<px:PXSelector ID="edAPPayment__PaymentMethodID" runat="server" DataField="APPayment__PaymentMethodID" />
					<px:PXTextEdit ID="edAPPayment__ExtRefNbr" runat="server" DataField="APPayment__ExtRefNbr" />
					<px:PXDateTimeEdit ID="edAPPayment__DocDate" runat="server" DataField="APPayment__DocDate" Enabled="False" />
					<px:PXNumberEdit ID="edAPPayment__CuryOrigDocAmt" runat="server" DataField="APPayment__CuryOrigDocAmt" />
					<px:PXDateTimeEdit ID="edAPRegisterAlias__DocDate" runat="server" DataField="APRegisterAlias__DocDate" Enabled="False" />
					<px:PXTextEdit ID="edAddendaPaymentRelatedInfo" runat="server" DataField="AddendaPaymentRelatedInfo" />
				</RowTemplate>
				<Columns>
					<px:PXGridColumn DataField="OrigDocType" Type="DropDownList" />
					<px:PXGridColumn DataField="APPayment__RefNbr" LinkCommand="ViewAPDocument" />
					<px:PXGridColumn DataField="APPayment__VendorID" />
					<px:PXGridColumn DataField="APPayment__VendorLocationID" />
					<px:PXGridColumn DataField="APPayment__DocDate" />
					<px:PXGridColumn DataField="APPayment__Status" />
					<px:PXGridColumn DataField="APPayment__CuryID" />
					<px:PXGridColumn DataField="APPayment__DocDesc" />
					<px:PXGridColumn DataField="APPayment__PaymentMethodID" />
					<px:PXGridColumn DataField="APPayment__ExtRefNbr" />
					<px:PXGridColumn DataField="APPayment__CuryOrigDocAmt" TextAlign="Right" />
					<px:PXGridColumn DataField="APRegisterAlias__DocDate" />
					<px:PXGridColumn DataField="AddendaPaymentRelatedInfo" SyncVisibility="true" />
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<CallbackCommands>
            <Refresh RepaintControlsIDs="form" CommitChanges="true" />
        </CallbackCommands>
		<ActionBar DefaultAction="ViewAPDocument">
			<Actions>
				<AddNew Enabled="False" />
			</Actions>
			<CustomItems>
				<px:PXToolBarButton Text="Add Payments" CommandSourceID="ds" CommandName="AddPayments">
					<AutoCallBack Command="AddPayments" Target="ds" />
				</px:PXToolBarButton>
			</CustomItems>
		</ActionBar>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXGrid>

	<px:PXGrid ID="gridAddendaInfo" runat="server" Caption="Payments" SkinID="Details" ActionsPosition="Top">
		<Levels>
			<px:PXGridLevel DataMember="AddendaInfo">
				<Columns>
					<px:PXGridColumn DataField="RefNbr" />
					<px:PXGridColumn DataField="DocDesc" />
					<px:PXGridColumn DataField="DocDate" />
					<px:PXGridColumn DataField="CuryOrigDocAmt" />
					<px:PXGridColumn DataField="ExtRefNbr" />
					<px:PXGridColumn DataField="APInvoice__RefNbr" />
					<px:PXGridColumn DataField="APInvoice__InvoiceNbr" />
					<px:PXGridColumn DataField="APInvoice__DocDesc" />
					<px:PXGridColumn DataField="APInvoice__DocDate" />
					<px:PXGridColumn DataField="APAdjust__CuryAdjgAmt" />
					<px:PXGridColumn DataField="Vendor__AcctName" />
				</Columns>
			</px:PXGridLevel>
		</Levels>
	</px:PXGrid>

	<px:PXSmartPanel ID="PanelAddPayment" runat="server" Key="AvailablePayments" Caption="Add Payments" CaptionVisible="True" LoadOnDemand="True" AutoCallBack-Command="Refresh" 
		AutoCallBack-Target="frmPaymentFilter1" Width = "1100px" Height = "500px" DesignView="Content">
		<px:PXFormView ID="frmPaymentFilter1" runat="server" Caption="Payment Selection" CaptionVisible="False" DataMember="filter" DataSourceID="ds" SkinID="Transparent" TabIndex="2900">
			<Template>
				<px:PXLayoutRule ID="PXLayoutRule111" runat="server" ControlSize="XM" LabelsWidth="SM" StartColumn="True" />
				<px:PXTextEdit ID="edNextPaymentRefNumber" runat="server" CommitChanges="True" DataField="NextPaymentRefNumber" />
				<px:PXDateTimeEdit ID="edStartDate" runat="server" CommitChanges="True" DataField="StartDate" />
				<px:PXDateTimeEdit ID="edEndDate" runat="server" CommitChanges="True" DataField="EndDate" />
			</Template>
		</px:PXFormView>
		<px:PXGrid ID="gridOL1" runat="server" AllowPaging="True" DataSourceID="ds" SkinID="Details" TabIndex="3100" Width="100%">
			<Levels>
				<px:PXGridLevel DataKeyNames="DocType,RefNbr" DataMember="AvailablePayments">
					<Columns>
						<px:PXGridColumn AllowCheckAll="True" AllowNull="False" DataField="Selected" TextAlign="Center" Type="CheckBox" CommitChanges="true"/>
						<px:PXGridColumn DataField="DocType" MatrixMode="True" />
						<px:PXGridColumn DataField="RefNbr" />
						<px:PXGridColumn DataField="VendorID" />
						<px:PXGridColumn DataField="VendorID_BAccountR_acctName" />
						<px:PXGridColumn DataField="VendorLocationID" />
						<px:PXGridColumn DataField="ExtRefNbr" />
						<px:PXGridColumn AllowUpdate="False" DataField="DocDate" />
						<px:PXGridColumn AllowUpdate="False" DataField="DepositAfter" />
						<px:PXGridColumn DataField="CuryID" />
						<px:PXGridColumn AllowNull="False" DataField="CuryOrigDocAmt" TextAlign="Right" />
						<px:PXGridColumn DataField="CashAccountID" />
						<px:PXGridColumn DataField="CashAccountID_CashAccount_Descr" />
						<px:PXGridColumn DataField="PaymentMethodID" TextAlign="Left" />
					</Columns>
					<Layout FormViewHeight="" />
				</px:PXGridLevel>
			</Levels>
			<ActionBar>
				<Actions>
					<AddNew Enabled="false" ToolBarVisible="False" />
					<Delete Enabled="false" ToolBarVisible="False" />
				</Actions>
			</ActionBar>
			<AutoSize Enabled="True" MinHeight="300" />
		</px:PXGrid>
		<px:PXPanel ID="PXPanel111" runat="server" SkinID="Buttons">
			<px:PXButton ID="OK1" runat="server" DialogResult="OK" Text="Add & Close" />
			<px:PXButton ID="Cancel1" runat="server" DialogResult="No" Text="Cancel" />
		</px:PXPanel>
	</px:PXSmartPanel>

	<px:PXSmartPanel ID="PXSmartPanel1" runat="server" Key="VoidFilter" Caption="Void Batch Payments" CaptionVisible="True" LoadOnDemand="True" AutoCallBack-Command="Refresh" 
		AutoCallBack-Target="frmPaymentFilter1" Width = "200px" Height = "180px" DesignView="Content">
		<px:PXFormView ID="PXFormView1" runat="server" Caption="Void Batch Payments" CaptionVisible="False" DataMember="voidFilter" DataSourceID="ds" SkinID="Transparent" TabIndex="2900">
			<Template>
				<px:PXLayoutRule runat="server" StartColumn="true" ColumnWidth="L" />
				<px:PXGroupBox ID="gbVoidDateOption" runat="server" Caption="Void Payments On:" DataField="VoidDateOption"
					RenderStyle="Fieldset" CommitChanges="true">
					<Template>
						<px:PXRadioButton ID="gbVoidDateOption_opO" runat="server" Text="Original Payment Dates"
							Value="O" GroupName="gbVoidDateOption" />
						<px:PXRadioButton ID="gbVoidDateOption_opS" runat="server" Text="Specific Date"
							Value="S" GroupName="gbVoidDateOption" />
					</Template>
					<ContentLayout Layout="Stack" />
				</px:PXGroupBox>
				<px:PXDateTimeEdit ID="edVoidDate" runat="server" CommitChanges="True" DataField="VoidDate" />
			</Template>
		</px:PXFormView>
		<px:PXPanel ID="PXPanel1" runat="server" SkinID="Buttons">
			<px:PXButton ID="PXButton1" runat="server" DialogResult="OK" Text="Void" />
			<px:PXButton ID="PXButton2" runat="server" DialogResult="No" Text="Cancel" />
		</px:PXPanel>
	</px:PXSmartPanel>
</asp:Content>
