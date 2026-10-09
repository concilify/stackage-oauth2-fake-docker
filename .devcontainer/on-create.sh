#!/bin/bash
set -euxo pipefail

echo '
cc() {
    claude --permission-mode auto "$@"
}
' >> ~/.bashrc

sudo chown -R vscode:vscode /home/vscode/.nuget

dotnet restore
