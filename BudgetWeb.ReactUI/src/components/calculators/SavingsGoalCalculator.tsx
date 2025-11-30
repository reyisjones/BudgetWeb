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

interface SavingsGoalResult {
  monthsToGoal: number
  yearsToGoal: number
  totalContributions: number
  totalInterestEarned: number
  targetAmount: number
}

export default function SavingsGoalCalculator() {
  const [targetAmount, setTargetAmount] = useState('10000')
  const [currentSavings, setCurrentSavings] = useState('1000')
  const [monthlyContribution, setMonthlyContribution] = useState('500')
  const [annualReturn, setAnnualReturn] = useState('5')
  const [result, setResult] = useState<SavingsGoalResult | null>(null)
  const [error, setError] = useState('')

  const calculateGoal = () => {
    setError('')

    const target = parseFloat(targetAmount)
    const current = parseFloat(currentSavings)
    const monthly = parseFloat(monthlyContribution)
    const rate = parseFloat(annualReturn)

    if (isNaN(target) || target <= 0) {
      setError('Please enter a valid target amount')
      return
    }
    if (isNaN(current) || current < 0) {
      setError('Please enter a valid current savings amount')
      return
    }
    if (isNaN(monthly) || monthly <= 0) {
      setError('Please enter a valid monthly contribution')
      return
    }
    if (isNaN(rate) || rate < 0) {
      setError('Please enter a valid annual return rate')
      return
    }

    if (current >= target) {
      setError('You have already reached your goal!')
      return
    }

    const monthlyRate = rate / 12 / 100
    let balance = current
    let months = 0
    let totalContrib = 0
    const maxMonths = 1200 // 100 years cap

    while (balance < target && months < maxMonths) {
      const interest = balance * monthlyRate
      balance += monthly + interest
      totalContrib += monthly
      months++
    }

    if (months >= maxMonths) {
      setError('Goal will take more than 100 years to reach. Consider increasing contributions or adjusting your target.')
      return
    }

    const totalInterest = balance - current - totalContrib

    setResult({
      monthsToGoal: months,
      yearsToGoal: months / 12,
      totalContributions: totalContrib,
      totalInterestEarned: totalInterest,
      targetAmount: target,
    })
  }

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
    }).format(value)
  }

  return (
    <Box>
      <Typography variant="h5" gutterBottom>
        Savings Goal Calculator
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
        Calculate how long it will take to reach your savings goal with regular contributions
      </Typography>

      <Grid container spacing={3}>
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Goal Details
              </Typography>
              <Box component="form" noValidate sx={{ mt: 2 }}>
                <TextField
                  fullWidth
                  label="Target Savings Amount"
                  value={targetAmount}
                  onChange={(e) => setTargetAmount(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ startAdornment: '$' }}
                />
                <TextField
                  fullWidth
                  label="Current Savings"
                  value={currentSavings}
                  onChange={(e) => setCurrentSavings(e.target.value)}
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
                  label="Expected Annual Return"
                  value={annualReturn}
                  onChange={(e) => setAnnualReturn(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ endAdornment: '%' }}
                  helperText="Typical savings account: 1-2%, investments: 5-8%"
                />
                <Button variant="contained" fullWidth size="large" onClick={calculateGoal}>
                  Calculate Timeline
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
                  Timeline to Goal
                </Typography>
                <Box sx={{ mt: 2 }}>
                  <Box sx={{ textAlign: 'center', mb: 3 }}>
                    <Typography variant="h3" color="primary">
                      {result.yearsToGoal.toFixed(1)}
                    </Typography>
                    <Typography variant="body1" color="text.secondary">
                      years ({result.monthsToGoal} months)
                    </Typography>
                  </Box>
                  <Divider sx={{ my: 2 }} />
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="body2" color="text.secondary">
                      Target Amount:
                    </Typography>
                    <Typography variant="body2">{formatCurrency(result.targetAmount)}</Typography>
                  </Box>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="body2" color="text.secondary">
                      Total Contributions:
                    </Typography>
                    <Typography variant="body2">{formatCurrency(result.totalContributions)}</Typography>
                  </Box>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="body2" color="text.secondary">
                      Interest Earned:
                    </Typography>
                    <Typography variant="body2" color="success.main" fontWeight="bold">
                      {formatCurrency(result.totalInterestEarned)}
                    </Typography>
                  </Box>
                  <Alert severity="success" sx={{ mt: 3 }}>
                    Stay consistent with your {formatCurrency(parseFloat(monthlyContribution))} monthly contributions to reach your goal!
                  </Alert>
                </Box>
              </CardContent>
            </Card>
          )}
        </Grid>
      </Grid>
    </Box>
  )
}
