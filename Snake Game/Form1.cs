using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Snake_Game
{
    public partial class Form1 : Form
    {
        private const int GridSize = 20; // Size of each grid cell
        private const int InitialSnakeLength = 3; // Initial length of the snake
        private const int TimerInterval = 200; // Interval for the game timer in milliseconds

        private List<Point> snake; // List of points representing the snake
        private Point food; // Point representing the food
        private Point direction; // Current direction of the snake
        private Timer gameTimer; // Timer for game updates
        private int score; // Current score
        private bool isGameRunning; // Flag indicating if the game is running
        private bool isGamePaused; // Flag indicating if the game is paused

        private Label scoreLabel; // Label to display the score
        private Button startButton; // Button to start the game
        private Button pauseButton; // Button to pause/resume the game

        private Random random; // Random number generator
        private int maxX; // Maximum number of cells in the X direction
        private int maxY; // Maximum number of cells in the Y direction

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            snake = new List<Point>();
            direction = new Point(1, 0); // Start moving to the right
            score = 0;
            isGameRunning = false;
            isGamePaused = false;

            // Remove existing score label, start button, and pause button if they exist
            if (scoreLabel != null)
                Controls.Remove(scoreLabel);
            if (startButton != null)
                Controls.Remove(startButton);
            if (pauseButton != null)
                Controls.Remove(pauseButton);

            // Initialize the score label
            scoreLabel = new Label();
            scoreLabel.Text = "Score: 0";
            scoreLabel.Location = new Point(10, 10);
            scoreLabel.AutoSize = true;
            Controls.Add(scoreLabel);

            // Initialize the start button
            startButton = new Button();
            startButton.Text = "Start";
            startButton.AutoSize = true;
            startButton.Location = new Point((ClientSize.Width - startButton.Width) / 2, (ClientSize.Height - startButton.Height) / 2);
            startButton.Click += StartButton_Click;
            Controls.Add(startButton);

            // Initialize the pause button
            pauseButton = new Button();
            pauseButton.Text = "Pause";
            pauseButton.AutoSize = true;
            pauseButton.Location = new Point((ClientSize.Width - pauseButton.Width) / 2, (ClientSize.Height - pauseButton.Height) / 2);
            pauseButton.Enabled = false;
            pauseButton.Visible = false;
            pauseButton.Click += PauseButton_Click;
            Controls.Add(pauseButton);

            // Initialize the random number generator
            random = new Random();

            // Calculate the maximum number of cells in the X and Y directions
            maxX = ClientSize.Width / GridSize;
            maxY = ClientSize.Height / GridSize;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Paint += new PaintEventHandler(Form1_Paint);
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            if (isGameRunning)
            {
                // Draw the snake
                foreach (Point point in snake)
                {
                    canvas.FillRectangle(Brushes.Green, new Rectangle(point.X * GridSize, point.Y * GridSize, GridSize, GridSize));
                }

                // Draw the food
                canvas.FillEllipse(Brushes.Red, new Rectangle(food.X * GridSize, food.Y * GridSize, GridSize, GridSize));
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (isGameRunning && !isGamePaused)
            {
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        if (direction.X != 1)
                            direction = new Point(-1, 0);
                        break;
                    case Keys.Right:
                        if (direction.X != -1)
                            direction = new Point(1, 0);
                        break;
                    case Keys.Up:
                        if (direction.Y != 1)
                            direction = new Point(0, -1);
                        break;
                    case Keys.Down:
                        if (direction.Y != -1)
                            direction = new Point(0, 1);
                        break;
                    case Keys.Space:
                        PauseGame();
                        break;
                }
            }
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (!isGameRunning)
            {
                startButton.Enabled = false; // Disable the start button
                startButton.Visible = false; // Hide the start button
                pauseButton.Enabled = true; // Enable the pause button
                pauseButton.Visible = true; // Show the pause button

                // Create the initial snake
                snake.Clear();
                for (int i = InitialSnakeLength - 1; i >= 0; i--)
                {
                    snake.Add(new Point(i, 0));
                }

                // Generate the first food
                GenerateFood();

                // Start the game
                isGameRunning = true;

                // Start the game timer
                gameTimer = new Timer();
                gameTimer.Interval = TimerInterval;
                gameTimer.Tick += GameTimer_Tick;
                gameTimer.Start();
            }
        }

        private void PauseButton_Click(object sender, EventArgs e)
        {
            PauseGame();
        }

        private void PauseGame()
        {
            if (isGameRunning)
            {
                if (isGamePaused)
                {
                    // Resume the game
                    isGamePaused = false;
                    pauseButton.Text = "Pause";
                    gameTimer.Start();
                }
                else
                {
                    // Pause the game
                    isGamePaused = true;
                    pauseButton.Text = "Resume";
                    gameTimer.Stop();
                }
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            MoveSnake();
            CheckCollision();
            Refresh();
        }

        private void MoveSnake()
        {
            // Move the snake by adding a new head in the current direction and removing the tail
            Point newHead = new Point((snake[0].X + direction.X + maxX) % maxX, (snake[0].Y + direction.Y + maxY) % maxY);
            snake.Insert(0, newHead);
            snake.RemoveAt(snake.Count - 1);
        }

        private void CheckCollision()
        {
            // Check if the snake has collided with the borders or itself
            Point head = snake[0];

            if (snake.Skip(1).Any(p => p == head))
            {
                gameTimer.Stop();
                MessageBox.Show("Game Over! Score: " + score);
                InitializeGame();
                return;
            }

            // Check if the snake has eaten the food
            if (head == food)
            {
                // Generate new food and increase the score
                GenerateFood();
                snake.Add(new Point());
                score++;
                scoreLabel.Text = "Score: " + score;
            }
        }

        private void GenerateFood()
        {
            // Generate a random position for the food that is not occupied by the snake
            do
            {
                food = new Point(random.Next(maxX), random.Next(maxY));
            } while (snake.Any(p => p == food));
        }
    }
}
