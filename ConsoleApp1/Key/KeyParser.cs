using System;
using System.Collections.Generic;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace ConsoleApp1.Keys
{
    public class KeyParser
    {
        public List<Key> keys;
        private static string FilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Keys", "keys.xlsx");

        public KeyParser()
        {
            keys = ParseFile();
        }

        public static List<Key> ParseFile()
        {
            int startPosition = 1; // Игнорируем нулевую строку с заголовками столбцов
            var keys = new List<Key>();

            using (FileStream fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.ReadWrite))
            using (IWorkbook workbook = new XSSFWorkbook(fileStream)) 
            {
                ChangeHeaderOfFile(workbook);
                ISheet sheet = workbook.GetSheetAt(0);

                for (int rowIndex = startPosition; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    var row = sheet.GetRow(rowIndex);
                    if (IsEmptyRow(row)) break;

                    keys.Add(new Key
                    {
                        Id = row.GetCell(0)?.ToString() ?? string.Empty,
                        Link = row.GetCell(1)?.ToString() ?? string.Empty,
                        IsActive = (row.GetCell(2)?.ToString() ?? string.Empty) == "Active"
                    });
                }
            }

            return keys;
        }

        public static bool IsEmptyRow(IRow row)
        {
            if (row == null) return true;

            foreach (ICell cell in row.Cells)
            {
                if (cell != null && !string.IsNullOrWhiteSpace(cell.ToString()))
                    return false;
            }

            return true;
        }

        public static void ChangeHeaderOfFile(IWorkbook workbook)
        {
            var sheet = workbook?.GetSheetAt(0);
            var row = sheet?.GetRow(0) ?? sheet?.CreateRow(0);
            var cell = row?.GetCell(3) ?? row?.CreateCell(3);
            cell?.SetCellValue("Employee");
        }
    }
}