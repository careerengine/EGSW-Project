using EGSW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EGSW.Services.SeoUrls
{
    public partial class SeoUrlService : ISeoUrlService
    {
        private readonly IRepository<SeoUrl> _seoUrlRepository;

        public SeoUrlService(IRepository<SeoUrl> seoUrlRepository)
        {
            this._seoUrlRepository = seoUrlRepository;            
        }


        public SeoUrl GetSeoUrlBySeoName(string seoName)
        {
            var query = _seoUrlRepository.Table;

            var result = query.Where(o => o.SeoName == seoName.ToString()).FirstOrDefault();

            return result;
        }


        public SeoUrl GetSeoUrlById(int id)
        {
            if (id == 0)
                return null;

            return _seoUrlRepository.GetById(id);
        }

        public IList<SeoUrl> GetAllSeoUrl()
        {
            var query = _seoUrlRepository.Table;
            return query.ToList();
        }


        /// <summary>
        /// Updates the customer
        /// </summary>
        /// <param name="customer">Customer</param>
        public virtual void UpdateSeoUrl(SeoUrl entity)
        {
            if (_seoUrlRepository == null)
                throw new ArgumentNullException("SeoUrl");

            _seoUrlRepository.Update(entity);
        }

    }
}
