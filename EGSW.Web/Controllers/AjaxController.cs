using EGSW.Services;
using EGSW.Services.Directory;
using EGSW.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EGSW.Web.Controllers
{
    public class AjaxController : Controller
    {

        private readonly IZipCodeService _zipCodeService;
        private readonly IWorkflowMessageService _workflowMessageService;

        public AjaxController(IZipCodeService zipCodeService, IWorkflowMessageService workflowMessageService)
        {
            this._zipCodeService = zipCodeService;
            this._workflowMessageService = workflowMessageService;

        }

        // GET: Ajax
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetSongs(string searchTerm)
        {
            List<Songs> songList = new List<Songs>();
            songList.Add(new Songs { Name = "Addat", Artist = "Aatif Aslam" });
            songList.Add(new Songs { Name = "Woh Lamhey", Artist = "Jal - The band" });
            songList.Add(new Songs { Name = "Kryptonite", Artist = "3 Doors Down" });
            songList.Add(new Songs { Name = "Manja", Artist = "Amit Trivedi" });
            songList.Add(new Songs { Name = "Tum hi ho", Artist = "Arjit Singh" });

            //songList = songList.Where(m => m.Name.Contains(searchTerm)).ToList();

            //return songList;
            return Json(songList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCityStateInfoBasedOnZipcode(string inputzipcode)
        {
            var zipcodeResult = _zipCodeService.GetZipCodeDetailByZipcode(inputzipcode);
            //return songList;
            return Json(zipcodeResult, JsonRequestBehavior.AllowGet);
        }


        public JsonResult SendNewServiewRequest(NewServiceRequestModel model)
        {

            if (ModelState.IsValid)
            {
                _workflowMessageService.SendNewServiceRequestSiteOwnerNotification(model.ServiceEmailAdddress, model.ServiceZipCode);
                model.Result = true;

            }
            else
            {
                model.Result = false;
                model.Message = "Somthing Wrong.";
            }


            //return songList;
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }

    public class Songs
    {
        public string Name { get; set; }        
        public string Artist { get; set; }
    }
}