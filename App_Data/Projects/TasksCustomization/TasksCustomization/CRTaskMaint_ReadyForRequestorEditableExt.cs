using PX.Data;
using PX.Objects.CR;
using PX.Objects.EP;

namespace MyProject
{
    // Ensures Ready for Requestor status is treated as editable like Open/InProcess
    public class CRTaskMaint_ReadyForRequestorEditableExt : PXGraphExtension<CRTaskMaint>
    {
        public static bool IsActive() => true;

        public delegate bool IsTaskEditableDelegate(CRActivity row);

        public virtual bool IsTaskEditable(CRActivity row, IsTaskEditableDelegate baseMethod)
        {
            bool result = baseMethod(row);
            if (!result && row != null)
            {
                string status = (string)Base.Tasks.Cache.GetValueOriginal<CRActivity.uistatus>(row) ?? ActivityStatusListAttribute.Open;
                if (status == TaskCustomStatuses.ReadyForRequestor)
                    return true;
            }
            return result;
        }
    }
}
