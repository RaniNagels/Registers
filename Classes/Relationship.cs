using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registers.Classes
{
    public class Relationship
    {
        public Guid Id { get; set; }
        public Guid FromPersonId { get; set; }
        public Guid ToPersonId { get; set; }
        public RelationshipType Type { get; set; }
    }

    public enum RelationshipType
    {
        Mother,
        Father,
        Sister,
        Brother,
        Daughter,
        Son
    }
}
