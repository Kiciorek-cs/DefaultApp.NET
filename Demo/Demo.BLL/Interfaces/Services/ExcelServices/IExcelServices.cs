using System.Threading.Tasks;

namespace Demo.BLL.Interfaces.Services.ExcelServices;

public interface IExcelServices
{
    Task<byte[]> GenerateExcel();
}