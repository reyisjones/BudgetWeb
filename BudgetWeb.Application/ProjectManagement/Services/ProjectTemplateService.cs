using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using BudgetWeb.Domain.Entities;

namespace BudgetWeb.Application.ProjectManagement.Services
{
    public class ProjectTemplateService
    {
        private readonly ILogger<ProjectTemplateService> _logger;
        private readonly string _templatePath;
        private readonly Dictionary<string, ProjectTemplate> _templateCache;

        public ProjectTemplateService(ILogger<ProjectTemplateService> logger)
        {
            _logger = logger;
            _templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration", "ProjectTemplates");
            _templateCache = new Dictionary<string, ProjectTemplate>();
            LoadTemplates();
        }

        private void LoadTemplates()
        {
            try
            {
                if (!Directory.Exists(_templatePath))
                {
                    _logger.LogWarning($"Template directory not found: {_templatePath}");
                    return;
                }

                var templateFiles = Directory.GetFiles(_templatePath, "*.json");
                foreach (var file in templateFiles)
                {
                    try
                    {
                        var json = File.ReadAllText(file);
                        var template = JsonSerializer.Deserialize<ProjectTemplate>(json, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        if (template != null)
                        {
                            _templateCache[template.ProjectType] = template;
                            _logger.LogInformation($"Loaded template: {template.DisplayName}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to load template from {file}");
                    }
                }

                _logger.LogInformation($"Loaded {_templateCache.Count} project templates");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load project templates");
            }
        }

        public IEnumerable<ProjectTemplateSummary> GetAvailableTemplates()
        {
            return _templateCache.Values.Select(t => new ProjectTemplateSummary
            {
                ProjectType = t.ProjectType,
                DisplayName = t.DisplayName,
                Description = t.Description,
                Icon = t.Icon
            });
        }

        public ProjectTemplate? GetTemplate(string projectType)
        {
            if (_templateCache.TryGetValue(projectType, out var template))
            {
                return template;
            }

            _logger.LogWarning($"Template not found for project type: {projectType}");
            return null;
        }

        public Project CreateProjectFromTemplate(string projectType, CreateProjectFromTemplateRequest request)
        {
            var template = GetTemplate(projectType);
            if (template == null)
            {
                throw new ArgumentException($"Template not found for project type: {projectType}");
            }

            var project = new Project
            {
                Name = request.Name,
                Type = projectType,
                Description = request.Description ?? template.Description,
                Owner = request.Owner,
                StartDate = request.StartDate,
                Budget = request.Budget,
                Status = "Planning",
                CreatedDate = DateTime.UtcNow,
                Phases = new List<ProjectPhase>()
            };

            // Calculate total planned duration and budget percentage
            var totalDays = template.DefaultPhases.Sum(p => p.PlannedDuration);
            var totalBudgetPercentage = template.DefaultPhases.Sum(p => p.BudgetPercentage);

            // Create phases from template
            var currentStartDate = request.StartDate;
            foreach (var phaseTemplate in template.DefaultPhases)
            {
                var phase = new ProjectPhase
                {
                    Name = phaseTemplate.Name,
                    Description = phaseTemplate.Description,
                    StartDate = currentStartDate,
                    PlannedDuration = phaseTemplate.PlannedDuration,
                    Budget = (request.Budget * phaseTemplate.BudgetPercentage / 100.0m),
                    Status = "NotStarted",
                    Tasks = new List<ProjectTask>()
                };

                // Create tasks from template
                if (request.IncludeDefaultTasks && phaseTemplate.DefaultTasks != null)
                {
                    foreach (var taskTemplate in phaseTemplate.DefaultTasks)
                    {
                        var task = new ProjectTask
                        {
                            Name = taskTemplate.Name,
                            Priority = taskTemplate.Priority,
                            Status = "NotStarted",
                            PlannedHours = taskTemplate.PlannedHours,
                            StartDate = phase.StartDate,
                            DueDate = phase.StartDate.AddDays(phaseTemplate.PlannedDuration)
                        };
                        phase.Tasks.Add(task);
                    }
                }

                project.Phases.Add(phase);
                currentStartDate = currentStartDate.AddDays(phaseTemplate.PlannedDuration);
            }

            // Set project end date based on all phases
            project.EndDate = currentStartDate;

            // Add default inventory items if requested
            if (request.IncludeDefaultInventory && template.DefaultInventoryItems != null)
            {
                project.InventoryItems = template.DefaultInventoryItems.Select(item => new InventoryItem
                {
                    Name = item.Name,
                    Category = item.Category,
                    Unit = item.Unit,
                    QuantityOnHand = 0,
                    ReorderPoint = item.ReorderPoint,
                    UnitCost = 0
                }).ToList();
            }

            _logger.LogInformation($"Created project '{request.Name}' from template '{template.DisplayName}'");
            return project;
        }

        public void ReloadTemplates()
        {
            _templateCache.Clear();
            LoadTemplates();
        }
    }

    // Template models
    public class ProjectTemplate
    {
        public string ProjectType { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public List<PhaseTemplate> DefaultPhases { get; set; } = new();
        public List<BudgetCategoryTemplate> DefaultBudgetCategories { get; set; } = new();
        public List<ResourceTypeTemplate> DefaultResourceTypes { get; set; } = new();
        public List<InventoryItemTemplate> DefaultInventoryItems { get; set; } = new();
        public List<string> DefaultContractTypes { get; set; } = new();
        public string ReportingFrequency { get; set; } = string.Empty;
        public List<string> CriticalMetrics { get; set; } = new();
    }

    public class PhaseTemplate
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PlannedDuration { get; set; }
        public decimal BudgetPercentage { get; set; }
        public List<TaskTemplate>? DefaultTasks { get; set; }
    }

    public class TaskTemplate
    {
        public string Name { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public decimal PlannedHours { get; set; }
    }

    public class BudgetCategoryTemplate
    {
        public string Name { get; set; } = string.Empty;
        public decimal Percentage { get; set; }
        public List<string>? Subcategories { get; set; }
    }

    public class ResourceTypeTemplate
    {
        public string Type { get; set; } = string.Empty;
        public List<string> Examples { get; set; } = new();
    }

    public class InventoryItemTemplate
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public int ReorderPoint { get; set; }
    }

    public class ProjectTemplateSummary
    {
        public string ProjectType { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }

    public class CreateProjectFromTemplateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public decimal Budget { get; set; }
        public bool IncludeDefaultTasks { get; set; } = true;
        public bool IncludeDefaultInventory { get; set; } = true;
    }
}
