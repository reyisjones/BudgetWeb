#!/bin/bash

# Budget App Setup Script
# This script sets up the development environment for the Budget App

set -e  # Exit on error

echo "🚀 Budget App Setup Script"
echo "=========================="
echo ""

# Check for .NET 10
echo "📦 Checking for .NET 10..."
export PATH=/usr/local/share/dotnet:$PATH

if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET not found!"
    echo "Please install .NET 10 SDK from: https://dotnet.microsoft.com/download"
    exit 1
fi

DOTNET_VERSION=$(dotnet --version)
echo "✅ Found .NET version: $DOTNET_VERSION"

if [[ ! "$DOTNET_VERSION" =~ ^10\. ]]; then
    echo "⚠️  Warning: .NET 10 is recommended. Found version $DOTNET_VERSION"
fi

# Check for Node.js
echo ""
echo "📦 Checking for Node.js..."
if ! command -v node &> /dev/null; then
    echo "❌ Node.js not found!"
    echo "Please install Node.js from: https://nodejs.org/"
    exit 1
fi

NODE_VERSION=$(node --version)
echo "✅ Found Node.js version: $NODE_VERSION"

# Install .NET dependencies
echo ""
echo "📥 Restoring .NET packages..."
cd BudgetWeb
dotnet restore

# Install Node.js dependencies
echo ""
echo "📥 Installing Node.js packages..."
cd ClientApp
npm install

# Build React app
echo ""
echo "🔨 Building React app..."
npm run build

# Build .NET app
echo ""
echo "🔨 Building .NET app..."
cd ..
dotnet build -c Release

echo ""
echo "✅ Setup complete!"
echo ""
echo "📖 To run the application:"
echo ""
echo "  Development mode (2 terminals required):"
echo "  Terminal 1: cd BudgetWeb/ClientApp && npm start"
echo "  Terminal 2: cd BudgetWeb && dotnet run"
echo ""
echo "  Desktop mode:"
echo "  dotnet tool install ElectronNET.CLI -g"
echo "  cd BudgetWeb && electronize start"
echo ""
echo "See README.md for more information."
