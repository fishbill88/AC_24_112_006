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
using PX.Objects.CS;
using PX.Objects.TX;
using PX.TaxProvider;

namespace PX.Objects.CA
{
	public class CAReleaseProcessExternalTax : PXGraphExtension<CAReleaseProcess>
	{
		public static bool IsActive()
		{
			return PXAccess.FeatureInstalled<FeaturesSet.avalaraTax>();
		}

		protected Func<PXGraph, string, ITaxProvider> TaxProviderFactory;

		public override void Initialize()
		{
			TaxProviderFactory = ExternalTax.TaxProviderFactory;
		}

		public virtual bool IsExternalTax(string taxZoneID)
		{
			return ExternalTax.IsExternalTax(Base, taxZoneID);
		}

		protected Lazy<CATranEntry> LazyCaTranEntry =
			new Lazy<CATranEntry>(() => PXGraph.CreateInstance<CATranEntry>());

		[PXOverride]
		public virtual void OnBeforeRelease(CAAdj doc)
		{
			if (doc == null || doc.IsTaxValid == true || !IsExternalTax(doc.TaxZoneID))
			{
				return;
			}

			CATranEntry graph = LazyCaTranEntry.Value;
			graph.Clear();

			graph.CalculateExternalTax(doc);
		}

		[PXOverride]
		public virtual CAAdj CommitExternalTax(CAAdj doc)
		{
			if (doc?.IsTaxValid == true && doc.NonTaxable != true && IsExternalTax(doc.TaxZoneID) && doc.IsTaxPosted != true)
			{
				if (TaxPluginMaint.IsActive(Base, doc.TaxZoneID))
				{
					var service = ExternalTax.TaxProviderFactory(Base, doc.TaxZoneID);

					CATranEntry ie = PXGraph.CreateInstance<CATranEntry>();
					ie.CAAdjRecords.Current = doc;
					CATranEntryExternalTax ieExt = ie.GetExtension<CATranEntryExternalTax>();
					CommitTaxRequest request = ieExt.BuildCommitTaxRequest(doc);

					CommitTaxResult result = service.CommitTax(request);
					if (result.IsSuccess)
					{
						doc.IsTaxPosted = true;
					}
				}
			}

			return doc;
		}
	}
}
