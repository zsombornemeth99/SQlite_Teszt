using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LekvarDB
{
    class Program
    {
        static void Main(string[] args)
        {
            double meret = 0;
            string tipus = "";
            var conn = new SQLiteConnection("Data Source= mydb.db");
            conn.Open();

            var createComm = conn.CreateCommand();

            createComm.CommandText = @"
                CREATE TABLE IF NOT EXISTS lekvar(
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                meret INTEGER(10) NOT NULL,
                tipus VARCHAR(10) NOT NULL);";
            createComm.ExecuteNonQuery();

            do
            {
                meret = 0;
                tipus = "";
                Console.Write("Kérem adja meg az üveg méretét(literben): ");
                double.TryParse(Console.ReadLine(), out meret);
                if (meret != 0)
                {
                    Console.Write("Kérem adja meg a lekvár típusát: ");
                    tipus = Console.ReadLine();
                    var insertComm = conn.CreateCommand();

                    insertComm.CommandText = @"
                    INSERT INTO lekvar (meret, tipus)
                    VALUES (@meret,@tipus)";

                    insertComm.Parameters.AddWithValue("@meret", meret);
                    insertComm.Parameters.AddWithValue("@tipus", tipus);

                    insertComm.ExecuteNonQuery();
                    Console.Clear();
                }
                else
                {
                    Console.WriteLine("Adatbevitel vége. Nyomjon egy ENTER-t a továbblépéshez");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
            while (tipus != "" || meret != 0);

            //Mindent kilistáz
            //az ID is működik

            var selectComm = conn.CreateCommand();
            selectComm.CommandText = @"SELECT * FROM lekvar";

            var reader = selectComm.ExecuteReader();
            while (reader.Read())
            {
                var eid = reader.GetInt32(0);
                var emeret = reader.GetDouble(1);
                var etipus = reader.GetString(2);
                Console.WriteLine($"{eid} - {emeret} - {etipus}");
            }
            
            var osszLekvarComm = conn.CreateCommand();
            osszLekvarComm.CommandText = @"SELECT SUM(meret) FROM lekvar";

            //Összes lekvár mennyisége

            var reader2 = osszLekvarComm.ExecuteReader();
            while (reader2.Read())
            {
                var eosszMeret = reader2.GetDouble(0);
                Console.WriteLine($"Összes lekvár mennyisége: {eosszMeret} L");
            }

            //Fajtánként a mennyiség

            var osszLekvarFajtankentComm = conn.CreateCommand();
            osszLekvarFajtankentComm.CommandText = @"SELECT tipus, SUM(meret) FROM lekvar GROUP BY tipus";

            var reader3 = osszLekvarFajtankentComm.ExecuteReader();
            while (reader3.Read())
            {
                var eFajta = reader3.GetString(0);
                var eosszMeretFajtanlent = reader3.GetDouble(1);
                Console.WriteLine($"{eFajta,-10}: {eosszMeretFajtanlent} L");
            }

            //Átlagos üvegméret

            var avgUvegMeret = conn.CreateCommand();
            avgUvegMeret.CommandText = @"SELECT AVG(meret) FROM lekvar";

            var reader4 = avgUvegMeret.ExecuteReader();
            while (reader4.Read())
            {
                var eAvgMeret = reader4.GetDouble(0);
                Console.WriteLine($"Átlagos üveg méret: {eAvgMeret} L");
            }

            Console.ReadKey();
        }
    }
}
