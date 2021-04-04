using anghamiApi.VM;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace anghamiApi.Services
{
    public class ConstantsReaderService
    {
        private IConfiguration configuration;

        public ConstantsReaderService(IConfiguration config)
        {
            configuration = config;
        }

        public Constants ReadConstants()
        {
            return new Constants()
            {
                TableName = configuration.GetValue<string>("TableName"),
                InsertPersonQuery = configuration.GetValue<string>("InsertPersonQuery"),
                GetPeopleQuery = configuration.GetValue<string>("GetPeopleQuery"),
                Limit = configuration.GetValue<int>("Limit"),
                ConnectionString = configuration.GetValue<string>("ConnectionString"),
                GetPersonFromEmailQuery = configuration.GetValue<string>("GetPersonFromEmailQuery"),
                UpdatePersonInDBQuery = configuration.GetValue<string>("UpdatePersonInDBQuery"),
                UpdateColumn = configuration.GetValue<string>("UpdateColumn"),
                DeletePersonFromDBQuery = configuration.GetValue<string>("DeletePersonFromDBQuery"),
                GetFilteredPeopleQuery = configuration.GetValue<string>("GetFilteredPeopleQuery"),
                UpdateIntColumn = configuration.GetValue<string>("UpdateIntColumn"),
                PagingCaching = configuration.GetValue<string>("PagingCaching"),
                SearchPeopleQuery = configuration.GetValue<string>("SearchPeopleQuery")
            };
        }
    }
}
