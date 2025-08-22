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
using PX.Data;
using PX.Data.BQL;
using PX.Objects.CN.Common.DAC;

namespace PX.Objects.PJ.DailyFieldReports.PJ.DAC
{
    [PXCacheName("Weather Processing Log")]
    public class WeatherProcessingLog : BaseCache, IBqlTable
    {
        [PXDBIdentity(IsKey = true)]
        public int? WeatherProcessingLogId
        {
            get;
            set;
        }

        [PXBool]
        public bool? Selected
        {
            get;
            set;
        }

        [PXDBInt]
        [PXDefault(typeof(DailyFieldReport.dailyFieldReportId.FromCurrent))]
        public int? DailyFieldReportId
        {
            get;
            set;
        }

        [PXDBString(20, IsUnicode = true)]
        [PXDefault(typeof(WeatherIntegrationSetup.weatherApiService.FromCurrent))]
        [PXUIField(DisplayName = "Weather Service", Required = false)]
        public string WeatherService
        {
            get;
            set;
        }

        [PXDBDate(PreserveTime = true)]
        [PXUIField(DisplayName = "Request Time")]
        public DateTime? RequestTime
        {
            get;
            set;
        }

        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = "Request Body")]
        public string RequestBody
        {
            get;
            set;
        }

        [PXDBImage]
        [PXUIField(DisplayName = "Request Status Icon")]
        public string RequestStatusIcon
        {
            get;
            set;
        }

        [PXDBDate(PreserveTime = true)]
        [PXUIField(DisplayName = "Response Time")]
        public DateTime? ResponseTime
        {
            get;
            set;
        }

        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = "Response Body")]
        public string ResponseBody
        {
            get;
            set;
        }

        public abstract class weatherProcessingLogId : BqlInt.Field<weatherProcessingLogId>
        {
        }

        public abstract class selected : BqlBool.Field<selected>
        {
        }

        public abstract class dailyFieldReportId : BqlInt.Field<dailyFieldReportId>
        {
        }

        public abstract class weatherService : BqlString.Field<weatherService>
        {
        }

        public abstract class requestTime : BqlDateTime.Field<requestTime>
        {
        }

        public abstract class requestBody : BqlString.Field<requestBody>
        {
        }

        public abstract class requestStatusIcon : BqlString.Field<requestStatusIcon>
        {
        }

        public abstract class responseTime : BqlDateTime.Field<responseTime>
        {
        }

        public abstract class responseBody : BqlString.Field<responseBody>
        {
        }

        public abstract class tstamp : BqlByteArray.Field<tstamp>
        {
        }

        public abstract class createdById : BqlGuid.Field<createdById>
        {
        }

        public abstract class createdByScreenId : BqlString.Field<createdByScreenId>
        {
        }

        public abstract class createdDateTime : BqlDateTime.Field<createdDateTime>
        {
        }

        public abstract class lastModifiedById : BqlGuid.Field<lastModifiedById>
        {
        }

        public abstract class lastModifiedByScreenId : BqlString.Field<lastModifiedByScreenId>
        {
        }

        public abstract class lastModifiedDateTime : BqlDateTime.Field<lastModifiedDateTime>
        {
        }

        public abstract class noteID : BqlGuid.Field<noteID>
        {
        }
    }
}
