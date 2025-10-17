
using Xunit;
using TicTacToeApi.API.TicTacToe;

namespace TicTacToeApi.API.Tests
{
    public class GameTests
    {
        [Fact]
        public void ArePositionsOf()
        {
            var game = new Game("xox" + "   " + "oox", 'x');
            var r = game.ArePositionsOf(new BoardPosition(0), new BoardPosition(2), 'x');
            Assert.True(r);

            r = game.ArePositionsOf(new BoardPosition(0), new BoardPosition(2), 'o');
            Assert.False(r);

            r = game.ArePositionsOf(new BoardPosition(3), new BoardPosition(4), 'x');
            Assert.False(r);

            game = new Game("xx    oo ", 'x');
            var pos = new BoardPosition(2);
            r = game.ArePositionsOf(pos.Left(), pos.Left().Left(), 'x');
            Assert.True(r);
        }

        [Fact]
        public void CompleteThreeInARow()
        {
            var game = new Game("xx       ", 'x');
            var result = game.CompleteThreeInARow('x');
            Assert.Equal(2, result);

            game = new Game("xo  x    ", 'x');
            result = game.CompleteThreeInARow('x');
            Assert.Equal(8, result);
        }

        [Fact]
        public void CountAligned()
        {
            var game = new Game("xo  oxx o", 'x');
            var result = game.CountAligned(2, 'x');
            Assert.Equal(3, result);
            result = game.CountAligned(2, 'o');
            Assert.Equal(3, result);
        }

        [Fact]
        public void TryWin()
        {
            // Arrange
            var game = new Game("xx       ", 'x');
            // Act
            var result = game.TryWin();
            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public void TryBlock_ShouldReturnTrue_WhenThereIsAWinningMoveForOpponent()
        {
            // Arrange
            var game = new Game("oo       ", 'x');
            // Act
            var result = game.TryBlock();
            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public void TryFork_ShouldReturnTrue_WhenThereIsAForkingMove()
        {
            // Arrange
            var game = new Game("x o" +
                                "   " +
                                "  x", 'x');
            // Act
            var result = game.TryFork();
            // Assert
            Assert.Equal(6, result);
        }

        [Fact]
        public void TryBlockFork_ShouldReturnTrue_WhenThereIsAForkingMoveForOpponent()
        {
            // Arrange
            var game = new Game("o  " +
                                "  x" +
                                "  o", 'x');
            // Act
            var result = game.TryBlockFork();
            // Assert
            Assert.Equal(6, result);
        }

        [Fact]
        public void TryCenter_ShouldReturnTrue_WhenCenterIsEmpty()
        {
            // Arrange
            var game = new Game("         ", 'x');
            // Act
            var result = game.TryCenter();
            // Assert
            Assert.Equal(4, result);
        }

        [Fact]
        public void TryOppositeCorner_ShouldReturnTrue_WhenOpponentIsInCorner()
        {
            // Arrange
            var game = new Game("o        ", 'x');
            // Act
            var result = game.TryOppositeCorner();
            // Assert
            Assert.Equal(8, result);
        }

        [Fact]
        public void TryEmptyCorner_ShouldReturnTrue_WhenThereIsAnEmptyCorner()
        {
            // Arrange
            var game = new Game("         ", 'x');
            // Act
            var result = game.TryEmptyCorner();
            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void TryEmptySide_ShouldReturnTrue_WhenThereIsAnEmptySide()
        {
            // Arrange
            var game = new Game("xox" +
                                "oxo" +
                                " x ", 'x');
            // Act
            var result = game.TryEmptySide();
            // Assert
            Assert.Equal(-1, result);
        }

        [Fact]
        public void OptimalPlay_ShouldReturnWinningMove_WhenPossible()
        {
            // Arrange
            var game = new Game("xx     o ", 'x');
            // Act
            var result = game.OptimalPlay();
            // Assert
            Assert.Equal("xxx    o ", result);
        }

        [Fact]
        public void PlayersTurn_ShouldReturnTrue_WhenIsPlayersTurn()
        {
            // Arrange
            var game = new Game("x o x o ", 'x');
            // Act
            var result = game.PlayersTurn();
            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsGameOver_ShouldReturnTrue_WhenGameIsOver()
        {
            // Arrange
            var game = new Game("xxx      ", 'x');
            // Act
            var result = game.IsGameOver();
            // Assert
            Assert.True(result);
        }
    }
}
