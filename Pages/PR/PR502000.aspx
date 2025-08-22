<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="PR502000.aspx.cs" Inherits="PR_PR502000" Title="Prepare Tax Forms" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="server">
	<asp:ScriptManager ID='scriptManager1' runat='server' EnablePageMethods='true' />
		<script type="text/javascript">
			var requestPdf = true;
            var previousTaxForm = null;
            var previousYear = null;
			var previousEmployeeID = null;
			var previousIncludeUnreleasedPaychecks = null;

			function generatePdf(e, args) {
                if (!requestPdf) {
                    requestPdf = true;
                    return;
                }

				if (args.row != null && (args.oldRow == null || args.row.element.id != args.oldRow.element.id)) {
                    for (var index in args.row.cells) {
                        var cell = args.row.cells[index];
						if (cell.column.dataField == "BAccountID") {
                            if (previousEmployeeID != cell.getValue() || previousTaxForm != px_alls["edTaxForm"].value || previousYear != px_alls["edYear"].value || previousIncludeUnreleasedPaychecks != px_alls["edIncludeUnreleasedPaychecks"].value) {
								px_alls.ds.executeCallback("GeneratePdfDocument", cell.getValue());
								previousEmployeeID = cell.getValue();
                                previousTaxForm = px_alls["edTaxForm"].value;
                                previousYear = px_alls["edYear"].value;
								previousIncludeUnreleasedPaychecks = px_alls["edIncludeUnreleasedPaychecks"].value;
								break;
							}
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

            function refreshEmployeesWithPaychecksGrid(e, args) {
				if (e.activeControl.ID == px_alls["edOperation"].ID && e.activeControl.value == "CRT") {
					requestPdf = false;
					px_alls.gridEmployeesWithPaychecks.refresh();
				}
            }
        </script>

	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Objects.PR.PRPrepareTaxFormsMaint" PrimaryView="Filter" >
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Schedule" Visible="False" />
            <px:PXDSCallbackCommand Name="ViewTaxFormBatch" Visible="False" DependOnGrid="gridEmployeesWithPaychecks" />
            <px:PXDSCallbackCommand Name="ViewDiscrepancies" Visible="False" DependOnGrid="gridEmployeesWithPaychecks" />
        </CallbackCommands>
		<ClientEvents CommandPerformed="commandResult" /> 
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="server">
	<px:PXSplitContainer ID="splitContainerT4" runat="server" PositionInPercent="true" SplitterPosition="50" Orientation="Vertical" Height="100%">
		<Template1>
			<px:PXFormView ID="filterForm" runat="server" Style="z-index: 100" Width="100%" DataMember="Filter">
				<Template>
					<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
					<px:PXDropDown ID="edTaxForm" runat="server" DataField="TaxForm" CommitChanges="True" />
					<px:PXSelector ID="edYear" runat="server" DataField="Year" CommitChanges="True" />
					<px:PXBranchSelector ID="edOrgBAccountID" runat="server" DataField="OrgBAccountID" CommitChanges="True" />
					<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="M" />
					<px:PXDropDown ID="edOperation" runat="server" DataField="Operation" CommitChanges="True" />
					<px:PXDropDown ID="edDocType" runat="server" DataField="DocType" CommitChanges="True" />
					<px:PXCheckBox ID="edIncludeUnreleasedPaychecks" runat="server" DataField="IncludeUnreleasedPaychecks" CommitChanges="True" />
				</Template>
				<ClientEvents AfterRepaint="refreshEmployeesWithPaychecksGrid" />
			</px:PXFormView>
			<px:PXGrid ID="gridEmployeesWithPaychecks" runat="server" DataSourceID="ds" KeepPosition="True" SyncPosition="True" SkinID="Inquire" Height="500px" Width="100%">
				<Levels>
					<px:PXGridLevel DataMember="EmployeesWithPaychecksList">
						<Columns>
							<px:PXGridColumn DataField="BAccountID" />
							<px:PXGridColumn DataField="Selected" Type="CheckBox" TextAlign="Center" />
							<px:PXGridColumn DataField="AcctCD" />
							<px:PXGridColumn DataField="AcctName" />
							<px:PXGridColumn DataField="ParentBAccountID" />
							<px:PXGridColumn DataField="PayGroupName" />
							<px:PXGridColumn DataField="PublishedFrom" LinkCommand="ViewTaxFormBatch" />
							<px:PXGridColumn DataField="DocType" />
							<px:PXGridColumn DataField="BatchID" />
							<px:PXGridColumn DataField="HasDiscrepancies" Type="CheckBox" />
						</Columns>
					</px:PXGridLevel>
				</Levels>
				<AutoSize Container="Parent" Enabled="True" MinHeight="200" />
				<ClientEvents AfterRowChange="generatePdf" />
				<ActionBar>
					<CustomItems>
						<px:PXToolBarButton Text="View Discrepancies" DependOnGrid="gridEmployeesWithPaychecks" CommandSourceID="ds" StateColumn="HasDiscrepancies">
							<AutoCallBack Command="ViewDiscrepancies" Target="ds" />
						</px:PXToolBarButton>
					</CustomItems>
				</ActionBar>
			</px:PXGrid>
		</Template1>
		<Template2>
			<px:PXTab ID="tab" runat="server" Width="100%" DataSourceID="ds">
				<Items>
					<px:PXTabItem Text="Tax Form" >
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
	<px:pxsmartpanel runat="server" id="pnlViewDiscrepancies" caption="View Discrepancies" captionvisible="true" key="TaxFormDiscrepancies" autorepaint="True">
        <px:PXGrid ID="ViewDiscrepanciesGrid" runat="server" SyncPosition="True" DataSourceID="ds" SkinID="Inquire">
            <Levels>
                <px:PXGridLevel DataMember="TaxFormDiscrepancies">
                    <Columns>
                        <px:PXGridColumn DataField="EmployeeID" Width="240px" />
                        <px:PXGridColumn DataField="Box" Width="240px" />
                        <px:PXGridColumn DataField="OldValue" Width="120px" />
                        <px:PXGridColumn DataField="NewValue" Width="120px" />
                    </Columns>
                </px:PXGridLevel>
            </Levels>
            <AutoSize Enabled="true" MinHeight="300" />
        </px:PXGrid>
        <px:PXPanel ID="pnlViewDiscrepanciesButtons" runat="server" SkinID="Buttons">
			<px:PXButton ID="btnOK" runat="server" Text="OK" DialogResult="OK" />
		</px:PXPanel>
    </px:pxsmartpanel>
</asp:Content>