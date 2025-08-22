<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="CA205500.aspx.cs" Inherits="Page_CA205500" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.CA.CABankFeedMaint" PrimaryView="BankFeed" HeaderDescriptionField="Descr">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="ConnectFeed" CommitChanges="true" Visible="true" />
			<px:PXDSCallbackCommand Name="UpdateFeed" CommitChanges="true" Visible="true" />
			<px:PXDSCallbackCommand Name="ActivateFeed" CommitChanges="true" Visible="true" />
			<px:PXDSCallbackCommand Name="MigrateFeed" CommitChanges="true" Visible="true" />
			<px:PXDSCallbackCommand Name="SyncConnectFeed" CommitChanges="true" Visible="False" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="BankFeed" Width="100%" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="layoutRule1" runat="server" StartRow="True" />
			<px:PXLayoutRule runat="server" ID="layoutRule32" StartColumn="True" />
			<px:PXSelector runat="server" ID="selBankFeedID" DataField="BankFeedID" />
			<px:PXDropDown ID="edStatus" runat="server" DataField="Status" Enabled="False" />
			<px:PXDropDown CommitChanges="True" runat="server" ID="cmbBankFeedType" DataField="Type" />
			<px:PXDateTimeEdit runat="server" ID="dtImportStartDate" DataField="ImportStartDate" />
			<px:PXLayoutRule runat="server" ColumnSpan="2" />
			<px:PXTextEdit ID="edDescr" runat="server" DataField="Descr" />
			<px:PXLayoutRule runat="server" ID="layoutRule31" StartColumn="True" />
			<px:PXTextEdit runat="server" ID="txtInst" DataField="Institution" />
			<px:PXCheckBox runat="server" ID="createExpenseReceipts" CommitChanges="True" DataField="CreateExpenseReceipt" />
			<px:PXCheckBox runat="server" ID="chckCreateForPending" DataField="CreateReceiptForPendingTran" />
			<px:PXCheckBox runat="server" ID="chckMultipleMapping" CommitChanges="True" DataField="MultipleMapping" />
		</Template>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" />
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXTab LoadOnDemand="True" DataSourceID="ds" Height="100%" SelectedIndex="1" Width="100%" DataMember="CurrentBankFeed" runat="server" ID="tabs">
		<Items>
			<px:PXTabItem RepaintOnDemand="False" Text="Cash Accounts">
				<Template>
					<px:PXGrid DataSourceID="ds" SkinID="DetailsInTab" SyncPosition="true" Width="100%" Height="350px" runat="server" ID="gridAccounts">
						<ActionBar>
							<Actions>
								<AddNew Enabled="False" />
								<Delete Enabled="False" />
								<FilterShow Enabled="true" />
								<FilterSet Enabled="true" MenuVisible="true" />
							</Actions>
						</ActionBar>
						<Levels>
							<px:PXGridLevel DataMember="BankFeedDetail">								
								<Columns>
									<px:PXGridColumn DataField="Hidden" Width="30px" Type="CheckBox" />
									<px:PXGridColumn DataField="AccountName" Width="280px" />
									<px:PXGridColumn DataField="AccountMask" Width="160px" />
									<px:PXGridColumn DataField="Descr" Width="220px" />
									<px:PXGridColumn DataField="CashAccountID" Width="120px" CommitChanges="true" />
									<px:PXGridColumn DataField="StatementPeriod" CommitChanges="true" Type="DropDownList" />
									<px:PXGridColumn DataField="StatementStartDay" MatrixMode="true" Width="90px" />
									<px:PXGridColumn DataField="ImportStartDate" Width="100px" />
									<px:PXGridColumn DataField="Currency" Width="70px" />
									<px:PXGridColumn DataField="AccountType" Width="220px" />
									<px:PXGridColumn DataField="AccountSubType" Width="220px" />
									<px:PXGridColumn DataField="RetrievalStatus" Width="120px" />
									<px:PXGridColumn DataField="RetrievalDate" Width="120px" />
									<px:PXGridColumn DataField="ErrorMessage" />
									<px:PXGridColumn DataField="AccountID" Width="280px" />
								</Columns>
								<RowTemplate>
									<px:PXDropDown ID="dtStatementStartDay" runat="server" DataField="StatementStartDay" AutoRefresh="true" />
								</RowTemplate>
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem BindingContext="form" Text="Corporate Cards" VisibleExp="DataControls[&quot;createExpenseReceipts&quot;].Value == true">
				<Template>
					<px:PXGrid SyncPosition="true" MatrixMode="True" SkinID="DetailsInTab" Width="100%" Height="350px" runat="server" ID="gridCorpCC">
						<Levels>
							<px:PXGridLevel DataMember="BankFeedCorpCC">
								<Columns>
									<px:PXGridColumn DataField="AccountID" CommitChanges="True" Width="220px" />
									<px:PXGridColumn DataField="CashAccountID" Width="100px" CommitChanges="true" />
									<px:PXGridColumn CommitChanges="True" MatrixMode="True" DataField="MatchField" Width="180px" />
									<px:PXGridColumn CommitChanges="True" MatrixMode="True" DataField="MatchRule" Width="70px" />
									<px:PXGridColumn CommitChanges="True" MatrixMode="True" DataField="MatchValue" Width="70px" />
									<px:PXGridColumn CommitChanges="True" DataField="CorpCardID" Width="140px" />
									<px:PXGridColumn DataField="CardNumber" Width="140px" />
									<px:PXGridColumn DataField="CardName" Width="250px" />
									<px:PXGridColumn DataField="EmployeeID" Width="140px" />
									<px:PXGridColumn DataField="EmployeeName" Width="250px" />
								</Columns>
								<RowTemplate>
									<px:PXSelector ID="selCorpCard" runat="server" DataField="CorpCardID" AutoRefresh="true" />
									<px:PXSelector ID="selAccount" runat="server" DataField="AccountID" TextField="AccountName" AutoRefresh="true" />
								</RowTemplate>
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem BindingContext="form" Text="Expense Items" VisibleExp="DataControls[&quot;createExpenseReceipts&quot;].Value == true">
				<Template>
					<px:PXPanel ID="pnl3" runat="server" ContentLayout-OuterSpacing="Around" RenderSimple="True" RenderStyle="Simple">
						<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
						<px:PXSelector Size="M" runat="server" ID="defaultExpenseItemID" DataField="DefaultExpenseItemID" />
					</px:PXPanel>
					<px:PXGrid MatrixMode="True" runat="server" ID="gridExpense" Height="350px" SkinID="DetailsInTab" Width="100%">
						<Levels>
							<px:PXGridLevel DataMember="BankFeedExpense">
								<Columns>
									<px:PXGridColumn MatrixMode="True" DataField="MatchField" Width="180px" />
									<px:PXGridColumn CommitChanges="True" DataField="MatchRule" Width="70px" />
									<px:PXGridColumn DataField="MatchValue" Width="70px" />
									<px:PXGridColumn DataField="InventoryItemID" Width="70px" />
									<px:PXGridColumn Type="CheckBox" DataField="DoNotCreate" Width="60px" />
								</Columns>
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" />
						<ActionBar>
							<CustomItems>
								<px:PXToolBarButton Text="View Categories">
									<AutoCallBack Command="showCategories" Target="ds" />
								</px:PXToolBarButton>
							</CustomItems>
						</ActionBar>
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem BindingContext="form" Text="Custom Mapping Rules">
				<Template>
					<px:PXGrid DataSourceID="ds" SkinID="DetailsInTab" Width="100%" Height="350px" runat="server" ID="gridFieldMapping">
						<Mode AllowColMoving="false" InitNewRow="True" AllowUpload="False" AllowDragRows="True" />
						<Levels>
							<px:PXGridLevel DataMember="BankFeedFieldMapping">
								<Mode AllowColMoving="false" InitNewRow="True" AllowUpload="True" AllowDragRows="True" />
								<RowTemplate>
									<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM"></px:PXLayoutRule>
									<px:PXCheckBox runat="server" DataField="Active"></px:PXCheckBox>
									<px:PXDropDown runat="server" DataField="TargetField"></px:PXDropDown>
									<pxa:PXFormulaCombo ID="edImportSourceField" runat="server" DataField="SourceFieldOrValue" EditButton="True" IsInternalVisible="false" 
										SelectButton="False" FieldsAutoRefresh="True" PanelAutoRefresh="True"
										FieldsRootAutoRefresh="true" LastNodeName="Fields" ExternalNodeName="Bank Feed Transaction"
										OnRootFieldsNeeded="edImportSourceField_ExternalFieldsNeeded"
										OnExternalFieldsNeeded="edImportSourceField_ExternalFieldsNeeded">
									</pxa:PXFormulaCombo>
								</RowTemplate>
								<Columns>
									<px:PXGridColumn CommitChanges="True" DataField="Active" Type="CheckBox" TextAlign="Center" Width="60px" AllowNull="False"></px:PXGridColumn>
									<px:PXGridColumn CommitChanges="True" DataField="TargetField" AllowDragDrop="true" Width="150px"></px:PXGridColumn>
									<px:PXGridColumn CommitChanges="True" DataField="SourceFieldOrValue" AllowDragDrop="true" Width="150px"></px:PXGridColumn>
								</Columns>
							</px:PXGridLevel>
						</Levels>
						<ActionBar>
							<CustomItems>
								<px:PXToolBarButton CommandName="SetDefaultMapping" CommandSourceID="ds">
									<ActionBar Order="4" />
								</px:PXToolBarButton>
							</CustomItems>
						</ActionBar>
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
		</Items>
		<AutoSize Container="Window" Enabled="True" MinHeight="250" MinWidth="300" />
	</px:PXTab>
	<px:PXSmartPanel AutoReload="True" AutoRepaint="True" Caption="Categories" Key="BankFeedCategories" CaptionVisible="True" runat="server" ID="spCategories">
		<px:PXFormView ID="frmMyCommand" runat="server" SkinID="Transparent" DataMember="BankFeedCategories" DataSourceID="ds" EmailingGraph="">
			<Template>
				<px:PXGrid RepaintColumns="True" AllowPaging="False" MatrixMode="True" SyncPosition="True" Height="350px" Width="500px" SkinID="Inquire" runat="server" ID="gridCategories">
					<Levels>
						<px:PXGridLevel DataMember="BankFeedCategories">
							<Columns>
								<px:PXGridColumn DataField="Category" Width="220" />
							</Columns>
						</px:PXGridLevel>
					</Levels>
				</px:PXGrid>
			</Template>
		</px:PXFormView>
	</px:PXSmartPanel>
	<px:PXSmartPanel AutoRepaint="True" AutoReload="True" Caption="Load Transactions in Test Mode" Key="Filter" CaptionVisible="True" Width="1200px" Height="550px" runat="server" ID="PXSmartPanel2">
		<px:PXFormView ID="frmLoadTransactions" runat="server" SkinID="Transparent" DataMember="Filter" Width="100%" DataSourceID="ds">
			<Template>
				<px:PXLayoutRule runat="server" ID="layoutRule3" StartColumn="True" />
				<px:PXDateTimeEdit runat="server" ID="dtDate" DataField="Date" />
				<px:PXDateTimeEdit runat="server" ID="dtDate1" DataField="DateTo" />
				<px:PXNumberEdit ID="maxTrans" runat="server" DataField="MaxTransactions" CommitChanges="true" />
				<px:PXSelector AutoRefresh="true" CommitChanges="true" Width="250px" runat="server" ID="selLineNbr" DataField="LineNbr" />
			</Template>
		</px:PXFormView>
		<px:PXGrid RepaintColumns="True" AllowPaging="True" AdjustPageSize="Auto" Width="100%" Height="100%" SkinID="Inquire" runat="server" ID="gridTrans">
			<Levels>
				<px:PXGridLevel DataMember="BankFeedTransactions">
					<Columns>
						<px:PXGridColumn DataField="TransactionID" Width="300px" />
						<px:PXGridColumn DataField="Date" />
						<px:PXGridColumn DataField="Amount" Width="70px" />
						<px:PXGridColumn DataField="IsoCurrencyCode" />
						<px:PXGridColumn DataField="Name" Width="250px" />
						<px:PXGridColumn DataField="Category" Width="250px" />
						<px:PXGridColumn DataField="Pending" Type="CheckBox" />
						<px:PXGridColumn DataField="PendingTransactionID" />
						<px:PXGridColumn DataField="Type" Width="100px" />
						<px:PXGridColumn DataField="AccountID" />
						<px:PXGridColumn DataField="AccountOwner" />
						<px:PXGridColumn DataField="CheckNumber" />
						<px:PXGridColumn DataField="Memo" />
						<px:PXGridColumn DataField="CreatedAt" />
						<px:PXGridColumn DataField="PostedAt" />
						<px:PXGridColumn DataField="TransactedAt" />
						<px:PXGridColumn DataField="UpdatedAt" />
						<px:PXGridColumn DataField="AccountStringId" />
						<px:PXGridColumn DataField="CategoryGuid" />
						<px:PXGridColumn DataField="ExtendedTransactionType" />
						<px:PXGridColumn DataField="Id" />
						<px:PXGridColumn DataField="IsBillPay" />
						<px:PXGridColumn DataField="IsDirectDeposit" />
						<px:PXGridColumn DataField="IsExpense" />
						<px:PXGridColumn DataField="IsFee" />
						<px:PXGridColumn DataField="IsIncome" />
						<px:PXGridColumn DataField="IsInternational" />
						<px:PXGridColumn DataField="IsOverdraftFee" />
						<px:PXGridColumn DataField="IsPayrollAdvance" />
						<px:PXGridColumn DataField="IsRecurring" />
						<px:PXGridColumn DataField="IsSubscription" />
						<px:PXGridColumn DataField="Latitude" />
						<px:PXGridColumn DataField="LocalizedDescription" />
						<px:PXGridColumn DataField="LocalizedMemo" />
						<px:PXGridColumn DataField="Longitude" />
						<px:PXGridColumn DataField="MemberIsManagedByUser" />
						<px:PXGridColumn DataField="MerchantCategoryCode" />
						<px:PXGridColumn DataField="MerchantGuid" />
						<px:PXGridColumn DataField="MerchantLocationGuid" />
						<px:PXGridColumn DataField="Metadata" />
						<px:PXGridColumn DataField="OriginalDescription" />
						<px:PXGridColumn DataField="UserId" />
						<px:PXGridColumn DataField="AuthorizedDate" />
						<px:PXGridColumn DataField="AuthorizedDatetime" />
						<px:PXGridColumn DataField="DatetimeValue" />
						<px:PXGridColumn DataField="Address" />
						<px:PXGridColumn DataField="City" />
						<px:PXGridColumn DataField="Country" />
						<px:PXGridColumn DataField="PostalCode" />
						<px:PXGridColumn DataField="Region" />
						<px:PXGridColumn DataField="StoreNumber" />
						<px:PXGridColumn DataField="MerchantName" />
						<px:PXGridColumn DataField="PaymentChannel" />
						<px:PXGridColumn DataField="ByOrderOf" />
						<px:PXGridColumn DataField="Payee" />
						<px:PXGridColumn DataField="Payer" />
						<px:PXGridColumn DataField="PaymentMethod" />
						<px:PXGridColumn DataField="PaymentProcessor" />
						<px:PXGridColumn DataField="PpdId" />
						<px:PXGridColumn DataField="Reason" />
						<px:PXGridColumn DataField="ReferenceNumber" />
						<px:PXGridColumn DataField="PersonalFinanceCategory" />
						<px:PXGridColumn DataField="TransactionCode" />
						<px:PXGridColumn DataField="UnofficialCurrencyCode" />
						<px:PXGridColumn DataField="PartnerAccountID" />
					</Columns>
				</px:PXGridLevel>
			</Levels>
			<AutoSize Enabled="true" />
		</px:PXGrid>
		<px:PXPanel ID="pnl1" runat="server" SkinID="Buttons">
			<px:PXButton TextAlign="Center" Width="200px" Text="Load Transactions" runat="server" ID="btnLoadTransactions" CommandName="LoadTransactions" CommandSourceID="ds" />
			<px:PXButton ID="batClose" runat="server" Width="100px" DialogResult="Cancel" Text="Close" />
		</px:PXPanel>
	</px:PXSmartPanel>
	<px:PXSmartPanel AutoReload="true" AutoRepaint="true" Key="CurrentBankFeed" Caption="Access ID" CaptionVisible="True" runat="server" ID="spShowIDs">
		<px:PXFormView runat="server" ID="frmShowIDs" DataMember="CurrentBankFeed" SkinID="Transparent" DataSourceID="ds">
			<Template>
				<px:PXLayoutRule runat="server" ID="layoutRule8" StartColumn="True"></px:PXLayoutRule>
				<px:PXLabel Width="500px" Size="XL" Height="100px" runat="server" ID="lblWarningAccessToken" Text="Warning: These identifiers are used to authenticate your company in your financial institution, they are stored securely inside Acumatica ERP. You may be asked to provide this info to Acumatica or Plaid/MX support. If so, please take security precautions to send this information. DO NOT save this ID outside of Acumatica ERP, copy and paste it into an email, or provide it to anyone except Acumatica's or partner's support."></px:PXLabel>
				<px:PXTextEdit runat="server" ID="etLayoutRule9" DataField="ExternalUserID"></px:PXTextEdit>
				<px:PXTextEdit runat="server" ID="etLayoutRule8" DataField="ExternalItemID"></px:PXTextEdit>
			</Template>
		</px:PXFormView>
	</px:PXSmartPanel>
</asp:Content>
