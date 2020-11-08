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
using ASC.Files.Core;

namespace ASC.Files.Thirdparty.SharePoint
{
    internal class SharePointTagDao: SharePointDaoBase, ITagDao
    {
        public SharePointTagDao(SharePointProviderInfo sharePointInfo, SharePointDaoSelector sharePointDaoSelector)
            : base(sharePointInfo, sharePointDaoSelector)
        {
        }

        public void Dispose()
        {
            ProviderInfo.Dispose();
        }

        public IEnumerable<Tag> GetTags(TagType tagType, IEnumerable<FileEntry> fileEntries)
        {
            return new List<Tag>();
        }

        public IEnumerable<Tag> GetTags(Guid owner, TagType tagType)
        {
            return new List<Tag>();
        }

        public IEnumerable<Tag> GetTags(string name, TagType tagType)
        {
            return new List<Tag>();
        }

        public IEnumerable<Tag> GetTags(string[] names, TagType tagType)
        {
            return new List<Tag>();
        }

        public IEnumerable<Tag> GetNewTags(Guid subject, Folder parentFolder, bool deepSearch)
        {
            using (var dbManager = new DbManager(FileConstant.DatabaseId))
            {
                var folderId = SharePointDaoSelector.ConvertId(parentFolder.ID);

                var fakeFolderId = parentFolder.ID.ToString();

                var entryIDs = dbManager.ExecuteList(Query("files_thirdparty_id_mapping")
                                                         .Select("hash_id")
                                                         .Where(Exp.Like("id", fakeFolderId, SqlLike.StartWith)))
                                        .ConvertAll(x => x[0]);

                if (!entryIDs.Any()) return new List<Tag>();

                var sqlQuery = new SqlQuery("files_tag ft")
                    .Select("ft.name",
                            "ft.flag",
                            "ft.owner",
                            "ftl.entry_id",
                            "ftl.entry_type",
                            "ftl.tag_count",
                            "ft.id")
                    .Distinct()
                    .LeftOuterJoin("files_tag_link ftl",
                                   Exp.EqColumns("ft.tenant_id", "ftl.tenant_id") &
                                   Exp.EqColumns("ft.id", "ftl.tag_id"))
                    .Where(Exp.Eq("ft.tenant_id", TenantID) &
                           Exp.Eq("ftl.tenant_id", TenantID) &
                           Exp.Eq("ft.flag", (int) TagType.New) &
                           Exp.In("ftl.entry_id", entryIDs));

                if (subject != Guid.Empty)
                    sqlQuery.Where(Exp.Eq("ft.owner", subject));

                var tags = dbManager.ExecuteList(sqlQuery).ConvertAll(r => new Tag
                    {
                        TagName = Convert.ToString(r[0]),
                        TagType = (TagType) r[1],
                        Owner = new Guid(r[2].ToString()),
                        EntryId = MappingID(r[3]),
                        EntryType = (FileEntryType) r[4],
                        Count = Convert.ToInt32(r[5]),
                        Id = Convert.ToInt32(r[6])
                    });

                if (deepSearch) return tags;

                var folderFileIds = new[] {fakeFolderId}
                    .Concat(ProviderInfo.GetFolderFolders(folderId).Select(x => ProviderInfo.MakeId(x.ServerRelativeUrl)))
                    .Concat(ProviderInfo.GetFolderFiles(folderId).Select(x => ProviderInfo.MakeId(x.ServerRelativeUrl)));

                return tags.Where(tag => folderFileIds.Contains(tag.EntryId.ToString()));
            }
        }

        public IEnumerable<Tag> GetNewTags(Guid subject, IEnumerable<FileEntry> fileEntries)
        {
            return new List<Tag>();
        }

        public IEnumerable<Tag> GetNewTags(Guid subject, FileEntry fileEntry)
        {
            return new List<Tag>();
        }

        public IEnumerable<Tag> SaveTags(IEnumerable<Tag> tag)
        {
            return new List<Tag>();
        }

        public IEnumerable<Tag> SaveTags(Tag tag)
        {
            return new List<Tag>();
        }

        public void UpdateNewTags(IEnumerable<Tag> tag)
        {
        }

        public void UpdateNewTags(Tag tag)
        {
        }

        public void RemoveTags(IEnumerable<Tag> tag)
        {
        }

        public void RemoveTags(Tag tag)
        {
        }

        public IEnumerable<Tag> GetTags(object entryID, FileEntryType entryType, TagType tagType)
        {
            return new List<Tag>();
        }
    }
}
