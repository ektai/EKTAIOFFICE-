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
    /// Implements SIP "To" value. Defined in RFC 3261.
    /// The To header field specifies the logical recipient of the request.
    /// </summary>
    /// <remarks>
    /// <code>
    /// RFC 3261 Syntax:
    ///     To        = ( name-addr / addr-spec ) *( SEMI to-param )
    ///     to-param  = tag-param / generic-param
    ///     tag-param = "tag" EQUAL token
    /// </code>
    /// </remarks>
    public class SIP_t_To : SIP_t_ValueWithParams
    {
        #region Members

        private readonly SIP_t_NameAddress m_pAddress;

        #endregion

        #region Properties

        /// <summary>
        /// Gets to address.
        /// </summary>
        public SIP_t_NameAddress Address
        {
            get { return m_pAddress; }
        }

        /// <summary>
        /// Gets or sets tag parameter value.
        /// The "tag" parameter serves as a general mechanism for dialog identification.
        /// Value null means that 'tag' paramter doesn't exist.
        /// </summary>
        public string Tag
        {
            get
            {
                SIP_Parameter parameter = Parameters["tag"];
                if (parameter != null)
                {
                    return parameter.Value;
                }
                else
                {
                    return null;
                }
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Parameters.Remove("tag");
                }
                else
                {
                    Parameters.Set("tag", value);
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="value">To: header field value.</param>
        public SIP_t_To(string value)
        {
            m_pAddress = new SIP_t_NameAddress();

            Parse(new StringReader(value));
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="address">To address.</param>
        public SIP_t_To(SIP_t_NameAddress address)
        {
            m_pAddress = address;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parses "To" from specified value.
        /// </summary>
        /// <param name="value">SIP "To" value.</param>
        /// <exception cref="ArgumentNullException">Raised when <b>reader</b> is null.</exception>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        public void Parse(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("reader");
            }

            Parse(new StringReader(value));
        }

        /// <summary>
        /// Parses "To" from specified reader.
        /// </summary>
        /// <param name="reader">Reader from where to parse.</param>
        /// <exception cref="ArgumentNullException">Raised when <b>reader</b> is null.</exception>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        public override void Parse(StringReader reader)
        {
            /* To       =  ( name-addr / addr-spec ) *( SEMI to-param )
               to-param =  tag-param / generic-param
            */

            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            // Parse address
            m_pAddress.Parse(reader);

            // Parse parameters
            ParseParameters(reader);
        }

        /// <summary>
        /// Converts this to valid "To" value.
        /// </summary>
        /// <returns>Returns "To" value.</returns>
        public override string ToStringValue()
        {
            StringBuilder retVal = new StringBuilder();
            retVal.Append(m_pAddress.ToStringValue());
            retVal.Append(ParametersToString());

            return retVal.ToString();
        }

        #endregion
    }
}