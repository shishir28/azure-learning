using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace VideoProcessor
{

    public class ApprovalTableEntity : TableEntity
    {
        public string OrchestrationId { get; set; }

    }
}
