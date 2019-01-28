using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SE.PopCom.Host.Business
{
    /// <summary>
    /// 电流谐波
    /// </summary>
    public class THDiDiagnose : DiagnoseModelBase
    {
        public override string SeriousAlarmCode
        {
            get { return "706"; }
        }

        public override string LightAlarmCode
        {
            get { return "705"; }
        }

        public override string ModelName
        {
            get
            {
                return "电流谐波THDi";
            }
        }
    }
}