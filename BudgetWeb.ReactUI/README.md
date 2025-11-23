# BudgetWeb React UI

Modern React.js frontend for BudgetWeb using Material UI Dashboard template.

## Features

- 🎨 Material UI Dashboard Template
- ⚡ Vite for fast development
- 📘 TypeScript for type safety
- 📊 Recharts for data visualization
- 📱 Responsive design
- 🎯 Clean architecture with service layer

## Tech Stack

- **React 18** - UI library
- **TypeScript** - Type safety
- **Material UI 5** - Component library
- **Vite** - Build tool
- **React Router 6** - Routing
- **Axios** - HTTP client
- **Recharts** - Charts and visualization
- **MUI X Data Grid** - Advanced tables

## Getting Started

### Prerequisites

- Node.js 18+ and npm
- .NET 10 API running on https://localhost:5001

### Installation

```bash
# Install dependencies
npm install

# Copy environment variables
cp .env.example .env

# Start development server
npm run dev
```

The application will be available at http://localhost:3000

### Build for Production

```bash
npm run build
npm run preview
```

## Project Structure

```
src/
├── layouts/          # Layout components
│   ├── DashboardLayout.tsx
│   └── components/   # Layout sub-components
│       ├── AppBar.tsx
│       └── Drawer.tsx
├── pages/            # Page components
│   ├── Dashboard.tsx
│   ├── Budgets/
│   │   ├── BudgetList.tsx
│   │   ├── BudgetDetail.tsx
│   │   └── BudgetCreate.tsx
│   ├── Projects/
│   ├── Transactions/
│   ├── Reports.tsx
│   └── Calculators.tsx
├── services/         # API services
│   └── api.ts
├── theme.ts          # Material UI theme
├── App.tsx           # Main app component
└── main.tsx          # Entry point
```

## API Integration

The app connects to the .NET API at https://localhost:5001/api

API endpoints:
- `/budgets` - Budget management
- `/projects` - Project tracking
- `/transactions` - Transaction management
- `/finance` - Financial dashboard and analytics
- `/reports` - Report generation
- `/calculators` - F# calculation functions

## Material UI Dashboard

Based on the official Material UI Dashboard template with:
- Persistent drawer navigation
- Responsive app bar
- Chart visualizations
- Data tables with sorting/filtering
- Form components
- Card layouts
- Chip status indicators

## Development

### Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build
- `npm run lint` - Run ESLint

### Environment Variables

Create a `.env` file with:

```
VITE_API_URL=https://localhost:5001/api
```

## Learn More

- [Material UI Documentation](https://mui.com/)
- [Material UI Dashboard Template](https://mui.com/material-ui/getting-started/templates/dashboard/)
- [React Documentation](https://react.dev/)
- [Vite Documentation](https://vitejs.dev/)
