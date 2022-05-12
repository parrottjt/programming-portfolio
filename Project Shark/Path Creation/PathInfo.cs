

namespace EndOceanGen
{
    public abstract class PathInfo
    {
        protected GridObject<EndlessOceanRoom> Grid;

        protected PathInfo(GridObject<EndlessOceanRoom> grid)
        {
            Grid = grid;
        }
    }
}
