using Plotly.NET;
namespace solid_volume_finder;


// Creates the interface for the randomwalker
// Must define a Walk function which manages recording valid moves and update current position
// and a FindStep which figures out which are valid and returns the current position if no valid moves exist
public interface IWalker
{
    double[] FindStep(Path path, double x, double y, double z);

    double[] Start();

    int Numsteps();
}



// Creates a class for a Self Avoiding Random Walker implementing IWalker
// Implements standard walk algorithim using a for loop
// FindStep looks if the point has already been visited and if its within the provided Solid 
public class SARandomWalker(double stepsize, int numsteps, double[] start_point, Solid solid) : IWalker
{
    public readonly int _numsteps = numsteps;
    public readonly double _stepsize = stepsize;
    public readonly double[] _startpt = start_point;
    public readonly Random _random = new();
    public readonly Solid _solid = solid;

    public double[] Start()
    {
        return _startpt;
    }

    public int Numsteps()
    {
        return _numsteps;
    }
    public double[] FindStep(Path path, double x, double y, double z) // self avoiding walker in 3d
    {
        // creates list of possible actions represented as tuples of ints called crossJoin
        List<int> pdirections = [0, 1, 2];
        List<int> pplanar = [-1, 1];

        var crossJoin = new List<(int, int)>();

        foreach (var d in pdirections)
        {
            foreach (var p in pplanar)
            {
                crossJoin.Add((d, p));
            }
        }
        // while crossJoin has items 1) pick one randomly, 2) compute new point, 3) Check for validity if valid return new point
        // if invalid remove action from crossJoin and start over
        while (crossJoin.Count > 0)
        {
            var ind = _random.Next(crossJoin.Count);
            var pair = crossJoin[ind];
            var direction = pair.Item1;
            int step = pair.Item2;

            double[] new_pos = direction switch
            {
                0 => [x + step * _stepsize, y, z],
                1 => [x, y + step * _stepsize, z],
                2 => [x, y, z + step * _stepsize],
                _ => [x, y, z]

            };
            if (path.Retrace(new_pos) == false && _solid.Inside(new_pos) == true)
            {
                return new_pos;
            }
            else
            {
                crossJoin.Remove((direction, step));
            }
        }
        // if no valid points exist return the current position
        return [x, y, z];

    }
}

public class RandomWalker(double stepsize, int numsteps, double[] startpoint, Solid solid) : IWalker
{
    private readonly double _stepsize = stepsize;
    private readonly int _numsteps = numsteps;
    private readonly double[] _startpt = startpoint;
    private readonly Solid _solid = solid;
    private readonly Random _random = new();

    public double[] Start()
    {
        return _startpt;
    }

    public int Numsteps()
    {
        return _numsteps;
    }
    public double[] FindStep(Path path, double x, double y, double z) 
    {
        // creates list of possible actions represented as tuples of ints called crossJoin
        List<int> pdirections = [0, 1, 2];
        List<int> pplanar = [-1, 1];

        var crossJoin = new List<(int, int)>();

        foreach (var d in pdirections)
        {
            foreach (var p in pplanar)
            {
                crossJoin.Add((d, p));
            }
        }
        // while crossJoin has items 1) pick one randomly, 2) compute new point, 3) Check for validity if valid return new point
        // if invalid remove action from crossJoin and start over
        while (crossJoin.Count > 0)
        {
            var ind = _random.Next(crossJoin.Count);
            var pair = crossJoin[ind];
            var direction = pair.Item1;
            int step = pair.Item2;

            double[] new_pos = direction switch
            {
                0 => [x + step * _stepsize, y, z],
                1 => [x, y + step * _stepsize, z],
                2 => [x, y, z + step * _stepsize],
                _ => [x, y, z]

            };
            if (_solid.Inside(new_pos) == true)
            {
                return new_pos;
            }
            else
            {
                crossJoin.Remove((direction, step));
            }
        }
        return [x, y, z];
    }
}

public class ThetaRandomWalker(double stepsize, int numsteps, double[] start_point, Solid solid) : IWalker
{
    public readonly int _numsteps = numsteps;
    public readonly double _stepsize = stepsize;
    public readonly double[] _startpt = start_point;
    public readonly Random _random = new();
    public readonly Solid _solid = solid;

    public double[] Start()
    {
        return _startpt;
    }

    public int Numsteps()
    {
        return _numsteps;
    }

    public double[] FindStep(Path path, double x, double y, double z)
    {
        bool Looking = true;
        while (Looking == true)
        {

            double theta = _random.NextDouble() * 2 * Math.PI;
            double phi = _random.NextDouble() * 2 * Math.PI;
            double new_x = _stepsize * Math.Sin(theta) * Math.Cos(phi) + x;
            double new_y = _stepsize * Math.Sin(theta) * Math.Sin(phi) + y;
            double new_z = _stepsize * Math.Cos(theta) + z;

            double[] new_pos = [new_x, new_y, new_z];
            if (_solid.Inside(new_pos) == true)
            {
                Looking = false;
                return new_pos;
            }
        }
        return [x, y, z];

    }
}

public class SAThetaRandomWalker(double stepsize, int numsteps, double[] start_point, Solid solid) : IWalker
{
    public readonly int _numsteps = numsteps;
    public readonly double _stepsize = stepsize;
    public readonly double[] _startpt = start_point;
    public readonly Random _random = new();
    public readonly Solid _solid = solid;

    public double[] Start()
    {
        return _startpt;
    }

    public int Numsteps()
    {
        return _numsteps;
    }

    public double[] FindStep(Path path, double x, double y, double z)
    {
        bool Looking = true;
        while (Looking == true)
        {

            double theta = _random.NextDouble() * 2 * Math.PI;
            double phi = _random.NextDouble() * 2 * Math.PI;
            double new_x = _stepsize * Math.Sin(theta) * Math.Cos(phi) + x;
            double new_y = _stepsize * Math.Sin(theta) * Math.Sin(phi) + y;
            double new_z = _stepsize * Math.Cos(theta) + z;

            double[] new_pos = [new_x, new_y, new_z];
            if (_solid.Inside(new_pos) == true && path.Repath(new_pos) == false)
            {
                Looking = false;
                return new_pos;
            }
        }
        return [x, y, z];

    }
}


public class Path(double[] startpt) // must be given a start point
{
    public List<double[]> TPath { get; set; } = [startpt];

    public bool Retrace(double[] comp)
    // function for computing if provided point is on the path 
    // false: point is not on path
    // true: point is on path
    {
        foreach (double[] pt in TPath)
        {
            int matches = 0;
            foreach (var (ppt, cpt) in pt.Zip(comp))
            {
                if (ppt == cpt)
                {
                    matches += 1;
                }
            }
            if (matches == 3)
            {
                return true;
            }

        }
        return false;
    }

    public bool Repath(double[] pot_point)
    // function for computing if provided point and end point of Path cross any other lines
    // false: line formed by new point crosses no previous line segments
    // true: line formed by new point crosses previous line segments 
    {
        if (TPath.Count < 2)
        {
            return false;
        }
        else if (TPath.Count >= 2)
        {
            for (int i = 0; i < TPath.Count-1; i++)
            {
                if (OnLine(TPath[i], TPath[i + 1], TPath[TPath.Count-1], pot_point) == true)
                    {
                            return false;
                        }
            }
            }
            
        return true;
    }


    public static bool OnLine(double[] comp1, double[] comp2, double[] end_point, double[] pot_point)
    // true is the lines cross, false they do not
    {
        double A_x = comp1[0] - end_point[0];
        double A_y = comp1[1] - end_point[1];
        double A_z = comp1[2] - end_point[2];

        double B_x = comp2[0] - comp1[0];
        double B_y = comp2[1] - comp1[1];
        double B_z = comp2[2] - comp1[2];

        double C_x = pot_point[0] - end_point[0];
        double C_y = pot_point[1] - end_point[1];
        double C_z = pot_point[2] - end_point[2];

        if (B_x == C_x && B_y == C_y && B_z == C_z)
        {
            Console.WriteLine("Lines are parallel");
            return false;
        }
        else
        {
            // double ADotA = Dot3(A_x, A_y, A_z, A_x, A_y, A_z);
            // double BDotA = Dot3(B_x, B_y, B_z, A_x, A_y, A_z);
            // double BDotC = Dot3(B_x, B_y, B_z, C_x, C_y, C_z);
            // double CDotA = Dot3(C_x, C_y, C_z, A_x, A_y, A_z);

            double ADotB = Dot3(A_x, A_y, A_z, B_x, B_y, B_z);
            double ADotC = Dot3(A_x, A_y, A_z, C_x, C_y, C_z);

            
            double BDotB = Dot3(B_x, B_y, B_z, B_x, B_y, B_z);
            double CDotB = Dot3(C_x, C_y, C_z, B_x, B_y, B_z);
            double CDotC = Dot3(C_x, C_y, C_z, C_x, C_y, C_z);

            double ma = (ADotC * CDotB - ADotB * CDotC) / (BDotB * CDotC - CDotB * CDotB);
            double mb = (ADotC + ma * CDotB) / CDotC;

            double[] pa = [comp1[0] + B_x * ma, comp1[1] + B_y * ma, comp1[2] + B_z * ma];
            double[] pb = [end_point[0] + C_x * mb, end_point[1] + C_y * mb, end_point[2] + C_z * mb];
            if (pa[0] - pb[0] < double.Epsilon && pa[1] - pb[1] < double.Epsilon && pa[2] - pb[2] < double.Epsilon)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public static double Dot3(double x1, double y1, double z1, double x2, double y2, double z2)
    {
        return x1 * x2 + y1 * y2 + z1 * z2;
    }

    // method for adding points to path
    public void Add(double[] new_pos)
    {
        TPath.Add(new_pos);
    }

    public int Length()
    {
        return TPath.Count;
    }

    // prints out the path
    public void PrintPath()
    {
        foreach (double[] p in TPath)
        {
            Console.WriteLine($"Position: ({p[0]}, {p[1]}, {p[2]})");
        }
    }
    // Gets points for graphing
    public GenericChart Graph()
    {
        var xs = new List<double> { };
        var ys = new List<double> { };
        var zs = new List<double> { };
        foreach (double[] p in TPath)
        {
            xs.Add(p[0]);
            ys.Add(p[1]);
            zs.Add(p[2]);
        }

        return Chart3D.Chart.Line3D<double, double, double, string>(
                x: xs,
                y: ys,
                z: zs,
                Name: "3D Line Plot");
    }
}

