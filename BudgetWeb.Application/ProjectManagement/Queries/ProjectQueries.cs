using System;
using System.Collections.Generic;
using BudgetWeb.Application.ProjectManagement.DTOs;

namespace BudgetWeb.Application.ProjectManagement.Queries
{
    // ==================== PROJECT QUERIES ====================

    public class GetAllProjectsQuery
    {
        public string? Status { get; set; }
        public string? ProjectType { get; set; }
        public string? Owner { get; set; }
    }

    public class GetProjectByIdQuery
    {
        public Guid Id { get; set; }
    }

    public class GetProjectDetailQuery
    {
        public Guid Id { get; set; }
    }

    // ==================== PHASE QUERIES ====================

    public class GetPhasesByProjectQuery
    {
        public Guid ProjectId { get; set; }
    }

    public class GetPhaseByIdQuery
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
    }

    // ==================== TASK QUERIES ====================

    public class GetTasksByProjectQuery
    {
        public Guid ProjectId { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
    }

    public class GetTasksByPhaseQuery
    {
        public Guid ProjectId { get; set; }
        public Guid PhaseId { get; set; }
    }

    public class GetTaskByIdQuery
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
    }

    public class GetTasksByAssigneeQuery
    {
        public Guid ProjectId { get; set; }
        public string Assignee { get; set; } = string.Empty;
    }

    // ==================== EXPENSE QUERIES ====================

    public class GetExpensesByProjectQuery
    {
        public Guid ProjectId { get; set; }
        public string? Category { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class GetExpenseByIdQuery
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
    }

    // ==================== RESOURCE QUERIES ====================

    public class GetResourceAllocationsByProjectQuery
    {
        public Guid ProjectId { get; set; }
        public string? ResourceType { get; set; }
    }

    public class GetResourceAllocationsByTaskQuery
    {
        public Guid ProjectId { get; set; }
        public Guid TaskId { get; set; }
    }

    // ==================== INVENTORY QUERIES ====================

    public class GetInventoryByProjectQuery
    {
        public Guid ProjectId { get; set; }
        public bool? BelowReorderPoint { get; set; }
    }

    public class GetInventoryItemByIdQuery
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
    }

    // ==================== CONTRACT QUERIES ====================

    public class GetContractsByProjectQuery
    {
        public Guid ProjectId { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
    }

    public class GetContractByIdQuery
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
    }

    // ==================== REPORT QUERIES ====================

    public class GetProgressReportQuery
    {
        public Guid ProjectId { get; set; }
    }

    public class GetCostAnalysisReportQuery
    {
        public Guid ProjectId { get; set; }
    }

    public class GetResourceUtilizationReportQuery
    {
        public Guid ProjectId { get; set; }
    }

    public class GetProjectDashboardQuery
    {
        public Guid ProjectId { get; set; }
    }

    // ==================== RESPONSE MODELS ====================

    public class ProjectDashboardDto
    {
        public ProjectDetailDto Project { get; set; } = null!;
        public ProgressReportDto ProgressReport { get; set; } = null!;
        public CostAnalysisReportDto CostAnalysisReport { get; set; } = null!;
        public ResourceUtilizationReportDto ResourceUtilizationReport { get; set; } = null!;
        public List<ProjectTaskDto> UpcomingTasks { get; set; } = new();
        public List<ProjectTaskDto> BlockedTasks { get; set; } = new();
        public List<InventoryItemDto> LowInventoryItems { get; set; } = new();
    }
}
