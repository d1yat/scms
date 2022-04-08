using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScmsSoaTester
{
  public class Contact
  {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public string Country { get; set; }

    public Contact(string firstName, string lastName, DateTime birthDate, string country)
    {
      FirstName = firstName;
      LastName = lastName;
      BirthDate = birthDate;
      Country = country;
    }

    public override string ToString()
    {
      return string.Concat(FirstName, " ", LastName, " ", BirthDate, " ", Country);
    }

    public static List<Contact> GetContactsList()
    {
      var result = new List<Contact>();

      result.Add(new Contact("Zephr", "Austin", new DateTime(1967, 11, 07), "Afghanistan"));
      result.Add(new Contact("Odette", "Bean", new DateTime(1993, 05, 18), "Uzbekistan"));
      result.Add(new Contact("Maggie", "Mcpherson", new DateTime(2001, 06, 12), "Kiribati"));
      result.Add(new Contact("Aileen", "Walton", new DateTime(1992, 06, 14), "Cameroon"));
      result.Add(new Contact("Mary", "Keith", new DateTime(1980, 10, 12), "Palau"));
      result.Add(new Contact("Kylee", "Rhodes", new DateTime(2002, 06, 13), "Gibraltar"));
      result.Add(new Contact("Harding", "Abbott", new DateTime(2002, 11, 25), "Eritrea"));
      result.Add(new Contact("Wesley", "Guerrero", new DateTime(1980, 01, 17), "Kuwait"));
      result.Add(new Contact("Colin", "Kinney", new DateTime(1999, 10, 19), "Dominican Republic"));
      result.Add(new Contact("Madeline", "Robles", new DateTime(1961, 08, 13), "Chad"));
      result.Add(new Contact("Owen", "Robbins", new DateTime(1964, 09, 13), "Comoros"));
      result.Add(new Contact("Palmer", "Kim", new DateTime(1993, 09, 14), "Ecuador"));
      result.Add(new Contact("Nathan", "Butler", new DateTime(1991, 04, 08), "Syrian Arab Republic"));
      result.Add(new Contact("Camden", "Cardenas", new DateTime(1956, 10, 28), "Slovakia"));
      result.Add(new Contact("Erin", "Rosales", new DateTime(2003, 07, 19), "Finland"));
      result.Add(new Contact("Yuli", "Gordon", new DateTime(1952, 06, 16), "Austria"));
      result.Add(new Contact("Fiona", "Powers", new DateTime(1957, 11, 25), "Austria"));
      result.Add(new Contact("Leroy", "Oneill", new DateTime(2002, 08, 17), "Ireland"));
      result.Add(new Contact("Arden", "Torres", new DateTime(2002, 11, 03), "Ukraine"));
      result.Add(new Contact("Evelyn", "Lawson", new DateTime(2000, 10, 13), "Poland"));
      result.Add(new Contact("Joel", "Eaton", new DateTime(1980, 07, 18), "Lesotho"));
      result.Add(new Contact("Myles", "Acevedo", new DateTime(1956, 08, 09), "Yemen"));
      result.Add(new Contact("Vincent", "Owen", new DateTime(1997, 01, 02), "Botswana"));
      result.Add(new Contact("Ria", "Simon", new DateTime(1956, 09, 01), "Egypt"));
      result.Add(new Contact("Louis", "Carver", new DateTime(1978, 12, 21), "Aruba"));
      result.Add(new Contact("Taylor", "Callahan", new DateTime(1984, 08, 10), "Iceland"));
      result.Add(new Contact("Wing", "Hughes", new DateTime(1964, 04, 27), "Burkina Faso"));
      result.Add(new Contact("Aiko", "Barton", new DateTime(1982, 08, 20), "Sri Lanka"));
      result.Add(new Contact("Isabelle", "Bruce", new DateTime(1994, 03, 10), "Russian Federation"));
      result.Add(new Contact("Hadley", "Hewitt", new DateTime(1994, 08, 31), "El Salvador"));
      result.Add(new Contact("Avram", "Travis", new DateTime(1962, 07, 16), "Macedonia"));
      result.Add(new Contact("Lance", "Mcdowell", new DateTime(1987, 07, 11), "Madagascar"));
      result.Add(new Contact("Kyla", "Leonard", new DateTime(2001, 11, 08), "Croatia"));
      result.Add(new Contact("Minerva", "Gentry", new DateTime(1980, 09, 14), "Azerbaijan"));
      result.Add(new Contact("Velma", "Walters", new DateTime(1985, 10, 18), "Costa Rica"));
      result.Add(new Contact("Yuri", "Sherman", new DateTime(1971, 03, 12), "Grenada"));
      result.Add(new Contact("Hayden", "Roy", new DateTime(1960, 03, 07), "Slovenia"));
      result.Add(new Contact("Keiko", "Santiago", new DateTime(1964, 04, 11), "Sweden"));
      result.Add(new Contact("Grant", "Dunn", new DateTime(2007, 11, 29), "Vanuatu"));
      result.Add(new Contact("Belle", "Mitchell", new DateTime(2006, 01, 22), "Haiti"));
      result.Add(new Contact("Stuart", "Ferrell", new DateTime(2000, 02, 22), "Thailand"));
      result.Add(new Contact("Henry", "Short", new DateTime(1972, 05, 10), "India"));
      result.Add(new Contact("Lynn", "Lane", new DateTime(1970, 02, 12), "Rwanda"));
      result.Add(new Contact("Tanya", "Holland", new DateTime(1982, 08, 26), "Madagascar"));
      result.Add(new Contact("Darrel", "Sawyer", new DateTime(2004, 12, 13), "Georgia"));
      result.Add(new Contact("Amy", "Goodwin", new DateTime(1986, 11, 14), "Bulgaria"));
      result.Add(new Contact("Teegan", "Pennington", new DateTime(1987, 07, 30), "Hungary"));
      result.Add(new Contact("Sonia", "Sandoval", new DateTime(2002, 05, 27), "Anguilla"));
      result.Add(new Contact("Darryl", "Chavez", new DateTime(1986, 09, 12), "Rwanda"));
      result.Add(new Contact("Keiko", "Bartlett", new DateTime(1991, 08, 28), "Angola"));
      result.Add(new Contact("Kristen", "Powers", new DateTime(1967, 06, 14), "Sudan"));
      result.Add(new Contact("Danielle", "Molina", new DateTime(1978, 06, 20), "Eritrea"));
      result.Add(new Contact("Price", "Stevenson", new DateTime(1978, 07, 11), "Belize"));
      result.Add(new Contact("Ayanna", "Velasquez", new DateTime(1978, 12, 18), "Japan"));
      result.Add(new Contact("Cruz", "Sellers", new DateTime(2006, 07, 24), "Canada"));
      result.Add(new Contact("Merritt", "Dudley", new DateTime(1986, 06, 23), "Ireland"));
      result.Add(new Contact("Justina", "Kelly", new DateTime(2004, 05, 26), "Austria"));
      result.Add(new Contact("Skyler", "Gill", new DateTime(1992, 01, 24), "Switzerland"));
      result.Add(new Contact("Iona", "Shepard", new DateTime(2007, 01, 12), "Georgia"));
      result.Add(new Contact("Quemby", "Morse", new DateTime(1973, 12, 12), "Palau"));
      result.Add(new Contact("Kameko", "Brooks", new DateTime(1996, 01, 31), "Botswana"));
      result.Add(new Contact("Lamar", "Stokes", new DateTime(1988, 09, 26), "Guyana"));
      result.Add(new Contact("Justin", "Preston", new DateTime(1975, 12, 25), "Portugal"));
      result.Add(new Contact("Grant", "Bartlett", new DateTime(1985, 12, 16), "Ethiopia"));
      result.Add(new Contact("Anastasia", "Knox", new DateTime(1999, 10, 10), "Korea"));
      result.Add(new Contact("Portia", "Leach", new DateTime(1953, 05, 28), "Malaysia"));
      result.Add(new Contact("Kirk", "Fitzpatrick", new DateTime(1957, 01, 14), "Chile"));
      result.Add(new Contact("Linda", "Riley", new DateTime(1968, 11, 19), "Portugal"));
      result.Add(new Contact("Gemma", "Small", new DateTime(1987, 06, 10), "Peru"));
      result.Add(new Contact("Ocean", "Graham", new DateTime(1988, 12, 11), "Italy"));
      result.Add(new Contact("India", "Perry", new DateTime(1981, 02, 02), "Latvia"));
      result.Add(new Contact("Nissim", "Riley", new DateTime(2005, 11, 23), "Yemen"));
      result.Add(new Contact("Jena", "Whitney", new DateTime(1952, 09, 23), "Korea"));
      result.Add(new Contact("Charles", "Randolph", new DateTime(1991, 05, 17), "Japan"));
      result.Add(new Contact("Emi", "Goodman", new DateTime(1955, 04, 27), "Croatia"));
      result.Add(new Contact("Samson", "Velazquez", new DateTime(1964, 02, 17), "Guadeloupe"));
      result.Add(new Contact("Kay", "Morrison", new DateTime(1962, 11, 08), "Poland"));
      result.Add(new Contact("Winifred", "Blanchard", new DateTime(1983, 07, 25), "Israel"));

      return result;
    }
  }
}
