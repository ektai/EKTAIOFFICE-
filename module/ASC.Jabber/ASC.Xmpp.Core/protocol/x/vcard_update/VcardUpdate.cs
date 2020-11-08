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

using ASC.Xmpp.Core.utils.Xml.Dom;

#endregion

namespace ASC.Xmpp.Core.protocol.x.vcard_update
{

    #region usings

    #endregion

    /*
        <presence>
          <x xmlns='vcard-temp:x:update'>
            <photo/>
          </x>
        </presence>
    */

    /// <summary>
    /// </summary>
    public class VcardUpdate : Element
    {
        #region Constructor

        /// <summary>
        ///   Initializes a new instance of the <see cref="VcardUpdate" /> class.
        /// </summary>
        public VcardUpdate()
        {
            TagName = "x";
            Namespace = Uri.VCARD_UPDATE;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="VcardUpdate" /> class.
        /// </summary>
        /// <param name="photo"> The photo. </param>
        public VcardUpdate(string photo) : this()
        {
            Photo = photo;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   SHA1 hash of the avatar image data <para>if no image/avatar should be advertised, or other clients should be forced
        ///                                        to remove the image set it to a empty string value ("")</para> <para>if this protocol is supported but you ae not ready o advertise a imaeg yet
        ///                                                                                                         set teh value to null.</para> <para>Otherwise teh value must the SHA1 hash of the image data.</para>
        /// </summary>
        public string Photo
        {
            get { return GetTag("photo"); }

            set
            {
                if (value == null)
                {
                    RemoveTag("photo");
                }
                else
                {
                    SetTag("photo", value);
                }
            }
        }

        #endregion
    }
}