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
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace PX.Commerce.Amazon.API.Rest
{
	public class ReportReader : IAmazonReportReader
	{
		public async Task<byte[]> ReadAsync(string url, string compressionAlgorithm)
		{
			using HttpClient client = new HttpClient();
			using Stream stream = await client.GetStreamAsync(url);
			byte[] binaryReport = await ReadAsync(stream);
			return compressionAlgorithm == CompressionAlgorithm.GZIP ? await Decompress(binaryReport)
				: binaryReport;
		}

		private async Task<byte[]> ReadAsync(Stream input)
		{
			byte[] buffer = new byte[16 * 1024];
			using (MemoryStream ms = new MemoryStream())
			{
				int read;
				while ((read = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
				{
					await ms.WriteAsync(buffer, 0, read);
				}
				return ms.ToArray();
			}
		}

		private async Task<byte[]> Decompress(byte[] originalBytes)
		{
			using GZipStream stream = new GZipStream(new MemoryStream(originalBytes), CompressionMode.Decompress);
			return await ReadAsync(stream);
		}
	}
}
