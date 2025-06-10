using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp1.Keys
{
    public class KeyParser
    {
        public List<Key> Keys { get; private set; } = new List<Key>();
        private readonly string _filePath;

        public KeyParser() : this(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Keys", "keys.xlsx"))
        {

        }

        public KeyParser(string filePath)
        {
            _filePath = filePath;
            Keys.AddRange(ParseFromFile());
        }

        public List<Key> ParseFromFile()
        {
            const int startRow = 1;
            var keys = new List<Key>();

            using (var fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fileStream);
                ISheet worksheet = workbook.GetSheetAt(0);

                if (worksheet == null || worksheet.LastRowNum < startRow)
                    return keys;

                for (int row = startRow; row <= worksheet.LastRowNum; row++)
                {
                    IRow currentRow = worksheet.GetRow(row);
                    if (currentRow == null) continue;

                    keys.Add(new Key
                    {
                        Id = currentRow.GetCell(0)?.StringCellValue,
                        Link = currentRow.GetCell(1)?.StringCellValue,
                        IsActive = currentRow.GetCell(2)?.StringCellValue == "Active"
                    });
                }
            }

            return keys;
        }
    }
}