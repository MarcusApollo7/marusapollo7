using System.IO.Compression;
using Plotly.NET;
namespace solid_volume_finder;

// Creates the interface for the shape of the base
// Must an area function, a method of determining whether a provided point is inside the border, 
// and Permimeter which generates a 3d plotly plot of the base of the shape
public interface IShape
{
    double Area();
    bool Inside(double x, double y);
    GenericChart Perimeter(int resolution);
}

public interface ITop
{
    bool Under(double x, double y, double z);
}

// Creates a class for a Circle implementing IWalker
// Straightforwardly implements the required functions
public class Circle(double radius, double centerX, double centerY) : IShape
{
    public readonly double _radius = radius;
    private readonly double _centerX = centerX;
    private readonly double _centerY = centerY;

    public double Area()
    {
        return Math.PI * (Math.Pow(_radius, 2));
    }

    public bool Inside(double x, double y)
    {
        {
            // Calculate distance from point to center of the circle
            double dx = x - _centerX;
            double dy = y - _centerY;

            // Check if distance is less than or equal to radius
            return (dx * dx + dy * dy) <= (_radius * _radius);
        }
    }
    public GenericChart Perimeter(int resolution)
    {
        double[] theta = Enumerable.Range(0, resolution).Select(i => 2 * Math.PI * i / (resolution - 1)).ToArray();
        double[] x = new double[resolution];
        double[] y = new double[resolution];
        double[] z = new double[resolution];
        for (int j = 0; j < resolution; j++)
        {
            x[j] = _radius * Math.Cos(theta[j]) + _centerX;
            y[j] = _radius * Math.Sin(theta[j]) + _centerY;
            z[j] = 0;
        }
        var lineChart = Chart3D.Chart.Scatter3D<double, double, double, string>(x: x, y: y, z: z, mode: StyleParam.Mode.Lines);
        return lineChart;
    }
    public (double, double, double) Features()
    {
        return (_radius, _centerX, _centerY);
    }
}
// implementation of the Rectange/Square based on the position of the top left corner
public class Rect( double left, double top, double width, double length) : IShape
{
    
    private readonly double _left = left;
    private readonly double _top = top;
    
    private readonly double _length = length;
    private readonly double _width = width;

    public double Area()
    {

        return _width * _length;
    }

    public bool Inside(double x, double y)
    {
        {
            // Check if distance is less than or equal to radius
            return x >= _left && x <= _left + _width && y >= _top && y <= _top + _length;
        }
    }
    public GenericChart Perimeter(int resolution) // resolution is never used because it doesn't matter
    {
        var x = new double[] { _left, _left, _left + _width, _left + _width, _left };
        var y = new double[] { _top, _top + _length, _top +_length, _top, _top};
        var z = new double[] { 0, 0, 0, 0, 0 };
        

        var lineChart = Chart3D.Chart.Scatter3D<double, double, double, string>(x: x, y: y, z: z, mode: StyleParam.Mode.Lines);
        return lineChart;
    }
}

// Solid class which takes a IShape for the base, a double for the height of the top, 
// apex is true if the shape meets at a single point such as a cone or pyramid
// inside function mostly uses the shape's inside but computes if its less than the height

public class Prism(double height) : ITop
{
    private readonly double _height = height;

    public bool Under(double x, double y, double z)
    {
        if (z <= _height && 0 <= z)
        {
            return true;
        }
        return false;
    }
}


public class Solid(IShape shape, ITop top)
{
    private readonly IShape _shape = shape;
    private readonly ITop _top = top;

    public bool Inside(double[] pt)
    {
        var x = pt[0];
        var y = pt[1];
        var z = pt[2];
        if (_shape.Inside(x, y) == true && _top.Under(x, y, z) == true)
        {
            return true;
        }
        return false;
    }

    public GenericChart Draw()
    {
        return _shape.Perimeter(50);
    }

}
    
    