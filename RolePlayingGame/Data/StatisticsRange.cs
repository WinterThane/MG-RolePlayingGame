using Microsoft.Xna.Framework.Content;
using System;
using System.Text;

namespace RolePlayingGame.Data
{
    public class StatisticsRange
    {
        [ContentSerializer(Optional = true)]
        public Int32Range HealthPointsRange;

        [ContentSerializer(Optional = true)]
        public Int32Range MagicPointsRange;

        [ContentSerializer(Optional = true)]
        public Int32Range PhysicalOffenseRange;

        [ContentSerializer(Optional = true)]
        public Int32Range PhysicalDefenseRange;

        [ContentSerializer(Optional = true)]
        public Int32Range MagicalOffenseRange;

        [ContentSerializer(Optional = true)]
        public Int32Range MagicalDefenseRange;

        /// <summary>
        /// Generate a random value between the minimum and maximum, inclusively.
        /// </summary>
        /// <param name="random">The Random object used to generate the value.</param>
        public StatisticsValue GenerateValue(Random random)
        {
            // check the parameters
            Random usedRandom = random;
            if (usedRandom == null)
            {
                usedRandom = new Random();
            }

            // generate the new value
            StatisticsValue outputValue = new()
            {
                HealthPoints = HealthPointsRange.GenerateValue(usedRandom),
                MagicPoints = MagicPointsRange.GenerateValue(usedRandom),
                PhysicalOffense = PhysicalOffenseRange.GenerateValue(usedRandom),
                PhysicalDefense = PhysicalDefenseRange.GenerateValue(usedRandom),
                MagicalOffense = MagicalOffenseRange.GenerateValue(usedRandom),
                MagicalDefense = MagicalDefenseRange.GenerateValue(usedRandom)
            };

            return outputValue;
        }

        /// <summary>
        /// Builds a string that describes this object.
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new();

            sb.Append("HP:");
            sb.Append(HealthPointsRange.ToString());

            sb.Append("; MP:");
            sb.Append(MagicPointsRange.ToString());

            sb.Append("; PO:");
            sb.Append(PhysicalOffenseRange.ToString());

            sb.Append("; PD:");
            sb.Append(PhysicalDefenseRange.ToString());

            sb.Append("; MO:");
            sb.Append(MagicalOffenseRange.ToString());

            sb.Append("; MD:");
            sb.Append(MagicalDefenseRange.ToString());

            return sb.ToString();
        }

        /// <summary>
        /// Builds a string that describes a modifier, where non-zero stats are skipped.
        /// </summary>
        public string GetModifierString()
        {
            StringBuilder sb = new();
            bool firstStatistic = true;

            // add the health points value, if any
            if ((HealthPointsRange.Minimum != 0) || (HealthPointsRange.Maximum != 0))
            {
                if (firstStatistic)
                {
                    firstStatistic = false;
                }
                else
                {
                    sb.Append("; ");
                }
                sb.Append("HP:");
                sb.Append(HealthPointsRange.ToString());
            }

            // add the magic points value, if any
            if ((MagicPointsRange.Minimum != 0) || (MagicPointsRange.Maximum != 0))
            {
                if (firstStatistic)
                {
                    firstStatistic = false;
                }
                else
                {
                    sb.Append("; ");
                }
                sb.Append("MP:");
                sb.Append(MagicPointsRange.ToString());
            }

            // add the physical offense value, if any
            if ((PhysicalOffenseRange.Minimum != 0) || (PhysicalOffenseRange.Maximum != 0))
            {
                if (firstStatistic)
                {
                    firstStatistic = false;
                }
                else
                {
                    sb.Append("; ");
                }
                sb.Append("PO:");
                sb.Append(PhysicalOffenseRange.ToString());
            }

            // add the physical defense value, if any
            if ((PhysicalDefenseRange.Minimum != 0) || (PhysicalDefenseRange.Maximum != 0))
            {
                if (firstStatistic)
                {
                    firstStatistic = false;
                }
                else
                {
                    sb.Append("; ");
                }
                sb.Append("PD:");
                sb.Append(PhysicalDefenseRange.ToString());
            }

            // add the magical offense value, if any
            if ((MagicalOffenseRange.Minimum != 0) ||
                (MagicalOffenseRange.Maximum != 0))
            {
                if (firstStatistic)
                {
                    firstStatistic = false;
                }
                else
                {
                    sb.Append("; ");
                }
                sb.Append("MO:");
                sb.Append(MagicalOffenseRange.ToString());
            }

            // add the magical defense value, if any
            if ((MagicalDefenseRange.Minimum != 0) || (MagicalDefenseRange.Maximum != 0))
            {
                if (firstStatistic)
                {
                    firstStatistic = false;
                }
                else
                {
                    sb.Append("; ");
                }
                sb.Append("MD:");
                sb.Append(MagicalDefenseRange.ToString());
            }

            return sb.ToString();
        }

        /// <summary>
        /// Add one value to another, piecewise, and return the result.
        /// </summary>
        public static StatisticsRange Add(StatisticsRange value1, StatisticsValue value2)
        {
            StatisticsRange outputRange = new()
            {
                HealthPointsRange = value1.HealthPointsRange + value2.HealthPoints,
                MagicPointsRange = value1.MagicPointsRange + value2.MagicPoints,
                PhysicalOffenseRange = value1.PhysicalOffenseRange + value2.PhysicalOffense,
                PhysicalDefenseRange = value1.PhysicalDefenseRange + value2.PhysicalDefense,
                MagicalOffenseRange = value1.MagicalOffenseRange + value2.MagicalOffense,
                MagicalDefenseRange = value1.MagicalDefenseRange + value2.MagicalDefense
            };
            return outputRange;
        }

        /// <summary>
        /// Add one value to another, piecewise, and return the result.
        /// </summary>
        public static StatisticsRange operator +(StatisticsRange value1, StatisticsValue value2)
        {
            return Add(value1, value2);
        }

        /// <summary>
        /// Reads a StatisticsRange object from the content pipeline.
        /// </summary>
        public class StatisticsRangeReader : ContentTypeReader<StatisticsRange>
        {
            protected override StatisticsRange Read(ContentReader input, StatisticsRange existingInstance)
            {
                StatisticsRange output = new()
                {
                    HealthPointsRange = input.ReadObject<Int32Range>(),
                    MagicPointsRange = input.ReadObject<Int32Range>(),
                    PhysicalOffenseRange = input.ReadObject<Int32Range>(),
                    PhysicalDefenseRange = input.ReadObject<Int32Range>(),
                    MagicalOffenseRange = input.ReadObject<Int32Range>(),
                    MagicalDefenseRange = input.ReadObject<Int32Range>()
                };

                return output;
            }
        }
    }
}
