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
using PX.Data;
using PX.Objects.CS;
using PX.Objects.AR;
using PX.Objects.IN;
using PX.Objects.CR;
using PX.Objects.PM;

namespace PX.Objects.CT
{
	public class ContractLineNbr : PXLineNbrAttribute
	{
		public ContractLineNbr(Type sourceType)
			: base(sourceType)
		{
		}

		protected HashSet<object> _justInserted = null;

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);
			_justInserted = new HashSet<object>();
		}

		public override void RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			base.RowInserted(sender, e);
			_justInserted.Add(e.Row);
		}

		public override void RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			//the idea is not to decrement linecntr on deletion
			if (_justInserted.Contains(e.Row))
			{
				base.RowDeleted(sender, e);
				_justInserted.Remove(e.Row);
			}
		}
	}

	/// <summary>
	/// Contract Selector. Dispalys all contracts.
	/// </summary>
	[PXDBInt()]
	[PXUIField(DisplayName = "Contract", Visibility = PXUIVisibility.Visible)]
	public class ContractAttribute : PXEntityAttribute
	{
		public const string DimensionName = "CONTRACT";

		public class dimension : PX.Data.BQL.BqlString.Constant<dimension>
		{
			public dimension() : base(DimensionName) {; }
		}

		public ContractAttribute()
		{
			PXDimensionSelectorAttribute select = new PXDimensionSelectorAttribute(
				DimensionName,
				typeof(Search2<Contract.contractID,
					InnerJoin<ContractBillingSchedule,
						On<ContractBillingSchedule.contractID, Equal<Contract.contractID>>,
					LeftJoin<BAccountR,
						On<BAccountR.bAccountID, Equal<Contract.customerID>>>>,
					Where<Contract.baseType, Equal<CTPRType.contract>>>)
				, typeof(Contract.contractCD)
				, typeof(Contract.contractCD), typeof(Contract.customerID), typeof(Contract.locationID), typeof(Contract.description), typeof(Contract.status), typeof(Contract.expireDate), typeof(ContractBillingSchedule.lastDate), typeof(ContractBillingSchedule.nextDate));

			select.DescriptionField = typeof(Contract.description);
			_Attributes.Add(select);
			_SelAttrIndex = _Attributes.Count - 1;
		}

		public ContractAttribute(Type WhereType)
		{
			Type SearchType =
				BqlCommand.Compose(
				typeof(Search2<,,,>),
				typeof(Contract.contractID),
				typeof(InnerJoin<ContractBillingSchedule,
							On<ContractBillingSchedule.contractID, Equal<Contract.contractID>>,
					   LeftJoin<BAccountR,
							On<BAccountR.bAccountID, Equal<Contract.customerID>>>>),
				typeof(Where<,,>),
				typeof(Contract.baseType),
				typeof(Equal<>),
				typeof(CTPRType.contract),
				typeof(And<>),
				WhereType,
				typeof(OrderBy<Desc<Contract.contractCD>>));

			PXDimensionSelectorAttribute select = new PXDimensionSelectorAttribute(DimensionName, SearchType, typeof(Contract.contractCD),
				typeof(Contract.contractCD), typeof(Contract.customerID), typeof(Contract.locationID), typeof(Contract.description), typeof(Contract.status), typeof(Contract.expireDate), typeof(ContractBillingSchedule.lastDate), typeof(ContractBillingSchedule.nextDate));

			select.DescriptionField = typeof(Contract.description);
			_Attributes.Add(select);
			_SelAttrIndex = _Attributes.Count - 1;
		}
	}

	[PXDBInt()]
	[PXUIField(DisplayName = "Contract", Visibility = PXUIVisibility.Visible)]
	[PXRestrictor(typeof(Where<Contract.baseType, NotEqual<CT.CTPRType.projectTemplate>,
		And<Contract.baseType, NotEqual<CT.CTPRType.contractTemplate>>>), PX.Objects.CT.Messages.TemplateContract, typeof(Contract.contractCD))]
	[PXRestrictor(typeof(Where<Contract.baseType, Equal<CT.CTPRType.contract>, Or<Contract.nonProject, Equal<True>>>), PX.Objects.CT.Messages.NotAContract, typeof(Contract.contractCD))]
	[PXRestrictor(typeof(Where<Contract.isCancelled, Equal<False>>), PX.Objects.CT.Messages.CancelledContract, typeof(Contract.contractCD))]
	[PXRestrictor(typeof(Where<Contract.isCompleted, Equal<False>>), PX.Objects.CT.Messages.CompleteContract, typeof(Contract.contractCD))]
	[PXRestrictor(typeof(Where<Contract.isActive, Equal<True>, Or<Contract.nonProject, Equal<True>>>), PX.Objects.CT.Messages.InactiveContract, typeof(Contract.contractCD))]
	public class ActiveContractBaseAttribute : PXEntityAttribute, IPXFieldVerifyingSubscriber
	{
		protected Type customerField;

		public ActiveContractBaseAttribute() : this(null) { }

		public ActiveContractBaseAttribute(Type customerField)
		{
			this.customerField = customerField;

			PXDimensionSelectorAttribute select = new PXDimensionSelectorAttribute(ProjectAttribute.DimensionName,
				typeof(Search2<Contract.contractID,
					LeftJoin<Customer, On<Customer.bAccountID, Equal<Contract.customerID>>>,
					Where<Contract.nonProject, Equal<True>, Or<Match<Current<AccessInfo.userName>>>>>)
					, typeof(Contract.contractCD)
					, typeof(Contract.contractCD)
					, typeof(Contract.description)
					, typeof(Contract.status)
					, typeof(Contract.customerID)
					, typeof(Customer.acctName)
					, typeof(Contract.curyID));
			
			select.DescriptionField = typeof(Contract.description);
			select.ValidComboRequired = true;
			select.CacheGlobal = true;

			_Attributes.Add(select);
			_SelAttrIndex = _Attributes.Count - 1;

			Filterable = true;
		}

		public virtual void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			Contract project = PXSelect<Contract>.Search<Contract.contractID>(sender.Graph, e.NewValue);
			if (customerField != null && project != null && project.NonProject != true)
			{
				int? customerID = (int?)sender.GetValue(e.Row, customerField.Name);

				if (customerID != project.CustomerID)
				{
					sender.RaiseExceptionHandling(FieldName, e.Row, e.NewValue,
						new PXSetPropertyException(Warnings.SelectedProjectCustomerDontMatchTheDocument, PXErrorLevel.Warning));
				}
			}
		}
	}

	/// <summary>
	/// Contract Template Selector. Displays all Contract Templates.
	/// </summary>
	[PXDBInt()]
	[PXUIField(DisplayName = "Contract Template", Visibility = PXUIVisibility.Visible)]
	public class ContractTemplateAttribute : PXEntityAttribute
	{
		public const string DimensionName = "TMCONTRACT";

		public class dimension : PX.Data.BQL.BqlString.Constant<dimension>
		{
			public dimension() : base(DimensionName) {; }
		}

		public ContractTemplateAttribute()
		{
			PXDimensionSelectorAttribute select = new PXDimensionSelectorAttribute(
				DimensionName,
				typeof(Search<ContractTemplate.contractID,
				Where<ContractTemplate.baseType, Equal<CTPRType.contractTemplate>>>)
				, typeof(ContractTemplate.contractCD)
				, typeof(ContractTemplate.contractCD), typeof(ContractTemplate.description), typeof(ContractTemplate.status));

			select.DescriptionField = typeof(ContractTemplate.description);
			_Attributes.Add(select);
			_SelAttrIndex = _Attributes.Count - 1;
		}

		public ContractTemplateAttribute(Type WhereType)
		{
			Type SearchType =
				BqlCommand.Compose(
				typeof(Search<,>),
				typeof(ContractTemplate.contractID),
				typeof(Where<,,>),
				typeof(ContractTemplate.baseType),
				typeof(Equal<>),
				typeof(CTPRType.contractTemplate),
				typeof(And<>),
				WhereType);

			PXDimensionSelectorAttribute select = new PXDimensionSelectorAttribute(DimensionName, SearchType, typeof(ContractTemplate.contractCD),
				typeof(ContractTemplate.contractCD), typeof(ContractTemplate.description), typeof(ContractTemplate.status));

			select.DescriptionField = typeof(ContractTemplate.description);
			_Attributes.Add(select);
			_SelAttrIndex = _Attributes.Count - 1;
		}
	}

	/// <summary>
	/// Contract Item Selector. Displays all Contract Items.
	/// </summary>
	[PXDBString(InputMask = "", IsUnicode = true)]
	[PXUIField(DisplayName = "Contract Item", Visibility = PXUIVisibility.Visible)]
	public class ContractItemAttribute : PXEntityAttribute
	{
		public const string DimensionName = "CONTRACTITEM";

		public ContractItemAttribute()
		{
			PXDimensionSelectorAttribute select = new PXDimensionSelectorAttribute(
				DimensionName,
				typeof(Search<ContractItem.contractItemCD>)
				, typeof(ContractItem.contractItemCD)
				, typeof(ContractItem.contractItemCD), typeof(ContractItem.descr), typeof(ContractItem.baseItemID));

			select.DescriptionField = typeof(ContractItem.descr);
			_Attributes.Add(select);
			_SelAttrIndex = _Attributes.Count - 1;
		}

		public ContractItemAttribute(Type WhereType)
		{
			Type SearchType =
				BqlCommand.Compose(
				typeof(Search<,,>),
				typeof(ContractItem.contractItemCD),
				WhereType,
				typeof(OrderBy<Desc<ContractItem.contractItemCD>>));

			PXDimensionSelectorAttribute select = new PXDimensionSelectorAttribute(DimensionName, SearchType,
				typeof(ContractItem.contractItemCD)
				, typeof(ContractItem.contractItemCD), typeof(ContractItem.descr), typeof(ContractItem.baseItemID));

			select.DescriptionField = typeof(ContractItem.descr);
			_Attributes.Add(select);
			_SelAttrIndex = _Attributes.Count - 1;
		}
	}


	[PXDBInt()]
	[PXUIField(DisplayName = "Contract Item")]
	[PXRestrictor(typeof(Where<InventoryItem.stkItem, NotEqual<True>>), Messages.ContractInventoryItemCantBeStock, typeof(InventoryItem.inventoryCD))]
	[PXRestrictor(typeof(Where<InventoryItem.itemStatus, NotEqual<InventoryItemStatus.unknown>>), Messages.ContractInventoryItemCantBeUnknown, typeof(InventoryItem.inventoryCD))]
	[PXRestrictor(typeof(Where<InventoryItem.isTemplate, Equal<False>>), IN.Messages.InventoryItemIsATemplate)]
	public class ContractInventoryItemAttribute : PXEntityAttribute
	{
		public const string DimensionName = "INVENTORY";

		public ContractInventoryItemAttribute()
		{
			Type SearchType =
				BqlCommand.Compose(
				typeof(Search5<,,>),
				typeof(InventoryItem.inventoryID),
				typeof(LeftJoin<ARSalesPrice, On<ARSalesPrice.inventoryID, Equal<InventoryItem.inventoryID>,
					And<ARSalesPrice.uOM, Equal<InventoryItem.baseUnit>,
					And<ARSalesPrice.siteID, IsNull,
					And<ARSalesPrice.curyID, Equal<Current<ContractItem.curyID>>,
					And<ARSalesPrice.priceType, Equal<PriceTypes.basePrice>,
					And<ARSalesPrice.breakQty, Equal<decimal0>,
					And<ARSalesPrice.isPromotionalPrice, Equal<False>,
					And<ARSalesPrice.isFairValue, Equal<False>>>>>>>>>>),
				typeof(Aggregate<GroupBy<InventoryItem.inventoryID,
					GroupBy<InventoryItem.stkItem>>>));

			PXDimensionSelectorAttribute select = new PXDimensionSelectorAttribute(
			   DimensionName,
			   SearchType,
			   typeof(InventoryItem.inventoryCD)
			   );

			select.DescriptionField = typeof(InventoryItem.descr);
			_Attributes.Add(select);
			_SelAttrIndex = _Attributes.Count - 1;
		}
	}

	public class ContractDetailAccumAttribute : PXAccumulatorAttribute
	{
		public ContractDetailAccumAttribute()
		{
			base._SingleRecord = true;
		}
		protected override bool PrepareInsert(PXCache sender, object row, PXAccumulatorCollection columns)
		{
			if (!base.PrepareInsert(sender, row, columns))
			{
				return false;
			}

			columns.UpdateOnly = true;

			ContractDetailAcum item = (ContractDetailAcum)row;
			columns.Update<ContractDetailAcum.used>(item.Used, PXDataFieldAssign.AssignBehavior.Summarize);
			columns.Update<ContractDetailAcum.usedTotal>(item.UsedTotal, PXDataFieldAssign.AssignBehavior.Summarize);

			//columns.Update<ContractDetailAcum.contractID>(item.ContractID, PXDataFieldAssign.AssignBehavior.Initialize);
			//columns.Update<ContractDetailAcum.inventoryID>(item.InventoryID, PXDataFieldAssign.AssignBehavior.Initialize);

			return true;
		}
	}

	public static class GroupType
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Contract },
				new string[] { Messages.AttributeEntity_Contract })
			{ }
		}
		public const string Contract = "CONTRACT";

		public class ContractType : PX.Data.BQL.BqlString.Constant<ContractType>
		{
			public ContractType() : base(GroupType.Contract) { }
		}

	}
}
