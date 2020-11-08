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
using System.Configuration;
using ASC.Core;
using ASC.Xmpp.Server.Exceptions;
using ASC.Xmpp.Server.storage.Interface;
using UserRosterItemDic = System.Collections.Generic.Dictionary<string, ASC.Xmpp.Server.storage.UserRosterItem>;

namespace ASC.Xmpp.Server.storage
{
	using protocol.iq.roster;

	class ASCRosterStore : DbStoreBase, IRosterStore
	{
		private static readonly string GroupSeparator = "$@$;";

		private readonly object syncRoot = new object();

		private IDictionary<string, UserRosterItemDic> cache;

		private IDictionary<string, UserRosterItemDic> RosterItems
		{
			get
			{
				if (cache == null)
				{
					lock (syncRoot)
					{
						if (cache == null) cache = LoadRosterItems();
					}
				}
				return cache;
			}
		}

		protected override string[] CreateSchemaScript
		{
			get
			{
				return new[] {
					"create table Roster(UserName TEXT NOT NULL, Jid TEXT NOT NULL, Name TEXT, [Subscription] INTEGER, Ask INTEGER, Groups TEXT, primary key(UserName, Jid))", 
				};
			}
		}

		protected override string[] DropSchemaScript
		{
			get
			{
				return new[] { 
					"drop table Roster",
				};
			}
		}

		#region IRosterStore Members

		public ASCRosterStore(ConnectionStringSettings connectionSettings)
			: base(connectionSettings)
		{
			InitializeDbSchema(false);
		}

		public ASCRosterStore(string provider, string connectionString)
			: base(provider, connectionString)
		{
			InitializeDbSchema(false);
		}

		#endregion

		#region IRosterStore Members

		public List<UserRosterItem> GetRosterItems(string userName)
		{
			try
			{
				lock (syncRoot)
				{
					var items = GetASCRosterItems(userName);
					if (RosterItems.ContainsKey(userName)) items.AddRange(RosterItems[userName].Values);
					return items;
				}
			}
			catch (Exception e)
			{
				throw new JabberServiceUnavailableException("Could not get roster items.", e);
			}
		}

		public List<UserRosterItem> GetRosterItems(string userName, SubscriptionType subscriptionType)
		{
			lock (syncRoot)
			{
				return GetRosterItems(userName).FindAll(i => { return i.Subscribtion == subscriptionType; });
			}
		}

		public UserRosterItem GetRosterItem(string userName, string jid)
		{
			lock (syncRoot)
			{
				return GetRosterItems(userName).Find(i => { return string.Compare(new Jid(i.Jid).Bare, jid, true) == 0; });
			}
		}

		public UserRosterItem SaveOrUpdateRosterItem(string userName, UserRosterItem item)
		{
			if (item == null) throw new ArgumentNullException("item");
			if (IsASCRosterItem(userName, item.Jid)) throw new JabberForbiddenException();

			try
			{
				lock (syncRoot)
				{
					if (string.IsNullOrEmpty(item.Name)) item.Name = item.Jid;

					ExecuteNonQuery(
						"insert or replace into Roster(UserName, Jid, Name, Subscription, Ask, Groups) values (@userName, @jid, @name, @subscription, @ask, @groups)",
						new[] { "userName", "jid", "name", "subscription", "ask", "groups" },
						new object[] { userName, item.Jid, item.Name, (Int32)item.Subscribtion, (Int32)item.Ask, item.Groups != null ? item.Groups.ToArray() : null }
					);
					if (!RosterItems.ContainsKey(userName)) RosterItems[userName] = new UserRosterItemDic();
					RosterItems[userName][item.Jid] = item;

					return item;
				}
			}
			catch (Exception e)
			{
				throw new JabberServiceUnavailableException("Could not save or update roster item.", e);
			}
		}

		public void RemoveRosterItem(string userName, string jid)
		{
			if (IsASCRosterItem(userName, jid)) throw new JabberForbiddenException();

			try
			{
				lock (syncRoot)
				{
					if (RosterItems.ContainsKey(userName) && RosterItems[userName].ContainsKey(jid))
					{
						ExecuteNonQuery(
							"delete from Roster where UserName = @userName and Jid = @jid",
							new[] { "userName", "jid" },
							new object[] { userName, jid }
						);
						RosterItems[userName].Remove(jid);
					}
				}
			}
			catch (Exception e)
			{
				throw new JabberServiceUnavailableException("Could not remove roster item.", e);
			}
		}

		#endregion

		private IDictionary<string, UserRosterItemDic> LoadRosterItems()
		{
			var items = new Dictionary<string, UserRosterItemDic>();
			using (var connect = GetDbConnection())
			using (var command = connect.CreateCommand())
			{
				command.CommandText = "select Jid, Name, Subscription, Ask, Groups, UserName from Roster";
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						var item = new UserRosterItem(reader.GetString(0))
						{
							Name = reader[1] as string,
							Subscribtion = !reader.IsDBNull(2) ? (SubscriptionType)reader.GetInt32(2) : default(SubscriptionType),
							Ask = !reader.IsDBNull(3) ? (AskType)reader.GetInt32(3) : default(AskType),
							Groups = !reader.IsDBNull(4) ?
								new List<string>(((string)reader[4]).Split(new[] { GroupSeparator }, StringSplitOptions.RemoveEmptyEntries)) :
								null
						};
						var userName = reader.GetString(5);
						if (!items.ContainsKey(userName)) items[userName] = new UserRosterItemDic();
						items[userName][item.Jid] = item;
					}
				}
			}
			return items;
		}

		private List<UserRosterItem> GetASCRosterItems(string userName)
		{
			var items = new List<UserRosterItem>();
			var domain = Core.Server.ServerDomain;

			foreach (var u in CoreContext.UserManager.GetUsers())
			{
				if (string.Compare(userName, u.UserName, true) == 0) continue;

				var item = new UserRosterItem(new Jid(u.UserName, domain, null).ToString())
				{
					Name = string.Format("{0} {1}", u.LastName, u.FirstName),
					Subscribtion = SubscriptionType.both,
					Ask = AskType.NONE,
					Groups = null
				};
				foreach (var g in CoreContext.UserManager.GetUserGroups(u.ID, CoreContext.UserManager.MainGroupCategory.ID))
				{
					if (item.Groups == null) item.Groups = new List<string>();
					item.Groups.Add(g.Name);
				}
				items.Add(item);
			}
			return items;
		}

		private bool IsASCRosterItem(string userName, string jid)
		{
			return GetASCRosterItems(userName).Exists(i => { return string.Compare(new Jid(i.Jid).Bare, jid) == 0; });
		}
	}
}