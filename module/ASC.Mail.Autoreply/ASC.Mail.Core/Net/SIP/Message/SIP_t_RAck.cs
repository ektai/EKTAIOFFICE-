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
    /// Implements SIP "RAck" value. Defined in RFC 3262.
    /// </summary>
    /// <remarks>
    /// <code>
    /// RFC 3262 Syntax:
    ///     RAck         = response-num LWS CSeq-num LWS Method
    ///     response-num = 1*DIGIT
    ///     CSeq-num     = 1*DIGIT
    /// </code>
    /// </remarks>
    public class SIP_t_RAck : SIP_t_Value
    {
        #region Members

        private int m_CSeqNumber = 1;
        private string m_Method = "";
        private int m_ResponseNumber = 1;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets response number.
        /// </summary>
        public int ResponseNumber
        {
            get { return m_ResponseNumber; }

            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("ResponseNumber value must be >= 1 !");
                }

                m_ResponseNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets CSeq number.
        /// </summary>
        public int CSeqNumber
        {
            get { return m_CSeqNumber; }

            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("CSeqNumber value must be >= 1 !");
                }

                m_CSeqNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets method.
        /// </summary>
        public string Method
        {
            get { return m_Method; }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Method");
                }

                m_Method = value;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="value">RAck value.</param>
        public SIP_t_RAck(string value)
        {
            Parse(value);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="responseNo">Response number.</param>
        /// <param name="cseqNo">CSeq number.</param>
        /// <param name="method">Request method.</param>
        public SIP_t_RAck(int responseNo, int cseqNo, string method)
        {
            ResponseNumber = responseNo;
            CSeqNumber = cseqNo;
            Method = method;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parses "RAck" from specified value.
        /// </summary>
        /// <param name="value">SIP "RAck" value.</param>
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
        /// Parses "RAck" from specified reader.
        /// </summary>
        /// <param name="reader">Reader from where to parse.</param>
        /// <exception cref="ArgumentNullException">Raised when <b>reader</b> is null.</exception>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        public override void Parse(StringReader reader)
        {
            /*
                RAck         = response-num LWS CSeq-num LWS Method
                response-num = 1*DIGIT
                CSeq-num     = 1*DIGIT
            */

            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            // response-num
            string word = reader.ReadWord();
            if (word == null)
            {
                throw new SIP_ParseException("RAck response-num value is missing !");
            }
            try
            {
                m_ResponseNumber = Convert.ToInt32(word);
            }
            catch
            {
                throw new SIP_ParseException("Invalid RAck response-num value !");
            }

            // CSeq-num
            word = reader.ReadWord();
            if (word == null)
            {
                throw new SIP_ParseException("RAck CSeq-num value is missing !");
            }
            try
            {
                m_CSeqNumber = Convert.ToInt32(word);
            }
            catch
            {
                throw new SIP_ParseException("Invalid RAck CSeq-num value !");
            }

            // Get request method
            word = reader.ReadWord();
            if (word == null)
            {
                throw new SIP_ParseException("RAck Method value is missing !");
            }
            m_Method = word;
        }

        /// <summary>
        /// Converts this to valid "RAck" value.
        /// </summary>
        /// <returns>Returns "RAck" value.</returns>
        public override string ToStringValue()
        {
            /*
                RAck         = response-num LWS CSeq-num LWS Method
                response-num = 1*DIGIT
                CSeq-num     = 1*DIGIT
            */

            return m_ResponseNumber + " " + m_CSeqNumber + " " + m_Method;
        }

        #endregion
    }
}