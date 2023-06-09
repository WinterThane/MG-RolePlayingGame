﻿using RolePlayingGame.Combat.Actions;
using RolePlayingGame.Data;
using RolePlayingGame.SessionObjects;
using RolePlayingGame.Spells;
using System;
using System.Collections.Generic;

namespace RolePlayingGame.Combat
{
    public class ArtificialIntelligence
    {
        /// <summary>
        /// The monster that this object is choosing actions for.
        /// </summary>
        private CombatantMonster _monster;

        /// <summary>
        /// The offensive actions available to the monster.
        /// </summary>
        private List<CombatAction> _offensiveActions = new();

        /// <summary>
        /// The defensive actions available to the monster.
        /// </summary>
        private List<CombatAction> _defensiveActions = new();

        /// <summary>
        /// Construct a new ArtificialIntelligence object to control a given combatant.
        /// </summary>
        public ArtificialIntelligence(CombatantMonster monster)
        {
            // check the parameter
            if (monster == null)
            {
                throw new ArgumentNullException("monster");
            }

            // assign the parameter
            _monster = monster;

            // generate all actions available
            GenerateAllActions();
        }

        /// <summary>
        /// Generate the actions available to this monster.
        /// </summary>
        private void GenerateAllActions()
        {
            // clear out any pre-existing actions
            _offensiveActions.Clear();
            _defensiveActions.Clear();

            // generate the melee attack option
            GenerateMeleeAction();

            // generate the spell attack options
            GenerateSpellAttackActions();

            // generate the defend action
            GenerateDefendAction();

            // sort the lists by potential, descending
            _offensiveActions.Sort(CombatAction.CompareCombatActionsByHeuristic);
            _defensiveActions.Sort(CombatAction.CompareCombatActionsByHeuristic);
        }

        /// <summary>
        /// Generate the melee attack option for this monster.
        /// </summary>
        private void GenerateMeleeAction()
        {
            // add a new melee action to the list
            _offensiveActions.Add(new MeleeCombatAction(_monster));
        }

        /// <summary>
        /// Generate the melee attack option for this monster.
        /// </summary>
        private void GenerateDefendAction()
        {
            // add a new melee action to the list
            _defensiveActions.Add(new DefendCombatAction(_monster));
        }

        /// <summary>
        /// Generate the spell attack options for this monster.
        /// </summary>
        private void GenerateSpellAttackActions()
        {
            // retrieve the spells for this monster
            List<Spell> spells = _monster.Monster.Spells;

            // if there are no spells, then there's nothing to do
            if ((spells == null) || (spells.Count <= 0))
            {
                return;
            }

            // check each spell for attack actions
            foreach (Spell spell in spells)
            {
                // skip non-offensive spells
                if (!spell.IsOffensive)
                {
                    continue;
                }

                // add the new action to the list
                _offensiveActions.Add(new SpellCombatAction(_monster, spell));
            }
        }

        /// <summary>
        /// Choose the next action for the monster.
        /// </summary>
        /// <returns>The chosen action, or null if no action is desired.</returns>
        public CombatAction ChooseAction()
        {
            CombatAction combatAction = null;

            // determine if the monster will use a defensive action
            if ((_monster.Monster.DefendPercentage > 0) && (_defensiveActions.Count > 0) && (Session.Random.Next(0, 100) < _monster.Monster.DefendPercentage))
            {
                combatAction = ChooseDefensiveAction();
            }

            // if we do not have an action yet, choose an offensive action
            combatAction = (combatAction ?? ChooseOffensiveAction());

            // reset the action to the initial state
            combatAction.Reset();

            return combatAction;
        }

        /// <summary>
        /// Choose which offensive action to perform.
        /// </summary>
        /// <returns>The chosen action, or null if no action is desired.</returns>
        private CombatAction ChooseOffensiveAction()
        {
            List<CombatantPlayer> players = CombatEngine.Players;

            // be sure that there is a valid combat in progress
            if ((players == null) || (players.Count <= 0))
            {
                return null;
            }

            // randomly choose a living target from the party
            int targetIndex;
            do
            {
                targetIndex = Session.Random.Next(players.Count);
            }
            while (players[targetIndex].IsDeadOrDying);
            CombatantPlayer target = players[targetIndex];

            // the action lists are sorted by descending potential, 
            // so find the first eligible action
            foreach (CombatAction action in _offensiveActions)
            {
                // check the restrictions on the action
                if (action.IsCharacterValidUser)
                {
                    action.Target = target;
                    return action;
                }
            }

            // no eligible actions found
            return null;
        }

        /// <summary>
        /// Choose which defensive action to perform.
        /// </summary>
        /// <returns>The chosen action, or null if no action is desired.</returns>
        private CombatAction ChooseDefensiveAction()
        {
            List<CombatantMonster> monsters = CombatEngine.Monsters;

            // be sure that there is a valid combat in progress
            if ((monsters == null) || (monsters.Count <= 0))
            {
                return null;
            }

            // find the monster with the least health
            CombatantMonster target = null;
            int leastHealthAmount = Int32.MaxValue;
            foreach (CombatantMonster targetMonster in monsters)
            {
                // skip dead or dying targets
                if (targetMonster.IsDeadOrDying)
                {
                    continue;
                }
                // if the monster is damaged and it has the least health points,
                // then it becomes the new target
                StatisticsValue maxStatistics = targetMonster.Monster.CharacterClass.GetStatisticsForLevel(targetMonster.Monster.CharacterLevel);
                int targetMonsterHealthPoints = targetMonster.Statistics.HealthPoints;
                if ((targetMonsterHealthPoints < maxStatistics.HealthPoints) && (targetMonsterHealthPoints < leastHealthAmount))
                {
                    target = targetMonster;
                    leastHealthAmount = targetMonsterHealthPoints;
                }
            }

            // if there is no target, then don't do anything
            if (target == null)
            {
                return null;
            }

            // the action lists are sorted by descending potential, 
            // so find the first eligible action
            foreach (CombatAction action in _defensiveActions)
            {
                // check the restrictions on the action
                if (action.IsCharacterValidUser)
                {
                    action.Target = target;
                    return action;
                }
            }

            // no eligible actions found
            return null;
        }
    }
}
