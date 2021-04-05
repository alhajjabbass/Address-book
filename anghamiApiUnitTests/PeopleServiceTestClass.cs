using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace anghamiApiUnitTests
{
    [TestClass]
    public class PeopleServiceTestClass
    {
        private readonly PeopleService peopleService;

        public PeopleServiceTestClass(PeopleService service)
        {
            peopleService = service;
        }

        [TestMethod]
        public void Asserts_GetAllPeople_Returns_List()
        {
            //Arrange
            int page = 1;

            //Act
            var response = peopleService.GetAllPeople(page);

            //Assert
            Assert.IsInstanceOfType(response, typeof(List<Person>));
        }
    }
}
