# Budget App - .NET 10 + React 18 Desktop Application

A modern, cross-platform desktop budget application built with .NET 10 and React 18.

## 🚀 Quick Start

### macOS

```bash
# Terminal 1 - Frontend
cd BudgetWeb/ClientApp
npm install
npm start

# Terminal 2 - Backend
export PATH=/usr/local/share/dotnet:$PATH
cd BudgetWeb
dotnet run
```

### Windows

```powershell
# Terminal 1 - Frontend
cd BudgetWeb\ClientApp
npm install
npm start

# Terminal 2 - Backend
cd BudgetWeb
dotnet run
```

Visit: http://localhost:3000

## 🏗️ Tech Stack

### Backend
- **.NET 10.0.100** - Latest .NET runtime
- **ASP.NET Core** - Web framework
- **Electron.NET** - Desktop app wrapper

### Frontend
- **React 18.3** - UI library
- **React Router v6** - Navigation
- **Redux Toolkit** - State management
- **Bootstrap 5** - Styling
- **Vite** - Build tool
- **TypeScript 5.6** - Type safety

## 📦 Build for Production

### Web Application
```bash
# Build frontend
cd BudgetWeb/ClientApp
npm run build

# Build backend
cd ../
dotnet publish -c Release -o ./publish
```

### Desktop Application

#### macOS
```bash
export PATH=/usr/local/share/dotnet:$PATH
cd BudgetWeb
electronize build /target osx
```

#### Windows
```bash
cd BudgetWeb
electronize build /target win
```

## 📖 Documentation

See [MIGRATION_GUIDE.md](./MIGRATION_GUIDE.md) for complete migration documentation including:
- Detailed package changes
- Breaking changes and fixes
- Performance improvements
- Build instructions for all platforms

## 🔧 Development

### Project Structure
```
BudgetWeb/
├── ClientApp/              # React frontend
│   ├── src/
│   │   ├── components/    # React components
│   │   ├── store/         # Redux store
│   │   └── index.tsx      # Entry point
│   ├── public/            # Static assets
│   ├── vite.config.ts     # Vite configuration
│   └── package.json
├── Controllers/           # API controllers
├── Pages/                 # Razor pages
├── Program.cs            # Application entry point
├── BudgetWeb.csproj      # Project file
└── MIGRATION_GUIDE.md    # Full documentation
```

### Available Scripts

#### Frontend (ClientApp/)
- `npm start` - Start Vite dev server (fast HMR)
- `npm run build` - Build for production
- `npm run preview` - Preview production build

#### Backend (BudgetWeb/)
- `dotnet run` - Start development server
- `dotnet build` - Build project
- `dotnet publish` - Create production build
- `electronize start` - Run as desktop app
- `electronize build` - Build desktop executable

## 🌟 Features

- ✅ Cross-platform (macOS, Windows, Linux)
- ✅ Modern React 18 with hooks
- ✅ Fast Vite development server
- ✅ Redux Toolkit for state management
- ✅ TypeScript for type safety
- ✅ Bootstrap 5 responsive design
- ✅ Desktop app with Electron.NET
- ✅ .NET 10 performance improvements

## 📝 License

This project is licensed under the MIT License.

## 🙋 Support

For detailed migration information and troubleshooting, see [MIGRATION_GUIDE.md](./MIGRATION_GUIDE.md).
