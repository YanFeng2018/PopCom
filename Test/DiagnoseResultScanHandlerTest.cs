using Newtonsoft.Json;
using SE.PopCom.DataAccess;
using SE.PopCom.Host.Business;
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace Test
{
    public class DiagnoseResultScanHandlerTest
    {
        private MockDiagnoseResultHandler diagnoseScanResultHanlder;

        public DiagnoseResultScanHandlerTest()
        {
            diagnoseScanResultHanlder = new MockDiagnoseResultHandler();
            AppConfig appConfig = JsonConvert.DeserializeObject<AppConfig>(
                File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetAssembly(typeof(DiagnoseResultScanHandlerTest)).Location),"AppSettings.json")));
            DataAccessBase da = new DataAccessBase();
            da.InitConnString(appConfig.DBConnectionString);
        }

        [Fact]
        public void ResetAlarmWithLowLevelAlarmAlarmSettingOn()
        {
            var diagnoseResults = DiagnoseResultHelper.CreateLowLevelAlarmDiagnoseResult(true);
            diagnoseScanResultHanlder.Process(diagnoseResults);
        }
        [Fact]
        public void CreateHighLevelAlarmAlarmSettingOn()
        {
            var diagnoseResults = DiagnoseResultHelper.CreateHighLevelAlarmDiagnoseResult(true);
            diagnoseScanResultHanlder.Process(diagnoseResults);
        }
        [Fact]
        public void ResetAlarmWithNormalDataAlarmSettingOff()
        {
            var diagnoseResults = DiagnoseResultHelper.CreateNormalDiagnoseResult(false);
            diagnoseScanResultHanlder.Process(diagnoseResults);
        }
       
        [Fact]
        public void BackTracking()
        {
            var diagnoseResults = DiagnoseResultHelper.CreateManyDiagnoseResults(25);
            diagnoseScanResultHanlder.Process(diagnoseResults);
        }
        [Fact]
        public void CreateNewAlarm()
        {

        }
    }
}
