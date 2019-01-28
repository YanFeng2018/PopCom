using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE.PopCom.Entity
{
    public enum DiagnoseAlarmLevel
    {
        [Description("不报警")]
        None=0,

        [Description("低级报警")]
        LowLevel=1,

        [Description("中级报警")]
        MiddleLevel=2,

        [Description("高级报警")]
        HighLevel = 3,
    }

    public enum DiagnoseModelType
    {
        [Description("负载率l_lr")]
        load_I_Ir = 1,

        [Description("负载率l_lj")]
        load_I_Ij = 2,

        [Description("负载率lm_lr")]
        load_ID_Ir = 3,

        [Description("负载率lm_lj")]
        load_ID_Ij = 4,

        [Description("电流不平衡")]
        Iunbal = 5,

        [Description("电流谐波")]
        THDi = 6,

        [Description("老化率")]
        Aging = 7,

        [Description("温升")]
        Temp_delta = 8,

        [Description("故障跳闸")]
        SDE_Trip = 9,

        [Description("电压谐波")]
        THDu = 10,

        [Description("功率因数")]
        PF = 11,

        [Description("Cosy")]
        COSy = 12,

        [Description("电压不平衡")]
        Uunbal = 13,

        [Description("高电压")]
        Uhigh = 14,

        [Description("低电压")]
        Ulow = 15,

        [Description("频率")]
        F = 16,

        [Description("系统电流谐波")]
        Sys_THDi = 17
    }

    public enum DiagnoseStatus
    {
        [Description("未设置")]
        None=0,

        [Description("正常")]
        Normal=1,

        [Description("暂停")]
        Pause=2
    }

    public enum DiagnoseJudge
    {
        [Description("小于")]
        Less=1,

        [Description("小于等于")]
        LessOrEqual=2,

        [Description("等于")]
        Equal=3,

        [Description("大于等于")]
        GreaterOrEqual=4,

        [Description("大于")]
        Greater=5
    }

    public enum DiagnoseDataState
    {
        [Description("正常")]
        Normal = 1,

        [Description("忽略")]
        Ignored = 2
    }

    public enum DiagnoseDataType
    {
        [Description("正常")]
        Normal = 0,

        [Description("一般异常")]
        Light = 1,

        [Description("严重异常")]
        Serious = 2,

        [Description("极值")]
        Extremum = 4,
    }

    public enum SystemStructure
    {
        [Description("进线开关")]
        inc = 1,

        [Description("关键回路")]
        KeyF = 2,

        [Description("普通回路")]
        NorF = 3,

        [Description("二次盘柜")]
        SndP = 4
    }

    public enum HealthLevel
    {
        [Description("健康")]
        Low=0,

        [Description("预警")]
        Middle=1,

        [Description("危险")]
        High=2
    }

    public enum EmergencyLevel
    {
        [Description("低")]
        Low = 0,

        [Description("中等")]
        Middle = 1,

        [Description("紧急")]
        High = 2
    }

    public enum RiskLevel
    {
        [Description("安全")]
        Safe=0,

        [Description("低")]
        Green=1,

        [Description("中")]
        Mid=2,

        [Description("高")]
        H=3
    }

    /// <summary>
    /// 该枚举值包含两部分，高位表示系统结构（1，2，3，4），低位表示RiskLevel（0，1，2，3）
    /// </summary>
    public enum StructureRiskLevel
    {
        [Description("进线开关(高)")]
        Inc_H = 13,

        [Description("进线开关(中)")]
        Inc_Mid = 12,

        [Description("进线开关(低)")]
        Inc_G = 11,

        [Description("关键回路(高)")]
        KeyF_H = 23,

        [Description("关键回路(中)")]
        KeyF_Mid = 22,

        [Description("普通回路(高)")]
        NorF_H = 33,

        [Description("普通回路(中)")]
        NorF_M = 32,

        [Description("二次盘柜(高)")]
        SndP_H = 43,

        [Description("二次盘柜(中)")]
        SndP_M = 42,

        [Description("其它(低)")]
        OthG = 1,

        [Description("0,0")]
        Safe = 0
    }

    /// <summary>
    /// 变压器系统数据点的枚举值
    /// </summary>
    public enum TransformerTagType
    {
        [Description("电流")]
        Electric=1,

        [Description("负载率")]
        Load=2,

        [Description("系统电压")]
        SystemVoltage=3,

        [Description("THDu")]
        THDu=4,

        [Description("功率因数")]
        PF=5,

        [Description("最高温度")]
        Temperature=6
    }

    public enum SolutionState
    {
        [Description("草稿")]
        Draft = -1,

        [Description("已取消")]
        Cancelled = 0,

        [Description("未推送")]
        NotPushed = 1,

        [Description("待分配")]
        ToBeAssign = 2,

        [Description("进行中")]
        Ongoing = 3,

        [Description("已完成")]
        Executed = 4
    }

    public enum OperationType
    {
        [Description("创建方案")]
        Create = 1,

        [Description("推送方案")]
        Push = 2,

        [Description("创建工单")]
        CreateTicket = 3,

        [Description("开始执行")]
        StartExecute = 4,

        [Description("提交工单")]
        Auth = 5,

        [Description("已完成")]
        Executed = 6
    }

    /// <summary>
    /// 极值类型
    /// </summary>
    public enum DiagnoseExtremumType
    {
        /// <summary>
        /// 根据诊断自动识别
        /// </summary>
        AutoByDiagnose = 0,
        
        /// <summary>
        /// 指定类型为查找极大值
        /// </summary>
        ExtremumMax = 1,
        
        /// <summary>
        /// 指定类型为查找极小值
        /// </summary>
        ExtremumMin = 2,
    }

    /// <summary>
    /// 诊断的恶劣趋势
    /// </summary>
    public enum DiagnoseBadTrend
    { 
        /// <summary>
        /// 诊断类中值越大问题越严重
        /// </summary>
        ValueMaxToBad = 1,
        /// <summary>
        /// 诊断类型中值越小问题越严重
        /// </summary>
        ValueMinToBad = 2,
        /// <summary>
        /// 诊断类型中值越大或越小都是问题越严重
        /// </summary>
        ValueBothEndToBad = 4,

        /// <summary>
        /// 诊断类型中数据值为1 或 True表示问题越严重
        /// </summary>
        Value1OrTrueToBad = 8,

        /// <summary>
        /// 诊断类型中数据值为0 或 False表示问题越严重
        /// </summary>
        Value0OrFalseToBad = 16,
    }
}
