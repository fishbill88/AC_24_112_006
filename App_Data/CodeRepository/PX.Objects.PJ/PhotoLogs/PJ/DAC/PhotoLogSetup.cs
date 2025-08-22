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
using PX.Objects.PJ.PhotoLogs.PJ.Graphs;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.CN.Common.DAC;
using PX.Objects.CS;

namespace PX.Objects.PJ.PhotoLogs.PJ.DAC
{
    [Serializable]
    [PXPrimaryGraph(typeof(PhotoLogSetupMaint))]
    [PXCacheName("Photo Log Preferences")]
    public class PhotoLogSetup : BaseCache, IBqlTable
    {
        [PXDBString(10, IsUnicode = true)]
        [PXDefault("PHOTOLOG")]
        [PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
        [PXUIField(DisplayName = "Photo Log Numbering Sequence")]
        public string PhotoLogNumberingId
        {
            get;
            set;
        }

        [PXDBString(10, IsUnicode = true)]
        [PXDefault("PHOTO")]
        [PXSelector(typeof(Numbering.numberingID), DescriptionField = typeof(Numbering.descr))]
        [PXUIField(DisplayName = "Photo Numbering Sequence")]
        public string PhotoNumberingId
        {
            get;
            set;
        }

        public abstract class photoLogNumberingId : BqlString.Field<photoLogNumberingId>
        {
        }

        public abstract class photoNumberingId : BqlString.Field<photoNumberingId>
        {
        }
    }
}