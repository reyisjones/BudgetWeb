import { useState } from 'react'
import {
  Box,
  Grid,
  TextField,
  Button,
  Card,
  CardContent,
  Typography,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Alert,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Divider,
} from '@mui/material'
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts'

interface MortgageResult {
  loanAmount: number
  interestRate: number
  termYears: number
  paymentFrequency: string
  regularPayment: number
  totalPayments: number
  totalInterest: number
  totalPaid: number
}

export default function MortgageCalculator() {
  const [principal, setPrincipal] = useState('300000')
  const [interestRate, setInterestRate] = useState('4.5')
  const [termYears, setTermYears] = useState('30')
  const [frequency, setFrequency] = useState('Monthly')
  const [extraPayment, setExtraPayment] = useState('0')
  const [result, setResult] = useState<MortgageResult | null>(null)
  const [error, setError] = useState('')

  const calculateMortgage = () => {
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

    // Calculate based on frequency
    let periodsPerYear = 12
    if (frequency === 'BiWeekly') periodsPerYear = 26
    if (frequency === 'Weekly') periodsPerYear = 52

    const totalPeriods = y * periodsPerYear
    const periodicRate = r / 100 / periodsPerYear

    let payment: number
    if (periodicRate === 0) {
      payment = p / totalPeriods
    } else {
      payment = p * (periodicRate * Math.pow(1 + periodicRate, totalPeriods)) / (Math.pow(1 + periodicRate, totalPeriods) - 1)
    }

    // Calculate total interest with extra payments
    let balance = p
    let totalInterestPaid = 0
    let paymentCount = 0
    const totalPaymentAmount = payment + extra

    while (balance > 0 && paymentCount < totalPeriods * 2) {
      const interestPayment = balance * periodicRate
      const principalPayment = Math.min(totalPaymentAmount - interestPayment, balance)
      balance -= principalPayment
      totalInterestPaid += interestPayment
      paymentCount++
    }

    const totalPaid = payment * paymentCount + extra * paymentCount

    setResult({
      loanAmount: p,
      interestRate: r,
      termYears: y,
      paymentFrequency: frequency,
      regularPayment: payment,
      totalPayments: paymentCount,
      totalInterest: totalInterestPaid,
      totalPaid: totalPaid,
    })
  }

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
    }).format(value)
  }

  // Generate chart data
  const generateChartData = () => {
    if (!result) return []

    const data = []
    let balance = result.loanAmount
    const extra = parseFloat(extraPayment)
    const periodsPerYear = frequency === 'Monthly' ? 12 : frequency === 'BiWeekly' ? 26 : 52
    const periodicRate = result.interestRate / 100 / periodsPerYear
    const totalPaymentAmount = result.regularPayment + extra

    for (let i = 0; i <= result.totalPayments && i <= 360; i += 12) {
      data.push({
        payment: i,
        balance: Math.max(0, balance),
      })

      // Advance 12 payments
      for (let j = 0; j < 12 && balance > 0; j++) {
        const interestPayment = balance * periodicRate
        const principalPayment = Math.min(totalPaymentAmount - interestPayment, balance)
        balance -= principalPayment
      }
    }

    return data
  }

  return (
    <Box>
      <Typography variant="h5" gutterBottom>
        Mortgage Calculator
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
        Calculate your monthly mortgage payment and see how extra payments can save you money
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
                  label="Loan Amount"
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
                  label="Loan Term"
                  value={termYears}
                  onChange={(e) => setTermYears(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ endAdornment: 'years' }}
                />
                <FormControl fullWidth sx={{ mb: 2 }}>
                  <InputLabel>Payment Frequency</InputLabel>
                  <Select value={frequency} onChange={(e) => setFrequency(e.target.value)} label="Payment Frequency">
                    <MenuItem value="Monthly">Monthly</MenuItem>
                    <MenuItem value="BiWeekly">Bi-Weekly</MenuItem>
                    <MenuItem value="Weekly">Weekly</MenuItem>
                  </Select>
                </FormControl>
                <TextField
                  fullWidth
                  label="Extra Payment (Optional)"
                  value={extraPayment}
                  onChange={(e) => setExtraPayment(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ startAdornment: '$' }}
                  helperText="Additional amount to pay each period"
                />
                <Button variant="contained" fullWidth size="large" onClick={calculateMortgage}>
                  Calculate
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
                  Results
                </Typography>
                <Box sx={{ mt: 2 }}>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
                    <Typography variant="body1">Regular Payment:</Typography>
                    <Typography variant="h6" color="primary">
                      {formatCurrency(result.regularPayment)}
                    </Typography>
                  </Box>
                  <Divider sx={{ my: 2 }} />
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="body2" color="text.secondary">
                      Total Payments:
                    </Typography>
                    <Typography variant="body2">{result.totalPayments}</Typography>
                  </Box>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="body2" color="text.secondary">
                      Total Interest:
                    </Typography>
                    <Typography variant="body2">{formatCurrency(result.totalInterest)}</Typography>
                  </Box>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="body2" color="text.secondary">
                      Total Amount Paid:
                    </Typography>
                    <Typography variant="body2" fontWeight="bold">
                      {formatCurrency(result.totalPaid)}
                    </Typography>
                  </Box>
                  {parseFloat(extraPayment) > 0 && (
                    <Alert severity="success" sx={{ mt: 2 }}>
                      Extra payments will save you interest and reduce your loan term!
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
                  Loan Balance Over Time
                </Typography>
                <ResponsiveContainer width="100%" height={300}>
                  <LineChart data={generateChartData()}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="payment" label={{ value: 'Payment Number', position: 'insideBottom', offset: -5 }} />
                    <YAxis label={{ value: 'Balance ($)', angle: -90, position: 'insideLeft' }} />
                    <Tooltip formatter={(value: number) => formatCurrency(value)} />
                    <Legend />
                    <Line type="monotone" dataKey="balance" stroke="#1976d2" name="Remaining Balance" />
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
