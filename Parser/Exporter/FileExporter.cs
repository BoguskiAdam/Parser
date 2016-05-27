using System;
using System.IO;
using Parser.Models;
using System.Reflection;

namespace Parser.Exporter
{
    public class FileExporter
    {
        string path = string.Empty;
        ConsoleColor normalColor;
        ConsoleColor redColor;
        public FileExporter(string filePath)
        {
            normalColor = Console.ForegroundColor;
            redColor = ConsoleColor.Red;
            path = filePath;
            if (File.Exists(filePath))
                File.Delete(filePath);
            using (StreamWriter f = File.AppendText(path))
            {
                string line = string.Empty;
                foreach (var column in ExcelExporter.ColumnNames)
                {
                    line += column + ";";
                }
                line = line.Substring(0, line.Length - 2);
                f.WriteLine(line);
            }
        }

        public void ExportLine(string line)
        {
            bool boolean = true;
            while (boolean)
            {
                try
                {
                    using (StreamWriter file = File.AppendText(path))
                    {
                        file.WriteLine(line);
                        boolean = false;
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    Console.ForegroundColor = redColor;
                    Console.WriteLine("Plik jest używany przez inny proces.");
                    Console.WriteLine("Wcisnij coś, aby sprobowac jeszcze raz");
                    Console.ForegroundColor = normalColor;
                    Console.ReadKey();
                }
                catch (DirectoryNotFoundException)
                {
                    Console.ForegroundColor = redColor;
                    Console.WriteLine("Nie ma pliku.");
                    Console.WriteLine("Wcisnij coś, aby sprobowac jeszcze raz");
                    Console.ForegroundColor = normalColor;
                    Console.ReadKey();
                }
                catch (IOException)
                {
                    Console.ForegroundColor = redColor;
                    Console.WriteLine("Plik jest używany przez inny proces.");
                    Console.WriteLine("Wcisnij coś, aby sprobowac jeszcze raz");
                    Console.ForegroundColor = normalColor;
                    Console.ReadKey();
                }

            }
        }

        internal void ExportHolidayModel(HolidayModel model)
        {
            string result = string.Empty;
            Type itemType = model.GetType();
            PropertyInfo[] properties = itemType.GetProperties();
            ExcelOutputModel newExcelOutputModel = new ExcelOutputModel();
            foreach (PropertyInfo property in properties)
            {
                string propertyName = property.Name;
                string propertyValue = string.Empty;
                var propertyVarValue = property.GetValue(model, null);
                result += propertyVarValue.ToString()+";";
                //newExcelOutputModel.Row.Add(propertyVarValue.ToString());
            }
            result = result.Substring(0, result.Length - 2);
            ExportLine(result);
        }
    }
}
