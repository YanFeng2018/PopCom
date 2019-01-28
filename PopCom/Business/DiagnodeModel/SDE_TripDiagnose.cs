using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SE.PopCom.Host.Business
{
    /// <summary>
    /// 故障跳闸
    /// </summary>
    public class SDE_TripDiagnose : DiagnoseModelBase
    {
        public override string SeriousAlarmCode
        {
            get { return "303"; }
        }

        public override string LightAlarmCode
        {
            get { return string.Empty; }
        }

        public override string ModelName
        {
            get
            {
                return "故障跳闸";
            }
        }
    }
}