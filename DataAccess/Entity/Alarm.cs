using System;
using System.Collections.Generic;

namespace SE.PopCom.Entity
{
    public class Alarm
    {
        public long Id { get; set; }

        public long? DeviceParameterUniqueId { get; set; }

        public string Code { get; set; }

        public string ActualValue { get; set; }

        public string ResetValue { get; set; }

        public string ThresholdValue { get; set; }

        public DateTime AlarmTime { get; set; }

        public DateTime? SecureTime { get; set; }

        public DateTime? ResetTime { get; set; }

        public long HierarchyId { get; set; }

        public int Level { get; set; }

        public AlarmSecureType SecureType { get; set; }

        public string Description { get; set; }

        public string Comment { get; set; }

        public string Uom { get; set; }

        public long? SecureUserId { get; set; }

        public AlarmType AlarmType { get; set; }

        /// <summary>
        /// 维保状态(0,未维保,1,正常维保)
        /// </summary>
        public MaintenanceState MaintenanceState { get; set; }

    }
}
