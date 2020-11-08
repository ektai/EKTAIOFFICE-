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


using ASC.HealthCheck.Models;
using ASC.HealthCheck.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ASC.TestHealthCheck
{
    [TestClass]
    public class ServiceRepositoryUnitTest
    {
        private const string JabberService = "EKTAIOFFICESvcJabber";

        [TestMethod]
        public void ServiceRepositoryAddRemove()
        {
            IServiceRepository serviceRepository = new ServiceRepository();
            serviceRepository.Add(JabberService);
            Assert.AreEqual(true, serviceRepository.HasAtempt(JabberService));
            var service = serviceRepository.GetService(JabberService);
            Assert.AreEqual(0, service.Attempt);
            Assert.AreEqual(JabberService, service.ServiceName);
            serviceRepository.Remove(JabberService);
            try
            {
                serviceRepository.GetService(JabberService);
            }
            catch (InvalidOperationException)
            {
                // Expected exception
                return;
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            Assert.Fail("Error! Never get this command.");
        }

        [TestMethod]
        public void ServiceRepositorySetStates()
        {
            var serviceRepository = new ServiceRepository();
            serviceRepository.Add(JabberService);
            serviceRepository.SetStates(JabberService, HealthCheckResource.StatusServiceStoped, HealthCheckResource.ServiceStop);
            var service = serviceRepository.GetService(JabberService);
            Assert.AreEqual(0, service.Attempt);
            Assert.AreEqual(JabberService, service.ServiceName);
            Assert.AreEqual(HealthCheckResource.ServiceStop, service.Message);
        }

        [TestMethod]
        public void ServiceRepositoryDropAttemptsShouldRestart()
        {
            var serviceRepository = new ServiceRepository();
            serviceRepository.Add(JabberService);
            Assert.AreEqual(true, serviceRepository.HasAtempt(JabberService));
            var service = serviceRepository.GetService(JabberService);
            Assert.AreEqual(0, service.Attempt);
            Assert.AreEqual(false, serviceRepository.ShouldRestart(JabberService));
            Assert.AreEqual(1, service.Attempt);
            serviceRepository.DropAttempt(JabberService);
            Assert.AreEqual(0, service.Attempt);
        }
    }
}
