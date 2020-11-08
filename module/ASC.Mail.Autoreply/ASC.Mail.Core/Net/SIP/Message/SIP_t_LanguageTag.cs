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
    using System.Text;

    #endregion

    /// <summary>
    /// Implements SIP "language-tag" value. Defined in RFC 3261.
    /// </summary>
    /// <remarks>
    /// <code>
    /// RFC 3261 Syntax:
    ///     language-tag = primary-tag *( "-" subtag )
    ///     primary-tag  = 1*8ALPHA
    ///     subtag       = 1*8ALPHA
    /// </code>
    /// </remarks>
    public class SIP_t_LanguageTag : SIP_t_ValueWithParams
    {
        #region Members

        private string m_LanguageTag = "";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets language tag.
        /// </summary>
        public string LanguageTag
        {
            get { return m_LanguageTag; }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Property LanguageTag value can't be null or empty !");
                }

                m_LanguageTag = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parses "language-tag" from specified value.
        /// </summary>
        /// <param name="value">SIP "language-tag" value.</param>
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
        /// Parses "language-tag" from specified reader.
        /// </summary>
        /// <param name="reader">Reader from where to parse.</param>
        /// <exception cref="ArgumentNullException">Raised when <b>reader</b> is null.</exception>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        public override void Parse(StringReader reader)
        {
            /* 
                Content-Language =  "Content-Language" HCOLON language-tag *(COMMA language-tag)
                language-tag     =  primary-tag *( "-" subtag )
                primary-tag      =  1*8ALPHA
                subtag           =  1*8ALPHA
            */

            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            // Parse content-coding
            string word = reader.ReadWord();
            if (word == null)
            {
                throw new SIP_ParseException("Invalid Content-Language value, language-tag value is missing !");
            }
            m_LanguageTag = word;

            // Parse parameters
            ParseParameters(reader);
        }

        /// <summary>
        /// Converts this to valid "language-tag" value.
        /// </summary>
        /// <returns>Returns "language-tag" value.</returns>
        public override string ToStringValue()
        {
            /* 
                Content-Language =  "Content-Language" HCOLON language-tag *(COMMA language-tag)
                language-tag     =  primary-tag *( "-" subtag )
                primary-tag      =  1*8ALPHA
                subtag           =  1*8ALPHA
            */

            StringBuilder retVal = new StringBuilder();
            retVal.Append(m_LanguageTag);
            retVal.Append(ParametersToString());

            return retVal.ToString();
        }

        #endregion
    }
}