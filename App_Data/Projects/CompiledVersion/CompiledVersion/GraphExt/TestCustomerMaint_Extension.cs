using PX.Data;
using PX.Objects.AR;
using PX.Objects.CR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledVersion.GraphExt
{
    public class TestCustomerMaint_Extension : PXGraphExtension<PX.Objects.AR.CustomerMaint>
    {
        public static bool IsActive() => true;

        public PXDBAction<Customer> viewBusnessAccount2;
        [PXUIField(DisplayName = PX.Objects.AR.Messages.ViewBusnessAccount)]
        [PXButton]
        public virtual IEnumerable ViewBusnessAccount2(PXAdapter adapter)
        {
            BAccount bacct = Base.BAccount.Current;
            if (bacct != null)
            {
                BusinessAccountMaint editingBO = PXGraph.CreateInstance<PX.Objects.CR.BusinessAccountMaint>();
                editingBO.Load();
                editingBO.Clear();
                editingBO.BAccount.Current = editingBO.BAccount.Search<BAccount.acctCD>(bacct.AcctCD);
                throw new PXRedirectRequiredException(editingBO, "Edit Business Account");
            }
            return adapter.Get();
        }
    }
}
