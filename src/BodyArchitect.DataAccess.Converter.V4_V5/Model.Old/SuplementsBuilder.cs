using System;
using System.Collections.Generic;


namespace BodyArchitect.Model.Old
{
    public class SuplementsBuilder
    {
        #region Data
        public List<Suplement> Create()
        {
            List<Suplement> suplements = new List<Suplement>();


            aminokwasy(suplements);

            kreatyna(suplements);

            inne(suplements);

            odchudzanie(suplements);
            return suplements;
        }

        private void odchudzanie(List<Suplement> suplements)
        {
            var suplement = new Suplement();
            suplement.SuplementId = new Guid("1C056347-996B-444A-98B9-4D00B1AF2663");
            suplement.Name = "CLA";
            suplements.Add(suplement);

            suplement = new Suplement();
            suplement.SuplementId = new Guid("ED696310-1AE1-4E3A-99DA-78B05A21F73A");
            suplement.Name = "karnityna";
            suplements.Add(suplement);

            suplement = new Suplement();
            suplement.SuplementId = new Guid("6924B01A-AE6B-4056-BEDA-7EDAE5F5B1C4");
            suplement.Name = "Spalacz tłuszczu";
            suplements.Add(suplement);
        }

        private void inne(List<Suplement> suplements)
        {
            var suplement = new Suplement();
            suplement.SuplementId = new Guid("3B2BF26F-5310-4148-8E4E-3C0431C0FA75");
            suplement.Name = "Białko";
            suplements.Add(suplement);

            suplement = new Suplement();
            suplement.SuplementId = new Guid("1F052451-42AA-4C8D-8FD8-E099C5237B1A");
            suplement.Name = "Gainer";
            suplements.Add(suplement);

            suplement = new Suplement();
            suplement.SuplementId = new Guid("20F62CA7-B5CC-4765-B17E-B807F72C6234");
            suplement.Name = "Bulk";
            suplements.Add(suplement);

            suplement = new Suplement();
            suplement.SuplementId = new Guid("0BB3367F-509A-4CD2-9BB0-5CE9740A2DF8");
            suplement.Name = "Carbo";
            suplements.Add(suplement);

            suplement = new Suplement();
            suplement.SuplementId = new Guid("677EBCC7-8113-4B81-9061-54D8CC07C7D2");
            suplement.Name = "Kofeina";
            suplements.Add(suplement);

            suplement = new Suplement();
            suplement.SuplementId = new Guid("7281644E-6231-435A-A9F3-2E0D07411578");
            suplement.Name = "Dekstroza";
            suplements.Add(suplement);

            suplement = new Suplement();
            suplement.SuplementId = new Guid("A1D6A2FC-3FC1-4EF8-B0C3-E591E553E8EA");
            suplement.Name = "ZMA";
            suplements.Add(suplement);

            suplement = new Suplement();
            suplement.SuplementId = new Guid("85BECB6E-6469-48FC-8249-3BC283ADEF4E");
            suplement.Name = "Ryboza";
            suplements.Add(suplement);


        }

        private void kreatyna(List<Suplement> suplements)
        {
            var suplement = new Suplement();
            suplement.SuplementId = new Guid("296004CF-4F0D-4FA9-8897-2349F3F411D3");
            suplement.Name = "Kreatyna - monohydrat";
            suplements.Add(suplement);

            suplement = new Suplement();
            suplement.SuplementId = new Guid("094A2EC2-D81C-4B41-B8F5-2A2DEB4BFF2F");
            suplement.Name = "Kreatyna - jabłczan";
            suplements.Add(suplement);

            suplement = new Suplement();
            suplement.SuplementId = new Guid("598A10DA-EE36-4706-8ABC-AA218B0AC142");
            suplement.Name = "Kreatyna - stak";
            suplements.Add(suplement);
        }

        void aminokwasy(List<Suplement> suplements)
        {
            Suplement suplement = new Suplement();
            suplement.SuplementId = new Guid("34A141F4-3448-4552-9AB7-1B2537F7DA1A");
            suplement.Name = "BCAA";
            suplements.Add(suplement);

            suplement = new Suplement();
            suplement.SuplementId = new Guid("6930E4C1-18E9-482E-BAF2-90697F4AE75E");
            suplement.Name = "Leucyna";
            suplements.Add(suplement);

            suplement = new Suplement();
            suplement.SuplementId = new Guid("1EE3FA0F-0835-4462-B8F7-1C036EFD5097");
            suplement.Name = "Glutamina";
            suplements.Add(suplement);

            suplement = new Suplement();
            suplement.SuplementId = new Guid("C5660BD4-77EC-465B-A752-5D808662078E");
            suplement.Name = "HMB";
            suplements.Add(suplement);

            suplement = new Suplement();
            suplement.SuplementId = new Guid("6F1E5C88-4BB2-4E61-BACE-F05BF4442AEB");
            suplement.Name = "L-Arginina";
            suplements.Add(suplement);

            suplement = new Suplement();
            suplement.SuplementId = new Guid("13826022-8F20-4210-BF5E-0AA963CFC194");
            suplement.Name = "Tauryna";
            suplements.Add(suplement);

            suplement = new Suplement();
            suplement.SuplementId = new Guid("E6BB4CF7-60B2-46A4-9E83-252E386C6F2B");
            suplement.Name = "Aminokwasy";
            suplements.Add(suplement);
        }

        #endregion

       
    }
}
