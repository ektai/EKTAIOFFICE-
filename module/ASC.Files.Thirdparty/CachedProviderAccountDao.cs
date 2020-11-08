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


using ASC.Common.Caching;
using ASC.Files.Core;
using System;
using System.Collections.Concurrent;
using System.Globalization;

namespace ASC.Files.Thirdparty
{
    internal class CachedProviderAccountDao : ProviderAccountDao
    {
        private static readonly ConcurrentDictionary<string, IProviderInfo> cache = new ConcurrentDictionary<string, IProviderInfo>();
        private static readonly ICacheNotify cacheNotify;

        private readonly string _rootKey;


        static CachedProviderAccountDao()
        {
            cacheNotify = AscCache.Notify;
            cacheNotify.Subscribe<ProviderAccountCacheItem>((i, a) => RemoveFromCache(i.Key));
        }

        public CachedProviderAccountDao(int tenantID, string storageKey)
            : base(tenantID, storageKey)
        {
            _rootKey = tenantID.ToString(CultureInfo.InvariantCulture);
        }

        public override IProviderInfo GetProviderInfo(int linkId)
        {
            var key = _rootKey + linkId.ToString(CultureInfo.InvariantCulture);
            IProviderInfo value;
            if (!cache.TryGetValue(key, out value))
            {
                value = base.GetProviderInfo(linkId);
                cache.TryAdd(key, value);
            }
            return value;
        }

        public override void RemoveProviderInfo(int linkId)
        {
            base.RemoveProviderInfo(linkId);

            var key = _rootKey + linkId.ToString(CultureInfo.InvariantCulture);
            cacheNotify.Publish(new ProviderAccountCacheItem { Key = key }, CacheNotifyAction.Remove);
        }

        public override int UpdateProviderInfo(int linkId, string customerTitle, AuthData authData, FolderType folderType, Guid? userId = null)
        {
            var result = base.UpdateProviderInfo(linkId, customerTitle, authData, folderType, userId);

            var key = _rootKey + linkId.ToString(CultureInfo.InvariantCulture);
            cacheNotify.Publish(new ProviderAccountCacheItem { Key = key }, CacheNotifyAction.Update);
            return result;
        }

        public override int UpdateProviderInfo(int linkId, AuthData authData)
        {
            var result = base.UpdateProviderInfo(linkId, authData);

            var key = _rootKey + linkId.ToString(CultureInfo.InvariantCulture);
            cacheNotify.Publish(new ProviderAccountCacheItem { Key = key }, CacheNotifyAction.Update);
            return result;
        }


        private static void RemoveFromCache(string key)
        {
            IProviderInfo value = null;
            cache.TryRemove(key, out value);
        }


        [Serializable]
        class ProviderAccountCacheItem
        {
            public string Key { get; set; }
        }
    }
}