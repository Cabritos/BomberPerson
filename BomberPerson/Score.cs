using System;
using System.Collections.Generic;
using System.Text;
using SFML.Graphics;
using SFML.System;

namespace BomberPerson
{
    class Score
    {
        private int _player;
        public int Points { get; private set; }
        public Text ScoreText { get; }

        private int _goalPoints;
        
        public Score(int player, int goalPoints, float offset)
        {
            _player = player;
            _goalPoints = goalPoints;
            Points = 0;

            ScoreText = new Text(Points.ToString(), new Font("font.ttf"), Application.WindowHeight / 12);
            ScoreText.DisplayedString = Points.ToString();
            ScoreText.Origin = new Vector2f(ScoreText.GetGlobalBounds().Width / 2, ScoreText.GetGlobalBounds().Height / 2);

            ScoreText.Position = _player switch
            {
                0 => new Vector2f(offset * 2, Application.WindowHeight / 5),
                1 => new Vector2f(Application.WindowWidth - offset * 2, Application.WindowHeight / 5 * 4),
                2 => new Vector2f(Application.WindowWidth - offset * 2, Application.WindowHeight / 5),
                _ => new Vector2f(offset * 2, Application.WindowHeight / 5 * 4)
            };
        }

        public bool AddPoint()
        {
            Points++;
            ScoreText.DisplayedString = Points.ToString();
            
            ScoreText.Origin = new Vector2f(ScoreText.GetGlobalBounds().Width / 2, ScoreText.GetGlobalBounds().Height / 2);

            return (Points >= _goalPoints);
        }
    }
}
