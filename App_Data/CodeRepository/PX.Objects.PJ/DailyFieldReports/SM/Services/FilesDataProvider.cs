/* ---------------------------------------------------------------------*
*                             Acumatica Inc.                            *

*              Copyright (c) 2005-2024 All rights reserved.             *

*                                                                       *

*                                                                       *

* This file and its contents are protected by United States and         *

* International copyright laws.  Unauthorized reproduction and/or       *

* distribution of all or any portion of the code contained herein       *

* is strictly prohibited and will result in severe civil and criminal   *

* penalties.  Any violations of this copyright will be prosecuted       *

* to the fullest extent possible under law.                             *

*                                                                       *

* UNDER NO CIRCUMSTANCES MAY THE SOURCE CODE BE USED IN WHOLE OR IN     *

* PART, AS THE BASIS FOR CREATING A PRODUCT THAT PROVIDES THE SAME, OR  *

* SUBSTANTIALLY THE SAME, FUNCTIONALITY AS ANY ACUMATICA PRODUCT.       *

*                                                                       *

* THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE.              *

* --------------------------------------------------------------------- */

using System;
using System.Linq;
using PX.Objects.PJ.DailyFieldReports.PJ.DAC;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;

namespace PX.Objects.PJ.DailyFieldReports.SM.Services
{
    public class FilesDataProvider : IFilesDataProvider
    {
        private readonly PXGraph graph;

        public FilesDataProvider(PXGraph graph)
        {
            this.graph = graph;
        }

        public bool DoesFileHaveRelatedHistoryRevision(Guid? fileId)
        {
            return SelectFrom<DailyFieldReportHistory>
                .InnerJoin<NoteDoc>.On<NoteDoc.noteID.IsEqual<DailyFieldReportHistory.noteID>>
                .Where<NoteDoc.fileID.IsEqual<P.AsGuid>>.View.Select(graph, fileId).Any();
        }
    }
}
