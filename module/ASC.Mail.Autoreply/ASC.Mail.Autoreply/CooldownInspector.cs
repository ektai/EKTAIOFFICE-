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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using log4net;

namespace ASC.Mail.Autoreply
{
    internal class CooldownInspector
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(CooldownInspector));

        private readonly object _syncRoot = new object();
        private readonly Dictionary<Guid, List<DateTime>> _lastUsagesByUser = new Dictionary<Guid, List<DateTime>>();

        private readonly int _allowedRequests;
        private readonly TimeSpan _duringTime;
        private readonly TimeSpan _cooldownLength;

        private Timer _clearTimer;

        public CooldownInspector(CooldownConfigurationElement config)
        {
            _allowedRequests = config.AllowedRequests;
            _duringTime = config.DuringTimeInterval;
            _cooldownLength = config.Length;
        }

        public void Start()
        {
            if (!IsDisabled() && _clearTimer == null)
            {
                _clearTimer = new Timer(x => ClearExpiredRecords(), null, _cooldownLength, _cooldownLength);
            }
        }

        public void Stop()
        {
            if (_clearTimer != null)
            {
                _clearTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _clearTimer.Dispose();
                _clearTimer = null;
                _lastUsagesByUser.Clear();
            }
        }

        public TimeSpan GetCooldownRemainigTime(Guid userId)
        {
            if (IsDisabled()) return TimeSpan.Zero;

            lock (_syncRoot)
            {
                var lastUsages = GetLastUsages(userId);
                return lastUsages.Count >= _allowedRequests ? _cooldownLength - (DateTime.UtcNow - lastUsages.Max()) : TimeSpan.Zero;
            }
        }

        public void RegisterServiceUsage(Guid userId)
        {
            if (IsDisabled()) return;

            lock (_syncRoot)
            {
                var lastUsages = GetLastUsages(userId);
                lastUsages.Add(DateTime.UtcNow);
                _lastUsagesByUser[userId] = lastUsages;
            }
        }

        private void ClearExpiredRecords()
        {
            lock (_syncRoot)
            {
                _log.Debug("start clearing expired usage records");
                try
                {
                    foreach (var userId in _lastUsagesByUser.Keys.ToList())
                    {
                        _lastUsagesByUser[userId] = GetLastUsages(userId);
                        if (_lastUsagesByUser[userId].Count == 0)
                            _lastUsagesByUser.Remove(userId);
                    }
                }
                catch (Exception error)
                {
                    _log.Error("error while clearing expired usage records", error);
                }
            }
        }

        private List<DateTime> GetLastUsages(Guid userId)
        {
            if (!_lastUsagesByUser.ContainsKey(userId))
                return new List<DateTime>();

            return _lastUsagesByUser[userId]
                .Where(timestamp => timestamp > DateTime.UtcNow - _duringTime)
                .OrderByDescending(timestamp => timestamp)
                .Take(_allowedRequests)
                .ToList();
        }

        private bool IsDisabled()
        {
            return _allowedRequests <= 0 || _duringTime <= TimeSpan.Zero || _cooldownLength <= TimeSpan.Zero;
        }
    }
}
