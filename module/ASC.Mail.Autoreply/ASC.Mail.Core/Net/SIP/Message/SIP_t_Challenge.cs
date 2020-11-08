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


namespace ASC.Mail.Net.SIP.Message
{
    #region usings

    using System;

    #endregion

    /// <summary>
    /// Implements SIP "challenge" value. Defined in RFC 3261.
    /// </summary>
    /// <remarks>
    /// <code>
    /// RFC 3261 Syntax:
    ///     challenge = ("Digest" LWS digest-cln *(COMMA digest-cln)) / other-challenge
    /// </code>
    /// </remarks>
    public class SIP_t_Challenge : SIP_t_Value
    {
        #region Members

        private string m_AuthData = "";
        private string m_Method = "";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets authentication method. Normally this value is always 'Digest'.
        /// </summary>
        public string Method
        {
            get { return m_Method; }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Property Method value cant be null or mepty !");
                }

                m_Method = value;
            }
        }

        /// <summary>
        /// Gets or sets authentication data. That value depends on authentication type.
        /// </summary>
        public string AuthData
        {
            get { return m_AuthData; }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Property AuthData value cant be null or mepty !");
                }

                m_AuthData = value;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="value">SIP challenge value.</param>
        public SIP_t_Challenge(string value)
        {
            Parse(new StringReader(value));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parses "challenge" from specified value.
        /// </summary>
        /// <param name="value">SIP "challenge" value.</param>
        /// <exception cref="ArgumentNullException">Raised when <b>value</b> is null.</exception>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        public void Parse(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            Parse(new StringReader(value));
        }

        /// <summary>
        /// Parses "challenge" from specified reader.
        /// </summary>
        /// <param name="reader">Reader what contains challenge value.</param>
        /// <exception cref="ArgumentNullException">Raised when <b>reader</b> is null.</exception>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        public override void Parse(StringReader reader)
        {
            // challenge = ("Digest" LWS digest-cln *(COMMA digest-cln)) / other-challenge

            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            // Get authentication method
            string word = reader.ReadWord();
            if (word == null)
            {
                throw new SIP_ParseException(
                    "Invalid WWW-Authenticate: value, authentication method is missing !");
            }
            m_Method = word;

            // Get authentication data
            word = reader.ReadToEnd();
            if (word == null)
            {
                throw new SIP_ParseException(
                    "Invalid WWW-Authenticate: value, authentication parameters are missing !");
            }
            m_AuthData = word.Trim();
        }

        /// <summary>
        /// Converts this to valid "challenge" value.
        /// </summary>
        /// <returns>Returns "challenge" value.</returns>
        public override string ToStringValue()
        {
            return m_Method + " " + m_AuthData;
        }

        #endregion
    }
}