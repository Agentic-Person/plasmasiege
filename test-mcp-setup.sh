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
