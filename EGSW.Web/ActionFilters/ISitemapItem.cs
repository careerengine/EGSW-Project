using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGSW.Web.ActionFilters
{
   public interface ISitemapItem
    {
        string Url { get; }

        DateTime? LastModified { get; }

        ChangeFrequency? ChangeFrequency { get; }

        float? Priority { get; }
    }
}
