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


namespace ASC.Mail.Net.RTP
{
    #region usings

    using System;

    #endregion

    /// <summary>
    /// This class holds receiver report info.
    /// </summary>
    public class RTCP_Report_Receiver
    {
        #region Members

        private readonly uint m_CumulativePacketsLost;
        private readonly uint m_DelaySinceLastSR;
        private readonly uint m_ExtHigestSeqNumber;
        private readonly uint m_FractionLost;
        private readonly uint m_Jitter;
        private readonly uint m_LastSR;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the fraction of RTP data packets from source SSRC lost since the previous SR or 
        /// RR packet was sent.
        /// </summary>
        public uint FractionLost
        {
            get { return m_FractionLost; }
        }

        /// <summary>
        /// Gets total number of RTP data packets from source SSRC that have
        /// been lost since the beginning of reception.
        /// </summary>
        public uint CumulativePacketsLost
        {
            get { return m_CumulativePacketsLost; }
        }

        /// <summary>
        /// Gets extended highest sequence number received.
        /// </summary>
        public uint ExtendedSequenceNumber
        {
            get { return m_ExtHigestSeqNumber; }
        }

        /// <summary>
        /// Gets an estimate of the statistical variance of the RTP data packet
        /// interarrival time, measured in timestamp units and expressed as an
        /// unsigned integer.
        /// </summary>
        public uint Jitter
        {
            get { return m_Jitter; }
        }

        /// <summary>
        /// Gets when last sender report(SR) was recieved.
        /// </summary>
        public uint LastSR
        {
            get { return m_LastSR; }
        }

        /// <summary>
        /// Gets delay since last sender report(SR) was received.
        /// </summary>
        public uint DelaySinceLastSR
        {
            get { return m_DelaySinceLastSR; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rr">RTCP RR report.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>rr</b> is null reference.</exception>
        internal RTCP_Report_Receiver(RTCP_Packet_ReportBlock rr)
        {
            if (rr == null)
            {
                throw new ArgumentNullException("rr");
            }

            m_FractionLost = rr.FractionLost;
            m_CumulativePacketsLost = rr.CumulativePacketsLost;
            m_ExtHigestSeqNumber = rr.ExtendedHighestSeqNo;
            m_Jitter = rr.Jitter;
            m_LastSR = rr.LastSR;
            m_DelaySinceLastSR = rr.DelaySinceLastSR;
        }

        #endregion
    }
}