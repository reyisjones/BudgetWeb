import { useState, useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import {
  Box,
  Container,
  Typography,
  Button,
  Card,
  CardContent,
  Grid,
  Chip,
  LinearProgress,
  Paper,
  List,
  ListItem,
  ListItemText,
  Divider,
  Alert,
} from '@mui/material'
import ArrowBackIcon from '@mui/icons-material/ArrowBack'
import TrendingUpIcon from '@mui/icons-material/TrendingUp'
import TrendingDownIcon from '@mui/icons-material/TrendingDown'
import CheckCircleIcon from '@mui/icons-material/CheckCircle'
import WarningIcon from '@mui/icons-material/Warning'
import ErrorIcon from '@mui/icons-material/Error'
import { BarChart, Bar, LineChart, Line, PieChart, Pie, Cell, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts'

interface ProgressReport {
  projectName: string
  completionPercentage: number
  tasksTotal: number
  tasksCompleted: number
  tasksInProgress: number
  tasksBlocked: number
  onSchedule: boolean
  scheduleVarianceDays: number
  estimatedCompletionDate: string
}

interface CostAnalysisReport {
  projectName: string
  budget: number
  actualCost: number
  variance: number
  variancePercent: number
  isOverBudget: boolean
  costsByCategory: Record<string, number>
  cpi?: number
  forecastedTotal: number
  estimatedToComplete: number
}

interface ResourceUtilizationReport {
  projectName: string
  totalLaborHours: number
  laborCosts: number
  materialCosts: number
  equipmentCosts: number
  lowInventoryItems: InventoryItem[]
}

interface InventoryItem {
  id: string
  name: string
  category: string
  quantityOnHand: number
  reorderPoint: number
  belowReorderPoint: boolean
}

interface Task {
  id: string
  name: string
  status: string
  priority: string
  dueDate?: string
  assignedTo: string[]
}

interface Dashboard {
  progress: ProgressReport
  costAnalysis: CostAnalysisReport
  resourceUtilization: ResourceUtilizationReport
  upcomingTasks: Task[]
  blockedTasks: Task[]
}

const COLORS = ['#0088FE', '#00C49F', '#FFBB28', '#FF8042', '#8884D8', '#82CA9D']

export default function ProjectDashboard() {
  const { projectId } = useParams<{ projectId: string }>()
  const navigate = useNavigate()
  const [dashboard, setDashboard] = useState<Dashboard | null>(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    loadDashboard()
  }, [projectId])

  const loadDashboard = async () => {
    setLoading(true)
    try {
      // TODO: Replace with actual API call
      const mockDashboard: Dashboard = {
        progress: {
          projectName: 'Downtown Office Building',
          completionPercentage: 35,
          tasksTotal: 45,
          tasksCompleted: 18,
          tasksInProgress: 12,
          tasksBlocked: 2,
          onSchedule: true,
          scheduleVarianceDays: -3,
          estimatedCompletionDate: '2025-12-15',
        },
        costAnalysis: {
          projectName: 'Downtown Office Building',
          budget: 5000000,
          actualCost: 1850000,
          variance: 100000,
          variancePercent: 2.0,
          isOverBudget: false,
          costsByCategory: {
            Labor: 650000,
            Materials: 850000,
            Equipment: 250000,
            Permits: 50000,
            Overhead: 50000,
          },
          cpi: 1.05,
          forecastedTotal: 4900000,
          estimatedToComplete: 3050000,
        },
        resourceUtilization: {
          projectName: 'Downtown Office Building',
          totalLaborHours: 3250,
          laborCosts: 650000,
          materialCosts: 850000,
          equipmentCosts: 250000,
          lowInventoryItems: [
            {
              id: '1',
              name: 'Lumber 2x4x8',
              category: 'Materials',
              quantityOnHand: 45,
              reorderPoint: 100,
              belowReorderPoint: true,
            },
            {
              id: '2',
              name: 'Concrete Mix',
              category: 'Materials',
              quantityOnHand: 28,
              reorderPoint: 50,
              belowReorderPoint: true,
            },
          ],
        },
        upcomingTasks: [
          {
            id: '1',
            name: 'Install electrical systems',
            status: 'NotStarted',
            priority: 'High',
            dueDate: '2025-02-15',
            assignedTo: ['John Smith'],
          },
          {
            id: '2',
            name: 'Install plumbing',
            status: 'NotStarted',
            priority: 'High',
            dueDate: '2025-02-20',
            assignedTo: ['Jane Doe'],
          },
        ],
        blockedTasks: [
          {
            id: '3',
            name: 'Pour foundation',
            status: 'Blocked',
            priority: 'Critical',
            assignedTo: ['Mike Johnson'],
          },
          {
            id: '4',
            name: 'Install HVAC',
            status: 'Blocked',
            priority: 'High',
            assignedTo: ['Sarah Williams'],
          },
        ],
      }
      setDashboard(mockDashboard)
    } catch (error) {
      console.error('Error loading dashboard:', error)
    } finally {
      setLoading(false)
    }
  }

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
      minimumFractionDigits: 0,
    }).format(value)
  }

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    })
  }

  if (loading) {
    return (
      <Container maxWidth="xl" sx={{ mt: 4 }}>
        <LinearProgress />
      </Container>
    )
  }

  if (!dashboard) {
    return (
      <Container maxWidth="xl" sx={{ mt: 4 }}>
        <Alert severity="error">Failed to load dashboard data</Alert>
      </Container>
    )
  }

  const { progress, costAnalysis, resourceUtilization, upcomingTasks, blockedTasks } = dashboard

  // Prepare data for charts
  const taskStatusData = [
    { name: 'Completed', value: progress.tasksCompleted, color: '#00C49F' },
    { name: 'In Progress', value: progress.tasksInProgress, color: '#0088FE' },
    { name: 'Blocked', value: progress.tasksBlocked, color: '#FF8042' },
    {
      name: 'Not Started',
      value: progress.tasksTotal - progress.tasksCompleted - progress.tasksInProgress - progress.tasksBlocked,
      color: '#CCCCCC',
    },
  ]

  const costCategoryData = Object.entries(costAnalysis.costsByCategory).map(([name, value]) => ({
    name,
    value,
  }))

  const resourceCostData = [
    { name: 'Labor', value: resourceUtilization.laborCosts },
    { name: 'Materials', value: resourceUtilization.materialCosts },
    { name: 'Equipment', value: resourceUtilization.equipmentCosts },
  ]

  const budgetData = [
    {
      name: 'Budget vs Actual',
      Budget: costAnalysis.budget,
      Actual: costAnalysis.actualCost,
      Forecasted: costAnalysis.forecastedTotal,
    },
  ]

  return (
    <Container maxWidth="xl" sx={{ mt: 4, mb: 4 }}>
      {/* Header */}
      <Box sx={{ display: 'flex', alignItems: 'center', mb: 3 }}>
        <Button startIcon={<ArrowBackIcon />} onClick={() => navigate('/projects')} sx={{ mr: 2 }}>
          Back to Projects
        </Button>
        <Box>
          <Typography variant="h4" gutterBottom>
            {progress.projectName}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Project Dashboard - Real-time metrics and analytics
          </Typography>
        </Box>
      </Box>

      {/* Key Metrics Row */}
      <Grid container spacing={3} sx={{ mb: 3 }}>
        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Typography color="text.secondary" gutterBottom variant="body2">
                Completion
              </Typography>
              <Typography variant="h4" sx={{ mb: 1 }}>
                {progress.completionPercentage}%
              </Typography>
              <LinearProgress variant="determinate" value={progress.completionPercentage} sx={{ height: 8, borderRadius: 1 }} />
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Typography color="text.secondary" gutterBottom variant="body2">
                Schedule Status
              </Typography>
              <Box sx={{ display: 'flex', alignItems: 'center' }}>
                {progress.onSchedule ? (
                  <>
                    <CheckCircleIcon color="success" sx={{ mr: 1 }} />
                    <Typography variant="h6" color="success.main">
                      On Track
                    </Typography>
                  </>
                ) : (
                  <>
                    <WarningIcon color="warning" sx={{ mr: 1 }} />
                    <Typography variant="h6" color="warning.main">
                      Behind
                    </Typography>
                  </>
                )}
              </Box>
              <Typography variant="caption" color="text.secondary">
                {Math.abs(progress.scheduleVarianceDays)} days {progress.scheduleVarianceDays < 0 ? 'ahead' : 'behind'}
              </Typography>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Typography color="text.secondary" gutterBottom variant="body2">
                Budget Status
              </Typography>
              <Box sx={{ display: 'flex', alignItems: 'center' }}>
                {!costAnalysis.isOverBudget ? (
                  <>
                    <TrendingDownIcon color="success" sx={{ mr: 1 }} />
                    <Typography variant="h6" color="success.main">
                      Under Budget
                    </Typography>
                  </>
                ) : (
                  <>
                    <TrendingUpIcon color="error" sx={{ mr: 1 }} />
                    <Typography variant="h6" color="error.main">
                      Over Budget
                    </Typography>
                  </>
                )}
              </Box>
              <Typography variant="caption" color="text.secondary">
                {formatCurrency(Math.abs(costAnalysis.variance))} ({Math.abs(costAnalysis.variancePercent).toFixed(1)}%)
              </Typography>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Typography color="text.secondary" gutterBottom variant="body2">
                Cost Performance Index
              </Typography>
              <Typography variant="h4">{costAnalysis.cpi?.toFixed(2) || 'N/A'}</Typography>
              <Typography variant="caption" color="text.secondary">
                {(costAnalysis.cpi || 0) > 1 ? 'Under budget efficiency' : 'Over budget efficiency'}
              </Typography>
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      {/* Charts Row 1 - Progress and Cost */}
      <Grid container spacing={3} sx={{ mb: 3 }}>
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Task Status Distribution
              </Typography>
              <ResponsiveContainer width="100%" height={300}>
                <PieChart>
                  <Pie data={taskStatusData} dataKey="value" nameKey="name" cx="50%" cy="50%" outerRadius={100} label>
                    {taskStatusData.map((entry, index) => (
                      <Cell key={`cell-${index}`} fill={entry.color} />
                    ))}
                  </Pie>
                  <Tooltip formatter={(value) => `${value} tasks`} />
                  <Legend />
                </PieChart>
              </ResponsiveContainer>
              <Box sx={{ mt: 2, display: 'flex', justifyContent: 'space-around' }}>
                <Box sx={{ textAlign: 'center' }}>
                  <Typography variant="h5">{progress.tasksCompleted}</Typography>
                  <Typography variant="caption" color="text.secondary">
                    Completed
                  </Typography>
                </Box>
                <Box sx={{ textAlign: 'center' }}>
                  <Typography variant="h5">{progress.tasksInProgress}</Typography>
                  <Typography variant="caption" color="text.secondary">
                    In Progress
                  </Typography>
                </Box>
                <Box sx={{ textAlign: 'center' }}>
                  <Typography variant="h5">{progress.tasksBlocked}</Typography>
                  <Typography variant="caption" color="text.secondary">
                    Blocked
                  </Typography>
                </Box>
              </Box>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Budget Overview
              </Typography>
              <ResponsiveContainer width="100%" height={300}>
                <BarChart data={budgetData}>
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis dataKey="name" />
                  <YAxis tickFormatter={(value) => `${(value / 1000000).toFixed(1)}M`} />
                  <Tooltip formatter={(value: number) => formatCurrency(value)} />
                  <Legend />
                  <Bar dataKey="Budget" fill="#8884d8" />
                  <Bar dataKey="Actual" fill="#82ca9d" />
                  <Bar dataKey="Forecasted" fill="#ffc658" />
                </BarChart>
              </ResponsiveContainer>
              <Box sx={{ mt: 2 }}>
                <Grid container spacing={2}>
                  <Grid item xs={4}>
                    <Typography variant="body2" color="text.secondary">
                      Budget
                    </Typography>
                    <Typography variant="h6">{formatCurrency(costAnalysis.budget)}</Typography>
                  </Grid>
                  <Grid item xs={4}>
                    <Typography variant="body2" color="text.secondary">
                      Actual Cost
                    </Typography>
                    <Typography variant="h6">{formatCurrency(costAnalysis.actualCost)}</Typography>
                  </Grid>
                  <Grid item xs={4}>
                    <Typography variant="body2" color="text.secondary">
                      Forecasted
                    </Typography>
                    <Typography variant="h6">{formatCurrency(costAnalysis.forecastedTotal)}</Typography>
                  </Grid>
                </Grid>
              </Box>
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      {/* Charts Row 2 - Cost Breakdown and Resource Costs */}
      <Grid container spacing={3} sx={{ mb: 3 }}>
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Cost by Category
              </Typography>
              <ResponsiveContainer width="100%" height={300}>
                <BarChart data={costCategoryData} layout="vertical">
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis type="number" tickFormatter={(value) => `${(value / 1000).toFixed(0)}K`} />
                  <YAxis dataKey="name" type="category" width={100} />
                  <Tooltip formatter={(value: number) => formatCurrency(value)} />
                  <Bar dataKey="value" fill="#8884d8">
                    {costCategoryData.map((entry, index) => (
                      <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                    ))}
                  </Bar>
                </BarChart>
              </ResponsiveContainer>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Resource Costs Breakdown
              </Typography>
              <ResponsiveContainer width="100%" height={300}>
                <PieChart>
                  <Pie data={resourceCostData} dataKey="value" nameKey="name" cx="50%" cy="50%" outerRadius={100} label>
                    {resourceCostData.map((entry, index) => (
                      <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                    ))}
                  </Pie>
                  <Tooltip formatter={(value: number) => formatCurrency(value)} />
                  <Legend />
                </PieChart>
              </ResponsiveContainer>
              <Box sx={{ mt: 2 }}>
                <Typography variant="body2" color="text.secondary">
                  Total Labor Hours
                </Typography>
                <Typography variant="h6">{resourceUtilization.totalLaborHours.toLocaleString()}</Typography>
              </Box>
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      {/* Tasks and Alerts Row */}
      <Grid container spacing={3}>
        <Grid item xs={12} md={4}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Upcoming Tasks
              </Typography>
              {upcomingTasks.length > 0 ? (
                <List>
                  {upcomingTasks.map((task, index) => (
                    <Box key={task.id}>
                      <ListItem disablePadding sx={{ py: 1 }}>
                        <ListItemText
                          primary={task.name}
                          secondary={
                            <>
                              <Chip label={task.priority} size="small" sx={{ mr: 1, mt: 0.5 }} />
                              {task.dueDate && (
                                <Typography variant="caption" component="span">
                                  Due: {formatDate(task.dueDate)}
                                </Typography>
                              )}
                              <br />
                              <Typography variant="caption" component="span">
                                Assigned: {task.assignedTo.join(', ')}
                              </Typography>
                            </>
                          }
                        />
                      </ListItem>
                      {index < upcomingTasks.length - 1 && <Divider />}
                    </Box>
                  ))}
                </List>
              ) : (
                <Typography variant="body2" color="text.secondary">
                  No upcoming tasks
                </Typography>
              )}
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} md={4}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom sx={{ display: 'flex', alignItems: 'center' }}>
                <ErrorIcon color="error" sx={{ mr: 1 }} />
                Blocked Tasks
              </Typography>
              {blockedTasks.length > 0 ? (
                <List>
                  {blockedTasks.map((task, index) => (
                    <Box key={task.id}>
                      <ListItem disablePadding sx={{ py: 1 }}>
                        <ListItemText
                          primary={task.name}
                          secondary={
                            <>
                              <Chip label={task.priority} color="error" size="small" sx={{ mr: 1, mt: 0.5 }} />
                              <br />
                              <Typography variant="caption" component="span">
                                Assigned: {task.assignedTo.join(', ')}
                              </Typography>
                            </>
                          }
                        />
                      </ListItem>
                      {index < blockedTasks.length - 1 && <Divider />}
                    </Box>
                  ))}
                </List>
              ) : (
                <Typography variant="body2" color="text.secondary">
                  No blocked tasks
                </Typography>
              )}
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} md={4}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom sx={{ display: 'flex', alignItems: 'center' }}>
                <WarningIcon color="warning" sx={{ mr: 1 }} />
                Low Inventory Alerts
              </Typography>
              {resourceUtilization.lowInventoryItems.length > 0 ? (
                <List>
                  {resourceUtilization.lowInventoryItems.map((item, index) => (
                    <Box key={item.id}>
                      <ListItem disablePadding sx={{ py: 1 }}>
                        <ListItemText
                          primary={item.name}
                          secondary={
                            <>
                              <Typography variant="caption" component="span" color="error">
                                Stock: {item.quantityOnHand} / Reorder: {item.reorderPoint}
                              </Typography>
                              <br />
                              <Typography variant="caption" component="span" color="text.secondary">
                                {item.category}
                              </Typography>
                            </>
                          }
                        />
                      </ListItem>
                      {index < resourceUtilization.lowInventoryItems.length - 1 && <Divider />}
                    </Box>
                  ))}
                </List>
              ) : (
                <Typography variant="body2" color="text.secondary">
                  All inventory levels adequate
                </Typography>
              )}
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      {/* Estimated Completion */}
      <Paper sx={{ p: 3, mt: 3, bgcolor: 'primary.light' }}>
        <Grid container spacing={2} alignItems="center">
          <Grid item xs={12} md={6}>
            <Typography variant="h6" color="primary.contrastText">
              Estimated Project Completion
            </Typography>
            <Typography variant="body2" color="primary.contrastText" sx={{ opacity: 0.9 }}>
              Based on current progress and performance metrics
            </Typography>
          </Grid>
          <Grid item xs={12} md={6} sx={{ textAlign: { xs: 'left', md: 'right' } }}>
            <Typography variant="h4" color="primary.contrastText">
              {formatDate(progress.estimatedCompletionDate)}
            </Typography>
            <Typography variant="body2" color="primary.contrastText" sx={{ opacity: 0.9 }}>
              Remaining: {formatCurrency(costAnalysis.estimatedToComplete)}
            </Typography>
          </Grid>
        </Grid>
      </Paper>
    </Container>
  )
}
