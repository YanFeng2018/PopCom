using SE.PopCom.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace SE.PopCom.Host.Business
{
    /// <summary>
    /// 频率
    /// </summary>
    public class FDiagnose : DiagnoseModelBase
    {
        public override string SeriousAlarmCode
        {
            get { return "722"; }
        }

        public override string LightAlarmCode
        {
            get { return string.Empty; }
        }

        public override string ModelName
        {
            get
            {
                return "频率";
            }
        }

        public override float? GetThresholdValue(float? highLimit, float? lowLimit, string alarmCode, double? value)
        {
            if (string.IsNullOrEmpty(alarmCode))
            {
                return null;
            }

            if (value > highLimit)
            {
                return highLimit;
            }
            else
            {
                return lowLimit;
            }
        }

        public override bool JudgeValueWithDeadArea(float? lowLimit, float? highLimit, float? deadArea, double? value, double? lastAbnormalValue, string lastAlarmCode, string currentAlarmCode)
        {
            var lastValueAlarmLevel = this.SeriousAlarmCode == lastAlarmCode ? DiagnoseAlarmLevel.HighLevel : this.LightAlarmCode == lastAlarmCode ? DiagnoseAlarmLevel.LowLevel : DiagnoseAlarmLevel.None;
            var currentValueAlarmLevel = this.SeriousAlarmCode == currentAlarmCode ? DiagnoseAlarmLevel.HighLevel : this.LightAlarmCode == currentAlarmCode ? DiagnoseAlarmLevel.LowLevel : DiagnoseAlarmLevel.None;

            if (currentValueAlarmLevel > lastValueAlarmLevel)
            {
                return true;
            }

            if (currentValueAlarmLevel == lastValueAlarmLevel)
            {
                return false;
            }

            if (lastAbnormalValue < lowLimit)
            {
                return lowLimit + deadArea <= value;
            }

            if (lastAbnormalValue > highLimit)
            {
                return highLimit - deadArea >= value;
            }

            return true;
        }
    }
}