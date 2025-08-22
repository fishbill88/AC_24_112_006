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

#nullable enable

using PX.Commerce.Amazon.API.Rest.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PX.Commerce.Amazon.API.Rest
{
	internal class XmlFeedDataProvider : FeedDataProvider, IXmlFeedDataProvider
	{
		XMLFeedData _xmlFeedData;
		const string MessageBody = "MessageBody";
		const string MessagegBodyTag = "<MessageBody>MessageBody</MessageBody>";
		const string IndentChars ="  ";
		public override string FeedContentType => "text/plain; charset=utf-8";

		public XmlFeedDataProvider(IAmazonRestClient restClient, XMLFeedData xmlFeedData) : base(restClient)
		{
			_xmlFeedData = xmlFeedData;
		}

		public async Task<IEnumerable<FeedProcessingResult<TMessage>>> SendFeedAsync<TMessage>(IEnumerable<TMessage> messages) where TMessage : IMessage
		{
			Regex xmlEmptyTagsRemove = new Regex(@"(\s)*<(\w:\w|\w)*(\s)*/>");
			List<FeedProcessingResult<TMessage>> feedProcessingResults = new();
			StringBuilder sbXmlFeedData = new();
			bool anyPendingMessage = true;
			List<TMessage> currentListOfMessages = new();

			double megaBytesLength = 0.0d;

			CreateFeedSpecification feedSpecification = CreateFeedSpecification(_xmlFeedData.FeedType, _xmlFeedData.Marketplace);
			var resultFeed = ConstructEnvelope<TMessage>();

			foreach (TMessage message in messages)
			{
				string msgToXmlString = ConvertObjectToXml(message);

				// calculating size of xml string before appending it
				var byteData = Encoding.UTF8.GetBytes(sbXmlFeedData.ToString() + msgToXmlString);
				megaBytesLength = (byteData.Length / 1024f) / 1024f;

				// if size of the xml string will be larger than the max size allowed then submit it without appending current message
				if (megaBytesLength >= feedSize)
				{
					feedProcessingResults.Add(await SubmitMessages(xmlEmptyTagsRemove, feedSpecification, resultFeed, sbXmlFeedData, currentListOfMessages));
					sbXmlFeedData = new();
					currentListOfMessages = new();
					anyPendingMessage = false;
				}
				else
				{
					anyPendingMessage = true;
				}

				sbXmlFeedData.AppendLine(msgToXmlString);
				currentListOfMessages.Add(message);
			}

			// submit any pending messages that have not yet been submitted
			if (anyPendingMessage)
			{
				feedProcessingResults.Add(await SubmitMessages(xmlEmptyTagsRemove, feedSpecification, resultFeed, sbXmlFeedData, currentListOfMessages));
			}
			return feedProcessingResults;
		}

		private async Task<FeedProcessingResult<TMessage>> SubmitMessages<TMessage>(Regex xmlEmptyTagsRemove, CreateFeedSpecification feedSpecification, string resultFeed, StringBuilder sbXmlFeedData, List<TMessage> currentListOfMessages) where TMessage : IMessage
		{
			string xmlFeedContent = xmlEmptyTagsRemove.Replace(resultFeed.Replace(MessagegBodyTag, sbXmlFeedData.ToString()), string.Empty);
			return await SendFeed(feedSpecification, currentListOfMessages, xmlFeedContent);
		}

		private async Task<FeedProcessingResult<TMessage>> SendFeed<TMessage>(CreateFeedSpecification specs, List<TMessage> currentListOfMessages, string currentXmlFeedContent) where TMessage : IMessage
		{
			XmlProcessingReport currentFeedProcessingResult;

			try
			{
				FeedDocument feedDocument = await SubmitAndProcessFeedAsync(specs, currentXmlFeedContent);
				currentFeedProcessingResult = GetFeedProcessingReport<TMessage>(feedDocument.Url);
			}
			catch (Exception ex)
			{
				currentFeedProcessingResult = new()
				{
					Result = new() {
								new () {
									ResultMessageCode = FeedErrorMessageCode.XmlDocumentLevelError,
									ResultDescription = !string.IsNullOrEmpty(ex.Message) ? ex.Message : ex.GetType() == typeof(ApiException) ? ((ApiException)ex).ResponseMessage : string.Empty
								}
							}
				};
			}

			return new FeedProcessingResult<TMessage>()
			{
				ProcessingReport = currentFeedProcessingResult,
				FeedMessages = currentListOfMessages
			};
		}

		/// <summary>
		/// Returns feed processing result using a pre-signed URL returned in step 5
		/// Note: this URL will be alive within a few minutes only 
		/// </summary>
		/// <typeparam name="TMessage"></typeparam>
		/// <param name="url"></param>
		/// <returns></returns>
		private XmlProcessingReport GetFeedProcessingReport<TMessage>(string url)
			where TMessage : IMessage
		{
			XmlSerializer serializer = new XmlSerializer(typeof(AmazonEnvelope<TMessage>));
			using (XmlTextReader reader = new XmlTextReader(url))
			{
				var envelope = (AmazonEnvelope<TMessage>)serializer.Deserialize(reader);
				return envelope?.Message?.ProcessingReport;
			}
		}

		private string ConvertObjectToXml<TMessage>(TMessage message) where TMessage : IMessage
		{
			var nms = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
			var serializer = new XmlSerializer(message.GetType());

			XmlWriterSettings xmlWritterSettings = new XmlWriterSettings()
			{
				Encoding = Encoding.UTF8,
				ConformanceLevel = ConformanceLevel.Document,
				CloseOutput = true,
				Indent = true,
				IndentChars = IndentChars,
				NewLineHandling = NewLineHandling.Replace
			};

			xmlWritterSettings.OmitXmlDeclaration = true;

			using (var tempStream = new MemoryStream())
			{
				using (var stream = new StreamWriter(tempStream, new UTF8Encoding(false)))
				using (var writer = XmlWriter.Create(stream, xmlWritterSettings))
				{
					serializer.Serialize(writer, message, nms);
				}

				return Encoding.UTF8.GetString(tempStream.ToArray());
			}
		}

		private string ConstructEnvelope<TMessage>() where TMessage : IMessage
		{
			Header objHeader = new Header(_xmlFeedData.SellerId);
			AmazonEnvelope<TMessage> objEnvelope = new AmazonEnvelope<TMessage>(_xmlFeedData.MessageType, objHeader, MessageBody);
			return objEnvelope.FormAmazonEnvelopeXmlStr();
		}
	}
}
