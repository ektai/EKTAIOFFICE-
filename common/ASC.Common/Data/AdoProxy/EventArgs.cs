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


using System;
using System.Data;
using System.Data.Common;
using System.Text;

namespace ASC.Common.Data.AdoProxy
{
    class ExecutedEventArgs : EventArgs
    {
        public TimeSpan Duration { get; private set; }

        public string SqlMethod { get; private set; }

        public string Sql { get; private set; }

        public string SqlParameters { get; private set; }


        public ExecutedEventArgs(string method, TimeSpan duration)
            : this(method, duration, null)
        {

        }

        public ExecutedEventArgs(string method, TimeSpan duration, DbCommand command)
        {
            SqlMethod = method;
            Duration = duration;
            if (command != null)
            {
                Sql = command.CommandText;
                
                if (0 < command.Parameters.Count)
                {
                    var stringBuilder = new StringBuilder();
                    foreach (IDbDataParameter p in command.Parameters)
                    {
                        if (!string.IsNullOrEmpty(p.ParameterName)) stringBuilder.AppendFormat("{0}=", p.ParameterName);
                        stringBuilder.AppendFormat("{0}, ", p.Value == null || DBNull.Value.Equals(p.Value) ? "NULL" : p.Value.ToString());
                    }
                    SqlParameters = stringBuilder.ToString(0, stringBuilder.Length - 2);
                }
            }
        }
    }
}