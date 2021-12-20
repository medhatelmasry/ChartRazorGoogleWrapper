using ChartRazorGoogleWrapper.NW;
using Google.DataTable.Net.Wrapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ChartRazorGoogleWrapper.Pages;

public class ChartDataModel : PageModel
{
    private readonly ILogger<ChartDataModel> _logger;
    private readonly NorthwindContext _northwindContext;

    public ChartDataModel(ILogger<ChartDataModel> logger, NorthwindContext northwindContext)
    {
        _logger = logger;
        _northwindContext = northwindContext;
    }

    public async Task<IActionResult> OnGet()
    {
        var data = await _northwindContext.Orders
        .GroupBy(_ => _.ShipCity)
        .Select(g => new
        {
            Name = g.Key,
            Count = g.Count()
        })
        .OrderByDescending(cp => cp.Count)
        .ToListAsync();

        //let's instantiate the DataTable.
        var dt = new Google.DataTable.Net.Wrapper.DataTable();
        dt.AddColumn(new Column(ColumnType.String, "Name", "Name"));
        dt.AddColumn(new Column(ColumnType.Number, "Count", "Count"));

        foreach (var item in data)
        {
            Row r = dt.NewRow();
            r.AddCellRange(new Cell[]
            {
                new Cell(item.Name),
                new Cell(item.Count)
            });
            dt.AddRow(r);
        }

        //Let's create a Json string as expected by the Google Charts API.
        return Content(dt.GetJson());
    }
}