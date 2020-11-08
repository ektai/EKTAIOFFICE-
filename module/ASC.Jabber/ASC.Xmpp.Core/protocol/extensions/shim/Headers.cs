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

namespace ASC.Xmpp.Core.protocol.extensions.shim
{
    /// <summary>
    ///   JEP-0131: Stanza Headers and Internet Metadata (SHIM)
    /// </summary>
    public class Headers : Element
    {
        // <headers xmlns='http://jabber.org/protocol/shim'>
        //	 <header name='In-Reply-To'>123456789@capulet.com</header>
        // <header name='Keywords'>shakespeare,&lt;xmpp/&gt;</header>
        // </headers>
        public Headers()
        {
            TagName = "headers";
            Namespace = Uri.SHIM;
        }

        /// <summary>
        ///   Adds a new Header
        /// </summary>
        /// <returns> </returns>
        public Header AddHeader()
        {
            var h = new Header();
            AddChild(h);
            return h;
        }

        /// <summary>
        ///   Adds the given Header
        /// </summary>
        /// <param name="header"> </param>
        /// <returns> returns the given Header </returns>
        public Header AddHeader(Header header)
        {
            AddChild(header);
            return header;
        }

        /// <summary>
        ///   Adds a new Header
        /// </summary>
        /// <param name="name"> header name </param>
        /// <param name="val"> header value </param>
        /// <returns> returns the new added header </returns>
        public Header AddHeader(string name, string val)
        {
            var header = new Header(name, val);
            AddChild(header);
            return header;
        }

        public void SetHeader(string name, string val)
        {
            Header header = GetHeader(name);
            if (header != null)
                header.Value = val;
            else
                AddHeader(name, val);
        }

        /// <summary>
        /// </summary>
        /// <param name="name"> </param>
        public Header GetHeader(string name)
        {
            return (Header) SelectSingleElement("header", "name", name);
        }

        public Header[] GetHeaders()
        {
            ElementList nl = SelectElements("header");
            var headers = new Header[nl.Count];

            int i = 0;
            foreach (Element e in nl)
            {
                headers[i] = (Header) e;
                i++;
            }
            return headers;
        }
    }
}