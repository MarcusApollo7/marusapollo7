using System;
using System.Collections;
using System.Runtime.InteropServices;
using Plotly.NET.LayoutObjects;
using System.Linq;
using System.Text;
using Plotly.NET.TraceObjects;
using Plotly.NET;
namespace solid_volume_finder;

 class Program
{
    public static Path Walk(IWalker walker)
    {
        double[] startpt = walker.Start();
        int numsteps = walker.Numsteps();

        // sets current position as the starting point
        var cur_x = startpt[0];
        var cur_y = startpt[1];
        var cur_z = startpt[2];

        // Instantiates path variable 
        var path = new Path(startpt);

        for (int N = 0; N < numsteps; N++)
        {
            double[] next_step = walker.FindStep(path, cur_x, cur_y, cur_z);
            if (next_step[0] == cur_x && next_step[1] == cur_y && next_step[2] == cur_z)
            {
                Console.WriteLine($"Dead End after: {N} steps");
                return path;
            }
            else
            {
                path.Add(next_step);
                cur_x = next_step[0];
                cur_y = next_step[1];
                cur_z = next_step[2];
            }

        }
        Console.WriteLine("Finished Waking");
        return path;
    }
   
    static void Main(string[] args)
    {
        Circle sbase = new(1, 0, 0);
        Prism stop = new(1);
        Solid cyl = new(sbase, stop);
        Rect srect = new(-.5, -.5, 1, 1);
        Solid cube = new(srect, stop);
        var start = new double[] { 0, 0, .5 };


        var walker = new SAThetaRandomWalker(.1, 1000, start, cube);
        Path path = Walk(walker);
        var scatter = path.Graph();


        
        GenericChart cube_base = cube.Draw();
        var combinedChart = Chart.Combine([scatter, cube_base]);
        combinedChart.Show();
    } 
}

