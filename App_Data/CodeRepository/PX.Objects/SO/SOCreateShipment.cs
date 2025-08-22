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
using System.Collections;
using System.Collections.Generic;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.AR.MigrationMode;
using PX.Objects.CA;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.SO.GraphExtensions.SOOrderEntryExt;

namespace PX.Objects.SO
{
	[PX.Objects.GL.TableAndChartDashboardType]
	public class SOCreateShipment : PXGraph<SOCreateShipment>
	{
		public PXCancel<SOOrderFilter> Cancel;
		public PXAction<SOOrderFilter> viewDocument;

		public PXFilter<SOOrderFilter> Filter;
		[PXFilterable]
		public PXFilteredProcessing<SOOrder, SOOrderFilter> Orders;


		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXEditDetailButton]
		public virtual IEnumerable ViewDocument(PXAdapter adapter)
		{
			if (Orders.Current != null)
			{
				SOOrderEntry docgraph = PXGraph.CreateInstance<SOOrderEntry>();
				docgraph.Document.Current = docgraph.Document.Search<SOOrder.orderNbr>(Orders.Current.OrderNbr, Orders.Current.OrderType);
				throw new PXRedirectRequiredException(docgraph, true, "Order") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
			}
			return adapter.Get();
		}

		public PXSelect<INSite> INSites;
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName = Messages.SiteDescr, Visibility = PXUIVisibility.SelectorVisible)]
		protected virtual void INSite_Descr_CacheAttached(PXCache sender) { }

		public PXSelect<Carrier> Carriers;
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		[PXUIField(DisplayName = Messages.CarrierDescr, Visibility = PXUIVisibility.SelectorVisible)]
		protected virtual void Carrier_Description_CacheAttached(PXCache sender) { }

		[PXInt]
		public virtual void SOOrder_EmployeeID_CacheAttached(PXCache sender) { }

		[PXDecimal]
		public virtual void SOOrder_CuryDocBal_CacheAttached(PXCache sender) { }

		public SOCreateShipment()
		{
			ARSetupNoMigrationMode.EnsureMigrationModeDisabled(this);

			Orders.SetSelected<SOOrder.selected>();
		}

		public virtual void SOOrderFilter_Action_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			var filter = (SOOrderFilter)e.Row;
			if (filter.Action.IsIn(WellKnownActions.SOOrderScreen.CreateChildOrders, WellKnownActions.SOOrderScreen.ProcessExpiredOrder))
			{
				sender.SetValueExt<SOOrderFilter.dateSel>(filter, SOOrderFilter.dateSel.OrderDate);
				sender.SetValueExt<SOOrderFilter.carrierPluginID>(filter, null);
				sender.SetValueExt<SOOrderFilter.shipVia>(filter, null);
			}
		}

		public virtual void SOOrderFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			var filter = (SOOrderFilter)e.Row;
			if (filter == null) return;
			bool isCreateShipment = filter.Action == WellKnownActions.SOOrderScreen.CreateShipment;
			PXUIFieldAttribute.SetVisible<SOOrderFilter.shipmentDate>(sender, null, isCreateShipment);
			PXUIFieldAttribute.SetVisible<SOOrderFilter.siteID>(sender, null, isCreateShipment);
			PXUIFieldAttribute.SetVisible<SOOrder.shipSeparately>(Orders.Cache, null, isCreateShipment);
			bool isCreateChildOrders = filter.Action == WellKnownActions.SOOrderScreen.CreateChildOrders;
			bool isBlanketAction = isCreateChildOrders || filter.Action == WellKnownActions.SOOrderScreen.ProcessExpiredOrder;
			PXUIFieldAttribute.SetVisible<SOOrderFilter.schedOrderDate>(sender, null, isCreateChildOrders);
			PXUIFieldAttribute.SetEnabled<SOOrderFilter.schedOrderDate>(sender, null, isCreateChildOrders);
			PXUIFieldAttribute.SetEnabled<SOOrderFilter.dateSel>(sender, null, !isBlanketAction);
			PXUIFieldAttribute.SetVisible<SOOrderFilter.carrierPluginID>(sender, null, !isBlanketAction);
			PXUIFieldAttribute.SetEnabled<SOOrderFilter.carrierPluginID>(sender, null, !isBlanketAction);
			PXUIFieldAttribute.SetVisible<SOOrderFilter.shipVia>(sender, null, !isBlanketAction);
			PXUIFieldAttribute.SetEnabled<SOOrderFilter.shipVia>(sender, null, !isBlanketAction);
			PXUIFieldAttribute.SetVisible<SOOrder.minSchedOrderDate>(Orders.Cache, null, isBlanketAction);
			PXUIFieldAttribute.SetVisible<SOOrder.requestDate>(Orders.Cache, null, !isBlanketAction);
			PXUIFieldAttribute.SetVisible<SOOrder.shipDate>(Orders.Cache, null, !isBlanketAction);
			PXUIFieldAttribute.SetVisible<SOOrder.expireDate>(Orders.Cache, null, isBlanketAction);
			if (!String.IsNullOrEmpty(filter.Action))
			{
				Orders.SetProcessWorkflowAction(
					filter.Action,
					isCreateShipment ? filter.ShipmentDate
						: isCreateChildOrders ? filter.SchedOrderDate
						: filter.EndDate,
					filter.SiteID);
			}
		}

		protected bool _ActionChanged = false;

		public virtual void SOOrderFilter_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			_ActionChanged = !sender.ObjectsEqual<SOOrderFilter.action>(e.Row, e.OldRow);
		}

		public override int ExecuteUpdate(string viewName, IDictionary keys, IDictionary values, params object[] parameters)
		{
			if (viewName != nameof(Orders))
				return base.ExecuteUpdate(viewName, keys, values, parameters);

			Dictionary<string, object> strippedValues = new Dictionary<string, object>(keys.Count + 1);
			foreach (var key in values.Keys)
			{
				if (keys.Contains(key))
				{
					strippedValues.Add(key.ToString(), values[key]);
				}
				else if (string.Equals(key.ToString(), Orders.Cache.GetField(typeof(SOOrder.selected)), StringComparison.InvariantCultureIgnoreCase))
				{
					strippedValues.Add(key.ToString(), values[key]);
				}
			}

			return base.ExecuteUpdate(viewName, keys, strippedValues, parameters);
		}

		public virtual IEnumerable orders()
		{
			PXUIFieldAttribute.SetDisplayName<SOOrder.customerID>(Caches[typeof(SOOrder)], Messages.CustomerID);

			SOOrderFilter filter = PXCache<SOOrderFilter>.CreateCopy(Filter.Current);
			if (filter.Action == PXAutomationMenuAttribute.Undefined)
				yield break;

			if (_ActionChanged)
				Orders.Cache.Clear();

			PXSelectBase<SOOrder> cmd = GetSelectCommand(filter);

			AddCommonFilters(filter, cmd);

			int startRow = PXView.StartRow;
			int totalRows = 0;

			foreach (PXResult<SOOrder> res in cmd.View.Select(null, null, PXView.Searches, PXView.SortColumns, PXView.Descendings, PXView.Filters, ref startRow, PXView.MaximumRows, ref totalRows))
			{
				SOOrder order = res;
				SOOrder cached;

				if ((cached = (SOOrder)Orders.Cache.Locate(order)) != null)
				{
					order.Selected = cached.Selected;
				}

				yield return order;
			}

			PXView.StartRow = 0;

			Orders.Cache.IsDirty = false;
		}

		protected virtual PXSelectBase<SOOrder> GetSelectCommand(SOOrderFilter filter)
		{
			switch (filter.Action)
			{
				case WellKnownActions.SOOrderScreen.CreateChildOrders:
					return BuildCommandCreateChildOrders();
				case WellKnownActions.SOOrderScreen.CreateShipment:
					return BuildCommandCreateShipment(filter);
				case WellKnownActions.SOOrderScreen.PrepareInvoice:
					return BuildCommandPrepareInvoice();
				case WellKnownActions.SOOrderScreen.ProcessExpiredOrder:
					return BuildCommandProcessExpiredOrder();
				case WellKnownActions.SOOrderScreen.CancelOrder:
					return BuildCommandCancelOrder();
				case WellKnownActions.SOOrderScreen.CompleteOrder:
					return BuildCommandCompleteOrder();
				case WellKnownActions.SOOrderScreen.CreateAndAuthorizePayment:
					return BuildCommandCreateCCPayment();
				case WellKnownActions.SOOrderScreen.CreateAndCapturePayment:
					return BuildCommandCreateCCAndEFTPayment();
				default:
					return BuildCommandDefault();
			}
		}

		protected virtual PXSelectBase<SOOrder> BuildCommandCreateShipment(SOOrderFilter filter)
		{
			PXSelectBase<SOOrder> cmd = new SelectFrom<SOOrder>
				.LeftJoin<Customer>
					.On<SOOrder.FK.Customer>
					.SingleTableOnly
				.LeftJoin<Carrier>
					.On<SOOrder.FK.Carrier>
				.Where<SOOrder.status.IsNotEqual<SOOrderStatus.shipping>
					.And<Customer.bAccountID.IsNull.Or<Match<Customer, Current<AccessInfo.userName>>>>
					.And<Exists<
						SelectFrom<INItemPlanToShip>
							.InnerJoin<INSite>
								.On<INItemPlanToShip.FK.Site>
						.Where<
							INItemPlanToShip.refNoteID.IsEqual<SOOrder.noteID>
							.And<INItemPlanToShip.reverse.IsEqual<False>>
							.And<INItemPlanToShip.inclQtySOBackOrdered.IsEqual<short0>>
							.And<Match<INSite, Current<AccessInfo.userName>>>
							.And<SOOrderFilter.siteID.FromCurrent.IsNull.Or<INItemPlanToShip.siteID.IsEqual<SOOrderFilter.siteID.FromCurrent>>>
							.And<SOOrderFilter.dateSel.FromCurrent.IsNotEqual<SOOrderFilter.dateSel.shipDate>
								.Or<Brackets<SOOrderFilter.startDate.FromCurrent.IsNull.Or<INItemPlanToShip.planDate.IsGreaterEqual<SOOrderFilter.startDate.FromCurrent>>>
									.And<Brackets<SOOrderFilter.endDate.FromCurrent.IsNull.Or<INItemPlanToShip.planDate.IsLessEqual<SOOrderFilter.endDate.FromCurrent>>>>>>>>>>
				.View(this);

			return cmd;
		}

		protected virtual PXSelectBase<SOOrder> BuildCommandPrepareInvoice()
		{
			var cmd =
				new PXSelectJoinGroupBy<SOOrder,
						InnerJoin<SOOrderType, On<SOOrderType.orderType, Equal<SOOrder.orderType>, And<SOOrderType.aRDocType, NotEqual<ARDocType.noUpdate>>>,
						LeftJoin<Carrier, On<SOOrder.shipVia, Equal<Carrier.carrierID>>,
						LeftJoin<SOOrderShipment, On<SOOrderShipment.orderType, Equal<SOOrder.orderType>, And<SOOrderShipment.orderNbr, Equal<SOOrder.orderNbr>>>,
						LeftJoinSingleTable<ARInvoice, On<ARInvoice.docType, Equal<SOOrderShipment.invoiceType>, And<ARInvoice.refNbr, Equal<SOOrderShipment.invoiceNbr>>>,
						LeftJoinSingleTable<Customer, On<SOOrder.customerID, Equal<Customer.bAccountID>>>>>>>,
					Where<SOOrder.hold, Equal<boolFalse>, And<SOOrder.cancelled, Equal<boolFalse>,
						And<Where<Customer.bAccountID, IsNull, Or<Match<Customer, Current<AccessInfo.userName>>>>>>>,
					Aggregate<
						GroupBy<SOOrder.orderType,
						GroupBy<SOOrder.orderNbr,
						GroupBy<SOOrder.approved>>>>>(this);

			if (PXAccess.FeatureInstalled<FeaturesSet.inventory>())
			{
				cmd.WhereAnd<
					Where<Sub<Sub<Sub<SOOrder.shipmentCntr,
																	SOOrder.openShipmentCntr>,
																	SOOrder.billedCntr>,
																	SOOrder.releasedCntr>, Greater<short0>,
						Or2<Where2<Where<SOOrder.orderQty, Equal<decimal0>,
								Or<SOOrder.openLineCntr, Equal<int0>, And<SOOrder.isLegacyMiscBilling, Equal<False>>>>,
							And<Where<SOOrder.curyUnbilledMiscTot, Greater<decimal0>,
								Or<SOOrder.curyUnbilledMiscTot, Equal<decimal0>, And<SOOrder.unbilledOrderQty, Greater<decimal0>>>>>>,
						Or<Where<Not<SOBehavior.RequireShipment<SOOrder.behavior, SOOrderType.requireShipping>>>>>>>();
			}
			else
			{
				cmd.WhereAnd<Where<
					SOOrder.isLegacyMiscBilling, Equal<False>, And2<Where<SOOrder.curyUnbilledMiscTot, Greater<decimal0>,
							Or<SOOrder.curyUnbilledMiscTot, Equal<decimal0>, And<SOOrder.unbilledOrderQty, Greater<decimal0>>>>,
					Or<Where<SOOrder.curyUnbilledMiscTot, Greater<decimal0>, And<SOOrderShipment.shipmentNbr, IsNull,
						Or<Where<ARInvoice.refNbr, IsNull, And<Not<SOBehavior.RequireShipment<SOOrder.behavior, SOOrderType.requireShipping>>>>>>>>>>>();
			}

			return cmd;
		}

		protected virtual PXSelectBase<SOOrder> BuildCommandProcessExpiredOrder()
		{
			var cmd = BuildCommandDefault();

			cmd.WhereAnd<Where<SOOrder.expireDate, Less<Current<AccessInfo.businessDate>>>>();

			return cmd;
		}

		protected virtual PXSelectBase<SOOrder> BuildCommandCancelOrder()
		{
			return new
				SelectFrom<SOOrder>.
				InnerJoin<SOOrderType>.On<SOOrder.FK.OrderType>.
				LeftJoin<Carrier>.On<SOOrder.FK.Carrier>.
				LeftJoin<Customer>.On<SOOrder.FK.Customer>.SingleTableOnly.
				LeftJoin<SOOrderShipment>.On<SOOrderShipment.FK.Order>.
				Where<
					SOOrder.behavior.IsNotEqual<SOBehavior.bL>.
					And<SOOrderShipment.orderNbr.IsNull>.
					And<
						Customer.bAccountID.IsNull.
						Or<Match<Customer, AccessInfo.userName.FromCurrent>>>>.
				View(this);
		}

		protected virtual PXSelectBase<SOOrder> BuildCommandCompleteOrder()
		{
			return new
				SelectFrom<SOOrder>.
				InnerJoin<SOOrderType>.On<SOOrder.FK.OrderType>.
				LeftJoin<Carrier>.On<SOOrder.FK.Carrier>.
				LeftJoin<Customer>.On<SOOrder.FK.Customer>.SingleTableOnly.
				Where<
					Not< /// similar to <see cref="Workflow.SalesOrder.ScreenConfiguration.Conditions.CanNotBeCompleted"/>
						SOOrder.completed.IsEqual<True>.
						Or<SOOrder.shipmentCntr.IsEqual<Zero>>.
						Or<SOOrder.openShipmentCntr.IsGreater<Zero>>>.
					And<
						Customer.bAccountID.IsNull.
						Or<Match<Customer, AccessInfo.userName.FromCurrent>>>>.
				View(this);
		}

		protected virtual PXSelectBase<SOOrder> BuildCommandDefault()
		{
			return new PXSelectJoin<SOOrder,
				LeftJoin<Carrier, On<SOOrder.shipVia, Equal<Carrier.carrierID>>,
				LeftJoinSingleTable<Customer, On<SOOrder.customerID, Equal<Customer.bAccountID>>>>,
				Where<Customer.bAccountID, IsNull, Or<Match<Customer, Current<AccessInfo.userName>>>>>(this);
		}

		protected virtual PXSelectBase<SOOrder> BuildCommandCreatePayment()
		{
			var cmd = BuildCommandDefault();

			cmd.Join<InnerJoin<SOOrderType,
				On<SOOrder.FK.OrderType>>>();

			cmd.Join<InnerJoin<PaymentMethod, On<PaymentMethod.paymentMethodID, Equal<SOOrder.paymentMethodID>>,
					 InnerJoin<CustomerPaymentMethod, On<CustomerPaymentMethod.pMInstanceID, Equal<SOOrder.pMInstanceID>>,
					 InnerJoin<CCProcessingCenter, On<CCProcessingCenter.processingCenterID, Equal<CustomerPaymentMethod.cCProcessingCenterID>>>>>>();

			cmd.WhereAnd<Where<SOOrderType.aRDocType, Equal<ARDocType.invoice>,
				And<PaymentMethod.isActive, Equal<True>,
				And<PaymentMethod.useForAR, Equal<True>,
				And<SOOrder.pMInstanceID, IsNotNull,
				And<SOOrder.curyUnpaidBalance, Greater<decimal0>,
				And<CCProcessingCenter.isExternalAuthorizationOnly, NotEqual<True>>>>>>>>();

			return cmd;
		}

		protected virtual PXSelectBase<SOOrder> BuildCommandCreateCCAndEFTPayment()
		{
			var cmd = BuildCommandCreatePayment();
			cmd.WhereAnd<Where<PaymentMethod.paymentType,
				In3<PaymentMethodType.creditCard, PaymentMethodType.eft>>>();
			return cmd;
		}
		protected virtual PXSelectBase<SOOrder> BuildCommandCreateCCPayment()
		{

			var cmd = BuildCommandCreatePayment();
			cmd.WhereAnd<Where<PaymentMethod.paymentType,
				Equal<PaymentMethodType.creditCard>>>();
			return cmd;
		}

		protected virtual PXSelectBase<SOOrder> BuildCommandCreateChildOrders()
		{
			var cmd = BuildCommandDefault();

			cmd.WhereAnd<
				Where<SOOrder.minSchedOrderDate, LessEqual<Current<SOOrderFilter.schedOrderDate>>>>();

			return cmd;
		}

		protected virtual void AddCommonFilters(SOOrderFilter filter, PXSelectBase<SOOrder> cmd)
		{
			bool isCreateShipmentAction = filter.Action == WellKnownActions.SOOrderScreen.CreateShipment;

			cmd.WhereAnd<Where<PX.SM.WorkflowAction.IsEnabled<SOOrder, SOOrderFilter.action>>>();

			if (filter.EndDate != null)
			{
				switch (filter.DateSel)
				{
					case SOOrderFilter.dateSel.ShipDate:
						if (!isCreateShipmentAction)
							cmd.WhereAnd<Where<SOOrder.shipDate, LessEqual<Current<SOOrderFilter.endDate>>>>();
						break;
					case SOOrderFilter.dateSel.CancelBy:
						cmd.WhereAnd<Where<SOOrder.cancelDate, LessEqual<Current<SOOrderFilter.endDate>>>>();
						break;
					case SOOrderFilter.dateSel.OrderDate:
						cmd.WhereAnd<Where<SOOrder.orderDate, LessEqual<Current<SOOrderFilter.endDate>>>>();
						break;
				}
			}

			if (filter.StartDate != null)
			{
				switch (filter.DateSel)
				{
					case SOOrderFilter.dateSel.ShipDate:
						if (!isCreateShipmentAction)
							cmd.WhereAnd<Where<SOOrder.shipDate, GreaterEqual<Current<SOOrderFilter.startDate>>>>();
						break;
					case SOOrderFilter.dateSel.CancelBy:
						cmd.WhereAnd<Where<SOOrder.cancelDate, GreaterEqual<Current<SOOrderFilter.startDate>>>>();
						break;
					case SOOrderFilter.dateSel.OrderDate:
						cmd.WhereAnd<Where<SOOrder.orderDate, GreaterEqual<Current<SOOrderFilter.startDate>>>>();
						break;
				}
			}

			if (!string.IsNullOrEmpty(filter.CarrierPluginID))
			{
				cmd.WhereAnd<Where<Carrier.carrierPluginID, Equal<Current<SOOrderFilter.carrierPluginID>>>>();
			}

			if (!string.IsNullOrEmpty(filter.ShipVia))
			{
				cmd.WhereAnd<Where<SOOrder.shipVia, Equal<Current<SOOrderFilter.shipVia>>>>();
			}

			if (filter.CustomerID != null)
			{
				cmd.WhereAnd<Where<SOOrder.customerID, Equal<Current<SOOrderFilter.customerID>>>>();
			}
		}

		public class WellKnownActions
		{
			public class SOOrderScreen
			{
				public const string ScreenID = "SO301000";

				public const string CreateChildOrders
					= ScreenID + "$" + nameof(Blanket.createChildOrders);

				public const string CreateShipment
					= ScreenID + "$" + nameof(SOOrderEntry.createShipmentIssue);

				public const string OpenOrder
					= ScreenID + "$" + nameof(SOOrderEntry.openOrder);

				public const string ReleaseFromCreditHold
					= ScreenID + "$" + nameof(SOOrderEntry.releaseFromCreditHold);

				public const string PrepareInvoice
					= ScreenID + "$" + nameof(SOOrderEntry.prepareInvoice);

				public const string CancelOrder
					= ScreenID + "$" + nameof(SOOrderEntry.cancelOrder);

				public const string ProcessExpiredOrder
					= ScreenID + "$" + nameof(Blanket.processExpiredOrder);

				public const string CompleteOrder
					= ScreenID + "$" + nameof(SOOrderEntry.completeOrder);

				public const string CreateAndAuthorizePayment
					= ScreenID + "$" + nameof(CreatePaymentExt.createAndAuthorizePayment);

				public const string CreateAndCapturePayment
					= ScreenID + "$" + nameof(CreatePaymentExt.createAndCapturePayment);
			}
		}
	}

	[Serializable]
	public partial class SOOrderFilter : PXBqlTable, IBqlTable
	{
		#region Action
		[PX.Data.Automation.PXWorkflowMassProcessing(DisplayName = "Action")]
		public virtual string Action { get; set; }
		public abstract class action : PX.Data.BQL.BqlString.Field<action> { }
		#endregion
		#region DateSel
		public abstract class dateSel : PX.Data.BQL.BqlString.Field<dateSel>
		{
			public class ListAttribute : PXStringListAttribute
			{
				public ListAttribute() : base(
					new[]
				{
						Pair(ShipDate, Messages.ShipDate),
						Pair(CancelBy, Messages.CancelBy),
						Pair(OrderDate, Messages.OrderDate),
					})
				{ }
			}

			public const string ShipDate = "S";
			public const string CancelBy = "C";
			public const string OrderDate = "O";

			public class shipDate : PX.Data.BQL.BqlString.Constant<shipDate> { public shipDate() : base(ShipDate) { } }
			public class cancelBy : PX.Data.BQL.BqlString.Constant<cancelBy> { public cancelBy() : base(CancelBy) { } }
			public class orderDate : PX.Data.BQL.BqlString.Constant<orderDate> { public orderDate() : base(OrderDate) { } }
		}
		protected string _DateSel;
		[PXDBString]
		[PXDefault(dateSel.ShipDate)]
		[PXUIField(DisplayName = "Select By")]
		[dateSel.List]
		public virtual string DateSel
		{
			get
			{
				return this._DateSel;
			}
			set
			{
				this._DateSel = value;
			}
		}
		#endregion
		#region StartDate
		public abstract class startDate : PX.Data.BQL.BqlDateTime.Field<startDate> { }
		protected DateTime? _StartDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Start Date", Visibility = PXUIVisibility.SelectorVisible, Required = false)]
		[PXDefault()]
		public virtual DateTime? StartDate
		{
			get
			{
				return this._StartDate;
			}
			set
			{
				this._StartDate = value;
			}
		}
		#endregion
		#region SchedOrderDate
		public abstract class schedOrderDate : BqlDateTime.Field<schedOrderDate> { }
		[PXDBDate]
		[PXUIField(DisplayName = "Sched. Order Date")]
		[PXDefault(typeof(AccessInfo.businessDate))]
		public virtual DateTime? SchedOrderDate
		{
			get;
			set;
		}
		#endregion
		#region EndDate
		public abstract class endDate : PX.Data.BQL.BqlDateTime.Field<endDate> { }
		protected DateTime? _EndDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "End Date", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(typeof(AccessInfo.businessDate))]
		public virtual DateTime? EndDate
		{
			get
			{
				return this._EndDate;
			}
			set
			{
				this._EndDate = value;
			}
		}
		#endregion
		#region SiteID
		public abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID> { }
		protected Int32? _SiteID;
		[IN.Site(DisplayName = "Warehouse", DescriptionField = typeof(INSite.descr))]
		public virtual Int32? SiteID
		{
			get
			{
				return this._SiteID;
			}
			set
			{
				this._SiteID = value;
			}
		}
		#endregion
		#region CarrierPluginID
		public abstract class carrierPluginID : PX.Data.BQL.BqlString.Field<carrierPluginID> { }
		protected String _CarrierPluginID;
		[PXDBString(15, IsUnicode = true, InputMask = ">aaaaaaaaaaaaaaa")]
		[PXUIField(DisplayName = "Carrier", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<CarrierPlugin.carrierPluginID>))]
		public virtual String CarrierPluginID
		{
			get
			{
				return this._CarrierPluginID;
			}
			set
			{
				this._CarrierPluginID = value;
			}
		}
		#endregion
		#region ShipVia
		public abstract class shipVia : PX.Data.BQL.BqlString.Field<shipVia> { }
		protected String _ShipVia;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Ship Via")]
		[PXSelector(typeof(Search<Carrier.carrierID>), DescriptionField = typeof(Carrier.description), CacheGlobal = true)]
		public virtual String ShipVia
		{
			get
			{
				return this._ShipVia;
			}
			set
			{
				this._ShipVia = value;
			}
		}
		#endregion
		#region CustomerID
		public abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }
		protected int? _CustomerID;
		[PXUIField(DisplayName = "Customer")]
		[Customer(DescriptionField = typeof(Customer.acctName))]
		public virtual int? CustomerID
		{
			get
			{
				return _CustomerID;
			}
			set
			{
				_CustomerID = value;
			}
		}
		#endregion
		#region ShipmentDate
		public abstract class shipmentDate : PX.Data.BQL.BqlDateTime.Field<shipmentDate> { }
		protected DateTime? _ShipmentDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Shipment Date", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(typeof(AccessInfo.businessDate))]
		public virtual DateTime? ShipmentDate
		{
			get
			{
				return this._ShipmentDate;
			}
			set
			{
				this._ShipmentDate = value;
			}
		}
		#endregion
	}
}
