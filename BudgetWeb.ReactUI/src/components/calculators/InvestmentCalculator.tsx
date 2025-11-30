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
} from '@mui/material'
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts'

interface InvestmentResult {
  futureValue: number
  totalContributions: number
  totalGains: number
  years: number
}

export default function InvestmentCalculator() {
  const [initialInvestment, setInitialInvestment] = useState('10000')
  const [monthlyContribution, setMonthlyContribution] = useState('500')
  const [years, setYears] = useState('10')
  const [annualReturn, setAnnualReturn] = useState('7')
  const [result, setResult] = useState<InvestmentResult | null>(null)
  const [error, setError] = useState('')

  const calculateInvestment = () => {
    setError('')

    const initial = parseFloat(initialInvestment)
    const monthly = parseFloat(monthlyContribution)
    const y = parseInt(years)
    const rate = parseFloat(annualReturn)

    if (isNaN(initial) || initial < 0) {
      setError('Please enter a valid initial investment')
      return
    }
    if (isNaN(monthly) || monthly < 0) {
      setError('Please enter a valid monthly contribution')
      return
    }
    if (isNaN(y) || y <= 0) {
      setError('Please enter a valid time period')
      return
    }
    if (isNaN(rate) || rate < 0) {
      setError('Please enter a valid annual return rate')
      return
    }

    const monthlyRate = rate / 12 / 100
    const numMonths = y * 12
    let balance = initial
    let totalContrib = 0

    for (let i = 0; i < numMonths; i++) {
      const interest = balance * monthlyRate
      balance += monthly + interest
      totalContrib += monthly
    }

    const futureValue = balance
    const totalGains = futureValue - initial - totalContrib

    setResult({
      futureValue,
      totalContributions: totalContrib,
      totalGains,
      years: y,
    })
  }

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
    }).format(value)
  }

  const generateProjectionData = () => {
    if (!result) return []

    const initial = parseFloat(initialInvestment)
    const monthly = parseFloat(monthlyContribution)
    const rate = parseFloat(annualReturn) / 12 / 100
    const data = []
    let balance = initial

    for (let year = 0; year <= result.years; year++) {
      data.push({
        year,
        value: balance,
      })

      // Advance one year
      for (let month = 0; month < 12; month++) {
        const interest = balance * rate
        balance += monthly + interest
      }
    }

    return data
  }

  return (
    <Box>
      <Typography variant="h5" gutterBottom>
        Investment Growth Calculator
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
        Project your investment growth with compound returns and regular contributions
      </Typography>

      <Grid container spacing={3}>
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Investment Details
              </Typography>
              <Box component="form" noValidate sx={{ mt: 2 }}>
                <TextField
                  fullWidth
                  label="Initial Investment"
                  value={initialInvestment}
                  onChange={(e) => setInitialInvestment(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ startAdornment: '$' }}
                />
                <TextField
                  fullWidth
                  label="Monthly Contribution"
                  value={monthlyContribution}
                  onChange={(e) => setMonthlyContribution(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ startAdornment: '$' }}
                />
                <TextField
                  fullWidth
                  label="Time Period"
                  value={years}
                  onChange={(e) => setYears(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ endAdornment: 'years' }}
                />
                <TextField
                  fullWidth
                  label="Expected Annual Return"
                  value={annualReturn}
                  onChange={(e) => setAnnualReturn(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ endAdornment: '%' }}
                  helperText="Historical S&P 500 average: ~10%, Conservative: 5-7%"
                />
                <Button variant="contained" fullWidth size="large" onClick={calculateInvestment}>
                  Calculate Growth
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
                  Projected Value
                </Typography>
                <Box sx={{ mt: 2 }}>
                  <Box sx={{ textAlign: 'center', mb: 3 }}>
                    <Typography variant="h3" color="primary">
                      {formatCurrency(result.futureValue)}
                    </Typography>
                    <Typography variant="body1" color="text.secondary">
                      in {result.years} years
                    </Typography>
                  </Box>
                  <Divider sx={{ my: 2 }} />
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="body2" color="text.secondary">
                      Initial Investment:
                    </Typography>
                    <Typography variant="body2">{formatCurrency(parseFloat(initialInvestment))}</Typography>
                  </Box>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="body2" color="text.secondary">
                      Total Contributions:
                    </Typography>
                    <Typography variant="body2">{formatCurrency(result.totalContributions)}</Typography>
                  </Box>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="body2" color="text.secondary">
                      Investment Gains:
                    </Typography>
                    <Typography variant="body2" color="success.main" fontWeight="bold">
                      {formatCurrency(result.totalGains)}
                    </Typography>
                  </Box>
                  <Alert severity="info" sx={{ mt: 3 }}>
                    <Typography variant="caption">
                      Disclaimer: This projection assumes consistent returns. Actual returns will vary with market conditions.
                    </Typography>
                  </Alert>
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
                  Growth Projection
                </Typography>
                <ResponsiveContainer width="100%" height={300}>
                  <LineChart data={generateProjectionData()}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="year" label={{ value: 'Year', position: 'insideBottom', offset: -5 }} />
                    <YAxis label={{ value: 'Portfolio Value ($)', angle: -90, position: 'insideLeft' }} />
                    <Tooltip formatter={(value: number) => formatCurrency(value)} />
                    <Legend />
                    <Line type="monotone" dataKey="value" stroke="#1976d2" strokeWidth={2} name="Portfolio Value" />
                  </LineChart>
                </ResponsiveContainer>
              </CardContent>
            </Card>
          </Grid>
        )}
      </Grid>
    </Box>
  )
}
