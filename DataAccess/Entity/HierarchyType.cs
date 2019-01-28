using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE.PopCom.Entity
{
    /// <summary>
    /// 层级结构类型，从Bass中迁移过来
    /// </summary>
    public enum HierarchyType
    {
        Customer = -1,

        Organization = 0,

        Site = 1,

        Building = 2,

        Room = 3,

        Panel = 4,

        Device = 5
    }
}
