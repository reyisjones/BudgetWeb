import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import {
  Container,
  Typography,
  Box,
  Card,
  CardContent,
  Chip,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  MenuItem,
  Grid,
  IconButton,
  Avatar,
  Tooltip,
  Alert,
  CircularProgress
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  DragIndicator as DragIcon
} from '@mui/icons-material';

interface Task {
  id: string;
  name: string;
  description: string;
  status: 'NotStarted' | 'InProgress' | 'Blocked' | 'Completed';
  priority: 'Low' | 'Medium' | 'High' | 'Critical';
  assignedTo: string;
  dueDate: string;
  plannedHours: number;
  actualHours: number;
  phaseId: string;
  phaseName: string;
}

interface Phase {
  id: string;
  name: string;
}

const taskStatuses: Array<'NotStarted' | 'InProgress' | 'Blocked' | 'Completed'> = [
  'NotStarted',
  'InProgress',
  'Blocked',
  'Completed'
];

const statusLabels: Record<string, string> = {
  NotStarted: 'Not Started',
  InProgress: 'In Progress',
  Blocked: 'Blocked',
  Completed: 'Completed'
};

const statusColors: Record<string, string> = {
  NotStarted: '#9e9e9e',
  InProgress: '#2196f3',
  Blocked: '#f44336',
  Completed: '#4caf50'
};

const priorityColors: Record<string, 'default' | 'primary' | 'warning' | 'error'> = {
  Low: 'default',
  Medium: 'primary',
  High: 'warning',
  Critical: 'error'
};

const TaskBoard: React.FC = () => {
  const { projectId } = useParams<{ projectId: string }>();
  const [tasks, setTasks] = useState<Task[]>([]);
  const [phases, setPhases] = useState<Phase[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedTask, setSelectedTask] = useState<Task | null>(null);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [dialogMode, setDialogMode] = useState<'create' | 'edit'>('create');
  const [draggedTask, setDraggedTask] = useState<Task | null>(null);

  const [formData, setFormData] = useState({
    name: '',
    description: '',
    status: 'NotStarted' as Task['status'],
    priority: 'Medium' as Task['priority'],
    assignedTo: '',
    dueDate: '',
    plannedHours: 0,
    phaseId: ''
  });

  useEffect(() => {
    loadData();
  }, [projectId]);

  const loadData = async () => {
    setLoading(true);
    try {
      // TODO: Replace with actual API calls
      // const tasksResponse = await fetch(`/api/projects/${projectId}/tasks`);
      // const phasesResponse = await fetch(`/api/projects/${projectId}/phases`);
      // const tasksData = await tasksResponse.json();
      // const phasesData = await phasesResponse.json();

      // Mock data
      setTimeout(() => {
        setPhases([
          { id: '1', name: 'Planning & Design' },
          { id: '2', name: 'Site Preparation' },
          { id: '3', name: 'Structure Construction' }
        ]);

        setTasks([
          {
            id: '1',
            name: 'Create architectural drawings',
            description: 'Complete detailed architectural plans',
            status: 'Completed',
            priority: 'High',
            assignedTo: 'Sarah Johnson',
            dueDate: '2024-11-30',
            plannedHours: 80,
            actualHours: 75,
            phaseId: '1',
            phaseName: 'Planning & Design'
          },
          {
            id: '2',
            name: 'Submit permit applications',
            description: 'Submit all required building permits',
            status: 'InProgress',
            priority: 'Critical',
            assignedTo: 'Mike Brown',
            dueDate: '2024-12-10',
            plannedHours: 20,
            actualHours: 12,
            phaseId: '1',
            phaseName: 'Planning & Design'
          },
          {
            id: '3',
            name: 'Clear and grade site',
            description: 'Site preparation and land clearing',
            status: 'NotStarted',
            priority: 'High',
            assignedTo: 'Tom Wilson',
            dueDate: '2024-12-15',
            plannedHours: 60,
            actualHours: 0,
            phaseId: '2',
            phaseName: 'Site Preparation'
          },
          {
            id: '4',
            name: 'Install temporary utilities',
            description: 'Set up temporary power and water',
            status: 'NotStarted',
            priority: 'Medium',
            assignedTo: 'Lisa Davis',
            dueDate: '2024-12-18',
            plannedHours: 30,
            actualHours: 0,
            phaseId: '2',
            phaseName: 'Site Preparation'
          },
          {
            id: '5',
            name: 'Foundation inspection',
            description: 'Waiting for city inspector',
            status: 'Blocked',
            priority: 'Critical',
            assignedTo: 'Mike Brown',
            dueDate: '2024-12-08',
            plannedHours: 8,
            actualHours: 0,
            phaseId: '2',
            phaseName: 'Site Preparation'
          },
          {
            id: '6',
            name: 'Erect structural framing',
            description: 'Steel frame construction',
            status: 'InProgress',
            priority: 'Critical',
            assignedTo: 'John Smith',
            dueDate: '2025-01-15',
            plannedHours: 300,
            actualHours: 120,
            phaseId: '3',
            phaseName: 'Structure Construction'
          }
        ]);
        setLoading(false);
      }, 500);
    } catch (error) {
      console.error('Failed to load data', error);
      setLoading(false);
    }
  };

  const handleOpenDialog = (mode: 'create' | 'edit', task?: Task) => {
    setDialogMode(mode);
    if (mode === 'edit' && task) {
      setSelectedTask(task);
      setFormData({
        name: task.name,
        description: task.description,
        status: task.status,
        priority: task.priority,
        assignedTo: task.assignedTo,
        dueDate: task.dueDate,
        plannedHours: task.plannedHours,
        phaseId: task.phaseId
      });
    } else {
      setSelectedTask(null);
      setFormData({
        name: '',
        description: '',
        status: 'NotStarted',
        priority: 'Medium',
        assignedTo: '',
        dueDate: '',
        plannedHours: 0,
        phaseId: phases[0]?.id || ''
      });
    }
    setDialogOpen(true);
  };

  const handleCloseDialog = () => {
    setDialogOpen(false);
    setSelectedTask(null);
  };

  const handleSubmit = async () => {
    // TODO: Implement API call
    if (dialogMode === 'create') {
      // POST /api/projects/${projectId}/tasks
      console.log('Creating task:', formData);
    } else {
      // PUT /api/projects/${projectId}/tasks/${selectedTask?.id}
      console.log('Updating task:', formData);
    }
    handleCloseDialog();
    // Reload tasks after save
    // loadData();
  };

  const handleDeleteTask = async (taskId: string) => {
    if (window.confirm('Are you sure you want to delete this task?')) {
      // TODO: DELETE /api/projects/${projectId}/tasks/${taskId}
      console.log('Deleting task:', taskId);
      setTasks(tasks.filter(t => t.id !== taskId));
    }
  };

  const handleDragStart = (task: Task) => {
    setDraggedTask(task);
  };

  const handleDragOver = (e: React.DragEvent) => {
    e.preventDefault();
  };

  const handleDrop = async (newStatus: Task['status']) => {
    if (!draggedTask) return;

    // TODO: PATCH /api/projects/${projectId}/tasks/${draggedTask.id}/status
    console.log(`Moving task ${draggedTask.id} to ${newStatus}`);

    setTasks(tasks.map(t =>
      t.id === draggedTask.id ? { ...t, status: newStatus } : t
    ));
    setDraggedTask(null);
  };

  const getTasksByStatus = (status: Task['status']) => {
    return tasks.filter(task => task.status === status);
  };

  const formatDate = (dateString: string): string => {
    return new Date(dateString).toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric'
    });
  };

  const isOverdue = (dueDate: string): boolean => {
    return new Date(dueDate) < new Date() && new Date(dueDate).toDateString() !== new Date().toDateString();
  };

  if (loading) {
    return (
      <Container maxWidth="xl" sx={{ mt: 4, display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '400px' }}>
        <CircularProgress size={60} />
      </Container>
    );
  }

  return (
    <Container maxWidth="xl" sx={{ mt: 4, mb: 4 }}>
      {/* Header */}
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4">Task Board</Typography>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={() => handleOpenDialog('create')}
        >
          Add Task
        </Button>
      </Box>

      {/* Kanban Board */}
      <Grid container spacing={2}>
        {taskStatuses.map((status) => {
          const statusTasks = getTasksByStatus(status);
          return (
            <Grid item xs={12} sm={6} md={3} key={status}>
              <Box
                onDragOver={handleDragOver}
                onDrop={() => handleDrop(status)}
                sx={{
                  bgcolor: 'grey.100',
                  borderRadius: 1,
                  p: 2,
                  minHeight: 500,
                  borderTop: 3,
                  borderColor: statusColors[status]
                }}
              >
                {/* Column Header */}
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
                  <Typography variant="h6" sx={{ fontWeight: 600 }}>
                    {statusLabels[status]}
                  </Typography>
                  <Chip label={statusTasks.length} size="small" />
                </Box>

                {/* Task Cards */}
                <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
                  {statusTasks.map((task) => (
                    <Card
                      key={task.id}
                      draggable
                      onDragStart={() => handleDragStart(task)}
                      sx={{
                        cursor: 'grab',
                        '&:active': { cursor: 'grabbing' },
                        '&:hover': { boxShadow: 3 }
                      }}
                    >
                      <CardContent sx={{ p: 2, '&:last-child': { pb: 2 } }}>
                        {/* Task Header */}
                        <Box sx={{ display: 'flex', alignItems: 'flex-start', mb: 1 }}>
                          <DragIcon sx={{ color: 'grey.400', mr: 0.5, mt: 0.5 }} />
                          <Typography variant="body1" sx={{ flex: 1, fontWeight: 500 }}>
                            {task.name}
                          </Typography>
                          <Box>
                            <IconButton size="small" onClick={() => handleOpenDialog('edit', task)}>
                              <EditIcon fontSize="small" />
                            </IconButton>
                            <IconButton size="small" onClick={() => handleDeleteTask(task.id)}>
                              <DeleteIcon fontSize="small" />
                            </IconButton>
                          </Box>
                        </Box>

                        {/* Description */}
                        {task.description && (
                          <Typography variant="body2" color="text.secondary" sx={{ mb: 1.5 }}>
                            {task.description.length > 60
                              ? `${task.description.substring(0, 60)}...`
                              : task.description}
                          </Typography>
                        )}

                        {/* Priority & Phase */}
                        <Box sx={{ display: 'flex', gap: 0.5, mb: 1.5 }}>
                          <Chip label={task.priority} size="small" color={priorityColors[task.priority]} />
                          <Chip label={task.phaseName} size="small" variant="outlined" />
                        </Box>

                        {/* Due Date */}
                        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                          <Typography
                            variant="caption"
                            color={isOverdue(task.dueDate) ? 'error' : 'text.secondary'}
                            sx={{ fontWeight: isOverdue(task.dueDate) ? 600 : 400 }}
                          >
                            Due: {formatDate(task.dueDate)}
                            {isOverdue(task.dueDate) && ' (Overdue)'}
                          </Typography>
                        </Box>

                        {/* Assignee */}
                        <Box sx={{ display: 'flex', alignItems: 'center', mt: 1.5 }}>
                          <Tooltip title={task.assignedTo}>
                            <Avatar sx={{ width: 24, height: 24, fontSize: '0.75rem' }}>
                              {task.assignedTo.split(' ').map(n => n[0]).join('')}
                            </Avatar>
                          </Tooltip>
                          <Typography variant="caption" color="text.secondary" sx={{ ml: 1 }}>
                            {task.assignedTo}
                          </Typography>
                        </Box>

                        {/* Hours */}
                        {task.status !== 'NotStarted' && (
                          <Typography variant="caption" color="text.secondary" display="block" sx={{ mt: 1 }}>
                            Hours: {task.actualHours} / {task.plannedHours}
                          </Typography>
                        )}
                      </CardContent>
                    </Card>
                  ))}

                  {statusTasks.length === 0 && (
                    <Box sx={{ textAlign: 'center', py: 4 }}>
                      <Typography variant="body2" color="text.secondary">
                        No tasks
                      </Typography>
                    </Box>
                  )}
                </Box>
              </Box>
            </Grid>
          );
        })}
      </Grid>

      {/* Task Dialog */}
      <Dialog open={dialogOpen} onClose={handleCloseDialog} maxWidth="sm" fullWidth>
        <DialogTitle>
          {dialogMode === 'create' ? 'Create New Task' : 'Edit Task'}
        </DialogTitle>
        <DialogContent>
          <Grid container spacing={2} sx={{ mt: 0.5 }}>
            <Grid item xs={12}>
              <TextField
                fullWidth
                label="Task Name"
                value={formData.name}
                onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                required
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                fullWidth
                multiline
                rows={3}
                label="Description"
                value={formData.description}
                onChange={(e) => setFormData({ ...formData, description: e.target.value })}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                select
                label="Status"
                value={formData.status}
                onChange={(e) => setFormData({ ...formData, status: e.target.value as Task['status'] })}
              >
                {taskStatuses.map((status) => (
                  <MenuItem key={status} value={status}>
                    {statusLabels[status]}
                  </MenuItem>
                ))}
              </TextField>
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                select
                label="Priority"
                value={formData.priority}
                onChange={(e) => setFormData({ ...formData, priority: e.target.value as Task['priority'] })}
              >
                <MenuItem value="Low">Low</MenuItem>
                <MenuItem value="Medium">Medium</MenuItem>
                <MenuItem value="High">High</MenuItem>
                <MenuItem value="Critical">Critical</MenuItem>
              </TextField>
            </Grid>
            <Grid item xs={12}>
              <TextField
                fullWidth
                select
                label="Phase"
                value={formData.phaseId}
                onChange={(e) => setFormData({ ...formData, phaseId: e.target.value })}
                required
              >
                {phases.map((phase) => (
                  <MenuItem key={phase.id} value={phase.id}>
                    {phase.name}
                  </MenuItem>
                ))}
              </TextField>
            </Grid>
            <Grid item xs={12}>
              <TextField
                fullWidth
                label="Assigned To"
                value={formData.assignedTo}
                onChange={(e) => setFormData({ ...formData, assignedTo: e.target.value })}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                type="date"
                label="Due Date"
                value={formData.dueDate}
                onChange={(e) => setFormData({ ...formData, dueDate: e.target.value })}
                InputLabelProps={{ shrink: true }}
                required
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                type="number"
                label="Planned Hours"
                value={formData.plannedHours}
                onChange={(e) => setFormData({ ...formData, plannedHours: parseFloat(e.target.value) || 0 })}
                inputProps={{ min: 0, step: 1 }}
              />
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog}>Cancel</Button>
          <Button onClick={handleSubmit} variant="contained">
            {dialogMode === 'create' ? 'Create' : 'Save'}
          </Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
};

export default TaskBoard;
