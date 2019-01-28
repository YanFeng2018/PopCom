using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE.PopCom.Entity
{
    /// <summary>
    /// 从Bass项目中迁移过来，告警级别
    /// </summary>
    public enum AlarmLevel : int
    {
        Unknow = 0,
        Low = 1,
        Medium = 2,
        High = 3,
    }
}
