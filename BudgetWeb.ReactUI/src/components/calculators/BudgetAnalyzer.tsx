import { useState } from 'react'
import {
  Box,
  Grid,
  TextField,
  Button,
  Card,
  CardContent,
  Typography,
  Alert,
  Divider,
  IconButton,
  Chip,
  List,
  ListItem,
  ListItemText,
} from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import DeleteIcon from '@mui/icons-material/Delete'
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts'

interface BudgetCategory {
  id: number
  name: string
  budgeted: string
  actual: string
}

interface CategoryAnalysis {
  name: string
  budgeted: number
  actual: number
  variance: number
  variancePercent: number
  status: 'overspent' | 'underspent' | 'on-track'
}

interface BudgetResult {
  totalBudgeted: number
  totalActual: number
  totalVariance: number
  categories: CategoryAnalysis[]
  overspentCategories: CategoryAnalysis[]
  underspentCategories: CategoryAnalysis[]
  onTrackCategories: CategoryAnalysis[]
}

export default function BudgetAnalyzer() {
  const [categories, setCategories] = useState<BudgetCategory[]>([
    { id: 1, name: 'Housing', budgeted: '1500', actual: '1500' },
    { id: 2, name: 'Food', budgeted: '600', actual: '650' },
    { id: 3, name: 'Transportation', budgeted: '400', actual: '380' },
  ])
  const [nextId, setNextId] = useState(4)
  const [result, setResult] = useState<BudgetResult | null>(null)
  const [error, setError] = useState('')

  const addCategory = () => {
    setCategories([
      ...categories,
      {
        id: nextId,
        name: '',
        budgeted: '0',
        actual: '0',
      },
    ])
    setNextId(nextId + 1)
  }

  const removeCategory = (id: number) => {
    setCategories(categories.filter((cat) => cat.id !== id))
  }

  const updateCategory = (id: number, field: keyof BudgetCategory, value: string) => {
    setCategories(categories.map((cat) => (cat.id === id ? { ...cat, [field]: value } : cat)))
  }

  const analyzeBudget = () => {
    setError('')

    if (categories.length === 0) {
      setError('Please add at least one budget category')
      return
    }

    const analyses: CategoryAnalysis[] = []
    let totalBudgeted = 0
    let totalActual = 0

    for (const cat of categories) {
      if (!cat.name.trim()) {
        setError('Please provide a name for all categories')
        return
      }

      const budgeted = parseFloat(cat.budgeted)
      const actual = parseFloat(cat.actual)

      if (isNaN(budgeted) || budgeted < 0) {
        setError(`Invalid budgeted amount for category "${cat.name}"`)
        return
      }
      if (isNaN(actual) || actual < 0) {
        setError(`Invalid actual amount for category "${cat.name}"`)
        return
      }

      const variance = actual - budgeted
      const variancePercent = budgeted > 0 ? (variance / budgeted) * 100 : 0

      let status: 'overspent' | 'underspent' | 'on-track'
      if (Math.abs(variancePercent) <= 5) {
        status = 'on-track'
      } else if (variance > 0) {
        status = 'overspent'
      } else {
        status = 'underspent'
      }

      analyses.push({
        name: cat.name,
        budgeted,
        actual,
        variance,
        variancePercent,
        status,
      })

      totalBudgeted += budgeted
      totalActual += actual
    }

    const totalVariance = totalActual - totalBudgeted

    const overspent = analyses.filter((a) => a.status === 'overspent')
    const underspent = analyses.filter((a) => a.status === 'underspent')
    const onTrack = analyses.filter((a) => a.status === 'on-track')

    setResult({
      totalBudgeted,
      totalActual,
      totalVariance,
      categories: analyses,
      overspentCategories: overspent,
      underspentCategories: underspent,
      onTrackCategories: onTrack,
    })
  }

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
    }).format(value)
  }

  const getChartData = () => {
    if (!result) return []
    return result.categories.map((cat) => ({
      name: cat.name,
      Budgeted: cat.budgeted,
      Actual: cat.actual,
    }))
  }

  return (
    <Box>
      <Typography variant="h5" gutterBottom>
        Budget Analyzer
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
        Track your budget performance and identify spending patterns
      </Typography>

      <Grid container spacing={3}>
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
                <Typography variant="h6">Budget Categories</Typography>
                <Button startIcon={<AddIcon />} onClick={addCategory} size="small" variant="outlined">
                  Add Category
                </Button>
              </Box>

              <Box sx={{ maxHeight: 500, overflowY: 'auto' }}>
                {categories.map((cat) => (
                  <Card key={cat.id} sx={{ mb: 2, bgcolor: 'background.default' }}>
                    <CardContent>
                      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 1 }}>
                        <TextField
                          label="Category Name"
                          value={cat.name}
                          onChange={(e) => updateCategory(cat.id, 'name', e.target.value)}
                          size="small"
                          sx={{ flexGrow: 1, mr: 1 }}
                        />
                        <IconButton onClick={() => removeCategory(cat.id)} size="small" color="error">
                          <DeleteIcon />
                        </IconButton>
                      </Box>
                      <Grid container spacing={1}>
                        <Grid item xs={6}>
                          <TextField
                            fullWidth
                            label="Budgeted"
                            value={cat.budgeted}
                            onChange={(e) => updateCategory(cat.id, 'budgeted', e.target.value)}
                            type="number"
                            size="small"
                            InputProps={{ startAdornment: '$' }}
                          />
                        </Grid>
                        <Grid item xs={6}>
                          <TextField
                            fullWidth
                            label="Actual"
                            value={cat.actual}
                            onChange={(e) => updateCategory(cat.id, 'actual', e.target.value)}
                            type="number"
                            size="small"
                            InputProps={{ startAdornment: '$' }}
                          />
                        </Grid>
                      </Grid>
                    </CardContent>
                  </Card>
                ))}
              </Box>

              <Button variant="contained" fullWidth size="large" onClick={analyzeBudget} sx={{ mt: 2 }}>
                Analyze Budget
              </Button>

              {error && (
                <Alert severity="error" sx={{ mt: 2 }}>
                  {error}
                </Alert>
              )}
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} md={6}>
          {result && (
            <Card>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Budget Summary
                </Typography>
                <Box sx={{ mt: 2 }}>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="body2" color="text.secondary">
                      Total Budgeted:
                    </Typography>
                    <Typography variant="body2">{formatCurrency(result.totalBudgeted)}</Typography>
                  </Box>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="body2" color="text.secondary">
                      Total Actual:
                    </Typography>
                    <Typography variant="body2">{formatCurrency(result.totalActual)}</Typography>
                  </Box>
                  <Divider sx={{ my: 1 }} />
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
                    <Typography variant="body1" fontWeight="bold">
                      Total Variance:
                    </Typography>
                    <Typography variant="body1" color={result.totalVariance > 0 ? 'error.main' : 'success.main'} fontWeight="bold">
                      {result.totalVariance > 0 ? '+' : ''}
                      {formatCurrency(result.totalVariance)}
                    </Typography>
                  </Box>

                  {result.totalVariance > 0 ? (
                    <Alert severity="warning" sx={{ mb: 2 }}>
                      You're over budget by {formatCurrency(Math.abs(result.totalVariance))}
                    </Alert>
                  ) : result.totalVariance < 0 ? (
                    <Alert severity="success" sx={{ mb: 2 }}>
                      You're under budget by {formatCurrency(Math.abs(result.totalVariance))}
                    </Alert>
                  ) : (
                    <Alert severity="info" sx={{ mb: 2 }}>
                      You're exactly on budget!
                    </Alert>
                  )}

                  {result.overspentCategories.length > 0 && (
                    <>
                      <Typography variant="subtitle2" gutterBottom color="error.main" sx={{ mt: 2 }}>
                        Overspent Categories ({result.overspentCategories.length})
                      </Typography>
                      <List dense>
                        {result.overspentCategories.map((cat) => (
                          <ListItem key={cat.name} sx={{ px: 0 }}>
                            <ListItemText
                              primary={cat.name}
                              secondary={`${formatCurrency(cat.variance)} over (${cat.variancePercent.toFixed(0)}%)`}
                            />
                            <Chip label="Over" color="error" size="small" />
                          </ListItem>
                        ))}
                      </List>
                    </>
                  )}

                  {result.underspentCategories.length > 0 && (
                    <>
                      <Typography variant="subtitle2" gutterBottom color="success.main" sx={{ mt: 2 }}>
                        Underspent Categories ({result.underspentCategories.length})
                      </Typography>
                      <List dense>
                        {result.underspentCategories.map((cat) => (
                          <ListItem key={cat.name} sx={{ px: 0 }}>
                            <ListItemText
                              primary={cat.name}
                              secondary={`${formatCurrency(Math.abs(cat.variance))} under (${Math.abs(cat.variancePercent).toFixed(0)}%)`}
                            />
                            <Chip label="Under" color="success" size="small" />
                          </ListItem>
                        ))}
                      </List>
                    </>
                  )}

                  {result.onTrackCategories.length > 0 && (
                    <>
                      <Typography variant="subtitle2" gutterBottom sx={{ mt: 2 }}>
                        On-Track Categories ({result.onTrackCategories.length})
                      </Typography>
                      <List dense>
                        {result.onTrackCategories.map((cat) => (
                          <ListItem key={cat.name} sx={{ px: 0 }}>
                            <ListItemText primary={cat.name} secondary={`Within 5% of budget`} />
                            <Chip label="On Track" color="default" size="small" />
                          </ListItem>
                        ))}
                      </List>
                    </>
                  )}

                  <Divider sx={{ my: 2 }} />

                  <Typography variant="subtitle2" gutterBottom>
                    Recommendations
                  </Typography>
                  {result.overspentCategories.length > 0 && (
                    <Alert severity="info" sx={{ mb: 1 }}>
                      Review overspent categories to identify unnecessary expenses
                    </Alert>
                  )}
                  {result.underspentCategories.length > 0 && (
                    <Alert severity="info" sx={{ mb: 1 }}>
                      Consider reallocating savings from underspent categories to savings or debt payoff
                    </Alert>
                  )}
                  {result.onTrackCategories.length === result.categories.length && (
                    <Alert severity="success">Great job staying on track with all categories!</Alert>
                  )}
                </Box>
              </CardContent>
            </Card>
          )}
        </Grid>

        {result && result.categories.length > 0 && (
          <Grid item xs={12}>
            <Card>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Budget vs Actual Spending
                </Typography>
                <ResponsiveContainer width="100%" height={300}>
                  <BarChart data={getChartData()}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="name" />
                    <YAxis />
                    <Tooltip formatter={(value: number) => formatCurrency(value)} />
                    <Legend />
                    <Bar dataKey="Budgeted" fill="#1976d2" />
                    <Bar dataKey="Actual" fill="#d32f2f" />
                  </BarChart>
                </ResponsiveContainer>
              </CardContent>
            </Card>
          </Grid>
        )}
      </Grid>
    </Box>
  )
}
