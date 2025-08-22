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
using PX.Objects.PJ.OutlookIntegration.OU.DAC;
using PX.Objects.PJ.Common.Descriptor.Attributes;
using PX.Objects.PJ.ProjectManagement.PJ.DAC;
using PX.Data;

namespace PX.Objects.PJ.OutlookIntegration.OU.Descriptor.Attributes
{
    /// <summary>
    /// Attribute used for resetting response due date on <see cref="RequestForInformationOutlook"/>
    /// and <see cref="ProjectIssueOutlook"/> classes.
    /// Referenced <see cref="ProjectManagementClass.ProjectManagementClassId"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ResetOutlookResponseDueDateAttribute : ResetResponseDueDateAttribute
    {
        public ResetOutlookResponseDueDateAttribute(Type dueDateFieldType, Type createDateFieldType)
            : base(dueDateFieldType, createDateFieldType)
        {
        }

        protected override int? GetDefaultResponseTimeFrame(object row, PXCache cache)
        {
            var projectManagementClass = GetProjectManagementClass(cache, row);
            if (projectManagementClass == null)
            {
                return null;
            }
            switch (row)
            {
                case RequestForInformationOutlook _:
                    return projectManagementClass.RequestForInformationResponseTimeFrame;
                case ProjectIssueOutlook _:
                    return projectManagementClass.ProjectIssueResponseTimeFrame;
                default:
                    return null;
            }
        }
    }
}
