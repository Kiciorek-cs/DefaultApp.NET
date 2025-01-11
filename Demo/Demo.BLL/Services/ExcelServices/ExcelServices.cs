using System.IO;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Demo.BLL.Interfaces.Services.ExcelServices;

namespace Demo.BLL.Services.ExcelServices;

public class ExcelServices : IExcelServices
{
    private readonly int _lastColumn = 2;

    private static XLColor CustomGreyColor => XLColor.FromHtml("FFD9D9D9");

    public Task<byte[]> GenerateExcel()
    {
        return Task.FromResult(GenerateExcelCommon());
    }

    private byte[] GenerateExcelCommon()
    {
        using var workbook = new XLWorkbook();

        var worksheet = workbook.AddWorksheet();
        var currentRow = 1;

        #region Header

        worksheet.Cell(currentRow, 1).Value = "Lp.";
        worksheet.Cell(currentRow, 1).Value = "Number";

        worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, _lastColumn)).Style.Fill
            .BackgroundColor = CustomGreyColor;
        worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, _lastColumn)).Style.Font.Bold = true;
        worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, _lastColumn)).Style.Border
            .InsideBorder = XLBorderStyleValues.Thin;
        worksheet.Range(worksheet.Cell(currentRow, 1), worksheet.Cell(currentRow, _lastColumn)).Style.Border
            .OutsideBorder = XLBorderStyleValues.Thin;

        worksheet.Cell(currentRow, _lastColumn).Style.Alignment.WrapText = true;

        currentRow++;

        #endregion

        #region Body

        for (var i = 0; i < 100; i++)
        {
            worksheet.Cell(currentRow, 1).Value = i;
            worksheet.Cell(currentRow, 2).Value = i + 2;

            currentRow++;
        }

        #endregion

        worksheet.Columns().AdjustToContents();
        worksheet.Columns().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        worksheet.Columns().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        return stream.ToArray();
    }
}