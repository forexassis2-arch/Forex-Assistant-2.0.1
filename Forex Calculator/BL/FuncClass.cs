using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forex_Assistant.BL
{
    public class FuncClass
    {

        public string TimeConStr(DateTime _dtSrc)
        {
            double _Diff = DateTime.UtcNow.Subtract(_dtSrc).TotalMinutes;

            if(_Diff <= 60)
            {
                return "about a " + Convert.ToInt32(_Diff) + " minutes ago";
            }
            else
            {
                return "about an " + Convert.ToInt32(_Diff/60) + " hour ago";
            }
        }

    }
}
