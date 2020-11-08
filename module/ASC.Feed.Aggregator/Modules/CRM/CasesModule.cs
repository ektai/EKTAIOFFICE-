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
using System.Globalization;
using System.Linq;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Web.CRM.Core;
using ASC.Web.Studio.Utility;
using Autofac;

namespace ASC.Feed.Aggregator.Modules.CRM
{
    internal class CasesModule : FeedModule
    {
        private const string item = "cases";


        protected override string Table
        {
            get { return "crm_case"; }
        }

        protected override string LastUpdatedColumn
        {
            get { return "create_on"; }
        }

        protected override string TenantColumn
        {
            get { return "tenant_id"; }
        }

        protected override string DbId
        {
            get { return Constants.CrmDbId; }
        }


        public override string Name
        {
            get { return Constants.CasesModule; }
        }

        public override string Product
        {
            get { return ModulesHelper.CRMProductName; }
        }

        public override Guid ProductID
        {
            get { return ModulesHelper.CRMProductID; }
        }

        public override bool VisibleFor(Feed feed, object data, Guid userId)
        {
            return base.VisibleFor(feed, data, userId) && CRMSecurity.CanAccessTo((Cases)data);
        }

        public override IEnumerable<Tuple<Feed, object>> GetFeeds(FeedFilter filter)
        {
            var query = new SqlQuery("crm_case c")
                .Select(CasesColumns().Select(c => "c." + c).ToArray())
                .LeftOuterJoin("crm_entity_contact ec",
                               Exp.EqColumns("ec.entity_id", "c.id")
                               & Exp.Eq("ec.entity_type", 7)
                )
                .Select("group_concat(distinct cast(ec.contact_id as char))")
                .Where("c.tenant_id", filter.Tenant)
                .Where(Exp.Between("c.create_on", filter.Time.From, filter.Time.To))
                .GroupBy("c.id");

            using (var db = new DbManager(DbId))
            using (var scope = DIHelper.Resolve())
            {
                var dao = scope.Resolve<DaoFactory>().ContactDao;
                var cases = db.ExecuteList(query).ConvertAll(ToCases);
                return cases.Select(c => new Tuple<Feed, object>(ToFeed(c, dao), c)).ToList();
            }
        }


        private static IEnumerable<string> CasesColumns()
        {
            return new[]
                {
                    "id",
                    "title",
                    "is_closed",
                    "create_by",
                    "create_on",
                    "last_modifed_by",
                    "last_modifed_on" //6
                };
        }

        private static Cases ToCases(object[] r)
        {
            var cases = new Cases
                {
                    ID = Convert.ToInt32(r[0]),
                    Title = Convert.ToString(r[1]),
                    IsClosed = Convert.ToBoolean(r[2]),
                    CreateBy = new Guid(Convert.ToString(r[3])),
                    CreateOn = Convert.ToDateTime(r[4]),
                    LastModifedBy = new Guid(Convert.ToString(r[5])),
                    LastModifedOn = Convert.ToDateTime(r[6])
                };

            var members = Convert.ToString(r[7]);
            if (!string.IsNullOrEmpty(members))
            {
                cases.Members = new HashSet<int>(
                    members.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                           .Select(x => Convert.ToInt32(x))
                    );
            }

            return cases;
        }

        private Feed ToFeed(Cases cases, ContactDao dao)
        {
            var contactsString = Helper.GetContactsString(cases.Members, dao);

            var itemUrl = "/products/crm/cases.aspx?id=" + cases.ID + "#profile";
            return new Feed(cases.CreateBy, cases.CreateOn)
            {
                Item = item,
                ItemId = cases.ID.ToString(CultureInfo.InvariantCulture),
                ItemUrl = CommonLinkUtility.ToAbsolute(itemUrl),
                Product = Product,
                Module = Name,
                Title = cases.Title,
                AdditionalInfo = contactsString,
                Keywords = cases.Title,
                HasPreview = false,
                CanComment = false,
                GroupId = GetGroupId(item, cases.CreateBy)
            };
        }
    }
}