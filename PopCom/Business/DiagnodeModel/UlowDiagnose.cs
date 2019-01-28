using SE.PopCom.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SE.PopCom.Host.Business
{
    /// <summary>
    /// 低电压
    /// </summary>
    public class UlowDiagnose : DiagnoseModelBase
    {
        public override string SeriousAlarmCode
        {
            get { return "721"; }
        }

        public override string LightAlarmCode
        {
            get { return "720"; }
        }

        public override string ModelName
        {
            get
            {
                return "低电压";
            }
        }

        public override float? GetThresholdValue(float? highLimit, float? lowLimit, string alarmCode, double? value)
        {
            if (string.IsNullOrEmpty(alarmCode))
            {
                return null;
            }

            if (alarmCode == this.SeriousAlarmCode)
            {
                return lowLimit;
            }
            else
            {
                return highLimit;
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

            if (currentValueAlarmLevel == DiagnoseAlarmLevel.LowLevel)
            {
                return (lowLimit + deadArea) <= value;
            }
            else
            {
                return (highLimit + deadArea) <= value;
            }
        }
    }
}