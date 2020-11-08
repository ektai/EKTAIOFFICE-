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


namespace ASC.Mail.Net.FTP.Client
{
    #region usings

    using System;

    #endregion

    /// <summary>
    /// FTP client exception.
    /// </summary>
    public class FTP_ClientException : Exception
    {
        #region Members

        private readonly string m_ResponseText = "";
        private readonly int m_StatusCode = 500;

        #endregion

        #region Properties

        /// <summary>
        /// Gets FTP status code.
        /// </summary>
        public int StatusCode
        {
            get { return m_StatusCode; }
        }

        /// <summary>
        /// Gets FTP server response text after status code.
        /// </summary>
        public string ResponseText
        {
            get { return m_ResponseText; }
        }

        /// <summary>
        /// Gets if it is permanent FTP(5xx) error.
        /// </summary>
        public bool IsPermanentError
        {
            get
            {
                if (m_StatusCode >= 500 && m_StatusCode <= 599)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="responseLine">FTP server response line.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>responseLine</b> is null.</exception>
        public FTP_ClientException(string responseLine) : base(responseLine)
        {
            if (responseLine == null)
            {
                throw new ArgumentNullException("responseLine");
            }

            string[] code_text = responseLine.Split(new[] {' '}, 2);
            try
            {
                m_StatusCode = Convert.ToInt32(code_text[0]);
            }
            catch {}
            if (code_text.Length == 2)
            {
                m_ResponseText = code_text[1];
            }
        }

        #endregion
    }
}