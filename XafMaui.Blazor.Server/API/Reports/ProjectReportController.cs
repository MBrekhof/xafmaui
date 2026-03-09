#pragma warning disable CA1416 // Platform compatibility - DevExpress.Drawing.Skia handles cross-platform rendering
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XafMaui.Module.BusinessObjects;

namespace XafMaui.Blazor.Server.API.Reports;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ProjectReportController : ControllerBase
{
    readonly IObjectSpaceFactory _objectSpaceFactory;

    public ProjectReportController(IObjectSpaceFactory objectSpaceFactory)
    {
        _objectSpaceFactory = objectSpaceFactory;
    }

    [HttpGet("WeeklyTimesheet")]
    public IActionResult WeeklyTimesheet([FromQuery] DateTime? weekStart = null)
    {
        var start = weekStart ?? StartOfWeek(DateTime.Today);
        var end = start.AddDays(7);

        using var os = _objectSpaceFactory.CreateObjectSpace(typeof(TimeEntry));
        var entries = os.GetObjectsQuery<TimeEntry>()
            .Include(e => e.ProjectTask).ThenInclude(t => t!.Project)
            .Include(e => e.User)
            .Where(e => e.Date >= start && e.Date < end)
            .OrderBy(e => e.Date).ThenBy(e => e.ProjectTask!.Project!.Name)
            .ToList();

        var data = entries.Select(e => new
        {
            Date = e.Date.ToString("ddd dd MMM"),
            Project = e.ProjectTask?.Project?.Name ?? "—",
            Task = e.ProjectTask?.Name ?? "—",
            e.Hours,
            Status = e.Status.ToString(),
            Note = e.Note ?? "",
            Consultant = e.User?.UserName ?? "—"
        }).ToList();

        var report = new XtraReport { Landscape = false };
        report.DataSource = data;

        var pageHeader = new PageHeaderBand { HeightF = 40 };
        pageHeader.Controls.Add(new XRLabel
        {
            Text = $"Weekly Timesheet: {start:dd MMM yyyy} – {end.AddDays(-1):dd MMM yyyy}",
            BoundsF = new System.Drawing.RectangleF(0, 0, 650, 30),
            Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold),
        });
        report.Bands.Add(pageHeader);

        var groupHeader = new GroupHeaderBand { HeightF = 25, GroupFields = { new GroupField("Date") } };
        groupHeader.Controls.Add(new XRLabel
        {
            ExpressionBindings = { new ExpressionBinding("Text", "[Date]") },
            BoundsF = new System.Drawing.RectangleF(0, 0, 200, 25),
            Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold),
            BackColor = System.Drawing.Color.LightGray
        });
        report.Bands.Add(groupHeader);

        var detail = new DetailBand { HeightF = 20 };
        AddLabel(detail, "[Project]", 0, 180);
        AddLabel(detail, "[Task]", 185, 160);
        AddLabel(detail, "[Hours]", 350, 50);
        AddLabel(detail, "[Status]", 405, 70);
        AddLabel(detail, "[Consultant]", 480, 100);
        report.Bands.Add(detail);

        var groupFooter = new GroupFooterBand { HeightF = 25 };
        var totalLabel = new XRLabel
        {
            ExpressionBindings = { new ExpressionBinding("Text", "'Day Total: ' + FormatString('{0:F1}h', sumSum([Hours]))") },
            BoundsF = new System.Drawing.RectangleF(300, 0, 150, 25),
            Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold)
        };
        groupFooter.Controls.Add(totalLabel);
        report.Bands.Add(groupFooter);

        var reportFooter = new ReportFooterBand { HeightF = 30 };
        reportFooter.Controls.Add(new XRLabel
        {
            ExpressionBindings = { new ExpressionBinding("Text", "'Grand Total: ' + FormatString('{0:F1}h', sumSum([Hours]))") },
            BoundsF = new System.Drawing.RectangleF(300, 5, 200, 25),
            Font = new System.Drawing.Font("Arial", 11, System.Drawing.FontStyle.Bold)
        });
        report.Bands.Add(reportFooter);

        return ExportToPdf(report, $"Weekly Timesheet {start:yyyy-MM-dd}");
    }

    [HttpGet("ProjectBudget")]
    public IActionResult ProjectBudget()
    {
        using var os = _objectSpaceFactory.CreateObjectSpace(typeof(Project));
        var projects = os.GetObjectsQuery<Project>()
            .Include(p => p.Client)
            .Include(p => p.ProjectTasks).ThenInclude(t => t.TimeEntries)
            .Where(p => p.Status == ProjectStatus.Active || p.Status == ProjectStatus.OnHold)
            .OrderBy(p => p.Name)
            .ToList();

        var data = projects.Select(p => new
        {
            p.Name,
            Client = p.Client?.Name ?? "—",
            Status = p.Status.ToString(),
            Budget = p.BudgetHours,
            Actual = p.ProjectTasks.SelectMany(t => t.TimeEntries).Sum(e => e.Hours),
            Remaining = p.BudgetHours - p.ProjectTasks.SelectMany(t => t.TimeEntries).Sum(e => e.Hours),
            BurnPct = p.BudgetHours > 0
                ? Math.Round(p.ProjectTasks.SelectMany(t => t.TimeEntries).Sum(e => e.Hours) / p.BudgetHours * 100, 1)
                : 0m
        }).ToList();

        var report = new XtraReport { Landscape = true };
        report.DataSource = data;

        var pageHeader = new PageHeaderBand { HeightF = 40 };
        pageHeader.Controls.Add(new XRLabel
        {
            Text = $"Project Budget Report — {DateTime.Today:dd MMM yyyy}",
            BoundsF = new System.Drawing.RectangleF(0, 0, 800, 30),
            Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold),
        });
        report.Bands.Add(pageHeader);

        // Column headers
        var colHeader = new PageHeaderBand { HeightF = 25 };
        // XtraReports only allows one PageHeaderBand - merge headers into same band
        pageHeader.HeightF = 65;
        float y = 40;
        AddHeaderLabel(pageHeader, "Project", 0, 180, y);
        AddHeaderLabel(pageHeader, "Client", 185, 130, y);
        AddHeaderLabel(pageHeader, "Status", 320, 70, y);
        AddHeaderLabel(pageHeader, "Budget (h)", 395, 80, y);
        AddHeaderLabel(pageHeader, "Actual (h)", 480, 80, y);
        AddHeaderLabel(pageHeader, "Remaining", 565, 80, y);
        AddHeaderLabel(pageHeader, "Burn %", 650, 60, y);

        var detail = new DetailBand { HeightF = 20 };
        AddLabel(detail, "[Name]", 0, 180);
        AddLabel(detail, "[Client]", 185, 130);
        AddLabel(detail, "[Status]", 320, 70);
        AddLabel(detail, "FormatString('{0:F1}', [Budget])", 395, 80);
        AddLabel(detail, "FormatString('{0:F1}', [Actual])", 480, 80);
        AddLabel(detail, "FormatString('{0:F1}', [Remaining])", 565, 80);
        AddLabel(detail, "FormatString('{0:F1}%', [BurnPct])", 650, 60);
        report.Bands.Add(detail);

        return ExportToPdf(report, "Project Budget Report");
    }

    static void AddLabel(DetailBand band, string expression, float x, float width)
    {
        var label = new XRLabel
        {
            ExpressionBindings = { new ExpressionBinding("Text", expression) },
            BoundsF = new System.Drawing.RectangleF(x, 0, width, 20),
            Font = new System.Drawing.Font("Arial", 9)
        };
        band.Controls.Add(label);
    }

    static void AddHeaderLabel(Band band, string text, float x, float width, float y)
    {
        band.Controls.Add(new XRLabel
        {
            Text = text,
            BoundsF = new System.Drawing.RectangleF(x, y, width, 20),
            Font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Bold),
            BackColor = System.Drawing.Color.LightGray
        });
    }

    IActionResult ExportToPdf(XtraReport report, string filename)
    {
        using var ms = new MemoryStream();
        report.ExportToPdf(ms);
        ms.Position = 0;
        return File(ms.ToArray(), "application/pdf", $"{filename}.pdf");
    }

    static DateTime StartOfWeek(DateTime dt)
    {
        int diff = (7 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
        return dt.AddDays(-diff).Date;
    }
}
