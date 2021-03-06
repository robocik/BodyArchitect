﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BodyArchitect.Service.V2.Model
{
    public partial class WorkoutPlanSearchCriteria
    {
        public WorkoutPlanSearchCriteria()
        {
            SearchGroups = new List<WorkoutPlanSearchCriteriaGroup>();
            Days = new List<int>();
            WorkoutPlanType = new List<TrainingType>();
            Languages = new List<string>();
            Purposes = new List<WorkoutPlanPurpose>();
            Difficults = new List<TrainingPlanDifficult>();
        }
    }
}
