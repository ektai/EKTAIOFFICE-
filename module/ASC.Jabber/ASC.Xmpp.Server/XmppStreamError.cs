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


using ASC.Xmpp.Core.protocol;

namespace ASC.Xmpp.Server
{
	public static class XmppStreamError
	{
		public static Error BadFormat
		{
			get { return new Error(StreamErrorCondition.BadFormat) { Prefix = Uri.PREFIX }; }
		}

		public static Error Conflict
		{
			get { return new Error(StreamErrorCondition.Conflict) { Prefix = Uri.PREFIX }; }
		}


        public static Error HostGone
        {
            get { return new Error(StreamErrorCondition.HostGone) { Prefix = Uri.PREFIX }; }
        }

		public static Error HostUnknown
		{
			get { return new Error(StreamErrorCondition.HostUnknown) { Prefix = Uri.PREFIX }; }
		}

		public static Error BadNamespacePrefix
		{
			get { return new Error(StreamErrorCondition.BadNamespacePrefix) { Prefix = Uri.PREFIX }; }
		}

		public static Error NotAuthorized
		{
			get { return new Error(StreamErrorCondition.NotAuthorized) { Prefix = Uri.PREFIX }; }
		}

		public static Error InvalidFrom
		{
			get { return new Error(StreamErrorCondition.InvalidFrom) { Prefix = Uri.PREFIX }; }
		}

		public static Error ImproperAddressing
		{
			get { return new Error(StreamErrorCondition.ImproperAddressing) { Prefix = Uri.PREFIX }; }
		}

		public static Error InternalServerError
		{
			get { return new Error(StreamErrorCondition.InternalServerError) { Prefix = Uri.PREFIX }; }
		}
	}
}
