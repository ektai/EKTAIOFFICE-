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

namespace ASC.Xmpp.Core.protocol.iq.agent
{
    //	<agent jid="conference.myjabber.net"><name>Public Conferencing</name><service>public</service></agent>
    //	<agent jid="aim.myjabber.net"><name>AIM Transport</name><service>aim</service><transport>Enter ID</transport><register/></agent>
    //	<agent jid="yahoo.myjabber.net"><name>Yahoo! Transport</name><service>yahoo</service><transport>Enter ID</transport><register/></agent>
    //	<agent jid="icq.myjabber.net"><name>ICQ Transport</name><service>icq</service><transport>Enter ID</transport><register/></agent>
    //	<agent jid="msn.myjabber.net"><name>MSN Transport</name><service>msn</service><transport>Enter ID</transport><register/></agent>

    /// <summary>
    ///   Zusammenfassung f�r Agent.
    /// </summary>
    public class Agent : Element
    {
        public Agent()
        {
            TagName = "agent";
            Namespace = Uri.IQ_AGENTS;
        }

        public Jid Jid
        {
            get { return new Jid(GetAttribute("jid")); }
            set { SetAttribute("jid", value.ToString()); }
        }

        public string Name
        {
            get { return GetTag("name"); }
            set { SetTag("name", value); }
        }

        public string Service
        {
            get { return GetTag("service"); }
            set { SetTag("service", value); }
        }

        public string Description
        {
            get { return GetTag("description"); }
            set { SetTag("description", value); }
        }

        /// <summary>
        ///   Can we register this agent/transport
        /// </summary>
        public bool CanRegister
        {
            get { return HasTag("register"); }
            set
            {
                if (value)
                    SetTag("register");
                else
                    RemoveTag("register");
            }
        }

        /// <summary>
        ///   Can we search thru this agent/transport
        /// </summary>
        public bool CanSearch
        {
            get { return HasTag("search"); }
            set
            {
                if (value)
                    SetTag("search");
                else
                    RemoveTag("search");
            }
        }

        /// <summary>
        ///   Is this agent a transport?
        /// </summary>
        public bool IsTransport
        {
            get { return HasTag("transport"); }
            set
            {
                if (value)
                    SetTag("transport");
                else
                    RemoveTag("transport");
            }
        }

        /// <summary>
        ///   Is this agent for groupchat
        /// </summary>
        public bool IsGroupchat
        {
            get { return HasTag("groupchat"); }
            set
            {
                if (value)
                    SetTag("groupchat");
                else
                    RemoveTag("groupchat");
            }
        }
    }
}