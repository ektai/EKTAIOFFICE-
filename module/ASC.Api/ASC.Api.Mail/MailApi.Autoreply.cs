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


using ASC.Api.Attributes;
using System;
using ASC.Mail.Core.Engine;
using ASC.Mail.Data.Contracts;

namespace ASC.Api.Mail
{
    public partial class MailApi
    {
        /// <summary>
        /// This method needed for update or create autoreply.
        /// </summary>
        /// <param name="mailboxId">Id of updated mailbox.</param>
        /// <param name="turnOn">New autoreply status.</param>
        /// <param name="onlyContacts">If true then send autoreply only for contacts.</param>
        /// <param name="turnOnToDate">If true then field To is active.</param>
        /// <param name="fromDate">Start date of autoreply sending.</param>
        /// <param name="toDate">End date of autoreply sending.</param>
        /// <param name="subject">New autoreply subject.</param>
        /// <param name="html">New autoreply value.</param>
        [Create(@"autoreply/update/{mailboxId:[0-9]+}")]
        public MailAutoreplyData UpdateAutoreply(int mailboxId, bool turnOn, bool onlyContacts,
            bool turnOnToDate, DateTime fromDate, DateTime toDate, string subject, string html)
        {
            var result = MailEngineFactory
                .AutoreplyEngine
                .SaveAutoreply(mailboxId, turnOn, onlyContacts, turnOnToDate, fromDate, toDate, subject, html);

            CacheEngine.Clear(Username);

            return result;
        }
    }
}
