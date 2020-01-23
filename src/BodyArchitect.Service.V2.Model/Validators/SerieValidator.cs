using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Service.V2.Model.Validators
{
    public class SerieValidator
    {
        private Dictionary<ExerciseType, KeyValuePair<double, double>> limits;

        public SerieValidator()
        {
            limits = new Dictionary<ExerciseType, KeyValuePair<double, double>>();
            limits.Add(ExerciseType.Barki, new KeyValuePair<double, double>(30, 400));
            limits.Add(ExerciseType.Biceps, new KeyValuePair<double, double>(20, 200));
            limits.Add(ExerciseType.Brzuch, new KeyValuePair<double, double>(30, 200));
            limits.Add(ExerciseType.Czworoboczny, new KeyValuePair<double, double>(40, 500));
            limits.Add(ExerciseType.Klatka, new KeyValuePair<double, double>(50, 600));
            limits.Add(ExerciseType.Lydki, new KeyValuePair<double, double>(50, 600));
            limits.Add(ExerciseType.Nogi, new KeyValuePair<double, double>(50, 600));
            limits.Add(ExerciseType.Plecy, new KeyValuePair<double, double>(50, 600));
            limits.Add(ExerciseType.Przedramie, new KeyValuePair<double, double>(20, 200));
            limits.Add(ExerciseType.Triceps, new KeyValuePair<double, double>(20, 300));
        }

        public bool IsCorrect(SerieDTO serie)
        {
            var incorrectSets = GetIncorrectSets(serie.StrengthTrainingItem);
            return !incorrectSets.Contains(serie);
        }

        public static IList<SerieDTO> GetIncorrectSets(StrengthTrainingEntryDTO entry)
        {
            List<SerieDTO> incorrectSets = new List<SerieDTO>();
            if(entry==null)
            {//we delete this entry
                return incorrectSets;
            }
            SerieValidator validator = new SerieValidator();
            
            foreach (var item in entry.Entries)
            {
                incorrectSets.AddRange(validator.GetIncorrectSets(item));
            }
            return incorrectSets;
        }

        public IList<SerieDTO> GetIncorrectSets(StrengthTrainingItemDTO item)
        {
            List<SerieDTO> incorrectSets = new List<SerieDTO>();
            if (!limits.ContainsKey(item.Exercise.ExerciseType))
            {
                return incorrectSets;
            }
            var allWithWeight=item.Series.Where(x => x.Weight != null);
            var limit = limits[item.Exercise.ExerciseType];
            var wrongBecauseOfMaxLimit = allWithWeight.Where(x => x.Weight.Value >= (decimal)limit.Value).ToArray();
            incorrectSets.AddRange(wrongBecauseOfMaxLimit);

            //var afterLimit = item.Series.Where(x => x.Weight.HasValue && x.Weight.Value < (decimal)limit.Item2).ToArray();
            var afterLimit = allWithWeight.Where(x => !incorrectSets.Contains(x)).ToArray();
            if (afterLimit.Length < 3)
            {
                return incorrectSets;
            }


            var withoutMax = afterLimit.Select(x => (double)x.Weight.Value).OrderBy(x => x).Take(afterLimit.Length - 1).ToList();
            var max = withoutMax.Last();
            withoutMax.Add(max + limit.Key);

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



        //public bool IsCorrect(SerieDTO serie)
        //{
        //    var incorrectSets = GetIncorrectSets(serie.StrengthTrainingItem);
        //    return !incorrectSets.Contains(serie);
        //}

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
