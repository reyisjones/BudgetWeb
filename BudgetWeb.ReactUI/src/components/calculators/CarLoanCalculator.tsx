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

interface CarLoanResult {
  vehiclePrice: number
  loanAmount: number
  monthlyPayment: number
  totalInterest: number
  totalCost: number
  salesTax: number
}

export default function CarLoanCalculator() {
  const [vehiclePrice, setVehiclePrice] = useState('30000')
  const [downPayment, setDownPayment] = useState('5000')
  const [tradeIn, setTradeIn] = useState('0')
  const [salesTaxRate, setSalesTaxRate] = useState('8')
  const [fees, setFees] = useState('500')
  const [interestRate, setInterestRate] = useState('5')
  const [termMonths, setTermMonths] = useState('60')
  const [result, setResult] = useState<CarLoanResult | null>(null)
  const [error, setError] = useState('')

  const calculateCarLoan = () => {
    setError('')

    const price = parseFloat(vehiclePrice)
    const down = parseFloat(downPayment)
    const trade = parseFloat(tradeIn)
    const taxRate = parseFloat(salesTaxRate)
    const fee = parseFloat(fees)
    const rate = parseFloat(interestRate)
    const term = parseInt(termMonths)

    if (isNaN(price) || price <= 0) {
      setError('Please enter a valid vehicle price')
      return
    }
    if (isNaN(down) || down < 0) {
      setError('Please enter a valid down payment')
      return
    }
    if (isNaN(trade) || trade < 0) {
      setError('Please enter a valid trade-in value')
      return
    }
    if (isNaN(taxRate) || taxRate < 0) {
      setError('Please enter a valid sales tax rate')
      return
    }
    if (isNaN(fee) || fee < 0) {
      setError('Please enter valid fees')
      return
    }
    if (isNaN(rate) || rate < 0) {
      setError('Please enter a valid interest rate')
      return
    }
    if (isNaN(term) || term <= 0) {
      setError('Please enter a valid loan term')
      return
    }

    const salesTax = price * (taxRate / 100)
    const totalPrice = price + salesTax + fee
    const loanAmount = totalPrice - down - trade

    if (loanAmount <= 0) {
      setError('Down payment and trade-in exceed vehicle price')
      return
    }

    const monthlyRate = rate / 12 / 100
    let monthlyPayment: number

    if (monthlyRate === 0) {
      monthlyPayment = loanAmount / term
    } else {
      monthlyPayment = loanAmount * (monthlyRate * Math.pow(1 + monthlyRate, term)) / (Math.pow(1 + monthlyRate, term) - 1)
    }

    const totalPaid = monthlyPayment * term
    const totalInterest = totalPaid - loanAmount
    const totalCost = down + trade + totalPaid

    setResult({
      vehiclePrice: price,
      loanAmount,
      monthlyPayment,
      totalInterest,
      totalCost,
      salesTax,
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
        Car Loan Calculator
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
        Calculate your auto loan payment including sales tax and fees
      </Typography>

      <Grid container spacing={3}>
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Vehicle Information
              </Typography>
              <Box component="form" noValidate sx={{ mt: 2 }}>
                <TextField
                  fullWidth
                  label="Vehicle Price"
                  value={vehiclePrice}
                  onChange={(e) => setVehiclePrice(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ startAdornment: '$' }}
                />
                <TextField
                  fullWidth
                  label="Down Payment"
                  value={downPayment}
                  onChange={(e) => setDownPayment(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ startAdornment: '$' }}
                />
                <TextField
                  fullWidth
                  label="Trade-In Value"
                  value={tradeIn}
                  onChange={(e) => setTradeIn(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ startAdornment: '$' }}
                />
                <TextField
                  fullWidth
                  label="Sales Tax Rate"
                  value={salesTaxRate}
                  onChange={(e) => setSalesTaxRate(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ endAdornment: '%' }}
                />
                <TextField
                  fullWidth
                  label="Fees (Title, Registration, etc.)"
                  value={fees}
                  onChange={(e) => setFees(e.target.value)}
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
                  InputProps={{ endAdornment: '% APR' }}
                />
                <TextField
                  fullWidth
                  label="Loan Term"
                  value={termMonths}
                  onChange={(e) => setTermMonths(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ endAdornment: 'months' }}
                  helperText="Common terms: 36, 48, 60, or 72 months"
                />
                <Button variant="contained" fullWidth size="large" onClick={calculateCarLoan}>
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
                      Vehicle Price:
                    </Typography>
                    <Typography variant="body2">{formatCurrency(result.vehiclePrice)}</Typography>
                  </Box>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="body2" color="text.secondary">
                      Sales Tax:
                    </Typography>
                    <Typography variant="body2">{formatCurrency(result.salesTax)}</Typography>
                  </Box>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="body2" color="text.secondary">
                      Loan Amount:
                    </Typography>
                    <Typography variant="body2">{formatCurrency(result.loanAmount)}</Typography>
                  </Box>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="body2" color="text.secondary">
                      Total Interest:
                    </Typography>
                    <Typography variant="body2">{formatCurrency(result.totalInterest)}</Typography>
                  </Box>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="body2" color="text.secondary">
                      Total Cost:
                    </Typography>
                    <Typography variant="body2" fontWeight="bold">
                      {formatCurrency(result.totalCost)}
                    </Typography>
                  </Box>
                </Box>
              </CardContent>
            </Card>
          )}
        </Grid>
      </Grid>
    </Box>
  )
}
