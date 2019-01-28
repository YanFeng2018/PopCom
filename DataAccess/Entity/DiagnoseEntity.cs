using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE.PopCom.Entity
{
    public class DiagnoseEntity
    {
        private DateTime? startTime = new DateTime(DateTime.Now.Year, 1, 1);

        /// <summary>
        /// 诊断Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 客户Id
        /// </summary>
        public long CustomerId { get; set; }

        /// <summary>
        /// 设备Id
        /// </summary>
        public long DeviceId { get; set; }

        /// <summary>
        /// 诊断模型
        /// </summary>
        public DiagnoseModelType DiagnoseModel { get; set; }

        /// <summary>
        /// 诊断名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 低门限
        /// </summary>
        public float? LowLimit { get; set; }

        /// <summary>
        /// 高门限
        /// </summary>
        public float? HighLimit { get; set; }

        /// <summary>
        /// 死区
        /// </summary>
        public float? DeadArea { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime
        {
            get
            {
                if (this.startTime.HasValue)
                {
                    return this.startTime;
                }

                return new DateTime(DateTime.Now.Year, 1, 1);
            }

            set
            {
                this.startTime = value;
            }
        }

        /// <summary>
        /// 低门限报警级别
        /// </summary>
        public DiagnoseAlarmLevel? LightAlarmLevel { get; set; }

        /// <summary>
        /// 高门限报警级别
        /// </summary>
        public DiagnoseAlarmLevel? SeriousAlarmLevel { get; set; }

        /// <summary>
        /// 最后忽略时间
        /// </summary>
        public DateTime? LastIgnoreTime { get; set; }

        /// <summary>
        /// 诊断状态
        /// </summary>
        public DiagnoseStatus DiagnoseStatus { get; set; }

        public long? ParameterAId { get; set; }

        public long? ParameterBId { get; set; }

        public long? ParameterCId { get; set; }

        public long? ParameterDId { get; set; }

        public float? Factor { get; set; }

        public string DataSpiritDiagnoseId { get; set; }

        public long TagId { get; set; }

        public DateTime UpdateTime { get; set; }

        public long? AlarmId { get; set; }

        public string AlarmCode { get; set; }

        public DateTime? LastAbnormalTime { get; set; }

        public string HealthCode { get; set; }

        public float? HealthValue { get; set; }

        public long? RelativeDiagnoseId { get; set; }

        public float? RelativeValue { get; set; }

        public bool EnableRelativeDiagnose { get; set; }

        public DateTime? LastScanTime { get; set; }

    }
}
