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

using PX.Commerce.Amazon.API.Rest.Constants;
using PX.Commerce.Core;
using PX.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.Amazon.API.Rest
{
	public class ReportRunner<TOut>
		where TOut : IExternEntity, new()
	{
		private const int WAIT_DELAY = 5000;

		IAmazonReportDataProvider _dataProvider;
		IAmazonReportConverter _reportConverter;
		IAmazonReportReader _reportReader;

		public ReportRunner(IAmazonReportDataProvider dataProvider, IAmazonReportConverter reportConverter, IAmazonReportReader reportReader)
		{
			_dataProvider = dataProvider;
			_reportConverter = reportConverter;
			_reportReader = reportReader;
		}

		public async Task<IEnumerable<TOut>> GetRecordsFromReport()
		{
			ReportDocument report = await GetReport();
			byte[] byteReport = await _reportReader.ReadAsync(report.URL, report.CompressionAlgorithm);
			return _reportConverter.ConverFromBytesTo<TOut>(byteReport);
		}

		private async Task<ReportDocument> GetReport()
		{
			CreateReportResponse reportResponse = await _dataProvider.RequestReport();
			ReportVerificationDocument verificationResult = await VerifyReport(reportResponse.ReportId);
			ReportDocument report = await _dataProvider.RetrieveReport(verificationResult.ReportDocumentId);

			return report;
		}

		private async Task<ReportVerificationDocument> VerifyReport(string reportId)
		{
			ReportVerificationDocument result = await _dataProvider.GetReportProcessingStatus(reportId);

			while (result.ProcessingStatus.IsIn(FeedProcessingStatus.InProgress, FeedProcessingStatus.InQueue))
			{
				Thread.Sleep(WAIT_DELAY);
				result = await _dataProvider.GetReportProcessingStatus(reportId);
			}

			return result;
		}
	}
}
