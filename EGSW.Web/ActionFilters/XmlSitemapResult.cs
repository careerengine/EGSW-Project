using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Globalization;

namespace EGSW.Web.ActionFilters
{
    public class XmlSitemapResult : ActionResult
    {
        private IEnumerable<ISitemapItem> _items;

        public XmlSitemapResult(IEnumerable<ISitemapItem> items)
        {
            _items = items;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            string encoding = context.HttpContext.Response.ContentEncoding.WebName;
            XDocument sitemap = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                 new XElement("urlset", new XAttribute(XNamespace.Xmlns.GetName("Xmlns"), "http://www.sitemaps.org/schemas/sitemap/0.84"),
                      from item in _items
                      select CreateItemElement(item)
                      )
                 );

            context.HttpContext.Response.ContentType = "text/xml";
            context.HttpContext.Response.Flush();
            context.HttpContext.Response.Write(sitemap.Declaration + sitemap.ToString());
        }

        private XElement CreateItemElement(ISitemapItem item)
        {
            XElement itemElement = new XElement("url", new XElement("loc", item.Url.ToLower()));

            if (item.LastModified.HasValue)
                itemElement.Add(new XElement("lastmod", item.LastModified.Value.ToString("yyyy-MM-dd")));

            if (item.ChangeFrequency.HasValue)
                itemElement.Add(new XElement("changefreq", item.ChangeFrequency.Value.ToString().ToLower()));

            if (item.Priority.HasValue)
                itemElement.Add(new XElement("priority", item.Priority.Value.ToString(CultureInfo.InvariantCulture)));

            return itemElement;
        }
    }
}