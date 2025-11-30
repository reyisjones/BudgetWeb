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

interface DTIResult {
  dtiRatio: number
  monthlyDebtPayments: number
  monthlyGrossIncome: number
  status: string
  statusColor: 'success' | 'warning' | 'error'
  recommendation: string
}

export default function DebtToIncomeCalculator() {
  const [monthlyIncome, setMonthlyIncome] = useState('5000')
  const [mortgage, setMortgage] = useState('1500')
  const [carLoan, setCarLoan] = useState('400')
  const [creditCards, setCreditCards] = useState('200')
  const [studentLoans, setStudentLoans] = useState('300')
  const [otherDebts, setOtherDebts] = useState('0')
  const [result, setResult] = useState<DTIResult | null>(null)
  const [error, setError] = useState('')

  const calculateDTI = () => {
    setError('')

    const income = parseFloat(monthlyIncome)
    const mort = parseFloat(mortgage)
    const car = parseFloat(carLoan)
    const cc = parseFloat(creditCards)
    const student = parseFloat(studentLoans)
    const other = parseFloat(otherDebts)

    if (isNaN(income) || income <= 0) {
      setError('Please enter a valid monthly income')
      return
    }

    const totalDebts = mort + car + cc + student + other
    const dtiRatio = (totalDebts / income) * 100

    let status = ''
    let statusColor: 'success' | 'warning' | 'error' = 'success'
    let recommendation = ''

    if (dtiRatio <= 28) {
      status = 'Excellent'
      statusColor = 'success'
      recommendation = 'Your debt-to-income ratio is excellent. You have good financial flexibility.'
    } else if (dtiRatio <= 36) {
      status = 'Good'
      statusColor = 'success'
      recommendation = 'Your debt-to-income ratio is healthy. Most lenders will view you favorably.'
    } else if (dtiRatio <= 43) {
      status = 'Fair'
      statusColor = 'warning'
      recommendation = 'Your DTI is manageable but could be improved. Consider paying down debt before taking on new loans.'
    } else if (dtiRatio <= 50) {
      status = 'High'
      statusColor = 'error'
      recommendation = 'Your DTI is high. Focus on reducing debt and increasing income. New loan approval may be difficult.'
    } else {
      status = 'Very High'
      statusColor = 'error'
      recommendation = 'Your DTI is very high. Prioritize debt reduction immediately. Avoid taking on new debt.'
    }

    setResult({
      dtiRatio,
      monthlyDebtPayments: totalDebts,
      monthlyGrossIncome: income,
      status,
      statusColor,
      recommendation,
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
        Debt-to-Income Ratio Calculator
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
        Calculate your DTI ratio to understand your financial health and loan qualification potential
      </Typography>

      <Grid container spacing={3}>
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Income & Debts
              </Typography>
              <Box component="form" noValidate sx={{ mt: 2 }}>
                <TextField
                  fullWidth
                  label="Monthly Gross Income"
                  value={monthlyIncome}
                  onChange={(e) => setMonthlyIncome(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ startAdornment: '$' }}
                  helperText="Before taxes and deductions"
                />
                <Divider sx={{ my: 2 }}>
                  <Typography variant="caption" color="text.secondary">
                    Monthly Debt Payments
                  </Typography>
                </Divider>
                <TextField
                  fullWidth
                  label="Mortgage/Rent Payment"
                  value={mortgage}
                  onChange={(e) => setMortgage(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ startAdornment: '$' }}
                />
                <TextField
                  fullWidth
                  label="Car Loan Payment"
                  value={carLoan}
                  onChange={(e) => setCarLoan(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ startAdornment: '$' }}
                />
                <TextField
                  fullWidth
                  label="Credit Card Minimum Payments"
                  value={creditCards}
                  onChange={(e) => setCreditCards(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ startAdornment: '$' }}
                />
                <TextField
                  fullWidth
                  label="Student Loan Payment"
                  value={studentLoans}
                  onChange={(e) => setStudentLoans(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ startAdornment: '$' }}
                />
                <TextField
                  fullWidth
                  label="Other Debt Payments"
                  value={otherDebts}
                  onChange={(e) => setOtherDebts(e.target.value)}
                  type="number"
                  sx={{ mb: 2 }}
                  InputProps={{ startAdornment: '$' }}
                  helperText="Personal loans, alimony, etc."
                />
                <Button variant="contained" fullWidth size="large" onClick={calculateDTI}>
                  Calculate DTI
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
                  Your DTI Ratio
                </Typography>
                <Box sx={{ mt: 2, textAlign: 'center', mb: 3 }}>
                  <Typography variant="h2" color="primary" gutterBottom>
                    {result.dtiRatio.toFixed(1)}%
                  </Typography>
                  <Chip label={result.status} color={result.statusColor} size="large" sx={{ mb: 2 }} />
                </Box>
                <Divider sx={{ my: 2 }} />
                <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                  <Typography variant="body2" color="text.secondary">
                    Monthly Gross Income:
                  </Typography>
                  <Typography variant="body2">{formatCurrency(result.monthlyGrossIncome)}</Typography>
                </Box>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                  <Typography variant="body2" color="text.secondary">
                    Total Monthly Debts:
                  </Typography>
                  <Typography variant="body2">{formatCurrency(result.monthlyDebtPayments)}</Typography>
                </Box>
                <Alert severity={result.statusColor === 'success' ? 'success' : result.statusColor === 'warning' ? 'warning' : 'error'} sx={{ mt: 3 }}>
                  {result.recommendation}
                </Alert>
                <Box sx={{ mt: 3 }}>
                  <Typography variant="subtitle2" gutterBottom>
                    DTI Guidelines:
                  </Typography>
                  <Typography variant="caption" display="block" color="text.secondary">
                    • 0-28%: Excellent - Strong financial position
                  </Typography>
                  <Typography variant="caption" display="block" color="text.secondary">
                    • 29-36%: Good - Healthy debt level
                  </Typography>
                  <Typography variant="caption" display="block" color="text.secondary">
                    • 37-43%: Fair - Manageable but improvement recommended
                  </Typography>
                  <Typography variant="caption" display="block" color="text.secondary">
                    • 44%+: High - Focus on debt reduction
                  </Typography>
                </Box>
              </CardContent>
            </Card>
          )}
        </Grid>
      </Grid>
    </Box>
  )
}
