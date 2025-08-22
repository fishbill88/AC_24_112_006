<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="PR304000.aspx.cs" Inherits="PR_PR304000" Title="Batch for Submission" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="server">
	<asp:ScriptManager ID='scriptManager1' runat='server' EnablePageMethods='true' />
		<script type="text/javascript">
            var previousEmployeeID = null;

			function generatePdf(e, args) {
				if (args.row != null && (args.oldRow == null || args.row.element.id != args.oldRow.element.id)) {
					for (var index in args.row.cells) {
						var cell = args.row.cells[index];
                        if (cell.column.dataField == "EmployeeID" && previousEmployeeID != cell.getValue()) {
							px_alls.ds.executeCallback("GeneratePdfDocument", cell.getValue());
                            previousEmployeeID = cell.getValue();
                            break;
                        }
                    }
				}
            }

			function commandResult(ds, context) {
				if (context.command == 'GeneratePdfDocument') {
                    var pdfBase64String = px_callback.getLargeProp({ ID: "PdfWebControl" }, "PdfAsString");

                    var iframe = document.getElementById('t4Form');
                    if (pdfBase64String != null) {
                        iframe.src = 'data:application/pdf;base64,' + pdfBase64String;
                    }
                    else {
                        iframe.src = ''
                    }
                }
            }
        </script>

	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.PR.PRTaxFormBatchMaint" PrimaryView="SubmissionDocument" >
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Publish" Visible="False" CommitChanges="true" />
            <px:PXDSCallbackCommand Name="Unpublish" Visible="false" CommitChanges="true" />
			<px:PXDSCallbackCommand Name="ViewTaxFormBatch" Visible="False" DependOnGrid="gridBatchEmployees" />
        </CallbackCommands>
		<ClientEvents CommandPerformed="commandResult" /> 
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="server">
	<px:PXSplitContainer ID="splitContainerT4" runat="server" PositionInPercent="true" SplitterPosition="50" Orientation="Vertical" Height="100%">
		<Template1>
			<px:PXFormView ID="batchForm" runat="server" Style="z-index: 100" Width="100%" DataMember="SubmissionDocument">
				<Template>
					<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
					<px:PXSelector ID="edBatchID" runat="server" DataField="BatchID" />
					<px:PXDropDown ID="edFormType" runat="server" DataField="FormType" />
					<px:PXTextEdit ID="edYear" runat="server" DataField="Year" />
					<px:PXSelector ID="edOrgBAccountID" runat="server" DataField="OrgBAccountID" />
					<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
					<px:PXDropDown ID="edDocType" runat="server" DataField="DocType" />
					<px:PXTextEdit ID="edDownloadedAt" runat="server" DataField="DownloadedAt" />
					<px:PXNumberEdit ID="edNumberOfEmployees" runat="server" DataField="NumberOfEmployees" />
				</Template>
			</px:PXFormView>
			<px:PXGrid ID="gridBatchEmployees" runat="server" DataSourceID="ds" KeepPosition="True" SyncPosition="True" SkinID="Inquire" Height="500px" Width="100%">
				<Levels>
					<px:PXGridLevel DataMember="BatchEmployees">
						<Columns>
							<px:PXGridColumn DataField="EmployeeID" />
							<px:PXGridColumn DataField="PREmployee__AcctCD" />
							<px:PXGridColumn DataField="PREmployee__AcctName" />
							<px:PXGridColumn DataField="PREmployee__ParentBAccountID" />
							<px:PXGridColumn DataField="PREmployee__PayGroupID" />
							<px:PXGridColumn DataField="Published" Type="CheckBox" TextAlign="Center" />
							<px:PXGridColumn DataField="PublishedFrom" LinkCommand="ViewTaxFormBatch" />
							<px:PXGridColumn DataField="NotPublished" Type="CheckBox" AllowShowHide="False" Visible="False" />
						</Columns>
					</px:PXGridLevel>
				</Levels>
				<AutoSize Container="Parent" Enabled="True" MinHeight="200" />
				<ClientEvents AfterRowChange="generatePdf" />
				<ActionBar>
					<CustomItems>
						<px:PXToolBarButton Text="Publish" DependOnGrid="gridBatchEmployees" CommandSourceID="ds" StateColumn="NotPublished">
							<AutoCallBack Command="Publish" Target="ds" />
						</px:PXToolBarButton>
						<px:PXToolBarButton Text="Unpublish" DependOnGrid="gridBatchEmployees" CommandSourceID="ds" StateColumn="Published">
							<AutoCallBack Command="Unpublish" Target="ds" />
						</px:PXToolBarButton>
					</CustomItems>
				</ActionBar>
			</px:PXGrid>
		</Template1>
		<Template2>
			<px:PXTab ID="tab" runat="server" Width="100%" DataSourceID="ds">
				<Items>
					<px:PXTabItem Text="Document" >
						<Template>
							<px:PXFormView ID="form" runat="server" DataSourceID="ds" AllowCollapse="False" Width="100%" Height="100%" TabIndex="120" CaptionVisible="false">
								<Template>
									<table runat="server" style="width: 100%; height: 100%">
										<tr>
											<td>
												<iframe id="t4Form" width="100%" height="700" scrolling="yes" type="application/pdf"></iframe>
											</td>
										</tr>
									</table>
								</Template>
								<AutoSize Container="Window" Enabled="True" />
							</px:PXFormView>
						</Template>
					</px:PXTabItem>
				</Items>
				<AutoSize Container="Window" Enabled="True" MinHeight="150" />
			</px:PXTab>			
		</Template2>
		<AutoSize Enabled="true" Container="Window" />
	</px:PXSplitContainer>
	<px:pxsmartpanel runat="server" id="pnlEmployeeSlipAlreadyPublished" caption="Confirmation" captionvisible="true" key="EmployeeSlipAlreadyPublished" autorepaint="True" closebuttondialogresult="No">
        <px:PXFormView ID="EmployeeSlipAlreadyPublishedForm" runat="server" DataSourceID="ds" DataMember="EmployeeSlipAlreadyPublished" RenderStyle="Simple" SkinID="Transparent">
            <Template>
                <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" SuppressLabel="True" />
                <px:PXTextEdit ID="edEmployeeSlipAlreadyPublished" runat="server" DataField="Message" TextMode="MultiLine" Enabled="False" Height="100" Width="300" />
            </Template>
        </px:PXFormView>
        <px:PXButton ID="btnConfirm" runat="server" Text="Confirm" DialogResult="OK" />
        <px:PXButton ID="btnCancel" runat="server" Text="Cancel" DialogResult="Cancel" />
    </px:pxsmartpanel>
</asp:Content>