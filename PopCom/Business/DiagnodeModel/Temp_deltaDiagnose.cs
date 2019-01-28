namespace SE.PopCom.Host.Business
{
    /// <summary>
    /// 温升
    /// </summary>
    public class Temp_deltaDiagnose : DiagnoseModelBase
    {
        public override string SeriousAlarmCode
        {
            get { return "710"; }
        }

        public override string LightAlarmCode
        {
            get { return "709"; }
        }

        public override string ModelName
        {
            get
            {
                return "温升";
            }
        }
    }
}