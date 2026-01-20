using SPlus.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPlus.UseCases 
{
    public static class OrgStrucureTree
    {
        #region OrgStructure Tree
        public static List<CAOrgStructureWeightDTO> BuildTree(this List<CAOrgStructureWeightDTO> source)
        {
            var groups = source.GroupBy(i => i.ParentID);

            var roots = groups.Where(g => g.Key.HasValue == false).FirstOrDefault().ToList();

            if (roots.Count > 0)
            {
                var dict = groups.Where(g => g.Key.HasValue).ToDictionary(g => g.Key.Value, g => g.ToList());
                for (int i = 0; i < roots.Count; i++)
                    AddChildren(roots[i], dict);
            }

            return roots;
        }
        private static void AddChildren(CAOrgStructureWeightDTO node, IDictionary<int, List<CAOrgStructureWeightDTO>> source)
        {
            if (source.ContainsKey(node.ID))
            {
                node.Childrens = source[node.ID];
                for (int i = 0; i < node.Childrens.Count; i++)
                    AddChildren(node.Childrens[i], source);
            }
            else
            {
                node.Childrens = new List<CAOrgStructureWeightDTO>();
            }
        }
        #endregion
    }
}