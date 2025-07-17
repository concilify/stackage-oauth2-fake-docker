$ComposeFiles = @(
"-f"
"docker-compose.app.yaml"
)

docker compose $ComposeFiles build

if ($? -eq $false) {
  exit
}

docker compose $ComposeFiles up --detach --renew-anon-volumes app

$env:STACKAGEOAUTH2FAKETESTS_APP_URL = "http://localhost:32111/"
$env:STACKAGEOAUTH2FAKETESTS_ISSUER_URL = "http://localhost:32111"

dotnet test -l "console;verbosity=detailed" -c Release outside-in.tests/Stackage.OAuth2.Fake.OutsideIn.Tests/Stackage.OAuth2.Fake.OutsideIn.Tests.csproj

if ($? -eq $false) {
  exit
}

docker compose $ComposeFiles down
