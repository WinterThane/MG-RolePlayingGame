using System;
using System.Collections.Generic;

namespace RolePlayingGame.Data
{
    public class StatisticsValueStack
    {
        /// <summary>
        /// One entry in the stack.
        /// </summary>
        private class StatisticsValueStackEntry
        {
            public StatisticsValue Statistics;
            public int RemainingDuration;
        }

        /// <summary>
        /// One entry in the stack.
        /// </summary>
        private List<StatisticsValueStackEntry> _entriesList = new();

        /// <summary>
        /// The total of all unexpired statistics in the stack.
        /// </summary>
        private StatisticsValue _totalStatistics = new();

        /// <summary>
        /// The total of all unexpired statistics in the stack.
        /// </summary>
        public StatisticsValue TotalStatistics => _totalStatistics;

        /// <summary>
        /// Calculate the total of all unexpired entries.
        /// </summary>
        private void CalculateTotalStatistics()
        {
            _totalStatistics = new StatisticsValue();
            foreach (StatisticsValueStackEntry entry in _entriesList)
            {
                _totalStatistics += entry.Statistics;
            }
        }

        /// <summary>
        /// Add a new statistics, with a given duration, to the stack.
        /// </summary>
        /// <remarks>Entries with durations of 0 or less never expire.</remarks>
        public void AddStatistics(StatisticsValue statistics, int duration)
        {
            if (duration < 0)
            {
                throw new ArgumentOutOfRangeException("duration");
            }

            StatisticsValueStackEntry entry = new()
            {
                Statistics = statistics,
                RemainingDuration = duration
            };

            _entriesList.Add(entry);

            CalculateTotalStatistics();
        }


        /// <summary>
        /// Advance the stack and remove expired entries.
        /// </summary>
        public void Advance()
        {
            // remove the entries at 1 - they are about to go to zero
            // -- values that are zero now, never expire
            _entriesList.RemoveAll(delegate (StatisticsValueStackEntry entry)
            {
                return (entry.RemainingDuration == 1);
            });

            // decrement all of the remaining entries.
            foreach (StatisticsValueStackEntry entry in _entriesList)
            {
                entry.RemainingDuration--;
            }

            // recalculate the total
            CalculateTotalStatistics();
        }
    }
}
