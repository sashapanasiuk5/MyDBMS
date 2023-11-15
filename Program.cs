// See https://aka.ms/new-console-template for more information

using System.Runtime.Serialization.Formatters.Binary;
using DataBase_BTree;
using DataBase_BTree.InputOutputStrategies;

//Database db = new Database("test.data");
Database db = Database.Create("test.data");

Dictionary<string, IDataType> template = new Dictionary<string, IDataType>();
template.Add("ID", new IntegerType());
template.Add("Price", new IntegerType());
template.Add("Name", new VarcharType(10));
db.CreateTable(template, 1);

Type myType = typeof(string);
SortedSet<>

Dictionary<string, object> values = new Dictionary<string, object>();
/*int price, code, id;
for (int i = 0; i < 6; i++)
{
    Console.WriteLine("ID: ");
    id = Int32.Parse(Console.ReadLine());
    Console.WriteLine("Price: ");
    price = Int32.Parse(Console.ReadLine());
    Console.WriteLine("Code: ");
    code = Int32.Parse(Console.ReadLine());
    values.Add("ID", id);
    values.Add("Price", price);
    values.Add("Code", code);
    db.InsertIntoTable(values);
    values.Clear();
}*/

values.Add("ID", 5);
values.Add("Price", 120);
values.Add("Name", "Product1");
db.InsertIntoTable(values);
values.Clear();

values.Add("ID", 9);
values.Add("Price", 45);
values.Add("Name", "Product2");
db.InsertIntoTable(values);
values.Clear();

values.Add("ID", 4);
values.Add("Price", 118);
values.Add("Name", "Product3");
db.InsertIntoTable(values);
values.Clear();


values.Add("ID", 2);
values.Add("Price", 140);
values.Add("Name", "Product4");
db.InsertIntoTable(values);
values.Clear();

values.Add("ID", 1);
values.Add("Price", 99);
values.Add("Name", "Product5");
db.InsertIntoTable(values);
values.Clear();


values.Add("ID", 6);
values.Add("Price", 180);
values.Add("Name", "Product6");
db.InsertIntoTable(values);
values.Clear();
/*

values.Add("ID", 8);
values.Add("Price", 175);
values.Add("Code", 2);
db.InsertIntoTable(values);
values.Clear();


values.Add("ID", 7);
values.Add("Price", 20);
values.Add("Code", 0);
db.InsertIntoTable(values);
values.Clear();


values.Add("ID", 10);
values.Add("Price", 452);
values.Add("Code", 3);
db.InsertIntoTable(values);
values.Clear();


values.Add("ID", 11);
values.Add("Price", 155);
values.Add("Code", 2);
db.InsertIntoTable(values);
values.Clear();

values.Add("ID", 12);
values.Add("Price", 700);
values.Add("Code", 3);
db.InsertIntoTable(values);
values.Clear();


values.Add("ID", 16);
values.Add("Price", 25);
values.Add("Code", 0);
db.InsertIntoTable(values);
values.Clear();

values.Add("ID", 20);
values.Add("Price", 199);
values.Add("Code", 2);
db.InsertIntoTable(values);
values.Clear();

values.Add("ID", 21);
values.Add("Price", 1788);
values.Add("Code", 3);
db.InsertIntoTable(values);
values.Clear();

values.Add("ID", 18);
values.Add("Price", 400);
values.Add("Code", 3);
db.InsertIntoTable(values);
values.Clear();

values.Add("ID", 17);
values.Add("Price", 172);
values.Add("Code", 2);
db.InsertIntoTable(values);
values.Clear();

values.Add("ID", 19);
values.Add("Price", 277);
values.Add("Code", 2);
db.InsertIntoTable(values);
values.Clear();

values.Add("ID", 25);
values.Add("Price", 101);
values.Add("Code", 1);
db.InsertIntoTable(values);
values.Clear();

values.Add("ID", 23);
values.Add("Price", 20);
values.Add("Code", 0);
db.InsertIntoTable(values);
values.Clear();

values.Add("ID", 30);
values.Add("Price", 74);
values.Add("Code", 0);
db.InsertIntoTable(values);
values.Clear();

values.Add("ID", 28);
values.Add("Price", 255);
values.Add("Code", 2);
db.InsertIntoTable(values);
values.Clear();

values.Add("ID", 27);
values.Add("Price", 147);
values.Add("Code", 1);
db.InsertIntoTable(values);
values.Clear();
*/

db.PrintTable();