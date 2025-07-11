#!/bin/bash
# Start all Plasma Siege services

echo "Starting Plasma Siege services..."

# Start N8N if not running
if ! docker ps | grep -q n8n-plasmasiege; then
    echo "Starting N8N container..."
    docker run -d \
        --name n8n-plasmasiege \
        --restart unless-stopped \
        -p 5678:5678 \
        -e N8N_BASIC_AUTH_ACTIVE=true \
        -e N8N_BASIC_AUTH_USER=admin \
        -e N8N_BASIC_AUTH_PASSWORD=plasmasiege2024 \
        -e N8N_ENCRYPTION_KEY=$(openssl rand -hex 32) \
        -v ~/.n8n:/home/node/.n8n \
        n8nio/n8n
else
    echo "N8N already running"
fi

# Start Unity MCP server
echo "Starting Unity MCP server..."
cd /home/benton/projects/plasmasiegeprj/tools/unity-mcp
source venv/bin/activate
python3 server.py &
UNITY_MCP_PID=$!
echo "Unity MCP server started (PID: $UNITY_MCP_PID)"

echo ""
echo "All services started!"
echo "N8N: http://localhost:5678 (admin/plasmasiege2024)"
echo "Unity MCP: ws://localhost:5010"
echo ""
echo "Remember to restart Cursor to load MCP configuration!"
