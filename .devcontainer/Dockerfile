FROM mcr.microsoft.com/devcontainers/dotnet:10.0

USER vscode

ENV PATH="/home/vscode/.dotnet/tools:/home/vscode/.local/bin:${PATH}"

RUN curl -fsSL https://claude.ai/install.sh | bash \
    && claude --version
