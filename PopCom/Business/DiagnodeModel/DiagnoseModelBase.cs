using SE.PopCom.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SE.PopCom.Host
{
    public abstract class DiagnoseModelBase
    {

        public abstract string SeriousAlarmCode { get; }

        public abstract string LightAlarmCode { get; }

        public abstract string ModelName { get; }
      
        /// <summary>
        /// 根据报警代码,获取阀值
        /// </summary>
        /// <param name="highLimit"></param>
        /// <param name="lowLimit"></param>
        /// <param name="alarmCode"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual float? GetThresholdValue(float? highLimit, float? lowLimit, string alarmCode, double? value)
        {
            if (string.IsNullOrEmpty(alarmCode))
            {
                return null;
            }

            if (alarmCode == this.SeriousAlarmCode)
            {
                return highLimit;
            }
            else
            {
                return lowLimit;
            }
        }

        public virtual bool JudgeValueWithDeadArea(float? lowLimit, float? highLimit, float? deadArea, double? value, double? lastAbnormalValue, string lastAlarmCode, string currentAlarmCode)
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

            if (currentValueAlarmLevel == DiagnoseAlarmLevel.None)
            {
                return (lowLimit - deadArea) > value;
            }

            if (currentValueAlarmLevel == DiagnoseAlarmLevel.LowLevel)
            {
                return (highLimit - deadArea) > value;
            }

            return true;
        }

        public List<string> GetUomsFromModel(DiagnoseModelType model)
        {
            List<string> uoms = new List<string>();
            switch (model)
            {
                case DiagnoseModelType.load_I_Ir:
                case DiagnoseModelType.load_I_Ij:
                case DiagnoseModelType.load_ID_Ir:
                case DiagnoseModelType.load_ID_Ij:
                    uoms.Add("a");
                    break;
                case DiagnoseModelType.Iunbal:
                case DiagnoseModelType.THDi:
                case DiagnoseModelType.Aging:
                case DiagnoseModelType.THDu:
                case DiagnoseModelType.Uunbal:
                case DiagnoseModelType.Sys_THDi:
                    uoms.Add("%");
                    break;
                case DiagnoseModelType.Temp_delta:
                    uoms.Add("°C");
                    uoms.Add("摄氏度");
                    break;
                case DiagnoseModelType.Uhigh:
                case DiagnoseModelType.Ulow:
                    uoms.Add("v");
                    uoms.Add("kv");
                    break;
                case DiagnoseModelType.F:
                    uoms.Add("hz");
                    break;
                case DiagnoseModelType.PF:
                case DiagnoseModelType.COSy:
                case DiagnoseModelType.SDE_Trip:
                    break;
            }

            return uoms;
        }


    }
}
