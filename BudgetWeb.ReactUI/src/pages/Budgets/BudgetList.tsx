import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import Container from '@mui/material/Container'
import Typography from '@mui/material/Typography'
import Button from '@mui/material/Button'
import Box from '@mui/material/Box'
import Paper from '@mui/material/Paper'
import AddIcon from '@mui/icons-material/Add'
import { DataGrid, GridColDef, GridRenderCellParams } from '@mui/x-data-grid'
import Chip from '@mui/material/Chip'
import IconButton from '@mui/material/IconButton'
import EditIcon from '@mui/icons-material/Edit'
import DeleteIcon from '@mui/icons-material/Delete'
import VisibilityIcon from '@mui/icons-material/Visibility'
import { budgetApi, Budget } from '@/services/api'

export default function BudgetList() {
  const navigate = useNavigate()
  const [budgets, setBudgets] = useState<Budget[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    fetchBudgets()
  }, [])

  const fetchBudgets = async () => {
    try {
      setLoading(true)
      // const data = await budgetApi.getAll()
      // Mock data for now
      const mockData: Budget[] = [
        {
          id: '1',
          name: '2025 Q1 Budget',
          description: 'First quarter operational budget',
          totalAmount: { amount: 250000, currency: 'USD' },
          spentAmount: { amount: 175000, currency: 'USD' },
          period: 'Quarterly',
          startDate: '2025-01-01',
          endDate: '2025-03-31',
          status: 'Active',
          categories: [],
        },
        {
          id: '2',
          name: 'Marketing Campaign 2025',
          description: 'Annual marketing budget',
          totalAmount: { amount: 150000, currency: 'USD' },
          spentAmount: { amount: 45000, currency: 'USD' },
          period: 'Annual',
          startDate: '2025-01-01',
          endDate: '2025-12-31',
          status: 'Active',
          categories: [],
        },
        {
          id: '3',
          name: 'IT Infrastructure',
          description: 'Technology and infrastructure investment',
          totalAmount: { amount: 500000, currency: 'USD' },
          spentAmount: { amount: 320000, currency: 'USD' },
          period: 'Annual',
          startDate: '2025-01-01',
          endDate: '2025-12-31',
          status: 'Active',
          categories: [],
        },
      ]
      setBudgets(mockData)
    } catch (error) {
      console.error('Failed to fetch budgets:', error)
    } finally {
      setLoading(false)
    }
  }

  const handleDelete = async (id: string) => {
    if (window.confirm('Are you sure you want to delete this budget?')) {
      try {
        await budgetApi.delete(id)
        fetchBudgets()
      } catch (error) {
        console.error('Failed to delete budget:', error)
      }
    }
  }

  const getStatusColor = (status: string) => {
    switch (status.toLowerCase()) {
      case 'active':
        return 'success'
      case 'completed':
        return 'info'
      case 'draft':
        return 'warning'
      default:
        return 'default'
    }
  }

  const getUtilizationColor = (percentage: number) => {
    if (percentage >= 90) return 'error'
    if (percentage >= 75) return 'warning'
    return 'success'
  }

  const columns: GridColDef[] = [
    {
      field: 'name',
      headerName: 'Budget Name',
      flex: 1,
      minWidth: 200,
    },
    {
      field: 'period',
      headerName: 'Period',
      width: 120,
    },
    {
      field: 'totalAmount',
      headerName: 'Total Amount',
      width: 150,
      valueGetter: (params) => params.row.totalAmount.amount,
      renderCell: (params: GridRenderCellParams) =>
        `$${params.row.totalAmount.amount.toLocaleString()}`,
    },
    {
      field: 'spentAmount',
      headerName: 'Spent',
      width: 150,
      valueGetter: (params) => params.row.spentAmount.amount,
      renderCell: (params: GridRenderCellParams) =>
        `$${params.row.spentAmount.amount.toLocaleString()}`,
    },
    {
      field: 'utilization',
      headerName: 'Utilization',
      width: 140,
      renderCell: (params: GridRenderCellParams) => {
        const percentage = (params.row.spentAmount.amount / params.row.totalAmount.amount) * 100
        return (
          <Chip
            label={`${percentage.toFixed(1)}%`}
            color={getUtilizationColor(percentage)}
            size="small"
          />
        )
      },
    },
    {
      field: 'status',
      headerName: 'Status',
      width: 120,
      renderCell: (params: GridRenderCellParams) => (
        <Chip label={params.row.status} color={getStatusColor(params.row.status)} size="small" />
      ),
    },
    {
      field: 'actions',
      headerName: 'Actions',
      width: 150,
      sortable: false,
      renderCell: (params: GridRenderCellParams) => (
        <Box>
          <IconButton
            size="small"
            color="primary"
            onClick={() => navigate(`/budgets/${params.row.id}`)}
          >
            <VisibilityIcon />
          </IconButton>
          <IconButton
            size="small"
            color="primary"
            onClick={() => navigate(`/budgets/${params.row.id}/edit`)}
          >
            <EditIcon />
          </IconButton>
          <IconButton size="small" color="error" onClick={() => handleDelete(params.row.id)}>
            <DeleteIcon />
          </IconButton>
        </Box>
      ),
    },
  ]

  return (
    <Container maxWidth="xl" sx={{ mt: 4, mb: 4 }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4">Budgets</Typography>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={() => navigate('/budgets/create')}
        >
          Create Budget
        </Button>
      </Box>

      <Paper sx={{ width: '100%', overflow: 'hidden' }}>
        <DataGrid
          rows={budgets}
          columns={columns}
          loading={loading}
          initialState={{
            pagination: {
              paginationModel: { pageSize: 10, page: 0 },
            },
          }}
          pageSizeOptions={[5, 10, 25, 50]}
          checkboxSelection
          disableRowSelectionOnClick
          autoHeight
        />
      </Paper>
    </Container>
  )
}
