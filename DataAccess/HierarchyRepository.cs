using SE.PopCom.Contract;
using SE.PopCom.DataAccess;
using SE.PopCom.Entity;
using Dapper;

namespace SE.PopCom.DataAccess
{
    public class HierarchyRepository : DataAccessBase, IHierarchyRepository
    {
        public int GetBuildingId(long hierarchyId)
        {
            using (var db = this.Database)
            {
               var sql =
                $@"select top 1 HP.Id from Hierarchy HP
                  inner join Hierarchy HC on HP.CustomerId=HC.CustomerId
                  where HC.Id ={hierarchyId} and HC.[Path].IsDescendantOf(HP.[Path])=1 and HP.Type=2";
                return db.QuerySingleOrDefault<int>(sql);
            }
        }

       
    }
}
