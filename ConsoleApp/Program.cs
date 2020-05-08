using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp
{
    class Program
    {
        private static SamuraiContext _context = new SamuraiContext();

        private static void Main(string[] args)
        {
            //AddSamurai();
            //GetSamurais("After Add:");
            //InsertMultipleSamurais();
            //InsertVariousTypes();
            //GetSamuraisSimpler();
            //QueryFilters();
            //RetrieveAndUpdateSamurai();
            //RetrieveAndUpdateMultipleSamurai();
            //MultipleDataBaseoperations();
            //RetrieveAndDeleteASamurai();
            //InsertBattle();
            //QueryAndUpdateBattle_Disconnected();

            //InsertNewSamuraiWithAQuote();
            //InsertNewSamuraiWithManyQuotes();
            //AddQuoteToExistingSamuraiWhileTracked();
            //AddQuoteToExistingSamuraiNotTracked(11);
            //AddQuoteToExistingSamuraiNotTracked_Easy(12);

            //EagerLoadSamuraiwithQuotes();
            //ProjectSomeProperties();
            //ExplicitLoadQuotes();
            //FilteringWithRelatedQuotes();
            //ModifyingRelatedDataWhenTracked();
            //ModifyingRelatedDataWhenNotTracked();
            //JoinBattleAndSamurai();
            //EnlistSamuraiIntoABattle();
            //RemoveJoinBetweenSamuraiAndBattleSimple();
            //GetSamuraiWithBattles();
            //AddNewSamuraiWithHorse();
            //AddNewHorseToSamuraiUsingId();
            //AddNewHorseToSamuraiObject();
            //AddNewHorseToDisconnectedSamuraiObject();
            //ReplaceAHorse();
            //GetSamuraiWithClan();

            QueryUsingRawSql();
            QueryUsingRawSqlWithInterpolation();


            Console.Write("Press any key...");
            Console.ReadKey();
        }

        private static void QueryUsingRawSqlWithInterpolation()
        {
            var name = "Kikuchiko";
            var samurais = _context.Samurais
                .FromSqlInterpolated($"select * from Samurais where Name = {name}")
                .ToArray();
         }

        private static void QueryUsingRawSql()
        {
            var samurais = _context.Samurais
                .FromSqlRaw("select * from Samurais")
                .ToArray();
        }

        private static void GetSamuraiWithClan()
        {
            var sam = _context.Samurais.Include(x => x.Clan).FirstOrDefault();
        }
        
        private static void GetClanSamurai()
        {
            //var clan = _context.Clans.Find(2);
            var clan = _context.Clans.FirstOrDefault();
            //bad practice better use navigation props
            var clansWithSam = _context.Samurais.Where(x => x.Clan.Id == clan.Id);
        }

        private static void GetSamuraisWithHorse()
        {
            var samurai = _context.Samurais.Include(s => s.Horse).ToList();

        }
        private static void GetHorseWithSamurai()
        {
            var horseWithoutSamurai = _context.Set<Horse>().Find(3);

            var horseWithSamurai = _context.Samurais.Include(s => s.Horse)
              .FirstOrDefault(s => s.Horse.Id == 3);

            var horsesWithSamurais = _context.Samurais
              .Where(s => s.Horse != null)
              .Select(s => new { Horse = s.Horse, Samurai = s })
              .ToList();

        }

        private static void ReplaceAHorse()
        {
            //var samurai = _context.Samurais.Include(s => s.Horse).FirstOrDefault(s => s.Id == 23);
            var samurai = _context.Samurais.Find(19); //has a horse
            //norint pakeisti arkli reikia kad chlidas butu memoryje, tada bus
            //istrintas senasis arklys ir pridetas naujas DB
            samurai.Horse = new Horse { Name = "Trigger" };
            _context.SaveChanges();
        }

        private static void AddNewHorseToDisconnectedSamuraiObject()
        {
            var samurai = _context.Samurais.AsNoTracking().FirstOrDefault(s => s.Id == 19);
            samurai.Horse = new Horse { Name = "Mr. Ed" };
            using (var newContext = new SamuraiContext())
            {
                newContext.Attach(samurai);
                newContext.SaveChanges();
            }
        }

        private static void AddNewHorseToSamuraiObject()
        {
            var samurai = _context.Samurais.Find(20);
            samurai.Horse = new Horse { Name = "Black Beauty" };
            _context.SaveChanges();
        }
        private static void AddNewSamuraiWithHorse()
        {
            var samurai = new Samurai { Name = "Jina Ujichika" };
            samurai.Horse = new Horse { Name = "Silver" };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }
        private static void AddNewHorseToSamuraiUsingId()
        {
            var horse = new Horse { Name = "Scout", SamuraiId = 2 };
            _context.Add(horse);
            _context.SaveChanges();
        }
        private static void GetSamuraiWithBattles()
        {
            var samuraiWithBattle = _context.Samurais
              .Include(s => s.SamuraiBattles)
              .ThenInclude(sb => sb.Battle)
              .FirstOrDefault(samurai => samurai.Id == 2);

            var samuraiWithBattlesCleaner = _context.Samurais.Where(s => s.Id == 2)
              .Select(s => new
              {
                  Samurai = s,
                  Battles = s.SamuraiBattles.Select(sb => sb.Battle)
              })
              .FirstOrDefault();
        }
        private static void RemoveJoinBetweenSamuraiAndBattleSimple()
        {
            var join = new SamuraiBattle { BattleId = 1, SamuraiId = 2 };
            _context.Remove(join);
            _context.SaveChanges();
        }
        private static void EnlistSamuraiIntoABattle()
        {
            var battle = _context.Battles.Find(1);
            battle.SamuraiBattles
                .Add(new SamuraiBattle { SamuraiId = 14 });
            _context.SaveChanges();

        }
        private static void JoinBattleAndSamurai()
        {
            //Samurai and Battle already exist and we have their IDs
            var sbJoin = new SamuraiBattle { SamuraiId = 2, BattleId = 1 };
            _context.Add(sbJoin);
            _context.SaveChanges();
        }
        private static void ModifyingRelatedDataWhenNotTracked()
        {
            var samurai = _context.Samurais.Include(s => s.Quotes).FirstOrDefault(s => s.Id == 2);
            var quote = samurai.Quotes[0];
            quote.Text = "Did you hear that again?";
            using (var newContext = new SamuraiContext())
            {
                //newContext.Quotes.Update(quote); //tracks all objects
                newContext.Entry(quote).State = EntityState.Modified; //tracks only quote
                newContext.SaveChanges();
            }
        }
        private static void ModifyingRelatedDataWhenTracked()
        {
            var samurai = _context.Samurais.Include(s => s.Quotes).FirstOrDefault(s => s.Id == 2);
            samurai.Quotes[0].Text = " Did you hear that?";
            _context.Quotes.Remove(samurai.Quotes[2]);
            _context.SaveChanges();
        }
        private static void FilteringWithRelatedQuotes()
        {
            //filtering on quotes in DB side but returns only samurais
            var samurais = _context.Samurais
                .Where(s => s.Quotes.Any(q => q.Text.Contains("happy")))
                .ToArray();
        }
        private static void ExplicitLoadQuotes()
        {
            var samurai = _context.Samurais.FirstOrDefault(x => x.Name.Contains("idas"));
            _context.Entry(samurai).Collection(x => x.Quotes).Load();
            _context.Entry(samurai).Reference(x => x.Horse).Load();
        }
        private static void ProjectSomeProperties()
        {
            //var someProperties = _context.Samurais
            //   .Select(x => new { x.Id, x.Name, HaappyQuotes = x.Quotes.Count })
            //   .ToArray();

            //var someProperties = _context.Samurais
            //    .Select(x => new
            //    {
            //        x.Id,
            //        x.Name,
            //        HaappyQuotes = x.Quotes.Where(x => x.Text.Contains("happy"))
            //    })
            //    .ToArray();

            var samuraisWithHappyQuotes = _context.Samurais
               .Select(x => new
               {
                   Samurai = x,
                   HappyQuotes = x.Quotes.Where(x => x.Text.Contains("happy"))
               }).ToArray();

            var firstSamurai = samuraisWithHappyQuotes[0].Samurai.Name += " The Happiest";
            var a = _context.ChangeTracker.Entries();
        }
        private static void EagerLoadSamuraiwithQuotes()
        {
            var samuraiWithQuotes = _context.Samurais.Include(x => x.Quotes).ToArray();

            var samuraisWithQuotes = _context.Samurais
                .Where(x => x.Name.Contains("idas"))
                .Include(x => x.Quotes).ToArray()
                .ToArray();
        }
        private static void AddQuoteToExistingSamuraiNotTracked_Easy(int samuraiId)
        {
            var quote = new Quote
            {
                Text = "Now that I saved you, will you feed me dinner again?",
                SamuraiId = samuraiId
            };
            using (var newContext = new SamuraiContext())
            {
                newContext.Quotes.Add(quote);
                newContext.SaveChanges();
            }
        }
        private static void AddQuoteToExistingSamuraiNotTracked(int samuraiId)
        {
            var samurai = _context.Samurais.Find(samuraiId);
            samurai.Quotes.Add(new Quote
            {
                Text = "Now that I saved you, will you feed me dinner?"
            });
            using (var newContext = new SamuraiContext())
            {
                newContext.Samurais.Attach(samurai);
                newContext.SaveChanges();
            }
        }
        private static void AddQuoteToExistingSamuraiWhileTracked()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Quotes.Add(new Quote
            {
                Text = "I bet you're happy that I've saved you!"
            });
            _context.SaveChanges();
        }
        private static void InsertNewSamuraiWithManyQuotes()
        {
            var samurai = new Samurai
            {
                Name = "Kyūzō",
                Quotes = new List<Quote> {
                 new Quote {Text = "Watch out for my sharp sword!"},
                 new Quote {Text="I told you to watch out for the sharp sword! Oh well!" }
               }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }
        private static void InsertNewSamuraiWithAQuote()
        {
            var samurai = new Samurai
            {
                Name = "Kambei Shimada",
                Quotes = new List<Quote>{
                    new Quote { Text = "I've come to save you" }
                }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }


        //private static void QueryAndUpdateBattle_Disconnected()
        //{
        //    var battle = _context.Battles.AsNoTracking().FirstOrDefault();

        //    battle.EndDate = new DateTime(1560, 6, 30);
        //    using (var newContext = new SamuraiContextNoTracking())
        //    {
        //        newContext.Battles.Update(battle);
        //        newContext.SaveChanges();
        //    }
        //}
        private static void InsertBattle()
        {
            _context.Add(new Battle
            {
                Name = "Battle of Okehazama",
                StartDate = new DateTime(1560, 05, 01),
                EndDate = new DateTime(1560, 06, 15),
            });
            _context.SaveChanges();
        }
        private static void RetrieveAndDeleteASamurai()
        {
            var samurai = _context.Samurais.Find(7);
            _context.Remove(samurai);
            _context.SaveChanges();
        }
        private static void MultipleDataBaseoperations()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Name += "San";
            _context.Add(new Samurai { Name = "Kikuchiko" });
            _context.SaveChanges();
        }
        private static void RetrieveAndUpdateMultipleSamurai()
        {
            var samurai = _context.Samurais.Skip(1).Take(4).ToList();
            samurai.ForEach(s => s.Name += "San");
            _context.SaveChanges();
        }
        private static void RetrieveAndUpdateSamurai()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            samurai.Name += "San";
            _context.SaveChanges();
        }
        private static void QueryFilters()
        {
            var name = "Sampson";
            //var samurais = _context.Samurais.Where(s => s.Name == name).ToArray();
            //var samurai = _context.Samurais.Where(s => s.Name == name).FirstOrDefault();
            //var samurai = _context.Samurais.Find(2); //tik pagal PK jei buvo objectas trackinamas nereikes eiti i db

            // lastOrDefault() veik tik jei pries tai buvo orderinta kitu atveju exception
            var samurai = _context.Samurais.OrderBy(s => s.Id).LastOrDefault(s => s.Name == name);



            //works similarly all will be converted to sql
            //var samurais = _context.Samurais.Where(s => EF.Functions.Like(s.Name, "A%")).ToArray();
            //var samurais1 = _context.Samurais.Where(s => s.Name.StartsWith("A")).ToArray();
            //var samurais2 = _context.Samurais.Where(s => s.Name.Contains("A")).ToArray(); 
        }
        private static void GetSamuraisSimpler()
        {
            var query = _context.Samurais;
            //var samurais = query.ToList(); //query execution     

            foreach (var samurai in query)  //by enumeraiting query execution happends
            {
                Console.WriteLine(samurai.Name);
            } //connection stays open until end of loop bad practice
        }
        private static void InsertVariousTypes()
        {
            var samurai1 = new Samurai { Name = "Sampson" };
            var clan = new Clan() { ClanName = "Imperial clan" };
            _context.AddRange(samurai1, clan);
            _context.SaveChanges();
        }
        private static void InsertMultipleSamurais()
        {
            var samurai1 = new Samurai { Name = "Sampson" };
            var samurai2 = new Samurai { Name = "Tasha" };
            var samurai3 = new Samurai { Name = "samurai3" };
            var samurai4 = new Samurai { Name = "samurai4" };
            _context.Samurais.AddRange(samurai1, samurai2, samurai3, samurai4);
            _context.SaveChanges();
        }
        private static void AddSamurai()
        {
            var samurai = new Samurai { Name = "Aidas" };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }
        private static void GetSamurais(string text)
        {
            var samurais = _context.Samurais.ToList();
            Console.WriteLine($"{text}: Samurai count is {samurais.Count}");
            foreach (var samurai in samurais)
            {
                Console.WriteLine(samurai.Name);
            }
        }
    }
}
