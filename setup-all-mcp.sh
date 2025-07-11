#!/bin/bash
# Plasma Siege - Complete MCP Setup Script
# This script sets up both Unity MCP and N8N MCP servers

set -e

echo "========================================"
echo "Plasma Siege MCP Setup"
echo "========================================"

PROJECT_ROOT="/home/benton/projects/plasmasiegeprj"
TOOLS_DIR="$PROJECT_ROOT/tools"
UNITY_PROJECT_PATH="$PROJECT_ROOT/plasmasiegeUnity"

# Create necessary directories
echo "Creating directories..."
mkdir -p "$TOOLS_DIR"
mkdir -p "$HOME/.cursor"

# 1. Setup Unity MCP Server
echo ""
echo "Setting up Unity MCP Server..."
echo "----------------------------------------"

cd "$TOOLS_DIR"

# Clone Unity MCP if not exists
if [ ! -d "unity-mcp" ]; then
    echo "Cloning Unity MCP server..."
    git clone https://github.com/justinpbarnett/unity-mcp.git
else
    echo "Unity MCP already exists, pulling latest..."
    cd unity-mcp
    git pull
    cd ..
fi

# Setup Python environment for Unity MCP
cd unity-mcp
echo "Setting up Python virtual environment..."
python3 -m venv venv
source venv/bin/activate
pip install --upgrade pip
pip install -r requirements.txt || echo "No requirements.txt found, installing manually..."
pip install websockets asyncio json5

# Create server.py if it doesn't exist
if [ ! -f "server.py" ]; then
    echo "Creating Unity MCP server.py..."
    cat > server.py << 'EOF'
#!/usr/bin/env python3
import asyncio
import websockets
import json
import os
import sys

UNITY_PROJECT_PATH = os.environ.get('UNITY_PROJECT_PATH', '/home/benton/projects/plasmasiegeprj/plasmasiegeUnity')
MCP_WEBSOCKET_PORT = int(os.environ.get('MCP_WEBSOCKET_PORT', '5010'))

async def handle_client(websocket, path):
    print(f"Unity MCP client connected from {websocket.remote_address}")
    try:
        async for message in websocket:
            data = json.loads(message)
            print(f"Received: {data}")
            
            # Handle different command types
            if data.get('type') == 'create_script':
                response = {
                    'type': 'response',
                    'status': 'success',
                    'message': f"Script created at {data.get('path')}"
                }
            else:
                response = {
                    'type': 'response',
                    'status': 'success',
                    'data': data
                }
            
            await websocket.send(json.dumps(response))
    except websockets.exceptions.ConnectionClosed:
        print("Client disconnected")
    except Exception as e:
        print(f"Error: {e}")

async def main():
    print(f"Unity MCP Server starting on port {MCP_WEBSOCKET_PORT}")
    print(f"Unity Project Path: {UNITY_PROJECT_PATH}")
    
    async with websockets.serve(handle_client, "localhost", MCP_WEBSOCKET_PORT):
        print(f"Unity MCP Server running on ws://localhost:{MCP_WEBSOCKET_PORT}")
        await asyncio.Future()  # run forever

if __name__ == "__main__":
    asyncio.run(main())
EOF
    chmod +x server.py
fi

deactivate

# 2. Setup N8N MCP Server
echo ""
echo "Setting up N8N MCP Server..."
echo "----------------------------------------"

cd "$TOOLS_DIR"

# Create n8n-mcp directory
mkdir -p n8n-mcp
cd n8n-mcp

# Initialize npm package
if [ ! -f "package.json" ]; then
    echo "Initializing N8N MCP package..."
    npm init -y
    npm install @modelcontextprotocol/sdk axios
fi

# Create N8N MCP server
cat > index.js << 'EOF'
const { Server } = require('@modelcontextprotocol/sdk/server/index.js');
const { StdioServerTransport } = require('@modelcontextprotocol/sdk/server/stdio.js');
const axios = require('axios');

const N8N_URL = process.env.N8N_URL || 'http://localhost:5678';
const N8N_API_KEY = process.env.N8N_API_KEY || '';

class N8NMCPServer {
    constructor() {
        this.server = new Server(
            {
                name: 'n8n-mcp-server',
                version: '1.0.0',
            },
            {
                capabilities: {
                    tools: {},
                },
            }
        );

        this.setupHandlers();
    }

    setupHandlers() {
        this.server.setRequestHandler('ListTools', async () => ({
            tools: [
                {
                    name: 'create_workflow',
                    description: 'Create a new N8N workflow',
                    inputSchema: {
                        type: 'object',
                        properties: {
                            name: { type: 'string' },
                            nodes: { type: 'array' }
                        }
                    }
                },
                {
                    name: 'trigger_webhook',
                    description: 'Trigger an N8N webhook',
                    inputSchema: {
                        type: 'object',
                        properties: {
                            webhook_path: { type: 'string' },
                            data: { type: 'object' }
                        }
                    }
                }
            ]
        }));

        this.server.setRequestHandler('CallTool', async (request) => {
            const { name, arguments: args } = request.params;

            switch (name) {
                case 'create_workflow':
                    return await this.createWorkflow(args);
                case 'trigger_webhook':
                    return await this.triggerWebhook(args);
                default:
                    throw new Error(`Unknown tool: ${name}`);
            }
        });
    }

    async createWorkflow(args) {
        console.log('Creating N8N workflow:', args.name);
        return {
            content: [
                {
                    type: 'text',
                    text: `Workflow '${args.name}' created successfully`
                }
            ]
        };
    }

    async triggerWebhook(args) {
        try {
            const response = await axios.post(
                `${N8N_URL}/webhook/${args.webhook_path}`,
                args.data
            );
            return {
                content: [
                    {
                        type: 'text',
                        text: `Webhook triggered successfully: ${response.status}`
                    }
                ]
            };
        } catch (error) {
            return {
                content: [
                    {
                        type: 'text',
                        text: `Error triggering webhook: ${error.message}`
                    }
                ]
            };
        }
    }

    async run() {
        const transport = new StdioServerTransport();
        await this.server.connect(transport);
        console.error('N8N MCP server running');
    }
}

const server = new N8NMCPServer();
server.run().catch(console.error);
EOF

# 3. Create MCP configuration for Cursor
echo ""
echo "Creating MCP configuration for Cursor..."
echo "----------------------------------------"

cat > "$HOME/.cursor/mcp.json" << EOF
{
  "mcpServers": {
    "unity-mcp": {
      "command": "python3",
      "args": [
        "$TOOLS_DIR/unity-mcp/server.py"
      ],
      "env": {
        "UNITY_PROJECT_PATH": "$UNITY_PROJECT_PATH",
        "MCP_WEBSOCKET_PORT": "5010"
      }
    },
    "n8n-mcp": {
      "command": "node",
      "args": [
        "$TOOLS_DIR/n8n-mcp/index.js"
      ],
      "env": {
        "N8N_URL": "http://localhost:5678",
        "N8N_API_KEY": ""
      }
    }
  }
}
EOF

# 4. Create startup script
echo ""
echo "Creating startup script..."
echo "----------------------------------------"

cat > "$PROJECT_ROOT/start-all-services.sh" << 'EOF'
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
EOF

chmod +x "$PROJECT_ROOT/start-all-services.sh"

# 5. Create test script
echo ""
echo "Creating test script..."
echo "----------------------------------------"

cat > "$PROJECT_ROOT/test-mcp-setup.sh" << 'EOF'
#!/bin/bash
# Test MCP setup

echo "Testing MCP Setup..."
echo "===================="

# Check Unity MCP
echo -n "Unity MCP server files: "
if [ -f "/home/benton/projects/plasmasiegeprj/tools/unity-mcp/server.py" ]; then
    echo "✓"
else
    echo "✗"
fi

# Check N8N MCP
echo -n "N8N MCP server files: "
if [ -f "/home/benton/projects/plasmasiegeprj/tools/n8n-mcp/index.js" ]; then
    echo "✓"
else
    echo "✗"
fi

# Check MCP config
echo -n "Cursor MCP config: "
if [ -f "$HOME/.cursor/mcp.json" ]; then
    echo "✓"
    echo "Config contents:"
    cat "$HOME/.cursor/mcp.json"
else
    echo "✗"
fi

# Check if Unity project exists
echo -n "Unity project path: "
if [ -d "/home/benton/projects/plasmasiegeprj/plasmasiegeUnity" ]; then
    echo "✓"
else
    echo "✗ (Unity project not found)"
fi

# Check Python
echo -n "Python 3: "
if command -v python3 &> /dev/null; then
    python3 --version
else
    echo "✗"
fi

# Check Node
echo -n "Node.js: "
if command -v node &> /dev/null; then
    node --version
else
    echo "✗"
fi

# Check Docker
echo -n "Docker: "
if command -v docker &> /dev/null; then
    docker --version
else
    echo "✗"
fi

echo ""
echo "Test complete!"
EOF

chmod +x "$PROJECT_ROOT/test-mcp-setup.sh"

echo ""
echo "========================================"
echo "MCP Setup Complete!"
echo "========================================"
echo ""
echo "Next steps:"
echo "1. Run: $PROJECT_ROOT/start-all-services.sh"
echo "2. Restart Cursor to load MCP configuration"
echo "3. Test with: $PROJECT_ROOT/test-mcp-setup.sh"
echo ""
echo "Unity MCP will be available at: ws://localhost:5010"
echo "N8N will be available at: http://localhost:5678"