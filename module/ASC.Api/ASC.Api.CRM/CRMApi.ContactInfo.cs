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
using ASC.Api.Attributes;
using ASC.Api.CRM.Wrappers;
using ASC.Api.Collections;
using ASC.Api.Exceptions;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.ElasticSearch;
using ASC.MessagingSystem;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Core.Search;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ASC.Api.CRM
{
    public partial class CRMApi
    {
        /// <summary>
        ///   Returns the list of all available contact categories
        /// </summary>
        /// <param name="infoType">
        ///    Contact information type
        /// </param>
        /// <short>Get all categories</short> 
        /// <category>Contacts</category>
        /// <returns>
        ///   List of all available contact categories
        /// </returns>
        [Read(@"contact/data/{infoType}/category")]
        public IEnumerable<string> GetContactInfoCategory(ContactInfoType infoType)
        {
            return Enum.GetNames(ContactInfo.GetCategory(infoType)).ToItemList();
        }

        /// <summary>
        ///   Returns the list of all available contact information types
        /// </summary>
        /// <short>Get all contact info types</short> 
        /// <category>Contacts</category>
        /// <returns></returns>
        [Read(@"contact/data/infoType")]
        public IEnumerable<string> GetContactInfoType()
        {
            return Enum.GetNames(typeof(ContactInfoType)).ToItemList();
        }

        /// <summary>
        ///    Returns the detailed information for the contact
        /// </summary>
        /// <param name="contactid">Contact ID</param>
        /// <short>Get contact information</short> 
        /// <category>Contacts</category>
        /// <returns>
        ///   Contact information
        /// </returns>
        [Read(@"contact/{contactid:[0-9]+}/data")]
        public IEnumerable<ContactInfoWrapper> GetContactInfo(int contactid)
        {
            if (contactid <= 0) throw new ArgumentException();

            var contact = DaoFactory.ContactDao.GetByID(contactid);
            if (contact == null || !CRMSecurity.CanAccessTo(contact)) throw new ItemNotFoundException();

            return DaoFactory.ContactInfoDao.GetList(contactid, null, null, null)
                .OrderByDescending(info => info.ID)
                .ToList()
                .ConvertAll(ToContactInfoWrapper);
        }

        /// <summary>
        ///   Returns the detailed list of all information available for the contact with the ID specified in the request
        /// </summary>
        /// <param name="contactid">Contact ID</param>
        /// <param name="id">Contact information ID</param>
        /// <short>Get contact info</short> 
        /// <category>Contacts</category>
        /// <returns>Contact information</returns>
        ///<exception cref="ArgumentException"></exception>
        [Read(@"contact/{contactid:[0-9]+}/data/{id:[0-9]+}")]
        public ContactInfoWrapper GetContactInfoByID(int contactid, int id)
        {
            if (contactid <= 0 || id <= 0) throw new ArgumentException();

            var contact = DaoFactory.ContactDao.GetByID(contactid);
            if (contact == null || !CRMSecurity.CanAccessTo(contact)) throw new ItemNotFoundException();

            var contactInfo = DaoFactory.ContactInfoDao.GetByID(id);

            if (contactInfo == null || contactInfo.ContactID != contactid) throw new ArgumentException();

            return ToContactInfoWrapper(contactInfo);
        }

        /// <summary>
        ///    Adds the information with the parameters specified in the request to the contact with the selected ID
        /// </summary>
        ///<param name="contactid">Contact ID</param>
        ///<param name="infoType">Contact information type</param>
        ///<param name="data">Data</param>
        ///<param name="isPrimary">Contact importance: primary or not</param>
        ///<param   name="category">Category</param>
        ///<short> Add contact info</short> 
        ///<category>Contacts</category>
        /// <seealso cref="GetContactInfoType"/>
        /// <seealso cref="GetContactInfoCategory"/>
        /// <returns>
        ///    Contact information
        /// </returns> 
        ///<exception cref="ArgumentException"></exception>
        [Create(@"contact/{contactid:[0-9]+}/data")]
        public ContactInfoWrapper CreateContactInfo(int contactid, ContactInfoType infoType, string data, bool isPrimary, string category)
        {
            if (string.IsNullOrEmpty(data) || contactid <= 0) throw new ArgumentException();
            var contact = DaoFactory.ContactDao.GetByID(contactid);
            if (contact == null) throw new ItemNotFoundException();

            if (infoType == ContactInfoType.Twitter)
            {
                if (!CRMSecurity.CanAccessTo(contact)) throw new ItemNotFoundException();
            }
            else
            {
                if (!CRMSecurity.CanEdit(contact)) throw new ItemNotFoundException();
            }

            var categoryType = ContactInfo.GetCategory(infoType);
            if (!Enum.IsDefined(categoryType, category)) throw new ArgumentException();


            var contactInfo = new ContactInfo
                {
                    Data = data,
                    InfoType = infoType,
                    ContactID = contactid,
                    IsPrimary = isPrimary,
                    Category = (int)Enum.Parse(categoryType, category)
                };

            if (contactInfo.InfoType == ContactInfoType.Address)
            {
                Address res;
                if (!Address.TryParse(contactInfo, out res))
                    throw new ArgumentException();
            }

            var contactInfoID = DaoFactory.ContactInfoDao.Save(contactInfo);

            var messageAction = contact is Company ? MessageAction.CompanyUpdatedPrincipalInfo : MessageAction.PersonUpdatedPrincipalInfo;
            MessageService.Send(Request, messageAction, MessageTarget.Create(contact.ID), contact.GetTitle());

            var contactInfoWrapper = ToContactInfoWrapper(contactInfo);
            contactInfoWrapper.ID = contactInfoID;
            return contactInfoWrapper;
        }

        /// <summary>
        ///    Adds the address information to the contact with the selected ID
        /// </summary>
        /// <param name="contactid">Contact ID</param>
        /// <param name="address">Address data</param>
        /// <short>Add address info</short> 
        /// <category>Contacts</category>
        /// <seealso cref="GetContactInfoType"/>
        /// <seealso cref="GetContactInfoCategory"/>
        /// <returns>
        ///    Contact information
        /// </returns> 
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        [Create(@"contact/{contactid:[0-9]+}/addressdata")]
        public ContactInfoWrapper CreateContactInfoAddress(int contactid, Address address)
        {
            if (contactid <= 0) throw new ArgumentException("Invalid value", "contactid");

            var contact = DaoFactory.ContactDao.GetByID(contactid);

            if (contact == null || !CRMSecurity.CanEdit(contact)) throw new ItemNotFoundException();

            if (address == null) throw new ArgumentException("Value cannot be null", "address");

            if (!Enum.IsDefined(typeof(AddressCategory), address.Category)) throw new ArgumentException("Value does not fall within the expected range.", "address.Category");

            address.CategoryName = ((AddressCategory)address.Category).ToLocalizedString();

            var settings = new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                        {
                            NamingStrategy = new CamelCaseNamingStrategy()
                        },
                    Formatting = Formatting.Indented
                };

            var contactInfo = new ContactInfo
                {
                    InfoType = ContactInfoType.Address,
                    ContactID = contactid,
                    IsPrimary = address.IsPrimary,
                    Category = address.Category,
                    Data = JsonConvert.SerializeObject(address, settings)
                };

            contactInfo.ID = DaoFactory.ContactInfoDao.Save(contactInfo);

            var messageAction = contact is Company ? MessageAction.CompanyUpdatedPrincipalInfo : MessageAction.PersonUpdatedPrincipalInfo;
            MessageService.Send(Request, messageAction, MessageTarget.Create(contact.ID), contact.GetTitle());

            return ToContactInfoWrapper(contactInfo);
        }

        /// <summary>
        ///  Creates contact information (add new information to the old list) with the parameters specified in the request for the contact with the selected ID
        /// </summary>
        ///<short>Group contact info</short> 
        /// <param name="contactid">Contact ID</param>
        /// <param name="items">Contact information</param>
        /// <remarks>
        /// <![CDATA[
        ///  items has format
        ///  [{infoType : 1, category : 1, categoryName : 'work', data : "myemail@email.com", isPrimary : true}, {infoType : 0, category : 0, categoryName : 'home', data : "+8999111999111", isPrimary : true}]
        /// ]]>
        /// </remarks>
        /// <category>Contacts</category>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>
        ///   Contact information
        /// </returns>
        /// <visible>false</visible>
        [Create(@"contact/{contactid:[0-9]+}/batch")]
        public IEnumerable<ContactInfoWrapper> CreateBatchContactInfo(int contactid, IEnumerable<ContactInfoWrapper> items)
        {
            if (contactid <= 0) throw new ArgumentException();

            var contact = DaoFactory.ContactDao.GetByID(contactid);
            if (contact == null || !CRMSecurity.CanEdit(contact)) throw new ItemNotFoundException();

            var itemsList = items != null ? items.ToList() : new List<ContactInfoWrapper>();
            var contactInfoList = itemsList.Select(FromContactInfoWrapper).ToList();

            foreach (var contactInfo in contactInfoList)
            {
                if (contactInfo.InfoType == ContactInfoType.Address)
                {
                    Address res;
                    if(!Address.TryParse(contactInfo, out res))
                        throw new ArgumentException();
                }
                contactInfo.ContactID = contactid;
            }

            var ids = DaoFactory.ContactInfoDao.SaveList(contactInfoList, contact);

            for (var index = 0; index < itemsList.Count; index++)
            {
                var infoWrapper = itemsList[index];
                infoWrapper.ID = ids[index];
            }
            return itemsList;
        }

        /// <summary>
        ///   Updates the information with the parameters specified in the request for the contact with the selected ID
        /// </summary>
        ///<param name="id">Contact information record ID</param>
        ///<param name="contactid">Contact ID</param>
        ///<param optional="true" name="infoType">Contact information type</param>
        ///<param name="data">Data</param>
        ///<param optional="true" name="isPrimary">Contact importance: primary or not</param>
        ///<param optional="true" name="category">Contact information category</param>
        ///<short>Update contact info</short> 
        ///<category>Contacts</category>
        ///<exception cref="ArgumentException"></exception>
        /// <returns>
        ///   Contact information
        /// </returns>
        [Update(@"contact/{contactid:[0-9]+}/data/{id:[0-9]+}")]
        public ContactInfoWrapper UpdateContactInfo(int id, int contactid, ContactInfoType? infoType, string data, bool? isPrimary, string category)
        {
            if (id <= 0 || string.IsNullOrEmpty(data) || contactid <= 0) throw new ArgumentException();

            var contact = DaoFactory.ContactDao.GetByID(contactid);
            if (contact == null || !CRMSecurity.CanEdit(contact)) throw new ItemNotFoundException();

            var contactInfo = DaoFactory.ContactInfoDao.GetByID(id);

            if (infoType != null)
            {
                var categoryType = ContactInfo.GetCategory(infoType.Value);

                if (!string.IsNullOrEmpty(category) && Enum.IsDefined(categoryType, category))
                {
                    contactInfo.Category = (int)Enum.Parse(categoryType, category);
                }

                contactInfo.InfoType = infoType.Value;
            }

            contactInfo.ContactID = contactid;

            if (isPrimary != null)
            {
                contactInfo.IsPrimary = isPrimary.Value;
            }

            contactInfo.Data = data;

            if (contactInfo.InfoType == ContactInfoType.Address)
            {
                Address res;
                if (!Address.TryParse(contactInfo, out res))
                    throw new ArgumentException();
            }

            DaoFactory.ContactInfoDao.Update(contactInfo);

            var messageAction = contact is Company ? MessageAction.CompanyUpdatedPrincipalInfo : MessageAction.PersonUpdatedPrincipalInfo;
            MessageService.Send(Request, messageAction, MessageTarget.Create(contact.ID), contact.GetTitle());

            var contactInfoWrapper = ToContactInfoWrapper(contactInfo);
            return contactInfoWrapper;
        }

        /// <summary>
        ///   Updates the address information with the parameters specified in the request for the contact with the selected ID
        /// </summary>
        /// <param name="id">Contact information record ID</param>
        /// <param name="contactid">Contact ID</param>
        /// <param name="address">Address data</param>
        /// <short>Update address info</short> 
        /// <category>Contacts</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///   Contact information
        /// </returns>
        [Update(@"contact/{contactid:[0-9]+}/addressdata/{id:[0-9]+}")]
        public ContactInfoWrapper UpdateContactInfoAddress(int id, int contactid, Address address)
        {
            if (id <= 0) throw new ArgumentException("Invalid value", "id");

            var contactInfo = DaoFactory.ContactInfoDao.GetByID(id);

            if (contactInfo == null || contactInfo.InfoType != ContactInfoType.Address) throw new ItemNotFoundException();

            if (contactid <= 0) throw new ArgumentException("Invalid value", "contactid");

            var contact = DaoFactory.ContactDao.GetByID(contactid);

            if (contact == null || !CRMSecurity.CanEdit(contact) || contactInfo.ContactID != contactid) throw new ItemNotFoundException();

            if (address == null) throw new ArgumentException("Value cannot be null", "address");

            if (!Enum.IsDefined(typeof(AddressCategory), address.Category)) throw new ArgumentException("Value does not fall within the expected range.", "address.Category");

            address.CategoryName = ((AddressCategory) address.Category).ToLocalizedString();

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            };

            contactInfo.IsPrimary = address.IsPrimary;
            contactInfo.Category = address.Category;
            contactInfo.Data = JsonConvert.SerializeObject(address, settings);

            DaoFactory.ContactInfoDao.Update(contactInfo);

            var messageAction = contact is Company ? MessageAction.CompanyUpdatedPrincipalInfo : MessageAction.PersonUpdatedPrincipalInfo;
            MessageService.Send(Request, messageAction, MessageTarget.Create(contact.ID), contact.GetTitle());

            return ToContactInfoWrapper(contactInfo);
        }

        /// <summary>
        ///  Updates contact information (delete old information and add new list) with the parameters specified in the request for the contact with the selected ID
        /// </summary>
        ///<short>Group contact info update</short> 
        ///<param name="contactid">Contact ID</param>
        ///<param name="items">Contact information</param>
        /// <![CDATA[
        ///  items has format
        ///  [{infoType : 1, category : 1, categoryName : 'work', data : "myemail@email.com", isPrimary : true}, {infoType : 0, category : 0, categoryName : 'home', data : "+8999111999111", isPrimary : true}]
        /// ]]>
        ///<category>Contacts</category>
        ///<exception cref="ArgumentException"></exception>
        /// <returns>
        ///   Contact information
        /// </returns>
        /// <visible>false</visible>
        [Update(@"contact/{contactid:[0-9]+}/batch")]
        public IEnumerable<ContactInfoWrapper> UpdateBatchContactInfo(int contactid, IEnumerable<ContactInfoWrapper> items)
        {
            if (contactid <= 0) throw new ArgumentException();

            var contact = DaoFactory.ContactDao.GetByID(contactid);
            if (contact == null || !CRMSecurity.CanEdit(contact)) throw new ItemNotFoundException();

            var itemsList = items != null ? items.ToList() : new List<ContactInfoWrapper>();
            var contactInfoList = itemsList.Select(FromContactInfoWrapper).ToList();

            foreach (var contactInfo in contactInfoList)
            {
                if (contactInfo.InfoType == ContactInfoType.Address)
                {
                    Address res;
                    if (!Address.TryParse(contactInfo, out res))
                        throw new ArgumentException();
                }
                contactInfo.ContactID = contactid;
            }

            DaoFactory.ContactInfoDao.DeleteByContact(contactid);
            var ids = DaoFactory.ContactInfoDao.SaveList(contactInfoList, contact);

            for (var index = 0; index < itemsList.Count; index++)
            {
                var infoWrapper = itemsList[index];
                infoWrapper.ID = ids[index];
            }
            return itemsList;
        }

        /// <summary>
        ///    Returns the detailed information for the contact with the selected ID by the information type specified in the request
        /// </summary>
        /// <param name="contactid">Contact ID</param>
        /// <param name="infoType">Contact information type</param>
        /// <short>Get contact information by type</short> 
        /// <category>Contacts</category>
        /// <returns>
        ///   Contact information
        /// </returns>
        [Read(@"contact/{contactid:[0-9]+}/data/{infoType}")]
        public IEnumerable<string> GetContactInfo(int contactid, ContactInfoType infoType)
        {
            if (contactid <= 0) throw new ArgumentException();

            var contact = DaoFactory.ContactDao.GetByID(contactid);
            if (contact == null || !CRMSecurity.CanAccessTo(contact)) throw new ItemNotFoundException();

            return DaoFactory.ContactInfoDao.GetListData(contactid, infoType);
        }


        /// <summary>
        ///   Deletes the contact information for the contact with the ID specified in the request
        /// </summary>
        /// <param name="contactid">Contact ID</param>
        /// <param name="id">Contact information record ID</param>
        /// <short>Delete contact info</short> 
        /// <category>Contacts</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///   Contact information
        /// </returns>
        [Delete(@"contact/{contactid:[0-9]+}/data/{id:[0-9]+}")]
        public ContactInfoWrapper DeleteContactInfo(int contactid, int id)
        {
            if (id <= 0 || contactid <= 0) throw new ArgumentException();

            var contact = DaoFactory.ContactDao.GetByID(contactid);
            if (contact == null || !CRMSecurity.CanEdit(contact)) throw new ItemNotFoundException();

            var contactInfo = DaoFactory.ContactInfoDao.GetByID(id);
            if (contactInfo == null) throw new ItemNotFoundException();

            var wrapper = ToContactInfoWrapper(contactInfo);

            DaoFactory.ContactInfoDao.Delete(id);

            var messageAction = contact is Company ? MessageAction.CompanyUpdatedPrincipalInfo : MessageAction.PersonUpdatedPrincipalInfo;
            MessageService.Send(Request, messageAction, MessageTarget.Create(contact.ID), contact.GetTitle());

            if (contactInfo.InfoType == ContactInfoType.Email)
            {
                FactoryIndexer<EmailWrapper>.DeleteAsync(EmailWrapper.ToEmailWrapper(contact, new List<ContactInfo> { contactInfo}));
            }
            FactoryIndexer<InfoWrapper>.DeleteAsync(contactInfo);

            return wrapper;
        }


        private static ContactInfoWrapper ToContactInfoWrapper(ContactInfo contactInfo)
        {
            return new ContactInfoWrapper(contactInfo);
        }

        private static ContactInfo FromContactInfoWrapper(ContactInfoWrapper contactInfoWrapper)
        {
            return new ContactInfo
                {
                    ID = contactInfoWrapper.ID,
                    Category = contactInfoWrapper.Category,
                    Data = contactInfoWrapper.Data,
                    InfoType = contactInfoWrapper.InfoType,
                    IsPrimary = contactInfoWrapper.IsPrimary
                };
        }
    }
}