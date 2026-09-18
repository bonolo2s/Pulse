$REGION = "eu-west-1"
$ACCOUNT_ID = "881005428470"
$API_TAG = "api-v1"
$LAMBDA_TAG = "lambda-v1"
$MIGRATE_TAG = "migrate"

$ErrorActionPreference = "Stop"

function Run-Step {
    param($Description, $ScriptBlock)
    Write-Host $Description
    try {
        & $ScriptBlock
        if ($LASTEXITCODE -ne 0) {
            throw "Command failed with exit code $LASTEXITCODE"
        }
    } catch {
        Write-Host "FAILED: $Description"
        Write-Host $_.Exception.Message
        exit 1
    }
}

Write-Host "Creating ECR repositories (safe to fail if they already exist)..."
try { aws ecr create-repository --repository-name pulse-api --region $REGION } catch {}
try { aws ecr create-repository --repository-name pulse-lambda --region $REGION } catch {}

Run-Step "Building API image..." { docker build -t pulse-api:$API_TAG . }

Run-Step "Building Lambda image..." { docker build --provenance=false -t pulse-lambda:$LAMBDA_TAG -f Dockerfile.lambda . }

Run-Step "Building Migration image..." { docker build -t pulse-api:$MIGRATE_TAG -f Dockerfile.migrate . }

Run-Step "Logging into ECR..." {
    aws ecr get-login-password --region $REGION | docker login --username AWS --password-stdin "$ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com"
}

Run-Step "Tagging and pushing API image..." {
    docker tag pulse-api:$API_TAG "$ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/pulse-api:$API_TAG"
    docker push "$ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/pulse-api:$API_TAG"
}

Run-Step "Tagging and pushing Migration image..." {
    docker tag pulse-api:$MIGRATE_TAG "$ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/pulse-api:$MIGRATE_TAG"
    docker push "$ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/pulse-api:$MIGRATE_TAG"
}

Run-Step "Tagging and pushing Lambda image..." {
    docker tag pulse-lambda:$LAMBDA_TAG "$ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/pulse-lambda:$LAMBDA_TAG"
    docker push "$ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/pulse-lambda:$LAMBDA_TAG"
}

Write-Host "Done. Images pushed to AWS."