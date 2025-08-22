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
using PX.Objects.CS;
using System;

namespace PX.Objects.CR.Extensions.CRCreateActions
{
	/// <exclude/>
	public abstract class CRCreateLeadAction<TGraph, TMain> : CRCreateActionBaseInit<TGraph, TMain>
		where TGraph : PXGraph, new()
		where TMain : class, IBqlTable, new()
	{
		public virtual TMain GetCurrentMain(params object[] pars)
		{
			return (TMain)Base.Caches<TMain>().Current;
		}

		#region Actions

		public PXAction<TMain> CreateLead;
		[PXUIField(DisplayName = Messages.AddNewLead, FieldClass = FeaturesSet.customerModule.FieldClass, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual void createLead()
		{
			if (Base.IsDirty)
				Base.Actions.PressSave();

			var document = Documents.Current;
			var contact = Contacts.SelectSingle();
			var address = Addresses.SelectSingle();

			if (document == null || contact == null || address == null) return;

			var graph = PXGraph.CreateInstance<LeadMaint>();

			var newLead = graph.Lead.Insert();

			newLead.RefContactID = document.ContactID < 0 ? null : document.ContactID;
			newLead.BAccountID = document.BAccountID;

			newLead.OverrideSalesTerritory = document.OverrideSalesTerritory;
			if (newLead.OverrideSalesTerritory is true)
			{
				newLead.SalesTerritoryID = document.SalesTerritoryID;
			}
			
			MapContact(contact, newLead);
			MapConsentable(contact, newLead);

			CRLeadClass cls = PXSelect<
					CRLeadClass,
				Where<
					CRLeadClass.classID, Equal<Current<CRLead.classID>>>>
				.SelectSingleBound(Base, new object[] { newLead });
			if (cls?.DefaultOwner == CRDefaultOwnerAttribute.Source)
			{
				newLead.WorkgroupID = document.WorkgroupID;
				newLead.OwnerID = document.OwnerID;
			}

			Contact parentContact = Contact.PK.Find(Base, newLead.RefContactID);

			if (parentContact != null)
			{
				CRContactClass contactClass = Contact.FK.Class.FindParent(Base, parentContact);

				if (contactClass != null)
				{
					newLead.ClassID = contactClass.TargetLeadClassID;
				}
			}

			UDFHelper.CopyAttributes(Base.Caches<TMain>(), GetCurrentMain(new object[] { newLead.RefContactID }), graph.Lead.Cache, graph.Lead.Current, newLead.ClassID);
			graph.Lead.Update(newLead);

			var newAddress = graph.AddressCurrent.SelectSingle()
				?? throw new InvalidOperationException("Cannot get Address for Lead."); // just to ensure

			MapAddress(address, newAddress);

			graph.AddressCurrent.Cache.Update(newAddress);

			if (!Base.IsContractBasedAPI)
				PXRedirectHelper.TryRedirect(graph, PXRedirectHelper.WindowMode.NewWindow);

			graph.Save.Press();
		}

		#endregion
	}
}
