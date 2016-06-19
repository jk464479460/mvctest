using System.Collections.Generic;

namespace DemoSite.Models
{
    public class TreeNode
    {
        public int ID { get; set; }
        public string text { get; set; }
        public List<TreeNode> children { get; set; }
    }
}