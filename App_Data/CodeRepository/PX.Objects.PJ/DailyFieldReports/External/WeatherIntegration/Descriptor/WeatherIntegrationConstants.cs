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

using PX.Data.BQL;
using PX.Web.UI;

namespace PX.Objects.PJ.DailyFieldReports.External.WeatherIntegration.Descriptor
{
    public class WeatherIntegrationConstants
    {
        public const string NoContentResponse = "null";

        public class ProcessingLogFilter
        {
            public const int DefaultRequestDateFromDifference = -4;
        }

        public class WeatherIntegrationSetup
        {
            public const int DefaultWeatherProcessingLogTerm = 5;
        }

        public class UnitOfMeasures
        {
            public const string Fahrenheit = "imperial";
            public const string WeatherBitFahrenheit = "I";
            public const decimal NumberOfMetersPerMile = 2.237M;
            public const decimal NumberOfMillimetersPerInch = 25.4M;
        }

        public class RequestStatusIcons
        {
            public const string RequestStatusHeaderIcon = Sprite.AliasControl + "@" + Sprite.Control.Complete;
            public const string RequestStatusSuccessIcon = Sprite.AliasMain + "@" + Sprite.Main.Success;
            public const string RequestStatusFailIcon = Sprite.AliasMain + "@" + Sprite.Main.Fail;

            public class requestStatusFailIcon : BqlString.Constant<requestStatusFailIcon>
            {
                public requestStatusFailIcon()
                    : base(RequestStatusFailIcon)
                {
                }
            }
        }

        public class WeatherBitServiceWeatherCodes
        {
            public const string StartCodeOfSnowRange = "6";
        }
    }
}
