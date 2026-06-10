$ENDPOINT = "http://localhost:4566"
$REGION = "eu-west-1"
$PROFILE = "localstack" 

Write-Host "Setting up LocalStack resources..."

# SNS
Write-Host "Creating SNS topic..."
aws sns create-topic `
  --name pulse-alerts-dev `
  --profile $PROFILE `
  --endpoint-url $ENDPOINT `
  --region $REGION

# SES
Write-Host "Verifying SES email identity..."
aws ses verify-email-identity `
  --email-address noreply@pulse.dev `
  --profile $PROFILE `
  --endpoint-url $ENDPOINT `
  --region $REGION

# EventBridge rules
Write-Host "Creating EventBridge rules..."
$intervals = @(
    @{ Name = "1min";  Rate = "rate(1 minute)";  Seconds = 60 },
    @{ Name = "5min";  Rate = "rate(5 minutes)"; Seconds = 300 },
    @{ Name = "10min"; Rate = "rate(10 minutes)"; Seconds = 600 },
    @{ Name = "15min"; Rate = "rate(15 minutes)"; Seconds = 900 },
    @{ Name = "30min"; Rate = "rate(30 minutes)"; Seconds = 1800 },
    @{ Name = "1hour"; Rate = "rate(1 hour)";     Seconds = 3600 }
)

foreach ($interval in $intervals) {
    aws events put-rule `
      --name "pulse-health-check-$($interval.Name)" `
      --schedule-expression "$($interval.Rate)" `
      --state ENABLED `
      --profile $PROFILE `
      --endpoint-url $ENDPOINT `
      --region $REGION

    Write-Host "Created rule: pulse-health-check-$($interval.Name) ($($interval.Seconds)s)"
}

Write-Host "Done. LocalStack resources provisioned."