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
using System.Text.RegularExpressions;
using ASC.Files.Core;
using ASC.Files.Core.Security;

namespace ASC.Files.Thirdparty
{
    internal class DbDaoSelector : RegexDaoSelectorBase<int>
    {
        public DbDaoSelector(Func<object, IFileDao> fileDaoActivator,
                             Func<object, IFolderDao> folderDaoActivator,
                             Func<object, ISecurityDao> securityDaoActivator,
                             Func<object, ITagDao> tagDaoActivator)
            : base(new Regex(@"^\d+$", RegexOptions.Singleline | RegexOptions.Compiled),
                   fileDaoActivator,
                   folderDaoActivator,
                   securityDaoActivator,
                   tagDaoActivator,
                   IdConverter)
        {
        }

        public override object GetIdCode(object id)
        {
            return 0;
        }

        private static int IdConverter(object id)
        {
            int result;
            if (Int32.TryParse(Convert.ToString(id), out result))
                return result;

            throw new ArgumentException(string.Format("Int32.TryParse({0})", id));
        }
    }
}