using System;
using System.Collections.Generic;


namespace BodyArchitect.Model.Old
{
    public class ExercisesBuilderPL : IExerciseBuilder
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
            czworoboczny(list);
            cardio(list);
            return list;
        }

        private void cardio(List<Exercise> list)
        {
            Exercise exercise = new Exercise(new Guid("2611C00B-1AFF-4869-8254-7C86172A6BAF"));
            exercise.ExerciseType = ExerciseType.Cardio;
            exercise.Name = "Rower";
            exercise.Shortcut = "CB";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/bicycling";
            exercise.Difficult = ExerciseDifficult.One;
            list.Add(exercise);

            exercise = new Exercise(new Guid("652C3FA6-A0CC-40BC-BBC8-EEDB8A422CBA"));
            exercise.ExerciseType = ExerciseType.Cardio;
            exercise.Name = "Rower stacjonarny";
            exercise.Shortcut = "CBS";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/bicycling-stationary";
            exercise.Difficult = ExerciseDifficult.One;
            list.Add(exercise);

            exercise = new Exercise(new Guid("C09BD20F-39B1-4B42-AF3A-0DCDEBD8F8A6"));
            exercise.ExerciseType = ExerciseType.Cardio;
            exercise.Name = "Orbitrek";
            exercise.Shortcut = "CO";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/elliptical-trainer";
            exercise.Difficult = ExerciseDifficult.Two;
            list.Add(exercise);

            exercise = new Exercise(new Guid("068F4A66-41A0-49DF-8B68-08F41CCC99B3"));
            exercise.ExerciseType = ExerciseType.Cardio;
            exercise.Name = "Bieg na bieżni stacjonarnej";
            exercise.Shortcut = "CRS";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/jogging-treadmill";
            exercise.Difficult = ExerciseDifficult.One;
            list.Add(exercise);

            exercise = new Exercise(new Guid("A816AC6E-3E20-4AB5-BFD0-B7632C8CEEC4"));
            exercise.ExerciseType = ExerciseType.Cardio;
            exercise.Name = "Rower siedzący";
            exercise.Shortcut = "CRE";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/recumbent-bike";
            exercise.Difficult = ExerciseDifficult.One;
            list.Add(exercise);

            exercise = new Exercise(new Guid("A5400CCA-6466-43A3-BED4-3C6CD4832ECB"));
            exercise.ExerciseType = ExerciseType.Cardio;
            exercise.Name = "Wiosłowanie";
            exercise.Shortcut = "CW";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/rowing-stationary";
            exercise.Difficult = ExerciseDifficult.Two;
            list.Add(exercise);


            exercise = new Exercise(new Guid("636EF56B-3989-413C-8E10-F987E58A9CA7"));
            exercise.ExerciseType = ExerciseType.Cardio;
            exercise.Name = "Jazda na rolkach";
            exercise.Shortcut = "CS";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/skating";
            exercise.Difficult = ExerciseDifficult.Two;
            list.Add(exercise);

            exercise = new Exercise(new Guid("9D83FE30-4637-49E7-91E4-A4429F05F340"));
            exercise.ExerciseType = ExerciseType.Cardio;
            exercise.Name = "Stepper";
            exercise.Shortcut = "CSP";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/stairmaster";
            exercise.Difficult = ExerciseDifficult.Two;
            list.Add(exercise);

            exercise = new Exercise(new Guid("F954C821-0012-4E58-A886-528E47C21DE2"));
            exercise.ExerciseType = ExerciseType.Cardio;
            exercise.Name = "Interwały";
            exercise.Shortcut = "CI";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/trail-runningwalking";
            exercise.Difficult = ExerciseDifficult.One;
            list.Add(exercise);
        }

        private void triceps(List<Exercise> list)
        {
            Exercise exercise = new Exercise(new Guid("01C796E2-0119-4F95-9A9A-79F2890DF658"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "PROSTOWANIE RAMION NA WYCIĄGU STOJĄC";
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.Shortcut = "TW";
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#t1";
            exercise.Difficult = ExerciseDifficult.One;
            list.Add(exercise);

            exercise = new Exercise(new Guid("26162403-4D0B-4DEB-8A7E-0F39F6E67F3A"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "PROSTOWANIE RAMION NA WYCIĄGU ZE SZNUREM";
            exercise.Shortcut = "TWS";
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#t1";
            exercise.Difficult = ExerciseDifficult.One;
            list.Add(exercise);

            exercise = new Exercise(new Guid("000EE742-BB0A-422D-BF57-7829A7A1B237"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "PROSTOWANIE RAMION NA WYCIĄGU Z UCHYTEM V";
            exercise.Shortcut = "TWV";
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#t1";
            exercise.Difficult = ExerciseDifficult.One;
            list.Add(exercise);

            exercise = new Exercise(new Guid("7944E92A-7068-4417-B2F4-C408141EDEB2"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "WYCISKANIE „FRANCUSKIE” SZTANGI W SIADZIE";
            exercise.Shortcut = "TSFS";
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#t2";
            exercise.Difficult = ExerciseDifficult.Two;
            list.Add(exercise);

            exercise = new Exercise(new Guid("687E33EF-11D4-4221-A15E-B088F64DB3A0"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "WYCISKANIE „FRANCUSKIE” SZTANGI W LEŻENIU PŁASKO";
            exercise.Shortcut = "TSL";
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.Difficult = ExerciseDifficult.One;
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#t4";
            list.Add(exercise);

            exercise = new Exercise(new Guid("8082FF60-2D28-40D6-95B9-858BA0FD4274"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "WYCISKANIE „FRANCUSKIE” SZTANGI W LEŻENIU NA ŁAWCE SKOŚNEJ GŁOWĄ W DÓŁ";
            exercise.Shortcut = "TSLSD";
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.Difficult = ExerciseDifficult.One;
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/decline-ez-bar-triceps-extension";
            list.Add(exercise);

            exercise = new Exercise(new Guid("80ABC7EE-230D-4E90-9244-4E60D1469161"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "WYCISKANIE „FRANCUSKIE” SZTANGI W LEŻENIU NA ŁAWCE SKOŚNEJ GŁOWĄ W GÓRĘ";
            exercise.Shortcut = "TSLSG";
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.Difficult = ExerciseDifficult.One;
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/incline-barbell-triceps-extension";
            list.Add(exercise);

            exercise = new Exercise(new Guid("3FBF5036-CD9F-401E-A4D1-E03B76A27C35"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "WYCISKANIE “FRANCUSKIE” JEDNORĄCZ HANTELKI";
            exercise.Shortcut = "TH";
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#t3";
            exercise.Difficult = ExerciseDifficult.Two;
            list.Add(exercise);
            
            exercise = new Exercise(new Guid("E584C030-DF25-47E7-A762-43F58F87DB96"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "WYCISKANIE „FRANCUSKIE” HANTELKI W LEŻENIU";
            exercise.Shortcut = "THFL";
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#t5";
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.Difficult = ExerciseDifficult.One;
            list.Add(exercise);

            exercise = new Exercise(new Guid("91850B2E-27AC-4FF0-85FE-57549A4F4585"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "PROSTOWNIE RAMIENIA Z HANTLĄ W OPADZIE TUŁOWIA";
            exercise.Shortcut = "THL";
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#t6";
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.Difficult = ExerciseDifficult.One;
            list.Add(exercise);

            exercise = new Exercise(new Guid("29DD77D9-2E91-456D-BB5C-83A743FB80ED"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "PROSTOWANIE RAMION NA WYCIĄGU W PŁASZCZYŹNE POZIOMEJ STOJĄC";
            exercise.Shortcut = "TWPS";
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#t7";
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.Difficult = ExerciseDifficult.Two;
            list.Add(exercise);

            exercise = new Exercise(new Guid("D6562C2A-BD74-46BE-8398-57B56BD698D5"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "PROSTOWANIE RAMION NA WYCIĄGU W PŁASZCZYŹNIE POZIOMEJ W PODPORZE";
            exercise.Shortcut = "TWPP";
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#t8";
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.Difficult = ExerciseDifficult.One;
            list.Add(exercise);

            exercise = new Exercise(new Guid("E833BAB7-DBCC-48F2-B92F-BF1A78E07DE0"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "POMPKI NA PORĘCZACH";
            exercise.Shortcut = "TPP";
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#t9";
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.Difficult = ExerciseDifficult.One;
            list.Add(exercise);

            exercise = new Exercise(new Guid("C5276D0F-5BAD-479A-B582-0855032A401A"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "POMPKI W PODPORZE TYŁEM";
            exercise.Shortcut = "TPT";
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#t10";
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.Difficult = ExerciseDifficult.One;
            list.Add(exercise);

            exercise = new Exercise(new Guid("60F86528-650C-47BA-AD9E-6FA08E44DDD5"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "PROSTOWANIE RAMIENIA PODCHWYTEM NA WYCIĄGU STOJĄC";
            exercise.Shortcut = "TWJ";
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#t11";
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.Difficult = ExerciseDifficult.One;
            list.Add(exercise);

            exercise = new Exercise(new Guid("9934E9E3-50DC-4B34-8B67-092084894740"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "WYCISKANIE W LEŻENIU NA ŁAWCE POZIOMEJ WĄSKIM UCHWYTEM";
            exercise.Shortcut = "TSP";
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#t12";
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.Difficult = ExerciseDifficult.Two;
            list.Add(exercise);

            exercise = new Exercise(new Guid("2D92DDA4-9152-4BC6-ABBB-C179DD7580C9"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "WYCISKANIE W LEŻENIU NA ŁAWCE POZIOMEJ WĄSKIM ODWROTNYM UCHWYTEM";
            exercise.Shortcut = "TSPR";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/reverse-triceps-bench-press";
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.Difficult = ExerciseDifficult.Three;
            list.Add(exercise);

            exercise = new Exercise(new Guid("63F604B6-DA97-4335-8AA7-24594B13684D"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "Dip Machine";
            exercise.Shortcut = "TMD";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/dip-machine";
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.Difficult = ExerciseDifficult.One;
            list.Add(exercise);

            exercise = new Exercise(new Guid("56AD4824-1BB2-4818-93CC-B90CC1C92DAA"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "Prostowanie ramienia w górę na wyciągu ze sznurem";
            exercise.Shortcut = "TWTS";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/cable-rope-overhead-triceps-extension";
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.Difficult = ExerciseDifficult.One;
            list.Add(exercise);

            exercise = new Exercise(new Guid("C19970C9-B2E2-4A01-9534-DDE77DD6B594"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "PROSTOWANIE RAMION PODCHWYTEM NA WYCIĄGU STOJĄC";
            exercise.Shortcut = "TWP";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/cable-rope-overhead-triceps-extension";
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.Difficult = ExerciseDifficult.One;
            list.Add(exercise);

            exercise = new Exercise(new Guid("78987E30-12D8-439E-9F04-3C319570BC3E"));
            exercise.ExerciseType = ExerciseType.Triceps;
            exercise.Name = "Wyciskanie 'francuskie' hantelki oburącz";
            exercise.Shortcut = "T2H";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/seated-triceps-press";
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.Difficult = ExerciseDifficult.One;
            list.Add(exercise);
        }

        private void przedramie(List<Exercise> list)
        {
            Exercise exercise = new Exercise(new Guid("2B9F0D8B-ADAA-4113-A465-A288908DAF3D"));
            exercise.ExerciseType = ExerciseType.Przedramie;
            exercise.Name = "UGINANIE RAMION ZE SZTANGA NACHWYTEM STOJĄC";
            exercise.Shortcut = "FSG";
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#9";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);

            exercise = new Exercise(new Guid("BEC17263-B690-428D-B07D-32FAD8F0A798"));
            exercise.ExerciseType = ExerciseType.Przedramie;
            exercise.Name = "UGINANIE RAMION Z HANTLAMI NACHWYTEM STOJĄC";
            exercise.Shortcut = "FHG";
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#9";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);

            exercise = new Exercise(new Guid("8C1D7A3B-C286-4289-AE66-407267F5678E"));
            exercise.ExerciseType = ExerciseType.Przedramie;
            exercise.Name = "UGINANIE RAMION ZE SZTANGA NACHWYTEM NA „MODLITEWNIKU”";
            exercise.Shortcut = "FSGM";
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#10";
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.Difficult = ExerciseDifficult.One;
            list.Add(exercise);

            exercise = new Exercise(new Guid("746C7749-3B0B-4293-996E-BDEAA7F25FBC"));
            exercise.ExerciseType = ExerciseType.Przedramie;
            exercise.Name = "UGINANIE NADGARSTKÓW ZE SZTANGĄ PODCHWYTEM W SIADZIE";
            exercise.Shortcut = "FSD";
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.Difficult = ExerciseDifficult.One;
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#11";
            list.Add(exercise);

            exercise = new Exercise(new Guid("E04572F7-16FF-4A5A-9CD6-5F1538BA2749"));
            exercise.ExerciseType = ExerciseType.Przedramie;
            exercise.Name = "UGINANIE NADGARSTKÓW ZE SZTANGĄ NACHWYTEM W SIADZIE";
            exercise.Shortcut = "FSNG";
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.Difficult = ExerciseDifficult.One;
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#12";
            list.Add(exercise);

            exercise = new Exercise(new Guid("3E341B4F-2007-448A-8A01-A8142571D7A7"));
            exercise.ExerciseType = ExerciseType.Przedramie;
            exercise.Name = "UGINANIE NADGARSTKÓW Z HANTLAMI PODCHWYTEM W SIADZIE";
            exercise.Shortcut = "FHD";
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#11";
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.Difficult = ExerciseDifficult.One;
            list.Add(exercise);

            exercise = new Exercise(new Guid("D18BEB76-EBD8-4671-A4A0-A5929AD586E4"));
            exercise.ExerciseType = ExerciseType.Przedramie;
            exercise.Name = "UGINANIE NADGARSTKÓW Z HANTLAMI NACHWYTEM W SIADZIE";
            exercise.Shortcut = "FHND";
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#12";
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.Difficult = ExerciseDifficult.One;
            list.Add(exercise);

            exercise = new Exercise(new Guid("51688E8D-3B75-4A9B-B63A-C8B402E4EEB6"));
            exercise.ExerciseType = ExerciseType.Przedramie;
            exercise.Name = "Plate Pinch";
            exercise.Shortcut = "FPP";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/plate-pinch";
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Static;
            exercise.Difficult = ExerciseDifficult.Two;
            list.Add(exercise);

            exercise = new Exercise(new Guid("9C4BD158-CEC3-475E-8542-8B7E9E413AAC"));
            exercise.ExerciseType = ExerciseType.Przedramie;
            exercise.Name = "UGINANIE NADGARSTKÓW NACHWYTEM W SIADZIE NA WYCIĄGU";
            exercise.Shortcut = "FPP";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/cable-wrist-curl";
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.Difficult = ExerciseDifficult.One;
            list.Add(exercise);

            exercise = new Exercise(new Guid("3CBB5600-2B3E-4610-AF0A-3065315EE0AD"));
            exercise.ExerciseType = ExerciseType.Przedramie;
            exercise.Name = "UGINANIE NADGARSTKÓW ZE SZTANGĄ Z TYŁU NACHWYTEM STOJĄC";
            exercise.Shortcut = "FSDT";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/standing-palms-up-barbell-behind-the-back-wrist-curl";
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.Difficult = ExerciseDifficult.One;
            list.Add(exercise);

            exercise = new Exercise(new Guid("1C7DF47E-92A3-43F2-B121-4DB038145BE4"));
            exercise.ExerciseType = ExerciseType.Przedramie;
            exercise.Name = "UGINANIE NADGARSTKA Z HANTELKĄ PODCHWYTEM";
            exercise.Shortcut = "FHJD";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/seated-one-arm-dumbbell-palms-up-wrist-curl";
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.Difficult = ExerciseDifficult.One;
            list.Add(exercise);

            exercise = new Exercise(new Guid("9B868604-C770-4B13-A8E0-32C4E0BEA5B8"));
            exercise.ExerciseType = ExerciseType.Przedramie;
            exercise.Name = "UGINANIE NADGARSTKA Z HANTELKĄ NACHWYTEM";
            exercise.Shortcut = "FHJND";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/seated-one-arm-dumbbell-palms-down-wrist-curl";
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.Difficult = ExerciseDifficult.One;
            list.Add(exercise);
        }

        private void biceps(List<Exercise> list)
        {
            Exercise exercise = new Exercise(new Guid("B72695CA-77B5-46C8-8EB2-E44CD7BED7A5"));
            exercise.ExerciseType = ExerciseType.Biceps;
            exercise.Name = "UGINANIE RAMION ZE SZTANGĄ STOJAC PODCHWYTEM";
            exercise.Shortcut = "BS";
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#1";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);

            exercise = new Exercise(new Guid("602E8A6D-4604-46BF-9620-E5FCCDF8CB07"));
            exercise.ExerciseType = ExerciseType.Biceps;
            exercise.Name = "UGINANIE RAMION Z HANTLAMI STOJĄC PODCHWYTEM(Z „SUPINACJĄ” NADGARSTKA)";
            exercise.Shortcut = "BHS";
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#2";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);

            exercise = new Exercise(new Guid("F645E750-5E27-4A5A-ABB4-D923903B9B3C"));
            exercise.ExerciseType = ExerciseType.Biceps;
            exercise.Name = "UGINANIE RAMION Z HANTLAMI STOJĄC (UCHWYT „MŁOTKOWY”)";
            exercise.Shortcut = "BHM";
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#3";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);

            exercise = new Exercise(new Guid("3EA96687-3643-49B4-A4EB-0AD66ABEC5D6"));
            exercise.ExerciseType = ExerciseType.Biceps;
            exercise.Name = "UGINANIE RAMION ZE SZTANGĄ NA „MODLITEWNIKU”";
            exercise.Shortcut = "BMS";
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#4";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);

            exercise = new Exercise(new Guid("3437D6CC-79A4-40E0-88F7-22AA7D240FEE"));
            exercise.ExerciseType = ExerciseType.Biceps;
            exercise.Name = "UGINANIE RAMIENIA Z HANTLĄ NA „MODLITEWNIKU”";
            exercise.Shortcut = "BMH";
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#5";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);

            exercise = new Exercise(new Guid("FCFCB4CC-3F95-4F23-ACBA-E3953CFA41E1"));
            exercise.ExerciseType = ExerciseType.Biceps;
            exercise.Name = "UGINANIE RAMION Z HANTLAMI W SIADZIE NA ŁAWCE SKOŚNEJ(Z SUPINACJĄ NADGARSTKA)";
            exercise.Shortcut = "BHSS";
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#6";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);

            exercise = new Exercise(new Guid("78AD19ED-D527-4B07-9366-7847DADC8E21"));
            exercise.ExerciseType = ExerciseType.Biceps;
            exercise.Name = "UGINANIE RAMION Z HANTLAMI W SIADZIE NA ŁAWCE SKOŚNEJ(CHWYT MŁOTKOWY)";
            exercise.Shortcut = "BHSM";
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#6";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);

            exercise = new Exercise(new Guid("A3F08B5B-95A7-4089-81B0-189B874E32C7"));
            exercise.ExerciseType = ExerciseType.Biceps;
            exercise.Name = "UGINANIE RAMIENIA Z HANTLĄ W SIADZIE-W PODPORZE O KOLANO";
            exercise.Shortcut = "BRH";
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#7";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);

            exercise = new Exercise(new Guid("FE20077B-F504-4BDE-826F-5EAB44FFF5F8"));
            exercise.ExerciseType = ExerciseType.Biceps;
            exercise.Name = "UGINANIE RAMION PODCHWYTEM STOJĄC-Z RĄCZKĄ WYCIĄGU";
            exercise.Shortcut = "BW";
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#8";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);

            exercise = new Exercise(new Guid("543A0F4F-A72D-4CB8-9F59-47D7DF6F6043"));
            exercise.ExerciseType = ExerciseType.Biceps;
            exercise.Name = "UGINANIE RAMION NA WYCIĄGU (BRAMA)";
            exercise.Shortcut = "BB";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);

            exercise = new Exercise(new Guid("B0A84324-6C6F-42AD-9A80-35226678C1C2"));
            exercise.ExerciseType = ExerciseType.Biceps;
            exercise.Name = "UGINANIE RAMION ZE SZTANGĄ W LEŻENIU NA ŁAWCE SKOŚNEJ";
            exercise.Shortcut = "BSLS";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);

            exercise = new Exercise(new Guid("02A3B488-1823-4703-AEE2-AE0A87ADF43D"));
            exercise.ExerciseType = ExerciseType.Biceps;
            exercise.Name = "KRZYŻOWE UGINANIE RAMION Z HANTLAMI";
            exercise.Shortcut = "BHC";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/cross-body-hammer-curl";
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);

            exercise = new Exercise(new Guid("1350E110-E8C7-4920-994B-8EB3958A7E62"));
            exercise.ExerciseType = ExerciseType.Biceps;
            exercise.Name = "UGINANIE RAMION Z HANTLAMI W LEŻENIU NA ŁAWCE SKOŚNEJ";
            exercise.Shortcut = "BHLS";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/dumbbell-prone-incline-curl";
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);

            exercise = new Exercise(new Guid("336E0AAC-FE97-4284-AF0D-EAFF155D55A3"));
            exercise.ExerciseType = ExerciseType.Biceps;
            exercise.Name = "UGINANIE RAMION NA WYCIĄGU LEŻĄC";
            exercise.Shortcut = "BWL";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/lying-cable-curl";
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);

            exercise = new Exercise(new Guid("A5364635-495B-47A6-9118-629C3B3BF871"));
            exercise.ExerciseType = ExerciseType.Biceps;
            exercise.Name = "UGINANIE RAMION Z HANTLAMI STOJĄC PODCHWYTEM";
            exercise.Shortcut = "BH";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.Url = "http://www.kulturystyka.pl/atlas/ramiona.asp#2";
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);
        }

        private void brzuch(List<Exercise> list)
        {
            Exercise exercise = new Exercise(new Guid("071B59C5-F933-4D1C-8200-8F2B4FA3047A"));
            exercise.ExerciseType = ExerciseType.Brzuch;
            exercise.Name = "SKŁONY W LEŻENIU PŁASKO";
            exercise.Shortcut = "ASLP";
            exercise.Url = "http://www.kulturystyka.pl/atlas/brzuch.asp#1";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);

            exercise = new Exercise(new Guid("1961C8B6-A4C4-43EE-A44F-030498E49C9E"));
            exercise.ExerciseType = ExerciseType.Brzuch;
            exercise.Name = "SKŁONY W LEŻENIU GŁOWĄ W DÓŁ";
            exercise.Shortcut = "ASLD";
            exercise.Url = "http://www.kulturystyka.pl/atlas/brzuch.asp#2";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);

            exercise = new Exercise(new Guid("C3349E17-7CEF-4A2D-AD8C-04F1BE7B6579"));
            exercise.ExerciseType = ExerciseType.Brzuch;
            exercise.Name = "UNOSZENIE NÓG W LEŻENIU NA SKOŚNEJ ŁAWCE";
            exercise.Shortcut = "AUNS";
            exercise.Url = "http://www.kulturystyka.pl/atlas/brzuch.asp#3";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);

            exercise = new Exercise(new Guid("59192DCD-32EF-49AC-A144-C7EA000D9EA5"));
            exercise.ExerciseType = ExerciseType.Brzuch;
            exercise.Name = "UNOSZENIE NÓG W ZWISIE NA DRĄŻKU";
            exercise.Shortcut = "AUNW";
            exercise.Url = "http://www.kulturystyka.pl/atlas/brzuch.asp#4";
            exercise.Difficult = ExerciseDifficult.Three;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);

            exercise = new Exercise(new Guid("FE12B2FB-35B0-4566-A5D4-B50C8D5DFE4D"));
            exercise.ExerciseType = ExerciseType.Brzuch;
            exercise.Name = "UNOSZENIE NÓG W PODPORZE";
            exercise.Shortcut = "AUNP";
            exercise.Url = "http://www.kulturystyka.pl/atlas/brzuch.asp#5";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);

            exercise = new Exercise(new Guid("F176D5FF-4239-4D70-A1C7-93E1F1AB7B0E"));
            exercise.ExerciseType = ExerciseType.Brzuch;
            exercise.Name = "UNOSZENIE KOLAN W LEŻENIU PŁASKO";
            exercise.Shortcut = "AUKP";
            exercise.Url = "http://www.kulturystyka.pl/atlas/brzuch.asp#6";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);

            exercise = new Exercise(new Guid("03F3B090-32F2-4232-B967-A5AA0CC85427"));
            exercise.ExerciseType = ExerciseType.Brzuch;
            exercise.Name = "SKŁONY TUŁOWIA Z LINKĄ WYCIĄGU SIEDZĄC";
            exercise.Shortcut = "ASTWS";
            exercise.Url = "http://www.kulturystyka.pl/atlas/brzuch.asp#7";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);

            exercise = new Exercise(new Guid("8EA54B3A-40EA-446E-A5D7-CA89097A8B8E"));
            exercise.ExerciseType = ExerciseType.Brzuch;
            exercise.Name = "SKRĘTY TUŁOWIA";
            exercise.Shortcut = "AST";
            exercise.Url = "http://www.kulturystyka.pl/atlas/brzuch.asp#8";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);

            exercise = new Exercise(new Guid("CDB6840E-CD81-409E-B55C-92907C94E2CC"));
            exercise.ExerciseType = ExerciseType.Brzuch;
            exercise.Name = "SKŁONY TUŁOWIA Z LINKĄ WYCIĄGU KLĘCZĄC";
            exercise.Shortcut = "ASTWK";
            exercise.Url = "http://www.kulturystyka.pl/atlas/brzuch.asp#9";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);

            exercise = new Exercise(new Guid("D69F6E75-8927-4714-AC97-CC992F2C2C62"));
            exercise.ExerciseType = ExerciseType.Brzuch;
            exercise.Name = "SKŁONY BOCZNE";
            exercise.Shortcut = "ASB";
            exercise.Url = "http://www.kulturystyka.pl/atlas/brzuch.asp#10";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);

            exercise = new Exercise(new Guid("F0ED350C-F5B7-466A-B10A-AC73A6E2183F"));
            exercise.ExerciseType = ExerciseType.Brzuch;
            exercise.Name = "SKŁONY BOCZNE NA ŁAWCE";
            exercise.Shortcut = "ASBL";
            exercise.Url = "http://www.kulturystyka.pl/atlas/brzuch.asp#11";
            exercise.Difficult = ExerciseDifficult.NotSet;
            list.Add(exercise);

            exercise = new Exercise(new Guid("782FF1A2-16B6-4A39-A0D7-0B59606215D4"));
            exercise.ExerciseType = ExerciseType.Brzuch;
            exercise.Name = "SKRĘTY TUŁOWIA W LEŻENIU";
            exercise.Shortcut = "ASTL";
            exercise.Url = "http://www.kulturystyka.pl/atlas/brzuch.asp#12";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);

            exercise = new Exercise(new Guid("270BD499-7BE7-49D8-A57B-A9BB70AC8BF8"));
            exercise.ExerciseType = ExerciseType.Brzuch;
            exercise.Name = "Aerobiczna 6 Weidera";
            exercise.Shortcut = "A6W";
            exercise.Url = "http://a6weidera.pl/";
            exercise.Difficult = ExerciseDifficult.Three;
            list.Add(exercise);

            exercise = new Exercise(new Guid("B72C04B8-1877-4E34-872A-F5DFD0E85150"));
            exercise.ExerciseType = ExerciseType.Brzuch;
            exercise.Name = "ABS";
            exercise.Shortcut = "ABS";
            exercise.Difficult = ExerciseDifficult.Two;
            list.Add(exercise);

            exercise = new Exercise(new Guid("AD5E2E70-8995-4BC3-9E4B-572EA66E5F5C"));
            exercise.ExerciseType = ExerciseType.Brzuch;
            exercise.Name = "ABS 2";
            exercise.Shortcut = "ABS2";
            exercise.Url = "http://www.abs2.pl/";
            exercise.Difficult = ExerciseDifficult.Two;
            list.Add(exercise);

            exercise = new Exercise(new Guid("2C2E6622-D5EF-4B22-9E86-9B01576BBC11"));
            exercise.ExerciseType = ExerciseType.Brzuch;
            exercise.Name = "8 minutes ABS";
            exercise.Shortcut = "8ABS";
            exercise.Url = "http://www.youtube.com/watch?v=iWxM995gDDA";
            exercise.Difficult = ExerciseDifficult.NotSet;
            list.Add(exercise);

            exercise = new Exercise(new Guid("5FFF178B-CA17-4CD1-9C93-8B36E7AD073F"));
            exercise.ExerciseType = ExerciseType.Brzuch;
            exercise.Name = "ROWEREK";
            exercise.Shortcut = "AR";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/air-bike";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);

            exercise = new Exercise(new Guid("8788C4FB-F5FF-4880-A57A-9D2F4C211DF8"));
            exercise.ExerciseType = ExerciseType.Brzuch;
            exercise.Name = "BRZUSZKI KRZYŻOWE";
            exercise.Shortcut = "ABK";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/air-bike";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);

            exercise = new Exercise(new Guid("8A670960-D7CE-4F8F-8A89-7CA23D84D4BD"));
            exercise.ExerciseType = ExerciseType.Brzuch;
            exercise.Name = "BRZUSZKI Z NOGAMI NA PIŁCE";
            exercise.Shortcut = "ABNP";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/crunch-legs-on-exercise-ball";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);

            exercise = new Exercise(new Guid("154BE2D8-9205-4AFD-B9E4-79EF8C5D9540"));
            exercise.ExerciseType = ExerciseType.Brzuch;
            exercise.Name = "BRZUSZKI NA PIŁCE";
            exercise.Shortcut = "ABP";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/exercise-ball-crunch";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);

            exercise = new Exercise(new Guid("C9D3163C-A7BE-4208-8950-E128C9524752"));
            exercise.ExerciseType = ExerciseType.Brzuch;
            exercise.Name = "SCYZORYK";
            exercise.Shortcut = "AS";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/jackknife-sit-up";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);

            exercise = new Exercise(new Guid("F71733FA-B1BF-414A-BC03-F532C1D50AE1"));
            exercise.ExerciseType = ExerciseType.Brzuch;
            exercise.Name = "SKŁONY BOCZNE LEŻĄC";
            exercise.Shortcut = "ASBL";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/oblique-crunches-on-the-floor";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);

            exercise = new Exercise(new Guid("8C69791D-E6D1-4552-9412-C09D3781B217"));
            exercise.ExerciseType = ExerciseType.Brzuch;
            exercise.Name = "Plate Twist";
            exercise.Shortcut = "APT";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/plate-twist";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);
        }

        private void lydki(List<Exercise> list)
        {
            Exercise exercise = new Exercise(new Guid("82DB0EFE-ABFB-4350-8A7D-EA69B4AC7AAE"));
            exercise.ExerciseType = ExerciseType.Lydki;
            exercise.Name = "WSPIECIA NA PALCE W STANIU ZE SZTANGĄ";
            exercise.Shortcut = "LWSS";
            exercise.Url = "http://www.kulturystyka.pl/atlas/nogi.asp#l1";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);

            exercise = new Exercise(new Guid("4A865184-98E4-4A60-BA8E-9DCE8BA4C79F"));
            exercise.ExerciseType = ExerciseType.Lydki;
            exercise.Name = "WSPIECIA NA PALCE W STANIU NA MASZYNIE";
            exercise.Shortcut = "LWSM";
            exercise.Url = "http://www.kulturystyka.pl/atlas/nogi.asp#l1";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);

            exercise = new Exercise(new Guid("D7B32CE1-13D8-4C06-A1C6-779D45AEFA96"));
            exercise.ExerciseType = ExerciseType.Lydki;
            exercise.Name = "WSPIĘCIA NA PALCE W SIADZIE NA MASZYNIE";
            exercise.Shortcut = "LWM";
            exercise.Url = "http://www.kulturystyka.pl/atlas/nogi.asp#l2";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);

            exercise = new Exercise(new Guid("097476A6-1F24-41F0-BD93-2173758D0A1F"));
            exercise.ExerciseType = ExerciseType.Lydki;
            exercise.Name = "WSPIĘCIA NA PALCE W SIADZIE ZE SZTANGĄ";
            exercise.Shortcut = "LWS";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/barbell-seated-calf-raise";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);

            exercise = new Exercise(new Guid("D192DDA3-A8F8-4DC6-B216-81080584DA11"));
            exercise.ExerciseType = ExerciseType.Lydki;
            exercise.Name = "WSPIĘCIA NA PALCE W SIADZIE Z HANTLĄ";
            exercise.Shortcut = "LWH";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/dumbbell-seated-one-leg-calf-raise";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);

            exercise = new Exercise(new Guid("D4664345-DB3F-4310-8F97-FCF8CAC4B2D9"));
            exercise.ExerciseType = ExerciseType.Lydki;
            exercise.Name = "OŚLE WSPIĘCIA";
            exercise.Shortcut = "LOW";
            exercise.Url = "http://www.kulturystyka.pl/atlas/nogi.asp#l3";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);

            exercise = new Exercise(new Guid("ACCF32CC-52A6-48D5-95F7-340ACBD1C4B4"));
            exercise.ExerciseType = ExerciseType.Lydki;
            exercise.Name = "WSPIĘCIA NA PALCE NA HACK-MASZYNIE";
            exercise.Shortcut = "LHM";
            exercise.Url = "http://www.kulturystyka.pl/atlas/nogi.asp#l4";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);

            exercise = new Exercise(new Guid("8186BA53-5B43-42CC-930A-D3AFB866D2AE"));
            exercise.ExerciseType = ExerciseType.Lydki;
            exercise.Name = "WYPYCHANIE CIĘŻARU NA MASZYNIE/SUWNICY PALCAMI NÓG";
            exercise.Shortcut = "LM";
            exercise.Url = "http://www.kulturystyka.pl/atlas/nogi.asp#l5";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);

            exercise = new Exercise(new Guid("593FE198-26DF-455B-985F-1526139D6A17"));
            exercise.ExerciseType = ExerciseType.Lydki;
            exercise.Name = "ODWROTNE WSPIĘCIA W STANIU";
            exercise.Shortcut = "LOS";
            exercise.Url = "http://www.kulturystyka.pl/atlas/nogi.asp#l6";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);

            exercise = new Exercise(new Guid("C8BA73AF-9BFD-4ABB-8141-24802E0886E8"));
            exercise.ExerciseType = ExerciseType.Lydki;
            exercise.Name = "WSPIĘCIA NA PALCE W STANIU Z HANTLAMI";
            exercise.Shortcut = "LWSH";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/standing-dumbbell-calf-raise";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
        }

        private void nogi(List<Exercise> list)
        {
            Exercise exercise = new Exercise(new Guid("3E06A130-B811-4E45-9285-F087403615BF"));
            exercise.ExerciseType = ExerciseType.Nogi;
            exercise.Name = "PRZYSIADY ZE SZTANGĄ NA BARKACH";
            exercise.Shortcut = "NP";
            exercise.Url = "http://www.kulturystyka.pl/atlas/nogi.asp#1";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("0B11D9C5-772C-4C9F-96F5-79DA28E77149"));
            exercise.ExerciseType = ExerciseType.Nogi;
            exercise.Name = "PRZYSIADY ZE SZTANGĄ TRZYMANĄ Z PRZODU";
            exercise.Shortcut = "NPP";
            exercise.Url = "http://www.kulturystyka.pl/atlas/nogi.asp#2";
            exercise.Difficult = ExerciseDifficult.Three;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("F08599A2-2F05-46D8-A61E-7FDF7D888C24"));
            exercise.ExerciseType = ExerciseType.Nogi;
            exercise.Name = "HACK-PRZYSIADY";
            exercise.Shortcut = "NHP";
            exercise.Url = "http://www.kulturystyka.pl/atlas/nogi.asp#3";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("81AA48EE-D5BC-498B-9B2B-7E1C7D9DC0BD"));
            exercise.ExerciseType = ExerciseType.Nogi;
            exercise.Name = "PRZYSIADY NA SUWNICY SKOŚNEJ";
            exercise.Shortcut = "NPS";
            exercise.Url = "http://www.kulturystyka.pl/atlas/nogi.asp#4";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("237A013E-42E6-4879-869D-36433E15F0CD"));
            exercise.ExerciseType = ExerciseType.Nogi;
            exercise.Name = "SYZYFKI";
            exercise.Shortcut = "NS";
            exercise.Url = "http://www.kulturystyka.pl/atlas/nogi.asp#5";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("74DE54F2-CEDC-41D1-8B50-5B8209420DDB"));
            exercise.ExerciseType = ExerciseType.Nogi;
            exercise.Name = "PROSTOWNIE NÓG W SIADZIE";
            exercise.Shortcut = "NLE";
            exercise.Url = "http://www.kulturystyka.pl/atlas/nogi.asp#6";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("C0E248CC-FF85-444F-96C7-3A966F663C9F"));
            exercise.ExerciseType = ExerciseType.Nogi;
            exercise.Name = "WYPYCHANIE CIĘŻARU NA SUWNICY(MASZYNIE)";
            exercise.Shortcut = "NSN";
            exercise.Url = "http://www.kulturystyka.pl/atlas/nogi.asp#7";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("042346B0-4E44-403D-9648-A5B9441BA299"));
            exercise.ExerciseType = ExerciseType.Nogi;
            exercise.Name = "UGINANIE NÓG W LEŻENIU";
            exercise.Shortcut = "NLCL";
            exercise.Url = "http://www.kulturystyka.pl/atlas/nogi.asp#8";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);
            exercise = new Exercise(new Guid("C0613588-CC24-4EC7-BD9B-C906643817EB"));
            exercise.ExerciseType = ExerciseType.Nogi;
            exercise.Name = "PRZYSIADY WYKROCZNE (WYKROKI)";
            exercise.Shortcut = "NPW";
            exercise.Url = "http://www.kulturystyka.pl/atlas/nogi.asp#9";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.NotSet;
            list.Add(exercise);
            exercise = new Exercise(new Guid("903785E5-B116-41A2-B380-DB9E57695E59"));
            exercise.ExerciseType = ExerciseType.Nogi;
            exercise.Name = "NOŻYCE";
            exercise.Shortcut = "NN";
            exercise.Url = "http://www.kulturystyka.pl/atlas/nogi.asp#10";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.NotSet;
            list.Add(exercise);
            exercise = new Exercise(new Guid("72768460-6E09-4E60-9671-6D977BCA90F4"));
            exercise.ExerciseType = ExerciseType.Nogi;
            exercise.Name = "WYSOKI STEP ZE SZTANGĄ";
            exercise.Shortcut = "NWSS";
            exercise.Url = "http://www.kulturystyka.pl/atlas/nogi.asp#11";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.NotSet;
            list.Add(exercise);
            exercise = new Exercise(new Guid("C67FE645-D00F-45E4-A624-1951B49F5B4A"));
            exercise.ExerciseType = ExerciseType.Nogi;
            exercise.Name = "WYSOKI STEP Z HANTLAMI";
            exercise.Shortcut = "NWSH";
            exercise.Url = "http://www.kulturystyka.pl/atlas/nogi.asp#11";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.NotSet;
            list.Add(exercise);
            exercise = new Exercise(new Guid("3662F588-465E-4EF3-8987-18ADB3A3B71D"));
            exercise.ExerciseType = ExerciseType.Nogi;
            exercise.Name = "ODWODZENIE NOGI W TYŁ";
            exercise.Shortcut = "NOT";
            exercise.Url = "http://www.kulturystyka.pl/atlas/nogi.asp#12";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);
            exercise = new Exercise(new Guid("AA565EFD-8146-4B39-96A4-4F08B411D677"));
            exercise.ExerciseType = ExerciseType.Nogi;
            exercise.Name = "ŚCIĄGANIE KOLAN W SIADZIE";
            exercise.Shortcut = "NSKS";
            exercise.Url = "http://www.kulturystyka.pl/atlas/nogi.asp#13";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("9935CAF6-64FA-4787-97A7-528229D1C0FA"));
            exercise.ExerciseType = ExerciseType.Nogi;
            exercise.Name = "PRZYWODZENIE NÓG DO WEWNĄTRZ";
            exercise.Shortcut = "NPDW";
            exercise.Url = "http://www.kulturystyka.pl/atlas/nogi.asp#14";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);
            exercise = new Exercise(new Guid("10B3983C-EDDD-45B2-B4F3-964404487F1A"));
            exercise.ExerciseType = ExerciseType.Nogi;
            exercise.Name = "ODWODZENIE NÓG NA ZEWNĄTRZ";
            exercise.Shortcut = "NOZ";
            exercise.Url = "http://www.kulturystyka.pl/atlas/nogi.asp#15";
            exercise.Difficult = ExerciseDifficult.NotSet;
            list.Add(exercise);
            exercise = new Exercise(new Guid("BF47A7D9-5DAD-427B-9A9F-FB5D89C47D1E"));
            exercise.ExerciseType = ExerciseType.Nogi;
            exercise.Name = "UGINANIE NÓG W SIADZIE (NLC)";
            exercise.Shortcut = "NLC";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            list.Add(exercise);
            exercise = new Exercise(new Guid("6D2AD301-23B8-4C6C-97BB-D965CD909DB8"));
            exercise.ExerciseType = ExerciseType.Nogi;
            exercise.Name = "WYKROKI BOCZNE ZE SZTANGĄ";
            exercise.Shortcut = "NPWB";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/barbell-side-split-squat";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);

            exercise = new Exercise(new Guid("6C6643ED-5CFE-401C-A626-B903261F539E"));
            exercise.ExerciseType = ExerciseType.Nogi;
            exercise.Name = "PRZYSIADY ZE SZTANGĄ DO SIADU";
            exercise.Shortcut = "NPD";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/barbell-side-split-squat";
            exercise.Difficult = ExerciseDifficult.Three;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);

            exercise = new Exercise(new Guid("DFE36417-1BC5-4F6D-AC32-3A9A8DF520B2"));
            exercise.ExerciseType = ExerciseType.Nogi;
            exercise.Name = "PRZYSIADY Z HANTLAMI";
            exercise.Shortcut = "NPH";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/dumbbell-squat";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
            exercise = new Exercise(new Guid("8F0F1A45-5776-47CB-9188-F6B5DE153277"));
            exercise.ExerciseType = ExerciseType.Nogi;
            exercise.Name = "PRZYSIADY ROZKROCZNE Z HANTLĄ";
            exercise.Shortcut = "NPRH";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/plie-dumbbell-squat";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            list.Add(exercise);
        }
        
        private void czworoboczny(List<Exercise> list)
        {
            Exercise exercise = new Exercise(new Guid("0685238E-116A-477C-8881-F39D6F25C686"));
            exercise.ExerciseType = ExerciseType.Czworoboczny;
            exercise.Name = "SZRUGSY Z HANTLAMI";
            exercise.Shortcut = "PS";
            exercise.Url = "http://www.kulturystyka.pl/atlas/plecy.asp#18";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);

            exercise = new Exercise(new Guid("3C3E3783-E36A-4785-93F5-2DF10F3EEFD0"));
            exercise.ExerciseType = ExerciseType.Czworoboczny;
            exercise.Name = "SZRUGSY ZE SZTANGĄ";
            exercise.Shortcut = "PSS";
            exercise.Url = "http://www.kulturystyka.pl/atlas/plecy.asp#18";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);

            exercise = new Exercise(new Guid("3A6BD1AE-A281-4297-9E2F-2E4FCF0DAE56"));
            exercise.ExerciseType = ExerciseType.Czworoboczny;
            exercise.Name = "SZRUGSY ZE SZTANGĄ Z TYŁU";
            exercise.Shortcut = "PSST";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/barbell-shrug-behind-the-back";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);

            exercise = new Exercise(new Guid("378D874E-DA1A-4E12-9A0B-2DBB2DC1805A"));
            exercise.ExerciseType = ExerciseType.Czworoboczny;
            exercise.Name = "SZRUGSY NA WYCIĄGU";
            exercise.Shortcut = "PSW";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/barbell-shrug-behind-the-back";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);

        }

        private void plecy(List<Exercise> list)
        {
            Exercise exercise = new Exercise(new Guid("C32BAB2E-90F4-4293-817E-375BDB4F5830"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "PODCIĄGANIE NA DRĄŻKU SZEROKIM UCHWYTEM (NACHWYT)";
            exercise.Shortcut = "PPN";
            exercise.Url = "http://www.kulturystyka.pl/atlas/plecy.asp#1";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);
            exercise = new Exercise(new Guid("71FCB610-AFF9-4098-9F82-A1BEFAAA1645"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "PODCIĄGANIE NA DRĄŻKU W UCHWYCIE NEUTRALNYM";
            exercise.Shortcut = "PPUN";
            exercise.Url = "http://www.kulturystyka.pl/atlas/plecy.asp#2";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);
            exercise = new Exercise(new Guid("BA836E4B-13EA-4F71-AD15-E35778754750"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "PODCIĄGANIE NA DRĄŻKU PODCHWYTEM";
            exercise.Shortcut = "PPDP";
            exercise.Url = "http://www.kulturystyka.pl/atlas/plecy.asp#3";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);
            exercise = new Exercise(new Guid("19507F5A-528D-487C-BF1C-92C92F644DF3"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "PODCIĄGANIE SZTANGI W OPADZIE/NA ŁAWCE POZIOMEJ(WIOSŁOWANIE)";
            exercise.Shortcut = "PSD";
            exercise.Url = "http://www.kulturystyka.pl/atlas/plecy.asp#4";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);
            exercise = new Exercise(new Guid("8AD4AD3E-99B7-429B-975A-7157B539C64B"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "PODCIĄGANIE HANTELKI W OPADZIE(WIOSŁOWANIE)";
            exercise.Shortcut = "PHJ";
            exercise.Url = "http://www.kulturystyka.pl/atlas/plecy.asp#5";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);
            exercise = new Exercise(new Guid("DB0677D6-9E4A-4BD3-96B1-BBA05212ECD6"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "PODCIĄGANIE KOŃCA SZTANGI W OPADZIE";
            exercise.Shortcut = "PPKSO";
            exercise.Url = "http://www.kulturystyka.pl/atlas/plecy.asp#6";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);
            exercise = new Exercise(new Guid("DDC69F16-2BA4-4BB2-9F77-8879DCD21DF2"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "PODCIĄGANIE KOŃCA SZTANGI W OPADZIE JEDNĄ RĘKĄ";
            exercise.Shortcut = "PPJSO";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/bent-over-one-arm-long-bar-row";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);
            exercise = new Exercise(new Guid("728AED2B-E1EC-48FA-B917-9CC2A91CDB9C"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "PRZYCIĄGANIE LINKI WYCIĄGU DOLNEGO W SIADZIE PŁASKIM";
            exercise.Shortcut = "PDS";
            exercise.Url = "http://www.kulturystyka.pl/atlas/plecy.asp#7";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);
            exercise = new Exercise(new Guid("C14739B7-257F-4D0C-8945-BD09DB3A3B2F"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "PRZYCIĄGANIE LINKI WYCIĄGU GÓRNEGO W SIADZIE";
            exercise.Shortcut = "PUS";
            exercise.Url = "http://www.kulturystyka.pl/atlas/plecy.asp#8";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);
            exercise = new Exercise(new Guid("AF9624D9-1051-4FAA-88BE-063099412021"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "ŚCIĄGANIE DRĄŻKA WYCIĄGU GÓRNEGO W SIADZIE SZEROKIM UCHWYTEM (NACHWYT)";
            exercise.Shortcut = "PDSN";
            exercise.Url = "http://www.kulturystyka.pl/atlas/plecy.asp#9";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);
            exercise = new Exercise(new Guid("7592DDE4-FA1F-4C7F-A60C-B8C921BBBA63"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "ŚCIĄGANIE DRĄŻKA WYCIĄGU GÓRNEGO W SIADZIE SZEROKIM UCHWYTEM (NACHWYT) ZA SZYJĘ";
            exercise.Shortcut = "PDSNT";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/wide-grip-pulldown-behind-the-neck";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);
            exercise = new Exercise(new Guid("82716E4C-23C3-4940-A4A1-C89C4067B933"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "ŚCIĄGANIE DRĄŻKA WYCIĄGU GÓRNEGO W SIADZIE PODCHWYTEM";
            exercise.Shortcut = "PWSP";
            exercise.Url = "http://www.kulturystyka.pl/atlas/plecy.asp#10";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);
            exercise = new Exercise(new Guid("5084EECB-41FD-49D1-9396-1750743247B7"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "ŚCIĄGANIE DRĄŻKA WYCIĄGU GÓRNEGO W SIADZIE UCHWYT NEUTRALNY";
            exercise.Shortcut = "PWGSN";
            exercise.Url = "http://www.kulturystyka.pl/atlas/plecy.asp#11";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);
            exercise = new Exercise(new Guid("A8321E65-9A0E-420A-8591-04DED46EBC6B"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "PRZNOSZENIE SZTANGI W LEŻENIU NA ŁAWCE POZIOMEJ";
            exercise.Shortcut = "PSLP";
            exercise.Url = "http://www.kulturystyka.pl/atlas/plecy.asp#12";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);
            exercise = new Exercise(new Guid("71656EF5-9FF1-47EC-ACEF-F0BC57F817D7"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "PRZYCIĄGANIE WYCIĄGU GÓRNEGO NA ŁAWCE SKOŚNEJ";
            exercise.Shortcut = "PWSS";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/cable-incline-pushdown";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);
            exercise = new Exercise(new Guid("5484E989-7142-4E1F-8090-ED0E27233A86"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "PODCIĄGANIE SZTANGI (WIOSŁOWANIE) W LEŻENIU NA ŁAWECZCE POZIOMEJ";
            exercise.Shortcut = "PPSLP";
            exercise.Url = "http://www.kulturystyka.pl/atlas/plecy.asp#13";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);
            exercise = new Exercise(new Guid("777F7E70-2363-4F81-A528-A60AB60086C9"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "PODCIĄGANIE HANTLI (WIOSŁOWANIE) W LEŻENIU NA ŁAWECZCE POZIOMEJ";
            exercise.Shortcut = "PPHLP";
            exercise.Url = "http://www.kulturystyka.pl/atlas/plecy.asp#13";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);
            exercise = new Exercise(new Guid("D9679391-109F-474B-A7F6-9843468240BC"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "SKŁONY ZE SZTANGĄ TRZYMANĄ NA KARKU";
            exercise.Shortcut = "PDD";
            exercise.Url = "http://www.kulturystyka.pl/atlas/plecy.asp#14";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);
            exercise = new Exercise(new Guid("8FCAF1D7-D31A-4D65-9215-EBD5468748F8"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "UNOSZENIE TUŁOWIA Z OPADU";
            exercise.Shortcut = "PW";
            exercise.Url = "http://www.kulturystyka.pl/atlas/plecy.asp#15";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);
            exercise = new Exercise(new Guid("505988E1-5663-41F1-AA1A-9B92EA584263"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "MARTWY CIĄG";
            exercise.Shortcut = "PMC";
            exercise.Url = "http://www.kulturystyka.pl/atlas/plecy.asp#16";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);
            exercise = new Exercise(new Guid("2AADC062-DBDB-4B9F-9182-1E9AC4C1F6F9"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "MARTWY CIĄG NA PROSTYCH NOGACH";
            exercise.Shortcut = "PMCP";
            exercise.Url = "http://www.kulturystyka.pl/atlas/plecy.asp#17";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);
            exercise = new Exercise(new Guid("77EE1230-43A9-4514-8C79-0E4CEDD3FC56"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "WIOSŁOWANIE NA MASZYNIE (LOW ROW)";
            exercise.Shortcut = "PLR";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);
            exercise = new Exercise(new Guid("B47029FA-0CEE-44D3-922E-946C7CBCC92E"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "PODCIĄGANIE HANTLI W OPADZIE STOJĄC (WIOSŁOWANIE)";
            exercise.Shortcut = "PHD";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/bent-over-two-dumbbell-row";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);
            exercise = new Exercise(new Guid("07684561-B086-4E9D-B277-D5CAA7138CDB"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "PODCIĄGANIE SZTANGI NA ŁAWCE SKOŚNEJ";
            exercise.Shortcut = "PSDS";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/incline-bench-pull";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);

            exercise = new Exercise(new Guid("684C538D-0A00-4DF7-930C-83D9218D83FD"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "ŚCIĄGANIE DRĄŻKA NA RĘKACH PROSTYCH";
            exercise.Shortcut = "PWR";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/straight-arm-pulldown";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);
            exercise = new Exercise(new Guid("4115C742-F6A1-477C-A3D2-98B211ACE201"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "ŚCIĄGANIE DRĄŻKA JEDNĄ RĘKĄ";
            exercise.Shortcut = "PWJ";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/straight-arm-pulldown";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);

            exercise = new Exercise(new Guid("D050B87A-CC22-4E36-9ECE-F784E183AA1D"));
            exercise.ExerciseType = ExerciseType.Plecy;
            exercise.Name = "MARTWY CIĄG SUMO";
            exercise.Shortcut = "PMCS";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/sumo-deadlift";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);
        }
        
        private void barki(List<Exercise> list)
        {
            Exercise exercise = new Exercise(new Guid("32C99BF1-1256-4FB2-9379-5325EE68CC9A"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Wyciskanie sztangi sprzed głowy";
            exercise.Shortcut = "SSP";
            exercise.Url = "http://www.kulturystyka.pl/atlas/naramienne.asp#1";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);

            exercise = new Exercise(new Guid("D66E8977-803B-46AD-B608-EACDBA05DDFF"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Wyciskanie do góry na maszynie";
            exercise.Shortcut = "SM";
            exercise.Url = "http://www.kulturystyka.pl/atlas/naramienne.asp#1";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);

            exercise = new Exercise(new Guid("9e6cda99-c456-4d1a-bc67-7d4b43b6713d"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Siłowanie na rękę na wyciągu";
            exercise.Shortcut = "SNR";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);

            exercise = new Exercise(new Guid("AEF35059-4C40-4DE3-9FF3-2DF417A9658B"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Wyciskanie sztangi zza głowy";
            exercise.Shortcut = "SS";
            exercise.Url = "http://www.kulturystyka.pl/atlas/naramienne.asp#2";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);
            
            exercise = new Exercise(new Guid("8FDFEC0A-98DF-4542-8167-CA35D3586370"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Wyciskanie hantli do góry";
            exercise.Shortcut = "SH";
            exercise.Url = "http://www.kulturystyka.pl/atlas/naramienne.asp#3";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);

            exercise = new Exercise(new Guid("D265CA35-24F0-4362-82BA-4607B96560BD"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Arnoldki";
            exercise.Shortcut = "SA";
            exercise.Url = "http://www.kulturystyka.pl/atlas/naramienne.asp#4";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);

            exercise = new Exercise(new Guid("C9219C4B-D3F3-4846-80B4-D1A88C60C236"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Unoszenie hantli bokiem w górę";
            exercise.Shortcut = "SHB";
            exercise.Url = "http://www.kulturystyka.pl/atlas/naramienne.asp#5";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);

            exercise = new Exercise(new Guid("1D2314F0-0BA3-46F8-94FD-E6B876C38C85"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Unoszenie hantli w opadzie tułowia";
            exercise.Shortcut = "SHT";
            exercise.Url = "http://www.kulturystyka.pl/atlas/naramienne.asp#6";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);

            exercise = new Exercise(new Guid("D733D246-54C3-4559-A520-F132EB8956AC"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Podciąganie sztangi wzdłuż tułowia";
            exercise.Shortcut = "SSPT";
            exercise.Url = "http://www.kulturystyka.pl/atlas/naramienne.asp#7";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);

            exercise = new Exercise(new Guid("EF271AE1-BD2D-4B9A-971D-451DE6EAF220"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Podciąganie hantli wzdłuż tułowia";
            exercise.Shortcut = "SHPT";
            exercise.Url = "http://www.kulturystyka.pl/atlas/naramienne.asp#8";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);

            exercise = new Exercise(new Guid("41673F1D-BDC8-49A4-8EDB-0F40CE746648"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Unoszenie ramion w przód ze sztangą";
            exercise.Shortcut = "SUS";
            exercise.Url = "http://www.kulturystyka.pl/atlas/naramienne.asp#9";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);

            exercise = new Exercise(new Guid("4DA1B922-17E6-4645-94D7-AC27DA5F2C30"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Unoszenie ramion w przód z hantlami";
            exercise.Shortcut = "SHP";
            exercise.Url = "http://www.kulturystyka.pl/atlas/naramienne.asp#10";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);


            exercise = new Exercise(new Guid("5E1116CA-528A-42D6-8499-B6BFC75324A7"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Unoszenie ramion ze sztangielkami w leżeniu";
            exercise.Shortcut = "SHLP";
            exercise.Url = "http://www.kulturystyka.pl/atlas/naramienne.asp#11";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);

            exercise = new Exercise(new Guid("16377D68-4F7C-4D8A-8B4D-BA7185FA36C8"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Unoszenie ramion w przód z linkami wyciągu";
            exercise.Shortcut = "SWP";
            exercise.Url = "http://www.kulturystyka.pl/atlas/naramienne.asp#12";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);

            exercise = new Exercise(new Guid("88C6329F-D030-4211-90E4-E0A4B7425642"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Unoszenie ramion bokiem w górę z linkami wyciągu";
            exercise.Shortcut = "SWB";
            exercise.Url = "http://www.kulturystyka.pl/atlas/naramienne.asp#13";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);

            exercise = new Exercise(new Guid("B8C3D936-9D6B-4C78-ACF5-460385D94650"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Unoszenie ramion bokiem w górę w opadzie tułowia z linkami wyciągu";
            exercise.Shortcut = "SWT";
            exercise.Url = "http://www.kulturystyka.pl/atlas/naramienne.asp#14";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);

            exercise = new Exercise(new Guid("FA6FF143-63EC-40C5-9479-AFB712751112"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Odwrotne rozpiętki (na maszynie)";
            exercise.Shortcut = "SRB";
            exercise.Url = "http://www.kulturystyka.pl/atlas/naramienne.asp#15";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);

            exercise = new Exercise(new Guid("5E32F316-73A7-42B4-819E-5CDC737A30EB"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Unoszenie prostych ramion z hantlami ponad glowę";
            exercise.Shortcut = "SHPR";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);

            exercise = new Exercise(new Guid("526664B6-2D74-4F28-95B8-4FEB702E3E46"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Podciąganie wyciągu wzdłuż tułowia";
            exercise.Shortcut = "SW";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/upright-cable-row";
            list.Add(exercise);

            exercise = new Exercise(new Guid("A7437A40-9746-4E68-9812-BEF55E7B9133"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Przyciąganie wyciągu w opadzie (brama)";
            exercise.Shortcut = "STB";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/cable-seated-lateral-raise";
            list.Add(exercise);

            exercise = new Exercise(new Guid("51B0D434-D79F-4FEF-9861-A10D70709920"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Podrzut sztangi";
            exercise.Shortcut = "SPS";
            exercise.Difficult = ExerciseDifficult.Three;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/clean-and-jerk";
            list.Add(exercise);

            exercise = new Exercise(new Guid("ADBF6FFD-0567-420C-A0C9-B2339E55EF4C"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Unoszenie hantli na ławce skośnej";
            exercise.Shortcut = "SHST";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/dumbbell-lying-rear-lateral-raise";
            list.Add(exercise);

            exercise = new Exercise(new Guid("59B46EA0-89B4-4AF3-99BE-09FD546BD120"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Podciąganie hantli bokiem";
            exercise.Shortcut = "SHPB";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/dumbbell-raise";
            list.Add(exercise);

            exercise = new Exercise(new Guid("C81E364B-989A-4FB2-B500-12055D60AB85"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Unoszenie hantli na ławce skośnej przodem";
            exercise.Shortcut = "SHPP";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/front-incline-dumbbell-raise";
            list.Add(exercise);

            exercise = new Exercise(new Guid("8B639BDA-FE3A-45FD-89D3-C30D1746502B"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Pompki w staniu na rękach";
            exercise.Shortcut = "SP";
            exercise.Difficult = ExerciseDifficult.Three;
            exercise.MechanicsType = MechanicsType.Isolation;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/handstand-push-ups";
            list.Add(exercise);

            exercise = new Exercise(new Guid("8640FDF6-7931-4118-82F7-66A864CF71D4"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Iron Cross";
            exercise.Shortcut = "SIC";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/iron-cross";
            list.Add(exercise);

            exercise = new Exercise(new Guid("CDC5244F-95EF-4C79-9709-5233B5A3E981"));
            exercise.ExerciseType = ExerciseType.Barki;
            exercise.Name = "Military Press";
            exercise.Shortcut = "SMP";
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.Difficult = ExerciseDifficult.One;
            exercise.MechanicsType = MechanicsType.Compound;
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/standing-military-press";
            list.Add(exercise);
        }

        private void klatka(List<Exercise> list)
        {
            Exercise exercise = new Exercise(new Guid("ECE5DFD7-F995-45AE-BB34-067F26C4F7B4"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "WYCISKANIE SZTANGI W LEŻENIU NA ŁAWCE POZIOMEJ";
            exercise.Shortcut = "KSP";
            exercise.Url = "http://www.kulturystyka.pl/atlas/klatka.asp#1";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);

            exercise = new Exercise(new Guid("2166400E-F37F-464C-8403-C1566952D6E8"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "WYCISKANIE HANTLI W LEŻENIU NA ŁAWCE POZIOMEJ";
            exercise.Shortcut = "KHP";
            exercise.Url = "http://www.kulturystyka.pl/atlas/klatka.asp#2";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);

            exercise = new Exercise(new Guid("6BB5D744-D7D8-4E69-9537-4B7683FF9829"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "WYCISKANIE HANTLI W LEŻENIU NA ŁAWCE POZIOMEJ z SUPLINACJĄ";
            exercise.Shortcut = "KHPS";
            exercise.Url = "http://www.kulturystyka.pl/atlas/klatka.asp#2";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);

            exercise = new Exercise(new Guid("F0AB1656-B94D-4665-9AC9-F02F100F6E8C"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "WYCISKANIE SZTANGI W LEŻENIU NA ŁAWCE SKOŚNEJ-GŁOWĄ  W GÓRĘ";
            exercise.Shortcut = "KSSG";
            exercise.Url = "http://www.kulturystyka.pl/atlas/klatka.asp#3";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);

            exercise = new Exercise(new Guid("0E9578B5-ADA7-419D-BCC9-6CEB17E22235"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "WYCISKANIE HANTLI W LEŻENIU NA ŁAWCE SKOŚNEJ-GŁOWĄ W GÓRĘ";
            exercise.Shortcut = "KHSG";
            exercise.Url = "http://www.kulturystyka.pl/atlas/klatka.asp#4";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);

            exercise = new Exercise(new Guid("C0FBF54A-CC27-495C-B758-891C1617B70A"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "WYCISKANIE HANTLI W LEŻENIU NA ŁAWCE SKOŚNEJ Z SUPLINACJĄ-GŁOWĄ W GÓRĘ";
            exercise.Shortcut = "KHSGS";
            exercise.Url = "http://www.kulturystyka.pl/atlas/klatka.asp#4";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);

            exercise = new Exercise(new Guid("7282357C-EA8B-4792-A5C6-78034554FAEA"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "WYCISKANIE SZTANGI W LEŻENIU NA ŁAWCE SKOŚNEJ-GŁOWĄ W DÓŁ";
            exercise.Shortcut = "KSSD";
            exercise.Url = "http://www.kulturystyka.pl/atlas/klatka.asp#5";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);

            exercise = new Exercise(new Guid("FEDB8ACD-E737-4AB5-83A1-0E4DAA39F817"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "WYCISKANIE HANTLI W LEŻENIU NA ŁAWCE SKOŚNEJ-GŁOWĄ W DÓŁ";
            exercise.Shortcut = "KHSD";
            exercise.Url = "http://www.kulturystyka.pl/atlas/klatka.asp#6";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);

            exercise = new Exercise(new Guid("F2EFC7FC-ED76-4353-AD05-AD6022D4CFA8"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "WYCISKANIE HANTLI W LEŻENIU NA ŁAWCE SKOŚNEJ Z SUPLINACJĄ-GŁOWĄ W DÓŁ";
            exercise.Shortcut = "KHSDS";
            exercise.Url = "http://www.kulturystyka.pl/atlas/klatka.asp#6";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);

            exercise = new Exercise(new Guid("841DBC08-8F5C-48ED-A57B-F2FD3A0F2098"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "ROZPIĘTKI Z HANTLAMI W LEŻENIU NA ŁAWCE POZIOMEJ";
            exercise.Shortcut = "KR";
            exercise.Url = "http://www.kulturystyka.pl/atlas/klatka.asp#7";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);

            exercise = new Exercise(new Guid("B10166D8-942B-4F06-8F2E-FB5AC8AAF31A"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "ROZPIĘTKI Z HANTLAMI W LEŻENIU NA ŁAWCE SKOŚNEJ-GŁOWĄ DO GÓRY";
            exercise.Shortcut = "KRG";
            exercise.Url = "http://www.kulturystyka.pl/atlas/klatka.asp#8";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);

            exercise = new Exercise(new Guid("08489780-7984-42CF-BD8A-4DBED43CEF23"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "ROZPIĘTKI Z HANTLAMI W LEŻENIU NA ŁAWCE SKOŚNEJ-GŁOWĄ W DÓŁ";
            exercise.Shortcut = "KRD";
            exercise.Url = "http://www.kulturystyka.pl/atlas/klatka.asp#8";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);

            exercise = new Exercise(new Guid("44C88981-F89B-462D-9834-711DBFDFA667"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "PRZENOSZENIE HANTLI W LEŻENIU W POPRZEK ŁAWKI POZIOMEJ";
            exercise.Shortcut = "KHLP";
            exercise.Url = "http://www.kulturystyka.pl/atlas/klatka.asp#10";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);

            exercise = new Exercise(new Guid("943F6E63-AC82-4DA2-AAA6-A1532AF32D97"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "POMPKI NA PORĘCZACH";
            exercise.Shortcut = "KPP";
            exercise.Url = "http://www.kulturystyka.pl/atlas/klatka.asp#11";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);

            exercise = new Exercise(new Guid("9745A89E-09CE-483A-93CA-6BFB60CE308C"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "ROZPIĘTKI W SIADZIE NA MASZYNIE (BUTTERFLY)";
            exercise.Shortcut = "KBF";
            exercise.Url = "http://www.kulturystyka.pl/atlas/klatka.asp#12";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);

            exercise = new Exercise(new Guid("85583670-5D44-445C-8FCD-E4EBE30B0F0C"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "KRZYŻOWANIE LINEK WYCIĄGU W STANIU (BRAMA)";
            exercise.Shortcut = "KB";
            exercise.Url = "http://www.kulturystyka.pl/atlas/klatka.asp#13";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Pull;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);

            exercise = new Exercise(new Guid("2A775A62-B219-40FA-A6B4-0A7285A11E9B"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "WYCISKANIA POZIOME W SIADZIE NA MASZYNIE";
            exercise.Shortcut = "KMP";
            exercise.Url = "http://www.kulturystyka.pl/atlas/klatka.asp#14";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);

            exercise = new Exercise(new Guid("54C53090-D58B-4E52-ABC0-2B26710B95DF"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "PRZENOSZENIE PROSTYCH RAMION WZDŁUŻ TUŁOWIA";
            exercise.Shortcut = "KPT";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/around-the-worlds";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);

            exercise = new Exercise(new Guid("3C80331D-F8D6-4801-81A8-E9F6EC7D2B12"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "ROZPIĘTKI NA ŁAWCE PŁASKIEJ NA WYCIĄGU";
            exercise.Shortcut = "KRW";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/flat-bench-cable-flyes";
            exercise.Difficult = ExerciseDifficult.Two;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Isolation;
            list.Add(exercise);

            exercise = new Exercise(new Guid("7B1982B9-A9EA-4D39-84FF-DB6764B66EC0"));
            exercise.ExerciseType = ExerciseType.Klatka;
            exercise.Name = "POMPKI";
            exercise.Shortcut = "KP";
            exercise.Url = "http://www.bodybuilding.com/exercises/detail/view/name/pushups";
            exercise.Difficult = ExerciseDifficult.One;
            exercise.ExerciseForceType = ExerciseForceType.Push;
            exercise.MechanicsType = MechanicsType.Compound;
            list.Add(exercise);
        }
    }
}
