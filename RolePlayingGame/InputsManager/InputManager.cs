using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace RolePlayingGame.InputsManager
{
    public static class InputManager
    {
        public enum Action
        {
            MainMenu,
            Ok,
            Back,
            CharacterManagement,
            ExitGame,
            TakeView,
            DropUnEquip,
            MoveCharacterUp,
            MoveCharacterDown,
            MoveCharacterLeft,
            MoveCharacterRight,
            CursorUp,
            CursorDown,
            DecreaseAmount,
            IncreaseAmount,
            PageLeft,
            PageRight,
            TargetUp,
            TargetDown,
            ActiveCharacterLeft,
            ActiveCharacterRight,
            TotalActionCount
        }

        private static readonly string[] _actionNames =
        {
            "Main Menu",
            "Ok",
            "Back",
            "Character Management",
            "Exit Game",
            "Take / View",
            "Drop / Unequip",
            "Move Character - Up",
            "Move Character - Down",
            "Move Character - Left",
            "Move Character - Right",
            "Move Cursor - Up",
            "Move Cursor - Down",
            "Decrease Amount",
            "Increase Amount",
            "Page Screen Left",
            "Page Screen Right",
            "Select Target -Up",
            "Select Target - Down",
            "Select Active Character - Left",
            "Select Active Character - Right",
        };

        public static string GetActionName(Action action)
        {
            int index = (int)action;

            if ((index < 0) || (index > _actionNames.Length))
            {
                throw new ArgumentException("action");
            }

            return _actionNames[index];
        }

        public enum GamePadButtons
        {
            Start,
            Back,
            A,
            B,
            X,
            Y,
            Up,
            Down,
            Left,
            Right,
            LeftShoulder,
            RightShoulder,
            LeftTrigger,
            RightTrigger
        }

        public class ActionMap
        {
            /// <summary>
            /// List of GamePad controls to be mapped to a given action.
            /// </summary>
            public List<GamePadButtons> gamePadButtons = new();


            /// <summary>
            /// List of Keyboard controls to be mapped to a given action.
            /// </summary>
            public List<Keys> keyboardKeys = new();
        }

        const float _analogLimit = 0.5f;

        private static KeyboardState _currentKeyboardState;
        public static KeyboardState CurrentKeyboardState => _currentKeyboardState;

        private static KeyboardState _previousKeyboardState;

        public static bool IsKeyPressed(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key);
        }

        public static bool IsKeyTriggered(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key) && (!_previousKeyboardState.IsKeyDown(key));
        }

        private static GamePadState _currentGamePadState;
        public static GamePadState CurrentGamePadState => _currentGamePadState;

        private static GamePadState _previousGamePadState;

        public static bool IsGamePadStartPressed()
        {
            return _currentGamePadState.Buttons.Start == ButtonState.Pressed;
        }

        public static bool IsGamePadBackPressed()
        {
            return _currentGamePadState.Buttons.Back == ButtonState.Pressed;
        }

        public static bool IsGamePadAPressed()
        {
            return _currentGamePadState.Buttons.A == ButtonState.Pressed;
        }

        public static bool IsGamePadBPressed()
        {
            return _currentGamePadState.Buttons.B == ButtonState.Pressed;
        }

        public static bool IsGamePadXPressed()
        {
            return _currentGamePadState.Buttons.X == ButtonState.Pressed;
        }

        public static bool IsGamePadYPressed()
        {
            return _currentGamePadState.Buttons.Y == ButtonState.Pressed;
        }

        public static bool IsGamePadLeftShoulderPressed()
        {
            return _currentGamePadState.Buttons.LeftShoulder == ButtonState.Pressed;
        }

        public static bool IsGamePadRightShoulderPressed()
        {
            return _currentGamePadState.Buttons.RightShoulder == ButtonState.Pressed;
        }

        public static bool IsGamePadDPadUpPressed()
        {
            return _currentGamePadState.DPad.Up == ButtonState.Pressed;
        }

        public static bool IsGamePadDPadDownPressed()
        {
            return _currentGamePadState.DPad.Down == ButtonState.Pressed;
        }

        public static bool IsGamePadDPadLeftPressed()
        {
            return _currentGamePadState.DPad.Left == ButtonState.Pressed;
        }

        public static bool IsGamePadDPadRightPressed()
        {
            return _currentGamePadState.DPad.Right == ButtonState.Pressed;
        }

        public static bool IsGamePadLeftTriggerPressed()
        {
            return _currentGamePadState.Triggers.Left > _analogLimit;
        }

        public static bool IsGamePadRightTriggerPressed()
        {
            return _currentGamePadState.Triggers.Right > _analogLimit;
        }

        public static bool IsGamePadLeftStickUpPressed()
        {
            return _currentGamePadState.ThumbSticks.Left.Y > _analogLimit;
        }

        public static bool IsGamePadLeftStickDownPressed()
        {
            return -1f * _currentGamePadState.ThumbSticks.Left.Y > _analogLimit;
        }

        public static bool IsGamePadLeftStickLeftPressed()
        {
            return -1f * _currentGamePadState.ThumbSticks.Left.X > _analogLimit;
        }

        public static bool IsGamePadLeftStickRightPressed()
        {
            return _currentGamePadState.ThumbSticks.Left.X > _analogLimit;
        }

        private static bool IsGamePadButtonPressed(GamePadButtons gamePadKey)
        {
            switch (gamePadKey)
            {
                case GamePadButtons.Start:
                    return IsGamePadStartPressed();

                case GamePadButtons.Back:
                    return IsGamePadBackPressed();

                case GamePadButtons.A:
                    return IsGamePadAPressed();

                case GamePadButtons.B:
                    return IsGamePadBPressed();

                case GamePadButtons.X:
                    return IsGamePadXPressed();

                case GamePadButtons.Y:
                    return IsGamePadYPressed();

                case GamePadButtons.LeftShoulder:
                    return IsGamePadLeftShoulderPressed();

                case GamePadButtons.RightShoulder:
                    return IsGamePadRightShoulderPressed();

                case GamePadButtons.LeftTrigger:
                    return IsGamePadLeftTriggerPressed();

                case GamePadButtons.RightTrigger:
                    return IsGamePadRightTriggerPressed();

                case GamePadButtons.Up:
                    return IsGamePadDPadUpPressed() ||
                        IsGamePadLeftStickUpPressed();

                case GamePadButtons.Down:
                    return IsGamePadDPadDownPressed() ||
                        IsGamePadLeftStickDownPressed();

                case GamePadButtons.Left:
                    return IsGamePadDPadLeftPressed() ||
                        IsGamePadLeftStickLeftPressed();

                case GamePadButtons.Right:
                    return IsGamePadDPadRightPressed() ||
                        IsGamePadLeftStickRightPressed();
            }

            return false;
        }

        public static bool IsGamePadStartTriggered()
        {
            return (_currentGamePadState.Buttons.Start == ButtonState.Pressed) && (_previousGamePadState.Buttons.Start == ButtonState.Released);
        }

        public static bool IsGamePadBackTriggered()
        {
            return (_currentGamePadState.Buttons.Back == ButtonState.Pressed) && (_previousGamePadState.Buttons.Back == ButtonState.Released);
        }

        public static bool IsGamePadATriggered()
        {
            return (_currentGamePadState.Buttons.A == ButtonState.Pressed) && (_previousGamePadState.Buttons.A == ButtonState.Released);
        }

        public static bool IsGamePadBTriggered()
        {
            return (_currentGamePadState.Buttons.B == ButtonState.Pressed) && (_previousGamePadState.Buttons.B == ButtonState.Released);
        }

        public static bool IsGamePadXTriggered()
        {
            return (_currentGamePadState.Buttons.X == ButtonState.Pressed) && (_previousGamePadState.Buttons.X == ButtonState.Released);
        }

        public static bool IsGamePadYTriggered()
        {
            return (_currentGamePadState.Buttons.Y == ButtonState.Pressed) && (_previousGamePadState.Buttons.Y == ButtonState.Released);
        }

        public static bool IsGamePadLeftShoulderTriggered()
        {
            return (_currentGamePadState.Buttons.LeftShoulder == ButtonState.Pressed) && (_previousGamePadState.Buttons.LeftShoulder == ButtonState.Released);
        }

        public static bool IsGamePadRightShoulderTriggered()
        {
            return (_currentGamePadState.Buttons.RightShoulder == ButtonState.Pressed) && (_previousGamePadState.Buttons.RightShoulder == ButtonState.Released);
        }

        public static bool IsGamePadDPadUpTriggered()
        {
            return (_currentGamePadState.DPad.Up == ButtonState.Pressed) && (_previousGamePadState.DPad.Up == ButtonState.Released);
        }

        public static bool IsGamePadDPadDownTriggered()
        {
            return (_currentGamePadState.DPad.Down == ButtonState.Pressed) && (_previousGamePadState.DPad.Down == ButtonState.Released);
        }

        public static bool IsGamePadDPadLeftTriggered()
        {
            return (_currentGamePadState.DPad.Left == ButtonState.Pressed) && (_previousGamePadState.DPad.Left == ButtonState.Released);
        }

        public static bool IsGamePadDPadRightTriggered()
        {
            return (_currentGamePadState.DPad.Right == ButtonState.Pressed) && (_previousGamePadState.DPad.Right == ButtonState.Released);
        }

        public static bool IsGamePadLeftTriggerTriggered()
        {
            return (_currentGamePadState.Triggers.Left > _analogLimit) && (_previousGamePadState.Triggers.Left < _analogLimit);
        }

        public static bool IsGamePadRightTriggerTriggered()
        {
            return (_currentGamePadState.Triggers.Right > _analogLimit) && (_previousGamePadState.Triggers.Right < _analogLimit);
        }

        public static bool IsGamePadLeftStickUpTriggered()
        {
            return (_currentGamePadState.ThumbSticks.Left.Y > _analogLimit) && (_previousGamePadState.ThumbSticks.Left.Y < _analogLimit);
        }

        public static bool IsGamePadLeftStickDownTriggered()
        {
            return (-1f * _currentGamePadState.ThumbSticks.Left.Y > _analogLimit) && (-1f * _previousGamePadState.ThumbSticks.Left.Y < _analogLimit);
        }

        public static bool IsGamePadLeftStickLeftTriggered()
        {
            return (-1f * _currentGamePadState.ThumbSticks.Left.X > _analogLimit) && (-1f * _previousGamePadState.ThumbSticks.Left.X < _analogLimit);
        }

        public static bool IsGamePadLeftStickRightTriggered()
        {
            return (_currentGamePadState.ThumbSticks.Left.X > _analogLimit) && (_previousGamePadState.ThumbSticks.Left.X < _analogLimit);
        }

        private static bool IsGamePadButtonTriggered(GamePadButtons gamePadKey)
        {
            switch (gamePadKey)
            {
                case GamePadButtons.Start:
                    return IsGamePadStartTriggered();

                case GamePadButtons.Back:
                    return IsGamePadBackTriggered();

                case GamePadButtons.A:
                    return IsGamePadATriggered();

                case GamePadButtons.B:
                    return IsGamePadBTriggered();

                case GamePadButtons.X:
                    return IsGamePadXTriggered();

                case GamePadButtons.Y:
                    return IsGamePadYTriggered();

                case GamePadButtons.LeftShoulder:
                    return IsGamePadLeftShoulderTriggered();

                case GamePadButtons.RightShoulder:
                    return IsGamePadRightShoulderTriggered();

                case GamePadButtons.LeftTrigger:
                    return IsGamePadLeftTriggerTriggered();

                case GamePadButtons.RightTrigger:
                    return IsGamePadRightTriggerTriggered();

                case GamePadButtons.Up:
                    return IsGamePadDPadUpTriggered() || IsGamePadLeftStickUpTriggered();

                case GamePadButtons.Down:
                    return IsGamePadDPadDownTriggered() || IsGamePadLeftStickDownTriggered();

                case GamePadButtons.Left:
                    return IsGamePadDPadLeftTriggered() || IsGamePadLeftStickLeftTriggered();

                case GamePadButtons.Right:
                    return IsGamePadDPadRightTriggered() || IsGamePadLeftStickRightTriggered();
            }

            return false;
        }

        private static ActionMap[] _actionMaps;

        public static ActionMap[] ActionMaps
        {
            get { return _actionMaps; }
        }

        private static void ResetActionMaps()
        {
            _actionMaps = new ActionMap[(int)Action.TotalActionCount];

            _actionMaps[(int)Action.MainMenu] = new ActionMap();
            _actionMaps[(int)Action.MainMenu].keyboardKeys.Add(Keys.Tab);
            _actionMaps[(int)Action.MainMenu].gamePadButtons.Add(GamePadButtons.Start);

            _actionMaps[(int)Action.Ok] = new ActionMap();
            _actionMaps[(int)Action.Ok].keyboardKeys.Add(Keys.Enter);
            _actionMaps[(int)Action.Ok].gamePadButtons.Add(GamePadButtons.A);

            _actionMaps[(int)Action.Back] = new ActionMap();
            _actionMaps[(int)Action.Back].keyboardKeys.Add(Keys.Escape);
            _actionMaps[(int)Action.Back].gamePadButtons.Add(GamePadButtons.B);

            _actionMaps[(int)Action.CharacterManagement] = new ActionMap();
            _actionMaps[(int)Action.CharacterManagement].keyboardKeys.Add(Keys.Space);
            _actionMaps[(int)Action.CharacterManagement].gamePadButtons.Add(GamePadButtons.Y);

            _actionMaps[(int)Action.ExitGame] = new ActionMap();
            _actionMaps[(int)Action.ExitGame].keyboardKeys.Add(Keys.Escape);
            _actionMaps[(int)Action.ExitGame].gamePadButtons.Add(GamePadButtons.Back);

            _actionMaps[(int)Action.TakeView] = new ActionMap();
            _actionMaps[(int)Action.TakeView].keyboardKeys.Add(Keys.LeftControl);
            _actionMaps[(int)Action.TakeView].gamePadButtons.Add(GamePadButtons.Y);

            _actionMaps[(int)Action.DropUnEquip] = new ActionMap();
            _actionMaps[(int)Action.DropUnEquip].keyboardKeys.Add(Keys.D);
            _actionMaps[(int)Action.DropUnEquip].gamePadButtons.Add(GamePadButtons.X);

            _actionMaps[(int)Action.MoveCharacterUp] = new ActionMap();
            _actionMaps[(int)Action.MoveCharacterUp].keyboardKeys.Add(Keys.Up);
            _actionMaps[(int)Action.MoveCharacterUp].gamePadButtons.Add(GamePadButtons.Up);

            _actionMaps[(int)Action.MoveCharacterDown] = new ActionMap();
            _actionMaps[(int)Action.MoveCharacterDown].keyboardKeys.Add(Keys.Down);
            _actionMaps[(int)Action.MoveCharacterDown].gamePadButtons.Add(GamePadButtons.Down);

            _actionMaps[(int)Action.MoveCharacterLeft] = new ActionMap();
            _actionMaps[(int)Action.MoveCharacterLeft].keyboardKeys.Add(Keys.Left);
            _actionMaps[(int)Action.MoveCharacterLeft].gamePadButtons.Add(GamePadButtons.Left);

            _actionMaps[(int)Action.MoveCharacterRight] = new ActionMap();
            _actionMaps[(int)Action.MoveCharacterRight].keyboardKeys.Add(Keys.Right);
            _actionMaps[(int)Action.MoveCharacterRight].gamePadButtons.Add(GamePadButtons.Right);

            _actionMaps[(int)Action.CursorUp] = new ActionMap();
            _actionMaps[(int)Action.CursorUp].keyboardKeys.Add(Keys.Up);
            _actionMaps[(int)Action.CursorUp].gamePadButtons.Add(GamePadButtons.Up);

            _actionMaps[(int)Action.CursorDown] = new ActionMap();
            _actionMaps[(int)Action.CursorDown].keyboardKeys.Add(Keys.Down);
            _actionMaps[(int)Action.CursorDown].gamePadButtons.Add(GamePadButtons.Down);

            _actionMaps[(int)Action.DecreaseAmount] = new ActionMap();
            _actionMaps[(int)Action.DecreaseAmount].keyboardKeys.Add(Keys.Left);
            _actionMaps[(int)Action.DecreaseAmount].gamePadButtons.Add(GamePadButtons.Left);

            _actionMaps[(int)Action.IncreaseAmount] = new ActionMap();
            _actionMaps[(int)Action.IncreaseAmount].keyboardKeys.Add(Keys.Right);
            _actionMaps[(int)Action.IncreaseAmount].gamePadButtons.Add(GamePadButtons.Right);

            _actionMaps[(int)Action.PageLeft] = new ActionMap();
            _actionMaps[(int)Action.PageLeft].keyboardKeys.Add(Keys.LeftShift);
            _actionMaps[(int)Action.PageLeft].gamePadButtons.Add(GamePadButtons.LeftTrigger);

            _actionMaps[(int)Action.PageRight] = new ActionMap();
            _actionMaps[(int)Action.PageRight].keyboardKeys.Add(Keys.RightShift);
            _actionMaps[(int)Action.PageRight].gamePadButtons.Add(GamePadButtons.RightTrigger);

            _actionMaps[(int)Action.TargetUp] = new ActionMap();
            _actionMaps[(int)Action.TargetUp].keyboardKeys.Add(Keys.Up);
            _actionMaps[(int)Action.TargetUp].gamePadButtons.Add(GamePadButtons.Up);

            _actionMaps[(int)Action.TargetDown] = new ActionMap();
            _actionMaps[(int)Action.TargetDown].keyboardKeys.Add(Keys.Down);
            _actionMaps[(int)Action.TargetDown].gamePadButtons.Add(GamePadButtons.Down);

            _actionMaps[(int)Action.ActiveCharacterLeft] = new ActionMap();
            _actionMaps[(int)Action.ActiveCharacterLeft].keyboardKeys.Add(Keys.Left);
            _actionMaps[(int)Action.ActiveCharacterLeft].gamePadButtons.Add(GamePadButtons.Left);

            _actionMaps[(int)Action.ActiveCharacterRight] = new ActionMap();
            _actionMaps[(int)Action.ActiveCharacterRight].keyboardKeys.Add(Keys.Right);
            _actionMaps[(int)Action.ActiveCharacterRight].gamePadButtons.Add(GamePadButtons.Right);
        }

        public static bool IsActionPressed(Action action)
        {
            return IsActionMapPressed(_actionMaps[(int)action]);
        }

        public static bool IsActionTriggered(Action action)
        {
            return IsActionMapTriggered(_actionMaps[(int)action]);
        }

        private static bool IsActionMapPressed(ActionMap actionMap)
        {
            for (int i = 0; i < actionMap.keyboardKeys.Count; i++)
            {
                if (IsKeyPressed(actionMap.keyboardKeys[i]))
                {
                    return true;
                }
            }
            if (_currentGamePadState.IsConnected)
            {
                for (int i = 0; i < actionMap.gamePadButtons.Count; i++)
                {
                    if (IsGamePadButtonPressed(actionMap.gamePadButtons[i]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool IsActionMapTriggered(ActionMap actionMap)
        {
            for (int i = 0; i < actionMap.keyboardKeys.Count; i++)
            {
                if (IsKeyTriggered(actionMap.keyboardKeys[i]))
                {
                    return true;
                }
            }
            if (_currentGamePadState.IsConnected)
            {
                for (int i = 0; i < actionMap.gamePadButtons.Count; i++)
                {
                    if (IsGamePadButtonTriggered(actionMap.gamePadButtons[i]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void Initialize()
        {
            ResetActionMaps();
        }

        public static void Update()
        {
            // update the keyboard state
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            // update the gamepad state
            _previousGamePadState = _currentGamePadState;
            _currentGamePadState = GamePad.GetState(PlayerIndex.One);
        }
    }
}
