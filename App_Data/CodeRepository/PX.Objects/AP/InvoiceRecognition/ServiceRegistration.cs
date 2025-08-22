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

using Autofac;
using PX.Objects.AP.InvoiceRecognition.Feedback;
using PX.Objects.AP.InvoiceRecognition.VendorSearch;

namespace PX.Objects.AP.InvoiceRecognition
{
	internal class ServiceRegistration : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<InvoiceRecognitionService>()
                .As<IInvoiceRecognitionService>()
                .PreserveExistingDefaults();

            builder
                .RegisterType<ContactRepository>()
                .As<IContactRepository>()
                .SingleInstance();

            builder
                .RegisterType<VendorRepository>()
                .As<IVendorRepository>()
                .SingleInstance();

            builder
                .RegisterType<VendorSearchFeedbackBuilder>()
                .AsSelf();

            builder
                .RegisterType<VendorSearcher>()
                .As<IVendorSearchService>();

			builder
				.RegisterType<RecognizedRecordDetailsManager>()
				.AsSelf();
        }
    }
}
