using SE.PopCom.Contract;
using SE.PopCom.DataAccess;
using SE.PopCom.Entity;
using Dapper;

namespace SE.PopCom.DataAccess
{
    public class BuildingRepository : DataAccessBase, IBuildingRepository
    {
        public Building GetById(long id)
        {
            using (var db = this.Database)
            {
                var result = db.QuerySingleOrDefault<Building>($"SELECT * FROM Building WHERE HierarchyId={id}");

                return result;
            }
        }

      
    }
}
