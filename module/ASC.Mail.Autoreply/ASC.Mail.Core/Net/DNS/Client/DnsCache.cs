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


namespace ASC.Mail.Net.Dns.Client
{
    #region usings

    using System;
    using System.Collections;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    #endregion

    #region struct CacheEntry

    /// <summary>
    /// Dns cache entry.
    /// </summary>
    [Serializable]
    internal struct DnsCacheEntry
    {
        #region Members

        private readonly DnsServerResponse m_pResponse;
        private readonly DateTime m_Time;

        #endregion

        #region Properties

        /// <summary>
        /// Gets dns answers.
        /// </summary>
        public DnsServerResponse Answers
        {
            get { return m_pResponse; }
        }

        /// <summary>
        /// Gets entry add time.
        /// </summary>
        public DateTime Time
        {
            get { return m_Time; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="answers">Dns answers.</param>
        /// <param name="addTime">Entry add time.</param>
        public DnsCacheEntry(DnsServerResponse answers, DateTime addTime)
        {
            m_pResponse = answers;
            m_Time = addTime;
        }

        #endregion
    }

    #endregion

    /// <summary>
    /// This class implements dns query cache.
    /// </summary>
    public class DnsCache
    {
        #region Members

        private static long m_CacheTime = 10000;
        private static Hashtable m_pCache;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets how long(seconds) to cache dns query.
        /// </summary>
        public static long CacheTime
        {
            get { return m_CacheTime; }

            set { m_CacheTime = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        static DnsCache()
        {
            m_pCache = new Hashtable();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Tries to get dns records from cache, if any.
        /// </summary>
        /// <param name="qname"></param>
        /// <param name="qtype"></param>
        /// <returns>Returns null if not in cache.</returns>
        public static DnsServerResponse GetFromCache(string qname, int qtype)
        {
            try
            {
                if (m_pCache.Contains(qname + qtype))
                {
                    DnsCacheEntry entry = (DnsCacheEntry) m_pCache[qname + qtype];

                    // If cache object isn't expired
                    if (entry.Time.AddSeconds(m_CacheTime) > DateTime.Now)
                    {
                        return entry.Answers;
                    }
                }
            }
            catch {}

            return null;
        }

        /// <summary>
        /// Adds dns records to cache. If old entry exists, it is replaced.
        /// </summary>
        /// <param name="qname"></param>
        /// <param name="qtype"></param>
        /// <param name="answers"></param>
        public static void AddToCache(string qname, int qtype, DnsServerResponse answers)
        {
            if (answers == null)
            {
                return;
            }

            try
            {
                lock (m_pCache)
                {
                    // Remove old cache entry, if any.
                    if (m_pCache.Contains(qname + qtype))
                    {
                        m_pCache.Remove(qname + qtype);
                    }
                    m_pCache.Add(qname + qtype, new DnsCacheEntry(answers, DateTime.Now));
                }
            }
            catch {}
        }

        /// <summary>
        /// Clears DNS cache.
        /// </summary>
        public static void ClearCache()
        {
            lock (m_pCache)
            {
                m_pCache.Clear();
            }
        }

        /// <summary>
        /// Serializes current cache.
        /// </summary>
        /// <returns>Return serialized cache.</returns>
        public static byte[] SerializeCache()
        {
            lock (m_pCache)
            {
                MemoryStream retVal = new MemoryStream();

                BinaryFormatter b = new BinaryFormatter();
                b.Serialize(retVal, m_pCache);

                return retVal.ToArray();
            }
        }

        /// <summary>
        /// DeSerializes stored cache.
        /// </summary>
        /// <param name="cacheData">This value must be DnsCache.SerializeCache() method value.</param>
        public static void DeSerializeCache(byte[] cacheData)
        {
            lock (m_pCache)
            {
                MemoryStream retVal = new MemoryStream(cacheData);

                BinaryFormatter b = new BinaryFormatter();
                m_pCache = (Hashtable) b.Deserialize(retVal);
            }
        }

        #endregion
    }
}