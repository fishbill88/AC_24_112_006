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
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using PX.BarcodeProcessing;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.IN;
using PX.Objects.IN.WMS;

namespace PX.Objects.SO.WMS.Worksheet
{
	using WMSBase = WarehouseManagementSystem<PickPackShip, PickPackShip.Host>;

	public class PaperlessOnlyPacking : PaperlessPicking.ScanExtension
	{
		public static bool IsActive() => IsActiveBase();
		public bool IsPackOnly => !Basis.HasPick && Basis.HasPack;

		/// Overrides <see cref="BarcodeDrivenStateMachine{TSelf, TGraph}.CreateScanModes"/>
		[PXOverride]
		public IEnumerable<ScanMode<PickPackShip>> CreateScanModes(Func<IEnumerable<ScanMode<PickPackShip>>> base_CreateScanModes)
		{
			foreach (var mode in base_CreateScanModes())
				yield return mode;

			yield return new PaperlessPackOnlyMode();
		}

		public sealed class PaperlessPackOnlyMode : PickPackShip.ScanMode
		{
			public const string Value = "PPAO";
			public class value : BqlString.Constant<value> { public value() : base(PaperlessPackOnlyMode.Value) { } }

			public override string Code => Value;
			public override string Description => Msg.DisplayName;

			protected override bool IsModeActive() => Basis.Get<PaperlessOnlyPacking>().IsPackOnly;

			#region State Machine
			protected override IEnumerable<ScanState<PickPackShip>> CreateStates()
			{
				yield return new PaperlessPicking.PickListState();
				yield return new WMSBase.LocationState();
				yield return new PickPackShip.PackMode.BoxState();
				yield return new WMSBase.InventoryItemState() { AlternateType = INPrimaryAlternateType.CPN, IsForIssue = true };
				yield return new WMSBase.LotSerialState();
				yield return new WMSBase.ExpireDateState() { IsForIssue = true };
				yield return new PickPackShip.PackMode.ConfirmState();
				yield return new PickPackShip.CommandOrShipmentOnlyState();

				yield return new PickPackShip.PackMode.BoxConfirming.StartState();
				yield return new PickPackShip.PackMode.BoxConfirming.WeightState();
				yield return new PickPackShip.PackMode.BoxConfirming.DimensionsState();
				yield return new PickPackShip.PackMode.BoxConfirming.CompleteState();

				// directly set states
				yield return new PaperlessPicking.WarehouseState();
				yield return new PaperlessPicking.NearestLocationState();
			}

			protected override IEnumerable<ScanTransition<PickPackShip>> CreateTransitions()
			{
				var itemFlow = StateFlow(flow => flow
					.ForkBy(basis => basis.Remove != true)
					.PositiveBranch(pfl => pfl
						.From<PaperlessPicking.PickListState>()
						.NextTo<PickPackShip.PackMode.BoxState>()
						.NextTo<WMSBase.LocationState>()
						.NextTo<WMSBase.InventoryItemState>()
						.NextTo<WMSBase.LotSerialState>()
						.NextTo<WMSBase.ExpireDateState>())
					.NegativeBranch(nfl => nfl
						.From<PaperlessPicking.PickListState>()
						.NextTo<WMSBase.InventoryItemState>()
						.NextTo<WMSBase.LotSerialState>()
						.NextTo<WMSBase.LocationState>()));

				var boxFlow = StateFlow(flow => flow
					.From<PickPackShip.PackMode.BoxConfirming.StartState>()
					.NextTo<PickPackShip.PackMode.BoxConfirming.WeightState>()
					.NextTo<PickPackShip.PackMode.BoxConfirming.DimensionsState>()
					.NextTo<PickPackShip.PackMode.BoxConfirming.CompleteState>());

				return itemFlow.Concat(boxFlow);
			}

			protected override IEnumerable<ScanCommand<PickPackShip>> CreateCommands()
			{
				yield return new PickPackShip.PackMode.RemoveCommand();
				yield return new WMSBase.QtySupport.SetQtyCommand();
				yield return new PickPackShip.PackMode.ConfirmPackageCommand();
				yield return new ConfirmPackListCommand();
				yield return new PaperlessPicking.TakeNextPickListCommand();
				yield return new ConfirmPackListAndTakeNextCommand();
				yield return new PaperlessPicking.ConfirmLineQtyCommand();
			}

			protected override IEnumerable<ScanQuestion<PickPackShip>> CreateQuestions()
			{
				yield return new PickPackShip.PackMode.BoxConfirming.WeightState.SkipQuestion();
				yield return new PickPackShip.PackMode.BoxConfirming.WeightState.SkipScalesQuestion();
				yield return new PickPackShip.PackMode.BoxConfirming.DimensionsState.SkipQuestion();
			}

			protected override IEnumerable<ScanRedirect<PickPackShip>> CreateRedirects() => AllWMSRedirects.CreateFor<PickPackShip>();

			protected override void ResetMode(bool fullReset)
			{
				base.ResetMode(fullReset);
				Clear<PaperlessPicking.PickListState>(when: fullReset && !Basis.IsWithinReset);
				Clear<WMSBase.LocationState>(when: fullReset);
				Clear<PickPackShip.PackMode.BoxState>(when: fullReset);
				Clear<WMSBase.InventoryItemState>(when: fullReset);
				Clear<WMSBase.LotSerialState>();
				Clear<WMSBase.ExpireDateState>();

				Clear<PickPackShip.PackMode.BoxConfirming.WeightState>();
				Clear<PickPackShip.PackMode.BoxConfirming.DimensionsState>();

				if (fullReset)
					Get<PickPackShip.PackMode.Logic>().PackageLineNbrUI = null;
			}
			#endregion

			#region Commands
			public sealed class ConfirmPackListCommand : PickPackShip.ScanCommand
			{
				public override string Code => "CONFIRM*PACK";
				public override string ButtonName => "scanConfirmPackList";
				public override string DisplayName => Msg.DisplayName;
				protected override bool IsEnabled => Basis.DocumentIsEditable;

				protected override bool Process()
				{
					Basis.Get<Logic>().ConfirmPackList();
					return true;
				}

				#region Logic
				[PXProtectedAccess]
				public abstract class Logic : PickPackShip.ScanExtension<PickPackShip.ConfirmShipmentCommand.Logic>
				{
					public static bool IsActive() => PaperlessOnlyPacking.IsActive();

					public virtual void ConfirmPackList()
					{
						if (!CanConfirm(false))
							return;

						var packLogic = Basis.Get<PickPackShip.PackMode.Logic>();
						if (packLogic.SelectedPackage?.Confirmed == false)
							if (Basis.Get<PickPackShip.PackMode.BoxConfirming.CompleteState.Logic>().TryAutoConfirm() == false)
								return;

						int? packageLineNbr = packLogic.PackageLineNbr;
						Basis.Reset(fullReset: false);
						Basis.Clear<PickPackShip.InventoryItemState>();
						packLogic.PackageLineNbr = packageLineNbr;

						packLogic.HasSingleAutoPackage(Basis.RefNbr, out SOPackageDetailEx autoPackageToConfirm);

						Basis.SaveChanges();

						Basis
						.AwaitFor<SOShipment>((basis, doc, ct) => ConfirmSinglePickListHandler(doc.ShipmentNbr, autoPackageToConfirm, ct))
						.WithDescription(Msg.InProcess, Basis.RefNbr)
						.ActualizeDataBy((basis, doc) => SOShipment.PK.Find(basis, doc))
						.OnSuccess(ConfigureOnSuccessAction)
						.OnFail(x => x.Say(Msg.Fail))
						.BeginAwait(Basis.Shipment);
					}

					public virtual void ConfigureOnSuccessAction(ScanLongRunAwaiter<PickPackShip, SOShipment>.IResultProcessor onSuccess)
					{
						onSuccess
							.Say(Msg.Success)
							.ChangeStateTo<WorksheetPicking.PickListState>();
					}

					[Obsolete("Use the " + nameof(ConfirmSinglePickListHandler) + " method instead.")]
					protected static Task ConfirmShipmentHandler(string shipmentNbr, SOPickPackShipSetup setup, SOPickPackShipUserSetup userSetup, SOPackageDetailEx autoPackageToConfirm, CancellationToken cancellationToken)
						=> ConfirmSinglePickListHandler(shipmentNbr, autoPackageToConfirm, cancellationToken);

					protected static async Task ConfirmSinglePickListHandler(string shipmentNbr, SOPackageDetailEx autoPackageToConfirm, CancellationToken cancellationToken)
						{
							await PickPackShip.WithSuppressedRedirects(async () =>
							{
								var wsGraph = PXGraph.CreateInstance<SOPickingWorksheetReview>();
								var (sheet, pickList, pickingJob) =
									SelectFrom<SOPickingWorksheet>.
									InnerJoin<SOPicker>.On<SOPicker.FK.Worksheet>.
									InnerJoin<SOPickingJob>.On<SOPickingJob.FK.Picker>.
									Where<SOPickingWorksheet.singleShipmentNbr.IsEqual<@P.AsString>>.
									View.Select(wsGraph, shipmentNbr)
									.AsEnumerable().Cast<PXResult<SOPickingWorksheet, SOPicker, SOPickingJob>>().Single();

							using (var pickListTransaction = new PXTransactionScope())
								{
									wsGraph.PickListConfirmation.ConfirmPickList(pickList, sortingLocationID: null);
									await wsGraph.PickListConfirmation.FulfillShipmentsAndConfirmWorksheet(sheet, cancellationToken);
								pickListTransaction.Complete();
								}

								if (pickingJob.AutomaticShipmentConfirmation == true)
										await PXGraph
									.CreateInstance<PickPackShip.Host>()
									.GetExtension<WorksheetPicking>()
									.TryConfirmShipmentRightAfterPickList(shipmentNbr, autoPackageToConfirm, cancellationToken);
							});
					}

					/// Uses <see cref="PickPackShip.ConfirmShipmentCommand.Logic.CanConfirm(bool)"/>
					[PXProtectedAccess(typeof(PickPackShip.ConfirmShipmentCommand.Logic))]
					protected abstract bool CanConfirm(bool confirmAsIs);
				}
				#endregion

				#region Messages
				[PXLocalizable]
				public abstract class Msg : WorksheetPicking.ConfirmPickListCommand.Msg
				{
					public new const string DisplayName = "Confirm Pack List";
				}
				#endregion
			}

			public sealed class ConfirmPackListAndTakeNextCommand : PickPackShip.ScanCommand
			{
				public override string Code => "CONFIRM*PACK*AND*NEXT";
				public override string ButtonName => "scanConfirmPackListAndTakeNext";
				public override string DisplayName => PaperlessPicking.ConfirmPickListAndTakeNextCommand.Msg.DisplayName;
				protected override bool IsEnabled => Basis.CurrentMode.Commands.OfType<ConfirmPackListCommand>().First().IsApplicable;

				private bool _inProcess = false;
				protected override bool Process()
				{
					try
					{
						_inProcess = true;
						return Basis.CurrentMode.Commands.OfType<ConfirmPackListCommand>().First().Execute();
					}
					finally
					{
						_inProcess = false;
					}
				}

				/// Overrides <see cref="ConfirmPackListCommand.Logic"/>
				public class AlterConfirmPackListCommandLogic : PickPackShip.ScanExtension<ConfirmPackListCommand.Logic>
				{
					public static bool IsActive() => PaperlessPicking.IsActive();

					/// Overrides <see cref="ConfirmPackListCommand.Logic.ConfigureOnSuccessAction(ScanLongRunAwaiter{PickPackShip, SOShipment}.IResultProcessor)"/>
					[PXOverride]
					public void ConfigureOnSuccessAction(ScanLongRunAwaiter<PickPackShip, SOShipment>.IResultProcessor onSuccess,
						Action<ScanLongRunAwaiter<PickPackShip, SOShipment>.IResultProcessor> base_ConfigureOnSuccessAction)
					{
						base_ConfigureOnSuccessAction(onSuccess);

						if (Basis.CurrentMode.Commands.OfType<ConfirmPackListAndTakeNextCommand>().FirstOrDefault()?._inProcess == true)
							onSuccess.Do((basis, picker) => basis.CurrentMode.Commands.OfType<PaperlessPicking.TakeNextPickListCommand>().First().Execute());
					}
				}
			}
			#endregion

			#region Messages
			[PXLocalizable]
			public new abstract class Msg : PickPackShip.ScanMode.Msg
			{
				public const string DisplayName = "Paperless Pack";
				public const string BoxConfirmOrContinueByLocationPrompt = "Confirm package, or scan the next location.";
			}
			#endregion
		}

		#region Buttons
		[Obsolete, PXInternalUseOnly]
		public PXAction<ScanHeader> ReviewPackLines;
		[PXButton, PXUIField(DisplayName = "Pick Review"), Obsolete, PXInternalUseOnly]
		protected System.Collections.IEnumerable reviewPackLines(PXAdapter adapter) => adapter.Get();
		[Obsolete, PXInternalUseOnly]
		protected void _(Events.RowSelected<ScanHeader> e)
		{
			if (e.Row != null)
				ReviewPackLines.SetVisible(Base.IsMobile && e.Row.Mode == PaperlessPackOnlyMode.Value);
		}
		#endregion

		#region Decorations
		public virtual void InjectPickListHandleAbsenceByPackShipment(PaperlessPicking.PickListState pickList)
		{
			pickList.Intercept.HandleAbsence.ByAppend((basis, barcode) =>
			{
				if (basis.FindMode<PickPackShip.PackMode>() is PickPackShip.PackMode packMode && packMode.IsActive)
				{
					if (packMode.TryProcessBy<PickPackShip.PackMode.ShipmentState>(barcode) == true)
					{
						basis.SetScanMode<PickPackShip.PackMode>();
						basis.FindState<PickPackShip.PackMode.ShipmentState>().Process(barcode);
						return AbsenceHandling.Done;
					}
				}

				return AbsenceHandling.Skipped;
			});
		}

		public virtual void InjectPickListShipmentValidation(PaperlessPicking.PickListState pickListState)
		{
			pickListState.Intercept.Validate.ByAppend((basis, pickList) =>
			{
				var shipment = SOPickingWorksheet.FK.SingleShipment.FindParent(basis, pickList);
				return basis
					.FindMode<PickPackShip.PackMode>()?
					.TryValidate(shipment)
					.By<PickPackShip.PackMode.ShipmentState>()
					?? Validation.Ok;
			});
		}

		public virtual void InjectPickListDispatchToCommandStateOnCantPack(PaperlessPicking.PickListState pickListState)
		{
			pickListState.Intercept.SetNextState.ByReplace(basis =>
			{
				var mode = basis.Get<PickPackShip.PackMode.Logic>();
				if (basis.Remove == true || mode.CanPack || mode.HasConfirmableBoxes)
					basis.DispatchNext();
				else
				{
					basis.ReportInfo(BarcodeProcessing.CommonMessages.SentenceCombiner, basis.Info.Current.Message,
						Basis.Localize(PickPackShip.PackMode.Msg.Completed, basis.RefNbr));
					basis.SetScanState(BuiltinScanStates.Command);
				}
			});
		}

		public virtual void InjectShipmentAbsenceHandlingByWorksheetOfSingleType(PickPackShip.PackMode.ShipmentState packShipment)
		{
			packShipment.Intercept.HandleAbsence.ByPrepend((basis, barcode) =>
			{
				if (barcode.Contains("/") == false)
				{
					if (basis.FindMode<PaperlessPackOnlyMode>() is PaperlessPackOnlyMode paperlessPack && paperlessPack.IsActive)
					{
						if (paperlessPack.TryProcessBy<WorksheetPicking.PickListState>(barcode, StateSubstitutionRule.KeepAbsenceHandling))
						{
							basis.SetScanMode<PaperlessPackOnlyMode>();
							basis.FindState<WorksheetPicking.PickListState>().Process(barcode);
							return AbsenceHandling.Done;
						}
					}
				}

				return AbsenceHandling.Skipped;
			});
		}

		public virtual void InjectItemPromptForPackageConfirmOnPaperlessPack(WMSBase.InventoryItemState itemState)
		{
			itemState.Intercept.StatePrompt.ByOverride((basis, base_StatePrompt) =>
				basis.Get<PickPackShip.PackMode.Logic>().With(mode =>
					basis.Remove != true && mode.CanConfirmPackage
						? PickPackShip.PackMode.Msg.BoxConfirmOrContinuePrompt
						: null)
				?? base_StatePrompt());
		}

		public virtual void InjectLocationPromptForPackageConfirmOnPaperlessPack(WMSBase.LocationState locationState)
		{
			locationState.Intercept.StatePrompt.ByOverride((basis, base_StatePrompt) =>
				basis.Get<PickPackShip.PackMode.Logic>().With(mode =>
					basis.Remove != true && mode.CanConfirmPackage
						? PaperlessPackOnlyMode.Msg.BoxConfirmOrContinueByLocationPrompt
						: null)
				?? base_StatePrompt());
		}

		public virtual void InjectLocationAbsenceHandlingByBox(WMSBase.LocationState locationState)
		{
			locationState.Intercept.HandleAbsence.ByAppend((basis, barcode) =>
				basis.Get<PickPackShip.PackMode.Logic>().TryAutoConfirmCurrentPackageAndLoadNext(barcode) == false
					? AbsenceHandling.Skipped
					: AbsenceHandling.Done);
		}

		public virtual void InjectConfirmCombinedFromPackAndWorksheet(PickPackShip.PackMode.ConfirmState confirmState)
		{
			confirmState.Intercept.PerformConfirmation.ByOverride((basis, base_PerformConfirmation) =>
			{
				basis.Get<PaperlessPicking>().EnsureLocationFromLastVisited();

				FlowStatus worksheetConfirm = basis.Get<WorksheetPicking>().ConfirmSuitableSplits();
				if (worksheetConfirm.IsError != false)
					return worksheetConfirm;

				SOPickerListEntry lastPickedSplit = basis.Graph.Caches<SOPickerListEntry>().Dirty.Cast<SOPickerListEntry>().First();

				FlowStatus packConfirm = base_PerformConfirmation();
				if (packConfirm.IsError != false)
					return packConfirm.WithChangesDiscard;

				basis.Get<PaperlessPicking.ConfirmState.Logic>().VisitSplit(lastPickedSplit);

				return packConfirm;
			});
		}

		public virtual void InjectTakeNextEnablingForPaperlessPackOnly(PaperlessPicking.TakeNextPickListCommand takeNext)
		{
			takeNext.Intercept.IsEnabled.ByDisjoin(basis =>
				basis.CurrentMode is PaperlessPackOnlyMode && (basis.RefNbr == null || basis.DocumentIsConfirmed || basis.Get<WorksheetPicking>().NotStarted));
		}
		#endregion

		#region Overrides
		/// Overrides <see cref="BarcodeDrivenStateMachine{TSelf, TGraph}.DecorateScanMode"/>
		[PXOverride]
		public ScanMode<PickPackShip> DecorateScanMode(ScanMode<PickPackShip> original,
			Func<ScanMode<PickPackShip>, ScanMode<PickPackShip>> base_DecorateScanMode)
		{
			var mode = base_DecorateScanMode(original);

			if (mode is PickPackShip.PackMode pack && IsPackOnly)
				pack.Intercept.CreateCommands.ByAppend(basis => new[] { new PaperlessPicking.TakeNextPickListCommand() });

			return mode;
		}

		/// Overrides <see cref="BarcodeDrivenStateMachine{TSelf, TGraph}.DecorateScanState(ScanState{TSelf})"/>
		[PXOverride]
		public ScanState<PickPackShip> DecorateScanState(ScanState<PickPackShip> original,
			Func<ScanState<PickPackShip>, ScanState<PickPackShip>> base_DecorateScanState)
		{
			var state = base_DecorateScanState(original);

			if (state.ModeCode == PaperlessPackOnlyMode.Value)
			{
				if (state is PaperlessPicking.PickListState pickList)
				{
					InjectPickListShipmentValidation(pickList);
					InjectPickListDispatchToCommandStateOnCantPack(pickList);
					InjectPickListHandleAbsenceByPackShipment(pickList);
					PPBasis.InjectPickListPaperless(pickList);
				}
				else if (state is WMSBase.LocationState locState)
				{
					PPBasis.InjectNavigationOnLocation(locState);
					InjectLocationPromptForPackageConfirmOnPaperlessPack(locState);
				}
				else if (state is WMSBase.InventoryItemState itemState)
				{
					PPBasis.InjectNavigationOnItem(itemState);
					InjectItemPromptForPackageConfirmOnPaperlessPack(itemState);
					Basis.Get<PickPackShip.PackMode.Logic>().InjectItemAbsenceHandlingByBox(itemState);
				}
				else if (state is WMSBase.LotSerialState lsState)
				{
					PPBasis.InjectNavigationOnLotSerial(lsState);
					Basis.InjectLotSerialDeactivationOnDefaultLotSerialOption(lsState, isEntranceAllowed: true);
				}
				else if (state is PickPackShip.PackMode.ConfirmState confirmState)
				{
					InjectConfirmCombinedFromPackAndWorksheet(confirmState);
				}
			}
			else
			{
				if (state is PickPackShip.PackMode.ShipmentState packShipment && IsPackOnly)
				{
					PPBasis.InjectShipmentPromptWithTakeNext(packShipment);
					PPBasis.InjectSuppressShipmentWithWorksheetOfSingleType(packShipment);
					InjectShipmentAbsenceHandlingByWorksheetOfSingleType(packShipment);
				}
			}

			return state;
		}

		/// Overrides <see cref="BarcodeDrivenStateMachine{TSelf, TGraph}.DecorateScanCommand"/>
		[PXOverride]
		public ScanCommand<PickPackShip> DecorateScanCommand(ScanCommand<PickPackShip> original,
			Func<ScanCommand<PickPackShip>, ScanCommand<PickPackShip>> base_DecorateScanCommand)
		{
			var command = base_DecorateScanCommand(original);

			if (command.ModeCode == PaperlessPackOnlyMode.Value)
			{
				if (command is WMSBase.RemoveCommand remove)
					PPBasis.InjectRemoveClearLocationAndInventory(remove);
				else if (command is PaperlessPicking.TakeNextPickListCommand takeNext)
					InjectTakeNextEnablingForPaperlessPackOnly(takeNext);
			}

			return command;
		}

		/// Overrides <see cref="WorksheetPicking.SetPickList(PXResult{SOPickingWorksheet, SOPicker})"/>
		[PXOverride]
		public void SetPickList(PXResult<SOPickingWorksheet, SOPicker> pickList,
			Action<PXResult<SOPickingWorksheet, SOPicker>> base_SetPickList)
		{
			base_SetPickList(pickList);

			if (Basis.CurrentMode is PaperlessPackOnlyMode)
			{
				Basis.RefNbr = pickList?.GetItem<SOPickingWorksheet>()?.SingleShipmentNbr;
				Basis.Graph.Document.Current = SOShipment.PK.Find(Basis, Basis.RefNbr, PKFindOptions.IncludeDirty);
				Basis.NoteID = Basis.Shipment?.NoteID;
			}
		}

		/// Overrides <see cref="WorksheetPicking.CheckAvailability(decimal)"/>
		[Obsolete(Common.Messages.ItemIsObsoleteAndWillBeRemoved2023R2)]
		[PXOverride]
		public FlowStatus CheckAvailability(decimal deltaQty,
			Func<decimal, FlowStatus> base_CheckAvailability)
		{
			return Basis.CurrentMode is PaperlessPackOnlyMode
				? FlowStatus.Ok
				: base_CheckAvailability(deltaQty);
		}

		/// Overrides <see cref="WorksheetPicking.CheckAvailability(decimal, SOPickerListEntry)"/>
		[PXOverride]
		public virtual FlowStatus CheckAvailability(decimal deltaQty, SOPickerListEntry pickedSplit,
			Func<decimal, SOPickerListEntry, FlowStatus> base_CheckAvailability)
		{
			return Basis.CurrentMode is PaperlessPackOnlyMode
				? FlowStatus.Ok
				: base_CheckAvailability(deltaQty, pickedSplit);
		}

		/// Overrides <see cref="WorksheetPicking.ShowWorksheetNbrForMode(string)"/>
		[PXOverride]
		public bool ShowWorksheetNbrForMode(string modeCode,
			Func<string, bool> base_ShowWorksheetNbrForMode)
			=> base_ShowWorksheetNbrForMode(modeCode) && modeCode != PaperlessPackOnlyMode.Value;

		/// Overrides <see cref="WorksheetPicking.FindModeForWorksheet(SOPickingWorksheet)"/>
		[PXOverride]
		public ScanMode<PickPackShip> FindModeForWorksheet(SOPickingWorksheet sheet,
			Func<SOPickingWorksheet, ScanMode<PickPackShip>> base_FindModeForWorksheet)
		{
			if (sheet.WorksheetType == SOPickingWorksheet.worksheetType.Single && IsPackOnly)
				return Basis.FindMode<PaperlessPackOnlyMode>();

			return base_FindModeForWorksheet(sheet);
		}

		/// Overrides <see cref="WorksheetPicking.InjectValidationPickFirst(PickPackShip.ShipmentState)"/>
		[PXOverride]
		public void InjectValidationPickFirst(PickPackShip.ShipmentState refNbrState,
			Action<PickPackShip.ShipmentState> base_InjectValidationPickFirst)
		{
			if (Basis.Get<PaperlessOnlyPacking>().IsPackOnly)
				refNbrState.Intercept.Validate.ByAppend((basis, shipment) =>
					shipment.CurrentWorksheetNbr != null && shipment.Picked == false && SOPickingWorksheet.PK.Find(basis, shipment.CurrentWorksheetNbr).With(w => w.WorksheetType != SOPickingWorksheet.worksheetType.Single)
						? Validation.Fail(PickPackShip.PackMode.ShipmentState.Msg.ShouldBePickedFirst, shipment.ShipmentNbr)
						: Validation.Ok);
			else
				base_InjectValidationPickFirst(refNbrState);
		}

		/// Overrides <see cref="WorksheetPicking.GetEntriesToPick"/>
		[PXOverride]
		public IEnumerable<SOPickerListEntry> GetEntriesToPick(
			Func<IEnumerable<SOPickerListEntry>> base_GetEntriesToPick)
		{
			if (Basis.CurrentMode is PaperlessPackOnlyMode)
			{
				var ppBasis = Basis.Get<PaperlessPicking>();
				return Basis.Remove == true
					? ppBasis.GetSplitsForRemoval()
					: ppBasis.GetWantedSplitsForIncrease();
			}
			else
				return base_GetEntriesToPick();
		}

		/// Overrides <see cref="PickPackShip.DocumentIsConfirmed"/>
		[PXOverride]
		public bool get_DocumentIsConfirmed(Func<bool> base_DocumentIsConfirmed) => Basis.CurrentMode is PaperlessPackOnlyMode
			? Basis.Shipment?.Confirmed == true || WSBasis.PickList?.Confirmed == true
			: base_DocumentIsConfirmed();

		/// Overrides <see cref="PaperlessPicking.EnsureShipmentUserLinkForPaperlessPick"/>
		[PXOverride]
		public void EnsureShipmentUserLinkForPaperlessPick(Action base_EnsureShipmentUserLinkForPaperlessPick)
		{
			if (Basis.CurrentMode is PaperlessPackOnlyMode)
				Graph.WorkLogExt.EnsureFor(
					PPBasis.SingleShipmentNbr,
					Graph.Accessinfo.UserID,
					SOShipmentProcessedByUser.jobType.PackOnly);
			else
				base_EnsureShipmentUserLinkForPaperlessPick();
		}

		/// Overrides <see cref="PaperlessPicking.ReturnCurrentJobToQueue()"/>
		[PXOverride]
		public bool ReturnCurrentJobToQueue(Func<bool> base_ReturnCurrentJobToQueue)
		{
			bool anyChanged = base_ReturnCurrentJobToQueue();

			if (anyChanged && Basis.CurrentMode is PaperlessPackOnlyMode)
				Graph.WorkLogExt.SuspendFor(PPBasis.SingleShipmentNbr, Graph.Accessinfo.UserID, SOShipmentProcessedByUser.jobType.PackOnly);

			return anyChanged;
		}

		/// Overrides <see cref="PickPackShip.UpdateWorkLogOnLogScan"/>
		[PXOverride]
		public void UpdateWorkLogOnLogScan(SOShipmentEntry.WorkLog workLogger, bool isError,
			Action<SOShipmentEntry.WorkLog, bool> base_UpdateWorkLogOnLogScan)
		{
			base_UpdateWorkLogOnLogScan(workLogger, isError);
			if (Basis.CurrentMode is PaperlessPackOnlyMode && !string.IsNullOrEmpty(PPBasis.SingleShipmentNbr))
			{
				workLogger.LogScanFor(
					PPBasis.SingleShipmentNbr,
					Graph.Accessinfo.UserID,
					SOShipmentProcessedByUser.jobType.PackOnly,
					isError);
			}
		}

		/// Overrides <see cref="BarcodeDrivenStateMachine{TSelf, TGraph}.OnBeforeFullClear"/>
		[PXOverride]
		public void OnBeforeFullClear(Action base_OnBeforeFullClear)
		{
			base_OnBeforeFullClear();

			if (Basis.CurrentMode is PaperlessPackOnlyMode && PPBasis.SingleShipmentNbr != null)
				if (Graph.WorkLogExt.SuspendFor(PPBasis.SingleShipmentNbr, Graph.Accessinfo.UserID, SOShipmentProcessedByUser.jobType.PackOnly))
					Graph.WorkLogExt.PersistWorkLog();
		}

		public class AlterTakeNextPickListCommandLogic : PaperlessPicking.ScanExtension<PaperlessPicking.TakeNextPickListCommand.Logic>
		{
			public static bool IsActive() => IsActiveBase();

			/// Overrides <see cref="PaperlessPicking.TakeNextPickListCommand.Logic.TakeNext"/>
			[PXOverride]
			public bool TakeNext(Func<bool> base_TakeNext)
			{
				if (Basis.RefNbr == null && Basis.CurrentMode is PickPackShip.PackMode && Basis.Get<PaperlessOnlyPacking>().IsPackOnly)
					Basis.SetScanMode<PaperlessPackOnlyMode>();

				return base_TakeNext();
			}

			/// Overrides <see cref="PaperlessPicking.TakeNextPickListCommand.Logic.ApplyCommonFilters(PXSelectBase{SOPickingJob})"/>
			[PXOverride]
			public void ApplyCommonFilters(PXSelectBase<SOPickingJob> command,
				Action<PXSelectBase<SOPickingJob>> base_ApplyCommonFilters)
			{
				base_ApplyCommonFilters(command);
				if (Basis.CurrentMode is PickPackShip.PackMode || Basis.CurrentMode is PaperlessPackOnlyMode)
					command.WhereAnd<Where<SOPickingWorksheet.worksheetType.IsEqual<SOPickingWorksheet.worksheetType.single>>>();
			}
		}

		public class AlterPackModeLogic : PickPackShip.ScanExtension<PickPackShip.PackMode.Logic>
		{
			public static bool IsActive() => PaperlessOnlyPacking.IsActive();

			/// Overrides <see cref="PickPackShip.PackMode.Logic.CanPack"/>
			[PXOverride]
			public bool get_CanPack(Func<bool> base_CanPack)
			{
				if (Basis.Get<PaperlessOnlyPacking>().IsPackOnly && Basis.CurrentMode is PaperlessPackOnlyMode)
					return Target.PickedForPack.SelectMain().Any(s => s.PackedQty < s.Qty && RelatedPickListSplitForceCompleted.GetValue(Basis, s) != true);
				else
					return base_CanPack();
			}

			/// Overrides <see cref="PickPackShip.PackMode.Logic.ShowPackTab(ScanHeader)"/>
			[PXOverride]
			public bool ShowPackTab(ScanHeader row,
				Func<ScanHeader, bool> base_ShowPackTab)
			{
				return base_ShowPackTab(row) || Basis.Get<PaperlessOnlyPacking>().IsPackOnly && row.Mode == PaperlessPackOnlyMode.Value;
			}

			public class AlterCommandOrShipmentOnlyStatePrompt : PickPackShip.ScanExtension<PickPackShip.CommandOrShipmentOnlyState.Logic>
			{
				public static bool IsActive() => PaperlessOnlyPacking.IsActive();

				/// Overrides <see cref="PickPackShip.CommandOrShipmentOnlyState.Logic.GetCommandOrShipmentOnlyPrompt"/>
			[PXOverride]
			public string GetCommandOrShipmentOnlyPrompt(Func<string> base_GetCommandOrShipmentOnlyPrompt)
			{
					if (Basis.CurrentMode is PaperlessPackOnlyMode &&
						Basis.Get<PickPackShip.PackMode.Logic>() is PickPackShip.PackMode.Logic mode &&
						mode.CanConfirmPackage)
					return PickPackShip.PackMode.Msg.BoxConfirmPrompt;

				return base_GetCommandOrShipmentOnlyPrompt();
			}
			}

			protected virtual void _(Events.RowSelected<ScanHeader> args)
			{
				if (args.Row?.Mode == PaperlessPackOnlyMode.Value)
					Target.ReviewPack.SetVisible(Base.IsMobile);
			}

			public class AlterConfirmStateLogic : PickPackShip.ScanExtension<PickPackShip.PackMode.ConfirmState.Logic>
			{
				public static bool IsActive() => PaperlessOnlyPacking.IsActive();

				/// Overrides <see cref="PickPackShip.PackMode.ConfirmState.Logic.GetSplitsToPack"/>
				[PXOverride]
				public IEnumerable<SOShipLineSplit> GetSplitsToPack(Func<IEnumerable<SOShipLineSplit>> base_GetSplitsToPack)
				{
					if (Basis.CurrentMode is PaperlessPackOnlyMode)
					{
						/// <see cref="InjectConfirmCombinedFromPackAndWorksheet"/>
						SOPickerListEntry lastPickedSplit = Graph.Caches<SOPickerListEntry>().Dirty.Cast<SOPickerListEntry>().First();

						return Basis.Get<PickPackShip.PackMode.Logic>()
							.PickedForPack.SelectMain()
							.Where(s =>
								s.LocationID == lastPickedSplit.LocationID &&
								s.InventoryID == lastPickedSplit.InventoryID &&
								s.SubItemID == lastPickedSplit.SubItemID &&
								s.LotSerialNbr == lastPickedSplit.LotSerialNbr &&
								(Basis.Remove == true ? s.PackedQty > 0 : Target.TargetQty(s) > s.PackedQty));
					}
					else
					{
						return base_GetSplitsToPack();
					}
				}
			}
		}

		public class AlterPaperlessPickingConfirmLineQtyCommandLogic : PaperlessPicking.ScanExtension<PaperlessPicking.ConfirmLineQtyCommand.Logic>
		{
			public static bool IsActive() => PaperlessOnlyPacking.IsActive();

			/// Overrides <see cref="PaperlessPicking.ConfirmLineQtyCommand.Logic.ReopenQtyOfCurrentSplit"/>
			[PXOverride]
			public bool ReopenQtyOfCurrentSplit(Func<bool> base_ReopenQtyOfCurrentSplit)
			{
				if (Basis.CurrentMode is PaperlessPackOnlyMode)
				{
					var selectedSplit = Basis.Get<PickPackShip.PackMode.Logic>().PickedForPack.Current;
					if (selectedSplit != null)
					{
						var pickListEntry = Basis.Get<RelatedPickListSplitForceCompleted>().GetRelatedPickListEntry(selectedSplit);
						if (pickListEntry != null)
							WSBasis.PickListOfPicker.Current = pickListEntry;
					}
				}

				return base_ReopenQtyOfCurrentSplit();
			}
		}
		#endregion

		#region Attached Fields
		// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
		[PXUIField(DisplayName = RelatedPickListSplitForceCompleted.Msg.FieldDisplayName)]
		public class RelatedPickListSplitForceCompleted : PickPackShip.FieldAttached.To<SOShipLineSplit>.AsBool.Named<RelatedPickListSplitForceCompleted>
		{
			private Dictionary<int, int> splitsToEntries;

			protected override bool? Visible => PaperlessOnlyPacking.IsActive() && Base.WMS.CurrentMode is PaperlessPackOnlyMode;
			public override bool? GetValue(SOShipLineSplit row)
			{
				if (Visible == false || row == null || row.SplitLineNbr == null)
					return null;

				SOPickerListEntry pickListEntry = GetRelatedPickListEntry(row);

				return pickListEntry?.ForceCompleted;
			}

			public virtual SOPickerListEntry GetRelatedPickListEntry(SOShipLineSplit row)
			{
				if (splitsToEntries == null || !splitsToEntries.ContainsKey(row.SplitLineNbr.Value))
				{
					var allShipmentSplits =
						SelectFrom<Table.SOShipLineSplit>.
						Where<Table.SOShipLineSplit.FK.Shipment.SameAsCurrent>.
						OrderBy<
							Table.SOShipLineSplit.locationID.Asc,
							Table.SOShipLineSplit.inventoryID.Asc,
							Table.SOShipLineSplit.subItemID.Asc,
							Table.SOShipLineSplit.lotSerialNbr.Asc,
							Table.SOShipLineSplit.baseQty.Asc,
							Table.SOShipLineSplit.basePickedQty.Asc,
							Table.SOShipLineSplit.splitLineNbr.Asc
						>.
						View.Select(Base).RowCast<Table.SOShipLineSplit>().ToArray();

					var allPickListEntries =
						SelectFrom<SOPickerListEntry>.
						Where<SOPickerListEntry.FK.Picker.SameAsCurrent>.
						OrderBy<
							SOPickerListEntry.locationID.Asc,
							SOPickerListEntry.inventoryID.Asc,
							SOPickerListEntry.subItemID.Asc,
							SOPickerListEntry.lotSerialNbr.Asc,
							SOPickerListEntry.baseQty.Asc,
							SOPickerListEntry.basePickedQty.Asc,
							SOPickerListEntry.entryNbr.Asc
						>.
						View.Select(Base).RowCast<SOPickerListEntry>().ToArray();

					splitsToEntries = allShipmentSplits
						.Zip(allPickListEntries, (s, e) => (SplitKey: s.SplitLineNbr.Value, EntryNbr: e.EntryNbr.Value))
						.ToDictionary(pair => pair.SplitKey, pair => pair.EntryNbr);
				}

				if (splitsToEntries.TryGetValue(row.SplitLineNbr.Value, out int entryNbr))
				return Base.WMS.Get<WorksheetPicking>().PickListOfPicker.Search<SOPickerListEntry.entryNbr>(entryNbr);
				else
					return null;
			}

			[PXLocalizable]
			public abstract class Msg
			{
				public const string FieldDisplayName = "Quantity Confirmed";
			}
		}
		#endregion
	}
}
