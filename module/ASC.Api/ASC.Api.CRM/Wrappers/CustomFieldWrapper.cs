/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@EKTAIOFFICE.com
 *
 * The interactive user interfaces in modified source and object code versions of EKTAIOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original EKTAIOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by EKTAIOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Runtime.Serialization;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Specific;

namespace ASC.Api.CRM.Wrappers
{
    [DataContract(Name = "customField", Namespace = "")]
    public class CustomFieldWrapper : CustomFieldBaseWrapper
    {
        public CustomFieldWrapper(int id)
            : base(id)
        {
        }

        public CustomFieldWrapper(CustomField customField)
            : base(customField)
        {
        }

        [DataMember]
        public int RelativeItemsCount { get; set; }

        public new static CustomFieldWrapper GetSample()
        {
            return new CustomFieldWrapper(0)
                {
                    Position = 10,
                    EntityId = 14523423,
                    FieldType = CustomFieldType.Date,
                    FieldValue = ApiDateTime.GetSample().ToString(),
                    Label = "Birthdate",
                    Mask = "",
                    RelativeItemsCount = 0
                };
        }
    }

    /// <summary>
    ///  User custom fields
    /// </summary>
    [DataContract(Name = "customField", Namespace = "")]
    public class CustomFieldBaseWrapper : ObjectWrapperBase
    {
        public CustomFieldBaseWrapper(int id) : base(id)
        {
        }

        public CustomFieldBaseWrapper(CustomField customField)
            : base(customField.ID)
        {
            EntityId = customField.EntityID;
            Label = customField.Label;
            FieldValue = customField.Value;
            FieldType = customField.FieldType;
            Position = customField.Position;
            Mask = customField.Mask;
        }

        [DataMember]
        public int EntityId { get; set; }

        [DataMember]
        public String Label { get; set; }

        [DataMember]
        public String FieldValue { get; set; }

        [DataMember]
        public CustomFieldType FieldType { get; set; }

        [DataMember]
        public int Position { get; set; }

        [DataMember]
        public String Mask { get; set; }

        public static CustomFieldBaseWrapper GetSample()
        {
            return new CustomFieldBaseWrapper(0)
                {
                    Position = 10,
                    EntityId = 14523423,
                    FieldType = CustomFieldType.Date,
                    FieldValue = ApiDateTime.GetSample().ToString(),
                    Label = "Birthdate",
                    Mask = ""
                };
        }
    }
}