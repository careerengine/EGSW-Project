using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EGSW.Services
{
    public partial interface IQuestionAnswerEntityData
    {
        List<SelectListItem> SquareFootageAnswer();

        List<SelectListItem> YearBuiltAnswer();

        List<SelectListItem> StyleOfGuttersAnswer();

        List<SelectListItem> RoofMaterialAnswer();

        List<SelectListItem> DeliveryTimeAnswer();

        decimal CalculateCost(int QuestionSquareFootage, int QuestionYearBuilt, int QuestionStyleOfGutter, string RoofMaterial, int QuestionDeliveryTime);
       
    }
}
