using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DragonFlyBugTrackerNet6.Models
{
  public class AmChartData
  {
    public AmItem[] Data { get; set; }
  }


  public class AmItem
  {
    public string? Project { get; set; }
    public int Tickets { get; set; }
    public int Developers { get; set; }
  }
}
