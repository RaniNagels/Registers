using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registers.Classes.ProjectManagement
{
    public class ProjectInfo
    {
        public string FilePath { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }

        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }

        public string Author { get; set; }
    }
}
