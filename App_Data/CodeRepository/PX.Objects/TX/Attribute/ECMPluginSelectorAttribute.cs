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
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using PX.Objects.CA;

namespace PX.Objects.TX
{
	public class ECMPluginSelectorAttribute : PXCustomSelectorAttribute 
	{
		private static Type[] _interfaces = { typeof(TaxProvider.IExemptionCertificateProvider) };

		[PXHidden]
		public class ECMPlugin : PXBqlTable, IBqlTable
		{
			#region TaxPluginID
			public abstract class taxPluginID : PX.Data.BQL.BqlString.Field<taxPluginID> { }
			[PXDBString(15, IsUnicode = true)]
			[PXUIField(DisplayName = "Provider ID ")]
			public virtual string TaxPluginID { get; set; }
			#endregion
		}

		public ECMPluginSelectorAttribute()
			: base(typeof(ECMPlugin.taxPluginID))
		{}

		protected IEnumerable GetRecords()
		{
			PXProviderTypeSelectorAttribute.ProviderRec[] eCMPlugins = PXProviderTypeSelectorAttribute.GetProviderRecs(_interfaces).ToArray();

			var taxPlugins = PXSelect<TaxPlugin>.Select(_Graph);
			List<ECMPlugin> list = new List<ECMPlugin>();

			if (taxPlugins.Count > 0)
			{
				foreach (TaxPlugin row in taxPlugins)
				{
					PXProviderTypeSelectorAttribute.ProviderRec provider = eCMPlugins?.
							FirstOrDefault(plugin => plugin.TypeName.Trim() == row.PluginTypeName.Trim());
					if (provider != null)
						list.Add(new ECMPlugin { TaxPluginID = row.TaxPluginID });
				}
			}
			foreach (ECMPlugin plugin in list)
				yield return plugin;
		}
	}
}
