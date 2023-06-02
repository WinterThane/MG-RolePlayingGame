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

        private static readonly string[] actionNames =
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

            if ((index < 0) || (index > actionNames.Length))
            {
                throw new ArgumentException("action");
            }

            return actionNames[index];
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

        const float analogLimit = 0.5f;

        private static KeyboardState currentKeyboardState;
        public static KeyboardState CurrentKeyboardState
        {
            get { return currentKeyboardState; }
        }

        private static KeyboardState previousKeyboardState;

        public static bool IsKeyPressed(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key);
        }

        public static bool IsKeyTriggered(Keys key)
        {
            return (currentKeyboardState.IsKeyDown(key)) && (!previousKeyboardState.IsKeyDown(key));
        }

        private static GamePadState currentGamePadState;
        public static GamePadState CurrentGamePadState
        {
            get { return currentGamePadState; }
        }

        private static GamePadState previousGamePadState;

        public static bool IsGamePadStartPressed()
        {
            return (currentGamePadState.Buttons.Start == ButtonState.Pressed);
        }

        public static bool IsGamePadBackPressed()
        {
            return (currentGamePadState.Buttons.Back == ButtonState.Pressed);
        }

        public static bool IsGamePadAPressed()
        {
            return (currentGamePadState.Buttons.A == ButtonState.Pressed);
        }

        public static bool IsGamePadBPressed()
        {
            return (currentGamePadState.Buttons.B == ButtonState.Pressed);
        }

        public static bool IsGamePadXPressed()
        {
            return (currentGamePadState.Buttons.X == ButtonState.Pressed);
        }

        public static bool IsGamePadYPressed()
        {
            return (currentGamePadState.Buttons.Y == ButtonState.Pressed);
        }

        public static bool IsGamePadLeftShoulderPressed()
        {
            return (currentGamePadState.Buttons.LeftShoulder == ButtonState.Pressed);
        }

        public static bool IsGamePadRightShoulderPressed()
        {
            return (currentGamePadState.Buttons.RightShoulder == ButtonState.Pressed);
        }

        public static bool IsGamePadDPadUpPressed()
        {
            return (currentGamePadState.DPad.Up == ButtonState.Pressed);
        }

        public static bool IsGamePadDPadDownPressed()
        {
            return (currentGamePadState.DPad.Down == ButtonState.Pressed);
        }

        public static bool IsGamePadDPadLeftPressed()
        {
            return (currentGamePadState.DPad.Left == ButtonState.Pressed);
        }

        public static bool IsGamePadDPadRightPressed()
        {
            return (currentGamePadState.DPad.Right == ButtonState.Pressed);
        }

        public static bool IsGamePadLeftTriggerPressed()
        {
            return (currentGamePadState.Triggers.Left > analogLimit);
        }

        public static bool IsGamePadRightTriggerPressed()
        {
            return (currentGamePadState.Triggers.Right > analogLimit);
        }

        public static bool IsGamePadLeftStickUpPressed()
        {
            return (currentGamePadState.ThumbSticks.Left.Y > analogLimit);
        }

        public static bool IsGamePadLeftStickDownPressed()
        {
            return (-1f * currentGamePadState.ThumbSticks.Left.Y > analogLimit);
        }

        public static bool IsGamePadLeftStickLeftPressed()
        {
            return (-1f * currentGamePadState.ThumbSticks.Left.X > analogLimit);
        }

        public static bool IsGamePadLeftStickRightPressed()
        {
            return (currentGamePadState.ThumbSticks.Left.X > analogLimit);
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
            return ((currentGamePadState.Buttons.Start == ButtonState.Pressed) &&
              (previousGamePadState.Buttons.Start == ButtonState.Released));
        }

        public static bool IsGamePadBackTriggered()
        {
            return ((currentGamePadState.Buttons.Back == ButtonState.Pressed) &&
              (previousGamePadState.Buttons.Back == ButtonState.Released));
        }

        public static bool IsGamePadATriggered()
        {
            return ((currentGamePadState.Buttons.A == ButtonState.Pressed) &&
              (previousGamePadState.Buttons.A == ButtonState.Released));
        }

        public static bool IsGamePadBTriggered()
        {
            return ((currentGamePadState.Buttons.B == ButtonState.Pressed) &&
              (previousGamePadState.Buttons.B == ButtonState.Released));
        }

        public static bool IsGamePadXTriggered()
        {
            return ((currentGamePadState.Buttons.X == ButtonState.Pressed) &&
              (previousGamePadState.Buttons.X == ButtonState.Released));
        }

        public static bool IsGamePadYTriggered()
        {
            return ((currentGamePadState.Buttons.Y == ButtonState.Pressed) &&
              (previousGamePadState.Buttons.Y == ButtonState.Released));
        }

        public static bool IsGamePadLeftShoulderTriggered()
        {
            return (
                (currentGamePadState.Buttons.LeftShoulder == ButtonState.Pressed) &&
                (previousGamePadState.Buttons.LeftShoulder == ButtonState.Released));
        }

        public static bool IsGamePadRightShoulderTriggered()
        {
            return (
                (currentGamePadState.Buttons.RightShoulder == ButtonState.Pressed) &&
                (previousGamePadState.Buttons.RightShoulder == ButtonState.Released));
        }

        public static bool IsGamePadDPadUpTriggered()
        {
            return ((currentGamePadState.DPad.Up == ButtonState.Pressed) &&
              (previousGamePadState.DPad.Up == ButtonState.Released));
        }

        public static bool IsGamePadDPadDownTriggered()
        {
            return ((currentGamePadState.DPad.Down == ButtonState.Pressed) &&
              (previousGamePadState.DPad.Down == ButtonState.Released));
        }

        public static bool IsGamePadDPadLeftTriggered()
        {
            return ((currentGamePadState.DPad.Left == ButtonState.Pressed) &&
              (previousGamePadState.DPad.Left == ButtonState.Released));
        }

        public static bool IsGamePadDPadRightTriggered()
        {
            return ((currentGamePadState.DPad.Right == ButtonState.Pressed) &&
              (previousGamePadState.DPad.Right == ButtonState.Released));
        }

        public static bool IsGamePadLeftTriggerTriggered()
        {
            return ((currentGamePadState.Triggers.Left > analogLimit) &&
                (previousGamePadState.Triggers.Left < analogLimit));
        }

        public static bool IsGamePadRightTriggerTriggered()
        {
            return ((currentGamePadState.Triggers.Right > analogLimit) &&
                (previousGamePadState.Triggers.Right < analogLimit));
        }

        public static bool IsGamePadLeftStickUpTriggered()
        {
            return ((currentGamePadState.ThumbSticks.Left.Y > analogLimit) &&
                (previousGamePadState.ThumbSticks.Left.Y < analogLimit));
        }

        public static bool IsGamePadLeftStickDownTriggered()
        {
            return ((-1f * currentGamePadState.ThumbSticks.Left.Y > analogLimit) &&
                (-1f * previousGamePadState.ThumbSticks.Left.Y < analogLimit));
        }

        public static bool IsGamePadLeftStickLeftTriggered()
        {
            return ((-1f * currentGamePadState.ThumbSticks.Left.X > analogLimit) &&
                (-1f * previousGamePadState.ThumbSticks.Left.X < analogLimit));
        }

        public static bool IsGamePadLeftStickRightTriggered()
        {
            return ((currentGamePadState.ThumbSticks.Left.X > analogLimit) &&
                (previousGamePadState.ThumbSticks.Left.X < analogLimit));
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
                    return IsGamePadDPadUpTriggered() ||
                        IsGamePadLeftStickUpTriggered();

                case GamePadButtons.Down:
                    return IsGamePadDPadDownTriggered() ||
                        IsGamePadLeftStickDownTriggered();

                case GamePadButtons.Left:
                    return IsGamePadDPadLeftTriggered() ||
                        IsGamePadLeftStickLeftTriggered();

                case GamePadButtons.Right:
                    return IsGamePadDPadRightTriggered() ||
                        IsGamePadLeftStickRightTriggered();
            }

            return false;
        }

        private static ActionMap[] actionMaps;

        public static ActionMap[] ActionMaps
        {
            get { return actionMaps; }
        }

        private static void ResetActionMaps()
        {
            actionMaps = new ActionMap[(int)Action.TotalActionCount];

            actionMaps[(int)Action.MainMenu] = new ActionMap();
            actionMaps[(int)Action.MainMenu].keyboardKeys.Add(
                Keys.Tab);
            actionMaps[(int)Action.MainMenu].gamePadButtons.Add(
                GamePadButtons.Start);

            actionMaps[(int)Action.Ok] = new ActionMap();
            actionMaps[(int)Action.Ok].keyboardKeys.Add(
                Keys.Enter);
            actionMaps[(int)Action.Ok].gamePadButtons.Add(
                GamePadButtons.A);

            actionMaps[(int)Action.Back] = new ActionMap();
            actionMaps[(int)Action.Back].keyboardKeys.Add(
                Keys.Escape);
            actionMaps[(int)Action.Back].gamePadButtons.Add(
                GamePadButtons.B);

            actionMaps[(int)Action.CharacterManagement] = new ActionMap();
            actionMaps[(int)Action.CharacterManagement].keyboardKeys.Add(
                Keys.Space);
            actionMaps[(int)Action.CharacterManagement].gamePadButtons.Add(
                GamePadButtons.Y);

            actionMaps[(int)Action.ExitGame] = new ActionMap();
            actionMaps[(int)Action.ExitGame].keyboardKeys.Add(
                Keys.Escape);
            actionMaps[(int)Action.ExitGame].gamePadButtons.Add(
                GamePadButtons.Back);

            actionMaps[(int)Action.TakeView] = new ActionMap();
            actionMaps[(int)Action.TakeView].keyboardKeys.Add(
                Keys.LeftControl);
            actionMaps[(int)Action.TakeView].gamePadButtons.Add(
                GamePadButtons.Y);

            actionMaps[(int)Action.DropUnEquip] = new ActionMap();
            actionMaps[(int)Action.DropUnEquip].keyboardKeys.Add(
                Keys.D);
            actionMaps[(int)Action.DropUnEquip].gamePadButtons.Add(
                GamePadButtons.X);

            actionMaps[(int)Action.MoveCharacterUp] = new ActionMap();
            actionMaps[(int)Action.MoveCharacterUp].keyboardKeys.Add(
                Keys.Up);
            actionMaps[(int)Action.MoveCharacterUp].gamePadButtons.Add(
                GamePadButtons.Up);

            actionMaps[(int)Action.MoveCharacterDown] = new ActionMap();
            actionMaps[(int)Action.MoveCharacterDown].keyboardKeys.Add(
                Keys.Down);
            actionMaps[(int)Action.MoveCharacterDown].gamePadButtons.Add(
                GamePadButtons.Down);

            actionMaps[(int)Action.MoveCharacterLeft] = new ActionMap();
            actionMaps[(int)Action.MoveCharacterLeft].keyboardKeys.Add(
                Keys.Left);
            actionMaps[(int)Action.MoveCharacterLeft].gamePadButtons.Add(
                GamePadButtons.Left);

            actionMaps[(int)Action.MoveCharacterRight] = new ActionMap();
            actionMaps[(int)Action.MoveCharacterRight].keyboardKeys.Add(
                Keys.Right);
            actionMaps[(int)Action.MoveCharacterRight].gamePadButtons.Add(
                GamePadButtons.Right);

            actionMaps[(int)Action.CursorUp] = new ActionMap();
            actionMaps[(int)Action.CursorUp].keyboardKeys.Add(
                Keys.Up);
            actionMaps[(int)Action.CursorUp].gamePadButtons.Add(
                GamePadButtons.Up);

            actionMaps[(int)Action.CursorDown] = new ActionMap();
            actionMaps[(int)Action.CursorDown].keyboardKeys.Add(
                Keys.Down);
            actionMaps[(int)Action.CursorDown].gamePadButtons.Add(
                GamePadButtons.Down);

            actionMaps[(int)Action.DecreaseAmount] = new ActionMap();
            actionMaps[(int)Action.DecreaseAmount].keyboardKeys.Add(
                Keys.Left);
            actionMaps[(int)Action.DecreaseAmount].gamePadButtons.Add(
                GamePadButtons.Left);

            actionMaps[(int)Action.IncreaseAmount] = new ActionMap();
            actionMaps[(int)Action.IncreaseAmount].keyboardKeys.Add(
                Keys.Right);
            actionMaps[(int)Action.IncreaseAmount].gamePadButtons.Add(
                GamePadButtons.Right);

            actionMaps[(int)Action.PageLeft] = new ActionMap();
            actionMaps[(int)Action.PageLeft].keyboardKeys.Add(
                Keys.LeftShift);
            actionMaps[(int)Action.PageLeft].gamePadButtons.Add(
                GamePadButtons.LeftTrigger);

            actionMaps[(int)Action.PageRight] = new ActionMap();
            actionMaps[(int)Action.PageRight].keyboardKeys.Add(
                Keys.RightShift);
            actionMaps[(int)Action.PageRight].gamePadButtons.Add(
                GamePadButtons.RightTrigger);

            actionMaps[(int)Action.TargetUp] = new ActionMap();
            actionMaps[(int)Action.TargetUp].keyboardKeys.Add(
                Keys.Up);
            actionMaps[(int)Action.TargetUp].gamePadButtons.Add(
                GamePadButtons.Up);

            actionMaps[(int)Action.TargetDown] = new ActionMap();
            actionMaps[(int)Action.TargetDown].keyboardKeys.Add(
                Keys.Down);
            actionMaps[(int)Action.TargetDown].gamePadButtons.Add(
                GamePadButtons.Down);

            actionMaps[(int)Action.ActiveCharacterLeft] = new ActionMap();
            actionMaps[(int)Action.ActiveCharacterLeft].keyboardKeys.Add(
                Keys.Left);
            actionMaps[(int)Action.ActiveCharacterLeft].gamePadButtons.Add(
                GamePadButtons.Left);

            actionMaps[(int)Action.ActiveCharacterRight] = new ActionMap();
            actionMaps[(int)Action.ActiveCharacterRight].keyboardKeys.Add(
                Keys.Right);
            actionMaps[(int)Action.ActiveCharacterRight].gamePadButtons.Add(
                GamePadButtons.Right);
        }

        public static bool IsActionPressed(Action action)
        {
            return IsActionMapPressed(actionMaps[(int)action]);
        }

        public static bool IsActionTriggered(Action action)
        {
            return IsActionMapTriggered(actionMaps[(int)action]);
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
            if (currentGamePadState.IsConnected)
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
            if (currentGamePadState.IsConnected)
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
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            // update the gamepad state
            previousGamePadState = currentGamePadState;
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
        }
    }
}
