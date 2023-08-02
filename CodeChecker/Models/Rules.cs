using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeChecker.Models
{

    public class Rules
    {
        public Rules()
        {

        }
        public Rules(string v1, string v2, int v3)
        {
            nameRules = v1;
            data = v2;
            point = v3;
        }

        public string nameRules { get; set; }
        public string data { get; set; }
        public int point { get; set; }
    }

}
