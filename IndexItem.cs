using System;
using System.Collections.Generic;
using System.Text;

namespace UpdateSearchIndex
{
    public class IndexItem
    {
        public string id { get; set; }
        public string caption { get; set; }
        public string photoId { get; set; }
        public string url { get; set; }
        public string userId { get; set; }
    }
}