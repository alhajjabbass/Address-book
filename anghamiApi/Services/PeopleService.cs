using anghamiApi.VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace anghamiApi.Services
{
    public class PeopleService
    {
        private readonly ConstantsReaderService constantsReaderService;
        private readonly Constants constants;
        private readonly NpgsqlConnection conn;

        public PeopleService(ConstantsReaderService constantsReader)
        {
            //Take ConstantsReaderService that's a singleton class
            constantsReaderService = constantsReader;
            constants = constantsReaderService.ReadConstants();

            //Get the connection string from the constants reader service, which in turns gets it from the app.setting file
            string ConnectionString = constants.ConnectionString;
            //Create only 1 connection that is reused during the entire code
            //This might make a problem with connection pooling, something I would look up if I had more time
            conn = new NpgsqlConnection(ConnectionString);
        }

        public List<Person> GetAllPeople(int page)
        {
            try
            {
                conn.Close();
                conn.Open();

                //Implement the caching
                ObjectCache cache = MemoryCache.Default;

                //If List is found in the cache, return it, else, send the request to the DB
                if (cache.Contains(constants.TableName))
                {
                    return (List<Person>)cache.Get(constants.TableName);
                }

                List<Person> people = new List<Person>();

                //Get the sql query from teh constants reader service, which in turns reads it from the app.settings file
                string sql = constants.GetPeopleQuery;

                //Format sql with the right params and pagination
                //adding the table name to the sql
                //adding the offset and limit for pagination, each page should contain 10 so the offset should be the page*10(limit) - 10(limit)
                sql = string.Format(sql, constants.TableName, (constants.Limit * (page - 1)).ToString(), (constants.Limit * page).ToString());

                using (NpgsqlCommand command = new NpgsqlCommand(sql, conn))
                {
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        //Get each person's values from the reader
                        Person person = new Person
                        {
                            firstname = reader[1].ToString(),
                            lastname = reader[2].ToString(),
                            age = int.Parse(reader[3].ToString()),
                            email = reader[4].ToString(),
                            phone = reader[5].ToString(),
                            job = reader[6].ToString(),
                            location = reader[7].ToString()
                        };

                        //Adding them to the list to be returned
                        people.Add(person);
                    }
                }
                conn.Close();

                // Store data in the cache    
                // Using the table name as the cachekey
                CacheItemPolicy cacheItemPolicy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(5.0)
                };
                cache.Add(constants.TableName, people, cacheItemPolicy);

                return people;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.StackTrace);
                //Log to Exception table if I had one
                return new List<Person>();
            }
        }

        public void InsertPersonToDB(Person person)
        {
            try
            {
                conn.Close();
                conn.Open();

                //Format the sql query with the appropriate parameters
                string sql = constants.InsertPersonQuery;
                sql = string.Format(sql, constants.TableName, person.firstname, person.lastname, person.age, person.email, person.phone, person.job, person.location);

                var cmd = new NpgsqlCommand
                {
                    Connection = conn,
                    CommandText = sql
                };
                cmd.ExecuteNonQuery();

                conn.Close();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.StackTrace);
                //Log to Exception table if I had one
            }
        }

        public void UpdatePersonInDB(string email, Person person)
        {
            try
            {
                conn.Close();
                conn.Open();

                string sql = constants.UpdatePersonInDBQuery;

                string updates = string.Empty;
                string update = constants.UpdateColumn;

                //Adding the updates to the query
                //if the variable is not null, add <column_name>=<variable> as an update condition
                if (person.phone != null)
                {
                    string temp = string.Format(update, "phone", person.phone);
                    updates += temp;
                }
                if (person.job != null)
                {
                    string temp = string.Format(update, "job", person.job);
                    updates += updates == string.Empty ? temp : ", " + temp;
                }
                if (person.location != null)
                {
                    string temp = string.Format(update, "location", person.location);
                    updates += updates == string.Empty ? temp : ", " + temp;
                }

                //Format the query with the appropriate parameters.
                sql = string.Format(sql, constants.TableName, updates, email);

                var cmd = new NpgsqlCommand
                {
                    Connection = conn,
                    CommandText = sql
                };
                cmd.ExecuteNonQuery();

                conn.Close();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.StackTrace);
                //Log to Exception table if I had one
            }
        }

        public bool SearchByEmail(string email)
        {
            try
            {
                conn.Close();
                conn.Open();

                var command = new NpgsqlCommand
                {
                    Connection = conn
                };

                string sqlEmail = constants.GetPersonFromEmailQuery;
                sqlEmail = string.Format(sqlEmail, constants.TableName, email);

                command.CommandText = sqlEmail;

                NpgsqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.StackTrace);
                throw exception;
            }
        }

        public void DeletePersonFromDB(string email)
        {
            try
            {
                conn.Close();
                conn.Open();

                string sql = constants.DeletePersonFromDBQuery;
                sql = string.Format(sql, constants.TableName, email);

                var cmd = new NpgsqlCommand
                {
                    Connection = conn,
                    CommandText = sql
                };
                cmd.ExecuteNonQuery();

                conn.Close();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.StackTrace);
                //Log to Exception table if I had one
                throw exception;
            }
        }

        public List<Person> FilterPeopleFromDB(string location, string job, int age, int page)
        {
            try
            {
                conn.Close();
                conn.Open();

                List<Person> people = new List<Person>();

                //Get the sql query from teh constants reader service, which in turns reads it from the app.settings file
                string sql = constants.GetFilteredPeopleQuery;

                string updates = string.Empty;
                string update = constants.UpdateColumn;
                string updateInt = constants.UpdateIntColumn;

                if (job != null)
                {
                    string temp = string.Format(update, "job", job);
                    updates += temp;
                }
                if (location != null)
                {
                    string temp = string.Format(update, "location", location);
                    updates += updates == string.Empty ? temp : "AND " + temp;
                }
                if (age != default)
                {
                    string temp = string.Format(updateInt, "age", age);
                    updates += updates == string.Empty ? temp : "AND " + temp;
                }

                //Format sql with the right params and pagination
                //adding the table name to the sql
                //adding the offset and limit for pagination, each page should contain 10 so the offset should be the page*10(limit) - 10(limit)
                sql = string.Format(sql, constants.TableName, updates, (constants.Limit * (page - 1)).ToString(), (constants.Limit * page).ToString());

                using (NpgsqlCommand command = new NpgsqlCommand(sql, conn))
                {
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        //Get each person's values from the reader
                        Person person = new Person
                        {
                            firstname = reader[1].ToString(),
                            lastname = reader[2].ToString(),
                            age = int.Parse(reader[3].ToString()),
                            email = reader[4].ToString(),
                            phone = reader[5].ToString(),
                            job = reader[6].ToString(),
                            location = reader[7].ToString()
                        };

                        //Adding them to the list to be returned
                        people.Add(person);
                    }
                }
                conn.Close();

                return people;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.StackTrace);
                //Log to Exception table if I had one and skip to return new List
                return new List<Person>();
            }
        }

        public List<Person> SearchPeopleInDB(string firstName, string lastName, int age, string email, string phone)
        {
            try
            {
                conn.Close();
                conn.Open();

                List<Person> people = new List<Person>();

                //Get the sql query from teh constants reader service, which in turns reads it from the app.settings file
                string sql = constants.SearchPeopleQuery;

                string updates = string.Empty;
                string update = constants.UpdateColumn;
                string updateInt = constants.UpdateIntColumn;

                if (firstName != null)
                {
                    string temp = string.Format(update, "firstname", firstName);
                    updates += temp;
                }
                if (lastName != null)
                {
                    string temp = string.Format(update, "lastname", lastName);
                    updates += updates == string.Empty ? temp : "AND " + temp;
                }
                if (age != default)
                {
                    string temp = string.Format(updateInt, "age", age);
                    updates += updates == string.Empty ? temp : "AND " + temp;
                }
                if (email != null)
                {
                    string temp = string.Format(update, "email", email);
                    updates += updates == string.Empty ? temp : "AND " + temp;
                }
                if (phone != null)
                {
                    string temp = string.Format(update, "phone", phone);
                    updates += updates == string.Empty ? temp : "AND " + temp;
                }

                //adding the table name to the sql
                sql = string.Format(sql, constants.TableName, updates);

                using (NpgsqlCommand command = new NpgsqlCommand(sql, conn))
                {
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        //Get each person's values from the reader
                        Person person = new Person
                        {
                            firstname = reader[1].ToString(),
                            lastname = reader[2].ToString(),
                            age = int.Parse(reader[3].ToString()),
                            email = reader[4].ToString(),
                            phone = reader[5].ToString(),
                            job = reader[6].ToString(),
                            location = reader[7].ToString()
                        };

                        //Adding them to the list to be returned
                        people.Add(person);
                    }
                }
                conn.Close();

                return people;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.StackTrace);
                //Log to Exception table if I had one and skip to return new List
                return new List<Person>();
            }
        }

        //Check the validity of an email
        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        //Regex expression to check the validity of a phone number with the lebanese format
        public bool IsValidPhone(String input)
        {
            Regex regex = new Regex("^[0-9]+-[0-9]+$", RegexOptions.IgnoreCase);
            return regex.IsMatch(input);
        }
    }
}
