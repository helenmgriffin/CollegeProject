using Microsoft.VisualStudio.TestTools.UnitTesting;
using CollegeProject;
using CollegeProject.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using CollegeProject.Models;

namespace CollegeProject.Test.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        private readonly IConfiguration _configuration;
        public HomeControllerTest()
        {
            //Arrange
            var inMemorySettings = new Dictionary<string, string> {
                {"AWS:GetEndpointUrl", "https://p5fpyxi8ha.execute-api.eu-west-1.amazonaws.com/prod/"},
                {"AWS:GetByIDEndpointUrl", "https://74sug7mg0d.execute-api.eu-west-1.amazonaws.com/prod/"},
                {"AWS:CreateEndpointUrl", "https://7tmjsi9uk4.execute-api.eu-west-1.amazonaws.com/prod/"},
                {"AWS:UpdateEndpointUrl", "https://q3worczk9d.execute-api.eu-west-1.amazonaws.com/prod/"}
            };

            IConfiguration Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _configuration = Configuration;
        }
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController(_configuration);
            // Act
            ViewResult result = controller.Index() as ViewResult;

            //Assert the View is not null
            Assert.IsNotNull(result);
            //Assert Model is not null
            Assert.IsNotNull(result.Model, "The Model (IList<Tickets>) is null");
            // Assert tickets were returned
            IList<Ticket> tickets = (IList<Ticket>)result.Model;
            Assert.IsTrue(tickets.Count > 0, "The ticket count was not greater than 0");
        }

        [TestMethod]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController(_configuration);
            // Act
            ViewResult result = controller.About() as ViewResult;
            // Assert
            Assert.AreEqual("Your application description page.", result.ViewData["Message"]);
        }

        [TestMethod]
        public void Contact()
        {
            // Arrange
            HomeController controller = new HomeController(_configuration);
            // Act
            ViewResult result = controller.Contact() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }
    }
}
