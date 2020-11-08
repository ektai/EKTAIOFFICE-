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
using ASC.Core;
using ASC.Specific;
using ASC.Web.Community.News.Code;

namespace ASC.Api.Events
{
    [DataContract(Name = "poll", Namespace = "")]
    public class PollWrapper
    {
        public PollWrapper(FeedPoll poll)
        {
            Voted = poll.IsUserVote(SecurityContext.CurrentAccount.ID.ToString());
            if (Voted)
            {
                //Get results
            }
            StartDate = (ApiDateTime)poll.StartDate;
            EndDate = (ApiDateTime)poll.EndDate;
            PollType = poll.PollType;
            Votes = poll.Variants.Select(x=>new VoteWrapper(){Id = x.ID, Name = x.Name, Votes = poll.GetVariantVoteCount(x.ID)});
        }

        internal PollWrapper()
        {

        }

        [DataMember(Order = 200, EmitDefaultValue = true)]
        public FeedPollType PollType { get; set; }

        [DataMember(Order = 200, EmitDefaultValue = false)]
        public ApiDateTime EndDate { get; set; }

        [DataMember(Order = 200, EmitDefaultValue = false)]
        public ApiDateTime StartDate { get; set; }

        [DataMember(Order = 200, EmitDefaultValue = true)]
        public bool Voted { get; set; }

        [DataMember(Order = 300)]
        public IEnumerable<VoteWrapper> Votes { get; set; }

        public static PollWrapper GetSample()
        {
            return new PollWrapper
                {
                    EndDate = ApiDateTime.GetSample(),
                    PollType = FeedPollType.SimpleAnswer,
                    StartDate = ApiDateTime.GetSample(),
                    Voted = false,
                    Votes = new List<VoteWrapper> {VoteWrapper.GetSample()}
                };
        }
    }
}