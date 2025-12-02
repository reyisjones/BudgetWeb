import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Container,
  Typography,
  Box,
  Button,
  TextField,
  MenuItem,
  Card,
  CardContent,
  Grid,
  Alert,
  CircularProgress,
  Breadcrumbs,
  Link
} from '@mui/material';
import {
  ArrowBack as ArrowBackIcon,
  Save as SaveIcon
} from '@mui/icons-material';

interface Project {
  id: string;
  name: string;
  type: string;
  status: string;
  description: string;
  owner: string;
  startDate: string;
  endDate: string;
  budget: number;
}

const projectTypes = [
  { value: 'Construction', label: 'Construction', icon: '🏗️' },
  { value: 'FilmProduction', label: 'Film Production', icon: '🎬' },
  { value: 'Manufacturing', label: 'Manufacturing', icon: '🏭' },
  { value: 'HomeImprovement', label: 'Home Improvement', icon: '🏠' },
  { value: 'Custom', label: 'Custom', icon: '⚙️' }
];

const projectStatuses = [
  { value: 'Planning', label: 'Planning' },
  { value: 'InProgress', label: 'In Progress' },
  { value: 'OnHold', label: 'On Hold' },
  { value: 'Completed', label: 'Completed' },
  { value: 'Cancelled', label: 'Cancelled' }
];

const ProjectEdit: React.FC = () => {
  const { projectId } = useParams<{ projectId: string }>();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  const [formData, setFormData] = useState<Project>({
    id: '',
    name: '',
    type: '',
    status: '',
    description: '',
    owner: '',
    startDate: '',
    endDate: '',
    budget: 0
  });

  const [errors, setErrors] = useState<{ [key: string]: string }>({});

  useEffect(() => {
    loadProject();
  }, [projectId]);

  const loadProject = async () => {
    setLoading(true);
    setError(null);

    try {
      // TODO: Replace with actual API call
      // const response = await fetch(`/api/projects/${projectId}/detail`);
      // const data = await response.json();
      // setFormData(data);

      // Mock data for development
      setTimeout(() => {
        setFormData({
          id: projectId || '1',
          name: 'Downtown Office Building',
          type: 'Construction',
          status: 'InProgress',
          description: 'New 5-story office building construction in downtown area with modern amenities and sustainable design features.',
          owner: 'John Smith',
          startDate: '2024-01-15',
          endDate: '2024-12-31',
          budget: 2500000
        });
        setLoading(false);
      }, 500);
    } catch (err) {
      setError('Failed to load project data');
      setLoading(false);
    }
  };

  const handleChange = (field: keyof Project) => (
    event: React.ChangeEvent<HTMLInputElement>
  ) => {
    const value = field === 'budget' ? parseFloat(event.target.value) || 0 : event.target.value;
    setFormData(prev => ({ ...prev, [field]: value }));
    
    // Clear error for this field
    if (errors[field]) {
      setErrors(prev => {
        const newErrors = { ...prev };
        delete newErrors[field];
        return newErrors;
      });
    }
  };

  const validateForm = (): boolean => {
    const newErrors: { [key: string]: string } = {};

    if (!formData.name.trim()) {
      newErrors.name = 'Project name is required';
    }

    if (!formData.type) {
      newErrors.type = 'Project type is required';
    }

    if (!formData.status) {
      newErrors.status = 'Project status is required';
    }

    if (!formData.owner.trim()) {
      newErrors.owner = 'Owner is required';
    }

    if (!formData.startDate) {
      newErrors.startDate = 'Start date is required';
    }

    if (!formData.endDate) {
      newErrors.endDate = 'End date is required';
    }

    if (formData.startDate && formData.endDate && formData.endDate < formData.startDate) {
      newErrors.endDate = 'End date must be after start date';
    }

    if (formData.budget <= 0) {
      newErrors.budget = 'Budget must be greater than 0';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setSuccess(false);
    setError(null);

    if (!validateForm()) {
      return;
    }

    setSaving(true);

    try {
      // TODO: Replace with actual API call
      // const response = await fetch(`/api/projects/${projectId}`, {
      //   method: 'PUT',
      //   headers: { 'Content-Type': 'application/json' },
      //   body: JSON.stringify(formData)
      // });

      // Mock API call
      await new Promise(resolve => setTimeout(resolve, 1000));

      setSuccess(true);
      setTimeout(() => {
        navigate(`/projects/${projectId}`);
      }, 1500);
    } catch (err) {
      setError('Failed to update project. Please try again.');
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <Container maxWidth="md" sx={{ mt: 4, display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '400px' }}>
        <CircularProgress size={60} />
      </Container>
    );
  }

  return (
    <Container maxWidth="md" sx={{ mt: 4, mb: 4 }}>
      {/* Breadcrumbs */}
      <Breadcrumbs sx={{ mb: 3 }}>
        <Link
          component="button"
          variant="body2"
          onClick={() => navigate('/projects')}
          sx={{ textDecoration: 'none', '&:hover': { textDecoration: 'underline' } }}
        >
          Projects
        </Link>
        <Link
          component="button"
          variant="body2"
          onClick={() => navigate(`/projects/${projectId}`)}
          sx={{ textDecoration: 'none', '&:hover': { textDecoration: 'underline' } }}
        >
          {formData.name}
        </Link>
        <Typography color="text.primary">Edit</Typography>
      </Breadcrumbs>

      {/* Header */}
      <Box sx={{ mb: 3 }}>
        <Button
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate(`/projects/${projectId}`)}
          sx={{ mb: 2 }}
        >
          Back to Project
        </Button>
        <Typography variant="h4" gutterBottom>
          Edit Project
        </Typography>
        <Typography variant="body2" color="text.secondary">
          Update project details and settings
        </Typography>
      </Box>

      {/* Alerts */}
      {error && (
        <Alert severity="error" sx={{ mb: 3 }} onClose={() => setError(null)}>
          {error}
        </Alert>
      )}
      {success && (
        <Alert severity="success" sx={{ mb: 3 }}>
          Project updated successfully! Redirecting...
        </Alert>
      )}

      {/* Form */}
      <Card>
        <CardContent>
          <form onSubmit={handleSubmit}>
            <Grid container spacing={3}>
              {/* Project Name */}
              <Grid item xs={12}>
                <TextField
                  fullWidth
                  label="Project Name"
                  value={formData.name}
                  onChange={handleChange('name')}
                  error={!!errors.name}
                  helperText={errors.name}
                  required
                />
              </Grid>

              {/* Project Type */}
              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  select
                  label="Project Type"
                  value={formData.type}
                  onChange={handleChange('type')}
                  error={!!errors.type}
                  helperText={errors.type}
                  required
                >
                  {projectTypes.map((type) => (
                    <MenuItem key={type.value} value={type.value}>
                      {type.icon} {type.label}
                    </MenuItem>
                  ))}
                </TextField>
              </Grid>

              {/* Project Status */}
              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  select
                  label="Status"
                  value={formData.status}
                  onChange={handleChange('status')}
                  error={!!errors.status}
                  helperText={errors.status}
                  required
                >
                  {projectStatuses.map((status) => (
                    <MenuItem key={status.value} value={status.value}>
                      {status.label}
                    </MenuItem>
                  ))}
                </TextField>
              </Grid>

              {/* Description */}
              <Grid item xs={12}>
                <TextField
                  fullWidth
                  multiline
                  rows={4}
                  label="Description"
                  value={formData.description}
                  onChange={handleChange('description')}
                  helperText="Provide a detailed description of the project"
                />
              </Grid>

              {/* Owner */}
              <Grid item xs={12}>
                <TextField
                  fullWidth
                  label="Project Owner"
                  value={formData.owner}
                  onChange={handleChange('owner')}
                  error={!!errors.owner}
                  helperText={errors.owner}
                  required
                />
              </Grid>

              {/* Start Date */}
              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  type="date"
                  label="Start Date"
                  value={formData.startDate}
                  onChange={handleChange('startDate')}
                  error={!!errors.startDate}
                  helperText={errors.startDate}
                  InputLabelProps={{ shrink: true }}
                  required
                />
              </Grid>

              {/* End Date */}
              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  type="date"
                  label="End Date"
                  value={formData.endDate}
                  onChange={handleChange('endDate')}
                  error={!!errors.endDate}
                  helperText={errors.endDate}
                  InputLabelProps={{ shrink: true }}
                  required
                />
              </Grid>

              {/* Budget */}
              <Grid item xs={12}>
                <TextField
                  fullWidth
                  type="number"
                  label="Budget"
                  value={formData.budget}
                  onChange={handleChange('budget')}
                  error={!!errors.budget}
                  helperText={errors.budget}
                  InputProps={{
                    startAdornment: <Typography sx={{ mr: 1 }}>$</Typography>
                  }}
                  inputProps={{ min: 0, step: 1000 }}
                  required
                />
              </Grid>

              {/* Actions */}
              <Grid item xs={12}>
                <Box sx={{ display: 'flex', gap: 2, justifyContent: 'flex-end' }}>
                  <Button
                    variant="outlined"
                    onClick={() => navigate(`/projects/${projectId}`)}
                    disabled={saving}
                  >
                    Cancel
                  </Button>
                  <Button
                    type="submit"
                    variant="contained"
                    startIcon={saving ? <CircularProgress size={20} /> : <SaveIcon />}
                    disabled={saving}
                  >
                    {saving ? 'Saving...' : 'Save Changes'}
                  </Button>
                </Box>
              </Grid>
            </Grid>
          </form>
        </CardContent>
      </Card>
    </Container>
  );
};

export default ProjectEdit;
