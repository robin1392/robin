using System;
using Photon.Deterministic;

namespace Quantum
{
    public class FieldPositions
    {
        private readonly FPVector2[] _bottomPositions;
        private readonly FPVector2[] _topPositions;
        private readonly FPVector2 _bottomPlayerPosition;
        private readonly FPVector2 _topPlayerPosition;
        
        public FieldPositions(FPVector2[] bottomPositions, FPVector2[] topPositions, FPVector2 bottomPlayerPosition, FPVector2 topPlayerPosition)
        {
            _bottomPositions = bottomPositions;
            _topPositions = topPositions;
            _bottomPlayerPosition = bottomPlayerPosition;
            _topPlayerPosition = topPlayerPosition;
        }

        public FPVector2 GetBottomPosition(Int32 fieldIndex)
        {
            return _bottomPositions[fieldIndex];
        }
        
        public FPVector2 GetTopPosition(Int32 fieldIndex)
        {
            return _topPositions[fieldIndex];
        }

        public FPVector2 GetPosition(Int32 i, Int32 fieldIndex)
        {
            if (i == GameConstants.BottomCamp)
            {
                return GetBottomPosition(fieldIndex);
            }
            else if (i == GameConstants.TopCamp)
            {
                return GetTopPosition(fieldIndex);
            }

            return FPVector2.Zero;
        }
        
        public FPVector2 GetBottomPlayerPosition()
        {
            return _bottomPlayerPosition;
        }
        
        public FPVector2 GetTopPlayerPosition()
        {
            return _topPlayerPosition;
        }
    }
}