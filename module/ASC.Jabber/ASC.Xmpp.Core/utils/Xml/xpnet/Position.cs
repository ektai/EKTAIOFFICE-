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


#region using

using System;

#endregion

namespace ASC.Xmpp.Core.utils.Xml.xpnet
{

    #region usings

    #endregion

    /**
 * Represents a position in an entity.
 * A position can be modified by <code>Encoding.movePosition</code>.
 * @see Encoding#movePosition
 * @version $Revision: 1.2 $ $Date: 1998/02/17 04:24:15 $
 */

    /// <summary>
    /// </summary>
    public class Position : ICloneable
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Position()
        {
            LineNumber = 1;
            ColumnNumber = 0;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public int ColumnNumber { get; set; }

        /// <summary>
        /// </summary>
        public int LineNumber { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        /// <exception cref="NotImplementedException"></exception>
        public object Clone()
        {
#if CF
	  throw new util.NotImplementedException();
#else
            throw new NotImplementedException();
#endif
        }

        #endregion

        /**
   * Creates a position for the start of an entity: the line number is
   * 1 and the column number is 0.
   */

        /**
   * Returns the line number.
   * The first line number is 1.
   */

        /**
   * Returns the column number.
   * The first column number is 0.
   * A tab character is not treated specially.
   */

        /**
   * Returns a copy of this position.
   */
    }
}