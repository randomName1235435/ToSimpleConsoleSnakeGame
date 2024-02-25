using System.Runtime.InteropServices;
using System.Xml;

class Program
{
    const int ConsoleWidth = 50;
    const int ConsoleHeight = 20;
    static readonly TimeSpan SnakeSpeed = TimeSpan.FromMilliseconds(100);

    static int score = 0;
    static bool gameOver = false;
    static Direction direction = Direction.Right;
    static List<Position> snake = new();

    static void Main(string[] args)
    {

        Console.SetWindowSize(ConsoleWidth, ConsoleHeight);
        Console.SetBufferSize(ConsoleWidth, ConsoleHeight);
        Console.CursorVisible = false;

        InitializeGame();

        while (!gameOver)
        {
            if (Console.KeyAvailable)
            {
                HandleInput(Console.ReadKey(true).Key);
            }

            MoveSnake();
            CheckCollision();
            PlaceFood();

            if (!gameOver)
            {
                Console.Clear();
                DrawSnake();
                DrawFood();
                DrawScore();
                Thread.Sleep(SnakeSpeed);
            }
        }

        Console.SetCursorPosition(ConsoleWidth / 2 - 5, ConsoleHeight / 2);
        Console.Write("Game Over!");
        Console.ReadLine();
    }

    static void InitializeGame()
    {
        snake.Add(new Position(ConsoleWidth / 2, ConsoleHeight / 2));
        snake.Add(new Position(ConsoleWidth / 2 - 1, ConsoleHeight / 2));
        snake.Add(new Position(ConsoleWidth / 2 - 2, ConsoleHeight / 2));

        PlaceFood();
    }

    private static void DrawSnake()
    {
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        foreach (var segment in snake)
        {
            Console.SetCursorPosition(segment.X, segment.Y);
            Console.Write("*");
        }
    }

    private static void DrawFood()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.SetCursorPosition(food.X, food.Y);
        Console.Write("#");
    }

    private static void DrawScore()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(0, 0);
        Console.Write($"Score: {score}");
    }

    private static void HandleInput(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.UpArrow:
                if (direction != Direction.Down)
                    direction = Direction.Up;
                break;
            case ConsoleKey.DownArrow:
                if (direction != Direction.Up)
                    direction = Direction.Down;
                break;
            case ConsoleKey.LeftArrow:
                if (direction != Direction.Right)
                    direction = Direction.Left;
                break;
            case ConsoleKey.RightArrow:
                if (direction != Direction.Left)
                    direction = Direction.Right;
                break;
        }
    }

    private static void MoveSnake()
    {
        var head = snake.First();
        Position newHead = GetDirection(head);

        snake.Insert(0, newHead);
        if (newHead.Equals(food))
        {
            score++;
        }
        else
        {
            snake.RemoveAt(snake.Count - 1);
        }
    }

    private static Position GetDirection(Position head)
    {
        return direction switch
        {
            Direction.Up => new Position(head.X, head.Y - 1),
            Direction.Down => new Position(head.X, head.Y + 1),
            Direction.Left => new Position(head.X - 1, head.Y),
            Direction.Right => new Position(head.X + 1, head.Y),
            _ => throw new InvalidOperationException()
        };
    }
    private static Random rand = new Random();
    private static Position food = new(rand.Next(0, ConsoleWidth), rand.Next(0, ConsoleHeight));

    static void PlaceFood()
    {
        while (snake.Contains(food))
        {
            var x = rand.Next(0, ConsoleWidth);
            var y = rand.Next(0, ConsoleHeight);
            food = new Position(x, y);
        }
    }

    static void CheckCollision()
    {
        var head = snake.First();
        if (head.X < 0 || head.X >= ConsoleWidth || head.Y < 0 || head.Y >= ConsoleHeight)
        {
            gameOver = true;
            return;
        }
        if (snake.Count != snake.Distinct().Count())
        {
            gameOver = true;
            return;
        }
    }

    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    struct Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Position))
                return false;

            var other = (Position)obj;
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
    }
}
