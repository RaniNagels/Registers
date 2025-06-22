using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registers.Classes.Helpers
{
    class ConfigurationSettings
    {
        [JsonProperty("recent_files")]
        public List<string> RecentFiles { get; set; } = new List<string>();

        [JsonProperty("current_theme")]
        public string CurrentTheme { get; set; } = "Light";
    }
}
