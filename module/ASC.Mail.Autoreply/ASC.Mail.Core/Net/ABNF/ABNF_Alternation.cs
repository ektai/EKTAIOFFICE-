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
    using System.Collections.Generic;
    using System.IO;

    #endregion

    /// <summary>
    /// This class represent ABNF "alternation". Defined in RFC 5234 4.
    /// </summary>
    public class ABNF_Alternation
    {
        #region Members

        private readonly List<ABNF_Concatenation> m_pItems;

        #endregion

        #region Properties

        /// <summary>
        /// Gets alternation items.
        /// </summary>
        public List<ABNF_Concatenation> Items
        {
            get { return m_pItems; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ABNF_Alternation()
        {
            m_pItems = new List<ABNF_Concatenation>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static ABNF_Alternation Parse(StringReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            // alternation = concatenation *(*c-wsp "/" *c-wsp concatenation)

            ABNF_Alternation retVal = new ABNF_Alternation();

            while (true)
            {
                ABNF_Concatenation item = ABNF_Concatenation.Parse(reader);
                if (item != null)
                {
                    retVal.m_pItems.Add(item);
                }

                // We reached end of string.
                if (reader.Peek() == -1)
                {
                    break;
                }
                    // We have next alternation item.
                else if (reader.Peek() == '/')
                {
                    reader.Read();
                }
                    // We have unexpected value, probably alternation ends.
                else
                {
                    break;
                }
            }

            return retVal;
        }

        #endregion
    }
}