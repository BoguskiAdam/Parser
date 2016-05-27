using Parser.Exporter;
using Parser.Models;
using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace Parser
{
    public class TvParser
    {
        public void ParseTvDay(string day, bool first = false, FileExporter fileExporter = null)
        {
            WebClient tvWebClient = new WebClient();
            string respondedString = tvWebClient.DownloadString(Settings.TvProgramSiteUrl + Settings.TvProgramDayPrefix + day);
            int indexOfStart;
            int indexOfEnd;
            string listElementString = string.Empty;
            string parsedUrl = string.Empty;
            respondedString = respondedString.Substring(respondedString.IndexOf("\"progFiltry\""));
            indexOfStart = respondedString.IndexOf("<ul>");
            indexOfEnd = respondedString.IndexOf("</ul>", indexOfStart);
            if (indexOfStart != -1 && indexOfEnd != -1)
            {
                respondedString = respondedString.Substring(indexOfStart, indexOfEnd + 5 - indexOfStart);
                indexOfStart = respondedString.IndexOf("<li");
                do
                {
                    indexOfEnd = respondedString.IndexOf("</li>");
                    TvProgramModel tempModel = new TvProgramModel();
                    if (indexOfEnd < 15)
                        continue;
                    listElementString = respondedString.Substring(0, indexOfEnd += 5);
                    respondedString = respondedString.Substring(indexOfEnd);
                    ParseListElement(listElementString, ref tempModel);

                    if (tempModel.Url == null)
                        continue;
                    Console.WriteLine($"Parsing program url: {tempModel.Url}");
                    tempModel.Date = day;
                    ParseTvProgram(ref tempModel);

                    int index = ProgramsHolder.ProgramModel.Count();
                    if (!first)
                    {
                        TvProgramModel model = ProgramsHolder.ProgramModel.ElementAt(index - 1);
                        model.Ends = tempModel.Starts;
                        model.Duration = model.Ends.TotalMinutes - model.Starts.TotalMinutes;
                        if (model.Duration < 0)
                            model.Duration += 60 * 24;
                        fileExporter.ExportLine(GetLineByModel(ProgramsHolder.ProgramModel.ElementAt(index - 1)));
                    }
                    first = false;

                    ProgramsHolder.AddProgram(tempModel);
                    indexOfStart = indexOfEnd;
                } while (indexOfEnd != -1);
                fileExporter.ExportLine(GetLineByModel(ProgramsHolder.ProgramModel.Last()));
                AddEndsTime();
            }
        }

        private void AddEndsTime()
        {
            int programCount = ProgramsHolder.ProgramModel.Count;
            for (int i = 0; i < programCount; i++)
            {
                TvProgramModel tvDayModel = ProgramsHolder.ProgramModel.ElementAt(i);
                if (i == programCount - 1)
                    tvDayModel.Ends = new TimeSpan(6, 0, 0);
                else
                    tvDayModel.Ends = ProgramsHolder.ProgramModel.ElementAt(i + 1).Starts;
                tvDayModel.Duration = tvDayModel.Ends.TotalMinutes - tvDayModel.Starts.TotalMinutes;
                if (tvDayModel.Duration < 0)
                    tvDayModel.Duration += 60 * 24;
            }

        }

        private string GetLineByModel(TvProgramModel model)
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
                result +=propertyVarValue.ToString() + ";";
            }
            result = result.Substring(0, result.Length - 2);
            return result;
        }

        private void ParseListElement(string text, ref TvProgramModel tvProgramModel)
        {
            int indexOfStart = text.IndexOf("<a href=\"");
            int indexOfEnd;
            string result = string.Empty;
            if (indexOfStart != -1)
            {
                indexOfEnd = text.IndexOf("\">", indexOfStart);
                result = Settings.TvProgramSiteUrl + text.Substring(indexOfStart + 10, indexOfEnd - indexOfStart - 10);
                tvProgramModel.Url = result;
            }
            indexOfStart = text.IndexOf("<em>");
            if (indexOfStart != -1)
            {
                indexOfEnd = text.IndexOf("</em>");
                result = text.Substring(indexOfStart + 4, indexOfEnd - indexOfStart - 4);
                result = result.Trim(' ');
                tvProgramModel.Starts = TimeConverter.StringToTimeSpan(result);
            }
        }

        private void ParseTvProgram(ref TvProgramModel tvProgramModel)
        {
            //TvProgramModel tvProgramModel = new TvProgramModel();

            string respondedString = string.Empty;
            using (WebClient tvDayWebClient = new WebClient())
            {
                tvDayWebClient.Encoding = Encoding.UTF8;
                respondedString = tvDayWebClient.DownloadString(tvProgramModel.Url);
            }


            string searchedString = "id=\"audycja\" class=\"column\">";
            string ageCategorySearchedString = "<h1 class=\"";
            string programNameSearchedString = "content=\"";
            string categorySearchedString = "itemprop=\"genre\" content=\"";
            string result = string.Empty;
            int indexOfStart = respondedString.IndexOf(searchedString);
            int indexOfEnd = 0;
            if (indexOfStart >= 0)
            {
                respondedString = respondedString.Substring(indexOfStart + searchedString.Length + 1);

                //Age category
                indexOfStart = respondedString.IndexOf(ageCategorySearchedString, indexOfEnd) + ageCategorySearchedString.Length;
                indexOfEnd = respondedString.IndexOf("\">", indexOfStart);
                result = respondedString.Substring(indexOfStart, indexOfEnd - indexOfStart);
                AgeCategoryEnum ageCategoryEnum = GetAgeCategoryEnum(result);
                tvProgramModel.AgeCategory = ageCategoryEnum;

                //Program name
                indexOfStart = respondedString.IndexOf(programNameSearchedString, indexOfEnd) + programNameSearchedString.Length;
                indexOfEnd = respondedString.IndexOf("\"", indexOfStart);
                result = respondedString.Substring(indexOfStart, indexOfEnd - indexOfStart);
                tvProgramModel.Name = result.Replace("&quot;", "\"");
                tvProgramModel.Name = result.Replace("&#039;", "'");
                tvProgramModel.Name = result.Replace(";", "");

                //Category
                indexOfStart = respondedString.IndexOf(categorySearchedString, indexOfEnd);
                if (indexOfStart == -1)
                {
                    result = "No category";
                }
                else
                {
                    indexOfStart += categorySearchedString.Length;
                    indexOfEnd = respondedString.IndexOf("\"", indexOfStart);
                    result = respondedString.Substring(indexOfStart, indexOfEnd - indexOfStart);
                }
                tvProgramModel.Category = result;
            }
        }

        private AgeCategoryEnum GetAgeCategoryEnum(string result)
        {
            switch (result)
            {
                case "wiek0":
                    return AgeCategoryEnum.Zero;
                case "wiek1":
                    return AgeCategoryEnum.One;
                case "wiek7":
                    return AgeCategoryEnum.Seven;
                case "wiek8":
                    return AgeCategoryEnum.Eight;
                case "wiek9":
                    return AgeCategoryEnum.Nine;
                case "wiek10":
                    return AgeCategoryEnum.Ten;
                case "wiek11":
                    return AgeCategoryEnum.Eleven;
                case "wiek12":
                    return AgeCategoryEnum.Twelve;
                case "wiek13":
                    return AgeCategoryEnum.Thirteen;
                case "wiek14":
                    return AgeCategoryEnum.Fourteen;
                case "wiek15":
                    return AgeCategoryEnum.Fifteen;
                case "wiek16":
                    return AgeCategoryEnum.Eighteen;
                case "wiek18":
                    return AgeCategoryEnum.Eighteen;
                default:
                    return AgeCategoryEnum.NoCategory;
            }
        }
    }
}
