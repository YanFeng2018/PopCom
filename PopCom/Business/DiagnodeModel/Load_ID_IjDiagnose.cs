using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SE.PopCom.Host.Business
{
    /// <summary>
    /// 负载率Load_ID_Ij
    /// </summary>
    public class Load_ID_IjDiagnose : DiagnoseModelBase
    {
        public override string SeriousAlarmCode
        {
            get { return "702"; }
        }

        public override string LightAlarmCode
        {
            get { return "701"; }
        }

        public override string ModelName
        {
            get
            {
                return "负载率（ID，Ij）";
            }
        }

    }
}