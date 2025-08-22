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

using PX.Data;
using PX.Data.WorkflowAPI;
using PX.Objects.CS;

namespace PX.Objects.AR
{
	using static PX.Data.WorkflowAPI.BoundedTo<CustomerMaint, Customer>;

	public class CustomerMaintECMWorkflowExt : PXGraphExtension<CustomerMaint_Workflow, CustomerMaint>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<CS.FeaturesSet.eCM>();
		}

		public sealed override void Configure(PXScreenConfiguration config)
		{
			Configure(config.GetScreenConfigurationContext<CustomerMaint, Customer>());
		}

		protected static void Configure(WorkflowContext<CustomerMaint, Customer> context)
		{
			var ecmCategory = context.Categories.CreateNew(ECMCategoryID.ECM,
				category => category.DisplayName(ECMCategoryNames.ECM));

			#region Conditions
			Condition Bql<T>() where T : IBqlUnary, new() => context.Conditions.FromBql<T>();
			var conditions = new
			{
				HasCreatedInECM
					= Bql<Customer.eCMCompanyCode.IsNotNull.And<Customer.eCMCompanyCode.IsNotEqual<Empty>>>(),
				IsECMValid
					= Bql<Customer.isECMValid.IsEqual<True>>(),
				IsECMOff
					= PXAccess.FeatureInstalled<FeaturesSet.eCM>() ? Bql<True.IsEqual<False>>() : Bql<True.IsEqual<True>>(),
			}.AutoNameConditions();
			#endregion

			context.UpdateScreenConfigurationFor(screen =>
			{
				return screen
					.WithActions(actions =>
					{
						actions.Add<CustomerMaintExternalECMExt>(g => g.retrieveCertificate, a => a
										.WithCategory(ecmCategory)
										.IsDisabledWhen(!conditions.HasCreatedInECM)
										.IsHiddenWhen(conditions.IsECMOff));
						actions.Add<CustomerMaintExternalECMExt>(g => g.requestCertificate, a => a
										.WithCategory(ecmCategory)
										.IsDisabledWhen(!conditions.HasCreatedInECM)
										.IsHiddenWhen(conditions.IsECMOff));
						actions.Add<CustomerMaintExternalECMExt>(g => g.createCustomerInECM, a => a
										.WithCategory(ecmCategory)
										.IsHiddenWhen(conditions.IsECMOff));
						actions.Add<CustomerMaintExternalECMExt>(g => g.updateCustomerInECM, a => a
										.WithCategory(ecmCategory)
										.IsDisabledWhen(!conditions.HasCreatedInECM || conditions.IsECMValid)
										.IsHiddenWhen(conditions.IsECMOff));
					}).WithCategories(categories => categories.Add(ecmCategory));
			});
		}

		public static class ECMCategoryNames
		{
			public const string ECM = "Exemption Certificates";
		}

		public static class ECMCategoryID
		{
			public const string ECM = "ECMID";
		}
	}
}
