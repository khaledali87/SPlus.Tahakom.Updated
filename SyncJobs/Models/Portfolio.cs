using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncJobs.Models
{
    public class Portfolio
    {
        public int Id
        {
            set;
            get;
        }
        public string NameEnglish
        {
            set;
            get;
        }

        public string NameArabic
        {
            set;
            get;
        }

        public int Progress
        {
            set;
            get;
        }

        public string Status
        {
            set;
            get;
        }
        public string ProjectUID { get; set; }
    }

    public class AllData 
    {
        Data Data;

}

    public class Data {


        List<Portfolio> Portfolios;
    }





}
