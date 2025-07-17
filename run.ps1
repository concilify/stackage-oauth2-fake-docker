$ComposeFiles = @(
"-f"
"docker-compose.app.yaml"
)

docker compose $ComposeFiles build

if ($? -eq $false) {
  exit
}

docker compose $ComposeFiles up --renew-anon-volumes
