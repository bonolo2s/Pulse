$REGION = "eu-west-1"
$ACCOUNT_ID = "881005428470"
$IMAGE_TAG = "net9-v1"

Write-Host "Building API image..." ##for new changes
docker build -t pulse-api:$IMAGE_TAG .

Write-Host "Logging into ECR..."
aws ecr get-login-password --region $REGION | docker login --username AWS --password-stdin "$ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com"

Write-Host "Tagging and pushing API image..."
docker tag pulse-api:$IMAGE_TAG "$ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/pulse-api:$IMAGE_TAG"
docker push "$ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/pulse-api:$IMAGE_TAG"

Write-Host "Pushing Lambda zip to S3..."
aws s3 cp publish/lambda.zip "s3://pulse-logs-dev-$ACCOUNT_ID/lambda/lambda.zip"

Write-Host "Done. Image and Lambda code pushed to AWS."