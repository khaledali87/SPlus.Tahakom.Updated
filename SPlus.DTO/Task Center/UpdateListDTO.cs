using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.DTO
{
    public class UpdateListDTO
    {
        public int Type { get; set; }
        public int ID { get; set; }
        public string EnglishName { get; set; }
        public string ArabicName { get; set; }
        public DateTime DueDate { get; set; }
        public string UnitOfMeasure { get; set; }
        public UserListDTO ChampionModel { get; set; }
        public UserListDTO OwnerModel { get; set; }
        public string EnglishUnitDetails { get; set; }
        public string ArabicUnitDetails { get; set; }
        public string Status { get; set; }
        public string DataSource { get; set; }
        public bool IsLocked { get; set; }
        public List<KPIMeasureDTO> Measure { get; set; }
        public List<ParameterDTO> Parameters { get; set; }
    }
}
