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
using PX.Common;
using PX.Data;
using PX.Data.EP;
using PX.Data.Wiki.Parser;
using PX.SM;

namespace PX.Objects.CR.Extensions
{
	// Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
	public class CREmailActivityMaint_CRSelectNotificatonTemplateExt : CRSelectNotificatonTemplateExt<CREmailActivityMaint, CRSMEmail>
	{
		#region Events

		protected virtual void _(Events.RowSelected<CRSMEmail> e)
		{
			if (e.Row == null)
				return;
			
			LoadEmailSource.SetEnabled(e.Row.IsIncome != true && e.Row.MPStatus == MailStatusListAttribute.Draft && e.Row.RefNoteID != null);
		}

		#endregion

		#region Methods

		public override void MapData(Notification notification)
		{
			object row = new EntityHelper(Base).GetEntityRow(Base.Message.Current.RefNoteID, true);
			if (row == null)
				return;

			var rowType = row.GetType();
			PXPrimaryGraphAttribute.FindPrimaryGraph(Base.Caches[rowType], true, ref row, out var primaryGraphType);

			var templateGraph = PXGraph.CreateInstance(primaryGraphType);
			var type = templateGraph.GetPrimaryCache().GetItemType();

			if (!rowType.Equals(type))
			{
				row = new EntityHelper(Base).GetEntityRow(type, Base.Message.Current.RefNoteID);
			}

			if (!templateGraph.Views.ContainsKey(GeneralInfoSelect.ViewName))
			{
				PXSelectBase generalInfo = new GeneralInfoSelect(templateGraph);
				templateGraph.Views.Add(GeneralInfoSelect.ViewName, generalInfo.View);
			}

			var keys = GetKeys(row, templateGraph.Caches[type]);
			EntityHelper eh = new EntityHelper(templateGraph);
			templateGraph.Caches[type].Current = eh.GetEntityRow(type, keys);
			Notification upd = PXCache<Notification>.CreateCopy(notification);
			upd.Subject = PXTemplateContentParser.Instance.Process(notification.Subject, templateGraph, type, null);
			string subjectError = "";
			if (templateGraph.HasException())
			{
				Base.Message.Cache.RaiseExceptionHandling<CRSMEmail.subject>(Base.Message.Current, null,
					new PXSetPropertyException(MessagesNoPrefix.CannotFindViewOnParse, PXErrorLevel.Warning));
				subjectError = PXUIFieldAttribute.GetWarning(
					templateGraph.Views[templateGraph.PrimaryView].Cache,
					templateGraph.Views[templateGraph.PrimaryView].Cache.Current,
					templateGraph.Views[templateGraph.PrimaryView].Cache.Keys[0]);
			}

			upd.Body = PXTemplateContentParser.Instance.Process(notification.Body, templateGraph, type, null);
			if (templateGraph.HasException())
			{
				if (string.IsNullOrEmpty(subjectError) ||
						subjectError != PXUIFieldAttribute.GetWarning(
										templateGraph.Views[templateGraph.PrimaryView].Cache,
										null,
										templateGraph.Views[templateGraph.PrimaryView].Cache.Keys[0]))
				{
					Base.Message.Cache.RaiseExceptionHandling<CRSMEmail.body>(Base.Message.Current, null,
						new PXSetPropertyException(MessagesNoPrefix.CannotFindViewOnParse, PXErrorLevel.Warning));
				}
			}
			upd.NTo = PXTemplateContentParser.Instance.Process(notification.NTo, templateGraph, type, null);
			upd.NCc = PXTemplateContentParser.Instance.Process(notification.NCc, templateGraph, type, null);
			upd.NBcc = PXTemplateContentParser.Instance.Process(notification.NBcc, templateGraph, type, null);

			if (upd.NFrom.HasValue && MailAccountManager.GetEmailAccountIfAllowed(Base, upd.NFrom) != null)
			{
				Base.Message.Current.MailAccountID = upd.NFrom.Value;
				Base.Message.Current.MailFrom = CREmailActivityMaint.FillMailFrom(Base, Base.Message.Current, true);
				Base.Message.Current.MailReply = CREmailActivityMaint.FillMailReply(Base, Base.Message.Current);
			}

			if (NotificationInfo.Current.ReplaceEmailContents == false)
			{
				Base.Message.Current.MailTo = AppendAddresses<CRSMEmail.mailTo>(Base.Message.Cache, Base.Message.Current.MailTo, upd.NTo);
				Base.Message.Current.MailCc = AppendAddresses<CRSMEmail.mailCc>(Base.Message.Cache, Base.Message.Current.MailCc, upd.NCc);
				Base.Message.Current.MailBcc = AppendAddresses<CRSMEmail.mailBcc>(Base.Message.Cache, Base.Message.Current.MailBcc, upd.NBcc);
				Base.Message.Current.Subject += $" {upd.Subject}";
				Base.Message.Current.Body = Tools.AppendToHtmlBody(Base.Message.Current.Body, "<br/>", Tools.GetBody(upd.Body));
			}
			else
			{
				Base.Message.Current.MailTo = upd.NTo;
				Base.Message.Current.MailCc = upd.NCc;
				Base.Message.Current.MailBcc = upd.NBcc;
				Base.Message.Current.Body = upd.Body;
				Base.Message.Current.Subject = upd.Subject;
			}

			bool isReplyEmail = Base.Message.Current.ResponseToNoteID != null && Base.Message.Current.Outgoing == true;

			Base.Message.Current.Body = MailAccountManager.AppendSignature(
				Base.Message.Current.Body,
				Base,
				isReplyEmail
					? MailAccountManager.SignatureOptions.ReplyAndForward
					: MailAccountManager.SignatureOptions.NewEmail
			);

			Base.Message.Current.Body = PX.Web.UI.PXRichTextConverter.NormalizeHtml(Base.Message.Current.Body);
		}

		protected string AppendAddresses<TEmailField>(PXCache cache, string address1, string address2)
			where
			TEmailField : IBqlField
		{
			try
			{
				return PXDBEmailAttribute.AppendAddresses(address1, address2);
			}
			catch (Exception ex)
			{
				cache.RaiseExceptionHandling<TEmailField>(cache.Current, address1,
					new PXSetPropertyException(ex.InnerException, PXErrorLevel.Warning, MessagesNoPrefix.AtLeastOneDataFieldError));
			}
			return address1;
		}

		protected virtual object[] GetKeys(object e, PXCache cache)
		{
			return cache.BqlKeys.Select(t => cache.GetValue(e, t.Name)).ToArray();
		}

		#endregion
	}
}
