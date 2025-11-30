import { useState } from 'react'
import Container from '@mui/material/Container'
import Typography from '@mui/material/Typography'
import Box from '@mui/material/Box'
import Tabs from '@mui/material/Tabs'
import Tab from '@mui/material/Tab'
import Paper from '@mui/material/Paper'
import MortgageCalculator from '../components/calculators/MortgageCalculator'
import CarLoanCalculator from '../components/calculators/CarLoanCalculator'
import StudentLoanCalculator from '../components/calculators/StudentLoanCalculator'
import LoanAcceleratorCalculator from '../components/calculators/LoanAcceleratorCalculator'
import DebtToIncomeCalculator from '../components/calculators/DebtToIncomeCalculator'
import SavingsGoalCalculator from '../components/calculators/SavingsGoalCalculator'
import InvestmentCalculator from '../components/calculators/InvestmentCalculator'
import RefinanceCalculator from '../components/calculators/RefinanceCalculator'
import BudgetAnalyzer from '../components/calculators/BudgetAnalyzer'

interface TabPanelProps {
  children?: React.ReactNode
  index: number
  value: number
}

function TabPanel(props: TabPanelProps) {
  const { children, value, index, ...other } = props

  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`calculator-tabpanel-${index}`}
      aria-labelledby={`calculator-tab-${index}`}
      {...other}
    >
      {value === index && <Box sx={{ p: 3 }}>{children}</Box>}
    </div>
  )
}

export default function Calculators() {
  const [currentTab, setCurrentTab] = useState(0)

  const handleTabChange = (_event: React.SyntheticEvent, newValue: number) => {
    setCurrentTab(newValue)
  }

  return (
    <Container maxWidth="xl" sx={{ mt: 4, mb: 4 }}>
      <Typography variant="h4" gutterBottom>
        Financial Calculators
      </Typography>
      <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
        Comprehensive financial planning tools powered by our calculation engine
      </Typography>

      <Paper sx={{ width: '100%' }}>
        <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
          <Tabs
            value={currentTab}
            onChange={handleTabChange}
            variant="scrollable"
            scrollButtons="auto"
            aria-label="calculator tabs"
          >
            <Tab label="Mortgage" />
            <Tab label="Car Loan" />
            <Tab label="Student Loan" />
            <Tab label="Loan Accelerator" />
            <Tab label="Debt-to-Income" />
            <Tab label="Savings Goal" />
            <Tab label="Investment" />
            <Tab label="Refinance" />
            <Tab label="Budget Analyzer" />
          </Tabs>
        </Box>

        <TabPanel value={currentTab} index={0}>
          <MortgageCalculator />
        </TabPanel>
        <TabPanel value={currentTab} index={1}>
          <CarLoanCalculator />
        </TabPanel>
        <TabPanel value={currentTab} index={2}>
          <StudentLoanCalculator />
        </TabPanel>
        <TabPanel value={currentTab} index={3}>
          <LoanAcceleratorCalculator />
        </TabPanel>
        <TabPanel value={currentTab} index={4}>
          <DebtToIncomeCalculator />
        </TabPanel>
        <TabPanel value={currentTab} index={5}>
          <SavingsGoalCalculator />
        </TabPanel>
        <TabPanel value={currentTab} index={6}>
          <InvestmentCalculator />
        </TabPanel>
        <TabPanel value={currentTab} index={7}>
          <RefinanceCalculator />
        </TabPanel>
        <TabPanel value={currentTab} index={8}>
          <BudgetAnalyzer />
        </TabPanel>
      </Paper>
    </Container>
  )
}
