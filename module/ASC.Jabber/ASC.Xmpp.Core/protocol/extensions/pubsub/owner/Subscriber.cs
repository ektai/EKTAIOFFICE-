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

namespace ASC.Xmpp.Core.protocol.extensions.pubsub.owner
{
    /*
        <iq type='result'
            from='pubsub.shakespeare.lit'
            to='hamlet@denmark.lit/elsinore'
            id='subman1'>
          <pubsub xmlns='http://jabber.org/protocol/pubsub#owner'>
            <subscribers node='blogs/princely_musings'>
              <subscriber jid='hamlet@denmark.lit' subscription='subscribed'/>
              <subscriber jid='polonius@denmark.lit' subscription='unconfigured'/>
            </subscribers>
          </pubsub>
        </iq>
     
        <xs:element name='subscriber'>
            <xs:complexType>
              <xs:simpleContent>
                <xs:extension base='empty'>
                  <xs:attribute name='subscription' use='required'>
                    <xs:simpleType>
                      <xs:restriction base='xs:NCName'>
                        <xs:enumeration value='none'/>
                        <xs:enumeration value='pending'/>
                        <xs:enumeration value='subscribed'/>
                        <xs:enumeration value='unconfigured'/>
                      </xs:restriction>
                    </xs:simpleType>
                  </xs:attribute>
                  <xs:attribute name='jid' type='xs:string' use='required'/>
                </xs:extension>
              </xs:simpleContent>
            </xs:complexType>
        </xs:element>
    */

    public class Subscriber : Element
    {
        #region << Constructors >>

        public Subscriber()
        {
            TagName = "subscriber";
            Namespace = Uri.PUBSUB_OWNER;
        }

        public Subscriber(Jid jid, SubscriptionState sub) : this()
        {
            Jid = jid;
            SubscriptionState = sub;
        }

        #endregion

        public SubscriptionState SubscriptionState
        {
            get { return (SubscriptionState) GetAttributeEnum("subscription", typeof (SubscriptionState)); }
            set { SetAttribute("subscription", value.ToString()); }
        }

        public Jid Jid
        {
            get
            {
                if (HasAttribute("jid"))
                    return new Jid(GetAttribute("jid"));
                else
                    return null;
            }
            set
            {
                if (value != null)
                    SetAttribute("jid", value.ToString());
            }
        }
    }
}