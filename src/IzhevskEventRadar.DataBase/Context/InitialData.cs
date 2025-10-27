using IzhevskEventRadar.DataBase.DataModels;

namespace IzhevskEventRadar.DataBase.Context;

internal class InitialData
{
    private static readonly List<string> _internalIds = new List<string>
    {
        "verbasong",
        "club204542457",
        "solenoe_nebo",
        "hmelion",
        "club93669411",
        "grigger",
        "laskina.voice",
        "club179780208",
        "sunnyjain",
        "club700270",
        "club39964891",
        "waltzing_dogs",
        "jazz18",
        "bardfest_babushkinadacha",
        "bluescrew",
        "mcilchante",
        "snyafrikantza",
        "club77921354",
        "sasha_afrikanets",
        "favorityluny",
        "izhjazzfest",
        "aes_18",
        "udmurtskayatoska",
        "tyloburdo",
        "kareninband",
        "bereg_rock",
        "jahra_reggae",
        "csdr_izh",
        "panamakanal",
        "club123198303",
        "club228777277",
        "ryabchik721",
        "polina_vanez",
        "club224889358",
        "club229240924",
        "club224378859",
        "irisovopole",
        "club224080404",
        "club179780208",
        "udmfil"
    };

    public static IEnumerable<GroupDataModel> InitGroups()
    {
        int id = 1;

        foreach (var internalId in _internalIds)
        {
            yield return new GroupDataModel
            {
                Id = id++,
                InternalId = internalId
            };
        }
    }
}
