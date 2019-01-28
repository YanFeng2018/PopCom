using SE.PopCom.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SE.PopCom.Host.Business
{
    public class DiagnoseModelFactory
    {
        public static DiagnoseModelBase GetDiagnoseModelByType(DiagnoseModelType modelType)
        {
            DiagnoseModelBase diagnoseModel = null;
            switch (modelType)
            {
                case DiagnoseModelType.load_I_Ij:
                    diagnoseModel = new Load_I_IjDiagnose();
                    break;
                case DiagnoseModelType.load_I_Ir:
                    diagnoseModel = new Load_I_IrDiagnose();
                    break;
                case DiagnoseModelType.load_ID_Ij:
                    diagnoseModel = new Load_ID_IjDiagnose();
                    break;
                case DiagnoseModelType.load_ID_Ir:
                    diagnoseModel = new Load_ID_IrDiagnose();
                    break;
                case DiagnoseModelType.Iunbal:
                    diagnoseModel = new IunbalDiagnose();
                    break;
                case DiagnoseModelType.Aging:
                    diagnoseModel = new AgingDiagnose();
                    break;
                case DiagnoseModelType.PF:
                    diagnoseModel = new PFDiagnose();
                    break;
                case DiagnoseModelType.SDE_Trip:
                    diagnoseModel = new SDE_TripDiagnose();
                    break;
                case DiagnoseModelType.Sys_THDi:
                    diagnoseModel = new Sys_THDiDiagnose();
                    break;
                case DiagnoseModelType.Temp_delta:
                    diagnoseModel = new Temp_deltaDiagnose();
                    break;
                case DiagnoseModelType.THDi:
                    diagnoseModel = new THDiDiagnose();
                    break;
                case DiagnoseModelType.THDu:
                    diagnoseModel = new THDuDiagnose();
                    break;
                case DiagnoseModelType.Uhigh:
                    diagnoseModel = new UhighDiagnose();
                    break;
                case DiagnoseModelType.Ulow:
                    diagnoseModel = new UlowDiagnose();
                    break;
                case DiagnoseModelType.Uunbal:
                    diagnoseModel = new UunbalDiagnose();
                    break;
                case DiagnoseModelType.COSy:
                    diagnoseModel = new COSyDiagnose();
                    break;
                case DiagnoseModelType.F:
                    diagnoseModel = new FDiagnose();
                    break;
            }

            return diagnoseModel;
        }
    }
}
