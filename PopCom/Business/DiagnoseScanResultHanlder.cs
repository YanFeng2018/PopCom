using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SE.PopCom.Contract;
using SE.PopCom.DataAccess;
using SE.PopCom.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SE.PopCom.Host.Business
{
    public class DiagnoseScanResultHanlder : MessageHandlerBase
    {
        private static readonly string AlarmTopic = "V1.0/DiagnosticResult/+";
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(DiagnoseScanResultHanlder));

        private IDiagnoseRepository diagnoseRepository;
        private IAlarmRepository alarmRepository;

        public override TopicInfo SubscribingTopic
        {
            get
            {
                return new TopicInfo
                {
                    QOS = 1,
                    Topic = AlarmTopic
                };
            }
        }
        public DiagnoseScanResultHanlder()
        {
            this.diagnoseRepository = new DiagnoseRepository();
            this.alarmRepository = new AlarmRepository();
        }

        protected override bool? Process(IncomingMsgEventArgs args)
        {
            logger.Debug("the function of set pop diagnose scan result function going to execute");
            var diagnoseDataList = JsonConvert.DeserializeObject<IEnumerable<DiagnoseResult>>(args.Body);
            if (diagnoseDataList == null || diagnoseDataList.Count() < 1)
            {
                logger.Debug("no data is returned from DataSpirit for this diagnose");
                return true;
            }
            return ScanDiagnoseResult(diagnoseDataList);
        }

        protected bool? ScanDiagnoseResult(IEnumerable<DiagnoseResult> diagnoseDataList)
        {
            var diagnoseDataSpiritId = diagnoseDataList.First().DiagnosticId;
            var diagnoseEntity = this.diagnoseRepository.GetByDataSpiritId(diagnoseDataSpiritId); ////DiagnosticId is equal to DiagnosticDataSpiritId
            if (diagnoseEntity == null)
            {
                logger.Debug($"No diagnose can be found with this diagnose dataspirit id {diagnoseDataSpiritId}");
                return true;
            }

            var highLimit = diagnoseEntity.HighLimit;
            var lowLimit = diagnoseEntity.LowLimit;
            double? abnormalValue = diagnoseEntity.HealthValue;
            DateTime? abnormalTime = diagnoseEntity.LastAbnormalTime;


            // Get the last alarm of the diagnose
            var lastAlarm = this.alarmRepository.GetLastDiagnoseAlarm(diagnoseEntity.Id);

            DiagnoseResult firstNormalDataPoint = null;
            DiagnoseResult firstAbormalDataPoint = null;

            long? currentAlarmId = diagnoseEntity.AlarmId;
            string currentAlarmCode = diagnoseEntity.AlarmCode;

            // backtracking scan
            // if (diagnoseEntity.LastScanTime == null)
            if (diagnoseEntity.LastScanTime != null)
            {
                logger.Debug("Backtracking scan started");
                firstAbormalDataPoint = diagnoseDataList.FirstOrDefault(d => !string.IsNullOrEmpty(d.Code));
                if (firstAbormalDataPoint != null)
                {
                    abnormalTime = CalculateNewAbnormTime(abnormalTime, firstAbormalDataPoint);
                }
            }
            else
            {
                // Alarm setting is OFF
                if (diagnoseEntity.SeriousAlarmLevel == 0 && diagnoseEntity.LightAlarmLevel == 0)
                {
                    logger.Debug("Alarm setting is OFF");
                    if (lastAlarm != null)
                    {
                        var newAlarms = CalculateDiagnoseData(diagnoseDataList, diagnoseEntity, lastAlarm, ref firstNormalDataPoint, ref abnormalTime);

                        if (firstNormalDataPoint != null)
                        {
                            this.alarmRepository.ResetAlarm(diagnoseEntity.Id, firstNormalDataPoint.Value.ToString(), firstNormalDataPoint.LocalTime, "智能诊断自动恢复");
                        }
                    }
                }
                // Alarm Setting is ON
                else
                {
                    var newAlarms = CalculateDiagnoseData(diagnoseDataList, diagnoseEntity, lastAlarm, ref firstNormalDataPoint, ref abnormalTime);
                    if (lastAlarm != null && firstNormalDataPoint != null)
                    {
                        abnormalTime = CalculateNewAbnormTime(abnormalTime, firstNormalDataPoint);
                        this.alarmRepository.ResetAlarm(diagnoseEntity.Id, firstNormalDataPoint.Value.ToString(), firstNormalDataPoint.LocalTime, "智能诊断自动恢复");
                    }
                    // Create new alarms if any
                    if (newAlarms.Any())
                    {
                        newAlarms.ForEach(e => this.alarmRepository.Add(e));
                        lastAlarm = this.alarmRepository.GetLastDiagnoseAlarm(diagnoseEntity.Id);
                        currentAlarmId = lastAlarm.Id;
                        currentAlarmCode = lastAlarm.Code;
                    }

                }
            }

            var lastDiagnoseResult = diagnoseDataList.LastOrDefault();
            this.diagnoseRepository.SetLastAbnormalTime(diagnoseEntity.Id, abnormalTime, currentAlarmId, currentAlarmCode, lastDiagnoseResult.Code, (float?)lastDiagnoseResult.Value, DateTime.Now);

            logger.Debug("the function of set pop diagnose scan result function have executed");
            return true;
        }

        private DateTime? CalculateNewAbnormTime(DateTime? oldAbnormalTime, DiagnoseResult newDiagnoseData)
        {
            var newAbnormalTime = oldAbnormalTime.HasValue ? oldAbnormalTime : !string.IsNullOrEmpty(newDiagnoseData.Code) ? (DateTime?)newDiagnoseData.LocalTime : null;
            if (newAbnormalTime.HasValue && newDiagnoseData.LocalTime < newAbnormalTime) ////rescan
            {
                newAbnormalTime = newDiagnoseData.LocalTime;
            }
            return newAbnormalTime;
        }

        private List<Alarm> CalculateDiagnoseData(IEnumerable<DiagnoseResult> diagnoseDataList, DiagnoseEntity diagnoseEntity, Alarm lastAlarm, ref DiagnoseResult firstNormalDataPoint, ref DateTime? abnormalTime)
        {
            // Get the attributes of the diagnose model
            DiagnoseModelBase diagnoseModel = DiagnoseModelFactory.GetDiagnoseModelByType(diagnoseEntity.DiagnoseModel);

            var currentAlarmCode = string.Empty;
            var currentAlarmLevel = DiagnoseAlarmLevel.None;
            long? currentAlarmId = diagnoseEntity.AlarmId;
            float? thresholdValue = null;

            bool isNormalWithDeadArea = false;

            string currentHealthCode = string.Empty;

            var lastAlarmCode = lastAlarm == null ? "" : lastAlarm.Code;
            var lastAlarmLevel = lastAlarm == null ? (int)DiagnoseAlarmLevel.None : lastAlarm.Level;
            var lastAbnormalValue = lastAlarm == null ? 0 : string.IsNullOrEmpty(lastAlarm.ActualValue) ? 0 : double.Parse(lastAlarm.ActualValue);
            var maintenanceState = this.alarmRepository.GetDeviceMaintenanceState(diagnoseEntity.DeviceId);
            var uom = diagnoseModel.GetUomsFromModel(diagnoseEntity.DiagnoseModel).FirstOrDefault();

            List<Alarm> newAlarms = new List<Alarm>();

            foreach (var diagnoseData in diagnoseDataList)
            {
                currentAlarmCode = diagnoseData.Code ?? string.Empty;
                currentHealthCode = diagnoseData.Code ?? string.Empty;

                currentAlarmLevel = diagnoseData.GetAlarmLevelByCode(diagnoseModel, diagnoseEntity).Value;
                isNormalWithDeadArea = diagnoseModel.JudgeValueWithDeadArea(diagnoseEntity.LowLimit, diagnoseEntity.HighLimit, diagnoseEntity.DeadArea, diagnoseData.Value, lastAbnormalValue, lastAlarmCode, currentAlarmCode);

                ////报警级别降低时,需要加入死区的判断,判断是否真的降低
                if ((int)currentAlarmLevel < lastAlarmLevel)
                {
                    if (isNormalWithDeadArea)
                    {
                        if (firstNormalDataPoint == null)
                        {
                            firstNormalDataPoint = new DiagnoseResult()
                            {
                                Code = diagnoseData.Code,
                                Value = diagnoseData.Value,
                                LocalTime = diagnoseData.LocalTime,
                                Time = diagnoseData.Time,
                                DiagnosticId = diagnoseData.DiagnosticId,
                                State = diagnoseData.State

                            };
                        }
                        newAlarms.Clear();
                        //this.alarmRepository.ResetAlarm(diagnoseEntity.Id, diagnoseData.Value.ToString(), diagnoseData.LocalTime, "智能诊断自动恢复");  ////重置上一次的报警
                    }
                }

                ////报警级别真的发生变化,而且不为空(正常时),需要记录报警 
                ////1. LowLevel->HighLevel, create anew HighLevel alarm 2. After Reset HighLevel, create a new Lowlevel alarm
                if (isNormalWithDeadArea)
                {
                    if (!string.IsNullOrEmpty(currentAlarmCode))
                    {
                        thresholdValue = diagnoseModel.GetThresholdValue(diagnoseEntity.HighLimit, diagnoseEntity.LowLimit, currentAlarmCode, diagnoseData.Value);
                        newAlarms.Add(new Alarm()
                        {
                            HierarchyId = diagnoseEntity.DeviceId,
                            DeviceParameterUniqueId = diagnoseEntity.Id,
                            Code = currentAlarmCode,
                            ActualValue = diagnoseData.Value.ToString(),
                            ThresholdValue = thresholdValue.HasValue ? thresholdValue.Value.ToString() : null,
                            AlarmTime = diagnoseData.LocalTime,
                            Level = (int)currentAlarmLevel,
                            Description = diagnoseEntity.Name + "异常",
                            Uom = uom,
                            AlarmType = AlarmType.PopDiagnose,
                            MaintenanceState = maintenanceState,
                        });
                    }
                }
                else
                {
                    currentAlarmCode = lastAlarmCode;
                }

                abnormalTime = CalculateNewAbnormTime(abnormalTime, diagnoseData);
                lastAlarmCode = currentAlarmCode;
            }
            return newAlarms;
        }
    }
}
