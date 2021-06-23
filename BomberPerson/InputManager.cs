using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using SFML.Window;

namespace BomberPerson
{
    class InputManager
    {
        private Keyboard.Key _up;
        private Keyboard.Key _down;
        private Keyboard.Key _left;
        private Keyboard.Key _right;
        private Keyboard.Key _action;
        
        public InputManager(Keyboard.Key up, Keyboard.Key down, Keyboard.Key left, Keyboard.Key right, Keyboard.Key action)
        {
            _up = up;
            _down = down;
            _left = left;
            _right = right;
            _action = action;
        }

        public bool Up()
        {
            return Keyboard.IsKeyPressed(_up);
        }

        public bool Down()
        {
            return Keyboard.IsKeyPressed(_down);
        }

        public bool Left()
        {
            return Keyboard.IsKeyPressed(_left);
        }

        public bool Right()
        {
            return Keyboard.IsKeyPressed(_right);
        }

        public bool Action()
        {
            return Keyboard.IsKeyPressed(_action);
        }

        //Debido a las restricciones de input, la pausa se gestiona únicamente desde la clase Round con un botón único
        /*
        public bool Pause()
        {
            return Keyboard.IsKeyPressed(Keyboard.Key.Escape);
        }
        */
    }
}
