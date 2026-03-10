#pragma warning disable CA1416
using DevExpress.XtraReports.UI;
using System.Drawing;

namespace XafMaui.Module.Reports;

public class WeeklyTimesheetReport : XtraReport
{
    public WeeklyTimesheetReport()
    {
        Landscape = false;

        var pageHeader = new PageHeaderBand { HeightF = 40 };
        pageHeader.Controls.Add(new XRLabel
        {
            Text = "Weekly Timesheet",
            BoundsF = new RectangleF(0, 0, 650, 30),
            Font = new Font("Arial", 14, FontStyle.Bold),
        });
        Bands.Add(pageHeader);

        var groupHeader = new GroupHeaderBand { HeightF = 25 };
        groupHeader.GroupFields.Add(new GroupField("Date"));
        groupHeader.Controls.Add(new XRLabel
        {
            ExpressionBindings = { new ExpressionBinding("Text", "FormatString('{0:ddd dd MMM yyyy}', [Date])") },
            BoundsF = new RectangleF(0, 0, 650, 25),
            Font = new Font("Arial", 10, FontStyle.Bold),
            BackColor = Color.LightGray
        });
        Bands.Add(groupHeader);

        var detail = new DetailBand { HeightF = 20 };
        AddLabel(detail, "[ProjectTask.Project.Name]", 0, 180);
        AddLabel(detail, "[ProjectTask.Name]", 185, 160);
        AddLabel(detail, "FormatString('{0:F1}', [Hours])", 350, 50);
        AddLabel(detail, "[Status]", 405, 70);
        AddLabel(detail, "[User.UserName]", 480, 100);
        Bands.Add(detail);

        var groupFooter = new GroupFooterBand { HeightF = 25 };
        groupFooter.Controls.Add(new XRLabel
        {
            ExpressionBindings = { new ExpressionBinding("Text", "'Day Total: ' + FormatString('{0:F1}h', sumSum([Hours]))") },
            BoundsF = new RectangleF(300, 0, 150, 25),
            Font = new Font("Arial", 9, FontStyle.Bold)
        });
        Bands.Add(groupFooter);

        var reportFooter = new ReportFooterBand { HeightF = 30 };
        reportFooter.Controls.Add(new XRLabel
        {
            ExpressionBindings = { new ExpressionBinding("Text", "'Grand Total: ' + FormatString('{0:F1}h', sumSum([Hours]))") },
            BoundsF = new RectangleF(300, 5, 200, 25),
            Font = new Font("Arial", 11, FontStyle.Bold)
        });
        Bands.Add(reportFooter);
    }

    static void AddLabel(DetailBand band, string expression, float x, float width)
    {
        band.Controls.Add(new XRLabel
        {
            ExpressionBindings = { new ExpressionBinding("Text", expression) },
            BoundsF = new RectangleF(x, 0, width, 20),
            Font = new Font("Arial", 9)
        });
    }
}
