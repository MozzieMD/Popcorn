using System.Windows;

namespace Popcorn.UserControls.FluidLayout
{
    internal class FluidLayoutEventArgs
    {
        #region Nested type: PositionChanged

        internal class PositionChanged : FluidLayoutEventArgs
        {
            public PositionChanged(Tile.Position position)
            {
                Position = position;
            }

            public Tile.Position Position { get; private set; }
        }

        #endregion

        #region Nested type: SizeChanged

        internal class SizeChanged : FluidLayoutEventArgs
        {
            public SizeChanged(Size previousSize, Size newSize)
            {
                PreviousSize = previousSize;
                NewSize = newSize;
            }

            public Size PreviousSize { get; private set; }

            public Size NewSize { get; private set; }
        }

        #endregion
    }
}
