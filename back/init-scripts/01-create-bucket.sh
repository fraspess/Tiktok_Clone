#!/bin/bash

aws --endpoint-url=http://localhost:4566 s3 mb s3://tiktok-clone-academy2026 || true

aws --endpoint-url=http://localhost:4566 s3api put-bucket-cors \
  --bucket tiktok-clone-academy2026 \
  --cors-configuration '{
    "CORSRules": [{
      "AllowedOrigins": ["*"],
      "AllowedMethods": ["GET", "PUT", "HEAD"],
      "AllowedHeaders": ["*"],
      "ExposeHeaders": ["ETag"],
      "MaxAgeSeconds": 3000
    }]
  }' || true

