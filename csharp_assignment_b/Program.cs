using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace csharp_assignment_b
{
    class Program
    {
        //definisanje lokacije gde ce slika biti sacuvana, u ovom slucaju desktop
        private static string Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\dataPieChart.png";

        static async Task Main(string[] args)
        {
            Console.Title = "Create Pie Chart";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync("site.com{key}");

                string responseString = await response.Content.ReadAsStringAsync();
                List<EmployeeInfo> employees = JsonSerializer.Deserialize<List<EmployeeInfo>>(responseString);

                var employeeHours = employees.GroupBy(e => e.EmployeeName)
                                     .Select(g => new { EmployeeName = g.Key, TotalHours = g.Sum(e => (e.EndTimeUtc - e.StarTimeUtc).TotalHours) });
                
                Chart chart = new Chart();                      //definisanje chart-a
                chart.Palette = ChartColorPalette.Pastel;
                chart.Width = 800;
                chart.Height = 600;

                ChartArea chartArea = new ChartArea();          //definisanje podeoka chart-a
                chart.ChartAreas.Add(chartArea);
                
                Series series = new Series();                   //stilizovanje chart-a
                series.ChartType = SeriesChartType.Pie;

                series.CustomProperties = "PieLabelStyle=Disabled, PieDrawingStyle=SoftEdge";
                series.Font = new Font("Arial", 8f);
                series["PieLabelStyle"] = "Outside";
                series["PieLineColor"] = "Black";
                series["PieLabelStyleOutsideLineColor"] = "Black";
                series["PieLabelStyleOutsideBackColor"] = "White";
                series["PieLabelStyleInsideTextOrientation"] = "Radial";
                series["CollectedThreshold"] = "0";

                foreach (var employee in employeeHours)         //loop kroz sve radnike i njihove sate rada, dodavanje u chart
                {
                    double totalHours = employee.TotalHours;
                    TimeSpan timeSpan = TimeSpan.FromHours(totalHours);

                    if (employee.EmployeeName != null && employee.TotalHours > 0)
                    {
                        series.Points.AddXY($"{employee.EmployeeName} - {(int)employee.TotalHours} hrs", (int)employee.TotalHours);
                    }
                }

                chart.Series.Add(series);                       
                chart.Titles.Add("Data");                       //dodavanje titla na vrhu 

                Legend legend = new Legend();                   //dodavanje legend sa strane
                legend.Name = "Legend";
                legend.Title = "Legend";
                chart.Legends.Add(legend);

                chart.SaveImage(Path, ChartImageFormat.Png);    //finaliziranje slike na odredjenoj lokaciji 'Path'

                Console.WriteLine($"Successfully created pie chart at : {Path}");
                Console.ReadKey();
            }
        }
    }
}