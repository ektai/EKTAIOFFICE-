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
    /// <summary>
    /// SIP Warning Codes. Defined in RFC 3261 27.2.
    /// </summary>
    public class SIP_WarningCodes
    {
        #region Members

        /// <summary>
        /// One or more network protocols contained in the session description are not available.
        /// </summary>
        public const int x300_Incompatible_network_protocol = 300;

        /// <summary>
        /// One or more network address formats contained in the session description are not available.
        /// </summary>
        public const int x301_Incompatible_network_address_formats = 301;

        /// <summary>
        /// One or more transport protocols described in the session description are not available.
        /// </summary>
        public const int x302_Incompatible_network_address_formats = 302;

        /// <summary>
        /// One or more bandwidth measurement units contained in the session description were not understood.
        /// </summary>
        public const int x303_Incompatible_bandwidth_units = 303;

        /// <summary>
        /// One or more media types contained in the session description are not available.
        /// </summary>
        public const int x304_Media_type_not_available = 304;

        /// <summary>
        /// One or more media formats contained in the session description are not available.
        /// </summary>
        public const int x305_Incompatible_media_format = 305;

        /// <summary>
        /// One or more of the media attributes in the session description are not supported.
        /// </summary>
        public const int x306_Attribute_not_understood = 306;

        /// <summary>
        /// A parameter other than those listed above was not understood.
        /// </summary>
        public const int x307_Session_description_parameter_not_understood = 307;

        /// <summary>
        /// The site where the user is located does not support multicast.
        /// </summary>
        public const int x330_Multicast_not_available = 330;

        /// <summary>
        /// The site where the user is located does not support unicast communication 
        /// (usually due to the presence of a firewall).
        /// </summary>
        public const int x331_Unicast_not_available = 331;

        /// <summary>
        /// The bandwidth specified in the session description or defined by the media 
        /// exceeds that known to be available.
        /// </summary>
        public const int x370_Insufficient_bandwidth = 370;

        /// <summary>
        /// The warning text can include arbitrary information to be presented to a human user or logged. 
        /// A system receiving this warning MUST NOT take any automated action.
        /// </summary>
        public const int x399_Miscellaneous_warning = 399;

        #endregion
    }
}