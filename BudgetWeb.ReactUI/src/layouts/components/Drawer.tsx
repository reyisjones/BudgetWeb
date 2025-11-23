import { Link as RouterLink, useLocation } from 'react-router-dom'
import MuiDrawer from '@mui/material/Drawer'
import List from '@mui/material/List'
import ListItem from '@mui/material/ListItem'
import ListItemButton from '@mui/material/ListItemButton'
import ListItemIcon from '@mui/material/ListItemIcon'
import ListItemText from '@mui/material/ListItemText'
import Divider from '@mui/material/Divider'
import IconButton from '@mui/material/IconButton'
import ChevronLeftIcon from '@mui/icons-material/ChevronLeft'
import DashboardIcon from '@mui/icons-material/Dashboard'
import AccountBalanceWalletIcon from '@mui/icons-material/AccountBalanceWallet'
import WorkIcon from '@mui/icons-material/Work'
import ReceiptIcon from '@mui/icons-material/Receipt'
import AssessmentIcon from '@mui/icons-material/Assessment'
import CalculateIcon from '@mui/icons-material/Calculate'
import { styled } from '@mui/material/styles'

interface DrawerProps {
  open: boolean
  onClose: () => void
  drawerWidth: number
}

const DrawerHeader = styled('div')(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  padding: theme.spacing(0, 1),
  ...theme.mixins.toolbar,
  justifyContent: 'flex-end',
}))

const menuItems = [
  { text: 'Dashboard', icon: <DashboardIcon />, path: '/dashboard' },
  { text: 'Budgets', icon: <AccountBalanceWalletIcon />, path: '/budgets' },
  { text: 'Projects', icon: <WorkIcon />, path: '/projects' },
  { text: 'Transactions', icon: <ReceiptIcon />, path: '/transactions' },
  { text: 'Reports', icon: <AssessmentIcon />, path: '/reports' },
  { text: 'Calculators', icon: <CalculateIcon />, path: '/calculators' },
]

export default function Drawer({ open, onClose, drawerWidth }: DrawerProps) {
  const location = useLocation()

  return (
    <MuiDrawer
      sx={{
        width: drawerWidth,
        flexShrink: 0,
        '& .MuiDrawer-paper': {
          width: drawerWidth,
          boxSizing: 'border-box',
        },
      }}
      variant="persistent"
      anchor="left"
      open={open}
    >
      <DrawerHeader>
        <IconButton onClick={onClose}>
          <ChevronLeftIcon />
        </IconButton>
      </DrawerHeader>
      <Divider />
      <List>
        {menuItems.map((item) => (
          <ListItem key={item.text} disablePadding>
            <ListItemButton
              component={RouterLink}
              to={item.path}
              selected={location.pathname === item.path}
            >
              <ListItemIcon>{item.icon}</ListItemIcon>
              <ListItemText primary={item.text} />
            </ListItemButton>
          </ListItem>
        ))}
      </List>
    </MuiDrawer>
  )
}
