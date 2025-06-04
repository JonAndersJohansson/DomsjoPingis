using DataAccessLayer.Data.Models;

namespace Service.Infrastructure.Translator
{
    public static class GenderTranslator
    {
        public static string Translate(Gender gender)
        {
            return gender switch
            {
                Gender.Male => "Man",
                Gender.Female => "Kvinna",
                Gender.Other => "Annat",
                Gender.Unspecified => "Ej angivet",
                _ => gender.ToString()
            };
        }
    }
}
