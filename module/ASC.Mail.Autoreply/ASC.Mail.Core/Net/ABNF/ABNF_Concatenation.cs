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
    /// This class represent ABNF "concatenation". Defined in RFC 5234 4.
    /// </summary>
    public class ABNF_Concatenation
    {
        #region Members

        private readonly List<ABNF_Repetition> m_pItems;

        #endregion

        #region Properties

        /// <summary>
        /// Gets concatenation items.
        /// </summary>
        public List<ABNF_Repetition> Items
        {
            get { return m_pItems; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ABNF_Concatenation()
        {
            m_pItems = new List<ABNF_Repetition>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static ABNF_Concatenation Parse(StringReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            // concatenation  =  repetition *(1*c-wsp repetition)
            // repetition     =  [repeat] element

            ABNF_Concatenation retVal = new ABNF_Concatenation();

            while (true)
            {
                ABNF_Repetition item = ABNF_Repetition.Parse(reader);
                if (item != null)
                {
                    retVal.m_pItems.Add(item);
                }
                    // We reached end of string.
                else if (reader.Peek() == -1)
                {
                    break;
                }
                    // We have next concatenation item.
                else if (reader.Peek() == ' ')
                {
                    reader.Read();
                }
                    // We have unexpected value, probably concatenation ends.
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