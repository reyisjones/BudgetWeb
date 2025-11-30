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
  Chip,
} from '@mui/material'
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts'

interface RefinanceResult {
  currentMonthlyPayment: number
  newMonthlyPayment: number
  monthlySavings: number
  currentTotalInterest: number
  newTotalInterest: number
  totalInterestSavings: number
  breakEvenMonths: number
  netSavings: number
  worthwhile: boolean
}

export default function RefinanceCalculator() {
  const [currentBalance, setCurrentBalance] = useState('250000')
  const [currentRate, setCurrentRate] = useState('6.5')
  const [remainingMonths, setRemainingMonths] = useState('300')
  const [newRate, setNewRate] = useState('5.5')
  const [newTerm, setNewTerm] = useState('360')
  const [closingCosts, setClosingCosts] = useState('5000')
  const [result, setResult] = useState<RefinanceResult | null>(null)
  const [error, setError] = useState('')

  const calculatePayment = (balance: number, rate: number, months: number) => {
    const r = rate / 12 / 100
    if (r === 0) return balance / months
    const payment = (balance * r * Math.pow(1 + r, months)) / (Math.pow(1 + r, months) - 1)
    return payment
  }

  const calculateRefinance = () => {
    setError('')

    const balance = parseFloat(currentBalance)
    const curRate = parseFloat(currentRate)
    const remMonths = parseInt(remainingMonths)
    const nRate = parseFloat(newRate)
    const nTerm = parseInt(newTerm)
    const costs = parseFloat(closingCosts)

    if (isNaN(balance) || balance <= 0) {
      setError('Please enter a valid current loan balance')
      return
    }
    if (isNaN(curRate) || curRate < 0) {
      setError('Please enter a valid current interest rate')
      return
    }
    if (isNaN(remMonths) || remMonths <= 0) {
      setError('Please enter a valid remaining term')
      return
    }
    if (isNaN(nRate) || nRate < 0) {
      setError('Please enter a valid new interest rate')
      return
    }
    if (isNaN(nTerm) || nTerm <= 0) {
      setError('Please enter a valid new loan term')
      return
    }
    if (isNaN(costs) || costs < 0) {
      setError('Please enter valid closing costs')
      return
    }

    const currentMonthlyPayment = calculatePayment(balance, curRate, remMonths)
    const newMonthlyPayment = calculatePayment(balance, nRate, nTerm)
    const monthlySavings = currentMonthlyPayment - newMonthlyPayment

    const currentTotalPaid = currentMonthlyPayment * remMonths
    const currentTotalInterest = currentTotalPaid - balance

    const newTotalPaid = newMonthlyPayment * nTerm
    const newTotalInterest = newTotalPaid - balance

    const totalInterestSavings = currentTotalInterest - newTotalInterest

    const breakEvenMonths = monthlySavings > 0 ? Math.ceil(costs / monthlySavings) : 0
    const netSavings = totalInterestSavings - costs

    const worthwhile = netSavings > 0 && breakEvenMonths > 0 && breakEvenMonths < nTerm

    setResult({
      currentMonthlyPayment,
      newMonthlyPayment,
      monthlySavings,
      currentTotalInterest,
      newTotalInterest,
      totalInterestSavings,
      breakEvenMonths,
      netSavings,
      worthwhile,
    })
  }

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
    }).format(value)
  }

  const getComparisonData = () => {
    if (!result) return []
    return [
      {
        name: 'Monthly Payment',
        Current: result.currentMonthlyPayment,
        New: result.newMonthlyPayment,
      },
      {
        name: 'Total Interest',
        Current: result.currentTotalInterest,
        New: result.newTotalInterest,
      },
    ]
  }

  return (
    <Box>
      <Typography variant="h5" gutterBottom>
        Refinance Calculator
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
        Analyze whether refinancing your loan makes financial sense
      </Typography>

      <Grid container spacing={3}>
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Current Loan
              </Typography>
              <Box component="form" noValidate sx={{ mt: 2 }}>
                <TextField
                  fullWidth
                  label="Current Balance"
                  value={currentBalance}
                  onChange={(e) => setCurrentBalance(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ startAdornment: '$' }}
                />
                <TextField
                  fullWidth
                  label="Current Interest Rate"
                  value={currentRate}
                  onChange={(e) => setCurrentRate(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ endAdornment: '%' }}
                />
                <TextField
                  fullWidth
                  label="Remaining Months"
                  value={remainingMonths}
                  onChange={(e) => setRemainingMonths(e.target.value)}
                  type="number"
                  sx={{ mb: 3 }}
                  InputProps={{ endAdornment: 'months' }}
                />
                <Divider sx={{ my: 2 }} />
                <Typography variant="h6" gutterBottom>
                  New Loan
                </Typography>
                <TextField
                  fullWidth
                  label="New Interest Rate"
                  value={newRate}
                  onChange={(e) => setNewRate(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ endAdornment: '%' }}
                />
                <TextField
                  fullWidth
                  label="New Loan Term"
                  value={newTerm}
                  onChange={(e) => setNewTerm(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ endAdornment: 'months' }}
                />
                <TextField
                  fullWidth
                  label="Closing Costs"
                  value={closingCosts}
                  onChange={(e) => setClosingCosts(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ startAdornment: '$' }}
                />
                <Button variant="contained" fullWidth size="large" onClick={calculateRefinance}>
                  Compare Options
                </Button>
              </Box>
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
                  Refinance Analysis
                </Typography>
                <Box sx={{ mt: 2 }}>
                  <Box sx={{ textAlign: 'center', mb: 3 }}>
                    {result.worthwhile ? (
                      <Chip label="Refinancing Recommended" color="success" size="large" sx={{ mb: 2 }} />
                    ) : (
                      <Chip label="Refinancing Not Recommended" color="error" size="large" sx={{ mb: 2 }} />
                    )}
                  </Box>

                  <Typography variant="subtitle2" gutterBottom>
                    Monthly Payment Comparison
                  </Typography>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="body2" color="text.secondary">
                      Current Payment:
                    </Typography>
                    <Typography variant="body2">{formatCurrency(result.currentMonthlyPayment)}</Typography>
                  </Box>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="body2" color="text.secondary">
                      New Payment:
                    </Typography>
                    <Typography variant="body2">{formatCurrency(result.newMonthlyPayment)}</Typography>
                  </Box>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
                    <Typography variant="body2" fontWeight="bold">
                      Monthly Savings:
                    </Typography>
                    <Typography variant="body2" color={result.monthlySavings > 0 ? 'success.main' : 'error.main'} fontWeight="bold">
                      {formatCurrency(result.monthlySavings)}
                    </Typography>
                  </Box>

                  <Divider sx={{ my: 2 }} />

                  <Typography variant="subtitle2" gutterBottom>
                    Long-term Impact
                  </Typography>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="body2" color="text.secondary">
                      Total Interest Savings:
                    </Typography>
                    <Typography variant="body2" color="success.main">
                      {formatCurrency(result.totalInterestSavings)}
                    </Typography>
                  </Box>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="body2" color="text.secondary">
                      Closing Costs:
                    </Typography>
                    <Typography variant="body2" color="error.main">
                      {formatCurrency(parseFloat(closingCosts))}
                    </Typography>
                  </Box>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="body2" fontWeight="bold">
                      Net Savings:
                    </Typography>
                    <Typography variant="body2" color={result.netSavings > 0 ? 'success.main' : 'error.main'} fontWeight="bold">
                      {formatCurrency(result.netSavings)}
                    </Typography>
                  </Box>

                  <Divider sx={{ my: 2 }} />

                  <Typography variant="subtitle2" gutterBottom>
                    Break-Even Analysis
                  </Typography>
                  <Box sx={{ textAlign: 'center', py: 2, bgcolor: 'primary.light', borderRadius: 1 }}>
                    <Typography variant="body2" color="primary.contrastText">
                      Break-even in <strong>{result.breakEvenMonths}</strong> months ({(result.breakEvenMonths / 12).toFixed(1)} years)
                    </Typography>
                  </Box>

                  {result.worthwhile ? (
                    <Alert severity="success" sx={{ mt: 3 }}>
                      Refinancing appears beneficial. You'll save {formatCurrency(result.monthlySavings)}/month and recoup closing costs in{' '}
                      {result.breakEvenMonths} months.
                    </Alert>
                  ) : (
                    <Alert severity="warning" sx={{ mt: 3 }}>
                      Refinancing may not be worth it. Consider if you'll stay in the loan long enough to recoup the closing costs.
                    </Alert>
                  )}
                </Box>
              </CardContent>
            </Card>
          )}
        </Grid>

        {result && (
          <Grid item xs={12}>
            <Card>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Cost Comparison
                </Typography>
                <ResponsiveContainer width="100%" height={300}>
                  <BarChart data={getComparisonData()}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="name" />
                    <YAxis />
                    <Tooltip formatter={(value: number) => formatCurrency(value)} />
                    <Legend />
                    <Bar dataKey="Current" fill="#d32f2f" />
                    <Bar dataKey="New" fill="#2e7d32" />
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
