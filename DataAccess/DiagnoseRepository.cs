using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SE.PopCom.Contract;
using SE.PopCom.Entity;
using Dapper;

namespace SE.PopCom.DataAccess
{
    public class DiagnoseRepository : DataAccessBase, IDiagnoseRepository
    {
        public void SetLastAbnormalTime(long id, DateTime? time, long? alarmId, string code, string healthCode, float? healthValue, DateTime lastScanTime)
        {
            using (var db = this.Database)
            {
                //var sql = $"update PopDiagnose set LastAbnormalTime='{time}',AlarmId={alarmId},AlarmCode='{code}',HealthCode='{healthCode}',HealthValue={healthValue}, LastScanTime='{lastScanTime}' where Id ={id} ";
                var sql = $"update PopDiagnose set LastAbnormalTime=@LastAbnormalTime,AlarmId=@AlarmId,AlarmCode=@AlarmCode,HealthCode=@HealthCode,HealthValue=@HealthValue where Id =@Id ";
                db.Execute(sql, new { LastAbnormalTime=time, AlarmId=alarmId, AlarmCode=code, HealthCode=healthCode, HealthValue = healthValue, lastScanTime, Id=id });
            }
          
        }

        public DiagnoseEntity GetByDataSpiritId(string spiritId)
        {
            using (var db = this.Database)
            {
                var sql = $"select * from PopDiagnose where DataSpiritDiagnoseId='{spiritId}'";
                return db.QueryFirstOrDefault<DiagnoseEntity>(sql);
            }
        }

       
    }
}
