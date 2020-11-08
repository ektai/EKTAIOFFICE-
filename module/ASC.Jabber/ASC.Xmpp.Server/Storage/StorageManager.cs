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


using ASC.Xmpp.Server.Storage.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ASC.Xmpp.Server.Storage
{
    public class StorageManager : IDisposable
    {
        private IDictionary<string, object> storages = new ConcurrentDictionary<string, object>();

        public IOfflineStore OfflineStorage
        {
            get { return GetStorage<IOfflineStore>("offline"); }
        }

        public IRosterStore RosterStorage
        {
            get { return GetStorage<IRosterStore>("roster"); }
        }

        public IVCardStore VCardStorage
        {
            get { return GetStorage<IVCardStore>("vcard"); }
        }

        public IPrivateStore PrivateStorage
        {
            get { return GetStorage<IPrivateStore>("private"); }
        }

        public IMucStore MucStorage
        {
            get { return GetStorage<IMucStore>("muc"); }
        }

        public IUserStore UserStorage
        {
            get { return GetStorage<IUserStore>("users"); }
        }

        public T GetStorage<T>(string storageName)
        {
            object storage;
            storages.TryGetValue(storageName, out storage);
            return (T)storage;
        }

        public void SetStorage(string storageName, object storage)
        {
            storages[storageName] = storage;
        }

        public void Dispose()
        {
            foreach (var s in storages.Values)
            {
                var disposable = s as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}