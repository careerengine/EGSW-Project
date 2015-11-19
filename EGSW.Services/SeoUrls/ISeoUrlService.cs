using EGSW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EGSW.Services.SeoUrls
{
    public partial interface ISeoUrlService
    {
        IList<SeoUrl> GetAllSeoUrl();

        IList<SeoUrl> GetCountySeoUrl(string Countyname );



        SeoUrl GetSeoUrlBySeoName(string seoName);

        SeoUrl GetSeoUrlById(int id);

        void UpdateSeoUrl(SeoUrl entity);
    }
}
