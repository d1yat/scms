using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;
//using ScmsModel.Core;

namespace ScmsSoaTester
{
  class Program1
  {

    static void Main(string[] args)
    {
      Console.WriteLine("Get all customers in a specific country - regular Linq query");
      var query = Contact.GetContactsList().Where(c => c.Country == "Austria");
      // equivalent to : var query = from c in Contact.GetContactsList() where c.Country == "Austria" select c;
      DumpQuery(query);

      //Console.WriteLine("Get all customers in a specific country - dynamic Linq query");
      //query = Contact.GetContactsList().AsQueryable().Where("Country == @0", "Austria");
      //// equivalent to  query = Contact.GetContactsList().AsQueryable().Where("it.Country == @0", "Austria");
      //DumpQuery(query);

      //Console.WriteLine("Get all customers in a specific country and born in 1957- dynamic Linq query");
      //query = Contact.GetContactsList().AsQueryable().Where("Country == @0 && BirthDate.Year == 1957", "Austria");
      //// equivalent to  query = Contact.GetContactsList().AsQueryable().Where("it.Country == @0", "Austria");
      //DumpQuery(query);

      //Console.WriteLine("Get all customers whose country is in a specific list of countries - regular Linq query");
      //query = Contact.GetContactsList().Where(c => new List<String>() { "Austria", "Poland" }.Contains(c.Country));
      //DumpQuery(query);

      Console.WriteLine("Get all customers whose country is in a specific list of countries - dynamic Linq query");
      query = Contact.GetContactsList().AsQueryable().Where("@0.Contains(outerIt.Country)", new List<String>() { "Austria", "Poland" });
      DumpQuery(query);

      Console.WriteLine("Get all customers whose country is in a specific list of countries, born after 1955 - dynamic Linq query");
      query = Contact.GetContactsList().AsQueryable().Where("@0.Contains(outerIt.Country) && it.BirthDate.Year > @1", new List<String>() { "Austria", "Poland" }, 1955);
      DumpQuery(query);
    }

    static void DumpQuery(IEnumerable query)
    {
      foreach (var contact in query)
      {
        Console.WriteLine(contact.ToString());
      }

      Console.WriteLine("Enter to continue");
      Console.ReadLine();
    }
  }
}
