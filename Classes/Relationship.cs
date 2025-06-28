using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registers.Classes
{
    public class Relationship : IdItem
    {
        public Guid PersonId { get; set; }
        public Guid ToPersonId { get; set; }
        public RelationshipType Type { get; set; }

        public override bool HasReferences()
        {
            if (PersonId == Guid.Empty || ToPersonId == Guid.Empty)
                return false;
            else return true;
        }
    }

    public enum RelationshipType
    {
        Mother,
        Father,
        Sibling,
        HalfSibling,
        Child,
        Spouse
    }
}
