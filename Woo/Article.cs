using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Woo
{
    public class Article
    {
        public int ID { get; set; }

        public string Title { get; set; }
        public string Link { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }

        public DateTime WriteTime { get; set; }

        public Article()
        {
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
