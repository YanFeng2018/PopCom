using SE.PopCom.Host.Business;
using System;
using SE.PopCom.Entity;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Test
{
    public static class DiagnoseResultHelper
    {
        //Client: 353956 Device: 358263 DiagnoseModelType: Load_I_Ir
        private static string DATASPIRIT_DIAGNOSE_ID_ALARM_SETTING_ON = "5c2f13fcb82b16000161abf7";
        private static string DATASPIRIT_DIAGNOSE_ID_ALARM_SETTING_OFF = "5c3437a2669c120001092a5f";
        public static bool AlarmSetting { get; set; }
        public static DiagnoseResult[] CreateLowLevelAlarmDiagnoseResult(bool alarmSetting)
        {
            DiagnoseResult[] diagnoseResults = new DiagnoseResult[1];
            diagnoseResults[0] = new DiagnoseResult()
            {
                Code = "701",
                Value = 60,
                Time = DateTime.Now,
                State = DiagnoseDataState.Normal,
                DiagnosticId = alarmSetting? DATASPIRIT_DIAGNOSE_ID_ALARM_SETTING_ON: DATASPIRIT_DIAGNOSE_ID_ALARM_SETTING_OFF

            };
            //var diagnoseDataJson = JsonConvert.SerializeObject(r);
            return diagnoseResults;
        }

        public static DiagnoseResult[] CreateHighLevelAlarmDiagnoseResult(bool alarmSetting)
        {
            DiagnoseResult[] diagnoseResults = new DiagnoseResult[1];
            diagnoseResults[0] = new DiagnoseResult()
            {
                Code = "702",
                Value = 90,
                Time = DateTime.Now,
                State = DiagnoseDataState.Normal,
                DiagnosticId = alarmSetting ? DATASPIRIT_DIAGNOSE_ID_ALARM_SETTING_ON : DATASPIRIT_DIAGNOSE_ID_ALARM_SETTING_OFF

            };
            //var diagnoseDataJson = JsonConvert.SerializeObject(r);
            return diagnoseResults;
        }

        public static DiagnoseResult[] CreateNormalDiagnoseResult(bool alarmSetting)
        {
            DiagnoseResult[] diagnoseResults = new DiagnoseResult[1];
            diagnoseResults[0] = new DiagnoseResult()
            {
                Code = "",
                Value = 40,
                Time = DateTime.Now,
                State = DiagnoseDataState.Normal,
                DiagnosticId = alarmSetting ? DATASPIRIT_DIAGNOSE_ID_ALARM_SETTING_ON : DATASPIRIT_DIAGNOSE_ID_ALARM_SETTING_ON

            };
            //var diagnoseDataJson = JsonConvert.SerializeObject(r);
            return diagnoseResults;
        }

        public static DiagnoseResult[] CreateManyDiagnoseResults(int days, bool alarmSetting=true)
        {
            DiagnoseResult[] diagnoseResults = new DiagnoseResult[96*days];
            DateTime startDateTime = DateTime.Now.AddDays(-days);
            Random random = new Random();
            int interval = 15;
            for (int i = 0; i < diagnoseResults.Length; i++)
            {
                var rdata = random.Next(40, 90);
                interval = 15 + interval;
                diagnoseResults[i] = new DiagnoseResult()
                {
                    Code = rdata < 50 ? "" : rdata < 80 ? "701" : "702",
                    Value = rdata,
                    Time = startDateTime.AddMinutes(interval),
                    State = DiagnoseDataState.Normal,
                    DiagnosticId = alarmSetting ? DATASPIRIT_DIAGNOSE_ID_ALARM_SETTING_ON : DATASPIRIT_DIAGNOSE_ID_ALARM_SETTING_OFF

                };

            }
           
            //var diagnoseDataJson = JsonConvert.SerializeObject(r);
            return diagnoseResults;
        }

    }
}
