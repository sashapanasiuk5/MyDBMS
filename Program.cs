// See https://aka.ms/new-console-template for more information

using System.Runtime.Serialization.Formatters.Binary;
using DataBase_BTree;
//Database db = new Database("test.data");
Database db = Database.Create("test.data");


db.AddRecord(5,485);

db.AddRecord(3,3441);


db.AddRecord(20,7222);
db.AddRecord(21,4445);
/*
db.AddRecord(22,88);
db.AddRecord(23,73);
db.AddRecord(24,61);
db.AddRecord(25,74);

db.AddRecord(26,14);
db.AddRecord(27,17);
db.AddRecord(12,59);
db.AddRecord(15,121);
db.AddRecord(17,457);*/
db.AddRecord(19,473);

db.AddRecord(11,111);
db.AddRecord(14,101);
db.AddRecord(16,777);
db.AddRecord(4,234);
db.AddRecord(2,345);
db.AddRecord(1,223);
db.AddRecord(18,7875);

db.AddRecord(9,78);
db.AddRecord(7,45);
db.AddRecord(10,457);
db.AddRecord(6,437);



db.Print();
Console.WriteLine("------------------------------");
db.Delete(21);
db.Delete(19);
db.Print();
string input;/*
input = Console.ReadLine();
while (input != "q")
{
    int key = Int32.Parse(input);
    Record record;
    
    record = db.Find(key);
    Console.WriteLine("Key: "+record.Key +" Value: "+record.Value);
    
input = Console.ReadLine();
}
*/


db.Close();
