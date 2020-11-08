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


namespace ASC.Mail.Net.SMTP.Server
{
    #region usings

    using System;
    using System.Collections.Generic;
    using TCP;

    #endregion

    /// <summary>
    /// This class implements SMTP server.  Defined RFC 5321.
    /// </summary>
    public class SMTP_Server : TCP_Server<SMTP_Session>
    {
        #region Members

        private readonly List<string> m_pServiceExtentions;
        private string m_GreetingText = "";
        private int m_MaxBadCommands = 30;
        private int m_MaxMessageSize = 10000000;
        private int m_MaxRecipients = 100;
        private int m_MaxTransactions = 10;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SMTP_Server()
        {
            m_pServiceExtentions = new List<string>();
            m_pServiceExtentions.Add(SMTP_ServiceExtensions.PIPELINING);
            m_pServiceExtentions.Add(SMTP_ServiceExtensions.SIZE);
            m_pServiceExtentions.Add(SMTP_ServiceExtensions.STARTTLS);
            m_pServiceExtentions.Add(SMTP_ServiceExtensions._8BITMIME);
            m_pServiceExtentions.Add(SMTP_ServiceExtensions.BINARYMIME);
            m_pServiceExtentions.Add(SMTP_ServiceExtensions.CHUNKING);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets server greeting text.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public string GreetingText
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_GreetingText;
            }

            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                m_GreetingText = value;
            }
        }

        /// <summary>
        /// Gets or sets how many bad commands session can have before it's terminated. Value 0 means unlimited.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <exception cref="ArgumentException">Is raised when invalid value is passed.</exception>
        public int MaxBadCommands
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_MaxBadCommands;
            }

            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }
                if (value < 0)
                {
                    throw new ArgumentException("Property 'MaxBadCommands' value must be >= 0.");
                }

                m_MaxBadCommands = value;
            }
        }

        /// <summary>
        /// Gets or sets maximum message size in bytes.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <exception cref="ArgumentException">Is raised when invalid value is passed.</exception>
        public int MaxMessageSize
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_MaxMessageSize;
            }

            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                if (value < 500)
                {
                    throw new ArgumentException("Property 'MaxMessageSize' value must be >= 500.");
                }

                m_MaxMessageSize = value;
            }
        }

        /// <summary>
        /// Gets or sets maximum allowed recipients per SMTP transaction.
        /// </summary>
        /// <remarks>According RFC 5321 this value SHOULD NOT be less than 100.</remarks>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <exception cref="ArgumentException">Is raised when invalid value is passed.</exception>
        public int MaxRecipients
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_MaxRecipients;
            }

            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }
                if (value < 1)
                {
                    throw new ArgumentException("Property 'MaxRecipients' value must be >= 1.");
                }

                m_MaxRecipients = value;
            }
        }

        /// <summary>
        /// Gets or sets maximum mail transactions per session. Value 0 means unlimited.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <exception cref="ArgumentException">Is raised when invalid value is passed.</exception>
        public int MaxTransactions
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_MaxTransactions;
            }

            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }
                if (value < 0)
                {
                    throw new ArgumentException("Property 'MaxTransactions' value must be >= 0.");
                }

                m_MaxTransactions = value;
            }
        }

        /// <summary>
        /// Gets or sets SMTP server supported service extentions.
        /// Supported values: PIPELINING,SIZE,STARTTLS,8BITMIME,BINARYMIME,CHUNKING,DSN.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when null reference passed.</exception>
        public string[] ServiceExtentions
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_pServiceExtentions.ToArray();
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("ServiceExtentions");
                }

                m_pServiceExtentions.Clear();

                foreach (string extention in value)
                {
                    if (extention.ToUpper() == SMTP_ServiceExtensions.PIPELINING)
                    {
                        m_pServiceExtentions.Add(SMTP_ServiceExtensions.PIPELINING);
                    }
                    else if (extention.ToUpper() == SMTP_ServiceExtensions.SIZE)
                    {
                        m_pServiceExtentions.Add(SMTP_ServiceExtensions.SIZE);
                    }
                    else if (extention.ToUpper() == SMTP_ServiceExtensions.STARTTLS)
                    {
                        m_pServiceExtentions.Add(SMTP_ServiceExtensions.STARTTLS);
                    }
                    else if (extention.ToUpper() == SMTP_ServiceExtensions._8BITMIME)
                    {
                        m_pServiceExtentions.Add(SMTP_ServiceExtensions._8BITMIME);
                    }
                    else if (extention.ToUpper() == SMTP_ServiceExtensions.BINARYMIME)
                    {
                        m_pServiceExtentions.Add(SMTP_ServiceExtensions.BINARYMIME);
                    }
                    else if (extention.ToUpper() == SMTP_ServiceExtensions.CHUNKING)
                    {
                        m_pServiceExtentions.Add(SMTP_ServiceExtensions.CHUNKING);
                    }
                    else if (extention.ToUpper() == SMTP_ServiceExtensions.DSN)
                    {
                        m_pServiceExtentions.Add(SMTP_ServiceExtensions.DSN);
                    }
                }
            }
        }

        /// <summary>
        /// Gets SMTP service extentions list.
        /// </summary>
        internal List<string> Extentions
        {
            get { return m_pServiceExtentions; }
        }

        #endregion

        // TODO:

        //public override Dispose

        #region Overrides

        /// <summary>
        /// Is called when new incoming session and server maximum allowed connections exceeded.
        /// </summary>
        /// <param name="session">Incoming session.</param>
        /// <remarks>This method allows inhereted classes to report error message to connected client.
        /// Session will be disconnected after this method completes.
        /// </remarks>
        protected override void OnMaxConnectionsExceeded(SMTP_Session session)
        {
            session.TcpStream.WriteLine(
                "421 Client host rejected: too many connections, please try again later.");
        }

        /// <summary>
        /// Is called when new incoming session and server maximum allowed connections per connected IP exceeded.
        /// </summary>
        /// <param name="session">Incoming session.</param>
        /// <remarks>This method allows inhereted classes to report error message to connected client.
        /// Session will be disconnected after this method completes.
        /// </remarks>
        protected override void OnMaxConnectionsPerIPExceeded(SMTP_Session session)
        {
            session.TcpStream.WriteLine("421 Client host rejected: too many connections from your IP(" +
                                        session.RemoteEndPoint.Address + "), please try again later.");
        }

        #endregion
    }
}