﻿using Google.Apis.Auth.OAuth2;

namespace ManagedCode.Storage.Gcp.Options
{
    public class StorageOptions
    {
        public GoogleCredential GoogleCredential { get; set; }
        public BucketOptions BucketOptions { get; set; }
    }
}