using Domen.Enums;
using Domen.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public static class Utils
    {
        public static Colors Color2Text(Color ComparedColor)
        {
            if (ComparedColor.Red == 100
            && ComparedColor.Green == 100
            && ComparedColor.Blue == 0)
                return Colors.Yellow;

            if (ComparedColor.Red == 55
            && ComparedColor.Green == 55
            && ComparedColor.Blue == 55)
                return Colors.Gray;

            if (ComparedColor.Red == 100
            && ComparedColor.Green == 10
            && ComparedColor.Blue == 10)
                return Colors.Red;

            if (ComparedColor.Red == 100
            && ComparedColor.Green == 100
            && ComparedColor.Blue == 100)
                return Colors.White;

            return Colors.None;
        }

        public static double Distance2Km(Distance distance)
        {
            if (distance.Measure == "m")
            {
                return distance.Value / 1000;
            }
            return distance.Value;
        }
    }
}
