<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SM209700.aspx.cs" Inherits="Page_SM209700" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PageLoadBehavior="InsertRecord"
        TypeName="PX.Objects.AP.InvoiceRecognition.PdfViewerManager" PrimaryView="File" HeaderDescriptionField="FileName">
		<CallbackCommands>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Width="100%" DataMember="File">
		<Template>
            <px:PXTextEdit runat="server" ID="edFileId" DataField="FileId" style="display: none;" />
		</Template>
	    <AutoSize Container="Window" Enabled="true"/>
        <ClientEvents AfterRepaint="renderPdf" Initialize="renderPdf" />
	</px:PXFormView>
    <div>
        <object id="pdfObject" width="100%" height="100%" type="application/pdf" style="position: absolute; left: 0; top: 0;" data="">
        </object>
    </div>
    <script type="text/javascript">
        const pdfBaseUrl = '~/Frames/GetFile.ashx?inmemory=1&fileID=';

        function renderPdf() {
            let fileIdControl = px_all[fileControlId];
            if (!fileIdControl) {
                return;
            }

            let fileId = fileIdControl.getValue();
            if (!fileId) {
                return;
            }

            let pdfObject = document.getElementById('pdfObject');
            if (!pdfObject) {
                return;
            }

            pdfObject.data = pdfBaseUrl + fileId;
        }
    </script>
</asp:Content>