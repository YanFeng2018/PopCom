using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE.PopCom.Entity
{
    public enum AlarmSecureType
    {
        /// <summary>
        /// 未解除
        /// </summary>
        NoSecure = 0,

        /// <summary>
        /// 手动解除
        /// </summary>
        ManualSecure = 1,

        /// <summary>
        /// 自动解除
        /// </summary>
        AutoSecure = 2,

        /// <summary>
        /// 设备移除的告警，强制解除
        /// </summary>
        ForceSecure = 4
    }

    /// <summary>
    /// 报警类型
    /// </summary>
    public enum AlarmType
    {
        [Description("以前默认")]
        Default=0,

        [Description("智能诊断")]
        PopDiagnose = 1
    }
}
