using EGSW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGSW.Services.Directory
{
    public partial class ZipCodeService : IZipCodeService
    {
        private readonly IDbContext _context;
        private readonly IRepository<ZipCode> _zipCodeRepository;

        public ZipCodeService(IDbContext context, IRepository<ZipCode> zipCodeRepository)
        {
            this._context = context;
            this._zipCodeRepository = zipCodeRepository;
        }

        public ZipCode GetZipCodeDetailByZipcode(string zipcode)
        {
            if (string.IsNullOrWhiteSpace(zipcode))
                return null;

            var query = from c in _zipCodeRepository.Table
                        where  c.CityType == "D" && c.ZIPCode1 == zipcode                       
                        select c;
            var zipcodeResult = query.FirstOrDefault();
            return zipcodeResult;
        }
    }
}
