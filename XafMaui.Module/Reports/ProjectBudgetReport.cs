#pragma warning disable CA1416
using DevExpress.XtraReports.UI;
using System.Drawing;

namespace XafMaui.Module.Reports;

public class ProjectBudgetReport : XtraReport
{
    public ProjectBudgetReport()
    {
        Landscape = true;

        var pageHeader = new PageHeaderBand { HeightF = 65 };
        pageHeader.Controls.Add(new XRLabel
        {
            Text = "Project Budget Report",
            BoundsF = new RectangleF(0, 0, 800, 30),
            Font = new Font("Arial", 14, FontStyle.Bold),
        });

        float y = 40;
        AddHeaderLabel(pageHeader, "Project", 0, 180, y);
        AddHeaderLabel(pageHeader, "Client", 185, 130, y);
        AddHeaderLabel(pageHeader, "Status", 320, 70, y);
        AddHeaderLabel(pageHeader, "Budget (h)", 395, 80, y);
        AddHeaderLabel(pageHeader, "Actual (h)", 480, 80, y);
        AddHeaderLabel(pageHeader, "Remaining", 565, 80, y);
        Bands.Add(pageHeader);

        var detail = new DetailBand { HeightF = 20 };
        AddLabel(detail, "[Name]", 0, 180);
        AddLabel(detail, "[Client.Name]", 185, 130);
        AddLabel(detail, "[Status]", 320, 70);
        AddLabel(detail, "FormatString('{0:F1}', [BudgetHours])", 395, 80);
        Bands.Add(detail);
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

    static void AddHeaderLabel(Band band, string text, float x, float width, float y)
    {
        band.Controls.Add(new XRLabel
        {
            Text = text,
            BoundsF = new RectangleF(x, y, width, 20),
            Font = new Font("Arial", 9, FontStyle.Bold),
            BackColor = Color.LightGray
        });
    }
}
