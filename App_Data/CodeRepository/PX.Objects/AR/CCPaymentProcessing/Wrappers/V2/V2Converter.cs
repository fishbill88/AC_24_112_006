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
using PX.Data;
using AutoMapper;
using PX.Objects.AR.CCPaymentProcessing.Common;
using V2 = PX.CCProcessingBase.Interfaces.V2;
using PX.Objects.CC;

namespace PX.Objects.AR.CCPaymentProcessing.Wrappers
{
	public class V2Converter
	{
		private static readonly IMapper Mapper;

		static V2Converter()
		{
			MapperConfiguration mapperConf = new MapperConfiguration(cfg =>
			{
				cfg.CreateMap<CCTranType, V2.CCTranType>()
				.AfterMap((i, v2) => {
					if (!Enum.IsDefined(v2.GetType(), v2))
					{
						throw new PXException(CCProcessingBase.Messages.UnexpectedTranType, i);
					}
				});
				cfg.CreateMap<V2.CCTranType, CCTranType>()
				.AfterMap((v2, o) => {
					if (!Enum.IsDefined(o.GetType(), o))
					{
						throw new PXException(CCProcessingBase.Messages.UnexpectedTranType, v2);
					}
				});
				cfg.CreateMap<V2.SettingsControlType, int?>()
				.ConvertUsing((V2.SettingsControlType i) =>
					convertSettingsControlType(i));
				cfg.CreateMap<PluginSettingDetail, V2.SettingsValue>();
				cfg.CreateMap<V2.SettingsDetail, PluginSettingDetail>()
				.ForMember(dst => dst.Value, opts => opts.MapFrom(src => src.DefaultValue))
				.ForMember(dst => dst.ComboValuesCollection, opts => { opts.AllowNull(); opts.MapFrom(src => src.ComboValues); });
				cfg.CreateMap<V2.HostedFormResponse, HostedFormResponse>();
			});
			Mapper = mapperConf.CreateMapper();
		}

		public static CCTranType ConvertTranType(V2.CCTranType v2TranType)
		{
			return Mapper.Map<CCTranType>(v2TranType);
		}

		public static CCCardType ConvertCardType(V2.CCCardType v2TranType)
		{
			switch (v2TranType)
			{
				case V2.CCCardType.Other:
					return CCCardType.Other;
				case V2.CCCardType.Visa:
					return CCCardType.Visa;
				case V2.CCCardType.MasterCard:
					return CCCardType.MasterCard;
				case V2.CCCardType.AmericanExpress:
					return CCCardType.AmericanExpress;
				case V2.CCCardType.Discover:
					return CCCardType.Discover;
				case V2.CCCardType.JCB:
					return CCCardType.JCB;
				case V2.CCCardType.DinersClub:
					return CCCardType.DinersClub;
				case V2.CCCardType.UnionPay:
					return CCCardType.UnionPay;
				case V2.CCCardType.Debit:
					return CCCardType.Debit;
				case V2.CCCardType.EFT:
					return CCCardType.EFT;
				default:
					return CCCardType.Other;
			}
		}

		public static V2.CCCardType ConvertCardType(CCCardType cardType)
		{
			switch (cardType)
			{
				case CCCardType.Other:
					return V2.CCCardType.Other;
				case CCCardType.Visa:
					return V2.CCCardType.Visa;
				case CCCardType.MasterCard:
					return V2.CCCardType.MasterCard;
				case CCCardType.AmericanExpress:
					return V2.CCCardType.AmericanExpress;
				case CCCardType.Discover:
					return V2.CCCardType.Discover;
				case CCCardType.JCB:
					return V2.CCCardType.JCB;
				case CCCardType.EFT:
					return V2.CCCardType.EFT;
				case CCCardType.DinersClub:
					return V2.CCCardType.DinersClub;
				case CCCardType.UnionPay:
					return V2.CCCardType.UnionPay;
				case CCCardType.Debit:
					return V2.CCCardType.Debit;
				default: return V2.CCCardType.Other;
			}
		}

		public static HostedFormResponse ConvertHostedFormResponse(V2.HostedFormResponse v2FormResponse)
		{
			return Mapper.Map<HostedFormResponse>(v2FormResponse);
		}

		public static V2.CCTranType ConvertTranTypeToV2(CCTranType tranType)
		{
			return Mapper.Map<V2.CCTranType>(tranType);
		}

		public static V2.SettingsValue ConvertSettingDetailToV2(PluginSettingDetail detail)
		{
			return Mapper.Map<V2.SettingsValue>(detail);
		}

		internal static V2.CCTranStatus ConvertTranStatusToV2(CCTranStatus tranStatus)
		{
			return Mapper.Map<V2.CCTranStatus>(tranStatus);
		}

		public static TranProcessingResult ConvertTranProcessingResult(V2.ProcessingResult processingResult)
		{
			if (processingResult == null) throw new ArgumentNullException(nameof(processingResult));

			TranProcessingResult result = new TranProcessingResult()
			{
				AuthorizationNbr = processingResult.AuthorizationNbr,
				CcvVerificatonStatus = ConvertCvvStatus(processingResult.CcvVerificatonStatus),
				ExpireAfterDays = processingResult.ExpireAfterDays,
				Success = true,
				PCResponse = processingResult.ResponseText,
				PCResponseCode = processingResult.ResponseCode,
				PCResponseReasonCode = processingResult.ResponseReasonCode,
				PCResponseReasonText = processingResult.ResponseReasonText,
				PCTranNumber = processingResult.TransactionNumber,
				ResultFlag = CCResultFlag.None,
				TranStatus = CCTranStatus.Approved,
				CardType = V2Converter.ConvertCardType(processingResult.CardTypeCode),
				ProcCenterCardTypeCode = processingResult.CardType,
				Level3Support = processingResult.Level3Support,
				L3Status = GetL3TranStatus(processingResult.Level3Support ? ExtTransactionL3StatusCode.Pending : ExtTransactionL3StatusCode.NotApplicable),
                POSTerminalID = processingResult.POSTerminalID,
            };
			return result;
		}

		public static L3TranStatus GetL3TranStatus(string status)
		{
			switch (status)
			{
				case ExtTransactionL3StatusCode.Pending:
					return L3TranStatus.Pending;
				case ExtTransactionL3StatusCode.Sent:
					return L3TranStatus.Sent;
				case ExtTransactionL3StatusCode.Failed:
					return L3TranStatus.Failed;
				case ExtTransactionL3StatusCode.Rejected:
					return L3TranStatus.Rejected;
				case ExtTransactionL3StatusCode.ResendFailed:
					return L3TranStatus.ResendFailed;
				case ExtTransactionL3StatusCode.ResendRejected:
					return L3TranStatus.ResendRejected;
				case ExtTransactionL3StatusCode.NotApplicable:
				default:
					return L3TranStatus.NotApplicable;
			}
		}

		public static string GetL3Status(L3TranStatus status)
		{
			switch (status)
			{
				case L3TranStatus.Pending:
					return ExtTransactionL3StatusCode.Pending;
				case L3TranStatus.Sent:
					return ExtTransactionL3StatusCode.Sent;
				case L3TranStatus.Failed:
					return ExtTransactionL3StatusCode.Failed;
				case L3TranStatus.Rejected:
					return ExtTransactionL3StatusCode.Rejected;
				case L3TranStatus.ResendRejected:
					return ExtTransactionL3StatusCode.ResendRejected;
				case L3TranStatus.ResendFailed:
					return ExtTransactionL3StatusCode.ResendFailed;
				case L3TranStatus.NotApplicable:
				default:
					return ExtTransactionL3StatusCode.NotApplicable;
			}
		}

		public static PluginSettingDetail ConvertSettingsDetail(V2.SettingsDetail detail)
		{
			return Mapper.Map<PluginSettingDetail>(detail);
		}

		public static int? ConvertSettingsControlType(V2.SettingsControlType controlType)
		{
			return Mapper.Map<int?>(controlType);
		}

		public static CcvVerificationStatus ConvertCardVerificationStatus(V2.CcvVerificationStatus status)
		{
			CcvVerificationStatus ret = Mapper.Map<CcvVerificationStatus>(status);
			if (!Enum.IsDefined(typeof(CcvVerificationStatus), ret))
			{
				ret = CcvVerificationStatus.Unknown;
			}
			return ret;
		}

		internal static V2.CcvVerificationStatus ConvertCardVerificationStatus(CcvVerificationStatus status)
		{
			V2.CcvVerificationStatus ret = Mapper.Map<V2.CcvVerificationStatus>(status);
			if (!Enum.IsDefined(typeof(V2.CcvVerificationStatus), ret))
			{
				ret = V2.CcvVerificationStatus.Unknown;
			}
			return ret;
		}

		public static CCTranStatus ConvertTranStatus(V2.CCTranStatus status)
		{
			CCTranStatus ret = Mapper.Map<CCTranStatus>(status);
			if (!Enum.IsDefined(typeof(CCTranStatus), ret))
			{
				ret = CCTranStatus.Unknown;
			}
			return ret;
		}

		public static CcvVerificationStatus ConvertCvvStatus(V2.CcvVerificationStatus status)
		{
			return Mapper.Map<CcvVerificationStatus>(status);
		}

		private static int? convertSettingsControlType(V2.SettingsControlType i)
		{
			switch (i)
			{
				case V2.SettingsControlType.CheckBox:
					return 3;
				case V2.SettingsControlType.Combo:
					return 2;
				case V2.SettingsControlType.Text:
					return 1;
				default:
					return null;
			}
		}
	}
}
