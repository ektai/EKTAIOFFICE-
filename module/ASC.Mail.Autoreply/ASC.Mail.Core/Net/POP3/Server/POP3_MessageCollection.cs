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


namespace ASC.Mail.Net.POP3.Server
{
    #region usings

    using System.Collections;
    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// POP3 messages info collection.
    /// </summary>
    public class POP3_MessageCollection : IEnumerable
    {
        #region Members

        private readonly List<POP3_Message> m_pMessages;

        #endregion

        #region Properties

        /// <summary>
        /// Gets number of messages in the collection.
        /// </summary>
        public int Count
        {
            get { return m_pMessages.Count; }
        }

        /// <summary>
        /// Gets a POP3_Message object in the collection by index number.
        /// </summary>
        /// <param name="index">An Int32 value that specifies the position of the POP3_Message object in the POP3_MessageCollection collection.</param>
        public POP3_Message this[int index]
        {
            get { return m_pMessages[index]; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public POP3_MessageCollection()
        {
            m_pMessages = new List<POP3_Message>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds new message info to the collection.
        /// </summary>
        /// <param name="id">Message ID.</param>
        /// <param name="uid">Message UID.</param>
        /// <param name="size">Message size in bytes.</param>
        public POP3_Message Add(string id, string uid, long size)
        {
            return Add(id, uid, size, null);
        }

        /// <summary>
        /// Adds new message info to the collection.
        /// </summary>
        /// <param name="id">Message ID.</param>
        /// <param name="uid">Message UID.</param>
        /// <param name="size">Message size in bytes.</param>
        /// <param name="tag">Message user data.</param>
        public POP3_Message Add(string id, string uid, long size, object tag)
        {
            POP3_Message message = new POP3_Message(this, id, uid, size);
            m_pMessages.Add(message);
            message.Tag = tag;

            return message;
        }

        /// <summary>
        /// Removes specified message from the collection.
        /// </summary>
        /// <param name="message">Message to remove.</param>
        public void Remove(POP3_Message message)
        {
            m_pMessages.Remove(message);
        }

        /// <summary>
        /// Gets if collection contains message with the specified UID.
        /// </summary>
        /// <param name="uid">Message UID to check.</param>
        /// <returns></returns>
        public bool ContainsUID(string uid)
        {
            foreach (POP3_Message message in m_pMessages)
            {
                if (message.UID.ToLower() == uid.ToLower())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes all messages from the collection.
        /// </summary>
        public void Clear()
        {
            m_pMessages.Clear();
        }

        /// <summary>
        /// Checks if message exists. NOTE marked for delete messages returns false.
        /// </summary>
        /// <param name="sequenceNo">Message 1 based sequence number.</param>
        /// <returns></returns>
        public bool MessageExists(int sequenceNo)
        {
            if (sequenceNo > 0 && sequenceNo <= m_pMessages.Count)
            {
                if (!m_pMessages[sequenceNo - 1].MarkedForDelete)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets messages total sizes. NOTE messages marked for deletion is excluded.
        /// </summary>
        /// <returns></returns>
        public long GetTotalMessagesSize()
        {
            long totalSize = 0;
            foreach (POP3_Message msg in m_pMessages)
            {
                if (!msg.MarkedForDelete)
                {
                    totalSize += msg.Size;
                }
            }

            return totalSize;
        }

        /// <summary>
        /// Resets deleted flags on all messages.
        /// </summary>
        public void ResetDeletedFlag()
        {
            foreach (POP3_Message msg in m_pMessages)
            {
                msg.MarkedForDelete = false;
            }
        }

        /// <summary>
        /// Gets enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return m_pMessages.GetEnumerator();
        }

        #endregion
    }
}