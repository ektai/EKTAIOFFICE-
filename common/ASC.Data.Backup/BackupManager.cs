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
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ASC.Data.Backup
{
    public class BackupManager
    {
        private const string ROOT = "backup";
        private const string XML_NAME = "backupinfo.xml";

        private IDictionary<string, IBackupProvider> providers;
        private readonly string backup;
        private readonly string[] configs;
        
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;


        public BackupManager(string backup)
            : this(backup, null)
        {
        }

        public BackupManager(string backup, params string[] configs)
        {
            this.backup = backup;
            this.configs = configs ?? new string[0];

            providers = new Dictionary<string, IBackupProvider>();
            AddBackupProvider(new DbBackupProvider());
            AddBackupProvider(new FileBackupProvider());
        }

        public void AddBackupProvider(IBackupProvider provider)
        {
            providers.Add(provider.Name, provider);
            provider.ProgressChanged += OnProgressChanged;
        }

        public void RemoveBackupProvider(string name)
        {
            if (providers.ContainsKey(name))
            {
                providers[name].ProgressChanged -= OnProgressChanged;
                providers.Remove(name);
            }
        }


        public void Save(int tenant)
        {
            using (var backupWriter = new ZipWriteOperator(backup))
            {
                var doc = new XDocument(new XElement(ROOT, new XAttribute("tenant", tenant)));
                foreach (var provider in providers.Values)
                {
                    var elements = provider.GetElements(tenant, configs, backupWriter);
                    if (elements != null)
                    {
                        doc.Root.Add(new XElement(provider.Name, elements));
                    }
                }

                backupWriter.WriteEntry(XML_NAME, doc.ToString(SaveOptions.None));
            }
            ProgressChanged(this, new ProgressChangedEventArgs(string.Empty, 100, true));
        }

        public void Load()
        {
            using (var reader = new ZipReadOperator(backup))
            using (var stream = reader.GetEntry(XML_NAME))
            using (var xml = XmlReader.Create(stream))
            {
                var root = XDocument.Load(xml).Element(ROOT);
                if (root == null) return;

                var tenant = int.Parse(root.Attribute("tenant").Value);
                foreach (var provider in providers.Values)
                {
                    var element = root.Element(provider.Name);
                    provider.LoadFrom(element != null ? element.Elements() : new XElement[0], tenant, configs, reader);
                }
            }
            ProgressChanged(this, new ProgressChangedEventArgs(string.Empty, 100, true));
        }


        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (ProgressChanged != null) ProgressChanged(this, e);
        }
    }
}