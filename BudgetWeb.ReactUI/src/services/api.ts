import axios from 'axios'

const API_BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:5001/api'

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
})

// Request interceptor for adding auth token
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('authToken')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => Promise.reject(error)
)

// Response interceptor for handling errors
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Handle unauthorized
      localStorage.removeItem('authToken')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

// Types
export interface Budget {
  id: string
  name: string
  description: string
  totalAmount: Money
  spentAmount: Money
  period: string
  startDate: string
  endDate: string
  status: string
  categories: Category[]
}

export interface Money {
  amount: number
  currency: string
}

export interface Category {
  id: string
  name: string
  budgetedAmount: Money
  spentAmount: Money
  type: string
}

export interface Project {
  id: string
  name: string
  description: string
  budget: Money
  actualCost: Money
  startDate: string
  endDate: string
  status: string
  priority: string
  completionPercentage: number
  phases: ProjectPhase[]
}

export interface ProjectPhase {
  id: string
  name: string
  startDate: string
  endDate: string
  budget: Money
  actualCost: Money
  status: string
}

export interface Transaction {
  id: string
  description: string
  amount: Money
  date: string
  type: string
  status: string
  categoryId: string
  budgetId: string
  projectId?: string
  vendor?: string
  tags: string[]
}

export interface DashboardData {
  totalBudgets: number
  activeBudgets: number
  totalProjects: number
  activeProjects: number
  totalTransactions: number
  totalSpent: Money
  totalBudgeted: Money
  utilizationRate: number
  monthlyTrend: ChartDataPoint[]
  categoryBreakdown: ChartDataPoint[]
}

export interface ChartDataPoint {
  name: string
  value: number
}

export interface VarianceReport {
  budgetId: string
  budgetName: string
  budgeted: Money
  actual: Money
  variance: Money
  variancePercentage: number
  status: string
}

// Budget API
export const budgetApi = {
  getAll: async (): Promise<Budget[]> => {
    const response = await apiClient.get('/budgets')
    return response.data
  },

  getById: async (id: string): Promise<Budget> => {
    const response = await apiClient.get(`/budgets/${id}`)
    return response.data
  },

  create: async (budget: Partial<Budget>): Promise<Budget> => {
    const response = await apiClient.post('/budgets', budget)
    return response.data
  },

  update: async (id: string, budget: Partial<Budget>): Promise<Budget> => {
    const response = await apiClient.put(`/budgets/${id}`, budget)
    return response.data
  },

  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/budgets/${id}`)
  },

  activate: async (id: string): Promise<Budget> => {
    const response = await apiClient.post(`/budgets/${id}/activate`)
    return response.data
  },

  getVarianceReport: async (id: string): Promise<VarianceReport> => {
    const response = await apiClient.get(`/budgets/${id}/variance`)
    return response.data
  },
}

// Project API
export const projectApi = {
  getAll: async (): Promise<Project[]> => {
    const response = await apiClient.get('/projects')
    return response.data
  },

  getById: async (id: string): Promise<Project> => {
    const response = await apiClient.get(`/projects/${id}`)
    return response.data
  },

  create: async (project: Partial<Project>): Promise<Project> => {
    const response = await apiClient.post('/projects', project)
    return response.data
  },

  update: async (id: string, project: Partial<Project>): Promise<Project> => {
    const response = await apiClient.put(`/projects/${id}`, project)
    return response.data
  },

  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/projects/${id}`)
  },

  getCostAnalysis: async (id: string): Promise<any> => {
    const response = await apiClient.get(`/projects/${id}/cost-analysis`)
    return response.data
  },
}

// Transaction API
export const transactionApi = {
  getAll: async (): Promise<Transaction[]> => {
    const response = await apiClient.get('/transactions')
    return response.data
  },

  getById: async (id: string): Promise<Transaction> => {
    const response = await apiClient.get(`/transactions/${id}`)
    return response.data
  },

  create: async (transaction: Partial<Transaction>): Promise<Transaction> => {
    const response = await apiClient.post('/transactions', transaction)
    return response.data
  },

  update: async (id: string, transaction: Partial<Transaction>): Promise<Transaction> => {
    const response = await apiClient.put(`/transactions/${id}`, transaction)
    return response.data
  },

  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/transactions/${id}`)
  },

  getByBudget: async (budgetId: string): Promise<Transaction[]> => {
    const response = await apiClient.get(`/transactions/budget/${budgetId}`)
    return response.data
  },

  getByDateRange: async (startDate: string, endDate: string): Promise<Transaction[]> => {
    const response = await apiClient.get('/transactions/date-range', {
      params: { startDate, endDate },
    })
    return response.data
  },
}

// Finance API
export const financeApi = {
  getDashboard: async (): Promise<DashboardData> => {
    const response = await apiClient.get('/finance/dashboard')
    return response.data
  },

  getSummary: async (): Promise<any> => {
    const response = await apiClient.get('/finance/summary')
    return response.data
  },

  getCashFlow: async (startDate: string, endDate: string): Promise<any> => {
    const response = await apiClient.get('/finance/cash-flow', {
      params: { startDate, endDate },
    })
    return response.data
  },

  getTrends: async (): Promise<any> => {
    const response = await apiClient.get('/finance/trends')
    return response.data
  },

  getForecasts: async (): Promise<any> => {
    const response = await apiClient.get('/finance/forecasts')
    return response.data
  },
}

// Calculator API
export const calculatorApi = {
  calculateVariance: async (budgeted: number, actual: number): Promise<any> => {
    const response = await apiClient.post('/calculators/variance', { budgeted, actual })
    return response.data
  },

  calculateROI: async (investment: number, returns: number): Promise<any> => {
    const response = await apiClient.post('/calculators/roi', { investment, returns })
    return response.data
  },

  calculateIRR: async (initialInvestment: number, cashFlows: number[]): Promise<any> => {
    const response = await apiClient.post('/calculators/irr', { initialInvestment, cashFlows })
    return response.data
  },

  calculateLoanPayment: async (principal: number, rate: number, periods: number): Promise<any> => {
    const response = await apiClient.post('/calculators/loan-payment', { principal, rate, periods })
    return response.data
  },
}

export default apiClient
