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

namespace PX.Objects.FS
{
    public class CloseRouteProcess : PXGraph<CloseRouteProcess>
    {
        public CloseRouteProcess()
        {
            RouteDocumentMaint graphRouteDocumentMaint = CreateInstance<RouteDocumentMaint>();
            RouteClosingMaint graphRouteClosingMaint = CreateInstance<RouteClosingMaint>();

            RouteDocs.SetProcessDelegate(
                delegate(CloseRouteProcess processor, FSRouteDocument fsRouteDocumentRow)
                {
                    processor.Clear();
                    graphRouteDocumentMaint.Clear();
                    graphRouteClosingMaint.Clear();

                    processor.CloseRoute(graphRouteDocumentMaint, graphRouteClosingMaint, fsRouteDocumentRow);
                });
        }

        #region DACFilter
        [Serializable]
        public partial class RouteFilter : PXBqlTable, IBqlTable
        {
            #region Date
            public abstract class date : PX.Data.BQL.BqlDateTime.Field<date> { }

            [PXDBDate]
            [PXUIField(DisplayName = "Date")]
            public virtual DateTime? Date { get; set; }
            #endregion
            #region ShowClosedRoutes
            public abstract class showClosedRoutes : PX.Data.BQL.BqlBool.Field<showClosedRoutes> { }

            [PXBool]
            [PXDefault(false)]
            [PXUIField(DisplayName = "Show Closed Routes")]
            public virtual bool? ShowClosedRoutes { get; set; }
            #endregion
        }
        #endregion

        #region Select
        [PXHidden]
        public PXSetup<FSSetup> SetupRecord;

        [PXHidden]
        public PXFilter<RouteFilter> Filter;
        public PXCancel<RouteFilter> Cancel;

        [PXFilterable]
        public PXFilteredProcessingJoin<FSRouteDocument, RouteFilter,
               InnerJoin<FSRoute,
               On<
                   FSRoute.routeID, Equal<FSRouteDocument.routeID>>>,
               Where2<
                   Where<
                       CurrentValue<RouteFilter.date>, IsNull,
                       Or<FSRouteDocument.date, Equal<CurrentValue<RouteFilter.date>>>>,
                   And<
                       Where2<
                           Where<FSRouteDocument.status, Equal<FSRouteDocument.status.Completed>>,
                           Or<
                               Where<
                                   FSRouteDocument.status, Equal<FSRouteDocument.status.Closed>,
                                   And<CurrentValue<RouteFilter.showClosedRoutes>, Equal<True>>>>>>>,
               OrderBy<
                   Asc<FSRouteDocument.refNbr>>> RouteDocs;
        #endregion

        #region Actions
        public PXAction<RouteFilter> openRoute;
        [PXUIField(MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        public virtual void OpenRoute()
        {
            if (RouteDocs.Current.Status == ID.Status_Route.COMPLETED || RouteDocs.Current.Status == ID.Status_Route.CLOSED) 
            {
                RouteClosingMaint graphRouteClosingMaint = PXGraph.CreateInstance<RouteClosingMaint>();

                graphRouteClosingMaint.RouteDocumentSelected.Current = RouteDocs.Current;

                throw new PXRedirectRequiredException(graphRouteClosingMaint, null) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Try to close a set of routes.
        /// </summary>
        /// <param name="graphRouteDocumentMaint"> Route Document graph.</param>
        /// <param name="graphRouteClosingMaint"> Route Closing Document graph.</param>
        /// <param name="fsRouteDocumentRow">FSRouteDocument row to be processed.</param>
        public virtual void CloseRoute(RouteDocumentMaint graphRouteDocumentMaint, RouteClosingMaint graphRouteClosingMaint, FSRouteDocument fsRouteDocumentRow)
        {
            if (fsRouteDocumentRow.Status != ID.Status_Route.CLOSED)
            {
                graphRouteDocumentMaint.RouteRecords.Current = graphRouteDocumentMaint.RouteRecords.Search<FSRouteDocument.refNbr>(fsRouteDocumentRow.RefNbr);
                graphRouteClosingMaint.RouteDocumentSelected.Current = graphRouteDocumentMaint.RouteRecords.Current;
                graphRouteClosingMaint.AutomaticallyCloseRoute = true;
                graphRouteClosingMaint.closeRoute.PressButton();
                fsRouteDocumentRow.Status = graphRouteClosingMaint.RouteDocumentSelected.Current.Status;
            }
        }
        #endregion

        #region Events
        protected virtual void _(Events.RowSelected<FSRouteDocument> e)
        {
            if (e.Row == null)
            {
                return;
            }

            FSRouteDocument fsRouteDocumentRow = (FSRouteDocument)e.Row;

            if (fsRouteDocumentRow.Status == ID.Status_Route.CLOSED)
            {
                PXUIFieldAttribute.SetEnabled<FSRouteDocument.selected>(e.Cache, fsRouteDocumentRow, false);
            }
        }
        #endregion
    }
}
