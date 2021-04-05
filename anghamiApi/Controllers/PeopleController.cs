using anghamiApi.Services;
using anghamiApi.VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace anghamiApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PeopleController : ControllerBase
    {

        private IConfiguration configuration;
        private readonly PeopleService peopleService;
        private readonly ConstantsReaderService constantsReaderService;
        private readonly Constants constants;

        public PeopleController(IConfiguration config, PeopleService people, ConstantsReaderService constantsReader)
        {
            configuration = config;
            peopleService = people;
            constantsReaderService = constantsReader;
            constants = constantsReaderService.ReadConstants();
        }

        [HttpPut]
        [Route("/api/v1/person")]
        public IActionResult InsertPersonToDB([FromBody] Person person)
        {
            if (person.firstname == null)
                return BadRequest("First Name should be present");
            if (person.lastname == null)
                return BadRequest("Last Name should be present");
            if (person.job == null)
                return BadRequest("Job should be present");
            if (person.location == null)
                return BadRequest("Location should be present");
            if (person.phone == null)
                return BadRequest("Phone number should be present");
            if (person.age == default || person.age < 0)
                return BadRequest("Age should be present and positive");
            if (person.email == null)
                return BadRequest("Email should be present");
            if (person.email != null && !peopleService.IsValidEmail(person.email))
                return BadRequest("Invalid email");
            if (person.phone != null && !peopleService.IsValidPhone(person.phone))
                return BadRequest("Invalid phone number");
            if (peopleService.SearchByEmail(person.email))
                return BadRequest("User already exists");

            peopleService.InsertPersonToDB(person);

            return Ok("Inserted successfully");
        }

        [HttpPatch]
        [Route("/api/v1/person/{email}")]
        public IActionResult UpdatePersonInDB(string email, [FromBody] Person person)
        {
            if (email != null && !peopleService.IsValidEmail(email))
                return BadRequest("Invalid email");

            if(!peopleService.SearchByEmail(email))
                return NotFound("User Not Found bl DB aslan");

            peopleService.UpdatePersonInDB(email, person);

            return Ok("Updated successfully");
        }

        [HttpDelete]
        [Route("/api/v1/person/{email}")]
        public IActionResult DeletePersonFromDB(string email)
        {
            if (email != null && !peopleService.IsValidEmail(email))
                return BadRequest("Invalid email");

            if (!peopleService.SearchByEmail(email))
                return NotFound("User Not Found bl DB aslan");

            peopleService.DeletePersonFromDB(email);

            return Ok("Deleted Successfully");
        }

        [HttpGet]
        [Route("/api/v1/people")]
        public IActionResult GetAllPeople(string location, string job, string page, int age = default)
        {
            if (age < 0)
                return BadRequest("Age should be non negative");

            if (location == null && job == null && age == default)
                return Ok(peopleService.GetAllPeople(int.Parse(page)));
            else
                return Ok(peopleService.FilterPeopleFromDB(location, job, age, int.Parse(page)));
        }

        [HttpGet]
        [Route("/api/v1/people/search")]
        public IActionResult SearchPeopleInDB(string firstName, string lastName, string email, string phone, int age = default)
        {
            if (age < 0)
                return BadRequest("Age should be non negative");
            if (email != null && !peopleService.IsValidEmail(email))
                return BadRequest("Invalid email");
            if (phone != null && !peopleService.IsValidPhone(phone))
                return BadRequest("Invalid phone number, must be (xx-xxxxxx)");
            if (firstName == null && lastName == null && age <= 0 && email == null && phone == null)
                return BadRequest("No parameters have been entered");

            return Ok(peopleService.SearchPeopleInDB(firstName, lastName, age, email, phone));
        }
    }
}
