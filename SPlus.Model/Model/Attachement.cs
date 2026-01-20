using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class Attachement
    {

        public string AttachementID
        {
            get;
            set;
        }

        public string AttachementName
        {
            get;
            set;
        }

        public string AttachementContent
        {
            set;
            get;
        }

        public string AttachementType
        {
            set;
            get;
        }

        public string AttachementSize
        {
            set;
            get;
        }

        public string AttachementDate
        {
            set;
            get;
        }
        public string ItemType
        {
            set;
            get;
        }
        public byte[] AttachementDateAsByte
        {
            set;
            get;
        }
        public string AttachementURL
        {
            set;
            get;
        }
        public int KPIID
        {
            set;
            get;
        }
        public int RelatedItemID
        {
            set;
            get;
        }
        public int Value
        {
            set;
            get;
        }
        public string Status
        {
            set;
            get;
        }

    }
}
