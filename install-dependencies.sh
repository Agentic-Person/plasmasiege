#!/bin/bash
# Install dependencies for Plasma Siege MCP setup

echo "========================================"
echo "Installing Dependencies"
echo "========================================"
echo ""
echo "This script will install the required system packages."
echo "You may be prompted for your sudo password."
echo ""

# Check if running on Ubuntu/Debian
if [ -f /etc/debian_version ]; then
    echo "Detected Debian/Ubuntu system"
    
    # Update package list
    echo "Updating package list..."
    sudo apt update
    
    # Install Python dependencies
    echo "Installing Python dependencies..."
    sudo apt install -y python3-venv python3-pip python3-dev
    
    # Install Node.js if not present
    if ! command -v node &> /dev/null; then
        echo "Installing Node.js..."
        curl -fsSL https://deb.nodesource.com/setup_20.x | sudo -E bash -
        sudo apt install -y nodejs
    else
        echo "Node.js already installed: $(node --version)"
    fi
    
    # Install Docker if not present
    if ! command -v docker &> /dev/null; then
        echo "Installing Docker..."
        sudo apt install -y docker.io docker-compose
        sudo usermod -aG docker $USER
        echo "IMPORTANT: You need to log out and back in for Docker group changes to take effect!"
    else
        echo "Docker already installed: $(docker --version)"
    fi
    
    # Install git if not present
    if ! command -v git &> /dev/null; then
        echo "Installing git..."
        sudo apt install -y git
    fi
    
else
    echo "This script is designed for Debian/Ubuntu systems."
    echo "Please install the following manually:"
    echo "- python3-venv"
    echo "- python3-pip"
    echo "- nodejs (v20+)"
    echo "- docker"
    echo "- git"
fi

echo ""
echo "========================================"
echo "Dependency installation complete!"
echo "========================================"
echo ""
echo "Next steps:"
echo "1. If Docker was just installed, log out and back in"
echo "2. Run: ./setup-all-mcp.sh"