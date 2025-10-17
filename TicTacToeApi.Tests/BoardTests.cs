using Xunit;
using TicTacToeApi.API.TicTacToe;

namespace TicTacToeApi.API.Tests
{
    public class BoardPositionTests
    {
        [Fact]
        public void Up__ShouldWrapToBottomRow_WhenInUpperRow()
        {
            var pos = new BoardPosition(0);
            var up = pos.Up();
            Assert.Equal(6, up.ToArrayIndex());
        }

        [Fact]
        public void Left_ShouldWrapToRightColumn_WhenInLeftRow()
        {
            var pos = new BoardPosition(3);
            var newPos = pos.Left();
            Assert.Equal(5, newPos.ToArrayIndex());
        }

        [Fact]
        public void Right_ShouldWrapToLeftColumn_WhenInRightRow()
        {
            var pos = new BoardPosition(8);
            var newPos = pos.Right();
            Assert.Equal(6, newPos.ToArrayIndex());
        }

        [Fact]
        public void Down_ShouldWrapToUpperRow_WhenInBottomRow()
        {
            var pos = new BoardPosition(7);
            var newPos = pos.Down();
            Assert.Equal(1, newPos.ToArrayIndex());
        }

        [Fact]
        public void NextOnDiagonal_MainDiagonal()
        {
            var pos = new BoardPosition(0);
            var next = pos.NextOnDiagonal();
            var nextnext = next.NextOnDiagonal();
            var nextnextnext = nextnext.NextOnDiagonal();
            Assert.Equal(4, next.ToArrayIndex());
            Assert.Equal(8, nextnext.ToArrayIndex());
            Assert.Equal(0, nextnextnext.ToArrayIndex());
        }
    }
}
