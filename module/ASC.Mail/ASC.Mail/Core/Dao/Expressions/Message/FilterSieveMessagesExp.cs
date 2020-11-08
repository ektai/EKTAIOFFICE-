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
using System.Linq.Expressions;
using ASC.Common.Data.Sql.Expressions;
using ASC.ElasticSearch;
using ASC.Mail.Core.DbSchema.Tables;
using ASC.Mail.Data.Contracts;
using ASC.Mail.Data.Search;
using ASC.Mail.Enums.Filter;
using ASC.Mail.Extensions;

namespace ASC.Mail.Core.Dao.Expressions.Message
{
    public class FilterSieveMessagesExp : IMessagesExp
    {
        public List<int> Ids { get; private set; }
        public MailSieveFilterData Filter { get; private set; }
        public int Tenant { get; private set; }
        public string User { get; private set; }

        public string OrderBy
        {
            get { return "date_sent"; }
        }

        public bool? OrderAsc
        {
            get
            {
                return null;
            }
        }

        public int? StartIndex { get; set; }

        public int? Limit { get; set; }

        public List<int> TagIds
        {
            get { return null; }
        }

        public int? UserFolderId
        {
            get { return null; }
        }

        public FilterSieveMessagesExp(List<int> ids, int tenant, string user, MailSieveFilterData filter, int page,
            int pageSize)
        {
            Filter = filter;
            Tenant = tenant;
            User = user;

            if (ids.Any())
            {
                Ids = ids.Skip(page*pageSize).Take(pageSize).ToList();
                return;
            }

            StartIndex = page*pageSize;
            Limit = pageSize;
        }

        private const string MM_ALIAS = "mm";

        public virtual Exp GetExpression()
        {
            var filterExp = Exp.Empty;

            if (!FactoryIndexer<MailWrapper>.Support)
            {
                Func<ConditionKeyType, string> toDbField = c =>
                {
                    switch (c)
                    {
                        case ConditionKeyType.From:
                            return MailTable.Columns.From.Prefix(MM_ALIAS);
                        case ConditionKeyType.To:
                            return MailTable.Columns.To.Prefix(MM_ALIAS);
                        case ConditionKeyType.Cc:
                            return MailTable.Columns.Cc.Prefix(MM_ALIAS);
                        case ConditionKeyType.Subject:
                            return MailTable.Columns.Subject.Prefix(MM_ALIAS);
                        default:
                            throw new ArgumentOutOfRangeException("c", c, null);
                    }
                };

                Func<MailSieveFilterConditionData, Exp> getConditionExp = c =>
                {
                    var e = Exp.Empty;

                    switch (c.Operation)
                    {
                        case ConditionOperationType.Matches:
                            e = c.Key == ConditionKeyType.ToOrCc
                                ? Exp.Or(Exp.Eq(MailTable.Columns.To.Prefix(MM_ALIAS), c.Value),
                                    Exp.Eq(MailTable.Columns.Cc.Prefix(MM_ALIAS), c.Value))
                                : Exp.Eq(toDbField(c.Key), c.Value);
                            break;
                        case ConditionOperationType.Contains:
                            e = c.Key == ConditionKeyType.ToOrCc
                                ? Exp.Or(Exp.Like(MailTable.Columns.To.Prefix(MM_ALIAS), c.Value, SqlLike.AnyWhere),
                                    Exp.Like(MailTable.Columns.Cc.Prefix(MM_ALIAS), c.Value, SqlLike.AnyWhere))
                                : Exp.Like(toDbField(c.Key), c.Value, SqlLike.AnyWhere);
                            break;
                        case ConditionOperationType.NotMatches:
                            e = c.Key == ConditionKeyType.ToOrCc
                                ? !Exp.And(Exp.Eq(MailTable.Columns.To.Prefix(MM_ALIAS), c.Value),
                                    Exp.Eq(MailTable.Columns.Cc.Prefix(MM_ALIAS), c.Value))
                                : !Exp.Eq(toDbField(c.Key), c.Value);
                            break;
                        case ConditionOperationType.NotContains:
                            e = c.Key == ConditionKeyType.ToOrCc
                                ? Exp.And(!Exp.Like(MailTable.Columns.To.Prefix(MM_ALIAS), c.Value, SqlLike.AnyWhere),
                                    !Exp.Like(MailTable.Columns.Cc.Prefix(MM_ALIAS), c.Value, SqlLike.AnyWhere))
                                : !Exp.Like(toDbField(c.Key), c.Value, SqlLike.AnyWhere);
                            break;
                    }

                    return e;
                };

                if (Filter.Conditions != null && Filter.Conditions.Any())
                {
                    var cExp = Exp.Empty;

                    foreach (var c in Filter.Conditions)
                    {
                        switch (Filter.Options.MatchMultiConditions)
                        {
                            case MatchMultiConditionsType.MatchAll:
                            case MatchMultiConditionsType.None:
                                cExp &= getConditionExp(c);
                                break;
                            case MatchMultiConditionsType.MatchAtLeastOne:
                                cExp |= getConditionExp(c);
                                break;
                        }
                    }

                    filterExp &= cExp;
                }

            }

            if (Ids != null && Ids.Any())
            {
                filterExp &= Exp.In(MailTable.Columns.Id.Prefix(MM_ALIAS), Ids);
            }

            if (Filter.Options.ApplyTo.Folders.Any())
            {
                filterExp &= Exp.In(MailTable.Columns.Folder.Prefix(MM_ALIAS), Filter.Options.ApplyTo.Folders);
            }

            if (Filter.Options.ApplyTo.Mailboxes.Any())
            {
                filterExp &= Exp.In(MailTable.Columns.MailboxId.Prefix(MM_ALIAS), Filter.Options.ApplyTo.Mailboxes);
            }

            switch (Filter.Options.ApplyTo.WithAttachments)
            {
                case ApplyToAttachmentsType.WithAttachments:
                    filterExp &= Exp.Gt(MailTable.Columns.AttachCount.Prefix(MM_ALIAS), 0);
                    break;
                case ApplyToAttachmentsType.WithoutAttachments:
                    filterExp &= Exp.Eq(MailTable.Columns.AttachCount.Prefix(MM_ALIAS), 0);
                    break;
                case ApplyToAttachmentsType.WithAndWithoutAttachments:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var exp = Exp.Eq(MailTable.Columns.Tenant.Prefix(MM_ALIAS), Tenant) &
                      Exp.Eq(MailTable.Columns.User.Prefix(MM_ALIAS), User) &
                      Exp.Eq(MailTable.Columns.IsRemoved.Prefix(MM_ALIAS), false);

            exp &= filterExp;

            return exp;
        }

        public static bool TryGetFullTextSearchIds(MailSieveFilterData filter, string user, out List<int> ids, out long total)
        {
            ids = new List<int>();

            if (!FactoryIndexer<MailWrapper>.Support)
            {
                total = 0;
                return false;
            }

            var userId = new Guid(user);

            Func<ConditionKeyType, Expression<Func<MailWrapper, object>>> getExp = (c) =>
            {
                switch (c)
                {
                    case ConditionKeyType.From:
                        return w => w.FromText;
                    case ConditionKeyType.To:
                        return w => w.ToText;
                    case ConditionKeyType.Cc:
                        return w => w.Cc;
                    case ConditionKeyType.Subject:
                        return w => w.Subject;
                    default:
                        throw new ArgumentOutOfRangeException("c", c, null);
                }
            };

            Func<MailSieveFilterConditionData, string> getValue = (c) =>
            {
                return c.Operation == ConditionOperationType.Matches || c.Operation == ConditionOperationType.NotMatches
                    ? string.Format("\"{0}\"", c.Value)
                    : c.Value;
            };

            Func<MailSieveFilterConditionData, Selector<MailWrapper>> setSelector = (c) =>
            {
                Selector<MailWrapper> sel;

                var value = getValue(c);

                if (c.Key == ConditionKeyType.ToOrCc)
                {
                    sel = new Selector<MailWrapper>().Or(
                        s => s.Match(w => w.ToText, value), 
                        s => s.Match(w => w.Cc, value));
                }
                else
                {
                    sel = new Selector<MailWrapper>().Match(getExp(c.Key), value);
                }

                if (c.Operation == ConditionOperationType.NotMatches ||
                    c.Operation == ConditionOperationType.NotContains)
                {
                    return new Selector<MailWrapper>().Not(s => sel);
                }

                return sel;
            };

            var selector = new Selector<MailWrapper>();

            foreach (var c in filter.Conditions)
            {
                if (filter.Options.MatchMultiConditions == MatchMultiConditionsType.MatchAll ||
                    filter.Options.MatchMultiConditions == MatchMultiConditionsType.None)
                {
                    selector &= setSelector(c);
                }
                else
                {
                    selector |= setSelector(c);
                }
            }

            if (filter.Options.ApplyTo.Folders.Any())
            {
                selector.In(r => r.Folder, filter.Options.ApplyTo.Folders);
            }

            if (filter.Options.ApplyTo.Mailboxes.Any())
            {
                selector.In(r => r.MailboxId, filter.Options.ApplyTo.Mailboxes);
            }

            if (filter.Options.ApplyTo.WithAttachments != ApplyToAttachmentsType.WithAndWithoutAttachments)
            {
                selector.Where(r => r.HasAttachments,
                    filter.Options.ApplyTo.WithAttachments == ApplyToAttachmentsType.WithAttachments);
            }

            selector
                .Where(r => r.UserId, userId)
                .Sort(r => r.DateSent, true);

            List<int> mailIds;
            if (!FactoryIndexer<MailWrapper>.TrySelectIds(s => selector, out mailIds, out total))
                return false;

            ids = mailIds;

            return true;
        }
    }
}