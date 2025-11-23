import { Routes, Route, Navigate } from 'react-router-dom'
import DashboardLayout from './layouts/DashboardLayout'
import Dashboard from './pages/Dashboard'
import BudgetList from './pages/Budgets/BudgetList'
import BudgetDetail from './pages/Budgets/BudgetDetail'
import BudgetCreate from './pages/Budgets/BudgetCreate'
import ProjectList from './pages/Projects/ProjectList'
import ProjectDetail from './pages/Projects/ProjectDetail'
import ProjectCreate from './pages/Projects/ProjectCreate'
import TransactionList from './pages/Transactions/TransactionList'
import TransactionCreate from './pages/Transactions/TransactionCreate'
import Reports from './pages/Reports'
import Calculators from './pages/Calculators'

function App() {
  return (
    <Routes>
      <Route path="/" element={<DashboardLayout />}>
        <Route index element={<Navigate to="/dashboard" replace />} />
        <Route path="dashboard" element={<Dashboard />} />
        
        {/* Budget Routes */}
        <Route path="budgets" element={<BudgetList />} />
        <Route path="budgets/create" element={<BudgetCreate />} />
        <Route path="budgets/:id" element={<BudgetDetail />} />
        
        {/* Project Routes */}
        <Route path="projects" element={<ProjectList />} />
        <Route path="projects/create" element={<ProjectCreate />} />
        <Route path="projects/:id" element={<ProjectDetail />} />
        
        {/* Transaction Routes */}
        <Route path="transactions" element={<TransactionList />} />
        <Route path="transactions/create" element={<TransactionCreate />} />
        
        {/* Other Routes */}
        <Route path="reports" element={<Reports />} />
        <Route path="calculators" element={<Calculators />} />
      </Route>
    </Routes>
  )
}

export default App
