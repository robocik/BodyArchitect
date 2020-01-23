﻿using System;
using System.Collections.Generic;

namespace BodyArchitect.Model
{
    public class ExercisesBuilderEN : IExerciseBuilder
    {
        public List<Exercise> Create()
        {
            List<Exercise> list = new List<Exercise>();
            barki(list);
            klatka(list);
            plecy(list);
            nogi(list);
            lydki(list);
            brzuch(list);
            biceps(list);
            przedramie(list);
            triceps(list);
            return list;
        }

        private void triceps(List<Exercise> list)
        {
            Exercise exercise = new Exercise(new Guid("9D6A5C48-B2A3-4613-80BB-047DC770C896"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "Bench Dips";
            exercise.Shortcut = "TBD";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/bench-dips";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("11CD7D95-4B09-4506-BE71-2E9740E818E7"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "Cable Incline Triceps Extension";
            exercise.Shortcut = "TCIE";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/cable-incline-triceps-extension";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("E69A21C7-AB15-469D-888E-3FC87B7D50ED"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "Cable Lying Triceps Extension";
            exercise.Shortcut = "TCLE";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/cable-lying-triceps-extension";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("D5D746BA-5DAB-4375-9FB6-167D35DB161B"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "Close-Grip Barbell Bench Press";
            exercise.Shortcut = "TCGB";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/close-grip-barbell-bench-press";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("0D9AB7A4-C73B-4A83-89EB-CE3828AAB90B"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "Decline EZ Bar Triceps Extension";
            exercise.Shortcut = "TDBE";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/decline-ez-bar-triceps-extension";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("920912E8-B2B7-4F4A-B588-13F3E3E4763A"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "Dumbbell One-Arm Triceps Extension";
            exercise.Shortcut = "TDOE";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/dumbbell-one-arm-triceps-extension";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("1CDF4117-128F-49DD-927F-EB9E0B1BB4BB"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "Dips - Triceps Version";
            exercise.Shortcut = "TD";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/dips-triceps-version";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);

            exercise = new Exercise(new Guid("FBA79703-6FAF-4A5F-959A-8F50A068885F"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "Cable One Arm Tricep Extension";
            exercise.Shortcut = "TCOE";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/cable-one-arm-tricep-extension";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("036DD77D-A211-4476-BC87-A7DC1203A41E"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "Decline Close-Grip Bench To Skull Crusher";
            exercise.Shortcut = "TDCSC";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/decline-close-grip-bench-to-skull-crusher";
            exercise.Difficult = ExerciseDifficult.Three;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);
            exercise = new Exercise(new Guid("C46CA4C1-8E90-41EB-9396-E9441E7BB63B"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "Decline Dumbbell Triceps Extension";
            exercise.Shortcut = "TDDE";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/decline-dumbbell-triceps-extension";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("C98C899B-A4C4-4829-9AB2-2C7A783848DD"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "Dip Machine";
            exercise.Shortcut = "TDM";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/dip-machine";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("4D6B28B0-C341-43BA-9D69-441773828C90"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "Incline Barbell Triceps Extension";
            exercise.Shortcut = "TIBE";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/incline-barbell-triceps-extension";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("6637A887-230C-4E8C-8610-9045341E1E62"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "JM Press";
            exercise.Shortcut = "TJM";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/jm-press";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
        }

        private void przedramie(List<Exercise> list)
        {
            Exercise exercise = new Exercise(new Guid("B83D171F-18A7-4997-8F81-8F9A77C694DF"));
            exercise.ExerciseType = ExerciseType.Przedramie;
            exercise.Name = "Cable Wrist Curl";
            exercise.Shortcut = "FCC";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/cable-wrist-curl";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);
            exercise = new Exercise(new Guid("439E2060-488B-4DB8-9D09-09CDF2422D51"));
            exercise.ExerciseType = ExerciseType.Przedramie;
            exercise.Name = "Seated One-Arm Dumbbell Palms-Up Wrist Curl";
            exercise.Shortcut = "FSDC";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/seated-one-arm-dumbbell-palms-up-wrist-curl";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);
        }

        private void biceps(List<Exercise> list)
        {
            Exercise exercise = new Exercise(new Guid("0BE35E12-4838-4740-B67F-A789E4475BBC"));
            exercise.ExerciseType = ExerciseType.Biceps;
            exercise.Name = "Alternate Hammer Curl";
            exercise.Shortcut = "BAHC";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/alternate-hammer-curl";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);
            exercise = new Exercise(new Guid("E4F2FF97-D09F-4E2A-9D24-DCC58DFEC5A2"));
            exercise.ExerciseType = ExerciseType.Biceps;
            exercise.Name = "Barbell Curl";
            exercise.Shortcut = "BBC";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/barbell-curl";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);
            exercise = new Exercise(new Guid("1BCC52C4-5D63-4974-A68C-B16941F73C81"));
            exercise.ExerciseType = ExerciseType.Biceps;
            exercise.Name = "Concentration Curls";
            exercise.Shortcut = "BCC";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/concentration-curls";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);
            exercise = new Exercise(new Guid("B4555475-7A3A-4DD0-8F40-81897F66448B"));
            exercise.ExerciseType = ExerciseType.Biceps;
            exercise.Name = "Dumbbell Alternate Biceps Curl";
            exercise.Shortcut = "BDAC";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/dumbbell-alternate-bicep-curl";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);
            exercise = new Exercise(new Guid("198603B6-E379-43DD-9369-1984F7B2C5B2"));
            exercise.ExerciseType = ExerciseType.Biceps;
            exercise.Name = "Preacher Curl";
            exercise.Shortcut = "BPC";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/preacher-curl";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);
        }

        private void brzuch(List<Exercise> list)
        {
            Exercise exercise = new Exercise(new Guid("3930FC43-615B-46A7-AA46-066BE6CEC6F1"));
            exercise.ExerciseType = ExerciseType.Brzuch;
            exercise.Name = "ABS";
            exercise.Shortcut = "ABS";
            exercise.Url = "";
            exercise.Difficult = ExerciseDifficult.Two;
            list.Add(exercise);
            exercise = new Exercise(new Guid("0AD2F209-9A5D-4349-A652-42ADE4E3E389"));
            exercise.ExerciseType = ExerciseType.Brzuch;
            exercise.Name = "ABS2";
            exercise.Shortcut = "ABS2";
            exercise.Url = "";
            exercise.Difficult = ExerciseDifficult.Two;
            list.Add(exercise);
        }

        private void lydki(List<Exercise> list)
        {
            Exercise exercise = new Exercise(new Guid("CACE747B-0460-4EFA-942F-B4A2937D78D6"));
            exercise.ExerciseType = ExerciseType.Lydki;
            exercise.Name = "Barbell Seated Calf Raise";
            exercise.Shortcut = "CABSR";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/barbell-seated-calf-raise";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("1950E60F-4040-4EF9-A054-CE4254B97D4A"));
            exercise.ExerciseType = ExerciseType.Lydki;
            exercise.Name = "Calf Press On The Leg Press Machine";
            exercise.Shortcut = "CAPM";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/calf-press-on-the-leg-press-machine";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Push;

            exercise = new Exercise(new Guid("E91ECCED-3379-428A-AFA7-675FD3011341"));
            exercise.ExerciseType = ExerciseType.Lydki;
            exercise.Name = "Calf Press One The Leg Press Machine";
            exercise.Shortcut = "CLPM";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/dumbbell-seated-one-leg-calf-raise";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
        }

        private void nogi(List<Exercise> list)
        {
            Exercise exercise = new Exercise(new Guid("172C045D-BAAC-4EF3-A7BB-AF343BD4479E"));
            exercise.ExerciseType = ExerciseType.Nogi;
            exercise.Name = "Barbell Lunge";
            exercise.Shortcut = "LBL";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/barbell-lunge";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);
            exercise = new Exercise(new Guid("DD324460-8630-49D2-91A5-18E0B3BE98F5"));
            exercise.ExerciseType = ExerciseType.Nogi;
            exercise.Name = "Barbell Squat";
            exercise.Shortcut = "LBS";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/barbell-squat";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("F428F25B-D077-4123-B998-7F0984C29867"));
            exercise.ExerciseType = ExerciseType.Nogi;
            exercise.Name = "Dumbbell Step Ups";
            exercise.Shortcut = "LDSU";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/dumbbell-step-ups";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("0CF31AEA-DDDC-4FCC-AC95-97735DFDA280"));
            exercise.ExerciseType = ExerciseType.Nogi;
            exercise.Name = "Leg Extensions";
            exercise.Shortcut = "LE";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/leg-extensions";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("41B9CF36-ABF3-4140-B8CD-68477C68FAB7"));
            exercise.ExerciseType = ExerciseType.Nogi;
            exercise.Name = "Lying Machine Squat";
            exercise.Shortcut = "LLMS";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/lying-machine-squat";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
        }

        private void plecy(List<Exercise> list)
        {
            Exercise exercise = new Exercise(new Guid("F747A74E-A544-4FF2-B05E-77EA3476F72D"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "Chin-Up";
            exercise.Shortcut = "BaCU";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/chin-up";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);
            exercise = new Exercise(new Guid("95E2F7D6-D75B-4064-906F-E80E7616D8E9"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "Close-Grip Front Lat Pulldown";
            exercise.Shortcut = "BaCGF";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/close-grip-front-lat-pulldown";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);
            exercise = new Exercise(new Guid("533428A6-A2E4-41F9-B1AB-F2FE8450D38A"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "Pullups";
            exercise.Shortcut = "BaP";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/pullups";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);
            exercise = new Exercise(new Guid("BD8C45FA-0AAC-4A1F-8FC2-CEDA8C945858"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "Incline Bench Pull";
            exercise.Shortcut = "BaIBP";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/incline-bench-pull";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);
            exercise = new Exercise(new Guid("C1343295-0FED-419F-83AB-58F63FBA0AB7"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "Bent Over Two-Arm Long Bar Row";
            exercise.Shortcut = "BaBO";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/bent-over-two-arm-long-bar-row";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);
            exercise = new Exercise(new Guid("2A73061D-9090-4CDE-AAD8-5B7B89472556"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "One-Arm Dumbbell Row";
            exercise.Shortcut = "BBaDR";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/one-arm-dumbbell-row";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);
        }

        private void klatka(List<Exercise> list)
        {
            Exercise exercise = new Exercise(new Guid("BD06B342-C28A-4799-B889-FAEF80BDFFCB"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "Barbell Bench Press - Medium Grip";
            exercise.Shortcut = "CBPM";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/barbell-bench-press-medium-grip";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("916DAE2E-AC56-431C-9F2C-9FF408BDBA00"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "Butterfly";
            exercise.Shortcut = "CBF";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/butterfly";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);
            exercise = new Exercise(new Guid("DB7D7256-14C7-4CF2-AC5C-12C88295BFE7"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "Decline Barbell Bench Press";
            exercise.Shortcut = "CDBP";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/decline-barbell-bench-press";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("FD8F3C12-B4A9-4B94-B5DA-E67628531D2E"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "Incline Cable Flye";
            exercise.Shortcut = "CICF";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/incline-cable-flye";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("74C34D5D-D2F7-424D-9E8D-2E677B08B6A1"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "Bench Press - With Bands";
            exercise.Shortcut = "CBPB";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/bench-press-with-bands";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("F9FF7DA5-7F15-47F8-88F0-37A2E361E4D7"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "Cable Crossover";
            exercise.Shortcut = "CC";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/cable-crossover";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);
            exercise = new Exercise(new Guid("02EEF7A0-22C3-4A5F-B2A5-F4861AF9AA2B"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "Decline Dumbbell Bench Press";
            exercise.Shortcut = "CDDP";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/decline-dumbbell-bench-press";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("9BA4D0C8-0DDB-4A1E-BEDA-AC9F871044E0"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "Dumbbell Flyes";
            exercise.Shortcut = "CDF";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/dumbbell-flyes";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);
            exercise = new Exercise(new Guid("8B1D32B9-6590-43E8-A93B-24BD71ECF565"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "Dips - Chest Version";
            exercise.Shortcut = "CDD";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/dips-chest-version";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("A350269C-A915-404C-A8D8-D2F2A93D13AE"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "Dumbbell Bench Press";
            exercise.Shortcut = "CDP";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/dumbbell-bench-press";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("A2AAEE3E-EF64-42C0-89AA-A605AAAC538F"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "Incline Dumbbell Flyes";
            exercise.Shortcut = "CIDP";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/incline-dumbbell-flyes";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
        }

        private void barki(List<Exercise> list)
        {
            Exercise exercise = new Exercise(new Guid("104BB7A1-B668-4461-91C2-C37102B92E0C"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Arnold Dumbbell Press";
            exercise.Shortcut = "SAD";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/arnold-dumbbell-press";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("BE9FECAB-7CDA-422C-8146-F72047D99CE2"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Barbell Shoulder Press";
            exercise.Shortcut = "SBD";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/barbell-shoulder-press";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("6C9C673A-7C68-42EF-A59C-E0BDA3751CF6"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Dumbbell Shoulder Press";
            exercise.Shortcut = "SDP";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/dumbbell-shoulder-press";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("CE902A64-8249-4BF0-8965-765E4AEF8896"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Front Dumbbell Raise";
            exercise.Shortcut = "SFDR";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/front-dumbbell-raise";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("A0A7673A-F3A2-4A64-88FB-892438575DB4"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Bent Over Dumbbell Rear Delt Raise With Head On Bench";
            exercise.Shortcut = "SDB";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/bent-over-dumbbell-rear-delt-raise-with-head-on-bench";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);
            exercise = new Exercise(new Guid("492E8097-9826-40BE-8808-EDDFF8FE91FE"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Lateral Raise - With Bands";
            exercise.Shortcut = "SLRB";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/lateral-raise-with-bands";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("C3BEF822-5146-4F08-A434-3E982ECB95A7"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Machine Shoulder (Military) Press ";
            exercise.Shortcut = "SMMP";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/machine-shoulder-military-press";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
        }
    }
}