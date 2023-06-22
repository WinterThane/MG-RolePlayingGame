using RolePlayingGame.Characters;
using RolePlayingGame.InputsManager;
using RolePlayingGame.MapObjects;
using RolePlayingGame.SessionObjects;
using System;

namespace RolePlayingGame.ScreensManager.Screens.GameScreens
{
    public class PlayerNpcScreen : NpcScreen<Player>
    {
        /// <summary>
        /// If true, the NPC's introduction dialogue is shown.
        /// </summary>
        private bool _isIntroduction = true;

        /// <summary>
        /// Constructs a new PlayerNpcScreen object.
        /// </summary>
        /// <param name="mapEntry"></param>
        public PlayerNpcScreen(MapEntry<Player> mapEntry) : base(mapEntry)
        {
            // assign and check the parameter
            Player playerNpc = _character as Player;
            if (playerNpc == null)
            {
                throw new ArgumentException("PlayerNpcScreen requires a MapEntry with a Player");
            }

            DialogueText = playerNpc.IntroductionDialogue;
            BackText = "Reject";
            SelectText = "Accept";
            _isIntroduction = true;
        }

        /// <summary>
        /// Handles user input.
        /// </summary>
        public override void HandleInput()
        {
            // view the player's statistics
            if (InputManager.IsActionTriggered(InputManager.Action.TakeView))
            {
                ScreenManager.AddScreen(new StatisticsScreen(_character as Player));
                return;
            }

            if (_isIntroduction)
            {
                // accept the invitation
                if (InputManager.IsActionTriggered(InputManager.Action.Ok))
                {
                    _isIntroduction = false;
                    Player player = _character as Player;
                    Session.Party.JoinParty(player);
                    Session.RemovePlayerNpc(_mapEntry);
                    DialogueText = player.JoinAcceptedDialogue;
                    BackText = "Back";
                    SelectText = "Back";
                }
                // reject the invitation
                if (InputManager.IsActionTriggered(InputManager.Action.Back))
                {
                    _isIntroduction = false;
                    Player player = _character as Player;
                    DialogueText = player.JoinRejectedDialogue;
                    BackText = "Back";
                    SelectText = "Back";
                }
            }
            else
            {
                // exit the screen
                if (InputManager.IsActionTriggered(InputManager.Action.Ok) || InputManager.IsActionTriggered(InputManager.Action.Back))
                {
                    ExitScreen();
                    return;
                }
            }
        }
    }
}
