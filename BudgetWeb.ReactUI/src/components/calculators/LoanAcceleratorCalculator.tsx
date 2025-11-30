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
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts'

interface AccelerationResult {
  regularPayments: number
  acceleratedPayments: number
  paymentsSaved: number
  regularInterest: number
  acceleratedInterest: number
  interestSaved: number
  yearsReduced: number
}

export default function LoanAcceleratorCalculator() {
  const [principal, setPrincipal] = useState('200000')
  const [interestRate, setInterestRate] = useState('4.5')
  const [termYears, setTermYears] = useState('30')
  const [extraPayment, setExtraPayment] = useState('200')
  const [result, setResult] = useState<AccelerationResult | null>(null)
  const [error, setError] = useState('')

  const calculateAcceleration = () => {
    setError('')

    const p = parseFloat(principal)
    const r = parseFloat(interestRate)
    const y = parseInt(termYears)
    const extra = parseFloat(extraPayment)

    if (isNaN(p) || p <= 0) {
      setError('Please enter a valid loan amount')
      return
    }
    if (isNaN(r) || r < 0) {
      setError('Please enter a valid interest rate')
      return
    }
    if (isNaN(y) || y <= 0) {
      setError('Please enter a valid term')
      return
    }
    if (isNaN(extra) || extra < 0) {
      setError('Please enter a valid extra payment amount')
      return
    }

    const monthlyRate = r / 12 / 100
    const totalPayments = y * 12

    // Calculate regular payment
    let regularPayment: number
    if (monthlyRate === 0) {
      regularPayment = p / totalPayments
    } else {
      regularPayment = p * (monthlyRate * Math.pow(1 + monthlyRate, totalPayments)) / (Math.pow(1 + monthlyRate, totalPayments) - 1)
    }

    // Calculate regular scenario
    let regularBalance = p
    let regularInterestTotal = 0
    for (let i = 0; i < totalPayments; i++) {
      const interest = regularBalance * monthlyRate
      regularInterestTotal += interest
      regularBalance -= regularPayment - interest
    }

    // Calculate accelerated scenario
    let acceleratedBalance = p
    let acceleratedInterestTotal = 0
    let acceleratedPayments = 0
    const acceleratedPaymentAmount = regularPayment + extra

    while (acceleratedBalance > 0 && acceleratedPayments < totalPayments * 2) {
      const interest = acceleratedBalance * monthlyRate
      const principal = Math.min(acceleratedPaymentAmount - interest, acceleratedBalance)
      acceleratedInterestTotal += interest
      acceleratedBalance -= principal
      acceleratedPayments++
    }

    const paymentsSaved = totalPayments - acceleratedPayments
    const interestSaved = regularInterestTotal - acceleratedInterestTotal
    const yearsReduced = paymentsSaved / 12

    setResult({
      regularPayments: totalPayments,
      acceleratedPayments,
      paymentsSaved,
      regularInterest: regularInterestTotal,
      acceleratedInterest: acceleratedInterestTotal,
      interestSaved,
      yearsReduced,
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
    return [
      {
        name: 'Regular',
        Payments: result.regularPayments,
        Interest: result.regularInterest,
      },
      {
        name: 'Accelerated',
        Payments: result.acceleratedPayments,
        Interest: result.acceleratedInterest,
      },
    ]
  }

  return (
    <Box>
      <Typography variant="h5" gutterBottom>
        Loan Accelerator Calculator
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
        See how extra payments can dramatically reduce your loan term and save on interest
      </Typography>

      <Grid container spacing={3}>
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Loan Details
              </Typography>
              <Box component="form" noValidate sx={{ mt: 2 }}>
                <TextField
                  fullWidth
                  label="Current Loan Balance"
                  value={principal}
                  onChange={(e) => setPrincipal(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ startAdornment: '$' }}
                />
                <TextField
                  fullWidth
                  label="Interest Rate"
                  value={interestRate}
                  onChange={(e) => setInterestRate(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ endAdornment: '%' }}
                />
                <TextField
                  fullWidth
                  label="Remaining Term"
                  value={termYears}
                  onChange={(e) => setTermYears(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ endAdornment: 'years' }}
                />
                <TextField
                  fullWidth
                  label="Extra Monthly Payment"
                  value={extraPayment}
                  onChange={(e) => setExtraPayment(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ startAdornment: '$' }}
                  helperText="Additional amount to pay each month"
                />
                <Button variant="contained" fullWidth size="large" onClick={calculateAcceleration}>
                  Calculate Savings
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
                  Acceleration Impact
                </Typography>
                <Box sx={{ mt: 2 }}>
                  <Alert severity="success" sx={{ mb: 2 }}>
                    You'll save <strong>{formatCurrency(result.interestSaved)}</strong> and pay off your loan{' '}
                    <strong>{result.yearsReduced.toFixed(1)} years</strong> earlier!
                  </Alert>
                  <Divider sx={{ my: 2 }} />
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="body2" color="text.secondary">
                      Regular Payments:
                    </Typography>
                    <Typography variant="body2">{result.regularPayments} months</Typography>
                  </Box>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="body2" color="text.secondary">
                      Accelerated Payments:
                    </Typography>
                    <Typography variant="body2">{result.acceleratedPayments} months</Typography>
                  </Box>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="body2" color="text.secondary">
                      Payments Saved:
                    </Typography>
                    <Typography variant="body2" color="success.main" fontWeight="bold">
                      {result.paymentsSaved} months
                    </Typography>
                  </Box>
                  <Divider sx={{ my: 2 }} />
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="body2" color="text.secondary">
                      Regular Interest:
                    </Typography>
                    <Typography variant="body2">{formatCurrency(result.regularInterest)}</Typography>
                  </Box>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="body2" color="text.secondary">
                      Accelerated Interest:
                    </Typography>
                    <Typography variant="body2">{formatCurrency(result.acceleratedInterest)}</Typography>
                  </Box>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="body2" color="text.secondary">
                      Interest Saved:
                    </Typography>
                    <Typography variant="body2" color="success.main" fontWeight="bold">
                      {formatCurrency(result.interestSaved)}
                    </Typography>
                  </Box>
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
                  Comparison Chart
                </Typography>
                <ResponsiveContainer width="100%" height={300}>
                  <BarChart data={getChartData()}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="name" />
                    <YAxis yAxisId="left" orientation="left" stroke="#8884d8" label={{ value: 'Payments', angle: -90, position: 'insideLeft' }} />
                    <YAxis yAxisId="right" orientation="right" stroke="#82ca9d" label={{ value: 'Interest ($)', angle: 90, position: 'insideRight' }} />
                    <Tooltip formatter={(value: number, name: string) => (name === 'Interest' ? formatCurrency(value) : value)} />
                    <Legend />
                    <Bar yAxisId="left" dataKey="Payments" fill="#8884d8" />
                    <Bar yAxisId="right" dataKey="Interest" fill="#82ca9d" />
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
