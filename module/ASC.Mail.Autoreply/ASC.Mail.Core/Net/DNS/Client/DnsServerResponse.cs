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


namespace ASC.Mail.Net.Dns.Client
{
    #region usings

    using System;
    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// This class represents dns server response.
    /// </summary>
    [Serializable]
    public class DnsServerResponse
    {
        #region Members

        private readonly List<DNS_rr_base> m_pAdditionalAnswers;
        private readonly List<DNS_rr_base> m_pAnswers;
        private readonly List<DNS_rr_base> m_pAuthoritiveAnswers;
        private readonly RCODE m_RCODE = RCODE.NO_ERROR;
        private readonly bool m_Success = true;

        #endregion

        #region Properties

        /// <summary>
        /// Gets if connection to dns server was successful.
        /// </summary>
        public bool ConnectionOk
        {
            get { return m_Success; }
        }

        /// <summary>
        /// Gets dns server response code.
        /// </summary>
        public RCODE ResponseCode
        {
            get { return m_RCODE; }
        }

        /// <summary>
        /// Gets all resource records returned by server (answer records section + authority records section + additional records section). 
        /// NOTE: Before using this property ensure that ConnectionOk=true and ResponseCode=RCODE.NO_ERROR.
        /// </summary>
        public DNS_rr_base[] AllAnswers
        {
            get
            {
                List<DNS_rr_base> retVal = new List<DNS_rr_base>();
                retVal.AddRange(m_pAnswers.ToArray());
                retVal.AddRange(m_pAuthoritiveAnswers.ToArray());
                retVal.AddRange(m_pAdditionalAnswers.ToArray());

                return retVal.ToArray();
            }
        }

        /// <summary>
        /// Gets dns server returned answers. NOTE: Before using this property ensure that ConnectionOk=true and ResponseCode=RCODE.NO_ERROR.
        /// </summary>
        /// <code>
        /// // NOTE: DNS server may return diffrent record types even if you query MX.
        /// //       For example you query lumisoft.ee MX and server may response:	
        ///	//		 1) MX - mail.lumisoft.ee
        ///	//		 2) A  - lumisoft.ee
        ///	// 
        ///	//       Before casting to right record type, see what type record is !
        ///				
        /// 
        /// foreach(DnsRecordBase record in Answers){
        ///		// MX record, cast it to MX_Record
        ///		if(record.RecordType == QTYPE.MX){
        ///			MX_Record mx = (MX_Record)record;
        ///		}
        /// }
        /// </code>
        public DNS_rr_base[] Answers
        {
            get { return m_pAnswers.ToArray(); }
        }

        /// <summary>
        /// Gets name server resource records in the authority records section. NOTE: Before using this property ensure that ConnectionOk=true and ResponseCode=RCODE.NO_ERROR.
        /// </summary>
        public DNS_rr_base[] AuthoritiveAnswers
        {
            get { return m_pAuthoritiveAnswers.ToArray(); }
        }

        /// <summary>
        /// Gets resource records in the additional records section. NOTE: Before using this property ensure that ConnectionOk=true and ResponseCode=RCODE.NO_ERROR.
        /// </summary>
        public DNS_rr_base[] AdditionalAnswers
        {
            get { return m_pAdditionalAnswers.ToArray(); }
        }

        #endregion

        #region Constructor

        internal DnsServerResponse(bool connectionOk,
                                   RCODE rcode,
                                   List<DNS_rr_base> answers,
                                   List<DNS_rr_base> authoritiveAnswers,
                                   List<DNS_rr_base> additionalAnswers)
        {
            m_Success = connectionOk;
            m_RCODE = rcode;
            m_pAnswers = answers;
            m_pAuthoritiveAnswers = authoritiveAnswers;
            m_pAdditionalAnswers = additionalAnswers;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets IPv4 host addess records.
        /// </summary>
        /// <returns></returns>
        public DNS_rr_A[] GetARecords()
        {
            List<DNS_rr_A> retVal = new List<DNS_rr_A>();
            foreach (DNS_rr_base record in m_pAnswers)
            {
                if (record.RecordType == QTYPE.A)
                {
                    retVal.Add((DNS_rr_A) record);
                }
            }

            return retVal.ToArray();
        }

        /// <summary>
        /// Gets name server records.
        /// </summary>
        /// <returns></returns>
        public DNS_rr_NS[] GetNSRecords()
        {
            List<DNS_rr_NS> retVal = new List<DNS_rr_NS>();
            foreach (DNS_rr_base record in m_pAnswers)
            {
                if (record.RecordType == QTYPE.NS)
                {
                    retVal.Add((DNS_rr_NS) record);
                }
            }

            return retVal.ToArray();
        }

        /// <summary>
        /// Gets CNAME records.
        /// </summary>
        /// <returns></returns>
        public DNS_rr_CNAME[] GetCNAMERecords()
        {
            List<DNS_rr_CNAME> retVal = new List<DNS_rr_CNAME>();
            foreach (DNS_rr_base record in m_pAnswers)
            {
                if (record.RecordType == QTYPE.CNAME)
                {
                    retVal.Add((DNS_rr_CNAME) record);
                }
            }

            return retVal.ToArray();
        }

        /// <summary>
        /// Gets SOA records.
        /// </summary>
        /// <returns></returns>
        public DNS_rr_SOA[] GetSOARecords()
        {
            List<DNS_rr_SOA> retVal = new List<DNS_rr_SOA>();
            foreach (DNS_rr_base record in m_pAnswers)
            {
                if (record.RecordType == QTYPE.SOA)
                {
                    retVal.Add((DNS_rr_SOA) record);
                }
            }

            return retVal.ToArray();
        }

        /// <summary>
        /// Gets PTR records.
        /// </summary>
        /// <returns></returns>
        public DNS_rr_PTR[] GetPTRRecords()
        {
            List<DNS_rr_PTR> retVal = new List<DNS_rr_PTR>();
            foreach (DNS_rr_base record in m_pAnswers)
            {
                if (record.RecordType == QTYPE.PTR)
                {
                    retVal.Add((DNS_rr_PTR) record);
                }
            }

            return retVal.ToArray();
        }

        /// <summary>
        /// Gets HINFO records.
        /// </summary>
        /// <returns></returns>
        public DNS_rr_HINFO[] GetHINFORecords()
        {
            List<DNS_rr_HINFO> retVal = new List<DNS_rr_HINFO>();
            foreach (DNS_rr_base record in m_pAnswers)
            {
                if (record.RecordType == QTYPE.HINFO)
                {
                    retVal.Add((DNS_rr_HINFO) record);
                }
            }

            return retVal.ToArray();
        }

        /// <summary>
        /// Gets MX records.(MX records are sorted by preference, lower array element is prefered)
        /// </summary>
        /// <returns></returns>
        public DNS_rr_MX[] GetMXRecords()
        {
            List<DNS_rr_MX> mx = new List<DNS_rr_MX>();
            foreach (DNS_rr_base record in m_pAnswers)
            {
                if (record.RecordType == QTYPE.MX)
                {
                    mx.Add((DNS_rr_MX) record);
                }
            }

            // Sort MX records by preference.
            DNS_rr_MX[] retVal = mx.ToArray();
            Array.Sort(retVal);

            return retVal;
        }

        /// <summary>
        /// Gets text records.
        /// </summary>
        /// <returns></returns>
        public DNS_rr_TXT[] GetTXTRecords()
        {
            List<DNS_rr_TXT> retVal = new List<DNS_rr_TXT>();
            foreach (DNS_rr_base record in m_pAnswers)
            {
                if (record.RecordType == QTYPE.TXT)
                {
                    retVal.Add((DNS_rr_TXT) record);
                }
            }

            return retVal.ToArray();
        }

        /// <summary>
        /// Gets IPv6 host addess records.
        /// </summary>
        /// <returns></returns>
        public DNS_rr_A[] GetAAAARecords()
        {
            List<DNS_rr_A> retVal = new List<DNS_rr_A>();
            foreach (DNS_rr_base record in m_pAnswers)
            {
                if (record.RecordType == QTYPE.AAAA)
                {
                    retVal.Add((DNS_rr_A) record);
                }
            }

            return retVal.ToArray();
        }

        /// <summary>
        /// Gets SRV resource records.
        /// </summary>
        /// <returns></returns>
        public DNS_rr_SRV[] GetSRVRecords()
        {
            List<DNS_rr_SRV> retVal = new List<DNS_rr_SRV>();
            foreach (DNS_rr_base record in m_pAnswers)
            {
                if (record.RecordType == QTYPE.SRV)
                {
                    retVal.Add((DNS_rr_SRV) record);
                }
            }

            return retVal.ToArray();
        }

        /// <summary>
        /// Gets NAPTR resource records.
        /// </summary>
        /// <returns></returns>
        public DNS_rr_NAPTR[] GetNAPTRRecords()
        {
            List<DNS_rr_NAPTR> retVal = new List<DNS_rr_NAPTR>();
            foreach (DNS_rr_base record in m_pAnswers)
            {
                if (record.RecordType == QTYPE.NAPTR)
                {
                    retVal.Add((DNS_rr_NAPTR) record);
                }
            }

            return retVal.ToArray();
        }

        #endregion

        #region Utility methods

        /// <summary>
        /// Filters out specified type of records from answer.
        /// </summary>
        /// <param name="answers"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private List<DNS_rr_base> FilterRecordsX(List<DNS_rr_base> answers, QTYPE type)
        {
            List<DNS_rr_base> retVal = new List<DNS_rr_base>();
            foreach (DNS_rr_base record in answers)
            {
                if (record.RecordType == type)
                {
                    retVal.Add(record);
                }
            }

            return retVal;
        }

        #endregion
    }
}