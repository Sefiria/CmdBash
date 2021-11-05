using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmdBash
{
    public class Project
    {
        [JsonProperty("ProjectName")]
        public string Name;

        public Project(string Name)
        {
            this.Name = Name;
        }
    }
}
