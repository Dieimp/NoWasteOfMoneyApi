#!/usr/bin/env bash
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
TEST_PROJECT="$REPO_ROOT/tests/NoWasteOfMoney.Tests/NoWasteOfMoney.Tests.csproj"

find_backend_container() {
  local filter line
  for filter in "name=nowasteofmoney_api" "name=nowasteofmoney-api" "name=api"; do
    line="$(docker ps --filter "$filter" --format '{{.ID}}|{{.Names}}|{{.Ports}}' 2>/dev/null | head -n 1 || true)"
    if [[ -n "$line" ]]; then
      echo "$line"
      return 0
    fi
  done

  docker ps --format '{{.ID}}|{{.Names}}|{{.Ports}}' 2>/dev/null \
    | grep -Ei 'api|backend|nowasteofmoney' \
    | head -n 1 || true
}

extract_host_port() {
  local ports="$1"
  if [[ "$ports" =~ 0\.0\.0\.0:([0-9]+)-\> ]]; then
    echo "${BASH_REMATCH[1]}"
  elif [[ "$ports" =~ :([0-9]+)-\> ]]; then
    echo "${BASH_REMATCH[1]}"
  else
    echo "8080"
  fi
}

echo "[1/4] Detecting backend container..."
container_line="$(find_backend_container || true)"

if [[ -z "$container_line" ]]; then
  echo "WARN: No backend container found. Using API_BASE_URL=http://localhost:8080"
  export API_BASE_URL="http://localhost:8080"
else
  IFS='|' read -r container_id container_name container_ports <<< "$container_line"
  host_port="$(extract_host_port "$container_ports")"
  export API_BASE_URL="http://localhost:${host_port}"
  echo "Container: ${container_name} (${container_id})"
  echo "API_BASE_URL: ${API_BASE_URL}"
fi

echo "[2/4] Ensuring test project exists..."
[[ -f "$TEST_PROJECT" ]] || { echo "Test project not found: $TEST_PROJECT"; exit 1; }

echo "[3/4] Restoring test dependencies (idempotent)..."
dotnet restore "$TEST_PROJECT"

echo "[4/4] Running smoke tests..."
dotnet test "$TEST_PROJECT" --no-restore --verbosity normal
