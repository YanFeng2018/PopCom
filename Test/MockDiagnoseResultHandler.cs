using SE.PopCom.Host.Business;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    public class MockDiagnoseResultHandler : DiagnoseScanResultHanlder
    {
        public void Process(DiagnoseResult[] results)
        {
            base.ScanDiagnoseResult(results);
        }
    }
}
