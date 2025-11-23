import { useState, useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import Container from '@mui/material/Container'
import Typography from '@mui/material/Typography'
import Paper from '@mui/material/Paper'
import Grid from '@mui/material/Grid'
import Button from '@mui/material/Button'
import Box from '@mui/material/Box'
import Chip from '@mui/material/Chip'
import LinearProgress from '@mui/material/LinearProgress'
import Card from '@mui/material/Card'
import CardContent from '@mui/material/CardContent'
import ArrowBackIcon from '@mui/icons-material/ArrowBack'
import EditIcon from '@mui/icons-material/Edit'
import { budgetApi, Budget } from '@/services/api'

export default function BudgetDetail() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const [budget, setBudget] = useState<Budget | null>(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    if (id) {
      fetchBudget(id)
    }
  }, [id])

  const fetchBudget = async (budgetId: string) => {
    try {
      setLoading(true)
      // const data = await budgetApi.getById(budgetId)
      // Mock data
      const mockData: Budget = {
        id: budgetId,
        name: '2025 Q1 Budget',
        description: 'First quarter operational budget covering all departments',
        totalAmount: { amount: 250000, currency: 'USD' },
        spentAmount: { amount: 175000, currency: 'USD' },
        period: 'Quarterly',
        startDate: '2025-01-01',
        endDate: '2025-03-31',
        status: 'Active',
        categories: [
          {
            id: '1',
            name: 'Payroll',
            budgetedAmount: { amount: 100000, currency: 'USD' },
            spentAmount: { amount: 75000, currency: 'USD' },
            type: 'Operational',
          },
          {
            id: '2',
            name: 'Equipment',
            budgetedAmount: { amount: 50000, currency: 'USD' },
            spentAmount: { amount: 35000, currency: 'USD' },
            type: 'Capital',
          },
          {
            id: '3',
            name: 'Marketing',
            budgetedAmount: { amount: 75000, currency: 'USD' },
            spentAmount: { amount: 50000, currency: 'USD' },
            type: 'Marketing',
          },
          {
            id: '4',
            name: 'Operations',
            budgetedAmount: { amount: 25000, currency: 'USD' },
            spentAmount: { amount: 15000, currency: 'USD' },
            type: 'Operational',
          },
        ],
      }
      setBudget(mockData)
    } catch (error) {
      console.error('Failed to fetch budget:', error)
    } finally {
      setLoading(false)
    }
  }

  if (loading) {
    return <LinearProgress />
  }

  if (!budget) {
    return <Typography>Budget not found</Typography>
  }

  const utilizationRate = (budget.spentAmount.amount / budget.totalAmount.amount) * 100

  return (
    <Container maxWidth="xl" sx={{ mt: 4, mb: 4 }}>
      <Box sx={{ display: 'flex', alignItems: 'center', mb: 3 }}>
        <Button startIcon={<ArrowBackIcon />} onClick={() => navigate('/budgets')} sx={{ mr: 2 }}>
          Back
        </Button>
        <Typography variant="h4" sx={{ flexGrow: 1 }}>
          {budget.name}
        </Typography>
        <Button variant="contained" startIcon={<EditIcon />} onClick={() => navigate(`/budgets/${id}/edit`)}>
          Edit
        </Button>
      </Box>

      <Grid container spacing={3}>
        {/* Budget Overview */}
        <Grid item xs={12} md={8}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Budget Overview
            </Typography>
            <Typography variant="body2" color="text.secondary" paragraph>
              {budget.description}
            </Typography>
            <Grid container spacing={2}>
              <Grid item xs={6}>
                <Typography variant="body2" color="text.secondary">
                  Period
                </Typography>
                <Typography variant="body1">{budget.period}</Typography>
              </Grid>
              <Grid item xs={6}>
                <Typography variant="body2" color="text.secondary">
                  Status
                </Typography>
                <Chip label={budget.status} color="success" size="small" />
              </Grid>
              <Grid item xs={6}>
                <Typography variant="body2" color="text.secondary">
                  Start Date
                </Typography>
                <Typography variant="body1">{new Date(budget.startDate).toLocaleDateString()}</Typography>
              </Grid>
              <Grid item xs={6}>
                <Typography variant="body2" color="text.secondary">
                  End Date
                </Typography>
                <Typography variant="body1">{new Date(budget.endDate).toLocaleDateString()}</Typography>
              </Grid>
            </Grid>
          </Paper>
        </Grid>

        {/* Financial Summary */}
        <Grid item xs={12} md={4}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Financial Summary
              </Typography>
              <Box sx={{ mb: 2 }}>
                <Typography variant="body2" color="text.secondary">
                  Total Budget
                </Typography>
                <Typography variant="h5">
                  ${budget.totalAmount.amount.toLocaleString()}
                </Typography>
              </Box>
              <Box sx={{ mb: 2 }}>
                <Typography variant="body2" color="text.secondary">
                  Spent
                </Typography>
                <Typography variant="h5" color="primary">
                  ${budget.spentAmount.amount.toLocaleString()}
                </Typography>
              </Box>
              <Box sx={{ mb: 2 }}>
                <Typography variant="body2" color="text.secondary">
                  Remaining
                </Typography>
                <Typography variant="h5" color="success.main">
                  ${(budget.totalAmount.amount - budget.spentAmount.amount).toLocaleString()}
                </Typography>
              </Box>
              <Box>
                <Typography variant="body2" color="text.secondary" gutterBottom>
                  Utilization: {utilizationRate.toFixed(1)}%
                </Typography>
                <LinearProgress variant="determinate" value={utilizationRate} sx={{ height: 8, borderRadius: 4 }} />
              </Box>
            </CardContent>
          </Card>
        </Grid>

        {/* Categories */}
        <Grid item xs={12}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              Budget Categories
            </Typography>
            <Grid container spacing={2}>
              {budget.categories.map((category) => {
                const categoryUtilization = (category.spentAmount.amount / category.budgetedAmount.amount) * 100
                return (
                  <Grid item xs={12} sm={6} md={3} key={category.id}>
                    <Card variant="outlined">
                      <CardContent>
                        <Typography variant="h6" gutterBottom>
                          {category.name}
                        </Typography>
                        <Typography variant="body2" color="text.secondary">
                          Type: {category.type}
                        </Typography>
                        <Box sx={{ mt: 2 }}>
                          <Typography variant="body2" color="text.secondary">
                            Budget: ${category.budgetedAmount.amount.toLocaleString()}
                          </Typography>
                          <Typography variant="body2" color="primary">
                            Spent: ${category.spentAmount.amount.toLocaleString()}
                          </Typography>
                          <Box sx={{ mt: 1 }}>
                            <Typography variant="caption" color="text.secondary">
                              {categoryUtilization.toFixed(1)}%
                            </Typography>
                            <LinearProgress
                              variant="determinate"
                              value={categoryUtilization}
                              sx={{ height: 6, borderRadius: 3 }}
                              color={categoryUtilization >= 90 ? 'error' : categoryUtilization >= 75 ? 'warning' : 'success'}
                            />
                          </Box>
                        </Box>
                      </CardContent>
                    </Card>
                  </Grid>
                )
              })}
            </Grid>
          </Paper>
        </Grid>
      </Grid>
    </Container>
  )
}
