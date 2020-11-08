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
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Mail.Core.Dao.Expressions.Mailbox;
using ASC.Mail.Core.Dao.Interfaces;
using ASC.Mail.Core.DbSchema;
using ASC.Mail.Core.DbSchema.Interfaces;
using ASC.Mail.Core.DbSchema.Tables;
using ASC.Mail.Core.Entities;
using ASC.Mail.Utils;

namespace ASC.Mail.Core.Dao
{
    public class MailboxDao : BaseDao, IMailboxDao
    {
        protected static ITable table = new MailTableFactory().Create<MailboxTable>();

        public MailboxDao(IDbManager dbManager) : 
            base(table, dbManager, -1)
        {
        }

        public Mailbox GetMailBox(IMailboxExp exp)
        {
            var query = Query()
                .Where(exp.GetExpression());

            return Db.ExecuteList(query)
                .ConvertAll(ToMailbox)
                .SingleOrDefault();
        }

        public List<Mailbox> GetMailBoxes(IMailboxesExp exp)
        {
            var query = Query()
                .Where(exp.GetExpression());

            if (!string.IsNullOrEmpty(exp.OrderBy) && exp.OrderAsc.HasValue)
            {
                query.OrderBy(exp.OrderBy, exp.OrderAsc.Value);
            }

            if (exp.Limit.HasValue)
            {
                query.SetMaxResults(exp.Limit.Value);
            }

            return Db.ExecuteList(query)
                .ConvertAll(ToMailbox);
        }

        public Mailbox GetNextMailBox(IMailboxExp exp)
        {
            var query = Query()
                .Where(exp.GetExpression())
                .OrderBy(MailboxTable.Columns.Id, true)
                .SetMaxResults(1);

            var mailbox = Db.ExecuteList(query)
                .ConvertAll(ToMailbox)
                .SingleOrDefault();

            return mailbox;
        }

        public Tuple<int, int> GetRangeMailboxes(IMailboxExp exp)
        {
            var query = new SqlQuery(MailboxTable.TABLE_NAME)
                .SelectMin(MailboxTable.Columns.Id)
                .SelectMax(MailboxTable.Columns.Id)
                .Where(exp.GetExpression());

            var range = Db.ExecuteList(query)
                .ConvertAll(r => new Tuple<int, int>(Convert.ToInt32(r[0]), Convert.ToInt32(r[1])))
                .SingleOrDefault();

            return range;
        }

        public List<Tuple<int, string>> GetMailUsers(IMailboxExp exp)
        {
            var query = Query()
                .Select(MailboxTable.Columns.Tenant, MailboxTable.Columns.User)
                .Where(exp.GetExpression());

            var list = Db.ExecuteList(query)
                .ConvertAll(r => new Tuple<int, string>(Convert.ToInt32(r[0]), Convert.ToString(r[1])));

            return list;
        }

        public int SaveMailBox(Mailbox mailbox)
        {
            var query = new SqlInsert(MailboxTable.TABLE_NAME, true)
                .InColumnValue(MailboxTable.Columns.Id, mailbox.Id)
                .InColumnValue(MailboxTable.Columns.Tenant, mailbox.Tenant)
                .InColumnValue(MailboxTable.Columns.User, mailbox.User)
                .InColumnValue(MailboxTable.Columns.Address, mailbox.Address)
                .InColumnValue(MailboxTable.Columns.Name, mailbox.Name)
                .InColumnValue(MailboxTable.Columns.Enabled, mailbox.Enabled)
                .InColumnValue(MailboxTable.Columns.IsRemoved, mailbox.IsRemoved)
                .InColumnValue(MailboxTable.Columns.IsProcessed, mailbox.IsProcessed)
                .InColumnValue(MailboxTable.Columns.IsServerMailbox, mailbox.IsTeamlabMailbox)
                .InColumnValue(MailboxTable.Columns.Imap, mailbox.Imap)
                .InColumnValue(MailboxTable.Columns.UserOnline, mailbox.UserOnline)
                .InColumnValue(MailboxTable.Columns.IsDefault, mailbox.IsDefault)
                .InColumnValue(MailboxTable.Columns.MsgCountLast, mailbox.MsgCountLast)
                .InColumnValue(MailboxTable.Columns.SizeLast, mailbox.SizeLast)
                .InColumnValue(MailboxTable.Columns.LoginDelay, mailbox.LoginDelay)
                .InColumnValue(MailboxTable.Columns.QuotaError, mailbox.QuotaError)
                .InColumnValue(MailboxTable.Columns.ImapIntervals, mailbox.ImapIntervals)
                .InColumnValue(MailboxTable.Columns.BeginDate, mailbox.BeginDate)
                .InColumnValue(MailboxTable.Columns.EmailInFolder, mailbox.EmailInFolder)
                .InColumnValue(MailboxTable.Columns.Password, MailUtil.EncryptPassword(mailbox.Password))
                .InColumnValue(MailboxTable.Columns.SmtpPassword,
                    !string.IsNullOrEmpty(mailbox.SmtpPassword)
                        ? MailUtil.EncryptPassword(mailbox.SmtpPassword)
                        : "")
                .InColumnValue(MailboxTable.Columns.OAuthToken,
                    !string.IsNullOrEmpty(mailbox.OAuthToken)
                        ? MailUtil.EncryptPassword(mailbox.OAuthToken)
                        : "")
                .InColumnValue(MailboxTable.Columns.OAuthType, mailbox.OAuthType)
                .InColumnValue(MailboxTable.Columns.SmtpServerId, mailbox.SmtpServerId)
                .InColumnValue(MailboxTable.Columns.ServerId, mailbox.ServerId)
                .InColumnValue(MailboxTable.Columns.DateChecked, mailbox.DateChecked)
                .InColumnValue(MailboxTable.Columns.DateUserChecked, mailbox.DateUserChecked)
                .InColumnValue(MailboxTable.Columns.DateLoginDelayExpires, mailbox.DateLoginDelayExpires)
                .InColumnValue(MailboxTable.Columns.DateAuthError, mailbox.DateAuthError)
                .InColumnValue(MailboxTable.Columns.DateCreated, mailbox.DateCreated)
                .Identity(0, 0, true);

            var result = Db.ExecuteScalar<int>(query);

            return result;
        }

        public bool SetMailboxRemoved(Mailbox mailbox)
        {
            var query = new SqlUpdate(MailboxTable.TABLE_NAME)
                .Set(MailboxTable.Columns.IsRemoved, true)
                .Where(MailboxTable.Columns.Id, mailbox.Id)
                .Where(MailboxTable.Columns.Tenant, mailbox.Tenant)
                .Where(MailTable.Columns.User, mailbox.User);

            var result = Db.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool RemoveMailbox(Mailbox mailbox)
        {
            var query = new SqlDelete(MailboxTable.TABLE_NAME)
                .Where(MailboxTable.Columns.Id, mailbox.Id)
                .Where(MailboxTable.Columns.Tenant, mailbox.Tenant)
                .Where(MailTable.Columns.User, mailbox.User);
            var result = Db.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool Enable(IMailboxExp exp, bool enabled)
        {
            var query = new SqlUpdate(MailboxTable.TABLE_NAME)
                .Set(MailboxTable.Columns.Enabled, enabled)
                .Where(exp.GetExpression());

            if (enabled)
                query.Set(MailboxTable.Columns.DateAuthError, null);

            var result = Db.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool SetNextLoginDelay(IMailboxExp exp, TimeSpan delay)
        {
            var query = new SqlUpdate(MailboxTable.TABLE_NAME)
                .Set(MailboxTable.Columns.IsProcessed, false)
                .Set(string.Format(SET_LOGIN_DELAY_EXPIRES, (int)delay.TotalSeconds))
                .Where(exp.GetExpression());

            var result = Db.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool SetMailboxEmailIn(Mailbox mailbox, string emailInFolder)
        {
            var query = new SqlUpdate(MailboxTable.TABLE_NAME)
                .Set(MailboxTable.Columns.EmailInFolder, "" != emailInFolder ? emailInFolder : null)
                .Where(MailboxTable.Columns.Id, mailbox.Id)
                .Where(MailboxTable.Columns.Tenant, mailbox.Tenant)
                .Where(MailboxTable.Columns.User, mailbox.User)
                .Where(MailboxTable.Columns.IsRemoved, false);

            var result = Db.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool SetMailboxesActivity(int tenant, string user, bool userOnline = true)
        {
            var query = new SqlUpdate(MailboxTable.TABLE_NAME)
                .Where(MailboxTable.Columns.Tenant, tenant)
                .Where(MailboxTable.Columns.User, user)
                .Where(MailboxTable.Columns.IsRemoved, false)
                .Set(SET_DATE_USER_CHECKED)
                .Set(MailboxTable.Columns.UserOnline, userOnline);

            var result = Db.ExecuteNonQuery(query);

            return result > 0;
        }

        private const string SET_DATE_CHECKED = MailboxTable.Columns.DateChecked + " = UTC_TIMESTAMP()";
        private const string SET_DATE_USER_CHECKED = MailboxTable.Columns.DateUserChecked + " = UTC_TIMESTAMP()";

        private const string SET_LOGIN_DELAY_EXPIRES =
            MailboxTable.Columns.DateLoginDelayExpires + " = DATE_ADD(UTC_TIMESTAMP(), INTERVAL {0} SECOND)";

        private static readonly string SetDefaultLoginDelayExpires =
            MailboxTable.Columns.DateLoginDelayExpires + " = DATE_ADD(UTC_TIMESTAMP(), INTERVAL " +
            Defines.DefaultServerLoginDelayStr + " SECOND)";

        public bool SetMailboxInProcess(int id)
        {
            var query = new SqlUpdate(MailboxTable.TABLE_NAME)
                .Set(MailboxTable.Columns.IsProcessed, true)
                .Set(SET_DATE_CHECKED)
                .Where(MailboxTable.Columns.Id, id)
                .Where(MailboxTable.Columns.IsProcessed, false)
                .Where(MailboxTable.Columns.IsRemoved, false);

            var result = Db.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool SetMailboxProcessed(Mailbox mailbox, int nextLoginDelay, bool? enabled = null,
            int? messageCount = null, long? size = null, bool? quotaError = null, string oAuthToken = null,
            string imapIntervalsJson = null, bool? resetImapIntervals = false)
        {
            if (nextLoginDelay < Defines.DefaultServerLoginDelay)
                nextLoginDelay = Defines.DefaultServerLoginDelay;

            var query = new SqlUpdate(MailboxTable.TABLE_NAME)
                .Set(MailboxTable.Columns.IsProcessed, false)
                .Set(SET_DATE_CHECKED)
                .Set(nextLoginDelay > Defines.DefaultServerLoginDelay
                    ? string.Format(SET_LOGIN_DELAY_EXPIRES, nextLoginDelay)
                    : SetDefaultLoginDelayExpires)
                .Where(MailboxTable.Columns.Id, mailbox.Id);

            if (enabled.HasValue)
            {
                query.Set(MailboxTable.Columns.Enabled, enabled.Value);
            }

            if (messageCount.HasValue)
            {
                query.Set(MailboxTable.Columns.MsgCountLast, messageCount.Value);
            }

            if (size.HasValue)
            {
                query.Set(MailboxTable.Columns.SizeLast, size.Value);
            }

            if (quotaError.HasValue)
            {
                query.Set(MailboxTable.Columns.QuotaError, quotaError.Value);
            }

            if (!string.IsNullOrEmpty(oAuthToken))
            {
                query.Set(MailboxTable.Columns.OAuthToken, MailUtil.EncryptPassword(oAuthToken));
            }

            if (resetImapIntervals.HasValue)
            {
                query.Set(MailboxTable.Columns.ImapIntervals, null);
            }
            else
            {
                if (!string.IsNullOrEmpty(imapIntervalsJson))
                {
                    query.Set(MailboxTable.Columns.ImapIntervals, imapIntervalsJson);
                }
            }

            var result = Db.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool SetMailboxAuthError(int id, DateTime? authErroDate)
        {
            var query = new SqlUpdate(MailboxTable.TABLE_NAME)
                .Set(MailboxTable.Columns.DateAuthError, authErroDate)
                .Where(MailboxTable.Columns.Id, id);

            if (authErroDate.HasValue)
                query.Where(MailboxTable.Columns.DateAuthError, null);

            var result = Db.ExecuteNonQuery(query);

            return result > 0;
        }

        private const string SET_PROCESS_EXPIRES =
            "TIMESTAMPDIFF(MINUTE, " + MailboxTable.Columns.DateChecked + ", UTC_TIMESTAMP()) > {0}";

        public List<int> SetMailboxesProcessed(int timeoutInMinutes)
        {
            var query = new SqlQuery(MailboxTable.TABLE_NAME)
                .Select(MailboxTable.Columns.Id)
                .Where(MailboxTable.Columns.IsProcessed, true)
                .Where(!Exp.Eq(MailboxTable.Columns.DateChecked, null))
                .Where(string.Format(SET_PROCESS_EXPIRES, timeoutInMinutes));

            var ids = Db.ExecuteList(query)
                .ConvertAll(r => Convert.ToInt32(r[0]));

            if (!ids.Any())
                return ids;

            var update = new SqlUpdate(MailboxTable.TABLE_NAME)
                        .Set(MailboxTable.Columns.IsProcessed, false)
                        .Where(Exp.In(MailboxTable.Columns.Id, ids.ToArray()));

            Db.ExecuteNonQuery(update);

            return ids;
        }

        public bool CanAccessTo(IMailboxExp exp)
        {
            var query = new SqlQuery(MailboxTable.TABLE_NAME)
                .Select(MailboxTable.Columns.Id)
                .Where(exp.GetExpression());

            var foundIds = Db.ExecuteList(query)
                .ConvertAll(res => Convert.ToInt32(res[0]));

            return foundIds.Any();
        }

        public MailboxStatus GetMailBoxStatus(IMailboxExp exp)
        {
            var query = new SqlQuery(MailboxTable.TABLE_NAME)
                .Select(MailboxTable.Columns.Id,
                    MailboxTable.Columns.IsRemoved,
                    MailboxTable.Columns.Enabled,
                    MailboxTable.Columns.BeginDate)
                .Where(exp.GetExpression());

            return Db.ExecuteList(query)
                .ConvertAll(ToMMailboxStatus)
                .SingleOrDefault();
        }

        protected MailboxStatus ToMMailboxStatus(object[] r)
        {
            var status = new MailboxStatus
            {
                Id = Convert.ToInt32(r[0]),
                IsRemoved = Convert.ToBoolean(r[1]),
                Enabled = Convert.ToBoolean(r[2]),
                BeginDate = Convert.ToDateTime(r[3])
            };

            return status;
        }

        protected Mailbox ToMailbox(object[] r)
        {
            var mb = new Mailbox
            {
                Id = Convert.ToInt32(r[0]),
                User = Convert.ToString(r[1]),
                Tenant = Convert.ToInt32(r[2]),
                Address = Convert.ToString(r[3]),
                Enabled = Convert.ToBoolean(r[4]),

                MsgCountLast = Convert.ToInt32(r[6]),
                SizeLast = Convert.ToInt32(r[7]),

                Name = Convert.ToString(r[9]),
                LoginDelay = Convert.ToInt32(r[10]),
                IsProcessed = Convert.ToBoolean(r[11]),
                IsRemoved = Convert.ToBoolean(r[12]),
                IsDefault = Convert.ToBoolean(r[13]),
                QuotaError = Convert.ToBoolean(r[14]),
                Imap = Convert.ToBoolean(r[15]),
                BeginDate = Convert.ToDateTime(r[16]),
                OAuthType = Convert.ToInt32(r[17]),

                ImapIntervals = Convert.ToString(r[19]),
                SmtpServerId = Convert.ToInt32(r[20]),
                ServerId = Convert.ToInt32(r[21]),
                EmailInFolder = Convert.ToString(r[22]),
                IsTeamlabMailbox = Convert.ToBoolean(r[23]),
                DateCreated = Convert.ToDateTime(r[24]),
                DateChecked = Convert.ToDateTime(r[25]),
                DateUserChecked = Convert.ToDateTime(r[26]),
                UserOnline = Convert.ToBoolean(r[27]),
                DateLoginDelayExpires = Convert.ToDateTime(r[28]),
                DateAuthError = r[29] == null ? (DateTime?) null : Convert.ToDateTime(r[29])
            };

            string password = Convert.ToString(r[5]),
                smtpPassword = Convert.ToString(r[8]),
                oAuthToken = Convert.ToString(r[18]);

            MailUtil.TryDecryptPassword(password, out password);

            mb.Password = password;

            if (!string.IsNullOrEmpty(smtpPassword))
            {
                MailUtil.TryDecryptPassword(smtpPassword, out smtpPassword);
            }

            mb.SmtpPassword = smtpPassword ?? "";

            MailUtil.TryDecryptPassword(oAuthToken, out oAuthToken);

            mb.OAuthToken = oAuthToken;

            return mb;
        }
    }
}