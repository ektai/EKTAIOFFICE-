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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace ASC.Common.DependencyInjection
{
    [TypeConverter(typeof(ListElementTypeConverter))]
    public class ListElementCollection : ConfigurationElementCollection<ListItemElement>
    {
        public ListElementCollection()
          : base("item")
        {
        }

        private class ListElementTypeConverter : TypeConverter
        {
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                var instantiableType = GetInstantiableType(destinationType);
                var elementCollection = (ListElementCollection) value;
                if (elementCollection == null || !(instantiableType != null))
                    return base.ConvertTo(context, culture, value, destinationType);
                var genericArguments = instantiableType.GetGenericArguments();
                var list = (IList)Activator.CreateInstance(instantiableType);
                foreach (var listItemElement in elementCollection)
                    list.Add(TypeManipulation.ChangeToCompatibleType(listItemElement.Value, genericArguments[0], null));
                return list;
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                return GetInstantiableType(destinationType) != null || base.CanConvertTo(context, destinationType);
            }

            private static Type GetInstantiableType(Type destinationType)
            {
                if (!typeof(IEnumerable).IsAssignableFrom(destinationType)) return null;
                var typeArray1 = !destinationType.IsGenericType ? new[] { typeof(object) } : destinationType.GetGenericArguments();
                var typeArray2 = typeArray1;
                if (typeArray2.Length != 1)
                    return null;
                var c = typeof(List<>).MakeGenericType(typeArray2);
                return destinationType.IsAssignableFrom(c) ? c : null;
            }
        }
    }
}
