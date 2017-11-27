using JustClimbTrial.DataAccess;
using System.Windows;

namespace JustClimbTrial.Extensions
{
    public static class RockExtension
    {
        public static Point GetPoint(this Rock rock)
        {
            return new Point(rock.CoorX.GetValueOrDefault(0), rock.CoorY.GetValueOrDefault(0));
        }
    }
}
