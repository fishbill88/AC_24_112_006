using System.Collections;
using System.Collections.Generic;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using System;
using PX.Objects;
using PX.Objects.SO;

namespace PX.Objects.SO
{
  public class SOSetupMaint_Extension : PXGraphExtension<PX.Objects.SO.SOSetupMaint>
  {
    #region Event Handlers
		public delegate Boolean PrePersistDelegate();
		[PXOverride]
		public Boolean PrePersist(PrePersistDelegate baseMethod)
		{
			return baseMethod();
		}


    #endregion
  }
}