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


namespace ASC.Mail.Net.IMAP.Server
{
    /// <summary>
    /// Provides data for SetFolderACL event.
    /// </summary>
    public class IMAP_SETACL_eArgs
    {
        #region Members

        private readonly IMAP_ACL_Flags m_ACL_Flags = IMAP_ACL_Flags.None;
        private readonly IMAP_Flags_SetType m_FlagsSetType = IMAP_Flags_SetType.Replace;
        private readonly string m_pFolderName = "";
        private readonly IMAP_Session m_pSession;
        private readonly string m_UserName = "";
        private string m_ErrorText = "";

        #endregion

        #region Properties

        /// <summary>
        /// Gets current IMAP session.
        /// </summary>
        public IMAP_Session Session
        {
            get { return m_pSession; }
        }

        /// <summary>
        /// Gets folder name which ACL to set.
        /// </summary>
        public string Folder
        {
            get { return m_pFolderName; }
        }

        /// <summary>
        /// Gets user name which ACL to set.
        /// </summary>
        public string UserName
        {
            get { return m_UserName; }
        }

        /// <summary>
        /// Gets how ACL flags must be stored.
        /// </summary>
        public IMAP_Flags_SetType FlagsSetType
        {
            get { return m_FlagsSetType; }
        }

        /// <summary>
        /// Gets ACL flags. NOTE: See this.FlagsSetType how to store flags.
        /// </summary>
        public IMAP_ACL_Flags ACL
        {
            get { return m_ACL_Flags; }
        }

        /// <summary>
        /// Gets or sets error text returned to connected client.
        /// </summary>
        public string ErrorText
        {
            get { return m_ErrorText; }

            set { m_ErrorText = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="session">Owner IMAP session.</param>
        /// <param name="folderName">Folder name which ACL to set.</param>
        /// <param name="userName">User name which ACL to set.</param>
        /// <param name="flagsSetType">Specifies how flags must be stored.</param>
        /// <param name="aclFlags">Flags which to store.</param>
        public IMAP_SETACL_eArgs(IMAP_Session session,
                                 string folderName,
                                 string userName,
                                 IMAP_Flags_SetType flagsSetType,
                                 IMAP_ACL_Flags aclFlags)
        {
            m_pSession = session;
            m_pFolderName = folderName;
            m_UserName = userName;
            m_FlagsSetType = flagsSetType;
            m_ACL_Flags = aclFlags;
        }

        #endregion
    }
}