using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace anghamiApi.VM
{
    public class Constants
    {
        public string TableName { get; set; }
        public string InsertPersonQuery { get; set; }
        public string GetPeopleQuery { get; set; }
        public int Limit { get; set; }
        public string ConnectionString { get; set; }
        public string GetPersonFromEmailQuery { get; set; }
        public string UpdatePersonInDBQuery { get; set; }
        public string UpdateColumn { get; set; }
        public string DeletePersonFromDBQuery { get; set; }
        public string GetFilteredPeopleQuery { get; set; }
        public string UpdateIntColumn { get; set; }
        public string PagingCaching { get; set; }
        public string SearchPeopleQuery { get; set; }
    }
}
