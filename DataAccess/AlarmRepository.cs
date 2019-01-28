using SE.PopCom.Contract;
using SE.PopCom.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace SE.PopCom.DataAccess
{
    public class AlarmRepository : DataAccessBase, IAlarmRepository
    {
        private IHierarchyRepository hierarchyRepository;
        private IBuildingRepository buildingRepository;

        public AlarmRepository()
        {
            this.hierarchyRepository = new HierarchyRepository();
            this.buildingRepository = new BuildingRepository();

        }

        /// <summary>
        /// create a new diagnose alarm
        /// </summary>
        /// <param name="entity"></param>
        public Alarm Add(Alarm entity)
        {
            using (var db = this.Database)
            {
                string sql =
                    @"INSERT INTO [Alarm](DeviceParameterUniqueId,Code,ActualValue,ThresholdValue,AlarmTime, AlarmType, SecureTime,[HierarchyId],[Level],SecureType,Description, Comment, Uom, MaintenanceState)
                    VALUES(@DeviceParameterUniqueId, @Code, @ActualValue, @ThresholdValue, @AlarmTime, 1, @SecureTime, @HierarchyId, @Level, @SecureType,@Description, @Comment, @Uom, @MaintenanceState);
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                var newId= db.Query<int>(sql, entity).Single();
                return db.QuerySingleOrDefault<Alarm>("SELECT * FROM Alarm WHERE ID = @id", new { id = newId });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueId">diagnose ID</param>
        /// <param name="acutalValue">reset value</param>
        /// <param name="resetTime">reset time</param>
        /// <param name="comment">description</param>
        /// <returns></returns>
        public Alarm ResetAlarm(long uniqueId, string acutalValue, DateTime resetTime, string comment)
        {
            using (var db = this.Database)
            {
                Alarm alarm = null;
                var alarmList = db.Query<Alarm>("SELECT * FROM Alarm with(nolock) WHERE deviceparameteruniqueid = @deviceparameteruniqueid and securetime is null and resetvalue is null and alarmType=@alarmType",
                    new { deviceparameteruniqueid = uniqueId, alarmType=AlarmType.PopDiagnose }).ToList();
                if (alarmList != null && alarmList.Count >= 1)
                {
                    var alarmIds = String.Join(',', alarmList.Select(x => x.Id).ToArray());
                    db.Execute("UPDATE Alarm SET resetvalue = @resetvalue, securetime = @securetime, secureuserid = -1, securetype =@securetype, comment = @comment WHERE id in (@ids)"
                        ,new { resetvalue =acutalValue, securetime=resetTime, securetype= (int)AlarmSecureType.AutoSecure, comment = comment,ids = alarmIds });
                                    
                    alarm = alarmList.LastOrDefault();
                    alarm.ResetValue = acutalValue;
                    alarm.SecureTime = resetTime;
                    alarm.SecureUserId = -1;
                    alarm.SecureType = AlarmSecureType.AutoSecure;
                    alarm.Comment = comment;
                }

                return alarm;
            }
             
        }

        /// <summary>
        /// Get the last diagnose alarm by diagnose id
        /// </summary>
        /// <param name="uniqueId">diagnose id</param>
        /// <returns></returns>
        public Alarm GetLastDiagnoseAlarm(long uniqueId)
        {
            using (var db = this.Database)
            {
                var sql = "select top 1 * from Alarm where DeviceParameterUniqueId=@DeviceParameterUniqueId and AlarmType=@AlarmType and securetime is null  order by AlarmTime desc";
                return db.QueryFirstOrDefault<Alarm>(sql,new { DeviceParameterUniqueId=uniqueId, AlarmType = (int)AlarmType.PopDiagnose });
            }
               
        }
        public MaintenanceState GetDeviceMaintenanceState(long deviceId)
        {
            MaintenanceState result = MaintenanceState.NotConfig;
            var hierarchyId = this.hierarchyRepository.GetBuildingId(deviceId);
            
            var building = this.buildingRepository.GetById(hierarchyId);
            if (building != null)
            {
                var date = building.MaintenanceDate;
                if (date.HasValue)
                {
                    if (date.Value.Date >= DateTime.Now.Date)
                    {
                        result = MaintenanceState.Normal;
                    }
                }
            }
            

            return result;
        }
    }
}
