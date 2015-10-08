using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EGSW.Web.ActionFilters
{
    public class SitemapItem:ISitemapItem
    {
        public SitemapItem(string url)
        {
            Url = url;
        }

        public string Url { get; set; }

        public DateTime? LastModified { get; set; }

        public ChangeFrequency? ChangeFrequency { get; set; }

        public float? Priority { get; set; }
    }
}