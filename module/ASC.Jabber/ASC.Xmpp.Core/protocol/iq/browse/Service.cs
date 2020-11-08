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

namespace ASC.Xmpp.Core.protocol.iq.browse
{
    /// <summary>
    ///   Historically each category was used as the name of an element, and the type was an attribute, such as &lt;service type="aim"/&gt;. The proper expression for all new implementations supporting this specification is to express the type information as attributes on a generic item element: &lt;item category="service" type="aim"/&gt;. When processing returned browse information this new syntax should always be handled first, and the old syntax only used if it is important to be able to access older implementations. Additional unofficial categories or types may be specified by prefixing their name with an "x-", such as "service/x-virgeim" or "x-location/gps". Changes to the official categories and subtypes may be defined either by revising this JEP or by activating another JEP. Removal of a category or subtype must be noted in this document.
    /// </summary>
    public class Service : Element
    {
        /*
		<iq from="myjabber.net" xmlns="jabber:client" id="agsXMPP_5" type="result" to="gnauck@myjabber.net/myJabber v3.5">

			<service name="myJabber Server" jid="myjabber.net" type="jabber" xmlns="jabber:iq:browse"> 
				
				<item version="0.6.0" name="Public Conferencing" jid="conference.myjabber.net" type="public" category="conference"> 
					<ns>http://jabber.org/protocol/muc</ns> 
				</item> 

				<service name="AIM Transport" jid="aim.myjabber.net" type="aim"> 
					<ns>jabber:iq:gateway</ns> 
					<ns>jabber:iq:register</ns> 
				</service> 

				<service name="Yahoo! Transport" jid="yahoo.myjabber.net" type="yahoo"> 
					<ns>jabber:iq:gateway</ns> 
					<ns>jabber:iq:register</ns> 
				</service> 

				<service name="ICQ Transport" jid="icq.myjabber.net" type="icq"> 
					<ns>jabber:iq:gateway</ns> 
					<ns>jabber:iq:register</ns> 
				</service> 

				<service name="MSN Transport" jid="msn.myjabber.net" type="msn"> 
					<ns>jabber:iq:gateway</ns> 
					<ns>jabber:iq:register</ns> 
				</service> 

				<item name="Online Users" jid="myjabber.net/admin"/>				
				<ns>jabber:iq:admin</ns>
			</service>
		</iq> 
		*/

        public Service()
        {
            TagName = "service";
            Namespace = Uri.IQ_BROWSE;
        }


        public string Name
        {
            get { return GetAttribute("name"); }
            set { SetAttribute("name", value); }
        }

        public Jid Jid
        {
            get { return new Jid(GetAttribute("jid")); }
            set { SetAttribute("jid", value.ToString()); }
        }

        public string Type
        {
            get { return GetAttribute("type"); }
            set { SetAttribute("type", value); }
        }

        /// <summary>
        ///   Gets all advertised namespaces of this service
        /// </summary>
        /// <returns> string array that contains the advertised namespaces </returns>
        public string[] GetNamespaces()
        {
            ElementList elements = SelectElements("ns");
            var nss = new string[elements.Count];

            int i = 0;
            foreach (Element ns in elements)
            {
                nss[i] = ns.Value;
                i++;
            }

            return nss;
        }

        public BrowseItem[] GetItems()
        {
            ElementList nl = SelectElements(typeof (BrowseItem));
            var items = new BrowseItem[nl.Count];
            int i = 0;
            foreach (Element item in nl)
            {
                items[i] = item as BrowseItem;
                i++;
            }
            return items;
        }

        /// <summary>
        ///   Gets all "ChilsServices" od this service
        /// </summary>
        /// <returns> </returns>
        public Service[] GetServices()
        {
            ElementList nl = SelectElements(typeof (Service));
            var Services = new Service[nl.Count];
            int i = 0;
            foreach (Element service in nl)
            {
                Services[i] = service as Service;
                i++;
            }
            return Services;
        }
    }
}