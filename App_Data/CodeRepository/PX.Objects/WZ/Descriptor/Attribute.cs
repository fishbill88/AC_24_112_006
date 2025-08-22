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

namespace PX.Objects.WZ
{
    #region WizardScenarioStatusesAttribute

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class WizardScenarioStatusesAttribute : PXStringListAttribute
    {
        public const string _PENDING = "PN";
        public const string _ACTIVE = "AC";
        public const string _SUSPEND = "SU";
        public const string _COMPLETED = "CP";

        public WizardScenarioStatusesAttribute()
            : base(new[] { _PENDING, _ACTIVE, _SUSPEND, _COMPLETED },
                    new[] { Messages.Pending, Messages.Active, Messages.Suspend, Messages.Completed })
        { }

        public sealed class Pending : PX.Data.BQL.BqlString.Constant<Pending>
		{
            public Pending() : base(_PENDING) { }
        }

        public sealed class Active : PX.Data.BQL.BqlString.Constant<Active>
		{
            public Active() : base(_ACTIVE) { }
        }

        public sealed class Suspend : PX.Data.BQL.BqlString.Constant<Suspend>
		{
            public Suspend() : base(_SUSPEND) { }
        }

        public sealed class Completed : PX.Data.BQL.BqlString.Constant<Completed>
		{
            public Completed() : base(_COMPLETED) { }
        }
    }

    #endregion

    #region WizardTaskTypesAttribute

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class WizardTaskTypesAttribute : PXStringListAttribute
    {
        public const string _ARTICLE = "AR";
        public const string _SCREEN = "SC";

        public WizardTaskTypesAttribute()
            : base(new[] { _ARTICLE, _SCREEN },
                    new[] { Messages.Article, Messages.Screen })
        { }

        public sealed class Article : PX.Data.BQL.BqlString.Constant<Article>
		{
            public Article() : base(_ARTICLE) { }
        }

        public sealed class Screen : PX.Data.BQL.BqlString.Constant<Screen>
		{
            public Screen() : base(_SCREEN) { }
        }
    }

    #endregion

    #region WizardTaskStatusesAttribute

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class WizardTaskStatusesAttribute : PXStringListAttribute
    {
        public const string _PENDING = "PN";
        public const string _SKIPPED = "SK";
        public const string _ACTIVE = "AC";
        public const string _DISABLED = "DS";
        public const string _COMPLETED = "CP";
        public const string _OPEN = "OP";

        public WizardTaskStatusesAttribute()
            : base(new[] { _PENDING, _SKIPPED, _ACTIVE, _DISABLED, _COMPLETED, _OPEN },
                    new[] { Messages.Pending, Messages.Skipped, Messages.Active, Messages.Disabled, Messages.Completed, Messages.Open })
        { }

        public sealed class Pending : PX.Data.BQL.BqlString.Constant<Pending>
		{
            public Pending() : base(_PENDING) { }
        }

        public sealed class Skipped : PX.Data.BQL.BqlString.Constant<Skipped>
		{
            public Skipped() : base(_SKIPPED) { }
        }

        public sealed class Active : PX.Data.BQL.BqlString.Constant<Active>
		{
            public Active() : base(_ACTIVE) { }
        }

        public sealed class Disabled : PX.Data.BQL.BqlString.Constant<Disabled>
		{
            public Disabled() : base(_DISABLED) { }
        }

        public sealed class Completed : PX.Data.BQL.BqlString.Constant<Completed>
		{
            public Completed() : base(_COMPLETED) { }
        }

        public sealed class Open : PX.Data.BQL.BqlString.Constant<Open>
		{
            public Open() : base(_OPEN) { }
        }

    }

    #endregion
}
