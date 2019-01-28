namespace SE.PopCom.Host.Business
{
    /// <summary>
    /// 老化率
    /// </summary>
    public class AgingDiagnose : DiagnoseModelBase
    {
        public override string SeriousAlarmCode
        {
            get { return "708"; }
        }

        public override string LightAlarmCode
        {
            get { return "707"; }
        }

        public override string ModelName
        {
            get
            {
                return "老化率";
            }
        }
    }
}