using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SE.PopCom.Host.Business
{
    public class DiagnoseResultMsg
    {
        /// <summary>
        /// array of Diagnose result
        /// </summary>
        [JsonProperty("", Required = Required.Always)]
        public long ParaHardwareId { get; set; }

    }
}
