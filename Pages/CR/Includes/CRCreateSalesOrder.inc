<px:PXSmartPanel ID="panelCreateSalesOrder" Key="CreateOrderParams" runat="server" Style="z-index: 108; left: 27px; position: absolute; top: 99px;" Caption="Create Sales Order" CaptionVisible="true"
	DesignView="Hidden" LoadOnDemand="true" AllowResize="False" AutoResize="True" AutoCallBack-Enabled="true" AutoCallBack-Target="formCopyTo" AutoCallBack-Command="Refresh"	CallBackMode-CommitChanges="True" CallBackMode-PostData="Page"
	AcceptButtonID="btnCreateSalesOrderOK" CancelButtonID="btnCreateSalesOrderCancel" CloseButtonDialogResult="Abort" AutoReload="True" Width="520px" >

	<px:PXTab ID="tabCreateSalesOrder" runat="server" DataSourceID="ds" DataMember="CreateOrderParams"  Width="100%">
		<Items>
			<px:PXTabItem Text="Main" RepaintOnDemand="True">
				<Template>
					<px:PXFormView runat="server" ID="frmCreateSalesOrderSummary" DataSourceID="ds" DataMember="CreateOrderParams"
								   Style="z-index: 100" Width="100%" CaptionVisible="False" DefaultControlID="edOrderNbr">
						<Template>
							<px:PXLayoutRule runat="server" GroupCaption="Sales Order" />
							<px:PXLayoutRule ID="PXLayoutRule7" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
							<px:PXSelector ID="edOrderType" runat="server" AllowNull="False" DataField="OrderType" DisplayMode="Hint" CommitChanges="True" />
							<px:PXTextEdit ID="edOrderNbr" runat="server" DataField="OrderNbr" />
							<px:PXCheckBox ID="edMakeQuotePrimary" runat="server" DataField="MakeQuotePrimary" CommitChanges="true"/>
							<px:PXCheckBox ID="PXCheckBox2" runat="server" DataField="RecalculatePrices" CommitChanges="true" />
							<px:PXCheckBox ID="PXCheckBox3" runat="server" DataField="OverrideManualPrices" CommitChanges="true" Style="margin-left: 25px" />
							<px:PXCheckBox ID="PXCheckBox4" runat="server" DataField="RecalculateDiscounts" CommitChanges="true"/>
							<px:PXCheckBox ID="PXCheckBox5" runat="server" DataField="OverrideManualDiscounts" CommitChanges="true" Style="margin-left: 25px"/>
							<px:PXCheckBox ID="PXCheckBox6" runat="server" DataField="OverrideManualDocGroupDiscounts" CommitChanges="true" Style="margin-left: 25px" /> 
							<px:PXCheckBox ID="edConfirmManualAmount" runat="server" DataField="ConfirmManualAmount" Width="480px" CommitChanges="true" UncheckImages="" />
							<px:PXCheckBox ID="cstChkAMIncludeEstimate" runat="server" DataField="AMIncludeEstimate" />
							<px:PXCheckBox ID="cstChkAMCopyConfigurations" runat="server" DataField="AMCopyConfigurations" />
						</Template>
					</px:PXFormView>

					<px:PXFormView ID="frmCustomerInfo" runat="server" DataSourceID="ds" Width="100%" Caption="Creation Dialog" CaptionVisible="False" SkinID="Transparent"
									DataMember="CustomerInfo">
						<Template>
							<px:PXLayoutRule runat="server" GroupCaption="Customer"/>
							<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM"/>
							<px:PXSelector ID="edAcctCD" runat="server" DataField="AcctCD" DisplayMode="Hint" TabIndex="40"/>
							<px:PXSelector ID="edClassID" runat="server" DataField="ClassID" TabIndex="60" AutoRefresh="True"/>
							<px:PXMailEdit ID="edEmail" runat="server" DataField="Email" TabIndex="70"/>
							
							<px:PXLayoutRule runat="server" StartRow="True" SuppressLabel="True" />
							<px:PXTextEdit
								runat="server"
								ID="edCreateCustomerWarning"
								Encode="True"
								DataField="WarningMessage"
								TextMode="MultiLine"
								DisableSpellcheck="True"
								Height="70px"
							/>

						</Template>
					</px:PXFormView>


				</Template>
			</px:PXTabItem>

			<px:PXTabItem Text="Attributes" RepaintOnDemand="False" BindingContext="frmCustomerInfo" VisibleExp="DataControls[&quot;edAcctCD&quot;].Hidden == false">
				<Template>
					<px:PXLayoutRule runat="server" GroupCaption="Customer"/>
					<px:PXGrid ID="grdCustomerInfoAttributes" runat="server" Width="100%" Height="211px" DataSourceID="ds" MatrixMode="True" AutoAdjustColumns="true">
						<Levels>
							<px:PXGridLevel DataMember="CustomerInfoAttributes">
								<Columns>
									<px:PXGridColumn DataField="DisplayName" Width="244px"/>
									<px:PXGridColumn DataField="Value" AutoCallBack="True" CommitChanges="True" Width="244px"/>
								</Columns>
								<Layout ColumnsMenu="False" />
								<Mode AllowAddNew="false" AllowDelete="false" />
							</px:PXGridLevel>
						</Levels>
						<Mode InitNewRow="true" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>

			<px:PXTabItem Text="User-Defined Fields" RepaintOnDemand="false" BindingContext="frmCustomerInfo" VisibleExp="DataControls[&quot;edAcctCD&quot;].Hidden == false">
				<Template>
					<px:PXLayoutRule runat="server" GroupCaption="Customer"/>
					<px:PXGrid ID="grdCustomerInfoOrderUDF" runat="server" Width="100%" Height="211px" DataSourceID="ds" MatrixMode="True" AutoAdjustColumns="true">
						<Levels>
							<px:PXGridLevel DataMember="CustomerInfoUDF">
								<Columns>
									<px:PXGridColumn DataField="DisplayName" Width="244px"/>
									<px:PXGridColumn DataField="Value" AutoCallBack="True" CommitChanges="True" Width="244px"/>
								</Columns>
								<Layout ColumnsMenu="False" />
								<Mode AllowAddNew="false" AllowDelete="false" />
							</px:PXGridLevel>
						</Levels>
						<Mode InitNewRow="true" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>

		</Items>
	</px:PXTab>

	<px:PXPanel ID="pnlCreateSalesOrderButtons" runat="server" SkinID="Transparent">
		<px:PXButton ID="btnCreateSalesOrderCreateCustomer" runat="server" CommandSourceID="ds" CommandName="CreateCustomerInPanel"
			style="float: left; margin: 2px" DialogResult="Retry"/>
		
		<px:PXButton ID="btnCreateSalesOrderCancel" runat="server"
			style="float: right; margin: 2px" Text="Cancel" DialogResult="Abort"/>

		<px:PXButton ID="btnCreateSalesOrderOK" runat="server" CommandSourceID="ds" CommandName="CreateSalesOrderInPanel"
			style="float: right; margin: 2px" Text="Create And Review" DialogResult="OK"/>

	</px:PXPanel>

	<style type="text/css">
		#ctl00_phDialogs_panelCreateSalesOrder_tabCreateSalesOrder_t0_frmCustomerInfo_edCreateCustomerWarning
		{
			border: 0px;
			background-color: #FFFACD !important;
			border-radius: 5px;
			padding: 10px;
			padding-left: 40px;
			vertical-align: middle;
			height: auto;

			background-image: url("data:image/svg+xml;utf8,<svg width='24' height='24' viewBox='0 0 24 24' fill='none' xmlns='http://www.w3.org/2000/svg'><path fill-rule='evenodd' clip-rule='evenodd' d='M11.1195 3.13515C11.4971 2.43395 12.5029 2.43395 12.8805 3.13515L21.7063 19.5259C22.065 20.1921 21.5825 21 20.8258 21H3.17422C2.41754 21 1.93501 20.1921 2.29374 19.5259L11.1195 3.13515ZM13.5 16.5C13.5 17.3284 12.8284 18 12 18C11.1716 18 10.5 17.3284 10.5 16.5C10.5 15.6716 11.1716 15 12 15C12.8284 15 13.5 15.6716 13.5 16.5ZM13.5 8.99999C13.5 8.17157 12.8284 7.49999 12 7.49999C11.1716 7.49999 10.5 8.17157 10.5 8.99999V12C10.5 12.8284 11.1716 13.5 12 13.5C12.8284 13.5 13.5 12.8284 13.5 12V8.99999Z' fill='%23FFAE4E'/></svg>");
			background-position: 10px 10px;
			background-repeat: no-repeat;
		}
	</style>
</px:PXSmartPanel>