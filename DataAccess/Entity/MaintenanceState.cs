using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE.PopCom.Entity
{
    public enum MaintenanceState
    {
        [Description("未配置")]
        NotConfig=0,

        [Description("维保中")]
        Normal=1,

        [Description("即将过期")]
        SoonOverdue=2,

        [Description("已经过期")]
        Overdue=3
    }
}
