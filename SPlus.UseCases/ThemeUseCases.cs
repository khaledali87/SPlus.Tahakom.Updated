using AutoMapper;
using SPlus.BLL;
using SPlus.DTO;
using SPlus.Model.Domain;
using StructureMap;
using System.Collections.Generic;
using System.Linq;

namespace SPlus.UseCases
{
    public class ThemeUseCases : LoggingUseCases
    {


        Container _Container = IOC.InitializeContainer();
        private readonly ThemeBLL ThemeBLL;
        public ThemeUseCases()
        {
            ThemeBLL = _Container.GetInstance<ThemeBLL>();
        }


        #region Create

        public CAThemeDTO Create(CAThemeDTO theme)
        {
            if (string.IsNullOrWhiteSpace(theme.ArabicName))
                theme.ArabicName = theme.EnglishName;
            Theme Theme = AutoMapper.Mapper.Map<Theme>(theme);
            var allthemes = ThemeBLL.Read();
            if (allthemes.Any(a => a.Order == theme.Order))
            {
                //throw new System.Exception("Order Must be Unique");
                CAThemeDTO cAThemeDTO = new CAThemeDTO();
                cAThemeDTO.ID = -1;
                cAThemeDTO.EnglishName = "Order Must be Unique";
                return cAThemeDTO;
            }
            else
            {


                var result = ThemeBLL.Create(Theme);
                return AutoMapper.Mapper.Map<CAThemeDTO>(ThemeBLL.ReadByID(result.ID));
            }
        }

        #endregion

        #region Read
        public List<CAThemeDTO> Read()
        {
            var res = ThemeBLL.Read();
            List<CAThemeDTO> Themes = AutoMapper.Mapper.Map<List<CAThemeDTO>>(res);
            return Themes;
        }
        public List<CAThemeStrategicObjectiveDTO> ReadWithStrategicObjectives()
        {
            var res = ThemeBLL.ReadWithStrategicObjectives();
            foreach (var theme in res)
            {
                theme.StrategicObjectives.OrderBy(o => o.Order);
            }
            List<CAThemeStrategicObjectiveDTO> Themes = AutoMapper.Mapper.Map<List<CAThemeStrategicObjectiveDTO>>(res);
            return Themes;
        }
        public CAThemeDTO ReadByID(int ID)
        {
            var res = ThemeBLL.ReadByID(ID);
            CAThemeDTO Theme = AutoMapper.Mapper.Map<CAThemeDTO>(res);
            return Theme;
        }

        #endregion

        #region Update
        public CAThemeDTO Update(CAThemeDTO theme)
        {
            if (string.IsNullOrWhiteSpace(theme.ArabicName))
                theme.ArabicName = theme.EnglishName;

            Theme Theme = Mapper.Map<Theme>(theme);
            var allthemes = ThemeBLL.Read();
            if (allthemes.Any(a => a.Order == theme.Order && a.ID != theme.ID))
            {
                //throw new System.Exception("Order Must be Unique");
                CAThemeDTO cAThemeDTO = new CAThemeDTO();
                cAThemeDTO.ID = -1;
                cAThemeDTO.EnglishName = "Order Must be Unique";
                return cAThemeDTO;

            }
            else
            {
                var res = ThemeBLL.Update(Theme);
                return AutoMapper.Mapper.Map<CAThemeDTO>(res);
            }
        }


        #endregion

        #region Delete
        public bool Delete(int id)
        {
            return ThemeBLL.Delete(id);
        }

        #endregion
    }
}
