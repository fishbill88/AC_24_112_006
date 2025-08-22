<px:PXSmartPanel ID="CreatePrepaymentInvoiceSmartPanel" runat="server" Caption="Create Prepayment Invoice" CaptionVisible="True" LoadOnDemand="True" Key="QuickPrepaymentInvoice"
	AutoCallBack-Target="CreatePrepaymentInvoiceFormView" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True" CallBackMode-PostData="Page">

	<px:PXLayoutRule runat="server" StartColumn="True" />

	<px:PXFormView ID="CreatePrepaymentInvoiceFormView" runat="server" DataSourceID="ds" Width="100%" DataMember="QuickPrepaymentInvoice" CaptionVisible="False" SkinID="Transparent" Height="70px">
		<Template>
			<px:PXLayoutRule runat="server" LabelsWidth="130px"/>
			<px:PXNumberEdit ID="edPrepaymentPct" runat="server" DataField="PrepaymentPct" CommitChanges="True"/>
			<px:PXLayoutRule runat="server" Merge="True" />
			<px:PXNumberEdit ID="edCuryPrepaymentAmt" runat="server" DataField="CuryPrepaymentAmt" CommitChanges="True"/>
			<px:PXSelector ID="edCuryID" runat="server" DataField="CuryID" Enabled="False" SuppressLabel="True" Width="60px"/>
		</Template>
	</px:PXFormView>

	<px:PXPanel runat="server" ID="CreatePrepaymentInvoiceSmartButtons" SkinID="Buttons">
		<px:PXButton ID="CreatePrepaymentInvoiceOKButton" runat="server" CommitChanges="True" CommandName="CreatePrepaymentInvoiceOK" CommandSourceID="ds" Height="20" SyncVisible="true" DialogResult="OK" />
		<px:PXButton ID="CreatePrepaymentInvoiceCancelButton" runat="server" DialogResult="Cancel" Text="Cancel" Height="20" />
	</px:PXPanel>

</px:PXSmartPanel>