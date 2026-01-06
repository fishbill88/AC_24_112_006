using System;
using PX.Data;
using PX.Objects.IN;

namespace ACustom
{
    /// <summary>
    /// Extension for INItemClass to add hierarchy-related unbound fields
    /// </summary>
    public sealed class INItemClassExt : PXCacheExtension<INItemClass>
    {
        #region UsrHierarchyLevel
        /// <summary>
        /// Displays the hierarchy depth level (0 for root classes)
        /// </summary>
        [PXInt]
        [PXUIField(DisplayName = "Hierarchy Level", Enabled = false, Visible = true)]
        public int? UsrHierarchyLevel { get; set; }
        public abstract class usrHierarchyLevel : PX.Data.BQL.BqlInt.Field<usrHierarchyLevel> { }
        #endregion

        #region UsrFullHierarchyPath
        /// <summary>
        /// Displays the full hierarchy path from root to current class (e.g., "A → A1 → aa1")
        /// </summary>
        [PXString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Hierarchy Path", Enabled = false, Visible = true)]
        public string UsrFullHierarchyPath { get; set; }
        public abstract class usrFullHierarchyPath : PX.Data.BQL.BqlString.Field<usrFullHierarchyPath> { }
        #endregion

        #region UsrHasChildren
        /// <summary>
        /// Indicates whether this item class has any child classes
        /// </summary>
        [PXBool]
        [PXUIField(DisplayName = "Has Child Classes", Enabled = false, Visible = true)]
        public bool? UsrHasChildren { get; set; }
        public abstract class usrHasChildren : PX.Data.BQL.BqlBool.Field<usrHasChildren> { }
        #endregion

        #region UsrAttributeCount
        /// <summary>
        /// Total count of attributes including cascaded attributes from parent classes
        /// </summary>
        [PXInt]
        [PXUIField(DisplayName = "Total Attributes (Cascaded)", Enabled = false, Visible = true)]
        public int? UsrAttributeCount { get; set; }
        public abstract class usrAttributeCount : PX.Data.BQL.BqlInt.Field<usrAttributeCount> { }
        #endregion
    }
}
