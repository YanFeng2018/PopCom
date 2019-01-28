using SE.PopCom.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SE.PopCom.Host.Business
{
    public class DiagnoseResult
    {
        private DateTime? localTime = null;
        public string Id { get; set; }

        public string DiagnosticId { get; set; }

        public string Code { get; set; }

        public DateTime Time { get; set; }

        public DateTime LocalTime
        {
            get
            {
                if (this.localTime.HasValue)
                {
                    return this.localTime.Value;
                }

                this.localTime = this.Time.ToLocalTime();
                return this.localTime.Value;
            }

            set
            {
                this.localTime = value;
            }
        }

        public DiagnoseDataState State { get; set; }

        public double? Value { get; set; }

        public DiagnoseAlarmLevel? GetAlarmLevelByCode(DiagnoseModelBase model, DiagnoseEntity entity)
        {
            ////当为严重报警时
            if (Code == model.SeriousAlarmCode)
            {
                return entity.SeriousAlarmLevel;
            }
            else if (Code == model.LightAlarmCode)
            {
                return entity.LightAlarmLevel;

            }
            else
            {
                return DiagnoseAlarmLevel.None;
            }
        }
    }
}
