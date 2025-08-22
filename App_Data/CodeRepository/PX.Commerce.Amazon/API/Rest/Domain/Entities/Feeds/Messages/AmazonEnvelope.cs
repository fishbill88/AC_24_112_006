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

using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace PX.Commerce.Amazon.API.Rest
{
    [XmlType("AmazonEnvelope")]
    public class AmazonEnvelope<TMessage>
        where TMessage : IMessage
    {
        private const string XSI = "xsi";
        private const string XMLSCHEMA = "http://www.w3.org/2001/XMLSchema-instance";
        private const string NONAMESPACESCHEMALOCATION = "noNamespaceSchemaLocation";
        private const string AMZNENVELOPE = "amzn-envelope.xsd";

        public Header Header { get; set; }
        public string MessageType { get; set; }
        public TMessage Message { get; set; }
        public string MessageBody { get; set; }

        public AmazonEnvelope(string messageType, Header header, string messageBody)
        {
            this.MessageType = messageType;
            this.Header = header;
            this.MessageBody = messageBody;
        }

        private AmazonEnvelope()
        {

        }

        /// <summary>
        /// Convert Amazon Envelope from object to xml string
        /// </summary>
        /// <param name="amazonEnvelope"></param>
        /// <returns></returns>
        public string FormAmazonEnvelopeXmlStr()
        {
            var nms = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(this.GetType());
            nms.Add(XSI, XMLSCHEMA);
            nms.Add(NONAMESPACESCHEMALOCATION, AMZNENVELOPE);

            XmlWriterSettings xmlWritterSettings = new XmlWriterSettings()
            {
                Encoding = Encoding.UTF8,
                ConformanceLevel = ConformanceLevel.Document,
                CloseOutput = true,
                Indent = true,
                IndentChars = "  ",
                NewLineHandling = NewLineHandling.Replace
            };

            xmlWritterSettings.OmitXmlDeclaration = false;

            using (var tempStream = new MemoryStream())
            {
                using (var stream = new StreamWriter(tempStream, new UTF8Encoding(false)))
                using (var writer = XmlWriter.Create(stream, xmlWritterSettings))
                {
                    serializer.Serialize(writer, this, nms);
                }

                return Encoding.UTF8.GetString(tempStream.ToArray());
            }
        }
    }
}
