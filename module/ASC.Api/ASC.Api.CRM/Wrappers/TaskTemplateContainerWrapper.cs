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
using System.Collections.Generic;
using System.Runtime.Serialization;
using ASC.Api.Employee;

namespace ASC.Api.CRM.Wrappers
{
    [DataContract(Namespace = "taskTemplateContainer")]
    public class TaskTemplateContainerWrapper : ObjectWrapperBase
    {
        public TaskTemplateContainerWrapper() :
            base(0)
        {
        }

        public TaskTemplateContainerWrapper(int id)
            : base(id)
        {
        }

        [DataMember(IsRequired = true, EmitDefaultValue = true)]
        public String Title { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = true)]
        public String EntityType { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<TaskTemplateWrapper> Items { get; set; }

        public static TaskTemplateContainerWrapper GetSample()
        {
            return new TaskTemplateContainerWrapper
                {
                    EntityType = "contact",
                    Title = "Birthday greetings",
                    Items = new List<TaskTemplateWrapper>
                        {
                            TaskTemplateWrapper.GetSample()
                        }
                };
        }
    }

    [DataContract(Namespace = "taskTemplate")]
    public class TaskTemplateWrapper : ObjectWrapperBase
    {
        public TaskTemplateWrapper() : base(0)
        {
        }

        public TaskTemplateWrapper(int id) :
            base(id)
        {
        }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public int ContainerID { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public String Title { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String Description { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public EmployeeWraper Responsible { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public TaskCategoryWrapper Category { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool isNotify { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public long OffsetTicks { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool DeadLineIsFixed { get; set; }

        public static TaskTemplateWrapper GetSample()
        {
            return new TaskTemplateWrapper
                {
                    Title = "Send an Email",
                    Category = TaskCategoryWrapper.GetSample(),
                    isNotify = true,
                    Responsible = EmployeeWraper.GetSample(),
                    ContainerID = 12,
                    DeadLineIsFixed = false,
                    OffsetTicks = TimeSpan.FromDays(10).Ticks,
                    Description = ""
                };
        }
    }
}