using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE.PopCom.Entity
{
    public class Hierarchy
    {
        public Hierarchy()
        {
        }

        public Hierarchy(string name, string code, long spId, long? parentId, long customerId, HierarchyType hierarchyType)
            : this(name, code, spId, hierarchyType)
        {
            this.ParentId = parentId;
            this.CustomerId = customerId;
        }

        public Hierarchy(string name, string code, long spId, long? industryId, long? parentId, long customerId, HierarchyType hierarchyType)
            : this(name, code, spId, parentId, customerId, hierarchyType)
        {
            this.IndustryId = industryId;
        }

        public Hierarchy(string name, string code, long spId, HierarchyType hierarchyType)
        {
            this.Type = hierarchyType;
            this.Code = code;
            this.Name = name;
            this.TimezoneId = 1;
            this.PathLevel = 1;
            this.Status = 1;
            this.CalcStatus = true;
            this.SpId = spId;
            this.UpdateTime = DateTime.Now;
        }

        public long Id { get; set; }

        public HierarchyType Type { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public long TimezoneId { get; set; }

        public string Comment { get; set; }

        public long? ParentId { get; set; }

        public long CustomerId { get; set; }

        public string Path { get; set; }

        public int PathLevel { get; set; }

        public int Status { get; set; }

        public long? IndustryId { get; set; }

        public long? ZoneId { get; set; }

        public bool CalcStatus { get; set; }

        public long SpId { get; set; }

        public string PathFromBuilding { get; set; }

        public string UpdateUser { get; set; }

        public DateTime? UpdateTime { get; set; }

        public long? Version { get; set; }

        public DateTime? MaintenanceDate { get; set; }

        public int? ServiceGrade { get; set; }

        /// <summary>
        /// 为层级树中的节点增加二维码字段
        /// </summary>
        public string QRCode { get; set; }

        public long AssetId { get; set; }

        public long BuildingPictureId { get; set; }

        public int? SubType { get; set; }

        public bool IsActive()
        {
            return this.Status == 1;
        }

        public void SetActive()
        {
            this.Status = 1;
        }
    }
}
