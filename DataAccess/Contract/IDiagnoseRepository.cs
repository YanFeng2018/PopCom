using SE.PopCom.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE.PopCom.Contract
{
    public interface IDiagnoseRepository 
    {
        void SetLastAbnormalTime(long id, DateTime? time, long? alarmId, string code, string healthCode, float? healthValue, DateTime lastScanTime);

        DiagnoseEntity GetByDataSpiritId(string spiritId);
      
    }
}
