using SE.PopCom.Entity;
using System.Collections.Generic;

namespace SE.PopCom.Contract
{
    public interface IHierarchyRepository 
    {
        int GetBuildingId(long hierarchyId);
       
    }
}
