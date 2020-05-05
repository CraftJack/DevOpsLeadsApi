using System;
using System.Collections.Generic;
using System.Linq;
using DevOpsLeadsApi.Api;
using DevOpsLeadsApi.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace UnitTests
{
    public class LeadsControllerTests
    {
        private readonly Mock<ILeadsService> _service;
        private readonly Mock<ILogger<LeadsController>> _logger;
        private readonly LeadsController _controller;

        public LeadsControllerTests()
        {
            _service = new Mock<ILeadsService>();
            _logger = new Mock<ILogger<LeadsController>>();
            _controller = new LeadsController(_service.Object,_logger.Object);
        }

        [Fact]
        public void ShouldGetIndex()
        {
            var expected = new List<Lead>();
            _service.Setup(s => s.GetLeads()).Returns(expected);
            var result = _controller.Get();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Same(expected,okResult.Value);
        }
        
        [Fact]
        public void ShouldGetLead()
        {
            var leads = BuildLeads().ToList();
            var expected = leads.OrderBy(x => Guid.NewGuid()).First();
            _service.Setup(s => s.GetLeads()).Returns(leads);
            var result = _controller.GetLead(expected.Id.Value);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Same(expected,okResult.Value);
        }
        
        [Fact]
        public void ShouldNotGetLead()
        {
            var leads = BuildLeads().ToList();
            _service.Setup(s => s.GetLeads()).Returns(leads);
            var result = _controller.GetLead(-1);
            Assert.IsType<NotFoundResult>(result.Result);
        }
        
        [Fact]
        public void ShouldCreateLead()
        {
            var lead = new Lead(Faker.Name.First(),Faker.Name.Last());
            _service.Setup(s => s.Add(lead)).Returns(() => { 
                lead.Id = 999;
                return lead;
            }).Verifiable();
            
            var result = _controller.CreateLead(lead);
            _service.Verify();
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdLead = Assert.IsType<Lead>(createdResult.Value);
            Assert.NotNull(createdLead.Id);
            Assert.Equal("GetLead",createdResult.ActionName);
            Assert.Equal(createdLead.Id,createdResult.RouteValues["id"]);
        }
        
        
        [Fact]
        public void ShouldUpdateLead()
        {
            var leads = BuildLeads().ToList();
            var toUpdate = leads.OrderBy(x => Guid.NewGuid()).First();
            var result = _controller.UpdateLead(toUpdate.Id.Value,toUpdate);
            _service.Verify(s => s.Update(toUpdate));
            Assert.IsType<NoContentResult>(result);
        }
        
        [Fact]
        public void ShouldFailToUpdateLeadWithBadRequest()
        {
            var leads = BuildLeads().ToList();
            var toUpdate = leads.OrderBy(x => Guid.NewGuid()).First();
            var result = _controller.UpdateLead(toUpdate.Id.Value-1,toUpdate);
            _service.Verify(s => s.Update(toUpdate),Times.Never);
            Assert.IsType<BadRequestResult>(result);
        }
        
        [Fact]
        public void ShouldFailToUpdateLeadWithUnprocessableEntity()
        {
            var leads = BuildLeads().ToList();
            var toUpdate = leads.OrderBy(x => Guid.NewGuid()).First();
            _service.Setup(s => s.Update(toUpdate)).Throws<Exception>().Verifiable();
            var result = _controller.UpdateLead(toUpdate.Id.Value,toUpdate);
            _service.Verify();
            Assert.IsType<UnprocessableEntityResult>(result);
        }
        
        [Fact]
        public void ShouldFailToDeleteLeadWithBadRequest()
        {
            var toDelete = 100;
            _service.Setup(s => s.Delete(toDelete)).Throws<Exception>().Verifiable();
            var result = _controller.DeleteLead(toDelete);
            _service.Verify();
            Assert.IsType<BadRequestResult>(result);
        }
        
        [Fact]
        public void ShouldDeleteLead()
        {
            var toDelete = 100;
            var result = _controller.DeleteLead(toDelete);
            _service.Verify(s => s.Delete(toDelete));
            Assert.IsType<OkResult>(result);
        }
        private static IEnumerable<Lead> BuildLeads()
        {
            return new LeadsService().GetLeads();
        }
    }
}