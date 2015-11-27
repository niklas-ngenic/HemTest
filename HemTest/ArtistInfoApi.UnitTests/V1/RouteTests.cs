using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Http;
using System.Net.Http;
using ArtistInfoApi.V1.Controllers;

namespace ArtistInfoApi.UnitTests.V1
{
    [TestClass]
    public class RouteTests
    {
        string artistInfo = string.Format("http://domain.com/api/v1/artistinfo/{0}", Guid.NewGuid());

        HttpConfiguration config;

        [TestInitialize]
        public void Initialize()
        {
            config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            SetupRoutesExtension.SetRoutes(config);
        }

        [TestMethod]
        public void Get_ArtistInfo()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, artistInfo);
            var routeTester = new RouteTester(config, request);
            Assert.AreEqual(typeof(ArtistInfoController), routeTester.GetControllerType());
            Assert.AreEqual(ReflectionHelper.GetMethodName((ArtistInfoController c) => c.Get(Guid.NewGuid())), routeTester.GetActionName());
        }
    }
}
