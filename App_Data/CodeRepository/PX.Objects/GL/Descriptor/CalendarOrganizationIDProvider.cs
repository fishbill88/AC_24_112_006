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
using System.Collections.Generic;
using System.Linq;
using PX.Data;
using PX.Objects.Common.Extensions;
using PX.Objects.Common.GraphExtensions.Abstract;
using PX.Objects.GL.FinPeriods.TableDefinition;

namespace PX.Objects.GL.Descriptor
{
    public interface ICalendarOrganizationIDProvider :
        IPeriodKeyProvider<
            PeriodKeyProviderBase.SourcesSpecificationCollection,
            PeriodKeyProviderBase.SourceSpecificationItem,
            CalendarOrganizationIDProvider.PeriodKeyWithSourceValuesCollection,
            CalendarOrganizationIDProvider.KeyWithSourceValues,
            FinPeriod.Key>
    {
        int? GetCalendarOrganizationID(PXGraph graph, PXCache attributeCache, object extRow);

        int? GetCalendarOrganizationID(
            PXGraph graph,
            object[] pars,
            bool takeBranchForSelectorFromQueryParams,
            bool takeOrganizationForSelectorFromQueryParams);

        List<int?> GetDetailOrganizationIDs(PXGraph graph);
    }

    public class CalendarOrganizationIDProvider :
        PeriodKeyProviderBase<
            PeriodKeyProviderBase.SourcesSpecificationCollection,
            PeriodKeyProviderBase.SourceSpecificationItem,
            CalendarOrganizationIDProvider.PeriodKeyWithSourceValuesCollection,
            CalendarOrganizationIDProvider.KeyWithSourceValues,
            FinPeriod.Key>,
        ICalendarOrganizationIDProvider
    {
        #region Types

        public class KeyWithSourceValues : KeyWithSourceValues<SourceSpecificationItem, FinPeriod.Key>
        {

        }

        public class PeriodKeyWithSourceValuesCollection : KeyWithSourceValuesCollection<KeyWithSourceValues, SourceSpecificationItem, FinPeriod.Key>
        {

        }

        #endregion

        public override int? MasterValue => FinPeriod.organizationID.MasterValue;

        public CalendarOrganizationIDProvider(
            SourcesSpecificationCollection sourcesSpecification = null,
            Type[] sourceSpecificationTypes = null,
            Type useMasterCalendarSourceType = null,
            bool useMasterOrganizationIDByDefault = false)
            : base(
                sourcesSpecification: sourcesSpecification,
                sourceSpecificationTypes: sourceSpecificationTypes,
                useMasterCalendarSourceType: useMasterCalendarSourceType,
                useMasterOrganizationIDByDefault: useMasterOrganizationIDByDefault)
        {

        }

        public int? GetCalendarOrganizationID(PXGraph graph, PXCache attributeCache, object extRow)
        {
            return GetKey(graph, attributeCache, extRow).OrganizationID;
        }

        public override FinPeriod.Key GetKey(PXGraph graph, PXCache attributeCache, object extRow)
        {
            FinPeriod.Key key = base.GetKey(graph, attributeCache, extRow);

            if (key.OrganizationID == null
                && (UseMasterOrganizationIDByDefault || UseMasterCalendarSourceType != null))
            {
                key.OrganizationID = MasterValue;
            }

            return key;
        }

        public virtual int? GetCalendarOrganizationID(
            PXGraph graph,
            object[] pars,
            bool takeBranchForSelectorFromQueryParams,
            bool takeOrganizationForSelectorFromQueryParams)
        {
            KeyWithSourceValues keyWithSourceValuesItem = EvaluateRawKey(graph,
                new KeyWithSourceValues()
                {
                    SourceBranchIDs = takeBranchForSelectorFromQueryParams
						? ((int?) pars[0]).SingleToList()
                        : null,
                    SourceOrganizationIDs = takeOrganizationForSelectorFromQueryParams
						? ((int?) pars[takeBranchForSelectorFromQueryParams ? 1 : 0]).SingleToList()
                        : null
                });

			return IsIDsUndefined(keyWithSourceValuesItem.KeyOrganizationIDs) && UseMasterOrganizationIDByDefault
				? MasterValue
				: keyWithSourceValuesItem.KeyOrganizationIDs.FirstOrDefault();
        }

        public virtual List<int?> GetDetailOrganizationIDs(PXGraph graph)
        {

            IDocumentWithFinDetailsGraphExtension documentGraphExtension = graph.FindImplementation<IDocumentWithFinDetailsGraphExtension>();

            if (documentGraphExtension == null)
            {
                return new List<int?>();
            }

            return documentGraphExtension.GetOrganizationIDsInDetails();
        }

        protected override KeyWithSourceValues EvaluateRawKey(PXGraph graph, KeyWithSourceValues keyWithSourceValues)
        {
            if (keyWithSourceValues == null)
                return null;

            keyWithSourceValues.KeyOrganizationIDs = keyWithSourceValues.SourceOrganizationIDs;

            if (IsIDsUndefined(keyWithSourceValues.KeyOrganizationIDs))
            {
                if (keyWithSourceValues.SourceBranchIDs != null)
                {
                    keyWithSourceValues.KeyOrganizationIDs =
                        keyWithSourceValues.SourceBranchIDs
                            .Select(branchID => PXAccess.GetParentOrganizationID(branchID))
                            .ToList();
                }
            }

            keyWithSourceValues.Key.OrganizationID = keyWithSourceValues.KeyOrganizationIDs.First();

            return keyWithSourceValues;
        }
    }
}
