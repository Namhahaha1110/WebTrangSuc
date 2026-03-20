using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using SportsStore.Models;

[Route("api/[controller]")]
[ApiController]
public class ExportController : ControllerBase
{
    private readonly IOrderRepository _repo;

    public ExportController(IOrderRepository repo)
    {
        _repo = repo;
    }

    [HttpGet("orders")]
    public IActionResult ExportOrders()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        var orders = _repo.Orders.ToList();

        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Orders");

        ws.Cells[1, 1].Value = "OrderID";
        ws.Cells[1, 2].Value = "Name";
        ws.Cells[1, 3].Value = "Phone";
        ws.Cells[1, 4].Value = "Address";
        ws.Cells[1, 5].Value = "Total";
        ws.Cells[1, 6].Value = "Shipped";

        int row = 2;
        foreach (var o in orders)
        {
            ws.Cells[row, 1].Value = o.OrderID;
            ws.Cells[row, 2].Value = o.Name;
            ws.Cells[row, 3].Value = o.Phone;
            ws.Cells[row, 4].Value = o.AddressDetail;
            ws.Cells[row, 5].Value = o.TotalPrice;
            ws.Cells[row, 6].Value = o.Shipped ? "Yes" : "No";
            row++;
        }

        ws.Cells.AutoFitColumns();

        var bytes = package.GetAsByteArray();

        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "Orders.xlsx");
    }
}
