using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SE.PopCom.Host.Business
{
    /// <summary>
    /// 高电压
    /// </summary>
    public class UhighDiagnose : DiagnoseModelBase
    {
        public override string SeriousAlarmCode
        {
            get { return "719"; }
        }

        public override string LightAlarmCode
        {
            get { return "718"; }
        }

        public override string ModelName
        {
            get
            {
                return "高电压";
            }
        }
    }
}