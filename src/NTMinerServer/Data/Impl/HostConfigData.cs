﻿using System;

namespace NTMiner.Data.Impl {
    public class HostConfigData {
        public Guid Id { get; set; }
        public string OssAccessKeyId { get; set; }
        public string OssAccessKeySecret { get; set; }
        public string OssEndpoint { get; set; }
    }
}
