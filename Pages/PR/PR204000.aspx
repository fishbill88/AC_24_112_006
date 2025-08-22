<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true"
	ValidateRequest="false" CodeFile="PR204000.aspx.cs" Inherits="Page_PR204000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.PR.PTOBankMaint" PrimaryView="Bank">
		<CallbackCommands>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Bank">
		<Template>
			<px:PXLayoutRule runat="server" StartRow="True" ControlSize="L" LabelsWidth="SM" />
			<px:PXSelector runat="server" ID="edBankID" DataField="BankID" CommitChanges="true" />
			
			<px:PXTextEdit runat="server" ID="edDescription" DataField="Description" />
			
			<px:PXSelector runat="server" ID="edEarningTypeCD" DataField="EarningTypeCD" />
			
			<px:PXLayoutRule runat="server" StartColumn="True" ControlSize="L" LabelsWidth="SM" />
			<px:PXCheckBox runat="server" ID="edIsActive" DataField="IsActive" />
			<px:PXCheckBox runat="server" ID="edApplyBandingRules" DataField="ApplyBandingRules" CommitChanges="true" />
			<px:PXCheckBox runat="server" ID="edIsCertifiedJobAccrual" DataField="IsCertifiedJobAccrual" />
			
			<px:PXCheckBox runat="server" ID="edCreateFinancialTransaction" DataField="CreateFinancialTransaction" CommitChanges="true" />

		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="server">
	<px:PXTab ID="tab" runat="server" Width="100%" Height="150px" DataSourceID="ds" DataMember="CurrentBank">
		<Items>
			<px:PXTabItem Text="General">
				<Template>
					<px:PXLayoutRule runat="server" StartColumn="true" LabelsWidth="SM" ControlSize="SM" />
					<px:PXDropDown runat="server" ID="edAccrualMethod" DataField="AccrualMethod" CommitChanges="true" />

					
					<px:PXDropDown runat="server" ID="edTransferDateType" DataField="TransferDateType" CommitChanges="true" />
					<px:PXLayoutRule runat="server" LabelsWidth="SM" ControlSize="S" Merge="true" />
					<px:PXDateTimeEdit runat="server" ID="edStartDateError" DataField="StartDate" Enabled="false" LabelWidth="134px" Width="0" />
					<px:PXDropDown runat="server" ID="edStartDateMonth" DataField="StartDateMonth" CommitChanges="true" SuppressLabel="true" />
					<px:PXNumberEdit runat="server" ID="edStartDateDay" DataField="StartDateDay" CommitChanges="true" SuppressLabel="true" Width="35" />
					
					<px:PXLayoutRule runat="server" StartColumn="true" LabelsWidth="SM" ControlSize="SM" />
					<px:PXDropDown runat="server" ID="edCarryoverType" DataField="CarryoverType" CommitChanges="true" />
					<px:PXDropDown ID="edSettlementBalanceType" DataField="SettlementBalanceType" CommitChanges="true" runat="server" />
					
					<px:PXLayoutRule runat="server" GroupCaption="Disbursing Rules" />
					<px:PXDropDown runat="server" ID="edDisbursingType" DataField="DisbursingType" CommitChanges="true" />

					
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Employee Classes">
				<Template>
					<px:PXGrid runat="server" DataSourceID="ds" ID="grdPTOBanks" SkinID="DetailsInTab" Width="100%">
						<Levels>
							<px:PXGridLevel DataMember="EmployeeClassPTOBanks">
								<RowTemplate>
									<px:PXNumberEdit ID="edAccrualRate" runat="server" DataField="AccrualRate" />
									<px:PXNumberEdit ID="edHoursPerYear" runat="server" DataField="HoursPerYear" />
									<px:PXNumberEdit ID="edAccrualLimit" runat="server" DataField="AccrualLimit" />
									<px:PXNumberEdit ID="edCarryoverAmount" runat="server" DataField="CarryoverAmount" />
									<px:PXNumberEdit ID="edFrontLoadingAmount" runat="server" DataField="FrontLoadingAmount" />
								</RowTemplate>
								<Columns>
									<px:PXGridColumn DataField="IsActive" Type="CheckBox" TextAlign="Center" Width="60px" />
									<px:PXGridColumn DataField="EmployeeClassID" CommitChanges="true" />
									<px:PXGridColumn DataField="StartDate" CommitChanges="true" />
									<px:PXGridColumn DataField="AccrualRate" />
									<px:PXGridColumn DataField="HoursPerYear" />
									<px:PXGridColumn DataField="AccrualLimit" />
									<px:PXGridColumn DataField="AllowNegativeBalance" Type="CheckBox" TextAlign="Center" Width="60px" />
									<px:PXGridColumn DataField="DisburseFromCarryover" Type="CheckBox" TextAlign="Center" Width="60px" />
									<px:PXGridColumn DataField="CarryoverAmount" CommitChanges="true" />
									<px:PXGridColumn DataField="FrontLoadingAmount" />
									<px:PXGridColumn DataField="ProbationPeriodBehaviour" />
								</Columns>
							</px:PXGridLevel>
						</Levels>
						<AutoSize Container="Window" Enabled="True" MinHeight="150" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Banding rules" VisibleExp="DataControls[&quot;edApplyBandingRules&quot;].Value == true" BindingContext="form" RepaintOnDemand="false">
				<Template>
					<px:PXFormView ID="splitTypeForm" runat="server" DataMember="CurrentBank">
						<Template>
							<px:PXLayoutRule runat="server" StartRow="true" ControlSize="S" LabelsWidth="L" />
							<px:PXDropDown runat="server" ID="edBandingRuleRoundingMethod" DataField="BandingRuleRoundingMethod" />
						</Template>
					</px:PXFormView>
					<px:PXGrid ID="grdBandingRules" runat="server" DataSourceID="ds" SkinID="DetailsInTab" Width="100%" >
						<Levels>
							<px:PXGridLevel DataMember="BandingRulePTOBanks">
								<RowTemplate>
									<px:PXNumberEdit ID="edAccrualRateBandingRule" runat="server" DataField="AccrualRate" />
									<px:PXNumberEdit ID="edHoursPerYearBandingRule" runat="server" DataField="HoursPerYear" />
									<px:PXNumberEdit ID="edAccrualLimitBandingRule" runat="server" DataField="AccrualLimit" />
									<px:PXNumberEdit ID="edFrontLoadingAmountBandingRule" runat="server" DataField="FrontLoadingAmount" />
									<px:PXNumberEdit ID="edCarryoverAmountBandingRule" runat="server" DataField="CarryoverAmount" />
								</RowTemplate>
								<Columns>
									<px:PXGridColumn DataField="EmployeeClassID" CommitChanges="true" />
									<px:PXGridColumn DataField="YearsOfService" CommitChanges="true" />
									<px:PXGridColumn DataField="AccrualRate" />
									<px:PXGridColumn DataField="HoursPerYear" />
									<px:PXGridColumn DataField="AccrualLimit" />
									<px:PXGridColumn DataField="FrontLoadingAmount" />
									<px:PXGridColumn DataField="CarryoverAmount" CommitChanges="true" />
								</Columns>
							</px:PXGridLevel>
						</Levels>
						<AutoSize Container="Window" Enabled="True" MinHeight="150" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="GL Accounts" RepaintOnDemand="false">
				<Template>
					<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
					<px:PXSelector ID="edPTOExpenseActID" runat="server" DataField="PTOExpenseAcctID" />
					<px:PXSegmentMask ID="edPTOExpenseID" runat="server" DataField="PTOExpenseSubID" />
					<px:PXSelector ID="edPTOLiabilityAcctID" runat="server" DataField="PTOLiabilityAcctID" />
					<px:PXSegmentMask ID="edPTOLiabilitySubID" runat="server" DataField="PTOLiabilitySubID" />
					<px:PXSelector ID="edPTOAssetAcctID" runat="server" DataField="PTOAssetAcctID" />
					<px:PXSegmentMask ID="edPTOAssetSubID" runat="server" DataField="PTOAssetSubID" />
				</Template>
			</px:PXTabItem>
		</Items>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXTab>
</asp:Content>
