using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EGSW.Services
{
    public partial class QuestionAnswerEntityData : IQuestionAnswerEntityData
    {
        public List<SelectListItem> SquareFootageAnswer()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "< 2000", Value = "2000" });
            items.Add(new SelectListItem { Text = "< 2400", Value = "2400", });
            items.Add(new SelectListItem { Text = "< 2800", Value = "2800", });
            items.Add(new SelectListItem { Text = "< 3000", Value = "3000", });
            items.Add(new SelectListItem { Text = "< 3400", Value = "3400", });
            items.Add(new SelectListItem { Text = "< 3800", Value = "3800", });
            items.Add(new SelectListItem { Text = "< 4000", Value = "4000", });
            items.Add(new SelectListItem { Text = "< 4400", Value = "4400", });
            items.Add(new SelectListItem { Text = "< 4800", Value = "4800", });
            items.Add(new SelectListItem { Text = "< 5200", Value = "5200", });
            items.Add(new SelectListItem { Text = "< 5800", Value = "5800", });
            items.Add(new SelectListItem { Text = "< 6200", Value = "6200", });
            items.Add(new SelectListItem { Text = "< 7000", Value = "7000", });
            items.Add(new SelectListItem { Text = "< 8000", Value = "8000", });
            items.Add(new SelectListItem { Text = "< 9000", Value = "9000", });
            items.Add(new SelectListItem { Text = "< 10000", Value = "10000", });
            items.Add(new SelectListItem { Text = "< 15000", Value = "15000", });
            items.Add(new SelectListItem { Text = "< 20000", Value = "20000", });
            //items.Add(new SelectListItem { Text = "", Value = "", });
            //items.Add(new SelectListItem { Text = "", Value = "", });
            //items.Add(new SelectListItem { Text = "", Value = "", });
            //items.Add(new SelectListItem { Text = "", Value = "", });
            //items.Add(new SelectListItem { Text = "", Value = "", });
            //items.Add(new SelectListItem { Text = "", Value = "", });
            //items.Add(new SelectListItem { Text = "", Value = "", });
            //items.Add(new SelectListItem { Text = "", Value = "", });



            return items;
        }

        public List<SelectListItem> YearBuiltAnswer()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            //for (int i = 1900; i <= DateTime.Now.Year; i++)
            //    items.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });

            items.Add(new SelectListItem { Text = "1900 - 1934", Value = "1", });
            items.Add(new SelectListItem { Text = "1935 - 1989", Value = "2", });
            items.Add(new SelectListItem { Text = "1990 - Present", Value = "3", });

            return items;
        }

        public List<SelectListItem> StyleOfGuttersAnswer()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Front + Back", Value = "1", });
            items.Add(new SelectListItem { Text = "Front, Back + Sides", Value = "2", });
            //items.Add(new SelectListItem { Text = "", Value = "", });
            //items.Add(new SelectListItem { Text = "", Value = "", });
            //items.Add(new SelectListItem { Text = "", Value = "", });
            //items.Add(new SelectListItem { Text = "", Value = "", });
            //items.Add(new SelectListItem { Text = "", Value = "", });
            //items.Add(new SelectListItem { Text = "", Value = "", });

            return items;
        }

        public List<SelectListItem> RoofMaterialAnswer()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Asphalt", Value = "Asphalt", });
            items.Add(new SelectListItem { Text = "Cedar", Value = "Cedar", });
            items.Add(new SelectListItem { Text = "Copper", Value = "Copper", });
            items.Add(new SelectListItem { Text = "Shingle", Value = "Shingle", });
            items.Add(new SelectListItem { Text = "Slate", Value = "Slate", });
            //items.Add(new SelectListItem { Text = "", Value = "", });

            return items;
        }

        public List<SelectListItem> DeliveryTimeAnswer()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Within 5 business days", Value = "1", });
            items.Add(new SelectListItem { Text = "Within 8 business hours – 2x price", Value = "2", });
            items.Add(new SelectListItem { Text = "Within 4 business hours”- 3x price", Value = "3", });
            

            return items;
        }

        public decimal CalculateCost(int QuestionSquareFootage, int QuestionYearBuilt, int QuestionStyleOfGutter, string RoofMaterial, int QuestionDeliveryTime)
        {
            decimal totalcost = 00M;
            decimal result;
            result=GetQuestionSquareFootageCost(QuestionSquareFootage, totalcost);
            result = GetYearBuiltCost(QuestionYearBuilt, result);
            result = GetStyleOfGuttersCost(QuestionStyleOfGutter, result);
            result = GetRoofMaterialAnswerCost(RoofMaterial, result);
            result = GetDeliveryTimeCost(QuestionDeliveryTime, result);
            totalcost = result;
            return totalcost;
        }


        private decimal GetQuestionSquareFootageCost(int QuestionSquareFootage, decimal amount )
        {
            decimal cost = 00M;

            if (QuestionSquareFootage > 0 && QuestionSquareFootage == 2000)
                cost = 86.00M;

            if (QuestionSquareFootage > 2000 && QuestionSquareFootage == 2400)
                cost = 96.00M;

            if (QuestionSquareFootage > 2400 && QuestionSquareFootage == 2800)
                cost = 106.00M;

            if (QuestionSquareFootage > 2800 && QuestionSquareFootage == 3000)
                cost = 116.00M;

            if (QuestionSquareFootage > 3000 && QuestionSquareFootage == 3400)
                cost = 126.00M;

            if (QuestionSquareFootage > 3400 && QuestionSquareFootage == 3800)
                cost = 146.00M;

            if (QuestionSquareFootage > 3800 && QuestionSquareFootage == 4000)
                cost = 166.00M;




            if (QuestionSquareFootage > 4000 && QuestionSquareFootage == 4400)
                cost = 176.00M;

            if (QuestionSquareFootage > 4400 && QuestionSquareFootage == 4800)
                cost = 186.00M;

            if (QuestionSquareFootage > 4800 && QuestionSquareFootage == 5200)
                cost = 196.00M;

            if (QuestionSquareFootage > 5200 && QuestionSquareFootage == 5800)
                cost = 206.00M;

            if (QuestionSquareFootage > 5800 && QuestionSquareFootage == 6200)
                cost = 216.00M;

            if (QuestionSquareFootage > 6200 && QuestionSquareFootage == 7000)
                cost = 246.00M;



            if (QuestionSquareFootage > 7000 && QuestionSquareFootage == 8000)
                cost = 256.00M;


            if (QuestionSquareFootage > 8000 && QuestionSquareFootage == 9000)
                cost = 266.00M;

            if (QuestionSquareFootage > 9000 && QuestionSquareFootage == 10000)
                cost = 306.00M;

            if (QuestionSquareFootage > 10000 && QuestionSquareFootage == 15000)
                cost = 406.00M;

            if (QuestionSquareFootage > 15000 && QuestionSquareFootage == 20000)
                cost = 506.00M;

            return cost;
        }


        private decimal GetYearBuiltCost(int QuestionYearBuilt, decimal amount)
        {
            decimal cost = amount;
            decimal percentag = 00M;
            if (QuestionYearBuilt == 1)
                percentag = 18;

            if (QuestionYearBuilt == 2)
                percentag = 0;

            if (QuestionYearBuilt == 3)
                percentag = 18;

            //calculate discount amount
            decimal result = 00m;
            if (percentag > 0)
            {
                result = (decimal)((((float)cost) * ((float)percentag)) / 100f);
            }

            return cost + result;


        }


        private decimal GetStyleOfGuttersCost(int QuestionStyleOfGutter, decimal amount)
        {
            decimal cost = amount;
            decimal percentag = 00M;
            if (QuestionStyleOfGutter == 1)
                percentag = 0;

            if (QuestionStyleOfGutter == 2)
                percentag = 20;



            //calculate discount amount
            decimal result = 00m;
            if (percentag > 0)
            {
                result = (decimal)((((float)cost) * ((float)percentag)) / 100f);
            }

            return cost + result; ;
        }

        private decimal GetRoofMaterialAnswerCost(string RoofMaterial, decimal amount)
        {
            decimal cost = amount;
            decimal percentag = 00M;
            if (RoofMaterial.ToLower() == "asphalt" || RoofMaterial.ToLower() == "shingle")
                percentag = 1;

            if (RoofMaterial.ToLower() == "cedar" || RoofMaterial.ToLower() == "slate")
                percentag = 2;

            if (RoofMaterial.ToLower() == "Copper")
                percentag = 1.5M;

            //calculate discount amount
            decimal result = cost;
            if (percentag > 0)
            {
                result = (decimal)(((float)cost) * ((float)percentag));
            }

            return  result;

          
        }

        private decimal GetDeliveryTimeCost(int DeliveryTime, decimal amount)
        {
            decimal cost = amount;
            
            decimal percentag = 1;
            
            if (DeliveryTime == 1)
                percentag = 1;

            if (DeliveryTime == 2)
                percentag = 2;

            if (DeliveryTime == 3)
                percentag = 3;



            //calculate discount amount
            decimal result = 00m;
            if (percentag > 0)
            {
                result = (decimal)(((float)cost) * ((float)percentag));
            }

            return  result;
        }


       
    }
}
