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
using ASC.Api.Documents;
using ASC.Api.Employee;
using ASC.Core;
using ASC.Core.Users;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Web.Studio.UserControls.Common.Comments;
using ASC.Web.Studio.Utility.HtmlUtility;

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "message", Namespace = "")]
    public class MessageWrapperFull : MessageWrapper
    {
        [DataMember]
        public bool CanEditFiles { get; set; }

        [DataMember]
        public bool CanReadFiles { get; set; }

        [DataMember]
        public List<EmployeeWraperFull> Subscribers { get; set; }

        [DataMember]
        public List<FileWrapper> Files { get; set; }

        [DataMember]
        public List<CommentInfo> Comments { get; set; }

        [DataMember]
        public ProjectWrapperFull Project { get; set; }

        public MessageWrapperFull(ProjectApiBase projectApiBase, Message message, ProjectWrapperFull project, IEnumerable<EmployeeWraperFull> subscribers)
            : base(projectApiBase, message)
        {
            CanEditFiles = projectApiBase.ProjectSecurity.CanEditFiles(message);
            CanReadFiles = projectApiBase.ProjectSecurity.CanReadFiles(message.Project);
            Text = HtmlUtility.GetFull(Text);
            Project = project;
            Subscribers = subscribers.ToList();
        }

        public MessageWrapperFull(ProjectApiBase projectApiBase, Message message, ProjectWrapperFull project, IEnumerable<EmployeeWraperFull> subscribers, IEnumerable<FileWrapper> files, IEnumerable<CommentInfo> comments) :
            this(projectApiBase, message, project, subscribers)
        {
            Files = files.ToList();
            var creator = CoreContext.UserManager.GetUsers(message.CreateBy);
            Comments = new List<CommentInfo>(comments.Count() + 1)
            {
                new CommentInfo
                {
                    TimeStamp = message.CreateOn,
                    TimeStampStr = message.CreateOn.Ago(),
                    CommentBody = HtmlUtility.GetFull(message.Description),
                    CommentID = SecurityContext.CurrentAccount.ID.ToString() + "1",
                    UserID = message.CreateBy,
                    UserFullName = creator.DisplayUserName(),
                    UserProfileLink = creator.GetUserProfilePageURL(),
                    Inactive = false,
                    IsEditPermissions = false,
                    IsResponsePermissions = false,
                    IsRead = true,
                    UserAvatarPath = creator.GetBigPhotoURL(),
                    UserPost = creator.Title,
                    CommentList = new List<CommentInfo>()
                }
            };
            Comments.AddRange(comments);
            
        }
    }
}