using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE.PopCom.Entity
{
    public class Building
    {
        public Building()
        {
        }

        public Building(long hierarchyId, decimal? buildingArea, DateTime? finishingdate)
        {
            this.HierarchyId = hierarchyId;
            this.BuildingArea = buildingArea;
            this.FinishingDate = finishingdate;
        }

        public long HierarchyId { get; set; } 

        public decimal? BuildingArea { get; set; }

        public DateTime? FinishingDate { get; set; }

        public int SubType { get; set; }

        /// <summary>
        /// 维保日期
        /// </summary>
        public DateTime? MaintenanceDate { get; set; }

        /// <summary>
        /// 维保公司
        /// </summary>
        public string MaintenanceCompany { get; set; }

        /// <summary>
        /// 服务等级
        /// </summary>
        public int? ServiceGrade { get; set; }
    }
}
