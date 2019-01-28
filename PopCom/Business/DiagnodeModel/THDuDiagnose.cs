using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SE.PopCom.Host.Business
{
    /// <summary>
    /// 电压谐波
    /// </summary>
    public class THDuDiagnose : DiagnoseModelBase
    {
        public override string SeriousAlarmCode
        {
            get { return "712"; }
        }

        public override string LightAlarmCode
        {
            get { return "711"; }
        }

        public override string ModelName
        {
            get
            {
                return "电压谐波THDu";
            }
        }
    }
}