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
  FormControl,
  InputLabel,
  Select,
  MenuItem,
} from '@mui/material'

interface StudentLoanResult {
  monthlyPayment: number
  totalPayments: number
  totalInterest: number
  totalPaid: number
  loanBalance: number
}

export default function StudentLoanCalculator() {
  const [principal, setPrincipal] = useState('30000')
  const [interestRate, setInterestRate] = useState('5')
  const [termYears, setTermYears] = useState('10')
  const [gracePeriod, setGracePeriod] = useState('6')
  const [deferment, setDeferment] = useState('0')
  const [planType, setPlanType] = useState('standard')
  const [annualIncome, setAnnualIncome] = useState('50000')
  const [familySize, setFamilySize] = useState('1')
  const [result, setResult] = useState<StudentLoanResult | null>(null)
  const [error, setError] = useState('')

  const calculateStudentLoan = () => {
    setError('')

    const p = parseFloat(principal)
    const r = parseFloat(interestRate)
    const y = parseInt(termYears)
    const grace = parseInt(gracePeriod)
    const defer = parseInt(deferment)

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

    const monthlyRate = r / 12 / 100

    // Calculate capitalized interest during grace and deferment
    const gracePeriodInterest = p * monthlyRate * grace
    const defermentInterest = (p + gracePeriodInterest) * monthlyRate * defer
    const newPrincipal = p + gracePeriodInterest + defermentInterest

    let monthlyPayment: number

    if (planType === 'income-based') {
      // Simplified income-based calculation
      const income = parseFloat(annualIncome)
      const size = parseInt(familySize)
      const povertyGuideline = size === 1 ? 15060 : size === 2 ? 20440 : size === 3 ? 25820 : 31200
      const discretionaryIncome = Math.max(0, income - povertyGuideline * 1.5)
      monthlyPayment = (discretionaryIncome * 0.1) / 12
    } else {
      // Standard repayment
      const totalPayments = y * 12
      if (monthlyRate === 0) {
        monthlyPayment = newPrincipal / totalPayments
      } else {
        monthlyPayment =
          newPrincipal * (monthlyRate * Math.pow(1 + monthlyRate, totalPayments)) / (Math.pow(1 + monthlyRate, totalPayments) - 1)
      }
    }

    const totalPayments = y * 12
    const totalPaid = monthlyPayment * totalPayments
    const totalInterest = totalPaid - newPrincipal + gracePeriodInterest + defermentInterest

    setResult({
      monthlyPayment,
      totalPayments,
      totalInterest,
      totalPaid,
      loanBalance: newPrincipal,
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
        Student Loan Calculator
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
        Calculate student loan payments with grace period and income-based repayment options
      </Typography>

      <Grid container spacing={3}>
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Loan Information
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
                  label="Repayment Term"
                  value={termYears}
                  onChange={(e) => setTermYears(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ endAdornment: 'years' }}
                />
                <TextField
                  fullWidth
                  label="Grace Period"
                  value={gracePeriod}
                  onChange={(e) => setGracePeriod(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ endAdornment: 'months' }}
                  helperText="Typical grace period is 6 months"
                />
                <TextField
                  fullWidth
                  label="Deferment Period"
                  value={deferment}
                  onChange={(e) => setDeferment(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ endAdornment: 'months' }}
                />
                <FormControl fullWidth sx={{ mb: 2 }}>
                  <InputLabel>Repayment Plan</InputLabel>
                  <Select value={planType} onChange={(e) => setPlanType(e.target.value)} label="Repayment Plan">
                    <MenuItem value="standard">Standard (10 years)</MenuItem>
                    <MenuItem value="income-based">Income-Based Repayment</MenuItem>
                  </Select>
                </FormControl>
                {planType === 'income-based' && (
                  <>
                    <TextField
                      fullWidth
                      label="Annual Income"
                      value={annualIncome}
                      onChange={(e) => setAnnualIncome(e.target.value)}
                      type="number"
                      sx={{ mb: 2 }}
                      InputProps={{ startAdornment: '$' }}
                    />
                    <TextField
                      fullWidth
                      label="Family Size"
                      value={familySize}
                      onChange={(e) => setFamilySize(e.target.value)}
                      type="number"
                      sx={{ mb: 2 }}
                    />
                  </>
                )}
                <Button variant="contained" fullWidth size="large" onClick={calculateStudentLoan}>
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
                    <Typography variant="body1">Monthly Payment:</Typography>
                    <Typography variant="h6" color="primary">
                      {formatCurrency(result.monthlyPayment)}
                    </Typography>
                  </Box>
                  <Divider sx={{ my: 2 }} />
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="body2" color="text.secondary">
                      Adjusted Loan Balance:
                    </Typography>
                    <Typography variant="body2">{formatCurrency(result.loanBalance)}</Typography>
                  </Box>
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
                  {planType === 'income-based' && (
                    <Alert severity="info" sx={{ mt: 2 }}>
                      Income-based payments may qualify for loan forgiveness after 20-25 years
                    </Alert>
                  )}
                </Box>
              </CardContent>
            </Card>
          )}
        </Grid>
      </Grid>
    </Box>
  )
}
