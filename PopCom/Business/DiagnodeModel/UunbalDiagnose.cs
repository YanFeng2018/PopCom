using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SE.PopCom.Host.Business
{
    /// <summary>
    /// 三相电压不平衡
    /// </summary>
    public class UunbalDiagnose : DiagnoseModelBase
    {
        public override string SeriousAlarmCode
        {
            get { return "717"; }
        }

        public override string LightAlarmCode
        {
            get { return "716"; }
        }

        public override string ModelName
        {
            get
            {
                return "三相电压不平衡";
            }
        }
    }
}