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

using PX.Commerce.Core;
using PX.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PX.Commerce.Amazon.API.Rest
{
	public class ReportDataProvider : RestDataProviderBase, IAmazonReportDataProvider
	{
		private string _reportType;

		private string[] _marketplaceIds;

		protected override string GetListUrl => throw new NotImplementedException();

		protected override string GetSingleUrl => throw new NotImplementedException();

		protected string CreateReportUrl { get; } = "/reports/2021-06-30/reports";

		protected string GetReportUrl { get; } = "/reports/2021-06-30/reports/{reportId}";

		protected string GetReportDocumentUrl { get; } = "/reports/2021-06-30/documents/{reportDocumentId}";

		public ReportDataProvider(IAmazonRestClient restClient, string reportType, string[] marketplaceIds)
			: base(restClient)
		{
			_reportType = reportType;
			_marketplaceIds = marketplaceIds;
		}

		private UrlSegments MakeUrlSegments(string id, string key)
		{
			var segments = new UrlSegments();
			segments.Add(key, id);
			return segments;
		}

		public async Task<CreateReportResponse> RequestReport()
		{
			CreateReportSpecification data = new CreateReportSpecification()
			{
				reportType = _reportType,
				MarketplaceIds = _marketplaceIds
			};
			return await base.Post<CreateReportSpecification, CreateReportResponse>(data, CreateReportUrl);
		}

		public async Task<ReportVerificationDocument> GetReportProcessingStatus(string reportId)
		{
			return await GetByID<ReportVerificationDocument>(MakeUrlSegments(reportId, "reportId"), GetReportUrl);
		}

		public async Task<ReportDocument> RetrieveReport(string reportId)
		{
			return await base.GetByID<ReportDocument>(MakeUrlSegments(reportId, "reportDocumentId"), GetReportDocumentUrl);
		}
	}
}
