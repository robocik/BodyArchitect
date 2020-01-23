using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;

namespace BodyArchitect.DataAccess.Converter.V4_V5
{
    public class SerieValidator
    {
        private Dictionary<ExerciseType, Tuple<double, double>> limits;

        public SerieValidator()
        {
            limits = new Dictionary<ExerciseType, Tuple<double, double>>();
            limits.Add(ExerciseType.Barki, new Tuple<double, double>(30, 400));
            limits.Add(ExerciseType.Biceps, new Tuple<double, double>(20, 200));
            limits.Add(ExerciseType.Brzuch, new Tuple<double, double>(30, 200));
            limits.Add(ExerciseType.Czworoboczny, new Tuple<double, double>(40, 500));
            limits.Add(ExerciseType.Klatka, new Tuple<double, double>(50, 600));
            limits.Add(ExerciseType.Lydki, new Tuple<double, double>(50, 600));
            limits.Add(ExerciseType.Nogi, new Tuple<double, double>(50, 600));
            limits.Add(ExerciseType.Plecy, new Tuple<double, double>(50, 600));
            limits.Add(ExerciseType.Przedramie, new Tuple<double, double>(20, 200));
            limits.Add(ExerciseType.Triceps, new Tuple<double, double>(20, 300));
        }

        public IList<Serie> GetIncorrectSets(StrengthTrainingItem item)
        {
            List<Serie> incorrectSets = new List<Serie>();
            if (!limits.ContainsKey(item.Exercise.ExerciseType))
            {
                return incorrectSets;
            }
            var allWithWeight = item.Series.Where(x => x.Weight != null);
            var limit = limits[item.Exercise.ExerciseType];
            var wrongBecauseOfMaxLimit = allWithWeight.Where(x => x.Weight.Value >= (decimal)limit.Item2).ToArray();
            incorrectSets.AddRange(wrongBecauseOfMaxLimit);

            //var afterLimit = item.Series.Where(x => x.Weight.HasValue && x.Weight.Value < (decimal)limit.Item2).ToArray();
            var afterLimit = allWithWeight.Where(x => !incorrectSets.Contains(x)).ToArray();
            if (afterLimit.Length < 3)
            {
                return incorrectSets;
            }


            var withoutMax = afterLimit.Select(x => (double)x.Weight.Value).OrderBy(x => x).Take(afterLimit.Length - 1).ToList();
            var max = withoutMax.Last();
            withoutMax.Add(max + limit.Item1);

            //var avg = srednia(item.Series.Select(x => (double)x.Weight.Value).ToArray());
            var srWithoutMax = srednia(withoutMax.ToArray());
            var odchSdWithoutMax = odchylenie(srWithoutMax, withoutMax.ToArray());
            foreach (var set in afterLimit)
            {
                //var arr = item.Series.Where(x => x != set).Select(x => (double)x.Weight.Value).ToArray();
                //var sr = srednia(arr);
                //var odchStd = odchylenie(sr, arr);
                if (3 * odchSdWithoutMax < Math.Abs((double)set.Weight.Value - srWithoutMax))
                // if (Math.Abs(avg-sr)>70)
                {
                    incorrectSets.Add(set);
                }
            }
            return incorrectSets;
        }

        public bool IsCorrect(Serie serie)
        {
            var incorrectSets = GetIncorrectSets(serie.StrengthTrainingItem);
            return !incorrectSets.Contains(serie);
        }

        double srednia(params double[] liczby)
        {
            double suma = 0;
            foreach (var d in liczby)
            {
                suma += d;
            }
            return suma / liczby.Length;
        }

        double odchylenie(double srednia, double[] tab)
        {
            double dodaj = 0;
            double odchylenie = 0;
            for (int i = 0; i < tab.Length; i++)
            {
                dodaj = dodaj + (tab[i] - srednia) * (tab[i] - srednia);
            }


            odchylenie = Math.Sqrt(dodaj / tab.Length);
            return odchylenie;
        }
    }
}
