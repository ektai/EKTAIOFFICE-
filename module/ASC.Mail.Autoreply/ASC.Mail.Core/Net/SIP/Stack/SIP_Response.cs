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


namespace ASC.Mail.Net.SIP.Stack
{
    #region usings

    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using Message;

    #endregion

    /// <summary>
    /// SIP server response. Related RFC 3261.
    /// </summary>
    public class SIP_Response : SIP_Message
    {
        #region Members

        private readonly SIP_Request m_pRequest;
        private string m_ReasonPhrase = "";
        private double m_SipVersion = 2.0d;
        private int m_StatusCode = 100;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SIP_Response() {}

        /// <summary>
        /// SIP_Request.CreateResponse constructor.
        /// </summary>
        /// <param name="request">Owner request.</param>
        internal SIP_Response(SIP_Request request)
        {
            m_pRequest = request;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets reponse reasong phrase. This just StatusCode describeing text.
        /// </summary>
        public string ReasonPhrase
        {
            get { return m_ReasonPhrase; }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("ReasonPhrase");
                }

                m_ReasonPhrase = value;
            }
        }

        /// <summary>
        /// Gets SIP request which response it is. This value is null if this is stateless response.
        /// </summary>
        public SIP_Request Request
        {
            get { return m_pRequest; }
        }

        /// <summary>
        /// Gets or sets SIP version.
        /// </summary>
        /// <exception cref="ArgumentException">Is raised when invalid SIP version value passed.</exception>
        public double SipVersion
        {
            get { return m_SipVersion; }

            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("Property SIP version must be >= 1.0 !");
                }

                m_SipVersion = value;
            }
        }

        /// <summary>
        /// Gets or sets response status code. This value must be between 100 and 999.
        /// </summary>
        /// <exception cref="ArgumentException">Is raised when value is out of allowed range.</exception>
        public int StatusCode
        {
            get { return m_StatusCode; }

            set
            {
                if (value < 1 || value > 999)
                {
                    throw new ArgumentException("Property 'StatusCode' value must be >= 100 && <= 999 !");
                }

                m_StatusCode = value;
            }
        }

        /// <summary>
        /// Gets or sets SIP Status-Code with Reason-Phrase (Status-Code SP Reason-Phrase).
        /// </summary>
        public string StatusCode_ReasonPhrase
        {
            get { return m_StatusCode + " " + m_ReasonPhrase; }

            set
            {
                // Status-Code SP Reason-Phrase

                if (value == null)
                {
                    throw new ArgumentNullException("StatusCode_ReasonPhrase");
                }

                string[] code_reason = value.Split(new[] {' '}, 2);
                if (code_reason.Length != 2)
                {
                    throw new ArgumentException(
                        "Invalid property 'StatusCode_ReasonPhrase' Reason-Phrase value !");
                }
                try
                {
                    StatusCode = Convert.ToInt32(code_reason[0]);
                }
                catch
                {
                    throw new ArgumentException(
                        "Invalid property 'StatusCode_ReasonPhrase' Status-Code value !");
                }
                ReasonPhrase = code_reason[1];
            }
        }

        /// <summary>
        /// Gets SIP status code type.
        /// </summary>
        public SIP_StatusCodeType StatusCodeType
        {
            get
            {
                if (m_StatusCode >= 100 && m_StatusCode < 200)
                {
                    return SIP_StatusCodeType.Provisional;
                }
                else if (m_StatusCode >= 200 && m_StatusCode < 300)
                {
                    return SIP_StatusCodeType.Success;
                }
                else if (m_StatusCode >= 300 && m_StatusCode < 400)
                {
                    return SIP_StatusCodeType.Redirection;
                }
                else if (m_StatusCode >= 400 && m_StatusCode < 500)
                {
                    return SIP_StatusCodeType.RequestFailure;
                }
                else if (m_StatusCode >= 500 && m_StatusCode < 600)
                {
                    return SIP_StatusCodeType.ServerFailure;
                }
                else if (m_StatusCode >= 600 && m_StatusCode < 700)
                {
                    return SIP_StatusCodeType.GlobalFailure;
                }
                else
                {
                    throw new Exception("Unknown SIP StatusCodeType !");
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parses SIP_Response from byte array.
        /// </summary>
        /// <param name="data">Valid SIP response data.</param>
        /// <returns>Returns parsed SIP_Response obeject.</returns>
        /// <exception cref="ArgumentNullException">Raised when <b>data</b> is null.</exception>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        public static SIP_Response Parse(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            return Parse(new MemoryStream(data));
        }

        /// <summary>
        /// Parses SIP_Response from stream.
        /// </summary>
        /// <param name="stream">Stream what contains valid SIP response.</param>
        /// <returns>Returns parsed SIP_Response obeject.</returns>
        /// <exception cref="ArgumentNullException">Raised when <b>stream</b> is null.</exception>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        public static SIP_Response Parse(Stream stream)
        {
            /* Syntax:
                SIP-Version SP Status-Code SP Reason-Phrase
                SIP-Message                          
            */

            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            SIP_Response retVal = new SIP_Response();

            // Parse Response-line
            StreamLineReader r = new StreamLineReader(stream);
            r.Encoding = "utf-8";
            string[] version_code_text = r.ReadLineString().Split(new[] {' '}, 3);
            if (version_code_text.Length != 3)
            {
                throw new SIP_ParseException(
                    "Invalid SIP Status-Line syntax ! Syntax: {SIP-Version SP Status-Code SP Reason-Phrase}.");
            }
            // SIP-Version
            try
            {
                retVal.SipVersion = Convert.ToDouble(version_code_text[0].Split('/')[1],
                                                     NumberFormatInfo.InvariantInfo);
            }
            catch
            {
                throw new SIP_ParseException("Invalid Status-Line SIP-Version value !");
            }

            // Status-Code
            try
            {
                retVal.StatusCode = Convert.ToInt32(version_code_text[1]);
            }
            catch
            {
                throw new SIP_ParseException("Invalid Status-Line Status-Code value !");
            }

            // Reason-Phrase
            retVal.ReasonPhrase = version_code_text[2];

            // Parse SIP-Message
            retVal.InternalParse(stream);

            return retVal;
        }

        /// <summary>
        /// Clones this request.
        /// </summary>
        /// <returns>Returns new cloned request.</returns>
        public SIP_Response Copy()
        {
            SIP_Response retVal = Parse(ToByteData());

            return retVal;
        }

        /// <summary>
        /// Checks if SIP response has all required values as response line,header fields and their values.
        /// Throws Exception if not valid SIP response.
        /// </summary>
        public void Validate()
        {
            // Via: + branch prameter
            // To:
            // From:
            // CallID:
            // CSeq

            if (Via.GetTopMostValue() == null)
            {
                throw new SIP_ParseException("Via: header field is missing !");
            }
            if (Via.GetTopMostValue().Branch == null)
            {
                throw new SIP_ParseException("Via: header fields branch parameter is missing !");
            }

            if (To == null)
            {
                throw new SIP_ParseException("To: header field is missing !");
            }

            if (From == null)
            {
                throw new SIP_ParseException("From: header field is missing !");
            }

            if (CallID == null)
            {
                throw new SIP_ParseException("CallID: header field is missing !");
            }

            if (CSeq == null)
            {
                throw new SIP_ParseException("CSeq: header field is missing !");
            }

            // TODO: INVITE 2xx must have only 1 contact header with SIP or SIPS.
        }

        /// <summary>
        /// Stores SIP_Response to specified stream.
        /// </summary>
        /// <param name="stream">Stream where to store.</param>
        public void ToStream(Stream stream)
        {
            // Status-Line = SIP-Version SP Status-Code SP Reason-Phrase CRLF

            // Add response-line
            byte[] responseLine =
                Encoding.UTF8.GetBytes("SIP/" + SipVersion.ToString("f1").Replace(',', '.') + " " + StatusCode +
                                       " " + ReasonPhrase + "\r\n");
            stream.Write(responseLine, 0, responseLine.Length);

            // Add SIP-message
            InternalToStream(stream);
        }

        /// <summary>
        /// Converts this response to raw srver response data.
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteData()
        {
            MemoryStream retVal = new MemoryStream();
            ToStream(retVal);

            return retVal.ToArray();
        }

        /// <summary>
        /// Returns response as string.
        /// </summary>
        /// <returns>Returns response as string.</returns>
        public override string ToString()
        {
            return Encoding.UTF8.GetString(ToByteData());
        }

        #endregion
    }
}