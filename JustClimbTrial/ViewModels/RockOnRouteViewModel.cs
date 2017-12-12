using JustClimbTrial.DataAccess;
using JustClimbTrial.Enums;
using System.Windows.Shapes;

namespace JustClimbTrial.ViewModels
{
    public class RockOnRouteViewModel
    {
        public Rock MyRock { get; set; }
        public RockOnBoulderStatus BoulderStatus { get; set; }
        public int TrainingSeq { get; set; }
        public Shape ShapeOnCanvas { get; set; }
    }
}
