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

namespace PX.Objects.Common.Interfaces
{
	public interface IAlternateSelectable
	{
		string AlternateID { get; set; }
		string AlternateType { get; set; }
		string AlternateDescr { get; set; }

		string BarCode { get; set; }
		string BarCodeType { get; set; }
		string BarCodeDescr { get; set; }

		string InventoryAlternateID { get; set; }
		string InventoryAlternateType { get; set; }
		string InventoryAlternateDescr { get; set; }

		int? InventoryID { get; set; }
	}
}
