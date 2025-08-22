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

using PX.Data;
using PX.SM;
using System;

namespace PX.Objects.FS
{
    [Serializable]
    public class FSCalendarComponentField : PXBqlTable, IBqlTable, ISortOrder
    {
        #region ComponentType
        public new abstract class componentType : PX.Data.BQL.BqlString.Field<componentType>
        {
            public abstract class Values : ListField_ComponentType { }
        }

        [PXDefault]
        [PXDBString(2, IsFixed = true, IsKey = true)]
        [PXUIField(DisplayName = "Component Type", Enabled = false)]
        [componentType.Values.List]
        public virtual string ComponentType { get; set; }
        #endregion
        #region LineNbr
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }

        protected int? _LineNbr;

        [PXInt]
        [PXUIField(Visible = false, Enabled = false)]
        public virtual int? LineNbr
        {
            get
            {
                if (_LineNbr == null)
                {
                    _LineNbr = SortOrder;
                }

                return _LineNbr;
            }
            set
            {
                _LineNbr = value;
            }
        }
        #endregion
        #region SortOrder
        public abstract class sortOrder : PX.Data.BQL.BqlInt.Field<sortOrder> { }

        [PXDBInt]
        [PXDefault(int.MaxValue)]
        [PXUIField(Visible = false, Enabled = false)]
        public virtual int? SortOrder { get; set; }
        #endregion
        #region IsActive
        public abstract class isActive : PX.Data.BQL.BqlBool.Field<isActive> { }

        [PXDBBool]
        [PXDefault(true)]
        [PXUIField(DisplayName = "Visible")]
        public bool? IsActive { get; set; }
        #endregion
        #region ObjectName
        public abstract class objectName : PX.Data.BQL.BqlString.Field<objectName> { }

        [PXDefault]
        [PXDBString(InputMask = "", IsUnicode = false, IsKey = true)]
        [PXUIField(DisplayName = "Object", Visibility = PXUIVisibility.SelectorVisible)]
        [PXStringList(new string[] { null }, new string[] { "" }, ExclusiveValues = false)]
        public virtual string ObjectName { get; set; }
        #endregion
        #region FieldName
        public abstract class fieldName : PX.Data.BQL.BqlString.Field<fieldName> { }

        [PXDefault]
        [PXDBString(InputMask = "", IsUnicode = false, IsKey = true)]
        [PXUIField(DisplayName = "Field Name")]
        [PXStringList(new string[] { null }, new string[] { "" }, ExclusiveValues = false)]
        public virtual string FieldName { get; set; }
        #endregion
        #region ImageUrl
        public abstract class imageUrl : PX.Data.BQL.BqlString.Field<imageUrl> { }

        [PXDBString(255, IsUnicode = false)]
        [PXUIField(DisplayName = "Icon")]
        [PXIconsList]
        public virtual String ImageUrl { get; set; }
        #endregion
        #region CreatedByID
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }

        [PXDBCreatedByID]
        [PXUIField(DisplayName = "CreatedByID")]
        public virtual Guid? CreatedByID { get; set; }
        #endregion
        #region CreatedByScreenID
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }

        [PXDBCreatedByScreenID]
        [PXUIField(DisplayName = "CreatedByScreenID")]
        public virtual string CreatedByScreenID { get; set; }
        #endregion
        #region CreatedDateTime
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }

        [PXDBCreatedDateTime]
        [PXUIField(DisplayName = "CreatedDateTime")]
        public virtual DateTime? CreatedDateTime { get; set; }
        #endregion
        #region LastModifiedByID
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }

        [PXDBLastModifiedByID]
        [PXUIField(DisplayName = "LastModifiedByID")]
        public virtual Guid? LastModifiedByID { get; set; }
        #endregion
        #region LastModifiedByScreenID
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }

        [PXDBLastModifiedByScreenID]
        [PXUIField(DisplayName = "LastModifiedByScreenID")]
        public virtual string LastModifiedByScreenID { get; set; }
        #endregion
        #region LastModifiedDateTime
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }

        [PXDBLastModifiedDateTime]
        [PXUIField(DisplayName = "LastModifiedDateTime")]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        #endregion
        #region tstamp
        public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }

        [PXDBTimestamp(VerifyTimestamp = VerifyTimestampOptions.BothFromGraphAndRecord)]
        [PXUIField(DisplayName = "tstamp")]
        public virtual byte[] tstamp { get; set; }
        #endregion
    }

    #region Projections
    #region AppoitmentBoxComponentField
    [Serializable]
    [PXBreakInheritance]
    [PXProjection(typeof(Select<FSCalendarComponentField,
                         Where<
                             FSCalendarComponentField.componentType, Equal<FSCalendarComponentField.componentType.Values.appointmentBox>>>), Persistent = true)]
    public partial class AppointmentBoxComponentField : FSCalendarComponentField
    {
        #region ComponentType
        public new abstract class componentType : PX.Data.BQL.BqlString.Field<componentType>
        {
            public abstract class Values : ListField_ComponentType { }
        }

        [PXDefault(componentType.Values.AppointmentBox)]
        [PXDBString(2, IsFixed = true, IsKey = true)]
        [PXUIField(DisplayName = "Component Type", Enabled = false)]
        [componentType.Values.List]
        public override string ComponentType { get; set; }
        #endregion
        #region SortOrder
        public new abstract class sortOrder : PX.Data.BQL.BqlInt.Field<sortOrder> { }

        [PXDBInt]
        [PXDefault(int.MaxValue)]
        [PXUIField(Visible = false, Enabled = false)]
        public override int? SortOrder { get; set; }
        #endregion
        #region ObjectName
        public new abstract class objectName : PX.Data.BQL.BqlString.Field<objectName> { }


        [PXDefault("PX.Objects.FS.FSAppointment")]
        [PXDBString(InputMask = "", IsUnicode = false, IsKey = true)]
        [PXUIField(DisplayName = "Object", Visibility = PXUIVisibility.SelectorVisible)]
        [PXStringList(new string[] { null }, new string[] { "" }, ExclusiveValues = false)]
        public override string ObjectName { get; set; }
        #endregion
        #region FieldName
        public new abstract class fieldName : PX.Data.BQL.BqlString.Field<fieldName> { }

        [PXDefault]
        [PXDBString(InputMask = "", IsUnicode = false, IsKey = true)]
        [PXUIField(DisplayName = "Field Name")]
        [PXStringList(new string[] { null }, new string[] { "" }, ExclusiveValues = false)]
        public override string FieldName { get; set; }
        #endregion
    }
    #endregion
    #region ServiceOrderComponentField
    [Serializable]
    [PXBreakInheritance]
    [PXProjection(typeof(Select<FSCalendarComponentField,
                         Where<
                             FSCalendarComponentField.componentType, Equal<FSCalendarComponentField.componentType.Values.serviceOrderGrid>>>), Persistent = true)]
    public partial class ServiceOrderComponentField : FSCalendarComponentField
    {
        #region ComponentType
        public new abstract class componentType : PX.Data.BQL.BqlString.Field<componentType>
        {
            public abstract class Values : ListField_ComponentType { }
        }

        [PXDBString(2, IsFixed = true, IsKey = true)]
        [PXDefault(componentType.Values.ServiceOrderGrid)]
        [PXUIField(DisplayName = "Component Type", Enabled = false)]
        [componentType.Values.List]
        public override string ComponentType { get; set; }
        #endregion
        #region SortOrder
        public new abstract class sortOrder : PX.Data.BQL.BqlInt.Field<sortOrder> { }

        [PXDBInt]
        [PXDefault(int.MaxValue)]
        [PXUIField(Visible = false, Enabled = false)]
        public override int? SortOrder { get; set; }
        #endregion
        #region ObjectName
        public new abstract class objectName : PX.Data.BQL.BqlString.Field<objectName> { }

        [PXDefault("PX.Objects.FS.FSServiceOrder")]
        [PXDBString(InputMask = "", IsUnicode = false, IsKey = true)]
        [PXUIField(DisplayName = "Object", Visibility = PXUIVisibility.SelectorVisible)]
        [PXStringList(new string[] { null }, new string[] { "" }, ExclusiveValues = false)]
        public override string ObjectName { get; set; }
        #endregion
        #region FieldName
        public new abstract class fieldName : PX.Data.BQL.BqlString.Field<fieldName> { }

        [PXDefault]
        [PXDBString(InputMask = "", IsUnicode = false, IsKey = true)]
        [PXUIField(DisplayName = "Field Name")]
        [PXStringList(new string[] { null }, new string[] { "" }, ExclusiveValues = false)]
        public override string FieldName { get; set; }
        #endregion
    }
    #endregion
    #region UnassignedAppComponentField
    [Serializable]
    [PXBreakInheritance]
    [PXProjection(typeof(Select<FSCalendarComponentField,
                         Where<
                             FSCalendarComponentField.componentType, Equal<FSCalendarComponentField.componentType.Values.unassignedAppGrid>>>), Persistent = true)]
    public partial class UnassignedAppComponentField : FSCalendarComponentField
    {
        #region ComponentType
        public new abstract class componentType : PX.Data.BQL.BqlString.Field<componentType>
        {
            public abstract class Values : ListField_ComponentType { }
        }

        [PXDBString(2, IsFixed = true, IsKey = true)]
        [PXDefault(componentType.Values.UnassignedAppGrid)]
        [PXUIField(DisplayName = "Component Type", Enabled = false)]
        [componentType.Values.List]
        public override string ComponentType { get; set; }
        #endregion
        #region SortOrder
        public new abstract class sortOrder : PX.Data.BQL.BqlInt.Field<sortOrder> { }

        [PXDBInt]
        [PXDefault(int.MaxValue)]
        [PXUIField(Visible = false, Enabled = false)]
        public override int? SortOrder { get; set; }
        #endregion
        #region ObjectName
        public new abstract class objectName : PX.Data.BQL.BqlString.Field<objectName> { }

        [PXDefault("PX.Objects.FS.FSAppointment")]
        [PXDBString(InputMask = "", IsUnicode = false, IsKey = true)]
        [PXUIField(DisplayName = "Object", Visibility = PXUIVisibility.SelectorVisible)]
        [PXStringList(new string[] { null }, new string[] { "" }, ExclusiveValues = false)]
        public override string ObjectName { get; set; }
        #endregion
        #region FieldName
        public new abstract class fieldName : PX.Data.BQL.BqlString.Field<fieldName> { }

        [PXDefault]
        [PXDBString(InputMask = "", IsUnicode = false, IsKey = true)]
        [PXUIField(DisplayName = "Field Name")]
        [PXStringList(new string[] { null }, new string[] { "" }, ExclusiveValues = false)]
        public override string FieldName { get; set; }
        #endregion
    }
    #endregion
    #endregion
}
