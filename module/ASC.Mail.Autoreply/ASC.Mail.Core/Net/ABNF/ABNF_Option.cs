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


namespace ASC.Mail.Net.ABNF
{
    #region usings

    using System;
    using System.IO;

    #endregion

    /// <summary>
    /// This class represent ABNF "option". Defined in RFC 5234 4.
    /// </summary>
    public class ABNF_Option : ABNF_Element
    {
        #region Members

        private ABNF_Alternation m_pAlternation;

        #endregion

        #region Properties

        /// <summary>
        /// Gets option alternation elements.
        /// </summary>
        public ABNF_Alternation Alternation
        {
            get { return m_pAlternation; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static ABNF_Option Parse(StringReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            // option = "[" *c-wsp alternation *c-wsp "]"

            if (reader.Peek() != '[')
            {
                throw new ParseException("Invalid ABNF 'option' value '" + reader.ReadToEnd() + "'.");
            }

            // Eat "[".
            reader.Read();

            // TODO: *c-wsp

            ABNF_Option retVal = new ABNF_Option();

            // We reached end of stream, no closing "]".
            if (reader.Peek() == -1)
            {
                throw new ParseException("Invalid ABNF 'option' value '" + reader.ReadToEnd() + "'.");
            }

            retVal.m_pAlternation = ABNF_Alternation.Parse(reader);

            // We don't have closing ")".
            if (reader.Peek() != ']')
            {
                throw new ParseException("Invalid ABNF 'option' value '" + reader.ReadToEnd() + "'.");
            }
            else
            {
                reader.Read();
            }

            return retVal;
        }

        #endregion
    }
}