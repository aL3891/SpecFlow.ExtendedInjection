using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace betterbinding
{
    public class KeyAttribute : Attribute
    {
        public string Key { get; set; }
    }

    public class ReturnKeyAttribute : Attribute
    {
        public string Key { get; set; }
    }
}
