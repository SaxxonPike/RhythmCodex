namespace RhythmCodex.Stepmania.Model
{
    public static class SmGameTypes
    {
        public const string Single = "single";
        public const string Double = "double";
        public const string Couple = "couple";
        public const string Solo = "solo";
        public const string ThreePanel = "threepanel";
        public const string Real = "real";

        public const string Dance = "dance";
        public const string Pump = "pump";
        public const string Ez2 = "ez2";
        public const string Para = "para";
        
        public const string DanceSingle = Dance + "-" + Single;
        public const string DanceDouble = Dance + "-" + Double;
        public const string DanceCouple = Dance + "-" + Couple;
        public const string DanceSolo = Dance + "-" + Solo;
        public const string PumpSingle = Pump + "-" + Single;
        public const string Ez2Single = Ez2 + "-" + Single;
        public const string Ez2Double = Ez2 + "-" + Double;
        public const string ParaSingle = Para + "-" + Single;
        public const string PumpDouble = Pump + "-" + Double;
        public const string PumpCouple = Pump + "-" + Couple;
        public const string Ez2Real = Ez2 + "-" + Real;
    }
}