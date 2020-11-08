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


namespace ASC.Mail.Net.SIP.UA
{
    /// <summary>
    /// This enum specifies SIP UA call states.
    /// </summary>
    public enum SIP_UA_CallState
    {
        /// <summary>
        /// Outgoing call waits to be started.
        /// </summary>
        WaitingForStart,

        /// <summary>
        /// Outgoing calling is in progress.
        /// </summary>
        Calling,

        /// <summary>
        /// Outgoing call remote end party is ringing.
        /// </summary>
        Ringing,

        /// <summary>
        /// Outgoing call remote end pary queued a call.
        /// </summary>
        Queued,

        /// <summary>
        /// Incoming call waits to be accepted.
        /// </summary>
        WaitingToAccept,

        /// <summary>
        /// Call is active.
        /// </summary>
        Active,

        /// <summary>
        /// Call is terminating.
        /// </summary>
        Terminating,

        /// <summary>
        /// Call is terminated.
        /// </summary>
        Terminated,

        /// <summary>
        /// Call has disposed.
        /// </summary>
        Disposed
    }
}