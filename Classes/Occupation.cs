using Registers.Classes.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registers.Classes
{
    public class Occupation : IdItem
    {
        public string? JobName { get; set; }

        public override string ToString()
        {
            return $"{JobName}";
        }

        public Occupation(string jobName)
        {
            this.Id = Guid.NewGuid();
            this.JobName = jobName;
        }

        public override bool HasReferences()
        {
            // see if any person has this occupation
            throw new NotImplementedException();
        }
    }

    public class TimedOccupation
    {
        public DateTime? Date { get; set; } // when was this person doing this job
        public Guid Occupation {  get; set; }

        public TimedOccupation(DateTime? date, Guid occupation)
        {
            this.Date = date;
            this.Occupation = occupation;
        }
    }
}
