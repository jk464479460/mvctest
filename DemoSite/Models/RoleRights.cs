using System.Collections.Generic;

namespace DemoSite.Models
{
    public class RoleRights
    {
        public int Type { get; set; }
        public int RoleId { get; set; }
        public List<int> Data { get; set; }
    }
}