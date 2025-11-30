import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import {
  Box,
  Container,
  Typography,
  Button,
  Card,
  CardContent,
  Grid,
  Chip,
  LinearProgress,
  TextField,
  MenuItem,
  IconButton,
  Tooltip,
} from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import VisibilityIcon from '@mui/icons-material/Visibility'
import EditIcon from '@mui/icons-material/Edit'
import DeleteIcon from '@mui/icons-material/Delete'
import DashboardIcon from '@mui/icons-material/Dashboard'

interface Project {
  id: string
  name: string
  projectType: string
  status: string
  description: string
  startDate: string
  plannedEndDate: string
  actualEndDate?: string
  budget: number
  owner: string
  createdDate: string
  phaseCount: number
  taskCount: number
  completionPercentage: number
}

const statusColors: Record<string, 'default' | 'primary' | 'success' | 'warning' | 'error'> = {
  Planning: 'default',
  InProgress: 'primary',
  OnHold: 'warning',
  Completed: 'success',
  Cancelled: 'error',
}

const projectTypeIcons: Record<string, string> = {
  Construction: '🏗️',
  FilmProduction: '🎬',
  Manufacturing: '🏭',
  HomeImprovement: '🏠',
  Custom: '📋',
}

export default function ProjectList() {
  const navigate = useNavigate()
  const [projects, setProjects] = useState<Project[]>([])
  const [loading, setLoading] = useState(false)
  const [filterStatus, setFilterStatus] = useState<string>('')
  const [filterType, setFilterType] = useState<string>('')

  useEffect(() => {
    loadProjects()
  }, [filterStatus, filterType])

  const loadProjects = async () => {
    setLoading(true)
    try {
      // TODO: Replace with actual API call
      const mockProjects: Project[] = [
        {
          id: '1',
          name: 'Downtown Office Building',
          projectType: 'Construction',
          status: 'InProgress',
          description: 'Commercial office building construction',
          startDate: '2025-01-01',
          plannedEndDate: '2025-12-31',
          budget: 5000000,
          owner: 'John Doe',
          createdDate: '2024-12-01',
          phaseCount: 5,
          taskCount: 45,
          completionPercentage: 35,
        },
        {
          id: '2',
          name: 'Action Movie Production',
          projectType: 'FilmProduction',
          status: 'Planning',
          description: 'Feature film production',
          startDate: '2025-03-01',
          plannedEndDate: '2025-08-31',
          budget: 2000000,
          owner: 'Jane Smith',
          createdDate: '2024-12-15',
          phaseCount: 4,
          taskCount: 28,
          completionPercentage: 10,
        },
      ]
      setProjects(mockProjects)
    } catch (error) {
      console.error('Error loading projects:', error)
    } finally {
      setLoading(false)
    }
  }

  const handleCreateProject = () => {
    navigate('/projects/create')
  }

  const handleViewProject = (projectId: string) => {
    navigate(`/projects/${projectId}`)
  }

  const handleEditProject = (projectId: string) => {
    navigate(`/projects/${projectId}/edit`)
  }

  const handleViewDashboard = (projectId: string) => {
    navigate(`/projects/${projectId}/dashboard`)
  }

  const handleDeleteProject = async (projectId: string) => {
    if (window.confirm('Are you sure you want to delete this project?')) {
      // TODO: Implement delete API call
      console.log('Deleting project:', projectId)
      loadProjects()
    }
  }

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
      minimumFractionDigits: 0,
    }).format(value)
  }

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    })
  }

  const filteredProjects = projects.filter((project) => {
    if (filterStatus && project.status !== filterStatus) return false
    if (filterType && project.projectType !== filterType) return false
    return true
  })

  return (
    <Container maxWidth="xl" sx={{ mt: 4, mb: 4 }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Box>
          <Typography variant="h4" gutterBottom>
            Project Management
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Enterprise-grade project administration and tracking
          </Typography>
        </Box>
        <Button variant="contained" startIcon={<AddIcon />} size="large" onClick={handleCreateProject}>
          New Project
        </Button>
      </Box>

      {/* Filters */}
      <Box sx={{ mb: 3, display: 'flex', gap: 2 }}>
        <TextField
          select
          label="Filter by Status"
          value={filterStatus}
          onChange={(e) => setFilterStatus(e.target.value)}
          sx={{ minWidth: 200 }}
          size="small"
        >
          <MenuItem value="">All Statuses</MenuItem>
          <MenuItem value="Planning">Planning</MenuItem>
          <MenuItem value="InProgress">In Progress</MenuItem>
          <MenuItem value="OnHold">On Hold</MenuItem>
          <MenuItem value="Completed">Completed</MenuItem>
          <MenuItem value="Cancelled">Cancelled</MenuItem>
        </TextField>

        <TextField
          select
          label="Filter by Type"
          value={filterType}
          onChange={(e) => setFilterType(e.target.value)}
          sx={{ minWidth: 200 }}
          size="small"
        >
          <MenuItem value="">All Types</MenuItem>
          <MenuItem value="Construction">Construction</MenuItem>
          <MenuItem value="FilmProduction">Film Production</MenuItem>
          <MenuItem value="Manufacturing">Manufacturing</MenuItem>
          <MenuItem value="HomeImprovement">Home Improvement</MenuItem>
          <MenuItem value="Custom">Custom</MenuItem>
        </TextField>
      </Box>

      {loading && <LinearProgress sx={{ mb: 2 }} />}

      {/* Project Cards */}
      <Grid container spacing={3}>
        {filteredProjects.map((project) => (
          <Grid item xs={12} md={6} lg={4} key={project.id}>
            <Card sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
              <CardContent sx={{ flexGrow: 1 }}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', mb: 2 }}>
                  <Box>
                    <Typography variant="h6" gutterBottom>
                      {projectTypeIcons[project.projectType] || '📋'} {project.name}
                    </Typography>
                    <Chip label={project.status} color={statusColors[project.status]} size="small" sx={{ mb: 1 }} />
                  </Box>
                  <Box>
                    <Tooltip title="View Dashboard">
                      <IconButton size="small" onClick={() => handleViewDashboard(project.id)} color="primary">
                        <DashboardIcon />
                      </IconButton>
                    </Tooltip>
                    <Tooltip title="View Details">
                      <IconButton size="small" onClick={() => handleViewProject(project.id)}>
                        <VisibilityIcon />
                      </IconButton>
                    </Tooltip>
                    <Tooltip title="Edit">
                      <IconButton size="small" onClick={() => handleEditProject(project.id)}>
                        <EditIcon />
                      </IconButton>
                    </Tooltip>
                    <Tooltip title="Delete">
                      <IconButton size="small" onClick={() => handleDeleteProject(project.id)} color="error">
                        <DeleteIcon />
                      </IconButton>
                    </Tooltip>
                  </Box>
                </Box>

                <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                  {project.description}
                </Typography>

                <Box sx={{ mb: 2 }}>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 0.5 }}>
                    <Typography variant="caption" color="text.secondary">
                      Progress
                    </Typography>
                    <Typography variant="caption" fontWeight="bold">
                      {project.completionPercentage}%
                    </Typography>
                  </Box>
                  <LinearProgress variant="determinate" value={project.completionPercentage} sx={{ height: 8, borderRadius: 1 }} />
                </Box>

                <Grid container spacing={1} sx={{ mb: 2 }}>
                  <Grid item xs={6}>
                    <Typography variant="caption" color="text.secondary" display="block">
                      Budget
                    </Typography>
                    <Typography variant="body2" fontWeight="bold">
                      {formatCurrency(project.budget)}
                    </Typography>
                  </Grid>
                  <Grid item xs={6}>
                    <Typography variant="caption" color="text.secondary" display="block">
                      Owner
                    </Typography>
                    <Typography variant="body2">{project.owner}</Typography>
                  </Grid>
                  <Grid item xs={6}>
                    <Typography variant="caption" color="text.secondary" display="block">
                      Phases
                    </Typography>
                    <Typography variant="body2">{project.phaseCount}</Typography>
                  </Grid>
                  <Grid item xs={6}>
                    <Typography variant="caption" color="text.secondary" display="block">
                      Tasks
                    </Typography>
                    <Typography variant="body2">{project.taskCount}</Typography>
                  </Grid>
                </Grid>

                <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                  <Typography variant="caption" color="text.secondary">
                    Start: {formatDate(project.startDate)}
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    End: {formatDate(project.plannedEndDate)}
                  </Typography>
                </Box>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>

      {filteredProjects.length === 0 && !loading && (
        <Box sx={{ textAlign: 'center', py: 8 }}>
          <Typography variant="h6" color="text.secondary" gutterBottom>
            No projects found
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
            Create your first project to get started
          </Typography>
          <Button variant="contained" startIcon={<AddIcon />} onClick={handleCreateProject}>
            Create Project
          </Button>
        </Box>
      )}
    </Container>
  )
}
