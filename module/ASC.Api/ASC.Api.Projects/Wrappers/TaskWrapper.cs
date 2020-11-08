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
using System.Linq;
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.Projects.Core.Domain;
using ASC.Specific;

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "task", Namespace = "")]
    public class TaskWrapper : ObjectWrapperFullBase
    {
        [DataMember]
        public bool CanEdit { get; set; }

        [DataMember]
        public bool CanCreateSubtask { get; set; }

        [DataMember]
        public bool CanCreateTimeSpend { get; set; }

        [DataMember]
        public bool CanDelete { get; set; }

        [DataMember]
        public bool CanReadFiles { get; set; }

        [DataMember(Order = 12, EmitDefaultValue = false)]
        public ApiDateTime Deadline { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public ApiDateTime StartDate { get; set; }

        [DataMember(Order = 13, EmitDefaultValue = false)]
        public int MilestoneId { get; set; }

        [DataMember(Order = 12)]
        public TaskPriority Priority { get; set; }

        [DataMember(Order = 14)]
        public SimpleProjectWrapper ProjectOwner { get; set; }

        [DataMember(Order = 15, EmitDefaultValue = false)]
        public int Progress { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public List<SubtaskWrapper> Subtasks { get; set; }

        [DataMember(Order = 21, EmitDefaultValue = false)]
        public IEnumerable<TaskLinkWrapper> Links { get; set; }

        [DataMember(Order = 53)]
        public List<EmployeeWraper> Responsibles { get; set; }

        [DataMember(Order = 53, EmitDefaultValue=false)]
        public List<Guid> ResponsibleIds { get; set; }

        [DataMember(Order = 54, EmitDefaultValue = false)]
        public SimpleMilestoneWrapper Milestone { get; set; }

        [DataMember(Order = 55, EmitDefaultValue = false)]
        public int? CustomTaskStatus { get; set; }

        private TaskWrapper()
        {
        }

        public TaskWrapper(ProjectApiBase projectApiBase, Task task)
        {
            Id = task.ID;
            Title = task.Title;
            Description = task.Description;
            Status = (int)task.Status;

            if (Status > 2)
            {
                Status = 1;
            }

            CustomTaskStatus = task.CustomTaskStatus;

            Deadline = (task.Deadline == DateTime.MinValue ? null : new ApiDateTime(task.Deadline, TimeZoneInfo.Local));
            Priority = task.Priority;
            ProjectOwner = new SimpleProjectWrapper(task.Project);
            MilestoneId = task.Milestone;
            Created = (ApiDateTime)task.CreateOn;
            Updated = (ApiDateTime)task.LastModifiedOn;
            StartDate = task.StartDate.Equals(DateTime.MinValue) ? null : (ApiDateTime)task.StartDate;

            if (task.SubTasks != null)
            {
                Subtasks = task.SubTasks.Select(x => new SubtaskWrapper(projectApiBase, x, task)).ToList();
            }

            Progress = task.Progress;

            if (task.Milestone != 0 && task.MilestoneDesc != null)
            {
                Milestone = new SimpleMilestoneWrapper(task.MilestoneDesc);
            }

            if (task.Links.Any())
            {
                Links = task.Links.Select(r => new TaskLinkWrapper(r));
            }

            if (task.Security == null)
            {
                projectApiBase.ProjectSecurity.GetTaskSecurityInfo(task);
            }

            if (projectApiBase.Context.GetRequestValue("simple") != null)
            {
                CreatedById = task.CreateBy;
                UpdatedById = task.LastModifiedBy;
                if (task.Responsibles != null)
                {
                    ResponsibleIds = task.Responsibles;
                }
            }
            else
            {
                CreatedBy = projectApiBase.GetEmployeeWraper(task.CreateBy);
                if (task.CreateBy != task.LastModifiedBy)
                {
                    UpdatedBy = projectApiBase.GetEmployeeWraper(task.LastModifiedBy);
                }
                if (task.Responsibles != null)
                {
                    Responsibles = task.Responsibles.Select(projectApiBase.GetEmployeeWraper).OrderBy(r => r.DisplayName).ToList();
                }
            }

            CanEdit = task.Security.CanEdit;
            CanCreateSubtask = task.Security.CanCreateSubtask;
            CanCreateTimeSpend = task.Security.CanCreateTimeSpend;
            CanDelete = task.Security.CanDelete;
            CanReadFiles = task.Security.CanReadFiles;
        }

        public TaskWrapper(ProjectApiBase projectApiBase, Task task, Milestone milestone)
            : this(projectApiBase, task)
        {
            if (milestone != null && task.Milestone != 0)
                Milestone = new SimpleMilestoneWrapper(milestone);
        }


        public static TaskWrapper GetSample()
        {
            return new TaskWrapper
                {
                    Id = 10,
                    Title = "Sample Title",
                    Description = "Sample description",
                    Deadline = ApiDateTime.GetSample(),
                    Priority = TaskPriority.High,
                    Status = (int)MilestoneStatus.Open,
                    Responsible = EmployeeWraper.GetSample(),
                    ProjectOwner = SimpleProjectWrapper.GetSample(),
                    MilestoneId = 123,
                    Created = ApiDateTime.GetSample(),
                    CreatedBy = EmployeeWraper.GetSample(),
                    Updated = ApiDateTime.GetSample(),
                    UpdatedBy = EmployeeWraper.GetSample(),
                    StartDate = ApiDateTime.GetSample()
                };
        }
    }
}