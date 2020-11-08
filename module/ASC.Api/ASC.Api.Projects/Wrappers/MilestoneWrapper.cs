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
using ASC.Api.Employee;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Specific;

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "milestone", Namespace = "")]
    public class MilestoneWrapper : ObjectWrapperFullBase
    {
        [DataMember(Order = 14)]
        public SimpleProjectWrapper ProjectOwner { get; set; }

        [DataMember(Order = 20)]
        public ApiDateTime Deadline { get; set; }

        [DataMember(Order = 20)]
        public bool IsKey { get; set; }

        [DataMember(Order = 20)]
        public bool IsNotify { get; set; }

        [DataMember]
        public bool CanEdit { get; set; }

        [DataMember]
        public bool CanDelete { get; set; }

        [DataMember(Order = 20)]
        public int ActiveTaskCount { get; set; }

        [DataMember(Order = 20)]
        public int ClosedTaskCount { get; set; }


        private MilestoneWrapper()
        {
        }

        public MilestoneWrapper(ProjectApiBase projectApiBase, Milestone milestone)
        {
            Id = milestone.ID;
            ProjectOwner = new SimpleProjectWrapper(milestone.Project);
            Title = milestone.Title;
            Description = milestone.Description;
            Created = (ApiDateTime)milestone.CreateOn;
            Updated = (ApiDateTime)milestone.LastModifiedOn;
            Status = (int)milestone.Status;
            Deadline = new ApiDateTime(milestone.DeadLine, TimeZoneInfo.Local);
            IsKey = milestone.IsKey;
            IsNotify = milestone.IsNotify;
            CanEdit = projectApiBase.ProjectSecurity.CanEdit(milestone);
            CanDelete = projectApiBase.ProjectSecurity.CanDelete(milestone);
            ActiveTaskCount = milestone.ActiveTaskCount;
            ClosedTaskCount = milestone.ClosedTaskCount;

            if (projectApiBase.Context.GetRequestValue("simple") != null)
            {
                CreatedById = milestone.CreateBy;
                UpdatedById = milestone.LastModifiedBy;
                if (!milestone.Responsible.Equals(Guid.Empty))
                {
                    ResponsibleId = milestone.Responsible;
                }
            }
            else
            {
                CreatedBy = projectApiBase.GetEmployeeWraper(milestone.CreateBy);
                if (milestone.CreateBy != milestone.LastModifiedBy)
                {
                    UpdatedBy = projectApiBase.GetEmployeeWraper(milestone.LastModifiedBy);
                }
                if (!milestone.Responsible.Equals(Guid.Empty))
                {
                    Responsible = projectApiBase.GetEmployeeWraper(milestone.Responsible);
                }
            }
        }

        public static MilestoneWrapper GetSample()
        {
            return new MilestoneWrapper
                {
                    Id = 10,
                    ProjectOwner = SimpleProjectWrapper.GetSample(),
                    Title = "Sample Title",
                    Description = "Sample description",
                    Created = ApiDateTime.GetSample(),
                    CreatedBy = EmployeeWraper.GetSample(),
                    Updated = ApiDateTime.GetSample(),
                    UpdatedBy = EmployeeWraper.GetSample(),
                    Responsible = EmployeeWraper.GetSample(),
                    Status = (int)MilestoneStatus.Open,
                    Deadline = ApiDateTime.GetSample(),
                    IsKey = false,
                    IsNotify = false,
                    CanEdit = true,
                    ActiveTaskCount = 15,
                    ClosedTaskCount = 5
                };
        }
    }
}