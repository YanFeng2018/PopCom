using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SE.PopCom.Host.Business
{
    /// <summary>
    /// 系统电流谐波
    /// </summary>
    public class Sys_THDiDiagnose : DiagnoseModelBase
    {
        public override string SeriousAlarmCode
        {
            get { return "724"; }
        }

        public override string LightAlarmCode
        {
            get { return "723"; }
        }

        public override string ModelName
        {
            get
            {
                return "系统电流谐波THDI";
            }
        }
    }
}