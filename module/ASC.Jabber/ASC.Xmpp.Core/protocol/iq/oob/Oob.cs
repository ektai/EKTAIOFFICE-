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


using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.iq.oob
{
    //	<iq type="set" to="horatio@denmark" from="sailor@sea" id="i_oob_001">
    //		<query xmlns="jabber:iq:oob">
    //			<url>http://denmark/act4/letter-1.html</url>
    //			<desc>There's a letter for you sir.</desc>
    //		</query>
    // </iq>	

    /// <summary>
    ///   Zusammenfassung f�r Oob.
    /// </summary>
    public class Oob : Element
    {
        public Oob()
        {
            TagName = "query";
            Namespace = Uri.IQ_OOB;
        }

        public string Url
        {
            set { SetTag("url", value); }
            get { return GetTag("url"); }
        }

        public string Description
        {
            set { SetTag("desc", value); }
            get { return GetTag("desc"); }
        }
    }
}