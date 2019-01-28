using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SE.PopCom.Host.Business
{
    /// <summary>
    /// 电流不平衡
    /// </summary>
    public class IunbalDiagnose : DiagnoseModelBase
    {
        public override string SeriousAlarmCode
        {
            get { return "704"; }
        }

        public override string LightAlarmCode
        {
            get { return "703"; }
        }

        public override string ModelName
        {
            get
            {
                return "三相电流不平衡";
            }
        }
    }
}