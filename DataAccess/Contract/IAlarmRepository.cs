using System;
using System.Collections;
using System.Collections.Generic;
using SE.PopCom.Entity;

namespace SE.PopCom.Contract
{
    public interface IAlarmRepository
    {
        Alarm Add(Alarm entity);
        Alarm ResetAlarm(long uniqueId, string acutalValue, DateTime resetTime, string comment);
        Alarm GetLastDiagnoseAlarm(long uniqueId);
        MaintenanceState GetDeviceMaintenanceState(long deviceId);
    }
}
