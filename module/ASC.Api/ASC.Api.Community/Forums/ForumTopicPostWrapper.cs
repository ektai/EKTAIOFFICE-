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
using ASC.Forum;
using ASC.Specific;

namespace ASC.Api.Forums
{
    [DataContract(Name = "post", Namespace = "")]
    public class ForumTopicPostWrapper : IApiSortableDate
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string Subject { get; set; }

        [DataMember(Order = 2)]
        public string Text { get; set; }

        [DataMember(Order = 3)]
        public ApiDateTime Created { get; set; }

        private ApiDateTime updated;

        [DataMember(Order = 3)]
        public ApiDateTime Updated
        {
            get { return updated >= Created ? updated : Created; }
            set { updated = value; }
        }

        [DataMember(Order = 9)]
        public EmployeeWraper CreatedBy { get; set; }

        [DataMember(Order = 10)]
        public string ThreadTitle { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public List<ForumTopicPostAttachmentWrapper> Attachments { get; set; }

        public ForumTopicPostWrapper(Post post)
        {
            Id = post.ID;
            Text = post.Text;
            Created = (ApiDateTime)post.CreateDate;
            Updated = (ApiDateTime)post.EditDate;
            Subject = post.Subject;
            CreatedBy = EmployeeWraper.Get(Core.CoreContext.UserManager.GetUsers(post.PosterID));
            Attachments = post.Attachments.Select(x => new ForumTopicPostAttachmentWrapper(x)).ToList();
        }

        private ForumTopicPostWrapper()
        {
        }

        public static ForumTopicPostWrapper GetSample()
        {
            return new ForumTopicPostWrapper
                {
                    Id = 123,
                    CreatedBy = EmployeeWraper.GetSample(),
                    Created = ApiDateTime.GetSample(),
                    Updated = ApiDateTime.GetSample(),
                    Subject = "Sample subject",
                    Text = "Post text",
                    Attachments = new List<ForumTopicPostAttachmentWrapper> { ForumTopicPostAttachmentWrapper.GetSample() }
                };
        }
    }
}