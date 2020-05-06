using System;
using System.Linq;
using DevOpsLeadsApi.Api;
using Xunit;

namespace UnitTests
{
    public class LeadsServiceTests
    {
        [Fact]
        public void ShouldGetLeads()
        {
            var service = new LeadsService();
            var leads = service.GetLeads();
            Assert.Equal(5,leads.Count());
        }
        
        [Fact]
        public void ShouldAddLeads()
        {
            var service = new LeadsService();
            var leads = service.GetLeads().ToList();
            var originalCount = leads.Count();
            var expectedId = leads.Max(l => l.Id) + 1;
            var newLead = CreateLead();
            var result = service.Add(newLead);
            var updatedLeads = service.GetLeads().ToList();
            Assert.Contains(newLead, updatedLeads);
            Assert.Equal(newLead,result);
            Assert.Equal(newLead.Id, expectedId);
            Assert.Equal(originalCount+1,updatedLeads.Count());
        }
        
        [Fact]
        public void ShouldUpdateLeads()
        {
            var service = new LeadsService();
            var leads = service.GetLeads().ToList();
            var leadToUpdate = leads.OrderBy(x => Guid.NewGuid()).First().Clone();
            var originalCount = leads.Count();
            var originalAddress = leadToUpdate.Address;
            leadToUpdate.Address = Faker.Address.StreetAddress();
            service.Update(leadToUpdate);
            var updatedLeads = service.GetLeads().ToList();
            Assert.Contains(leadToUpdate, updatedLeads);
            Assert.All(updatedLeads,l => Assert.NotEqual(originalAddress,l.Address));
            Assert.Equal(originalCount,updatedLeads.Count());
        }
        
        [Fact]
        public void ShouldDeleteLeads()
        {
            var service = new LeadsService();
            var leads = service.GetLeads().ToList();
            var leadToDelete = leads.OrderBy(x => Guid.NewGuid()).First();
            var originalCount = leads.Count();
            service.Delete(leadToDelete.Id.Value);
            var updatedLeads = service.GetLeads().ToList();
            Assert.DoesNotContain(leadToDelete, updatedLeads);
            Assert.All(updatedLeads,l => Assert.NotEqual(leadToDelete.Id,l.Id));
            Assert.Equal(originalCount-1,updatedLeads.Count());
        }
        
        private Lead CreateLead()
        {
            return new Lead(Faker.Name.First(), Faker.Name.Last())
            {
                Address = Faker.Address.StreetAddress(),
                Service = LeadsService.Services[new Random().Next(LeadsService.Services.Length)]
            };
        }
    }
}