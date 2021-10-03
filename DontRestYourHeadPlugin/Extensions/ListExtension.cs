using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DontRestYourHeadPlugin.Extensions
{
    public static class ListExtension
    {
        public static int PopHighest(this List<short> list)
        {
            short o = short.MinValue;
            foreach (var val in list)
            {
                if (val > o) o = val;
            }

            list.Remove(o);
            return o;
        }
    }
}
