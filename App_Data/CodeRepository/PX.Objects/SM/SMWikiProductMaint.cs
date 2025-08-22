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
using System;
using PX.SM;
using PX.Data.Search;

namespace PX.Objects.SM
{
	[Serializable]
	public class SPWikiProductMaint : PXGraph<SPWikiProductMaint>
	{
        public SPWikiProductMaint()
        {
            Wiki.BlockIfOnlineHelpIsOn();
        }

        #region Select
		public PXSelect<SPWikiProduct> WikiProduct;
		public PXSelect<SPWikiProductTags, Where<SPWikiProductTags.productID, Equal<Current<SPWikiProduct.productID>>>> WikiProductDetails;
		public SPWikiCategoryMaint.PXSelectWikiFoldersTree Folders;
		#endregion

		#region Actions
		public PXSave<SPWikiProduct> Save;
		public PXCancel<SPWikiProduct> Cancel;
		public PXInsert<SPWikiProduct> Insert;
		public PXDelete<SPWikiProduct> Delete;
		public PXFirst<SPWikiProduct> First;
		public PXPrevious<SPWikiProduct> Previous;
		public PXNext<SPWikiProduct> Next;
		public PXLast<SPWikiProduct> Last;
		#endregion

		#region EventHandler
		protected virtual void SPWikiProductTags_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
		{
			SPWikiProductTags row = e.Row as SPWikiProductTags;
			if (row != null)
			{
				row.ProductID = WikiProduct.Current.ProductID;

				if (row == null || row.PageName == null)
					return;

				PXResult<WikiPage, WikiPageLanguage> currentrow = (PXResult<WikiPage, WikiPageLanguage>)PXSelectJoin<WikiPage,
					LeftJoin<WikiPageLanguage, On<WikiPage.pageID, Equal<WikiPageLanguage.pageID>>>,
					Where<WikiPage.name, Equal<Required<WikiPage.name>>>>.SelectWindowed(this, 0, 1, row.PageName);

				if (currentrow != null)
				{
					WikiPage wp = currentrow[typeof(WikiPage)] as WikiPage;
					WikiPageLanguage wpl = currentrow[typeof(WikiPageLanguage)] as WikiPageLanguage;

					if (wp != null)
					{
						row.PageName = wp.Name;
						row.PageID = wp.PageID;
					}

					if (wpl != null)
					{
						row.PageTitle = wpl.Title;
					}
					else
					{
						row.PageTitle = row.PageName;
					}
				}
			}
		}

		protected virtual void SPWikiProductTags_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			SPWikiProductTags row = e.Row as SPWikiProductTags;
			if (row == null || row.PageName == null)
				return;

			PXResult<WikiPage, WikiPageLanguage> currentrow = (PXResult<WikiPage, WikiPageLanguage>)PXSelectJoin<WikiPage,
					LeftJoin<WikiPageLanguage, On<WikiPage.pageID, Equal<WikiPageLanguage.pageID>>>,
					Where<WikiPage.name, Equal<Required<WikiPage.name>>>>.SelectWindowed(this, 0, 1, row.PageName);

			if (currentrow != null)
			{
				WikiPage wp = currentrow[typeof(WikiPage)] as WikiPage;
				WikiPageLanguage wpl = currentrow[typeof(WikiPageLanguage)] as WikiPageLanguage;

				if (wp != null)
				{
					row.PageName = wp.Name;
					row.PageID = wp.PageID;
				}

				if (wpl != null)
				{
					row.PageTitle = wpl.Title;
				}
				else
				{
					row.PageTitle = row.PageName;
				}
			}
		}
		#endregion
	}
}

