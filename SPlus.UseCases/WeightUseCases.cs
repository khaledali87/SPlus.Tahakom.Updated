using AutoMapper;
using SPlus.BLL;
using SPlus.DTO;
using SPlus.Helper;
using SPlus.Model.Domain;
using StructureMap;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using model = SPlus.Model;

namespace SPlus.UseCases
{
    public class WeightUseCases : LoggingUseCases
    {

        Container _Container = IOC.InitializeContainer();
        private readonly ThemeBLL ThemeBLL;
        private readonly StrategyBLL StrategyBLL;
        private readonly KPIBLL KPIBLL;
        private readonly StrategicObjectiveBLL StrategicObjectiveBLL;
        private readonly OrgStructureBLL OrgStructureBLL;
        private readonly DivisionalObjectiveBLL DivisionalObjectiveBLL;
        public WeightUseCases()
        {
            StrategyBLL = _Container.GetInstance<StrategyBLL>();
            ThemeBLL = _Container.GetInstance<ThemeBLL>();
            StrategicObjectiveBLL = _Container.GetInstance<StrategicObjectiveBLL>();
            KPIBLL = _Container.GetInstance<KPIBLL>();
            OrgStructureBLL = _Container.GetInstance<OrgStructureBLL>();
            DivisionalObjectiveBLL = _Container.GetInstance<DivisionalObjectiveBLL>();
        }
        #region Read


        public List<CAStrategyWeightDTO> ReadWeightingStrategicList(int? Year)
        {
            
            var stratagies = StrategyBLL.ReadForWeightStrategy();
            List<CAStrategyWeightDTO> finalList = new List<CAStrategyWeightDTO>();
            CAStrategyWeightDTO final = null;

            //if (Year.HasValue)
            //stratagies = stratagies.Where(a => a.Year == Year).ToList();
            if (Year.HasValue)
            {
                stratagies = stratagies.Where(w => w.Years.Any(a => a == Year.ToString())).ToList();
            }
            foreach (var strategy in stratagies)
            {
                foreach (var theme in strategy.Themes)
                {
                    foreach (StrategicObjective strategicObjective in theme.StrategicObjectives)
                    {
                        strategicObjective.KPIs = strategicObjective.KPIs.Where(a => !a.DivisionalObjectiveID.HasValue && a.KPIType.IsDepartmental == false).ToList();
                        if (Year.HasValue)
                        {
                            strategicObjective.KPIs = strategicObjective.KPIs.Where(a => a.StartDate.Year == Year).ToList();
                        }
                    }

                }
                final = new CAStrategyWeightDTO();
                final = Mapper.Map<CAStrategyWeightDTO>(strategy);
                finalList.Add(final);
            }


            return finalList;
        }

        public ObjectiveWeightDTO WeightByObjectiveId(int id, string type, int? Year = null)
        {
            if (type.ToLower() == "strategic")
            {
                var stratagies = StrategyBLL.ReadForWeightStrategyBystartegicId(id);
                if (Year.HasValue)
                {
                    stratagies = stratagies.Where(w => w.Years.Any(a => a == Year.ToString())).ToList();
                }
                List<CAStrategyWeightDTO> finalList = new List<CAStrategyWeightDTO>();
                CAStrategyWeightDTO final = null;
                foreach (var strategy in stratagies)
                {
                    foreach (var theme in strategy.Themes)
                    {
                        foreach (StrategicObjective strategicObjective in theme.StrategicObjectives)
                        {
                            strategicObjective.KPIs = strategicObjective.KPIs.Where(a => a.KPIType.IsDepartmental == false).ToList();
                            if (Year.HasValue)
                            {
                                strategicObjective.KPIs = strategicObjective.KPIs.Where(a => a.StartDate.Year == Year).ToList();
                            }
                        }


                    }
                    final = new CAStrategyWeightDTO();
                    final = Mapper.Map<CAStrategyWeightDTO>(strategy);
                    finalList.Add(final);
                }

                ObjectiveWeightDTO cAStrategicObjectiveWeightDTO = new ObjectiveWeightDTO();
                cAStrategicObjectiveWeightDTO = Mapper.Map<ObjectiveWeightDTO>(finalList.SelectMany(a => a.Themes).SelectMany(a => a.StrategicObjectives).Where(a => a.ID == id).FirstOrDefault());
                return cAStrategicObjectiveWeightDTO;
            }

            else
            {
                var ReadWithDivisionalObjectives = StrategicObjectiveBLL.ReadWithDivisionalObjectives();
                //var divisionals = DivisionalObjectiveBLL.ReadForWeightDepartmentalObjectie();
                List<CAStrategicObjectiveDivisionalWeightDTO> finalList = new List<CAStrategicObjectiveDivisionalWeightDTO>();
                CAStrategicObjectiveDivisionalWeightDTO final = null;

                foreach (var objective in ReadWithDivisionalObjectives)
                {
                    if (objective.KPIs != null)
                    {
                        objective.KPIs = objective.KPIs.Where(a => a.KPIType?.IsDepartmental == true).ToList();
                        if (Year.HasValue)
                        {
                            objective.KPIs = objective.KPIs.Where(a => a.StartDate.Year == Year).ToList();
                        }

                    }

                    final = new CAStrategicObjectiveDivisionalWeightDTO();
                    final = Mapper.Map<CAStrategicObjectiveDivisionalWeightDTO>(objective);

                    // final.DivisionalObjective = Mapper.Map<List<CADivisionalObjectiveWeightDTO>>(org);
                    finalList.Add(final);
                }
                ObjectiveWeightDTO cAStrategicObjectiveWeightDTO = new ObjectiveWeightDTO();
               
                cAStrategicObjectiveWeightDTO = Mapper.Map<ObjectiveWeightDTO>(finalList.SelectMany(a => a.DivisionalObjectives).Where(a => a.ID == id).FirstOrDefault());
                return cAStrategicObjectiveWeightDTO;

            }
        }

        public List<CAStrategicObjectiveDivisionalWeightDTO> ReadForWeightDivisional(int? Year)
        {

            var DivisionalObjectives = StrategicObjectiveBLL.ReadWithDivisionalObjectives();
            //var divisionals = DivisionalObjectiveBLL.ReadForWeightDepartmentalObjectie();
            List<CAStrategicObjectiveDivisionalWeightDTO> finalList = new List<CAStrategicObjectiveDivisionalWeightDTO>();
            CAStrategicObjectiveDivisionalWeightDTO final = null;

            foreach (var objective in DivisionalObjectives)
            {
                if (Year.HasValue)
                {
                    foreach (var item in objective.DivisionalObjectives)
                    {
                        item.KPIs = item.KPIs.Where(a => a.StartDate.Year == Year).ToList();
                    }
                }
                final = new CAStrategicObjectiveDivisionalWeightDTO();
                final = Mapper.Map<CAStrategicObjectiveDivisionalWeightDTO>(objective);

                // final.DivisionalObjective = Mapper.Map<List<CADivisionalObjectiveWeightDTO>>(org);
                finalList.Add(final);
            }

            return finalList;
        }

        public List<OrgStructureWeightDTO> ReadOrgStructureWeight(string UserName, int? Year)
        {
            var orgStructures = OrgStructureBLL.Read(UserName,Year);
            var KPIs = KPIBLL.Read(Year);
            if (Year.HasValue)
            {
                orgStructures = orgStructures.Where(a => a.Years.Any(year => year == Year.ToString())).ToList();
                KPIs = KPIs.Where(a => a.StartDate.Year == Year).ToList();
            }
            //var divisionals = DivisionalObjectiveBLL.ReadForWeightDepartmentalObjectie();
            List<OrgStructureWeightDTO> finalList = new List<OrgStructureWeightDTO>();
            OrgStructureWeightDTO final = null;

            Func<KPI, bool> CorporateOrgStructureFilterPredicate = a =>
             (!a.DivisionalObjectiveID.HasValue && a.OrgStructureID.HasValue &&
                 (a.OrgStructureID.Value == Constants.CorporateDepartmentID ||
                 a.OrgStructureID.Value == Constants.CorporateSectortID)) ||
                 (a.DivisionalObjectiveID.HasValue && a.DivisionalObjective != null &&
                 (a.DivisionalObjective.OrgStructureId == Constants.CorporateDepartmentID ||
                 a.DivisionalObjective.OrgStructureId == Constants.CorporateSectortID));


            foreach (var orgStructure in orgStructures)
            {
                if (orgStructure.ID == Constants.CorporateDepartmentID || orgStructure.ID == Constants.CorporateSectortID)
                {
                    if (orgStructure.ID == Constants.CorporateDepartmentID)
                        continue;

                    orgStructure.IsCorporate = true;
                    orgStructure.KPIs = KPIs.Where(CorporateOrgStructureFilterPredicate).ToList();
                }
                else
                    orgStructure.KPIs = KPIs.Where(w => w.OrgStructureID == orgStructure.ID || (w.DivisionalObjectiveID.HasValue && w.DivisionalObjective.OrgStructureId == orgStructure.ID)).ToList();
                final = new OrgStructureWeightDTO();
                final = Mapper.Map<OrgStructureWeightDTO>(orgStructure);
                final.LevelID = orgStructure.ParentID.HasValue ? 3 : orgStructure.IsCorporate ? 1 : 2;

                finalList.Add(final);
            }

            return finalList;
        }

        #endregion

        #region Update

        public bool UpdateWeight(UpdateWeightDTO UpdateWeight)
        {
            switch (UpdateWeight.Level)
            {
                case (int)model.LevelTypeEnum.KPI:
                    List<KPI> kpis = AutoMapper.Mapper.Map<List<KPI>>(UpdateWeight.Weights);
                    KPIBLL.Update(kpis);
                    break;
                case (int)model.LevelTypeEnum.StrategicObjective:
                    List<StrategicObjective> strategicObjectives = AutoMapper.Mapper.Map<List<StrategicObjective>>(UpdateWeight.Weights);
                    StrategicObjectiveBLL.Update(strategicObjectives);
                    break;
                case (int)model.LevelTypeEnum.Theme:
                    List<Theme> themes = AutoMapper.Mapper.Map<List<Theme>>(UpdateWeight.Weights);
                    ThemeBLL.Update(themes);
                    break;
                case (int)model.LevelTypeEnum.DivisionalKPI:
                    List<KPI> divisionalKPIs = AutoMapper.Mapper.Map<List<KPI>>(UpdateWeight.Weights);
                    KPIBLL.Update(divisionalKPIs);
                    break;
                case (int)model.LevelTypeEnum.DivisionalObjective:
                    List<DivisionalObjective> divisionalObjectives = AutoMapper.Mapper.Map<List<DivisionalObjective>>(UpdateWeight.Weights);
                    DivisionalObjectiveBLL.Update(divisionalObjectives);
                    break;
                case (int)model.LevelTypeEnum.BusinessUnitKPIs:
                    List<KPI> BusinessUnitKPIs = AutoMapper.Mapper.Map<List<KPI>>(UpdateWeight.Weights);
                    KPIBLL.UpdateBusinessWeight(BusinessUnitKPIs);
                    break;

                default:
                    return false;
            }

            return true;
        }

        #endregion


    }
}
