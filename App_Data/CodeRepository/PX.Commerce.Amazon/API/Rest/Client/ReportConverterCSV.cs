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
using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;


namespace PX.Commerce.Amazon.API.Rest
{
    public class ReportConverterCSV: IAmazonReportConverter
    {
        public IEnumerable<TOut> ConverFromBytesTo<TOut>(byte[] byteReport)
            where TOut : IExternEntity, new()
        {
            IEnumerable<TOut> listings = GetRecordsFromReport<TOut>(byteReport);
            return listings;
        }

        private IEnumerable<TOut> GetRecordsFromReport<TOut>(byte[] bytes)
            where TOut : IExternEntity, new()
        {
            List<TOut> result = new List<TOut>();
            var codePage = Encoding.GetEncoding(Encoding.ASCII.EncodingName).CodePage;
            using (CSVReader csvReader = new CSVReader(bytes, codePage) { Separator = "\t" })
            {
                csvReader.Reset();
                Dictionary<int, string> headers = csvReader.IndexKeyPairs.ToDictionary(x => x.Key, x => x.Value);
                while (csvReader.MoveNext())
                {
                    TOut externalAPIObject = new TOut();

                    foreach (var propertyInfo in typeof(TOut).GetProperties())
                    {
                        string attributePropertyName = propertyInfo.GetCustomAttribute<Newtonsoft.Json.JsonPropertyAttribute>()?.PropertyName;
                        if (string.IsNullOrEmpty(attributePropertyName)) continue;

                        var value = csvReader.GetValue(headers.FirstOrDefault(x => x.Value == attributePropertyName).Key);
                        object convertedObject = propertyInfo.PropertyType.IsEnum ?
                            BCExtensions.ParseEnum(propertyInfo.PropertyType, value) :
                            Convert.ChangeType(value, propertyInfo.PropertyType);

                        propertyInfo.SetValue(externalAPIObject, convertedObject);
                    }
                    result.Add(externalAPIObject);
                }
            }

            return result;
        }
    }
}