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

namespace PX.Objects.AR.CCPaymentProcessing.Common
{
	public enum SyncStatus
	{ 
		None,
		Success,
		Error,
		Warning
	}
	
	/// <summary>Defines the credit card transaction types.</summary>
	public enum CCTranType
	{
		/// <summary>Checks if the requested amount might be taken from the credit card, locks it on the credit card account, and takes the authorized amount from the card simultaneously.</summary>
		AuthorizeAndCapture,
		/// <summary>Checks if the requested amount might be taken from the credit card and locks it on the credit card account. </summary>
		AuthorizeOnly,
		/// <summary>Captures the previously authorized transaction.</summary>
		PriorAuthorizedCapture,
		/// <summary>Captures the manually authorized transaction.</summary>
		CaptureOnly,
		/// <summary>Returns the money back to the card.</summary>
		Credit,
		/// <summary>Reverses the authorized or captured transaction.</summary>
		Void,
		/// <summary>Performs a void first and then performs a credit if the void failed.</summary>
		VoidOrCredit,
		/// <summary>Imports the unknown transaction.</summary>
		Unknown
	}

	/// <summary>Contains the transaction statuses returned by the processing center.</summary>
	public enum CCTranStatus
	{
		/// <summary>The transaction was approved.</summary>
		Approved,
		/// <summary>The transaction was declined.</summary>
		Declined,
		/// <summary>
		/// An error occurred when the transaction is processed.
		/// </summary>
		Error,
		/// <summary>
		/// The transaction is under review.
		/// </summary>
		HeldForReview,
		/// <summary>The transaction was expired.</summary>
		Expired,
		/// <summary>
		/// There was no answer or the answer can't be interpreted.
		/// </summary>
		Unknown
	}

	/// <summary>
	/// Contains the Level 3 Data transaction statuses
	/// </summary>
	public enum L3TranStatus
	{
		/// <summary>
		/// The transaction is not subject for L3 and will not be available for further L3processing.
		/// </summary>
		NotApplicable,
		/// <summary>
		/// The transaction is ready for sending Level 3 Data.
		/// </summary>
		Pending,
		/// <summary>
		/// The Level 3 Data has been sent.
		/// </summary>
		Sent,
		/// <summary>
		/// Sending of Level 3 is failed.
		/// </summary>
		Failed,
		/// <summary>
		/// The Level 3 Data has been rejected by processing center.
		/// </summary>
		Rejected,
		/// <summary>
		/// Reseneded Level 3 Data has been rejected by processing center.
		/// </summary>
		ResendRejected,
		/// <summary>
		/// Resending of Level 3 Data is failed.
		/// </summary>
		ResendFailed
	}

	/// <summary>Defines the CVV verification statuses returned by the credit card authority.</summary>
	public enum CcvVerificationStatus
	{
		/// <summary>The CVV code is correct.</summary>
		Match,
		/// <summary>The CVV code is incorrect.</summary>
		NotMatch,
		/// <summary>The CVV code is not processed.</summary>
		NotProcessed,
		/// <summary>The CVV code was not provided but is required for the authorization.</summary>
		ShouldHaveBeenPresent,
		/// <summary>The card issue authority was unable to verify the code.</summary>
		IssuerUnableToProcessRequest,
		/// <summary>The CVV code has already been verified by the Acumatica ERP core.</summary>
		RelyOnPreviousVerification,
		/// <summary>Not applicable</summary>
		NotApplicable,
		/// <summary>Empty status is returned.</summary>
		Empty,
		/// <summary>Any other status is returned.</summary>
		Unknown
	}

	[Flags]
	public enum CCResultFlag
	{
		None,
		OrigTransactionExpired,
		OrigTransactionNotFound,
	}

	public enum CCProcessingFeature
	{
		Base,
		ProfileManagement,
		ExtendedProfileManagement,
		HostedForm,
		PaymentHostedForm,
		WebhookManagement,
		TransactionGetter,
		PaymentForm,
		CapturePreauthorization,
		ProfileEditForm,
		ProfileForm,
		TransactionFinder,
		EFTSupport,
		PayLink,
		Level3,
		TerminalGetter,
	}

	public enum ProcessingStatus
	{
		Unknown,
		AuthorizeFail,
		CaptureFail,
		VoidFail,
		CreditFail,
		AuthorizeSuccess,
		AuthorizeExpired,
		CaptureSuccess,
		VoidSuccess,
		CreditSuccess,
		AuthorizeHeldForReview,
		CaptureHeldForReview,
		VoidHeldForReview,
		CreditHeldForReview,
		AuthorizeDecline,
		CaptureDecline,
		VoidDecline,
		CreditDecline,
		CaptureExpired
	}

	/// <summary>
	/// Result of CC plug-in validation
	/// </summary>
	public enum CCPluginCheckResult
	{
		/// <summary>Plug-in type validation passed.</summary>
		Ok,
		/// <summary>Plug-in type is empty.</summary>
		Empty,
		/// <summary>Plug-in type is missing.</summary>
		Missing,
		/// <summary>Plug-in type is unsupported.</summary>
		Unsupported,
		/// <summary>Validation was not performed.</summary>
		NotPerformed,
	}
	/// <summary>Defines the possible card types</summary>
	public enum CCCardType
	{
		Other,
		Visa,
		MasterCard,
		AmericanExpress,
		Discover,
		JCB,
		DinersClub,
		UnionPay,
		Debit,
		Alelo,
		Alia,
		Cabal,
		Carnet,
		Dankort,
		Elo,
		Forbrugsforeningen,
		Maestro,
		Naranja,
		Sodexo,
		Vr,
		EFT
	}
}
